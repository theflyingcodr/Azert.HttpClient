using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azert.HttpClient.Exceptions;
using Azert.HttpClient.Services;
using Azert.HttpClient.Tests.Mocks;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Azert.HttpClient.Tests.Services
{
    public class HttpCachingServiceTests
    {
        private IFixture _fixture;
        private HttpCachingService _sut;

        private Dictionary<string, RandomObject> _cache;
        private string _baseAddress;
        private string _uri;
        private const string headerKey = "x-resource-identifier";

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture()
                  .Customize(new AutoMoqCustomization())
                  .Customize(new MultipleCustomization());

            _cache = new Dictionary<string, RandomObject>();
            _baseAddress = _fixture.Create<string>();
            _uri = _fixture.Create<string>();

            _sut = _fixture.Create<HttpCachingService>();
        }

        #region  CheckCache

        [Test]
        public async void CheckCache_cacheCheck_not_null_should_invoke()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, string.Empty), obj);

            // act
            var response = await _sut.CheckCache(_baseAddress, _uri, null, CacheCheck);

            // assert
            response.PointlessProperty.Should().Be(obj.PointlessProperty);

        }

        [Test]
        public async void CheckCache_cacheCheck_func_null_should_return_null()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, string.Empty), obj);

            // act
            var response = await _sut.CheckCache<RandomObject>(_baseAddress, _uri);

            // assert
            response.Should().BeNull();

        }

        [Test]
        public async void CheckCache_cacheCheck_object_not_found_should_return_null()
        {
            // arrange
            _cache.Clear();

            // act
            var response = await _sut.CheckCache(_baseAddress, _uri, null, CacheCheck);

            // assert
            response.Should().BeNull();
        }

        [Test]
        public async void CacheCheck_request_with_header_set_should_return_response()
        {
            var obj = _fixture.Create<RandomObject>();
            var identifier = _fixture.Create<string>();
            var header = new Dictionary<string, string>
                         {
                             {headerKey, identifier}
                         };

            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, identifier), obj);

            // act
            var response = await _sut.CheckCache(_baseAddress, _uri, header, CacheCheck);

            // assert
            response.PointlessProperty.Should().Be(obj.PointlessProperty);
        }

        [Test, ExpectedException(typeof(MissingHeaderException))]
        public async void CacheCheck_request_with_header_missing_should_throw_exception()
        {
            var obj = _fixture.Create<RandomObject>();
            var identifier = _fixture.Create<string>();
            var header = new Dictionary<string, string>
                         {
                             {_fixture.Create<string>(),identifier }
                         };

            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, identifier), obj);

            // act
            await _sut.CheckCache(_baseAddress, _uri, header, CacheCheck);
        }

        #endregion

        #region AddToCache

        [Test]
        public async void AddToCache_adding_item_should_be_found_in_cache()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();
            _cache.Count.Should().Be(0);

            // act
            await _sut.AddToCache(obj, _baseAddress, _uri, null, SetCache);

            // assert
            _cache.Count.Should().Be(1);
            _cache.ElementAt(0).Value.Should().BeSameAs(obj);
        }

        [Test]
        public async void AddToCache_SetCache_not_defined_should_not_add_item()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();
            _cache.Count.Should().Be(0);

            // act
            await _sut.AddToCache(obj, _baseAddress, _uri);

            // assert
            _cache.Count.Should().Be(0);
        }

        [Test]
        public async void AddToCache_adding_item_with_header_should_be_found_in_cache()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();
            _cache.Count.Should().Be(0);
            var identifier = _fixture.Create<string>();
            var header = new Dictionary<string, string>
                         {
                             {headerKey, identifier}
                         };

            // act
            await _sut.AddToCache(obj, _baseAddress, _uri, header, SetCache);

            // assert
            _cache.Count.Should().Be(1);
            _cache.ElementAt(0).Value.Should().BeSameAs(obj);
        }

        [Test, ExpectedException(typeof(MissingHeaderException))]
        public async void AddToCache_adding_item_with_header_missing_should_throw_exception()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();
            _cache.Count.Should().Be(0);
            var identifier = _fixture.Create<string>();
            var header = new Dictionary<string, string>
                         {
                             {_fixture.Create<string>(), identifier}
                         };

            // act
            await _sut.AddToCache(obj, _baseAddress, _uri, header, SetCache);
        }

        #endregion

        #region VoidCache

        [Test]
        public async void VoidCache_should_remove_item()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();
            _cache.Count.Should().Be(0);
            var identifier = _fixture.Create<string>();
            var header = new Dictionary<string, string>
                         {
                             {headerKey, identifier}
                         };

            // act
            await _sut.VoidCache(_baseAddress, _uri, VoidCache);

            // assert
            _cache.Count.Should().Be(0);
        }

        #endregion

        private async Task<RandomObject> CacheCheck(string key)
        {
            var item = _cache.SingleOrDefault(x => x.Key == key);

            return item.Equals(default(KeyValuePair<string, RandomObject>)) ? null : item.Value;
        }

        private async Task SetCache(RandomObject obj, string key)
        {
            _cache.Add(key, obj);
        }

        private async Task VoidCache(string key)
        {
            _cache.Remove(key);
        }

    }
}
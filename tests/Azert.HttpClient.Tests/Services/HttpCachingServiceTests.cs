using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, obj), obj);

            // act
            var response = await _sut.CheckCache(obj, _baseAddress, _uri, CacheCheck);

            // assert
            response.PointlessProperty.Should().Be(obj.PointlessProperty);

        }

        [Test]
        public async void CheckCache_cacheCheck_func_null_should_return_null()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Add(_sut.CreateCacheKey(_baseAddress, _uri, obj), obj);

            // act
            var response = await _sut.CheckCache<RandomObject, RandomObject>(obj, _baseAddress, _uri);

            // assert
            response.Should().BeNull();

        }

        [Test]
        public async void CheckCache_cacheCheck_object_not_found_should_return_null()
        {
            // arrange
            var obj = _fixture.Create<RandomObject>();
            _cache.Clear();

            // act
            var response = await _sut.CheckCache<RandomObject, RandomObject>(obj, _baseAddress, _uri, CacheCheck);

            // assert
            response.Should().BeNull();
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
            await _sut.AddToCache(obj, obj, _baseAddress, _uri, SetCache);

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
            await _sut.AddToCache(obj, obj, _baseAddress, _uri);

            // assert
            _cache.Count.Should().Be(0);
        }

        #endregion

        #region VoidCache

        public async void VoidCache_should_remove_item()
        {
            // arrange

            // act
            //await _sut.VoidCache(_baseAddress, _uri,)

            // assert
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

    }
}
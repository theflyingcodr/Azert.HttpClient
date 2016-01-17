using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azert.HttpClient.Clients;
using Azert.HttpClient.Services.Interfaces;
using Azert.HttpClient.Tests.Mocks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace Azert.HttpClient.Tests.Clients
{
    [TestFixture]
    public class HttpAsyncClientTests
    {
        private IFixture _fixture;
        private Mock<IHttpService> _httpService;
        private Mock<IHttpCachingService> _httpCachingService;

        private HttpAsyncClient _sut;

        private string _baseAddress;
        private string _uri;
        private Dictionary<string, string> _headers;
        private RandomObject _cachedObject;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture()
                  .Customize(new AutoMoqCustomization())
                  .Customize(new MultipleCustomization());

            _httpService = _fixture.Freeze<Mock<IHttpService>>();
            _httpCachingService = _fixture.Freeze<Mock<IHttpCachingService>>();

            _baseAddress = _fixture.Create<string>();
            _uri = _fixture.Create<string>();
            _headers = _fixture.Create<Dictionary<string, string>>();

            _sut = _fixture.Create<HttpAsyncClient>();
        }

        #region Get

        [Test]
        public async void Get_valid_request_returns_response_not_cached()
        {
            // arrange

            var responseObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(x => x.CheckCache<object, RandomObject>(null, _baseAddress, _uri, null))
                               .Returns(Task.FromResult((RandomObject)null));

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, object>(_baseAddress, _uri, null, _headers, HttpMethods.GET))
                        .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Get<RandomObject>(_baseAddress, _uri, _headers);

            // assert
            response.Should().BeOfType<RandomObject>();
            response.PointlessProperty.Should().Be(responseObj.PointlessProperty);
            response.BackInTime.Should().Be(responseObj.BackInTime);

        }

        [Test]
        public async void Get_valid_request_returns_response_not_cached_should_add_to_cache()
        {
            // arrange

            var responseObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(x => x.CheckCache<object, RandomObject>(null, _baseAddress, _uri, null))
                               .Returns(Task.FromResult((RandomObject)null));

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, object>(_baseAddress, _uri, null, _headers, HttpMethods.GET))
                        .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Get<RandomObject>(_baseAddress, _uri, _headers);

            // assert
            _httpCachingService.Verify(
                                       x =>
                                       x.AddToCache<RandomObject, object>(It.IsAny<RandomObject>(), null, _baseAddress, _uri,
                                                    It.IsAny<Func<RandomObject, string, Task>>()), Times.Once);

        }

        [Test]
        public async void Get_valid_request_cached_should_return_cache_not_hit_CallHttpMethod()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(
                                      x =>
                                      x.CheckCache<object, RandomObject>(null, _baseAddress, _uri, CheckCache))
                               .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Get(_baseAddress, _uri, _headers, CheckCache);

            // assert
            _httpService.Verify(x => x.CallHttpMethod<RandomObject, object>(_baseAddress, _uri, null, _headers, HttpMethods.GET), Times.Never);
            response.BackInTime.Should().Be(responseObj.BackInTime);
        }
        #endregion

        #region Post

        [Test]
        public async void Post_valid_request_returns_response_not_cached()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(x => x.CheckCache<RandomObject, RandomObject>(requestObj, _baseAddress, _uri, null))
                               .Returns(Task.FromResult((RandomObject)null));

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.POST))
                        .Returns(Task.FromResult(responseObj));

            // act
            RandomObject response = await _sut.Post<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers);

            // assert
            response.PointlessProperty.Should().Be(responseObj.PointlessProperty);
            response.BackInTime.Should().Be(responseObj.BackInTime);
        }

        [Test]
        public async void Post_valid_request_returns_response_not_cached_should_add_to_cache()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();
            _cachedObject = null;

            _httpCachingService.Setup(x => x.CheckCache(requestObj, _baseAddress, _uri, CheckCache))
                               .Returns(Task.FromResult((RandomObject)null));

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.POST))
                        .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Post<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, CheckCache);

            // assert
            _httpCachingService.Verify(
                                       x =>
                                       x.AddToCache(It.IsAny<RandomObject>(), It.IsAny<RandomObject>(), _baseAddress,
                                                   _uri, null), Times.Once);

        }

        [Test]
        public async void Post_valid_request_cached_should_return_cache_not_hit_CallHttpMethod()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();

            _cachedObject = responseObj;

            _httpCachingService.Setup(
                                      x =>
                                      x.CheckCache<object, RandomObject>(It.IsAny<RandomObject>(), _baseAddress,
                                                                               _uri, It.IsAny<Func<string, Task<RandomObject>>>()))
                               .Returns(Task.FromResult(_cachedObject));

            // act
            var response = await _sut.Post(_baseAddress, _uri, requestObj, _headers, CheckCache);

            // assert
            _httpService.Verify(x => x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.GET), Times.Never);
            response.BackInTime.Should().Be(_cachedObject.BackInTime);
        }
        #endregion

        #region Put

        [Test]
        public async void Put_valid_request_returns_response_not_cached()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(x => x.CheckCache<RandomObject, RandomObject>(requestObj, _baseAddress, _uri, null))
                               .Returns(Task.FromResult((RandomObject)null));

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.PUT))
                        .Returns(Task.FromResult(responseObj));

            // act
            RandomObject response = await _sut.Put<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers);

            // assert
            response.PointlessProperty.Should().Be(responseObj.PointlessProperty);
            response.BackInTime.Should().Be(responseObj.BackInTime);
        }

        [Test]
        public async void Put_valid_request_returns_response_not_cached_should_add_to_cache()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();
            _cachedObject = null;

            _httpCachingService.Setup(x => x.CheckCache(requestObj, _baseAddress, _uri, CheckCache))
                               .Returns(Task.FromResult((RandomObject)null));


            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.PUT))
                        .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Put(_baseAddress, _uri, requestObj, _headers, CheckCache);

            // assert
            _httpCachingService.Verify(
                                       x =>
                                       x.AddToCache(It.IsAny<RandomObject>(), It.IsAny<RandomObject>(), _baseAddress,
                                                   _uri, null), Times.Once);

        }

        [Test]
        public async void Put_valid_request_cached_should_return_cache_not_hit_CallHttpMethod()
        {
            // arrange
            var responseObj = _fixture.Create<RandomObject>();
            var requestObj = _fixture.Create<RandomObject>();

            _cachedObject = responseObj;

            _httpCachingService.Setup(
                                      x =>
                                      x.CheckCache<object, RandomObject>(It.IsAny<RandomObject>(), _baseAddress,
                                                                               _uri, It.IsAny<Func<string, Task<RandomObject>>>()))
                               .Returns(Task.FromResult(_cachedObject));

            // act
            var response = await _sut.Put(_baseAddress, _uri, requestObj, _headers, CheckCache);

            // assert
            _httpService.Verify(x => x.CallHttpMethod<RandomObject, RandomObject>(_baseAddress, _uri, requestObj, _headers, HttpMethods.PUT), Times.Never);
            response.BackInTime.Should().Be(_cachedObject.BackInTime);
        }
        #endregion

        #region Delete

        [Test]
        public async void Delete_should_call_delete_http_method()
        {
            // act
            await _sut.Delete(_baseAddress, _uri, _headers);

            // assert
            _httpService.Verify(x => x.CallHttpMethod<object, object>(_baseAddress, _uri, null, _headers, HttpMethods.DELETE), Times.Once);
        }

        [Test]
        public async void Delete_should_call_void_cache()
        {
            // act
            await _sut.Delete(_baseAddress, _uri, _headers);

            // assert
            _httpCachingService.Verify(x => x.VoidCache(_baseAddress, _uri, null), Times.Once);

        }

        #endregion

        public async Task<RandomObject> CheckCache(string key)
        {
            return _cachedObject;
        }
    }


}
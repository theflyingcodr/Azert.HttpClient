using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azert.HttpClient.Clients;
using Azert.HttpClient.Services.Interfaces;
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

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture()
                  .Customize(new AutoMoqCustomization())
                  .Customize(new MultipleCustomization());

            _httpService = _fixture.Freeze<Mock<IHttpService>>();
            _httpCachingService = _fixture.Freeze<Mock<IHttpCachingService>>();

            _sut = _fixture.Create<HttpAsyncClient>();
        }

        #region Get

        [Test]
        public async void Get_valid_request_returns_response()
        {
            // arrange
            var baseAddress = _fixture.Create<string>();
            var uri = _fixture.Create<string>();
            var headers = _fixture.Create<Dictionary<string, string>>();

            var responseObj = _fixture.Create<RandomObject>();

            _httpCachingService.Setup(x => x.CheckCache<object, RandomObject>(null, baseAddress, uri, null))
                               .Returns((RandomObject)null);

            _httpService.Setup(
                               x =>
                               x.CallHttpMethod<RandomObject, object>(baseAddress, uri, null, headers, HttpMethods.GET))
                        .Returns(Task.FromResult(responseObj));

            // act
            var response = await _sut.Get<RandomObject>(baseAddress, uri, headers);

            // assert
            response.Should().BeOfType<RandomObject>();
            response.PointlessProperty.Should().Be(responseObj.PointlessProperty);
            response.BackInTime.Should().Be(responseObj.BackInTime);

        }

        #endregion
    }

    public class RandomObject
    {
        public string PointlessProperty { get; set; }
        public DateTime BackInTime { get; set; }
    }
}
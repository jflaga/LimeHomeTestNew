using LimeHomeTest.Dto;
using LimeHomeTest.Dto.CrossCut;
using LimeHomeTest.Services;
using LimeHomeTest.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LimeHomeTest.Tests.MessangerTest
{
    [TestClass, TestCategory("Unit")]
    public class MessangerTests
    {
        public static IOptions<HereApi> hereApi;

        public Mock<IRestClient> RestClientMock { get; set; }
        public Mock<ILogger<Messanger>> LoggerMock { get; set; }

        public Messanger sut { get; set; }

        [ClassInitialize]
        public static void InitClass(TestContext context)
        {
            hereApi = Options.Create<HereApi>(new HereApi { ApiKey = "TestApiKey", RequestUrl = @"http://test.com" });
        }

        [TestInitialize]
        public void Init()
        {
            RestClientMock = new Mock<IRestClient>();
            LoggerMock = new Mock<ILogger<Messanger>>();

            sut = new Messanger(RestClientMock.Object, LoggerMock.Object, hereApi);

        }

        [TestMethod]
        public void Ctor_CanInstantiate()
        {
            var instance = new Messanger(RestClientMock.Object, LoggerMock.Object, hereApi);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Ctor_RestClientIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Messanger(null, LoggerMock.Object, hereApi));
        }

        [TestMethod]
        public void Ctor_LoggerIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Messanger(RestClientMock.Object, null, hereApi));
        }

        [TestMethod]
        public void Ctor_hereApiIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Messanger(RestClientMock.Object, LoggerMock.Object, null));
        }
      
        [TestMethod]
        public void GetHotelsByLocation_CoordinatesIsNotValid_NullOrEmptyString_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.GetHotelsByLocation(string.Empty).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetHotelsByLocation_CoordinatesValid_ExpectedResult_WithCallSaveToDb_OneRecord()
        {
            IRestResponse resp = new RestResponse() { Content = JsonConvert.SerializeObject(new Result() { Results = new List<HotelResult>() }) };
            RestClientMock.Setup(m => m.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(resp))
                .Verifiable();

            var responce = sut.GetHotelsByLocation("19.22,189.33").GetAwaiter().GetResult();

            RestClientMock.Verify(x => x.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsNotNull(responce);
            Assert.IsInstanceOfType(responce, typeof(Result));
            Assert.AreEqual(0, responce.Results.Count());
        }

    }
}

using LimeHomeTest.Controllers;
using LimeHomeTest.Dto.Dtos;
using LimeHomeTest.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LimeHomeTest.Tests.Controller
{
    [TestClass, TestCategory("Unit")]
    public class ControllerTest
    {
        public Mock<IService> ServiceMock { get; set; }
        public Mock<ILogger<PropertiesController>> LoggerMock { get; set; }

        public PropertiesController sut { get; set; }

        [TestInitialize]
        public void Init()
        {
            ServiceMock = new Mock<IService>();
            LoggerMock = new Mock<ILogger<PropertiesController>>();

            sut = new PropertiesController(ServiceMock.Object, LoggerMock.Object);
        }

        [TestMethod]
        public void Ctor_CanInstantiate()
        {
            var instance = new PropertiesController(ServiceMock.Object, LoggerMock.Object);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Ctor_ServiceIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertiesController(null, LoggerMock.Object));
        }

        [TestMethod]
        public void Ctor_LoggerIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PropertiesController(ServiceMock.Object, null));
        }

        [TestMethod]
        public void GetLocation_CoordinatesIsNotValid_NullOrEmptyString_BadRequest()
        {
            var response = sut.GetLocation(string.Empty).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void GetLocation_CoordinatesIsNotValid_NotCoordinatesString_BadRequest()
        {
            var response = sut.GetLocation("Test").GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void GetLocation_CoordinatesValid_ExpectedResult()
        {
            IList<HotelDto> list = new List<HotelDto> { new HotelDto() };
            ServiceMock.Setup(m => m.GetByLocation(It.IsAny<string>())).Returns(Task.FromResult(list))
                .Verifiable();

            var response = sut.GetLocation("19.22,189.33").GetAwaiter().GetResult();

            ServiceMock.Verify();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, (response as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
        }

        [TestMethod]
        public void CreateBookings_BookingIsNotValid_Null_BadRequest()
        {
            var response = sut.CreateBookings(null).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void CreateBookings_BookingIsNotValid_FromMoreThanTo_BadRequest()
        {
            var response = sut.CreateBookings(new BookingDto { From = DateTime.Now, To = DateTime.Now.AddDays(-1), HotelId = 1 })
                .GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void CreateBookings_CoordinatesValid_ExpectedResult()
        {
            IList<HotelDto> list = new List<HotelDto> { new HotelDto() };
            
            ServiceMock.Setup(m => m.CreateBookings(It.IsAny<BookingDto>())).Returns(Task.FromResult("test"))
                .Verifiable();

            var response = sut.CreateBookings(new BookingDto { From = DateTime.Now, To = DateTime.Now.AddDays(1), HotelId = 1 })
                .GetAwaiter().GetResult();

            ServiceMock.Verify();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, (response as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
        }

        [TestMethod]
        public void GetBookings_IdIsNull_BadRequest()
        {
            var response = sut.GetBookings(null).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void GetBookings_IdIsLessOrEqualToZero_BadRequest()
        {
            var response = sut.GetBookings(0).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (response as Microsoft.AspNetCore.Mvc.BadRequestObjectResult).StatusCode);
        }

        [TestMethod]
        public void GetBookings_IdIsValid_ExpectedResult()
        {
            IList<BookingDto> list = new List<BookingDto> { new BookingDto() };
            ServiceMock.Setup(m => m.GetBookings(It.IsAny<int>())).Returns(Task.FromResult(list))
                .Verifiable();

            var response = sut.GetBookings(1).GetAwaiter().GetResult();

            ServiceMock.Verify();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(Microsoft.AspNetCore.Mvc.OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, (response as Microsoft.AspNetCore.Mvc.OkObjectResult).StatusCode);
        }
    }
}

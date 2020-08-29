using AutoMapper;
using LimeHomeTest.Dto;
using LimeHomeTest.Dto.Dtos;
using LimeHomeTest.Repository;
using LimeHomeTest.Repository.Models;
using LimeHomeTest.Services;
using LimeHomeTest.Services.Contracts;
using LimeHomeTest.Services.Mapper;
using LimeHomeTest.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LimeHomeTest.Tests.Services
{
    [TestClass, TestCategory("Unit")]
    public class ServiceTest
    {
        public static IMapper Mapper { get; set; }

        public Mock<IDBRepository> RepoMock { get; set; }
        public Mock<IMessanger> MessangerMock { get; set; }
        public Mock<ILogger<IService>> LoggerMock { get; set; }

        public Service sut { get; set; }

        [ClassInitialize]
        public static void InitClass(TestContext context)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            Mapper = mappingConfig.CreateMapper();
        }

        [TestInitialize]
        public void Init()
        {
            RepoMock = new Mock<IDBRepository>();
            MessangerMock = new Mock<IMessanger>();
            LoggerMock = new Mock<ILogger<IService>>();

            sut = new Service(RepoMock.Object, Mapper, MessangerMock.Object, LoggerMock.Object);
        }

        [TestMethod]
        public void Ctor_CanInstantiate()
        {
            var instance = new Service(RepoMock.Object, Mapper, MessangerMock.Object, LoggerMock.Object);
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Ctor_RepoIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Service(null, Mapper, MessangerMock.Object, LoggerMock.Object));
        }

        [TestMethod]
        public void Ctor_MapperIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Service(RepoMock.Object, null, MessangerMock.Object, LoggerMock.Object));
        }

        [TestMethod]
        public void Ctor_MessangerIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Service(RepoMock.Object, Mapper, null, LoggerMock.Object));
        }

        [TestMethod]
        public void Ctor_LoggerIsNull_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Service(RepoMock.Object, Mapper, MessangerMock.Object, null));
        }

        [TestMethod]
        public void GetByLocation_CoordinatesIsNotValid_NullOrEmptyString_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.GetByLocation(string.Empty).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetByLocation_CoordinatesValid_ExpectedResult_WithCallSaveToDb_OneRecord()
        {
            IList<HotelResult> list = new List<HotelResult> { new HotelResult() };
            Dto.Result res = new Dto.Result { Results = list };
            MessangerMock.Setup(m => m.GetHotelsByLocation(It.IsAny<string>())).Returns(Task.FromResult(res))
                .Verifiable();
            RepoMock.Setup(m => m.AddRangeHotels(It.IsAny<IEnumerable<Hotel>>())).Returns(Task.CompletedTask)
                .Verifiable();

            var response = sut.GetByLocation("19.22,189.33").GetAwaiter().GetResult();

            var hotelsDbo = Mapper.Map<IEnumerable<Hotel>>(list);
            var expected = Mapper.Map<List<HotelDto>>(hotelsDbo);

            MessangerMock.Verify();
            RepoMock.Verify(x => x.AddRangeHotels(It.IsAny<IEnumerable<Hotel>>()), Times.Once);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(List<HotelDto>));
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        public void GetByLocation_CoordinatesValid_ExpectedResult_WithoutCallSaveToDb_EmptyResult()
        {
            IList<HotelResult> list = new List<HotelResult>();
            Dto.Result res = new Dto.Result { Results = list };
            MessangerMock.Setup(m => m.GetHotelsByLocation(It.IsAny<string>())).Returns(Task.FromResult(res))
                .Verifiable();

            var response = sut.GetByLocation("19.22,189.33").GetAwaiter().GetResult();

            var hotelsDbo = Mapper.Map<IEnumerable<Hotel>>(list);
            var expected = Mapper.Map<List<HotelDto>>(hotelsDbo);

            MessangerMock.Verify();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(List<HotelDto>));
            Assert.AreEqual(expected.Count, response.Count);
        }

        [TestMethod]
        public void CreateBookings_BookingIsNotValid_Null_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.CreateBookings(null).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void CreateBookings_BookingIsValid_ExpectedResult_WithoutCallSaveToDb_NullResult()
        {
            BookingDto bookingDto = new BookingDto { From = DateTime.Today, To = DateTime.Today.AddDays(1), HotelId = 1 };
            Hotel hotel = new Hotel();
            RepoMock.Setup(m => m.GetHotelById(It.IsAny<int>())).Returns(Task.FromResult(hotel))
                .Verifiable();

            var response = sut.CreateBookings(bookingDto).GetAwaiter().GetResult();   

            RepoMock.Verify(x => x.GetHotelById(It.IsAny<int>()), Times.Once);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(string));
        }

        [TestMethod]
        public void CreateBookings_BookingIsValid_ExpectedResult_WithCallSaveToDb_OneRecord()
        {
            BookingDto bookingDto = new BookingDto { From = DateTime.Today, To = DateTime.Today.AddDays(1), HotelId = 1 };
            Booking booking = Mapper.Map<Booking>(bookingDto);
            Hotel hotel = new Hotel { Id = 1, Title = "Test", Distance = 0 };
            RepoMock.Setup(m => m.GetHotelById(It.IsAny<int>())).Returns(Task.FromResult(hotel))
                .Verifiable();
            RepoMock.Setup(m => m.AddBooking(It.IsAny<Booking>())).Returns(Task.CompletedTask)
                .Verifiable();

            var response = sut.CreateBookings(bookingDto).GetAwaiter().GetResult();

            RepoMock.Verify(x => x.GetHotelById(It.IsAny<int>()), Times.Once);
            RepoMock.Verify(x => x.AddBooking(It.IsAny<Booking>()), Times.Once);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(string));
        }

        [TestMethod]
        public void GetBookings_HotelIdIsNotValid_NotPositive_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.GetBookings(-1).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetBookings_HotelIdIsValid_ExpectedResult_WithCallSaveToDb_ReturnResult()
        {
            int hotelId = 1;
            List<Booking> bookings = new List<Booking> { new Booking() };
            RepoMock.Setup(m => m.GetBookingsByHotelId(It.IsAny<int>())).Returns(Task.FromResult(bookings))
                .Verifiable();
            var response = sut.GetBookings(hotelId).GetAwaiter().GetResult();

            var expected = Mapper.Map<List<BookingDto>>(bookings);

            RepoMock.Verify(x => x.GetBookingsByHotelId(It.IsAny<int>()), Times.Once);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(List<BookingDto>));
            Assert.AreEqual(expected.Count, response.Count);
        }
    }
}
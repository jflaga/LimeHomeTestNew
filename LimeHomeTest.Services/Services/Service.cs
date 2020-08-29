using AutoMapper;
using LimeHomeTest.Dto.Dtos;
using LimeHomeTest.Repository;
using LimeHomeTest.Repository.Models;
using LimeHomeTest.Services.Contracts;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LimeHomeTest.Services.Services
{
    public class Service : IService
    {
        private IDBRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMessanger _messanger;
        private readonly ILogger<IService> _logger;

        public Service(IDBRepository repository, IMapper mapper, IMessanger messanger, ILogger<IService> logger)
        {
            Verify(repository, mapper, messanger, logger);

            _repository = repository;
            _mapper = mapper;
            _messanger = messanger;
            _logger = logger;
        }

        public async Task<IList<HotelDto>> GetByLocation(string at)
        {
            if (string.IsNullOrWhiteSpace(at))
            {
                _logger.LogError("Coordinates are null");
                throw new ArgumentNullException("Coordinates are null");
            }

            var hotels = await _messanger.GetHotelsByLocation(at);   
            var hotelsDbo = _mapper.Map<IEnumerable<Hotel>>(hotels.Results);

            if(hotelsDbo.Any())
                await _repository.AddRangeHotels(hotelsDbo);          
            
            return _mapper.Map<List<HotelDto>>(hotelsDbo); 
        }

        public async Task<string> CreateBookings(BookingDto booking)
        {
            if (booking == null)
            {
                _logger.LogError("Booking model are null");
                throw new ArgumentNullException("Booking model are null");
            }

            var bookingDbo = _mapper.Map<Booking>(booking);
            var hotel = await _repository.GetHotelById(booking.HotelId);
            if (hotel != null)
            {
                bookingDbo.Hotel = hotel;
                await _repository.AddBooking(bookingDbo);
                return "Booking successfully added";
            }
            return "There is no such hotel";            
        }

        public async Task<IList<BookingDto>> GetBookings(int hotelId)
        {
            if (hotelId <= 0)
            {
                _logger.LogError("hotelId invalid");
                throw new ArgumentNullException("hotelId invalid");
            }

            var bookingsDbo = await _repository.GetBookingsByHotelId(hotelId);
            return _mapper.Map<List<BookingDto>>(bookingsDbo);
        }

        private void Verify(IDBRepository repository, IMapper mapper, IMessanger messanger, ILogger<IService> logger)
        {            
            if (repository == null)
            {
                _logger.LogError("Repository is null");
                throw new ArgumentNullException("Repository is null");
            }
            if (mapper == null)
            {
                _logger.LogError("Mapper is null");
                throw new ArgumentNullException("Mapper is null");
            }
            if (messanger == null)
            {
                _logger.LogError("Messanger is null");
                throw new ArgumentNullException("Messanger is null");
            }
            if (logger == null)
            {                
                throw new ArgumentNullException("Logger is null");
            }
        }
    }
}
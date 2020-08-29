using LimeHomeTest.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LimeHomeTest.Repository
{
    public interface IDBRepository
    {       
        Task AddRangeHotels(IEnumerable<Hotel> hotels);
        Task<Hotel> GetHotelById(int hotelId);
        Task AddBooking(Booking booking);
        Task<List<Booking>> GetBookingsByHotelId(int hotelId);
        Task<Booking> GetBookingById(int bookingId);
    }
}
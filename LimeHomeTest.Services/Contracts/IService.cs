using LimeHomeTest.Dto.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LimeHomeTest.Services.Contracts
{
    public interface IService
    {
        Task<IList<HotelDto>> GetByLocation(string at);
        Task<string> CreateBookings(BookingDto booking);
        Task<IList<BookingDto>> GetBookings(int hotelId);
    }
}
using LimeHomeTest.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimeHomeTest.Repository
{
    public class DBRepository : IDBRepository
    {
        private readonly Context _context;
        private readonly IServiceScope _scope;

        public DBRepository(IServiceProvider services)
        {
            _scope = services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<Context>();
        }      
        public Task AddRangeHotels(IEnumerable<Hotel> hotels)
        {
             _context.Hotels.AddRange(hotels);            
            return _context.SaveChangesAsync();
        }

        public Task<Hotel> GetHotelById(int hotelId)
        {
            return _context.Hotels.Where(x => x.Id == hotelId).FirstOrDefaultAsync();
        }

        public Task AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            return _context.SaveChangesAsync();
        }
        public Task<List<Booking>> GetBookingsByHotelId(int hotelId)
        {
            return _context.Bookings.Where(x=>x.HotelId==hotelId).ToListAsync();
        }
        public Task<Booking> GetBookingById(int bookingId)
        {
            return _context.Bookings.Where(x => x.Id == bookingId).FirstOrDefaultAsync();
        }
    }
}
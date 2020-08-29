using AutoMapper;
using LimeHomeTest.Dto;
using LimeHomeTest.Dto.Dtos;
using LimeHomeTest.Repository.Models;

namespace LimeHomeTest.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Hotel, HotelDto>();
            CreateMap<HotelDto, Hotel>();
            CreateMap<HotelResult, Hotel>();
            CreateMap<BookingDto, Booking>();
            CreateMap<Booking, BookingDto>();
        }
    }
}
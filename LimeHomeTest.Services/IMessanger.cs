using LimeHomeTest.Dto;
using System.Threading.Tasks;

namespace LimeHomeTest.Services
{
    public interface IMessanger
    {
        Task<Result> GetHotelsByLocation(string at);
    }
}
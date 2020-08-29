using LimeHomeTest.Dto.Dtos;
using LimeHomeTest.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LimeHomeTest.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api")]
    public class PropertiesController : Controller
    {
        private readonly IService _service;
        private readonly ILogger<PropertiesController> _logger;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public PropertiesController(IService service, ILogger<PropertiesController> logger)
        {
            if (service == null || logger == null)
            {
                throw new ArgumentNullException("Parameter is null");
            }

            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets a location to your application
        /// </summary>
        /// <param name="at">Specifies an explicit position as a point, separeted by comma, f.e. 40.74917,-73.98529</param>
        /// <returns>Returns the property around Lat/Lon</returns>
        [HttpGet("properties")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetLocation([FromQuery] string at)
        {
            _logger.LogInformation("Call GetLocation method with params - "+at);
            if (!ModelState.IsValid || !IsCoordivatesValid(at))
            {
                _logger.LogInformation("return Bad request");
                return BadRequest("Please, specify a valid coordinates, separated by comma");                
            }

            var properties = await _service.GetByLocation(at);
            _logger.LogInformation("return OK");

            return Ok(properties);
        }

        /// <summary>
        /// Create Bookings by Hotel Id
        /// </summary>
        [HttpPost("bookings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateBookings([FromBody] BookingDto booking)
        {
            _logger.LogInformation("Call CreateBookings method with params - " + JsonConvert.SerializeObject(booking));
            if (!ModelState.IsValid || booking == null || booking.From > booking.To)
            {
                _logger.LogError("return Bad request");
                return BadRequest("Please, specify a valid fields");               
            }           
            var message= await _service.CreateBookings(booking);
            _logger.LogInformation("return OK");
            return Ok(message);
        }

        /// <summary>
        /// Get Bookings by Hotel Id
        /// </summary>
        [HttpGet("{id}/bookings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBookings(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogError("return Bad request");
                return BadRequest("Invalid parameter Id");
            }

            _logger.LogInformation("Call GetBookings method with param - " + id.ToString());
            var bookings = await _service.GetBookings(id.Value);
            _logger.LogInformation("return OK");
            return Ok(bookings);
        }

        private bool IsCoordivatesValid(string latLong)
        {
            char separator = ',';

            if (!string.IsNullOrWhiteSpace(latLong) && latLong.Contains(separator, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var splitted = latLong.Split(separator);
                if (splitted.Length == 2 && splitted.All(x => double.TryParse(x, out double res)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
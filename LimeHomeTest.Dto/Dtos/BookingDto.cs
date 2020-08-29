using System;
using System.ComponentModel.DataAnnotations;

namespace LimeHomeTest.Dto.Dtos
{
    public class BookingDto
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime From { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime To { get; set; }

        [Required]
        public int HotelId { get; set; }
    }
}
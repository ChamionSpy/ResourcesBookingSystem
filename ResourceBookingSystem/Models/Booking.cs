using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }  // Primary key

        [Required]
        public int ResourceId { get; set; }  // ID of the booked resource

        [Required]
        public DateTime StartTime { get; set; }  // Booking start time

        [Required]
        public DateTime EndTime { get; set; }  // Booking end time

        [Required]
        public string BookedBy { get; set; } = "";  // Name of the person who booked

        [Required]
        public string Purpose { get; set; } = "";  // Purpose of the booking

        // Navigation property to the booked resource
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; } = null!;
    }
}

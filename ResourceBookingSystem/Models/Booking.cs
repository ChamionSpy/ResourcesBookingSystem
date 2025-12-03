using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }  // Primary key

        [Required(ErrorMessage = "Please select a resource")]
        public int ResourceId { get; set; }  // ID of the booked resource

        [Required(ErrorMessage = "StartTime is required")]
        public DateTime StartTime { get; set; }  // Booking start time

        [Required(ErrorMessage = "EndTime is required")]
        public DateTime EndTime { get; set; }  // Booking end time
        public string BookedBy { get; set; } = "";  // Name of the person who booked

        [Required(ErrorMessage = "Purpose is required")]
        public string Purpose { get; set; } = "";  // Purpose of the booking

        // Navigation property to the booked resource
        [ForeignKey("ResourceId")]
        [ValidateNever] //Skip validation during model binding
        public Resource Resource { get; set; } = null!;
    }
}

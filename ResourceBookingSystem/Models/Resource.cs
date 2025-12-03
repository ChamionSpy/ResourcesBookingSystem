using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ResourceBookingSystem.Models
{
    public class Resource
    {
        public int Id { get; set; }  // Primary key

        [Required]
        public string Name { get; set; } = "";  // Name of the resource

        public string Description { get; set; } = "";  // Optional description
        public string Location { get; set; } = "";     // Location of the resource

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }  // Maximum capacity

        public bool IsAvailable { get; set; } = true;  // Availability status

        // List of bookings for this resource
        [ValidateNever] //Skip validation during model binding
        public List<Booking> Bookings { get; set; } = new();
    }
}

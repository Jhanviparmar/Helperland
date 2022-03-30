using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class CustomRatings
    {
        public int ServiceRequestId { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserProfilePicture { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string spFirstName { get; set; }
        public string spLastName { get; set; }
        public decimal Ratings { get; set; }
        public DateTime ServiceStartDate { get; set; } = DateTime.Now;
        public int? Status { get; set; }
        public int? ServiceProviderId { get; set; }
        public decimal TotalCost { get; set; }
        public string Comments { get; set; }

        public DateTime RatingDate { get; set; }

        public int ServiceRequestExtraId { get; set; }
        public int ServiceExtraId { get; set; }
    }
}

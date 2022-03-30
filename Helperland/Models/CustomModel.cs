using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class CustomModel
    {
        public int ServiceRequestId { get; set; }
        public int UserId { get; set; }
        public string UserProfilePicture { get; set; }
        public int ServiceId { get; set; }
        [NotMapped]
        public int AddressId { get; set; }

        public int ServiceExtraId { get; set; }
        public DateTime ServiceStartDate { get; set; } = DateTime.UtcNow;
        public string ZipCode { get; set; }
        public decimal? ServiceHourlyRate { get; set; }
        public double ServiceHours { get; set; }
        public double? ExtraHours { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalCost { get; set; }
        public string Comments { get; set; }
        public string PaymentTransactionRefNo { get; set; }
        public bool PaymentDue { get; set; }
        public int? ServiceProviderId { get; set; }
        public DateTime? SpacceptedDate { get; set; } = DateTime.UtcNow;
        public bool HasPets { get; set; }
        public int? Status { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Mobile { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        public int UserTypeId { get; set; }
        public int? Gender { get; set; }

        public string spFirstName { get; set; }
        public string spLastName { get; set; }
        public decimal Ratings { get; set; }

        public int ServiceRequestExtraId { get; set; }
       


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Helperland.Models
{
    public partial class ContactU
    {
        public int ContactUsId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string Subject { get; set; }
        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression("^[6789][0-9]{9}$", ErrorMessage = "Enter valid mobile number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
        public string UploadFileName { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public string FileName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Helperland.Models
{
    public partial class ContactU
    {
        public int ContactUsId { get; set; }
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Name Should be min 2 and max 20 length")]
        public string Name { get; set; }
        [NotMapped]
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Name Should be min 2 and max 20 length")]
        public string FirstName { get; set; }
        [NotMapped]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Name Should be min 2 and max 20 length")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter your email address")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public string Subject { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid Mobile number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
        public string UploadFileName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public string FileName { get; set; }
    }
}

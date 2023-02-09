using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace University.Portal.Data.Data.ViewModels
{
    public class UniversityRegisterViewModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(20, ErrorMessage = "Must be between 5 and 20 characters", MinimumLength = 5)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> Role { get; set; }
        
        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "University is required")]
        public string University { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10, ErrorMessage = "Must be 10 digits", MinimumLength = 10)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Input should contain only numbers")]
        public string PhoneNumber { get; set; }

        public string UserNameMatchError { get; set; }
        public string UniversitySelectionError { get; set; }
        public bool IsRegistrationFailed { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace University.Portal.Data.Data.ViewModels
{
	public class StudentViewModel
	{		
		public int Id { get; set; }
		
		[Required(ErrorMessage = "First Name is required")]
        [StringLength(20, ErrorMessage = "Must be between 3 and 20 characters", MinimumLength = 3)]
		public string FirstName { get; set; }
		
		[StringLength(10, ErrorMessage = "Must be between 1 and 10 characters", MinimumLength = 1)]
		public string MiddleName { get; set; }
		
		[StringLength(10, ErrorMessage = "Must be between 1 and 10 characters", MinimumLength = 1)]
		public string LastName { get; set; }
		public IEnumerable<SelectListItem> Gender { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string GenderId { get; set; }
		
		[Required(ErrorMessage = "Email is required")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone Number is required")]
		[DataType(DataType.PhoneNumber)]
		[StringLength(10, ErrorMessage = "Must be 10 digits", MinimumLength = 10)]
		[RegularExpression(@"^\d+$", ErrorMessage = "Input should contain only numbers")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Date of Birth is required")]
		public DateTime? DOB { get; set; }

		[Required(ErrorMessage = "Date of Joining is required")]
		public DateTime? DOJ { get; set; }

		public IEnumerable<SelectListItem> Department { get; set; }

		[Required(ErrorMessage = "Department is required")]
		public string DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Year { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public string YearId { get; set; }

        public bool IsRegistrationFailed { get; set; }
	}
}

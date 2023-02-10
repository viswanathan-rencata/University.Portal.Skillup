using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data.Models;

namespace University.Portal.Data.Data.ViewModels
{
	public class StudentGridModel
	{		
		public int Id { get; set; }
		public string StudentCode { get; set; }
		public string FullName { get; set; }
		public string Gender { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string DateOfBirth { get; set; }
		public string DateOfJoining { get; set; }
		public string DepartmentName { get; set; }
		public int Year { get; set; }
	}
}

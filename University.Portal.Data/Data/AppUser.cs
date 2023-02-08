using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data
{
	[Table("AppUser")]
	public class AppUser
	{
		[Key, Column("ID", Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column("UserName")]
		public string UserName { get; set; }

		[Column("PasswordHash")]
		public byte[] PasswordHash { get; set; }

		[Column("PasswordSalt")]
		public byte[] PasswordSalt { get; set; }

		[Column("CompanyOrCollege")]
		public int CompanyOrCollege { get; set; }

		[Column("CollegeId")]
		public int? CollegeId { get; set; }

		[Column("CompanyId")]
		public int? CompanyId { get; set; }

		[Column("Email")]
		public string Email { get; set; }

		[Column("PhoneNumber")]
		public string PhoneNumber { get; set; }

		[Column("Status")]
		public bool Status { get; set; }

		[Column("Education")]
		public string Education { get; set; }

		[Column("AddressLine1")]
		public string AddressLine1 { get; set; }

		[Column("AddressLine2")]
		public string AddressLine2 { get; set; }

		[Column("Postcode")]
		public string Postcode { get; set; }

		[Column("Area")]
		public string Area { get; set; }

		[Column("Country")]
		public string Country { get; set; }

		[Column("State")]
		public string State { get; set; }

		[Column("City")]
		public string City { get; set; }

		[Column("Created")]
		public DateTime Created { get; set; } = DateTime.Now;

		[Column("LastActive")]
		public DateTime LastActive { get; set; } = DateTime.Now;
	}
}

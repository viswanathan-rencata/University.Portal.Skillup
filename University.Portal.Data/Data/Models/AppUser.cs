using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Portal.Data.Data.Models;

namespace University.Portal.Data.Data
{
	[Table("AppUser", Schema = "UniversityPortal")]
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

		[Column("StudentOrUniversity")]
		public int StudentOrUniversity { get; set; }

		[Column("UniversityId")]
		public int? UniversityId { get; set; }

		[Column("StudentId")]
		public int? StudentId { get; set; }

		[Column("Status")]
		public bool Status { get; set; }
        
		[Column("Email")]
        public string Email { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("CreatedOn")]
		public DateTime CreatedOn { get; set; } = DateTime.Now;

		public AppUserRole AppUserRole { get; set; }
		public UniversityMaster? University { get; set; }
    }
}

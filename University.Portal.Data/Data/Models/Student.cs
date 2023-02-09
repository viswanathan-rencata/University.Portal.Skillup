using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("Student", Schema = "UniversityPortal")]
    public class Student
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("StudentCode")]
        public string StudentCode { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("MiddleName")]
        public string MiddleName { get; set; }
        
        [Column("LastName")]
        public string LastName { get; set; }

        [Column("Gender")]
        public char Gender { get; set; }
        
        [Column("Email")]
        public string Email { get; set; }

        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Column("DOB")]
        public DateTime? DOB { get; set; }

        [Column("DOJ")]
        public DateTime? DOJ { get; set; }

        [Column("UniversityId")]
        public int? UniversityId { get; set; }

        [Column("DepartmentId")]
        public int? DepartmentId { get; set; }

        [Column("Status")]
        public bool Status { get; set; }
        
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public Department? Department { get; set; }
        public UniversityMaster? University { get; set; }
    }
}

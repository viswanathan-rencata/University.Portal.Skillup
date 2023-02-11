using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("TutionFeeDetails", Schema = "UniversityPortal")]
    public class TutionFeeDetails
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("UniversityId")]
        public int UniversityId { get; set; }
        
        [Column("DepartmentId")]
        public int DepartmentId { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("DueDate")]
        public DateTime DueDate { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
        [Column("IsActive")]
        public bool IsActive { get; set; }
        public UniversityMaster University { get; set; }
        public Department Department { get; set; }
    }
}

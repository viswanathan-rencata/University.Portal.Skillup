using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("ExamSchedule", Schema = "UniversityPortal")]
    public class ExamSchedule
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("DepartmentId")]
        public int DepartmentId { get; set; }
        
        [Column("UniversityId")]
        public int UniversityId { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }
        
        [Column("IsActive")]
        public bool IsActive { get; set; }

        public Department Department { get; set; }
        public UniversityMaster University { get; set; }
    }
}

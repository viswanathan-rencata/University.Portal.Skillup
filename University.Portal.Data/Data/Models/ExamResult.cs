using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("ExamResult", Schema = "UniversityPortal")]
    public class ExamResult
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("StudentId")]
        public int StudentId { get; set; }

        [Column("CGPA")]
        public decimal CGPA { get; set; }

        [Column("ExamResult")]
        public bool Result { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
        
        public Student Student { get; set; }
    }
}

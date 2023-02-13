using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("SubjectResult", Schema = "UniversityPortal")]
    public class SubjectResult
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("SubjectMasterId")]
        public int SubjectMasterId { get; set; }

        [Column("StudentId")]
        public int StudentId { get; set; }

        [Column("ExamResult")]
        public bool ExamResult { get; set; }

        [Column("Mark")]
        public decimal Mark { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public SubjectMaster SubjectMaster { get; set; }
        public Student Student { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("SubjectMaster", Schema = "UniversityPortal")]
    public class SubjectMaster
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("DepartmentId")]
        public int DepartmentId { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("SubjectCode")]
        public string SubjectCode { get; set; }

        [Column("SubjectName")]
        public string SubjectName { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }        
        public Department Department { get; set; }
    }
}

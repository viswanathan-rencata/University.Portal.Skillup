using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("StudentDocument", Schema = "UniversityPortal")]
    public class StudentDocument
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("StudentId")]
        public int StudentId { get; set; }

        [Column("DocumentMasterId")]
        public int DocumentMasterId { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
        public Student Student { get; set; }
        public DocumentMaster DocumentMaster { get; set; }
    }
}

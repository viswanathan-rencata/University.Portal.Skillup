using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("UploadDocument", Schema = "UniversityPortal")]
    public class UploadDocument
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("StudentId")]
        public int StudentId { get; set; }

        [Column("DocumentType")]
        public string DocumentType { get; set; }
        
        [Column("DocumentName")]
        public string DocumentName { get; set; }

        [Column("DocumentData")]
        public byte[] DocumentData { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }
        
        [Column("Status")]
        public bool? Status { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Student Student { get; set; }
    }
}

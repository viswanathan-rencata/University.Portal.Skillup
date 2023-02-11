using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("Notification", Schema = "UniversityPortal")]
    public class Notification
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("StudentOrUniversity")]
        public int StudentOrUniversity { get; set; }

        [Column("StudentID")]
        public int? StudentID { get; set; }
        
        [Column("UniversityId")]
        public int? UniversityId { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public Student? Student { get; set; }
        public UniversityMaster? University { get; set; }
    }
}

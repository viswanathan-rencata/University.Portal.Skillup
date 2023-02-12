using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("DocumentMaster", Schema = "UniversityPortal")]
    public class DocumentMaster
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("DocCode")]
        public string DocCode { get; set; }

        [Column("DocName")]
        public string DocName { get; set; }
        
    }
}

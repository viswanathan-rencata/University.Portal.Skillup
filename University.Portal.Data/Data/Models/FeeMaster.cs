using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("FeeMaster", Schema = "UniversityPortal")]
    public class FeeMaster
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("FeeType")]
        public string FeeType { get; set; }
        
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}

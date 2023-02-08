using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("Role", Schema = "UniversityPortal")]
    public class Role
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("RoleName")]
        public string RoleName { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}

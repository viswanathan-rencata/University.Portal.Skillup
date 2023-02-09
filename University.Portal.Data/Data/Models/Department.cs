using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("Department", Schema = "UniversityPortal")]
    public class Department
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("DepartmentName")]
        public string DepartmentName { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}

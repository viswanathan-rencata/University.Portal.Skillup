using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("AppUserRole", Schema = "UniversityPortal")]
    public class AppUserRole
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("AppUserID")]
        public int AppUserID { get; set; }

        [Column("RoleID")]
        public int RoleID { get; set; }
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public Role Role { get; set; }
        public AppUser AppUser { get; set; }
    }
}

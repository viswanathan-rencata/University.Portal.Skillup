using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.Models
{
    [Table("FeePayment", Schema = "UniversityPortal")]
    public class FeePayment
    {
        [Key, Column("ID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("FeeDetailsId")]
        public int FeeDetailsId { get; set; }
        
        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("StudentID")]
        public int StudentID { get; set; }
        
        [Column("PaymentDate")]
        public DateTime PaymentDate { get; set; }
        public FeeDetails FeeDetails { get; set; }
        public Student Student { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int FeeDetailsId { get; set; }
        public decimal Amount { get; set; }
        public int StudentID { get; set; }
    }
}

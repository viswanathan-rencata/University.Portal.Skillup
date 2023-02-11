using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class FeePaymentViewModel
    {
        public int Id { get; set; }
        public string FeeType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}

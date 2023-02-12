using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class PendingPaymentViewModel
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; } 
        public string Department { get; set;}
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}

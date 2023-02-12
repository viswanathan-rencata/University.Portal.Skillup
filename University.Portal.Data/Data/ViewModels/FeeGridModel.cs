using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class FeeGridModel
    {        
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string Year { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}

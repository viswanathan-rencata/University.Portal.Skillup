using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class ExamResultGridViewModel
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
    }
}

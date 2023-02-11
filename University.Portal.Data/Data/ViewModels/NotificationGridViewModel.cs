using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class NotificationGridViewModel
    {        
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class ExamResultNewViewModel
    {
        public int Id { get; set; }
        public int SubjectMasterId1 { get; set; }
        public string SubjectName1 { get;set; }
        
        [Required(ErrorMessage ="Subject value is required!")]
        public int SubjectValue1 { get; set; }
        
        public int SubjectMasterId2 { get; set; }
        public string SubjectName2 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue2 { get; set; }
        
        public int SubjectMasterId3 { get; set; }
        public string SubjectName3 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue3 { get; set; }
        
        public int SubjectMasterId4 { get; set; }
        public string SubjectName4 { get; set; }

        [Required(ErrorMessage = "Subject value is required!")] 
        public int SubjectValue4 { get; set; }
        
        public int SubjectMasterId5 { get; set; }
        public string SubjectName5 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue5 { get; set; }
        
        public int SubjectMasterId6 { get; set; }
        public string SubjectName6 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue6 { get; set; }

        public int SubjectMasterId7 { get; set; }        
        public string SubjectName7 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue7 { get; set; }
        
        public int SubjectMasterId8 { get; set; }
        public string SubjectName8 { get; set; }
        
        [Required(ErrorMessage = "Subject value is required!")]
        public int SubjectValue8 { get; set; }
    }
}

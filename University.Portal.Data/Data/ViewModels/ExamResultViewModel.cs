using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class ExamResultViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CGPA is required")]
        public decimal? CGPA { get; set; }

        public IEnumerable<SelectListItem> Result { get; set; }

        [Required(ErrorMessage = "Result is required")]
        public string ResultId { get; set; }
    }
}

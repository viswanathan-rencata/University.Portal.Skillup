using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class ExamScheduleViewModel
    {
        public int Id { get; set; }
        public IEnumerable<SelectListItem> Department { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string DepartmentId { get; set; }

        public IEnumerable<SelectListItem> Year { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public string YearId { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime? StartDate { get; set; }
        
        [Required(ErrorMessage = "End Date is required.")]
        public DateTime? EndDate { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class FeeDetailsViewModel
    {
        public int Id { get; set; }

        public IEnumerable<SelectListItem> Year { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public string YearId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]        
        public string Amount { get; set; }

        [Required(ErrorMessage = "DueDate is required.")]        
        public DateTime? DueDate { get; set; }

        public IEnumerable<SelectListItem> Department { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string DepartmentId { get; set; }
        
    }
}

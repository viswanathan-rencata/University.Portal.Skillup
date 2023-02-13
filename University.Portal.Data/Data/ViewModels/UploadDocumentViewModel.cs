using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class UploadDocumentViewModel
    {
        [Required(ErrorMessage ="Document Type is required")]
        public string DocumentType { get; set; }
        
        public IFormFile DocumentData { get; set; }

        public List<UploadDocumentGrid> UploadDocumentGrid { get; set; }
    }
}

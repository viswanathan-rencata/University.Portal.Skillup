using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class UploadDocumentGrid
    {
        public int Id { get; set; }
        public string DocumentType { get;set; }
        public string DocumentName { get; set; }
        public string UploadedOn { get; set; }
        public string Status { get; set; }
    }
}

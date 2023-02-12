using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class PublishDocumentViewModel
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string StudentName { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string Document { get; set; }
    }
}

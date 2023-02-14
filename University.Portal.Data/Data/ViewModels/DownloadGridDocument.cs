using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Portal.Data.Data.ViewModels
{
    public class DownloadGridDocument
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

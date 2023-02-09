using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using University.Portal.Data.Data;
using University.Portal.Data.Data.ViewModels;

namespace University.Portal.Web.Controllers
{
    [Authorize]
    public class UniversityController : Controller
    {
        public UniversityController()
        {
            
        }

        public IActionResult Index()
        {                      
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;

namespace University.Portal.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult University()
        {
            return View();
        }

        public IActionResult Student()
        {
            var notificationList = (from a in _unitOfWork.NotificationRepository
                                    .GetByFilter(x => x.StudentID == GetStudentId, IncludeStr: "Student")
                                    select new NotificationGridViewModel
                                    {
                                        Id = a.Id,                                        
                                        Message = a.Message,
                                        CreatedOn = a.CreatedOn
                                    }).ToList();
            return View(notificationList);
        }

        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
    }
}

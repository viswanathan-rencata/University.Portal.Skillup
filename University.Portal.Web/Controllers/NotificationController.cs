using Microsoft.AspNetCore.Mvc;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;
using static University.Portal.Data.Data.Enum;

namespace University.Portal.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }        
        public IActionResult Notification()
        {
            var notificationList = new List<NotificationGridViewModel>();

            if (GetStudentOrUniversityFlag == (int)StudentOrUniversity.University)
            {
                notificationList = (from a in _unitOfWork.NotificationRepository
                                    .GetByFilter(x => x.UniversityId == GetUniversityId)
                                    select new NotificationGridViewModel
                                    {
                                        Id = a.Id,
                                        Message = a.Message,
                                        CreatedOn = a.CreatedOn
                                    }).ToList();
            }
            else
            {
                notificationList = (from a in _unitOfWork.NotificationRepository
                                    .GetByFilter(x => x.StudentID == GetStudentId)
                                    select new NotificationGridViewModel
                                    {
                                        Id = a.Id,
                                        Message = a.Message,
                                        CreatedOn = a.CreatedOn
                                    }).ToList();
            }

            TempData["SetActiveTab"] = $"setActiveTabClass('notifications');";

            return View(notificationList);
        }
        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
        private int GetUniversityId => Convert.ToInt32(HttpContext.User.FindFirst("UniversityId").Value);
        private int GetStudentOrUniversityFlag => Convert.ToInt32(HttpContext.User.FindFirst("StudentOrUniversity").Value);
    }
}

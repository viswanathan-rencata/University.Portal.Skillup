using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;

namespace University.Portal.Web.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DownloadDocuments()
        {
            return View();
        }   
        
        public IActionResult FeePayment()
        {
            var student = _unitOfWork.StudentRepository.Get(GetStudentId);
            
            var availableFeeDetails = (from a in _unitOfWork.FeeDetailsRepository.GetAll(IncludeStr: "FeeMaster")
                                       where a.IsActive == true && a.UniversityId == student.UniversityId
                                       && a.DepartmentId == student.DepartmentId && a.Year == student.Year
                                       select a).ToList();

            var feePaymentInfo = (from a in _unitOfWork.FeePaymentRepository.GetAll()
                                  where a.StudentID == GetStudentId
                                  select a).ToList();

            availableFeeDetails = availableFeeDetails.Where(x=> !feePaymentInfo.Select(x=>x.FeeDetailsId).Contains(x.Id)).ToList();

            List<FeePaymentViewModel> list = (from a in availableFeeDetails
                                              select new FeePaymentViewModel
                                              {
                                                  Id = a.Id,
                                                  FeeType = a.FeeMaster.FeeType,
                                                  Amount = a.Amount,
                                                  DueDate = a.DueDate
                                              }).ToList();
            return View(list);
        }

        public IActionResult Payment(int id)
        {
            var feeDetail = _unitOfWork.FeeDetailsRepository.Get(id);

            PaymentViewModel model = new();
            model.FeeDetailsId = id;
            model.Amount = feeDetail.Amount;
            model.StudentID = GetStudentId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            var feeDetail = _unitOfWork.FeeDetailsRepository.Get(model.FeeDetailsId);

            var feePayment = new FeePayment()
            {
                FeeDetailsId = feeDetail.Id,
                Amount = feeDetail.Amount,
                StudentID = GetStudentId,
            };

            _unitOfWork.FeePaymentRepository.Add(feePayment);
            
            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('Payment completed Succeefully!','');";

            return RedirectToAction("FeePayment");
        }

        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
    }
}

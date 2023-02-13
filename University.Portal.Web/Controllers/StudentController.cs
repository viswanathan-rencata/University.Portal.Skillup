using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SelectPdf;
using System.Reflection;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;
using static University.Portal.Data.Data.Enum;

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
            var studentDocument = (from a in _unitOfWork.StudentDocumentRepository.GetByFilter(x => x.StudentId == GetStudentId, IncludeStr: "DocumentMaster")
                                   select new DownloadGridDocument()
                                   {
                                       Id = a.Id,
                                       DocumentId = a.DocumentMasterId,
                                       DocumentName  =a.DocumentMaster.DocName
                                   }).ToList();

            return View(studentDocument);
        }
        
        [HttpPost]
        public JsonResult DownloadDocument(int id)
        {
            var studentDocument = _unitOfWork.StudentDocumentRepository
                .GetByFilter(x => x.Id == id, IncludeStr: "DocumentMaster").FirstOrDefault();

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            //// set converter options
            converter.Options.PdfPageSize = PdfPageSize.Letter;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            string data = ReadTextFile(studentDocument.DocumentMaster.DocCode);

            PdfDocument doc = converter.ConvertHtmlString(data);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            return Json(new { data = pdf });
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
                PaymentDate = DateTime.Now
            };

            _unitOfWork.FeePaymentRepository.Add(feePayment);
            
            SendTutionFeePaymentNotification(feePayment);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('Payment completed Succeefully!','');";

            return RedirectToAction("FeePayment");
        }

        private void SendTutionFeePaymentNotification(FeePayment feePayment)
        {
            List<Notification> notificationList = new();

            var student = _unitOfWork.StudentRepository.Get(GetStudentId);

            var notification = new Notification()
            {
                UniversityId = student.UniversityId,
                StudentOrUniversity = (int)StudentOrUniversity.University,
                Message = $"Tution Fee payment completed by {student.FirstName} {student.LastName}. Total paid amount Rs.{feePayment.Amount.ToString("0.00")} and payment date is {DateTime.Now.ToString("MM/dd/yyyy")}"
            };

            _unitOfWork.NotificationRepository.Add(notification);
        }

        public string ReadTextFile(string docCode)
        {
            string filePath = GetDocumentPath(docCode);

            var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var fullPath = Path.Combine(buildDir, filePath);

            return System.IO.File.ReadAllText(fullPath);
        }

        private string GetDocumentPath(string docCode)
        {
            return docCode switch
            {
                "DOC_001" => @"wwwroot\formTemplate\hallticket.html",
                "DOC_002" => @"wwwroot\formTemplate\marksheet.html",
                "DOC_003" => @"wwwroot\formTemplate\provisionalcertificate.html",
                "DOC_004" => @"wwwroot\formTemplate\degreecertificate.html",
                _ => string.Empty,
            };
        }
        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
    }
}

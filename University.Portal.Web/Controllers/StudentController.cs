using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SelectPdf;
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
            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            //// set converter options
            converter.Options.PdfPageSize = PdfPageSize.Letter;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            string data = GetDocumentTemplate();

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

        private string GetDocumentTemplate()
        {
            return
                @"<!DOCTYPE html>
                <html lang=""en"">
                <head>
                  <meta charset=""UTF-8"">
                  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                  <script src=""https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.0.1/js/bootstrap.bundle.min.js""
                    integrity=""sha512-sH8JPhKJUeA9PWk3eOcOl8U+lfZTgtBXD41q6cO/slwxGHCxKcW45K4oPCUhHG7NMB4mbKEddVmPuTXtpbCbFA==""
                    crossorigin=""anonymous"" referrerpolicy=""no-referrer""></script>
                  <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.0.1/css/bootstrap.min.css""
                    integrity=""sha512-Ez0cGzNzHR1tYAv56860NLspgUGuQw16GiOOp/I2LuTmpSK9xDXlgJz3XN4cnpXWDmkNBKXR/VDMTCnAaEooxA==""
                    crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
                  <link rel=""stylesheet"" href=""app.css"">
                  <title>Document</title>
                </head>
                <body>
                  <div class=""conatiner"" style=""margin: 50px 100px;border-style:groove;padding-left: 50px;padding-bottom: 50px;"">
                    <div class=""row"">                      
                      <div class=""col"">
                        <h1>Employment Offer Letter</h1>
                      </div>                      
                    </div>
                    <div class=""row"">
                      <div class=""col""></div>
                      <div class=""col""></div>
                      <div class=""col center"">
                        <h3>[CompanyName]</h3>
                        <br>
                        <p style=""padding-left: 50px;"">[OfferDate]</p>
                      </div>
                    </div>
                    <div class=""row"">
                      <div class=""col"">
                        [CandidateName]<br>
                        [CandidateAddress]<br>
                        [CandidateCityStateZip]<br>
                      </div>
                    </div>
                    <div class=""row"" style=""padding: 150px 0px 50px 0px;"">
                      <div class=""col"">
                        Dear [CandidateName],
                      </div>
                    </div>
                    <div class=""row"">
                      <div class=""col"">
                        <p>
                          We are pleased to offer you the full-time position of software Trainee at [CompanyName] with a start date of
                          [Startdate], contingent upon [background check, I-9 form, etc.].<br>
                          You will be reporting directly to manager at [Workplacelocation]. We believe your skills and experience are an
                          excellent match for our company. this role, you will be <br>
                          required to researching, investigating and fixing a wide range of technical issues.The annual starting salary
                          for this position is 2,15,000 to be paid on a monthly <br>
                          basis by direct deposit to your account. In addition to this starting salary, we’re offering you stock
                          options, bonuses, commission structures, etc (if applicable).<br>
                          Your employment with [CompanyName] will be on an at-will basis, which means you and the company are free to
                          terminate the employment relationship at any time for any reason. <br>
                          This letter is not a contract or guarantee of employment for a definitive period of time.As an employee of
                          [CompanyName], you are also eligible for our benefits program, <br>
                          which includes [medical insurance, 401(k), vacation time, etc.], and other benefits which will be described in
                          more detail in the employee handbook.<br>
                          Please confirm your acceptance of this offer by signing and returning this letter by
                          [OfferExpirationDate].<br>
                          We are excited to have you join our team! If you have any questions, please feel free to reach out at any
                          time.<br>
                        </p>
                      </div>
                    </div>
                    <div class=""row"" style=""padding-top: 50px;"">
                      <div class=""col"">
                        Sincerely,<br>
                        [Signature]
                      </div>
                    </div>
                  </div>
                </body>
                </html>
                ";
        }

        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
    }
}

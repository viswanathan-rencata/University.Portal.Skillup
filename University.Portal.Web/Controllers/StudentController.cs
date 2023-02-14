using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SelectPdf;
using System.IO;
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
                                       DocumentName = a.DocumentMaster.DocName
                                   }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('downloadDocuments');";

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

            string data = GetTemplate(studentDocument.DocumentMaster.DocCode);

            data = ProcessTemplate(studentDocument.DocumentMaster.DocCode, data);

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

            availableFeeDetails = availableFeeDetails.Where(x => !feePaymentInfo.Select(x => x.FeeDetailsId).Contains(x.Id)).ToList();

            List<FeePaymentViewModel> list = (from a in availableFeeDetails
                                              select new FeePaymentViewModel
                                              {
                                                  Id = a.Id,
                                                  FeeType = a.FeeMaster.FeeType,
                                                  Amount = a.Amount,
                                                  DueDate = a.DueDate
                                              }).ToList();
            
            TempData["SetActiveTab"] = $"setActiveTabClass('feePayment');";
            
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

        public IActionResult ViewResult()
        {
            var departmentList = _unitOfWork.DepartmentRepository.GetAll();

            var examResultViewModel = (from a in _unitOfWork.SubjectResultRepository.GetByFilter(x => x.StudentId == GetStudentId, IncludeStr: "Student,SubjectMaster")
                                       let department = departmentList.Where(x => x.Id == a.Student.DepartmentId).FirstOrDefault()
                                       select new ViewExamResultModel()
                                       {
                                           SubjectCode = a.SubjectMaster.SubjectCode,
                                           SubjectName = a.SubjectMaster.SubjectName,
                                           Mark = a.Mark.ToString("0.00"),
                                           Result = a.ExamResult ? "Pass" : "Fail"
                                       }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('viewResults');";

            return View(examResultViewModel);
        }
        
        public IActionResult UploadDocument()
        {
            var model = new UploadDocumentViewModel();

            var uploadDocumentList = (from a in _unitOfWork.UploadDocumentRepository.GetByFilter(x => x.StudentId == GetStudentId)
                                      select new UploadDocumentGrid
                                      {
                                          Id = a.Id,
                                          DocumentType = a.DocumentType,
                                          DocumentName = a.DocumentName,
                                          UploadedOn = a.CreatedOn.ToString("MM/dd/yyyy"),
                                          Status = a.Status != null ? (a.Status.Value ? "Accepted" : "Rejected") : string.Empty
                                      }).ToList();
            
            model.UploadDocumentGrid = uploadDocumentList;

            TempData["SetActiveTab"] = $"setActiveTabClass('uploadDocument');";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(UploadDocumentViewModel model)
        {
            byte[] fileContent = null;

            if(model.DocumentData == null)
            {
                ModelState.AddModelError("DocumentData", "Please select file");
            }

            if (ModelState.IsValid)
            {
                if (model.DocumentData.Length > 0)
                {
                    //var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.                    
                    using (var ms = new MemoryStream())
                    {
                        model.DocumentData.CopyTo(ms);
                        fileContent = ms.ToArray();
                    }
                }

                var uploadDocument = new UploadDocument()
                {
                    StudentId = GetStudentId,
                    DocumentType = model.DocumentType,
                    DocumentName = model.DocumentData.FileName,
                    DocumentData = fileContent,
                    IsActive = true
                };

                _unitOfWork.UploadDocumentRepository.Add(uploadDocument);

                SendUploadDocumentNotification(uploadDocument);

                TempData["JavaScriptFunction"] = $"showToastrMessage('document uploaded Succeefully!','');";

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("UploadDocument");
                else
                {
                    var uploadDocumentList = (from a in _unitOfWork.UploadDocumentRepository.GetByFilter(x => x.StudentId == GetStudentId)
                                              select new UploadDocumentGrid
                                              {
                                                  Id = a.Id,
                                                  DocumentName = a.DocumentName,
                                                  UploadedOn = a.CreatedOn.ToString("MM/dd/yyyy"),
                                                  Status = a.Status != null ? (a.Status.Value ? "Accepted" : "Rejected") : string.Empty
                                              }).ToList();
                    model.UploadDocumentGrid = uploadDocumentList;

                    return View(model);
                }
            }
            else
            {
                var uploadDocumentList = (from a in _unitOfWork.UploadDocumentRepository.GetByFilter(x => x.StudentId == GetStudentId)
                                          select new UploadDocumentGrid
                                          {
                                              Id = a.Id,
                                              DocumentName = a.DocumentName,
                                              UploadedOn = a.CreatedOn.ToString("MM/dd/yyyy"),
                                              Status = a.Status != null ? (a.Status.Value ? "Accepted" : "Rejected") : string.Empty
                                          }).ToList();
                model.UploadDocumentGrid = uploadDocumentList;

                return View(model);
            }
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

        private void SendUploadDocumentNotification(UploadDocument uploadDocument)
        {
            var student = _unitOfWork.StudentRepository.Get(GetStudentId);

            var notification = new Notification()
            {
                UniversityId = student.UniversityId,
                StudentOrUniversity = (int)StudentOrUniversity.University,
                Message = $"{uploadDocument.DocumentName} uploaded for verification on {DateTime.Now.ToString("MM/dd/yyyy")} by {student.FirstName} {student.MiddleName} {student.LastName}"
            };

            _unitOfWork.NotificationRepository.Add(notification);
        }

        private string GetTemplate(string docCode)
        {
            string filePath = GetDocumentPath(docCode);

            var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var fullPath = Path.Combine(buildDir, filePath);

            return System.IO.File.ReadAllText(fullPath);
        }

        private string ProcessTemplate(string docCode, string template)
        {
            string output = template;

            var student = _unitOfWork.StudentRepository.GetByFilter(x => x.Id == GetStudentId, IncludeStr: "University,Department").FirstOrDefault();
            var examSchedule = _unitOfWork.ExamScheduleRepository
                .GetByFilter(x => x.UniversityId == student.UniversityId && x.DepartmentId == student.DepartmentId && x.Year == student.Year).FirstOrDefault();

            if (docCode == "DOC_001")
            {
                output = output.Replace("[UniversityName]", student.University.UniversityName);
                output = output.Replace("[StudentCode]", student.StudentCode);
                output = output.Replace("[StudentName]", $"{student.FirstName} {student.MiddleName} {student.LastName}");
                output = output.Replace("[DepartmentName]", student.Department.DepartmentName);
                output = output.Replace("[Class]", GetYearDesc(student.Year.Value));
                output = output.Replace("[ExamDate]", $"{examSchedule.StartDate.ToString("MM/dd/yyyy")} - {examSchedule.EndDate.ToString("MM/dd/yyyy")}");
            }
            else if (docCode == "DOC_002")
            {
                var subjectResult = _unitOfWork.SubjectResultRepository
                    .GetByFilter(x => x.StudentId == GetStudentId, IncludeStr: "SubjectMaster").ToList();

                output = output.Replace("[UniversityName]", student.University.UniversityName);
                output = output.Replace("[StudentCode]", student.StudentCode);
                output = output.Replace("[StudentName]", $"{student.FirstName} {student.MiddleName} {student.LastName}");
                output = output.Replace("[DepartmentName]", student.Department.DepartmentName);
                output = output.Replace("[Class]", GetYearDesc(student.Year.Value));
                output = output.Replace("[SUBCODE1]", subjectResult.Where(x => x.SubjectMasterId == 1).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE2]", subjectResult.Where(x => x.SubjectMasterId == 2).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE3]", subjectResult.Where(x => x.SubjectMasterId == 3).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE4]", subjectResult.Where(x => x.SubjectMasterId == 4).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE5]", subjectResult.Where(x => x.SubjectMasterId == 5).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE6]", subjectResult.Where(x => x.SubjectMasterId == 6).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE7]", subjectResult.Where(x => x.SubjectMasterId == 7).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());
                output = output.Replace("[SUBCODE8]", subjectResult.Where(x => x.SubjectMasterId == 8).Select(y => y.SubjectMaster.SubjectCode).FirstOrDefault());

                output = output.Replace("[SUBNAME1]", subjectResult.Where(x => x.SubjectMasterId == 1).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME2]", subjectResult.Where(x => x.SubjectMasterId == 2).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME3]", subjectResult.Where(x => x.SubjectMasterId == 3).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME4]", subjectResult.Where(x => x.SubjectMasterId == 4).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME5]", subjectResult.Where(x => x.SubjectMasterId == 5).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME6]", subjectResult.Where(x => x.SubjectMasterId == 6).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME7]", subjectResult.Where(x => x.SubjectMasterId == 7).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());
                output = output.Replace("[SUBNAME8]", subjectResult.Where(x => x.SubjectMasterId == 8).Select(y => y.SubjectMaster.SubjectName).FirstOrDefault());

                output = output.Replace("[MARK1]", subjectResult.Where(x => x.SubjectMasterId == 1).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK2]", subjectResult.Where(x => x.SubjectMasterId == 2).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK3]", subjectResult.Where(x => x.SubjectMasterId == 3).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK4]", subjectResult.Where(x => x.SubjectMasterId == 4).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK5]", subjectResult.Where(x => x.SubjectMasterId == 5).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK6]", subjectResult.Where(x => x.SubjectMasterId == 6).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK7]", subjectResult.Where(x => x.SubjectMasterId == 7).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());
                output = output.Replace("[MARK8]", subjectResult.Where(x => x.SubjectMasterId == 8).Select(y => y.Mark.ToString("0.00")).FirstOrDefault());

                output = output.Replace("[RESULT1]", subjectResult.Where(x => x.SubjectMasterId == 1).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT2]", subjectResult.Where(x => x.SubjectMasterId == 2).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT3]", subjectResult.Where(x => x.SubjectMasterId == 3).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT4]", subjectResult.Where(x => x.SubjectMasterId == 4).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT5]", subjectResult.Where(x => x.SubjectMasterId == 5).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT6]", subjectResult.Where(x => x.SubjectMasterId == 6).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT7]", subjectResult.Where(x => x.SubjectMasterId == 7).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
                output = output.Replace("[RESULT8]", subjectResult.Where(x => x.SubjectMasterId == 8).Select(y => y.ExamResult ? "PASS" : "FAIL").FirstOrDefault());
            }
            else if (docCode == "DOC_003")
            {
                output = output.Replace("[UniversityName]", student.University.UniversityName);
                output = output.Replace("[StudentCode]", student.StudentCode);
                output = output.Replace("[StudentName]", $"{student.FirstName} {student.MiddleName} {student.LastName}");
                output = output.Replace("[DepartmentName]", student.Department.DepartmentName);
                output = output.Replace("[Class]", GetYearDesc(student.Year.Value));                
            }
            else if (docCode == "DOC_004")
            {
                output = output.Replace("[UniversityName]", student.University.UniversityName);
                output = output.Replace("[StudentCode]", student.StudentCode);
                output = output.Replace("[StudentName]", $"{student.FirstName} {student.MiddleName} {student.LastName}");
                output = output.Replace("[DepartmentName]", student.Department.DepartmentName);
                output = output.Replace("[Class]", GetYearDesc(student.Year.Value));
            }
            return output;
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

        private string GetYearDesc(int year)
        {
            return year switch
            {
                1 => "1st Year",
                2 => "2nd Year",
                3 => "3rd Year",
                4 => "4th Year",
                _ => string.Empty,
            };
        }
        private int GetStudentId => Convert.ToInt32(HttpContext.User.FindFirst("StudentId").Value);
    }
}

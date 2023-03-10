using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using University.Portal.Data.Data;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;
using static University.Portal.Data.Data.Enum;

namespace University.Portal.Web.Controllers
{
    [Authorize]
    public class UniversityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UniversityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            int universityId = Convert.ToInt32(HttpContext.User.FindFirst("UniversityId").Value);
            List<StudentGridModel> studentList = new List<StudentGridModel>();

            studentList = (from a in _unitOfWork.StudentRepository.GetByFilter(x => x.UniversityId == universityId, IncludeStr: "Department")
                           select new StudentGridModel
                           {
                               Id = a.Id,
                               StudentCode = a.StudentCode,
                               FullName = $"{a.FirstName} {a.MiddleName} {a.LastName}",
                               Gender = a.Gender == 'M' ? "Male" : "Female",
                               Email = a.Email,
                               PhoneNumber = a.PhoneNumber,
                               DateOfBirth = a.DateOfBirth,
                               DateOfJoining = a.DateOfJoining,
                               DepartmentName = a.Department?.DepartmentName,
                               Year = a.Year != null ? GetYearDesc(a.Year.Value) : string.Empty
                           }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('studentDetails');";

            return View(studentList);
        }

        public IActionResult Create(int id)
        {
            var model = GetStudentRegisterViewModel();

            if (id > 0)
            {
                var student = _unitOfWork.StudentRepository.Get(id);
                model.Id = student.Id;
                model.FirstName = student.FirstName;
                model.LastName = student.LastName;
                model.MiddleName = student.MiddleName;
                model.Email = student.Email;
                model.PhoneNumber = student.PhoneNumber;
                model.DOB = student.DOB;
                model.DOJ = student.DOJ;
                model.GenderId = student.Gender == 'M' ? "M" : "F";
                model.DepartmentId = Convert.ToString(student.DepartmentId.Value);
                model.YearId = Convert.ToString(student.Year.Value);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (model.GenderId == "0")
            {
                ModelState.AddModelError("Gender", "Please select Gender");
            }

            if (model.DepartmentId == "0")
            {
                ModelState.AddModelError("Department", "Please select Department");
            }

            if (model.YearId == "0")
            {
                ModelState.AddModelError("Year", "Please select Class");
            }

            if (model.DOB >= DateTime.Now)
            {
                ModelState.AddModelError("DOB", "Future Date is not allowed");
            }

            if (model.DOJ >= DateTime.Now)
            {
                ModelState.AddModelError("DOJ", "Future Date is not allowed");
            }

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                {
                    int universityId = Convert.ToInt32(HttpContext.User.FindFirst("UniversityId").Value);
                    var allStudentCount = _unitOfWork.StudentRepository.GetAll().Count() + 1;

                    var student = new Student()
                    {
                        StudentCode = "STUD" + allStudentCount.ToString("000"),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        MiddleName = model.MiddleName,
                        Gender = model.GenderId == "M" ? 'M' : 'F',
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        DOB = model.DOB,
                        DOJ = model.DOJ,
                        UniversityId = universityId,
                        DepartmentId = Convert.ToInt32(model.DepartmentId),
                        Year = Convert.ToInt32(model.YearId),
                        Status = true,
                    };

                    using var hmac = new HMACSHA512();

                    var AppUser = new AppUser()
                    {
                        UserName = student.StudentCode,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password@123")),
                        PasswordSalt = hmac.Key,
                        StudentOrUniversity = (int)StudentOrUniversity.Student,
                        UniversityId = universityId,
                        Status = true,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Student = student,
                    };

                    _unitOfWork.UserRepository.Add(AppUser);
                }
                else
                {
                    var student = _unitOfWork.StudentRepository.Get(model.Id);
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.MiddleName = model.MiddleName;
                    student.Email = model.Email;
                    student.PhoneNumber = model.PhoneNumber;
                    student.DOB = model.DOB;
                    student.DOJ = model.DOJ;
                    student.Gender = model.GenderId == "M" ? 'M' : 'F';
                    student.DepartmentId = Convert.ToInt32(model.DepartmentId);
                    student.Year = Convert.ToInt32(model.YearId);
                    _unitOfWork.StudentRepository.Update(student);
                }

                string message = string.Empty;
                if (model.Id <= 0)
                {
                    message = "Student details added successfully.!";
                }
                else
                {
                    message = "Student details updated successfully.!";
                }

                TempData["JavaScriptFunction"] = $"showToastrMessage('{message}','');";

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("Index");
                else
                {
                    return View(GetStudentRegisterViewModel(true));
                }
            }
            else
            {
                return View(GetStudentRegisterViewModel(true));
            }
        }

        public IActionResult TutionFeeDetails()
        {
            var list = (from a in _unitOfWork.FeeDetailsRepository
                        .GetByFilter(x => x.UniversityId == GetUniversityId && x.FeeMasterId == 1 && x.IsActive == true, IncludeStr: "Department")
                        select new FeeGridModel
                        {
                            Id = a.Id,
                            DepartmentName = a.Department.DepartmentName,
                            Year = GetYearDesc(a.Year),
                            Amount = a.Amount,
                            DueDate = a.DueDate
                        }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('tutionFeeDetails');";

            return View(list);
        }

        public IActionResult AddEditTutionFeeDetails(int id)
        {
            var model = GetTutionFeeViewModel();

            if (id > 0)
            {
                var tuttionFee = _unitOfWork.FeeDetailsRepository.Get(id);
                model.Id = tuttionFee.Id;
                model.YearId = Convert.ToString(tuttionFee.Year);
                model.Amount = tuttionFee.Amount.ToString("0.00");
                model.DueDate = tuttionFee.DueDate;
                model.DepartmentId = Convert.ToString(tuttionFee.DepartmentId);
            }
            
            TempData["SetActiveTab"] = $"setActiveTabClass('tutionFeeDetails');";
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditTutionFeeDetails(FeeDetailsViewModel model)
        {
            var tutionFee = new FeeDetails();

            if (model.DepartmentId == "0")
            {
                ModelState.AddModelError("Department", "Please select Department");
            }

            if (model.YearId == "0")
            {
                ModelState.AddModelError("Year", "Please select Class");
            }

            if (ModelState.IsValid)
            {

                if (model.Id <= 0)
                {
                    var tutionFeeMasterId = _unitOfWork.FeeMasterRepository
                        .GetByFilter(x => x.FeeType == "TutionFee")
                        .Select(x => x.Id).FirstOrDefault();

                    tutionFee = new FeeDetails()
                    {
                        FeeMasterId = tutionFeeMasterId,
                        UniversityId = GetUniversityId,
                        DepartmentId = Convert.ToInt32(model.DepartmentId),
                        Year = Convert.ToInt32(model.YearId),
                        Amount = Convert.ToDecimal(model.Amount.Replace("₹", "")),
                        DueDate = model.DueDate.Value,
                        IsActive = true
                    };

                    _unitOfWork.FeeDetailsRepository.Add(tutionFee);
                }
                else
                {
                    tutionFee = _unitOfWork.FeeDetailsRepository.Get(model.Id);
                    tutionFee.DepartmentId = Convert.ToInt32(model.DepartmentId);
                    tutionFee.Year = Convert.ToInt32(model.YearId);
                    tutionFee.Amount = Convert.ToDecimal(model.Amount.Replace("₹", ""));
                    tutionFee.DueDate = model.DueDate.Value;

                    _unitOfWork.FeeDetailsRepository.Update(tutionFee);
                }

                string message = string.Empty;
                if (model.Id <= 0)
                {
                    message = "Tution Fee details added successfully.!";
                }
                else
                {
                    message = "Tution Fee details updated successfully.!";
                }

                TempData["JavaScriptFunction"] = $"showToastrMessage('{message}','');";

                SendTutionFeeNotification(tutionFee);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("TutionFeeDetails");
                else
                {
                    return View(GetTutionFeeViewModel(true));
                }
            }
            else
            {
                return View(GetTutionFeeViewModel());
            }
        }

        public IActionResult ViewPayment(int id)
        {
            List<PendingPaymentViewModel> model = new();

            var feeDetails = _unitOfWork.FeeDetailsRepository.GetByFilter(x => x.Id == id, IncludeStr: "FeeMaster").FirstOrDefault();

            var feePaymentList = _unitOfWork.FeePaymentRepository
                .GetByFilter(x => x.FeeDetailsId == id).ToList();

            var studentList = _unitOfWork.StudentRepository
                .GetByFilter(x => x.UniversityId == feeDetails.UniversityId && x.DepartmentId == feeDetails.DepartmentId
                             && x.Year == feeDetails.Year, IncludeStr: "Department").ToList();

            List<PendingPaymentViewModel> list = (from a in studentList
                                                  let feePayment = feePaymentList.Where(x => x.StudentID == a.Id).FirstOrDefault()
                                                  select new PendingPaymentViewModel
                                                  {


                                                      StudentId = a.Id,
                                                      StudentCode = a.StudentCode,
                                                      StudentName = $"{a.FirstName} {a.LastName}",
                                                      Department = a.Department.DepartmentName,
                                                      Amount = feeDetails.Amount,
                                                      DueDate = feeDetails.DueDate,
                                                      FeeType = feeDetails.FeeMaster.FeeType,
                                                      IsPaymentCompleted = feePayment != null ? true : false,
                                                      PaymentDate = feePayment != null ? feePayment.PaymentDate.ToString("MM/dd/yyyy") : string.Empty,
                                                  }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('feeDetails');";

            return View(list);
        }

        public async Task<IActionResult> SendReminder(int id)
        {
            var notification = new Notification()
            {
                StudentID = id,
                StudentOrUniversity = (int)StudentOrUniversity.Student,
                Message = $"Reminder: Please pay the total amount on or before due date."
            };

            _unitOfWork.NotificationRepository.Add(notification);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('Reminder sent successfully!','');";

            return RedirectToAction("TutionFeeDetails");
        }

        public async Task<IActionResult> PublishHallTicket(int id)
        {
            var hallticketMasterId = _unitOfWork.DocumentMasterRepository.GetByFilter(x => x.DocCode == "DOC_001").Select(y => y.Id).FirstOrDefault();

            var studentDocument = new StudentDocument()
            {
                StudentId = id,
                DocumentMasterId = hallticketMasterId,
                IsActive = true,
            };

            _unitOfWork.StudentDocumentRepository.Add(studentDocument);

            SendDocumentPublishNotification(studentDocument);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('document added successfully!','');";

            return RedirectToAction("ExamFeeDetails");
        }

        public IActionResult ExamSchedule()
        {
            var examScheduleDetails = (from a in _unitOfWork.ExamScheduleRepository
                .GetByFilter(x => x.UniversityId == GetUniversityId && x.IsActive == true, IncludeStr: "Department")
                                       select new ExamScheduleListViewModel()
                                       {
                                           Id = a.Id,
                                           Department = a.Department.DepartmentName,
                                           Year =  GetYearDesc(a.Year),
                                           StartDate = a.StartDate,
                                           EndDate = a.EndDate,
                                       }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('examSchedule');";

            return View(examScheduleDetails);
        }

        public ActionResult AddEditExamSchedule(int id)
        {
            ExamScheduleViewModel model = GetExamScheduleViewModel();
            if (id > 0)
            {
                var examSchedule = _unitOfWork.ExamScheduleRepository.Get(id);
                model.Id = id;
                model.DepartmentId = examSchedule.DepartmentId.ToString();
                model.YearId = examSchedule.Year.ToString();
                model.StartDate = examSchedule.StartDate;
                model.EndDate = examSchedule.EndDate;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddEditExamSchedule(ExamScheduleViewModel model)
        {
            var examSchedule = new ExamSchedule();
            if (model.YearId == "0")
            {
                ModelState.AddModelError("Year", "Please select Class");
            }

            if (model.DepartmentId == "0")
            {
                ModelState.AddModelError("Department", "Please select Department");
            }

            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                {
                    examSchedule = new ExamSchedule()
                    {
                        UniversityId = GetUniversityId,
                        DepartmentId = Convert.ToInt32(model.DepartmentId),
                        Year = Convert.ToInt32(model.YearId),
                        StartDate = model.StartDate.Value,
                        EndDate = model.EndDate.Value,
                        IsActive = true
                    };

                    _unitOfWork.ExamScheduleRepository.Add(examSchedule);
                }
                else
                {
                    examSchedule = _unitOfWork.ExamScheduleRepository.Get(model.Id);
                    examSchedule.DepartmentId = Convert.ToInt32(model.DepartmentId);
                    examSchedule.Year = Convert.ToInt32(model.YearId);
                    examSchedule.StartDate = model.StartDate.Value;
                    examSchedule.EndDate = model.EndDate.Value;

                    _unitOfWork.ExamScheduleRepository.Update(examSchedule);
                }

                string message = string.Empty;
                if (model.Id <= 0)
                {
                    message = "Exam schedule details added successfully.!";
                }
                else
                {
                    message = "Exam schedule details updated successfully.!";
                }

                TempData["JavaScriptFunction"] = $"showToastrMessage('{message}','');";

                SendExamScheduleNotification(examSchedule);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("ExamSchedule");
                else
                {
                    return View(GetExamScheduleViewModel());
                }
            }
            else
            {
                return View(GetExamScheduleViewModel());
            }
        }

        public IActionResult ExamFeeDetails()
        {
            var list = (from a in _unitOfWork.FeeDetailsRepository
                        .GetByFilter(x => x.UniversityId == GetUniversityId && x.FeeMasterId == 2 && x.IsActive == true, IncludeStr: "Department")
                        select new FeeGridModel
                        {
                            Id = a.Id,
                            DepartmentName = a.Department.DepartmentName,
                            Year = GetYearDesc(a.Year),
                            Amount = a.Amount,
                            DueDate = a.DueDate
                        }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('feeDetails');";

            return View(list);
        }

        public IActionResult AddEditExamFeeDetails(int id)
        {
            var model = GetTutionFeeViewModel();

            if (id > 0)
            {
                var tuttionFee = _unitOfWork.FeeDetailsRepository.Get(id);
                model.Id = tuttionFee.Id;
                model.YearId = Convert.ToString(tuttionFee.Year);
                model.Amount = tuttionFee.Amount.ToString("0.00");
                model.DueDate = tuttionFee.DueDate;
                model.DepartmentId = Convert.ToString(tuttionFee.DepartmentId);
            }

            TempData["SetActiveTab"] = $"setActiveTabClass('feeDetails');";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditExamFeeDetails(FeeDetailsViewModel model)
        {
            var tutionFee = new FeeDetails();

            if (model.DepartmentId == "0")
            {
                ModelState.AddModelError("Department", "Please select Department");
            }

            if (model.YearId == "0")
            {
                ModelState.AddModelError("Year", "Please select Class");
            }

            if (ModelState.IsValid)
            {

                if (model.Id <= 0)
                {
                    var tutionFeeMasterId = _unitOfWork.FeeMasterRepository
                        .GetByFilter(x => x.FeeType == "ExamFee")
                        .Select(x => x.Id).FirstOrDefault();

                    tutionFee = new FeeDetails()
                    {
                        FeeMasterId = tutionFeeMasterId,
                        UniversityId = GetUniversityId,
                        DepartmentId = Convert.ToInt32(model.DepartmentId),
                        Year = Convert.ToInt32(model.YearId),
                        Amount = Convert.ToDecimal(model.Amount.Replace("₹", "")),
                        DueDate = model.DueDate.Value,
                        IsActive = true
                    };

                    _unitOfWork.FeeDetailsRepository.Add(tutionFee);
                }
                else
                {
                    tutionFee = _unitOfWork.FeeDetailsRepository.Get(model.Id);
                    tutionFee.DepartmentId = Convert.ToInt32(model.DepartmentId);
                    tutionFee.Year = Convert.ToInt32(model.YearId);
                    tutionFee.Amount = Convert.ToDecimal(model.Amount.Replace("₹", ""));
                    tutionFee.DueDate = model.DueDate.Value;

                    _unitOfWork.FeeDetailsRepository.Update(tutionFee);
                }

                string message = string.Empty;
                if (model.Id <= 0)
                {
                    message = "Exam Fee details added successfully.!";
                }
                else
                {
                    message = "Exam Fee details updated successfully.!";
                }

                TempData["JavaScriptFunction"] = $"showToastrMessage('{message}','');";

                SendTutionFeeNotification(tutionFee);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("ExamFeeDetails");
                else
                {
                    return View(GetTutionFeeViewModel(true));
                }
            }
            else
            {
                return View(GetTutionFeeViewModel());
            }
        }

        public IActionResult PublishExamResult()
        {
            var feePaymentDetails = (from a in _unitOfWork.FeePaymentRepository
                                     .GetByFilter(x => x.FeeDetails.UniversityId == GetUniversityId && x.FeeDetails.FeeMasterId == 2, IncludeStr: "FeeDetails")
                                     select a.StudentID).ToList();

            var examResultList = _unitOfWork.ExamResultRepository.GetAll();

            var studentList = (from a in _unitOfWork.StudentRepository
                            .GetByFilter(x => feePaymentDetails.Contains(x.Id), IncludeStr: "Department")
                               let examResult = examResultList.Where(x=>x.StudentId == a.Id).FirstOrDefault()
                               select new ExamResultGridViewModel()
                               {
                                   Id = a.Id,
                                   StudentCode = a.StudentCode,
                                   StudentName = $"{a.FirstName} {a.LastName}",
                                   Department = a.Department.DepartmentName,
                                   Class = GetYearDesc(a.Year.Value),
                                   IsResultPublished = examResult != null ? true : false,
                               }).ToList();

            TempData["SetActiveTab"] = $"setActiveTabClass('publishExamResult');";

            return View(studentList);
        }

        public IActionResult ViewResult(int id)
        {
            var departmentList = _unitOfWork.DepartmentRepository.GetAll();

            var examResultViewModel = (from a in _unitOfWork.SubjectResultRepository.GetByFilter(x => x.StudentId == id, IncludeStr: "Student,SubjectMaster")
                                       let department = departmentList.Where(x=>x.Id == a.Student.DepartmentId).FirstOrDefault()
                                       select new ViewExamResultModel() 
                                       {
                                           StudentName = $"{a.Student.FirstName} {a.Student.MiddleName} {a.Student.LastName}",
                                           Class = GetYearDesc(a.Student.Year.Value),
                                           Department = department != null ? department.DepartmentName : string.Empty,
                                           SubjectCode = a.SubjectMaster.SubjectCode,
                                           SubjectName= a.SubjectMaster.SubjectName,
                                           Mark = a.Mark.ToString("0.00"),
                                           Result = a.ExamResult ? "Pass" : "Fail"
                                       }).ToList();

            return View(examResultViewModel);
        }

        public IActionResult AddEditExamResult(int id)
        {
            var model = GetExamResultViewModel();
            model.Id = id;
            return View(model);
        }

        public IActionResult AddEditExamResultNew(int id)
        {
            var model = GetExamResultNewViewModel(id);            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditExamResultNew(ExamResultNewViewModel model)
        {            
            var subjectResult = new SubjectResult();
            if (ModelState.IsValid)
            {
                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId1,
                    StudentId = model.Id,
                    Mark = model.SubjectValue1,
                    ExamResult = model.SubjectValue1 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId2,
                    StudentId = model.Id,
                    Mark = model.SubjectValue2,
                    ExamResult = model.SubjectValue2 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId3,
                    StudentId = model.Id,
                    Mark = model.SubjectValue3,
                    ExamResult = model.SubjectValue3 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId4,
                    StudentId = model.Id,
                    Mark = model.SubjectValue4,
                    ExamResult = model.SubjectValue4 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId5,
                    StudentId = model.Id,
                    Mark = model.SubjectValue5,
                    ExamResult = model.SubjectValue5 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId6,
                    StudentId = model.Id,
                    Mark = model.SubjectValue6,
                    ExamResult = model.SubjectValue6 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId7,
                    StudentId = model.Id,
                    Mark = model.SubjectValue7,
                    ExamResult = model.SubjectValue7 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                subjectResult = new SubjectResult()
                {
                    SubjectMasterId = model.SubjectMasterId8,
                    StudentId = model.Id,
                    Mark = model.SubjectValue8,
                    ExamResult = model.SubjectValue8 <= 4 ? false : true,
                    IsActive = true,
                };

                _unitOfWork.SubjectResultRepository.Add(subjectResult);

                var examResult = new ExamResult()
                {
                    StudentId = model.Id,
                    Result = (model.SubjectValue1 <= 4 || model.SubjectValue2 <= 4 || model.SubjectValue3 <= 4 || model.SubjectValue4 <= 4 || model.SubjectValue5 <= 4 || model.SubjectValue6 <= 4 || model.SubjectValue7 <= 4 || model.SubjectValue8 <= 4) ? false : true,
                    CGPA = (model.SubjectValue1 + model.SubjectValue2 + model.SubjectValue3 + model.SubjectValue4 + model.SubjectValue5 + model.SubjectValue6 + model.SubjectValue7 + model.SubjectValue8) / 8,
                    IsActive = true
                };

                TempData["JavaScriptFunction"] = $"showToastrMessage('Exam result published successfully!','');";

                SendExamResultAnnouncement(examResult);

                _unitOfWork.ExamResultRepository.Add(examResult);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("PublishExamResult");
                else
                {
                    return View(GetExamResultNewViewModel(model.Id));
                }
            }
            else
            {
                return View(GetExamResultNewViewModel(model.Id));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEditExamResult(ExamResultViewModel model)
        {
            var examResult = new ExamResult();
            if (model.ResultId == "0")
            {
                ModelState.AddModelError("Year", "Please select result");
            }

            if (ModelState.IsValid)
            {
                examResult = _unitOfWork.ExamResultRepository.GetByFilter(x => x.StudentId == model.Id).FirstOrDefault();

                if (examResult != null)
                {
                    examResult.CGPA = model.CGPA.Value;
                    examResult.Result = model.ResultId.ToString() == "1" ? true : false;
                    _unitOfWork.ExamResultRepository.Update(examResult);
                }
                else
                {
                    examResult = new ExamResult()
                    {
                        StudentId = model.Id,
                        Result = model.ResultId.ToString() == "1" ? true : false,
                        CGPA = model.CGPA.Value,
                        IsActive = true
                    };

                    _unitOfWork.ExamResultRepository.Add(examResult);
                }

                TempData["JavaScriptFunction"] = $"showToastrMessage('Exam result published successfully!','');";

                SendExamResultAnnouncement(examResult);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("PublishExamResult");
                else
                {
                    return View(GetExamScheduleViewModel());
                }
            }
            else
            {
                return View(GetExamResultViewModel());
            }
        }

        public IActionResult PublishDocuments()
        {
            List<PublishDocumentViewModel> model = new();

            var examResult = _unitOfWork.ExamResultRepository
                .GetByFilter(x => x.Student.UniversityId == GetUniversityId, IncludeStr: "Student").ToList();

            var documentMaster = _unitOfWork.DocumentMasterRepository.GetByFilter(x => x.DocCode != "DOC_001").ToList();

            var departmentMaster = _unitOfWork.DepartmentRepository.GetAll().ToList();

            foreach (var student in examResult)
            {
                foreach (var document in documentMaster)
                {
                    var item = new PublishDocumentViewModel()
                    {
                        Id = student.StudentId,
                        DocumentId = document.Id,
                        StudentName = $"{student.Student.FirstName} {student.Student.LastName}",
                        Department = departmentMaster.Where(x => x.Id == student.Student.DepartmentId).Select(y => y.DepartmentName).FirstOrDefault(),
                        Class = GetYearDesc(student.Student.Year.Value),
                        Document = document.DocName
                    };
                    model.Add(item);
                }
            }

            TempData["SetActiveTab"] = $"setActiveTabClass('publishDocuments');";

            return View(model);
        }

        public async Task<IActionResult> PublishDocument(int id, int documentId)
        {

            var studentDocument = new StudentDocument()
            {
                StudentId = id,
                DocumentMasterId = documentId,
                IsActive = true
            };

            _unitOfWork.StudentDocumentRepository.Add(studentDocument);

            SendDocumentPublishNotification(studentDocument);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('document added successfully!','');";

            return RedirectToAction("PublishDocuments");
        }

        public IActionResult VerifyDocuments()
        {
            var model = new List<DocumentVerificationGrid>();

            var uploadDocumentList = (from stud in _unitOfWork.StudentRepository.GetByFilter(x=>x.UniversityId == GetUniversityId, IncludeStr: "Department")
                                      join doc in _unitOfWork.UploadDocumentRepository.GetAll() on stud.Id equals doc.StudentId
                                      select new DocumentVerificationGrid
                                      {
                                          Id = doc.Id,
                                          StudentName = $"{stud.FirstName} {stud.MiddleName} {stud.LastName}",
                                          Department = stud.Department.DepartmentName,
                                          Class = GetYearDesc(stud.Year.Value),
                                          DocumentType = doc.DocumentType,
                                          DocumentName = doc.DocumentName,
                                          UploadedOn = doc.CreatedOn.ToString("MM/dd/yyyy"),
                                          Status = doc.Status != null ? (doc.Status.Value ? "Accepted" : "Rejected") : string.Empty
                                      }
                                      ).ToList();

            model = uploadDocumentList;

            TempData["SetActiveTab"] = $"setActiveTabClass('verifyDocument');";

            return View(model);
        }

        
        public async Task<IActionResult> AcceptUploadedDocument(int Id)
        {
            var uploadDocument = _unitOfWork.UploadDocumentRepository.Get(Id);
            uploadDocument.Status = true;

            _unitOfWork.UploadDocumentRepository.Update(uploadDocument);

            SendUploadDocumentAcceptRejectNotification(uploadDocument);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('document acceepted successfully!','');";

            return RedirectToAction("VerifyDocuments");
        }
        
        public async Task<IActionResult> RejectUploadedDocument(int Id)
        {
            var uploadDocument = _unitOfWork.UploadDocumentRepository.Get(Id);
            uploadDocument.Status = false;

            _unitOfWork.UploadDocumentRepository.Update(uploadDocument);
            
            SendUploadDocumentAcceptRejectNotification(uploadDocument);

            await _unitOfWork.CompleteAsync();

            TempData["JavaScriptFunction"] = $"showToastrMessage('document rejected successfully!','');";

            return RedirectToAction("VerifyDocuments");
        }

        private StudentViewModel GetStudentRegisterViewModel(bool isRegistrationFailed = false)
        {
            var studentRegisteVM = new StudentViewModel();
            var DeptList = _unitOfWork.DepartmentRepository.GetAll();
            studentRegisteVM.Department = GetDepartmentDropDownItems(DeptList);
            studentRegisteVM.Gender = GetGenderDropDownItems();
            studentRegisteVM.Year = GetYearDropDownItems();
            studentRegisteVM.IsRegistrationFailed = isRegistrationFailed;
            return studentRegisteVM;
        }

        private FeeDetailsViewModel GetTutionFeeViewModel(bool isRegistrationFailed = false)
        {
            var tutionFeeRegisteVM = new FeeDetailsViewModel();
            var DeptList = _unitOfWork.DepartmentRepository.GetAll();
            tutionFeeRegisteVM.Department = GetDepartmentDropDownItems(DeptList);
            tutionFeeRegisteVM.Year = GetYearDropDownItems();
            return tutionFeeRegisteVM;
        }

        private ExamScheduleViewModel GetExamScheduleViewModel()
        {
            var model = new ExamScheduleViewModel();
            model.Year = GetYearDropDownItems();
            var DeptList = _unitOfWork.DepartmentRepository.GetAll();
            model.Department = GetDepartmentDropDownItems(DeptList);
            return model;
        }

        private ExamResultViewModel GetExamResultViewModel()
        {
            var model = new ExamResultViewModel();
            model.Result = GetResultDropDownItems();
            return model;
        }

        private ExamResultNewViewModel GetExamResultNewViewModel(int id)
        {
            var model = new ExamResultNewViewModel();
            model.Id = id;

            var student = _unitOfWork.StudentRepository.Get(id);

            var subjectMasterList = _unitOfWork.SubjectMasterRepository
                .GetByFilter(x => x.DepartmentId == student.DepartmentId && x.Year == student.Year).ToList();

            int i = 0;

            foreach (var subjectMaster in subjectMasterList)
            {
                if (i == 0)
                {
                    model.SubjectMasterId1 = subjectMaster.Id;
                    model.SubjectName1 = subjectMaster.SubjectName;
                }
                if (i == 1)
                {
                    model.SubjectMasterId2 = subjectMaster.Id;
                    model.SubjectName2 = subjectMaster.SubjectName;
                }
                if (i == 2)
                {
                    model.SubjectMasterId3 = subjectMaster.Id;
                    model.SubjectName3 = subjectMaster.SubjectName;
                }
                if (i == 3)
                {
                    model.SubjectMasterId4 = subjectMaster.Id;
                    model.SubjectName4 = subjectMaster.SubjectName;
                }
                if (i == 4)
                {
                    model.SubjectMasterId5 = subjectMaster.Id;
                    model.SubjectName5 = subjectMaster.SubjectName;
                }
                if (i == 5)
                {
                    model.SubjectMasterId6 = subjectMaster.Id;
                    model.SubjectName6 = subjectMaster.SubjectName;
                }
                if (i == 6)
                {
                    model.SubjectMasterId7 = subjectMaster.Id;
                    model.SubjectName7 = subjectMaster.SubjectName;
                }
                if (i == 7)
                {
                    model.SubjectMasterId8 = subjectMaster.Id;
                    model.SubjectName8 = "Electronic Circuits";
                }
                i++;
            }

            return model;
        }

        private List<SelectListItem> GetDepartmentDropDownItems(List<Department> list)
        {
            List<SelectListItem> dropdownList = new();
            dropdownList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
            foreach (var item in list)
            {
                dropdownList.Add(new SelectListItem { Text = item.DepartmentName, Value = item.Id.ToString() });
            }
            return dropdownList;
        }

        private List<SelectListItem> GetGenderDropDownItems()
        {
            List<SelectListItem> dropdownList = new();
            dropdownList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
            dropdownList.Add(new SelectListItem { Text = "Male", Value = "M" });
            dropdownList.Add(new SelectListItem { Text = "Female", Value = "F" });
            return dropdownList;
        }

        private List<SelectListItem> GetYearDropDownItems()
        {
            List<SelectListItem> dropdownList = new();
            dropdownList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
            dropdownList.Add(new SelectListItem { Text = "1st Year", Value = "1" });
            dropdownList.Add(new SelectListItem { Text = "2nd Year", Value = "2" });
            dropdownList.Add(new SelectListItem { Text = "3rd Year", Value = "3" });
            dropdownList.Add(new SelectListItem { Text = "4th Year", Value = "4" });
            return dropdownList;
        }

        private List<SelectListItem> GetResultDropDownItems()
        {
            List<SelectListItem> dropdownList = new();
            dropdownList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
            dropdownList.Add(new SelectListItem { Text = "Pass", Value = "1" });
            dropdownList.Add(new SelectListItem { Text = "Fail", Value = "2" });
            return dropdownList;
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

        private int GetUniversityId => Convert.ToInt32(HttpContext.User.FindFirst("UniversityId").Value);
        private int GetUserId => Convert.ToInt32(HttpContext.User.FindFirst("UserId").Value);

        private void SendTutionFeeNotification(FeeDetails tutionFeeDetails)
        {
            List<Notification> notificationList = new();

            var studentIdList = (from a in _unitOfWork.StudentRepository
                               .GetByFilter(x => x.UniversityId == GetUniversityId && x.Year == tutionFeeDetails.Year && x.DepartmentId == tutionFeeDetails.DepartmentId)
                                 select a.Id).ToList();

            foreach (var studentId in studentIdList)
            {
                var notification = new Notification()
                {
                    StudentID = studentId,
                    StudentOrUniversity = (int)StudentOrUniversity.Student,
                    Message = $"Tution Fee details published. Amount is Rs.{tutionFeeDetails.Amount.ToString("0.00")} and due date is on {tutionFeeDetails.DueDate.ToString("MM/dd/yyyy")}" +
                              $".<br/> Please pay the total amount on or before due date."
                };

                _unitOfWork.NotificationRepository.Add(notification);
            }
        }

        private void SendExamFeeNotification(FeeDetails tutionFeeDetails)
        {
            List<Notification> notificationList = new();

            var studentIdList = (from a in _unitOfWork.StudentRepository
                               .GetByFilter(x => x.UniversityId == GetUniversityId && x.Year == tutionFeeDetails.Year && x.DepartmentId == tutionFeeDetails.DepartmentId)
                                 select a.Id).ToList();

            foreach (var studentId in studentIdList)
            {
                var notification = new Notification()
                {
                    StudentID = studentId,
                    StudentOrUniversity = (int)StudentOrUniversity.Student,
                    Message = $"Exam Fee details published. Amount is Rs.{tutionFeeDetails.Amount.ToString("0.00")} and due date is on {tutionFeeDetails.DueDate.ToString("MM/dd/yyyy")}" +
                              $".<br/> Please pay the total amount on or before due date."
                };

                _unitOfWork.NotificationRepository.Add(notification);
            }
        }

        private void SendExamScheduleNotification(ExamSchedule examSchedule)
        {
            List<Notification> notificationList = new();

            var studentIdList = (from a in _unitOfWork.StudentRepository
                               .GetByFilter(x => x.UniversityId == GetUniversityId && x.Year == examSchedule.Year && x.DepartmentId == examSchedule.DepartmentId)
                                 select a.Id).ToList();

            foreach (var studentId in studentIdList)
            {
                var notification = new Notification()
                {
                    StudentID = studentId,
                    StudentOrUniversity = (int)StudentOrUniversity.Student,
                    Message = $"Exam Scheduled between {examSchedule.StartDate.ToString("MM/dd/yyyy")} and {examSchedule.EndDate.ToString("MM/dd/yyyy")}"
                };

                _unitOfWork.NotificationRepository.Add(notification);
            }
        }

        private void SendExamResultAnnouncement(ExamResult examResult)
        {
            var notification = new Notification()
            {
                StudentID = examResult.StudentId,
                StudentOrUniversity = (int)StudentOrUniversity.Student,
                Message = $"Exam result published on {DateTime.Now.ToString("MM/dd/yyyy")}.You can view result in View Result tab."
            };

            _unitOfWork.NotificationRepository.Add(notification);
        }

        private void SendDocumentPublishNotification(StudentDocument studentDocument)
        {
            var document = _unitOfWork.DocumentMasterRepository.Get(studentDocument.DocumentMasterId);

            var notification = new Notification()
            {
                StudentID = studentDocument.StudentId,
                StudentOrUniversity = (int)StudentOrUniversity.Student,
                Message = $"{document.DocName} added in your document folder.You can download in Download Documents tab."
            };

            _unitOfWork.NotificationRepository.Add(notification);
        }

        private void SendUploadDocumentAcceptRejectNotification(UploadDocument uploadDocument)
        {
            var notification = new Notification()
            {
                StudentID = uploadDocument.StudentId,
                StudentOrUniversity = (int)StudentOrUniversity.Student,
                Message = $"{uploadDocument.DocumentName} { (uploadDocument.Status.Value ? "Accepted" : "Rejected") } by Admin on {DateTime.Now.ToString("MM/dd/yyyy")}"
            };

            _unitOfWork.NotificationRepository.Add(notification);
        }
    }
}

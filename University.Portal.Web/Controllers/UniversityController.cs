using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text;
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
            var list = (from a in _unitOfWork.TutionFeeDetailsRepository
                        .GetByFilter(x => x.UniversityId == GetUniversityId && x.IsActive == true, IncludeStr: "Department")
                        select new TutionFeeGridModel
                        {
                            Id = a.Id,
                            DepartmentName = a.Department.DepartmentName,
                            Year = GetYearDesc(a.Year),
                            Amount = a.Amount,
                            DueDate = a.DueDate
                        }).ToList();

            return View(list);
        }

        public IActionResult AddEditTutionFeeDetails(int id)
        {
            var model = GetTutionFeeViewModel();

            if (id > 0)
            {
                var tuttionFee = _unitOfWork.TutionFeeDetailsRepository.Get(id);
                model.Id = tuttionFee.Id;
                model.YearId = Convert.ToString(tuttionFee.Year);
                model.Amount = tuttionFee.Amount.ToString("0.00");
                model.DueDate = tuttionFee.DueDate;
                model.DepartmentId = Convert.ToString(tuttionFee.DepartmentId);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditTutionFeeDetails(TutionFeeDetailsViewModel model)
        {
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
                    var tutionFee = new TutionFeeDetails()
                    {
                        UniversityId = GetUniversityId,
                        DepartmentId = Convert.ToInt32(model.DepartmentId),
                        Year = Convert.ToInt32(model.YearId),
                        Amount = Convert.ToDecimal(model.Amount.Replace("₹", "")),
                        DueDate = model.DueDate.Value,
                        IsActive = true
                    };

                    _unitOfWork.TutionFeeDetailsRepository.Add(tutionFee);
                }
                else
                {
                    var tutionFee = _unitOfWork.TutionFeeDetailsRepository.Get(model.Id);
                    tutionFee.DepartmentId = Convert.ToInt32(model.DepartmentId);
                    tutionFee.Year = Convert.ToInt32(model.YearId);
                    tutionFee.Amount = Convert.ToDecimal(model.Amount.Replace("₹", ""));
                    tutionFee.DueDate = model.DueDate.Value;

                    _unitOfWork.TutionFeeDetailsRepository.Update(tutionFee);
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

                

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("TutionFeeDetails");
                else
                {
                    return View(GetStudentRegisterViewModel(true));
                }
            }
            else
            {
                return View(GetTutionFeeViewModel());
            }
        }

        public IActionResult ExamScheduleNotification()
        {
            return View();
        }

        public IActionResult ExamFeeCollection()
        {
            return View();
        }

        public IActionResult PublishExamResult()
        {
            return View();
        }

        public IActionResult PublishDocuments()
        {
            return View();
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

        private TutionFeeDetailsViewModel GetTutionFeeViewModel(bool isRegistrationFailed = false)
        {
            var tutionFeeRegisteVM = new TutionFeeDetailsViewModel();
            var DeptList = _unitOfWork.DepartmentRepository.GetAll();
            tutionFeeRegisteVM.Department = GetDepartmentDropDownItems(DeptList);
            tutionFeeRegisteVM.Year = GetYearDropDownItems();
            return tutionFeeRegisteVM;
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

    }
}

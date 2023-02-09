using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;
using University.Portal.Data.Data;
using University.Portal.Data.Data.Models;
using University.Portal.Data.Data.ViewModels;
using University.Portal.Data.Interface;
using static University.Portal.Data.Data.Enum;

namespace University.Portal.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            var model = GetRegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UniversityRegisterViewModel model)
        {
            if (model.RoleId == "0")
            {
                ModelState.AddModelError("College", "Please select any College Name");
            }

            if (ModelState.IsValid)
            {
                var userFromDb = await _unitOfWork.UserRepository.GetByFilterAsync(x => x.UserName == model.UserName);

                if (userFromDb.Count > 0)
                {
                    ModelState.AddModelError("UserNameMatchError", "UserName is already exists..!");
                    return View(GetRegisterViewModel(true));
                }

                var allUniversityUsers = await _unitOfWork.UserRepository.GetByFilterAsync(x => x.UniversityId != null, IncludeStr: "University,AppUserRole");
                var roleMaster = await _unitOfWork.RoleRepository.GetAllAsync();

                if (allUniversityUsers.Any(x => x?.University?.UniversityName.ToLower() == model.University.ToLower() && x.AppUserRole.RoleID == Convert.ToInt32(model.RoleId)))
                {
                    ModelState.AddModelError("UniversitySelectionError", "Selected university is already registered with same role. Please contact administrator.");
                    return View(GetRegisterViewModel(true));
                }

                var university = new UniversityMaster
                {
                    UniversityName = model.University.ToUpper(),
                };                

                var appUserRole = new AppUserRole
                {
                    RoleID = Convert.ToInt32(model.RoleId)
                };

                using var hmac = new HMACSHA512();

                var user = new AppUser
                {
                    UserName = model.UserName.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                    PasswordSalt = hmac.Key,
                    StudentOrUniversity = (int)StudentOrUniversity.University,
                    University = university,
                    Status = true,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AppUserRole = appUserRole,
                };

                _unitOfWork.UserRepository.Add(user);

                if (await _unitOfWork.CompleteAsync()) return RedirectToAction("Index");
                else
                {
                    return View(GetRegisterViewModel(true));
                }
            }
            else
            {
                return View(GetRegisterViewModel(true));
            }
        }

        private UniversityRegisterViewModel GetRegisterViewModel(bool isRegistrationFailed = false)
        {
            var universityRegisteVM = new UniversityRegisterViewModel();
            var roleList = _unitOfWork.RoleRepository.GetAll();
            universityRegisteVM.Role = GetDropDownItems(roleList);
            universityRegisteVM.IsRegistrationFailed = isRegistrationFailed;
            return universityRegisteVM;
        }

        private List<SelectListItem> GetDropDownItems(List<Role> list)
        {
            List<SelectListItem> dropdownList = new();
            dropdownList.Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });
            foreach (var item in list)
            {
                dropdownList.Add(new SelectListItem { Text = item.RoleName, Value = item.Id.ToString() });
            }
            return dropdownList;
        }


    }
}

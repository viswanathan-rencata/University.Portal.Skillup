using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
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

                var universityList = await _unitOfWork.UniversityRepository.GetByFilterAsync(x => x.UniversityName.ToLower() == model.University.ToLower());

                var university = universityList.FirstOrDefault();
                if(university is null)
                {
                    university = new UniversityMaster
                    {
                        UniversityName = model.University.ToUpper(),
                    };
                }                              

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
                
                TempData["JavaScriptFunction"] = $"showToastrMessage('Registration completed successfully!','');";

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

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userList = await _unitOfWork.UserRepository.GetByFilterAsync(x => x.UserName == model.UserName 
                                                && x.StudentOrUniversity == (int)StudentOrUniversity.University, IncludeStr: "University");
                var user = userList.FirstOrDefault();

                if (user == null)
                {
                    ModelState.AddModelError("PasswordMatchError", "UserName/Password is incorrect");
                    model.IsLoginSucceed = false;
                    return Json("UserName/Password is incorrect");
                }
                else
                {
                    if (!user.Status)
                    {
                        ModelState.AddModelError("UserInactiveError", $"{model.UserName} is inactive.Please contact administrator. ");
                        model.IsLoginSucceed = false;
                        return Json($"{model.UserName} is inactive.Please contact administrator. ");
                    }                    
                }

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

                if (!computedHash.SequenceEqual(user.PasswordHash))
                {
                    ModelState.AddModelError("PasswordMatchError", "UserName/Password is incorrect");
                    model.IsLoginSucceed = false;
                    return Json("UserName/Password is incorrect");
                }
                var userRoleList = await _unitOfWork.AppUserRoleRepository.GetByFilterAsync(x => x.AppUserID == user.Id, IncludeStr:"Role");
                var userRole = userRoleList.FirstOrDefault();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("StudentOrUniversity", user.StudentOrUniversity.ToString()),
                    new Claim("UniversityId", user.UniversityId.ToString()),
                    new Claim("UniversityName", user?.University?.UniversityName),
                    new Claim("Role", userRole?.Role.RoleName)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Json("Success");
            }
            else
            {
                model.IsLoginSucceed = false;
                return Json("Please enter valid UserName/Password");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StudentLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userList = await _unitOfWork.UserRepository.GetByFilterAsync(x => x.UserName == model.UserName
                                                && x.StudentOrUniversity == (int)StudentOrUniversity.Student, IncludeStr: "Student,University");
                var user = userList.FirstOrDefault();

                if (user == null)
                {
                    ModelState.AddModelError("PasswordMatchError", "UserName/Password is incorrect");
                    model.IsLoginSucceed = false;
                    return Json("UserName/Password is incorrect");
                }
                else
                {
                    if (!user.Status)
                    {
                        ModelState.AddModelError("UserInactiveError", $"{model.UserName} is inactive.Please contact administrator. ");
                        model.IsLoginSucceed = false;
                        return Json($"{model.UserName} is inactive.Please contact administrator. ");
                    }
                }

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

                if (!computedHash.SequenceEqual(user.PasswordHash))
                {
                    ModelState.AddModelError("PasswordMatchError", "UserName/Password is incorrect");
                    model.IsLoginSucceed = false;
                    return Json("UserName/Password is incorrect");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user?.Student.FirstName} {user?.Student.LastName}" ),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("StudentOrUniversity", user.StudentOrUniversity.ToString()),
                    new Claim("StudentId", user.StudentId.ToString()),
                    new Claim("UniversityName", user?.University?.UniversityName),
                    new Claim("Role", "Student")
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Json("Success");
            }
            else
            {
                model.IsLoginSucceed = false;
                return Json("Please enter valid UserName/Password");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Dashboard");
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

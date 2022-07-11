using Core.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Models;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace WebUI.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager, IWebHostEnvironment hosting) :
            base(userManager, signInManager, logger, context, roleManager, hosting)
        {


        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                if (_signInManager.IsSignedIn(User))
                    return RedirectToAction("GoToMainPage");
                else
                {
                    return View("Login", new LoginViewModel());

                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();

            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            try
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                return View("Login", new LoginViewModel());
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }



        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("GoToMainPage", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View("Login", new LoginViewModel());
                }




            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }


        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }


        }






        public async Task<IActionResult> GoToMainPage()
        {
            try
            {
                return await Task.FromResult(View("Home"));
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }
        }
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateNewEmp()
        {
            try
            {
                ViewBag.DepartmentsList = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.RolesList = new SelectList(_roleManager.Roles, "Id", "Name");

                return await Task.FromResult(View());
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GoToEmployeeMNGMNT()
        {
            try
            {
                EmployeeManagementViewModel employeeManagementViewModel = new EmployeeManagementViewModel();


                return await Task.FromResult(View("EmployeeManagement", employeeManagementViewModel));
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();

            }
        }
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public IActionResult LoadData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var EmpData = _context.Employees.Select(x => new EmployeeViewModel
                {
                    Address = x.Address,
                    DateofBirth = x.DateofBirth.ToShortDateString(),
                    DepartmentName = x.Department.Name,
                    Name = x.Name,
                    Email = x.User.Email,
                    ImageUrl = x.ImageUrl,
                    JoiningDate = x.JoiningDate.ToShortDateString(),
                    EmpId = x.ID

                });
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    EmpData = EmpData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    EmpData = EmpData.Where(m => m.Name.Contains(searchValue)
                                                || m.Email.Contains(searchValue));

                }
                recordsTotal = EmpData.Count();
                var data = EmpData.Skip(skip).Take(pageSize).ToList();

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return BadRequest();

            }
        }

        [Authorize(Roles = "Admin,HR")]

        [HttpPost]
        public async Task<IActionResult> Delete(Guid ID)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var emp = await _context.Employees.SingleOrDefaultAsync(x => x.ID == ID);
                if (emp != null)
                {



                    var emppos = await _context.EmployeePosition.Where(x => x.EmployeeID == ID).ToListAsync();
                    if (emppos != null && emppos.Count > 0)
                    {
                        _context.EmployeePosition.RemoveRange(emppos);
                        await _context.SaveChangesAsync();
                    }

                    _context.Employees.Remove(emp);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users.SingleAsync(x => x.Id == emp.UserId);

                    var roles = await _userManager.GetRolesAsync(user);

                    var result = await _userManager.RemoveFromRolesAsync(user, roles);

                    if (result.Succeeded)
                    {
                        var res = await _userManager.DeleteAsync(user);
                        if (res.Succeeded)
                        {
                            string fullPathToDel = Path.Combine(_hosting.WebRootPath, @"img\", emp.ImageUrl);
                            if (System.IO.File.Exists(fullPathToDel))
                            {
                                System.IO.File.Delete(fullPathToDel);
                            }
                            transaction.Commit();
                            return Ok("Deleted");
                        }
                        else
                        {
                            transaction.Rollback();
                            BasicNotification(String.Join(',', res.Errors.Select(x => x.Description)), NotificationType.error, "Opps!!");
                            return BadRequest();

                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        BasicNotification(String.Join(',', result.Errors.Select(x => x.Description)), NotificationType.error, "Opps!!");

                        return BadRequest();

                    }
                }
                else
                {

                    return BadRequest();
                }


            }
            catch (Exception ex)
            {
                transaction.Rollback();
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");

                return Error();
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateNewEmployee(CreateNewEmployeeViewModel model)
        {
            try
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                    var RoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (RoleResult.Succeeded)

                    {
                        var userId = await _userManager.FindByEmailAsync(model.Email);

                        if (model.Image != null)
                        {
                            var extensionFile = Path.GetExtension(model.Image.FileName);
                            var newFileName = Guid.NewGuid().ToString() + extensionFile;
                            string uploads = Path.Combine(_hosting.WebRootPath, @"img\");
                            string fullPath = Path.Combine(uploads, newFileName);
                            model.ImageUrl = newFileName;
                            using (var stream = System.IO.File.Create(fullPath))
                            {
                                await model.Image.CopyToAsync(stream);
                            }
                           
                        }
                        _context.Employees.Add(new Employee
                        {
                            Address = model.Address,
                            DateofBirth = model.DateofBirth,
                            DepartmentId = model.DepartmentId,
                            ImageUrl = model.Image != null ? model.ImageUrl : string.Empty,
                            JoiningDate = DateTime.Now,
                            Name = model.Name,
                            UserId = userId.Id,


                        });
                        await _context.SaveChangesAsync();
                        BasicNotification("Employee Created", NotificationType.success, "Sucsess");
                    }

                    else
                    {
                        BasicNotification("Cannot Create Employee", NotificationType.error, "Opps!!");


                    }


                }
                else
                {
                    BasicNotification("Cannot Create Employee", NotificationType.error, "Opps!!");


                }








            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");

                return Error();
            }

            return await GoToEmployeeMNGMNT();
        }
       
        public async Task<IActionResult> UserProfilePicMethod()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                user = await _userManager.Users.Include(x => x.Employee).SingleAsync(x => x.Id == user.Id);

                var profilPicName = user.Employee.ImageUrl;
                if (profilPicName != null)
                {
                    string profilePicPath = Path.Combine(_hosting.WebRootPath, "img", profilPicName);

                    return PhysicalFile(profilePicPath, "image/" + Path.GetExtension(profilePicPath));
                }
                else
                {
                    //If no image exists return a placeholder image
                    string profilePicPathPH = Path.Combine(_hosting.WebRootPath, "img", "PlaceHolder.png");

                    return PhysicalFile(profilePicPathPH, "image/" + Path.GetExtension(profilePicPathPH));
                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }

        }
        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateEmployeeInfo(Guid ID)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(x => x.User)
                    .Include(x => x.Department)
                    .SingleOrDefaultAsync(x => x.ID == ID);

                if (employee != null)
                {
                    ViewBag.DepartmentsList = new SelectList(_context.Departments, "ID", "Name");
                    UpdateEmployeeInfoViewModel viewModel = new UpdateEmployeeInfoViewModel
                    {
                        Address = employee.Address,
                        DateofBirth = employee.DateofBirth,
                        DepartmentId = employee.DepartmentId,
                        Email = employee.User.Email,
                        ImageUrl = employee.ImageUrl,
                        Name = employee.Name,
                        empid = employee.ID
                    };
                    return View(viewModel);
                }
                else
                {
                    BasicNotification("Emplooye Not Found", NotificationType.error, "Opps!!");
                    return await GoToEmployeeMNGMNT();

                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();
            }


        }
        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateEmployeeInfo(UpdateEmployeeInfoViewModel model)
        {

            try
            {


                var employee = await _context.Employees.SingleAsync(x => x.ID == model.empid);
                if (employee != null)
                {
                    employee.Name = model.Name;
                    employee.Address = model.Address;
                    employee.DepartmentId = model.DepartmentId;
                    employee.DateofBirth = model.DateofBirth;
                    if (model.Image != null)
                    {
                        string fullPathToDel = Path.Combine(_hosting.WebRootPath, @"img\", employee.ImageUrl);
                        if (System.IO.File.Exists(fullPathToDel))
                        {
                            System.IO.File.Delete(fullPathToDel);
                        }
                        var extensionFile = Path.GetExtension(model.Image.FileName);
                        var newFileName = Guid.NewGuid().ToString() + extensionFile;
                        string uploads = Path.Combine(_hosting.WebRootPath, @"img\");
                        string fullPath = Path.Combine(uploads, newFileName);
                        model.Image.CopyTo(new FileStream(fullPath, FileMode.Create));

                        employee.ImageUrl = newFileName;
                    }
                    _context.Employees.Update(employee);
                    await _context.SaveChangesAsync();
                    BasicNotification("Employee Updated", NotificationType.success, "Success");

                    if (User.IsInRole("Admin") || User.IsInRole("HR"))
                    {
                        return await GoToEmployeeMNGMNT();


                    }
                    else
                    {
                        return await GoToMainPage();
                    }
                }
                else
                {


                    BasicNotification("Emplooye Not Found", NotificationType.error, "Opps!!");
                    return await GoToEmployeeMNGMNT();


                }

            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();

            }



        }
        public async Task<IActionResult> GoToProfileDetails()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var emp = await _context.Employees.Include(x => x.Department).Include(x => x.UserPositions).SingleOrDefaultAsync(x => x.UserId == user.Id);
                var roles = await _userManager.GetRolesAsync(user);
                var pos = _context.Positions.Where(x => emp.UserPositions.Select(x => x.PositionID).Contains(x.ID)).Select(x => x.Name);


                UserDetailsViewModel ViewModel = new UserDetailsViewModel()
                {
                    Address = emp.Address,
                    DateOfJoin = emp.JoiningDate,
                    DepartmentCode = emp.Department.Code,
                    DepartmentName = emp.Department.Name,
                    Imageurl = emp.ImageUrl,
                    Name = emp.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = String.Join(',', roles),
                    positions = String.Join(',', pos),
                    EmpId = emp.ID

                };
                return View(ViewModel);
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();

            }


        }

    }
}

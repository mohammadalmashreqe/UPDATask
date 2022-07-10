using Core.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DepartmentController : BaseController
    {
        public DepartmentController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager, IWebHostEnvironment hosting) :
           base(userManager, signInManager, logger, context, roleManager, hosting)
        {



        }
        public IActionResult Index()
        {
            return View();
        }
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

                var DepartmentDate = _context.Departments.Select(x => new DepartmentViewModel
                {
                    ID = x.ID,

                    Name = x.Name


                });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    DepartmentDate = DepartmentDate.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    DepartmentDate = DepartmentDate.Where(m => m.Name.Contains(searchValue));


                }
                recordsTotal = DepartmentDate.Count();
                var data = DepartmentDate.Skip(skip).Take(pageSize).ToList();

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return BadRequest();

            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            
                try
                {
                    var result = await _context.Departments.AddAsync(new Departments { Name = name });
                    await _context.SaveChangesAsync();
                    BasicNotification("Department Created", NotificationType.success, "Success");


                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                    return Error();

                }
            
           
        }
        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                var dep = await _context.Departments.SingleOrDefaultAsync(x => x.ID == id);
                if (dep != null)
                {

                    return View(new DepartmentViewModel { ID = dep.ID, Name = dep.Name });
                }
                else
                {
                    BasicNotification("Department Not Found", NotificationType.error, "Opps!!");
                    return View(new DepartmentViewModel { Name = dep.Name });
                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return Error();


            }


        }
        [HttpPost]
        public async Task<IActionResult> Update([Required] DepartmentViewModel model)
        {

            try
            {
                var result = await _context.Departments.SingleOrDefaultAsync(x => x.ID == model.ID);
                if (result != null)
                {
                    result.Name = model.Name;
                    _context.Departments.Update(result);
                    await _context.SaveChangesAsync();
                    BasicNotification("Department Updated", NotificationType.success, "Success");

                    return RedirectToAction("Index");
                }
                else
                {
                    BasicNotification("Department Not Found", NotificationType.error, "Opps!!");
                    return View(new DepartmentViewModel { Name = model.Name });

                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return View(new DepartmentViewModel { Name = model.Name });


            }


        }
    }
}

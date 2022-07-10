using Core.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class RoleController : BaseController
    {

        public RoleController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager, IWebHostEnvironment hosting) :
            base(userManager, signInManager, logger, context, roleManager, hosting)
        {



        }
        public IActionResult Index()
        {

            return View(_roleManager.Roles);
        }
        public IActionResult Create()
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
                var RoleData = _roleManager.Roles;
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    RoleData = RoleData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    RoleData = RoleData.Where(m => m.Name.Contains(searchValue));


                }
                recordsTotal = RoleData.Count();
                var data = RoleData.Skip(skip).Take(pageSize).ToList();

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");

                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {try
                {
                    IdentityResult result = await _roleManager.CreateAsync(new IdentityRole<Guid>(name));
                    if (result.Succeeded)
                    {
                        BasicNotification("Role Added", NotificationType.success, "Success");
                        return RedirectToAction("Index");
                    }
                    else
                        Errors(result);
                }
                catch (Exception ex)
                {
                    BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                    return RedirectToAction("Index");
                }
            }
            return View(name);
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                IdentityRole<Guid> role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    List<ApplicationUser> members = new List<ApplicationUser>();
                    List<ApplicationUser> nonMembers = new List<ApplicationUser>();
                    foreach (ApplicationUser user in _userManager.Users)
                    {
                        var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                        list.Add(user);
                    }
                    return View(new RoleEdit
                    {
                        Role = role,
                        Members = members,
                        NonMembers = nonMembers
                    });
                }
                else
                {
                    BasicNotification("No Role found", NotificationType.error, "Opps!!");
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            try
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                return RedirectToAction("Index");

            }

            if (ModelState.IsValid)
            {
                BasicNotification("Role Updated", NotificationType.success, "Success");

                return RedirectToAction(nameof(Index));

            }
            else
                return await Update(model.RoleId);
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

    }
}

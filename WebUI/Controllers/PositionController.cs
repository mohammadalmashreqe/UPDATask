using Core.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using WebUI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace WebUI.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class PositionController : BaseController
    {

        public PositionController(UserManager<ApplicationUser> userManager,
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

                var PositionsData = _context.Positions.Select(x => new PositionViewModel
                {
                    ID = x.ID,

                    Name = x.Name


                });

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PositionsData = PositionsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    PositionsData = PositionsData.Where(m => m.Name.Contains(searchValue));


                }
                recordsTotal = PositionsData.Count();
                var data = PositionsData.Skip(skip).Take(pageSize).ToList();

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
                var result = await _context.Positions.AddAsync(new Position { Name = name });
                await _context.SaveChangesAsync();
                BasicNotification("Position Added", NotificationType.success, "Success");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");

                return Error();
            }


        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var Pos = await _context.Positions.SingleOrDefaultAsync(x => x.ID == id);
            if (Pos != null)
            {
                var transaction = await _context.Database.BeginTransactionAsync();
                try
                {

                    var userPositionToDelete = await _context.EmployeePosition.Where(x => x.PositionID == Pos.ID).ToListAsync();
                    if (userPositionToDelete != null && userPositionToDelete.Count > 0)
                    {
                        _context.EmployeePosition.RemoveRange(userPositionToDelete);
                        await _context.SaveChangesAsync();
                    }

                    _context.Positions.Remove(Pos);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    BasicNotification("Position Deleted", NotificationType.success, "Success");

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                    return RedirectToAction("Index");

                }

            }
            else
            {
                BasicNotification("No Position found", NotificationType.error, "Opps!!");
                return RedirectToAction("Index");

            }

        }
        public async Task<IActionResult> Update(Guid id)
        {
            var PosToUpdate = await _context.Positions.SingleOrDefaultAsync(x => x.ID == id);
            if (PosToUpdate != null)
            {
                try
                {
                    List<EmployeeViewModel> members = new List<EmployeeViewModel>();
                    List<EmployeeViewModel> nonMembers = new List<EmployeeViewModel>();



                    foreach (Employee user in _context.Employees.Include(x => x.UserPositions))
                    {

                        if (user.UserPositions != null && user.UserPositions.Count > 0)
                        {
                            if (user.UserPositions.Select(x => x.PositionID).Contains(PosToUpdate.ID))
                                members.Add(new EmployeeViewModel { EmpId = user.ID, Name = user.Name });
                            else
                                nonMembers.Add(new EmployeeViewModel { EmpId = user.ID, Name = user.Name });
                        }
                        else
                        {
                            nonMembers = await _context.Employees
                                                 .Select(x => new EmployeeViewModel
                                                 {
                                                     EmpId = x.ID,
                                                     Name = x.Name
                                                 }).ToListAsync();

                        }


                    }

                    return View(new PositionUpdateViewModel
                    {
                        Position = new PositionViewModel { ID = PosToUpdate.ID, Name = PosToUpdate.Name },
                        Members = members,
                        NonMembers = nonMembers
                    });
                }
                catch (Exception ex)
                {
                    BasicNotification(ex.Message, NotificationType.error, "Opps!!");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                BasicNotification("No Position found", NotificationType.error, "Opps!!");
                return RedirectToAction("Index");

            }

        }

        [HttpPost]
        public async Task<IActionResult> Update(PositionModification model)
        {


            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                foreach (Guid userId in model.AddIds ?? new Guid[] { })
                {
                    await _context.EmployeePosition.AddAsync(new EmployeePosition { EmployeeID = userId, PositionID = model.PositionId });
                    await _context.SaveChangesAsync();
                }
                foreach (Guid userId in model.DeleteIds ?? new Guid[] { })
                {
                    var userposToDelete = await _context.EmployeePosition.Where(x => x.EmployeeID == userId && x.PositionID == model.PositionId).FirstOrDefaultAsync();
                    _context.EmployeePosition.Remove(userposToDelete);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                BasicNotification(" Position Updated", NotificationType.success, "Success");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                BasicNotification(ex.Message, NotificationType.error, "Opps!!");

                return await Update(model.PositionId);
            }

        }
    }
}

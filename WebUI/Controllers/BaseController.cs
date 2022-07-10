using Core.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<HomeController> _logger;
        protected UserManager<ApplicationUser> _userManager;
        protected SignInManager<ApplicationUser> _signInManager;
        protected RoleManager<IdentityRole<Guid>> _roleManager;
        protected readonly ApplicationDbContext _context;
        protected readonly IWebHostEnvironment _hosting;
        public BaseController (UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole<Guid>> roleManager, IWebHostEnvironment hosting)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _hosting = hosting;

        }
        public void BasicNotification(string msj, NotificationType type, string title = "")
        {
        
            TempData["notification"] = $"Swal.fire('{title}','{msj}', '{type.ToString().ToLower()}')";
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

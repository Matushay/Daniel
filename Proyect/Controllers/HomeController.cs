using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyect.Models;
using System.Diagnostics;

namespace Proyect.Controllers
{
    [Authorize]
    [Authorize(Policy = "AccederDashboard")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}

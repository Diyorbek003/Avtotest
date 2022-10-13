using Avtotest.Web.Models;
using Avtotest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Avtotest.Web.Controllers
{
    public class HomeController : Controller
    { 
        private readonly UserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _userService = new UserService();
            _logger = logger;
        }
       

       
        public IActionResult Index()
        {
            bool isLogin = true;
            var user = _userService.GetUserFromCookie(HttpContext);
            if (user == null)
                isLogin = false;

            ViewBag.IsLogin = isLogin;  
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

        public IActionResult Profile()
        {
            return View();
        }
    }
}
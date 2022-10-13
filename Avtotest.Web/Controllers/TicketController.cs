using Avtotest.Web.Repositories;
using Avtotest.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Avtotest.Web.Controllers
{
    public class TicketController : Controller
    {
        private UserService _userService;

        private TicketRepository _ticketRepository;

        public TicketController()
        {
             _userService = new UserService();
            _ticketRepository = new TicketRepository();
        }
        public IActionResult Index()
        {
            var user = _userService.GetUserFromCookie(HttpContext);// user royhatdan otkan bolsa uni cookiedagi qiymatini tekshirish kk 
            if (user == null) // user null bosa uni qugidagi royhatdan otish pagesiga otkaziladi
                return RedirectToAction("SignIn", "Users");

            var ticket = _ticketRepository.GetTicketUserById(user.Index);

            return View(ticket);
        }
    }
}

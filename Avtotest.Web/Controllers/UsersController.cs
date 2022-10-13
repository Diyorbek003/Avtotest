using Avtotest.Web.Repositories;
using Avtotest.Web.Services;
using Avtotest.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Avtotest.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UsersRepository _usersRepository;
        private readonly CookiesService _cookiesService;
        private readonly UserService _userService;
        private readonly TicketRepository _ticketRepository;
        private readonly QuestionsRepository _questionsRepository;

        public UsersController()
        {

            _usersRepository = new UsersRepository();
            _cookiesService = new CookiesService();
            _userService = new UserService();
            _questionsRepository = new QuestionsRepository();
            _ticketRepository = new TicketRepository();
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var user = _userService.GetUserFromCookie(HttpContext);
            if (user != null) // userni tekshiryapmiz
            {
                return View(user); // bor bosa user uni profile viewga jonatyapmz
            }
            return RedirectToAction("SignIn"); // aks holda uni royhatdan otishka jonatyapmz
        }
        public IActionResult SignIn()
        {

            return View();
        }
        [HttpPost]
        public IActionResult SignIn(UserDto user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            

            var _user = _usersRepository.GetUserByPhoneNumber(user.Phone!);
            if (_user.Password == user.Password)
            {
                _cookiesService.SendUserPhoneToCookie(user.Phone!, HttpContext);
                return RedirectToAction("Index");
            }
            return RedirectToAction("SignIn");
        }
       
        
        public IActionResult SignUp()
        {
            return View();

        }
        [HttpPost]
        public IActionResult SignUp(User user)//post
        {

            //if (ModelState.IsValid)//barcha metodlar togri kirib kelganini tekshirib beradi
            //{
            //    return View(user);
            //}

            _usersRepository.InsertUser(user);
            var _user = _usersRepository.GetUserByPhoneNumber (user.Phone!);
            _ticketRepository.InsertUserTrainingTickets(_user.Index, _questionsRepository.GetQuestionsCount() / 20, 20);
            _cookiesService.SendUserPhoneToCookie(user.Phone!, HttpContext);

            return RedirectToAction("Index");
        }

        

        private string? SaveUserImage(IFormFile? imageFile)
        {
            if(imageFile == null)
            {
                return "1.png";
            }
            var imagePath = Guid.NewGuid().ToString("N") + Path.GetExtension(imageFile.FileName);

            var ms = new MemoryStream();
            imageFile.CopyTo(ms);
            System.IO.File.WriteAllBytes(Path.Combine("wwwroot", "Profile", imagePath), ms.ToArray());

            return imagePath;
        }
    }
}
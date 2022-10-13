using Avtotest.Web.Models;
using Avtotest.Web.Options;
using Avtotest.Web.Repositories;
using Avtotest.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Avtotest.Web.Controllers
{
    public class ExaminationsController : Controller
    {

        private readonly QuestionsRepository _questionsRepository;

        private readonly UserService _userService;

        private readonly TicketRepository _ticketRepository;

        private readonly int TicketQuestionCount = 20;

        public ExaminationsController(IOptions<TicketSettings> option)
        {
            _questionsRepository = new QuestionsRepository();   // galabalni qilib abyet olinyapti
            _userService = new UserService(); // userni tekshirish uchun
            _ticketRepository = new TicketRepository();

        }

        public IActionResult Index() // home pagedagi struktura
        {
            var user = _userService.GetUserFromCookie(HttpContext); // userni olyapti
           
            if(user == null) //  userni tekshiryapti
            {
                 return RedirectToAction("SignIn", "Users"); // yoq bolsa shu joyga jonatyapti
            }

            var ticket = CreateRandomTicket(user); //  bor bolsa exam pagega randim fuksiyasi orqali ticketni tanlab beryapti

           return View(ticket); // natijani ekranga chiqarish uchun vievga ticketni jonatilyapti model orqali
        }

        public Ticket CreateRandomTicket(User user) // ticketlarni random orqali olish uchun
        {
                var rendom = new Random(); //  abyekt olinyapti randomdan

            var ticketCount = _questionsRepository.GetQuestionsCount() / TicketQuestionCount; // hamma savollarni tepada elon qilingan ticketquestioncount ozgaruvchiga bolinib kicketlarni sonini chiqaryapmiz
            var ticketIndex = rendom.Next(0, ticketCount); // randomni ticketlarni sonicha aylantiryapmiz
            var from = ticketIndex * TicketQuestionCount; // tanlangan ticketni ticketquestioncountga kopaytirib savolni nechanchiligini topyapmiz
            var ticket = new Ticket(user.Index, from, TicketQuestionCount); // ticketdan abyect olib userga bosolgan ticketni fromidan ticketquestioncountigacha korsatish uchun 

            _ticketRepository.InsertTicket(ticket); // ticketga qiymatlarini berish uchun  ticketrepositoridagi INSERTTICKET funksiyasiga argument qilib ticketni beryapmiz
            var id = _ticketRepository.GetLastRowId(); // harbir qilingan amaldan keyin Id osib boradi 
            ticket.Id = id; // userni idsini unga tenglayapmiz GETLASROWIDdan kelgan idga
            return ticket; //tayyor qiymat qaytarilyapti



        }

        public IActionResult Exam(int ticketId, int? questionId = null, int? choiceId = null) // Exam pagesi unda biz userni imtixon yani Exam qilishimiz uchun
        {
           
            var user = _userService.GetUserFromCookie(HttpContext);// user royhatdan otkan bolsa uni cookiedagi qiymatini tekshirish kk 
            if (user == null) // user null bosa uni qugidagi royhatdan otish pagesiga otkaziladi
                return RedirectToAction("SignIn", "Users");
           
           
            var ticket = _ticketRepository.GetTicketById(ticketId, user.Index);  //harbir qilingan amaldan keyin Id osib boradi uni qiymatlarini beryapmiz
            
            questionId = questionId ?? ticket.FromIndex; // birinchi qiymat null bolganda defult holatda user tanlagan ticketni birinchi savolini oldik
            
            // user tanlagan savol harqanaqangi holatda kiritishi mumkin bolgani uchun fromindexdan katta va undan kiyinchi 20ta savoldan kichkina bolishi kk 
            if (ticket.FromIndex <= questionId && ticket.QuestionCount + ticket.FromIndex > questionId)
            {    
                ViewBag.Ticket = ticket; // pagega chiqarish uchun qiymatlar dynamic qiymat olivchi viewbag.Ticket ozgaruvchiga olinyapti
                var question = _questionsRepository.GetQuestionById(questionId.Value);// user tanlagan questionni GETQUESTIONBYID funsiyasidan foydalani unga yiymatini berib savolni topyapmiz

                ViewBag.TicketData = _ticketRepository.GetTicketdataById(ticket.Id); // userga tanlagan ticketlarini qiymatlarini GETTICKETDATA funsiyasi bn ticket.idni qiymatini olyapmiz

                var _ticketData = _ticketRepository.GetTicketdataByQuestionId(ticketId,questionId.Value);//ticketdatadagi questionidni qimatini olyapmiz

                var _choiceId = null as int?; // ozgaruvchi int toyfasida null
                var _answer = false; // boolean toyfasida

                if(_ticketData != null) // qiymatlar olingan bolsa unga javobni korsatadi
                { 
                    _choiceId = _ticketData.ChoiceId; // varianti
                    _answer = _ticketData.Answer;// variantni javobi
                }
                else if(choiceId != null) // varyanlari null bomasa 
                { 
                  var answer = question.Choices!.First(choice => choice.Id == choiceId).Ansvers; // answerni qiymati olinyapti

                    var ticketData = new Ticketdata() // abyet olinyapti va qiymatlari joyida berilyapti
                    {
                        TicketId = ticketId,
                        QuestionId = question.id,
                        ChoiceId = choiceId.Value,
                        Answer = answer
                    };
                    _ticketRepository.InsertTicketData(ticketData);// tayyorqiymatni INSERTTICKETDATA funksiyasiga berib qiymatlarini joylayapmiz

                    _choiceId = choiceId;
                    _answer = answer;

                    if (_answer) // javob togri bolsa correctcountga qoshib ketadi
                    {
                         _ticketRepository.UpdateTicketCorrectCount(ticket.Id);
                    }

                   

                    if(ticket.QuestionCount == _ticketRepository.GetTicketAnswerCount(ticket.Id))// togri javoblar soni ticketquestioncountga teng bosa examresult pagega resultni chiqqaradi
                    {
                        return RedirectToAction("ExamResult",new {ticketid = ticket.Id });
                    }
                }

                ViewBag.TicketId = ticketId;    
                ViewBag.ChoiceId = _choiceId;// viewda ishlatish uchun 

                ViewBag.Answer = _answer;// viewda ishlatish uchun
                return View(question);// tayyorqiymat qaytarilyapti
                

            }
            return NotFound();// agar savol tanlangan ticketdan  bolmasa  not foundga otadi
             
        }

        public IActionResult GetQuestionById(int questionId)// tanlangan savolni question idsini olyapmiz
        {
            
            var question = _questionsRepository.GetQuestionById(questionId);
           return View(question);   
        }

       
        public IActionResult ExamResult(int ticketId)// resultni chiqarish uchun yangi page
        {
           
            var user = _userService.GetUserFromCookie(HttpContext);//userni cookiesi
            
            if (user == null) // royhatda borligini tekshiryapti
                return RedirectToAction("SignIn", "Users");// royhatda bolmasa shu yerga jonatyapti
            
            var ticket = _ticketRepository.GetTicketById(ticketId, user.Index);//user turgan indexga tiketni idsini berib qiymatini olyapti
            
            return View(ticket);// ishlatish uchun viewga jonatyapti
        }
    }

   
}

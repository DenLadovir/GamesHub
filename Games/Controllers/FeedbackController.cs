using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Services;

namespace Games.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly EmailService _emailService;
        private readonly TelegramService _telegramService;

        public FeedbackController(EmailService emailService, TelegramService telegramService)
        {
            _emailService = emailService;
            _telegramService = telegramService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Check(Feedback feedback)
        {
            if (!ModelState.IsValid)
                return View("Index");

            string message = $"Имя: {feedback.Name} {feedback.Surname}\n" +
                             $"Возраст: {feedback.Age}\n" +
                             $"Email: {feedback.Email}\n" +
                             $"Сообщение: {feedback.Message}";

            await _emailService.SendEmail("Поступил новый отзыв", message, "denis.ladovir@yandex.ru");
            await _telegramService.SendMessageAsync(message);

            return Redirect("/");
        }
    }
}
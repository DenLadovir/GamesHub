using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Games.Services
{
    public class TelegramService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public TelegramService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task SendMessageAsync(string message)
        {
            string botToken = _config["Telegram:BotToken"];
            string chatId = _config["Telegram:ChatId"];

            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            await _http.GetAsync(url);
        }
    }
}
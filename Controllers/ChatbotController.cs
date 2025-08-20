using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ChatBotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetReply([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("الرسالة فاضية!");

            string userMessage = request.Message.Trim().ToLower();
            string reply;

            // ردود بسيطة
            if (userMessage.Contains("hello") || userMessage.Contains("hi"))
            {
                reply = "Hello! How can I help you today?";
            }
            else if (userMessage.Contains("ازيك") || userMessage.Contains("عامل ايه"))
            {
                reply = "اهلا بيك 👋، عامل ايه؟";
            }
            else if (userMessage.Contains("اسمك"))
            {
                reply = "انا شات بوت تجريبي 😊";
            }
            else if (userMessage.Contains("bye") || userMessage.Contains("مع السلامه"))
            {
                reply = "مع السلامة 👋، يومك سعيد!";
            }
            else
            {
                reply = "ممكن توضح سؤالك اكتر؟ 🤔";
            }

            return Ok(new { Reply = reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
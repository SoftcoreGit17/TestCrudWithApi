using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TestData.Models.Viewmodel;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        //[HttpPost]
        //public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        //{
        //    var userMessage = request.Message;

        //    // Temporary response (we'll replace with AI)
        //    var botReply = $"You said: {userMessage}";

        //    return Ok(new { reply = botReply });
        //}
        //[HttpPost]
        //public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        //{
        //    var apiKey = "YOUR_NEW_API_KEY";

        //    using var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization =
        //        new AuthenticationHeaderValue("Bearer", apiKey);

        //    var requestBody = new
        //    {
        //        model = "gpt-4o-mini",
        //        messages = new[]
        //        {
        //    new { role = "user", content = request.Message }
        //}
        //    };

        //    var json = JsonSerializer.Serialize(requestBody);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync(
        //        "https://api.openai.com/v1/chat/completions",
        //        content
        //    );

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return BadRequest(responseString);
        //    }

        //    // Parse only the reply text (important fix)
        //    using var doc = JsonDocument.Parse(responseString);
        //    var reply = doc.RootElement
        //                   .GetProperty("choices")[0]
        //                   .GetProperty("message")
        //                   .GetProperty("content")
        //                   .GetString();

        //    return Ok(new { reply });
        //}
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };

            var requestBody = new { model = "llama3", prompt = request.Message, stream = false };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:11434/api/generate", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Deserialize the response
            var llamaResponse = JsonSerializer.Deserialize<LlamaResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Return only the text
            return Ok(new { message = llamaResponse.Response });
        }
    }
}

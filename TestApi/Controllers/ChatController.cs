using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TestData.Models.Viewmodel;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        // 1️⃣ Declare _httpClient as a private readonly field
        private readonly HttpClient _httpClient;

        // 2️⃣ Inject HttpClient via constructor (best practice)
        public ChatController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //[HttpPost]
        //public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        //{
        //    var userMessage = request.Message;

        //    // Temporary response (we'll replace with AI)
        //    var botReply = $"You said: {userMessage}";

        //    return Ok(new { reply = botReply });
        //}
        //using open Api
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
            var requestBody = new
            {
                model = "llama3",
                prompt = request.Message,
                stream = true,
                keep_alive = "10m",
                options = new { num_predict = 150 }
            };
            var json = JsonSerializer.Serialize(requestBody);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead // ✅ now valid
            );
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var fullResponse = new StringBuilder();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                try
                {
                    var chunk = JsonSerializer.Deserialize<LlamaResponse>(
                        line,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    if (chunk?.Response != null)
                    {
                        fullResponse.Append(chunk.Response);
                   }
                    if (chunk?.Done == true)
                    {
                        break;
                    }
                }
                catch (JsonException)
                {
                    // ignore bad chunks
                }
            }

            return Ok(new { message = fullResponse.ToString() });
        }
        //[HttpPost("chat")]
        //public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        //{
        //    using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        //    var requestBody = new { model = "llama3", prompt = request.Message, stream = false };
        //    var json = JsonSerializer.Serialize(requestBody);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync("http://localhost:11434/api/generate", content);
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    // Deserialize the response
        //    var llamaResponse = JsonSerializer.Deserialize<LlamaResponse>(responseString, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });
        //    // Return only the text
        //    return Ok(new { message = llamaResponse.Response });
        //}
    }
}

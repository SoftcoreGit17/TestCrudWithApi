using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Testdata.Viewmodel;

namespace Test.Controllers
{

    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginPost([FromBody] Logindata model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/AdminLogin/Login", model);
            var result = await response.Content.ReadFromJsonAsync<ResponseModel<LoginModel>>();

            if (result != null && result.Status)
            {
                Response.Cookies.Append("accessToken", result.Data.AccessToken, new CookieOptions
                {
                    HttpOnly = false, 
                    Secure = false,   
                    SameSite = SameSiteMode.Lax,
                 //   Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok(result);
            }

            return BadRequest(result);
        }

    }
}

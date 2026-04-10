using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Test.Models;
using Testdata.Viewmodel;
using TestData;
using TestData.Models.Viewmodel;
using TestServices.Utilities;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly DapperContext _DapperContext;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory,DapperContext dapperContext)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("MyApiClient");
            _DapperContext = dapperContext;
        }

        public IActionResult Index()
        {
            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }
        public IActionResult TestUi()
        {
            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddCustomer(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new CustomerModel()); 
            }

            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/CustomerDetails/GetCustomerDetailbyid?id={id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); 
            }

            var customer = await response.Content.ReadFromJsonAsync<ResponseModel<CustomerModel>>();

            if (customer == null || customer.Data == null)
            {
                return NotFound();
            }

            return View(customer.Data); 
        }
        //[HttpPost]
        //public async Task<IActionResult> AddCustomer([FromForm] CustomerModel model)
        //{
        //    string token = Request.Cookies["accessToken"];
        //    if (string.IsNullOrEmpty(token))
        //        return Unauthorized();
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    string apiUrl = model.id > 0
        //        ? "api/CustomerDetails/UpdateCustomerbyid"
        //        : "api/CustomerDetails/CustomerRegistration";
        //    var method = model.id > 0 ? HttpMethod.Put : HttpMethod.Post;
        //    var request = new HttpRequestMessage(method, apiUrl)
        //    {
        //        Content = JsonContent.Create(model)
        //    };
        //    var response = await _httpClient.SendAsync(request);
        //    var content = await response.Content.ReadAsStringAsync();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return Content(content, "application/json");
        //    }
        //    return StatusCode((int)response.StatusCode, content);
        //}
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromForm] CustomerModel model)
        {
            string token = Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var apiUrl = model.id > 0
                ? "api/CustomerDetails/UpdateCustomerbyid"
                : "api/CustomerDetails/CustomerRegistration";

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.CustomerName ?? ""), "CustomerName");
            content.Add(new StringContent(model.CustomerMobileno?.ToString() ?? ""), "CustomerMobileno");
            content.Add(new StringContent(model.CustomerPincode?.ToString() ?? ""), "CustomerPincode");
            content.Add(new StringContent(model.Address ?? ""), "Address");
            content.Add(new StringContent(model.Email ?? ""), "Email");
            content.Add(new StringContent(model.id?.ToString() ?? ""), "id");

            if (model.Image != null && model.Image.Length > 0)
            {
                var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);
                ms.Position = 0;
                content.Add(new StreamContent(ms), "Image", model.Image.FileName);
            }
            else
            {
                content.Add(new StringContent(model.Profileimage ?? ""), "Profileimage");
            }

            using var request = new HttpRequestMessage(model.id > 0 ? HttpMethod.Put : HttpMethod.Post, apiUrl)
            {
                Content = content
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Content(responseContent, "application/json");
        }
        [HttpGet]
        [Route("Home/GetCustomerDetails")]
        public async Task<IActionResult> GetCustomerDetails()
        {
            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/CustomerDetails/GetCustomerDetail");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "No customer data available.";
                return View("GetCustomerDetails", new ResponseModel<List<CustomerModel>>
                {
                    Data = new List<CustomerModel>()
                });
            }

            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<ResponseModel<List<CustomerModel>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                ViewBag.Error = "No customer data available.";
                data = new ResponseModel<List<CustomerModel>>
                {
                    Data = new List<CustomerModel>()
                };
            }
            else if (data.Data == null)
            {
                ViewBag.Error = "No customer data available.";
                data.Data = new List<CustomerModel>();
            }

            return View("GetCustomerDetails", data);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (id == 0)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { success = false, message = "Unauthorized" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/CustomerDetails/DeleteCustomerbyid?id={id}");

            var apiResponseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(apiResponseContent, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, apiResponseContent);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetailbyid(int id)
        {
            if (id == 0)
            {
                return BadRequest(new { success = false, message = "Invalid data." });
            }

            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/CustomerDetails/GetCustomerDetailbyid?id={id}");

            var apiResponseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(apiResponseContent, "application/json");
            }
            else
            {
                return StatusCode((int)response.StatusCode, apiResponseContent);
            }
        }
        [HttpGet]
        public IActionResult SendMessage()
        {
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Message))
                return BadRequest(new { message = "Message is required" });

            string token = Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Unauthorized" });

            var apiUrl = "api/Chat/chat"; // your internal API route

            // Prepare JSON body exactly like your API expects
            var requestBody = new
            {
                model = "llama3",
                prompt = model.Message,
                stream = true,
                keep_alive = "10m",
                options = new { num_predict = 150 }
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Content(responseContent, "application/json");
        }
    }
}

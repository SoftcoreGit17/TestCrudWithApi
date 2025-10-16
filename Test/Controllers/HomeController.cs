using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Test.Models;
using Testdata.Viewmodel;
using TestData;
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

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerModel model)
        {
            string token = Request.Cookies["accessToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string apiUrl = model.id > 0
                ? "api/CustomerDetails/UpdateCustomerbyid"
                : "api/CustomerDetails/CustomerRegistration";

            var method = model.id > 0 ? HttpMethod.Put : HttpMethod.Post;

            var request = new HttpRequestMessage(method, apiUrl)
            {
                Content = JsonContent.Create(model)
            };

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, content);
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

    }
}

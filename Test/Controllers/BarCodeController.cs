using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using Test.Models;

namespace Test.Controllers
{
    public class BarCodeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;
        public BarCodeController(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _httpClient = httpClientFactory.CreateClient("MyApiClient");

        }
        [HttpGet]
        public IActionResult CreateBarcode()
        {
            string token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        [HttpPost]
        public IActionResult CreateBarcode(GenerateBarcodeModel generateBarcode)
        {
            if (string.IsNullOrWhiteSpace(generateBarcode.BarcodeText))
            {
                ModelState.AddModelError("BarcodeText", "Barcode text is required.");
                return View();
            }

            try
            {
                string token = Request.Cookies["accessToken"];

                if (string.IsNullOrEmpty(token))
                    return Unauthorized();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                GeneratedBarcode barcode = IronBarCode.BarcodeWriter.CreateBarcode(generateBarcode.BarcodeText, BarcodeWriterEncoding.Code128);

                barcode.ResizeTo(200, 40); 
                barcode.ChangeBarCodeColor(Color.Black);
                barcode.SetMargins(10);

                using var barcodeImage = barcode.Image;
                int finalWidth = barcodeImage.Width;
                int finalHeight = barcodeImage.Height + 30; 

                using var finalImage = new Bitmap(finalWidth, finalHeight);
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.White);

                    g.DrawImage(barcodeImage, 0, 0);

                    using var font = new Font("Arial", 12, FontStyle.Bold);
                    using var brush = new SolidBrush(Color.Black);

                    var text = generateBarcode.BarcodeText;
                    SizeF textSize = g.MeasureString(text, font);
                    float x = (finalWidth - textSize.Width) / 2;
                    float y = barcodeImage.Height + 5;

                    g.DrawString(text, font, brush, x, y);
                }
                string path = Path.Combine(_environment.WebRootPath, "GeneratedBarcode");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = $"barcode_{Guid.NewGuid()}.png";
                string filePath = Path.Combine(path, fileName);
                finalImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                string imageUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/GeneratedBarcode/{fileName}";
                ViewBag.QrCodeUri = imageUrl;

                return View();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "There was an error generating the barcode.");
                return View();
            }
        }

    }


}

using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace Test.Controllers
{
    public class PrinterController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;
        public PrinterController(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _httpClient = httpClientFactory.CreateClient("MyApiClient");

        }
        public IActionResult GetPrinters()
        {
            var printers = PrinterSettings.InstalledPrinters
                .Cast<string>()
                .ToList();

            return Json(printers);
        }
        public IActionResult CreatePdf()
        {
            var renderer = new ChromePdfRenderer();

            var pdf = renderer.RenderHtmlAsPdf(
                "<h1>Hello, IronPDF!</h1><p>This is a simple PDF document.</p>"
            );

            return File(pdf.BinaryData, "application/pdf", "MyFirstPdf.pdf");
        }
        public IActionResult PrintPdf()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PrintPdf([FromBody] string printerName)
        {
            var htmlContent = "<h1>Invoice</h1><p>Thank you for your business!</p>";

            var renderer = new ChromePdfRenderer();
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);

            var printDoc = pdf.GetPrintDocument();

            printDoc.PrinterSettings.PrinterName = printerName;

            if (!printDoc.PrinterSettings.IsValid)
                return BadRequest("Invalid printer selected.");

            printDoc.Print();

            return Content($"Sent to printer: {printerName}");
        }
    }
}


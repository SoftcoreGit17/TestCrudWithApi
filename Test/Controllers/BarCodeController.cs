using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Test.Models;

namespace Test.Controllers
{
    public class BarCodeController : Controller
    {
       private readonly IWebHostEnvironment _environment;
        public BarCodeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult CreateBarcode()
        {
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
                GeneratedBarcode barcode = IronBarCode.BarcodeWriter.CreateBarcode(
                    generateBarcode.BarcodeText, BarcodeWriterEncoding.Code128);

                barcode.ResizeTo(450, 150);
                barcode.AddBarcodeValueTextBelowBarcode();
                barcode.ChangeBarCodeColor(Color.Black);
                barcode.SetMargins(15);

                string path = Path.Combine(_environment.WebRootPath, "GeneratedBarcode");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = $"barcode_{Guid.NewGuid()}.png";
                string filePath = Path.Combine(path, fileName);
                barcode.SaveAsPng(filePath);
                string imageUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/GeneratedBarcode/{fileName}";
                ViewBag.QrCodeUri = imageUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "There was an error generating the barcode.");
                return View();
            }
        }

    }


}

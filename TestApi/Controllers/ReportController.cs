using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using Testdata.Viewmodel;
using TestData.Models.Entities;
using TestServices.Utilities;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
            private readonly ICustomerInterface _ICustomerInterface;

            public ReportController(ICustomerInterface customerInterface)
            {
                _ICustomerInterface = customerInterface;
            }

        [HttpGet, Route("GetReport")]
        public async Task<IActionResult> GetReport()
        {
            var result = await _ICustomerInterface.getcustomerDetail();

            if (result == null || result.Count == 0)
                return NotFound("No customer data found.");

            string imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfileImages");

            foreach (var c in result)
            {
                string imageFile = string.IsNullOrEmpty(c.Profileimage) ? "no-image.jpg" : c.Profileimage;
                string fullPath = Path.Combine(imageFolder, imageFile);

                c.Profileimage = $"file:///{fullPath.Replace("\\", "/")}";
            }

            var rdlcPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "CustomerReport.rdl");

            if (!System.IO.File.Exists(rdlcPath))
                return NotFound($"RDLC report not found at {rdlcPath}");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcPath;
            report.DataSources.Add(new ReportDataSource("DataSet1", result));
            report.EnableExternalImages = true;

            byte[] pdfBytes = report.Render("PDF");

            return File(pdfBytes, "application/pdf", "CustomerReport.pdf");
        }
    }
    }


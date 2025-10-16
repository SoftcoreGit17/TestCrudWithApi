using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class GenerateBarcodeModel
    {
        [Display(Name = "Enter Barcode Text")]
        public string BarcodeText{get;set;}
    }
}

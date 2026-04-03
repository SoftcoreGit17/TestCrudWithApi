using Microsoft.AspNetCore.Http;

namespace Testdata.Viewmodel
{
    public class CustomerModel
    {
        public int? id { get; set; }
        public string? CustomerName { get; set; }
        public long? CustomerMobileno { get; set; }
        public long? CustomerPincode { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public IFormFile? Image { get; set; }
        public string? Profileimage { get; set; }
        public bool? Isdelete { get; set; }
    }
}

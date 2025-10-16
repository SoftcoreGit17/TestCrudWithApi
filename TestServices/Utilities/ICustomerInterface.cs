using Testdata.Viewmodel;
using TestData.Models.Entities;

namespace TestServices.Utilities
{
    public interface ICustomerInterface
    {
        public Task<CustomerRe> AddCustomer(CustomerModel model);
        public Task<List<CustomerModel>> getcustomerDetail();
        public Task<CustomerRe> deletecustomerbyid(int id);
        public Task<CustomerRe> updatecustomerbyid(CustomerModel model);
        public Task<Logindata> Logininterface(Logindata model);
        public Task<CustomerModel> getcustomerDetailbyid(int id);
      //  Task SendEmailAsync(MailRequest mailRequest);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testdata.Viewmodel;
using TestData.Models.Entities;

namespace TestServices.Utilities
{
    public class Customer : ICustomerInterface
    {
        private readonly Db23320Context _context;
        private readonly MailSettings _mailSettings;

        public Customer(Db23320Context context, IOptions<MailSettings> mailSettings)
        {
            _context = context;
            _mailSettings = mailSettings.Value;
        }


        public async Task<CustomerRe> AddCustomer(CustomerModel model)
        {
            try
            {
                CustomerRe customer = new CustomerRe
                {
                    CustomerName = model.CustomerName,
                    CustomerMobileno = model.CustomerMobileno,
                    CustomerPincode = model.CustomerPincode,
                    Email =model.Email,
                    Address =model.Address,
                    Isdelete = false,
                };
                await _context.AddAsync(customer);
                await _context.SaveChangesAsync();

                return customer;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<CustomerModel>> getcustomerDetail()
        {
            try
            {
                var data = await _context.CustomerRes
                    .Select(c => new CustomerModel
                    {
                        id = c.Id,
                        CustomerName = c.CustomerName,
                        CustomerMobileno = c.CustomerMobileno,
                        CustomerPincode = c.CustomerPincode,
                        Address = c.Address,
                        Email = c.Email,
                        Isdelete = c.Isdelete,
                    }).OrderByDescending(x=>x.id).Where(x=>x.Isdelete == false).ToListAsync();

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CustomerRe> deletecustomerbyid(int id)
        {
            try
            {
                var data =  _context.CustomerRes.Where(x=>x.Id == id).FirstOrDefault();
                 data.Isdelete = true;
                //_context.CustomerRes.Remove(data);
                _context.SaveChanges();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CustomerRe> updatecustomerbyid(CustomerModel model)
        {
            try
            {
                var data = _context.CustomerRes.Where(x => x.Id == model.id).FirstOrDefault();
                if (data != null)
                {
                    CustomerRe customer = new CustomerRe();
                    {
                        data.CustomerName = model.CustomerName;
                        data.CustomerMobileno = model.CustomerMobileno;
                        data.CustomerPincode = model.CustomerPincode;
                        data.Email = model.Email;
                        data.Address = model.Address;
                    };
                    _context.SaveChanges();
                }
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Logindata> Logininterface(Logindata model)
        {
            try
            {
                var data = await _context.CustomerRes
                    .FirstOrDefaultAsync(x => x.CustomerName == model.Username && x.Password == model.Password);

                if (data != null)
                {
                    return new Logindata
                    {
                        Username = data.CustomerName,
                        Password = data.Password,
                    };
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CustomerModel> getcustomerDetailbyid(int id)
        {
            try
            {
                var data = await _context.CustomerRes
                  .Select(c => new CustomerModel
                  {
                      id = c.Id,
                      CustomerName = c.CustomerName,
                      CustomerMobileno = c.CustomerMobileno,
                      CustomerPincode = c.CustomerPincode,
                      Address = c.Address,
                      Email = c.Email
                  }).Where(x => x.id == id).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public async Task SendEmailAsync(MailRequest mailRequest)
        //{
        //    var email = new MimeMessage();
        //    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        //    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        //    email.Subject = mailRequest.Subject;
        //    var builder = new BodyBuilder();
        //    if (mailRequest.Attachments != null)
        //    {
        //        byte[] fileBytes;
        //        foreach (var file in mailRequest.Attachments)
        //        {
        //            if (file.Length > 0)
        //            {
        //                using (var ms = new MemoryStream())
        //                {
        //                    file.CopyTo(ms);
        //                    fileBytes = ms.ToArray();
        //                }
        //                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
        //            }
        //        }
        //    }
        //    builder.HtmlBody = mailRequest.Body;
        //    email.Body = builder.ToMessageBody();
        //    using var smtp = new SmtpClient();
        //    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        //    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        //    await smtp.SendAsync(email);
        //    smtp.Disconnect(true);
        //}
    }

}


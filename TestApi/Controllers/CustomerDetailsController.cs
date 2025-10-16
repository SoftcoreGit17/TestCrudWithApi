using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Testdata.Viewmodel;
using TestData.Models.Entities;
using TestServices.JwtToken;
using TestServices.Utilities;


namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerDetailsController : ControllerBase
    {
        private readonly Db23320Context _context;
        private readonly ICustomerInterface _ICustomerInterface;
        public CustomerDetailsController(Db23320Context context, ICustomerInterface customerInterface)
        {
            _context = context;
            _ICustomerInterface = customerInterface;
        }
        [HttpPost, Route("CustomerRegistration")]
        public async Task<IActionResult> CustomerRegistration(CustomerModel customer)
        {
            ResponseModel<CustomerRe> response = new ResponseModel<CustomerRe>();
            try
            {
                var result = await _ICustomerInterface.AddCustomer(customer);
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
                    return NotFound(response);
                }
                else
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Customer registered successfully.";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Data = null;
                return BadRequest(response);
            }
        }
        [HttpGet, Route("GetCustomerDetail")]
        public async Task<IActionResult> GetCustomerDetail()
        {
            ResponseModel<List<CustomerModel>> response = new ResponseModel<List<CustomerModel>>();
            try
            {
                var result = await _ICustomerInterface.getcustomerDetail();
                if (result == null || result.Count == 0)
                {
                    response.IsSuccess = false;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
                    return NotFound(response);
                }
                else
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Status = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Customer details fetched successfully.";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = false;
                response.Message = ex.Message;
                response.Data = null;
                response.StatusCode = StatusCodes.Status400BadRequest;

                return BadRequest(response);
            }
        }
        [HttpDelete, Route("DeleteCustomerbyid")]
        public async Task<IActionResult> DeleteCustomerbyid(int id)
        {
            ResponseModel<CustomerRe> response = new ResponseModel<CustomerRe>();
            try
            {
                var result = await _ICustomerInterface.deletecustomerbyid(id);
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
                    return NotFound(response);
                }
                else
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Status = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Data Delete successfully.";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = false;
                response.Message = ex.Message;
                response.Data = null;
                response.StatusCode = StatusCodes.Status400BadRequest;

                return BadRequest(response);
            }
        }
        [HttpPut, Route("UpdateCustomerbyid")]
        public async Task<IActionResult> UpdateCustomerbyid(CustomerModel customer)
        {
            ResponseModel<CustomerRe> response = new ResponseModel<CustomerRe>();
            try
            {
                var result = await _ICustomerInterface.updatecustomerbyid(customer);
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
                    return NotFound(response);
                }
                else
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Status = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Data Update successfully.";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = false;
                response.Message = ex.Message;
                response.Data = null;
                response.StatusCode = StatusCodes.Status400BadRequest;

                return BadRequest(response);
            }
        }
        [HttpGet, Route("GetCustomerDetailbyid")]
        public async Task<IActionResult> GetCustomerDetailbyid(int id)
        {
            ResponseModel<CustomerModel> response = new ResponseModel<CustomerModel>();
            try
            {
                var result = await _ICustomerInterface.getcustomerDetailbyid(id);
                if (result == null)
                {
                    response.IsSuccess = false;
                    response.Status = false;
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Data not found.";
                    return NotFound(response);
                }
                else
                {
                    response.Data = result;
                    response.IsSuccess = true;
                    response.Status = true;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Message = "Customer details fetched successfully.";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = false;
                response.Message = ex.Message;
                response.Data = null;
                response.StatusCode = StatusCodes.Status400BadRequest;

                return BadRequest(response);
            }
        }
    }
}

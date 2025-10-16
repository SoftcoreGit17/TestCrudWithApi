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
    public class AdminLoginController : ControllerBase
    {
        private readonly Db23320Context _context;
        private readonly ICustomerInterface _ICustomerInterface;
        private readonly IJwtAuth _IJwtAuth;
        public AdminLoginController(Db23320Context context, ICustomerInterface CustomerInterface, IJwtAuth jwtAuth)
        {
            _context = context;
            _ICustomerInterface = CustomerInterface;
            _IJwtAuth = jwtAuth;
        }
        [HttpPost, Route("Login")]
        public async Task<IActionResult> Login(Logindata model)
        {
            ResponseModel<LoginModel> response = new ResponseModel<LoginModel>();
            try
            {
                var data = await _context.CustomerRes
                    .FirstOrDefaultAsync(x => x.CustomerName == model.Username && x.Password == model.Password);
                if (data == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Status = false;
                    response.Message = "Incorrect username or password.";
                    return Ok(response);
                }
                Logindata result = await _ICustomerInterface.Logininterface(model);

                var token = _IJwtAuth.GenerateAccessToken(result);
                var refreshToken = _IJwtAuth.GenerateRefreshToken();

                // Store refresh token in DB
                data.RefreshToken = refreshToken;
                data.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
                await _context.SaveChangesAsync();
                if (string.IsNullOrEmpty(token))
                {
                    response.Status = false;
                    response.Message = "Token generation failed.";
                    return StatusCode(500, response);
                }
                var responsedata = new LoginModel()
                {
                    Username = result.Username,
                    Password = result.Password,
                    AccessToken = token
                };
                response.StatusCode = StatusCodes.Status200OK;
                response.Status = true;
                response.Message = "Login successful.";
                response.Data = responsedata;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Status = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);
            }
        }

        [HttpPost, Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestModel model)
        {
            var response = new ResponseModel<LoginModel>();

            if (string.IsNullOrWhiteSpace(model.AccessToken) || string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                response.Status = false;
                response.Message = "Token is missing.";
                return BadRequest(response);
            }

            ClaimsPrincipal principal;
            try
            {
                principal = _IJwtAuth.GetPrincipalFromExpiredToken(model.AccessToken);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Invalid token.";
                response.Exception = ex;
                return BadRequest(response);
            }

            var username = principal?.Identity?.Name;

            var user = await _context.CustomerRes
                .FirstOrDefaultAsync(u => u.CustomerName == username);

            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                response.Status = false;
                response.Message = "Invalid refresh token.";
                return Unauthorized(response);
            }

            var newAccessToken = _IJwtAuth.GenerateAccessToken(new Logindata { Username = user.CustomerName });
            var newRefreshToken = _IJwtAuth.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();

            response.Status = true;
            response.Message = "Token refreshed successfully.";
            response.Data = new LoginModel
            {
                Username = user.CustomerName,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return Ok(response);
        }
    }
}

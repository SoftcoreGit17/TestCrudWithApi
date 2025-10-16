namespace Testdata.Viewmodel
{
    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class Logindata
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class TokenRequestModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}

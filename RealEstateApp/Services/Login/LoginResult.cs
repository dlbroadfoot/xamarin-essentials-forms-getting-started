namespace RealEstateApp.Services.Login
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

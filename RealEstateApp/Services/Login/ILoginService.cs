using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Services.Login
{
    public interface ILoginService
    {
        Task<LoginResult> Login(string username, string password);
        Task<bool> IsValidAccessToken(string accessToken);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Services.Login
{
    public class MockLoginService : ILoginService
    {
        public Task<LoginResult> Login(string username, string password)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var result = new LoginResult();

                if (password == "password")
                {
                    result.Succeeded = true;
                    result.AccessToken = "access_token_123";
                    result.RefreshToken = "refresh_token_123";
                }

                return result;
            });
        }

        public Task<bool> IsValidAccessToken(string accessToken)
        {
            return Task.FromResult(accessToken?.StartsWith("access_token") == true);
        }
    }
}

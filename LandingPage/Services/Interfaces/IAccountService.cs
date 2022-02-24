using LandingPage.Data.Entities;

namespace LandingPage.WebApi.Services.Interfaces
{
    public interface IAccountService
    {
        bool Success { get; set; }

        void Authen(string userName, string password);

        string GenerateOtp();

        void Logout();

        string HashPassword(string password);

        string GenerateJWT(User user);

        string GenerateRefreshToken();

        bool Validate(string refreshToken);
    }
}
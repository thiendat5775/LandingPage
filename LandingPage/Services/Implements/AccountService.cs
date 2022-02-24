using LandingPage.Data.Entities;
using LandingPage.Infrastructure.Interfaces;
using LandingPage.WebApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace LandingPage.WebApi.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<User, Guid> _userRepo;
        private readonly IConfiguration _config;

        public AccountService(IRepository<User, Guid> userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public bool Success { get; set; }

        public void Authen(string userName, string password)
        {
            var account = _userRepo.FindAll().SingleOrDefault(x => x.UserName == userName);

            // check account found and verify password
            /*if (account == null || !BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
                Success = false;
            else
                Success = true;*/
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public string GenerateOtp()
        {
            Random ramdom = new Random();
            string OtpRamdom = ramdom.Next(1000, 9999).ToString();
            return OtpRamdom;
        }

        public string HashPassword(string password)
        {
            return HashPassword(password);
        }

        public string GenerateJWT(User user)
        {
            Claim[] claims = new[]
               {
                     new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                     new Claim("id", user.Id.ToString()),
                     new Claim("fullName", user.FullName),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            double timeExpire = double.Parse(_config["JwtSettings:AccessTokenExpire"]);

            return TokenGenerate(_config["JwtSettings:Key"],
                _config["JwtSettings:IsUser"],
                _config["JwtSettings:IsAudience"],
                DateTime.UtcNow.AddHours(timeExpire),
                claims);
        }

        public string GenerateRefreshToken()
        {
            double timeExpire;
            bool ok = double.TryParse(_config["JwtSettings:RefreshTokenExpire"], out timeExpire);
            if (!ok) throw new Exception("Check appsettings.json -> JwtSettings");

            return TokenGenerate(_config["JwtSettings:RefreshKey"],
                _config["JwtSettings:IsUser"],
                _config["JwtSettings:IsAudience"],
                DateTime.UtcNow.AddHours(timeExpire));
        }

        private string TokenGenerate(string secretKey, string isUser, string isAudience, DateTime? expireTime, IEnumerable<Claim> claims = null)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(isUser,
                isAudience,
                claims,
                expires: expireTime,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool Validate(string refreshToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:RefreshKey"]);
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["JwtSettings:IsUser"],
                ValidAudience = _config["JwtSettings:IsAudience"],
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
using LandingPage.Data.Entities;
using System;

namespace LandingPage.WebApi.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        void Create(RefreshToken refreshToken);

        RefreshToken GetByToken(string token);

        void Save();

        void Delete(Guid id);
        void DeleteAll(Guid userId);
    }
}
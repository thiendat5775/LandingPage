using LandingPage.Data.Entities;
using LandingPage.Infrastructure.Interfaces;
using LandingPage.WebApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandingPage.WebApi.Services.Implements
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RefreshToken, Guid> _refreshRepository;

        public RefreshTokenService(IUnitOfWork unitOfWork, IRepository<RefreshToken, Guid> refreshRepository)
        {
            _unitOfWork = unitOfWork;
            _refreshRepository = refreshRepository;
        }

        public void Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();
            _refreshRepository.Add(refreshToken);
        }

        public RefreshToken GetByToken(string token)
        {
            RefreshToken refreshToken = _refreshRepository.FindSingle(x => x.Token == token);
            return refreshToken;
        }

        public void Delete(Guid id)
        {
            _refreshRepository.Remove(id);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void DeleteAll(Guid userId)
        {
            List<RefreshToken> refreshTokens = _refreshRepository.FindAll(x => x.UserId == userId).ToList();
            _refreshRepository.RemoveMultiple(refreshTokens);
        }
    }
}
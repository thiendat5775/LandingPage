using AutoMapper;
using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels;
using LandingPage.Infrastructure.Interfaces;
using LandingPage.WebApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandingPage.WebApi.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, Guid> _userRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IRepository<User, Guid> userRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Create(User user)
        {
            _userRepo.Add(user);
        }

        public User FindUserByName(string uniqueName)
        {
            return _userRepo.FindSingle(x => x.UserName == uniqueName);
        }

        public User FindUserById(Guid id)
        {
            return _userRepo.FindById(id);
        }

        public void Update(User user)
        {
            _userRepo.Update(user);
        }

        public bool CheckPhone(string phonenumber)
        {
            List<User> user = _userRepo.FindAll(x => x.PhoneNumber == phonenumber).ToList();
            if (user.Any()) return true;
            else return false;
        }
    }
}
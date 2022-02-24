using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels;
using System;

namespace LandingPage.WebApi.Services.Interfaces
{
    public interface IUserService
    {
        void Create(User user);
        void Update(User user);
        bool CheckPhone(string phonenumber);

        void Save();

        User FindUserByName(string uniqueName);

        User FindUserById(Guid id);
    }
}
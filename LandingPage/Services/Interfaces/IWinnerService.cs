using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;
using System;
using System.Collections.Generic;

namespace LandingPage.WebApi.Services.Interfaces
{
    public interface IWinnerService
    {
        List<GiftViewModel> GetGiftByUser(Guid userId);

        void Create(CampaignDetailViewModel campaignVm);

        void Save();
    }
}
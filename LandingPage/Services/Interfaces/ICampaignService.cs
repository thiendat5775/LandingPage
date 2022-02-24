using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;

namespace LandingPage.WebApi.Services.Interfaces
{
    public interface ICampaignService
    {
        Campaign GetCampaignInfo(int campaignId);

        void Create(CampaignViewModel campaignVm);

        void Update(int campaignId);

        void Save();

        void Delete(int campaignId);
    }
}
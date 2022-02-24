using AutoMapper;
using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;
using LandingPage.Infrastructure.Interfaces;
using LandingPage.WebApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandingPage.WebApi.Services.Implements
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<Campaign, int> _campaignRepo;
        private readonly IRepository<Gift, int> _giftRepo;
        private readonly IRepository<Rule, int> _ruleRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<CampaignDetail, int>_campaignDetailRepo;

        public CampaignService(IRepository<Campaign, int> campaignRepo,
            IRepository<Gift, int> giftRepo, IRepository<Rule, int> ruleRepo,
            IUnitOfWork unitOfWork, IMapper mapper, IRepository<CampaignDetail, int> campaignDetailRepo)
        {
            _campaignRepo = campaignRepo;
            _giftRepo = giftRepo;
            _ruleRepo = ruleRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _campaignDetailRepo = campaignDetailRepo;
        }

        public void Create(CampaignViewModel campaignVm)
        {
            Campaign campaign = _mapper.Map<CampaignViewModel, Campaign>(campaignVm);
            _campaignRepo.Add(campaign);
        }

        public void Delete(int campaignId)
        {
            _campaignRepo.Remove(campaignId);
        }

        public Campaign GetCampaignInfo(int campaignId)
        {
            Campaign campaign = _campaignRepo.FindById(campaignId);

            Rule rule = _ruleRepo.FindSingle(x => x.Id == campaign.RuleId);
            campaign.Rule = rule;

            List<Gift> gifts = _giftRepo.FindAll(x => x.CampaignId == campaignId).ToList();
            campaign.Gifts = gifts;

            List<CampaignDetail> detais = _campaignDetailRepo.FindAll(x => x.CampaignId == campaignId).ToList();
            campaign.CampaignDetails = detais;

            return campaign;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(int campaignId)
        {
            Campaign campaign = _campaignRepo.FindById(campaignId);
            _campaignRepo.Update(campaign);
        }
    }
}
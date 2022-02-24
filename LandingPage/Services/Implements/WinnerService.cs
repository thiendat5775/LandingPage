using AutoMapper;
using AutoMapper.QueryableExtensions;
using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;
using LandingPage.Infrastructure.Interfaces;
using LandingPage.WebApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandingPage.WebApi.Services.Implements
{
    public class WinnerService : IWinnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<CampaignDetail, int> _campaignDetailRepo;
        private readonly IRepository<Gift, int> _giftRepo;

        public WinnerService(IUnitOfWork unitOfWork, IMapper mapper,
            IRepository<CampaignDetail, int> campaignDetailRepo, IRepository<Gift, int> giftRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _campaignDetailRepo = campaignDetailRepo;
            _giftRepo = giftRepo;
        }

        public void Create(CampaignDetailViewModel campaignVm)
        {
            CampaignDetail campaignDetail = _mapper.Map<CampaignDetailViewModel, CampaignDetail>(campaignVm);
            _campaignDetailRepo.Add(campaignDetail);
        }

        public List<GiftViewModel> GetGiftByUser(Guid userId)
        {
            List<CampaignDetail> campaignDetails = _campaignDetailRepo.FindAll(x => x.WinnerId == userId).ToList();
            List<GiftViewModel> giftVms = new List<GiftViewModel>();
            foreach(CampaignDetail detail in campaignDetails)
            {
                GiftViewModel gift = _mapper.Map<Gift, GiftViewModel>(_giftRepo.FindById(detail.GiftId));
                giftVms.Add(gift);
            }
            return giftVms;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}

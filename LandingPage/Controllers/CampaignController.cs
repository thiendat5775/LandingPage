using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;
using LandingPage.Kernel.Commons.Response;
using LandingPage.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandingPage.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet("get-info")]
        public IActionResult GetCampaignInfo(int campaignId)
        {
            Campaign campaign = _campaignService.GetCampaignInfo(campaignId);
            if (campaign == null) return BadRequest(new ErrorResponse("Không tìm thấy chiến dịch khuyến mãi"));
            return Ok(campaign);
        }

        [HttpPost("create-or-updatecampaign")]
        public IActionResult SaveCampaign(CampaignViewModel campaignVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }
            if(campaignVm.Id == 0)
            {
                _campaignService.Create(campaignVm);
            }
            else
            {
                _campaignService.Update(campaignVm.Id);
            }
            _campaignService.Save();
            return Ok(new SuccessResponse("Tạo thành công"));
        }
        [HttpDelete("remove-campaign")]
        public IActionResult Delete(int campaignId)
        {
            Campaign campaign = _campaignService.GetCampaignInfo(campaignId);
            if (campaign == null) return BadRequest(new ErrorResponse("Không tìm thấy chiến dịch khuyến mãi"));
            _campaignService.Delete(campaignId);
            _campaignService.Save();
            return Ok(new SuccessResponse("Xoá thành công"));
        }
    }
}
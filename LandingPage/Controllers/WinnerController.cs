using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels.Campaigns;
using LandingPage.Kernel.Commons.Response;
using LandingPage.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandingPage.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class WinnerController : ControllerBase
    {
        private readonly IWinnerService _winnerService;
        private readonly IUserService _userService;

        public WinnerController(IWinnerService winnerService, IUserService userService)
        {
            _winnerService = winnerService;
            _userService = userService;
        }

        [HttpPost("winner")]
        public IActionResult CreateWinner(CampaignDetailViewModel campaignDetailVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage));
                return BadRequest(errorMessage);
            }
            _winnerService.Create(campaignDetailVm);
            _winnerService.Save();

            User user = _userService.FindUserById(campaignDetailVm.WinnerId);
            return Ok(new SuccessResponse($"Thêm phần thưởng thành công cho {user.FullName} ngày {campaignDetailVm.AwareDate}"));
        }
        [HttpGet("get-user-gift")]
        public IActionResult GetUserGift(Guid userId)
        {
            List<GiftViewModel> gifts = _winnerService.GetGiftByUser(userId);
            if (gifts == null) return NotFound(new SuccessResponse("Bạn không có phần thưởng nào"));
            return Ok(gifts);
        }
    }
}

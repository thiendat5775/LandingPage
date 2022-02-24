using LandingPage.Data.Entities;
using LandingPage.Data.Tranfer.ViewModels;
using LandingPage.Kernel.Commons.Response;
using LandingPage.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LandingPage.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public UserController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        [HttpPut("update-user")]
        [AllowAnonymous]
        public IActionResult Update(UserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorResponse(errorMessage));
            }
            
            if (_userService.CheckPhone(userVm.PhoneNumber)) return BadRequest(new ErrorResponse("Số điện thoại đã được sử dụng bởi tài khoản khác, vui lòng nhập số điện thoại khác"));

            string rawUserId = HttpContext.User.FindFirstValue("id");
            if (!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            User user = _userService.FindUserById(userId);
            user.FullName = userVm.FullName;
            user.BirthDay = userVm.BirthDay;
            user.PhoneNumber = userVm.PhoneNumber;
            if(userVm.BusinessTypeId != 0) user.BusinessTypeId = userVm.BusinessTypeId;
            if(userVm.PositionId != 0) user.PositionId = userVm.PositionId;
            _userService.Update(user);
            _userService.Save();
            return Ok(new SuccessResponse("Thông tin tài khoản của bạn đã được cập nhật"));
        }
    }
}

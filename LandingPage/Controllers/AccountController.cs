using LandingPage.Data.Entities;
using LandingPage.Kernel.Commons;
using LandingPage.Kernel.Commons.Response;
using LandingPage.WebApi.Models;
using LandingPage.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace LandingPage.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AccountController(IAccountService accountService, IUserService userService,
            IRefreshTokenService refreshTokenService)
        {
            _accountService = accountService;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel registerVm)
        {
            if (!ModelState.IsValid || registerVm == null)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorResponse(errorMessage));
            }
            User user = new User()
            {
                Id = Guid.NewGuid(),
                FullName = registerVm.FullName,
                BirthDay = registerVm.BirthDay,
                PhoneNumber = registerVm.PhoneNumber,
                PositionId = registerVm.PositionId,
                BusinessTypeId = registerVm.BusinessTypeId,
                UserName = registerVm.UserName,
                PasswordHash = _accountService.HashPassword(registerVm.Password)
            };
            User userExist = _userService.FindUserByName(registerVm.UserName);
            if (userExist != null) return Conflict(new ErrorResponse(Constant.UserConstant.Existed));
            _userService.Create(user);
            _userService.Save();
            return Ok(Constant.UserConstant.RegisterSuccess);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginViewModel loginVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }
            User user = _userService.FindUserByName(loginVm.UserName);
            if (user == null) return Unauthorized(new ErrorResponse(Constant.UserConstant.NotFound));

            _accountService.Authen(loginVm.UserName, loginVm.Password);
            if (_accountService.Success == true)
            {
                AuthenticationResponse response = GenerateAuthenticateResponse(user);
                return Ok(response);
            }
            else
            {
                return Unauthorized(new ErrorResponse(Constant.UserConstant.InValidPassword));
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshViewModel refreshVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessage = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errorMessage);
            }
            bool isValidRefeshToken = _accountService.Validate(refreshVm.RefreshToken);
            if (!isValidRefeshToken) return BadRequest(new ErrorResponse("Refresh Token không hợp lệ"));

            RefreshToken refreshTokenDTO = _refreshTokenService.GetByToken(refreshVm.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return NotFound(new ErrorResponse("Không tìm thấy Refresh Token "));
            }

            _refreshTokenService.Delete(refreshTokenDTO.Id);
            _refreshTokenService.Save();

            User user = _userService.FindUserById(refreshTokenDTO.UserId);
            if (user == null) return NotFound(new ErrorResponse("Không tìm thấy người dùng"));

            AuthenticationResponse response = GenerateAuthenticateResponse(user);
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");
            if(!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            _refreshTokenService.DeleteAll(userId);
            _refreshTokenService.Save();

            return NoContent();
        }

        [HttpGet("ramdom-otp")]
        public IActionResult GetOtp()
        {
            return Ok(_accountService.GenerateOtp());
        }

        private AuthenticationResponse GenerateAuthenticateResponse(User user)
        {
            string accessToken = _accountService.GenerateJWT(user);
            string refreshToken = _accountService.GenerateRefreshToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };
            _refreshTokenService.Create(refreshTokenDTO);
            _refreshTokenService.Save();

            return new AuthenticationResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
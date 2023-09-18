using ASK.API.Core.Entities;
using ASK.API.Core.Interfaces;
using ASK.API.Dtos.AccountDtos;
using ASK.API.Dtos.ErrorMessageDtos;
using ASK.API.Helpers;
using ASK.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using shortid.Configuration;
using shortid;
using UserManagement.Service.Models;
using UserManagement.Service.Services;
using System.Text;

namespace ASK.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly AskDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(UserManager<AuthUser> userManager, RoleManager<IdentityRole<long>> roleManager, AskDbContext context, ITokenService tokenService, IEmailService emailService, IWebHostEnvironment hostEnvironment, ILogger<IdentityController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (_context.Users.Any(U => U.Email == registerUserDto.Email))
            {
                return BadRequest("User already exists");
            }

            var phoneNumber = PhoneNumberFormater.FormatThePhoneNumber(registerUserDto.PhoneNumber);

            string fullName = registerUserDto.FullName;

            string[] nameParts = fullName.Split(new char[] { ' ' }, 2);

            string firstName = nameParts[0];

            string lastName = nameParts.Length > 1 ? nameParts[1] : null;

            var user = new AuthUser
            {
                PhoneNumber = phoneNumber,
                Email = registerUserDto.Email,
                UserName = registerUserDto.Email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                var customerRoleExists = await _roleManager.RoleExistsAsync("Customer");

                if (customerRoleExists)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

                    if (addToRoleResult.Succeeded)
                    {
                        var message = new Message(new string[]
                        {
                    registerUserDto.Email
                        },
                        "Ask App Kenya",
                        "You have successfully registered To Ask App Kenya.Please proceed to login to start enjoying the services."
                        );

                        _emailService.SendEmail(message);

                        var res = new
                        {
                            title = "Success",
                            description = "User registered successfully"
                        };

                        return Ok(res);
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);

                        return BadRequest(addToRoleResult.Errors);
                    }
                }
                else
                {
                    return BadRequest("The 'Customer' role does not exist.");
                }
            }

            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user != null)
            {
                if (!user.IsActive)
                {
                    var error = new ErrorMessage
                    {
                        title = "Error",
                        message = "This User Is Deactivated"
                    };

                    return BadRequest(error);
                }

                bool checkPasswordResult = await _userManager.CheckPasswordAsync(user, login.Password);

                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var jwtToken = _tokenService.CreateJwtToken(user, roles.ToList());

                    var res = new LoginResponseDto()
                    {
                        id = user.Id,
                        username = user.UserName,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        token = jwtToken
                    };

                    var jsonRes = JsonConvert.SerializeObject(res);

                    return Content(jsonRes, "application/json");
                }
                else
                {
                    var error = new ErrorMessage()
                    {
                        title = "Invalid Credentials",
                        message = "The Submitted Login Credentials are Invalid"
                    };

                    return BadRequest(error);

                }
            }

            var invalidCredentials = new ErrorMessage()
            {
                title = "Invalid Credentials",
                message = "The Submitted Login Credentials are Invalid"
            };

            return BadRequest(invalidCredentials);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto requestPasswordResetDto)
        {
            var user = await _userManager.FindByEmailAsync(requestPasswordResetDto.Email);

            if (user == null)
            {
                return NotFound("User Not Found");
            }
        

            string resetCode = ResetCodeHelper.GenerateResetCode();

            var encodedResetCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(resetCode));

            var hashedResetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            user.RealPasswordResetToken = hashedResetPasswordToken;

            user.PasswordResetToken = encodedResetCode;

            user.PasswordResetTokenExpiration = DateTimeOffset.UtcNow.AddDays(1);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var message = new Message(new string[]
                            {
                requestPasswordResetDto.Email

                            },
                            "Ask Reset Password Code",
                            resetCode
                            );

                _emailService.SendEmail(message);

                return Ok("Password reset token has been sent to your email,You may now reset password");
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user != null)
            {
                var encodedIncomingToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(resetPassword.Token));

                if (encodedIncomingToken == user.PasswordResetToken)
                {
                    if (user.PasswordResetTokenExpiration.HasValue && user.PasswordResetTokenExpiration.Value >= DateTimeOffset.UtcNow)
                    {
                        var realResetPasswordToken = user.RealPasswordResetToken;
                        
                        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, realResetPasswordToken, resetPassword.Password);

                        if (resetPasswordResult.Succeeded)
                        {
                            var message = new Message(new string[]
                            {
                        resetPassword.Email
                            },
                            "Password Reset Successfully",
                            "Your password has been successfully reset.");

                            _emailService.SendEmail(message);

                            return StatusCode(StatusCodes.Status200OK, new  { Title = "Success", Message = "Password changed successfully" });
                        }
                        else
                        {
                            foreach (var err in resetPasswordResult.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                            return Ok(ModelState);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ExpiredToken", "Password reset token has expired.");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    ModelState.AddModelError("InvalidToken", "Invalid password reset token.");
                    return BadRequest(ModelState);
                }
            }

            return BadRequest();
        }

        

        [HttpPost]
        [Route("Roles")]
        [ValidateModel]
        public async Task<IActionResult> CreateRoles([FromBody] RoleDto roleDto)
        {
            var role = new IdentityRole<long> { Name = roleDto.Name };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Name = role.Name });
            }

            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            _logger.LogError("Fetching Roles from Database");

            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }


    }
}

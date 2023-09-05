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
using System.Security.Cryptography;
using System.Text;
using UserManagement.Service.Models;
using UserManagement.Service.Services;
using static ASK.API.Dtos.AccountDtos.ResetPasswordDto;

namespace ASK.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly AskDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AuthUser> userManager, RoleManager<IdentityRole<long>> roleManager, AskDbContext context, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
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
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }
        [HttpPost]
        [Route("Register")]
        [ValidateModel]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (_context.Users.Any(U => U.Email == registerUserDto.Email))
            {
                return BadRequest("User Already Exists");
            }

            var password = registerUserDto.Password;

            var user = new AuthUser { UserName = registerUserDto.UserName, PhoneNumber = registerUserDto.PhoneNumber, Email = registerUserDto.Email, FirstName = registerUserDto.FirstName, LastName = registerUserDto.LastName, IsActive = true };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                if (registerUserDto.Roles != null && registerUserDto.Roles.Length > 0)
                {
                    foreach (var roleName in registerUserDto.Roles)
                    {
                        var existingRole = await _roleManager.FindByNameAsync(roleName);

                        if (existingRole != null)
                        {
                            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);

                            if (!addToRoleResult.Succeeded)
                            {
                                await _userManager.DeleteAsync(user);

                                return BadRequest(addToRoleResult.Errors);
                            }
                        }
                    }
                }

                var message = new Message(new string[]
                    {
                registerUserDto.Email

                    },
                    "ASK Password",
                    password
                    );

                _emailService.SendEmail(message);

                var res = new
                {
                    title = "Success",
                    description = "User Registered successfully with roles"
                };

                return Ok(res);
            }
            else
            {

                return BadRequest(result.Errors);
            }
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

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var message = new Message(new string[]
                        {
                requestPasswordResetDto.Email

                        },
                        "Ask Reset Password Code",
                        passwordResetToken
                        );

            _emailService.SendEmail(message);

            return Ok("Password reset token has been sent to your email,You may now reset password");

        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

                if (!resetPasswordResult.Succeeded)
                {
                    foreach(var err in resetPasswordResult.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);

                    }
                    return Ok(ModelState);
                }
                var message = new Message(new string[]
                        {
                resetPassword.Email

                        },
                        "Password Reset Successfully",
                        $"Your New ASK Password is {resetPassword.Password}"
                        );

                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK, new ErrorMessage { title = "Success", message = "Password Changed Successfully" });


            }

            return BadRequest();

        }
        [NonAction]


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

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
    }
}

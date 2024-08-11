﻿using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_DongHo_API.Data;
using Web_DongHo_API.Models;
using Web_DongHo_API.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Web_DongHo_API.Controllers
{
    public class UserLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }

    }
    public class UserRegistrationRequest
    {
        public string FullName { get; set; }
        public bool Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
    public class ChangeInfoRequest
    {
        public string FullName { get; set; }
        public bool? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
    }
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    public class Login_RegistrationResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
    }
    public class AutoLoginRequest
    {
        public string Token { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _web;
        public AccountController(AppDbContext context, ITokenService tokenService, EmailService emailService, IWebHostEnvironment web)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
            _web = web;
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", null, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                AllowRefresh = true
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                return BadRequest("Google authentication failed.");
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            }).ToList();

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (email == null)
            {
                return BadRequest("Email not found.");
            }
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                var userName = new string(fullName.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
                var newPassword = PasswordHelper.GeneratePassword(6);

                var newUser = new User
                {
                    Email = email,
                    UserName = userName,
                    FullName = fullName,
                    Password = PasswordHelper.GetMd5Hash(newPassword),
                    RoleId = 2
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                user = newUser;
            }

            var token = _tokenService.GenerateToken(user.UserName, user.RoleId);

            var redirectUrl = $"https://localhost:44395/authentication?token={token}";
            return Redirect(redirectUrl);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Invalid login request." });
            }

            var hashPassBeforeCheck = PasswordHelper.GetMd5Hash(request.Password);
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == request.UserName && x.Password == hashPassBeforeCheck);


            if (user != null)
            {
                var token = _tokenService.GenerateToken(user.UserName, user.RoleId);

                return Ok(new Login_RegistrationResponse
                {
                    Token = token,
                    UserName = user.UserName,
                    RoleId = user.RoleId
                });
            }

            return Unauthorized(new { message = "Invalid username or password." });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new ErrorResponse { Message = "Invalid registration details." });
            }

            var existingUser = await _context.Users
                .AnyAsync(x => x.UserName.Equals(request.UserName));

            if (existingUser)
            {
                return Conflict(new ErrorResponse { Message = "Username or email already exists." });
            }

            var hashedPassword = PasswordHelper.GetMd5Hash(request.Password);

            var newUser = new User
            {
                FullName = request.FullName,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                UserName = request.UserName,
                Password = hashedPassword,
                BirthDate = request.BirthDate,
                RoleId = 2
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(newUser.UserName, newUser.RoleId);

            return Ok(new Login_RegistrationResponse
            {
                Token = token,
                UserName = newUser.UserName,
                RoleId = newUser.RoleId
            });
        }
        [HttpPost("auto-login")]
        public IActionResult AutoLogin([FromBody] AutoLoginRequest request)
        {
            var principal = _tokenService.ValidateToken(request.Token);
            if (principal == null)
            {
                return Unauthorized(new ErrorResponse { Message = "Invalid token." });
            }

            var userName = principal.FindFirst(c => c.Type == "Name")?.Value;
            var roleId = int.Parse(principal.FindFirst(c => c.Type == "Role")?.Value);

            return Ok(new Login_RegistrationResponse
            {
                Token = request.Token,
                UserName = userName,
                RoleId = roleId
            });
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Invalid request. Email must be provided." });
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new { message = "Email không tồn tại." });
            }

            string newPassword = PasswordHelper.GeneratePassword(8);
            user.Password = PasswordHelper.GetMd5Hash(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(user.Email, "Quên mật khẩu", $"Mật khẩu mới của bạn là: {newPassword}");

            return Ok(new { message = "Mật khẩu mới đã được gửi qua email." });
        }

        [HttpPost("change-info")]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeInfoRequest user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "User not found." });
            }

            userToUpdate.FullName = user.FullName;
            userToUpdate.PhoneNumber = user.PhoneNumber;
            userToUpdate.Gender = user.Gender;
            userToUpdate.BirthDate = user.BirthDate;

            //if (file != null && file.Length > 0)
            //{
            //    var fileName = Path.GetFileName(file.FileName);
            //    var filePath = Path.Combine(_web.WebRootPath, "uploads", fileName);

            //    if (!System.IO.File.Exists(filePath))
            //    {
            //        using (var stream = new FileStream(filePath, FileMode.Create))
            //        {
            //            await file.CopyToAsync(stream);
            //        }
            //    }

            //    userToUpdate.Image = fileName;
            //}

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User information updated successfully." });
        }
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _context.Users
                .Where(u => u.UserName == username)
                .Select(u => new ChangeInfoRequest
                {
                    UserName = u.UserName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender ?? true,
                    BirthDate = u.BirthDate ?? DateTime.MinValue // Default to DateTime.MinValue if null
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }



        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string username,[FromBody] ChangePasswordRequest request)
        {
            // Hash the current password entered by the user
            var hashedCurrentPassword = PasswordHelper.GetMd5Hash(request.CurrentPassword);

            // Find the user by the hashed current password
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return BadRequest("Mật khẩu hiện tại không đúng.");
            }

            // Ensure new password and confirm password match
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return BadRequest("Mật khẩu mới và xác nhận mật khẩu không khớp.");
            }

            // Update the user's password with the new hashed password
            user.Password = PasswordHelper.GetMd5Hash(request.ConfirmNewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Đổi mật khẩu thành công.");
        }


    }
}

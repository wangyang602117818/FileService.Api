using FileService.Api.Models;
using FileService.Business;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        User user = new User();
        public ResponseItem<string> Index()
        {

            return new ResponseItem<string>(ErrorCode.success, "file servivce api home page");
        }
        [AllowAnonymous]
        [HttpPost]
        public ResponseItem<string> Login(LoginForm loginForm)
        {
            BsonDocument bsonUser = user.Login(loginForm.UserName, loginForm.PassWord);
            if (bsonUser == null) return new ResponseItem<string>(ErrorCode.invalid_username_or_password, "");
            var claims = new[] {
                new Claim(ClaimTypes.Name,bsonUser["UserName"].AsString),
                new Claim(ClaimTypes.Role,bsonUser["Role"].AsString)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
              AppSettings.Configuration["jwt:issuer"],
              AppSettings.Configuration["jwt:issuer"],
              claims,
              null,
              DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59),
              signingCredentials: creds);
            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return new ResponseItem<string>(ErrorCode.success, tokenStr);
        }

    }
}
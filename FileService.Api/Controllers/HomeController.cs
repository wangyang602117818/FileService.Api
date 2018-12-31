using FileService.Api.Models;
using FileService.Business;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : BaseController
    {
        User user = new User();
        public ResponseItem<string> Index()
        {
            return new ResponseItem<string>(ErrorCode.success, "file servivce api home page");
        }
        [HttpPost]
        public ResponseItem<string> Login(LoginForm loginForm)
        {
            BsonDocument bsonUser = user.Login(loginForm.UserName, loginForm.PassWord);
            if (bsonUser == null) return new ResponseItem<string>(ErrorCode.invalid_username_or_password, "");
            string userName = bsonUser["UserName"].AsString;
            string role = bsonUser["Role"].AsString;
            BsonDocument app = application.FindByAuthCode(loginForm.AuthCode);
            if (app == null) return new ResponseItem<string>(ErrorCode.invalid_authcode, "");
            string appName = app["ApplicationName"].AsString;
            if (!string.IsNullOrEmpty(loginForm.Code))
            {
                string openId = GetOpenId(loginForm.Code);
                user.UpdateUser(userName, new BsonDocument("OpenId", openId));
            }
            LogInRecord("Login", appName, userName, loginForm.ApiType);
            return new ResponseItem<string>(ErrorCode.success, GetToken(userName, appName, loginForm.ApiType, role));
        }
        [Authorize]
        [HttpGet]
        public ResponseItem<string> LogOut()
        {
            if (user.UpdateUser(User.Identity.Name, new BsonDocument("OpenId", "")))
            {
                Log("-", "LogOut");
                return new ResponseItem<string>(ErrorCode.success, "");
            }
            return new ResponseItem<string>(ErrorCode.server_exception, "");
        }
        [HttpPost]
        public ResponseItem<string> WeChatLogin(WeChatLoginForm weChatLogin)
        {
            string openId = GetOpenId(weChatLogin.Code);
            BsonDocument app = application.FindByAuthCode(weChatLogin.AuthCode);
            if (app == null) return new ResponseItem<string>(ErrorCode.invalid_authcode, "");
            if (!string.IsNullOrEmpty(openId))
            {
                BsonDocument bsonUser = user.GetUserByOpenId(openId);
                if (bsonUser == null) return new ResponseItem<string>(ErrorCode.invalid_code, "");
                string userName = bsonUser["UserName"].AsString;
                string role = bsonUser["Role"].AsString;
                string appName = app["ApplicationName"].AsString;
                LogInRecord("Login", appName, userName, weChatLogin.ApiType);
                return new ResponseItem<string>(ErrorCode.success, GetToken(userName, appName, weChatLogin.ApiType, role));
            }
            else
            {
                return new ResponseItem<string>(ErrorCode.invalid_code, "");
            }
        }
        private string GetToken(string userName, string appName, string apiType, params string[] roles)
        {
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, userName),
                new Claim("AppName",appName),
                new Claim("ApiType",apiType)
            };
            foreach (string role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
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
            return tokenStr;
        }
        private string GetOpenId(string code)
        {
            string url = AppSettings.Configuration["weChat:openIdUrl"] + "?appid=" + AppSettings.Configuration["weChat:appId"] + "&secret=" + AppSettings.Configuration["weChat:appSecret"] + "&js_code=" + code + "&grant_type=authorization_code";
            BsonDocument result = BsonDocument.Parse(new HttpRequestHelper().Get(url).Result);
            if (result.Contains("openid")) return result["openid"].AsString;
            return "";
        }
    }
}
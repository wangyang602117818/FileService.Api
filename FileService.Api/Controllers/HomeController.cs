using FileService.Api.Models;
using FileService.Business;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileService.Api.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        [AllowAnonymous]
        public ResponseItem<string> Index()
        {
            return new ResponseItem<string>(ErrorCode.success, "file servivce api home page");
        }
        [HttpPost]
        public ResponseItem<string> Login(LoginForm loginForm)
        {
            BsonDocument bsonUser = user.Login(loginForm.UserCode, loginForm.PassWord);
            if (bsonUser == null) return new ResponseItem<string>(ErrorCode.invalid_username_or_password, "");
            string userCode = bsonUser["UserCode"].ToString();
            string userName = bsonUser["UserName"].AsString;
            string role = bsonUser["Role"].AsString;
            BsonDocument app = application.FindByAuthCode(loginForm.AuthCode);
            if (app == null) return new ResponseItem<string>(ErrorCode.invalid_authcode, "");
            string appName = app["ApplicationName"].AsString;
            if (!string.IsNullOrEmpty(loginForm.Code))
            {
                string openId = GetOpenId(loginForm.Code);
                user.UpdateUser(userCode, new BsonDocument("OpenId", openId));
            }
            LogInRecord("Login", appName, userName, loginForm.ApiType);
            return new ResponseItem<string>(ErrorCode.success, GetToken(userCode, userName, appName, loginForm.ApiType, role));
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
                string userCode = bsonUser["UserCode"].ToString();
                string userName = bsonUser["UserName"].AsString;
                string role = bsonUser["Role"].AsString;
                string appName = app["ApplicationName"].AsString;
                LogInRecord("Login", appName, userName, weChatLogin.ApiType);
                return new ResponseItem<string>(ErrorCode.success, GetToken(userCode, userName, appName, weChatLogin.ApiType, role));
            }
            else
            {
                return new ResponseItem<string>(ErrorCode.invalid_code, "");
            }
        }
        public ResponseItem<string> LogOut()
        {
            if (user.UpdateUser(User.Identity.Name, new BsonDocument("OpenId", "")))
            {
                Log("-", "LogOut");
                return new ResponseItem<string>(ErrorCode.success, "");
            }
            return new ResponseItem<string>(ErrorCode.server_exception, "");
        }
        public IActionResult GetUser(string userCode)
        {
            BsonDocument userBson = user.GetUser(userCode);
            userBson.Remove("PassWord");
            return new ResponseModel<BsonDocument>(ErrorCode.success, userBson);
        }
        public IActionResult GetCount()
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            result.Add("recyle", filesWrap.CountDeleted());
            result.Add("log", log.Count());
            result.Add("extension", extension.Count());
            result.Add("application", application.Count());
            result.Add("user", user.Count());
            result.Add("company", department.Count());
            return new ResponseModel<Dictionary<string, long>>(ErrorCode.success, result);
        }
        public IActionResult ChangePassword(string password)
        {
            BsonDocument userBson = new BsonDocument()
            {
                {"PassWord",password.ToMD5() },
                {"Modified",true },
                {"OpenId","" },
                {"UpdateTime",DateTime.Now }
            };
            if (user.UpdateUser(User.Identity.Name, userBson))
            {
                return new ResponseModel<string>(ErrorCode.success, "");
            }
            else
            {
                return new ResponseModel<string>(ErrorCode.server_exception, "");
            }
        }
        [AllowAnonymous]
        private string GetToken(string userCode, string userName, string appName, string apiType, params string[] roles)
        {
            var claims = new List<Claim>() {
                new Claim("UserName",userName),
                new Claim(ClaimTypes.Name, userCode),
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
        [AllowAnonymous]
        private string GetOpenId(string code)
        {
            string url = AppSettings.Configuration["weChat:openIdUrl"] + "?appid=" + AppSettings.Configuration["weChat:appId"] + "&secret=" + AppSettings.Configuration["weChat:appSecret"] + "&js_code=" + code + "&grant_type=authorization_code";
            BsonDocument result = BsonDocument.Parse(new HttpRequestHelper().Get(url).Result);
            if (result.Contains("openid")) return result["openid"].AsString;
            return "";
        }
    }
}
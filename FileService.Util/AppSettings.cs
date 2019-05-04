using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Util
{
    public class AppSettings
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static Settings Settings { get; set; }
        static AppSettings()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
    public class Settings
    {
        public Jwt Jwt { get; set; }
        public WeChat WeChat { get; set; }
        public Mognodb Mognodb { get; set; }
        public string TempFileDir { get; set; }
    }
    public class Jwt
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
    }
    public class WeChat
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string OpenIdUrl { get; set; }
    }
    public class Mognodb
    {
        public string ConntionString { get; set; }
        public string Database { get; set; }
    }

}

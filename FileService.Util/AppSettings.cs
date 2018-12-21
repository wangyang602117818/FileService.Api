using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileService.Util
{
    public class AppSettings
    {
        public static IConfigurationRoot Configuration { get; set; }
        static AppSettings()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
   
}

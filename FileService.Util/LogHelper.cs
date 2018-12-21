using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;

namespace FileService.Util
{
    public static class LogHelper
    {
        static string repositoryName = "NETCoreRepository";
        static ILoggerRepository repository = LogManager.CreateRepository(repositoryName);
        static ILog log = LogManager.GetLogger(repositoryName, "FileLogAppender");
        static LogHelper()
        {
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }
        /// <summary>
        /// 错误日志 
        /// </summary>
        /// <param name="ex">异常信息</param>
        public static void ErrorLog(Exception ex)
        {
            log.Error(ex);
        }
        /// <summary>
        /// 错误日志 
        /// </summary>
        /// <param name="str">异常信息</param>
        public static void ErrorLog(string str)
        {
            log.Error(str);
        }
        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="ex"></param>
        public static void InfoLog(Exception ex)
        {
            log.Error(ex);
        }
        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="str"></param>
        public static void InfoLog(string str)
        {
            log.Info(str);
        }

    }
}

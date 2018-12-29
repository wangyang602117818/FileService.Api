using FileService.Business;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace FileService.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Log log = new Log();
        protected MongoFile mongoFile = new MongoFile();
        protected MongoFileConvert mongoFileConvert = new MongoFileConvert();
        protected Download download = new Download();
        protected FilesWrap filesWrap = new FilesWrap();
        protected Application application = new Application();
        protected ActionResult GetSourceFile(ObjectId id, string contentType, string fileName)
        {
            GridFSDownloadStream stream = mongoFile.DownLoad(id);
            return File(stream, contentType, fileName);
        }
        protected ActionResult GetConvertFile(ObjectId id)
        {
            GridFSDownloadStream stream = mongoFileConvert.DownLoad(id);
            return File(stream, stream.FileInfo.Metadata["ContentType"].AsString, stream.FileInfo.Filename);
        }
        protected ObjectId GetObjectIdFromId(string id)
        {
            ObjectId newId = ObjectId.Empty;
            ObjectId.TryParse(id, out newId);
            return newId;
        }
        protected void Log(string fileId, string content)
        {
            var authCode = Request.Headers["AuthCode"];
            var appName = Request.Headers["AppName"];
            string apiType = Request.Headers["ApiType"];
            string userIp = Request.Headers["UserIp"];
            string userAgent = Request.Headers["UserAgent"];
            string userAgent1 = Request.Headers["User-Agent"];

            if (string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(authCode))
            {
                appName = new Application().FindByAuthCode(authCode)["ApplicationName"].AsString;
            }
            log.Insert(appName, fileId, content, User.Identity.Name,
                apiType ?? "",
                userIp ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                userAgent ?? userAgent1);
        }
        protected void AddDownload(ObjectId fileWrapId)
        {
            string appName = Request.Headers["AppName"];
            string userIp = Request.Headers["UserIp"];
            string userAgent = Request.Headers["UserAgent"];
            string userAgent1 = Request.Headers["User-Agent"];
            if (!download.AddedInOneMinute(appName, fileWrapId, User.Identity.Name))
            {
                download.AddDownload(fileWrapId, appName, User.Identity.Name,
                    userIp ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                    userAgent ?? userAgent1);
                filesWrap.AddDownloads(fileWrapId);
            }
        }
    }
}
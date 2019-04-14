using FileService.Business;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.IO;
using System.Linq;

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
        protected Queue queue = new Queue();
        protected Converter converter = new Converter();
        protected Extension extension = new Extension();
        protected Department department = new Department();
        protected M3u8 m3u8 = new M3u8();
        protected Ts ts = new Ts();
        protected Task task = new Task();
        protected User user = new User();
        private readonly IHostingEnvironment _hostingEnvironment;
        public BaseController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
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
            var appName = User.Claims.Where(w => w.Type == "AppName").FirstOrDefault().Value;
            string apiType = User.Claims.Where(w => w.Type == "ApiType").FirstOrDefault().Value;
            string userIp = Request.Headers["UserIp"];
            string userAgent = Request.Headers["UserAgent"];
            string userAgent1 = Request.Headers["User-Agent"];
            log.Insert(appName, fileId, content,
                User.Identity.Name,
                apiType,
                userIp ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                userAgent ?? userAgent1);
        }
        protected void LogInRecord(string content, string appName, string userName, string apiType)
        {
            string userIp = Request.Headers["UserIp"];
            string userAgent = Request.Headers["UserAgent"];
            string userAgent1 = Request.Headers["User-Agent"];
            log.Insert(appName, "-", content,
                userName,
                apiType,
                userIp ?? HttpContext.Connection.RemoteIpAddress.ToString(),
                userAgent ?? userAgent1);
        }
        protected void AddDownload(ObjectId fileWrapId)
        {
            var appName = User.Claims.Where(w => w.Type == "AppName").FirstOrDefault().Value;
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
        protected IActionResult GetIcon(BsonDocument file,string ext)
        {
            string imagePath = _hostingEnvironment.WebRootPath + "\\images\\";
            if (file == null)
            {
                string type = extension.GetTypeByExtension(ext).ToLower();
                switch (type)
                {
                    case "text":
                    case "video":
                    case "image":
                    case "attachment":
                    case "audio":
                    case "pdf":
                        return File(System.IO.File.ReadAllBytes(imagePath + type + ".png"), "image/png");
                    case "office":
                        if (ext == ".doc" || ext == ".docx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "word.png"), "image/png");
                        if (ext == ".xls" || ext == ".xlsx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "excel.png"), "image/png");
                        if (ext == ".ppt" || ext == ".pptx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "ppt.png"), "image/png");
                        if (new string[] { ".odg", ".ods", ".odp", ".odf", ".odt" }.Contains(ext))
                            return File(System.IO.File.ReadAllBytes(imagePath + "libreoffice.png"), "image/png");
                        if (new string[] { ".wps", ".dps", ".et" }.Contains(ext))
                            return File(System.IO.File.ReadAllBytes(imagePath + "wps.png"), "image/png");
                        return File(System.IO.File.ReadAllBytes(imagePath + "attachment.png"), "image/png");
                    default:
                        return File(System.IO.File.ReadAllBytes(imagePath + "attachment.png"), "image/png");
                }
            }
            string contentType = Extension.GetContentType(Path.GetExtension(file["FileName"].AsString.ToLower()).ToLower());
            return File(file["File"].AsByteArray, contentType);
        }
        protected void RemoveFile(string id)
        {
            ObjectId fileWrapId = ObjectId.Parse(id);
            BsonDocument fileWrap = filesWrap.FindOne(fileWrapId);
            if (filesWrap == null) return;
            task.RemoveByFileId(fileWrapId);
            filesWrap.Remove(fileWrapId);
        }
    }
}
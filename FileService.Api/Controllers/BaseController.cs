using FileService.Business;
using FileService.Model;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected Log log = new Log();
        protected Regex regex = new Regex(@"\\(\w+)\\$");
        protected Converter converter = new Converter();
        protected Task task = new Task();
        protected Queue queue = new Queue();
        protected Department department = new Department();
        protected FilesWrap filesWrap = new FilesWrap();
        protected Thumbnail thumbnail = new Thumbnail();
        protected M3u8 m3u8 = new M3u8();
        protected Ts ts = new Ts();
        protected VideoCapture videoCapture = new VideoCapture();
        protected FilesConvert filesConvert = new FilesConvert();
        protected MongoFile mongoFile = new MongoFile();
        protected MongoFileConvert mongoFileConvert = new MongoFileConvert();
        protected FilePreview filePreview = new FilePreview();
        protected FilePreviewMobile filePreviewMobile = new FilePreviewMobile();
        protected Shared shared = new Shared();
        protected Download download = new Download();
        protected Application application = new Application();
        protected Extension extension = new Extension();
        protected User user = new User();
        protected readonly IHostingEnvironment _hostingEnvironment;
        public BaseController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        protected void InsertTask(string handlerId, ObjectId fileId, string fileName, string type, string from, BsonDocument outPut, BsonArray access, string owner)
        {
            converter.AddCount(handlerId, 1);
            ObjectId taskId = ObjectId.GenerateNewId();
            task.Insert(taskId, fileId, DateTime.Now.ToString("yyyyMMdd"), fileName,
                type, from, outPut, access, owner, handlerId, 0, TaskStateEnum.wait, 0);
            //添加队列
            queue.Insert(handlerId, type, "Task", taskId, false, new BsonDocument());
        }
        protected void UpdateTask(ObjectId id, string handlerId, string fileName, string type, int percent, TaskStateEnum state)
        {
            converter.AddCount(handlerId, 1);
            BsonDocument item = new BsonDocument()
            {
                {"Folder",DateTime.Now.ToString("yyyyMMdd") },
                {"FileName",fileName },
                {"ProcessCount",0 },
                {"State",state },
                {"StateDesc",state.ToString() },
                {"Percent",percent }
            };
            task.Update(id, item);
            queue.Insert(handlerId, type, "Task", id, false, new BsonDocument());
        }
        protected void ConvertAccess(List<AccessModel> accessList)
        {
            foreach (AccessModel accessModel in accessList)
            {
                string companyName = "";
                List<string> departmentDisplay = new List<string>() { };
                accessModel.Authority = "0";
                accessModel.AccessCodes = accessModel.DepartmentCodes;
                department.GetNamesByCodes(accessModel.Company, accessModel.DepartmentCodes, out companyName, out departmentDisplay);
                accessModel.CompanyDisplay = companyName;
                accessModel.DepartmentDisplay = departmentDisplay.ToArray();
            }
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
        protected string GetTempFilePath(BsonDocument task)
        {
            return AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + task["Folder"].ToString() + "\\" + task["FileId"].ToString() + Path.GetExtension(task["FileName"].ToString());
        }
        protected string GetTempFilePath(string folder, string fileId, string fileName)
        {
            return AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + folder + "\\" + fileId + Path.GetExtension(fileName);
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
        protected void LogInRecord(string content, string appName, string userCode, string apiType)
        {
            string userIp = Request.Headers["UserIp"];
            string userAgent = Request.Headers["UserAgent"];
            string userAgent1 = Request.Headers["User-Agent"];
            log.Insert(appName, "-", content,
                userCode,
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
        protected bool DeleteFile(string id)
        {
            Log(id, "DeleteFile");
            ObjectId fileWrapId = ObjectId.Parse(id);
            BsonDocument fileWrap = filesWrap.FindOne(fileWrapId);
            if (fileWrap == null) return false;
            //删除 thumbnail
            if (fileWrap["FileType"] == "image" && fileWrap.Contains("Thumbnail"))
            {
                List<ObjectId> thumbnailIds = new List<ObjectId>();
                foreach (BsonDocument d in fileWrap["Thumbnail"].AsBsonArray) thumbnailIds.Add(d["_id"].AsObjectId);
                thumbnail.DeleteMany(thumbnailIds);
            }
            //删除 video 相关
            if (fileWrap["FileType"] == "video" && fileWrap.Contains("Videos"))
            {
                List<ObjectId> m3u8Ids = new List<ObjectId>();
                List<ObjectId> videoCpIds = new List<ObjectId>();
                foreach (BsonDocument d in fileWrap["Videos"].AsBsonArray)
                {
                    ObjectId fileId = d["_id"].AsObjectId;
                    if (d["Format"].AsInt32 == 0) m3u8Ids.Add(fileId);
                };
                IEnumerable<BsonDocument> m3u8s = m3u8.FindByIds(m3u8Ids);
                foreach (BsonDocument m3u8 in m3u8s)
                {
                    List<ObjectId> tsIds = m3u8["File"].AsString.GetTsIds();
                    ts.DeleteByIds(m3u8["From"].AsString, m3u8["_id"].AsObjectId, tsIds);
                }
                foreach (BsonObjectId oId in fileWrap["VideoCpIds"].AsBsonArray) videoCpIds.Add(oId.AsObjectId);
                m3u8.DeleteMany(m3u8Ids);
                videoCapture.DeleteByIds(fileWrap["From"].AsString, videoCpIds);
            }
            //删除 attachment 相关
            if (fileWrap["FileType"] == "office" || fileWrap["FileType"] == "attachment")
            {
                foreach (BsonDocument bson in fileWrap["Files"].AsBsonArray)
                {
                    if (!bson.Contains("_id")) continue;
                    if (filesConvert.FindOne(bson["_id"].AsObjectId) != null) mongoFileConvert.Delete(bson["_id"].AsObjectId);
                }
                if (fileWrap.Contains("VideoCpIds"))
                {
                    videoCapture.DeleteByIds(fileWrap["From"].AsString, fileWrap["VideoCpIds"].AsBsonArray.Select(s => s.AsObjectId));
                }
            }
            //如果源文件没有被引用，则删除
            if (filesWrap.CountByFileId(fileWrap["FileId"].AsObjectId) == 1 && fileWrap["FileId"].AsObjectId != ObjectId.Empty)
            {
                ObjectId fId = fileWrap["FileId"].AsObjectId;
                mongoFile.Delete(fId);
                //删除转换的小图标
                filePreview.DeleteOne(fId);
                //删除转换的大图标
                filePreviewMobile.DeleteOne(fId);
            }
            //删除缓存文件
            IEnumerable<BsonDocument> tasks = task.FindCacheFiles(fileWrapId);
            foreach (BsonDocument task in tasks)
            {
                string fullPath = GetTempFilePath(task);
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
            //删除共享信息
            shared.DeleteShared(fileWrapId);
            task.DeleteByFileId(fileWrapId);
            filesWrap.DeleteOne(fileWrapId);
            return true;
        }
        protected void DeleteSubFiles(BsonDocument fileWrap)
        {
            //删除 thumbnail
            if (fileWrap["FileType"] == "image" && fileWrap.Contains("Thumbnail"))
            {
                List<ObjectId> thumbnailIds = new List<ObjectId>();
                foreach (BsonDocument d in fileWrap["Thumbnail"].AsBsonArray) thumbnailIds.Add(d["_id"].AsObjectId);
                thumbnail.DeleteMany(thumbnailIds);
            }
            //删除 video 相关
            if (fileWrap["FileType"] == "video" && fileWrap.Contains("Videos"))
            {
                List<ObjectId> m3u8Ids = new List<ObjectId>();
                List<ObjectId> videoCpIds = new List<ObjectId>();
                foreach (BsonDocument d in fileWrap["Videos"].AsBsonArray)
                {
                    ObjectId fileId = d["_id"].AsObjectId;
                    if (d["Format"].AsInt32 == 0) m3u8Ids.Add(fileId);
                }
                IEnumerable<BsonDocument> m3u8s = m3u8.FindByIds(m3u8Ids);
                foreach (BsonDocument m3u8 in m3u8s)
                {
                    List<ObjectId> tsIds = m3u8["File"].AsString.GetTsIds();
                    ts.DeleteByIds(m3u8["From"].AsString, m3u8["_id"].AsObjectId, tsIds);
                }
                foreach (BsonObjectId oId in fileWrap["VideoCpIds"].AsBsonArray) videoCpIds.Add(oId.AsObjectId);
                m3u8.DeleteMany(m3u8Ids);
                videoCapture.DeleteByIds(fileWrap["From"].AsString, videoCpIds);
            }
            // 删除 attachment 相关
            if (fileWrap["FileType"] == "office" || fileWrap["FileType"] == "attachment")
            {
                foreach (BsonDocument bson in fileWrap["Files"].AsBsonArray)
                {
                    if (!bson.Contains("_id")) continue;
                    if (filesConvert.FindOne(bson["_id"].AsObjectId) != null) mongoFileConvert.Delete(bson["_id"].AsObjectId);
                }
                if (fileWrap.Contains("VideoCpIds"))
                {
                    videoCapture.DeleteByIds(fileWrap["From"].AsString, fileWrap["VideoCpIds"].AsBsonArray.Select(s => s.AsObjectId));
                }
            }
            //如果源文件没有被引用，则删除转换的大图标和小图标
            if (filesWrap.CountByFileId(fileWrap["FileId"].AsObjectId) == 1 && fileWrap["FileId"].AsObjectId != ObjectId.Empty)
            {
                ObjectId fId = fileWrap["FileId"].AsObjectId;
                //删除转换的小图标
                filePreview.DeleteOne(fId);
                //删除转换的大图标
                filePreviewMobile.DeleteOne(fId);
            }
        }
    }
}
using FileService.Api.Models;
using FileService.Model;
using FileService.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileService.Api.Controllers
{
    public class UploadController : BaseController
    {
        string tempFileDirectory = AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + DateTime.Now.ToString("yyyyMMdd") + "\\";
        public UploadController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        [HttpPost]
        public IActionResult Image([FromForm]UploadImgModel uploadImgModel)
        {
            List<FileResponse> response = new List<FileResponse>();
            List<ImageOutPut> output = new List<ImageOutPut>();
            List<AccessModel> accessList = new List<AccessModel>();
            if (Request.Headers["DefaultConvert"] == "true")
            {
                output = JsonConvert.DeserializeObject<List<ImageOutPut>>(Request.Headers["Thumbnails"]);
            }
            else
            {
                if (!string.IsNullOrEmpty(uploadImgModel.OutPut))
                {
                    output = JsonConvert.DeserializeObject<List<ImageOutPut>>(uploadImgModel.OutPut);
                }
            }
            if (!string.IsNullOrEmpty(uploadImgModel.Access))
            {
                accessList = JsonConvert.DeserializeObject<List<AccessModel>>(uploadImgModel.Access);
                ConvertAccess(accessList);
            }
            if (!Directory.Exists(tempFileDirectory))
                Directory.CreateDirectory(tempFileDirectory);
            foreach (IFormFile file in uploadImgModel.Images)
            {
                //过滤不正确的格式
                string contentType = "";
                string fileType = "";
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!extension.CheckFileExtensionImage(ext, ref contentType, ref fileType))
                {
                    response.Add(new FileResponse()
                    {
                        FileId = ObjectId.Empty.ToString(),
                        FileName = file.FileName,
                        SubFiles = new List<SubFileItem>()
                    });
                    continue;
                }
                //要存到表中的数据
                BsonArray thumbnail = new BsonArray();
                foreach (ImageOutPut thumb in output)
                {
                    thumb.Id = ObjectId.GenerateNewId();
                    thumbnail.Add(new BsonDocument()
                        {
                            {"_id",thumb.Id },
                            {"Format",thumb.Format },
                            {"Flag", thumb.Flag}
                        });
                }
                BsonArray access = new BsonArray(accessList.Select(a => a.ToBsonDocument()));
                ObjectId fileId = ObjectId.GenerateNewId();
                //上传到TempFiles
                using (FileStream stream = new FileStream(tempFileDirectory + fileId.ToString() + ext, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                string appName = User.Claims.Where(w => w.Type == "AppName").First().Value;
                filesWrap.InsertImage(fileId, ObjectId.Empty, file.FileName, file.Length, appName, 0, fileType, contentType, thumbnail, access, uploadImgModel.ExpiredDay, User.Identity.Name);

                string handlerId = converter.GetHandlerId();
                if (output.Count == 0)
                {
                    InsertTask(handlerId, fileId, file.FileName, fileType, appName, new BsonDocument(), access, User.Identity.Name);
                }
                else
                {
                    foreach (ImageOutPut o in output)
                    {
                        InsertTask(handlerId, fileId, file.FileName, fileType, appName, o.ToBsonDocument(), access, User.Identity.Name);
                    }
                }
                //日志
                Log(fileId.ToString(), "UploadImage");
                response.Add(new FileResponse()
                {
                    FileId = fileId.ToString(),
                    FileName = file.FileName,
                    FileSize = file.Length,
                    SubFiles = thumbnail.Select(sel => new SubFileItem() { FileId = sel["_id"].ToString(), Flag = sel["Flag"].ToString() })
                });
            }
            return new ResponseModel<IEnumerable<FileResponse>>(ErrorCode.success, response);
        }
        [HttpPost]
        public IActionResult Video([FromForm]UploadVideoModel uploadVideoModel)
        {
            List<FileResponse> response = new List<FileResponse>();
            List<VideoOutPut> outputs = new List<VideoOutPut>();
            List<AccessModel> accessList = new List<AccessModel>();
            if (Request.Headers["DefaultConvert"] == "true")
            {
                outputs = JsonConvert.DeserializeObject<List<VideoOutPut>>(Request.Headers["Videos"]);
            }
            else
            {
                if (!string.IsNullOrEmpty(uploadVideoModel.OutPut))
                {
                    outputs = JsonConvert.DeserializeObject<List<VideoOutPut>>(uploadVideoModel.OutPut);
                }
            }
            if (!string.IsNullOrEmpty(uploadVideoModel.Access))
            {
                accessList = JsonConvert.DeserializeObject<List<AccessModel>>(uploadVideoModel.Access);
                ConvertAccess(accessList);
            }
            if (!Directory.Exists(tempFileDirectory))
                Directory.CreateDirectory(tempFileDirectory);
            foreach (IFormFile file in uploadVideoModel.Videos)
            {
                //过滤不正确的格式
                string contentType = "";
                string fileType = "";
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!extension.CheckFileExtensionVideo(ext, ref contentType, ref fileType))
                {
                    response.Add(new FileResponse()
                    {
                        FileId = ObjectId.Empty.ToString(),
                        FileName = file.FileName,
                        SubFiles = new List<SubFileItem>()
                    });
                    continue;
                }
                //要存到表中的数据
                BsonArray videos = new BsonArray();
                foreach (VideoOutPut output in outputs)
                {
                    output.Id = ObjectId.GenerateNewId();
                    videos.Add(new BsonDocument()
                    {
                        {"_id",output.Id },
                        {"Format",output.Format },
                        {"Flag",output.Flag }
                    });
                }
                BsonArray access = new BsonArray(accessList.Select(a => a.ToBsonDocument()));
                ObjectId fileId = ObjectId.GenerateNewId();
                //上传到TempFiles
                using (FileStream stream = new FileStream(tempFileDirectory + fileId.ToString() + ext, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                string appName = User.Claims.Where(w => w.Type == "AppName").First().Value;
                filesWrap.InsertVideo(fileId, ObjectId.Empty, file.FileName, file.Length, appName, 0, fileType, contentType, videos, access, uploadVideoModel.ExpiredDay, User.Identity.Name);

                string handlerId = converter.GetHandlerId();
                if (outputs.Count == 0)
                {
                    InsertTask(handlerId, fileId, file.FileName, "video", appName, new BsonDocument(), access, User.Identity.Name);
                }
                else
                {
                    foreach (VideoOutPut o in outputs)
                    {
                        InsertTask(handlerId, fileId, file.FileName, "video", appName, o.ToBsonDocument(), access, User.Identity.Name);
                    }
                }
                //日志
                Log(fileId.ToString(), "UploadVideo");

                response.Add(new FileResponse()
                {
                    FileId = fileId.ToString(),
                    FileName = file.FileName,
                    FileSize = file.Length,
                    SubFiles = videos.Select(sel => new SubFileItem() { FileId = sel["_id"].ToString(), Flag = sel["Flag"].AsString })
                });
            }
            return new ResponseModel<IEnumerable<FileResponse>>(ErrorCode.success, response);
        }
        [HttpPost]
        public IActionResult Attachment([FromForm]UploadAttachmentModel uploadAttachmentModel)
        {
            List<FileResponse> response = new List<FileResponse>();
            List<AccessModel> accessList = new List<AccessModel>();
            if (!string.IsNullOrEmpty(uploadAttachmentModel.Access))
            {
                accessList = JsonConvert.DeserializeObject<List<AccessModel>>(uploadAttachmentModel.Access);
                ConvertAccess(accessList);
            }
            if (!Directory.Exists(tempFileDirectory))
                Directory.CreateDirectory(tempFileDirectory);
            foreach (IFormFile file in uploadAttachmentModel.Attachments)
            {
                string ext = Path.GetExtension(file.FileName).ToLower();
                //过滤不正确的格式
                string contentType = "";
                string fileType = "";
                if (!extension.CheckFileExtension(ext, ref contentType, ref fileType))
                {
                    response.Add(new FileResponse()
                    {
                        FileId = ObjectId.Empty.ToString(),
                        FileName = file.FileName
                    });
                    continue;
                }
                BsonArray files = new BsonArray();
                //office
                if (fileType.ToLower() == "office")
                {
                    files.Add(new BsonDocument() {
                        {"_id",ObjectId.Empty },
                        {"Format",AttachmentOutput.pdf },
                        {"Flag","preview" }
                    });
                }
                BsonArray access = new BsonArray(accessList.Select(a => a.ToBsonDocument()));
                ObjectId fileId = ObjectId.GenerateNewId();
                //上传到TempFiles
                using (FileStream stream = new FileStream(tempFileDirectory + fileId.ToString() + ext, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                string appName = User.Claims.Where(w => w.Type == "AppName").First().Value;
                filesWrap.InsertAttachment(fileId, ObjectId.Empty, file.FileName, fileType, file.Length, appName, 0, contentType, files, access, uploadAttachmentModel.ExpiredDay, User.Identity.Name);

                string handlerId = converter.GetHandlerId();
                //office转换任务
                if (fileType == "office")
                {
                    InsertTask(handlerId, fileId, file.FileName, fileType, appName, new BsonDocument() {
                        {"_id",ObjectId.Empty },
                        {"Format",AttachmentOutput.pdf },
                        {"Flag","preview" } },
                        access,
                        User.Identity.Name
                    );
                }
                //zip转换任务
                else if (ext == ".zip" || ext == ".rar")
                {
                    InsertTask(handlerId, fileId, file.FileName, fileType, appName, new BsonDocument() {
                        {"_id",ObjectId.Empty },
                        {"Flag","zip" }
                    },
                    access,
                    User.Identity.Name);
                }
                else
                {
                    InsertTask(handlerId, fileId, file.FileName, fileType, appName, new BsonDocument(), access, User.Identity.Name);
                }
                //日志
                Log(fileId.ToString(), "UploadAttachment");
                response.Add(new FileResponse()
                {
                    FileId = fileId.ToString(),
                    FileName = file.FileName,
                    FileSize = file.Length,
                });
            }
            return new ResponseModel<IEnumerable<FileResponse>>(ErrorCode.success, response);
        }
    }
}
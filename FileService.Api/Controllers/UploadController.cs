using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Api.Models;
using FileService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace FileService.Api.Controllers
{
    public class UploadController : BaseController
    {
        public UploadController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult Image(UploadImgModel uploadImgModel)
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
            foreach (IFormFile file in uploadImgModel.Images)
            {
                //初始化参数
                string contentType = "", fileType = "", handlerId = "", saveFileType = "", saveFileApi = "", saveFilePath = "", saveFileName = "";
                ObjectId saveFileId = ObjectId.Empty;
                //检测文件
                if (!CheckFileAndHandler("image", file.FileName, file.InputStream, ref contentType, ref fileType, ref handlerId, ref saveFileType, ref saveFilePath, ref saveFileApi, ref saveFileId, ref saveFileName, ref response))
                {
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
                //上传文件
                if (!SaveFile(saveFileType, saveFilePath, saveFileApi, saveFileName, file, ref response)) continue;
                filesWrap.InsertImage(saveFileId,
                    ObjectId.Empty,
                    file.FileName,
                    file.InputStream.Length,
                    Request.Headers["AppName"],
                    0,
                    fileType,
                    contentType,
                    thumbnail,
                    access,
                    uploadImgModel.ExpiredDay,
                    Request.Headers["UserCode"] ?? User.Identity.Name);

                if (output.Count == 0)
                {
                    InsertTask(handlerId, saveFileId, file.FileName, fileType, Request.Headers["AppName"], new BsonDocument(), access, Request.Headers["UserCode"] ?? User.Identity.Name);
                }
                else
                {
                    foreach (ImageOutPut o in output)
                    {
                        InsertTask(handlerId, saveFileId, file.FileName, fileType, Request.Headers["AppName"], o.ToBsonDocument(), access, Request.Headers["UserCode"] ?? User.Identity.Name);
                    }
                }
                //日志
                Log(saveFileId.ToString(), "UploadImage");
                response.Add(new FileResponse()
                {
                    FileId = saveFileId.ToString(),
                    FileName = file.FileName,
                    FileSize = file.InputStream.Length,
                    SubFiles = thumbnail.Select(sel => new SubFileItem() { FileId = sel["_id"].ToString(), Flag = sel["Flag"].ToString() })
                });
                file.InputStream.Close();
                file.InputStream.Dispose();
            }
            return new ResponseModel<IEnumerable<FileResponse>>(ErrorCode.success, response);
        }
    }
}
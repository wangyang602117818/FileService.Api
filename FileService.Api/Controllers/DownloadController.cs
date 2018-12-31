using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileService.Business;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class DownloadController : BaseController
    {
        public IActionResult Get(string id, bool deleted = false)
        {
            ObjectId fileWrapId = GetObjectIdFromId(id);
            if (fileWrapId == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            BsonDocument fileWrap = deleted ? filesWrap.FindOne(fileWrapId) : filesWrap.FindOneNotDelete(fileWrapId);
            if (fileWrap == null) return File(new MemoryStream(), "application/octet-stream");
            AddDownload(fileWrapId);
            ObjectId fileId = fileWrap["FileId"].AsObjectId;
            string fileName = fileWrap["FileName"].AsString;
            string contentType = fileWrap["ContentType"].AsString;
            Response.Headers.Add("Accept-Ranges", "bytes");
            if (fileId == ObjectId.Empty)
            {
                string tempFilePath = AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + fileWrap["CreateTime"].ToUniversalTime().ToString("yyyyMMdd") + "\\" + fileName;
                FileStream fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType, fileName);
            }
            else
            {
                return GetSourceFile(fileId, contentType, fileName);
            }
        }
    }
}
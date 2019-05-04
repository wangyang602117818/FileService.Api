using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileService.Business;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace FileService.Api.Controllers
{
    public class DownloadController : BaseController
    {
        static string m3u8Template = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\template.m3u8");
        public DownloadController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult Get(string id, bool deleted = false)
        {
            ObjectId fileWrapId = GetObjectIdFromId(id);
            if (fileWrapId == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            BsonDocument fileWrap = deleted ? filesWrap.FindOne(fileWrapId) : filesWrap.FindOneNotDelete(fileWrapId);
            if (fileWrap == null) return File(new MemoryStream(), "application/octet-stream");
            if (fileWrap.Contains("ExpiredTime") && (fileWrap["CreateTime"].ToUniversalTime() >= fileWrap["ExpiredTime"].ToUniversalTime()))
            {
                return GetFileExpired();
            }
            AddDownload(fileWrapId);
            ObjectId fileId = fileWrap["FileId"].AsObjectId;
            string fileName = fileWrap["FileName"].AsString;
            string contentType = fileWrap["ContentType"].AsString;
            Response.Headers.Add("Accept-Ranges", "bytes");
            if (fileId == ObjectId.Empty)
            {
                string relativePath = GetTempFilePath(fileWrap["CreateTime"].ToUniversalTime().ToString("yyyyMMdd"), fileWrap["_id"].ToString(), fileName);
                Stream fileStream = GetCacheFile(relativePath);
                return File(fileStream, contentType, fileName);
            }
            else
            {
                return GetSourceFile(fileId, contentType, fileName);
            }
        }
        public IActionResult GetConvert(string id)
        {
            ObjectId fId = GetObjectIdFromId(id);
            if (fId == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            GridFSDownloadStream stream = mongoFileConvert.DownLoad(fId);
            ObjectId fileWrapId = stream.FileInfo.Metadata["Id"].AsObjectId;
            AddDownload(fileWrapId);
            Response.Headers.Add("Accept-Ranges", "bytes");
            return File(stream, stream.FileInfo.Metadata["ContentType"].AsString, stream.FileInfo.Filename);
        }
        [AllowAnonymous]
        public IActionResult M3u8MultiStream(string id)
        {
            if (id.StartsWith("m")) return M3u8(id.TrimStart('m'));
            if (id.StartsWith("t")) return Ts(id.TrimStart('t'));
            string m3u8File = m3u8Template;
            ObjectId newId = GetObjectIdFromId(id);
            if (newId == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            var list = m3u8.FindBySourceIdAndSort(newId).ToList();
            for (var i = 0; i < 4; i++)
            {
                var r = list.Where(s => s["Quality"].AsInt32 >= i).FirstOrDefault();
                BsonDocument item = r == null ? list[list.Count - 1] : r;
                m3u8File = m3u8File.Replace("{level-" + i + "}", "m" + item["_id"].ToString());
            }
            return File(Encoding.UTF8.GetBytes(m3u8File), "application/x-mpegURL", list[0]["FileName"].ToString());
        }
        [AllowAnonymous]
        public ActionResult M3u8(string id)
        {
            if (id.StartsWith("t")) return Ts(id.TrimStart('t'));
            ObjectId m3u8Id = GetObjectIdFromId(id);
            if (m3u8Id == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            BsonDocument document = m3u8.FindOne(m3u8Id);
            if (document == null)
            {
                return File(new MemoryStream(), "application/octet-stream");
            }
            else
            {
                document["File"] = Regex.Replace(document["File"].AsString, "(\\w+).ts", (match) =>
                {
                    return "t" + match.Groups[1].Value;
                });
                return File(Encoding.UTF8.GetBytes(document["File"].AsString), "application/x-mpegURL", document["FileName"].AsString);
            }
        }
        [AllowAnonymous]
        public ActionResult Ts(string id)
        {
            ObjectId newId = GetObjectIdFromId(id);
            if (newId == ObjectId.Empty) return File(new MemoryStream(), "application/octet-stream");
            BsonDocument document = ts.FindOne(newId);
            if (document == null)
            {
                return File(new MemoryStream(), "application/octet-stream");
            }
            else
            {
                return File(document["File"].AsByteArray, "video/vnd.dlna.mpeg-tts", document["_id"].ToString() + ".ts");
            }
        }
    }
}
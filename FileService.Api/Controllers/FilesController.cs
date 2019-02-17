using FileService.Api.Models;
using FileService.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class FilesController : BaseController
    {
        FilePreviewMobile filePreviewMobile = new FilePreviewMobile();
        FilePreview filePreview = new FilePreview();
        public FilesController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult GetFiles(int pageIndex = 1, int pageSize = 10, string from = "", string orderField = "CreateTime", string orderFieldType = "desc", string filter = "", string fileType = "", string startTime = null, string endTime = null)
        {
            long count = 0;
            DateTime.TryParse(startTime, out DateTime timeStart);
            DateTime.TryParse(endTime, out DateTime timeEnd);
            Dictionary<string, string> sorts = new Dictionary<string, string> { { orderField, orderFieldType } };
            BsonDocument eqs = new BsonDocument("Delete", false);
            if (!string.IsNullOrEmpty(fileType)) eqs.Add("FileType", fileType);
            if (!string.IsNullOrEmpty(from)) eqs.Add("From", from);
            IEnumerable<BsonDocument> result = filesWrap.GetPageList(pageIndex, pageSize, eqs, timeStart, timeEnd, sorts, filter, new List<string>() { "FileName" }, new List<string>() { }, out count, User.Identity.Name);
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, count);
        }
        public IActionResult GetExtensions()
        {
            IEnumerable<BsonDocument> result = extension.FindAll();
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result);
        }
        //[ResponseCache(CacheProfileName = "default", VaryByQueryKeys = new string[] { "id" })]
        [AllowAnonymous]
        public IActionResult GetFileIconMobile(string id)
        {
            string ext = "." + id.Split('.')[1].TrimEnd('/').ToLower();
            BsonDocument file = filePreviewMobile.FindOne(ObjectId.Parse(id.Split('.')[0]));
            return GetIcon(file, ext);
        }
        //[ResponseCache(CacheProfileName = "default", VaryByQueryKeys = new string[] { "id" })]
        [AllowAnonymous]
        public IActionResult GetFileIcon(string id)
        {
            string ext = "." + id.Split('.')[1].TrimEnd('/').ToLower();
            BsonDocument file = filePreview.FindOne(ObjectId.Parse(id.Split('.')[0]));
            return GetIcon(file, ext);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Remove(string id)
        {
            RemoveFile(id);
            Log(id, "RemoveFile");
            return new ResponseModel<string>(ErrorCode.success, "");
        }
    }
}
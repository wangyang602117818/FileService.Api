using FileService.Api.Models;
using FileService.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        FilesWrap filesWrap = new FilesWrap();
        FilePreview filePreview = new FilePreview();
        Extension extension = new Extension();
        private readonly IHostingEnvironment _hostingEnvironment;
        public FilesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult GetFiles(int pageIndex = 1, int pageSize = 10, string orderField = "CreateTime", string orderFieldType = "desc", string filter = "", string startTime = null, string endTime = null)
        {
            long count = 0;
            DateTime.TryParse(startTime, out DateTime timeStart);
            DateTime.TryParse(endTime, out DateTime timeEnd);
            Dictionary<string, string> sorts = new Dictionary<string, string> { { orderField, orderFieldType } };
            IEnumerable<BsonDocument> result = filesWrap.GetPageList(pageIndex, pageSize, new BsonDocument("Delete", false), timeStart, timeEnd, sorts, filter, new List<string>() { "FileName" }, new List<string>() { }, out count, User.Identity.Name);
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, count);
        }
        [ResponseCache(CacheProfileName = "default", VaryByQueryKeys = new string[] { "id" })]
        public ActionResult GetFileIcon(string id)
        {
            BsonDocument file = filePreview.FindOne(ObjectId.Parse(id.Split('.')[0]));
            string imagePath = _hostingEnvironment.WebRootPath + "\\images\\";
            string ext = "." + id.Split('.')[1].TrimEnd('/');
            if (file == null)
            {
                string type = extension.GetTypeByExtension(ext).ToLower();
                switch (type)
                {
                    case "text":
                    case "video":
                    case "image":
                    case "attachment":
                    case "pdf":
                        return File(System.IO.File.ReadAllBytes(imagePath + type + ".png"), "application/octet-stream");
                    case "office":
                        if (ext == ".doc" || ext == ".docx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "word.png"), "application/octet-stream");
                        if (ext == ".xls" || ext == ".xlsx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "excel.png"), "application/octet-stream");
                        if (ext == ".ppt" || ext == ".pptx")
                            return File(System.IO.File.ReadAllBytes(imagePath + "ppt.png"), "application/octet-stream");
                        if (new string[] { ".odg", ".ods", ".odp", ".odf", ".odt" }.Contains(ext))
                            return File(System.IO.File.ReadAllBytes(imagePath + "libreoffice.png"), "application/octet-stream");
                        if (new string[] { ".wps", ".dps", ".et" }.Contains(ext))
                            return File(System.IO.File.ReadAllBytes(imagePath + "wps.png"), "application/octet-stream");
                        return File(System.IO.File.ReadAllBytes(imagePath + "attachment.png"), "application/octet-stream");
                    default:
                        return File(System.IO.File.ReadAllBytes(imagePath + "attachment.png"), "application/octet-stream");
                }
            }
            return File(file["File"].AsByteArray, "application/octet-stream");
        }
    }
}
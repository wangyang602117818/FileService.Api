using FileService.Api.Models;
using FileService.Model;
using FileService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class TaskController : BaseController
    {
        Business.Task task = new Business.Task();
        public TaskController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult GetTasks(int pageIndex = 1, int pageSize = 10, string from = "", string orderField = "CreateTime", string orderFieldType = "desc", string filter = "", string startTime = null, string endTime = null)
        {
            long count = 0;
            DateTime.TryParse(startTime, out DateTime timeStart);
            DateTime.TryParse(endTime, out DateTime timeEnd);
            Dictionary<string, string> sorts = new Dictionary<string, string> { { orderField, orderFieldType } };
            BsonDocument eqs = new BsonDocument("Delete", false);
            if (!string.IsNullOrEmpty(from)) eqs.Add("From", from);
            IEnumerable<BsonDocument> result = task.GetPageList(pageIndex, pageSize, eqs, timeStart, timeEnd, sorts, filter, new List<string>() { "FileName" }, new List<string>() { }, out count, User.Identity.Name);
            foreach (BsonDocument bson in result)
            {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + bson["Folder"].ToString() + "\\" + bson["FileName"].ToString();
                if (System.IO.File.Exists(fullPath))
                {
                    bson.Add("FileExists", true);
                }
                else
                {
                    bson.Add("FileExists", false);
                }
                bson.Remove("Machine");
            }
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, count);
        }
        public ActionResult ReDo(string id, string type)
        {
            Log(id, "ReDo");
            BsonDocument document = task.FindOne(ObjectId.Parse(id));
            string handlerId = document["HandlerId"].AsString;
            int state = Convert.ToInt32(document["State"]);
            if (state == 2 || state == 4 || state == -1)
            {
                task.UpdateState(ObjectId.Parse(id), TaskStateEnum.wait, 0);
                queue.Insert(handlerId, type, "Task", ObjectId.Parse(id), false, new BsonDocument());
                converter.AddCount(handlerId, 1);
                return new ResponseModel<string>(ErrorCode.success, "");
            }
            else
            {
                return new ResponseModel<string>(ErrorCode.task_not_completed, "");
            }
        }
        public ActionResult DeleteAllCacheFile()
        {
            Log("-", "DeleteAllCacheFile");
            IEnumerable<BsonDocument> list = task.FindCacheFiles();
            int count = 0;
            foreach (BsonDocument bson in list)
            {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + AppSettings.Configuration["tempFileDir"] + bson["Folder"].ToString() + "\\" + bson["FileName"].ToString();
                if (System.IO.File.Exists(fullPath))
                {
                    count++;
                    System.IO.File.Delete(fullPath);
                }
            }
            return new ResponseModel<string>(ErrorCode.success, "", count);
        }
    }
}
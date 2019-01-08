using FileService.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        Business.Task task = new Business.Task();
        public IActionResult GetTasks(int pageIndex = 1, int pageSize = 10, string from = "", string orderField = "CreateTime", string orderFieldType = "desc", string filter = "", string startTime = null, string endTime = null)
        {
            long count = 0;
            DateTime.TryParse(startTime, out DateTime timeStart);
            DateTime.TryParse(endTime, out DateTime timeEnd);
            Dictionary<string, string> sorts = new Dictionary<string, string> { { orderField, orderFieldType } };
            BsonDocument eqs = new BsonDocument("Delete", false);
            if (!string.IsNullOrEmpty(from)) eqs.Add("From", from);
            IEnumerable<BsonDocument> result = task.GetPageList(pageIndex, pageSize, eqs, timeStart, timeEnd, sorts, filter, new List<string>() { "FileName" }, new List<string>() { }, out count, User.Identity.Name);
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, count);
        }
    }
}
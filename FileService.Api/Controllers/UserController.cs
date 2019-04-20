using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        public UserController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult GetUsers(string company,int pageIndex = 1, int pageSize = 10, string orderField = "UserName", string orderFieldType = "desc", string filter = "")
        {
            long count = 0;
            Dictionary<string, string> sorts = new Dictionary<string, string> { { orderField, orderFieldType } };
            BsonDocument eqs = new BsonDocument("Company", company);
            IEnumerable<BsonDocument> result = user.GetPageList(pageIndex, pageSize, eqs, null, null, sorts, filter, new List<string>() { "UserName" }, new List<string>() {"OpenId", "Modified" }, out count, null);
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, count);
        }
    }
}
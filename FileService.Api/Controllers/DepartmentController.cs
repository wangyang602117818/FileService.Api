using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Api.Models;
using FileService.Business;
using FileService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FileService.Api.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]
    [Authorize]
    public class DepartmentController : BaseController
    {
        public DepartmentController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult GetDepartment(string code)
        {
            List<DepartmentItem> result = department.GetDepartments(code);
            return new ResponseModel<List<DepartmentItem>>(ErrorCode.success, result, result.Count);
        }
        public IActionResult GetAllDepartment()
        {
            IEnumerable<BsonDocument> result = department.GetAllDepartment();
            return new ResponseModel<IEnumerable<BsonDocument>>(ErrorCode.success, result, result.Count());
        }
    }
}
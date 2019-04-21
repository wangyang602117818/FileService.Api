using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Api.Controllers
{
    public class UploadController : BaseController
    {
        public UploadController(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment) { }
        public IActionResult Image()
        {

            return null;
        }
    }
}
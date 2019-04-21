using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileService.Api.Models;
using FileService.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return null;
        }
    }
}
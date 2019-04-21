using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Api.Models
{
    public class UploadImgModel
    {
        [Required]
        public List<IFormFile> Images { get; set; }
        public string OutPut { get; set; }
        public string Access { get; set; }
        public int ExpiredDay { get; set; }
    }
}

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
    public class UploadVideoModel
    {
        [Required]
        public List<IFormFile> Videos { get; set; }
        public string OutPut { get; set; }
        public string Access { get; set; }
        public int ExpiredDay { get; set; }
    }
    public class UploadAttachmentModel
    {
        [Required]
        public List<IFormFile> Attachments { get; set; }
        public string Access { get; set; }
        public int ExpiredDay { get; set; }
    }
    public class UploadVideoCPModel
    {
        [Required]
        public string FileId { get; set; }
        [Required]
        public string FileBase64 { get; set; }
    }
    public class UploadVideoCPStreamModel
    {
        [Required]
        public string FileId { get; set; }
        [Required]
        public List<IFormFile> VideoCPs { get; set; }
    }
    public class ReplaceFileModel
    {
        [Required]
        public string FileId { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}

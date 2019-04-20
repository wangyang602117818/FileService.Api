using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Api.Models
{
    public class LoginForm
    {
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string PassWord { get; set; }
        [Required]
        public string AuthCode { get; set; }
        [Required]
        public string ApiType { get; set; }
        public string Code { get; set; }
    }
    public class WeChatLoginForm
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string AuthCode { get; set; }
        [Required]
        public string ApiType { get; set; }
    }
}

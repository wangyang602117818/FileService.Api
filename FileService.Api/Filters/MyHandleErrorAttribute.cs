using FileService.Api.Models;
using FileService.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileService.Api.Filters
{
    public class MyHandleErrorAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            LogHelper.ErrorLog(context.Exception);
            context.Result = new JsonResult(new ResponseItem<string>(ErrorCode.server_exception, context.Exception.Message));
        }
    }
}

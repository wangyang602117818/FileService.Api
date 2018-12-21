using FileService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace FileService.Api.Filters
{
    public class ValidateModelStateAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var item in context.ModelState)
                {
                    if (item.Value.Errors.Count > 0) errors.Add(item.Value.Errors[0].ErrorMessage);
                }
                context.Result = new JsonResult(new ResponseItem<string>(ErrorCode.params_valid_fault, string.Join(",", errors)));
            }
        }
    }
}

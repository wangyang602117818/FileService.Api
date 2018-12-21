using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Text;

namespace FileService.Api.Models
{
    public class ResponseItem<T>
    {
        public ResponseItem(ErrorCode code, T t, long count = 0)
        {
            this.code = code;
            this.message = code.ToString();
            this.result = t;
            this.count = count;
        }
        public Enum code { get; set; }
        public string message { get; set; }
        public T result { get; set; }
        public long count { get; set; }
    }
    public class ResponseModel<T> : ContentResult
    {
        public ResponseModel(ErrorCode code, T t, long count = 0)
        {
            switch (code)
            {
                case ErrorCode.success:
                    Content = "{\"code\":" + (int)code + ",\"message\":\"" + code.ToString() + "\",\"result\":" + t.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.Strict }) + ",\"count\":" + count + "}";
                    break;
                case ErrorCode.redirect:
                    Content = "{\"code\":" + (int)code + ",\"message\":\"" + code.ToString() + "\",\"result\":\"" + t.ToString() + "\",\"count\":" + count + "}";
                    break;
                default:
                    Content = "{\"code\":" + (int)code + ",\"message\":\"" + code.ToString() + "\"}";
                    break;
            }
            ContentType = "application/json";
        }
    }
}

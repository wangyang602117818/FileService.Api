using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Util
{
    public class HttpRequestHelper
    {
        public Task<string> Post(string url, string paras, Dictionary<string, string> headers)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }
            byte[] bs = Encoding.UTF8.GetBytes(paras);
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEndAsync();
                }
            }
        }
        public Task<string> Get(string url, Dictionary<string, string> headers = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "get";
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEndAsync();
        }

    }
}

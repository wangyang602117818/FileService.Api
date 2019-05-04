using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FileService.Util
{
    public static class StringExtention
    {
        public static string ToMD5(this string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] md5bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in md5bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string GetSha256(this string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] by = Sha256.ComputeHash(SHA256Data);
            return BitConverter.ToString(by).Replace("-", "").ToLower();
        }
        public static string ToStr(this Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        public static byte[] ToBytes(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
        public static string GetMachineName(this string str)
        {
            Match match = Regex.Match(str, @"\\\\(.+?)\\");
            return match.Groups[1].Value;
        }
        public static List<ObjectId> GetTsIds(this string str)
        {
            List<ObjectId> tsIds = new List<ObjectId>();
            var matchs = Regex.Matches(str, "(\\w+).ts");
            for (var i = 0; i < matchs.Count; i++)
            {
                Match match = matchs[i];
                if (match.Success) tsIds.Add(ObjectId.Parse(match.Groups[1].Value));
            }
            return tsIds;
        }
        public static string RemoveHtml(this string str)
        {
            return Regex.Replace(str, "<[^>]+>", "").Replace("&[^;]+;", "");
        }
        public static string GetFileName(this string str)
        {
            var index = str.LastIndexOf("\\");
            return str.Substring(index + 1);
        }
        public static string GetFileExt(this string str)
        {
            var index = str.LastIndexOf(".");
            return str.Substring(index);
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace FileService.Util
{
    public class ServerState
    {
        public string ServerName { get; set; }
        public string OS { get; set; }
        public string MemoryTotal { get; set; }
        public string Disk { get; set; }
        public string CacheFiles { get; set; }
        public string LogFiles { get; set; }
        
        public static string GetCacheFiles(string path)
        {
            return GetFileConvertSize(DirectorySize(new DirectoryInfo(path))) + "/" + Directory.GetDirectories(path).Length;
        }
        public static int DeleteAllCacheFiles(string saveFilePath,IEnumerable<string> notDeletePaths)
        {
            int count = 0;
            DirectoryInfo[] directoryInfos = new DirectoryInfo(saveFilePath).GetDirectories();
            for (var i = 0; i < directoryInfos.Length; i++)
            {
                FileInfo[] fileInfo = directoryInfos[i].GetFiles();
                if (fileInfo.Length == 0) Directory.Delete(directoryInfos[i].FullName);
                for (var j = 0; j < fileInfo.Length; j++)
                {
                    var fullPath = fileInfo[j].FullName;
                    if (notDeletePaths.Contains(fullPath)) continue;
                    if (System.IO.File.Exists(fullPath))
                    {
                        count++;
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return count;
        }
        public static string GetDirvesInfo()
        {
            DriveInfo[] allDirves = DriveInfo.GetDrives();
            List<string> dirvesInfo = new List<string>();
            foreach (DriveInfo item in allDirves)
            {
                if (item.IsReady)
                {
                    dirvesInfo.Add(item.Name.TrimEnd('\\') + Math.Round(item.TotalFreeSpace * 1.0 / 1024 / 1024 / 1024) + "G/" + Math.Round(item.TotalSize * 1.0 / 1024 / 1024 / 1024) + "G");
                }
            }
            return string.Join(",", dirvesInfo);
        }
        static long DirectorySize(DirectoryInfo directoryInfo)
        {
            long size = 0;
            IEnumerable<FileInfo> fis = directoryInfo.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            DirectoryInfo[] dis = directoryInfo.GetDirectories();
            foreach (DirectoryInfo d in dis)
            {
                size += DirectorySize(d);
            }
            return size;
        }
        public static string GetFileConvertSize(double size)
        {
            size = size / 1024.0;
            if (size < 1024) return Math.Round(size, 2) + "KB";
            size = size / 1024.0;
            if (size < 1024) return Math.Round(size, 2) + "MB";
            size = size / 1024.0;
            if (size < 1024) return Math.Round(size, 2) + "GB";
            size = size / 1024.0;
            return Math.Round(size, 2) + "TB";
        }
    }
    public class MongoServerState
    {
        public string ServerName { get; set; }
        public int Port { get; set; }
        public int State { get; set; }    //1:PRIMARY,2:SECONDARY,7:ARBITER,other:not reachable
        public string Version { get; set; }
        public string OS { get; set; }
        public string Memory { get; set; }
        public int Health { get; set; }  //1:ok,other:error
    }
}

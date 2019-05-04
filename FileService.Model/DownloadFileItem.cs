using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Model
{
    /// <summary>
    /// 下载文件的描述
    /// </summary>
    public class DownloadFileItem
    {
        /// <summary>
        /// 文件描述
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// ContentType类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Content长度
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// 当前视频播放的进度，只有在下载m3u8列表的时候有用
        /// </summary>
        public string UserTsTime { get; set; }
        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream { get; set; }
    }
}

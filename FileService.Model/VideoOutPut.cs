using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Model
{
    public class VideoOutPut
    {
        public ObjectId Id { get; set; }
        public VideoOutPutFormat Format { get; set; }
        public VideoOutPutQuality Quality { get; set; }
        public string Flag { get; set; }
    }
    public enum VideoOutPutFormat
    {
        M3u8 = 0,
        Mp4 = 1,
        Avi = 2,
        Wmv = 3
    }
    public enum VideoOutPutQuality
    {
        /// <summary>
        /// 原画
        /// </summary>
        Original = 0,
        /// <summary>
        /// 稍差
        /// </summary>
        Lower = 1,
        /// <summary>
        /// 画质中等
        /// </summary>
        Medium = 2,
        /// <summary>
        /// 画质差
        /// </summary>
        Bad = 3

    }
}

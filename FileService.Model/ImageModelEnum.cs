using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Model
{
    public enum ImageModelEnum
    {
        /// <summary>
        /// 缩放
        /// </summary>
        scale = 0,
        /// <summary>
        /// 剪切
        /// </summary>
        cut = 1,
        /// <summary>
        /// 按宽度
        /// </summary>
        width = 2,
        /// <summary>
        /// 按高度
        /// </summary>
        height = 3
    }
}

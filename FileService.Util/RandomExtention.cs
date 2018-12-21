using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Util
{
    public static class RandomExtention
    {
        /// <summary>
        /// 获取随机字符串，
        /// </summary>
        /// <param name="random"></param>
        /// <param name="numb">随机字符串个数</param>
        /// <returns></returns>
        public static string RandomCode(this Random random, int numb)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numb; i++)
            {
                int res = random.Next(0, 3);
                switch (res)
                {
                    case 0:
                        sb.Append(random.Next(0, 10));  //数字
                        break;
                    case 1:
                        sb.Append((char)random.Next(97, 123));  //小写字母
                        break;
                    case 2:
                        sb.Append((char)random.Next(65, 91));  //大写字母
                        break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 随机16精制字符串
        /// </summary>
        /// <param name="random"></param>
        /// <param name="numb">16精制字符串长度</param>
        /// <returns></returns>
        public static string RandomCodeHex(this Random random, int numb)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numb; i++)
            {
                int res = random.Next(0, 2);
                switch (res)
                {
                    case 0:
                        sb.Append(random.Next(0, 10));  //数字
                        break;
                    case 1:
                        sb.Append((char)random.Next(97, 103));  //小写字母
                        break;
                }
            }
            return sb.ToString();
        }
    }
}

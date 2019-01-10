using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Model
{
    public enum ConverterStateEnum
    {
        idle = 0,
        running = 1,
        offline = -1
    }
}

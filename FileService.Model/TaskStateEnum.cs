using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Model
{
    public enum TaskStateEnum
    {
        wait = 0,
        processing = 1,
        completed = 2,
        deleted = 3,
        updated = 4,
        fault = -1,
        error = -100
    }
}

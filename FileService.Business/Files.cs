using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Business
{
    public class Files : FilesBase
    {
        public Files() : base(new Data.Files()) { }
    }
    public class FilesConvert : FilesBase
    {
        public FilesConvert() : base(new Data.FilesConvert()) { }
    }
}

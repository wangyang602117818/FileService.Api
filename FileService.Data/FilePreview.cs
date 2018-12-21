using MongoDB.Bson;
using MongoDB.Driver;

namespace FileService.Data
{
    public class FilePreview : MongoBase
    {
        public FilePreview() : base("FilePreview") { }
    }
}

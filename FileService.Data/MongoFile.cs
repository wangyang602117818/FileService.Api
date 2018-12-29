using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace FileService.Data
{
    public class MongoFile : MongoFileBase
    {
        public MongoFile() : base("admin", "fs") { }
    }
    public class MongoFileConvert : MongoFileBase
    {
        public MongoFileConvert() : base("admin", "convert") { }
    }
}

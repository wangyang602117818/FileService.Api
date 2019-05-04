using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace FileService.Business
{
    public partial class Extension : ModelBase<Data.Extension>
    {
        public static List<BsonDocument> Extensions = new List<BsonDocument>();
        static Extension()
        {
            Extensions = new Data.Extension().FindAll().ToList();
        }
        public Extension() : base(new Data.Extension()) { }
        public bool CheckFileExtension(string extension, ref string contentType, ref string fileType)
        {
            BsonDocument document = mongoData.FindByExtension(extension);
            if (document == null) return true;
            contentType = document["ContentType"].AsString;
            fileType = document["Type"].AsString;
            if (document["Action"].AsString.ToLower() == "block") return false;
            return true;
        }
        public bool CheckFileExtensionVideo(string extension, ref string contentType, ref string fileType)
        {
            BsonDocument document = mongoData.FindByExtension(extension);
            if (document == null) return false;
            contentType = document["ContentType"].AsString;
            fileType = document["Type"].AsString;
            if (document["Type"].AsString.ToLower() != "video") return false;
            if (document["Action"].AsString.ToLower() == "block") return false;
            return true;
        }
        public bool CheckFileExtensionImage(string extension, ref string contentType, ref string fileType)
        {
            BsonDocument document = mongoData.FindByExtension(extension);
            if (document == null) return false;
            contentType = document["ContentType"].AsString;
            fileType = document["Type"].AsString;
            if (document["Type"].AsString.ToLower() != "image") return false;
            if (document["Action"].AsString.ToLower() == "block") return false;
            return true;
        }
        public string GetTypeByExtension(string extension)
        {
            BsonDocument document = mongoData.FindByExtension(extension);
            if (document == null) return "";
            return document["Type"].AsString;
        }
        public static string GetContentType(string extension)
        {
            BsonDocument document = Extensions.Where(ext => ext["Extension"].AsString == extension).FirstOrDefault();
            if (document == null) return "application/octet-stream";
            return document["ContentType"].AsString;
        }
        public BsonDocument GetByExtension(string extension)
        {
            return mongoData.FindByExtension(extension);
        }
        public bool DeleteExtension(string extension)
        {
            return mongoData.DeleteExtension(extension);
        }
        public IEnumerable<BsonDocument> FindByType(string type)
        {
            return mongoData.FindByType(type);
        }
    }
}

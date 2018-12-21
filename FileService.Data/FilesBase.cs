using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class FilesBase : MongoBase
    {
        public FilesBase(string collectionName) : base(collectionName) { }
        
        public BsonDocument GetFileByMD5(string md5)
        {
            var filter = FilterBuilder.Eq("md5", md5);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public IEnumerable<BsonDocument> GetByIds(IEnumerable<ObjectId> ids)
        {
            return MongoCollection.Find(FilterBuilder.In("_id",ids)).ToEnumerable();
        }
    }
}

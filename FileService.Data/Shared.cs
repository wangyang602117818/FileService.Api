using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class Shared : MongoBase
    {
        public Shared() : base("Shared") { }
        public IEnumerable<BsonDocument> GetShared(ObjectId fileId)
        {
            return MongoCollection.Find(FilterBuilder.Eq("FileId", fileId)).ToEnumerable();
        }
        public bool DisabledShared(ObjectId id, bool disable)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), Builders<BsonDocument>.Update.Set("Disabled", disable)).IsAcknowledged;
        }
        public bool DeleteShared(ObjectId fileId)
        {
            return MongoCollection.DeleteMany(FilterBuilder.Eq("FileId", fileId)).IsAcknowledged;
        }
    }
}

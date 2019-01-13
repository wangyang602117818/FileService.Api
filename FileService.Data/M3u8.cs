using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class M3u8:MongoBase
    {
        public M3u8() : base("M3u8") { }
        public IEnumerable<BsonDocument> FindBySourceId(ObjectId sourceId)
        {
            return MongoCollection.Find(FilterBuilder.Eq("SourceId", sourceId))
                .Project(Builders<BsonDocument>.Projection
                .Exclude("File")).ToEnumerable();
        }
        public IEnumerable<BsonDocument> FindBySourceIdAndSort(ObjectId sourceId)
        {
            return MongoCollection.Find(FilterBuilder.Eq("SourceId", sourceId))
               .Project(Builders<BsonDocument>.Projection
               .Exclude("File"))
               .SortBy(s => s["Quality"])
               .ToEnumerable();
        }
    }
}

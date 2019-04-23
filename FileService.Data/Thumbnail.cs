using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FileService.Data
{
    public class Thumbnail : MongoBase
    {
        public Thumbnail() : base("Thumbnail") { }
        public IEnumerable<BsonDocument> FindByIds(string from, IEnumerable<ObjectId> ids)
        {
            return MongoCollection.Find(FilterBuilder.Eq("From", from) & FilterBuilder.In("_id", ids))
                .Project(Builders<BsonDocument>.Projection
                .Exclude("File")).ToEnumerable();
        }
        public IEnumerable<BsonDocument> FindThumbnailMetadata(string from, IEnumerable<ObjectId> ids)
        {
            var proj = new BsonDocument()
                 {
                     {"Length",1 },
                     {"Width",1 },
                     {"Height",1 },
                     {"Flag",1 }
                 };
            var filter = FilterBuilder.Eq("From", from) & FilterBuilder.In("_id", ids);
            return MongoCollection.Find(filter)
                 .Project(proj)
                 .Sort(new BsonDocument("Length", 1))
                 .ToEnumerable();
        }
    }
}

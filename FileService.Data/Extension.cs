using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class Extension : MongoBase
    {
        public Extension() : base("Extension") { }
        public BsonDocument FindByExtension(string extension)
        {
            var filter = FilterBuilder.Eq("Extension", extension);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public bool DeleteExtension(string extension)
        {
            var filter = FilterBuilder.Eq("Extension", extension);
            return MongoCollection.DeleteOne(filter).IsAcknowledged;
        }
        public IEnumerable<BsonDocument> FindByType(string type)
        {
            var filter = FilterBuilder.Eq("Type", type);
            return MongoCollection.Find(filter).ToEnumerable();
        }
    }
}

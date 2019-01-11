using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class Department : MongoBase
    {
        public Department() : base("Department") { }
        public bool UpdateDepartment(ObjectId id, BsonDocument document)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), new BsonDocument("$set", document)).IsAcknowledged;
        }
        public bool ChangeOrder(ObjectId id, BsonArray departments)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), Builders<BsonDocument>.Update.Set("Department", departments)).IsAcknowledged;
        }
        public IEnumerable<BsonDocument> GetAllDepartment()
        {
            return MongoCollection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("Department")).ToEnumerable();
        }
        public BsonDocument GetByCode(string code)
        {
            return MongoCollection.Find(FilterBuilder.Eq("DepartmentCode",code)).FirstOrDefault();
        }
    }
}

using FileService.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace FileService.Data
{
    public class User : MongoBase
    {
        public User() : base("User") { }
        public bool CheckExists(string userName)
        {
            var filter = FilterBuilder.Eq("UserName", userName);
            return MongoCollection.CountDocuments(filter) > 0;
        }
        public BsonDocument GetUser(string userName)
        {
            var filter = FilterBuilder.Eq("UserName", userName);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public bool UpdateUser(string userName, BsonDocument document)
        {
            var filter = FilterBuilder.Eq("UserName", userName); 
            return MongoCollection.UpdateOne(filter, new BsonDocument() { { "$set", document } }).IsAcknowledged;
        }
        public bool DeleteUser(string userName)
        {
            var filter = FilterBuilder.Eq("UserName", userName);
            return MongoCollection.DeleteOne(filter).IsAcknowledged;
        }
        public BsonDocument Login(string userName, string password)
        {
            var filter = FilterBuilder.Eq("UserName", userName) & FilterBuilder.Eq("PassWord", password.ToMD5());
            return MongoCollection.Find(filter).FirstOrDefault();
        }
    }
}

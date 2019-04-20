using FileService.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace FileService.Data
{
    public class User : MongoBase
    {
        public User() : base("User") { }
        public BsonDocument GetUser(string userCode)
        {
            var filter = FilterBuilder.Eq("UserCode", userCode);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public BsonDocument GetUserByOpenId(string openId)
        {
            var filter = FilterBuilder.Eq("OpenId", openId);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public bool UpdateUser(string userCode, BsonDocument document)
        {
            var filter = FilterBuilder.Eq("UserCode", userCode); 
            return MongoCollection.UpdateOne(filter, new BsonDocument() { { "$set", document } }).IsAcknowledged;
        }
        public BsonDocument Login(string userCode, string password)
        {
            var filter = FilterBuilder.Eq("UserCode", userCode) & FilterBuilder.Eq("PassWord", password.ToMD5());
            return MongoCollection.Find(filter).FirstOrDefault();
        }
    }
}

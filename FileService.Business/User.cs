using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Business
{
    public class User : ModelBase<Data.User>
    {
        public User() : base(new Data.User()) { }
        public BsonDocument GetUser(string userCode)
        {
            return mongoData.GetUser(userCode);
        }
        public BsonDocument GetUserByOpenId(string openId)
        {
            return mongoData.GetUserByOpenId(openId);
        }
        public bool UpdateUser(string userCode, BsonDocument document)
        {
            return mongoData.UpdateUser(userCode, document);
        }
        public BsonDocument Login(string userCode, string passWord)
        {
            return mongoData.Login(userCode, passWord);
        }
    }
}

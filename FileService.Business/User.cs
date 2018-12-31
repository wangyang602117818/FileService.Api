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
        public bool CheckExists(string userName)
        {
            return mongoData.CheckExists(userName);
        }
        public BsonDocument GetUser(string userName)
        {
            return mongoData.GetUser(userName);
        }
        public BsonDocument GetUserByOpenId(string openId)
        {
            return mongoData.GetUserByOpenId(openId);
        }
        public bool UpdateUser(string userName, BsonDocument document)
        {
            return mongoData.UpdateUser(userName, document);
        }
        public bool DeleteUser(string userName)
        {
            return mongoData.DeleteUser(userName);
        }
        public BsonDocument Login(string userName, string passWord)
        {
            return mongoData.Login(userName, passWord);
        }
    }
}

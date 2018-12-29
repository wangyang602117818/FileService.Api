using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Business
{
    public class Application : ModelBase<Data.Application>
    {
        public Application() : base(new Data.Application()) { }
        public BsonDocument FindByAuthCode(string authCode)
        {
            return mongoData.FindByAuthCode(authCode);
        }
        public BsonDocument FindByAppName(string appName)
        {
            return mongoData.FindByAppName(appName);
        }
        public void AddApplication(BsonDocument application)
        {
            application.Add("CreateTime", DateTime.Now);
            mongoData.Insert(application);
        }
    }
}

using MongoDB.Bson;
using System;

namespace FileService.Business
{
    public class Log : ModelBase<Data.Log>
    {
        public Log() : base(new Data.Log()) { }
        public void Insert(string from, string fileId, string content, string userName, string apiType, string userIp, string userAgent)
        {
            BsonDocument document = new BsonDocument()
            {
                {"From",from??"" },
                {"FileId",fileId },
                {"Content",content },
                {"UserName",userName },
                {"ApiType",apiType },
                {"UserIp",userIp },
                {"UserAgent",userAgent??"" },
                {"CreateTime",DateTime.Now }
            };
            mongoData.Insert(document);
        }
    }
}

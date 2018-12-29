using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Business
{
    public class Download : ModelBase<Data.Download>
    {
        public Download() : base(new Data.Download()) { }
        public void AddDownload(ObjectId fileId, string from, string user, string ip, string agent)
        {
            BsonDocument bson = new BsonDocument()
            {
                {"FileId",fileId },
                {"From",from },
                {"User",user },
                {"Ip",ip??"" },
                {"Agent",agent??"" },
                {"CreateTime",DateTime.Now },
            };
            mongoData.Insert(bson);
        }
        public bool AddedInOneMinute(string from, ObjectId fileId, string user)
        {
            DateTime gtDate = DateTime.Now.AddMinutes(-1);
            return mongoData.DocumentMinute(from, fileId, user, gtDate) == null ? false : true;
        }
        public IEnumerable<BsonDocument> GetCountByRecentMonth(DateTime startDateTime)
        {
            return mongoData.GetCountByRecentMonth(startDateTime);
        }
        public IEnumerable<BsonDocument> GetDownloadsByAppName(DateTime startDateTime)
        {
            return mongoData.GetDownloadsByAppName(startDateTime);
        }
    }
}

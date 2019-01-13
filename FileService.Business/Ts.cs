using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Business
{
    public class Ts : ModelBase<Data.Ts>
    {
        public Ts() : base(new Data.Ts()) { }
        public void Insert(ObjectId id, string from, long length, string md5, IEnumerable<ObjectId> sourceIds, byte[] file)
        {
            BsonDocument document = new BsonDocument()
            {
                {"_id",id },
                {"From",from },
                {"Md5",md5 },
                {"Length",length},
                {"SourceIds",new BsonArray(sourceIds)},
                {"File",file },
                {"CreateTime",DateTime.Now }
            };
            mongoData.Insert(document);
        }
        public bool AddSourceId(string from, ObjectId id, ObjectId sourceId)
        {
            return mongoData.AddSourceId(from, id, sourceId);
        }
        public BsonDocument GetIdByMd5(string from, string md5)
        {
            return mongoData.GetIdByMd5(from, md5);
        }
        public bool DeleteByIds(string from, ObjectId sourceId, IEnumerable<ObjectId> ids)
        {
            if (mongoData.DeleteSourceId(from, sourceId, ids))
            {
                return mongoData.DeleteByIds(from, sourceId, ids);
            }
            else
            {
                return false;
            }

        }
    }
}

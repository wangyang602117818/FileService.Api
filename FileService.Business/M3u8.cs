using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Business
{
    public class M3u8 : ModelBase<Data.M3u8>
    {
        public M3u8() : base(new Data.M3u8()) { }
        public void Replace(ObjectId id, string from, ObjectId sourceId, string fileName, string file, int quality, int duration, int tsCount, string flag)
        {
            BsonDocument document = new BsonDocument()
            {
                {"_id", id},
                {"From", from},
                {"SourceId", sourceId},
                {"FileName", fileName},
                {"Quality", quality},
                {"Duration", duration},
                {"TsCount", tsCount},
                {"File", file}, {"Flag",flag },
                {"CreateTime",DateTime.Now }
            };
            mongoData.Replace(document);
        }
        public IEnumerable<BsonDocument> FindBySourceId(ObjectId sourceId)
        {
            return mongoData.FindBySourceId(sourceId);
        }
        public IEnumerable<BsonDocument> FindBySourceIdAndSort(ObjectId sourceId)
        {
            return mongoData.FindBySourceIdAndSort(sourceId);
        }
    }
}

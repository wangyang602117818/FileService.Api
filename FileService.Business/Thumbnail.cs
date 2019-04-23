using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Business
{
    public partial class Thumbnail : ModelBase<Data.Thumbnail>
    {
        public Thumbnail() : base(new Data.Thumbnail()) { }
        public bool Replace(ObjectId id, string from, ObjectId sourceId, long length, int width, int height, string fileName, string flag, byte[] file, DateTime expiredTime)
        {
            BsonDocument document = new BsonDocument()
            {
                {"_id",id },
                {"From",from },
                {"SourceId",sourceId },
                {"Length",length },
                {"Width",width },
                {"Height",height },
                {"FileName",fileName },
                {"File",file },
                {"Flag",flag },
                {"CreateTime",DateTime.Now },
                {"ExpiredTime",expiredTime },
            };
            return mongoData.Replace(document);
        }
        public IEnumerable<BsonDocument> FindByIds(string from, IEnumerable<ObjectId> ids)
        {
            return mongoData.FindByIds(from, ids);
        }
        public IEnumerable<BsonDocument> FindThumbnailMetadata(string from, IEnumerable<ObjectId> ids)
        {
            return mongoData.FindThumbnailMetadata(from, ids);
        }
    }
}

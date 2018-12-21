using MongoDB.Bson;
using System.Collections.Generic;

namespace FileService.Business
{
    public class FilesBase : ModelBase<Data.FilesBase>
    {
        public FilesBase(Data.FilesBase filesBase) : base(filesBase) { }

        public BsonDocument GetFileByMd5(string md5)
        {
            return mongoData.GetFileByMD5(md5);
        }
        public IEnumerable<BsonDocument> GetByIds(IEnumerable<ObjectId> ids)
        {
            return mongoData.GetByIds(ids);
        }
    }
}

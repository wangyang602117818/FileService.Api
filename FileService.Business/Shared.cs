using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Business
{
    public class Shared : ModelBase<Data.Shared>
    {
        public Shared() : base(new Data.Shared()) { }
        public IEnumerable<BsonDocument> GetShared(ObjectId fileId)
        {
            return mongoData.GetShared(fileId);
        }
        public bool DisabledShared(ObjectId id, bool disable)
        {
            return mongoData.DisabledShared(id, disable);
        }
        public bool DeleteShared(ObjectId fileId)
        {
            return mongoData.DeleteShared(fileId);
        }
    }
}

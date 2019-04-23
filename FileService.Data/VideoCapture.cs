using MongoDB.Bson;
using System.Collections.Generic;

namespace FileService.Data
{
    public class VideoCapture : MongoBase
    {
        public VideoCapture() : base("VideoCapture") { }
        public bool DeleteByIds(string from, IEnumerable<ObjectId> ids)
        {
            var filter = FilterBuilder.Eq("From", from) & FilterBuilder.In("_id", ids);
            return MongoCollection.DeleteMany(filter).IsAcknowledged;
        }
    }
}

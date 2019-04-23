using MongoDB.Bson;
using System.Collections.Generic;

namespace FileService.Business
{
    public class VideoCapture : ModelBase<Data.VideoCapture>
    {
        public VideoCapture() : base(new Data.VideoCapture()) { }
        public bool DeleteByIds(string from, IEnumerable<ObjectId> ids)
        {
            return mongoData.DeleteByIds(from, ids);
        }
    }
}

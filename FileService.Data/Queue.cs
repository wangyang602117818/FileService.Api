using FileService.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class Queue : MongoBase
    {
        public Queue() : base("queue")
        {
        }
        public async Task<IAsyncCursor<BsonDocument>> MonitorMessage(string handlerId, string collection = "Task")
        {
            var filter = FilterBuilder.Eq("handlerId", handlerId) & FilterBuilder.Eq("collectionName", collection) & FilterBuilder.Eq("processed", false);
            return await MongoCollection.FindAsync(filter, new FindOptions<BsonDocument> { CursorType = CursorType.TailableAwait });
        }
        public bool MessageProcessed(ObjectId id)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), Builders<BsonDocument>.Update.Set("processed", true)).IsAcknowledged;
        }
    }
}

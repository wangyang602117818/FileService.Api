using FileService.Data;
using FileService.Util;
using MongoDB.Bson;
using System;
using System.Collections.Concurrent;

namespace FileService.Business
{
    public partial class Queue : ModelBase<Data.Queue>
    {
        public static BlockingCollection<FileItem> itemlist = null;
        public static BlockingCollection<int> tasklist = null;
        static Queue()
        {
            int taskCount = Convert.ToInt32(AppSettings.Configuration["taskCount"]);
            if (taskCount > 0)
            {
                itemlist = new BlockingCollection<FileItem>(taskCount);
                tasklist = new BlockingCollection<int>(taskCount);
            }
            for (var i = 0; i < taskCount; i++) tasklist.Add(1);
        }
        public Queue() : base(new Data.Queue()) { }
        public void Insert(string handlerId, string type, string collectionName, ObjectId collectionId, bool processed, BsonDocument data)
        {
            BsonDocument queue = new BsonDocument()
            {
                {"handlerId",handlerId },
                {"type",type },
                {"collectionName",collectionName },
                {"collectionId",collectionId },
                {"op","insert" },
                {"data",data },
                {"processed",processed },
                {"createTime",DateTime.Now },
            };
            mongoData.InsertOneAsync(queue);
        }
        public bool MessageProcessed(ObjectId id)
        {
            return mongoData.MessageProcessed(id);
        }
        public async void MonitorMessage(string handlerId)
        {
            var cursor = await mongoData.MonitorMessage(handlerId);
            while (await cursor.MoveNextAsync())
            {
                var batch = cursor.Current;
                foreach (var doc in batch)
                {
                    ObjectId queueId = doc["_id"].AsObjectId;
                    string collectionName = doc["collectionName"].AsString;
                    ObjectId collectionId = doc["collectionId"].AsObjectId;
                    BsonDocument taskItem = new MongoBase(collectionName).FindOne(collectionId);
                    if (taskItem == null) continue;
                    itemlist.Add(new FileItem()
                    {
                        QueueId = queueId,
                        Message = taskItem,
                    });
                }
            }
        }


    }
    public class FileItem
    {
        public ObjectId QueueId { get; set; }
        public BsonDocument Message { get; set; }
    }
}

using FileService.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileService.Data
{
    public class Converter : MongoBase
    {
        public Converter() : base("Converter") { }
        public IEnumerable<BsonDocument> FindAllExistsMachine(string machine)
        {
            var filter = FilterBuilder.Eq("MonitorStateList.Machine", machine);
            return MongoCollection.Find(filter).ToEnumerable();
        }
        public BsonDocument FindByHandler(string handlerId)
        {
            return MongoCollection.Find(FilterBuilder.Eq("HandlerId", handlerId)).FirstOrDefault();
        }
        public bool Running(string handlerId)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), Builders<BsonDocument>.Update.Set("State", ConverterStateEnum.running).Set("StartTime", DateTime.Now)).IsAcknowledged;
        }
        public bool Offline(string handlerId)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), Builders<BsonDocument>.Update.Set("State", ConverterStateEnum.offline).Set("EndTime", DateTime.Now)).IsAcknowledged;
        }
        public bool UpdateByHanderId(string handlerId, BsonDocument document)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), new BsonDocument("$set", document), new UpdateOptions() { IsUpsert = true }).IsAcknowledged;
        }
        public bool UpdateStatesByHanderId(string handlerId, BsonArray array)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), Builders<BsonDocument>.Update.Set("MonitorStateList", array)).IsAcknowledged;
        }
        public bool AddCount(string handlerId, int total)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), Builders<BsonDocument>.Update.Inc("Total", total)).IsAcknowledged;
        }
        public bool Empty(string handlerId)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("HandlerId", handlerId), Builders<BsonDocument>.Update.Set("Total", 0)).IsAcknowledged;
        }
    }
}

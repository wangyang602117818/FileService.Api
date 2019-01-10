using FileService.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileService.Business
{
    public partial class Converter : ModelBase<Data.Converter>
    {
        public Converter() : base(new Data.Converter()) { }
        public bool UpdateByHanderId()
        {
            BsonDocument handler = mongoData.FindByHandler(HandlerId);
            if (handler != null)
            {
                return mongoData.Running(HandlerId);
            }
            else
            {
                return mongoData.UpdateByHanderId(HandlerId, this.ToBsonDocument());
            }
        }
        public bool UpdateStatesByHanderId(string handlerId, BsonArray array)
        {
            return mongoData.UpdateStatesByHanderId(handlerId, array);
        }
        public bool AddCount(string handlerId, int total)
        {
            return mongoData.AddCount(handlerId, total);
        }
        public bool Empty(string handlerId)
        {
            return mongoData.Empty(handlerId);
        }
        public int TaskCount(string handlerId)
        {
            BsonDocument handler = mongoData.FindByHandler(handlerId);
            return handler["Total"].AsInt32;
        }
        public bool Running()
        {
            return mongoData.Running(HandlerId);
        }
        public bool Offline()
        {
            return mongoData.Offline(HandlerId);
        }
        public string GetHandlerId()
        {
            string machine = Environment.MachineName;
            IEnumerable<BsonDocument> all = mongoData.FindAllExistsMachine(machine).OrderBy(o => o["Total"]);
            if (all.Count() == 0) return "unknown";
            if (all.Count() == 1) return all.First()["HandlerId"].AsString;
            IEnumerable<BsonDocument> run = all.Where(sel => sel["State"].AsInt32 >= 0).OrderBy(o => o["State"]).OrderBy(o => o["State"]);
            if (run.Count() == 0)
            {
                return all.First()["HandlerId"].AsString;
            }
            else
            {
                return run.First()["HandlerId"].AsString;
            }
        }
    }
    public partial class Converter
    {
        public string HandlerId { get; set; }
        public string MachineName { get; set; }
        public int Total { get; set; }
        public ConverterStateEnum State { get; set; }
        public List<MonitorState> MonitorStateList { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class MonitorState
    {
        public string Machine { get; set; }
        public string Message { get; set; }
        public DateTime MonitorTime { get; set; }
    }
}

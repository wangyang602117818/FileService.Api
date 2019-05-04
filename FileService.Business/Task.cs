using FileService.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace FileService.Business
{
    public partial class Task : ModelBase<Data.Task>
    {
        public Task() : base(new Data.Task()) { }
        public void Insert(ObjectId id, ObjectId fileId, string folder, string fileName, string type, string from, BsonDocument output, BsonArray access, string owner, string handlerId, int processCount, TaskStateEnum state, int priority)
        {
            BsonDocument task = new BsonDocument()
            {
                {"_id",id },
                {"FileId",fileId },
                {"Machine",Environment.MachineName },
                {"Folder",folder },
                {"FileName",fileName },
                {"Type",type },
                {"From",from },
                {"Output",output },
                {"Access",access },
                {"Owner",owner },
                {"Delete",false },
                {"DeleteTime",BsonNull.Value },
                {"HandlerId",handlerId },
                {"ProcessCount",processCount },
                {"State",state },
                {"StateDesc",state.ToString() },
                {"Percent",0 },
                {"Priority",priority },
                {"CreateTime",DateTime.Now },
                {"CompletedTime",BsonNull.Value }
            };
            mongoData.InsertOneAsync(task);
        }
        public bool UpdateState(ObjectId id, TaskStateEnum state, int percent)
        {
            return mongoData.UpdateState(id, state, percent);
        }
        public bool UpdatePercent(ObjectId id, int percent)
        {
            return mongoData.UpdatePercent(id, percent);
        }
        public bool UpdateOutPutId(ObjectId id, ObjectId outputId)
        {
            return mongoData.UpdateOutPutId(id, outputId);
        }
        public bool Compeleted(ObjectId id)
        {
            return mongoData.Compeleted(id);
        }
        public bool Error(ObjectId id)
        {
            return mongoData.Error(id);
        }
        public bool Fault(ObjectId id)
        {
            return mongoData.Fault(id);
        }
        public bool DeleteByOutputId(ObjectId id)
        {
            return mongoData.DeleteByOutputId(id);
        }
        public bool RemoveByFileId(ObjectId fileId)
        {
            return mongoData.RemoveByFileId(fileId);
        }
        public bool RestoreByFileId(ObjectId fileId)
        {
            return mongoData.RestoreByFileId(fileId);
        }
        public bool RestoreByFileIds(IEnumerable<ObjectId> fileIds)
        {
            return mongoData.RestoreByFileIds(fileIds);
        }
        public bool DeleteByFileId(ObjectId fileId)
        {
            return mongoData.DeleteByFileId(fileId);
        }
        public IEnumerable<BsonDocument> GetCountByRecentMonth(DateTime startDateTime)
        {
            return mongoData.GetCountByRecentMonth(startDateTime);
        }
        public bool UpdateAccess(IEnumerable<ObjectId> fileIds, BsonArray array)
        {
            return mongoData.UpdateAccess(fileIds, array);
        }
        public IEnumerable<BsonDocument> GetFilesByAppName()
        {
            return mongoData.GetFilesByAppName();
        }
        public IEnumerable<BsonDocument> FindCacheFiles(string handlerId)
        {
            return mongoData.FindCacheFiles(handlerId);
        }
        public IEnumerable<BsonDocument> FindCacheFiles()
        {
            return mongoData.FindCacheFiles();
        }
        public IEnumerable<BsonDocument> FindCacheFiles(ObjectId fileId)
        {
            return mongoData.FindCacheFiles(fileId);
        }
        public IEnumerable<BsonDocument> GetCountByAppName(DateTime startDateTime)
        {
            return mongoData.GetCountByAppName(startDateTime);
        }
        
    }
}

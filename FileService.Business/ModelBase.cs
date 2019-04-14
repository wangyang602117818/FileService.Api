using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FileService.Business
{
    public class ModelBase<T> where T : Data.MongoBase
    {
        [BsonIgnore]
        protected T mongoData;
        public ModelBase(T mongoData)
        {
            this.mongoData = mongoData;
        }
        public BsonDocument ServerStatus()
        {
            return mongoData.ServerStatus();
        }
        public BsonDocument DbStats()
        {
            return mongoData.DbStats();
        }
        public BsonDocument HostInfo()
        {
            return mongoData.HostInfo();
        }
        public void Insert(BsonDocument document)
        {
            mongoData.Insert(document);
        }
        public void InsertOneAsync(BsonDocument document)
        {
            mongoData.InsertOneAsync(document);
        }
        public void InsertManyAsync(IEnumerable<BsonDocument> documents)
        {
            mongoData.InsertManyAsync(documents);
        }
        public bool Update(ObjectId id, BsonDocument document)
        {
            return mongoData.Update(id, document);
        }
        public bool DeleteOne(ObjectId id)
        {
            return mongoData.DeleteOne(id);
        }
        public bool DeleteMany(IEnumerable<ObjectId> ids)
        {
            return mongoData.DeleteMany(ids);
        }
        public IEnumerable<BsonDocument> Find(BsonDocument document)
        {
            return mongoData.Find(document);
        }
        public IEnumerable<BsonDocument> FindByIds(IEnumerable<ObjectId> ids)
        {
            return mongoData.FindByIds(ids);
        }
        public BsonDocument FindOne(ObjectId id)
        {
            return mongoData.FindOne(id);
        }
        public BsonDocument FindOneNotDelete(ObjectId id)
        {
            return mongoData.FindOneNotDelete(id);
        }
        public IEnumerable<BsonDocument> FindAll()
        {
            return mongoData.FindAll();
        }
        public bool Replace(BsonDocument document)
        {
            return mongoData.Replace(document);
        }
        public IEnumerable<BsonDocument> GetPageList(int pageIndex, int pageSize, BsonDocument eqs, DateTime? start, DateTime? end, Dictionary<string, string> sorts, string filter, IEnumerable<string> fields, IEnumerable<string> excludeFields, out long count, string userName = null)
        {
            return mongoData.GetPageList(pageIndex, pageSize, eqs, start, end, sorts, filter, fields, excludeFields, out count, userName);
        }
        public long Count()
        {
            return mongoData.Count();
        }
        /// <summary>
        /// 将doc中的fields字段的值中word字样添加<span class=\"search_word\"></span>包围
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fields"></param>
        /// <param name="word"></param>
        public static void StringRepace(BsonDocument doc, IEnumerable<string> fields, string word)
        {
            foreach (string str in fields)
            {
                if (doc.Contains(str)) doc[str] = Regex.Replace(doc[str].ToString(), word, MatcFunc, RegexOptions.IgnoreCase);
            }
        }
        public static string MatcFunc(Match match)
        {
            return "<span class=\"search_word\">" + match.Value + "</span>";
        }
    }
}

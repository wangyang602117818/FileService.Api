using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileService.Data
{
    public class MongoBase
    {
        protected IMongoDatabase MongoDatabase;
        protected IMongoCollection<BsonDocument> MongoCollection;
        protected FilterDefinitionBuilder<BsonDocument> FilterBuilder = Builders<BsonDocument>.Filter;
        public MongoBase(string collectionName)
        {
            MongoDatabase = MongoDataSource.MongoClient.GetDatabase(MongoDataSource.databaseName);
            MongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
        public BsonDocument ServerStatus()
        {
            return MongoDatabase.RunCommand<BsonDocument>(new BsonDocument("serverStatus", 1));
        }
        public BsonDocument DbStats()
        {
            return MongoDatabase.RunCommand<BsonDocument>(new BsonDocument("dbStats", 1));
        }
        public BsonDocument HostInfo()
        {
            return MongoDatabase.RunCommand<BsonDocument>(new BsonDocument("hostInfo", 1));
        }
        public void Insert(BsonDocument document)
        {
            MongoCollection.InsertOne(document);
        }
        public void InsertOneAsync(BsonDocument document)
        {
            MongoCollection.InsertOneAsync(document);
        }
        public void InsertManyAsync(IEnumerable<BsonDocument> documents)
        {
            MongoCollection.InsertManyAsync(documents);
        }
        public bool Update(ObjectId id, BsonDocument document)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), new BsonDocument("$set", document)).IsAcknowledged;
        }
        public bool DeleteOne(ObjectId id)
        {
            return MongoCollection.DeleteOne(new BsonDocument("_id", id)).IsAcknowledged;
        }
        public bool DeleteMany(IEnumerable<ObjectId> ids)
        {
            return MongoCollection.DeleteMany(FilterBuilder.In("_id", ids)).IsAcknowledged;
        }
        public IEnumerable<BsonDocument> Find(BsonDocument document)
        {
            return MongoCollection.Find(document).ToEnumerable();
        }
        public IEnumerable<BsonDocument> FindNotDelete(BsonDocument document)
        {
            document.Add("Delete", false);
            return MongoCollection.Find(document).ToEnumerable();
        }
        public BsonDocument FindOne(ObjectId id)
        {
            return MongoCollection.Find(new BsonDocument("_id", id)).FirstOrDefault();
        }
        public IEnumerable<BsonDocument> FindByIds(IEnumerable<ObjectId> ids)
        {
            return MongoCollection.Find(FilterBuilder.In("_id", ids)).ToEnumerable();
        }
        public BsonDocument FindOneNotDelete(ObjectId id)
        {
            var filter = FilterBuilder.Eq("_id", id) & FilterBuilder.Eq("Delete", false);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public IEnumerable<BsonDocument> FindAll()
        {
            return MongoCollection.Find(new BsonDocument()).ToEnumerable();
        }
        public virtual long Count()
        {
            return MongoCollection.EstimatedDocumentCount();
        }
        public bool Replace(BsonDocument document)
        {
            return MongoCollection.ReplaceOne(new BsonDocument("_id", document["_id"].AsObjectId), document, new UpdateOptions() { IsUpsert = true }).IsAcknowledged;
        }
        public FilterDefinition<BsonDocument> GetAccessFilterBase(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            string companyCode = "";
            IEnumerable<string> departments = new string[] { };
            if (userName != "local")
            {
                BsonDocument user = new User().GetUser(userName);
                companyCode = user["Company"].AsString;
                departments = user["Department"].AsBsonArray.Select(s => s.ToString());
            }
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            //这种全部可见
            list.Add(FilterBuilder.Size("Access", 0));
            //Owner是我就可见
            list.Add(FilterBuilder.Eq("Owner", userName));
            list.Add(FilterBuilder.Eq("Owner", ""));
            //属于companyCode公司的人可见
            list.Add(FilterBuilder.ElemMatch<BsonDocument>("Access", new BsonDocument() {
                { "Company",companyCode },
                { "AccessCodes",new BsonArray()},
                { "AccessUsers",new BsonArray()},
            }));
            //属于companyCode公司 并且 部门相匹配的
            list.Add(FilterBuilder.ElemMatch<BsonDocument>("Access", new BsonDocument() {
                {"Company",companyCode },
                {"AccessCodes",new BsonDocument("$in",new BsonArray(departments))},
            }));
            //属于companyCode公司 并且 用户相匹配的
            list.Add(FilterBuilder.ElemMatch<BsonDocument>("Access", new BsonDocument() {
                {"Company",companyCode },
                {"AccessUsers",userName},
            }));
            return FilterBuilder.Or(list);
        }
        public virtual FilterDefinition<BsonDocument> GetAccessFilter(string userName)
        {
            return null;
        }
        public virtual FilterDefinition<BsonDocument> GetAndFilter()
        {
            return null;
        }
        public FilterDefinition<BsonDocument> GetPageFilters(BsonDocument eqs, DateTime start, DateTime end, IEnumerable<string> fields, string filter, string userName)
        {
            FilterDefinition<BsonDocument> filterBuilder = null;
            if (!string.IsNullOrEmpty(filter))
            {
                List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
                foreach (string field in fields)
                {
                    if (field == "_id" || field == "FileId")
                    {
                        if (ObjectId.TryParse(filter, out ObjectId objectId))
                        {
                            list.Add(FilterBuilder.Eq(field, objectId));
                        }
                    }
                    else
                    {
                        list.Add(FilterBuilder.Regex(field, new Regex("^.*" + filter + ".*$", RegexOptions.IgnoreCase)));
                    }
                }
                filterBuilder = FilterBuilder.Or(list);
            }
            else
            {
                filterBuilder = new BsonDocument();
            }
            List<FilterDefinition<BsonDocument>> result = new List<FilterDefinition<BsonDocument>>();
            result.Add(filterBuilder);
            if (eqs != null) result.Add(eqs);
            if (start != DateTime.MinValue) result.Add(FilterBuilder.Gte("CreateTime", start.AddHours(0).AddMinutes(0).AddSeconds(0)));
            if (end != DateTime.MinValue) result.Add(FilterBuilder.Lte("CreateTime", end.AddHours(23).AddMinutes(59).AddSeconds(59)));
            var accessFilter = GetAccessFilter(userName);
            if (accessFilter != null) result.Add(accessFilter);
            var andFilter = GetAndFilter();
            if (andFilter != null) result.Add(andFilter);
            return FilterBuilder.And(result);
        }
        public IEnumerable<BsonDocument> GetPageList(int pageIndex, int pageSize, BsonDocument eqs, DateTime start, DateTime end, Dictionary<string, string> sorts, string filter, IEnumerable<string> fields, IEnumerable<string> excludeFields, out long count, string userName)
        {
            FilterDefinition<BsonDocument> filterBuilder = GetPageFilters(eqs, start, end, fields, filter, userName);
            count = MongoCollection.CountDocuments(filterBuilder);
            var exclude = Builders<BsonDocument>.Projection.Exclude("PassWord");
            foreach (string ex in excludeFields)
            {
                exclude = exclude.Exclude(ex);
            }
            var find = MongoCollection.Find(filterBuilder).Project(exclude);
            if (sorts != null)
            {
                foreach (var item in sorts)
                {
                    if (item.Value == "asc") find = find.SortBy(sort => sort[item.Key]);
                    if (item.Value == "desc") find = find.SortByDescending(sort => sort[item.Key]);
                }
            }
            return find.Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToEnumerable();
        }
    }
}

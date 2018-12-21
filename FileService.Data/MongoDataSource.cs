using FileService.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace FileService.Data
{
    public class MongoDataSource
    {
        public static MongoClient MongoClient;   //mongo数据库操作
        public static string UserName;
        public static string Password;
        public static int Port;
        public static string conntionString = AppSettings.Configuration["mongodb:conntionstring"];
        public static string databaseName = AppSettings.Configuration["mongodb:database"];
        static MongoDataSource()
        {
            MongoClient = new MongoClient(conntionString);
            IMongoDatabase database = MongoClient.GetDatabase(databaseName);
            MongoDBInit.Init(database);
        }
    }
}

using FileService.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace FileService.Data
{
    public static class MongoDBInit
    {
        public static void Init(IMongoDatabase database)
        {
            var databases = database.ListCollectionNames().ToList();
            if (!databases.Contains("queue"))
            {
                database.CreateCollection("queue", new CreateCollectionOptions() { Capped = true, MaxSize = 1048576 * 100 });
                database.GetCollection<BsonDocument>("queue").InsertOne(new BsonDocument()
                {
                    {"handlerId","system" },
                    {"type","system" },
                    {"collectionName","system" },
                    {"collectionId",ObjectId.GenerateNewId() },
                    {"op","init" },
                    {"data",new BsonDocument() },
                    {"processed",true },
                    {"createTime",DateTime.Now }
                });
            }
            if (!databases.Contains("Extension"))
            {
                database.CreateCollection("Extension");
                List<BsonDocument> list = new List<BsonDocument>()
                {
                    new BsonDocument(){{"Extension",".jpg"},{"Type","image"}, { "ContentType","image/jpeg"}, {"Action","allow"},{"Description","image"},{"CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".png"},{"Type","image"}, { "ContentType", "image/png" }, {"Action","allow"},{"Description","image" }, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension",".gif"},{"Type","image"}, { "ContentType", "image/gif" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".bmp"},{"Type","image"}, { "ContentType", "application/x-bmp" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".jpeg"},{"Type","image"}, { "ContentType", "image/jpeg" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".pic"},{"Type","image"}, { "ContentType", "application/x-pic" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".ico"},{"Type","image"}, { "ContentType", "image/x-icon" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".tif"},{"Type","image"}, { "ContentType", "image/tiff" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },
                    new BsonDocument(){{"Extension", ".svg"},{"Type","image"}, { "ContentType", "image/svg+xml" }, {"Action","allow"},{"Description","image"}, { "CreateTime", DateTime.Now } },

                    new BsonDocument(){{"Extension", ".mp3"},{"Type","audio"}, { "ContentType", "audio/mpeg" }, {"Action","allow"},{"Description", "audio" }, { "CreateTime", DateTime.Now } },

                    new BsonDocument(){{"Extension",".mp4"},{"Type","video"}, { "ContentType", "video/mp4" }, {"Action","allow"},{"Description","video"}, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".avi"},{"Type","video"}, { "ContentType", "video/avi" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".wmv"},{"Type","video"}, { "ContentType", "video/x-ms-wmv" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".mov"},{"Type","video"}, { "ContentType", "video/quicktime" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".mkv"},{"Type","video"}, { "ContentType", "video/mkv" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".flv" },{"Type","video"}, { "ContentType", "video/x-flv" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".rm"},{"Type","video"}, { "ContentType", "application/vnd.rn-realmedia" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".rmvb"},{"Type","video"}, { "ContentType", "application/vnd.rn-realmedia-vbr" }, {"Action","allow"},{"Description", "video" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension",".m3u8" },{"Type","video"}, { "ContentType", "application/x-mpegURL" }, {"Action","allow"},{"Description", "HTTP Live Streaming" }, { "CreateTime", DateTime.Now }},

                    new BsonDocument(){{"Extension",".doc" },{"Type","office"}, { "ContentType", "application/msword" }, {"Action","allow"},{"Description", "microsoft office" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".docx" },{"Type", "office" }, { "ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }, { "Action","allow"}, { "Description", "microsoft office" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".xls" },{"Type", "office" }, { "ContentType", "application/vnd.ms-excel" }, { "Action","allow"}, { "Description", "microsoft office" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".xlsx" },{"Type", "office" }, { "ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }, { "Action","allow"}, { "Description", "microsoft office" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".ppt" },{"Type", "office" }, { "ContentType", "application/vnd.ms-powerpoint" }, { "Action","allow"}, { "Description", "microsoft office" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".pptx" },{"Type", "office" }, { "ContentType", "application/vnd.openxmlformats-officedocument.presentationml.presentation" }, { "Action","allow"}, { "Description", "microsoft office" }, { "CreateTime", DateTime.Now }},

                    new BsonDocument(){{"Extension", ".txt" },{"Type", "text" },{ "ContentType", "text/plain" }, { "Action","allow"}, { "Description", "text" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".js" },{"Type", "text" }, { "ContentType", "application/javascript" }, { "Action","allow"}, { "Description", "code" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".htm" },{"Type", "text" }, { "ContentType", "text/html" }, { "Action","allow"}, { "Description", "code" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".html" },{"Type", "text" }, { "ContentType", "text/html" }, { "Action","allow"}, { "Description", "code" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".css" },{"Type", "text" }, { "ContentType", "text/css" }, { "Action","allow"}, { "Description", "code" }, { "CreateTime", DateTime.Now }},

                    new BsonDocument(){{"Extension", ".zip" }, {"Type", "attachment" },{ "ContentType", "application/x-zip-compressed" }, { "Action","allow"}, { "Description", "compress" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".rar" },{"Type", "attachment" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "compress" }, { "CreateTime", DateTime.Now }},

                    new BsonDocument(){{"Extension", ".pdf" },{"Type", "pdf" }, { "ContentType", "application/pdf" }, { "Action","allow"}, { "Description", "Portable Document Format" }, { "CreateTime", DateTime.Now }},

                    new BsonDocument(){{"Extension", ".odg" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "libreoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".ods" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "libreoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".odp" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "libreoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".odf" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "libreoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".odt" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "libreoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".wps" },{"Type", "office" }, { "ContentType", "application/vnd.ms-works" }, { "Action","allow"}, { "Description", "wpsoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".et" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "wpsoffice" }, { "CreateTime", DateTime.Now }},
                    new BsonDocument(){{"Extension", ".dps" },{"Type", "office" }, { "ContentType", "application/octet-stream" }, { "Action","allow"}, { "Description", "wpsoffice" }, { "CreateTime", DateTime.Now }},
                     new BsonDocument(){{"Extension", ".exe" },{"Type", "attachment" }, { "ContentType", "application/octet-stream" }, { "Action","block"}, { "Description", "executable file" }, { "CreateTime", DateTime.Now }}
                };
                database.GetCollection<BsonDocument>("Extension").InsertMany(list);
            }
            if (!databases.Contains("Application"))
            {
                database.CreateCollection("Application");
                database.GetCollection<BsonDocument>("Application").InsertOne(new BsonDocument()
                {
                    {"ApplicationName","FileServiceWeb" },
                    {"AuthCode","3c9deb1f8f6e" },
                    {"Action","allow" },
                    {"Thumbnails",new BsonArray() },
                    {"ThumbnailsDisplay",new BsonArray() },
                    {"Videos",new BsonArray() },
                    {"VideosDisplay",new BsonArray() },
                    {"CreateTime", DateTime.Now }
                });
            }
            if (!databases.Contains("Department"))
            {
                database.CreateCollection("Department");
                database.GetCollection<BsonDocument>("Department").InsertOne(new BsonDocument()
                {
                    {"DepartmentName","Company" },
                    {"DepartmentCode",new Random().RandomCodeHex(12) },
                    {"Department",new BsonArray() },
                    {"CreateTime", DateTime.Now }
                });
            }
            //设置分片键/////////////////////////////////////////////////////////////////////////////////////
            if (!databases.Contains("Ts"))
            {
                database.CreateCollection("Ts");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } });  //shared key
                database.GetCollection<BsonDocument>("Ts").Indexes.CreateOne(c);
            }
            if (!databases.Contains("Download"))
            {
                database.CreateCollection("Download");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } }); //shared key
                database.GetCollection<BsonDocument>("Download").Indexes.CreateOne(c);
            }
            if (!databases.Contains("Log"))
            {
                database.CreateCollection("Log");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } }); //shared key
                database.GetCollection<BsonDocument>("Log").Indexes.CreateOne(c);
            }
            if (!databases.Contains("VideoCapture"))
            {
                database.CreateCollection("VideoCapture");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } });  //shared key
                database.GetCollection<BsonDocument>("VideoCapture").Indexes.CreateOne(c);
            }
            if (!databases.Contains("FilePreview"))
            {
                database.CreateCollection("FilePreview");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } });  //shared key
                database.GetCollection<BsonDocument>("FilePreview").Indexes.CreateOne(c);
            }
            if (!databases.Contains("FilePreviewMobile"))
            {
                database.CreateCollection("FilePreviewMobile");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } });  //shared key
                database.GetCollection<BsonDocument>("FilePreviewMobile").Indexes.CreateOne(c);
            }
            if (!databases.Contains("Thumbnail"))
            {
                database.CreateCollection("Thumbnail");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "From", 1 }, { "CreateTime", 1 } });  //shared key
                database.GetCollection<BsonDocument>("Thumbnail").Indexes.CreateOne(c);
            }
            if (!databases.Contains("M3u8"))
            {
                database.CreateCollection("M3u8");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "SourceId", 1 } });  //index
                database.GetCollection<BsonDocument>("M3u8").Indexes.CreateOne(c);
            }
            if (!databases.Contains("Shared"))
            {
                database.CreateCollection("Shared");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "FileId", 1 } });  //index
                database.GetCollection<BsonDocument>("Shared").Indexes.CreateOne(c);
            }
            if (!databases.Contains("TsTime"))
            {
                database.CreateCollection("TsTime");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "SourceId", 1 } });  //index
                database.GetCollection<BsonDocument>("TsTime").Indexes.CreateOne(c);
            }
            if (!databases.Contains("Task"))
            {
                database.CreateCollection("Task");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "FileId", 1 } });  //index
                database.GetCollection<BsonDocument>("Task").Indexes.CreateOne(c);
                var c1 = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "Output._id", 1 } });  //index
                database.GetCollection<BsonDocument>("Task").Indexes.CreateOne(c1);
            }
            if (!databases.Contains("fs.files"))
            {
                database.CreateCollection("fs.files");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "md5", 1 } });  //index
                database.GetCollection<BsonDocument>("fs.files").Indexes.CreateOne(c);
            }
            if (!databases.Contains("convert.files"))
            {
                database.CreateCollection("convert.files");
                var c = new CreateIndexModel<BsonDocument>(new BsonDocument() { { "md5", 1 } });  //index
                database.GetCollection<BsonDocument>("convert.files").Indexes.CreateOne(c);
            }
        }
    }
}

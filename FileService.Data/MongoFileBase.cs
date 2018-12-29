using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.IO;
using System.Threading.Tasks;

namespace FileService.Data
{
    public class MongoFileBase : MongoBase
    {
        public GridFSBucket gridFSBucket;
        public MongoFileBase(string collection, string bucketName) : base(collection)
        {
            gridFSBucket = new GridFSBucket(MongoDatabase, new GridFSBucketOptions() { BucketName = bucketName });
        }
        public ObjectId Upload(string fileName, Stream stream, BsonDocument metadata)
        {
            return gridFSBucket.UploadFromStream(fileName, stream, new GridFSUploadOptions() { Metadata = metadata });
        }
        public Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata)
        {
            return gridFSBucket.UploadFromStreamAsync(fileName, stream, new GridFSUploadOptions() { Metadata = metadata });
        }
        public GridFSDownloadStream DownLoad(ObjectId id)
        {
            return gridFSBucket.OpenDownloadStream(id);
        }
        public GridFSDownloadStream DownLoadSeekable(ObjectId id)
        {
            return gridFSBucket.OpenDownloadStream(id, new GridFSDownloadOptions() { Seekable = true });
        }
        public void Delete(ObjectId id)
        {
            gridFSBucket.Delete(id);
        }
        
    }
}

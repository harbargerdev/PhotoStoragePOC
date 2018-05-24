using Amazon.DynamoDBv2.DataModel;
using System;

namespace PhotoStoragePOC.DocumentUpload.Entities
{
    [DynamoDBTable("photostoragepoc")]
    public class DocumentEntity
    {
        [DynamoDBHashKey]
        public long ID { get; set; }
        public string DocumentOwner { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string DocumentType { get; set; }

        public DocumentEntity()
        {
        }
    }
}

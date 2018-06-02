using Amazon.DynamoDBv2.DataModel;
using System;

namespace PhotoStoragePOC.DocumentUpload.Entities
{
    [DynamoDBTable("photostoragepoc")]
    [Serializable]
    public class DocumentEntity
    {
        [DynamoDBHashKey]
        public long ID { get; set; }
        [DynamoDBProperty(AttributeName = "DocumentOwner")]
        public string DocumentOwner { get; set; }
        [DynamoDBProperty(AttributeName = "CreateDate")]
        public DateTime CreateDate { get; set; }
        [DynamoDBProperty(AttributeName = "LastUpdateDate")]
        public DateTime LastUpdateDate { get; set; }
        [DynamoDBProperty(AttributeName = "FileName")]
        public string FileName { get; set; }
        [DynamoDBProperty(AttributeName = "Url")]
        public string Url { get; set; }
        [DynamoDBIgnore]
        public string DocumentType { get; set; }

        public DocumentEntity()
        {
        }
    }
}

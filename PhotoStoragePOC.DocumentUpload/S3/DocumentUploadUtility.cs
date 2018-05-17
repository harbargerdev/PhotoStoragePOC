using Amazon.S3;
using Amazon.S3.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using System.IO;

namespace PhotoStoragePOC.DocumentUpload.S3
{
    public class DocumentUploadUtility
    {
        private string AccessKey;
        private string SecretKey;
        private string DefaultRegion;
        private string BucketName;

        private static AmazonS3Client s3Client;
        
        public DocumentUploadUtility()
        {
        }

        /// <summary>
        /// Constructor to take parameters for the Key and Secret for AWS.
        /// </summary>
        /// <param name="key">AWS Key</param>
        /// <param name="secret">AWS Secret</param>
        public DocumentUploadUtility(string key, string secret)
        {
            AccessKey = key;
            SecretKey = secret;
        }

        /// <summary>
        /// Constructor that takes the bucket, key, and secret
        /// </summary>
        /// <param name="bucketName">AWS Bucket name</param>
        /// <param name="key">AWS Key</param>
        /// <param name="secret">AWS Secret</param>
        public DocumentUploadUtility(string bucketName, string key, string secret)
        {
            BucketName = bucketName;
            AccessKey = key;
            SecretKey = secret;
        }

        public UploadStatusEntity UploadDocumentToS3(FileStream stream, string fileName, string userId)
        {
            UploadStatusEntity uploadStatus = new UploadStatusEntity();

            s3Client = new AmazonS3Client(AccessKey, SecretKey, DefaultRegion);

            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = BucketName,
                Key = userId + '/' + fileName,
                InputStream = stream
            };

            return uploadStatus;
        }
    }
}

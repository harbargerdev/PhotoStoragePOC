using Amazon.S3;
using Amazon.S3.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using System.IO;
using Amazon.SecurityToken.Model;
using Amazon;
using PhotoStoragePOC.DocumentUpload.STS;
using System;

namespace PhotoStoragePOC.DocumentUpload.S3
{
    public class DocumentUploadUtility
    {
        private string AccessKey;
        private string SecretKey;
        private string SessionToken;
        private string BucketName;

        private static AmazonS3Client s3Client;
        
        public DocumentUploadUtility()
        {
            throw new NotImplementedException("Cannot use default constructor");
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

            StsUtility stsUtility = new StsUtility();
            Credentials credentials = stsUtility.GetSessionToken(key, secret);

            AccessKey = credentials.AccessKeyId;
            SecretKey = credentials.SecretAccessKey;
            SessionToken = credentials.SessionToken;

            //AWSCredentials credentials = BuildCredentials(key, secret);
            //GetSessionToken(credentials);
        }

        #region Public Methods

        public UploadStatusEntity UploadDocumentToS3(FileStream stream, string fileName, string userId)
        {
            UploadStatusEntity uploadStatus = new UploadStatusEntity();

            AmazonS3Config config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };

            s3Client = new AmazonS3Client(AccessKey, SecretKey, SessionToken, config);

            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = BucketName,
                Key = userId + '/' + fileName,
                InputStream = stream
            };

            try
            {
                PutObjectResponse response = s3Client.PutObject(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploadStatus.IsSuccess = true;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            
            return uploadStatus;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}

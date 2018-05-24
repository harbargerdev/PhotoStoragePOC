using Amazon.S3;
using Amazon.S3.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using System.IO;
using Amazon.SecurityToken.Model;
using Amazon;
using PhotoStoragePOC.DocumentUpload.STS;
using System;
using System.Collections.Generic;
using System.Net;
using PhotoStoragePOC.DocumentUpload.DataAccessLayer;

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

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    uploadStatus.IsSuccess = true;
                    DocumentEntity entity = new DocumentEntity()
                    {
                        DocumentOwner = userId,
                        FileName = fileName,
                        CreateDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,
                        Url = "https://s3.amazonaws.com/" + BucketName + "/" + userId + "/" + fileName
                    };

                    DocumentDbDALC documentDb = new DocumentDbDALC(AccessKey, SecretKey, SessionToken);

                    documentDb.InsertDocumentRecord(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return uploadStatus;
        }

        public List<DocumentEntity> ListDocuments(string userId)
        {
            List<DocumentEntity> documents = new List<DocumentEntity>();

            SetS3Client();

            ListObjectsV2Request request = new ListObjectsV2Request()
            {
                BucketName = this.BucketName,
                Prefix = userId
            };

            try
            {
                ListObjectsV2Response response = s3Client.ListObjectsV2(request);

                if(response.S3Objects.Count > 0)
                {
                    foreach(S3Object s3object in response.S3Objects)
                    {
                        documents.Add(ConvertS3ObjectToDocumentEntity(s3object));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return documents;
        }

        public TestDocumentEntity GetTestDocument(string userId, string fileName, string bucketName)
        {
            TestDocumentEntity document = null;

            SetS3Client();

            GetObjectRequest request = new GetObjectRequest()
            {
                Key = userId + '/' + fileName,
                BucketName = bucketName                
            };

            try
            {
                GetObjectResponse response = s3Client.GetObject(request);

                if(response.HttpStatusCode.Equals(HttpStatusCode.OK))
                {
                    document = new TestDocumentEntity();

                    string[] split_key = response.Key.Split('/');
                    document.Owner = split_key[0];
                    document.FileName = split_key[1];

                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        document.Contents = reader.ReadToEnd();
                    }

                    //FileStream copy = File.Create("~/" + response.Key);
                    //response.WriteResponseStreamToFile(copy.Name, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return document;
        }

        public bool DeleteS3Object(string fileName, string userId, string bucketName)
        {
            bool status = false;

            SetS3Client();

            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = userId + "/" + fileName
            };

            try
            {
                DeleteObjectResponse response = s3Client.DeleteObject(request);

                status = !response.ResponseMetadata.RequestId.Equals(String.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return status;
        }

        #endregion

        #region Private Methods

        private DocumentEntity ConvertS3ObjectToDocumentEntity(S3Object s3Object)
        {
            DocumentEntity document = new DocumentEntity();

            document.CreateDate = s3Object.LastModified;
            string[] owner_key = s3Object.Key.Split('/');
            document.DocumentOwner = owner_key[0];
            document.FileName = owner_key[1];
            
            return document;
        }

        private void SetS3Client()
        {
            if(s3Client == null)
            {
                AmazonS3Config config = new AmazonS3Config() { RegionEndpoint = RegionEndpoint.USEast1 };

                s3Client = new AmazonS3Client(AccessKey, SecretKey, SessionToken, config);
            }
        }

        #endregion
    }
}

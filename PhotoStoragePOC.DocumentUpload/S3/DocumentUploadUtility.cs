using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using System.IO;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon;

namespace PhotoStoragePOC.DocumentUpload.S3
{
    public class DocumentUploadUtility
    {
        private string AccessKey;
        private string SecretKey;
        private string DefaultRegion;
        private string SessionToken;
        private string BucketName;

        private static AmazonS3Client s3Client;
        
        public DocumentUploadUtility()
        {
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

            AWSCredentials credentials = BuildCredentials(key, secret);
            GetSessionToken(credentials);
        }

        public DocumentUploadUtility(AWSCredentials credentials, string bucketName)
        {
            GetSessionToken(credentials);

            BucketName = bucketName;
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

        private void GetSessionToken(AWSCredentials credentials)
        {
            using (var stsClient = new AmazonSecurityTokenServiceClient(credentials))
            {                
                try
                {
                    var getSessionTokenRequest = new GetSessionTokenRequest()
                    {
                        DurationSeconds = 7200
                    };

                    GetSessionTokenResponse response = stsClient.GetSessionToken(getSessionTokenRequest);

                    Credentials sessionCredentials = response.Credentials;

                    AccessKey = sessionCredentials.AccessKeyId;
                    SecretKey = sessionCredentials.SecretAccessKey;
                    SessionToken = sessionCredentials.SessionToken;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }                
            }
        }

        private AWSCredentials BuildCredentials(string accessKey, string secretKey)
        {
            AWSCredentials credentials = null;

            var options = new CredentialProfileOptions()
            {
                AccessKey = accessKey,
                SecretKey = secretKey
            };

            CredentialProfile profile = new CredentialProfile("default", options);
            profile.Region = RegionEndpoint.USEast1;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);

            AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out credentials);

            return credentials;
        }

        #endregion
    }
}

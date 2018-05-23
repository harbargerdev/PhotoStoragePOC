using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload.STS
{
    public class StsUtility
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        public StsUtility()
        {
        }

        public StsUtility(string accessKey, string secretKey)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>AWSCredential for the instance versions of the string</returns>
        public AWSCredentials GetAWSCredentials()
        {
            AWSCredentials credentials = null;

            var options = new CredentialProfileOptions()
            {
                AccessKey = this.AccessKey,
                SecretKey = this.SecretKey
            };

            CredentialProfile profile = new CredentialProfile("default", options);
            profile.Region = RegionEndpoint.USEast1;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);

            AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out credentials);

            return credentials;
        }

        public Credentials GetSessionToken(AWSCredentials credentials)
        {
            Credentials sessionCredentials = null;

            using (var stsClient = new AmazonSecurityTokenServiceClient(credentials))
            {
                try
                {
                    var getSessionTokenRequest = new GetSessionTokenRequest() { DurationSeconds = 7200 };

                    GetSessionTokenResponse response = stsClient.GetSessionToken(getSessionTokenRequest);

                    sessionCredentials = response.Credentials;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return sessionCredentials;
        }

        /// <summary>
        /// Master method that calls the GetAWSCredentials method and returns standard method
        /// of GetSessionToken
        /// </summary>
        /// <param name="accessKey">AWS API Access Key</param>
        /// <param name="secretKey">AWS API Secret Key</param>
        /// <returns></returns>
        public Credentials GetSessionToken(string accessKey, string secretKey)
        {
            // Update Properties
            AccessKey = accessKey;
            SecretKey = secretKey;

            // Get AWSCredentials
            AWSCredentials awsCredentials = GetAWSCredentials();

            // Return shared method
            return GetSessionToken(awsCredentials);
        }
    }
}

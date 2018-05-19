using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.IdentityManagement;
using Amazon;
using Amazon.IdentityManagement.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using PhotoStoragePOC.DocumentUpload.S3;
using System.IO;

namespace PhotoStoragePOC.ConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteStartHeader();
            //CheckSerivceAccountAccess();
            VerifyBucketUpload();

            Console.Read();
        }

        private static void VerifyBucketUpload()
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("| Testing S3 File Upload                             |");
            Console.WriteLine("======================================================");

            // Set some defaults
            string defaultBucket = "photostoragepoc";
            // AWSCredentials credentials = BuildCredentials();

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(defaultBucket, "", "");


            // Build file stream
            string fileName = "test" + DateTime.Now.ToFileTime().ToString() + ".txt";
            FileStream stream = File.Create(fileName);
            string testMessage = "This is a test message created " + DateTime.Now.ToFileTime().ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(testMessage);
            stream.BeginWrite(Encoding.ASCII.GetBytes(testMessage), 0, Encoding.ASCII.GetByteCount(testMessage), null, null);

            // Test file upload
            utility.UploadDocumentToS3(stream, fileName, "testuser");
        }

        private static void WriteStartHeader()
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("| PhotoStoragePoc Console Tests                      |");
            Console.WriteLine("======================================================");
            Console.WriteLine("\n");
        }

        private static AWSCredentials BuildCredentials()
        {
            AWSCredentials credentials = null;

            var options = new CredentialProfileOptions()
            {
                AccessKey = "",
                SecretKey = ""
            };

            CredentialProfile profile = new CredentialProfile("development", options);
            profile.Region = RegionEndpoint.USEast1;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);

            AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out credentials);

            return credentials;
        }
    }
}

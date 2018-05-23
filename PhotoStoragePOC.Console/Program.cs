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
using PhotoStoragePOC.DocumentUpload.STS;
using Amazon.SecurityToken.Model;

namespace PhotoStoragePOC.ConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteStartHeader();
            TestStsUtility();
            VerifyBucketUpload();

            Console.Read();
        }

        private static void VerifyBucketUpload()
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing S3 File Upload                               |");
            Console.WriteLine("========================================================");

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

            try
            {
                // Test file upload
                utility.UploadDocumentToS3(stream, fileName, "testuser");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }
        }

        private static void TestStsUtility()
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing StsUtility                                   |");
            Console.WriteLine("========================================================");
            Console.WriteLine("\n");

            try
            {
                StsUtility stsUtility = new StsUtility();
                Credentials credentials = stsUtility.GetSessionToken("", "");

                Console.WriteLine("Temporary Credentials were located ...");
                Console.WriteLine("Session Access Key: " + credentials.AccessKeyId);
                Console.WriteLine("Session Token: " + credentials.SessionToken);
                Console.WriteLine("Session expiration: " + credentials.Expiration);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }
            
        }

        private static void WriteStartHeader()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("| PhotoStoragePoc Console Tests                          |");
            Console.WriteLine("==========================================================");
            Console.WriteLine("\n");
        }
    }
}

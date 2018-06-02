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
using PhotoStoragePOC.DocumentUpload;

namespace PhotoStoragePOC.ConsoleTests
{
    public class Program
    {
        private static string AccessKey = "";
        private static string SecretKey = "";
        private static string DefaultBucket = "photostoragepoc";
        private static string TestUser = "testuser";
        private static UserEntity User;

        public static void Main(string[] args)
        {
            string filename;

            // Prep Tests
            CreateTestUser();
            WriteStartHeader();

            // Tests
            // TestStsUtility();
            filename = VerifyBucketUpload();
            TestListS3Objects();
            TestGetS3Object(filename);
            TestUpdateS3Object(filename);
            TestDeleteS3Object(filename);

            Console.Read();
        }

        private static string VerifyBucketUpload()
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing S3 File Upload                               |");
            Console.WriteLine("========================================================");

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(DefaultBucket, AccessKey, SecretKey);
            
            // Build file stream
            string fileName = "test" + DateTime.Now.ToFileTime().ToString() + ".txt";
            FileStream stream = File.Create(fileName);
            string testMessage = "This is a test message created " + DateTime.Now.ToFileTime().ToString();
            //byte[] bytes = Encoding.ASCII.GetBytes(testMessage);
            stream.BeginWrite(Encoding.ASCII.GetBytes(testMessage), 0, Encoding.ASCII.GetByteCount(testMessage), null, null);


            // Switching to Document Processor
            DocumentProcessor processor = new DocumentProcessor(User, DefaultBucket, AccessKey, SecretKey);

            try
            {
                // Test file upload
                //utility.UploadDocumentToS3(stream, fileName, TestUser);
                DocumentEntity document = processor.UploadDocument(fileName, "txt", stream);

                if(document.Url != null && !document.Url.Equals(string.Empty))
                {
                    Console.WriteLine("Document successfully uploaded.");
                    Console.WriteLine("Url: " + document.Url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }

            Console.WriteLine();

            // Clean Up Test File
            stream.Close();
            File.Delete(fileName);

            return fileName;
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
                Credentials credentials = stsUtility.GetSessionToken(AccessKey, SecretKey);

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

            Console.WriteLine();
        }

        private static void TestListS3Objects()
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing S3 File List                                  |");
            Console.WriteLine("========================================================");

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(DefaultBucket, AccessKey, SecretKey);

            try
            {
                // Get List of Objects
                List<DocumentEntity> documents = utility.ListDocuments(TestUser);

                foreach(DocumentEntity document in documents)
                {
                    Console.WriteLine(String.Format("Document found... Document Owner: {0}  Key: {1}", document.DocumentOwner, document.FileName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }

            Console.WriteLine();
        }

        private static void TestGetS3Object(string fileName)
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing Get S3 File                                  |");
            Console.WriteLine("========================================================");

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(DefaultBucket, AccessKey, SecretKey);

            try
            {
                TestDocumentEntity document = utility.GetTestDocument(TestUser, fileName, DefaultBucket);

                Console.WriteLine("Document found ...");
                Console.WriteLine("File Name: " + document.FileName);
                Console.WriteLine("Owner: " + document.Owner);
                Console.WriteLine("File contents:");
                Console.WriteLine(document.Contents);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }

            Console.WriteLine();
        }

        private static void TestUpdateS3Object(string filename)
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing Update S3 File                               |");
            Console.WriteLine("========================================================");

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(DefaultBucket, AccessKey, SecretKey);

            try
            {
                TestDocumentEntity document = utility.GetTestDocument(TestUser, filename, DefaultBucket);

                FileStream stream = File.Create(filename);
                string newLine = "\nThis is a new line for update " + DateTime.Now.ToString();
                stream.BeginWrite(Encoding.ASCII.GetBytes(document.Contents + newLine), 0, Encoding.ASCII.GetByteCount(document.Contents + newLine), null, null);

                DocumentProcessor processor = new DocumentProcessor(User, DefaultBucket, AccessKey, SecretKey);
                DocumentEntity entity = processor.UploadDocument(filename, "txt", stream);

                if(entity != null)
                {
                    Console.WriteLine("Successfully updated file in S3: ");
                    Console.WriteLine("Document Name: " + entity.FileName);
                    Console.WriteLine("Document Created: " + entity.CreateDate.ToLongDateString());
                    Console.WriteLine("Document Last Updated: " + entity.LastUpdateDate.ToLongDateString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }

            File.Delete(filename);

            Console.WriteLine();
        }

        private static void TestDeleteS3Object(string fileName)
        {
            Console.WriteLine("========================================================");
            Console.WriteLine("| Testing Get S3 File                                  |");
            Console.WriteLine("========================================================");

            // Create new utility instance
            DocumentUploadUtility utility = new DocumentUploadUtility(DefaultBucket, AccessKey, SecretKey);

            try
            {
                bool status = utility.DeleteS3Object(fileName, TestUser, DefaultBucket);

                if (status)
                    Console.WriteLine("Successfully deleted " + fileName);
                else
                    Console.WriteLine("Failed to delete " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured ...");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace:\n" + ex.StackTrace);
            }

            Console.WriteLine();
        }

        private static void WriteStartHeader()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("| PhotoStoragePoc Console Tests                          |");
            Console.WriteLine("==========================================================");
            Console.WriteLine("\n");
        }

        private static void CreateTestUser()
        {
            User = new UserEntity()
            {
                UserName = "testuser",
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "fubar@email.org"
            };

        }
    }
}

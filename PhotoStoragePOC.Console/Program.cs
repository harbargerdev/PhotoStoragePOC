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

namespace PhotoStoragePOC.ConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteStartHeader();
            CheckSerivceAccountAccess();

            Console.Read();
        }

        private static void WriteStartHeader()
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("| PhotoStoragePoc Console Tests                      |");
            Console.WriteLine("======================================================");
            Console.WriteLine("\n");
        }

        private static void CheckSerivceAccountAccess()
        {
            Console.WriteLine("Starting test to validate service account access ...\n\n");

            Console.WriteLine("Accessing the local configuration file ...\n\n");

            // Get Credentials from configuration
            var options = new CredentialProfileOptions()
            {
                AccessKey = "",
                SecretKey = ""
            };
            CredentialProfile profile = new CredentialProfile("development", options);
            profile.Region = RegionEndpoint.USEast1;
            var sharedFile = new SharedCredentialsFile();
            sharedFile.RegisterProfile(profile);

            AWSCredentials credentials;
            AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out credentials);
            try
            {
                using (var client = new AmazonIdentityManagementServiceClient(credentials))
                {
                    GetRoleRequest request = new GetRoleRequest();
                    request.RoleName = "ListPutGetPhotoStoratePOCPolicy";

                    var response = client.GetRole(request);

                    Console.WriteLine("Got the following role from IAM: " + response.Role.RoleName.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured in method CheckSerivceAccountAccess()\n");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
            }            
        }
    }
}

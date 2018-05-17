using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload.S3
{
    public class DocumentUploadUtility
    {
        private string AccessKey;
        private string SecretKey;
        private string DefaultRegion;
        private string BucketName;
        
        public DocumentUploadUtility()
        {
        }

        public DocumentUploadUtility(string key, string secret)
        {
            AccessKey = key;
            SecretKey = secret;
        }
    }
}

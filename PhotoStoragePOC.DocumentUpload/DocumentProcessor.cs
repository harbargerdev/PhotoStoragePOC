using PhotoStoragePOC.DocumentUpload.DataAccessLayer;
using PhotoStoragePOC.DocumentUpload.Entities;
using PhotoStoragePOC.DocumentUpload.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload
{
    public class DocumentProcessor
    {
        public UserEntity User { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        // Utilities
        private DocumentUploadUtility documentUploadUtility { get; set; }
        private DocumentDbDALC documentDb { get; set; }

        public DocumentProcessor(UserEntity user, string bucketName, string accessKey, string secretKey)
        {
            User = user;

            AccessKey = accessKey;
            SecretKey = secretKey;

            documentUploadUtility = new DocumentUploadUtility(bucketName, AccessKey, SecretKey);
            documentDb = new DocumentDbDALC(AccessKey, SecretKey);
        }

        public DocumentEntity UploadDocument(string filename, string extension, Stream filestream)
        {
            // Try to retrieve existing file
            DocumentEntity document = documentDb.GetDocumentRecord(User.UserName, filename);

            // If Exists
            if(document != null)
            {
                var status = documentUploadUtility.UploadDocumentToS3(filestream, filename, User.UserName);

                if(status.IsSuccess)
                {
                    document.LastUpdateDate = DateTime.Now;

                    documentDb.UpdateDocumentRecord(document);
                }
            }
            else
            {
                document = UploadNewDocument(filename, extension, filestream);
            }

            return document;
        }

        #region Private Methods

        private DocumentEntity UploadNewDocument(string filename, string extension, Stream filestream)
        {
            DocumentEntity document = null;

            var status = documentUploadUtility.UploadDocumentToS3(filestream, filename, User.UserName);

            if (status.IsSuccess)
            {
                document = status.Document;
                document.CreateDate = DateTime.Now;
                document.LastUpdateDate = DateTime.Now;
                document.DocumentOwner = User.UserName;

                documentDb.InsertDocumentRecord(document);
            }

            return document;
        }

        #endregion
    }
}

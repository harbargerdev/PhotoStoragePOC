using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload.Entities
{
    public class UploadStatusEntity
    {
        public bool IsSuccess { get; set; } 
        public string ErrorMessage { get; set; }
        public string DocumentUrl { get; set; }
        public DocumentEntity Document { get; set; }

        public UploadStatusEntity()
        {
            IsSuccess = false;
            ErrorMessage = string.Empty;
            DocumentUrl = string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload.Entities
{
    public class DocumentEntity
    {
        public long ID { get; set; }
        public string DocumentOwner { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string DocumentType { get; set; }

        public DocumentEntity()
        {
        }
    }
}

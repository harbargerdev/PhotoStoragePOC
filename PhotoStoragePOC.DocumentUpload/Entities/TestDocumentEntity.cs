using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStoragePOC.DocumentUpload.Entities
{
    public class TestDocumentEntity
    {
        public string FileName { get; set; }
        public string Owner { get; set; }
        public string Contents { get; set; }
        public string Url { get; set; }
    }
}

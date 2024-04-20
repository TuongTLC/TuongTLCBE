using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities
{
    public partial class FileUpload
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = null!;
        public Guid? UploadedBy { get; set; }
        public DateTime? UploadDate { get; set; }

        public virtual User? UploadedByNavigation { get; set; }
    }
}

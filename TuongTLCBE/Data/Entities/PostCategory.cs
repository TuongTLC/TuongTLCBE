using System;
using System.Collections.Generic;

namespace TuongTLCBE.Data.Entities
{
    public partial class PostCategory
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}

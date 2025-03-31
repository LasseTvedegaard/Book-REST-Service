using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ReadingNote
    {
        public int NoteId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

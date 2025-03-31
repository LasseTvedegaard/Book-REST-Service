using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class ReadingNoteDto
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

    }

}

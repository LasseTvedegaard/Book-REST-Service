using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class LogCreateDto
    {
        public int BookId { get; set; }
        public string UserId { get; set; }
        public int CurrentPage { get; set; }
        public int NoOfPages { get; set; }
        public string ListType { get; set; }
    }
}

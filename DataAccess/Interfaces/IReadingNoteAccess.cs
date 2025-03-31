using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IReadingNoteAccess
    {
        Task<ReadingNote> GetNoteAsync(int bookId, string userId);
        Task UpsertNoteAsync(ReadingNote note);
        Task<IEnumerable<ReadingNote>> GetAllNotesForBookAsync(int bookId, string userId);

    }

}

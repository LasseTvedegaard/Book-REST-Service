using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IReadingNoteControl
    {
        Task<ReadingNoteDto> GetNoteAsync(int bookId, string userId);
        Task UpsertNoteAsync(ReadingNoteDto dto);

        // NYT: Til versionshistorik
        Task<IEnumerable<ReadingNoteDto>> GetAllNotesForBookAsync(int bookId, string userId);
    }
}

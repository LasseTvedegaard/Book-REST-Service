using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using DTOs;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ReadingNoteControl : IReadingNoteControl
    {
        private readonly IReadingNoteAccess _access;

        public ReadingNoteControl(IReadingNoteAccess access)
        {
            _access = access;
        }

        public async Task<IEnumerable<ReadingNoteDto>> GetAllNotesForBookAsync(int bookId, string userId)
        {
            var notes = await _access.GetAllNotesForBookAsync(bookId, userId);

            return notes.Select(note => new ReadingNoteDto
            {
                BookId = note.BookId,
                UserId = note.UserId,
                Note = note.Note,
                CreatedAt = note.CreatedAt // ✅ returnér CreatedAt
            });
        }

        public async Task UpsertNoteAsync(ReadingNoteDto dto)
        {
            var note = new ReadingNote
            {
                BookId = dto.BookId,
                UserId = dto.UserId,
                Note = dto.Note
                // CreatedAt sættes i DB via DEFAULT GETDATE()
            };

            await _access.UpsertNoteAsync(note);
        }

        public async Task<ReadingNoteDto> GetNoteAsync(int bookId, string userId)
        {
            var note = await _access.GetNoteAsync(bookId, userId);
            if (note == null) return null;

            return new ReadingNoteDto
            {
                BookId = note.BookId,
                UserId = note.UserId,
                Note = note.Note,
                CreatedAt = note.CreatedAt // ✅ for konsistens
            };
        }
    }
}

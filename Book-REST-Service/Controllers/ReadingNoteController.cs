using BusinessLogic.Interfaces;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Book_REST_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadingNoteController : ControllerBase
    {
        private readonly IReadingNoteControl _control;

        public ReadingNoteController(IReadingNoteControl control)
        {
            _control = control;
        }

        // Hent nyeste enkelt-note
        [HttpGet("{bookId}/{userId}")]
        public async Task<IActionResult> GetNote(int bookId, string userId)
        {
            var note = await _control.GetNoteAsync(bookId, userId);
            if (note == null) return NotFound();
            return Ok(note);
        }

        // Tilføj ny note (bliver en ny række)
        [HttpPost]
        public async Task<IActionResult> UpsertNote([FromBody] ReadingNoteDto dto)
        {
            await _control.UpsertNoteAsync(dto);
            return Ok();
        }

        // Hent alle noter (versionshistorik), nyeste først
        [HttpGet]
        public async Task<IActionResult> GetAllNotes([FromQuery] int bookId, [FromQuery] string userId)
        {
            var notes = await _control.GetAllNotesForBookAsync(bookId, userId);
            return Ok(notes.OrderByDescending(n => n.CreatedAt)); // ✅ nyeste først
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesV1.Models.NoteModel;
using NotesV1.Services;

namespace NotesV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private NoteService _noteService;

        public NotesController(NoteService noteService)
        {
            _noteService = noteService;
        }

        /// <summary>
        /// Add a new note
        /// </summary>
        /// <param name="noteText"></param>
        /// <param name="projectName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        [HttpPost("add-note")]
        public ActionResult AddNote(string noteText, string projectName, string attributeName)
        {
            var addNote = _noteService.AddNote(noteText, projectName, attributeName);
            if(addNote == false)
            {
               return BadRequest("No notes added");
            }
            return Ok(addNote);
        }

        /// <summary>
        /// Update notes
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="noteText"></param>
        /// <returns></returns>
        [HttpPut("update-note")]
        public ActionResult UpdateNote(int noteId, string noteText)
        {
            var updateNote = _noteService.UpdateNote(noteId, noteText);
            if(updateNote == false)
            {
               return BadRequest("no notes updated");
            }
            return Ok(updateNote);
        }

        /// <summary>
        /// Delete a note
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        [HttpDelete("delete-note")]
        public ActionResult DeleteNote(int noteId)
        {
            var deleteNote = _noteService.DeleteNote(noteId);
            if(deleteNote == false)
            {
                return BadRequest("No notes deleted");
            }
            return Ok(deleteNote);
        }

        /// <summary>
        /// Get all the notes all information can be null or information can be added
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        [HttpGet("get-all-notes")]
        public async Task<ActionResult<Note>> GetNotes(int? projectId, int? attributeId)
        {
            var getNotes = _noteService.GetNotes(projectId, attributeId);
            if(getNotes == null)
            {
                return BadRequest("No notes found");
            }
            return Ok(getNotes);
        }

        /// <summary>
        /// Get the count of notes that have projects
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-project-notes")]
        public ActionResult GetProjectNotes()
        {
            var projectNotes = _noteService.GetProjectNoteCounts();
            if(projectNotes == null)
            {
                return BadRequest("Error occured getting project notes.");
            }
            return Ok(projectNotes);
        }

        /// <summary>
        /// Get the count of notes that have attributes
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-attribute-notes")]
        public ActionResult GetAttributeNotes()
        {
            var attributeNotes = _noteService.GetAttributesNoteCounts();
            if(attributeNotes == null)
            {
                return BadRequest("Error occured getting attribute notes.");
            }
            return Ok(attributeNotes);
        }
    }

    
}

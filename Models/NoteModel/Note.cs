using NotesV1.Models.AttributeModel;

namespace NotesV1.Models.NoteModel
{
    public class Note
    {
        public int NoteId { get; set; }
        public DateTime Creation { get; set; }
        public string? NoteText { get; set; }
        public string? ProjectName { get; set; }
        public List<DataAttribute> Attributes { get; set; }

    }
}

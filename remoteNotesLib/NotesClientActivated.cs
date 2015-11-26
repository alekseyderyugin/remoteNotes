using System;
using System.Collections.Generic;

namespace remoteNotesLib
{
    public class NotesClientActivated : MarshalByRefObject
    {
        private List<Note> notes;

        public NotesClientActivated()
        {
            notes = new List<Note>();
            Logger.Write("NotesClientActivated was created");
        }

        public void createRecord(Note note)
        {
            note.StateField = StateField.Added;
            notes.Add(note);
        }

        public void updateRecord(Note note)
        {
            note.StateField = StateField.Updated;
            int index = notes.IndexOf(note);
            notes[index] = note;
        }

        public void deleteRecord(Note note)
        {
            note.StateField = StateField.Deleted;
            int index = notes.IndexOf(note);
            notes.RemoveAt(index);
        }

        public List<Note> requestCacheRecords()
        {
            return notes;
        }

        public void clear()
        {
            clear();
        }
    }
}

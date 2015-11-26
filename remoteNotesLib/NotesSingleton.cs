using System;
using System.Collections.Generic;

namespace remoteNotesLib
{
    public class NotesSingleton : MarshalByRefObject
    {
        private List<Note> notes;

        public NotesSingleton()
        {
            notes = new List<Note>();
            PopulateNotes();
            Logger.Write("NotesSingleton was created");
        }

        public List<Note> GetPesistentData()
        {
            Logger.Write("getPersistentData()");
            return notes;
        }

        private void PopulateNotes()
        {
            notes.Add(new Note("Заметка 1", "Контент заметки 1"));
            notes.Add(new Note("Заметка 2", "Контент заметки 2"));
            notes.Add(new Note("Заметка 3", "Контент заметки 3"));
        }

        public void PrintNotes()
        {
            Logger.Write("Singleton stored notes:");
            foreach (Note note in notes) {
                Logger.Write(note.Inspect());
            }
        }
    }
}

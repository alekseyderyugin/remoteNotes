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

            populateNotes();

            Console.WriteLine("NotesSingleton created");
        }

        public List<Note> getPersistentData()
        {
            return notes;
        }

        private void populateNotes()
        {
            notes.Add(new Note("Заметка 1", "Контент заметки 1"));
            notes.Add(new Note("Заметка 2", "Контент заметки 2"));
            notes.Add(new Note("Заметка 3", "Контент заметки 3"));
        }
    }
}

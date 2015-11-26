using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

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
            //note.state = State.Added;
            notes.Add(note);
        }

        public void updateRecord(Note note)
        {
            note.state = State.Updated;
            AddOrReplaceIfExists(note);
        }

        public void deleteRecord(Note note)
        {
            note.state = State.Deleted;
            AddOrReplaceIfExists(note);
        }

        public List<Note> requestCacheRecords()
        {
            return notes;
        }

        public void clear()
        {
            notes.Clear();
        }

        private void AddOrReplaceIfExists(Note note)
        {
            int index = notes.IndexOf(note);
            if (index == -1) {
                //Если записи нет в списке транзакции, добавляем её
                notes.Add(note);
            } else {
                //Иначе запись уже присутствует в списке транзакции, заменяем ёе на новейшую версию
                notes[index] = note;
            }
        }

        public void printNotes()
        {
            Logger.Write("ClentActivated stored notes:");
            foreach (Note note in notes) {
                Logger.Write(note.Inspect());
            }
        }
    }
}

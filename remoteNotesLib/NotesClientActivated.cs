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

        public void CreateRecord(Note note)
        {
            note.state = State.Added;
            notes.Add(note);
        }

        public void UpdateRecord(Note note)
        {
            note.state = State.Updated;
            AddOrReplaceIfExists(note);
        }

        public void DeleteRecord(Note note)
        {
            note.state = State.Deleted;
            AddOrReplaceIfExists(note);
        }

        public List<Note> RequestCacheRecords()
        {
            return notes;
        }

        public void Clear()
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

        public void PrintNotes()
        {
            Logger.Write("ClentActivated stored notes:");
            foreach (Note note in notes) {
                Logger.Write(note.Inspect());
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace remoteNotesLib
{
    public class NotesTransactionSinglecall : MarshalByRefObject
    {
        NotesSingleton singleton;

        public NotesTransactionSinglecall()
        {
            singleton = (NotesSingleton)Activator.GetObject(typeof(NotesSingleton), "http://localhost:13000/NotesSingleton.soap");
            Logger.Write("NoteTransactionSinglecall was created");
        }

        public void Commit(NotesClientActivated clientActivated)
        {
            List<Note> notes = singleton.GetPesistentData();
            Logger.Write("Record");
            Logger.Write(notes[0].id.ToString());
            Logger.Write("notes from db");
            singleton.PrintNotes();
            List<Note> changedNotes = clientActivated.RequestCacheRecords();
            Logger.Write("Record");
            Logger.Write(changedNotes[0].id.ToString());
            Logger.Write("notes from transaction");
            clientActivated.PrintNotes();
            /*Logger.Write(notes.Count.ToString());
            Logger.Write(changedNotes.Count.ToString());
            foreach (Note note in changedNotes) {
                Logger.Write(note.ToString());
                switch (note.state) {
                    case State.Added:
                        Logger.Write("Added");
                        notes.Add(note);
                        Logger.Write(note.ToString());
                        break;
                    case State.Deleted:
                        Logger.Write("Deleted");
                        int index = notes.IndexOf(note);
                        //Правильно функционирующий клиент 
                        //не может содержать в транзации ноду на обновление,
                        //которой нет на сервере в WKS
                        //Debug.Assert(index != -1, "Node should exist");
                        notes.RemoveAt(index);
                        Logger.Write(note.ToString());
                        break;
                    case State.Updated:
                        Logger.Write("Updated");
                        int index1 = notes.IndexOf(note);
                        //Debug.Assert(index1 != -1, "Node should exist");
                        notes[index1] = note;
                        Logger.Write(note.ToString());
                        break;
                    default:
                        Logger.Write("Default case");
                        break;
                }
            }
            */
        }

        public void Rollback(NotesClientActivated clientActivated)
        {
            clientActivated.Clear();
        }
    }
}

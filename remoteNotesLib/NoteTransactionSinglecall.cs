using System;

namespace remoteNotesLib
{
    public class NoteTransactionSinglecall : MarshalByRefObject
    {
        public NoteTransactionSinglecall()
        {
            //NotesSingleton = (NotesSingleton)Activator.GetObject(typeof(NotesSingleton), "ipc://ServerIPC/NotesSingleton.rem");
            Console.WriteLine("NoteTransactionSinglecall created");
        }

        public void commit(NotesClientActivated clientCache)
        {

        }

        public void rollback(NotesClientActivated clientCache)
        {
            clientCache.clear();
        }
    }
}


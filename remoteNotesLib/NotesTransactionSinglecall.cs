using System;
using System.Collections.Generic;

namespace remoteNotesLib
{
    public class NotesTransactionSinglecall : MarshalByRefObject
    {
        NotesSingleton singleton;

        public NotesTransactionSinglecall()
        {
            //Получение инстанса синглтона через remoting. 
            //Это не обходимо потому что если обратиться к нему локально,
            //то создастся обычный объект и в итоге клиент и сервер будут иметь разные инстансы "синглтона"
            singleton = (NotesSingleton)Activator.GetObject(typeof(NotesSingleton), "ipc://NotesIpcChannel/NotesSingleton.rem");
            Logger.Write("NoteTransactionSinglecall was created");
        }

        public void Commit(NotesClientActivated clientActivated)
        {
            List<Note> notes = singleton.GetPesistentData();
            List<Note> changedNotes = clientActivated.RequestCacheRecords();

            Logger.Write("Transaction begin");
            //Проверка целостности, обеспечивающая атомарность транзакции
            //т.е. либо запишется всё, либо ничего
            //(Да, O(2n). Можно сделать за n создавая промежуточный список 
            //с оригинальными данными участвующими в транзакции, но лень)
            VerifyTransaction(notes, changedNotes);
            //Завершение транзакции, если проверка пройдена
            completeTransaction(notes, changedNotes);
            //Очищение списка транзакции клиента
            clientActivated.Clear();
            Logger.Write("Transaction end");
        }

        private void VerifyTransaction(List<Note> notes, List<Note> changedNotes)
        {
            //Для каждого объекта в списке транзакции клиента
            foreach (Note note in changedNotes) {
                //Проверяем только удалённые и обновлённые записи,
                //добавленные добявятся в любом случе без конфликта
                if (note.state == State.Deleted || note.state == State.Updated) {
                    //Ищем объект в списке синглтона
                    int index = notes.IndexOf(note);
                    if (index == -1) {
                        //Запись не найдена, значит она уже была удалена
                        //Rollback();
                        throw new Exception("Не удалось обновить или удалить заметку. Она уже была удалена другим клиентом");
                    } else {
                        Note storedNote = notes[index];
                        //На этапе создания объекта и при сохранении в синглколл
                        //устанавливается поле updatedAt, которое содержит таймстамп.
                        //Тем самым, клиенты, запрашивая список объектов, получают 
                        //объекты с установленным временем последнего сохранения.
                        //Когда какой либо из клиентов изменяет запись и (успешно)
                        //делает коммит, время последнего сохранения объекта обновляется.
                        //Соотвественно, если детектируется попытка другого 
                        //клиента обновить (или удалить) запись которая уже была обновлена первым 
                        //клиентом, транзакция откатывается.
                        if (note.updatedAt != storedNote.updatedAt) {
                            //Rollback();
                            throw new Exception("Не удалось обновить заметку. Она уже была обновлена другим клиентом");
                        }
                    }
                }
            }
        }

        private void completeTransaction(List<Note> notes, List<Note> changedNotes)
        {
            //Для каждого объекта в списке транзакции клиента
            foreach (Note note in changedNotes) {
                switch (note.state) {
                    case State.Added:
                        notes.Add(note);
                        break;
                    case State.Deleted:
                        //Получение индекса объекта в списке синглтона и его удаление
                        notes.RemoveAt(notes.IndexOf(note));
                        break;
                    case State.Updated:
                        //Обновление таймстампа
                        note.updatedAt = DateTime.Now;
                        //Получение индекса объекта в списке синглтона 
                        //и замена на обновлённый клиентом
                        notes[notes.IndexOf(note)] = note;
                        break;
                    default:
                        //Никогда не должно происходить, 
                        //т.к. запись может иметь только три состояния.
                        System.Diagnostics.Debug.Assert(false, "Default case!");
                        break;
                }
            }
            /*В .Net списки не наследуют MarshalByRefObject, но сериализуются,
              соотвественно, получается так, что получив список по ссылке, мы получаем
              всего лишь бесполезную копию списка, а не прокси-объект. Так как он был сериализован,
              передан по сети и десериализован на сервере. Поэтому после добавления,
              удаления или замены данных в нём, его успешно съедал GC.
              Однако реализовать список имплементирующий MarshalByRefObject тоже
              нельзя, потому что каждый вызов будет передаваться по сети (или ipc в
              данном случае, не важно), что слишком ресурсозатратно (время -- тоже ресурс).
              Оптимальнее просто передать весь список по окончании обработки.
              Метод SetPersistentData(List<Note> notes) заменяет список в синглтоне на обновлённый список.
              Метод имеет модификатор доступа internal и может использоваться только
              из текущего assembly. Т.е. из классов библиотеки.
              http://stackoverflow.com/questions/13486467/net-remoting-why-isnt-a-list-remotable
              http://stackoverflow.com/questions/6503380/how-expensive-is-using-marshalbyrefobject-compared-to-serialization
              https://msdn.microsoft.com/en-us/library/7c5ka91b.aspx
             */
            singleton.SetPersistentData(notes);
        }

        public void Rollback(NotesClientActivated clientActivated)
        {
            //Очищение списка транзакции клиента
            clientActivated.Clear();
        }
    }
}

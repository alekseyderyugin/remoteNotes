using Gtk;
using System;
using System.Runtime.Remoting;
using remoteNotesLib;

[Gtk.TreeNode(ListOnly = true)]
public class NoteTreeNode : Gtk.TreeNode
{
    public Note note;

    public NoteTreeNode(Note note)
    {
        this.note = note;
    }

    [Gtk.TreeNodeValue(Column = 0)]
    public string Title
    {
        get
        {
            return note.title;
        }
    }

    [Gtk.TreeNodeValue(Column = 1)]
    public string Content
    {
        get
        {
            return note.content;
        }
    }

    public Note GetNote()
    {
        return note;
    }
}

public partial class MainWindow: Gtk.Window
{
    NotesClientActivated clientActivated;
    NotesSingleton singleton;
    NotesTransactionSinglecall singlecall;

    Gtk.NodeView view;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        VBox mainVBox = new VBox(false, 0);
        HBox nodeViewHBox = new HBox(true, 0);
        HBox crudButtonsHBox = new HBox(true, 0);
        HBox transactionContolButtonsHBox = new HBox(true, 0);

        Button refreshButton = new Button("Refresh");
        Button createButton = new Button("Create");
        Button updateButton = new Button("Update");
        Button deleteButton = new Button("Delete");
        Button commitButton = new Button("Commit");
        Button rollbackButton = new Button("Rollback");

        refreshButton.Clicked += RefreshButtonClicked;
        createButton.Clicked += CreateButtonClicked;
        updateButton.Clicked += UpdateButtonClicked;
        deleteButton.Clicked += DeleteButtonClicked;
        commitButton.Clicked += CommitButtonClicked;
        rollbackButton.Clicked += RollbackButtonClicked;

        HSeparator separator = new HSeparator();

        view = new Gtk.NodeView(Store);

        CellRendererText titleRenderer = new Gtk.CellRendererText();
        CellRendererText contentRenderer = new Gtk.CellRendererText();

        titleRenderer.Editable = true;
        contentRenderer.Editable = true;
        titleRenderer.Edited += NoteTitleEdited;
        contentRenderer.Edited += NoteContentEdited;

        view.AppendColumn("Title", new CellRendererText(), "text", 0);
        view.AppendColumn("Content", new CellRendererText(), "text", 1);

        try {
            //Если сервер и клиент запускаются из ИДЕ (по порядку, но практически одновременно),
            //сервер не успевает создать сокет, поэтому надо немного подождать
            System.Threading.Thread.Sleep(1000);
            RemotingConfiguration.Configure("remoteNotes.exe.config", false);

            clientActivated = new NotesClientActivated();
            singleton = new NotesSingleton();
            singlecall = new NotesTransactionSinglecall();
        } catch (System.Net.WebException) {
            Logger.Write("Unable to connect");
            return;
        }

        foreach (Note note in singleton.GetPesistentData()) {
            store.AddNode(new NoteTreeNode(note));
        }

        nodeViewHBox.PackStart(view, false, true, 0);

        crudButtonsHBox.PackStart(refreshButton, false, true, 0);
        crudButtonsHBox.PackStart(createButton, false, true, 0);
        crudButtonsHBox.PackStart(updateButton, false, true, 0);
        crudButtonsHBox.PackStart(deleteButton, false, true, 0);

        transactionContolButtonsHBox.PackStart(commitButton, false, true, 0);
        transactionContolButtonsHBox.PackStart(rollbackButton, false, true, 0);

        mainVBox.PackStart(nodeViewHBox, true, false, 0);
        mainVBox.PackStart(crudButtonsHBox, true, false, 0);
        mainVBox.PackStart(separator, true, false, 2);
        mainVBox.PackStart(transactionContolButtonsHBox, true, false, 0);

        Add(mainVBox);

        mainVBox.ShowAll();

        Build();
    }

    Gtk.NodeStore store;

    Gtk.NodeStore Store
    {
        get
        {
            if (store == null) {
                store = new Gtk.NodeStore(typeof(NoteTreeNode));
            }
            return store;
        }
    }

    private void NoteTitleEdited(object o, Gtk.EditedArgs args)
    {
        Logger.Write("edited");
        //Gtk.TreeIter iter;
        //store.GetIter(out iter, new Gtk.TreePath(args.Path));

        //Song song = (Song)musicListStore.GetValue(iter, 0);
        //song.Artist = args.NewText;
    }

    private void NoteContentEdited(object o, Gtk.EditedArgs args)
    {
        Logger.Write("content edited");
        //Gtk.TreeIter iter;
        //musicListStore.GetIter(out iter, new Gtk.TreePath(args.Path));

        //Song song = (Song)musicListStore.GetValue(iter, 0);
        //song.Artist = args.NewText;
    }

    private void RefreshButtonClicked(object obj, EventArgs args)
    {
        //Logger.Write(singleton.GetHashCode().ToString());
        singleton.PrintNotes();
        clientActivated.PrintNotes();
        clientActivated.Clear();
        store.Clear();
        FillTable();
    }

    private void CreateButtonClicked(object obj, EventArgs args)
    {
        Note note = new Note("New", "");
        store.AddNode(new NoteTreeNode(note));
        clientActivated.CreateRecord(note);
    }

    private void UpdateButtonClicked(object obj, EventArgs args)
    {
        if (view.NodeSelection.SelectedNode != null) {
            NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
            clientActivated.UpdateRecord(selected.GetNote());
        }
    }

    private void DeleteButtonClicked(object obj, EventArgs args)
    {
        if (view.NodeSelection.SelectedNode != null) {
            NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
            clientActivated.DeleteRecord(selected.GetNote());
        }
    }

    private void CommitButtonClicked(object obj, EventArgs args)
    {
        singlecall.Commit(clientActivated);
    }

    private void RollbackButtonClicked(object obj, EventArgs args)
    {
        singlecall.Rollback(clientActivated);
        store.Clear();
        FillTable();
    }

    private void FillTable()
    {
        foreach (Note note in singleton.GetPesistentData()) {
            store.AddNode(new NoteTreeNode(note));
        }
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

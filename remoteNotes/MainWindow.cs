using Gtk;
using System;
using System.Runtime.Remoting;
using remoteNotesLib;

[Gtk.TreeNode(ListOnly = true)]
public class NoteTreeNode : TreeNode
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

    NodeView view;

    public MainWindow() : base(WindowType.Toplevel)
    {
        VBox mainVBox = new VBox(false, 0);
        HBox nodeViewHBox = new HBox(true, 0);
        HBox crudButtonsHBox = new HBox(true, 0);
        HBox transactionContolButtonsHBox = new HBox(true, 0);

        Button refreshButton = new Button("Refresh");
        Button createButton = new Button("Create");
        Button deleteButton = new Button("Delete");
        Button commitButton = new Button("Commit");
        Button rollbackButton = new Button("Rollback");

        refreshButton.Clicked += RefreshButtonClicked;
        createButton.Clicked += CreateButtonClicked;
        deleteButton.Clicked += DeleteButtonClicked;
        commitButton.Clicked += CommitButtonClicked;
        rollbackButton.Clicked += RollbackButtonClicked;

        HSeparator separator = new HSeparator();

        view = new NodeView(Store);

        CellRendererText titleRenderer = new CellRendererText();
        CellRendererText contentRenderer = new CellRendererText();

        titleRenderer.Editable = true;
        contentRenderer.Editable = true;
        titleRenderer.Edited += NoteTitleEdited;
        contentRenderer.Edited += NoteContentEdited;

        view.AppendColumn("Title", titleRenderer, "text", 0);
        view.AppendColumn("Content", contentRenderer, "text", 1);

        ScrolledWindow scrolledWindow = new ScrolledWindow();
        scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        scrolledWindow.AddWithViewport(view);

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

        nodeViewHBox.PackStart(scrolledWindow, false, true, 0);
        nodeViewHBox.SetSizeRequest(200, 200);

        crudButtonsHBox.PackStart(refreshButton, false, true, 0);
        crudButtonsHBox.PackStart(createButton, false, true, 0);
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
        NoteTreeNode node = store.GetNode(new TreePath(args.Path)) as NoteTreeNode;
        Note note = node.GetNote();
        note.title = args.NewText;
        clientActivated.UpdateRecord(note);
    }

    private void NoteContentEdited(object o, Gtk.EditedArgs args)
    {
        Logger.Write("content edited");
        NoteTreeNode node = store.GetNode(new TreePath(args.Path)) as NoteTreeNode;
        Note note = node.GetNote();
        note.content = args.NewText;
        clientActivated.UpdateRecord(note);
    }

    private void RefreshButtonClicked(object obj, EventArgs args)
    {
        singleton.PrintNotes();
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

    private void DeleteButtonClicked(object obj, EventArgs args)
    {
        if (view.NodeSelection.SelectedNode != null) {
            NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
            store.RemoveNode(selected);
            clientActivated.DeleteRecord(selected.GetNote());
        }
    }

    private void CommitButtonClicked(object obj, EventArgs args)
    {
        
        clientActivated.PrintNotes();
        try {
            singlecall.Commit(clientActivated);
        } catch (Exception e) {
            Logger.Write(e.Message);
            MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.None, e.Message);
            dialog.Show();
        } finally {
            RefreshButtonClicked(obj, args);
        }
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

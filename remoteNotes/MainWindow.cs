using Gtk;
using System;
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

    Button updateButton;
    Button deleteButton;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        VBox mainVBox = new VBox(false, 0);
        HBox nodeViewHBox = new HBox(true, 0);
        HBox crudButtonsHBox = new HBox(true, 0);
        HBox transactionContolButtonsHBox = new HBox(true, 0);

        Button refreshButton = new Button("Refresh");
        Button createButton = new Button("Create");
        updateButton = new Button("Update");
        deleteButton = new Button("Delete");
        Button commitButton = new Button("Commit");
        Button rollbackButton = new Button("Rollback");

        refreshButton.Clicked += RefreshAction;
        createButton.Clicked += CreateAction;
        updateButton.Clicked += UpdateAction;
        deleteButton.Clicked += DeleteAction;
        commitButton.Clicked += CommitAction;
        rollbackButton.Clicked += rollbackAction;

        updateButton.Sensitive = false;
        deleteButton.Sensitive = false;

        HSeparator separator = new HSeparator();

        view = new Gtk.NodeView(Store);
        view.AppendColumn("Title", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Content", new Gtk.CellRendererText(), "text", 1);
        view.NodeSelection.Changed += OnSelectionChanged;

        clientActivated = new NotesClientActivated();
        singleton = new NotesSingleton();
        singlecall = new NotesTransactionSinglecall();

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

    private void OnSelectionChanged(object obj, EventArgs args)
    {
        updateButton.Sensitive = true;
        deleteButton.Sensitive = true;
    }

    private void RefreshAction(object obj, EventArgs args)
    {
        clientActivated.PrintNotes();
        store.Clear();
        foreach (Note note in singleton.GetPesistentData()) {
            store.AddNode(new NoteTreeNode(note));
        }
    }

    private void CreateAction(object obj, EventArgs args)
    {
        Note note = new Note("", "");
        store.AddNode(new NoteTreeNode(note));
        clientActivated.CreateRecord(note);
    }

    private void UpdateAction(object obj, EventArgs args)
    {
        NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
        clientActivated.UpdateRecord(selected.GetNote());
    }

    private void DeleteAction(object obj, EventArgs args)
    {
        NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
        clientActivated.DeleteRecord(selected.GetNote());
    }

    private void CommitAction(object obj, EventArgs args)
    {
        singlecall.Commit(clientActivated);
    }

    private void rollbackAction(object obj, EventArgs args)
    {
        singlecall.Rollback(clientActivated);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

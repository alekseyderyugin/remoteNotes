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

    public Note getNote()
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

    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
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

        refreshButton.Clicked += refreshAction;
        createButton.Clicked += createAction;
        updateButton.Clicked += updateAction;
        deleteButton.Clicked += deleteAction;
        commitButton.Clicked += commitAction;
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

        foreach (Note note in singleton.getPesistentData())
        {
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
            if (store == null)
            {
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

    private void refreshAction(object obj, EventArgs args)
    {
        clientActivated.printNotes();
        store.Clear();
        foreach (Note note in singleton.getPesistentData())
        {
            store.AddNode(new NoteTreeNode(note));
        }
    }

    private void createAction(object obj, EventArgs args)
    {
        Note note = new Note("", "");
        store.AddNode(new NoteTreeNode(note));
        clientActivated.createRecord(note);
    }

    private void updateAction(object obj, EventArgs args)
    {
        NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
        clientActivated.updateRecord(selected.getNote());
    }

    private void deleteAction(object obj, EventArgs args)
    {
        NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
        clientActivated.deleteRecord(selected.getNote());
    }

    private void commitAction(object obj, EventArgs args)
    {
        singlecall.commit(clientActivated);
    }

    private void rollbackAction(object obj, EventArgs args)
    {
        singlecall.rollback(clientActivated);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

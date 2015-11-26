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
            return this.note.title;
        }
    }

    [Gtk.TreeNodeValue(Column = 1)]
    public string Content
    {
        get
        {
            return this.note.content; 
        } 
    }

    public Note getNote()
    {
        return this.note;
    }
}

public partial class MainWindow: Gtk.Window
{
    private NotesClientActivated clientActivated;
    private NotesSingleton singleton;
    private NotesTransactionSinglecall singlecall;

    private Gtk.NodeView view;

    private Button updateButton;
    private Button deleteButton;

    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        VBox mainVBox = new VBox(false, 0);
        HBox nodeViewHBox = new HBox(true, 0);
        HBox crudButtonsHBox = new HBox(true, 0);
        HBox transactionContolButtonsHBox = new HBox(true, 0);

        Button refreshButton = new Button("Refresh");
        Button createButton = new Button("Create");
        this.updateButton = new Button("Update");
        this.deleteButton = new Button("Delete");
        Button commitButton = new Button("Commit");
        Button rollbackButton = new Button("Rollback");

        refreshButton.Clicked += this.refreshAction;
        createButton.Clicked += this.createAction;
        this.updateButton.Clicked += this.updateAction;
        this.deleteButton.Clicked += this.deleteAction;
        commitButton.Clicked += this.commitAction;
        rollbackButton.Clicked += this.rollbackAction;

        this.updateButton.Sensitive = false;
        this.deleteButton.Sensitive = false;

        HSeparator separator = new HSeparator();

        this.view = new Gtk.NodeView(Store);
        view.AppendColumn("Title", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Content", new Gtk.CellRendererText(), "text", 1);
        view.NodeSelection.Changed += new System.EventHandler(OnSelectionChanged);

        this.clientActivated = new NotesClientActivated();
        this.singleton = new NotesSingleton();
        this.singlecall = new NotesTransactionSinglecall();

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
        this.updateButton.Sensitive = true;
        this.deleteButton.Sensitive = true;
    }

    private void refreshAction(object obj, EventArgs args)
    {
        store.Clear();
        foreach (Note note in this.singleton.getPesistentData())
        {
            store.AddNode(new NoteTreeNode(note));
        }
    }

    private void createAction(object obj, EventArgs args)
    {
        
        Note note = new Note("Note", "Empty");
        store.AddNode(new NoteTreeNode(note));
        System.Console.Write("12312");
        this.clientActivated.createRecord(note);
    }

    private void updateAction(object obj, EventArgs args)
    {
        
    }

    private void deleteAction(object obj, EventArgs args)
    {
        NoteTreeNode selected = (NoteTreeNode)view.NodeSelection.SelectedNode;
        this.clientActivated.deleteRecord(selected.getNote());
    }

    private void commitAction(object obj, EventArgs args)
    {
        this.singlecall.commit(this.clientActivated);
    }

    private void rollbackAction(object obj, EventArgs args)
    {
        this.singlecall.rollback(clientActivated);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

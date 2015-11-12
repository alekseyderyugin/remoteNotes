using Gtk;

[Gtk.TreeNode(ListOnly = true)]
public class MyTreeNode : Gtk.TreeNode
{
    string song_title;

    public MyTreeNode(string artist, string song_title)
    {
        Artist = artist;
        this.song_title = song_title;
    }

    [Gtk.TreeNodeValue(Column = 0)]
    public string Artist;

    [Gtk.TreeNodeValue(Column = 1)]
    public string SongTitle { get { return song_title; } }
}

public partial class MainWindow: Gtk.Window
{
    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
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

        HSeparator separator = new HSeparator();

        Gtk.NodeView view = new Gtk.NodeView(Store);
        view.AppendColumn("Artist", new Gtk.CellRendererText(), "text", 0);
        view.AppendColumn("Song Title", new Gtk.CellRendererText(), "text", 1);

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
                store = new Gtk.NodeStore(typeof(MyTreeNode));
                store.AddNode(new MyTreeNode("The Beatles", "Yesterday"));
                store.AddNode(new MyTreeNode("Peter Gabriel", "In Your Eyes"));
                store.AddNode(new MyTreeNode("Rush", "Fly By Night"));
            }
            return store;
        }
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

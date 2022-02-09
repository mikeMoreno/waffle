using Newtonsoft.Json;

namespace Waffle
{
    public partial class BookmarkAdder : Form
    {
        private readonly string absoluteUrl;
        List<BookmarkEntity> BookmarkEntities { get; }

        public BookmarkAdder(string absoluteUrl)
        {
            InitializeComponent();

            absoluteUrl = absoluteUrl.Trim();

            this.absoluteUrl = absoluteUrl;

            txtName.Text = absoluteUrl;

            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            if (!File.Exists(bookmarkFile))
            {
                return;
            }

            BookmarkEntities = LoadBookmarks();

            PopulateBookmarkTree(BookmarkEntities);
        }

        private void PopulateBookmarkTree(List<BookmarkEntity> bookmarkEntities)
        {
            bookmarkTree.Nodes.Clear();

            foreach (var bookmarkEntity in bookmarkEntities)
            {
                PopulateBookmarkTree(bookmarkEntity);
            }
        }

        private void PopulateBookmarkTree(BookmarkEntity bookmarkEntity)
        {
            if (bookmarkEntity is not BookmarkFolder folder)
            {
                var bookmark = bookmarkEntity as Bookmark;

                var bookmarkNode = new TreeNode()
                {
                    Text = bookmark.Name,
                    Tag = bookmark,
                };

                bookmarkTree.Nodes.Add(bookmarkNode);

                return;
            }

            var folderNode = new TreeNode()
            {
                Text = folder.Name,
                Tag = folder,
            };

            bookmarkTree.Nodes.Add(folderNode);

            foreach (var childBookmark in folder.BookmarkEntities.OfType<Bookmark>())
            {
                folderNode.Nodes.Add(new TreeNode()
                {
                    Text = childBookmark.Name,
                    Tag = childBookmark,
                });
            }

            foreach (var childFolder in folder.BookmarkEntities.OfType<BookmarkFolder>())
            {
                PopulateBookmarkTree(childFolder);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");

                return;
            }

            var selectedNode = bookmarkTree.SelectedNode;

            if (selectedNode == null)
            {
                var bookmark = new Bookmark()
                {
                    Name = name,
                    Url = absoluteUrl,
                };

                BookmarkEntities.Add(bookmark);
            }
            else
            {
                var entity = selectedNode.Tag as BookmarkEntity;

                if (entity.BookmarkEntityType == "BookmarkFolder")
                {
                    var bookmark = new Bookmark()
                    {
                        Name = name,
                        Url = absoluteUrl,
                    };

                    var folder = entity as BookmarkFolder;

                    folder.BookmarkEntities.Add(bookmark);
                }
                else
                {
                    var parent = selectedNode.Parent;

                    if (parent == null)
                    {
                        var bookmark = new Bookmark()
                        {
                            Name = name,
                            Url = absoluteUrl,
                        };

                        BookmarkEntities.Add(bookmark);
                    }
                    else
                    {
                        var bookmark = new Bookmark()
                        {
                            Name = name,
                            Url = absoluteUrl,
                        };

                        var parentFolder = parent.Tag as BookmarkFolder;

                        parentFolder.BookmarkEntities.Add(bookmark);
                    }
                }
            }

            SaveBookmarks(BookmarkEntities);

            PopulateBookmarkTree(BookmarkEntities);
        }

        private List<BookmarkEntity> LoadBookmarks()
        {
            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            if (!File.Exists(bookmarkFile))
            {
                return new List<BookmarkEntity>();
            }

            var bookmarkEntities = JsonConvert.DeserializeObject<List<BookmarkEntity>>(File.ReadAllText(bookmarkFile), new BookmarkEntityConverter());

            return bookmarkEntities;
        }

        private void SaveBookmarks(List<BookmarkEntity> bookmarkEntities)
        {
            var serializedBookmarks = JsonConvert.SerializeObject(bookmarkEntities, Formatting.Indented);

            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            File.WriteAllText(bookmarkFile, serializedBookmarks);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            //var name = txtName.Text.Trim();

            //if (string.IsNullOrWhiteSpace(name))
            //{
            //    MessageBox.Show("Name cannot be empty.");

            //    return;
            //}

            //var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            //BookmarkFolder bookmarksFolder;

            //if (File.Exists(bookmarkFile))
            //{
            //    var bookmarksText = File.ReadAllText(bookmarkFile);
            //    bookmarksFolder = JsonSerializer.Deserialize<BookmarkFolder>(bookmarksText);
            //}
            //else
            //{
            //    bookmarksFolder = new BookmarkFolder();
            //}

            //var bookmark = bookmarksFolder.Bookmarks.FirstOrDefault(b => b.Url == absoluteUrl);

            //if (bookmark != null)
            //{
            //    bookmarksFolder.Bookmarks.Remove(bookmark);

            //    var serializedBookmarks = JsonSerializer.Serialize(bookmarksFolder, new JsonSerializerOptions() { WriteIndented = true });

            //    File.WriteAllText(bookmarkFile, serializedBookmarks);
            //}

            this.Close();
        }
    }
}

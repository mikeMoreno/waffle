using Newtonsoft.Json;
using Waffle.Lib;

namespace Waffle.Bookmarks
{
    public partial class BookmarkAdder : Form
    {
        private string AbsoluteUrl { get; }

        private List<BookmarkEntity> BookmarkEntities { get; }

        public BookmarkAdder(LinkLine linkLine)
        {
            InitializeComponent();

            AbsoluteUrl = linkLine.Raw.Trim();

            txtName.Text = linkLine.GetUserFriendlyName();

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

        private void PopulateBookmarkTree(BookmarkEntity bookmarkEntity, TreeNode parent = null)
        {
            if (bookmarkEntity is not BookmarkFolder folder)
            {
                var bookmark = bookmarkEntity as Bookmark;

                var bookmarkNode = new TreeNode()
                {
                    Text = bookmark.Name,
                    Tag = bookmark,
                };

                if (parent != null)
                {
                    parent.Nodes.Add(bookmarkNode);
                }
                else
                {
                    bookmarkTree.Nodes.Add(bookmarkNode);
                }

                return;
            }

            var folderNode = new TreeNode()
            {
                Text = folder.Name,
                Tag = folder,
            };

            if (parent != null)
            {
                parent.Nodes.Add(folderNode);
            }
            else
            {
                bookmarkTree.Nodes.Add(folderNode);
            }

            foreach (var childBookmark in folder.BookmarkEntities.OfType<Bookmark>())
            {
                PopulateBookmarkTree(childBookmark, folderNode);
            }

            foreach (var childFolder in folder.BookmarkEntities.OfType<BookmarkFolder>())
            {
                PopulateBookmarkTree(childFolder, folderNode);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // TODO: persist entire selectorLine

            var name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");

                return;
            }

            var bookmark = new Bookmark()
            {
                Name = name,
                Url = AbsoluteUrl,
            };

            var selectedNode = bookmarkTree.SelectedNode;

            if (selectedNode == null)
            {
                BookmarkEntities.Add(bookmark);
            }
            else
            {
                var entity = selectedNode.Tag as BookmarkEntity;

                if (entity.BookmarkEntityType == "BookmarkFolder")
                {
                    var folder = entity as BookmarkFolder;

                    folder.BookmarkEntities.Add(bookmark);
                }
                else
                {
                    var parent = selectedNode.Parent;

                    if (parent == null)
                    {
                        BookmarkEntities.Add(bookmark);
                    }
                    else
                    {
                        var parentFolder = parent.Tag as BookmarkFolder;

                        parentFolder.BookmarkEntities.Add(bookmark);
                    }
                }
            }

            Close();
        }

        private List<BookmarkEntity> LoadBookmarks()
        {
            if (!File.Exists(Globals.BookmarksFile))
            {
                return new List<BookmarkEntity>();
            }

            var bookmarkEntities = JsonConvert.DeserializeObject<List<BookmarkEntity>>(File.ReadAllText(Globals.BookmarksFile), new BookmarkEntityConverter());

            if (bookmarkEntities == null)
            {
                return new List<BookmarkEntity>();
            }

            return bookmarkEntities;
        }

        private void SaveBookmarks(List<BookmarkEntity> bookmarkEntities)
        {
            var serializedBookmarks = JsonConvert.SerializeObject(bookmarkEntities, Formatting.Indented);

            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            File.WriteAllText(bookmarkFile, serializedBookmarks);
        }

        private void btnNewFolder_Click(object sender, EventArgs e)
        {
            bookmarkTree.LabelEdit = true;

            var newBookmarkFolder = new BookmarkFolder()
            {
                Name = "New Folder",
            };

            var newFolderNode = new TreeNode()
            {
                Text = "New Folder",
                Tag = newBookmarkFolder,
            };

            var selectedNode = bookmarkTree.SelectedNode;

            if (selectedNode == null)
            {
                bookmarkTree.Nodes.Add(newFolderNode);

                BookmarkEntities.Add(newBookmarkFolder);
            }
            else
            {
                var entity = selectedNode.Tag as BookmarkEntity;

                if (entity.BookmarkEntityType == "BookmarkFolder")
                {
                    selectedNode.Nodes.Add(newFolderNode);

                    (entity as BookmarkFolder).BookmarkEntities.Add(newBookmarkFolder);
                }
                else
                {
                    var parent = selectedNode.Parent;

                    if (parent == null)
                    {
                        bookmarkTree.Nodes.Add(newFolderNode);

                        BookmarkEntities.Add(newBookmarkFolder);
                    }
                    else
                    {
                        parent.Nodes.Add(newFolderNode);

                        (parent.Tag as BookmarkFolder).BookmarkEntities.Add(newBookmarkFolder);
                    }
                }
            }

            bookmarkTree.SelectedNode = newFolderNode;
            bookmarkTree.SelectedNode.BeginEdit();
        }

        private void BookmarkAdder_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveBookmarks(BookmarkEntities);
        }

        private void bookmarkTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            BeginInvoke(new Action(() => afterAfterEdit(e.Node)));
        }

        private void afterAfterEdit(TreeNode node)
        {
            var bookmarkEntity = node.Tag as BookmarkEntity;

            bookmarkEntity.Name = node.Text;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Waffle
{
    partial class BookmarkPanel : UserControl
    {
        private List<BookmarkEntity> BookmarkEntities { get; }

        public delegate void BookmarkClickedEventHandler(object sender, BookmarkClickedEventArgs e);

        public event BookmarkClickedEventHandler LinkClicked;

        public BookmarkPanel()
        {
            InitializeComponent();

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

        private List<BookmarkEntity> LoadBookmarks()
        {
            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            if (!File.Exists(bookmarkFile))
            {
                return new List<BookmarkEntity>();
            }

            var bookmarkEntities = JsonConvert.DeserializeObject<List<BookmarkEntity>>(File.ReadAllText(bookmarkFile), new BookmarkEntityConverter());

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            SaveBookmarks(BookmarkEntities);

            this.Parent.Controls.Remove(this);
        }

        private void bookmarkTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            BeginInvoke(new Action(() => afterAfterEdit(e.Node)));
        }

        private void afterAfterEdit(TreeNode node)
        {
            var bookmarkEntity = node.Tag as BookmarkEntity;

            bookmarkEntity.Name = node.Text;

            SaveBookmarks(BookmarkEntities);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedNode = bookmarkTree.SelectedNode;

            if (selectedNode == null)
            {
                return;
            }

            BookmarkEntities.Remove(selectedNode.Tag as BookmarkEntity);

            bookmarkTree.Nodes.Remove(selectedNode);

            SaveBookmarks(BookmarkEntities);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedNode = bookmarkTree.SelectedNode;

            if (selectedNode == null)
            {
                return;
            }

            var bookmarkEntity = selectedNode.Tag as BookmarkEntity;

            using (var bookmarkEditor = new BookmarkEditor(bookmarkEntity))
            {
                var ans = bookmarkEditor.ShowDialog();

                if (ans == DialogResult.OK)
                {
                    bookmarkEntity.Name = bookmarkEditor.BookmarkName;

                    if (bookmarkEntity is Bookmark bookmark)
                    {
                        bookmark.Url = bookmarkEditor.BookmarkUrl;
                    }
                }
            }

            PopulateBookmarkTree(BookmarkEntities);
            SaveBookmarks(BookmarkEntities);
        }

        private void bookmarkTree_MouseDown(object sender, MouseEventArgs e)
        {
            var selectedNode = bookmarkTree.GetNodeAt(e.Location);

            if (selectedNode != null)
            {
                bookmarkTree.SelectedNode = selectedNode;
            }

            if (selectedNode != null && e.Button == MouseButtons.Left)
            {
                if (bookmarkTree.SelectedNode.Tag is Bookmark bookmark)
                {
                    LinkClicked?.Invoke(this, new BookmarkClickedEventArgs(bookmark));
                }
            }
        }

        private void btnAddBookmark_Click(object sender, EventArgs e)
        {
            var newBookmark = new Bookmark();

            using (var bookmarkEditor = new BookmarkEditor(newBookmark))
            {
                bookmarkEditor.Text = "Add";

                var ans = bookmarkEditor.ShowDialog();

                if (ans != DialogResult.OK)
                {
                    return;
                }

                newBookmark.Name = bookmarkEditor.BookmarkName;
                newBookmark.Url = bookmarkEditor.BookmarkUrl;

                var selectedNode = bookmarkTree.SelectedNode;

                if (selectedNode == null)
                {
                    BookmarkEntities.Add(newBookmark);
                }
                else
                {
                    var bookmarkEntity = selectedNode.Tag as BookmarkEntity;

                    if (bookmarkEntity is BookmarkFolder folder)
                    {
                        folder.BookmarkEntities.Add(newBookmark);
                    }
                    else
                    {
                        var parent = selectedNode.Parent;

                        if (parent == null)
                        {
                            BookmarkEntities.Add(newBookmark);
                        }
                        else
                        {
                            (parent.Tag as BookmarkFolder).BookmarkEntities.Add(newBookmark);
                        }
                    }
                }
            }

            PopulateBookmarkTree(BookmarkEntities);
            SaveBookmarks(BookmarkEntities);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var newFolder = new BookmarkFolder();

            using (var bookmarkEditor = new BookmarkEditor(newFolder))
            {
                bookmarkEditor.Text = "Add";

                var ans = bookmarkEditor.ShowDialog();

                if (ans != DialogResult.OK)
                {
                    return;
                }

                newFolder.Name = bookmarkEditor.BookmarkName;

                var selectedNode = bookmarkTree.SelectedNode;

                if (selectedNode == null)
                {
                    BookmarkEntities.Add(newFolder);
                }
                else
                {
                    var bookmarkEntity = selectedNode.Tag as BookmarkEntity;

                    if (bookmarkEntity is BookmarkFolder folder)
                    {
                        folder.BookmarkEntities.Add(newFolder);
                    }
                    else
                    {
                        var parent = selectedNode.Parent;

                        if (parent == null)
                        {
                            BookmarkEntities.Add(newFolder);
                        }
                        else
                        {
                            (parent.Tag as BookmarkFolder).BookmarkEntities.Add(newFolder);
                        }
                    }
                }
            }

            PopulateBookmarkTree(BookmarkEntities);
            SaveBookmarks(BookmarkEntities);
        }
    }
}

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

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            bookmarkTree.SelectedNode.BeginEdit();
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

        private void bookmarkTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;

            if ((node.Tag as BookmarkEntity).BookmarkEntityType != "Bookmark")
            {
                return;
            }

            var bookmark = node.Tag as Bookmark;

            LinkClicked?.Invoke(this, new BookmarkClickedEventArgs(bookmark));
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
    }
}

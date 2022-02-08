using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Waffle
{
    public partial class FavoriteAdder : Form
    {
        private readonly string absoluteUrl;
        public FavoriteAdder(string absoluteUrl)
        {
            InitializeComponent();

            absoluteUrl = absoluteUrl.Trim();

            this.absoluteUrl = absoluteUrl;

            txtName.Text = absoluteUrl;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");

                return;
            }

            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            BookmarksFolder bookmarksFolder;

            if (File.Exists(bookmarkFile))
            {
                var bookmarksText = File.ReadAllText(bookmarkFile);
                bookmarksFolder = JsonSerializer.Deserialize<BookmarksFolder>(bookmarksText);
            }
            else
            {
                bookmarksFolder = new BookmarksFolder();
            }

            var bookmark = new Bookmark()
            {
                Name = name,
                Location = absoluteUrl,
            };

            bookmarksFolder.Bookmarks.Add(bookmark);

            var serializedBookmarks = JsonSerializer.Serialize(bookmarksFolder, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(bookmarkFile, serializedBookmarks);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.");

                return;
            }

            var bookmarkFile = Path.Combine(Globals.ApplicationFolder, "bookmarks.json");

            BookmarksFolder bookmarksFolder;

            if (File.Exists(bookmarkFile))
            {
                var bookmarksText = File.ReadAllText(bookmarkFile);
                bookmarksFolder = JsonSerializer.Deserialize<BookmarksFolder>(bookmarksText);
            }
            else
            {
                bookmarksFolder = new BookmarksFolder();
            }

            var bookmark = bookmarksFolder.Bookmarks.FirstOrDefault(b => b.Location == absoluteUrl);

            if (bookmark != null)
            {
                bookmarksFolder.Bookmarks.Remove(bookmark);

                var serializedBookmarks = JsonSerializer.Serialize(bookmarksFolder, new JsonSerializerOptions() { WriteIndented = true });

                File.WriteAllText(bookmarkFile, serializedBookmarks);
            }
        }
    }
}

using Newtonsoft.Json;

namespace Waffle
{
    partial class BookmarkEditor : Form
    {
        public string BookmarkName
        {
            get
            {
                return txtName.Text.Trim();
            }
        }

        public string BookmarkUrl
        {
            get
            {
                return txtUrl.Text.Trim();
            }
        }

        public BookmarkEditor(BookmarkEntity bookmarkEntity)
        {
            InitializeComponent();

            txtName.Text = bookmarkEntity.Name;

            if (bookmarkEntity is Bookmark bookmark)
            {
                txtUrl.Text = bookmark.Url;
            }
            else
            {
                lblUrl.Visible = false;
                txtUrl.Visible = false;
            }
        }
    }
}

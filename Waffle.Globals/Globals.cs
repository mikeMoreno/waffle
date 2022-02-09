namespace Waffle
{
    public static class Globals
    {
        public static string ApplicationName => "Waffle";

        public static string ApplicationVersion => "0.0.1";

        private static string applicationFolder;
        public static string ApplicationFolder
        {
            get
            {
                if(applicationFolder == null)
                {
                    var folder = Environment.SpecialFolder.LocalApplicationData;
                    var path = Environment.GetFolderPath(folder);

                    var appDirectory = Path.Join(path, ApplicationName);

                    applicationFolder = appDirectory;
                }

                return applicationFolder;
            }
        }

        public static string BookmarksFile
        {
            get
            {
                var bookmarkFile = Path.Combine(ApplicationFolder, "bookmarks.json");

                return bookmarkFile;
            }
        }

        public static string HistoryFolder
        {
            get
            {
                var historyFolder = Path.Combine(ApplicationFolder, "History");

                return historyFolder;
            }
        }
    }
}
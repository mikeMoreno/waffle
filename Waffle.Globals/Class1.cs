namespace Waffle
{
    public static class Globals
    {
        public static string ApplicationName => "Waffle";

        public static string ApplicationVersion => "0.0.1";

        public static string ApplicationFolder
        {
            get
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);

                var appDirectory = Path.Join(path, ApplicationName);

                return appDirectory;
            }
        }
    }
}
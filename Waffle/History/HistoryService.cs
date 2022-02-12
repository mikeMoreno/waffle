using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waffle.Lib;

namespace Waffle.History
{
    internal class HistoryService
    {
        public void AddUrl(Guid tabKey, SelectorLine selectorLine)
        {
            var history = LoadHistory(tabKey);

            history.Add(new HistoryEntity()
            {
                Timestamp = DateTime.Now,
                SelectorLine = selectorLine,
            });

            SaveHistory(history, tabKey);
        }

        public void Consolidate()
        {
            var allEntities = LoadHistory();

            var historyFiles = Directory.GetFiles(Globals.HistoryFolder);

            foreach (var historyfile in historyFiles)
            {
                if (Path.GetFileName(historyfile) == "all_history.json")
                {
                    continue;
                }

                var historyEntities = LoadHistory(historyfile);

                allEntities.AddRange(historyEntities);

                File.Delete(historyfile);
            }

            SaveHistory(allEntities);
        }

        public List<HistoryEntity> LoadHistory(Guid? tabKey = null)
        {
            var historyFileName = GetHistoryFileName(tabKey);

            var historyEntities = LoadHistory(historyFileName);

            return historyEntities.OrderByDescending(h => h.Timestamp).ToList();
        }

        private static List<HistoryEntity> LoadHistory(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new List<HistoryEntity>();
            }

            var historyEntities = JsonConvert.DeserializeObject<List<HistoryEntity>>(File.ReadAllText(fileName));

            if(historyEntities == null)
            {
                return new List<HistoryEntity>();
            }

            return historyEntities;
        }

        private void SaveHistory(List<HistoryEntity> historyEntities, Guid? tabKey = null)
        {
            var historyFileName = GetHistoryFileName(tabKey);

            var serializedBookmarks = JsonConvert.SerializeObject(historyEntities, Formatting.Indented);

            File.WriteAllText(historyFileName, serializedBookmarks);
        }

        private static string GetHistoryFileName(Guid? tabKey = null)
        {
            var historyFileName = tabKey == null ? Path.Combine(Globals.HistoryFolder, "all_history.json") :
                                       Path.Combine(Globals.HistoryFolder, $"{tabKey}_history.json");
            return historyFileName;
        }
    }
}

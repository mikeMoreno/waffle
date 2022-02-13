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
            var allHistory = LoadHistory();
            var tabHistory = LoadHistory(tabKey);

            var historyEntity = new HistoryEntity()
            {
                Key = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                SelectorLine = selectorLine,
            };

            allHistory.Add(historyEntity);
            tabHistory.Add(historyEntity);

            SaveHistory(tabHistory, tabKey);
            SaveHistory(allHistory);
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

            if (historyEntities == null)
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

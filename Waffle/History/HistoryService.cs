using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waffle.History
{
    internal class HistoryService
    {
        public void AddUrl(Guid tabKey, string url)
        {
            var history = LoadHistory(tabKey);

            history.Add(new HistoryEntity()
            {
                Timestamp = DateTime.Now,
                Url = url,
            });

            SaveHistory(tabKey, history);
        }

        private List<HistoryEntity> LoadHistory(Guid tabKey)
        {
            var historyFileName = Path.Combine(Globals.HistoryFolder, $"{tabKey}_history.json");

            if (!File.Exists(historyFileName))
            {
                return new List<HistoryEntity>();
            }

            var historyEntities = JsonConvert.DeserializeObject<List<HistoryEntity>>(File.ReadAllText(historyFileName));

            if (historyEntities == null)
            {
                return new List<HistoryEntity>();
            }

            return historyEntities;
        }

        private void SaveHistory(Guid tabKey, List<HistoryEntity> historyEntities)
        {
            var historyFileName = Path.Combine(Globals.HistoryFolder, $"{tabKey}_history.json");

            var serializedBookmarks = JsonConvert.SerializeObject(historyEntities, Formatting.Indented);

            File.WriteAllText(historyFileName, serializedBookmarks);
        }
    }
}

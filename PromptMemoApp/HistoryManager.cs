using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PromptMemoApp
{
    public class HistoryItem
    {
        public string Category { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastAccessed { get; set; }
        public int AccessCount { get; set; }
        public string LastContent { get; set; }

        public HistoryItem()
        {
            LastModified = DateTime.Now;
            LastAccessed = DateTime.Now;
            AccessCount = 0;
        }
    }

    public class HistoryManager
    {
        private string historyFilePath;
        private List<HistoryItem> history;
        private const int MaxHistoryItems = 100;

        public HistoryManager(string baseDirectory)
        {
            historyFilePath = Path.Combine(baseDirectory, "history.json");
            history = LoadHistory();
        }

        public List<HistoryItem> GetRecentHistory(int count = 20)
        {
            return history.OrderByDescending(h => h.LastAccessed).Take(count).ToList();
        }

        public List<HistoryItem> GetHistoryByCategory(string category)
        {
            return history.Where(h => h.Category == category)
                        .OrderByDescending(h => h.LastAccessed)
                        .ToList();
        }

        public void UpdateHistory(string category, string fileName, string filePath, string content)
        {
            var existing = history.FirstOrDefault(h => h.Category == category && h.FileName == fileName);
            
            if (existing != null)
            {
                existing.LastAccessed = DateTime.Now;
                existing.LastModified = DateTime.Now;
                existing.AccessCount++;
                existing.LastContent = content;
                existing.FilePath = filePath;
            }
            else
            {
                var historyItem = new HistoryItem
                {
                    Category = category,
                    FileName = fileName,
                    FilePath = filePath,
                    LastContent = content
                };
                history.Add(historyItem);
            }

            // 履歴の最大数を制限
            if (history.Count > MaxHistoryItems)
            {
                history = history.OrderByDescending(h => h.LastAccessed)
                               .Take(MaxHistoryItems)
                               .ToList();
            }

            SaveHistory();
        }

        public void RemoveFromHistory(string category, string fileName)
        {
            var item = history.FirstOrDefault(h => h.Category == category && h.FileName == fileName);
            if (item != null)
            {
                history.Remove(item);
                SaveHistory();
            }
        }

        public void ClearHistory()
        {
            history.Clear();
            SaveHistory();
        }

        private List<HistoryItem> LoadHistory()
        {
            if (File.Exists(historyFilePath))
            {
                try
                {
                    var json = File.ReadAllText(historyFilePath);
                    var result = JsonSerializer.Deserialize<List<HistoryItem>>(json);
            return result ?? new List<HistoryItem>();
                }
                catch
                {
                    return new List<HistoryItem>();
                }
            }
            return new List<HistoryItem>();
        }

        private void SaveHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(historyFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"履歴の保存に失敗しました: {ex.Message}", "エラー", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}

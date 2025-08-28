using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PromptMemoApp
{
    /// <summary>
    /// 履歴アイテムの情報を保持するクラス
    /// </summary>
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

    /// <summary>
    /// 履歴機能を管理するクラス
    /// </summary>
    public class HistoryManager
    {
        #region 定数・フィールド
        private const int DefaultMaxHistoryItems = 100;
        private const int DefaultRecentItemsCount = 20;

        private readonly string historyFilePath;
        private readonly int maxHistoryItems;
        private List<HistoryItem> history;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// HistoryManagerの新しいインスタンスを初期化
        /// </summary>
        /// <param name="baseDirectory">履歴ファイルを保存するディレクトリ</param>
        /// <param name="maxItems">保持する最大履歴件数（デフォルト：100件）</param>
        public HistoryManager(string baseDirectory, int maxItems = DefaultMaxHistoryItems)
        {
            historyFilePath = Path.Combine(baseDirectory, "history.json");
            maxHistoryItems = maxItems;
            history = LoadHistory();
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// 最近の履歴を取得
        /// </summary>
        /// <param name="count">取得する件数（デフォルト：20件）</param>
        /// <returns>最近の履歴アイテムのリスト</returns>
        public List<HistoryItem> GetRecentHistory(int count = DefaultRecentItemsCount)
        {
            return history
                .OrderByDescending(h => h.LastAccessed)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// 指定されたカテゴリの履歴を取得
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <returns>指定カテゴリの履歴アイテムのリスト</returns>
        public List<HistoryItem> GetHistoryByCategory(string category)
        {
            return history
                .Where(h => h.Category == category)
                .OrderByDescending(h => h.LastAccessed)
                .ToList();
        }

        /// <summary>
        /// 履歴を更新または追加
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="content">ファイル内容</param>
        public void UpdateHistory(string category, string fileName, string filePath, string content)
        {
            var existingItem = FindHistoryItem(category, fileName);

            if (existingItem != null)
            {
                UpdateExistingHistoryItem(existingItem, filePath, content);
            }
            else
            {
                AddNewHistoryItem(category, fileName, filePath, content);
            }

            LimitHistorySize();
            SaveHistory();
        }

        /// <summary>
        /// 指定されたアイテムを履歴から削除
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        public void RemoveFromHistory(string category, string fileName)
        {
            var item = FindHistoryItem(category, fileName);
            if (item == null)
                return; // 存在しない場合は何もしない

            history.Remove(item);
            SaveHistory();
        }

        /// <summary>
        /// すべての履歴をクリア
        /// </summary>
        public void ClearHistory()
        {
            history.Clear();
            SaveHistory();
        }

        /// <summary>
        /// 現在の履歴件数を取得
        /// </summary>
        /// <returns>履歴件数</returns>
        public int GetHistoryCount()
        {
            return history.Count;
        }
        #endregion

        #region プライベートメソッド
        /// <summary>
        /// 指定されたカテゴリとファイル名の履歴アイテムを検索
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>見つかった場合はHistoryItem、見つからない場合はnull</returns>
        private HistoryItem FindHistoryItem(string category, string fileName)
        {
            return history.FirstOrDefault(h =>
                h.Category == category && h.FileName == fileName);
        }

        /// <summary>
        /// 既存の履歴アイテムを更新
        /// </summary>
        /// <param name="item">更新対象のアイテム</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="content">ファイル内容</param>
        private void UpdateExistingHistoryItem(HistoryItem item, string filePath, string content)
        {
            item.LastAccessed = DateTime.Now;
            item.LastModified = DateTime.Now;
            item.AccessCount++;
            item.LastContent = content;
            item.FilePath = filePath;
        }

        /// <summary>
        /// 新しい履歴アイテムを追加
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="content">ファイル内容</param>
        private void AddNewHistoryItem(string category, string fileName, string filePath, string content)
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

        /// <summary>
        /// 履歴の件数を制限
        /// </summary>
        private void LimitHistorySize()
        {
            if (history.Count <= maxHistoryItems)
                return;

            history = history
                .OrderByDescending(h => h.LastAccessed)
                .Take(maxHistoryItems)
                .ToList();
        }

        /// <summary>
        /// 履歴データをファイルから読み込み
        /// </summary>
        /// <returns>履歴アイテムのリスト</returns>
        private List<HistoryItem> LoadHistory()
        {
            if (!File.Exists(historyFilePath))
                return new List<HistoryItem>();

            try
            {
                var json = File.ReadAllText(historyFilePath);
                var result = JsonSerializer.Deserialize<List<HistoryItem>>(json);
                return result ?? new List<HistoryItem>();
            }
            catch (Exception)
            {
                // JSONの読み込みに失敗した場合は空のリストを返す
                return new List<HistoryItem>();
            }
        }

        /// <summary>
        /// 履歴データをファイルに保存
        /// </summary>
        private void SaveHistory()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(history, options);
                File.WriteAllText(historyFilePath, json);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"履歴の保存に失敗しました: {ex.Message}");
            }
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowErrorMessage(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "エラー",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        }
        #endregion
    }
}
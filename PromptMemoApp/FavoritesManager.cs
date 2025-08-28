using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PromptMemoApp
{
    /// <summary>
    /// お気に入りアイテムの情報を保持するクラス
    /// </summary>
    public class FavoriteItem
    {
        public string Category { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime AddedDate { get; set; }
        public int AccessCount { get; set; }
        public DateTime LastAccessed { get; set; }

        public FavoriteItem()
        {
            AddedDate = DateTime.Now;
            LastAccessed = DateTime.Now;
            AccessCount = 0;
        }
    }

    /// <summary>
    /// お気に入り機能を管理するクラス
    /// </summary>
    public class FavoritesManager
    {
        #region フィールド
        private readonly string favoritesFilePath;
        private List<FavoriteItem> favorites;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// FavoritesManagerの新しいインスタンスを初期化
        /// </summary>
        /// <param name="baseDirectory">お気に入りファイルを保存するディレクトリ</param>
        public FavoritesManager(string baseDirectory)
        {
            favoritesFilePath = Path.Combine(baseDirectory, "favorites.json");
            favorites = LoadFavorites();
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// すべてのお気に入りを最終アクセス日時順で取得
        /// </summary>
        /// <returns>お気に入りアイテムのリスト</returns>
        public List<FavoriteItem> GetAllFavorites()
        {
            return favorites
                .OrderByDescending(f => f.LastAccessed)
                .ToList();
        }

        /// <summary>
        /// 指定されたカテゴリのお気に入りを取得
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <returns>指定カテゴリのお気に入りアイテムのリスト</returns>
        public List<FavoriteItem> GetFavoritesByCategory(string category)
        {
            return favorites
                .Where(f => f.Category == category)
                .OrderByDescending(f => f.LastAccessed)
                .ToList();
        }

        /// <summary>
        /// お気に入りに追加
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="filePath">ファイルパス</param>
        public void AddFavorite(string category, string fileName, string filePath)
        {
            if (IsFavorite(category, fileName))
                return; // 既に存在する場合は何もしない

            var favorite = new FavoriteItem
            {
                Category = category,
                FileName = fileName,
                FilePath = filePath
            };

            favorites.Add(favorite);
            SaveFavorites();
        }

        /// <summary>
        /// お気に入りから削除
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        public void RemoveFavorite(string category, string fileName)
        {
            var favorite = FindFavorite(category, fileName);
            if (favorite == null)
                return; // 存在しない場合は何もしない

            favorites.Remove(favorite);
            SaveFavorites();
        }

        /// <summary>
        /// アクセス回数を更新
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        public void UpdateAccessCount(string category, string fileName)
        {
            var favorite = FindFavorite(category, fileName);
            if (favorite == null)
                return; // 存在しない場合は何もしない

            favorite.AccessCount++;
            favorite.LastAccessed = DateTime.Now;
            SaveFavorites();
        }

        /// <summary>
        /// 指定されたファイルがお気に入りかどうかを判定
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>お気に入りの場合はtrue</returns>
        public bool IsFavorite(string category, string fileName)
        {
            return FindFavorite(category, fileName) != null;
        }
        #endregion

        #region プライベートメソッド
        /// <summary>
        /// 指定されたカテゴリとファイル名のお気に入りアイテムを検索
        /// </summary>
        /// <param name="category">カテゴリ名</param>
        /// <param name="fileName">ファイル名</param>
        /// <returns>見つかった場合はFavoriteItem、見つからない場合はnull</returns>
        private FavoriteItem FindFavorite(string category, string fileName)
        {
            return favorites.FirstOrDefault(f =>
                f.Category == category && f.FileName == fileName);
        }

        /// <summary>
        /// お気に入りデータをファイルから読み込み
        /// </summary>
        /// <returns>お気に入りアイテムのリスト</returns>
        private List<FavoriteItem> LoadFavorites()
        {
            if (!File.Exists(favoritesFilePath))
                return new List<FavoriteItem>();

            try
            {
                var json = File.ReadAllText(favoritesFilePath);
                var result = JsonSerializer.Deserialize<List<FavoriteItem>>(json);
                return result ?? new List<FavoriteItem>();
            }
            catch (Exception)
            {
                // JSONの読み込みに失敗した場合は空のリストを返す
                return new List<FavoriteItem>();
            }
        }

        /// <summary>
        /// お気に入りデータをファイルに保存
        /// </summary>
        private void SaveFavorites()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(favorites, options);
                File.WriteAllText(favoritesFilePath, json);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"お気に入りの保存に失敗しました: {ex.Message}");
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
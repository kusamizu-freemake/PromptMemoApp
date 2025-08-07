using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PromptMemoApp
{
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

    public class FavoritesManager
    {
        private string favoritesFilePath;
        private List<FavoriteItem> favorites;

        public FavoritesManager(string baseDirectory)
        {
            favoritesFilePath = Path.Combine(baseDirectory, "favorites.json");
            favorites = LoadFavorites();
        }

        public List<FavoriteItem> GetAllFavorites()
        {
            return favorites.OrderByDescending(f => f.LastAccessed).ToList();
        }

        public List<FavoriteItem> GetFavoritesByCategory(string category)
        {
            return favorites.Where(f => f.Category == category)
                          .OrderByDescending(f => f.LastAccessed)
                          .ToList();
        }

        public void AddFavorite(string category, string fileName, string filePath)
        {
            var existing = favorites.FirstOrDefault(f => f.Category == category && f.FileName == fileName);
            if (existing == null)
            {
                var favorite = new FavoriteItem
                {
                    Category = category,
                    FileName = fileName,
                    FilePath = filePath
                };
                favorites.Add(favorite);
                SaveFavorites();
            }
        }

        public void RemoveFavorite(string category, string fileName)
        {
            var favorite = favorites.FirstOrDefault(f => f.Category == category && f.FileName == fileName);
            if (favorite != null)
            {
                favorites.Remove(favorite);
                SaveFavorites();
            }
        }

        public void UpdateAccessCount(string category, string fileName)
        {
            var favorite = favorites.FirstOrDefault(f => f.Category == category && f.FileName == fileName);
            if (favorite != null)
            {
                favorite.AccessCount++;
                favorite.LastAccessed = DateTime.Now;
                SaveFavorites();
            }
        }

        public bool IsFavorite(string category, string fileName)
        {
            return favorites.Any(f => f.Category == category && f.FileName == fileName);
        }

        private List<FavoriteItem> LoadFavorites()
        {
            if (File.Exists(favoritesFilePath))
            {
                try
                {
                    var json = File.ReadAllText(favoritesFilePath);
                    var result = JsonSerializer.Deserialize<List<FavoriteItem>>(json);
            return result ?? new List<FavoriteItem>();
                }
                catch
                {
                    return new List<FavoriteItem>();
                }
            }
            return new List<FavoriteItem>();
        }

        private void SaveFavorites()
        {
            try
            {
                var json = JsonSerializer.Serialize(favorites, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(favoritesFilePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"お気に入りの保存に失敗しました: {ex.Message}", "エラー", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}

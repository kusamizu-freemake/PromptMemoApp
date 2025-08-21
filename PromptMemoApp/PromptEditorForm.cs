using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private string baseDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentCategory = "";
        private string currentFilePath = "";
        private Dictionary<Keys, Action> shortcuts = new Dictionary<Keys, Action>();
        private FavoritesManager favoritesManager;
        private HistoryManager historyManager;
        private TranslationManager translationManager;

        // 並び替え機能用のフィールド
        private SortOrder currentSortOrder = SortOrder.Ascending;
        private string currentSortField = "Name";
        private List<FileInfo> currentFileList = new List<FileInfo>();

        // 統計情報用
        private Dictionary<string, int> categoryStats = new Dictionary<string, int>();

        public PromptEditorForm()
        {
            InitializeComponent();
            InitializeData();
            InitializeShortcuts();
            InitializeManagers();
        }

        private void InitializeShortcuts()
        {
            // デフォルトのショートカットを設定
            shortcuts[Keys.Control | Keys.N] = () => btnNew_Click(null, null);
            shortcuts[Keys.Control | Keys.S] = () => btnSave_Click(null, null);
            shortcuts[Keys.Control | Keys.D] = () => btnDelete_Click(null, null);
            shortcuts[Keys.Control | Keys.R] = () => btnRename_Click(null, null);
            shortcuts[Keys.Control | Keys.M] = () => btnMove_Click(null, null);
            shortcuts[Keys.Control | Keys.T] = () => btnAddCategory_Click(null, null);
            shortcuts[Keys.F5] = () => LoadCategories();
            shortcuts[Keys.Control | Keys.F] = () => ShowSearchDialog();
            shortcuts[Keys.Control | Keys.O] = () => ShowFavoritesDialog();
            shortcuts[Keys.Control | Keys.H] = () => ShowHistoryDialog();
        }

        private void InitializeManagers()
        {
            favoritesManager = new FavoritesManager(baseDirectory);
            historyManager = new HistoryManager(baseDirectory);
            translationManager = new TranslationManager();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (shortcuts.ContainsKey(keyData))
            {
                shortcuts[keyData]();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeData()
        {
            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            LoadCategories();
        }

        private void LoadCategories()
        {
            treeViewCategories.Nodes.Clear();
            comboBoxCategories.Items.Clear();

            var dirs = Directory.GetDirectories(baseDirectory);
            foreach (var dir in dirs)
            {
                string category = Path.GetFileName(dir);
                TreeNode node = new TreeNode(category);
                treeViewCategories.Nodes.Add(node);
                comboBoxCategories.Items.Add(category);
            }

            if (treeViewCategories.Nodes.Count > 0)
                treeViewCategories.SelectedNode = treeViewCategories.Nodes[0];
        }

        // listBoxFiles関連の処理をlistViewFilesに置き換え
        // LoadFiles, SortAndDisplayFiles, listBoxFiles_SelectedIndexChanged, UpdateFavoriteButton などを修正

        private void LoadFiles(string category)
        {
            listViewFiles.Items.Clear();
            currentFileList.Clear();
            string dirPath = Path.Combine(baseDirectory, category);
            if (!Directory.Exists(dirPath)) return;

            var files = Directory.GetFiles(dirPath, "*.txt");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                currentFileList.Add(fileInfo);
            }
            SortAndDisplayFiles();
        }

        private void SortAndDisplayFiles()
        {
            var sortedFiles = currentFileList.AsEnumerable();
            switch (currentSortField)
            {
                case "Name":
                    sortedFiles = currentSortOrder == SortOrder.Ascending ? sortedFiles.OrderBy(f => f.Name) : sortedFiles.OrderByDescending(f => f.Name);
                    break;
                case "Created":
                    sortedFiles = currentSortOrder == SortOrder.Ascending ? sortedFiles.OrderBy(f => f.CreationTime) : sortedFiles.OrderByDescending(f => f.CreationTime);
                    break;
                case "Modified":
                    sortedFiles = currentSortOrder == SortOrder.Ascending ? sortedFiles.OrderBy(f => f.LastWriteTime) : sortedFiles.OrderByDescending(f => f.LastWriteTime);
                    break;
                case "Size":
                    sortedFiles = currentSortOrder == SortOrder.Ascending ? sortedFiles.OrderBy(f => f.Length) : sortedFiles.OrderByDescending(f => f.Length);
                    break;
            }
            listViewFiles.Items.Clear();
            foreach (var file in sortedFiles)
            {
                var item = new ListViewItem(new string[] {
                    Path.GetFileNameWithoutExtension(file.Name),
                    file.CreationTime.ToString("yyyy/MM/dd HH:mm"),
                    file.LastWriteTime.ToString("yyyy/MM/dd HH:mm"),
                    file.Length.ToString()
                });
                listViewFiles.Items.Add(item);
            }
        }

        public void SortFiles(string sortField)
        {
            if (currentSortField == sortField)
            {
                // 同じフィールドでソートする場合は順序を反転
                currentSortOrder = currentSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                currentSortField = sortField;
                currentSortOrder = SortOrder.Ascending;
            }

            SortAndDisplayFiles();
        }

        // エクスポート機能
        public void ExportData(string exportPath)
        {
            try
            {
                var exportData = new ExportData
                {
                    Categories = new List<CategoryData>()
                };

                var categories = Directory.GetDirectories(baseDirectory);
                foreach (var categoryPath in categories)
                {
                    var categoryName = Path.GetFileName(categoryPath);
                    var categoryData = new CategoryData
                    {
                        Name = categoryName,
                        Files = new List<FileData>()
                    };

                    var files = Directory.GetFiles(categoryPath, "*.txt");
                    foreach (var filePath in files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);
                        var content = File.ReadAllText(filePath);
                        var fileInfo = new FileInfo(filePath);

                        categoryData.Files.Add(new FileData
                        {
                            Name = fileName,
                            Content = content,
                            Created = fileInfo.CreationTime,
                            Modified = fileInfo.LastWriteTime
                        });
                    }

                    exportData.Categories.Add(categoryData);
                }

                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(exportPath, json);

                MessageBox.Show("データのエクスポートが完了しました。", "エクスポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エクスポート中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // インポート機能
        public void ImportData(string importPath)
        {
            try
            {
                var json = File.ReadAllText(importPath);
                var importData = JsonSerializer.Deserialize<ExportData>(json);

                if (importData?.Categories == null)
                {
                    MessageBox.Show("インポートファイルの形式が正しくありません。", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var categoryData in importData.Categories)
                {
                    var categoryPath = Path.Combine(baseDirectory, categoryData.Name);
                    if (!Directory.Exists(categoryPath))
                    {
                        Directory.CreateDirectory(categoryPath);
                    }

                    foreach (var fileData in categoryData.Files)
                    {
                        var filePath = Path.Combine(categoryPath, fileData.Name + ".txt");
                        File.WriteAllText(filePath, fileData.Content);

                        // ファイルの作成日時と更新日時を設定
                        File.SetCreationTime(filePath, fileData.Created);
                        File.SetLastWriteTime(filePath, fileData.Modified);
                    }
                }

                LoadCategories();
                MessageBox.Show("データのインポートが完了しました。", "インポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"インポート中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 統計情報の取得
        public Dictionary<string, object> GetStatistics()
        {
            var stats = new Dictionary<string, object>();

            try
            {
                var categories = Directory.GetDirectories(baseDirectory);
                int totalFiles = 0;
                long totalSize = 0;
                var categoryCounts = new Dictionary<string, int>();

                foreach (var categoryPath in categories)
                {
                    var categoryName = Path.GetFileName(categoryPath);
                    var files = Directory.GetFiles(categoryPath, "*.txt");
                    var fileCount = files.Length;

                    categoryCounts[categoryName] = fileCount;
                    totalFiles += fileCount;

                    foreach (var filePath in files)
                    {
                        var fileInfo = new FileInfo(filePath);
                        totalSize += fileInfo.Length;
                    }
                }

                stats["TotalCategories"] = categories.Length;
                stats["TotalFiles"] = totalFiles;
                stats["TotalSize"] = totalSize;
                stats["CategoryCounts"] = categoryCounts;
                stats["AverageFilesPerCategory"] = categories.Length > 0 ? (double)totalFiles / categories.Length : 0;
            }
            catch (Exception ex)
            {
                stats["Error"] = ex.Message;
            }

            return stats;
        }

        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentCategory = e.Node.Text;
            LoadFiles(currentCategory);
            txtEditor.Clear();
            currentFilePath = "";
        }

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            string[] fields = { "Name", "Created", "Modified", "Size" };
            string sortField = fields[e.Column];
            SortFiles(sortField);
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;
            string fileName = listViewFiles.SelectedItems[0].Text + ".txt";
            string filePath = Path.Combine(baseDirectory, currentCategory, fileName);
            currentFilePath = filePath;
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                txtEditor.Text = content;
                // 履歴を更新
                historyManager.UpdateHistory(currentCategory, listViewFiles.SelectedItems[0].Text, filePath, content);
                // お気に入り状態を更新
                UpdateFavoriteButton();
            }
        }

        private void UpdateFavoriteButton()
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                var fileName = listViewFiles.SelectedItems[0].Text;
                var isFavorite = favoritesManager.IsFavorite(currentCategory, fileName);
                btnFavorite.Text = isFavorite ? "★" : "☆";
                btnFavorite.ForeColor = isFavorite ? Color.Red : Color.Black;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("ファイルが選択されていません。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            File.WriteAllText(currentFilePath, txtEditor.Text);
            MessageBox.Show("保存しました。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFavorite_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("ファイルが選択されていません。", "お気に入り", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fileName = listViewFiles.SelectedItems[0].Text;
            var isFavorite = favoritesManager.IsFavorite(currentCategory, fileName);

            if (isFavorite)
            {
                favoritesManager.RemoveFavorite(currentCategory, fileName);
                btnFavorite.Text = "★";
                MessageBox.Show("お気に入りから削除しました。", "お気に入り", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                favoritesManager.AddFavorite(currentCategory, fileName, currentFilePath);
                btnFavorite.Text = "★";
                btnFavorite.ForeColor = Color.Red;
                MessageBox.Show("お気に入りに追加しました。", "お気に入り", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEditor.Text))
            {
                MessageBox.Show("翻訳するテキストがありません。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new TranslationDialog(translationManager, txtEditor.Text))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 元言語・翻訳後言語を取得
                    string sourceLang = dialog.GetSourceLanguage(); // "JA" or "EN"
                    string targetLang = dialog.GetTargetLanguage(); // "JA" or "EN"
                    if (sourceLang == targetLang)
                    {
                        MessageBox.Show("元言語と翻訳後の言語が同じです。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    // 非同期で翻訳
                    string translated = await translationManager.TranslateAsync(txtEditor.Text, sourceLang, targetLang);
                    txtEditor.Text = translated;
                }
            }
        }
            
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentCategory))
            {
                MessageBox.Show("カテゴリを選択してください。", "新規作成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new InputDialog("新規ファイル作成", "ファイル名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string name = dialog.InputText;
                    string path = Path.Combine(baseDirectory, currentCategory, name + ".txt");

                    if (File.Exists(path))
                    {
                        MessageBox.Show("同名のファイルが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.WriteAllText(path, "");
                    LoadFiles(currentCategory);
                }
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;

            string oldName = listViewFiles.SelectedItems[0].Text;
            using (var dialog = new InputDialog("ファイル名変更", "新しいファイル名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText;
                    string oldPath = Path.Combine(baseDirectory, currentCategory, oldName + ".txt");
                    string newPath = Path.Combine(baseDirectory, currentCategory, newName + ".txt");

                    if (File.Exists(newPath))
                    {
                        MessageBox.Show("同名のファイルが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.Move(oldPath, newPath);
                    LoadFiles(currentCategory);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var items = listViewFiles.SelectedItems.Cast<ListViewItem>().ToList();
            if (items.Count == 0) return;

            if (MessageBox.Show("選択したファイルを削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var item in items)
                {
                    string path = Path.Combine(baseDirectory, currentCategory, item.Text + ".txt");
                    if (File.Exists(path))
                        File.Delete(path);
                }
                LoadFiles(currentCategory);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            var items = listViewFiles.SelectedItems.Cast<ListViewItem>().ToList();
            if (items.Count == 0 || comboBoxCategories.SelectedItem == null) return;

            string destCategory = comboBoxCategories.SelectedItem.ToString();
            foreach (var item in items)
            {
                string srcPath = Path.Combine(baseDirectory, currentCategory, item.Text + ".txt");
                string destPath = Path.Combine(baseDirectory, destCategory, item.Text + ".txt");
                if (File.Exists(srcPath))
                    File.Move(srcPath, destPath);
            }
            LoadFiles(currentCategory);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputDialog("カテゴリ作成", "カテゴリ名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string name = dialog.InputText;
                    string path = Path.Combine(baseDirectory, name);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void renameCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string oldName = treeViewCategories.SelectedNode.Text;
            using (var dialog = new InputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText;
                    string oldPath = Path.Combine(baseDirectory, oldName);
                    string newPath = Path.Combine(baseDirectory, newName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.Move(oldPath, newPath);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void deleteCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string category = treeViewCategories.SelectedNode.Text;
            string path = Path.Combine(baseDirectory, category);
            if (MessageBox.Show("カテゴリを削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(path, true);
                LoadCategories();
                listViewFiles.Items.Clear();
                txtEditor.Clear();
            }
        }

        private void menuExit_Click(object sender, EventArgs e) => this.Close();
        private void menuNew_Click(object sender, EventArgs e) => btnNew_Click(sender, e);
        private void menuFavorites_Click(object sender, EventArgs e) => ShowFavoritesDialog();
        private void menuHistory_Click(object sender, EventArgs e) => ShowHistoryDialog();
        private void menuSearch_Click(object sender, EventArgs e) => ShowSearchDialog();
        private void menuShortcuts_Click(object sender, EventArgs e) => ShowShortcutSettings();
        private void menuSort_Click(object sender, EventArgs e) => ShowSortSettings();
        private void menuStatistics_Click(object sender, EventArgs e) => ShowStatistics();
        private void menuExportImport_Click(object sender, EventArgs e) => ShowExportImportDialog();

        private void ShowFavoritesDialog()
        {
            using (var dialog = new FavoritesDialog(favoritesManager, OnFavoriteSelected))
            {
                dialog.ShowDialog();
            }
        }

        private void ShowHistoryDialog()
        {
            using (var dialog = new HistoryDialog(historyManager, OnHistorySelected))
            {
                dialog.ShowDialog();
            }
        }

        private void ShowSearchDialog()
        {
            using (var dialog = new SearchDialog(baseDirectory, OnSearchResultSelected))
            {
                dialog.ShowDialog();
            }
        }

        private void OnFavoriteSelected(string category, string fileName)
        {
            // カテゴリを選択
            var categoryNode = treeViewCategories.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == category);
            if (categoryNode != null)
            {
                treeViewCategories.SelectedNode = categoryNode;
                // ファイルを選択
                var item = listViewFiles.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Text == fileName);
                if (item != null)
                {
                    item.Selected = true;
                    item.Focused = true;
                    listViewFiles.Select();
                }
            }
        }

        private void OnHistorySelected(string category, string fileName)
        {
            OnFavoriteSelected(category, fileName);
        }

        private void OnSearchResultSelected(string category, string fileName)
        {
            OnFavoriteSelected(category, fileName);
        }

        private void ShowShortcutSettings()
        {
            var shortcutNames = new Dictionary<string, Keys>
            {
                { "新規作成", Keys.Control | Keys.N },
                { "保存", Keys.Control | Keys.S },
                { "削除", Keys.Control | Keys.D },
                { "名前変更", Keys.Control | Keys.R },
                { "移動", Keys.Control | Keys.M },
                { "カテゴリ作成", Keys.Control | Keys.T },
                { "更新", Keys.F5 },
                { "検索", Keys.Control | Keys.F },
                { "お気に入り", Keys.Control | Keys.O },
                { "履歴", Keys.Control | Keys.H }
            };

            using (var dialog = new ShortcutSettingsForm(shortcutNames))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // ショートカット設定を更新
                    var newShortcuts = dialog.GetShortcuts();
                    UpdateShortcuts(newShortcuts);
                }
            }
        }

        private void ShowSortSettings()
        {
            using (var dialog = new SortSettingsDialog(this))
            {
                dialog.ShowDialog();
            }
        }

        private void ShowStatistics()
        {
            var stats = GetStatistics();
            using (var dialog = new StatisticsDialog(stats))
            {
                dialog.ShowDialog();
            }
        }

        private void ShowExportImportDialog()
        {
            using (var dialog = new ExportImportDialog(this))
            {
                dialog.ShowDialog();
            }
        }

        private void SearchInFiles(string searchText)
        {
            var results = new List<string>();
            foreach (var category in Directory.GetDirectories(baseDirectory))
            {
                var categoryName = Path.GetFileName(category);
                foreach (var file in Directory.GetFiles(category, "*.txt"))
                {
                    var content = File.ReadAllText(file);
                    if (content.Contains(searchText))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        results.Add($"{categoryName}\\{fileName}");
                    }
                }
            }

            if (results.Count > 0)
            {
                var resultText = string.Join("\n", results);
                MessageBox.Show($"検索結果:\n\n{resultText}", "検索結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("検索結果が見つかりませんでした。", "検索結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateShortcuts(Dictionary<string, Keys> newShortcuts)
        {
            shortcuts.Clear();
            foreach (var shortcut in newShortcuts)
            {
                switch (shortcut.Key)
                {
                    case "新規作成":
                        shortcuts[shortcut.Value] = () => btnNew_Click(null, null);
                        break;
                    case "保存":
                        shortcuts[shortcut.Value] = () => btnSave_Click(null, null);
                        break;
                    case "削除":
                        shortcuts[shortcut.Value] = () => btnDelete_Click(null, null);
                        break;
                    case "名前変更":
                        shortcuts[shortcut.Value] = () => btnRename_Click(null, null);
                        break;
                    case "移動":
                        shortcuts[shortcut.Value] = () => btnMove_Click(null, null);
                        break;
                    case "カテゴリ作成":
                        shortcuts[shortcut.Value] = () => btnAddCategory_Click(null, null);
                        break;
                    case "更新":
                        shortcuts[shortcut.Value] = () => LoadCategories();
                        break;
                    case "検索":
                        shortcuts[shortcut.Value] = () => ShowSearchDialog();
                        break;
                    case "お気に入り":
                        shortcuts[shortcut.Value] = () => ShowFavoritesDialog();
                        break;
                    case "履歴":
                        shortcuts[shortcut.Value] = () => ShowHistoryDialog();
                        break;
                }
            }
        }

        private void PromptEditorForm_Load(object sender, EventArgs e)
        {

        }

        private async Task<string> TranslateTextAsync(string text, string fromLang, string toLang)
        {
            // ここに本物のAPI呼び出しを実装してください
            // 例: Google翻訳API, DeepL, Azure Translator など
            // 今はダミーで「[翻訳]」を付けて返します
            await Task.Delay(500); // 疑似的な非同期処理
            if (string.IsNullOrWhiteSpace(text)) return "";
            if (fromLang == toLang) return text;
            return $"[翻訳]{text}";
        }
    }

    // エクスポート/インポート用のデータクラス
    public class ExportData
    {
        public List<CategoryData> Categories { get; set; }
        public DateTime ExportDate { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0";
    }

    public class CategoryData
    {
        public string Name { get; set; }
        public List<FileData> Files { get; set; }
    }

    public class FileData
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}

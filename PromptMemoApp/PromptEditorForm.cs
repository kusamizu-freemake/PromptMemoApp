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
        #region フィールド
        private readonly string baseDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentCategory = "";
        private string currentFilePath = "";

        // 各種マネージャー - インターフェースを使用
        private FavoritesManager favoritesManager;
        private HistoryManager historyManager;
        private TranslationManager translationManager;

        // ショートカット機能
        private Dictionary<Keys, Action> shortcuts = new Dictionary<Keys, Action>();

        // 並び替え機能
        private SortOrder currentSortOrder = SortOrder.Ascending;
        private string currentSortField = "Name";
        private List<FileInfo> currentFileList = new List<FileInfo>();
        #endregion

        #region 初期化
        public PromptEditorForm()
        {
            InitializeComponent();
            InitializeApplication();
        }

        /// <summary>
        /// アプリケーション全体の初期化を実行
        /// </summary>
        private void InitializeApplication()
        {
            CreateDirectoryIfNotExists();
            InitializeManagers();
            InitializeShortcuts();
            LoadCategories();
        }

        /// <summary>
        /// ベースディレクトリが存在しない場合は作成
        /// </summary>
        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);
        }

        /// <summary>
        /// 各種マネージャーを初期化
        /// </summary>
        private void InitializeManagers()
        {
            // FavoritesManagerをIFavoritesManagerとして初期化
            favoritesManager = new FavoritesManager(baseDirectory);
            historyManager = new HistoryManager(baseDirectory);
            translationManager = new TranslationManager();
        }

        /// <summary>
        /// キーボードショートカットを設定
        /// </summary>
        private void InitializeShortcuts()
        {
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (shortcuts.ContainsKey(keyData))
            {
                shortcuts[keyData]();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region カテゴリ管理
        /// <summary>
        /// すべてのカテゴリを読み込んでUIに表示
        /// </summary>
        private void LoadCategories()
        {
            treeViewCategories.Nodes.Clear();
            comboBoxCategories.Items.Clear();

            var directories = Directory.GetDirectories(baseDirectory);
            foreach (var directory in directories)
            {
                string categoryName = Path.GetFileName(directory);
                AddCategoryToUI(categoryName);
            }

            SelectFirstCategoryIfExists();
        }

        /// <summary>
        /// カテゴリをUIに追加
        /// </summary>
        private void AddCategoryToUI(string categoryName)
        {
            TreeNode node = new TreeNode(categoryName);
            treeViewCategories.Nodes.Add(node);
            comboBoxCategories.Items.Add(categoryName);
        }

        /// <summary>
        /// 最初のカテゴリが存在する場合は選択
        /// </summary>
        private void SelectFirstCategoryIfExists()
        {
            if (treeViewCategories.Nodes.Count > 0)
                treeViewCategories.SelectedNode = treeViewCategories.Nodes[0];
        }
        #endregion

        #region ファイル管理
        /// <summary>
        /// 指定されたカテゴリのファイル一覧を読み込み
        /// </summary>
        private void LoadFiles(string category)
        {
            listViewFiles.Items.Clear();
            currentFileList.Clear();

            string directoryPath = Path.Combine(baseDirectory, category);
            if (!Directory.Exists(directoryPath)) return;

            var textFiles = Directory.GetFiles(directoryPath, "*.txt");
            foreach (var file in textFiles)
            {
                currentFileList.Add(new FileInfo(file));
            }

            SortAndDisplayFiles();
        }

        /// <summary>
        /// ファイル一覧をソートしてUIに表示
        /// </summary>
        private void SortAndDisplayFiles()
        {
            var sortedFiles = GetSortedFiles();
            DisplayFilesInListView(sortedFiles);
        }

        /// <summary>
        /// 現在の設定に基づいてファイルをソート
        /// </summary>
        private IEnumerable<FileInfo> GetSortedFiles()
        {
            var sortedFiles = currentFileList.AsEnumerable();

            switch (currentSortField)
            {
                case "Name":
                    sortedFiles = currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.Name)
                        : sortedFiles.OrderByDescending(f => f.Name);
                    break;
                case "Created":
                    sortedFiles = currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.CreationTime)
                        : sortedFiles.OrderByDescending(f => f.CreationTime);
                    break;
                case "Modified":
                    sortedFiles = currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.LastWriteTime)
                        : sortedFiles.OrderByDescending(f => f.LastWriteTime);
                    break;
                case "Size":
                    sortedFiles = currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.Length)
                        : sortedFiles.OrderByDescending(f => f.Length);
                    break;
            }

            return sortedFiles;
        }

        /// <summary>
        /// ファイル一覧をListViewに表示
        /// </summary>
        private void DisplayFilesInListView(IEnumerable<FileInfo> files)
        {
            listViewFiles.Items.Clear();
            foreach (var file in files)
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

        /// <summary>
        /// ファイルのソート順を変更
        /// </summary>
        public void SortFiles(string sortField)
        {
            if (currentSortField == sortField)
            {
                // 同じフィールドの場合は昇順・降順を切り替え
                currentSortOrder = currentSortOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                currentSortField = sortField;
                currentSortOrder = SortOrder.Ascending;
            }

            SortAndDisplayFiles();
        }
        #endregion

        #region エクスポート・インポート
        /// <summary>
        /// データをJSONファイルにエクスポート
        /// </summary>
        public void ExportData(string exportPath)
        {
            try
            {
                var exportData = CreateExportData();
                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(exportPath, json);

                MessageBox.Show("データのエクスポートが完了しました。", "エクスポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("エクスポート中にエラーが発生しました", ex.Message);
            }
        }

        /// <summary>
        /// エクスポート用のデータオブジェクトを作成
        /// </summary>
        private ExportData CreateExportData()
        {
            var exportData = new ExportData
            {
                Categories = new List<CategoryData>()
            };

            var categories = Directory.GetDirectories(baseDirectory);
            foreach (var categoryPath in categories)
            {
                var categoryData = CreateCategoryData(categoryPath);
                exportData.Categories.Add(categoryData);
            }

            return exportData;
        }

        /// <summary>
        /// カテゴリデータを作成
        /// </summary>
        private CategoryData CreateCategoryData(string categoryPath)
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
                var fileData = CreateFileData(filePath);
                categoryData.Files.Add(fileData);
            }

            return categoryData;
        }

        /// <summary>
        /// ファイルデータを作成
        /// </summary>
        private FileData CreateFileData(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = File.ReadAllText(filePath);
            var fileInfo = new FileInfo(filePath);

            return new FileData
            {
                Name = fileName,
                Content = content,
                Created = fileInfo.CreationTime,
                Modified = fileInfo.LastWriteTime
            };
        }

        /// <summary>
        /// JSONファイルからデータをインポート
        /// </summary>
        public void ImportData(string importPath)
        {
            try
            {
                var json = File.ReadAllText(importPath);
                var importData = JsonSerializer.Deserialize<ExportData>(json);

                if (!IsValidImportData(importData))
                {
                    MessageBox.Show("インポートファイルの形式が正しくありません。", "エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ImportCategories(importData.Categories);
                LoadCategories();

                MessageBox.Show("データのインポートが完了しました。", "インポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("インポート中にエラーが発生しました", ex.Message);
            }
        }

        /// <summary>
        /// インポートデータの妥当性チェック
        /// </summary>
        private bool IsValidImportData(ExportData importData)
        {
            return importData?.Categories != null;
        }

        /// <summary>
        /// カテゴリデータをインポート
        /// </summary>
        private void ImportCategories(List<CategoryData> categories)
        {
            foreach (var categoryData in categories)
            {
                ImportSingleCategory(categoryData);
            }
        }

        /// <summary>
        /// 単一カテゴリをインポート
        /// </summary>
        private void ImportSingleCategory(CategoryData categoryData)
        {
            var categoryPath = Path.Combine(baseDirectory, categoryData.Name);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            foreach (var fileData in categoryData.Files)
            {
                ImportSingleFile(categoryPath, fileData);
            }
        }

        /// <summary>
        /// 単一ファイルをインポート
        /// </summary>
        private void ImportSingleFile(string categoryPath, FileData fileData)
        {
            var filePath = Path.Combine(categoryPath, fileData.Name + ".txt");
            File.WriteAllText(filePath, fileData.Content);

            // ファイルの作成日時と更新日時を設定
            File.SetCreationTime(filePath, fileData.Created);
            File.SetLastWriteTime(filePath, fileData.Modified);
        }
        #endregion

        #region 統計情報
        /// <summary>
        /// アプリケーションの統計情報を取得
        /// </summary>
        public Dictionary<string, object> GetStatistics()
        {
            var stats = new Dictionary<string, object>();

            try
            {
                CalculateStatistics(stats);
            }
            catch (Exception ex)
            {
                stats["Error"] = ex.Message;
            }

            return stats;
        }

        /// <summary>
        /// 統計情報を計算
        /// </summary>
        private void CalculateStatistics(Dictionary<string, object> stats)
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

                totalSize += files.Sum(filePath => new FileInfo(filePath).Length);
            }

            stats["TotalCategories"] = categories.Length;
            stats["TotalFiles"] = totalFiles;
            stats["TotalSize"] = totalSize;
            stats["CategoryCounts"] = categoryCounts;
            stats["AverageFilesPerCategory"] = categories.Length > 0 ? (double)totalFiles / categories.Length : 0;
        }
        #endregion

        #region イベントハンドラー
        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentCategory = e.Node.Text;
            LoadFiles(currentCategory);
            ClearEditor();
        }

        private void ClearEditor()
        {
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

            var selectedItem = listViewFiles.SelectedItems[0];
            LoadSelectedFile(selectedItem);
        }

        /// <summary>
        /// 選択されたファイルを読み込み
        /// </summary>
        private void LoadSelectedFile(ListViewItem selectedItem)
        {
            string fileName = selectedItem.Text + ".txt";
            string filePath = Path.Combine(baseDirectory, currentCategory, fileName);
            currentFilePath = filePath;

            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                txtEditor.Text = content;

                // 履歴とお気に入り状態を更新
                historyManager.UpdateHistory(currentCategory, selectedItem.Text, filePath, content);
                UpdateFavoriteButton();
            }
        }

        /// <summary>
        /// お気に入りボタンの表示を更新
        /// </summary>
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
        #endregion

        #region ボタンイベント
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

            ToggleFavorite();
        }

        /// <summary>
        /// お気に入り状態を切り替え
        /// </summary>
        private void ToggleFavorite()
        {
            var fileName = listViewFiles.SelectedItems[0].Text;
            var isFavorite = favoritesManager.IsFavorite(currentCategory, fileName);

            if (isFavorite)
            {
                favoritesManager.RemoveFavorite(currentCategory, fileName);
                MessageBox.Show("お気に入りから削除しました。", "お気に入り", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                favoritesManager.AddFavorite(currentCategory, fileName, currentFilePath);
                MessageBox.Show("お気に入りに追加しました。", "お気に入り", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            UpdateFavoriteButton();
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEditor.Text))
            {
                MessageBox.Show("翻訳するテキストがありません。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await PerformTranslation();
        }

        /// <summary>
        /// 翻訳処理を実行
        /// </summary>
        private async Task PerformTranslation()
        {
            using (var dialog = new TranslationDialog(translationManager, txtEditor.Text))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string sourceLang = dialog.GetSourceLanguage();
                string targetLang = dialog.GetTargetLanguage();

                if (sourceLang == targetLang)
                {
                    MessageBox.Show("元言語と翻訳後の言語が同じです。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string translated = await translationManager.TranslateAsync(txtEditor.Text, sourceLang, targetLang);
                txtEditor.Text = translated;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentCategory))
            {
                MessageBox.Show("カテゴリを選択してください。", "新規作成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CreateNewFile();
        }

        /// <summary>
        /// 新しいファイルを作成
        /// </summary>
        private void CreateNewFile()
        {
            using (var dialog = new InputDialog("新規ファイル作成", "ファイル名を入力してください:", ""))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string fileName = dialog.InputText;
                string filePath = Path.Combine(baseDirectory, currentCategory, fileName + ".txt");

                if (File.Exists(filePath))
                {
                    MessageBox.Show("同名のファイルが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                File.WriteAllText(filePath, "");
                LoadFiles(currentCategory);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;
            RenameSelectedFile();
        }

        /// <summary>
        /// 選択されたファイルの名前を変更
        /// </summary>
        private void RenameSelectedFile()
        {
            string oldName = listViewFiles.SelectedItems[0].Text;
            using (var dialog = new InputDialog("ファイル名変更", "新しいファイル名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedItems = listViewFiles.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectedItems.Count == 0) return;

            if (ConfirmDeletion())
            {
                DeleteSelectedFiles(selectedItems);
                LoadFiles(currentCategory);
            }
        }

        /// <summary>
        /// 削除の確認
        /// </summary>
        private bool ConfirmDeletion()
        {
            return MessageBox.Show("選択したファイルを削除しますか？", "確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// 選択されたファイルを削除
        /// </summary>
        private void DeleteSelectedFiles(List<ListViewItem> items)
        {
            foreach (var item in items)
            {
                string filePath = Path.Combine(baseDirectory, currentCategory, item.Text + ".txt");
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            var selectedItems = listViewFiles.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectedItems.Count == 0 || comboBoxCategories.SelectedItem == null) return;

            MoveSelectedFiles(selectedItems);
            LoadFiles(currentCategory);
        }

        /// <summary>
        /// 選択されたファイルを移動
        /// </summary>
        private void MoveSelectedFiles(List<ListViewItem> items)
        {
            string destCategory = comboBoxCategories.SelectedItem.ToString();
            foreach (var item in items)
            {
                string srcPath = Path.Combine(baseDirectory, currentCategory, item.Text + ".txt");
                string destPath = Path.Combine(baseDirectory, destCategory, item.Text + ".txt");
                if (File.Exists(srcPath))
                    File.Move(srcPath, destPath);
            }
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        /// <summary>
        /// 新しいカテゴリを作成
        /// </summary>
        private void CreateNewCategory()
        {
            using (var dialog = new InputDialog("カテゴリ作成", "カテゴリ名を入力してください:", ""))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string categoryName = dialog.InputText;
                string categoryPath = Path.Combine(baseDirectory, categoryName);

                if (Directory.Exists(categoryPath))
                {
                    MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Directory.CreateDirectory(categoryPath);
                LoadCategories();
            }
        }
        #endregion

        #region メニューイベント
        private void renameCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;
            RenameCategoryDialog();
        }

        /// <summary>
        /// カテゴリ名変更ダイアログを表示
        /// </summary>
        private void RenameCategoryDialog()
        {
            string oldName = treeViewCategories.SelectedNode.Text;
            using (var dialog = new InputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string newName = dialog.InputText;
                string oldPath = Path.Combine(baseDirectory, oldName);
                string newPath = Path.Combine(baseDirectory, newName);

                if (Directory.Exists(newPath))
                {
                    MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Directory.Move(oldPath, newPath);
                LoadCategories();
            }
        }

        private void deleteCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            if (ConfirmCategoryDeletion())
            {
                DeleteSelectedCategory();
            }
        }

        /// <summary>
        /// カテゴリ削除の確認
        /// </summary>
        private bool ConfirmCategoryDeletion()
        {
            return MessageBox.Show("カテゴリを削除しますか？", "確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        /// <summary>
        /// 選択されたカテゴリを削除
        /// </summary>
        private void DeleteSelectedCategory()
        {
            string category = treeViewCategories.SelectedNode.Text;
            string categoryPath = Path.Combine(baseDirectory, category);

            Directory.Delete(categoryPath, true);
            LoadCategories();
            listViewFiles.Items.Clear();
            txtEditor.Clear();
        }

        // メニューイベントハンドラー（簡潔に）
        private void menuExit_Click(object sender, EventArgs e) => this.Close();
        private void menuNew_Click(object sender, EventArgs e) => btnNew_Click(sender, e);
        private void menuFavorites_Click(object sender, EventArgs e) => ShowFavoritesDialog();
        private void menuHistory_Click(object sender, EventArgs e) => ShowHistoryDialog();
        private void menuSearch_Click(object sender, EventArgs e) => ShowSearchDialog();
        private void menuShortcuts_Click(object sender, EventArgs e) => ShowShortcutSettings();
        private void menuSort_Click(object sender, EventArgs e) => ShowSortSettings();
        private void menuStatistics_Click(object sender, EventArgs e) => ShowStatistics();
        private void menuExportImport_Click(object sender, EventArgs e) => ShowExportImportDialog();
        #endregion

        #region ダイアログ表示
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
        #endregion

        #region ファイル選択・コールバック
        /// <summary>
        /// お気に入りから選択されたファイルを開く
        /// </summary>
        private void OnFavoriteSelected(string category, string fileName)
        {
            SelectFileInCategory(category, fileName);
        }

        /// <summary>
        /// 履歴から選択されたファイルを開く
        /// </summary>
        private void OnHistorySelected(string category, string fileName)
        {
            SelectFileInCategory(category, fileName);
        }

        /// <summary>
        /// 検索結果から選択されたファイルを開く
        /// </summary>
        private void OnSearchResultSelected(string category, string fileName)
        {
            SelectFileInCategory(category, fileName);
        }

        /// <summary>
        /// 指定されたカテゴリとファイル名のファイルを選択
        /// </summary>
        private void SelectFileInCategory(string category, string fileName)
        {
            // カテゴリを選択
            var categoryNode = treeViewCategories.Nodes.Cast<TreeNode>()
                .FirstOrDefault(n => n.Text == category);
            if (categoryNode != null)
            {
                treeViewCategories.SelectedNode = categoryNode;

                // ファイルを選択
                var item = listViewFiles.Items.Cast<ListViewItem>()
                    .FirstOrDefault(i => i.Text == fileName);
                if (item != null)
                {
                    item.Selected = true;
                    item.Focused = true;
                    listViewFiles.Select();
                }
            }
        }
        #endregion

        #region ショートカット管理
        /// <summary>
        /// ショートカット設定を更新
        /// </summary>
        private void UpdateShortcuts(Dictionary<string, Keys> newShortcuts)
        {
            shortcuts.Clear();
            foreach (var shortcut in newShortcuts)
            {
                var action = GetActionForShortcut(shortcut.Key);
                if (action != null)
                {
                    shortcuts[shortcut.Value] = action;
                }
            }
        }

        /// <summary>
        /// ショートカット名に対応するアクションを取得
        /// </summary>
        private Action GetActionForShortcut(string shortcutName)
        {
            switch (shortcutName)
            {
                case "新規作成": return () => btnNew_Click(null, null);
                case "保存": return () => btnSave_Click(null, null);
                case "削除": return () => btnDelete_Click(null, null);
                case "名前変更": return () => btnRename_Click(null, null);
                case "移動": return () => btnMove_Click(null, null);
                case "カテゴリ作成": return () => btnAddCategory_Click(null, null);
                case "更新": return () => LoadCategories();
                case "検索": return () => ShowSearchDialog();
                case "お気に入り": return () => ShowFavoritesDialog();
                case "履歴": return () => ShowHistoryDialog();
                default: return null;
            }
        }
        #endregion

        #region ユーティリティメソッド
        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show($"{title}: {message}", "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }

    #region データクラス
    /// <summary>
    /// エクスポート用のデータ構造
    /// </summary>
    public class ExportData
    {
        public List<CategoryData> Categories { get; set; }
        public DateTime ExportDate { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0";
    }

    /// <summary>
    /// カテゴリデータ
    /// </summary>
    public class CategoryData
    {
        public string Name { get; set; }
        public List<FileData> Files { get; set; }
    }

    /// <summary>
    /// ファイルデータ
    /// </summary>
    public class FileData
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    #endregion
}
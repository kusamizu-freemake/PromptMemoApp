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
    /// <summary>
    /// プロンプト編集メインフォーム
    /// ファイル管理、編集、翻訳機能を提供
    /// </summary>
    public partial class PromptEditorForm : Form
    {
        #region フィールド
        private readonly string _baseDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string _currentCategory = "";
        private string _currentFilePath = "";

        // 各種マネージャー
        private FavoritesManager _favoritesManager;
        private HistoryManager _historyManager;
        private TranslationManager _translationManager;

        // ショートカット機能
        private Dictionary<Keys, Action> _shortcuts = new Dictionary<Keys, Action>();

        // 並び替え機能
        private SortOrder _currentSortOrder = SortOrder.Ascending;
        private string _currentSortField = "Name";
        private List<FileInfo> _currentFileList = new List<FileInfo>();

        // 定数
        private const string TEXT_FILE_EXTENSION = "*.txt";
        private const string FILE_EXTENSION = ".txt";
        #endregion

        #region コンストラクタ・初期化
        /// <summary>
        /// PromptEditorFormの新しいインスタンスを初期化します
        /// </summary>
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
            try
            {
                CreateDirectoryIfNotExists();
                InitializeManagers();
                InitializeShortcuts();
                LoadCategories();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("初期化エラー", $"アプリケーションの初期化に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ベースディレクトリが存在しない場合は作成
        /// </summary>
        private void CreateDirectoryIfNotExists()
        {
            if (!Directory.Exists(_baseDirectory))
                Directory.CreateDirectory(_baseDirectory);
        }

        /// <summary>
        /// 各種マネージャーを初期化
        /// </summary>
        private void InitializeManagers()
        {
            _favoritesManager = new FavoritesManager(_baseDirectory);
            _historyManager = new HistoryManager(_baseDirectory);
            _translationManager = new TranslationManager();
        }

        /// <summary>
        /// キーボードショートカットを設定
        /// </summary>
        private void InitializeShortcuts()
        {
            _shortcuts = new Dictionary<Keys, Action>
            {
                [Keys.Control | Keys.N] = CreateNewFile,
                [Keys.Control | Keys.S] = SaveCurrentFile,
                [Keys.Control | Keys.D] = DeleteSelectedFiles,
                [Keys.Control | Keys.R] = RenameSelectedFile,
                [Keys.Control | Keys.M] = () => MoveSelectedFiles(comboBoxCategories.SelectedItem?.ToString()),
                [Keys.Control | Keys.T] = CreateNewCategory,
                [Keys.F5] = LoadCategories,
                [Keys.Control | Keys.F] = ShowSearchDialog,
                [Keys.Control | Keys.O] = ShowFavoritesDialog,
                [Keys.Control | Keys.H] = ShowHistoryDialog
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_shortcuts.ContainsKey(keyData))
            {
                _shortcuts[keyData]();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region カテゴリ管理
        /// <summary>
        /// すべてのカテゴリを読み込んでUIに表示
        /// </summary>
        public void LoadCategories()
        {
            try
            {
                ClearCategoryViews();
                LoadCategoryDirectories();
                SelectFirstCategoryIfExists();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("カテゴリ読み込みエラー", $"カテゴリの読み込みに失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// カテゴリビューをクリア
        /// </summary>
        private void ClearCategoryViews()
        {
            treeViewCategories.Nodes.Clear();
            comboBoxCategories.Items.Clear();
        }

        /// <summary>
        /// カテゴリディレクトリを読み込み
        /// </summary>
        private void LoadCategoryDirectories()
        {
            var directories = Directory.GetDirectories(_baseDirectory);
            foreach (var directory in directories)
            {
                string categoryName = Path.GetFileName(directory);
                AddCategoryToViews(categoryName);
            }
        }

        /// <summary>
        /// カテゴリをビューに追加
        /// </summary>
        private void AddCategoryToViews(string categoryName)
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

        /// <summary>
        /// 新しいカテゴリを作成
        /// </summary>
        public void CreateNewCategory()
        {
            var categoryName = ShowInputDialog("カテゴリ作成", "カテゴリ名を入力してください:");
            if (string.IsNullOrEmpty(categoryName)) return;

            try
            {
                if (CategoryExists(categoryName))
                {
                    ShowWarningMessage("同名のカテゴリが既に存在します。");
                    return;
                }

                CreateCategoryDirectory(categoryName);
                LoadCategories();
                ShowInfoMessage("カテゴリを作成しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("カテゴリ作成エラー", $"カテゴリの作成に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// カテゴリが存在するかチェック
        /// </summary>
        private bool CategoryExists(string categoryName)
        {
            return Directory.Exists(Path.Combine(_baseDirectory, categoryName));
        }

        /// <summary>
        /// カテゴリディレクトリを作成
        /// </summary>
        private void CreateCategoryDirectory(string categoryName)
        {
            string categoryPath = Path.Combine(_baseDirectory, categoryName);
            Directory.CreateDirectory(categoryPath);
        }

        /// <summary>
        /// カテゴリ名を変更
        /// </summary>
        public void RenameCategory(string oldName)
        {
            var newName = ShowInputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください:", oldName);
            if (string.IsNullOrEmpty(newName) || newName == oldName) return;

            try
            {
                if (CategoryExists(newName))
                {
                    ShowWarningMessage("同名のカテゴリが既に存在します。");
                    return;
                }

                string oldPath = Path.Combine(_baseDirectory, oldName);
                string newPath = Path.Combine(_baseDirectory, newName);
                Directory.Move(oldPath, newPath);
                LoadCategories();
                ShowInfoMessage("カテゴリ名を変更しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("カテゴリ名変更エラー", $"カテゴリ名の変更に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// カテゴリを削除
        /// </summary>
        public void DeleteCategory(string categoryName)
        {
            if (!ShowConfirmationDialog("カテゴリを削除しますか？\n※カテゴリ内のすべてのファイルも削除されます。"))
                return;

            try
            {
                string categoryPath = Path.Combine(_baseDirectory, categoryName);
                Directory.Delete(categoryPath, true);
                LoadCategories();
                ClearEditorAndSelection();
                ShowInfoMessage("カテゴリを削除しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("カテゴリ削除エラー", $"カテゴリの削除に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// エディターと選択状態をクリア
        /// </summary>
        private void ClearEditorAndSelection()
        {
            listViewFiles.Items.Clear();
            txtEditor.Clear();
            _currentCategory = "";
            _currentFilePath = "";
        }
        #endregion

        #region ファイル管理
        /// <summary>
        /// 指定されたカテゴリのファイル一覧を読み込み
        /// </summary>
        public void LoadFiles(string category)
        {
            try
            {
                _currentCategory = category;
                ClearFileList();
                LoadFileInfoList(category);
                SortAndDisplayFiles();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル読み込みエラー", $"ファイルの読み込みに失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ファイル一覧をクリア
        /// </summary>
        private void ClearFileList()
        {
            listViewFiles.Items.Clear();
            _currentFileList.Clear();
        }

        /// <summary>
        /// ファイル情報一覧を読み込み
        /// </summary>
        private void LoadFileInfoList(string category)
        {
            string directoryPath = Path.Combine(_baseDirectory, category);
            if (!Directory.Exists(directoryPath)) return;

            var textFiles = Directory.GetFiles(directoryPath, TEXT_FILE_EXTENSION);
            foreach (var file in textFiles)
            {
                _currentFileList.Add(new FileInfo(file));
            }
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
            var sortedFiles = _currentFileList.AsEnumerable();

            switch (_currentSortField)
            {
                case "Name":
                    sortedFiles = _currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.Name)
                        : sortedFiles.OrderByDescending(f => f.Name);
                    break;
                case "Created":
                    sortedFiles = _currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.CreationTime)
                        : sortedFiles.OrderByDescending(f => f.CreationTime);
                    break;
                case "Modified":
                    sortedFiles = _currentSortOrder == SortOrder.Ascending
                        ? sortedFiles.OrderBy(f => f.LastWriteTime)
                        : sortedFiles.OrderByDescending(f => f.LastWriteTime);
                    break;
                case "Size":
                    sortedFiles = _currentSortOrder == SortOrder.Ascending
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
            if (_currentSortField == sortField)
            {
                // 同じフィールドの場合は昇順・降順を切り替え
                _currentSortOrder = _currentSortOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _currentSortField = sortField;
                _currentSortOrder = SortOrder.Ascending;
            }

            SortAndDisplayFiles();
        }

        /// <summary>
        /// 新しいファイルを作成
        /// </summary>
        public void CreateNewFile()
        {
            if (string.IsNullOrEmpty(_currentCategory))
            {
                ShowWarningMessage("カテゴリを選択してください。");
                return;
            }

            var fileName = ShowInputDialog("新規ファイル作成", "ファイル名を入力してください:");
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                if (FileExists(fileName))
                {
                    ShowWarningMessage("同名のファイルが既に存在します。");
                    return;
                }

                CreateFileWithContent(fileName, "");
                LoadFiles(_currentCategory);
                ShowInfoMessage("ファイルを作成しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル作成エラー", $"ファイルの作成に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ファイルが存在するかチェック
        /// </summary>
        private bool FileExists(string fileName)
        {
            string filePath = Path.Combine(_baseDirectory, _currentCategory, fileName + FILE_EXTENSION);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 内容付きでファイルを作成
        /// </summary>
        private void CreateFileWithContent(string fileName, string content)
        {
            string filePath = Path.Combine(_baseDirectory, _currentCategory, fileName + FILE_EXTENSION);
            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// 現在のファイルを保存
        /// </summary>
        public void SaveCurrentFile()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                ShowWarningMessage("ファイルが選択されていません。");
                return;
            }

            try
            {
                File.WriteAllText(_currentFilePath, txtEditor.Text);
                ShowInfoMessage("保存しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("保存エラー", $"ファイルの保存に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 選択されたファイルを読み込み
        /// </summary>
        public void LoadSelectedFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_baseDirectory, _currentCategory, fileName + FILE_EXTENSION);
                _currentFilePath = filePath;

                if (File.Exists(filePath))
                {
                    var content = File.ReadAllText(filePath);
                    txtEditor.Text = content;

                    // 履歴とお気に入り状態を更新
                    _historyManager.UpdateHistory(_currentCategory, fileName, filePath, content);
                    UpdateFavoriteButtonState();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル読み込みエラー", $"ファイルの読み込みに失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 選択されたファイル名を変更
        /// </summary>
        public void RenameSelectedFile()
        {
            if (listViewFiles.SelectedItems.Count == 0) return;

            string oldName = listViewFiles.SelectedItems[0].Text;
            var newName = ShowInputDialog("ファイル名変更", "新しいファイル名を入力してください:", oldName);
            if (string.IsNullOrEmpty(newName) || newName == oldName) return;

            try
            {
                if (FileExists(newName))
                {
                    ShowWarningMessage("同名のファイルが既に存在します。");
                    return;
                }

                string oldPath = Path.Combine(_baseDirectory, _currentCategory, oldName + FILE_EXTENSION);
                string newPath = Path.Combine(_baseDirectory, _currentCategory, newName + FILE_EXTENSION);

                File.Move(oldPath, newPath);
                LoadFiles(_currentCategory);
                ShowInfoMessage("ファイル名を変更しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル名変更エラー", $"ファイル名の変更に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 選択されたファイルを削除
        /// </summary>
        public void DeleteSelectedFiles()
        {
            var selectedItems = GetSelectedFileItems();
            if (!selectedItems.Any()) return;

            string message = selectedItems.Count == 1
                ? "選択したファイルを削除しますか？"
                : $"{selectedItems.Count}個のファイルを削除しますか？";

            if (!ShowConfirmationDialog(message)) return;

            try
            {
                foreach (var item in selectedItems)
                {
                    string filePath = Path.Combine(_baseDirectory, _currentCategory, item.Text + FILE_EXTENSION);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                LoadFiles(_currentCategory);
                ClearEditor();
                ShowInfoMessage("ファイルを削除しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル削除エラー", $"ファイルの削除に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 選択されたファイルアイテムを取得
        /// </summary>
        private List<ListViewItem> GetSelectedFileItems()
        {
            return listViewFiles.SelectedItems.Cast<ListViewItem>().ToList();
        }

        /// <summary>
        /// ファイルを移動
        /// </summary>
        public void MoveSelectedFiles(string targetCategory)
        {
            var selectedItems = GetSelectedFileItems();
            if (!selectedItems.Any())
            {
                ShowWarningMessage("移動するファイルを選択してください。");
                return;
            }

            if (string.IsNullOrEmpty(targetCategory))
            {
                ShowWarningMessage("移動先のカテゴリを選択してください。");
                return;
            }

            try
            {
                foreach (var item in selectedItems)
                {
                    string srcPath = Path.Combine(_baseDirectory, _currentCategory, item.Text + FILE_EXTENSION);
                    string destPath = Path.Combine(_baseDirectory, targetCategory, item.Text + FILE_EXTENSION);
                    if (File.Exists(srcPath))
                        File.Move(srcPath, destPath);
                }

                LoadFiles(_currentCategory);
                ShowInfoMessage($"{selectedItems.Count}個のファイルを移動しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル移動エラー", $"ファイルの移動に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// エディターをクリア
        /// </summary>
        private void ClearEditor()
        {
            txtEditor.Clear();
            _currentFilePath = "";
        }
        #endregion

        #region お気に入り管理
        /// <summary>
        /// お気に入りボタンの表示を更新
        /// </summary>
        private void UpdateFavoriteButtonState()
        {
            if (listViewFiles.SelectedItems.Count > 0)
            {
                var fileName = listViewFiles.SelectedItems[0].Text;
                var isFavorite = _favoritesManager.IsFavorite(_currentCategory, fileName);
                btnFavorite.Text = isFavorite ? "★" : "☆";
                btnFavorite.ForeColor = isFavorite ? Color.Red : Color.Black;
            }
        }

        /// <summary>
        /// お気に入り状態を切り替え
        /// </summary>
        public void ToggleFavorite()
        {
            if (listViewFiles.SelectedItems.Count == 0)
            {
                ShowWarningMessage("ファイルを選択してください。");
                return;
            }

            try
            {
                var fileName = listViewFiles.SelectedItems[0].Text;
                var isFavorite = _favoritesManager.IsFavorite(_currentCategory, fileName);

                if (isFavorite)
                {
                    _favoritesManager.RemoveFavorite(_currentCategory, fileName);
                    ShowInfoMessage("お気に入りから削除しました。");
                }
                else
                {
                    _favoritesManager.AddFavorite(_currentCategory, fileName, _currentFilePath);
                    ShowInfoMessage("お気に入りに追加しました。");
                }

                UpdateFavoriteButtonState();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("お気に入り操作エラー", $"お気に入りの操作に失敗しました。{ex.Message}");
            }
        }
        #endregion

        #region 翻訳機能
        /// <summary>
        /// 翻訳処理を実行
        /// </summary>
        public async Task PerformTranslation()
        {
            if (string.IsNullOrEmpty(txtEditor.Text))
            {
                ShowWarningMessage("翻訳するテキストがありません。");
                return;
            }

            try
            {
                using (var dialog = new TranslationDialog(_translationManager, txtEditor.Text))
                {
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string sourceLang = dialog.GetSourceLanguage();
                    string targetLang = dialog.GetTargetLanguage();

                    if (sourceLang == targetLang)
                    {
                        ShowInfoMessage("元言語と翻訳後の言語が同じです。");
                        return;
                    }

                    string translated = await _translationManager.TranslateAsync(txtEditor.Text, sourceLang, targetLang);
                    txtEditor.Text = translated;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("翻訳エラー", $"翻訳処理に失敗しました。{ex.Message}");
            }
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

                ShowInfoMessage("データのエクスポートが完了しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("エクスポートエラー", $"エクスポートに失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// エクスポート用のデータオブジェクトを作成
        /// </summary>
        private ExportData CreateExportData()
        {
            var exportData = new ExportData();

            var categories = Directory.GetDirectories(_baseDirectory);
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
            var categoryData = new CategoryData { Name = categoryName };

            var files = Directory.GetFiles(categoryPath, TEXT_FILE_EXTENSION);
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
                    ShowWarningMessage("インポートファイルの形式が正しくありません。");
                    return;
                }

                ImportCategories(importData.Categories);
                LoadCategories();
                ShowInfoMessage("データのインポートが完了しました。");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("インポートエラー", $"インポートに失敗しました。{ex.Message}");
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
            var categoryPath = Path.Combine(_baseDirectory, categoryData.Name);
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
            var filePath = Path.Combine(categoryPath, fileData.Name + FILE_EXTENSION);
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
            var categories = Directory.GetDirectories(_baseDirectory);
            int totalFiles = 0;
            long totalSize = 0;
            var categoryCounts = new Dictionary<string, int>();

            foreach (var categoryPath in categories)
            {
                var categoryName = Path.GetFileName(categoryPath);
                var files = Directory.GetFiles(categoryPath, TEXT_FILE_EXTENSION);
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
            LoadFiles(e.Node.Text);
            ClearEditor();
        }

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            string[] fields = { "Name", "Created", "Modified", "Size" };
            if (e.Column < fields.Length)
            {
                SortFiles(fields[e.Column]);
            }
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewFiles.SelectedItems.Count == 0) return;

            var selectedItem = listViewFiles.SelectedItems[0];
            LoadSelectedFile(selectedItem.Text);
        }

        // ボタンイベントハンドラー
        private void btnNew_Click(object sender, EventArgs e) => CreateNewFile();
        private void btnSave_Click(object sender, EventArgs e) => SaveCurrentFile();
        private void btnDelete_Click(object sender, EventArgs e) => DeleteSelectedFiles();
        private void btnRename_Click(object sender, EventArgs e) => RenameSelectedFile();
        private void btnMove_Click(object sender, EventArgs e) => MoveSelectedFiles(comboBoxCategories.SelectedItem?.ToString());
        private void btnAddCategory_Click(object sender, EventArgs e) => CreateNewCategory();
        private void btnFavorite_Click(object sender, EventArgs e) => ToggleFavorite();
        private async void btnTranslate_Click(object sender, EventArgs e) => await PerformTranslation();

        // メニューイベントハンドラー
        private void menuExit_Click(object sender, EventArgs e) => Close();
        private void menuNew_Click(object sender, EventArgs e) => CreateNewFile();
        private void menuFavorites_Click(object sender, EventArgs e) => ShowFavoritesDialog();
        private void menuHistory_Click(object sender, EventArgs e) => ShowHistoryDialog();
        private void menuSearch_Click(object sender, EventArgs e) => ShowSearchDialog();
        private void menuShortcuts_Click(object sender, EventArgs e) => ShowShortcutSettingsDialog();
        private void menuSort_Click(object sender, EventArgs e) => ShowSortSettingsDialog();
        private void menuStatistics_Click(object sender, EventArgs e) => ShowStatisticsDialog();
        private void menuExportImport_Click(object sender, EventArgs e) => ShowExportImportDialog();

        // コンテキストメニューイベント
        private void renameCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode != null)
            {
                RenameCategory(treeViewCategories.SelectedNode.Text);
            }
        }

        private void deleteCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode != null)
            {
                DeleteCategory(treeViewCategories.SelectedNode.Text);
            }
        }
        #endregion

        #region ダイアログ表示メソッド
        /// <summary>
        /// お気に入りダイアログを表示
        /// </summary>
        private void ShowFavoritesDialog()
        {
            try
            {
                using (var dialog = new FavoritesDialog(_favoritesManager, SelectFileInCategory))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"お気に入りダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 履歴ダイアログを表示
        /// </summary>
        private void ShowHistoryDialog()
        {
            try
            {
                using (var dialog = new HistoryDialog(_historyManager, SelectFileInCategory))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"履歴ダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 検索ダイアログを表示
        /// </summary>
        private void ShowSearchDialog()
        {
            try
            {
                using (var dialog = new SearchDialog(_baseDirectory, SelectFileInCategory))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"検索ダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ショートカット設定ダイアログを表示
        /// </summary>
        private void ShowShortcutSettingsDialog()
        {
            try
            {
                var shortcutNames = GetShortcutNames();

                using (var dialog = new ShortcutSettingsForm(shortcutNames))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        var newShortcuts = dialog.GetShortcuts();
                        UpdateShortcuts(newShortcuts);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"ショートカット設定ダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ソート設定ダイアログを表示
        /// </summary>
        private void ShowSortSettingsDialog()
        {
            try
            {
                using (var dialog = new SortSettingsDialog(this))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"ソート設定ダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// 統計情報ダイアログを表示
        /// </summary>
        private void ShowStatisticsDialog()
        {
            try
            {
                var stats = GetStatistics();
                using (var dialog = new StatisticsDialog(stats))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"統計情報ダイアログの表示に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// エクスポート・インポートダイアログを表示
        /// </summary>
        private void ShowExportImportDialog()
        {
            try
            {
                using (var dialog = new ExportImportDialog(this))
                {
                    dialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ表示エラー", $"エクスポート・インポートダイアログの表示に失敗しました。{ex.Message}");
            }
        }
        #endregion

        #region ファイル選択・ナビゲーション
        /// <summary>
        /// 指定されたカテゴリとファイル名のファイルを選択
        /// </summary>
        public void SelectFileInCategory(string category, string fileName)
        {
            try
            {
                // カテゴリを選択
                var categoryNode = treeViewCategories.Nodes.Cast<TreeNode>()
                    .FirstOrDefault(n => n.Text == category);
                if (categoryNode != null)
                {
                    treeViewCategories.SelectedNode = categoryNode;
                    LoadFiles(category);

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
            catch (Exception ex)
            {
                ShowErrorMessage("ファイル選択エラー", $"ファイルの選択に失敗しました。{ex.Message}");
            }
        }
        #endregion

        #region ショートカット管理
        /// <summary>
        /// ショートカット名とキーの対応を取得
        /// </summary>
        private Dictionary<string, Keys> GetShortcutNames()
        {
            return new Dictionary<string, Keys>
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
        }

        /// <summary>
        /// ショートカット設定を更新
        /// </summary>
        private void UpdateShortcuts(Dictionary<string, Keys> newShortcuts)
        {
            try
            {
                _shortcuts.Clear();
                foreach (var shortcut in newShortcuts)
                {
                    var action = GetActionForShortcut(shortcut.Key);
                    if (action != null)
                    {
                        _shortcuts[shortcut.Value] = action;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ショートカット更新エラー", $"ショートカットの更新に失敗しました。{ex.Message}");
            }
        }

        /// <summary>
        /// ショートカット名に対応するアクションを取得
        /// </summary>
        private Action GetActionForShortcut(string shortcutName)
        {
            switch (shortcutName)
            {
                case "新規作成": return CreateNewFile;
                case "保存": return SaveCurrentFile;
                case "削除": return DeleteSelectedFiles;
                case "名前変更": return RenameSelectedFile;
                case "移動": return () => MoveSelectedFiles(comboBoxCategories.SelectedItem?.ToString());
                case "カテゴリ作成": return CreateNewCategory;
                case "更新": return LoadCategories;
                case "検索": return ShowSearchDialog;
                case "お気に入り": return ShowFavoritesDialog;
                case "履歴": return ShowHistoryDialog;
                default: return null;
            }
        }
        #endregion

        #region メッセージ表示ヘルパー
        /// <summary>
        /// 情報メッセージを表示
        /// </summary>
        private void ShowInfoMessage(string message, string title = "情報")
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        private void ShowWarningMessage(string message, string title = "警告")
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] {title}: {message}");
        }

        /// <summary>
        /// 確認ダイアログを表示
        /// </summary>
        private bool ShowConfirmationDialog(string message, string title = "確認")
        {
            return MessageBox.Show(this, message, title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// 入力ダイアログを表示
        /// </summary>
        private string ShowInputDialog(string title, string message, string defaultValue = "")
        {
            try
            {
                using (var dialog = new InputDialog(title, message, defaultValue))
                {
                    return dialog.ShowDialog(this) == DialogResult.OK ? dialog.InputText : null;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("入力ダイアログエラー", $"入力ダイアログの表示に失敗しました。{ex.Message}");
                return null;
            }
        }
        #endregion
    }

    #region データクラス
    /// <summary>
    /// エクスポート用のデータ構造
    /// アプリケーションの全データをJSONで保存・復元するための構造
    /// </summary>
    public class ExportData
    {
        public List<CategoryData> Categories { get; set; } = new List<CategoryData>();
        public DateTime ExportDate { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0";
        public string ApplicationName { get; set; } = "PromptMemoApp";
    }

    /// <summary>
    /// カテゴリデータ
    /// 1つのカテゴリとその中に含まれるファイル群を表現
    /// </summary>
    public class CategoryData
    {
        public string Name { get; set; } = "";
        public List<FileData> Files { get; set; } = new List<FileData>();
        public DateTime Created { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ファイルデータ
    /// 個別ファイルの内容とメタデータを保持
    /// </summary>
    public class FileData
    {
        public string Name { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
    }
    #endregion
}
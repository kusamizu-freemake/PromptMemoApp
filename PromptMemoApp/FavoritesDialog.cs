using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PromptMemoApp
{
    /// <summary>
    /// お気に入りダイアログ（リファクタリング版）
    /// </summary>
    public partial class FavoritesDialog : Form
    {
        private readonly IFavoritesManager _favoritesManager;
        private readonly Action<string, string> _onFavoriteSelected;
        private List<FavoriteItem> _allFavorites;
        private List<FavoriteItem> _filteredFavorites;
        private FavoriteViewMode _currentViewMode = FavoriteViewMode.Category;

        // UI Components
        private ComboBox cmbCategories;
        private ComboBox cmbSortBy;
        private ComboBox cmbViewMode;
        private TextBox txtFilter;
        private ListView listViewFavorites;
        private Button btnOpen;
        private Button btnRemove;
        private Button btnRemoveAll;
        private Button btnExport;
        private Button btnImport;
        private Button btnClose;
        private Label lblStatus;
        private ToolTip toolTip;

        public FavoritesDialog(IFavoritesManager favoritesManager, Action<string, string> onFavoriteSelected)
        {
            _favoritesManager = favoritesManager ?? throw new ArgumentNullException(nameof(favoritesManager));
            _onFavoriteSelected = onFavoriteSelected ?? throw new ArgumentNullException(nameof(onFavoriteSelected));
            _allFavorites = new List<FavoriteItem>();
            _filteredFavorites = new List<FavoriteItem>();

            InitializeComponent();
            SetupEventHandlers();
            _ = LoadFavoritesAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // フォーム設定
            this.Text = "お気に入り管理";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(700, 500);
            this.KeyPreview = true;

            // ツールチップ
            toolTip = new ToolTip();

            // フィルターパネル
            var filterPanel = CreateFilterPanel();

            // メインパネル
            var mainPanel = CreateMainPanel();

            // ボタンパネル
            var buttonPanel = CreateButtonPanel();

            // レイアウト設定
            filterPanel.Dock = DockStyle.Top;
            buttonPanel.Dock = DockStyle.Bottom;
            mainPanel.Dock = DockStyle.Fill;

            this.Controls.AddRange(new Control[] {
                filterPanel,
                mainPanel,
                buttonPanel
            });

            this.ResumeLayout(false);
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Height = 80,
                Padding = new Padding(10, 5, 10, 5)
            };

            // カテゴリフィルター
            var lblCategory = new Label
            {
                Text = "カテゴリ:",
                Location = new Point(10, 15),
                Size = new Size(60, 20)
            };

            cmbCategories = new ComboBox
            {
                Location = new Point(75, 12),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // 並び順
            var lblSort = new Label
            {
                Text = "並び順:",
                Location = new Point(240, 15),
                Size = new Size(50, 20)
            };

            cmbSortBy = new ComboBox
            {
                Location = new Point(295, 12),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSortBy.Items.AddRange(new[] { "最終アクセス順", "追加日順", "アクセス回数順", "ファイル名順" });
            cmbSortBy.SelectedIndex = 0;

            // 表示モード
            var lblViewMode = new Label
            {
                Text = "表示:",
                Location = new Point(430, 15),
                Size = new Size(40, 20)
            };

            cmbViewMode = new ComboBox
            {
                Location = new Point(475, 12),
                Size = new Size(100, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbViewMode.Items.AddRange(new[] { "詳細", "カテゴリ別", "使用頻度別" });
            cmbViewMode.SelectedIndex = 0;

            // テキストフィルター
            var lblFilter = new Label
            {
                Text = "フィルター:",
                Location = new Point(10, 45),
                Size = new Size(60, 20)
            };

            txtFilter = new TextBox
            {
                Location = new Point(75, 42),
                Size = new Size(200, 23)
            };
            toolTip.SetToolTip(txtFilter, "ファイル名でフィルタリング");

            var btnClearFilter = new Button
            {
                Text = "クリア",
                Location = new Point(285, 42),
                Size = new Size(50, 23),
                UseVisualStyleBackColor = true
            };
            btnClearFilter.Click += (s, e) => txtFilter.Clear();

            panel.Controls.Add(listViewFavorites);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            var panel = new Panel
            {
                Height = 70,
                Padding = new Padding(10, 5, 10, 5)
            };

            // ステータスラベル
            lblStatus = new Label
            {
                Text = "お気に入りを読み込み中...",
                Location = new Point(10, 10),
                Size = new Size(400, 20),
                AutoEllipsis = true
            };

            // 操作ボタン群
            btnOpen = new Button
            {
                Text = "開く(&O)",
                Location = new Point(10, 35),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnRemove = new Button
            {
                Text = "削除(&D)",
                Location = new Point(100, 35),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnRemoveAll = new Button
            {
                Text = "すべて削除(&A)",
                Location = new Point(190, 35),
                Size = new Size(100, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            // インポート/エクスポートボタン
            btnExport = new Button
            {
                Text = "エクスポート(&E)",
                Location = new Point(300, 35),
                Size = new Size(100, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnImport = new Button
            {
                Text = "インポート(&I)",
                Location = new Point(410, 35),
                Size = new Size(100, 25),
                UseVisualStyleBackColor = true
            };

            btnClose = new Button
            {
                Text = "閉じる(&C)",
                Location = new Point(panel.Width - 90, 35),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };

            panel.Controls.AddRange(new Control[] {
                lblStatus, btnOpen, btnRemove, btnRemoveAll,
                btnExport, btnImport, btnClose
            });

            return panel;
        }

        private void SetupEventHandlers()
        {
            // コンボボックス
            cmbCategories.SelectedIndexChanged += Filter_Changed;
            cmbSortBy.SelectedIndexChanged += Filter_Changed;
            cmbViewMode.SelectedIndexChanged += ViewMode_Changed;

            // テキストフィルター
            txtFilter.TextChanged += Filter_Changed;

            // リストビュー
            listViewFavorites.SelectedIndexChanged += ListView_SelectedIndexChanged;
            listViewFavorites.DoubleClick += ListView_DoubleClick;
            listViewFavorites.ColumnClick += ListView_ColumnClick;
            listViewFavorites.KeyDown += ListView_KeyDown;

            // ボタン
            btnOpen.Click += BtnOpen_Click;
            btnRemove.Click += BtnRemove_Click;
            btnRemoveAll.Click += BtnRemoveAll_Click;
            btnExport.Click += BtnExport_Click;
            btnImport.Click += BtnImport_Click;

            // フォーム
            this.KeyDown += FavoritesDialog_KeyDown;

            // お気に入り変更イベント
            _favoritesManager.FavoriteChanged += FavoritesManager_FavoriteChanged;
        }

        private async Task LoadFavoritesAsync()
        {
            try
            {
                lblStatus.Text = "お気に入りを読み込み中...";
                _allFavorites = await _favoritesManager.GetAllFavoritesAsync();
                await UpdateCategoriesAsync();
                ApplyFiltersAndSort();
                lblStatus.Text = $"{_allFavorites.Count}件のお気に入りが登録されています";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"お気に入りの読み込み中にエラーが発生しました: {ex.Message}",
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "読み込みエラー";
            }
        }

        private async Task UpdateCategoriesAsync()
        {
            var categories = await _favoritesManager.GetCategoriesAsync();

            cmbCategories.Items.Clear();
            cmbCategories.Items.Add("すべて");
            cmbCategories.Items.AddRange(categories.ToArray());
            cmbCategories.SelectedIndex = 0;
        }

        private void ApplyFiltersAndSort()
        {
            // フィルタリング
            _filteredFavorites = _allFavorites.Where(ApplyFilter).ToList();

            // ソート
            _filteredFavorites = ApplySort(_filteredFavorites).ToList();

            // 表示更新
            UpdateFavoritesList();
        }

        private bool ApplyFilter(FavoriteItem favorite)
        {
            // カテゴリフィルター
            var selectedCategory = cmbCategories.SelectedItem?.ToString();
            if (selectedCategory != "すべて" && favorite.Category != selectedCategory)
                return false;

            // テキストフィルター
            var filterText = txtFilter.Text.Trim();
            if (!string.IsNullOrEmpty(filterText) &&
                !favorite.FileName.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        private IEnumerable<FavoriteItem> ApplySort(IEnumerable<FavoriteItem> favorites)
        {
            return cmbSortBy.SelectedIndex switch
            {
                0 => favorites.OrderByDescending(f => f.LastAccessed), // 最終アクセス順
                1 => favorites.OrderByDescending(f => f.AddedDate),    // 追加日順
                2 => favorites.OrderByDescending(f => f.AccessCount),  // アクセス回数順
                3 => favorites.OrderBy(f => f.FileName),               // ファイル名順
                _ => favorites.OrderByDescending(f => f.LastAccessed)
            };
        }

        private void UpdateFavoritesList()
        {
            listViewFavorites.BeginUpdate();
            listViewFavorites.Items.Clear();

            if (_currentViewMode == FavoriteViewMode.Category)
            {
                UpdateCategoryGroupedView();
            }
            else if (_currentViewMode == FavoriteViewMode.Frequency)
            {
                UpdateFrequencyGroupedView();
            }
            else
            {
                UpdateDetailView();
            }

            listViewFavorites.EndUpdate();
            UpdateButtonStates();
        }

        private void UpdateDetailView()
        {
            foreach (var favorite in _filteredFavorites)
            {
                var item = CreateListViewItem(favorite);
                listViewFavorites.Items.Add(item);
            }
        }

        private void UpdateCategoryGroupedView()
        {
            var groups = _filteredFavorites.GroupBy(f => f.Category).OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                var listGroup = new ListViewGroup($"{group.Key} ({group.Count()}件)");
                listViewFavorites.Groups.Add(listGroup);

                foreach (var favorite in group)
                {
                    var item = CreateListViewItem(favorite);
                    item.Group = listGroup;
                    listViewFavorites.Items.Add(item);
                }
            }
        }

        private void UpdateFrequencyGroupedView()
        {
            var frequencyGroups = new[]
            {
                ("高頻度", _filteredFavorites.Where(f => f.AccessFrequency == "高頻度")),
                ("中頻度", _filteredFavorites.Where(f => f.AccessFrequency == "中頻度")),
                ("低頻度", _filteredFavorites.Where(f => f.AccessFrequency == "低頻度")),
                ("新規", _filteredFavorites.Where(f => f.AccessFrequency == "新規"))
            };

            foreach (var (name, items) in frequencyGroups)
            {
                if (!items.Any()) continue;

                var listGroup = new ListViewGroup($"{name} ({items.Count()}件)");
                listViewFavorites.Groups.Add(listGroup);

                foreach (var favorite in items)
                {
                    var item = CreateListViewItem(favorite);
                    item.Group = listGroup;

                    // 頻度に応じた色分け
                    item.BackColor = name switch
                    {
                        "高頻度" => Color.LightGreen,
                        "中頻度" => Color.LightYellow,
                        "低頻度" => Color.LightGray,
                        _ => Color.White
                    };

                    listViewFavorites.Items.Add(item);
                }
            }
        }

        private ListViewItem CreateListViewItem(FavoriteItem favorite)
        {
            var item = new ListViewItem(favorite.Category);
            item.SubItems.Add(favorite.FileName);
            item.SubItems.Add(favorite.AddedDate.ToString("yyyy/MM/dd"));
            item.SubItems.Add(favorite.AccessCount.ToString());
            item.SubItems.Add(favorite.LastAccessed.ToString("yyyy/MM/dd HH:mm"));
            item.SubItems.Add(favorite.AccessFrequency);
            item.SubItems.Add(favorite.RelativeAccessTime);
            item.Tag = favorite;

            // ツールチップ設定
            item.ToolTipText = $"カテゴリ: {favorite.Category}\n" +
                              $"ファイル名: {favorite.FileName}\n" +
                              $"追加日: {favorite.AddedDate:yyyy/MM/dd HH:mm}\n" +
                              $"アクセス回数: {favorite.AccessCount}\n" +
                              $"最終アクセス: {favorite.LastAccessed:yyyy/MM/dd HH:mm}";

            return item;
        }

        private void UpdateButtonStates()
        {
            var hasSelection = listViewFavorites.SelectedItems.Count > 0;
            var hasItems = _filteredFavorites.Count > 0;

            btnOpen.Enabled = hasSelection;
            btnRemove.Enabled = hasSelection;
            btnRemoveAll.Enabled = hasItems;
            btnExport.Enabled = hasItems;
        }

        // イベントハンドラ群
        private void Filter_Changed(object sender, EventArgs e)
        {
            ApplyFiltersAndSort();
        }

        private void ViewMode_Changed(object sender, EventArgs e)
        {
            _currentViewMode = (FavoriteViewMode)cmbViewMode.SelectedIndex;
            listViewFavorites.Groups.Clear();
            ApplyFiltersAndSort();
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count > 0)
            {
                OpenSelectedFavorite();
            }
        }

        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var sorter = listViewFavorites.ListViewItemSorter as ListViewColumnSorter ??
                        new ListViewColumnSorter();

            sorter.SortColumn = e.Column;
            sorter.Order = sorter.Order == SortOrder.Ascending ?
                          SortOrder.Descending : SortOrder.Ascending;

            listViewFavorites.ListViewItemSorter = sorter;
            listViewFavorites.Sort();
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedFavorites();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                OpenSelectedFavorite();
                e.Handled = true;
            }
        }

        private async void BtnOpen_Click(object sender, EventArgs e)
        {
            await OpenSelectedFavoriteAsync();
        }

        private async void BtnRemove_Click(object sender, EventArgs e)
        {
            await RemoveSelectedFavoritesAsync();
        }

        private async void BtnRemoveAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("すべてのお気に入りを削除しますか？\nこの操作は取り消せません。",
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    foreach (var favorite in _allFavorites.ToList())
                    {
                        await _favoritesManager.RemoveFavoriteAsync(favorite.Category, favorite.FileName);
                    }
                    MessageBox.Show("すべてのお気に入りを削除しました。", "完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除中にエラーが発生しました: {ex.Message}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|CSV files (*.csv)|*.csv",
                DefaultExt = "json"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (Path.GetExtension(saveDialog.FileName).ToLower() == ".json")
                        await ExportToJsonAsync(saveDialog.FileName);
                    else
                        await ExportToCsvAsync(saveDialog.FileName);

                    MessageBox.Show("お気に入りをエクスポートしました。", "完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エクスポート中にエラーが発生しました: {ex.Message}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await ImportFromJsonAsync(openDialog.FileName);
                    MessageBox.Show("お気に入りをインポートしました。", "完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"インポート中にエラーが発生しました: {ex.Message}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FavoritesDialog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    _ = LoadFavoritesAsync();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private async void FavoritesManager_FavoriteChanged(object sender, FavoriteChangedEventArgs e)
        {
            // UIスレッドで実行
            if (InvokeRequired)
            {
                Invoke(new Action(() => _ = LoadFavoritesAsync()));
            }
            else
            {
                await LoadFavoritesAsync();
            }
        }

        // ビジネスロジック
        private async Task OpenSelectedFavoriteAsync()
        {
            if (listViewFavorites.SelectedItems.Count == 0) return;

            var item = listViewFavorites.SelectedItems[0];
            var favorite = item.Tag as FavoriteItem;

            await _favoritesManager.UpdateAccessCountAsync(favorite.Category, favorite.FileName);
            _onFavoriteSelected(favorite.Category, favorite.FileName);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OpenSelectedFavorite()
        {
            _ = OpenSelectedFavoriteAsync();
        }

        private async Task RemoveSelectedFavoritesAsync()
        {
            if (listViewFavorites.SelectedItems.Count == 0) return;

            var selectedFavorites = listViewFavorites.SelectedItems.Cast<ListViewItem>()
                .Select(item => item.Tag as FavoriteItem).ToList();

            var message = selectedFavorites.Count == 1
                ? $"「{selectedFavorites[0].FileName}」をお気に入りから削除しますか？"
                : $"{selectedFavorites.Count}件のお気に入りを削除しますか？";

            if (MessageBox.Show(message, "確認", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    foreach (var favorite in selectedFavorites)
                    {
                        await _favoritesManager.RemoveFavoriteAsync(favorite.Category, favorite.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除中にエラーが発生しました: {ex.Message}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveSelectedFavorites()
        {
            _ = RemoveSelectedFavoritesAsync();
        }

        private async Task ExportToJsonAsync(string filePath)
        {
            var json = JsonConvert.SerializeObject(_allFavorites, Formatting.Indented);
            // .NET Framework 4.7.2 用の非同期ファイル書き込み
            await Task.Run(() => File.WriteAllText(filePath, json, Encoding.UTF8));
        }

        private async Task ExportToCsvAsync(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("カテゴリ,ファイル名,追加日,アクセス回数,最終アクセス");

            foreach (var favorite in _allFavorites)
            {
                sb.AppendLine($"\"{favorite.Category}\"," +
                             $"\"{favorite.FileName}\"," +
                             $"\"{favorite.AddedDate:yyyy/MM/dd HH:mm}\"," +
                             $"{favorite.AccessCount}," +
                             $"\"{favorite.LastAccessed:yyyy/MM/dd HH:mm}\"");
            }

            // .NET Framework 4.7.2 用の非同期ファイル書き込み
            await Task.Run(() => File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8));
        }

        private async Task ImportFromJsonAsync(string filePath)
        {
            // .NET Framework 4.7.2 用の非同期ファイル読み込み
            var json = await Task.Run(() => File.ReadAllText(filePath, Encoding.UTF8));
            var importedFavorites = JsonConvert.DeserializeObject<List<FavoriteItem>>(json);

            if (importedFavorites?.Count > 0)
            {
                foreach (var favorite in importedFavorites)
                {
                    await _favoritesManager.AddFavoriteAsync(favorite.Category, favorite.FileName);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_favoritesManager != null)
                {
                    _favoritesManager.FavoriteChanged -= FavoritesManager_FavoriteChanged;
                }
                toolTip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// お気に入り表示モード
    /// </summary>
    public enum FavoriteViewMode
    {
        Detail,     // 詳細表示
        Category,   // カテゴリ別グループ表示
        Frequency   // 使用頻度別グループ表示
    }
}
Controls.AddRange(new Control[] {
                lblCategory, cmbCategories, lblSort, cmbSortBy,
                lblViewMode, cmbViewMode, lblFilter, txtFilter, btnClearFilter
            });

return panel;
        }

        private Panel CreateMainPanel()
{
    var panel = new Panel();

    // リストビュー
    listViewFavorites = new ListView
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        MultiSelect = true,
        AllowColumnReorder = true,
        HideSelection = false
    };

    // カラム設定
    listViewFavorites.Columns.AddRange(new[] {
                new ColumnHeader { Text = "カテゴリ", Width = 100 },
                new ColumnHeader { Text = "ファイル名", Width = 200 },
                new ColumnHeader { Text = "追加日", Width = 100 },
                new ColumnHeader { Text = "アクセス回数", Width = 90 },
                new ColumnHeader { Text = "最終アクセス", Width = 120 },
                new ColumnHeader { Text = "使用頻度", Width = 80 },
                new ColumnHeader { Text = "相対時間", Width = 100 }
            });

    panel.
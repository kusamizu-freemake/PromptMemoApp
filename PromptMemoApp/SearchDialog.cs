using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// 検索サービスのインターフェース
    /// </summary>
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> SearchAsync(string searchText, SearchOptions options,
            IProgress<SearchProgress> progress, CancellationToken cancellationToken);
    }

    /// <summary>
    /// ファイル検索サービス
    /// </summary>
    public class FileSearchService : ISearchService
    {
        private readonly string _baseDirectory;

        public FileSearchService(string baseDirectory)
        {
            _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(string searchText, SearchOptions options,
            IProgress<SearchProgress> progress, CancellationToken cancellationToken)
        {
            return await Task.Run(() => PerformSearch(searchText, options, progress, cancellationToken), cancellationToken);
        }

        private IEnumerable<SearchResult> PerformSearch(string searchText, SearchOptions options,
            IProgress<SearchProgress> progress, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            var allFiles = new List<string>();

            // 対象ファイルを収集
            foreach (var category in Directory.GetDirectories(_baseDirectory))
            {
                foreach (var ext in options.FileExtensions)
                {
                    var files = Directory.GetFiles(category, $"*{ext}");
                    allFiles.AddRange(files);
                }
            }

            var totalFiles = allFiles.Count;
            var processedFiles = 0;

            // 正規表現の準備
            Regex regex = null;
            if (options.UseRegex)
            {
                try
                {
                    var regexOptions = options.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
                    regex = new Regex(searchText, regexOptions);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("無効な正規表現です");
                }
            }

            foreach (var filePath in allFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var content = File.ReadAllText(filePath);
                    var categoryName = Path.GetFileName(Path.GetDirectoryName(filePath));
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileInfo = new FileInfo(filePath);

                    var matches = FindMatches(content, searchText, options, regex);

                    if (matches.Count > 0)
                    {
                        var result = new SearchResult
                        {
                            Category = categoryName,
                            FileName = fileName,
                            FilePath = filePath,
                            Content = content,
                            Matches = matches,
                            FileSize = fileInfo.Length,
                            LastModified = fileInfo.LastWriteTime
                        };
                        results.Add(result);
                    }
                }
                catch (Exception)
                {
                    // ファイル読み込みエラーは無視して続行
                }

                processedFiles++;
                var percentage = (int)((double)processedFiles / totalFiles * 100);
                progress?.Report(new SearchProgress
                {
                    Percentage = percentage,
                    Message = $"検索中... ({processedFiles}/{totalFiles})",
                    FilesProcessed = processedFiles,
                    TotalFiles = totalFiles
                });
            }

            return results;
        }

        private List<SearchMatch> FindMatches(string content, string searchText, SearchOptions options, Regex regex)
        {
            var matches = new List<SearchMatch>();

            if (options.UseRegex && regex != null)
            {
                var regexMatches = regex.Matches(content);
                foreach (Match match in regexMatches)
                {
                    matches.Add(new SearchMatch
                    {
                        Index = match.Index,
                        Length = match.Length,
                        Text = match.Value
                    });
                }
            }
            else
            {
                var searchString = options.CaseInsensitive ? searchText.ToLower() : searchText;
                var contentToSearch = options.CaseInsensitive ? content.ToLower() : content;

                if (options.WholeWordOnly)
                {
                    var pattern = @"\b" + Regex.Escape(searchString) + @"\b";
                    var regexOptions = options.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
                    var wordRegex = new Regex(pattern, regexOptions);
                    var regexMatches = wordRegex.Matches(content);

                    foreach (Match match in regexMatches)
                    {
                        matches.Add(new SearchMatch
                        {
                            Index = match.Index,
                            Length = match.Length,
                            Text = match.Value
                        });
                    }
                }
                else
                {
                    var index = 0;
                    while ((index = contentToSearch.IndexOf(searchString, index)) != -1)
                    {
                        matches.Add(new SearchMatch
                        {
                            Index = index,
                            Length = searchString.Length,
                            Text = content.Substring(index, searchString.Length)
                        });
                        index += searchString.Length;
                    }
                }
            }

            return matches;
        }
    }

    /// <summary>
    /// 検索結果クラス
    /// </summary>
    public class SearchResult
    {
        public string Category { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public List<SearchMatch> Matches { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }

        public SearchResult()
        {
            Matches = new List<SearchMatch>();
        }
    }

    /// <summary>
    /// 検索マッチ情報
    /// </summary>
    public class SearchMatch
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string Text { get; set; }
    }
    /// <summary>
    /// 検索ダイアログ（リファクタリング版）
    /// </summary>
    public partial class SearchDialog : Form
    {
        private readonly ISearchService _searchService;
        private readonly Action<string, string> _onSearchResultSelected;
        private readonly SearchHistory _searchHistory;
        private CancellationTokenSource _cancellationTokenSource;
        private List<SearchResult> _currentResults;

        // UI Components
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnCancel;
        private CheckBox chkCaseInsensitive;
        private CheckBox chkRegex;
        private CheckBox chkWholeWord;
        private ComboBox cmbFileTypes;
        private ListView listViewResults;
        private Button btnOpen;
        private Button btnClose;
        private Button btnExport;
        private Label lblStatus;
        private ProgressBar progressBar;
        private ToolTip toolTip;

        public SearchDialog(string baseDirectory, Action<string, string> onSearchResultSelected)
        {
            _searchService = new FileSearchService(baseDirectory);
            _onSearchResultSelected = onSearchResultSelected ?? throw new ArgumentNullException(nameof(onSearchResultSelected));
            _searchHistory = new SearchHistory();
            _currentResults = new List<SearchResult>();

            InitializeComponent();
            LoadSearchHistory();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // フォーム設定
            this.Text = "高度な検索";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(800, 600);
            this.KeyPreview = true;

            // ツールチップ
            toolTip = new ToolTip();

            // 検索条件パネル
            var searchPanel = CreateSearchPanel();

            // 結果表示パネル
            var resultsPanel = CreateResultsPanel();

            // ボタンパネル
            var buttonPanel = CreateButtonPanel();

            // レイアウト設定
            searchPanel.Dock = DockStyle.Top;
            buttonPanel.Dock = DockStyle.Bottom;
            resultsPanel.Dock = DockStyle.Fill;

            this.Controls.AddRange(new Control[] {
                searchPanel,
                resultsPanel,
                buttonPanel
            });

            this.ResumeLayout(false);
        }

        private Panel CreateSearchPanel()
        {
            var panel = new Panel
            {
                Height = 120,
                Padding = new Padding(10)
            };

            // 検索テキストボックス
            txtSearch = new TextBox
            {
                Location = new Point(80, 10),
                Size = new Size(300, 23),
                Font = new Font("メイリオ", 9F)
            };
            toolTip.SetToolTip(txtSearch, "検索するテキストを入力してください（Enterキーで検索開始）");

            var lblSearch = new Label
            {
                Text = "検索文字列:",
                Location = new Point(10, 13),
                Size = new Size(65, 20)
            };

            // 検索ボタン
            btnSearch = new Button
            {
                Text = "検索開始",
                Location = new Point(390, 10),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true
            };

            // キャンセルボタン
            btnCancel = new Button
            {
                Text = "停止",
                Location = new Point(480, 10),
                Size = new Size(60, 25),
                Enabled = false,
                UseVisualStyleBackColor = true
            };

            // チェックボックス群
            chkCaseInsensitive = new CheckBox
            {
                Text = "大文字小文字を区別しない",
                Location = new Point(10, 45),
                Size = new Size(180, 20),
                Checked = true
            };

            chkRegex = new CheckBox
            {
                Text = "正規表現を使用",
                Location = new Point(200, 45),
                Size = new Size(120, 20)
            };

            chkWholeWord = new CheckBox
            {
                Text = "完全な単語のみ",
                Location = new Point(330, 45),
                Size = new Size(120, 20)
            };

            // ファイル種別選択
            var lblFileType = new Label
            {
                Text = "ファイル種別:",
                Location = new Point(10, 75),
                Size = new Size(80, 20)
            };

            cmbFileTypes = new ComboBox
            {
                Location = new Point(90, 72),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFileTypes.Items.AddRange(new[] { "すべて", "テキスト(.txt)", "マークダウン(.md)", "JSON(.json)" });
            cmbFileTypes.SelectedIndex = 0;

            // プログレスバー
            progressBar = new ProgressBar
            {
                Location = new Point(220, 75),
                Size = new Size(250, 20),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            panel.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnCancel,
                chkCaseInsensitive, chkRegex, chkWholeWord,
                lblFileType, cmbFileTypes, progressBar
            });

            return panel;
        }

        private Panel CreateResultsPanel()
        {
            var panel = new Panel();

            // 検索結果リストビュー
            listViewResults = new ListView
            {
                Location = new Point(10, 10),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true,
                AllowColumnReorder = true,
                Dock = DockStyle.Fill
            };

            // カラム設定
            listViewResults.Columns.AddRange(new[] {
                new ColumnHeader { Text = "カテゴリ", Width = 100 },
                new ColumnHeader { Text = "ファイル名", Width = 180 },
                new ColumnHeader { Text = "マッチ数", Width = 80 },
                new ColumnHeader { Text = "ファイルサイズ", Width = 90 },
                new ColumnHeader { Text = "更新日時", Width = 130 },
                new ColumnHeader { Text = "プレビュー", Width = 300 }
            });

            // カラムソート機能
            listViewResults.ColumnClick += ListView_ColumnClick;

            panel.Controls.Add(listViewResults);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            var panel = new Panel
            {
                Height = 60,
                Padding = new Padding(10)
            };

            // ステータスラベル
            lblStatus = new Label
            {
                Text = "検索するテキストを入力して「検索開始」ボタンをクリックしてください",
                Location = new Point(10, 10),
                Size = new Size(600, 20),
                AutoEllipsis = true
            };

            // ボタン群
            btnOpen = new Button
            {
                Text = "開く(&O)",
                Location = new Point(10, 30),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnExport = new Button
            {
                Text = "結果出力(&E)",
                Location = new Point(100, 30),
                Size = new Size(90, 25),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            btnClose = new Button
            {
                Text = "閉じる(&C)",
                Location = new Point(panel.Width - 90, 30),
                Size = new Size(80, 25),
                UseVisualStyleBackColor = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };

            panel.Controls.AddRange(new Control[] {
                lblStatus, btnOpen, btnExport, btnClose
            });

            return panel;
        }

        private void SetupEventHandlers()
        {
            // イベントハンドラの設定
            txtSearch.KeyDown += TxtSearch_KeyDown;
            btnSearch.Click += BtnSearch_Click;
            btnCancel.Click += BtnCancel_Click;
            btnOpen.Click += BtnOpen_Click;
            btnExport.Click += BtnExport_Click;
            listViewResults.SelectedIndexChanged += ListView_SelectedIndexChanged;
            listViewResults.DoubleClick += ListView_DoubleClick;
            chkRegex.CheckedChanged += ChkRegex_CheckedChanged;

            // フォームイベント
            this.FormClosing += SearchDialog_FormClosing;
            this.KeyDown += SearchDialog_KeyDown;
        }

        private void LoadSearchHistory()
        {
            // 検索履歴の読み込み（実装省略）
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            await PerformSearchAsync();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CancelSearch();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                _ = PerformSearchAsync();
            }
        }

        private void ChkRegex_CheckedChanged(object sender, EventArgs e)
        {
            // 正規表現選択時は完全一致を無効化
            chkWholeWord.Enabled = !chkRegex.Checked;
            if (chkRegex.Checked)
            {
                chkWholeWord.Checked = false;
            }
        }

        private async Task PerformSearchAsync()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("検索するテキストを入力してください。", "検索",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearch.Focus();
                return;
            }

            // UI状態の更新
            SetSearchingState(true);
            _currentResults.Clear();
            listViewResults.Items.Clear();

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var searchOptions = CreateSearchOptions();

                // 検索履歴に追加
                _searchHistory.AddSearch(txtSearch.Text);

                // 非同期検索の実行
                var progress = new Progress<SearchProgress>(UpdateSearchProgress);
                var results = await _searchService.SearchAsync(
                    txtSearch.Text,
                    searchOptions,
                    progress,
                    _cancellationTokenSource.Token);

                _currentResults = results.ToList();
                UpdateSearchResults();

                lblStatus.Text = $"{_currentResults.Count}件のファイルで" +
                    $"{_currentResults.Sum(r => r.Matches.Count)}件のマッチが見つかりました";
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "検索がキャンセルされました";
            }
            catch (Exception ex)
            {
                var message = $"検索中にエラーが発生しました: {ex.Message}";
                MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "検索中にエラーが発生しました";
            }
            finally
            {
                SetSearchingState(false);
            }
        }

        private SearchOptions CreateSearchOptions()
        {
            return new SearchOptions
            {
                CaseInsensitive = chkCaseInsensitive.Checked,
                UseRegex = chkRegex.Checked,
                WholeWordOnly = chkWholeWord.Checked,
                FileExtensions = GetSelectedFileExtensions(),
                MaxResults = 1000
            };
        }

        private string[] GetSelectedFileExtensions()
        {
            return cmbFileTypes.SelectedIndex switch
            {
                1 => new[] { ".txt" },
                2 => new[] { ".md" },
                3 => new[] { ".json" },
                _ => new[] { ".txt", ".md", ".json" }
            };
        }

        private void UpdateSearchProgress(SearchProgress progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SearchProgress>(UpdateSearchProgress), progress);
                return;
            }

            progressBar.Value = Math.Min(progress.Percentage, 100);
            lblStatus.Text = progress.Message;
        }

        private void UpdateSearchResults()
        {
            listViewResults.BeginUpdate();
            listViewResults.Items.Clear();

            foreach (var result in _currentResults.OrderByDescending(r => r.Matches.Count))
            {
                var item = new ListViewItem(result.Category);
                item.SubItems.Add(result.FileName);
                item.SubItems.Add(result.Matches.Count.ToString());
                item.SubItems.Add(FormatFileSize(result.FileSize));
                item.SubItems.Add(result.LastModified.ToString("yyyy/MM/dd HH:mm"));
                item.SubItems.Add(CreatePreviewText(result));
                item.Tag = result;

                // マッチ数に応じた色分け
                if (result.Matches.Count > 10)
                    item.BackColor = Color.LightYellow;
                else if (result.Matches.Count > 5)
                    item.BackColor = Color.LightBlue;

                listViewResults.Items.Add(item);
            }

            listViewResults.EndUpdate();
            btnExport.Enabled = _currentResults.Count > 0;
        }

        private string CreatePreviewText(SearchResult result)
        {
            if (!result.Matches.Any()) return "";

            var firstMatch = result.Matches[0];
            const int contextLength = 40;

            var start = Math.Max(0, firstMatch.Index - contextLength);
            var end = Math.Min(result.Content.Length, firstMatch.Index + firstMatch.Length + contextLength);
            var preview = result.Content.Substring(start, end - start);

            // マッチ部分をハイライト
            var matchStart = firstMatch.Index - start;
            var matchEnd = matchStart + firstMatch.Length;

            if (matchStart >= 0 && matchEnd <= preview.Length)
            {
                preview = preview.Substring(0, matchStart) +
                         "【" + preview.Substring(matchStart, firstMatch.Length) + "】" +
                         preview.Substring(matchEnd);
            }

            return preview.Replace('\r', ' ').Replace('\n', ' ');
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
            return $"{bytes / (1024 * 1024):F1} MB";
        }

        private void SetSearchingState(bool searching)
        {
            txtSearch.Enabled = !searching;
            btnSearch.Enabled = !searching;
            btnCancel.Enabled = searching;
            chkCaseInsensitive.Enabled = !searching;
            chkRegex.Enabled = !searching;
            chkWholeWord.Enabled = !searching;
            cmbFileTypes.Enabled = !searching;
            progressBar.Visible = searching;

            if (!searching)
            {
                progressBar.Value = 0;
            }
        }

        private void CancelSearch()
        {
            _cancellationTokenSource?.Cancel();
            lblStatus.Text = "検索をキャンセル中...";
        }

        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var sorter = listViewResults.ListViewItemSorter as ListViewColumnSorter ??
                        new ListViewColumnSorter();

            sorter.SortColumn = e.Column;
            sorter.Order = sorter.Order == SortOrder.Ascending ?
                          SortOrder.Descending : SortOrder.Ascending;

            listViewResults.ListViewItemSorter = sorter;
            listViewResults.Sort();
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOpen.Enabled = listViewResults.SelectedItems.Count > 0;
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedResult();
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedResult();
        }

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            if (!_currentResults.Any()) return;

            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt",
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await ExportResultsAsync(saveDialog.FileName);
                    MessageBox.Show("検索結果をエクスポートしました。", "完了",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エクスポート中にエラーが発生しました: {ex.Message}",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ExportResultsAsync(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("カテゴリ,ファイル名,マッチ数,ファイルサイズ,更新日時,プレビュー");

            foreach (var result in _currentResults)
            {
                sb.AppendLine($"\"{result.Category}\"," +
                             $"\"{result.FileName}\"," +
                             $"{result.Matches.Count}," +
                             $"{FormatFileSize(result.FileSize)}," +
                             $"\"{result.LastModified:yyyy/MM/dd HH:mm}\"," +
                             $"\"{CreatePreviewText(result)}\"");
            }

            // .NET Framework 4.7.2 用の非同期ファイル書き込み
            await Task.Run(() => File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8));
        }

        private void OpenSelectedResult()
        {
            if (listViewResults.SelectedItems.Count == 0) return;

            var item = listViewResults.SelectedItems[0];
            var result = item.Tag as SearchResult;

            _onSearchResultSelected(result.Category, result.FileName);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SearchDialog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (btnCancel.Enabled)
                        CancelSearch();
                    else
                        this.Close();
                    break;
                case Keys.F3:
                    _ = PerformSearchAsync();
                    break;
            }
        }

        private void SearchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                toolTip?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // サポートクラス群
    public class SearchOptions
    {
        public bool CaseInsensitive { get; set; } = true;
        public bool UseRegex { get; set; }
        public bool WholeWordOnly { get; set; }
        public string[] FileExtensions { get; set; } = { ".txt" };
        public int MaxResults { get; set; } = 1000;
    }

    public class SearchProgress
    {
        public int Percentage { get; set; }
        public string Message { get; set; }
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
    }

    public class SearchHistory
    {
        private readonly List<string> _history = new List<string>();
        private const int MaxHistoryItems = 20;

        public void AddSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return;

            _history.Remove(searchText);
            _history.Insert(0, searchText);

            if (_history.Count > MaxHistoryItems)
                _history.RemoveAt(_history.Count - 1);
        }

        public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();
    }

    // リスト並び替え用クラス
    public class ListViewColumnSorter : System.Collections.IComparer
    {
        public int SortColumn { get; set; }
        public SortOrder Order { get; set; } = SortOrder.Ascending;

        public int Compare(object x, object y)
        {
            var itemX = x as ListViewItem;
            var itemY = y as ListViewItem;

            if (itemX?.SubItems.Count <= SortColumn || itemY?.SubItems.Count <= SortColumn)
                return 0;

            var textX = itemX.SubItems[SortColumn].Text;
            var textY = itemY.SubItems[SortColumn].Text;

            // 数値カラムの処理
            if (SortColumn == 2 && int.TryParse(textX, out var numX) && int.TryParse(textY, out var numY))
            {
                var result = numX.CompareTo(numY);
                return Order == SortOrder.Ascending ? result : -result;
            }

            // 文字列比較
            var stringResult = string.Compare(textX, textY, StringComparison.CurrentCulture);
            return Order == SortOrder.Ascending ? stringResult : -stringResult;
        }
    }
}
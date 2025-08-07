using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class SearchDialog : Form
    {
        private string baseDirectory;
        private Action<string, string> onSearchResultSelected;
        private List<SearchResult> searchResults;

        public SearchDialog(string baseDirectory, Action<string, string> onSearchResultSelected)
        {
            this.baseDirectory = baseDirectory;
            this.onSearchResultSelected = onSearchResultSelected;
            this.searchResults = new List<SearchResult>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "検索";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 検索テキストボックス
            this.txtSearch = new TextBox();
            this.txtSearch.Location = new System.Drawing.Point(10, 10);
            this.txtSearch.Size = new System.Drawing.Size(300, 20);
            // PlaceholderText is not available in .NET Framework 4.7.2
            // this.txtSearch.PlaceholderText = "検索するテキストを入力してください";

            // 検索ボタン
            this.btnSearch = new Button();
            this.btnSearch.Text = "検索";
            this.btnSearch.Location = new System.Drawing.Point(320, 10);
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.Click += BtnSearch_Click;

            // 大文字小文字を区別しないチェックボックス
            this.chkCaseInsensitive = new CheckBox();
            this.chkCaseInsensitive.Text = "大文字小文字を区別しない";
            this.chkCaseInsensitive.Location = new System.Drawing.Point(420, 12);
            this.chkCaseInsensitive.Size = new System.Drawing.Size(200, 20);
            this.chkCaseInsensitive.Checked = true;

            // 正規表現チェックボックス
            this.chkRegex = new CheckBox();
            this.chkRegex.Text = "正規表現";
            this.chkRegex.Location = new System.Drawing.Point(420, 35);
            this.chkRegex.Size = new System.Drawing.Size(100, 20);

            // 検索結果リストビュー
            this.listViewResults = new ListView();
            this.listViewResults.Location = new System.Drawing.Point(10, 45);
            this.listViewResults.Size = new System.Drawing.Size(760, 450);
            this.listViewResults.View = View.Details;
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.GridLines = true;
            this.listViewResults.DoubleClick += ListViewResults_DoubleClick;

            // リストビューのカラム
            this.listViewResults.Columns.Add("カテゴリ", 100);
            this.listViewResults.Columns.Add("ファイル名", 200);
            this.listViewResults.Columns.Add("マッチ数", 80);
            this.listViewResults.Columns.Add("プレビュー", 350);

            // ボタン
            this.btnOpen = new Button();
            this.btnOpen.Text = "開く";
            this.btnOpen.Location = new System.Drawing.Point(10, 510);
            this.btnOpen.Size = new System.Drawing.Size(80, 25);
            this.btnOpen.Click += BtnOpen_Click;

            this.btnClose = new Button();
            this.btnClose.Text = "閉じる";
            this.btnClose.Location = new System.Drawing.Point(690, 510);
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.DialogResult = DialogResult.Cancel;

            // ステータスラベル
            this.lblStatus = new Label();
            this.lblStatus.Text = "検索するテキストを入力して「検索」ボタンをクリックしてください";
            this.lblStatus.Location = new System.Drawing.Point(100, 515);
            this.lblStatus.Size = new System.Drawing.Size(580, 20);

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                this.txtSearch,
                this.btnSearch,
                this.chkCaseInsensitive,
                this.chkRegex,
                this.listViewResults,
                this.btnOpen,
                this.btnClose,
                this.lblStatus
            });

            this.ResumeLayout(false);
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private CheckBox chkCaseInsensitive;
        private CheckBox chkRegex;
        private ListView listViewResults;
        private Button btnOpen;
        private Button btnClose;
        private Label lblStatus;

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("検索するテキストを入力してください。", "検索", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PerformSearch();
        }

        private void PerformSearch()
        {
            searchResults.Clear();
            listViewResults.Items.Clear();
            lblStatus.Text = "検索中...";
            Application.DoEvents();

            try
            {
                var searchText = txtSearch.Text;
                var caseInsensitive = chkCaseInsensitive.Checked;
                var useRegex = chkRegex.Checked;

                Regex regex = null;
                if (useRegex)
                {
                    try
                    {
                        var options = caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
                        regex = new Regex(searchText, options);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show($"正規表現が無効です: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                foreach (var category in System.IO.Directory.GetDirectories(baseDirectory))
                {
                    var categoryName = System.IO.Path.GetFileName(category);
                    foreach (var file in System.IO.Directory.GetFiles(category, "*.txt"))
                    {
                        var content = System.IO.File.ReadAllText(file);
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        var filePath = file;

                        var matches = new List<SearchMatch>();
                        
                        if (useRegex && regex != null)
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
                            var searchString = caseInsensitive ? searchText.ToLower() : searchText;
                            var contentToSearch = caseInsensitive ? content.ToLower() : content;
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

                        if (matches.Count > 0)
                        {
                            var result = new SearchResult
                            {
                                Category = categoryName,
                                FileName = fileName,
                                FilePath = filePath,
                                Content = content,
                                Matches = matches
                            };
                            searchResults.Add(result);
                        }
                    }
                }

                UpdateSearchResults();
                lblStatus.Text = $"{searchResults.Count}件のファイルで{searchResults.Sum(r => r.Matches.Count)}件のマッチが見つかりました";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"検索中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "検索中にエラーが発生しました";
            }
        }

        private void UpdateSearchResults()
        {
            listViewResults.Items.Clear();
            
            foreach (var result in searchResults.OrderByDescending(r => r.Matches.Count))
            {
                var item = new ListViewItem(result.Category);
                item.SubItems.Add(result.FileName);
                item.SubItems.Add(result.Matches.Count.ToString());
                
                // プレビューテキストを作成（最初のマッチをハイライト）
                var preview = CreatePreviewText(result);
                item.SubItems.Add(preview);
                
                item.Tag = result;
                listViewResults.Items.Add(item);
            }
        }

        private string CreatePreviewText(SearchResult result)
        {
            if (result.Matches.Count == 0) return "";

            var firstMatch = result.Matches[0];
            var start = Math.Max(0, firstMatch.Index - 30);
            var end = Math.Min(result.Content.Length, firstMatch.Index + firstMatch.Length + 30);
            var preview = result.Content.Substring(start, end - start);

            // マッチ部分をハイライト表示
            var matchStart = firstMatch.Index - start;
            var matchEnd = matchStart + firstMatch.Length;
            
            if (matchStart >= 0 && matchEnd <= preview.Length)
            {
                preview = preview.Substring(0, matchStart) + 
                         "【" + preview.Substring(matchStart, firstMatch.Length) + "】" + 
                         preview.Substring(matchEnd);
            }

            return preview.Replace("\r", " ").Replace("\n", " ");
        }

        private void ListViewResults_DoubleClick(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                OpenSelectedResult();
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedResult();
        }

        private void OpenSelectedResult()
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var item = listViewResults.SelectedItems[0];
                var result = item.Tag as SearchResult;
                
                onSearchResultSelected(result.Category, result.FileName);
                this.Close();
            }
        }
    }

    public class SearchResult
    {
        public string Category { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public List<SearchMatch> Matches { get; set; }

        public SearchResult()
        {
            Matches = new List<SearchMatch>();
        }
    }

    public class SearchMatch
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string Text { get; set; }
    }
}

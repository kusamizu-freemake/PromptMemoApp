using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// 統計情報を表示するダイアログ
    /// アプリケーションの使用状況やデータ情報を一覧形式で表示
    /// </summary>
    public partial class StatisticsDialog : Form
    {
        #region フィールド
        private readonly Dictionary<string, object> _statistics;
        private ListView _listViewStats;
        private Button _btnClose;
        #endregion

        #region 定数
        private const int DIALOG_WIDTH = 500;
        private const int DIALOG_HEIGHT = 400;
        private const int MARGIN = 20;
        private const int BUTTON_WIDTH = 80;
        private const int BUTTON_HEIGHT = 25;
        private const int LIST_HEIGHT = 300;
        private const int COLUMN_ITEM_WIDTH = 200;
        private const int COLUMN_VALUE_WIDTH = 250;
        private const double BYTES_TO_KB = 1024.0;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// StatisticsDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="statistics">表示する統計データ</param>
        /// <exception cref="ArgumentNullException">statistics が null の場合</exception>
        public StatisticsDialog(Dictionary<string, object> statistics)
        {
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));

            InitializeComponent();
            SetupDialog();
            LoadStatistics();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// コンポーネントの初期化を行います
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            CreateControls();
            SetupControlProperties();
            AddControlsToForm();

            ResumeLayout(false);
        }

        /// <summary>
        /// ダイアログの追加設定を行います
        /// </summary>
        private void SetupDialog()
        {
            // フォーム設定
            Text = "統計情報";
            Size = new System.Drawing.Size(DIALOG_WIDTH, DIALOG_HEIGHT);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // キーボードショートカット設定
            KeyPreview = true;
            KeyDown += StatisticsDialog_KeyDown;
        }

        /// <summary>
        /// コントロールを作成します
        /// </summary>
        private void CreateControls()
        {
            CreateListView();
            CreateCloseButton();
        }

        /// <summary>
        /// リストビューを作成します
        /// </summary>
        private void CreateListView()
        {
            _listViewStats = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new System.Drawing.Point(MARGIN, MARGIN),
                Size = new System.Drawing.Size(DIALOG_WIDTH - (MARGIN * 3), LIST_HEIGHT),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // カラムヘッダー追加
            _listViewStats.Columns.Add("項目", COLUMN_ITEM_WIDTH);
            _listViewStats.Columns.Add("値", COLUMN_VALUE_WIDTH);
        }

        /// <summary>
        /// 閉じるボタンを作成します
        /// </summary>
        private void CreateCloseButton()
        {
            _btnClose = new Button
            {
                Text = "閉じる",
                Location = new System.Drawing.Point(
                    DIALOG_WIDTH - BUTTON_WIDTH - MARGIN - 10,
                    LIST_HEIGHT + (MARGIN * 2)
                ),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                DialogResult = DialogResult.OK,
                UseVisualStyleBackColor = true
            };

            _btnClose.Click += BtnClose_Click;
        }

        /// <summary>
        /// コントロールのプロパティを設定します
        /// </summary>
        private void SetupControlProperties()
        {
            // Accept/Cancelボタンの設定
            AcceptButton = _btnClose;
            CancelButton = _btnClose;
        }

        /// <summary>
        /// コントロールをフォームに追加します
        /// </summary>
        private void AddControlsToForm()
        {
            Controls.AddRange(new Control[] {
                _listViewStats,
                _btnClose
            });
        }
        #endregion

        #region データ表示
        /// <summary>
        /// 統計データをリストビューに読み込みます
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                _listViewStats.Items.Clear();

                // エラー情報がある場合は優先表示
                if (HasErrorInformation())
                {
                    DisplayErrorInformation();
                    return;
                }

                // 基本統計情報を表示
                DisplayBasicStatistics();

                // カテゴリ別統計情報を表示
                DisplayCategoryStatistics();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("統計情報の読み込み中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// エラー情報があるかチェックします
        /// </summary>
        /// <returns>エラー情報がある場合は true</returns>
        private bool HasErrorInformation()
        {
            return _statistics.ContainsKey("Error");
        }

        /// <summary>
        /// エラー情報を表示します
        /// </summary>
        private void DisplayErrorInformation()
        {
            var errorMessage = _statistics["Error"]?.ToString() ?? "不明なエラー";
            AddStatisticItem("エラー", errorMessage);
        }

        /// <summary>
        /// 基本統計情報を表示します
        /// </summary>
        private void DisplayBasicStatistics()
        {
            DisplayTotalCategories();
            DisplayTotalFiles();
            DisplayTotalSize();
            DisplayAverageFilesPerCategory();
        }

        /// <summary>
        /// 総カテゴリ数を表示します
        /// </summary>
        private void DisplayTotalCategories()
        {
            if (_statistics.TryGetValue("TotalCategories", out var value))
            {
                AddStatisticItem("総カテゴリ数", value.ToString());
            }
        }

        /// <summary>
        /// 総ファイル数を表示します
        /// </summary>
        private void DisplayTotalFiles()
        {
            if (_statistics.TryGetValue("TotalFiles", out var value))
            {
                AddStatisticItem("総ファイル数", value.ToString());
            }
        }

        /// <summary>
        /// 総ファイルサイズを表示します
        /// </summary>
        private void DisplayTotalSize()
        {
            if (_statistics.TryGetValue("TotalSize", out var value) && value is long totalSize)
            {
                var sizeText = FormatFileSize(totalSize);
                AddStatisticItem("総ファイルサイズ", sizeText);
            }
        }

        /// <summary>
        /// カテゴリあたりの平均ファイル数を表示します
        /// </summary>
        private void DisplayAverageFilesPerCategory()
        {
            if (_statistics.TryGetValue("AverageFilesPerCategory", out var value) && value is double avg)
            {
                AddStatisticItem("カテゴリあたりの平均ファイル数", avg.ToString("F1"));
            }
        }

        /// <summary>
        /// カテゴリ別統計情報を表示します
        /// </summary>
        private void DisplayCategoryStatistics()
        {
            if (!_statistics.TryGetValue("CategoryCounts", out var value) ||
                !(value is Dictionary<string, int> categoryCounts))
            {
                return;
            }

            // セパレーターと見出しを追加
            AddSeparatorRow();
            AddStatisticItem("カテゴリ別ファイル数", "");

            // カテゴリ別のデータを表示
            foreach (var categoryData in categoryCounts)
            {
                AddStatisticItem($"  {categoryData.Key}", categoryData.Value.ToString());
            }
        }

        /// <summary>
        /// 統計項目をリストに追加します
        /// </summary>
        /// <param name="itemName">項目名</param>
        /// <param name="itemValue">項目値</param>
        private void AddStatisticItem(string itemName, string itemValue)
        {
            var listItem = new ListViewItem(new[] { itemName ?? "", itemValue ?? "" });
            _listViewStats.Items.Add(listItem);
        }

        /// <summary>
        /// セパレーター行を追加します
        /// </summary>
        private void AddSeparatorRow()
        {
            AddStatisticItem("", "");
        }
        #endregion

        #region ユーティリティメソッド
        /// <summary>
        /// ファイルサイズを読みやすい形式にフォーマットします
        /// </summary>
        /// <param name="bytes">バイト数</param>
        /// <returns>フォーマット済みのファイルサイズ文字列</returns>
        private string FormatFileSize(long bytes)
        {
            if (bytes < 0)
            {
                return "0 B";
            }

            string[] sizeUnits = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unitIndex = 0;

            while (size >= BYTES_TO_KB && unitIndex < sizeUnits.Length - 1)
            {
                unitIndex++;
                size /= BYTES_TO_KB;
            }

            return $"{size:0.##} {sizeUnits[unitIndex]}";
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// 閉じるボタンクリック時の処理
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// キー押下時の処理
        /// </summary>
        private void StatisticsDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        #endregion

        #region エラーハンドリング
        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(this, fullMessage, "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] StatisticsDialog: {message} - {ex}");
        }
        #endregion

        #region リソース管理
        /// <summary>
        /// リソースの適切な解放を行います
        /// </summary>
        /// <param name="disposing">マネージドリソースを解放する場合は true</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _listViewStats?.Dispose();
                _btnClose?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
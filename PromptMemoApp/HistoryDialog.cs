using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// 履歴表示・管理ダイアログ
    /// ユーザーのファイルアクセス履歴を表示し、選択・削除機能を提供
    /// </summary>
    public partial class HistoryDialog : Form
    {
        #region フィールド
        private readonly HistoryManager _historyManager;
        private readonly Action<string, string> _onHistorySelected;
        private List<HistoryItem> _allHistory;
        #endregion

        #region 定数
        private const string ALL_CATEGORIES = "すべて";
        private const string DIALOG_TITLE = "履歴";
        private const string CONFIRM_TITLE = "確認";
        private const string DATE_FORMAT = "yyyy/MM/dd HH:mm";
        private const int FORM_WIDTH = 700;
        private const int FORM_HEIGHT = 500;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// HistoryDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="historyManager">履歴管理クラス</param>
        /// <param name="onHistorySelected">履歴選択時のコールバック</param>
        /// <exception cref="ArgumentNullException">引数が null の場合</exception>
        public HistoryDialog(HistoryManager historyManager, Action<string, string> onHistorySelected)
        {
            _historyManager = historyManager ?? throw new ArgumentNullException(nameof(historyManager));
            _onHistorySelected = onHistorySelected ?? throw new ArgumentNullException(nameof(onHistorySelected));

            InitializeComponent();
            SetupDialog();
            LoadHistoryData();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// ダイアログの追加設定を行います
        /// </summary>
        private void SetupDialog()
        {
            // フォーム設定
            Text = DIALOG_TITLE;
            Size = new System.Drawing.Size(FORM_WIDTH, FORM_HEIGHT);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // イベントハンドラー設定
            SetupEventHandlers();
        }

        /// <summary>
        /// イベントハンドラーを設定します
        /// </summary>
        private void SetupEventHandlers()
        {
            if (comboBoxCategories != null)
            {
                comboBoxCategories.SelectedIndexChanged += ComboBoxCategories_SelectedIndexChanged;
            }

            if (listViewHistory != null)
            {
                listViewHistory.DoubleClick += ListViewHistory_DoubleClick;
                // 選択変更時にボタン状態を更新
                listViewHistory.SelectedIndexChanged += (s, e) => UpdateButtonStates();
            }

            if (btnOpen != null) btnOpen.Click += BtnOpen_Click;
            if (btnRemove != null) btnRemove.Click += BtnRemove_Click;
            if (btnClear != null) btnClear.Click += BtnClear_Click;
        }
        #endregion

        #region データ操作
        /// <summary>
        /// 履歴データを読み込みます
        /// </summary>
        private void LoadHistoryData()
        {
            try
            {
                _allHistory = _historyManager.GetRecentHistory();
                UpdateCategoryComboBox();
                UpdateHistoryListView();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("履歴データの読み込み中にエラーが発生しました。", ex);
                _allHistory = new List<HistoryItem>(); // 空リストで初期化
            }
        }

        /// <summary>
        /// カテゴリコンボボックスを更新します
        /// </summary>
        private void UpdateCategoryComboBox()
        {
            if (comboBoxCategories == null) return;

            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add(ALL_CATEGORIES);

            // カテゴリを取得してソート
            var categories = _allHistory
                .Select(h => h.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToArray();

            comboBoxCategories.Items.AddRange(categories);
            comboBoxCategories.SelectedIndex = 0; // "すべて"を選択
        }

        /// <summary>
        /// 履歴リストビューを更新します
        /// </summary>
        private void UpdateHistoryListView()
        {
            if (listViewHistory == null) return;

            listViewHistory.Items.Clear();

            var filteredHistory = GetFilteredHistory();

            foreach (var item in filteredHistory)
            {
                var listItem = CreateHistoryListItem(item);
                listViewHistory.Items.Add(listItem);
            }
        }

        /// <summary>
        /// 選択されたカテゴリに基づいて履歴をフィルタリングします
        /// </summary>
        /// <returns>フィルタリングされた履歴リスト</returns>
        private List<HistoryItem> GetFilteredHistory()
        {
            var selectedCategory = comboBoxCategories?.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == ALL_CATEGORIES)
            {
                return _allHistory;
            }

            return _historyManager.GetHistoryByCategory(selectedCategory);
        }

        /// <summary>
        /// 履歴アイテム用のリストビューアイテムを作成します
        /// </summary>
        /// <param name="historyItem">履歴アイテム</param>
        /// <returns>作成されたリストビューアイテム</returns>
        private ListViewItem CreateHistoryListItem(HistoryItem historyItem)
        {
            var listItem = new ListViewItem(historyItem.Category);
            listItem.SubItems.Add(historyItem.FileName);
            listItem.SubItems.Add(historyItem.LastAccessed.ToString(DATE_FORMAT));
            listItem.SubItems.Add(historyItem.LastModified.ToString(DATE_FORMAT));
            listItem.SubItems.Add(historyItem.AccessCount.ToString());
            listItem.Tag = historyItem;

            return listItem;
        }
        #endregion

        #region UI状態管理
        /// <summary>
        /// ボタンの有効/無効状態を更新します
        /// </summary>
        private void UpdateButtonStates()
        {
            var hasSelection = listViewHistory?.SelectedItems.Count > 0;
            var hasHistory = _allHistory?.Count > 0;

            if (btnOpen != null) btnOpen.Enabled = hasSelection;
            if (btnRemove != null) btnRemove.Enabled = hasSelection;
            if (btnClear != null) btnClear.Enabled = hasHistory;
        }
        #endregion

        #region 履歴操作
        /// <summary>
        /// 選択された履歴を開きます
        /// </summary>
        private void OpenSelectedHistory()
        {
            var selectedItem = GetSelectedHistoryItem();
            if (selectedItem != null)
            {
                _onHistorySelected(selectedItem.Category, selectedItem.FileName);
                Close();
            }
        }

        /// <summary>
        /// 選択された履歴アイテムを取得します
        /// </summary>
        /// <returns>選択された履歴アイテム、選択がない場合は null</returns>
        private HistoryItem GetSelectedHistoryItem()
        {
            if (listViewHistory?.SelectedItems.Count > 0)
            {
                return listViewHistory.SelectedItems[0].Tag as HistoryItem;
            }
            return null;
        }

        /// <summary>
        /// 選択された履歴を削除します
        /// </summary>
        private void RemoveSelectedHistory()
        {
            var selectedItem = GetSelectedHistoryItem();
            if (selectedItem == null) return;

            var message = $"「{selectedItem.FileName}」を履歴から削除しますか？";
            if (ShowConfirmationDialog(message))
            {
                try
                {
                    _historyManager.RemoveFromHistory(selectedItem.Category, selectedItem.FileName);
                    LoadHistoryData();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("履歴の削除中にエラーが発生しました。", ex);
                }
            }
        }

        /// <summary>
        /// 全履歴を削除します
        /// </summary>
        private void ClearAllHistory()
        {
            if (!ShowConfirmationDialog("すべての履歴を削除しますか？"))
            {
                return;
            }

            try
            {
                _historyManager.ClearHistory();
                LoadHistoryData();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("履歴の削除中にエラーが発生しました。", ex);
            }
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// カテゴリ選択変更時の処理
        /// </summary>
        private void ComboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateHistoryListView();
            UpdateButtonStates();
        }

        /// <summary>
        /// リストビューダブルクリック時の処理
        /// </summary>
        private void ListViewHistory_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedHistory();
        }

        /// <summary>
        /// 開くボタンクリック時の処理
        /// </summary>
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedHistory();
        }

        /// <summary>
        /// 削除ボタンクリック時の処理
        /// </summary>
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedHistory();
        }

        /// <summary>
        /// 全削除ボタンクリック時の処理
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearAllHistory();
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// 確認ダイアログを表示します
        /// </summary>
        /// <param name="message">確認メッセージ</param>
        /// <returns>ユーザーがYesを選択した場合は true</returns>
        private bool ShowConfirmationDialog(string message)
        {
            return MessageBox.Show(this, message, CONFIRM_TITLE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

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

            // デバッグ用ログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] HistoryDialog: {message} - {ex}");
        }
        #endregion

        #region デザイナー生成コード（簡略化）
        private void InitializeComponent()
        {
            SuspendLayout();
            CreateControls();
            SetupControlLayout();
            SetupControlProperties();
            AddControlsToForm();
            ResumeLayout(false);
        }

        /// <summary>
        /// コントロールを作成します
        /// </summary>
        private void CreateControls()
        {
            comboBoxCategories = new ComboBox();
            listViewHistory = new ListView();
            btnOpen = new Button();
            btnRemove = new Button();
            btnClear = new Button();
            btnClose = new Button();
        }

        /// <summary>
        /// コントロールのレイアウトを設定します
        /// </summary>
        private void SetupControlLayout()
        {
            // カテゴリコンボボックス
            comboBoxCategories.Location = new System.Drawing.Point(10, 10);
            comboBoxCategories.Size = new System.Drawing.Size(200, 20);

            // リストビュー
            listViewHistory.Location = new System.Drawing.Point(10, 40);
            listViewHistory.Size = new System.Drawing.Size(660, 370);

            // ボタン配置
            btnOpen.Location = new System.Drawing.Point(10, 420);
            btnOpen.Size = new System.Drawing.Size(80, 25);

            btnRemove.Location = new System.Drawing.Point(100, 420);
            btnRemove.Size = new System.Drawing.Size(80, 25);

            btnClear.Location = new System.Drawing.Point(190, 420);
            btnClear.Size = new System.Drawing.Size(80, 25);

            btnClose.Location = new System.Drawing.Point(590, 420);
            btnClose.Size = new System.Drawing.Size(80, 25);
        }

        /// <summary>
        /// コントロールのプロパティを設定します
        /// </summary>
        private void SetupControlProperties()
        {
            // コンボボックス設定
            comboBoxCategories.DropDownStyle = ComboBoxStyle.DropDownList;

            // リストビュー設定
            listViewHistory.View = View.Details;
            listViewHistory.FullRowSelect = true;
            listViewHistory.GridLines = true;
            SetupListViewColumns();

            // ボタン設定
            btnOpen.Text = "開く";
            btnRemove.Text = "削除";
            btnClear.Text = "全削除";
            btnClose.Text = "閉じる";
            btnClose.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// リストビューのカラムを設定します
        /// </summary>
        private void SetupListViewColumns()
        {
            listViewHistory.Columns.Add("カテゴリ", 100);
            listViewHistory.Columns.Add("ファイル名", 200);
            listViewHistory.Columns.Add("最終アクセス", 120);
            listViewHistory.Columns.Add("最終更新", 120);
            listViewHistory.Columns.Add("アクセス回数", 80);
        }

        /// <summary>
        /// コントロールをフォームに追加します
        /// </summary>
        private void AddControlsToForm()
        {
            Controls.AddRange(new Control[] {
                comboBoxCategories,
                listViewHistory,
                btnOpen,
                btnRemove,
                btnClear,
                btnClose
            });
        }

        // コントロールフィールド
        private ComboBox comboBoxCategories;
        private ListView listViewHistory;
        private Button btnOpen;
        private Button btnRemove;
        private Button btnClear;
        private Button btnClose;
        #endregion
    }
}
using System;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// 新規ファイル作成用のダイアログフォーム
    /// </summary>
    public partial class NewFileForm : Form
    {
        #region フィールド
        // UI コントロールはデザイナーファイルで定義済み
        // txtFileName, comboBoxCategory, btnSave, btnCancel
        #endregion

        #region プロパティ
        /// <summary>
        /// 入力されたファイル名を取得
        /// </summary>
        public string FileName { get; private set; } = string.Empty;

        /// <summary>
        /// 選択されたカテゴリ名を取得
        /// </summary>
        public string SelectedCategory { get; private set; } = string.Empty;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// NewFileFormの新しいインスタンスを初期化
        /// </summary>
        /// <param name="categories">選択可能なカテゴリ一覧</param>
        public NewFileForm(string[] categories)
        {
            InitializeComponent();
            ConfigureForm(categories);
        }
        #endregion

        #region 初期化
        /// <summary>
        /// フォームの初期設定を行う
        /// </summary>
        /// <param name="categories">カテゴリ一覧</param>
        private void ConfigureForm(string[] categories)
        {
            PopulateCategoryComboBox(categories);
            SetupInitialFocus();
        }

        /// <summary>
        /// カテゴリコンボボックスにデータを設定
        /// </summary>
        /// <param name="categories">カテゴリ一覧</param>
        private void PopulateCategoryComboBox(string[] categories)
        {
            if (categories == null || categories.Length == 0)
            {
                ShowErrorMessage("利用可能なカテゴリがありません。");
                return;
            }

            comboBoxCategory.Items.Clear();
            comboBoxCategory.Items.AddRange(categories);
            comboBoxCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// 初期フォーカスを設定
        /// </summary>
        private void SetupInitialFocus()
        {
            txtFileName.Focus();
        }
        #endregion

        #region イベントハンドラー
        /// <summary>
        /// 保存ボタンクリック時の処理
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SaveFileInfo();
                AcceptDialog();
            }
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelDialog();
        }

        /// <summary>
        /// フォームが読み込まれた時の処理
        /// </summary>
        private void NewFileForm_Load(object sender, EventArgs e)
        {
            SetupInitialFocus();
        }

        /// <summary>
        /// ファイル名テキストボックスでエンターキーが押された時の処理
        /// </summary>
        private void txtFileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave_Click(sender, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// キー押下時の処理（エスケープキーでキャンセル）
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                btnCancel_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region バリデーション
        /// <summary>
        /// 入力値の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateInput()
        {
            return ValidateFileName() && ValidateCategory();
        }

        /// <summary>
        /// ファイル名の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateFileName()
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                ShowValidationError("ファイル名を入力してください。");
                txtFileName.Focus();
                return false;
            }

            if (ContainsInvalidFileNameCharacters(txtFileName.Text))
            {
                ShowValidationError("ファイル名に使用できない文字が含まれています。");
                txtFileName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// カテゴリ選択の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateCategory()
        {
            if (comboBoxCategory.SelectedItem == null)
            {
                ShowValidationError("カテゴリを選択してください。");
                comboBoxCategory.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// ファイル名に無効な文字が含まれているかチェック
        /// </summary>
        /// <param name="fileName">チェック対象のファイル名</param>
        /// <returns>無効な文字が含まれている場合はtrue</returns>
        private bool ContainsInvalidFileNameCharacters(string fileName)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return fileName.Any(c => invalidChars.Contains(c));
        }
        #endregion

        #region データ保存
        /// <summary>
        /// 入力されたファイル情報を保存
        /// </summary>
        private void SaveFileInfo()
        {
            FileName = txtFileName.Text.Trim();
            SelectedCategory = comboBoxCategory.SelectedItem?.ToString() ?? string.Empty;
        }
        #endregion

        #region ダイアログ終了処理
        /// <summary>
        /// ダイアログをOKで終了
        /// </summary>
        private void AcceptDialog()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// ダイアログをキャンセルで終了
        /// </summary>
        private void CancelDialog()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region メッセージ表示
        /// <summary>
        /// バリデーションエラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "入力エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region 静的メソッド（便利メソッド）
        /// <summary>
        /// 新規ファイル作成ダイアログを表示
        /// </summary>
        /// <param name="categories">選択可能なカテゴリ一覧</param>
        /// <returns>作成情報。キャンセルされた場合はnull</returns>
        public static NewFileInfo ShowDialog(string[] categories)
        {
            using (var dialog = new NewFileForm(categories))
            {
                return dialog.ShowDialog() == DialogResult.OK
                    ? new NewFileInfo(dialog.FileName, dialog.SelectedCategory)
                    : null;
            }
        }

        /// <summary>
        /// 親フォームを指定して新規ファイル作成ダイアログを表示
        /// </summary>
        /// <param name="owner">親フォーム</param>
        /// <param name="categories">選択可能なカテゴリ一覧</param>
        /// <returns>作成情報。キャンセルされた場合はnull</returns>
        public static NewFileInfo ShowDialog(IWin32Window owner, string[] categories)
        {
            using (var dialog = new NewFileForm(categories))
            {
                return dialog.ShowDialog(owner) == DialogResult.OK
                    ? new NewFileInfo(dialog.FileName, dialog.SelectedCategory)
                    : null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 新規ファイル作成情報を保持するクラス
    /// </summary>
    public class NewFileInfo
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// NewFileInfoの新しいインスタンスを初期化
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="category">カテゴリ名</param>
        public NewFileInfo(string fileName, string category)
        {
            FileName = fileName ?? string.Empty;
            Category = category ?? string.Empty;
        }
    }
}
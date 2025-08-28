using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// テキスト入力用のダイアログフォーム
    /// </summary>
    public partial class InputDialog : Form
    {
        #region フィールド
        // UI コントロールはデザイナーファイル（InputDialog.Designer.cs）で定義済み
        // lblMessage, txtInput, btnOK, btnCancel
        #endregion

        #region プロパティ
        /// <summary>
        /// 入力されたテキストを取得
        /// </summary>
        public string InputText => txtInput.Text.Trim();
        #endregion

        #region コンストラクタ
        /// <summary>
        /// InputDialogの新しいインスタンスを初期化
        /// </summary>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="message">ユーザーに表示するメッセージ</param>
        /// <param name="defaultValue">デフォルト値</param>
        public InputDialog(string title, string message, string defaultValue)
        {
            InitializeComponent();
            ConfigureDialog(title, message, defaultValue);
        }
        #endregion

        #region 初期化
        /// <summary>
        /// ダイアログの設定を行う
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="defaultValue">デフォルト値</param>
        private void ConfigureDialog(string title, string message, string defaultValue)
        {
            this.Text = title;
            lblMessage.Text = message;
            txtInput.Text = defaultValue ?? string.Empty;

            // テキストボックスにフォーカスを設定
            txtInput.Focus();

            // デフォルト値がある場合は全選択
            if (!string.IsNullOrEmpty(defaultValue))
            {
                txtInput.SelectAll();
            }
        }
        #endregion

        #region イベントハンドラー
        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
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
        private void InputDialog_Load(object sender, EventArgs e)
        {
            // テキストボックスにフォーカスを設定
            txtInput.Focus();
        }

        /// <summary>
        /// キー押下時の処理（エンターキーでOK、エスケープキーでキャンセル）
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    btnOK_Click(null, null);
                    return true;
                case Keys.Escape:
                    btnCancel_Click(null, null);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        #endregion

        #region バリデーション
        /// <summary>
        /// 入力値の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                ShowValidationError("値を入力してください。");
                txtInput.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// バリデーションエラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "入力エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #region 静的メソッド（便利メソッド）
        /// <summary>
        /// 簡単にInputDialogを表示するヘルパーメソッド
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>入力された文字列。キャンセルされた場合はnull</returns>
        public static string ShowDialog(string title, string message, string defaultValue = "")
        {
            using (var dialog = new InputDialog(title, message, defaultValue))
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.InputText : null;
            }
        }

        /// <summary>
        /// 親フォームを指定してInputDialogを表示
        /// </summary>
        /// <param name="owner">親フォーム</param>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>入力された文字列。キャンセルされた場合はnull</returns>
        public static string ShowDialog(IWin32Window owner, string title, string message, string defaultValue = "")
        {
            using (var dialog = new InputDialog(title, message, defaultValue))
            {
                return dialog.ShowDialog(owner) == DialogResult.OK ? dialog.InputText : null;
            }
        }
        #endregion
    }
}
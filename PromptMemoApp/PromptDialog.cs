using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// ユーザー入力を求めるダイアログ
    /// テキスト入力とOK/Cancelボタンを持つシンプルなダイアログ
    /// </summary>
    public partial class PromptDialog : Form
    {
        #region プロパティ
        /// <summary>
        /// ユーザーが入力したテキストを取得します
        /// </summary>
        public string InputText => txtInput?.Text ?? string.Empty;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// PromptDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="message">ユーザーに表示するメッセージ</param>
        /// <exception cref="ArgumentException">message が null または空の場合</exception>
        public PromptDialog(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("メッセージは必須です。", nameof(message));

            InitializeComponent();
            SetupEnhancements();
            SetMessage(message);
        }

        /// <summary>
        /// カスタムタイトルとメッセージを指定してPromptDialogを初期化します
        /// </summary>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="message">ユーザーに表示するメッセージ</param>
        /// <param name="defaultValue">入力フィールドの初期値（オプション）</param>
        public PromptDialog(string title, string message, string defaultValue = "") : this(message)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Text = title;
            }

            if (!string.IsNullOrEmpty(defaultValue))
            {
                SetDefaultValue(defaultValue);
            }
        }
        #endregion

        #region 初期化・拡張設定
        /// <summary>
        /// Designerで作成されたコントロールに追加の設定を行います
        /// </summary>
        private void SetupEnhancements()
        {
            // フォーム設定の拡張
            SetupFormEnhancements();

            // コントロール設定の拡張
            SetupControlEnhancements();

            // イベントハンドラーの追加設定
            SetupAdditionalEventHandlers();
        }

        /// <summary>
        /// フォームの追加設定を行います
        /// </summary>
        private void SetupFormEnhancements()
        {
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        /// <summary>
        /// コントロールの追加設定を行います
        /// </summary>
        private void SetupControlEnhancements()
        {
            // テキストボックスの設定
            if (txtInput != null)
            {
                txtInput.Multiline = false;
            }

            // ボタンの追加設定
            if (btnOK != null)
            {
                btnOK.DialogResult = DialogResult.OK;
            }

            if (btnCancel != null)
            {
                btnCancel.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// 追加のイベントハンドラーを設定します
        /// </summary>
        private void SetupAdditionalEventHandlers()
        {
            // フォーム表示時にテキストボックスにフォーカスを設定
            Shown += (s, e) => txtInput?.Focus();
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// 表示メッセージを設定します
        /// </summary>
        /// <param name="message">設定するメッセージ</param>
        public void SetMessage(string message)
        {
            if (lblMessage != null)
            {
                lblMessage.Text = message ?? string.Empty;
            }
        }

        /// <summary>
        /// 入力フィールドの初期値を設定します
        /// </summary>
        /// <param name="value">設定する初期値</param>
        public void SetDefaultValue(string value)
        {
            if (txtInput != null)
            {
                txtInput.Text = value ?? string.Empty;
                txtInput.SelectAll(); // 全選択状態にする
            }
        }

        /// <summary>
        /// 入力フィールドの最大文字数を設定します
        /// </summary>
        /// <param name="maxLength">最大文字数</param>
        public void SetMaxLength(int maxLength)
        {
            if (txtInput != null && maxLength > 0)
            {
                txtInput.MaxLength = maxLength;
            }
        }

        /// <summary>
        /// 入力フィールドを読み取り専用に設定します
        /// </summary>
        /// <param name="readOnly">読み取り専用にする場合は true</param>
        public void SetReadOnly(bool readOnly)
        {
            if (txtInput != null)
            {
                txtInput.ReadOnly = readOnly;
                if (readOnly)
                {
                    txtInput.BackColor = System.Drawing.SystemColors.Control;
                }
            }
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// 入力値の妥当性を検証します
        /// </summary>
        /// <returns>入力が有効な場合は true</returns>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtInput?.Text))
            {
                ShowValidationError("入力してください。");
                txtInput?.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// バリデーションエラーメッセージを表示します
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(this, message, "入力エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region 静的ファクトリーメソッド
        /// <summary>
        /// シンプルな入力ダイアログを表示します
        /// </summary>
        /// <param name="owner">親ウィンドウ</param>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="defaultValue">初期値</param>
        /// <returns>入力された文字列、キャンセル時は null</returns>
        public static string ShowDialog(IWin32Window owner, string message,
            string title = "入力", string defaultValue = "")
        {
            using (var dialog = new PromptDialog(title, message, defaultValue))
            {
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.InputText;
                }
                return null;
            }
        }

        /// <summary>
        /// 確認用の読み取り専用ダイアログを表示します
        /// </summary>
        /// <param name="owner">親ウィンドウ</param>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="content">表示する内容</param>
        /// <param name="title">ダイアログタイトル</param>
        /// <returns>OKが押された場合は true</returns>
        public static bool ShowConfirmDialog(IWin32Window owner, string message,
            string content, string title = "確認")
        {
            using (var dialog = new PromptDialog(title, message, content))
            {
                dialog.SetReadOnly(true);
                return dialog.ShowDialog(owner) == DialogResult.OK;
            }
        }
        #endregion
    }
}
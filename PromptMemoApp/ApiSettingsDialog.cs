using System;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// API設定ダイアログ
    /// TranslationManagerのAPIキー設定を行う
    /// </summary>
    public partial class ApiSettingsDialog : Form
    {
        #region フィールド
        private readonly TranslationManager _translationManager;
        #endregion

        #region 定数
        private const string MASKED_API_KEY = "********";
        private const string SETTINGS_TITLE = "設定";
        private const string ERROR_TITLE = "エラー";
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ApiSettingsDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="translationManager">翻訳管理クラス</param>
        /// <exception cref="ArgumentNullException">translationManager が null の場合</exception>
        public ApiSettingsDialog(TranslationManager translationManager)
        {
            _translationManager = translationManager ?? throw new ArgumentNullException(nameof(translationManager));

            InitializeComponent();
            SetupDialog();
            LoadCurrentApiKeyStatus();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// ダイアログの追加設定を行います
        /// </summary>
        private void SetupDialog()
        {
            // フォーム設定
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // テキストボックスの設定（セキュリティ強化）
            if (txtApiKey != null)
            {
                txtApiKey.UseSystemPasswordChar = true; // パスワード文字で表示
                txtApiKey.MaxLength = 256; // 最大長制限
            }

            // 追加のイベントハンドラー設定
            SetupAdditionalEventHandlers();
        }

        /// <summary>
        /// 追加のイベントハンドラーを設定します
        /// </summary>
        private void SetupAdditionalEventHandlers()
        {
            // フォーム表示時にテキストボックスにフォーカス
            Shown += (s, e) => txtApiKey?.Focus();

            // Enterキーで保存
            if (btnSave != null)
            {
                AcceptButton = btnSave;
            }

            // Escapeキーでキャンセル（btnCancelが存在する場合のみ）
            var cancelButton = Controls.Find("btnCancel", true).FirstOrDefault() as Button;
            if (cancelButton != null)
            {
                CancelButton = cancelButton;
            }
        }
        #endregion

        #region データ操作
        /// <summary>
        /// 現在のAPIキー状態を読み込みます
        /// </summary>
        private void LoadCurrentApiKeyStatus()
        {
            try
            {
                if (txtApiKey != null)
                {
                    // セキュリティ: 既存のAPIキーがある場合はマスク表示
                    txtApiKey.Text = _translationManager.HasApiKey ? MASKED_API_KEY : string.Empty;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("APIキー状態の読み込み中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// APIキーを保存します
        /// </summary>
        /// <returns>保存が成功した場合は true</returns>
        private bool SaveApiKey()
        {
            try
            {
                var apiKey = txtApiKey?.Text?.Trim();

                if (!ValidateApiKey(apiKey))
                {
                    return false;
                }

                // マスク表示の場合は既存キーを保持
                if (apiKey == MASKED_API_KEY)
                {
                    ShowSuccessMessage("既存のAPIキーを保持します。");
                    return true;
                }

                // 新しいAPIキーを設定
                _translationManager.SetApiKey(apiKey);

                System.Diagnostics.Debug.WriteLine("[DEBUG] APIキーを保存しました。");
                ShowSuccessMessage("APIキーを保存しました。");

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("APIキーの保存中にエラーが発生しました。", ex);
                return false;
            }
        }
        #endregion

        #region バリデーション
        /// <summary>
        /// APIキーの妥当性を検証します
        /// </summary>
        /// <param name="apiKey">検証するAPIキー</param>
        /// <returns>有効な場合は true</returns>
        private bool ValidateApiKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ShowValidationError("APIキーを入力してください。");
                txtApiKey?.Focus();
                return false;
            }

            // 基本的な形式チェック
            if (apiKey.Length < 10)
            {
                ShowValidationError("APIキーが短すぎます。正しいAPIキーを入力してください。");
                txtApiKey?.Focus();
                return false;
            }

            // 不正文字のチェック
            if (apiKey.Contains(" ") || apiKey.Contains("\t") || apiKey.Contains("\n"))
            {
                ShowValidationError("APIキーに無効な文字が含まれています。");
                txtApiKey?.Focus();
                return false;
            }

            return true;
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// 保存ボタンクリック時の処理
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveApiKey())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// APIキーテキストボックスのフォーカス取得時の処理
        /// </summary>
        private void txtApiKey_Enter(object sender, EventArgs e)
        {
            // マスク表示の場合は入力時にクリア
            if (txtApiKey?.Text == MASKED_API_KEY)
            {
                txtApiKey.Text = string.Empty;
            }
        }

        /// <summary>
        /// APIキーテキストボックスのフォーカス喪失時の処理
        /// </summary>
        private void txtApiKey_Leave(object sender, EventArgs e)
        {
            // 空欄で既存APIキーがある場合はマスク表示に戻す
            if (string.IsNullOrWhiteSpace(txtApiKey?.Text) && _translationManager.HasApiKey)
            {
                txtApiKey.Text = MASKED_API_KEY;
            }
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// 成功メッセージを表示します
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(this, message, SETTINGS_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// バリデーションエラーメッセージを表示します
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(this, message, ERROR_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(this, fullMessage, ERROR_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}: {ex}");
        }

        /// <summary>
        /// リソースの適切な解放を行います
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // セキュリティ: メモリからAPIキーを消去
                if (txtApiKey != null)
                {
                    txtApiKey.Text = string.Empty;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 削除推奨メソッド
        // 注意: このメソッドは不要で混乱を招く可能性があるため削除を推奨
        // private void BtnApiSettings_Click(object sender, EventArgs e)
        // {
        //     // この処理は ApiSettingsDialog 内部で ApiSettingsDialog を開いており、
        //     // 無限ループやメモリリークの原因となる可能性があります
        // }
        #endregion
    }
}
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// 翻訳機能のダイアログフォーム
    /// </summary>
    public partial class TranslationDialog : Form
    {
        #region フィールド
        private readonly TranslationManager translationManager;
        private readonly string originalText;

        // UI コントロール
        private ComboBox comboBoxSourceLang;
        private ComboBox comboBoxTargetLang;
        private Button btnTranslate;
        private Button btnDetect;
        private Button btnApiSettings;
        private TextBox txtOriginal;
        private TextBox txtTranslated;
        private Button btnCopy;
        private Button btnReplace;
        private Button btnClose;
        private Label lblStatus;
        private Label lblOriginal;
        private Label lblTranslated;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// TranslationDialogの新しいインスタンスを初期化
        /// </summary>
        /// <param name="translationManager">翻訳マネージャー</param>
        /// <param name="text">翻訳対象のテキスト</param>
        public TranslationDialog(TranslationManager translationManager, string text)
        {
            this.translationManager = translationManager;
            this.originalText = text;
            InitializeComponent();
            LoadOriginalText();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// フォームのコンポーネントを初期化
        /// </summary>
        private void InitializeComponent()
        {
            CreateLabels();
            CreateComboBoxes();
            CreateTextBoxes();
            CreateButtons();
            SetupFormLayout();
        }

        /// <summary>
        /// ラベルコントロールを作成
        /// </summary>
        private void CreateLabels()
        {
            lblOriginal = CreateLabel("元のテキスト：", 10, 40);
            lblTranslated = CreateLabel("翻訳結果：", 10, 225);
            lblStatus = CreateLabel("翻訳する言語を選択して「翻訳」ボタンをクリックしてください。", 10, 450, 610);
        }

        /// <summary>
        /// コンボボックスを作成
        /// </summary>
        private void CreateComboBoxes()
        {
            comboBoxSourceLang = CreateLanguageComboBox(115, 40, 0); // 日本語を初期選択
            comboBoxTargetLang = CreateLanguageComboBox(115, 225, 1); // 英語を初期選択
        }

        /// <summary>
        /// テキストボックスを作成
        /// </summary>
        private void CreateTextBoxes()
        {
            txtOriginal = CreateTextBox(10, 65, true);  // 読み取り専用
            txtTranslated = CreateTextBox(10, 250, false); // 編集可能
        }

        /// <summary>
        /// ボタンを作成
        /// </summary>
        private void CreateButtons()
        {
            btnTranslate = CreateButton("翻訳", 330, 10, BtnTranslate_Click);
            btnDetect = CreateButton("言語検出", 420, 10, BtnDetect_Click, false);
            btnApiSettings = CreateButton("API設定", 510, 10, BtnApiSettings_Click);
            btnCopy = CreateButton("コピー", 10, 420, BtnCopy_Click);
            btnReplace = CreateButton("置き換え", 100, 420, BtnReplace_Click);
            btnClose = CreateCloseButton("閉じる", 540, 420);
        }

        /// <summary>
        /// フォームのレイアウトを設定
        /// </summary>
        private void SetupFormLayout()
        {
            this.SuspendLayout();

            // コントロールをフォームに追加
            this.Controls.AddRange(new Control[] {
                lblOriginal, lblTranslated, lblStatus,
                comboBoxSourceLang, comboBoxTargetLang,
                txtOriginal, txtTranslated,
                btnTranslate, btnDetect, btnApiSettings,
                btnCopy, btnReplace, btnClose
            });

            // フォームの基本設定
            this.ClientSize = new System.Drawing.Size(632, 501);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TranslationDialog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "翻訳";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region UIコントロール作成ヘルパー
        /// <summary>
        /// ラベルを作成
        /// </summary>
        private Label CreateLabel(string text, int x, int y, int width = 100)
        {
            return new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, 20),
                AutoSize = false
            };
        }

        /// <summary>
        /// 言語選択用コンボボックスを作成
        /// </summary>
        private ComboBox CreateLanguageComboBox(int x, int y, int selectedIndex)
        {
            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(120, 23)
            };

            comboBox.Items.AddRange(new object[] {
                "日本語 (JA)",
                "英語 (EN)"
            });
            comboBox.SelectedIndex = selectedIndex;

            return comboBox;
        }

        /// <summary>
        /// テキストボックスを作成
        /// </summary>
        private TextBox CreateTextBox(int x, int y, bool readOnly)
        {
            return new TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(610, 150),
                Multiline = true,
                ReadOnly = readOnly,
                ScrollBars = ScrollBars.Vertical
            };
        }

        /// <summary>
        /// ボタンを作成
        /// </summary>
        private Button CreateButton(string text, int x, int y, EventHandler clickHandler, bool enabled = true)
        {
            var button = new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(80, 25),
                Enabled = enabled
            };
            button.Click += clickHandler;
            return button;
        }

        /// <summary>
        /// 閉じるボタンを作成
        /// </summary>
        private Button CreateCloseButton(string text, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(80, 25),
                DialogResult = DialogResult.Cancel
            };
        }
        #endregion

        #region 初期データ読み込み
        /// <summary>
        /// 元のテキストを表示
        /// </summary>
        private void LoadOriginalText()
        {
            txtOriginal.Text = originalText;
        }
        #endregion

        #region イベントハンドラー
        /// <summary>
        /// 翻訳ボタンクリック時の処理
        /// </summary>
        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            if (!ValidateTranslationInput())
                return;

            await PerformTranslation();
        }

        /// <summary>
        /// 言語検出ボタンクリック時の処理
        /// </summary>
        private async void BtnDetect_Click(object sender, EventArgs e)
        {
            if (!ValidateDetectionInput())
                return;

            await PerformLanguageDetection();
        }

        /// <summary>
        /// コピーボタンクリック時の処理
        /// </summary>
        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTranslated.Text))
                return;

            Clipboard.SetText(txtTranslated.Text);
            ShowInformationMessage("翻訳結果をクリップボードにコピーしました。", "コピー");
        }

        /// <summary>
        /// 置き換えボタンクリック時の処理
        /// </summary>
        private void BtnReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTranslated.Text))
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// API設定ボタンクリック時の処理
        /// </summary>
        private void BtnApiSettings_Click(object sender, EventArgs e)
        {
            using (var dialog = new ApiSettingsDialog(translationManager))
            {
                dialog.ShowDialog(this);
            }
        }
        #endregion

        #region バリデーション
        /// <summary>
        /// 翻訳入力の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateTranslationInput()
        {
            if (string.IsNullOrEmpty(txtOriginal.Text))
            {
                ShowWarningMessage("翻訳するテキストがありません。", "翻訳");
                return false;
            }

            if (!translationManager.HasApiKey)
            {
                ShowWarningMessage("DeepL APIキーが設定されていません。", "翻訳");
                return false;
            }

            if (GetSourceLanguage() == GetTargetLanguage())
            {
                ShowInformationMessage("元言語と翻訳後の言語が同じです。", "翻訳");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 言語検出入力の妥当性をチェック
        /// </summary>
        /// <returns>妥当な場合はtrue</returns>
        private bool ValidateDetectionInput()
        {
            if (!translationManager.HasApiKey)
            {
                ShowWarningMessage("DeepL APIキーが設定されていません。\n設定からAPIキーを入力してください。", "エラー");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtOriginal.Text))
            {
                ShowWarningMessage("言語を検出するテキストがありません。", "エラー");
                return false;
            }

            return true;
        }
        #endregion

        #region 翻訳・言語検出処理
        /// <summary>
        /// 翻訳を実行
        /// </summary>
        private async Task PerformTranslation()
        {
            try
            {
                string sourceLang = GetSourceLanguage();
                string targetLang = GetTargetLanguage();

                string translatedText = await translationManager.TranslateAsync(
                    txtOriginal.Text, sourceLang, targetLang);

                txtTranslated.Text = translatedText;
                System.Diagnostics.Debug.WriteLine($"[DEBUG] 翻訳結果: {translatedText}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "翻訳エラー");
            }
        }

        /// <summary>
        /// 言語検出を実行
        /// </summary>
        private async Task PerformLanguageDetection()
        {
            SetDetectionInProgress(true);

            try
            {
                var detectedLanguage = await translationManager.DetectLanguageAsync(txtOriginal.Text);
                var languageName = GetLanguageName(detectedLanguage);
                lblStatus.Text = $"検出された言語: {languageName} ({detectedLanguage})";
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"言語検出中にエラーが発生しました: {ex.Message}", "エラー");
                lblStatus.Text = "言語検出中にエラーが発生しました";
            }
            finally
            {
                SetDetectionInProgress(false);
            }
        }

        /// <summary>
        /// 言語検出の進行状態を設定
        /// </summary>
        /// <param name="inProgress">検出中の場合はtrue</param>
        private void SetDetectionInProgress(bool inProgress)
        {
            btnDetect.Enabled = !inProgress;
            lblStatus.Text = inProgress ? "言語検出中..." : "準備完了";
            Application.DoEvents();
        }
        #endregion

        #region 言語関連ユーティリティ
        /// <summary>
        /// 元言語のコードを取得
        /// </summary>
        /// <returns>言語コード</returns>
        public string GetSourceLanguage()
        {
            var selectedText = comboBoxSourceLang.SelectedItem?.ToString();
            return selectedText?.Contains("日本語") == true ? "JA" : "EN";
        }

        /// <summary>
        /// 翻訳先言語のコードを取得
        /// </summary>
        /// <returns>言語コード</returns>
        public string GetTargetLanguage()
        {
            var selectedText = comboBoxTargetLang.SelectedItem?.ToString();
            return selectedText?.Contains("日本語") == true ? "JA" : "EN";
        }

        /// <summary>
        /// 言語コードから言語名を取得
        /// </summary>
        /// <param name="languageCode">言語コード</param>
        /// <returns>言語名</returns>
        private string GetLanguageName(string languageCode)
        {
            return languageCode switch
            {
                "EN" => "英語",
                "DE" => "ドイツ語",
                "FR" => "フランス語",
                "ES" => "スペイン語",
                "IT" => "イタリア語",
                "PT" => "ポルトガル語",
                "RU" => "ロシア語",
                "ZH" => "中国語",
                "KO" => "韓国語",
                "JA" => "日本語",
                _ => languageCode
            };
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// 翻訳されたテキストを取得
        /// </summary>
        /// <returns>翻訳されたテキスト</returns>
        public string GetTranslatedText()
        {
            return txtTranslated.Text;
        }
        #endregion

        #region メッセージ表示ユーティリティ
        /// <summary>
        /// 情報メッセージを表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        private void ShowInformationMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        private void ShowWarningMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        private void ShowErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region 削除された不要なメソッド
        // 以下のメソッドは使用されていないか、重複していたため削除しました：
        // - GetTargetLanguageCode() - GetTargetLanguage()で代替
        // - LoadText() - LoadOriginalText()で代替して名前を明確化
        #endregion
    }
}
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class TranslationDialog : Form
    {
        private TranslationManager translationManager;
        private Label lblOriginal;
        private Label lblTranslated;
        private string originalText;

        public TranslationDialog(TranslationManager translationManager, string text)
        {
            this.translationManager = translationManager;
            this.originalText = text;
            InitializeComponent();
            LoadText();
        }

        private void InitializeComponent()
        {
            // --- 元のテキストラベルと元言語ComboBox ---
            this.lblOriginal = new System.Windows.Forms.Label();
            this.lblOriginal.Location = new System.Drawing.Point(10, 40);
            this.lblOriginal.Name = "lblOriginal";
            this.lblOriginal.Size = new System.Drawing.Size(100, 20);
            this.lblOriginal.TabIndex = 5;
            this.lblOriginal.Text = "元のテキスト：";

            this.comboBoxSourceLang = new System.Windows.Forms.ComboBox();
            this.comboBoxSourceLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSourceLang.Items.AddRange(new object[] {
        "日本語 (JA)",
        "英語 (EN)"
    });
            this.comboBoxSourceLang.Location = new System.Drawing.Point(115, 40); // ← lblOriginalの右隣に配置
            this.comboBoxSourceLang.Name = "comboBoxSourceLang";
            this.comboBoxSourceLang.Size = new System.Drawing.Size(120, 23);
            this.comboBoxSourceLang.TabIndex = 6;
            this.comboBoxSourceLang.SelectedIndex = 0;

            // --- 翻訳結果ラベルと翻訳後言語ComboBox ---
            this.lblTranslated = new System.Windows.Forms.Label();
            this.lblTranslated.Location = new System.Drawing.Point(10, 225);
            this.lblTranslated.Name = "lblTranslated";
            this.lblTranslated.Size = new System.Drawing.Size(100, 20);
            this.lblTranslated.TabIndex = 7;
            this.lblTranslated.Text = "翻訳結果：";

            this.comboBoxTargetLang = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTargetLang.Items.AddRange(new object[] {
        "日本語 (JA)",
        "英語 (EN)"
    });
            this.comboBoxTargetLang.Location = new System.Drawing.Point(115, 225); // ← lblTranslatedの右隣に配置
            this.comboBoxTargetLang.Name = "comboBoxTargetLang";
            this.comboBoxTargetLang.Size = new System.Drawing.Size(120, 23);
            this.comboBoxTargetLang.TabIndex = 8;
            this.comboBoxTargetLang.SelectedIndex = 1;

            // --- テキストボックスの位置も右にずらす ---
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.txtOriginal.Location = new System.Drawing.Point(10, 65);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.ReadOnly = true;
            this.txtOriginal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOriginal.Size = new System.Drawing.Size(610, 150);
            this.txtOriginal.TabIndex = 9;

            this.txtTranslated = new System.Windows.Forms.TextBox();
            this.txtTranslated.Location = new System.Drawing.Point(10, 250);
            this.txtTranslated.Multiline = true;
            this.txtTranslated.Name = "txtTranslated";
            this.txtTranslated.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTranslated.Size = new System.Drawing.Size(610, 150);
            this.txtTranslated.TabIndex = 10;

            this.btnTranslate = new System.Windows.Forms.Button();
            this.btnDetect = new System.Windows.Forms.Button();
            this.btnApiSettings = new System.Windows.Forms.Button();
            this.lblOriginal = new System.Windows.Forms.Label();
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.lblTranslated = new System.Windows.Forms.Label();
            this.txtTranslated = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(330, 10);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(80, 25);
            this.btnTranslate.TabIndex = 2;
            this.btnTranslate.Text = "翻訳";
            this.btnTranslate.Click += new System.EventHandler(this.BtnTranslate_Click);
            // 
            // btnDetect
            // 
            this.btnDetect.Location = new System.Drawing.Point(420, 10);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(80, 25);
            this.btnDetect.TabIndex = 3;
            this.btnDetect.Text = "言語検出";
            this.btnDetect.Enabled = false;
            this.btnDetect.Click += new System.EventHandler(this.BtnDetect_Click);
            // 
            // btnApiSettings
            // 
            this.btnApiSettings.Location = new System.Drawing.Point(510, 10);
            this.btnApiSettings.Name = "btnApiSettings";
            this.btnApiSettings.Size = new System.Drawing.Size(80, 25);
            this.btnApiSettings.TabIndex = 4;
            this.btnApiSettings.Text = "API設定";
            this.btnApiSettings.Click += new System.EventHandler(this.BtnApiSettings_Click);
            // 
            // lblOriginal
            // 
            this.lblOriginal.Location = new System.Drawing.Point(10, 40);
            this.lblOriginal.Name = "lblOriginal";
            this.lblOriginal.Size = new System.Drawing.Size(100, 20);
            this.lblOriginal.TabIndex = 5;
            this.lblOriginal.Text = "元のテキスト：";
            // 
            // txtOriginal
            // 
            this.txtOriginal.Location = new System.Drawing.Point(10, 65);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.ReadOnly = true;
            this.txtOriginal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOriginal.Size = new System.Drawing.Size(610, 150);
            this.txtOriginal.TabIndex = 6;
            // 
            // lblTranslated
            // 
            this.lblTranslated.Location = new System.Drawing.Point(10, 225);
            this.lblTranslated.Name = "lblTranslated";
            this.lblTranslated.Size = new System.Drawing.Size(100, 20);
            this.lblTranslated.TabIndex = 7;
            this.lblTranslated.Text = "翻訳結果：";
            // 
            // txtTranslated
            // 
            this.txtTranslated.Location = new System.Drawing.Point(10, 250);
            this.txtTranslated.Multiline = true;
            this.txtTranslated.Name = "txtTranslated";
            this.txtTranslated.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTranslated.Size = new System.Drawing.Size(610, 150);
            this.txtTranslated.TabIndex = 8;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(10, 420);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(80, 25);
            this.btnCopy.TabIndex = 9;
            this.btnCopy.Text = "コピー";
            this.btnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(100, 420);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(80, 25);
            this.btnReplace.TabIndex = 10;
            this.btnReplace.Text = "置き換え";
            this.btnReplace.Click += new System.EventHandler(this.BtnReplace_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(540, 420);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "閉じる";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(10, 450);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(610, 20);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "翻訳する言語を選択して「翻訳」ボタンをクリックしてください。初回使用時は「API設定」ボタンでDeepL APIキーを設定してください。";
            // 
            // TranslationDialog
            // 
            this.ClientSize = new System.Drawing.Size(632, 501);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtTranslated);
            this.Controls.Add(this.lblTranslated);
            this.Controls.Add(this.txtOriginal);
            this.Controls.Add(this.lblOriginal);
            this.Controls.Add(this.btnApiSettings);
            this.Controls.Add(this.btnDetect);
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.comboBoxSourceLang);
            this.Controls.Add(this.comboBoxTargetLang);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TranslationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "翻訳";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ComboBox comboBoxSourceLang; // 追加
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

        private void LoadText()
        {
            txtOriginal.Text = originalText;
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOriginal.Text))
            {
                MessageBox.Show("翻訳するテキストがありません。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!translationManager.HasApiKey)
            {
                MessageBox.Show("DeepL APIキーが設定されていません。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sourceLang = GetSourceLanguage(); // "JA" or "EN"
            string targetLang = GetTargetLanguage(); // "JA" or "EN"
            if (sourceLang == targetLang)
            {
                MessageBox.Show("元言語と翻訳後の言語が同じです。", "翻訳", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                string translated = await translationManager.TranslateAsync(txtOriginal.Text, sourceLang, targetLang);
                System.Diagnostics.Debug.WriteLine($"[DEBUG] 翻訳結果: {translated}"); // ← 追加
                txtTranslated.Text = translated;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "翻訳エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDetect_Click(object sender, EventArgs e)
        {
            if (!translationManager.HasApiKey)
            {
                MessageBox.Show("DeepL APIキーが設定されていません。\n設定からAPIキーを入力してください。", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOriginal.Text))
            {
                MessageBox.Show("言語を検出するテキストがありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnDetect.Enabled = false;
            lblStatus.Text = "言語検出中...";
            Application.DoEvents();

            try
            {
                var detectedLang = await translationManager.DetectLanguageAsync(txtOriginal.Text);
                var langName = GetLanguageName(detectedLang);
                lblStatus.Text = $"検出された言語: {langName} ({detectedLang})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"言語検出中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "言語検出中にエラーが発生しました";
            }
            finally
            {
                btnDetect.Enabled = true;
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTranslated.Text))
            {
                Clipboard.SetText(txtTranslated.Text);
                MessageBox.Show("翻訳結果をクリップボードにコピーしました。", "コピー",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnReplace_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTranslated.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnApiSettings_Click(object sender, EventArgs e)
        {
            using (var dialog = new ApiSettingsDialog(translationManager))
            {
                dialog.ShowDialog(this);
            }
        }

        private string GetTargetLanguageCode()
        {
            var selected = (comboBoxTargetLang.SelectedItem != null) ? comboBoxTargetLang.SelectedItem.ToString() : null;
            if (selected == null) return "EN";

            var parts = selected.Split('(');
            if (parts.Length > 1)
            {
                return parts[1].TrimEnd(')');
            }
            return "EN";
        }

        private string GetLanguageName(string langCode)
        {
            switch (langCode)
            {
                case "EN":
                    return "英語";
                case "DE":
                    return "ドイツ語";
                case "FR":
                    return "フランス語";
                case "ES":
                    return "スペイン語";
                case "IT":
                    return "イタリア語";
                case "PT":
                    return "ポルトガル語";
                case "RU":
                    return "ロシア語";
                case "ZH":
                    return "中国語";
                case "KO":
                    return "韓国語";
                case "JA":
                    return "日本語";
                default:
                    return langCode;
            }
        }

        public string GetTranslatedText()
        {
            return txtTranslated.Text;
        }

        public string GetSourceLanguage()
        {
            var selected = (comboBoxSourceLang.SelectedItem != null) ? comboBoxSourceLang.SelectedItem.ToString() : null;
            if (selected == null) return "JA";
            return selected.Contains("日本語") ? "JA" : "EN";
        }

        public string GetTargetLanguage()
        {
            var selected = (comboBoxTargetLang.SelectedItem != null) ? comboBoxTargetLang.SelectedItem.ToString() : null;
            if (selected == null) return "EN";
            return selected.Contains("日本語") ? "JA" : "EN";
        }
    }
}

using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class ApiSettingsDialog : Form
    {
        private TranslationManager translationManager;

        public ApiSettingsDialog(TranslationManager translationManager)
        {
            this.translationManager = translationManager;
            InitializeComponent();
            LoadCurrentApiKey();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "DeepL API設定";
            this.Size = new System.Drawing.Size(500, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // APIキーラベル
            var lblApiKey = new Label();
            lblApiKey.Text = "DeepL APIキー:";
            lblApiKey.Location = new System.Drawing.Point(20, 20);
            lblApiKey.Size = new System.Drawing.Size(100, 20);

            // APIキーテキストボックス
            this.txtApiKey = new TextBox();
            this.txtApiKey.Location = new System.Drawing.Point(20, 45);
            this.txtApiKey.Size = new System.Drawing.Size(440, 20);
            this.txtApiKey.PasswordChar = '*';

            // 説明ラベル
            var lblDescription = new Label();
            lblDescription.Text = "DeepL APIキーを入力してください。APIキーは https://www.deepl.com/pro-api で取得できます。";
            lblDescription.Location = new System.Drawing.Point(20, 75);
            lblDescription.Size = new System.Drawing.Size(440, 40);
            lblDescription.AutoSize = false;

            // ボタン
            this.btnSave = new Button();
            this.btnSave.Text = "保存";
            this.btnSave.Location = new System.Drawing.Point(300, 120);
            this.btnSave.Size = new System.Drawing.Size(80, 25);
            this.btnSave.Click += BtnSave_Click;

            this.btnCancel = new Button();
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.Location = new System.Drawing.Point(380, 120);
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                lblApiKey,
                this.txtApiKey,
                lblDescription,
                this.btnSave,
                this.btnCancel
            });

            this.ResumeLayout(false);
        }

        private TextBox txtApiKey;
        private Button btnSave;
        private Button btnCancel;

        private void LoadCurrentApiKey()
        {
            // 現在のAPIキーを表示（マスクされた状態）
            if (translationManager.HasApiKey)
            {
                txtApiKey.Text = "********";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var apiKey = txtApiKey.Text.Trim();
            
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("APIキーを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (apiKey == "********")
            {
                // 既存のAPIキーが設定されている場合は変更しない
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            try
            {
                translationManager.SetApiKey(apiKey);
                MessageBox.Show("APIキーが正常に保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("APIキーの保存に失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

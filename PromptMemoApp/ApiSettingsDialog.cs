using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class ApiSettingsDialog : Form
    {
        private TranslationManager translationManager;

        public ApiSettingsDialog(TranslationManager manager)
        {
            translationManager = manager;
            InitializeComponent(); // Ensure this method is defined in the designer file
            txtApiKey.Text = translationManager.HasApiKey ? "********" : "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtApiKey.Text))
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] 入力されたAPIキー: {txtApiKey.Text}");
                translationManager.SetApiKey(txtApiKey.Text);
                System.Diagnostics.Debug.WriteLine("[DEBUG] APIキーを保存しました。");
                MessageBox.Show("APIキーを保存しました。", "設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("APIキーを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnApiSettings_Click(object sender, EventArgs e)
        {
            using (var dialog = new ApiSettingsDialog(translationManager))
            {
                dialog.ShowDialog(this);
            }
        }
    }
}

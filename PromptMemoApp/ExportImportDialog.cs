using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class ExportImportDialog : Form
    {
        private PromptEditorForm parentForm;
        private Button btnExport;
        private Button btnImport;
        private Button btnCancel;

        public ExportImportDialog(PromptEditorForm parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "エクスポート/インポート";
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 説明ラベル
            var lblDescription = new Label();
            lblDescription.Text = "データのエクスポートまたはインポートを選択してください。\n\nエクスポート: 現在のデータをJSONファイルに保存します。\nインポート: JSONファイルからデータを読み込みます。";
            lblDescription.Location = new System.Drawing.Point(20, 20);
            lblDescription.Size = new System.Drawing.Size(350, 80);
            lblDescription.AutoSize = false;

            // エクスポートボタン
            this.btnExport = new Button();
            this.btnExport.Text = "エクスポート";
            this.btnExport.Location = new System.Drawing.Point(50, 120);
            this.btnExport.Size = new System.Drawing.Size(120, 40);
            this.btnExport.Click += BtnExport_Click;

            // インポートボタン
            this.btnImport = new Button();
            this.btnImport.Text = "インポート";
            this.btnImport.Location = new System.Drawing.Point(200, 120);
            this.btnImport.Size = new System.Drawing.Size(120, 40);
            this.btnImport.Click += BtnImport_Click;

            // キャンセルボタン
            this.btnCancel = new Button();
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.Location = new System.Drawing.Point(290, 180);
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                lblDescription,
                this.btnExport,
                this.btnImport,
                this.btnCancel
            });

            this.ResumeLayout(false);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*";
                saveDialog.FilterIndex = 1;
                saveDialog.DefaultExt = "json";
                saveDialog.FileName = $"PromptMemoApp_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        parentForm.ExportData(saveDialog.FileName);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"エクスポート中にエラーが発生しました: {ex.Message}", "エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*";
                openDialog.FilterIndex = 1;
                openDialog.DefaultExt = "json";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var result = MessageBox.Show("インポートすると既存のデータが上書きされる可能性があります。\n続行しますか？", 
                            "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        
                        if (result == DialogResult.Yes)
                        {
                            parentForm.ImportData(openDialog.FileName);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"インポート中にエラーが発生しました: {ex.Message}", "エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

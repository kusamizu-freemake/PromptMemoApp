using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class SortSettingsDialog : Form
    {
        private PromptEditorForm parentForm;
        private ComboBox comboBoxSortField;
        private ComboBox comboBoxSortOrder;
        private Button btnApply;
        private Button btnCancel;

        public SortSettingsDialog(PromptEditorForm parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "並び替え設定";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 並び替えフィールドラベル
            var lblSortField = new Label();
            lblSortField.Text = "並び替え項目:";
            lblSortField.Location = new System.Drawing.Point(20, 20);
            lblSortField.Size = new System.Drawing.Size(100, 20);

            // 並び替えフィールドコンボボックス
            this.comboBoxSortField = new ComboBox();
            this.comboBoxSortField.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSortField.Items.AddRange(new object[] {
                "ファイル名",
                "作成日時",
                "更新日時",
                "ファイルサイズ"
            });
            this.comboBoxSortField.Location = new System.Drawing.Point(20, 45);
            this.comboBoxSortField.Size = new System.Drawing.Size(150, 23);
            this.comboBoxSortField.SelectedIndex = 0;

            // 並び替え順序ラベル
            var lblSortOrder = new Label();
            lblSortOrder.Text = "並び替え順序:";
            lblSortOrder.Location = new System.Drawing.Point(200, 20);
            lblSortOrder.Size = new System.Drawing.Size(100, 20);

            // 並び替え順序コンボボックス
            this.comboBoxSortOrder = new ComboBox();
            this.comboBoxSortOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSortOrder.Items.AddRange(new object[] {
                "昇順",
                "降順"
            });
            this.comboBoxSortOrder.Location = new System.Drawing.Point(200, 45);
            this.comboBoxSortOrder.Size = new System.Drawing.Size(150, 23);
            this.comboBoxSortOrder.SelectedIndex = 0;

            // 説明ラベル
            var lblDescription = new Label();
            lblDescription.Text = "ファイルの表示順序を設定します。設定後は「適用」ボタンをクリックしてください。";
            lblDescription.Location = new System.Drawing.Point(20, 80);
            lblDescription.Size = new System.Drawing.Size(350, 40);
            lblDescription.AutoSize = false;

            // ボタン
            this.btnApply = new Button();
            this.btnApply.Text = "適用";
            this.btnApply.Location = new System.Drawing.Point(200, 120);
            this.btnApply.Size = new System.Drawing.Size(80, 25);
            this.btnApply.Click += BtnApply_Click;

            this.btnCancel = new Button();
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.Location = new System.Drawing.Point(290, 120);
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                lblSortField,
                this.comboBoxSortField,
                lblSortOrder,
                this.comboBoxSortOrder,
                lblDescription,
                this.btnApply,
                this.btnCancel
            });

            this.ResumeLayout(false);
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            string sortField = "";
            switch (comboBoxSortField.SelectedItem.ToString())
            {
                case "ファイル名":
                    sortField = "Name";
                    break;
                case "作成日時":
                    sortField = "Created";
                    break;
                case "更新日時":
                    sortField = "Modified";
                    break;
                case "ファイルサイズ":
                    sortField = "Size";
                    break;
            }

            parentForm.SortFiles(sortField);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class NewFileForm : Form
    {
        public string FileName { get; private set; } = string.Empty;
        public string SelectedCategory { get; private set; } = string.Empty;

        public NewFileForm(string[] categories)
        {
            InitializeComponent();
            comboBoxCategory.Items.AddRange(categories);
            if (categories.Length > 0)
                comboBoxCategory.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                MessageBox.Show("ファイル名を入力してください。");
                return;
            }

            if (comboBoxCategory.SelectedItem == null)
            {
                MessageBox.Show("カテゴリを選択してください。");
                return;
            }

            FileName = txtFileName.Text.Trim();
            SelectedCategory = comboBoxCategory.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

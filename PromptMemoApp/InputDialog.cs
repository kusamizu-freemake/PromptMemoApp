using System;
using System.Windows.Forms;

namespace PromptEditorApp
{
    public partial class InputDialog : Form
    {
        public string InputText
        {
            get => txtInput.Text;
            set => txtInput.Text = value;
        }
        public string InputTextValue { get; internal set; }

        public InputDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("入力が必要です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

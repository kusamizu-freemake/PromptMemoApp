using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class InputDialog : Form
    {
        public string InputText => txtInput.Text;

        public InputDialog(string title, string message, string defaultValue)
        {
            InitializeComponent();
            this.Text = title;
            lblMessage.Text = message;
            txtInput.Text = defaultValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("値を入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

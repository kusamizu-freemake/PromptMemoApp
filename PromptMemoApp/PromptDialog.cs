using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptDialog : Form
    {
        public string InputText => txtInput.Text;

        public PromptDialog(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("入力してください。");
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

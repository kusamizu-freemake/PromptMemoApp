using System.Windows.Forms;

namespace PromptMemoApp
{
    partial class PromptDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;
        private TextBox txtInput;
        private Button btnOK;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 9);
            this.lblMessage.Size = new System.Drawing.Size(80, 15);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "メッセージ";
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(15, 30);
            this.txtInput.Size = new System.Drawing.Size(250, 23);
            this.txtInput.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(110, 65);
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.Text = "OK";
            this.btnOK.TabIndex = 2;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(190, 65);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PromptDialog
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 101);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Name = "PromptDialog";
            this.Text = "入力";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

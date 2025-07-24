using System.Windows.Forms;

namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnRename;
        private System.Windows.Forms.ListBox lstPromptFiles;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileNew;
        private System.Windows.Forms.ToolStripMenuItem fileSave;
        private System.Windows.Forms.ToolStripMenuItem fileExit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.fileExit = new System.Windows.Forms.ToolStripMenuItem();

            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnRename = new System.Windows.Forms.ToolStripButton();

            this.lstPromptFiles = new System.Windows.Forms.ListBox();
            this.txtPrompt = new System.Windows.Forms.TextBox();

            // MenuStrip
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileMenu
            });
            this.fileMenu.Text = "ファイル";
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileNew, this.fileSave, this.fileExit
            });
            this.fileNew.Text = "新規作成";
            this.fileNew.Click += new System.EventHandler(this.btnNew_Click);
            this.fileSave.Text = "保存";
            this.fileSave.Click += new System.EventHandler(this.btnSave_Click);
            this.fileExit.Text = "終了";
            this.fileExit.Click += (s, e) => this.Close();

            // ToolStrip
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnNew, this.btnSave, this.btnDelete, this.btnRename
            });

            this.btnNew.Text = "新規";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnDelete.Text = "削除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnRename.Text = "名前変更";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);

            // ListBox
            this.lstPromptFiles.Location = new System.Drawing.Point(10, 60);
            this.lstPromptFiles.Size = new System.Drawing.Size(200, 380);
            this.lstPromptFiles.SelectedIndexChanged += new System.EventHandler(this.lstPromptFiles_SelectedIndexChanged);

            // TextBox
            this.txtPrompt.Location = new System.Drawing.Point(220, 60);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.ScrollBars = ScrollBars.Vertical;
            this.txtPrompt.Size = new System.Drawing.Size(550, 380);

            // PromptEditorForm
            this.Text = "プロンプトエディタ";
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.lstPromptFiles);
            this.Controls.Add(this.txtPrompt);
            this.MainMenuStrip = this.menuStrip;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
        }
    }
}

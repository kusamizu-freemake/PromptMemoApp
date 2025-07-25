using System.Windows.Forms;

namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem editMenu;

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnSave;

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TreeView treeViewPrompts;
        private System.Windows.Forms.Button btnCreateCategory;
        private System.Windows.Forms.TextBox txtPrompt;

        private System.Windows.Forms.ContextMenuStrip contextMenuCategory;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRenameCategory;
        private System.Windows.Forms.ToolStripMenuItem contextMenuDeleteCategory;

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
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();

            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();

            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.treeViewPrompts = new System.Windows.Forms.TreeView();
            this.btnCreateCategory = new System.Windows.Forms.Button();
            this.txtPrompt = new System.Windows.Forms.TextBox();

            this.contextMenuCategory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuRenameCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuDeleteCategory = new System.Windows.Forms.ToolStripMenuItem();

            // MenuStrip
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileMenu,
                this.editMenu
            });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);

            this.fileMenu.Text = "ファイル";
            this.editMenu.Text = "編集";

            // ToolStrip
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnNew,
                this.btnSave
            });
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(800, 25);

            this.btnNew.Text = "新規";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // SplitContainer
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 49);
            this.splitContainerMain.Name = "splitContainerMain";

            // 左側（ツリービュー + カテゴリ作成ボタン）
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewPrompts);
            this.splitContainerMain.Panel1.Controls.Add(this.btnCreateCategory);

            // 右側（プロンプトテキスト）
            this.splitContainerMain.Panel2.Controls.Add(this.txtPrompt);

            this.splitContainerMain.Size = new System.Drawing.Size(800, 401);
            this.splitContainerMain.SplitterDistance = 250;
            this.splitContainerMain.TabIndex = 2;

            // TreeView
            this.treeViewPrompts.Location = new System.Drawing.Point(0, 30);
            this.treeViewPrompts.Name = "treeViewPrompts";
            this.treeViewPrompts.Size = new System.Drawing.Size(250, 371);
            this.treeViewPrompts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.treeViewPrompts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPrompts_AfterSelect);
            this.treeViewPrompts.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewPrompts_NodeMouseClick);

            // btnCreateCategory
            this.btnCreateCategory.Location = new System.Drawing.Point(0, 0);
            this.btnCreateCategory.Name = "btnCreateCategory";
            this.btnCreateCategory.Size = new System.Drawing.Size(250, 30);
            this.btnCreateCategory.Text = "カテゴリ作成";
            this.btnCreateCategory.Click += new System.EventHandler(this.btnCreateCategory_Click);

            // txtPrompt
            this.txtPrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrompt.Multiline = true;
            this.txtPrompt.ScrollBars = ScrollBars.Vertical;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.Size = new System.Drawing.Size(546, 401);

            // ContextMenuStrip
            this.contextMenuCategory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.contextMenuRenameCategory,
                this.contextMenuDeleteCategory
            });

            this.contextMenuRenameCategory.Text = "名前変更";
            this.contextMenuRenameCategory.Click += new System.EventHandler(this.contextMenuRenameCategory_Click);

            this.contextMenuDeleteCategory.Text = "削除";
            this.contextMenuDeleteCategory.Click += new System.EventHandler(this.contextMenuDeleteCategory_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "PromptEditorForm";
            this.Text = "Prompt Memo App";

            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
        }
    }
}

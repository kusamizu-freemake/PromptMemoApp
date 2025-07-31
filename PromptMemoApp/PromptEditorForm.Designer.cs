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
        private System.Windows.Forms.ListBox lstPromptFiles;

        // 追加
        private System.Windows.Forms.ComboBox cmbCategories;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnMoveSelected;

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

            this.lstPromptFiles = new System.Windows.Forms.ListBox();
            this.txtPrompt = new System.Windows.Forms.TextBox();

            this.cmbCategories = new System.Windows.Forms.ComboBox();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnMoveSelected = new System.Windows.Forms.Button();

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
            this.menuStrip.Size = new System.Drawing.Size(900, 24);

            this.fileMenu.Text = "ファイル";
            this.editMenu.Text = "編集";

            // ToolStrip
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnNew,
                this.btnSave
            });
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(900, 25);

            this.btnNew.Text = "新規";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // SplitContainer
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 49);
            this.splitContainerMain.Name = "splitContainerMain";

            // 左パネル：TreeView + カテゴリ作成ボタン
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewPrompts);
            this.splitContainerMain.Panel1.Controls.Add(this.btnCreateCategory);

            // 右パネル：ファイル一覧 + 操作用UI + 編集TextBox
            this.splitContainerMain.Panel2.Controls.Add(this.lstPromptFiles);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbCategories);
            this.splitContainerMain.Panel2.Controls.Add(this.btnDeleteSelected);
            this.splitContainerMain.Panel2.Controls.Add(this.btnMoveSelected);
            this.splitContainerMain.Panel2.Controls.Add(this.txtPrompt);

            this.splitContainerMain.Size = new System.Drawing.Size(900, 500);
            this.splitContainerMain.SplitterDistance = 250;
            this.splitContainerMain.TabIndex = 2;

            // TreeView
            this.treeViewPrompts.Location = new System.Drawing.Point(0, 30);
            this.treeViewPrompts.Name = "treeViewPrompts";
            this.treeViewPrompts.Size = new System.Drawing.Size(250, 470);
            this.treeViewPrompts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.treeViewPrompts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPrompts_AfterSelect);
            this.treeViewPrompts.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewPrompts_NodeMouseClick);

            // btnCreateCategory
            this.btnCreateCategory.Location = new System.Drawing.Point(0, 0);
            this.btnCreateCategory.Name = "btnCreateCategory";
            this.btnCreateCategory.Size = new System.Drawing.Size(250, 30);
            this.btnCreateCategory.Text = "カテゴリ作成";
            this.btnCreateCategory.Click += new System.EventHandler(this.btnCreateCategory_Click);

            // ListBox (ファイル一覧)
            this.lstPromptFiles.Location = new System.Drawing.Point(0, 30);
            this.lstPromptFiles.Name = "lstPromptFiles";
            this.lstPromptFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPromptFiles.Size = new System.Drawing.Size(350, 200);
            this.lstPromptFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.lstPromptFiles.SelectedIndexChanged += new System.EventHandler(this.lstPromptFiles_SelectedIndexChanged);

            // ComboBox (カテゴリ選択)
            this.cmbCategories.Location = new System.Drawing.Point(0, 0);
            this.cmbCategories.Name = "cmbCategories";
            this.cmbCategories.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCategories.Size = new System.Drawing.Size(200, 23);

            // 削除ボタン
            this.btnDeleteSelected.Location = new System.Drawing.Point(210, 0);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(60, 23);
            this.btnDeleteSelected.Text = "削除";
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);

            // 移動ボタン
            this.btnMoveSelected.Location = new System.Drawing.Point(280, 0);
            this.btnMoveSelected.Name = "btnMoveSelected";
            this.btnMoveSelected.Size = new System.Drawing.Size(60, 23);
            this.btnMoveSelected.Text = "移動";
            this.btnMoveSelected.Click += new System.EventHandler(this.btnMoveSelected_Click);

            // TextBox
            this.txtPrompt.Location = new System.Drawing.Point(0, 240);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.ScrollBars = ScrollBars.Vertical;
            this.txtPrompt.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.Size = new System.Drawing.Size(646, 260);

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
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "PromptEditorForm";
            this.Text = "Prompt Memo App";

            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
        }
    }
}

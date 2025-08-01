using System;

namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuHistory;
        private System.Windows.Forms.ToolStripMenuItem menuFavorites;
        private System.Windows.Forms.ToolStripMenuItem menuSearch;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeViewCategories;
        private System.Windows.Forms.Panel panelRight;

        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.ComboBox comboBoxCategories;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;

        // 右クリックメニュー
        private System.Windows.Forms.ContextMenuStrip contextMenuCategory;
        private System.Windows.Forms.ToolStripMenuItem contextAddCategory;
        private System.Windows.Forms.ToolStripMenuItem contextRenameCategory;
        private System.Windows.Forms.ToolStripMenuItem contextDeleteCategory;

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// InitializeComponent
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // メニュー
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearch = new System.Windows.Forms.ToolStripMenuItem();

            // SplitContainer
            this.splitContainer = new System.Windows.Forms.SplitContainer();

            // TreeView + ContextMenu
            this.treeViewCategories = new System.Windows.Forms.TreeView();
            this.contextMenuCategory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextAddCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.contextRenameCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.contextDeleteCategory = new System.Windows.Forms.ToolStripMenuItem();

            // 右ペイン
            this.panelRight = new System.Windows.Forms.Panel();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.comboBoxCategories = new System.Windows.Forms.ComboBox();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();

            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile,
                this.menuHistory,
                this.menuFavorites,
                this.menuSearch
            });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(900, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";

            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuNew,
                this.menuSave,
                this.menuExit});
            this.menuFile.Text = "ファイル";

            this.menuNew.Text = "新規作成";
            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);

            this.menuSave.Text = "保存";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);

            this.menuExit.Text = "終了";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);

            // 履歴・お気に入り・検索メニュー
            this.menuHistory.Text = "履歴";
            this.menuHistory.Click += new System.EventHandler(this.menuHistory_Click);

            this.menuFavorites.Text = "お気に入り";
            this.menuFavorites.Click += new System.EventHandler(this.menuFavorites_Click);

            this.menuSearch.Text = "検索";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);

            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Size = new System.Drawing.Size(900, 550);
            this.splitContainer.SplitterDistance = 250;

            // 左ペイン：TreeView
            this.treeViewCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCategories.ContextMenuStrip = this.contextMenuCategory;
            this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);
            this.splitContainer.Panel1.Controls.Add(this.treeViewCategories);

            // ContextMenuStrip
            this.contextMenuCategory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.contextAddCategory,
                this.contextRenameCategory,
                this.contextDeleteCategory
            });

            this.contextAddCategory.Text = "カテゴリ作成";
            this.contextAddCategory.Click += new System.EventHandler(this.contextAddCategory_Click);

            this.contextRenameCategory.Text = "カテゴリ名変更";
            this.contextRenameCategory.Click += new System.EventHandler(this.contextRenameCategory_Click);

            this.contextDeleteCategory.Text = "カテゴリ削除";
            this.contextDeleteCategory.Click += new System.EventHandler(this.contextDeleteCategory_Click);

            // 右ペイン
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;

            // ボタンとコンボボックス
            this.btnRename.Text = "名前変更";
            this.btnRename.Location = new System.Drawing.Point(10, 5);
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);

            this.btnDelete.Text = "削除";
            this.btnDelete.Location = new System.Drawing.Point(100, 5);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnMove.Text = "カテゴリ移動";
            this.btnMove.Location = new System.Drawing.Point(190, 5);
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);

            this.comboBoxCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategories.Location = new System.Drawing.Point(300, 5);
            this.comboBoxCategories.Width = 150;

            // ListBox
            this.listBoxFiles.Location = new System.Drawing.Point(10, 40);
            this.listBoxFiles.Size = new System.Drawing.Size(440, 150);
            this.listBoxFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);

            // TextBox
            this.txtPrompt.Multiline = true;
            this.txtPrompt.Location = new System.Drawing.Point(10, 200);
            this.txtPrompt.Size = new System.Drawing.Size(440, 300);

            // 新規作成・保存ボタン
            this.btnNew.Text = "新規作成";
            this.btnNew.Location = new System.Drawing.Point(460, 40);
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text = "保存";
            this.btnSave.Location = new System.Drawing.Point(460, 70);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 右ペインに追加
            this.panelRight.Controls.Add(this.btnRename);
            this.panelRight.Controls.Add(this.btnDelete);
            this.panelRight.Controls.Add(this.btnMove);
            this.panelRight.Controls.Add(this.comboBoxCategories);
            this.panelRight.Controls.Add(this.listBoxFiles);
            this.panelRight.Controls.Add(this.txtPrompt);
            this.panelRight.Controls.Add(this.btnNew);
            this.panelRight.Controls.Add(this.btnSave);

            this.splitContainer.Panel2.Controls.Add(this.panelRight);

            // Form設定
            this.ClientSize = new System.Drawing.Size(900, 574);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Text = "Prompt Memo App";

            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
        }
    }
}

using System;

namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShortcuts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSort = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExportImport = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.treeViewCategories = new System.Windows.Forms.TreeView();
            this.contextMenuCategory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.txtEditor = new System.Windows.Forms.TextBox();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnFavorite = new System.Windows.Forms.Button();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.comboBoxCategories = new System.Windows.Forms.ComboBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.contextMenuCategory.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1200, 28);
            this.menuStrip.TabIndex = 2;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(65, 24);
            this.menuFile.Text = "ファイル";
            // 
            // menuNew
            // 
            this.menuNew.Name = "menuNew";
            this.menuNew.Size = new System.Drawing.Size(122, 26);
            this.menuNew.Text = "新規";
            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(122, 26);
            this.menuExit.Text = "終了";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFavorites,
            this.menuHistory,
            this.menuSearch,
            this.menuShortcuts,
            this.menuSort,
            this.menuStatistics,
            this.menuExportImport});
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(56, 24);
            this.menuTools.Text = "ツール";
            // 
            // menuFavorites
            // 
            this.menuFavorites.Name = "menuFavorites";
            this.menuFavorites.Size = new System.Drawing.Size(219, 26);
            this.menuFavorites.Text = "お気に入り";
            this.menuFavorites.Click += new System.EventHandler(this.menuFavorites_Click);
            // 
            // menuHistory
            // 
            this.menuHistory.Name = "menuHistory";
            this.menuHistory.Size = new System.Drawing.Size(219, 26);
            this.menuHistory.Text = "履歴";
            this.menuHistory.Click += new System.EventHandler(this.menuHistory_Click);
            // 
            // menuSearch
            // 
            this.menuSearch.Name = "menuSearch";
            this.menuSearch.Size = new System.Drawing.Size(219, 26);
            this.menuSearch.Text = "検索";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);
            // 
            // menuShortcuts
            // 
            this.menuShortcuts.Name = "menuShortcuts";
            this.menuShortcuts.Size = new System.Drawing.Size(219, 26);
            this.menuShortcuts.Text = "ショートカット設定";
            this.menuShortcuts.Click += new System.EventHandler(this.menuShortcuts_Click);
            // 
            // menuSort
            // 
            this.menuSort.Name = "menuSort";
            this.menuSort.Size = new System.Drawing.Size(219, 26);
            this.menuSort.Text = "並び替え";
            this.menuSort.Click += new System.EventHandler(this.menuSort_Click);
            // 
            // menuStatistics
            // 
            this.menuStatistics.Name = "menuStatistics";
            this.menuStatistics.Size = new System.Drawing.Size(219, 26);
            this.menuStatistics.Text = "統計情報";
            this.menuStatistics.Click += new System.EventHandler(this.menuStatistics_Click);
            // 
            // menuExportImport
            // 
            this.menuExportImport.Name = "menuExportImport";
            this.menuExportImport.Size = new System.Drawing.Size(219, 26);
            this.menuExportImport.Text = "エクスポート/インポート";
            this.menuExportImport.Click += new System.EventHandler(this.menuExportImport_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 28);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.leftPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.txtEditor);
            this.splitContainer.Panel2.Controls.Add(this.listViewFiles);
            this.splitContainer.Panel2.Controls.Add(this.panelButtons);
            this.splitContainer.Panel2.Controls.Add(this.comboBoxCategories);
            this.splitContainer.Size = new System.Drawing.Size(1200, 772);
            this.splitContainer.SplitterDistance = 495;
            this.splitContainer.TabIndex = 1;
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.treeViewCategories);
            this.leftPanel.Controls.Add(this.btnAddCategory);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(495, 772);
            this.leftPanel.TabIndex = 0;
            // 
            // treeViewCategories
            // 
            this.treeViewCategories.ContextMenuStrip = this.contextMenuCategory;
            this.treeViewCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCategories.Location = new System.Drawing.Point(0, 30);
            this.treeViewCategories.Name = "treeViewCategories";
            this.treeViewCategories.Size = new System.Drawing.Size(495, 742);
            this.treeViewCategories.TabIndex = 0;
            this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);
            // 
            // contextMenuCategory
            // 
            this.contextMenuCategory.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuCategory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameCategoryMenuItem,
            this.deleteCategoryMenuItem});
            this.contextMenuCategory.Name = "contextMenuCategory";
            this.contextMenuCategory.Size = new System.Drawing.Size(139, 52);
            // 
            // renameCategoryMenuItem
            // 
            this.renameCategoryMenuItem.Name = "renameCategoryMenuItem";
            this.renameCategoryMenuItem.Size = new System.Drawing.Size(138, 24);
            this.renameCategoryMenuItem.Text = "名前変更";
            this.renameCategoryMenuItem.Click += new System.EventHandler(this.renameCategoryMenuItem_Click);
            // 
            // deleteCategoryMenuItem
            // 
            this.deleteCategoryMenuItem.Name = "deleteCategoryMenuItem";
            this.deleteCategoryMenuItem.Size = new System.Drawing.Size(138, 24);
            this.deleteCategoryMenuItem.Text = "削除";
            this.deleteCategoryMenuItem.Click += new System.EventHandler(this.deleteCategoryMenuItem_Click);
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddCategory.Location = new System.Drawing.Point(0, 0);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(495, 30);
            this.btnAddCategory.TabIndex = 1;
            this.btnAddCategory.Text = "カテゴリ作成";
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            // 
            // txtEditor
            // 
            this.txtEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEditor.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtEditor.Location = new System.Drawing.Point(0, 190);
            this.txtEditor.Multiline = true;
            this.txtEditor.Name = "txtEditor";
            this.txtEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEditor.Size = new System.Drawing.Size(701, 582);
            this.txtEditor.TabIndex = 0;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            new System.Windows.Forms.ColumnHeader() { Text = "名前", Width = 250 },
            new System.Windows.Forms.ColumnHeader() { Text = "作成日", Width = 120 },
            new System.Windows.Forms.ColumnHeader() { Text = "更新日", Width = 120 },
            new System.Windows.Forms.ColumnHeader() { Text = "サイズ", Width = 80 }
});
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(0, 40);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(701, 150);
            this.listViewFiles.TabIndex = 1;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewFiles_ColumnClick);
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnNew);
            this.panelButtons.Controls.Add(this.btnRename);
            this.panelButtons.Controls.Add(this.btnDelete);
            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Controls.Add(this.btnFavorite);
            this.panelButtons.Controls.Add(this.btnTranslate);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Location = new System.Drawing.Point(0, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Padding = new System.Windows.Forms.Padding(5);
            this.panelButtons.Size = new System.Drawing.Size(701, 40);
            this.panelButtons.TabIndex = 2;
            this.panelButtons.WrapContents = false;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(8, 8);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "新規作成";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(89, 8);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 1;
            this.btnRename.Text = "名前変更";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(170, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "削除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(251, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnFavorite
            // 
            this.btnFavorite.Location = new System.Drawing.Point(332, 8);
            this.btnFavorite.Name = "btnFavorite";
            this.btnFavorite.Size = new System.Drawing.Size(75, 23);
            this.btnFavorite.TabIndex = 5;
            this.btnFavorite.Text = "お気に入り";
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);
            // 
            // btnTranslate
            // 
            this.btnTranslate.Location = new System.Drawing.Point(413, 8);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.Size = new System.Drawing.Size(75, 23);
            this.btnTranslate.TabIndex = 6;
            this.btnTranslate.Text = "翻訳";
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // comboBoxCategories
            // 
            this.comboBoxCategories.Location = new System.Drawing.Point(10, 5);
            this.comboBoxCategories.Name = "comboBoxCategories";
            this.comboBoxCategories.Size = new System.Drawing.Size(250, 25);
            this.comboBoxCategories.TabIndex = 3;
            // 
            // PromptEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "PromptEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prompt Memo App";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.contextMenuCategory.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // フィールド定義
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuFavorites;
        private System.Windows.Forms.ToolStripMenuItem menuHistory;
        private System.Windows.Forms.ToolStripMenuItem menuSearch;
        private System.Windows.Forms.ToolStripMenuItem menuShortcuts;
        private System.Windows.Forms.ToolStripMenuItem menuSort;
        private System.Windows.Forms.ToolStripMenuItem menuStatistics;
        private System.Windows.Forms.ToolStripMenuItem menuExportImport;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.TreeView treeViewCategories;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.TextBox txtEditor;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ComboBox comboBoxCategories;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnFavorite;
        private System.Windows.Forms.Button btnTranslate;

        private System.Windows.Forms.ContextMenuStrip contextMenuCategory;
        private System.Windows.Forms.ToolStripMenuItem renameCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCategoryMenuItem;
    }
}

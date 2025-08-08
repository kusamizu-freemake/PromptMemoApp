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

            // コンポーネント初期化
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
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.treeViewCategories = new System.Windows.Forms.TreeView();
            this.contextMenuCategory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.comboBoxCategories = new System.Windows.Forms.ComboBox();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnFavorite = new System.Windows.Forms.Button();
            this.btnTranslate = new System.Windows.Forms.Button();
            this.txtEditor = new System.Windows.Forms.TextBox();

            // MenuStrip
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools
        });
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuExit
        });
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFavorites,
            this.menuHistory,
            this.menuSearch,
            this.menuShortcuts,
            this.menuSort,
            this.menuStatistics,
            this.menuExportImport
        });
            this.menuFile.Text = "ファイル";
            this.menuNew.Text = "新規";
            this.menuExit.Text = "終了";
            this.menuTools.Text = "ツール";
            this.menuFavorites.Text = "お気に入り";
            this.menuHistory.Text = "履歴";
            this.menuSearch.Text = "検索";
            this.menuShortcuts.Text = "ショートカット設定";
            this.menuSort.Text = "並び替え";
            this.menuStatistics.Text = "統計情報";
            this.menuExportImport.Text = "エクスポート/インポート";

            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            this.menuFavorites.Click += new System.EventHandler(this.menuFavorites_Click);
            this.menuHistory.Click += new System.EventHandler(this.menuHistory_Click);
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);
            this.menuShortcuts.Click += new System.EventHandler(this.menuShortcuts_Click);
            this.menuSort.Click += new System.EventHandler(this.menuSort_Click);
            this.menuStatistics.Click += new System.EventHandler(this.menuStatistics_Click);
            this.menuExportImport.Click += new System.EventHandler(this.menuExportImport_Click);

            // SplitContainer
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.splitContainer.SplitterDistance = 250;

            // 左パネル：Panel で包んで上部にボタンを設置
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;

            this.btnAddCategory.Text = "カテゴリ作成";
            this.btnAddCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddCategory.Height = 30;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);

            this.treeViewCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewCategories.ContextMenuStrip = this.contextMenuCategory;
            this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);

            this.contextMenuCategory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameCategoryMenuItem,
            this.deleteCategoryMenuItem
        });
            this.renameCategoryMenuItem.Text = "名前変更";
            this.deleteCategoryMenuItem.Text = "削除";
            this.renameCategoryMenuItem.Click += new System.EventHandler(this.renameCategoryMenuItem_Click);
            this.deleteCategoryMenuItem.Click += new System.EventHandler(this.deleteCategoryMenuItem_Click);

            this.leftPanel.Controls.Add(this.treeViewCategories);
            this.leftPanel.Controls.Add(this.btnAddCategory);
            this.splitContainer.Panel1.Controls.Add(this.leftPanel);

            // 右パネル構成
            this.comboBoxCategories.Location = new System.Drawing.Point(10, 5);
            this.comboBoxCategories.Size = new System.Drawing.Size(250, 25);

            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.listViewFiles.Height = 150;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            new System.Windows.Forms.ColumnHeader() { Text = "ファイル名", Width = 180 },
            new System.Windows.Forms.ColumnHeader() { Text = "作成日", Width = 80 },
            new System.Windows.Forms.ColumnHeader() { Text = "更新日", Width = 80 },
            new System.Windows.Forms.ColumnHeader() { Text = "サイズ", Width = 60 }
        });
            this.listViewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewFiles_ColumnClick);
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);

            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelButtons.Height = 40;
            this.panelButtons.Padding = new System.Windows.Forms.Padding(5);
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelButtons.WrapContents = false;

            // ボタン群
            this.panelButtons.Controls.AddRange(new System.Windows.Forms.Control[] {
            this.btnNew, this.btnRename, this.btnDelete,
            this.btnMove, this.btnSave, this.btnFavorite, this.btnTranslate
        });

            this.btnNew.Text = "新規作成";
            this.btnRename.Text = "名前変更";
            this.btnDelete.Text = "削除";
            this.btnMove.Text = "移動";
            this.btnSave.Text = "保存";
            this.btnFavorite.Text = "お気に入り";
            this.btnTranslate.Text = "翻訳";

            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);

            // テキストエディタ
            this.txtEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEditor.Multiline = true;
            this.txtEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEditor.Font = new System.Drawing.Font("Consolas", 9F);

            // 右パネルに追加（順序に注意）
            this.splitContainer.Panel2.Controls.Add(this.txtEditor);
            this.splitContainer.Panel2.Controls.Add(this.listViewFiles);
            this.splitContainer.Panel2.Controls.Add(this.panelButtons);
            this.splitContainer.Panel2.Controls.Add(this.comboBoxCategories);

            // フォーム設定
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "PromptEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prompt Memo App";
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Font = new System.Drawing.Font("MS UI Gothic", 10F);

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
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnFavorite;
        private System.Windows.Forms.Button btnTranslate;

        private System.Windows.Forms.ContextMenuStrip contextMenuCategory;
        private System.Windows.Forms.ToolStripMenuItem renameCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCategoryMenuItem;

    }
}

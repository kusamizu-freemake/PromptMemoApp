using System;

namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFavorites = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeViewCategories = new System.Windows.Forms.TreeView();
            this.contextMenuCategory = new System.Windows.Forms.ContextMenuStrip();
            this.renameCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.comboBoxCategories = new System.Windows.Forms.ComboBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtEditor = new System.Windows.Forms.TextBox();

            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools});
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuExit});
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFavorites,
            this.menuHistory,
            this.menuSearch});
            this.menuFile.Text = "ファイル";
            this.menuNew.Text = "新規";
            this.menuExit.Text = "終了";
            this.menuTools.Text = "ツール";
            this.menuFavorites.Text = "お気に入り";
            this.menuHistory.Text = "履歴";
            this.menuSearch.Text = "検索";
            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            this.menuFavorites.Click += new System.EventHandler(this.menuFavorites_Click);
            this.menuHistory.Click += new System.EventHandler(this.menuHistory_Click);
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);

            this.contextMenuCategory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameCategoryMenuItem,
            this.deleteCategoryMenuItem});
            this.renameCategoryMenuItem.Text = "名前変更";
            this.deleteCategoryMenuItem.Text = "削除";
            this.renameCategoryMenuItem.Click += new System.EventHandler(this.renameCategoryMenuItem_Click);
            this.deleteCategoryMenuItem.Click += new System.EventHandler(this.deleteCategoryMenuItem_Click);

            this.splitContainer.Panel1.Controls.Add(this.btnAddCategory);
            this.splitContainer.Panel1.Controls.Add(this.treeViewCategories);
            this.splitContainer.Panel2.Controls.Add(this.comboBoxCategories);
            this.splitContainer.Panel2.Controls.Add(this.listBoxFiles);
            this.splitContainer.Panel2.Controls.Add(this.txtEditor);
            this.splitContainer.Panel2.Controls.Add(this.btnNew);
            this.splitContainer.Panel2.Controls.Add(this.btnRename);
            this.splitContainer.Panel2.Controls.Add(this.btnDelete);
            this.splitContainer.Panel2.Controls.Add(this.btnMove);
            this.splitContainer.Panel2.Controls.Add(this.btnSave);

            this.treeViewCategories.ContextMenuStrip = this.contextMenuCategory;
            this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);

            this.btnAddCategory.Text = "カテゴリ作成";
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);

            this.btnNew.Text = "新規";
            this.btnRename.Text = "名前変更";
            this.btnDelete.Text = "削除";
            this.btnMove.Text = "移動";
            this.btnSave.Text = "保存";

            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);

            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.splitContainer);
            
            // フォームの設定
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Name = "PromptEditorForm";
            this.Text = "Prompt Memo App";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            
            // メニューストリップの設定
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Size = new System.Drawing.Size(1200, 24);
            this.MainMenuStrip = this.menuStrip;
            
            // SplitContainerの設定
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Size = new System.Drawing.Size(1200, 776);
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
            
            // Panel1 (左側) - カテゴリツリー
            this.treeViewCategories.Location = new System.Drawing.Point(0, 40);
            this.treeViewCategories.Size = new System.Drawing.Size(300, 700);
            this.treeViewCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            
            this.btnAddCategory.Location = new System.Drawing.Point(0, 0);
            this.btnAddCategory.Size = new System.Drawing.Size(100, 30);
            this.btnAddCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            
            // Panel2 (右側) - ファイルリストとエディタ
            this.comboBoxCategories.Location = new System.Drawing.Point(0, 0);
            this.comboBoxCategories.Size = new System.Drawing.Size(200, 20);
            this.comboBoxCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            
            this.listBoxFiles.Location = new System.Drawing.Point(0, 30);
            this.listBoxFiles.Size = new System.Drawing.Size(200, 150);
            this.listBoxFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            
            // ボタンの配置
            this.btnNew.Location = new System.Drawing.Point(220, 0);
            this.btnNew.Size = new System.Drawing.Size(80, 25);
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            
            this.btnRename.Location = new System.Drawing.Point(310, 0);
            this.btnRename.Size = new System.Drawing.Size(80, 25);
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            
            this.btnDelete.Location = new System.Drawing.Point(400, 0);
            this.btnDelete.Size = new System.Drawing.Size(80, 25);
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            
            this.btnMove.Location = new System.Drawing.Point(490, 0);
            this.btnMove.Size = new System.Drawing.Size(80, 25);
            this.btnMove.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            
            this.btnSave.Location = new System.Drawing.Point(580, 0);
            this.btnSave.Size = new System.Drawing.Size(80, 25);
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            
            // テキストエディタの設定
            this.txtEditor.Location = new System.Drawing.Point(0, 190);
            this.txtEditor.Size = new System.Drawing.Size(900, 586);
            this.txtEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEditor.Multiline = true;
            this.txtEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEditor.Font = new System.Drawing.Font("Consolas", 9F);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuFavorites;
        private System.Windows.Forms.ToolStripMenuItem menuHistory;
        private System.Windows.Forms.ToolStripMenuItem menuSearch;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeViewCategories;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.TextBox txtEditor;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboBoxCategories;
        private System.Windows.Forms.Button btnAddCategory;

        private System.Windows.Forms.ContextMenuStrip contextMenuCategory;
        private System.Windows.Forms.ToolStripMenuItem renameCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCategoryMenuItem;
    }
}
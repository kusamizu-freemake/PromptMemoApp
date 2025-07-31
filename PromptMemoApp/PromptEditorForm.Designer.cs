namespace PromptMemoApp
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSearch;

        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnCategoryCreate;

        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.TreeView treeViewCategories;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.ComboBox comboBoxCategories;

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;

        /// <summary>
        /// リソース解放
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナー

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSearch = new System.Windows.Forms.ToolStripMenuItem();

            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnCategoryCreate = new System.Windows.Forms.Button();

            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.treeViewCategories = new System.Windows.Forms.TreeView();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.comboBoxCategories = new System.Windows.Forms.ComboBox();

            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();

            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();

            // menuStrip1
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.編集ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1050, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";

            // ファイルToolStripMenuItem
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.ファイルToolStripMenuItem.Text = "ファイル";

            // 編集ToolStripMenuItem
            this.編集ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSearch});
            this.編集ToolStripMenuItem.Name = "編集ToolStripMenuItem";
            this.編集ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.編集ToolStripMenuItem.Text = "編集";

            // menuSearch
            this.menuSearch.Name = "menuSearch";
            this.menuSearch.Size = new System.Drawing.Size(102, 22);
            this.menuSearch.Text = "検索";
            this.menuSearch.Click += new System.EventHandler(this.menuSearch_Click);

            // btnNew
            this.btnNew.Location = new System.Drawing.Point(10, 30);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(70, 25);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "新規";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(90, 30);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 25);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnRename
            this.btnRename.Location = new System.Drawing.Point(170, 30);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(80, 25);
            this.btnRename.TabIndex = 3;
            this.btnRename.Text = "名前変更";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(260, 30);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 25);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "削除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            // btnMove
            this.btnMove.Location = new System.Drawing.Point(340, 30);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(70, 25);
            this.btnMove.TabIndex = 5;
            this.btnMove.Text = "移動";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);

            // comboBoxCategories
            this.comboBoxCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategories.Location = new System.Drawing.Point(420, 30);
            this.comboBoxCategories.Name = "comboBoxCategories";
            this.comboBoxCategories.Size = new System.Drawing.Size(180, 23);
            this.comboBoxCategories.TabIndex = 6;

            // btnCategoryCreate
            this.btnCategoryCreate.Location = new System.Drawing.Point(10, 60);
            this.btnCategoryCreate.Name = "btnCategoryCreate";
            this.btnCategoryCreate.Size = new System.Drawing.Size(150, 25);
            this.btnCategoryCreate.TabIndex = 7;
            this.btnCategoryCreate.Text = "カテゴリ作成";
            this.btnCategoryCreate.UseVisualStyleBackColor = true;

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(620, 30);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 23);
            this.txtSearch.TabIndex = 8;

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(880, 30);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(70, 25);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // treeViewCategories
            this.treeViewCategories.Location = new System.Drawing.Point(10, 90);
            this.treeViewCategories.Name = "treeViewCategories";
            this.treeViewCategories.Size = new System.Drawing.Size(250, 520);
            this.treeViewCategories.TabIndex = 10;
            this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);

            // listBoxFiles
            this.listBoxFiles.Location = new System.Drawing.Point(270, 90);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(320, 200);
            this.listBoxFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFiles.TabIndex = 11;
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);

            // txtPrompt
            this.txtPrompt.Location = new System.Drawing.Point(270, 300);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.Size = new System.Drawing.Size(750, 310);
            this.txtPrompt.TabIndex = 12;

            // PromptEditorForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 650);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.comboBoxCategories);
            this.Controls.Add(this.btnCategoryCreate);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.treeViewCategories);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.txtPrompt);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PromptEditorForm";
            this.Text = "Prompt Memo App";

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

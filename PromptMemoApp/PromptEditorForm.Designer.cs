using System;

namespace PromptManager
{
    partial class PromptEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox lstPromptFiles;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRename;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lstPromptFiles = new System.Windows.Forms.ListBox();
            this.txtPrompt = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstPromptFiles
            // 
            this.lstPromptFiles.FormattingEnabled = true;
            this.lstPromptFiles.ItemHeight = 15;
            this.lstPromptFiles.Location = new System.Drawing.Point(12, 12);
            this.lstPromptFiles.Name = "lstPromptFiles";
            this.lstPromptFiles.Size = new System.Drawing.Size(200, 394);
            this.lstPromptFiles.TabIndex = 0;
            this.lstPromptFiles.SelectedIndexChanged += new System.EventHandler(this.lstPromptFiles_SelectedIndexChanged);
            // 
            // txtPrompt
            // 
            this.txtPrompt.Location = new System.Drawing.Point(218, 12);
            this.txtPrompt.Multiline = true;
            this.txtPrompt.Name = "txtPrompt";
            this.txtPrompt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrompt.Size = new System.Drawing.Size(400, 300);
            this.txtPrompt.TabIndex = 1;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(218, 330);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "新規作成";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(299, 330);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(380, 330);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "削除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(461, 330);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 5;
            this.btnRename.Text = "名前変更";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // PromptEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 420);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.txtPrompt);
            this.Controls.Add(this.lstPromptFiles);
            this.Name = "PromptEditorForm";
            this.Text = "プロンプトエディタ";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

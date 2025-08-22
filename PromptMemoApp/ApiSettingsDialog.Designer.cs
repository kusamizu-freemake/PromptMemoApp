namespace PromptMemoApp  
{  
    partial class ApiSettingsDialog  
    {  
        /// <summary>  
        /// 必要なデザイナー変数です。  
        /// </summary>  
        private System.ComponentModel.IContainer components = null;  

        /// <summary>  
        /// 使用中のリソースをすべてクリーンアップします。  
        /// </summary>  
        /// <param name="disposing">管理対象リソースを破棄する場合は true、それ以外の場合は false です。</param>  
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
            this.txtApiKey = new System.Windows.Forms.TextBox();  
            this.btnSave = new System.Windows.Forms.Button();  
            this.SuspendLayout();  
            //   
            // txtApiKey  
            //   
            this.txtApiKey.Location = new System.Drawing.Point(12, 12);  
            this.txtApiKey.Name = "txtApiKey";  
            this.txtApiKey.Size = new System.Drawing.Size(260, 20);  
            this.txtApiKey.TabIndex = 0;  
            //   
            // btnSave  
            //   
            this.btnSave.Location = new System.Drawing.Point(197, 38);  
            this.btnSave.Name = "btnSave";  
            this.btnSave.Size = new System.Drawing.Size(75, 23);  
            this.btnSave.TabIndex = 1;  
            this.btnSave.Text = "保存";  
            this.btnSave.UseVisualStyleBackColor = true;  
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);  
            //   
            // ApiSettingsDialog  
            //   
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);  
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;  
            this.ClientSize = new System.Drawing.Size(284, 71);  
            this.Controls.Add(this.btnSave);  
            this.Controls.Add(this.txtApiKey);  
            this.Name = "ApiSettingsDialog";  
            this.Text = "API設定";  
            this.ResumeLayout(false);  
            this.PerformLayout();  
        }  

        #endregion  

        private System.Windows.Forms.TextBox txtApiKey;  
        private System.Windows.Forms.Button btnSave;  
    }  
}  
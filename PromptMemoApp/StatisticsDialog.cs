using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class StatisticsDialog : Form
    {
        private Dictionary<string, object> statistics;
        private ListView listViewStats;
        private Button btnClose;

        public StatisticsDialog(Dictionary<string, object> statistics)
        {
            this.statistics = statistics;
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "統計情報";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // リストビュー
            this.listViewStats = new ListView();
            this.listViewStats.View = View.Details;
            this.listViewStats.FullRowSelect = true;
            this.listViewStats.GridLines = true;
            this.listViewStats.Location = new System.Drawing.Point(20, 20);
            this.listViewStats.Size = new System.Drawing.Size(450, 300);
            this.listViewStats.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));

            // カラムヘッダー
            this.listViewStats.Columns.Add("項目", 200);
            this.listViewStats.Columns.Add("値", 250);

            // 閉じるボタン
            this.btnClose = new Button();
            this.btnClose.Text = "閉じる";
            this.btnClose.Location = new System.Drawing.Point(390, 330);
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.DialogResult = DialogResult.OK;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                this.listViewStats,
                this.btnClose
            });

            this.ResumeLayout(false);
        }

        private void LoadStatistics()
        {
            listViewStats.Items.Clear();

            if (statistics.ContainsKey("Error"))
            {
                listViewStats.Items.Add(new ListViewItem(new string[] { "エラー", statistics["Error"].ToString() }));
                return;
            }

            // 基本統計
            if (statistics.ContainsKey("TotalCategories"))
            {
                listViewStats.Items.Add(new ListViewItem(new string[] { "総カテゴリ数", statistics["TotalCategories"].ToString() }));
            }

            if (statistics.ContainsKey("TotalFiles"))
            {
                listViewStats.Items.Add(new ListViewItem(new string[] { "総ファイル数", statistics["TotalFiles"].ToString() }));
            }

            if (statistics.ContainsKey("TotalSize"))
            {
                long totalSize = (long)statistics["TotalSize"];
                string sizeText = FormatFileSize(totalSize);
                listViewStats.Items.Add(new ListViewItem(new string[] { "総ファイルサイズ", sizeText }));
            }

            if (statistics.ContainsKey("AverageFilesPerCategory"))
            {
                double avg = (double)statistics["AverageFilesPerCategory"];
                listViewStats.Items.Add(new ListViewItem(new string[] { "カテゴリあたりの平均ファイル数", avg.ToString("F1") }));
            }

            // カテゴリ別統計
            if (statistics.ContainsKey("CategoryCounts"))
            {
                var categoryCounts = (Dictionary<string, int>)statistics["CategoryCounts"];
                listViewStats.Items.Add(new ListViewItem(new string[] { "", "" })); // 空行
                listViewStats.Items.Add(new ListViewItem(new string[] { "カテゴリ別ファイル数", "" }));

                foreach (var kvp in categoryCounts)
                {
                    listViewStats.Items.Add(new ListViewItem(new string[] { $"  {kvp.Key}", kvp.Value.ToString() }));
                }
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

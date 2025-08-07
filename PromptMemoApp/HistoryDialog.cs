using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class HistoryDialog : Form
    {
        private HistoryManager historyManager;
        private List<HistoryItem> allHistory;
        private Action<string, string> onHistorySelected;

        public HistoryDialog(HistoryManager historyManager, Action<string, string> onHistorySelected)
        {
            this.historyManager = historyManager;
            this.onHistorySelected = onHistorySelected;
            InitializeComponent();
            LoadHistory();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "履歴";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // カテゴリコンボボックス
            this.comboBoxCategories = new ComboBox();
            this.comboBoxCategories.Location = new System.Drawing.Point(10, 10);
            this.comboBoxCategories.Size = new System.Drawing.Size(200, 20);
            this.comboBoxCategories.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCategories.SelectedIndexChanged += ComboBoxCategories_SelectedIndexChanged;

            // ラベル
            var lblCategory = new Label();
            lblCategory.Text = "カテゴリ:";
            lblCategory.Location = new System.Drawing.Point(10, 35);
            lblCategory.Size = new System.Drawing.Size(100, 20);

            // リストビュー
            this.listViewHistory = new ListView();
            this.listViewHistory.Location = new System.Drawing.Point(10, 60);
            this.listViewHistory.Size = new System.Drawing.Size(660, 350);
            this.listViewHistory.View = View.Details;
            this.listViewHistory.FullRowSelect = true;
            this.listViewHistory.GridLines = true;
            this.listViewHistory.DoubleClick += ListViewHistory_DoubleClick;

            // リストビューのカラム
            this.listViewHistory.Columns.Add("カテゴリ", 100);
            this.listViewHistory.Columns.Add("ファイル名", 200);
            this.listViewHistory.Columns.Add("最終アクセス", 120);
            this.listViewHistory.Columns.Add("最終更新", 120);
            this.listViewHistory.Columns.Add("アクセス回数", 80);

            // ボタン
            this.btnOpen = new Button();
            this.btnOpen.Text = "開く";
            this.btnOpen.Location = new System.Drawing.Point(10, 420);
            this.btnOpen.Size = new System.Drawing.Size(80, 25);
            this.btnOpen.Click += BtnOpen_Click;

            this.btnRemove = new Button();
            this.btnRemove.Text = "削除";
            this.btnRemove.Location = new System.Drawing.Point(100, 420);
            this.btnRemove.Size = new System.Drawing.Size(80, 25);
            this.btnRemove.Click += BtnRemove_Click;

            this.btnClear = new Button();
            this.btnClear.Text = "全削除";
            this.btnClear.Location = new System.Drawing.Point(190, 420);
            this.btnClear.Size = new System.Drawing.Size(80, 25);
            this.btnClear.Click += BtnClear_Click;

            this.btnClose = new Button();
            this.btnClose.Text = "閉じる";
            this.btnClose.Location = new System.Drawing.Point(590, 420);
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.DialogResult = DialogResult.Cancel;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                this.comboBoxCategories,
                lblCategory,
                this.listViewHistory,
                this.btnOpen,
                this.btnRemove,
                this.btnClear,
                this.btnClose
            });

            this.ResumeLayout(false);
        }

        private ComboBox comboBoxCategories;
        private ListView listViewHistory;
        private Button btnOpen;
        private Button btnRemove;
        private Button btnClear;
        private Button btnClose;

        private void LoadHistory()
        {
            allHistory = historyManager.GetRecentHistory();
            
            // カテゴリリストを更新
            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add("すべて");
            var categories = allHistory.Select(h => h.Category).Distinct().OrderBy(c => c);
            comboBoxCategories.Items.AddRange(categories.ToArray());
            comboBoxCategories.SelectedIndex = 0;

            UpdateHistoryList();
        }

        private void UpdateHistoryList()
        {
            listViewHistory.Items.Clear();
            
            var selectedCategory = (comboBoxCategories.SelectedItem != null) ? comboBoxCategories.SelectedItem.ToString() : null;
            var history = selectedCategory == "すべて" || string.IsNullOrEmpty(selectedCategory) 
                ? allHistory 
                : historyManager.GetHistoryByCategory(selectedCategory);

            foreach (var item in history)
            {
                var listItem = new ListViewItem(item.Category);
                listItem.SubItems.Add(item.FileName);
                listItem.SubItems.Add(item.LastAccessed.ToString("yyyy/MM/dd HH:mm"));
                listItem.SubItems.Add(item.LastModified.ToString("yyyy/MM/dd HH:mm"));
                listItem.SubItems.Add(item.AccessCount.ToString());
                listItem.Tag = item;
                listViewHistory.Items.Add(listItem);
            }
        }

        private void ComboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateHistoryList();
        }

        private void ListViewHistory_DoubleClick(object sender, EventArgs e)
        {
            if (listViewHistory.SelectedItems.Count > 0)
            {
                OpenSelectedHistory();
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedHistory();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (listViewHistory.SelectedItems.Count > 0)
            {
                var item = listViewHistory.SelectedItems[0];
                var historyItem = item.Tag as HistoryItem;
                
                if (MessageBox.Show($"「{historyItem.FileName}」を履歴から削除しますか？", "確認", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    historyManager.RemoveFromHistory(historyItem.Category, historyItem.FileName);
                    LoadHistory();
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("すべての履歴を削除しますか？", "確認", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                historyManager.ClearHistory();
                LoadHistory();
            }
        }

        private void OpenSelectedHistory()
        {
            if (listViewHistory.SelectedItems.Count > 0)
            {
                var item = listViewHistory.SelectedItems[0];
                var historyItem = item.Tag as HistoryItem;
                
                onHistorySelected(historyItem.Category, historyItem.FileName);
                this.Close();
            }
        }
    }
}

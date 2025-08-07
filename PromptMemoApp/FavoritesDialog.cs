using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class FavoritesDialog : Form
    {
        private FavoritesManager favoritesManager;
        private List<FavoriteItem> allFavorites;
        private Action<string, string> onFavoriteSelected;

        public FavoritesDialog(FavoritesManager favoritesManager, Action<string, string> onFavoriteSelected)
        {
            this.favoritesManager = favoritesManager;
            this.onFavoriteSelected = onFavoriteSelected;
            InitializeComponent();
            LoadFavorites();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "お気に入り";
            this.Size = new System.Drawing.Size(600, 500);
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
            this.listViewFavorites = new ListView();
            this.listViewFavorites.Location = new System.Drawing.Point(10, 60);
            this.listViewFavorites.Size = new System.Drawing.Size(560, 350);
            this.listViewFavorites.View = View.Details;
            this.listViewFavorites.FullRowSelect = true;
            this.listViewFavorites.GridLines = true;
            this.listViewFavorites.DoubleClick += ListViewFavorites_DoubleClick;

            // リストビューのカラム
            this.listViewFavorites.Columns.Add("カテゴリ", 100);
            this.listViewFavorites.Columns.Add("ファイル名", 200);
            this.listViewFavorites.Columns.Add("追加日", 100);
            this.listViewFavorites.Columns.Add("アクセス回数", 80);
            this.listViewFavorites.Columns.Add("最終アクセス", 120);

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

            this.btnClose = new Button();
            this.btnClose.Text = "閉じる";
            this.btnClose.Location = new System.Drawing.Point(490, 420);
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.DialogResult = DialogResult.Cancel;

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                this.comboBoxCategories,
                lblCategory,
                this.listViewFavorites,
                this.btnOpen,
                this.btnRemove,
                this.btnClose
            });

            this.ResumeLayout(false);
        }

        private ComboBox comboBoxCategories;
        private ListView listViewFavorites;
        private Button btnOpen;
        private Button btnRemove;
        private Button btnClose;

        private void LoadFavorites()
        {
            allFavorites = favoritesManager.GetAllFavorites();
            
            // カテゴリリストを更新
            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add("すべて");
            var categories = allFavorites.Select(f => f.Category).Distinct().OrderBy(c => c);
            comboBoxCategories.Items.AddRange(categories.ToArray());
            comboBoxCategories.SelectedIndex = 0;

            UpdateFavoritesList();
        }

        private void UpdateFavoritesList()
        {
            listViewFavorites.Items.Clear();
            
            var selectedCategory = (comboBoxCategories.SelectedItem != null) ? comboBoxCategories.SelectedItem.ToString() : null;
            var favorites = selectedCategory == "すべて" || string.IsNullOrEmpty(selectedCategory) 
                ? allFavorites 
                : favoritesManager.GetFavoritesByCategory(selectedCategory);

            foreach (var favorite in favorites)
            {
                var item = new ListViewItem(favorite.Category);
                item.SubItems.Add(favorite.FileName);
                item.SubItems.Add(favorite.AddedDate.ToString("yyyy/MM/dd"));
                item.SubItems.Add(favorite.AccessCount.ToString());
                item.SubItems.Add(favorite.LastAccessed.ToString("yyyy/MM/dd HH:mm"));
                item.Tag = favorite;
                listViewFavorites.Items.Add(item);
            }
        }

        private void ComboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFavoritesList();
        }

        private void ListViewFavorites_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count > 0)
            {
                OpenSelectedFavorite();
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedFavorite();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (listViewFavorites.SelectedItems.Count > 0)
            {
                var item = listViewFavorites.SelectedItems[0];
                var favorite = item.Tag as FavoriteItem;
                
                if (MessageBox.Show($"「{favorite.FileName}」をお気に入りから削除しますか？", "確認", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    favoritesManager.RemoveFavorite(favorite.Category, favorite.FileName);
                    LoadFavorites();
                }
            }
        }

        private void OpenSelectedFavorite()
        {
            if (listViewFavorites.SelectedItems.Count > 0)
            {
                var item = listViewFavorites.SelectedItems[0];
                var favorite = item.Tag as FavoriteItem;
                
                favoritesManager.UpdateAccessCount(favorite.Category, favorite.FileName);
                onFavoriteSelected(favorite.Category, favorite.FileName);
                this.Close();
            }
        }
    }
}

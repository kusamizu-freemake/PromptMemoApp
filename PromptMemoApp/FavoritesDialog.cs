using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// お気に入り管理ダイアログ
    /// お気に入りの一覧表示、開く、削除の機能を提供する
    /// </summary>
    public partial class FavoritesDialog : Form
    {
        #region フィールド
        private readonly FavoritesManager _favoritesManager;
        private readonly Action<string, string> _onFavoriteSelected;
        private List<FavoriteItem> _allFavorites;

        // UI コントロール
        private ComboBox _comboBoxCategories;
        private ListView _listViewFavorites;
        private Button _btnOpen;
        private Button _btnRemove;
        private Button _btnClose;
        #endregion

        #region 定数
        private const string ALL_CATEGORIES = "すべて";
        #endregion

        #region コンストラクタ
        /// <summary>
        /// FavoritesDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="favoritesManager">お気に入り管理クラス</param>
        /// <param name="onFavoriteSelected">お気に入り選択時のコールバック</param>
        public FavoritesDialog(FavoritesManager favoritesManager, Action<string, string> onFavoriteSelected)
        {
            // null チェック
            _favoritesManager = favoritesManager ?? throw new ArgumentNullException(nameof(favoritesManager));
            _onFavoriteSelected = onFavoriteSelected ?? throw new ArgumentNullException(nameof(onFavoriteSelected));

            InitializeComponent();
            LoadFavorites();
        }
        #endregion

        #region UI初期化
        /// <summary>
        /// フォームのコンポーネントを初期化します
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            SetupForm();
            CreateControls();
            LayoutControls();
            AddControlsToForm();

            ResumeLayout(false);
        }

        /// <summary>
        /// フォームの基本設定を行います
        /// </summary>
        private void SetupForm()
        {
            Text = "お気に入り";
            Size = new System.Drawing.Size(600, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        /// <summary>
        /// UIコントロールを作成します
        /// </summary>
        private void CreateControls()
        {
            CreateCategoryComboBox();
            CreateFavoritesListView();
            CreateButtons();
        }

        /// <summary>
        /// カテゴリ選択用のコンボボックスを作成します
        /// </summary>
        private void CreateCategoryComboBox()
        {
            _comboBoxCategories = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _comboBoxCategories.SelectedIndexChanged += OnCategorySelectionChanged;
        }

        /// <summary>
        /// お気に入り一覧表示用のリストビューを作成します
        /// </summary>
        private void CreateFavoritesListView()
        {
            _listViewFavorites = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            // カラムを追加
            _listViewFavorites.Columns.Add("カテゴリ", 100);
            _listViewFavorites.Columns.Add("ファイル名", 200);
            _listViewFavorites.Columns.Add("追加日", 100);
            _listViewFavorites.Columns.Add("アクセス回数", 80);
            _listViewFavorites.Columns.Add("最終アクセス", 120);

            _listViewFavorites.DoubleClick += OnFavoritesListDoubleClick;
        }

        /// <summary>
        /// ボタンコントロールを作成します
        /// </summary>
        private void CreateButtons()
        {
            _btnOpen = new Button
            {
                Text = "開く"
            };
            _btnOpen.Click += OnOpenButtonClick;

            _btnRemove = new Button
            {
                Text = "削除"
            };
            _btnRemove.Click += OnRemoveButtonClick;

            _btnClose = new Button
            {
                Text = "閉じる",
                DialogResult = DialogResult.Cancel
            };
        }

        /// <summary>
        /// コントロールの配置を設定します
        /// </summary>
        private void LayoutControls()
        {
            _comboBoxCategories.Location = new System.Drawing.Point(10, 10);
            _comboBoxCategories.Size = new System.Drawing.Size(200, 20);

            var lblCategory = new Label
            {
                Text = "カテゴリ:",
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(100, 20)
            };

            _listViewFavorites.Location = new System.Drawing.Point(10, 60);
            _listViewFavorites.Size = new System.Drawing.Size(560, 350);

            _btnOpen.Location = new System.Drawing.Point(10, 420);
            _btnOpen.Size = new System.Drawing.Size(80, 25);

            _btnRemove.Location = new System.Drawing.Point(100, 420);
            _btnRemove.Size = new System.Drawing.Size(80, 25);

            _btnClose.Location = new System.Drawing.Point(490, 420);
            _btnClose.Size = new System.Drawing.Size(80, 25);

            // ラベルもコントロールとして管理
            Controls.Add(lblCategory);
        }

        /// <summary>
        /// フォームにコントロールを追加します
        /// </summary>
        private void AddControlsToForm()
        {
            Controls.AddRange(new Control[] {
                _comboBoxCategories,
                _listViewFavorites,
                _btnOpen,
                _btnRemove,
                _btnClose
            });
        }
        #endregion

        #region データ操作
        /// <summary>
        /// お気に入りデータを読み込みます
        /// </summary>
        private void LoadFavorites()
        {
            try
            {
                _allFavorites = _favoritesManager.GetAllFavorites();
                UpdateCategoriesComboBox();
                UpdateFavoritesList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("お気に入りの読み込み中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// カテゴリコンボボックスを更新します
        /// </summary>
        private void UpdateCategoriesComboBox()
        {
            _comboBoxCategories.Items.Clear();
            _comboBoxCategories.Items.Add(ALL_CATEGORIES);

            var categories = _allFavorites
                .Select(f => f.Category)
                .Distinct()
                .OrderBy(c => c);

            _comboBoxCategories.Items.AddRange(categories.ToArray());
            _comboBoxCategories.SelectedIndex = 0;
        }

        /// <summary>
        /// お気に入り一覧を更新します
        /// </summary>
        private void UpdateFavoritesList()
        {
            _listViewFavorites.Items.Clear();

            var selectedCategory = _comboBoxCategories.SelectedItem?.ToString();
            var favorites = GetFilteredFavorites(selectedCategory);

            foreach (var favorite in favorites)
            {
                AddFavoriteToListView(favorite);
            }
        }

        /// <summary>
        /// 選択されたカテゴリでフィルタされたお気に入りを取得します
        /// </summary>
        /// <param name="selectedCategory">選択されたカテゴリ</param>
        /// <returns>フィルタされたお気に入りリスト</returns>
        private IEnumerable<FavoriteItem> GetFilteredFavorites(string selectedCategory)
        {
            if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == ALL_CATEGORIES)
            {
                return _allFavorites;
            }

            return _favoritesManager.GetFavoritesByCategory(selectedCategory);
        }

        /// <summary>
        /// お気に入りアイテムをリストビューに追加します
        /// </summary>
        /// <param name="favorite">追加するお気に入りアイテム</param>
        private void AddFavoriteToListView(FavoriteItem favorite)
        {
            var item = new ListViewItem(favorite.Category);
            item.SubItems.Add(favorite.FileName);
            item.SubItems.Add(favorite.AddedDate.ToString("yyyy/MM/dd"));
            item.SubItems.Add(favorite.AccessCount.ToString());
            item.SubItems.Add(favorite.LastAccessed.ToString("yyyy/MM/dd HH:mm"));
            item.Tag = favorite;

            _listViewFavorites.Items.Add(item);
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// カテゴリ選択変更時の処理
        /// </summary>
        private void OnCategorySelectionChanged(object sender, EventArgs e)
        {
            UpdateFavoritesList();
        }

        /// <summary>
        /// リストビューダブルクリック時の処理
        /// </summary>
        private void OnFavoritesListDoubleClick(object sender, EventArgs e)
        {
            if (HasSelectedFavorite())
            {
                OpenSelectedFavorite();
            }
        }

        /// <summary>
        /// 開くボタンクリック時の処理
        /// </summary>
        private void OnOpenButtonClick(object sender, EventArgs e)
        {
            if (!HasSelectedFavorite())
            {
                MessageBox.Show("開くお気に入りを選択してください。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OpenSelectedFavorite();
        }

        /// <summary>
        /// 削除ボタンクリック時の処理
        /// </summary>
        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            if (!HasSelectedFavorite())
            {
                MessageBox.Show("削除するお気に入りを選択してください。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            RemoveSelectedFavorite();
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// お気に入りが選択されているかチェックします
        /// </summary>
        /// <returns>選択されている場合は true</returns>
        private bool HasSelectedFavorite()
        {
            return _listViewFavorites.SelectedItems.Count > 0;
        }

        /// <summary>
        /// 選択されたお気に入りを開きます
        /// </summary>
        private void OpenSelectedFavorite()
        {
            try
            {
                var favorite = GetSelectedFavorite();
                if (favorite == null) return;

                _favoritesManager.UpdateAccessCount(favorite.Category, favorite.FileName);
                _onFavoriteSelected(favorite.Category, favorite.FileName);
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("お気に入りを開く際にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// 選択されたお気に入りを削除します
        /// </summary>
        private void RemoveSelectedFavorite()
        {
            try
            {
                var favorite = GetSelectedFavorite();
                if (favorite == null) return;

                var result = MessageBox.Show(
                    $"「{favorite.FileName}」をお気に入りから削除しますか？",
                    "削除確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _favoritesManager.RemoveFavorite(favorite.Category, favorite.FileName);
                    LoadFavorites();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("お気に入りの削除中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// 選択されたお気に入りアイテムを取得します
        /// </summary>
        /// <returns>選択されたお気に入りアイテム、選択されていない場合は null</returns>
        private FavoriteItem GetSelectedFavorite()
        {
            if (!HasSelectedFavorite()) return null;

            var item = _listViewFavorites.SelectedItems[0];
            return item.Tag as FavoriteItem;
        }

        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(fullMessage, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}
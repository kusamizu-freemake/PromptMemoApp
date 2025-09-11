using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace PromptMemoApp
{
    /// <summary>
    /// ショートカット設定フォーム
    /// ユーザーが各機能のキーボードショートカットを設定・変更できるフォーム
    /// </summary>
    public partial class ShortcutSettingsForm : Form
    {
        #region フィールド
        private Dictionary<string, Keys> _currentShortcuts;
        private readonly Dictionary<string, Keys> _originalShortcuts;

        // UI要素（デザイナーが自動生成する代わりに手動定義）
        private ListBox listBoxShortcuts;
        private Button btnChange;
        private Button btnReset;
        private Button btnOK;
        private Button btnCancel;
        private Label lblCurrent;
        private Label lblShortcut;
        #endregion

        #region 定数
        private const string FORM_TITLE = "ショートカット設定";
        private const string NO_SHORTCUT_TEXT = "なし";
        private const string ERROR_TITLE = "エラー";
        private const char KEY_SEPARATOR = ':';

        // フォームサイズ関連の定数
        private const int FORM_WIDTH = 500;
        private const int FORM_HEIGHT = 400;
        private const int MARGIN = 10;
        private const int BUTTON_WIDTH = 80;
        private const int BUTTON_HEIGHT = 25;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在設定されているショートカット一覧を取得します
        /// </summary>
        public Dictionary<string, Keys> CurrentShortcuts => new Dictionary<string, Keys>(_currentShortcuts);
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ShortcutSettingsForm の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="shortcuts">現在のショートカット設定</param>
        /// <exception cref="ArgumentNullException">shortcuts が null の場合</exception>
        public ShortcutSettingsForm(Dictionary<string, Keys> shortcuts)
        {
            if (shortcuts == null)
                throw new ArgumentNullException(nameof(shortcuts), "ショートカット設定は必須です。");

            // ショートカット設定の初期化（元の設定と現在の設定を分けて管理）
            _currentShortcuts = new Dictionary<string, Keys>(shortcuts);
            _originalShortcuts = new Dictionary<string, Keys>(shortcuts);

            InitializeComponent();
            SetupForm();
            LoadShortcutList();
        }
        #endregion

        #region UI初期化
        /// <summary>
        /// コンポーネントの初期化を行います
        /// デザイナーファイルの代わりとなる手動初期化
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            try
            {
                // フォーム基本設定
                Text = FORM_TITLE;
                Size = new System.Drawing.Size(FORM_WIDTH, FORM_HEIGHT);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                // UI要素の作成と配置
                CreateListBox();
                CreateButtons();
                CreateLabels();

                // イベントハンドラーの設定
                SetupEventHandlers();

                // すべてのコントロールをフォームに追加
                Controls.AddRange(new Control[] {
                    listBoxShortcuts,
                    btnChange,
                    btnReset,
                    btnOK,
                    btnCancel,
                    lblCurrent,
                    lblShortcut
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] UI初期化エラー: {ex.Message}");
                throw new InvalidOperationException("UI初期化中にエラーが発生しました。", ex);
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        /// <summary>
        /// リストボックスを作成します
        /// </summary>
        private void CreateListBox()
        {
            listBoxShortcuts = new ListBox
            {
                Location = new System.Drawing.Point(MARGIN, MARGIN),
                Size = new System.Drawing.Size(300, 300),
                Name = "listBoxShortcuts"
            };
        }

        /// <summary>
        /// ボタン類を作成します
        /// </summary>
        private void CreateButtons()
        {
            // 変更ボタン
            btnChange = new Button
            {
                Text = "変更",
                Location = new System.Drawing.Point(320, MARGIN),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                Name = "btnChange"
            };

            // リセットボタン
            btnReset = new Button
            {
                Text = "リセット",
                Location = new System.Drawing.Point(320, 45),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                Name = "btnReset"
            };

            // OKボタン
            btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(320, 250),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                DialogResult = DialogResult.OK,
                Name = "btnOK"
            };

            // キャンセルボタン
            btnCancel = new Button
            {
                Text = "キャンセル",
                Location = new System.Drawing.Point(320, 285),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                DialogResult = DialogResult.Cancel,
                Name = "btnCancel"
            };
        }

        /// <summary>
        /// ラベル類を作成します
        /// </summary>
        private void CreateLabels()
        {
            // 現在のショートカットラベル
            lblCurrent = new Label
            {
                Text = "現在のショートカット:",
                Location = new System.Drawing.Point(MARGIN, 320),
                Size = new System.Drawing.Size(150, 20),
                Name = "lblCurrent"
            };

            // ショートカット表示ラベル
            lblShortcut = new Label
            {
                Text = NO_SHORTCUT_TEXT,
                Location = new System.Drawing.Point(170, 320),
                Size = new System.Drawing.Size(200, 20),
                Name = "lblShortcut"
            };
        }

        /// <summary>
        /// イベントハンドラーを設定します
        /// </summary>
        private void SetupEventHandlers()
        {
            if (listBoxShortcuts != null)
                listBoxShortcuts.SelectedIndexChanged += ListBoxShortcuts_SelectedIndexChanged;

            if (btnChange != null)
                btnChange.Click += BtnChange_Click;

            if (btnReset != null)
                btnReset.Click += BtnReset_Click;

            if (btnOK != null)
                btnOK.Click += BtnOK_Click;

            if (btnCancel != null)
                btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// フォームの追加設定を行います
        /// </summary>
        private void SetupForm()
        {
            // ボタンの既定動作設定
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            // 初期状態の設定
            UpdateCurrentShortcutDisplay();
        }
        #endregion

        #region データ操作
        /// <summary>
        /// ショートカット一覧をリストボックスに読み込みます
        /// </summary>
        private void LoadShortcutList()
        {
            try
            {
                listBoxShortcuts.Items.Clear();

                // ショートカット設定を読みやすい形式でリストに表示
                foreach (var shortcut in _currentShortcuts.OrderBy(x => x.Key))
                {
                    var displayText = FormatShortcutDisplayText(shortcut.Key, shortcut.Value);
                    listBoxShortcuts.Items.Add(displayText);
                }

                // 最初の項目を選択状態にする
                if (listBoxShortcuts.Items.Count > 0)
                {
                    listBoxShortcuts.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ショートカット一覧の読み込み中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// ショートカットの表示テキストをフォーマットします
        /// </summary>
        /// <param name="actionName">アクション名</param>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <returns>フォーマット済みの表示テキスト</returns>
        private string FormatShortcutDisplayText(string actionName, Keys shortcutKey)
        {
            var keyDisplayText = shortcutKey == Keys.None ? NO_SHORTCUT_TEXT : shortcutKey.ToString();
            return $"{actionName}{KEY_SEPARATOR} {keyDisplayText}";
        }

        /// <summary>
        /// 現在選択されているショートカットの表示を更新します
        /// </summary>
        private void UpdateCurrentShortcutDisplay()
        {
            if (listBoxShortcuts.SelectedIndex >= 0)
            {
                var selectedActionName = GetSelectedActionName();
                if (!string.IsNullOrEmpty(selectedActionName) && _currentShortcuts.ContainsKey(selectedActionName))
                {
                    var shortcutKey = _currentShortcuts[selectedActionName];
                    lblShortcut.Text = shortcutKey == Keys.None ? NO_SHORTCUT_TEXT : shortcutKey.ToString();
                    return;
                }
            }

            lblShortcut.Text = NO_SHORTCUT_TEXT;
        }

        /// <summary>
        /// 現在選択されているアクション名を取得します
        /// </summary>
        /// <returns>選択されているアクション名、選択されていない場合は null</returns>
        private string GetSelectedActionName()
        {
            if (listBoxShortcuts.SelectedIndex < 0 || listBoxShortcuts.SelectedItem == null)
                return null;

            try
            {
                var selectedText = listBoxShortcuts.SelectedItem.ToString();
                var separatorIndex = selectedText.IndexOf(KEY_SEPARATOR);

                return separatorIndex > 0 ? selectedText.Substring(0, separatorIndex) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] アクション名取得エラー: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// リストボックスの選択項目変更時の処理
        /// </summary>
        private void ListBoxShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurrentShortcutDisplay();
        }

        /// <summary>
        /// 変更ボタンクリック時の処理
        /// </summary>
        private void BtnChange_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedActionName = GetSelectedActionName();
                if (string.IsNullOrEmpty(selectedActionName))
                {
                    MessageBox.Show(this, "変更する項目を選択してください。", FORM_TITLE,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var currentKey = _currentShortcuts.ContainsKey(selectedActionName)
                    ? _currentShortcuts[selectedActionName]
                    : Keys.None;

                // ショートカット入力ダイアログを表示
                using (var dialog = new ShortcutInputDialog(selectedActionName, currentKey))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        // 新しいショートカットを設定
                        _currentShortcuts[selectedActionName] = dialog.ShortcutKey;

                        // 表示を更新
                        LoadShortcutList();
                        UpdateCurrentShortcutDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ショートカット変更中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// リセットボタンクリック時の処理
        /// </summary>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(this,
                    "すべてのショートカット設定を元に戻しますか？",
                    FORM_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // 元の設定に戻す
                    _currentShortcuts = new Dictionary<string, Keys>(_originalShortcuts);

                    // 表示を更新
                    LoadShortcutList();
                    UpdateCurrentShortcutDisplay();

                    MessageBox.Show(this, "ショートカット設定をリセットしました。", FORM_TITLE,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ショートカットリセット中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(this, fullMessage, ERROR_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}: {ex}");
        }
        #endregion

        #region パブリックメソッド
        /// <summary>
        /// 現在のショートカット設定を取得します
        /// 呼び出し元が設定を変更できないよう、新しいDictionaryインスタンスを返します
        /// </summary>
        /// <returns>現在のショートカット設定のコピー</returns>
        public Dictionary<string, Keys> GetShortcuts()
        {
            return new Dictionary<string, Keys>(_currentShortcuts);
        }
        #endregion
    }

    /// <summary>
    /// ショートカットキー入力ダイアログ
    /// ユーザーが特定のアクションに対してキーボードショートカットを設定するためのダイアログ
    /// </summary>
    public class ShortcutInputDialog : Form
    {
        #region 定数
        private const string DIALOG_TITLE_FORMAT = "ショートカット設定 - {0}";
        private const string INSTRUCTION_FORMAT = "「{0}」のショートカットキーを入力してください:";
        private const string ERROR_TITLE = "エラー";
        private const string NO_KEY_TEXT = "なし";

        // フォームサイズ関連の定数
        private const int DIALOG_WIDTH = 400;
        private const int DIALOG_HEIGHT = 150;
        private const int MARGIN = 10;
        private const int BUTTON_WIDTH = 80;
        private const int BUTTON_HEIGHT = 25;
        #endregion

        #region プロパティ
        /// <summary>
        /// ユーザーが入力したショートカットキーを取得します
        /// </summary>
        public Keys ShortcutKey { get; private set; }
        #endregion

        #region フィールド
        private TextBox _txtShortcut;
        private Button _btnOK;
        private Button _btnCancel;
        private Label _lblInstruction;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ShortcutInputDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="actionName">設定対象のアクション名</param>
        /// <param name="currentKey">現在設定されているショートカットキー</param>
        /// <exception cref="ArgumentException">actionName が null または空の場合</exception>
        public ShortcutInputDialog(string actionName, Keys currentKey)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("アクション名は必須です。", nameof(actionName));

            ShortcutKey = currentKey;

            InitializeComponent(actionName);
            SetupDialog(actionName, currentKey);
        }
        #endregion

        #region UI初期化
        /// <summary>
        /// コンポーネントの初期化を行います
        /// </summary>
        /// <param name="actionName">アクション名</param>
        private void InitializeComponent(string actionName)
        {
            SuspendLayout();

            try
            {
                // フォーム基本設定
                Text = string.Format(DIALOG_TITLE_FORMAT, actionName);
                Size = new System.Drawing.Size(DIALOG_WIDTH, DIALOG_HEIGHT);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                // UI要素の作成
                CreateControls(actionName);

                // イベントハンドラーの設定
                SetupEventHandlers();

                // コントロールをフォームに追加
                Controls.AddRange(new Control[] {
                    _lblInstruction,
                    _txtShortcut,
                    _btnOK,
                    _btnCancel
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] ダイアログUI初期化エラー: {ex.Message}");
                throw new InvalidOperationException("ダイアログ初期化中にエラーが発生しました。", ex);
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        /// <summary>
        /// コントロールを作成します
        /// </summary>
        /// <param name="actionName">アクション名</param>
        private void CreateControls(string actionName)
        {
            // 説明ラベル
            _lblInstruction = new Label
            {
                Text = string.Format(INSTRUCTION_FORMAT, actionName),
                Location = new System.Drawing.Point(MARGIN, MARGIN),
                Size = new System.Drawing.Size(350, 20)
            };

            // ショートカット表示テキストボックス
            _txtShortcut = new TextBox
            {
                Location = new System.Drawing.Point(MARGIN, 40),
                Size = new System.Drawing.Size(350, 20),
                ReadOnly = true
            };

            // OKボタン
            _btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(200, 80),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                DialogResult = DialogResult.OK
            };

            // キャンセルボタン
            _btnCancel = new Button
            {
                Text = "キャンセル",
                Location = new System.Drawing.Point(290, 80),
                Size = new System.Drawing.Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                DialogResult = DialogResult.Cancel
            };
        }

        /// <summary>
        /// イベントハンドラーを設定します
        /// </summary>
        private void SetupEventHandlers()
        {
            if (_txtShortcut != null)
                _txtShortcut.KeyDown += TxtShortcut_KeyDown;

            if (_btnOK != null)
                _btnOK.Click += BtnOK_Click;

            if (_btnCancel != null)
                _btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// ダイアログの設定を行います
        /// </summary>
        /// <param name="actionName">アクション名</param>
        /// <param name="currentKey">現在のショートカットキー</param>
        private void SetupDialog(string actionName, Keys currentKey)
        {
            try
            {
                // 既定ボタン設定
                AcceptButton = _btnOK;
                CancelButton = _btnCancel;

                // 初期値設定
                UpdateShortcutDisplay(currentKey);

                // フォーカス設定
                if (_txtShortcut != null)
                {
                    _txtShortcut.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("ダイアログ初期化中にエラーが発生しました。", ex);
            }
        }
        #endregion

        #region データ操作
        /// <summary>
        /// ショートカット表示を更新します
        /// </summary>
        /// <param name="key">表示するキー</param>
        private void UpdateShortcutDisplay(Keys key)
        {
            if (_txtShortcut != null)
            {
                _txtShortcut.Text = key == Keys.None ? NO_KEY_TEXT : key.ToString();
            }
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// ショートカット入力テキストボックスのキー押下時の処理
        /// </summary>
        private void TxtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // キーの既定動作を無効化
                e.SuppressKeyPress = true;
                e.Handled = true;

                // 押下されたキーを保存
                ShortcutKey = e.KeyData;

                // 表示を更新
                UpdateShortcutDisplay(e.KeyData);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("キー入力処理中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// OKボタンクリック時の処理
        /// </summary>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// キャンセルボタンクリック時の処理
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(this, fullMessage, ERROR_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}: {ex}");
        }
        #endregion
    }
}
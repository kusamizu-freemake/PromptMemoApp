using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// ソート設定ダイアログ
    /// ファイルの並び替え設定（項目と順序）を行う
    /// </summary>
    public partial class SortSettingsDialog : Form
    {
        #region フィールド
        private readonly PromptEditorForm _parentForm;
        #endregion

        #region 定数
        private const string DIALOG_TITLE = "並び替え設定";
        private const int DIALOG_WIDTH = 400;
        private const int DIALOG_HEIGHT = 200;
        #endregion

        #region プロパティ
        /// <summary>
        /// 選択された並び替えフィールド
        /// </summary>
        public SortField SelectedSortField { get; private set; }

        /// <summary>
        /// 選択された並び替え順序
        /// </summary>
        public SortOrder SelectedSortOrder { get; private set; }
        #endregion

        #region 列挙型
        /// <summary>
        /// 並び替えフィールドの種類
        /// </summary>
        public enum SortField
        {
            Name,       // ファイル名
            Created,    // 作成日時
            Modified,   // 更新日時
            Size        // ファイルサイズ
        }

        /// <summary>
        /// 並び替え順序の種類
        /// </summary>
        public enum SortOrder
        {
            Ascending,  // 昇順
            Descending  // 降順
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// SortSettingsDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="parentForm">親フォーム</param>
        /// <exception cref="ArgumentNullException">parentForm が null の場合</exception>
        public SortSettingsDialog(PromptEditorForm parentForm)
        {
            _parentForm = parentForm ?? throw new ArgumentNullException(nameof(parentForm));

            InitializeComponent();
            SetupDialog();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// ダイアログの追加設定を行います
        /// </summary>
        private void SetupDialog()
        {
            // フォーム設定
            Text = DIALOG_TITLE;
            Size = new System.Drawing.Size(DIALOG_WIDTH, DIALOG_HEIGHT);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // キーボードショートカットの設定
            SetupKeyboardShortcuts();

            // デフォルト値の設定
            SetDefaultValues();
        }

        /// <summary>
        /// キーボードショートカットを設定します
        /// </summary>
        private void SetupKeyboardShortcuts()
        {
            if (btnApply != null)
            {
                AcceptButton = btnApply;
            }

            if (btnCancel != null)
            {
                CancelButton = btnCancel;
            }
        }

        /// <summary>
        /// デフォルト値を設定します
        /// </summary>
        private void SetDefaultValues()
        {
            if (comboBoxSortField != null)
            {
                comboBoxSortField.SelectedIndex = 0;
                SelectedSortField = SortField.Name;
            }

            if (comboBoxSortOrder != null)
            {
                comboBoxSortOrder.SelectedIndex = 0;
                SelectedSortOrder = SortOrder.Ascending;
            }
        }

        /// <summary>
        /// コンポーネントを初期化します
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            CreateLabels();
            CreateComboBoxes();
            CreateButtons();

            ResumeLayout(false);
        }

        /// <summary>
        /// ラベルを作成します
        /// </summary>
        private void CreateLabels()
        {
            // 並び替えフィールドラベル
            var lblSortField = new Label
            {
                Text = "並び替え項目:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            // 並び替え順序ラベル
            var lblSortOrder = new Label
            {
                Text = "並び替え順序:",
                Location = new System.Drawing.Point(200, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            // 説明ラベル
            var lblDescription = new Label
            {
                Text = "ファイルの表示順序を設定します。設定後は「適用」ボタンをクリックしてください。",
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(350, 40),
                AutoSize = false
            };

            Controls.AddRange(new Control[] { lblSortField, lblSortOrder, lblDescription });
        }

        /// <summary>
        /// コンボボックスを作成します
        /// </summary>
        private void CreateComboBoxes()
        {
            // 並び替えフィールドコンボボックス
            comboBoxSortField = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(150, 23)
            };

            comboBoxSortField.Items.AddRange(new object[]
            {
                "ファイル名",
                "作成日時",
                "更新日時",
                "ファイルサイズ"
            });

            comboBoxSortField.SelectedIndexChanged += ComboBoxSortField_SelectedIndexChanged;

            // 並び替え順序コンボボックス
            comboBoxSortOrder = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(200, 45),
                Size = new System.Drawing.Size(150, 23)
            };

            comboBoxSortOrder.Items.AddRange(new object[]
            {
                "昇順",
                "降順"
            });

            comboBoxSortOrder.SelectedIndexChanged += ComboBoxSortOrder_SelectedIndexChanged;

            Controls.AddRange(new Control[] { comboBoxSortField, comboBoxSortOrder });
        }

        /// <summary>
        /// ボタンを作成します
        /// </summary>
        private void CreateButtons()
        {
            // 適用ボタン
            btnApply = new Button
            {
                Text = "適用",
                Location = new System.Drawing.Point(200, 120),
                Size = new System.Drawing.Size(80, 25)
            };
            btnApply.Click += BtnApply_Click;

            // キャンセルボタン
            btnCancel = new Button
            {
                Text = "キャンセル",
                Location = new System.Drawing.Point(290, 120),
                Size = new System.Drawing.Size(80, 25),
                DialogResult = DialogResult.Cancel
            };

            Controls.AddRange(new Control[] { btnApply, btnCancel });
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// 並び替えフィールドの選択変更時の処理
        /// </summary>
        private void ComboBoxSortField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSortField?.SelectedIndex >= 0)
            {
                SelectedSortField = (SortField)comboBoxSortField.SelectedIndex;
            }
        }

        /// <summary>
        /// 並び替え順序の選択変更時の処理
        /// </summary>
        private void ComboBoxSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSortOrder?.SelectedIndex >= 0)
            {
                SelectedSortOrder = (SortOrder)comboBoxSortOrder.SelectedIndex;
            }
        }

        /// <summary>
        /// 適用ボタンクリック時の処理
        /// </summary>
        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                {
                    return;
                }

                ApplySortSettings();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("並び替え設定の適用中にエラーが発生しました。", ex);
            }
        }
        #endregion

        #region ビジネスロジック
        /// <summary>
        /// 入力値の妥当性を検証します
        /// </summary>
        /// <returns>入力が有効な場合は true</returns>
        private bool ValidateInput()
        {
            if (comboBoxSortField?.SelectedIndex < 0)
            {
                ShowValidationError("並び替え項目を選択してください。");
                comboBoxSortField?.Focus();
                return false;
            }

            if (comboBoxSortOrder?.SelectedIndex < 0)
            {
                ShowValidationError("並び替え順序を選択してください。");
                comboBoxSortOrder?.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 並び替え設定を適用します
        /// </summary>
        private void ApplySortSettings()
        {
            var sortFieldString = ConvertSortFieldToString(SelectedSortField);

            // 注意: 現在のSortFilesメソッドは並び替えフィールドのみを受け取ります
            // 将来的に並び替え順序も対応する場合は、parentFormのSortFilesメソッドを拡張する必要があります
            _parentForm.SortFiles(sortFieldString);

            System.Diagnostics.Debug.WriteLine($"[DEBUG] 並び替え設定適用: {sortFieldString}, 選択順序: {SelectedSortOrder}");
        }

        /// <summary>
        /// SortField列挙型を文字列に変換します
        /// </summary>
        /// <param name="sortField">変換するSortField</param>
        /// <returns>対応する文字列</returns>
        private string ConvertSortFieldToString(SortField sortField)
        {
            return sortField switch
            {
                SortField.Name => "Name",
                SortField.Created => "Created",
                SortField.Modified => "Modified",
                SortField.Size => "Size",
                _ => "Name" // デフォルト値
            };
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// バリデーションエラーメッセージを表示します
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(this, message, "入力エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示します
        /// </summary>
        /// <param name="message">ユーザー向けメッセージ</param>
        /// <param name="ex">発生した例外</param>
        private void ShowErrorMessage(string message, Exception ex)
        {
            var fullMessage = $"{message}\n\n詳細: {ex.Message}";
            MessageBox.Show(this, fullMessage, "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            // デバッグログ出力
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}: {ex}");
        }
        #endregion

        #region フィールド（Designer用）
        private ComboBox comboBoxSortField;
        private ComboBox comboBoxSortOrder;
        private Button btnApply;
        private Button btnCancel;
        #endregion
    }
}
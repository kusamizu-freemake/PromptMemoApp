using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// データのエクスポート/インポート機能を提供するダイアログ
    /// JSONファイルを使用してアプリケーションデータの保存・読み込みを行います
    /// </summary>
    public partial class ExportImportDialog : Form
    {
        #region フィールド
        private readonly PromptEditorForm _parentForm;
        private Button _btnExport;
        private Button _btnImport;
        private Button _btnCancel;
        private Label _lblDescription;
        #endregion

        #region 定数
        private const string DIALOG_TITLE = "エクスポート/インポート";
        private const string JSON_FILTER = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*";
        private const string DEFAULT_EXTENSION = "json";
        private const string ERROR_TITLE = "エラー";
        private const string CONFIRM_TITLE = "確認";
        private const string EXPORT_FILENAME_FORMAT = "PromptMemoApp_Export_{0:yyyyMMdd_HHmmss}.json";
        #endregion

        #region コンストラクタ
        /// <summary>
        /// ExportImportDialog の新しいインスタンスを初期化します
        /// </summary>
        /// <param name="parentForm">親フォーム</param>
        /// <exception cref="ArgumentNullException">parentForm が null の場合</exception>
        public ExportImportDialog(PromptEditorForm parentForm)
        {
            _parentForm = parentForm ?? throw new ArgumentNullException(nameof(parentForm));
            InitializeComponent();
            SetupDialog();
        }
        #endregion

        #region 初期化
        /// <summary>
        /// コンポーネントの初期化を行います
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            SetupForm();
            CreateControls();
            AddControlsToForm();

            ResumeLayout(false);
        }

        /// <summary>
        /// フォームの基本設定を行います
        /// </summary>
        private void SetupForm()
        {
            Text = DIALOG_TITLE;
            Size = new System.Drawing.Size(400, 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        /// <summary>
        /// コントロールを作成します
        /// </summary>
        private void CreateControls()
        {
            CreateDescriptionLabel();
            CreateExportButton();
            CreateImportButton();
            CreateCancelButton();
        }

        /// <summary>
        /// 説明ラベルを作成します
        /// </summary>
        private void CreateDescriptionLabel()
        {
            _lblDescription = new Label
            {
                Text = "データのエクスポートまたはインポートを選択してください。\n\n" +
                       "エクスポート: 現在のデータをJSONファイルに保存します。\n" +
                       "インポート: JSONファイルからデータを読み込みます。",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 80),
                AutoSize = false
            };
        }

        /// <summary>
        /// エクスポートボタンを作成します
        /// </summary>
        private void CreateExportButton()
        {
            _btnExport = new Button
            {
                Text = "エクスポート",
                Location = new System.Drawing.Point(50, 120),
                Size = new System.Drawing.Size(120, 40)
            };
            _btnExport.Click += BtnExport_Click;
        }

        /// <summary>
        /// インポートボタンを作成します
        /// </summary>
        private void CreateImportButton()
        {
            _btnImport = new Button
            {
                Text = "インポート",
                Location = new System.Drawing.Point(200, 120),
                Size = new System.Drawing.Size(120, 40)
            };
            _btnImport.Click += BtnImport_Click;
        }

        /// <summary>
        /// キャンセルボタンを作成します
        /// </summary>
        private void CreateCancelButton()
        {
            _btnCancel = new Button
            {
                Text = "キャンセル",
                Location = new System.Drawing.Point(290, 180),
                Size = new System.Drawing.Size(80, 25),
                DialogResult = DialogResult.Cancel
            };
        }

        /// <summary>
        /// フォームにコントロールを追加します
        /// </summary>
        private void AddControlsToForm()
        {
            Controls.AddRange(new Control[] {
                _lblDescription,
                _btnExport,
                _btnImport,
                _btnCancel
            });
        }

        /// <summary>
        /// ダイアログの追加設定を行います
        /// </summary>
        private void SetupDialog()
        {
            CancelButton = _btnCancel;
        }
        #endregion

        #region イベントハンドラ
        /// <summary>
        /// エクスポートボタンクリック時の処理
        /// </summary>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var fileName = GetExportFileName();
                if (string.IsNullOrEmpty(fileName))
                    return;

                ExportData(fileName);
                ShowSuccessMessage("エクスポートが完了しました。");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("エクスポート中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// インポートボタンクリック時の処理
        /// </summary>
        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var fileName = GetImportFileName();
                if (string.IsNullOrEmpty(fileName))
                    return;

                if (!ConfirmImport())
                    return;

                ImportData(fileName);
                ShowSuccessMessage("インポートが完了しました。");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("インポート中にエラーが発生しました。", ex);
            }
        }
        #endregion

        #region データ操作
        /// <summary>
        /// エクスポート用のファイル名を取得します
        /// </summary>
        /// <returns>選択されたファイル名、キャンセル時は null</returns>
        private string GetExportFileName()
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = JSON_FILTER;
                saveDialog.FilterIndex = 1;
                saveDialog.DefaultExt = DEFAULT_EXTENSION;
                saveDialog.FileName = string.Format(EXPORT_FILENAME_FORMAT, DateTime.Now);

                return saveDialog.ShowDialog() == DialogResult.OK ? saveDialog.FileName : null;
            }
        }

        /// <summary>
        /// インポート用のファイル名を取得します
        /// </summary>
        /// <returns>選択されたファイル名、キャンセル時は null</returns>
        private string GetImportFileName()
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = JSON_FILTER;
                openDialog.FilterIndex = 1;
                openDialog.DefaultExt = DEFAULT_EXTENSION;

                return openDialog.ShowDialog() == DialogResult.OK ? openDialog.FileName : null;
            }
        }

        /// <summary>
        /// データをエクスポートします
        /// </summary>
        /// <param name="fileName">エクスポート先ファイル名</param>
        private void ExportData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("ファイル名が無効です。", nameof(fileName));

            _parentForm.ExportData(fileName);
        }

        /// <summary>
        /// データをインポートします
        /// </summary>
        /// <param name="fileName">インポート元ファイル名</param>
        private void ImportData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("ファイル名が無効です。", nameof(fileName));

            _parentForm.ImportData(fileName);
        }
        #endregion

        #region ヘルパーメソッド
        /// <summary>
        /// インポート実行の確認を行います
        /// </summary>
        /// <returns>実行を許可する場合は true</returns>
        private bool ConfirmImport()
        {
            var message = "インポートすると既存のデータが上書きされる可能性があります。\n続行しますか？";
            var result = MessageBox.Show(this, message, CONFIRM_TITLE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return result == DialogResult.Yes;
        }

        /// <summary>
        /// 成功メッセージを表示します
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(this, message, DIALOG_TITLE,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
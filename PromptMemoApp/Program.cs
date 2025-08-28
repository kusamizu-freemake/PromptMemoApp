using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    /// <summary>
    /// アプリケーションのエントリーポイントクラス
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイント
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Windows Formsアプリケーションの初期化
            InitializeApplication();

            // メインフォームを起動
            StartMainForm();
        }

        /// <summary>
        /// Windows Formsアプリケーションの基本設定を初期化
        /// </summary>
        private static void InitializeApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        /// <summary>
        /// メインフォームを作成して実行
        /// </summary>
        private static void StartMainForm()
        {
            Application.Run(new PromptEditorForm());
        }
    }
}
using PromptEditorApp;
using PromptManager;
using System;
using System.Windows.Forms;

namespace PromptMemoApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PromptEditorForm());
        }
    }
}

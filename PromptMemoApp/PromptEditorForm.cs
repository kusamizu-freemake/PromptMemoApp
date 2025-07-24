using PromptEditorApp;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptManager
{
    public partial class PromptEditorForm : Form
    {
        private string dataDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentFilePath = null;

        public PromptEditorForm()
        {
            InitializeComponent();
            LoadPromptFiles();
        }

        private void LoadPromptFiles()
        {
            lstPromptFiles.Items.Clear();

            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            var files = Directory.GetFiles(dataDirectory, "*.txt");
            foreach (var file in files)
                lstPromptFiles.Items.Add(Path.GetFileNameWithoutExtension(file));
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtPrompt.Text = "";
            currentFilePath = null;
            lstPromptFiles.ClearSelected();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string content = txtPrompt.Text.Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("内容が空です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(currentFilePath))
            {
                using (var dialog = new InputDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = dialog.InputText;
                        currentFilePath = Path.Combine(dataDirectory, fileName + ".txt");
                        File.WriteAllText(currentFilePath, content);
                        LoadPromptFiles();
                        SelectFileByName(fileName);
                    }
                }
            }
            else
            {
                File.WriteAllText(currentFilePath, content);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItem == null) return;

            string fileName = lstPromptFiles.SelectedItem.ToString();
            string filePath = Path.Combine(dataDirectory, fileName + ".txt");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                LoadPromptFiles();
                txtPrompt.Clear();
                currentFilePath = null;
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItem == null) return;

            string oldName = lstPromptFiles.SelectedItem.ToString();
            string oldPath = Path.Combine(dataDirectory, oldName + ".txt");

            using (var dialog = new InputDialog())
            {
                dialog.InputTextValue = oldName;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText;
                    string newPath = Path.Combine(dataDirectory, newName + ".txt");

                    if (File.Exists(newPath))
                    {
                        MessageBox.Show("同名ファイルがすでに存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.Move(oldPath, newPath);
                    currentFilePath = newPath;
                    LoadPromptFiles();
                    SelectFileByName(newName);
                }
            }
        }

        private void lstPromptFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItem == null) return;

            string fileName = lstPromptFiles.SelectedItem.ToString();
            string filePath = Path.Combine(dataDirectory, fileName + ".txt");

            if (File.Exists(filePath))
            {
                txtPrompt.Text = File.ReadAllText(filePath);
                currentFilePath = filePath;
            }
        }

        private void SelectFileByName(string fileName)
        {
            for (int i = 0; i < lstPromptFiles.Items.Count; i++)
            {
                if (lstPromptFiles.Items[i].ToString().Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    lstPromptFiles.SelectedIndex = i;
                    break;
                }
            }
        }
    }
}

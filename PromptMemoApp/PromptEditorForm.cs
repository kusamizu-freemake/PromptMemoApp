using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private readonly string dataDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentFilePath = null;

        public PromptEditorForm()
        {
            InitializeComponent();
            LoadCategoriesAndFiles();
        }

        private void LoadCategoriesAndFiles()
        {
            treeViewPrompts.Nodes.Clear();

            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            foreach (var dir in Directory.GetDirectories(dataDirectory))
            {
                var categoryName = Path.GetFileName(dir);
                var categoryNode = new TreeNode(categoryName) { Tag = dir };

                foreach (var file in Directory.GetFiles(dir, "*.txt"))
                {
                    var fileNode = new TreeNode(Path.GetFileNameWithoutExtension(file)) { Tag = file };
                    categoryNode.Nodes.Add(fileNode);
                }

                treeViewPrompts.Nodes.Add(categoryNode);
            }

            treeViewPrompts.ExpandAll();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (treeViewPrompts.SelectedNode == null || treeViewPrompts.SelectedNode.Parent != null)
            {
                MessageBox.Show("カテゴリを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var categoryNode = treeViewPrompts.SelectedNode;
            string categoryDir = categoryNode.Tag.ToString();

            using (var dialog = new InputDialog("新規ファイル", "ファイル名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dialog.InputText.Trim();
                    if (string.IsNullOrEmpty(fileName)) return;

                    string filePath = Path.Combine(categoryDir, fileName + ".txt");

                    if (File.Exists(filePath))
                    {
                        MessageBox.Show("同名ファイルが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.WriteAllText(filePath, "");
                    currentFilePath = filePath;
                    txtPrompt.Text = "";

                    LoadCategoriesAndFiles();
                    SelectNodeByPath(filePath);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("保存先が指定されていません。新規ファイルを作成してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            File.WriteAllText(currentFilePath, txtPrompt.Text);
            MessageBox.Show("保存しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void treeViewPrompts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return; // カテゴリノードは無視

            string filePath = e.Node.Tag.ToString();
            if (File.Exists(filePath))
            {
                currentFilePath = filePath;
                txtPrompt.Text = File.ReadAllText(filePath);
            }
        }

        private void btnCreateCategory_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputDialog("カテゴリ作成", "カテゴリ名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string categoryName = dialog.InputText.Trim();
                    if (string.IsNullOrEmpty(categoryName)) return;

                    string newCategoryPath = Path.Combine(dataDirectory, categoryName);
                    if (Directory.Exists(newCategoryPath))
                    {
                        MessageBox.Show("同名のカテゴリが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Directory.CreateDirectory(newCategoryPath);
                    LoadCategoriesAndFiles();
                }
            }
        }

        private void treeViewPrompts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeViewPrompts.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right && e.Node.Parent == null)
            {
                contextMenuCategory.Show(treeViewPrompts, e.Location);
            }
        }

        private void contextMenuRenameCategory_Click(object sender, EventArgs e)
        {
            if (treeViewPrompts.SelectedNode == null || treeViewPrompts.SelectedNode.Parent != null) return;

            string oldPath = treeViewPrompts.SelectedNode.Tag.ToString();
            string oldName = Path.GetFileName(oldPath);

            using (var dialog = new InputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText.Trim();
                    if (string.IsNullOrEmpty(newName)) return;

                    string newPath = Path.Combine(dataDirectory, newName);
                    if (Directory.Exists(newPath))
                    {
                        MessageBox.Show("同名カテゴリがすでに存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Directory.Move(oldPath, newPath);
                    LoadCategoriesAndFiles();
                }
            }
        }

        private void contextMenuDeleteCategory_Click(object sender, EventArgs e)
        {
            if (treeViewPrompts.SelectedNode == null || treeViewPrompts.SelectedNode.Parent != null) return;

            string path = treeViewPrompts.SelectedNode.Tag.ToString();
            var confirm = MessageBox.Show("カテゴリとその中の全ファイルを削除します。よろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                Directory.Delete(path, true);
                LoadCategoriesAndFiles();
                txtPrompt.Clear();
                currentFilePath = null;
            }
        }

        private void SelectNodeByPath(string filePath)
        {
            foreach (TreeNode categoryNode in treeViewPrompts.Nodes)
            {
                foreach (TreeNode fileNode in categoryNode.Nodes)
                {
                    if (fileNode.Tag.ToString().Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        treeViewPrompts.SelectedNode = fileNode;
                        return;
                    }
                }
            }
        }
    }
}

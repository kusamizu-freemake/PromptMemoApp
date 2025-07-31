using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private string dataDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentCategoryPath = null;
        private string currentFilePath = null;

        public PromptEditorForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        // --- カテゴリ読み込み ---
        private void LoadCategories()
        {
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            treeViewPrompts.Nodes.Clear();
            cmbCategories.Items.Clear();

            var dirs = Directory.GetDirectories(dataDirectory);

            foreach (var dir in dirs)
            {
                string categoryName = Path.GetFileName(dir);
                TreeNode node = new TreeNode(categoryName);
                treeViewPrompts.Nodes.Add(node);

                cmbCategories.Items.Add(categoryName);
            }

            if (cmbCategories.Items.Count > 0)
                cmbCategories.SelectedIndex = 0;
        }

        // --- ファイル一覧読み込み ---
        private void LoadFilesInCategory(string categoryPath)
        {
            lstPromptFiles.Items.Clear();

            if (!Directory.Exists(categoryPath)) return;

            var files = Directory.GetFiles(categoryPath, "*.txt");
            foreach (var file in files)
            {
                lstPromptFiles.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        // --- TreeViewカテゴリ選択 ---
        private void treeViewPrompts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentCategoryPath = Path.Combine(dataDirectory, e.Node.Text);
            LoadFilesInCategory(currentCategoryPath);
        }

        // --- 新規カテゴリ作成 ---
        private void btnCreateCategory_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputDialog("カテゴリ作成", "カテゴリ名を入力してください：", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newCategoryPath = Path.Combine(dataDirectory, dialog.InputText);
                    if (!Directory.Exists(newCategoryPath))
                    {
                        Directory.CreateDirectory(newCategoryPath);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("同名のカテゴリが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- 新規ファイル ---
        private void btnNew_Click(object sender, EventArgs e)
        {
            txtPrompt.Clear();
            currentFilePath = null;
            lstPromptFiles.ClearSelected();
        }

        // --- 保存 ---
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentCategoryPath))
            {
                MessageBox.Show("カテゴリを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string content = txtPrompt.Text.Trim();
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("内容が空です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(currentFilePath))
            {
                using (var dialog = new InputDialog("新規ファイル保存", "ファイル名を入力してください：", ""))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFilePath = Path.Combine(currentCategoryPath, dialog.InputText + ".txt");
                        File.WriteAllText(currentFilePath, content);
                        LoadFilesInCategory(currentCategoryPath);
                        SelectFileByName(dialog.InputText);
                    }
                }
            }
            else
            {
                File.WriteAllText(currentFilePath, content);
            }
        }

        // --- ファイル選択 ---
        private void lstPromptFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItem == null || string.IsNullOrEmpty(currentCategoryPath)) return;

            string fileName = lstPromptFiles.SelectedItem.ToString();
            currentFilePath = Path.Combine(currentCategoryPath, fileName + ".txt");

            if (File.Exists(currentFilePath))
                txtPrompt.Text = File.ReadAllText(currentFilePath);
        }

        // --- ファイルを名前で選択 ---
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

        // --- 一括削除 ---
        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("削除するファイルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"{lstPromptFiles.SelectedItems.Count} 件のファイルを削除しますか？",
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                foreach (var item in lstPromptFiles.SelectedItems)
                {
                    string filePath = Path.Combine(currentCategoryPath, item + ".txt");
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
                LoadFilesInCategory(currentCategoryPath);
                txtPrompt.Clear();
                currentFilePath = null;
            }
        }

        // --- 一括移動 ---
        private void btnMoveSelected_Click(object sender, EventArgs e)
        {
            if (lstPromptFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("移動するファイルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategories.SelectedItem == null)
            {
                MessageBox.Show("移動先のカテゴリを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetCategoryPath = Path.Combine(dataDirectory, cmbCategories.SelectedItem.ToString());

            if (targetCategoryPath.Equals(currentCategoryPath, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("同じカテゴリへの移動はできません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var item in lstPromptFiles.SelectedItems)
            {
                string sourcePath = Path.Combine(currentCategoryPath, item + ".txt");
                string targetPath = Path.Combine(targetCategoryPath, item + ".txt");

                if (File.Exists(sourcePath))
                {
                    if (File.Exists(targetPath))
                    {
                        // 同名ファイルがある場合はスキップ
                        MessageBox.Show($"ファイル {item} は既に存在するためスキップしました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }
                    File.Move(sourcePath, targetPath);
                }
            }

            LoadFilesInCategory(currentCategoryPath);
            txtPrompt.Clear();
            currentFilePath = null;
        }

        // --- TreeViewの右クリックメニュー ---
        private void treeViewPrompts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewPrompts.SelectedNode = e.Node;
                contextMenuCategory.Show(treeViewPrompts, e.Location);
            }
        }

        // --- カテゴリの名前変更 ---
        private void contextMenuRenameCategory_Click(object sender, EventArgs e)
        {
            if (treeViewPrompts.SelectedNode == null) return;

            string oldName = treeViewPrompts.SelectedNode.Text;
            string oldPath = Path.Combine(dataDirectory, oldName);

            using (var dialog = new InputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください：", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newPath = Path.Combine(dataDirectory, dialog.InputText);
                    if (Directory.Exists(newPath))
                    {
                        MessageBox.Show("同名のカテゴリが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Directory.Move(oldPath, newPath);
                    LoadCategories();
                }
            }
        }

        // --- カテゴリの削除 ---
        private void contextMenuDeleteCategory_Click(object sender, EventArgs e)
        {
            if (treeViewPrompts.SelectedNode == null) return;

            string categoryName = treeViewPrompts.SelectedNode.Text;
            string categoryPath = Path.Combine(dataDirectory, categoryName);

            var confirm = MessageBox.Show(
                $"カテゴリ '{categoryName}' を削除しますか？\n（中のファイルも全て削除されます）",
                "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (Directory.Exists(categoryPath))
                {
                    Directory.Delete(categoryPath, true);
                    LoadCategories();
                    lstPromptFiles.Items.Clear();
                    txtPrompt.Clear();
                }
            }
        }
    }
}

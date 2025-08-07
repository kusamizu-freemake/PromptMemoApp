using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private string baseDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentCategory = "";
        private string currentFilePath = "";

        public PromptEditorForm()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            if (!Directory.Exists(baseDirectory))
                Directory.CreateDirectory(baseDirectory);

            LoadCategories();
        }

        private void LoadCategories()
        {
            treeViewCategories.Nodes.Clear();
            comboBoxCategories.Items.Clear();

            var dirs = Directory.GetDirectories(baseDirectory);
            foreach (var dir in dirs)
            {
                string category = Path.GetFileName(dir);
                TreeNode node = new TreeNode(category);
                treeViewCategories.Nodes.Add(node);
                comboBoxCategories.Items.Add(category);
            }

            if (treeViewCategories.Nodes.Count > 0)
                treeViewCategories.SelectedNode = treeViewCategories.Nodes[0];
        }

        private void LoadFiles(string category)
        {
            listBoxFiles.Items.Clear();
            string dirPath = Path.Combine(baseDirectory, category);
            if (!Directory.Exists(dirPath)) return;

            var files = Directory.GetFiles(dirPath, "*.txt");
            foreach (var file in files)
            {
                listBoxFiles.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentCategory = e.Node.Text;
            LoadFiles(currentCategory);
            txtEditor.Clear();
            currentFilePath = "";
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;

            string fileName = listBoxFiles.SelectedItem.ToString() + ".txt";
            string filePath = Path.Combine(baseDirectory, currentCategory, fileName);
            currentFilePath = filePath;

            if (File.Exists(filePath))
                txtEditor.Text = File.ReadAllText(filePath);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("ファイルが選択されていません。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            File.WriteAllText(currentFilePath, txtEditor.Text);
            MessageBox.Show("保存しました。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentCategory))
            {
                MessageBox.Show("カテゴリを選択してください。", "新規作成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new InputDialog("新規ファイル作成", "ファイル名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string name = dialog.InputText;
                    string path = Path.Combine(baseDirectory, currentCategory, name + ".txt");

                    if (File.Exists(path))
                    {
                        MessageBox.Show("同名のファイルが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.WriteAllText(path, "");
                    LoadFiles(currentCategory);
                }
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;

            string oldName = listBoxFiles.SelectedItem.ToString();
            using (var dialog = new InputDialog("ファイル名変更", "新しいファイル名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText;
                    string oldPath = Path.Combine(baseDirectory, currentCategory, oldName + ".txt");
                    string newPath = Path.Combine(baseDirectory, currentCategory, newName + ".txt");

                    if (File.Exists(newPath))
                    {
                        MessageBox.Show("同名のファイルが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    File.Move(oldPath, newPath);
                    LoadFiles(currentCategory);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var items = listBoxFiles.SelectedItems.Cast<string>().ToList();
            if (items.Count == 0) return;

            if (MessageBox.Show("選択したファイルを削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (var name in items)
                {
                    string path = Path.Combine(baseDirectory, currentCategory, name + ".txt");
                    if (File.Exists(path))
                        File.Delete(path);
                }
                LoadFiles(currentCategory);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            var items = listBoxFiles.SelectedItems.Cast<string>().ToList();
            if (items.Count == 0 || comboBoxCategories.SelectedItem == null) return;

            string destCategory = comboBoxCategories.SelectedItem.ToString();
            foreach (var name in items)
            {
                string srcPath = Path.Combine(baseDirectory, currentCategory, name + ".txt");
                string destPath = Path.Combine(baseDirectory, destCategory, name + ".txt");
                if (File.Exists(srcPath))
                    File.Move(srcPath, destPath);
            }
            LoadFiles(currentCategory);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputDialog("カテゴリ作成", "カテゴリ名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string name = dialog.InputText;
                    string path = Path.Combine(baseDirectory, name);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void renameCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string oldName = treeViewCategories.SelectedNode.Text;
            using (var dialog = new InputDialog("カテゴリ名変更", "新しいカテゴリ名を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText;
                    string oldPath = Path.Combine(baseDirectory, oldName);
                    string newPath = Path.Combine(baseDirectory, newName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.Move(oldPath, newPath);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("同名のカテゴリが存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void deleteCategoryMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string category = treeViewCategories.SelectedNode.Text;
            string path = Path.Combine(baseDirectory, category);
            if (MessageBox.Show("カテゴリを削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Directory.Delete(path, true);
                LoadCategories();
                listBoxFiles.Items.Clear();
                txtEditor.Clear();
            }
        }

        private void menuExit_Click(object sender, EventArgs e) => this.Close();
        private void menuNew_Click(object sender, EventArgs e) => btnNew_Click(sender, e);
        private void menuFavorites_Click(object sender, EventArgs e) { /* 未実装 */ }
        private void menuHistory_Click(object sender, EventArgs e) { /* 未実装 */ }
        private void menuSearch_Click(object sender, EventArgs e) { /* 未実装 */ }
    }
}
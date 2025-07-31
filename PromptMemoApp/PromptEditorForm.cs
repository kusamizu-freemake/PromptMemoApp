using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private string dataDirectory = Path.Combine(Application.StartupPath, "prompts");
        private string currentCategory = null;
        private string currentFilePath = null;

        public PromptEditorForm()
        {
            InitializeComponent();
            LoadCategories();
            LoadMoveCategoryList();
        }

        // =============================
        // カテゴリ & ファイル一覧のロード
        // =============================
        private void LoadCategories()
        {
            treeViewCategories.Nodes.Clear();

            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);

            foreach (var dir in Directory.GetDirectories(dataDirectory))
            {
                var categoryNode = new TreeNode(Path.GetFileName(dir));
                foreach (var file in Directory.GetFiles(dir, "*.txt"))
                {
                    categoryNode.Nodes.Add(Path.GetFileName(file));
                }
                treeViewCategories.Nodes.Add(categoryNode);
            }

            treeViewCategories.ExpandAll();
        }

        private void LoadFiles(string categoryName)
        {
            listBoxFiles.Items.Clear();
            string categoryPath = Path.Combine(dataDirectory, categoryName);
            if (Directory.Exists(categoryPath))
            {
                foreach (var file in Directory.GetFiles(categoryPath, "*.txt"))
                {
                    listBoxFiles.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        private void LoadMoveCategoryList()
        {
            comboBoxCategories.Items.Clear();
            foreach (var dir in Directory.GetDirectories(dataDirectory))
            {
                comboBoxCategories.Items.Add(Path.GetFileName(dir));
            }
        }

        // =============================
        // 新規作成
        // =============================
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (currentCategory == null)
            {
                MessageBox.Show("先にカテゴリを選択してください。");
                return;
            }

            using (var dialog = new InputDialog("新規ファイル作成", "ファイル名を入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dialog.InputText + ".txt";
                    string filePath = Path.Combine(dataDirectory, currentCategory, fileName);

                    if (File.Exists(filePath))
                    {
                        MessageBox.Show("同名ファイルが既に存在します。");
                        return;
                    }

                    File.WriteAllText(filePath, "");
                    LoadFiles(currentCategory);
                    LoadCategories();
                    listBoxFiles.SelectedItem = dialog.InputText;
                }
            }
        }

        // =============================
        // TreeViewカテゴリ選択
        // =============================
        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                currentCategory = e.Node.Text;
                LoadFiles(currentCategory);
            }
            else if (e.Node.Level == 1)
            {
                currentCategory = e.Node.Parent.Text;
                LoadFiles(currentCategory);
                listBoxFiles.SelectedItem = Path.GetFileNameWithoutExtension(e.Node.Text);
                LoadFileContent();
            }
        }

        // =============================
        // ファイル選択
        // =============================
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null || currentCategory == null)
                return;

            LoadFileContent();
        }

        private void LoadFileContent()
        {
            if (listBoxFiles.SelectedItem == null) return;

            string fileName = listBoxFiles.SelectedItem.ToString() + ".txt";
            string filePath = Path.Combine(dataDirectory, currentCategory, fileName);
            if (File.Exists(filePath))
            {
                txtPrompt.Text = File.ReadAllText(filePath);
                currentFilePath = filePath;
            }
        }

        // =============================
        // 保存
        // =============================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (currentFilePath == null)
            {
                MessageBox.Show("新規ファイルを作成または選択してください。");
                return;
            }

            File.WriteAllText(currentFilePath, txtPrompt.Text);
            MessageBox.Show("保存しました。");
        }

        // =============================
        // 名前変更
        // =============================
        private void btnRename_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null)
                return;

            string oldName = listBoxFiles.SelectedItem.ToString();
            using (var dialog = new InputDialog("ファイル名を変更", "新しい名前を入力してください:", oldName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string newName = dialog.InputText + ".txt";
                    string oldPath = Path.Combine(dataDirectory, currentCategory, oldName + ".txt");
                    string newPath = Path.Combine(dataDirectory, currentCategory, newName);

                    if (File.Exists(newPath))
                    {
                        MessageBox.Show("同名ファイルが既に存在します。");
                        return;
                    }

                    File.Move(oldPath, newPath);
                    LoadFiles(currentCategory);
                    LoadCategories();
                }
            }
        }

        // =============================
        // 削除
        // =============================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("選択したファイルを削除しますか？", "削除確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (var item in listBoxFiles.SelectedItems.Cast<string>().ToList())
                {
                    string filePath = Path.Combine(dataDirectory, currentCategory, item + ".txt");
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                LoadFiles(currentCategory);
                LoadCategories();
            }
        }

        // =============================
        // 移動
        // =============================
        private void btnMove_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItems.Count == 0)
                return;

            if (comboBoxCategories.SelectedItem == null)
            {
                MessageBox.Show("移動先カテゴリを選択してください。");
                return;
            }

            string targetCategory = comboBoxCategories.SelectedItem.ToString();
            foreach (var item in listBoxFiles.SelectedItems.Cast<string>().ToList())
            {
                string oldPath = Path.Combine(dataDirectory, currentCategory, item + ".txt");
                string newPath = Path.Combine(dataDirectory, targetCategory, item + ".txt");

                if (File.Exists(oldPath))
                {
                    if (File.Exists(newPath))
                    {
                        MessageBox.Show($"{item} は既に存在します。");
                        continue;
                    }
                    File.Move(oldPath, newPath);
                }
            }

            LoadFiles(currentCategory);
            LoadCategories();
        }

        // =============================
        // 検索機能
        // =============================
        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch(txtSearch.Text.Trim());
        }

        private void menuSearch_Click(object sender, EventArgs e)
        {
            using (var dialog = new InputDialog("検索", "キーワードを入力してください:", ""))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PerformSearch(dialog.InputText.Trim());
                }
            }
        }

        private void PerformSearch(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return;

            bool found = false;
            foreach (TreeNode categoryNode in treeViewCategories.Nodes)
            {
                if (categoryNode.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    treeViewCategories.SelectedNode = categoryNode;
                    treeViewCategories.Focus();
                    found = true;
                    break;
                }

                foreach (TreeNode fileNode in categoryNode.Nodes)
                {
                    if (fileNode.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        treeViewCategories.SelectedNode = fileNode;
                        treeViewCategories.Focus();
                        found = true;
                        break;
                    }
                }

                if (found) break;
            }

            if (!found)
                MessageBox.Show("見つかりませんでした。");
        }
    }
}

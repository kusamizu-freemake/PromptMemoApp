using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class PromptEditorForm : Form
    {
        private string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "prompts");

        public PromptEditorForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        /// <summary>
        /// カテゴリ一覧を読み込む
        /// </summary>
        private void LoadCategories()
        {
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            treeViewCategories.Nodes.Clear();
            foreach (var dir in Directory.GetDirectories(baseDir))
            {
                string category = Path.GetFileName(dir);
                var node = new TreeNode(category) { Tag = dir };
                treeViewCategories.Nodes.Add(node);
            }
        }

        /// <summary>
        /// カテゴリ選択時にファイル一覧を表示
        /// </summary>
        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBoxFiles.Items.Clear();
            if (e.Node?.Tag == null) return;

            string categoryDir = e.Node.Tag.ToString();
            if (Directory.Exists(categoryDir))
            {
                foreach (var file in Directory.GetFiles(categoryDir, "*.txt"))
                {
                    listBoxFiles.Items.Add(Path.GetFileName(file));
                }
            }
            comboBoxCategories.Items.Clear();
            foreach (TreeNode node in treeViewCategories.Nodes)
            {
                comboBoxCategories.Items.Add(node.Text);
            }
        }

        /// <summary>
        /// ファイル選択時に内容を読み込む
        /// </summary>
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null || listBoxFiles.SelectedItem == null) return;

            string categoryDir = treeViewCategories.SelectedNode.Tag.ToString();
            string filePath = Path.Combine(categoryDir, listBoxFiles.SelectedItem.ToString());

            if (File.Exists(filePath))
            {
                txtPrompt.Text = File.ReadAllText(filePath);
            }
        }

        /// <summary>
        /// 保存処理
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null || listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("保存するファイルを選択してください。");
                return;
            }

            string categoryDir = treeViewCategories.SelectedNode.Tag.ToString();
            string filePath = Path.Combine(categoryDir, listBoxFiles.SelectedItem.ToString());

            File.WriteAllText(filePath, txtPrompt.Text);
            MessageBox.Show("保存しました");
        }

        private void menuSave_Click(object sender, EventArgs e) => btnSave_Click(sender, e);

        /// <summary>
        /// 新規作成
        /// </summary>
        private void btnNew_Click(object sender, EventArgs e)
        {
            // NewFileFormを開いて作成
            var categories = treeViewCategories.Nodes.Cast<TreeNode>().Select(n => n.Text).ToArray();
            using (var form = new NewFileForm(categories))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string category = form.SelectedCategory;
                    string fileName = form.FileName;

                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        MessageBox.Show("ファイル名を入力してください。");
                        return;
                    }

                    string categoryDir = Path.Combine(baseDir, category);
                    if (!Directory.Exists(categoryDir)) Directory.CreateDirectory(categoryDir);

                    string newFilePath = Path.Combine(categoryDir, fileName + ".txt");
                    File.WriteAllText(newFilePath, ""); // 空で作成

                    LoadCategories();
                }
            }
        }

        private void menuNew_Click(object sender, EventArgs e) => btnNew_Click(sender, e);

        /// <summary>
        /// 名前変更
        /// </summary>
        private void btnRename_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;

            string oldFile = listBoxFiles.SelectedItem.ToString();
            string categoryDir = treeViewCategories.SelectedNode.Tag.ToString();

            string newName = Microsoft.VisualBasic.Interaction.InputBox("新しい名前を入力してください", "名前変更", oldFile.Replace(".txt", ""));
            if (string.IsNullOrWhiteSpace(newName)) return;

            string oldPath = Path.Combine(categoryDir, oldFile);
            string newPath = Path.Combine(categoryDir, newName + ".txt");
            if (File.Exists(newPath))
            {
                MessageBox.Show("同じ名前のファイルが存在します。");
                return;
            }

            File.Move(oldPath, newPath);
            treeViewCategories_AfterSelect(this, new TreeViewEventArgs(treeViewCategories.SelectedNode));
        }

        /// <summary>
        /// 削除
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItems.Count == 0) return;

            if (MessageBox.Show("選択したファイルを削除してもよろしいですか？", "確認", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            string categoryDir = treeViewCategories.SelectedNode.Tag.ToString();
            foreach (var item in listBoxFiles.SelectedItems.Cast<string>().ToList())
            {
                string filePath = Path.Combine(categoryDir, item);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            treeViewCategories_AfterSelect(this, new TreeViewEventArgs(treeViewCategories.SelectedNode));
        }

        /// <summary>
        /// カテゴリ移動
        /// </summary>
        private void btnMove_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItems.Count == 0 || comboBoxCategories.SelectedItem == null) return;

            string targetCategory = comboBoxCategories.SelectedItem.ToString();
            string targetDir = Path.Combine(baseDir, targetCategory);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            string currentCategoryDir = treeViewCategories.SelectedNode.Tag.ToString();
            foreach (var item in listBoxFiles.SelectedItems.Cast<string>().ToList())
            {
                string oldPath = Path.Combine(currentCategoryDir, item);
                string newPath = Path.Combine(targetDir, item);

                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }
            }
            treeViewCategories_AfterSelect(this, new TreeViewEventArgs(treeViewCategories.SelectedNode));
        }

        /// <summary>
        /// 終了
        /// </summary>
        private void menuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// お気に入り
        /// </summary>
        private void menuFavorites_Click(object sender, EventArgs e)
        {
            MessageBox.Show("お気に入り機能は未実装です。");
        }

        /// <summary>
        /// 履歴
        /// </summary>
        private void menuHistory_Click(object sender, EventArgs e)
        {
            MessageBox.Show("履歴機能は未実装です。");
        }

        /// <summary>
        /// 検索
        /// </summary>
        private void menuSearch_Click(object sender, EventArgs e)
        {
            MessageBox.Show("検索機能は未実装です。");
        }

        /// <summary>
        /// カテゴリ作成
        /// </summary>
        private void contextAddCategory_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("新しいカテゴリ名を入力してください", "カテゴリ作成", "");
            if (string.IsNullOrWhiteSpace(name)) return;

            string newDir = Path.Combine(baseDir, name);
            if (Directory.Exists(newDir))
            {
                MessageBox.Show("カテゴリが既に存在します。");
                return;
            }
            Directory.CreateDirectory(newDir);
            LoadCategories();
        }

        /// <summary>
        /// カテゴリ名変更
        /// </summary>
        private void contextRenameCategory_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string oldDir = treeViewCategories.SelectedNode.Tag.ToString();
            string oldName = Path.GetFileName(oldDir);

            string newName = Microsoft.VisualBasic.Interaction.InputBox("新しいカテゴリ名を入力してください", "カテゴリ名変更", oldName);
            if (string.IsNullOrWhiteSpace(newName)) return;

            string newDir = Path.Combine(baseDir, newName);
            if (Directory.Exists(newDir))
            {
                MessageBox.Show("カテゴリが既に存在します。");
                return;
            }

            Directory.Move(oldDir, newDir);
            LoadCategories();
        }

        /// <summary>
        /// カテゴリ削除
        /// </summary>
        private void contextDeleteCategory_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null) return;

            string dir = treeViewCategories.SelectedNode.Tag.ToString();
            if (MessageBox.Show("このカテゴリを削除しますか？（中のファイルも削除されます）", "確認", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Directory.Delete(dir, true);
            LoadCategories();
        }
    }
}

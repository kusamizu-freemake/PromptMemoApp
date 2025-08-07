using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PromptMemoApp
{
    public partial class ShortcutSettingsForm : Form
    {
        private Dictionary<string, Keys> currentShortcuts;
        private Dictionary<string, Keys> originalShortcuts;

        public ShortcutSettingsForm(Dictionary<string, Keys> shortcuts)
        {
            InitializeComponent();
            currentShortcuts = new Dictionary<string, Keys>(shortcuts);
            originalShortcuts = new Dictionary<string, Keys>(shortcuts);
            LoadShortcuts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // フォーム設定
            this.Text = "ショートカット設定";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // リストボックス
            this.listBoxShortcuts = new ListBox();
            this.listBoxShortcuts.Location = new System.Drawing.Point(10, 10);
            this.listBoxShortcuts.Size = new System.Drawing.Size(300, 300);
            this.listBoxShortcuts.SelectedIndexChanged += ListBoxShortcuts_SelectedIndexChanged;

            // ボタン
            this.btnChange = new Button();
            this.btnChange.Text = "変更";
            this.btnChange.Location = new System.Drawing.Point(320, 10);
            this.btnChange.Size = new System.Drawing.Size(80, 25);
            this.btnChange.Click += BtnChange_Click;

            this.btnReset = new Button();
            this.btnReset.Text = "リセット";
            this.btnReset.Location = new System.Drawing.Point(320, 45);
            this.btnReset.Size = new System.Drawing.Size(80, 25);
            this.btnReset.Click += BtnReset_Click;

            this.btnOK = new Button();
            this.btnOK.Text = "OK";
            this.btnOK.Location = new System.Drawing.Point(320, 250);
            this.btnOK.Size = new System.Drawing.Size(80, 25);
            this.btnOK.DialogResult = DialogResult.OK;

            this.btnCancel = new Button();
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.Location = new System.Drawing.Point(320, 285);
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // ラベル
            this.lblCurrent = new Label();
            this.lblCurrent.Text = "現在のショートカット:";
            this.lblCurrent.Location = new System.Drawing.Point(10, 320);
            this.lblCurrent.Size = new System.Drawing.Size(150, 20);

            this.lblShortcut = new Label();
            this.lblShortcut.Text = "なし";
            this.lblShortcut.Location = new System.Drawing.Point(170, 320);
            this.lblShortcut.Size = new System.Drawing.Size(200, 20);

            // コントロール追加
            this.Controls.AddRange(new Control[] {
                this.listBoxShortcuts,
                this.btnChange,
                this.btnReset,
                this.btnOK,
                this.btnCancel,
                this.lblCurrent,
                this.lblShortcut
            });

            this.ResumeLayout(false);
        }

        private ListBox listBoxShortcuts;
        private Button btnChange;
        private Button btnReset;
        private Button btnOK;
        private Button btnCancel;
        private Label lblCurrent;
        private Label lblShortcut;

        private void LoadShortcuts()
        {
            listBoxShortcuts.Items.Clear();
            foreach (var shortcut in currentShortcuts)
            {
                listBoxShortcuts.Items.Add($"{shortcut.Key}: {shortcut.Value}");
            }
        }

        private void ListBoxShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxShortcuts.SelectedIndex >= 0)
            {
                var item = listBoxShortcuts.SelectedItem.ToString();
                var key = item.Split(':')[0];
                if (currentShortcuts.ContainsKey(key))
                {
                    lblShortcut.Text = currentShortcuts[key].ToString();
                }
            }
        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            if (listBoxShortcuts.SelectedIndex >= 0)
            {
                var item = listBoxShortcuts.SelectedItem.ToString();
                var key = item.Split(':')[0];
                
                using (var dialog = new ShortcutInputDialog(key, currentShortcuts.ContainsKey(key) ? currentShortcuts[key] : Keys.None))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        currentShortcuts[key] = dialog.ShortcutKey;
                        LoadShortcuts();
                    }
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            currentShortcuts = new Dictionary<string, Keys>(originalShortcuts);
            LoadShortcuts();
        }

        public Dictionary<string, Keys> GetShortcuts()
        {
            return currentShortcuts;
        }
    }

    public class ShortcutInputDialog : Form
    {
        public Keys ShortcutKey { get; private set; }

        public ShortcutInputDialog(string actionName, Keys currentKey)
        {
            InitializeComponent(actionName, currentKey);
        }

        private void InitializeComponent(string actionName, Keys currentKey)
        {
            this.SuspendLayout();
            
            this.Text = $"ショートカット設定 - {actionName}";
            this.Size = new System.Drawing.Size(400, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblInstruction = new Label();
            lblInstruction.Text = $"「{actionName}」のショートカットキーを入力してください:";
            lblInstruction.Location = new System.Drawing.Point(10, 10);
            lblInstruction.Size = new System.Drawing.Size(350, 20);

            var txtShortcut = new TextBox();
            txtShortcut.Location = new System.Drawing.Point(10, 40);
            txtShortcut.Size = new System.Drawing.Size(350, 20);
            txtShortcut.Text = currentKey.ToString();
            txtShortcut.KeyDown += TxtShortcut_KeyDown;
            txtShortcut.ReadOnly = true;

            var btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new System.Drawing.Point(200, 80);
            btnOK.Size = new System.Drawing.Size(80, 25);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += (s, e) => { ShortcutKey = currentKey; };

            var btnCancel = new Button();
            btnCancel.Text = "キャンセル";
            btnCancel.Location = new System.Drawing.Point(290, 80);
            btnCancel.Size = new System.Drawing.Size(80, 25);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] {
                lblInstruction,
                txtShortcut,
                btnOK,
                btnCancel
            });

            this.ResumeLayout(false);
        }

        private void TxtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            ShortcutKey = e.KeyData;
            ((TextBox)sender).Text = e.KeyData.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace TodoApp
{
    public partial class Main : Form
    {
        // This movement without title bar code is from:
        // http://www.codeproject.com/KB/cs/csharpmovewindow.aspx
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        NotesDisplayer _displayer;
        List<Note> _todoItems = new List<Note>();
        bool _dirty = true;

        public List<Note> Todo
        {
            get { return _todoItems; }
        }

        public bool Dirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        public Main()
        {
            InitializeComponent();
            _displayer = new NotesDisplayer(this);
            _displayer.Show();
            _displayer.Visible = false;
        }

        private void frmTodoApp_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void txtItemToAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (txtItemToAdd.Text == "**about")
                {
                    AboutBox a = new AboutBox();
                    a.Show();
                }
                else if (txtItemToAdd.Text == "**exit")
                {
                    this.Close();
                }
                else
                {
                    _todoItems.Add(new Note(txtItemToAdd.Text));
                    _dirty = true;
                    _displayer.RebindData();
                }
                txtItemToAdd.Text = "";
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                txtItemToAdd.Text = "";
                this.Visible = false;
            }
        }

        private void nicMinimized_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Activate();
            this.Focus();
        }

        private void frmTodoApp_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsShell.UnregisterHotKey(this);
            WindowsShell.UnregisterHotKey(_displayer);
        }

        private void frmTodoApp_Load(object sender, EventArgs e)
        {
            Keys mainKey = Keys.Space | Keys.Control;
            Keys displayerKey = Keys.Shift | Keys.Control | Keys.Space;
            WindowsShell.RegisterHotKey(this, mainKey);
            WindowsShell.RegisterHotKey(_displayer, displayerKey);
        }
        
        // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WindowsShell.WM_HOTKEY)
                this.Visible = !this.Visible;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r = MessageBox.Show("Would you like to save your todo list before exiting?","Save before exit?", MessageBoxButtons.YesNoCancel);
            if (r == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (_dirty &&  r == DialogResult.Yes)
            {
                _displayer.writeToFile();
            }
        }
    }
}

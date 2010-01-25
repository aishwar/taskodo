using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TodoApp
{
    public partial class NotesDisplayer : BaseForm
    {
        Main _parent;

        public NotesDisplayer(Main parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WindowsShell.WM_HOTKEY)
                this.Visible = !this.Visible;
        }

        private void NotesDisplayer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.Visible = false;
            } else if (e.Control && e.KeyCode == Keys.S)
            {
                writeToFile();
            }
        }

        public void RebindData()
        {
            int idx = lbxNotes.SelectedIndex;
            lbxNotes.DataSource = _parent.Todo.ToArray();
            if (idx < lbxNotes.Items.Count)
            {
                lbxNotes.SelectedIndex = idx;
            }
        }

        private void NotesDisplayer_VisibleChanged(object sender, EventArgs e)
        {
            RebindData();
        }

        private void lbxNotes_DrawItem(object sender, DrawItemEventArgs e)
        {
            Note currNote;
            if (e.Index >= 0)
            {
                e.DrawBackground();

                Rectangle timeRectangle = new Rectangle(e.Bounds.Location, new Size(lbxNotes.Width, 16));
                timeRectangle.Offset(0, 2);
                Rectangle messageRectangle = new Rectangle(timeRectangle.Location, timeRectangle.Size);
                messageRectangle.Offset(0, 10);
                messageRectangle.Height = 34;

                currNote = (Note)lbxNotes.Items[e.Index];

                Font timeFont = new Font("Serif", (float)8.0);
                e.Graphics.DrawString(currNote.StartTime.ToLongTimeString(), timeFont, Brushes.DimGray,
                    timeRectangle, StringFormat.GenericDefault);

                Font messageFont = new Font("Serif", (float)16.0);
                e.Graphics.DrawString(currNote.Message.ToString(), messageFont, 
                    (currNote.Status == Note.NoteStatus.Completed) ? Brushes.Green : Brushes.Black,
                    messageRectangle, StringFormat.GenericDefault);

                e.DrawFocusRectangle();
            }
        }

        private void NotesDisplayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void lbxNotes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                ((Note)lbxNotes.SelectedItem).end();
                _parent.Dirty = true;
                RebindData();
            }
            else if (e.KeyCode == Keys.U)
            {
                ((Note)lbxNotes.SelectedItem).Status = Note.NoteStatus.Started;
                _parent.Dirty = true;
                RebindData();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are you sure you want to delete this item?","Confirm Delete",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _parent.Todo.Remove(((Note)lbxNotes.SelectedItem));
                    _parent.Dirty = true;
                    RebindData();
                }
            }
            else
            {
                e.Handled = false;
                base.OnKeyDown(e);
            }
        }

        public void writeToFile()
        {
            SaveFileDialog s = new SaveFileDialog();
            if (s.ShowDialog() == DialogResult.OK)
            {
                TextWriter tx = new StreamWriter(s.FileName);

                foreach (Note note in _parent.Todo)
                {
                    tx.WriteLine(note.StartTime + " *|* " + 
                        ((note.Status == Note.NoteStatus.Completed) ? note.EndTime.ToString() : "") + 
                        " *|* " + note.Message);
                }

                tx.Close();

                _parent.Dirty = false;
            }
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            writeToFile();
        }
    }
}

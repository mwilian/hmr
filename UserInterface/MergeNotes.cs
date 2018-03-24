using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using System.Collections;
using CommonLib;

namespace UserInterface
{
    public partial class MergeNotes : Form
    {
        private ArrayList noteIDs = null;
        private XmlNodeList notes = null;
        private string mergeNoteID ="03";
        private string mergeNoteName = "病程记录";
        private int orgHeight = 0;
        private int uncommitedNoteCount = 0;
        public MergeNotes(XmlNodeList emrNotes, string NoteID)
        {
            InitializeComponent();
            orgHeight = tvNotes.Height;
            notes = emrNotes;
            XmlNode xmlPattern = Globals.emrPattern.GetEmrNote(NoteID);
            string MergeID = xmlPattern.Attributes["Merge"].Value;
            AllGroups(MergeID);
            NoteListForMerge(MergeID);
            if (Globals.commitedAsEnd && uncommitedNoteCount > 0)
            {
                MessageBox.Show(uncommitedNoteCount.ToString() + "存在草稿、审核中的记录，请慎重打印！",
                ErrorMessage.Warning);
                this.DialogResult = DialogResult.Cancel;              
            }
        }
        private void NoteListForMerge(string mergeCode)
        {
            noteIDs = new ArrayList();
            tvNotes.Nodes.Clear();
          
            foreach (XmlNode emrNote in notes)
            {

                if (emrNote.Attributes[AttributeNames.Merge].Value != mergeCode) continue;                
                string text = emrNote.Attributes[EmrConstant.AttributeNames.WrittenDate].Value;
                /**20110905  zzl**/
                if (emrNote.Attributes[EmrConstant.AttributeNames.RealName] == null || emrNote.Attributes[EmrConstant.AttributeNames.RealName].Value == "")
                    text += Delimiters.Space + emrNote.Attributes[EmrConstant.AttributeNames.NoteName].Value;
                else
                    text += Delimiters.Space + emrNote.Attributes[EmrConstant.AttributeNames.RealName].Value;
                //text += Delimiters.Space + emrNote.Attributes[EmrConstant.AttributeNames.NoteName].Value;
                int index = Convert.ToInt32(emrNote.Attributes[EmrConstant.AttributeNames.NoteStatus].Value);
                EmrConstant.NoteStateText nst = new EmrConstant.NoteStateText();
                text += Delimiters.Space + nst.Text[index];
                TreeNode newNode = tvNotes.Nodes.Add(text);
                newNode.Tag = emrNote.Attributes[EmrConstant.AttributeNames.Series].Value;
                newNode.Name = emrNote.Attributes[EmrConstant.AttributeNames.NoteID].Value;

                EmrConstant.NoteStatus ns = (EmrConstant.NoteStatus)index;
                //if (ns == EmrConstant.NoteStatus.Commited || ns == EmrConstant.NoteStatus.Checked
                //    || ns == EmrConstant.NoteStatus.FinallyChecked)
                //2009/6/19
                newNode.Checked = true;
                if (ns == EmrConstant.NoteStatus.Commited || ns == EmrConstant.NoteStatus.Checked
             || ns == EmrConstant.NoteStatus.FinallyChecked || ns == EmrConstant.NoteStatus.Checking || ns == EmrConstant.NoteStatus.Draft || ns == EmrConstant.NoteStatus.FinallyCkecking)
                {
                    newNode.Checked = true;
                }
                else
                {
                    if (Globals.commitedAsEnd)
                    {
                        newNode.Remove();
                        uncommitedNoteCount++;
                    }
                    else
                    {
                        newNode.Checked = false;
                    }
                }

                if (!ExistsInNoteIDs(emrNote.Attributes[EmrConstant.AttributeNames.NoteID].Value))
                    noteIDs.Add(emrNote.Attributes[EmrConstant.AttributeNames.NoteID].Value);
            }
        }
        private bool ExistsInNoteIDs(string noteID)
        {
            foreach (string id in noteIDs)
            {
                if (id == noteID) return true;
            }
            return false;
        }
        private void AllGroups(string MergeID)
        {
            XmlNodeList groups = Globals.emrPattern.GetAllEmrGroups();
            int height = 0;
            int count = 0;
            int left = 3;
            foreach (XmlNode group in groups)
            {
                if (group.Attributes[AttributeNames.Code].Value == MergeID)
                {
                    RadioButton rb = new RadioButton();
                    rb.AutoSize = true;
                    rb.Text = group.Attributes[AttributeNames.NoteName].Value;
                    rb.Tag = group.Attributes[AttributeNames.Code].Value;
                    rb.Name = group.Attributes[AttributeNames.NoteID].Value;
                    rb.Top = count * (rb.Height);
                    rb.Left = left;
                    //if (group.Attributes[AttributeNames.Code].Value == Globals.ThisAddIn.mergeCode
                    if (group.Attributes[AttributeNames.Code].Value == MergeID)
                    {
                        rb.Checked = true;
                        mergeNoteID = group.Attributes[AttributeNames.NoteID].Value;
                        mergeNoteName = rb.Text;
                    }

                    rb.Click += new EventHandler(rb_Click);
                    rb.Checked = true;
                    pnlGroup.Controls.Add(rb);
                    height += rb.Height + 4;
                    count++;
                }
            }
            pnlGroup.Height = height + 2;
            pnlNote.Top = pnlGroup.Height + pnlGroup.Top;
            pnlNote.Height = this.Height - pnlNote.Top - 2;
        }
        private void rb_Click(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            string mergeCode = rb.Tag.ToString();
            mergeNoteID = rb.Name;
            mergeNoteName = rb.Text;
            NoteListForMerge(mergeCode);
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            bool overRule = false;
            bool hasChecked = false;
            foreach (TreeNode content in tvNotes.Nodes)
            {
                if (content.Checked == true)
                {
                    EmrConstant.NoteStatus ns = (EmrConstant.NoteStatus)Convert.ToInt32(content.Name);
                    //if (ns == EmrConstant.NoteStatus.Draft || ns == EmrConstant.NoteStatus.Checking
                    //    || ns == EmrConstant.NoteStatus.FinallyCkecking)
                    //{
                    //    MessageBox.Show(EmrConstant.ErrorMessage.NoMarge, content.Text);
                    //    //content.Checked = false;
                    //    overRule = true;
                    //}
                    hasChecked = true;
                }
            }
            if (overRule) return;

            this.DialogResult = DialogResult.OK;
            if (!hasChecked) this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnUnselect_Click(object sender, EventArgs e)
        {
            foreach (TreeNode content in tvNotes.Nodes)
            {
                content.Checked = false;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (TreeNode content in tvNotes.Nodes)
            {
                content.Checked = true;
            }
        }
        public TreeView GetNotes()
        {
            return tvNotes;
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {

            tvNotes.ContextMenuStrip = null;
        }

        private void tsmiUp_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvNotes.SelectedNode;
            string name = tn.PrevNode.Name;
            string text = tn.PrevNode.Text;
            string tag = tn.PrevNode.Tag.ToString();
            tn.PrevNode.Tag = tn.Tag;
            tn.PrevNode.Text = tn.Text;
            tn.PrevNode.Name = tn.Name;
            tn.Text = text;
            tn.Tag = tag;
            tn.Name = name;
            tvNotes.ContextMenuStrip = null;
        }

        private void tsmiDown_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvNotes.SelectedNode;
            string name = tn.NextNode.Name;
            string text = tn.NextNode.Text;
            string tag = tn.NextNode.Tag.ToString();
            tn.NextNode.Tag = tn.Tag;
            tn.NextNode.Text = tn.Text;
            tn.NextNode.Name = tn.Name;
            tn.Text = text;
            tn.Tag = tag;
            tn.Name = name;
            tvNotes.ContextMenuStrip = null;
        }

        private void tvNotes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvNotes.ContextMenuStrip = contextMenuStrip1;
            tvNotes.SelectedNode = e.Node;
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            tsmiUp.Enabled = true;
            tsmiDown.Enabled = true;
            if (tvNotes.SelectedNode == tvNotes.Nodes[0])
                tsmiUp.Enabled = false;
            if (tvNotes.SelectedNode == tvNotes.Nodes[tvNotes.Nodes.Count - 1])
                tsmiDown.Enabled = false;
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            tvNotes.ContextMenuStrip = null;
        }

       
     

      
    }
}
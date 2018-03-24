using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace UserInterface
{
    public partial class SetPhrase : Form
    {
        private Color selectedColor = Color.BlueViolet;
        private Color unselectedColor;
        private TreeNode dptRoot, psnRoot, selectedNode;
        public bool editing = false;
        public SetPhrase(XmlNode dptPhrase, XmlNode prnPhrase)
        {
            InitializeComponent();
            unselectedColor = tvPhrase.ForeColor;
            tvPhrase.Nodes.Clear();
            dptRoot = tvPhrase.Nodes.Add(EmrConstant.StringGeneral.DepartmentPhrase);
            LoadPhraseTree(dptRoot,dptPhrase);
            #region Load phrase tree.
          
            if (tvPhrase.Nodes.Count == 1)
                psnRoot = tvPhrase.Nodes.Add(EmrConstant.StringGeneral.PersonPhrase);
            else psnRoot = tvPhrase.Nodes[1];
            LoadPhraseTree(psnRoot, prnPhrase);
            #endregion

        }
        private void LoadPhraseTree(TreeNode root , XmlNode dptPhrase)
        {  
         
     
                XmlDocument doc = new XmlDocument();
                XmlNodeList level0s = dptPhrase.SelectNodes(EmrConstant.ElementNames.Level0);
                //root.Nodes.Clear();
                foreach (XmlNode level0 in level0s)
                {
                    TreeNode level0Node =
                         root.Nodes.Add(level0.Attributes[EmrConstant.AttributeNames.CellText].Value);
                    level0Node.ImageIndex = 1;
                    XmlNodeList level1s = level0.SelectNodes(EmrConstant.ElementNames.Level1);
                    foreach (XmlNode level1 in level1s)
                    {
                        TreeNode level1Node = level0Node.Nodes.Add(level1.InnerText);
                        level1Node.ImageIndex = 2;
                    }
                }
           
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (rtbPhrase.Text.Length == 0) return;

            if (tvPhrase.SelectedNode == null)
            {
                TreeNode level0 = dptRoot.Nodes.Add(rtbPhrase.Text);
                tvPhrase.SelectedNode = level0;
                selectedNode = level0;
                level0.ForeColor = selectedColor;
            }
            else
            {
                switch (tvPhrase.SelectedNode.Level)
                {
                    case 0:
                        tvPhrase.SelectedNode.ForeColor = Color.Blue;
                        AddLevelx(tvPhrase.SelectedNode);
                        break;
                    case 1:
                        tvPhrase.SelectedNode.ForeColor = unselectedColor;
                        AddLevelx(tvPhrase.SelectedNode);
                        break;
                    case 2:
                        tvPhrase.SelectedNode.ForeColor = unselectedColor;
                        AddLevelx(tvPhrase.SelectedNode.Parent);
                        break;
                }
            }
            editing = true;
        }
        private void AddLevelx(TreeNode parent)
        {
            TreeNode levelx = parent.Nodes.Add(rtbPhrase.Text);
            tvPhrase.SelectedNode = levelx;
            tvPhrase.SelectedNode.ForeColor = selectedColor;
            selectedNode = levelx;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
           
            if (selectedNode == null) return;
            switch (selectedNode.Level)
            {
                case 1:
                    if (MessageBox.Show(EmrConstant.ErrorMessage.RemovePhrasesLevel0,
                        EmrConstant.ErrorMessage.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        selectedNode.Remove();
                        selectedNode = tvPhrase.SelectedNode;
                        if (selectedNode != null)
                        {
                            selectedNode.ForeColor = selectedColor;
                        }
                        editing = true;
                    }
                    break;
                case 2:
                    if (MessageBox.Show(EmrConstant.ErrorMessage.RemovePhrasesLevel1,
                        EmrConstant.ErrorMessage.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        rtbPhrase.Text = selectedNode.Text;
                        selectedNode.Remove();
                        selectedNode = tvPhrase.SelectedNode;
                        if (selectedNode != null)
                        {
                            selectedNode.ForeColor = selectedColor;
                        }
                        editing = true;
                    }
                    break;
            }
        }

        private void Phrase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (editing)
            {
                if (MessageBox.Show(EmrConstant.ErrorMessage.NoSavePhrases, EmrConstant.ErrorMessage.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes) btnSave_Click(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public XmlElement GetPhrases(string SG)
        {
            TreeNode root;
            if (SG == EmrConstant.StringGeneral.PersonPhrase)  root = psnRoot;
            else root = dptRoot;
             #region Save to local storage
            XmlDocument doc = new XmlDocument();
            XmlElement Phrases = doc.CreateElement(EmrConstant.ElementNames.Phrases);
            foreach (TreeNode level0Node in root.Nodes)
            {
                XmlElement level0 = doc.CreateElement(EmrConstant.ElementNames.Level0);
                level0.SetAttribute(EmrConstant.AttributeNames.CellText, level0Node.Text);
                foreach (TreeNode level1Node in level0Node.Nodes)
                {
                    XmlElement level1 = doc.CreateElement(EmrConstant.ElementNames.Level1);
                    level1.InnerText = level1Node.Text;
                    level0.AppendChild(level1);
                }
                Phrases.AppendChild(level0);
            }
            return Phrases;
            //doc.LoadXml(Phrases.OuterXml);
            //doc.Save(docName);
            #endregion

           
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (editing)
            {
                if (MessageBox.Show(EmrConstant.ErrorMessage.NoSavePhrases, EmrConstant.ErrorMessage.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes) btnSave_Click(sender, e);
            }
            this.Close();
        }

        private void tvPhrase_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
          
            if (selectedNode != null)
            {
                if (selectedNode.Level == 0) selectedNode.ForeColor = Color.Blue;
                else selectedNode.ForeColor = unselectedColor;
            }
            e.Node.ForeColor = selectedColor;
            selectedNode = e.Node;
        }

      
       
    }
}

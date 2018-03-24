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
    public partial class Phrase : Form
    {
        public Phrase(XmlNode dptPhrase, XmlNode prnPhrase)
        {
            InitializeComponent();
            tvPhrase.Nodes.Clear();
            TreeNode dptRoot = tvPhrase.Nodes.Add(EmrConstant.StringGeneral.DepartmentPhrase);
            LoadPhraseTree(dptRoot, dptPhrase);

            #region Load phrase tree.
            TreeNode psnRoot;
            if (tvPhrase.Nodes.Count == 1)
                psnRoot = tvPhrase.Nodes.Add(EmrConstant.StringGeneral.PersonPhrase);
            else psnRoot = tvPhrase.Nodes[1];
            LoadPhraseTree(psnRoot, prnPhrase);
            tvPhrase.ExpandAll();
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tvPhrase.SelectedNode.Level != 2) return;
            DialogResult = DialogResult.OK;
            this.Close();
        }
        public string GetText()
        {
            return tvPhrase.SelectedNode.Text;
        }
        private void LoadPhraseTree(TreeNode root, XmlNode dptPhrase)
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
    }
}

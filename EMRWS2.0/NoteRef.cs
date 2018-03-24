using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonLib;
using System.Collections;
using Word=Microsoft.Office.Interop.Word;
using System.IO;
using EmrConstant;
using System.Reflection;
using System.Xml;
namespace EMR
{
    public partial class NoteRef : Form
    {
        private string nodeText = null;
        public Word.Application wordApp2;
        MainForm emrTaskPane = null;
        private Object oDocument;
        public NoteRef(TreeNode patient,MainForm etp)
        {
            InitializeComponent();
            nodeText = patient.Text;
            emrTaskPane = etp;
            //if (ThisAddIn.CanOption(ElementNames.NoRefOther) == true)
            //{
            //    tabRefOther.Visible = false;              
            //}
            /* For self */
            foreach (TreeNode note in patient.Nodes)
            {
                tvRegistry.Nodes.Add((TreeNode)note.Clone());
            }
            tvRegistry.ExpandAll();

            //删除当前文档
            //foreach (TreeNode node in tvRegistry.Nodes)
            //{
            //    for (int i = 0; i < node.Nodes.Count; i++)
            //    {
            //        for (int j = 0; j < node.Nodes[i].Nodes.Count; j++)
            //        {
            //            if (emrTaskPane.tvPatients.SelectedNode != null)
            //            {
            //                TreeNode selectnode = emrTaskPane.tvPatients.SelectedNode;
            //                if (selectnode.Tag == node.Nodes[i].Nodes[j].Tag)
            //                    node.Nodes[i].Nodes[j].Remove(); break;
            //            }
            //        }
            //    }
            //}
            /* For others */
            CloneTreeView(patient.TreeView);

            if (Globals.offline)
            {
                //tabRef.TabPages.RemoveAt(2);
                return;
            }        
           // rbBoth.Checked = true;
            //CJJ.LoadComboBoxWithDepartmentNames(cbxDepart);

        }

        public NoteRef()
        {
            // TODO: Complete member initialization
        }
        private void CloneTreeView(TreeView orgTvPatients)
        {
            tvPatients.BeginUpdate();
            tvPatients.Nodes.Clear();
            foreach (TreeNode patinet in orgTvPatients.Nodes)
            {
                if (patinet.Text == nodeText) continue;
                tvPatients.Nodes.Add((TreeNode)patinet.Clone());
            }
            tvPatients.EndUpdate();
        }
       
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (wordApp2 != null)
            {
                try
                {
                    wordApp2.ActiveDocument.Save();
                }
                catch(Exception ex)
                {
                    Globals.logAdapter.Record("EX925511156757", ex.Message + ">>" + ex.ToString(), true);            
            
                }
                wordApp2 = null;
                axWbDoCView2.Navigate("about:blank");              
               // axWbDoCView2.Dispose();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnExit_Click(sender, e);
            this.Dispose();
        }

        private void tvRegistry_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level != 2) return;           
            string docName = emrTaskPane.GetWordDocNames(e.Node);
            if (docName.Trim()=="") return;
            btnExit_Click(sender, e);
            string str = OpenWordDoc(docName);
            if (str == null) return;
        }

        private void tvPatients_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 3)
            {
                string docName = emrTaskPane.GetWordDocNames(e.Node);
                if (docName.Trim() == "") return;
                string str = OpenWordDoc(docName);
                if (str == null) return;
            }
            if (e.Node.Level == 1)
            {
                emrTaskPane.OpenEmrDocument(e.Node);
            }
        }
      
        private string OpenWordDoc(string wdDocName)
        {
            if (!File.Exists(wdDocName)) return null;
            object oMissing = System.Reflection.Missing.Value;
            string filename = wdDocName;
            try
            {
                this.Invoke(new EventHandler(btnExit_Click));
                axWbDoCView2.Navigate(filename, ref  oMissing, ref   oMissing, ref   oMissing, ref   oMissing);
                return "success";
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925511256758", ex.Message + ">>" + ex.ToString(), true);            
            
                ThisAddIn.killWordProcess();
                wordApp2 = null;
                axWbDoCView2.Navigate("about:blank");
                return null;
            }            
        }

        private void axWbDoCView_NavigateComplete2(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateComplete2Event e)
        {
            if (axWbDoCView2.LocationName != "about:blank")
            {
                try
                {
                    Object o = e.pDisp;
                    oDocument = o.GetType().InvokeMember("Document", BindingFlags.GetProperty, null, o, null);
                    Object oApplication = o.GetType().InvokeMember("Application", BindingFlags.GetProperty, null, oDocument, null);
                    Object oName = o.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, oApplication, null);
                    Object refmissing = System.Reflection.Missing.Value;
                    axWbDoCView2.ExecWB(SHDocVw.OLECMDID.OLECMDID_HIDETOOLBARS, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref refmissing, ref refmissing);
                    wordApp2 = (Word.Application)oApplication;
                    Word.Document doc = oDocument as Word.Document;
                    wordApp2.ActiveDocument.Background.Visible = Microsoft.Office.Core.MsoTriState.msoFalse;
                    wordApp2.ActiveWindow.DisplayRulers = false;
                    //wordApp2.ActiveWindow.View.ShowBookmarks = true; //书签显示 LiuQi 2012-05-22
                    //wordApp2.ActiveDocument.ActiveWindow.DisplayRulers = true;//标尺显示 LiuQi 2012-09-03
                    //wordApp2.ActiveDocument.ActiveWindow.DisplayVerticalRuler = true;//标尺显示 LiuQi 2012-09-03
                    wordApp2.ActiveWindow.View.ShowRevisionsAndComments = false;   //参照时不显示痕迹 LiuQi 2012-07-03 
                    wordApp2.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;  //参照时不显示痕迹 LiuQi 2012-07-03 
                    // axWbDoCView2.Capture = false;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925511256759", ex.Message + ">>" + ex.ToString(), true);

                }
            }
        }

        private void NoteRef_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnExit_Click(sender, e);
            this.Dispose();  
        }

       
        private void search_Click(object sender, EventArgs e)
        {
            try
            {
                XmlNode patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.PatientName, txtName.Text.ToString().Trim());

                if (patients != null)
                {
                    if (patients.ChildNodes.Count > 0)
                    {
                        //string filename = Path.Combine(Globals.currentDirectory, "patients.xml");
                        //if (File.Exists(filename)) File.Delete(filename);
                        //XmlWriter writer = XmlWriter.Create(filename);
                        //patients.WriteTo(writer);
                        //writer.Close();
                        ThisAddIn.LoadTreeviewWithPatients(tvSelect, patients);
                    }
                    else
                    {
                        MessageBox.Show(EmrConstant.ErrorMessage.NoFindResult, EmrConstant.ErrorMessage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925511256769", ex.Message + ">>" + ex.ToString(), true);            
                  
            }
        }

        private void tvSelect_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 2012-10-17 LiuQi 修复参照记录无法打开病历
            if (e.Node.Level == 1)
            {
                emrTaskPane.OpenEmrDocument(e.Node);
            }
            if (e.Node.Level == 3)
            {
                string docName = emrTaskPane.GetWordDocNames(e.Node);
                if (docName.Trim() == "") return;
                string str = OpenWordDoc(docName);
                if (str == null) return;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Xml;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using CommonLib;
using System.Drawing.Text;
using UserInterface;
using DevComponents.DotNetBar.Rendering;
using System.Threading;
using System.Reflection;
using Office = Microsoft.Office.Core;
using EmrConstant;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using HuronControl;
using System.Drawing.Imaging;
using System.Diagnostics;
using HuronControl.Document;
using System.Drawing.Printing;
using EMR.Prints;
namespace EMR
{
    public partial class MainForm : Form
    {
        private Object oDocument;
        public Microsoft.Office.Interop.Word.Application wordApp;
        TreeNode tnNode = new TreeNode();
        XmlNode Doctors = null;
        public EmrTemplate NoteTemplate = null;
        private EmrDocuments emrDocuments = new EmrDocuments();
        private List<string> singlePageNoteID = new List<string>();
        public EmrNote currentNote;
        private MergeNotes mns = null;
        private int TotalRecordsNumber = 0;
        private int CheckedNumber = 0;
        private CSelectP select = null;
        private CSelectEndP selectend = null;
        private ValuateScore vs = null;

        private OpenDoc od = null;
        private NoteRef nr = null;
        private PatientInfoForm patientInfoForm = null;
        private EmrConstant.OperatorRole operatorRole = EmrConstant.OperatorRole.Nothing;
        public static PermissionLevel Permission = PermissionLevel.ReadOnly;
        SolidTextChange solidTextChange;
        List<ButtonX> lbrole = new List<ButtonX>();
        [DllImport("zip.dll", EntryPoint = "UnZipTo")]
        public static extern bool UnZipTo(string strZipFileName, string strDirectoryPath);
        public bool showWritenDate = false;
        public bool showWriter = false;
        /* user controls */
        XmlElement emrNoteDup = null;
        public Logon logon = null;
        private Word.Document myDoc = null;
        private delegate void DelShowMenu(int x, int y);
        private DelShowMenu delShowMenu = null;
        private delegate void DelHideMenu();
        private DelHideMenu delHideMenu = null;

        private void init()
        {
            axWbDoCView.delLoadFileFinished = loadFileFinished;
            axWbDoCView.Parent.Resize += axWbDoCView.plMain_Resize;
        }
        public MainForm()
        {
            InitializeComponent();
            init();
            //LocalService _ls = new LocalService();
            //_ls.start();
            //ProcessMonitor _pm = new ProcessMonitor(this);
            //_pm.start();
            //_ls.setPM(_pm);
            // BindControlID();
            // ControlRight();
            AddButton();
              Globals.frmMainText = this.Text;
            delShowMenu = new DelShowMenu(this.showMenu);
            delHideMenu = new DelHideMenu(this.hideMenu);
        }
        List<TabItem> tabItems = new List<TabItem>();
        /// <summary>
        /// 绑定选项卡Tag
        /// </summary>
        private void BindControlID()
        {
            tabItems.Clear();
            MenuNote.Tag = "01"; tabItems.Add(MenuNote);
            MenuMaintenance.Tag = "02"; tabItems.Add(MenuMaintenance);
            MenuQuery.Tag = "03"; tabItems.Add(MenuQuery);
            MenuOption.Tag = "04"; tabItems.Add(MenuOption);
            MenuOutput.Tag = "05"; tabItems.Add(MenuOutput);
            MenuOffline.Tag = "06"; tabItems.Add(MenuOffline);
            MenuBlock.Tag = "07"; tabItems.Add(MenuBlock);
            MenuLogoff.Tag = "08"; tabItems.Add(MenuLogoff);
        }
        /// <summary>
        /// 依照权限控制控件显示或隐藏
        /// </summary>
        private void ControlRight()
        {
            foreach (TabItem ti in tabItems)
            {
                if (Globals.myfunctions.Contains(ti.Tag.ToString()))
                    ti.Visible = true;
                else
                    ti.Visible = false;
            }

            foreach (Control control in RolePermission.GetMemberControls())
            {
                if (Globals.myfunctions.Contains(RolePermission.GetProperty(control).ToString()))
                    control.Visible = true;
                else
                    control.Visible = false;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            tsNowTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Globals.beginTime++;
        }
        public void loadFileFinished(Word.Document doc)
        {
            wordApp = doc.Application;
            ActiveDocumentManager.setDefaultAD(doc);
            myDoc = doc;
            //solidTextChange = new SolidTextChange(wordApp);//初始化solidTextChange窗口
            //Word.Document doc = oDocument as Word.Document;
            myDoc.XMLAfterInsert += new Word.DocumentEvents2_XMLAfterInsertEventHandler(XMLAfterInsert.myDoc_XMLAfterInsert);
            WordConfigs.setCopyConfig(ActiveDocumentManager.getDefaultAD().Application);
            _EditDoc();
            Globals.logAdapter.Record("模板打开成功", "", true);
            //wordApp.ActiveWindow.DocumentMap = false;
            //string n = axWbDoCView.LocationURL;
            //n = wordApp.ActiveWindow.Document.FullName;
            //wordApp.CommandBars.ActiveMenuBar.Visible = false;
            //wordApp.TaskPanes[Word.WdTaskPanes.wdTaskPaneXMLStructure].Visible = true;
            //ProcessMonitor.startMonitor();
            if (od != null) od.Close();
            axWbDoCView.Dock = DockStyle.Fill;
            myDoc.BuildingBlockInsert += new Word.DocumentEvents2_BuildingBlockInsertEventHandler(ActiveDocument_BuildingBlockInsert);
            myDoc.ContentControlAfterAdd += new Word.DocumentEvents2_ContentControlAfterAddEventHandler(ActiveDocument_ContentControlAfterAdd);
        }

        void ActiveDocument_ContentControlAfterAdd(Word.ContentControl NewContentControl, bool InUndoRedo)
        {
            throw new NotImplementedException();
        }

        void ActiveDocument_BuildingBlockInsert(Word.Range Range, string Name, string Category, string BlockType, string Template)
        {
            throw new NotImplementedException();
        }
        //private void axWbDoCView_NavigateComplete2(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateComplete2Event e)
        //{
        //    //ProcessMonitor.startMonitor();
        //    try
        //    {
        //        if (axWbDoCView.LocationName != "about:blank")
        //        {
        //            Object o = e.pDisp;
        //            oDocument = o.GetType().InvokeMember("Document", BindingFlags.GetProperty, null, o, null);
        //            Object oApplication = o.GetType().InvokeMember("Application", BindingFlags.GetProperty, null, oDocument, null);
        //            Object oName = o.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, oApplication, null);
        //            Object refmissing = System.Reflection.Missing.Value;
        //            //隐藏工具栏
        //            axWbDoCView.ExecWB(SHDocVw.OLECMDID.OLECMDID_HIDETOOLBARS, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref refmissing, ref refmissing);
        //            wordApp = (Word.Application)oApplication;
        //            ActiveDocumentManager.setDefaultAD(myDoc);
        //            myDoc = myDoc;
        //            myDoc.XMLAfterInsert += new Word.DocumentEvents2_XMLAfterInsertEventHandler(XMLAfterInsert.myDoc_XMLAfterInsert);
        //            //solidTextChange = new SolidTextChange(wordApp);//初始化solidTextChange窗口
        //            Word.Document doc = oDocument as Word.Document;
        //            WordConfigs.setCopyConfig(ActiveDocumentManager.getDefaultAD().Application);
        //            _EditDoc();
        //            //wordApp.ActiveWindow.DocumentMap = false;
        //            string n = axWbDoCView.LocationURL;
        //            n = wordApp.ActiveWindow.Document.FullName;
        //            //wordApp.CommandBars.ActiveMenuBar.Visible = false;
        //            wordApp.TaskPanes[Word.WdTaskPanes.wdTaskPaneXMLStructure].Visible = true;
        //            //ProcessMonitor.startMonitor();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Globals.logAdapter.Record("EX925511646755", ex.Message + ">>" + ex.ToString(), true);

        //    }
        //    finally
        //    {
        //        if (od != null) od.Close();
        //        axWbDoCView.Dock = DockStyle.Fill;
        //    }
        //}

        private void MainForm_Load(object sender, EventArgs e)
        {
            //分割线显示/隐藏逻辑
            //foreach (Control control in FlowLayoutPanels.GetMemberControls())//循环设计时确定的存在分割线的容器
            //{
            //    int VisibleBtnCount = 0;
            //    bool hide = false;
            //    foreach (Control ctrl in control.Controls)//循环容器中的控件
            //    {
            //        if (ctrl.Width < 5)//被视为分割线
            //        {
            //            if (VisibleBtnCount == 0)
            //            {
            //                if (!hide)
            //                {
            //                    ctrl.Visible = false;
            //                    hide = true;
            //                }
            //            }
            //            else VisibleBtnCount = 0;
            //            //splitIndex = control.Controls.IndexOf(ctrl);
            //        }
            //        else
            //        {
            //            if (ctrl.Visible) VisibleBtnCount++;
            //        }
            //    }
            //}
            ThisAddIn.xmlPatientWriter(Globals.DoctorID, tvPatients, 3, Globals.patientFile);
         
            tsOperatorName.Text = Globals.DoctorName;
            tsOperatorCode.Text = Globals.DoctorID;
            tsNowTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ShowMenu(MenuStatus.None);
            // BlockEnable(BlockStatus.Bnone);
            SetOp();
        }
        private void wordApp_WindowBeforeRightClick(Word.Selection Sel, ref bool Cancel)
        {
            //    Cancel = true;
            //    this.BeginInvoke(delShowMenu, new object[] { MousePosition.X, MousePosition.Y });
        }
        private void hideMenu()
        {
            contextMenuStrip1.Hide();
        }
        private void showMenu(int x, int y)
        {
            contextMenuStrip1.Show(x, y);
        }



        private void wordApp_WindowSelectionChange(Word.Selection Sel)
        {
            //Cancel = true;
            if (delHideMenu != null)
            {
                this.BeginInvoke(delHideMenu);
            }

        }
        //固化文字选择界面
        private Word.InlineShape ThisShape;
        private void wordApp_WindowBeforeDoubleClick(Word.Selection selection, ref bool Cancel)
        {
            Cancel = false;
            //solidTextChange.Dispose();
            //solidTextChange = new SolidTextChange(wordApp);
            //solidTextChange.Init();
            //SetForegroundWindow(solidTextChange.Handle);
            //solidTextChange.ShowDialog();
            //wordApp.ActiveWindow.SetFocus();
            //myDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range

            try
            {
                Word.Paragraphs items = wordApp.Selection.Paragraphs;
                foreach (Word.Paragraph item in items)
                {
                    if (item != null && item.Range.InlineShapes.Count != 0)
                    {
                        foreach (Word.InlineShape shape in item.Range.InlineShapes)
                        {
                            //判断类型
                            if (shape.Type == Word.WdInlineShapeType.wdInlineShapePicture || shape.Type == Word.WdInlineShapeType.wdInlineShapeLinkedPicture)
                            {

                                //利用剪贴板保存数据                          
                                shape.Select(); //选定当前图片
                                wordApp.Selection.Copy();//copy当前图片
                                ThisShape = shape;
                                this.Invoke(new EventHandler(EditImage));
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
        private void EditImage(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                Bitmap bmp = new Bitmap(Clipboard.GetImage());
                Painter p = new Painter(bmp, true);
                if (p.ShowDialog() == DialogResult.OK)
                {
                    string picName = Globals.workFolder + "\\MyTemplateFile.bmp";
                    object linkToFile = false;
                    object saveWithDocument = true;
                    p.FinalBitmap.Save(picName);
                    object start = ThisShape.Range.Start;
                    object end = ThisShape.Range.End;
                    object oMissing = Type.Missing;
                    Word.Range range = myDoc.Range(ref start, ref end);
                    object orange = (object)range;
                    range.Select();
                    //range.Paste();
                    //  Word.Range range2 = myDoc.Range(ref end, ref end);
                    //object orange2 = (object)range2;
                    //range2.Cut();
                    range.Delete();
                    range.InlineShapes.AddPicture(picName, ref linkToFile, ref saveWithDocument, ref orange);

                    //  range2.Delete();
                    //ThisShape.Range.Delete();
                    // p.Dispose();
                    // wordApp.Selection.Find.Replacement.Text = "22222222";

                }
            }
        }
        public string OpenWordDoc(string wdDocName)
        {

            //2012-09-03 LiuQi 启动时检查是否有word进程
            Process[] process;
            process = Process.GetProcesses();
            foreach (Process p in process)
            {
                try
                {
                    if (p.Id != 0 && p.Modules != null && p.Modules.Count > 0)
                    {
                        System.Diagnostics.ProcessModule pm = p.Modules[0];
                        if (pm.ModuleName.ToLower() == "winword.exe")
                        {
                            //MessageBox.Show("其他程序影响电子病历正常使用强制关闭");
                            p.Kill();
                            break;
                        }
                    }
                }
                catch { }
            }
            if (wordApp != null) _ExitDoc();
            patient1.Visible = false;
            //    od = new OpenDoc();
            // od.Show();
            axWbDoCView.Dock = DockStyle.None;
            axWbDoCView.Size = new Size(1, 1);
            if (!File.Exists(wdDocName)) return null;
            object oMissing = System.Reflection.Missing.Value;
            string filename = wdDocName;

            try
            {
                if (Globals.localDocumentEncode)
                {
                    string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
                    udt.jj.DecodeEmrDocument(wdDocName, tmpfile);
                    filename = tmpfile;
                }
                plMain.Visible = true;
                axWbDoCView.Navigate(filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

              

                return "success";

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9212474979", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.killWordProcess();
                if (od.Visible)
                {
                    od.Close();
                    od.Dispose();
                }
                _ExitDoc();
                //MessageBox.Show("文件已经损坏！" + ex.Message + Delimiters.Colon + ex.Source);
                return null;
            }
        }
        //LQ   复制记录
        public void ResetAuthor()
        {
            for (int i = ActiveDocumentManager.getDefaultAD().ContentControls.Count; i > 0; i--)
            {
                object index = i;
                Word.ContentControl cc = ActiveDocumentManager.getDefaultAD().ContentControls.get_Item(ref index);
                if (cc.Title == null) continue;
                if (cc.Title == StringGeneral.Label) continue;
                if (cc.Title == AttributeNames.Header) continue;

                if (cc.Title == AttributeNames.FinalCheckedDate || cc.Title == AttributeNames.CheckedDate)
                {
                    if (showWritenDate)
                    {
                        cc.LockContents = false;
                        cc.Range.Text = " ";
                        cc.LockContents = true;
                    }
                    continue;
                }
                if (cc.Title == AttributeNames.FinalChecker || cc.Title == AttributeNames.Checker)
                {
                    if (showWriter)
                    {
                        cc.LockContents = false;
                        string sign = cc.Range.Text.Split('：')[0];
                        cc.Range.Text = sign + '：';
                        cc.LockContents = true;
                    }
                    continue;
                }

                if (cc.Title == AttributeNames.Writer)
                {
                    if (showWriter)
                    {
                        cc.LockContents = false;
                        string sign = cc.Range.Text.Split('：')[0];
                        cc.Range.Text = sign + '：' + logon.GetOpName();
                        cc.LockContents = true;
                    }
                    continue;
                }
                if (cc.Title == AttributeNames.WrittenDate)
                {
                    if (showWritenDate)
                    {
                        cc.LockContents = false;
                        cc.Range.Text = " " + ThisAddIn.Today().ToString(StringGeneral.DateFormat);
                        cc.LockContents = true;
                    }
                    continue;
                }
            }

        }
        public string OpenBlock(string wdDocName)
        {
            od = new OpenDoc();
            od.Show();
            patient1.Visible = false;
            axWbDoCView.Dock = DockStyle.None;
            axWbDoCView.Size = new Size(1, 1);
            if (!File.Exists(wdDocName)) return null;
            object oMissing = System.Reflection.Missing.Value;
            string filename = wdDocName;

            try
            {
                string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.NullDoc);
                if (File.Exists(tmpfile)) File.Delete(tmpfile);
                File.Copy(wdDocName, tmpfile);
                filename = tmpfile;
                plMain.Visible = true;
                axWbDoCView.Navigate(filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                return "success";

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9122474979", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.killWordProcess();
                if (od.Visible)
                {
                    od.Close();
                    od.Dispose();
                }
                _ExitDoc();
                return null;
            }
        }
        public void OpenWordTem(string wdDocName)
        {

            patient1.Visible = false;
            axWbDoCView.Dock = DockStyle.None;
            axWbDoCView.Size = new Size(1, 1);
            bar1.Visible = true;
            MenuNote.Visible = true;
            btnReadExit.Visible = true;
            btnExexit.Visible = true;
            btnprintExit.Visible = true;
            if (!File.Exists(wdDocName))
            {
                MessageBox.Show(wdDocName + "文件不存在！");
                return;
            }
            object oMissing = System.Reflection.Missing.Value;
            string filename = wdDocName;

            try
            {
                plMain.Visible = true;
                axWbDoCView.Navigate(filename, ref  oMissing, ref   oMissing, ref   oMissing, ref   oMissing);

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9112474989", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.killWordProcess();
                _ExitDoc();

            }
            finally
            {
                ProtectDoc();
            }

        }
        private void LogonOut_Click(object sender, EventArgs e)
        {
            Logon oLogOn = new Logon();
            this.Visible = false;
            if (oLogOn.ShowDialog() == DialogResult.Cancel)
                this.Close();
            else
            {
                tcPatients.Enabled = true;
                plContain.Visible = false;
                tvPatients.Nodes.Clear();

                this.Visible = true;
            }
        }
        public string GetPatientName()
        {
            if (tvPatients.SelectedNode == null) return null;

            switch (tvPatients.SelectedNode.Level)
            {
                case 0:
                    return tvPatients.SelectedNode.Text.Split(' ')[1];
                case 1:
                    return tvPatients.SelectedNode.Parent.Text.Split(' ')[1];
                case 2:
                    return tvPatients.SelectedNode.Parent.Parent.Text.Split(' ')[1];
                case 3:
                    return tvPatients.SelectedNode.Parent.Parent.Parent.Text.Split(' ')[1];
            }
            return null;
        }
        public string GetRegistryID()
        {
            TreeNode selectedNode = tvPatients.SelectedNode;
            if (selectedNode == null) return null;
            string registryIDText = null;
            switch (selectedNode.Level)
            {
                case 1:
                    registryIDText = selectedNode.Text;
                    break;
                case 2:
                    registryIDText = selectedNode.Parent.Text;
                    break;
                case 3:
                    registryIDText = selectedNode.Parent.Parent.Text;
                    break;
            }
            string registryID, chargeDoctorID;
            if (registryIDText == null) registryID = "";
            else
                ParserRegistry(registryIDText, out registryID, out chargeDoctorID);

            return registryID;
        }
        public string GetChargingDoctorID()
        {
            TreeNode selectedNode = tvPatients.SelectedNode;
            if (selectedNode == null) return null;
            string registryIDText = null;
            switch (selectedNode.Level)
            {
                case 1:
                    registryIDText = selectedNode.Text;
                    break;
                case 2:
                    registryIDText = selectedNode.Parent.Text;
                    break;
                case 3:
                    registryIDText = selectedNode.Parent.Parent.Text;
                    break;
            }
            string registryID, chargeDoctorID;
            ParserRegistry(registryIDText, out registryID, out chargeDoctorID);
            return chargeDoctorID;
        }
        public string GetPatientDepartmentCode()
        {
            PatientInfo pi = GetPatientInfo(tvPatients.SelectedNode);
            if (pi == null) return string.Empty;
            return pi.DepartmentCode;
        }
        public string GetPatientArchiveNum()
        {
            PatientInfo pi = GetPatientInfo(tvPatients.SelectedNode);
            if (pi == null) return string.Empty;
            return pi.ArchiveNum;
        }
        public bool ParserRegistry(string nodeText, out string registryID, out string doctorID)
        {
            string[] tmp = nodeText.Split(Delimiters.Colon);
            if (tmp.Length >= 3)
            {
                registryID = tmp[1];
                doctorID = tmp[2];
                return EmrConstant.Return.Successful;
            }
            registryID = "";
            doctorID = "";
            return EmrConstant.Return.Failed;

        }
        public PatientInfo GetPatientInfo(TreeNode selectedNode)
        {
            if (!File.Exists(Globals.patientFile)) return null;

            if (selectedNode == null)
            {
                if (tvPatients.SelectedNode == null) return null;
                selectedNode = tvPatients.SelectedNode;
            }

             string registryIDText = "";
            string registryID, chargeDoctorID;
            Boolean findRegistryID = false;
            #region Get RegistryID and ChargeDoctorID of selected node.
            switch (selectedNode.Level)
            {
                case 0:
                    //if (selectedNode.Nodes.Count == 1)
                    //{
                    registryIDText = selectedNode.FirstNode.Text;
                    //}
                    //else
                    //{
                    //    MessageBox.Show("有多个登记号，必须明确选中一个！", EmrConstant.ErrorMessage.Warning);
                    //    return null;
                    //}
                    break;
                case 1:
                    registryIDText = selectedNode.Text;
                    break;
                case 2:
                    registryIDText = selectedNode.Parent.Text;
                    break;
                case 3:
                    registryIDText = selectedNode.Parent.Parent.Text;
                    break;
            }
            ParserRegistry(registryIDText, out registryID, out chargeDoctorID);
            #endregion

            /* open PatientFile, and set patient information needed */
            using (XmlReader reader = XmlReader.Create(Globals.patientFile))
            {

                PatientInfo patientInfo = new PatientInfo();
                reader.ReadToFollowing(EmrConstant.ElementNames.Patient);
                do
                {
                    patientInfo.ArchiveNum = reader.GetAttribute(EmrConstant.AttributeNames.ArchiveNum);
                    patientInfo.PatientName = reader.GetAttribute(EmrConstant.AttributeNames.PatientName);
                    patientInfo.Sex = reader.GetAttribute(EmrConstant.AttributeNames.Sex);
                    patientInfo.Birth = reader.GetAttribute(EmrConstant.AttributeNames.Birth);
                    patientInfo.Age = reader.GetAttribute(AttributeNames.Age);
                    patientInfo.AgeUnit = reader.GetAttribute(AttributeNames.AgeUnit);
                    patientInfo.Nation = reader.GetAttribute(EmrConstant.AttributeNames.Nation);
                    patientInfo.MaritalStatus = reader.GetAttribute(EmrConstant.AttributeNames.MaritalStatus);
                    patientInfo.NativePlace = reader.GetAttribute(EmrConstant.AttributeNames.NativePlace);
                    patientInfo.Job = reader.GetAttribute(EmrConstant.AttributeNames.Job);
                    patientInfo.Address = reader.GetAttribute(EmrConstant.AttributeNames.Address);
                    patientInfo.RegistryID = registryID;
                    patientInfo.Birthday = reader.GetAttribute(AttributeNames.Birthday);
                    reader.ReadToFollowing(EmrConstant.ElementNames.Registry);
                    #region Find required registry from all registryIDs
                    do
                    {
                        if (registryID == reader.GetAttribute(EmrConstant.AttributeNames.RegistryID))
                        {
                            patientInfo.RegistryDate = reader.GetAttribute(AttributeNames.RegistryDate);
                            patientInfo.RegistryTime = reader.GetAttribute(AttributeNames.RegistryTime);
                            patientInfo.DepartmentCode = reader.GetAttribute(AttributeNames.DepartmentCode);
                            patientInfo.BedNum = reader.GetAttribute(AttributeNames.BedNum);
                            patientInfo.CardNum = reader.GetAttribute(AttributeNames.CardNum);
                            patientInfo.CardType = reader.GetAttribute(AttributeNames.CardType);
                            patientInfo.CanBuyDrug = reader.GetAttribute(AttributeNames.CanBuyDrug);
                            patientInfo.LowInsurance = reader.GetAttribute(AttributeNames.LowInsurance);
                            patientInfo.Phone = reader.GetAttribute(EmrConstant.AttributeNames.Phone);

                            if (reader.GetAttribute(EmrConstant.AttributeNames.PatientStatus) != null)
                            {
                                if (reader.GetAttribute(EmrConstant.AttributeNames.PatientStatus).Length > 0)
                                {
                                    patientInfo.DischargedDate =
                                        reader.GetAttribute(EmrConstant.AttributeNames.DischargedDate);
                                }
                                else
                                {
                                    patientInfo.DischargedDate = null;
                                }
                            }
                            findRegistryID = true;
                            break;
                        }
                    } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Registry));
                    #endregion
                    if (findRegistryID == true) return patientInfo;
                } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Patient));
                reader.Close();
            }
            return null;
        }
        private void LoadPatientList(XmlNode Patients)
        {
            if (Patients != null)
            {
                XmlWriter writer = XmlWriter.Create(Globals.patientFile);
                Patients.WriteTo(writer);
                writer.Close();
            }
            tvPatients.Nodes.Clear();
            ThisAddIn.LoadTreeviewWithPatients(tvPatients, Patients);
        }
        private void tsmArchive_Click(object sender, EventArgs e)
        {
            //ArchiveEmr AE = new ArchiveEmr();

            //AE.ShowDialog();
        }
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void epSplitter_Click(object sender, EventArgs e)
        {

        }
        private void epSplitter_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            if (wordApp != null)
                epSplitter.Expandable = false;
            else tcPatients.Visible = epSplitter.Expanded;
        }
        //public XmlNode GetEmrNote(int series)
        //{
        //    foreach (XmlNode emrNote in emrDoc.DocumentElement)
        //    {
        //        int noteSeries = Convert.ToInt32(emrNote.Attributes[AttributeNames.Series].Value);
        //        if (noteSeries == series) return emrNote;
        //    }
        //    return null;
        //}
        public bool Isconsult(ref XmlNode consults, string registryID)
        {
            //string noteID = NoteID.GetNoteID();
            string startTime = Globals.emrPattern.StartTimeAttribute(Globals.NoteID);

            switch (startTime)
            {
                case "Consult":
                    //XmlNode consults = null;
                    ThisAddIn.ConsultTime(registryID, Globals.PatientConsultSequence, ref consults);
                    if (consults == null) return false;
                    else return true;
            }
            return false;
        }
        private void SetRange(DataSet dst, string registryID)
        {

            foreach (Microsoft.Office.Interop.Word.XMLNode xn in myDoc.XMLNodes)
            {

                ChangeRanges(xn, dst, registryID);

            }
            foreach (Microsoft.Office.Interop.Word.XMLNode xn in myDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.XMLNodes)
            {
                ChangeRanges(xn, dst, registryID);
            }
            foreach (Word.ContentControl cc in myDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.ContentControls)
            {
                cc.LockContents = false;
                cc.LockContentControl = false;
                ChangeRange(cc, dst, registryID);

            }
            DataSet dst1 = new DataSet();

            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
              
                dst1 = ep.GetCPatientInf(registryID);
            }
            //婴儿信息更新
              foreach (Word.ContentControl cc in wordApp.ActiveDocument.ContentControls)
                {
               
                    cc.LockContents = false;
                    cc.LockContentControl = false;
                    if (cc.Title == "PCname")
                    {
                        if (dst1.Tables[0].Rows.Count != 0)
                            cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst1.Tables[0].Rows[0]["xm"].ToString();
                        else cc.Range.Text = cc.Range.Text.Split('：')[0] + "：" + dst.Tables[0].Rows[0]["xm"].ToString() + "之子";
                        continue;
                    }
                    if (cc.Title == "PCsex")
                    {
                        if (dst1.Tables[0].Rows.Count != 0)
                        {
                            string str;
                            if (dst1.Tables[0].Rows[0]["xb"].ToString() == "1") str = "男";
                            else str = "女";

                            cc.Range.Text = "性别：" + str;
                        }
                        continue;
                    }
                    if (cc.Title == "CBirthday")
                    {
                        if (dst1.Tables[0].Rows.Count != 0)
                        {
                            string str = Convert.ToDateTime(dst1.Tables[0].Rows[0]["csrq"].ToString()).ToString("f");
                            //str=str.Split(':')[0];
                            str = str.Replace(":", "时");
                            str = str + "分";
                            cc.Range.Text = "出生时间：" + str;
                        }
                        continue;
                    }
                    if (cc.Title == "PCage")
                    {
                        if (dst1.Tables[0].Rows.Count != 0)
                        {
                            DateTime dt1 = Convert.ToDateTime(dst1.Tables[0].Rows[0]["csrq"].ToString());
                            TimeSpan ts = DateTime.Now.Subtract(dt1);
                            cc.Range.Text = "年龄：" + ts.Days.ToString() + "天"
                           + ts.Hours.ToString() + "小时";
                        }
                        continue;
                    }
                    ChangeRange(cc, dst, registryID);
                    //cc.LockContents = true;
                    cc.LockContentControl = true;
                }
                              

        }
        public void ChangeRanges(Microsoft.Office.Interop.Word.XMLNode cc, DataSet dst, string registryID)
        {
            bool NewDate = false;
            XmlNode consults = null;
            bool consult = Isconsult(ref consults, registryID);
            string str;
            string title = cc.BaseName;
            string xb;
            object ob = Word.WdEditorType.wdEditorEveryone;
            if (dst.Tables[0].Rows[0]["xb"].ToString() == "1") xb = "男";
            else xb = "女";
            switch (title)
            {
                case "":
                    break;

                case "门诊号":
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {

                        cc.Range.Text = ep.GetPatientClinicID(registryID);

                    }
                    break;
                case "医师":

                    break;
                case "男":
                    if (xb == "女")
                    {
                        cc.Text = "";
                        cc.Delete();
                    }
                    break;
                case "女":
                    if (xb == "男")
                    {
                        cc.Text = "";
                        cc.Delete();
                    }
                    break;

                case "审核者":

                    break;
                case "终审者":

                    break;
                case "病房房号":
                    cc.Text = dst.Tables[0].Rows[0]["BFFH"].ToString();

                    break;
                case "病区名称":
                    cc.Text = dst.Tables[0].Rows[0]["BQMC"].ToString();
                    break;
                case "病区编码":
                    cc.Text = dst.Tables[0].Rows[0]["BQBM"].ToString();
                    break;

                case "科室":
                    string strs;
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        strs = ep.GetDepartmentName(dst.Tables[0].Rows[0]["ksbm"].ToString());
                    }
                    cc.Text = strs;

                    break;
                case "患者姓名":
                    cc.Text = dst.Tables[0].Rows[0]["xm"].ToString();

                    break;
                case "病案号":
                    cc.Text = dst.Tables[0].Rows[0]["bah"].ToString();

                    break;
                case "性别":

                    if (dst.Tables[0].Rows[0]["xb"].ToString() == "1") str = "男";
                    else str = "女";

                    cc.Text = str;

                    break;
                case "生日":
                    cc.Text = dst.Tables[0].Rows[0]["csny"].ToString();
                    break;
                case "民族":
                    cc.Text = dst.Tables[0].Rows[0]["mz"].ToString();
                    break;
                case "婚否":
                    cc.Text = dst.Tables[0].Rows[0]["hf"].ToString();
                    break;
                case "籍贯":
                    cc.Text = dst.Tables[0].Rows[0]["jg"].ToString();
                    break;
                case "职业":
                    // cc.Text = dst.Tables[0].Rows[0]["zy"].ToString();
                    try
                    {
                        int zy = Convert.ToInt32(dst.Tables[0].Rows[0]["zy"].ToString());

                        string bh = dst.Tables[0].Rows[0]["zy"].ToString();

                        gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients();
                        DataSet dt = ep.GetPatientInfZY(bh);
                        cc.Text = dt.Tables[0].Rows[0]["NAME"].ToString();
                    }
                    catch
                    {
                        cc.Text = dst.Tables[0].Rows[0]["zy"].ToString();
                    }

                    break;
                case "住址":

                    cc.Text = dst.Tables[0].Rows[0]["jtzz"].ToString();
                    break;
                case "住院号":
                    cc.Text = dst.Tables[0].Rows[0]["zyh"].ToString();

                    break;
                case "床号":

                    cc.Text = dst.Tables[0].Rows[0]["ch"].ToString();

                    break;
                case "住院日期":
                    try
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("yyyy-MM-dd");
                            //if (NewDate == false)
                            //{
                            //    str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                            //    str = str.Split(' ')[0];
                            //}
                            cc.Text = str;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("yyyy年MM月dd日");
                            cc.Text = str;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9134474979", ex.Message + ">>" + ex.ToString(), true);
                    }
                    break;
                case "出院日期":
                    try
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("yyyy-MM-dd");
                            //if (NewDate == false)
                            //{
                            //    str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            //    str = str.Split(' ')[0];
                            //}
                            cc.Text = str;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("yyyy年MM月dd日");
                            cc.Text = str;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9134474979", ex.Message + ">>" + ex.ToString(), true);

                    }
                    break;
                case "出院时间":
                    try
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("yyyy-MM-dd HH:mm");
                            cc.Text = str;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("yyyy年MM月dd日 HH时mm分");
                            //if (NewDate == false)
                            //{
                            //    str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            //    str = str.Replace(":", "时");
                            //    str = str + "分";
                            //}
                            cc.Text = str;
                        }
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9134474999", ex.Message + ">>" + ex.ToString(), true);
                    }
                    break;
                case "出院时分":
                    try
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("HH:mm");
                            if (NewDate == false)
                            {

                            }
                            cc.Text = str;
                        }
                        else
                        {
                            str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("HH时mm分");
                            //if (NewDate == false)
                            //{
                            //    str = Convert.ToDateTime(dst.Tables[0].Rows[0]["cyrq"].ToString()).ToString("f");
                            //    str = str.Split(' ')[1];
                            //    str = str.Replace(":", "时");
                            //    str = str + "分";
                            //}
                            cc.Text = str;

                        }

                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9134474991", ex.Message + ">>" + ex.ToString(), true);
                    }
                    break;
                case "年龄":
                    string nldw = dst.Tables[0].Rows[0]["nldw"].ToString();
                    cc.Text = dst.Tables[0].Rows[0]["nl"].ToString().Split('.')[0] + nldw;
                    /*年龄自动带值，删除年龄*/
                    object start = cc.Range.End + 1;
                    object end = cc.Range.End + 3;
                    Word.Range ran = myDoc.Range(ref start, ref end);
                    if (ran.Text.Contains("岁"))
                    {
                        ran.Select();
                        ran.Text = "  ";
                    }
                    break;
                case "电话":
                    cc.Text = dst.Tables[0].Rows[0]["dh"].ToString();
                    break;
                case "出生日期":    //LiuQi-2012-09-24
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["csny"].ToString()).ToString("yyyy-MM-dd HH:mm");
                        cc.Text = str;
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["csny"].ToString()).ToString("yyyy年MM月dd日 HH时mm分");
                        cc.Text = str;
                    }
                    break;
                case "入院时间":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("yyyy-MM-dd HH:mm");
                        cc.Text = str;
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("yyyy年MM月dd日 HH时mm分");
                        //if (NewDate == false)
                        //{
                        //    str = str.Replace(":", "时");
                        //    str = str + "分";
                        //}
                        cc.Text = str;
                    }
                    break;
                case "入院时":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("HH");
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("HH时mm分");
                        //str = str.Split(':')[0];
                        //str = str + "时";
                        cc.Text = str;
                    }
                    break;
                case "入院时分":
                    if (ThisAddIn.CanOption(ElementNames.ShowTime))
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("HH:mm");
                    }
                    else
                    {
                        str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("HH时mm分");
                        ////if (NewDate == false)
                        ////{
                        //// str = Convert.ToDateTime(dst.Tables[0].Rows[0]["zyrq"].ToString()).ToString("f");
                        //// str = str.Replace(":", "时");
                        //// str = str + "分";
                        //// str = str.Split(' ')[1];
                        ////}
                    }
                    cc.Text = str;
                    break;
                case "现在日期":
                    if (!Globals.RefreshInf == true)
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = ThisAddIn.Today().Date.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            str = ThisAddIn.Today().ToString("yyyy年MM月dd日 HH时mm分");
                            //if (NewDate == false)
                            //{
                            //    str = ThisAddIn.Today().ToString("f");
                            //    str = str.Replace(":", "时");
                            //    str = str + "分";
                            //}
                        }
                        cc.Text = str;
                        if (cc.ParentNode.BaseName != "审")
                            cc.Range.Editors.Add(ref ob);
                    }
                    break;
                case "现在时间":
                    if (!Globals.RefreshInf == true)
                    {
                        Globals.logAdapter.Record("现在时间", "1" + ThisAddIn.Today().ToString("yyyy-MM-dd HH:mm"), true);
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = ThisAddIn.Today().ToString("yyyy-MM-dd HH:mm");
                            Globals.logAdapter.Record("现在时间", "2" + ThisAddIn.Today().ToString("yyyy-MM-dd HH:mm"), true);
                        }
                        else
                        {
                            str = ThisAddIn.Today().ToString("yyyy年MM月dd日 HH时mm分");
                            Globals.logAdapter.Record("现在时间", "3" + ThisAddIn.Today().ToString("yyyy年MM月dd日 HH时mm分"), true);
                            //if (NewDate == false)
                            //{
                            //    str = ThisAddIn.Today().ToString("f");
                            //    str = str.Replace(":", "时");
                            //    str = str + "分";
                            //}
                        }
                        Globals.logAdapter.Record("现在时间", "4:" + str, true);
                        cc.Text = str;
                        Globals.logAdapter.Record("现在时间", "5:OK", true);
                        if (cc.ParentNode.BaseName != "审")
                            cc.Range.Editors.Add(ref ob);
                    }
                    break;
                case "现在时分":
                    if (!Globals.RefreshInf == true)
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = ThisAddIn.Today().ToString("HH:mm");
                        }
                        else
                        {
                            str = ThisAddIn.Today().ToString("HH时mm分");
                            ////if (NewDate == false)
                            ////{
                            ////    str = ThisAddIn.Today().ToString("f");
                            ////    str = str.Split(':')[0];
                            ////    str = str + "时";
                            ////}
                        }
                        cc.Text = str;
                        if (cc.ParentNode.BaseName != "审")
                            cc.Range.Editors.Add(ref ob);
                    }
                    break;
                case "会诊目的":
                    if (consult)
                    {
                        ////cc.Text = consults.Attributes["reason"].Value;

                        cc.Text = consults.SelectSingleNode("Consultation").Attributes["Reason"].Value;
                    }
                    break;
                case "申请会诊时间":
                    if (consult)
                    {
                        if (ThisAddIn.CanOption(ElementNames.ShowTime))
                        {
                            str = Convert.ToDateTime(consults.SelectSingleNode("Consultation").Attributes["Datetime"].Value).ToString("yyyy-MM-dd HH:mm");
                            cc.Text = str;
                        }
                        else
                        {
                            str = Convert.ToDateTime(consults.SelectSingleNode("Consultation").Attributes["Datetime"].Value).ToString("yyyy年MM月dd日 HH时mm分");
                            //if (NewDate == false)
                            //{
                            //    str = Convert.ToDateTime(consults.SelectSingleNode("Consultation").Attributes["Datetime"].Value).ToString("f");
                            //    str = str.Replace(":", "时");
                            //    str = str + "分";
                            //}
                            cc.Text = str;
                        }

                    }
                    break;
                case "邀请会诊科室":
                    if (consult)
                    {
                        using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                        {
                            str = ep.GetDepartmentName(consults.SelectSingleNode("Consultation").Attributes["DepartmentCode"].Value);
                        }
                        cc.Text = str;
                    }
                    break;
                case "邀请会诊医师":
                    if (consult)
                    {
                        try
                        {
                            string userName = "";
                            string passwd = "";
                            using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                            {
                                pi.AuthenticChek(consults.SelectSingleNode("Consultation").Attributes["DoctorID"].Value, ref userName, ref passwd);
                            }
                            cc.Text = userName;
                        }
                        catch (Exception ex)
                        {
                            Globals.logAdapter.Record("EX9134474992", ex.Message + ">>" + ex.ToString(), true);

                        }
                        //str = consults.SelectSingleNode("Consultation").Attributes["DoctorID"].Value;
                        //  cc.Text=
                    }
                    break;

                default:
                    break;

            }
        }
        public XmlNode GetXmlfile()
        {
            string pathroot = Path.Combine(Globals.workFolder, "Xml");
            if (!Directory.Exists(pathroot)) Directory.CreateDirectory(pathroot);
            string fileName = Path.Combine(Globals.workFolder + @"\Xml\", "views.xml");
            string docfileName = Path.Combine(Globals.workFolder + @"\Xml\", "ptn.docx");
            object oMissing = System.Reflection.Missing.Value;
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9134474994", ex.Message + ">>" + ex.ToString(), true);

                    return null;
                }
            }
            try
            {
                object wdDocName = fileName;
                object FileFormat = (object)Word.WdSaveFormat.wdFormatXML;
                object False = (object)false;
                myDoc.SaveAs(ref wdDocName, ref FileFormat, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                FileFormat = (object)Word.WdSaveFormat.wdFormatXMLDocument;
                wdDocName = docfileName;
                myDoc.SaveAs(ref wdDocName, ref FileFormat, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(fileName);
                return xdoc;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9134474988", ex.Message + ">>" + ex.ToString(), true);

                return null;
            }


        }
        private void OpenPattern(string noteID)
        {
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                bool exam = es.SehPatternDoc(noteID);
                if (exam == true)
                {
                    MessageBox.Show("不存在此式样的模板！");
                    return;
                }
                XmlDocument doc = new XmlDocument();
                XmlNode pattern = doc.CreateElement(EmrConstant.ElementNames.NoteTemplate);

                es.GetPatternDoc(noteID, ref pattern);
                string templateDocName = Path.Combine(Globals.workFolder + "\\", "ptn.docx");
                udt.StringToWordDocument(templateDocName, pattern);
                plContain.Visible = true;
                string str = OpenWordDoc(templateDocName);
                if (str == null) return;
            }
        }
        public void ProtectDoc()
        {
            try
            {
                object False = (object)false;
                object psd = (object)"jwsj";
                if (wordApp == null) return;
                myDoc.Protect(Word.WdProtectionType.wdAllowOnlyReading, ref False, ref psd, ref False, ref False);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9134474888", ex.Message + ">>" + ex.ToString(), true);
            }
        }
        public void UnProtectDoc()
        {
            try
            {
                object psd = (object)"jwsj";
                if (wordApp == null) return;
                myDoc.Unprotect(ref psd);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX9134474788", ex.Message + ">>" + ex.ToString(), true);

            }
        }
        private void RefreshInfo_Click(object sender, EventArgs e)
        {
            string registryID = Globals.RegistryID;
            Globals.RefreshInf = true;
            DataSet dst = ThisAddIn.GetInf(registryID);
            if (dst == null) return;
            UnProtectDoc();
            SetRange(dst, registryID);
            ProtectDoc();
            Globals.RefreshInf = false;

        }
        private void noteSave_Click(object sender, EventArgs e)
        {
           Word.Document  myDoc = ActiveDocumentManager.getDefaultAD();
            myDoc.Activate();
          
            if (myDoc.Saved) return;
            Globals.saveOk = true;
            ThisAddIn.ResetBeginTime();
         
            //参照记录异常修改 2012-03-12
            //this.Capture = true;
            //this.Focus();
            //axWbDoCView.Capture = true;
            myDoc.Activate();
            axWbDoCView.Focus();
            /*应从ActiveDocumentManager.cs中获取打开的文档，不用通过
              指定的文档获得焦点的方式保证其不被存错了。
             */
            // if (axWbDoCView.Focused)
            //     return;
            //this.wordApp.Application.Activate(); 
            //wordApp.ActiveWindow.SetFocus();
            //if (!myDoc.Saved) return;
            string tmpfile = Path.Combine(Globals.currentDirectory, ResourceName.Mytmp);
            if (!CheckSave()) return;

            try
            {
                udt.jj.SaveWordDoc(myDoc, tmpfile, false);
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX9134474780", ex.Message + ">>" + ex.ToString(), true);
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            Globals.docPath = ActiveDocumentManager.getDefaultAD().Path;
            bool newNote = currentNote.noteInfo.GetNewNote();
            string noteID = currentNote.noteInfo.GetNoteID();
            if (EmrDocInfo.NoteID != noteID) {
                Globals.logAdapter.Record("EXMainForm noteSave_Click(object sender, EventArgs e)", "NoteID检测错误EmrDocInfo.NoteID:" + EmrDocInfo.NoteID + ",noteID:" + noteID, true);
            }
            bool flag = false;
             if (newNote)
            {
                EmrDocInfo.setIsNewAndNoteIDForCheck(newNote, noteID);         
                string uniqueText = currentNote.noteInfo.GetNoteUniqueFlag();
                if (tvPatientsNoteAdd(uniqueText, NoteStatus.Draft, EmrConstant.Button.SaveNote))
                {
                    currentNote.noteInfo.SetNewNoteFlag(false);
                   
                    try
                    {
                        myDoc.Saved = true;
                    }
                    catch (Exception ex)
                    {

                        Globals.logAdapter.Record("EX9134474677", ex.Message + ">>" + ex.ToString(), true);

                    }
                    if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.PathID).Trim() != "")
                    {
                        if (Globals.NoteID == ThisAddIn.CanOptionText(ElementNames.PathID).Trim())
                        {
                            //Globals.newPath = false;
                            Globals.NoteID = "";
                        }
                    }
                    flag = true;
                    OpDone opDone = new OpDone("寄存成功");
                    opDone.Show();
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                EmrDocInfo.setIsNewAndNoteIDForCheck(newNote, noteID);
         
                if (GetNoteStatus() == "1")
                {
                    if (MessageBox.Show("该病历已经提交，再提交为审核中且初写者无法修改,您是否继续操作？", EmrConstant.ErrorMessage.Warning,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                        == DialogResult.No) return;
                }
            
                EmrConstant.NoteStatus status = EmrConstant.NoteStatus.Draft;
                switch (currentNote.noteInfo.GetEditMode())
                {
                    case EmrConstant.NoteEditMode.Writing:
                        status = EmrConstant.NoteStatus.Draft;
                        if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.PathID).Trim() != "")
                        {
                            if (Globals.NoteID == ThisAddIn.CanOptionText(ElementNames.PathID).Trim())
                                status = EmrConstant.NoteStatus.FinallyChecked;
                        }
                        break;
                    case EmrConstant.NoteEditMode.Checking:
                        status = EmrConstant.NoteStatus.Checking;
                        break;
                    case EmrConstant.NoteEditMode.FinallyCkecking:
                        status = EmrConstant.NoteStatus.FinallyCkecking;
                        break;
                }
                if (!tvPatientsNoteUpdate(status, EmrConstant.Button.SaveNote)) return;
                else
                {
                    flag = true;
                    OpDone opDone = new OpDone("寄存成功");
                    opDone.Show();
                }

            }
            ///**20111011 zzl 更新临床路径**/
            //if (ThisAddIn.CanOption(ElementNames.ClinicPath) && ThisAddIn.CanOptionText(ElementNames.PathID).Trim() != "")
            //{
            //    if (flag && noteID == ThisAddIn.CanOptionText(ElementNames.PathID).Trim())
            //    {
            //        string zyh = GetRegistryID();
            //        //string zyh = "00009639";
            //        if (Globals.jdxh != null)
            //            Updatelclj(zyh, Globals.jdxh, DateTime.Now);
            //    }
            //    Globals.NoteID = "";
            //}
          
            ThisAddIn.ResetBeginTime();
        }
        private void expTemplate_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (plTemplate.Controls.Count > 0) return;
            plTemplate.Controls.Clear();
            if (NoteTemplate == null)
                NoteTemplate = new EmrTemplate(OpenEmrTemplate);
            this.plTemplate.Controls.Add(NoteTemplate);
            NoteTemplate.Dock = System.Windows.Forms.DockStyle.Fill;

        }
        private void BarHide_Click(object sender, EventArgs e)
        {
            MenuList.Visible = false;

        }
        private void ShowMenu(string Status)
        {

            ////循环全部visible false
            //for (int i = 0; i < lbrole.Count; i++)
            //{
            //    lbrole[i].Visible = false;
            //}
            ////检查权限
            //for (int i = 0; i < lbrole.Count; i++)
            //{
            //    for (int j = 0; j < Globals.myfunctions.Count; j++)
            //    {
            //        if (lbrole[i].Tag.ToString().Trim() == Globals.myfunctions[j].ToString().Trim())
            //            lbrole[i].Visible = true;
            //    }
            //}
            switch (Status)
            {
                case MenuStatus.Writer:
                    tcPatients.Visible = false;
                    plTemplate.Visible = false;
                    epSplitter.Expanded = false;
                    bar1.Visible = true;
                    MenuNote.Visible = true;
                    btnReadExit.Visible = true;
                    btnExexit.Visible = true;
                    btnprintExit.Visible = true;
                    tbpOp.Visible = true;
                    // tbBlock.Visible = false;
                    MenuOutput.Visible = true;
                    tabControl1.SelectedTabIndex = 0;
                    MenuList.Visible = false;
                    OpenTemplate.Visible = false; //2012-10-20 模板显示
                    btnProtect.Visible = true;
                    btnUnProtect.Visible = true;
                    ReportClose.Visible = true;
                    PrintReport.Visible = true;
                    break;
                case MenuStatus.Close:
                    tcPatients.Visible = true;
                    plTemplate.Visible = true;
                    epSplitter.Visible = true;
                    bar1.Visible = false;
                    MenuNote.Visible = false;//2012-10-19 LQ关闭当前页显示
                    btnReadExit.Visible = false;
                    btnExexit.Visible = false;
                    // tbBlock.Visible = true;
                    ReportClose.Visible = false;
                    PrintReport.Visible = false;
                    MenuOutput.Visible = false;
                    tbpOp.Visible = false;
                    tabControl1.SelectedTabIndex = 1;
                    asPersonTemplate2.Visible = false;
                    asDepartTemplate2.Visible = false;
                    asHospitalTemplate2.Visible = false;
                    if (Globals.OpenTemplate == true)
                    {
                        MenuList.Visible = true;
                    }
                    else
                    {
                        MenuList.Visible = false;
                    }
                    btnProtect.Visible = false;
                    btnUnProtect.Visible = false;
                    break;
                case MenuStatus.Reader:
                    bar1.Visible = true;
                    // tbBlock.Visible = false;
                    MenuOutput.Visible = true;
                    // tbpOp.Visible = false;
                    btnReadExit.Visible = true;
                    btnExexit.Visible = true;
                    btnprintExit.Visible = true;
                    tabControl1.SelectedTabIndex = 1;
                    tcPatients.Visible = false;
                    plTemplate.Visible = false;
                    epSplitter.Expanded = false;
                    MenuList.Visible = false;
                    btnProtect.Visible = true;
                    btnUnProtect.Visible = true;
                    ReportClose.Visible = true;
                    PrintReport.Visible = true;
                    break;
                case MenuStatus.None:
                    bar1.Visible = false;
                    MenuNote.Visible = false;
                    tbpOp.Visible = false;
                    btnReadExit.Visible = false;
                    btnExexit.Visible = false;
                    btnprintExit.Visible = false;
                    //tbBlock.Visible = true;
                    MenuOutput.Visible = false;
                    tabControl1.SelectedTabIndex = 1;
                    plTemplate.Visible = false;
                    MenuList.Visible = false;
                    btnProtect.Visible = false;
                    btnUnProtect.Visible = false;
                    ReportClose.Visible = false;
                    PrintReport.Visible = false;
                    break;
                case MenuStatus.TempViewer:
                    tcPatients.Visible = false;
                    plTemplate.Visible = false;
                    epSplitter.Expanded = false;
                    // bar1.Visible = true;
                    // NoteMenu.Visible = true;
                    // tbpOp.Visible = true;
                    //// tbBlock.Visible = false;
                    // //tbiPrint.Visible = true;
                    // tabControl1.SelectedTabIndex = 0;
                    bar1.Visible = false;
                    MenuNote.Visible = false;
                    btnReadExit.Visible = false;
                    btnExexit.Visible = false;
                    btnprintExit.Visible = false;
                    tbpOp.Visible = false;
                    //tbBlock.Visible = true;
                    MenuOutput.Visible = false;
                    tabControl1.SelectedTabIndex = 1;
                    saveTemplate.Visible = true;
                    btnReadExit.Visible = true;
                    plTemplate.Visible = false;
                    MenuList.Visible = false;
                    btnProtect.Visible = true;
                    btnUnProtect.Visible = true;

                    break;
                case MenuStatus.Manager:
                    tcPatients.Visible = false;
                    plTemplate.Visible = false;
                    epSplitter.Expanded = false;
                    bar1.Visible = true;
                    MenuNote.Visible = true;
                    btnReadExit.Visible = true;
                    btnExexit.Visible = true;
                    btnprintExit.Visible = true;
                    tbpOp.Visible = true;
                    // tbBlock.Visible = false;
                    MenuOutput.Visible = true;
                    tabControl1.SelectedTabIndex = 0;
                    MenuList.Visible = false;
                    btnProtect.Visible = true;
                    btnUnProtect.Visible = true;
                    //noteSave.Visible = false;
                    //noteCommit.Visible = false;
                    //noteRef.Visible = false;
                    //referOrder.Visible = false;
                    //referPhrase.Visible = false;
                    //referBlock.Visible = false;
                    //Icd10.Visible = false;
                    //MyPic2.Visible = false;
                    //btnPrintPic.Visible = false;

                    //asPersonTemplate.Visible = false;
                    break;
                default:
                    break;
            }


        }
        public string ErrorMsg(string noteID)
        {
            switch (Globals.emrPattern.StartTimeAttribute(noteID))
            {
                case StartTime.Operation:
                    return "手术尚未执行！";
                case StartTime.Rescued:
                    return "抢救尚未执行！";
                case StartTime.TransferIn:
                    return "患者尚未转入！";
                case StartTime.TakeOver:
                    return "尚未接班！";
                case StartTime.Consult:
                    return "没有会诊申请！";
            }
            return "Error";
        }
        public bool CheckPrecontition(string noteID, int index)
        {
            XmlDocument emrdoc = emrDocuments.Get(index);
            XmlNodeList emrNotes = emrdoc.DocumentElement.SelectNodes(ElementNames.EmrNote);
            XmlNode congeners = ThisAddIn.GetCongener0(noteID, emrNotes);
            string registryID = emrdoc.DocumentElement.GetAttribute(AttributeNames.RegistryID);
            string startTime = Globals.emrPattern.StartTimeAttribute(noteID);

            switch (startTime)
            {
                case StartTime.Operation:
                    XmlNode operations = null;
                    ThisAddIn.OperationTime(registryID, ref operations);
                    if (operations == null) return false;
                    if (congeners.ChildNodes.Count < operations.ChildNodes.Count) return true;
                    else return false;
                case StartTime.Rescued:
                    XmlNode rescues = null;
                    ThisAddIn.RescueTime(registryID, ref rescues);
                    if (rescues == null) return false;
                    if (congeners.ChildNodes.Count < rescues.ChildNodes.Count) return true;
                    else return false;
                case StartTime.TransferIn:
                    XmlNode transferIns = null;
                    ThisAddIn.TransferInTime(registryID, ref transferIns);
                    if (transferIns == null) return false;
                    if (congeners.ChildNodes.Count < transferIns.ChildNodes.Count) return true;
                    else return false;
                case StartTime.TakeOver:
                    XmlNode takeovers = null;
                    ThisAddIn.TakeOverTime(registryID, ref takeovers);
                    if (takeovers == null) return false;
                    if (congeners.ChildNodes.Count < takeovers.ChildNodes.Count) return true;
                    else return false;
                case StartTime.Consult:
                    if (tvPatients.SelectedNode.Level == 1)
                    {
                        string str = tvPatients.SelectedNode.Text;
                        if (str.Split(':').Length == 4)
                            Globals.PatientConsultSequence = str.Split(':')[3].ToString();
                    }
                    if (tvPatients.SelectedNode.Level == 2)
                    {
                        string str = tvPatients.SelectedNode.Parent.Text;
                        if (str.Split(':').Length == 4)
                            Globals.PatientConsultSequence = str.Split(':')[3].ToString();
                    }

                    if (tvPatients.SelectedNode.Level == 3)
                    {
                        string str = tvPatients.SelectedNode.Parent.Parent.Text;
                        if (str.Split(':').Length == 4)
                            Globals.PatientConsultSequence = str.Split(':')[3].ToString();
                    }
                    XmlNode consults = null;
                    ThisAddIn.ConsultTime(registryID, Globals.PatientConsultSequence, ref consults);
                    if (consults == null) return false;
                    else return true;
            }
            return true;
        }
        private void OpenTem()
        {
            string registryID = GetRegistryID();
            DataSet dst = ThisAddIn.GetInf(registryID);
            if (dst == null) return;
            UnProtectDoc();
            SetRange(dst, registryID);
            currentNote.SetEditModes();
            ProtectDoc();
        }
        public void OpenEmrTemplate(XmlNode template, long pk, string type, bool view)
        {
            if (string.IsNullOrEmpty(Globals.templateFile)) return;
            if (template == null) return;
            ThisAddIn.ResetBeginTime();
            
            // if (!IsLocked()) return;
            TreeNode tvPatientsSelectedNode = tvPatients.SelectedNode;

            if (!view)
            {
                if (tvPatientsSelectedNode == null)
                {
                    MessageBox.Show("没有打开已经指定的病历！");
                    return;
                }
                // string noteID = tvPatients.SelectedNode.Name;
                TreeNode registryIDNode;
                Globals.Ttem = true;
                if (tvPatientsSelectedNode == null || tvPatientsSelectedNode.Level == 0)
                {
                    MessageBox.Show(ErrorMessage.NoOpeningEmr, ErrorMessage.Error);
                    return;
                }
                int index = OpeningEmr(tvPatientsSelectedNode, out registryIDNode);

                if (index < 0)
                {
                    MessageBox.Show(ErrorMessage.NoOpeningEmr, ErrorMessage.Error);
                    return;
                }
                bool checkPrecontition = !CheckPrecontition(Globals.NoteID, index);
                if (checkPrecontition)
                {
                    MessageBox.Show(ErrorMsg(Globals.NoteID),
                        ErrorMessage.Warning);
                    return;
                }
                if (IsOpenForReadonly(index))
                {
                    MessageBox.Show(ErrorMessage.NoPrivilegeNew, ErrorMessage.Warning);
                    return;
                }
                TreeNode noteIDNodeExist = FindNoteIDNode(
                    registryIDNode, Globals.NoteID);
                if (noteIDNodeExist != null)
                {
                    /* Check unique? noteIDNodeExist.Tag is unique flag. */
                    if (noteIDNodeExist.Tag.ToString() == StringGeneral.Yes && noteIDNodeExist.Nodes.Count > 0)
                    {
                        MessageBox.Show(ErrorMessage.ExistNoteContent, ErrorMessage.Error);
                        return;
                    }
                }
            }

            #region authorInfo and patientInfo initialization
            AuthorInfo authorInfo = new AuthorInfo();
            EmrConstant.PatientInfo patientInfo = GetPatientInfo(tvPatientsSelectedNode);
            //EmrConstant.PatientInfo patientInfo = new PatientInfo();
            authorInfo.NoteID = template.Attributes[AttributeNames.NoteID].Value;
            authorInfo.NoteName = template.Attributes[AttributeNames.NoteName].Value;
            authorInfo.Writer = Globals.DoctorName;
            authorInfo.WrittenDate = ThisAddIn.Today().ToString(StringGeneral.DateFormat);
            authorInfo.Checker = string.Empty;
            authorInfo.CheckedDate = string.Empty;
            authorInfo.FinalChecker = string.Empty;
            authorInfo.FinalCheckedDate = string.Empty;
            authorInfo.TemplateType = type;
            authorInfo.TemplateName = template.Attributes[AttributeNames.Name].Value;
            authorInfo.WriterLable = template.Attributes[AttributeNames.Sign3].Value;
            if (template.Attributes[AttributeNames.ChildID] != null)
            {
                authorInfo.ChildID = template.Attributes[AttributeNames.ChildID].Value;
            }
            if (template.Attributes[AttributeNames.Sign2] != null)
                authorInfo.CheckerLable = template.Attributes[AttributeNames.Sign2].Value;
            else
                authorInfo.CheckerLable = string.Empty;
            if (template.Attributes[AttributeNames.Sign1] != null)
                authorInfo.FinalCheckerLable = template.Attributes[AttributeNames.Sign1].Value;
            else
                authorInfo.FinalCheckerLable = string.Empty;
            #endregion
            EmrDocInfo.newOpen(authorInfo.NoteID);
            currentNote =
            new EmrNote(authorInfo, (XmlElement)template, NoteEditMode.Writing, GetRegistryID(),
            this);
            currentNote.pk = pk;

            /* In tvPatients, there must be a openning emrDocument to which the new emrNote will belong. */

            string docName = Path.Combine(Globals.currentDirectory + "\\", "ptn.docx");
            try
            {
                if (File.Exists(docName)) File.Delete(docName);
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX9134274677", ex.Message + ">>" + ex.ToString(), true);

            }
            File.Copy(Globals.templateFile, docName);
            Globals.localDocumentEncode = false;
            // string str = OpenWordDoc(docName);
            // if (str == null) return;
            //LoadWord(OpenTemWord);
            OpenTemWord();
            Globals.localDocumentEncode = true;
            currentNote.BecomeNormalNoteNew();
            if (Globals.chuyuan == true)
            {
                Globals.chuyuan = false;
                return;
            }

            if (!view)
            {
                ShowMenu(MenuStatus.Writer);
                OperationEnableForStat(false);
            }
            else
            {
                ShowMenu(MenuStatus.TempViewer);
                //OperationEnableForStat(true);
                //btnReadExit.Visible = true;
            }
            // _Enble();
            TvPatientsEnable(false);
            NoteTemplate.TvTemplateEnable(false);  //2012-03-27 LiuQi模板列表显示功能
            ThisAddIn.ResetBeginTime();
            plContain.Visible = true;

        }
        private void OpenTemWord()
        {
            OpenWordDoc(Path.Combine(Globals.currentDirectory + "\\", "ptn.docx"));
            CloseLoad();

        }
        private void MyPicture_Click(object sender, EventArgs e)
        {
            //if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                XmlNode departPic = ThisAddIn.xmlPicGalleryWriter(Globals.OpDepartID);
                XmlNode personPic = ThisAddIn.xmlPicGalleryWriter(Globals.DoctorID);
                PicGallery pic = new PicGallery(departPic, personPic);
                FormShow(pic);
                Cursor.Current = Cursors.Default;
                if (pic.DialogResult == DialogResult.Cancel)
                    return;
                InsertPic(pic.templateFile, pic.linkToFile, pic.saveWithDocument);
                //InsertPicOle(pic.templateFile, pic.linkToFile);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX91344234677", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不能插入图片", 0.3, MsgTpe.Warning);
            }
        }
        private void referOrder_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            ThisAddIn.ResetBeginTime();
            Cursor.Current = Cursors.WaitCursor;
            string registryID = Globals.RegistryID;
            DoctorOrders orders = new DoctorOrders(registryID, wordApp, GetTreeviewPatientsStatus());
            if (orders.existError == false)
            {
                if (orders.ShowDialog() == DialogResult.OK)
                {
                    string text = orders.GetCopyText();
                    UnProtectDoc();
                    try
                    {
                        wordApp.Selection.InsertAfter(text);
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX91544234677", ex.Message + ">>" + ex.ToString(), true);

                        MessageBox.Show("插入失败！");
                    }
                    ProtectDoc();
                    orders.Close();
                    //FocusOnEnd();
                }
            }
            Cursor.Current = Cursors.Default;
        }
        public void SetTreeviewPatientsStatus(string status)
        {
            tvPatients.Tag = status;
        }
        public string GetTreeviewPatientsStatus()
        {
            return tvPatients.Tag.ToString();
        }
        private void FocusOnEnd()
        {
            Word.Range range = wordApp.Selection.Range;
            int start = range.End;
            range.Start = start;
            range.End = start;
            range.Select();
        }
        private void cbTrack_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTrack.Checked)
            {
                wordApp.ActiveWindow.View.ShowRevisionsAndComments = true;
                wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
            }
            else
            {
                wordApp.ActiveWindow.View.ShowRevisionsAndComments = false;
                wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
            }
        }
        /* ------------------------------------------------------------------------------------ */
        private NoteEditMode EditModeWhenNoteDraft(string writerID)
        {
            if (Globals.DoctorID == writerID)
            {
                return EmrConstant.NoteEditMode.Writing;
            }
            else
            {
                //if (Globals.ThisAddIn.auditSystem.Substring(0, 1) == EmrConstant.Auditing.LevelA)
                return EmrConstant.NoteEditMode.Reading;
                //else
                //    return EmrConstant.NoteEditMode.Nothing;
            }
        }
        /* ------------------------------------------------------------------------------------ */
        public void ViewTemplate(bool view)
        {
            if (view)
            {
                noteSave.Visible = false;
                RefreshInfo.Visible = false;
                noteCommit.Visible = false;
                noteRef.Visible = false;
                //referBlock.Visible = false;
                referOrder.Visible = false;
                referPhrase.Visible = false;
                asPersonTemplate.Visible = false;
                asDepartTemplate.Visible = false;
                //noteValuate.Enabled = false; ;
                saveTemplate.Visible = false;
                Icd10.Visible = false;
                //signature.Enabled = false;
                asHospitalTemplate.Visible = false;
            }
        }
        public void OperationEnableForStat(bool readOnly)
        {
            if (readOnly)
            {
                noteSave.Visible = false;
                RefreshInfo.Visible = false;
                noteCommit.Visible = false;
                noteRef.Visible = false;
                RefreshInfo.Visible = false;
                MyPic2.Visible = false;
                btnPrintPic.Visible = false;
                split3.Visible = false;
                split2.Visible = false;
                //referBlock.Visible = false;
                referOrder.Visible = false;
                referPhrase.Visible = false;
                asPersonTemplate.Visible = false;
                asDepartTemplate.Visible = false;
                //noteValuate.Enabled = false; ;
                saveTemplate.Visible = false;
                Icd10.Visible = false;
                //signature.Enabled = false;

                asHospitalTemplate.Visible = false;
            }
            else
            {
                noteSave.Visible = true;
                RefreshInfo.Visible = true;
                noteCommit.Visible = true;
                noteRef.Visible = true;
                //referBlock.Visible = true;
                referOrder.Visible = true;
                referPhrase.Visible = true;
                asPersonTemplate.Visible = true;
                asDepartTemplate.Visible = true;
                // saveTemplate.Visible = true;
                Icd10.Visible = true;
                //signature.Enabled = true;
                //noteValuate.Enabled = true;
                asHospitalTemplate.Visible = true;
            }
        }
        public void DisableEdit(bool value)
        {
            noteCommit.Visible = value;
            RefreshInfo.Visible = value;
            noteSave.Visible = value;
            //referBlock.Visible = value;
            referOrder.Visible = value;
            referPhrase.Visible = value;
        }
        private string NoteState(string state)
        {
            int index = Convert.ToInt32(state);
            EmrConstant.NoteStateText nst = new EmrConstant.NoteStateText();
            return " " + nst.Text[index] + " ";
        }
        private void noteCommit_Click(object sender, EventArgs e)
        {
            Word.Document myDoc = ActiveDocumentManager.getDefaultAD();
            myDoc.Activate();
            Globals.saveOk = true;
            Globals.Sign = false;
            ThisAddIn.ResetBeginTime();
            if (GetNoteStatus() == "1")
            {
                if (MessageBox.Show("该病历已经提交，再提交为已审核且初写者无法修改,您是否继续操作？", EmrConstant.ErrorMessage.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;
            }
            if (GetNoteStatus() == "3")
            {
                if (MessageBox.Show("该病历已审核，再提交为已终审且审核者无法修改,您是否继续操作？", EmrConstant.ErrorMessage.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    == DialogResult.No) return;
            }
            

            if (MessageBox.Show(ErrorMessage.ConfirmCommitNote, ErrorMessage.Warning,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel) return;
            //try
            //{

            //    object ob = Word.WdEditorType.wdEditorEveryone;
            //    if (ed != null)
            //        ed.Item(ref ob).DeleteAll();
            //}
            //catch
            //{
            if (ThisAddIn.CanOption(ElementNames.EncryptSign))
            {
                Globals.Sign = true;
            }
            if (GetNoteStatus() == "0")
                Globals.IsCommit = true;
            else Globals.IsCommit = false;
            //2012-03-14
            try
            {
                if (!IsNull())
                {
                    ProtectDoc();
                    return;
                }
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX91544534677", ex.Message + ">>" + ex.ToString(), true);

                MessageBox.Show("双击在当前打开的病历可编辑区域，或者先关闭当前参照记录的文档，再进行寄存或提交！");
                return;
            }

            /* After Commiting, no edit again and so the docment closed. */
            if (NoteCommiting())
            {
                //DocPrint_Click(sender, e);
                _ExitDoc();
            }
            else Globals.health = false;
            Globals.Sign = false;
            //刘伟加入提交后判断临床路径
            if (ThisAddIn.CanOption(ElementNames.ClinicPath))//如果开启临床路径
            {
                string registryID = GetRegistryID();
                string noteID = Globals.NoteID;
                string doctorID = Globals.DoctorID;
                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                {

                    DataSet dst = es.obtainjkdm(noteID);
                    if (dst != null)
                    {
                        if (dst.Tables[0].Rows.Count > 0)
                        {
                            string jkdm = Convert.ToString(dst.Tables[0].Rows[0]["jkdm"].ToString());
                            if (jkdm == null) return;
                            string tf = es.ClinicPathmonitor(registryID, jkdm, doctorID);
                        }
                    }
                }
            }
        }
        public bool IsNull()
        {

            for (int m = 1; m < myDoc.XMLNodes.Count + 1; m++)
            {
                Word.XMLNode xn = myDoc.XMLNodes[m];
                if (xn.BaseName == "电子病历")
                {
                    for (int i = 1; i <= xn.ChildNodes.Count; i++)
                    {
                        int j = 0;
                        if (xn.ChildNodes[i].Text == null) continue;
                        if (xn.ChildNodes[i].Text.Trim().Length < 11)
                        {
                            if (xn.ChildNodes[i].BaseName == "医师" || xn.ChildNodes[i].BaseName == "终身者" || xn.ChildNodes[i].BaseName == "审核者")
                                continue;
                            foreach (char c in xn.ChildNodes[i].Text.Trim())
                            {
                                j++;

                            }
                            if (j == 0)
                            {
                                MessageBox.Show(xn.ChildNodes[i].BaseName + "不能为空！", ErrorMessage.Warning);
                                xn.ChildNodes[i].Range.Select();
                                return false;
                            }
                        }
                    }
                    continue;
                }
                if (xn.BaseName == "删")
                {
                    UnProtectDoc();
                    xn.Range.Text = "";
                    xn.Delete();
                    m--;
                }
            }
            ProtectDoc();
            return true;
        }
        private void Encryption_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            ProtectDoc();

        }
        private void Decryption_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            UnProtectDoc();
        }
        private void Accept_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            object ob = Word.WdEditorType.wdEditorEveryone;
            myDoc.Application.Selection.Editors.Add(ref ob);

        }
        private void RemoveDoNotWant(int mode)
        {
            for (int k = tvPatients.Nodes.Count - 1; k >= 0; k--)
            {
                TreeNode patient = tvPatients.Nodes[k];
                if (patient.Text.Split(Delimiters.Space).Length == mode) patient.Remove();
            }
        }
        private void FontSet_Click(object sender, EventArgs e)
        {

            if (wordApp == null) return;
            if (!plContain.Visible) return;
            FontSetting fs = new FontSetting();
            if (fs.ShowDialog() != DialogResult.OK) return;


            try
            {
                myDoc.Application.Selection.Font.Name = fs.GetfamilyName();
                myDoc.Application.Selection.Font.Size = fs.GetemSize();
                myDoc.Application.Selection.Font.StrikeThrough = fs.GetStrikethrough();
                if (fs.GetUnderline()) myDoc.Application.Selection.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                else myDoc.Application.Selection.Font.Underline = Word.WdUnderline.wdUnderlineNone;
                myDoc.Application.Selection.Font.Bold = fs.GetBold();
                myDoc.Application.Selection.Font.Italic = fs.GetItalic();
                myDoc.Application.Selection.Font.Color = (Word.WdColor)ColorTranslator.ToWin32(fs.GetColor());
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915422346756", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }

        }
        private void btnBold_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                if (btnBold.Checked == false) myDoc.Application.Selection.Font.Bold = -1;
                else myDoc.Application.Selection.Font.Bold = 0;
                btnBold.Checked = !btnBold.Checked;

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915422246756", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }

        }
        private void btnUnderline_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                if (btnUnderline.Checked == false) myDoc.Application.Selection.Font.Underline = Word.WdUnderline.wdUnderlineSingle;
                else myDoc.Application.Selection.Font.Underline = Word.WdUnderline.wdUnderlineNone;
                btnUnderline.Checked = !btnUnderline.Checked;

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915422346754", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }
        }
        private void Fonts_Click(object sender, EventArgs e)
        {
            barFonts.Expanded = !barFonts.Expanded;
        }
        private void FontColor_SelectedColorChanged(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {

                myDoc.Application.Selection.Font.Color
                    = (Word.WdColor)ColorTranslator.ToWin32(FontColor.SelectedColor);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915422346751", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }
        }
        private void FontSading_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {

                if (myDoc.Application.Selection.Font.Shading.Texture == Word.WdTextureIndex.wdTextureNone)
                {
                    myDoc.Application.Selection.Font.Shading.Texture = Word.WdTextureIndex.wdTexture15Percent;
                    //myDoc.Application.Selection.Font.Shading.ForegroundPatternColor = WdColor.wdColorDarkBlue;
                    //myDoc.Application.Selection.Font.Shading.BackgroundPatternColor = WdColor.wdColorDarkYellow;
                    FontSading.Checked = false;

                }
                else
                {
                    myDoc.Application.Selection.Font.Shading.Texture = Word.WdTextureIndex.wdTextureNone;
                    myDoc.Application.Selection.Font.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlue;
                    myDoc.Application.Selection.Font.Shading.ForegroundPatternColor = Word.WdColor.wdColorDarkBlue;
                    FontSading.Checked = true;
                }

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915422346744", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }
        }
        private void btnItalic_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                if (btnItalic.Checked == false) myDoc.Application.Selection.Font.Italic = -1;
                else myDoc.Application.Selection.Font.Italic = 0;
                btnItalic.Checked = !btnItalic.Checked;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX915522346744", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档被保护，不能改变字体", 0.3, MsgTpe.Warning);
            }
        }
        private void SavePhrases(XmlElement Phrases, string SG)
        {
            // if (wordApp == null) return;
            // if (!plContain.Visible) return;
            #region Save to database

            /* Determine department or person */
            string departmentCode, doctorID;
            if (SG == EmrConstant.StringGeneral.PersonPhrase)
            {
                departmentCode = EmrConstant.StringGeneral.NullCode;
                doctorID = Globals.DoctorID;
            }
            else
            {
                departmentCode = Globals.OpDepartID;
                doctorID = EmrConstant.StringGeneral.NullCode;
            }
            /* Add or replace */
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string msg = es.AddNotePhrases(departmentCode, doctorID, Phrases);
                    if (msg != null)
                    {
                        MessageBox.Show(msg, EmrConstant.ErrorMessage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925522346744", ex.Message + ">>" + ex.ToString(), true);

                }

            }
            #endregion
        }
        private void Icd10_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            //DataTable ds = new DataTable();
            //ds.Select();
            Cursor.Current = Cursors.WaitCursor;
            icd10 icd = new icd10(Globals.icd10File);
            icd.Location = new Point(700, 60);
            if (icd.ShowDialog() == DialogResult.Cancel) return;
            string text = icd.GetIcd10Text();
            try
            {
                Word.Range range = wordApp.Selection.Range;
                range.InsertAfter(text);
            }
            catch (Exception ex)
            {

                Globals.logAdapter.Record("EX925522346244", ex.Message + ">>" + ex.ToString(), true);

            }
            Cursor.Current = Cursors.Default;
        }
        private void referPhrase_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            XmlNode dptPhrase = ThisAddIn.GetPhrase(Globals.OpDepartID, StringGeneral.NullCode);
            XmlNode prnPhrase = ThisAddIn.GetPhrase(StringGeneral.NullCode, Globals.DoctorID);
            Phrase phr = new Phrase(dptPhrase, prnPhrase);
            if (phr.ShowDialog() != DialogResult.OK) return;
            try
            {
                wordApp.Selection.InsertAfter(phr.GetText());
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925412346744", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不允许插入短语", 0.3, MsgTpe.Warning);
            }
        }
        private void TabelSet_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            TableInfo ti = new TableInfo();
            if (ti.ShowDialog() != DialogResult.OK) return;
            try
            {
                object unknow = Type.Missing;
                myDoc.Application.Selection.Range.Text = " ";
                UnProtectDoc();
                Word.Table newTable = myDoc.Tables
                    .Add(myDoc.Application.Selection.Range, ti.GetRow(), ti.GetColumn(), ref unknow, ref unknow);
                newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                newTable.Shading.ForegroundPatternColor = Word.WdColor.wdColorAutomatic;
                newTable.Shading.BackgroundPatternColor = Word.WdColor.wdColorWhite;
                ProtectDoc();
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925412346754", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不能插入表格", 0.3, MsgTpe.Warning);
            }
        }
        private void barSign_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            Symbol ti = new Symbol();
            if (ti.ShowDialog() != DialogResult.OK) return;
            try
            {

                myDoc.Application.Selection.Range.Text = ti.GetSign();

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925411646754", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不能插入图片", 0.3, MsgTpe.Warning);
            }

        }
        private void barPicture_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                object oMissing = Type.Missing;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "图片类型(*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png";
                ofd.Title = "选择图片";
                ofd.ValidateNames = true;
                ofd.CheckPathExists = true;
                ofd.CheckFileExists = true;
                Word.Range range = myDoc.Application.Selection.Range;
                object orange = (object)range;
                object oClassType = "Paint.Picture";
                object linkToFile = false;
                if (ofd.ShowDialog() == DialogResult.Cancel) return;
                object file = ofd.FileName;
                myDoc.Application.Selection.InlineShapes.AddPicture(ofd.FileName, oMissing, oMissing, oMissing);
                //myDoc.Application.Selection.InlineShapes.AddOLEObject(ref oClassType, ref file, ref linkToFile, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref orange);

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925411646755", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不能插入图片", 0.3, MsgTpe.Warning);
            }

        }
        private void FormShow(Form tform)
        {
            foreach (Form f in System.Windows.Forms.Application.OpenForms)
            {
                if (tform.GetType().Equals(f.GetType()))
                {
                    f.TopMost = true;
                    return;
                }
            }

            tform.ShowDialog();
        }
        //private Word.Document myDoc = null;
        private void setCopyConfig()
        {
            throw new NotImplementedException();
        }
        private void _EditDoc()
        {
            if (Globals.EdeditNote)
            {
                EditDoc();
                Globals.EdeditNote = false;
            }
            if (Globals.NnewNote)
            {
                NewDoc();
                Globals.NnewNote = false;
            }
            if (Globals.Mmarge)
            {
                margeDoc();
                Globals.Mmarge = false;
            }
            if (Globals.Ttem)
            {
               
                OpenTem();               
                Globals.Ttem = false;
            }
            if (Globals.Cselect)
            {
                _Cselect();
                Globals.Cselect = false;
            }
            if (Globals.Cselectd)
            {
                _Cselectd();
                Globals.Cselectd = false;
            }
            if (Globals.BnewBlock)
            {
                _newBlock();
                Globals.BnewBlock = false;
            }
            if (Globals.Vvaluate)
            {
                _Valuate();
                Globals.Vvaluate = false;
            }

            if (Globals.CreateConsent)
            {
                _CreateCon();
                Globals.CreateConsent = false;
            }
            if (Globals.NewInsertLc)
            {
                _NewInsertLc();
                Globals.NewInsertLc = false;
            }
            if (Globals.InsertLc)
            {
                _InsertLc();
                Globals.InsertLc = false;
            }

            if (Globals._DuplicateNote)
            {
                _DuplicateNote();
                Globals._DuplicateNote = false;
            }
            wordApp.WindowBeforeDoubleClick +=
                  new Word.ApplicationEvents4_WindowBeforeDoubleClickEventHandler(wordApp_WindowBeforeDoubleClick);
            wordApp.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(wordApp_WindowBeforeRightClick);
            wordApp.WindowSelectionChange += new Word.ApplicationEvents4_WindowSelectionChangeEventHandler(wordApp_WindowSelectionChange);
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // oDocument = null;
            axWbDoCView.Navigate("about:blank");
        }

        //private void referBlock_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = ThisAddIn.GetEmrBlock(Globals.OpDepartID);
        //    SetBlocks sbs = new SetBlocks(dt);
        //    sbs.ShowDialog();
        //    string blockName = sbs.GetBlockName();
        //    bool isblock = true;
        //    ThisAddIn.ImportWordDoc(blockName, wordApp, false, isblock);
        //    ThisAddIn.ResetBeginTime();
        //}
        private void OpenTemplate_Click(object sender, EventArgs e)
        {
            Globals.OpenTemplate = true;//激活记录模板操作
            OpenTemplates();
        }
        private void OpenTemplates()
        {
            MenuList.Visible = true;
            eplTemplate.Visible = true;
            eplRef.Visible = false;
            Cursor.Current = Cursors.WaitCursor;
            eplTemplate.Expanded = !eplTemplate.Expanded;
            eplTemplate.Expanded = true;
            Cursor.Current = Cursors.Default;
        }
        private void PatientSearch_Click_1(object sender, EventArgs e)
        {
            XmlNode ops = ThisAddIn.GetOperatorList();
            XmlNode departs = ThisAddIn.GetDepartList();
            QueryPatient qp = new QueryPatient(ops, departs);

            if (qp.ShowDialog() != DialogResult.OK) return;

            XmlNode patients = null;
            switch (qp.GetMode())
            {
                case 0:
                    patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.ArchiveNum, qp.GetText());
                    break;
                case 1:
                    patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.RegistryID, qp.GetText());
                    break;
                case 2:
                    patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.PatientName, qp.GetText());
                    break;
                case 3:
                    patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.Commpose, qp.GetText());

                    break;
                case 6:
                    patients = ThisAddIn.GetPatientsList(gjtEmrPatients.QueryMode.AllPatientName, qp.GetText());
                    break;
               
            }
            if (patients != null)
            {
                if (patients.ChildNodes.Count > 0)
                {
                    LoadPatientList(patients);
                    if (qp.GetMode() == 3) SetTreeviewPatientsStatus(InpatientStatus.Leave);
                    else SetTreeviewPatientsStatus(InpatientStatus.Stay);
                }
                else
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.NoFindResult, EmrConstant.ErrorMessage.Warning);
                }
            }
        }
        private void MyPic_Click(object sender, EventArgs e)
        {
            //if (wordApp == null) return;
            if (!plContain.Visible) return;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                XmlNode departPic = ThisAddIn.xmlPicGalleryWriter(Globals.OpDepartID);
                XmlNode personPic = ThisAddIn.xmlPicGalleryWriter(Globals.DoctorID);
                PicGallery pic = new PicGallery(departPic, personPic);
                //if (pic.ShowDialog() == DialogResult.Cancel) return;
                //pic.Show();
                FormShow(pic);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925511646756", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.ShowMessage("文档保护区域，不能插入图片", 0.3, MsgTpe.Warning);
            }
        }
        private void EmrPhrase_Click_1(object sender, EventArgs e)
        {
            XmlNode dptPhrase = ThisAddIn.GetPhrase(Globals.OpDepartID, StringGeneral.NullCode);
            XmlNode prnPhrase = ThisAddIn.GetPhrase(StringGeneral.NullCode, Globals.DoctorID);
            SetPhrase phr = new SetPhrase(dptPhrase, prnPhrase);
            FormShow(phr);
            SavePhrases(phr.GetPhrases(StringGeneral.PersonPhrase), StringGeneral.PersonPhrase);
            SavePhrases(phr.GetPhrases(StringGeneral.DepartmentPhrase), StringGeneral.DepartmentPhrase);

        }
        private void asPersonTemplate_Click(object sender, EventArgs e)
        {
            //bool newNote = currentNote.noteInfo.GetNewNote();
            //if (newNote)
            //{
            //    if (!Globals.saveOk)
            //        MessageBox.Show("请先进行寄存病历，再将病历保存模板！");
            //    return;
            //}
            SaveTemplate(StringGeneral.PersonTemplate);
        }
        private void asHospitalTemplate_Click(object sender, EventArgs e)
        {
            //bool newNote = currentNote.noteInfo.GetNewNote();
            //if (newNote)
            //{
            //    if(!Globals.saveOk)
            //         MessageBox.Show("请先进行寄存病历，再将病历保存模板！");
            //    return;
            //}
            SaveTemplate(StringGeneral.HospitalTemplate);

        }
        private void asDepartTemplate_Click(object sender, EventArgs e)
        {
            //bool newNote = currentNote.noteInfo.GetNewNote();
            //if (newNote)
            //{
            //    if(!Globals.saveOk)
            //         MessageBox.Show("请先进行寄存病历，再将病历保存模板！");
            //    return;
            //}
            SaveTemplate(StringGeneral.DepartTemplate);

        }
        private void noteRef_Click(object sender, EventArgs e)
        {
            TreeNode patient = GetPatientNode();
            if (nr == null || nr.IsDisposed)
            {
                nr = new NoteRef(patient, this);
                nr.Show();
            }
            else
            {
                MessageBox.Show("已经打开一个窗口！");
            }
        }
        private void btnQualityQuery_Click(object sender, EventArgs e)
        {
            if (!ThisAddIn.CanExeutiveCommand(RibbonID.QualityQuery)) return;
            if (Globals.offline) return;

            XmlDocument doc = new XmlDocument();
            string patientListFileName = udt.MakePatientListFileName(Globals.DoctorID);
            if (File.Exists(patientListFileName)) File.Delete(patientListFileName);
            ThisAddIn.xmlPatientWriterQL(Globals.DoctorID, 2, patientListFileName);
            if (!File.Exists(patientListFileName)) return;

            doc.Load(patientListFileName);
            XmlNode qualityInfo = doc.CreateElement(ElementNames.QualityInfo);
            if (!ThisAddIn.GetQualityInfo(doc.DocumentElement, ref qualityInfo, null, null)) return;

            XmlNodeList patients = qualityInfo.SelectNodes(ElementNames.Patient);
            Qualityks quality = new Qualityks(Globals.DoctorID, patients);
            quality.ShowDialog();
        }
        private void btnQCReportSelf_Click(object sender, EventArgs e)
        {

        }
        private void btnQCSelfReport_Click(object sender, EventArgs e)
        {
            using (QCReport qcr = new QCReport(false, true))
            {
                qcr.ShowDialog();
            }
        }
        private void btnPrintInfo_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            PrintInfo();
            ThisAddIn.ResetBeginTime();
        }
        private void _Cselect()
        {
            select.GetNewReport(select.GetDataNew());
            //_Enble();
        }
        private void _Cselectd()
        {
            selectend.GetNewReport(selectend.GetDataNew());
            // _Enble();
        }



        private void _Enble()
        {
            TvPatientsEnable(false);
            btnEmrQueryScoreEnd.Visible = false;
            btnEmrQueryScore.Visible = false;
            btnNewBlock.Visible = false;
            btnBlockWin.Visible = false;
        }
        private void btnEmrQueryScore_Click(object sender, EventArgs e)
        {
            selectend = new CSelectEndP(this);
            selectend.ShowInTaskbar = false;
            selectend.Show();

        }
        private void btnEmrQueryScoreEnd_Click(object sender, EventArgs e)
        {
            select = new CSelectP(this);
            select.ShowInTaskbar = false;
            select.Show();

            // ShowMenu(MenuStatus.Close);
        }
        private void buttonX1_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            using (SetIndividualOptions sos = new SetIndividualOptions())
            {
                sos.Location = new Point(100, 150);
                sos.ShowDialog();
                ThisAddIn.GetLocalOptionValues();
            }
            ThisAddIn.ResetBeginTime();
        }
        private void btnUpdateSetOption_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            ThisAddIn.GetOptionValues();
            SetOp();
            ThisAddIn.ResetBeginTime();
        }
        private void btnArchiveSettings_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            TitleLevel tl = Globals.doctors.GetTitleLevel(Globals.DoctorID);
            if ((int)tl > (int)TitleLevel.AttendingDoctor || tl == TitleLevel.Nothing)
            {
                MessageBox.Show("权限不足！", "提示");
                return;
            }
            FilingSetup FS = new FilingSetup(Globals.OpDepartID);
            FS.Location = new Point(100, 180);
            FS.ShowDialog();
            ThisAddIn.ResetBeginTime();
        }
        private void btnOnDeputing_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (!ThisAddIn.CanExeutiveCommand(EmrConstant.RibbonID.Deputing)) return;
            ThisAddIn.Deputing();
            ThisAddIn.ResetBeginTime();

        }
        private void btnOnDeputed_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (!ThisAddIn.CanExeutiveCommand(EmrConstant.RibbonID.Deputed)) return;
            ThisAddIn.Deputed();
            ThisAddIn.ResetBeginTime();
        }
        private void btnOnlineCheck_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            Cursor.Current = Cursors.WaitCursor;
            OnlineCheck();
            Cursor.Current = Cursors.Default;
            ThisAddIn.ResetBeginTime();
        }
        private void btnNewBlock_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            Globals.currentBlockIsNew = true;
            string meg = NewEmrBlock();
            if (meg == null) return;
            BlockEnable(BlockStatus.Bnew);
        }
        private void btnSaveBlock_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            SaveEmrBlock();
            btnCloseBlock_Click(sender, e);
            BlockEnable(BlockStatus.Bnone);
        }
        private void btnCloseBlock_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            _ExitDoc();
            //CloseEmrBlock();
            BlockEnable(BlockStatus.Bnone);
        }
        private void btnBlockWin_Click(object sender, EventArgs e)
        {
            if (Globals.offline)
            {
                return;
            }
            DataTable dt = ThisAddIn.GetEmrBlock(Globals.OpDepartID);
            SetBlocks sbs = new SetBlocks(dt);
            sbs.ShowDialog(); if (sbs.DialogResult != DialogResult.OK) return;
            string blockName = sbs.GetBlockName();
            if (wordApp != null) return;
            string mes = OpenBlock(blockName);
            if (mes == null) return;
            Globals.Bblock = true;
            Globals.BdocFile = blockName;
            BlockEnable(BlockStatus.Blist);
            ThisAddIn.ResetBeginTime();
        }
        private void btnExit_Click_1(object sender, EventArgs e)
        {
            if (nr == null || nr.IsDisposed)
            {
                ThisAddIn.logon.Show();
                Globals.WriteOff = true;
                this.Hide();
                return;
            }
            else nr.Close();
            ThisAddIn.logon.Show();
            Globals.WriteOff = true;
            this.Hide();
        }
        private void btnReadExit_Click(object sender, EventArgs e)
        {
            //ProcessMonitor.endMonitor();
            if (wordApp != null)
                try
                {
                    Word.Document myDoc = ActiveDocumentManager.getDefaultAD(); //2012-10-18 关闭当前病历
                    myDoc.Activate();
                    _ExitDoc();
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX9225145659826", ex.Message + ">>" + ex.ToString(), true);
                    MessageBox.Show("系统出现异常,请点击注销重新登陆！");
                }
        }
        private void _ExitDoc()
        {
            try
            {
                if (wordApp != null)
                {
                    //2012-03-14
                    ShowMenu(MenuStatus.Close);
                    try
                    {
                        //myDoc.Save();
                    }
                    catch (Exception) { }

                }

            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX925511646757", ex.Message + ">>" + ex.ToString(), true);

                ThisAddIn.killWordProcess();
                wordApp = null;
                // solidTextChange = null;
                od.Dispose();
                axWbDoCView.Navigate("about:blank");
                TvPatientsEnable(true);

                btnReadExit.Visible = false;
                //btnSelectExit.Visible = false;
                saveTemplate.Visible = false;
            }
            finally
            {
                wordApp = null;
                axWbDoCView.Navigate("about:blank");
                TvPatientsEnable(true);
                if (NoteTemplate != null)
                {
                    NoteTemplate.TvTemplateEnable(true);//2012-03-27 模板列表显示功能zzl
                }
                OpenTemplate.Visible = true;
                btnReadExit.Visible = false;
                //btnSelectExit.Visible = false;
                saveTemplate.Visible = false;
            }

        }
        //private void _ExitDoc2()
        //{

        //    if (wordApp != null)
        //    {
        //        try
        //        {
        //            myDoc.Save();
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        finally
        //        {E:\EMR2.0NEW\EMRWS2.0\EMRWS2.0\app.config
        //            wordApp = null;
        //            solidTextChange = null;
        //            od.Dispose();
        //            ShowMenu(MenuStatus.Close);
        //            axWbDoCView.Navigate("about:blank");
        //            TvPatientsEnable(true);
        //            btnReadExit.Visible = false;
        //            btnSelectExit.Visible = false;
        //            btnEmrQueryScore.Visible = true;
        //            btnEmrQueryScoreEnd.Visible = true;
        //            btnBlockWin.Visible = true;
        //            btnNewBlock.Visible = true;
        //        }
        //    }
        //    axWbDoCView.Navigate("about:blank");
        //    TvPatientsEnable(true);
        //}
        private void PatientStayin_Click(object sender, EventArgs e)
        {
            Globals.inoutmode = 6;
            //int index = cbxArea.SelectedIndex;
            //cbxArea.SelectedIndex = 0;
            //cbxArea.SelectedIndex = index;           
            tcPatients_s();
        }
        private void PatientGone_Click(object sender, EventArgs e)
        {
            Globals.inoutmode = 5;
            tcPatients_s();
        }
        private void PatientsConsult_Click(object sender, EventArgs e)
        {
            GetPatientsConsult();
        }
        //打印事件
        private void DocPrint_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            axWbDoCView.Focus();
            //没有提交不能打印
            if (ThisAddIn.CanOption(ElementNames.CommitedAsEnd))
            {
                if (!IsCommitedSelectedNote())
                {
                    MessageBox.Show(ErrorMessage.NoPrintNotCommited, ErrorMessage.Warning);
                    return;
                }
            }
            //无痕打印
            if (ThisAddIn.CanOption(ElementNames.PrintNoView))
            {
                wordApp.ActiveWindow.View.ShowRevisionsAndComments = false;
                wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
            }
            //续打是否开启
            if (ThisAddIn.CanOption(ElementNames.CanEndPrint))
            {
                string groupCode = Globals.emrPattern.EndPrintGroup(currentNote.noteInfo.GetNoteID());
                if (groupCode != Groups.Zero)
                {
                    //MessageBox.Show("非续打病历记录组！");
                    //return;
                    /**zzl 断续**/
                    string isSingle = "no";
                    if (Globals.emrPattern.GetEmrNote(currentNote.noteInfo.GetNoteID()).Attributes[AttributeNames.SingleContinue] != null)
                        isSingle = Globals.emrPattern.GetEmrNote(currentNote.noteInfo.GetNoteID()).Attributes[AttributeNames.SingleContinue].Value.ToString();

                    myDoc.Activate();
                    UnProtectDoc();
                    wordApp.WindowState = Word.WdWindowState.wdWindowStateMaximize;
                    string pageHeader = myDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Text;
                    Word.Font pageHeaderFont = myDoc.Sections[1].Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Font;
                    XmlNode group = Globals.emrPattern.GetOneGroup(groupCode);
                    bool ok, no, cancel, printAll, printThis, thiscursor, thiscursorDel, JHPrint, printPageRange;
                    bool printEnd1, rdefault1, self1;
                    int iPageStart, iPageEnd;
                    string page1 = "", printerName, space1;
                    DocPrintHelper dch = new DocPrintHelper(myDoc);
                    ContiPrint pm = new ContiPrint(dch);
                    ok = pm.ShowMsg(out printEnd1, out rdefault1, out self1, ref page1, out space1, out no, out cancel,
                        out printAll, out printThis, out  thiscursor, out JHPrint, out printerName, out thiscursorDel, out printPageRange, out iPageStart, out iPageEnd);
                    if (ok)
                    {
                        if (JHPrint)
                        {

                            SplitPagesBySection.setPageNum(wordApp);
                            //dch.setPrintName(printerName);
                            if (printThis)
                            {
                                //2012-12-24 职工去除底纹
                                if (ThisAddIn.CanOption(ElementNames.DelGrid))
                                    Del();
                                dch.printCurrPage(printerName);
                            }
                            else if (thiscursor)
                            {
                                //2012-12-24 职工去除底纹
                                if (ThisAddIn.CanOption(ElementNames.DelGrid))
                                    Del();
                                dch.printAfterCursor(printerName, true);
                            }
                            else if (printPageRange)
                            {
                                //2012-12-24 职工去除底纹
                                if (ThisAddIn.CanOption(ElementNames.DelGrid))
                                    Del();
                                dch.printByPageRange(printerName, iPageStart, iPageEnd);
                            }
                            else if (printAll)
                            {
                                ProtectDoc();
                                //2012-12-24 职工去除底纹
                                if (ThisAddIn.CanOption(ElementNames.DelGrid))
                                    Del();
                                dch.printAll(printerName);
                            }
                            else
                            {    //2012-12-24 职工去除底纹
                                if (ThisAddIn.CanOption(ElementNames.DelGrid))
                                    Del();
                                dch.printAfterCursor(printerName, false);
                            }
                            return;
                        }
                        if (!self1)
                        {
                            //默认打印位置
                            if (!printEnd1 && !self1)
                            {
                                //SetLastTop(groupCode, 0);老续打
                                SetLastPageNumber(groupCode, 0);
                                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                                {
                                    if (groupCode == "1")
                                        es.DeletRange(GetRegistryID());
                                    if (groupCode == "2")
                                        es.DeletRangex1(GetRegistryID());
                                    if (groupCode == "3")
                                        es.DeletRangex2(GetRegistryID());
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!printEnd1)
                        {
                            SetLastPageNumber(groupCode, 0);
                            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
                            {
                                es.DeletRange(GetRegistryID());
                            }
                        }
                        if (cancel)
                            return;
                        //正常打印 第一页第一行
                        if (no)
                        {
                            return;
                        }
                    }
                    // string isSingle = "no";
                    if (Globals.emrPattern.GetEmrNote(currentNote.noteInfo.GetNoteID()).Attributes[AttributeNames.SingleContinue] != null)
                        isSingle = Globals.emrPattern.GetEmrNote(currentNote.noteInfo.GetNoteID()).Attributes[AttributeNames.SingleContinue].Value.ToString();
                    int cjjLastPageNumber = GetLastPageNumber(groupCode);
                    long cjjLastTop = GetLastTop(groupCode);
                    if (self1)
                    {
                        cjjLastPageNumber = Convert.ToInt32(page1);
                    }
                    string endPrintNoteName = group.Attributes[AttributeNames.NoteName].Value;
                    string endPrintNoteID = group.Attributes[AttributeNames.NoteID].Value;

                    Word.Pane pane = wordApp.ActiveWindow.Panes[1];
                    bool IsRP = false;
                    if (self1)
                    {
                        RePrintEndSelf(pane, Convert.ToInt32(space1), printEnd1, groupCode, isSingle);
                        if (Convert.ToInt32(space1) > 1)
                        {
                            IsRP = true;
                        }
                        else
                        {
                            IsRP = false;
                        }
                    }
                    else
                    {
                        RePrintEnd(ref cjjLastPageNumber, ref IsRP,
                           pageHeader, endPrintNoteName, endPrintNoteID, pane, pageHeaderFont, groupCode, isSingle);
                    }
                    wordApp.ActiveWindow.View.Zoom.Percentage = 100;

                    bool firstPage = false;
                    if (!IsRP) firstPage = true;
                    bool selfPage = false;
                    if (space1 != "")
                    {
                        if (Convert.ToInt32(space1) == 1)
                        {
                            firstPage = true;
                        }
                        selfPage = true;
                    }
                    SetFooterForEndPrint(endPrintNoteName, cjjLastPageNumber, firstPage);
                    SetLastPageNumber(groupCode, cjjLastPageNumber + pane.Pages.Count - 1);
                    myDoc.Saved = true;
                }
            }
            /** 去底纹**/
            if (ThisAddIn.CanOption(ElementNames.DelGrid))
                Del();


            ProtectDoc();
            object objMissing = System.Reflection.Missing.Value;
            int printDialogResult = 0;
            PrintSetup ps = new PrintSetup(wordApp);
            ps.ShowDialog();
            if (printDialogResult == 1)
            {
                //myDoc.PrintOut(ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing);
                myDoc.PrintOut(ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing);
            }
        }
        private void OldDocPrint()
        {

        }
        private void cbxArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            emrDocuments = new EmrDocuments();
            string[] codename = cbxArea.Items[cbxArea.SelectedIndex].ToString().Split(' ');
            ThisAddIn.xmlPatientWriter(codename[0], tvPatients, 2, Globals.patientFile);
            Globals.selectIndex = cbxArea.SelectedIndex;
        }
        private void buttonX1_Click_1(object sender, EventArgs e)
        {
            int count = myDoc.InlineShapes.Count;
            Word.InlineShape sr = myDoc.InlineShapes[1];
            if (sr.Type == Word.WdInlineShapeType.wdInlineShapePicture || sr.Type == Word.WdInlineShapeType.wdInlineShapeLinkedPicture)
            {
                sr.Select();
                wordApp.Selection.Copy();
            }
            else return;

            if (Clipboard.ContainsImage())
            {
                Bitmap bmp = new Bitmap(Clipboard.GetImage());
                Clipboard.Clear();
                Painter p = new Painter(bmp, true);
                if (p.ShowDialog() == DialogResult.OK)
                {
                    string picName = Globals.workFolder + "\\MyTemplateFile.bmp";
                    object linkToFile = false;
                    object saveWithDocument = true;
                    p.FinalBitmap.Save(picName);
                    object start = sr.Range.Start;
                    object oMissing = Type.Missing;
                    sr.Delete();
                    Word.Range range = myDoc.Range(ref start, ref start);
                    object orange = (object)range;
                    range.InlineShapes.AddPicture(picName, ref linkToFile, ref saveWithDocument, ref orange);
                }
                p.Dispose();
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            DocPrint_Click(sender, e);
        }
        private void btnConvert_Click(object sender, EventArgs e)
        {
            FlashFolder = false;
            saveFileDialog.FilterIndex = 2;
            SaveAsFile(sender, e);
        }
        RunningLoadFlag loading;
        string thisText;
        bool FlashFolder = false;
        private void SaveAsFile(object sender, EventArgs e)
        {
            if ((!FlashFolder && saveFileDialog.ShowDialog(this) == DialogResult.OK) || (FlashFolder && folderBrowserDialog.ShowDialog(this) == DialogResult.OK))
            {
                this.Cursor = Cursors.WaitCursor;
                thisText = this.Text;
                this.Text += " - 正在转换...";

                loading = new RunningLoadFlag();
                loading.SuspendLayout();
                loading.StartPosition = FormStartPosition.CenterScreen;
                loading.Size = new Size(123, 123);
                loading.OpacityTo = 0.8d;
                loading.EnterOpacitySecend = 2;
                loading.ExitOpacitySecend = 2;
                loading.InnerCircleRadius = 15;
                loading.OutnerCircleRadius = 40;
                loading.SpokesMember = 15;
                loading.ResumeLayout(false);
                loading.PerformLayout();
                loading.StartMarquee();

                Thread tConvert = new Thread(DocConvert);
                tConvert.Start();

                loading.ShowDialog(this);
            }
        }
        private void DocConvert()
        {
            try
            {
                WordConvert wordConvert = new WordConvert(wordApp, saveFileDialog.FileName);
                if (FlashFolder)
                {
                    wordConvert = new WordConvert(wordApp, Application.StartupPath + "\\temp.pdf");
                    wordConvert.ConvertWord(WordConvert.FileType.PDF, false);
                    PDF2Flash pdf2flash = new PDF2Flash(Application.StartupPath + "\\temp.pdf", null, folderBrowserDialog.SelectedPath);
                    string[] fileList = new string[] { Application.StartupPath + "\\PDF2SWF\\main.swf", Application.StartupPath + "\\PDF2SWF\\index.html" };
                    pdf2flash.ConvertPDF(System.Windows.Forms.Application.StartupPath + "\\PDF2SWF\\logo.jpg", null, null, "EMR2012", "Jinwei Shoujia", fileList, true);
                }
                else
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1://Word
                            object objMissing = System.Reflection.Missing.Value;
                            object objOutDoc = saveFileDialog.FileName;
                            myDoc.SaveAs(ref objOutDoc, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing);
                            break;
                        case 2://PDF
                            wordConvert.ConvertWord(WordConvert.FileType.PDF, false);
                            break;
                        case 3://XPS
                            wordConvert.ConvertWord(WordConvert.FileType.XPS, false);
                            break;
                        case 4://FlashPacket
                            wordConvert = new WordConvert(wordApp, Application.StartupPath + "\\temp.pdf");
                            wordConvert.ConvertWord(WordConvert.FileType.PDF, false);
                            PDF2Flash pdf2flash = new PDF2Flash(Application.StartupPath + "\\temp.pdf", null, FileMethod.GetFilePath(saveFileDialog.FileName), FileMethod.GetFileName(saveFileDialog.FileName));
                            string[] fileList = new string[] { Application.StartupPath + "\\PDF2SWF\\main.swf", Application.StartupPath + "\\PDF2SWF\\index.html" };
                            pdf2flash.ConvertPDF(System.Windows.Forms.Application.StartupPath + "\\PDF2SWF\\logo.jpg", null, null, "EMR2012", "Jinwei Shoujia", fileList, true);
                            break;
                    }
            }
            catch (COMException)
            {
                if (MessageBox.Show(this, "检测到系统中未安装必要的Word加载项，是否现在安装？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = Application.StartupPath + "\\SaveAsPDFandXPS.exe";
                    p.Start();
                    p.WaitForExit();
                    if (MessageBox.Show(this, "是否重新启动主程序以启用该功能？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                if (MessageBox.Show(this, "检测到系统中未注册必要的组件，是否现在进行注册？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = Application.StartupPath + "\\PDF2SWF\\Register.exe";
                    p.Start();
                    p.WaitForExit();
                    MessageBox.Show(this, "若提示注册成功，您现在可以重新进行上一操作了。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Clipboard.SetText(ex.Message);
                }
                catch { }
                MessageBox.Show(this, "出现未知错误，错误信息已复制到剪贴板，请联系系统管理员。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }

            this.Text = thisText;
            this.Cursor = Cursors.Default;
            loading.StopClose();
        }
        private void btnPDF_Click(object sender, EventArgs e)
        {
            FlashFolder = false;
            saveFileDialog.FilterIndex = 2;
            SaveAsFile(sender, e);
        }
        private void btnXPS_Click(object sender, EventArgs e)
        {
            FlashFolder = false;
            saveFileDialog.FilterIndex = 3;
            SaveAsFile(sender, e);
        }
        private void btnFlashPacket_Click(object sender, EventArgs e)
        {
            FlashFolder = false;
            saveFileDialog.FilterIndex = 4;
            SaveAsFile(sender, e);
        }
        private void btnFlash_Click(object sender, EventArgs e)
        {
            FlashFolder = true;
            SaveAsFile(sender, e);
        }
        private void btnPrintPic_Click(object sender, EventArgs e)
        {
            Word.Paragraphs items = wordApp.Selection.Paragraphs;
            foreach (Word.Paragraph item in items)
            {
                if (item != null && item.Range.InlineShapes.Count != 0)
                {
                    foreach (Word.InlineShape shape in item.Range.InlineShapes)
                    {
                        //判断类型
                        if (shape.Type == Word.WdInlineShapeType.wdInlineShapePicture || shape.Type == Word.WdInlineShapeType.wdInlineShapeLinkedPicture)
                        {

                            //利用剪贴板保存数据
                            shape.Select(); //选定当前图片
                            wordApp.Selection.Copy();//copy当前图片
                            ThisShape = shape;
                            this.Invoke(new EventHandler(EditImage));
                            break;
                        }
                    }
                }
            }
        }
        private void Marge2PDF_Click(object sender, EventArgs e)
        {
            MergeAsWord(sender, e);
            FlashFolder = false;
            saveFileDialog.FilterIndex = 2;
            SaveAsFile(sender, e);
            CloseDoc_General();
        }
        private void Marge2XPS_Click(object sender, EventArgs e)
        {
            MergeAsWord(sender, e);
            FlashFolder = false;
            saveFileDialog.FilterIndex = 3;
            SaveAsFile(sender, e);
            CloseDoc_General();
        }
        private void Marge2FlashFolder_Click(object sender, EventArgs e)
        {
            MergeAsWord(sender, e);
            FlashFolder = true;
            SaveAsFile(sender, e);
            CloseDoc_General();
        }
        private void Marge2FlashPacket_Click(object sender, EventArgs e)
        {
            MergeAsWord(sender, e);
            FlashFolder = false;
            saveFileDialog.FilterIndex = 4;
            SaveAsFile(sender, e);
            CloseDoc_General();
        }
        private void MargeWord_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (tvPatients.SelectedNode.Level != 2) return;
            int index = Convert.ToInt32(tvPatients.SelectedNode.Parent.Tag);
            string strNodeID = tvPatients.SelectedNode.Name;

            XmlNode NodePattern = Globals.emrPattern.GetEmrNote(strNodeID);
            if (NodePattern == null) return;
            if (NodePattern.Attributes["Merge"] == null || NodePattern.Attributes["Merge"].Value == "0")
            {
                MessageBox.Show(ErrorMessage.Nomarge);
                return;
            }
            XmlDocument doc = emrDocuments.Get(index);
            Globals.Mdoc = doc;
            XmlNodeList emrNotes = doc.DocumentElement.SelectNodes(ElementNames.EmrNote);

            mns = new MergeNotes(emrNotes, strNodeID);

            if (mns.IsDisposed) return;
            if (mns.ShowDialog() == DialogResult.Cancel) return;
            //PatientInfo pi = GetPatientInfo(tvPatients.SelectedNode);

            LoadWord(MergeDoc);
        }
        private void MergeDoc()
        {
            TotalRecordsNumber = mns.GetNotes().Nodes.Count;
            singlePageNoteID.Clear();
            string registryID = GetRegistryID();

            Globals.Mmarge = true;
            bool up = false;
            XmlElement mergeNotes = Globals.Mdoc.CreateElement(ElementNames.EmrNotes);
            AuthorInfo author = MakeAuthorInfo(tvPatients.SelectedNode.Name, tvPatients.SelectedNode.Text);
            if (mns.GetNotes().Nodes.Count > 0)
            {
                foreach (TreeNode noteNode in mns.GetNotes().Nodes)
                {
                    if (!noteNode.Checked) continue;
                    CheckedNumber++;
                }
                foreach (TreeNode noteNode in mns.GetNotes().Nodes)
                {

                    TotalRecordsNumber--;
                    if (!noteNode.Checked) continue;
                    // CheckedNumber++;
                    int series = Convert.ToInt32(noteNode.Tag);
                    string noteID = noteNode.Name;
                    string wdNote = udt.MakeWdDocumentFileName(GetRegistryID(), noteID, series, Globals.workFolder);

                    string IsSingle = "no";
                    if (Globals.emrPattern.GetEmrNote(noteID).Attributes[AttributeNames.SingleContinue] != null)
                        IsSingle = Globals.emrPattern.GetEmrNote(noteID).Attributes[AttributeNames.SingleContinue].Value.ToString();

                    //if (!= null)
                    //{
                    string tmpfile = Path.Combine(Globals.workFolder, "病程合并.docx");
                    Globals.MIsSingle = IsSingle;
                    Globals.MnoteID = noteID;
                    Globals.Mnode = noteNode;
                    plContain.Visible = true;
                    OperationEnableForStat(true);
                    ShowMenu(MenuStatus.Writer);
                    OpenWordDoc(wdNote);

                    break;
                    //}
                }
                //2012-03-19  

            }
            CloseLoad();
        }
        private void saveTemplate_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            NoteTemplate.UpdateTemplate(wordApp);
            OpDone opDone = new OpDone("寄存成功"); //20120313模版保存
            opDone.Show(); //20120313模版保存
            ThisAddIn.ResetBeginTime();//20120313模版保存

        }
        private void btnEquation_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            using (frmEquation frmE = new frmEquation())
            {
                if (frmE.ShowDialog(this) == DialogResult.OK)
                {
                    myDoc.Application.Selection.InlineShapes.AddPicture(Application.StartupPath + "\\Equation.jpg");
                }
            }
            this.Cursor = Cursors.Default;
        }
        //知识库
        private void OpenKnowledge_Click(object sender, EventArgs e)
        {
            string strHelpFilePath = Application.StartupPath + "\\KnowledgeBase.chm";
            if (File.Exists(strHelpFilePath))
            {
                Help.ShowHelp(this, strHelpFilePath);
            }

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show(this, "你真的要退出吗？", "提示信息：", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                e.Cancel = false;
                if (wordApp != null)
                    try
                    {
                        Word.Document myDoc = ActiveDocumentManager.getDefaultAD(); //2012-10-18 关闭当前病历
                        myDoc.Activate();
                        axWbDoCView.Navigate("about:blank");
                        //myDoc.Save();
                        wordApp = null;
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        Globals.logAdapter.Record("EX9225145659826", ex.Message + ">>" + ex.ToString(), true);
                        MessageBox.Show("电子病历系统退出！");
                    }
                //axWbDoCView.Navigate("about:blank");
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }

        }

        private void asPersonTemplate2_Click(object sender, EventArgs e)
        {
            asPersonTemplate_Click(sender, e);
        }
        private void asDepartTemplate2_Click(object sender, EventArgs e)
        {
            asDepartTemplate_Click(sender, e);
        }
        private void asHospitalTemplate2_Click(object sender, EventArgs e)
        {
            asHospitalTemplate_Click(sender, e);
        }

        private void HelpManual_Click(object sender, EventArgs e)
        {
            string strHelpFilePath = Application.StartupPath + "\\help.chm";
            if (File.Exists(strHelpFilePath))
            {
                Help.ShowHelp(this, strHelpFilePath);
            }
        }
        private void Paste_Click(object sender, EventArgs e)
        {
            Word.Selection selection = wordApp.Selection;
            if (selection != null)
            {
                try
                {
                    wordApp.Selection.Paste();
                }
                catch (Exception ex) { }
            }

        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Word.Selection selection = wordApp.Selection;
            if (selection != null)
            {
                try
                {
                    wordApp.Selection.Copy();
                }
                catch (Exception ex) { }
            }
            //Win32.keybd_event(17, 0, 0, 0);
            //Win32.keybd_event(67, 0, 0, 0);
            //Win32.keybd_event(17, 0, 2, 0);
            //Win32.keybd_event(67, 0, 2, 0);
        }
        private void Shear__Click(object sender, EventArgs e)
        {
            Word.Selection selection = wordApp.Selection;
            if (selection != null)
            {
                try
                {
                    wordApp.Selection.Cut();
                }
                catch (Exception ex) { }
            }

        }

        private void contextMenuStrip1_MouseLeave(object sender, EventArgs e)
        {
            // hideMenu();
        }

        private void mstFont_Click(object sender, EventArgs e)
        {
            Fonts_Click(sender, e);
        }
        private void mstGS_Click(object sender, EventArgs e)
        {
            btnEquation_Click(sender, e);
        }

        private void mstTable_Click(object sender, EventArgs e)
        {
            TabelSet_Click(sender, e);
        }

        private void mstFH_Click(object sender, EventArgs e)
        {
            barSign_Click(sender, e);
        }

        private void tvPatients_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        //质检评分
        private void btnNoteValuate_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvPatients.SelectedNode;

            if (ThisAddIn.CanOption(ElementNames.QualityScore))
            {
                ThisAddIn.ResetBeginTime();
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Valuate)) return;
                if (tn == null)
                {
                    MessageBox.Show("请选择要质检的患者进行评分！！");
                    return;
                }
                //if (tn.Level == 1)
                
                if (tn != null && tn.Level == 1 && tn.Nodes.Count!=0)
                {
                    PatientInfo patientInfo = GetPatientInfo(tvPatients.SelectedNode);
                    string depName = "";
                    if (patientInfo.DischargedDate==null||patientInfo.DischargedDate.Trim() == "")
                    {
                        MessageBox.Show("只有出科病人才能评分！");
                        return;
                    }
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        depName = ep.GetDepartmentName(patientInfo.DepartmentCode.ToString());
                    }
                    string doctorName = Globals.doctors.GetDoctorName(tvPatients.SelectedNode.Text.Split(':')[2]);
                    ValuateNow vn = new ValuateNow(this);
                    vn.InitControls(patientInfo, depName, doctorName, ThisAddIn.Today());
                    vn.ShowDialog(this);
                }
                return;
            }
            else
            {
                MessageBox.Show("未开启此功能，请登录质管端开通！！！");
            }
        }
        private void btnPfQxTj_Click(object sender, EventArgs e)
        {
            if (ThisAddIn.CanOption(ElementNames.QualityScore))
            {
                ThisAddIn.ResetBeginTime();
                if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Valuate)) return;
                patient1.Visible = false;
                select = new CSelectP(this);
                select.ShowInTaskbar = false;
                select.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("未开启此功能，请登录质管端开通！！！");
            }

        }

        private void noteClose_Click(object sender, EventArgs e)
        {
            _ExitDoc();
        }

        private void TBDocPrint_Click(object sender, EventArgs e)
        {
            if (wordApp == null) return;
            if (!plContain.Visible) return;
            axWbDoCView.Focus();
            //无痕打印
            if (ThisAddIn.CanOption(ElementNames.PrintNoView))
            {
                wordApp.ActiveWindow.View.ShowRevisionsAndComments = false;
                wordApp.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
            }
            /** 去底纹**/
            if (ThisAddIn.CanOption(ElementNames.DelGrid))
                Del();

            ProtectDoc();
            object objMissing = System.Reflection.Missing.Value;
            int printDialogResult = 0;
            PrintSetup ps = new PrintSetup(wordApp);
            ps.ShowDialog();
            if (printDialogResult == 1)
            {
                //myDoc.PrintOut(ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing, ref objMissing);
                myDoc.PrintOut(ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing,
                    ref objMissing);
            }

        }

        private void OnQCTimeOutDepart_Click(object sender, EventArgs e)
        {
            ThisAddIn.ResetBeginTime();
            if (!ThisAddIn.CanExeutiveCommand(MyMenuItems.Valuate)) return;
            Qualityks qk = new Qualityks();
            qk.Show();
            // MonitorTimeOutDepart();
            ThisAddIn.ResetBeginTime();
        }


        private void 院感系统上报ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string doctorid = "";
            string archiveNum = "";
            EmrConstant.PatientInfo pi = GetPatientInfo(tvPatients.SelectedNode);
            archiveNum = pi.ArchiveNum;
            string departId = pi.DepartmentCode;
            if (tvPatients.SelectedNode.Level == 0)
            {
                doctorid = tvPatients.SelectedNode.Nodes[0].Text.Split(':')[2];
            }
            else if (tvPatients.SelectedNode.Level == 1)
            {
                doctorid = tvPatients.SelectedNode.Text.Split(':')[2];
            }
            else { return; }
            System.Diagnostics.Process.Start("iexplore.exe", "http://192.0.10.41:8080/web_new/docproject/tables.php?pathosid=" + archiveNum + "&docid=" +doctorid+ "&distno=" +departId);

        }

        private void 院感日志录入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmrConstant.PatientInfo pi = GetPatientInfo(tvPatients.SelectedNode);
           
            string departId = pi.DepartmentCode;
            System.Diagnostics.Process.Start("iexplore.exe", "http://192.0.10.41:8080/web_new/index_log.php?id=18&name=院感日志录入&userId=100" + "&dict=" + departId);

   
        }



    }
}

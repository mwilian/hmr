using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using CommonLib;
using EmrConstant;
using System.IO;
using System.Xml;
using System.Threading;
using HuronControl;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net;

namespace EMR
{
    public partial class Welcome : HuronControl.RoundcornerForm
    {
        Thread tLoadData;
        Thread tShowStatus;
        Process pUpdate = new Process();
        INIAdapter ini = new INIAdapter(Application.StartupPath + "\\sjsys.ini");

        public Welcome(bool CheckUpdate)
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
                            MessageBox.Show("其他程序影响电子病历正常使用须关闭!", "提示");
                            p.Kill();
                            break;
                        }
                    }
                }
                catch { }
            }
            InitializeComponent();
            //开始日志
            udt.jj.LoadlogAdapter();

            if (ini.ReadValue("System", "CheckClient").Trim() == true.ToString())
                CheckClient();

            try
            {
                tLoadData = new Thread(LoadData);
                tShowStatus = new Thread(ShowStatus);
                if (ini.ReadValue("System", "LiveUpdate").Trim() == true.ToString())
                {
                    try
                    {
                        pUpdate.StartInfo.FileName = Application.StartupPath + "\\liveupdate.exe";
                        pUpdate.StartInfo.Arguments = "-liveupdate[" + FileMethod.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "]";
                        pUpdate.Start();
                    }
                    catch (Win32Exception)
                    {
                        ini.WriteValue("System", "LiveUpdate", false.ToString());
                        MessageBox.Show(this, "启动自动更新程序失败，自动更新功能已关闭，若要重新开启此功能请联系系统管理员或重新安装。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    }
                }
                else { }

                ThisAddIn.logon = new Logon();
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX756987457748", ex.Message + ">>" + ex.ToString(), true);            
             
                MessageBox.Show(ex.ToString());
            }
        }

        private SqlConnection Connect(string DataBase)
        {
            INIAdapter iniAdt = new INIAdapter(Application.StartupPath + "\\sjsys.ini");
            SqlConnection conn;
            if (!iniAdt.Exists()) throw (iniAdt.INIException);
            else
            {
                string DataSource, InitialCatalog, UserID, Password;
                DataSource = iniAdt.ReadValue(DataBase, "DataSource");
                InitialCatalog = iniAdt.ReadValue(DataBase, "InitialCatalog");
                UserID = iniAdt.ReadValue(DataBase, "UserID");
                Password = iniAdt.ReadValue(DataBase, "Password");

                StringEncrypt.DecodeString(Password, out Password);

                conn = new SqlConnection();
                conn.ConnectionString = @"Data Source=" + DataSource + ";Initial Catalog=" + InitialCatalog + ";User ID=" + UserID + ";Password=" + Password;
            }
            return conn;
        }

        //检测是否是合法客户端
        private void CheckClient()
        {
            using (SqlConnection conn = Connect("Permission"))
            {
                conn.Open();

                string SubSystemID = ini.ReadValue("Permission", "SubSystemID");
                string ClientIP = NetAdapter.GetLocalIP();

                using (SqlDataAdapter adtPermission = new SqlDataAdapter("select * from SJ_ZXTYHMX where ZXTDM='" + SubSystemID + "' and IP='" + ClientIP + "'", conn))
                {
                    DataTable dtPermission = new DataTable();
                    adtPermission.Fill(dtPermission);
                    conn.Close();
                    if (dtPermission.Rows.Count != 1)
                    {
                        MessageBox.Show(this, "客户端未被授权，请联系系统管理员。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                    else { }//已被授权使用
                }
            }

            //// 加密字符串
            //string as_src = "";
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg);

            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x01;

            if (msg.Msg == WM_NCHITTEST)
                if (HTCLIENT == msg.Result.ToInt32())
                {
                    Point p = new Point();
                    p.X = (msg.LParam.ToInt32() & 0xFFFF);
                    p.Y = (msg.LParam.ToInt32() >> 16);

                    p = PointToClient(p);

                    msg.Result = new IntPtr(2);
                }
        }

        protected override void OnShown(EventArgs e)
        {
            try
            {
                tShowStatus.Start();
                loadingCircle.StartMarquee();
                tLoadData.Start();
                base.OnShown(e);
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX756987457778", ex.Message + ">>" + ex.ToString(), true);            
             
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowStatus()
        {
            if (ini.ReadValue("System", "LiveUpdate").Trim() == true.ToString())
            {
                lblStatus.Text = "正在检查更新...";
                try
                {
                    pUpdate.WaitForExit();
                }
                catch { }
            }
            else { }
            lblStatus.Text = "正在载入数据...";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString("Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(), new Font("宋体", 9), Brushes.Black, 30, 325);
            base.OnPaint(e);
        }

        private void LoadData()
        {
            try
            {
                if (!Directory.Exists(Globals.workFolder))
                {
                    Directory.CreateDirectory(Globals.workFolder);

                    File.SetAttributes(Globals.workFolder, FileAttributes.Hidden);

                }
                if (!Directory.Exists(Globals.templateFolder))
                {
                    Directory.CreateDirectory(Globals.templateFolder);
                }

                #region Check if offline
                string serverName="";
                try
                {
                    serverName = ThisAddIn.CheckOffline();
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX756987457750", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                if (Globals.offline)
                {
                    MessageBox.Show(serverName + ErrorMessage.OfflineAgain, ErrorMessage.Warning);
                }

                #endregion

                //#region Get current version number
                //string manifest = Path.Combine(Globals.currentDirectory, "EMR.vshost.exe.config");
                //using (XmlReader reader = XmlReader.Create(manifest))
                //{
                //    reader.ReadToFollowing("assemblyIdentity");
                //    Globals.currentVersion = reader.GetAttribute("version");
                //    reader.Close();
                //}
                //#endregion

                //Globals.currentVersion = Application.ProductVersion;

                //#region Is there new version
                //if (!Globals.offline)
                //{
                //    CheckVersion cv = new CheckVersion();
                //    if (cv.IsThereHigherVersion(Globals.currentVersion))
                //    {
                //        if (MessageBox.Show("有 S2012 的更新版本发布，下载吗？\n\n下载完毕，自动退出程序；\n需要重新启动Word 2007!",
                //            EmrConstant.ErrorMessage.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                //            MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                //        {
                //            if (!Directory.Exists(Globals.newVersionFolder)) Directory.CreateDirectory(Globals.newVersionFolder);
                //            string msg = cv.DownloadNewVersion(Globals.newVersionFolder);
                //            if (msg != null)
                //            {
                //                if (msg != EmrConstant.ErrorMessage.NoNewVersion)
                //                    MessageBox.Show(msg, EmrConstant.ErrorMessage.Warning);
                //            }
                //            else
                //            {
                //                /* Download success */
                //                #region Replace the old with the new
                //                string[] components;

                //                cv.GetComponentList(out components);

                //                for (int i = 0; i < components.Length; i++)
                //                {

                //                    string destinationFile = Path.Combine(Globals.currentDirectory, components[i]);

                //                    string oldFile = destinationFile + ".old";
                //                    string sourceFile = "";
                //                    sourceFile = Path.Combine(Globals.newVersionFolder, components[i]);
                //                    if (File.Exists(oldFile)) File.Delete(oldFile);
                //                    File.Move(destinationFile, oldFile);
                //                    File.Move(sourceFile, destinationFile);
                //                }
                //                #endregion
                //                // ExitWord(); 
                //            }
                //        }
                //    }
                //}
                //#endregion

                Globals.emrPatternFile = Path.Combine(Globals.currentDirectory, ResourceName.EmrPatternXml);
                Globals.ChildPatternFile = Path.Combine(Globals.currentDirectory, ResourceName.ChildPatternXml);

                #region Environment configuration
                string myConfigFile = Path.Combine(Globals.currentDirectory, ResourceName.MyConfigXml);
                string configString = Properties.Resources.ResourceManager.GetString("myconfig");
                Globals.myConfig = new MyConfig(myConfigFile, configString);
                string dpCode = Globals.myConfig.GetDepartmentCode();
                Globals.AreaID = Globals.myConfig.GetAreaCode();
                #endregion
                /* Local parameters */
                //GetLocalOptionValues();
                /* Global parameters */
                ThisAddIn.GetOptionValues();
                /* Create local storage */
                ThisAddIn.CreateWorkFolders(EmrConstant.CreateWorkFolderMode.Build);

                if (Globals.offline)
                {
                    Globals.emrPattern = new EmrPattern(Globals.emrPatternFile, null, ThisAddIn.PutPattern);
                    Globals.childPattern = new EmrPattern(Globals.ChildPatternFile, null, ThisAddIn.PutChildPattern);
                }
                else
                {
                    Globals.emrPattern = new EmrPattern(Globals.emrPatternFile, ThisAddIn.GetRules(), ThisAddIn.PutPattern);
                    Globals.childPattern = new EmrPattern(Globals.ChildPatternFile, null, ThisAddIn.PutChildPattern);
                }
                //InitTem();
                #region Get local machine name and ip address
                Globals.localMachineName = Environment.MachineName;
                System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(Globals.localMachineName);
                byte[] bytes = ips[0].GetAddressBytes();
                Globals.localMachineIP = bytes[0].ToString() + "." + bytes[1].ToString() + "." + bytes[2].ToString() + "." + bytes[3].ToString();
                #endregion

                #region Get hospital name
                if (Globals.offline)
                {
                    Globals.hospitalName = Globals.myConfig.GetHospitalName();
                }
                else
                {
                    using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
                    {
                        try
                        {
                            Globals.hospitalName = ep.GetHospitalName();
                            Globals.myConfig.SetHospitalName(Globals.hospitalName);
                        }
                        catch (Exception ex)
                        {
                            Globals.logAdapter.Record("EX756987457751", ex.Message + ">>" + ex.ToString(), true);            
             
                            Globals.hospitalName = " ";
                        }
                    }
                }
                #endregion

                lblStatus.Text = "正在进行初始化...";

                ThisAddIn.logon = new Logon();

                #region Use windows identity
                if (ThisAddIn.CanOption(ElementNames.UseDigitalSign))
                {
                    System.Security.Principal.WindowsIdentity user =
                        System.Security.Principal.WindowsIdentity.GetCurrent();
                    string[] items = user.Name.Split(EmrConstant.Delimiters.Slash);
                    string code = items[items.Length - 1];
                    Globals.tmpFolder = "C:\\" + code;
                    string passwd = null;
                    string userName = null;
                    using (gjtEmrPatients.emrPatients pi = new gjtEmrPatients.emrPatients())
                    {
                        pi.AuthenticChek(code, ref userName, ref passwd);
                    }
                    if (userName != null)
                    {
                        ThisAddIn.logon.InternalPassed(code, userName);
                        return;
                    }
                }
                #endregion

                ThisAddIn.InitRoles(configString);
                ThisAddIn.InitForNonQC();
                #region Init note fonts
                //SetFonts(configString);
                //GetLabelFont();
                //GetHeaderFont();
                //GetNoteNameFont();
                //GetContentFont();
                #endregion

                Globals.doctors = new Doctors(Globals.doctorsFile);
                Globals.departments = new Departments(Globals.departmentFile);

                Globals.icd10File = Path.Combine(Globals.templateFolder, Globals.icd10File);
                if (!Globals.offline)
                {
                    DataSet dsicd = ThisAddIn.GetIcd10();
                    if (dsicd != null) dsicd.WriteXml(Globals.icd10File);
                }
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX756987457752", ex.Message + ">>" + ex.ToString(), true);            
             
            }

            this.Close();
            this.Dispose();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

    }
}

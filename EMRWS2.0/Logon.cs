using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EmrConstant;
using CommonLib;
using System.Xml;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Com.Itrus.Cvm;
namespace EMR
{
    public partial class Logon : HuronControl.RoundcornerForm
    {
        private string passwd = "";
        private string departmentCode = null;
        private string departName = "";
        private ArrayList myfunctions = new ArrayList();
        public Logon()
        {
            InitializeComponent();
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

        private void IsUkey()
        {
            string strS = "";
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                strS = es.GetIsKeyUser(Globals.DoctorID);
            }
            if (ThisAddIn.CanOption(ElementNames.UKey) == true && strS.Trim() == "1")
            {
                this.cboCertNew.Visible = true;
                //this.label3.Visible = true;
                cboCertNew.Items.Clear();
                string strPath = Globals.currentDirectory;
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection filterCerts = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                String CN = "";
                foreach (X509Certificate2 x509 in filterCerts)
                {

                    CVM cvm = CVM.GetInstance();
                    string configFileName = strPath + @"cvm.xml";
                    cvm.config(configFileName);
                    CertificateStatus ret = cvm.VerifyCertificate(x509);
                    if (ret == CertificateStatus.VALID)
                    {
                        String subjectname = x509.Subject;
                        String[] names = subjectname.Split(',');
                        foreach (String name in names)
                        {
                            if (name.IndexOf("CN=") > 0)
                            {
                                cboCertNew.Visible = true;
                                int begin = name.IndexOf("CN=");
                                int len = name.Length;
                                CN = name.Substring(4, len - 4);
                                cboCertNew.Items.Add(CN);
                                //cboCertificate.Items.Add(CN);
                            }
                        }
                    }
                    else
                    {
                        cboCertNew.Visible = false;
                    }
                }
                if (CN.Equals("") || CN == null)
                {
                    // MessageBox.Show("没有有效的证书，请联系管理员！");
                }
                store.Close();
                //try  
                //{
                //    if (cboCertificate.Items.Count != 0)
                //    {
                //        cboCertificate.Text = cboCertificate.Items[0].ToString();
                //        cboCertNew.Text = cboCertificate.Items[0].ToString();
                //        cboCertNew.SelectedIndex = 0;
                //    }
                //    cboCertNew.Focus();
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }
        private void Passed()
        {

            if (ThisAddIn.CanOption(ElementNames.OperatorDepartment))
            {
                ThisAddIn.ChangeDepartment(departmentCode);
            }

          //  Globals.qualityInfo = Globals.myConfig.GetShowQCI(txtUserCode.Text);

            if (Globals.inStyle)
            {
           //     Globals.QualityInfo(txtUserCode.Text.Trim());

            }
           
        }




        private void txtUserCode_Leave(object sender, EventArgs e)
        {
            txtUserCode.Text = udt.NormalizeOpcode(txtUserCode.Text);
            lblUserName.Text = "";

            bool ret = Return.Failed;
            string userName = "";
            if (txtUserCode.Text.Length == 4)
            {
                ret = ThisAddIn.GetOperatorInf(txtUserCode.Text, ref userName, ref passwd, ref departmentCode, ref departName);
            }
            if (ret == Return.Successful)
            {
                lblUserName.Text = userName;
                lblDepartment.Text = departName;
                Globals.OpDepartName = departName;
                Globals.OpDepartID = departmentCode;
            }
            else
            {
                lblUserName.Text = "";
                lblDepartment.Text = "";
            }
        }

        private void LogonForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Globals.workFolder)) Directory.CreateDirectory(Globals.workFolder);
            File.SetAttributes(Globals.workFolder, FileAttributes.Hidden);
        }

        private void txtUserCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void tbPwd_KeyPress(object sender, KeyPressEventArgs e)
        
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
           
        }
        //LQ 复制病历
        public string GetOpName()
        {
            return lblUserName.Text;
        }
        public void InternalPassed(string code, string uname)
        {
            txtUserCode.Text = code;
            lblUserName.Text = uname;
            //Passed();
        }

        private void lbUserName_TextChanged(object sender, EventArgs e)
        {
            this.Invalidate(new Rectangle(lblUserName.Location, new Size(this.ClientSize.Width - lblUserName.Location.X, lblUserName.Height)), false);
            this.Update();
        }

        private void lblDepartment_TextChanged(object sender, EventArgs e)
        {
            this.Invalidate(new Rectangle(lblDepartment.Location, new Size(this.ClientSize.Width - lblDepartment.Location.X, lblDepartment.Height)), false);
            this.Update();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ThisAddIn.CanOption(ElementNames.UKey)) IsUkey();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "") return;
            if (ThisAddIn.CanOption(ElementNames.UKey))
            {
                IsUkey(); return;
            }
            if (lblUserName.Text.Trim() != "" && txtPassword.Text == passwd)
            {
                Globals.DoctorID = txtUserCode.Text;
                Globals.OpDepartID = departmentCode;
                Globals.DoctorName = lblUserName.Text;
                // Globals.AreaID = ThisAddIn.GetAreaID(departmentCode);
                XmlNode Config = null;
                string msg = ThisAddIn.GetConfig(ref Config);
                if (msg == null || msg == "")
                {
                    Globals.Config = Config;
                    RetrieveMyroles(txtUserCode.Text);
                    //ThisAddIn.RetrieveMyrolesHIS(txtUserCode.Text);
                    this.DialogResult = DialogResult.OK;
                    txtPassword.Text = "";
                    txtUserCode.Text = "";
                    lblUserName.Text = "";
                    lblDepartment.Text = "";
                    txtUserCode.Focus();
                    this.Hide();
                    if (Globals.WriteOff)
                    {
                        MainForm m = new MainForm();
                        m.Show();
                        Globals.WriteOff = false;
                    }
                    return;
                }
                MessageBox.Show(msg);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                MessageBox.Show("无效的用户名或密码！");
                txtUserCode.Text = "";
                txtPassword.Text = ""; 
                departmentCode = "";
                lblUserName.Text = "";
                departName = "";
            }
        }

        private void RetrieveMyroles(string opcode)
        {
            Globals.myfunctions.Clear();
            XmlNode roles = null;

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                string msg = es.GetRolesForOneOperator(opcode, ref roles);
                if (msg != null)
                {
                    MessageBox.Show(msg, ErrorMessage.Error);
                    return;
                }
                if (roles == null) return;

            }
            for (int k = 0; k < roles.ChildNodes.Count; k++)
            {

                string[] functios = GetFunctions(roles.ChildNodes[k].InnerText);
                for (int m = 0; m < functios.Length; m++) Globals.myfunctions.Add(functios[m]);
            }

        }
        public string[] GetFunctions(string roleid)
        {
            XmlNode roles = GetRoles();
            foreach (XmlNode role in roles.ChildNodes)
            {
                if (role.Attributes[AttributeNames.RoleID].Value == roleid)
                {
                    string[] functions = new string[role.ChildNodes.Count];
                    for (int k = 0; k < role.ChildNodes.Count; k++)
                    {
                        functions[k] = role.ChildNodes[k].InnerText;
                    }
                    return functions;
                }
            }
            return null;
        }
        public XmlNode GetRoles()
        {
            XmlNode roles = Globals.Config.SelectSingleNode(ElementNames.Roles);
            return roles;
        }

        private void Logon_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.DrawString(lblUserName.Text, new Font("微软雅黑", 12), new SolidBrush(Color.Black), lblUserName.Location);
            e.Graphics.DrawString(lblDepartment.Text, new Font("微软雅黑", 12), new SolidBrush(Color.Black), lblDepartment.Location);
        }
    }
}
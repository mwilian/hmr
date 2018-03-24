using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using System.Security.Cryptography.X509Certificates;
using CommonLib;
using UserInterface;
using Com.Itrus.Svm;
using Com.Itrus.Cvm;


namespace EMR
{
    public partial class DocTransfer : Form
    {
        string registryID = "";
        public DocTransfer(string regID,string name)
        {
            InitializeComponent();
            label4.Text = name;
           
            dgvDocInfo.Height = 23;
            registryID = regID;
            label2.Text = registryID;
                GetDocInfoFinal(registryID);
               GetDocInfo(registryID);
        }
        private void GetDocInfoFinal(string registryID)
        {
            DataSet dst = new DataSet();
            
                DataTable dtN = new DataTable();
                dtN.Columns.Add("ѡ��", typeof(bool));
                dtN.Columns.Add("��������", typeof(string));
                dtN.Columns.Add("����", typeof(int));
                //dtN.Columns.Add("��д��", typeof(string));
                dgvDocInfo.DataSource = dtN;
                return;

                using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
             {
                 dst = es.GetTransferInfo(registryID, "1", "1|1", true);
                 DataTable dt = dst.Tables[0];
                 dt.Columns.Add("ѡ��", typeof(bool));
                 dt.Columns["ѡ��"].SetOrdinal(0);
                 dgvDocInfo.Height = 23 + dt.Rows.Count * 23;
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     dt.Rows[i]["ѡ��"] = true;
                 }


                 dt.Columns.Remove("סԺ��");
                 dt.Columns.Remove("�ύ��");
                 dt.Columns.Remove("������");
                 dt.Columns.Remove("����ʱ��");
                 //dt.Columns.Remove("��д��");
                 dgvDocInfo.DataSource = dt;
             }
        }
        private void GetDocInfo(string registryID)
        {
            DataSet dst = new DataSet();
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                
                    dst = es.GetTransferInfoExEx(registryID, Globals.DoctorID,false);
              
               
                DataTable dt = dst.Tables[0];
                dt.Columns.Add("ѡ��", typeof(bool));
                dt.Columns["ѡ��"].SetOrdinal(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["ѡ��"] = true;
                }

                
                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                    return;
                }
                string noteID = ThisAddIn.CanOptionText(ElementNames.ConsentID);
               XmlNode[] emrNotes = Globals.childPattern.GetChildNote(ThisAddIn.CanOptionText(ElementNames.ConsentID));
                 DataTable dtN = new DataTable();
                dtN.Columns.Add("ѡ��", typeof(bool));
              dtN.Columns.Add("��������", typeof(string));
                 dtN.Columns.Add("����", typeof(int));
                
             string departmentCode = Globals.myConfig.GetDepartmentCode();
                 foreach (XmlNode emrNote in emrNotes)
                 {
                     if (emrNote == null) break;
                     if (emrNote.Attributes[AttributeNames.BelongDepartment] != null&&emrNote.Attributes[AttributeNames.BelongDepartment].Value != departmentCode)
                         continue;
                     string noteName = emrNote.Attributes[AttributeNames.NoteName].Value;
                     if (!HaveID(dt, noteName))
                     {
                         DataRow drTemp = dtN.NewRow();
                         drTemp["ѡ��"] = false;
                         drTemp["��������"] = noteName;
                         drTemp["����"] = 1;
                       
                         dtN.Rows.Add(drTemp);
                     }
                 }
                 dataGridView1.DataSource = dtN;
                
            }
        }
        private bool HaveID(DataTable dt, string noteName)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["��������"].ToString() == noteName) return true;
            }
            return false;
        }

        private void cboCertNew_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboCertNew_MouseClick(object sender, MouseEventArgs e)
        {

            this.cboCertNew.Visible = true;
            this.label3.Visible = true;

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
                            int begin = name.IndexOf("CN=");
                            int len = name.Length;
                            CN = name.Substring(4, len - 4);

                            cboCertNew.Items.Add(CN);
                            cboCertificate.Items.Add(CN);
                        }
                    }

                }

            }
            if (CN.Equals("") || CN == null)
            {
                // MessageBox.Show("û����Ч��֤�飬����ϵ����Ա��");
            }
            store.Close();

            if (cboCertificate.Items.Count != 0)
            {
                cboCertificate.Text = cboCertificate.Items[0].ToString();
                cboCertNew.Text = cboCertificate.Items[0].ToString();
                cboCertNew.SelectedIndex = 0;
            }
            try
            {
                cboCertNew.Focus();
            }
            catch(Exception ex)
            {
                Globals.logAdapter.Record("EX925511256761", ex.Message + ">>" + ex.ToString(), true);            
               
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboCertificate.Text.Trim() == "")
            {
                MessageBox.Show("ȷ���ύ���Ѳ���u-Key��", ErrorMessage.Warning);
                return;
            }
            if (MessageBox.Show("ȷ�ϱ�������ĵ���", ErrorMessage.Warning,
            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            == DialogResult.Cancel) return;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[2].Value == null) continue;
                    if (!udt.jj.IsNumaric(row.Cells[2].Value.ToString()))
                    {
                        MessageBox.Show("�����ʽ����ȷ��");
                        return;
                    }
                    bool selected = (bool)row.Cells[0].FormattedValue;
                    if (!selected) continue;
                    int count = Convert.ToInt32(row.Cells[2].Value.ToString());
                    if (es.UpdateTransferInfo(registryID, row.Cells[1].Value.ToString(), row.Cells[3].Value.ToString(), Globals.DoctorID,
                        cboCertificate.Text.Trim(), ThisAddIn.Today(), count)==0)
                        es.PutTransferInfo(registryID, row.Cells[1].Value.ToString(), "", row.Cells[3].Value.ToString(), Globals.DoctorID, cboCertificate.Text.Trim(), ThisAddIn.Today(), count, true);
                }
            }
            OpDone op = new OpDone("���ճɹ�");
            op.Show();
            this.Close();
        

        }

        private void btnRefer_Click(object sender, EventArgs e)
        {
           

            if (MessageBox.Show("��ȷ���ύ�ĵ���", ErrorMessage.Warning,
            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            == DialogResult.Cancel) return;
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
               es.DelTransferInfo(registryID,Globals.DoctorID);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[2].Value == null) continue;
                    if (!udt.jj.IsNumaric(row.Cells[2].Value.ToString()))
                    {
                        MessageBox.Show("�����ʽ����ȷ��");
                        return;
                    }
                    bool selected = (bool)row.Cells[0].FormattedValue;
                    if (!selected) continue;
                    int count =Convert.ToInt32( row.Cells[2].Value.ToString());
                    es.PutTransferInfo(registryID, row.Cells[1].Value.ToString(), Globals.DoctorID,
                        Globals.DoctorName, "", "", ThisAddIn.Today(), count, false);
                }
            }
            OpDone op = new OpDone("�ύ�ɹ�");
            op.Show();
            this.Close();
        }

        private void DocTransfer_Load(object sender, EventArgs e)
        {
                 clsIme.SetIme(this);
          
                btnOK.Visible = false;
                btnRefer.Visible = true;
                cboCertNew.Visible = false;
           
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("��ȷ��\"����\"�������Ϊ���֣��������뷨״̬Ϊ��ǣ�");
        }

    }
}
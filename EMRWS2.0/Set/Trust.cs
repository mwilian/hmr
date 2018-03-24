using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CommonLib;

namespace EMR
{
    public partial class Trust : Form
    {
        private EmrConstant.Trust trusMode = EmrConstant.Trust.Deputed;
        private bool noAction = false;

        public Trust(EmrConstant.Trust mode)
        {
            InitializeComponent();

            trusMode = mode;
            if (mode == EmrConstant.Trust.Deputed) lbPrompt.Text = EmrConstant.TrustText.Deputed;
            else lbPrompt.Text = EmrConstant.TrustText.Deputing;

            LoadDoctorList();

            TimeSpan oneday = TimeSpan.FromDays(1);
            dtpEnd.Value = ThisAddIn.Today().Add(oneday);
        }
        private void LoadDoctorList()
        {
            using (XmlReader reader = XmlReader.Create(Globals.doctorsFile))
            {
                if (!reader.ReadToFollowing(EmrConstant.ElementNames.Doctor))
                {
                    reader.Close();
                    return;
                }
                lbxDoctors.Items.Clear();
                do
                {
                    if (reader.GetAttribute(EmrConstant.AttributeNames.Name).Trim().Length == 0) continue;
                    string text = reader.GetAttribute(EmrConstant.AttributeNames.Name).PadRight(10) + "|" +
                        reader.GetAttribute(EmrConstant.AttributeNames.Code) + "|" +
                        Upend(reader.GetAttribute(EmrConstant.AttributeNames.Spell));

                        lbxDoctors.Items.Add(text);
                } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Doctor));
                reader.Close();
            }

        }
        private void tbxDoctor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                SelectDoctor();
            }
            else
            {
                if (e.KeyChar < 'a' || e.KeyChar > 'z') return;
                string spell = e.KeyChar.ToString().ToUpper() + Upend(tbxDoctor.Text.TrimEnd().ToUpper());
                for (int i = 0; i < lbxDoctors.Items.Count; i++)
                {                   
                    if (lbxDoctors.Items[i].ToString().EndsWith(spell))
                    {
                        noAction = true;
                        lbxDoctors.SelectedItem = lbxDoctors.Items[i];
                        break;
                    }
                }
            }
        }
        private string Upend(string text)
        {
            string upend = null;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                upend += text.Substring(i,1);
            }
            return upend;
        }
        private void lbxDoctors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!noAction)
            {
                SelectDoctor();
                noAction = true;
            }
        }
        private void SelectDoctor()
        {
            if (lbxDoctors.SelectedItem == null) return;
            string[] nameCode = lbxDoctors.SelectedItem.ToString().Split('|');
            tbxDoctor.Text = nameCode[0];
            tbxDoctor.Tag = nameCode[1];
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbxDoctors_Click(object sender, EventArgs e)
        {
            noAction = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (tbxDoctor.Text.Trim() == "") return;
            if (tbxDoctor.Tag.ToString().Length != 4) return;           
            string deputed = Globals.DoctorID;
            string deputing = tbxDoctor.Tag.ToString();
            if (trusMode == EmrConstant.Trust.Deputing)
            {
                deputed = tbxDoctor.Tag.ToString();
                deputing = Globals.DoctorID;
            }
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    string msg = es.AddTrust(deputed, deputing, dtpEnd.Value, Globals.DoctorID);
                    if (msg == null)
                    {
                        if (trusMode == EmrConstant.Trust.Deputing)
                            MessageBox.Show(EmrConstant.TrustText.DeputingSucc, EmrConstant.ErrorMessage.Warning);
                        else
                            MessageBox.Show(EmrConstant.TrustText.DeputedSucc, EmrConstant.ErrorMessage.Warning);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(msg, EmrConstant.ErrorMessage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX925521256895", ex.Message + ">>" + ex.ToString(), true);            
               
                }
            }

        }

        private void Trust_SizeChanged(object sender, EventArgs e)
        {
            lbxDoctors.Height = this.Height - lbxDoctors.Top - 30;
        }
    }
}
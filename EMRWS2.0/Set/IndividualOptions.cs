using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EmrConstant;
using CommonLib;

namespace EMR
{
    public partial class IndividualOptions : UserControl
    {
        private string opcode = null;      
        public IndividualOptions()
        {
            InitializeComponent();
        }
        public void Initialize(string code, string[] noteIDs, string[] mergeNoteName,
            bool use16k, int optime, int pitime, int outtime)
        {
            opcode = code;           
            chxQualityInfo.Checked = Globals.myConfig.GetShowQCI(opcode);
           // numSpan.Value = (decimal)Globals.myConfig.GetShowTime(opcode, ElementNames.ShowOpdownTime, optime) / 100;
            numPiSpan.Value = Globals.myConfig.GetShowTime(opcode, ElementNames.ShowPatientInfoTime, pitime) / 100;
            numTimeout.Value = Globals.myConfig.GetShowTime(opcode, ElementNames.Timeout, outtime) / 60;
            chxExpand.Checked = Globals.myConfig.GetExpand(opcode);
            chxCanWrite.Checked = Globals.myConfig.GetCanWriteNoteInOffline();
            if (use16k)
            {
                numLeft.Enabled = false;
                numTop.Enabled = false;
                numRight.Enabled = false;
                numBottom.Enabled = false;

            }
            else
            {
                float left = 0, top = 0, right = 0, bottom = 0;
                Globals.myConfig.GetPageMargin(ref left, ref top, ref right, ref bottom);
                numLeft.Value = (decimal)left;
                numTop.Value = (decimal)top;
                numRight.Value = (decimal)right;
                numBottom.Value = (decimal)bottom;
            }
            cbxDefaultPrescriptUnit.SelectedIndex = Globals.myConfig.GetDrugUnitMode();
            chxMustBePoorPatient.Checked = Globals.myConfig.GetMustByPoorPatient();
            chxBeCarefulSeePatient.Checked = Globals.myConfig.GetBecarefulSeePatient(opcode);
            chxSyncCkeck.Checked = Globals.myConfig.GetAutoSyncCheck(opcode);
            chxOperatorDepartment.Checked = Globals.myConfig.GetOperatorDepartment();
            chxEnterCommitTime.Checked = Globals.myConfig.GetEnterCommitTime(opcode);
            numDayLimit.Value = (decimal)Globals.myConfig.GetLimitDays();
            #region Use card
            switch (Globals.myConfig.GetCardUseWay())
            {
                case CardUseWay.NoCard:
                    rbtRegistryID.Checked = true;
                    break;
                case CardUseWay.Required:
                    rbtReadCard.Checked = true;
                    break;
                case CardUseWay.OptionNoCard:
                    rbtDefaultRegistryID.Checked = true;
                    break;
                case CardUseWay.OptionCard:
                    rbtDefaultReadCard.Checked = true;
                    break;
            }
            #endregion

            if (noteIDs != null)
            {
                for (int i = 0; i < noteIDs.Length; i++)
                {
                    cbxFirstVisit.Items.Add(noteIDs[i]);
                    cbxReturnVisit.Items.Add(noteIDs[i]);
                }
                string noteID = Globals.myConfig.GetVisitNoteID(ElementNames.FirstVisitNoteID);
                for (int i = 0; i < noteIDs.Length; i++)
                {
                    if (noteID == noteIDs[i].Split(' ')[0])
                    {
                        cbxFirstVisit.SelectedIndex = i;
                        break;
                    }
                }
                noteID = Globals.myConfig.GetVisitNoteID(ElementNames.ReturnVisitNoteID);
                for (int i = 0; i < noteIDs.Length; i++)
                {
                    if (noteID == noteIDs[i].Split(' ')[0])
                    {
                        cbxReturnVisit.SelectedIndex = i;
                        break;
                    }
                }
            }
            if (mergeNoteName != null)
            {
                for (int k = 0; k < mergeNoteName.Length; k++)
                {
                    cbxGroup.Items.Add(mergeNoteName[k]);
                }
                int index = Convert.ToInt32(Globals.myConfig.GetMergeCode(opcode)) - 1;
                if (index < mergeNoteName.Length) cbxGroup.SelectedIndex = index;
            }

        }
        private void marginChanged()
        {
            Globals.myConfig.SetPageMargin((float)numLeft.Value, (float)numTop.Value,
                (float)numRight.Value, (float)numBottom.Value);
        }

        private void chxQualityInfo_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetShowQCI(opcode, chxQualityInfo.Checked);
        }

        //private void numSpan_ValueChanged(object sender, EventArgs e)
        //{
        //    Globals.myConfig.SetShowTime(opcode, ElementNames.ShowOpdownTime, (int)numSpan.Value * 100);
        //}

        private void numPiSpan_ValueChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetShowTime(opcode, ElementNames.ShowPatientInfoTime, (int)numPiSpan.Value * 100);
        }
        private void numTimeout_ValueChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetShowTime(opcode, ElementNames.Timeout, (int)numTimeout.Value * 60);
        }
        private void numLeft_ValueChanged(object sender, EventArgs e)
        {
            marginChanged();
        }

        private void numTop_ValueChanged(object sender, EventArgs e)
        {
            marginChanged();
        }

        private void numBottom_ValueChanged(object sender, EventArgs e)
        {
            marginChanged();
        }

        private void numRight_ValueChanged(object sender, EventArgs e)
        {
            marginChanged();
        }

        private void cbxExpand_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetExpand(opcode, chxExpand.Checked);
        }

        private void chxCanWrite_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetCanWriteNoteInOffline(chxCanWrite.Checked);
        }

        private void cbxDefaultPrescriptUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetDrugUnitMode(cbxDefaultPrescriptUnit.SelectedIndex);
        }

        private void chxMustBePoorPatient_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetMustByPoorPatient(chxMustBePoorPatient.Checked);
        }

        private void chxBeCarefulSeePatient_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetBecarefulSeePatient(opcode, chxBeCarefulSeePatient.Checked);
        }

        private void rbtReadCard_Click(object sender, EventArgs e)
        {
            Globals.myConfig.SetCardUseWay(CardUseWay.Required);
        }

        private void rbtRegistryID_Click(object sender, EventArgs e)
        {
            Globals.myConfig.SetCardUseWay(CardUseWay.NoCard);
        }

        private void rbtDefaultReadCard_Click(object sender, EventArgs e)
        {
            Globals.myConfig.SetCardUseWay(CardUseWay.OptionCard);
        }

        private void rbtDefaultRegistryID_Click(object sender, EventArgs e)
        {
            Globals.myConfig.SetCardUseWay(CardUseWay.OptionNoCard);
        }

        private void cbxFirstVisit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetVisitNoteID(ElementNames.FirstVisitNoteID, cbxFirstVisit.Text.Split(' ')[0]);
        }

        private void cbxReturnVisit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetVisitNoteID(ElementNames.ReturnVisitNoteID, cbxReturnVisit.Text.Split(' ')[0]);
        }

        private void cbxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int group = cbxGroup.SelectedIndex + 1;
            Globals.myConfig.SetMergeCode(opcode, group.ToString());
        }

        private void numDayLimit_ValueChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetLimitDays((double)numDayLimit.Value);
        }

        private void chxSyncCkeck_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetAutoSyncCheck(opcode, chxSyncCkeck.Checked);
        }

        private void chxOperatorDepartment_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetOperatorDepartment(chxOperatorDepartment.Checked);
        }

        private void chxEnterCommitTime_CheckedChanged(object sender, EventArgs e)
        {
            Globals.myConfig.SetEnterCommitTime(opcode, chxEnterCommitTime.Checked);
        }
    }
}
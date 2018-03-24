using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UserInterface
{
    public partial class Patient : UserControl
    {
        public Patient()
        {
            InitializeComponent();
        }
        public void InitControls(EmrConstant.PatientInfo pi, DateTime todays)
        {
            if (pi == null) return;
            lbArchiveNum.Text = pi.ArchiveNum;
            lbName.Text = pi.PatientName;
            lbGender.Text = pi.Sex;
            lbJob.Text = pi.Job;
            lbMarried.Text = pi.MaritalStatus;
            lbNation.Text = pi.Nation;
            lbRegistryDate.Text = pi.RegistryDate;
            lbOutRegistryDate.Text = pi.OutRegistryDate;
            lbRegistryID.Text = pi.RegistryID;
            lbOriginal.Text = pi.NativePlace;
            lbBirth.Text = pi.Birth;
            DateTime today = todays;
            //int age = today.Year - Convert.ToDateTime(pi.Birth).Year;
            //lbAge.Text = age.ToString()+"Ëê";
            lbAge.Text = pi.Age + "" + pi.AgeUnit;
            lbAddr.Text = pi.Address;
            lbBed.Text = pi.BedNum;
            if (pi.DischargedDate == null || pi.DischargedDate.Length == 0)
            {
                DateTime RegistryDate = Convert.ToDateTime(pi.RegistryDate);
                TimeSpan ts = today.Subtract(RegistryDate);
                lbDays.Text = ts.Days.ToString();

            }
            else
            {
                TimeSpan ts = Convert.ToDateTime(pi.DischargedDate).Subtract(Convert.ToDateTime(pi.RegistryDate));
                lbDays.Text = ts.Days.ToString();
            }

        }
        public void InitControls(EmrConstant.PatientInfo pi, string itemText, DateTime todays)
        {
            if (pi == null) return;

            lbArchiveNum.Text = pi.ArchiveNum;
            lbName.Text = pi.PatientName;
            lbGender.Text = pi.Sex;
            lbJob.Text = pi.Job;
            lbMarried.Text = pi.MaritalStatus;
            lbNation.Text = pi.Nation;
            lbRegistryDate.Text = pi.RegistryDate;
            lbOutRegistryDate.Text = pi.OutRegistryDate;
            lbRegistryID.Text = pi.RegistryID;
            lbOriginal.Text = pi.NativePlace;
            lbBirth.Text = pi.Birth;
            DateTime today = todays;
            //int age = today.Year - Convert.ToDateTime(pi.Birth).Year;
            //lbAge.Text = age.ToString();
            lbAge.Text = pi.Age + " " + pi.AgeUnit;
            lbAddr.Text = pi.Address;
            lbBed.Text = pi.BedNum;
            string[] items = itemText.Split(EmrConstant.Delimiters.Seperator);
            lbDoctor.Text = items[0];
            lbArchive.Text = items[1];
            lbRef.Text = items[2];

            if (pi.DischargedDate == null || pi.DischargedDate.Length == 0)
            {
                DateTime RegistryDate = Convert.ToDateTime(pi.RegistryDate);
                //DateTime RegistryDate = new DateTime();
                TimeSpan ts = today.Subtract(RegistryDate);
                lbDays.Text = ts.Days.ToString();

            }
            else
            {
                TimeSpan ts = Convert.ToDateTime(pi.DischargedDate).Subtract(Convert.ToDateTime(pi.RegistryDate));
                lbDays.Text = ts.Days.ToString();

            }
        }      
    }
}

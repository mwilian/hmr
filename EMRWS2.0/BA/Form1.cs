using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.IO;

namespace EMR.BA
{
    public partial class Form1 : Form
    {
        string registryID = "";
        public Form1(string RegistryID)
        {
            registryID = RegistryID;
            InitializeComponent();

        }
        private void LoadFlaws(string RegistryID)
        {
            this.reportViewer2.RefreshReport();
            DataSet ds1 = new DataSet();
            DataSet ds2 = new DataSet();
            DataSet ds3 = new DataSet();
            DataSet ds4 = new DataSet();
            DataSet ds5 = new DataSet();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {

                //ep.GetMedicalData(ref ds1, ref ds2, ref ds3, RegistryID);
                ep.GetNewMedicalData(ref ds1, ref ds2, ref ds3, ref ds4, ref ds5, RegistryID);
            }

            //ReportParameter[] rp = new ReportParameter[1];

            //rp[0] = new ReportParameter("Report_Parameter_0", "这里是参数");
            //ReportViewer2.Visible = false;
            //return;
            //ReportViewer2.LocalReport.DataSources.Clear();
            //ReportParameter[] rp = new ReportParameter[1];
            //rp[0] = new ReportParameter("strParas", strParas);
            this.reportViewer2.LocalReport.DataSources.Clear();
            //this.reportViewer2.Reset();


            //this.reportViewer2.LocalReport.ReportPath = @"E:\emr\WordAddInEmrw\WordAddInEmrw\order\Report1.rdlc";

            //if (ds4.Tables[0].Rows.Count > 0)
            //{
            //    DataRow dr = ds4.Tables[0].Rows[0];

            //}
            if (ds1.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds1.Tables[0].Rows[0];
                //
                DataRow dr4 = ds4.Tables[0].Rows[0];
                dr.Table.Columns.Add("yydm");
                dr.Table.Columns.Add("yymc");
                dr.Table.Columns.Add("sfgm");
                dr["yydm"] = dr4.ItemArray[0];
                dr["yymc"] = dr4.ItemArray[1];
                dr.Table.Columns.Add("xsrnl");
                if (dr["nldw"].ToString() == "月")
                {
                    dr["xsrnl"] = (Convert.ToInt16(dr["nl"])).ToString();
                }
                //
                dr.Table.Columns.Add("zdmc");
                dr.Table.Columns.Add("icd");
                dr.Table.Columns.Add("rybq");
                for (int i = 1; i < 22; i++)
                {
                    dr.Table.Columns.Add("zdmc" + i.ToString("00"));
                    dr.Table.Columns.Add("icd" + i.ToString("00"));
                    dr.Table.Columns.Add("rybq" + i.ToString("00"));
                }
                get_mzmc(dr, "mz");
                get_gjmc(dr, "gjdm");
                get_ksmc(dr, "ryksbm");
                get_ksmc(dr, "zkksbm");
                get_ksmc(dr, "ksbm");
                get_ysmc(dr, "kzr");
                get_ysmc(dr, "zrys");
                get_ysmc(dr, "zzys");
                get_ysmc(dr, "zyys");
                get_ysmc(dr, "jxys");
                get_ysmc(dr, "yjssxys");
                get_ysmc(dr, "sxys");
                get_jtgx(dr, "lxrgx");

                if (dr["bxbs"].ToString() == "1")
                {
                    dr["bxbs"] = "1";
                }
                else
                {
                    dr["bxbs"] = "3";
                };

                dr["zdmc"] = "主要诊断：" + ds5.Tables[0].Rows[0][0].ToString();
                dr["icd"] = ds5.Tables[0].Rows[0][1].ToString();
                dr["rybq"] = ds5.Tables[0].Rows[0][2].ToString();
                dr["zdmc01"] = "其他诊断：" + ds5.Tables[0].Rows[0][3].ToString();
                dr["icd01"] = ds5.Tables[0].Rows[0][4].ToString();
                dr["rybq01"] = ds5.Tables[0].Rows[0][5].ToString();
                dr["zdmc02"] = ds5.Tables[0].Rows[0][6].ToString();
                dr["icd02"] = ds5.Tables[0].Rows[0][7].ToString();
                dr["rybq02"] = ds5.Tables[0].Rows[0][8].ToString();
                dr["zdmc03"] = ds5.Tables[0].Rows[0][9].ToString();
                dr["icd03"] = ds5.Tables[0].Rows[0][10].ToString();
                dr["rybq03"] = ds5.Tables[0].Rows[0][11].ToString();
                dr["zdmc04"] = ds5.Tables[0].Rows[0][12].ToString();
                dr["icd04"] = ds5.Tables[0].Rows[0][13].ToString();
                dr["rybq04"] = ds5.Tables[0].Rows[0][14].ToString();
                dr["zdmc05"] = ds5.Tables[0].Rows[0][15].ToString();
                dr["icd05"] = ds5.Tables[0].Rows[0][16].ToString();
                dr["rybq05"] = ds5.Tables[0].Rows[0][17].ToString();
                dr["zdmc06"] = ds5.Tables[0].Rows[0][18].ToString();
                dr["icd06"] = ds5.Tables[0].Rows[0][19].ToString();
                dr["rybq06"] = ds5.Tables[0].Rows[0][20].ToString();
                dr["zdmc07"] = ds5.Tables[0].Rows[0][21].ToString();
                dr["icd07"] = ds5.Tables[0].Rows[0][22].ToString();
                dr["rybq07"] = ds5.Tables[0].Rows[0][23].ToString();
                dr["zdmc08"] = ds5.Tables[0].Rows[0][24].ToString();
                dr["icd08"] = ds5.Tables[0].Rows[0][25].ToString();
                dr["rybq08"] = ds5.Tables[0].Rows[0][26].ToString();
                dr["zdmc09"] = ds5.Tables[0].Rows[0][27].ToString();
                dr["icd09"] = ds5.Tables[0].Rows[0][28].ToString();
                dr["rybq09"] = ds5.Tables[0].Rows[0][29].ToString();
                dr["zdmc10"] = ds5.Tables[0].Rows[0][30].ToString();
                dr["icd10"] = ds5.Tables[0].Rows[0][31].ToString();
                dr["rybq10"] = ds5.Tables[0].Rows[0][32].ToString();
                dr["zdmc11"] = "其他诊断：" + ds5.Tables[0].Rows[0][33].ToString();
                dr["icd11"] = ds5.Tables[0].Rows[0][34].ToString();
                dr["rybq11"] = ds5.Tables[0].Rows[0][35].ToString();
                dr["zdmc12"] = ds5.Tables[0].Rows[0][36].ToString();
                dr["icd12"] = ds5.Tables[0].Rows[0][37].ToString();
                dr["rybq12"] = ds5.Tables[0].Rows[0][38].ToString();
                dr["zdmc13"] = ds5.Tables[0].Rows[0][39].ToString();
                dr["icd13"] = ds5.Tables[0].Rows[0][40].ToString();
                dr["rybq13"] = ds5.Tables[0].Rows[0][41].ToString();
                dr["zdmc14"] = ds5.Tables[0].Rows[0][42].ToString();
                dr["icd14"] = ds5.Tables[0].Rows[0][43].ToString();
                dr["rybq14"] = ds5.Tables[0].Rows[0][44].ToString();
                dr["zdmc15"] = ds5.Tables[0].Rows[0][45].ToString();
                dr["icd15"] = ds5.Tables[0].Rows[0][46].ToString();
                dr["rybq15"] = ds5.Tables[0].Rows[0][47].ToString();
                dr["zdmc16"] = ds5.Tables[0].Rows[0][48].ToString();
                dr["icd16"] = ds5.Tables[0].Rows[0][49].ToString();
                dr["rybq16"] = ds5.Tables[0].Rows[0][50].ToString();
                dr["zdmc17"] = ds5.Tables[0].Rows[0][51].ToString();
                dr["icd17"] = ds5.Tables[0].Rows[0][52].ToString();
                dr["rybq17"] = ds5.Tables[0].Rows[0][53].ToString();
                dr["zdmc18"] = ds5.Tables[0].Rows[0][54].ToString();
                dr["icd18"] = ds5.Tables[0].Rows[0][55].ToString();
                dr["rybq18"] = ds5.Tables[0].Rows[0][56].ToString();
                dr["zdmc19"] = ds5.Tables[0].Rows[0][57].ToString();
                dr["icd19"] = ds5.Tables[0].Rows[0][58].ToString();
                dr["rybq19"] = ds5.Tables[0].Rows[0][59].ToString();
                dr["zdmc20"] = ds5.Tables[0].Rows[0][60].ToString();
                dr["icd20"] = ds5.Tables[0].Rows[0][61].ToString();
                dr["rybq20"] = ds5.Tables[0].Rows[0][62].ToString();
                dr["zdmc21"] = ds5.Tables[0].Rows[0][63].ToString();
                dr["icd21"] = ds5.Tables[0].Rows[0][64].ToString();
                dr["rybq21"] = ds5.Tables[0].Rows[0][65].ToString();
                //
                if ((Convert.IsDBNull(dr["ywgm"]) == true) || (dr["ywgm"].ToString().Trim() == "") || (dr["ywgm"].ToString().Trim() == "-") || (dr["ywgm"].ToString().Trim() == "—") || (dr["ywgm"].ToString().Trim() == "无"))
                {
                    dr["sfgm"] = "1";
                }
                else
                {
                    dr["sfgm"] = "2";
                }


                //
                dr["cyzd"] = "主要诊断 " + dr["cyzd"];
                dr["qtzd"] = "其他诊断 " + dr["qtzd"];
                dr["qtzd2"] = "         " + dr["qtzd2"];
                dr["qtzd3"] = "         " + dr["qtzd3"];
                dr["qtzd4"] = "         " + dr["qtzd4"];
                dr["qtzd5"] = "         " + dr["qtzd5"];
                dr["yngr"] = "医院感染名称 " + dr["yngr"];
                dr["blzd"] = "病例诊断 " + dr["blzd"];
                dr["hznl"] = (Convert.ToInt16(dr["nl"])).ToString() + dr["nldw"];

                DateTime dt, dt1, dt2;

                if (Convert.IsDBNull(dr["csny"]) == false)
                {
                    dt = Convert.ToDateTime(dr["csny"]);
                    dr["csrq_n"] = dt.Year.ToString();
                    dr["csrq_y"] = dt.Month.ToString();
                    dr["csrq_r"] = dt.Day.ToString();
                }


                if (Convert.IsDBNull(dr["zyrq"]) == false)
                {
                    dt = Convert.ToDateTime(dr["zyrq"]);
                    dr["zyrq_n"] = dt.Year.ToString();
                    dr["zyrq_y"] = dt.Month.ToString();
                    dr["zyrq_r"] = dt.Day.ToString();
                    dr["zyrq_s"] = dt.Hour.ToString();
                }

                if (Convert.IsDBNull(dr["cyrq"]) == false)
                {
                    dt = Convert.ToDateTime(dr["cyrq"]);
                    dr["cyrq_n"] = dt.Year.ToString();
                    dr["cyrq_y"] = dt.Month.ToString();
                    dr["cyrq_r"] = dt.Day.ToString();
                    dr["cyrq_s"] = dt.Hour.ToString();
                }

                if (Convert.IsDBNull(dr["ryzdrq"]) == false)
                {
                    dt = Convert.ToDateTime(dr["ryzdrq"]);
                    dr["ryzdrq_n"] = dt.Year.ToString();
                    dr["ryzdrq_y"] = dt.Month.ToString();
                    dr["ryzdrq_r"] = dt.Day.ToString();
                }

                if (Convert.IsDBNull(dr["cyrq"]) == false)
                {
                    dt1 = Convert.ToDateTime(dr["zyrq"]).Date;
                    dt2 = Convert.ToDateTime(dr["cyrq"]).Date;
                    TimeSpan span = dt2.Subtract(dt1);
                    int dayDiff = span.Days;
                    if (dayDiff == 0) dayDiff = 1;
                    dr["zyts"] = dayDiff.ToString();
                }
                else
                {
                    dt1 = Convert.ToDateTime(dr["zyrq"]).Date;
                    dt2 = DateTime.Now.Date;
                    TimeSpan span = dt2.Subtract(dt1);
                    int dayDiff = span.Days;
                    if (dayDiff == 0) dayDiff = 1;
                    dr["zyts"] = dayDiff.ToString();
                }


                if (dr["cyqk"].ToString() == "01") dr["cyqk01"] = "√";
                if (dr["cyqk"].ToString() == "02") dr["cyqk02"] = "√";
                if (dr["cyqk"].ToString() == "03") dr["cyqk03"] = "√";
                if (dr["cyqk"].ToString() == "04") dr["cyqk04"] = "√";
                if (dr["cyqk"].ToString() == "05") dr["cyqk05"] = "√";

                if (dr["qtcyqk"].ToString() == "01") dr["cyqk11"] = "√";
                if (dr["qtcyqk"].ToString() == "02") dr["cyqk12"] = "√";
                if (dr["qtcyqk"].ToString() == "03") dr["cyqk13"] = "√";
                if (dr["qtcyqk"].ToString() == "04") dr["cyqk14"] = "√";
                if (dr["qtcyqk"].ToString() == "05") dr["cyqk15"] = "√";

                if (dr["qtcyqk2"].ToString() == "01") dr["cyqk21"] = "√";
                if (dr["qtcyqk2"].ToString() == "02") dr["cyqk22"] = "√";
                if (dr["qtcyqk2"].ToString() == "03") dr["cyqk23"] = "√";
                if (dr["qtcyqk2"].ToString() == "04") dr["cyqk24"] = "√";
                if (dr["qtcyqk2"].ToString() == "05") dr["cyqk25"] = "√";

                if (dr["qtcyqk3"].ToString() == "01") dr["cyqk31"] = "√";
                if (dr["qtcyqk3"].ToString() == "02") dr["cyqk32"] = "√";
                if (dr["qtcyqk3"].ToString() == "03") dr["cyqk33"] = "√";
                if (dr["qtcyqk3"].ToString() == "04") dr["cyqk34"] = "√";
                if (dr["qtcyqk3"].ToString() == "05") dr["cyqk35"] = "√";

                if (dr["qtcyqk4"].ToString() == "01") dr["cyqk41"] = "√";
                if (dr["qtcyqk4"].ToString() == "02") dr["cyqk42"] = "√";
                if (dr["qtcyqk4"].ToString() == "03") dr["cyqk43"] = "√";
                if (dr["qtcyqk4"].ToString() == "04") dr["cyqk44"] = "√";
                if (dr["qtcyqk4"].ToString() == "05") dr["cyqk45"] = "√";

                if (dr["qtcyqk5"].ToString() == "01") dr["cyqk51"] = "√";
                if (dr["qtcyqk5"].ToString() == "02") dr["cyqk52"] = "√";
                if (dr["qtcyqk5"].ToString() == "03") dr["cyqk53"] = "√";
                if (dr["qtcyqk5"].ToString() == "04") dr["cyqk54"] = "√";
                if (dr["qtcyqk5"].ToString() == "05") dr["cyqk55"] = "√";

            }

            if (ds2.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds2.Tables[0].Rows[0];
                int irowcount8;
                irowcount8 = ds3.Tables[0].Rows.Count;
                if (irowcount8 > 8) irowcount8 = 8;
                for (int i = 0; i < irowcount8; i++)
                {
                    DataRow dr3 = ds3.Tables[0].Rows[i];
                    dr["CZBM" + (i + 1).ToString()] = dr3["SSBM"].ToString();
                    dr["SSRQ" + (i + 1).ToString()] = dr3["SSKSRQ"].ToString().Substring(0, 10);
                    dr["SSJB" + (i + 1).ToString()] = dr3["SSDJ"].ToString();
                    dr["SSMC" + (i + 1).ToString()] = dr3["SSMC"].ToString();
                    dr["SSYS" + (i + 1).ToString()] = dr3["SSYS1"].ToString();
                    dr["Z1" + (i + 1).ToString()] = dr3["SSYS2"].ToString();
                    dr["Z2" + (i + 1).ToString()] = dr3["SSYS3"].ToString();
                    dr["MZ" + (i + 1).ToString()] = dr3["MZBM"].ToString();
                    dr["QK" + (i + 1).ToString()] = dr3["QKYHDJ"].ToString();
                    dr["MZYS" + (i + 1).ToString()] = dr3["MZYS1"].ToString();
                }

                dr["ERZD1"] = "新生儿诊断 " + dr["ERZD1"];
                if (dr["ERCYQK1"].ToString() == "01") dr["ERCYQK11"] = "√";
                if (dr["ERCYQK1"].ToString() == "02") dr["ERCYQK12"] = "√";
                if (dr["ERCYQK1"].ToString() == "03") dr["ERCYQK13"] = "√";
                if (dr["ERCYQK1"].ToString() == "04") dr["ERCYQK14"] = "√";
                if (dr["ERCYQK1"].ToString() == "05") dr["ERCYQK15"] = "√";

                if (dr["ERCYQK2"].ToString() == "01") dr["ERCYQK21"] = "√";
                if (dr["ERCYQK2"].ToString() == "02") dr["ERCYQK22"] = "√";
                if (dr["ERCYQK2"].ToString() == "03") dr["ERCYQK23"] = "√";
                if (dr["ERCYQK2"].ToString() == "04") dr["ERCYQK24"] = "√";
                if (dr["ERCYQK2"].ToString() == "05") dr["ERCYQK25"] = "√";

                if (dr["ERCYQK3"].ToString() == "01") dr["ERCYQK31"] = "√";
                if (dr["ERCYQK3"].ToString() == "02") dr["ERCYQK32"] = "√";
                if (dr["ERCYQK3"].ToString() == "03") dr["ERCYQK33"] = "√";
                if (dr["ERCYQK3"].ToString() == "04") dr["ERCYQK34"] = "√";
                if (dr["ERCYQK3"].ToString() == "05") dr["ERCYQK35"] = "√";

                if (dr["SZQXLB"].ToString() == "周")
                {
                    dr["SZQX_Z"] = dr["SZQX"].ToString();
                }
                else if (dr["SZQXLB"].ToString() == "月")
                {
                    dr["SZQX_Y"] = dr["SZQX"].ToString();
                }
                else if (dr["SZQXLB"].ToString() == "年")
                {
                    dr["SZQX_N"] = dr["SZQX"].ToString();
                }

                dr["FY01"] = Convert.ToDecimal(dr["FY01"]) + Convert.ToDecimal(dr["FY17"]) + Convert.ToDecimal(dr["FY18"]) + Convert.ToDecimal(dr["FY19"]);
                dr["FY02"] = Convert.ToDecimal(dr["FY02"]) + Convert.ToDecimal(dr["FY20"]) + Convert.ToDecimal(dr["FY24"]);
                dr["FY03"] = Convert.ToDecimal(dr["FY03"]) + Convert.ToDecimal(dr["FY21"]);
                dr["FY04"] = Convert.ToDecimal(dr["FY04"]) + Convert.ToDecimal(dr["FY22"]);
                dr["FY09"] = Convert.ToDecimal(dr["FY09"]) + Convert.ToDecimal(dr["FY12"]);
                dr["FY05"] = Convert.ToDecimal(dr["FY05"]) + Convert.ToDecimal(dr["FY16"]) + Convert.ToDecimal(dr["FY23"]);
                dr["FY06"] = Convert.ToDecimal(dr["FY06"]) + Convert.ToDecimal(dr["FY07"]) + Convert.ToDecimal(dr["FY11"]);

            }

            //DataTable dtScoreOther = new DataTable("Result");
            //dtScoreOther.Columns.Add("PatientName");
            //DataRow dr2 = dtScoreOther.NewRow();
            //dr2["PatientName"] = "strPaitientName";
            //dtScoreOther.Rows.Add(dr2);

            //reportViewer2.LocalReport.DataSources.Add(new ReportDataSource("ScoreResult_Result", dtScoreOther));
            //reportViewer2.LocalReport.DataSources.Add(new ReportDataSource("ScoreResult_Result", ds1.Tables[0]));

            this.reportViewer2.LocalReport.DataSources.Add(new ReportDataSource("BA", ds1.Tables[0]));
            this.reportViewer2.LocalReport.DataSources.Add(new ReportDataSource("BA2", ds2.Tables[0]));
            this.reportViewer2.LocalReport.Refresh();
            this.reportViewer2.RefreshReport();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

           
            LoadFlaws(registryID);

        }
       

     


        private void get_ksmc(DataRow dr, string para1)
        {
            string ksbm = dr[para1].ToString();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = ep.GetMedicalDataEx("ksbm",ksbm);
                if (ds.Tables[0].Rows.Count == 0) return;
                DataRow dr1 = ds.Tables[0].Rows[0];
                dr[para1] = dr1["ksmc"].ToString();
            }
        }
        private void get_ysmc(DataRow dr, string para1)
        {
            string ysbm = dr[para1].ToString();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = ep.GetMedicalDataEx("ysbm", ysbm);

                if (ds.Tables[0].Rows.Count == 0) return;
                DataRow dr1 = ds.Tables[0].Rows[0];
                dr[para1] = dr1["ysm"].ToString();
            }
        }

        private void get_mzmc(DataRow dr, string para1)
        {
            string mzbm = dr[para1].ToString();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = ep.GetMedicalDataEx("MZDM", mzbm);

                if (ds.Tables[0].Rows.Count == 0) return;
                DataRow dr1 = ds.Tables[0].Rows[0];
                dr[para1] = dr1["MZ"].ToString();
            }
        }

        private void get_gjmc(DataRow dr, string para1)
        {
            string gjbm = dr[para1].ToString();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = ep.GetMedicalDataEx("SJDM", gjbm);
                if (ds.Tables[0].Rows.Count == 0) return;
                DataRow dr1 = ds.Tables[0].Rows[0];
                dr[para1] = dr1["ZWMC_1"].ToString();
            }
        }

        private void get_jtgx(DataRow dr, string para1)
        {
            string jtgxbm = dr[para1].ToString();
            using (gjtEmrPatients.emrPatients ep = new gjtEmrPatients.emrPatients())
            {
                DataSet ds = ep.GetMedicalDataEx("JTGXDM", jtgxbm);
                if (ds.Tables[0].Rows.Count == 0) return;
                DataRow dr1 = ds.Tables[0].Rows[0];
                dr[para1] = dr1["JTGX"].ToString();
            }
        }

  




    }
}
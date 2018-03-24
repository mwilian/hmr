using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Data.OracleClient;
using AboutConfig;
using System.Runtime.InteropServices;

using EmrConstant;
using System.Text;
using System.Collections.Generic;


/// <summary>
/// Summary description for emrPatients
/// </summary>
[WebService(Namespace = "http://shoujia.org/emrpatients")]
//[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[WebServiceBinding(ConformsTo = WsiProfiles.None)]
public class emrPatients : System.Web.Services.WebService
{

    private XmlDocument xmldoc;
    private string connnectString;
    private string hisDBType;
    private string hisDBBA;
    private string dbcf;
    private string EmrDB;

  //  private string EmrDB = "";
    public emrPatients()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        connnectString = ConfigClass.GetConfigString("appSettings", "HisDB");
        hisDBType = ConfigClass.GetConfigString("appSettings", "HisDBType");
        hisDBBA = ConfigClass.GetConfigString("appSettings", "hisDBBA");
        dbcf = ConfigClass.GetConfigString("appSettings", "DBCF");
       EmrDB = ConfigClass.GetConfigString("appSettings", "EmrDB");
    }
    private DataSet ExeuSentence(string sqlSentence, string hisDBType)
    {
        DataSet myDataSet = new DataSet();
        switch (hisDBType)
        {
            case "ORACLE":
                OracleConnection oconn = new OracleConnection(connnectString);
                oconn.Open();
                OracleDataAdapter ocommand = new OracleDataAdapter(sqlSentence, oconn);
                ocommand.Fill(myDataSet, "Results");
                oconn.Close();
                break;
            case "MSSQL":
                SqlConnection sconn = new SqlConnection(connnectString);
                sconn.Open();
                SqlDataAdapter scommand = new SqlDataAdapter(sqlSentence, sconn);
                scommand.Fill(myDataSet, "Results");
                sconn.Close();
                break;
        }
        return myDataSet;
    }
    private XmlNode DataError(Exception ex)
    {

        XmlDocument xmldoc = new XmlDocument();

        //加入XML的声明段落 
        XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
        xmldoc.AppendChild(xmlnode);
        //加入一个根元素
        XmlElement xmlelem = xmldoc.CreateElement("", "errors", "");
        xmldoc.AppendChild(xmlelem);

        XmlNode root = xmldoc.SelectSingleNode("errors");

        XmlElement xe1 = xmldoc.CreateElement("error");

        XmlAttribute xmlattr = xmldoc.CreateAttribute("errordate");
        xmlattr.Value = XmlConvert.DecodeName(System.DateTime.Now.ToString().Split(' ')[0]);
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("message");
        xmlattr.Value = XmlConvert.DecodeName(ex.Message);
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("method");
        xmlattr.Value = XmlConvert.DecodeName(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        xe1.Attributes.Append(xmlattr);

        root.AppendChild(xe1);

        return root;
    }

    /* ---------------------------------------------------------------------------
     * 返回 XmlNode:
     *    <Doctors>
     *      <Doctor Name="刘丽" />
     *      <Doctor Name="张建国" />
     *    </Doctors>
     * 注：相同姓名只取一个。
     * 出错：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="DoctorNameList" />
     *  </errors>
     ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns doctor name list", EnableSession = false)]
    public XmlNode DoctorNameList()
    {
        try
        {
            string SQLSentence = "SELECT DISTINCT ysm FROM tysm WHERE lb = '1' ORDER BY ysm";
            DataSet myDataSet = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement(EmrConstant.ElementNames.Doctors);               
            DataTable table = myDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                XmlElement doctor = xmldoc.CreateElement(EmrConstant.ElementNames.Doctor);
                doctor.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[0].ToString().Trim()+" 医师");
                root.AppendChild(doctor);
            }
            return root;
        } //try end
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    /* ---------------------------------------------------------------------------
      * 返回 XmlNode:
      *    <Doctors>
      *      <Doctor Code="1104" Name="刘丽" Spell="ll" TecqTitle="主任医师" TitleLevel="1" />
      *      <Doctor Code="1105" Name="张建国" Spell="zjg" TecqTitle="副主任医师" TitleLevel="2" />
      *      <Doctor Code="1106" Name="张建国" Spell="zjg" TecqTitle="主治医师" TitleLevel="3" />
      *      <Doctor Code="1107" Name="李剑" Spell="lj" TecqTitle="医师" TitleLevel="4" />
      *      <Doctor Code="1108" Name="王强" Spell="wq" TecqTitle="见习医师" TitleLevel="5" />
      *      <Doctor Code="1109" Name="秦朗" Spell="ql" TecqTitle="实习医师" TitleLevel="6" />
      *    </Doctors>
      * 注：所有属性不允许为空值     
      * 出错：
      *  <errors>
      *      <error errordate="2008-01-01" message="error message" method="DoctorList" />
      *  </errors>
      ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns codes, names, spellcodes, titles and titlelevels of doctors")]
    public XmlNode DoctorList()
    {
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "SELECT a.ysbm, a.ysm, a.xmpy, b.zcmc, b.zcjbbm, a.zlzz,a.lb " +
                "FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm " +
                "WHERE (a.lb = '1' or a.lb = '2')  ORDER BY a.ysm";
            DataSet myDataSet = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement(EmrConstant.ElementNames.Doctors);
            DataTable table = myDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                XmlElement doctor = xmldoc.CreateElement(EmrConstant.ElementNames.Doctor);
                doctor.SetAttribute(EmrConstant.AttributeNames.Code, row.ItemArray[0].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Spell, row.ItemArray[2].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TecqTitle, row.ItemArray[3].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TitleLevel, row.ItemArray[4].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Header, row.ItemArray[5].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.DoctorType, row.ItemArray[6].ToString());
              //  doctor.SetAttribute(EmrConstant.AttributeNames.LevelCode, row.ItemArray[7].ToString());
                root.AppendChild(doctor);
            }
            return root;
        } //try end
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    [WebMethod(Description = "Returns codes, names, spellcodes, titles and titlelevels of doctors")]
    public XmlNode DoctorListEmr(string Type)
    {
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "";
            if (Type == "")
            {
                SQLSentence = @"select * from operator ";
            }
            else
            {

                SQLSentence = @"select * from operator where doctortype = '" + Type + "'";
            }
           
            SqlHelper Helper = new SqlHelper("EmrDB");
            DataSet myDataSet=Helper.GetDataSet(SQLSentence);
            XmlDocument xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement(EmrConstant.ElementNames.Doctors);
            DataTable table = myDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                XmlElement doctor = xmldoc.CreateElement(EmrConstant.ElementNames.Doctor);
                doctor.SetAttribute(EmrConstant.AttributeNames.Code, row.ItemArray[0].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Spell, row.ItemArray[2].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TecqTitle, row.ItemArray[3].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TitleLevel, row.ItemArray[4].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Header, row.ItemArray[5].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.DoctorType, row.ItemArray[6].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.LevelCode, row.ItemArray[7].ToString());
                root.AppendChild(doctor);
            }
            return root;
        } //try end
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    //
    [WebMethod]
    public XmlNode DoctorListEx()
    {
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = @"SELECT a.ysbm, a.ysm, a.xmpy, b.zcmc, b.zcjbbm, a.zlzz,a.lb ,a.ksmc
                FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm 
                WHERE (a.lb = '1' or a.lb = '2')  ORDER BY a.ksmc";
            DataSet myDataSet = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement(EmrConstant.ElementNames.Doctors);
            DataTable table = myDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                XmlElement doctor = xmldoc.CreateElement(EmrConstant.ElementNames.Doctor);
                doctor.SetAttribute(EmrConstant.AttributeNames.Code, row.ItemArray[0].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Spell, row.ItemArray[2].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TecqTitle, row.ItemArray[3].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.TitleLevel, row.ItemArray[4].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.Header, row.ItemArray[5].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.DoctorType, row.ItemArray[6].ToString());
                doctor.SetAttribute(EmrConstant.AttributeNames.DepartmentNameA, row.ItemArray[7].ToString());
                root.AppendChild(doctor);
            }
            return root;
        } //try end
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
   
    [WebMethod]
    public DataSet LevelEx(string strLevel)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");

        string select = "select Code,Name,DJ from Level where type = '" + strLevel + "'";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(select);
            
        }
        catch (Exception ex)
        {
           
        }
        return dt;
    }

    [WebMethod]
    public DataTable OperatorType()
    {


        DataTable dt = new DataTable();

        dt.Columns.Add(new DataColumn("Code", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        DataRow dr = dt.NewRow();
        dr["Code"] = "01";
        dr["Name"] = "医生";
        dt.Rows.Add(dr);
        dr = dt.NewRow();
        dr["Code"] = "02";
        dr["Name"] = "护士";
        dt.Rows.Add(dr);
        dt.TableName = "Type";
        return dt;


    }
    [WebMethod]
    public string GetDoctorType(string strOpCode)
    {
        string strType = "";

        SqlHelper Helper = new SqlHelper("HisDB");
        string strSelect = "select lb from tysm where ysbm  = '" + strOpCode + "' ";
        DataTable dt = Helper.GetDataTable(strSelect);

        if (dt != null && dt.Rows.Count != 0)
        {
            strType = dt.Rows[0]["lb"].ToString();
        }
        return strType;
        
    }

    
    /* --------------------------------------------------------------------------
     * 返回 XmlNode（在院患者名单）:
     * <Patients xmlns="">
     *      <Patient ArchiveNum="008342" PatientName="郭墨海" Sex="男" Birth="1942-5-4"  
     *               Age="65" AgeUnit="岁" Nation="汉族" MaritalStatus="已婚" 
     *               NativePlace="天津" Job="工人" Address="河西区新会道恒山里14-209">
     *          <Registry RegistryID="00009438" BedNum="1628" RegistryDate="2007-3-10" 
     *                    RegistryTime="15:39:45" DoctorID="1104" DepartmentCode="0241" />
     *      </Patient>
     *      <Patient ArchiveNum="007636" PatientName="李凤兰" Sex="女" Birth="1951-8-14"
     *               Nation="汉族" MaritalStatus="已婚" NativePlace="天津" Job="" 
     *               Address="天津市河西区新安楼4-303">
     *          <Registry RegistryID="00009607" BedNum="1619" RegistryDate="2007-3-25" 
     *                    RegistryTime="16:01:20" DoctorID="1104" DepartmentCode="0241"
     *                    DischargedDate="" PatientStatus=""/>
     *          <Registry RegistryID="00008546" BedNum="1345" RegistryDate="2006-12-25"
     *                    RegistryTime="16:01:20" DoctorID="1104" DepartmentCode="0010" 
     *                    DischargedDate="2007-3-25 15:58:55" PatientStatus="出科" />
     *      </Patient>
     * </Patients>
     * 参数:
     *      EmrConstant.PatientGettingMode mode -- 选取方式
     * case mode = PatientGettingMode.MyPatients: 
     *      code 是医师编码
     *      取医师负责的患者名单
     * case mode = PatientGettingMode.PatientsInArea:
     *      code 是病区编码
     *      取指定病区的患者名单
     * case mode = PatientGettingMode.PatientsInDepartment:
     *      code 是科室编码
     *      取指定科室的患者名单
     * case PatientGettingMode.PatientsInHospital:
     *      code 不用
     *      取全部住院患者名单
     * 说明：
     *     ArchiveNum     患者在医院内唯一标识号，不管他(她)就医次数如何
     *     PatientName    患者姓名
     *     Sex            患者性别
     *     Birth          患者出生年月日 
     *     Age            患者年龄
     *     AgeUnit        年龄单位（岁、月、天）
     *     Nation         患者的民族，可以为空值
     *     MaritalStatus  患者的婚姻状况，可以为空值
     *     NativePlace    患者的出生地或祖籍，可以为空值
     *     Job            患者的职业，可以为空值
     *     Address        患者当前家庭住址，可以为空值
     *     RegistryID     患者住院登记号，一次住院一个
     *     BedNum         住院床号
     *     RegistryDate   入科日期
     *     RegistryTime   入科时间 "15:39:45"
     *     DoctorID       患者的责任医师编码
     *     DepartmentCode 住院科室编码 
     *     DischargedDate 出院日期和时间，只用于以往住院或出科未结算
     *     PatientStatus  = "" 在住院；= "出科" 出科未结算；= "出院" 以往住院
     * 出错返回：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="PatientList" />
     *  </errors>
     ----------------------------------------------------------------------------*/
    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientList(EmrConstant.PatientGettingMode mode, string code)
    {
        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
                case PatientGettingMode.MyPatients:
                    SQLSentence += 
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys =  '" + code + "'";
                    break;
                case PatientGettingMode.PatientsInArea:                   
                    SQLSentence += 
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.bqbm =  '" + code + "'";
                    break;
                case PatientGettingMode.PatientsInDepartment:                    
                    SQLSentence += 
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "'";
                    break;
                case PatientGettingMode.PatientsInHospital:                    
                    SQLSentence += 
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm";
                    break;
            }
            #endregion
            /* Execute sentence */
            SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }



    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListExEx(string Deptcode,string opcode, EmrConstant.OrderByMode bymode)  
    {
    

        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh ";
       XmlDocument xmldoc = new XmlDocument();
        XmlNode root = null;
     
        try
        {
         
                    SQLSentence +=
                    @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                    "WHERE (e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "' )";
                
            
            if (bymode == EmrConstant.OrderByMode.ByBed)
                SQLSentence += " ORDER BY a.ch";
            else
                SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root,true);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
}

    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListExExEx(string Deptcode, string opcode, EmrConstant.OrderByMode bymode)
    {

        SqlHelper emrHelper = new SqlHelper("EmrDB");
        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh,e.SERIAL_NO  ";
        XmlDocument xmldoc = new XmlDocument();
        XmlNode root = null;

        try
        {
            string strDep = "select [group]  from TB_group where [group]  like '%" + Deptcode + "%'";
         

            SQLSentence +=
            @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
            "WHERE ";

            DataTable dt1 = emrHelper.GetDataTable(strDep);
            if (dt1 != null && dt1.Rows.Count != 0)
            {
                string str = dt1.Rows[0]["Group"].ToString();
                string[] strlist = str.Split(',');
                if (strlist.Length != 0)
                {
                    for (int i = 0; i < strlist.Length; i++)
                    {
                        SQLSentence = SQLSentence + " (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "' )" + "OR";

                    }
                    SQLSentence = SQLSentence.Substring(0, SQLSentence.Length - 2);
                }
            
            }
            else
            {
                SQLSentence = SQLSentence + "(e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "' )";
            }

            if (bymode == EmrConstant.OrderByMode.ByBed)
                SQLSentence += " ORDER BY a.ch";
            else
                SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root, true);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    [WebMethod(Description = "Get filed time for the consult ", EnableSession = false)]
    public string DoneTimeForConsultExEx(string registryID, string Sequence, ref XmlNode doneTime)
    {

        try
        {
            if (Sequence != null && Sequence != "")
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no, reason " +
                    "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign = '2' and serial_no = '" + Sequence + "'";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
            else
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no, reason " +
                                 "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign = '2' ";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListEx(EmrConstant.PatientGettingMode mode, string code, EmrConstant.OrderByMode bymode)
    ////public XmlNode PatientListEx(string code)
    {
        ////EmrConstant.PatientGettingMode mode = PatientGettingMode.BloodDialysis;
        ////EmrConstant.OrderByMode bymode = OrderByMode.ByBed;

        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh  ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
                case PatientGettingMode.MyPatients:
                    SQLSentence +=
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.ZLzz:
                    SQLSentence +=
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm right JOIN tysm t ON t.zlzz=a.zrys WHERE a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInArea:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.bqbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartmentCK:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "'  and cyrq!='' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartment:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')"+
                         " order by a.cyrq,a.ch,a.zyh,a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyrq,a.zrys,a.ksbm,a.nl,a.nldw,a.dh  desc";
                       
                    break;
                case PatientGettingMode.PatientsInHospital:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm where a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.Consult:
                    SQLSentence +=
                    @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                    "WHERE  e.con_sign <> '2'";
                    break;
                case PatientGettingMode.MyOwner:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys in (select ysbm from tysm where zlzz in (select zlzz from tysm where ysbm = '" + code + "' )) ";
                    break;
                case PatientGettingMode.Operation:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh in (select zyh from ssmxk ) ";
                    break;
                case PatientGettingMode.BloodDialysis:
                    string strCode = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"] != null)
                    {
                        strCode = System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"].ToString().Trim();
                    }
                    else
                    {
                        strCode = "00410181";
                    }
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "') ";
//                    SQLSentence +=
//                @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
//                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "' AND  kqzlbs = '1' )";
                    break;

            }
            #endregion
            /* Execute sentence */
            if (mode != PatientGettingMode.PatientsInDepartment)
            {
                if (bymode == EmrConstant.OrderByMode.ByBed)
                    SQLSentence += " ORDER BY a.ch";
                else
                    SQLSentence += " ORDER BY a.xb, a.xm";
            }
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    //静海写婴儿病历
    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListEr(EmrConstant.PatientGettingMode mode, string code, EmrConstant.OrderByMode bymode)   
    {
        
        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh  ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
                case PatientGettingMode.MyPatients:
                    SQLSentence +=
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.ZLzz:
                    SQLSentence +=
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm right JOIN tysm t ON t.zlzz=a.zrys WHERE a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInArea:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.bqbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartmentCK:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "'  and cyrq!='' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartment:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')" +
                         " order by a.cyrq,a.ch,a.zyh,a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyrq,a.zrys,a.ksbm,a.nl,a.nldw,a.dh  desc";

                    break;
                case PatientGettingMode.PatientsInHospital:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm where a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.Consult:
                    SQLSentence +=
                    @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                    "WHERE  e.con_sign <> '2'";
                    break;
                case PatientGettingMode.MyOwner:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys in (select ysbm from tysm where zlzz in (select zlzz from tysm where ysbm = '" + code + "' )) ";
                    break;
                case PatientGettingMode.Operation:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh in (select zyh from ssmxk ) ";
                    break;
                case PatientGettingMode.BloodDialysis:
                    string strCode = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"] != null)
                    {
                        strCode = System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"].ToString().Trim();
                    }
                    else
                    {
                        strCode = "00410181";
                    }
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "') ";
                    //                    SQLSentence +=
                    //                @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                    //                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "' AND  kqzlbs = '1' )";
                    break;

            }
            #endregion
            /* Execute sentence */
            if (mode != PatientGettingMode.PatientsInDepartment)
            {
                if (bymode == EmrConstant.OrderByMode.ByBed)
                    SQLSentence += " ORDER BY a.ch";
                else
                    SQLSentence += " ORDER BY a.xb, a.xm";
            }
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListEx1(EmrConstant.PatientGettingMode mode, string code, EmrConstant.OrderByMode bymode)
    ////public XmlNode PatientListEx(string code)
    {
        ////EmrConstant.PatientGettingMode mode = PatientGettingMode.BloodDialysis;
        ////EmrConstant.OrderByMode bymode = OrderByMode.ByBed;

        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh,e.SERIAL_NO  ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
                case PatientGettingMode.MyPatients:
                    SQLSentence +=
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInArea:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.bqbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartment:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.ksbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInHospital:
                    SQLSentence +=
                        "FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm where a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.Consult:
                    SQLSentence +=
                    @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                    "WHERE  e.con_sign <> '2'";
                    break;
                case PatientGettingMode.MyOwner:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zrys in (select ysbm from tysm where zlzz in (select zlzz from tysm where ysbm = '" + code + "' )) ";
                    break;
                case PatientGettingMode.Operation:
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh in (select zyh from ssmxk ) ";
                    break;
                case PatientGettingMode.BloodDialysis:
                    string strCode = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"] != null)
                    {
                        strCode = System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"].ToString().Trim();
                    }
                    else
                    {
                        strCode = "00410181";
                    }
                    SQLSentence +=
                   @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "') ";
                    //                    SQLSentence +=
                    //                @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                    //                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "' AND  kqzlbs = '1' )";
                    break;

            }
            #endregion
            /* Execute sentence */
            if (bymode == EmrConstant.OrderByMode.ByBed)
                SQLSentence += " ORDER BY a.ch";
            else
                SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    [WebMethod(Description = "Return archive inpatient list", EnableSession = false)]
    public XmlNode PatientListExA(EmrConstant.PatientGettingMode mode, string code, EmrConstant.OrderByMode bymode)
    ////public XmlNode PatientListEx(string code)
    {
        ////EmrConstant.PatientGettingMode mode = PatientGettingMode.BloodDialysis;
        ////EmrConstant.OrderByMode bymode = OrderByMode.ByBed;

        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
                case PatientGettingMode.MyPatients:
                    SQLSentence +=
                        "FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.emrstatus = 1 and a.zrys =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInArea:
                    SQLSentence +=
                        "FROM tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE  a.emrstatus = 1 and a.bqbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInDepartment:
                    SQLSentence +=
                        "FROM tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE  a.emrstatus = 1  and a.ksbm =  '" + code + "' and a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.PatientsInHospital:
                    SQLSentence +=
                        "FROM tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm where a.emrstatus = 1 and  a.bah is not null and a.zyrq is not null and  (a.bqbm is not null or a.bqbm <>'')";
                    break;
                case PatientGettingMode.Consult:
                    SQLSentence +=
                    @"FROM tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                    "WHERE a.emrstatus = 1 and  e.doctor_no='" + code + "' AND e.con_sign <> '2'";
                    break;
                case PatientGettingMode.MyOwner:
                    SQLSentence +=
                   @"FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.emrstatus = 1 and  a.zrys in (select ysbm from tysm where zlzz in (select zlzz from tysm where ysbm = '" + code + "' )) ";
                    break;
                case PatientGettingMode.Operation:
                    SQLSentence +=
                   @"FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.emrstatus = 1 and  a.zyh in (select zyh from ssmxk ) ";
                    break;
                case PatientGettingMode.BloodDialysis:
                    string strCode = "";
                    if (System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"] != null)
                    {
                        strCode = System.Configuration.ConfigurationManager.AppSettings["BloodDialysis"].ToString().Trim();
                    }
                    else
                    {
                        strCode = "00410181";
                    }
                    SQLSentence +=
                   @"FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.emrstatus = 1 and   a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "') ";
                    //                    SQLSentence +=
                    //                @"FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                    //                       LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh IN (SELECT zyh FROM tlsyzz WHERE bm = '" + strCode + "' AND  kqzlbs = '1' )";
                    break;

            }
            #endregion
            /* Execute sentence */
            if (bymode == EmrConstant.OrderByMode.ByBed)
                SQLSentence += " ORDER BY a.ch";
            else
                SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListExConsult(EmrConstant.PatientGettingMode mode, string code, EmrConstant.OrderByMode bymode)
    ////public XmlNode PatientListEx(string code)
    {
        ////EmrConstant.PatientGettingMode mode = PatientGettingMode.BloodDialysis;
        ////EmrConstant.OrderByMode bymode = OrderByMode.ByBed;

        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh,e.SERIAL_NO ";
        xmldoc = new XmlDocument();
        XmlNode root = null;
        bool inStyle = true;
        try
        {
            #region Make SQL sentence
            switch (mode)
            {
               
                case PatientGettingMode.Consult:
//                    SQLSentence +=
//                    @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
//                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
//                    "WHERE e.doctor_no='" + code + "' AND e.con_sign <> '2'";
                    SQLSentence +=
                   @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
                   "WHERE  e.con_sign <> '2'";
                    break;
             
            }
            #endregion
            /* Execute sentence */
            if (bymode == EmrConstant.OrderByMode.ByBed)
                SQLSentence += " ORDER BY a.ch";
            else
                SQLSentence += " ORDER BY a.xb, a.xm";
            PatientProcess(SQLSentence, ref root, inStyle);
            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    private string GetSexName(string sexCode)
    {
        string sexName = "";
        switch (sexCode)
        {
            case "1":
                sexName = "男";
                break;
            case "2":
                sexName = "女";
                break;
        }
        return sexName;
    }
    public bool PatientProcess(string SQLSentence, ref XmlNode node, bool histroy)
    {
        //string select = "SELECT bh, name FROM batsxx WHERE (type = 'ZYXZ') ORDER BY bh";
        //DataSet dataSetJobs = ExeuSentence(select, hisDBType);
        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
            Jobs(ref jobs);
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(EmrConstant.ElementNames.Patients);

            DataSet dataSet = ExeuSentence(SQLSentence, hisDBType);
            DataTable table = dataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                 #region Create patient info
                if (row.ItemArray[10].ToString().Length == 0) continue;
                XmlElement patient = doc.CreateElement(EmrConstant.ElementNames.Patient);
                string archiveNum = row.ItemArray[0].ToString();
                //if (archiveNum.Length == 0) archiveNum = EmrConstant.StringGeneral.NullCode;
                patient.SetAttribute(AttributeNames.ArchiveNum, archiveNum);
                patient.SetAttribute(AttributeNames.PatientName, row.ItemArray[1].ToString());
                               
                patient.SetAttribute(AttributeNames.Sex, GetSexName(row.ItemArray[2].ToString()));
                patient.SetAttribute(AttributeNames.Birthday, row.ItemArray[3].ToString());
                string birth = row.ItemArray[3].ToString();
                if (birth.Length == 0) birth = DateTime.Today.ToString(StringGeneral.DateFormat);
                patient.SetAttribute(AttributeNames.Birth, birth.Split(' ')[0]);
                string age = row.ItemArray[15].ToString().Split('.')[0];
                DateTime nowday = DateTime.Today;
                DateTime daybirth = Convert.ToDateTime(birth);
                if (age.Length == 0)
                {                   
                    
                    TimeSpan tsage = nowday.Subtract(daybirth);
                    Double dbAge = (Double)(tsage.TotalDays / 365);
                    patient.SetAttribute(AttributeNames.Age, dbAge.ToString());
                    patient.SetAttribute(AttributeNames.AgeUnit, "岁");
                }
                else if (nowday.Year - daybirth.Year <= 0)
                {
                    //2012-08-16  zzl
                    if (nowday.Month - daybirth.Month == 0)
                    {
                        int dbAge = nowday.Day - daybirth.Day;                        
                        patient.SetAttribute(AttributeNames.Age, dbAge.ToString());
                        patient.SetAttribute(AttributeNames.AgeUnit, "天");

                    }
                    else 
                    {
                        int year = DateTime.Now.Year;
                        int month = DateTime.Now.Month;
                        int days = DateTime.DaysInMonth(year, month);
                        TimeSpan tsage = nowday.Subtract(daybirth); 
                        int mm = (int)(tsage.TotalDays / days);
                        int d = (int)(tsage.TotalDays % days);
                        //string day = mm.ToString().Substring(d, 1);
                        string dbAge = mm.ToString() +"("+ d.ToString()+ "/" + days +")";
                        patient.SetAttribute(AttributeNames.Age, dbAge.ToString());
                        patient.SetAttribute(AttributeNames.AgeUnit, "月");
 
                    }                  
                    
                   
                }
                else if (nowday.Year - daybirth.Year <= 3)
                {
                    int year = DateTime.Now.Year;
                    int month = DateTime.Now.Month;
                    // int days = DateTime.DaysInMonth(year, month);
                    int birthY = year - daybirth.Year;
                    int birthM = month - daybirth.Month;
                    string dbAge = birthY.ToString() + "(" + birthM.ToString() + "/" + "12)";
                    patient.SetAttribute(AttributeNames.Age, dbAge.ToString());
                    patient.SetAttribute(AttributeNames.AgeUnit, "岁");
        
                }
                else
                { 
                                 
                    patient.SetAttribute(AttributeNames.Age, age);
                    patient.SetAttribute(AttributeNames.AgeUnit, row.ItemArray[16].ToString());
                }

                patient.SetAttribute(EmrConstant.AttributeNames.Nation, row.ItemArray[4].ToString());
                patient.SetAttribute(EmrConstant.AttributeNames.MaritalStatus, row.ItemArray[5].ToString());
                patient.SetAttribute(EmrConstant.AttributeNames.NativePlace, row.ItemArray[6].ToString());

                DataRow[] rowsJob = jobs.Select("bh='" + row.ItemArray[7].ToString() + "'");
                if (rowsJob.Length == 1) 
                    patient.SetAttribute(EmrConstant.AttributeNames.Job, rowsJob[0][1].ToString());
                else
                    patient.SetAttribute(EmrConstant.AttributeNames.Job, row.ItemArray[7].ToString());

                patient.SetAttribute(EmrConstant.AttributeNames.Address, row.ItemArray[8].ToString());
                #endregion

                #region Create registry info
                XmlElement registry = doc.CreateElement(ElementNames.Registry);
                registry.SetAttribute(AttributeNames.RegistryID, row.ItemArray[9].ToString());
                registry.SetAttribute(AttributeNames.BedNum, row.ItemArray[10].ToString());
                if (row.ItemArray[11].ToString().Length == 0)
                {
                    registry.SetAttribute(AttributeNames.RegistryDate, DateTime.Today.ToString());
                    registry.SetAttribute(AttributeNames.RegistryTime, "00:00:00"); 
                }
                else
                {
                    string[] items = row.ItemArray[11].ToString().Split(Delimiters.Space);
                    registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, items[0]);
                    registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, items[1]);
                }
                
                registry.SetAttribute(AttributeNames.OutRegistryDate, row.ItemArray[14].ToString());
                registry.SetAttribute(AttributeNames.DoctorID, row.ItemArray[12].ToString());
                registry.SetAttribute(AttributeNames.DepartmentCode, row.ItemArray[13].ToString());
                registry.SetAttribute(AttributeNames.Phone, row.ItemArray[17].ToString());
                if (row.ItemArray.Length == 19)
                {
                    registry.SetAttribute(AttributeNames.Sequence, row.ItemArray[18].ToString());
                }
                registry.SetAttribute(AttributeNames.Birthday, row.ItemArray[3].ToString());
                registry.SetAttribute(AttributeNames.CardNum, "");
                registry.SetAttribute(AttributeNames.CardType, "");
                registry.SetAttribute(AttributeNames.CanBuyDrug, "");
                registry.SetAttribute(AttributeNames.LowInsurance, "0");
                if (Convert.IsDBNull(row.ItemArray[14]))
                {
                    registry.SetAttribute(AttributeNames.DischargedDate, "");
                    registry.SetAttribute(AttributeNames.PatientStatus, "");
                }
                else
                {
                    registry.SetAttribute(AttributeNames.DischargedDate, row.ItemArray[14].ToString());
                    if (histroy)
                        registry.SetAttribute(AttributeNames.PatientStatus, StringGeneral.QuasiDischarged);
                    else
                        registry.SetAttribute(AttributeNames.PatientStatus, StringGeneral.Discharged);
                }

                patient.AppendChild(registry);
                #endregion

                #region Has the patient come before?
                //string archiveNum = row.ItemArray[0].ToString();
                if (archiveNum != EmrConstant.StringGeneral.NullCode && histroy == true)
                {
                    string SQLSentence1;
                    SQLSentence1 = "SELECT zyh,ch,zyrq,zrys,ksbm,cyrq,emrstatus FROM tdjk WHERE bah = '" + archiveNum + "'";
                    DataSet dataSet1 = ExeuSentence(SQLSentence1, hisDBType);
                    foreach (DataRow row1 in dataSet1.Tables[0].Rows)
                    {
                        XmlElement registryOld = doc.CreateElement(EmrConstant.ElementNames.Registry);
                        registryOld.SetAttribute(EmrConstant.AttributeNames.RegistryID, row1.ItemArray[0].ToString());
                        registryOld.SetAttribute(EmrConstant.AttributeNames.BedNum, row1.ItemArray[1].ToString());
                        //registryOld.SetAttribute(EmrConstant.AttributeNames.BedNum, EmrConstant.StringGeneral.NullCode);
                        registryOld.SetAttribute(EmrConstant.AttributeNames.RegistryDate, row1.ItemArray[2].ToString().Split(' ')[0]);
                        registryOld.SetAttribute(EmrConstant.AttributeNames.DoctorID, row1.ItemArray[3].ToString());
                        registryOld.SetAttribute(EmrConstant.AttributeNames.DepartmentCode, row1.ItemArray[4].ToString());
                        registryOld.SetAttribute(EmrConstant.AttributeNames.DischargedDate, row1.ItemArray[5].ToString());
                        if (row1.ItemArray[6].ToString() == "1")
                        {
                            registryOld.SetAttribute(EmrConstant.AttributeNames.PatientStatus, EmrConstant.StringGeneral.Discharged + ":已归档");
                        }
                        else
                        {
                            registryOld.SetAttribute(EmrConstant.AttributeNames.PatientStatus, EmrConstant.StringGeneral.Discharged);
                        }
                        
                        patient.AppendChild(registryOld);
                    }
                }
                
                #endregion

                root.AppendChild(patient);
        

            }

            node = root;
            return true;
        }catch (Exception e)
        {
            node = DataError(e);
            return false;
        }

    }
    private void Jobs(ref DataTable jobs)
    {
        string select = "SELECT bh, name FROM batsxx WHERE (type = 'ZYXZ') ORDER BY bh";
        DataSet dataSetJobs = ExeuSentence(select, hisDBType);
         jobs = dataSetJobs.Tables[0];
        return;
      
    }

    [WebMethod(Description = "Return inpatient list", EnableSession = false)]
    public XmlNode PatientListFromRegistryIDs(XmlNode registryIDs)
    {
        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw " +
                        "FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh = '";
        xmldoc = new XmlDocument();
        XmlNode root = xmldoc.CreateElement(ElementNames.Patients); ;
        XmlNode tmp = null;
        try
        {
            foreach (XmlNode registryID in registryIDs.ChildNodes)
            {
                /* Execute sentence */
                string select = SQLSentence + registryID.InnerText + "'";
                if (!PatientProcess(select, ref tmp, false)) continue;
                if (tmp.FirstChild == null) continue;

                XmlElement one = xmldoc.CreateElement(ElementNames.RegistryID);
                one.InnerXml = tmp.InnerXml;
                root.AppendChild(one.FirstChild.Clone());
            }

            return root;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }


    /* --------------------------------------------------------------------------
     * 返回 XmlNode（出院患者名单）:
     * 参数:
     *      EmrConstant.QueryMode mode -- 选取方式
     * case mode = QueryMode.ArchiveNum: 
     *      criteria 是患者唯一标识号
     *      取指定患者的基本信息和住院信息
     * case mode = QueryMode.PatientName:
     *      criteria 是患者姓名，允许模糊，例如只给出姓氏
     *      取患者姓名以给定参数开头的患者名单
     * case mode = QueryMode.RegistryID:
     *      criteria 是患者住院登记号
     *      取指定一次住院信息及患者基本信息
     * case QueryMode.Commpose:
     *      criteria 是复合查询条件，具有如下格式：
     *          姓名|性别|开始日期|截至日期|医师编码
     *      姓名是准确的患者姓名，允许为空值，表示不限制
     *      性别取值 ‘1' 男 ‘2' 女 ‘Both' 不限制, 不准空值
     *      出院时间段，‘2008-01-23 00:00:00’和 ‘2008-06-23 23:59:59’，不准空值
     *      医师编码是责任医师，允许为空值，表示不限制
     * 说明：
     *     返回结果与 PatientList（）相同，不同处这里都是已经出院的记录
     * 出错返回：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="QueryPatientList" />
     *  </errors>
     ----------------------------------------------------------------------------*/
    [WebMethod(Description = "Return Query patient list for the discharged")]
 public XmlNode QueryPatientList(EmrConstant.QueryMode mode, string criteria, bool inStyle)
    // public XmlNode QueryPatientList( string criteria)
    {
        //EmrConstant.QueryMode mode = QueryMode.Commpose;
        //bool inStyle = true;


        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
        Jobs(ref jobs);

        string SQLSentence = null;
        xmldoc = new XmlDocument();
        DataSet dataset = null;
        XmlElement patients = xmldoc.CreateElement(EmrConstant.ElementNames.Patients);
        XmlElement patient = null;
        try
        {
            #region Make SQL sentence
            if (inStyle)
            {
                switch (mode)
                {
                    #region Archive Number
                    case QueryMode.ArchiveNum:
                        SQLSentence = @"SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz,
                            tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus 
                            FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.bah =  '" + criteria + "' ";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah like  '%" + criteria + "'";
                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistryEx(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }
                        else
                        {

                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistry(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);

                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw 
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah like  '%" + criteria + "'";
                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistryEx(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }

                        break;
                    #endregion


                    #region Patient name
                    case QueryMode.PatientName:
                        string[] item = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string sqlCon="";
                        string sqlCons="";
                        string criter = criteria;
                        if (item.Length > 2)
                        {
                            sqlCon = "  tdjkz.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                             sqlCons = "  tdjk.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                             criter = item[2];

                        }
                        string strSelect = "SELECT  DISTINCT  tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                 "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                 "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";
                        strSelect += sqlCon ;
                        /**20110831 zzl**/
                        if (!string.IsNullOrEmpty(criter) && !string.IsNullOrEmpty(sqlCon))
                        {
                            strSelect += " and tdjkz.xmpy like '%" + criter + "%'";
                        }
                        else 
                        {
                            if (!string.IsNullOrEmpty(criter))
                            {
                                strSelect += " tdjkz.xmpy like '%" + criter + "%' ";
                            }                            
                        }
                        strSelect += " order by tdjkz.zyrq desc";

                           DataSet ds = ExeuSentence(strSelect, hisDBType);
                        
                          if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                          {
                              foreach (DataRow dr in ds.Tables[0].Rows)
                              {
                                  patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                  OnePatient(dr, inStyle, ref patient, jobs);
                             
                                      OneRegistryEx(dr, inStyle, ref patient);
                                      patients.AppendChild(patient);                                
                              }
                          }
                          SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                                 "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                                 "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                          SQLSentence += sqlCons; //+ " and tdjk.xmpy = '" + criter + "'order by tdjk.zyrq desc";
                          if (!string.IsNullOrEmpty(criter) && !string.IsNullOrEmpty(sqlCons))
                          {

                              SQLSentence += " and tdjk.xmpy like '%" + criter + "%' ";
                          }
                          else
                          {
                              if (!string.IsNullOrEmpty(criter))
                              {
                                  SQLSentence += " tdjk.xmpy like '%" + criter + "%' ";
                              }
                          }
                          SQLSentence += " order by tdjk.zyrq desc";
                          ds = ExeuSentence(SQLSentence, hisDBType);
                          if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                          {
                              foreach (DataRow dr in ds.Tables[0].Rows)
                              {
                                  patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                  OnePatient(dr, inStyle, ref patient, jobs);

                                  OneRegistry(dr, inStyle, ref patient);
                                  patients.AppendChild(patient);


                              }
                          }
                        break;
                        
                        SQLSentence = "SELECT DISTINCT tdjk.bah FROM tdjk WHERE tdjk.xmpy LIKE '%" + criteria + "%' ";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            string strSelect2 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%'";
                            DataSet ds2 = ExeuSentence(strSelect2, hisDBType);
                            if (ds2.Tables.Count == 0) break;
                            if (ds2.Tables[0].Rows.Count == 0) break;

                            strSelect2 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect + "tdjkz.bah='" + archiveNum + "' ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }
                        }
                        else
                        {
                            SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                                "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                                "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                string archiveNum = row.ItemArray[0].ToString();
                                string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "' ORDER BY tdjk.zyh DESC";
                                dataset = ExeuSentence(sentence, hisDBType);
                                if (dataset.Tables.Count == 0) continue;
                                if (dataset.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in dataset.Tables[0].Rows)
                                {
                                    OneRegistry(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }



                            string strSelect3 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%'";
                            DataSet ds3 = ExeuSentence(strSelect3, hisDBType);
                            if (ds3.Tables.Count == 0) break;
                            if (ds3.Tables[0].Rows.Count == 0) break;

                            strSelect3 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds3.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect3 + "tdjkz.bah='" + archiveNum + "' ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }


                        }




                        break;
                       
                    #endregion
                    #region All Patient name
                    case QueryMode.AllPatientName:
                        string[] item2 = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string sqlCon2 = "";
                        string sqlCons2 = "";
                        string criter2 = criteria;
                        if (item2.Length > 2)
                        {
                            sqlCon2 = "  tdjkz.zyrq BETWEEN '" + item2[0] + "' AND '" + item2[1] + "'";
                            sqlCons2 = "  tdjk.zyrq BETWEEN '" + item2[0] + "' AND '" + item2[1] + "'";
                            criter2 = item2[2];

                        }
                        string strSelectAll = "SELECT  DISTINCT  tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                 "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                 "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";
                        strSelectAll += sqlCon2;
                        /**20110831 zzl**/
                        if (!string.IsNullOrEmpty(criter2) && !string.IsNullOrEmpty(sqlCon2))
                        {
                            strSelectAll += " and tdjkz.xm = '" + criter2 + "'";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter2))
                            {
                                strSelectAll += " tdjkz.xm= '" + criter2 + "' ";
                            }
                        }
                        strSelectAll += " order by tdjkz.zyrq desc";

                        DataSet dsAll = ExeuSentence(strSelectAll, hisDBType);

                        if (dsAll.Tables.Count != 0 && dsAll.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in dsAll.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistryEx(dr, inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                               "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                               "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                               "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        SQLSentence += sqlCons2; //+ " and tdjk.xmpy = '" + criter + "'order by tdjk.zyrq desc";
                        if (!string.IsNullOrEmpty(criter2) && !string.IsNullOrEmpty(sqlCons2))
                        {

                            SQLSentence += " and tdjk.xm= '" + criter2 + "' ";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter2))
                            {
                                SQLSentence += " tdjk.xm='" + criter2 + "' ";
                            }
                        }
                        SQLSentence += " order by tdjk.zyrq desc";
                        dsAll = ExeuSentence(SQLSentence, hisDBType);
                        if (dsAll.Tables.Count != 0 && dsAll.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in dsAll.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistry(dr, inStyle, ref patient);
                                patients.AppendChild(patient);


                            }
                        }
                         break;

                        SQLSentence = "SELECT DISTINCT tdjk.bah FROM tdjk WHERE tdjk.xm= '" + criteria + "' ";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            string strSelectAll2 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xm= '" + criteria + "'";
                            DataSet dsAll2 = ExeuSentence(strSelectAll2, hisDBType);
                            if (dsAll2.Tables.Count == 0) break;
                            if (dsAll2.Tables[0].Rows.Count == 0) break;

                            strSelectAll2 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in dsAll2.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelectAll2 + "tdjkz.bah='" + archiveNum + "' ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }
                        }
                        else
                        {
                            SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                                "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                                "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                string archiveNum = row.ItemArray[0].ToString();
                                string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "' ORDER BY tdjk.zyh DESC";
                                dataset = ExeuSentence(sentence, hisDBType);
                                if (dataset.Tables.Count == 0) continue;
                                if (dataset.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in dataset.Tables[0].Rows)
                                {
                                    OneRegistry(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }



                            string strSelect3 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%'";
                            DataSet ds3 = ExeuSentence(strSelect3, hisDBType);
                            if (ds3.Tables.Count == 0) break;
                            if (ds3.Tables[0].Rows.Count == 0) break;

                            strSelect3 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds3.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect3 + "tdjkz.bah='" + archiveNum + "' ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }


                        }




                        break;

                          #endregion
                    #region RegistryID
                    case QueryMode.RegistryID:

                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.zyh like  '%" + criteria + "'";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistry(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }


                        SQLSentence = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                           "tdjkz.zyh, tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                           "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                           "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.zyh like  '%" + criteria + "'";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistryEx(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        break;
                    #endregion

                    #region Commpose
                    case QueryMode.Commpose:
                        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string whereSentence = null;
                        SQLSentence = "SELECT DISTINCT bah, xb, xm FROM tdjk WHERE ";
                        if (items[0].Length > 0)
                            whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
                        if (items[1] != EmrConstant.StringGeneral.Both)
                        {
                            whereSentence += "tdjk.xb='" + items[1] + "' AND ";
                        }
                        whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                        if (items[4].Length > 0) whereSentence += " AND tdjk.zrys='" + items[4] + "'";
                        if (items[5].Length > 0) whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
                        if (items.Length >= 7)
                        {
                            switch (items[6])
                            {
                                case "1":
                                    whereSentence += " AND  tdjk.emrstatus= 1";
                                    break;
                                case "2":
                                    whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                                    break;
                                case "3":
                                    break;
                            }
                        }
                        whereSentence += " ORDER BY tdjk.xb, tdjk.xm";
                        SQLSentence += whereSentence;
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0) break;
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        foreach (DataRow row in dataset.Tables[0].Rows)
                        {
                            string archiveNum = row.ItemArray[0].ToString();
                            string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "'";
                            DataSet dataset1 = ExeuSentence(sentence, hisDBType);
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset1.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row1 in dataset1.Tables[0].Rows)
                            {
                                OneRegistry(row1, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }



                        break;
                    #endregion
                }
            }
            else
            {
                #region OutPatient
                switch (mode)
                {
                    case QueryMode.ArchiveNum:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE ylzh = '" + criteria + "'";
                        break;
                    case QueryMode.PatientName:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE hzxm like '" + criteria + "%'";
                        break;
                    case QueryMode.RegistryID:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE mzh = '" + criteria + "'";
                        break;
                }
                #endregion
            }
            #endregion
            /* Execute sentence */
            return patients;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    //20140227
    [WebMethod(Description = "Return Query patient list for the discharged")]
    public XmlNode QueryPatientListJH(EmrConstant.QueryMode mode, string criteria, bool inStyle,string departID)
    // public XmlNode QueryPatientList( string criteria)
    {
        //EmrConstant.QueryMode mode = QueryMode.Commpose;
        //bool inStyle = true;


        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
        Jobs(ref jobs);

        string SQLSentence = null;
        xmldoc = new XmlDocument();
        DataSet dataset = null;
        XmlElement patients = xmldoc.CreateElement(EmrConstant.ElementNames.Patients);
        XmlElement patient = null;
        try
        {
            #region Make SQL sentence
            if (inStyle)
            {
                switch (mode)
                {
                    #region Archive Number
                    case QueryMode.ArchiveNum:
                        SQLSentence = @"SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz,
                            tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus 
                            FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.bah =  '" + criteria + "' and tdjk.ksbm='"+departID+"' ";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah like  '%" + criteria + "' and tdjkz.ksbm='" + departID + "' ";
                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistryEx(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }
                        else
                        {

                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistry(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);

                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw 
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah like  '%" + criteria + "' and tdjkz.ksbm='" + departID + "' ";
                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistryEx(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }

                        break;
                    #endregion


                    #region Patient name
                    case QueryMode.PatientName:
                        string[] item = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string sqlCon = "";
                        string sqlCons = "";
                        string criter = criteria;
                        if (item.Length > 2)
                        {
                            sqlCon = "  tdjkz.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                            sqlCons = "  tdjk.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                            criter = item[2];

                        }
                        string strSelect = "SELECT  DISTINCT  tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                 "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                 "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";
                        strSelect += sqlCon;
                        /**20110831 zzl**/
                        if (!string.IsNullOrEmpty(criter) && !string.IsNullOrEmpty(sqlCon))
                        {
                            strSelect += " and tdjkz.xmpy like '%" + criter + "%' and tdjkz.ksbm='" + departID + "' ";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter))
                            {
                                strSelect += " tdjkz.xmpy like '%" + criter + "%'  and tdjkz.ksbm='" + departID + "' ";
                            }
                        }
                        strSelect += " order by tdjkz.zyrq desc";

                        DataSet ds = ExeuSentence(strSelect, hisDBType);

                        if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistryEx(dr, inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                               "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                               "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                               "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        SQLSentence += sqlCons; //+ " and tdjk.xmpy = '" + criter + "'order by tdjk.zyrq desc";
                        if (!string.IsNullOrEmpty(criter) && !string.IsNullOrEmpty(sqlCons))
                        {

                            SQLSentence += " and tdjk.xmpy like '%" + criter + "%' and tdjk.ksbm='" + departID + "'";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter))
                            {
                                SQLSentence += " tdjk.xmpy like '%" + criter + "%' and tdjk.ksbm='" + departID + "' ";
                            }
                        }
                        SQLSentence += " order by tdjk.zyrq desc";
                        ds = ExeuSentence(SQLSentence, hisDBType);
                        if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistry(dr, inStyle, ref patient);
                                patients.AppendChild(patient);


                            }
                        }
                        break;

                        SQLSentence = "SELECT DISTINCT tdjk.bah FROM tdjk WHERE tdjk.xmpy LIKE '%" + criteria + "%' and tdjk.ksbm='" + departID + "' ";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            string strSelect2 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%' and tdjkz.ksbm='" + departID + "' ";
                            DataSet ds2 = ExeuSentence(strSelect2, hisDBType);
                            if (ds2.Tables.Count == 0) break;
                            if (ds2.Tables[0].Rows.Count == 0) break;

                            strSelect2 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect + "tdjkz.bah='" + archiveNum + "' and tdjkz.ksbm='" + departID + "'  ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }
                        }
                        else
                        {
                            SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                                "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                                "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                string archiveNum = row.ItemArray[0].ToString();
                                string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "' and tdjk.ksbm='" + departID + "'  ORDER BY tdjk.zyh DESC";
                                dataset = ExeuSentence(sentence, hisDBType);
                                if (dataset.Tables.Count == 0) continue;
                                if (dataset.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in dataset.Tables[0].Rows)
                                {
                                    OneRegistry(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }



                            string strSelect3 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%' and tdjk.ksbm='" + departID + "' ";
                            DataSet ds3 = ExeuSentence(strSelect3, hisDBType);
                            if (ds3.Tables.Count == 0) break;
                            if (ds3.Tables[0].Rows.Count == 0) break;

                            strSelect3 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds3.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect3 + "tdjkz.bah='" + archiveNum + "' and tdjkz.ksbm='" + departID + "'  ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }


                        }




                        break;

                    #endregion
                    #region All Patient name
                    case QueryMode.AllPatientName:
                        string[] item2 = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string sqlCon2 = "";
                        string sqlCons2 = "";
                        string criter2 = criteria;
                        if (item2.Length > 2)
                        {
                            sqlCon2 = "  tdjkz.zyrq BETWEEN '" + item2[0] + "' AND '" + item2[1] + "'";
                            sqlCons2 = "  tdjk.zyrq BETWEEN '" + item2[0] + "' AND '" + item2[1] + "'";
                            criter2 = item2[2];

                        }
                        string strSelectAll = "SELECT  DISTINCT  tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                 "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                 "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";
                        strSelectAll += sqlCon2;
                        /**20110831 zzl**/
                        if (!string.IsNullOrEmpty(criter2) && !string.IsNullOrEmpty(sqlCon2))
                        {
                            strSelectAll += " and tdjkz.xm = '" + criter2 + "' and tdjkz.ksbm='" + departID + "' ";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter2))
                            {
                                strSelectAll += " tdjkz.xm= '" + criter2 + "' and tdjkz.ksbm='" + departID + "' ";
                            }
                        }
                        strSelectAll += " order by tdjkz.zyrq desc";

                        DataSet dsAll = ExeuSentence(strSelectAll, hisDBType);

                        if (dsAll.Tables.Count != 0 && dsAll.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in dsAll.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistryEx(dr, inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                               "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                               "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                               "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        SQLSentence += sqlCons2; //+ " and tdjk.xmpy = '" + criter + "'order by tdjk.zyrq desc";
                        if (!string.IsNullOrEmpty(criter2) && !string.IsNullOrEmpty(sqlCons2))
                        {

                            SQLSentence += " and tdjk.xm= '" + criter2 + "' and tdjk.ksbm='" + departID + "' ";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(criter2))
                            {
                                SQLSentence += " tdjk.xm='" + criter2 + "' and tdjk.ksbm='" + departID + "' ";
                            }
                        }
                        SQLSentence += " order by tdjk.zyrq desc";
                        dsAll = ExeuSentence(SQLSentence, hisDBType);
                        if (dsAll.Tables.Count != 0 && dsAll.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in dsAll.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistry(dr, inStyle, ref patient);
                                patients.AppendChild(patient);


                            }
                        }
                        break;

                        SQLSentence = "SELECT DISTINCT tdjk.bah FROM tdjk WHERE tdjk.xm= '" + criteria + "' and tdjk.ksbm='" + departID + "' ";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {
                            string strSelectAll2 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xm= '" + criteria + "' and tdjkz.ksbm='" + departID + "' ";
                            DataSet dsAll2 = ExeuSentence(strSelectAll2, hisDBType);
                            if (dsAll2.Tables.Count == 0) break;
                            if (dsAll2.Tables[0].Rows.Count == 0) break;

                            strSelectAll2 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in dsAll2.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelectAll2 + "tdjkz.bah='" + archiveNum + "'and tdjkz.ksbm='" + departID + "'   ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }
                        }
                        else
                        {
                            SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                                "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                                "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                string archiveNum = row.ItemArray[0].ToString();
                                string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "' and tdjk.ksbm='" + departID + "'  ORDER BY tdjk.zyh DESC";
                                dataset = ExeuSentence(sentence, hisDBType);
                                if (dataset.Tables.Count == 0) continue;
                                if (dataset.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in dataset.Tables[0].Rows)
                                {
                                    OneRegistry(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }



                            string strSelect3 = "SELECT DISTINCT tdjkz.bah FROM tdjkz WHERE tdjkz.xmpy LIKE '%" + criteria + "%' and tdjkz.ksbm='" + departID + "' ";
                            DataSet ds3 = ExeuSentence(strSelect3, hisDBType);
                            if (ds3.Tables.Count == 0) break;
                            if (ds3.Tables[0].Rows.Count == 0) break;

                            strSelect3 = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                                "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";

                            foreach (DataRow dr in ds3.Tables[0].Rows)
                            {
                                string archiveNum = dr.ItemArray[0].ToString();
                                string sentence = strSelect3 + "tdjkz.bah='" + archiveNum + "' and tdjkz.ksbm='" + departID + "' ORDER BY tdjkz.zyh DESC";
                                ds = ExeuSentence(sentence, hisDBType);
                                if (ds.Tables.Count == 0) continue;
                                if (ds.Tables[0].Rows.Count == 0) continue;
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(ds.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                foreach (DataRow row1 in ds.Tables[0].Rows)
                                {
                                    OneRegistryEx(row1, inStyle, ref patient);
                                }
                                patients.AppendChild(patient);
                            }


                        }




                        break;

                    #endregion
                    #region RegistryID
                    case QueryMode.RegistryID:

                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.zyh like  '%" + criteria + "' and tdjk.ksbm='" + departID + "' ";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistry(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }


                        SQLSentence = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                           "tdjkz.zyh, tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw " +
                           "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                           "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.zyh like  '%" + criteria + "' and tdjkz.ksbm='" + departID + "' ";
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistryEx(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        break;
                    #endregion

                    #region Commpose
                    case QueryMode.Commpose:
                        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string whereSentence = null;
                        SQLSentence = "SELECT DISTINCT bah, xb, xm FROM tdjk WHERE ";
                        if (items[0].Length > 0)
                            whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
                        if (items[1] != EmrConstant.StringGeneral.Both)
                        {
                            whereSentence += "tdjk.xb='" + items[1] + "' AND ";
                        }
                        whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                        if (items[4].Length > 0) whereSentence += " AND tdjk.zrys='" + items[4] + "'";
                        if (items[5].Length > 0) whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
                        if (items.Length >= 7)
                        {
                            switch (items[6])
                            {
                                case "1":
                                    whereSentence += " AND  tdjk.emrstatus= 1";
                                    break;
                                case "2":
                                    whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                                    break;
                                case "3":
                                    break;
                            }
                        }
                        whereSentence += " and tdjk.ksbm='" + departID + "'  ORDER BY tdjk.xb, tdjk.xm";
                        SQLSentence += whereSentence;
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0) break;
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        foreach (DataRow row in dataset.Tables[0].Rows)
                        {
                            string archiveNum = row.ItemArray[0].ToString();
                            string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "' and tdjk.ksbm='" + departID + "' ";
                            DataSet dataset1 = ExeuSentence(sentence, hisDBType);
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset1.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row1 in dataset1.Tables[0].Rows)
                            {
                                OneRegistry(row1, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }



                        break;
                    #endregion
                }
            }
            else
            {
                #region OutPatient
                switch (mode)
                {
                    case QueryMode.ArchiveNum:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE ylzh = '" + criteria + "'";
                        break;
                    case QueryMode.PatientName:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE hzxm like '" + criteria + "%'";
                        break;
                    case QueryMode.RegistryID:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE mzh = '" + criteria + "'";
                        break;
                }
                #endregion
            }
            #endregion
            /* Execute sentence */
            return patients;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    [WebMethod(Description = "Return Query patient list for the discharged")]
    public XmlNode CommposePatientList(string criteria, bool archive)
    {
        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
        Jobs(ref jobs);

        string SQLSentence = null;
        xmldoc = new XmlDocument();
        DataSet dataset = null;
        XmlElement patients = xmldoc.CreateElement(EmrConstant.ElementNames.Patients);
        XmlElement patient = null;
        #region Commpose

        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
        string whereSentence = null;
        SQLSentence = "SELECT DISTINCT bah, xb, xm FROM tdjk WHERE ";
        if (items[0].Length > 0)
            whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
        if (items[1] != EmrConstant.StringGeneral.Both)
        {
            whereSentence += "tdjk.xb='" + items[1] + "' AND ";
        }
        if (archive)
        {
            if (items.Length >= 7 && items[6] != "2")
                whereSentence += "tdjk.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
            if (items.Length >= 7 && items[6] == "2")
            {
                whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                whereSentence += " AND zyrq > '2011-03-01 00:00:00'";
            }
        }
        else whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
        if (items[4].Length > 0) whereSentence += " AND tdjk.zrys='" + items[4] + "'";
        if (items[5].Length > 0) whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
        if (items.Length >= 7)
        {
            switch (items[6])
            {
                case "1":
                    whereSentence += " AND  tdjk.emrstatus= 1";
                    break;
                case "2":
                    whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                    break;
                case "3":
                    break;
            }
        }
        whereSentence += " ORDER BY tdjk.xb, tdjk.xm";
        SQLSentence += whereSentence;
        dataset = ExeuSentence(SQLSentence, hisDBType);
        if (dataset.Tables.Count == 0) return patients;
        if (dataset.Tables[0].Rows.Count == 0) return patients;
        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
        foreach (DataRow row in dataset.Tables[0].Rows)
        {
            string archiveNum = row.ItemArray[0].ToString();
            string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "'";
            DataSet dataset1 = ExeuSentence(sentence, hisDBType);
            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
            OnePatient(dataset1.Tables[0].Rows[0], true, ref patient, jobs);
            foreach (DataRow row1 in dataset1.Tables[0].Rows)
            {
                OneRegistry(row1, true, ref patient);
            }
            patients.AppendChild(patient);
        }



        return patients;
        #endregion
    }
    private void OnePatient(DataRow row, bool inStyle, ref XmlElement patient, DataTable jobs)
    {
        patient.SetAttribute(EmrConstant.AttributeNames.ArchiveNum, row.ItemArray[0].ToString());
        patient.SetAttribute(EmrConstant.AttributeNames.PatientName, row.ItemArray[1].ToString());

        if (inStyle)
        {
            patient.SetAttribute(EmrConstant.AttributeNames.Sex, GetSexName(row.ItemArray[2].ToString()));
            string birth = row.ItemArray[3].ToString();
            if (birth.Length == 0) birth = DateTime.Today.ToString(EmrConstant.StringGeneral.DateFormat);
            patient.SetAttribute(EmrConstant.AttributeNames.Birth, birth.Split(' ')[0]);
            patient.SetAttribute(AttributeNames.Age, row.ItemArray[15].ToString().Split('.')[0]);
            string ageUnit = "岁";
            if (!Convert.IsDBNull(row.ItemArray[16])) ageUnit = row.ItemArray[16].ToString();
            patient.SetAttribute(AttributeNames.AgeUnit, ageUnit);
        }
        else
        {
            patient.SetAttribute(EmrConstant.AttributeNames.Sex, row.ItemArray[2].ToString());
            double dbAge = Convert.ToDouble(row.ItemArray[3]);
            TimeSpan age = TimeSpan.FromDays(365 * dbAge);
            DateTime tmp = DateTime.Today.Subtract(age);
            patient.SetAttribute(EmrConstant.AttributeNames.Birth, tmp.ToString().Split(' ')[0]);
        }

        patient.SetAttribute(EmrConstant.AttributeNames.Nation, row.ItemArray[4].ToString());
        patient.SetAttribute(EmrConstant.AttributeNames.MaritalStatus, row.ItemArray[5].ToString());
        patient.SetAttribute(EmrConstant.AttributeNames.NativePlace, row.ItemArray[6].ToString());

        DataRow[] rowsJob = jobs.Select("bh='" + row.ItemArray[7].ToString() + "'");
        if (rowsJob.Length == 1)
            patient.SetAttribute(EmrConstant.AttributeNames.Job, rowsJob[0][1].ToString());
        else
            patient.SetAttribute(EmrConstant.AttributeNames.Job, row.ItemArray[7].ToString());
        patient.SetAttribute(EmrConstant.AttributeNames.Address, row.ItemArray[8].ToString());
    }
    private void OneRegistry(DataRow row, bool inStyle, ref XmlElement patient)
    {
        XmlElement registry = patient.OwnerDocument.CreateElement(EmrConstant.ElementNames.Registry);
        registry.SetAttribute(EmrConstant.AttributeNames.RegistryID, row.ItemArray[9].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.BedNum, row.ItemArray[10].ToString());
        string[] items = row.ItemArray[11].ToString().Split(' ');
        if (items.Length == 2)
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, items[0]);
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, items[1]);
        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, "");
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, "");
        }
        registry.SetAttribute(EmrConstant.AttributeNames.DoctorID, row.ItemArray[12].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.DepartmentCode, row.ItemArray[13].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.DischargedDate, row.ItemArray[14].ToString());
        if (row.ItemArray[17].ToString() == "1")
        {
            registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, EmrConstant.StringGeneral.Discharged + ":已归档");
        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, EmrConstant.StringGeneral.Discharged);
        }
        patient.AppendChild(registry);
    }
    private void OneRegistryEx(DataRow row, bool inStyle, ref XmlElement patient)
    {
        XmlElement registry = patient.OwnerDocument.CreateElement(EmrConstant.ElementNames.Registry);
        registry.SetAttribute(EmrConstant.AttributeNames.RegistryID, row.ItemArray[9].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.BedNum, row.ItemArray[10].ToString());
        //registry.SetAttribute(EmrConstant.AttributeNames.BedNum, "床3");
        string[] items = row.ItemArray[11].ToString().Split(' ');
        if (items.Length == 2)
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, items[0]);
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, items[1]);
           
        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, "");
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, "");
          
        }
        registry.SetAttribute(EmrConstant.AttributeNames.DoctorID, row.ItemArray[12].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.DepartmentCode, row.ItemArray[13].ToString());
        if (row.ItemArray[14].ToString() != "")
        {
            registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, "出科");
        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, "");
        }
        registry.SetAttribute(EmrConstant.AttributeNames.DischargedDate, row.ItemArray[14].ToString());
       
        patient.AppendChild(registry);
    }
    [WebMethod]
    public string  GetGroupOwner(string strDoctorID)
    {
        SqlHelper helper = new SqlHelper("HisDB");
        string strSelect = "select zlzz from tysm where ysbm = '" + strDoctorID + "' and zlzz is not null ";
        string result = "";
        DataTable dt = helper.GetDataTable(strSelect);
        try
        {
           
            if (dt != null && dt.Rows.Count != 0)
            {
                result = dt.Rows[0]["zlzz"].ToString();
            }
        }
        catch (Exception ex)
        {
            result = null;
        }
        return result;

    }
    [WebMethod(EnableSession = true, MessageName = "GetOperatorName1")]
    public bool GetOperatorName(string opcode, ref string opname, ref string passwd, ref string department,ref string departname)
    {
        if (!AuthenticChek(opcode, ref opname, ref passwd)) return Return.Failed;

        string select = "SELECT ksbm,ksmc FROM tysm WHERE ysbm = '" + opcode + "'";
        DataSet ds = ExeuSentence(select, hisDBType);
        if (ds.Tables.Count == 0) return Return.Successful;
        if (ds.Tables[0].Rows.Count != 1) return Return.Successful;

        department = ds.Tables[0].Rows[0]["ksbm"].ToString();
        departname = ds.Tables[0].Rows[0]["ksmc"].ToString();
        return true;
    }
    //2014-02-27
    [WebMethod(EnableSession = true, MessageName = "GetOperatorNameBq")]
    public bool GetOperatorNameBq(string opcode, ref string opname, ref string passwd, ref string department, ref string departname, ref string AreaCode)
    {
        if (!AuthenticChek(opcode, ref opname, ref passwd)) return Return.Failed;

        string select = "SELECT ksbm,ksmc FROM tysm WHERE ysbm = '" + opcode + "'";
        DataSet ds = ExeuSentence(select, hisDBType);
        if (ds.Tables.Count == 0) return Return.Successful;
        if (ds.Tables[0].Rows.Count != 1) return Return.Successful;

        department = ds.Tables[0].Rows[0]["ksbm"].ToString();
        departname = ds.Tables[0].Rows[0]["ksmc"].ToString();

        string bqSelect = "select BQBM from bq_ks_dzb where ksbm='"+department+"'";
        DataSet ds2 = ExeuSentence(bqSelect, hisDBType);
        if (ds2.Tables.Count != 0 && ds2.Tables[0].Rows.Count!=0)
        AreaCode = ds2.Tables[0].Rows[0]["BQBM"].ToString();
    
        return true;
    }
    [WebMethod(EnableSession = true, MessageName = "GetOperatorName2")]
    public bool GetOperatorName(string opcode, ref string opname, ref string passwd, ref string department)
    {
        if (!AuthenticChek(opcode, ref opname, ref passwd)) return Return.Failed;

        string select = "SELECT ksbm FROM tysm WHERE ysbm = '" + opcode + "'";
        DataSet ds = ExeuSentence(select, hisDBType);
        if (ds.Tables.Count == 0) return Return.Successful;
        if (ds.Tables[0].Rows.Count != 1) return Return.Successful;

        department = ds.Tables[0].Rows[0]["ksbm"].ToString();
       
        return true;
    }
    /* -----------------------------------------------------------------------------
     * 取一操作员的姓名和注册口令
     * 参数:
     *      userCode 操作员编码，医师注册ID
     * [ref]userName 返回操作员姓名
     * [ref]passwd   返回操作员注册口令，平码
     * 返回 true 如果成功
     * 返回 false 如果userCode无效
     -------------------------------------------------------------------------------*/
    [WebMethod(Description = "Check authentication for a user")]
    public bool AuthenticChek(string opcode, ref string opname, ref string passwd)
    {
        string select = null;
        if (dbcf == StringGeneral.Yes)
            select = "SELECT czyxm, czymm FROM sj_czydm WHERE ZXBS is null and  czydm = '" + opcode + "'";
        else
            select = "SELECT czyxm, mm FROM sczymm  WHERE ZXBS is null and  czydm = '" + opcode + "'";
        DataSet ds = ExeuSentence(select, hisDBType);
        if (ds.Tables.Count == 0) return false;
        if (ds.Tables[0].Rows.Count != 1) return false;

        opname = ds.Tables[0].Rows[0][0].ToString();
        if (dbcf == StringGeneral.Yes) passwd = NewDecodePasswd(ds.Tables[0].Rows[0][1].ToString());
        else passwd = OldDecodePasswd(ds.Tables[0].Rows[0][1].ToString());
        return true;
    }
    private string OldDecodePasswd(string encodedPasswd)
    {
        int o = 1;
        string[] ms = new string[] {"1987234056", "3198407256", "5407321896", "3251807496",
                                  "3819254076", "2407653198", "0319728456", "8409567231"};
        string str = encodedPasswd;
        while (o <= str.Trim().Length)
        {
            string ss = str.Substring(o - 1, 1);
            sbyte x = (sbyte)ss[0];
            int y = (int)((ulong)x - 48);
            ss = ms[o - 1].Substring(y, 1);
            str = str.Remove(o - 1, 1);
            str = str.Insert(o - 1, ss);

            o++;
        }
        return str;
    }
    //[DllImport("GSJEncrypt.dll", EntryPoint = "DecodeString", CharSet = CharSet.Auto,
    //    CallingConvention = CallingConvention.StdCall)]
  //  private static extern int DecodeString(char[] encodedPasswd, ref byte[] passwd);
    private static string NewDecodePasswd(string encodedPasswd)
    {
        string passwd = null;
        CSJEncrypt.SjEncrypt.DecodeString(encodedPasswd, ref passwd);
        return passwd;
    }
    private static string CjjToString(byte[] bytes)
    {
        string tmp = null;
        for (int k = 0; k < bytes.Length; k++)
        {
            tmp += Convert.ToString(Convert.ToChar(bytes[k]));
        }
        return tmp;
    }

    /* ---------------------------------------------------------------------------
     * 返回 XmlNode: （医院科室名单）
     *    <Departments>     
     *         <Department Code="0100" Name="呼吸内科" />
     *         <Department Code="0101" Name="心内科" />
     *    </Departments>
     * 参数：
     *     EmrConstant.WorkMode mode 指定科室服务性质
     * case mode = WorkMode.InHospital:
     *     取住院部科室名单
     * case mode = WorkMode.OutHospital:
     *     取门诊科室名单
     * 
     * 出错：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="GetDepartmentListByMode" />
     *  </errors>
     ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns department list")]
    public XmlNode GetDepartmentListByMode(EmrConstant.WorkMode mode)
    {
        string SQLSentence = null;
        switch (mode)
        {
            case WorkMode.InHospital:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND " +
                    "ksty is null ORDER BY ksbm";
                //SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND " +
                //    "(ghbs is null OR ghbs = '0') AND ksty is null ORDER BY ksmc";
                break;
            case WorkMode.OutHospital:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE ghbs = '1' AND ksty is null ORDER BY ksbm";
                break;
            case WorkMode.Service:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE kslx = '5' AND ksty is null ORDER BY ksbm";
                break;
        }

        try
        {
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlNode departmentsNode = xmldoc.CreateElement(ElementNames.Departments);
            //if (dataset.Tables.Count == 0) return EmrConstant.Return.Failed;
            if (dataset.Tables[0].Rows.Count == 0) return departmentsNode;
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement department = xmldoc.CreateElement(ElementNames.Department);
                department.SetAttribute(AttributeNames.Code, row.ItemArray[0].ToString());
                department.SetAttribute(AttributeNames.Name, row.ItemArray[1].ToString());
                departmentsNode.AppendChild(department);
            }
            return departmentsNode;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    /* ---------------------------------------------------------------------------
     * 返回 string: （指定科室所在病区编码）
     * 参数：
     *     departmentCode 科室编码
     * 
     * 出错：
     *     返回 null
     ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns department list")]
    public string GetAreaCodeOfDepartment(string departmentCode)
    {
        string SQLSentence = "SELECT bqbm FROM mz_ksbm WHERE ksbm='" + departmentCode + "'";
        try
        {
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            if (dataset.Tables[0].Rows.Count == 0) return null;
            return dataset.Tables[0].Rows[0][0].ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }
    /// <summary>
    /// 2009/6/8
    /// </summary>
    /// <param name="departmentCode"></param>
    /// <returns></returns>
    [WebMethod(Description = "Returns department Name")]
    public string GetDepartmentName(string departmentCode)
    {
        SqlHelper Helper = new SqlHelper("HisDB");
        string SQLSentence = "SELECT ksmc FROM mz_ksbm WHERE ksbm='" + departmentCode + "'";
        try
        {
            string Name = "";
            DataTable dt = Helper.GetDataTable(SQLSentence);
            if (dt != null && dt.Rows.Count != 0)
            {
                 Name = dt.Rows[0]["ksmc"].ToString();
            }
            return Name;
        }
        catch (Exception)
        {
            return null;
        }
    }


    /* ---------------------------------------------------------------------------
     * 返回 XmlNode: （医院病区单）
     *    <Areas>     
     *         <Area Code="01" Name="一病区" />
     *         <Area Code="02" Name="二病区" />
     *    </Areas>
     * 
     * 出错：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="GetAreas" />
     *  </errors>
     ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns area list")]
    public XmlNode GetAreas()
    {
        string SQLSentence = "SELECT bqbm, bqmc FROM bqbmk";
        try
        {
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlNode areas = xmldoc.CreateElement(ElementNames.Areas);
            if (dataset.Tables[0].Rows.Count == 0) return areas;
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement area = xmldoc.CreateElement(ElementNames.Area);
                area.SetAttribute(AttributeNames.Code, row.ItemArray[0].ToString());
                area.SetAttribute(AttributeNames.Name, row.ItemArray[1].ToString());
                areas.AppendChild(area);
            }
            return areas;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    /* ---------------------------------------------------------------------------
     * Return a XmlNode:
     *   <TestsAndExams xmlns="">
     *     <Form Num="152939" Type="test" Name="肝功能" Doctor="王莉" Date="2007-01-12">
     *          <Item Name="白蛋白" Value="200" Unit="g/L" Result="+" />
     *          <Item Name="球蛋白" Value="200" Unit="g/L" Result="-" />
     *     </Form>
     *     <Form Num="152940" Type="test" Name="尿常规" Doctor=" " Date="" >
     *          <Item Name="糖" Value="" Unit="" Result="3+" />
     *          <Item Name="酮体" Value="" Unit="" Result="-" />
     *     </Form>
     *     <Form Num="152944" Type="exam" Name="胃镜" Doctor=" " Date="" >
     *          <Item Name="胃镜检查" Value="" Unit="" Result="未发现异常" />
     *     </Form>
     *   </TestsAndExams>
     * 说明：元素 Form 表示检验检查单，属性：
     *            Num     处方号，允许空值
     *            Type    化验还是检查  ="test" 化验 ="exam" 检查
     *            Name    检验检查单名称
     *            Doctor  开单医师姓名，允许空值
     *            Date    检验检查日期，允许空值
     *       元素 Item 表示检验项目或检查项目，属性：
     *            Name    检验项目名称或检查项目名称
     *            Value   检验获得数值，允许空值
     *            Unit    数值单位， 允许空值
     *            Result  判断检验结果，或描述检查结果
     * 
     * 参数：
     *     string registryID 住院登记号或门诊登记号
     *     bool inStyle      =true 住院患者； =false 门诊患者
     ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns Tests and Exams", EnableSession = false)]
    public XmlNode GetTestsAndExams(string registryID, bool inStyle)
    {
        try
        {
            #region Make SQL sentence
            string SQLSentence = null;
            string SQLSentence2 = null;
            if (inStyle)
            {
                SQLSentence =
                    "SELECT kd_mx.xh, kd_mx.kdmc, ' ' AS dname, ysz_zyhzjyd.jyrq " +
                    "FROM kd_mx INNER JOIN ysz_zyhzjyd ON kd_mx.xh = ysz_zyhzjyd.dykdxh " +
                    "WHERE kd_mx.zyh = '" + registryID + "'";
                SQLSentence2 =
                      "select xh,kdmc,kdys,kdrq,jcjg,jcyx from kd_mx  where kdmc NOT in (SELECT kd_mx.kdmc "+
                    "FROM kd_mx INNER JOIN ysz_zyhzjyd ON kd_mx.xh = ysz_zyhzjyd.dykdxh "+
                    "WHERE kd_mx.zyh = '" + registryID + "') and zyh = '" + registryID + "'";
            }
            else
            {
                SQLSentence = "SELECT kd_mx_mz.xh, kd_mx_mz.kdmc, ' ' AS dname, mz_jyd.jyrq " +
                    "FROM kd_mx_mz INNER JOIN mz_jyd ON kd_mx_mz.xh = mz_jyd.dykdxh " +
                    "WHERE kd_mx_mz.mzh = '" + registryID + "'";
                   
            }
            #endregion

            xmldoc = new XmlDocument();
            XmlNode testsAndExams = xmldoc.CreateElement(EmrConstant.ElementNames.TestsAndExams);
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            DataSet dataset2 = ExeuSentence(SQLSentence2, hisDBType);
            if (dataset == null) return testsAndExams;
          
            foreach(DataRow row in dataset.Tables[0].Rows)
            {
                #region Create attributes for the top level 
                XmlElement form = xmldoc.CreateElement(EmrConstant.ElementNames.Form);
                form.SetAttribute(EmrConstant.AttributeNames.Num, row.ItemArray[0].ToString());
                form.SetAttribute(EmrConstant.AttributeNames.Type,EmrConstant.StringGeneral.Test);
                form.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                form.SetAttribute(EmrConstant.AttributeNames.Doctor, row.ItemArray[2].ToString());
                form.SetAttribute(EmrConstant.AttributeNames.Date, row.ItemArray[3].ToString().Split(' ')[0]);
                #endregion

                string SQLSentence1 =
                    "SELECT mz_jyxmdy.jyxmmc, zy_jyd_mx_sh.jg_sz, zy_jyd_mx_sh.jg_ms,zy_jyd_mx_sh.shjg,zy_jyd_mx_sh.jgzt,zy_jyd_mx_sh.dw " +
                    "FROM ysz_zyhzjyd, zy_jyd_mx_sh, mz_jyxmdy " +
                    "WHERE ysz_zyhzjyd.jysqxh = zy_jyd_mx_sh.jysqxh " +
                    "AND zy_jyd_mx_sh.jyxmbm = mz_jyxmdy.jyxmbm " +
                    "AND ysz_zyhzjyd.dykdxh='" + row.ItemArray[0].ToString() + "'";


                //        select zy_jyd_mx_sh.jyxmbm, mz_jyxmdy.jyxmmc, zy_jyd_mx_sh.jg_sz, zy_jyd_mx_sh.jg_ms,  zy_jyd_mx_sh.shjg
                //from zy_jyd_mx_sh, mz_jyxmdy
                //where zy_jyd_mx_sh.jyxmbm = mz_jyxmdy.jyxmbm
                //and zy_jyd_mx_sh.jysqxh = :jysqxh;

                //模式1：
                //jg_sz数值
                //jg_ms单位
                //模式2：
                //shjg 数值单位

                //:jysqxh 申请单序号
                DataSet dataset1 = ExeuSentence(SQLSentence1, hisDBType);
                foreach (DataRow row1 in dataset1.Tables[0].Rows)
                {
                    XmlElement item = xmldoc.CreateElement(EmrConstant.ElementNames.Item);
                    //item.SetAttribute(EmrConstant.AttributeNames.Type, EmrConstant.StringGeneral.None);
                    item.SetAttribute(EmrConstant.AttributeNames.Name, row1.ItemArray[0].ToString());
                    item.SetAttribute(EmrConstant.AttributeNames.Value, row1.ItemArray[1].ToString());
                    item.SetAttribute(EmrConstant.AttributeNames.Unit, row1.ItemArray[2].ToString());
                    if (row1.ItemArray[5].ToString().Contains("^"))
                    item.SetAttribute(EmrConstant.AttributeNames.ValueUnit, row1.ItemArray[3].ToString() +"*"+ row1.ItemArray[5].ToString());
                    else
                    item.SetAttribute(EmrConstant.AttributeNames.ValueUnit, row1.ItemArray[3].ToString() + " " + row1.ItemArray[5].ToString());
                    item.SetAttribute(EmrConstant.AttributeNames.Result, row1.ItemArray[4].ToString());
                    form.AppendChild(item);
                }

               
                testsAndExams.AppendChild(form);
            }
            foreach (DataRow row in dataset2.Tables[0].Rows)
            {
                #region Create attributes for the top level
                XmlElement form2 = xmldoc.CreateElement(EmrConstant.ElementNames.Form);
                form2.SetAttribute(EmrConstant.AttributeNames.Num, row.ItemArray[0].ToString());
                form2.SetAttribute(EmrConstant.AttributeNames.Type, EmrConstant.StringGeneral.Exam);
                form2.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                form2.SetAttribute(EmrConstant.AttributeNames.Doctor, row.ItemArray[2].ToString());
                form2.SetAttribute(EmrConstant.AttributeNames.Date, row.ItemArray[3].ToString().Split(' ')[0]);
                #endregion


                    XmlElement item2 = xmldoc.CreateElement(EmrConstant.ElementNames.Item);
                    //item.SetAttribute(EmrConstant.AttributeNames.Type, EmrConstant.StringGeneral.None);
                    item2.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[1].ToString());
                    item2.SetAttribute(EmrConstant.AttributeNames.Result,row.ItemArray[1].ToString()+"： 检查结果："+ row.ItemArray[4].ToString()+",检查印象： " + row.ItemArray[5].ToString());
                    form2.AppendChild(item2);
                
                testsAndExams.AppendChild(form2);
            }
            return testsAndExams;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    /* ---------------------------------------------------------------------------
     * 返回 XmlNode 指定患者的医嘱清单:
     *   <DoctorOrders>
     *     <LongOrder Num="1" Type="drug" Doctor="刘莹" Date="2007-12-10">
     *       <Item Name="双环醇片（百赛诺）" Quantity="25.00" Unit="mg" HowUse="口服    "
     *             HowOften="TID" StopDate="2007-12-6" HowLong="" />
     *       <Item Name="单硝酸异山梨酯片(欣康)" Quantity="20.00" Unit="mg" HowUse="口服    "
     *             HowOften="BID" StopDate="2007-12-6" HowLong="" />
     *       <Item Name="马来酸氯苯那敏（扑尔敏）" Quantity="4.00" Unit="mg" HowUse="口服    "
     *             HowOften="TID" StopDate="2007-12-6" HowLong="" />
     *     </LongOrder>
     *     <LongOrder Num="2" Type="drug" Doctor="刘莹" Date="2007-12-10">
     *       <Item Name="利肺片" Quantity=".25" Unit="g" HowUse="口服    " 
     *             HowOften="TID" StopDate="2007-12-6" HowLong="" />
     *     </LongOrder>
     *     <TempOrder Num="3" Type="drug" Doctor="刘莹" Date="2007-3-25">
     *       <Item Name="双环醇片（百赛诺）" Quantity="25.00" Unit="mg" HowUse="口服    "
     *             HowOften="ST" StopDate="" HowLong="1" />
     *       <Item Name="单硝酸异山梨酯片(欣康)" Quantity="20.00" Unit="mg" HowUse="口服    "
     *             HowOften="ST" StopDate="" HowLong="1" />
     *       <Item Name="硫酸阿米卡星(1)" Quantity=".20" Unit="g" HowUse="静脉点滴"
     *             HowOften="ST" StopDate="" HowLong="1" />
     *     </TempOrder>
     *     <TempOrder Num="4" Type="drug" Doctor="刘莹" Date="2007-3-26">
     *       <Item Name="利肺片" Quantity=".25" Unit="g" HowUse="口服    " 
     *             HowOften="ST" StopDate="" HowLong="1" />
     *       <Item Name="皮肤康洗液" Quantity="50.00" Unit="ml" HowUse="外用" 
     *             HowOften="ST" StopDate="" HowLong="1" />
     *     </TempOrder>
     *   </DoctorOrders>
     * 参数: 
     *      string registryID 患者住院登记号或门诊登记号
     *      bool inStyle      = true 住院患者；= false 门诊患者
     * 说明:
     *      元素 LongOrder 表示长期医嘱，属性(允许空值)：
     *          Num    处方编号
     *          Type   处方类型,取值 ="drug"药品 或 ="treat"治疗
     *          Doctor 处方医师姓名
     *          Date   处方执行开始日期
     *      元素 TempOrder 表示临时医嘱，属性与 LongOrder 相同
     *      元素 Item 表示处方项目(细节)，属性：
     *          Name       药品名称或治疗名称
     *          Quantity   药品用量或治疗次数
     *          Unit       单位
     *          HowUse     药品服法，允许空值
     *          HowOften   用药或治疗频次
     *          StopDate   停止日期，允许空值
     *          HowLong    已经执行天数，允许空值
     * 出错返回：
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="GetDoctorOrders" />
     *  </errors>
     * ----------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns DoctorOrders for patient in bed. ", EnableSession = false)]
    public XmlNode GetDoctorOrders(string registryID, bool inStyle)
    {
        #region Make select
        //////string SQLSentence = "SELECT DISTINCT 'LongOrder' AS Title, a.cfh, b.ysm " +
        //////    "FROM tcqyz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
        //////    "WHERE a.zyh='" + registryID + "' " +
        //////    "UNION ALL SELECT DISTINCT 'TempOrder' as Title, a.cfh, b.ysm " +
        //////    "FROM tlsyzz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
        //////    "WHERE a.zyh='" + registryID + "' ORDER BY cfh";
        //////string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
        //////    "FROM tcqyz a INNER JOIN yp_zy b ON a.bm=b.bm " +
        //////    "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
        //////    "AND a.dyyzbs='1'";
        //////string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
        //////    "FROM tlsyzz a INNER JOIN yp_zy b on a.bm=b.bm " +
        //////    "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
        //////    "AND a.dyyzbs='1'";
        //////string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
        //////    "FROM tcqyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //////    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //////    "AND a.dyyzbs='1'";
        //////string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
        //////    "FROM tlsyzz a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //////    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //////    "AND a.dyyzbs='1'";

        string SQLSentence = "SELECT DISTINCT 'LongOrder' AS Title, a.cfh, b.ysm " +
            "FROM tcqyz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
            "WHERE a.zyh='" + registryID + "' " +
            "UNION ALL SELECT DISTINCT 'TempOrder' as Title, a.cfh, b.ysm " +
            "FROM tlsyzz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
            "WHERE a.zyh='" + registryID + "' ORDER BY cfh";
        string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.jsrq  as tyrq, a.ts, a.ksrq " +
            "FROM tcqyz a INNER JOIN yp_zy b ON a.bm=b.bm " +
            "WHERE a.dyyzbs='1' ";
        string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.ysqrsj as tyrq, a.ts, a.rq " +
            "FROM tlsyzz a INNER JOIN yp_zy b on a.bm=b.bm " +
            "WHERE a.dyyzbs='1' ";
        string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.jsrq as tyrq, a.ts, a.ksrq " +
            "FROM tcqyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
            "WHERE a.dyyzbs='1' ";
        string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.ysqrsj as tyrq, a.ts, a.rq " +
            "FROM tlsyzz a INNER JOIN tsfxm b ON a.bm=b.bm " +
            "WHERE a.dyyzbs='1' ";

        string tempDrug1 = null;
        string tempTreat1 = null;
        string longDrug1 = null;
        string longTreat1 = null;
        #endregion
        try
        {
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlNode doctorOrders = xmldoc.CreateElement(EmrConstant.ElementNames.DoctorOrders);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                if (row.ItemArray[1].ToString() == "") continue;
                if (row.ItemArray[1].ToString().Length == 0) continue;
                string title = row.ItemArray[0].ToString();
                string num = row.ItemArray[1].ToString();
                string doctor = row.ItemArray[2].ToString();
 
                switch (title)
                {
                    case "LongOrder":
                        #region Long order
                        if (row.ItemArray[1].ToString() == "")
                        {
                            longDrug1 = longDrug + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                            longTreat1 = longTreat + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                        }
                        else
                        {
                            longDrug1 = longDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                            longTreat1 = longTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                        }
                        DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
                        if (dsLongDrug.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(AttributeNames.Num, num);
                            order.SetAttribute(AttributeNames.Type, StringGeneral.Drug);
                            order.SetAttribute(AttributeNames.Doctor, doctor);
                            order.SetAttribute(AttributeNames.Date, "x");
                            order.SetAttribute(AttributeNames.Style, title);
                            OneSelect(order, dsLongDrug);
                            doctorOrders.AppendChild(order);
                        }
                        DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
                        if (dsLongTreat.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsLongTreat);
                            doctorOrders.AppendChild(order);
                        }
                        #endregion
                        break;
                    case "TempOrder":
                        #region Temporary order
                        if (row.ItemArray[1].ToString() == "")
                        {
                            tempDrug1 = tempDrug + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                            tempTreat1 = tempTreat + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                        }
                        else
                        {
                            tempDrug1 = tempDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                            tempTreat1 = tempTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                        }
                        DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);                   
                        if (dsTempDrug.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Drug);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsTempDrug);
                            doctorOrders.AppendChild(order);
                        }
                        DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
                        if (dsTempTreat.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsTempTreat);
                            doctorOrders.AppendChild(order);
                        }
                        #endregion
                        break;
                }
            }
            return doctorOrders;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    private void OneSelect(XmlNode order, DataSet result)
    {
        foreach (DataRow row in result.Tables[0].Rows)
        {
            if (row.ItemArray[0].ToString().Length == 0) continue;
            XmlElement item = order.OwnerDocument.CreateElement(EmrConstant.ElementNames.Item);
            item.SetAttribute(EmrConstant.AttributeNames.Name, row.ItemArray[0].ToString());
            string sQuantity = row.ItemArray[1].ToString();
            double dbQuantity = 0;
            if (sQuantity.Length > 0) dbQuantity = Convert.ToDouble(sQuantity);
            item.SetAttribute(AttributeNames.Quantity, dbQuantity.ToString("#.00"));
            item.SetAttribute(AttributeNames.Unit, row.ItemArray[2].ToString());
            item.SetAttribute(AttributeNames.HowUse, row.ItemArray[3].ToString());
            item.SetAttribute(AttributeNames.HowOften, row.ItemArray[4].ToString());
            item.SetAttribute(AttributeNames.StopDate, row.ItemArray[5].ToString());
            item.SetAttribute(AttributeNames.HowLong, row.ItemArray[6].ToString());
            item.SetAttribute(AttributeNames.Start, row.ItemArray[7].ToString());
            order.AppendChild(item);
        }
    }


    [WebMethod(Description = "Returns DoctorOrders for patient in ded. ", EnableSession = false)]
    public XmlNode GetDoctorOrdersEx(string registryID, bool inStyle)
    {
        #region Make select
        string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ksrq, a.ysbm " +
            "FROM tcqyz a INNER JOIN yp_zy b ON a.bm=b.bm " +
            "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
            "AND a.dyyzbs='1'";
        string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
            "FROM tlsyzz a INNER JOIN yp_zy b on a.bm=b.bm " +
            "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
            "AND a.dyyzbs='1'";
        string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.tyrq, a.ksrq, a.ysbm " +
            "FROM tcqyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
            "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
            "AND a.dyyzbs='1'";
        string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
            "FROM tlsyzz a INNER JOIN tsfxm b ON a.bm=b.bm " +
            "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
            "AND a.dyyzbs='1'";
        string tempDrug1 = null;
        string tempTreat1 = null;
        string longDrug1 = null;
        string longTreat1 = null;
        #endregion
        try
        {
            
            xmldoc = new XmlDocument();
            XmlNode doctorOrders = xmldoc.CreateElement(ElementNames.DoctorOrders);

            #region Long order
            longDrug1 = longDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            longTreat1 = longTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
            if (dsLongDrug.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.LongDrug);
                OneSelectEx(order, dsLongDrug);
                doctorOrders.AppendChild(order);
            }
            DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
            if (dsLongTreat.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.LongTreat);
                OneSelectEx(order, dsLongTreat);
                doctorOrders.AppendChild(order);
            }
            #endregion

            #region Temporary order
            tempDrug1 = tempDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh"; 
            tempTreat1 = tempTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh"; 
            DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);
            if (dsTempDrug.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                 order.SetAttribute(AttributeNames.Type, StringGeneral.TempDrug);
                 OneSelectEx(order, dsTempDrug);
                doctorOrders.AppendChild(order);
            }
            DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
            if (dsTempTreat.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.TempTreat);
                OneSelectEx(order, dsTempTreat);
                doctorOrders.AppendChild(order);
            }
            #endregion

            return doctorOrders;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    private void OneSelectEx(XmlNode order, DataSet result)
    {
        foreach (DataRow row in result.Tables[0].Rows)
        {
            if (row.ItemArray[0].ToString().Trim() == string.Empty) continue;
            XmlElement item = order.OwnerDocument.CreateElement(ElementNames.Item);
            item.SetAttribute(AttributeNames.Name, row.ItemArray[0].ToString());
            string sQuantity = row.ItemArray[1].ToString();
            double dbQuantity = 0;
            if (sQuantity.Length > 0) dbQuantity = Convert.ToDouble(sQuantity);
            //item.SetAttribute(AttributeNames.Quantity, dbQuantity.ToString("#.00"));
            item.SetAttribute(AttributeNames.Quantity, dbQuantity.ToString());
            item.SetAttribute(AttributeNames.Unit, row.ItemArray[2].ToString());
            item.SetAttribute(AttributeNames.HowUse, row.ItemArray[3].ToString());
            item.SetAttribute(AttributeNames.HowOften, row.ItemArray[4].ToString());
            item.SetAttribute(AttributeNames.StopDate, row.ItemArray[5].ToString());
            item.SetAttribute(AttributeNames.Start, row.ItemArray[6].ToString());
            item.SetAttribute(AttributeNames.DoctorID, row.ItemArray[7].ToString());
            order.AppendChild(item);
        }
    }

    //[WebMethod(Description = "Returns DoctorOrders for discharged patient. ", EnableSession = false)]
    //public XmlNode GetDoctorOrdersDischargedEx(string registryID)
    //{
    //    #region Make select
    //    string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ksrq, a.ysbm " +
    //        "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm " +
    //        "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
    //        "AND a.dyyzbs='1'";
    //    string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
    //        "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm " +
    //        "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
    //        "AND a.dyyzbs='1'";
    //    string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.tyrq, a.ksrq, a.ysbm " +
    //        "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
    //        "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
    //        "AND a.dyyzbs='1'";
    //    string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
    //        "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
    //        "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
    //        "AND a.dyyzbs='1'";
    //    string tempDrug1 = null;
    //    string tempTreat1 = null;
    //    string longDrug1 = null;
    //    string longTreat1 = null;
    //    #endregion
    //    try
    //    {

    //        xmldoc = new XmlDocument();
    //        XmlNode doctorOrders = xmldoc.CreateElement(ElementNames.DoctorOrders);

    //        #region Long order
    //        longDrug1 = longDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
    //        longTreat1 = longTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
    //        DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
    //        if (dsLongDrug.Tables[0].Rows.Count > 0)
    //        {
    //            XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
    //            order.SetAttribute(AttributeNames.Type, StringGeneral.LongDrug);
    //            OneSelectEx(order, dsLongDrug);
    //            doctorOrders.AppendChild(order);
    //        }
    //        DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
    //        if (dsLongTreat.Tables[0].Rows.Count > 0)
    //        {
    //            XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
    //            order.SetAttribute(AttributeNames.Type, StringGeneral.LongTreat);
    //            OneSelectEx(order, dsLongTreat);
    //            doctorOrders.AppendChild(order);
    //        }
    //        #endregion

    //        #region Temporary order
    //        tempDrug1 = tempDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
    //        tempTreat1 = tempTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
    //        DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);
    //        if (dsTempDrug.Tables[0].Rows.Count > 0)
    //        {
    //            XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
    //            order.SetAttribute(AttributeNames.Type, StringGeneral.TempDrug);
    //            OneSelectEx(order, dsTempDrug);
    //            doctorOrders.AppendChild(order);
    //        }
    //        DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
    //        if (dsTempTreat.Tables[0].Rows.Count > 0)
    //        {
    //            XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
    //            order.SetAttribute(AttributeNames.Type, StringGeneral.TempTreat);
    //            OneSelectEx(order, dsTempTreat);
    //            doctorOrders.AppendChild(order);
    //        }
    //        #endregion

    //        return doctorOrders;
    //    }
    //    catch (Exception ex)
    //    {
    //        return DataError(ex);
    //    }
    //}
    [WebMethod(Description = "Returns DoctorOrders for discharged patient. ", EnableSession = false)]
    public XmlNode GetDoctorOrdersDischargedEx(string registryID)//刘伟修改药品编码限制
    {
        #region Make select
        //string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ksrq, a.ysbm " +
        //    "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm " +
        //    "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
        //    "AND a.dyyzbs='1'";
        string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ksrq, a.ysbm " +
    "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm WHERE a.dyyzbs='1'";

        //string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
        //    "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm " +
        //    "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
        //    "AND a.dyyzbs='1'";
        string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
    "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm WHERE a.dyyzbs='1'";
        //string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.tyrq, a.ksrq, a.ysbm " +
        //    "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //    "AND a.dyyzbs='1'";
        string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, a.tyrq, a.ksrq, a.ysbm " +
    "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        //string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
        //    "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //    "AND a.dyyzbs='1'";
        string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.rq, a.ysbm " +
    "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        string tempDrug1 = null;
        string tempTreat1 = null;
        string longDrug1 = null;
        string longTreat1 = null;
        #endregion
        try
        {

            xmldoc = new XmlDocument();
            XmlNode doctorOrders = xmldoc.CreateElement(ElementNames.DoctorOrders);

            #region Long order
            longDrug1 = longDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            longTreat1 = longTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
            if (dsLongDrug.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.LongDrug);
                OneSelectEx(order, dsLongDrug);
                doctorOrders.AppendChild(order);
            }
            DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
            if (dsLongTreat.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.LongTreat);
                OneSelectEx(order, dsLongTreat);
                doctorOrders.AppendChild(order);
            }
            #endregion

            #region Temporary order
            tempDrug1 = tempDrug + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            tempTreat1 = tempTreat + " AND a.zyh='" + registryID + "' ORDER BY a.cfh";
            DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);
            if (dsTempDrug.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.TempDrug);
                OneSelectEx(order, dsTempDrug);
                doctorOrders.AppendChild(order);
            }
            DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
            if (dsTempTreat.Tables[0].Rows.Count > 0)
            {
                XmlElement order = xmldoc.CreateElement(ElementNames.OrderNumber);
                order.SetAttribute(AttributeNames.Type, StringGeneral.TempTreat);
                OneSelectEx(order, dsTempTreat);
                doctorOrders.AppendChild(order);
            }
            #endregion

            return doctorOrders;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    /* ---------------------------------------------------------------------------------------------
     * 返回 XmlNode 指定出院患者的医嘱清单:
     * -------------------------------------------------------------------------------------------- */
    //[WebMethod(Description = "Returns DoctorOrders for discharged patient", EnableSession = false)]
    //public XmlNode GetDoctorOrdersDischarged(string registryID)
    //{
    //    #region Make select
    //    string SQLSentence = "SELECT DISTINCT 'LongOrder' AS Title, a.cfh, b.ysm " +
    //        "FROM tcq a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
    //        "WHERE a.dyyzbs='1' and a.zyh='" + registryID + "' " +
    //        "UNION ALL SELECT DISTINCT 'TempOrder' as Title, a.cfh, b.ysm " +
    //        "FROM tlsyz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
    //        "WHERE a.dyyzbs='1' and a.zyh='" + registryID + "' ORDER BY cfh";
    //    string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
    //        "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm " +
    //        "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
    //        "AND a.dyyzbs='1'";
    //    string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
    //        "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm " +
    //        "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
    //        "AND a.dyyzbs='1'";
    //    string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
    //        "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
    //        "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
    //        "AND a.dyyzbs='1'";
    //    string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
    //        "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
    //        "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
    //        "AND a.dyyzbs='1'";
    //    string longDrug1 = null;
    //    string tempDrug1 = null;
    //    string longTreat1 = null;
    //    string tempTreat1 = null;
    //    #endregion
    //    try
    //    {
    //        DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
    //        xmldoc = new XmlDocument();
    //        XmlNode doctorOrders = xmldoc.CreateElement(EmrConstant.ElementNames.DoctorOrders);
    //        foreach (DataRow row in dataset.Tables[0].Rows)
    //        {
    //            if (row.ItemArray[1].ToString() == "") continue;
    //            string title = row.ItemArray[0].ToString();
    //            string num = row.ItemArray[1].ToString();
    //            string doctor = row.ItemArray[2].ToString();
    //            switch (title)
    //            {
    //                case "LongOrder":
    //                    #region Long order
    //                    if (row.ItemArray[1].ToString() == "")
    //                    {
    //                        longDrug1 = longDrug + " AND a.cfh IS null AND a.zyh='" + registryID + "'";
    //                        longTreat1 = longTreat + " AND a.cfh IS null AND a.zyh='" + registryID + "'";
    //                    }
    //                    else
    //                    {
    //                        longDrug1 = longDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
    //                        longTreat1 = longTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
    //                    }
    //                    DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
    //                    if (dsLongDrug.Tables[0].Rows.Count > 0)
    //                    {
    //                        XmlElement order = xmldoc.CreateElement(title);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Num, num);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Drug);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
    //                        order.SetAttribute(EmrConstant.AttributeNames.Style, title);
    //                        OneSelect(order, dsLongDrug);
    //                        doctorOrders.AppendChild(order);
    //                    }
    //                    DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
    //                    if (dsLongTreat.Tables[0].Rows.Count > 0)
    //                    {
    //                        XmlElement order = xmldoc.CreateElement(title);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Num, num);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
    //                        order.SetAttribute(EmrConstant.AttributeNames.Style, title);
    //                        OneSelect(order, dsLongTreat);
    //                        doctorOrders.AppendChild(order);
    //                    }
    //                    #endregion
    //                    break;
    //                case "TempOrder":
    //                    #region Temporary order
    //                    if (row.ItemArray[1].ToString() == "")
    //                    {
    //                        tempDrug1 = tempDrug + " AND a.cfh is null AND a.zyh='" + registryID + "'";
    //                        tempTreat1 = tempTreat + " AND a.cfh is null AND a.zyh='" + registryID + "'";
    //                    }
    //                    else
    //                    {
    //                        tempDrug1 = tempDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
    //                        tempTreat1 = tempTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
    //                    }
    //                    DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);
    //                    if (dsTempDrug.Tables[0].Rows.Count > 0)
    //                    {
    //                        XmlElement order = xmldoc.CreateElement(title);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Num, num);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Drug);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
    //                        order.SetAttribute(EmrConstant.AttributeNames.Style, title);
    //                        OneSelect(order, dsTempDrug);
    //                        doctorOrders.AppendChild(order);
    //                    }
    //                    DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
    //                    if (dsTempTreat.Tables[0].Rows.Count > 0)
    //                    {
    //                        XmlElement order = xmldoc.CreateElement(title);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Num, num);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
    //                        order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
    //                        order.SetAttribute(EmrConstant.AttributeNames.Style, title);
    //                        OneSelect(order, dsTempTreat);
    //                        doctorOrders.AppendChild(order);
    //                    }
    //                    #endregion
    //                    break;
    //            }
    //        }
    //        return doctorOrders;
    //    }
    //    catch (Exception ex)
    //    {
    //        return DataError(ex);
    //    }
    //}

    /* ---------------------------------------------------------------------------------------------
    * 返回 XmlNode 指定出院患者的医嘱清单:
    * -------------------------------------------------------------------------------------------- */
    [WebMethod(Description = "Returns DoctorOrders for discharged patient", EnableSession = false)]
    public XmlNode GetDoctorOrdersDischarged(string registryID)//刘伟修改药品名称限制
    {
        #region Make select
        string SQLSentence = "SELECT DISTINCT 'LongOrder' AS Title, a.cfh, b.ysm " +
            "FROM tcq a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
            "WHERE a.dyyzbs='1' and a.zyh='" + registryID + "' " +
            "UNION ALL SELECT DISTINCT 'TempOrder' as Title, a.cfh, b.ysm " +
            "FROM tlsyz a INNER JOIN tysm b ON a.ysbm=b.ysbm " +
            "WHERE a.dyyzbs='1' and a.zyh='" + registryID + "' ORDER BY cfh";
        //string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
        //    "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm " +
        //    "WHERE (left(a.bm,1)='1' OR left(a.bm,1)='2' OR left(a.bm,1)='3') " +
        //    "AND a.dyyzbs='1'";
        string longDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
    "FROM tcq a INNER JOIN yp_zy b ON a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        //string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
        //    "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm " +
        //    "WHERE (left(a.bm,1)='1' or left(a.bm,1)='2' or left(a.bm,1)='3') " +
        //    "AND a.dyyzbs='1'";
        string tempDrug = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
    "FROM tlsyz a INNER JOIN yp_zy b on a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        //string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
        //    "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //    "AND a.dyyzbs='1'";
        string longTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl,a.tyrq, a.ts, a.ksrq " +
    "FROM tcq a INNER JOIN tsfxm b ON a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        //string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
        //    "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
        //    "WHERE left(a.bm,1)<>'1' AND left(a.bm,1)<>'2' AND left(a.bm,1)<>'3' " +
        //    "AND a.dyyzbs='1'";
        string tempTreat = "SELECT b.pm, a.ypyl, a.hldw, a.zxfs, a.yfyl, '' as tyrq, a.ts, a.rq " +
    "FROM tlsyz a INNER JOIN tsfxm b ON a.bm=b.bm " +
    "WHERE a.dyyzbs='1'";
        string longDrug1 = null;
        string tempDrug1 = null;
        string longTreat1 = null;
        string tempTreat1 = null;
        #endregion
        try
        {
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            xmldoc = new XmlDocument();
            XmlNode doctorOrders = xmldoc.CreateElement(EmrConstant.ElementNames.DoctorOrders);
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                if (row.ItemArray[1].ToString() == "") continue;
                string title = row.ItemArray[0].ToString();
                string num = row.ItemArray[1].ToString();
                string doctor = row.ItemArray[2].ToString();
                switch (title)
                {
                    case "LongOrder":
                        #region Long order
                        if (row.ItemArray[1].ToString() == "")
                        {
                            longDrug1 = longDrug + " AND a.cfh IS null AND a.zyh='" + registryID + "'";
                            longTreat1 = longTreat + " AND a.cfh IS null AND a.zyh='" + registryID + "'";
                        }
                        else
                        {
                            longDrug1 = longDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                            longTreat1 = longTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                        }
                        DataSet dsLongDrug = ExeuSentence(longDrug1, hisDBType);
                        if (dsLongDrug.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Drug);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsLongDrug);
                            doctorOrders.AppendChild(order);
                        }
                        DataSet dsLongTreat = ExeuSentence(longTreat1, hisDBType);
                        if (dsLongTreat.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsLongTreat);
                            doctorOrders.AppendChild(order);
                        }
                        #endregion
                        break;
                    case "TempOrder":
                        #region Temporary order
                        if (row.ItemArray[1].ToString() == "")
                        {
                            tempDrug1 = tempDrug + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                            tempTreat1 = tempTreat + " AND a.cfh is null AND a.zyh='" + registryID + "'";
                        }
                        else
                        {
                            tempDrug1 = tempDrug + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                            tempTreat1 = tempTreat + " AND a.cfh=" + num + " AND a.zyh='" + registryID + "'";
                        }
                        DataSet dsTempDrug = ExeuSentence(tempDrug1, hisDBType);
                        if (dsTempDrug.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Drug);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsTempDrug);
                            doctorOrders.AppendChild(order);
                        }
                        DataSet dsTempTreat = ExeuSentence(tempTreat1, hisDBType);
                        if (dsTempTreat.Tables[0].Rows.Count > 0)
                        {
                            XmlElement order = xmldoc.CreateElement(title);
                            order.SetAttribute(EmrConstant.AttributeNames.Num, num);
                            order.SetAttribute(EmrConstant.AttributeNames.Type, StringGeneral.Treat);
                            order.SetAttribute(EmrConstant.AttributeNames.Doctor, doctor);
                            order.SetAttribute(EmrConstant.AttributeNames.Date, "x");
                            order.SetAttribute(EmrConstant.AttributeNames.Style, title);
                            OneSelect(order, dsTempTreat);
                            doctorOrders.AppendChild(order);
                        }
                        #endregion
                        break;
                }
            }
            return doctorOrders;
        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }

    /*----------------------------------------------------------------------------------------------
     * 测试 HIS 数据库的连接
     * 返回 true 连接成功
     * 返回 false 连接失败
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Test connection to HIS database.", EnableSession = false)]
    public Boolean ConnectHisdb()
    {
        try
        {
            switch(hisDBType)
            {
                case "ORACLE":
                    OracleConnection oconn = new OracleConnection(connnectString);
                    oconn.Open();
                    oconn.Close();
                    return EmrConstant.Return.Successful;
                case "MSSQL":
                    SqlConnection sconn = new SqlConnection(connnectString);
                    sconn.Open();
                    sconn.Close();
                    return EmrConstant.Return.Successful;
                default:
                    return EmrConstant.Return.Failed;
            }
        }
        catch (Exception ex)
        {
            DataError(ex);
            return EmrConstant.Return.Failed;
        }
    }

    /* -----------------------------------------------------------------------------
     * 判断一组住院登记号是否已经在指定日期之前出院，将出院的返回。
     * 参数：
     *     XmlNode inRegistryIDs 一组住院登记号
     *     <RegistryIDs>
     *          <RegistryID>00001245</RegistryID>
     *          <RegistryID>00011245</RegistryID>
     *     </RegistryIDs>
     *     DateTime endDate 出院日期截止到endDate
     *     [ref] XmlNode outRegistryIDs 返回一组已经出院的住院登记号，来自 inRegistryIDs
     *     结构与 inRegistryIDs 相同
     * 出错：outRegistryIDs 返回下面
     *  <errors>
     *      <error errordate="2008-01-01" message="error message" method="IsDischarged" />
     *  </errors>
     * ----------------------------------------------------------------------------  */
    [WebMethod(Description = "is discharged", EnableSession = false)]
    public Boolean IsDischarged(XmlNode inRegistryIDs, DateTime endDate, ref XmlNode outRegistryIDs)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
            doc.AppendChild(root);
            foreach (XmlNode inRegistryID in inRegistryIDs)
            {
                string SQLSentence = "SELECT COUNT(*) FROM tdjk WHERE zyh = '" + inRegistryID.InnerText + "'";
                SQLSentence += " AND cyrq <= '" + endDate.ToString() + "'";
                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row.ItemArray[0]) == 1)
                    {
                        XmlNode outRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                        outRegistryID.InnerText = inRegistryID.InnerText;
                        root.AppendChild(outRegistryID);
                    }
                    
                }
            }
            outRegistryIDs = doc.Clone();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            outRegistryIDs = DataError(ex);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "get by zyrq", EnableSession = false)]
    public Boolean Byzyrq(DateTime beginDate, DateTime endDate, ref XmlNode outRegistryIDs)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
            doc.AppendChild(root);

            string SQLSentence = "SELECT zyh FROM tdjk " +
                "WHERE " + " zyrq >= '" + beginDate.ToString() + "'" + " AND zyrq <= '" + endDate.ToString() + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlNode outRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                    outRegistryID.InnerText = row.ItemArray[0].ToString();
                    root.AppendChild(outRegistryID);

                }
            outRegistryIDs = doc.Clone();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            outRegistryIDs = DataError(ex);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "get by cyrq", EnableSession = false)]
    public Boolean Bycyrq(DateTime beginDate, DateTime endDate, ref XmlNode outRegistryIDs)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
            doc.AppendChild(root);

            string SQLSentence = "SELECT zyh FROM tdjk " +
                "WHERE " + " cyrq >= '" + beginDate.ToString() + "'" + " AND cyrq <= '" + endDate.ToString() + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlNode outRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                outRegistryID.InnerText = row.ItemArray[0].ToString();
                root.AppendChild(outRegistryID);

            }
            outRegistryIDs = doc.Clone();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            outRegistryIDs = DataError(ex);
            return EmrConstant.Return.Failed;
        }
    }

    /* -----------------------------------------------------------------------------
     * 获得医院名称
     -------------------------------------------------------------------------------*/
    [WebMethod(Description = "Return hospital name", EnableSession = false)]
    public string GetHospitalName()
    {
        string select = "SELECT yymc FROM yyjbqk";
        DataSet dataset = ExeuSentence(select, hisDBType);
        if (dataset.Tables[0].Rows.Count == 1) return dataset.Tables[0].Rows[0][0].ToString();
        else return null;
    }

    /* -----------------------------------------------------------------------------
     * 获得一住院患者的姓名和病案号
     * 参数:
     *     string registryID -- 患者住院登记号，每住院一次分配一次
     * [ref] string pname -- 返回患者姓名
     * [ref] string archiveNum -- 返回患者病案号 (PID)
     * 成功返回 true
     * 失败放回 false
     -------------------------------------------------------------------------------*/
    [WebMethod(Description = "Get name and archive number of a patient by registry id")]
    public Boolean GetNameAndArchiveNumberForInpatient(string registryID, ref string pname, ref string archiveNum)
    {
        string stayInHospital = "SELECT xm, bah FROM tdjkz  WHERE zyh = '" + registryID + "'";
        string leftFromHospital = "SELECT xm, bah FROM tdjk  WHERE zyh = '" + registryID + "'";

        DataSet dataset = ExeuSentence(stayInHospital, hisDBType);
        if (dataset.Tables.Count == 0) return EmrConstant.Return.Failed;
        if (dataset.Tables[0].Rows.Count == 1)
        {
            pname = dataset.Tables[0].Rows[0].ItemArray[0].ToString();
            archiveNum = dataset.Tables[0].Rows[0].ItemArray[1].ToString();
            return EmrConstant.Return.Successful;
        }
        else
        {
            dataset = ExeuSentence(leftFromHospital, hisDBType);
            if (dataset.Tables.Count == 0) return EmrConstant.Return.Failed;
            if (dataset.Tables[0].Rows.Count == 1)
            {
                pname = dataset.Tables[0].Rows[0].ItemArray[0].ToString();
                archiveNum = dataset.Tables[0].Rows[0].ItemArray[1].ToString();
                return EmrConstant.Return.Successful;
            }
        }
        return EmrConstant.Return.Failed;
    }


    [WebMethod(Description = "Check the operator is a nurse")]
    public bool IsNurse(string opcode)
    {
        try
        {
            string SQLSentence = "SELECT lb FROM tysm WHERE ysbm = '" + opcode + "'";
            DataSet myDataSet = ExeuSentence(SQLSentence, hisDBType);
            if (myDataSet.Tables[0].Rows.Count == 0) return false;
            return myDataSet.Tables[0].Rows[0][0].ToString() == "2";
        } //try end
        catch (Exception)
        {
            return false;
        }
    }


    [WebMethod(Description = "Get start time for emr quality control", EnableSession = false)]
    public string GetStartTime(string noteID, string stime, string registryID, ref DateTime startTime)
    {
        string msg = null;
        switch (stime)
        {
            case StartTime.Registry:
                msg = RegistryTime(registryID, ref startTime);
                break;
            case StartTime.Discharged:
                msg = DischargedTime(registryID, ref startTime);
                break;
            case StartTime.Dead:
                msg = DeadTime(registryID, ref startTime);
                break;
            case StartTime.Monthly:
                msg = RegistryTime(registryID, ref startTime);
                break;
        }
        return msg;
    }
    private string  RegistryTime(string registryID, ref DateTime regtime)
    {
        try
        {
            string SQLSentence = "SELECT zyrq FROM tdjkz WHERE zyh = '" + registryID + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                if (!Convert.IsDBNull(dataset.Tables[0].Rows[0][0]))
                    regtime = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
                return null;
            }
            SQLSentence = "SELECT zyrq FROM tdjk WHERE zyh = '" + registryID + "'";
            dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                if (!Convert.IsDBNull(dataset.Tables[0].Rows[0][0]))
                    regtime = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    private string DischargedTime(string registryID, ref DateTime regtime)
    {
        try
        {
            string SQLSentence = "SELECT cyrq FROM tdjkz WHERE zyh = '" + registryID + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                if (!Convert.IsDBNull(dataset.Tables[0].Rows[0][0]))
                    regtime = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
                return null;
            }
            SQLSentence = "SELECT cyrq FROM tdjk WHERE zyh = '" + registryID + "'";
            dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                if (!Convert.IsDBNull(dataset.Tables[0].Rows[0][0]))
                    regtime = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    private string DeadTime(string registryID, ref DateTime  deadtime)
    {
        try
        {
            string SQLSentence = "SELECT CYRQ FROM TDJK  WHERE zyh = '" +
                registryID + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 1)
            {
                if (!Convert.IsDBNull(dataset.Tables[0].Rows[0][0]))
                    deadtime = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
            }
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get Diagnose of Previous Operation", EnableSession = false)]
    public string GetDiagnosePreOperation(string registryID, ref XmlNode result)
    {
         string SQLSentence = "SELECT ssmc, ssksrq, sqzd FROM ssmxk WHERE zyh = '"
                + registryID + "' AND ssjsrq IS NULL";
         try
         {
             DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
             if (dataset.Tables[0].Rows.Count == 0) return null;
             XmlDocument doc = new XmlDocument();
             XmlElement operation = doc.CreateElement(ElementNames.Operation);
             operation.SetAttribute(AttributeNames.OperationName, dataset.Tables[0].Rows[0][0].ToString());
             operation.SetAttribute(AttributeNames.DateTime, dataset.Tables[0].Rows[0][1].ToString());
             operation.SetAttribute(AttributeNames.PreDiagnose, dataset.Tables[0].Rows[0][2].ToString());
             result = operation.Clone();
             return null;
         }
         catch (Exception ex)
         {
             return ex.Message + "-" + ex.Source;
         }
    }

    [WebMethod(Description = "Get start time and degree order ", EnableSession = false)]
    public string DegreeOrderTime(string registryID, ref XmlNode degree)
    {
        try
        {
            string SQLSentence = "SELECT Start, Degree FROM illDegree WHERE RegistryID = '" +
                registryID + "' ORDER BY Start";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement registryIDs = doc.CreateElement(ElementNames.RegistryIDs);
            foreach(DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement regID = doc.CreateElement(ElementNames.RegistryID);
                regID.SetAttribute(AttributeNames.StartTime, row[0].ToString());
                regID.SetAttribute(AttributeNames.Critical, row[1].ToString());
                registryIDs.AppendChild(regID);
            }
            degree = registryIDs.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the new degree order ", EnableSession = false)]
    public string LastStratTimeForDegreeOrder(string registryID, ref DateTime start)
    {
        try
        {
            string SQLSentence = "SELECT Max(Start) FROM illDegree WHERE RegistryID = '" +
                registryID + "'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            if (Convert.IsDBNull(dataset.Tables[0].Rows[0][0])) return null;
            start = Convert.ToDateTime(dataset.Tables[0].Rows[0][0]);
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the operations ", EnableSession = false)]
    public string DoneTimeForOperations(string registryID, ref XmlNode doneTime)
    {
        try
        {
            string SQLSentence = "SELECT a.ssjsrq, a.ssxh, a.ssmc, a.sqzd, a.shzd, a.ssys1, a.ssys2, " +
                "a.hs1, a.hs2, a.hs3, a.mzys1, a.mzys2, a.ssksrq, b.mc " +
                "FROM ssmxk a LEFT OUTER JOIN mzfsbm b ON a.mzbm=b.bm WHERE a.zyh = '"
                + registryID + "' AND a.ssjsrq IS NOT NULL";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement operations = doc.CreateElement(ElementNames.Operations);
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement operation = doc.CreateElement(ElementNames.Operation);
                operation.SetAttribute(AttributeNames.DateTime, row[0].ToString());    // ssjsrq
                operation.SetAttribute(AttributeNames.Sequence, row[1].ToString());    // ssxh
                operation.SetAttribute(AttributeNames.OperationName, row[2].ToString()); // ssmc
                operation.SetAttribute(AttributeNames.PreDiagnose, row[3].ToString());  // sqzd
                operation.SetAttribute(AttributeNames.ProDiagnose, row[4].ToString()); // shzd
                operation.SetAttribute(AttributeNames.Surgeon, row[5].ToString()); // ssys1
                operation.SetAttribute(AttributeNames.StartTime, row[12].ToString()); // ssksrq

                XmlElement assistance = doc.CreateElement(ElementNames.OperationAssistance);
                assistance.SetAttribute(AttributeNames.Name, row[6].ToString()); //ssys2
                operation.AppendChild(assistance);

                XmlElement nurse = doc.CreateElement(ElementNames.Nurse);
                nurse.SetAttribute(AttributeNames.Name, row[7].ToString()); // hs1
                operation.AppendChild(nurse);
                nurse = doc.CreateElement(ElementNames.Nurse);
                nurse.SetAttribute(AttributeNames.Name, row[8].ToString()); // hs2
                operation.AppendChild(nurse);
                nurse = doc.CreateElement(ElementNames.Nurse);
                nurse.SetAttribute(AttributeNames.Name, row[9].ToString()); // hs3
                operation.AppendChild(nurse);

                XmlElement anaesthesia = doc.CreateElement(ElementNames.Anaesthesia);
                anaesthesia.SetAttribute(AttributeNames.AnaesthesiaWay, row[13].ToString());  // mc
                operation.AppendChild(anaesthesia);

                XmlElement anaesthetist = doc.CreateElement(ElementNames.Anaesthetist);
                anaesthetist.SetAttribute(AttributeNames.Name, row[10].ToString());   // mzys1
                operation.AppendChild(anaesthetist);
                anaesthetist = doc.CreateElement(ElementNames.Anaesthetist);
                anaesthetist.SetAttribute(AttributeNames.Name, row[11].ToString());   // mzys2
                operation.AppendChild(anaesthetist);

                operations.AppendChild(operation);
            }
            doneTime = operations.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the rescues ", EnableSession = false)]
    public string DoneTimeForRescues(string registryID, ref XmlNode doneTime)
    {
        try
        {
            string SQLSentence = "SELECT jsrq, xh FROM bl_brqk WHERE zyh = '" +
                registryID + "' AND jsrq IS NOT NULL AND sjlxbm = 'F'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement rescues = doc.CreateElement(ElementNames.Rescues);
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement rescue = doc.CreateElement(ElementNames.Rescue);
                rescue.SetAttribute(AttributeNames.DateTime, row[0].ToString());
                rescue.SetAttribute(AttributeNames.RescueSequence, row[1].ToString());
                rescues.AppendChild(rescue);
            }
            doneTime = rescues.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the transfer in ", EnableSession = false)]
    public string DoneTimeForTransferIn(string registryID, ref XmlNode doneTime)
    {
        try
        {
            string SQLSentence = "SELECT jsrq, xh FROM bl_brqk WHERE zyh = '" +
                registryID + "' AND jsrq IS NOT NULL AND sjlxbm = 'C2'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement transferIns = doc.CreateElement(ElementNames.TrasferIn + "s");
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement transferIn = doc.CreateElement(ElementNames.TrasferIn);
                transferIn.SetAttribute(AttributeNames.DateTime, row[0].ToString());
                transferIn.SetAttribute(AttributeNames.TransferInSequence, row[1].ToString());
                transferIns.AppendChild(transferIn);
            }
            doneTime = transferIns.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the transfer out ", EnableSession = false)]
    public string DoneTimeForTransferOut(string registryID, ref XmlNode doneTime)
    {
        try
        {
            string SQLSentence = "SELECT zcrq, xh FROM tzkku WHERE zyh = '" +
               registryID + "' AND zkbs = '1'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement transferOuts = doc.CreateElement(ElementNames.TransferOut + "s");
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement transferOut = doc.CreateElement(ElementNames.TransferOut);
                transferOut.SetAttribute(AttributeNames.DateTime, row[0].ToString());
                transferOut.SetAttribute(AttributeNames.TransferOutSequence, row[1].ToString());
                transferOuts.AppendChild(transferOut);
            }
            doneTime = transferOuts.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get start time for the take over ", EnableSession = false)]
    public string DoneTimeForTakeOver(string registryID, ref XmlNode doneTime)
    {
        try
        {
            string SQLSentence = "SELECT jsrq, xh FROM bl_brqk WHERE zyh = '" +
                registryID + "' AND jsrq IS NOT NULL AND sjlxbm = 'D2'";
            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement takeovers = doc.CreateElement(ElementNames.TakeOver + "s");
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement takeover = doc.CreateElement(ElementNames.TakeOver);
                takeover.SetAttribute(AttributeNames.DateTime, row[0].ToString());
                takeover.SetAttribute(AttributeNames.TakeOverSequence, row[1].ToString());
                takeovers.AppendChild(takeover);
            }
            doneTime = takeovers.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get filed time for the consult ", EnableSession = false)]
    public string DoneTimeForConsultQT(string registryID, string Sequence, ref XmlNode doneTime)
    {
        try
        {
            if (Sequence != null && Sequence != "")
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no,reason,jybs,tz,xgjczl,nzjb,hzlb =case when hzlb = 1 then '普通' when hzlb= '2' then '急' end " +
                    "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign <> '2 ' and serial_no = '" + Sequence + "'";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consult.SetAttribute("jybs", row[6].ToString());
                    consult.SetAttribute("tz", row[7].ToString());
                    consult.SetAttribute("xgjczl", row[8].ToString());
                    consult.SetAttribute("nzjb", row[9].ToString());
                       consult.SetAttribute("hzlb", row[10].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
            else
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no,reason,jybs,tz,xgjczl,nzjb,hzlb =case when hzlb = 1 then '普通' when hzlb= '2' then '急' end "  +
                                 "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign <> '2 ' ";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consult.SetAttribute("jybs", row[6].ToString());
                    consult.SetAttribute("tz", row[7].ToString());
                    consult.SetAttribute("xgjczl", row[8].ToString());
                    consult.SetAttribute("nzjb", row[9].ToString());
                       consult.SetAttribute("hzlb", row[10].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
   
    /// <summary>
    /// gjt 2009/5/26
    /// </summary>
    /// <param name="registryID"></param>
    /// <param name="doneTime"></param>
    /// <returns></returns>
    [WebMethod(Description = "Get filed time for the consult ", EnableSession = false)]
    public string DoneTimeForConsult(string registryID,string Sequence, ref XmlNode doneTime)
    {
  
        try
        {
            if (Sequence != null && Sequence != "")
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no, reason " +
                    "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign <> '2 ' and serial_no = '" + Sequence + "'";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
            else
            {
                string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no, reason " +
                                 "FROM mr_consultation WHERE zyh = '" + registryID + "' and con_sign <> '2 ' ";


                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
                if (dataset.Tables[0].Rows.Count == 0) return null;
                XmlDocument doc = new XmlDocument();
                XmlElement consults = doc.CreateElement(ElementNames.Consultations);
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                    consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                    consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                    consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                    consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                    consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                    consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                    consults.AppendChild(consult);
                }
                doneTime = consults.Clone();
                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get filed time for the consult ", EnableSession = false)]
    public string DoneTimeForConsultEx(string opcode,string registryID, ref XmlNode doneTime)
    {
        try
        {
           
            string SQLSentence = "SELECT serial_no, apply_time, apply_doctor_no, dept_no, doctor_no, reason " +
              "FROM mr_consultation WHERE  con_sign <> '2 ' AND zyh = '" + registryID + "' AND doctor_no='" + opcode + "'";

            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            XmlDocument doc = new XmlDocument();
            XmlElement consults = doc.CreateElement(ElementNames.Consultations);
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement consult = doc.CreateElement(ElementNames.Consultation);
                consult.SetAttribute(AttributeNames.Sequence, row[0].ToString());
                consult.SetAttribute(AttributeNames.DateTime, row[1].ToString());
                consult.SetAttribute(AttributeNames.DoctorID, row[2].ToString());
                consult.SetAttribute(AttributeNames.DepartmentCode, row[3].ToString());
                consult.SetAttribute(AttributeNames.FileDoctorID, row[4].ToString());
                consult.SetAttribute(AttributeNames.Reason, row[5].ToString());
                consults.AppendChild(consult);
            }
            doneTime = consults.Clone();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod]
    public int RemindConsultEx(string opcode, string Deptcode)
    {

        SqlHelper Helper = new SqlHelper("HisDB");
        int Sum = 0;
        SqlHelper emrHelper = new SqlHelper("EmrDB");
        try
        {
            DataTable dt = new DataTable();

            //////            string SQLSentence = @"SELECT count(serial_no) 
            //////                 FROM mr_consultation WHERE  con_sign <> '2 ' AND dept_no = '" + strdept_no + "' AND doctor_no='" + opcode + "'";
            //////            DataTable dt = Helper.GetDataTable(SQLSentence);
            //////            if (dt != null && dt.Rows.Count != 0)
            //////            {
            //////                Sum = Sum + Convert.ToInt32(dt.Rows[0][0].ToString());
            //////            }
            //////            SQLSentence = @"SELECT count(serial_no) 
            //////                 FROM mr_consultation WHERE  con_sign <> '2 ' AND dept_no = '" + strdept_no + "' AND doctor_no!='" + opcode + "'";
            //////            dt = Helper.GetDataTable(SQLSentence);
            //////            if (dt != null && dt.Rows.Count != 0)
            //////            {
            //////                Sum = Sum + Convert.ToInt32(dt.Rows[0][0].ToString());
            //////            }
            //////            SQLSentence = @"SELECT count(serial_no) 
            //////                 FROM mr_consultation WHERE  con_sign <> '2 ' AND dept_no != '" + strdept_no + "' AND doctor_no='" + opcode + "'";
            //////            dt = Helper.GetDataTable(SQLSentence);
            //////            if (dt != null && dt.Rows.Count != 0)
            //////            {
            //////                Sum = Sum + Convert.ToInt32(dt.Rows[0][0].ToString());
            //////            }



            string strDep = "select [group]  from TB_group where [group]  like '%" + Deptcode + "%'";





            string SQLSentence = @"SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz,
                        a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw ";



            SQLSentence +=
            @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
            "WHERE ";
            DataTable dt1 = emrHelper.GetDataTable(strDep);
            if (dt1 != null && dt1.Rows.Count != 0)
            {
                string str = dt1.Rows[0]["Group"].ToString();
                string[] strlist = str.Split(',');
                if (strlist.Length != 0)
                {
                    for (int i = 0; i < strlist.Length; i++)
                    {
                        SQLSentence = SQLSentence + " (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "' )" + "OR";

                    }
                    SQLSentence = SQLSentence.Substring(0, SQLSentence.Length - 2);
                }
      
            }
            else
            {
                SQLSentence = SQLSentence + "(e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + Deptcode + "' AND e.con_sign <> '2'  AND e.DOCTOR_NO = '" + opcode + "' )";
            }
         

            dt = Helper.GetDataTable(SQLSentence);
            if (dt != null && dt.Rows.Count != 0)
            {
                Sum = Sum + dt.Rows.Count;
            }

            return Sum;
        }
        catch (Exception ex)
        {
            //return ex.Message;
            return -1;
        }
    }

    //大港会诊 申请者写病历 20140228
    [WebMethod]
    public int RemindConsultExDg(string opcode, string Deptcode)
    {

        SqlHelper Helper = new SqlHelper("HisDB");
        int Sum = 0;
        SqlHelper emrHelper = new SqlHelper("EmrDB");
        try
        {
            DataTable dt = new DataTable(); 
            string strDep = "select [group]  from TB_group where [group]  like '%" + Deptcode + "%'";

            string SQLSentence = @"SELECT a.bah,a.xm,a.xb,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz,
                        a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw ";

            SQLSentence +=
            @"FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm 
                     LEFT JOIN jgbm d ON a.jg=d.jgbm INNER JOIN mr_consultation e ON a.zyh=e.zyh " +
            "WHERE ";
            DataTable dt1 = emrHelper.GetDataTable(strDep);
            if (dt1 != null && dt1.Rows.Count != 0)
            {
                string str = dt1.Rows[0]["Group"].ToString();
                string[] strlist = str.Split(',');
                if (strlist.Length != 0)
                {
                    for (int i = 0; i < strlist.Length; i++)
                    {
                        SQLSentence = SQLSentence + " (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + strlist[i].ToString().Trim() + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO = '" + opcode + "' )" + "OR";

                    }
                    SQLSentence = SQLSentence.Substring(0, SQLSentence.Length - 2);
                }

            }
            else
            {
                SQLSentence = SQLSentence + "(e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO = '" + opcode + "') OR (e.DEPT_NO='" + Deptcode + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO != '" + opcode + "')OR( e.DEPT_NO!='" + Deptcode + "' AND e.con_sign <> '2'  AND e.APPLY_DOCTOR_NO = '" + opcode + "' )";
            }


            dt = Helper.GetDataTable(SQLSentence);
            if (dt != null && dt.Rows.Count != 0)
            {
                Sum = Sum + dt.Rows.Count;
            }

            return Sum;
        }
        catch (Exception ex)
        {
            //return ex.Message;
            return -1;
        }
    }

    [WebMethod(Description = "Get operators ", EnableSession = false)]
    public string Getoperators(ref XmlNode ops)
    {
        if (ops == null)
        {
            XmlDocument doc = new XmlDocument();
            ops = doc.CreateElement(ElementNames.Operators);
        }
        try
        {
            string SQLSentence = "SELECT czydm, czyxm,ZXBS,PYM FROM SJ_CZYDM  Where ZXBS is null ORDER BY czydm";

            DataSet dataset = ExeuSentence(SQLSentence, hisDBType);
            if (dataset.Tables[0].Rows.Count == 0) return null;
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                XmlElement op = ops.OwnerDocument.CreateElement(ElementNames.Operator);
                op.SetAttribute(AttributeNames.Code, row[0].ToString());
                op.SetAttribute(AttributeNames.Name, row[1].ToString());
                op.SetAttribute(AttributeNames.Spell, row[3].ToString());
                ops.AppendChild(op);
            }
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    [WebMethod]
    public string FinishConsultNote(string sequence, string registryID, string RetrunDoctorID)
    {
        //	Update mr_consultation set con_sign = ‘2’ where seitial_no = sequence;
        SqlHelper Helper = new SqlHelper("HisDB");

        string strUpdate = @"UPDATE mr_consultation set  con_sign = '2',RETURN_TIME = '" + DateTime.Now + "',RETURN_DOCTOR_NO = '" + RetrunDoctorID + "' WHERE Serial_NO = '" + sequence + "' AND ZYH='" + registryID + "'";

        try
        {
            if (Helper.ExecuteNonQuery(strUpdate) != 0)
            {
                return null;

            }
            else
            {
                return "";
            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }


    [WebMethod]
    public DataSet GetFinishConsultationList(string strOpCode, string startTime, string endTime)
    {
       // return null;

        string strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(10),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(10),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2' and a.RETURN_DOCTOR_NO = '" + strOpCode + "' and a.return_time between '" + startTime + "' and  '" + endTime + "'";

        SqlHelper Helper = new SqlHelper("HisDB");
        DataSet dt = Helper.GetDataSet(strSelect);
        try
        {

            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    string selectTdjkz = @"select a.xm,a.nl,b.ksmc from tdjkz a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '"+dt.Tables[0].Rows[i]["zyh"].ToString()+"' ";
                   DataTable TempDT = Helper.GetDataTable(selectTdjkz);
                   if (TempDT != null && TempDT.Rows.Count != 0)
                   {
                       dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                       string strAge = TempDT.Rows[0]["nl"].ToString();
                       dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                       dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                   }
                   else
                   {
                       string selectTdjk =  @"select a.xm,a.nl,b.ksmc from tdjk a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '"+dt.Tables[0].Rows[i]["zyh"].ToString()+"' ";
                       TempDT = Helper.GetDataTable(selectTdjk);
                       if (TempDT != null && TempDT.Rows.Count != 0)
                       {
                           dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                           string strAge = TempDT.Rows[0]["nl"].ToString();
                           dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                           dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                       }

                   }
                }
            }
            return dt;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    [WebMethod]
    public DataSet GetFinishConsultationListQC(string startTime, string endTime)
    {
        // return null;

        string strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(10),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(10),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "'  order by d.czyxm";

        SqlHelper Helper = new SqlHelper("HisDB");
        DataSet dt = Helper.GetDataSet(strSelect);
        try
        {

            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    string selectTdjkz = @"select a.xm,a.nl,b.ksmc from tdjkz a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '" + dt.Tables[0].Rows[i]["zyh"].ToString() + "' ";
                    DataTable TempDT = Helper.GetDataTable(selectTdjkz);
                    if (TempDT != null && TempDT.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                        string strAge = TempDT.Rows[0]["nl"].ToString();
                        dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                        dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                    }
                    else
                    {
                        string selectTdjk = @"select a.xm,a.nl,b.ksmc from tdjk a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '" + dt.Tables[0].Rows[i]["zyh"].ToString() + "' ";
                        TempDT = Helper.GetDataTable(selectTdjk);
                        if (TempDT != null && TempDT.Rows.Count != 0)
                        {
                            dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                            string strAge = TempDT.Rows[0]["nl"].ToString();
                            dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                            dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                        }

                    }
                }
            }
            return dt;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    [WebMethod]
    public DataSet GetFinishConsultationListQCNew(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        // return null;

        string strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(16),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(16),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "'  order by d.czyxm";

        if (strDepartment == "$" && strDoctor == "$")
        {
            strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(16),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(16),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "'  order by d.czyxm";
        }
        if (strDepartment == "$" && strDoctor != "$")
        {
            strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(16),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(16),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "' and a.RETURN_DOCTOR_NO = '" + strDoctor + "'  order by d.czyxm";
        }
        if (strDepartment != "$" && strDoctor == "$")
        {
            strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(16),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(16),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "' and a.DEPT_NO = '" + strDepartment + "'  order by d.czyxm";
        }
        if (strDepartment != "$" && strDoctor != "$")
        {
            strSelect = @" select a.zyh,c.czyxm as '申请医生',convert(varchar(16),a.APPLY_TIME,120)
 as '申请时间',''as '患者姓名',''as '年龄','' as '申请科室',convert(varchar(16),a.return_time,120) as '回复时间',d.czyxm as '回复医生', '类别'=case when a.hzlb = 1 then '普通' when a.hzlb= '2' then '急' end
                         from mr_consultation a  LEFT JOIN sj_czydm c on a.APPLY_DOCTOR_NO = c.czydm 
                         LEFT JOIN sj_czydm d on a.RETURN_DOCTOR_NO = d.czydm where  a.con_sign = '2'  and a.return_time between '" + startTime + "' and  '" + endTime + "' and a.DEPT_NO = '" + strDepartment + "' and a.RETURN_DOCTOR_NO = '" + strDoctor + "'   order by d.czyxm";
        }


        SqlHelper Helper = new SqlHelper("HisDB");
        DataSet dt = Helper.GetDataSet(strSelect);
        try
        {

            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    string selectTdjkz = @"select a.xm,a.nl,b.ksmc from tdjkz a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '" + dt.Tables[0].Rows[i]["zyh"].ToString() + "' ";
                    DataTable TempDT = Helper.GetDataTable(selectTdjkz);
                    if (TempDT != null && TempDT.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                        string strAge = TempDT.Rows[0]["nl"].ToString();
                        dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                        dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                    }
                    else
                    {
                        string selectTdjk = @"select a.xm,a.nl,b.ksmc from tdjk a 
                                           LEFT JOIN mz_ksbm b ON a.ksbm = b.ksbm where a.zyh = '" + dt.Tables[0].Rows[i]["zyh"].ToString() + "' ";
                        TempDT = Helper.GetDataTable(selectTdjk);
                        if (TempDT != null && TempDT.Rows.Count != 0)
                        {
                            dt.Tables[0].Rows[i]["患者姓名"] = TempDT.Rows[0]["xm"].ToString();
                            string strAge = TempDT.Rows[0]["nl"].ToString();
                            dt.Tables[0].Rows[i]["年龄"] = strAge.Split('.')[0].ToString();
                            dt.Tables[0].Rows[i]["申请科室"] = TempDT.Rows[0]["ksmc"].ToString();
                        }

                    }
                }
            }
            return dt;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    [WebMethod]
    public DataSet GetQCTableByMonth(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        string strSelect = "";
         if (strDepartment == "$" && strDoctor == "$")
        {
           strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "'";
         }
         if (strDepartment == "$" && strDoctor != "$")
         {
              strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "'";
         }
         if (strDepartment != "$" && strDoctor == "$")
         {
              strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "'and  DepartmentCode = '" + strDepartment + "'";
         }
         if (strDepartment != "$" && strDoctor != "$")
         {
              strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "' and  DepartmentCode = '" + strDepartment + "'";
         }

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = Helper.GetDataSet(strSelect);


         
          try
          {
              if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
              {
                  SqlHelper HelperHis = new SqlHelper("HisDB");
                  int i = 0;
                  int count = dt.Tables[0].Rows.Count;

              while(i<count)
              {
                 
                      string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";
                  
                 
                      DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);

                      string strSelectScore = @"select sum(score) as score from valuatedetail 
                                            where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";

                     DataTable dtScore = Helper.GetDataTable(strSelectScore);
                     if (dtScore != null && dtScore.Rows.Count != 0)
                     {
                         dt.Tables[0].Rows[i]["得分"] = dtScore.Rows[0]["score"].ToString();
                     }

                      if (dtTemp != null && dtTemp.Rows.Count != 0)
                      {
                          dt.Tables[0].Rows[i]["出院日期"] = dtTemp.Rows[0]["出院日期"];
                          dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                          dt.Tables[0].Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                          dt.Tables[0].Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                          dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                      }
                   
                      i++;
                  }

              }
    
          }
          catch (Exception ex)
          {
              dt = null;
          }
          return dt;
    }
    /// <summary>
    /// 现岗评分查询
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="strDepartment"></param>
    /// <param name="strDoctor"></param>
    /// <returns></returns>
    [WebMethod]
    public DataSet GetQCTableByMonthNew(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        ////string strSelect = "";
        ////if (strDepartment == "$" && strDoctor == "$")
        ////{
        ////    strSelect = "select ValuateScore.opDate as '检查日期' ,'' as  '主管医师','' as '科室','' as  '主管医师','' as '病案号',ValuateScore.registryID,ValuateScore.Score as '得分' ,'病历级别' = case when ValuateScore.ScoreLevel='0' then '甲'  when ValuateScore.ScoreLevel='1' then '乙' when ValuateScore.ScoreLevel='2' then '丙' when ValuateScore.ScoreLevel='3' then '不及格' end  from  ValuateScore Left JOIN  valuatedetail on ValuateScore.registryid = valuatedetail.registryid where ValuateScore.opDate between  '" + startTime + "' and '" + endTime + "' order by ValuateScore.opDate";
        ////}
        ////if (strDepartment == "$" && strDoctor != "$")
        ////{
        ////    strSelect = "select ValuateScore.opDate as '检查日期' ,'' as  '主管医师','' as '科室','' as  '主管医师','' as '病案号',ValuateScore.registryID,ValuateScore.Score as '得分' ,'病历级别' = case when ValuateScore.ScoreLevel='0' then '甲'  when ValuateScore.ScoreLevel='1' then '乙' when ValuateScore.ScoreLevel='2' then '丙' when ValuateScore.ScoreLevel='3' then '不及格' end  from  ValuateScore Left JOIN  valuatedetail on ValuateScore.registryid = valuatedetail.registryid where ValuateScore.opDate between  '" + startTime + "' and '" + endTime + "' and ValuateScore.DoctorID = '" + strDoctor + "' order by ValuateScore.opDate ";
        ////}
        ////if (strDepartment != "$" && strDoctor == "$")
        ////{
        ////    strSelect = "select ValuateScore.opDate as '检查日期' ,'' as  '主管医师','' as '科室','' as  '主管医师','' as '病案号',ValuateScore.registryID,ValuateScore.Score as '得分' ,'病历级别' = case when ValuateScore.ScoreLevel='0' then '甲'  when ValuateScore.ScoreLevel='1' then '乙' when ValuateScore.ScoreLevel='2' then '丙' when ValuateScore.ScoreLevel='3' then '不及格' end  from  ValuateScore Left JOIN  valuatedetail on ValuateScore.registryid = valuatedetail.registryid where ValuateScore.opDate between  '" + startTime + "' and '" + endTime + "'  and  ValuateScore.DepartmentCode = '" + strDepartment + "' order by ValuateScore.opDate";
        ////}
        ////if (strDepartment != "$" && strDoctor != "$")
        ////{
        ////    strSelect = "select ValuateScore.opDate as '检查日期' ,'' as  '主管医师','' as '科室','' as  '主管医师','' as '病案号',ValuateScore.registryID,ValuateScore.Score as '得分' ,'病历级别' = case when ValuateScore.ScoreLevel='0' then '甲'  when ValuateScore.ScoreLevel='1' then '乙' when ValuateScore.ScoreLevel='2' then '丙' when ValuateScore.ScoreLevel='3' then '不及格' end  from  ValuateScore Left JOIN  valuatedetail on ValuateScore.registryid = valuatedetail.registryid where ValuateScore.opDate between  '" + startTime + "' and '" + endTime + "'  and ValuateScore.DoctorID = '" + strDoctor + "' and  ValuateScore.DepartmentCode = '" + strDepartment + "' order by ValuateScore.opDate";
        ////}

        string strSelect = "";
        if (strDepartment == "$" && strDoctor == "$")
        {
            strSelect = "select  convert(varchar(10),ValuateScore.opDate,120) as '检查日期' ,'' as  '扣分原因','' as  '扣分','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "'";
        }
        if (strDepartment == "$" && strDoctor != "$")
        {
            strSelect = "select convert(varchar(10),ValuateScore.opDate,120) as '检查日期' ,'' as  '扣分原因','' as  '扣分','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "'";
        }
        if (strDepartment != "$" && strDoctor == "$")
        {
            strSelect = "select convert(varchar(10),ValuateScore.opDate,120) as '检查日期' ,'' as  '扣分原因','' as  '扣分','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "'and  DepartmentCode = '" + strDepartment + "'";
        }
        if (strDepartment != "$" && strDoctor != "$")
        {
            strSelect = "select convert(varchar(10),ValuateScore.opDate,120) as '检查日期' ,'' as  '扣分原因','' as  '扣分','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScore  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "' and  DepartmentCode = '" + strDepartment + "'";
        }                                               

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = Helper.GetDataSet(strSelect);



        try
        {
            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                SqlHelper HelperHis = new SqlHelper("HisDB");
                int i = 0;
                int count = dt.Tables[0].Rows.Count;

                while (i < count)
                {

                    string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";

                    string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";


                    DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);
                    if (dtTemp != null && dtTemp.Rows.Count != 0) dtTemp = HelperHis.GetDataTable(strSelectHIS); 
                    else
                        dtTemp = HelperHis.GetDataTable(strSelectHISOut);

                    string strSelectScore = @"select sum(score) as score from valuatedetail 
                                            where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";

                    DataTable dtScore = Helper.GetDataTable(strSelectScore);
                    if (dtScore != null && dtScore.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["得分"] = dtScore.Rows[0]["score"].ToString();
                    }
                    //string strSum = "";

                        string strSelectReason = @"select  NoteID,Flaws from ValuateDetail  where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";
                        DataTable dtReason = Helper.GetDataTable(strSelectReason);
                        if (dtReason != null && dtReason.Rows.Count != 0)
                        {
                            decimal ScoreKnockoff = 0;
                            string strSumTemp = "";
                            for (int j = 0; j < dtReason.Rows.Count; j++)
                            {
                                string str = dtReason.Rows[j]["Flaws"].ToString();
                                XmlDocument xmldoc = new XmlDocument();
                                xmldoc.LoadXml(str);
                                XmlElement Root = xmldoc.DocumentElement;
                                string strName =   Root.Attributes["NoteName"].Value.ToString();

                                string strTemp = strName + ":";
                                foreach (XmlNode Node in Root.ChildNodes)
                                {
                                    string strKnockoff = Node.Attributes["Knockoff"].Value.ToString();
                                    string strReason = Node.InnerText;
                                    strTemp = strTemp + strReason + " " + strKnockoff + "\r";
                                    ScoreKnockoff = ScoreKnockoff + Convert.ToDecimal(strKnockoff);
                                }
                           
                                string strSelectOther = "Select OffScore,Reason  From  ValueOffEx Where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "' and  NoteID = '" + dtReason.Rows[j]["NoteID"].ToString() + "'";
                                DataTable dtOther = Helper.GetDataTable(strSelectOther);
                                if (dtOther != null)
                                {
                                    for (int k = 0; k < dtOther.Rows.Count; k++)
                                    {
                                        ScoreKnockoff = ScoreKnockoff + Convert.ToDecimal(dtOther.Rows[k]["OffScore"].ToString());
                                        strTemp = strTemp + dtOther.Rows[k]["Reason"].ToString().Trim() + " " + dtOther.Rows[k]["OffScore"].ToString().Trim() + "\r";
                                    }
                                }

                                strSumTemp = strSumTemp + strTemp;
                            }

                            dt.Tables[0].Rows[i]["扣分原因"] = strSumTemp;
                            dt.Tables[0].Rows[i]["扣分"] = ScoreKnockoff.ToString();
                        }
                

                

                    if (dtTemp != null && dtTemp.Rows.Count != 0)
                    {

                        dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                        dt.Tables[0].Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                        //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                        dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                    }

                    i++;
                }

            }

        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;
    }

    [WebMethod]
    public bool InSertValueOff(string RegistryID,string NoteID,string Reason,string OffScore)
    {
        bool blResult = false;
        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelect = "select * from ValueOffEx where RegistryID = '" + RegistryID + "' and NoteID = '" + NoteID + "'";
        string strInsert = "Insert Into ValueOffEx (RegistryID,NoteID,Reason,OffScore) Values('" + RegistryID + "','" + NoteID + "','" + Reason + "','" + OffScore + "')";
        string strUpdate = "Update ValueOffEx set Reason = '" + Reason + "' ,OffScore = '" + OffScore + "' where  RegistryID = '"+RegistryID+"' and NoteID = '"+NoteID+"' ";

        try
        {
            DataTable dtSearch = new DataTable();
           dtSearch = Helper.GetDataTable(strSelect);
            if(dtSearch!=null&& dtSearch.Rows.Count!=0)
            {
                Helper.ExecuteSql(strUpdate);
            }
            else
            {
                 Helper.ExecuteSql(strInsert);
            }
           
            blResult = true;

        }
        catch (Exception ex)
        {
            blResult = false;
        }
        return blResult;
    }
    [WebMethod]
    public DataSet GetQCTableByMonthEnd(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        string strSelect = "";

        if (strDepartment == "$" && strDoctor == "$")
        {
            strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "'";
        }
        if (strDepartment == "$" && strDoctor != "$")
        {
            strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "'";
        }
        if (strDepartment != "$" && strDoctor == "$")
        {
            strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "'and  DepartmentCode = '" + strDepartment + "'";
        }
        if (strDepartment != "$" && strDoctor != "$")
        {
            strSelect = "select '' as '出院日期' ,'' as '患者姓名','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "' and  DepartmentCode = '" + strDepartment + "'";
        } 

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = Helper.GetDataSet(strSelect);
        try
        {
            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                SqlHelper HelperHis = new SqlHelper("HisDB");
                int i = 0;
                int count = dt.Tables[0].Rows.Count;

                while (i < count)
                {
                    string strSelectHIS = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";
                    DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);

                    string strSelectScore = @"select sum(score) as score from valuatedetailEnd 
                                            where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";

                    DataTable dtScore = Helper.GetDataTable(strSelectScore);
                    if (dtScore != null && dtScore.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["得分"] = dtScore.Rows[0]["score"].ToString();
                    }

                    if (dtTemp != null && dtTemp.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["出院日期"] = dtTemp.Rows[0]["出院日期"];
                        dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                        dt.Tables[0].Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                        dt.Tables[0].Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                        dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                    }

                    i++;
                }

            }

        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;
    }
    [WebMethod]
    public DataSet GetQCTableByMonthEndNew(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        string strSelect = "";

        if (strDepartment == "$" && strDoctor == "$")
        {
            strSelect = "select '' as '出院日期' ,'' as  '扣分原因','' as  '扣分','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "'";
        }
        if (strDepartment == "$" && strDoctor != "$")
        {
            strSelect = "select '' as '出院日期' ,'' as  '扣分原因','' as  '扣分','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "'";
        }
        if (strDepartment != "$" && strDoctor == "$")
        {
            strSelect = "select '' as '出院日期' ,'' as  '扣分原因','' as  '扣分','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "'and  DepartmentCode = '" + strDepartment + "'";
        }
        if (strDepartment != "$" && strDoctor != "$")
        {
            strSelect = "select '' as '出院日期' ,'' as  '扣分原因','' as  '扣分','' as  '主管医师','' as '科室','' as  '主管医师','' as 病案号,registryID,Score as '得分' ,'病历级别' = case when ScoreLevel='0' then '甲'  when ScoreLevel='1' then '乙' when ScoreLevel='2' then '丙' when ScoreLevel='3' then '不及格' end  from  ValuateScoreEnd  where opDate between  '" + startTime + "' and '" + endTime + "' and DoctorID = '" + strDoctor + "' and  DepartmentCode = '" + strDepartment + "'";
        }

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = Helper.GetDataSet(strSelect);

        decimal OtherScoreOff = 0;
        try
        {
            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                SqlHelper HelperHis = new SqlHelper("HisDB");
                int i = 0;
                int count = dt.Tables[0].Rows.Count;

                while (i < count)
                {
                    string strSelectHIS = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";
                    string strSelectHISIn = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where zyh = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";

                    DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);
                    if (dtTemp == null || dtTemp.Rows.Count == 0) 
                         dtTemp = HelperHis.GetDataTable(strSelectHISIn);


                    string strSelectScore = @"select sum(score) as score from valuatedetailEnd 
                                            where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";


                    string strSelectReason = @"select  NoteID,Flaws from ValuateDetailEnd  where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "'";
                    DataTable dtReason = Helper.GetDataTable(strSelectReason);
                    if (dtReason != null && dtReason.Rows.Count != 0)
                    {
                        decimal ScoreKnockoff = 0;
                        string strSumTemp = "";
                        for (int j = 0; j < dtReason.Rows.Count; j++)
                        {
                            string str = dtReason.Rows[j]["Flaws"].ToString();
                            XmlDocument xmldoc = new XmlDocument();
                            xmldoc.LoadXml(str);
                            XmlElement Root = xmldoc.DocumentElement;
                            string strName = Root.Attributes["NoteName"].Value.ToString();

                            string strTemp = strName + ":";
                            foreach (XmlNode Node in Root.ChildNodes)
                            {
                                string strKnockoff = Node.Attributes["Knockoff"].Value.ToString();
                                string strReason = Node.InnerText;
                                strTemp = strTemp + strReason + " " + strKnockoff + "\r";
                                ScoreKnockoff = ScoreKnockoff + Convert.ToDecimal(strKnockoff);
                            }

                            string strSelectOther = "Select OffScore,Reason  From  ValueOffEx Where registryid = '" + dt.Tables[0].Rows[i]["registryID"].ToString() + "' and  NoteID = '" + dtReason.Rows[j]["NoteID"].ToString() + "'";
                            DataTable dtOther = Helper.GetDataTable(strSelectOther);
                            if (dtOther != null)
                            {
                                for (int k = 0; k < dtOther.Rows.Count; k++)
                                {
                                    ScoreKnockoff = ScoreKnockoff + Convert.ToDecimal(dtOther.Rows[k]["OffScore"].ToString());
                                    strTemp = strTemp + dtOther.Rows[k]["Reason"].ToString().Trim() + " " + dtOther.Rows[k]["OffScore"].ToString().Trim() + "\r";
                                    OtherScoreOff = OtherScoreOff + Convert.ToDecimal(dtOther.Rows[k]["OffScore"].ToString());
                                }
                            }

                            strSumTemp = strSumTemp + strTemp;
                        }

                        dt.Tables[0].Rows[i]["扣分原因"] = strSumTemp;
                        dt.Tables[0].Rows[i]["扣分"] = ScoreKnockoff.ToString();
                    }
                


                    if (dtTemp != null && dtTemp.Rows.Count != 0)
                    {
                        dt.Tables[0].Rows[i]["出院日期"] = dtTemp.Rows[0]["出院日期"];
                        dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                        dt.Tables[0].Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                       /// dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                        dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                    }

                    DataTable dtScore = Helper.GetDataTable(strSelectScore);
                    if (dtScore != null && dtScore.Rows.Count != 0)
                    {
                        ////int iScore = Convert.ToInt32(dtScore.Rows[0]["score"].ToString()) - OtherScoreOff;
                        decimal iScore = Convert.ToDecimal(dtScore.Rows[0]["score"].ToString());

                        dt.Tables[0].Rows[i]["得分"] = iScore - Convert.ToDecimal(OtherScoreOff);
                    }

                    i++;
                }

            }

        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;
    }
    [WebMethod]
    public bool IsInHospital(string strRegistryID)
    {
        bool blResult = false;
        try
        {
            string strSelect = "select zyh from tdjkz where zyh = '" + strRegistryID + "'";
            SqlHelper Helper = new SqlHelper("HisDB");
            DataTable dt = Helper.GetDataTable(strSelect);
           
            if (dt != null && dt.Rows.Count != 0)
            {
                blResult = true;
            }
        }
        catch (Exception ex)
        {
           
        }
        return blResult;
    }

    [WebMethod]
    public DataSet GetDepartmentList()
    {
        SqlHelper Helper = new SqlHelper("HisDB");
        string SQLSentence = @"SELECT ksbm, ksmc FROM mz_ksbm WHERE
                    ksty is null and  czbs='1' ORDER BY ksmc ";
        try
        {
            //DataTable dt = new DataTable();
            DataSet dt = Helper.GetDataSet(SQLSentence);
            return dt;

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    [WebMethod(Description = "Returns codes, names")]
    public DataSet GetDoctorList()
    {
        DataSet myDataSet = new DataSet();
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "SELECT a.ysbm, a.ysm " +
                "FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm " +
                "WHERE  a.lb = '1'  ORDER BY a.ysm";
            myDataSet = ExeuSentence(SQLSentence, hisDBType);
            //dt = myDataSet.Tables[0];
           
        } //try end
        catch (Exception ex)
        {
            myDataSet = null;
            
        }
        return myDataSet;
    }
    [WebMethod(Description = "Returns codes, names")]
    public DataSet GetDoctorListDg(string zcbm)
    {
        DataSet myDataSet = new DataSet();
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "SELECT a.ysbm, a.ysm " +
                "FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm " +
                "WHERE  a.lb = '1'  and b.zcbm='" + zcbm + "'  ORDER BY a.ysm";
            myDataSet = ExeuSentence(SQLSentence, hisDBType);
            //dt = myDataSet.Tables[0];

        } //try end
        catch (Exception ex)
        {
            myDataSet = null;

        }
        return myDataSet;
    }
    [WebMethod(Description = "Returns codes, names")]
    public DataSet GetDoctorListByDepartment(string strDepartmentCode)
    {
        DataSet dt = new DataSet();
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "SELECT a.ysbm, a.ysm " +
                "FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm " +
                "WHERE  a.lb = '1' and a.ksbm = '"+strDepartmentCode+"' ORDER BY a.ysm";
            dt = ExeuSentence(SQLSentence, hisDBType);
           

        } //try end
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod(Description = "Returns codes, names")]
    public DataSet GetDoctorListByDepartmentAndTitle(string strDepartmentCode, string Title)
    {
        DataSet dt = new DataSet();
        try
        { //string SQLSentence1="";
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            if (Title == "主任医师") Title = "0111";
            else if (Title == "主治医师") Title = "0131";
            else if (Title == "主管医师") Title = "0141";

            string SQLSentence = "SELECT a.ysbm, a.ysm " +
                "FROM tysm a LEFT OUTER JOIN zcbm b ON a.zcdm = b.zcbm " +
                "WHERE  a.lb = '1' and b.zcbm='" + Title + "'";
            if(strDepartmentCode =="全部")
                SQLSentence = SQLSentence + "' ORDER BY a.ysm";
                  else
               SQLSentence = SQLSentence + "and a.ksbm ='" + strDepartmentCode + "'  ORDER BY a.ysm";

            dt = ExeuSentence(SQLSentence, hisDBType);


        } //try end
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod(Description = "Returns codes, names")]
    public DataSet GetDepartmentByDoctor(string strCode)
    {
        DataSet dt = new DataSet();
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "SELECT ksbm, ksmc FROM tysm WHERE  tysm.lb = '1' and tysm.ysbm = '" + strCode + "' ";
            dt = ExeuSentence(SQLSentence, hisDBType);
            //dt = myDataSet.Tables[0];

        } //try end
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod]
    public DataSet GetIdentification()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelect = "select IdentID,IdentName,IdentChoice FROM TB_Identification";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(strSelect);
        }
        catch (Exception ex)
        {
            dt = null;


        }
        return dt;
    }
    [WebMethod]
    public DataSet GetDescription()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelect = "select Description,IdentID,DescTittle,DescTag FROM TB_Description";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(strSelect);
        }
        catch (Exception ex)
        {
            dt = null;


        }
        return dt;
    }
    [WebMethod]
    public void UpdateIdentification(string identName, string identId, string identChoice, bool getBool)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string delete = "DELETE  FROM TB_Identification WHERE IdentName = '" + identName
            + "'";
        string insert = "INSERT INTO TB_Identification VALUES('" + identId + "','" + identName
            + "','" + identChoice + "')";
   
        DataTable dt = new DataTable();
        try
        {
            if (getBool)
                Helper.ExecuteSql(insert);
            else
                Helper.ExecuteSql(delete);
      

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
    }
    [WebMethod]
    public DataSet GetDesc(string identName)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        //string strSelect = "select Description,IdentID,DescTittle FROM TB_Description Where IdentId = '" + identId
        //    + "'";
        string strSelects = "select * FROM TB_Description  WHERE (select IdentID FROM TB_Identification WHERE IdentName = '" + identName
       + "' )=IdentID";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(strSelects);
        }
        catch (Exception ex)
        {
            dt = null;
            
        }
        return dt;
    }
    [WebMethod]
    public DataSet GetIdents()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelect = "select IdentID,IdentName,IdentChoice FROM TB_Identification ORDER by IdentName";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(strSelect);
        }
        catch (Exception ex)
        {
            dt = null;


        }
        return dt;
    }
    [WebMethod]
    public void UpdateDescription(string description, string identId, string descTittle, int getNum)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string delete = "DELETE  FROM TB_Description WHERE Description = '" + description
            + "'";
        string insert = "INSERT INTO TB_Description VALUES('" + description + "','" + identId
            + "','" + descTittle + "','"+ "')";
        string deletes = "DELETE  FROM TB_Description WHERE IdentID = '" + identId
            + "'";
        DataTable dt = new DataTable();
        try
        {
            switch (getNum)
            {
                case 1:
                    Helper.ExecuteSql(insert);
                    break;
                case 2:
                    Helper.ExecuteSql(delete);
                    break;
                case 3:
                    Helper.ExecuteSql(deletes);
                    break;
            }


        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
    }
    [WebMethod]
    public string  GetPatientClinicID(string strRegistryID)
    {
        SqlHelper Helper = new SqlHelper("ClinicDB");
        string strSelect = "select mzh from mz_ysz_zzy where zyh = '" + strRegistryID + "' ";
        DataTable dt = new DataTable();
        string  strResesult = "";
        try
        {
            dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                strResesult = dt.Rows[0]["mzh"].ToString();
             
            }

        }
        catch (Exception ex)
        {
            /* Error ocuured */
           // return;
        }
        return strResesult;
    }
    [WebMethod]
    public string ReturnConsultNote(string sequence, string registryID)
    {

        SqlHelper Helper = new SqlHelper("HisDB");

        string strUpdate = @"UPDATE mr_consultation set  con_sign = '1',RETURN_TIME =null,RETURN_DOCTOR_NO = null WHERE Serial_NO = '" + sequence + "' AND ZYH='" + registryID + "'";

        try
        {
            if (Helper.ExecuteNonQuery(strUpdate) != 0)
            {
                return null;

            }
            else
            {
                return "";
            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }

    }
    [WebMethod]
    public void UpdateOperators(string code, string upcode)
    {
        string str = "UPDATE Operator set GuideCode = '" + upcode + "' WHERE Code = '" + code + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");

        try
        {
            Helper.ExecuteSql(str);


        }
        catch (Exception ex)
        {

        }
        return;
    }
    [WebMethod]
    public DataSet GetOpInf(string code)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT * FROM  Operator  WHERE code = '" + code + "'";
        DataSet dt = new DataSet();
        try
        {
            dt = Helper.GetDataSet(SELECT);
        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;

    }
    [WebMethod]
    public DataSet IsGuideD(string code)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT * FROM  Operator  WHERE guidecode = '" + code + "'";
        DataSet dt = new DataSet();
        //bool blResult = false;
        try
        {
            dt = Helper.GetDataSet(SELECT);

        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;

    }
    [WebMethod(Description = "Return inpatient info", EnableSession = false)]
    public DataSet GetPatientInfZY(string code)
    {
        string sql = "select Name from batsxx where type = 'ZYXZ' and BH='"+code+"'";

        DataSet dt = new DataSet();
        try
        {
            dt = ExeuSentence(sql, hisDBType);           

        }
        catch (Exception ex)
        {
            return null;
        }
        return dt;
 
    }
    [WebMethod(Description = "Return inpatient info", EnableSession = false)]
    public DataSet GetPatientInf( string code)
    {


        string SQLSentence = "SELECT a.bah,a.xm,a.xb,a.bqmc,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz,a.sfzh,a.bffh," +
                        "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh " +
                        "FROM  tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                        "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh =  '" + code + "'";
        string SQLSentences = "SELECT a.bah,a.xm,a.xb,a.bqmc,a.csny,b.mz,c.hf,d.jg,a.zy,a.jtzz,a.bffh," +
                "a.zyh,a.ch,a.zyrq,a.zrys,a.ksbm,a.cyrq,a.nl,a.nldw,a.dh " +
                "FROM  tdjk a LEFT JOIN mzdm b ON a.mz=b.mzdm LEFT JOIN hfbm c ON a.hf=c.hfbm " +
                "LEFT JOIN jgbm d ON a.jg=d.jgbm WHERE a.zyh =  '" + code + "'";
        DataSet dt = new DataSet();
        try
        {
           dt = ExeuSentence(SQLSentence, hisDBType);
            if(dt.Tables[0].Rows.Count==0)
                dt = ExeuSentence(SQLSentences, hisDBType);

        }
        catch (Exception ex)
        {
            return null;
        }
        return dt;
    }



    ////////////////////////////////////////大港油田----20100513/////////////////////////////////////


    [WebMethod(Description = "Return inpatient'child info", EnableSession = false)]
    public DataSet GetCPatientInf(string code)
    {


        string SQLSentence = "SELECT * FROM  bq_erqk WHERE zyh =  '" + code + "'";
        DataSet dt = new DataSet();
        try
        {
            dt = ExeuSentence(SQLSentence, hisDBType);

        }
        catch (Exception ex)
        {
            return null;
        }
        return dt;
    }
    [WebMethod(Description = "Returns department list")]
    public DataSet GetDepartmentListByModeDS(EmrConstant.WorkMode mode)
    {
        string SQLSentence = null;
        DataSet ds = new DataSet();
        switch (mode)
        {
            case WorkMode.InHospital:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND " +
                    "ksty is null ORDER BY ksbm";
                //SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND " +
                //    "(ghbs is null OR ghbs = '0') AND ksty is null ORDER BY ksmc";
                break;
            case WorkMode.OutHospital:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE ghbs = '1' AND ksty is null ORDER BY ksbm";
                break;
            case WorkMode.Service:
                SQLSentence = "SELECT ksbm, ksmc FROM mz_ksbm WHERE kslx = '5' AND ksty is null ORDER BY ksbm";
                break;
        }

        try
        {
            ds = ExeuSentence(SQLSentence, hisDBType);


        }
        catch (Exception ex)
        {
            ds = null;

        }
        return ds;
    }

    [WebMethod(Description = "Returns archive Patient list")]
    public DataSet GetPatientListByEmrstatus(EmrConstant.QueryMode mode, string criteria, bool archive)
    {
        string SQLSentence = null;
        DataSet ds = new DataSet();
       
            switch (mode)
            {
                #region Commpose
                case QueryMode.Commpose:
                    string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                    string whereSentence = null;
                    SQLSentence = "SELECT zyh, bah, xb, xm, mz_ksbm.ksmc as ksmc, zrys,zyrq, cyrq,emrgdsj, case emrstatus when '1' then '已归档' else '未归档' end as emrstatus FROM tdjk,mz_ksbm where tdjk.ksbm = mz_ksbm.ksbm and ";
                    if (items[0].Length > 0)
                        whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
                    if (items[1] != EmrConstant.StringGeneral.Both)
                    {
                        whereSentence += "tdjk.xb='" + items[1] + "' AND ";
                    }
                    if (archive)
                        whereSentence += "tdjk.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                    else whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                    if (items[4].Length > 0) whereSentence += " AND tdjk.zrys='" + items[4] + "'";
                    if (items[5].Length > 0) whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
                    if (items.Length >= 7)
                    {
                        switch (items[6])
                        {
                            case "1":
                                whereSentence += " AND tdjk.emrstatus= 1";
                                break;
                            case "2":
                                whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                                break;
                            case "3":
                                break;
                        }
                        //if (items[6].Length > 0) whereSentence += " AND tdjk.emrstatus=" + items[6];
                    }
                    whereSentence += " AND zyrq > '2011-03-01 00:00:00'";          
                    whereSentence += " ORDER BY tdjk.ksbm";
                    SQLSentence += whereSentence;
                    ds = ExeuSentence(SQLSentence, hisDBType);
                    break;
                #endregion
           

           
        }
        return ds;
    }
    [WebMethod(Description = "Returns Medical's DataSet")]
    public void GetNewMedicalData(ref DataSet dst1, ref DataSet dst2, ref DataSet dst3, ref DataSet dst4, ref DataSet dt, string RegistryID)
    {
        try
        {
            SqlHelper Helper = new SqlHelper("HisDB");
            string strSql1 = "SELECT basyzd.blh,jgbm.jg,tdjkz.csd_s,tdjkz.csd_x,tdjkz.xsercstz,tdjkz.xserrytz,tdjkz.jkkh,tdjkz.ylfkfs,tdjkz.zyh,   tdjkz.bah,   tdjkz.ch,   tdjkz.ch as cych,   tdjkz.xm,   tdjkz.xb,   tdjkz.mzzd,   tdjkz.zyrq,   tdjkz.zzysbm,   tdjkz.ryqk,   tdjkz.nl,   tdjkz.csny,   tdjkz.hf,   tdjkz.gjdm,   tdjkz.mz,   tdjkz.sfzh,   tdjkz.hkdz,   tdjkz.gzdw,   tdjkz.dh,   tdjkz.gz_yzbm,   tdjkz.hk_yzbm,   tdjkz.jg,   tdjkz.lxr,   tdjkz.lxrgx,   tdjkz.zy,   tdjkz.lxr_dz,   tdjkz.lxr_dh,   tdjkz.ksbm,   tdjkz.ryksbm,   tdjkz.zkksbm,   tdjkz.cyrq,   tdjkz.gms,   tdjkz.bxbs,   tdjkz.mzzdmc,   tdjkz.zyxh,  basyzd.zyh,basyzd.xx,basyzd.rh,basyzd.zrhs,   basyzd.ryzdbm,   basyzd.ryzd,   basyzd.ryzdrq,   basyzd.ryzdys,   basyzd.qzzdbm,   basyzd.qzzd,   basyzd.qzzdrq,   basyzd.qzzdys,   basyzd.cyzdbm,   basyzd.cyzd,   basyzd.cyzdrq,   basyzd.cyzdys,   basyzd.cyqk,   basyzd.qtzdbm,   basyzd.qtzd,   basyzd.qtcyqk,   basyzd.qtzdbm2,   basyzd.qtzd2,   basyzd.qtcyqk2,   basyzd.qtzdbm3,   basyzd.qtzd3,   basyzd.qtcyqk3,   basyzd.qtzdbm4,   basyzd.qtzd4,   basyzd.qtcyqk4,   basyzd.qtzdbm5,   basyzd.qtzd5,   basyzd.qtcyqk5,   basyzd.yngrbm,   basyzd.yngr,   basyzd.grcyqk,   basyzd.blzdbm,   basyzd.blzd,   basyzd.sszdbm,   basyzd.sszd,   basyzd.hbsag,   basyzd.hcvab,   basyzd.hivab,   basyzd.mcfh,   basyzd.rcfh,   basyzd.sqshfh,   basyzd.lcblfh,   basyzd.fsblfh,   basyzd.qjcs,   basyzd.qjcgcs,   basyzd.mjzzd,   basyzd.ryqk,   basyzd.kzr,   basyzd.zrys,   basyzd.zzys,   basyzd.zyys,   basyzd.jxys,   basyzd.yjssxys,   basyzd.sxys,   basyzd.qtzdbm6,   basyzd.qtzd6,   basyzd.qtcyqk6,   basyzd.ywgm,basyzd.sfsj,tdjkz.nldw, '' as cyqk01, '' as cyqk02, '' as cyqk03, '' as cyqk04, '' as cyqk05, '' as cyqk11, '' as cyqk12, '' as cyqk13, '' as cyqk14, '' as cyqk15, '' as cyqk21, '' as cyqk22, '' as cyqk23, '' as cyqk24, '' as cyqk25, '' as cyqk31, '' as cyqk32, '' as cyqk33, '' as cyqk34, '' as cyqk35, '' as cyqk41, '' as cyqk42, '' as cyqk43, '' as cyqk44, '' as cyqk45, '' as cyqk51, '' as cyqk52, '' as cyqk53, '' as cyqk54, '' as cyqk55, '' as hznl, '' as csrq_n, '' as csrq_y, '' as csrq_r, '' as zyrq_n, '' as zyrq_y, '' as zyrq_r, '' as zyrq_s, '' as cyrq_n, '' as cyrq_y, '' as cyrq_r, '' as cyrq_s, '' as zycs, '' as ryzdrq_n, '' as ryzdrq_y, '' as ryzdrq_r, '' as zyts ,'' as caselevel,'' as zjys,'' as zkhs,'' as mbm FROM basyzd, tdjkz left join jgbm on jgbm.jgbm = tdjkz.jg where tdjkz.zyh = basyzd.zyh and tdjkz.zyh ='" + RegistryID + "'"
            + " union all " + "SELECT basyzd.blh,jgbm.jg,tdjk.csd_s,tdjk.csd_x,tdjk.xsercstz,tdjk.xserrytz,tdjk.jkkh,tdjk.ylfkfs,tdjk.zyh,   tdjk.bah,   tdjk.ch,   tdjk.ch as cych,   tdjk.xm,   tdjk.xb,   tdjk.mzzd,   tdjk.zyrq,   tdjk.zzysbm,   tdjk.ryqk,   tdjk.nl,   tdjk.csny,   tdjk.hf,   tdjk.gjdm,   tdjk.mz,   tdjk.sfzh,   tdjk.hkdz,   tdjk.gzdw,   tdjk.dh,   tdjk.gz_yzbm,   tdjk.hk_yzbm,   tdjk.jg,   tdjk.lxr,   tdjk.lxrgx,   tdjk.zy,   tdjk.lxr_dz,   tdjk.lxr_dh,   tdjk.ksbm,   tdjk.ryksbm,   tdjk.zkksbm,   tdjk.cyrq,   tdjk.gms,   tdjk.bxbs,   tdjk.mzzdmc,   tdjk.zyxh,  basyzd.zyh, basyzd.xx,basyzd.rh,basyzd.zrhs,   basyzd.ryzdbm,   basyzd.ryzd,   basyzd.ryzdrq,   basyzd.ryzdys,   basyzd.qzzdbm,   basyzd.qzzd,   basyzd.qzzdrq,   basyzd.qzzdys,   basyzd.cyzdbm,   basyzd.cyzd,   basyzd.cyzdrq,   basyzd.cyzdys,   basyzd.cyqk,   basyzd.qtzdbm,   basyzd.qtzd,   basyzd.qtcyqk,   basyzd.qtzdbm2,   basyzd.qtzd2,   basyzd.qtcyqk2,   basyzd.qtzdbm3,   basyzd.qtzd3,   basyzd.qtcyqk3,   basyzd.qtzdbm4,   basyzd.qtzd4,   basyzd.qtcyqk4, basyzd.qtzdbm5,   basyzd.qtzd5,   basyzd.qtcyqk5,   basyzd.yngrbm,   basyzd.yngr,   basyzd.grcyqk,   basyzd.blzdbm,   basyzd.blzd,   basyzd.sszdbm,   basyzd.sszd,   basyzd.hbsag,   basyzd.hcvab,   basyzd.hivab,   basyzd.mcfh,   basyzd.rcfh,   basyzd.sqshfh,   basyzd.lcblfh,   basyzd.fsblfh,   basyzd.qjcs,   basyzd.qjcgcs,   basyzd.mjzzd,   basyzd.ryqk,   basyzd.kzr,   basyzd.zrys,   basyzd.zzys,   basyzd.zyys,   basyzd.jxys,   basyzd.yjssxys,   basyzd.sxys,   basyzd.qtzdbm6,   basyzd.qtzd6,   basyzd.qtcyqk6,   basyzd.ywgm,basyzd.sfsj,tdjk.nldw, '' as cyqk01, '' as cyqk02, '' as cyqk03, '' as cyqk04, '' as cyqk05, '' as cyqk11, '' as cyqk12, '' as cyqk13, '' as cyqk14, '' as cyqk15, '' as cyqk21, '' as cyqk22, '' as cyqk23, '' as cyqk24, '' as cyqk25, '' as cyqk31, '' as cyqk32, '' as cyqk33, '' as cyqk34, '' as cyqk35, '' as cyqk41, '' as cyqk42, '' as cyqk43, '' as cyqk44, '' as cyqk45, '' as cyqk51, '' as cyqk52, '' as cyqk53, '' as cyqk54, '' as cyqk55, '' as hznl, '' as csrq_n, '' as csrq_y, '' as csrq_r, '' as zyrq_n, '' as zyrq_y, '' as zyrq_r, '' as zyrq_s, '' as cyrq_n, '' as cyrq_y, '' as cyrq_r, '' as cyrq_s, '' as zycs, '' as ryzdrq_n, '' as ryzdrq_y, '' as ryzdrq_r, '' as zyts,'' as caselevel,'' as zjys,'' as zkhs,'' as mbm FROM basyzd,tdjk  left join jgbm on jgbm.jgbm = tdjk.jg  where tdjk.zyh = basyzd.zyh and tdjk.zyh  = '" + RegistryID + "'";
            dst1 = Helper.GetDataSet(strSql1);

            string strSql2 = "SELECT TFY.ZYH,   TFY.FY01,   TFY.FY02,   TFY.FY03,   TFY.FY04,   TFY.FY05,   TFY.FY06,   TFY.FY07,   TFY.FY08,   TFY.FY09,   TFY.FY10,   TFY.FY11,   TFY.FY12,   TFY.FY13,   TFY.FY14,   TFY.FY15,   TFY.FY16,   TFY.FY17,   TFY.FY18,   TFY.FY19,   TFY.FY20,   TFY.FY21,   TFY.FY22,   TFY.FY23,   TFY.FY24,   TFY.HJJE, basyzd.lyfs,basyzd.lyfs_zy1,basyzd.lyfs_zy2,basyzd.cy31tzzy,basyzd.cy31tzzy_md, basyzd.hmsj_ryq_t,basyzd.hmsj_ryq_h,basyzd.hmsj_ryq_m,basyzd.hmsj_ryh_t,basyzd.hmsj_ryh_h,basyzd.hmsj_ryh_m,basyzd.ybsm_jy, BASYZD.SFSJ,   BASYZD.BYSL,   BASYZD.SFSZ,   BASYZD.SZQX,   BASYZD.XX,   BASYZD.RH,   BASYZD.SXFY,   BASYZD.SX_HXB,   BASYZD.SX_XXB,   BASYZD.SX_XJ,   BASYZD.SX_QX,   BASYZD.SX_QT,   BASYZD.SSSM,  BASYZD.ZYH,   BASYZD.SJBL,   BASYZD.SZQXLB,   BASYZD.ERZD1,   BASYZD.ERZD2,   BASYZD.ERZD3,   BASYZD.ERZDBM1,   BASYZD.ERZDBM2,   BASYZD.ERZDBM3,   BASYZD.ERCYQK1,   BASYZD.ERCYQK2,   BASYZD.ERCYQK3,   BASYZD.ERXB,   BASYZD.ERKJM,   BASYZD.ERYGYM,   BASYZD.ERTSH,   BASYZD.ERPKU,   BASYZD.ERTZ,   BASYZD.ERAPGARPF,   BASYZD.CRBBGYB,   BASYZD.SBBGYB,basyzd.ybsm_jy,'' as CZBM1,'' as CZBM2,'' as CZBM3,'' as CZBM4,'' as CZBM5,'' as CZBM6,'' as CZBM7,'' as CZBM8,'' as SSRQ1,'' as SSRQ2,'' as SSRQ3,'' as SSRQ4,'' as SSRQ5,'' as SSRQ6,'' as SSRQ7,'' as SSRQ8,'' as SSJB1,'' as SSJB2,'' as SSJB3,'' as SSJB4,'' as SSJB5,'' as SSJB6,'' as SSJB7,'' as SSJB8,'' as SSMC1,'' as SSMC2,'' as SSMC3,'' as SSMC4,'' as SSMC5,'' as SSMC6,'' as SSMC7,'' as SSMC8, '' AS SSYS1, '' AS SSYS2, '' AS SSYS3, '' AS SSYS4,'' AS SSYS5, '' AS SSYS6, '' AS SSYS7, '' AS SSYS8, '' AS Z11, '' AS Z12, '' AS Z13, '' AS Z14,'' AS Z15, '' AS Z16, '' AS Z17, '' AS Z18, '' AS Z21, '' AS Z22, '' AS Z23, '' AS Z24, '' AS Z25, '' AS Z26, '' AS Z27, '' AS Z28,'' AS MZ1, '' AS MZ2, '' AS MZ3, '' AS MZ4, '' AS MZ5, '' AS MZ6, '' AS MZ7, '' AS MZ8,'' AS QK1, '' AS QK2, '' AS QK3, '' AS QK4,'' AS QK5, '' AS QK6, '' AS QK7, '' AS QK8, '' AS MZYS1, '' AS MZYS2, '' AS MZYS3, '' AS MZYS4, '' AS MZYS5, '' AS MZYS6, '' AS MZYS7, '' AS MZYS8, '' as ERCYQK11, '' as ERCYQK12, '' as ERCYQK13, '' as ERCYQK14, '' as ERCYQK15, '' as ERCYQK21, '' as ERCYQK22, '' as ERCYQK23, '' as ERCYQK24, '' as ERCYQK25, '' as ERCYQK31, '' as ERCYQK32, '' as ERCYQK33, '' as ERCYQK34, '' as ERCYQK35, '' as SZQX_Z, '' as SZQX_Y, '' as SZQX_N,'' as ZFJR, '' as fy_00, '' as fy_01, '' as fy_02, '' as fy_03, '' as fy_04, '' as fy_05, '' as fy_06, '' as fy_07, '' as fy_08, '' as fy_09, '' as fy_0901, '' as fy_10, '' as fy_1001, '' as fy_1002, '' as fy_11, '' as fy_12, '' as fy_13, '' as fy_1301, '' as fy_14, '' as fy_15, '' as fy_16, '' as fy_17, '' as fy_18, '' as fy_19, '' as fy_20, '' as fy_21, '' as fy_22, '' as fy_23,'' as fy_24 FROM TFY,   BASYZD    WHERE ( TFY.ZYH = BASYZD.ZYH ) and   TFY.zyh = '" + RegistryID + "'"
            + " union all " + "SELECT TFYZ.ZYH,   TFYZ.FY01,   TFYZ.FY02,   TFYZ.FY03,   TFYZ.FY04,   TFYZ.FY05,   TFYZ.FY06,   TFYZ.FY07,   TFYZ.FY08,   TFYZ.FY09,   TFYZ.FY10,   TFYZ.FY11,   TFYZ.FY12,   TFYZ.FY13,   TFYZ.FY14,   TFYZ.FY15,   TFYZ.FY16,   TFYZ.FY17,   TFYZ.FY18,   TFYZ.FY19,   TFYZ.FY20,   TFYZ.FY21,   TFYZ.FY22,   TFYZ.FY23,   TFYZ.FY24,   TFYZ.HJJE, basyzd.lyfs,basyzd.lyfs_zy1,basyzd.lyfs_zy2,basyzd.cy31tzzy,basyzd.cy31tzzy_md, basyzd.hmsj_ryq_t,basyzd.hmsj_ryq_h,basyzd.hmsj_ryq_m,basyzd.hmsj_ryh_t,basyzd.hmsj_ryh_h,basyzd.hmsj_ryh_m,basyzd.ybsm_jy, BASYZD.SFSJ,   BASYZD.BYSL,   BASYZD.SFSZ,   BASYZD.SZQX,   BASYZD.XX,   BASYZD.RH,   BASYZD.SXFY,   BASYZD.SX_HXB,   BASYZD.SX_XXB,   BASYZD.SX_XJ,   BASYZD.SX_QX,   BASYZD.SX_QT,   BASYZD.SSSM,  BASYZD.ZYH,   BASYZD.SJBL,   BASYZD.SZQXLB,   BASYZD.ERZD1,   BASYZD.ERZD2,   BASYZD.ERZD3,   BASYZD.ERZDBM1,   BASYZD.ERZDBM2,   BASYZD.ERZDBM3,   BASYZD.ERCYQK1,   BASYZD.ERCYQK2,   BASYZD.ERCYQK3,   BASYZD.ERXB,   BASYZD.ERKJM,   BASYZD.ERYGYM,   BASYZD.ERTSH,   BASYZD.ERPKU,   BASYZD.ERTZ,   BASYZD.ERAPGARPF,   BASYZD.CRBBGYB,   BASYZD.SBBGYB,basyzd.ybsm_jy,'' as CZBM1,'' as CZBM2,'' as CZBM3,'' as CZBM4,'' as CZBM5,'' as CZBM6,'' as CZBM7,'' as CZBM8,'' as SSRQ1,'' as SSRQ2,'' as SSRQ3,'' as SSRQ4,'' as SSRQ5,'' as SSRQ6,'' as SSRQ7,'' as SSRQ8,'' as SSJB1,'' as SSJB2,'' as SSJB3,'' as SSJB4,'' as SSJB5,'' as SSJB6,'' as SSJB7,'' as SSJB8,'' as SSMC1,'' as SSMC2,'' as SSMC3,'' as SSMC4,'' as SSMC5,'' as SSMC6,'' as SSMC7,'' as SSMC8, '' AS SSYS1, '' AS SSYS2, '' AS SSYS3, '' AS SSYS4,'' AS SSYS5, '' AS SSYS6, '' AS SSYS7, '' AS SSYS8, '' AS Z11, '' AS Z12, '' AS Z13, '' AS Z14,'' AS Z15, '' AS Z16, '' AS Z17, '' AS Z18, '' AS Z21, '' AS Z22, '' AS Z23, '' AS Z24, '' AS Z25, '' AS Z26, '' AS Z27, '' AS Z28,'' AS MZ1, '' AS MZ2, '' AS MZ3, '' AS MZ4, '' AS MZ5, '' AS MZ6, '' AS MZ7, '' AS MZ8,'' AS QK1, '' AS QK2, '' AS QK3, '' AS QK4,'' AS QK5, '' AS QK6, '' AS QK7, '' AS QK8, '' AS MZYS1, '' AS MZYS2, '' AS MZYS3, '' AS MZYS4, '' AS MZYS5, '' AS MZYS6, '' AS MZYS7, '' AS MZYS8, '' as ERCYQK11, '' as ERCYQK12, '' as ERCYQK13, '' as ERCYQK14, '' as ERCYQK15, '' as ERCYQK21, '' as ERCYQK22, '' as ERCYQK23, '' as ERCYQK24, '' as ERCYQK25, '' as ERCYQK31, '' as ERCYQK32, '' as ERCYQK33, '' as ERCYQK34, '' as ERCYQK35, '' as SZQX_Z, '' as SZQX_Y, '' as SZQX_N,'' as ZFJR , '' as fy_00, '' as fy_01, '' as fy_02, '' as fy_03, '' as fy_04, '' as fy_05, '' as fy_06, '' as fy_07, '' as fy_08, '' as fy_09,'' as fy_0901, '' as fy_10, '' as fy_1001, '' as fy_1002, '' as fy_11, '' as fy_12, '' as fy_13, '' as fy_1301, '' as fy_14, '' as fy_15, '' as fy_16, '' as fy_17, '' as fy_18, '' as fy_19, '' as fy_20, '' as fy_21, '' as fy_22, '' as fy_23,'' as fy_24 FROM TFYZ,   BASYZD    WHERE ( TFYZ.ZYH = BASYZD.ZYH ) and   TFYZ.zyh = '" + RegistryID + "'";
            dst2 = Helper.GetDataSet(strSql2);

            string strSql3 = "SELECT SSBM,   SSKSRQ,   SSYS1,   MZYS1,   MZBM,   SSYS2,   SSYS3,   SSDJ,   SSLB,   QKYHDJ,   SSMC  FROM SSMXK  WHERE zyh  = '" + RegistryID + "'";
            dst3 = Helper.GetDataSet(strSql3);

            string strSql4 = "SELECT yybm,yymc FROM yyjbqk";
            dst4 = Helper.GetDataSet(strSql4);

            DataSet dst5;
            string strSql5 = "select count(1) as zycs from (select zyh,bah from tdjkz union all select zyh,bah from tdjk  ) aa where bah in(select bah from(select zyh,bah from tdjkz union all select zyh,bah from tdjk  ) bb where zyh ='" + RegistryID + "')";
            dst5 = Helper.GetDataSet(strSql5);

            DataSet dst6;
            string strSql6 = "select zdmc,icd,rybq from dbo.ysz_zdrzxx where zyh = '" + RegistryID + "' and zdlx = '4'"
             + " union all " + "select zdmc,icd,rybq from dbo.ysz_zdrzxx where zyh = '" + RegistryID + "' and zdlx = '3'";
            dst6 = Helper.GetDataSet(strSql6);

            DataSet dst7;
            string strSql7 = "select caselevel,zjys,zkhs,mbm from ba_main where zyh = '" + RegistryID + "'";
            dst7 = Helper.GetDataSet(strSql7);

            DataSet dst8;
            string strSql8 = "select sum(xjzf) from tfy where zyh = '" + RegistryID + "'";
            dst8 = Helper.GetDataSet(strSql8);
            DataSet dst9;
            string strSql9 = "select sum(xjzf) from tzyfy where zyh = '" + RegistryID + "'";
            dst9 = Helper.GetDataSet(strSql9);

            DataSet dst10;
            string strSql10 = @"SELECT sum(je) as fy_00,
	       sum(case when flh5 = '01' then je else 0 end) as fy_01,
	       sum(case when flh5 = '02' then je else 0 end) as fy_02,
	       sum(case when flh5 = '03' then je else 0 end) as fy_03,
	       sum(case when flh5 = '04' then je else 0 end) as fy_04,
	       sum(case when flh5 = '05' then je else 0 end) as fy_05,
	       sum(case when flh5 = '06' then je else 0 end) as fy_06,
	       sum(case when flh5 = '07' then je else 0 end) as fy_07,
	       sum(case when flh5 = '08' then je else 0 end) as fy_08,
	       sum(case when left(flh5,2) = '09' then je else 0 end) as fy_09,
	       sum(case when flh5 = '0901' then je else 0 end) as fy_0901,
	       sum(case when left(flh5,2) = '10' then je else 0 end) as fy_10,
	       sum(case when flh5 = '1001' then je else 0 end) as fy_1001,
	       sum(case when flh5 = '1002' then je else 0 end) as fy_1002,
	       sum(case when flh5 = '11' then je else 0 end) as fy_11,
	       sum(case when flh5 = '12' then je else 0 end) as fy_12,
	       sum(case when left(flh5,2) = '13' then je else 0 end) as fy_13,
	       sum(case when flh5 = '1301' then je else 0 end) as fy_1301,
	       sum(case when flh5 = '14' then je else 0 end) as fy_14,
	       sum(case when flh5 = '15' then je else 0 end) as fy_15,
	       sum(case when flh5 = '16' then je else 0 end) as fy_16,
	       sum(case when flh5 = '17' then je else 0 end) as fy_17,
	       sum(case when flh5 = '18' then je else 0 end) as fy_18,
	       sum(case when flh5 = '19' then je else 0 end) as fy_19,
	       sum(case when flh5 = '20' then je else 0 end) as fy_20,
	       sum(case when flh5 = '21' then je else 0 end) as fy_21,
	       sum(case when flh5 = '22' then je else 0 end) as fy_22,
	       sum(case when flh5 = '23' then je else 0 end) as fy_23,
	       sum(case when flh5 = '24' or flh5 is null or rtrim(ltrim(flh5)) = '' then je else 0 end) as fy_24
from zyfy_bahs where zyh = '" + RegistryID + "'";
            dst10 = Helper.GetDataSet(strSql10);
            DataRow fy = dst2.Tables[0].Rows[0];
            //各种费用
            if (dst10.Tables[0].Rows.Count > 0)
            {
                DataRow dr10 = dst10.Tables[0].Rows[0];
                fy["fy_00"] = dr10["fy_00"].ToString();
                fy["fy_01"] = dr10["fy_01"].ToString();
                fy["fy_02"] = dr10["fy_02"].ToString();
                fy["fy_03"] = dr10["fy_03"].ToString();
                fy["fy_04"] = dr10["fy_04"].ToString();
                fy["fy_05"] = dr10["fy_05"].ToString();
                fy["fy_06"] = dr10["fy_06"].ToString();
                fy["fy_07"] = dr10["fy_07"].ToString();
                fy["fy_08"] = dr10["fy_08"].ToString();
                fy["fy_09"] = dr10["fy_09"].ToString();
                fy["fy_0901"] = dr10["fy_0901"].ToString();
                fy["fy_10"] = dr10["fy_10"].ToString();
                fy["fy_1001"] = dr10["fy_1001"].ToString();
                fy["fy_1002"] = dr10["fy_1002"].ToString();
                fy["fy_11"] = dr10["fy_11"].ToString();
                fy["fy_12"] = dr10["fy_12"].ToString();
                fy["fy_13"] = dr10["fy_13"].ToString();
                fy["fy_1301"] = dr10["fy_1301"].ToString();
                fy["fy_14"] = dr10["fy_14"].ToString();
                fy["fy_15"] = dr10["fy_15"].ToString();
                fy["fy_16"] = dr10["fy_16"].ToString();
                fy["fy_17"] = dr10["fy_17"].ToString();
                fy["fy_18"] = dr10["fy_18"].ToString();
                fy["fy_19"] = dr10["fy_19"].ToString();
                fy["fy_20"] = dr10["fy_20"].ToString();
                fy["fy_21"] = dr10["fy_21"].ToString();
                fy["fy_22"] = dr10["fy_22"].ToString();
                fy["fy_23"] = dr10["fy_23"].ToString();
                fy["fy_24"] = dr10["fy_24"].ToString();
            }
            //自付金额
            Decimal zfjrsum = 0;
            Decimal zfjrzy = 0;
            Decimal zfjr = 0;

            if ((Convert.IsDBNull(dst8.Tables[0].Rows[0][0]) == true))
            {
                zfjr = 0;
            }
            else
            {
                zfjr = Convert.ToDecimal(dst8.Tables[0].Rows[0][0]);
            }
            if ((Convert.IsDBNull(dst9.Tables[0].Rows[0][0]) == true))
            {
                zfjrzy = 0;
            }
            else
            {
                zfjrzy = Convert.ToDecimal(dst9.Tables[0].Rows[0][0]);
            }

            zfjrsum = zfjr + zfjrzy;
            //DataRow tempzfjr = dst2.Tables[0].Rows[0];
            fy["ZFJR"] = zfjr.ToString();
            //
            DataColumn zdmc = new DataColumn("zdmc", typeof(String));
            DataColumn icd = new DataColumn("icd", typeof(String));
            DataColumn rybq = new DataColumn("rybq", typeof(String));
            DataTable tep = new DataTable();
            dt.Tables.Add(tep);

            dt.Tables[0].Columns.Add(zdmc);
            dt.Tables[0].Columns.Add(icd);
            dt.Tables[0].Columns.Add(rybq);

            DataRow dr1 = null;
            dr1 = dt.Tables[0].NewRow();
            if (dst6.Tables[0].Rows.Count > 0)
            {
                dr1["zdmc"] = dst6.Tables[0].Rows[0][0].ToString();
                dr1["icd"] = dst6.Tables[0].Rows[0][1].ToString();
                dr1["rybq"] = dst6.Tables[0].Rows[0][2].ToString();
            }
            else
            {
                dr1["zdmc"] = "";
                dr1["icd"] = "";
                dr1["rybq"] = "";
            }

            for (int i = 1; i < dst6.Tables[0].Rows.Count; i++)
            {
                DataColumn temp1 = new DataColumn("zdmc" + i.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp1);
                DataColumn temp2 = new DataColumn("icd" + i.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp2);
                DataColumn temp3 = new DataColumn("rybq" + i.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp3);

                dr1["zdmc" + i.ToString("00")] = dst6.Tables[0].Rows[i][0].ToString();
                dr1["icd" + i.ToString("00")] = dst6.Tables[0].Rows[i][1].ToString();
                dr1["rybq" + i.ToString("00")] = dst6.Tables[0].Rows[i][2].ToString();

            }
            for (int j = dst6.Tables[0].Rows.Count; j < 22; j++)
            {
                DataColumn temp1 = new DataColumn("zdmc" + j.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp1);
                DataColumn temp2 = new DataColumn("icd" + j.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp2);
                DataColumn temp3 = new DataColumn("rybq" + j.ToString("00"), typeof(String));
                dt.Tables[0].Columns.Add(temp3);
                dr1["zdmc" + j.ToString("00")] = "";
                dr1["icd" + j.ToString("00")] = "";
                dr1["rybq" + j.ToString("00")] = "";
            }
            dt.Tables[0].Rows.Add(dr1);

            if (dst1.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dst1.Tables[0].Rows[0];
                if (dst5.Tables[0].Rows.Count > 0)
                {
                    DataRow dr5 = dst5.Tables[0].Rows[0];
                    dr["zycs"] = dr5["zycs"].ToString();
                }
                else
                {
                    dr["zycs"] = "1";
                }

                if (dst7.Tables[0].Rows.Count > 0)
                {
                    DataRow dr7 = dst7.Tables[0].Rows[0];
                    dr["caselevel"] = dr7["caselevel"].ToString();
                    dr["zjys"] = dr7["zjys"].ToString();
                    dr["zkhs"] = dr7["zkhs"].ToString();
                    dr["mbm"] = dr7["mbm"].ToString();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
    [WebMethod(Description = "Returns Medical's DataSet")]
    public void GetMedicalData(ref DataSet dst1, ref DataSet dst2, ref DataSet dst3, string RegistryID)
    {
        SqlHelper Helper = new SqlHelper("HisDB");
        string strSql1 = "SELECT tdjkz.zyh,   tdjkz.bah,   tdjkz.ch,   tdjkz.ch as cych,   tdjkz.xm,   tdjkz.xb,   tdjkz.mzzd,   tdjkz.zyrq,   tdjkz.zzysbm,   tdjkz.ryqk,   tdjkz.nl,   tdjkz.csny,   tdjkz.hf,   tdjkz.gjdm,   tdjkz.mz,   tdjkz.sfzh,   tdjkz.hkdz,   tdjkz.gzdw,   tdjkz.dh,   tdjkz.gz_yzbm,   tdjkz.hk_yzbm,   tdjkz.jg,   tdjkz.lxr,   tdjkz.lxrgx,   tdjkz.zy,   tdjkz.lxr_dz,   tdjkz.lxr_dh,   tdjkz.ksbm,   tdjkz.ryksbm,   tdjkz.zkksbm,   tdjkz.cyrq,   tdjkz.gms,   tdjkz.bxbs,   tdjkz.mzzdmc,   tdjkz.zyxh,  basyzd.zyh,   basyzd.ryzdbm,   basyzd.ryzd,   basyzd.ryzdrq,   basyzd.ryzdys,   basyzd.qzzdbm,   basyzd.qzzd,   basyzd.qzzdrq,   basyzd.qzzdys,   basyzd.cyzdbm,   basyzd.cyzd,   basyzd.cyzdrq,   basyzd.cyzdys,   basyzd.cyqk,   basyzd.qtzdbm,   basyzd.qtzd,   basyzd.qtcyqk,   basyzd.qtzdbm2,   basyzd.qtzd2,   basyzd.qtcyqk2,   basyzd.qtzdbm3,   basyzd.qtzd3,   basyzd.qtcyqk3,   basyzd.qtzdbm4,   basyzd.qtzd4,   basyzd.qtcyqk4,   basyzd.qtzdbm5,   basyzd.qtzd5,   basyzd.qtcyqk5,   basyzd.yngrbm,   basyzd.yngr,   basyzd.grcyqk,   basyzd.blzdbm,   basyzd.blzd,   basyzd.sszdbm,   basyzd.sszd,   basyzd.hbsag,   basyzd.hcvab,   basyzd.hivab,   basyzd.mcfh,   basyzd.rcfh,   basyzd.sqshfh,   basyzd.lcblfh,   basyzd.fsblfh,   basyzd.qjcs,   basyzd.qjcgcs,   basyzd.mjzzd,   basyzd.ryqk,   basyzd.kzr,   basyzd.zrys,   basyzd.zzys,   basyzd.zyys,   basyzd.jxys,   basyzd.yjssxys,   basyzd.sxys,   basyzd.qtzdbm6,   basyzd.qtzd6,   basyzd.qtcyqk6,   basyzd.ywgm,tdjkz.nldw, '' as cyqk01, '' as cyqk02, '' as cyqk03, '' as cyqk04, '' as cyqk05, '' as cyqk11, '' as cyqk12, '' as cyqk13, '' as cyqk14, '' as cyqk15, '' as cyqk21, '' as cyqk22, '' as cyqk23, '' as cyqk24, '' as cyqk25, '' as cyqk31, '' as cyqk32, '' as cyqk33, '' as cyqk34, '' as cyqk35, '' as cyqk41, '' as cyqk42, '' as cyqk43, '' as cyqk44, '' as cyqk45, '' as cyqk51, '' as cyqk52, '' as cyqk53, '' as cyqk54, '' as cyqk55, '' as hznl, '' as csrq_n, '' as csrq_y, '' as csrq_r, '' as zyrq_n, '' as zyrq_y, '' as zyrq_r, '' as zyrq_s, '' as cyrq_n, '' as cyrq_y, '' as cyrq_r, '' as cyrq_s, '' as zycs, '' as ryzdrq_n, '' as ryzdrq_y, '' as ryzdrq_r, '' as zyts FROM basyzd,    tdjkz   where tdjkz.zyh = basyzd.zyh and tdjkz.zyh ='" + RegistryID + "'"
        + " union all " + "SELECT tdjk.zyh,   tdjk.bah,   tdjk.ch,   tdjk.ch as cych,   tdjk.xm,   tdjk.xb,   tdjk.mzzd,   tdjk.zyrq,   tdjk.zzysbm,   tdjk.ryqk,   tdjk.nl,   tdjk.csny,   tdjk.hf,   tdjk.gjdm,   tdjk.mz,   tdjk.sfzh,   tdjk.hkdz,   tdjk.gzdw,   tdjk.dh,   tdjk.gz_yzbm,   tdjk.hk_yzbm,   tdjk.jg,   tdjk.lxr,   tdjk.lxrgx,   tdjk.zy,   tdjk.lxr_dz,   tdjk.lxr_dh,   tdjk.ksbm,   tdjk.ryksbm,   tdjk.zkksbm,   tdjk.cyrq,   tdjk.gms,   tdjk.bxbs,   tdjk.mzzdmc,   tdjk.zyxh,  basyzd.zyh,   basyzd.ryzdbm,   basyzd.ryzd,   basyzd.ryzdrq,   basyzd.ryzdys,   basyzd.qzzdbm,   basyzd.qzzd,   basyzd.qzzdrq,   basyzd.qzzdys,   basyzd.cyzdbm,   basyzd.cyzd,   basyzd.cyzdrq,   basyzd.cyzdys,   basyzd.cyqk,   basyzd.qtzdbm,   basyzd.qtzd,   basyzd.qtcyqk,   basyzd.qtzdbm2,   basyzd.qtzd2,   basyzd.qtcyqk2,   basyzd.qtzdbm3,   basyzd.qtzd3,   basyzd.qtcyqk3,   basyzd.qtzdbm4,   basyzd.qtzd4,   basyzd.qtcyqk4, basyzd.qtzdbm5,   basyzd.qtzd5,   basyzd.qtcyqk5,   basyzd.yngrbm,   basyzd.yngr,   basyzd.grcyqk,   basyzd.blzdbm,   basyzd.blzd,   basyzd.sszdbm,   basyzd.sszd,   basyzd.hbsag,   basyzd.hcvab,   basyzd.hivab,   basyzd.mcfh,   basyzd.rcfh,   basyzd.sqshfh,   basyzd.lcblfh,   basyzd.fsblfh,   basyzd.qjcs,   basyzd.qjcgcs,   basyzd.mjzzd,   basyzd.ryqk,   basyzd.kzr,   basyzd.zrys,   basyzd.zzys,   basyzd.zyys,   basyzd.jxys,   basyzd.yjssxys,   basyzd.sxys,   basyzd.qtzdbm6,   basyzd.qtzd6,   basyzd.qtcyqk6,   basyzd.ywgm,tdjk.nldw, '' as cyqk01, '' as cyqk02, '' as cyqk03, '' as cyqk04, '' as cyqk05, '' as cyqk11, '' as cyqk12, '' as cyqk13, '' as cyqk14, '' as cyqk15, '' as cyqk21, '' as cyqk22, '' as cyqk23, '' as cyqk24, '' as cyqk25, '' as cyqk31, '' as cyqk32, '' as cyqk33, '' as cyqk34, '' as cyqk35, '' as cyqk41, '' as cyqk42, '' as cyqk43, '' as cyqk44, '' as cyqk45, '' as cyqk51, '' as cyqk52, '' as cyqk53, '' as cyqk54, '' as cyqk55, '' as hznl, '' as csrq_n, '' as csrq_y, '' as csrq_r, '' as zyrq_n, '' as zyrq_y, '' as zyrq_r, '' as zyrq_s, '' as cyrq_n, '' as cyrq_y, '' as cyrq_r, '' as cyrq_s, '' as zycs, '' as ryzdrq_n, '' as ryzdrq_y, '' as ryzdrq_r, '' as zyts FROM basyzd,    tdjk   where tdjk.zyh = basyzd.zyh and tdjk.zyh  = '" + RegistryID + "'";
        dst1 = Helper.GetDataSet(strSql1);

        string strSql2 = "SELECT TFY.ZYH,   TFY.FY01,   TFY.FY02,   TFY.FY03,   TFY.FY04,   TFY.FY05,   TFY.FY06,   TFY.FY07,   TFY.FY08,   TFY.FY09,   TFY.FY10,   TFY.FY11,   TFY.FY12,   TFY.FY13,   TFY.FY14,   TFY.FY15,   TFY.FY16,   TFY.FY17,   TFY.FY18,   TFY.FY19,   TFY.FY20,   TFY.FY21,   TFY.FY22,   TFY.FY23,   TFY.FY24,   TFY.HJJE,   BASYZD.SFSJ,   BASYZD.BYSL,   BASYZD.SFSZ,   BASYZD.SZQX,   BASYZD.XX,   BASYZD.RH,   BASYZD.SXFY,   BASYZD.SX_HXB,   BASYZD.SX_XXB,   BASYZD.SX_XJ,   BASYZD.SX_QX,   BASYZD.SX_QT,   BASYZD.SSSM,  BASYZD.ZYH,   BASYZD.SJBL,   BASYZD.SZQXLB,   BASYZD.ERZD1,   BASYZD.ERZD2,   BASYZD.ERZD3,   BASYZD.ERZDBM1,   BASYZD.ERZDBM2,   BASYZD.ERZDBM3,   BASYZD.ERCYQK1,   BASYZD.ERCYQK2,   BASYZD.ERCYQK3,   BASYZD.ERXB,   BASYZD.ERKJM,   BASYZD.ERYGYM,   BASYZD.ERTSH,   BASYZD.ERPKU,   BASYZD.ERTZ,   BASYZD.ERAPGARPF,   BASYZD.CRBBGYB,   BASYZD.SBBGYB,basyzd.ybsm_jy,'' as CZBM1,'' as CZBM2,'' as CZBM3,'' as CZBM4,'' as SSRQ1,'' as SSRQ2,'' as SSRQ3,'' as SSRQ4,'' as SSMC1,'' as SSMC2,'' as SSMC3,'' as SSMC4, '' AS SSYS1, '' AS SSYS2, '' AS SSYS3, '' AS SSYS4, '' AS Z11, '' AS Z12, '' AS Z13, '' AS Z14, '' AS Z21, '' AS Z22, '' AS Z23, '' AS Z24, '' AS MZ1, '' AS MZ2, '' AS MZ3, '' AS MZ4, '' AS QK1, '' AS QK2, '' AS QK3, '' AS QK4, '' AS MZYS1, '' AS MZYS2, '' AS MZYS3, '' AS MZYS4, '' as ERCYQK11, '' as ERCYQK12, '' as ERCYQK13, '' as ERCYQK14, '' as ERCYQK15, '' as ERCYQK21, '' as ERCYQK22, '' as ERCYQK23, '' as ERCYQK24, '' as ERCYQK25, '' as ERCYQK31, '' as ERCYQK32, '' as ERCYQK33, '' as ERCYQK34, '' as ERCYQK35, '' as SZQX_Z, '' as SZQX_Y, '' as SZQX_N FROM TFY,   BASYZD    WHERE ( TFY.ZYH = BASYZD.ZYH ) and   TFY.zyh = '" + RegistryID + "'"
        + " union all " + "SELECT TFYZ.ZYH,   TFYZ.FY01,   TFYZ.FY02,   TFYZ.FY03,   TFYZ.FY04,   TFYZ.FY05,   TFYZ.FY06,   TFYZ.FY07,   TFYZ.FY08,   TFYZ.FY09,   TFYZ.FY10,   TFYZ.FY11,   TFYZ.FY12,   TFYZ.FY13,   TFYZ.FY14,   TFYZ.FY15,   TFYZ.FY16,   TFYZ.FY17,   TFYZ.FY18,   TFYZ.FY19,   TFYZ.FY20,   TFYZ.FY21,   TFYZ.FY22,   TFYZ.FY23,   TFYZ.FY24,   TFYZ.HJJE,   BASYZD.SFSJ,   BASYZD.BYSL,   BASYZD.SFSZ,   BASYZD.SZQX,   BASYZD.XX,   BASYZD.RH,   BASYZD.SXFY,   BASYZD.SX_HXB,   BASYZD.SX_XXB,   BASYZD.SX_XJ,   BASYZD.SX_QX,   BASYZD.SX_QT,   BASYZD.SSSM,  BASYZD.ZYH,   BASYZD.SJBL,   BASYZD.SZQXLB,   BASYZD.ERZD1,   BASYZD.ERZD2,   BASYZD.ERZD3,   BASYZD.ERZDBM1,   BASYZD.ERZDBM2,   BASYZD.ERZDBM3,   BASYZD.ERCYQK1,   BASYZD.ERCYQK2,   BASYZD.ERCYQK3,   BASYZD.ERXB,   BASYZD.ERKJM,   BASYZD.ERYGYM,   BASYZD.ERTSH,   BASYZD.ERPKU,   BASYZD.ERTZ,   BASYZD.ERAPGARPF,   BASYZD.CRBBGYB,   BASYZD.SBBGYB,basyzd.ybsm_jy,'' as CZBM1,'' as CZBM2,'' as CZBM3,'' as CZBM4,'' as SSRQ1,'' as SSRQ2,'' as SSRQ3,'' as SSRQ4,'' as SSMC1,'' as SSMC2,'' as SSMC3,'' as SSMC4, '' AS SSYS1, '' AS SSYS2, '' AS SSYS3, '' AS SSYS4, '' AS Z11, '' AS Z12, '' AS Z13, '' AS Z14, '' AS Z21, '' AS Z22, '' AS Z23, '' AS Z24, '' AS MZ1, '' AS MZ2, '' AS MZ3, '' AS MZ4, '' AS QK1, '' AS QK2, '' AS QK3, '' AS QK4, '' AS MZYS1, '' AS MZYS2, '' AS MZYS3, '' AS MZYS4, '' as ERCYQK11, '' as ERCYQK12, '' as ERCYQK13, '' as ERCYQK14, '' as ERCYQK15, '' as ERCYQK21, '' as ERCYQK22, '' as ERCYQK23, '' as ERCYQK24, '' as ERCYQK25, '' as ERCYQK31, '' as ERCYQK32, '' as ERCYQK33, '' as ERCYQK34, '' as ERCYQK35, '' as SZQX_Z, '' as SZQX_Y, '' as SZQX_N FROM TFYZ,   BASYZD    WHERE ( TFYZ.ZYH = BASYZD.ZYH ) and   TFYZ.zyh  = '" + RegistryID + "'";
        dst2 = Helper.GetDataSet(strSql2);

        string strSql3 = "SELECT SSBM,   SSKSRQ,   SSYS1,   MZYS1,   MZBM,   SSYS2,   SSYS3,   SSDJ,   SSLB,   QKYHDJ,   SSMC  FROM SSMXK  WHERE zyh  = '" + RegistryID + "'";
        dst3 = Helper.GetDataSet(strSql3);

        DataSet dst4;
        string strSql4 = "select count(1) as zycs from (select zyh,bah from tdjkz union all select zyh,bah from tdjk  ) aa where bah in(select bah from(select zyh,bah from tdjkz union all select zyh,bah from tdjk  ) bb where zyh ='" + RegistryID + "')";
        dst4 = Helper.GetDataSet(strSql4);
        if (dst1.Tables[0].Rows.Count > 0)
        {
           DataRow dr = dst1.Tables[0].Rows[0];
           if (dst4.Tables[0].Rows.Count > 0)
           {
               DataRow dr4 = dst4.Tables[0].Rows[0];
               dr["zycs"] = dr4["zycs"].ToString();
           }
           else
           {
               dr["zycs"] = "1";
           }
        }

    }
    [WebMethod(Description = "Returns Medical's DataSet")]
    public DataSet GetMedicalDataEx(string name, string para1)
    {
        SqlHelper Helper = new SqlHelper("HisDB");
        string strSql = "";
        switch(name)
        {
            case "ksbm":
                strSql = "select ksmc from mz_ksbm where ksbm = " + "'" + para1 + "'";
                break;
            case "ysbm":
                strSql = "select ysm from tysm where ysbm = " + "'" + para1 + "'";
                break;
            case "MZDM":
                strSql = "select MZ from MZDM where MZDM = " + "'" + para1 + "'";
                break;  
            case "SJDM":
                strSql = "select ZWMC_1 from GJDM where SJDM = " + "'" + para1 + "'";
                break;
            case "JTGXDM":
                strSql = "select JTGX from JTGXDM where JTGXDM = " + "'" + para1 + "'";
                break;
        }
        DataSet ds = Helper.GetDataSet(strSql);
        return ds;
    }
    [WebMethod]
    public DataTable GetQCTableByMonthStatistics_PersonalEnd(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        SqlHelper Helperlist = new SqlHelper("HisDB");
        //SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND ksty is null ORDER BY ksbm
        string SQLSentence = @"select DISTINCT ysbm,ysm,ksbm,ksmc 
FROM
((select DISTINCT
tysm.ysbm,tysm.ysm,mz_ksbm.ksbm,mz_ksbm.ksmc
from tdjk 
right JOIN tysm ON tdjk.zrys = tysm.ysbm right join mz_ksbm ON tdjk.ksbm = mz_ksbm.ksbm
where emrgdsj between   '" + startTime + "' and '" + endTime + "' and emrstatus = '1' )UNION ALL select DISTINCT tysm.ysbm,tysm.ysm,mz_ksbm.ksbm,mz_ksbm.ksmc from tdjkz right JOIN tysm ON tdjkz.zrys = tysm.ysbm right join mz_ksbm ON tdjkz.ksbm = mz_ksbm.ksbm where emrgdsj between   '" + startTime + "' and '" + endTime + "' and emrstatus = '1' )tjxx order by ksbm";
        DataTable dtlist = Helperlist.GetDataTable(SQLSentence);

        DataTable dt = new DataTable();
        dt.TableName = "Table";
        dt.Columns.Add("科室");
        dt.Columns.Add("主管医师");
        dt.Columns.Add("病历总数");
        dt.Columns.Add("参评病历");
        dt.Columns.Add("平均分");
        dt.Columns.Add("甲级病历");
        dt.Columns.Add("乙级病历");
        dt.Columns.Add("丙级病历");
        dt.Columns.Add("甲级率");

        SqlHelper HelperHis = new SqlHelper("HisDB");//获得病历总数

        SqlHelper Helper = new SqlHelper("EmrDB");

        string strSelectHIS = @"select zyh,zrys from tdjk where emrgdsj between  '" + startTime + "' and '" + endTime + "' and emrstatus = '1' UNION ALL select zyh,ksbm from tdjkz where emrgdsj between  '" + startTime + "' and '" + endTime + "' and emrstatus = '1' ";
        try
        {
            DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            for (int j = 0; j < dtTemp.Rows.Count; j++)
            {
                if (j == 0) strBuilder.Append("'" + dtTemp.Rows[j]["zyh"] + "'");

                else strBuilder.Append("," + "'" + dtTemp.Rows[j]["zyh"] + "'");

            }
            string strSelect = "select * from ValuateScoreEnd where opDate between  '" + startTime + "' and '" + endTime + "'  and RegistryID in (" + strBuilder.ToString() + ")";
            DataTable dtemr = Helper.GetDataTable(strSelect);

            for (int i = 0; i < dtlist.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dtTemp.Rows[0][0]) > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["科室"] = dtlist.Rows[i][3].ToString();


                    dr["主管医师"] = dtlist.Rows[i][1].ToString();

                    DataRow[] dwhis = dtTemp.Select("zrys='" + dtlist.Rows[i][0].ToString() + "'");//病历总数
                    if (dwhis.Length.ToString() == "0") continue;
                    dr["病历总数"] = dwhis.Length.ToString();

                    DataRow[] dw = dtemr.Select("DoctorID='" + dtlist.Rows[i][0].ToString() + "'");//参评病历数
                    dr["参评病历"] = dw.Length.ToString();
                    object o = dtemr.Compute("sum(Score)", "DoctorID='" + dtlist.Rows[i][0].ToString() + "'");
                    if (o.ToString() == "") continue;

                    Decimal avg = Convert.ToDecimal(dtemr.Compute("sum(Score)", "DoctorID='" + dtlist.Rows[i][0].ToString() + "'"));//平均分

                    dr["平均分"] = (avg * 100 / dw.Length).ToString("0.00");

                    DataRow[] dwjia = dtemr.Select("DoctorID='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=0");//甲级病历数
                    dr["甲级病历"] = dwjia.Length.ToString();

                    DataRow[] dwyi = dtemr.Select("DoctorID='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=1");//乙甲级病历数
                    dr["乙级病历"] = dwyi.Length.ToString();

                    DataRow[] dwbing = dtemr.Select("DoctorID='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=2");//丙甲级病历数
                    dr["丙级病历"] = dwbing.Length.ToString();

                    if ((Convert.ToDecimal(dw.Length) == 0) || (Convert.ToDecimal(dwjia.Length) == 0))
                    {
                        dr["甲级率"] = "0.0%";
                    }
                    else
                    {
                        double d = Convert.ToDouble(dwjia.Length) / Convert.ToDouble(dw.Length);
                        dr["甲级率"] = string.Format("{0:#.0%} ", d);

                    }

                    dt.Rows.Add(dr);
                }

            }

        }


        catch (Exception ex)
        {
            dt = null;
        }
        return dt;
    }

    [WebMethod]
    public DataTable GetQCTableByMonthStatisticsEnd(string startTime, string endTime, string strDepartment, string strDoctor)
    {
        SqlHelper Helperlist = new SqlHelper("HisDB");
        string SQLSentence = @"SELECT ksbm, ksmc FROM mz_ksbm WHERE lcbs = '1' AND ksty is null ORDER BY ksbm";
        DataTable dtlist = Helperlist.GetDataTable(SQLSentence);

        DataTable dt = new DataTable();
        dt.TableName = "Table";
        dt.Columns.Add("科室");
        dt.Columns.Add("病历总数");
        dt.Columns.Add("参评病历数");
        dt.Columns.Add("甲级病历数");
        dt.Columns.Add("乙级病历数");
        dt.Columns.Add("丙级病历数");
        dt.Columns.Add("甲级率");

        SqlHelper HelperHis = new SqlHelper("HisDB");//获得病历总数

        SqlHelper Helper = new SqlHelper("EmrDB");

        string strSelectHIS = @"select zyh,ksbm from tdjk where emrgdsj between  '" + startTime + "' and '" + endTime + "' and emrstatus = '1' UNION ALL select zyh,ksbm from tdjkz where emrgdsj between  '" + startTime + "' and '" + endTime + "' and emrstatus = '1' ";
        try
        {
            DataTable dtTemp = HelperHis.GetDataTable(strSelectHIS);

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            for (int j = 0; j < dtTemp.Rows.Count; j++)
            {
                if (j == 0) strBuilder.Append("'" + dtTemp.Rows[j]["zyh"] + "'");

                else strBuilder.Append("," + "'" + dtTemp.Rows[j]["zyh"] + "'");

            }
            string strSelect = "select * from ValuateScoreEnd where opDate between  '" + startTime + "' and '" + endTime + "' and RegistryID in (" + strBuilder.ToString() + ")";
            DataTable dtemr = Helper.GetDataTable(strSelect);

            for (int i = 0; i < dtlist.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dtTemp.Rows[0][0]) > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["科室"] = dtlist.Rows[i][1].ToString();

                    DataRow[] dwhis = dtTemp.Select("ksbm='" + dtlist.Rows[i][0].ToString() + "'");//病历总数
                    if (dwhis.Length.ToString() == "0") continue;
                    dr["病历总数"] = dwhis.Length.ToString();

                    DataRow[] dw = dtemr.Select("DepartmentCode='" + dtlist.Rows[i][0].ToString() + "'");//参评病历数
                    dr["参评病历数"] = dw.Length.ToString();

                    DataRow[] dwjia = dtemr.Select("DepartmentCode='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=0");//甲级病历数
                    dr["甲级病历数"] = dwjia.Length.ToString();

                    DataRow[] dwyi = dtemr.Select("DepartmentCode='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=1");//乙甲级病历数
                    dr["乙级病历数"] = dwyi.Length.ToString();

                    DataRow[] dwbing = dtemr.Select("DepartmentCode='" + dtlist.Rows[i][0].ToString() + "' and ScoreLevel=2");//丙甲级病历数
                    dr["丙级病历数"] = dwbing.Length.ToString();

                    if ((Convert.ToDecimal(dw.Length) == 0) || (Convert.ToDecimal(dwjia.Length) == 0))
                    {
                        dr["甲级率"] = "0.0%";
                    }
                    else
                    {
                        double d = Convert.ToDouble(dwjia.Length) / Convert.ToDouble(dw.Length);
                        dr["甲级率"] = string.Format("{0:#.0%} ", d);

                    }

                    dt.Rows.Add(dr);
                }

            }

        }


        catch (Exception ex)
        {
            dt = null;
        }
        return dt;
    }
    [WebMethod(Description = "Return Query patient list for the discharged")]
    public XmlNode QueryPatientListEx(EmrConstant.QueryMode mode, string criteria, bool inStyle)
    // public XmlNode QueryPatientList( string criteria)
    {
        //EmrConstant.QueryMode mode = QueryMode.Commpose;
        //bool inStyle = true;


        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
        Jobs(ref jobs);

        string SQLSentence = null;
        xmldoc = new XmlDocument();
        DataSet dataset = null;
        XmlElement patients = xmldoc.CreateElement(EmrConstant.ElementNames.Patients);
        XmlElement patient = null;
        try
        {
            #region Make SQL sentence
            if (inStyle)
            {
                switch (mode)
                {
                    #region Archive Number
                    case QueryMode.ArchiveNum:
                        SQLSentence = @"SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz,
                            tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus 
                            FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.bah =  '" + criteria + "' ";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0)
                        {

                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw,tdjkz.emrstatus 
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah =  '" + criteria + "' ";


                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {

                                OneRegistryExEx(row, inStyle, ref patient);

                            }
                            patients.AppendChild(patient);
                        }
                        else
                        {

                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistry(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);

                            SQLSentence = @"SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz,
                            tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw,tdjkz.emrstatus
                            FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm 
                            LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.bah like  '%" + criteria + "'";
                            dataset = ExeuSentence(SQLSentence, hisDBType);
                            if (dataset.Tables.Count == 0) break;
                            if (dataset.Tables[0].Rows.Count == 0) break;
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row in dataset.Tables[0].Rows)
                            {
                                OneRegistryExEx(row, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }

                        break;
                    #endregion


                    #region Patient name
                    case QueryMode.PatientName:
                        string[] item = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string sqlCon = "";
                        string sqlCons = "";
                        string criter = criteria;
                        if (item.Length > 2)
                        {
                            sqlCon = " and tdjkz.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                            sqlCons = " and tdjk.zyrq BETWEEN '" + item[0] + "' AND '" + item[1] + "'";
                            criter = item[2];

                        }
                        string strSelect = "SELECT  DISTINCT  tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                                 "tdjkz.zyh,tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw,tdjkz.emrstatus " +
                                 "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                                 "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.xmpy = '" + criter + "'";
                        strSelect += sqlCon + " order by tdjkz.zyrq desc";
                        DataSet ds = ExeuSentence(strSelect, hisDBType);

                        if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistryExEx(dr, inStyle, ref patient);
                                patients.AppendChild(patient);


                            }
                        }
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                               "tdjk.zyh,tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                               "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                               "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.xmpy = '" + criter + "'";
                        SQLSentence += sqlCons + " order by tdjk.zyrq desc";
                        ds = ExeuSentence(SQLSentence, hisDBType);
                        if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dr, inStyle, ref patient, jobs);

                                OneRegistry(dr, inStyle, ref patient);
                                patients.AppendChild(patient);


                            }
                        }
                        break;
                    #endregion

                    #region RegistryID
                    case QueryMode.RegistryID:

                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE tdjk.zyh like  '%" + criteria + "'";


                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistry(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }

                        SQLSentence = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
                           "tdjkz.zyh, tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw,tdjkz.emrstatus " +
                           "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
                           "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE tdjkz.zyh like  '%" + criteria + "'";

                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 1)
                        {
                            if (dataset.Tables[0].Rows.Count == 1)
                            {
                                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                                OnePatient(dataset.Tables[0].Rows[0], inStyle, ref patient, jobs);
                                OneRegistryExEx(dataset.Tables[0].Rows[0], inStyle, ref patient);
                                patients.AppendChild(patient);
                            }
                        }
                        break;
                    #endregion

                    #region Commpose
                    case QueryMode.Commpose:
                        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                        string whereSentence = null;
                        SQLSentence = "SELECT DISTINCT bah, xb, xm FROM tdjk WHERE ";
                        if (items[0].Length > 0)
                            whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
                        if (items[1] != EmrConstant.StringGeneral.Both)
                        {
                            whereSentence += "tdjk.xb='" + items[1] + "' AND ";
                        }
                        whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                        if (items[4].Length > 0) whereSentence += " AND tdjk.zrys='" + items[4] + "'";
                        if (items[5].Length > 0) whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
                        if (items.Length >= 7)
                        {
                            switch (items[6])
                            {
                                case "1":
                                    whereSentence += " AND  tdjk.emrstatus= 1";
                                    break;
                                case "2":
                                    whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                                    break;
                                case "3":
                                    break;
                            }
                        }
                        whereSentence += " ORDER BY tdjk.xb, tdjk.xm";
                        SQLSentence += whereSentence;
                        dataset = ExeuSentence(SQLSentence, hisDBType);
                        if (dataset.Tables.Count == 0) break;
                        if (dataset.Tables[0].Rows.Count == 0) break;
                        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
                            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
                            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
                            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
                        foreach (DataRow row in dataset.Tables[0].Rows)
                        {
                            string archiveNum = row.ItemArray[0].ToString();
                            string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "'";
                            DataSet dataset1 = ExeuSentence(sentence, hisDBType);
                            patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                            OnePatient(dataset1.Tables[0].Rows[0], inStyle, ref patient, jobs);
                            foreach (DataRow row1 in dataset1.Tables[0].Rows)
                            {
                                OneRegistry(row1, inStyle, ref patient);
                            }
                            patients.AppendChild(patient);
                        }



                        break;
                    #endregion
                }
            }
            else
            {
                #region OutPatient
                switch (mode)
                {
                    case QueryMode.ArchiveNum:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE ylzh = '" + criteria + "'";
                        break;
                    case QueryMode.PatientName:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE hzxm like '" + criteria + "%'";
                        break;
                    case QueryMode.RegistryID:
                        SQLSentence = "SELECT ylzh,hzxm,hzxb,hznl,'不详','不详','不详','不详','不详'," +
                            "mzh,'####',ghrq,ysbm,ksbm " +
                            "FROM  mz_ghmx WHERE mzh = '" + criteria + "'";
                        break;
                }
                #endregion
            }
            #endregion
            /* Execute sentence */
            return patients;

        }
        catch (Exception ex)
        {
            return DataError(ex);
        }
    }
    private void OneRegistryExEx(DataRow row, bool inStyle, ref XmlElement patient)
    {
        XmlElement registry = patient.OwnerDocument.CreateElement(EmrConstant.ElementNames.Registry);
        registry.SetAttribute(EmrConstant.AttributeNames.RegistryID, row.ItemArray[9].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.BedNum, row.ItemArray[10].ToString());

        string[] items = row.ItemArray[11].ToString().Split(' ');
        if (items.Length == 2)
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, items[0]);
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, items[1]);

        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryDate, "");
            registry.SetAttribute(EmrConstant.AttributeNames.RegistryTime, "");

        }
        registry.SetAttribute(EmrConstant.AttributeNames.DoctorID, row.ItemArray[12].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.DepartmentCode, row.ItemArray[13].ToString());
        registry.SetAttribute(EmrConstant.AttributeNames.DischargedDate, row.ItemArray[14].ToString());
        if (row.ItemArray[14].ToString() != "")
        {
            if (row.ItemArray[17].ToString() == "1")
                registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, "出科:已归档");
            else
                registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, "出科");
        }
        else
        {
            registry.SetAttribute(EmrConstant.AttributeNames.PatientStatus, "");
        }
        patient.AppendChild(registry);

    }
    [WebMethod(Description = "Returns archive Patient list")]
    public DataSet GetPatientListByEmrstatusEx(EmrConstant.QueryMode mode, string criteria, bool archive)
    {
        string SQLSentence = null;
        string SQLSentenceIn = null;
        DataSet ds = new DataSet();

        switch (mode)
        {
            #region Commpose
            case QueryMode.Commpose:
                string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
                string whereSentence = null;
                string whereSentenceIn = null;
                SQLSentence = "SELECT zyh, bah, xb, xm, mz_ksbm.ksmc as ksmc, zrys,zyrq, cyrq,emrgdsj, case emrstatus when '1' then '已归档' else '未归档' end as emrstatus FROM tdjk,mz_ksbm where tdjk.ksbm = mz_ksbm.ksbm and ";
                SQLSentenceIn = "SELECT zyh, bah, xb, xm, mz_ksbm.ksmc as ksmc, zrys,zyrq, cyrq,emrgdsj, case emrstatus when '1' then '已归档' else '未归档' end as emrstatus FROM tdjkz,mz_ksbm where tdjkz.ksbm = mz_ksbm.ksbm and ";
                if (items[0].Length > 0)
                {
                    whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
                    whereSentenceIn = "tdjkz.xmpy='" + items[0] + "' AND ";
                }
                if (items[1] != EmrConstant.StringGeneral.Both)
                {
                    whereSentence += "tdjk.xb='" + items[1] + "' AND ";
                    whereSentenceIn += "tdjkz.xb='" + items[1] + "' AND ";
                }
                if (archive)
                {
                    whereSentence += "tdjk.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                    whereSentenceIn += "tdjkz.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                }
                else
                {
                    whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                    whereSentenceIn += "tdjkz.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                }
                if (items[4].Length > 0)
                {
                    whereSentence += " AND tdjk.zrys='" + items[4] + "'";
                    whereSentenceIn += " AND tdjkz.zrys='" + items[4] + "'";
                }
                if (items[5].Length > 0)
                {
                    whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
                    whereSentenceIn += " AND tdjkz.ksbm='" + items[5] + "'";
                }
                if (items.Length >= 7)
                {
                    switch (items[6])
                    {
                        case "1":
                            whereSentence += " AND tdjk.emrstatus= 1";
                            whereSentenceIn += " AND tdjkz.emrstatus= 1";
                            break;
                        case "2":
                            whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                            whereSentence += " AND zyrq > '2011-03-01 00:00:00'";
                            whereSentenceIn += " AND (tdjkz.emrstatus is null or tdjkz.emrstatus <>1)";
                            whereSentenceIn += " AND zyrq > '2011-03-01 00:00:00'";
                            break;
                        case "3":
                            break;
                    }
                    //if (items[6].Length > 0) whereSentence += " AND tdjk.emrstatus=" + items[6];
                }
                //whereSentence += " AND zyrq > '2011-03-01 00:00:00'";
                //whereSentenceIn += " AND zyrq > '2011-03-01 00:00:00'";  
                //whereSentence += " ORDER BY tdjk.ksbm";

                SQLSentence = "SELECT zyh, bah, xb, xm, ksmc, zrys, zyrq, cyrq, emrgdsj, emrstatus FROM(" + SQLSentence + whereSentence + "UNION ALL " + SQLSentenceIn + whereSentenceIn + ")tjxx order by ksmc";
                ds = ExeuSentence(SQLSentence, hisDBType);
                break;
            #endregion



        }
        return ds;
    }
    [WebMethod(Description = "is discharged", EnableSession = false)]
    public Boolean IsDischargedEx(XmlNode inRegistryIDs, DateTime endDate, ref XmlNode outRegistryIDs)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
            doc.AppendChild(root);
            foreach (XmlNode inRegistryID in inRegistryIDs)
            {
                string SQLSentence = "SELECT COUNT(*) FROM tdjk WHERE zyh = '" + inRegistryID.InnerText + "' AND cyrq <= '" + endDate.ToString() + "'";
                string SQLSentenceIn = "SELECT COUNT(*) FROM tdjkz WHERE zyh = '" + inRegistryID.InnerText + "' AND cyrq <= '" + endDate.ToString() + "'";
                DataSet dataset = ExeuSentence(SQLSentence, hisDBType);


                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row.ItemArray[0]) == 0)
                    {
                        dataset = ExeuSentence(SQLSentenceIn, hisDBType);
                    }

                }


                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row.ItemArray[0]) == 1)
                    {
                        XmlNode outRegistryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                        outRegistryID.InnerText = inRegistryID.InnerText;
                        root.AppendChild(outRegistryID);
                    }

                }
            }
            outRegistryIDs = doc.Clone();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            outRegistryIDs = DataError(ex);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod]
    public DataSet GetDoctorLevel(string opcode)
    {
        DataSet myDataSet = new DataSet();
        try
        {
            //string SQLSentence = "SELECT ysbm,ysm,ksbm FROM tysm WHERE lb = '1' ORDER BY ysm";
            string SQLSentence = "";

            SQLSentence = @"select * from operator where code = '" + opcode + "' ";


            SqlHelper Helper = new SqlHelper("EmrDB");
            myDataSet = Helper.GetDataSet(SQLSentence);

        } //try end
        catch (Exception ex)
        {
            myDataSet = null;
        }
        return myDataSet;
    }
    [WebMethod(Description = "Return Query patient list for the discharged")]
    public XmlNode CommposePatientListEx(string criteria, bool archive)
    {
        DataTable jobs = new DataTable();
        jobs.TableName = "jobs";
        Jobs(ref jobs);

        string SQLSentence = null;
        xmldoc = new XmlDocument();
        DataSet dataset = null;
        XmlElement patients = xmldoc.CreateElement(EmrConstant.ElementNames.Patients);
        XmlElement patient = null;
        #region Commpose

        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
        string whereSentence = null;
        string whereSentenceIn = null;
        string SQLSentenceIn = null;
        SQLSentence = "SELECT DISTINCT bah, xb, xm FROM tdjk WHERE ";
        SQLSentenceIn = "SELECT DISTINCT bah, xb, xm FROM tdjkz WHERE ";
        if (items[0].Length > 0)
        {
            whereSentence = "tdjk.xmpy='" + items[0] + "' AND ";
            whereSentenceIn = "tdjkz.xmpy='" + items[0] + "' AND ";
        }
        if (items[1] != EmrConstant.StringGeneral.Both)
        {
            whereSentence += "tdjk.xb='" + items[1] + "' AND ";
            whereSentenceIn += "tdjkz.xb='" + items[1] + "' AND ";
        }
        if (archive)
        {
            if (items.Length >= 7 && items[6] != "2")
            {
                whereSentence += "tdjk.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                whereSentenceIn += "tdjkz.emrgdsj BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
            }
            if (items.Length >= 7 && items[6] == "2")
            {
                whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                whereSentence += " AND zyrq > '2011-03-01 00:00:00'";
                whereSentenceIn += "tdjkz.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
                whereSentenceIn += " AND zyrq > '2011-03-01 00:00:00'";
            }
        }
        else
        {
            whereSentence += "tdjk.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
            whereSentenceIn += "tdjkz.cyrq BETWEEN '" + items[2] + "' AND '" + items[3] + "'";
        }
        if (items[4].Length > 0)
        {
            whereSentence += " AND tdjk.zrys='" + items[4] + "'";
            whereSentenceIn += " AND tdjkz.zrys='" + items[4] + "'";
        }
        if (items[5].Length > 0)
        {
            whereSentence += " AND tdjk.ksbm='" + items[5] + "'";
            whereSentenceIn += " AND tdjkz.ksbm='" + items[5] + "'";
        }
        if (items.Length >= 7)
        {
            switch (items[6])
            {
                case "1":
                    whereSentence += " AND  tdjk.emrstatus= 1";
                    whereSentenceIn += " AND  tdjkz.emrstatus= 1";
                    break;
                case "2":
                    whereSentence += " AND (tdjk.emrstatus is null or tdjk.emrstatus <>1)";
                    whereSentenceIn += " AND (tdjkz.emrstatus is null or tdjkz.emrstatus <>1)";
                    break;
                case "3":
                    break;
            }
        }

        whereSentence = "select DISTINCT bah, xb, xm FROM (" + SQLSentence + whereSentence + "UNION ALL " + SQLSentenceIn + whereSentenceIn + ")tjxx order by xb,xm";
        SQLSentence = whereSentence;
        dataset = ExeuSentence(SQLSentence, hisDBType);
        if (dataset.Tables.Count == 0) return patients;
        if (dataset.Tables[0].Rows.Count == 0) return patients;
        SQLSentence = "SELECT tdjk.bah,tdjk.xm,tdjk.xb,tdjk.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjk.zy,tdjk.jtzz," +
            "tdjk.zyh, tdjk.ch,tdjk.zyrq,tdjk.zrys,tdjk.ksbm,tdjk.cyrq,tdjk.nl,tdjk.nldw,tdjk.emrstatus " +
            "FROM  tdjk LEFT JOIN mzdm ON tdjk.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjk.hf=hfbm.hfbm " +
            "LEFT JOIN jgbm ON tdjk.jg=jgbm.jgbm WHERE ";
        SQLSentenceIn = "SELECT tdjkz.bah,tdjkz.xm,tdjkz.xb,tdjkz.csny,mzdm.mz,hfbm.hf,jgbm.jg,tdjkz.zy,tdjkz.jtzz," +
            "tdjkz.zyh, tdjkz.ch,tdjkz.zyrq,tdjkz.zrys,tdjkz.ksbm,tdjkz.cyrq,tdjkz.nl,tdjkz.nldw,tdjkz.emrstatus " +
            "FROM  tdjkz LEFT JOIN mzdm ON tdjkz.mz=mzdm.mzdm LEFT JOIN hfbm ON tdjkz.hf=hfbm.hfbm " +
            "LEFT JOIN jgbm ON tdjkz.jg=jgbm.jgbm WHERE ";
        foreach (DataRow row in dataset.Tables[0].Rows)
        {
            string archiveNum = row.ItemArray[0].ToString();
            string sentence = SQLSentence + "tdjk.bah='" + archiveNum + "'";
            string sentenceIn = SQLSentenceIn + "tdjkz.bah='" + archiveNum + "'";
            DataSet dataset1 = ExeuSentence(sentence, hisDBType);
            if (dataset1.Tables[0].Rows.Count == 0)
            {
                dataset1 = ExeuSentence(sentenceIn, hisDBType);
                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                OnePatient(dataset1.Tables[0].Rows[0], true, ref patient, jobs);
                foreach (DataRow row1 in dataset1.Tables[0].Rows)
                {
                    OneRegistryExEx(row1, true, ref patient);
                }
            }
            else
            {
                patient = xmldoc.CreateElement(EmrConstant.ElementNames.Patient);
                OnePatient(dataset1.Tables[0].Rows[0], true, ref patient, jobs);
                foreach (DataRow row1 in dataset1.Tables[0].Rows)
                {
                    OneRegistry(row1, true, ref patient);
                }
            }
            patients.AppendChild(patient);
        }



        return patients;
        #endregion
    }

    [WebMethod(Description = " ", EnableSession = false)]
    public XmlNode ValuateNowEx()
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlNode testsAndExams = xmldoc.CreateElement(EmrConstant.ElementNames.TestsAndExams);

        DataSet myDataSet = new DataSet();
        string strSql = "SELECT [XMMC]" +
        ",[SZFHJ]" +
        ",[SZF]" +
        ",[MC]" +
        ",[GLMC]" +
        "  FROM [BAZLBZ]";
        SqlHelper Helper = new SqlHelper("hisDBBA");
        myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count != 0)
        {
            foreach (DataRow row in myDataSet.Tables[0].Rows)
            {
                XmlElement archive = xmldoc.CreateElement(EmrConstant.ElementNames.archive);
                archive.SetAttribute(EmrConstant.AttributeNames.ProName, row.ItemArray[0].ToString());
                archive.SetAttribute(EmrConstant.AttributeNames.SZF, row.ItemArray[2].ToString());
                archive.SetAttribute(EmrConstant.AttributeNames.SZFHJ, row.ItemArray[1].ToString());
                archive.SetAttribute(EmrConstant.AttributeNames.Content, row.ItemArray[3].ToString());
                archive.SetAttribute(EmrConstant.AttributeNames.GLMC, row.ItemArray[4].ToString());
                string strSql2 = "SELECT " +
                                 "NAME" +
                                 ",CKKF" +
                                 "  FROM [BAZLKFYYK]  WHERE BH='" + row.ItemArray[4].ToString() + "'";

                DataSet dataset2 = new DataSet();
                SqlConnection sconn2 = new SqlConnection(hisDBBA);
                sconn2.Open();
                SqlDataAdapter scommand2 = new SqlDataAdapter(strSql2, sconn2);
                scommand2.Fill(dataset2, "Results2");
                sconn2.Close();
                if (dataset2.Tables.Count > 0)
                {
                    foreach (DataRow row1 in dataset2.Tables[0].Rows)
                    {
                        XmlElement item = xmldoc.CreateElement(EmrConstant.ElementNames.Item);
                        //item.SetAttribute(EmrConstant.AttributeNames.Type, EmrConstant.StringGeneral.None);
                        item.SetAttribute(EmrConstant.AttributeNames.KFYY, row1.ItemArray[0].ToString());
                        item.SetAttribute(EmrConstant.AttributeNames.CKKF, row1.ItemArray[1].ToString());
                        //item.SetAttribute(EmrConstant.AttributeNames.XH, row1.ItemArray[0].ToString());

                        archive.AppendChild(item);
                    }

                }
                testsAndExams.AppendChild(archive);
            }
        }
        return testsAndExams;
    }

    [WebMethod(Description = " 现岗", EnableSession = false)]
    public string ValuateNowISSH(string zyh)
    {
        string strSql = "select shbz  from bazlgl_kj where zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string count = (string)Helper.ExecuteScalar(strSql);

        return count;

    }
    [WebMethod(Description = " 终末", EnableSession = false)]
    public string ValuateEndIsCheck(string zyh)
    {
        string strSql = "select shbz  from bazlgl_End where zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string count = (string)Helper.ExecuteScalar(strSql);

        return count;

    }
    [WebMethod(Description = " 现岗", EnableSession = false)]
    public DataTable ValuateNowBAPSBZ(string zyh)
    {
        DataTable dt = null;
        string strSql = "select  kj.bapsbz,mx.kfyy,mx.pfbz, kj.pj, kj.sdf from bazlgl_kj as kj left join bazlgl_kfmx as mx ON kj.psbah=mx.psbah where kj.zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;

    }
    [WebMethod(Description = " 现岗", EnableSession = false)]
    public DataTable ValuateNowBAPSBZNew(string zyh)
    {
        DataTable dt = null;
        string strSql = "select  kj.bapsbz,mx.kfyy,mx.pfbz, kj.pj, kj.sdf,mx.kf  from bazlgl_kj as kj left join bazlgl_kfmx as mx ON kj.psbah=mx.psbah where kj.zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;

    }
    [WebMethod(Description = "终末 ", EnableSession = false)]
    public DataTable ValuateEndCheckInfo(string zyh)
    {
        DataTable dt = null;
        string strSql = "select  kj.bapsbz,mx.kfyy,mx.pfbz, kj.pj, kj.sdf  from bazlgl_End as kj left join bazlgl_End_kfmx as mx ON kj.psbah=mx.psbah where kj.zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;

    }
    [WebMethod(Description = " ", EnableSession = false)]
    public bool ValuateNowSH(string zyh, string shry, string shrq, XmlNode xmg)
    {
        //bool flag = false;
        //string strUpdate = "UPDATE [bazlgl_kj] SET [shbz]=1" + ",[shry]='" + shry + "',[shrq]='" + shrq + "' where zyh='" + zyh + "'";

        //SqlHelper Helper = new SqlHelper("EmrDB");
        //int count = Helper.ExecuteNonQuery(strUpdate);

        //if (count == 1)
        //{

        //    if (ValuateNowPSHis(xmg, shry, shrq))
        //        flag = true;
        //}
        //return flag;
        bool flag = false;
        //SqlHelper Helper = new SqlHelper("EmrDB");
        SqlConnection con = new SqlConnection(EmrDB);//获取数据库连接
        con.Open();//打开连接
        SqlTransaction sqltra = con.BeginTransaction();//开始事务
        SqlCommand cmd = new SqlCommand();//实例化
        cmd.Connection = con;//获取数据连接
        cmd.Transaction = sqltra;//，在执行SQL时，
        try
        {
            string strUpdate = "UPDATE [bazlgl_kj] SET [shbz]=1" + ",[shry]='" + shry + "',[shrq]='" + shrq + "' where zyh='" + zyh + "'";

            cmd.CommandText = strUpdate;
            int count = cmd.ExecuteNonQuery();
            //int count = Helper.ExecuteNonQuery(strUpdate);

            if (count == 1)
            {

                if (ValuateNowPSHis(xmg, shry, shrq))
                    flag = true;
            }
            if (flag)
                sqltra.Commit();
            else sqltra.Rollback();
        }
        catch (Exception ex)
        {
            sqltra.Rollback();
        }
        return flag;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public bool ValuateEndSH(string zyh, string shry, string shrq, XmlNode xmg)
    {
        bool flag = false;
        string strUpdate = "UPDATE [bazlgl_End] SET [shbz]=1" + ",[shry]='" + shry + "',[shrq]='" + shrq + "' where zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strUpdate);

        if (count == 1)
        {
            flag = true;
        }
        return flag;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public bool ValuateEndCheck(string zyh, string shry, string shrq, XmlNode xmg)
    {
        bool flag = false;
        string strUpdate = "UPDATE [bazlgl_End] SET [shbz]=1" + ",[shry]='" + shry + "',[shrq]='" + shrq + "' where zyh='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strUpdate);

        if (count == 1)
        {

            if (ValuateNowPSHis(xmg, shry, shrq))
                flag = true;
        }
        return flag;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public bool ValuateNowPSHis(XmlNode xmg, string shry, string shrq)
    {

        bool flag = false;
        //string registryID = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
        string psbah = xmg.Attributes[EmrConstant.AttributeNames.ArchiveNum].Value.ToString();
        string pscyrq = xmg.Attributes[EmrConstant.AttributeNames.PSCYRQ].Value.ToString();
        string psrq = xmg.Attributes[EmrConstant.AttributeNames.PSRQ].Value.ToString();
        string pszjys = xmg.Attributes[EmrConstant.AttributeNames.PSZJYS].Value.ToString();
        string pj = xmg.Attributes[EmrConstant.AttributeNames.PJ].Value.ToString();
        string bapsbz = xmg.Attributes[EmrConstant.AttributeNames.BAPSBZ].Value.ToString();
        //string departID = xmg.Attributes[EmrConstant.AttributeNames.DepartmentCode].Value.ToString();
        string gkf = xmg.Attributes[EmrConstant.AttributeNames.GKF].Value.ToString();
        string sdf = xmg.Attributes[EmrConstant.AttributeNames.SDF].Value.ToString();
        string zyh = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
        string strSql = "INSERT INTO [BAZLGL_KJ]" +
      "(zyh,[PSBAH]" +
       ",[PSCYRQ]" +
      ",[PSRQ]" +
      ",[PJ]" +
      ",[PSZJYS]" +
      ",[GKF]" +
      ",[SDF]" +
      ",[BAPSBZ]" +
      ",[shbz]" +
      ",[shry]" +
      ",[shrq])" +
"VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + psrq + "','" + pj + "','" + pszjys + "'," + gkf + "," + sdf + ",'"
+ bapsbz + "','1','" + shry + "','" + shrq + "');";
        SqlHelper Helper = new SqlHelper("hisDBBA");
        int count = Helper.ExecuteNonQuery(strSql);
        //  string kf=xmg.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
        foreach (XmlNode Flaw in xmg.ChildNodes)
        {
            //string qxmc=Flaw.Attributes[EmrConstant.AttributeNames.QXMC].Value.ToString();
            //string qxmcxh=Flaw.Attributes[EmrConstant.AttributeNames.QXMCXH].Value.ToString();
            string kf = Flaw.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
            string kfxm = Flaw.Attributes[EmrConstant.AttributeNames.KFXM].Value.ToString();
            string kfyy = Flaw.Attributes[EmrConstant.AttributeNames.KFYY].Value.ToString();
            string pfbz = Flaw.Attributes[EmrConstant.AttributeNames.GLMC].Value.ToString();

            string strSql2 = "  INSERT INTO [BAZLGL_KFMX]" +
            "(zyh,[PSBAH]" +
             ",[PSCYRQ]" +
            ",[lb]" +
            ",[kfxm]" +
            ",[kfyy]" +
            ",[kf] ,pfbz)" +

      "VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + 0 + "','" + kfxm + "','" + kfyy + "','" + kf + "','" + pfbz + "')";
            SqlHelper Helper2 = new SqlHelper("hisDBBA");
            int count2 = Helper2.ExecuteNonQuery(strSql2);

            if (count2 == 1)
                flag = true;

        }
        if (xmg.ChildNodes.Count == 0 && count != 0)
            flag = true;
        return flag;
    }
    [WebMethod(Description = " 现岗", EnableSession = false)]
    public bool ValuateNowPS(XmlNode xmg)
    {

        bool flag = false;
        //string registryID = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
        string psbah = xmg.Attributes[EmrConstant.AttributeNames.ArchiveNum].Value.ToString();
        string pscyrq = xmg.Attributes[EmrConstant.AttributeNames.PSCYRQ].Value.ToString();
        string psrq = xmg.Attributes[EmrConstant.AttributeNames.PSRQ].Value.ToString();
        string pszjys = xmg.Attributes[EmrConstant.AttributeNames.PSZJYS].Value.ToString();
        string pj = xmg.Attributes[EmrConstant.AttributeNames.PJ].Value.ToString();
        string bapsbz = xmg.Attributes[EmrConstant.AttributeNames.BAPSBZ].Value.ToString();
        //string departID = xmg.Attributes[EmrConstant.AttributeNames.DepartmentCode].Value.ToString();
        string gkf = xmg.Attributes[EmrConstant.AttributeNames.GKF].Value.ToString();
        string zyh = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();

        string sdf = xmg.Attributes[EmrConstant.AttributeNames.SDF].Value.ToString();
        string strSql = "INSERT INTO [BAZLGL_KJ]" +
      "(zyh,[PSBAH]" +
       ",[PSCYRQ]" +
      ",[PSRQ]" +
      ",[PJ]" +
      ",[PSZJYS]" +
      ",[GKF]" +
      ",[SDF]" +
      ",[BAPSBZ]" +
      ",[shbz]" +
      ",[shry]" +
      ",[shrq])" +
"VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + psrq + "','" + pj + "','" + pszjys + "'," + gkf + "," + sdf + ",'"
+ bapsbz + "','','','');";
        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strSql);
        //  string kf=xmg.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
        foreach (XmlNode Flaw in xmg.ChildNodes)
        {
            //string qxmc=Flaw.Attributes[EmrConstant.AttributeNames.QXMC].Value.ToString();
            //string qxmcxh=Flaw.Attributes[EmrConstant.AttributeNames.QXMCXH].Value.ToString();
            string kf = Flaw.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
            string kfxm = Flaw.Attributes[EmrConstant.AttributeNames.KFXM].Value.ToString();
            string kfyy = Flaw.Attributes[EmrConstant.AttributeNames.KFYY].Value.ToString();
            string pfbz = Flaw.Attributes[EmrConstant.AttributeNames.GLMC].Value.ToString();

            string strSql2 = "  INSERT INTO [BAZLGL_KFMX]" +
            "(zyh,[PSBAH]" +
             ",[PSCYRQ]" +
            ",[lb]" +
            ",[kfxm]" +
            ",[kfyy]" +
            ",[kf] ,pfbz)" +

      "VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + 0 + "','" + kfxm + "','" + kfyy + "','" + kf + "','" + pfbz + "')";
            SqlHelper Helper2 = new SqlHelper("EmrDB");
            int count2 = Helper2.ExecuteNonQuery(strSql2);

            if (count2 == 1)
                flag = true;
        }
        if (xmg.ChildNodes.Count == 0 && count != 0)
            flag = true;
        return flag;
    }
    [WebMethod(Description = " 现岗", EnableSession = false)]
    public bool ValuateEndPs(XmlNode xmg)
    {

        bool flag = false;
        //string registryID = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
        string psbah = xmg.Attributes[EmrConstant.AttributeNames.ArchiveNum].Value.ToString();
        string pscyrq = xmg.Attributes[EmrConstant.AttributeNames.PSCYRQ].Value.ToString();
        string psrq = xmg.Attributes[EmrConstant.AttributeNames.PSRQ].Value.ToString();
        string pszjys = xmg.Attributes[EmrConstant.AttributeNames.PSZJYS].Value.ToString();
        string pj = xmg.Attributes[EmrConstant.AttributeNames.PJ].Value.ToString();
        string bapsbz = xmg.Attributes[EmrConstant.AttributeNames.BAPSBZ].Value.ToString();
        //string departID = xmg.Attributes[EmrConstant.AttributeNames.DepartmentCode].Value.ToString();
        string gkf = xmg.Attributes[EmrConstant.AttributeNames.GKF].Value.ToString();
        string zyh = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();

        string sdf = xmg.Attributes[EmrConstant.AttributeNames.SDF].Value.ToString();
        string strSql = "INSERT INTO [BAZLGL_End]" +
      "(zyh,[PSBAH]" +
       ",[PSCYRQ]" +
      ",[PSRQ]" +
      ",[PJ]" +
      ",[PSZJYS]" +
      ",[GKF]" +
      ",[SDF]" +
      ",[BAPSBZ]" +
      ",[shbz]" +
      ",[shry]" +
      ",[shrq])" +
"VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + psrq + "','" + pj + "','" + pszjys + "'," + gkf + "," + sdf + ",'"
+ bapsbz + "','','','');";
        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strSql);
        //  string kf=xmg.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
        foreach (XmlNode Flaw in xmg.ChildNodes)
        {
            //string qxmc=Flaw.Attributes[EmrConstant.AttributeNames.QXMC].Value.ToString();
            //string qxmcxh=Flaw.Attributes[EmrConstant.AttributeNames.QXMCXH].Value.ToString();
            string kf = Flaw.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
            string kfxm = Flaw.Attributes[EmrConstant.AttributeNames.KFXM].Value.ToString();
            string kfyy = Flaw.Attributes[EmrConstant.AttributeNames.KFYY].Value.ToString();
            string pfbz = Flaw.Attributes[EmrConstant.AttributeNames.GLMC].Value.ToString();

            string strSql2 = "  INSERT INTO [BAZLGL_End_KFMX]" +
            "(zyh,[PSBAH]" +
             ",[PSCYRQ]" +
            ",[lb]" +
            ",[kfxm]" +
            ",[kfyy]" +
            ",[kf] ,pfbz)" +

      "VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + 0 + "','" + kfxm + "','" + kfyy + "','" + kf + "','" + pfbz + "')";
            SqlHelper Helper2 = new SqlHelper("EmrDB");
            int count2 = Helper2.ExecuteNonQuery(strSql2);

            if (count2 == 1)
                flag = true;
        }
        if (xmg.ChildNodes.Count == 0 && count != 0)
            flag = true;
        return flag;
    }
    [WebMethod(Description = " 现岗查询", EnableSession = false)]
    public DataTable ValuateNowPSSelectKs(string item1, string item2, string departID)
    {
        //DataSet ds = null;
        DataTable dt = null;
        DataTable newdt = new DataTable();
        DataTable dtTemp = null;
        string strSql = "select '' as '住院日期','' as '科室',pszjys as '质检医师',BAZLGL_kj.psbah as 病案号,BAZLGL_kj.zyh as 住院号,BAZLGL_kj.GKF as 共扣分,sdf as 总得分,pj as 评审结果,mx.kfxm as 扣分项目, mx.kfyy as 扣分原因 "

                        + " from  BAZLGL_kj  inner join BAZLGL_kfmx as mx ON  BAZLGL_kj.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjkz.ksbm= '" + departID + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjk.ksbm= '" + departID + "'";
                dtTemp = HelperHis.GetDataTable(strSelectHIS);
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                    dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    //dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                }

                i++;

            }

            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);  
            //}           
        }
        //if (dtTemp == null || dtTemp.Rows.Count == 0) return dtTemp;
        //else
            return newdt;
    }
    [WebMethod(Description = "终末查询", EnableSession = false)]
    public DataTable ValuateEndPSSelectKs(string item1, string item2, string departID)
    {
        //DataSet ds = null;
        DataTable dt = new DataTable();    
        DataTable newdt = new DataTable();
        DataTable dtTemp = new DataTable();
        string strSql = "select '' as '住院日期','' as '科室',pszjys as '质检医师',BAZLGL_End.psbah as 病案号,BAZLGL_End.zyh as 住院号,BAZLGL_End.GKF as 共扣分,sdf as 总得分,pj as 评审结果,mx.kfxm as 扣分项目, mx.kfyy as 扣分原因 "

                        + " from  BAZLGL_End  inner join BAZLGL_End_kfmx as mx ON  BAZLGL_End.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjkz.ksbm= '" + departID + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjk.ksbm= '" + departID + "'";
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                    dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    //dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                }

                i++;

            }

            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);
            //}

        }
        //if (dtTemp == null || dtTemp.Rows.Count == 0) return dtTemp;
        //else
            return dt;
    }
    [WebMethod(Description = "New 终末查询", EnableSession = false)]
    public DataTable ValuateEndPSSelectEx(string startTime, string endTime, string departID,string doctor,string level,string title)
    {
        //DataSet ds = null;
        DataTable dt = new DataTable();
        DataTable newdt = new DataTable();
        DataTable dtTemp = new DataTable();
        string strSql = "select pj as '评审结果','' as '科室','' as '患者姓名',BAZLGL_End.zyh as 住院号,'' as '" + title + "',sdf as 总得分 "

                        + " from  BAZLGL_End  inner join BAZLGL_End_kfmx as mx ON  BAZLGL_End.zyh=mx.zyh  where psrq between '" + startTime + "' and '" + endTime + "'";


        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',tdjkz.zyysbm as '主管医师',tdjkz.zzysbm as '主治医师',tdjkz.zrys as '主任医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjkz.ksbm= '" + departID + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',tdjk.zyysbm as '主管医师',tdjk.zzysbm as '主治医师',tdjk.zrys as '主任医师'   from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'and tdjk.ksbm= '" + departID + "'";
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                    dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    if (level.EndsWith("主管医师"))
                        dt.Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                else if (level.EndsWith("主任医师"))
                        dt.Rows[i]["主任医师"] = dtTemp.Rows[0]["主任医师"];
                else if (level.EndsWith("主治医师"))
                        dt.Rows[i]["主治医师"] = dtTemp.Rows[0]["主治医师"];
                }

                i++;

            }

            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);
            //}

        }
        //if (dtTemp == null || dtTemp.Rows.Count == 0) return dtTemp;
        //else
            return dt;
    }
    [WebMethod(Description = "New 现岗查询", EnableSession = false)]
    public DataTable ValuatePSSelectEx(string startTime, string endTime, string departID, string doctor, string level, string title)
    {
        //DataSet ds = null;
        DataTable dt = new DataTable();
        DataTable newdt = new DataTable();
        DataTable dtTemp = new DataTable();
        string strTitle = "";
       
        string strSql = "select pj as '评审结果','' as '科室','' as '患者姓名',BAZLGL_kj.zyh as 住院号,'' as '" + title + "',sdf as 总得分 "

                        + " from  BAZLGL_kj    where psrq between '" + startTime + "' and '" + endTime + "'";

        if (level != "" && level != "全部")
            strSql = strSql + " and pj ='" + level + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;
            string strSelectHIS="";
            string strSelectHISOut="";
            while (i < count)
            {
                strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',tdjkz.zyysbm as '主管医师',tdjkz.zzysbm as '主治医师',tdjkz.zrys as '主任医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";

              
                strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',tdjk.zyysbm as '主管医师',tdjk.zzysbm as '主治医师',tdjk.zrys as '主任医师'   from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";
                if (departID != "$")
                {                   
                    strSelectHIS = strSelectHIS + " and tdjkz.ksbm= '" + departID + "'";
                    strSelectHISOut = strSelectHISOut + " and tdjk.ksbm= '" + departID + "'";
                }

                if (title == "主任医师")
                {
                    if (doctor == "$")
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zrys!= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys!= '" + doctor + "'";
                    }
                    else
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zrys= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys= '" + doctor + "'";
                    }
                }
                else if (title == "主治医师")
                {
                    if (doctor == "$")
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zrys!= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys!= '" + doctor + "'";
                    }
                    else
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zzysbm= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys= '" + doctor + "'";
                    }
                }
                else if (title == "主管医师" )
                {
                    if (doctor == "$")
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zrys!= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys!= '" + doctor + "'";
                    }
                    else
                    {
                        strSelectHIS = strSelectHIS + " and tdjkz.zyysbm= '" + doctor + "'";
                        strSelectHISOut = strSelectHISOut + " and tdjk.zrys= '" + doctor + "'";
                    }
                }

                dtTemp = HelperHis.GetDataTable(strSelectHIS);
              
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                   // dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    if (title.EndsWith("主管医师")&&  dtTemp.Rows[0]["主管医师"]!="")
                        dt.Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];

                    else if (title.EndsWith("主任医师") && dtTemp.Rows[0]["主任医师"] != "")
                        dt.Rows[i]["主任医师"] = dtTemp.Rows[0]["主任医师"];
                    else if (title.EndsWith("主治医师") && dtTemp.Rows[0]["主治医师"] != "")
                        dt.Rows[i]["主治医师"] = dtTemp.Rows[0]["主治医师"];
                    else
                        dt.Rows.RemoveAt(i);
                }else
                    dt.Rows.RemoveAt(i);

                i++;

            }

            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);
            //}
           
        }
        //if (dtTemp == null || dtTemp.Rows.Count == 0) return dtTemp;
        //else
            return dt;
    }
    [WebMethod(Description = " 现岗查询全院", EnableSession = false)]
    public DataTable ValuateNowPSSelectQy(string item1, string item2)
    {
        //DataSet ds = null;
        DataTable dt = null;
        DataTable newdt = new DataTable();
        DataTable dtTemp = null;
        string strSql = "select '' as '住院日期','' as '科室',pszjys as '质检医师',BAZLGL_kj.psbah as 病案号,BAZLGL_kj.zyh as 住院号,BAZLGL_kj.GKF as 共扣分,sdf as 总得分,pj as 评审结果,mx.kfxm as 扣分项目, mx.kfyy as 扣分原因 "

                        + " from  BAZLGL_kj  inner join BAZLGL_kfmx as mx ON  BAZLGL_kj.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";
                dtTemp = HelperHis.GetDataTable(strSelectHIS);
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                    dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    //dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                }

                i++;

            }
            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);
            //}         
        }
        return dt;
    }
    //NEW 修改评分新方法 2013-08-07
    [WebMethod(Description = " 现岗", EnableSession = false)]
    public bool DeleteAndInsertValue(XmlNode xmg)
    {
        bool flag = false;
        try
        {
            string strInsertSql2 = "";
            string strInsertSql1 = "";
            string strDelSql = "";


            //string registryID = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
            string psbah = xmg.Attributes[EmrConstant.AttributeNames.ArchiveNum].Value.ToString();
            string pscyrq = xmg.Attributes[EmrConstant.AttributeNames.PSCYRQ].Value.ToString();
            string psrq = xmg.Attributes[EmrConstant.AttributeNames.PSRQ].Value.ToString();
            string pszjys = xmg.Attributes[EmrConstant.AttributeNames.PSZJYS].Value.ToString();
            string pj = xmg.Attributes[EmrConstant.AttributeNames.PJ].Value.ToString();
            string bapsbz = xmg.Attributes[EmrConstant.AttributeNames.BAPSBZ].Value.ToString();
            //string departID = xmg.Attributes[EmrConstant.AttributeNames.DepartmentCode].Value.ToString();
            string gkf = xmg.Attributes[EmrConstant.AttributeNames.GKF].Value.ToString();
            string zyh = xmg.Attributes[EmrConstant.AttributeNames.RegistryID].Value.ToString();
            string sdf = xmg.Attributes[EmrConstant.AttributeNames.SDF].Value.ToString();
            strInsertSql1 = "INSERT INTO [BAZLGL_KJ]" +
          "(zyh,[PSBAH]" +
           ",[PSCYRQ]" +
          ",[PSRQ]" +
          ",[PJ]" +
          ",[PSZJYS]" +
          ",[GKF]" +
          ",[SDF]" +
          ",[BAPSBZ]" +
          ",[shbz]" +
          ",[shry]" +
          ",[shrq])" +
    "VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + psrq + "','" + pj + "','" + pszjys + "'," + gkf + "," + sdf + ",'"
    + bapsbz + "','','','');";
            strDelSql = "Delete BAZLGL_KJ  WHERE zyh ='" + zyh + "'; Delete BAZLGL_KFMX  WHERE zyh ='" + zyh + "'";


            //SqlHelper Helper = new SqlHelper("EmrDB");
            //int count = Helper.ExecuteNonQuery(strSql);
            //  string kf=xmg.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
            foreach (XmlNode Flaw in xmg.ChildNodes)
            {
                //string qxmc=Flaw.Attributes[EmrConstant.AttributeNames.QXMC].Value.ToString();
                //string qxmcxh=Flaw.Attributes[EmrConstant.AttributeNames.QXMCXH].Value.ToString();
                string kf = Flaw.Attributes[EmrConstant.AttributeNames.KF].Value.ToString();
                string kfxm = Flaw.Attributes[EmrConstant.AttributeNames.KFXM].Value.ToString();
                string kfyy = Flaw.Attributes[EmrConstant.AttributeNames.KFYY].Value.ToString();
                string pfbz = Flaw.Attributes[EmrConstant.AttributeNames.GLMC].Value.ToString();

                strInsertSql2 += "  INSERT INTO [BAZLGL_KFMX]" +
                "(zyh,[PSBAH]" +
                 ",[PSCYRQ]" +
                ",[lb]" +
                ",[kfxm]" +
                ",[kfyy]" +
                ",[kf] ,pfbz)" +

          "VALUES ('" + zyh + "'," + psbah + ",'" + pscyrq + "','" + 0 + "','" + kfxm + "','" + kfyy + "','" + kf + "','" + pfbz + "');";
                //SqlHelper Helper2 = new SqlHelper("EmrDB");
                //int count2 = Helper2.ExecuteNonQuery(strSql2);

                //if (count2 == 1)
                //    flag = true;
            }
            List<string> SQLStringList = new List<string>();
            SQLStringList.Add(strDelSql);
            SQLStringList.Add(strInsertSql1);
            SQLStringList.Add(strInsertSql2);
            ExecuteSqlTran(SQLStringList);
            flag = true;
        }
        catch
        {
            flag = false;
        }

        return flag;
    }
    /// <summary>
    /// 执行多条SQL语句，实现数据库事务。
    /// </summary>sql2005数据库
    /// <param name="SQLStringList">多条SQL语句</param>     
    public void ExecuteSqlTran(List<string> SQLStringList)
    {
        using (SqlConnection conn = new SqlConnection(EmrDB))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            SqlTransaction tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                for (int n = 0; n < SQLStringList.Count; n++)
                {
                    string strsql = SQLStringList[n].ToString();
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                tx.Rollback();
                throw new Exception(E.Message);
            }
        }
    }


    [WebMethod(Description = " 终末查询全院", EnableSession = false)]
    public DataTable ValuateEndPSSelectQy(string item1, string item2)
    {
        //DataSet ds = null;
        DataTable dt = null;
        DataTable newdt = new DataTable();
        DataTable dtTemp = null;
        string strSql = "select '' as '住院日期','' as '科室',pszjys as '质检医师',BAZLGL_End.psbah as 病案号,BAZLGL_End.zyh as 住院号,BAZLGL_End.GKF as 共扣分,sdf as 总得分,pj as 评审结果,mx.kfxm as 扣分项目, mx.kfyy as 扣分原因 "

                        + " from  BAZLGL_End  inner join BAZLGL_End_kfmx as mx ON  BAZLGL_End.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.zyrq,120) as '住院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["住院号"].ToString() + "'";
                dtTemp = HelperHis.GetDataTable(strSelectHIS);
                if (dtTemp == null || dtTemp.Rows.Count == 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);
                if (dtTemp != null && dtTemp.Rows.Count != 0)
                {

                    //dt.Tables[0].Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                    dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                    dt.Rows[i]["住院日期"] = dtTemp.Rows[0]["住院日期"];

                    //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                    //dt.Tables[0].Rows[i]["主管医师"] = dtTemp.Rows[0]["主管医师"];
                }

                i++;

            }
            //newdt = dt.Clone();
            //DataRow[] dr = dt.Select("科室='" + departID + "'");
            //for (int j = 0; j < dr.Length; j++)
            //{
            //    newdt.ImportRow((DataRow)dr[j]);
            //}         
        }
        return dt;
    }
    [WebMethod(Description = " 现岗查询", EnableSession = false)]
    public DataTable ValuateNowPSSelectHz(string item1, string item2)
    {
        DataTable dt = null;
        DataTable dtTemp = null;
        //DataSet ds = null;
        string strSql = "select '' as '科室','' as 责任医师,'' as  病历总数, '' as 参评病历,'' as 平均分,''as 甲级病历, '' as 甲级率, "

                        + "'' as 乙级,'' as 乙级率, '' as 丙级,'' as 丙级率,BAZLGL_kj.zyh  from  BAZLGL_kj  inner join BAZLGL_kfmx as mx ON  BAZLGL_kj.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["zyh"].ToString() + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["zyh"].ToString() + "'";
                dtTemp = HelperHis.GetDataTable(strSelectHIS);
                if (dtTemp == null || dtTemp.Rows.Count != 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);


            }
            if (dtTemp != null && dtTemp.Rows.Count != 0)
            {

                dt.Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                dt.Rows[i]["责任医师"] = dtTemp.Rows[0]["主管医师"];
            }

            i++;
        }
        return dt;
    }
    [WebMethod(Description = " 终末查询", EnableSession = false)]
    public DataTable ValuateEndPSSelectHz(string item1, string item2)
    {
        DataTable dt = null;
        DataTable dtTemp = null;
        //DataSet ds = null;
        string strSql = "select '' as '科室','' as 责任医师,'' as  病历总数, '' as 参评病历,'' as 平均分,''as 甲级病历, '' as 甲级率, "

                        + "'' as 乙级,'' as 乙级率, '' as 丙级,'' as 丙级率,BAZLGL_End.zyh  from  BAZLGL_End  inner join BAZLGL_End_kfmx as mx ON  BAZLGL_End.zyh=mx.zyh  where psrq between '" + item1 + "' and '" + item2 + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];

        if (dt != null && dt.Rows.Count != 0)
        {
            SqlHelper HelperHis = new SqlHelper("HisDB");
            int i = 0;
            int count = dt.Rows.Count;

            while (i < count)
            {
                string strSelectHIS = @"select tdjkz.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjkz.xm as '患者姓名',convert(varchar(10),tdjkz.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjkz LEFT JOIN Mz_ksbm ON tdjkz.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjkz.zrys = sj_czydm.czydm where tdjkz.zyh = '" + dt.Rows[i]["zyh"].ToString() + "'";

                string strSelectHISOut = @"select tdjk.bah as '病案号',Mz_ksbm.ksmc as '科室',tdjk.xm as '患者姓名',convert(varchar(10),tdjk.cyrq,120) as '出院日期',sj_czydm.czyxm as '主管医师'  from tdjk LEFT JOIN Mz_ksbm ON tdjk.ksbm = Mz_ksbm.ksbm 
                                            Left JOIN sj_czydm on tdjk.zrys = sj_czydm.czydm where tdjk.zyh = '" + dt.Rows[i]["zyh"].ToString() + "'";
                dtTemp = HelperHis.GetDataTable(strSelectHIS);
                if (dtTemp == null || dtTemp.Rows.Count != 0)
                    dtTemp = HelperHis.GetDataTable(strSelectHISOut);


            }
            if (dtTemp != null && dtTemp.Rows.Count != 0)
            {

                dt.Rows[i]["病案号"] = dtTemp.Rows[0]["病案号"];
                dt.Rows[i]["科室"] = dtTemp.Rows[0]["科室"];
                //dt.Rows[i]["患者姓名"] = dtTemp.Rows[0]["患者姓名"];
                dt.Rows[i]["责任医师"] = dtTemp.Rows[0]["主管医师"];
            }

            i++;
        }
        return dt;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public DataTable ValuateNowPSSelect(string item1, string item2, string departID)
    {
        DataTable dt = null;
        //string strSql = "SELECT * FROM ValuateDetailNew WHERE departID='"+departID+"'  and bapsbz=1";
        //string strSql = "SELECT psbah as 病案号, 100-sum(KF) as 总得分  FROM ValuateDetailNew WHERE departID='" + departID + "  and bapsbz=1  group by psbah ";
        string strSelect = "SELECT * FROM [ValuateNow] " +
                 " WHERE zyrq BETWEEN '" + item1 + "' AND '" + item2 + "'  AND  ksbm='" + departID + "'  order by sdf desc";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSelect);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public DataTable ValuateNowPSSelectQ(string item1, string item2)
    {
        DataTable dt = null;
        //string strSql = "SELECT * FROM ValuateDetailNew WHERE departID='"+departID+"'  and bapsbz=1";
        //string strSql = "SELECT psbah as 病案号, 100-sum(KF) as 总得分  FROM ValuateDetailNew WHERE departID='" + departID + "  and bapsbz=1  group by psbah ";
        string strSelect = "SELECT * FROM [ValuateNow] " +
                 " WHERE zyrq BETWEEN '" + item1 + "' AND '" + item2 + "' order by sdf desc";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSelect);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;
    }


    //[WebMethod(Description = " ", EnableSession = false)]
    //public DataTable ValuateNowPSSelect2(string departID, string registryID)
    //{
    //    DataTable dt = null;
    //     string strSql2 ="SELECT psbah as 病案号, 100-sum(KF) as 总得分 ," +
    //                     "(100-sum(KF))/count(distinct psbah) as 平均分," +
    //                     "'病历等级'= " +
    //                     "case  " +
    //                     "when (100-sum(KF))<60 then '丙级病历'" +
    //                     "when (100-sum(KF))<80 then '乙级病历' else '甲级病历' end " +
    //                     "FROM ValuateDetailNew WHERE departID='1001' and bapsbz=1  group by psbah ";

    //    SqlHelper Helper = new SqlHelper("EmrDB");
    //    DataSet myDataSet = Helper.GetDataSet(strSql2);
    //    if (myDataSet.Tables.Count > 0)
    //        dt = myDataSet.Tables[0];
    //    return dt;
    //}
    [WebMethod(Description = " ", EnableSession = false)]
    public DataTable ValuateNowPjL(string start, string end)
    {
        DataTable dt = null;
        string strSql = "declare @ACount int select @ACount=count(*) from [ValuateNow] " +
       "select [ValuateNow].KSMC as 科室,cast(@ACount as numeric(10,0)) as 科室质检数, [ValuateNow].PJ as 评级,cast(count(*) as numeric(10,0)) as 病历数,cast(count(*) as numeric(10,3))/cast(@ACount as numeric(10,3)) as 病历率 from [ValuateNow] where [ValuateNow].zyrq between '" + start + "' and '" + end + "' group by [ValuateNow].PJ,[ValuateNow].KSMC ";


        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet myDataSet = Helper.GetDataSet(strSql);
        if (myDataSet.Tables.Count > 0)
            dt = myDataSet.Tables[0];
        return dt;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public int DelValuateNow(string zyh)
    {

        string strSql = "Delete BAZLGL_KJ  WHERE zyh ='" + zyh + "'; Delete BAZLGL_KFMX  WHERE zyh ='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strSql);

        return count;
    }
    [WebMethod(Description = "终末 ", EnableSession = false)]
    public int DelValuateEnd(string zyh)
    {

        string strSql = "Delete BAZLGL_End  WHERE zyh ='" + zyh + "'; Delete BAZLGL_KFMX  WHERE zyh ='" + zyh + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        int count = Helper.ExecuteNonQuery(strSql);

        return count;
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public DataSet GetValuateNowYsm(string bah)
    {

        string strSql = "SELECT dzy,dzz,dzr FROM [archive].[dbo].[BA_MAIN] WHERE bah ='" + bah + "'";

        SqlHelper Helper = new SqlHelper("HisDB");
        DataSet ds = Helper.GetDataSet(strSql);

        return ds;
    }
}


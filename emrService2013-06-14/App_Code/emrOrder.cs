using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using EmrConstant;
using System.Data.OracleClient;
using AboutConfig;
using System.Runtime.InteropServices;
//using Emr = EmrConstant;

    /// <summary>
    /// Summary description for EmrOrder
    /// </summary>
[WebService(Namespace = "http://shoujia.org/emrorders/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class EmrOrder : System.Web.Services.WebService
{
    private string connectString;
    private string hisDBType;
    private string dbcf;

    public EmrOrder()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
        connectString = ConfigClass.GetConfigString("appSettings", "HisDB");
        hisDBType = ConfigClass.GetConfigString("appSettings", "HisDBType");
        dbcf = ConfigClass.GetConfigString("appSettings", "DBCF");
    }
    private DateTime today()
    {
        switch (hisDBType)
        {
            case "ORACLE":
                OracleConnection oconn = new OracleConnection(connectString);
                oconn.Open();
                OracleCommand ocommand = new OracleCommand("SELECT SYSDATE FROM dual", oconn);
                OracleDataReader oreader = ocommand.ExecuteReader();
                oreader.Read();
                DateTime onow = Convert.ToDateTime(oreader[0]);
                oreader.Close();
                oconn.Close();
                return onow;
            case "MSSQL":
                SqlConnection sconn = new SqlConnection(connectString);
                sconn.Open();
                SqlCommand scommand = new SqlCommand("SELECT GETDATE()", sconn);
                SqlDataReader sreader = scommand.ExecuteReader();
                sreader.Read();
                DateTime snow = Convert.ToDateTime(sreader[0]);
                sreader.Close();
                sconn.Close();
                return snow;
        }
        return DateTime.Now;
    }
    private DataSet ExecuteSentence(string sqlSentence, string hisDBType)
    {
        DataSet myDataSet = new DataSet();
        switch (hisDBType)
        {
            case "ORACLE":
                OracleConnection oconn = new OracleConnection(connectString);
                oconn.Open();
                OracleDataAdapter ocommand = new OracleDataAdapter(sqlSentence, oconn);
                ocommand.Fill(myDataSet, "Results");
                oconn.Close();
                break;
            case "MSSQL":
                SqlConnection sconn = new SqlConnection(connectString);
                sconn.Open();
                SqlDataAdapter scommand = new SqlDataAdapter(sqlSentence, sconn);
                scommand.Fill(myDataSet, "Results");
                sconn.Close();
                break;
        }
        return myDataSet;
    }
    private string ExecuteNoQureySentence(string sqlSentence, string hisDBType)
    {
        string ret = null;
        switch (hisDBType)
        {
            case "ORACLE":
                OracleConnection oconn = new OracleConnection(connectString);
                oconn.Open();
                OracleCommand ocommand = new OracleCommand(sqlSentence, oconn);
                try
                {
                    ocommand.ExecuteNonQuery();
                    oconn.Close();
                }
                catch (InvalidOperationException ex)
                {
                    ret = ex.Message + "--" + ex.Source;
                }
                break;
            case "MSSQL":
                SqlConnection sconn = new SqlConnection(connectString);
                sconn.Open();
                SqlCommand scommand = new SqlCommand(sqlSentence, sconn);
                try
                {
                    scommand.ExecuteNonQuery();
                    sconn.Close();
                }
                catch (InvalidOperationException ex)
                {
                    ret = ex.Message + "--" + ex.Source;
                }
                break;
        }
        return ret;
    }
    private DataSet ExecuteSentenceEmr(string select)
    {
        DataSet myDataSet = new DataSet();
        SqlConnection cs = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        cs.Open();
        SqlDataAdapter scommand = new SqlDataAdapter(select, cs);
        scommand.Fill(myDataSet, "Results");
        cs.Close();
        return myDataSet;
    }

    #region Drug info
    [WebMethod(Description = "Returns drug list for one pharmacy")]
    public DataSet DrugListForOnePharmacy(string pharmacyCode)
    {
        string select =
            "SELECT a.bxbs AS 医保, a.pm AS 名称, a.gg AS 规格, b.fyjg AS 发药价格, " +
                "a.dw1 AS 发药单位, b.zxjg AS 执行价格, a.dw2 AS 执行单位, a.zhs, a.dw3, " +
                "a.bm, a.pym, a.flh3, a.flh4, a.bxxl," +
                "a.gybs, a.hss, a.bxxs, a.gfxl, a.gfxs, a.mzyybs, a.flh1, " +
                "a.gsbxbs, a.syfwxd, b.zxys, a.dl, b.yfbm as pharmacyCode, a.pzwh, a.dmbs," +
                "a.zxfsbm, a.yzpcbm, a.rzdyl, a.alias " +
                "FROM yp_zy a INNER JOIN yf_zy b ON a.bm = b.bm " +
                "WHERE b.zxys > 0 AND b.yfbm = '" + pharmacyCode + "' ORDER BY pym";
        //AND dl IN (SELECT dl
        //             FROM ypdlbm
        //            WHERE gsdl = 'A');
        DataSet dsDrugsForOnePharmacy = ExecuteSentence(select, hisDBType);
        dsDrugsForOnePharmacy.Tables[0].TableName = EmrConstant.DbTables.DrugList + pharmacyCode;
        return dsDrugsForOnePharmacy;
    }

    [WebMethod(Description = "Returns drug list for one pharmacy")]
    public DataSet AnaesthesiaDrugList(string pharmacyCode)
    {
        string select =
            "SELECT yp_zy.bxbs AS 医保, yp_zy.pm AS 名称, yp_zy.gg AS 规格, yf_zy.fyjg AS 发药价格, " +
                "yp_zy.dw1 AS 发药单位, yf_zy.zxjg AS 执行价格, yp_zy.dw2 AS 执行单位, yp_zy.zhs, yp_zy.dw3, " +
                "yp_zy.bm, yp_zy.pym, yp_zy.flh3, yp_zy.flh4, yp_zy.bxxl," +
                "yp_zy.gybs, yp_zy.hss, yp_zy.bxxs, yp_zy.gfxl, yp_zy.gfxs, yp_zy.mzyybs, yp_zy.flh1, " +
                "yp_zy.gsbxbs, yp_zy.syfwxd,yf_zy.zxys,yp_zy.dl, yf_zy.yfbm as pharmacyCode " +
                "FROM yp_zy INNER JOIN yf_zy ON yp_zy.bm = yf_zy.bm " +
                "WHERE  yf_zy.yfbm='" + pharmacyCode + "' " +
                "AND yf_zy.zxys > 0 AND yp_zy.dmbs = '1'";
        DataSet dsDrugs = ExecuteSentence(select, hisDBType);
        dsDrugs.Tables[0].TableName = EmrConstant.DbTables.AnaesthesiaDrugList + pharmacyCode;
        return dsDrugs;
    }

    [WebMethod(Description = "Returns drug way list")]
    public DataSet DrugWayList()
    {
        string select =
            "SELECT zxfsbm.zxfsbm as code, zxfsbm.zxfs as text, zxfsbm.czfybm feeCode, zxfsbm.clfbm," +
            "zxfsbm.yylx as kind, zxfsbm.dyfl " +
            "FROM zxfsbm LEFT OUTER JOIN tsfxm ON zxfsbm.czfybm = tsfxm.bm ";
        DataSet dsWay = ExecuteSentence(select, hisDBType);
        dsWay.Tables[0].TableName = EmrConstant.DbTables.DrugWayList;
        return dsWay;
    }

    [WebMethod(Description = "Returns Chinese drug way list")]
    public DataSet ChineseDrugWayList()
    {
        string select =
            "SELECT bm as code, mc as text FROM mz_cyyfbm ";
        DataSet dsWay = ExecuteSentence(select, hisDBType);
        dsWay.Tables[0].TableName = EmrConstant.DbTables.ChineseDrugWayList;
        return dsWay;
    }

    [WebMethod(Description = "Returns Chinese drug how list")]
    public DataSet ChineseDrugHowList()
    {
        string select =
            "SELECT bm as code, mc as text, lx, dybs FROM mz_ypffbm ORDER BY code";
        DataSet dsHow = ExecuteSentence(select, hisDBType);
        dsHow.Tables[0].TableName = EmrConstant.DbTables.ChineseDrugHowList;
        return dsHow;
    }

    [WebMethod(Description = "Returns drug time list")]
    public DataSet DrugTimeList()
    {
        string select =
            "SELECT pm as text, pmsm as description, pym , cs as count, sl quantity, ts as days, " +
            "bm as code, yzsj " +
            "FROM yzyyk";
        DataSet dsTimeList = ExecuteSentence(select, hisDBType);
        dsTimeList.Tables[0].TableName = EmrConstant.DbTables.DrugTimeList;
        return dsTimeList;
    }

    [WebMethod(Description = "Returns pharmacy list")]
    public DataSet PharmacyList()
    {
        string select =
            "SELECT ksmc as name, yfbm as code, yflx as type FROM mz_ksbm WHERE yfbm IS NOT NULL";
        DataSet dsPharmacyList = ExecuteSentence(select, hisDBType);
        dsPharmacyList.Tables[0].TableName = EmrConstant.DbTables.PharmacyList;
        return dsPharmacyList;
    }

    [WebMethod(Description = "Returns drug kind list")]
    public DataSet DrugKindList()
    {
        string select = "SELECT dl, dlmc FROM ypdlbm";
        DataSet dsDrugKindList = ExecuteSentence(select, hisDBType);
        dsDrugKindList.Tables[0].TableName = EmrConstant.DbTables.DrugKind;
        return dsDrugKindList;
    }

    [WebMethod(Description = "Returns combine conflict of the grugs")]
    public DataSet CombineConflict()
    {
        string select = "SELECT ypbm, jjypbm, jjlx FROM yp_cypwjj";
        DataSet dsConflict = ExecuteSentence(select, hisDBType);
        dsConflict.Tables[0].TableName = EmrConstant.DbTables.CombineConflict;
        return dsConflict;
    }

    [WebMethod(Description = "Returns drug list with drugWayList, drugTimeList and pharmacyList")]
    public DataSet DrugListEx(string[] pharmacyCodes, int pharmacyCount)
    {
        DataSet dsDrugs = DrugWayList();

        DataSet dsConflic = CombineConflict();
        DataTable conflict = dsConflic.Tables[0].Copy();
        dsDrugs.Tables.Add(conflict);

        DataSet dsChineseDrugWay = ChineseDrugWayList();
        DataTable chineseDrugWay = dsChineseDrugWay.Tables[0].Copy();
        dsDrugs.Tables.Add(chineseDrugWay);

        DataSet dsChineseDrugHow = ChineseDrugHowList();
        DataTable chineseDrugHow = dsChineseDrugHow.Tables[0].Copy();
        dsDrugs.Tables.Add(chineseDrugHow);

        DataSet dsDrugKind = DrugKindList();
        DataTable drugKind = dsDrugKind.Tables[0].Copy();
        dsDrugs.Tables.Add(drugKind);

        DataSet dsDrugTime = DrugTimeList();
        DataTable tableTime = dsDrugTime.Tables[0].Copy();
        dsDrugs.Tables.Add(tableTime);

        DataSet dsPharmacy = PharmacyList();
        DataTable tablePharmacy = dsPharmacy.Tables[0].Copy();
        dsDrugs.Tables.Add(tablePharmacy);

        for (int i = 0; i < pharmacyCount; i++)
        {
            DataSet dsDrugList = DrugListForOnePharmacy(pharmacyCodes[i]);
            DataTable tableDrugList = dsDrugList.Tables[0].Copy();
            dsDrugs.Tables.Add(tableDrugList);
        }

        for (int j = 0; j < pharmacyCount; j++)
        {
            DataSet dsAnaesthesiaDrugList = AnaesthesiaDrugList(pharmacyCodes[j]);
            if (dsAnaesthesiaDrugList.Tables[0].Rows.Count == 0) continue;
            DataTable tableDrugList = dsAnaesthesiaDrugList.Tables[0].Copy();
            dsDrugs.Tables.Add(tableDrugList);
        }

        return dsDrugs;
    }
    #endregion
    #region keyword
   



    #endregion



    #region Maintain predefined orders
    [WebMethod(Description = "Returns predefined orders for one doctor")]
    public DataSet PredefinedOrderForDoctor(string doctorID)
    {

        string select = "SELECT * FROM PredefinedOrder WHERE DoctorID = '" + doctorID + "'";
        DataSet dsPredefinedOrder = ExecuteSentenceEmr(select);
        dsPredefinedOrder.Tables[0].TableName = EmrConstant.DbTables.PredefinedOrder + doctorID;
        return dsPredefinedOrder;
    }

    [WebMethod(Description = "Returns predefined orders for one department")]
    public DataSet PredefinedOrderForDepartment(string departmentCode)
    {

        string select = "SELECT * FROM PredefinedOrder WHERE DepartmentCode = '" + departmentCode + "'";
        DataSet dsPredefinedOrder = ExecuteSentenceEmr(select);
        dsPredefinedOrder.Tables[0].TableName = EmrConstant.DbTables.PredefinedOrder + departmentCode;
        return dsPredefinedOrder;
    }

    [WebMethod(Description = "Returns predefined order items for one doctor")]
    public DataSet PredefinedItemsForDoctor(string doctorID)
    {

        string select = "SELECT a.* FROM PredefinedPrescript a " +
            "INNER JOIN PredefinedOrder b ON a.OrderPk = b.Pk " +
            "WHERE b.DoctorID = '" + doctorID + "'";
        DataSet dsPredefinedItems = ExecuteSentenceEmr(select);
        dsPredefinedItems.Tables[0].TableName = EmrConstant.DbTables.PredefinedOrderItems + doctorID;
        return dsPredefinedItems;
    }

    [WebMethod(Description = "Returns predefined order items for one department")]
    public DataSet PredefinedItemsForDepartment(string departmentCode)
    {

        string select = "SELECT * FROM PredefinedPrescript " +
            "INNER JOIN PredefinedOrder ON PredefinedPrescript.OrderPk = PredefinedOrder.Pk " +
            "WHERE PredefinedOrder.DepartmentCode = '" + departmentCode + "'";
        DataSet dsPredefinedItems = ExecuteSentenceEmr(select);
        dsPredefinedItems.Tables[0].TableName = EmrConstant.DbTables.PredefinedOrderItems + departmentCode;
        return dsPredefinedItems;
    }

    [WebMethod(Description = "Returns private drug names")]
    public DataSet PrivateDrugList(string doctorID)
    {
        string select = "SELECT mz_gryp.bm, yp_zy.pm, mz_gryp.dl, ypdlbm.dlmc FROM mz_gryp " +
            "INNER JOIN yp_zy ON mz_gryp.bm=yp_zy.bm " +
            "INNER JOIN ypdlbm ON mz_gryp.dl = ypdlbm.dl " +
            "WHERE mz_gryp.ysbm = '" + doctorID + "' UNION ALL " +
            "SELECT mz_gryp.bm, tsfxm.pm, mz_gryp.dl, ypdlbm.dlmc FROM mz_gryp " +
            "INNER JOIN tsfxm ON mz_gryp.bm=tsfxm.bm " +
            "INNER JOIN ypdlbm ON mz_gryp.dl = ypdlbm.dl " +
            "WHERE mz_gryp.ysbm = '" + doctorID + "'";
        DataSet dsPrivateDrugNames = ExecuteSentence(select, hisDBType);
        dsPrivateDrugNames.Tables[0].TableName = EmrConstant.DbTables.PrivateDrugNames + doctorID;
        return dsPrivateDrugNames;
    }

    [WebMethod(Description = "Add a row into private drug names(mz_gryp)")]
    public string AddPrivateDrug(string drugCode, string doctorID, string drugKindCode)
    {
        string insert = "INSERT INTO mz_gryp VALUES('" + drugCode + "','" + doctorID + "','" + drugKindCode + "')";

        return ExecuteNoQureySentence(insert, hisDBType);
    }

    [WebMethod(Description = "Returns predefined order and their items")]
    public DataSet PredefinedFull(string doctorID, string departmentCode)
    {
        DataSet predefined = new DataSet();
        DataSet tmp = PredefinedOrderForDoctor(doctorID);
        DataTable predefinedOrder = tmp.Tables[0].Copy();
        predefined.Tables.Add(predefinedOrder);

        tmp = PredefinedItemsForDoctor(doctorID);
        DataTable predefinedItems = tmp.Tables[0].Copy();
        predefined.Tables.Add(predefinedItems);

        tmp = PredefinedItemsForDepartment(departmentCode);
        DataTable predefinedItemsForDepartment = tmp.Tables[0].Copy();
        predefined.Tables.Add(predefinedItemsForDepartment);

        tmp = PredefinedOrderForDepartment(departmentCode);
        DataTable predefinedOrderForDepartment = tmp.Tables[0].Copy();
        predefined.Tables.Add(predefinedOrderForDepartment);

        tmp = PrivateDrugList(doctorID);
        DataTable privateDrugNames = tmp.Tables[0].Copy();
        predefined.Tables.Add(privateDrugNames);
        return predefined;


    }

    [WebMethod(Description = "Add predefined order and their items")]
    public string AddPredefinedFull(string doctorID, string departmentCode, DataSet predefined, ref int orderPk)
    {
        #region Table name
        DataTable order = null;
        DataTable prescript = null;
        if (doctorID == EmrConstant.StringGeneral.NullCode)
        {
            order = predefined.Tables[EmrConstant.DbTables.PredefinedOrder + departmentCode];
            if (order == null) return "No data table PredefinedOrder";
            prescript = predefined.Tables[EmrConstant.DbTables.PredefinedOrderItems + departmentCode];
            if (prescript == null) return "No data table PredefinedOrderItems";
        }
        else
        {
            order = predefined.Tables[EmrConstant.DbTables.PredefinedOrder + doctorID];
            if (order == null) return "No data table PredefinedOrder";
            prescript = predefined.Tables[EmrConstant.DbTables.PredefinedOrderItems + doctorID];
            if (prescript == null) return "No data table PredefinedOrderItems";
        }
        #endregion

        using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
        {

            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;
            connection.Open();
            transaction = connection.BeginTransaction();

            /* Must assign both transaction object and connection to Command object
             * for a pending local transaction */
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                #region Insert into PredefinedOrder
                orderPk = AddPredefinedOrder(command, order);
                #endregion

                #region Insert into PredefinedPrescript
                AddPredefinedPrecsript(command, prescript, orderPk);
                #endregion

                transaction.Commit();
                return null;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ex.Message + "--" + ex.Source;

            }
        }
    }
    private int AddPredefinedOrder(SqlCommand command, DataTable order)
    {
        command.CommandText = "INSERT INTO PredefinedOrder VALUES ('" +
            order.Rows[0][EmrConstant.DataColumnNames.SpellCode] + "','" +
            order.Rows[0][EmrConstant.DataColumnNames.Description] + "','" +
            order.Rows[0][EmrConstant.DataColumnNames.DoctorID] + "','" +
            order.Rows[0][EmrConstant.DataColumnNames.DepartmentCode] + "')";
        command.ExecuteNonQuery();
        command.CommandText = "SELECT Pk FROM PredefinedOrder " +
            "WHERE SpellCode='" + order.Rows[0][EmrConstant.DataColumnNames.SpellCode] + "' AND " +
            "DoctorID='" + order.Rows[0][EmrConstant.DataColumnNames.DoctorID] + "'";
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        int orderPk = Convert.ToInt32(reader[0]);
        reader.Close();
        return orderPk;
    }
    private void AddPredefinedPrecsript(SqlCommand command, DataTable prescript, int orderPk)
    {
        command.CommandText = "INSERT INTO PredefinedPrescript VALUES " +
        "(@orderPk,@itemCode,@itemName,@quantity," +
        "@quantityUnit,@timesText,@wayCode,@days,@doctorExhort,@orderType)";
        command.Parameters.Add(new SqlParameter("@orderPk", SqlDbType.BigInt));
        command.Parameters.Add(new SqlParameter("@itemCode", SqlDbType.VarChar, 8));
        command.Parameters.Add(new SqlParameter("@itemName", SqlDbType.VarChar, 50));
        command.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Float));
        command.Parameters.Add(new SqlParameter("@quantityUnit", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@timesText", SqlDbType.VarChar, 10));
        command.Parameters.Add(new SqlParameter("@wayCode", SqlDbType.VarChar, 10));
        command.Parameters.Add(new SqlParameter("@days", SqlDbType.Float));
        command.Parameters.Add(new SqlParameter("@doctorExhort", SqlDbType.VarChar, 100));
        command.Parameters.Add(new SqlParameter("@orderType", SqlDbType.VarChar, 1));
        //command.Prepare();
        foreach (DataRow row in prescript.Rows)
        {
            command.Parameters[0].Value = orderPk;
            command.Parameters[1].Value = row[EmrConstant.DataColumnNames.ItemCode];
            command.Parameters[2].Value = row[EmrConstant.DataColumnNames.ItemName];
            command.Parameters[3].Value = row[EmrConstant.DataColumnNames.Quantity];
            command.Parameters[4].Value = row[EmrConstant.DataColumnNames.QuantityUnit];
            command.Parameters[5].Value = row[EmrConstant.DataColumnNames.TimesText];
            command.Parameters[6].Value = row[EmrConstant.DataColumnNames.WayCode];
            command.Parameters[7].Value = row[EmrConstant.DataColumnNames.Days];
            command.Parameters[8].Value = row[EmrConstant.DataColumnNames.DoctorExhort];
            command.Parameters[9].Value = row[EmrConstant.DataColumnNames.OrderType];
            command.ExecuteNonQuery();
        }
    }

    [WebMethod(Description = "Update predefined order and their items")]
    public string UpdatePredefinedFull(string doctorID, string departmentCode, DataSet predefined,
        int oldOrderPk, ref int newOrderPk)
    {
        #region Table name
        DataTable order = null;
        DataTable prescript = null;
        if (doctorID == EmrConstant.StringGeneral.NullCode)
        {
            order = predefined.Tables[EmrConstant.DbTables.PredefinedOrder + departmentCode];
            if (order == null) return "No data table PredefinedOrder";
            prescript = predefined.Tables[EmrConstant.DbTables.PredefinedOrderItems + departmentCode];
            if (prescript == null) return "No data table PredefinedOrderItems";
        }
        else
        {
            order = predefined.Tables[EmrConstant.DbTables.PredefinedOrder + doctorID];
            if (order == null) return "No data table PredefinedOrder";
            prescript = predefined.Tables[EmrConstant.DbTables.PredefinedOrderItems + doctorID];
            if (prescript == null) return "No data table PredefinedOrderItems";
        }
        #endregion

        using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
        {

            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;
            connection.Open();
            transaction = connection.BeginTransaction();

            /* Must assign both transaction object and connection to Command object
             * for a pending local transaction */
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                #region Delete from PredefinedOrder and PredefinedPrescript
                command.CommandText = "DELETE FROM PredefinedOrder WHERE pk=" + Convert.ToString(oldOrderPk);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM PredefinedPrescript WHERE OrderPk=" + Convert.ToString(oldOrderPk);
                command.ExecuteNonQuery();
                #endregion

                #region Add into PredefinedOrder and PredefinedPrescript
                newOrderPk = AddPredefinedOrder(command, order);
                AddPredefinedPrecsript(command, prescript, newOrderPk);
                #endregion

                transaction.Commit();
                return null;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ex.Message + "--" + ex.Source;

            }
        }
    }

    [WebMethod(Description = "Delete predefined order and their items")]
    public string DeletePredefinedFull(int oldOrderPk)
    {
        using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
        {
            SqlCommand command = connection.CreateCommand();
            SqlTransaction transaction;
            connection.Open();
            transaction = connection.BeginTransaction();

            /* Must assign both transaction object and connection to Command object
             * for a pending local transaction */
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                #region Delete from PredefinedOrder and PredefinedPrescript
                command.CommandText = "DELETE FROM PredefinedOrder WHERE pk=" + Convert.ToString(oldOrderPk);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM PredefinedPrescript WHERE OrderPk=" + Convert.ToString(oldOrderPk);
                command.ExecuteNonQuery();
                #endregion

                transaction.Commit();
                return null;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return ex.Message + "--" + ex.Source;

            }
        }
    }

    #endregion

    #region Out patient Orders
    //[WebMethod(Description = "Returns items and other info of one prescription")]
    //public DataSet GetOutPrescription(decimal prescriptionNum)
    //{
    //    string select = "SELECT ypxh, bm, dw, jg, ysbm, ksbm, czks, yfbm, flh3, xmbm, cfsl," +
    //    "cffs, je, cfh, hjrq, ylsp, yfyl, zybj, hl, hldw, ypyl, cs, ts," +
    //    "fsbm, zxfs, zbybs, yzzs, sfbs, fybs, tfbs, dmbs " +
    //    "FROM mz_ypmx WHERE cfh = " + prescriptionNum.ToString();

    //    DataSet dsPrescription = ExecuteSentence(select, hisDBType);
    //    dsPrescription.Tables[0].TableName = DbTables.PrescriptionItems;
    //    foreach (DataRow row in dsPrescription.Tables[0].Rows)
    //    {
    //        if (Convert.IsDBNull(row[TabPrescription.Charged]))
    //            row[TabPrescription.Charged] = StringGeneral.Zero;
    //        if (Convert.IsDBNull(row[TabPrescription.UnCharged]))
    //            row[TabPrescription.UnCharged] = StringGeneral.Zero;
    //        if (Convert.IsDBNull(row[TabPrescription.Done]))
    //            row[TabPrescription.Done] = StringGeneral.Zero;
    //        if (Convert.IsDBNull(row[TabPrescription.Aneathsia]))
    //            row[TabPrescription.Aneathsia] = StringGeneral.Zero;
    //    }
    //    return dsPrescription;
    //}
    //[WebMethod(Description = "Add items and other info of one prescription")]
    //public Decimal AddOutPrescription(DataSet dsPrescription, ref string error)
    //{
    //    switch (hisDBType)
    //    {
    //        case "ORACLE":
    //            return 0;
    //        case "MSSQL":
    //            return SqlAddOutPrescription(dsPrescription, ref error);
    //    }

    //    return 0;
    //}
    //private Decimal SqlAddOutPrescription(DataSet dsPrescription, ref string error)
    //{
    //    SqlConnection connection = new SqlConnection(connectString);
    //    connection.Open();

    //    #region New precription number
    //    decimal num = SqlNewPrescriptionNum(ref error, connection);
    //    if (num == 0)
    //    {
    //        connection.Close();
    //        return num;
    //    }
    //    #endregion

    //    bool ret = SqlAddPrescriptionItems(ref error, connection, dsPrescription.Tables[0], num);
    //    if (!ret) num = 0;

    //    return num;
    //}
    //private bool SqlAddPrescriptionItems(ref string msg, SqlConnection connection, DataTable items, decimal num)
    //{
    //    /*
    //    int notEnoughCount = 0;
    //    foreach (DataRow item in items.Rows)
    //    {
    //        string drugCode = item[EmrConstant.TabPrescription.DrugCode].ToString();
    //        string pharmacyCode = item[EmrConstant.TabPrescription.PharmacyCode].ToString();
    //        double requiredAmount = Convert.ToDouble(item[EmrConstant.TabPrescription.Total]);
    //        if (!SqlIsEnoughStock(requiredAmount, drugCode, pharmacyCode, connection))
    //        {
    //            msg += drugCode + ":";
    //            notEnoughCount++;
    //        }
    //    }
    //    if (notEnoughCount > 0) return false;
    //    SqlCommand command = new SqlCommand();
    //    SqlTransaction transaction = connection.BeginTransaction();
    //    command.Connection = connection;
    //    command.Transaction = transaction;

    //    #region Get leader doctor of the duty docotr
    //    string doctorID = items.Rows[0]["ysbm"].ToString();
    //    string registryID = items.Rows[0]["mzh"].ToString();
    //    command.CommandText = "SELECT zlzz FROM tysm WHERE ysbm='" + doctorID + "'";
    //    SqlDataReader reader = command.ExecuteReader();
    //    string zlzz = doctorID;
    //    if (reader.Read())
    //    {
    //        if (!Convert.IsDBNull(reader[0])) zlzz = reader[0].ToString();
    //    }
    //    reader.Close();
    //    #endregion

    //    try
    //    {
    //        #region Create insert statement
    //        command.CommandText = "INSERT INTO mz_ypmx (je, yfyl, zybj, hldw, ypyl, cs, ts, fsbm, zxfs, cfh," +
    //            "hjks, czks, hjry, hjrq, hl, zbybs, yzzs, yzsj, tjzs, ylsp, kh, zlzz, mzh, bm, dw, jg, ysbm, " +
    //            "ksbm, yfbm, flh3, xmbm, cfsl, cffs) VALUES " +
    //            "(@je, @yfyl, @zybj, @hldw, @ypyl, @cs, @ts,@fsbm, @zxfs, @cfh," +
    //            "@hjks, @czks, @hjry, @hjrq, @hl, @zbybs, @yzzs, @yzsj, @tjzs, @ylsp, @kh, @zlzz, " +
    //            "@mzh, @bm, @dw, @jg, @ysbm, @ksbm, @yfbm, @flh3, @xmbm, @cfsl, @cffs)";
    //        #endregion
    //        #region Create Parameters
    //        SqlParameter jeParam = new SqlParameter("@je", SqlDbType.Float, 53);
    //        SqlParameter yfylParam = new SqlParameter("@yfyl", SqlDbType.VarChar, 4);
    //        SqlParameter zybjParam = new SqlParameter("@zybj", SqlDbType.VarChar, 1);
    //        SqlParameter hldwParam = new SqlParameter("@hldw", SqlDbType.VarChar, 10);
    //        SqlParameter ypylParam = new SqlParameter("@ypyl", SqlDbType.Float, 53);
    //        SqlParameter csParam = new SqlParameter("@cs", SqlDbType.Float, 53);
    //        SqlParameter tsParam = new SqlParameter("@ts", SqlDbType.Float, 53);
    //        SqlParameter fsbmParam = new SqlParameter("@fsbm", SqlDbType.VarChar, 4);
    //        SqlParameter zxfsParam = new SqlParameter("@zxfs", SqlDbType.VarChar, 8);
    //        SqlParameter cfhParam = new SqlParameter("@cfh", SqlDbType.Decimal);
    //        SqlParameter hjksParam = new SqlParameter("@hjks", SqlDbType.VarChar, 4);
    //        SqlParameter czksParam = new SqlParameter("@czks", SqlDbType.VarChar, 4);
    //        SqlParameter hjryParam = new SqlParameter("@hjry", SqlDbType.VarChar, 4);
    //        SqlParameter hjrqParam = new SqlParameter("@hjrq", SqlDbType.DateTime);
    //        SqlParameter hlParam = new SqlParameter("@hl", SqlDbType.Float, 53);
    //        SqlParameter zbybsParam = new SqlParameter("@zbybs", SqlDbType.VarChar, 1);
    //        SqlParameter yzzsParam = new SqlParameter("@yzzs", SqlDbType.VarChar, 60);
    //        SqlParameter yzsjParam = new SqlParameter("@yzsj", SqlDbType.VarChar, 60);
    //        SqlParameter tjzsParam = new SqlParameter("@tjzs", SqlDbType.VarChar, 4);
    //        SqlParameter ylspParam = new SqlParameter("@ylsp", SqlDbType.VarChar, 4);
    //        SqlParameter khParam = new SqlParameter("@kh", SqlDbType.VarChar, 10);
    //        SqlParameter zlzzParam = new SqlParameter("@zlzz", SqlDbType.VarChar, 4);
    //        SqlParameter mzhParam = new SqlParameter("@mzh", SqlDbType.VarChar, 12);
    //        SqlParameter bmParam = new SqlParameter("@bm", SqlDbType.VarChar, 8);
    //        SqlParameter dwParam = new SqlParameter("@dw", SqlDbType.VarChar, 10);
    //        SqlParameter jgParam = new SqlParameter("@jg", SqlDbType.Float, 53);
    //        SqlParameter ysbmParam = new SqlParameter("@ysbm", SqlDbType.VarChar, 4);
    //        SqlParameter ksbmParam = new SqlParameter("@ksbm", SqlDbType.VarChar, 4);
    //        SqlParameter yfbmParam = new SqlParameter("@yfbm", SqlDbType.VarChar, 2);
    //        SqlParameter flh3Param = new SqlParameter("@flh3", SqlDbType.VarChar, 2);
    //        SqlParameter xmbmParam = new SqlParameter("@xmbm", SqlDbType.VarChar, 2);
    //        SqlParameter cfslParam = new SqlParameter("@cfsl", SqlDbType.Float, 53);
    //        SqlParameter cffsParam = new SqlParameter("@cffs", SqlDbType.Float, 53);
    //        command.Parameters.Add(jeParam);
    //        command.Parameters.Add(yfylParam);
    //        command.Parameters.Add(zybjParam);
    //        command.Parameters.Add(hldwParam);
    //        command.Parameters.Add(ypylParam);
    //        command.Parameters.Add(csParam);
    //        command.Parameters.Add(tsParam);
    //        command.Parameters.Add(fsbmParam);
    //        command.Parameters.Add(zxfsParam);
    //        command.Parameters.Add(cfhParam);
    //        command.Parameters.Add(hjksParam);
    //        command.Parameters.Add(czksParam);
    //        command.Parameters.Add(hjryParam);
    //        command.Parameters.Add(hjrqParam);
    //        command.Parameters.Add(hlParam);
    //        command.Parameters.Add(zbybsParam);
    //        command.Parameters.Add(yzzsParam);
    //        command.Parameters.Add(yzsjParam);
    //        command.Parameters.Add(tjzsParam);
    //        command.Parameters.Add(ylspParam);
    //        command.Parameters.Add(khParam);
    //        command.Parameters.Add(zlzzParam);
    //        command.Parameters.Add(mzhParam);
    //        command.Parameters.Add(bmParam);
    //        command.Parameters.Add(dwParam);
    //        command.Parameters.Add(jgParam);
    //        command.Parameters.Add(ysbmParam);
    //        command.Parameters.Add(ksbmParam);
    //        command.Parameters.Add(yfbmParam);
    //        command.Parameters.Add(flh3Param);
    //        command.Parameters.Add(xmbmParam);
    //        command.Parameters.Add(cfslParam);
    //        command.Parameters.Add(cffsParam);
    //        //command.Prepare();
    //        #endregion
    //        #region Execute insert statment for each item
    //        foreach (DataRow row in items.Rows)
    //        {
    //            yfylParam.Value = row[EmrConstant.TabPrescription.TimesDaily];
    //            zybjParam.Value = row[EmrConstant.TabPrescription.zybj];
    //            hldwParam.Value = row[EmrConstant.TabPrescription.ContentUnit];
    //            ypylParam.Value = row[EmrConstant.TabPrescription.Quantity];
    //            csParam.Value = row[EmrConstant.TabPrescription.Times];
    //            tsParam.Value = row[EmrConstant.TabPrescription.Days];
    //            fsbmParam.Value = row[EmrConstant.TabPrescription.WayCode];
    //            zxfsParam.Value = row[EmrConstant.TabPrescription.WayText];
    //            cfhParam.Value = num;
    //            hjksParam.Value = row[EmrConstant.TabPrescription.MakeCostDepartmentCode];
    //            czksParam.Value = row[EmrConstant.TabPrescription.TreatDepartmentCode];
    //            hjryParam.Value = row[EmrConstant.TabPrescription.CostMaker];
    //            hjrqParam.Value = DateTime.Now; // row[EmrConstant.TabPrescription.hjrq];
    //            hlParam.Value = row[EmrConstant.TabPrescription.Content];
    //            zbybsParam.Value = row[EmrConstant.TabPrescription.NoBuy];
    //            yzzsParam.Value = row[EmrConstant.TabPrescription.DoctorExhort];
    //            yzsjParam.Value = row[EmrConstant.TabPrescription.yzsj];
    //            tjzsParam.Value = row[EmrConstant.TabPrescription.DoseDirect];
    //            ylspParam.Value = row[EmrConstant.TabPrescription.Ratifier];
    //            khParam.Value = row[EmrConstant.TabPrescription.CardNum];
    //            zlzzParam.Value = zlzz;
    //            mzhParam.Value = row[EmrConstant.TabPrescription.registryID];
    //            bmParam.Value = row[EmrConstant.TabPrescription.DrugCode];
    //            dwParam.Value = row[EmrConstant.TabPrescription.Unit];
    //            jgParam.Value = row[EmrConstant.TabPrescription.Price];
    //            ysbmParam.Value = row[EmrConstant.TabPrescription.DoctorID];
    //            ksbmParam.Value = row[EmrConstant.TabPrescription.DepartmentCode];
    //            yfbmParam.Value = row[EmrConstant.TabPrescription.PharmacyCode];
    //            flh3Param.Value = row[EmrConstant.TabPrescription.InsureGroup];
    //            xmbmParam.Value = row[EmrConstant.TabPrescription.OutCheckGroup];
    //            cfslParam.Value = row[EmrConstant.TabPrescription.Total];
    //            cffsParam.Value = row[EmrConstant.TabPrescription.cffs];
    //            jeParam.Value = Convert.ToDouble(jgParam.Value) *
    //                Convert.ToDouble(cfslParam.Value) * Convert.ToDouble(cffsParam.Value);

    //            command.ExecuteNonQuery();
    //        }
    //        #endregion
    //        #region Set flag of having determined on price
    //        command.CommandText = "SELECT cfdh FROM mz_ghmx WHERE mzh='" + registryID + "'";

    //        reader = command.ExecuteReader();
    //        double cfdh = 1;
    //        if (reader.Read())
    //        {
    //            if (!Convert.IsDBNull(reader[0])) cfdh = Convert.ToDouble(reader[0]) + 1.0;
    //        }
    //        reader.Close();

    //        command.CommandText = "UPDATE mz_ghmx SET hjbs='1',cfdh='" + cfdh + "' WHERE mzh='" +
    //            registryID + "'";
    //        command.ExecuteNonQuery();

    //        #endregion

    //        transaction.Commit();

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        transaction.Rollback();
    //        msg = ex.Message + "--" + ex.Source;
    //        return false;
    //    }
    //    */
    //    return false;
    //}
    //private bool SqlIsEnoughStock(double requiredAmount, string drugCode, string pharmacyCode, SqlConnection conn)
    //{
    //    string select = "SELECT zxys FROM yf_zy WHERE bm='" + drugCode + "' AND yfbm='" + pharmacyCode + "'";
    //    SqlCommand command = new SqlCommand(select, conn);
    //    SqlDataReader reader = command.ExecuteReader();
    //    if (reader.Read())
    //    {
    //        if (Convert.IsDBNull(reader[0])) goto fail;
    //        if (requiredAmount > Convert.ToDouble(reader[0]))
    //        { goto fail; }
    //        else
    //        {
    //            reader.Close();
    //            return true;
    //        }
    //    }
    //fail: reader.Close();
    //    return false;
    //}
    //private decimal SqlNewPrescriptionNum(ref string msg, SqlConnection conn)
    //{
    //    string query = "SELECT dqcfh FROM mz_dqcfh";
    //    try
    //    {
    //        SqlCommand command = new SqlCommand(query, conn);
    //        SqlDataReader reader = command.ExecuteReader();
    //        reader.Read();
    //        decimal num = (decimal)reader[0];
    //        reader.Close();

    //        decimal newNum = num + 1;
    //        if (num == 99999999) newNum = 1;

    //        query = "UPDATE mz_dqcfh SET dqcfh='" + newNum + "'";
    //        command = new SqlCommand(query, conn);
    //        command.ExecuteNonQuery();
    //        return num;
    //    }
    //    catch (SqlException ex)
    //    {
    //        msg = ex.Message + "--" + ex.Source;
    //        return 0;
    //    }
    //}

    private int PassCheck(DataTable itmes)
    {
        /*功能：PASS自动审查、手动审查、用药研究、警告值查询
        参数：
        1：住院医生工作站保存自动审查；
        2：住院医生工作站提交自动审查
        33：门诊医生工作站保存自动审查
        34：门诊医生工作站提交自动审查
        3：手工审查；
        4：临床药学单病人审查
        5：多病人批量审查 （采集）
        7：多病人批量审查（不采集）
        12：用药研究；
        6：警告查询；
        */

        //        int ll_row,ll_rowcount;
        //        int ll_currentrow;
        //        int[] laRow;
        //        string[] laOrderUniqueCode;
        //        int i;

        //        //传处方用药清单
        //        String ls_isdrug;   //医嘱类别
        //        String ls_iscancel ; //是否作废
        //        String ls_isstop;    //是否停嘱
        //        String ls_StartDate;   //开嘱日期
        //        String ls_StopDate;     //停嘱日期
        //        String ls_OrderUniqueCode;   //医嘱唯一码
        //        String ls_DrugCode;    //药品唯一码
        //        String ls_DrugName;   //药品名称
        //        String ls_SingleDose ;  //单次剂量值
        //        String ls_DoseUnit;     //单次剂量单位
        //        String ls_Frequency;    //用药频次
        //        String ls_RouteName;   //给药途径名称
        //        String ls_GroupTag;    //成组医嘱标记
        //        String ls_OrderType;   //医嘱类型（长期0/临时1）
        //        String ls_DoctorCodeAndName; //医生编码/姓名
        //        String ls_Currow_OrderUniqueCode;

        //        int li_Warn, li_maxwarn

        //        //PASS系统不调动或异常，则返回0 
        //        If gi_passenabled = 0 THEN return 0
        //        if PassGetState('0') = 0 then return 0
        //        //ldw = [病人用药清单数据窗口]
        //        ll_rowcount = idw.rowcount()
        //        if ll_rowcount = 0 then return 0

        //        //取当前行号
        //        ll_currentrow =idw.getrow()

        //        /*循环传入病人当天所有用药信息，具体要求如下：
        //            1、如果是门诊处方，则要求处方内的所有用药记录信息（非药品类信息不传）；
        //            2、如果是病区医嘱，则要求传入所有未停长期药品类医嘱和当天开立的临时药品类医嘱。注意以下医嘱不能传入：
        //            （1）、非药品类医嘱不能传入；
        //            （2）、已停长期医嘱不能传入；
        //            （3）、已作废医嘱不能传入；
        //            （4）、非当天开的临时医嘱不能传入；
        //        */

        //I = 0
        //FOR ll_row = 1 TO ll_rowcount

        //    ls_isdrug = '1'
        //    ls_isstop = '0'
        //    ls_iscancel = '0'
        //    //如果当前医嘱为非药品类医嘱或已停或已作废，则不能传入此记录，而应进入下一循环。
        //    IF ls_isdrug <> '1' OR  ls_isstop = '1' OR ls_iscancel = '1' THEN CONTINUE

        //    ls_StartDate = string(idw.object.hjrq[ll_row], 'yyyy-mm-dd')
        //    //[开嘱日期]：字符串类型，传入参数，表示开立医嘱日期。格式为"yyyy-mm-dd"，例如开嘱日期为1999年3月12日，
        //    //            则应传入"1999-03-12"。

        //    if idw.object.cs[ll_row] > 1 or idw.object.ts[ll_row] > 1 then
        //        ls_ordertype = '0'
        //    else
        //        ls_OrderType = '1'
        //    end if
        //    //[医嘱类型]：指是长期医嘱还是临时医嘱，ls_OrderType='0'表示长期医嘱；ls_OrderType='1'，表示临时医嘱；

        //    //ls_StopDate [停嘱日期]：字符串类型，传入参数，表示停嘱日期，格式为"yyyy-mm-dd"，例如开嘱日期为1999年3月12日，
        //    //                        则应传入"1999-03-12"。临嘱停嘱日期等于开嘱日期，未停长期医嘱停嘱日期取当天。
        //    IF ls_OrderType = "1" THEN  //如果为临时医嘱，则停嘱日期等于开嘱日期
        //        ls_StopDate = String(Today(),"yyyy-mm-dd")
        //    ELSE //如果为长期医嘱，则取停嘱日期，如果未停，则停嘱日期为当天
        //        ls_StopDate = String(Today(),"yyyy-mm-dd")
        //    END IF

        //   //如果为非当天用药，则进入下一循环
        //    IF ls_StopDate < String(Today(),"yyyy-mm-dd")  THEN  CONTINUE

        //    ls_OrderUniqueCode = string(ll_row)
        //    //[医嘱唯一码]:字符串类型，传入参数，表示医嘱唯一码，PASS系统将根据此参数来识别和区分传入的各条医嘱记录，
        //    //             审查后HIS系统只能通过此参数来获取PASS审查的结果值。在同一循环传入时，要求各记录的ls_OrderUniqueCodee值
        //    //             必须唯一，例如，可传入记录的行号值。

        //    //将行号和医嘱唯一码保存到动态数组中
        //    I++
        //    laRow[i] = ll_row
        //    laOrderUniqueCode [i] = ls_OrderUniqueCode

        //    ls_DrugCode = idw.object.bm[ll_row]
        //    //[药品唯一码]：字符串类型，传入参数，表示药品唯一码，要求与PASS系统配对时采用的药品唯一码完全一致，否则PASS系统无法
        //    //              识别药品信息。此参数不能为空。

        //    ls_DrugName = idw.object.pm[ll_row]
        //    //[药品名称]：符串类型，传入参数，表示药品名称。

        //    ls_SingleDose = string(idw.object.ypyl[ll_row])
        //    //[单次剂量值]：符串类型，传入参数，表示每次使用剂量的数字部分，传入此参数主要用于PASS对病人每次服用剂量的审查。
        //    //              注意：此处要求是转化为与药品配对剂量单位完全一致单位后的数值。例如药品配对剂量单位为"mg"，而病人的
        //    //              每次服用剂量为"0.5g"，此时就不能传入"0.5"，而应换算为"500mg"后，传入"500"。此参数如果为空，则不能审查
        //    //              剂量。

        //    ls_DoseUnit = idw.object.hldw[ll_row]
        //    //[单次剂量单位]：字符串类型，传入参数，表示每次服用剂量单位，要求与药品配对剂量单位完全一致单位完全一致，否则可能造成
        //    //                剂量审查不正确。
        //    string ls_yfyl
        //    ls_yfyl = idw.object.yfyl[ll_row]
        //    double ldb_cs, ldb_ts
        //    select cs, ts into :ldb_cs, :ldb_ts from yzyyk where pm = :ls_yfyl using xtca;
        //    if isnull(ldb_cs) or ldb_cs < 1 then ldb_cs = 1
        //    if isnull(ldb_ts) or ldb_ts < 1 then ldb_ts = 1
        //    ls_Frequency = string(ldb_cs) + '/' + string(ldb_ts)
        //    //字符串类型，传入参数，表示药品服用频次信息。传入要求：n天m次，传"m/n"，例如：1天3次，传"3/1"；7天2次，传"2/7"。

        //    ls_RouteName = idw.object.zxfs[ll_row]
        //    //[给药途径名称]：字符串类型，传入参数，表示给药途径名称，例如"口服"、"静滴"等。注意，由于PASS系统审查与给药途径关系密切，
        //    //                此参数传入错误，将直接导致审查错误；如果传空，则导致PASS系统不作任何审查。

        //    string ls_zybj
        //    long j
        //    ls_zybj = idw.object.zybj[ll_row]
        //    if isnull(ls_zybj) or trim(ls_zybj) = '' then
        //        ls_GroupTag = string(ll_row)
        //    else
        //        for j = 1 to ll_row
        //            if ls_zybj = idw.object.zybj[j] then
        //                ls_grouptag = string(j)
        //                exit
        //            end if
        //        next
        //    end if
        //    /*[成组医嘱标记]:字符串类型，传入参数，表示成组医嘱标记。主要用于PASS系统进行注射剂体外配伍审查识别注射剂是否配在一起使用，
        //                      在循环传入的医嘱中，如果此参数值相同，则表示是配制在一起用，此种情况下才有可能存在体外配伍问题，反之，
        //                          如果不配在一起用的医嘱，则要求传入的此参数值一定要不相同。例如：传入10条医嘱进行审查，其中第2、3条是
        //                          配在一起用的医嘱，第6、7为另一组配在一起用的医嘱，则循环传入10条医嘱的cGroupTag参数应为：
        //                                        第n条医嘱        传入cGroupTag参数值
        //                                        第1条医嘱                 "1"
        //                                        第2条医嘱                 "2"
        //                                        第3条医嘱                 "2"
        //                                        第4条医嘱                 "4"
        //                                        第5条医嘱                 "5"
        //                                        第6条医嘱                 "6"
        //                                        第7条医嘱                 "6"
        //                                        第8条医嘱                 "8"
        //                                        第9条医嘱                 "9"
        //                                        第10条医嘱                "10"
        //                        由上例可看出，传cGroupTag参数时要求为：配在一起用的医嘱的cGroupTag参数值必须相同（第2、3条医嘱
        //                        的cGroupTag参数值都为"2"；第6、7条医嘱的cGroupTag参数值都为"6"）；没有配在一起用的医嘱的cGroupTag参数值
        //                        必须不相同（第1条医嘱的cGroupTag参数值都为"1"，第2、3条医嘱的cGroupTag参数值都为"2"，第3条医嘱的
        //                        cGroupTag参数值都为"3"，第4条医嘱的cGroupTag参数值都为"4"，第5条医嘱的cGroupTag参数值都为"5"，第6、7条
        //                        医嘱的cGroupTag参数值都为"6"，第8条医嘱的cGroupTag参数值都为"8"，第9条医嘱的cGroupTag参数值都为"9"，
        //                        第10条医嘱的cGroupTag参数值都为"10"）。
        //    */
        //    string ls_ysbm
        //    ls_ysbm = idw.object.ysbm[ll_row]
        //    ls_DoctorCodeAndName = ls_ysbm + '/' + f_get_ysm(ls_ysbm)
        //    //[医生编码/名称]：字符串类型，传入参数，表示医生姓名，主要用于PASS审查结果数据采集与查询统计。格式为"科室编码/名称"，例如"04/王彬"，其中"04"为医生编码。

        //    //传入一条药品信息
        //    PassSetRecipeInfo(ls_OrderUniqueCode, &
        //                            ls_DrugCode, &
        //                            ls_DrugName, &
        //                            ls_SingleDose, &
        //                            ls_DoseUnit, &
        //                            ls_Frequency, &
        //                            ls_StartDate, &
        //                            ls_StopDate, &
        //                            ls_RouteName, &
        //                            ls_GroupTag, &
        //                            ls_OrderType, &
        //                            ls_DoctorCodeAndName)
        //NEXT 

        ////如果为单药警告信息查询时，要求调用PassSetWarnDrug()函数传入当前行对应的医嘱唯一码值。
        //If aitem = 6 then 
        //   ls_Currow_OrderUniqueCode = string(ll_currentrow)
        //    PassSetWarnDrug(string(ls_Currow_OrderUniqueCode))
        //End if

        ////调用审查函数
        //PassDoCommand(aitem)

        //li_maxwarn = 0
        //If  (aitem=1) OR (aitem=2) OR (aitem=33) OR (aitem=34) OR (aitem=3)Then 
        //    //如果为住院（门诊）医生工作站保存、提交自动审查，则返回审查结果值并在HIS界面亮灯。 
        //    For I = 1 TO UPPERBOUND(laRow[]) 
        //        li_warn = PassGetWarn(trim(laOrderUniqueCode [i]))
        //    If li_warn = 3 then li_maxwarn = 3
        //        //给数据窗口警告列赋值
        //    ll_row = laRow[i]
        //    idw.Object.iWarn[ll_row] = li_warn
        ////   	//给警告列赋值后，将状态改为非修改状态，用户可以考虑实际情况是否需要修改状态。
        //    idw.SetItemStatus(ll_row, "iWarn", primary!, NotModified!) 
        //    NEXT
        //END IF
        //RETURN li_maxwarn
        return 0;
    }
    private decimal SqlNewPrescriptionNumEx(ref string msg)
    {
        string query = "SELECT dqcfh FROM mz_dqcfh";
        try
        {
            SqlConnection conn = new SqlConnection(connectString);
            conn.Open();
            SqlCommand command = new SqlCommand(query, conn);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            decimal num = (decimal)reader[0];
            reader.Close();

            decimal newNum = num + 1;
            if (num == 99999999) newNum = 1;

            query = "UPDATE mz_dqcfh SET dqcfh='" + newNum + "'";
            command = new SqlCommand(query, conn);
            command.ExecuteNonQuery();
            conn.Close();
            return num;
        }
        catch (SqlException ex)
        {
            msg = ex.Message + "--" + ex.Source;
            connectString.Clone();
            return 0;
        }
    }

    [WebMethod(Description = "Add mix order")]
    public bool NewOutOrder(DataSet dsOrder, ref XmlNode errors)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml("<OrderNumbers />");

        bool ret = true;
        DataTable regInfo = dsOrder.Tables[DbTables.OutPatientRegInfo];
        DataTable order = null;

        #region Test
        order = dsOrder.Tables[ExamKind.TestExam];
        if (order.Rows.Count > 0)
        {
            XmlElement testOrderNumber = doc.CreateElement(ExamKind.TestExam);
            string msg = NewOutTestExam(order, regInfo, testOrderNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.TestExam);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(testOrderNumber);
            }
        }
        #endregion
        #region Physical
        order = dsOrder.Tables[ExamKind.PhysicalExam];
        if (order.Rows.Count > 0)
        {
            XmlElement physicalOrderNumber = doc.CreateElement(ExamKind.PhysicalExam);
            string msg = NewOutPhysicalExam(order, regInfo, physicalOrderNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.PhysicalExam);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(physicalOrderNumber);
            }
        }
        #endregion
        #region Image
        order = dsOrder.Tables[ExamKind.ImageExam];
        if (order.Rows.Count > 0)
        {
            XmlElement imageOrderNumber = doc.CreateElement(ExamKind.ImageExam);
            string msg = NewOutImageExam(order, regInfo, imageOrderNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.ImageExam);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(imageOrderNumber);
            }
        }
        #endregion
        #region Prescription
        order = dsOrder.Tables[DbTables.OrderItems];
        if (order.Rows.Count > 0)
        {
            XmlElement prescriptionNumber = doc.CreateElement(DbTables.OrderItems);
            string msg = NewOutPrescription(order, regInfo, prescriptionNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.OrderItems);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(prescriptionNumber);
            }
        }
        #endregion
        #region Treat cost
        order = dsOrder.Tables[DbTables.TreatSubitems];
        if (order.Rows.Count > 0)
        {
            XmlElement treatNumber = doc.CreateElement(DbTables.TreatSubitems);
            string msg = NewOutTreatCost(order, regInfo, treatNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.TreatSubitems);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(treatNumber);
            }
        }
        #endregion
        #region Treat order
        order = dsOrder.Tables[DbTables.Treat];
        DataTable orderDetail = dsOrder.Tables[DbTables.TreatOrderDetail];
        if (order.Rows.Count > 0)
        {
            XmlElement treatOrderNumber = doc.CreateElement(DbTables.Treat);
            string msg = NewOutTreatOrder(order, orderDetail, regInfo, treatOrderNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.Treat);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(treatOrderNumber);
            }
        }
        #endregion

        if (ret) errors = doc.DocumentElement.Clone();
        return ret;
    }
    private string SqlNewOutTestExam(DataTable order, DataTable regInfo, XmlElement testOrderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        string msg = null;
        decimal prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
        if (prescriptionNum == 0) return msg;

        #region Create parameters for command
        command.CommandText = "INSERT INTO kd_mx_mz (jybb, kdrq, kdys, kdks, zxks, kdmc, dh, jyzt, jcyq," +
            "jymd, lczd, mzh, cqrq, partid, fzks) VALUES " +
            "(@jybb, @kdrq, @kdys, @kdks, @zxks, @kdmc, @dh, @jyzt, @jcyq, " +
            "@jymd, @lczd, @mzh, @cqrq, @partid, @fzks)";
        SqlParameter sample = new SqlParameter("@jybb", SqlDbType.VarChar, 20);
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter orderDepartment = new SqlParameter("@kdks", SqlDbType.VarChar, 4);
        SqlParameter orderDoctor = new SqlParameter("@kdys", SqlDbType.VarChar, 4);
        SqlParameter doingDepartment = new SqlParameter("@zxks", SqlDbType.VarChar, 4);
        SqlParameter formName = new SqlParameter("@kdmc", SqlDbType.VarChar, 40);
        SqlParameter formCode = new SqlParameter("@dh", SqlDbType.VarChar, 8);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter sampleDate = new SqlParameter("@cqrq", SqlDbType.DateTime);
        SqlParameter partid = new SqlParameter("@partid", SqlDbType.VarChar, 2);
        SqlParameter assistantDepartment = new SqlParameter("@fzks", SqlDbType.VarChar, 4);
        SqlParameter registryID = new SqlParameter("@mzh", SqlDbType.VarChar, 12);

        command.Parameters.Add(sample);
        command.Parameters.Add(orderDate);
        command.Parameters.Add(orderDepartment);
        command.Parameters.Add(orderDoctor);
        command.Parameters.Add(doingDepartment);
        command.Parameters.Add(formName);
        command.Parameters.Add(formCode);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(sampleDate);
        command.Parameters.Add(partid);
        command.Parameters.Add(assistantDepartment);
        command.Parameters.Add(registryID);
        //command.Prepare();
        #endregion
        foreach (DataRow testForm in order.Rows)
        {
            XmlElement orderNumber = testOrderNumber.OwnerDocument.CreateElement(ElementNames.OrderNumber);
            msg = SqlNewOutTestExamForm(testForm, regInfo.Rows[0], command, command2, prescriptionNum
                , orderNumber);
            if (msg != null)
            {
                transaction.Rollback();
                connection.Close();
                return msg;
            }
            else
            {
                testOrderNumber.AppendChild(orderNumber);
            }
        }

        transaction.Commit();
        connection.Close();
        return null;
    }
    private string SqlNewOutTestExamForm(DataRow form, DataRow regInfo, SqlCommand command,
        SqlCommand command2, decimal prescriptionNum, XmlElement orderNumber)
    {
        DateTime now = today();

        #region Get test form number (mbxh)
        command2.CommandText = "SELECT mbxh FROM mz_jyd_mbdy WHERE jydbh = '"
            + form[TabTestExam.FormNum].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidTestFormNum;
        }
        string formNum = reader[0].ToString();
        reader.Close();
        #endregion

        #region Get partid and doing deparment
        command2.CommandText = "SELECT ksbm, partid FROM mbdm WHERE mbxh = '" + formNum + "'";
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidTestItemNum;
        }
        string doingDepartmentCode = reader[0].ToString();
        string partid = reader[1].ToString();
        reader.Close();
        #endregion

        #region Get assistant department code
        command2.CommandText = "SELECT fzks FROM kd_ks_dzb WHERE ksbm = '" + doingDepartmentCode + "'"
            + " AND dh = '" + form[TabTestExam.Class] + "'";
        reader = command2.ExecuteReader();
        string assistantDepartmentCode = "";
        if (reader.Read()) assistantDepartmentCode = reader[0].ToString();
        reader.Close();
        #endregion

        #region Exexute command
        command.Parameters[0].Value = form[TabTestExam.Sample];
        command.Parameters[1].Value = now;
        command.Parameters[2].Value = regInfo[CommonInfoBase.DepartmentCode];
        command.Parameters[3].Value = regInfo[CommonInfoBase.DoctorID];
        command.Parameters[4].Value = doingDepartmentCode;
        command.Parameters[5].Value = form[TabTestExam.FormName];
        command.Parameters[6].Value = form[TabTestExam.Class];
        command.Parameters[7].Value = form[TabTestExam.Urgent];
        command.Parameters[8].Value = form[TabTestExam.Want];
        command.Parameters[9].Value = form[TabTestExam.Goal];
        command.Parameters[10].Value = form[TabTestExam.Diagnose];
        command.Parameters[11].Value = now.AddDays(1);
        command.Parameters[12].Value = partid;
        command.Parameters[13].Value = assistantDepartmentCode;
        command.Parameters[14].Value = regInfo[CommonInfoOut.RegistryID];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }
        #endregion

        #region Get sequence number of this form
        command2.CommandText = "SELECT max(xh) FROM kd_mx_mz WHERE mzh= '"
            + regInfo[CommonInfoOut.RegistryID].ToString() + "'";
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidRegistryID;
        }
        double formSequence = Convert.ToDouble(reader[0]);
        reader.Close();
        #endregion

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + regInfo[CommonInfoBase.DoctorID].ToString() + "'";
        reader = command2.ExecuteReader();
        string teamHeader = regInfo[CommonInfoBase.DoctorID].ToString();
        if (reader.Read())
            teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Create command for insert treat list
        SqlCommand cmd = command2.Clone();
        cmd.CommandText = "INSERT INTO mz_czmx (mzh, cfh, ysbm, ksbm, hjks, hjry, hjrq, czks, kdxh," +
            "kh, zlzz, jg, xmbm, flh3, sl, cs, bm, je) VALUES " +
            "(@mzh, @cfh, @ysbm, @ksbm, @hjks, @hjry, @hjrq, @czks, @kdxh, " +
                "@kh, @zlzz, @jg, @xmbm, @flh3, @sl, @cs, @bm, @je)";
        cmd.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        cmd.Parameters.Add(new SqlParameter("@cfh", SqlDbType.Decimal));
        cmd.Parameters.Add(new SqlParameter("@ysbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@ksbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjry", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjrq", SqlDbType.DateTime));
        cmd.Parameters.Add(new SqlParameter("@czks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@kdxh", SqlDbType.Decimal));

        cmd.Parameters.Add(new SqlParameter("@kh", SqlDbType.VarChar, 10));
        cmd.Parameters.Add(new SqlParameter("@zlzz", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@sl", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@jg", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@xmbm", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@flh3", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@cs", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@bm", SqlDbType.VarChar, 8));
        cmd.Parameters.Add(new SqlParameter("@je", SqlDbType.Float, 53));
        //command.Parameters.Add(registryID);
        #endregion

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = regInfo[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = formSequence;
        cmd.Parameters[9].Value = regInfo[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the list for this test form
        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = '" + formNum + "'";
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex) { return ex.Message; }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion

        #region New addtional test form (mz_jyd)
        cmd.Parameters.Clear();
        cmd.CommandText = "INSERT INTO mz_jyd (mzblh, mzh, jydbh, dykdxh, jyks, sqrq, sjrq, sjys, jybbbh)" +
            "VALUES (@mzblh, @mzh, @jydbh, @dykdxh, @jyks, @sqrq, @sjrq, @sjys, @jybbbh)";
        cmd.Parameters.Add(new SqlParameter("@mzblh", SqlDbType.VarChar, 18));
        cmd.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        cmd.Parameters.Add(new SqlParameter("@jydbh", SqlDbType.Decimal));
        cmd.Parameters.Add(new SqlParameter("@dykdxh", SqlDbType.Decimal));
        cmd.Parameters.Add(new SqlParameter("@jyks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@sqrq", SqlDbType.DateTime));
        cmd.Parameters.Add(new SqlParameter("@sjrq", SqlDbType.DateTime));
        cmd.Parameters.Add(new SqlParameter("@sjys", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@jybbbh", SqlDbType.VarChar, 10));

        cmd.Parameters[0].Value = regInfo[CommonInfoOut.ArchiveNum];
        cmd.Parameters[1].Value = regInfo[CommonInfoOut.RegistryID];
        cmd.Parameters[2].Value = form[TabTestExam.FormNum];
        cmd.Parameters[3].Value = formSequence;
        cmd.Parameters[4].Value = doingDepartmentCode;
        cmd.Parameters[5].Value = today();
        cmd.Parameters[6].Value = today();
        cmd.Parameters[7].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[8].Value = form[TabTestExam.Sample];
        try { cmd.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }
        #endregion

        #region Get sequence number of this form
        command2.CommandText = "SELECT jysqxh FROM mz_jyd WHERE dykdxh=" + formSequence.ToString();
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return ErrorMessage.InvalidTestFormNum;
        }
        double additionalFormSequence = Convert.ToDouble(reader[0]);
        reader.Close();
        #endregion

        #region List for additional test form
        /*
            #region Create the list for additional test form
            command2.CommandText = "SELECT jyxmbm FROM mz_jyd_xmdy WHERE jydbh = '" + 
                form[TabTestExam.FormNum].ToString() + "'";
            reader = command2.ExecuteReader();
            while (reader.Read())
            {
                code.Add(reader[0]);
            }
            reader.Close();
            #endregion

            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO mz_jyd_mx (jysqxh, jyxmbm) VALUES (@jysqxh, @jyxmbm)";
            cmd.Parameters.Add(new SqlParameter(@jysqxh, SqlDbType.Decimal));
            cmd.Parameters.Add(new SqlParameter(@jyxmbm, SqlDbType.VarChar, 8));
            for (int i = 0; i < code.Count; i++)
            {

            }
            */
        #endregion

        orderNumber.SetAttribute(AttributeNames.Num, formSequence.ToString());
        orderNumber.SetAttribute(AttributeNames.Code, additionalFormSequence.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }
    private string NewOutTestExam(DataTable order, DataTable regInfo, XmlElement testOrderNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutTestExam(order, regInfo, testOrderNumber);
        }
        return null;
    }

    private string NewOutPhysicalExam(DataTable order, DataTable regInfo, XmlElement physicalOrderNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutPhysicalExam(order, regInfo, physicalOrderNumber);
        }
        return null;
    }
    private string SqlNewOutPhysicalExam(DataTable order, DataTable regInfo, XmlElement physicalOrderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        string msg = null;
        decimal prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
        if (prescriptionNum == 0) return msg;

        //kdmxmz_rec.kdrq := SYSDATE;
        //kdmxmz_rec.kdys := as_kdys;
        //kdmxmz_rec.kdks := as_kdks;
        //kdmxmz_rec.zxks := s_zxks;
        //kdmxmz_rec.kdmc := as_mbmc;
        //kdmxmz_rec.dh := as_dh;
        //kdmxmz_rec.jyzt := as_jyzt;
        //kdmxmz_rec.jcyq := as_jcyq;
        //kdmxmz_rec.jymd := as_jymd;
        //kdmxmz_rec.lczd := as_lczd;
        //kdmxmz_rec.nxrq := ad_jcrq;
        //kdmxmz_rec.mzh := as_mzh;
        //kdmxmz_rec.cqrq := sysdate + 1;
        //kdmxmz_rec.partid := v_partid;
        //kdmxmz_rec.fzks

        #region Create parameters for command
        command.CommandText = "INSERT INTO kd_mx_mz (kdrq, kdys, kdks, zxks, kdmc, dh, jyzt, jcyq," +
            "jymd, lczd, nxrq, mzh, cqrq, partid, fzks, jybb) VALUES " +
            "(@kdrq, @kdys, @kdks, @zxks, @kdmc, @dh, @jyzt, @jcyq, " +
            "@jymd, @lczd, @nxrq, @mzh, @cqrq, @partid, @fzks, @jybb)";
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter orderDepartment = new SqlParameter("@kdks", SqlDbType.VarChar, 4);
        SqlParameter orderDoctor = new SqlParameter("@kdys", SqlDbType.VarChar, 4);
        SqlParameter doingDepartment = new SqlParameter("@zxks", SqlDbType.VarChar, 4);
        SqlParameter formName = new SqlParameter("@kdmc", SqlDbType.VarChar, 40);
        SqlParameter formCode = new SqlParameter("@dh", SqlDbType.VarChar, 8);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter wantDate = new SqlParameter("@nxrq", SqlDbType.DateTime);
        SqlParameter registryID = new SqlParameter("@mzh", SqlDbType.VarChar, 12);
        SqlParameter sampleDate = new SqlParameter("@cqrq", SqlDbType.DateTime);
        SqlParameter partid = new SqlParameter("@partid", SqlDbType.VarChar, 2);
        SqlParameter assistantDepartment = new SqlParameter("@fzks", SqlDbType.VarChar, 4);
        SqlParameter physicalExam = new SqlParameter("@jybb", SqlDbType.VarChar, 20);
        command.Parameters.Add(wantDate);
        command.Parameters.Add(orderDate);
        command.Parameters.Add(orderDepartment);
        command.Parameters.Add(orderDoctor);
        command.Parameters.Add(doingDepartment);
        command.Parameters.Add(formName);
        command.Parameters.Add(formCode);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(sampleDate);
        command.Parameters.Add(partid);
        command.Parameters.Add(assistantDepartment);
        command.Parameters.Add(registryID);
        command.Parameters.Add(physicalExam);
        //command.Prepare();
        #endregion
        foreach (DataRow physicalForm in order.Rows)
        {
            XmlElement orderNumber = physicalOrderNumber.OwnerDocument.CreateElement(ElementNames.OrderNumber);
            msg = SqlNewOutPhysicalExamForm(physicalForm, regInfo.Rows[0], command, command2, prescriptionNum
                , orderNumber);
            if (msg != null)
            {
                transaction.Rollback();
                connection.Close();
                return msg;
            }
            else
            {
                physicalOrderNumber.AppendChild(orderNumber);
            }
        }

        transaction.Commit();
        connection.Close();
        return null;
    }
    private string SqlNewOutPhysicalExamForm(DataRow form, DataRow regInfo, SqlCommand command,
        SqlCommand command2, decimal prescriptionNum, XmlElement orderNumber)
    {
        DateTime now = today();

        #region Get partid and doing deparment
        command2.CommandText = "SELECT ksbm, partid FROM mbdm WHERE mbxh = " +
            form[TabPhysicalExam.ItemCode].ToString();
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidTestItemNum;
        }
        string doingDepartmentCode = reader[0].ToString();
        string partid = reader[1].ToString();
        reader.Close();
        #endregion

        #region Get assistant department code
        command2.CommandText = "SELECT fzks FROM kd_ks_dzb WHERE ksbm = '" + doingDepartmentCode + "'"
            + " AND dh = '" + form[TabTestExam.Class] + "'";
        reader = command2.ExecuteReader();
        string assistantDepartmentCode = "";
        if (reader.Read()) assistantDepartmentCode = reader[0].ToString();
        reader.Close();
        #endregion

        #region Exexute command
        command.Parameters[0].Value = form[TabPhysicalExam.WantDate];
        command.Parameters[1].Value = now;
        command.Parameters[2].Value = regInfo[CommonInfoBase.DepartmentCode];
        command.Parameters[3].Value = regInfo[CommonInfoBase.DoctorID];
        command.Parameters[4].Value = doingDepartmentCode;
        command.Parameters[5].Value = form[TabPhysicalExam.ItemName];
        command.Parameters[6].Value = form[TabPhysicalExam.Class];
        command.Parameters[7].Value = form[TabPhysicalExam.Urgent];
        command.Parameters[8].Value = form[TabPhysicalExam.Want];
        command.Parameters[9].Value = "";// form[TabPhysicalExam.Goal];
        command.Parameters[10].Value = form[TabPhysicalExam.Diagnose];
        command.Parameters[11].Value = now.AddDays(1);
        command.Parameters[12].Value = partid;
        command.Parameters[13].Value = assistantDepartmentCode;
        command.Parameters[14].Value = regInfo[CommonInfoOut.RegistryID];
        command.Parameters[15].Value = "";// enhance flag for image only
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }
        #endregion

        #region Get sequence number of this form
        command2.CommandText = "SELECT max(xh) FROM kd_mx_mz WHERE mzh= '"
            + regInfo[CommonInfoOut.RegistryID].ToString() + "'";
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidRegistryID;
        }
        double formSequence = Convert.ToDouble(reader[0]);
        reader.Close();
        #endregion

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + regInfo[CommonInfoBase.DoctorID].ToString() + "'";
        reader = command2.ExecuteReader();
        string groupHeader = regInfo[CommonInfoBase.DoctorID].ToString();
        if (reader.Read())
            groupHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Create command for insert treat list
        SqlCommand cmd = command2.Clone();
        cmd.CommandText = "INSERT INTO mz_czmx (mzh, cfh, ysbm, ksbm, hjks, hjry, hjrq, czks, kdxh," +
            "kh, zlzz, jg, xmbm, flh3, sl, cs, bm, je) VALUES " +
            "(@mzh, @cfh, @ysbm, @ksbm, @hjks, @hjry, @hjrq, @czks, @kdxh, " +
                "@kh, @zlzz, @jg, @xmbm, @flh3, @sl, @cs, @bm, @je)";
        cmd.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        cmd.Parameters.Add(new SqlParameter("@cfh", SqlDbType.Decimal));
        cmd.Parameters.Add(new SqlParameter("@ysbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@ksbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjry", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjrq", SqlDbType.DateTime));
        cmd.Parameters.Add(new SqlParameter("@czks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@kdxh", SqlDbType.Decimal));

        cmd.Parameters.Add(new SqlParameter("@kh", SqlDbType.VarChar, 10));
        cmd.Parameters.Add(new SqlParameter("@zlzz", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@sl", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@jg", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@xmbm", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@flh3", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@cs", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@bm", SqlDbType.VarChar, 8));
        cmd.Parameters.Add(new SqlParameter("@je", SqlDbType.Float, 53));
        //command.Parameters.Add(registryID);
        #endregion

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = regInfo[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = formSequence;
        cmd.Parameters[9].Value = regInfo[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = groupHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the list for this test form
        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = " + form[TabPhysicalExam.ItemCode].ToString();
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex) { return ex.Message; }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion


        orderNumber.SetAttribute(AttributeNames.Num, formSequence.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    private string NewOutImageExam(DataTable order, DataTable regInfo, XmlElement imageOrderNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutImageExam(order, regInfo, imageOrderNumber);
        }
        return null;
    }
    private string SqlNewOutImageExam(DataTable order, DataTable regInfo, XmlElement imageOrderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        string msg = null;
        decimal prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
        if (prescriptionNum == 0) return msg;

        #region Create parameters for command
        command.CommandText = "INSERT INTO kd_mx_mz (kdrq, kdys, kdks, zxks, kdmc, dh, jyzt, jcyq," +
            "jymd, lczd, mzh, qtyxxjczd, yxjcfs, cqrq, partid, fzks, jybb) VALUES " +
            "(@kdrq, @kdys, @kdks, @zxks, @kdmc, @dh, @jyzt, @jcyq, " +
            "@jymd, @lczd, @mzh, @qtyxxjczd, @yxjcfs, @cqrq, @partid, @fzks, @jybb)";
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter orderDepartment = new SqlParameter("@kdks", SqlDbType.VarChar, 4);
        SqlParameter orderDoctor = new SqlParameter("@kdys", SqlDbType.VarChar, 4);
        SqlParameter doingDepartment = new SqlParameter("@zxks", SqlDbType.VarChar, 4);
        SqlParameter formName = new SqlParameter("@kdmc", SqlDbType.VarChar, 40);
        SqlParameter classCode = new SqlParameter("@dh", SqlDbType.VarChar, 8);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter registryID = new SqlParameter("@mzh", SqlDbType.VarChar, 12);
        SqlParameter alterImageExamDiagnose = new SqlParameter("@qtyxxjczd", SqlDbType.VarChar, 300);
        SqlParameter examStyle = new SqlParameter("@yxjcfs", SqlDbType.VarChar, 1);
        SqlParameter sampleDate = new SqlParameter("@cqrq", SqlDbType.DateTime);
        SqlParameter partid = new SqlParameter("@partid", SqlDbType.VarChar, 2);
        SqlParameter assistantDepartment = new SqlParameter("@fzks", SqlDbType.VarChar, 4);
        SqlParameter physicalExam = new SqlParameter("@jybb", SqlDbType.VarChar, 20);

        command.Parameters.Add(alterImageExamDiagnose);
        command.Parameters.Add(orderDate);
        command.Parameters.Add(orderDepartment);
        command.Parameters.Add(orderDoctor);
        command.Parameters.Add(doingDepartment);
        command.Parameters.Add(formName);
        command.Parameters.Add(classCode);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(sampleDate);
        command.Parameters.Add(partid);
        command.Parameters.Add(assistantDepartment);
        command.Parameters.Add(registryID);
        command.Parameters.Add(physicalExam);
        command.Parameters.Add(examStyle);
        //command.Prepare();
        #endregion
        foreach (DataRow imageForm in order.Rows)
        {
            XmlElement orderNumber = imageOrderNumber.OwnerDocument.CreateElement(ElementNames.OrderNumber);
            msg = SqlNewOutImageExamForm(imageForm, regInfo.Rows[0], command, command2, prescriptionNum
                , orderNumber);
            if (msg != null)
            {
                transaction.Rollback();
                connection.Close();
                return msg;
            }
            else
            {
                imageOrderNumber.AppendChild(orderNumber);
            }
        }

        transaction.Commit();
        connection.Close();
        return null;
    }
    private string SqlNewOutImageExamForm(DataRow form, DataRow regInfo, SqlCommand command,
        SqlCommand command2, decimal prescriptionNum, XmlElement orderNumber)
    {
        DateTime now = today();


        #region Get form number(mbxh), partid and doing department
        command2.CommandText =
            "SELECT a.mbxh, a.zqmbxh, a.ksbm, b.partid " +
            "FROM kd_yxbwbm a LEFT JOIN mbdm b ON a.mbxh = b.mbxh " +
            "WHERE a.yxbwbm = '" + form[TabImageExam.PartCode].ToString() +
            "' AND a.yxflbm = '" + form[TabImageExam.SubClass].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return ErrorMessage.NoImageCostDefine + "--" + form[TabImageExam.PartCode].ToString();
        }
        string formNum = null;
        if (form[TabImageExam.Enhance].ToString() == StringGeneral.One)
        {
            if (Convert.IsDBNull(reader[1]))
            {
                reader.Close();
                return ErrorMessage.NoImageCostDefine;
            }
            formNum = reader[1].ToString();
        }
        else
        {
            if (Convert.IsDBNull(reader[0]))
            {
                reader.Close();
                return ErrorMessage.NoImageCostDefine;
            }
            formNum = reader[0].ToString();
        }
        string doingDepartmentCode = reader[2].ToString();
        string partid = reader[3].ToString();
        reader.Close();
        #endregion

        #region Get assistant department code
        command2.CommandText = "SELECT fzks FROM kd_ks_dzb WHERE ksbm = '" + doingDepartmentCode + "'"
            + " AND dh = '" + form[TabImageExam.Class] + "'";
        reader = command2.ExecuteReader();
        string assistantDepartmentCode = "";
        if (reader.Read()) assistantDepartmentCode = reader[0].ToString();
        reader.Close();
        #endregion

        #region Exexute command

        command.Parameters[0].Value = form[TabImageExam.AlterDiagnose];
        command.Parameters[1].Value = now;
        command.Parameters[2].Value = regInfo[CommonInfoBase.DepartmentCode];
        command.Parameters[3].Value = regInfo[CommonInfoBase.DoctorID];
        command.Parameters[4].Value = doingDepartmentCode;
        command.Parameters[5].Value = form[TabImageExam.PartName];
        command.Parameters[6].Value = form[TabImageExam.Class];
        command.Parameters[7].Value = form[TabImageExam.Urgent];
        command.Parameters[8].Value = form[TabImageExam.Want];
        command.Parameters[9].Value = "";// form[TabImageExam.Goal];
        command.Parameters[10].Value = form[TabImageExam.Diagnose];
        command.Parameters[11].Value = now.AddDays(1);
        command.Parameters[12].Value = partid;
        command.Parameters[13].Value = assistantDepartmentCode;
        command.Parameters[14].Value = regInfo[CommonInfoOut.RegistryID];
        command.Parameters[15].Value = form[TabImageExam.Enhance]; // enhance flag;
        string subclass = form[TabImageExam.SubClass].ToString();
        command.Parameters[16].Value = subclass.Substring(subclass.Length - 1);
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }
        #endregion

        #region Get sequence number of this form

        command2.CommandText = "SELECT max(xh) FROM kd_mx_mz WHERE mzh= '"
            + regInfo[CommonInfoOut.RegistryID].ToString() + "'";
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return EmrConstant.ErrorMessage.InvalidRegistryID;
        }
        decimal formSequence = Convert.ToDecimal(reader[0]);
        reader.Close();
        #endregion

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + regInfo[CommonInfoBase.DoctorID].ToString() + "'";
        reader = command2.ExecuteReader();
        string teamHeader = regInfo[CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Create command for insert treat list
        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);
        //command.Parameters.Add(registryID);
        #endregion

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = regInfo[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = regInfo[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = regInfo[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = formSequence;
        cmd.Parameters[9].Value = regInfo[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the list for this test form
        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = " + formNum;
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex) { return ex.Message; }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion

        orderNumber.SetAttribute(AttributeNames.Num, formSequence.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    private string NewOutPrescription(DataTable order, DataTable regInfo, XmlElement prescriptionNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutPrescription(order, regInfo, prescriptionNumber, 0M);
        }
        return null;
    }
    private string SqlNewOutPrescription(DataTable order, DataTable regInfo, XmlElement prescriptionNumber, decimal oldPrescriptionNum)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        string msg = null;
        DateTime now = today();

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + regInfo.Rows[0][CommonInfoBase.DoctorID].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        string teamHeader = regInfo.Rows[0][CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Create insert statement
        command.CommandText = "INSERT INTO mz_ypmx (je, yfyl, zybj, hldw, ypyl, cs, ts, fsbm, zxfs, cfh," +
            "hjks, czks, hjry, hjrq, hl, zbybs, yzzs, yzsj, tjzs, ylsp, kh, zlzz, mzh, bm, dw, jg, ysbm, " +
            "ksbm, yfbm, flh3, xmbm, cfsl, cffs) VALUES " +
            "(@je, @yfyl, @zybj, @hldw, @ypyl, @cs, @ts,@fsbm, @zxfs, @cfh," +
            "@hjks, @czks, @hjry, @hjrq, @hl, @zbybs, @yzzs, @yzsj, @tjzs, @ylsp, @kh, @zlzz, " +
            "@mzh, @bm, @dw, @jg, @ysbm, @ksbm, @yfbm, @flh3, @xmbm, @cfsl, @cffs)";
        #endregion

        #region Create Parameters
        SqlParameter jeParam = new SqlParameter("@je", SqlDbType.Float, 53);
        SqlParameter yfylParam = new SqlParameter("@yfyl", SqlDbType.VarChar, 4);
        SqlParameter zybjParam = new SqlParameter("@zybj", SqlDbType.VarChar, 1);
        SqlParameter hldwParam = new SqlParameter("@hldw", SqlDbType.VarChar, 10);
        SqlParameter ypylParam = new SqlParameter("@ypyl", SqlDbType.Float, 53);
        SqlParameter csParam = new SqlParameter("@cs", SqlDbType.Float, 53);
        SqlParameter tsParam = new SqlParameter("@ts", SqlDbType.Float, 53);
        SqlParameter fsbmParam = new SqlParameter("@fsbm", SqlDbType.VarChar, 4);
        SqlParameter zxfsParam = new SqlParameter("@zxfs", SqlDbType.VarChar, 8);
        SqlParameter cfhParam = new SqlParameter("@cfh", SqlDbType.Decimal);
        SqlParameter hjksParam = new SqlParameter("@hjks", SqlDbType.VarChar, 4);
        SqlParameter czksParam = new SqlParameter("@czks", SqlDbType.VarChar, 4);
        SqlParameter hjryParam = new SqlParameter("@hjry", SqlDbType.VarChar, 4);
        SqlParameter hjrqParam = new SqlParameter("@hjrq", SqlDbType.DateTime);
        SqlParameter hlParam = new SqlParameter("@hl", SqlDbType.Float, 53);
        SqlParameter zbybsParam = new SqlParameter("@zbybs", SqlDbType.VarChar, 1);
        SqlParameter yzzsParam = new SqlParameter("@yzzs", SqlDbType.VarChar, 60);
        SqlParameter yzsjParam = new SqlParameter("@yzsj", SqlDbType.VarChar, 60);
        SqlParameter tjzsParam = new SqlParameter("@tjzs", SqlDbType.VarChar, 4);
        SqlParameter ylspParam = new SqlParameter("@ylsp", SqlDbType.VarChar, 4);
        SqlParameter khParam = new SqlParameter("@kh", SqlDbType.VarChar, 10);
        SqlParameter zlzzParam = new SqlParameter("@zlzz", SqlDbType.VarChar, 4);
        SqlParameter mzhParam = new SqlParameter("@mzh", SqlDbType.VarChar, 12);
        SqlParameter bmParam = new SqlParameter("@bm", SqlDbType.VarChar, 8);
        SqlParameter dwParam = new SqlParameter("@dw", SqlDbType.VarChar, 10);
        SqlParameter jgParam = new SqlParameter("@jg", SqlDbType.Float, 53);
        SqlParameter ysbmParam = new SqlParameter("@ysbm", SqlDbType.VarChar, 4);
        SqlParameter ksbmParam = new SqlParameter("@ksbm", SqlDbType.VarChar, 4);
        SqlParameter yfbmParam = new SqlParameter("@yfbm", SqlDbType.VarChar, 2);
        SqlParameter flh3Param = new SqlParameter("@flh3", SqlDbType.VarChar, 2);
        SqlParameter xmbmParam = new SqlParameter("@xmbm", SqlDbType.VarChar, 2);
        SqlParameter cfslParam = new SqlParameter("@cfsl", SqlDbType.Float, 53);
        SqlParameter cffsParam = new SqlParameter("@cffs", SqlDbType.Float, 53);
        command.Parameters.Add(jeParam);
        command.Parameters.Add(yfylParam);
        command.Parameters.Add(zybjParam);
        command.Parameters.Add(hldwParam);
        command.Parameters.Add(ypylParam);
        command.Parameters.Add(csParam);
        command.Parameters.Add(tsParam);
        command.Parameters.Add(fsbmParam);
        command.Parameters.Add(zxfsParam);
        command.Parameters.Add(cfhParam);
        command.Parameters.Add(hjksParam);
        command.Parameters.Add(czksParam);
        command.Parameters.Add(hjryParam);
        command.Parameters.Add(hjrqParam);
        command.Parameters.Add(hlParam);
        command.Parameters.Add(zbybsParam);
        command.Parameters.Add(yzzsParam);
        command.Parameters.Add(yzsjParam);
        command.Parameters.Add(tjzsParam);
        command.Parameters.Add(ylspParam);
        command.Parameters.Add(khParam);
        command.Parameters.Add(zlzzParam);
        command.Parameters.Add(mzhParam);
        command.Parameters.Add(bmParam);
        command.Parameters.Add(dwParam);
        command.Parameters.Add(jgParam);
        command.Parameters.Add(ysbmParam);
        command.Parameters.Add(ksbmParam);
        command.Parameters.Add(yfbmParam);
        command.Parameters.Add(flh3Param);
        command.Parameters.Add(xmbmParam);
        command.Parameters.Add(cfslParam);
        command.Parameters.Add(cffsParam);
        //command.Prepare();
        #endregion

        #region Fill common parameters
        //command.Parameters.Add(jeParam);
        //command.Parameters.Add(yfylParam);
        //command.Parameters.Add(zybjParam);
        //command.Parameters.Add(hldwParam);
        //command.Parameters.Add(ypylParam);
        //command.Parameters.Add(csParam);
        //command.Parameters.Add(tsParam);
        //command.Parameters.Add(fsbmParam);
        //command.Parameters.Add(zxfsParam);
        //command.Parameters[9].Value = prescriptionNum;   //.Add(cfhParam);
        command.Parameters[10].Value = regInfo.Rows[0][CommonInfoBase.DepartmentCode];  //.Add(hjksParam);
        //command.Parameters[11] =   //.Add(czksParam);
        command.Parameters[12].Value = regInfo.Rows[0][CommonInfoBase.DoctorID];  //.Add(hjryParam);
        command.Parameters[13].Value = now;   //.Add(hjrqParam);
        //command.Parameters[14] =   //.Add(hlParam);
        //command.Parameters[15] =   //.Add(zbybsParam);
        //command.Parameters[16] =   //.Add(yzzsParam);
        command.Parameters[17].Value = ""; //.Add(yzsjParam);
        command.Parameters[18].Value = ""; //.Add(tjzsParam);
        //command.Parameters[19] =   //.Add(ylspParam);
        command.Parameters[20].Value = regInfo.Rows[0][CommonInfoBase.CardNum];  //.Add(khParam);
        command.Parameters[21].Value = teamHeader;  //.Add(zlzzParam);
        command.Parameters[22].Value = regInfo.Rows[0][CommonInfoOut.RegistryID];   //.Add(mzhParam);
        //command.Parameters[23] =  //.Add(bmParam);
        //command.Parameters[24] =  //.Add(dwParam);
        //command.Parameters[25] =   //.Add(jgParam);
        command.Parameters[26].Value = regInfo.Rows[0][CommonInfoBase.DoctorID]; //.Add(ysbmParam);
        command.Parameters[27].Value = regInfo.Rows[0][CommonInfoBase.DepartmentCode]; //.Add(ksbmParam);
        //command.Parameters[28] =  //.Add(yfbmParam);
        //command.Parameters[29] =  //.Add(flh3Param);
        //command.Parameters[30] =  //.Add(xmbmParam);
        //command.Parameters[31] =  //.Add(cfslParam);
        //command.Parameters[32] =  //.Add(cffsParam);
        #endregion

        #region Chinese drug
        string exp = TabPrescription.DrugKind + "='" + DrugKind.Chinese + "'";
        DataRow[] drugC = order.Select(exp);
        if (drugC.Length > 0)
        {
            decimal prescriptionNum = 0;
            if (oldPrescriptionNum == 0)
            {
                prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
                if (prescriptionNum == 0) return msg;
            }
            else
            {
                prescriptionNum = oldPrescriptionNum;
            }

            command.Parameters[9].Value = prescriptionNum;   //.Add(cfhParam);
            XmlElement orderNumber = prescriptionNumber.OwnerDocument.CreateElement(ElementNames.OrderNumber);

            foreach (DataRow drug in drugC)
            {
                msg = SqlNewOutPrescriptionDrug(drug, command, command2, orderNumber);
                if (msg != null)
                {
                    transaction.Rollback();
                    connection.Close();
                    return msg;
                }
            }
            orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
            orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
            orderNumber.SetAttribute(AttributeNames.Type, DrugKind.Chinese);
            prescriptionNumber.AppendChild(orderNumber);
        }
        #endregion

        #region West drug
        exp = TabPrescription.DrugKind + "='" + DrugKind.West + "'";
        DataRow[] drugW = order.Select(exp);
        if (drugW.Length > 0)
        {
            decimal prescriptionNum = 0;
            if (oldPrescriptionNum == 0)
            {
                prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
                if (prescriptionNum == 0) return msg;
            }
            else
            {
                prescriptionNum = oldPrescriptionNum;
            }

            command.Parameters[9].Value = prescriptionNum;   //.Add(cfhParam);
            XmlElement orderNumber = prescriptionNumber.OwnerDocument.CreateElement(ElementNames.OrderNumber);

            foreach (DataRow drug in drugW)
            {
                msg = SqlNewOutPrescriptionDrug(drug, command, command2, orderNumber);
                if (msg != null)
                {
                    transaction.Rollback();
                    connection.Close();
                    return msg;
                }
            }
            orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
            orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
            orderNumber.SetAttribute(AttributeNames.Type, DrugKind.West);
            prescriptionNumber.AppendChild(orderNumber);
        }
        #endregion

        transaction.Commit();
        connection.Close();
        return null;
    }
    private string SqlNewOutPrescriptionDrug(DataRow drug, SqlCommand command, SqlCommand command2, XmlElement orderNumber)
    {

        #region Get treat department code
        //command2.CommandText = "SELECT fzks FROM kd_ks_dzb WHERE ksbm = '" + doingDepartmentCode + "'"
        //    + " AND dh = '" + form[TabImageExam.FormCode] + "'";
        //reader = command2.ExecuteReader();
        string treatDepartmentCode = "";
        //if (reader.Read()) assistantDepartmentCode = reader[0].ToString();
        //reader.Close();
        #endregion

        #region Get drug info
        command2.CommandText =
            "SELECT b.flh4, b.flh3, b.fyjg, b.dw1, b.zxjg, b.dw2, b.zhs, b.dw3, b.hss, a.zxys " +
            "FROM yf_zy a INNER JOIN yp_zy b ON a.bm = b.bm " +
            "WHERE a.bm = '" + drug[TabPrescription.DrugCode].ToString() + "' AND a.yfbm='" +
            drug[TabPrescription.PharmacyCode].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return ErrorMessage.InvalidDrugCode + "--" + drug[TabPrescription.DrugCode].ToString();
        }
        #endregion

        #region Calculate total
        double price = 0.0;
        string unit = null;
        double total = 0.0;
        double timesDaily = Convert.ToDouble(drug[TabPrescription.TimesDaily]);
        double days = Convert.ToDouble(drug[TabPrescription.Days]);
        double quantity = Convert.ToDouble(drug[TabPrescription.Quantity]);
        double zhs = Convert.ToDouble(reader[6]);
        double stock = Convert.ToDouble(reader[9]);
        if (zhs == 0) zhs = 1.0;
        double hss = Convert.ToDouble(reader[8]);
        if (hss == 0) hss = 1.0;
        if (Convert.ToInt16(drug[TabPrescription.PriceUnitMode]) == Convert.ToInt16(PriceUnitMode.Sell))
        {
            /* 发药单位 */
            total = quantity * timesDaily * days / zhs / hss;
            price = Convert.ToDouble(reader[2]);
            unit = reader[3].ToString();
        }
        else
        {
            total = quantity * timesDaily * days / zhs;
            price = Convert.ToDouble(reader[4]);
            unit = reader[5].ToString();
        }
        if (drug[TabPrescription.NoBuy].ToString() == StringGeneral.One) total = 0.0;
        total = Math.Ceiling(total);
        if (total > stock)
        {
            reader.Close();
            return ErrorMessage.UselessDrugCode + "--" + drug[TabPrescription.DrugCode].ToString();
        }
        #endregion

        #region Fill parameters
        command.Parameters[0].Value = total * price;      //.Add(jeParam);
        command.Parameters[1].Value = drug[TabPrescription.TimesText];      //.Add(yfylParam);
        command.Parameters[2].Value = drug[TabPrescription.MixFlag];     //.Add(zybjParam);
        command.Parameters[3].Value = reader[7];   //.Add(hldwParam);
        command.Parameters[4].Value = drug[TabPrescription.Quantity];    //.Add(ypylParam);
        command.Parameters[5].Value = drug[TabPrescription.TimesDaily];    //.Add(csParam);
        command.Parameters[6].Value = drug[TabPrescription.Days];    //.Add(tsParam);
        command.Parameters[7].Value = drug[TabPrescription.WayCode];    //.Add(fsbmParam);
        command.Parameters[8].Value = drug[TabPrescription.WayText];    //.Add(zxfsParam);
        command.Parameters[11].Value = treatDepartmentCode;  //.Add(czksParam);
        command.Parameters[14].Value = reader[6];  //.Add(hlParam);
        command.Parameters[15].Value = drug[TabPrescription.NoBuy];  //.Add(zbybsParam);
        command.Parameters[16].Value = drug[TabPrescription.DoctorExhort];  //.Add(yzzsParam);
        command.Parameters[19].Value = drug[TabPrescription.Ratifier];  //.Add(ylspParam);
        command.Parameters[23].Value = drug[TabPrescription.DrugCode]; //.Add(bmParam);
        command.Parameters[24].Value = unit; //.Add(dwParam);
        command.Parameters[25].Value = price;  //.Add(jgParam);
        command.Parameters[28].Value = drug[TabPrescription.PharmacyCode]; //.Add(yfbmParam);
        command.Parameters[29].Value = reader[1]; //.Add(flh3Param);
        command.Parameters[30].Value = reader[0]; //.Add(xmbmParam);
        command.Parameters[31].Value = total; //.Add(cfslParam);
        command.Parameters[32].Value = drug[TabPrescription.CopyCount]; //.Add(cffsParam);

        reader.Close();
        #endregion

        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }


        return null;
    }

    private string NewOutTreatCost(DataTable order, DataTable regInfo, XmlElement feeOrderNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutTreatCost(order, regInfo, feeOrderNumber);
        }
        return null;
    }
    private string SqlNewOutTreatCost(DataTable order, DataTable regInfo, XmlElement orderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        DateTime now = today();
        DataRow info = regInfo.Rows[0];
        string msg = null;

        #region Get new preacription number
        decimal prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
        if (msg != null) return msg;
        #endregion

        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);
        msg = ExecuteCommandForInsertTreatList(cmd, prescriptionNum, now, info, order, orderNumber);
        if (msg != null)
        {
            transaction.Rollback();
            connection.Close();
            return msg;
        }

        transaction.Commit();
        connection.Close();

        orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    private string NewOutTreatOrder(DataTable order, DataTable orderDetail, DataTable regInfo, XmlElement feeOrderNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlNewOutTreatOrder(order, orderDetail, regInfo, feeOrderNumber);
        }
        return null;
    }
    private string SqlNewOutTreatOrder(DataTable order, DataTable orderDetail, DataTable regInfo, XmlElement orderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        DateTime now = today();
        DataRow info = regInfo.Rows[0];
        string msg = null;
        decimal prescriptionNum = 0;

        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);

        command2.CommandText = "INSERT INTO OutTreatOrder VALUES (@prescriptionNum, @registryID, @sequence, @quantity)";
        command2.Parameters.Add("@prescriptionNum", SqlDbType.Decimal);
        command2.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
        command2.Parameters.Add("@sequence", SqlDbType.Decimal);
        command2.Parameters.Add("@quantity", SqlDbType.Float);

        foreach (DataRow orderItem in order.Rows)
        {
            #region Get new preacription number
            prescriptionNum = SqlNewPrescriptionNumEx(ref msg);
            if (msg != null)
            {
                transaction.Rollback();
                connection.Close();
                return msg;
            }
            #endregion

            command2.Parameters[0].Value = prescriptionNum;
            command2.Parameters[1].Value = info[CommonInfoOut.RegistryID];
            command2.Parameters[2].Value = orderItem[EmrConstant.TreatOrder.Code];
            command2.Parameters[3].Value = orderItem[EmrConstant.TreatOrder.Quantity];
            try
            {
                command2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                connection.Close();
                return ex.Message;
            }
            XmlElement prescript = orderNumber.OwnerDocument.CreateElement(ElementNames.PrescriptionNum);
            prescript.InnerText = prescriptionNum.ToString();
            prescript.SetAttribute(AttributeNames.Code, orderItem[EmrConstant.TreatOrder.Code].ToString());
            orderNumber.AppendChild(prescript);

            #region Treat detail for one treat order
            string exp = EmrConstant.TreatOrder.Code + "=" + orderItem[EmrConstant.TreatOrder.Code].ToString();
            DataRow[] items = orderDetail.Select(exp);
            DataTable oneDetail = orderDetail.Clone();
            foreach (DataRow item in items)
            {
                DataRow newItem = oneDetail.NewRow();
                newItem.ItemArray = item.ItemArray;
                oneDetail.Rows.Add(newItem);
            }

            msg = ExecuteCommandForInsertTreatList(cmd, prescriptionNum, now, info, oneDetail, orderNumber);
            if (msg != null)
            {
                transaction.Rollback();
                connection.Close();
                return msg;
            }
            #endregion
        }

        transaction.Commit();
        connection.Close();

        //orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    private void CreateCommandForInsertTreatList(SqlCommand cmd)
    {
        cmd.CommandText = "INSERT INTO mz_czmx (mzh, cfh, ysbm, ksbm, hjks, hjry, hjrq, czks, kdxh," +
            "kh, zlzz, jg, xmbm, flh3, sl, cs, bm, je) VALUES " +
            "(@mzh, @cfh, @ysbm, @ksbm, @hjks, @hjry, @hjrq, @czks, @kdxh, " +
                "@kh, @zlzz, @jg, @xmbm, @flh3, @sl, @cs, @bm, @je)";
        cmd.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        cmd.Parameters.Add(new SqlParameter("@cfh", SqlDbType.Decimal));
        cmd.Parameters.Add(new SqlParameter("@ysbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@ksbm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjry", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@hjrq", SqlDbType.DateTime));
        cmd.Parameters.Add(new SqlParameter("@czks", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@kdxh", SqlDbType.Decimal));

        cmd.Parameters.Add(new SqlParameter("@kh", SqlDbType.VarChar, 10));
        cmd.Parameters.Add(new SqlParameter("@zlzz", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@sl", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@jg", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@xmbm", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@flh3", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@cs", SqlDbType.Float, 53));
        cmd.Parameters.Add(new SqlParameter("@bm", SqlDbType.VarChar, 8));
        cmd.Parameters.Add(new SqlParameter("@je", SqlDbType.Float, 53));
    }
    private string ExecuteCommandForInsertTreatList(SqlCommand cmd, decimal prescriptionNum,
        DateTime now, DataRow info, DataTable order, XmlElement orderNumber)
    {
        SqlCommand command = cmd.Clone();
        command.Parameters.Clear();

        #region Get header of the doctor
        command.CommandText =
            "SELECT zlzz FROM tysm WHERE ysbm = '" + info[CommonInfoBase.DoctorID].ToString() + "'";
        SqlDataReader reader = command.ExecuteReader();
        string teamHeader = info[CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = info[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[8].Value = 0; //form[TabImageExam.FormSequence];
        cmd.Parameters[9].Value = info[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Add items into mz_czmx
        string select = "SELECT jg1, flh4, flh3 FROM tsfxm WHERE bm = '";

        foreach (DataRow item in order.Rows)
        {
            if (item[TabTreatCost.Locked].ToString() == StringGeneral.Yes) continue;
            command.CommandText = select + item[TabTreatCost.Code].ToString() + "'";
            reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return ErrorMessage.InvalidPriceItemCode + "--" + item[TabTreatCost.Code].ToString();
            }

            cmd.Parameters[12].Value = reader[0]; // price[k];
            cmd.Parameters[13].Value = reader[1]; // insuranceGroup[k];
            cmd.Parameters[14].Value = reader[2]; // normalGroup[k];
            cmd.Parameters[15].Value = item[TabTreatCost.Quantity];
            cmd.Parameters[16].Value = item[TabTreatCost.Code];
            cmd.Parameters[17].Value =
                Convert.ToDouble(item[TabTreatCost.Quantity]) * Convert.ToDouble(reader[0]);

            reader.Close();

            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                return ex.Message;
            }

            XmlElement itemPrice = orderNumber.OwnerDocument.CreateElement(ElementNames.Item);
            itemPrice.SetAttribute(AttributeNames.Code, cmd.Parameters[16].Value.ToString());
            itemPrice.SetAttribute(AttributeNames.Price, cmd.Parameters[12].Value.ToString());
            itemPrice.SetAttribute(AttributeNames.Quantity, cmd.Parameters[15].Value.ToString());
            itemPrice.SetAttribute(AttributeNames.Cost, cmd.Parameters[17].Value.ToString());
            orderNumber.AppendChild(itemPrice);
        }
        #endregion

        return null;
    }
    private void MakeErrorMsg(ref XmlNode errors, string msg, string exam)
    {
        if (errors == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Errors xmlns=\"\"/>");
            errors = doc.DocumentElement.Clone();
        }

        XmlElement error = errors.OwnerDocument.CreateElement(EmrConstant.ElementNames.Error);
        error.SetAttribute(EmrConstant.AttributeNames.Type, exam);
        error.InnerText = msg;
        errors.AppendChild(error);
    }

    [WebMethod(Description = "Update mix order")]
    public bool OldOutOrder(DataSet dsOrder, ref XmlNode errors)
    {
        bool ret = true;
        DataTable regInfo = dsOrder.Tables[DbTables.OutPatientRegInfo];
        DataTable order = null;

        #region Test
        order = dsOrder.Tables[ExamKind.TestExam];
        if (order.Rows.Count > 0)
        {
            string msg = OldOutTestExam(order, regInfo); //, testOrderNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.TestExam);
                ret = false;
            }
        }
        #endregion

        #region Physical
        order = dsOrder.Tables[ExamKind.PhysicalExam];
        if (order.Rows.Count > 0)
        {
            string msg = OldOutPhysicalExam(order, regInfo);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.PhysicalExam);
                ret = false;
            }
        }
        #endregion

        #region Prescription
        order = dsOrder.Tables[DbTables.OrderItems];
        if (order.Rows.Count > 0)
        {
            //XmlElement prescriptionNumber = doc.CreateElement(DbTables.OrderItems);
            string msg = OldOutPrescription(order, regInfo);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.OrderItems);
                ret = false;
            }
            //else
            //{
            //    doc.DocumentElement.AppendChild(prescriptionNumber);
            //}
        }
        #endregion

        #region Image
        order = dsOrder.Tables[ExamKind.ImageExam];
        if (order.Rows.Count > 0)
        {
            string msg = OldOutImageExam(order, regInfo);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, ExamKind.ImageExam);
                ret = false;
            }
        }
        #endregion

        #region Treat cost
        order = dsOrder.Tables[DbTables.TreatSubitems];
        if (order.Rows.Count > 0)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<OrderNumbers />");
            XmlElement treatNumber = doc.CreateElement(DbTables.TreatSubitems);
            string msg = OldOutTreatCost(order, regInfo, treatNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.TreatSubitems);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(treatNumber);
            }
            if (ret) errors = doc.DocumentElement.Clone();
        }
        #endregion

        #region Treat order
        order = dsOrder.Tables[DbTables.Treat];
        DataTable orderDetail = dsOrder.Tables[DbTables.TreatOrderDetail];
        if (order.Rows.Count > 0)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<OrderNumbers />");
            XmlElement treatNumber = doc.CreateElement(DbTables.Treat);
            string msg = OldOutTreatOrder(order, orderDetail, regInfo, treatNumber);
            if (msg != null)
            {
                MakeErrorMsg(ref errors, msg, DbTables.Treat);
                ret = false;
            }
            else
            {
                doc.DocumentElement.AppendChild(treatNumber);
            }
            if (ret) errors = doc.DocumentElement.Clone();
        }
        #endregion
        return ret;
    }
    private string OldOutTestExam(DataTable order, DataTable regInfo)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutTestExam(order, regInfo);
        }
        return null;
    }
    private string SqlOldOutTestExam(DataTable order, DataTable regInfo)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        DateTime now = today();
        DataRow form = order.Rows[0];
        DataRow info = regInfo.Rows[0];


        string formSequence = Convert.ToDecimal(form[TabTestExam.FormSequence]).ToString();

        #region Get test form number (mbxh)
        command2.CommandText = "SELECT mbxh FROM mz_jyd_mbdy WHERE jydbh = "
            + form[TabTestExam.FormNum].ToString();
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            connection.Close();
            return ErrorMessage.InvalidTestFormNum + "--" + form[TabTestExam.FormNum].ToString();
        }
        string feeIndex = reader[0].ToString();
        reader.Close();
        #endregion

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + info[CommonInfoBase.DoctorID].ToString() + "'";
        reader = command2.ExecuteReader();
        string teamHeader = info[CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Get old preacription number
        command2.CommandText = "SELECT DISTINCT cfh FROM mz_czmx WHERE kdxh=" +
            form[TabImageExam.FormSequence].ToString();
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            connection.Close();
            return "database error";
        }

        decimal prescriptionNum = Convert.ToDecimal(reader[0]);
        reader.Close();
        #endregion

        #region Get partid and doing deparment
        command2.CommandText = "SELECT ksbm, partid FROM mbdm WHERE mbxh = " + feeIndex;
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            connection.Close();
            return ErrorMessage.InvalidTestItemNum;
        }
        string doingDepartmentCode = reader[0].ToString();
        string partid = reader[1].ToString();
        reader.Close();
        #endregion


        #region Create command parameters for update kd_mx_mz
        command.CommandText = "UPDATE kd_mx_mz SET kdrq = @kdrq, jyzt = @jyzt, jcyq = @jcyq, jymd = @jymd, " +
            "lczd = @lczd, cqrq = @cqrq, jybb = @jybb WHERE xh = " + formSequence;
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter sampleDate = new SqlParameter("@cqrq", SqlDbType.DateTime);
        SqlParameter sample = new SqlParameter("@jybb", SqlDbType.VarChar, 40);

        command.Parameters.Add(orderDate);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(sampleDate);
        command.Parameters.Add(sample);
        #endregion
        #region Exexute command
        command.Parameters[0].Value = now;
        command.Parameters[1].Value = form[TabTestExam.Urgent];
        command.Parameters[2].Value = form[TabTestExam.Want];
        command.Parameters[3].Value = form[TabTestExam.Goal];
        command.Parameters[4].Value = form[TabTestExam.Diagnose];
        command.Parameters[5].Value = now.AddDays(1);
        command.Parameters[6].Value = form[TabTestExam.Sample];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion


        #region update addtional test form (mz_jyd)
        command.Parameters.Clear();
        command.CommandText = "UPDATE mz_jyd SET sqrq = @sqrq, sjrq = @sjrq, jydbh = @jydbh, " +
            "jybbbh = @jybbbh WHERE dykdxh = " + formSequence;
        command.Parameters.Add(new SqlParameter("@sqrq", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@sjrq", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@jybbbh", SqlDbType.VarChar, 10));
        command.Parameters.Add(new SqlParameter("@jydbh", SqlDbType.Decimal));
        command.Parameters[0].Value = now;
        command.Parameters[1].Value = now;
        command.Parameters[2].Value = form[TabTestExam.Sample];
        command.Parameters[3].Value = form[TabTestExam.FormNum];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion

        #region Remove old treal list for this form
        command2.CommandText = "DELETE FROM mz_czmx WHERE kdxh=" + formSequence;
        try { command2.ExecuteNonQuery(); }
        catch (SqlException ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion


        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = info[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = form[TabImageExam.FormSequence];
        cmd.Parameters[9].Value = info[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the subitem list for this test form

        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = " + feeIndex;
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                transaction.Rollback();
                connection.Close();
                return ex.Message;
            }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion

        transaction.Commit();
        connection.Close();
        return null;
    }

    private string OldOutPhysicalExam(DataTable order, DataTable regInfo)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutPhysicalExam(order, regInfo);
        }
        return null;
    }
    private string SqlOldOutPhysicalExam(DataTable order, DataTable regInfo)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;


        DateTime now = today();
        DataRow form = order.Rows[0];
        DataRow info = regInfo.Rows[0];

        string formSequence = form[TabPhysicalExam.FormSequence].ToString();
        string feeIndex = form[TabPhysicalExam.ItemCode].ToString();

        #region Get header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + info[CommonInfoBase.DoctorID].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        string teamHeader = info[CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        #region Get old preacription number
        command2.CommandText = "SELECT DISTINCT cfh FROM mz_czmx WHERE kdxh=" + formSequence;
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            connection.Close();
            return "database error";
        }

        decimal prescriptionNum = Convert.ToDecimal(reader[0]);
        reader.Close();
        #endregion

        #region Get partid and doing deparment
        command2.CommandText = "SELECT ksbm, partid FROM mbdm WHERE mbxh = " + feeIndex;
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            connection.Close();
            return ErrorMessage.InvalidTestItemNum;
        }
        string doingDepartmentCode = reader[0].ToString();
        string partid = reader[1].ToString();
        reader.Close();
        #endregion

        #region Create parameters for command
        command.CommandText = "UPDATE kd_mx_mz SET kdrq = @kdrq, jyzt = @jyzt, jcyq = @jcyq, jymd = @jymd, " +
            "lczd = @lczd, kdys = @kdys, kdks = @kdks, kdmc = @kdmc " +
            "WHERE xh = " + form[TabImageExam.FormSequence].ToString();
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter doctorID = new SqlParameter("@kdys", SqlDbType.VarChar, 4);
        SqlParameter departmentCode = new SqlParameter("@kdks", SqlDbType.VarChar, 4);
        SqlParameter formName = new SqlParameter("@kdmc", SqlDbType.VarChar, 40);
        command.Parameters.Add(orderDate);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(doctorID);
        command.Parameters.Add(departmentCode);
        command.Parameters.Add(formName);
        #endregion
        #region Exexute command to update kd_mx_mz
        command.Parameters[0].Value = now;
        command.Parameters[1].Value = form[TabPhysicalExam.Urgent];
        command.Parameters[2].Value = form[TabPhysicalExam.Want];
        command.Parameters[3].Value = ""; // form[TabTestExam.Goal];
        command.Parameters[4].Value = form[TabPhysicalExam.Diagnose];
        command.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        command.Parameters[6].Value = info[CommonInfoBase.DepartmentCode];
        command.Parameters[7].Value = form[TabPhysicalExam.ItemName];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion


        #region Remove old treal list for this form
        command2.CommandText = "DELETE FROM mz_czmx WHERE kdxh=" + formSequence;
        try { command2.ExecuteNonQuery(); }
        catch (SqlException ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion


        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);

        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = info[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = formSequence;
        cmd.Parameters[9].Value = info[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the subitem list for this test form

        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = " + feeIndex;
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                transaction.Rollback();
                connection.Close();
                return ex.Message;
            }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion
        transaction.Commit();
        connection.Close();
        return null;
    }

    private string OldOutPrescription(DataTable order, DataTable regInfo)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutPrescription(order, regInfo);
        }
        return null;
    }
    private string SqlOldOutPrescription(DataTable order, DataTable regInfo)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete the old
        decimal prescriptionNum = Convert.ToDecimal(order.Rows[0][TabPrescription.PrescriptionNum]);
        string registryID = regInfo.Rows[0][CommonInfoOut.RegistryID].ToString();
        command.CommandText = "DELETE FROM mz_ypmx WHERE cfh = " + prescriptionNum.ToString() +
            " AND mzh = '" + registryID + "'";
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }
        #endregion

        XmlDocument doc = new XmlDocument();
        XmlElement num = doc.CreateElement(ElementNames.OrderNumber);
        string msg = SqlNewOutPrescription(order, regInfo, num, prescriptionNum);
        if (msg != null)
        {
            transaction.Rollback();
            connection.Close();
            return msg;
        }

        transaction.Commit();
        connection.Close();
        return null;
    }

    private string OldOutImageExam(DataTable order, DataTable regInfo)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutImageExam(order, regInfo);
        }
        return null;
    }
    private string SqlOldOutImageExam(DataTable order, DataTable regInfo)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        //string msg = null;
        DataRow form = order.Rows[0];
        DataRow info = regInfo.Rows[0];
        DateTime now = today();

        #region Get charge index (mbxh)
        command2.CommandText =
            "SELECT mbxh, zqmbxh, ksbm " +
            "FROM kd_yxbwbm " +
            "WHERE yxbwbm = '" + form[TabImageExam.PartCode].ToString() +
            "' AND yxflbm = '" + form[TabImageExam.SubClass].ToString() + "'";
        SqlDataReader reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            return ErrorMessage.NoImageCostDefine + "--" + form[TabImageExam.PartCode].ToString();
        }
        string feeIndex = null;
        if (form[TabImageExam.Enhance].ToString() == StringGeneral.One)
        {
            if (Convert.IsDBNull(reader[1]))
            {
                reader.Close();
                return ErrorMessage.NoImageCostDefine;
            }
            feeIndex = reader[1].ToString();
        }
        else
        {
            if (Convert.IsDBNull(reader[0]))
            {
                reader.Close();
                return ErrorMessage.NoImageCostDefine;
            }
            feeIndex = reader[0].ToString();
        }
        string doingDepartmentCode = reader[2].ToString();
        reader.Close();
        #endregion

        #region Create parameters for command
        command.CommandText = "UPDATE kd_mx_mz SET kdrq = @kdrq, jyzt = @jyzt, jcyq = @jcyq, jymd = @jymd, " +
            "lczd = @lczd, kdys = @kdys, kdks = @kdks, kdmc = @kdmc, jybb = @jybb, qtyxxjczd = @qtyxxjczd " +
            "WHERE xh = " + form[TabImageExam.FormSequence].ToString();
        SqlParameter orderDate = new SqlParameter("@kdrq", SqlDbType.DateTime);
        SqlParameter urgent = new SqlParameter("@jyzt", SqlDbType.VarChar, 1);
        SqlParameter orderWant = new SqlParameter("@jcyq", SqlDbType.VarChar, 40);
        SqlParameter orderGoal = new SqlParameter("@jymd", SqlDbType.VarChar, 50);
        SqlParameter diagnose = new SqlParameter("@lczd", SqlDbType.VarChar, 200);
        SqlParameter doctorID = new SqlParameter("@kdys", SqlDbType.VarChar, 4);
        SqlParameter departmentCode = new SqlParameter("@kdks", SqlDbType.VarChar, 4);
        SqlParameter formName = new SqlParameter("@kdmc", SqlDbType.VarChar, 40);
        SqlParameter enhance = new SqlParameter("@jybb", SqlDbType.VarChar, 1);
        SqlParameter alterImageExamDiagnose = new SqlParameter("@qtyxxjczd", SqlDbType.VarChar, 300);
        command.Parameters.Add(orderDate);
        command.Parameters.Add(urgent);
        command.Parameters.Add(orderWant);
        command.Parameters.Add(orderGoal);
        command.Parameters.Add(diagnose);
        command.Parameters.Add(doctorID);
        command.Parameters.Add(departmentCode);
        command.Parameters.Add(formName);
        command.Parameters.Add(enhance);
        command.Parameters.Add(alterImageExamDiagnose);
        #endregion

        #region Exexute command to update kd_mx_mz
        command.Parameters[0].Value = now;
        command.Parameters[1].Value = form[TabImageExam.Urgent];
        command.Parameters[2].Value = form[TabImageExam.Want];
        command.Parameters[3].Value = "";
        command.Parameters[4].Value = form[TabImageExam.Diagnose];
        command.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        command.Parameters[6].Value = info[CommonInfoBase.DepartmentCode];
        command.Parameters[7].Value = form[TabImageExam.PartName];
        command.Parameters[8].Value = form[TabImageExam.Enhance];
        command.Parameters[9].Value = form[TabImageExam.AlterDiagnose];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); return ex.Message; }
        #endregion

        #region Get old preacription number
        command2.CommandText = "SELECT DISTINCT cfh FROM mz_czmx WHERE kdxh=" +
            form[TabImageExam.FormSequence].ToString();
        reader = command2.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            transaction.Rollback();
            return "database error";
        }

        decimal prescriptionNum = Convert.ToDecimal(reader[0]);
        reader.Close();
        #endregion

        #region Remove old treal list for this form
        command2.CommandText = "DELETE FROM mz_czmx WHERE kdxh=" + form[TabImageExam.FormSequence].ToString();
        try { command2.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); return ex.Message; }
        #endregion

        #region Get team header of the doctor
        command2.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = '"
            + info[CommonInfoBase.DoctorID].ToString() + "'";
        reader = command2.ExecuteReader();
        string teamHeader = info[CommonInfoBase.DoctorID].ToString();
        if (reader.Read()) teamHeader = reader[0].ToString();
        reader.Close();
        #endregion

        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);


        #region Fill values into parameters for common data
        cmd.Parameters[0].Value = info[CommonInfoOut.RegistryID];
        cmd.Parameters[1].Value = prescriptionNum;
        cmd.Parameters[2].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[3].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[4].Value = info[CommonInfoBase.DepartmentCode];
        cmd.Parameters[5].Value = info[CommonInfoBase.DoctorID];
        cmd.Parameters[6].Value = now;
        cmd.Parameters[7].Value = doingDepartmentCode;
        cmd.Parameters[8].Value = form[TabImageExam.FormSequence];
        cmd.Parameters[9].Value = info[CommonInfoBase.CardNum];
        cmd.Parameters[10].Value = teamHeader;
        cmd.Parameters[11].Value = 1;
        #endregion

        #region Create the list for this test form
        command2.CommandText = "SELECT a.sfxmbm, a.sl, b.jg1, b.flh4, b.flh3 " +
            "FROM mb_jczl a INNER JOIN tsfxm b ON a.sfxmbm = b.bm " +
            "WHERE mbxh = " + feeIndex;
        reader = command2.ExecuteReader();
        ArrayList price = new ArrayList();
        ArrayList quantity = new ArrayList();
        ArrayList code = new ArrayList();
        ArrayList insuranceGroup = new ArrayList();
        ArrayList normalGroup = new ArrayList();
        while (reader.Read())
        {
            price.Add(reader[2]);
            insuranceGroup.Add(reader[3]);
            normalGroup.Add(reader[4]);
            quantity.Add(reader[1]);
            code.Add(reader[0]);
        }
        reader.Close();

        for (int k = 0; k < code.Count; k++)
        {
            cmd.Parameters[12].Value = price[k];
            cmd.Parameters[13].Value = insuranceGroup[k];
            cmd.Parameters[14].Value = normalGroup[k];
            cmd.Parameters[15].Value = quantity[k];
            cmd.Parameters[16].Value = code[k];
            cmd.Parameters[17].Value = Convert.ToDouble(price[k]) * Convert.ToDouble(quantity[k]);
            try { cmd.ExecuteNonQuery(); }
            catch (SqlException ex) { return ex.Message; }
        }
        price.Clear();
        insuranceGroup.Clear();
        normalGroup.Clear();
        quantity.Clear();
        code.Clear();

        #endregion

        transaction.Commit();
        connection.Close();
        return null;
    }

    private string OldOutTreatCost(DataTable order, DataTable regInfo, XmlElement treatNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutTreatCost(order, regInfo, treatNumber);
        }
        return null;
    }
    private string SqlOldOutTreatCost(DataTable order, DataTable regInfo, XmlElement orderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        DateTime now = today();
        DataRow info = regInfo.Rows[0];
        string msg = null;

        decimal prescriptionNum = Convert.ToDecimal(order.Rows[0][TabTreatCost.PrescriptionNum]);

        foreach (DataRow item in order.Rows)
        {
            if (item[TabTreatCost.Locked].ToString() == StringGeneral.Yes) continue;
            command2.CommandText = "DELETE FROM mz_czmx WHERE cfh = " + prescriptionNum.ToString() +
                " AND bm = '" + item[TabTreatCost.Code].ToString() + "'";
            command2.ExecuteNonQuery();
        }

        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);
        msg = ExecuteCommandForInsertTreatList(cmd, prescriptionNum, now, info, order, orderNumber);
        if (msg != null)
        {
            transaction.Rollback();
            connection.Close();
            return msg;
        }

        transaction.Commit();
        connection.Close();

        orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    private string OldOutTreatOrder(DataTable order, DataTable orderDetail, DataTable regInfo, XmlElement treatNumber)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOldOutTreatCost(order, orderDetail, regInfo, treatNumber);
        }
        return null;
    }
    private string SqlOldOutTreatCost(DataTable order, DataTable orderDetail, DataTable regInfo, XmlElement orderNumber)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command2 = new SqlCommand();
        command2.Connection = connection;
        command2.Transaction = transaction;

        DateTime now = today();
        DataRow info = regInfo.Rows[0];
        string registryID = info[CommonInfoOut.RegistryID].ToString();
        string msg = null;

        decimal prescriptionNum = Convert.ToDecimal(order.Rows[0][TabTreatCost.PrescriptionNum]);
        command2.CommandText = "UPDATE OutTreatOrder SET OrderSequence = @sequence, Quantity = @quantity " +
            "WHERE PrescriptionNum = @prescriptionNum AND RegistryID = @regID";
        command2.Parameters.Add("@sequence", SqlDbType.Decimal);
        command2.Parameters.Add("@quantity", SqlDbType.Float);
        command2.Parameters.Add("@prescriptionNum", SqlDbType.Decimal);
        command2.Parameters.Add("@regID", SqlDbType.VarChar, 12);
        command2.Parameters[0].Value = order.Rows[0][EmrConstant.TreatOrder.Code];
        command2.Parameters[1].Value = order.Rows[0][EmrConstant.TreatOrder.Quantity];
        command2.Parameters[2].Value = prescriptionNum;
        command2.Parameters[3].Value = registryID;
        try
        {
            command2.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }

        command2.CommandText = "DELETE FROM mz_czmx WHERE cfh = @prescriptionNum AND mzh = @regID";
        try
        {
            command2.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }

        command2.Parameters.Clear();
        SqlCommand cmd = command2.Clone();
        CreateCommandForInsertTreatList(cmd);
        msg = ExecuteCommandForInsertTreatList(cmd, prescriptionNum, now, info, orderDetail, orderNumber);
        if (msg != null)
        {
            transaction.Rollback();
            connection.Close();
            return msg;
        }

        transaction.Commit();
        connection.Close();

        orderNumber.SetAttribute(AttributeNames.Num, prescriptionNum.ToString());
        orderNumber.SetAttribute(AttributeNames.DateTime, now.ToString());
        return null;
    }

    [WebMethod(Description = "Remove simple order")]
    public bool RemoveOutOrder(string orderType, string registryID, decimal sequence, ref string error)
    {
        bool ret = true;
        switch (orderType)
        {
            #region Test
            case ExamKind.TestExam:
                error = RemoveOutTestExam(sequence);
                if (error != null) ret = false;
                break;
            #endregion
            #region Image
            case ExamKind.ImageExam:
                error = RemoveOutImageExam(sequence);
                if (error != null) ret = false;
                break;
            #endregion
            #region Physical
            case ExamKind.PhysicalExam:
                error = RemoveOutImageExam(sequence);  // similar to image
                if (error != null) ret = false;
                break;
            #endregion
            #region Drug
            case ExamKind.Prescript:
                error = RemoveOutPrecription(sequence);
                if (error != null) ret = false;
                break;
            #endregion
            #region Fee
            case ExamKind.Fee:
                error = RemoveOutTreatCost(registryID, sequence);
                if (error != null) ret = false;
                break;
            #endregion
            #region Treat
            case ExamKind.Treat:
                error = RemoveOutTreatOrder(registryID, sequence);
                if (error != null) ret = false;
                break;
            #endregion
        }

        return ret;
    }
    private string RemoveOutTestExam(decimal sequence)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRemoveOutTestExam(sequence);
        }
        return null;
    }
    private string SqlRemoveOutTestExam(decimal sequence)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete from kd_mx_mz
        command.CommandText = "DELETE FROM kd_mx_mz WHERE xh = " + sequence.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        #region Delete from mz_jyd
        command.CommandText = "DELETE FROM mz_jyd WHERE dykdxh = " + sequence.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        #region Delete from mz_czmx
        command.CommandText = "DELETE FROM mz_czmx WHERE kdxh = " + sequence.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        command.Transaction.Commit();
        connection.Close();
        return null;
    }
    private string RemoveOutImageExam(decimal sequence)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRemoveOutImageExam(sequence);
        }
        return null;
    }
    private string SqlRemoveOutImageExam(decimal sequence)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete from kd_mx_mz
        command.CommandText = "DELETE FROM kd_mx_mz WHERE xh = " + sequence.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        #region Delete from mz_czmx
        command.CommandText = "DELETE FROM mz_czmx WHERE kdxh = " + sequence.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        command.Transaction.Commit();
        connection.Close();
        return null;
    }
    private string RemoveOutPrecription(decimal prescriptionNum)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRemoveOutPrecription(prescriptionNum);
        }
        return null;
    }
    private string SqlRemoveOutPrecription(decimal prescriptionNum)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete from mz_ypmx
        command.CommandText = "DELETE FROM mz_ypmx WHERE cfh = " + prescriptionNum.ToString();
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        command.Transaction.Commit();
        connection.Close();
        return null;
    }
    private string RemoveOutTreatCost(string registryID, decimal prescriptionNum)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRemoveOutTreatCost(registryID, prescriptionNum);
        }
        return null;
    }
    private string SqlRemoveOutTreatCost(string registryID, decimal prescriptionNum)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete from mz_czmx
        command.CommandText = "DELETE FROM mz_czmx WHERE cfh = " +
            prescriptionNum.ToString() + " AND mzh = '" + registryID + "'";
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion
        #region Delete from OutTreatOrder
        command.CommandText = "DELETE FROM OutTreatOrder WHERE PrescriptionNum = " +
            prescriptionNum.ToString() + " AND RegistryID = '" + registryID + "'";
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        command.Transaction.Commit();
        connection.Close();
        return null;
    }
    private string RemoveOutTreatOrder(string registryID, decimal prescriptionNum)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRemoveOutTreatOrder(registryID, prescriptionNum);
        }
        return null;
    }
    private string SqlRemoveOutTreatOrder(string registryID, decimal prescriptionNum)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Delete from OutTreatOrder
        command.CommandText = "DELETE FROM OutTreatOrder WHERE PrescriptionNum = " +
            prescriptionNum.ToString() + " AND RegistryID = '" + registryID + "'";
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        #region Delete from mz_czmx
        command.CommandText = "DELETE FROM mz_czmx WHERE cfh = " +
            prescriptionNum.ToString() + " AND mzh = '" + registryID + "'";
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            command.Transaction.Rollback();
            connection.Close();
            return ex.Message + "--" + ex.Source;
        }
        #endregion

        command.Transaction.Commit();
        connection.Close();
        return null;
    }
    #endregion

    #region Out patient utilities
    [WebMethod(Description = "Return the pay for treat item")]
    public string PayReturn(string registryID, string doctorID, double total, string billNum)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlPayReturn(registryID, doctorID, total, billNum);
        }
        return null;
    }
    private string SqlPayReturn(string registryID, string doctorID, double total, string billNum)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = "SELECT kh FROM mz_ghmx WHERE mzh='" + registryID + "'";
        SqlDataReader reader = command.ExecuteReader();

        string cardNum = null;
        DateTime now = today();

        if (reader.Read())
        {
            if (!Convert.IsDBNull(reader[0])) cardNum = reader[0].ToString();
        }
        reader.Close();

        if (cardNum != null)
        {
            SqlCommand cmd = command.Clone();
            string msg = SqlPayReturnForCardInfo(registryID, doctorID, cardNum, total, now, cmd);
            if (msg != null)
            {
                transaction.Rollback();
                return msg;
            }
        }

        command.CommandText = "UPDATE mz_czmx SET tczry = @opcode, tczrq = @now " +
            "WHERE pjh = @billNum AND mzh = @registryID";
        command.Parameters.Add(new SqlParameter("@opcode", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@billNum", SqlDbType.VarChar, 16));
        command.Parameters.Add(new SqlParameter("@registryID", SqlDbType.VarChar, 12));
        command.Parameters[0].Value = doctorID;
        command.Parameters[1].Value = now;
        command.Parameters[2].Value = billNum;
        command.Parameters[3].Value = registryID;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); return ex.Message; }

        transaction.Commit();
        return null;
    }
    private string SqlPayReturnForCardInfo(string registryID, string doctorID, string cardNum, double total,
        DateTime now, SqlCommand cmd)
    {
        cmd.CommandText = "UPDATE mz_kjl SET fyhj = fyhj - @total WHERE kh = @cardNum";
        cmd.Parameters.Add(new SqlParameter("@total", SqlDbType.Float));
        cmd.Parameters.Add(new SqlParameter("@cardNum", SqlDbType.VarChar, 10));
        cmd.Parameters[0].Value = total;
        cmd.Parameters[1].Value = cardNum;
        try { cmd.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }

        cmd.Parameters.Clear();
        cmd.CommandText = "INSERT INTO mz_yjmx (kh, mzh, czydm, lx, rq, je) VALUES" +
            "(@kh, @mzh, @czydm, @lx, @rq, @je)";
        cmd.Parameters.Add(new SqlParameter("@je", SqlDbType.Float));
        cmd.Parameters.Add(new SqlParameter("@kh", SqlDbType.VarChar, 10));
        cmd.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        cmd.Parameters.Add(new SqlParameter("@lx", SqlDbType.VarChar, 2));
        cmd.Parameters.Add(new SqlParameter("@czydm", SqlDbType.VarChar, 4));
        cmd.Parameters.Add(new SqlParameter("@rq", SqlDbType.DateTime));
        cmd.Parameters[0].Value = total;
        cmd.Parameters[1].Value = cardNum;
        cmd.Parameters[2].Value = registryID;
        cmd.Parameters[3].Value = "14";
        cmd.Parameters[4].Value = doctorID;
        cmd.Parameters[5].Value = now;

        try { cmd.ExecuteNonQuery(); }
        catch (SqlException ex) { return ex.Message; }

        return null;
    }

    [WebMethod(Description = "Set done flag for out patient")]
    public string FinishOutPatient(string registryID, bool callQueue)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlFinishOutPatient(registryID, callQueue);
        }
        return null;
    }
    private string SqlFinishOutPatient(string registryID, bool callQueue)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        if (callQueue)
        {
            command.CommandText = "UPDATE dz_pdxx SET pdzt = '9' WHERE mzh = '" + registryID + "'";
            try { command.ExecuteNonQuery(); }
            catch (SqlException ex) { connection.Close(); return ex.Message; }
        }

        command.CommandText = "UPDATE mz_ghmx SET zlzt = '9', zbsj = @now WHERE mzh = @mzh";
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters[0].Value = today();
        command.Parameters[1].Value = registryID;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        transaction.Commit();
        connection.Close();
        return null;
    }

    [WebMethod(Description = "recall out patient")]
    public string RecallOutPatient(string opcode, string registryID, string reason, string doctorID)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlRecallOutPatient(opcode, registryID, reason, doctorID);
        }
        return null;
    }
    private string SqlRecallOutPatient(string opcode, string registryID, string reason, string doctorID)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        DateTime now = today();

        command.CommandText = "UPDATE mz_ghmx SET zlzt = '1', zbsj = @now WHERE mzh = @mzh";
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters[0].Value = now;
        command.Parameters[1].Value = registryID;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        command.CommandText = "INSERT INTO mz_zhjl(ysbm, mzh, zhyy, zhsj, ylys) " +
            "VALUES (@ysbm, @mzh, @reason, @now, @ylys)";
        command.Parameters.Clear();
        command.Parameters.Add(new SqlParameter("@ysbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@reason", SqlDbType.VarChar, 200));
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@ylys", SqlDbType.VarChar, 4));
        command.Parameters[0].Value = opcode;
        command.Parameters[1].Value = registryID;
        command.Parameters[2].Value = reason;
        command.Parameters[3].Value = now;
        command.Parameters[4].Value = doctorID;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }


        transaction.Commit();
        connection.Close();
        return null;
    }

    [WebMethod(Description = "Change department for out patient")]
    public string OutTransfer(string registryID, string department, string regType,
        string oldDepartment, bool callQueue)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOutTransfer(registryID, department, regType, oldDepartment, callQueue);
        }
        return null;
    }
    private string SqlOutTransfer(string registryID, string department, string regType,
        string oldDepartment, bool callQueue)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        DateTime now = today();

        #region Get a new transfer number
        command.CommandText = "SELECT MAX(zkh) FROM mz_zkjl WHERE mzh = '" + registryID + "'";
        SqlDataReader reader = command.ExecuteReader();
        string transferNum = "Z1";
        reader.Read();
        if (!Convert.IsDBNull(reader[0]))
        {
            if (regType == "2" && reader[0].ToString() == "Z9")
            {
                reader.Close();
                connection.Close();
                return "error1";
            }
            if (regType != "2" && reader[0].ToString() == "Z3")
            {
                reader.Close();
                connection.Close();
                return "error2";
            }
            int n = Convert.ToInt16(reader[0].ToString().Substring(1)) + 1;
            transferNum = "Z" + n.ToString();
        }
        reader.Close();
        #endregion

        if (callQueue)
        {
            command.CommandText = "UPDATE dz_pdxx SET pdzt = NULL, ksbm = @zrksbm, ysbm = NULL, " +
                "zsbm = NULL, hjsj = NULL, hjcs = 0 WHERE mzh = @mzh";
            command.Parameters.Add(new SqlParameter("@zrksbm", SqlDbType.VarChar, 4));
            command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
            command.Parameters[0].Value = department;
            command.Parameters[1].Value = registryID;
            try { command.ExecuteNonQuery(); }
            catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }
        }

        command.CommandText = "UPDATE mz_ghmx SET zlzt = '3', ksbm = @zrksbm, ysbm = '' WHERE mzh = @mzh";
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        command.CommandText = "INSERT INTO mz_zkjl (mzh, zkh, zkqksbm, ksbm, ysbm, zkrq, jzrq)" +
            "VALUES (@mzh, @zkh, @ksbm, @zrksbm, NULL, @now, NULL)";
        command.Parameters.Clear();
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@zkh", SqlDbType.VarChar, 2));
        command.Parameters.Add(new SqlParameter("@ksbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@zrksbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters[0].Value = registryID;
        command.Parameters[1].Value = transferNum;
        command.Parameters[2].Value = oldDepartment;
        command.Parameters[3].Value = department;
        command.Parameters[4].Value = today();
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        transaction.Commit();
        connection.Close();
        return null;


    }


    [WebMethod(Description = "Remove a out patient from seeing queue")]
    public string UnSeePatient(string registryID, string registryType, bool callQueue)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlUnSeePatient(registryID, registryType, callQueue);
        }
        return null;
    }
    private string SqlUnSeePatient(string registryID, string registryType, bool callQueue)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        #region Has fee?
        command.CommandText = "SELECT COUNT(*) FROM mz_czmx WHERE mzh = '" + registryID + "'";
        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        if (Convert.ToInt32(reader[0]) > 0)
        {
            reader.Close();
            connection.Close();
            return EmrConstant.ErrorMessage.HasFee;
        }
        reader.Close();

        command.CommandText = "SELECT COUNT(*) FROM mz_ypmx WHERE mzh = '" + registryID + "'";
        reader = command.ExecuteReader();
        reader.Read();
        if (Convert.ToInt32(reader[0]) > 0)
        {
            reader.Close();
            connection.Close();
            return EmrConstant.ErrorMessage.HasFee;
        }
        reader.Close();
        #endregion

        command.CommandText = "UPDATE mz_zkjl SET ysbm = NULL, jzrq = NULL " +
            "WHERE mzh = @mzh AND zkh = " +
            "(SELECT MAX(zkh) AS maxzkh FROM  mz_zkjl WHERE mzh = @mzh) ";
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters[0].Value = registryID;
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        if (registryType == "3" || registryType == "4")
        {
            command.CommandText = "UPDATE mz_ghmx  SET zlzt = NULL WHERE mzh = @mzh";
        }
        else
        {
            command.CommandText = "UPDATE mz_ghmx  SET ysbm = NULL, zlzt = NULL WHERE mzh = @mzh";
        }
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        if (callQueue)
        {
            command.CommandText = "UPDATE dz_pdxx SET pdzt = NULL, ysbm = NULL, zsbm = NULL," +
                "hjsj = NULL, hjcs = 0 WHERE mzh = @mzh";
            try { command.ExecuteNonQuery(); }
            catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }
        }


        transaction.Commit();
        connection.Close();
        return null;
    }

    [WebMethod(Description = "Become in patient from out patient")]
    public string BeInpatient(DataSet dsInfo, bool callQueue)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlBeInpatient(dsInfo, callQueue);
        }
        return null;
    }
    private string SqlBeInpatient(DataSet dsInfo, bool callQueue)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;


        DataRow info = dsInfo.Tables[0].Rows[0];


        command.CommandText = "UPDATE mz_ghmx  SET zlzt = '9', zbsj = @now WHERE mzh = @mzh";
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters[0].Value = today();
        command.Parameters[1].Value = info[CommonInfoOut.RegistryID];
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        if (callQueue)
        {
            command.CommandText = "UPDATE dz_pdxx SET pdzt = '9' WHERE mzh = @mzh";
            try { command.ExecuteNonQuery(); }
            catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }
        }

        command.CommandText = "INSERT INTO mz_ysz_zzy " +
            "(mzh, zylb, yfk, ydbq, mzzd, zrysbm, ksbm, zrysdh, mzysbm, mzysks, mzysdh, lxr, lxrdh, rq)" +
            " VALUES (@mzh, @zylb, @yfk, @ydbq, @mzzd, @zrysbm, @ksbm, @zrysdh, @mzysbm, @mzysks," +
            "@mzysdh, @lxr, @lxrdh, @rq)";
        command.Parameters.Clear();
        command.Parameters.Add(new SqlParameter("@mzh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@zylb", SqlDbType.VarChar, 1));
        command.Parameters.Add(new SqlParameter("@yfk", SqlDbType.Float));
        command.Parameters.Add(new SqlParameter("@ydbq", SqlDbType.VarChar, 2));
        command.Parameters.Add(new SqlParameter("@mzzd", SqlDbType.VarChar, 100));
        command.Parameters.Add(new SqlParameter("@zrysbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@ksbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@zrysdh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@mzysbm", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@mzysks", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@mzysdh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@lxr", SqlDbType.VarChar, 20));
        command.Parameters.Add(new SqlParameter("@lxrdh", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@rq", SqlDbType.DateTime));

        command.Parameters[0].Value = info[CommonInfoOut.RegistryID];
        command.Parameters[1].Value = info[TabBeInpatient.InType];
        command.Parameters[2].Value = info[TabBeInpatient.Deposit];
        command.Parameters[3].Value = info[TabBeInpatient.Area];
        command.Parameters[4].Value = info[TabBeInpatient.Diagnose];
        command.Parameters[5].Value = info[TabBeInpatient.ChiefDoctorID];
        command.Parameters[6].Value = info[TabBeInpatient.ChiefDepartmentCode];
        command.Parameters[7].Value = info[TabBeInpatient.ChiefTel];
        command.Parameters[8].Value = info[TabBeInpatient.WorkDoctorID];
        command.Parameters[9].Value = info[TabBeInpatient.WorkDepartmentCode];
        command.Parameters[10].Value = info[TabBeInpatient.WorkTel];
        command.Parameters[11].Value = info[TabBeInpatient.Agent];
        command.Parameters[12].Value = info[TabBeInpatient.AgentTel];
        command.Parameters[13].Value = info[TabBeInpatient.WorkDate];

        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        transaction.Commit();
        connection.Close();
        return null;
    }

    [WebMethod(Description = "Outpatient become in seeing status. ")]
    public string OutpatientBeenSeeing(string regID, string doctorID, string departmentCode,
        RegState rstatus, bool callQueue)
    {
        switch (hisDBType)
        {
            case "ORACLE":
                return null;
            case "MSSQL":
                return SqlOutpatientBeenSeeing(regID, doctorID, departmentCode, rstatus, callQueue);
        }
        return null;
    }
    private string SqlOutpatientBeenSeeing(string regID, string doctorID, string departmentCode, 
        RegState rstatus, bool callQueue)
    {
        SqlConnection connection = new SqlConnection(connectString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        SqlDataReader reader = null;

        #region Transfer
        if (rstatus == RegState.Transfered)
        {
            command.CommandText = "SELECT max(zkh) FROM mz_zkjl" ;
            try { reader = command.ExecuteReader(); }
            catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }
            if (reader.Read())
            {
                command.CommandText = "UPDATE mz_zkjl SET ysbm = @doctor, jzrq = @now " +
                    "WHERE mzh = @regID AND zkh = @transferID";
                command.Parameters.Clear();
                command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
                command.Parameters.Add(new SqlParameter("@doctor", SqlDbType.VarChar, 4));
                command.Parameters.Add(new SqlParameter("@regID", SqlDbType.VarChar, 12));
                command.Parameters.Add(new SqlParameter("@transferID", SqlDbType.VarChar, 2));
                command.Parameters[0].Value = today();
                command.Parameters[1].Value = doctorID;
                command.Parameters[2].Value = regID;
                command.Parameters[3].Value = reader[0];
                reader.Close();
                try { command.ExecuteNonQuery(); }
                catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }
 
            }
        }
        #endregion

        command.CommandText = "SELECT zlzz FROM tysm WHERE ysbm = @doctor";
        command.Parameters.Clear();
        command.Parameters.Add(new SqlParameter("@doctor", SqlDbType.VarChar, 4));
        command.Parameters[0].Value = doctorID;
        try { reader = command.ExecuteReader(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        command.CommandText = "UPDATE mz_ghmx SET ysbm = @doctor, ksbm = @deparment, zlzt = '1', " +
                "jzsj = @now, zlzz = @header WHERE mzh = @regID";
        command.Parameters.Clear();
        command.Parameters.Add(new SqlParameter("@now", SqlDbType.DateTime));
        command.Parameters.Add(new SqlParameter("@doctor", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@regID", SqlDbType.VarChar, 12));
        command.Parameters.Add(new SqlParameter("@header", SqlDbType.VarChar, 4));
        command.Parameters.Add(new SqlParameter("@deparment", SqlDbType.VarChar, 4));
        command.Parameters[0].Value = today();
        command.Parameters[1].Value = doctorID;
        command.Parameters[2].Value = regID;
        command.Parameters[3].Value = doctorID;
        if (reader.Read())
        {
            if (!Convert.IsDBNull(reader[0])) command.Parameters[3].Value = reader[0];
        }
        command.Parameters[4].Value = departmentCode;
        reader.Close();
        try { command.ExecuteNonQuery(); }
        catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        if (callQueue)
        {
            command.CommandText = "UPDATE dz_pdxx SET pdzt = '4' WHERE mzh = @regID";
            try { command.ExecuteNonQuery(); }
            catch (SqlException ex) { transaction.Rollback(); connection.Close(); return ex.Message; }

        }

        transaction.Commit();
        connection.Close();
        return null;
    }



    #endregion

    #region Image examination
    [WebMethod(Description = "Returns types of image examination")]
    public DataSet ImageExamTypes()
    {
        string select = "SELECT pym, yxflbm AS subclass, yxflmc AS name, kdbm AS class FROM kd_yxflbm " +
            "UNION ALL SELECT pym, bm, mc, dh FROM  mz_jcdflbm";
        DataSet dsImageTypes = ExecuteSentence(select, hisDBType);
        dsImageTypes.Tables[0].TableName = EmrConstant.DbTables.ImageExamTypes;
        return dsImageTypes;
    }

    [WebMethod(Description = "Returns types of image examination")]
    public DataSet ImageExamItems()
    {
        string select = "SELECT pym, yxbwbm AS code, yxbwmc AS name, yxflbm AS subclass, mbxh, zqmbxh, " +
            "'10000002' AS class, null AS jydlx, null AS sample FROM kd_yxbwbm " +
            "UNION ALL SELECT c.pym, STR(c.mbxh), c.mbmc, a.bm, c.mbxh, null, a.dh, null AS jydlx, " +
            "null AS sample FROM  mz_jcdflbm a " +
            "INNER JOIN mz_jcdfl_mbdm_dzb b ON a.bm = b.jcdflbm " +
            "INNER JOIN mbdm c ON b.mbxh = c.mbxh WHERE a.dh <> '10000006' " +
            "UNION ALL SELECT a.pym, STR(a.jydbh), a.jydmc, a.jyxmflbm, b.mbxh, null, '10000006' AS class, " +
            "a.jydlx, a.jybbmc FROM mz_jyddy a INNER JOIN mz_jyd_mbdy AS b ON a.jydbh = b.jydbh";

        DataSet dsImageItems = ExecuteSentence(select, hisDBType);
        dsImageItems.Tables[0].TableName = EmrConstant.DbTables.ImageExmaItems;
        return dsImageItems;
    }

    [WebMethod(Description = "Returns styles of image examination")]
    public DataSet ImageExamStyles()
    {
        string select = "SELECT a.yxbwbm, b.yxwzmc " +
            "FROM kd_yxbwwz_dzb a INNER JOIN kd_yxwzbm b ON a.yxwzbm = b.yxwzbm ORDER BY a.yxbwbm";
        DataSet dsImageStyles = ExecuteSentence(select, hisDBType);
        dsImageStyles.Tables[0].TableName = EmrConstant.DbTables.ImageExmaStyles;
        return dsImageStyles;
    }

    [WebMethod(Description = "Returns fee indexes of image examination")]
    public DataSet ImageExamFeeIndexes()
    {
        string select = "SELECT mbxh, zqmbxh, yxflbm, yxbwmc FROM kd_yxbwbm";
        DataSet dsImageFeeIndexes = ExecuteSentence(select, hisDBType);
        dsImageFeeIndexes.Tables[0].TableName = EmrConstant.DbTables.ImageExmaFeeIndexes;
        return dsImageFeeIndexes;
    }

    [WebMethod(Description = "Returns info of image examination")]
    public DataSet ImageExamInfo()
    {
        DataSet dsImageExamInfo = ImageExamTypes();

        //DataSet dsImageExamFeeIndexes = ImageExamFeeIndexes();
        //DataTable imageExamFeeIndexes = dsImageExamFeeIndexes.Tables[0].Copy();
        //dsImageExamInfo.Tables.Add(imageExamFeeIndexes);

        DataSet dsImageExamStyles = ImageExamStyles();
        DataTable imageExamStyles = dsImageExamStyles.Tables[0].Copy();
        dsImageExamInfo.Tables.Add(imageExamStyles);

        DataSet dsImageExamItems = ImageExamItems();
        DataTable imageExamItems = dsImageExamItems.Tables[0].Copy();
        dsImageExamInfo.Tables.Add(imageExamItems);
        return dsImageExamInfo;
    }
    #endregion

    #region Physical examination and test
    [WebMethod(Description = "Returns types of physical examination")]
    public DataSet PhysicalExamTypes()
    {
        string select = "SELECT pym, bm, mc, dh FROM  mz_jcdflbm";
        DataSet dsPhysicalExamTypes = ExecuteSentence(select, hisDBType);
        dsPhysicalExamTypes.Tables[0].TableName = EmrConstant.DbTables.PhysicalExamTypes;
        return dsPhysicalExamTypes;
    }
    [WebMethod(Description = "Returns items of physical examination")]
    public DataSet PhysicalExamItems()
    {
        string select = "SELECT a.jcdflbm, a.mbxh, b.mbmc FROM  mz_jcdfl_mbdm_dzb a " +
            "INNER JOIN mbdm b ON a.mbxh = b.mbxh";
        DataSet dsPhysicalExamItems = ExecuteSentence(select, hisDBType);
        dsPhysicalExamItems.Tables[0].TableName = EmrConstant.DbTables.PhysicalExamItems;
        return dsPhysicalExamItems;
    }
    [WebMethod(Description = "Returns types of test")]
    public DataSet TestTypes()
    {
        string select = "SELECT jyxmfl, jyxmflbm FROM kd_jyxmflbm";
        DataSet dsTestTypes = ExecuteSentence(select, hisDBType);
        dsTestTypes.Tables[0].TableName = EmrConstant.DbTables.TestTypes;
        return dsTestTypes;
    }
    [WebMethod(Description = "Returns items of test")]
    public DataSet TestItems()
    {
        string select = "SELECT jydbh, jydmc, jybbyq, jyxmflbm, jydlx, jybbmc FROM mz_jyddy";
        DataSet dsTestItems = ExecuteSentence(select, hisDBType);
        dsTestItems.Tables[0].TableName = EmrConstant.DbTables.TestItems;
        return dsTestItems;
    }
    [WebMethod(Description = "Returns items of test for departments")]
    public DataSet TestItemsDepartments()
    {
        string select = "SELECT jydbh, ksbm FROM mz_ksjyd";
        DataSet dsTestItemsDepartments = ExecuteSentence(select, hisDBType);
        dsTestItemsDepartments.Tables[0].TableName = EmrConstant.DbTables.TestItemsDepartments;
        return dsTestItemsDepartments;
    }
    [WebMethod(Description = "Returns subitems of test items")]
    public DataSet TestSubitems()
    {
        string select = "SELECT a.jyxmbm, a.jyxmcx, b.jyxmmc, a.jydbh " +
            "FROM mz_jyd_xmdy a INNER JOIN mz_jyxmdy b ON a.jyxmbm=b.jyxmbm";
        DataSet dsTestSubitems = ExecuteSentence(select, hisDBType);
        dsTestSubitems.Tables[0].TableName = EmrConstant.DbTables.TestSubitems;
        return dsTestSubitems;
    }

    [WebMethod(Description = "Returns info of physical examination")]
    public DataSet ExamTestInfo()
    {
        DataSet dsExamTestInfo = PhysicalExamTypes();

        DataSet dsPhysicalExamItems = PhysicalExamItems();
        DataTable physicalExamItems = dsPhysicalExamItems.Tables[0].Copy();
        dsExamTestInfo.Tables.Add(physicalExamItems);

        DataSet dsTestItems = TestItems();
        DataTable testItems = dsTestItems.Tables[0].Copy();
        dsExamTestInfo.Tables.Add(testItems);

        DataSet dsTestItemsDepartments = TestItemsDepartments();
        DataTable testItemsDepartments = dsTestItemsDepartments.Tables[0].Copy();
        dsExamTestInfo.Tables.Add(testItemsDepartments);

        DataSet dsTestTypes = TestTypes();
        DataTable testTypes = dsTestTypes.Tables[0].Copy();
        dsExamTestInfo.Tables.Add(testTypes);

        DataSet dsTestSubitems = TestSubitems();
        DataTable testSubitems = dsTestSubitems.Tables[0].Copy();
        dsExamTestInfo.Tables.Add(testSubitems);
        return dsExamTestInfo;
    }
    #endregion

    #region Teatment
    [WebMethod(Description = "Returns class of treat charge")]
    public DataSet TreatClass()
    {
        string select = "SELECT bm, mc FROM  sfbzbm";
        DataSet dsTreatClass = ExecuteSentence(select, hisDBType);
        dsTreatClass.Tables[0].TableName = EmrConstant.DbTables.TreatClass;
        return dsTreatClass;
    }
    [WebMethod(Description = "Returns subitems of treat order")]
    public DataSet TreatSubitems()
    {
        string select = "SELECT bm, zyzbm, sl FROM  bq_fzyzbm";
        DataSet dsTreatSubitems = ExecuteSentence(select, hisDBType);
        dsTreatSubitems.Tables[0].TableName = EmrConstant.DbTables.TreatSubitems;
        return dsTreatSubitems;
    }
    [WebMethod(Description = "Returns list of treat order")]
    public DataSet TreatOrder()
    {
        string select = "SELECT mbxh, pym, mbmc, ksbm, ysbm, jlsj, ksqx, fzks, mzzybs " +
            "FROM mbdm WHERE mblx = '4'";
        DataSet dsTreatOrder = ExecuteSentence(select, hisDBType);
        dsTreatOrder.Tables[0].TableName = EmrConstant.DbTables.Treat;
        return dsTreatOrder;
    }
    [WebMethod(Description = "Returns list of treat order")]
    public DataSet TreatOrderDetail(decimal orderSequence)
    {
        string select = "SELECT sfxmbm, sl FROM mb_jczl WHERE mbxh = " + orderSequence.ToString();
        return ExecuteSentence(select, hisDBType);
    }
    [WebMethod(Description = "Treatment info")]
    public DataSet TreatInfo()
    {
        DataSet dsTreatInfo = TreatClass();

        dsTreatInfo.Tables.Add(TreatSubitems().Tables[0].Copy());

        dsTreatInfo.Tables.Add(TreatOrder().Tables[0].Copy());

        return dsTreatInfo;
    }
    #endregion

    #region Charge info
    [WebMethod(Description = "Returns fee items of one examination, test or treat")]
    public DataSet ExamTestFeeItems()
    {
        string select = "SELECT mbxh, sfxmbm, sl, dw FROM mb_jczl";
        DataSet dsExamTestFeeItems = ExecuteSentence(select, hisDBType);
        dsExamTestFeeItems.Tables[0].TableName = EmrConstant.DbTables.ExamTestFeeItems;
        return dsExamTestFeeItems;
    }

    [WebMethod(Description = "Returns price list of examination, test, treat or materials")]
    public DataSet ExamTestPriceList(string departmentCode)
    {
        string select = null;
        if (departmentCode == EmrConstant.StringGeneral.NullCode)
            select = "SELECT bxbs, pm, jg1, dw, pym, bm, fyzbs FROM tsfxm ORDER BY pym";
        else
            select = "SELECT a.bxbs,a.pm,a.jg1,a.dw,a.pym,a.bm,a.fyzbs " +
                "FROM tsfxm a INNER JOIN kssfxm b ON a.bm=b.bm " +
                "WHERE b.ksbm='" + departmentCode + "' ORDER BY pym";

        DataSet dsExamTestFeeItems = ExecuteSentence(select, hisDBType);
        dsExamTestFeeItems.Tables[0].TableName = EmrConstant.DbTables.ExamTestPriceList;
        return dsExamTestFeeItems;
    }

    [WebMethod(Description = "Returns info of charge")]
    public DataSet ExamTestChargeInfo(string departmentCode)
    {
        DataSet dsChargeInfo = ExamTestPriceList(departmentCode);

        DataSet dsFeeItems = ExamTestFeeItems();
        DataTable feeItems = dsFeeItems.Tables[0].Copy();
        dsChargeInfo.Tables.Add(feeItems);

        return dsChargeInfo;
    }
    #endregion

    #region OutPatient info
    [WebMethod(Description = "Returns registry id for a patient by card number")]
    public DataSet GetRegistryIDByCardNum(string doctorID, string department, DateTime regDate, string cardNum)
    {
        string select = "SELECT mzh, hzxm FROM mz_ghmx " +
            "WHERE ((mz_ghmx.zlzt is null) OR (mz_ghmx.zlzt = '') OR (mz_ghmx.zlzt in ('1','2','3','4'))) " +
            "AND ((mz_ghmx.thbs is null) OR (mz_ghmx.thbs <> '1')) " +
            "AND ((mz_ghmx.ysbm is null) OR (mz_ghmx.ysbm = '') OR (mz_ghmx.ysbm = '" + doctorID + "') " +
            "AND (mz_ghmx.ghrq >= '" + regDate.ToString() + "') AND ( mz_ghmx.kh = '" + cardNum + "')";
        if (department != EmrConstant.StringGeneral.NullCode)
            select += " AND (mz_ghmx.ksbm = '" + department + "')";
        DataSet dsRegistryIDs = ExecuteSentence(select, hisDBType);
        dsRegistryIDs.Tables[0].TableName = EmrConstant.DbTables.OutRegistryID;
        if (dsRegistryIDs.Tables[0].Rows.Count == 1)
        {
            string registryID = dsRegistryIDs.Tables[0].Rows[0]["mzh"].ToString();
            DataSet dsRegInfo = RegistryInfo(registryID);
            DataTable regInfo = dsRegInfo.Tables[0].Copy();
            dsRegistryIDs.Tables.Add(regInfo);
        }
        return dsRegistryIDs;
    }


    [WebMethod(Description = "Returns clinical info of a patient by short registryID")]
    public DataSet GetClinicalInfo(string registryID, string department, double limitInDays)
    {
        string regID = EmrConstant.StringGeneral.NullCode;
        string maxregid = "SELECT MAX(mzh) AS maxmzh FROM mz_ghmx " +
            "WHERE mzh LIKE '%" + registryID + "'" + " AND (mz_ghmx.thbs is null OR mz_ghmx.thbs <> '1')";
        if (department != StringGeneral.NullCode)
            maxregid += " AND (mz_ghmx.ksbm = '" + department + "')";
        DataSet dstmp = ExecuteSentence(maxregid, hisDBType);
        if (dstmp.Tables[0].Rows.Count == 1) regID = dstmp.Tables[0].Rows[0]["maxmzh"].ToString();

        DataSet info = ClinicalInfo(regID);
        if (info.Tables[0].Rows.Count == 0) return info;

        if (Convert.IsDBNull(info.Tables[0].Rows[0]["zlzt"]))
        {
            info.Tables[0].Rows[0]["zlzt"] = RegState.Normal.ToString("d");
            CheckExpired(info, limitInDays);
        }
        else
        {
            RegState regStatus = (RegState)Convert.ToInt32(info.Tables[0].Rows[0]["zlzt"]);
            if (regStatus != RegState.Finished) CheckExpired(info, limitInDays);
        }
        return info;
    }
    private DataSet RegistryInfo(string regID)
    {
        //TimeSpan limitDays = TimeSpan.FromDays(limitInDays);
        //DateTime dayStart = DateTime.Now.Subtract(limitDays);
        string select = "SELECT a.mzh, a.hzxm, a.hzxb, a.hznl, a.hzlx," +
            "a.icno, a.ksbm,a.hbbm, a.ghrq, a.ysbm," +
            "a.fzbs, a.zlzt, a.kh, b.hbmc, b.yxts, " +
            "'' AS CardType, '0' AS CanBuyDrug, '0' AS PoorPatient, a.nldw " +
            "FROM mz_ghmx a INNER JOIN mz_hbbm b ON a.hbbm = b.hbbm " +
            "WHERE a.mzh = '" + regID + "'";
        //(mz_ghmx.zlzt is null OR mz_ghmx.zlzt = '' OR mz_ghmx.zlzt in ('1','2','3','4')) " +
        //"AND (mz_ghmx.thbs is null OR mz_ghmx.thbs <> '1') " +
        //"AND (mz_ghmx.ysbm is null OR mz_ghmx.ysbm = '' OR mz_ghmx.ysbm = '" + doctorID + "') " +
        //"AND (mz_ghmx.ghrq >= '" + dayStart.ToString() + "') AND ( mz_ghmx.mzh LIKE '%" + registryID + "')";


        DataSet dsRegistryInfo = ExecuteSentence(select, hisDBType);
        dsRegistryInfo.Tables[0].TableName = DbTables.OutPatientRegInfo;
        if (dsRegistryInfo.Tables[0].Rows.Count == 0) return dsRegistryInfo;

        if (!Convert.IsDBNull(dsRegistryInfo.Tables[0].Rows[0]["kh"]))
        {
            string cardNum = dsRegistryInfo.Tables[0].Rows[0]["kh"].ToString();
            DataSet dsCardType = GetCardTypeByCardNum(cardNum);
            if (dsCardType.Tables[0].Rows.Count == 1)
            {
                dsRegistryInfo.Tables[0].Rows[0]["CardType"] = dsCardType.Tables[0].Rows[0]["klx"];
                dsRegistryInfo.Tables[0].Rows[0]["CanBuyDrug"] = dsCardType.Tables[0].Rows[0]["kfgy"];
            }
        }
        //string regID = dsClinicalInfo.Tables[0].Rows[0]["mzh"].ToString();
        DataSet dsDeposit = GetDeposit(regID);
        if (dsDeposit.Tables[0].Rows.Count == 1) dsRegistryInfo.Tables[0].Rows[0]["PoorPatient"] = "1";

        return dsRegistryInfo;

    }
    private DataSet ClinicalInfo(string regID)
    {
        DataSet dsClinicalInfo = RegistryInfo(regID);
        if (dsClinicalInfo.Tables[0].Rows.Count == 0) return dsClinicalInfo;

        DataSet dsDrugs = GetPrescriptionForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsDrugs.Tables[0].Copy());

        DataSet dsPrescriptionNums = GetPrescriptionNumsForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsPrescriptionNums.Tables[0].Copy());

        DataSet dsTestNums = GetTestRequisitionNumsForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsTestNums.Tables[0].Copy());

        DataSet dsPhysicalNums = GetPhysicalRequisitionNumsForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsPhysicalNums.Tables[0].Copy());

        DataSet dsImageNums = GetImageRequisitionNumsForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsImageNums.Tables[0].Copy());

        DataSet dsTreatCostPrescriptionNums = GetTreatCostPrescriptionNumsForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsTreatCostPrescriptionNums.Tables[0].Copy());

        DataSet dsTreatCost = GetTreatCostForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsTreatCost.Tables[0].Copy());

        DataSet dsTreatOrder = GetTreatOrdersForOutPatient(regID);
        dsClinicalInfo.Tables.Add(dsTreatOrder.Tables[0].Copy());
        return dsClinicalInfo;

    }
    private void CheckExpired(DataSet clinicalInfo, double limitInDays)
    {
        object limit = clinicalInfo.Tables[0].Rows[0]["yxts"];
        object registryDate = clinicalInfo.Tables[0].Rows[0]["ghrq"];
        double days = limitInDays;
        if (!Convert.IsDBNull(limit)) days = Convert.ToDouble(limit);
        TimeSpan expiration = TimeSpan.FromDays(days);
        DateTime regDate = Convert.ToDateTime(registryDate);
        DateTime limitDate = today().Subtract(expiration);
        if (regDate < limitDate)
        {
            clinicalInfo.Tables[0].Rows[0]["zlzt"] = RegState.Expired.ToString("d");
        }
    }
    private DataSet GetPrescriptionNumsForOutPatient(string registryID)
    {
        string select = "SELECT DISTINCT cfh FROM mz_ypmx WHERE mzh = '" + registryID + "' ";
        DataSet dsPrescript = ExecuteSentence(select, hisDBType);
        dsPrescript.Tables[0].TableName = EmrConstant.DbTables.OutPrescriptionNums;
        return dsPrescript;
    }
    private DataSet GetTreatCostPrescriptionNumsForOutPatient(string registryID)
    {
        string select = "SELECT DISTINCT cfh FROM mz_czmx WHERE kdxh = 0 AND mzh = '" + registryID + "' ";
        DataSet dsPrescript = ExecuteSentence(select, hisDBType);
        dsPrescript.Tables[0].TableName = DbTables.OutTreatCostPrescriptionNums;
        return dsPrescript;
    }
    private DataSet GetTreatCostForOutPatient(string registryID)
    {
        string select = "SELECT cfh, bm, jg, sl, je, hjry, hjks, sfbs, tfbs " +
            "FROM mz_czmx WHERE kdxh = 0 AND mzh = '" + registryID + "' ";
        DataSet dsPrescript = ExecuteSentence(select, hisDBType);
        dsPrescript.Tables[0].TableName = DbTables.OutTreatCost;
        return dsPrescript;
    }
    private DataSet GetTestResultsForOutPatient(string registryID)
    {
        string select = "SELECT jysqxh FROM mz_jyd WHERE mzh = '" + registryID + "' ";
        DataSet dsTest = ExecuteSentence(select, hisDBType);
        dsTest.Tables[0].TableName = EmrConstant.DbTables.OutTestRequisitionNums;
        return dsTest;
    }
    private DataSet GetTreatOrdersForOutPatient(string registryID)
    {
        string select = "SELECT a.PrescriptionNum, a.OrderSequence, a.quantity, '0' AS tfbs, '0' AS sfbs, " +
            "'' AS hjry, '' AS hjks, b.mbmc " +
            "FROM OutTreatOrder a INNER JOIN mbdm b ON a.OrderSequence = b.mbxh " +
            "WHERE RegistryID = '" + registryID + "'";
        DataSet dsTreatOrder = ExecuteSentence(select, hisDBType);
        dsTreatOrder.Tables[0].TableName = DbTables.OutTreat;
        if (dsTreatOrder.Tables[0].Rows.Count == 0) return dsTreatOrder;

        foreach (DataRow item in dsTreatOrder.Tables[0].Rows)
        {
            select = "SELECT tfbs, sfbs, hjry, hjks FROM mz_czmx WHERE mzh = '" +
                registryID + "' AND cfh = " + item[TabTreatOrder.PrescriptionNum].ToString();
            DataSet detail = ExecuteSentence(select, hisDBType);
            if (detail.Tables[0].Rows.Count == 0) continue;
            if (HasChargedItem(detail.Tables[0])) item[TabTreatOrder.Charged] = StringGeneral.One;
            if (HasUnchargedItem(detail.Tables[0])) item[TabTreatOrder.UnCharged] = StringGeneral.One;
            item[TabTreatOrder.Opcode] = detail.Tables[0].Rows[0][2];
            item[TabTreatOrder.OpDepartmentCode] = detail.Tables[0].Rows[0][3];

        }
        return dsTreatOrder;
    }
    private bool HasChargedItem(DataTable detail)
    {
        foreach (DataRow oneDetail in detail.Rows)
        {
            if (!Convert.IsDBNull(oneDetail[1]) && oneDetail[1].ToString() == StringGeneral.One) return true;
        }
        return false;
    }
    private bool HasUnchargedItem(DataTable detail)
    {
        foreach (DataRow oneDetail in detail.Rows)
        {
            if (!Convert.IsDBNull(oneDetail[0]) && oneDetail[0].ToString() == StringGeneral.One) return true;
        }
        return false;
    }
    //private DataSet GetPhysicalResultsForOutPatient(string registryID)
    //{
    //    string select = "SELECT jysqxh FROM mz_jyd WHERE mzh = '" + registryID + "' ";
    //    DataSet dsTest = ExecuteSentence(select, hisDBType);
    //    dsTest.Tables[0].TableName = EmrConstant.DbTables.OutTestRequisitionNums;
    //    return dsTest;
    //}


    [WebMethod(Description = "Returns old Prescription by registryID")]
    public DataSet GetPrescriptionForOutPatient(string registryID)
    {
        string select = SelectPrescription();
        select += "WHERE a.mzh = '" + registryID + "' ORDER BY a.cfh asc, a.ypxh asc";
        DataSet dsDrugs = ExecuteSentence(select, hisDBType);
        dsDrugs.Tables[0].TableName = EmrConstant.DbTables.OutPrescription;

        foreach (DataRow row in dsDrugs.Tables[0].Rows)
        {
            if (Convert.IsDBNull(row[TabPrescription.Charged]))
                row[TabPrescription.Charged] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.UnCharged]))
                row[TabPrescription.UnCharged] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.Done]))
                row[TabPrescription.Done] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.Aneathsia]))
                row[TabPrescription.Aneathsia] = StringGeneral.Zero;
        }

        return dsDrugs;
    }
    [WebMethod(Description = "Returns old rescription by registryID and prescription number ")]
    public DataSet GetPrescriptionForOneNumber(string registryID, decimal prescriptionNum)
    {
        string select = SelectPrescription();
        select += "WHERE a.mzh = '" + registryID + "' AND a.cfh = " + prescriptionNum.ToString() +
            " ORDER BY a.cfh asc, a.ypxh asc";
        DataSet dsDrugs = ExecuteSentence(select, hisDBType);
        dsDrugs.Tables[0].TableName = EmrConstant.DbTables.OutPrescription;

        foreach (DataRow row in dsDrugs.Tables[0].Rows)
        {
            if (Convert.IsDBNull(row[TabPrescription.Charged]))
                row[TabPrescription.Charged] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.UnCharged]))
                row[TabPrescription.UnCharged] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.Done]))
                row[TabPrescription.Done] = StringGeneral.Zero;
            if (Convert.IsDBNull(row[TabPrescription.Aneathsia]))
                row[TabPrescription.Aneathsia] = StringGeneral.Zero;
        }

        return dsDrugs;
    }
    private string SelectPrescription()
    {
        string select = "SELECT a.ypxh, a.mzh, a.bm, a.dw, a.jg, a.ysbm," +
                "a.ksbm, a.yfbm, a.flh3,a.xmbm, a.cfsl," +
                "a.cffs, a.je, a.cfh, a.hjrq, a.sfbs, a.ylsp," +
                "a.yfyl, a.zybj, a.hl AS zhs, a.hldw, a.ypyl, a.cs, a.ts," +
                "a.fsbm, a.zxfs, a.zbybs, a.yzzs," +
                "a.fybs, a.sfbs, a.tfbs, a.dmbs, b.pm, b.dw1, b.dw2, c.gsdl " +
                "FROM mz_ypmx a " +
                "INNER JOIN yp_zy b ON a.bm = b.bm INNER JOIN ypdlbm c ON b.dl = c.dl ";
        return select;
    }
    private DataSet GetTestRequisitionNumsForOutPatient(string registryID)
    {
        string select = "SELECT a.jysqxh, a.jydbh, a.sqrq, a.jyrq, a.jyry, b.xh, b.kdys, b.kdks, b.kdmc," +
            "b.dh, b.lczd, b.jymd, b.jybb, b.jyzt, b.jcyq, b.zxqk, '0' AS charged " +
            "FROM mz_jyd a INNER JOIN kd_mx_mz b ON a.dykdxh = b.xh " +
            "WHERE b.zfbs is null AND b.mzh = '" + registryID + "' ";
        DataSet dsTest = ExecuteSentence(select, hisDBType);
        dsTest.Tables[0].TableName = EmrConstant.DbTables.OutTestRequisitionNums;
        for (int k = 0; k < dsTest.Tables[0].Rows.Count; k++)
        {
            if (Convert.IsDBNull(dsTest.Tables[0].Rows[k][TabTestExam.Done]))
                dsTest.Tables[0].Rows[k][TabTestExam.Done] = StringGeneral.Zero;
            if (dsTest.Tables[0].Rows[k][TabTestExam.Done].ToString() == StringGeneral.Zero)
            {
                if (IsChargedForm(dsTest.Tables[0].Rows[k][TabTestExam.FormSequence].ToString()))
                    dsTest.Tables[0].Rows[k][TabTestExam.Charged] = StringGeneral.One;
                else
                    dsTest.Tables[0].Rows[k][TabTestExam.Charged] = StringGeneral.Zero;
            }
        }
        return dsTest;
    }
    private DataSet GetPhysicalRequisitionNumsForOutPatient(string registryID)
    {
        string select = "SELECT xh, kdys, kdks, kdmc, dh, lczd, jymd, jyzt, jcyq, kdrq, zxqk,'0' AS charged " +
            "FROM kd_mx_mz " +
            "WHERE zfbs is null AND mzh = '" + registryID + "' AND dh <> '10000002' AND dh <> '10000006'";
        DataSet dsPhysical = ExecuteSentence(select, hisDBType);
        dsPhysical.Tables[0].TableName = EmrConstant.DbTables.OutPhysicalExamRequisitionNums;
        for (int k = 0; k < dsPhysical.Tables[0].Rows.Count; k++)
        {
            if (Convert.IsDBNull(dsPhysical.Tables[0].Rows[k][TabTestExam.Done]))
                dsPhysical.Tables[0].Rows[k][TabTestExam.Done] = StringGeneral.Zero;
            if (dsPhysical.Tables[0].Rows[k][TabTestExam.Done].ToString() == StringGeneral.Zero)
            {
                if (IsChargedForm(dsPhysical.Tables[0].Rows[k][TabImageExam.FormSequence].ToString()))
                    dsPhysical.Tables[0].Rows[k][TabPhysicalExam.Charged] = StringGeneral.One;
                else
                    dsPhysical.Tables[0].Rows[k][TabPhysicalExam.Charged] = StringGeneral.Zero;
            }
        }
        return dsPhysical;
    }
    private DataSet GetImageRequisitionNumsForOutPatient(string registryID)
    {
        string select =
            "SELECT xh, kdys AS ysbm, kdks AS ksbm, kdmc, qtyxxjczd AS qtyxzd, dh, lczd, jymd, jyzt, jcyq," +
            "zxqk, kdrq, jybb AS enhance, '0' AS charged " +
            "FROM kd_mx_mz " +
            "WHERE zfbs is null AND mzh = '" + registryID + "' AND dh = '10000002'";
        DataSet dsImage = ExecuteSentence(select, hisDBType);
        dsImage.Tables[0].TableName = EmrConstant.DbTables.OutImageExamRequisitionNums;
        for (int k = 0; k < dsImage.Tables[0].Rows.Count; k++)
        {
            if (Convert.IsDBNull(dsImage.Tables[0].Rows[k][TabTestExam.Done]))
                dsImage.Tables[0].Rows[k][TabTestExam.Done] = StringGeneral.Zero;
            if (dsImage.Tables[0].Rows[k][TabTestExam.Done].ToString() == StringGeneral.Zero)
            {
                if (IsChargedForm(dsImage.Tables[0].Rows[k][TabImageExam.FormSequence].ToString()))
                    dsImage.Tables[0].Rows[k][TabImageExam.Charged] = StringGeneral.One;
                else
                    dsImage.Tables[0].Rows[k][TabImageExam.Charged] = StringGeneral.Zero;
            }
        }
        return dsImage;
    }
    [WebMethod(Description = "Returns treat charge info. by registryID number for back pay")]
    public DataSet GetTreatForOutPatientByRegistryID(string registryID, string departmentCode)
    {
        string select = "SELECT b.pm 名称, a.sl 数量, a.je 金额, a.sfrq 收费日期, a.sfry 收费人员, a.pjh " +
            "FROM mz_czmx a INNER JOIN tsfxm b ON a.bm = b.bm " +
            "WHERE a.mzh = '" + registryID + "' AND a.czks = '" + departmentCode + "' " +
            "AND a.sfbs = '1' AND a.qrbs = '1' AND a.tczrq is null";
        DataSet dsTreat = ExecuteSentence(select, hisDBType);
        dsTreat.Tables[0].TableName = EmrConstant.DbTables.OutTreatCost;

        select = "SELECT DISTINCT pjh " +
            "FROM mz_czmx " +
            "WHERE mzh = '" + registryID + "' AND czks = '" + departmentCode + "' " +
            "AND sfbs = '1' AND qrbs = '1' AND tczrq is null";
        DataSet dsBillNums = ExecuteSentence(select, hisDBType);
        dsBillNums.Tables[0].TableName = DbTables.OutTreat;

        dsTreat.Tables.Add(dsBillNums.Tables[0].Copy());

        return dsTreat;
    }
    [WebMethod(Description = "Returns treat charge info. by bill number for back pay")]
    public DataSet GetTreatForOutPatientByBill(string billNum, string departmentCode)
    {
        string select = "SELECT b.pm 名称, a.sl 数量, a.je 金额, a.sfrq 收费日期, a.sfry 收费人员, a.mzh " +
            "FROM mz_czmx a INNER JOIN tsfxm b ON a.bm = b.bm " +
            "WHERE a.pjh = '" + billNum + "' AND a.czks = '" + departmentCode + "' " +
            "AND a.sfbs = '1' AND a.qrbs = '1' AND a.tczrq is null";
        DataSet dsTreat = ExecuteSentence(select, hisDBType);
        dsTreat.Tables[0].TableName = EmrConstant.DbTables.OutTreatCost;
        if (dsTreat.Tables[0].Rows.Count == 0) return dsTreat;

        select = "SELECT a.hzxm, a.hzlx, a.ysbm, c.ysm " +
            "FROM mz_ghmx AS a " +
            "LEFT OUTER JOIN tysm AS c ON a.ysbm = c.ysbm WHERE a.mzh = '" +
            dsTreat.Tables[0].Rows[0][5].ToString() + "'";
        DataSet dsReg = ExecuteSentence(select, hisDBType);
        dsReg.Tables[0].TableName = EmrConstant.DbTables.OutPatientRegInfo;

        dsTreat.Tables.Add(dsReg.Tables[0].Copy());

        return dsTreat;
    }
    [WebMethod(Description = "Returns treat charge info. by registryID number for back pay")]
    public DataSet GetTestResultForOneRequisition(decimal requisitionNum)
    {
        string select = "SELECT b.jyxmmc, a.actual_val, a.dw_of_val, a.jyjg, a.standard_val " +
            "FROM mz_jyd_tjmx AS a INNER JOIN mz_jyxmdy AS b ON a.jyxmbm = b.jyxmbm " +
            "WHERE a.jysqxh = " + requisitionNum.ToString();

        DataSet dsResult = ExecuteSentence(select, hisDBType);
        dsResult.Tables[0].TableName = DbTables.TestItems;
        return dsResult;
    }

    [WebMethod(Description = "Returns poor patients who have valid registry. ")]
    public DataSet GetPoorPatients(string doctorID, string departmentCode, double dayLimit)
    {
        DateTime dayFrom = today().Subtract(TimeSpan.FromDays(dayLimit));
        string select = "SELECT a.mzh 门诊号, b.hzxm 姓名, b.hzxb 性别, b.hznl 年龄, a.yjyj 押金, a.yjye 余额 " +
            "FROM mz_gcghmx a INNER JOIN mz_ghmx b ON a.mzh = b.mzh " +
            "WHERE b.ksbm = '" + departmentCode + "' " +
            "AND (b.ysbm = '" + doctorID + "' OR b.ysbm = '' OR b.ysbm IS NULL) " +
            "AND b.ghrq >= '" + dayFrom.ToString() + "' " +
            "AND (b.zlzt IS NULL  OR b.zlzt = '' OR zlzt IN ('1', '2', '4')) " +
            "AND a.jsrq IS NULL AND a.yjyj > 0 AND a.lx = '3'";

        DataSet dsResult = ExecuteSentence(select, hisDBType);
        dsResult.Tables[0].TableName = DbTables.Patients;
        return dsResult;
    }
    /*         
     
     */



    private bool IsChargedForm(string formSequence)
    {
        string select =
            "SELECT COUNT(*) FROM mz_czmx WHERE sfrq IS NOT NULL AND tfrq is null AND kdxh=" + formSequence;
        DataSet charge = ExecuteSentence(select, hisDBType);
        if (Convert.ToInt32(charge.Tables[0].Rows[0][0]) > 0) return true;
        else return false;
    }
    [WebMethod(Description = "Returns history registry info of a patient by archive number")]
    public DataSet GetHistoryRegistryInfo(string archiveNum)
    {
        if (archiveNum.Length <= 0) return new DataSet();

        string select = "SELECT mzh FROM mz_ghmx WHERE icno = '" + archiveNum + "' ORDER BY ghrq DESC";
        DataSet dsRegistryIDs = ExecuteSentence(select, hisDBType);
        if (dsRegistryIDs.Tables[0].Rows.Count == 0) return dsRegistryIDs;
        DataSet dsRegistryInfo = new DataSet();

        for (int i = 1; i < dsRegistryIDs.Tables[0].Rows.Count; i++)
        {
            DataSet registryInfo = RegistryInfo(dsRegistryIDs.Tables[0].Rows[i]["mzh"].ToString());
            if (registryInfo.Tables[0].Rows.Count == 0) continue;
            DataTable reginfo = registryInfo.Tables[0].Copy();
            reginfo.TableName += i.ToString();
            dsRegistryInfo.Tables.Add(reginfo);
        }

        return dsRegistryInfo;
    }



    //[WebMethod(Description = "Returns registry info of a patient by short registryID for modifing order")]
    //public DataSet GetRegistryInfoByShortRegistryIDForModifingOrder(string shortRegistryID, DateTime dayStart)
    //{
    //    string select = "SELECT mz_ghmx.mzh, mz_ghmx.hzxm,mz_ghmx.hzxb, mz_ghmx.hznl, mz_ghmx.hzlx," +
    //        "mz_ghmx.icno, mz_ghmx.ksbm,mz_ghmx.hbbm, mz_ghmx.ghrq, mz_ghmx.ysbm," +
    //        "mz_ghmx.fzbs, mz_ghmx.zlzt, mz_ghmx.kh, mz_hbbm.hbmc, mz_hbbm.yxts, " +
    //        "'' AS CardType, '0' AS CanBuyDrug, '0' AS LowInsurance  " +
    //        "FROM mz_ghmx INNER JOIN mz_hbbm ON mz_ghmx.hbbm = mz_hbbm.hbbm " +
    //        "WHERE (mz_ghmx.mzh LIKE '%" + shortRegistryID + "') AND (mz_ghmx.thbs IS NULL) " +
    //        "AND (mz_ghmx.ghrq >= '" + dayStart.ToString() + "')";
    //    DataSet dsRegistryInfo = ExecuteSentence(select, hisDBType);
    //    dsRegistryInfo.Tables[0].TableName = EmrConstant.DbTables.OutPatientRegInfo;

    //    #region Get Card type
    //    if (dsRegistryInfo.Tables[0].Rows.Count == 1)
    //    {
    //        if (!Convert.IsDBNull(dsRegistryInfo.Tables[0].Rows[0]["kh"]))
    //        {
    //            string cardNum = dsRegistryInfo.Tables[0].Rows[0]["kh"].ToString();
    //            DataSet dsCardType = GetCardTypeByCardNum(cardNum);
    //            if (dsCardType.Tables[0].Rows.Count == 1)
    //            {
    //                dsRegistryInfo.Tables[0].Rows[0]["CardType"] = dsCardType.Tables[0].Rows[0]["klx"];
    //                dsRegistryInfo.Tables[0].Rows[0]["CanBuyDrug"] = dsCardType.Tables[0].Rows[0]["kfgy"];
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Get low insurance flag
    //    if (dsRegistryInfo.Tables[0].Rows.Count == 1)
    //    {
    //        string registryID = dsRegistryInfo.Tables[0].Rows[0]["mzh"].ToString();
    //        DataSet dsDeposit = GetDeposit(registryID);
    //        if (dsDeposit.Tables[0].Rows.Count == 1)
    //        {
    //            dsRegistryInfo.Tables[0].Rows[0]["LowInsurance"] = "1";
    //        }
    //    }
    //    #endregion

    //    return dsRegistryInfo;

    //}




    private DataSet GetRegistryInfoByBillNum(string billNum)
    {
        string select = "SELECT max(mzh) AS mzh FROM mz_pjmx WHERE pjh = '" + billNum + "'";
        DataSet regInfo = ExecuteSentence(select, hisDBType);
        if (regInfo.Tables[0].Rows.Count == 1)
        {
            string registryID = regInfo.Tables[0].Rows[0]["mzh"].ToString();
            regInfo = RegistryInfo(registryID);
            regInfo.Tables[0].TableName = EmrConstant.DbTables.OutPatientRegInfo;
        }
        return regInfo;
    }
    private DataSet GetRegistryInfoByCardNum(string cardNum, DateTime dayStart, string departmentCode)
    {
        string select = "SELECT MAX(mzh) AS mzh FROM mz_ghmx WHERE ksbm = '" + departmentCode + "' " +
            "AND ghrq >= '" + dayStart.ToString() + "' " +
            "AND kh = '" + cardNum + "'";
        DataSet regInfo = ExecuteSentence(select, hisDBType);
        if (regInfo.Tables[0].Rows.Count == 1)
        {
            string registryID = regInfo.Tables[0].Rows[0]["mzh"].ToString();
            regInfo = RegistryInfo(registryID);
            regInfo.Tables[0].TableName = EmrConstant.DbTables.OutPatientRegInfo;
        }
        return regInfo;
    }

    //[WebMethod(Description = "Returns old Treat and patient registry info. by bill number for back pay")]
    //public DataSet GetTreatForOutPatientByBillNum(string billNum, string departmentCode)
    //{
    //    DataSet dsRegInfo = GetRegistryInfoByBillNum(billNum);
    //    string select = "SELECT czxh, bm, sl, cs, je, pjh, tfbs, sfrq, qrrq, qrry, tczrq, tczry, qrbs " +
    //        "FROM mz_czmx " +
    //        "WHERE pjh = '" + billNum + "' AND czks = '" + departmentCode + "' " +
    //        "AND sfbs = '1' AND qrbs = '1' AND tczrq is null";
    //    DataSet dsTreat = ExecuteSentence(select, hisDBType);
    //    dsTreat.Tables[0].TableName = EmrConstant.DbTables.OutTreat;
    //    DataTable treat = dsTreat.Tables[0].Copy();
    //    dsRegInfo.Tables.Add(treat);

    //    return dsRegInfo;
    //}

    [WebMethod(Description = "Returns old Treat and patient registry info. by card number for back pay")]
    public DataSet GetTreatForOutPatientByCardNum(string cardNum, DateTime dayStart, string departmentCode)
    {
        DataSet dsRegInfo = GetRegistryInfoByCardNum(cardNum, dayStart, departmentCode);
        DataTable regInfo = dsRegInfo.Tables[EmrConstant.DbTables.OutPatientRegInfo];
        if (regInfo == null) return dsRegInfo;
        if (regInfo.Rows.Count == 1)
        {
            string registryID = dsRegInfo.Tables[0].Rows[0]["mzh"].ToString();
            string select = "SELECT b.pm AS 名称, a.sl*a.cs AS 数量, a.je AS 金额, a.sfrq AS 收费日期, " +
                "a.qrrq AS 确认日期, a.qrry AS 确认医师, a.pjh, a.tfbs,  a.tczrq, a.tczry, a.qrbs, a.czxh " +
                "FROM mz_czmx a INNER JOIN tsfxm b ON a.bm = b.bm " +
                "WHERE a.mzh = '" + registryID + "' AND a.czks = '" + departmentCode + "' " +
                "AND a.qrbs = '1' AND a.tczrq is null";
            DataSet dsTreat = ExecuteSentence(select, hisDBType);
            dsTreat.Tables[0].TableName = EmrConstant.DbTables.OutTreat;
            DataTable treat = dsTreat.Tables[0].Copy();
            dsRegInfo.Tables.Add(treat);
        }

        return dsRegInfo;
    }

    [WebMethod(Description = "Returns patients for geting archive number")]
    public DataSet GetPatientsForArchiveNumByName(string patientName)
    {
        string select = "SELECT cardno, xm, xb, csrq, jtzz, sfzh FROM mz_card WHERE xm = '" +
            patientName + "'";
        DataSet dsCard = ExecuteSentence(select, hisDBType);
        return dsCard;
    }

    [WebMethod(Description = "Returns registryIDs by card number")]
    public DataSet GetRegistryIDsForOneCardNum(string cardNum)
    {
        string select = "SELECT mzh, hzxm, ghrq, ysbm, ksbm, hzlx FROM mz_ghmx WHERE kh = '" + cardNum +
            "' ORDER BY ghrq DESC";
        DataSet dsRegistryID = ExecuteSentence(select, hisDBType);
        return dsRegistryID;
    }

    [WebMethod(Description = "Returns card type by card number")]
    public DataSet GetCardTypeByCardNum(string cardNum)
    {
        string select = "SELECT klx, kfgy FROM mz_kjl WHERE kh = '" + cardNum + "'";
        DataSet dsCardType = ExecuteSentence(select, hisDBType);
        return dsCardType;
    }
    [WebMethod(Description = "Returns deposit for urgent observation patient ")]
    public DataSet GetDeposit(string registryID)
    {
        string select = "SELECT yjye FROM mz_gcghmx WHERE mzh = '" + registryID +
            "' AND jsrq is null";
        DataSet dsDeposit = ExecuteSentence(select, hisDBType);
        return dsDeposit;
    }

    [WebMethod(Description = "Returns registry info by registryID for query ")]
    public DataSet GetRegistryInfoByCardNumForCallBack(string department, DateTime dayStart, string cardNum)
    {
        string select = "SELECT mzh FROM mz_ghmx " +
            "WHERE kh='" + cardNum + "' AND " +
            "ksbm='" + department + "' AND " +
            "ghrq >= '" + dayStart.ToString() + "' AND " +
            "thbs is null AND zlzt='9'";
        DataSet dsRegistryInfo = ExecuteSentence(select, hisDBType);
        if (dsRegistryInfo.Tables[0].Rows.Count == 0) return dsRegistryInfo;
        string registryID = dsRegistryInfo.Tables[0].Rows[0]["mzh"].ToString();
        dsRegistryInfo = RegistryInfo(registryID);
        return dsRegistryInfo;
    }

    [WebMethod(Description = "Returns order info by card number for call back ")]
    public DataSet GetOrderInfoByRegistryIDForQuery(string registryID)
    {
        #region West drug
        string select = "SELECT a.cfh AS 处方号, b.pm AS 药品名称, a.ypyl AS 用量, a.ts AS 天数, a.cs AS 次数, " +
            "a.cfsl AS 处方数量, a.dw 单位, a.yfyl AS 频次, a.fsbm AS 执行方式, '' AS 收费情况, " +
            "a.sfbs, a.tfbs, a.jfwb, tybs " +
            "FROM mz_ypmx a INNER JOIN yp_zy b ON a.bm = b.bm " +
            "WHERE a.mzh='" + registryID + "' AND b.dl='A' ORDER BY 处方号";
        DataSet dsOrder = ExecuteSentence(select, hisDBType);
        dsOrder.Tables[0].TableName = EmrConstant.DbTables.WestDrug;
        #endregion
        #region Chinese drug
        select = "SELECT a.cfh AS 处方号, b.pm AS 药品名称, a.ypyl AS 用量, a.ts AS 天数, a.cs AS 次数, " +
            "a.cfsl AS 处方数量, a.dw 单位, a.yfyl AS 频次, a.fsbm AS 执行方式, '' AS 收费情况, " +
            "a.sfbs, a.tfbs, a.jfwb, tybs " +
            "FROM mz_ypmx a INNER JOIN yp_zy b ON a.bm = b.bm " +
            "WHERE a.mzh='" + registryID + "' AND b.dl<>'A' AND b.dl<>'C' ORDER BY 处方号";
        DataSet dsChineseDrug = ExecuteSentence(select, hisDBType);
        DataTable chineseDrug = dsChineseDrug.Tables[0].Copy();
        chineseDrug.TableName = EmrConstant.DbTables.ChineseDrug;
        dsOrder.Tables.Add(chineseDrug);
        #endregion
        #region Herbal drug
        select = "SELECT a.cfh, b.pm, a.ypyl,a.cfsl, a.dw, a.cffs, a.je, a.zxfs, a.sfbs " +
            "FROM mz_ypmx a INNER JOIN yp_zy b ON a.bm = b.bm " +
            "WHERE a.mzh='" + registryID + "' AND b.dl='C'";
        DataSet dsHerbalDrug = ExecuteSentence(select, hisDBType);
        DataTable herbalDrug = dsHerbalDrug.Tables[0].Copy();
        herbalDrug.TableName = EmrConstant.DbTables.HerbalDrug;
        dsOrder.Tables.Add(herbalDrug);
        #endregion
        #region Examination
        select = "SELECT a.kdmc AS 开单名称, a.zxqk AS 执行情况, a.kdrq AS 开单时间, a.zxrq AS 执行时间, " +
            "b.sfbs, b.tczry " +
            "FROM kd_mx_mz a INNER JOIN mz_czmx b ON a.mzh=b.mzh " +
            "WHERE (a.mzh = '" + registryID + "' AND a.zfbs is null AND a.dh <> '00000003') OR " +
            "(a.mzh = '" + registryID + "' AND a.zfbs <> '1' AND a.dh <> '00000003')";
        DataSet dsExam = ExecuteSentence(select, hisDBType);
        DataTable exam = dsExam.Tables[0].Copy();
        exam.TableName = EmrConstant.DbTables.Exam;
        dsOrder.Tables.Add(exam);
        #endregion



        return dsOrder;

    }

    [WebMethod(Description = "Returns departments for out patients to registry ")]
    public DataSet GetDepartmentsForRegistry()
    {
        string select = "SELECT ksbm, ksmc FROM mz_ksbm WHERE ghbs = '1'";
        DataSet dsDeposit = ExecuteSentence(select, hisDBType);
        return dsDeposit;
    }

    [WebMethod(Description = "Returns Treat items for one oprator")]
    public DataSet GetOutPatientTreatFeeForOneOperator(string opcode, DateTime dayFrom, DateTime dayTo)
    {
        string select = "SELECT b.pm AS 名称, a.sl*a.cs AS 数量, a.je AS 金额, " +
                "a.czks AS 执行科室, a.hjrq AS 划价日期, c.hzxm AS 患者姓名 " +
                "FROM mz_czmx a INNER JOIN tsfxm b ON a.bm = b.bm INNER JOIN mz_ghmx c ON a.mzh = c.mzh " +
                "WHERE a.ysbm = '" + opcode + "' AND (a.hjrq BETWEEN '" + dayFrom.ToString() + "' " +
                "AND '" + dayTo.ToString() + "') " +
                "AND ((a.kh is not null AND a.jfwb = '1' AND a.tczrq is null) OR " +
                "(a.kh is null AND a.sfbs = '1' AND (a.tfbs <> '1' OR a.tfbs is null)))";
        DataSet dsTreat = ExecuteSentence(select, hisDBType);

        return dsTreat;
    }

    [WebMethod(Description = "Returns Treat items for one oprator")]
    public DataSet GetTreatFeeForOneOutPatient(string opcode, string registryID)
    {
        string select = "SELECT b.pm AS 名称, a.sl*a.cs AS 数量, a.je AS 金额, " +
                "a.czks AS 执行科室, a.hjrq AS 划价日期 " +
                "FROM mz_czmx a INNER JOIN tsfxm b ON a.bm = b.bm INNER JOIN mz_ghmx c ON a.mzh = c.mzh " +
                "WHERE (a.ysbm = '" + opcode + "' AND a.mzh = '" + registryID + "' AND " +
                "a.kh is not null AND a.jfwb = '1' AND a.tczrq is null) OR " +
                "(a.ysbm = '" + opcode + "' AND a.mzh = '" + registryID + "' AND " +
                "a.kh is null AND a.sfbs = '1' AND (a.tfbs <> '1' OR a.tfbs is null))";
        DataSet dsTreat = ExecuteSentence(select, hisDBType);

        return dsTreat;
    }
    #endregion

    [WebMethod(Description = "Returns icd10")]
    public DataSet Icd10()
    {
        string select = "SELECT distinct pym, jbfl FROM icd10 order by pym";
        DataSet icd10 = ExecuteSentence(select, hisDBType);
        icd10.Tables[0].TableName = EmrConstant.DbTables.Icd10;
        return icd10;
    }

    [WebMethod(Description = "Returns ")]
    public DataSet GetItemsForBeInpatient()
    {
        string select = "SELECT  lbbm code, lbmc name, tpbs, zfbl FROM zy_hzlbbm";
        DataSet dsItems = ExecuteSentence(select, hisDBType);
        dsItems.Tables[0].TableName = DbTables.PType;

        select = "SELECT bqbm code, bqmc name FROM bqbmk WHERE (lcbs = '1') and (bqbm <> '00')";
        DataSet dsAreas = ExecuteSentence(select, hisDBType);
        dsAreas.Tables[0].TableName = DbTables.Areas;
        dsItems.Tables.Add(dsAreas.Tables[0].Copy());

        select = "SELECT ksbm code, ksmc name FROM mz_ksbm WHERE lcbs='1'";
        DataSet dsDepart = ExecuteSentence(select, hisDBType);
        dsDepart.Tables[0].TableName = DbTables.Departments;
        dsItems.Tables.Add(dsDepart.Tables[0].Copy());

        select = "SELECT ysbm code, ysm name, xmpy pym FROM tysm WHERE lb='1' OR lb='2' ORDER BY xmpy";
        DataSet dsDoctors = ExecuteSentence(select, hisDBType);
        dsDoctors.Tables[0].TableName = DbTables.Doctors;
        dsItems.Tables.Add(dsDoctors.Tables[0].Copy());

        DataSet dsIcd10 = Icd10();
        dsItems.Tables.Add(dsIcd10.Tables[0].Copy());


        return dsItems;
    }

    [WebMethod(Description = "Returns room and bed info")]
    public DataSet Rooms(string areaCode)
    {
        string select = "SELECT bffh number, bfxb sex, kcsl total, bzcf price FROM bfbmk WHERE bqbm = '" +
            areaCode + "' ORDER BY bffh ASC";
        DataSet dsRooms = ExecuteSentence(select, hisDBType);
        dsRooms.Tables[0].TableName = EmrConstant.DbTables.Rooms;

        select = "SELECT a.bffh, a.bcbm, a.zcbs FROM bcbmk AS a INNER JOIN bfbmk " +
            "AS b ON a.bffh = b.bffh WHERE b.bqbm = '" + areaCode + "'";
        DataSet dsBeds = ExecuteSentence(select, hisDBType);
        dsBeds.Tables[0].TableName = EmrConstant.DbTables.Beds;
        dsRooms.Tables.Add(dsBeds.Tables[0].Copy());

        select = "SELECT ch, xm, xb, nl, zyh, zyrq, mzzdmc, ych, bffh FROM tdjkz WHERE cyrq IS null " +
            "AND bqbm = '" + areaCode + "'";
        DataSet dsPatients = ExecuteSentence(select, hisDBType);
        dsPatients.Tables[0].TableName = DbTables.Patients;
        dsRooms.Tables.Add(dsPatients.Tables[0].Copy());

        return dsRooms;
    }

    /*
             SELECT dz_pdxx.mzh,   
             dz_pdxx.hzxm,   
             dz_pdxx.ksbm,   
             dz_pdxx.ysbm,   
             dz_pdxx.yxj,   
             dz_pdxx.pdh,   
             dz_pdxx.hjsj,   
             dz_pdxx.hbbm,   
             dz_pdxx.fzbs,   
             dz_pdxx.zsbm  
        FROM dz_pdxx  
       WHERE ( dz_pdxx.pdzt is null OR dz_pdxx.pdzt = '' ) AND  
             ( dz_pdxx.zsbm is null or dz_pdxx.zsbm = '') AND  
             ( dz_pdxx.ghrq >= :rq1 ) AND  
             ( dz_pdxx.ghrq <= :rq2 ) AND  
             ( dz_pdxx.ksbm = :ks ) AND  
             ( dz_pdxx.hbbm <> '3' AND  
             dz_pdxx.hbbm <> '4' )  
     * 
     SELECT dz_pdxx.mzh,   
             dz_pdxx.hzxm,   
             dz_pdxx.ksbm,   
             dz_pdxx.ysbm,   
             dz_pdxx.yxj,   
             dz_pdxx.pdh,   
             dz_pdxx.hjsj,   
             dz_pdxx.hbbm,   
             dz_pdxx.fzbs,   
             dz_pdxx.zsbm  
        FROM dz_pdxx  
       WHERE ( dz_pdxx.pdzt is null OR dz_pdxx.pdzt = '' ) AND  
             ( dz_pdxx.zsbm is null ) AND  
             ( dz_pdxx.ghrq >= :rq1 ) AND  
             ( dz_pdxx.ghrq <= :rq2 ) AND  
             ( dz_pdxx.ksbm = :ks ) AND  
             ( dz_pdxx.hbbm = '3' OR  
             dz_pdxx.hbbm = '4' ) AND  
             ( dz_pdxx.ysbm = :ys ) 
     * 
     * SELECT dz_pdxx.mzh,   
         dz_pdxx.hzxm,   
         dz_pdxx.ksbm,   
         dz_pdxx.ysbm,   
         dz_pdxx.yxj,   
         dz_pdxx.pdh,   
         dz_pdxx.hjsj,   
         dz_pdxx.hbbm,   
         dz_pdxx.zsbm  
    FROM dz_pdxx  
   WHERE ( dz_pdxx.pdzt = '0' OR  z_pdxx.pdzt = '1' OR  dz_pdxx.pdzt = '2' or dz_pdxx.pdzt = '3' ) AND  
         ( dz_pdxx.ghrq >= :rq1 ) AND  
         ( dz_pdxx.ghrq <= :rq2 ) AND  
         ( dz_pdxx.mzh = :mzh )   and 
         ( dz_pdxx.zqbm = :zqbm ) 
     * 
     
     SELECT dz_pdxx.hzxm,   
         dz_pdxx.pdh,   
         dz_pdxx.yxj,
         dz_pdxx.ghrq  
    FROM dz_pdxx 
  where (dz_pdxx.pdzt is null or dz_pdxx.pdzt = '' ) and
        dz_pdxx.hbbm <> '3' and dz_pdxx.hbbm <> '4' and
        dz_pdxx.ghrq >= :rq1 and
        dz_pdxx.ghrq <= :rq2 and
        dz_pdxx.ksbm = :ks and 
        dz_pdxx.zqbm = :zqbm 
     * 
    SELECT dz_pdxx.hzxm,   
         dz_pdxx.pdh,   
         dz_pdxx.yxj,
         dz_pdxx.ghrq  
    FROM dz_pdxx   
   where  (dz_pdxx.pdzt is null or dz_pdxx.pdzt = '' ) and
          ( dz_pdxx.hbbm = '3' or dz_pdxx.hbbm = '4' ) and
          dz_pdxx.ysbm = :ys and
          dz_pdxx.ghrq >= :rq1 and  
          dz_pdxx.ghrq <= :rq2 and
          dz_pdxx.ksbm = :ks and 
          dz_pdxx.zqbm = :zqbm
     */

}


using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;

using AboutConfig;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using EmrConstant;
using System.Collections.Generic;
using Lucene.Net;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.PanGu;


/// <summary>
/// Summary description for emrServiceXml
/// </summary>
[WebService(Namespace = "http://shoujia.org/emrservices/")]
//[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class emrServiceXml : System.Web.Services.WebService
{
    private string MakeArchiveNum()
    {
        string prefix;
        long currentNum = 0;
        prefix = "SJ";

        return prefix + currentNum.ToString("00000000");

    }

    private bool GetEmrNotes(string registryID, XmlNode emrNotes)
    {

        string Query = "SELECT NoteIDSeries, CONVERT(xml, NoteDocument) FROM EmrNote WHERE RegistryID = '"
            + registryID + " '";
        //string Query = "SELECT CONVERT(xml, NoteDocument) FROM EmrNote WHERE RegistryID = '" + registryID + " '";
        SqlDataReader reader = null;
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = new SqlCommand(Query, CS);
            myCommand.CommandType = CommandType.Text;
            reader = myCommand.ExecuteReader();
            while (reader.Read() == true)
            {
                XmlElement emrNote = emrNotes.OwnerDocument.CreateElement(EmrConstant.ElementNames.EmrNote);
                emrNote.InnerXml = reader[1].ToString();
                //emrNote.InnerXml = reader[0].ToString();
                XmlElement emrNoteChild = (XmlElement)emrNote.FirstChild;
                emrNoteChild.SetAttribute("NoteIDSeries", reader[0].ToString());
                emrNotes.AppendChild(emrNoteChild);
            }
            //emrNotes = doc.Clone;
            reader.Close();
            CS.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("GetEmrDocument", "EmrDocument", e.Message, e.Source);
            return EmrConstant.Return.Failed;
        }

    }
    [WebMethod(Description = " emrNotes")]
    public bool GetAllEmrNotes(ref XmlNode emrNotes)
    {

        string Query = "SELECT [RegistryID], NoteIDSeries, CONVERT(xml, NoteDocument) FROM EmrNote ";

        //string Query = "SELECT CONVERT(xml, NoteDocument) FROM EmrNote WHERE RegistryID = '" + registryID + " '";
        SqlDataReader reader = null;
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = new SqlCommand(Query, CS);
            myCommand.CommandType = CommandType.Text;
            reader = myCommand.ExecuteReader();
            while (reader.Read() == true)
            {
                XmlElement emrNote = emrNotes.OwnerDocument.CreateElement(EmrConstant.ElementNames.EmrNote);
                emrNote.InnerXml = reader[2].ToString();
                //emrNote.InnerXml = reader[0].ToString();
                XmlElement emrNoteChild = (XmlElement)emrNote.FirstChild;
                emrNoteChild.SetAttribute("registryID", reader[0].ToString());
                emrNoteChild.SetAttribute("NoteIDSeries", reader[1].ToString());
                emrNotes.AppendChild(emrNoteChild);
            }
            //emrNotes = doc.Clone;
            reader.Close();
            CS.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("GetEmrDocument", "EmrDocument", e.Message, e.Source);
            return EmrConstant.Return.Failed;
        }

    }
    private bool GetDocument(string registryID, SqlConnection connection, ref XmlNode emr)
    {
        string SQLSentence = "SELECT Document FROM EmrDocument WHERE RegistryID='" + registryID + "'";
        SqlCommand myCommand = new SqlCommand(SQLSentence, connection);
        myCommand.CommandType = CommandType.Text;
        SqlDataReader reader = myCommand.ExecuteReader();
        if (reader.Read())
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            emr = doc.DocumentElement.Clone();
            reader.Close();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        reader.Close();
        reader.Close();
        connection.Close();
        return EmrConstant.Return.Failed;
    }

    #region Utilities
    [WebMethod]
    public DateTime SysTime()
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        SqlCommand command = new SqlCommand("SysTime", connection);
        command.CommandType = CommandType.StoredProcedure;

        SqlParameter parameter = command.Parameters.Add(
          "@today", SqlDbType.DateTime);
        parameter.Direction = ParameterDirection.Output;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        DateTime today = Convert.ToDateTime(command.Parameters["@today"].Value);
        reader.Close();
        connection.Close();

        return today;


    }

    [WebMethod]
    public DateTime ErrorLogEx(string method, string dbtable, string op, string comm)
    {
        /* make error log file name*/
        string logFilePath = ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath");
        if (!Directory.Exists(logFilePath)) Directory.CreateDirectory(logFilePath);
        string logFileFullPath = logFilePath + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName");

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.PreserveWhitespace = true;
        XmlNode root = null;
        if (!File.Exists(logFileFullPath))
        {
            //加入XML的声明段落 
            XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmldoc.AppendChild(xmlnode);
            //加入一个根元素
            XmlElement xmlelem = xmldoc.CreateElement("", "errors", "");
            xmldoc.AppendChild(xmlelem);
            root = xmldoc.SelectSingleNode("errors");

        }
        else
        {
            xmldoc.Load(logFileFullPath);
            root = xmldoc.SelectSingleNode("errors");
        }

        XmlElement xe1 = xmldoc.CreateElement("error");

        XmlAttribute xmlattr = xmldoc.CreateAttribute("errordate");
        DateTime errorTime = DateTime.Now;
        xmlattr.Value = XmlConvert.DecodeName(errorTime.ToString());
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("computername");
        xmlattr.Value = XmlConvert.DecodeName(ComputerName());
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("method");
        xmlattr.Value = XmlConvert.DecodeName(method);
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("dbtable");
        xmlattr.Value = XmlConvert.DecodeName(dbtable);
        xe1.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("comment");
        xmlattr.Value = XmlConvert.DecodeName(comm);
        xe1.Attributes.Append(xmlattr);

        root.AppendChild(xe1);

        xmldoc.Save(logFileFullPath);
        return errorTime;
    }
    /* ----------------------------------------------------------------------------------
     * Return the error message just occured from error log file.
     * 
     * 
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Get last error message", EnableSession = false)]
    public string GetLastErrorEx(DateTime errorTime)
    {
        string logFileFullPath = ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath")
            + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName");
        if (!File.Exists(logFileFullPath)) return null;

        XmlDocument doc = new XmlDocument();
        doc.PreserveWhitespace = true;
        try
        {
            doc.Load(logFileFullPath);

            XmlNodeList errors = doc.DocumentElement.SelectNodes("error");

            string errTime = errorTime.ToString();
            for (int i = errors.Count - 1; i > 0; i--)
            {
                if (errTime == errors[i].Attributes["errordate"].Value)
                    return errors[i].Attributes["comment"].Value;
            }
        }
        catch (XmlException ex)
        {
            return ex.Message;
        }
        return null;


    }

    /* --------------------------------------------------------------------------
     * During the operation on database, some errors maybe occure. If so, you 
     * should logged the error message.
     * error message format:
     *      series:datetime:method:db table:operation:comment
     *      series   -- sequence number the error ocuured
     *      datetime -- 2006-02-25 08:12:55 time error occured
     *      method   -- function name
     *      db table -- name of table on which operation
     *      comment  -- additonal specification(optional)
     * log file is [client]errorlog.xml and errorlog.xsd that's schema 
     * log file name prefix [client] is client computer name
 
     * ----------------------------------------------------------------------------- */

    [WebMethod]
    public void ErrorLog(string method, string dbtable, string op, string comm)
    {
        /* make error log file name*/



        // xmlattr = xmldoc.CreateAttribute("message");
        // xmlattr.Value = XmlConvert.DecodeName(ex.Message);
        // xe1.Attributes.Append(xmlattr);

        //加入XML的声明段落 
        if (!Directory.Exists(ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath")))
        {
            Directory.CreateDirectory(ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath"));
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.PreserveWhitespace = true;
            XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");

            xmldoc.AppendChild(xmlnode);
            //加入一个根元素
            XmlElement xmlelem = xmldoc.CreateElement("", "errors", "");
            xmldoc.AppendChild(xmlelem);

            XmlNode root = xmldoc.SelectSingleNode("errors");


            XmlElement xe1 = xmldoc.CreateElement("error");

            XmlAttribute xmlattr = xmldoc.CreateAttribute("errordate");
            DateTime errorTime = DateTime.Now;
            xmlattr.Value = XmlConvert.DecodeName(errorTime.ToString().Split(' ')[0]);
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("computername");
            xmlattr.Value = XmlConvert.DecodeName(ComputerName());
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("method");
            xmlattr.Value = XmlConvert.DecodeName(method);
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("dbtable");
            xmlattr.Value = XmlConvert.DecodeName(dbtable);
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("comment");
            xmlattr.Value = XmlConvert.DecodeName(comm);
            xe1.Attributes.Append(xmlattr);

            root.AppendChild(xe1);

            //string logName = ComputerName() + EmrConstant.ErrorMessage.ErrLogFileName;
            xmldoc.Save(ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath") + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName"));
        }
        else
        {

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.PreserveWhitespace = true;
            xmldoc.Load(ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath") + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName"));

            XmlNode root = xmldoc.SelectSingleNode("errors");

            XmlNode xe1 = xmldoc.CreateElement("error");

            XmlAttribute xmlattr = xmldoc.CreateAttribute("errordate");
            xmlattr.Value = XmlConvert.DecodeName(System.DateTime.Now.ToString());
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("computername");
            xmlattr.Value = XmlConvert.DecodeName(ComputerName());
            xe1.Attributes.Append(xmlattr);


            xmlattr = xmldoc.CreateAttribute("method");
            xmlattr.Value = XmlConvert.DecodeName(method);
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("dbtable");
            xmlattr.Value = XmlConvert.DecodeName(dbtable);
            xe1.Attributes.Append(xmlattr);

            xmlattr = xmldoc.CreateAttribute("comment");
            xmlattr.Value = XmlConvert.DecodeName(comm);
            xe1.Attributes.Append(xmlattr);

            root.AppendChild(xe1);
            xmldoc.Save(ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath") + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName"));


        }




    }
    /* ------------------------------------------------------------------------------
     * You must look for a way to get client ID that consumes this web service.
     * I am not sure whether User.Identity.Name is client site or server site.
     -------------------------------------------------------------------------------*/
    private string ComputerName()
    {
        /* User.Identity.Name = computername/username   ex. cjj/Administrator */
        //string machineName = System.Net.Dns.GetHostName();
        string machineName = Context.Request.UserHostName;
        //Context.Request.UserHostName();


        // string userName = Context.User.Identity.Name;
        string userName = HttpContext.Current.User.Identity.Name;

        //return machineName + "\\" + userName;
        return machineName + "//" + userName;
    }
    /* ----------------------------------------------------------------------------------
     * Return the error message just occured from error log file.
     * 
     * 
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Get last error message", EnableSession = false)]
    public string GetLastError()
    {

        XmlDocument doc = new XmlDocument();
        doc.PreserveWhitespace = true;
        //doc.LoadXml("<book>" + "book<title>Pride And Prejudice</title>"  +
        //            "price<price>19.95</price>"  +          
        //            "</book>");
        //doc.Load("D:\\Error\\errorLog.xml");
        string errorDoc = ConfigClass.GetConfigString("appSettings", "ErrorLogFilePath")
            + ConfigClass.GetConfigString("appSettings", "ErrorLogFileName");

        doc.Load(errorDoc);
        XmlNodeList xnl = doc.SelectSingleNode("errorlist").ChildNodes;

        XmlElement xe = (XmlElement)xnl.Item(xnl.Count - 1);

        string date = xe.GetAttribute("errordate");
        string message = xe.GetAttribute("message");
        return date + message;





        // string errorMessage = "Error Message";
        // return errorMessage;
    }
    #endregion

    #region Documents
    [WebMethod(Description = "Returns emrDocument and its emrNotes")]
    public Boolean GetEmrDocument(string registryID, ref XmlNode root, ref XmlNode emrNotes)
    {

        string Query = "SELECT CONVERT(xml, document), status FROM Emrdocument WHERE RegistryID = '" + registryID + " '";
        SqlDataReader reader = null;
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = new SqlCommand(Query, CS);
            myCommand.CommandType = CommandType.Text;
            reader = myCommand.ExecuteReader();
            if (reader.Read() == true)
            {
                string str = reader[0].ToString();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(str);
                xmlDoc.DocumentElement.Attributes[EmrConstant.AttributeNames.EmrStatus].Value = reader[1].ToString();
                root = xmlDoc.Clone();
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                string str = "<Emr RegistryID=" + "\"" + registryID + "\"" + " EmrStatus=\"0\" Series=\"0\" />";
                xmlDoc.LoadXml(str);
                root = xmlDoc.Clone();
            }
            reader.Close();
            CS.Close();
            return GetEmrNotes(registryID, emrNotes);
        }
        catch (Exception e)
        {
            ErrorLog("GetEmrDocument", "EmrDocument", e.Message, e.Source);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "Add new emrDocument and one emrNote")]
    public Boolean InsertEmrDocument(string registryID, string archiveNum, XmlNode root, int status,
       string noteIDSeries, XmlNode emrNote)
    {

        // string Query = "INSERT INTO EmrDocument (RegistryID, ArchiveNum, Document, Status) VALUES( '" + registryID + " ','" + archiveNum +"','" + root + "','" + status +" ')" ;
        try
        {
            string Query = "INSERT EmrDocument (RegistryID, ArchiveNum, Document, Status) VALUES( '"
                + registryID + " ','" + archiveNum + " ','" + root.OuterXml + " ','" + status + " ')";

            //string Query = "INSERT EmrDocument (RegistryID,ArchiveNum, Document, Status) VALUES( '" + registryID + " ','" + archiveNum + "',cast('" + root.OuterXml + " ' as xml ) ,'" + status + " ')";

            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = new SqlCommand(Query, CS);
            SqlDataReader reader = myCommand.ExecuteReader();
            //return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("InsertEmrDocument", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Insert, ex.Message);
            return EmrConstant.Return.Failed;
        }

        return InsertEmrNote(registryID, noteIDSeries, emrNote);
    }

    [WebMethod(Description = "Add new one emrNote without emrDocument(mey be no longer useful)")]
    public bool InsertEmrNote(string registryID, string noteIDSeries, XmlNode emrNote)
    {
        try
        {
            SqlConnection CS2 = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS2.Open();
            string Query2 = "INSERT EmrNote (RegistryID, NoteIDSeries, NoteDocument) VALUES( '"
                + registryID + " ','" + noteIDSeries + " ','" + emrNote.OuterXml + " ')";
            SqlCommand myCommand2 = new SqlCommand(Query2, CS2);
            myCommand2.ExecuteNonQuery();
            CS2.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("InsertEmrNote", EmrConstant.DbTables.EmrDocument, EmrConstant.SqlOperations.Insert, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "Update an EmrDocument and Add a new EmrNote")]
    public Boolean AddNoteDocument(string registryID, XmlNode root, int status,
        string noteIDSeries, XmlNode emrNote)
    {
        string Query = "UPDATE EmrDocument SET Document =  cast('" + root.OuterXml
            + " ' as xml ) ,Status = '" + status + "' WHERE RegistryID = '" + registryID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("UpdateEmrDocument", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }

        return InsertEmrNote(registryID, noteIDSeries, emrNote);
    }

    [WebMethod(Description = "Get EmrDocument without any EmrNote")]
    public bool GetEmrDocumentWithoutNote(string registryID, ref XmlNode emr)
    {

        //string SQLSentence = "SELECT CONVERT(xml, document) FROM EmrDocument WHERE RegistryID='"+ registryID + "'";
        string SQLSentence = "SELECT Document FROM EmrDocument WHERE RegistryID='" + registryID + "'";
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();

        SqlCommand myCommand = new SqlCommand(SQLSentence, connection);
        myCommand.CommandType = CommandType.Text;
        SqlDataReader reader = myCommand.ExecuteReader();
        if (reader.Read())
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            emr = doc.DocumentElement.Clone();
            reader.Close();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        reader.Close();
        connection.Close();
        return EmrConstant.Return.Failed;
    }

    [WebMethod(Description = "Put EmrDocument and one EmrNote into database(mey be no longer useful)")]
    public Boolean PutEmrDocument(string registryID, string archiveNum, XmlNode root, int status,
        string noteIDSeries, XmlNode emrNote)
    {
        archiveNum = MakeArchiveNum();
        return InsertEmrDocument(registryID, archiveNum, root, status, noteIDSeries, emrNote);
    }

    [WebMethod(Description = "Update EmrDocument and one EmrNote")]
    public Boolean UpdateEmrDocument(string registryID, XmlNode root, int status,
        string noteIDSeries, XmlNode emrNote)
    {
        string Query = "UPDATE EmrDocument SET Document =  cast('" + root.OuterXml
            + " ' as xml ) ,Status = '" + status + "' WHERE RegistryID = '" + registryID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            CS.Close();
            return UpdateEmrNote(registryID, noteIDSeries, emrNote);
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("UpdateEmrDocument", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }


    }

    [WebMethod(Description = "Update EmrDocument only")]
    public Boolean UpdateEmrDocumentWithoutNote(string registryID, XmlNode root)
    {
        //this.Context.Request.UserHostAddress
        string Query = "UPDATE EmrDocument SET Document =  cast('" + root.OuterXml
            + " ' as xml ) WHERE RegistryID = '" + registryID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = CS.CreateCommand();
            string strSql = "insert into test_webServiceLog(funName,logTime,SqlText,registryid) values('UpdateEmrDocumentWithoutNote',getdate(),'" + Query.Replace("'", "''") + "','" + registryID.Replace("'", "''") + "')";
            myCommand.CommandText = strSql;
            myCommand.CommandType = CommandType.Text;
            myCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {

        }
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            CS.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("UpdateEmrDocument", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }

    }


    [WebMethod(Description = "Update EmrNote")]
    public bool UpdateEmrNote(string registryID, string noteIDSeries, XmlNode emrNote)
    {
        string delete = "DELETE  FROM EmrNote WHERE RegistryID = '" + registryID
            + "' AND NoteIDSeries = '" + noteIDSeries + "'";
        string insert = "INSERT INTO EmrNote VALUES('" + registryID + "','" + noteIDSeries
            + "','" + emrNote.OuterXml + "')";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand myCommand = new SqlCommand(delete, CS);
            myCommand.ExecuteNonQuery();

            myCommand = new SqlCommand(insert, CS);
            myCommand.ExecuteNonQuery();

            CS.Close();
            return EmrConstant.Return.Successful;

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("UpdateEmrNote", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "Delete one EmrNote and update EmrDocument")]
    public string DeleteEmrNote(string registryID, XmlNode emrdoc, string noteIDSeries)
    {
        //string update = "UPDATE EmrDocument SET [Document].modify('delete /Emr/EmrNote[@Series=\"" +
        //    series.ToString() + "\" ]') WHERE (RegistryID = '" + registryID + "')";
        string update = "UPDATE EmrDocument SET [Document] = '" + emrdoc.OuterXml + "' " +
            "WHERE RegistryID = '" + registryID + "'";
        string delete =
            "DELETE FROM EmrNote WHERE RegistryID='" + registryID + "' AND NoteIDSeries='" + noteIDSeries + "'";
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = update;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            ErrorLog("DeleteEmrNote", DbTables.EmrDocument, SqlOperations.Insert, ex.Message);
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        command.CommandText = delete;
        try
        {
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            ErrorLog("DeleteEmrNote", DbTables.EmrDocument, SqlOperations.Insert, ex.Message);
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Sync emr document")]
    public bool SyncEmrDocument(string registryID, string archiveNum, int status, XmlNode emrdoc)
    {
        return true;
    }

    [WebMethod(Description = "Sync emr document")]
    public string CjjSyncEmrDocument(XmlNode emrdoc, XmlNode emrnotes)
    {
        //string delete = "DELETE FROM EmrDocument WHERE RegistryID = '" + registryID + "'";
        //string insert = "INSERT INTO EmrDocument (RegistryID, ArchiveNum, Document, Status) VALUES( '"
        //    + registryID + " ','" + archiveNum + " ','" + root.OuterXml + "'," + status.ToString() + ")";

        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        CS.Open();

        SqlTransaction transaction = CS.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = CS;
        command.Transaction = transaction;
        SqlCommand command2 = command.Clone();

        #region Update notes
        command.CommandText = "DELETE FROM EmrNote WHERE RegistryID = @regID AND NoteIDSeries = @idseries";
        command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
        command.Parameters.Add("@idseries", SqlDbType.VarChar, 8);
        command.Parameters[0].Value = emrnotes.Attributes[AttributeNames.RegistryID].Value;
        command2.CommandText = "INSERT INTO EmrNote VALUES(@regID, @idseries, @note)";
        command2.Parameters.Add("@regID", SqlDbType.VarChar, 12);
        command2.Parameters.Add("@idseries", SqlDbType.VarChar, 8);
        command2.Parameters.Add("@note", SqlDbType.Xml);
        command2.Parameters[0].Value = emrnotes.Attributes[AttributeNames.RegistryID].Value;
        foreach (XmlNode note in emrnotes.SelectNodes(ElementNames.EmrNote))
        {
            command.Parameters[1].Value = note.Attributes[AttributeNames.NoteIDSeries].Value;
            try { command.ExecuteNonQuery(); }
            catch (Exception ex)
            {
                transaction.Rollback();
                CS.Close();
                ErrorLog("SyncEmrDocument", "EmrNote", "DELETE", ex.Message);
                return ex.Message;
            }
            command2.Parameters[1].Value = note.Attributes[AttributeNames.NoteIDSeries].Value;
            note.Attributes.Remove(note.Attributes[AttributeNames.NoteIDSeries]);
            command2.Parameters[2].Value = note.OuterXml;
            try { command2.ExecuteNonQuery(); }
            catch (Exception ex)
            {
                transaction.Rollback();
                CS.Close();
                ErrorLog("SyncEmrDocument", "EmrNote", "INSERT", ex.Message);
                return ex.Message;
            }
        }
        #endregion

        command.CommandText = "DELETE FROM EmrDocument WHERE RegistryID = @regID";
        command.Parameters.Clear();
        command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
        command.Parameters.Add("@archiveNum", SqlDbType.VarChar, 10);
        command.Parameters.Add("@emrdoc", SqlDbType.Xml);
        command.Parameters.Add("@status", SqlDbType.Int);
        command.Parameters[0].Value = emrnotes.Attributes[AttributeNames.RegistryID].Value;
        command.Parameters[1].Value = emrnotes.Attributes[AttributeNames.ArchiveNum].Value;
        command.Parameters[2].Value = emrdoc.OuterXml;
        command.Parameters[3].Value = Convert.ToInt16(emrdoc.Attributes[AttributeNames.EmrStatus].Value);
        try { command.ExecuteNonQuery(); }
        catch (Exception ex)
        {
            transaction.Rollback();
            CS.Close();
            ErrorLog("SyncEmrDocument", "EmrDocument", "DELETE", ex.Message);
            return ex.Message;
        }
        command.CommandText = "INSERT INTO EmrDocument VALUES(@regID, @archiveNum, @emrdoc, @status)";
        try { command.ExecuteNonQuery(); }
        catch (Exception ex)
        {
            transaction.Rollback();
            CS.Close();
            ErrorLog("SyncEmrDocument", "EmrDocument", "INSERT", ex.Message);
            return ex.Message;
        }

        transaction.Commit();
        CS.Close();
        return null;

    }

    [WebMethod(Description = "Get one EmrNote by registryID and noteIDseries")]
    public bool GetOneEmrNote(string registryID, string noteIDSeries, ref XmlNode emrNote)
    {
        string Query = "SELECT NoteDocument FROM EmrNote WHERE RegistryID = '" + registryID +
            "' AND NoteIDSeries = '" + noteIDSeries + "'";
        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        try
        {
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                if (emrNote == null)
                {
                    emrNote = doc.DocumentElement.Clone();
                }
                else
                {
                    emrNote.InnerText = doc.DocumentElement.InnerText;
                    foreach (XmlAttribute att in doc.DocumentElement.Attributes)
                    {
                        XmlAttribute attribute = emrNote.OwnerDocument.CreateAttribute(att.Name);
                        attribute.Value = att.Value;
                        emrNote.Attributes.Append(attribute);
                    }
                }
                reader.Close();
                CS.Close();
                return EmrConstant.Return.Successful;
            }
            else
            {
                ErrorLog("GetOneEmrNote", "EmrNote",
                    EmrConstant.SqlOperations.Select, EmrConstant.ErrorMessage.NoFindResult);
                return EmrConstant.Return.Failed;
            }

        }
        catch (Exception ex)
        {
            CS.Close();
            /* Error ocuured */
            ErrorLog("GetOneEmrNote", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "", EnableSession = false)]
    public string SetPrintedForEmrdocument(string registryID)
    {
        string SQLSentence =
            "UPDATE EmrDocument SET Status = 1 WHERE RegistryID = @regID";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "", EnableSession = false)]
    public string GetPrintedStatusForEmrdocument(string registryID, ref string printed)
    {
        string SQLSentence =
            "SELECT Status FROM EmrDocument WHERE RegistryID = @regID";
        printed = StringGeneral.Zero;
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) printed = reader[0].ToString();
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "", EnableSession = false)]
    public string GetRegistryIDsXquery(string criteria, ref XmlNode result)
    //  public string GetRegistryIDsXquery(string criteria)
    {

        //   XmlNode result = null;

        string select = "SELECT RegistryID FROM EmrDocument " +
            "WHERE ([Document].exist(N'/Emr/EmrNote/SubTitle[contains(., \"" + criteria + "\")]') = 1)";
        XmlDocument doc = new XmlDocument();
        XmlElement registryIDs = doc.CreateElement(ElementNames.RegistryIDs);
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(select, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                XmlElement registryID = doc.CreateElement(ElementNames.RegistryID);
                registryID.InnerText = reader[0].ToString();
                registryIDs.AppendChild(registryID);
            }
            reader.Close();
            connection.Close();
            if (registryIDs.ChildNodes.Count > 0) result = registryIDs.Clone();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get last write time of an emr document")]
    public string GetLastWriteTimeOfEmrDocument(string registryID, ref long lwt)
    {
        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        CS.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = CS;
        command.CommandText = "SELECT [Document].value('(/Emr/@Lwt)[1]', 'varchar(20)') " +
            "FROM EmrDocument WHERE RegistryID = @regID";
        command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
        command.Parameters[0].Value = registryID;

        try
        {
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) lwt = Convert.ToInt64(reader[0]);
            else lwt = 0;
        }
        catch (Exception ex)
        {
            CS.Close();
            ErrorLog("GetLastWriteTimeOfEmrDocument", "EmrDocument",
                "SELECT [Document].value('(/Emr/@Lwt)[1]', 'varchar(20)')", ex.Message);
            return ex.Message;
        }
        CS.Close();
        return null;
    }

    [WebMethod(Description = "Get EmrDocument without any EmrNote")]
    public XmlNode GetEmrDocumentWithoutNote2(string registryID)
    {

        //string SQLSentence = "SELECT CONVERT(xml, document) FROM EmrDocument WHERE RegistryID='"+ registryID + "'";
        string SQLSentence = "SELECT Document FROM EmrDocument WHERE RegistryID='" + registryID + "'";
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();

        XmlDocument emr = new XmlDocument();
        emr.LoadXml("<Emr/>");
        SqlCommand myCommand = new SqlCommand(SQLSentence, connection);
        myCommand.CommandType = CommandType.Text;
        SqlDataReader reader = myCommand.ExecuteReader();
        if (reader.Read())
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            foreach (XmlNode note in doc.DocumentElement.SelectNodes(ElementNames.EmrNote))
            {
                XmlElement emrNote = emr.CreateElement(ElementNames.EmrNote);
                string noteIDSeries = note.Attributes[AttributeNames.NoteID].Value +
                    note.Attributes[AttributeNames.Series].Value.PadLeft(6, '0');
                emrNote.SetAttribute(AttributeNames.NoteIDSeries, noteIDSeries);
                emrNote.SetAttribute(AttributeNames.NoteName, note.Attributes[AttributeNames.NoteName].Value);
                emr.DocumentElement.AppendChild(emrNote);
            }
        }
        reader.Close();
        connection.Close();
        return emr.DocumentElement;
    }


    private string MakeNoteIDSeries(string noteID, int series)
    {
        string sseries = series.ToString().PadLeft(8 - noteID.Length, '0');
        return noteID + sseries;
    }
    private void SetNode(XmlDocument xmldoc, string RegistryID, int Series)
    {
        // XmlDocument xmldoc = new XmlDocument();
        XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");

        xmldoc.AppendChild(xmlnode);
        //加入一个根元素
        XmlElement xmlelem = xmldoc.CreateElement("", "Emr", "");
        xmldoc.AppendChild(xmlelem);

        XmlNode root = xmldoc.SelectSingleNode("Emr");

        XmlAttribute xmlattr = xmldoc.CreateAttribute("RegistryID");
        xmlattr.Value = RegistryID; ;
        root.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("EmrStatus");
        xmlattr.Value = "0"; ;
        root.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("Series");
        xmlattr.Value = Series.ToString();
        root.Attributes.Append(xmlattr);

        xmlattr = xmldoc.CreateAttribute("Lwt");
        xmlattr.Value = "";
        root.Attributes.Append(xmlattr);

        // return root;

    }

    [WebMethod(Description = "Add a new EmrNote")]
    public string NewEmrNoteEx(string registryID, string ArchieveNum, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, ref int Series)
    {

        //        //SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        //        //CS.Open();

        //        SqlHelper Helper = new SqlHelper("EmrDB");
        //        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        //        Series = -1;
        //        DataTable dt = Helper.GetDataTable(strSelectDocument);



        //        try
        //        {
        //        if (dt != null && dt.Rows.Count == 0)
        //        {
        //            Series = 1;
        //            XmlDocument xmldoc = new XmlDocument(); 
        //           SetNode(xmldoc,registryID, Series);
        //           string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
        //           noteForEmrDoc.Attributes["Series"].Value = Series.ToString();        


        //            XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
        //            XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

        //            Root.AppendChild(Node);
        //            string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
        //                                Values('" + registryID + "','" + ArchieveNum + "','" + Root.OuterXml.ToString() + "','0')";


        //            string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
        //            string[] strList = { Insert, strInsert };

        //            if (Helper.ExecuteSqlByTran(strList) == true)
        //                return null;
        //            else
        //                return "Error";



        //        }
        //        else
        //        {

        //         SqlTransaction tran;
        //         SqlCommand cmd = new SqlCommand();
        //         using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
        //         {
        //             try
        //             {
        //                 con.Open();
        //                 tran = con.BeginTransaction();
        //                 try
        //                 {
        //                     string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
        //                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
        //                     cmd.CommandText = strSelect;
        //                     cmd.Connection = tran.Connection;
        //                     cmd.Transaction = tran;
        //                     SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                     DataSet ds = new DataSet();
        //                     da.Fill(ds);
        //                     da.Dispose();
        //                     cmd.Dispose();
        //                    DataTable  dtSeries = ds.Tables[0];
        //                     if (dtSeries != null && dtSeries.Rows.Count != 0)
        //                     {
        //                         string str = dtSeries.Rows[0]["Series"].ToString();
        //                         Series = Convert.ToInt32(str);
        //                         Series = Series + 1;
        //                     }
        //                     noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
        //                     string strUpdate = @"update EmrDocument set document.modify
        //                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
        //                     cmd.CommandText = strUpdate;
        //                     cmd.Connection = tran.Connection;
        //                     cmd.Transaction = tran;
        //                     cmd.ExecuteNonQuery();
        //                     tran.Commit();
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     tran.Rollback();
        //                     tran.Dispose();
        //                     throw (e);
        //                 }
        //             }
        //             catch (Exception ex)
        //             {
        //                 LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

        //             }
        //             finally
        //             {
        //                 cmd.Dispose();
        //                 con.Dispose();
        //             }
        //         }

        ////            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
        ////                                from EmrDocument with (holdlock)  where RegistryID='" + registryID + "'";
        ////            DataTable dtSeries = Helper.GetDataTable(strSelect);



        ////            if (dtSeries != null && dtSeries.Rows.Count != 0)
        ////            {
        ////                string str = dtSeries.Rows[0]["Series"].ToString();
        ////                Series = Convert.ToInt32(str);
        ////                Series = Series + 1;
        ////            }

        ////            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();

        //////            string strUpdate = @"update EmrDocument set document.modify
        //////                                ('replace value of (/Emr[@Series]/@Series)[1] with '"+Series.ToString()+"' ')  where RegistryID='"+registryID+"'";

        ////            string strUpdate = @"update EmrDocument set document.modify
        ////                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
        ////            Helper.ExecuteSqlByTran(strUpdate);






        //            string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


        //            string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
        //            string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
        //            string[] strList = { Insert, strUpdateNode };
        //            if (Helper.ExecuteSqlByTran(strList) == true)
        //                return null;
        //            else
        //                return "Error";



        //        }   
        //        }
        //        catch (Exception ex)
        //        {
        //            /* Error ocuured */

        //            return ex.Message + "-" + ex.Source;
        //        }

        string strError = "未知异常";
        int iSeries = -1;
        /*下面代码为插入一个新纪录做准备，如果不需要插入新纪录，则下列代码不会再SQL语句中运行*/
        XmlDocument xmldoc = new XmlDocument();
        SetNode(xmldoc, registryID, 1);
        string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, 1);
        noteForEmrDoc.Attributes["Series"].Value = "-1000";
        XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
        XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);
        Root.AppendChild(Node);
        string strSql = "";
        try
        {
            string strTemp = @"
begin tran
	begin try
		declare @registryID varchar(12)
		declare @Series  int
        declare @SeriesAddOne int
		declare @XACT_STATE int
        declare @check nvarchar(50)
		------此段定义变量用来计算NoteIDSeries开始------
		declare @NoteIDSeries  nvarchar(8)
		declare @NoteIDSeriesLength int
		set @NoteIDSeriesLength = 8        
		declare @NoteIDSeriesBlank nchar(1)
		set @NoteIDSeriesBlank = '0'
		declare @noteID nvarchar(8)
		set @noteID = '{4}'
		------此段定义变量用来计算NoteIDSeries结束------
        set @registryID = '{0}'
		if not exists(select * from emrdocument where registryID =  @registryID)     --EmrDocument表中不存在此患者的基本信息，则插入一条默认信息
		begin
		    set @Series = 1
            set @SeriesAddOne = 1
			Insert into EmrDocument ([registryID],[ArchiveNum],[Document],[status]) 
			Values('{0}','{1}','{5}','0')
			
			--计算NoteIDSeries开始
			select @NoteIDSeries= @noteID+replicate(0,@NoteIDSeriesLength-len(@noteID)-len(convert(nvarchar(8),@Series)) )+convert(nvarchar(8),@Series)
			--计算NoteIDSeries结束

			--Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('{0}',@NoteIDSeries,'{2}')
		end
		else                       --EmrDocument表中存在此患者的基本信息，则插入一条默认信息
		begin
			select @Series = Document.value('(Emr/@Series)[1]','varchar(50)')
            from EmrDocument with (rowlock holdlock)  where RegistryID='{0}'
            if @Series is null
            begin
				RAISERROR ('数据插入失败：未发现患者{0} 信息的@Series节点。' , 16, 1) WITH NOWAIT
            end
            else
            begin
                set @SeriesAddOne = @Series + 1
                update EmrDocument set document.modify('replace value of (/Emr[@Series]/@Series)[1] with sql:variable( " + "\"@SeriesAddOne\"" + @") ')  where RegistryID='{0}'
                update emrdocument with(holdlock) SET document.modify ('insert {3} as last into (/Emr[1])' )    where RegistryID='{0}'
            end
		end
		
        update emrdocument  SET document.modify('replace value of (/Emr[1]/EmrNote[@Series=-1000]/@Series)[1] with sql:variable( " + "\"@SeriesAddOne\"" + @")') where RegistryID='{0}' 
					--计算NoteIDSeries开始
			select @NoteIDSeries= @noteID+replicate(0,@NoteIDSeriesLength-len(@noteID)-len(convert(nvarchar(8),@SeriesAddOne)) )+convert(nvarchar(8),@SeriesAddOne)
			--计算NoteIDSeries结束
		Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('{0}',@NoteIDSeries,'{2}')
        --开始进行插入检测
        select @check = Document.value('(Emr/EmrNote/@Series = sql:variable( " + "\"@SeriesAddOne\"" + @"))','nvarchar(50)')  from EmrDocument 
		where RegistryID = '{0}'
	    if @check = 'true'            --EmrDocument表中信息是否插入成功
	    begin
            if exists(select * from emrnote                   --EmrNote表中信息是否插入成功
					  where RegistryID = @registryID
					  and NoteIDSeries = @NoteIDSeries)      
			begin
                set @XACT_STATE = XACT_STATE()
				if(@XACT_STATE = 1)
				begin
                    commit tran
					select @SeriesAddOne as Series , null as ErrMessage
					return
				end
				else
				begin
					RAISERROR ('XACT_STATE()值不等于1,事物无法提交',16,1) WITH NOWAIT
				end
				
			end
			else
			begin
				RAISERROR ('数据插入检查失败：在emrNote表中未发现患者{0}' , 16, 1) WITH NOWAIT
			end
	        
	    end
	    else
	    begin
			RAISERROR ('数据插入检查失败：在emrDocument表中未发现患者{0}' , 16, 1) WITH NOWAIT
	    end
        RAISERROR ('病例未能正确保存' , 16, 1) WITH NOWAIT
end try
begin catch
rollback tran
select -1 as Series ,ERROR_Message() as ErrMessage
end catch
";
            strSql = String.Format(strTemp, registryID, ArchieveNum, noteForWordDoc.OuterXml.ToString(), noteForEmrDoc.OuterXml.ToString(), noteForEmrDoc.Attributes["NoteID"].Value, Root.OuterXml.ToString());
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
            {
                DataSet ds = new DataSet();
                con.Open();
                SqlCommand cmd = con.CreateCommand();

                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sqlAda = new SqlDataAdapter(cmd);
                sqlAda.Fill(ds);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    iSeries = Convert.ToInt32(ds.Tables[0].Rows[0]["Series"]);
                    strError = Convert.ToString(ds.Tables[0].Rows[0]["ErrMessage"]);
                }
                else
                {
                    Series = -1;
                    return "保存数据失败";
                }
            }
        }
        catch (Exception ex)
        {
            Series = -1;
            return ex.Message + "-" + ex.Source;
        }
        finally
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    if (strError == null)
                    {
                        strError = "";
                    }
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    string strSqlLog = "INSERT INTO [emr].[dbo].[test_EmrNoteEmrDocumentLog]([registryid],[archiveNum],[SqlString],[iSeries],[Err],[logTime])VALUES('" + registryID.Replace("'", "''") + "'," + ArchieveNum.Replace("'", "''") + ",'" + strSql.Replace("'", "''") + "'," + iSeries.ToString() + ",'" + strError.Replace("'", "''") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    cmd.CommandText = strSqlLog;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }
        if (iSeries < 0)
        {
            Series = iSeries;
            return strError;
        }
        else
        {
            Series = iSeries;
            return null;
        }
    }
    [WebMethod(Description = "Add a new EmrNote")]
    public string NewEmrNoteExZ(string registryID, string ArchieveNum, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, XmlNode noteForEmrXml, ref int Series)
    {
        //SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        //CS.Open();

        //        SqlHelper Helper = new SqlHelper("EmrDB");
        //        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        //        Series = -1;
        //        DataTable dt = Helper.GetDataTable(strSelectDocument);



        //        try
        //        {
        //            if (dt != null && dt.Rows.Count == 0)
        //            {
        //                Series = 1;
        //                XmlDocument xmldoc = new XmlDocument();
        //                SetNode(xmldoc, registryID, Series);
        //                string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
        //                noteForEmrDoc.Attributes["Series"].Value = Series.ToString();


        //                XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
        //                XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

        //                Root.AppendChild(Node);
        //                string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
        //                                Values('" + registryID + "','" + ArchieveNum + "','" + Root.OuterXml.ToString() + "','0')";


        //                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument,EmrXml) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "','" + noteForEmrXml.OuterXml + "')";
        //                string[] strList = { Insert, strInsert };

        //                if (Helper.ExecuteSqlByTran(strList) == true)
        //                    return null;
        //                else
        //                    return "Error";



        //            }
        //            else
        //            {

        //                SqlTransaction tran;
        //                SqlCommand cmd = new SqlCommand();
        //                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
        //                {
        //                    try
        //                    {
        //                        con.Open();
        //                        tran = con.BeginTransaction();
        //                        try
        //                        {
        //                            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
        //                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
        //                            cmd.CommandText = strSelect;
        //                            cmd.Connection = tran.Connection;
        //                            cmd.Transaction = tran;
        //                            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                            DataSet ds = new DataSet();
        //                            da.Fill(ds);
        //                            da.Dispose();
        //                            cmd.Dispose();
        //                            DataTable dtSeries = ds.Tables[0];
        //                            if (dtSeries != null && dtSeries.Rows.Count != 0)
        //                            {
        //                                string str = dtSeries.Rows[0]["Series"].ToString();
        //                                Series = Convert.ToInt32(str);
        //                                Series = Series + 1;
        //                            }
        //                            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
        //                            string strUpdate = @"update EmrDocument set document.modify
        //                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
        //                            cmd.CommandText = strUpdate;
        //                            cmd.Connection = tran.Connection;
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            tran.Commit();
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            tran.Rollback();
        //                            tran.Dispose();
        //                            throw (e);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

        //                    }
        //                    finally
        //                    {
        //                        cmd.Dispose();
        //                        con.Dispose();
        //                    }
        //                }

        //                //            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
        //                //                                from EmrDocument with (holdlock)  where RegistryID='" + registryID + "'";
        //                //            DataTable dtSeries = Helper.GetDataTable(strSelect);



        //                //            if (dtSeries != null && dtSeries.Rows.Count != 0)
        //                //            {
        //                //                string str = dtSeries.Rows[0]["Series"].ToString();
        //                //                Series = Convert.ToInt32(str);
        //                //                Series = Series + 1;
        //                //            }

        //                //            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();

        //                ////            string strUpdate = @"update EmrDocument set document.modify
        //                ////                                ('replace value of (/Emr[@Series]/@Series)[1] with '"+Series.ToString()+"' ')  where RegistryID='"+registryID+"'";

        //                //            string strUpdate = @"update EmrDocument set document.modify
        //                //                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
        //                //            Helper.ExecuteSqlByTran(strUpdate);






        //                string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


        //                string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
        //                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
        //                string[] strList = { Insert, strUpdateNode };
        //                if (Helper.ExecuteSqlByTran(strList) == true)
        //                    return null;
        //                else
        //                    return "Error";



        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            /* Error ocuured */
        //            return ex.Message + "-" + ex.Source;
        //        }
        string strError = "未知异常";
        int iSeries = -1;
        /*下面代码为插入一个新纪录做准备，如果不需要插入新纪录，则下列代码不会再SQL语句中运行*/
        XmlDocument xmldoc = new XmlDocument();
        SetNode(xmldoc, registryID, 1);
        string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, 1);
        noteForEmrDoc.Attributes["Series"].Value = "-1000";
        XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
        XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);
        Root.AppendChild(Node);
        string strSql = "";
        try
        {
            string strTemp = @"
begin tran
	begin try
		
		declare @registryID varchar(12)
		declare @Series  int
        declare @SeriesAddOne int
		declare @XACT_STATE int
        declare @check nvarchar(50)
		------此段定义变量用来计算NoteIDSeries开始------
		declare @NoteIDSeries  nvarchar(8)
		declare @NoteIDSeriesLength int
		set @NoteIDSeriesLength = 8        
		declare @NoteIDSeriesBlank nchar(1)
		set @NoteIDSeriesBlank = '0'
		declare @noteID nvarchar(8)
		set @noteID = '{4}'
		------此段定义变量用来计算NoteIDSeries结束------
        set @registryID = '{0}'
		if not exists(select * from emrdocument where registryID =  @registryID)     --EmrDocument表中不存在此患者的基本信息，则插入一条默认信息
		begin
		    set @Series = 1
            set @SeriesAddOne = 1
			Insert into EmrDocument ([registryID],[ArchiveNum],[Document],[status]) 
			Values('{0}','{1}','{5}','0')
			
			--计算NoteIDSeries开始
			select @NoteIDSeries= @noteID+replicate(0,@NoteIDSeriesLength-len(@noteID)-len(convert(nvarchar(8),@Series)) )+convert(nvarchar(8),@Series)
			--计算NoteIDSeries结束

			--Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument,EmrXml) values('{0}',@NoteIDSeries,'{2}','{6}')
		end
		else                       --EmrDocument表中存在此患者的基本信息，则插入一条默认信息
		begin
			select @Series = Document.value('(Emr/@Series)[1]','varchar(50)')
            from EmrDocument with (rowlock holdlock)  where RegistryID='{0}'
            if @Series is null
            begin
				RAISERROR ('数据插入失败：未发现患者{0} 信息的@Series节点。' , 16, 1) WITH NOWAIT
            end
            else
            begin
                set @SeriesAddOne = @Series + 1
                update EmrDocument set document.modify('replace value of (/Emr[@Series]/@Series)[1] with sql:variable( " + "\"@SeriesAddOne\"" + @") ')  where RegistryID='{0}'
                update emrdocument with(holdlock) SET document.modify ('insert {3} as last into (/Emr[1])' )    where RegistryID='{0}'
            end
		end
		
        update emrdocument  SET document.modify('replace value of (/Emr[1]/EmrNote[@Series=-1000]/@Series)[1] with sql:variable( " + "\"@SeriesAddOne\"" + @")') where RegistryID='{0}' 
			--计算NoteIDSeries开始
			select @NoteIDSeries= @noteID+replicate(0,@NoteIDSeriesLength-len(@noteID)-len(convert(nvarchar(8),@SeriesAddOne)) )+convert(nvarchar(8),@SeriesAddOne)
			--计算NoteIDSeries结束
		Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument,EmrXml) values('{0}',@NoteIDSeries,'{2}','{6}')
        --开始进行插入检测
        select @check = Document.value('(Emr/EmrNote/@Series = sql:variable( " + "\"@SeriesAddOne\"" + @"))','nvarchar(50)')  from EmrDocument 
		where RegistryID = '{0}'
	    if @check = 'true'            --EmrDocument表中信息是否插入成功
	    begin
            if exists(select * from emrnote                   --EmrNote表中信息是否插入成功
					  where RegistryID = @registryID
					  and NoteIDSeries = @NoteIDSeries)      
			begin
                set @XACT_STATE = XACT_STATE()
				if(@XACT_STATE = 1)
				begin
                    commit tran
					select @SeriesAddOne as Series , null as ErrMessage
					return
				end
				else
				begin
					RAISERROR ('XACT_STATE()值不等于1,事物无法提交',16,1) WITH NOWAIT
				end
				
			end
			else
			begin
				RAISERROR ('数据插入检查失败：在emrnote表中未发现患者{0}' , 16, 1) WITH NOWAIT
			end
	        
	    end
	    else
	    begin
			RAISERROR ('数据插入检查失败：在EmrDocument表中未发现患者{0}' , 16, 1) WITH NOWAIT
	    end
	    RAISERROR ('病例未能正确保存' , 16, 1) WITH NOWAIT
end try
begin catch
rollback tran
select -1 as Series ,ERROR_Message() as ErrMessage
end catch
";
            strSql = String.Format(strTemp, registryID, ArchieveNum, noteForWordDoc.OuterXml.ToString(), noteForEmrDoc.OuterXml.ToString(), noteForEmrDoc.Attributes["NoteID"].Value, Root.OuterXml.ToString(), noteForEmrXml.OuterXml.ToString());
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
            {
                DataSet ds = new DataSet();
                con.Open();
                SqlCommand cmd = con.CreateCommand();

                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sqlAda = new SqlDataAdapter(cmd);
                sqlAda.Fill(ds);
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    iSeries = Convert.ToInt32(ds.Tables[0].Rows[0]["Series"]);
                    strError = Convert.ToString(ds.Tables[0].Rows[0]["ErrMessage"]);
                }
                else
                {
                    Series = -1;
                    return "保存数据失败";
                }
            }
        }
        catch (Exception ex)
        {
            Series = -1;
            return ex.Message + "-" + ex.Source;
        }
        finally
        {
            try
            {
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    string strSqlLog = "INSERT INTO [emr].[dbo].[test_EmrNoteEmrDocumentLog]([registryid],[archiveNum],[SqlString],[iSeries],[Err],[logTime])VALUES('" + registryID.Replace("'", "''") + "'," + ArchieveNum.Replace("'", "''") + ",'" + strSql.Replace("'", "''") + "'," + iSeries.ToString() + ",'" + strError.Replace("'", "''") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    cmd.CommandText = strSqlLog;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }
        if (iSeries < 0)
        {
            Series = iSeries;
            return strError;
        }
        else
        {
            Series = iSeries;
            return null;
        }
    }
    [WebMethod(Description = "Insert or Replace attribute of /Emr[1] in Emrdocument")]
    public string InsertOrReplaceAttributeOfEmrdocument_Emr(string registryID, XmlElement xmlEle)
    {
        string strXmlNodeAttrName = "";
        string strXmlNodeAttrValue = "";
        string strError = "未知异常";
        using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(@"begin tran
                                                                               begin try
                            declare @attrName nvarchar(100) 
                             ");
                foreach (XmlAttribute tempXmlAttr in xmlEle.Attributes)
                {
                    strXmlNodeAttrName = tempXmlAttr.LocalName;
                    strXmlNodeAttrValue = tempXmlAttr.Value;
                    if (strXmlNodeAttrName.ToLower().Equals("xmlns"))
                    {
                        continue;
                    }
                    if (strXmlNodeAttrName.Length > 100)
                    {
                        strError = "更新数据失败:XML节点名过长";
                        return strError;
                    }
                    sb.Append(@" select @attrName = document.value('(/Emr[1]/@" + strXmlNodeAttrName + @")','nvarchar(100)')
                            from emrdocument
                            where registryid = '" + registryID + @"'
                            if @attrName is not null
                            begin
	                            --存在对应的属性，需要更新
	                            update emrdocument
	                            set document.modify('replace value of (/Emr[1]/@" + strXmlNodeAttrName + @")[1] with " + "\"" + strXmlNodeAttrValue + "\"')" + @"
	                            where registryid = '" + registryID + @"'
                            end
                            else
                            begin
	                            --不存在对应的属性，需要插入
	                            update emrdocument 
	                            set document.modify('insert attribute " + strXmlNodeAttrName + @" {" + "\"" + strXmlNodeAttrValue + "\"" + @"} into (/Emr[1])')
	                            where registryid = '" + registryID + @"'
                            end
               ");
                }
                sb.Append(@"
                            if(@@error = 0)
                            begin
	                            if(XACT_STATE()=1)
	                            begin
		                            commit
		                            return
	                            end
                            end
                            rollback
                            end try
                            begin catch
                            rollback
                            end catch
                            "
                           );
                string strSql = sb.ToString();
                //strSql = String.Format(strSql, strXmlNodeAttrName, strXmlNodeAttrValue, registryID);
                DataSet ds = new DataSet();
                cmd.CommandText = strSql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                cmd.Clone();
            }
            return null;
        }
    }
    [WebMethod(Description = "Update an EmrNote")]
    public string UpdateEmrNoteEx(string registryID, XmlNode noteForEmrDoc, XmlNode noteForWordDoc)
    {


        //SqlHelper Helper = new SqlHelper("EmrDB");
        //string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        //string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        //try
        //{
        //string NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
        //string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
        //string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  /Emr/EmrNote[@Series=" + Series + "]') where  RegistryID='" + registryID + "'";
        //string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";
        //string[] strList = { strUpdateNote, strDeleteDocument, strUpdateDocument };
        //if (Helper.ExecuteSqlByTran(strList) == true)
        //    return null;
        //else
        //    return "Error";

        //}
        //catch (Exception ex)
        //{
        //    /* Error ocuured */
        //    return ex.Message + "-" + ex.Source;
        //}


        SqlHelper Helper = new SqlHelper("EmrDB");

        string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        try
        {
            string NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
            string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
            string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";
            string[] strList = { strUpdateNote, strUpdateDocument, strDeleteDocument };
            if (Helper.ExecuteSqlByTran(strList) == true)
                return null;
            else
                return "Error";

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }







    }
    [WebMethod(Description = "Update an EmrNote")]
    public string UpdateEmrNoteExZ(string registryID, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, XmlNode EmrXml)
    {




        SqlHelper Helper = new SqlHelper("EmrDB");
        string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        try
        {
            string NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
            string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            string strUpdateNotexml = @"Update EmrNote set EmrXml = '" + EmrXml.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
            string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";
            string[] strList = { strUpdateNote, strUpdateDocument, strDeleteDocument, strUpdateNotexml };
            if (Helper.ExecuteSqlByTran(strList) == true)
                return null;
            else
                return "Error";

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }







    }

    [WebMethod(Description = "Delete an EmrNote")]
    public string DeleteEmrNoteEx(string registryID, string noteID, int series)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string NoteIDSeries = MakeNoteIDSeries(noteID, series);
        //string strSelectNote = @"select EmrNote from EmrDocument where registryID = '" + registryID + "'";
       // Series = -1;
       // DataTable dt = Helper.GetDataTable(strSelectDocument);



        try
        {
            //if (dt != null && dt.Rows.Count == 0)
            //{

                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  /Emr/EmrNote[@Series=" + series + "]') where  RegistryID='" + registryID + "'";
                string strDeleteNote = "delete from EmrNote where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //string strInsertNote = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string[] strList = { strDeleteDocument, strDeleteNote};
                if (Helper.ExecuteSqlByTran(strList) == true)
                {
                    return null;
                }
                else
                {
                    return "Error";
                }
            //}
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }


    [WebMethod]
    public string UncommitEmrNoteEx(string registryID, string Opcode,
                    string departmentCode, string noteID, int series, XmlNode reasion, EmrConstant.NoteStatus status)
    {

        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strInsert = @"Insert UncommitEmrNote (RegistryID,Opcode,DeparmentCode,NoteID,Series,reasion,Opdate) 
            Values('" + registryID + "','" + Opcode + "','" + departmentCode + "','" + noteID + "','" + series + "','" + reasion.OuterXml.ToString() + "','" + DateTime.Now + "')";

            string strUpdate = @"Update EmrDocument set document.modify
                        ('replace value of (/Emr/EmrNote[@Series=" + series + "]/@NoteStatus)[1] with " + Convert.ToInt32(status) + "')  where RegistryID='" + registryID + "'";

            string[] strList = { strInsert, strUpdate };
            if (Helper.ExecuteSqlByTran(strList) == true)
            {
                return null;
            }
            else
            {
                return "Error";
            }

        }
        catch (Exception ex)
        {
            return ex.Message + "-" + ex.Source;
        }
    }



    /// <summary>
    /// just for digital sign
    /// </summary>
    /// <param name="registryID"></param>
    /// <param name="ArchieveNum"></param>
    /// <param name="noteForEmrDoc"></param>
    /// <param name="noteForWordDoc"></param>
    /// <param name="Series"></param>
    /// <returns></returns>
    [WebMethod(Description = "Add a new EmrNote")]
    public string NewEmrNoteExExa(string registryID, string ArchieveNum, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, string Sign, string strOpcode, ref int Series, ref string NoteIDSeries, string UniqueID)
    {


        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        Series = -1;
        DataTable dt = Helper.GetDataTable(strSelectDocument);



        try
        {
            if (dt != null && dt.Rows.Count == 0)
            {
                Series = 1;
                XmlDocument xmldoc = new XmlDocument();
                SetNode(xmldoc, registryID, Series);
                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                noteForEmrDoc.Attributes["Series"].Value = Series.ToString();


                XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
                XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

                Root.AppendChild(Node);
                string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
                                Values('" + registryID + "','" + ArchieveNum + "','" + Root.OuterXml.ToString() + "','0')";


                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string InsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + UniqueID + "')";
                string[] strList = { Insert, strInsert, InsertSign };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error1";



            }
            else
            {

                SqlTransaction tran;
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    try
                    {
                        con.Open();
                        tran = con.BeginTransaction();
                        try
                        {
                            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strSelect;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            cmd.Dispose();
                            DataTable dtSeries = ds.Tables[0];
                            if (dtSeries != null && dtSeries.Rows.Count != 0)
                            {
                                string str = dtSeries.Rows[0]["Series"].ToString();
                                Series = Convert.ToInt32(str);
                                Series = Series + 1;
                            }
                            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
                            string strUpdate = @"update EmrDocument set document.modify
                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strUpdate;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            tran.Dispose();
                            throw (e);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Dispose();
                    }
                }




                string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + UniqueID + "')";
                string[] strList = { Insert, strUpdateNode, strInsertSign };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error2";



            }
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }
    //新添加方法北京CA验证 2012-05-08
    // [WebMethod(Description = " Verify Cert And UserID   验证北京CA证书与医师编码匹配信息")]
    ////public void VerifyCertAndUserID(string certID,ref string opCode,ref string bjca_sfzh,ref string bjca_image,ref string certContent)
    // {

    //   SqlHelper Helper = new SqlHelper("HisDB");
    //   string strSql = "SELECT YSBM FROM tysm_bjcazs WHERE zsxlh ='"+certID+"'";
    //   DataTable dt = Helper.GetDataTable(strSql);
    //   if (dt != null && dt.Rows.Count == 1)
    //   {
    //       opCode = dt.Rows[0].ItemArray[0].ToString();
    //  }

    // }
    //新添加方法北京CA验证 2012-05-08 查询参数身份证号
    [WebMethod(Description = " Verify Cert And UserID   验证北京CA证书与医师编码匹配信息")]
    public void VerifyCertAndUserID(string certID, ref string opCode)
    {

        SqlHelper Helper = new SqlHelper("HisDB");
        string strSql = "SELECT YSBM FROM tysm  WHERE bjca_sfzh  ='" + certID + "'";
        try
        {
            DataTable dt = Helper.GetDataTable(strSql);
            if (dt != null && dt.Rows.Count == 1)
            {
                opCode = dt.Rows[0].ItemArray[0].ToString();
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    //2012-06-29 LiuQi 删除审批个人模板
    [WebMethod]
    public void DelTemplateDetail(string DoctorID, string pk)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string del = "Delete FROM TD_NoteTemplate WHERE departmentcode = '####' AND doctorid = '" + DoctorID + "' AND Ischecked IS NULL AND PK = '" + pk + "'";
        DataTable dt = new DataTable();
        try
        {
            Helper.ExecuteSql(del);

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
        return;
    }

    [WebMethod(Description = "Add a new EmrNote")]
    public string NewEmrNoteExExaZ(string registryID, string ArchieveNum, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, XmlNode noteForEmrXml, string Sign, string strOpcode, ref int Series, ref string NoteIDSeries)
    {


        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        Series = -1;
        DataTable dt = Helper.GetDataTable(strSelectDocument);



        try
        {
            if (dt != null && dt.Rows.Count == 0)
            {
                Series = 1;
                XmlDocument xmldoc = new XmlDocument();
                SetNode(xmldoc, registryID, Series);
                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                noteForEmrDoc.Attributes["Series"].Value = Series.ToString();


                XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
                XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

                Root.AppendChild(Node);
                string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
                                Values('" + registryID + "','" + ArchieveNum + "','" + Root.OuterXml.ToString() + "','0')";


                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument,EmrXml) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "'" + noteForEmrXml.OuterXml + "')";
                string InsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strInsert, InsertSign };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error1";
            }
            else
            {

                SqlTransaction tran;
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    try
                    {
                        con.Open();
                        tran = con.BeginTransaction();
                        try
                        {
                            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strSelect;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            cmd.Dispose();
                            DataTable dtSeries = ds.Tables[0];
                            if (dtSeries != null && dtSeries.Rows.Count != 0)
                            {
                                string str = dtSeries.Rows[0]["Series"].ToString();
                                Series = Convert.ToInt32(str);
                                Series = Series + 1;
                            }
                            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
                            string strUpdate = @"update EmrDocument set document.modify
                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strUpdate;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            tran.Dispose();
                            throw (e);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Dispose();
                    }
                }




                string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strUpdateNode, strInsertSign };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error2";



            }
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }

    /// <summary>
    /// use for digital sign by ca
    /// </summary>
    /// <param name="registryID"></param>
    /// <param name="ArchieveNum"></param>
    /// <param name="noteForEmrDoc"></param>
    /// <param name="noteForWordDoc"></param>
    /// <param name="Sign"></param>
    /// <param name="strOpcode"></param>
    /// <param name="Series"></param>
    /// <returns></returns>
    [WebMethod(Description = "Add a new EmrNote for ca")]
    public string NewEmrNoteExEx(string registryID, string ArchieveNum, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, string Sign, string strOpcode, ref int Series)
    {
        string NoteIDSeries = "";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        Series = -1;
        DataTable dt = Helper.GetDataTable(strSelectDocument);



        try
        {
            if (dt != null && dt.Rows.Count == 0)
            {
                Series = 1;
                XmlDocument xmldoc = new XmlDocument();
                SetNode(xmldoc, registryID, Series);
                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                noteForEmrDoc.Attributes["Series"].Value = Series.ToString();


                XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
                XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

                Root.AppendChild(Node);
                string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
                                Values('" + registryID + "','" + ArchieveNum + "','" + Root.OuterXml.ToString() + "','0')";


                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string InsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strInsert, InsertSign };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";



            }
            else
            {

                SqlTransaction tran;
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    try
                    {
                        con.Open();
                        tran = con.BeginTransaction();
                        try
                        {
                            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strSelect;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            cmd.Dispose();
                            DataTable dtSeries = ds.Tables[0];
                            if (dtSeries != null && dtSeries.Rows.Count != 0)
                            {
                                string str = dtSeries.Rows[0]["Series"].ToString();
                                Series = Convert.ToInt32(str);
                                Series = Series + 1;
                            }
                            noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
                            string strUpdate = @"update EmrDocument set document.modify
                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strUpdate;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            tran.Dispose();
                            throw (e);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Dispose();
                    }
                }




                string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


                NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc.OuterXml + "')";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strUpdateNode, strInsertSign };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";



            }
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }

    /// <summary>
    /// use for digital sign by ca
    /// </summary>
    /// <param name="registryID"></param>
    /// <param name="noteForEmrDoc"></param>
    /// <param name="noteForWordDoc"></param>
    /// <param name="Sign"></param>
    /// <param name="strOpcode"></param>
    /// <param name="NoteIDSeries"></param>
    /// <returns></returns>
    [WebMethod(Description = "Update an EmrNote")]
    public string UpdateEmrNoteExEx(string registryID, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, string Sign, string strOpcode)
    {
        string NoteIDSeries = "";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        try
        {
            NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
            string strSelect = "select pk from EmrNote where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";

                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";

                string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                DataTable dt1 = Helper.GetDataTable(strSelect1);
                if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                else strDealSign = strInsertSign;
                string[] strList = { strUpdateNote, strDealSign, strUpdateDocument, strDeleteDocument };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

            else
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";

                string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                DataTable dt1 = Helper.GetDataTable(strSelect1);
                if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                else strDealSign = strInsertSign;

                string[] strList = { strUpdateNote, strUpdateSign, strUpdateDocument, strDeleteDocument };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }

    }

    [WebMethod(Description = "Update an EmrNote for ca01")]
    public string UpdateEmrNoteExExa(string registryID, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, string Sign, string strOpcode, ref string NoteIDSeries, string certID)
    {


        SqlHelper Helper = new SqlHelper("EmrDB");
        string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        try
        {
            NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
            string strSelect = "select pk from EmrNote where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + certID + "')";
                // string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";

                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";

                // string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                // DataTable dt1 = Helper.GetDataTable(strSelect1);
                // if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                // else 
                strDealSign = strInsertSign;
                string[] strList = { strUpdateNote, strDealSign, strUpdateDocument, strDeleteDocument };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

            else
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + certID + "')";

                //  string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";

                //string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //DataTable dt1 = Helper.GetDataTable(strSelect1);
                //if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                //else
                strDealSign = strInsertSign;

                string[] strList = { strUpdateNote, strDealSign, strUpdateDocument, strDeleteDocument };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }

    }

    [WebMethod(Description = "Update an EmrNote for ca01")]
    public string UpdateEmrNoteExExaZ(string registryID, XmlNode noteForEmrDoc, XmlNode noteForWordDoc, XmlNode xmlForEmrXml, string Sign, string strOpcode, ref string NoteIDSeries, string certID)
    {


        SqlHelper Helper = new SqlHelper("EmrDB");
        string Series = noteForEmrDoc.Attributes["Series"].Value.ToString();
        string NodeId = noteForEmrDoc.Attributes["NoteID"].Value.ToString();
        try
        {
            NoteIDSeries = MakeNoteIDSeries(NodeId, Convert.ToInt16(Series));
            string strSelect = "select pk from EmrNote where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                // string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                // string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + certID + "')";

                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";
                string strUpdateNotexml = @"Update EmrNote set EmrXml = '" + xmlForEmrXml.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //DataTable dt1 = Helper.GetDataTable(strSelect1);
                //if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                //else 
                strDealSign = strInsertSign;
                string[] strList = { strUpdateNote, strDealSign, strUpdateDocument, strDeleteDocument, strUpdateNotexml };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

            else
            {
                string strDealSign;
                string strUpdateNote = @"Update EmrNote set NoteDocument = '" + noteForWordDoc.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                // string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                // string strUpdateSign = @"Update signContent set ciphertext = '" + Sign + "',signer = '" + strOpcode + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer,CertID) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "','" + certID + "')";

                string strUpdateDocument = @"update EmrDocument SET document.modify ('insert " + noteForEmrDoc.OuterXml + " after (/Emr/EmrNote[@Series=" + Series + "])[1]' )    where RegistryID='" + registryID + "'";
                string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  (/Emr/EmrNote[@Series=" + Series + "])[1]') where  RegistryID='" + registryID + "'";
                string strUpdateNotexml = @"Update EmrNote set EmrXml = '" + xmlForEmrXml.OuterXml.ToString() + "' where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //string strSelect1 = "select * from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
                //DataTable dt1 = Helper.GetDataTable(strSelect1);
                //if (dt1 != null && dt1.Rows.Count != 0) strDealSign = strUpdateSign;
                //else 
                strDealSign = strInsertSign;

                string[] strList = { strUpdateNote, strDealSign, strUpdateDocument, strDeleteDocument, strUpdateNotexml };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";
            }

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }

    }

    [WebMethod(Description = "Delete an EmrNote")]
    public string DeleteEmrNoteExEx(string registryID, string noteID, int series)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string NoteIDSeries = MakeNoteIDSeries(noteID, series);

        try
        {
            string strDeleteDocument = "UPDATE EmrDocument set document.modify('delete  /Emr/EmrNote[@Series=" + series + "]') where  RegistryID='" + registryID + "'";
            string strDeleteNote = "delete from EmrNote where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            string strDeleteSign = "delete from signContent where  RegistryID='" + registryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            string[] strList = { strDeleteDocument, strDeleteNote, strDeleteSign };
            if (Helper.ExecuteSqlByTran(strList) == true)
            {
                return null;
            }
            else
            {
                return "Error";
            }

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }


    [WebMethod]
    public string UncommitEmrNoteExEx(string registryID, string Opcode,
                    string departmentCode, string noteID, int series, XmlNode reasion, EmrConstant.NoteStatus status)
    {

        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strInsert = @"Insert UncommitEmrNote (RegistryID,Opcode,DeparmentCode,NoteID,Series,reasion,Opdate) 
            Values('" + registryID + "','" + Opcode + "','" + departmentCode + "','" + noteID + "','" + series + "','" + reasion.OuterXml.ToString() + "','" + DateTime.Now + "')";

            string strUpdate = @"Update EmrDocument set document.modify
                        ('replace value of (/Emr/EmrNote[@Series=" + series + "]/@NoteStatus)[1] with " + Convert.ToInt32(status) + "')  where RegistryID='" + registryID + "'";

            string[] strList = { strInsert, strUpdate };
            if (Helper.ExecuteSqlByTran(strList) == true)
            {
                return null;
            }
            else
            {
                return "Error";
            }

        }
        catch (Exception ex)
        {
            return ex.Message + "-" + ex.Source;
        }
    }




    #endregion

    #region Configure
    /* ----------------------------------------------------------------------------------
     * Update Configure 
     * 
     * 
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Update Configure", EnableSession = false)]
    public Boolean UpdateConfigure(EmrConstant.ConfigureMode mode, string value)
    {
        switch (mode)
        {
            case EmrConstant.ConfigureMode.AuthenticLevel:
                /* value = "A", "B", "C", or "D", SET AuthenticLevel = value */

                break;
            case EmrConstant.ConfigureMode.AutoArchiveNum:
                /* value = EmrConstant.GenerationArchiveNumMode, SET AutoArchiveNum = value */

                break;
            case EmrConstant.ConfigureMode.MainPasswd:
                /* value = string(6) SET MainPasswd = encript(value) */
                break;

        }
        /* Error occured */
        ErrorLog("UpdateConfigure", EmrConstant.DbTables.Configure,
            EmrConstant.SqlOperations.Update, "");
        return EmrConstant.Return.Failed;
    }
    /* ----------------------------------------------------------------------------------
     * Parameters:
     *      upper   -- upper doctorID
     *      lowers  -- upper's lower doctorIDs
     *      opcode  -- operator code
     * Insert rows into Relationship
     * 
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Insert Relationship", EnableSession = false)]
    public Boolean PutRelationship(string upper, string[] lowers, string opcode)
    {
        foreach (string doctorID in lowers)
        {
            /* INSERT INTO Relationship VALUS(upper, doctorID, opcode, today) */
        }
        return EmrConstant.Return.Successful;
    }
    /* ----------------------------------------------------------------------------------
     * Parameters:
     *      upper   -- upper doctorID
     * [ref]lowers  -- upper's lower doctorIDs
     * Get upper's lower doctorIDs
     * Return count of lowers retrieved
     * Return -1 if error
     * If the count actually retrieved > lowers.length, the rest will be give up.
    ------------------------------------------------------------------------------------ */
    [WebMethod(Description = "Insert Relationship", EnableSession = false)]
    public int GetLowers(string upper, ref string[] lowers)
    {
        int lowerCount = 0;

        /* SELECT Lower INTO lowers FROM Relationship WHERE Upper = upper */
        return lowerCount;
    }
    /* ----------------------------------------------------------------------------------
     * Parameters:
     *      upper   -- upper doctorID
     *      lowers  -- upper's lower doctorIDs
     *      opcode  -- operator code
     * Rebuilt the relationship for the upper and his lowers.
     * 
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Update Relationship", EnableSession = false)]
    public Boolean UpdateRelationship(string upper, string[] lowers, string opcode)
    {
        /* DELETE FROM Relationship WHERE Upper = upper */
        foreach (string doctorID in lowers)
        {
            /* INSERT INTO Relationship VALUS(upper, doctorID, opcode, today) */
        }
        return EmrConstant.Return.Successful;
    }
    /* ----------------------------------------------------------------------------------
     * Check patients
     * if patients.RegistryID is existing in table EmrDocument:
     *      remove this patient from homonymy;
     * else:
     *      if patients.PatientName and Sex are not same from existing(in table Patients):
     *          insert a row into Patients;
     *          insert a row into EmrDocument, where column Document is null
     *              and ArchiveNum maked by method MakeArchiveNum();
     *          remove this patient from homonymy;
     *      else:
     *          replace patients.ArchiveNum with that of homonymy existed(in table Patients)
     * Parameter:
     *      patients -- XmlNode(see patients.xsd),information of a set of patisnts
     * Return:
     *      XmlNode -- information of a set of patisnts that could be same patients
     *                 with someones in table Patients.
    ------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Check Patients", EnableSession = false)]
    public XmlNode CheckPatients(XmlNode patients)
    {
        XmlNode homonymy;

        homonymy = patients;
        return homonymy;
        //throw new System.NotImplementedException();
    }
    #endregion

    #region Templates
    private long GetMaxPk(SqlConnection CS)
    {
        try
        {
            string Query = "SELECT max(pk) FROM NoteTemplate";
            SqlCommand command = new SqlCommand(Query, CS);

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return -1;
            long pk = Convert.ToInt32(reader[0]);
            reader.Close();
            return pk;
        }
        catch (Exception)
        {
            return -1;
        }
    }
    private long GetMaxPkZlg(SqlConnection CS)
    {
        try
        {
            string Query = "SELECT max(pk) FROM TD_NoteTemplate";
            SqlCommand command = new SqlCommand(Query, CS);

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return -1;
            long pk = Convert.ToInt32(reader[0]);
            reader.Close();
            return pk;
        }
        catch (Exception)
        {
            return -1;
        }
    }
    [WebMethod(Description = "NewNoteTemplate", EnableSession = false)]
    public long NewNoteTemplate(string doctorID, string departCode, XmlNode note)
    {
        string Query = "INSERT NoteTemplate (DoctorID ,Note,DepartmentCode) VALUES( '" +
            doctorID + "',cast('" + note.OuterXml + " ' as xml ) ,'" + departCode + " ')";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPk(CS);

            CS.Close();
            return pk;
        }
        catch (Exception e)
        {
            ErrorLog("NewNoteTemplate", "NoteTemplate", SqlOperations.Insert, e.Message);
            return -1;
        }
    }
    [WebMethod(Description = "NewNoteTemplate", EnableSession = false)]
    public long NewNoteTemplateZlg(string doctorID, string departCode, XmlNode note)
    {
        string Query = "INSERT TD_NoteTemplate (DoctorID ,Note,DepartmentCode) VALUES( '" +
            doctorID + "',cast('" + note.OuterXml + " ' as xml ) ,'" + departCode + " ')";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPkZlg(CS);

            CS.Close();
            return pk;
        }
        catch (Exception e)
        {
            ErrorLog("NewNoteTemplate", "NoteTemplate", SqlOperations.Insert, e.Message);
            return -1;
        }
    }
    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public bool UpdateNoteTemplate(long pk, XmlNode note)
    {
        string Query = "UPDATE NoteTemplate SET Note =  cast('" + note.OuterXml +
            " ' as xml )  WHERE pk = '" + pk + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("UpdateNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Update, e.Message);
            return EmrConstant.Return.Failed;
        }


    }
    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public bool UpdateNoteTemplateZlg(long pk, XmlNode note)
    {
        string Query = "UPDATE TD_NoteTemplate SET Note =  cast('" + note.OuterXml +
            " ' as xml )  WHERE pk = '" + pk + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("UpdateNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Update, e.Message);
            return EmrConstant.Return.Failed;
        }


    }
    [WebMethod(Description = "Remove a note template", EnableSession = false)]
    public bool RemoveNoteTemplate(long pk)
    {

        string Query = "delete from NoteTemplate where pk =  '" + pk + "'";
        try
        {

            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("RemoveNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Delete, e.Message);
            return EmrConstant.Return.Failed;

        }

    }
    [WebMethod(Description = "Remove a note template", EnableSession = false)]
    public bool RemoveNoteTemplateZlg(long pk)
    {

        string Query = "delete from TD_NoteTemplate where pk =  '" + pk + "'";
        try
        {

            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("RemoveNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Delete, e.Message);
            return EmrConstant.Return.Failed;

        }

    }
    /*-------------------------------------------------------------------------------------------------
     * Parameters:
     *   long pk -- primary key
     *   ref XmlNode template -- return result
     *     if template == null, template.OuterXml is result
     *     if template != null, template.InnerXml is result
     * ------------------------------------------------------------------------------------------------ */
    [WebMethod(Description = "Get a note template")]
    public string GetNoteTemplate(long pk, ref XmlNode template)
    {
        string Query = "SELECT Note FROM NoteTemplate WHERE pk='" + pk + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return EmrConstant.ErrorMessage.NoFindResult;

            if (template == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                template = doc.Clone();
            }
            else
            {
                template.InnerXml = reader[0].ToString();
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }

    }
    [WebMethod(Description = "Get a note template")]
    public string GetNoteTemplateZlg(long pk, ref XmlNode template)
    {
        string Query = "SELECT Note FROM TD_NoteTemplate WHERE pk='" + pk + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return EmrConstant.ErrorMessage.NoFindResult;

            if (template == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                template = doc.Clone();
            }
            else
            {
                template.InnerXml = reader[0].ToString();
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }

    }

    [WebMethod(Description = "Get department template pks", EnableSession = false)]
    public string GetDepartTemplatePks(ref XmlNode pks, string departCode)
    {
        string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }
    [WebMethod(Description = "Get department template pks", EnableSession = false)]
    public string GetDepartTemplatePksZlg(ref XmlNode pks, string departCode)
    {
        string Query = "SELECT pk from TD_NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }
    [WebMethod(Description = "Get hospital template pks", EnableSession = false)]
    public string GetHospitalTemplatePks(ref XmlNode pks)
    {
        string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '----'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }

    [WebMethod(Description = "Get hospital template pks", EnableSession = false)]
    public string GetHospitalTemplatePksZlg(ref XmlNode pks)
    {
        string Query = "SELECT pk from TD_NoteTemplate WHERE DepartmentCode =  '----'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }

    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public string GetPersonTemplatePks(ref XmlNode pks, string doctorID)
    {
        string Query = "SELECT pk from NoteTemplate WHERE DoctorID =  '" + doctorID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }

    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public string GetPersonTemplatePksZlg(ref XmlNode pks, string doctorID)
    {
        string Query = "SELECT pk from TD_NoteTemplate WHERE DoctorID =  '" + doctorID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }
    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public string GetPersonTemplatePksEx(ref XmlNode pks, string doctorID)
    {
        string Query = "SELECT pk from NoteTemplate WHERE DoctorID =  '" + doctorID + "' and ischecked is not null";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }

    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public string GetPersonTemplatePksExZlg(ref XmlNode pks, string doctorID)
    {
        string Query = "SELECT pk from TD_NoteTemplate WHERE DoctorID =  '" + doctorID + "' and ischecked is not null";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Pks/>");
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            reader.Close();

            if (pks == null)
            {
                pks = doc.Clone();
            }
            else
            {
                pks.InnerXml = doc.DocumentElement.OuterXml;
            }
            return null;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return e.Message + "-" + e.Source;

        }
    }
    //**************************************************************//
    //methord: DepartTemplate
    //Parameter:noteTemplate(XmlNode),departCode(string)
    //pupose:get a xmlnode from table:NoteTemplate which includes the same  DepartmentCode   
    //***************************************************************//
    [WebMethod(Description = "DepartTemplate", EnableSession = false)]
    public bool DepartTemplate(ref XmlNode noteTemplate, string departCode)
    {
        string Query = "select Note ,pk from NoteTemplate where DepartmentCode =  '" + departCode + "'";
        XmlNode node = null;
        if (OperateTemplate(Query, ref node, "DepartmentCode", departCode) == true)
        {
            noteTemplate = node;
            return EmrConstant.Return.Successful;
        }
        else
            return EmrConstant.Return.Failed;
    }

    [WebMethod(Description = "DepartTemplate", EnableSession = false)]
    public bool DepartTemplateZlg(ref XmlNode noteTemplate, string departCode)
    {
        string Query = "select Note ,pk from TD_NoteTemplate where DepartmentCode =  '" + departCode + "'";
        XmlNode node = null;
        if (OperateTemplate(Query, ref node, "DepartmentCode", departCode) == true)
        {
            noteTemplate = node;
            return EmrConstant.Return.Successful;
        }
        else
            return EmrConstant.Return.Failed;
    }
    //**************************************************************//
    //methord: PersonNoteTemplate
    //Parameter:noteTemplate(XmlNode),doctorID(string)
    //pupose:get a xmlnode from table:NoteTemplate which includes the same  DepartmentCode   
    //***************************************************************//
    [WebMethod(Description = "PersonNoteTemplate", EnableSession = false)]
    public bool PersonNoteTemplate(ref XmlNode noteTemplate, string doctorID)
    {
        string Query = "select Note ,pk from NoteTemplate where DoctorID =  '" + doctorID + "'";
        XmlNode node = null;
        if (OperateTemplate(Query, ref node, "DoctorID", doctorID) == true)
        {
            noteTemplate = node;
            return EmrConstant.Return.Successful;
        }
        else
            return EmrConstant.Return.Failed;
    }

    [WebMethod(Description = "PersonNoteTemplate", EnableSession = false)]
    public bool PersonNoteTemplateZlg(ref XmlNode noteTemplate, string doctorID)
    {
        string Query = "select Note ,pk from TD_NoteTemplate where DoctorID =  '" + doctorID + "'";
        XmlNode node = null;
        if (OperateTemplate(Query, ref node, "DoctorID", doctorID) == true)
        {
            noteTemplate = node;
            return EmrConstant.Return.Successful;
        }
        else
            return EmrConstant.Return.Failed;
    }
    [WebMethod(Description = "Dose the template exist? ", EnableSession = false)]
    public bool TemplateExist(string templateName, string departmentCode)
    {
        string Query = "SELECT COUNT(*) FROM NoteTemplate WHERE DepartmentCode = '" +
            departmentCode + "' AND (Note.exist('(/NoteTemplate[@Name=\"" + templateName + "\"])') = 1)";

        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        CS.Open();
        SqlCommand command = new SqlCommand(Query, CS);
        SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            bool ret = false;
            if (Convert.ToInt16(reader[0]) > 0) ret = true;
            reader.Close();
            CS.Close();
            return ret;
        }
        else
        {
            reader.Close();
            CS.Close();
            return false;
        }

    }
    [WebMethod(Description = "Dose the template exist? ", EnableSession = false)]
    public bool TemplateExistZlg(string templateName, string departmentCode)
    {
        string Query = "SELECT COUNT(*) FROM TD_NoteTemplate WHERE DepartmentCode = '" +
            departmentCode + "' AND (Note.exist('(/NoteTemplate[@Name=\"" + templateName + "\"])') = 1)";

        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        CS.Open();
        SqlCommand command = new SqlCommand(Query, CS);
        SqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            bool ret = false;
            if (Convert.ToInt16(reader[0]) > 0) ret = true;
            reader.Close();
            CS.Close();
            return ret;
        }
        else
        {
            reader.Close();
            CS.Close();
            return false;
        }

    }

    public bool OperateTemplate(string query, ref XmlNode TemplateNode, string type, string id)
    {
        XmlDocument xmldoc = null;
        try
        {

            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(query, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            xmldoc = new XmlDocument();
            xmldoc.PreserveWhitespace = true;
            XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmldoc.AppendChild(xmlnode);
            XmlElement xmlelem = xmldoc.CreateElement("", "NoteTemplates", "");
            xmldoc.AppendChild(xmlelem);
            XmlNode root = xmldoc.SelectSingleNode("NoteTemplates");
            string str = "";
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            XmlNode node = null;
            if (reader.Read() == false)
            {
                xmldoc.LoadXml("<NoteTemplates> " + "</NoteTemplates>");
                root = xmldoc.Clone();
                TemplateNode = root;
                return EmrConstant.Return.Successful;
            }
            else
            {
                do
                {
                    doc.LoadXml(reader[0].ToString());
                    XmlElement xn = doc.DocumentElement;
                    xn.SetAttribute("pk", reader[1].ToString());
                    node = doc.Clone();
                    str = node.OuterXml + str;
                } while (reader.Read() == true);

                xmldoc.LoadXml("<NoteTemplates " + type + "=" + "\"" + id + "\"" + ">" + str + "</NoteTemplates>");
                root = xmldoc.Clone();
                TemplateNode = root;
                return EmrConstant.Return.Successful;
            }


        }
        catch (Exception e)
        {
            ErrorLog("DepartTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Select, e.Message);
            TemplateNode = null;
            return EmrConstant.Return.Failed;

        }

    }

    #endregion

    #region Pattern

    /*----------------------------------------------------------------------------------------------
     * parameter:   xmlNode emrPattern
     * if found the xmlnode need, return TRUE, otherwise return FALSE
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Return a xmlnode", EnableSession = false)]
    public string GetEmrPattern(string departmentCode, ref XmlNode emrPattern)
    {
        try
        {
            string SQLSentence = "SELECT CONVERT(xml, Pattern) FROM EmrPattern WHERE DepartmentCode='" +
                departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "";
            if (reader.Read() == true)
            {
                strXml = reader[0].ToString();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strXml);
                emrPattern = doc.Clone();
                reader.Close();
                connection.Close();
                return null;
            }
            else
            {
                reader.Close();
                connection.Close();
                return "没有病历式样定义！";
            }
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    /*----------------------------------------------------------------------------------------------
     * parameter:   xmlNode emrPattern  
     * if update the emrPattern record successfully, return TRUE, otherwise return FALSE
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Add or Replace pattern into table 'EmrPattern'", EnableSession = false)]
    public string AddEmrPattern(string opcode, string departmentCode, XmlNode emrPattern)
    {
        string today = Convert.ToString(SysTime());
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            string SQLSentence = "SELECT Opcode FROM EmrPattern WHERE DepartmentCode='" + departmentCode + "'";
            SqlCommand cmd = new SqlCommand(SQLSentence, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                SQLSentence = "UPDATE EmrPattern SET Pattern = '" + emrPattern.OuterXml + "', " +
                    "Opcode = '" + opcode + "', Opdate='" + today + "' " +
                    "WHERE DepartmentCode='" + departmentCode + "'";
            }
            else
            {
                SQLSentence = "INSERT INTO EmrPattern (Pattern, DepartmentCode, Opcode, Opdate) VALUES('"
                    + emrPattern.OuterXml + "','" + departmentCode + "','" + opcode + "','" + today + "')";
            }
            reader.Close();

            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Add a new EmrNote into patterns in table 'EmrPattern'", EnableSession = false)]
    public string AddEmrNoteIntoPatterns(string opcode, XmlNode emrNoteParent)
    {
        //string today = Convert.ToString(SysTime());
        try
        {
            ArrayList departments = new ArrayList();
            using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
            {
                connection.Open();
                string SQLSentence = "SELECT DepartmentCode FROM EmrPattern";
                SqlCommand cmd = new SqlCommand(SQLSentence, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
            }

            foreach (string department in departments)
            {

                XmlNode pattern = null;
                string msg = GetEmrPattern(department, ref pattern);
                if (msg != null) return msg;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(pattern.OuterXml);
                XmlElement newEmrNote = doc.CreateElement(EmrConstant.ElementNames.EmrNote);

                newEmrNote.InnerXml = emrNoteParent.FirstChild.InnerXml;
                foreach (XmlAttribute att in emrNoteParent.FirstChild.Attributes)
                {
                    newEmrNote.SetAttribute(att.Name, att.Value);
                }
                doc.DocumentElement.AppendChild(newEmrNote);
                msg = AddEmrPattern(opcode, department, doc.DocumentElement);
                if (msg != null) return msg;
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Remove an EmrNote from patterns in table 'EmrPattern'", EnableSession = false)]
    public string RemoveEmrNoteFromPatterns(string opcode, string noteID)
    {
        //string today = Convert.ToString(SysTime());
        try
        {
            #region Get department codes
            ArrayList departments = new ArrayList();
            using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
            {
                connection.Open();
                string SQLSentence = "SELECT DepartmentCode FROM EmrPattern";
                SqlCommand cmd = new SqlCommand(SQLSentence, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
            }
            #endregion
            #region Remove from each department's pattern the emrNote of which NoteID = noteID
            foreach (string department in departments)
            {
                XmlNode pattern = null;
                string msg = GetEmrPattern(department, ref pattern);
                if (msg != null) return msg;
                XmlNodeList emrNotes = pattern.FirstChild.SelectNodes(EmrConstant.ElementNames.EmrNote);
                foreach (XmlNode emrNote in emrNotes)
                {
                    if (emrNote.Attributes[EmrConstant.AttributeNames.NoteID].Value == noteID)
                    {
                        if (emrNote.Attributes[EmrConstant.AttributeNames.Valid] == null)
                        {
                            XmlAttribute valid =
                                emrNote.OwnerDocument.CreateAttribute(EmrConstant.AttributeNames.Valid);
                            emrNote.Attributes.Append(valid);
                        }
                        emrNote.Attributes[EmrConstant.AttributeNames.Valid].Value = EmrConstant.StringGeneral.No;
                        //pattern.FirstChild.RemoveChild(emrNote);
                        msg = AddEmrPattern(opcode, department, pattern);
                        if (msg != null) return msg;
                        break;
                    }
                }
            }
            #endregion
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Replace a EmrNote in patterns with the newEmrNote", EnableSession = false)]
    public string ReplaceEmrNoteInPatterns(string opcode, XmlNode newEmrNoteParent)
    {

        try
        {
            #region Get department codes
            ArrayList departments = new ArrayList();
            using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
            {
                connection.Open();
                string SQLSentence = "SELECT DepartmentCode FROM EmrPattern";
                SqlCommand cmd = new SqlCommand(SQLSentence, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
            }
            #endregion

            foreach (string department in departments)
            {
                string msg = ReplaceEmrNoteInPatternForOneDepartment(opcode, department, newEmrNoteParent);
                if (msg != null) return msg;
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Replace a EmrNote in pattern with the newEmrNote for one department", EnableSession = false)]
    public string ReplaceEmrNoteInPatternForOneDepartment(string opcode, string department, XmlNode newEmrNoteParent)
    {
        XmlNode pattern = null;
        string msg = GetEmrPattern(department, ref pattern);
        if (msg != null)
        {
            msg = GetEmrPattern(EmrConstant.StringGeneral.NullCode, ref pattern);
            if (msg != null) return msg;
        }
        #region Update emr note
        XmlNode newEmrNote = newEmrNoteParent.FirstChild;
        foreach (XmlNode oldEmrNote in pattern.FirstChild.SelectNodes(EmrConstant.ElementNames.EmrNote))
        {
            if (oldEmrNote.Attributes[AttributeNames.NoteID].Value ==
                newEmrNote.Attributes[AttributeNames.NoteID].Value)
            {
                #region Remove from old Emr note the subtitles that not in new emr note
                for (int n = oldEmrNote.ChildNodes.Count - 1; n >= 0; n--)
                {
                    XmlNode removedSubtitle = oldEmrNote.ChildNodes[n];
                    string titleName = removedSubtitle.Attributes[AttributeNames.TitleName].Value;
                    if (!SubtitleNameExists(newEmrNote, titleName)) oldEmrNote.RemoveChild(removedSubtitle);
                }
                #endregion
                #region Replace attributes for the EmrNote
                oldEmrNote.Attributes.RemoveAll();
                XmlElement tmp = (XmlElement)oldEmrNote;
                foreach (XmlAttribute attr in newEmrNote.Attributes)
                {
                    tmp.SetAttribute(attr.Name, attr.Value);
                }
                #endregion
                #region Add or replace subtitles for the EmrNote
                for (int j = 0; j < newEmrNote.ChildNodes.Count; j++)
                {
                    XmlNode newSubtitle = newEmrNote.ChildNodes[j];
                    XmlNode oldSubtitle = GetSubtitleByIndex(oldEmrNote, j);
                    if (newSubtitle.Attributes[EmrConstant.AttributeNames.TitleName].Value
                        == oldSubtitle.Attributes[EmrConstant.AttributeNames.TitleName].Value)
                    {

                        #region Update attributes of the old subtitle
                        /* The new has more attributes than the old. */
                        for (int kk = oldSubtitle.Attributes.Count; kk < newSubtitle.Attributes.Count; kk++)
                        {
                            XmlAttribute att =
                                oldSubtitle.OwnerDocument.CreateAttribute(newSubtitle.Attributes[kk].Name);
                            oldSubtitle.Attributes.Append(att);
                        }
                        /* Replace attribute value. */
                        for (int k = 0; k < newSubtitle.Attributes.Count; k++)
                        {
                            oldSubtitle.Attributes[k].Value = newSubtitle.Attributes[k].Value;
                        }
                        #endregion
                        oldSubtitle.InnerXml = newSubtitle.InnerXml;
                    }
                    else
                    {
                        #region Add new subtitle into old emrNote
                        XmlElement subtitle = oldEmrNote.OwnerDocument.CreateElement(newSubtitle.Name);
                        for (int m = 0; m < newSubtitle.Attributes.Count; m++)
                        {
                            subtitle.SetAttribute(newSubtitle.Attributes[m].Name, newSubtitle.Attributes[m].Value);
                        }
                        subtitle.InnerXml = newSubtitle.InnerXml;
                        /* oldSubtitle.Attributes[EmrConstant.AttributeNames.TitleName].Value is nullcode, 
                         that directs the new will be the last one. see GetSubtitleByIndex(oldEmrNote, j).*/
                        if (oldSubtitle.Attributes[AttributeNames.TitleName].Value == StringGeneral.NullCode)
                            oldEmrNote.AppendChild(subtitle);
                        else
                            oldEmrNote.InsertBefore(subtitle, oldSubtitle);
                        #endregion
                    }
                }
                #endregion

                break;
            }
        }
        #endregion

        return AddEmrPattern(opcode, department, pattern);
    }
    private bool SubtitleNameExists(XmlNode emrNote, string titleName)
    {
        foreach (XmlNode subtitle in emrNote.ChildNodes)
        {
            if (subtitle.Attributes[EmrConstant.AttributeNames.TitleName].Value == titleName) return true;
        }
        return false;
    }
    private XmlNode GetSubtitleByIndex(XmlNode emrNote, int index)
    {
        if (index + 1 > emrNote.ChildNodes.Count)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement subtitle = doc.CreateElement(EmrConstant.ElementNames.SubTitle);
            subtitle.SetAttribute(EmrConstant.AttributeNames.TitleName, EmrConstant.StringGeneral.NullCode);
            return subtitle;
        }
        return emrNote.ChildNodes[index];
    }
    [WebMethod(Description = "Add group element in patterns", EnableSession = false)]
    public string AddEndPrintGroupInPatterns(string opcode, XmlNode group)
    {

        try
        {
            #region Get department codes
            ArrayList departments = new ArrayList();
            using (SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB")))
            {
                connection.Open();
                string SQLSentence = "SELECT DepartmentCode FROM EmrPattern";
                SqlCommand cmd = new SqlCommand(SQLSentence, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(reader[0].ToString());
                }
                reader.Close();
                connection.Close();
            }
            #endregion

            foreach (string department in departments)
            {
                string msg = AddEndPrintGroupInPattern(opcode, department, group);
                if (msg != null) return msg;
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Add group element in pattern for one deparment", EnableSession = false)]
    public string AddEndPrintGroupInPattern(string opcode, string department, XmlNode group)
    {
        XmlNode pattern = null;
        string msg = GetEmrPattern(department, ref pattern);
        if (msg != null) return msg;

        XmlNodeList groups = pattern.FirstChild.SelectNodes(ElementNames.Group);
        if (groups.Count == 0)
        {
            NewGroup(group, pattern);
            return AddEmrPattern(opcode, department, pattern);
        }
        else
        {
            foreach (XmlNode gp in groups)
            {
                if (gp.Attributes[AttributeNames.Code].Value == group.Attributes[AttributeNames.Code].Value)
                {
                    gp.Attributes[AttributeNames.NoteID].Value = group.Attributes[AttributeNames.NoteID].Value;
                    return AddEmrPattern(opcode, department, pattern);
                }
            }
            NewGroup(group, pattern);
            return AddEmrPattern(opcode, department, pattern);
        }
    }
    private void NewGroup(XmlNode group, XmlNode pattern)
    {
        XmlElement gp = pattern.OwnerDocument.CreateElement(ElementNames.Group);
        gp.SetAttribute(AttributeNames.Code, group.Attributes[AttributeNames.Code].Value);
        gp.SetAttribute(AttributeNames.NoteID, group.Attributes[AttributeNames.NoteID].Value);
        pattern.AppendChild(gp);
    }
    #endregion

    #region Blocks

    [WebMethod(Description = "add new Blocks or replace old one", EnableSession = false)]
    public Boolean AddEmrBlocks(string departmentCode, XmlNode emrBlocks)
    {
        try
        {
            string SQLSentence = "SELECT COUNT(*) FROM EmrBlocks WHERE DepartmentCode='" + departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read() == false) return EmrConstant.Return.Failed;

            string SQLSentence1 = "";
            if (Convert.ToInt16(reader[0]) > 0)
                SQLSentence1 = "UPDATE EmrBlocks SET Blocks = cast('" + emrBlocks.OuterXml +
                    "' as xml) WHERE DepartmentCode='" + departmentCode + "'";

            else SQLSentence1 = "INSERT EmrBlocks (DepartmentCode, Blocks) VALUES ('" + departmentCode +
                "', cast('" + emrBlocks.OuterXml + "' as xml))";

            reader.Close();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence1, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception)
        {
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "Add new block ", EnableSession = false)]
    public int AddEmrBlock(string departmentCode, XmlNode emrBlock)
    {
        try
        {

            string SQLSentence = "INSERT EmrBlocks (DepartmentCode, Blocks) VALUES ('" + departmentCode +
                "', cast('" + emrBlock.OuterXml + "' as xml))";
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            SQLSentence = "SELECT MAX(pk) FROM EmrBlocks WHERE DepartmentCode = '" + departmentCode + "'";
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            int pk = 0;
            if (reader.Read()) pk = Convert.ToInt32(reader[0]);
            connection.Close();
            return pk;
        }
        catch (Exception ex)
        {
            ErrorLog("AddEmrBlock", "EmrBlocks", "INSERT", ex.Message);
            return 0;
        }
    }
    [WebMethod(Description = "Update a block ", EnableSession = false)]
    public Boolean UpdateEmrBlock(int pk, XmlNode block)
    {
        string SQLSentence = "UPDATE EmrBlocks SET  Blocks = '"
            + block.OuterXml + "' WHERE pk = '" + pk + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("UpdateEmrBlock", "EmrBlocks", "Update", ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "Delete a block ", EnableSession = false)]
    public Boolean DeleteEmrBlock(int pk)
    {
        string SQLSentence = "DELETE FROM EmrBlocks WHERE pk = '" + pk + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("DeleteEmrBlock", "EmrBlocks", "Delete", ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    /*----------------------------------------------------------------------------------------------
     * parameter:   string departmentCode   
     *              xmlNode emrBlocks
     * if found the xmlnode need, return TRUE, otherwise return FALSE
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Return a xmlnode, note blocks for one deparment.", EnableSession = false)]
    public Boolean GetEmrBlocksForOneDepartment(string departmentCode, ref XmlNode emrBlocks)
    {
        try
        {
            string SQLSentence = "SELECT CONVERT(xml, Blocks) FROM EmrBlocks WHERE DepartmentCode='" + departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<EmrBlocks></EmrBlocks>");

            while (reader.Read())
            {
                XmlElement blockParent = doc.CreateElement(EmrConstant.ElementNames.EmrBlocks);
                blockParent.InnerXml = reader[0].ToString();
                XmlNode block = blockParent.FirstChild.Clone();
                doc.DocumentElement.AppendChild(block);
            }

            doc.DocumentElement.SetAttribute(EmrConstant.AttributeNames.Code, departmentCode);
            emrBlocks = doc.Clone();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("GetEmrBlocks", "Blocks", EmrConstant.SqlOperations.Select, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "Return a pks for emrBlocks", EnableSession = false)]
    public Boolean GetEmrBlockKeys(string departmentCode, ref XmlNode blockKeys)
    {
        try
        {
            string SQLSentence = "SELECT pk FROM EmrBlocks WHERE DepartmentCode='" + departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "<Pks></Pks>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            blockKeys = doc.Clone();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("GetEmrBlocks", "Blocks", EmrConstant.SqlOperations.Select, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "Return a xmlnode for an emrBlock", EnableSession = false)]
    public Boolean GetEmrBlock(int pk, ref XmlNode emrBlock)
    {
        try
        {
            string SQLSentence = "SELECT CONVERT(xml, Blocks) FROM EmrBlocks WHERE pk='" + pk + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "";
            if (reader.Read() == true) strXml = reader[0].ToString();
            else strXml = "<Block></Block>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            emrBlock = doc.Clone();

            //emrBlock.InnerText = strXml;


            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("GetEmrBlocks", "Blocks", EmrConstant.SqlOperations.Select, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "export emrBlocks", EnableSession = false)]
    public string ExportEmrBlocks(ref XmlNode emrBlocks)
    {
        try
        {
            string SQLSentence = "SELECT DISTINCT DepartmentCode FROM EmrBlocks";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            ArrayList department = new ArrayList();
            while (reader.Read())
            {
                department.Add(reader[0].ToString());
            }
            reader.Close();
            connection.Close();
            XmlDocument doc = new XmlDocument();
            XmlElement blocksSet = doc.CreateElement(EmrConstant.ElementNames.BlocksSet);
            foreach (string code in department)
            {
                XmlNode tmp = null;
                GetEmrBlocksForOneDepartment(code, ref tmp);
                XmlElement blocks = doc.CreateElement(EmrConstant.ElementNames.EmrBlocks);
                blocks.InnerXml = tmp.FirstChild.InnerXml;
                blocks.SetAttribute(EmrConstant.AttributeNames.Code, code);
                blocksSet.AppendChild(blocks);
            }


            emrBlocks = blocksSet.Clone();

            return null;
        }
        catch (Exception ex)
        {
            ErrorLog("GetEmrBlocks", "Blocks", EmrConstant.SqlOperations.Select, ex.Message);
            return ex.Message + "--" + ex.Source;
        }
    }
    #endregion

    #region Pictures
    /*----------------------------------------------------------------------------------------------
     * parameter:   picName          string    name of the picture
     *              departmentCode   string     
     *              picture          xmlnode   its attribute is name, its text is bit steam
     * if add the picture successfully, return TRUE, otherwise return FALSE
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Add a new picture", EnableSession = false)]
    public Boolean AddPicture(string picName, string departmentCode, XmlNode picture)
    {
        try
        {
            string SQLSentence = "INSERT PicGallery (PicName, DepartmentCode, Picture) VALUES('" +
                picName + "','" + departmentCode + "',cast('" + picture.OuterXml + "' as xml))";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception)
        {
            return EmrConstant.Return.Failed;
        }
    }
    /*----------------------------------------------------------------------------------------------
     * parameter:   departmentCode(string)
     * Return a xmlnode which include all pictures of the department.
     * Return null if there is no picture for the department.
     * Return null when ERROR.
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Return a xmlnode which include all pictures", EnableSession = false)]
    public XmlNode GetPictures(string departmentCode)
    {
        try
        {
            string SQLSentence = "SELECT CONVERT(xml, Picture) FROM PicGallery WHERE DepartmentCode='" + departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "<PicGallery>";
            if (reader.Read())
            {
                do
                {
                    strXml += reader[0].ToString();
                } while (reader.Read() == true);
            }
            strXml += "</PicGallery>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            XmlNode picGallery = doc.Clone();
            connection.Close();
            return picGallery;
        }
        catch (Exception)
        {
            return null;
        }
    }
    /*----------------------------------------------------------------------------------------------
     * parameter:   picName(string)
     *              departmentCode(string)
     * Delete a picture
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Delete a picture", EnableSession = false)]
    public void DeletePicture(string picName, string departmentCode)
    {
        try
        {
            string SQLSentence =
                "DELETE FROM PicGallery WHERE PicName='" + picName + "' and DepartmentCode='" + departmentCode + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            connection.Close();
        }
        catch (Exception)
        {
        }
    }

    #endregion

    /*----------------------------------------------------------------------------------------------
     *If connect with SQL batabase?
     * return  true:  it connect with the database
     *         false: it doesn't connect with the database
     -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "If connect with SQL database?", EnableSession = false)]
    public Boolean ConnectSql()
    {
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception)
        {
            return EmrConstant.Return.Failed;
        }
    }

    #region Phrases
    /*----------------------------------------------------------------------------------------------
     * parameter:
     *      string doctorID
     *      string departmentCode
     *      xmlNode emrPattern
     * if found the xmlnode need, return TRUE, otherwise return FALSE
     * 2007-08-02
    -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "add new Phrases or replace old one", EnableSession = false)]
    public string AddNotePhrases(string departmentCode, string doctorID, XmlNode emrPhrases)
    {
        try
        {
            string SQLSentence = "SELECT COUNT(*) FROM NotePhrase WHERE DepartmentCode='"
                + departmentCode + "' AND DoctorID='" + doctorID + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read() == false) return EmrConstant.ErrorMessage.NoFindResult;

            string SQLSentence1 = "";
            if (Convert.ToInt16(reader[0]) > 0)
                SQLSentence1 = "UPDATE NotePhrase SET Phrases = '" + emrPhrases.OuterXml +
                    "' WHERE DepartmentCode='" + departmentCode + "' AND DoctorID='" + doctorID + "'";

            else SQLSentence1 = "INSERT INTO NotePhrase (DepartmentCode, DoctorID, Phrases) VALUES ('"
                + departmentCode + "', '" + doctorID + "', '" + emrPhrases.OuterXml + "')";

            reader.Close();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence1, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("AddEmrPhrases", "EmrPhrase", "INSERT or UPDATE", ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }

    [WebMethod(Description = "get a Phrases", EnableSession = false)]
    public string GetNotePhrases(string departmentCode, string doctorID, ref XmlNode emrPhrases)
    {
        try
        {
            string SQLSentence = "SELECT Phrases FROM NotePhrase WHERE DepartmentCode='"
                + departmentCode + "' AND DoctorID='" + doctorID + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read() == false) return EmrConstant.ErrorMessage.NoFindResult;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            emrPhrases = doc.Clone();

            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("GetNotePhrases", "NotePhrase", "SELECT", ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }

    [WebMethod(Description = "Export Phrases", EnableSession = false)]
    public string ExportNotePhrases(ref XmlNode notePhrasesSet)
    {
        try
        {
            string SQLSentence = "SELECT DepartmentCode, Phrases FROM NotePhrase WHERE DepartmentCode<>'####'";
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            XmlElement phrasesSet = doc.CreateElement(EmrConstant.ElementNames.PhraseSet);
            while (reader.Read())
            {
                XmlElement tmp = doc.CreateElement("tmp");
                tmp.InnerXml = reader[1].ToString();
                XmlElement phrase = (XmlElement)tmp.FirstChild;
                phrase.SetAttribute(EmrConstant.AttributeNames.Code, reader[0].ToString());
                phrasesSet.AppendChild(phrase);
            }
            notePhrasesSet = phrasesSet.Clone();
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }
    #endregion

    #region Archive
    /*----------------------------------------------------------------------------------------------
     * Parameter:
     *      string registryID
     * Function:
     *      Set status = Locked
     * if success, return TRUE otherwise return FALSE
     * 2007-08-02
    -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = " ", EnableSession = false)]
    public Boolean Archive(string registryID)
    {
        try
        {
            string SQLSentence = "UPDATE EmrDocument SET status = 1 WHERE RegistryID='"
                + registryID + "'";
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();

            string SQLSentence_his = "UPDATE tdjk SET emrstatus = 1 WHERE zyh='"
                + registryID + "'";
            SqlConnection connection_his = new SqlConnection(ConfigClass.GetConfigString("appSettings", "HisDB"));
            connection_his.Open();

            SqlDataAdapter adapter_his = new SqlDataAdapter(SQLSentence_his, connection_his);
            DataSet dataSet_his = new DataSet();
            adapter_his.Fill(dataSet_his);

            connection_his.Close();

            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("Archive", "EmrDocument", "UPDATE", ex.Source);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public void ArchiveByMe()
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        SqlHelper oHelpers = new SqlHelper("HisDB");
        string strSelect = "select RegistryID from EmrDocument where status = 1";
        DataTable dt = oHelper.GetDataTable(strSelect);
        foreach (DataRow dr in dt.Rows)
        {
            string SQLSentence_his = "UPDATE tdjk SET emrstatus = 1 WHERE zyh='"
                   + dr[0] + "'";
            oHelpers.ExecuteSql(SQLSentence_his);
        }
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public Boolean ArchiveBatch(XmlNode registryIDs)
    {
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();

            foreach (XmlNode registryID in registryIDs)
            {
                string SQLSentence = "UPDATE EmrDocument SET status = 1 WHERE RegistryID='"
                    + registryID.InnerText + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
            }

            connection.Close();

            SqlConnection connection_his = new SqlConnection(ConfigClass.GetConfigString("appSettings", "HisDB"));
            connection_his.Open();

            foreach (XmlNode registryID in registryIDs)
            {
                string SQLSentence_his = "UPDATE tdjk SET emrstatus = 1,emrgdsj='" + DateTime.Now + "' WHERE zyh='"
                    + registryID.InnerText + "'";
                SqlDataAdapter adapter_his = new SqlDataAdapter(SQLSentence_his, connection_his);
                DataSet dataSet_his = new DataSet();
                adapter_his.Fill(dataSet_his);
            }

            connection_his.Close();


            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("ArchiveAll", "EmrDocument", "UPDATE", ex.Source);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = " 按照病案号归档", EnableSession = false)]
    public Boolean Archive_ArchiveNum(string ArchiveNum)
    {
        try
        {

            string SQLSentence = "UPDATE EmrDocument SET status = 1 WHERE ArchiveNum='"
                + ArchiveNum + "'";
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("Archive", "EmrDocument", "UPDATE", ex.Source);
            return EmrConstant.Return.Failed;
        }
    }

    #endregion

    [WebMethod(Description = " ", EnableSession = false)]
    public Boolean GetLastWriteTime(XmlNode inEmrs, ref XmlNode outEmrs)
    {

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();

        XmlDocument doc = new XmlDocument();
        XmlNode emrInDatabase = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
        XmlNode emrsTmp = doc.CreateElement(EmrConstant.ElementNames.Emrs);

        XmlNodeList emrs = inEmrs.SelectNodes(EmrConstant.ElementNames.Emr);
        foreach (XmlNode emr in emrs)
        {
            string registryID = emr.Attributes[EmrConstant.AttributeNames.RegistryID].Value;
            long lastWriteTime = Convert.ToInt64(emr.Attributes[EmrConstant.AttributeNames.LastWriteTime].Value);

            bool ret = GetDocument(registryID, connection, ref emrInDatabase);
            if (ret == EmrConstant.Return.Failed) continue;
            /* Is in local newer than in database? */
            long lastWriteTimeInDatabase =
                Convert.ToInt64(emrInDatabase.Attributes[EmrConstant.AttributeNames.LastWriteTime].Value);
            if (lastWriteTime == lastWriteTimeInDatabase) continue;
            /* Yes */
            XmlElement outEmr = doc.CreateElement(EmrConstant.ElementNames.Emr);
            outEmr.SetAttribute(EmrConstant.AttributeNames.RegistryID, registryID);
            XmlNodeList emrNotes = emrInDatabase.SelectNodes(EmrConstant.ElementNames.EmrNote);
            foreach (XmlNode emrNote in emrNotes)
            {
                XmlElement outEmrNote = doc.CreateElement(EmrConstant.ElementNames.EmrNote);
                outEmrNote.SetAttribute(EmrConstant.AttributeNames.NoteID,
                    emrNote.Attributes[EmrConstant.AttributeNames.NoteID].Value);
                outEmrNote.SetAttribute(EmrConstant.AttributeNames.Series,
                    emrNote.Attributes[EmrConstant.AttributeNames.Series].Value);
                outEmr.AppendChild(outEmrNote);
                outEmrNote.SetAttribute(EmrConstant.AttributeNames.LastWriteTime,
                    emrNote.Attributes[EmrConstant.AttributeNames.LastWriteTime].Value);
            }
            emrsTmp.AppendChild(outEmr);
        }
        outEmrs = emrsTmp.Clone();

        connection.Close();
        return EmrConstant.Return.Successful;
        //}
        //catch (Exception ex)
        //{
        //    ErrorLog("GetLastWriteTime", "EmrDocument", "SELECT", ex.Source);
        //    return EmrConstant.Return.Failed;
        //}
    }

    [WebMethod]  // 自动代值 LiuQi2012-05-02
    public int Savenew(string strRegistryID, string strInSituation, string strSubjective, string strExam, string strDiagnose, string HistoryNow, string HistoryPast, string Test, string Other,
     string Other1, string Other2, string Other3, string Other4, string Other5, string Other6, string Other7, string Other8, string Other9)
    {
        int iResult = -1;
        try
        {
            string strSelect = "SELECT InSituation FROM TD_BasicInfo WHERE RegistryID = '" + strRegistryID + "' ";
            SqlHelper Helper = new SqlHelper("EmrDB");
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                string str = "";
                if (strInSituation != "")
                {
                    str = "InSituation = '" + strInSituation + "'" + ",";

                }
                if (strSubjective != "")
                {
                    str = str + "Subjective = '" + strSubjective + "' " + ",";
                }
                if (strExam != "")
                {
                    str = str + "Exam = '" + strExam + "' " + ",";
                }
                if (strDiagnose != "")
                {
                    str = str + "Diagnose = '" + strDiagnose + "' " + ",";
                }
                if (HistoryNow != "")
                {
                    str = str + "HistoryNow = '" + HistoryNow + "' " + ",";
                }
                if (HistoryPast != "")
                {
                    str = str + "HistoryPast = '" + HistoryPast + "' " + ",";
                }
                if (Test != "")
                {
                    str = str + "Test = '" + Test + "' " + ",";
                }
                if (Other != "")
                {
                    str = str + "Other = '" + Other + "' " + ",";
                }
                if (Other1 != "")
                {
                    str = str + "OtherA = '" + Other1 + "' " + ",";
                }
                if (Other2 != "")
                {
                    str = str + "OtherB = '" + Other2 + "' " + ",";
                }
                if (Other3 != "")
                {
                    str = str + "OtherC = '" + Other3 + "' " + ",";
                }
                if (Other4 != "")
                {
                    str = str + "OtherD = '" + Other4 + "' " + ",";
                }
                if (Other5 != "")
                {
                    str = str + "OtherE = '" + Other5 + "' " + ",";
                }
                if (Other6 != "")
                {
                    str = str + "OtherF = '" + Other6 + "' " + ",";
                }
                if (Other7 != "")
                {
                    str = str + "OtherG = '" + Other7 + "' " + ",";
                }
                if (Other8 != "")
                {
                    str = str + "OtherH = '" + Other8 + "' " + ",";
                }
                if (Other9 != "")
                {
                    str = str + "OtherI = '" + Other9 + "' " + ",";
                }
                if (str == "")
                {
                    return -1;
                }
                else
                {
                    str = str.Trim().Substring(0, str.Trim().Length - 1);
                }
                string strUpdate = "UPDATE TD_BasicInfo SET " + str + " WHERE RegistryID = '" + strRegistryID + "'";
                iResult = Helper.ExecuteNonQuery(strUpdate);
            }
            else
            {
                string strSave = "INSERT INTO TD_BasicInfo  (InSituation,Subjective,Exam,Diagnose,RegistryID,HistoryNow,HistoryPast,Test,Other,OtherA,OtherB,OtherC,OtherD,OtherE,OtherF,OtherG,OtherH,OtherI) VALUES( '" + strInSituation + "', '" + strSubjective + "','" + strExam + "','" + strDiagnose + "','" + strRegistryID + "','" + HistoryNow + "','" + HistoryPast + "','" + Test + "','" + Other + "','" + Other1 + "','" + Other2 + "','" + Other3 + "','" + Other4 + "','" + Other5 + "','" + Other6 + "','" + Other7 + "','" + Other8 + "','" + Other9 + "')";
                iResult = Helper.ExecuteNonQuery(strSave);
            }

        }
        catch (Exception ex)
        {

        }
        return iResult;

    }



    #region Security
    [WebMethod(Description = "Update Securiy", EnableSession = false)]
    public Boolean UpdateSecuriy(XmlNode root)
    {

        string update = "UPDATE Security SET Config = '" + root.OuterXml + "'";
        string insert = "INSERT INTO Security (Config) VALUES( '" + root.OuterXml + "')";
        string select = "SELECT COUNT(*) FROM Security";

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(select, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();
            int count = Convert.ToInt16(reader[0]);
            reader.Close();

            if (count == 0)
            {
                SqlCommand command1 = new SqlCommand(insert, CS);
                command1.CommandType = CommandType.Text;
                SqlDataReader reader1 = command1.ExecuteReader();
                //reader1.Close();
            }
            else
            {
                SqlCommand command1 = new SqlCommand(update, CS);
                command1.CommandType = CommandType.Text;
                SqlDataReader reader1 = command1.ExecuteReader();
                //reader1.Close();
            }
            CS.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("UpdateSecurity", EmrConstant.DbTables.EmrDocument,
                EmrConstant.SqlOperations.Update, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod(Description = "Get Securiy", EnableSession = false)]
    public string GetSecurityConfig(ref XmlNode content)
    {
        string select = "SELECT Config FROM Security";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(select, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                CS.Close();
                return "没有安全配置记录！";
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            content = doc.Clone();

            reader.Close();
            CS.Close();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    private bool GetKeyAndIV(XmlNode content, out byte[] key, out byte[] IV)
    {
        XmlNode keys = content.FirstChild.SelectSingleNode(EmrConstant.ElementNames.Keys);
        byte[] key1 = new byte[keys.ChildNodes.Count];
        for (int n = 0; n < keys.ChildNodes.Count; n++)
        {
            key1[n] = Convert.ToByte(keys.ChildNodes[n].InnerText);
        }
        XmlNode ivs = content.FirstChild.SelectSingleNode("IVs");
        byte[] IV1 = new byte[ivs.ChildNodes.Count];
        for (int n = 0; n < ivs.ChildNodes.Count; n++)
        {
            IV1[n] = Convert.ToByte(ivs.ChildNodes[n].InnerText);
        }
        if (key1.Length > 0 && IV1.Length > 0)
        {
            key = (byte[])key1.Clone();
            IV = (byte[])IV1.Clone();
            return EmrConstant.Return.Successful;
        }
        key = null;
        IV = null;
        return EmrConstant.Return.Failed;
    }
    private ICryptoTransform SaDecoder(XmlNode content)
    {
        byte[] key, IV;
        if (GetKeyAndIV(content, out key, out IV) == EmrConstant.Return.Failed) return null;

        SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
        ICryptoTransform saDecoder = sa.CreateDecryptor(key, IV);
        return saDecoder;
    }
    private ICryptoTransform SaEncoder(XmlNode content)
    {
        byte[] key, IV;
        if (GetKeyAndIV(content, out key, out IV) == EmrConstant.Return.Failed) return null;

        SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
        ICryptoTransform saEncoder = sa.CreateEncryptor(key, IV);
        return saEncoder;
    }
    private byte[] FromString(string text)
    {
        byte[] bytes = new byte[text.Length];
        for (int i = 0; i < text.Length; i++) bytes[i] = (byte)Convert.ToChar(text.Substring(i, 1));
        return bytes;
    }
    private string ToString(byte[] bytes)
    {
        string tmp = null;
        for (int k = 0; k < bytes.Length; k++)
        {
            tmp += Convert.ToChar(bytes[k]);
        }
        return tmp;
    }

    [WebMethod(Description = "Get password in plain text", EnableSession = false)]
    public Boolean GetPassword(ref string passwd)
    {
        /* Get security config data. */
        XmlDocument doc = new XmlDocument();
        XmlNode content = doc.CreateElement(EmrConstant.ElementNames.Content);
        string msg = GetSecurityConfig(ref content);
        if (msg != null) return EmrConstant.Return.Failed;
        /* Base on the config data, create a decoder. */
        ICryptoTransform decoder = SaDecoder(content);
        /* Find password from the config data. */
        XmlNode xmlpwd = content.FirstChild.SelectSingleNode(EmrConstant.ElementNames.Password);
        byte[] pwd = Convert.FromBase64String(xmlpwd.InnerText);
        if (pwd.Length <= 0) return EmrConstant.Return.Failed;

        byte[] rett = decoder.TransformFinalBlock(pwd, 0, pwd.Length);
        if (rett.Length <= 0) return EmrConstant.Return.Failed;
        passwd = ToString(rett);

        return EmrConstant.Return.Successful;
    }

    [WebMethod(Description = "replace password", EnableSession = false)]
    public Boolean ReplacePassword(string passwd)
    {
        /* Get security config data. */
        XmlDocument doc = new XmlDocument();
        XmlNode content = doc.CreateElement(EmrConstant.ElementNames.Content);
        string msg = GetSecurityConfig(ref content);
        if (msg != null) return EmrConstant.Return.Failed;
        /* Base on the config data, create a encoder. */
        ICryptoTransform encoder = SaEncoder(content);
        /* Encoding the passwd. */

        byte[] pwd = FromString(passwd);
        byte[] rett = encoder.TransformFinalBlock(pwd, 0, pwd.Length);
        if (rett.Length <= 0) return EmrConstant.Return.Failed;
        /* Replace passwd node with the new. */
        XmlNode xmlpwd = content.FirstChild.SelectSingleNode(EmrConstant.ElementNames.Password);
        xmlpwd.InnerText = Convert.ToBase64String(rett);

        return UpdateSecuriy(content);
    }

    [WebMethod(Description = "Encoder a data block with the security config data", EnableSession = false)]
    public bool EmrEncoder(byte[] indata, ref byte[] outdata)
    {
        /* Get security config data. */
        XmlDocument doc = new XmlDocument();
        XmlNode content = doc.CreateElement(EmrConstant.ElementNames.Content);
        if (GetSecurityConfig(ref content) != null) return EmrConstant.Return.Failed;

        ICryptoTransform encoder = SaEncoder(content);
        if (encoder == null) return EmrConstant.Return.Failed;
        byte[] ret = encoder.TransformFinalBlock(indata, 0, indata.Length);
        if (ret.Length > 0)
        {
            outdata = (byte[])ret.Clone();
            return EmrConstant.Return.Successful;
        }
        return EmrConstant.Return.Failed;
    }

    [WebMethod(Description = "Decoder a data block with the security config data", EnableSession = false)]
    public bool EmrDecoder(byte[] indata, ref byte[] outdata)
    {
        /* Get security config data. */
        XmlDocument doc = new XmlDocument();
        XmlNode content = doc.CreateElement(EmrConstant.ElementNames.Content);
        if (GetSecurityConfig(ref content) != null) return EmrConstant.Return.Failed;

        ICryptoTransform decoder = SaDecoder(content);
        if (decoder == null) return EmrConstant.Return.Failed;
        byte[] ret = decoder.TransformFinalBlock(indata, 0, indata.Length);
        if (ret.Length > 0)
        {
            outdata = (byte[])ret.Clone();
            return EmrConstant.Return.Successful;
        }
        return EmrConstant.Return.Failed;
    }

    [WebMethod(Description = "Grant a doctor read privilege for an emrDocument", EnableSession = false)]
    public Boolean GrantReaderForRegistry(GrantReader gr)
    {
        string insert = "INSERT INTO GrantReader VALUES( '" + gr.registryID + "','" + gr.doctorID + "','"
            + gr.startDate + "','" + gr.expiration + "')";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(insert, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            CS.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("GrantReaderForRegistry", "GrantReader", "INSERT", ex.Message);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "Do you have read privilege for an emrDocument", EnableSession = false)]
    public Boolean HavePrivilegeForRegistry(string registryID, string doctorID)
    {
        string select = "SELECT StartDate, Expiration FROM GrantReader WHERE RegistryID='" + registryID
            + "' AND DoctorID='" + doctorID + "'";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(select, CS);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                CS.Close();
                return EmrConstant.Return.Failed;
            }
            DateTime startDate = Convert.ToDateTime(reader[0]);
            int expiration = Convert.ToInt32(reader[1]);
            TimeSpan span = DateTime.Now.Subtract(startDate);
            if (span.Days <= expiration)
            {
                reader.Close();
                CS.Close();
                return EmrConstant.Return.Successful;
            }
            else
            {
                reader.Close();
                CS.Close();
                return EmrConstant.Return.Failed;
            }
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            ErrorLog("HavePrivilegeForRegistry", "GrantReader", "SELECT", ex.Message);
            return EmrConstant.Return.Failed;
        }
    }

    [WebMethod(Description = "Add row into Trust")]
    public string AddTrust(string deputed, string deputing, DateTime end, string opcode)
    {
        string endTime = end.ToString();
        string insert = "INSERT INTO Trust VALUES('" + deputed + "','" + deputing + "','" + endTime.ToString() +
            "','" + opcode + "','" + SysTime().ToString() + "')";

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = new SqlCommand(insert, connection);
        //command.CommandType = CommandType.Text;
        try
        {
            command.ExecuteNonQuery();
            return null;
        }
        catch (SqlException ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }

    [WebMethod(Description = "Is the opcode the deputing of the doctor")]
    public string IsTruster(string opcode, string doctor)
    {
        string select = "SELECT Deputed, EndTime FROM Trust WHERE EndTime=(SELECT MAX(EndTime) AS maxTime " +
            "FROM Trust AS Trust1 WHERE deputing='" + opcode + "' )";

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = new SqlCommand(select, connection);
        try
        {
            SqlDataReader reader = command.ExecuteReader();
            string msg = "不是有效代理医师"; ;
            if (reader.Read())
            {
                string deputed = reader[0].ToString();
                DateTime end = (DateTime)reader[1];
                if (end > SysTime() && deputed == doctor) msg = null;
            }
            return msg;
        }
        catch (SqlException ex)
        {
            return ex.Message + "--" + ex.Source;
        }
    }

    #endregion

    #region Quality Control
    //[WebMethod]
    //public Boolean AddQualityInfo(EmrConstant.QualityInfo qualityInfo)
    //{
    //    try
    //    {
    //        string Query = "INSERT Quality VALUES( '"
    //            + qualityInfo.registryID + " ','" + qualityInfo.noteID + " ','" + qualityInfo.noteName + " ','"
    //            + qualityInfo.startTime + " ','" + qualityInfo.writenTime + " ','" + qualityInfo.limit + " ','" 
    //            + qualityInfo.score + " ')";
    //        SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
    //        CS.Open();
    //        SqlCommand myCommand = new SqlCommand(Query, CS);
    //        SqlDataReader reader = myCommand.ExecuteReader();
    //        return EmrConstant.Return.Successful;
    //    }
    //    catch (Exception ex)
    //    {
    //        ErrorLog("AddQuality", "Quality", EmrConstant.SqlOperations.Insert, ex.Message);
    //        return EmrConstant.Return.Failed;
    //    }
    //}
    #endregion

    #region Update version
    [WebMethod(Description = "Add new version emrw componet.", EnableSession = false)]
    public string AddComponet(string name, XmlNode component)
    {
        try
        {
            string SQLSentence = "SELECT COUNT(*) FROM Version WHERE Name = '" + name + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read() == false) return EmrConstant.ErrorMessage.WebServiceError;

            string SQLSentence1 = "";
            if (Convert.ToInt16(reader[0]) > 0)
                SQLSentence1 = "UPDATE Version SET Component = '" + component.OuterXml +
                    "' WHERE Name = '" + name + "'";

            else SQLSentence1 = "INSERT Version VALUES ('" + name + "','" + component.OuterXml + "')";

            reader.Close();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence1, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + " - " + ex.Source;
        }
    }

    [WebMethod(Description = "Get emrw componet.", EnableSession = false)]
    public string GetComponet(string name, ref XmlNode component)
    {
        string SQLSentence = "SELECT component FROM Version WHERE Name = '" + name + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read() == false)
            {
                reader.Close();
                return EmrConstant.ErrorMessage.NoNewVersion;
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader[0].ToString());
            component = doc.DocumentElement.Clone();
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message + " - " + ex.Source;
        }
    }
    #endregion

    [WebMethod(Description = "Set traffic light to red in order to avoid from other peaple to open the same emr document.")]
    public string SetTrafficLightToRed(string registryID, ref string clientIP, ref string clientMachine)
    {
        string select = "SELECT ClientIP, ClientMachine, Light FROM TrafficLight WHERE registryID='" + registryID + "'";
        string insert = "INSERT INTO TrafficLight VALUES ('" + registryID + "','" + clientIP + "','" + clientMachine + "','R')";
        //string update = "UPDATE TrafficLight SET ClientIP='" + clientIP + "',ClientMachine='" + clientMachine +
        //    "',Light='R' WHERE RegistryID='" + registryID + "'";
        string clientIpOld = clientIP;
        string clientMachineOld = clientMachine;
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(select, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                clientIP = reader[0].ToString();
                clientMachine = reader[1].ToString();
                reader.Close();
                connection.Close();
                return EmrConstant.ErrorMessage.HasOpeningEmr;
            }
            else
            {
                reader.Close();
                command = new SqlCommand(insert, connection);
                command.ExecuteNonQuery();
                connection.Close();
                return null;
            }
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }

    [WebMethod(Description = "Set traffic light to green in order to allow other peaple to open the same emr document.")]
    public string SetTrafficLightToGreen(string registryID)
    {
        //string update = "UPDATE TrafficLight SET Light='G' WHERE RegistryID='" + registryID + "'";
        string delete = "DELETE FROM TrafficLight";
        if (registryID != EmrConstant.StringGeneral.NullCode) delete += " WHERE RegistryID='" + registryID + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(delete, connection);
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }

    [WebMethod(Description = "Returns registryIDs with red light ", EnableSession = false)]
    public XmlNode GetRegistryIDsWithRedLight()
    {
        string SQLSentence = "SELECT RegistryID, ClientMachine, ClientIP FROM TrafficLight ORDER BY RegistryID";
        XmlDocument doc = new XmlDocument();
        XmlElement IDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                XmlElement registryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                registryID.SetAttribute(EmrConstant.AttributeNames.Code, reader[0].ToString());
                registryID.SetAttribute(EmrConstant.AttributeNames.HostName, reader[1].ToString());
                registryID.SetAttribute(EmrConstant.AttributeNames.HostIP, reader[2].ToString());
                IDs.AppendChild(registryID);
            }
            reader.Close();
            connection.Close();
            return IDs;
        } //try end
        catch (Exception)
        {
            //IDs.InnerText = ex.Message;
            return null;
        }
    }

    #region Requisition print
    [WebMethod(Description = "New a requisition", EnableSession = false)]
    public string NewPrintRequisition(string registryID, string noteIDSeries, string department, string opcode,
        string noteName, string patientName, string archiveNum)
    {

        string SQLSentence =
            "INSERT INTO PrintRequisition VALUES(@registryID, @noteIDSeries, @department, @opcode, @today," +
            "@printed, @noteName, @pname, @archiveNum)";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteIDSeries", SqlDbType.VarChar, 8);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@today", SqlDbType.DateTime);
            command.Parameters.Add("@printed", SqlDbType.VarChar, 1);
            command.Parameters.Add("@noteName", SqlDbType.VarChar, 50);
            command.Parameters.Add("@pname", SqlDbType.VarChar, 20);
            command.Parameters.Add("@archiveNum", SqlDbType.VarChar, 10);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteIDSeries;
            command.Parameters[2].Value = department;
            command.Parameters[3].Value = opcode;
            command.Parameters[4].Value = SysTime();
            command.Parameters[5].Value = "0";
            command.Parameters[6].Value = noteName;
            command.Parameters[7].Value = patientName;
            command.Parameters[8].Value = archiveNum;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Retrun print requisitions as xml", EnableSession = false)]
    public string GetPrintRequisitions(DateTime from, DateTime to, bool hasPrinted, ref XmlNode requisitios)
    {
        if (requisitios == null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Requisitions />");
            requisitios = doc.DocumentElement.Clone();
        }
        string SQLSentence =
            "SELECT RegistryID, NoteIDSeries, Department, WantDate, Printed, NoteName, PatientName, ArchiveNum " +
            "FROM PrintRequisition WHERE (WantDate BETWEEN @from AND @to)";
        if (!hasPrinted) SQLSentence += " AND (Printed = @printed)";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@from", SqlDbType.DateTime);
            command.Parameters.Add("@to", SqlDbType.DateTime);
            command.Parameters.Add("@printed", SqlDbType.VarChar, 1);
            command.Parameters[0].Value = from;
            command.Parameters[1].Value = to;
            command.Parameters[2].Value = StringGeneral.Zero;

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                OneItem(requisitios, reader);
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    private void OneItem(XmlNode root, SqlDataReader reader)
    {
        XmlNode department = FindDepartment(root, reader[2].ToString());
        XmlNode patient = FindPatient(department, reader[0].ToString(), reader[6].ToString(), reader[7].ToString());
        XmlElement note = root.OwnerDocument.CreateElement(ElementNames.Note);
        note.SetAttribute(AttributeNames.NoteID, reader[1].ToString());
        note.SetAttribute(AttributeNames.Printed, reader[4].ToString());
        note.SetAttribute(AttributeNames.NoteName, reader[5].ToString());
        patient.AppendChild(note);
    }
    private XmlNode FindDepartment(XmlNode root, string code)
    {
        foreach (XmlNode department in root.ChildNodes)
        {
            if (department.Attributes[AttributeNames.Code].Value == code) return department;
        }
        XmlElement newDepartment = root.OwnerDocument.CreateElement(ElementNames.Department);
        newDepartment.SetAttribute(AttributeNames.Code, code);
        root.AppendChild(newDepartment);
        return newDepartment;
    }
    private XmlNode FindPatient(XmlNode department, string registryID, string pname, string archiveNum)
    {
        foreach (XmlNode patient in department.ChildNodes)
        {
            if (patient.Attributes[AttributeNames.RegistryID].Value == registryID) return patient;
        }
        XmlElement newPatient = department.OwnerDocument.CreateElement(ElementNames.Patient);
        newPatient.SetAttribute(AttributeNames.RegistryID, registryID);
        newPatient.SetAttribute(AttributeNames.PatientName, pname);
        newPatient.SetAttribute(AttributeNames.ArchiveNum, archiveNum);
        department.AppendChild(newPatient);
        return newPatient;
    }

    [WebMethod(Description = "Set to '1' for printed", EnableSession = false)]
    public string SetPrinted(string registryID, string noteIDSeries)
    {
        string SQLSentence =
            "UPDATE PrintRequisition SET Printed = '1' WHERE RegistryID = @regID AND noteIDSeries = @noteIDS";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteIDS", SqlDbType.VarChar, 8);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteIDSeries;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Is printed?", EnableSession = false)]
    public string GetPrintedState(string registryID, string noteIDSeries, ref string printed)
    {
        string SQLSentence =
            "SELECT Printed FROM PrintRequisition WHERE RegistryID = @regID AND noteIDSeries = @noteIDS";
        printed = StringGeneral.Zero;
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteIDS", SqlDbType.VarChar, 8);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteIDSeries;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                printed = reader[0].ToString();
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Count note printing", EnableSession = false)]
    public string CountNotePrinting(XmlNode printInfo)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Parameters.Add("@departmentCode", SqlDbType.VarChar, 4);
        command.Parameters.Add("@departmentName", SqlDbType.VarChar, 40);
        command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
        command.Parameters.Add("@opname", SqlDbType.VarChar, 20);
        command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
        command.Parameters.Add("@pname", SqlDbType.VarChar, 20);
        command.Parameters.Add("@noteID", SqlDbType.VarChar, 4);
        command.Parameters.Add("@noteName", SqlDbType.VarChar, 50);
        command.Parameters.Add("@series", SqlDbType.Int);
        command.Parameters.Add("@printCount", SqlDbType.Int);
        command.Parameters.Add("@today", SqlDbType.DateTime);

        command.Parameters[0].Value = printInfo.Attributes[AttributeNames.DepartmentCode].Value;
        command.Parameters[1].Value = printInfo.Attributes[AttributeNames.DepartmentName].Value;
        command.Parameters[2].Value = printInfo.Attributes[AttributeNames.DoctorID].Value;
        command.Parameters[3].Value = printInfo.Attributes[AttributeNames.Doctor].Value;
        command.Parameters[4].Value = printInfo.Attributes[AttributeNames.RegistryID].Value;
        command.Parameters[5].Value = printInfo.Attributes[AttributeNames.PatientName].Value;
        command.Parameters[6].Value = printInfo.Attributes[AttributeNames.NoteID].Value;
        command.Parameters[7].Value = printInfo.Attributes[AttributeNames.NoteName].Value;
        command.Parameters[8].Value = Convert.ToInt16(printInfo.Attributes[AttributeNames.Series].Value);
        command.Parameters[9].Value = 1;
        command.Parameters[10].Value = SysTime();

        command.CommandText = "SELECT PrintCount FROM PrintLog WHERE RegistryID=@registryID AND Series=@series";

        int count = 0;
        try
        {
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) count = Convert.ToInt16(reader[0]);
            reader.Close();
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }

        if (count == 0)
        {
            command.CommandText = "INSERT INTO PrintLog VALUES(@departmentCode, @departmentName, @opcode, " +
                "@opname, @registryID, @pname, @noteID, @noteName, @series, @printCount, @today)";
            try { command.ExecuteNonQuery(); connection.Close(); return null; }
            catch (Exception ex) { connection.Close(); return ex.Message; }
        }
        else
        {
            command.Parameters[9].Value = count + 1;
            command.CommandText = "UPDATE PrintLog SET PrintCount = @printCount " +
                "WHERE RegistryID = @registryID AND Series = @series";
            try { command.ExecuteNonQuery(); connection.Close(); return null; }
            catch (Exception ex) { connection.Close(); return ex.Message; }
        }
    }

    [WebMethod(Description = "Get print info", EnableSession = false)]
    public string GetNotePrinting(int printCount, ref XmlNode printInfo)
    {
        if (printInfo == null) return null;
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Parameters.Add("@printCount", SqlDbType.Int);
        command.Parameters.Add("@dayFrom", SqlDbType.DateTime);
        command.Parameters.Add("@dayTo", SqlDbType.DateTime);
        command.Parameters.Add("@departCode", SqlDbType.VarChar, 4);

        command.Parameters[0].Value = printCount;
        command.Parameters[1].Value = Convert.ToDateTime(printInfo.Attributes[AttributeNames.DayFrom].Value);
        command.Parameters[2].Value = Convert.ToDateTime(printInfo.Attributes[AttributeNames.DayTo].Value);
        command.Parameters[3].Value = printInfo.Attributes[AttributeNames.DepartmentCode].Value;

        command.CommandText = "SELECT * FROM PrintLog WHERE PrintCount > @printCount AND " +
            "DepartmentCode = @departCode AND Opdate BETWEEN @dayFrom AND @dayTo ORDER BY Opcode";
        try
        {
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string doctorID = reader[3].ToString().Trim();
                string doctorName = reader[4].ToString().Trim();
                string registryID = reader[5].ToString().Trim();
                string patientName = reader[6].ToString().Trim();
                XmlNode patientNode = FindPatientNode(registryID, patientName, doctorID, doctorName, printInfo);
                XmlElement note = printInfo.OwnerDocument.CreateElement(ElementNames.Note);
                note.SetAttribute(AttributeNames.NoteID, reader[7].ToString().Trim());
                note.SetAttribute(AttributeNames.NoteName, reader[8].ToString().Trim());
                note.SetAttribute(AttributeNames.Series, reader[9].ToString());
                note.SetAttribute(AttributeNames.PrintCount, reader[10].ToString());
                patientNode.AppendChild(note);
            }
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    private XmlNode FindDoctorNode(string doctorID, string doctorName, XmlNode printInfo)
    {
        foreach (XmlNode doctor in printInfo.ChildNodes)
        {
            if (doctor.Attributes[AttributeNames.DoctorID].Value == doctorID) return doctor;
        }
        XmlElement newDoctorNode = printInfo.OwnerDocument.CreateElement(ElementNames.Doctor);
        newDoctorNode.SetAttribute(AttributeNames.DoctorID, doctorID);
        newDoctorNode.SetAttribute(AttributeNames.Doctor, doctorName);
        printInfo.AppendChild(newDoctorNode);
        return newDoctorNode;
    }
    private XmlNode FindPatientNode(string registryID, string pname, string doctorID, string doctorName, XmlNode printInfo)
    {
        XmlNode doctor = FindDoctorNode(doctorID, doctorName, printInfo);
        foreach (XmlNode patient in doctor.ChildNodes)
        {
            if (patient.Attributes[AttributeNames.RegistryID].Value == registryID) return patient;
        }
        XmlElement newPatientNode = printInfo.OwnerDocument.CreateElement(ElementNames.Patient);
        newPatientNode.SetAttribute(AttributeNames.RegistryID, registryID);
        newPatientNode.SetAttribute(AttributeNames.PatientName, pname);
        doctor.AppendChild(newPatientNode);
        return newPatientNode;
    }
    #endregion

    #region Unlock emr document
    [WebMethod(Description = "Unlock a emr document", EnableSession = false)]
    public string UnlockEmrdocument(string registryID, string pname, string opcode,
        string opname, int expire, bool forPublic, XmlNode reasion)
    {
        string SQLSentence =
            "INSERT INTO UnlockEmr VALUES(@registryID,@pname,@opcode,@opname,@expire,@public,@reasion,@today)";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@pname", SqlDbType.VarChar, 20);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opname", SqlDbType.VarChar, 20);
            command.Parameters.Add("@expire", SqlDbType.Int);
            command.Parameters.Add("@public", SqlDbType.VarChar, 1);
            command.Parameters.Add("@reasion", SqlDbType.Xml);
            command.Parameters.Add("@today", SqlDbType.DateTime);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = pname;
            command.Parameters[2].Value = opcode;
            command.Parameters[3].Value = opname;
            command.Parameters[4].Value = expire;
            command.Parameters[5].Value = StringGeneral.Zero;
            if (forPublic) command.Parameters[5].Value = StringGeneral.One;
            command.Parameters[6].Value = reasion.OuterXml;
            command.Parameters[7].Value = SysTime();
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [WebMethod(Description = "Has privilege to edit a loacked emr document", EnableSession = false)]
    public string CanEditLockedEmrdocument(string registryID, string opcode, ref string yesno)
    {
        DateTime today = SysTime();


        string SQLSentence =
            "SELECT ForPublic, Expire, Opcode, Opdate FROM UnlockEmr WHERE RegistryID=@registryID";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //if (reader[0].ToString() == StringGeneral.Zero && reader[2].ToString() != opcode) continue;
                if (reader[0].ToString() == StringGeneral.Zero) continue;             
                TimeSpan days = TimeSpan.FromDays(Convert.ToDouble(reader[1]));
                DateTime opdate = (DateTime)reader[3];
                if (today > opdate.Add(days)) continue;
                yesno = StringGeneral.One;
                reader.Close();
                connection.Close();
                return null;
            }
           
            yesno = StringGeneral.Zero;
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    //新添加方法2013-11-1 病历封存
    [WebMethod(Description = "Has privilege to edit a loacked emr document", EnableSession = false)]
    public string LockedEmrdocument(string registryID)
    {
        DateTime today = SysTime();
        string msg="";
        int count = 0;
        string SQLSentence =
            "Update UnlockEmr set ForPublic=0 WHERE RegistryID=@registryID";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            count = command.ExecuteNonQuery();
            //SqlDataReader reader = command.ExecuteReader();
            //while (reader.Read())
            //{
            //    if (reader[0].ToString() == StringGeneral.Zero && reader[2].ToString() != opcode) continue;
            //    TimeSpan days = TimeSpan.FromDays(Convert.ToDouble(reader[1]));
            //    DateTime opdate = (DateTime)reader[3];
            //    if (today > opdate.Add(days)) continue;
            //    yesno = StringGeneral.One;
            //    reader.Close();
            //    connection.Close();
            //    return null;
            //}
            //yesno = StringGeneral.Zero;
            //reader.Close();
            connection.Close();
            if (count == 0)
            {
                msg = "更新失败！";
            }
            return msg;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    [WebMethod(Description = "Has privilege to edit a loacked emr document", EnableSession = false)]
    public string GetEmrLockInfo(string registryID, ref DataSet result)
    {
        string SQLSentence =
            "SELECT Pname, Opcode, Opname, ForPublic, Expire, Reasion, Opdate FROM UnlockEmr " +
            "WHERE RegistryID='" + registryID + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter scommand = new SqlDataAdapter(SQLSentence, connection);
            result = new DataSet();
            scommand.Fill(result, "Results");
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    #region Uncommit emr note
    [WebMethod(Description = "Uncommit a emr note", EnableSession = false)]
    public string UncommitEmrNote(string registryID, string opcode, string departmentCode,
        string noteID, int series, XmlNode reasion, XmlNode emrDoc)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        SqlCommand command = connection.CreateCommand();
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();

        /* Must assign both transaction object and connection to Command object
         * for a pending local transaction */
        command.Connection = connection;
        command.Transaction = transaction;

        #region Add new row into UncommitEmrNote
        command.CommandText = "INSERT INTO UncommitEmrNote VALUES" +
            "(@registryID,@opcode,@department,@noteID,@series,@reasion,@today)";
        try
        {
            command.Parameters.Add("@registryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 3);
            command.Parameters.Add("@series", SqlDbType.Int);
            command.Parameters.Add("@reasion", SqlDbType.Xml);
            command.Parameters.Add("@today", SqlDbType.DateTime);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = opcode;
            command.Parameters[2].Value = departmentCode;
            command.Parameters[3].Value = noteID;
            command.Parameters[4].Value = series;
            command.Parameters[5].Value = reasion.OuterXml;
            command.Parameters[6].Value = SysTime();
            command.ExecuteNonQuery();
        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
        #endregion

        try
        {
            command.CommandText = "UPDATE EmrDocument SET Document = @doc WHERE RegistryID = @regID";
            command.Parameters.Clear();
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@doc", SqlDbType.Xml);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = emrDoc.OuterXml;
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get uncommit info", EnableSession = false)]
    public string GetUncommitInfo(DateTime from, DateTime to, ref XmlNode info)
    {
        from = from.Subtract(from.TimeOfDay);
        to = to.Subtract(to.TimeOfDay);
        to = to.Add(TimeSpan.FromDays(1));
        string select = "SELECT * " +
            "FROM UncommitEmrNote WHERE Opdate BETWEEN @from AND @to " +
            "ORDER BY DeparmentCode,Opcode,RegistryID";
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        SqlCommand command = connection.CreateCommand();
        connection.Open();

        command.CommandText = select;
        command.Parameters.Add("@from", SqlDbType.DateTime);
        command.Parameters.Add("@to", SqlDbType.DateTime);
        command.Parameters[0].Value = from;
        command.Parameters[1].Value = to;
        try
        {
            SqlDataReader reader = command.ExecuteReader();

            XmlDocument doc = new XmlDocument();
            XmlElement departments = doc.CreateElement(ElementNames.Departments);

            while (reader.Read())
            {
                XmlNode department = GetDepartment(reader[2].ToString(), departments);
                XmlNode doctor = GetDoctor(reader[1].ToString(), department);
                XmlNode patient = GetPatient(reader[0].ToString(), doctor);
                XmlNode reason = doc.CreateElement(ElementNames.Reason);
                reason.InnerXml = reader[5].ToString();
                if (patient.Attributes[AttributeNames.PatientName] == null)
                {
                    XmlAttribute pname = doc.CreateAttribute(AttributeNames.PatientName);
                    pname.Value = reason.FirstChild.Attributes[AttributeNames.PatientName].Value;
                    patient.Attributes.Append(pname);
                }
                XmlAttribute opdate = doc.CreateAttribute(AttributeNames.DateTime);
                opdate.Value = reader[6].ToString();
                reason.FirstChild.Attributes.Append(opdate);

                patient.AppendChild(reason.FirstChild);
            }
            info = departments.Clone();
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    private XmlNode GetPatient(string registryID, XmlNode doctor)
    {
        foreach (XmlNode patient in doctor.ChildNodes)
        {
            if (registryID == patient.Attributes[AttributeNames.RegistryID].Value) return patient;
        }
        XmlElement newPatient = doctor.OwnerDocument.CreateElement(ElementNames.Patient);
        newPatient.SetAttribute(AttributeNames.RegistryID, registryID);
        doctor.AppendChild(newPatient);
        return newPatient;
    }
    private XmlNode GetDoctor(string opcode, XmlNode department)
    {
        foreach (XmlNode doctor in department.ChildNodes)
        {
            if (opcode == doctor.Attributes[AttributeNames.DoctorID].Value) return doctor;
        }
        XmlElement newDoctor = department.OwnerDocument.CreateElement(ElementNames.Doctor);
        newDoctor.SetAttribute(AttributeNames.DoctorID, opcode);
        department.AppendChild(newDoctor);
        return newDoctor;
    }
    private XmlNode GetDepartment(string code, XmlNode departments)
    {
        foreach (XmlNode department in departments.ChildNodes)
        {
            if (code == department.Attributes[AttributeNames.DepartmentCode].Value) return department;
        }
        XmlElement newDepartment = departments.OwnerDocument.CreateElement(ElementNames.Department);
        newDepartment.SetAttribute(AttributeNames.DepartmentCode, code);
        departments.AppendChild(newDepartment);
        return newDepartment;
    }
    #endregion

    #region Valuate emr document
    [WebMethod(Description = "New valuate rules for a note", EnableSession = false)]
    public string NewValuateRules(string noteID, XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.CommandText = "DELETE FROM ValuateRules WHERE noteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 4);
            command.Parameters.Add("@rules", SqlDbType.Xml);
            command.Parameters[0].Value = noteID;
            command.Parameters[1].Value = rules.OuterXml;
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO ValuateRules VALUES(@noteID, @rules,null)";
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "New valuate rules for a note", EnableSession = false)]
    public string NewValuateRulesEnd(string noteID, XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.CommandText = "DELETE FROM ValuateRulesEnd WHERE noteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@rules", SqlDbType.Xml);
            command.Parameters[0].Value = noteID;
            command.Parameters[1].Value = rules.OuterXml;
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO ValuateRulesEnd VALUES(@noteID, @rules,null)";
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Remove a note with valuate rules", EnableSession = false)]
    public string RemoveNoteWithValuateRules(string noteID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "DELETE FROM ValuateRules WHERE NoteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "Remove a note with valuate rules", EnableSession = false)]
    public string RemoveNoteWithValuateRulesEnd(string noteID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "DELETE FROM ValuateRulesEnd WHERE NoteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get valuate rules for a note", EnableSession = false)]
    public string GetValuateRules(string noteID, ref XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT rules FROM ValuateRules WHERE noteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                rules = doc.DocumentElement.Clone();
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get valuate rules for a note", EnableSession = false)]
    public string GetValuateRulesEnd(string noteID, ref XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT rules FROM ValuateRulesEnd WHERE noteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                rules = doc.DocumentElement.Clone();
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get notes with valuate rules", EnableSession = false)]
    public string GetNotesWithValuateRules(ref DataSet rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT NoteID FROM ValuateRules";
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet rules1 = new DataSet();
            adapter.Fill(rules1);
            rules = rules1.Copy();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod]
    public string GetNotesWithValuateRulesEnd(ref DataSet rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT NoteID FROM ValuateRulesEnd";
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet rules1 = new DataSet();
            adapter.Fill(rules1);
            rules = rules1.Copy();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "Get notes with valuate rules", EnableSession = false)]
    public string GetNoteIDsWithValuateRules(ref XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        XmlDocument doc = new XmlDocument();
        XmlNode root = doc.CreateElement(ElementNames.Rules);
        XmlNode tmp = doc.CreateElement("tmp");
        try
        {
            command.CommandText = "SELECT NoteID, Rules FROM ValuateRules";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                XmlElement rule = doc.CreateElement(ElementNames.Rule);
                rule.SetAttribute(AttributeNames.NoteID, reader[0].ToString());
                tmp.InnerXml = reader[1].ToString();
                foreach (XmlAttribute attr in tmp.FirstChild.Attributes)
                {
                    rule.SetAttribute(attr.Name, attr.Value);
                }
                root.AppendChild(rule);

            }
            rules = root.Clone();
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "New valuate detail", EnableSession = false)]
    public string NewValuateDetail(bool self, string registryID, string noteID, decimal score, string opcode, XmlNode flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@score", SqlDbType.Decimal);
            command.Parameters.Add("@flaws", SqlDbType.Xml);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opdate", SqlDbType.DateTime);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteID;
            command.Parameters[2].Value = score;
            command.Parameters[3].Value = flaws.OuterXml;
            command.Parameters[4].Value = opcode;
            command.Parameters[5].Value = SysTime();

            if (self)
            {
                command.CommandText = "DELETE FROM SelfValuateDetail WHERE RegistryID = @regID AND NoteID=@noteID";
            }
            else
            {
                command.CommandText = "DELETE FROM ValuateDetail WHERE RegistryID = @regID AND NoteID=@noteID";
            }
            command.ExecuteNonQuery();

            if (self)
            {
                command.CommandText =
                    "INSERT INTO SelfValuateDetail VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ValuateDetail VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
            }
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "New valuate detail", EnableSession = false)]
    public string NewValuateDetailEnd(bool self, string registryID, string noteID, decimal score, string opcode, XmlNode flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@score", SqlDbType.Decimal);
            command.Parameters.Add("@flaws", SqlDbType.Xml);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opdate", SqlDbType.DateTime);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteID;
            command.Parameters[2].Value = score;
            command.Parameters[3].Value = flaws.OuterXml;
            command.Parameters[4].Value = opcode;
            command.Parameters[5].Value = SysTime();

            if (self)
            {
                command.CommandText = "DELETE FROM SelfValuateDetailEnd WHERE RegistryID = @regID AND NoteID=@noteID";
            }
            else
            {
                command.CommandText = "DELETE FROM ValuateDetailEnd WHERE RegistryID = @regID AND NoteID=@noteID";
            }
            command.ExecuteNonQuery();

            if (self)
            {
                command.CommandText =
                    "INSERT INTO SelfValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
            }
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod]
    public XmlDataDocument GetValuateDetailDT()
    {
        bool self = true;
        string registryID = "00035287";
        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelfSelect = "SELECT Score, Flaws, NoteID FROM SelfValuateDetail WHERE RegistryID = '" + registryID + "'";
        string strSelect = "SELECT Score, Flaws, NoteID FROM ValuateDetail WHERE RegistryID = '" + registryID + "'";
        DataSet dsResult = new DataSet();
        try
        {
            if (self)
            {
                dsResult = Helper.GetDataSet(strSelfSelect);
            }
            else
            {
                dsResult = Helper.GetDataSet(strSelect);
            }
            XmlDataDocument XMLDoc = new XmlDataDocument(dsResult);

            return XMLDoc;

        }
        catch (Exception ex)
        {
            return null;
        }


    }

    [WebMethod]
    public DataSet GetValuateDetailDTEx(bool self, string registryID)
    {
        SqlHelper HelperHis = new SqlHelper("HisDB");

        string strselectZ = "select zyh from tdjkz where zyh = '" + registryID + "'";
        DataTable dt = HelperHis.GetDataTable(strselectZ);

        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelfSelect = "SELECT Score, Flaws, NoteID,Flaws.value('(EmrNote/@NoteName)[1]','varchar(200)') as NoteName FROM SelfValuateDetail WHERE RegistryID = '" + registryID + "'";
        string strSelect = "SELECT Score, Flaws, NoteID,Flaws.value('(EmrNote/@NoteName)[1]','varchar(200)') as NoteName FROM ValuateDetail WHERE RegistryID = '" + registryID + "'";
        string strSelectEnd = "SELECT Score, Flaws, NoteID,Flaws.value('(EmrNote/@NoteName)[1]','varchar(200)') as NoteName FROM ValuateDetailEnd WHERE RegistryID = '" + registryID + "'";

        DataSet dsResult = new DataSet();

        try
        {
            if (dt != null && dt.Rows.Count != 0)
            {
                if (self)
                {
                    dsResult = Helper.GetDataSet(strSelfSelect);
                }
                else
                {
                    dsResult = Helper.GetDataSet(strSelect);
                }

            }
            else
            {
                if (self)
                {
                    dsResult = Helper.GetDataSet(strSelfSelect);
                }
                else
                {
                    dsResult = Helper.GetDataSet(strSelectEnd);
                }
            }

            return dsResult;

        }
        catch (Exception ex)
        {
            return null;
        }


    }

    [WebMethod(Description = "New valuate detail", EnableSession = false)]
    public string GetValuateDetail(bool self, string registryID, ref XmlNode flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            if (self)
            {
                command.CommandText = "SELECT Score, Flaws, NoteID FROM SelfValuateDetail WHERE RegistryID = @regID";
            }
            else
            {
                command.CommandText = "SELECT Score, Flaws, NoteID FROM ValuateDetail WHERE RegistryID = @regID";
            }
            SqlDataReader reader = command.ExecuteReader();
            XmlDocument doc = new XmlDocument();
            XmlNode valuate = doc.CreateElement(ElementNames.ValuateEmr);
            while (reader.Read())
            {
                XmlElement note = doc.CreateElement(ElementNames.EmrNotes);
                note.InnerXml = reader[1].ToString();
                XmlElement detail = (XmlElement)note.FirstChild;
                detail.SetAttribute(AttributeNames.Score, reader[0].ToString());
                detail.SetAttribute(AttributeNames.NoteID, reader[2].ToString());
                valuate.AppendChild(detail);
            }
            if (valuate.ChildNodes.Count > 0) flaws = valuate.Clone();
            reader.Close();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod(Description = "New valuate detail", EnableSession = false)]
    public string GetValuateDetailEnd(bool self, string registryID, ref XmlNode flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters[0].Value = registryID;
            if (self)
            {
                command.CommandText = "SELECT Score, Flaws, NoteID FROM SelfValuateDetailEnd WHERE RegistryID = @regID";
            }
            else
            {
                command.CommandText = "SELECT Score, Flaws, NoteID FROM ValuateDetailEnd WHERE RegistryID = @regID";
            }
            SqlDataReader reader = command.ExecuteReader();
            XmlDocument doc = new XmlDocument();
            XmlNode valuate = doc.CreateElement(ElementNames.ValuateEmr);
            while (reader.Read())
            {
                XmlElement note = doc.CreateElement(ElementNames.EmrNotes);
                note.InnerXml = reader[1].ToString();
                XmlElement detail = (XmlElement)note.FirstChild;
                detail.SetAttribute(AttributeNames.Score, reader[0].ToString());
                detail.SetAttribute(AttributeNames.NoteID, reader[2].ToString());
                valuate.AppendChild(detail);
            }
            if (valuate.ChildNodes.Count > 0) flaws = valuate.Clone();
            reader.Close();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "New valuate score", EnableSession = false)]
    public string NewValuateScore(bool self, string registryID, string doctorID, string departmentCode,
        string opcode, decimal score, int level)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@doctorID", SqlDbType.VarChar, 4);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opdate", SqlDbType.DateTime);
            command.Parameters.Add("@score", SqlDbType.Decimal);
            command.Parameters.Add("@level", SqlDbType.Int);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = doctorID;
            command.Parameters[2].Value = departmentCode;
            command.Parameters[3].Value = opcode;
            command.Parameters[4].Value = SysTime();
            command.Parameters[5].Value = score;
            command.Parameters[6].Value = level;

            if (self)
            {
                command.CommandText = "DELETE FROM SelfValuateScore WHERE RegistryID = @regID";
            }
            else
            {
                command.CommandText = "DELETE FROM ValuateScore WHERE RegistryID = @regID";
            }
            command.ExecuteNonQuery();

            if (self)
            {
                command.CommandText =
                    "INSERT INTO SelfValuateScore VALUES(@regID, @doctorID, @department, @opcode, @opdate, @score, @level)";
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ValuateScore VALUES(@regID, @doctorID, @department, @opcode, @opdate, @score, @level)";
            }
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "New valuate score", EnableSession = false)]
    public string NewValuateScoreEnd(bool self, string registryID, string doctorID, string departmentCode,
        string opcode, decimal score, int level)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@doctorID", SqlDbType.VarChar, 4);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opdate", SqlDbType.DateTime);
            command.Parameters.Add("@score", SqlDbType.Decimal);
            command.Parameters.Add("@level", SqlDbType.Int);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = doctorID;
            command.Parameters[2].Value = departmentCode;
            command.Parameters[3].Value = opcode;
            command.Parameters[4].Value = SysTime();
            command.Parameters[5].Value = score;
            command.Parameters[6].Value = level;

            if (self)
            {
                command.CommandText = "DELETE FROM SelfValuateScore WHERE RegistryID = @regID";
            }
            else
            {
                command.CommandText = "DELETE FROM ValuateScoreEnd WHERE RegistryID = @regID";
            }
            command.ExecuteNonQuery();

            if (self)
            {
                command.CommandText =
                    "INSERT INTO SelfValuateScore VALUES(@regID, @doctorID, @department, @opcode, @opdate, @score, @level)";
            }
            else
            {
                command.CommandText =
                    "INSERT INTO ValuateScoreEnd VALUES(@regID, @doctorID, @department, @opcode, @opdate, @score, @level)";
            }
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }


    [WebMethod(Description = "Get valuate score for doctors", EnableSession = false)]
    public string GetDoctorScoreForDepartment(bool self, string department, DateTime dtfrom, DateTime dtto, ref XmlNode result)
    {
        DateTime dayfrom = Convert.ToDateTime(dtfrom.ToString(StringGeneral.DateFormat) + " 00:00:00");
        DateTime dayto = Convert.ToDateTime(dtto.ToString(StringGeneral.DateFormat) + " 23:59:59");

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        SqlCommand cmd = command.Clone();

        ArrayList opcode = new ArrayList();
        try
        {
            #region Prepare sentences
            if (self)
            {
                cmd.CommandText = "SELECT DISTINCT DoctorID FROM SelfValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department";
                command.CommandText = "SELECT count(*) FROM SelfValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND ScoreLevel = @level AND DoctorID = @doctor";
            }
            else
            {
                cmd.CommandText = "SELECT DISTINCT DoctorID FROM ValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department";
                command.CommandText = "SELECT count(*) FROM ValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND ScoreLevel = @level AND DoctorID = @doctor";
            }

            command.Parameters.Add("@dtfrom", SqlDbType.DateTime);
            command.Parameters.Add("@dtto", SqlDbType.DateTime);
            command.Parameters.Add("@level", SqlDbType.Int);
            command.Parameters.Add("@doctor", SqlDbType.VarChar, 4);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters[0].Value = dayfrom;
            command.Parameters[1].Value = dayto;
            command.Parameters[4].Value = department;

            cmd.Parameters.Add("@dtfrom", SqlDbType.DateTime);
            cmd.Parameters.Add("@dtto", SqlDbType.DateTime);
            cmd.Parameters.Add("@department", SqlDbType.VarChar, 4);
            cmd.Parameters[0].Value = dayfrom;
            cmd.Parameters[1].Value = dayto;
            cmd.Parameters[2].Value = department;
            #endregion
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                opcode.Add(reader[0].ToString());
            }
            reader.Close();
            if (opcode.Count == 0)
            {
                connection.Close();
                return null;
            }
            XmlDocument doc = new XmlDocument();
            XmlElement valuate = doc.CreateElement(ElementNames.ValuateEmr);

            foreach (string code in opcode)
            {
                XmlElement doctor = doc.CreateElement(ElementNames.Doctor);
                doctor.SetAttribute(AttributeNames.Code, code);
                command.Parameters[3].Value = code;
                int emrCount = 0;

                command.Parameters[2].Value = ValuateIndex.A;
                reader = command.ExecuteReader();
                if (reader.Read()) doctor.SetAttribute(AttributeNames.A, reader[0].ToString());
                else doctor.SetAttribute(AttributeNames.A, "0");
                reader.Close();
                emrCount += Convert.ToInt16(doctor.Attributes[AttributeNames.A].Value);

                command.Parameters[2].Value = ValuateIndex.B;
                reader = command.ExecuteReader();
                if (reader.Read()) doctor.SetAttribute(AttributeNames.B, reader[0].ToString());
                else doctor.SetAttribute(AttributeNames.B, "0");
                reader.Close();
                emrCount += Convert.ToInt16(doctor.Attributes[AttributeNames.B].Value);

                command.Parameters[2].Value = ValuateIndex.C;
                reader = command.ExecuteReader();
                if (reader.Read()) doctor.SetAttribute(AttributeNames.C, reader[0].ToString());
                else doctor.SetAttribute(AttributeNames.C, "0");
                reader.Close();
                emrCount += Convert.ToInt16(doctor.Attributes[AttributeNames.C].Value);

                command.Parameters[2].Value = ValuateIndex.D;
                reader = command.ExecuteReader();
                if (reader.Read()) doctor.SetAttribute(AttributeNames.D, reader[0].ToString());
                else doctor.SetAttribute(AttributeNames.D, "0");
                reader.Close();
                emrCount += Convert.ToInt16(doctor.Attributes[AttributeNames.D].Value);

                doctor.SetAttribute(AttributeNames.Num, emrCount.ToString());
                valuate.AppendChild(doctor);
            }
            result = valuate.Clone();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get valuate score for departments", EnableSession = false)]
    public string GetDepartmentScore(bool self, DateTime dtfrom, DateTime dtto, ref XmlNode result)
    {
        DateTime dayfrom = Convert.ToDateTime(dtfrom.ToString(StringGeneral.DateFormat) + " 00:00:00");
        DateTime dayto = Convert.ToDateTime(dtto.ToString(StringGeneral.DateFormat) + " 23:59:59");

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        SqlCommand cmd = command.Clone();

        ArrayList opcode = new ArrayList();
        try
        {
            #region Prepare sentences
            if (self)
            {
                cmd.CommandText = "SELECT DISTINCT DepartmentCode FROM SelfValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto";
                command.CommandText = "SELECT count(*) FROM SelfValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND ScoreLevel = @level";
            }
            else
            {
                cmd.CommandText = "SELECT DISTINCT DepartmentCode FROM ValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto";
                command.CommandText = "SELECT count(*) FROM ValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND ScoreLevel = @level";
            }

            command.Parameters.Add("@dtfrom", SqlDbType.DateTime);
            command.Parameters.Add("@dtto", SqlDbType.DateTime);
            command.Parameters.Add("@level", SqlDbType.Int);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters[0].Value = dayfrom;
            command.Parameters[1].Value = dayto;

            cmd.Parameters.Add("@dtfrom", SqlDbType.DateTime);
            cmd.Parameters.Add("@dtto", SqlDbType.DateTime);
            cmd.Parameters[0].Value = dayfrom;
            cmd.Parameters[1].Value = dayto;
            #endregion
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                opcode.Add(reader[0].ToString());
            }
            reader.Close();
            if (opcode.Count == 0)
            {
                connection.Close();
                return null;
            }
            XmlDocument doc = new XmlDocument();
            XmlElement valuate = doc.CreateElement(ElementNames.ValuateEmr);


            foreach (string code in opcode)
            {
                XmlElement department = doc.CreateElement(ElementNames.Department);
                department.SetAttribute(AttributeNames.Code, code);
                command.Parameters[3].Value = code;
                int emrCount = 0;

                command.Parameters[2].Value = ValuateIndex.A;
                reader = command.ExecuteReader();
                if (reader.Read()) department.SetAttribute(AttributeNames.A, reader[0].ToString());
                else department.SetAttribute(AttributeNames.A, "0");
                reader.Close();
                emrCount += Convert.ToInt16(department.Attributes[AttributeNames.A].Value);

                command.Parameters[2].Value = ValuateIndex.B;
                reader = command.ExecuteReader();
                if (reader.Read()) department.SetAttribute(AttributeNames.B, reader[0].ToString());
                else department.SetAttribute(AttributeNames.B, "0");
                reader.Close();
                emrCount += Convert.ToInt16(department.Attributes[AttributeNames.B].Value);

                command.Parameters[2].Value = ValuateIndex.C;
                reader = command.ExecuteReader();
                if (reader.Read()) department.SetAttribute(AttributeNames.C, reader[0].ToString());
                else department.SetAttribute(AttributeNames.C, "0");
                reader.Close();
                emrCount += Convert.ToInt16(department.Attributes[AttributeNames.C].Value);

                command.Parameters[2].Value = ValuateIndex.D;
                reader = command.ExecuteReader();
                if (reader.Read()) department.SetAttribute(AttributeNames.D, reader[0].ToString());
                else department.SetAttribute(AttributeNames.D, "0");
                reader.Close();
                emrCount += Convert.ToInt16(department.Attributes[AttributeNames.D].Value);

                department.SetAttribute(AttributeNames.Num, emrCount.ToString());
                valuate.AppendChild(department);
            }
            result = valuate.Clone();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get patient score for a doctor", EnableSession = false)]
    public string GetPatientScoreForDoctor(bool self, string doctorID, string department,
        DateTime dtfrom, DateTime dtto, ref XmlNode result)
    {
        DateTime dayfrom = Convert.ToDateTime(dtfrom.ToString(StringGeneral.DateFormat) + " 00:00:00");
        DateTime dayto = Convert.ToDateTime(dtto.ToString(StringGeneral.DateFormat) + " 23:59:59");

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();

        try
        {
            #region Prepare sentences
            if (self)
            {
                command.CommandText = "SELECT RegistryID, ScoreLevel FROM SelfValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND DoctorID = @doctor";
            }
            else
            {
                command.CommandText = "SELECT registryID, ScoreLevel FROM ValuateScore " +
                      "WHERE Opdate BETWEEN @dtfrom AND @dtto " +
                      "AND DepartmentCode = @department " +
                      "AND DoctorID = @doctor";
            }

            command.Parameters.Add("@dtfrom", SqlDbType.DateTime);
            command.Parameters.Add("@dtto", SqlDbType.DateTime);
            command.Parameters.Add("@doctor", SqlDbType.VarChar, 4);
            command.Parameters.Add("@department", SqlDbType.VarChar, 4);
            command.Parameters[0].Value = dayfrom;
            command.Parameters[1].Value = dayto;
            command.Parameters[2].Value = doctorID;
            command.Parameters[3].Value = department;

            #endregion

            XmlDocument doc = new XmlDocument();
            XmlElement valuate = doc.CreateElement(ElementNames.ValuateEmr);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                XmlElement patient = doc.CreateElement(ElementNames.Patient);
                patient.SetAttribute(AttributeNames.RegistryID, reader[0].ToString());
                patient.SetAttribute(AttributeNames.Score, reader[1].ToString());

                valuate.AppendChild(patient);
            }
            reader.Close();
            connection.Close();
            result = valuate.Clone();
            return null;


        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    #endregion

    #region Roles
    [WebMethod(Description = "Get operators as well as their roles.")]
    public string GetOperatorsRoles(ref XmlNode operators)
    {
        string select = "SELECT * FROM AsRole";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(select, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (operators == null)
                {
                    XmlDocument doc = new XmlDocument();
                    operators = doc.CreateElement(ElementNames.Operators);
                }
                XmlElement op = FindOperator(operators, reader[0].ToString());
                XmlElement roleid = op.OwnerDocument.CreateElement(ElementNames.RoleID);
                roleid.InnerText = reader[1].ToString();
                op.AppendChild(roleid);

            }
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }
    private XmlElement FindOperator(XmlNode ops, string opcode)
    {
        foreach (XmlNode op in ops.SelectNodes(ElementNames.Operator))
        {
            if (op.Attributes[AttributeNames.Code].Value == opcode) return (XmlElement)op;
        }
        XmlElement newop = ops.OwnerDocument.CreateElement(ElementNames.Operator);
        newop.SetAttribute(AttributeNames.Code, opcode);
        ops.AppendChild(newop);
        return newop;
    }
    [WebMethod(Description = "Get operators as well as their roles.")]
    public string SetOperatorRoles(XmlNode op)
    {
        if (op == null) return null;
        //if (op.ChildNodes.Count == 0) return null;

        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction trans = connection.BeginTransaction();

        SqlCommand command = new SqlCommand("DELETE FROM AsRole WHERE Opcode='"
            + op.Attributes[AttributeNames.Code].Value + "'", connection);
        SqlCommand command2 = new SqlCommand("INSERT INTO AsRole VALUES(@opcode, @roleid)", connection);
        command.Transaction = trans;
        command2.Transaction = trans;

        command2.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
        command2.Parameters.Add("@roleid", SqlDbType.VarChar, 2);
        command2.Parameters[0].Value = op.Attributes[AttributeNames.Code].Value;

        try { command.ExecuteNonQuery(); }
        catch (Exception ex)
        {
            trans.Rollback();
            connection.Close();
            return ex.Message + "-" + ex.Source;
        }

        foreach (XmlNode roleid in op.SelectNodes(ElementNames.RoleID))
        {
            command2.Parameters[1].Value = roleid.InnerText;
            try { command2.ExecuteNonQuery(); }
            catch (Exception ex)
            {
                trans.Rollback();
                connection.Close();
                return ex.Message + "--" + ex.Source;
            }
        }
        trans.Commit();
        connection.Close();
        return null;
    }
    [WebMethod(Description = "Get roles for one operator.")]
    public string GetRolesForOneOperator(string opcode, ref XmlNode roles)
    {
        string select = "SELECT RoleID FROM AsRole WHERE opcode='" + opcode + "'";
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(select, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (roles == null)
                {
                    XmlDocument doc = new XmlDocument();
                    roles = doc.CreateElement(ElementNames.Roles);
                }
                XmlElement roleid = roles.OwnerDocument.CreateElement(ElementNames.RoleID);
                XmlElement roleid2 = roles.OwnerDocument.CreateElement(ElementNames.Role);

                roleid.InnerText = reader[0].ToString();
                roleid2.InnerText = reader[0].ToString();
                roles.AppendChild(roleid);
                roles.AppendChild(roleid2);

            }
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }
    [WebMethod(Description = "Get roles for one operator.")]
    public string GetRolesForOneOperatorHIS(string opcode, string zxtdm, ref XmlNode roles)
    {
        string select = " SELECT distinct zxtdm,cddm,cdmc,"

          + "ckdm,"
          + "cdsm,"
          + "cdcs,"
          + "ckmc,"
          + "zjdm,"
          + "tbmc,"
          + "dkfs,"
          + "crcs,"
          + "cksm,"
          + "cksfsm,"
          + "ckbt,"
         + "onlyopenthis"
 + " FROM ("
   + "SELECT sj_zxtcd.zxtdm,"
          + "sj_zxtcd.cddm,"
          + "sj_zxtcd.cdmc,"
          + "sj_zxtcd.ckdm,"
          + "sj_zxtcd.cdsm,"
          + "sj_zxtcd.cdcs,"
          + "sj_ckdy.ckmc,"
          + "sj_ckdy.zjdm,"
          + "case when sj_ckdy.ckdm is null then sj_zxtcd.tbmc else sj_ckdy.tbmc end  as tbmc,"
          + "sj_ckdy.dkfs,"
          + "sj_ckdy.crcs,"
          + "sj_ckdy.cksm,"
          + "sj_ckdy.cksfsm,"
          + "sj_ckdy.ckbt,"
         + "sj_ckdy.onlyopenthis"
     + " FROM sj_yhzczy,"
          + "sj_yhzqx,"
         + " sj_zxtcd LEFT OUTER JOIN sj_ckdy ON sj_zxtcd.ckdm = sj_ckdy.ckdm"
    + " WHERE ( sj_yhzczy.yhzdm = sj_yhzqx.yhzdm ) and "
          + "( sj_yhzqx.zxtdm = sj_zxtcd.zxtdm ) and  "
          + "( sj_yhzqx.cddm = sj_zxtcd.cddm ) and  "
          + "( sj_zxtcd.zxtdm = '" + zxtdm + "') and "
         + " ( sj_yhzczy.czydm = '" + opcode + "' )  "
 + "union all "
   + "SELECT sj_zxtcd.zxtdm,  "
          + "sj_zxtcd.cddm,"
          + "sj_zxtcd.cdmc,"
          + "sj_zxtcd.ckdm,"
          + "sj_zxtcd.cdsm,"
          + "sj_zxtcd.cdcs,"
          + "sj_ckdy.ckmc,"
          + "sj_ckdy.zjdm,"
         + "case when sj_ckdy.ckdm is null then sj_zxtcd.tbmc else sj_ckdy.tbmc end,"
          + "sj_ckdy.dkfs,"
          + "sj_ckdy.crcs,"
          + "sj_ckdy.cksm,"
          + "sj_ckdy.cksfsm,"
          + "sj_ckdy.ckbt,"
         + "sj_ckdy.onlyopenthis"
    + " FROM sj_czyqx,"
          + "sj_zxtcd LEFT OUTER JOIN sj_ckdy ON sj_zxtcd.ckdm = sj_ckdy.ckdm"
   + " WHERE ( sj_czyqx.zxtdm = sj_zxtcd.zxtdm ) and "
          + " ( sj_czyqx.cddm = sj_zxtcd.cddm ) and "
          + "( sj_czyqx.zxtdm = '" + zxtdm + "') and "
         + "( sj_czyqx.czydm = '" + opcode + "') "
+ ") a";

        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "SjsysDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(select, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (roles == null)
                {
                    XmlDocument doc = new XmlDocument();
                    roles = doc.CreateElement(ElementNames.Roles);
                }
                //XmlElement cddm = roles.OwnerDocument.CreateElement(ElementNames.cddm);
                XmlElement cdmc = roles.OwnerDocument.CreateElement(ElementNames.cdmc);
                //XmlElement ckdm = roles.OwnerDocument.CreateElement(ElementNames.ckdm);
                // cddm.InnerText = reader[0].ToString();
                cdmc.InnerText = reader[1].ToString();
                // ckdm.InnerText = reader[2].ToString();
                //roles.AppendChild(cddm);
                roles.AppendChild(cdmc);
                //roles.AppendChild(ckdm);

            }
            reader.Close();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return ex.Message + "--" + ex.Source;
        }
    }
    #endregion
    [WebMethod(Description = "Reset database", EnableSession = false)]
    public string CheckSysClient()
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "HisDB"));
        connection.Open();
        string select = "";
        SqlCommand command = new SqlCommand(select, connection);
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
        }
        connection.Close();
        return null;
    }
    [WebMethod(Description = "Reset database", EnableSession = false)]
    public string ResetDatabase()
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        string cmdText = "DELETE FROM ";

        string[] tableNames = { "EmrDocument", "EmrNote", "Trust", "Version", "ValuateScore","SelfValuateScore",
            "ValuateDetail", "SelfValuateDetail", "PrintRequisition", "UnlockEmr",
            "UncommitEmrNote", "AsRole", "PrintLog", "NotePhrase", "NoteTemplate", "Picgallery" };

        for (int k = 0; k < tableNames.Length; k++)
        {
            command.CommandText = cmdText + tableNames[k];
            try { command.ExecuteNonQuery(); }
            catch (Exception ex) { connection.Close(); return ex.Message; }
        }
        connection.Close();
        return null;
    }

    [WebMethod(Description = "Returns registryIDs of witch emr document containts the key word ")]
    public XmlNode GetRegistryIDsWithKeyword(string keyword)
    {
        string SQLSentence = "SELECT RegistryID FROM EmrDocument " +
            "WHERE ([Document].exist(N'/Emr/EmrNote/SubTitle[contains(., " +
            keyword + ")]') = 1)";
        XmlDocument doc = new XmlDocument();
        XmlElement IDs = doc.CreateElement(EmrConstant.ElementNames.RegistryIDs);
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                XmlElement registryID = doc.CreateElement(EmrConstant.ElementNames.RegistryID);
                registryID.SetAttribute(EmrConstant.AttributeNames.Code, reader[0].ToString());
                registryID.SetAttribute(EmrConstant.AttributeNames.HostName, reader[1].ToString());
                registryID.SetAttribute(EmrConstant.AttributeNames.HostIP, reader[2].ToString());
                IDs.AppendChild(registryID);
            }
            reader.Close();
            connection.Close();
            return IDs;
        } //try end
        catch (Exception ex)
        {
            //IDs.InnerText = ex.Message;
            return null;
        }
    }

    [WebMethod]
    //  public string DoneTimeForConsult(string opcode, string registryID, ref XmlNode doneTime)
    public XmlNode DoneTimeForConsult(string opcode, string registryID)
    {
        XmlNode doneTime;
        SqlHelper Helper = new SqlHelper("HisDB");

        string strSelect = @"SELECT Serial_NO,Apply_Time,Apply_Doctor_No,Dept_No,Doctor_No,Reason FROM mr_consultation WHERE  con_sign <> '2 ' AND zyh = '" + registryID + "' AND doctor_no='" + opcode + "'";
        try
        {
            DataSet ds = Helper.GetDataSet(strSelect);
            if (ds != null)
            {
                XmlDataDocument xmlDoc = new XmlDataDocument(ds);
                doneTime = (XmlNode)(xmlDoc.DocumentElement);
                return doneTime;
                //   return null;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            // return ex.Message;
            return null;
        }
    }
    [WebMethod]
    public string EnableCommitLog(string doctorID, string registryID, int series, string noteName, string patientName)
    {
        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strInsert = @"INSERT INTO EnableCommit (doctorID,registryID,series,noteName,patientName,enabled) 
                            VALUES('" + doctorID + "','" + registryID + "','" + series + "','" + noteName + "','" + patientName + "','0')";
            int iResult = Helper.ExecuteNonQuery(strInsert);
            if (iResult == 1)
                return null;
            else
                return "Insert error";
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "EnableCommitLog", ex.ToString(), RecordType.Constraint);
            return ex.Message;
        }

    }
    [WebMethod]
    public string SetEnabledStatusToOne(string registryID, int series)
    {

        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strUpdate = @"Update  EnableCommit  SET enabled = '1'  WHERE registryID= '" + registryID + "' AND series='" + series + "'";

            int iResult = Helper.ExecuteNonQuery(strUpdate);
            if (iResult == 1)
                return null;
            else
                return "Update error";
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "SetEnabledStatusToOne", ex.ToString(), RecordType.Constraint);
            return ex.Message;
        }

    }
    [WebMethod]
    public string IsEnabledCommit(string registryID, int series)
    {
        ////Select  enabled from EnableCommit  WHERE registryID= registryID AND series=resies;

        ////If error return error msg
        ////Else return enabled
        try
        {
            string strSelect = "SELECT enabled FROM EnableCommit WHERE registryID= '" + registryID + "' AND series='" + series + "'";
            SqlHelper Helper = new SqlHelper("EmrDB");

            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
                return dt.Rows[0]["enabled"].ToString();
            else if (dt != null && dt.Rows.Count == 0)
                return "No Record!";
            else
                return "Error";
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "IsEnabledCommit", ex.ToString(), RecordType.Constraint);
            return ex.Message;
        }
    }
    [WebMethod]
    public string GetEnabledStatusForOneDoctor(string doctorID, ref XmlNode result)
    {
        string strSelect = @"SELECT registryID, series, noteName, patientName FROM EnableCommit
                             WHERE doctorID= '" + doctorID + "' AND enabled='0' ORDER By registryID";
        result = null;
        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement notes = doc.CreateElement(ElementNames.EmrNotes);
                doc.AppendChild(notes);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    XmlElement note = doc.CreateElement(ElementNames.EmrNote);
                    note.SetAttribute(AttributeNames.RegistryID, dt.Rows[i]["registryID"].ToString());
                    note.SetAttribute(AttributeNames.PatientName, dt.Rows[i]["patientName"].ToString());
                    note.SetAttribute(AttributeNames.Series, dt.Rows[i]["series"].ToString());
                    note.SetAttribute(AttributeNames.NoteName, dt.Rows[i]["noteName"].ToString());

                    notes.AppendChild(note);
                }

                result = notes.Clone();
                return null;
            }
            else
            {
                return "No Record";
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "GetEnabledStatusForOneDoctor", ex.ToString(), RecordType.Constraint);
            return ex.Message;
        }
    }

    [WebMethod]
    public DataSet GetEnabledStatusForOneDoctorDT(string doctorID)
    {
        string strSelect = @"SELECT registryID as '住院号', series as '流水号', noteName as '记录名称', patientName as '患者姓名' FROM EnableCommit
                             WHERE doctorID= '" + doctorID + "' AND enabled='0' ORDER By registryID";

        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            DataSet dt = Helper.GetDataSet(strSelect);
            //dt.TableName = "table";
            if (dt.Tables[0] != null && dt.Tables[0].Rows.Count != 0)
            {
                return dt;

            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "GetEnabledStatusForOneDoctorDT", ex.ToString(), RecordType.Constraint);
            return null;
        }
    }
    [WebMethod]
    public bool GetSignMessage(string registryID, string strNoteID, int Series, ref string SrcMessage, ref string SignMessage, ref string strDateTime)
    // public bool GetSignMessage(string registryID, string strNoteID, int Series)
    {
        //string SrcMessage = "";
        //string SignMessage = "";
        //string strDateTime = "";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string NoteIDSeries = MakeNoteIDSeries(strNoteID, Series);
        SrcMessage = "";
        SignMessage = "";
        strDateTime = "";
        bool blResult = false;
        try
        {
            string strSelectSign = "  select Sign,signdate from sign  where registryid = '" + registryID + "' and noteidseries = '" + NoteIDSeries + "'";
            string strSelectNote = " select NoteDocument from EmrNote  where registryid = '" + registryID + "' and noteidseries = '" + NoteIDSeries + "'";

            DataTable dt = Helper.GetDataTable(strSelectSign);
            if (dt != null && dt.Rows.Count != 0)
            {
                SignMessage = dt.Rows[0]["Sign"].ToString();
                strDateTime = dt.Rows[0]["signdate"].ToString();
                if (SignMessage != "")
                {
                    DataTable dtScr = Helper.GetDataTable(strSelectNote);
                    if (dtScr != null && dtScr.Rows.Count != 0)
                    {
                        SrcMessage = dtScr.Rows[0]["NoteDocument"].ToString();
                        blResult = true;
                    }
                }


            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteLog("", LogType.Error, "", "GetSignMessage", ex.ToString(), RecordType.Constraint);
        }
        return blResult;
    }

    [WebMethod]
    public bool UpdateOperator(string Code, string TecqTitle, string TitleLevel, string LevelCode)
    {
        string strUpdate = "update Operator set TecqTitle = '" + TecqTitle + "',TitleLevel = '" + TitleLevel + "',LevelCode = '" + LevelCode + "' where Code = '" + Code + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool blResult = false;
        try
        {

            blResult = Helper.ExecuteSql(strUpdate);

        }
        catch (Exception ex)
        {

        }
        return blResult;
    }

    [WebMethod]
    public DataSet SelectOperator(string Code)
    {
        string strSelect = "select * from Operator where code = '" + Code + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {

            dt = Helper.GetDataSet(strSelect);

        }
        catch (Exception ex)
        {

        }
        return dt;
    }

    [WebMethod]
    public DataSet GetImageUrl(string RegistryID)
    {
        string strSelect = "select * from Check_Report where zyh = '" + RegistryID + "' ";
        SqlHelper Helper = new SqlHelper("HisDB");
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
    public DataSet GetImage(string RegistryID)
    {
        string strSelect = "select * from Image_Temp where RegistryID = '" + RegistryID + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");
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
    public string GetXmlDocument(string RegistryID)
    {
        string strSelect = "select Document from EmrDocument where RegistryID = '" + RegistryID + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataTable dt = new DataTable();
        string strResult = "";
        try
        {

            dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                strResult = dt.Rows[0]["Document"].ToString();
            }


        }
        catch (Exception ex)
        {
            dt = null;
        }
        return strResult;
    }

    [WebMethod]
    public bool EnableChangeStatus(string RegistryID, string Series)
    {
        string strUpdate = @"Update EmrDocument set document.modify ('replace value of (/Emr/EmrNote[@Series= " + Series + "]/@NoteStatus)[1] with 0')  where registryid = '" + RegistryID + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool blResult = false;
        try
        {
            Helper.ExecuteNonQuery(strUpdate);
            blResult = true;
        }
        catch (Exception ex)
        {

        }
        return blResult;
    }



    [WebMethod]
    public DataSet GetChanges(string opCode, string strDept, DateTime Start, DateTime End)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");

        DataSet dt = new DataSet();
        try
        {
            string strSelect = "";
            if (opCode != "$" && strDept != "$")
            {
                strSelect = @"
select opdate as '日期',
 Reasion.value('(/Reason/@Doctor)[1]','varchar(200)') as '医师名',
Reasion.value('(/Reason/@NoteName)[1]','varchar(200)') as '记录名称',
Reasion.value('(/Reason)[1]','varchar(2000)') as '返修原因'
from UncommitEmrNote where opdate between '" + Start + "' and '" + End + "' and Opcode = '" + opCode + "' and DeparmentCode = '" + strDept + "'";
            }
            if (opCode == "$" && strDept == "$")
            {
                strSelect = @"
select opdate as '日期',
 Reasion.value('(/Reason/@Doctor)[1]','varchar(200)') as '医师名',
Reasion.value('(/Reason/@NoteName)[1]','varchar(200)') as '记录名称',
Reasion.value('(/Reason)[1]','varchar(2000)') as '返修原因'
from UncommitEmrNote where opdate between '" + Start + "' and '" + End + "'";
            }
            if (opCode == "$" && strDept != "$")
            {

                strSelect = @"
select opdate as '日期',
 Reasion.value('(/Reason/@Doctor)[1]','varchar(200)') as '医师名',
Reasion.value('(/Reason/@NoteName)[1]','varchar(200)') as '记录名称',
Reasion.value('(/Reason)[1]','varchar(2000)') as '返修原因'
from UncommitEmrNote where opdate between '" + Start + "' and '" + End + "'  and DeparmentCode = '" + strDept + "'";
            }
            if (opCode != "$" && strDept == "$")
            {
                strSelect = @"
select opdate as '日期',
 Reasion.value('(/Reason/@Doctor)[1]','varchar(200)') as '医师名',
Reasion.value('(/Reason/@NoteName)[1]','varchar(200)') as '记录名称',
Reasion.value('(/Reason)[1]','varchar(2000)') as '返修原因'
from UncommitEmrNote where opdate between '" + Start + "' and '" + End + "'  and Opcode = '" + opCode + "'";
            }

            dt = Helper.GetDataSet(strSelect);

        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;

    }

    [WebMethod]
    public int Save(string strRegistryID, string strInSituation, string strSubjective, string strExam, string strDiagnose)
    {
        int iResult = -1;
        try
        {
            string strSelect = "SELECT InSituation FROM TD_BasicInfo WHERE RegistryID = '" + strRegistryID + "' ";
            SqlHelper Helper = new SqlHelper("EmrDB");
            DataTable dt = Helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                string strUpdate = "UPDATE TD_BasicInfo SET InSituation = '" + strInSituation + "',Subjective = '" + strSubjective + "',Exam = '" + strExam + "',Diagnose = '" + strDiagnose + "' WHERE RegistryID = '" + strRegistryID + "'";
                iResult = Helper.ExecuteNonQuery(strUpdate);
            }
            else
            {
                string strSave = "INSERT INTO TD_BasicInfo  (InSituation,Subjective,Exam,Diagnose,RegistryID) VALUES( '" + strInSituation + "', '" + strSubjective + "','" + strExam + "','" + strDiagnose + "','" + strRegistryID + "')";
                iResult = Helper.ExecuteNonQuery(strSave);
            }

        }
        catch (Exception ex)
        {

        }
        return iResult;

    }

    [WebMethod]
    public DataSet GetAllBasic(string strRegistryID)
    {
        DataSet dt = new DataSet();
        try
        {
            string strSelect = "SELECT * FROM TD_BasicInfo WHERE RegistryID = '" + strRegistryID + "' ";
            SqlHelper Helper = new SqlHelper("EmrDB");
            dt = Helper.GetDataSet(strSelect);


        }
        catch (Exception ex)
        {
            dt = null;
        }
        return dt;

    }
    /// <summary>
    /// IsHospital 0 不属于医院范围
    /// IsHospital 1 属于医院范围
    /// </summary>
    /// <param name="strDepartmentCode"></param>
    /// <param name="IsHospital"></param>
    /// <returns></returns>
    [WebMethod]
    public DataSet GetIllName(string strDepartmentCode, int IsHospital)
    {
        DataSet dt = new DataSet();
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {
            if (IsHospital == 0)
            {
                string strSelect = "SELECT * FROM TB_Ill WHERE DepartmentCode = '" + strDepartmentCode + "' ";
                dt = Helper.GetDataSet(strSelect);
            }
            else
            {
                string strSelect = "SELECT * FROM TB_Ill WHERE IsHospital = '" + IsHospital + "' ";

                dt = Helper.GetDataSet(strSelect);
            }
        }
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }

    [WebMethod]
    public DataSet GetTemplateIllName(string pk)
    {
        DataSet dt = new DataSet();
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            string strSelect = "SELECT DepartmentCode,DoctorID,Illtype,IllName,Creator,CreateDate,NoteID,NoteName,Sex,Type,TypeName	  FROM NoteTemplate WHERE pk = '" + pk + "' ";

            dt = Helper.GetDataSet(strSelect);

        }
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod]
    public DataSet GetTemplateIllNameZlg(string pk)
    {
        DataSet dt = new DataSet();
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            string strSelect = "SELECT DepartmentCode,DoctorID,Illtype,IllName,Creator,CreateDate,NoteID,NoteName,Sex,Type,TypeName	  FROM TD_NoteTemplate WHERE pk = '" + pk + "' ";

            dt = Helper.GetDataSet(strSelect);

        }
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod]
    public DataSet GetTemplateType()
    {
        DataSet dt = new DataSet();
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            string strSelect = "SELECT * from TB_TemplateType";

            dt = Helper.GetDataSet(strSelect);


        }
        catch (Exception ex)
        {
            dt = null;

        }
        return dt;
    }
    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public bool UpdateNoteTemplateEx(long pk, XmlNode note, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
    {
        string Query = "UPDATE NoteTemplate SET Note =  cast('" + note.OuterXml +
            " ' as xml ),IllType = '" + IllType + "',IllName = '" + IllName + "',Creator = '" + Creator + "', CreateDate = '" + CreateDate + "',  Sex = '" + Sex + "', Type = '" + Type + "', TypeName = '" + TypeName + "',  NoteID = '" + NoteID + "',  NoteName = '" + NoteName + "', TemplateName = '" + TemplateName + "'  WHERE pk = '" + pk + "'";

        if (IllType == null)
        {
            Query = "UPDATE NoteTemplate SET Note =  cast('" + note.OuterXml +
                       " ' as xml ),TemplateName = '" + TemplateName + "'  WHERE pk = '" + pk + "'";
        }

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("UpdateNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Update, e.Message);
            return EmrConstant.Return.Failed;
        }


    }
    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public bool UpdateNoteTemplateExZlg(long pk, XmlNode note, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
    {
        string Query = "UPDATE TD_NoteTemplate SET Note =  cast('" + note.OuterXml +
            " ' as xml ),IllType = '" + IllType + "',IllName = '" + IllName + "',Creator = '" + Creator + "', CreateDate = '" + CreateDate + "',  Sex = '" + Sex + "', Type = '" + Type + "', TypeName = '" + TypeName + "',  NoteID = '" + NoteID + "',  NoteName = '" + NoteName + "', TemplateName = '" + TemplateName + "'  WHERE pk = '" + pk + "'";

        if (IllType == null)
        {
            Query = "UPDATE TD_NoteTemplate SET Note =  cast('" + note.OuterXml +
                       " ' as xml ),TemplateName = '" + TemplateName + "'  WHERE pk = '" + pk + "'";
        }

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlDataAdapter myCommand = new SqlDataAdapter(Query, CS);
            DataSet myDataSet = new DataSet();
            myCommand.Fill(myDataSet, "Results");
            return EmrConstant.Return.Successful;
        }
        catch (Exception e)
        {
            ErrorLog("UpdateNoteTemplate", "NoteTemplate",
                           EmrConstant.SqlOperations.Update, e.Message);
            return EmrConstant.Return.Failed;
        }


    }

    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public long InSertNoteTemplateEx(XmlNode note, string DoctorID, string DepartmentCode, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
    {
        string Query = @"INSERT INTO NoteTemplate  (Note,DoctorID,DepartmentCode,IllType,IllName,Creator,CreateDate,Sex,Type,TypeName,NoteID,NoteName,TemplateName)
             VALUES('" + note.OuterXml + "','" + DoctorID + "','" + DepartmentCode + "','" + IllType + "','" + IllName + "','" + Creator + "','" + CreateDate + "','" + Sex + "','" + Type + "','" + TypeName + "','" + NoteID + "','" + NoteName + "','" + TemplateName + "')";

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPk(CS);

            CS.Close();
            return pk;
        }
        catch (Exception e)
        {
            ErrorLog("NewNoteTemplate", "NoteTemplate", SqlOperations.Insert, e.Message);
            return -1;
        }

    }
    [WebMethod(Description = "Update a note template", EnableSession = false)]
    public long InSertNoteTemplateExZlg(XmlNode note, string DoctorID, string DepartmentCode, string IllType, string IllName, string Creator, string CreateDate, string Sex, string Type, string TypeName, string NoteID, string NoteName, string TemplateName)
    {
        string Query = @"INSERT INTO TD_NoteTemplate  (Note,DoctorID,DepartmentCode,IllType,IllName,Creator,CreateDate,Sex,Type,TypeName,NoteID,NoteName,TemplateName)
             VALUES('" + note.OuterXml + "','" + DoctorID + "','" + DepartmentCode + "','" + IllType + "','" + IllName + "','" + Creator + "','" + CreateDate + "','" + Sex + "','" + Type + "','" + TypeName + "','" + NoteID + "','" + NoteName + "','" + TemplateName + "')";

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPkZlg(CS);

            CS.Close();
            return pk;
        }
        catch (Exception e)
        {
            ErrorLog("NewNoteTemplate", "NoteTemplate", SqlOperations.Insert, e.Message);
            return -1;
        }

    }
    [WebMethod]
    public int InsertIll(string IllID, string IllName, string DepartmentCode, int Flag)
    {
        int iResult = -1;
        string strInsert = @"INSERT INTO TB_Ill (IllType,IllName,DepartmentCode,IsHospital) 
        VALUES('" + IllID + "','" + IllName + "','" + DepartmentCode + "','" + Flag + "')";
        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            iResult = Helper.ExecuteNonQuery(strInsert);
        }
        catch (Exception ex)
        {
            ErrorLog("InsertIll", "InsertIll", SqlOperations.Insert, ex.Message);

        }
        return iResult;
    }
    [WebMethod(Description = "Get hospital template pks", EnableSession = false)]
    public DataSet GetHospitalTemplatePkz(int i, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '----'";
        string Query1 = "SELECT DISTINCT IllName from NoteTemplate WHERE DepartmentCode =  '----'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from NoteTemplate WHERE DepartmentCode =  '----' AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from NoteTemplate WHERE DepartmentCode =  '----' AND IllName='" + illname + "' AND NoteName='" + notename + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch (Exception e)
        {

            return null;
        }
    }
    [WebMethod(Description = "Get hospital template pks", EnableSession = false)]
    public DataSet GetHospitalTemplatePkzZlg(int i, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '----'";
        string Query1 = "SELECT DISTINCT IllName from TD_NoteTemplate WHERE DepartmentCode =  '----'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from TD_NoteTemplate WHERE DepartmentCode =  '----' AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from TD_NoteTemplate WHERE DepartmentCode =  '----' AND IllName='" + illname + "' AND NoteName='" + notename + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch (Exception e)
        {

            return null;
        }
    }
    [WebMethod(Description = "Get department template pks", EnableSession = false)]
    public DataSet GetDepartTemplatePkz(int i, string departCode, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        string Query1 = "SELECT DISTINCT IllName from NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from NoteTemplate WHERE DepartmentCode =  '" + departCode + "'AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from NoteTemplate WHERE DepartmentCode  =  '" + departCode + "'AND IllName='" + illname + "' AND NoteName='" + notename + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch (Exception e)
        {

            return null;
        }
    }
    [WebMethod(Description = "Get department template pks", EnableSession = false)]
    public DataSet GetDepartTemplatePkzZlg(int i, string departCode, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        string Query1 = "SELECT DISTINCT IllName from TD_NoteTemplate WHERE DepartmentCode =  '" + departCode + "'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from TD_NoteTemplate WHERE DepartmentCode =  '" + departCode + "'AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from TD_NoteTemplate WHERE DepartmentCode  =  '" + departCode + "'AND IllName='" + illname + "' AND NoteName='" + notename + "'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch (Exception e)
        {

            return null;
        }
    }
    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public DataSet GetPersonTemplatePkz(int i, string doctorID, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DoctorID =  '" + doctorID + "'";
        string Query1 = "SELECT DISTINCT IllName from NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "' AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "' AND IllName='" + illname + "' AND NoteName='" + notename + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch (Exception e)
        {

            return null;
        }

    }
    [WebMethod(Description = "Get person template pks", EnableSession = false)]
    public DataSet GetPersonTemplatePkzZlg(int i, string doctorID, string illname, string notename)
    {
        //string Query = "SELECT pk from NoteTemplate WHERE DoctorID =  '" + doctorID + "'";
        string Query1 = "SELECT DISTINCT IllName from TD_NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "'";
        string Query2 = "SELECT DISTINCT NoteName,NoteID from TD_NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "' AND IllName='" + illname + "'";
        string Query3 = "SELECT TemplateName,pk,Sex  from TD_NoteTemplate WHERE DepartmentCode ='####' AND DoctorID =  '" + doctorID + "' AND IllName='" + illname + "' AND NoteName='" + notename + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        try
        {
            switch (i)
            {
                case 2:
                    dt = Helper.GetDataSet(Query1);
                    break;
                case 3:
                    dt = Helper.GetDataSet(Query2);
                    break;
                case 4:
                    dt = Helper.GetDataSet(Query3);
                    break;
                default:
                    return null;

            }
            return dt;

        }
        catch
        {

            return null;
        }

    }
    /*----------------------------------------------------------------------------------------------
  
  * Return a xmlnode which include all pictures of  the hospitol.
  * Return null if there is no picture for the hospitol.
  * Return null when ERROR.
  -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Return a xmlnode which include all pictures", EnableSession = false)]
    public XmlNode GetPicturesNew()
    {
        try
        {
            string SQLSentence = "SELECT CONVERT(xml, Picture) FROM PicGallery ";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "<PicGallery>";
            if (reader.Read())
            {
                do
                {
                    strXml += reader[0].ToString();
                } while (reader.Read() == true);
            }
            strXml += "</PicGallery>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            XmlNode picGallery = doc.Clone();
            connection.Close();
            return picGallery;
        }
        catch (Exception)
        {
            return null;
        }
    }
    /*----------------------------------------------------------------------------------------------
    * parameter:   picName(string)
    *              departmentCode(string)
    * Delete a picture
    -----------------------------------------------------------------------------------------------*/
    [WebMethod(Description = "Delete a picture", EnableSession = false)]
    public void DeletePictureNew(string picName)
    {
        try
        {
            string SQLSentence =
                "DELETE FROM PicGallery WHERE PicName='" + picName + "'";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            connection.Close();
        }
        catch (Exception)
        {
        }
    }
    [WebMethod(Description = "Return a pks for emrBlocks", EnableSession = false)]
    public Boolean GetEmrBlockKeysNew(ref XmlNode blockKeys)
    {
        try
        {
            string SQLSentence = "SELECT pk FROM EmrBlocks ORDER BY pk ASC";

            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();
            SqlCommand command = new SqlCommand(SQLSentence, connection);
            command.CommandType = CommandType.Text;
            SqlDataReader reader = command.ExecuteReader();

            string strXml = "<Pks></Pks>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXml);
            while (reader.Read())
            {
                XmlElement pk = doc.CreateElement(EmrConstant.ElementNames.Pk);
                pk.InnerText = reader[0].ToString();
                doc.DocumentElement.AppendChild(pk);
            }
            blockKeys = doc.Clone();
            connection.Close();
            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("GetEmrBlocks", "Blocks", EmrConstant.SqlOperations.Select, ex.Message);
            return EmrConstant.Return.Failed;
        }
    }
    [WebMethod]
    public void InsertLink(string registryID, XmlNode xmlNode, string Lable)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Insert = "INSERT TD_Link  (registryID,Link,BookMark) Values('" + registryID + "','" + xmlNode.OuterXml + "', '" + Lable + "')";
        try
        {
            Helper.ExecuteSql(Insert);
        }
        catch (Exception ex)
        {
        };


    }

    [WebMethod]
    public DataSet GetLink(string registryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT Link,BookMark FROM  TD_Link  WHERE registryID = '" + registryID + "'";
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
    public DataSet GetPageInf(string pagename)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT rightmargin,leftmargin FROM  PageSize  WHERE pagename = '" + pagename + "'";
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
    public void UpdatePageInf(string pagename, float right, float left)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string up = "Update PageSize set rightmargin ='" + right + "' , leftmargin ='" + left + "' WHERE pagename = '" + pagename + "'";
        DataTable dt = new DataTable();
        try
        {
            Helper.ExecuteSql(up);

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
        return;
    }


    [WebMethod]
    public DataSet GetDoctorListWithPTemplate()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        SqlHelper HelperHis = new SqlHelper("HisDB");
        string SELECT = "SELECT  DISTINCT doctorid FROM noteTemplate WHERE departmentcode = '####' AND Ischecked IS NULL ";

        DataSet ds = new DataSet();
        DataSet dsNew = new DataSet();
        DataTable dt = new DataTable();
        dt.TableName = "Table";
        dt.Columns.Add("Name");
        dt.Columns.Add("ID");
        try
        {
            ds = Helper.GetDataSet(SELECT);
            if (ds != null && ds.Tables.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {


                    DataTable dtTemp = new DataTable();

                    string strSelect = "SELECT ysbm,ysm FROM tysm WHERE  ysbm = '" + ds.Tables[0].Rows[i]["doctorid"].ToString().Trim() + "'";
                    dtTemp = HelperHis.GetDataTable(strSelect);
                    if (dtTemp != null && dtTemp.Rows.Count != 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Name"] = dtTemp.Rows[0]["ysm"].ToString();
                        dr["ID"] = dtTemp.Rows[0]["ysbm"].ToString();
                        dt.Rows.Add(dr);

                    }

                }
                dsNew.Tables.Add(dt);
            }
        }
        catch (Exception ex)
        {
            dsNew = null;
        }
        return dsNew;
    }
    [WebMethod]
    public DataSet GetDoctorListWithPTemplateZlg()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        SqlHelper HelperHis = new SqlHelper("HisDB");
        string SELECT = "SELECT  DISTINCT doctorid FROM TD_noteTemplate WHERE departmentcode = '####' AND Ischecked IS NULL ";

        DataSet ds = new DataSet();
        DataSet dsNew = new DataSet();
        DataTable dt = new DataTable();
        dt.TableName = "Table";
        dt.Columns.Add("Name");
        dt.Columns.Add("ID");
        try
        {
            ds = Helper.GetDataSet(SELECT);
            if (ds != null && ds.Tables.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {


                    DataTable dtTemp = new DataTable();

                    string strSelect = "SELECT ysbm,ysm FROM tysm WHERE  ysbm = '" + ds.Tables[0].Rows[i]["doctorid"].ToString().Trim() + "'";
                    dtTemp = HelperHis.GetDataTable(strSelect);
                    if (dtTemp != null && dtTemp.Rows.Count != 0)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Name"] = dtTemp.Rows[0]["ysm"].ToString();
                        dr["ID"] = dtTemp.Rows[0]["ysbm"].ToString();
                        dt.Rows.Add(dr);

                    }

                }
                dsNew.Tables.Add(dt);
            }
        }
        catch (Exception ex)
        {
            dsNew = null;
        }
        return dsNew;
    }
    [WebMethod]
    public DataSet GetTemplateDetail(string DoctorID)
    {
        string str = "SELECT  Note,pk  FROM noteTemplate WHERE departmentcode = '####' AND doctorid = '" + DoctorID + "' AND Ischecked IS NULL ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet ds = new DataSet();
        DataTable dtTemp = new DataTable();
        dtTemp.TableName = "Table";
        dtTemp.Columns.Add("标识");
        dtTemp.Columns.Add("类型标识");
        dtTemp.Columns.Add("模板类型");
        dtTemp.Columns.Add("模板名称");

        try
        {
            DataTable dt = Helper.GetDataTable(str);
            if (dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strTemplateName = "";
                    string NoteName = "";
                    string pk = "";
                    string NoteID = "";
                    // string CreateDate = DateTime.Now.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dt.Rows[i]["Note"].ToString());
                    XmlElement ele = doc.DocumentElement;
                    if (ele.Attributes["Name"] != null)
                    {
                        strTemplateName = ele.Attributes["Name"].Value.ToString();
                    }
                    if (ele.Attributes["NoteName"] != null)
                    {
                        NoteName = ele.Attributes["NoteName"].Value.ToString();
                    }
                    if (ele.Attributes["NoteID"] != null)
                    {
                        NoteID = ele.Attributes["NoteID"].Value.ToString();
                    }


                    pk = dt.Rows[i]["pk"].ToString();

                    DataRow dr = dtTemp.NewRow();
                    dr["类型标识"] = NoteID;
                    dr["模板类型"] = NoteName;
                    dr["模板名称"] = strTemplateName;
                    dr["标识"] = pk;
                    dtTemp.Rows.Add(dr);

                }
            }
            ds.Tables.Add(dtTemp);

        }
        catch (Exception ex)
        {
            ds = null;
        }
        return ds;
    }
    [WebMethod]
    public DataSet GetTemplateDetailZlg(string DoctorID)
    {
        string str = "SELECT  Note,pk  FROM TD_noteTemplate WHERE departmentcode = '####' AND doctorid = '" + DoctorID + "' AND Ischecked IS NULL ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet ds = new DataSet();
        DataTable dtTemp = new DataTable();
        dtTemp.TableName = "Table";
        dtTemp.Columns.Add("标识");
        dtTemp.Columns.Add("类型标识");
        dtTemp.Columns.Add("模板类型");
        dtTemp.Columns.Add("模板名称");

        try
        {
            DataTable dt = Helper.GetDataTable(str);
            if (dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strTemplateName = "";
                    string NoteName = "";
                    string pk = "";
                    string NoteID = "";
                    // string CreateDate = DateTime.Now.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dt.Rows[i]["Note"].ToString());
                    XmlElement ele = doc.DocumentElement;
                    if (ele.Attributes["Name"] != null)
                    {
                        strTemplateName = ele.Attributes["Name"].Value.ToString();
                    }
                    if (ele.Attributes["NoteName"] != null)
                    {
                        NoteName = ele.Attributes["NoteName"].Value.ToString();
                    }
                    if (ele.Attributes["NoteID"] != null)
                    {
                        NoteID = ele.Attributes["NoteID"].Value.ToString();
                    }


                    pk = dt.Rows[i]["pk"].ToString();

                    DataRow dr = dtTemp.NewRow();
                    dr["类型标识"] = NoteID;
                    dr["模板类型"] = NoteName;
                    dr["模板名称"] = strTemplateName;
                    dr["标识"] = pk;
                    dtTemp.Rows.Add(dr);

                }
            }
            ds.Tables.Add(dtTemp);

        }
        catch (Exception)
        {
            ds = null;
        }
        return ds;
    }
    [WebMethod]
    public bool UpdateTemplatePTag(string pk, string strDoctor, string strDoctorID)
    {
        string str = "UPDATE Notetemplate set IsChecked = '1',CheckCode = '" + strDoctorID + "',CheckName = '" + strDoctor + "' ,CheckDate = '" + DateTime.Now + "' WHERE pk = '" + pk + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool blResult = false;
        try
        {
            int i = Helper.ExecuteNonQuery(str);
            if (i == 1) blResult = true;

        }
        catch (Exception ex)
        {

        }
        return blResult;
    }
    [WebMethod]
    public bool UpdateTemplatePTagZlg(string pk, string strDoctor, string strDoctorID)
    {
        string str = "UPDATE TD_Notetemplate set IsChecked = '1',CheckCode = '" + strDoctorID + "',CheckName = '" + strDoctor + "' ,CheckDate = '" + DateTime.Now + "' WHERE pk = '" + pk + "' ";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool blResult = false;
        try
        {
            int i = Helper.ExecuteNonQuery(str);
            if (i == 1) blResult = true;

        }
        catch (Exception ex)
        {

        }
        return blResult;
    }
    [WebMethod]
    public void Setfanxiu(string registryID, int series)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string up = "update emrdocument set document.modify('insert (  attribute fanxiu {\"Yes\" }  ) into (/Emr/EmrNote[@Series = " + series + "])[1]') where RegistryID='" + registryID + "'";
        //DataTable dt = new DataTable();
        try
        {
            bool res = Helper.ExecuteSql(up);

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
        return;
    }
    [WebMethod(Description = "Import Pattern's Template", EnableSession = false)]
    public void ImpPatternDoc(string noteID, XmlNode note)
    {
        string Query = "INSERT TB_PatternDoc (NoteID ,Note) VALUES( '" +
            noteID + "',cast('" + note.OuterXml + " ' as xml )  )";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPk(CS);

            CS.Close();

        }
        catch (Exception e)
        {
            ErrorLog("Pattern's Template", "Pattern's Template", SqlOperations.Insert, e.Message);

        }
        return;
    }
    [WebMethod(Description = "Import Pattern's Template", EnableSession = false)]
    public void ImpPatternDocZlg(string noteID, XmlNode note)
    {
        string Query = "INSERT TB_PatternDoc (NoteID ,Note) VALUES( '" +
            noteID + "',cast('" + note.OuterXml + " ' as xml )  )";
        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            long pk = GetMaxPkZlg(CS);

            CS.Close();

        }
        catch (Exception e)
        {
            ErrorLog("Pattern's Template", "Pattern's Template", SqlOperations.Insert, e.Message);

        }
        return;
    }
    [WebMethod(Description = "Search Pattern's Template", EnableSession = false)]
    public bool SehPatternDoc(string noteID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Query = "Select * from TB_PatternDoc Where noteid='" + noteID + "'";
        try
        {
            DataTable dt = Helper.GetDataTable(Query);
            if (dt.Rows.Count > 0)
            {

                return false;
            }
            else return true;
        }
        catch (Exception ep)
        {
            return true;
        }
    }
    [WebMethod]
    public DataSet ExpPatternDoc()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        DataSet dt = new DataSet();
        string Query = "Select * from TB_PatternDoc";
        try
        {
            dt = Helper.GetDataSet(Query);

        }
        catch (Exception ep)
        {

        }
        return dt;
    }
    [WebMethod(Description = "Delete Pattern's Template", EnableSession = false)]
    public void DelPatternDoc(string noteID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string del = "Delete from TB_PatternDoc Where noteid='" + noteID + "'";
        DataTable dt = new DataTable();
        try
        {
            Helper.ExecuteSql(del);

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
        return;
    }
    [WebMethod(Description = "Get Pattern's XmlFile", EnableSession = false)]
    public void GetPatternDoc(string noteID, ref XmlNode note)
    {

        string Query = "Select note from TB_PatternDoc Where noteid='" + noteID + "'";

        try
        {
            SqlConnection CS = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            CS.Open();
            SqlCommand command = new SqlCommand(Query, CS);
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return;
            if (note == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                note = doc.Clone();
            }
            else
            {
                note.InnerXml = reader[0].ToString();
            }
            return;
        }
        catch (Exception e)
        {
            //ErrorLog("RemoveNoteTemplate", "NoteTemplate",
            //               EmrConstant.SqlOperations.Delete, e.Message);
            return;

        }
    }

    [WebMethod]
    public string GetImageSign(string opCode)
    {
        SqlHelper helper = new SqlHelper("EmrDB");
        string strSelect = "select image from td_signimage where OpCode = '" + opCode + "'";
        string strResult = "";
        try
        {
            DataTable dt = helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(dt.Rows[0]["image"].ToString());
                strResult = xmldoc.DocumentElement.InnerText;
            }
        }
        catch (Exception ex)
        {
            strResult = "";

        }
        return strResult;


    }



    [WebMethod]
    public string GetIsKeyUser(string opCode)
    {
        SqlHelper helper = new SqlHelper("EmrDB");
        string strSelect = "select IsKey from TD_IsKey where Code = '" + opCode + "'";
        string strResult = "";
        try
        {
            DataTable dt = helper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                strResult = dt.Rows[0]["IsKey"].ToString();
            }
        }
        catch (Exception ex)
        {
            strResult = "";

        }
        return strResult;


    }

    [WebMethod]
    public bool UpdateUpload_Time(string Server_Time, string RegistryID, string NoteIDSeries)
    {
        bool result;
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {
            string UpdateUpload_Time = @"update signContent set Upload_Time = '" + Server_Time + "'where RegistryID = '" + RegistryID + "' and NoteIDSeries = '" + NoteIDSeries + "'";
            result = Helper.ExecuteSql(UpdateUpload_Time);
        }
        catch
        {
            result = false;
        }
        return result;
    }


    [WebMethod(Description = "Add a new EmrNote")]
    public string test()
    {
        string registryID = "00091957";
        string ArchieveNum = "066002";
        string noteForEmrDoc = "<EmrNote WrittenDate=\"2010-06-04\" WrittenTime=\"10:21\" NoteID=\"08\" NoteName=\"青光眼手术记录\" Header=\"none\" Unique=\"No\" NoteStatus=\"1\" WriterID=\"0000\" Writer=\"超级用户\" CheckerID=\"\" Checker=\"\" CheckedDate=\"\" FinalCheckerID=\"\" FinalChecker=\"\" FinalCheckedDate=\"\" Sign1=\"\" Sign2=\"\" Sign3=\"手术医师：\" Lwt=\"129200916615600000\" Merge=\"0\" StartTime=\"None\" Sex=\"No\" Single=\"no\" Series=\"0\"><SubTitle TitleName=\"手术日期：\">2010年06月04日</SubTitle><SubTitle TitleName=\"手术前诊断：\"></SubTitle><SubTitle TitleName=\"手术后诊断：\"></SubTitle><SubTitle TitleName=\"手术医师：\">选择一项。</SubTitle><SubTitle TitleName=\"助手：\">选择一项。</SubTitle><SubTitle TitleName=\"护士：\">选择一项。</SubTitle><SubTitle TitleName=\"麻醉师：\"></SubTitle><SubTitle TitleName=\"麻醉方法：\">表面麻醉药名剂量ml球后麻醉药名剂量ml眼轮匝肌药名剂量ml灌注液：ml</SubTitle><SubTitle TitleName=\"辅助用药：\"></SubTitle><SubTitle TitleName=\"手术经过：\"></SubTitle></EmrNote>";
        string noteForWordDoc = "<EmrNote>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</EmrNote>";
        string Sign = "MIKE6wYJKoZIhvcNAQcCoIKE3DCChNgCAQExCzAJBgUrDgMCGgUAMIJ7NgYJKoZIhvcNAQcBoIJ7JwSCeyM8RW1yTm90ZT5VRXNEQkJRQUJnQUlBQUFBSVFBYVF4L1Y3UUVBQUNNTUFBQVRBQWdDVzBOdmJuUmxiblJmVkhsd1pYTmRMbmh0YkNDaUJBSW9vQUFDQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQURFVmsxdjJ6QU12US9ZZnpCMExXS2xIVEFNUTV3ZXR2VzRGVmoyQTFTSmRvVEpraUNxSC9uM28rTllDOUxhRHBxNnVSaUlIYjczU0QyU1dsdy8xU1o3Z0lEYTJZSmQ1bk9XZ1pWT2FWc1Y3TS9xWnZhRlpSaUZWY0k0Q3dYYkFMTHI1Y2NQaTlYR0EyWVViYkZnNnhqOVY4NVJycUVXbURzUGxyNlVMdFFpMHM5UWNTL2tYMUVCdjVyUFAzUHBiQVFiWjdIQllNdkZMeElRdElMc1ZvVDRVOVRFd3g5ZFVMeDBMbG9YQVhPQ1k5bTNOcTZoTHBqdzNtZ3BJZ25uRDFZZGtNNWNXV29KeXNuN21xanlCczRISndHUlVxdE5ucUF2R21pK1hIeUhVdHlibVAxNEltMXRPYnl0RGtoMTNTVFJ2S2VJWHRtVmNZZ2liRWkvalN0eFoyQUsvVHZvQWYwQkRCNGtNRksxM1RIbEZMbXRMSzYxeHdHRzRXUFpWYmEzVHVsMGhtRmVjYm9KdVJiYWR2cDdkV0RjbUNrODF1S08wb05WRTVtOFF4NlNRS1c2RGM0akoydWNiRk5vbWtlQm1sR3ZlUWhSUTNKUGYvVWhSdXJKQ1hvY2Q4aEQ2YWM1QStIcTVQUmZuRElRanVUL2REYitOTEJTMzB6WGtSM1hrVVc1UEZ0UklpMHo0TnZuNlNLMk1LTXByMEdvU1h6WUFoL0pQNEVQaitSL3ozM1pPNDg2Zy9JenI0WC9PczQvSUpPV1I3ajdQWm1jUGZCUnErNzk5KzBueEI3NHFKRFcycWRQaUdlN1k3eG4wdXFXTHJ6aWl0bGQ5cHJvRnhZMjMxN3hsLzhBQUFELy93TUFVRXNEQkJRQUJnQUlBQUFBSVFBZWtScTM4d0FBQUU0Q0FBQUxBQWdDWDNKbGJITXZMbkpsYkhNZ29nUUNLS0FBQWdBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBakpMYlNnTkJESWJ2QmQ5aHlIMDMyd29pMHRuZVNLRjNJdXNEaEpuc0FYY096S1RhdnIyaklMcFEyMTdtOU9mTFQ5YWJnNXZVTzZjOEJxOWhXZFdnMkp0Z1I5OXJlRzIzaXdkUVdjaGJtb0puRFVmT3NHbHViOVl2UEpHVW9UeU1NYXVpNHJPR1FTUStJbVl6c0tOY2hjaStWTHFRSEVrSlU0K1J6QnYxakt1NnZzZjBWd09hbWFiYVdRMXBaKzlBdGNkWU5sL1dEbDAzR240S1p1L1l5NGtWeUFkaGI5a3VZaXBzU2NaeWpXb3A5U3dhYkREUEpaMlJZcXdLTnVCcG90WDFSUDlmaTQ2RkxBbWhDWW5QODN4MW5BTmFYZzkwMmFKNXg2ODdIeUZaTEJaOWUvdERnN012YUQ0QkFBRC8vd01BVUVzREJCUUFCZ0FJQUFBQUlRRG5Uc3BTZmdFQUFGY0lBQUFjQUFnQmQyOXlaQzlmY21Wc2N5OWtiMk4xYldWdWRDNTRiV3d1Y21Wc2N5Q2lCQUVvb0FBQkFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBS3hXeVU3RE1CQzlJL0VQa2UvRWRRdGxVZE5lRUZLdlVEN0FqU2VMU096SU5rdituaUZ0MnBRR2M1bUxwUm5MODE3bXpaTEY2cXV1b2crd3JqUTZZU0tlc0FoMGFsU3A4NFM5YnA2dTdsamt2TlJLVmtaRHdscHdiTFc4dkZnOFF5VTlQbkpGMmJnSW8yaVhzTUw3NW9Gemx4WlFTeGViQmpUZVpNYlcwcU5wYzk3STlFM213S2VUeVp6YllReTJQSWtaclZYQzdGb2gvcVp0RVBuLzJDYkx5aFFlVGZwZWcvWWpFTHdBcWNCaVJHbHo4Qml6czZjeGttUjhIRi9NS0Fsa1J2dU4zRlp3NUhCd2hWaVFrdmlFN1F0NGp4SzdJNDJCTTBUa2xqSWI0M0tJRUw2WVVoTElqUEhEZXRqWnN4QUJVbngzSmtQdkNWRVFsQ2x3dnEyd3B3OHRzYk5EOEhOSytMTEdhWEJFcjBHVmtuZE9FVGM2LzdNclNYTXdYb2ZCTXJpaFRBSm9wYkVRQnlyMG5wQU9ncFNEeDVFOUVLSXplWGVHK3hFWEJ0MThIdS9INEh5K3BzYi9wY1FQcGM0Vmt1S2Vtc1Q1VUFxclFKcUZ2RExPU2R2Mm14US9icjh2K3h1dTlrdTJUd28vK1IxWWZnTUFBUC8vQXdCUVN3TUVGQUFHQUFnQUFBQWhBUFhBNEdCdENnQUE3VmNBQUJFQUFBQjNiM0prTDJSdlkzVnRaVzUwTG5odGJPd2N5VzdiUnZSZW9QOGc4TzZJa216SkZpSUZTUndqQjZNd212YlFVMEdMdEVXRTVCQWt2ZlhrdUhWamVXbUNaZ05hQjRHN0lBbUt4a1ZjTkVtOTlHTWFVdEtwdjlBM015UmxTcUk5MW1ZNUlnK2lOTVBaM3I1Umw2OHNxa3BzWGpKTUdXazVMbkdKNTJLU1ZrQ2lyTTNtdU04L214Z2E1V0ttSldpaW9DQk55bkZMa3NsZHlYLzgwZVdGcklnS2M2cWtXVEdZUWpPejg5QmJ0Q3c5RzQrYmhhS2tDdVlscEVzYWRNNGdReFVzK0duTXhsWEJ1RDJuRHhXUXFndVdQQzByc3JVVVQvSjhtbk9uUVRsdXp0Q3k3aFJEcWx3d2tJbG1MRHdraTJabTVJTGszcndSQnN1NmRPUzR1Mld5WXR5UUZOZ0Qwc3lpckp2ZWJHcXJzOEVSaTk0azh5Y2RZbDVWdk9jV2RKYlZSRU5ZQUh5b0N0MzJBakpFM1VBRnlUU2hkWngyK2pNbStKUFdkZ0dJcC9CSHNHd2h1S2EzRTFXUU5YOGFUQjExK1BlUmR3bVFGNmRyeC9GVXRZTUFMUEpBUzlOSVhNSjNQYmFRQlZvVVA4MXhQRDk4TFpQS1RIQmUwN2cwSTh3cDFyRWVNbUxLd0RmRHZVMGd6VEpoaEdBV1pEbkgyYTgyM2g4K3dGTklnbWxkTldYaGVGdnhxbWJXSG9xVGpaRFBBbEtRQVlQbUJRVXZoeStPZEpoZmVhMHAwaEozVjRhN1RuZGdpaGFlQjI1OXNUR0FoQ0lMR0NMMExLUDBISll3NnpVbGFJc3NlZzNKOUVnaW5Va2szQ01ycUhEYjY0SlRYUWNBQTlOUFFxc2swcUc2SWhTa0lsSkVpYUFDNU1LVVlGamVHQmR0VTdXSHZrd20wNW1SREovQ3d6SGthbDBZZEphMGFORU9INGkxZFFtdVlXcENKRlBBK2o2WmtKNzJxUURtTHNvYVVKbEhNUFNNMDYzUkJad0d5N1dzcVFPSWNweHVTS1prekV0Y1B0YnNxajc5M2w0dGxiY1BuTktHczcxYmVmV0hmZmdJUXdoSUNsTWErVHdPQzJpRW54U0krUHpuemo0TWpKTWNkdEZPV0JhanY3dU1jK1l0MWJITVRVa0F3cVpVd01JMmFmcG94RGFZSUZzQXZwVXZQMWwxZHRic2UyLytPL2lCVDZkNVBzbkNBNFEvWEhFTXQzWVZRU3M3RDJIMSt0M3JFYXRpS2R1b2ZGdUFlVUM3blpGVms4bUlWYkZKUktIUUF2Q3R2UDM4Z1gxL0MvaTBjdlJqK2RsZVBhVTMwMVVYaEU5ZHpkb0ZTNjRGUUxkRjVhN0pOMkFLaWRWZUM1b2o0ZmFhcy96Y1h2c05LTDM4OEUxRTVneWVWSS9KUEVXOGlZVnNST1pOM1ZWV01yZmYvVms5K2diSVBETml2Nzd6QVJFNmRqUWl1NHZCN2hwbG9wK0FScG9VcGlXbHdVUDZBa0tGUkFvM2hCWlNnNm1TUWtJTEFVSEpCbjByVHlNRXpwTmZuZTFud0syTWpBbzc2RnJncE5NS04zQ2tQakFhODBrK3dZTjQ1TlBPOWhvL0RLQVA3RENTTDI2ZzlUUy9qbzNDR2VUTEp5aEV2QXdQcGluUWVmRmlsN1lxdSt2TzQ5OEhRTUpFL05zLy9CdVpCeVJWMVNRdXd5WThQZlBBdnY5ZHhMKzl6aUFHekRsaVN6ZmtEb1BPV0k5VElHd2t4S0IvdyszN2tVZ0JkNGlCTi9mdHQyc0RvSDM3TjlxSzg5MEdjcjNiQms5MnhGVlZkU2xzNWhTNGx3dXRHMjhXRWE3NG1GSUtOMFhEZGF6VWFYUU5MY0lQa0JjUVYvZVRwbTVDSHNjMmNNbkN5ZjdkTFd0SmtlQXBVZ3dndUd6cTU4cWFKYjRENGl5VnVnNFgxa3V3Q2Jma3dzcFhsMHZPeHN2M2I1ZXJPKy8rWFY2cGQ0dWFKd0ZJM3ZyWVNUcFZ3QkhZTDg5amFSZmNid01PMDY1OHJzTUJNdzRKRkxFUTc2Y3lobWJlZUNNc1Fvc1QrZ2lIUGRCWDZSRnFhZzFZN0xpVERxTzkvaEpDVXBHbUFwaTJXbnBXSjdtdzZVakpzbFowRmpRYzZ3cFZUdFpVNlRFNldaMlVZcFp5a2FieWRPNzVhYXFNVzZ2WEtnNGpUZFZWSFBaQVUyV2k2cksyUGF2MVgreWZjU0tmemNZNTNhYnUxMHF6aStwVGpiWnBqdytxcHNJdVNKVGh4a2J0S1pXRkhaUFRvUm1vMGFpeXNGMHhYZjE3di9wdGFUQ2lYeXljRzNqWEFZaWN4SGw2OGtyTW1mMFNmQncvUElVbGtoK3U2b3JwM0FOMnprVHhnWkJDWVRiZ1E0eVFzTFB6K0oyejkyZ2diSyt1Mm8wc2tSdVcxQk4rczZpeTg2TDY5Q2VLbjRCSnpCSlA3aVBiTnhhcmJPMUNNWHFURjZ2czBrcjE3ajIvUTFVQzUveGdwVzlYU2JCWmdKa2xaQlVhY3ZiUlU3Ny9OU1N1S1VGQ1k0UlZyRXRyT3ZXaVluWDdvSEw0eXQ1OFdsblo3QzVXKzkwb3VxZ0l2TFBwN0wxdy9ub042dHZuVmY5TFFLS0MvV2NnTkhQRHdFZTFsblI0ODNYV0VOUmJGcndXVEMycC9vUkJ2azR6aEp6amhpYmlVN0Rvalg2bnhaNWtZZGxzUklhS2sxQi9leXp5dDl2MXR5dEhxNURES3o5OEFmcDJBQXowaUhuN3B0eHpMUEt1MjJWZStqWkllZjllNVorN0E4dTg0OE9wWWZnYm9XWWxVRjRqaXpvT0NYZUJ3Q0RScTVvaDNoUGRPVUV1YWpReDFnRTBmeHNyNUc5ZWtueHoxVGw1bnYvdlFtclphQVRScjAzclNGVUZHelJQZDFGOXN6ZjB5K1g0L3dBQUFQLy8xRmRkYnR3MkVMNktvQU4wZDdWYWJVeDREU1IxYlFSd2dvVmRvSThCVjZJa29wSW9rTnlWblF1a1FmTFd0N1puYUY4RHRNbHA2dlN4VitnM3BQWW5jYmR4MjhSQWJGZ2FEV2VHdytFM1ArNllQVG9jZEV5N3A4bnNsNnF4b3ZGTWZCNGRkbXo3bWtNTXN2M3JCS0ltNkJnM3FaU3o4UHJuRjcrLy9qNEVRM0JqN3h2SmQzbmwvY1pzaFFaa0oxV1YwaEJmOFdvV0R0MVA2QmJNMHpVM2lvbEQvdms5ZVNVNWJlbFV2dEhTQ3UxVkxDL1c3SkhueUd6TmlKTEpLSmxHdzhRdlZDcjlkcjIwUGZBWnVDTHpFbTNGVTFHcUtoUHV2SmxLNTF6YnRjNnh5UG15c3ZPdDBKTW9TcWFUNlhCTTZuRDJQWDByTHExZndIYitITnQ5WFVBL1VsVGhZU2tiT3d2WEYrQ1A4eC9pYkkvZVBuL3g5cWRmcmwvK2V2M3F1ejkvKzRHTzlSa2o1VUlXalEvR0xZQXljdmZZc1RzR0NpTDhpZkhoa3F0UmM2MVU3dWgza0JFbDlOdER1TSszVFBOT05nWHcyVExaVkxJUlFTYU4vUnI1R2pycXdZWTYyMURuUkRrVkFCKzFKRWd2WitGMFFna2VCdW5WTEJ3bGNUSjJtUTJ6SXM5RmFyL3lraWdFbzRQaEJISkFNWjdhUFJmMEpJZGJScm1vQTVsQkxnd2FYZ3ZVbUIvZi9QSDhXZURUdm1YcDQ5V3A1bTBwMHhPTjlibUdJbWVGWndXWGRkVVloc3BVV3R1eXdjQ2twYWk1K1VLMW9zRmFyblROTFQ1MU1lalBYbGVEYURoTUJqV1hEVTYxTVhYTUxRK1dHalh0WDV0cVpXcVhXc0FhS0lhLzNpMVEvOXRhczVwTGhNaWJSaXo2YUNHV1Bsb1VSOXFWbGtnUW53UDZma2R2VWNuMlJGWVZIWmZvUUROUkx3U2lyaDltRGlLY0dhdUZUVXNTeVNGNmprc2tXenNMenU3V0ZPMWlXbktOczh0YzEvUldlUjRBSFBBT3NIQjN6QmxBOHcrSXdRWnI1VlliZXlwVUhSQUIxK0NCdXlDK09qTzlMMnVSL3BCK2UrY1h2SEhlOXNDZzI5ejlCcjJCUE5Gc2t3bWdjUVE4VUcyMWtkbjVuRUE2akI5TXg5TVQ3TC9wVmZxajlzZS95MDVMdUVGTTBhMW1ZYXVGRVhvRlZBWEJaMTZ0MGRkUk4zQWx3bVU5dTAzTnZ1ZEY3N2htb3lsK3NMbmZMVTV1TjBmdFE4NXRnSU1FYUFubUJ2bUc2dEpuZ2N1QjQzZ2NKelFCM2t5TW5ua0JwZmZTcFJRY2c5YTV5SVVXVFNvZ2FLOWFBRnFzUklNT3dLallvK3hNL1FYdms4NzhWTGFqMENNaVZ3cFQ0Z2ZOSDNqeis2UnZtaC81bHNUMk9aUkxWS1VkZDBiOVhMcHZoeHZ5a2Zlb0xTNW9ITzdROFlaeGhFcUpJUTkwUEkxN0I5cmlFYWRyc0tvbFB0ajQwTElvRWVnRGFya2RXeWhyVmIxZHJVUytYZlFIbUlYM0ptaXBIZlArUWZVZ29zOWlpVngwYlJqVjNjM3NOSC8zTlNlT0p0NUp0T1ZUN1NadWYzYzBLUmhTSjJJdTBTZG00WGprVGtSamprT09LNnNMbFYzNStxclNaVTMvZVB3RkFBRC8vd01BVUVzREJCUUFCZ0FJQUFBQUlRQVl5Zjl0YlFFQUFONERBQUFTQUFBQWQyOXlaQzltYjI5MGJtOTBaWE11ZUcxc3JGTExUc013RUx3ajhRK1I3NjFUUUJSRlRTcWhxbWZFNHdPTTR6UVd0dGV5bllUK1Bac25wVlJWaGJnNDhlenV6S3gzVit0UHJhSmFPQy9CcEdReGowa2tESWRjbWwxSzNsNjNzd2NTK2NCTXpoUVlrWks5OEdTZFhWK3RtcVFBQ0FhQzhCRnlHSi9VR0M1RHNBbWxucGRDTXo4SEt3d0dDM0NhQmJ5NkhkWE1mVlIyeGtGYkZ1UzdWRExzNlUwYzM1T0JCbEpTT1pNTUZETXR1UU1QUldoTEVpZ0t5Y1h3R1N2Y0picDk1UVo0cFlVSm5TSjFRcUVITUw2VTFvOXMrcTlzMkdJNWt0VG5tcWkxR3ZNYWU0bGE3bGlEQTlHcXQ5MkF5NjBETHJ4SGROTUhKOFpGZkU1N2VNQ1dZcXE0eE1KUHpkR0padEpNTk8xNkhNMS9HdDRjaDBkN2JkcFNmVGVDYjVFZExGUFVKR0Z2a2NrTHl4d0w0QWhDTWs5SjNPVlp2T0d5NXM4SXhIZVB5OXZsdGszb29JMG9XS1hDNzhqVEFkU0t1ZmFZQkdpMm9oMkdwKzMreDgwK2FZeURDZEpVM2VLOEhKdGMvTFBKazJMbkRHTVBvM3VmZlFFQUFQLy9Bd0JRU3dNRUZBQUdBQWdBQUFBaEFMQUc3aTlGQVFBQTVRSUFBQkFBQUFCM2IzSmtMMlp2YjNSbGNqTXVlRzFzbkpMTGJvTXdFRVgzbGZvUHlIdGkwbFpKaFFLUktwUjExY2NIdU1ZRXE3YkhzZzAwZjkvaDJjY2lpckpoaE1mMzNCblA3UFpmV2tXdGNGNkN5Y2g2bFpCSUdBNmxOTWVNdkw4ZDRrY1MrY0JNeVJRWWtaR1Q4R1NmMzk3c3VyUUtMa0sxOFdtTGlUb0VtMUxxZVMwMDh5dXd3bUN5QXFkWndGOTNwSnE1ejhiR0hMUmxRWDVJSmNPSjNpWEpoa3dZeUVqalREb2hZaTI1QXc5VjZDVXBWSlhrWWdxendsM2lPeW9MNEkwV0pneU8xQW1GTllEeHRiUitwdWxyYWRoaVBVUGFjMDIwV3MzM09udUpXK2xZaDZQUWFpeTdBMWRhQjF4NGo2ZkZtRnlJNitTYzkvU0FQV0pSWEZMQ1g4KzVFczJrV1REOVl2eWIveks4RlE2UGp0NjBSLzAwZ20rUjR4clpxRXR4L2NxWGpDVEp3OVAyZm5zZzgxRWhLdGFvOENzektKN2RFRjdEU1FtODJqS1ZFYlloTk45UjVQWFpQZzVmWE5MOEd3QUEvLzhEQUZCTEF3UVVBQVlBQ0FBQUFDRUFBbnlzaWtVQkFBRGxBZ0FBRUFBQUFIZHZjbVF2YUdWaFpHVnlNeTU0Yld5Y2tzdHVnekFRUmZlVitnL0llMkxTVnlvVWlGU2hyS3MrUHNBMUpsaTFQWlp0b1BuN0RzKzJXVVJSTjR6dytKNDc0NW50N2t1cnFCWE9TekFaV2E4U0VnbkRvWlRta0pIM3QzMzhTQ0lmbUNtWkFpTXljaFNlN1BMcnEyMlgxcVdMVUcxODJtS2lEc0dtbEhwZUM4MzhDcXd3bUt6QWFSYncxeDJvWnU2enNURUhiVm1RSDFMSmNLUTNTZkpBSmd4a3BIRW1uUkN4bHR5Qmh5cjBraFNxU25JeGhWbmhMdkVkbFFYd1Jnc1RCa2ZxaE1JYXdQaGFXai9UOUg5cDJHSTlROXB6VGJSYXpmYzZlNGxiNlZpSG85QnFMTHNEVjFvSFhIaVBwOFdZWElqcjVKejM5SUE5WWxGY1VzSmZ6N2tTemFSWk1QMWluTXgvR2Q0S2gwZEhiOXFqZmhyQnQ4aHhqV3pVcGJoKzVVdEdrdVR1YVhPNzJaUDVxQkFWYTFUNGxSa1V6MjRJcitHb0JGNXRtY29JdXljMDMxTGs5ZGsrRGw5YzB2d2JBQUQvL3dNQVVFc0RCQlFBQmdBSUFBQUFJUUN3QnU0dlJRRUFBT1VDQUFBUUFBQUFkMjl5WkM5bWIyOTBaWEl5TG5odGJKeVN5MjZETUJCRjk1WDZEOGg3WXRKV1NZVUNrU3FVZGRYSEI3akdCS3UyeDdJTk5IL2Y0ZG5ISW9xeVlZVEg5OXdaeit6MlgxcEZyWEJlZ3NuSWVwV1FTQmdPcFRUSGpMeS9IZUpIRXZuQVRNa1VHSkdSay9Ca245L2U3THEwQ2k1Q3RmRnBpNGs2Qkp0UzZua3ROUE1yc01KZ3NnS25XY0JmZDZTYXVjL0d4aHkwWlVGK1NDWERpZDRseVlaTUdNaEk0MHc2SVdJdHVRTVBWZWdsS1ZTVjVHSUtzOEpkNGpzcUMrQ05GaVlNanRRSmhUV0E4YlcwZnFicGEybllZajFEMm5OTnRGck45enA3aVZ2cFdJZWowR29zdXdOWFdnZGNlSStueFpoY2lPdmtuUGYwZ0QxaVVWeFN3bC9QdVJMTnBGa3cvV0w4bS84eXZCVU9qNDdldEVmOU5JSnZrZU1hMmFoTGNmM0tsNHdreWNQVDluNTdJUE5SSVNyV3FQQXJNeWllM1JCZXcwa0p2Tm95bFJHMklUVGZVZVQxMlQ0T1gxelMvQnNBQVAvL0F3QlFTd01FRkFBR0FBZ0FBQUFoQUxBRzdpOUZBUUFBNVFJQUFCQUFBQUIzYjNKa0wyWnZiM1JsY2pFdWVHMXNuSkxMYm9Nd0VFWDNsZm9QeUh0aTBsWkpoUUtSS3BSMTFjY0h1TVlFcTdiSHNnMDBmOS9oMmNjaWlySmhoTWYzM0JuUDdQWmZXa1d0Y0Y2Q3ljaDZsWkJJR0E2bE5NZU12TDhkNGtjUytjQk15UlFZa1pHVDhHU2YzOTdzdXJRS0xrSzE4V21MaVRvRW0xTHFlUzAwOHl1d3dtQ3lBcWRad0Y5M3BKcTV6OGJHSExSbFFYNUlKY09KM2lYSmhrd1l5RWpqVERvaFlpMjVBdzlWNkNVcFZKWGtZZ3F6d2wzaU95b0w0STBXSmd5TzFBbUZOWUR4dGJSK3B1bHJhZGhpUFVQYWMwMjBXczMzT251SlcrbFloNlBRYWl5N0ExZGFCMXg0ajZmRm1GeUk2K1NjOS9TQVBXSlJYRkxDWDgrNUVzMmtXVEQ5WXZ5Yi96SzhGUTZQanQ2MFIvMDBnbStSNHhyWnFFdHgvY3FYakNUSnc5UDJmbnNnODFFaEt0YW84Q3N6S0o3ZEVGN0RTUW04MmpLVkViWWhOTjlSNVBYWlBnNWZYTkw4R3dBQS8vOERBRkJMQXdRVUFBWUFDQUFBQUNFQTFTT0lHcFVDQUFCUENRQUFFQUFBQUhkdmNtUXZhR1ZoWkdWeU1pNTRiV3kwVnMrTDAwQVV2Z3YrRDJYdWFkSjIyMHJZWkZHN3hXTngxN09NeWJRSkpqUER6TFMxdDRvS3k0S29LQWg2a0o1NkVEeW9zQ3F5L2pObXU1NzhGM3lUSDkxdGhhVzA5Sktaekh2dis3NTU3ODBrdTN1UDRxZzBJRUtHakRxb1VyWlFpVkNQK1NIdE9lamVZZHU0Z1VwU1llcmppRkhpb0JHUmFNKzlmbTEzYUFlK0tFRTBsZllBRElGUzNEWk42UVVreHJMTU9LRmc3RElSWXdXdm9tZkdXRHpzYzhOak1jY3FmQkJHb1JxWlZjdHFvQnlHT2FndnFKMURHSEhvQ1NaWlYra1FtM1c3b1VmeW9ZZ1FxL0Jta1MzbTlXTkNWY3BvQ2hLQkJrWmxFSEpab01Ycm9zRVdnd0prY05VbUJuRlUrQTM1S215K3dFTW9SUnhsc29kTStGd3dqMGdKcTYzTU9FZXNXRmR4NXduVUVQT0lWU1FzY2haS1loelNPWXh1aktYNno0dFhodUtaR2JlcG9TNDJBcmx3b1kya3IvS2hJL1JFNFY1cGFBOXc1S0E3QlB0RUhPSWVNclVsOUF0RHRWR3ZOSnJWblhwbTRCSDJTTUFpY05aK1B2TTZXS2pDdVVXNnVCK3B6b1hUL1dxMTBhdzNyWm9PTjRmMlFqeThnNlJNQzB4dU02cWdiVFF1QjBRNEtQNWRCMW5XenExbXJkbEd4VkpPY3NtU1JtUXcvRUNOSWxMb3dhbHFUWnRaVTgwaW43ZUJUb0puRUZMbElJS2x1aWxEbk92TW5aU2JURjhuTDUvLy9mbE9xd2R0OEV4Um9EZFlkMThJQUZBakRsWHBDUndmS0VoR2xxZzFxTTVQMzg4K2ZGbUZaNS82NjdMb0M4Q1dIS3JvSUM2SUpHSkFrRnRhWWwxRC9kbDRtaHg5aEVUTjNweHNqTFkxbGNuM3IzOU9uLzVmempVMjNLeHZ2RTAzK2Z4NFk1RHQ1ZXJIY2ZMaUJIS1YvSHB5UGhuUFBrMld0RzdoRFB6K05sNkZaSU1ENE02bXI1WW8xaWkrWVJoV3BWYlpIQWd5dkRISTFqcGc5dmJaMmVRb2E0SXRxYlFhRGN1cUx0NC8rc0pPYjlyTEh3VllUTDlmTU1MdmtQc1BBQUQvL3dNQVVFc0RCQlFBQmdBSUFBQUFJUUJNSi9WVWJRRUFBTmdEQUFBUkFBQUFkMjl5WkM5bGJtUnViM1JsY3k1NGJXeXNVOHRPd3pBUXZDUHhENUh2clZOQUZFVk5LcUdxWjhUakE0enROQmF4MTdLZGhQNDltMmVoVkZXRnVEanhySGRtMXJ0ZXJUOTFHZFhTZVFVbUpZdDVUQ0pwT0FobGRpbDVlOTNPSGtqa0F6T0NsV0JrU3ZiU2szVjJmYlZxRW1tRWdTQjloQlRHSnpWR2l4QnNRcW5uaGRUTXo4RktnOEVjbkdZQnQyNUhOWE1mbFoxeDBKWUY5YTVLRmZiMEpvN3Z5VUFES2FtY1NRYUttVmJjZ1ljOHRDa0o1TG5pY3ZpTUdlNFMzVDV6QTd6UzBvUk9rVHBab2djd3ZsRFdqMno2cjJ4WVlqR1MxT2VLcUhVNW5tdnNKV3JDc1FiN29jdmVkZ05PV0FkY2VvL29wZzlPakl2NG5QWndnUzNGbEhHSmhaK2FveFBObEpsbzJ1azQ2di9VdkRrMmovYmF0S1U2RklKM2tSMW1LV3FTc0xkSTVLVmxqZ1Z3QkNFbFVoSjN4eXp1Y0ZURk13THgzZVB5ZHJsdEQzVFFSdWFzS3NQdnlOTTNxTlZ5N1RJSjBHeEZPd3hYMi8wUGMzM0tGZ2NUbEttNnFYazV0cmo0WjRzbnhjN1l4UUxHSjVsOUFRQUEvLzhEQUZCTEF3UVVBQVlBQ0FBQUFDRUFBbnlzaWtVQkFBRGxBZ0FBRUFBQUFIZHZjbVF2YUdWaFpHVnlNUzU0Yld5Y2tzdHVnekFRUmZlVitnL0llMkxTVnlvVWlGU2hyS3MrUHNBMUpsaTFQWlp0b1BuN0RzKzJXVVJSTjR6dytKNDc0NW50N2t1cnFCWE9TekFaV2E4U0VnbkRvWlRta0pIM3QzMzhTQ0lmbUNtWkFpTXljaFNlN1BMcnEyMlgxcVdMVUcxODJtS2lEc0dtbEhwZUM4MzhDcXd3bUt6QWFSYncxeDJvWnU2enNURUhiVm1RSDFMSmNLUTNTZkpBSmd4a3BIRW1uUkN4bHR5Qmh5cjBraFNxU25JeGhWbmhMdkVkbFFYd1Jnc1RCa2ZxaE1JYXdQaGFXai9UOUg5cDJHSTlROXB6VGJSYXpmYzZlNGxiNlZpSG85QnFMTHNEVjFvSFhIaVBwOFdZWElqcjVKejM5SUE5WWxGY1VzSmZ6N2tTemFSWk1QMWluTXgvR2Q0S2gwZEhiOXFqZmhyQnQ4aHhqV3pVcGJoKzVVdEdrdVR1YVhPNzJaUDVxQkFWYTFUNGxSa1V6MjRJcitHb0JGNXRtY29JdXljMDMxTGs5ZGsrRGw5YzB2d2JBQUQvL3dNQVVFc0RCQW9BQUFBQUFBQUFJUUJwZWRTaXdRVUFBTUVGQUFBVkFBQUFkMjl5WkM5dFpXUnBZUzlwYldGblpURXVjRzVuaVZCT1J3MEtHZ29BQUFBTlNVaEVVZ0FBQUhzQUFBQWJDQUlBQUFEVFVXZHlBQUFBQVhOU1IwSUFyczRjNlFBQUFBUm5RVTFCQUFDeGp3djhZUVVBQUFBZ1kwaFNUUUFBZWlZQUFJQ0VBQUQ2QUFBQWdPZ0FBSFV3QUFEcVlBQUFPcGdBQUJkd25McFJQQUFBQUFsd1NGbHpBQUFoMVFBQUlkVUJCSnkwblFBQUJTcEpSRUZVYUVQbG1XdFJKRUVRaERrRjRBQUo0QUFKS3dFSlNFQUNFcENBQkhDQWhKV0FCTzdieUl2YzJ1cWVtWnJlQ1FpNCtVRU1UWGM5c3JJZVBmejUvUHk4K1ArZXQ3YzNuTDY5dmIyNnV2cHE3MEc4ZlY1ZlgrL3U3dDdmMzd0Ly9hR0wrLzMrNmVucDV1YkdFRjllWHQ3ZjMzOThmR3pyVVZSMGZYMzk4UEFRNVYrMHlyQ0FmWmdGN3R1YThpM1M4T0x4OGJFRmVyZmJDWHBBMzhRd0NBcTRVWkZERzduYlFSejd0UFYzY0J4ZXcyVTd6Ni9HbDNldHc4b3pRUWZycWVyMC9QeTh3SEhaUjZ4bTBnMFQyWmJ5cFdnMFluRVZpa1huMDFtbi96QUJzZEQyOHdLTk1KaFNHUld4THBobUxDazZCVHZ4Q1BrOHZKanBDZTVEMTB3U1gxNWVIUFo1Yi9rck8xdUpVeWJpSHNKMXlnOHJjVDhocEtDbENsRDBPVzBEWWtURk5KWHF4QkxwQXFNeExkMVRlQ3JXZHNWbXhKVWQySUVzZm5KbUt1TUk1aXAyYUg5NjNDclFnanBqclliRDVzVEtPaTZpVHZSWk5RUXNvaEJYODdya3haMHV5MTNvVGhDSEVlcVo4Ukg2aVJwT2hYcDNaV2VTN1B4QVdpeTEyS0E2MEZKeTBWdHZzRHF0NEpyVEs5bzhqMDVkblhlYTRGTVY0b0E0Y1VaeDRpQVFhSjFIK0FLNnBIaVlXWnVNeUxHSE5namNVeVJVVldWUEtqdDFDS3dJSXhPTmNNM3M4N1k2ZGVadGlDSFVtS2ppN3ZwMlFEeFJHL3VTbitxVCtDK1lUSmFCRnE5YWdRclpUZkRJSWMzRm5oeGtuQkFmSHBaVHcwaEJ0ZVdPZDcwaHpTQnVnc3ZIcU5Uc1BGWVZGNHF1YmpVaWlPQnRyTlFacDUzT2RCRkt0VnVacE9rbEJrTmtHYTdqQ2hpeHhHRFVFVVVuY1pScGt3YmNhZDAzd1hraDVIRmtkRTRmRWNlbXRyRWtvYTRuWTBBbzdJbzI2b0NEUnlVYnoyTWFTYTk0T2phNmlWKzIwMm5LWWl3Z0d5SnVnanVEemM0NGFoOFJWM3htU3JNcWdLSXlrT3pPMzRpdjhva1YweUVWdERHYWl6MDhabTZjZ2lLTk5rVGNCSmRURGdCdzllK2NvbGlYVUJwc2xmWHBmTDJ3dEIzU0JNUldJNUppT1lhNHlhWDR4ZXhXYlRRRVd5RWU4UlVtTG1KNDEwZGNLWXhCTGVoeGREdW53Nmh4dTUvSUR2TGRYUTdjbzNFQ2JxQ0NtVzVJYXlNdEE1VE52bllPYUlsc2l4V2M5VllwR0FxNmszbmNvRXVXb1RkTUVXNXNIWmhWYkJrK0k2MExoMFlYNEpiZWdaNFc3elhkbTFjTVpLcjQ5YXoxemtodzVhaVR6UFRTeXlIQTNhTEdvcTRNNHFBNUxsTEViNTZyNEhCNU5jdklkNlNaNHpHWmJPdEFYSVd5YUd2MEpSQmRpclFkUHgveFJIQkp4bXhLbHA1WUtrOFFWNzFUWTFUdGRyZVVXWVN1dlpUVzY0eUtOUklFc1VZM0RWSmFGenNTSy9sMWJhTk9kU04rUG13cGNpYmlMY0hucytRRWNhR3A3eHNpaUxCSTMzeDFSZktGcFlpNGljQlpKNTNNbFY1ajRVblJORS9mcEJZVFh3Y2p1SEZTU3NmUFJMeEw4QmtMOHp4dUozRmIxT0JuckVyNjZJRkVNZEd6NXp3S0hnbTBQMzcwY0VuaEJldmpQU1hPR0VxSVJheTFvVVhjSFhKYmpxOGwrTUUyKzJET3VnS3lJbkJ0cmdvTEs2N0lsZXRKYkkvYWIxNm9qQ2lOd0pwSDM4MWR1K05kMGRvWGNlKzIzS254OXh5T215NFZIUDZ4d2RiSER0TTJLekRDWWs5VXNyNzRKY3VNVnUwR0RoM1hWMGtlekkxRFlRdW9DekVIRitGMlJCT2RWYXphbGpDTU9DanBiREhSTStLZUV4YkhEd1YyVlVPRHlLa1pJR0hWdjVCQVNucUx0UVYxclNOSTBGeHMzRjB3aSt4eHZQVlJTSWdYVGVwelBINEJtR0lUSVYyTVN2ZXM3aU9jeGNSNVVrK3B4cy9pc0lqOCtQMUVBdVBvcFRybTBXdXRSKzR4YTY5T3h6cU9mUnl1QUxGMlZxdlVnUy9iZy9GcFFoZFAyL0RNbStRT3YvWmc1My81WCtiOE55b2kxZUxGd2gybGJwTEN0cW93NXFwU1YvWTdka0oyNkVreEtkNG5rdGM2UGdERlgyTWZvVUZyTWRHUkFBQUFBRWxGVGtTdVFtQ0NVRXNEQkJRQUJnQUlBQUFBSVFESEhHMFVuQVlBQUZFYkFBQVZBQUFBZDI5eVpDOTBhR1Z0WlM5MGFHVnRaVEV1ZUcxczdGbE5ieHRGR0w0ajhSOUdlMjlqSjNZYVIzV3EyTEViYU5OR3NWdlU0M2c5M3AxNmRtYzFNMDdxRzJxUFNFaUlnbnFnRXVMQ0FRR1ZXZ2treXE5SktTcEY2bC9nblpuZDlVNjhKa2tiUVFYMUlmSE9QdS8zeDd3enZuanBUc1RRUGhHUzhyanBWYzlYUEVSaW53OXBIRFM5Ry8zdXVUVVBTWVhqSVdZOEprMXZTcVIzYWVQOTl5N2lkUldTaUNDZ2orVTZibnFoVXNuNjBwTDBZUm5MOHp3aE1id2JjUkZoQlk4aVdCb0tmQUI4STdhMFhLbXNMa1dZeGg2S2NRUnNyNDlHMUNmbzJjKy92UGptZ2JlUmNlOHdFQkVycVJkOEpucWFOM0ZJREhZNHJtcUVuTW8yRTJnZnM2WUhnb2I4b0UvdUtBOHhMQlc4YUhvVjgvR1dOaTR1NGZXVWlLa0Z0QVc2cnZta2RDbkJjTHhzWklwZ2tBdXRkbXVOQzFzNWZ3TmdhaDdYNlhUYW5Xck96d0N3NzRPbFZwY2l6MXAzcmRyS2VCWkE5dXM4NzNhbFhxbTUrQUwvbFRtZEc2MVdxOTVJZGJGTURjaCtyYzNoMXlxcnRjMWxCMjlBRmwrZnc5ZGFtKzMycW9NM0lJdGZuY04zTHpSV2F5N2VnRUpHNC9FY1dnZTAyMDI1NTVBUlo5dWw4RFdBcjFWUytBd0YyWkJubHhZeDRyRmFsR3NSdnMxRkZ3QWF5TENpTVZMVGhJeXdEMm5jeHRGQVVLd0Y0SFdDQzIvc2tpL25sclFzSkgxQkU5WDBQa3d3bE1TTTM2dW4zNzk2K2hnZDNuMXllUGVudzN2M0R1LythQms1Vk5zNERvcFVMNy85N00rSEg2TS9Ibi85OHY0WDVYaFp4UC8yd3lmUGZ2MjhIQWpsTTFQbitaZVBmbi95NlBtRFQxOThkNzhFdmlud29BanYwNGhJZEkwY29EMGVnV0hHSzY3bVpDQk9SOUVQTVMxU2JNYUJ4REhXVWtyNGQxVG9vSzlOTVV1ajQralJJcTRIYndwb0gyWEF5NVBianNLOVVFd1VMWkY4Sll3YzRBN25yTVZGcVJldWFGa0ZOL2NuY1ZBdVhFeUt1RDJNOTh0a3QzSHN4TGN6U2FCdlptbnBHTjRPaWFQbUxzT3h3Z0dKaVVMNkhSOFRVbUxkTFVvZHYrNVFYM0RKUndyZG9xaUZhYWxMK25UZ1pOT01hSnRHRUpkcG1jMFFiOGMzT3pkUmk3TXlxN2ZJdm91RXFzQ3NSUGsrWVk0YkwrT0p3bEVaeXo2T1dOSGhWN0VLeTVUc1RZVmZ4SFdrZ2tnSGhISFVHUklweTJpdUM3QzNFUFFyR0RwV2FkaDMyRFJ5a1VMUmNSblBxNWp6SW5LTGo5c2hqcEl5YkkvR1lSSDdnUnhEaW1LMHkxVVpmSWU3RmFLZklRNDRYaGp1bTVRNDRUNitHOXlnZ2FQU0xFSDBtNG5Rc1lSVzdYVGdpTVovMTQ0WmhYNXNjK0RzMmpFMHdPZGZQU3pKckxlMUVXL0NubFJXQ2R0SDJ1OGkzTkdtMitaaVNOLytucnVGSi9FdWdUU2YzM2pldGR4M0xkZjd6N2ZjUmZWODBrWTc2NjNRZHZYY1lJZGlNeUpIQ3lma0VXV3NwNmFNWEpWbVNKYXdUd3k3c0tqcHpQR1E1Q2VtSklTdmFWOTNjSUhBaGdZSnJqNmlLdXlGT0lFQnUrcHBKb0ZNV1FjU0pWekN3YzRzbC9MV2VCalNsVDBXMXZXQndmWURpZFVPSDlybEZiMmNuUXR5Tm1hM0NjemhNeE8wb2htY1ZOaktoWlFwbVAwNndxcGFxUk5McXhyVlRLdHpwT1VtUXd6blRZUEYzSnN3Z0NBWVc4RExxM0JBMTZMaFlJSVpHV3EvMjcwM0M0dUp3bG1HU0laNFNOSVlhYnZuWTFRMVFjcHl4ZHdFUU82VXhFZ2Y4bzd4V2tGYVE3TjlBMmtuQ1ZKUlhHMkJ1Q3g2YnhLbExJTm5VZEoxZTZRY1dWd3NUaGFqZzZiWHFDL1hQZVRqcE9tTjRFd0xYNk1Fb2k3MXpJZFpBRGREdmhJMjdZOHRabFBsczJnMk1zUGNJcWpDTllYMSs1ekJUaDlJaEZSYldJWTJOY3lyTkFWWXJDVlovWmZyNE5hek1zQm0rbXRvc2JJR3lmQ3ZhUUYrZEVOTFJpUGlxMkt3Q3l2YWQvWXhiYVY4b29qb2hjTUROR0FUc1ljaC9EcFZ3WjRobFhBMVlUcUNmb0I3Tk8xdDg4cHR6bW5SRlcrdkRNNnVZNWFFT0cyM3VrU3pTclp3VThlNUR1YXBvQjdZVnFxN01lNzBwcGlTUHlOVGltbjhQek5GN3lkd1U3QXkxQkh3NFI1WFlLVHJ0ZWx4b1VJT1hTZ0pxZDhWTURpWTNnSFpBbmV4OEJxU0NtNlR6WDlCOXZWL1czT1doeWxyT1BDcFBSb2dRV0UvVXFFZ1pCZmFrc20rWTVoVjA3M0xzbVFwSTVOUkJYVmxZdFVla0gzQytyb0hydXE5M1VNaHBMcnBKbWtiTUxpaitlYytweFUwQ1BTUVU2dzNwNGZrZTYrdGdYOTY4ckhGREVhNWZkZ01OSm4vY3hWTGRsVkxiOGl6dmJkb2lINHhHN05xV1ZXQXNNSlcwRWpML2pWVk9PVldhenZXbk1YTDlVdzVpT0s4eGJDWUQwUUozUGNnL1FmMlB5cDhSa3dhNncyMXovZWd0eUw0b1VFemc3U0JyRDVuQncra0c2UmRITURnWkJkdE1tbFcxclhwNktTOWxtM1daenpwNW5LUE9GdHJkcEo0bjlMWitYRG1pbk5xOFN5ZG5Yclk4YlZkVytocWlPelJFb1dsVVhhUU1ZRXh2MmtWZjNYaWc5c1E2QzI0MzU4d0pVMHl3VzlLQXNQbzJUTjFBTVZ2SlJyU2piOEFBQUQvL3dNQVVFc0RCQlFBQmdBSUFBQUFJUUNEMExYbDZnQUFBSzBDQUFBbEFBQUFkMjl5WkM5bmJHOXpjMkZ5ZVM5ZmNtVnNjeTlrYjJOMWJXVnVkQzU0Yld3dWNtVnNjNnlTeTA3RE1CQkY5MGo4Z3pWNzRyUWdoRkNkYmhCU3R4QSt3SFVtRDlVWlc1N2hrYi9IUW1wcFJWVTJXYzYxZk83eFk3WCtHcjM2d01SRElBT0xvZ1NGNUVJelVHZmdyWDYrZVFERllxbXhQaEFhbUpCaFhWMWZyVjdRVzhtYnVCOGlxMHdoTnRDTHhFZXQyZlU0V2k1Q1JNb3JiVWlqbFR5bVRrZnJkclpEdlN6TGU1Mk9HVkNkTU5XbU1aQTJ6UzJvZW9xNStYOTJhTnZCNFZOdzd5T1NuS25RbjdoOVJaRjhPTTVZbXpvVUEwZGhrVzFCbnhkWnppbkNmeXoyeVNXRnhhd0tNdm44bUlkcjRKLzVVdjNkblBWdElLbnQxdU92d1NIYVMraVRUMVo5QXdBQS8vOERBRkJMQXdRVUFBWUFDQUFBQUNFQWNqSzJ3cThEQUFCckNBQUFFUUFBQUhkdmNtUXZjMlYwZEdsdVozTXVlRzFzbkZiYmJ0czRFSDB2MEg4dzlGekhsempwUXFoVE5ISGNkT0YwaXlyWmZSNkpJNHNOTHdKSjJYRytmb2NpV2NYdHRnajJ5ZVNaTTRmRG1lSEk3OTQvU2pIYW9iRmNxMlUyTzVsbUkxU1ZabHh0bDluOTNYcjhSemF5RGhRRG9SVXVzd1BhN1AzRjYxZnY5cmxGNTRobVJ5U2hiSzZYV1dkVWJxc0dKZGl4NUpYUlZ0ZHVYR21aNjdybUZjYWZMSHFZWmRZNDErYVRTWFE2MFMwcVVxdTFrZURzaVRiYlNmQmM2YXFUcU54a1BwMmVUd3dLY0JTd2JYaHJrNXI4djJwMFZKTkVkcis3eEU2S3hOdlBwcjlqeHV2dXRXSGZQVjRTbm5kb2phN1FXc3FzRk9HNkVyaEtNbGE4UkNma2M4TkxBK2J3VE9TQ3l2YWt0Unp0OHhaTlJRbWxtaytuMmNRYlNqcWNHbUdsUDJ0WGRNYm9UckViQk1KK2FWNXI3YUtad3RaMTRjQWhpVzhOU0FsVTMwb2dxQ0RQc0laT3VEc29DNmRiSXUyQTdyS1l4OU9aZ1QzZCthUGg3RVliL3FTVkExRzBVQkdZeUxQcFdkUWF5SCtqY2J6Nm1YcDJIcW5jdGdJT2crWnE4TDJteGo4azhSUkg0Q2ZaWDdIblFiMXF3RUJGT1lpUlhsSFlSb3VrU2EzZkdpcm1sMDVWcnV0N052alZsRGhGdWZ0aWZPYlRqdHc0VzJZeGtoL1FtZmVjRE9UZ2lvb05PbkZ6TEhNTUpwVWpQeDhtT0IrSnBZVGpXcHY3VGFnNUNGQVZGbFFEZ1pjSGh5dmRsV0gxRDJldTZVbk1OOHdHWVllWFVEMVlBYmI1NElkRmIrekVuUUhlRnpJQVBmdjZzYVdSVWpTOGRsL1IwZGpvdWNDK2RkWnR1TUliNU52R2ZWTFVMQ0xxV0Z4ZmIrQ2dPMGRjU3NNUU04MHVabjN3ZnZHVkVwdXlQNTB1THQrZXZsMkhsSHZyaXl5cnhlbWlieDQ2SldyTDNBOEpuL0d3V2xPWlJ6SjA4QlhJMG5BWTNmb3hRckhKdkRRUGwxd2xlNGswenZDNXBlaktaQnlQZzhGS0VHSk5yWlFNTkVHQ2hWRS9yckR1aGNVdG1PMmczUGVKek0xL292VGMvdnl1NWw4Nm1vLzBwTnVndWpmUWZsS000SFRnYkxHSWVseFJEV1RDYlZjV3lVdlJOSGxtNmhUN2EyZTg0R1JJMEQ1MzlBR2dIaUlWR0o0dXF2RjlrVkgrRWF6N1lEa3NzNmRtZlBYWmUxTXhoU244ZHdOdm9XM0RneSszczJVbWZCdk12SnVqSFFQejBHL0s3VHphNXIyTmR0N1diNkR5bHlWMlhIaENXQklyTGdic05HR25BN1pJMkdMQXpoSjJObURuQ1R2M1dIT2dpU3E0ZXFENW5KWWVyN1VRZW8vc0pvSEw3Q2NvSk1FMjBDS1YyazlKNm1lZDkwQWNtM2EweS9HUnhqVXk3dWlUM0hJbTRkRlA3M25mcTVGTms0NWV5QkhYSzNseWU0U09HRGlxUVJnSFI4Nzk2L29obG4zT3NPTFVvOFZCbHNONE93bUJDMjVkZ1MxTlFxY05YYmtmN0cvNnZoaitKVno4Q3dBQS8vOERBRkJMQXdRVUFBWUFDQUFBQUNFQXovT0NjWlVDQUFBSEJ3QUFHZ0FBQUhkdmNtUXZaMnh2YzNOaGNua3ZaRzlqZFcxbGJuUXVlRzFzMUZYTGF0dEFGTjBYK2c5bTlvcGt5YkljRXpuRWxrV1hwbzkxR1V0alMzUTBJMmJHZGtVcGhKSVNTdW1xdEYwWFVrb1gyYWNVK2pPdFEvSVhIVWt6ZmkxQzBsMjhzTytkZTgrNWIzeHcrRExEalRsaVBLWEVCODA5Q3pRUWlXaWNrcWtQbmowTmpRNW9jQUZKRERFbHlBY0Y0dUN3OS9EQndhSTd4WlJ6eUlxQVJyTU1FZEdRVklSMzU5SXJFU0x2bWlhUEVwUkJ2a2R6UktSeFFsa0doVlRaMU13Z2V6SExqWWhtT1JUcE9NV3BLRXpic3RwQTBWQWZ6QmpwS2dvalN5TkdPWjJJRXRLbGswa2FJZldqRWV3MmNXdWtUcm1LYURLRVpRNlU4Q1ROdVdiTC9wZE5scGhva3ZsTlJjd3pyUDBXK1cyaXhRd3U1Rnd5WEtlOW9Dek9HWTBRNS9JMXFJMHJ4cVoxVTJ6VndKSmloYmhOQ3RzeGRTWVpUTW1LcHR5U25mbXZocmNuaDJmV3NjMlNhbDJJN0VWUDdsUk1veEZrZ20vSUcrS0lsUXFCR1dvc3VuT0lmUkNnQ1p4aE1jSXdRZ25GTVdMUGJidnR1WjdsQUxOMGpxQkFVOHFLWGVEeTR1THErMG50TTRVWUkxWm96bnhOVnRyTmJSSlI1S2hLcnhRMFpEd2U0ZWhSekpUL3ltZU1FamhQS2FzQVd0R2dpQkloejBaaHRMVnluYzdTV0x1OWNxM2hmdUNGcnVFTStwN1JhanJ5S0IzYk00NmNzRFhzRDF4bjJBNWZLeGJWdjdwUlN1blR1Q28vbDR6eXp1UEhQckRVQitnbjFjZktFbGl1RjFiVFlObzhrcmRsV1hiWWRteXJ0dFFSMkJOUjRGVVhvT281QzJWbFhHS1RsQWdmSU1qRkVVK2hTcEhWVU5GYmZ2aTBQUDExZVg2MlBEdTUrdjF4K2ZiYjVlZlQ1Zm1YUDhkdnlxYUxxdlZ5NGxMT3ErK3RldVNyMGpjV1pFTzh5NjYweXRTMng3eTVaUGRvVnpyN29UZllEenBHYTlDMGpWYlFIQnA5OThnekFydGxCUU4zNklTMmN4OTM1ZnI0M2VYN0gzOHZqcSsvL3J6N2ZxeDNSVjZYVkhiL3VYci9BQUFBLy84REFGQkxBd1FVQUFZQUNBQUFBQ0VBd25IOGVSOERBQURYQmdBQUdnQUFBSGR2Y21RdloyeHZjM05oY25rdmMyVjBkR2x1WjNNdWVHMXNuRlh2YjlNd0VQMk94UDlRNVROZDA5SU5GSzFEYkYwWnFBTkVOdmg4Y2E2Tm1XTkhaNmVoL1BXYzQ1aE00b2NRbitxOGQvZnNlNzV6ejE5OXE5WGtnR1NsMGF0a2ZwSW1FOVRDbEZMdlY4bjkzV2I2TXBsWUI3b0VaVFN1a2lQYTVOWEYweWZuWFdiUk9RNnpFNWJRTmpPcnBDV2RXVkZoRFhaYVMwSEdtcDJiQ2xOblpyZVRBb2VmWk1pZ1ZWSTUxMlN6MlpCMFloclVyTFl6VklPeko0YjJzNUM1TnFLdFVidlpJazNQWm9RS0hCL1lWckt4VWEzK1h6WGVxb29paDc4VmNhaFZqT3ZtNmQ4aWgzSTdRK1hQakg4NW5rOW95QWkwbHAydFZTaTNCcW1qakZYL29oUDgzTXFDZ0k2UFJDNzQyZzRTdXduL0FDdHBiN1JLWmg0dmVHL3VnN1Y1YjF6ZUVwbFdsemNJalAyUjNoampCcnJFSGJUSzNVR1JPOU5FL2VVaURlSWxRY2NWdlNGWmZrWnlVb0RLR3hBTXhkRDU2ZGtRS20yajRIaGpTSDQzMm9GYWo3blgzS25IbUJHbFEzeVUvVlAwSXFpTENnZ0VuM3JZL29xM0lLT2lKdmRxUSt6K3gxWUwxL1pOTnVReEFjNzdaUG5jdURGMHZ3MitnQUl0TU9kU0ZGNGVIYTVOVzRUVkYxbTZxZzhxdmFsYmhBTmVnbml3Q216MTJzOVRUN2JxamtEMmZnU2dqNzcrMXZEVTVaWGN1VS9vZUxMNldDaS90dFp0cGNZYmxQdkt2ZFhzdUJwMExHNnV0M0EwcmVQWVdaZjVZc0taZWJ4TDZ3L3ZGNS80MW1LOWFacXUwOU1YbTFDa1ozL0hzTmFnVUdkK1dqN1N4WGxZYmRpK1NSMmE2UXJxZ2lSTWJ2MDg4UW5xcktDSFM2a2pYeURQTlQ1bThyYUk1SFFhQ012OXFEWjhSWkhnVVFwTXlmZTh4bDB2ckc2QjlxTnkzd2wxUnI5RnVUUGYvVlFUL0lZZ3ZlSG1ib0pxUjlDODFTWERjY1A1Y2pub1NjMU8xeEczYlpISExNMWo5WWppU2Zsd0lDODRHdzNxTXNjdklYY0txOERZNTZpbjkzbkNMaU5ZOTlwS1dDWGZxK25WZTUvTlY2WW85dzhvM2tMVGhPa285dk5Wb3Z4bHozMmE0NjhTNktIL0tQYUxnVnYwSEg5NXJ2OEE0WXZsNkdIaEE4S1NvNGJGaUQyUDJQTVJXMFpzT1dLbkVUc2RzYk9Jblhtc09qWklTdW9IZnFqaTB1TTdvNVRwc0x5SjRDcjVCUW9tbENna3QwRitySXR4TWs4Q3A2UjFPVFk4eE00UXEvYlAyTFBlK3ZFZjZlSUhBQUFBLy84REFGQkxBd1FVQUFZQUNBQUFBQ0VBU3RpS2tyc0FBQUFFQVFBQUhRQUFBSGR2Y21RdloyeHZjM05oY25rdmQyVmlVMlYwZEdsdVozTXVlRzFzak03QmFzTXdETWJ4ZTJIdkVIUmZuZlV3U2toU0tLTXYwUFVCWEVkcERMRmtKRzNlOXZRMWJKZmRlaFNmK1BIdkQxOXBiVDVSTkRJTjhMSnRvVUVLUEVXNkRYQjVQejN2b1ZIek5QbVZDUWY0Um9YRCtMVHBTMWZ3ZWtheitxbE5WVWc3R1dBeHk1MXpHaFpNWHJlY2tlbzJzeVJ2OVpTYjQzbU9BZDg0ZkNRa2M3dTJmWFdDcTdkYW9Fdk1DbjlhZVVRckxGTVdEcWhhUTlMNjZ5VWZDY2JheU5saWlqOTRZamtLRjBWeFkrLyt0WTkzQUFBQS8vOERBRkJMQXdRVUFBWUFDQUFBQUNFQTRzZjUxdVVCQUFBcUJRQUFFZ0FBQUhkdmNtUXZabTl1ZEZSaFlteGxMbmh0YktTVFg0N1RNQkRHMzVHNFErUjNOczRmU3JiYWRGWEs1bkVmMk9VQWJ1bzBsbUk3OHJnTmV3WWV1UWMzNERad0R5YTJHMFFyUkNvYUtWSytjYjdPL1BMTjNmMW4yVVZIYmtCb1ZaTGtocEtJcTFydmhOcVg1Tk56OWFZZ0VWaW1kcXpUaXBma2hRTzVYNzErZFRjc0c2MHNSUGkrZ3FVcFNXdHR2NHhqcUZzdUdkem9uaXVzTmRwSVp2SFI3R1BkTktMbUgzUjlrRnpaT0tWMEVSdmVNWXYvRGEzb2dRUzNZWTdib00ydU43cm1BTmlzN0x5ZlpFS1JWZWd1R3BhS1NleDZ3enF4TmNJVmVxWTA4QVJyUjlhVmhLYTBvbS94UGw0NXpjWTdpVWVIdW1VR3VKME9VaTgzVElydTVhVENJQUI4b1JlMmJrLzZrUm5CdGgzM0pSQjdMQnhnUzB2eWtGQkswNm9pWGtsS2txT3cza3hLaWszNTMyMDRrMDBLZmg1c3pQbTRJOG10ODBFRmZjSmJycy9ZZjU4TEVqKy9mZm54L2FzRHdUcjdpSFJPSFQ4SitYUlFZWlFMUmdsZG9IMUdrOVBsRDU0eEtoWmUvcE1STzFnZGZPY2hDb05rdnhHbFJWR05hbEFtUk1uaUg0aEd0a21BTmhQUnM1QWNva2MrUkIrMVpENU5sNkZKRVVpR3djbGRlTEtyUW1PY3J3dlozTkRnR09sNmlnaE9za0hsWFpHNzJUQllFeEU2SXpUZVozNW9Oa3ppOXJDL3JNOUl3Sk1ZaVZ5M1B0ZVRXT1BjTkgwNFd4OUs4L2ZuMlpqUzhqL3JFL1lJVnI4QUFBRC8vd01BVUVzREJCUUFCZ0FJQUFBQUlRQ0dTRHc5aWdnQUFGNUNBQUFQQUFBQWQyOXlaQzl6ZEhsc1pYTXVlRzFzMUZ0TGI5dEdFTDRYNkg4Z2VIZXNseVhIaUJMNEVUY0c4bkFpR3oydnlKWEptdUtxSkJVL3prV0wvSUVlaWx4eTY2RzNvb2YrSWJmOUY5MmRKVmMwS1lvekpwT2l5RUVSdVR2ZlBMOVoyenRQbmwzUEErczlqMkpmaEdPNys2aGpXengwaE91SEYyUDcvT3g0YTllMjRvU0ZMZ3RFeU1mMkRZL3RaMCsvL3VySjFWNmMzQVE4dHFTQU1ONkx4cmFYSkl1OTdlM1k4ZmljeFkvRWdvZnkzVXhFYzViSXI5SEZ0cGpOZkljZkNXYzU1Mkd5M2V0MGh0c1JEMWdpd1dQUFg4UjJLdTBLSSsxS1JPNGlFZzZQWTZudFBORHk1c3dQN2FkU1BWYzRSM3pHbGtFU3E2L1JhWlIrVGIvQng3RUlrOWk2Mm1PeDQvdG5VbkZwNHR3UFJmUmlQNHg5Vzc3aExFNzJZNS9sWHo1UG42bjNubHFZZjJsMk9uR1NFM2pndTc2OXJVQXZlUlRLamU5Wk1MWjcrbEY4YXg1MHN5ZUhTaSs5S0YwVnNQQWllOGJEcmZOSlhyK3hmZXR0SGI1V2o2WVNhbXl6YUd1eXI0UnRnL0haWjg0SkMrTVN2YXJnTVJrWEdhV0pqckwwSjUrOUZNNGxkeWVKZkRHMlphYkF3L09UMDhnWGtaL2NqTzNIajlPSEV6NzNYL2l1eTFWU1pRdER6M2Y1dHg0UHoyUHVycDYvUFlZVVNTVTZZaGttMGpIREVVUXhpTjNuMXc1ZnFCU1JlQ0ZURVhxdE5nUktiSnpEQVlXVy9rb2IvYUNBQ2crL3p5QlRiNjlGOFRoVFpXQ0IvaHVCd09wbFk2Q2VzaWh2QU1nbDZkcHZMbUxRWE1ST2N4SEQ1aUpHelVWSThtc2FFWjBidWF6RUJ6VVJqazYrZkU3MEgydUNXSnV5YWtjcGkycDNsSkttZGtjcFIycDNsRktpZGtjcEEycDNsQUpldTZNVTM5b2RwWEJ1M09Fd0lLNWlGdlhCRzZqQ1B2T1RnS3Y5R3dtbzI1RHEwcVpnbmJLSVhVUnM0Vm1xTVJiVjNrU1drK1Uwd2FrS2RQcHdzcHdra1FndmFqM1MwMlh3WUU1K1BsOTRMUGJsaWFURzliMkdyajlqMDRCYjMwUytXd3UxbzVPdlpCT2NLdGJ5d1duQUhPNkp3T1dSZGNhdmRVUUorMThMYTdKZ2p1eUN0Y28xRE90TC84SkxySWtITGJjV2JGamg5R3BQYVBrdi9SaDhzTEdZaGhXbTFBbEh4WEJZa1pmVndsOXgxMS9PTTljZ1RpTkR6ZWVFTUJjZ1FNWE5MaHFvRUpXTHVOWUtGUUNNQ2JwZDBFMEErUWo5ZFhPaHkxY3h4dWl2VzlFRDVTUDAxNDNyZ2ZJaFB6YkhsOHcwUnl5NnRGRGxOU0xYN3FFSVJEUmJCbGtOMU5MRGlGekJCZ0puQXJtSWpYd1VTWXpJRlh5UFBxMTl4NUUvdVdIeWxCeUxGWThTVU1qaDBDaFFiSGhieUVFcDBGNlhZQkU1UUFXc0hnR3JHZGNTZ01pays0Ni85OVV2anFqTkFGamFuRFZyeTdsZjRRSFpnbEJuNkxkTGtkU2ZvWHNWbklkRk9RbmxyMHRpYnVIUStoV1ZoMFZMODBuM08wS01telUrQWxDekRrZ0FhdFlLQ1VBVitWRjk1akU5RVEvU3ZEa1NzTWkwYkxvWXBCMmFtVWRrWmpaQXRCYlFVdDlFbkw4cXFyYzZGOHA5RTRGQ0RsQzVieUpReU5FcDlETFROeEZZcmZWTkJGWkYxNmlPVVo1VEtVYVIrMllleUp3RUVCYTFROTRJb0hiSUd3SFVEbmtqZ0pxVGR6MUllK1NOd0NKemcrSFVQSGtqZ0dBSjVVZDlBNVFuYndRUW1SczAyNlcvTThyNkhralovTU50QytTTlFDRUhxRXplQ0JSeWRLcklHNEVGU3lpWlVNQXlWSWZBYW9lOEVVRHRrRGNDcUIzeVJnQzFROTRJb0hiSUd3SFVuTHpyUWRvamJ3UVdtUnNNcCtiSkd3RkVwZ2NEbENkdkJCQXNvWEREV3ZLR3F2L3M1STFBSVFlb1RONElGSEowQ29ScURxa0lMSEtBQ2xpR3ZCRllzSVNTRENrV0pEZkZxSGJJRzJGUk8rU05BR3FIdkJGQTdaQTNBcWc1ZWRlRHRFZmVDQ3d5TnhoT3paTTNBb2hNRHdZb1Q5NElJREkzckNWdktNYlBUdDRJRkhLQXl1U05RQ0ZIcDBDb2h1Y1FXT1FBRmJBTWVTT3dJRjhha3pjQ0NKWThGSWhpVVR2a2piQ29IZkpHQUxWRDNnaWc1dVJkRDlJZWVTT3d5TnhnT0RWUDNnZ2dNajBZb0R4NUk0REkzTENXdktGR1BqdDVJMURJQVNxVE53S0ZISjBDb1JyeVJtQ1JBMVRBTWxTSHdHcUh2QkZBa0ppTnlSc0JCRXNlQUFSVlJBbFRPK1NOc0tnZDhrWUFOU2Z2ZXBEMnlCdUJSZVlHdzZsNThrWUFrZW5CQU9YSkd3RkU1Z1oxejFiZUYwVmZUKzFXSkFIMm5rRjJxd0VOMktzSUVoWXdOZkFkbi9GSVRpTHgrdHNoRFFFekN3bUlGZW1CTmZGQWlFc0xkN0c3WDVFZ2FDaC9HdmdDcm5UZndDMmQzQ0JDZjdSaGt1RHN6YUgxUWcvQWxQWkJTdDIvZVNObmpQTGpRbXJNQ2NiRHBKN0p6VUtPN0N5eW0rVkttaHdsVW5OWjZRZ1FMRHlSQTBFTUpuN1VpSTljQTVOUDZhQVAvTWsyQllUL3kzRTFOMXZUNlF5T0J2M0JVQnNqWjVvVStKWHZpcXREZVljOUVvRlpxRmQ4NTJRUHBpTHgxRE9wUEd5VG42QkxXWHZIaytvN0NZODJhZDhwcVY5eHJSNU1XTTEwWk9wa2sxOW1wRW12dTNmSlUydGJvV1dpcnBKdjByQmIwbEE3MklKTDZObzlaYjNrVUJkb3Nqb2RybGZNM01hQzE4azAwS0dRL3prSlZiVGtVQi84NVUwbmhIdk50Rmo1L3BBSHdTc0dnVXZFb25wcHdHZUpmdHZ0UUJjdGlKSUJUY1M4ZW44RWw4eEJrM1VDWlB6enl1aXZ5b2pxeEFpWDh5bVAwdnZ4VlduZFcrTjJmVmRXTzlMVXBOUWMwaDdyOFdxOTdpWHNxc0Q2SlUxS0V3S2cwcFRKQ2IwM1prcVJkWWk1VWFqUGcxRi9kS3hGeURGTVZWNk91dEtibWJ2YlVmL1VlMmtRTEtnMjdCNlByQXdibEF3N1lFRWc1TXdnakQyc05Vb3JGUGpoWmFiSW9TeHpvcVgzVDZhQVUydjhhdHF6dTZ2aDR0dlZ0S2QrVnUrSmV5RjJsckhNL0ltYTFTeXlLaGlsZko0bjFyc1BmOTc5L3V2ZHB4L3ZmdjdwN3VOdjFzcnlpdERuM2NRR1JDZDlJWTlVNU1aT0tUZlVaS2ZrY2tKV1VBdUFsQlpwejFvY3VGQWJoc1dBQzlSc2N6b0JkanUyOVpGU2p1UmtZWVpDa3Exem1RaGxqMHliVEl6c0JqRDRMRCt6L0ZhM3duVVBXNGg0YkErNk8zM3RoTndhNEVqVlJHREpicjhEalZYS3plVEZJVnVjQ1RqbnBuU1ZPbWZWV0ZPY1ZCOWQ4V25oZi9uY0w3Zmxmejc5OGRmSEQ4U2MzL2xmNWZ5d2xQTXpJUzlCazNJK1BZODBPUkJVRjM2YTgxbFM1Ukx3U3lXcE9reGtKZk1mcDJqNVhDWlQ5TzhmZmlHbWFIb0VSZ2VzT2pxTmlqVnIzdkhUZndFQUFQLy9Bd0JRU3dNRUZBQUdBQWdBQUFBaEFFcllpcEs3QUFBQUJBRUFBQlFBQUFCM2IzSmtMM2RsWWxObGRIUnBibWR6TG5odGJJek93V3JETUF6RzhYdGg3eEIwWDUzMU1FcElVaWlqTDlEMUFWeEhhUXl4WkNSdDN2YjBOV3lYM1hvVW4vang3dzlmYVcwK1VUUXlEZkN5YmFGQkNqeEZ1ZzF3ZVQ4OTc2RlI4elQ1bFFrSCtFYUZ3L2kwNlV0WDhIcEdzL3FwVFZWSU94bGdNY3VkY3hvV1RGNjNuSkhxTnJNa2IvV1VtK041amdIZk9Id2tKSE83dG4xMWdxdTNXcUJMekFwL1dubEVLeXhURmc2b1drUFMrdXNsSHduRzJzalpZb28vZUdJNUNoZEZjV1B2L3JXUGR3QUFBUC8vQXdCUVN3TUVGQUFHQUFnQUFBQWhBRU9FTGxwMkFRQUF4UUlBQUJBQUNBRmtiMk5RY205d2N5OWhjSEF1ZUcxc0lLSUVBU2lnQUFFQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFuRkxMVHNNd0VMd2o4UTlSN3RScGl3cXFOcTVRRWVMQW8xSURuQzFuazFnNHRtVzdGZjE3TmcxTmc3amgwODZzUFo0ZEcxWmZyVTcyNklPeUprK25reXhOMEVoYktsUG42VnZ4Y0hXYkppRUtVd3B0RGVicEFVTzY0cGNYc1BIV29ZOEtRMElTSnVScEU2TmJNaFprZzYwSUUyb2I2bFRXdHlJUzlEV3pWYVVrM2x1NWE5RkVOc3V5QmNPdmlLYkU4c29OZ21tdnVOekgvNHFXVm5iK3dudHhjR1NZUTRHdDB5SWlmK25zYUdBREFZV05RaGVxUlo0UlBRRFlpQm9EbndMckMvaXd2Z3g4c1FEV1Y3QnVoQmN5VW5aOGZuTURiSVRoemptdHBJaVVLbjlXMHR0Z3E1aThIdWRQdXZQQXhsdUFNdG1pM0hrVkQ1Mk5NWVFuWmNqSUhGaGZrREV2YWk5YzgrTnVRTENWUXVPYUJ1ZVYwQUdCblFsWTI5WUpjK0RrODFTUjNtZDRjNFc5NzVMNU9mS2JIQTM1b1dLemRVS1NsK3ZyMlhqY1VRZTJGQXFXNVAra2R5YmdrUjdENis1U2lzclVXSjcyL0cxMEFiNzN2NUpQWjVPTTFqR3hFMGV2TW53WC9nMEFBUC8vQXdCUVN3TUVGQUFHQUFnQUFBQWhBT2tXeW9SQUJ3QUFrVG9BQUJnQUFBQjNiM0prTDJkc2IzTnpZWEo1TDNOMGVXeGxjeTU0Yld5MG0xMVQyem9RaHUvUHpQa1BIdC9UZkxWSnl6UjBLSlJUWnZwQkc1aHpyZGdLMXNHeGNteWxRSDk5cFpXdEdEdU9kN0VaTGlDeXBIZFh1M3BrUVB2K3c4TTY5bjd4TkJNeW1mdWpWMFBmNDBrZ1E1SGN6djJiNjR1anQ3NlhLWmFFTEpZSm4vdVBQUE0vblB6OTEvdjc0MHc5eGp6ejlBUkpkcHpPL1VpcHpmRmdrQVVSWDdQc2xkendSRDlieVhUTmxQNlkzZzdrYWlVQ2ZpNkQ3Wm9uYWpBZURxZURsTWRNYWZFc0Vwdk16MmU3eDh4Mkw5TndrOHFBWjVtMmRoM2IrZFpNSlA2Sk5pK1V3VGxmc1cyc012TXh2VXJ6ai9rbitIWWhFNVY1OThjc0M0UzQxb1pyRjljaWtlbm4weVFUdm43Q1dhWk9NOEhLRHovbGJlWjVaRHFXSDdxUlFhWktFMzRVb2ZBSFJ2U09wNGtlK0l2RmMzOXNtN0xmcm1GVXRKd1p1MnludkZmTWt0dWlqU2RITjR1eWZYUC9kM1IwOXMwMExiWFUzR2ZwMGVMVVREWUE1NHZ2cFVYWXVDV3h2U29ycHVPaW83U3dVZGJyeVZkZlpIREh3NFhTRCthK3poUm92TG04U29WTWhYcWMrKy9lNVkwTHZoYWZSUmh5azFSRnh5UVNJZjgzNHNsTnhzTmQrNDhMU0pGOHhrQnVFNlVYWmpxREtNWlorT2toNEJ1VElsb3ZZU1pDMzh5QTJFeWJsWFRBb0szWVdXTWJLcXJRK0g4aG1hLzJYcFdJTTdNTlBMRC9vQkI0dmUwc05EWWVsUjJBZVVtMlRycFA4YnI3RkcrNlR6SHRQc1dzK3hRYWZsMGpZbk9qbEpYNG9Db1oyT1FyNThUa25RWEUzcFExSTJwWjFEcWlsalN0STJvNTBqcWlsaEt0STJvWjBEcWlGdkRXRWJYNHRvNm9oZlBnaUlBQnVLcFpOSUhWUUczc2E2Rmlic1lmQk5Db0krcnlROEc3WWltN1Rka204c3pCV0RYN0VDd1gyNlhDbVFvNGZUNHNGeXFWeVczcmlvenRObmcya3ordE54SExoSDRqYVZuNmNjZWx2MmJMbUh2L3BDSnNsWHBqazYvbUU3eFY3T1hCVmN3Q0hzazQ1S2wzelI5c1JBbmp2MGx2c1dHQlBnVmJqZXNZMWkvaU5sTGVJb0lqdDFWczJyRG96U3RoNS84aU1saURnNXRwMnVCSzIrU29HRTRiOHJKNThxODhGTnQxc1RTSXQ1R3A1VGtoekJVSk1QSHdFcjAySWFwdjRsWXZUQUF3THRqamd1NEN6SSt3M3g0dTlQbE5qREgyMjZQb21mTWo3TGNIMXpQbmgvdzRIRjh5YWM1WmV1ZWh0dGVNdkhmUFpDelQxVFl1OWtBckhtYmtIZXdrY0M2UU43R2JId1dKR1hrSFA4R25keG9FK2pjM1RKNlNZN0hqS0VHRkhBNnJBcHNON3dzNUtCWHNqUWdla1FOVTBSb1R0THF4bGlCRWh1NVAva3VZUHh4UkR3T2d0SHZYYk4zT2s0WVYwRWNRNmgzNngxYXE5bmZvY1FQenNDcVhpZjV6U2NZOW5OcWtZZWRoMWZKOHN1Y2RJY2JkRGo2Q1VMY1RrQ0RVN1Nna0NEWGtSL003anpzVDhTTGREMGVDRmhuTDdoU0R0RU9UZVVZbXN4T2lIUUU5blp1STk2K0czZHVjQy9WekU2RkNEbEQ5M0VTb2tLTlRPY3ZjdVluUTZ1M2NSR2cxbkJyTk1Tb3psZUlVK2R3c0M3azNBWVJIL2NBYklkUVB2QkZDL2NBYklkUWQzdTBpL2NFYm9VVm1nMk5xR2Q0SUllaEMrVlhmQ1pYaGpSQWlzOEhTTHYrYlVYSHV3U3lIZjdudEFkNElGWEtBNnZCR3FKQ2owd1J2aEJaMG9XUkNSY3VoRHFIVkQ3d1JRdjNBR3lIVUQ3d1JRdjNBR3lIVUQ3d1JRdDNoM1M3U0g3d1JXbVEyT0thVzRZMFFJdVBCQ1pYaGpSQ0NMaFEyN0lVMzdQb1hoemRDaFJ5Z09yd1JLdVRvVklEcVhsSVJXdVFBVmJRY3ZCRmEwSVdTRExrV0pEZkZxWDdnamZDb0gzZ2poUHFCTjBLb0gzZ2poTHJEdTEya1AzZ2p0TWhzY0V3dHd4c2hSTWFERXlyREd5RkVac05lZU1ObWZIRjRJMVRJQWFyREc2RkNqazRGcUk1ekNDMXlnQ3BhRHQ0SUxjaVh6dkJHQ0VHWDV3cFJQT29IM2dpUCtvRTNRcWdmZUNPRXVzTzdYYVEvZUNPMHlHeHdUQzNER3lGRXhvTVRLc01iSVVSbXcxNTR3eDU1Y1hnalZNZ0Jxc01ib1VLT1RnV29EdDRJTFhLQUtsb09kUWl0ZnVDTkVJTEU3QXh2aEJCMGVZWVE3Q0pLbVBxQk44S2pmdUNORU9vTzczYVIvdUNOMENLendURzFERytFRUJrUFRxZ01iNFFRbVEzbW5xMitMNHErbmpwcVNBTHNQWVBpVmdOYWNOd1FKS3hnN3VCUHZ1S3Bya1RpN2JkRE9nb1dIaElVRzlJRDYrSkhLZTg4M01YdVNVT0NvS1hFTWhZU3JuUS93aTJkVWlIQ1pIYWdrdUQ2KzVuMzJSYkExTVpCU2oyOWVhTnJqTXJsUXFiTUNjckR0SjNxY2FOTGRqYkZ6WEl6bXk0bE1uVlplUWtRZEx6VUJVRU1LbjVNaVkvdUE1VlBlYUVQL01zMkY0U2ZkZVdTa2JnWG9idy8wemZWVXhrWFE0YldxZitDb21FcFZaU1hRY0V3YlNvbzFtME1JbTFrb0hoNnlNWmh6Y2lHeS9OZzZLNXlvekNucU85eWhVdTIzNU9ybkxxcDJVcGxMb3dmc25CVXM5QXVvd2RYemUzeTFPM1NwVnRneWU0ZGNMOWg3czRWUEZiTDJJWkMvM0NaaE5wSlhib0gvMSt6WVE4Zm1KMVdQei9qY2Z5VlFlQ1UzRFIzamZsSzJhZWpJWnlWbGFsMFFKVmNONDlQNFNvNVdMSnZBcjJ5WldQc1IrTkU4NUluMi9XU3Ava3QrS2JrSGU5WmRuc2oxaTZrMjNuYWNraHU3SW8zMi9Va1lYZmJhRkt6cEZZSEFDWXRtYTdEKys1cUVWbStkZEM1b1l0R1RjVEJtZUZ3ZUQ1OE03dXcwZGJGbG1aN0JlYmlidEhqN2RCOG1lZmFJZWl3YzZ6NEtUdjVBd0FBLy84REFGQkxBd1FVQUFZQUNBQUFBQ0VBNHNmNTF1VUJBQUFxQlFBQUd3QUFBSGR2Y21RdloyeHZjM05oY25rdlptOXVkRlJoWW14bExuaHRiS1NUWDQ3VE1CREczNUc0UStSM05zNGZTcmJhZEZYSzVuRWYyT1VBYnVvMGxtSTc4cmdOZXdZZXVRYzM0RFp3RHlhMkcwUXJSQ29hS1ZLK2NiN08vUExOM2YxbjJVVkhia0JvVlpMa2hwS0lxMXJ2aE5xWDVOTno5YVlnRVZpbWRxelRpcGZraFFPNVg3MStkVGNzRzYwc1JQaStncVVwU1d0dHY0eGpxRnN1R2R6b25pdXNOZHBJWnZIUjdHUGROS0xtSDNSOWtGelpPS1YwRVJ2ZU1Zdi9EYTNvZ1FTM1lZN2JvTTJ1TjdybUFOaXM3THlmWkVLUlZlZ3VHcGFLU2V4Nnd6cXhOY0lWZXFZMDhBUnJSOWFWaEthMG9tL3hQbDQ1emNZN2lVZUh1bVVHdUowT1VpODNUSXJ1NWFUQ0lBQjhvUmUyYmsvNmtSbkJ0aDMzSlJCN0xCeGdTMHZ5a0ZCSzA2b2lYa2xLa3FPdzNreEtpazM1MzIwNGswMEtmaDVzelBtNEk4bXQ4MEVGZmNKYnJzL1lmNThMRWorL2ZmbngvYXNEd1RyN2lIUk9IVDhKK1hSUVlaUUxSZ2xkb0gxR2s5UGxENTR4S2haZS9wTVJPMWdkZk9jaENvTmt2eEdsUlZHTmFsQW1STW5pSDRoR3RrbUFOaFBSczVBY29rYytSQisxWkQ1Tmw2RkpFVWlHd2NsZGVMS3JRbU9jcnd2WjNORGdHT2w2aWdoT3NrSGxYWkc3MlRCWUV4RTZJelRlWjM1b05remk5ckMvck05SXdKTVlpVnkzUHRlVFdPUGNOSDA0V3g5SzgvZm4yWmpTOGovckUvWUlWcjhBQUFELy93TUFVRXNEQkJRQUJnQUlBQUFBSVFDSE96WG1pUUVBQVBjQ0FBQVJBQWdCWkc5alVISnZjSE12WTI5eVpTNTRiV3dnb2dRQktLQUFBUUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFDY1VrRk93ekFRdkNQeGg4ajN4RTZLS2hTMXFRU29KeW9oVVFUaVp1eHRhNW80bHUwMnpRT1FlQUIvZ0JlQXhJTW96OEJKMnJRVm5QREp1ek03dXp0MmI3REtVbThKMm9oYzlsRVlFT1NCWkRrWGN0cEhOK09oZjRvOFk2bmtOTTBsOUZFSkJnMlM0Nk1lVXpITE5WenBYSUcyQW96bmxLU0ptZXFqbWJVcXh0aXdHV1RVQkk0aEhUakpkVWF0Qy9VVUs4cm1kQW80SXFTTE03Q1VVMHR4SmVpclZoRnRKRGxySmRWQ3A3VUFaeGhTeUVCYWc4TWd4RHV1QloyWlB3dHFaSStaQ1ZzcXQ5Tm0zSDF0emhxd1phK01hSWxGVVFSRnB4N0R6Ui9pdTlIbGRiMnFMMlRsRlFPVTlEaUxyYkFwSkQyOHU3cWJXVHc4QXJOTnVnMGN3RFJRbSt1RXVQUDkvclQrZkYyL3ZIMDlmOVQxVzdDeWZRNWxrV3R1bk1SQjVEUTRHS2FGc3U0eG13WUhDY2RPcWJFajk3b1RBZnlzL0tQWGIwN1ZVc05TVkQ4a0NldWViZWkyckUxdGhnZnVPWnZpeHRRdGN0czV2eGdQVVJLUmtQaWs2NU9UTVluaUtJb0p1YTlXTzZpdmJHc1MyV2JJZnl0dUJScVhEcjlxOGdNQUFQLy9Bd0JRU3dFQ0xRQVVBQVlBQ0FBQUFDRUFHa01mMWUwQkFBQWpEQUFBRXdBQUFBQUFBQUFBQUFBQUFBQUFBQUFBVzBOdmJuUmxiblJmVkhsd1pYTmRMbmh0YkZCTEFRSXRBQlFBQmdBSUFBQUFJUUFla1JxMzh3QUFBRTRDQUFBTEFBQUFBQUFBQUFBQUFBQUFBQ1lFQUFCZmNtVnNjeTh1Y21Wc2MxQkxBUUl0QUJRQUJnQUlBQUFBSVFEblRzcFNmZ0VBQUZjSUFBQWNBQUFBQUFBQUFBQUFBQUFBQUVvSEFBQjNiM0prTDE5eVpXeHpMMlJ2WTNWdFpXNTBMbmh0YkM1eVpXeHpVRXNCQWkwQUZBQUdBQWdBQUFBaEFQWEE0R0J0Q2dBQTdWY0FBQkVBQUFBQUFBQUFBQUFBQUFBQUNnb0FBSGR2Y21RdlpHOWpkVzFsYm5RdWVHMXNVRXNCQWkwQUZBQUdBQWdBQUFBaEFCakovMjF0QVFBQTNnTUFBQklBQUFBQUFBQUFBQUFBQUFBQXBoUUFBSGR2Y21RdlptOXZkRzV2ZEdWekxuaHRiRkJMQVFJdEFCUUFCZ0FJQUFBQUlRQ3dCdTR2UlFFQUFPVUNBQUFRQUFBQUFBQUFBQUFBQUFBQUFFTVdBQUIzYjNKa0wyWnZiM1JsY2pNdWVHMXNVRXNCQWkwQUZBQUdBQWdBQUFBaEFBSjhySXBGQVFBQTVRSUFBQkFBQUFBQUFBQUFBQUFBQUFBQXRoY0FBSGR2Y21RdmFHVmhaR1Z5TXk1NGJXeFFTd0VDTFFBVUFBWUFDQUFBQUNFQXNBYnVMMFVCQUFEbEFnQUFFQUFBQUFBQUFBQUFBQUFBQUFBcEdRQUFkMjl5WkM5bWIyOTBaWEl5TG5odGJGQkxBUUl0QUJRQUJnQUlBQUFBSVFDd0J1NHZSUUVBQU9VQ0FBQVFBQUFBQUFBQUFBQUFBQUFBQUp3YUFBQjNiM0prTDJadmIzUmxjakV1ZUcxc1VFc0JBaTBBRkFBR0FBZ0FBQUFoQU5VamlCcVZBZ0FBVHdrQUFCQUFBQUFBQUFBQUFBQUFBQUFBRHh3QUFIZHZjbVF2YUdWaFpHVnlNaTU0Yld4UVN3RUNMUUFVQUFZQUNBQUFBQ0VBVENmMVZHMEJBQURZQXdBQUVRQUFBQUFBQUFBQUFBQUFBQURTSGdBQWQyOXlaQzlsYm1SdWIzUmxjeTU0Yld4UVN3RUNMUUFVQUFZQUNBQUFBQ0VBQW55c2lrVUJBQURsQWdBQUVBQUFBQUFBQUFBQUFBQUFBQUJ1SUFBQWQyOXlaQzlvWldGa1pYSXhMbmh0YkZCTEFRSXRBQW9BQUFBQUFBQUFJUUJwZWRTaXdRVUFBTUVGQUFBVkFBQUFBQUFBQUFBQUFBQUFBT0VoQUFCM2IzSmtMMjFsWkdsaEwybHRZV2RsTVM1d2JtZFFTd0VDTFFBVUFBWUFDQUFBQUNFQXh4eHRGSndHQUFCUkd3QUFGUUFBQUFBQUFBQUFBQUFBQUFEVkp3QUFkMjl5WkM5MGFHVnRaUzkwYUdWdFpURXVlRzFzVUVzQkFpMEFGQUFHQUFnQUFBQWhBSVBRdGVYcUFBQUFyUUlBQUNVQUFBQUFBQUFBQUFBQUFBQUFwQzRBQUhkdmNtUXZaMnh2YzNOaGNua3ZYM0psYkhNdlpHOWpkVzFsYm5RdWVHMXNMbkpsYkhOUVN3RUNMUUFVQUFZQUNBQUFBQ0VBY2pLMndxOERBQUJyQ0FBQUVRQUFBQUFBQUFBQUFBQUFBQURSTHdBQWQyOXlaQzl6WlhSMGFXNW5jeTU0Yld4UVN3RUNMUUFVQUFZQUNBQUFBQ0VBei9PQ2NaVUNBQUFIQndBQUdnQUFBQUFBQUFBQUFBQUFBQUN2TXdBQWQyOXlaQzluYkc5emMyRnllUzlrYjJOMWJXVnVkQzU0Yld4UVN3RUNMUUFVQUFZQUNBQUFBQ0VBd25IOGVSOERBQURYQmdBQUdnQUFBQUFBQUFBQUFBQUFBQUI4TmdBQWQyOXlaQzluYkc5emMyRnllUzl6WlhSMGFXNW5jeTU0Yld4UVN3RUNMUUFVQUFZQUNBQUFBQ0VBU3RpS2tyc0FBQUFFQVFBQUhRQUFBQUFBQUFBQUFBQUFBQURUT1FBQWQyOXlaQzluYkc5emMyRnllUzkzWldKVFpYUjBhVzVuY3k1NGJXeFFTd0VDTFFBVUFBWUFDQUFBQUNFQTRzZjUxdVVCQUFBcUJRQUFFZ0FBQUFBQUFBQUFBQUFBQUFESk9nQUFkMjl5WkM5bWIyNTBWR0ZpYkdVdWVHMXNVRXNCQWkwQUZBQUdBQWdBQUFBaEFJWklQRDJLQ0FBQVhrSUFBQThBQUFBQUFBQUFBQUFBQUFBQTNqd0FBSGR2Y21RdmMzUjViR1Z6TG5odGJGQkxBUUl0QUJRQUJnQUlBQUFBSVFCSzJJcVN1d0FBQUFRQkFBQVVBQUFBQUFBQUFBQUFBQUFBQUpWRkFBQjNiM0prTDNkbFlsTmxkSFJwYm1kekxuaHRiRkJMQVFJdEFCUUFCZ0FJQUFBQUlRQkRoQzVhZGdFQUFNVUNBQUFRQUFBQUFBQUFBQUFBQUFBQUFJSkdBQUJrYjJOUWNtOXdjeTloY0hBdWVHMXNVRXNCQWkwQUZBQUdBQWdBQUFBaEFPa1d5b1JBQndBQWtUb0FBQmdBQUFBQUFBQUFBQUFBQUFBQUxra0FBSGR2Y21RdloyeHZjM05oY25rdmMzUjViR1Z6TG5odGJGQkxBUUl0QUJRQUJnQUlBQUFBSVFEaXgvblc1UUVBQUNvRkFBQWJBQUFBQUFBQUFBQUFBQUFBQUtSUUFBQjNiM0prTDJkc2IzTnpZWEo1TDJadmJuUlVZV0pzWlM1NGJXeFFTd0VDTFFBVUFBWUFDQUFBQUNFQWh6czE1b2tCQUFEM0FnQUFF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";
        Sign = "test";
        string strOpcode = "0000";

        SqlHelper Helper = new SqlHelper("EmrDB");
        string strSelectDocument = @"select registryID from EmrDocument where registryID = '" + registryID + "'";
        int Series = -1;
        DataTable dt = Helper.GetDataTable(strSelectDocument);



        try
        {
            if (dt != null && dt.Rows.Count == 0)
            {
                Series = 1;
                //XmlDocument xmldoc = new XmlDocument();
                //SetNode(xmldoc, registryID, Series);
                //string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                string NoteIDSeries = "99999999";
                //noteForEmrDoc.Attributes["Series"].Value = Series.ToString();


                //XmlNode Root = (XmlNode)(xmldoc.DocumentElement);
                //XmlNode Node = xmldoc.ImportNode(noteForEmrDoc, true);

                //Root.AppendChild(Node);
                string strInsert = @"Insert into EmrDocument (registryID,ArchiveNum,Document,status) 
                                Values('" + registryID + "','" + ArchieveNum + "','" + "test" + "','0')";


                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc + "')";
                string InsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strInsert, InsertSign };

                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";



            }
            else
            {

                SqlTransaction tran;
                SqlCommand cmd = new SqlCommand();
                using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["EmrDB"]))
                {
                    try
                    {
                        con.Open();
                        tran = con.BeginTransaction();
                        try
                        {
                            string strSelect = @"select  Document.value('(Emr/@Series)[1]','varchar(50)') as Series
                                from EmrDocument with (rowlock)  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strSelect;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            da.Dispose();
                            cmd.Dispose();
                            DataTable dtSeries = ds.Tables[0];
                            if (dtSeries != null && dtSeries.Rows.Count != 0)
                            {
                                string str = dtSeries.Rows[0]["Series"].ToString();
                                Series = Convert.ToInt32(str);
                                Series = Series + 1;
                            }
                            // noteForEmrDoc.Attributes["Series"].Value = Series.ToString();
                            string strUpdate = @"update EmrDocument set document.modify
                                ('replace value of (/Emr[@Series]/@Series)[1] with " + Series.ToString() + " ')  where RegistryID='" + registryID + "'";
                            cmd.CommandText = strUpdate;
                            cmd.Connection = tran.Connection;
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            tran.Dispose();
                            throw (e);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("", LogType.Error, "", "", ex.ToString(), RecordType.Constraint);

                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Dispose();
                    }
                }




                string strUpdateNode = @"update emrdocument SET document.modify ('insert " + noteForEmrDoc + " as last into (/Emr[1])' )    where RegistryID='" + registryID + "'";


                //string NoteIDSeries = MakeNoteIDSeries(noteForEmrDoc.Attributes["NoteID"].Value, Series);
                string NoteIDSeries = "99999999";
                string Insert = @"Insert Into EmrNote (RegistryID,NoteIDSeries,NoteDocument) values('" + registryID + "','" + NoteIDSeries + "','" + noteForWordDoc + "')";
                string strInsertSign = @"Insert Into signContent (RegistryID,NoteIDSeries,ciphertext,signer) values('" + registryID + "','" + NoteIDSeries + "','" + Sign + "','" + strOpcode + "')";
                string[] strList = { Insert, strUpdateNode, strInsertSign };
                if (Helper.ExecuteSqlByTran(strList) == true)
                    return null;
                else
                    return "Error";



            }
        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return ex.Message + "-" + ex.Source;
        }
    }

    [WebMethod(Description = "Insert or Replace attribute of /Emr[1] in Emrdocument")]

    //术后病程记录分组续打
    public bool IsRemberPoint1(string RegistryID, ref string DocRange)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT Range FROM TB_RemberGroup where RegistryID='" + RegistryID + "'";
        DataTable dt = new DataTable();
        try
        {
            dt = Helper.GetDataTable(SELECT);
            if (dt.Rows.Count == 0) return false;
            DocRange = dt.Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;

    }
    [WebMethod(Description = "Insert or Replace attribute of /Emr[1] in Emrdocument")]

    //产后病程记录分组续打
    public bool IsRemberPoint2(string RegistryID, ref string DocRange)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT Range FROM TB_RemberGroup2 where RegistryID='" + RegistryID + "'";
        DataTable dt = new DataTable();
        try
        {
            dt = Helper.GetDataTable(SELECT);
            if (dt.Rows.Count == 0) return false;
            DocRange = dt.Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;

    }
    [WebMethod]
    public bool IsRemberPoint(string RegistryID, ref string DocRange)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT Range FROM TB_RemberRange where RegistryID='" + RegistryID + "'";
        DataTable dt = new DataTable();
        try
        {
            dt = Helper.GetDataTable(SELECT);
            if (dt.Rows.Count == 0) return false;
            DocRange = dt.Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;

    }
    //[WebMethod]
    //public bool IsRemberPoint(string RegistryID, ref string DocRange,ref string isSingle)
    //{
    //    SqlHelper Helper = new SqlHelper("EmrDB");
    //    string SELECT = "SELECT Range,IsSingle FROM TB_RemberRange where RegistryID='" + RegistryID + "'";
    //    DataTable dt = new DataTable();
    //    try
    //    {
    //        dt = Helper.GetDataTable(SELECT);
    //        if (dt.Rows.Count == 0) return false;
    //        DocRange = dt.Rows[0][0].ToString();
    //        isSingle = dt.Rows[0][1].ToString();
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //    return true;

    //}
    [WebMethod]
    public void PutRange(string RegistryID, string DocRange)
    {
        DocRange.Replace('$', '\v');
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Insert = @"Insert into TB_RemberRange (RegistryID,Range) Values('" + RegistryID + "','" + DocRange.Replace("'", "''") + "')";

        DeletRange(RegistryID);
        try
        {
            Helper.ExecuteNonQuery(Insert);
        }
        catch (Exception ex)
        {
            return;
        }
    }

    //LiuQ 2011-12-21 术后续打分组
    [WebMethod]
    public void PutRangex1(string RegistryID, string DocRange)
    {
        DocRange.Replace('$', '\v');
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Insert = @"Insert into TB_RemberGroup (RegistryID,Range) Values('" + RegistryID + "','" + DocRange.Replace("'", "''") + "')";

        DeletRangex1(RegistryID);
        try
        {
            Helper.ExecuteNonQuery(Insert);
        }
        catch (Exception ex)
        {
            return;
        }
    }
    //LiuQ 2011-12-21 产后续打分组
    [WebMethod]
    public void PutRangex2(string RegistryID, string DocRange)
    {
        DocRange.Replace('$', '\v');
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Insert = @"Insert into TB_RemberGroup2 (RegistryID,Range) Values('" + RegistryID + "','" + DocRange.Replace("'", "''") + "')";

        DeletRangex2(RegistryID);
        try
        {
            Helper.ExecuteNonQuery(Insert);
        }
        catch (Exception ex)
        {
            return;
        }
    }
    [WebMethod]
    public void PutRangeEx(string RegistryID, XmlNode rule)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string DocRange = rule.Attributes[AttributeNames.Mark].Value;
        string Insert = @"Insert into TB_RemberRange (RegistryID,Range) Values('" + RegistryID + "','" + DocRange + "')";

        DeletRange(RegistryID);
        try
        {
            Helper.ExecuteNonQuery(Insert);
        }
        catch (Exception ex)
        {
            return;
        }
    }

    [WebMethod]
    public void DeletRange(string RegistryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string DEL = "DELETE FROM TB_RemberRange WHERE RegistryID ='" + RegistryID + "'";
        try
        {
            Helper.ExecuteNonQuery(DEL);
        }
        catch
        {
            return;
        }
    }
    [WebMethod]
    public void DeletRangex1(string RegistryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string DEL = "DELETE FROM TB_RemberGroup WHERE RegistryID ='" + RegistryID + "'";
        try
        {
            Helper.ExecuteNonQuery(DEL);
        }
        catch
        {
            return;
        }
    }
    [WebMethod]
    public void DeletRangex2(string RegistryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string DEL = "DELETE FROM TB_RemberGroup2 WHERE RegistryID ='" + RegistryID + "'";
        try
        {
            Helper.ExecuteNonQuery(DEL);
        }
        catch
        {
            return;
        }
    }
    [WebMethod]
    public DataSet GetNotetemp()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = "SELECT doctorID,Note FROM  NoteTemplate";
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
    public DataSet GetPatternType()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = @"SELECT distinct [TypeID] ,[TypeName]   FROM  [TB_PatternType]";
        DataSet ds = new DataSet();
        try
        {
            ds = Helper.GetDataSet(SELECT);
        }
        catch (Exception ex)
        {
            ds = null;
        }
        return ds;
    }
    [WebMethod]
    public DataSet GetPatternItemsByType(string TypeID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = @"SELECT ItemID,ItemName   FROM  [TB_PatternType] Where TypeID = '" + TypeID + "'";
        DataSet ds = new DataSet();
        try
        {
            ds = Helper.GetDataSet(SELECT);
        }
        catch (Exception ex)
        {
            ds = null;
        }
        return ds;
    }

    [WebMethod]
    public DataSet GetPatternTypes()
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = @"SELECT ID,Type   FROM  [TB_TypePattern]";
        DataSet ds = new DataSet();
        try
        {
            ds = Helper.GetDataSet(SELECT);
        }
        catch (Exception ex)
        {
            ds = null;
        }
        return ds;
    }


    [WebMethod]
    public bool UpdatePatternType(string ItemID, string ItemName, string TypeID, string TypeName, string DepartmentID, string DepartmentName)
    {
        bool bl = false;
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = @"SELECT ItemID,ItemName   FROM  [TB_PatternType] Where ItemID = '" + ItemID + "'";
        string Insert = @"Insert into TB_PatternType (TypeID,TypeName,ItemID,ItemName,DepartmentCode,DepartmentName) Values('" + TypeID + "','" + TypeName + "','" + ItemID + "','" + ItemName + "','" + DepartmentID + "','" + DepartmentName + "')";
        string Update = @"Update TB_PatternType set TypeID = '" + TypeID + "',TypeName = '" + TypeName + "',ItemName = '" + ItemName + "',DepartmentCode = '" + DepartmentID + "',DepartmentName = '" + DepartmentName + "'   Where ItemID = '" + ItemID + "'";
        DataSet ds = new DataSet();
        try
        {
            DataTable dt = Helper.GetDataTable(SELECT);
            if (dt != null && dt.Rows.Count != 0)
            {
                Helper.ExecuteNonQuery(Update);
            }
            else
            {
                Helper.ExecuteNonQuery(Insert);
            }
            bl = true;
        }
        catch (Exception ex)
        {

            ds = null;
        }
        return bl;

    }

    [WebMethod]
    public DataSet GetAllNoteNames()
    {
        DataSet ds = new DataSet();

        SqlHelper helper = new SqlHelper("EmrDB");
        try
        {
            int i = 1;
            while (i < 500)
            {
                string strSelect = @"select  Pattern.value('(/EmrPattern/EmrNote/@NoteName)[" + i.ToString() + "]','varchar(200)') as 'NoteName',Pattern.value('(/EmrPattern/EmrNote/@NoteID)[" + i.ToString() + "]','varchar(200)') as 'NoteID'from EmrPattern  where DepartmentCode = '####'";

                DataTable dt = new DataTable();

                dt = helper.GetDataTable(strSelect);
                dt.TableName = "TableName" + i.ToString();
                if (dt != null && dt.Rows.Count != 0)
                {
                    ds.Tables.Add(dt.Copy());
                }

                i++;
            }
            return ds;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    [WebMethod]
    public void SetGrade(string registryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string up = "update emrdocument set document.modify('insert (  attribute Grading {\"Yes\" }  ) into (/Emr)[1]') where RegistryID='" + registryID + "'";
        //DataTable dt = new DataTable();
        try
        {
            bool res = Helper.ExecuteSql(up);

        }
        catch (Exception ex)
        {
            /* Error ocuured */
            return;
        }
        return;
    }
    [WebMethod]
    public int IsValuate(string registryID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        SqlCommand command = new SqlCommand("IsValuate", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(
           "@id", SqlDbType.VarChar);
        command.Parameters["@id"].Value = registryID;
        command.Parameters.Add(
           "@flag", SqlDbType.Int);
        command.Parameters["@flag"].Direction = ParameterDirection.Output;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        int tf = Convert.ToInt16(command.Parameters["@flag"].Value);

        reader.Close();
        connection.Close();

        return tf;


    }
    [WebMethod(Description = "Get notes with FilingSetup", EnableSession = false)]
    public string GetNotesWithFilingSetup(string RegistryID, ref DataSet rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT NoteID FROM TB_FilingSetup WHERE RegistryID = '" + RegistryID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet rules1 = new DataSet();
            adapter.Fill(rules1);
            rules = rules1.Copy();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Remove a note with FilingSetup", EnableSession = false)]
    public string RemoveNoteWithFilingSetup(string RegistryID, string NoteID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "DELETE FROM TB_FilingSetup WHERE RegistryID = @RegistryID and NoteID = @NoteID";
            command.Parameters.Add("@RegistryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@NoteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = RegistryID;
            command.Parameters[1].Value = NoteID;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "get FilingSetup from Department", EnableSession = false)]
    public string GetFilingSetupfromDepartment(string RegistryID, string DepartmentID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "insert into TB_FilingSetup (RegistryID,noteid,Rules,userid)    select @RegistryID,noteid,Rules,userid from TB_FilingSetup , (select count(1) as sl from TB_FilingSetup where RegistryID = @RegistryID ) as bb  where RegistryID = @DepartmentID and bb.sl = 0 ";
            command.Parameters.Add("@RegistryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@DepartmentID", SqlDbType.VarChar, 4);
            command.Parameters[0].Value = RegistryID;
            command.Parameters[1].Value = DepartmentID;
            command.ExecuteNonQuery();
            connection.Close();
            return null;
        }
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "New FilingSetup for a note", EnableSession = false)]
    public string NewFilingSetup(string RegistryID, string noteID, XmlNode rules, string operatorID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.CommandText = "DELETE FROM TB_FilingSetup WHERE RegistryID = @RegistryID and NoteID = @NoteID";
            command.Parameters.Add("@RegistryID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@NoteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@rules", SqlDbType.Xml);
            command.Parameters.Add("@operatorID", SqlDbType.VarChar, 4);
            command.Parameters[0].Value = RegistryID;
            command.Parameters[1].Value = noteID;
            command.Parameters[2].Value = rules.OuterXml;
            command.Parameters[3].Value = operatorID;
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO TB_FilingSetup(RegistryID,noteid,Rules,userid) VALUES(@RegistryID,@noteID, @rules,@operatorID)";
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }


    [WebMethod(Description = "Get NoteStatus with Archive", EnableSession = false)]
    public DataSet GetNoteStatusWithArchive(string RegistryID)
    {
        SqlHelper sq = new SqlHelper("EmrDB");
        try
        {
            //command.CommandText 
            //刘伟补充原程序的判断条件，加入审核中和终审中这两个状态
            string s = "SELECT  Document.query('/Emr/EmrNote[@NoteStatus=0 or @NoteStatus=2 or @NoteStatus = 4]') as note FROM EmrDocument WHERE RegistryID = '" + RegistryID + "'";
            DataSet rules1 = new DataSet();
            rules1 = sq.GetDataSet(s);
            return rules1;

        } //try end
        catch (Exception ex)
        {
            return null;
        }
    }
    //SELECT  RegistryID,Document.query('/Emr/EmrNote[@NoteID="01"]') as note FROM EmrDocument  where RegistryID = '00042333'
    [WebMethod(Description = "Get NoteID with Archive", EnableSession = false)]
    public string GetNoteIDWithArchive(string RegistryID, string NoteID, ref DataSet rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT  Document.query('/Emr/EmrNote[@NoteID=" + "\"" + NoteID + "\"" + "]') as note FROM EmrDocument WHERE RegistryID = '" + RegistryID + "'";
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet rules1 = new DataSet();
            adapter.Fill(rules1);
            rules = rules1.Copy();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get roles for one operator.")]
    public bool GetRolesPermission(string opcode, string ID)
    {
        string select = "SELECT RoleID FROM AsRole WHERE opcode='" + opcode + "' and RoleID='" + ID + "'";
        SqlHelper sq = new SqlHelper("EmrDB");
        try
        {
            DataSet dst = new DataSet();
            dst = sq.GetDataSet(select);
            if (dst.Tables[0].Rows.Count == 0) return false;
            else return true;
        }
        catch (Exception ex)
        {
            //ErrorLog("SetTrafficLightToRed", "TrafficLight", ex.Message, ex.Source);
            return false;
        }
    }
    [WebMethod]
    public string GetEmrDocumentStatus(string strRegistryID)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select status from emrdocument where registryid = '" + strRegistryID + "'";
        DataTable dt = new DataTable();
        string strResult = "";
        try
        {
            dt = oHelper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            {
                strResult = dt.Rows[0]["status"].ToString();
            }

        }
        catch (Exception ex)
        {

        }
        return strResult;

    }
    //静海添加 病案归档不能返修病历 2013-06-13
    [WebMethod]
    public string GetArchiveStatus(string archiveNum)
    {
        SqlHelper oHelper = new SqlHelper("HisDB");
        string strSelect = "SELECT zyh,ch,zyrq,zrys,ksbm,cyrq,emrstatus FROM tdjk WHERE bah = '" + archiveNum + "'";

        DataSet dataSet1 = oHelper.GetDataSet(strSelect); 
        string emrStatus = "";
        if (dataSet1 != null && dataSet1.Tables.Count != 0)
        {
            foreach (DataRow row1 in dataSet1.Tables[0].Rows)
            {
                if (row1.ItemArray[6].ToString() == "1")
                {
                    emrStatus = row1.ItemArray[6].ToString();
                }
            }
        }
        return emrStatus;

    }
    // 职工添加 病案归档不能返修病历 2013-06-13
    [WebMethod]
    public string GetArchiveStatusByZyh(string zyh)
    {
        SqlHelper oHelper = new SqlHelper("HisDB");
        string strSelect = "SELECT zyh,ch,zyrq,zrys,ksbm,cyrq,emrstatus FROM tdjk WHERE zyh = '" + zyh + "'";

        DataSet dataSet1 = oHelper.GetDataSet(strSelect);
        string emrStatus = "";
        if (dataSet1 != null && dataSet1.Tables.Count != 0)
        {
            foreach (DataRow row1 in dataSet1.Tables[0].Rows)
            {
                if (row1.ItemArray[6].ToString() == "1")
                {
                    emrStatus = row1.ItemArray[6].ToString();
                }
            }
        }
        return emrStatus;

    }

    [WebMethod]
    public string GetBaBs(string strRegistryID)
    {
        SqlHelper oHelper = new SqlHelper("HisDB");
        string strSelect = "select babs from tdjk where zyh = '" + strRegistryID + "' And babs='1'"
                           +" UNION ALL select babs from tdjkz where zyh = '" + strRegistryID + "' And babs='1'";
       
        DataTable dt = new DataTable();
        string strResult = "";
        try
        {
            dt = oHelper.GetDataTable(strSelect);
            if (dt != null && dt.Rows.Count != 0)
            
{
                strResult = dt.Rows[0]["babs"].ToString();
               
            }

        }
        catch (Exception ex)
        {

        }
        return strResult;

    }

    [WebMethod(Description = "Get Sign with Archive", EnableSession = false)]
    public string GetSignWithArchive(string RegistryID, ref DataSet ds)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SELECT = @"select   Document.value('(Emr/@Series)[1]','int') as Series from EmrDocument Where RegistryID = '" + RegistryID + "'";

        try
        {
            DataTable dt = Helper.GetDataTable(SELECT);
            if (dt != null && dt.Rows.Count != 0)
            {
                int i = 1;
                while (i < int.Parse(dt.Rows[0][0].ToString()))
                {

                    string strSelect = @"select  '二级' as jb ,Document.value('(Emr/EmrNote/@NoteID)[" + i.ToString() + "]','varchar(50)') as noteID,Document.value('(Emr/EmrNote/@NoteName)[" + i.ToString() + "]','varchar(50)') as noteName from EmrDocument  where Document.value('(Emr/EmrNote/@CheckerID)[" + i.ToString() + "]','varchar(50)') =''and  Document.value('(Emr/EmrNote/@Sign2)[" + i.ToString() + "]','varchar(50)') is not null and RegistryID = '" + RegistryID + "'";

                    DataTable dt1 = new DataTable();

                    dt1 = Helper.GetDataTable(strSelect);
                    dt1.TableName = "TableName" + i.ToString();
                    if (dt1 != null && dt1.Rows.Count != 0)
                    {
                        ds.Tables.Add(dt1.Copy());
                    }
                    else
                    {
                        return "无2";
                    }
                    i++;
                }

            }
            else
            {
                return "无1";
            }
            return null;
        }
        catch (Exception ex)
        {
            ds = null;
            return ex.Message;
        }
    }
    [WebMethod(Description = "New valuate rules for a note", EnableSession = false)]
    public string NewValuateRulesEx(string noteID, XmlNode rules, string NoteName, bool End)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            if (!End) command.CommandText = "DELETE FROM ValuateRules WHERE noteID = @noteID";
            else command.CommandText = "DELETE FROM ValuateRulesEnd WHERE noteID = @noteID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@rules", SqlDbType.Xml);
            command.Parameters.Add("@NoteName", SqlDbType.VarChar, 50);
            command.Parameters[0].Value = noteID;
            command.Parameters[1].Value = rules.OuterXml;
            command.Parameters[2].Value = NoteName;
            command.ExecuteNonQuery();

            if (!End) command.CommandText = "INSERT INTO ValuateRules VALUES(@noteID, @rules,@NoteName)";
            else command.CommandText = "INSERT INTO ValuateRulesEnd VALUES(@noteID, @rules,@NoteName)";
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod]
    public DataSet GetTransferInfoEx(string registryID, string WriterID)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select NoteName as '病历名称',NoteCount as '张数'from TB_TransferInfo";
        DataSet dt = new DataSet();
        try
        {
            strSelect += " where registryid = '" + registryID + "'and WriterID= '" + WriterID + "' and Finall = 'false'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    [WebMethod]
    public DataSet GetTransferInfoExEx(string registryID, string WriterID, bool Finall)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select NoteName as '病历名称',NoteCount as '张数'from TB_TransferInfo";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where registryid = '" + registryID + "' and Finall= '" + Finall + "'";
            if (WriterID != StringGeneral.NullCode) strSelect += " and WriterID= '" + WriterID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    [WebMethod]
    public DataSet GetTransferInfo(string ID, string IDType, string criteria, bool Finall)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select NoteName as '病历名称',NoteCount as '张数',RegistryID as '住院号',InformerID as '提交人',"
        + "ReceiverID as '接收人',ReceiverTime  as '接收时间',WriterName as '填写人' from TB_TransferInfo";
        DataSet dt = new DataSet();
        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
        switch (IDType)
        {
            case "1":
                strSelect += " where registryid like  '%" + ID + "'";
                break;
            case "2":
                strSelect += " where InformerID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;
            case "3":
                strSelect += " where ReceiverID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;
            case "4":
                strSelect += " where WriterID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;

        }
        strSelect = strSelect + " and Finall= '" + Finall + "'";
        try
        {
            dt = oHelper.GetDataSet(strSelect);


        }
        catch (Exception)
        {
            return null;
        }
        return dt;

    }
     [WebMethod]
    public DataSet GetTransferInfoDg(string ID, string IDType, string criteria, bool Finall)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select NoteName as '病历名称',NoteCount as '张数',RegistryID as '住院号',InformerID as '提交人',"
        + "ReceiverID as '接收人',ReceiverTime  as '接收时间',WriterName as '填写人',delayTime as '延迟时间' from TB_TransferInfo";
        DataSet dt = new DataSet();
        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
        switch (IDType)
        {
            case "1":
                strSelect += " where registryid like  '%" + ID + "'";
                break;
            case "2":
                strSelect += " where InformerID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;
            case "3":
                strSelect += " where ReceiverID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;
            case "4":
                strSelect += " where WriterID = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;

        }
        strSelect = strSelect + " and Finall= '" + Finall + "'";
        try
        {
            dt = oHelper.GetDataSet(strSelect);


        }
        catch (Exception)
        {
            return null;
        }
        return dt;

    }
    [WebMethod]
    public bool PutTransferInfo(string RegistryID, string noteName, string WriterID, string WriterName, string ReceiverID, string InformerID, DateTime ReceiverTime, int Counts, bool Finall)
    {
        string Query = "INSERT into TB_TransferInfo ( RegistryID,NoteName,WriterID,WriterName ,ReceiverID, InformerID,  ReceiverTime,NoteCount, Finall) VALUES( '" +
            RegistryID + "','" + noteName + "','" + WriterID + "','" + WriterName + "','" + ReceiverID + "','" + InformerID + "','" + ReceiverTime + "','" + Counts + "','" + Finall + "')";
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            Helper.ExecuteNonQuery(Query);


            return true;
        }
        catch (Exception e)
        {
            ErrorLog("PutTransferInfo", "TB_TransferInfo", SqlOperations.Insert, e.Message);
            return false;
        }
    }
    [WebMethod]
    public bool PutTransferInfoDg(string RegistryID, string noteName, string WriterID, string WriterName, string ReceiverID, string InformerID, DateTime ReceiverTime, int Counts, bool Finall,string delayTime)
    {
        string Query = "INSERT into TB_TransferInfo ( RegistryID,NoteName,WriterID,WriterName ,ReceiverID, InformerID,  ReceiverTime,NoteCount, Finall,DelayTime) VALUES( '" +
            RegistryID + "','" + noteName + "','" + WriterID + "','" + WriterName + "','" + ReceiverID + "','" + InformerID + "','" + ReceiverTime + "','" + Counts + "','" + Finall + "','" + delayTime + "')";
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            Helper.ExecuteNonQuery(Query);


            return true;
        }
        catch (Exception e)
        {
            ErrorLog("PutTransferInfo", "TB_TransferInfo", SqlOperations.Insert, e.Message);
            return false;
        }
    }
    [WebMethod]
    public int UpdateTransferInfoDg(string RegistryID, string noteName, string WriterName, string ReceiverID, string InformerID, DateTime ReceiverTime, string DelayTime, int Counts)
    {
        string Query = "UPDATE TB_TransferInfo SET NoteName='" + noteName + "', Finall='true' ,ReceiverID='" + ReceiverID
            + "', InformerID='" + InformerID + "', ReceiverTime='" + ReceiverTime + "', NoteCount='" + Counts + " ', DelayTime='" + DelayTime + "' "
            + " where registryid = '" + RegistryID + "'and WriterName= '" + WriterName + "'and NoteName= '" + noteName + "' and Finall = 'false'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            int n = Helper.ExecuteNonQuery(Query);


            return n;
        }
        catch (Exception e)
        {
            ErrorLog("PutTransferInfo", "TB_TransferInfo", SqlOperations.Insert, e.Message);
            return 0;
        }
    }
    [WebMethod]
    public int UpdateTransferInfo(string RegistryID, string noteName, string WriterName, string ReceiverID, string InformerID, DateTime ReceiverTime, int Counts)
    {
        string Query = "UPDATE TB_TransferInfo SET NoteName='" + noteName + "', Finall='true' ,ReceiverID='" + ReceiverID
            + "', InformerID='" + InformerID + "', ReceiverTime='" + ReceiverTime + "', NoteCount='" + Counts
            + "' where registryid = '" + RegistryID + "'and WriterName= '" + WriterName + "'and NoteName= '" + noteName + "' and Finall = 'false'";

        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            int n = Helper.ExecuteNonQuery(Query);


            return n;
        }
        catch (Exception e)
        {
            ErrorLog("PutTransferInfo", "TB_TransferInfo", SqlOperations.Insert, e.Message);
            return 0;
        }
    }

    [WebMethod]
    public bool DelTransferInfo(string RegistryID, string DoctorID)
    {
        string Query = "delete from TB_TransferInfo where RegistryID='" + RegistryID + "' and WriterID='" + DoctorID + "' and Finall='false'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            Helper.ExecuteNonQuery(Query);


            return true;
        }
        catch (Exception e)
        {
            ErrorLog("PutTransferInfo", "TB_TransferInfo", SqlOperations.Insert, e.Message);
            return false;
        }
    }
    [WebMethod]
    public bool NewOperationLog(string RegistryID, int Series, string DoctorID, DateTime OpTime, int OpType, string NoteID)
    {
        string Query = "INSERT into EMR_OperationLog ( RegistryID,NoteIDSeries ,DoctorID, OperationTime, OperationType,NoteID) VALUES( '" +
           RegistryID + "','" + Series + "','" + DoctorID + "','" + OpTime + "','" + OpType + "','" + NoteID + "')";
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            Helper.ExecuteNonQuery(Query);


            return true;
        }
        catch (Exception e)
        {
            ErrorLog("PutOperationLog", "EMR_OperationLog", SqlOperations.Insert, e.Message);
            return false;
        }
    }

    /*
     * 以科室编码为查询条件返回有多少待审核的内容。
     */
    [WebMethod]
    public int RemindToReview(string strKsbm)
    {
        int iResult = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        SqlHelper Helper = new SqlHelper("HisDB");
        string strSql = @"SELECT a.zyh
                        FROM tdjkz a LEFT JOIN mzdm b ON a.mz=b.mzdm 
                        LEFT JOIN hfbm c ON a.hf=c.hfbm 
                        LEFT JOIN jgbm d ON a.jg=d.jgbm 
                        WHERE a.ksbm =  '" + strKsbm + @"' 
                        and a.bah is not null 
                        and a.zyrq is not null 
                        and  (a.bqbm is not null or a.bqbm <>'') ";
        try
        {
            DataSet ds = Helper.GetDataSet(strSql);
            if (ds == null || ds.Tables == null || ds.Tables[0] == null)
            {
                return 0;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                //该科室没有任何人员
                return 0;
            }
            sb.Append("'" + Convert.ToString(ds.Tables[0].Rows[0]["zyh"]) + "'");
            for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append(",'" + Convert.ToString(ds.Tables[0].Rows[i]["zyh"]) + "'");
            }
            strSql = @"
                    ;with a as
                    (
                    select  document.query('/Emr/EmrNote[ " + getRemindToReviewSql() + @"]').value('count(EmrNote)','int') as cou
                    from emr_wq.dbo.emrdocument
                    where registryid in
                    (
                       " + sb.ToString() + @"
                    )
                    )
                    select  count(*) as renShu,sum(cou) as BiaoShu from a";
            Helper = new SqlHelper("EmrDB");
            ds = Helper.GetDataSet(strSql);
            if (ds == null || ds.Tables == null || ds.Tables[0] == null)
            {
                return 0;
            }
            iResult = Convert.ToInt32(ds.Tables[0].Rows[0]["BiaoShu"]);
        }
        catch (Exception ex)
        {
            ErrorLog("PutOperationLog", "EMR_OperationLog", SqlOperations.Insert, ex.Message);
            return 0;
        }

        return iResult;


    }
    private string _strRemindToReview = null;
    public string getRemindToReviewSql()
    {
        if (_strRemindToReview == null)
        {
            _strRemindToReview = ConfigClass.GetConfigString("appSettings", "RemindToReview");
        }
        return _strRemindToReview;
    }
    /***********************************信息反馈 20110919*******************************************************/
    [WebMethod]
    public string InfoEmrNoteEx(string registryID, string Opcode,
                    string departmentCode, string noteID, int series, string doctorcode, XmlNode info, string ArchiveNum, EmrConstant.NoteStatus status)
    {

        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strInsert = @"Insert TD_RtInformation (RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation,ArchiveNum,Opdate,status) 
            Values('" + registryID + "','" + Opcode + "','" + departmentCode + "','" + noteID + "'," + series + ",'" + doctorcode + "','" + info.OuterXml.ToString() + "','" + ArchiveNum + "','" + DateTime.Now + "',0);";
            strInsert += @" Insert TD2_RtInformation (RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation,ArchiveNum,Opdate,status) 
            Values('" + registryID + "','" + Opcode + "','" + departmentCode + "','" + noteID + "'," + series + ",'" + doctorcode + "','" + info.OuterXml.ToString() + "','" + ArchiveNum + "','" + DateTime.Now + "',0)";

            string strUpdate = @"Update EmrDocument set document.modify
                        ('replace value of (/Emr/EmrNote[@Series=" + series + "]/@NoteStatus)[1] with " + Convert.ToInt32(status) + "')  where RegistryID='" + registryID + "'";

            string[] strList = { strInsert, strUpdate };
            if (Helper.ExecuteSqlByTran(strList) == true)
            {
                return null;
            }
            else
            {
                return "Error2";
            }

        }
        catch (Exception ex)
        {
            return ex.Message + "-" + ex.Source;
        }
    }

    [WebMethod]
    public DataSet GetInfoBack(int status, string start, string end)
    {
        try
        {
            DataSet dt = null;
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strQuery = "";
            if (status == 1)
            {
                strQuery = "Select RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation," +
              "ArchiveNum,Opdate,status From [TD2_RtInformation] Where status=1 and Opdate Between '" + start + "' and '" + end + "' order by Opdate desc";

            }
            else
            {
                strQuery = "Select RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation," +
               "ArchiveNum,Opdate,status From [TD_RtInformation] Where status=0 and Opdate Between '" + start + "' and '" + end + "' order by Opdate desc";
            }
            dt = Helper.GetDataSet(strQuery);
            return dt;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
    [WebMethod]
    public DataSet GetInfoBackByDoctor(string doctor)
    {
        try
        {
            DataSet dt = null;
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strQuery = "Select RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation," +
            "ArchiveNum,Opdate,status From [TD_RtInformation] Where status=0 and DoctorCode='" + doctor + "' order by Opdate desc";

            dt = Helper.GetDataSet(strQuery);
            return dt;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    [WebMethod]
    public DataSet GetInfoBackByDoctorReg(string doctor, string registryID)
    {
        try
        {
            DataSet ds = null;
            SqlHelper helper = new SqlHelper("EmrDB");
            string strQuery = "Select RegistryID,Opcode,DeparmentCode,NoteID,Series,DoctorCode,RtInformation," +
                "ArchiveNum,Opdate,status From [TD_RtInformation] Where status=0 and DoctorCode='" + doctor + "' and RegistryID='" + registryID + "' Order by Opdate desc";

            ds = helper.GetDataSet(strQuery);
            return ds;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    [WebMethod]
    public bool UpdateInfoBackByDoctorReg(string doctor, string registryID, int series)
    {
        try
        {
            SqlHelper Helper = new SqlHelper("EmrDB");
            string strUpdate = @"Update TD2_RtInformation set Status=1 Where DoctorCode='" + doctor + "'and series=" + series + " and RegistryID='" + registryID + "'";
            strUpdate += "Delete From TD_RtInformation Where DoctorCode='" + doctor + "'and series=" + series + " and RegistryID='" + registryID + "'";
            int count = Helper.ExecuteNonQuery(strUpdate);
            if (count > 0)
                return true;
            else return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    /*******************************************临床路径***********************************/
    [WebMethod]
    public DataSet DownLoadWord(string zyh)
    {
        try
        {
            SqlHelper Helper = new SqlHelper("ClinicPath");
            string strSql = @"Select doc,doc_name,jdxh from lclj_ssjd "
                          + "  where (wcrq is not null and wcrq!='1900-01-01 00:00:00.000') and (emrrq is null or emrrq='1900-01-01 00:00:00.000') and zyh='" + zyh + "'";


            DataSet ds = Helper.GetDataSet(strSql);

            return ds;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    [WebMethod]
    public bool Updatelclj(string zyh, string[] jdxh, DateTime datetime)
    {
        bool flag = false; int count = 0;
        try
        {
            for (int i = 0; i < jdxh.Length; i++)
            {
                SqlHelper Helper = new SqlHelper("ClinicPath");
                string strSql1 = @"Select doc,doc_name,jdxh from lclj_ssjd "
                          + "  where (wcrq is not null and wcrq!='1900-01-01 00:00:00.000') and (emrrq is null or emrrq='1900-01-01 00:00:00.000') and jdxh='" + jdxh[i] + "' and zyh='" + zyh + "'";


                DataSet ds = Helper.GetDataSet(strSql1);

                if (ds.Tables[0].Rows.Count == 0)
                    continue;


                string strSql = @"Update lclj_ssjd set emrrq='" + datetime + "' Where zyh='" + zyh + "' and jdxh='" + jdxh[i] + "' and (wcrq is not null and wcrq!='1900-01-01 00:00:00.000')";
                count = Helper.ExecuteNonQuery(strSql);
                if (count > 0)
                    flag = true;
                else
                    flag = false;
            }
            return flag;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    [WebMethod]
    public int GetlcCount(string zyh)
    {
        int count = 0;
        try
        {

            SqlHelper Helper = new SqlHelper("ClinicPath");
            string strSql = @"Select count(zyh) from lclj_ssjd where" +
" (emrrq is not null and emrrq!='1900-01-01 00:00:00.000') and wcrq is not null and (wcrq!='1900-01-01 00:00:00.000') and zyh='" + zyh + "'";
            count = (int)Helper.ExecuteScalar(strSql);
            return count;


        }
        catch (Exception ex)
        {
            throw;
        }
    }

    //判断是否有了更新，如果有了更新，重新加载临床路径表单
    [WebMethod]
    public DataSet ReDownLoadWord(string zyh)
    {
        try
        {
            SqlHelper Helper = new SqlHelper("ClinicPath");
            string strSql = @"Select doc,doc_name,jdxh from lclj_ssjd "
                          + "  where (wcrq is not null and wcrq!='1900-01-01 00:00:00.000') and zyh='" + zyh + "'";
            DataSet ds = Helper.GetDataSet(strSql);
            return ds;
        }
        catch (Exception ex)
        {
            throw;
        }

    }
    [WebMethod]
    public bool CheckUpdate(string zyh)
    {
        bool flag = false;
        try
        {

            SqlHelper Helper = new SqlHelper("ClinicPath");
            string strSql = "Select zyh from lclj_ss_hzmx where zxdbs = 1 and zyh='" + zyh + "'";
            DataSet ds = Helper.GetDataSet(strSql);
            if (ds != null && ds.Tables[0].Rows.Count != 0)
                flag = true;

        }
        catch (Exception ex)
        {
            throw;
        }
        return flag;
    }
    [WebMethod]
    public bool UpdateLcljHzmx(string zyh)
    {
        bool flag = false;
        try
        {

            SqlHelper Helper = new SqlHelper("ClinicPath");
            string strSql = "Update lclj_ss_hzmx set zxdbs=0 where zxdbs = 1 and zyh='" + zyh + "'";
            int ds = Helper.ExecuteNonQuery(strSql);
            if (ds != 0)
                flag = true;

        }
        catch (Exception ex)
        {
            throw;
        }
        return flag;
    }
    /**lw**/
    [WebMethod]
    public DataSet GetTransferInfoExExEx(string registryID, string WriterID, bool Finall)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select Receivertime as '接收时间' from TB_TransferInfo";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where registryid = '" + registryID + "' and Finall= '" + Finall + "'";
            if (WriterID != StringGeneral.NullCode) strSelect += " and WriterID= '" + WriterID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    //刘伟加入病历作废还原查询
    [WebMethod]
    public DataSet GetInvalidInfo(string ID, string IDType, string criteria)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select RegistryID as '住院号',NoteName as '病历名称',Opcode as '操作员',Writerid as '书写人ID',Writername as '书写人',Invalid as '操作', ReceiverTime  as '操作时间' from TB_Invalid";

        DataSet dt = new DataSet();
        string[] items = criteria.Split(EmrConstant.StringGeneral.Delimiter);
        switch (IDType)
        {
            case "1":
                strSelect += " where registryid like  '%" + ID + "'";//住院号
                break;
            case "2":
                strSelect += " where Opcode = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;
            case "3":
                strSelect += " where Writerid = '" + ID + "' AND ReceiverTime BETWEEN '" + items[0] + "' AND '" + items[1] + "'";
                break;

        }

        try
        {
            dt = oHelper.GetDataSet(strSelect);


        }
        catch (Exception)
        {
            return null;
        }
        return dt;

    }

    //刘伟加入作废
    [WebMethod]
    public bool Setzuofei(string registryID, int series)
    {
        string up = "update emrdocument set document.modify('insert (attribute zuofei {\"Yes\"}) into (/Emr/EmrNote[@Series = " + series + "])[1]') where RegistryID='" + registryID + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool result = false;
        try
        {
            result = Helper.ExecuteSql(up);
        }
        catch
        {
            result = false;
        }
        return result;
    }

    //刘伟加入作废还原
    [WebMethod]
    public bool Setunzuofei(string registryID, int series)
    {
        string up = "update emrdocument set document.modify('delete (/Emr/EmrNote[@Series = " + series + "]/@zuofei)[1]') where RegistryID='" + registryID + "'";
        SqlHelper Helper = new SqlHelper("EmrDB");
        bool result = false;
        try
        {
            result = Helper.ExecuteSql(up);
        }
        catch
        {
            result = false;
        }
        return result;
    }

    [WebMethod]
    public bool lw_invalidInfo(string RegistryID, string noteName, string Opcode, string WriterID, string WriterName, bool Invalid, System.DateTime ReceiverTime)
    {
        string Query = "INSERT into TB_invalid ( Registryid,Notename,Opcode,Writerid,Writername,Invalid,ReceiverTime) VALUES( '" +
            RegistryID + "','" + noteName + "','" + Opcode + "','" + WriterID + "','" + WriterName + "','" + Invalid + "','" + ReceiverTime + "')";
        SqlHelper Helper = new SqlHelper("EmrDB");
        try
        {

            Helper.ExecuteNonQuery(Query);


            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    [WebMethod]
    public DataSet lw_GetTransferInfoExEx(string registryID, string WriterID, bool Finall)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select NoteName as '病历名称',NoteCount as '张数',WriterName as '填写人' from TB_TransferInfo";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where registryid = '" + registryID + "' and Finall= '" + Finall + "'";
            //if (WriterID != StringGeneral.NullCode) strSelect += " and WriterID= '" + WriterID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    //刘伟加入获取临床路径jxdm20111108
    [WebMethod]
    public DataSet obtainjkdm(string noteID)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select jkdm from jkdmcontrast ";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where noteID = '" + noteID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    //刘伟加入临床路径挑勾20111108
    [WebMethod]
    public string ClinicPathmonitor(string registryID, string jkdm, string doctorID)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "ClinicPath"));
        SqlCommand command = new SqlCommand("LCLJ_EMR", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(
           "@zyh", SqlDbType.VarChar);
        command.Parameters["@zyh"].Value = registryID;
        command.Parameters.Add(
           "@jkdm", SqlDbType.VarChar);
        command.Parameters["@jkdm"].Value = jkdm;
        command.Parameters.Add(
           "@ysbm", SqlDbType.VarChar);
        command.Parameters["@ysbm"].Value = doctorID;

        command.Parameters.Add(
           "@msg", SqlDbType.VarChar);
        command.Parameters["@msg"].Size = 100;
        command.Parameters["@msg"].Direction = ParameterDirection.Output;

        connection.Open();

        SqlDataReader reader = command.ExecuteReader();
        reader.Read();
        string tf = Convert.ToString(command.Parameters["@msg"].Value);

        reader.Close();
        connection.Close();

        return tf;

    }
    /*******************************************临床路径***********************************/

    ////刘伟加入获得ValuateDetail 20111125
    [WebMethod(Description = "Get valuate Flaws for a note", EnableSession = false)]
    public string GetValuateDetailEx(string noteID, string RegistryID, ref XmlNode Flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            command.CommandText = "SELECT Flaws FROM ValuateDetail WHERE noteID = @noteID and RegistryID =@RegistryID";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            command.Parameters.Add("@RegistryID", SqlDbType.VarChar, 12);
            command.Parameters[1].Value = RegistryID;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                Flaws = doc.DocumentElement.Clone();
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    //刘伟加入建立电子病历全文索引
    [WebMethod]
    public int IndexSearcher()
    {
        try
        {
            string path = @"d:/index/";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            int fileNum = dir.GetFiles().Length;
            return fileNum;
        }
        catch (Exception e)
        {
            return 0;
        }

    }
    [WebMethod]
    public string GetCreationTime()
    {
        try
        {
            string path = @"d:/index/";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            string CreationTime = Convert.ToString(dir.CreationTime);
            return CreationTime;
        }
        catch (Exception e)
        {
            return null;
        }

    }
    private bool IsNumber(string str)
    {
        try
        {
            Double var = Convert.ToDouble(str);
            return true;
        }
        catch
        {
            return false;
        }
    }
    [WebMethod]
    public void Page_Load()
    {
        string path = @"d:/index/";
        if (File.Exists(path))
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            dir.Delete(true);
        }
        SqlDataReader myred = OpenTable();
        IndexWriter writer = CreateIndex(myred);
    }

    public SqlDataReader OpenTable()
    {
        SqlConnection mycon = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        mycon.Open();
        SqlCommand mycom = new SqlCommand(@"WITH XMLNAMESPACES ('Best' as ns1, 
                    'errors@Best' as ns2,
                    DEFAULT 'Best')
select registryid,
       noteIDseries,
       EmrXML,
	   isnull(
       EmrXML.value('(电子病历/合并/病历名称)[1]', 'nvarchar(max)'),
       EmrXML.value('(电子病历/病历名称)[1]', 'nvarchar(max)'))as blmc, -- 病历名称
	   isnull(
       EmrXML.value('(电子病历/合并/基础信息/性别)[1]', 'nvarchar(max)'),
       EmrXML.value('(电子病历/基础信息/性别)[1]', 'nvarchar(max)'))as xb, -- 性别

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/年龄)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/年龄)[1]', 'nvarchar(max)'))))as nl, -- 年龄

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/民族)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/民族)[1]', 'nvarchar(max)'))))as mz, -- 民族

       isnull((isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/籍贯)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/籍贯)[1]', 'nvarchar(max)'))))),
	   (isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/基础信息/籍贯)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/基础信息/籍贯)[1]', 'nvarchar(max)'))))))as jg, -- 籍贯

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/婚否)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/婚否)[1]', 'nvarchar(max)'))))as hf, -- 婚否

       isnull((isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/职业)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/职业)[1]', 'nvarchar(max)'))))),
	   (isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/合并/基础信息/基础信息/职业)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/基础信息/职业)[1]', 'nvarchar(max)'))))))as zy, -- 职业

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/ns2:病例特点/ns2:流行病学情况)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/ns2:病例特点/ns2:流行病学情况)[1]', 'nvarchar(max)'))))as lxbxqk, -- 首次病程记录/流行病学情况

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/ns2:病例特点/ns2:现病史摘要)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/ns2:病例特点/ns2:现病史摘要)[1]', 'nvarchar(max)'))))as xbszy,--首次病程记录/现病史摘要

	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/ns2:病例特点/ns2:查体)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/ns2:病例特点/ns2:查体)[1]', 'nvarchar(max)'))))as ct,--首次病程记录/查体
	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/ns2:病例特点/ns2:化验及辅助检查)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/ns2:病例特点/ns2:化验及辅助检查)[1]', 'nvarchar(max)'))))as hyjfzjc,--首次病程记录/化验及辅助检查
	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/拟诊讨论)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/拟诊讨论)[1]', 'nvarchar(max)'))))as nztl,--首次病程记录/拟诊讨论
	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/ns2:初步诊断)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/ns2:初步诊断)[1]', 'nvarchar(max)'))))as cbzd,--首次病程记录/初步诊断
	   isnull(
       ltrim(rtrim(EmrXML.value('(电子病历/主因/诊疗计划)[1]', 'nvarchar(max)'))),
       ltrim(rtrim(EmrXML.value('(电子病历/ns2:主因/诊疗计划)[1]', 'nvarchar(max)'))))as zljh,--首次病程记录/诊疗计划
       EmrXML.value('(电子病历/主诉)[1]', 'nvarchar(max)')as zs,--入院记录/主诉
       EmrXML.value('(电子病历/现病史)[1]', 'nvarchar(max)')as xbs,--入院记录/现病史
       EmrXML.value('(电子病历/既往史)[1]', 'nvarchar(max)')as jws,--入院记录/既往史
       EmrXML.value('(电子病历/个人史)[1]', 'nvarchar(max)')as grs,--入院记录/个人史
       EmrXML.value('(电子病历/女)[1]', 'nvarchar(max)')as yjs,--入院记录/月经史
       EmrXML.value('(电子病历/婚育史)[1]', 'nvarchar(max)')as hys,--入院记录/婚育史
       EmrXML.value('(电子病历/家族史)[1]', 'nvarchar(max)')as jzs,--入院记录/家族史
       ltrim(rtrim(EmrXML.value('(电子病历/体格一般检查/体温)[1]', 'nvarchar(max)')))as tw,--入院记录/体温
       ltrim(rtrim(EmrXML.value('(电子病历/体格一般检查/脉搏)[1]', 'nvarchar(max)')))as mb,--入院记录/脉搏
       ltrim(rtrim(EmrXML.value('(电子病历/体格一般检查/呼吸)[1]', 'nvarchar(max)')))as hx,--入院记录/呼吸
       ltrim(rtrim(EmrXML.value('(电子病历/体格一般检查/血压)[1]', 'nvarchar(max)')))as xy,--入院记录/血压
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/身高)[1]', 'nvarchar(max)')))as sg,--麻醉计划/身高
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/体重)[1]', 'nvarchar(max)')))as tz,--麻醉计划/体重
       EmrXML.value('(电子病历/体格一般检查/一般情况)[1]', 'nvarchar(max)')as ybqk,--入院记录/一般情况
       EmrXML.value('(电子病历/体格一般检查/专科情况)[1]', 'nvarchar(max)')as zkqk,--入院记录/专科情况
       EmrXML.value('(电子病历/体格一般检查/辅助检查)[1]', 'nvarchar(max)')as fzjc,--入院记录/辅助检查
       EmrXML.value('(电子病历/体格一般检查/印象检查)[1]', 'nvarchar(max)')as yxjc,--入院记录/印象检查
       EmrXML.value('(电子病历/ns2:病程记录)[1]', 'nvarchar(max)')as bcjl,--病程记录/病程记录
       EmrXML.value('(电子病历/其他/会诊目的)[1]', 'nvarchar(max)')as hzmd,--会诊记录/会诊目的
       ltrim(rtrim(EmrXML.value('(电子病历/基础信息/ns2:住院天数)[1]', 'nvarchar(max)')))as zyts,--出院记录/住院天数
       EmrXML.value('(电子病历/入院情况)[1]', 'nvarchar(max)')as ryqk,--出院记录/入院情况
       EmrXML.value('(电子病历/入院诊断)[1]', 'nvarchar(max)')as ryzd,--出院记录/入院诊断
       EmrXML.value('(电子病历/诊疗经过)[1]', 'nvarchar(max)')as zljg,--出院记录/诊疗经过
       EmrXML.value('(电子病历/出院诊断)[1]', 'nvarchar(max)')as cyzd,--出院记录/出院诊断
       EmrXML.value('(电子病历/出院情况)[1]', 'nvarchar(max)')as cyqk,--出院记录/出院情况
       EmrXML.value('(电子病历/出院医嘱)[1]', 'nvarchar(max)')as cyyz,--出院记录/出院医嘱
       EmrXML.value('(电子病历/病情简述)[1]', 'nvarchar(max)')as bqjs,--病情简述
       EmrXML.value('(电子病历/抢救经过)[1]', 'nvarchar(max)')as qjjg,--抢救经过
       EmrXML.value('(电子病历/目前情况)[1]', 'nvarchar(max)')as mqqk,--目前情况
       EmrXML.value('(电子病历/目前诊断)[1]', 'nvarchar(max)')as mqzd,--目前诊断
       EmrXML.value('(电子病历/讨论目的)[1]', 'nvarchar(max)')as tlmd,--讨论目的
       EmrXML.value('(电子病历/死亡原因)[1]', 'nvarchar(max)')as swyy,--死亡原因
       EmrXML.value('(电子病历/死亡诊断)[1]', 'nvarchar(max)')as swzd,--死亡诊断
       EmrXML.value('(电子病历/转诊目的)[1]', 'nvarchar(max)')as zzmd,--转诊目的
       EmrXML.value('(电子病历/转科时诊断)[1]', 'nvarchar(max)')as zkszd,--转科时诊断
       EmrXML.value('(电子病历/手术指征)[1]', 'nvarchar(max)')as sszz,--手术指征
       EmrXML.value('(电子病历/术前诊断)[1]', 'nvarchar(max)')as sqzd,--术前诊断
       EmrXML.value('(电子病历/术后诊断)[1]', 'nvarchar(max)')as shzd,--术后诊断
       EmrXML.value('(电子病历/手术名称)[1]', 'nvarchar(max)')as ssmc,--手术名称
       EmrXML.value('(电子病历/手术切口)[1]', 'nvarchar(max)')as ssqk,--手术切口
       EmrXML.value('(电子病历/麻醉方式)[1]', 'nvarchar(max)')as mzfs,--麻醉方式
       EmrXML.value('(电子病历/注意事项)[1]', 'nvarchar(max)')as zysx,--注意事项
       EmrXML.value('(电子病历/患者准备)[1]', 'nvarchar(max)')as hzzb,--患者准备
       EmrXML.value('(电子病历/手术风险)[1]', 'nvarchar(max)')as ssfx,--手术风险
       ksbm,
       zyrq 
from emrnote as emrNote
left join
(
	select ksbm,zyrq,zyh from tdjkplus
	union all
	select ksbm,zyrq,zyh from tdjkzplus
) as hospital
on 
emrNote.registryid = hospital.zyh
where emrXML is not null
order by zyrq", mycon);
        mycom.CommandTimeout = 900000000;
        return mycom.ExecuteReader();
    }
    public IndexWriter CreateIndex(SqlDataReader myred)
    {
        IndexWriter writer = new IndexWriter("d:/index/", new PanGuAnalyzer(), true);
        string NoteID = null;
        Hashtable hs = new Hashtable();
        try
        {
            while (myred.Read())
            {
                Document doc = new Document();
                doc.Add(new Field("RegistryID", myred["RegistryID"].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("NoteIDSeries", myred["NoteIDSeries"].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                if (myred["NoteIDSeries"].ToString().Substring(2, 1) == "0")
                    NoteID = myred["NoteIDSeries"].ToString().Substring(0, 2);
                else
                    NoteID = myred["NoteIDSeries"].ToString().Substring(0, 3);
                doc.Add(new Field("NoteID", NoteID, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("EmrXml", myred["RegistryID"].ToString() + myred["EmrXml"].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                //
                doc.Add(new Field("blmc", myred["blmc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("xb", myred["xb"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                if ((myred["nl"].ToString().Trim() != "") && (IsNumber(myred["nl"].ToString().Trim())))
                {
                    doc.Add(new Field("nl", NumberTools.LongToString((long)(Convert.ToDecimal(myred["nl"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                doc.Add(new Field("mz", myred["mz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("jg", myred["jg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("hf", myred["hf"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zy", myred["zy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("lxbxqk", myred["lxbxqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("xbszy", myred["xbszy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ct", myred["ct"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("hyjfzjc", myred["hyjfzjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("nztl", myred["nztl"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("cbzd", myred["cbzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zljh", myred["zljh"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zs", myred["zs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("xbs", myred["xbs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("jws", myred["jws"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("grs", myred["grs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("yjs", myred["yjs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("hys", myred["hys"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("jzs", myred["jzs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                if ((myred["tw"].ToString().Trim() != "") && (IsNumber(myred["tw"].ToString().Trim())))
                {
                    doc.Add(new Field("tw", NumberTools.LongToString((long)(Convert.ToDouble(myred["tw"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if ((myred["mb"].ToString().Trim() != "") && (IsNumber(myred["mb"].ToString().Trim())))
                {
                    doc.Add(new Field("mb", NumberTools.LongToString((long)(Convert.ToDecimal(myred["mb"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if ((myred["hx"].ToString().Trim() != "") && (IsNumber(myred["hx"].ToString().Trim())))
                {
                    doc.Add(new Field("hx", NumberTools.LongToString((long)(Convert.ToDecimal(myred["hx"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                string[] xml1 = myred["xy"].ToString().Trim().Split('/');
                ArrayList xml = new ArrayList();
                for (int k = 0; k < xml1.Length; k++)
                {
                    if (xml1[k].Trim().Length > 0) xml.Add(xml1[k]);
                }
                if ((xml.Count == 2) && (IsNumber(xml[0].ToString().Trim())) && (IsNumber(xml[1].ToString().Trim())))
                {
                    doc.Add(new Field("ssy", NumberTools.LongToString((long)(Convert.ToDecimal(xml[0].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("szy", NumberTools.LongToString((long)(Convert.ToDecimal(xml[1].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                //
                if ((myred["sg"].ToString().Trim() != "") && (IsNumber(myred["sg"].ToString().Trim())))
                {
                    doc.Add(new Field("sg", NumberTools.LongToString((long)(Convert.ToDecimal(myred["sg"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                if ((myred["tz"].ToString().Trim() != "") && (IsNumber(myred["yz"].ToString().Trim())))
                {
                    doc.Add(new Field("tz", NumberTools.LongToString((long)(Convert.ToDecimal(myred["tz"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                doc.Add(new Field("ybqk", myred["ybqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zkqk", myred["zkqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("fzjc", myred["fzjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("yxjc", myred["yxjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("bcjl", myred["bcjl"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("hzmd", myred["hzmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                if ((myred["zyts"].ToString().Trim() != "") && (IsNumber(myred["zyts"].ToString().Trim())))
                {
                    doc.Add(new Field("zyts", NumberTools.LongToString((long)(Convert.ToDecimal(myred["zyts"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }
                doc.Add(new Field("ryqk", myred["ryqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ryzd", myred["ryzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zljg", myred["zljg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("cyzd", myred["cyzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("cyqk", myred["cyqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("cyyz", myred["cyyz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("bqjs", myred["bqjs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("qjjg", myred["qjjg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("mqqk", myred["mqqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("mqzd", myred["mqzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("tlmd", myred["tlmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("swyy", myred["swyy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("swzd", myred["swzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zzmd", myred["zzmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zkszd", myred["zkszd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("sszz", myred["sszz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("sqzd", myred["sqzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("shzd", myred["shzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ssmc", myred["ssmc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ssqk", myred["ssqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("mzfs", myred["mzfs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("zysx", myred["zysx"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("hzzb", myred["hzzb"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("ssfx", myred["ssfx"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                //
                doc.Add(new Field("ksbm", myred["ksbm"].ToString().Trim(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("zyrq", myred["zyrq"].ToString().Trim(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                if (hs.Contains(myred["RegistryID"].ToString()))
                {
                    Document tmp = (Document)hs[myred["RegistryID"].ToString()];
                    string EmrXmltmp = tmp.Get("EmrXml");
                    tmp.RemoveField("EmrXml");
                    tmp.Add(new Field("EmrXml", EmrXmltmp + myred["EmrXml"].ToString(), Field.Store.YES, Field.Index.ANALYZED));

                    //
                    tmp.Add(new Field("blmc", myred["blmc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("xb", myred["xb"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    if ((myred["nl"].ToString().Trim() != "") && (IsNumber(myred["nl"].ToString().Trim())))
                    {
                        tmp.Add(new Field("nl", NumberTools.LongToString((long)(Convert.ToDecimal(myred["nl"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    tmp.Add(new Field("mz", myred["mz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("jg", myred["jg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("hf", myred["hf"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zy", myred["zy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("lxbxqk", myred["lxbxqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("xbszy", myred["xbszy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("ct", myred["ct"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("hyjfzjc", myred["hyjfzjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("nztl", myred["nztl"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("cbzd", myred["cbzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zljh", myred["zljh"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zs", myred["zs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("xbs", myred["xbs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("jws", myred["jws"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("grs", myred["grs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("yjs", myred["yjs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("hys", myred["hys"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("jzs", myred["jzs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    if ((myred["tw"].ToString().Trim() != "") && (IsNumber(myred["tw"].ToString().Trim())))
                    {
                        tmp.Add(new Field("tw", NumberTools.LongToString((long)(Convert.ToDouble(myred["tw"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }

                    if ((myred["mb"].ToString().Trim() != "") && (IsNumber(myred["mb"].ToString().Trim())))
                    {
                        tmp.Add(new Field("mb", NumberTools.LongToString((long)(Convert.ToDecimal(myred["mb"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }

                    if ((myred["hx"].ToString().Trim() != "") && (IsNumber(myred["hx"].ToString().Trim())))
                    {
                        tmp.Add(new Field("hx", NumberTools.LongToString((long)(Convert.ToDecimal(myred["hx"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }

                    string[] xml2 = myred["xy"].ToString().Trim().Split('/');
                    ArrayList xml3 = new ArrayList();
                    for (int k = 0; k < xml1.Length; k++)
                    {
                        if (xml2[k].Trim().Length > 0) xml.Add(xml2[k]);
                    }
                    if ((xml3.Count == 2) && (IsNumber(xml3[0].ToString().Trim())) && (IsNumber(xml3[1].ToString().Trim())))
                    {
                        tmp.Add(new Field("ssy", NumberTools.LongToString((long)(Convert.ToDecimal(xml3[0].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        tmp.Add(new Field("szy", NumberTools.LongToString((long)(Convert.ToDecimal(xml3[1].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    //
                    if ((myred["sg"].ToString().Trim() != "") && (IsNumber(myred["sg"].ToString().Trim())))
                    {
                        tmp.Add(new Field("sg", NumberTools.LongToString((long)(Convert.ToDecimal(myred["sg"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    if ((myred["tz"].ToString().Trim() != "") && (IsNumber(myred["yz"].ToString().Trim())))
                    {
                        tmp.Add(new Field("tz", NumberTools.LongToString((long)(Convert.ToDecimal(myred["tz"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    tmp.Add(new Field("ybqk", myred["ybqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zkqk", myred["zkqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("fzjc", myred["fzjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("yxjc", myred["yxjc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("bcjl", myred["bcjl"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("hzmd", myred["hzmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    if ((myred["zyts"].ToString().Trim() != "") && (IsNumber(myred["zyts"].ToString().Trim())))
                    {
                        tmp.Add(new Field("zyts", NumberTools.LongToString((long)(Convert.ToDecimal(myred["zyts"].ToString().Trim()))), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    tmp.Add(new Field("ryqk", myred["ryqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("ryzd", myred["ryzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zljg", myred["zljg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("cyzd", myred["cyzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("cyqk", myred["cyqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("cyyz", myred["cyyz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("bqjs", myred["bqjs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("qjjg", myred["qjjg"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("mqqk", myred["mqqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("mqzd", myred["mqzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("tlmd", myred["tlmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("swyy", myred["swyy"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("swzd", myred["swzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zzmd", myred["zzmd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zkszd", myred["zkszd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("sszz", myred["sszz"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("sqzd", myred["sqzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("shzd", myred["shzd"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("ssmc", myred["ssmc"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("ssqk", myred["ssqk"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("mzfs", myred["mzfs"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("zysx", myred["zysx"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("hzzb", myred["hzzb"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    tmp.Add(new Field("ssfx", myred["ssfx"].ToString().Trim(), Field.Store.YES, Field.Index.ANALYZED));
                    //

                    hs.Remove(myred["RegistryID"].ToString());
                    hs.Add(myred["RegistryID"].ToString(), tmp);
                }
                else
                {
                    hs.Add(myred["RegistryID"].ToString(), doc);
                }

                //writer.AddDocument(doc);

            }
            foreach (string mykey in hs.Keys)
            {

                Document tmp = (Document)hs[mykey];
                writer.AddDocument(tmp);
            }
            writer.Optimize();
            writer.Close();
        }
        catch (Exception e)
        {

        }
        return writer;
    }
    [WebMethod]
    public DataTable GetDataminingSearch(Int32 StartPos, string strStart, string strEnd, string cboDepartment, string strseach, out double zyhcount, out double tj, out decimal EndPos)
    {
        BooleanQuery.SetMaxClauseCount(100000000);
        Hits hits = null;
        zyhcount = 0;
        tj = 0;
        EndPos = 0;
        string SQLSentence = "";
        string SQLSentenceln = "";
        SqlHelper Helper = new SqlHelper("HisDB");
        if (cboDepartment == "$")
        {
            SQLSentence = "select count(*) from tdjk where zyrq between  '" + strStart + "' and '" + strEnd + "'";
            SQLSentenceln = "select count(*) from tdjkz where zyrq between  '" + strStart + "' and '" + strEnd + "'";
        }
        else
        {
            SQLSentence = "select count(*) from tdjkplus where zyrq between  '" + strStart + "' and '" + strEnd + "' and ksbm='" + cboDepartment + "'";
            SQLSentenceln = "select count(*) from tdjkzplus where zyrq between  '" + strStart + "' and '" + strEnd + "' and ksbm='" + cboDepartment + "'";
        }

        DataTable dtcy = Helper.GetDataTable(SQLSentence);

        DataTable dtzy = Helper.GetDataTable(SQLSentenceln);
        zyhcount = Convert.ToDouble(dtzy.Rows[0][0]) + Convert.ToDouble(dtcy.Rows[0][0]);


        DateTime dtEnd = DateTime.MaxValue;
        DateTime.TryParse(strEnd, out dtEnd);
        DateTime dtStart = DateTime.MinValue;
        DateTime.TryParse(strStart, out dtStart);
        DataTable dt = new DataTable("getqc");
        string[] xml1 = strseach.Split(' ');
        ArrayList xml = new ArrayList();
        for (int k = 0; k < xml1.Length; k++)
        {
            if (xml1[k].Length > 0) xml.Add(xml1[k]);
        }
        try
        {
            IndexSearcher mysea = new IndexSearcher("d:/index/");
            BooleanQuery query = new BooleanQuery();
            BooleanQuery query1 = new BooleanQuery();
            PanGuAnalyzer analyzer = new PanGuAnalyzer();//盘古分词

            String[] fields = { "EmrXml" };
            BooleanClause.Occur[] flags_MUST = new BooleanClause.Occur[] { BooleanClause.Occur.MUST };
            BooleanClause.Occur[] flags_MUST_NOT = new BooleanClause.Occur[] { BooleanClause.Occur.MUST_NOT };
            BooleanClause.Occur[] flags_SHOULD = new BooleanClause.Occur[] { BooleanClause.Occur.SHOULD };
            Query queryex = null;
            Query queryor = null;
            for (int i = 0; i < xml.Count; i++)
            {

                String[] key = new String[] { xml[i].ToString() };//用空格分词

                string[] keyex = xml[i].ToString().Split('|');//或的关系
                if (keyex.Length != 1)
                {
                    for (int j = 0; j < keyex.Length; j++)
                    {
                        String[] key_or = new String[] { keyex[j] };
                        queryor = MultiFieldQueryParser.Parse(key_or, fields, flags_SHOULD, analyzer);
                        query1.Add(queryor, BooleanClause.Occur.SHOULD);
                    }
                    query.Add(query1, BooleanClause.Occur.MUST);
                }
                else
                {
                    if (xml[i].ToString().Substring(0, 1) == "-")//非的关系
                    {
                        queryex = MultiFieldQueryParser.Parse(xml[i].ToString().Substring(1), fields, flags_MUST, analyzer);
                        query.Add(queryex, BooleanClause.Occur.MUST_NOT);
                    }
                    else
                    {
                        queryex = MultiFieldQueryParser.Parse(key, fields, flags_MUST, analyzer);
                        query.Add(queryex, BooleanClause.Occur.MUST);
                    }


                }

            }

            if (cboDepartment != "$")//按科室筛选
            {
                Query queryksbm = new TermQuery(new Term("ksbm", cboDepartment));
                query.Add(queryksbm, BooleanClause.Occur.MUST);
            }

            //Term date1 = new Term("zyrq", strStart.Substring(0, 10) + "*");//按时间筛选
            //Term date2 = new Term("zyrq", strEnd.Substring(0, 10) + "*");
            //Query dateRangeQuery = new RangeQuery(date1, date2, true);
            //query.Add(dateRangeQuery, BooleanClause.Occur.MUST);

            RangeFilter filter = new RangeFilter("zyrq", strStart, strEnd, true, true);
            //

            hits = mysea.Search(query, filter);

            tj = hits.Length();




            DataColumn dcRegistryID = new DataColumn("RegistryID", typeof(string));
            DataColumn dcNoteIDSeries = new DataColumn("NoteIDSeries", typeof(string));
            DataColumn dcNoteID = new DataColumn("NoteID", typeof(string));
            DataColumn dcEmrXml = new DataColumn("EmrXml", typeof(string));
            DataColumn dcKsbm = new DataColumn("ksbm", typeof(string));
            DataColumn dcZyrq = new DataColumn("zyrq", typeof(string));

            dt.Columns.Add(dcRegistryID);
            dt.Columns.Add(dcNoteIDSeries);
            dt.Columns.Add(dcNoteID);
            dt.Columns.Add(dcEmrXml);
            dt.Columns.Add(dcKsbm);
            dt.Columns.Add(dcZyrq);
            DataRow dr = null;


            if (tj != 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    dr = dt.NewRow();
                    dr["RegistryID"] = hits.Doc(StartPos).Get("RegistryID");
                    dr["NoteIDSeries"] = hits.Doc(StartPos).Get("NoteIDSeries");
                    dr["NoteID"] = hits.Doc(StartPos).Get("NoteID");
                    dr["EmrXml"] = hits.Doc(StartPos).Get("EmrXml");
                    dr["ksbm"] = hits.Doc(StartPos).Get("ksbm");
                    dr["zyrq"] = hits.Doc(StartPos).Get("zyrq");
                    StartPos = StartPos + 1;
                    dt.Rows.Add(dr);
                    if (StartPos >= tj) break;
                }
                EndPos = StartPos - 1;
            }
        }

        catch (Exception e)
        {
            throw e;
            //Response.Write(e);

        }
        return dt;

    }
    public static class MyClass
    {
        public static BooleanQuery query = new BooleanQuery();


    }
    private void gethits(string strseach, string EmrXml)
    {
        Hits hits = null;
        string[] xml1 = strseach.Split(' ');
        ArrayList xml = new ArrayList();
        for (int k = 0; k < xml1.Length; k++)
        {
            if (xml1[k].Length > 0) xml.Add(xml1[k]);
        }
        BooleanQuery query = new BooleanQuery();
        BooleanQuery query1 = new BooleanQuery();
        PanGuAnalyzer analyzer = new PanGuAnalyzer();//p盘古分词
        // KTDictSegAnalyzer analyzer = new KTDictSegAnalyzer();
        String[] fields = { EmrXml };
        BooleanClause.Occur[] flags_MUST = new BooleanClause.Occur[] { BooleanClause.Occur.MUST };
        BooleanClause.Occur[] flags_MUST_NOT = new BooleanClause.Occur[] { BooleanClause.Occur.MUST_NOT };
        BooleanClause.Occur[] flags_SHOULD = new BooleanClause.Occur[] { BooleanClause.Occur.SHOULD };
        Query queryex = null;
        Query queryor = null;
        for (int i = 0; i < xml.Count; i++)
        {

            String[] key = new String[] { xml[i].ToString() };//用空格分词

            string[] keyex = xml[i].ToString().Split('|');//或的关系
            if (keyex.Length != 1)
            {
                for (int j = 0; j < keyex.Length; j++)
                {
                    String[] key_or = new String[] { keyex[j] };
                    queryor = MultiFieldQueryParser.Parse(key_or, fields, flags_SHOULD, analyzer);
                    query1.Add(queryor, BooleanClause.Occur.SHOULD);
                }
                query.Add(query1, BooleanClause.Occur.MUST);
            }
            else
            {
                if (xml[i].ToString().Substring(0, 1) == "-")//非的关系
                {
                    queryex = MultiFieldQueryParser.Parse(xml[i].ToString().Substring(1), fields, flags_MUST, analyzer);
                    query.Add(queryex, BooleanClause.Occur.MUST_NOT);
                }
                else
                {
                    queryex = MultiFieldQueryParser.Parse(key, fields, flags_MUST, analyzer);
                    query.Add(queryex, BooleanClause.Occur.MUST);
                }


            }

        }
        MyClass.query.Add(query, BooleanClause.Occur.MUST);
    }
    [WebMethod]//System.Collections.Generic.Dictionary<string, string> dic
    public DataTable GetDataminingSearchEx(Int32 StartPos, string strStart, string strEnd, string cboDepartment, string[] strKeyValuePair, out double zyhcount, out double tj, out decimal EndPos)
    {
        System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();
        for (int i = 0; i < strKeyValuePair.Length; i += 2)
        {
            dic.Add(strKeyValuePair[i], strKeyValuePair[i + 1]);
        }

        BooleanQuery.SetMaxClauseCount(100000000);
        Hits hits = null;
        zyhcount = 0;
        tj = 0;
        EndPos = 0;
        string SQLSentence = "";
        string SQLSentenceln = "";
        MyClass.query = new BooleanQuery();
        IndexSearcher mysea = new IndexSearcher("d:/index/");

        SqlHelper Helper = new SqlHelper("HisDB");
        if (cboDepartment == "$")
        {
            SQLSentence = "select count(*) from tdjk where zyrq between  '" + strStart + "' and '" + strEnd + "'";
            SQLSentenceln = "select count(*) from tdjkz where zyrq between  '" + strStart + "' and '" + strEnd + "'";
        }
        else
        {
            SQLSentence = "select count(*) from tdjk where zyrq between  '" + strStart + "' and '" + strEnd + "' and ksbm='" + cboDepartment + "'";
            SQLSentenceln = "select count(*) from tdjkz where zyrq between  '" + strStart + "' and '" + strEnd + "' and ksbm='" + cboDepartment + "'";
        }

        DataTable dtcy = Helper.GetDataTable(SQLSentence);

        DataTable dtzy = Helper.GetDataTable(SQLSentenceln);
        zyhcount = Convert.ToDouble(dtzy.Rows[0][0]) + Convert.ToDouble(dtcy.Rows[0][0]);
        DateTime dtEnd = DateTime.MaxValue;
        DateTime.TryParse(strEnd, out dtEnd);
        DateTime dtStart = DateTime.MinValue;
        DateTime.TryParse(strStart, out dtStart);

        try
        {
            foreach (System.Collections.Generic.KeyValuePair<string, string> item in dic)
            {
                if (item.Value != "")
                {
                    if (item.Key == "nl_down" || item.Key == "nl_up" || item.Key == "tw_down" || item.Key == "tw_up" || item.Key == "mb_down" ||
    item.Key == "mb_up" || item.Key == "hx_down" || item.Key == "hx_up" || item.Key == "xy_ssy_down" || item.Key == "xy_ssy_up" || item.Key == "xy_szy_down" ||
    item.Key == "xy_szy_up" || item.Key == "zyts_down" || item.Key == "zyts_up" || item.Key == "sg_down" || item.Key == "sg_up" || item.Key == "yz_down" || item.Key == "yz_up") continue;
                    gethits(item.Value, item.Key);
                }

            }
            Filter nlRangeFilter = null;
            Filter twRangeFilter = null;
            if (cboDepartment != "$")//按科室筛选
            {
                Query queryksbm = new TermQuery(new Term("ksbm", cboDepartment));
                MyClass.query.Add(queryksbm, BooleanClause.Occur.MUST);
            }
            if (dic["nl_down"] != "")//按年龄筛选
            {

                Term nl_down = new Term("nl", NumberTools.LongToString(Convert.ToInt64(dic["nl_down"])));
                Term nl_up = new Term("nl", NumberTools.LongToString(Convert.ToInt64(dic["nl_up"])));
                Query nlRangeQuery = new RangeQuery(nl_down, nl_up, true);
                MyClass.query.Add(nlRangeQuery, BooleanClause.Occur.MUST);


            }

            if (dic["tw_down"] != "")//按体温筛选
            {
                Term tw_down = new Term("tw", NumberTools.LongToString((long)Convert.ToDouble(dic["tw_down"])));
                Term tw_up = new Term("tw", NumberTools.LongToString((long)Convert.ToDouble(dic["tw_up"])));
                Query twRangeQuery = new RangeQuery(tw_down, tw_up, true);
                MyClass.query.Add(twRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["mb_down"] != "")//按脉搏筛选
            {
                Term mb_down = new Term("mb", NumberTools.LongToString(Convert.ToInt64(dic["mb_down"])));
                Term mb_up = new Term("mb", NumberTools.LongToString(Convert.ToInt64(dic["mb_up"])));
                Query mbRangeQuery = new RangeQuery(mb_down, mb_up, true);
                MyClass.query.Add(mbRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["hx_down"] != "")//按呼吸筛选
            {
                Term hx_down = new Term("hx", NumberTools.LongToString(Convert.ToInt64(dic["hx_down"])));
                Term hx_up = new Term("hx", NumberTools.LongToString(Convert.ToInt64(dic["hx_up"])));
                Query hxRangeQuery = new RangeQuery(hx_down, hx_up, true);
                MyClass.query.Add(hxRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["xy_ssy_down"] != "")//按收缩压筛选
            {
                Term xy_ssy_down = new Term("ssy", NumberTools.LongToString(Convert.ToInt64(dic["xy_ssy_down"])));
                Term xy_ssy_up = new Term("ssy", NumberTools.LongToString(Convert.ToInt64(dic["xy_ssy_up"])));
                Query xy_ssyRangeQuery = new RangeQuery(xy_ssy_down, xy_ssy_up, true);
                MyClass.query.Add(xy_ssyRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["xy_szy_down"] != "")//按舒张压筛选
            {
                Term xy_szy_down = new Term("szy", NumberTools.LongToString(Convert.ToInt64(dic["xy_szy_down"])));
                Term xy_szy_up = new Term("szy", NumberTools.LongToString(Convert.ToInt64(dic["xy_szy_up"])));
                Query xy_szyRangeQuery = new RangeQuery(xy_szy_down, xy_szy_up, true);
                MyClass.query.Add(xy_szyRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["zyts_down"] != "")//按住院天数筛选
            {
                Term zyts_down = new Term("zyts", NumberTools.LongToString(Convert.ToInt64(dic["zyts_down"])));
                Term zyts_up = new Term("zyts", NumberTools.LongToString(Convert.ToInt64(dic["zyts_up"])));
                Query zytsRangeQuery = new RangeQuery(zyts_down, zyts_up, true);
                MyClass.query.Add(zytsRangeQuery, BooleanClause.Occur.MUST);
            }

            if (dic["sg_down"] != "")//按身高筛选
            {
                Term sg_down = new Term("sg", NumberTools.LongToString(Convert.ToInt64(dic["sg_down"])));
                Term sg_up = new Term("sg", NumberTools.LongToString(Convert.ToInt64(dic["sg_up"])));
                Query sgRangeQuery = new RangeQuery(sg_down, sg_up, true);
                MyClass.query.Add(sgRangeQuery, BooleanClause.Occur.MUST);
            }
            if (dic["tz_down"] != "")//按体重筛选
            {
                Term tz_down = new Term("tz", NumberTools.LongToString(Convert.ToInt64(dic["tz_down"])));
                Term tz_up = new Term("tz", NumberTools.LongToString(Convert.ToInt64(dic["tz_up"])));
                Query tzRangeQuery = new RangeQuery(tz_down, tz_up, true);
                MyClass.query.Add(tzRangeQuery, BooleanClause.Occur.MUST);
            }


            RangeFilter filter = new RangeFilter("zyrq", strStart, strEnd, true, true);
            hits = mysea.Search(MyClass.query, filter);
            tj = hits.Length();

            DataTable dt = new DataTable("getqc");
            DataColumn dcRegistryID = new DataColumn("RegistryID", typeof(String));
            DataColumn dcNoteIDSeries = new DataColumn("NoteIDSeries", typeof(String));
            DataColumn dcNoteID = new DataColumn("NoteID", typeof(String));
            DataColumn dcEmrXml = new DataColumn("EmrXml", typeof(String));
            DataColumn dcKsbm = new DataColumn("ksbm", typeof(String));
            DataColumn dcZyrq = new DataColumn("zyrq", typeof(String));

            dt.Columns.Add(dcRegistryID);
            dt.Columns.Add(dcNoteIDSeries);
            dt.Columns.Add(dcNoteID);
            dt.Columns.Add(dcEmrXml);
            dt.Columns.Add(dcKsbm);
            dt.Columns.Add(dcZyrq);
            DataRow dr = null;


            if (tj != 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    dr = dt.NewRow();
                    dr["RegistryID"] = hits.Doc(StartPos).Get("RegistryID");
                    dr["NoteIDSeries"] = hits.Doc(StartPos).Get("NoteIDSeries");
                    dr["NoteID"] = hits.Doc(StartPos).Get("NoteID");
                    dr["EmrXml"] = hits.Doc(StartPos).Get("EmrXml");
                    dr["ksbm"] = hits.Doc(StartPos).Get("ksbm");
                    dr["zyrq"] = hits.Doc(StartPos).Get("zyrq");
                    StartPos = StartPos + 1;
                    dt.Rows.Add(dr);
                    if (StartPos >= tj) break;
                }
                EndPos = StartPos - 1;
            }
            return dt;
            mysea.Close();
        }

        catch (Exception e)
        {
            throw e;
            //Response.Write(e);

        }

    }
    [WebMethod]
    public void SaveSeach(string strseach, string Opcode)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SQLSentence = @"Insert Into TB_DatamissingSeach (strSeach,Opcode,RecordTime) values('" + strseach + "','" + Opcode + "','" + DateTime.Now + "')";
        try
        {
            Helper.ExecuteSql(SQLSentence);
        }
        catch (Exception ex)
        {

        }
    }
    [WebMethod]
    public void SaveSeachEx(string name, XmlDocument doc, string Opcode)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SQLSentence = @"Insert Into TB_DatamissingSeach (Name,strSeachxml,Opcode,RecordTime) values('" + name + "','" + doc.OuterXml + "','" + Opcode + "','" + DateTime.Now + "')";
        try
        {
            Helper.ExecuteSql(SQLSentence);
        }
        catch (Exception ex)
        {

        }
    }
    [WebMethod]
    public void DelSeach(string pk)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SQLSentence = @"DELETE FROM TB_DatamissingSeach WHERE pk = '" + pk + "'";
        try
        {
            Helper.ExecuteSql(SQLSentence);
        }
        catch (Exception ex)
        {

        }
    }
    [WebMethod]
    public DataSet GetSeachList(string Opcode)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SQLSentence = @"SELECT pk,strSeach FROM TB_DatamissingSeach  WHERE Opcode = '" + Opcode + "'and strSeachxml is null";
        try
        {
            DataSet dt = Helper.GetDataSet(SQLSentence);
            return dt;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
    [WebMethod]
    public DataSet GetSeachListEx(string Opcode)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string SQLSentence = @"SELECT pk,Name,strSeachxml FROM TB_DatamissingSeach  WHERE Opcode = '" + Opcode + "'and strSeach is null";
        try
        {
            DataSet dt = Helper.GetDataSet(SQLSentence);
            return dt;

        }
        catch (Exception ex)
        {
            return null;
        }
    }
    [WebMethod(Description = "Get notes with valuate rules", EnableSession = false)]
    public string GetNoteIDsWithValuateRulesEnd(ref XmlNode rules)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        XmlDocument doc = new XmlDocument();
        XmlNode root = doc.CreateElement(ElementNames.Rules);
        XmlNode tmp = doc.CreateElement("tmp");
        try
        {
            command.CommandText = "SELECT NoteID, Rules FROM ValuateRulesEnd";
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                XmlElement rule = doc.CreateElement(ElementNames.Rule);
                rule.SetAttribute(AttributeNames.NoteID, reader[0].ToString());
                tmp.InnerXml = reader[1].ToString();
                foreach (XmlAttribute attr in tmp.FirstChild.Attributes)
                {
                    rule.SetAttribute(attr.Name, attr.Value);
                }
                root.AppendChild(rule);

            }
            rules = root.Clone();
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }

    [WebMethod(Description = "Get valuate Flaws for a note", EnableSession = false)]
    public string GetValuateDetailEndExEx(string noteID, string RegistryID, string series, ref XmlNode Flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlCommand command = connection.CreateCommand();
        try
        {
            if (noteID == "00") command.CommandText = "SELECT Flaws FROM ValuateDetailEnd WHERE noteID = @noteID and RegistryID =@RegistryID ";
            else
                command.CommandText = "SELECT Flaws FROM ValuateDetailEnd WHERE noteID = @noteID and RegistryID =@RegistryID and Flaws.exist('/EmrNote[@Series = \"" + series + "\"]') = 1";
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters[0].Value = noteID;
            command.Parameters.Add("@RegistryID", SqlDbType.VarChar, 12);
            command.Parameters[1].Value = RegistryID;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader[0].ToString());
                Flaws = doc.DocumentElement.Clone();
            }
            reader.Close();
            connection.Close();
            return null;
        } //try end
        catch (Exception ex)
        {
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod]
    public DataSet ValuateOpcodeEnd(string noteID, string RegistryID)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "SELECT Opcode FROM ValuateDetailEnd";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where noteID = '" + noteID + "' and RegistryID = '" + RegistryID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    [WebMethod(Description = "New valuate detail", EnableSession = false)]
    public string NewValuateDetailEndEX(bool self, string registryID, string noteID, string series, decimal score, string opcode, XmlNode flaws)
    {
        SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        try
        {
            command.Parameters.Add("@regID", SqlDbType.VarChar, 12);
            command.Parameters.Add("@noteID", SqlDbType.VarChar, 5);
            command.Parameters.Add("@score", SqlDbType.Decimal);
            command.Parameters.Add("@flaws", SqlDbType.Xml);
            command.Parameters.Add("@opcode", SqlDbType.VarChar, 4);
            command.Parameters.Add("@opdate", SqlDbType.DateTime);
            command.Parameters[0].Value = registryID;
            command.Parameters[1].Value = noteID;
            command.Parameters[2].Value = score;
            command.Parameters[3].Value = flaws.OuterXml;
            command.Parameters[4].Value = opcode;
            command.Parameters[5].Value = SysTime();

            if (self)
            {
                if (noteID == "00") command.CommandText = "select * FROM SelfValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "'";
                else
                    command.CommandText = "select * FROM SelfValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "' and Flaws.exist('/EmrNote[@Series = \"" + series + "\"]') = 1";
            }
            else
            {
                if (noteID == "00") command.CommandText = "select * FROM ValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "'";
                else
                    command.CommandText = "select * FROM ValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "' and Flaws.exist('/EmrNote[@Series = \"" + series + "\"]') = 1";
            }
            SqlHelper oHelper = new SqlHelper("EmrDB");
            DataTable dt = oHelper.GetDataTable(command.CommandText);
            if (dt.Rows.Count > 0)
            {
                if (self)
                {
                    if (noteID == "00") command.CommandText = "DELETE FROM SelfValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "'";
                    else
                        command.CommandText = "DELETE FROM SelfValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "' and Flaws.exist('/EmrNote[@Series = \"" + series + "\"]') = 1";
                }
                else
                {
                    if (noteID == "00") command.CommandText = "DELETE FROM ValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "'";
                    else
                        command.CommandText = "DELETE FROM ValuateDetailEnd WHERE RegistryID = '" + registryID + "' AND NoteID = '" + noteID + "' and Flaws.exist('/EmrNote[@Series = \"" + series + "\"]') = 1";
                }
                command.ExecuteNonQuery();
                if (self)
                {
                    command.CommandText = "INSERT INTO SelfValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
                }
                else
                {
                    command.CommandText = "INSERT INTO ValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
                }
                command.ExecuteNonQuery();

            }
            else
            {
                if (self)
                {
                    command.CommandText = "INSERT INTO SelfValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
                }
                else
                {
                    command.CommandText = "INSERT INTO ValuateDetailEnd VALUES(@regID, @noteID, @score, @flaws, @opcode, @opdate)";
                }
                command.ExecuteNonQuery();
            }




            transaction.Commit();
            connection.Close();
            return null;


        } //try end
        catch (Exception ex)
        {
            transaction.Rollback();
            connection.Close();
            return ex.Message;
        }
    }
    [WebMethod]
    public DataSet GetSettlementdate(string registryID)
    {
        SqlHelper oHelper = new SqlHelper("HisDB");
        string strSelect = "select jsrq as '结算日期' from tfy";
        DataSet dt = new DataSet();
        try
        {

            strSelect += " where zyh = '" + registryID + "'";
            dt = oHelper.GetDataSet(strSelect);

        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    
    [WebMethod]
    public void PutRangex(string RegistryID, string DocRange)
    {
        DocRange.Replace('$', '\v');
        SqlHelper Helper = new SqlHelper("EmrDB");
        string Insert = @"Insert into TB_RemberGroup (RegistryID,Range1) Values('" + RegistryID + "','" + DocRange.Replace("'", "''") + "')";

        DeletRangex(RegistryID);
        try
        {
            Helper.ExecuteNonQuery(Insert);
        }
        catch (Exception ex)
        {
            return;
        }
    }
    [WebMethod]
    public void DeletRangex(string RegistryID)
    {
        SqlHelper Helper = new SqlHelper("EmrDB");
        string DEL = "DELETE FROM TB_RemberGroup WHERE RegistryID ='" + RegistryID + "'";
        try
        {
            Helper.ExecuteNonQuery(DEL);
        }
        catch
        {
            return;
        }
    }
    [WebMethod(Description = " ", EnableSession = false)]
    public Boolean ArchiveBatchEx(XmlNode registryIDs)
    {
        try
        {
            SqlConnection connection = new SqlConnection(ConfigClass.GetConfigString("appSettings", "EmrDB"));
            connection.Open();

            foreach (XmlNode registryID in registryIDs)
            {
                string SQLSentence = "UPDATE EmrDocument SET status = 1 WHERE RegistryID='"
                    + registryID.InnerText + "'";
                SqlDataAdapter adapter = new SqlDataAdapter(SQLSentence, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
            }

            connection.Close();

            SqlConnection connection_his = new SqlConnection(ConfigClass.GetConfigString("appSettings", "HisDB"));
            connection_his.Open();
            SqlHelper HelperHis = new SqlHelper("HisDB");
            foreach (XmlNode registryID in registryIDs)
            {
                //
                string SQLSentence = "SELECT COUNT(*) FROM tdjk WHERE zyh = '" + registryID.InnerText + "'";
                string SQLSentenceIn = "SELECT COUNT(*) FROM tdjkz WHERE zyh = '" + registryID.InnerText + "'";
                DataTable dtTemp = HelperHis.GetDataTable(SQLSentence);
                foreach (DataRow row in dtTemp.Rows)
                {
                    if (Convert.ToInt32(row.ItemArray[0]) == 0)
                    {
                        string SQLSentence_his = "UPDATE tdjkz SET emrstatus = 1,emrgdsj='" + DateTime.Now + "' WHERE zyh='"
                    + registryID.InnerText + "'";
                        SqlDataAdapter adapter_his = new SqlDataAdapter(SQLSentence_his, connection_his);
                        DataSet dataSet_his = new DataSet();
                        adapter_his.Fill(dataSet_his);
                    }
                    else
                    {
                        string SQLSentence_his = "UPDATE tdjk SET emrstatus = 1,emrgdsj='" + DateTime.Now + "' WHERE zyh='"
                           + registryID.InnerText + "'";
                        SqlDataAdapter adapter_his = new SqlDataAdapter(SQLSentence_his, connection_his);
                        DataSet dataSet_his = new DataSet();
                        adapter_his.Fill(dataSet_his);
                    }
                }
                //

            }

            connection_his.Close();


            return EmrConstant.Return.Successful;
        }
        catch (Exception ex)
        {
            ErrorLog("ArchiveAll", "EmrDocument", "UPDATE", ex.Source);
            return EmrConstant.Return.Failed;
        }
    }
  

    //大港借阅功能
    [WebMethod]
    public DataTable GetBorrowInfoBackEmr(string code)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select BorrowDepartName as '借阅科室',BorrowPeopleName as '借阅人',BorrowTime as '借阅日期',BorrowPurpose as '借阅目的',"
        + "Checker as '审批人',CheckTime  as '审批时间',ReturnStatus as '送还状态',ShouldReturnTime as '应送还日期',RealReturnTime as '实际送还日期',DelayTime as '延迟时间' from BorrowEmrDocument where BorrowPeopleCode= '" + code + "' and ReturnStatus=0";
        DataSet dst = new DataSet();
        DataTable dt = null;
        try
        {
            dst = oHelper.GetDataSet(strSelect);

            if (dst.Tables.Count != 0)
                dt = dst.Tables[0];
        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
    [WebMethod]
    public int InsertBorrowInfo(string Category, string departCode,string departName,string docotorName, string docotorCode,string Checker,string Purpose,string shouldReturnTime)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
       
        string str = @"INSERT INTO [BorrowEmrDocument]
           ([BorrowDepartCode]
           ,[BorrowDepartName]
           ,[BorrowPeopleCode]
           ,[BorrowPeopleName]
           ,[BorrowCateGory]
           ,[BorrowTime]
           ,[BorrowPurpose]
           ,[Checker]
           ,[CheckTime]
           ,[ReturnStatus]
           ,[ShouldReturnTime]
           ,[RealReturnTime]
           ,[DelayTime])
     VALUES ('" + departCode + "','" + departName + "','"+ docotorCode + "','"  + docotorName + "','" + Category + "','" + DateTime.Now.ToString() + "','" + Purpose + "','" + Checker + "','" + DateTime.Now.ToString() + "',0,'" + shouldReturnTime + "','','')";
        DataSet dst = new DataSet();
        int count = 0;
        try
        {
            count = oHelper.ExecuteNonQuery(str);

        }
        catch (Exception)
        {
            return 0;
        }
        return count;
    }
    [WebMethod]
    public int BackEmr(DateTime start ,string code)
    {
        string delayTime = "",dateDiff="";        
        TimeSpan ts = DateTime.Now.Subtract(start);
        if (ts.TotalHours > 0)
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
        else dateDiff = "0";
        delayTime = dateDiff;
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string str = "Update BorrowEmrDocument set ReturnStatus=1,RealReturnTime='" + DateTime.Now.ToString() + "',DelayTime='" + delayTime + "' where BorrowPeopleCode='"+code+"'";
        DataSet dst = new DataSet();
        int  count = 0;
        try
        {
            count = oHelper.ExecuteNonQuery(str);
           
        }
        catch (Exception)
        {
            return 0;
        }
        return count;
        
    }

    [WebMethod]
    public DataTable GetBorrowInfo(string startTime,string endTime)
    {
        SqlHelper oHelper = new SqlHelper("EmrDB");
        string strSelect = "select BorrowDepartName as '借阅科室',BorrowPeopleName as '借阅人',BorrowTime as '借阅日期',BorrowPurpose as '借阅目的',"
        + "Checker as '审批人',CheckTime  as '审批时间',ReturnStatus as '送还状态',ShouldReturnTime as '应送还日期',RealReturnTime as '送还日期',DelayTime as '延迟时间' from BorrowEmrDocument where BorrowTime between '" + startTime+"' and '"+endTime+"'";
        DataSet dst = new DataSet();
        DataTable dt=null;
        try
        {
            dst = oHelper.GetDataSet(strSelect);

            if (dst.Tables.Count!= 0)
                dt = dst.Tables[0];
        }
        catch (Exception)
        {
            return null;
        }
        return dt;
    }
}
    


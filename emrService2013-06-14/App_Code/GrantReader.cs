using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for GrantReader
/// </summary>
public class GrantReader
{
    public string registryID;
    public string doctorID;
    public DateTime startDate;
    public int expiration;

    public GrantReader()
    {
    }
    public GrantReader(string sRegistryID, string sDoctorID, DateTime dtStartDate, int nExpiration)
    {
        registryID = sRegistryID;
        doctorID = sDoctorID;
        startDate = dtStartDate;
        expiration = nExpiration;
    }
}

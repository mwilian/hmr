using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using Word=Microsoft.Office.Interop.Word;
using EmrConstant;
using System.Windows.Forms;
using HuronControl;

namespace CommonLib
{
  public class Globals
    {

        public static string DoctorID = "";
        public static string DoctorName = "";
        public static string OpDepartID = "";
        public static string DepartID = "";
        public static string OpDepartName="";
        public static string DepartName = "";
        public static string AreaID = "";
        //public static string NoteID = "";
        //public static string NoteName = "";
        //public static string TemplateName = "";       
        //public static bool OnWriteForm = false;
        public static string workFolder = @"D:\emr";
        public static string blockFolder = @"D:\emr\Block";       
        public static string linkListFolder = null;
        public static string patientListFolder = null;
        public static string picGalleryFile = null;
        public static string doctorNamesFile = null;  
        public static string RegistryID = "";
        //public static bool self = false;
        public static string patientFile = @"D:\emr\patients.xml";
        public static int beginTime = 0;
        public static string PatientConsultSequence = "";
        public static string templateFolder = Path.Combine(Globals.workFolder, EmrConstant.ResourceName.TemplateFolder);
        public static XmlNode Config = null;
        public static List<string> myfunctions = new List<string>();
        public static string docPath = "";
        #region Attribute welcome
        public static bool offline = false;
        public static string currentDirectory = Application.StartupPath;
        public static string NullDoc = currentDirectory + @"\null.docx";
        public static string addinConfig = "";
        public static string hisServer = "";
        public static string emrServer = "";
        public static string currentVersion = "";
        public static string newVersionFolder = "";
        public static string emrPatternFile = "";
        public static string ChildPatternFile = "";
        public static string localMachineName = "";
        public static string localMachineIP = "";
        public static string hospitalName = "";        
        public static string tmpFolder = "";
        public static string doctorsFile = "";
        public static string departmentFile = "";
        public static string icd10File = "icd10.icd";

        //public static bool useDigitalSignature = false;         // 启用数字签名
        //public static bool continueAsEmrOpened = true;

        public static EmrPattern emrPattern = null;
        public static EmrPattern childPattern = null;
        public static MyConfig myConfig = null;
        public static Doctors doctors = null;
        public static Departments departments = null;      
        #endregion

        #region setOption             
        public static bool commitedAsEnd = false;



        #endregion
      
        #region Emr font
        public static float labelFontSize = 10.5F;
        public static float headerFontSize = 10.5F;
        public static float noteNameFontSize = 11F;
        public static Word.WdColor headerFontColor = Word.WdColor.wdColorGray85;
        public static Word.Font labelFont ;
        public static Word.Font contentFont ;
        public static Word.Font headerFont ;
        public static Word.Font noteNameFont ;       
        public static int countPerLine = 39;
        public static char padChar = ' ';
        public static System.Drawing.Color emrOpeningColor = System.Drawing.Color.Green;
        #endregion

        #region come from config
        public static bool patientInfoAtPageHeader = false;   // emr note header(patient info.) as page header
        //public static bool SignRight = false;
        public static bool chuyuan = false;
        #endregion

        public static bool inStyle = true;         // inpatient or outpatient department  
        public static bool autoCheckSync = false;
        public static AuthorSequence authorSeq = AuthorSequence.Backword;
        public static AuditLevelSystem auditLevelSystem = AuditLevelSystem.ChiefDoctor;
        public static string space6 = "  ";
        public static string auditSystem = "A1";
        public static bool localDocumentEncode = true;
        public static bool Isfanxiu = false;
        public static string myroles = "";
        public static object myFalse = false;
        public static object myTrue = true;
        public static object passwd = "jwsj";
        public static string NoteID = "";
        public static int commitTimeOut = 0;
        //public static bool CommitTime = false;
        public static int showOpDoneTime = 0;
        public static bool Sign = false;
        public static bool health = false;
        public static bool IsCommit = false;
        public static bool isEndRule = false;
        #region editword
        public static bool EdeditNote = false;
        public static NoteEditMode EeditMode;
        public static bool Eshixi = false;
        public static AuthorInfo EauthorInfo =new AuthorInfo();
        public static XmlNode ENodePattern = null;
        public static string EdoctorType = "";
        public static XmlElement EemrNote = null;
        #endregion
        #region newnote
        public static bool NnewNote = false;        
        #endregion
        #region marge
        public static bool Mmarge = false;
        public static XmlDocument Mdoc = null;
        public static string MIsSingle = "";
        public static string MnoteID = "";
        public static TreeNode Mnode = null;
        #endregion
        #region template
        public static bool Ttem = false;
        public static string templateFile = "";
        public static bool OpenTemplate = false;
        #endregion
        #region Block
        public static bool Bblock = false;
        public static string BdocFile = "";
        public static bool BnewBlock = false;
        #endregion
        public static bool Cselect = false;
        public static bool Cselectd = false;
        public static bool btnPfQxTj = false;
        public static bool Vvaluate = false;
        public static bool ValuatPrint = false;
        public static bool CreateConsent = false;
        public static string outDefaultFirstVisitNoteID = "31";
        public static string outDefaultReturnVisitNoteID = "32";
        public static EmrConstant.CardUseWay cardUseWay = EmrConstant.CardUseWay.NoCard;
        public static EmrConstant.PriceUnitMode defaultPriceUnitMode = EmrConstant.PriceUnitMode.Use;
        public static bool mustByPoorPatient = false;
        public static double limitInDays = 3;
        public static bool usePage16k = true;
        public static int timeout = 0;
        public static int showPatientInfoTime = 10000;
        public static bool expandPatientTree = false;
        public static string mergeCode = Groups.One;
        public static bool currentBlockIsNew = false;
        public static int currentBlockPk = 0;
        public static string currentBlockName = "";
        public static bool WriteOff = false;
        public static string frmMainText = "";
        
        public static string mainTitle = null;
       
        //临床路径
        public static bool newPath = false;
        public static string[] jdxh = null;
        public static bool NewInsertLc = false;
        public static bool InsertLc = false;
        public static string[] _docname = null;

      //复制记录
        public static bool _DuplicateNote = false;
      //在院出科
        public static int inoutmode = 0;
        public static string zxtdm = "19";
        public static int selectIndex = 0;

      //在院
        public static LogAdapter logAdapter;

      //确定已经进行了保存或者提交
        public static bool saveOk = false;

      //合并
        public static bool MagerPrint = false;
        public static bool RefreshInf = false;


        public static bool ArchiveDepartment = false;
        public static string qualityControl { get; set; }
        public static string ArchiveDepartmentText = "";
        public static string departmentCode { get; set; }
    }
}

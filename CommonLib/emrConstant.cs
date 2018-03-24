using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Xml;
using CommonLib;

namespace EmrConstant
{

    public struct VisitFlag
    {
        public const string First = "初诊";
        public const string Return = "复诊";
    }
    public struct RibbonID
    {
        public const string PrescriptionNum = "PrescriptionNum";
        public const string TestNum = "TestNum";
        public const string NewOrder = "NewOrder";
        public const string PatientName = "PatientName";
        public const string RegistryID = "RegistryID";
        public const string Transfer = "Transfer";
        public const string CallBackPatient = "CallBackPatient";
        public const string BecomeInPatient = "BecomeInPatient";
        public const string UnSeePatient = "UnSeePatient";
        public const string Finished = "Finished";
        public const string VisitFlag = "VisitFlag";
        public const string OldOrder = "OldOrder";
        public const string OldTestNum = "OldTestNum";
        public const string ReferenceTest = "ReferenceTest";
        public const string ClinicInfo = "ClinicInfo";
        public const string CostInfo = "CostInfo";
        public const string WriterOrder = "WriterOrder";
        public const string OPatient = "OPatient";
        public const string Pharmacy = "Pharmacy";
        public const string TabEmr = "emr";
        public const string WriteNote = "WriteNote";
        public const string TabBlock = "TabBlock";
        public const string Undo = "Undo";
        public const string Redo = "Redo";
        public const string Shape = "EmrShape";
        public const string PhysicalExamNum = "PhysicalExamineNum";
        public const string OldPhysicalExamNum = "OldPhysicalExamineNum";
        public const string ReferencePhysicalExam = "ReferencePhysicalExam";
        public const string ImageExamineNum = "ImageExamineNum";
        public const string OldImageExamineNum = "OldImageExamineNum";
        public const string ReferenceImageExam = "ReferenceImageExam";
        public const string TreatBillNum = "TreatBillNum";
        public const string TreatOrderNum = "TreatOrderNum";
        public const string OldTreatBill = "OldTreatBill";
        public const string OldTreatOrder = "OldTreatOrder";
        public const string UndoOrder = "UndoOrder";
        public const string ReferenceEhr = "ReferenceEhr";
        public const string QueryGallery = "QueryGallery";
        public const string SafeGallery = "SafeGallery";
        public const string MyPicture = "MyPicture";
        public const string MyPicture1 = "MyPicture1";
        public const string InsertHeader = "InsertHeader";
        public const string Congener = "Congener";

        public const string OldOrder1 = "OldOrder1";
        public const string NewOrder1 = "NewOrder1";
        public const string OkOrder1 = "OkOrder1";
        public const string TabOrders = "TabOrders";
        public const string UpdateHeader = "UpdateHeader";

        /* Block operations */
        public const string NewBlock = "NewBlock";
        public const string SaveBlock = "SaveBlock";
        public const string RemoveBlock = "RemoveBlock";
        public const string BlockWin = "BlockWin";
        public const string CloseBlock = "CloseBlock";
        /* Phrase operations */
        public const string EmrPhrase = "EmrPhrase";
        /* QC operations */
        public const string DefPattern = "DefPattern";
        public const string SetAudit = "SetAudit";
        public const string SetOptions = "SetOptions";
        public const string DefRules = "DefRules";
        public const string NewPattern = "NewPattern";
        public const string QualityQuery = "QualityQuery";
        public const string QCReport = "QCReport";
        public const string QCReportSelf = "QCReportSelf";
        public const string QCTimeOut = "QCTimeOut";
        public const string ShowOrders = "ShowOrders";
        public const string XmlView = "XmlView";
        public const string PhraseUse = "PhraseUse";
        public const string CommitE = "CommitE";
        public const string JieD = "JieD";
        public const string ShenHe = "ShenHe";
        /* Emr note operations */
        public const string Synchronize = "Synchronize";
        public const string Archive = "Archive";
        public const string EmrQuery = "EmrQuery";
        public const string EmrQuery2 = "EmrQuery2";
        /* System manager operations */
        public const string EnvConfig = "EnvConfig";
        public const string ImportPhrase = "ImportPhrase";
        public const string ExportPhrase = "ExportPhrase";
        public const string ImportBlock = "ImportBlock";
        public const string ExportBlock = "ExportBlock";
        public const string ImportTemplate = "ImportTemplate";
        public const string ExportTemplate = "ExportTemplate";
        public const string Deputed = "Deputed";
        public const string Deputing = "Deputing";
        //public const string EditServerHisUrl = "EditServerHisUrl";
        //public const string EditServerEmrUrl = "EditServerEmrUrl";
        //public const string AppServerHisUrl = "AppServerHisUrl";
        //public const string AppServerEmrUrl = "AppServerEmrUrl";
    }
    public enum NoteNameMode
    {
        TemplateName = 0,
        PartternNmae
    }
    public enum ImpExp
    {
        Import = 1,
        Export
    }
    public enum PriceUnitMode
    {
        Special = 0,
        Sell,
        Use
    }
    public enum AuthorSequence
    {
        Forword = 1,
        Backword
    }
    public enum OperationPattern
    {
        EmrNote = 1,
        Subtitle,
        Content,
        Phrase,
        Control,
        CompEmrNote
    }
    public enum OperationType
    {
        NewNote=0,//新建
        Commit,//提交
        Uncommit, //返修
        Delete,
    }
    public enum WorkMode
    {
        InHospital = 1,
        OutHospital,
        Service
    }
    public enum Button
    {
        SaveNote,
        CommitNote
    }

    public enum OperatorRole
    {
        Nothing = 0,
        Writer,
        Checker,
        FinalChecker
    }
    public struct TrustText
    {
        public const string Deputed = "代理医师：";
        public const string Deputing = "被代理医师：";
        public const string DeputedSucc = "为自己指定代理医师成功。";
        public const string DeputingSucc = "为自己指定被代理医师成功。";
    }
    public enum Trust
    {
        Deputing = 0,
        Deputed
    }
    public enum RegState
    {
        Normal = 0,
        Seeing,         // doing or recall
        Reserve1,
        Transfered,     // transfered from the other department
        Reaseve2,
        Reaseve3,
        Reaseve4,
        Reaseve5,
        Expired,
        Finished        // finished or become inpatient
    }
    public class RegStateText
    {
        public RegStateText() { }
        public string[] Text = { " ", "接诊中", " ", "转科", " ", " ", " ", " ", "过期", "诊毕" };
    }
    public class NoteStateText
    {
        public string[] Text = { "草稿", "已提交", "审核中", "已审核", "终审中", "已终审" };
        public NoteStateText() { }
    }
    public enum CardUseWay
    {
        NoCard = 0,
        Required,
        OptionNoCard,
        OptionCard
    }
    public enum OrderType
    {
        Prescription = 0,
        Image,
        Test,
        Physical,
        Fee,
        ChinesePrescription,
        TreatOrder,
    }
    public class OrderTypeText
    {
        public OrderTypeText() { }
        public string[] Text = { "西药方", "影像检查单", "化验单", "物理检查单", "治疗收费", "中药方", "治疗医嘱" };
    }
    public enum NoteNewDegree
    {
        Same = 1,
        Newer,
        New
    }
    public enum CreateWorkFolderMode
    {
        Build = 1,
        Rebuild
    }
    public enum PatientGettingMode
    {
        PatientsInHospital = 1,
        PatientsInArea,
        PatientsInDepartment,
        MyPatients,
        PatientsInDepartmentOut,
        MyPatientsOut,
        Consult
    }
    public enum QueryMode
    {
        ArchiveNum = 1,
        RegistryID,
        PatientName,
        CardNum,
        IDNum,
        Commpose
    }
    public enum ComeMode
    {
        Combox = 1,
        MouseClick
    }
    public enum NoteStatus
    {
        Draft = 0,
        Commited,
        Checking,
        Checked,
        FinallyCkecking,
        FinallyChecked
    }
    public enum NoteEditMode
    {
        Writing = 0,
        Checking,
        FinallyCkecking,
        Nothing,
        Reading
    }
    public enum PatientType
    {
        Common = 1,
        Insure
    }
    public enum MsgTpe
    {
        Error = 0,//"错误";
        Warning , //"警告";
        InfoPrompt//"提示";
    }
    public struct PatientTypeText
    {
        public const string Common = "普通";
        public const string Insure = "医保";
        public const string Unknown = "其他";
    }
    public struct RegistryKey
    {
        public const string keyName = "HKEY_CURRENT_USER\\Software";
        public const string valueName = "EMRWS";
        public const string NoEMRWS = "NoEMRWS";
    }
    public struct SubtitleContent
    {
        public const string Content = "正文";
        public const string Phrase = "填空";
        public const string Control = "选框";
    }
    public struct ResourceName
    {
        public const string PatientsXml = "patients.xml";
        public const string QueryPatientsXml = "queryPatients.xml";
        public const string PatientsXsd = "patients.xsd";
        public const string DoctorsXml = "Doctors.xml";
        public const string DoctorsXsd = "Doctors.xsd";
        public const string DoctorNamesXml = "医师名单.xml";
        public const string DepartmentsXml = "科室名单.xml";
        public const string DepartmentsXsd = "Departments.xsd";
        public const string EmrPatternXml = "EmrPattern.xml";
        public const string ChildPatternXml = "ChildPattern.xml";
        public const string EmrPatternXsd = "EmrPattern.xsd";
        public const string EmrBlocksXml = "EmrBlocks.xml";
        public const string EmrBlocksXsd = "EmrBlocks.xsd";
        public const string PicGalleryXml = "PicGallery.pic";
        public const string PicGalleryXsd = "PicGallery.xsd";

        public const string NotesXml = "notes.xml";
        public const string OperatorsXml = "operators.xml";
        public const string MyConfigXml = "myconfig.xml";
        public const string HospitalNoteTemplate = "HospitalTemplate.note";
        public const string DepartNoteTemplate = "dpTemplate.note";
        public const string PersonNoteTemplate = "pnTemplate.note";
        public const string NoteTemplateDoc = "Template.not";
        //public const string DepartNoteTemplateLocation = "dpTemplate";
        //public const string PersonNoteTemplateLocation = "pnTemplate";

        public const string Phrase = "phrase.phr";

        //public const string Mytmp = "mytmp.notx";
        public const string Mytmp = "病历记录.docx";
        public const string RefNote = "参照记录.docx";
        public const string MyDocName = "病程记录";
        public const string MargeDoc = "合并记录.docx";
        public const string NullName = "null";
        public const string NullDoc = "null.docx";


        public const string LinkListFolder = "linkList";
        public const string PatientFolder = "patients";
        public const string TemplateFolder = "templates";
        public const string WorkFolder = "emrworks";

        public const string blockFileExtension = ".block";
    }
    public struct SqlOperations
    {
        public const string Select = "SELECT";
        public const string Update = "UPDATE";
        public const string Insert = "INSERT";
        public const string Delete = "DELETE";
    }
    public struct DbTables
    {
        public const string EmrDocument = "EmrDocument";
        public const string InRegitry = "InRegistry";
        public const string Patients = "Patients";
        public const string Configure = "Configure";
        public const string GrantReader = "GrantReader";
        public const string DrugList = "DrugList";
        public const string AnaesthesiaDrugList = "AnaesthesiaList";
        public const string DrugWayList = "DrugWayList";
        public const string ChineseDrugWayList = "ChineseDrugWayList";
        public const string ChineseDrugHowList = "ChineseDrugHowList";
        public const string DrugTimeList = "DrugTimeList";
        public const string PharmacyList = "PharmacyList";
        public const string OrderItems = "OrderItems";
        public const string PredefinedOrder = "PredefinedOrder";
        public const string PredefinedOrderItems = "POItems";
        public const string PrivateDrugNames = "PrivateDrugNames";
        public const string DrugKind = "DrugKind";
        public const string PrescriptionItems = "PrescriptionItems";
        public const string CombineConflict = "CombineConflict";
        public const string ImageExamTypes = "ImageTypes";
        public const string ImageExmaItems = "ImageItems";
        public const string ImageExmaStyles = "ImageStyles";
        public const string ImageExmaFeeIndexes = "ImageFeeIndexes";
        public const string ExamTestFeeItems = "ExamTestFeeItems";
        public const string ExamTestPriceList = "Price";
        public const string PhysicalExamItems = "PhysicalItems";
        public const string PhysicalExamTypes = "PhysicalTypes";
        public const string TestItems = "TestItems";
        public const string TestTypes = "TestTypes";
        public const string TestItemsDepartments = "TestItemsDepart";
        public const string TestSubitems = "TestSubitems";
        public const string TreatClass = "TreatClass";
        public const string TreatSubitems = "TreatSubitems";
        public const string TreatOrderDetail = "Detail";
        public const string CommonInfo = "Common";

        public const string OutRegistryID = "OutReg";
        public const string OutPatientRegInfo = "OutRegInfo";
        public const string OutPrescription = "OutPrescript";
        public const string OutPrescriptionNums = "OutPrescriptNums";
        public const string OutTreatCostPrescriptionNums = "OutCostPrescriptNums";
        public const string OutTreatCost = "OutTreatCost";
        public const string OutTestResults = "OutTestResults";
        public const string OutTestRequisitionNums = "OutTestRequisitionNums";
        public const string OutImageExamRequisitions = "OutImageRequisitions";
        public const string OutImageExamRequisitionNums = "OutImageRequisitionNums";
        public const string OutPhysicalExamRequisitions = "OutPhysicalRequisitions";
        public const string OutPhysicalExamRequisitionNums = "OutPhysicalRequisitionNums";
        public const string OutTreat = "OutTreat";
        public const string OutTreatRequisitionNums = "OutTreatRequisitionNums";

        public const string WestDrug = "WestDrug";
        public const string ChineseDrug = "ChineseDrug";
        public const string HerbalDrug = "HerbalDrug";
        public const string Treat = "Treat";
        public const string Exam = "Exam";
        public const string Icd10 = "Icd10";

        public const string BeInpatient = "Inpatient";
        public const string Areas = "Areas";
        public const string PType = "PType";
        public const string Departments = "Departments";
        public const string Doctors = "Doctors";
        public const string Rooms = "Rooms";
        public const string Beds = "Beds";
    }
    public struct MyMenuItems
    {
        /* Note operations on patient tree */
        public const string OpenEmr = "openEmr";
        public const string CloseEmr = "closeEmr";
        public const string Archive = "archive";
        public const string UnlockEmr = "unlockEmr";
        public const string Browse = "browse";
        public const string NewNote = "newNote";
        public const string EditNote = "editNote";
        public const string SignNote = "signNote";
        public const string ExportNote = "exportNote";
        public const string Uncommit = "uncommit";
        public const string locate = "locate";
        public const string Valuate = "valuate";
        public const string DuplicateNote = "duplicateNote";
        public const string RequisitionPrintNote = "requistionPrint";
        public const string UseNoteTemplate = "useNoteTemplate";
        public const string DeleteNote = "delNote";
        public const string Merge = "marge";
        public const string InputBasicInfo = "InputBasicInfo";
        public const string AsDepartTemplates = "asDepartTemplate";
        public const string AsHospitalTemplates = "asHospitalTemplate";
        /* Note template operations */
        public const string Open = "open";
        public const string Use = "use";
        public const string Delete = "delete";
        public const string Rename = "rename";
        public const string Attribute = "attribute";

        /* Note operations on noteMenu */
        public const string NoteSave = "noteSave";
        public const string NoteCommit = "noteCommit";
        public const string Icd10 = "icd10";
        public const string AsTemplate = "asTemplate";
        public const string AsDepartTemplate = "asDepartmentTemplate";
        public const string Reference = "reference";
        public const string UseBlocks = "useBlocks";
        public const string TemplateSave = "templateSave";
        public const string TemplateAbort = "templateAbort";
        public const string NoteValuate = "noteValuate";


        public const string PrePrint = "prePrint";
        public const string PosPrint = "posPrint";

        public const string NoteSaveCaption = "寄存";
        public const string NoteCommitCaption = "提交";
        public const string AsTemplateCaption = "存为个人模板";
        public const string AsDepartTemplateCaption = "存为科室模板";
        public const string TemplateSaveCaption = "保存模板";
        public const string ReferOrderCaption = "引入医嘱";
        public const string UseBlocksCaption = "引入构件";
    }
    public struct Return
    {
        public const Boolean Successful = true;
        public const Boolean Failed = false;
    }
    public struct ErrorMessage
    {

        public const string ErrLogFileName = "errorlog.xml";
        public const string ErrLogFileSchema = "errorlog.xsd";
        public const string XmlErr = "error";

        public const string Error = "错误";
        public const string Warning = "警告";
        public const string InfoPrompt = "提示";
        public const string SystemError = "系统错误！";
        public const string WebServiceError = "Web Service 失败，找系统管理员！";
        /* Environment configuration */
        public const string PasswdUpdateSucc = "口令更新成功！";
        public const string RequiredPharmacies = "必须选择可用药房！";
        public const string RequiredWorkFolder = "必须选择工作目录！";
        public const string WorkFolderDescription = "第一次运行需要建立工作目录";
        public const string FailedNewPassword = "新口令核对失败！";
        /* End print */
        public const string MustUseBlankPaper = "上次续打刚好到页尾，现在一定要使用空白纸，否则发生混乱!!!";
        public const string NotEmrNote = "Word 文档不是病历记录，继续打印吗？";
        public const string OkEndPrint = "\r\r     如果选择续打，请确认打印纸的正确安装！";
        public const string OkRemainLastTop = "是否保留续打位置？";

        public const string NoValidOrder = "没有有效医嘱！";
        public const string HasInvalidOrder = "有无效医嘱，继续吗？";
        public const string NoNewVersion = "no";
        public const string NoFindResult = "搜索结果为空！";

        public const string LogonUser = "无效操作员编码";
        public const string LogonPasswd = "口令错误";

        public const string TooManyEmr = "打开的病历数太多，请关闭暂且不操作的病历！";
        public const string NoOpeningEmr = "没有指定已打开病历！";
        public const string HasOpeningEmr = "病历已经被他人打开，不能多人打开同一病历！";
        public const string EmrDocumentError = "文档系统出错，应属人为破坏！";

        public const string ExistNoteContent = "这种记录只允许有一个!";
        public const string OnlyDraftCanDelete = "只有草稿状态允许删除！";
        public const string NoPrivilegeDelete = "没有删除记录的权限！";

        public const string ConfirmCommitNote = "确认提交之后，不能再修改。";
        public const string NotSaved = "文档尚未寄存！\r寄存吗？";
        public const string NotClosed = "记录尚未寄存或提交。退出吗？";

        public const string DocumentLocked = "病历已经归档，不能再修改！";
        public const string NoPrivilege = "没有权限执行此操作！";
        public const string NoPrivilegeNew = "没有足够权限，不能新建病程记录！";

        public const string NoPrivilegeEdit = "没有编辑权限！";
        public const string NoMarge = "草稿、审核中及终审中的纪录不能合并！";
        public const string NoPrintNotCommited = "没有提交不算完成，因此不能打印！";

        public const string NoConfig = "脱机状态不能做高级配置！";
        public const string NoNewBlock = "脱机状态不能预制构件!";
        public const string NoReferOrder = "脱机状态不能引用医嘱!";
        public const string NoAddPicture = "脱机状态不能添加专用图谱!";
        public const string NoDeletePicture = "脱机状态不能删除专用图谱!";
        public const string NoLoggedDoctor = "脱机状态, 未曾注册过的医师不能在脱机状态下操作!";
        public const string NoArchive = "脱机状态不能做归档操作！";
        public const string NoUnlock = "脱机状态不能执行病历开封！";
        public const string NoUncommit = "脱机状态不能执行返修记录！";
        public const string NoLockEmr = "脱机状态不能执行封档操作！";
        public const string NoOffline = "脱机状态只能执行病历记录的编辑操作！";

        public const string MustGraphicsFile = "请选择图形文件!";
        public const string ExistSamePicName = "相同文件已存在!";

        #region Maintain note template
        public const string NoSameTemplateName = "新模板名称不能与现有重名!";
        public const string ConfirmDeleteTemplate = "确认删除模板之后，不能再恢复！";
        public const string NoNewTemplate = "脱机状态不能建立模版！";
        public const string NoUpdateTemplate = "脱机状态不能修改模版！";
        public const string NoDeleteTemplate = "脱机状态不能删除模版！";
        #endregion

        public const string ConfirmDeleteBlock = "确认删除构件之后，不能再恢复！";
        public const string ConfirmDeleteNote = "确认删除记录之后，不能再恢复！";
        public const string NoSameBlockName = "新建构件名称不能与现有重名!";

        public const string NoSavePhrases = "编辑结果尚未保存，保存吗？";
        public const string RemovePhrasesLevel0 = "删除该节点，它下面的子短语将全部删除，\n\n继续吗？";
        public const string RemovePhrasesLevel1 = "确认短语删除，继续吗？";

        public const string DefaultWorkFolder = "內建工作目录：";

        public const string SexConflict = "性专用模板, 不能使用！";

        #region Order
        public const string InvalidDrugCode = "无效药品编码！";
        public const string NotPharmacy = "当前药房无有此药！";
        public const string UselessDrugCode = "药品库存不够！";
        public const string QuantityOverLimit = "用量超限!";
        public const string DaysOverLimit = "用药天数超限!";
        public const string OKDeleteOrderItem = "确认删除本医嘱项目？";
        public const string OKUpdatePredefinedOrder = "确认更新内置处方，";
        public const string ImageExamPartErr = "检查单多检查项目必须属于同一组！";
        public const string OKDeleteOrder = "确认废除本医嘱！";
        public const string OKClearOrder = "确认清除医嘱簿！";
        public const string InvalidRegistryID =
            "无效门诊号！不能接诊该患者可能原因如下：\r[1] 该患者的挂号信息已经过期。\r[2] 不是本科室的患者。\r[3] 已经被本科室其他医师接诊。\r[4] 该患者已经诊毕。";
        public const string RegistryExpired = "患者的挂号信息已经过期！";
        public const string NoTransferForLowInsurance = "低保患者不能转诊！";
        public const string NoBecomeInForLowInsurance = "低保患者不能转住院！";
        public const string RequireDepartment = "请选择科室！";
        public const string OkUnRegister = "确认退诊";
        public const string OkFinish = "确认诊毕";
        public const string InvalidRegistryID2 = "划卡或门诊号错误，可能原因如下：\r1)卡号错误。\r2)门诊号错误。\r3)挂号已过期。\r4)不是本科室患者。";
        public const string NoCorrectPharmacy = "没有合适的药房！";
        public const string InvalidRegistryID3 = "没有符合条件的患者，请确认\r1) 划卡是否正确。\r2) 该患者是否是本科患者。\r3) 挂号信息是否过期。";
        public const string InvalidBillNum = "无效票据号，请确认票据号输入是否正确！";
        public const string NoTreatItem = "无可退治疗项目!";
        public const string InvalidRegistryID4 = "无效门诊号！不能召回该患者可能原因如下：\r1)划卡是否正确。\r2)该患者是否是本科患者。\r3)挂号信息是否过期。\r4)该患者还没被诊毕。";
        public const string InvalidCardNum = "无效卡号！";
        public const string InvalidTestFormNum = "无效检验单号！";
        public const string InvalidTestItemNum = "没有定义检验单收费模板！";
        public const string InvalidPriceItemCode = "无效收费项目编码！";
        public const string InvalidItemCode = "无效检查项目编码！";
        public const string NoImageCostDefine = "没有定义检查单收费模板！";
        public const string HasFee = "已经有处置或药品费用！";
        public const string NoOrderRecord = "没有医嘱!";
        public const string ConfirmDeletePredefined = "确认删除预定义处方！";

        public const string RequiredDeposit = "必须输入预付款！";
        public const string RequiredArea = "必须选择预定病区！";
        public const string RequiredDiagnose = "必须填写入院诊断！";
        public const string RequiredDoctor = "必须指定主管医生！";
        public const string RequiredDepartment = "必须选择所属科室！";
        #endregion

        public const string ConfirmCancelNewEmrNote = "确认放弃新建病历记录！";  // in pattern defination
        public const string ConfirmCancelSetAudit = "确认放弃审核制度的设置！";

        public const string ConfirmCancelSetOptions = "确认放弃选项设置！";

        public const string IncorrectPhrasesFile = "不是短语导出文件！";
        public const string IncorrectBlocksFile = "不是构件导出文件！";
        public const string CannotImpExp = "离线状态不能执行导入导出操作！";
        public const string CannotSetEnv = "离线状态不能执行环境设置！";
        public const string CannotEditPhrase = "离线状态不能维护短语！";

        public const string OnlineAgain = "服务器恢复响应，已进入正常工作状态！";
        public const string OfflineAgain = "没有响应，已进入离线工作状态！";

        public const string NoNotesInLocal = "本地没有病历记录！";
        public const string ConfirmSync = "确认上传本地病历记录，数据库中将被覆盖？";
        public const string FailedSync = "上传失败!";



        public const string InvalidDateString = "无效日期字串！";
        public const string SyncSuccessful = "同步操作成功！";


        public const string OKSetPrinted = "确认置打印完成标志?";
        public const string SetPrinted = "已置打印完成标志, 同时封档！";
        public const string RePrinted = "已经打印过，再打印吗？";
        public const string EmrLocked = "已经封档，不能再新建病历!";

        public const string RulesNotSaved = "当前记录的评分规则尚未保存，继续吗？";
        public const string OKRemoveRule = "确认删除一条评分规则?";

        public const string SignedNoUncommit = "已经签字的记录不能再返修！";
        public const string SignatureBeEnd = "签名行必须放在最后!";
        public const string SelfCannotUncommit = "不能返修自己提交的记录！";


        public const string InvalidServer = "无效服务器地址：\r";
        public const string Nomarge = "该类记录不能被合并";

    }
    public enum ConfigureMode
    {
        AuthenticLevel = 1,
        MainPasswd,
        AutoArchiveNum
    }
    public struct AuthorInfo
    {
        public string NoteID;
        public string ChildID;
        public string NoteName;
        public string Writer;
        public string WrittenDate;
        public string Checker;
        public string CheckedDate;
        public string FinalChecker;
        public string FinalCheckedDate;
        public string TemplateType;
        public string TemplateName;
        /* In order to have a flexible sign title, 3 attributes are add in emrPattern. */
        public string WriterLable;
        public string CheckerLable;
        public string FinalCheckerLable;
    }

    public struct StringGeneral
    {
        #region Datetime format
        //public const string NoArchiveNum = "?";
        public const string MonthFormat = "MM";
        public const string DateFormat1 = "yyyy-MM-dd";
        public const string DateFormat = "yyyy-MM-dd";
        public const string TimeShortFormat = "hh:mm";
        public const string TimeFormat = "hh:mm:ss";
        public const string DateFormatChs = "yyyy年MM月dd日";
        public const string TimeFormatChs = "HH时mm分";
        public const string TimeFormatChsH = "HH时";
        #endregion
        public const string WinTitle = "众心医疗科技--电子病历书写平台  -- ";
        public const string offlineTitle = "--离线";

        public const string NullEmrDocument = "<Emr RegistryID=\"\" EmrStatus=\"0\" Series=\"0\" />";

        public const string NullCode = "####";
        public const string ChildCode = "++++";
        public const string AllCode = "----";
        public const string supperUser = "0000";
        public const string Both = "Both";
        public const string Big = "Big";
        public const string Small = "Small";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string One = "1";
        public const string Zero = "0";
        public const string RegistryClosed = "-1";
        public const string CR = "\r";
        public const string In = "In";   // inpatient
        public const string Out = "Out";   // outpatient
        public const string TreeNode = "System.Windows.Forms.TreeNode";
        public const string Exit = "退出";
        #region Attribute type values in contents
        public const string Datetime = "Datetime";
        public const string Date = "Date";
        public const string Time = "Time";
        public const string String = "String";
        public const string LineFeed = "CR";
        public const string Space = "Space";

        public const string ConsultDepartment = "Cdepart";
        public const string ConsultDoctor = "Cdoctor";
        public const string FiledDoctor = "Cfiled";
        public const string Cdate = "Cdate";
        public const string Creason = "Creason";
        public const string OperationStartTime = "Ostime";
        public const string OperationEndTime = "Oetime";

        public const string OperationDate = "Odate";
        public const string OperationName = "Oname";
        public const string PreDiagnose = "Bdiag";
        public const string PosDiagnose = "Adiag";
        public const string Surgeon = "Surgeon";
        public const string Assistant = "Sassis";
        public const string Nurse = "Nurse";
        public const string Anesthetist = "Anest";
        public const string Anesthesia = "Away";
        #endregion
        /* Control types */
        public const string ComboBox = "ComboBox";
        public const string CheckBox = "CheckBox";
        #region Subtitle values in subtitles with values
        public const string OperatorName = "Opname";
        public const string PatientName = "Pname";
        public const string DepartmentName = "Dname";
        public const string PatientSex = "Psex";
        public const string PatientAge = "Page";
        public const string PatientMarriage = "Pmarriage";
        public const string PatientLand = "Pland";
        public const string PatientNantion = "Pnation";
        public const string PatientAddr = "Paddr";
        public const string PatientDays = "Pdays";
        public const string PatientJob = "Pjob";
        public const string PatientRegistry = "Preg";
        public const string PatientVisit = "Pvisit";
        public const string PatientDayIn = "Pdayin";
        public const string PatientDayOut = "Pdayout";
        public const string PatientToday = "Ptoday";
        public const string PatientArchiveNum = "Parchive";
        public const string Pbed = "Pbed";
        public const string Now = "Pnow";
        public const string Psnow = "Psnow";
        public const string Ptimein = "Ptimein";
        public const string Phone = "Phone";
        public const string Birthday = "Birthday";
        #endregion
        /* Tests and exams type */
        public const string Test = "test";
        public const string Exam = "exam";
        public const string None = "none";
        /* DoctorOrders */
        public const string Drug = "drug";
        public const string Treat = "treat";

        public const string LongDrug = "longdrug";
        public const string LongTreat = "longtreat";
        public const string TempDrug = "tempdrug";
        public const string TempTreat = "temptreat";

        #region Content control type
        public const string Label = "Label";  // contentControl as subTitle in emrNote
        public const string Control = "Control";  // contentControl as control in emrNote
        public const string RemoveFlag = "1";
        #endregion

        public const string NewBlockName = "新构件";
        public const string DepartmentPhrase = "科室";
        public const string PersonPhrase = "个人";
        public const string EmrCompletedOntime = "按时完成";
        public const string EmrCompleted = "未按时完成";
        public const string EmrNotCompleted = "尚未完成";

        public const string HisDBType = "HisDBType";

        public const string InPatient = "住院部";
        public const string OutPatient = "门诊部";
        public const string Insurable = "医保项目";
        public const string NotInsurable = "非医保项目";
        public const string MedicalFinished = "诊毕";
        public const string OutRegistryID = "门诊号";
        public const string ReadCardNum = "读卡号";
        public const string Insure = "医保";
        public const string UnInsure = "普通";

        public const string Discharged = "出院";
        public const string QuasiDischarged = "出科";
        public const string Unknown = "不详";
        public const string logonTitle = "医生注册";
        public const string taskPaneTitle = "患者清单";
        #region note template type
        public const string PersonTemplate = "1";
        public const string DepartTemplate = "2";
        public const string HospitalTemplate = "3";
        public const string NoneTemplate = "0";
        public const string Department = "科室模版";
        public const string Person = "个人模版";
        public const string Hospital = "全院模版";
        #endregion

        #region Web service server
        public const string hisService = "emrPatients.asmx";
        public const string emrService = "emrServiceXml.asmx";
        public const string ordService = "EmrOrder.asmx";
        public const string HisServer = "HIS服务器";
        public const string EmrServer = "EMR服务器";
        #endregion

        #region EmptySpace
        public const string SignSpace = "        ";
        #endregion
    }
    public struct IntGeneral
    {
        public const int DocumentMax = 30;
        public const int LableMax = 8;

        public const int blockSize = 32000;
        public const int TaskPaneWidth = 340;
    }

    public struct NoteBarSize
    {
        public const int Width = 410;
        public const int Height = 20;
        public const int HeightForBigHeader = 65;
        public const int HeightForSmallHeader = 45;
        public const int HeightForNoHeader = 20;
    }
    public struct ElementNames
    {
        /* Emr Document */
        public const string AssignCheckRight = "AssignCheckRight";
        public const string AdmissionNote = "AdmissionNote";
        public const string InitialNote = "InitialNote";
        public const string ProgressNote = "ProgressNote";
        public const string OperatorArchieve = "OperatorArchieve";
        public const string ClinicPath = "ClinicPath";
        public const string PathID = "PathID";

        public const string TrasferIn = "TrasferIn";
        public const string TransferOut = "TransferOut";
        public const string HandOver = "HandOver";
        public const string TakeOver = "TakeOver";
        public const string Summary = "Summary";
        public const string BeforeOperation = "BeforeOperation";
        public const string AfterOperation = "AfterOperation";
        public const string DischageNote = "DischageNote";
        public const string DeathDiscussion = "DeathDiscussion";
        public const string ArchieveLocked = "ArchieveLocked";
        public const string InformSpecial = "InformSpecial";

        public const string Consultation = "Consultation";
        public const string Consultations = "Consultations";
        public const string Anaesthesia = "Anaesthesia";
        public const string OperationAssistance = "Assistance";
        public const string Nurse = "Nurse";
        public const string Anaesthetist = "Anaesthetist";

        public const string NoteText = "NoteText";
        public const string Table = "Table";
        public const string NoteImage = "NoteImage";
        public const string Revisions = "Revisions";
        public const string RevisionBy = "RevisionBy";
        public const string Section = "Section";
        public const string Text = "Text";
        public const string ArchiveManagement = "ArchiveManagement";
        public const string CommitTime = "CommitTime";
        public const string ConsentID = "ConsentID";
        public const string Consent = "Consent";
        public const string UseNewTemp = "UseNewTemp";
        public const string PrintTop = "PrintTop";
        public const string PrintEnd = "PrintEnd";
        public const string RemindReview = "RemindReview";
        public const string DelGrid = "DelGrid";
        public const string bctime = "bctime";
        public const string ShowTime = "ShowTime";
       
        public const string ArchiveDepartment = "ArchiveDepartment";
        public const string ArchiveDepartmentText = "ArchiveDepartmentText";

        public const string QualityScore = "QualityScore";

        //public const string RefreshInf = "RefreshInf";
        public const string BA = "BA";
        public const string SubTitle = "SubTitle";
        public const string Control = "Control";
        public const string Item = "Item";
        public const string Link = "Link";
        public const string Phrase = "Phrase";
        public const string PhraseSet = "PhraseSet";
        public const string Content = "Content";
        public const string Merge = "Merge";
        public const string Group = "Group";
        public const string EmrNote = "EmrNote";
        public const string EmrNotes = "EmrNotes";
        public const string SubEmrNote = "SubEmrNote";
        public const string Emr = "Emr";
        public const string Emrs = "Emrs";
        /* Patient List */
        public const string Registry = "Registry";
        public const string Patients = "Patients";
        public const string Patient = "Patient";

        public const string Operators = "Operators";
        public const string Operator = "Operator";

        public const string WorkFolder = "WorkFolder";
        public const string Department = "Department";
        public const string Departments = "Departments";
        public const string WinTitle = "WinTitle";
        /* note template */
        public const string NoteTemplate = "NoteTemplate";
        public const string NoteTemplates = "NoteTemplates";
        /* Doctor Orders */
        public const string TestsAndExams = "TestsAndExams";
        public const string Form = "Form";
        public const string DoctorOrders = "DoctorOrders";
        public const string LongOrder = "LongOrder";
        public const string TempOrder = "TempOrder";
        public const string archive = "archive";

        /* emrblocks */
        public const string EmrBlocks = "EmrBlocks";
        public const string Block = "Block";
        public const string BlocksSet = "BlocksSet";
        public const string Pk = "Pk";
        public const string Pks = "Pks";

        /* PicGallery */
        public const string PicGallery = "PicGallery";
        public const string Picture = "Picture";

        public const string Cell = "Cell";

        public const string Phrases = "Phrases";
        public const string Level0 = "Level0";
        public const string Level1 = "Level1";


        public const string RegistryIDs = "RegistryIDs";
        public const string RegistryID = "RegistryID";

        /* Quantity Information */
        public const string QualityInfo = "QualityInfo";
        public const string Pattern = "Pattern";

        /* Doctor Orders */
        public const string Doctors = "Doctors";
        public const string Doctor = "Doctor";

        /* */
        public const string Style = "Style";

        /* GlobalConfig */
        public const string GlobalConfig = "GlobalConfig";
        public const string AuditSystem = "AuditSystem";
        public const string Password = "Password";
        public const string Keys = "Keys";
        public const string Key = "Key";
        public const string AutoArchive = "AutoArchive";
        public const string PatientInfoAtPageHeader = "AtPageHeader";
        public const string CanEndPrint = "CanEndPrint";
        public const string JHSelfPrint = "JHSelfPrint";
     
        public const string CannotUncommit = "CannotUncommit";
        public const string DischargedEmrCanEdit = "DischargedCanEdit";
        public const string OperatorDepartment = "OperatorDepartment";
        public const string Options = "Options";
        public const string PageFooter = "PageFooter";
        public const string SignRight = "SignRight";
        public const string Sign = "Sign";
        public const string LabelFont = "LabelFont";
        public const string HeaderFont = "HeaderFont";
        public const string NoteNameFont = "NoteNameFont";
        public const string PrintNoView = "PrintNoView";

        public const string Componet = "Componet";
        public const string Manifest = "Manifest";

        public const string QualityControl = "QC";
        public const string LastTop = "LastTop";
        public const string ShowQualityControlInfo = "ShowQCI";
        public const string ShowPatientInfoTime = "ShowPIT";
        public const string ShowOpdownTime = "ShowOPDT";
        public const string PageMargin = "PageMargin";
        public const string EnterCommitTime = "EnterCommitTime";
        public const string OrderByMode = "OrderByMode";

        public const string Expand = "Expand";
        public const string CanEditHeader = "CanEditHeader";
        public const string ShowWritenDate = "ShowWritenDate";
        public const string ShowWriter = "ShowWriter";
        public const string UseCallSystem = "CallSystem";
        public const string LimitInDepartment = "LimitInDepart";
        public const string LimitInDays = "LimitInDays";
        public const string CanWriteNoteInOffline = "WriteNoteInOffline";
        public const string AllowMixOrder = "MixOrder";
        public const string NameAsHeader = "NameAsHeader";
        public const string SpaceAsFooter = "SpaceAsFooter";
        public const string NoPatientInfo = "NoPatientInfo";
        public const string GroupOwner = "GroupOwner";
        public const string TemplateRight = "TemplateRight";
        public const string Pharmacies = "Pharmacies";
        public const string Pharmacy = "Pharmacy";
        public const string DrugUnitMode = "UnitMode";
        public const string ByPoorPatient = "ByPoorPatient";
        public const string BecarefulSeePatient = "BecarefulSeePatient";
        public const string CardUseWay = "CardUseWay";
        public const string FirstVisitNoteID = "FirstVisitNoteID";
        public const string ReturnVisitNoteID = "ReturnVisitNoteID";

        public const string Anesthesia = "Anesthesia";
        public const string AnesthesiaText = "AnesthesiaText";
        public const string ConsultationBHSY = "ConsultationBHSY";
        public const string ChildHeader = "ChildHeader";
        public const string EyesCenter = "EyesCenter";
        public const string UseOldPic = "UseOldPic";
        public const string TimeLock = "TimeLock";
        public const string chxTrace = "chxTrace";
        public const string NoRefOther = "NoRefOther";
        public const string IllTemplate = "IllTemplate";
        public const string UseOldTemplate = "UseOldTemplate";
        public const string Errors = "Errors";
        public const string Error = "Error";
        public const string PersonTemplateRight = "PersonTemplateRight";
        public const string EncryptSign = "EncryptSign";
        public const string OrderNumbers = "OrderNumbers";
        public const string OrderNumber = "OrderNumber";
      
        public const string Header = "Header";
        public const string ArchiveNum = "Archive";
        public const string k16 = "k16";
        public const string Note = "Note";
        public const string CanEditLockedEmr = "EditLockedEmr";
        public const string Reason = "Reason";
        public const string Rules = "Rules";
        public const string Rule = "Rule";
        public const string Flaw = "Flaw";
        public const string ValuateEmr = "ValuateEmr";

        public const string StartTime = "Start";
        public const string Operations = "Operations";
        public const string Operation = "Operation";

        public const string Rescues = "Rescues";
        public const string Rescue = "Rescue";

        public const string AutoPageNumber = "AutoPageNum";
        public const string CommitedAsEnd = "CommitedAsEnd";
        public const string AutoSyncCheck = "AutoSyncCheck";
        public const string ContinueAsEmrOpened = "ContinueAsEmrOpened";
        public const string UseWindowsIdentity = "UseWindowsIdentity";
        public const string UseDigitalSign = "UseDigitalSign";
        public const string UseTemplateNameAsNoteName = "UseTemplateNameAsNoteName";

        public const string PrescriptionNum = "PrescriptionNum";

        public const string Area = "Area";
        public const string Areas = "Areas";

        public const string VisitType = "VisitType";
        public const string HospitalName = "HospitalName";

        public const string EditOffline = "EditOffline";

        public const string Drug = "Drug";
        public const string Drugs = "Drugs";

        public const string MyRoles = "MyRoles";
        public const string Role = "Role";
        public const string Roles = "Roles";
        public const string RoleID = "RoleID";
        public const string Font = "Font";
        public const string Fonts = "Fonts";
        public const string EHRInterface = "EHRInterface";
        public const string RealName = "RealName";
        public const string Revision = "Revision";
        public const string Timeout = "Timeout";
        public const string Congener = "Congener";
        public const string UseNewOp = "UseNewOp";
        public const string GetValue = "GetValue";
        public const string AutoSign = "AutoSign";
        public const string ManualSign = "ManualSign";
        public const string RoleNewMenu = "RoleNewMenu";
        public const string OperationRight = "OperationRight";
        public const string Blood = "Blood";
        public const string BloodID = "BloodID";
        public const string OperationID = "OperationID";
        public const string UKey = "UKey";

        public const string Special = "Special";
        public const string SpecialText = "SpecialText";

    }
    public struct AttributeNames
    {
        /* Emr Document */
        public const string ShowName = "ShowName";
        public const string Birthday = "Birthday";
        public const string Both = "Both";
        public const string LevelCode = "LevelCode";
        public const string RegistryID = "RegistryID";
        public const string RegistryDate = "RegistryDate";
        public const string DischargedDate = "DischargedDate";
        public const string RegistryTime = "RegistryTime";
        public const string TimeLimit = "Limit";
        public const string Merge = "Merge";
        public const string BelongDepartment = "BelongDepartment";
        /*----------------2009-07-13 modify by guojt-----------------*/
        public const string DoctorType = "DoctorType";
        /*------------------------------------------------------------*/
        /*----------------2009-07-06 modify by sunfan-----------------*/
        public const string SingleContinue = "Single";
        /*------------------------------------------------------------*/
        public const string IllType = "IllType";
        public const string IllName = "IllName";
        //  public const string Type = "Type";已经定义过
        public const string TypeName = "TypeName";
        public const string Record = "Record";//多人写同一份病历

        public const string Routine = "Routine";
        public const string StartTime = "StartTime";
        public const string Score = "Score";
        public const string None = "None";
        public const string EmrStatus = "EmrStatus";
        public const string PatientStatus = "PatientStatus";
        public const string LastWriteTime = "Lwt";
        public const string Series = "Series";
        public const string NoteID = "NoteID";
        public const string NoteName = "NoteName";
        public const string RealName = "RealName";
        public const string Header = "Header";
        public const string Unique = "Unique";
        public const string NoteStatus = "NoteStatus";
        public const string WriterID = "WriterID";
        public const string Writer = "Writer";
        public const string ChildID = "ChildID";
        public const string WrittenDate = "WrittenDate";
        public const string WrittenTime = "WrittenTime";
        public const string CommitDate = "CommitDate";
        public const string CommitTime = "CommitTime";
        public const string SecondTime = "SecondTime";
        public const string CheckerID = "CheckerID";
        public const string Checker = "Checker";
        public const string CheckedDate = "CheckedDate";
        public const string FinalCheckerID = "FinalCheckerID";
        public const string FinalChecker = "FinalChecker";
        public const string FinalCheckedDate = "FinalCheckedDate";
        public const string Required = "Required";
        public const string Valid = "Valid";
        public const string FontName = "FontName";
        public const string Color = "Color";
        public const string Size = "Size";
        public const string Start = "Start";
        public const string End = "End";
        /* Attributes in subtitle */
        public const string Type = "Type";
        public const string Space = "Space";
        public const string SpaceCount = "SpaceCount";
        public const string TitleName = "TitleName";
        public const string CR = "CR";
        public const string Nurse = "Nurse";
        /* Emr Document--Table */
        public const string TableStart = "TableStart";
        public const string TableEnd = "TableEnd";
        public const string RowCount = "RowCount";
        public const string ColumnCount = "ColCount";
        public const string CellHeight = "Height";
        public const string CellWidth = "Width";
        public const string LineLocation = "LineLocation";
        public const string LineStyle = "LineStyle";
        public const string LineColor = "LineColor";
        public const string LineWidth = "LineWidth";
        public const string CellBackColor = "BackColor";
        public const string CellText = "CellText";
        public const string CellForeColor = "ForeColor";
        public const string CellFontName = "FontName";
        public const string CellFontSize = "FontSize";
        public const string CellRowIndex = "Row";
        public const string CellColIndex = "Col";
        /* Emr Document--NoteImage */
        public const string ImageStart = "ImageStart";
        public const string ImageEnd = "ImageEnd";
        /* Patient List */
        public const string PatientName = "PatientName";
        public const string ArchiveNum = "ArchiveNum";
        public const string Sex = "Sex";
        public const string Birth = "Birth";
        public const string Age = "Age";
        public const string AgeUnit = "AgeUnit";
        public const string Nation = "Nation";
        public const string MaritalStatus = "MaritalStatus";
        public const string NativePlace = "NativePlace";
        public const string Job = "Job";
        public const string Address = "Address";
        public const string Phone = "Phone";
        public const string BedNum = "BedNum";
        //public const string RegistryDate = "RegistryDate";
        public const string DoctorID = "DoctorID";
        public const string DepartmentCode = "DepartmentCode";
        public const string Code = "Code";
        public const string Name = "Name";
        public const string Spell = "Spell";
        public const string Last = "Last";
        public const string Pk = "pk";
        /* Department List */

        /* archive*/
        public const string ProName = "ProName";
        public const string SZF = "SZF";
        public const string SZFHJ = "SZFHJ";
        public const string Content = "Content";
        public const string GLMC = "GLMC";
        public const string CKKF = "CKKF";
        public const string XH = "XH";

        /* Doctor Orders */
        public const string Num = "Num";
        public const string Doctor = "Doctor";
        public const string Date = "Date";
        public const string Value = "Value";
        public const string Unit = "Unit";
        public const string ValueUnit = "ValueUnit";
        public const string Result = "Result";
        public const string Quantity = "Quantity";
        public const string Price = "Price";
        public const string Cost = "Cost";
        public const string HowUse = "HowUse";
        public const string HowOften = "HowOften";
        public const string StopDate = "StopDate";
        public const string State = "State";
        public const string HowLong = "HowLong";

        /* in emrpattern. */
        public const string Sign1 = "Sign1";
        public const string Sign2 = "Sign2";
        public const string Sign3 = "Sign3";
        public const string Style = "Style";
        public const string ParentID = "ParentID";
        /* emr block */
        public const string Category = "Category";

        /*  */
        public const string Condition = "Condition";

        public const string In = "In";   // inpatient
        public const string Out = "Out";   // outpatient

        public const string CardType = "CardType";
        public const string CanBuyDrug = "CanBuyDrug";
        public const string CardNum = "CardNum";
        public const string LowInsurance = "LowInsurance";

        public const string Bold = "Bold";
        public const string Italic = "Italic";

        public const string TecqTitle = "TecqTitle";
        public const string TitleLevel = "TitleLevel";

        public const string Level = "Level";
        public const string Mode = "Mode";
        public const string DateTime = "Datetime";

        public const string LastTop = "Top";
        public const string LastTop2 = "Top2";
        public const string LastTop3 = "Top3";
        public const string LastPageNumber = "PageNumber";
        public const string LastPageNumber2 = "PageNumber2";
        public const string LastPageNumber3 = "PageNumber3";

        public const string Auto = "Auto";

        public const string Message = "message";

        public const string MarginLeft = "Left";
        public const string MarginTop = "Top";
        public const string MarginRight = "Right";
        public const string MarginBottom = "Bottom";

        public const string HostName = "HostName";
        public const string HostIP = "HostIP";

        public const string Printed = "Printed";
        public const string DepartmentName = "Dname";
        public const string Mark = "Mark";
        public const string Knockoff = "Knockoff";
        public const string Substitute = "Substitute";
        public const string Late = "Late";

        public const string PSCYRQ = "PSCYRQ";
        public const string PSRQ = "PSRQ";
        public const string PJ = "PJ";
        public const string PSZJYS = "PSZJYS";
        public const string XMXH = "XMXH";
        public const string QXMC = "QXMC";
        public const string QXMCXH = "QXMCXH";
        public const string KF = "KF";
        public const string BAPSBZ = "BAPSBZ";
        public const string SHBZ = "SHBZ";
        public const string SHRQ = "SHRQ";
        public const string SHRY = "SHRY";
        public const string SDF = "SDF";
        public const string GKF = "GKF";
        public const string KFXM = "KFXM";
        public const string KFYY = "KFYY";

        public const string A = "A";
        public const string B = "B";
        public const string C = "C";
        public const string D = "D";

        public const string English = "English";
        public const string Chinise = "Chinise";

        public const string Normal = "Normal";
        public const string Chronic = "Chronic";
        public const string Bad = "Bad";
        public const string Critical = "Critical";
        public const string RestTime = "RestTime";

        public const string Sequence = "Sequence";
        public const string RescueSequence = "ResSequence";
        public const string TransferInSequence = "TinSequence";
        public const string TransferOutSequence = "ToutSequence";
        public const string TakeOverSequence = "TakeSequence";
        public const string FileDoctorID = "FileDoctorID";

        public const string NoteIDSeries = "NoteIDSeries";
        public const string AreaCode = "AreaCode";

        public const string Way = "Way";
        public const string Frequency = "Frequency";
        public const string TotalDays = "TotalDays";
        public const string Mix = "Mix";

        public const string RoleID = "RoleID";
        public const string RoleName = "RoleName";

        public const string Kind = "Kind";
        public const string PrintCount = "Count";
        public const string DayFrom = "From";
        public const string DayTo = "To";

        public const string Reason = "Reason";

        /* HIS values */
        public const string OperationName = "Oname";
        public const string PreDiagnose = "PreDiag";
        public const string ProDiagnose = "ProDiag";
        public const string Surgeon = "Surgeon";
        public const string AnaesthesiaWay = "Away";
    }
    public enum EmrStatus
    {
        Pending = 0,
        Locked
    }

    public struct MenuStatus
    {

        public const string Writer = "0";          // 病历修改中
        public const string Reader = "1";          // 病历阅读
        public const string TempViewer = "2";      // 查看模板
        public const string Close = "3";           // 关闭病历
        public const string None = "4";         //使用病历
        public const string Manager = "5";         //主任以上查看病历
      
    }
    public struct BlockStatus
    {
        public const string Bnone = "0";
        public const string Bnew = "1";
        public const string Bclose = "2";
        public const string Bsave = "3";
        public const string Blist = "4";
    }
    public enum PermissionLevel
    {
        NoPrivilege = 0,
        ReadOnly,
        RevisionOnly,
        ReadWrite,
        Trust,
        FinalRevisionOnly
    }
    public enum WhiteSpace
    {
        Word = 0,
        Space,
        CR
    }
    public struct EmrManagement
    {
        public const string RegisterTime = "RegTime";
        public const string OperationTime = "OpeTime";
        public const string RescueTime = "RscTime";

        public const string EmrCompletedOntime = "按时完成";
        public const string EmrCompleted = "未按时完成";
        public const string EmrNotCompleted = "尚未完成";
        public const string TimeLimitUnit = " 小时";
    }
    public struct AboutOrders
    {
        public const string UsePreviousPrescription = "使用前一个处方";
        public const string UsePredefinedPrescription = "使用处方库";
        public const string AddPredefinedPrescription = "添加处方库";
        public const string PredefinedPrescription = "处方库";
        public const string UsePersonGrug = "使用个人药名库";
        public const string AddPersonGrug = "添加个人药名库";
        public const string CommitOrder = "提交";
        public const string SaveOrder = "保存";
        public const string PrescriptionNum = "处方号：";
        public const string ExaminationNum = "检查单号：";
        public const string TestNum = "化验单号：";
        public const string NewOrderTitle = "开立医嘱";
        public const string NoCommitOrder = "医嘱尚未提交，继续吗？";
        public const string OrderNotEmpty = "处方内有项目，如果继续将清除处方，继续吗？";
        public const string FromPredefinedOrder = "1";
        public const string NotFromPredefinedOrder = "0";
        public const string PrivateDrugNames = "个人常用药品名";
        public const string AddItem = "加入处方";
        public const string RemoveItems = "清除处方";

        public const string WestDrugInput = "WestDrugInput";
        public const string ChineseDrugInput = "ChineseDrugInput";
        public const string Examination = "Examination";
        public const string Test = "Test";
        public const string Treatment = "Treat";
        public const string ImageEnhance = "Enhance";

        public const string PrescriptionChinese = "处方";
        public const string ExaminationChinese = "检查单";
        public const string TestChinese = "化验单";
        public const string TreatChinese = "治疗单";
        public const string Anaesthesia = "y";
        public const string NoAnaesthesia = "n";
    }
    public struct DrugKind
    {
        public const string TreatOrder = "T";
        public const string West = "W";
        public const string Chinese = "C";
    }
    public struct ExamKind
    {
        public const string ImageExam = "Image";
        public const string PhysicalExam = "Physical";
        public const string TestExam = "Test";
        public const string Treat = "Treat";
        public const string Prescript = "OrderItems";
        public const string Fee = "Fee";
    }
    public struct ClassCode
    {
        public const string Image = "10000002";
        public const string Test = "10000006";
    }
    public struct DataColumnNames
    {
        public const string SpellCode = "SpellCode";
        public const string Description = "Description";
        public const string DoctorID = "DoctorID";
        public const string DepartmentCode = "DepartmentCode";
        public const string OrderType = "OrderType";

        public const string ItemCode = "ItemCode";
        public const string ItemName = "ItemName";
        public const string Spec = "Spec";
        public const string QuantityLimit = "QuantityLimit";
        public const string Insurable = "Insurable";
        public const string InsureLimit = "Insurable";
        public const string Unit = "Unit";
        public const string Price = "Price";
        public const string InsureGroup = "InsureGroup";
        public const string OutGroup = "OutGroup";
        public const string Content = "Content";
        public const string QuantityUnit = "QuantityUnit";
        public const string NoBuy = "NoBuy";
        public const string Total = "Total";
        public const string TimesText = "TimesText";
        public const string TimesDaily = "TimesDaily";
        public const string Days = "Days";
        public const string DaysTag = "DaysTag";
        public const string WayCode = "WayCode";
        public const string WayText = "WayText";
        public const string HerbalCopys = "HerbalCopys";
        public const string Quantity = "Quantity";
        public const string RatifierID = "RatifierID";
        public const string DoctorExhort = "DoctorExhort";
        //adw_prescription.object.yzsj[ll_row] = is_yzsj	//医嘱时间
        public const string DoseDirect = "DoseDirect";
        public const string UnitTag = "UnitTag";
        public const string OrderPk = "OrderPk";
    }
    public struct PrivateGrug
    {
        public const string DrugKindCode = "dl";
        public const string DrugKindName = "dlmc";
        public const string DrugCode = "bm";
        public const string DrugName = "pm";
        public const string DoctorID = "ysbm";
    }
    public struct DrugList
    {
        public const string Code = "bm";
        public const string Name = "名称";
        public const string Package = "规格";
        public const string Quantity = "zhs";
        public const string Unit = "dw3";
        public const string Kind = "dl";
        public const string Spell = "pym";
        public const string Insurable = "医保";

        public const string SellUnit = "发药单位";
        public const string SellPrice = "发药价格";
        public const string UseUnit = "执行单位";
        public const string UsePrice = "执行价格";

        public const string Dose = "rzdyl";
        public const string WayCode = "zxfsbm";
        public const string TimesCode = "yzpcbm";
    }
    public struct TimeList
    {
        public const string Code = "text";
        public const string Name = "description";
        public const string Days = "days";
        public const string Times = "count";
    }
    public struct WayList
    {
        public const string Code = "code";
        public const string Name = "text";
        public const string FeeCode = "feeCode";
    }
    public struct TypeList
    {
        public const string Name = "name";
        public const string Spell = "pym";
        public const string Class = "class";
        public const string Subclass = "subclass";
    }
    public struct Icd10List
    {
        public const string Spell = "pym";
        public const string Name = "jbfl";
        public const string Icd = "icd";
    }
    public struct NameList
    {
        public const string Name = "name";
        public const string Spell = "pym";
        public const string Code = "code";
        public const string Class = "class";
        public const string Subclass = "subclass";
        public const string FeeIndex = "mbxh";
        public const string EnhancementFeeIndex = "zqmbxh";
        public const string Sample = "sample";
    }
    public struct TreatOrderList
    {
        public const string Spell = "pym";
        public const string Name = "pm";
        public const string Code = "bm";
        public const string Unit = "dw";
        public const string Insurable = "bxbs";
        public const string Price = "jg1";
    }
    public struct TreatOrder
    {
        public const string Spell = "pym";
        public const string Name = "mbmc";
        public const string Code = "mbxh";
        public const string Quantity = "sl";
        public const string WantDate = "kdrq";
    }
    public struct TreatOrderDetail
    {
        public const string Code = "sfxmbm";
        public const string Quantity = "sl";
    }
    public struct TabPrescription
    {
        public const string TimesText = "yfyl";
        public const string MixFlag = "zybj";
        public const string Quantity = "ypyl";
        public const string ContentQuantity = "zhs";
        public const string ContentQuantityUnit = "hldw";
        public const string TimesDaily = "timesDaily";
        public const string Days = "ts";
        public const string WayCode = "fsbm";
        public const string WayText = "zxfs";
        public const string PrescriptionNum = "cfh";
        public const string NoBuy = "zbybs";
        public const string DoctorExhort = "yzzs";
        public const string Ratifier = "ylsp";
        public const string DrugCode = "bm";
        public const string DrugName = "pm";
        public const string PharmacyCode = "yfbm";
        public const string Total = "cfsl";
        public const string PriceUnit = "dw";
        public const string Price = "jg";
        public const string CopyCount = "cffs";
        public const string Charged = "sfbs";
        public const string UnCharged = "tfbs";
        public const string Done = "fybs";
        public const string Aneathsia = "dmbs";
        public const string DayBegin = "hjrq";
        public const string LicenseNum = "pzwh";
        public const string SellUnit = "dw1";
        public const string UseUnit = "dw2";
        public const string PriceUnitMode = "unitMode";
        public const string BeginDate = "begin";
        public const string DrugKind = "kind";
    }
    public struct TabImageExam
    {
        public const string SubClass = "yxflbm";
        public const string PartCode = "yxbwbm";
        public const string PartName = "yxbwmc";
        public const string Want = "jcyq";
        public const string Diagnose = "lczd";
        public const string Enhance = "enhance";
        public const string AlterDiagnose = "qtyxzd";
        public const string Class = "dh";
        public const string Urgent = "jyzt";
        public const string RequisitionDate = "kdrq";
        public const string FormSequence = "xh";
        public const string FormName = "kdmc";
        public const string Charged = "charged";
        public const string Done = "zxqk";
    }
    public struct TabPhysicalExam
    {
        public const string ItemCode = "bm";
        public const string ItemName = "pm";
        public const string Want = "jcyq";
        public const string Goal = "jcmd";
        public const string Diagnose = "lczd";
        public const string Subclass = "jcdflbm";
        public const string Urgent = "jyzt";
        public const string Class = "dh";

        public const string WantDate = "kdrq";
        public const string FormName = "kdmc";
        public const string FormSequence = "xh";
        public const string Done = "zxqk";
        public const string Charged = "charged";
        public const string DoctorID = "kdys";
        public const string DepartmentCode = "kdks";
    }
    public struct TabTreatCost
    {
        public const string Code = "bm";
        public const string Quantity = "quantity";
        public const string PrescriptionNum = "cfh";
        public const string Locked = "locked";

        public const string Name = "pm";
        public const string Price = "jg";
        public const string Unit = "dw";
        public const string Total = "sl";
        public const string UnCharged = "tfbs";
        public const string Charged = "sfbs";
        public const string Cost = "je";
        public const string Opcode = "hjry";
        public const string OpDepartmentCode = "hjks";
    }
    public struct TabTreatOrder
    {
        public const string PrescriptionNum = "PrescriptionNum";
        public const string OrderSequence = "OrderSequence";
        public const string Quantity = "Quantity";
        public const string Name = "mbmc";
        public const string Price = "jg";
        public const string Unit = "dw";
        public const string Total = "sl";
        public const string UnCharged = "tfbs";
        public const string Charged = "sfbs";
        public const string Cost = "je";
        public const string Opcode = "hjry";
        public const string OpDepartmentCode = "hjks";
    }
    public struct TabTestExam
    {
        public const string FormNum = "jydbh";
        public const string Sample = "jybb";
        public const string FormName = "kdmc";
        public const string Want = "jcyq";
        public const string Goal = "jymd";
        public const string Diagnose = "lczd";
        public const string Urgent = "jyzt";
        public const string Class = "dh";
        public const string RequisitionNum = "jysqxh";
        public const string SampleDate = "cqrq";
        public const string FormSequence = "xh";
        public const string Done = "zxqk";
        public const string Charged = "charged";
        public const string RequisitionDate = "sqrq";
        public const string DoctorID = "kdys";
        public const string DepartmentCode = "kdks";
    }
    public struct CommonInfoBase
    {
        public const string DepartmentCode = "ksbm";
        public const string DoctorID = "ysbm";
        public const string CardType = "CardType";
        public const string CanBuyDrug = "CanBuyDrug";
        public const string CardNum = "kh";
        //public const string LowInsurance = "LowInsurance";      
    }
    public struct CommonInfoOut
    {
        public const string Sex = "hzxb";
        public const string RegistryID = "mzh";
        public const string RegistryDate = "ghrq";
        public const string PatientName = "hzxm";
        public const string Age = "hznl";
        public const string AgeUnit = "nldw";
        public const string RegistryStatus = "zlzt";
        public const string ArchiveNum = "icno";
        public const string Repeat = "fzbs";
        public const string PoorPatient = "PoorPatient";
        public const string RegistryType = "hbbm";
        public const string PatientType = "hzlx";
    }
    public struct CommonInfoIn
    {
        public const string Sex = "xb";
        public const string RegistryID = "zyh";
        public const string RegistryDate = "zyrq";
        public const string PatientName = "xm";
        public const string Age = "nl";
        public const string Bed = "ch";
        public const string Diagnose = "mzzdmc";
        public const string Room = "bffh";
    }
    public struct TabRooms
    {
        public const string Number = "number";
        public const string Total = "total";
        public const string Price = "price";
        public const string Sex = "sex";
    }
    public struct TabBeds
    {
        public const string Room = "bffh";
        public const string Bed = "bcbm";
        public const string Flag = "zcbs";
    }
    public struct OIType
    {
        public const string OrderItem = "WordAddInEmrw.order.OrderItem";
        public const string OrderItemExam = "WordAddInEmrw.order.OrderItemExam";
    }
    public enum OIStatus
    {
        Normal = 0,
        Charged,
        Done,
        Uncharged
    }
    public enum EnhanceMode
    {
        Required = 0,
        Option,
        None
    }
    public struct Auditing
    {
        public const string LevelA = "A";
        public const string LevelB = "B";
        public const string LevelC = "C";
    }
    public struct AuditSystem
    {
        public const string A1 = "A1";
        public const string A2 = "A2";
        public const string A3 = "A3";
        public const string B1 = "B1";
        public const string B2 = "B2";
        public const string B3 = "B3";
    }
    public enum AuditLevelSystem
    {
        GroupHeader = 1,
        ChiefDoctor
    }
    public enum TitleLevel
    {
        Nothing = 0,
        ChiefDoctor,
        ViceChiefDoctor,
        AttendingDoctor,
        NormalDoctor,
        InternDoctor,
        ExternDoctor,
        HouseDoctor
    }
    public struct StartTime
    {
        public const string Registry = "Registry";          // 入科时间
        public const string Operation = "Operation";        // 手术结束时间
        public const string TransferIn = "TransferIn";      // 转入时间
        public const string TransferOut = "TransferOut";     // 转科时间
        public const string Discharged = "Discharged";      // 出科时间
        public const string Rescued = "Rescued";            // 抢救结束时间
        public const string Dead = "Dead";                  // 死亡时间
        public const string TakeOver = "TakeOver";          // 接班时间
        public const string Monthly = "Monthly";            // 每月一次
        public const string Routine = "Routine";            // 每隔 n 天一次
        public const string Consult = "Consult";            // 会诊申请时间
        public const string None = "None";
    }
    public enum IllDegree
    {
        Normal = 0,
        Chronic,
        Bad,
        Critical
    }
    public struct QualityInfo
    {
        public string registryID;
        public string noteID;
        public string noteName;
        public DateTime startTime;
        public DateTime writenTime;
        public int limit;
        public string score;
    }
    public struct TabBeInpatient
    {
        public const string WorkDate = "workDate";
        public const string InType = "type";
        public const string Deposit = "deposit";
        public const string Area = "area";
        public const string Diagnose = "diagnose";
        public const string ChiefDoctorID = "chiefDoctor";
        public const string ChiefDepartmentCode = "chiefDepartment";
        public const string ChiefTel = "chiefTel";
        public const string WorkDoctorID = "workDoctor";
        public const string WorkDepartmentCode = "workDepartment";
        public const string WorkTel = "workTel";
        public const string Agent = "agent";
        public const string AgentTel = "agentTel";
    }
    public struct InpatientStatus
    {
        public const string Stay = "0";
        public const string Leave = "1";
    }
    public struct Delimiters
    {
        public const char Space = ' ';
        public const char Colon = ':';
        public const char Seperator = '|';
        public const char Slash = '\\';
        public const char Cut = '/';
        /// <summary>
        /// /gjt2009/6/1
        /// </summary>
        public const char ColonChn = '：';
    }
    public class ValuateText
    {
        public ValuateText() { }
        public string[] Text = { "甲", "乙", "丙", "不合格" };
    }
    public struct ValuateIndex
    {
        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;
    }
    public struct Groups
    {
        public const string Zero = "0";
        public const string One = "1";
        public const string Two = "2";
        public const string Three = "3";
    }
    public struct EventType
    {
        //public const string Admission = "A";            //	入院	
        //public const string AdmissionAgain = "A1";      //	再次或多次入院	
        public const string TransferOut = "C1";         //	转出	
        public const string TransferIn = "C2";          //	转入	
        public const string HandOver = "D1";            //	交班	
        public const string TakeOver = "D2";	        //  接班	
        //public const string Operation = "E";	        //  手术	
        public const string Rescued = "F";              //	抢救	
        public const string Dead = "G";	                //  死亡	
        public const string Dead24 = "G1";              //	24小时内入院死亡	
        //public const string Discharged = "H";          //	出院	
        //public const string Discharged24 = "H1";         //	24小时内入出院	
    }
    public struct Condition
    {
        public const string Dead = "Dead";
        public const string Operation = "Operation";
        public const string Hours48 = "48";
        public const string Hours72 = "72";
    }
    public class PatientInfo
    {
        public string ArchiveNum;
        public string PatientName;
        public string Sex;
        public string Birth;
        public string Age;
        public string AgeUnit;
        public string Nation;
        public string MaritalStatus;
        public string NativePlace;
        public string Job;
        public string Address;
        public string RegistryDate;
        public string OutRegistryDate;
        public string RegistryTime;
        public string DepartmentCode;
        public string BedNum;
        public string RegistryID;
        public string PatientType;
        public string CardNum;
        public string CardType;
        public string CanBuyDrug;
        public string LowInsurance;
        public string DischargedDate;
        public string Phone;
        public string Birthday;
        public PatientInfo()
        {
            Birthday = "";
            ArchiveNum = "";
            PatientName = "";
            Sex = "";
            Birth = "";
            Age = "";
            AgeUnit = "";
            Nation = "";
            MaritalStatus = "";
            NativePlace = "";
            Job = "";
            Address = "";
            RegistryDate = "";
            OutRegistryDate = "";
            RegistryTime = "";
            DepartmentCode = "";
            BedNum = "";
            RegistryID = "";
            PatientType = "";
            CardNum = "";
            CardType = "";
            CanBuyDrug = "";
            LowInsurance = "";
            DischargedDate = "";
        }
    }
    public delegate void GetData(DataTable dt, bool End);
    //public delegate void LoadPatientTree(XmlNode Patients);
    public delegate void OpenTemp(XmlNode template,long pk,string type,bool view);
    public delegate void MessShow(string msg, double d, MsgTpe mt);
    public delegate void OperPic(SqlOperations sqlop,XmlNode pic);
    public delegate int DeIsValuate(string RegiditID);
    public delegate bool PutEmrPatternIntoDB(XmlNode pattern);
    public delegate string DeGetPrintInfo(int printCount, ref XmlNode printInfo);
    public delegate void DeResetBeginTime();
}

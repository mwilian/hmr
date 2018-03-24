using System;
using System.Collections.Generic;
using Word = Microsoft.Office.Interop.Word;
using System.Text;
using System.IO;
using System.Xml;
using EmrConstant;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using HuronControl;

namespace CommonLib
{
    public class ivkey
    {
        public byte[] key = { 207,138,168,206,140,141,7,3,85,230,53,208,68,5,28,41,175,
            169,117,248,11,51,32,216,123,157,218,230,23,225,76,113 };
        public byte[] IV = { 169, 249, 29, 194, 113, 8, 21, 9, 56, 191, 91, 116, 41, 57, 26, 15 };
    }
    public partial class udt
    {
        public static string NormalizeRegistryID(string registryID)
        {
            return registryID.PadLeft(8, '0');
        }
        public static string NormalizeOpcode(string opcode)
        {
            string mycode = null;
            char[] bs = opcode.ToCharArray();
            int len = bs.Length;
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i] > 65247)
                {
                    mycode += Convert.ToChar(bs[i] - 65248);
                }
                else
                {
                    mycode += bs[i];
                }
            }
            return mycode;
        }

        public static bool IsValidByte(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[0-9]*$");
        }
        public static bool IsNumaric(string text)
        {
            if (text == null) return false;
            char[] cText = text.ToCharArray();
            Byte zero = (Byte)'0';
            Byte nine = (Byte)'9';
            Byte dot = (Byte)'.';
            for (int i = 0; i < cText.Length; i++)
            {
                Byte bit = (Byte)cText[i];
                if (bit < zero || bit > nine)
                {
                    if (bit != dot) return false;
                }
            }
            return true;
        }
        //public static void GetEmrDocument(string RegistryID, ref  XmlNode node, ref  XmlNode emrNotes)
        //{
        //    using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
        //    {
        //        es.GetEmrDocument(RegistryID, ref node, ref emrNotes);

        //    }
        //}

        public static string MakeWdDocumentFileName(string location, string noteIDSeries)
        {
            return Path.Combine(location, noteIDSeries + ".notx");
        }
        public static string MakePatientListFileName(string opcode)
        {
            if (Globals.inStyle)
            {
                Globals.patientFile = Path.Combine(Globals.patientListFolder, opcode + EmrConstant.ResourceName.PatientsXml);
            }
            else
            {
                string today = DateTime.Today.ToString(EmrConstant.StringGeneral.DateFormat);
                Globals.patientFile = Path.Combine(Globals.patientListFolder, today);
                if (!Directory.Exists(Globals.patientFile)) Directory.CreateDirectory(Globals.patientFile);
                Globals.patientFile = Path.Combine(Globals.patientFile, opcode + EmrConstant.ResourceName.PatientsXml);
            }
            return Globals.patientFile;
        }
        public static string MakeWdDocumentFileName(string registryID, string noteID, int series, string workFolder)
        {
            string wdDocPath = GetDocLocation(registryID, workFolder);
            string filename = MakeNoteIDSeries(noteID, series) + ".notx";
            return Path.Combine(wdDocPath, filename);
        }
        public static string GetDocLocation(string registryID, string workFolder)
        {
            string location = Path.Combine(workFolder, registryID);
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);
            return location;
        }
        public static string MakeEmrDocumentFileName(string registryID, string workFolder)
        {
            string wdDocPath = GetDocLocation(registryID, workFolder);
            return Path.Combine(wdDocPath, registryID + ".note");
        }
        public static string MakeBlockDocumentName(int blockPk, string blockName)
        {
            return blockName + "$" + blockPk + ".blk";
        }
        public static string MakeBlockDocumentPath(int blockPk, string blockName, string category, string blockFolder)
        {
            string categoryPath = Path.Combine(blockFolder, category);
            if (!Directory.Exists(categoryPath)) Directory.CreateDirectory(categoryPath);
            return Path.Combine(categoryPath, MakeBlockDocumentName(blockPk, blockName));
        }
        public static string MakeNoteIDSeries(string noteID, int series)
        {
            string sseries = series.ToString().PadLeft(8 - noteID.Length, '0');
            return noteID + sseries;
        }
        public static bool StringToWordDocument(string docName, XmlNode emrNote)
        {
            try
            {
                if (File.Exists(docName)) File.Delete(docName);
                FileStream noteStream = new FileStream(docName, FileMode.Create);
                if (noteStream == null)
                {
                    return false;
                }
                int size = emrNote.InnerText.Length;
                byte[] noteContent = new byte[size];
                noteContent = Convert.FromBase64String(emrNote.InnerText);
                noteStream.Write(noteContent, 0, noteContent.Length);
                noteStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Globals.logAdapter.Record("EX756987458864", ex.Message + ">>" + ex.ToString(), true);            
             
                return false;
            }
        }
        /* ------------------------------------------------------------------------------------
       * Convert the word document file into string
       * Parameters:
       *     string wordDocName -- file name
       * Return char string
       * ------------------------------------------------------------------------------------ */
        public static string WordDocumentToString(string wordDocName)
        {
            if (!File.Exists(wordDocName)) return null;

            FileStream docContent = File.OpenRead(wordDocName);
            long fileSize = docContent.Length;
            byte[] content = new byte[fileSize];
            int readCount = docContent.Read(content, 0, (int)fileSize);
            docContent.Close();
            return Convert.ToBase64String(content);
        }

        /* ---------------------------------------------------------------------------------------
         * Write the word window into a file.
         * Parameters:
         *     Word.Document doc -- object of word document window
         *     string fileName -- destination file name(fullpath)
         *     bool encode -- destination file will be cryptical
         * --------------------------------------------------------------------------------------- */
        //public static bool SaveWordDoc(Word.Document doc, string fileName, bool encode)
        //{
        //    if (fileName.Length == 0) return false;
        //    object oMissing = System.Reflection.Missing.Value;
        //    object wdDocName = fileName;
        //    doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
        //       ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //       ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

        //    /* Unlink the filename from acctive documenet. */
        //    wdDocName = EmrConstant.ResourceName.MyDocName;
        //    doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
        //        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
        //        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

        //    if (encode) EncodeEmrDocument(fileName);

        //    return true;
        //}
      
        #region Encrypt
        private static ICryptoTransform SaDecoder()
        {
            ivkey ik = new ivkey();
            SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
            ICryptoTransform saDecoder = sa.CreateDecryptor(ik.key, ik.IV);
            return saDecoder;
        }
        private static ICryptoTransform SaEncoder()
        {
            ivkey ik = new ivkey();
            SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
            ICryptoTransform saEncoder = sa.CreateEncryptor(ik.key, ik.IV);
            return saEncoder;
        }

        /* ------------------------------------------------------------------------------------
         * Encode the data buffer 
         * Parameters:
         *     byte[] indata      -- sourse data
         *     ref byte[] outdata -- destination data
         * Return true if succefully, else false
         * ------------------------------------------------------------------------------------ */
        public static bool EmrEncoder(byte[] indata, ref byte[] outdata)
        {

            ICryptoTransform encoder = SaEncoder();
            if (encoder == null) return EmrConstant.Return.Failed;
            byte[] ret = encoder.TransformFinalBlock(indata, 0, indata.Length);
            if (ret.Length > 0)
            {
                outdata = (byte[])ret.Clone();
                return EmrConstant.Return.Successful;
            }
            return EmrConstant.Return.Failed;
        }
        /* ------------------------------------------------------------------------------------
         * Decode the data buffer 
         * Parameters:
         *     byte[] indata      -- sourse data
         *     ref byte[] outdata -- destination data 
         * Return true if succefully, else false
         * ------------------------------------------------------------------------------------ */
        public static bool EmrDecoder(byte[] indata, ref byte[] outdata)
        {
            ICryptoTransform decoder = SaDecoder();
             if (decoder == null) return EmrConstant.Return.Failed;
            byte[] ret = decoder.TransformFinalBlock(indata, 0, indata.Length);
            if (ret.Length > 0)
            {
                outdata = (byte[])ret.Clone();
                return EmrConstant.Return.Successful;
            }
            return EmrConstant.Return.Failed;
        }

        /* ------------------------------------------------------------------------------------
         * Encode the emr document file 
         * Parameters:
         *     string emrdoc -- Is the file will be encoded 
         * Description: 
         *     emrdoc is a xml file witch includes full text of the emr notes for one patient,
         * except for the data of tables or images.
         *     Only 16 bytes will be emcoded
         * ------------------------------------------------------------------------------------ */
        public static void EncodeEmrDocument(string emrdoc)
        {
            string tmpfile = "tmpfile";
            FileStream old = File.OpenRead(emrdoc);
            FileStream tmp = File.OpenWrite(tmpfile);

            byte[] bytes = new byte[16];
            old.Read(bytes, 0, 16);
            byte[] newbytes = null;
            EmrEncoder(bytes, ref newbytes);
            bytes[0] = (byte)newbytes.Length;
            tmp.Write(bytes, 0, 1);
            tmp.Write(newbytes, 0, newbytes.Length);
            byte[] oldbytes = new byte[EmrConstant.IntGeneral.blockSize];
            while (true)
            {
                int realReadLength = old.Read(oldbytes, 0, EmrConstant.IntGeneral.blockSize);
                if (realReadLength == 0) break;
                tmp.Write(oldbytes, 0, realReadLength);
            }
            old.Close();
            tmp.Close();

            File.Delete(emrdoc);
            File.Move(tmpfile, emrdoc);
        }

      
        #endregion

        public static class jj
        {
            private static DeIsValuate IsValuate = null;
            #region Word document treatment
            /* ------------------------------------------------------------------------------------
         * Restore the word document file from the string witch is from a word document file.
         * Parameters:
         *     string docName  -- word document file name
         *     XmlNode emrNote -- of witch InnerText is a string from a word document file
         * ------------------------------------------------------------------------------------ */

            public static bool StringToWordDocument(string docName, XmlNode emrNote)
            {
                try
                {
                    if (File.Exists(docName)) File.Delete(docName);
                    FileStream noteStream = new FileStream(docName, FileMode.Create);
                    if (noteStream == null)
                    {
                        return false;
                    }
                    int size = emrNote.InnerText.Length;
                    byte[] noteContent = new byte[size];
                    noteContent = Convert.FromBase64String(emrNote.InnerText);
                    noteStream.Write(noteContent, 0, noteContent.Length);
                    noteStream.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX756987459981", ex.Message + ">>" + ex.ToString(), true);            
             
                    return false;
                }
            }

            /* ------------------------------------------------------------------------------------
             * Convert the word document file into string
             * Parameters:
             *     string wordDocName -- file name
             * Return char string
             * ------------------------------------------------------------------------------------ */
            public static string WordDocumentToString(string wordDocName)
            {
                if (!File.Exists(wordDocName)) return null;
                byte[] content = null;
                try
                {
                    FileStream docContent = File.OpenRead(wordDocName);
                    long fileSize = docContent.Length;
                    content = new byte[fileSize];
                    int readCount = docContent.Read(content, 0, (int)fileSize);
                    docContent.Close();
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX756987456636", ex.Message + ">>" + ex.ToString(), true);            
             
                }
                return Convert.ToBase64String(content);
            }

            /* ---------------------------------------------------------------------------------------
             * Write the word window into a file.
             * Parameters:
             *     Word.Document doc -- object of word document window
             *     string fileName -- destination file name(fullpath)
             *     bool encode -- destination file will be cryptical
             * --------------------------------------------------------------------------------------- */
            public static bool SaveWordDoc(Word.Document doc, string fileName, bool encode)
            {
                if (fileName.Length == 0) return false;
                object oMissing = System.Reflection.Missing.Value;
                object wdDocName = fileName;
                doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                /* Unlink the filename from acctive documenet. */
                wdDocName = EmrConstant.ResourceName.MyDocName;
                doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);


                if (encode) EncodeEmrDocument(fileName);
                return true;
            }
            public static bool SaveBlock(Word.Document doc, string fileName, bool encode)
            {
                if (fileName.Length == 0) return false;
                object oMissing = System.Reflection.Missing.Value;
                object wdDocName = fileName;
                doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                /* Unlink the filename from acctive documenet. */
                wdDocName = EmrConstant.ResourceName.NullName;
                doc.SaveAs(ref wdDocName, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                if (encode) EncodeEmrDocument(fileName);
                return true;
            }

            /* ------------------------------------------------------------------------------------
             * Dump the word document window into a word file.
             * Parameters:
             *     string fileName -- The file name into witch the word document window will be writen
             *     bool encode     -- Encodes the file or not
             * ------------------------------------------------------------------------------------ */
            public static bool ExportWordDoc(Word.Document doc, string fileName)
            {
                return SaveWordDoc(doc, fileName, false);
            }
            #endregion

            #region Encrypt
            private static ICryptoTransform SaDecoder()
            {
                ivkey ik = new ivkey();
                SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
                ICryptoTransform saDecoder = sa.CreateDecryptor(ik.key, ik.IV);
                return saDecoder;
            }
            private static ICryptoTransform SaEncoder()
            {
                ivkey ik = new ivkey();
                SymmetricAlgorithm sa = SymmetricAlgorithm.Create();
                ICryptoTransform saEncoder = sa.CreateEncryptor(ik.key, ik.IV);
                return saEncoder;
            }

            /* ------------------------------------------------------------------------------------
             * Encode the data buffer 
             * Parameters:
             *     byte[] indata      -- sourse data
             *     ref byte[] outdata -- destination data
             * Return true if succefully, else false
             * ------------------------------------------------------------------------------------ */
            public static bool EmrEncoder(byte[] indata, ref byte[] outdata)
            {

                ICryptoTransform encoder = SaEncoder();
                if (encoder == null) return EmrConstant.Return.Failed;
                byte[] ret = encoder.TransformFinalBlock(indata, 0, indata.Length);
                if (ret.Length > 0)
                {
                    outdata = (byte[])ret.Clone();
                    return EmrConstant.Return.Successful;
                }
                return EmrConstant.Return.Failed;
            }
            /* ------------------------------------------------------------------------------------
             * Decode the data buffer 
             * Parameters:
             *     byte[] indata      -- sourse data
             *     ref byte[] outdata -- destination data 
             * Return true if succefully, else false
             * ------------------------------------------------------------------------------------ */
            public static bool EmrDecoder(byte[] indata, ref byte[] outdata)
            {
                ICryptoTransform decoder = SaDecoder();
                if (decoder == null) return EmrConstant.Return.Failed;
                byte[] ret = decoder.TransformFinalBlock(indata, 0, indata.Length);
                if (ret.Length > 0)
                {
                    outdata = (byte[])ret.Clone();
                    return EmrConstant.Return.Successful;
                }
                return EmrConstant.Return.Failed;
            }

            /* ------------------------------------------------------------------------------------
             * Encode the emr document file 
             * Parameters:
             *     string emrdoc -- Is the file will be encoded 
             * Description: 
             *     emrdoc is a xml file witch includes full text of the emr notes for one patient,
             * except for the data of tables or images.
             *     Only 16 bytes will be emcoded
             * ------------------------------------------------------------------------------------ */
            public static void EncodeEmrDocument(string emrdoc)
            {
                string tmpfile = "tmpfile";
                FileStream old = File.OpenRead(emrdoc);
                FileStream tmp = File.OpenWrite(tmpfile);

                byte[] bytes = new byte[16];
                old.Read(bytes, 0, 16);
                byte[] newbytes = null;
                EmrEncoder(bytes, ref newbytes);
                bytes[0] = (byte)newbytes.Length;
                tmp.Write(bytes, 0, 1);
                tmp.Write(newbytes, 0, newbytes.Length);
                byte[] oldbytes = new byte[EmrConstant.IntGeneral.blockSize];
                while (true)
                {
                    int realReadLength = old.Read(oldbytes, 0, EmrConstant.IntGeneral.blockSize);
                    if (realReadLength == 0) break;
                    tmp.Write(oldbytes, 0, realReadLength);
                }
                old.Close();
                tmp.Close();

                File.Delete(emrdoc);
                File.Move(tmpfile, emrdoc);
            }

            /* ------------------------------------------------------------------------------------
             * Decode the emr document file 
             * Parameters:
             *     string emrdoc  -- Is the encoded file 
             *     string tmpfile -- Is the result of decoding
             * ------------------------------------------------------------------------------------ */
            public static void DecodeEmrDocument(string emrdoc, string tmpfile)
            {
                try
                {
                    FileStream old = File.OpenRead(emrdoc);
                    FileStream tmp = File.Create(tmpfile);

                    byte[] bytes = new byte[1];
                    old.Read(bytes, 0, 1);
                    int encryptedLength = (int)bytes[0];
                    byte[] encryptedBytes = new byte[encryptedLength];
                    old.Read(encryptedBytes, 0, encryptedLength);
                    byte[] sourceBytes = null;
                    EmrDecoder(encryptedBytes, ref sourceBytes);

                    tmp.Write(sourceBytes, 0, sourceBytes.Length);

                    byte[] oldbytes = new byte[EmrConstant.IntGeneral.blockSize];
                    while (true)
                    {
                        int realReadLength = old.Read(oldbytes, 0, EmrConstant.IntGeneral.blockSize);
                        if (realReadLength == 0) break;
                        tmp.Write(oldbytes, 0, realReadLength);
                    }
                    old.Close();
                    tmp.Close();
                }
                catch (Exception ex)
                {
                    Globals.logAdapter.Record("EX756987454416", ex.Message + ">>" + ex.ToString(), true);            
             
                   
                }
            }
            #endregion


            public static double MyToDoubleZero(object value)
            {
                if (Convert.IsDBNull(value)) return 0.0;

                string svalue = value.ToString();
                if (svalue.Length == 0) return 0.0;

                for (int k = 0; k < svalue.Length; k++)
                    if ((svalue[k] < '0' || svalue[k] > '9') && svalue[k] != '.') return 0.0;

                return Convert.ToDouble(value);
            }

            public static double MyToDoubleOne(object value)
            {
                if (Convert.IsDBNull(value)) return 1.0;
                string svalue = value.ToString();
                for (int k = 0; k < svalue.Length; k++)
                    if ((svalue[k] < '0' || svalue[k] > '9') && svalue[k] != '.') return 1.0;

                return Convert.ToDouble(value);
            }

            public static bool IsNumaric(string text)
            {
                if (text == null) return false;
                char[] cText = text.ToCharArray();
                Byte zero = (Byte)'0';
                Byte nine = (Byte)'9';
                Byte dot = (Byte)'.';
                for (int i = 0; i < cText.Length; i++)
                {
                    Byte bit = (Byte)cText[i];
                    if (bit < zero || bit > nine)
                    {
                        if (bit != dot) return false;
                    }
                }
                return true;
            }

            public static int MyToIntOne(object value)
            {
                if (Convert.IsDBNull(value)) return 1;
                return Convert.ToInt32(value);
            }

            public static string MyToStringForMoney(object value)
            {
                return MyToDoubleZero(value).ToString("#.00");
            }

            public static void LoadTreeviewWithPatients(TreeView tvPatients, string patientFile, bool inStyle, bool expandPatientTree)
            {
                if (!File.Exists(patientFile)) return;

                using (XmlReader reader = XmlReader.Create(patientFile))
                {
                    /* Suppress repainting the TreeView until all the objects have been created.*/
                    tvPatients.BeginUpdate();
                    tvPatients.Nodes.Clear();
                    //tvPatients.ForeColor = Color.SteelBlue;
                    //tvPatients.ImageList = imageList1;
                    XmlReader reader1 = XmlReader.Create(patientFile);
                    int i = 0;
                    /* no patients in the list */
                    if (!reader.ReadToFollowing(ElementNames.Patient) && !reader1.ReadToFollowing(ElementNames.Patient))
                    {
                        tvPatients.EndUpdate();
                        reader.Close();
                        reader1.Close();
                        return;
                    }
                    do
                    {

                        string sex = reader.GetAttribute(AttributeNames.Sex);
                        // string patientInfo = reader.GetAttribute(AttributeNames.PatientName) + " ";


                        reader1.ReadToFollowing(EmrConstant.ElementNames.Registry);

                        string DepartmentCode = reader1.GetAttribute(EmrConstant.AttributeNames.DepartmentCode);
                        string DepartmentName = GetDepartmentNameFromCode(DepartmentCode, "D:\\emrw\\linkList\\科室名单.xml");

                        //string patientInfo = reader1.GetAttribute(EmrConstant.AttributeNames.BedNum) +
                        //Delimiters.Space + reader.GetAttribute(AttributeNames.PatientName) +
                        //Delimiters.Space;

                        string patientInfo = reader1.GetAttribute(EmrConstant.AttributeNames.BedNum) +
                        Delimiters.Space + reader.GetAttribute(AttributeNames.PatientName) +
                        Delimiters.Space + DepartmentName + Delimiters.Space;


                        //DateTime birth = Convert.ToDateTime(reader.GetAttribute(EmrConstant.AttributeNames.Birth));
                        //int age = DateTime.Today.Year - birth.Year;
                        //patientInfo += age.ToString() + "岁";
                        patientInfo += reader.GetAttribute(AttributeNames.Age);
                        patientInfo += reader.GetAttribute(AttributeNames.AgeUnit);
                        tvPatients.Nodes.Add(patientInfo);
                        tvPatients.Nodes[i].Tag = reader.GetAttribute(AttributeNames.ArchiveNum);
                        if (inStyle) tvPatients.Nodes[i].Name = sex;
                        else tvPatients.Nodes[i].Name = reader.GetAttribute(AttributeNames.LowInsurance);
                        if (sex == "男") tvPatients.Nodes[i].ImageIndex = 3;
                        else tvPatients.Nodes[i].ImageIndex = 4;

                        /* child nodes (level 1)*/
                        bool firstRegistry = true;
                        reader.ReadToFollowing(EmrConstant.ElementNames.Registry);
                        do
                        {
                            string registryID = reader.GetAttribute(EmrConstant.AttributeNames.RegistryID);
                            string bedNum = reader.GetAttribute(EmrConstant.AttributeNames.BedNum);
                            string doctorID = reader.GetAttribute(EmrConstant.AttributeNames.DoctorID);

                            string itemtext = null;
                            string patientStatus = null;
                            if (inStyle)
                            {
                                itemtext = bedNum + ":" + registryID + ":" + doctorID;
                                patientStatus = reader.GetAttribute(AttributeNames.PatientStatus);
                                if (patientStatus != null && patientStatus.Length > 0)
                                {
                                    itemtext += ":" + patientStatus;
                                    if (firstRegistry)
                                    {
                                        tvPatients.Nodes[i].Text += " " + patientStatus;
                                        firstRegistry = false;
                                    }
                                }
                                else
                                {
                                    if (firstRegistry) firstRegistry = false;
                                }
                            }
                            else
                            {
                                itemtext = ":" + registryID + ":" + doctorID;
                                if (reader.GetAttribute(EmrConstant.AttributeNames.State) == "1")
                                    itemtext += ":" + EmrConstant.StringGeneral.MedicalFinished;
                            }

                            tvPatients.Nodes[i].Nodes.Add(itemtext);

                            if (patientStatus != null && patientStatus.Length > 0)
                            {
                                tvPatients.Nodes[i].LastNode.ForeColor = Color.DimGray;
                                tvPatients.Nodes[i].LastNode.ImageIndex = 1;
                            }
                            else
                            {
                                tvPatients.Nodes[i].LastNode.ForeColor = Color.FromArgb(55, 55, 55);
                                tvPatients.Nodes[i].LastNode.ImageIndex = 7;
                            }
                            tvPatients.Nodes[i].LastNode.Tag = EmrConstant.StringGeneral.RegistryClosed;
                        } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Registry) && reader1.ReadToNextSibling(EmrConstant.ElementNames.Registry));
                        i++;
                    } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Patient));
                    reader.Close();
                    reader1.Close();
                    if (expandPatientTree) tvPatients.ExpandAll();
                    // Begin repainting the TreeView.
                    tvPatients.EndUpdate();
                }
            }

            /**********************zzl 20110908******************************/
            public static void LoadTreeviewWithPatients(TreeView tvPatients, string patientFile,
                string departmentFile, bool inStyle, bool expandPatientTree, DeIsValuate deis, ref int j)
            {
                if (!File.Exists(patientFile)) return;

                IsValuate = deis;

                int n = 0;
                string sex, DepartmentCode, DepartmentName, patientInfo, registryID,
                            bedNum, doctorID, Sequence, itemtext, patientStatus;

                TreeNode treeNode = null;

                XmlDocument doc = new XmlDocument();

                doc.Load(patientFile);

                XmlNodeList list = doc.DocumentElement.SelectNodes(ElementNames.Patient);
                int i;
                for (i = j; i < list.Count; i++)
                {
                    XmlNode node = list[i];
                    bool firstRegistry = true;
                    sex = node.Attributes[AttributeNames.Sex].Value.ToString();
                    XmlNodeList childlist = list[i].SelectNodes(ElementNames.Registry);
                    DepartmentCode = childlist[0].Attributes[EmrConstant.AttributeNames.DepartmentCode].Value.ToString();
                    DepartmentName = GetDepartmentNameFromCode(DepartmentCode, departmentFile);
                    patientInfo = childlist[0].Attributes[EmrConstant.AttributeNames.BedNum].Value.ToString() +
                                  Delimiters.Space + node.Attributes[AttributeNames.PatientName].Value +
                                  Delimiters.Space + DepartmentName + Delimiters.Space;

                    patientInfo += node.Attributes[AttributeNames.Age].Value;
                    patientInfo += node.Attributes[AttributeNames.AgeUnit].Value;
                    tvPatients.Nodes.Add(patientInfo);
                    treeNode = tvPatients.Nodes[i];
                    treeNode.Tag = node.Attributes[AttributeNames.ArchiveNum].Value;
                    if (inStyle) treeNode.Name = sex;
                    else treeNode.Name = childlist[0].Attributes[AttributeNames.LowInsurance].Value.ToString();
                    if (sex == "男") treeNode.ImageIndex = 3;
                    else treeNode.ImageIndex = 4;


                    for (int k = 0; k < childlist.Count; k++)
                    {
                        itemtext = null;
                        patientStatus = null;
                        Sequence = "";
                        registryID = childlist[k].Attributes[EmrConstant.AttributeNames.RegistryID].Value;
                        bedNum = childlist[k].Attributes[EmrConstant.AttributeNames.BedNum].Value;
                        doctorID = childlist[k].Attributes[EmrConstant.AttributeNames.DoctorID].Value;
                        if (childlist[k].Attributes[EmrConstant.AttributeNames.Sequence] != null)
                            Sequence = childlist[k].Attributes[EmrConstant.AttributeNames.Sequence].Value;
                        itemtext = bedNum + ":" + registryID + ":" + doctorID;
                        if (!string.IsNullOrEmpty(Sequence))
                            itemtext = itemtext + ":" + Sequence;
                        if (inStyle)
                        {
                            patientStatus = childlist[k].Attributes[AttributeNames.PatientStatus].Value;
                            if (patientStatus != null && patientStatus.Length > 0)
                            {
                                itemtext += ":" + patientStatus;
                                if (patientStatus == "出院:已归档")
                                {
                                    patientStatus = "出院";
                                }
                                if (firstRegistry)
                                {
                                    treeNode.Text += " " + patientStatus;
                                    firstRegistry = false;
                                }
                            }
                            else
                            {
                                if (firstRegistry) firstRegistry = false;
                            }
                        }
                        else
                        {
                            if (childlist[k].Attributes[EmrConstant.AttributeNames.State].Value == "1")
                                itemtext += ":" + EmrConstant.StringGeneral.MedicalFinished;
                        }

                        treeNode.Nodes.Add(itemtext);

                        if (patientStatus != null && patientStatus.Length > 0)
                        {
                            treeNode.LastNode.ForeColor = Color.DimGray;
                            treeNode.LastNode.ImageIndex = 1;
                        }
                        else
                        {
                            treeNode.LastNode.ForeColor = Color.FromArgb(55, 55, 55);
                            treeNode.LastNode.ImageIndex = 7;
                        }
                        n = IsValuate(registryID);
                        if (n == 0 || n == 1 || n == 2)
                        {
                            treeNode.ForeColor = Color.Red;
                            treeNode.LastNode.BackColor = Color.LemonChiffon;
                        }
                        else if (n == 3)
                        {
                            treeNode.ForeColor = Color.Red;
                            treeNode.LastNode.BackColor = Color.PeachPuff;
                        }
                        treeNode.LastNode.Tag = EmrConstant.StringGeneral.RegistryClosed;
                    }

                    if (i >= 50)
                    {
                        j = i;
                        break;
                    }
                }
                if (expandPatientTree) tvPatients.ExpandAll();
                tvPatients.EndUpdate();

            }
            /*---------------------------------------------------------------------------------------------
          * Overwrite the method of LoadTreeviewWithPatients for adding the information of department's name
          * to information of patient
          *                                                        Modify by Sunfan
          *                                                                2009/06/29
         ----------------------------------------------------------------------------------------------*/
            public static void LoadTreeviewWithPatients(TreeView tvPatients, string patientFile, string departmentFile, bool inStyle, bool expandPatientTree, DeIsValuate deis)
            {
                if (!File.Exists(patientFile)) return;
                IsValuate = deis;
                using (XmlReader reader = XmlReader.Create(patientFile))
                {
                    /* Suppress repainting the TreeView until all the objects have been created.*/
                    tvPatients.BeginUpdate();
                    tvPatients.Nodes.Clear();
                    //tvPatients.ForeColor = Color.SteelBlue;
                    //tvPatients.ImageList = imageList1;
                    XmlReader reader1 = XmlReader.Create(patientFile);
                    int i = 0;
                    /* no patients in the list */
                    if (!reader.ReadToFollowing(ElementNames.Patient) && !reader1.ReadToFollowing(ElementNames.Patient))
                    {
                        tvPatients.EndUpdate();
                        reader.Close();
                        reader1.Close();
                        return;
                    }
                    do
                    {

                        string sex = reader.GetAttribute(AttributeNames.Sex);
                        // string patientInfo = reader.GetAttribute(AttributeNames.PatientName) + " ";


                        reader1.ReadToFollowing(EmrConstant.ElementNames.Registry);

                        string DepartmentCode = reader1.GetAttribute(EmrConstant.AttributeNames.DepartmentCode);
                        string DepartmentName = GetDepartmentNameFromCode(DepartmentCode, departmentFile);

                        string patientInfo = reader1.GetAttribute(EmrConstant.AttributeNames.BedNum) +
                        Delimiters.Space + reader.GetAttribute(AttributeNames.PatientName) +
                        Delimiters.Space + DepartmentName + Delimiters.Space;


                        //DateTime birth = Convert.ToDateTime(reader.GetAttribute(EmrConstant.AttributeNames.Birth));
                        //int age = DateTime.Today.Year - birth.Year;
                        //patientInfo += age.ToString() + "岁";
                        patientInfo += reader.GetAttribute(AttributeNames.Age);
                        patientInfo += reader.GetAttribute(AttributeNames.AgeUnit);
                        tvPatients.Nodes.Add(patientInfo);
                        tvPatients.Nodes[i].Tag = reader.GetAttribute(AttributeNames.ArchiveNum);
                        if (inStyle) tvPatients.Nodes[i].Name = sex;
                        else tvPatients.Nodes[i].Name = reader.GetAttribute(AttributeNames.LowInsurance);
                        if (sex == "男") tvPatients.Nodes[i].ImageIndex = 3;
                        else tvPatients.Nodes[i].ImageIndex = 4;

                        /* child nodes (level 1)*/
                        bool firstRegistry = true;
                        reader.ReadToFollowing(EmrConstant.ElementNames.Registry);
                        do
                        {
                            string registryID = reader.GetAttribute(EmrConstant.AttributeNames.RegistryID);
                            string bedNum = reader.GetAttribute(EmrConstant.AttributeNames.BedNum);
                            string doctorID = reader.GetAttribute(EmrConstant.AttributeNames.DoctorID);
                            string Sequence = "";

                            //if (reader.HasAttributes(EmrConstant.AttributeNames.Sequence) ==true)
                            //{
                            Sequence = reader.GetAttribute(EmrConstant.AttributeNames.Sequence);
                            //}

                            string itemtext = null;
                            string patientStatus = null;
                            if (inStyle)
                            {
                                itemtext = bedNum + ":" + registryID + ":" + doctorID;
                                patientStatus = reader.GetAttribute(AttributeNames.PatientStatus);
                                if (patientStatus != null && patientStatus.Length > 0)
                                {
                                    itemtext += ":" + patientStatus;

                                    //////////////////20110212 ancl/////////////////
                                    if (patientStatus == "出院:已归档")
                                    {
                                        patientStatus = "出院";
                                    }
                                    if (firstRegistry)
                                    {
                                        tvPatients.Nodes[i].Text += " " + patientStatus;
                                        firstRegistry = false;
                                    }
                                }
                                else
                                {
                                    if (firstRegistry) firstRegistry = false;
                                }
                            }
                            else
                            {
                                itemtext = ":" + registryID + ":" + doctorID;
                                if (reader.GetAttribute(EmrConstant.AttributeNames.State) == "1")
                                    itemtext += ":" + EmrConstant.StringGeneral.MedicalFinished;
                            }
                            itemtext = itemtext + ":" + Sequence;

                            tvPatients.Nodes[i].Nodes.Add(itemtext);
                            //////////////////20100514/////////////////

                            if (patientStatus != null && patientStatus.Length > 0)
                            {
                                tvPatients.Nodes[i].LastNode.ForeColor = Color.DimGray;
                                tvPatients.Nodes[i].LastNode.ImageIndex = 1;
                            }
                            else
                            {
                                tvPatients.Nodes[i].LastNode.ForeColor = Color.FromArgb(55, 55, 55);
                                tvPatients.Nodes[i].LastNode.ImageIndex = 7;
                            }
                            int n = IsValuate(registryID);
                            if (n == 0 || n == 1 || n == 2)
                            {
                                tvPatients.Nodes[i].ForeColor = Color.Red;
                                tvPatients.Nodes[i].LastNode.BackColor = Color.LemonChiffon;
                            }
                            else if (n == 3)
                            {
                                tvPatients.Nodes[i].ForeColor = Color.Red;
                                tvPatients.Nodes[i].LastNode.BackColor = Color.PeachPuff;
                            }
                            tvPatients.Nodes[i].LastNode.Tag = EmrConstant.StringGeneral.RegistryClosed;
                        } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Registry) && reader1.ReadToNextSibling(EmrConstant.ElementNames.Registry));
                        i++;
                    } while (reader.ReadToNextSibling(EmrConstant.ElementNames.Patient));
                    reader.Close();
                    reader1.Close();
                    if (expandPatientTree) tvPatients.ExpandAll();
                    // Begin repainting the TreeView.
                    tvPatients.EndUpdate();
                }
            }


            public static string GetDepartmentNameFromCode(string departmentCode, string departmentFile)
            {
                string departmentName = "";
                if (!File.Exists(departmentFile)) return departmentName;
                using (XmlReader reader = XmlReader.Create(departmentFile))
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.Close();
                        return departmentName;
                    }
                    reader.ReadToFollowing(ElementNames.Department);
                    do
                    {
                        if (reader.GetAttribute(AttributeNames.Code) == departmentCode)
                        {
                            departmentName = reader.GetAttribute(AttributeNames.Name);
                            reader.Close();
                            break;
                        }
                    } while (reader.ReadToNextSibling(ElementNames.Department));
                    reader.Close();
                }
                return departmentName;
            }

            //public static string GetDoctorNameFromCode(string doctorID, string doctorFile)
            //{
            //    string doctorName = "";
            //    if (!File.Exists(doctorFile)) return doctorName;
            //    using (XmlReader reader = XmlReader.Create(doctorFile))
            //    {
            //        if (reader.IsEmptyElement)
            //        {
            //            reader.Close();
            //            return doctorFile;
            //        }
            //        reader.ReadToFollowing(ElementNames.Doctor);
            //        do
            //        {
            //            if (reader.GetAttribute(AttributeNames.Code) == doctorID)
            //            {
            //                doctorName = reader.GetAttribute(AttributeNames.Name);
            //                reader.Close();
            //                break;
            //            }
            //        } while (reader.ReadToNextSibling(ElementNames.Doctor));
            //        reader.Close();
            //    }
            //    return doctorName;
            //}

            public static string GetDoctorNameFromCode(string doctorID, XmlNode doctors)
            {
                foreach (XmlNode doctor in doctors)
                {
                    if (doctor.Attributes[AttributeNames.Code].Value == doctorID)
                        return doctor.Attributes[AttributeNames.Name].Value;
                }
                return null;
            }

            public static XmlNode DoctorsAndTheirPatients(string doctorFile, XmlNode patients)
            {
                if (!File.Exists(doctorFile)) return null;
                if (patients == null) return null;


                XmlDocument doc = new XmlDocument();
                doc.Load(doctorFile);

                XmlElement depart = doc.CreateElement(ElementNames.Department);

                foreach (XmlNode patient in patients.SelectNodes(ElementNames.Patient))
                {
                    //string registryID = patient.FirstChild.Attributes[AttributeNames.RegistryID].Value;

                    string doctorID = patient.FirstChild.Attributes[AttributeNames.DoctorID].Value;
                    XmlNode doctor = FindDoctor(doctorID, depart, doc.DocumentElement);
                    XmlElement newpatient = doctor.OwnerDocument.CreateElement(ElementNames.Patient);
                    newpatient.SetAttribute(AttributeNames.PatientName,
                        patient.Attributes[AttributeNames.PatientName].Value);
                    newpatient.InnerXml = patient.InnerXml;

                    //newpatient.SetAttribute(AttributeNames.RegistryID, registryID);
                    doctor.AppendChild(newpatient);

                }

                return depart;
            }
            private static XmlNode FindDoctor(string doctorID, XmlNode depart, XmlNode doctors)
            {
                foreach (XmlNode doctor in depart.SelectNodes(ElementNames.Doctor))
                {
                    if (doctor.Attributes[AttributeNames.Code].Value == doctorID) return doctor;
                }
                XmlElement newDoctor = depart.OwnerDocument.CreateElement(ElementNames.Doctor);
                newDoctor.SetAttribute(AttributeNames.Code, doctorID);
                newDoctor.SetAttribute(AttributeNames.Name, GetDoctorNameFromCode(doctorID, doctors));
                depart.AppendChild(newDoctor);
                return newDoctor;
            }

            public static string AgeUnitFromCode(string code)
            {
                switch (code.ToUpper())
                {
                    case "Y":
                        return "岁";
                    case "M":
                        return "月";
                    case "D":
                        return "天";
                }
                return "岁";
            }

            public static string NormalizeRegistryID(string registryID)
            {
                return registryID.PadLeft(8, '0');
            }

            public static void LoadlogAdapter()
            {
                Globals.logAdapter = new LogAdapter(Application.StartupPath + "\\Log", true); //方法1
                //logAdapter = new LogAdapter(Application.StartupPath + "\\logfile.log"); //方法2
                Globals.logAdapter.Record("日志记录开始", "123456789", true);
  
            }
        }

    }
}

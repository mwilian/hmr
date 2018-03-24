using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using CommonLib;
using EmrConstant;
using System.Drawing.Imaging;

namespace EMR
{
    public partial class PicGallery : Form
    {
        private int picCount;
        private Size picSize;
        private int gapx = 5;
        private int gapy = 5;
        private PictureBox selectedPicBox = null;
        private bool depart = true;
        private string picGalleryFile;
        private XmlNode dptPic;
        private XmlNode psnPic;
        public object linkToFile = false;
        public object saveWithDocument = true;
        public string templateFile = "";

        public PicGallery(XmlNode dPic,XmlNode pPic)
        {
            InitializeComponent();
            picSize = new Size(80, 60);
            dptPic = dPic;
            psnPic = pPic;
            picGalleryFile = Globals.picGalleryFile;
            LoadPicture(dptPic);  
        }
        private void LoadPicture(XmlNode Pic)
        {
            plContain.Controls.Clear();
            if (Pic==null) return;
            picCount = -1;
            XmlDocument doc = new XmlDocument();
            XmlNodeList pictures = Pic.ChildNodes;
            foreach (XmlNode picture in pictures)
            {
                string picName = picture.Attributes[EmrConstant.AttributeNames.Name].Value;
                string text = picture.InnerText;
                MemoryStream mem = new MemoryStream(Convert.FromBase64String(text));
                Image image = Image.FromStream(mem);
                InsertPicture(picName, image);
            }
        }
        private void InsertPicture(string picName, Image image)
        {
            /* Calculate the position for the new picture */
            int n = picCount + 1;
            int posx = gapx;
            if (n % 2 == 1) posx = picSize.Width + gapx + gapx;
            int posy = (picSize.Height + gapy) * (int)(n / 2);
            /* Draw the new picture */
            PictureBox pb = new PictureBox();
            pb.Location = new System.Drawing.Point(posx, posy);
            pb.Size = picSize;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Image = image;
            pb.Name = picName;
            pb.ContextMenuStrip = PicMenu;

            plContain.Controls.Add(pb);
            Label lb = new Label();
            lb.Name = lb.Text = picName;
            lb.Location = new System.Drawing.Point(posx, posy+60);
            plContain.Controls.Add(lb);
            /* set event handlers */
            pb.DoubleClick += new EventHandler(pb_DoubleClick);
            pb.MouseEnter += new EventHandler(pb_MouseEnter);
            pb.MouseLeave += new EventHandler(pb_MouseLeave);
         
            picCount++;
        }
        void pb_MouseLeave(object sender, EventArgs e)
        {
            selectedPicBox = (PictureBox)sender;
            selectedPicBox.BorderStyle = BorderStyle.FixedSingle;
        }

        void pb_MouseEnter(object sender, EventArgs e)
        {
            if (selectedPicBox != null) selectedPicBox.BorderStyle = BorderStyle.None;
        }

        /* -------------------------------------------------------------------------------
         * Insert the selected picture onto word window. 
         --------------------------------------------------------------------------------- */
        void pb_DoubleClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            templateFile = "E:\\MyTemplateFile.bmp";
            PictureBox pb = (PictureBox)sender;
            pb.Image.Save(templateFile,ImageFormat.Bmp);
            this.Close();
        }
        /* -------------------------------------------------------------------------------
         * Add a new picture into the picture gallery. 
         --------------------------------------------------------------------------------- */
        private void addPicture_Click(object sender, EventArgs e)
        {
            if (Globals.offline)
            {
                MessageBox.Show(EmrConstant.ErrorMessage.NoAddPicture, EmrConstant.ErrorMessage.Warning);
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                /* Is it a graphics file? */
                FileInfo fi = new FileInfo(ofd.FileName);
                FileStream fs = fi.OpenRead();
                byte[] bInfo = new byte[2];
                fs.Read(bInfo, 0, 2);

                System.Text.StringBuilder sb = new System.Text.StringBuilder(bInfo.Length);
                foreach (byte b in bInfo) sb.Append(Convert.ToString(b, 16));
                string ss = sb.ToString();
                /* "ffd8"--*.jpeg   "4749"--*.gif   "424d"--*.bmp */
                if (ss != "ffd8" && ss != "4749" && ss != "424d")
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.MustGraphicsFile, EmrConstant.ErrorMessage.Warning);
                    return;
                }

                /* Exist the same graphics name? */
                if (!File.Exists(picGalleryFile)) return;
                string picName = Path.GetFileNameWithoutExtension(ofd.FileName);

                XmlDocument doc = new XmlDocument();
                doc.Load(picGalleryFile);
                XmlNode picGallery = doc.SelectSingleNode(EmrConstant.ElementNames.PicGallery);

                XmlNode picture = FindPictureNode(picName, picGallery);
                if (picture != null)
                {
                    MessageBox.Show(EmrConstant.ErrorMessage.ExistSamePicName, EmrConstant.ErrorMessage.Warning);
                    return;
                }

                /* Before show the picture, it must have been saved in both database and local file */
                if (SavePicture(ofd.FileName))
                {
                    Image image = Image.FromFile(ofd.FileName);
                    InsertPicture(picName, image);
                }
            }
        }
        /* -------------------------------------------------------------------------------
         * Remove a picture fromm the picture gallery. 
         --------------------------------------------------------------------------------- */
        private void deletePicture_Click(object sender, EventArgs e)
        {

            string opcode = Globals.DoctorID;
            if (opcode != "0000")
            {
                MessageBox.Show("只有管理员才能删除图谱，请与网络信息科联系！");
                return;
            }
            if (selectedPicBox == null) return;

            /* Delete an picture from PicGallery */
            DeletePicture(selectedPicBox.Name);

        }
        /* -------------------------------------------------------------------------------------------
         * Save the selected picture to sql server, and update PicGallery file in local client
         --------------------------------------------------------------------------------------------- */
        private Boolean SavePicture(string fileName)
        {
            if (!File.Exists(picGalleryFile)) return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(picGalleryFile);
            XmlNode picGallery = doc.SelectSingleNode(EmrConstant.ElementNames.PicGallery);

            Bitmap bmp = new Bitmap(fileName);
            MemoryStream mem = new MemoryStream();
            bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);
            string text = Convert.ToBase64String(mem.ToArray());

            string picName = Path.GetFileNameWithoutExtension(fileName);
            string departCode = Globals.OpDepartID;

            XmlElement ePicture = doc.CreateElement(EmrConstant.ElementNames.Picture);
            ePicture.SetAttribute(EmrConstant.AttributeNames.Name, picName);
            ePicture.InnerText = text;
            XmlNode nPicture = (XmlNode)ePicture;

            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    if (es.AddPicture(picName, departCode, nPicture) == EmrConstant.Return.Failed)
                    {
                        MessageBox.Show(EmrConstant.ErrorMessage.WebServiceError, EmrConstant.ErrorMessage.Error);
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX925511256768", ex.Message + ">>" + ex.ToString(), true);            
               
                }
            }

            picGallery.AppendChild(nPicture);
            doc.Save(picGalleryFile);
            return true;
        }
        /* -------------------------------------------------------------------------------------------
         * delete the selected picture from sql server
         --------------------------------------------------------------------------------------------- */
        private void DeletePicture(string picName)
        {
            if (!File.Exists(picGalleryFile)) return;
            XmlDocument doc = new XmlDocument();
            doc.Load(picGalleryFile);
            XmlNode picGallery = doc.SelectSingleNode(EmrConstant.ElementNames.PicGallery);
            XmlNode picture = FindPictureNode(picName, picGallery);
            if (picture != null) picGallery.RemoveChild(picture);
            doc.Save(picGalleryFile);
            string departCode;
            departCode = Globals.OpDepartID;
            /* Delete an picture from PicGallery */
            using (gjtEmrService.emrServiceXml es = new gjtEmrService.emrServiceXml())
            {
                try
                {
                    es.DeletePicture(picName, departCode);                    
                }
                catch (Exception ex)
                {

                    Globals.logAdapter.Record("EX925511256767", ex.Message + ">>" + ex.ToString(), true);            
               
                }
            }
            /* Get pictrues from PicGallery for a department */
            for (int i = this.plContain.Controls.Count - 1; i >= 0; i--)
            {
                //if(typeof(PictureBox).GetType()==Controls[i].GetType())
                this.plContain.Controls.RemoveAt(i);
            }
            LoadPicture(ThisAddIn.xmlPicGalleryWriter(Globals.OpDepartID));
        }
        private XmlNode FindPictureNode(string picName, XmlNode picGallery)
        {
            XmlNodeList pictures = picGallery.SelectNodes(EmrConstant.ElementNames.Picture);
            foreach (XmlNode picture in pictures)
            {
                if (picture.Attributes[EmrConstant.AttributeNames.Name].Value == picName)
                    return picture;
            }
            return null;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            PicMenu.Close();
            PicMenu2.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void pnSelector_Click(object sender, EventArgs e)
        {
            LoadPicture(psnPic);  
            depart = false;
        }

        private void dtSelector_Click(object sender, EventArgs e)
        {
            LoadPicture(dptPic);  
            depart = true;
        }

        private void PicMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void plContain_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
     
    }
}

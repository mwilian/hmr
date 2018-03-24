using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
namespace EMR
{
    /// <summary>
    /// WinWordControl allows you to load doc-Files to your
    /// own application without any loss, because it uses 
    /// the real WinWord.
    /// </summary>
    partial class WordContainer : System.Windows.Forms.UserControl
    {

        public WordContainer()
        {

        }

        public void plMain_Resize(object sender, EventArgs e)
        {
            if (wd != null)
            {
                OnResize();
            }
            //throw new NotImplementedException();
        }

        #region "API usage declarations"

        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(int hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "DrawMenuBar")]
        static extern Int32 DrawMenuBar(
            Int32 hWnd
            );

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        static extern Int32 GetMenuItemCount(
            Int32 hMenu
            );

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern Int32 GetSystemMenu(
            Int32 hWnd,
            bool b
            );

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern Int32 RemoveMenu(
            Int32 hMenu,
            Int32 nPosition,
            Int32 wFlags
            );


        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;


        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;

        #endregion



        /* I was testing wheater i could fix some exploid bugs or not.
		 * I left this stuff in here for people who need to know how to 
		 * interface the Win32-API

		[StructLayout(LayoutKind.Sequential)]
			public struct RECT 
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		
		[DllImport("user32.dll")]
		public static extern int GetWindowRect(int hwnd, ref RECT rc);
		
		[DllImport("user32.dll")]
		public static extern IntPtr PostMessage(
			int hWnd, 
			int msg, 
			int wParam, 
			int lParam
		);
		*/


        /// <summary>
        /// Change. Made the following variables public.
        /// </summary>

        public Word.Document document;
        public static Word.ApplicationClass wd = null;
        public static int wordWnd = 0;
        public static string filename = null;
        private static bool deactivateevents = false;

        /// <summary>
        /// needed designer variable
        /// </summary>
       

       

        /// <summary>
        /// cleanup Ressources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            CloseControl();
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Preactivation
        /// It's usefull, if you need more speed in the main Program
        /// so you can preload Word.
        /// </summary>
        public void PreActivate()
        {
            if (wd == null) wd = new Word.ApplicationClass();
        }


        /// <summary>
        /// Close the current Document in the control --> you can 
        /// load a new one with LoadDocument
        /// </summary>
        public void CloseControl()
        {
            /*
            * this code is to reopen Word.
            */

            try
            {
                deactivateevents = true;
                object dummy = null;
                object dummy2 = (object)false;
                try
                {
                    if (document != null)
                    {
                        document.Close(ref dummy, ref dummy, ref dummy);
                    }
                }
                catch (Exception e) { }
                // Change the line below.
                try
                {
                    if (wd != null)
                    {
                        wd.Quit(ref dummy2, ref dummy, ref dummy);
                    }
                }
                catch (Exception e) { }
                deactivateevents = false;
            }
            catch (Exception ex)
            {
                String strErr = ex.Message;
            }
        }


        /// <summary>
        /// catches Word's close event 
        /// starts a Thread that send a ESC to the word window ;)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="test"></param>
        private void OnClose(Word.Document doc, ref bool cancel)
        {
            if (!deactivateevents)
            {
                cancel = true;
            }
        }

        /// <summary>
        /// catches Word's open event
        /// just close
        /// </summary>
        /// <param name="doc"></param>
        private void OnOpenDoc(Word.Document doc)
        {
            OnNewDoc(doc);
        }

        /// <summary>
        /// catches Word's newdocument event
        /// just close
        /// </summary>
        /// <param name="doc"></param>
        private void OnNewDoc(Word.Document doc)
        {
            if (!deactivateevents)
            {
                deactivateevents = true;
                object dummy = null;
                doc.Close(ref dummy, ref dummy, ref dummy);
                deactivateevents = false;
            }
        }

        /// <summary>
        /// catches Word's quit event
        /// normally it should not fire, but just to be shure
        /// safely release the internal Word Instance 
        /// </summary>
        private void OnQuit()
        {
            //wd=null;
        }


        /// <summary>
        /// Loads a document into the control
        /// </summary>
        /// <param name="t_filename">path to the file (every type word can handle)</param>
        public Word.Document LoadDocument(string t_filename)
        {
            deactivateevents = true;
            filename = t_filename;

            if (wd == null) 
                wd = new Word.ApplicationClass();
            try
            {
                wd.CommandBars.AdaptiveMenus = false;
                wd.DocumentBeforeClose += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(OnClose);
                wd.ApplicationEvents4_Event_NewDocument += new Word.ApplicationEvents4_NewDocumentEventHandler(OnNewDoc);
                wd.DocumentOpen += new Word.ApplicationEvents4_DocumentOpenEventHandler(OnOpenDoc);
                wd.ApplicationEvents4_Event_Quit += new Word.ApplicationEvents4_QuitEventHandler(OnQuit);

            }
            catch { }

            if (document != null)
            {
                try
                {
                    object dummy = null;
                    wd.Documents.Close(ref dummy, ref dummy, ref dummy);
                }
                catch { }
            }

            if (wordWnd == 0) wordWnd = FindWindow("Opusapp", null);
            if (wordWnd != 0)
            {
                SetParent(wordWnd, this.Handle.ToInt32());

                object fileName = filename;
                object newTemplate = false;
                object docType = 0;
                object readOnly = true;
                object isVisible = true;
                object missing = System.Reflection.Missing.Value;

                try
                {
                    if (wd == null)
                    {
                        throw new WordInstanceException();
                    }

                    if (wd.Documents == null)
                    {
                        throw new DocumentInstanceException();
                    }

                    if (wd != null && wd.Documents != null)
                    {
                        document = wd.Documents.Add(ref fileName, ref newTemplate, ref docType, ref isVisible);
                    }

                    if (document == null)
                    {
                        throw new ValidDocumentException();
                    }
                }
                catch
                {
                }

                try
                {
                    wd.ActiveWindow.DisplayRightRuler = false;
                    wd.ActiveWindow.DisplayScreenTips = false;
                    wd.ActiveWindow.DisplayVerticalRuler = false;
                    wd.ActiveWindow.DisplayRightRuler = false;
                    wd.ActiveWindow.ActivePane.DisplayRulers = false;
                    wd.ActiveWindow.ActivePane.View.Type = Word.WdViewType.wdPrintView;
                    wd.CommandBars.ActiveMenuBar.Enabled = false;
                    //wd.ActiveWindow.ActivePane.View.Type = Word.WdViewType.wdPrintView;//wdWebView; // .wdNormalView;
                }
                catch
                {

                }


                /// Code Added
                /// Disable the specific buttons of the command bar
                /// By default, we disable/hide the menu bar
                /// The New/Open buttons of the command bar are disabled
                /// Other things can be added as required (and supported ..:) )
                /// Lots of commented code in here, if somebody needs to disable specific menu or sub-menu items.
                /// 
                int counter = wd.ActiveWindow.Application.CommandBars.Count;
                for (int i = 1; i <= counter; i++)
                {
                    try
                    {

                        String nm = wd.ActiveWindow.Application.CommandBars[i].Name;
                        if (nm == "Standard")
                        {
                            //nm=i.ToString()+" "+nm;
                            //MessageBox.Show(nm);
                            int count_control = wd.ActiveWindow.Application.CommandBars[i].Controls.Count;
                            for (int j = 1; j <= 2; j++)
                            {
                                //MessageBox.Show(wd.ActiveWindow.Application.CommandBars[i].Controls[j].ToString());
                                wd.ActiveWindow.Application.CommandBars[i].Controls[j].Enabled = false;

                            }
                        }

                        if (nm == "Menu Bar")
                        {
                            //To disable the menubar, use the following (1) line
                            wd.ActiveWindow.Application.CommandBars[i].Enabled = false;

                            /// If you want to have specific menu or sub-menu items, write the code here. 
                            /// Samples commented below

                            //							MessageBox.Show(nm);
                            //int count_control=wd.ActiveWindow.Application.CommandBars[i].Controls.Count;
                            //MessageBox.Show(count_control.ToString());						

                            /*
                            for(int j=1;j<=count_control;j++)
                            {
                                /// The following can be used to disable specific menuitems in the menubar	
                                /// wd.ActiveWindow.Application.CommandBars[i].Controls[j].Enabled=false;

                                //MessageBox.Show(wd.ActiveWindow.Application.CommandBars[i].Controls[j].ToString());
                                //MessageBox.Show(wd.ActiveWindow.Application.CommandBars[i].Controls[j].Caption);
                                //MessageBox.Show(wd.ActiveWindow.Application.CommandBars[i].Controls[j].accChildCount.ToString());


                                ///The following can be used to disable some or all the sub-menuitems in the menubar
								
								 
                                ////Office.CommandBarPopup c;
                                ////c = (Office.CommandBarPopup)wd.ActiveWindow.Application.CommandBars[i].Controls[j];
                                ////
                                ////for(int k=1;k<=c.Controls.Count;k++)
                                ////{
                                ////	//MessageBox.Show(k.ToString()+" "+c.Controls[k].Caption + " -- " + c.Controls[k].DescriptionText + " -- " );
                                ////	try
                                ////	{
                                ////		c.Controls[k].Enabled=false;
                                ////		c.Controls["Close Window"].Enabled=false;
                                ////	}
                                ////	catch
                                ////	{
                                ////
                                ////	}
                                ////}
								
								

                                    //wd.ActiveWindow.Application.CommandBars[i].Controls[j].Control	 Controls[0].Enabled=false;
                                }
                                */

                        }

                        nm = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                // Show the word-document
                try
                {
                    wd.Visible = true;
                    wd.Activate();

                    SetWindowPos(wordWnd, this.Handle.ToInt32(), 0, 0, this.Bounds.Width, this.Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);

                    //Call onresize--I dont want to write the same lines twice
                    OnResize();
                }
                catch
                {
                    MessageBox.Show("Error: do not load the document into the control until the parent window is shown!");
                }

                /// We want to remove the system menu also. The title bar is not visible, but we want to avoid accidental minimize, maximize, etc ..by disabling the system menu(Alt+Space)
                try
                {
                    int hMenu = GetSystemMenu(wordWnd, false);
                    if (hMenu > 0)
                    {
                        int menuItemCount = GetMenuItemCount(hMenu);
                        RemoveMenu(hMenu, menuItemCount - 1, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 2, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 3, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 4, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 5, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 6, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 7, MF_REMOVE | MF_BYPOSITION);
                        RemoveMenu(hMenu, menuItemCount - 8, MF_REMOVE | MF_BYPOSITION);
                        DrawMenuBar(wordWnd);
                    }
                }
                catch { };
                this.Parent.Focus();
            }

            deactivateevents = false;
            return document;
        }

        public void Navigate(string filename, ref object obj1, ref object obj2, ref object obj3, ref object obj4)
        {
             Navigate(filename);
        }
        public void Navigate(string filename)
        {
            if (filename.Equals("about:blank") && wd != null && wd.Documents.Count >0)
            {
                try
                {
                    deactivateevents = true;
                    object obj = 1;
                    Word.Document tempDoc = wd.Documents[obj];
                    object objSaveOption = Word.WdSaveOptions.wdDoNotSaveChanges;
                    object oMissing = System.Reflection.Missing.Value;
                    Word.Application wordApp = tempDoc.Application;
                    tempDoc.Close(objSaveOption, oMissing, oMissing);
                    wordApp.Quit(objSaveOption, oMissing, oMissing);
                }
                catch (Exception)
                {
                }
                finally
                {
                    deactivateevents = false;
                }
                try
                {
                    CloseControl();
                }
                catch (Exception) { }
                finally
                {
                    document = null;
                    wd = null;
                    hOffset = -1;
                    wordWnd = 0;
                }
                //this.CloseControl();
                return;
            }
            try
            {
                CloseControl();
            }
            catch (Exception) { }
            finally
            {
                document = null;
                wd = null;
                hOffset = -1;
                wordWnd = 0;
            }
            Word.Document doc = this.LoadDocument(filename);
            if (delLoadFileFinished != null)
            {
                delLoadFileFinished.Invoke(doc);
            }
        }
        public delegate void DELLoadFileFinished(Word.Document doc);
        public DELLoadFileFinished delLoadFileFinished = null;
        /// <summary>
        /// restores Word.
        /// If the program crashed somehow.
        /// Sometimes Word saves it's temporary settings :(
        /// </summary>
        public void RestoreWord()
        {
            try
            {
                int counter = wd.ActiveWindow.Application.CommandBars.Count;
                for (int i = 0; i < counter; i++)
                {
                    try
                    {
                        wd.ActiveWindow.Application.CommandBars[i].Enabled = true;
                    }
                    catch
                    {

                    }
                }
            }
            catch { };

        }

        /// <summary>
        /// internal resize function
        /// utilizes the size of the surrounding control
        /// 
        /// optimzed for Word2000 but it works pretty good with WordXP too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int hOffset = -1;
        public CommandBar returnRibbon(CommandBars cbs)
        {
            foreach (CommandBar cb in cbs)
            {
                if (cb.Name == "Ribbon")
                {

                    return cb;
                }
            }
            return null;
        }
        private void OnResize()
        {
            //The original one that I used is shown below. Shows the complete window, but its buttons (min, max, restore) are disabled
            //// MoveWindow(wordWnd,0,0,this.Bounds.Width,this.Bounds.Height,true);


            ///Change below
            ///The following one is better, if it works for you. We donot need the title bar any way. Based on a suggestion.
            //int borderWidth = SystemInformation.Border3DSize.Width;
            //int borderHeight = SystemInformation.Border3DSize.Height;
            //int captionHeight = SystemInformation.CaptionHeight;
            //int statusHeight = SystemInformation.ToolWindowCaptionHeight;
            //MoveWindow(
            //    wordWnd,
            //    -2 * borderWidth,
            //    -2 * borderHeight - captionHeight,
            //    this.Parent.Parent.Parent.Width - 4,
            //    this.Parent.Height,
            //    true);
            int borderWidth = SystemInformation.Border3DSize.Width;
            int borderHeight = SystemInformation.Border3DSize.Height;
            int captionHeight = SystemInformation.CaptionHeight;
            int statusHeight = SystemInformation.ToolWindowCaptionHeight;
            //wd.DisplayStatusBar = false;
            int h = 0;
            //wd.ActiveWindow.NewWindow();
            if (wd != null && hOffset == -1)
            {
                //wd.ApplicationEvents2_Event_DocumentOpen

                //wd.ex
                wd.DisplayStatusBar = false;
                wd.CommandBars["Formatting"].Visible = false;
                wd.CommandBars["Standard"].Visible = false;
                CommandBar cb = returnRibbon(wd.CommandBars);
                cb.Visible = false;
                hOffset = cb.Height;

                //wd.CommandBars
                //.wor
                //wd.DisplayStatusBar = false;
                //wd.CommandBars.MenuAnimationStyle = MsoMenuAnimation.msoMenuAnimationNone;
            }

            //hOffset = 0;
            //int uh = wd.ActiveWindow.UsableHeight;
            //!!!!!!!!!!!!!!!!!!!!!!

            MoveWindow(
                wordWnd,
                -2 * borderWidth,
                -hOffset,
                this.Parent.Width + 4 * borderWidth,
                this.Parent.Height + 2 * captionHeight - 40 + hOffset,
                true);

        }

        private void OnResize(object sender, System.EventArgs e)
        {
            OnResize();
        }


        /// Required. 
        /// Without this, the command bar buttons that have been disabled 
        /// will remain disabled permanently (does not occur at every machine or every time)
        public void RestoreCommandBars()
        {
            try
            {
                int counter = wd.ActiveWindow.Application.CommandBars.Count;
                for (int i = 1; i <= counter; i++)
                {
                    try
                    {

                        String nm = wd.ActiveWindow.Application.CommandBars[i].Name;
                        if (nm == "Standard")
                        {
                            int count_control = wd.ActiveWindow.Application.CommandBars[i].Controls.Count;
                            for (int j = 1; j <= 2; j++)
                            {
                                wd.ActiveWindow.Application.CommandBars[i].Controls[j].Enabled = true;
                            }
                        }
                        if (nm == "Menu Bar")
                        {
                            wd.ActiveWindow.Application.CommandBars[i].Enabled = true;
                        }
                        nm = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch { }

        }

    }
    public class DocumentInstanceException : Exception
    { }

    public class ValidDocumentException : Exception
    { }

    public class WordInstanceException : Exception
    { }
}

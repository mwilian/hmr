using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace EMR
{
  public static  class clsIme
    {
       //����һЩAPI����
        [DllImport("imm32.dll")]
       public static extern IntPtr ImmGetContext(IntPtr hwnd);
       [DllImport("imm32.dll")]
       public static extern bool ImmGetOpenStatus(IntPtr himc);
       [DllImport("imm32.dll")]
       public static extern bool ImmSetOpenStatus(IntPtr himc, bool b);
       [DllImport("imm32.dll")]
       public static extern bool ImmGetConversionStatus(IntPtr himc, ref int lpdw, ref int lpdw2);
       [DllImport("imm32.dll")]
       public static extern int ImmSimulateHotKey(IntPtr hwnd, int lngHotkey);
       public const int IME_CMODE_FULLSHAPE = 0x8;
       public const int IME_CHOTKEY_SHAPE_TOGGLE = 0x11;
       //����SetIme������Form
       public static void SetIme(Form frm)
       {
           frm.Paint += new PaintEventHandler(frm_Paint);
           ChangeAllControl(frm);
       }
       //����SetIme������Control
       public static void SetIme(Control ctl)
       {
           ChangeAllControl(ctl);
       }
       //����SetIme�����������
       public static void SetIme(IntPtr Handel)
       {
           ChangeControlIme(Handel);
       }

       private static void ChangeAllControl(Control ctl)
       {
           //�ڿؼ��ĵ�Enter�¼��д������������뷨״̬
           ctl.Enter += new EventHandler(ctl_Enter);
           //�����ӿؼ���ʹÿ���ؼ�������Enter��ί�д���
           foreach (Control ctlChild in ctl.Controls)
               ChangeAllControl(ctlChild);
       }

       static void frm_Paint(object sender, PaintEventArgs e)
       {
           /**//*������Ϊʲôʹ��Pain�¼���������Load�¼���Activated�¼����ǻ������п��ǣ�
            * 1��������Form�У���Щ�ؼ�����������ʱ��̬��ӵ�
            * 2��������Form�У�ʹ�õ��˷�.NET��OCX�ؼ�
            * 3��Form������Form��ʱ��Activated�¼��������ᴥ�� */
           ChangeControlIme(sender);
       }
       //�ؼ���Enter�������
       static void ctl_Enter(object sender, EventArgs e)
       {
           ChangeControlIme(sender);
       }
       private static void ChangeControlIme(object sender)
       {
           Control ctl = (Control)sender;
           ChangeControlIme(ctl.Handle);
       }
       //�������������������������뷨��ȫ�ǰ��״̬
       private static void ChangeControlIme(IntPtr h)
       {
           IntPtr HIme = ImmGetContext(h);            
           if (ImmGetOpenStatus(HIme))  //������뷨���ڴ�״̬
           {
               int iMode = 0;
               int iSentence = 0;
               bool bSuccess = ImmGetConversionStatus(HIme, ref iMode, ref iSentence);  //�������뷨��Ϣ
               if (bSuccess)
               {
                   if ((iMode & IME_CMODE_FULLSHAPE) > 0)   //�����ȫ��
                       ImmSimulateHotKey(h, IME_CHOTKEY_SHAPE_TOGGLE);  //ת���ɰ��
               }
           }
       }

    }
}

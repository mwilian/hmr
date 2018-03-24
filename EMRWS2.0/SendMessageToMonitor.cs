using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace EMR
{
    public static class SendMessageToMonitor
    {
        private static Thread thread = null;
        private static string __strMessage = "";
        public static void sendMessage(string strMessage) {
            if (SendMessageToMonitor.thread == null) {
                SendMessageToMonitor.thread = new Thread(SendMessageToMonitor.sendToMonitor);
                __strMessage = strMessage;
                SendMessageToMonitor.thread.IsBackground = true;
                SendMessageToMonitor.thread.Start();
                SendMessageToMonitor.thread = null;
            }
            
        }
        private static void sendToMonitor()
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("192.0.8.126", 8800);
                StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                sw.Write(SendMessageToMonitor.__strMessage);
                sw.Flush();
                sw.Close();
                tcpClient.Close();
            }
            catch (Exception ex) { }
            finally {
                
            }
        }
    }
}

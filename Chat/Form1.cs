using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Management;

namespace Chat
{
    public partial class Form1 : Form
    {
        public Form1(IPAddress IP, int port)
        {
            InitializeComponent();
            serverIP = IP;
            serverPort = port;
            StartServer();         
        }
        private void StartServer()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(serverIP, serverPort));
            server.Listen(10);
            Thread myThread = new Thread(ListenClientConnect);
            myThread.IsBackground = true;
            myThread.Start();
        }
        private string GetIP()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    return (nic["IPAddress"] as String[])[0];
                }
            }
            return "";
        }

        static Socket server;
        static Socket client;
        public IPAddress serverIP;//客户机IP
        public IPAddress clientIP;//本机IP
        public int serverPort;//服务端端口号
        public int clientPort;//客户端端口号
        static byte[] serverResult = new byte[1024];
        static byte[] clientResult = new byte[1024];
        bool connected = false;
        
        
        delegate void UpdateText(string text);
        private void UpdateTextBox(string text)
        {
            if (textBoxX2.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!textBoxX2.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (textBoxX2.Disposing || textBoxX2.IsDisposed)
                        return;
                }
                UpdateText d = new UpdateText(UpdateTextBox);
                textBoxX2.Invoke(d, new object[] { text });
            }
            else
            {
                textBoxX2.AppendText(text);
            }
        }
        private void ListenClientConnect()
        {
            //while (true)
            //{            
                Socket client = server.Accept();
                while (true)
                {
                    try
                    {
                        //通过clientSocket接收数据  
                        int receiveNumber = client.Receive(serverResult);
                        string msg = Encoding.Unicode.GetString(serverResult, 0, receiveNumber) + "\n";
                        string time = DateTime.Now.ToString("u");
                        time = time.Substring(0, time.Length - 1);
                        UpdateTextBox(time + "\r\n" + msg + "\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        break;
                    }
                }
           // }
        }

        private void StartClient()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(new IPEndPoint(clientIP, clientPort)); //配置服务器IP与端口  
            }
            catch
            {
                MessageBox.Show("连接服务器失败！");
                return;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                StartClient();
                connected = true;
            }
            if (textBoxX1.Text == "")
            {
                MessageBox.Show("发送消息为空！");
                return;
            }
            try
            {
                client.Send(Encoding.Unicode.GetBytes(textBoxX1.Text));                
                string time = DateTime.Now.ToString("u");
                time = time.Substring(0, time.Length - 1);
                textBoxX2.AppendText(time + "\r\n" + textBoxX1.Text + "\r\n");
                textBoxX1.Clear();
            }
            catch
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                MessageBox.Show("消息发送失败！");
            }
        }

        private void textBoxX1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBoxX1_KeyDown(object sender, KeyEventArgs e)
        {
            if (checkBoxItem1.Checked)
            {
                if (e.Modifiers.CompareTo(Keys.Shift) != 0 && e.KeyCode == Keys.Enter)
                {
                    buttonX1_Click(sender, e);
                    textBoxX1.Clear();
                    SendKeys.Send("{BACKSPACE}");
                }
            }
            else
            {
                if (e.Modifiers.CompareTo(Keys.Shift) == 0 && e.KeyCode == Keys.Enter)
                {
                    buttonX1_Click(sender, e); 
                    textBoxX1.Clear();
                    SendKeys.Send("{BACKSPACE}");
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Close();
            client.Close();
        }
    }
}

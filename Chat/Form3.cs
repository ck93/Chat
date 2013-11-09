using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    public partial class Form3 : Form
    {
        public Form3(Form2 f)
        {
            InitializeComponent();
            timer1.Start();
            f2 = f;
            f2.Hide();
            mydatabase = new mysql();
            mydatabase.Initialize("59.66.133.208", "chat", "root", "welcomeck", "3306");
            myname = f2.textBox1.Text;
            DisplayList();           
        }
        Form2 f2;
        mysql mydatabase;
        List<string>[] friendsInfo;//所有好友信息
        int myindex;//记录自己的信息在表中的位置
        string myname;
        string[] myinfo;//自己的信息
        void DisplayList()
        {
            TreeNode root = new TreeNode("我的好友");
            treeView1.Nodes.Add(root);
            friendsInfo = mydatabase.SelectAll();
            for (int i = 0; i < friendsInfo[0].Count; i++)
            {
                if (friendsInfo[0][i] == myname)
                {
                    myindex = i;
                    continue;
                }
                TreeNode node = new TreeNode(friendsInfo[0][i] + friendsInfo[1][i]);
                root.Nodes.Add(node);
            }
        }
        
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            mydatabase.Update("UPDATE user SET status='（离线）' WHERE name='" + myname + "'");
            timer1.Dispose();
            f2.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            treeView1.TopNode.Remove();//移除节点
            DisplayList();//重新显示
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            myinfo = mydatabase.Select(myname);
            Form1 f1 = new Form1(IPAddress.Parse(myinfo[3]), Convert.ToInt32(myinfo[4]));
            int index = treeView1.SelectedNode.Index;
            if (index >= myindex)
                index++;
            f1.Text = "与" + friendsInfo[0][index] + friendsInfo[1][index] + "会话中";
            f1.clientIP = IPAddress.Parse(friendsInfo[2][index]);
            f1.clientPort = Convert.ToInt32(friendsInfo[3][index]);
            f1.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int oldOnlineTime = Convert.ToInt32(myinfo[5]);
            mydatabase.Update("UPDATE user SET onlinetime='" + (oldOnlineTime+1).ToString() + "' WHERE name='" + myname + "'");
            treeView1.TopNode.Remove();
            DisplayList();
        }
        
    }
}

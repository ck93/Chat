using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;

namespace Chat
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            mydatabase.Initialize("59.66.133.208", "chat", "public", "tkzc6717137", "3306");
        }
        mysql mydatabase = new mysql();
        
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
        private void button2_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            if (name == "")
            {
                MessageBox.Show("用户名不能为空");
                return;
            }
            string password = textBox2.Text;
            if (password == "")
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            if (mydatabase.Count(name) > 0)
            {
                MessageBox.Show("该用户名已存在，请更换一个用户名注册！");
                return;
            }
            Random ro = new Random();
            mydatabase.Insert(name, password, "（在线）", GetIP(), ro.Next(8787,9898));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string name = textBox1.Text;
            if (name == "")
            {
                MessageBox.Show("用户名不能为空！");
                return;
            }
            string password = textBox2.Text;
            if (password == "")
            {
                MessageBox.Show("密码不能为空！");
                return;
            }
            progressBar1.Visible = true;
            if (mydatabase.Count(name) < 1)
            {
                MessageBox.Show("该用户不存在！");
                progressBar1.Visible = false;
                return;
            }          
            if (password == mydatabase.Select(name)[1])
            {
                mydatabase.Update("UPDATE user SET status='（在线）',ip='" + GetIP() + "' WHERE name='" + name + "'");                
                Form3 f = new Form3(this);                
                f.ShowDialog();
            }
            else
            {
                progressBar1.Visible = false;
                MessageBox.Show("您输入的用户名或密码错误！");                
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button1_Click(sender, e);
        }
        
    }
}

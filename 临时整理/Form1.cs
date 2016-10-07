using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace 临时整理
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //点击窗口“关闭” 并非关闭 并且让其窗口隐藏
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible == false)
                {
                    this.Visible = true;
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "exit")
            {
                //测试 this.Close();不能退出 ，因为存在托管线程
                System.Environment.Exit(0);
                //这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            string tempPath = fbd.SelectedPath;
            textBox2.Text = tempPath;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string dir = Application.StartupPath;
            string strConfigPath = dir + "\\" + "default-config.txt";
            if (File.Exists(strConfigPath))
            {
                StreamReader sr = File.OpenText(strConfigPath);
                string strConfigText = sr.ReadToEnd();
                sr.Close();
                string[] config = strConfigText.Split('\n');
                textBox2.Text = config[0];
                textBox1.Text = config[1];
            }
            else
            {
                string dir1 = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string tempPath = dir1 + "\\" + "tempFolders\\";
                textBox2.Text = tempPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strTempPath = textBox2.Text.ToString().Trim();
            string strCloudPath = textBox1.Text.ToString().Trim();
            ///获取程序启动相对路径
            string dir = Application.StartupPath;
            if (strTempPath == "" || strCloudPath == "")
            {
                MessageBox.Show("请填写好两个路径");
            }
            else
            {
                bool boolTempPath = Directory.Exists(strTempPath);
                bool boolCloudPath = Directory.Exists(strCloudPath);
                if (boolTempPath && boolCloudPath)
                {
                    string strConfigPath = dir + "\\" + "default-config.txt";
                    FileStream fs = File.Create(strConfigPath);
                    fs.Close();

                    string strConfigText = strTempPath + "\n" + strCloudPath;
                    StreamWriter sw = File.AppendText(strConfigPath);
                    sw.WriteLine(strConfigText);
                    sw.Close();
                }
                else if (!boolTempPath)
                {
                    MessageBox.Show("临时文件夹路径不存在！");
                }
                else if (!boolCloudPath)
                {
                    MessageBox.Show("云盘同步文件夹路径不存在！");
                }
                else
                {
                    MessageBox.Show("未知错误");
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string tempPath = textBox2.Text.ToString();
            string cloudPath = textBox2.Text.ToString();
            DirectoryInfo di1 = new DirectoryInfo(tempPath);
            if (!Directory.Exists(cloudPath))
            {
                Directory.CreateDirectory(cloudPath);
                foreach (FileInfo di2 in di1.GetFiles())
                {
                    File.Copy(tempPath + di2.Name, cloudPath + di2.Name);
                }
            }
            else
            {
                foreach (FileInfo di2 in di1.GetFiles())
                {
                    File.Copy(tempPath + di2.Name, cloudPath + di2.Name, true);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!(textBox2.Text.ToString() == ""))
            {
                Process.Start("explorer.exe", textBox2.Text.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!(textBox2.Text.ToString() == ""))
            {
                Process.Start("explorer.exe", textBox1.Text.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            string cloudPath = fbd.SelectedPath;
            textBox1.Text = cloudPath;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) //设置开机自启动  
            {
                MessageBox.Show("设置开机自启动，需要修改注册表", "提示");
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                rk2.SetValue("JcShutdown", path);
                rk2.Close();
                rk.Close();
            }
            else //取消开机自启动  
            {
                MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"SoftwareMicrosoftWindowsCurrentVersionRun");
                rk2.DeleteValue("JcShutdown", false);
                rk2.Close();
                rk.Close();
            }
        }
    }
}

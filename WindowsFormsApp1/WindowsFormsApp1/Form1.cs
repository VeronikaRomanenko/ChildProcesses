using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("ProcessName");
            listView1.Columns.Add("Id");
            listView1.Columns.Add("ThreadsCount");
            listView1.Columns.Add("HandleCount");
            listView1.View = View.Details;
            foreach (Process proc in Process.GetProcesses())
            {
                TreeNode newNode = new TreeNode(proc.ProcessName);
                newNode.Tag = proc;
                GetChildProc(proc, newNode, 0);
                treeView1.Nodes.Add(newNode);
            }
        }
        void GetChildProc(Process ParentProc, TreeNode tr, int i)
        {
            if (i > 5)
                return;
            List<Process> children = null;
            ManagementObjectSearcher mos = new ManagementObjectSearcher($"Select *from win32_process where parentprocessid={ParentProc.Id}");
            children = new List<Process>();
            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }
            if (children.Count == 0)
                return;
            foreach (Process proc in children)
            {
                TreeNode newNode = new TreeNode(proc.ProcessName);
                newNode.Tag = proc;
                GetChildProc(proc, newNode, ++i);
                tr.Nodes.Add(newNode);
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            listView1.Items.Clear();
            Process proc = treeView1.SelectedNode.Tag as Process;
            ListViewItem item = new ListViewItem(proc.ProcessName);
            item.SubItems.Add(proc.Id.ToString());
            item.SubItems.Add(proc.Threads.Count.ToString());
            item.SubItems.Add(proc.HandleCount.ToString());
            listView1.Items.Add(item);
        }
    }
}
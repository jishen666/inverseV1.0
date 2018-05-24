using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace analysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //加载时显示数据展示界面
        private void Form1_Load(object sender, EventArgs e)
        {
            Form frm = new dataview(this);
            frm.TopLevel = false;
            frm.Show();
            frm.Dock = DockStyle.Fill;
            panel2.Controls.Add(frm);
        }


        //打开数据展示界面
        private void btndataview_Click(object sender, EventArgs e)
        {
            Form frm = new dataview(this);
            frm.TopLevel = false;
            frm.Show();
            frm.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(frm);
        }

        //打开数据导入界面
        private void btndataimport_Click(object sender, EventArgs e)
        {
            var dataimport = new dataimport(this);
            dataimport.ShowDialog();
        }

        ////打开相关性分析界面
        private void btncor_Click(object sender, EventArgs e)
        {
            Form frm = new datacorrelation(this);
            frm.TopLevel = false;
            frm.Show();
            frm.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(frm);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex==0)
            {
                Form frm = new dataview(this);
                frm.TopLevel = false;
                frm.Show();
                frm.Dock = DockStyle.Fill;
                panel2.Controls.Clear();
                panel2.Controls.Add(frm);
            }
            if(tabControl1.SelectedIndex==1)
            {
                Form frm = new picprocessing(this);
                frm.TopLevel = false;
                frm.Show();
                frm.Dock = DockStyle.Fill;
                panel2.Controls.Clear();
                panel2.Controls.Add(frm);
            }
            if(tabControl1.SelectedIndex==2)
            {
                Form frm = new datacorrelation(this);
                frm.TopLevel = false;
                frm.Show();
                frm.Dock = DockStyle.Fill;
                panel2.Controls.Clear();
                panel2.Controls.Add(frm);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace analysis
{
    public partial class dataview : Form
    {
        private Form1 fm;

        public dataview(Form1 form1)
        {
            InitializeComponent();
            fm = form1;
        }
        public dataview()
        {
            InitializeComponent();

        }




        private void Form2_Load(object sender, EventArgs e)
        {
            //自已绘制  
            this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);
            
            //加载treeview
            MySqlCommand mycom = null;
            MySqlConnection conn1 = connect.mysqlopen();
            mycom = conn1.CreateCommand();
            mycom.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'actualdata'";
            MySqlDataReader sdr1 = mycom.ExecuteReader();
            int i = 0;
            while (sdr1.Read())
            {
                treeView1.Nodes[0].Nodes.Add(sdr1[0].ToString());

                i++;
            }
            conn1.Close();

        }
        //在绘制节点事件中，按自已想的绘制  
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显  
            return;

            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字  
                e.Graphics.FillRectangle(Brushes.DarkBlue, e.Node.Bounds);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
        }
        //点击子节点显示对应数据
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level > 0)
            {
                string list = treeView1.SelectedNode.Text;
                MySqlCommand mycom = null;
                MySqlConnection conn = connect.mysqlopen();
                mycom = conn.CreateCommand();
                string sql = string.Format("select * from " + list);
                mycom.CommandText = sql;
                MySqlDataAdapter adap = new MySqlDataAdapter(mycom);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0].DefaultView;
                label2.Text = list;
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes[0].Nodes.Clear();
            this.Form2_Load(null, null);
        }

    }
        
}


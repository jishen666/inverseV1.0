using MathNet.Numerics.Statistics;
using MySql.Data.MySqlClient;
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
    public partial class datacorrelation : Form
    {
        private Form1 fm1;

        DataTable datatable;
        DataTable datatablebands;
        double[,] bands;
        double[] pera;
        public datacorrelation()
        {
            InitializeComponent();
        }

        public datacorrelation(Form1 form1)
        {
            InitializeComponent();
            fm1 = form1;
        }

        //选择类型
        private void cmbtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbtype.SelectedItem.ToString() == "Landsat8")
                {
                    MySqlConnection conn = connect.mysqlopen();
                    MySqlCommand mycom = conn.CreateCommand();
                    mycom.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'actualdata'";
                    MySqlDataReader sdr1 = mycom.ExecuteReader();
                    int i = 0;
                    cmbfile.Items.Clear();
                    while (sdr1.Read())
                    {
                        //treeView1.Nodes[0].Nodes.Add(sdr1[0].ToString());
                        cmbfile.Items.Add(sdr1[0].ToString());

                        i++;
                    }
                    conn.Close();

                }
                else
                {
                    cmbfile.Items.Clear();
                }
            }
            catch (System.NullReferenceException)
            {

            }

        }
        //加载参数选项
        private void cmbfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取选中的文件名
            string list = cmbfile.SelectedItem.ToString();
            //从mysql读取相应名的表
            string sql = string.Format("select * from " + list);
            MySqlConnection conn = connect.mysqlopen();
            MySqlCommand mycom = conn.CreateCommand();
            mycom.CommandText = sql;
            MySqlDataAdapter adap = new MySqlDataAdapter(mycom);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            //保存表至datatable
            datatable = ds.Tables[0];
            //清空上次点击
            clbbands.Items.Clear();
            cmbpara.Items.Clear();
            //获取列名并显示
            foreach (DataColumn dc in datatable.Columns)
            {
                clbbands.Items.Add(dc.ColumnName);
                cmbpara.Items.Add(dc.ColumnName);
            }
        }

        //获得checklistbox选中的项的index并返回数组
        public string[] GetChecked(CheckedListBox checkList)
        {
            string str = "";
            for (int i = 0; i < checkList.Items.Count; i++)
            {
                //获取checkedlistbox中选中编号
                if (checkList.GetItemChecked(i))
                {
                    //strr += checkList.GetItemText(checkList.Items[i])+" "获取checkedlistbox中选中的内容
                    str += i + " ";
                }
            }
            string[] arr = str.Split(' ');//将选中内容保存至数组中

            return arr;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (datatable != null && cmbpara.SelectedItem != null && clbbands.SelectedIndex != -1)
            {


                string[] str = GetChecked(clbbands);
                //获取打开表的行数
                int rows = datatable.Rows.Count;
                //创建DataSet
                DataSet ds = new DataSet();
                //创建一个DataTalbe
                DataTable dt = new DataTable();
                //将checkedlistbox选中的项建立列名                               
                for (int i = 0; i < str.Length - 1; i++)
                {
                    int col = Convert.ToInt32(str[i]);
                    string header = datatable.Columns[col].ColumnName;
                    dt.Columns.Add(new DataColumn(header, typeof(string)));//为表内建立Column
                }
                //导入选中的列的数据，根据打开数据列名逐行获取
                for (int i = 0; i < rows; i++)
                {

                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < str.Length - 1; j++)
                    {
                        int col = Convert.ToInt32(str[j]);
                        string header = datatable.Columns[col].ColumnName;
                        string ai1 = datatable.Rows[i][col].ToString();
                        dr[header] = ai1;
                    }
                    dt.Rows.Add(dr);
                }
                datatablebands = dt;
                //获取选中波段
                bands = extarry.arrys(datatablebands);
                //获取选取的水质
                string cmbselect = cmbpara.SelectedItem.ToString();
                int cmbselectint = cmbpara.SelectedIndex;
                dt.Columns.Add(cmbselect, typeof(string));
                for (int i = 0; i < rows; i++)
                {
                    string sel = datatable.Rows[i][cmbselectint].ToString();
                    dt.Rows[i][str.Length - 1] = sel;
                }
                //datatable = dt;
                //获取选中水质参数
                pera = extarry.arry(datatable, datatable.Columns.Count - 1);
                dataGridView1.DataSource = dt;

            }
            else
            {
                MessageBox.Show("请选择完整数据！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //重置选择
        private void button2_Click(object sender, EventArgs e)
        {
            //cmbtype.SelectedIndex = -1;
            //cmbfile.Items.Clear();
            //clbbands.Items.Clear();
            //cmbpara.Items.Clear();
            //dataGridView1.DataSource = null;

            Form frm = new datacorrelation(fm1);
            frm.TopLevel = false;
            frm.Show();
            frm.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(frm);

        }

        //计算Bi/Bj与chla的相关性
        private void btnbdb_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                //清空控件数据
                dataGridView2.DataSource = null;
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                //选择到相关性结果表
                tabControl1.SelectedIndex = 1;
                //用于获取波段计算结果
                double[] a = new double[datatablebands.Rows.Count];
                //保存最佳波段计算结果
                double[] a1 = new double[datatablebands.Rows.Count];
                //选择波段的个数
                int cols = bands.GetLength(1);
                //保存相关系数
                double R1 = 0;
                //保存组合形式
                string R2 = null;
                //保存i
                int i1 = 0;
                //保存j
                int j1 = 0;
                //保存所有的组合情况
                string[,] c = new string[2, cols * cols];
                //用于建立datatable的列
                string[] d = new string[cols * cols];
                //计算相关系数，并获取最大的组合
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int num = cols * (i) + j;
                        if (i != j)
                        {
                            for (int k = 0; k < datatablebands.Rows.Count; k++)
                            {
                                //Bi/Bj
                                a[k] = bands[k, i] / bands[k, j];
                            }
                            //计算相关性
                            double r = Correlation.Pearson(a, pera);
                            string b = "B" + (i + 1) + "/" + "B" + (j + 1);
                            if (Math.Abs(R1) < Math.Abs(r))
                            {
                                R1 = r;
                                R2 = b;
                                i1 = i;
                                j1 = j;
                            }

                            c[0, num] = b;
                            c[1, num] = Convert.ToString(r);
                        }
                        else
                        {
                            string b = "B" + (i + 1) + "/" + "B" + (j + 1);
                            c[0, num] = b;
                            c[1, num] = "null";
                        }
                    }
                }
                //展示所有组合
                for (int i = 1; i <= cols * cols; i++)
                {
                    d[i - 1] = Convert.ToString(i);
                }
                DataTable dt = extarry.Convertdatatable(d, c);
                dataGridView2.DataSource = dt;
                //展示最佳组合到lable
                labR.Text = Convert.ToString(R1);
                labcor.Text = R2;
                labname.Text = cmbfile.SelectedItem.ToString() + ": Bi/Bj";
                //最佳组合的数组
                for (int k = 0; k < datatablebands.Rows.Count; k++)
                {
                    //Bi/Bj
                    a1[k] = bands[k, i1] / bands[k, j1];
                }
                //绘制最相关的组合至图表中
                chart1.Series[0].Points.DataBindXY(a1, pera);
                //rek reb 为系数，rr 为相关系数的平方，maxerr为最大偏差
                double rek, reb, maxerr, rr, max, min;
                //求回归系数
                regressionanalysis.CalcRegress(a1, pera, datatablebands.Rows.Count, out reb, out rek, out maxerr, out rr);
                extarry.maxmin(a1, out max, out min);
                //绘制回归方程图像图表中
                chart1.Series[1].Points.AddXY(min, rek * min + reb);
                chart1.Series[1].Points.AddXY(max, rek * max + reb);
                //展示回归方程到lable
                labreg.Text = "y = " + rek+"*x "+reb;

            }
            else
            {
                MessageBox.Show("请选择数据！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //计算Bi+Bj与chla的相关性
        private void btnbpb_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                //清空控件数据
                dataGridView2.DataSource = null;
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                //选择到相关性结果表
                tabControl1.SelectedIndex = 1;
                //用于获取波段计算结果
                double[] a = new double[datatablebands.Rows.Count];
                //保存最佳波段计算结果
                double[] a1 = new double[datatablebands.Rows.Count];
                //选择波段的个数
                int cols = bands.GetLength(1);
                //保存相关系数
                double R1 = 0;
                //保存组合形式
                string R2 = null;
                //保存i
                int i1 = 0;
                //保存j
                int j1 = 0;
                //保存所有的组合情况
                string[,] c = new string[2, cols * cols];
                //用于建立datatable的列
                string[] d = new string[cols * cols];
                //计算相关系数，并获取最大的组合
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int num = cols * (i) + j;
                        if (i != j)
                        {
                            for (int k = 0; k < datatablebands.Rows.Count; k++)
                            {
                                //Bi+Bj
                                a[k] = bands[k, i] + bands[k, j];
                            }
                            //计算相关性
                            double r = Correlation.Pearson(a, pera);
                            string b = "B" + (i + 1) + "+" + "B" + (j + 1);
                            if (Math.Abs(R1) < Math.Abs(r))
                            {
                                R1 = r;
                                R2 = b;
                                i1 = i;
                                j1 = j;
                            }

                            c[0, num] = b;
                            c[1, num] = Convert.ToString(r);
                        }
                        else
                        {
                            string b = "B" + (i + 1) + "/" + "B" + (j + 1);
                            c[0, num] = b;
                            c[1, num] = "null";
                        }
                    }
                }
                //展示所有组合
                for (int i = 1; i <= cols * cols; i++)
                {
                    d[i - 1] = Convert.ToString(i);
                }
                DataTable dt = extarry.Convertdatatable(d, c);
                dataGridView2.DataSource = dt;
                labR.Text = Convert.ToString(R1);
                labcor.Text = R2;
                labname.Text = cmbfile.SelectedItem.ToString() + ": Bi+Bj";
                //最佳组合的数组
                for (int k = 0; k < datatablebands.Rows.Count; k++)
                {
                    //Bi+Bj
                    a1[k] = bands[k, i1] + bands[k, j1];
                }
                //绘制最相关的组合至图表中
                chart1.Series[0].Points.DataBindXY(a1, pera);
                //rek reb 为系数，rr 为相关系数的平方，maxerr为最大偏差
                double rek, reb, maxerr, rr, max, min;
                //求回归系数
                regressionanalysis.CalcRegress(a1, pera, datatablebands.Rows.Count, out reb, out rek, out maxerr, out rr);
                extarry.maxmin(a1, out max, out min);
                //绘制回归方程图像图表中
                chart1.Series[1].Points.AddXY(min, rek * min + reb);
                chart1.Series[1].Points.AddXY(max, rek * max + reb);
                //展示回归方程到lable
                labreg.Text = "y = " + rek + "*x " + reb;
            }
            else
            {
                MessageBox.Show("请选择数据！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //计算Bi-Bj与chla的相关性
        private void btnbsb_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                //清空控件数据
                dataGridView2.DataSource = null;
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                //选择到相关性结果表
                tabControl1.SelectedIndex = 1;
                //用于获取波段计算结果
                double[] a = new double[datatablebands.Rows.Count];
                //保存最佳波段计算结果
                double[] a1 = new double[datatablebands.Rows.Count];
                //选择波段的个数
                int cols = bands.GetLength(1);
                //保存相关系数
                double R1 = 0;
                //保存组合形式
                string R2 = null;
                //保存i
                int i1 = 0;
                //保存j
                int j1 = 0;
                //保存所有的组合情况
                string[,] c = new string[2, cols * cols];
                //用于建立datatable的列
                string[] d = new string[cols * cols];
                //计算相关系数，并获取最大的组合
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int num = cols * (i) + j;
                        if (i != j)
                        {
                            for (int k = 0; k < datatablebands.Rows.Count; k++)
                            {
                                //Bi-Bj
                                a[k] = bands[k, i] - bands[k, j];
                            }
                            //计算相关性
                            double r = Correlation.Pearson(a, pera);
                            string b = "B" + (i + 1) + "-" + "B" + (j + 1);
                            if (Math.Abs(R1) < Math.Abs(r))
                            {
                                R1 = r;
                                R2 = b;
                                i1 = i;
                                j1 = j;
                            }

                            c[0, num] = b;
                            c[1, num] = Convert.ToString(r);
                        }
                        else
                        {
                            string b = "B" + (i + 1) + "-" + "B" + (j + 1);
                            c[0, num] = b;
                            c[1, num] = "null";
                        }
                    }
                }
                //展示所有组合
                for (int i = 1; i <= cols * cols; i++)
                {
                    d[i - 1] = Convert.ToString(i);
                }
                DataTable dt = extarry.Convertdatatable(d, c);
                dataGridView2.DataSource = dt;
                labR.Text = Convert.ToString(R1);
                labcor.Text = R2;
                labname.Text = cmbfile.SelectedItem.ToString() + ": Bi-Bj";
                //最佳组合的数组
                for (int k = 0; k < datatablebands.Rows.Count; k++)
                {
                    //Bi-Bj
                    a1[k] = bands[k, i1] - bands[k, j1];
                }
                //绘制最相关的组合至图表中
                chart1.Series[0].Points.DataBindXY(a1, pera);
                //rek reb 为系数，rr 为相关系数的平方，maxerr为最大偏差
                double rek, reb, maxerr, rr, max, min;
                //求回归系数
                regressionanalysis.CalcRegress(a1, pera, datatablebands.Rows.Count, out reb, out rek, out maxerr, out rr);
                extarry.maxmin(a1, out max, out min);
                //绘制回归方程图像图表中
                chart1.Series[1].Points.AddXY(min, rek * min + reb);
                chart1.Series[1].Points.AddXY(max, rek * max + reb);
                //展示回归方程到lable
                labreg.Text = "y = " + rek + "*x " + reb;
            }
            else
            {
                MessageBox.Show("请选择数据！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //计算Bi*Bj与chla的相关性
        private void btnbmb_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                //清空控件数据
                dataGridView2.DataSource = null;
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
                //选择到相关性结果表
                tabControl1.SelectedIndex = 1;
                //用于获取波段计算结果
                double[] a = new double[datatablebands.Rows.Count];
                //保存最佳波段计算结果
                double[] a1 = new double[datatablebands.Rows.Count];
                //选择波段的个数
                int cols = bands.GetLength(1);
                //保存相关系数
                double R1 = 0;
                //保存组合形式
                string R2 = null;
                //保存i
                int i1 = 0;
                //保存j
                int j1 = 0;
                //保存所有的组合情况
                string[,] c = new string[2, cols * cols];
                //用于建立datatable的列
                string[] d = new string[cols * cols];
                //计算相关系数，并获取最大的组合
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int num = cols * (i) + j;
                        if (i != j)
                        {
                            for (int k = 0; k < datatablebands.Rows.Count; k++)
                            {
                                //Bi*Bj
                                a[k] = bands[k, i] * bands[k, j];
                            }
                            //计算相关性
                            double r = Correlation.Pearson(a, pera);
                            string b = "B" + (i + 1) + "*" + "B" + (j + 1);
                            if (Math.Abs(R1) < Math.Abs(r))
                            {
                                R1 = r;
                                R2 = b;
                                i1 = i;
                                j1 = j;
                            }

                            c[0, num] = b;
                            c[1, num] = Convert.ToString(r);
                        }
                        else
                        {
                            string b = "B" + (i + 1) + "*" + "B" + (j + 1);
                            c[0, num] = b;
                            c[1, num] = "null";
                        }
                    }
                }
                //展示所有组合
                for (int i = 1; i <= cols * cols; i++)
                {
                    d[i - 1] = Convert.ToString(i);
                }
                DataTable dt = extarry.Convertdatatable(d, c);
                dataGridView2.DataSource = dt;
                labR.Text = Convert.ToString(R1);
                labcor.Text = R2;
                labname.Text = cmbfile.SelectedItem.ToString() + ": Bi*Bj";
                //最佳组合的数组
                for (int k = 0; k < datatablebands.Rows.Count; k++)
                {
                    //Bi*Bj
                    a1[k] = bands[k, i1] * bands[k, j1];
                }
                //绘制最相关的组合至图表中
                chart1.Series[0].Points.DataBindXY(a1, pera);
                //rek reb 为系数，rr 为相关系数的平方，maxerr为最大偏差
                double rek, reb, maxerr, rr, max, min;
                //求回归系数
                regressionanalysis.CalcRegress(a1, pera, datatablebands.Rows.Count, out reb, out rek, out maxerr, out rr);
                extarry.maxmin(a1, out max, out min);
                //绘制回归方程图像图表中
                chart1.Series[1].Points.AddXY(min, rek * min + reb);
                chart1.Series[1].Points.AddXY(max, rek * max + reb);
                //展示回归方程到lable
                labreg.Text = "y = " + rek + "*x " + reb;
            }
            else
            {
                MessageBox.Show("请选择数据！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}

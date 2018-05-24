using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace analysis
{
    public partial class dataimport : Form
    {
        private Form1 fm1;

        DataTable datatale;
        public dataimport(Form1 form1)
        {

            InitializeComponent();
            fm1 = form1;
        }

        public dataimport()
        {

            InitializeComponent();
            
        }

        public DataSet getData(OpenFileDialog file)
        {
            file.Filter = "Excel(*.xls)|*.xls|Excel(*.xlsx)|*.xlsx";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.Cancel) return null;
            //判断文件后缀
            var path = file.FileName;
            string fileSuffix = System.IO.Path.GetExtension(path);
            if (string.IsNullOrEmpty(fileSuffix)) return null;
            using (DataSet ds = new DataSet())
            {
                //判断Excel文件是2003版本还是2007版本
                string connString = "";
                if (fileSuffix == ".xls")
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                //读取文件
                string sql_select = " SELECT * FROM [Sheet1$]";
                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbDataAdapter cmd = new OleDbDataAdapter(sql_select, conn))
                {
                    conn.Open();
                    cmd.Fill(ds);
                }
                if (ds == null || ds.Tables.Count <= 0) return null;
                return ds;
            }
        }
        private void openfile_Click(object sender, EventArgs e)
        {
            //打开文件
            OpenFileDialog file = new OpenFileDialog();
            DataSet ds = getData(file);
            label1.Text = file.FileName;
            if (ds == null)
            {
                MessageBox.Show("请选择文件！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                datatale = ds.Tables[0];//打开excel
                int column = datatale.Columns.Count;//获取列数
                //string colname = dt.Columns[0].ColumnName获取列名
                //添加combobox的选项
                for (int i = 0; i < column; i++)
                {
                    comboBox1.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox2.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox3.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox4.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox5.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox6.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox7.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox8.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox9.Items.Add(datatale.Columns[i].ColumnName);
                    comboBox10.Items.Add(datatale.Columns[i].ColumnName);
                }
            }
        }
        //获取combobox选择项
        public string[] getcmbchecked()
        {
            string[] arr = new string[10];
            if (
                comboBox1.SelectedItem == null
                ||
                comboBox2.SelectedItem == null
                ||
                comboBox3.SelectedItem == null
                ||
                comboBox4.SelectedItem == null
                ||
                comboBox5.SelectedItem == null
                ||
                comboBox6.SelectedItem == null
                ||
                comboBox7.SelectedItem == null
                ||
                comboBox8.SelectedItem == null
                ||
                comboBox9.SelectedItem == null
                ||
                comboBox10.SelectedItem == null
                )
            {
                MessageBox.Show("导入参数不能有空！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            else
            {
                arr[0] = comboBox1.SelectedItem.ToString();
                arr[1] = comboBox2.SelectedItem.ToString();
                arr[2] = comboBox3.SelectedItem.ToString();
                arr[3] = comboBox4.SelectedItem.ToString();
                arr[4] = comboBox5.SelectedItem.ToString();
                arr[5] = comboBox6.SelectedItem.ToString();
                arr[6] = comboBox7.SelectedItem.ToString();
                arr[7] = comboBox8.SelectedItem.ToString();
                arr[8] = comboBox9.SelectedItem.ToString();
                arr[9] = comboBox10.SelectedItem.ToString();
            }
            return arr;
        }
        //导入数据至数据库
        private void btntrue_Click(object sender, EventArgs e)
        {
            if(datatale!=null)
            {
                int rows = datatale.Rows.Count;
                string[] str = getcmbchecked();
                if (str != null)
                {
                    DataTable dt = new DataTable();//创建一个DataTalbe
                                                   //将checkedlistbox选中的项建立列名
                    for (int i = 0; i < str.Length; i++)
                    {
                        string col = str[i];
                        dt.Columns.Add(new DataColumn(col, typeof(string)));//为表内建立Column
                    }
                    //导入选中的列的数据，根据打开数据列名逐行获取
                    for (int i = 0; i < rows; i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < str.Length; j++)
                        {
                            string col = str[j];
                            string ai1 = datatale.Rows[i][col].ToString();
                            dr[col] = ai1;
                        }
                        dt.Rows.Add(dr);
                    }
                    datatale = dt;
                    //创建表
                    if (string.IsNullOrWhiteSpace(textBox1.Text))
                    {
                        MessageBox.Show("导入名不能为空！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                    else
                    {
                        MySqlConnection conn = connect.mysqlopen();
                        MySqlCommand mycom = conn.CreateCommand();
                        string tablename = "landsat" + textBox1.Text;
                        string createStatement = "CREATE TABLE " + tablename + " (id int(11) NOT NULL AUTO_INCREMENT, lat double DEFAULT NULL,lon double DEFAULT NULL,B1 int(255) DEFAULT NULL,B2 int(255) DEFAULT NULL,B3 int(255) DEFAULT NULL,B4 int(255) DEFAULT NULL,B5 int(255) DEFAULT NULL,B6 int(255) DEFAULT NULL,B7 int(255) DEFAULT NULL,chla double DEFAULT NULL,PRIMARY KEY (`id`))";
                        mycom.CommandText = createStatement;
                        mycom.ExecuteNonQuery();
                        //导入excel表至数据库
                        MySqlCommand mysqlcom = conn.CreateCommand();
                        for (int i = 0; i < rows; i++)
                        {
                            double lat = Convert.ToDouble(datatale.Rows[i][0].ToString());
                            double lon = Convert.ToDouble(datatale.Rows[i][1].ToString());
                            int B1 = Convert.ToInt32(datatale.Rows[i][2].ToString());
                            int B2 = Convert.ToInt32(datatale.Rows[i][3].ToString());
                            int B3 = Convert.ToInt32(datatale.Rows[i][4].ToString());
                            int B4 = Convert.ToInt32(datatale.Rows[i][5].ToString());
                            int B5 = Convert.ToInt32(datatale.Rows[i][6].ToString());
                            int B6 = Convert.ToInt32(datatale.Rows[i][7].ToString());
                            int B7 = Convert.ToInt32(datatale.Rows[i][8].ToString());
                            double chla = Convert.ToDouble(datatale.Rows[i][9].ToString());
                            string query = string.Format("insert into " + tablename + " (lat,lon,B1,B2,B3,B4,B5,B6,B7,chla) values ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})", lat, lon, B1, B2, B3, B4, B5, B6, B7, chla);
                            mysqlcom.CommandText = query;
                            mysqlcom.ExecuteNonQuery();
                        }
                        conn.Close();
                        MessageBox.Show("导入成功");
                        this.Close();
                    }

                }
                else
                {

                }
            }else
            {
                MessageBox.Show("未选择文件", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            
        }

        private void btnfalse_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analysis
{
    class extarry
    {
        //获取datatable内单独的一列，并保存为数组
        public static double[] arry(DataTable datatable,int a)
        {
            double[] b = new double[datatable.Rows.Count];
            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                b[i] = Convert.ToDouble(datatable.Rows[i][a]);
            }
            return b;
        }
        //转换datatable为数组
        public static double [,] arrys(DataTable datatable)
        {
            int rows = datatable.Rows.Count;
            int columns = datatable.Columns.Count;
            double[,] a = new double [rows,columns];
            for(int i = 0;i<rows;i++)
            {
                for(int j =0;j<columns;j++)
                {
                    a[i,j]= Convert.ToDouble(datatable.Rows[i][j]);
                }
            }
            return a;
        }

        //二维数组转化为datatable
        public static DataTable Convertdatatable(string[] ColumnNames, string[,] Arrays)
        {
            DataTable dt = new DataTable();

            foreach (string ColumnName in ColumnNames)
            {
                dt.Columns.Add(ColumnName, typeof(string));
            }

            for (int i1 = 0; i1 < Arrays.GetLength(0); i1++)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    dr[i] = Arrays[i1, i].ToString();
                }
                dt.Rows.Add(dr);
            }
            return dt;

        }

        //求得数组最大值最小值
        public static void maxmin(double[] x,out double max,out double min)
        {
            if (x.Length > 1)
            {
                max = x[0];
                min = x[0];
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] > max)
                    {
                        max = x[i];
                    }
                    if (x[i] < min)
                    {
                        min = x[i];
                    }
                }
                return;
            }
            else
            {
                max = x[0];
                min = x[0];
                return;
            }
        }
        


    }
}

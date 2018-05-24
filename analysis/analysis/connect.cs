using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace analysis
{
   class connect
    {
        //连接数据库
        public static MySqlConnection mysqlopen()
        {
            //配置数据库信息
            String connetStr = "server=127.0.0.1;port=3306;user=root;password=root; database=actualdata;SslMode=None;";
            MySqlConnection conn = new MySqlConnection(connetStr);
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                Console.WriteLine("已经建立连接");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return conn;
        }
    }
}

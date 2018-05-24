using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analysis
{
    class regressionanalysis
    {
        public static void CalcRegress(double[] x, double[] y, int count, out double a, out double b, out double maxErr, out double r)
        {
            double sumX = 0;
            double sumY = 0;
            double avgX;
            double avgY;
            double r1;
            double r2;
            //数据量过少，无法计算
            if (count < 4)
            {
                a = 0;
                b = 0;
                maxErr = 0;
                r = 0;

                return;
            }
            //求x,y的和
            for (int i = 0; i < count; i++)
            {
                sumX += x[i];
                sumY += y[i];
            }
            //求x,y的平均数
            avgX = sumX / count;
            avgY = sumY / count;

            double SPxy = 0;
            double SSx = 0;
            double SSy = 0;
            for (int i = 0; i < count; i++)
            {
                SPxy += (x[i] - avgX) * (y[i] - avgY);
                SSx += (x[i] - avgX) * (x[i] - avgX);
                SSy += (y[i] - avgY) * (y[i] - avgY);
            }
            //如果所有点的x相同，直线平行于y轴，无法计算。
            //如果所有点的y相同直线为平行于x轴的直线y=k+0*x
            if (SSy == 0)
            {
                a = y[1];
                b = 0;
                r = 0;
                maxErr = 0;
                //return -1;
            }
            //y=bx+a
            b = SPxy / SSx;
            a = avgY - b * avgX;

            //开始计算R²值
            r1 = SPxy * SPxy;//分子的平方
            r2 = SSx * SSy;//分母的平方
            r = r1 / r2;    //计算R²值

            //下面代码计算最大偏差            
            maxErr = 0;
            for (int i = 0; i < count; i++)
            {
                double yi = a + b * x[i];
                double absErrYi = Math.Abs(yi - y[i]);//假动作

                if (absErrYi > maxErr)
                {
                    maxErr = absErrYi;
                }
            }


        }
    }
}

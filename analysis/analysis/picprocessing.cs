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
    public partial class picprocessing : Form
    {
        private Form1 fm1;
        public picprocessing()
        {
            InitializeComponent();
        }
        public picprocessing(Form1 form1)
        {
            InitializeComponent();
            fm1 = form1;
        }

    }

}

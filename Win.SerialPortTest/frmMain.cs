using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win.SerialPortTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button_Receive_Click(object sender, EventArgs e)
        {
            frmReceive recevieFrm = new frmReceive();
            recevieFrm.Show();

        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            frmSend sendForm = new frmSend();
            sendForm.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibService;

namespace DADSTORM
{
    public partial class MasterForm : Form
    {
        public DelAddMsgToLog formDelegateAddToLog;
        public DelExecutaScprit formDelegateExecutaScript;
        public DelAddMsgToLog sendCommand;

        public MasterForm()
        {
            InitializeComponent();
        }
        public void sendMessageToLog(string message)
        {
            this.LogBox.AppendText("\r\n" + message);
        }

        private void run_button_Click(object sender, EventArgs e)
        {
            string text = runBox.Text;
            int mode = 0;
            formDelegateExecutaScript(text,mode);
        }

        private void LogBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void send_button_Click(object sender, EventArgs e)
        {
            string text2 = sendBox.Text;
            sendCommand(text2);
        }

        private void MasterForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = runBox.Text;
            int mode = 1;
            formDelegateExecutaScript(text,mode);
        }
    }
}
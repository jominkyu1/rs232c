using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stdRS232C
{
    public partial class Form1 : Form
    {
        private readonly string PID = Process.GetCurrentProcess().Id.ToString();
        private readonly SerialPort sPort = new SerialPort();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sPort.DataReceived += OnDataReceived;

            lblID.Text = PID;

            InitSerialItems();
        }

        private void InitSerialItems()
        {
            //Serial Ports
            foreach (string port in SerialPort.GetPortNames()) cbSerialPort.Items.Add(port);
            cbSerialPort.SelectedIndex = cbSerialPort.Items.Count - 1;

            //Baud Rate
            int rate = 600;
            for (int i = 0; i < 5; i++)
            {
                cbBaudRate.Items.Add(rate);
                rate *= 2;
            }
            cbBaudRate.SelectedIndex = cbBaudRate.Items.Count - 1;
            
            //DataSize
            cbDatabits.Items.Add(8);
            cbDatabits.SelectedIndex = 0;

            //Parity
            cbParity.Items.Add("NONE");
            cbParity.SelectedIndex = 0;
            cbParity.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //이미 연결되어있다면
            if (sPort.IsOpen)
            {
                sPort.Close();
                DisplayEnableDisable();
            }
            else
            {
                try
                {
                    sPort.PortName = cbSerialPort.SelectedItem.ToString();
                    sPort.BaudRate = (int)cbBaudRate.SelectedItem;
                    sPort.DataBits = (int)cbDatabits.SelectedItem;
                    sPort.Parity = Parity.None;
                    sPort.Encoding = Encoding.UTF8;
                    
                    sPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                DisplayEnableDisable();
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string receivedString = sPort.ReadExisting();
            this.Invoke(new MethodInvoker(() =>
            {
                txtSendReceive.AppendText(receivedString);
            }));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = $"[{PID}] {txtSend.Text}\r\n";

                sPort.Write(msg);
                txtSendReceive.AppendText(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            txtSend.Text = "";
            txtSend.Focus();
        }

        private void DisplayEnableDisable()
        {
            cbSerialPort.Enabled = !sPort.IsOpen;
            cbBaudRate.Enabled = !sPort.IsOpen;
            cbDatabits.Enabled = !sPort.IsOpen;

            lblStatus.Text = sPort.IsOpen ? "CONNECTED" : "NOT CONNECTED";
            btnConnect.Text = sPort.IsOpen ? "DISCONNECT" : "CONNECT";
        }

    }
}

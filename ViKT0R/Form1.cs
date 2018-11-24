/*
    ViKT0R - Syslog scanner
    Copyright (C) - 2018
    Coded by George Delaportas (G0D/ViR4X)
*/



using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ViKT0R
{
    public partial class VForm : Form
    {
        private Thread New_Thread;
        private UdpClient UDP_Listener;
        private IPEndPoint End_Points;
        private bool isRunning = false;

        public VForm()
        {
            InitializeComponent();
        }

        private void VForm_Load(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
        }

        private void scanUDP()
        {
            isRunning = true;

            UDP_Listener = new UdpClient(514);
            End_Points = new IPEndPoint(IPAddress.Any, 514);

            try
            {
                while (true)
                {
                    byte[] bytes = UDP_Listener.Receive(ref End_Points);

                    textBox1.AppendText("[SOURCE]" + Environment.NewLine);
                    textBox1.AppendText(End_Points.ToString() + Environment.NewLine + Environment.NewLine);
                    textBox1.AppendText("[MESSAGE]" + Environment.NewLine);
                    textBox1.AppendText(Encoding.ASCII.GetString(bytes, 0, bytes.Length) + 
                                        Environment.NewLine + Environment.NewLine + Environment.NewLine);

                    File.AppendAllText(Application.StartupPath + "/ViKT0R.log", textBox1.Text + Environment.NewLine);

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233040)
                    MessageBox.Show(ex.Message.ToString(), "ViKT0R");
            }
            finally
            {
                UDP_Listener.Close();
            }
        }

        private void VForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isRunning)
            {
                New_Thread.Abort();

                UDP_Listener.Close();

                Application.ExitThread();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            New_Thread = new Thread(scanUDP);

            New_Thread.Start();

            button1.Enabled = false;
        }
    }
}

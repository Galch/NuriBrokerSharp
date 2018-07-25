using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NuriBrokerSharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            updateSerialPorts();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void updateSerialPorts()
        {
            var ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label_deg.Text = "Deg = " + ((Double)trackBar1.Value / 100) + " [Degree]";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label_vel.Text = "Vel = " + ((Double)trackBar2.Value / 100) + " [RPM]";
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label_degT.Text = "Deg Transition Time = " + ((Double)trackBar3.Value / 10) + " [sec]";

        }
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label_velT.Text = "Vel Transition Time = " + ((Double)trackBar4.Value / 10) + " [sec]";
        }
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            if (trackBar5.Value == 0)
                label9.Text = "Direction = Right";
            else
                label9.Text = "Direction = Left";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Connect")
            {
                if (comboBox1.SelectedIndex > -1)
                {
                    serialPort1.PortName = comboBox1.SelectedText;
                    serialPort1.Open();
                    button1.Text = "Discon";
                }
            }
            else
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);
                double dir = trackBar5.Value;
                double pos = (Double)trackBar1.Value / 100;
                double vel = (Double)trackBar2.Value / 100;

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_SET_POS_VEL, dir, pos, vel);

                serialPort1.Write(pac, 0, pac.Count());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);
                double dir = trackBar5.Value;
                double pos = (Double)trackBar1.Value / 100;
                double pos_time = trackBar3.Value;

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_SET_POS_RAMP, dir, pos, pos_time);

                serialPort1.Write(pac, 0, pac.Count());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);
                double dir = trackBar5.Value;
                double vel = (Double)trackBar2.Value / 100;
                double vel_time = trackBar4.Value;

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_SET_VEL_RAMP, dir, vel, vel_time);

                serialPort1.Write(pac, 0, pac.Count());
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_FACTORY_RESET);

                serialPort1.Write(pac, 0, pac.Count());
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);
                int Kp = Convert.ToInt32(textBox2.Text);
                int Ki = Convert.ToInt32(textBox3.Text);
                int Kd = Convert.ToInt32(textBox4.Text);
                int Imax = Convert.ToInt32(textBox5.Text) * 10;

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_SET_POS_PID, Kp, Ki, Kd, Imax);

                serialPort1.Write(pac, 0, pac.Count());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                int devID = Convert.ToInt32(textBox_ID.Text);
                int Kp = Convert.ToInt32(textBox9.Text);
                int Ki = Convert.ToInt32(textBox8.Text);
                int Kd = Convert.ToInt32(textBox7.Text);
                int Imax = Convert.ToInt32(textBox6.Text) * 10;

                byte[] pac = NuriProtocol.GetPacket(devID, NuriMode.NURI_SET_VEL_PID, Kp, Ki, Kd, Imax);

                serialPort1.Write(pac, 0, pac.Count());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace Plot
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

            string[] args = Environment.GetCommandLineArgs();

            try
            {

                StreamReader streamReaderChart = new StreamReader(args[1]);

                double ra = double.Parse(streamReaderChart.ReadLine().Split(new char[] { ' ' })[1], CultureInfo.InvariantCulture);
                double dec = double.Parse(streamReaderChart.ReadLine().Split(new char[] { ' ' })[1], CultureInfo.InvariantCulture);
                double fov = double.Parse(streamReaderChart.ReadLine().Split(new char[] { ' ' })[1], CultureInfo.InvariantCulture);

                this.Text += " " + ProductVersion + " [" + args[1] + "] FoV: " + fov.ToString(CultureInfo.InvariantCulture);

                streamReaderChart.Close();

                Sky sky = new Sky(new Viewport(pictureBox.Width, pictureBox.Height), ra, dec, fov, args[1]);
                pictureBox.Image = sky.Render(false);

            }
            catch (Exception exception)
            {

                labelError.Visible = true;
                labelError.Text = exception.Message + "\r\n";
                labelError.Text += exception.StackTrace + "\r\n";
           
            }

        }

    }

}
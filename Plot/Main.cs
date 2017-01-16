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

            double ra = double.Parse(args[1], CultureInfo.InvariantCulture);
            double dec = double.Parse(args[2], CultureInfo.InvariantCulture);
            double fov = double.Parse(args[3], CultureInfo.InvariantCulture);

            Sky sky = new Sky(new Viewport(pictureBox.Width, pictureBox.Height), ra, dec, fov);
            pictureBox.Image = sky.Render(args[4]);

        }

    }
}

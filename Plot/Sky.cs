using System;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace Plot
{

    class Sky
    {

        public Color LatitudeColor = Color.Red;
        public Color LongitudeColor = Color.DarkRed;

        private Viewport Viewport = null;
        private double RA;
        private double Dec;
        private double FoV;

        private String ChartFileName;

        public Sky(Viewport viewport, double ra, double dec, double fov, string chartFileName)
        {
            this.Viewport = viewport;
            this.ChartFileName = chartFileName;
            StreamReader streamReaderChart = new StreamReader(this.ChartFileName);
            this.RA = ra;
            this.Dec = dec;
            this.FoV = fov;
        }

        public Bitmap Render(bool drawSphere)
        {

            Bitmap bitmap = new Bitmap(Viewport.Width, Viewport.Height);
            Graphics3d graphics3d = new Graphics3d(
                bitmap, 
                this.Dec * Math.PI / 180 * -1, 
                (this.RA * 360 / 24) * Math.PI / 180 * -1, 
                this.FoV * Math.PI / 180);

            if (drawSphere) graphics3d.DrawSphere(LatitudeColor, LongitudeColor, Viewport.Height / 2);

            string line;
            int index = 1;
            StreamReader streamReaderChart = new StreamReader(this.ChartFileName);
            while ((line = streamReaderChart.ReadLine()) != null)
            {

                try
                {

                    string[] data = line.Split(new char[] { '|' });

                    int name = int.Parse(data[0]);
                    double vmag = double.Parse(data[1], CultureInfo.InvariantCulture);
                    string spect_type = data[2];
                    double ra = (double.Parse(data[3], CultureInfo.InvariantCulture) * 360 / 24) * Math.PI / 180;
                    double dec = double.Parse(data[4], CultureInfo.InvariantCulture) * Math.PI / 180;

                    graphics3d.DrawPointSpherical(Color.White, dec, ra, Viewport.Height / 2, (int)(15 - vmag));
                    graphics3d.DrawStringSpherical(Color.OrangeRed, 8, dec + 0.0006, ra + 0.0006, Viewport.Height / 2, index.ToString());

                    index++;

                }
                catch(Exception)
                { }


            }
            streamReaderChart.Close();
            return bitmap;

        }

       
    }

}
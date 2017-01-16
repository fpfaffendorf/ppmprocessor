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

        public Viewport Viewport = null;
        public double RA = 0;
        public double Dec = 0;
        public double FoV = 1;

        public Sky(Viewport viewport, double ra, double dec, double fov)
        {
            this.Viewport = viewport;
            this.RA = ra;
            this.Dec = dec;
            this.FoV = fov;
        }

        public Bitmap Render(string chartFileName)
        {

            Bitmap bitmap = new Bitmap(Viewport.Width, Viewport.Height);
            Graphics3d graphics3d = new Graphics3d(
                bitmap, 
                this.Dec * Math.PI / 180 * -1, 
                (this.RA * 360 / 24) * Math.PI / 180 * -1, 
                this.FoV);

            graphics3d.DrawSphere(LatitudeColor, LongitudeColor, Viewport.Height / 2);

            string line;
            StreamReader streamReaderChart = new StreamReader(chartFileName);
            while ((line = streamReaderChart.ReadLine()) != null)
            {

                string[] data = line.Split(new char[] { '|' });

                try
                {

                    int name = int.Parse(data[0]);
                    double vmag = double.Parse(data[1], CultureInfo.InvariantCulture);
                    string spect_type = data[2];
                    double ra = (double.Parse(data[3], CultureInfo.InvariantCulture) * 360 / 24) * Math.PI / 180;
                    double dec = double.Parse(data[4], CultureInfo.InvariantCulture) * Math.PI / 180;

                    graphics3d.DrawPointSpherical(Color.White, dec, ra, Viewport.Height / 2, 5);

                }
                catch(Exception)
                { }


            }

            return bitmap;

        }

       
    }

}
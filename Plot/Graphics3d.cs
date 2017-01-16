using System;
using System.Drawing;

namespace Plot
{
    class Graphics3d
    {

        private Graphics Graphics;
        private Bitmap Bitmap;
        private double ThitaX;
        private double ThitaY;
        private double Aspect = 0;
        private double FoV = 0;
        private const double NearZ = 100;
        private const double FarZ = 1000;

        private double[,] RotateY = new double[3, 3];
        private double[,] RotateX = new double[3, 3];
        private double[,] Camera = new double[4, 4];

        public Graphics3d(Bitmap bitmap, double thitaX, double thitaY, double fov)
        {

            this.Bitmap = bitmap;
            this.Graphics = Graphics.FromImage(bitmap);
            this.ThitaX = thitaX;
            this.ThitaY = thitaY;
            this.Aspect = bitmap.Width / bitmap.Height;
            this.FoV = fov;

            this.RotateY[0, 0] = Math.Cos(thitaY);
            this.RotateY[0, 2] = Math.Sin(thitaY) * -1;
            this.RotateY[1, 1] = 1;
            this.RotateY[2, 0] = Math.Sin(thitaY);
            this.RotateY[2, 2] = Math.Cos(thitaY);

            this.RotateX[0, 0] = 1;
            this.RotateX[1, 1] = Math.Cos(thitaX);
            this.RotateX[1, 2] = Math.Sin(thitaX);
            this.RotateX[2, 1] = Math.Sin(thitaX) * -1;
            this.RotateX[2, 2] = Math.Cos(thitaX);

            this.Camera[0, 0] = 1 / (this.Aspect * Math.Tan(this.FoV / 2));
            this.Camera[1, 1] = 1 / (Math.Tan(this.FoV / 2));
            this.Camera[2, 2] = ((-1 * NearZ) - FarZ) / (NearZ - FarZ);
            this.Camera[2, 3] = 1;
            this.Camera[3, 2] = (2 * FarZ * NearZ) / (NearZ - FarZ);

        }

        private void MapCoordinates(ref double x, ref double y, ref double z)
        {
            y *= -1;

            // ---------------------------------------------------------
            // Rotate Y Axis
            // ---------------------------------------------------------

            double[,] vector = new double[1, 3];
            vector[0, 0] = x;
            vector[0, 1] = y;
            vector[0, 2] = z;

            double[,] r = AstronomyMath.MultiplyMatrix(vector, this.RotateY);

            x = r[0, 0];
            y = r[0, 1];
            z = r[0, 2];

            // ---------------------------------------------------------
            // Rotate X Axis
            // ---------------------------------------------------------

            vector = new double[1, 3];
            vector[0, 0] = x;
            vector[0, 1] = y;
            vector[0, 2] = z;

            r = AstronomyMath.MultiplyMatrix(vector, this.RotateX);

            x = r[0, 0];
            y = r[0, 1];
            z = r[0, 2];

            // ---------------------------------------------------------
            // Camera 
            // ---------------------------------------------------------

            double[,] vectorW = new double[1, 4];
            vectorW[0, 0] = x;
            vectorW[0, 1] = y;
            vectorW[0, 2] = z;
            vectorW[0, 3] = 1;

            r = AstronomyMath.MultiplyMatrix(vectorW, this.Camera);

            x = r[0, 0];
            y = r[0, 1];
            z = r[0, 2];            

        }

        public void DrawPointCartesian(Color color, double x, double y, double z, int diameter)
        {

            this.MapCoordinates(ref x, ref y, ref z);

            // Solid points only
            if (z < 0) return;

            double originX = Bitmap.Width / 2 - x;
            double originY = Bitmap.Height / 2 + y;

            SolidBrush brush = new System.Drawing.SolidBrush(color);

            this.Graphics.FillEllipse(
                brush,
                (int)Math.Truncate(originX),
                (int)Math.Truncate(originY),
                diameter, diameter);

        }

        public void DrawPointSpherical(Color color, double latitude, double longitude, double radius, int diameter)
        {

            latitude *= -1;

            double thita = latitude + Math.PI / 2;
            double phi = longitude - Math.PI / 2;

            this.DrawPointCartesian(color,
                                    radius * Math.Sin(thita) * Math.Cos(phi),
                                    radius * Math.Cos(thita),
                                    radius * -1 * Math.Sin(thita) * Math.Sin(phi),
                                    diameter);

        }

        public void DrawStringCartesian(Color color, int size, double x, double y, double z, string str)
        {

            this.MapCoordinates(ref x, ref y, ref z);

            // Solid points only
            if (z < 0) return;

            double originX = Bitmap.Width / 2 - x;
            double originY = Bitmap.Height / 2 + y;

            SolidBrush brush = new System.Drawing.SolidBrush(color);
            Font font = new Font(FontFamily.GenericMonospace, size);

            this.Graphics.DrawString(str, 
                font, 
                brush,
                (int)Math.Truncate(originX),
                (int)Math.Truncate(originY));

        }

        public void DrawStringSpherical(Color color, int size, double latitude, double longitude, double radius, string str)
        {

            latitude *= -1;

            double thita = latitude + Math.PI / 2;
            double phi = longitude - Math.PI / 2;

            this.DrawStringCartesian(color, 
                                     size,
                                     radius * Math.Sin(thita) * Math.Cos(phi),
                                     radius * Math.Cos(thita),
                                     radius * -1 * Math.Sin(thita) * Math.Sin(phi),
                                     str);

        }

        public void DrawSphere(Color colorLatitude, Color colorLongitude, double radius)
        {

            // Draw Latitude Lines
            for (double thita = 0; thita < Math.PI; thita += Math.PI / 18)
            {
                double x = radius * Math.Sin(thita);
                double y = radius * Math.Cos(thita);
                double z = radius * -1 * Math.Sin(thita);
                for (double phi = 0; phi < 2 * Math.PI; phi += .03)
                {
                    this.DrawPointCartesian(
                            colorLatitude,
                            x * Math.Cos(phi),
                            y,
                            z * Math.Sin(phi),
                            2
                        );
                }
            }

            // Draw Longitude Lines
            for (double phi = 0; phi < Math.PI - 0.1; phi += Math.PI / 18)
            {
                double x = radius * Math.Cos(phi);
                double z = radius * -1 * Math.Sin(phi);
                for (double thita = 0; thita < 2 * Math.PI; thita += .03)
                {
                    if (((thita > 0.174) && (thita < 2.967)) ||
                        ((thita > 3.316) && (thita < 6.108)))
                    {
                        this.DrawPointCartesian(
                                colorLongitude,
                                Math.Sin(thita) * x,
                                radius * Math.Cos(thita),
                                Math.Sin(thita) * z,
                                2
                            );
                    }
                }
            }

        }

    }

}
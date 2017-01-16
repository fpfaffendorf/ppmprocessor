using System;
using System.Drawing;
using System.Drawing.Text;

namespace PPMProcessor
{
    class Graphics3d
    {

        // Canvas
        private Graphics Graphics;
        private Bitmap Bitmap;
        // Latitude Camera RAD
        private double Latitude;
        // Longitude Camera RAD
        private double Longitude;
        // Aspect Ratio
        private double Aspect = 1;
        // Field of View RAD
        private double FoV = 0;
        // Near and Far Z
        private const double NearZ = 100;
        private const double FarZ = 1000;

        // Rotation Matrix
        private double[,] RotateY = new double[3, 3];
        private double[,] RotateX = new double[3, 3];
        private double[,] Camera = new double[4, 4];

        // Current line point
        private Point currentLinePoint = new Point(-1, -1);

        // Constructor
        public Graphics3d(Bitmap bitmap, double latitude, double longitude, double fov)
        {

            longitude *= -1;

            this.Bitmap = bitmap;
            this.Graphics = Graphics.FromImage(bitmap);
            this.Graphics.FillRectangle(
                new System.Drawing.SolidBrush(Color.White),
                0,
                0,
                bitmap.Width,
                bitmap.Height
            );
            this.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.FoV = fov;

            this.RotateY[0, 0] = Math.Cos(longitude);
            this.RotateY[0, 2] = Math.Sin(longitude) * -1;
            this.RotateY[1, 1] = 1;
            this.RotateY[2, 0] = Math.Sin(longitude);
            this.RotateY[2, 2] = Math.Cos(longitude);

            this.RotateX[0, 0] = 1;
            this.RotateX[1, 1] = Math.Cos(latitude);
            this.RotateX[1, 2] = Math.Sin(latitude);
            this.RotateX[2, 1] = Math.Sin(latitude) * -1;
            this.RotateX[2, 2] = Math.Cos(latitude);

            this.Camera[0, 0] = 1 / (this.Aspect * Math.Tan(this.FoV / 2));
            this.Camera[1, 1] = 1 / (Math.Tan(this.FoV / 2));
            this.Camera[2, 2] = ((-1 * NearZ) - FarZ) / (NearZ - FarZ);
            this.Camera[2, 3] = 1;
            this.Camera[3, 2] = (2 * FarZ * NearZ) / (NearZ - FarZ);

        }

        // Map XYZ coordinates for the current camera
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

        // Draw a point using cartesian coordinates
        private void DrawPointCartesian(Color color, double x, double y, double z, int diameter)
        {

            this.MapCoordinates(ref x, ref y, ref z);
            if (z < 0) return;

            double originX = Bitmap.Width / 2 - x;
            double originY = Bitmap.Height / 2 - y;

            SolidBrush brush = new System.Drawing.SolidBrush(color);

            this.Graphics.FillEllipse(
                brush,
                (int)(Math.Truncate(originX) - (diameter / 2)),
                (int)(Math.Truncate(originY) - (diameter / 2)),
                diameter, diameter);

        }

        // Draw a point using spherical coordinates
        public void DrawPointSpherical(Color color, double latitude, double longitude, double radius, int diameter)
        {

            latitude = latitude + Math.PI / 2;
            longitude = longitude - Math.PI / 2;

            this.DrawPointCartesian(color,
                                    radius * Math.Sin(latitude) * Math.Cos(longitude),
                                    radius * Math.Cos(latitude),
                                    radius * -1 * Math.Sin(latitude) * Math.Sin(longitude),
                                    diameter);

        }

        // Draw line reset current coordinates
        public void DrawLineReset()
        {
            this.currentLinePoint = new Point(-1, -1);
        }

        // Draw a line from current coordinates to given coordinates using cartesian coordinates
        private void DrawLineCartesian(Color color, double x, double y, double z)
        {

            this.MapCoordinates(ref x, ref y, ref z);
            if (z < 0) return;

            double originX = Bitmap.Width / 2 - x;
            double originY = Bitmap.Height / 2 - y;

            Pen pen = new Pen(color);
            Point point = new Point((int)Math.Truncate(originX), (int)Math.Truncate(originY));

            if ((this.currentLinePoint.X >= 0) && (this.currentLinePoint.Y >= 0))
            {
                this.Graphics.DrawLine(
                    pen,
                    this.currentLinePoint,
                    point
                    );
            }

            this.currentLinePoint = point;

        }

        // Draw a line from current coordinates to given coordinates using spherical coordinates
        public void DrawLineSpherical(Color color, double latitude, double longitude, double radius)
        {

            latitude = latitude + Math.PI / 2;
            longitude = longitude - Math.PI / 2;

            this.DrawLineCartesian(color,
                                   radius * Math.Sin(latitude) * Math.Cos(longitude),
                                   radius * Math.Cos(latitude),
                                   radius * -1 * Math.Sin(latitude) * Math.Sin(longitude));

        }

        // Draw a string using cartesian coordinates
        private void DrawStringCartesian(Color color, int size, double x, double y, double z, string str)
        {

            this.MapCoordinates(ref x, ref y, ref z);
            if (z < 0) return;

            double originX = Bitmap.Width / 2 - x;
            double originY = Bitmap.Height / 2 - y;

            SolidBrush brush = new System.Drawing.SolidBrush(color);
            Font font = new Font(FontFamily.GenericMonospace, size);

            this.Graphics.DrawString(str, 
                font, 
                brush,
                (int)Math.Truncate(originX),
                (int)Math.Truncate(originY));

        }

        // Draw a string using spherical coordinates
        public void DrawStringSpherical(Color color, int size, double latitude, double longitude, double radius, string str)
        {

            latitude = latitude + Math.PI / 2;
            longitude = longitude - Math.PI / 2;

            this.DrawStringCartesian(color, 
                                     size,
                                     radius * Math.Sin(latitude) * Math.Cos(longitude),
                                     radius * Math.Cos(latitude),
                                     radius * -1 * Math.Sin(latitude) * Math.Sin(longitude),
                                     str);

        }

        // Draw coordinates grid
        public void DrawGrid(Color colorLatitude, Color colorLongitude, double radius)
        {

            double[] latitudeArray = {
                (this.Latitude) + this.FoV / 3,
                (this.Latitude) - this.FoV / 3
            };

            // Draw Latitude Lines
            foreach(double latitude in latitudeArray)
            {
                this.DrawLineReset();
                for (double longitude = 0; longitude < 2 * Math.PI; longitude += this.FoV / 20)
                {
                    this.DrawLineSpherical(
                            colorLatitude,
                            latitude,
                            longitude,
                            radius
                        );
                }
            }

            double[] longitudeArray = {
                (Math.Abs(this.Latitude) != Math.PI / 2 ? (this.Longitude * -1) + this.FoV / (Math.Cos(this.Latitude) * 3) : 0),
                (Math.Abs(this.Latitude) != Math.PI / 2 ? (this.Longitude * -1) - this.FoV / (Math.Cos(this.Latitude) * 3) : Math.PI / 2)
            };

            // Draw Longitude Lines
            foreach (double longitude in longitudeArray)
            {
                this.DrawLineReset();
                for (double latitude = 0; latitude < 2 * Math.PI; latitude += this.FoV / 20)
                {
                    this.DrawLineSpherical(
                            colorLongitude,
                            latitude,
                            longitude,
                            radius
                        );
                }

            }

        }

        // Draw circular FoV
        public void DrawFoV(Color color)
        {
            this.Graphics.DrawArc(new Pen(color), 0, 0, this.Bitmap.Width - 1, this.Bitmap.Height - 1, 0, 360);
        }

    }

}
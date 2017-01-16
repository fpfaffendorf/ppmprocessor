using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPMProcessor
{
    class AstronomyMath
    {

        // Multiply two matrixes 
        public static double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0;
            double[,] kHasil = new double[rA, cB];
            if (cA != rB)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }
        }

        // Angular distance (RAD) between two celestial bodies
        public static double AngularDistance(double ra1, double dec1, double ra2, double dec2)
        {
            return Math.Acos(Math.Sin(dec1) * Math.Sin(dec2) + Math.Cos(dec1) * Math.Cos(dec2) * Math.Cos(ra1 - ra2));
        }

        // Convert from Deg to RAD
        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        // Convert from Rad to Deg
        public static double RadToDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        // Convert from Deg to Hour
        public static double DegToHour(double deg)
        {
            return deg * 24 / 360;
        }

        // Convert from Hour to Deg
        public static double HourToDeg(double hour)
        {
            return hour * 360 / 24;
        }

        // Convert from Hour to HMS
        public static double[] HourToHMS(double hour)
        {
            double[] hms = new double[3];
            hms[0] = (int)Math.Truncate(hour);
            hms[1] = Math.Truncate((hour - hms[0]) * 60);
            hms[2] = (((hour - hms[0]) * 60) - hms[1]) * 60;
            return hms;
        }

        // Convert from Deg to DMS
        public static double[] DegToDMS(double deg)
        {
            double[] dms = new double[3];
            dms[0] = Math.Abs((int)Math.Truncate(deg));
            dms[1] = Math.Abs(Math.Truncate((deg - dms[0]) * 60));
            dms[2] = Math.Abs((((deg - dms[0]) * 60) - dms[1]) * 60);
            if (deg < 0) { dms[0] *= -1; dms[1] *= -1; dms[2] *= -1; }
            return dms;
        }

    }

}
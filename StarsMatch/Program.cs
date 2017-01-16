using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace StarsMatch
{
    class Program
    {
        static void Main(string[] args)
        {

            StreamReader streamReaderStars = new StreamReader("catalog/star_names.tdat");

            StreamWriter streamWriter = new StreamWriter("catalog/names.tdat");

            string line_stars;
            string line_ppm;

            bool first = true;

            while ((line_stars = streamReaderStars.ReadLine()) != null)
            {              

                int ra_h_stars = int.Parse(line_stars.Substring(0, 2));
                int ra_m_stars = int.Parse(line_stars.Substring(3, 2));

                int dec_d_stars = int.Parse(line_stars.Substring(13, 3));
                int dec_m_stars = int.Parse(line_stars.Substring(17, 2));

                int mag_stars = int.Parse(line_stars.Substring(27, 2));

                string name_stars = line_stars.Substring(34).Replace("[", "").Replace("]", "").Replace("(v)", "").Replace("(q)", "").Trim();


                StreamReader streamReaderPPM = new StreamReader("catalog/export.tdat");
                while ((line_ppm = streamReaderPPM.ReadLine()) != null)
                {

                    string[] data_ppm = line_ppm.Split(new char[] { '|' });
                    string name_ppm = data_ppm[0];

                    int ra_h_ppm = int.Parse(data_ppm[3].Split(new char[] { ' ' })[0]);
                    int ra_m_ppm = int.Parse(data_ppm[3].Split(new char[] { ' ' })[1]);

                    int dec_d_ppm = int.Parse(data_ppm[4].Split(new char[] { ' ' })[0]);
                    int dec_m_ppm = int.Parse(data_ppm[4].Split(new char[] { ' ' })[1]);

                    double mag_ppm = double.Parse(data_ppm[1], CultureInfo.InvariantCulture);

                    if (ra_h_stars == ra_h_ppm)
                    if (ra_m_stars == ra_m_ppm)
                    if (dec_d_stars == dec_d_ppm)
                    if (dec_m_stars == dec_m_ppm)
                    //if ((mag_stars >= mag_ppm - 1) && (mag_stars <= mag_ppm + 1))
                    {

                        if (!first) streamWriter.WriteLine();
                        first = false;
                        streamWriter.Write(name_ppm + "|" + name_stars);
                        break;

                    }

                }

            }

            streamWriter.Close();

        }
    }
}

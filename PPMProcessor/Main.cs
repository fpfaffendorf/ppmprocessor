using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.IO.Compression;

namespace PPMProcessor
{
    public partial class Main : Form
    {

        private FormConsole FormConsole = new FormConsole();

        private String FileNamePPMCatalog = "";
        private String FileNameNames = "";

        private Dictionary<string, string> Names = new Dictionary<string, string>();

        public Main()
        {
            InitializeComponent();
            FormConsole.Owner = this;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.FileNamePPMCatalog = config.AppSettings.Settings["FileNamePPMCatalog"].Value;
            this.FileNameNames = config.AppSettings.Settings["FileNameNames"].Value;
            string fileNamePPMCatalogZip = this.FileNamePPMCatalog.Replace(".tdat", ".zip");
            if (!File.Exists(this.FileNamePPMCatalog))
            {
                try
                {
                    ZipFile.ExtractToDirectory(fileNamePPMCatalogZip, this.FileNamePPMCatalog.Replace("heasarc_ppm.tdat", ""));
                }
                catch(Exception)
                {

                }
            }
            try { File.Delete(fileNamePPMCatalogZip); } catch (Exception) { }
        }

        private void Main_Load(object sender, EventArgs e)
        {

            this.Text = "PPM Processor " + ProductVersion;
            dataGridView.ColumnCount = 26;

            dataGridView.Columns[0].Name = "name";
            dataGridView.Columns[1].Name = "dm_number";
            dataGridView.Columns[2].Name = "vmag";
            dataGridView.Columns[2].DefaultCellStyle.Format = "N2";
            dataGridView.Columns[3].Name = "spect_type";
            dataGridView.Columns[4].Name = "ra";
            dataGridView.Columns[5].Name = "dec";
            dataGridView.Columns[6].Name = "lii";
            dataGridView.Columns[7].Name = "bii";
            dataGridView.Columns[8].Name = "ra_cat";
            dataGridView.Columns[9].Name = "dec_cat";
            dataGridView.Columns[10].Name = "ra_prop";
            dataGridView.Columns[11].Name = "dec_prop";
            dataGridView.Columns[12].Name = "n_pub";
            dataGridView.Columns[13].Name = "ra_mean_err";
            dataGridView.Columns[14].Name = "dec_mean_err";
            dataGridView.Columns[15].Name = "pm_ra_mean_err";
            dataGridView.Columns[16].Name = "pm_dec_mean_err";
            dataGridView.Columns[17].Name = "epa";
            dataGridView.Columns[18].Name = "epd";
            dataGridView.Columns[19].Name = "sao";
            dataGridView.Columns[20].Name = "hd";
            dataGridView.Columns[21].Name = "agk3";
            dataGridView.Columns[22].Name = "cpd";
            dataGridView.Columns[23].Name = "notes";
            dataGridView.Columns[24].Name = "class";
            dataGridView.Columns[25].Name = "common_name";

            foreach(DataGridViewColumn c in dataGridView.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            StreamReader streamReader = new StreamReader(this.FileNameNames);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] s = line.Split(new char[] { '|' });
                this.Names.Add(s[0], s[1]);
            }

            toolStripMenuItemConsole_Click(sender, e);

        }

        private void toolStripMenuItemConsole_Click(object sender, EventArgs e)
        {
            if (!FormConsole.Visible)
            {
                FormConsole.Left = this.Width - FormConsole.Width - 45;
                FormConsole.Top = this.Height - FormConsole.Height - 45;
                FormConsole.Show();
                FormConsole.BringToFront();
            }
            else
            {
                FormConsole.Hide();
            }
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(dataGridView.CurrentCell.Value.ToString());
        }
                
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !(MessageBox.Show("Closing PPM Processor.\r\nAre you sure you want to quit ?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }

        public void List()
        {

            tabControl.SelectedTab = tabPageList;

            switch (FormConsole.TypeOfList)
            {
                case "basic":
                {
                    HideColumns(new int[] { 1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 24 });
                }
                break;

                case "all":
                {
                    HideColumns(new int[] { });
                }
                break;
            }

            StreamReader streamReader = new StreamReader(this.FileNamePPMCatalog);
            StreamWriter streamWriter = null;
            if (FormConsole.FileName != null)
            {
                try { File.Delete(FormConsole.FileName); } catch (Exception) {}
                streamWriter = new StreamWriter(FormConsole.FileName);
                bool first = true;
                streamWriter.WriteLine("ra " + FormConsole.RA.ToString(CultureInfo.InvariantCulture));
                streamWriter.WriteLine("dec " + FormConsole.Dec.ToString(CultureInfo.InvariantCulture));
                streamWriter.WriteLine("fov " + FormConsole.FoV.ToString(CultureInfo.InvariantCulture));
                foreach (DataGridViewColumn c in dataGridView.Columns)
                {
                    if (c.Visible)
                    {
                        streamWriter.Write((first ? "" : "|") + c.HeaderText);
                        first = false;
                    }
                }
                streamWriter.WriteLine();
            }

            String line;
            int count = 0;
            bool dataFound = false;
            char[] charArrayPipe = new char[] { '|' };
            char[] charArraySpace = new char[] { ' ' };
            if (FormConsole.FileName == null)
            {
                dataGridView.Visible = false;
                dataGridView.Rows.Clear();
            }
            while ((line = streamReader.ReadLine()) != null)
            {
                if (dataFound)
                {
                    if (line.Trim().ToUpper() != "<END>")
                    {
                        string[] data = line.Split(charArrayPipe);
                        
                        // Name
                        data[0] = data[0].Substring(4);
                        if ((FormConsole.PPMName != "") && (!data[0].Contains(FormConsole.PPMName))) continue;

                        // Common Name
                        this.Names.TryGetValue(data[0], out data[25]);
                        if ((FormConsole.CommonName != "") && ((data[25] == null) || (!data[25].ToLower().Contains(FormConsole.CommonName)))) continue;

                        // Magnitude
                        double vmagStar = double.Parse(data[2], CultureInfo.InvariantCulture);
                        data[2] = double.Parse(data[2], CultureInfo.InvariantCulture).ToString("N2", CultureInfo.InvariantCulture);
                        if ((vmagStar > FormConsole.VMag2) || (vmagStar < FormConsole.VMag1)) continue;

                        // Spectral Type
                        string spectStar = data[3];
                        if (!spectStar.StartsWith(FormConsole.Spect)) continue;

                        // RA
                        double raStar = double.Parse(data[4], CultureInfo.InvariantCulture) * 24 / 360; // RA H format
                        if (FormConsole.RAFormat == "h") data[4] = (raStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "d") data[4] = (raStar * 360 / 24).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "r") data[4] = (raStar * 360 / 24 * Math.PI / 180).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "hms")
                        {
                            double h = (int)Math.Truncate(raStar);
                            double m = Math.Truncate((raStar - h) * 60);
                            double s = (((raStar - h) * 60) - m) * 60;
                            data[4] = (h).ToString(CultureInfo.InvariantCulture) + " " + m.ToString(CultureInfo.InvariantCulture) + " " + s.ToString("N3", CultureInfo.InvariantCulture);
                        }

                        // Dec 
                        double decStar = double.Parse(data[5], CultureInfo.InvariantCulture); // Dec D format
                        if (FormConsole.DecFormat == "d") data[5] = (decStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.DecFormat == "r") data[5] = (decStar * Math.PI / 180).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.DecFormat == "dms")
                        {
                            double d = (int)Math.Truncate(decStar);
                            double m = Math.Truncate((decStar - d) * 60);
                            double s = (((decStar - d) * 60) - m) * 60;
                            if (decStar < 0)
                            {
                                d *= -1; m *= -1; s *= -1;
                            }
                            data[5] = (decStar < 0 ? "-" : "") + (d).ToString(CultureInfo.InvariantCulture) + " " + m.ToString(CultureInfo.InvariantCulture) + " " + s.ToString("N3", CultureInfo.InvariantCulture);
                        }

                        // FoV
                        if (FormConsole.FoV > 0)
                        {

                            double fov = (FormConsole.FoV * Math.PI / 180) / 2;
                            double d = this.angularDistance(
                                                    (FormConsole.RA * 360 / 24) * Math.PI / 180,
                                                    (FormConsole.Dec * Math.PI / 180),
                                                    (raStar * 360 / 24) * Math.PI / 180,
                                                    (decStar * Math.PI / 180));

                            if (fov < d) continue;

                        }

                        // RA Cat
                        data[8] = data[8].Replace("  ", " ");

                        // Dec Cat
                        data[9] = data[9].Replace("  ", " ");
                        data[9] = data[9].Replace("- ", "-");
                        data[9] = data[9].Replace("+", "");
                        data[9] = data[9].Trim();

                        // Notes
                        data[23] = data[23].Replace(" ", "");

                        // List                       
                        {

                            if ((FormConsole.Page - 1) * FormConsole.Records <= count)
                            {
                                if (FormConsole.FileName == null)
                                {
                                    dataGridView.Rows.Add(data);
                                    dataGridView.Rows[count - ((FormConsole.Page - 1) * FormConsole.Records)].HeaderCell.Value = (count + 1).ToString();
                                }
                                else
                                {
                                    bool first = true;
                                    foreach (DataGridViewColumn c in dataGridView.Columns)
                                    {
                                        if (c.Visible) 
                                        {
                                            streamWriter.Write((first ? "" : "|") + data[c.Index]);
                                            first = false;
                                        }
                                    }
                                    streamWriter.WriteLine();
                                }
                            }
                            if (FormConsole.FileName == null) if (++count == FormConsole.Page * FormConsole.Records) break;

                        }

                    }
                }
                else
                {
                    dataFound = (line.Trim().ToUpper() == "<DATA>");
                }

            }
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView.Visible = true;
            streamReader.Close();
            if (streamWriter != null) streamWriter.Close();
            contextMenuStripDataGridView.Enabled = true;
            toolStripMenuItemCopy.Enabled = true;
        }

        public void Center(int recordIndex, ref double RA, ref double Dec)
        {
            RA = double.Parse(dataGridView.Rows[recordIndex - 1].Cells[4].Value.ToString(), CultureInfo.InvariantCulture);
            Dec = double.Parse(dataGridView.Rows[recordIndex - 1].Cells[5].Value.ToString(), CultureInfo.InvariantCulture);
        }

        public void Chart()
        {

            tabControl.SelectedTab = tabPageChart;

            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics3d graphics3d = new Graphics3d(
                bitmap,
                FormConsole.Dec * Math.PI / 180 * -1,
                (FormConsole.RA * 360 / 24) * Math.PI / 180 * -1,
                FormConsole.FoV * Math.PI / 180);

            double raGrid = FormConsole.RA * 360 / 24;
            double decGrid = FormConsole.Dec * Math.PI / 180;

            for (double l = -90; l <= 90; l += FormConsole.FoV / 100)
            {
                graphics3d.DrawPointSpherical(Color.Red, l * Math.PI / 180, (raGrid - FormConsole.FoV / 3) * Math.PI / 180, pictureBox.Height / 2, 2);
                graphics3d.DrawPointSpherical(Color.Red, l * Math.PI / 180, (raGrid) * Math.PI / 180                      , pictureBox.Height / 2, 2);
                graphics3d.DrawPointSpherical(Color.Red, l * Math.PI / 180, (raGrid + FormConsole.FoV / 3) * Math.PI / 180, pictureBox.Height / 2, 2);
            }

            for (double l = 0; l <= 360; l += FormConsole.FoV / 100)
            {
                graphics3d.DrawPointSpherical(Color.Red, (decGrid - FormConsole.FoV / 3) * Math.PI / 180, l * Math.PI / 180, pictureBox.Height / 2, 2);
                graphics3d.DrawPointSpherical(Color.Red, (decGrid * Math.PI / 180)                      , l * Math.PI / 180, pictureBox.Height / 2, 2);
                graphics3d.DrawPointSpherical(Color.Red, (decGrid + FormConsole.FoV / 3) * Math.PI / 180, l * Math.PI / 180, pictureBox.Height / 2, 2);
            }


            int index = 1;

            foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
            {

                try
                {

                    double vmag = double.Parse(dataGridViewRow.Cells[2].Value.ToString(), CultureInfo.InvariantCulture);
                    double ra = (double.Parse(dataGridViewRow.Cells[4].Value.ToString(), CultureInfo.InvariantCulture) * 360 / 24) * Math.PI / 180;
                    double dec = double.Parse(dataGridViewRow.Cells[5].Value.ToString(), CultureInfo.InvariantCulture) * Math.PI / 180;

                    graphics3d.DrawPointSpherical(Color.Black, dec, ra, pictureBox.Height / 2, (int)(15 - vmag));
                    graphics3d.DrawStringSpherical(Color.Black, 8, dec + FormConsole.FoV / 1800, ra + FormConsole.FoV / 1800, pictureBox.Height / 2, index.ToString());

                    index++;

                }
                catch (Exception)
                { }
                
            }

            graphics3d.DrawFoV(Color.Black);
            pictureBox.Image = bitmap;

        }

        private void HideColumns(int[] columns)
        {
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                c.Visible = !columns.Contains(c.Index);
            }
        }

        private double angularDistance(double ra1, double dec1, double ra2, double dec2)
        {

            return Math.Acos(Math.Sin(dec1) * Math.Sin(dec2) + Math.Cos(dec1) * Math.Cos(dec2) * Math.Cos(ra1 - ra2));

        }

    }
}

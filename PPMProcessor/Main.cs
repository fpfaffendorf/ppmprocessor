﻿using System;
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
using System.Drawing.Imaging;

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
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
            tabPageList.Font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Regular);
        }

        private void Main_Load(object sender, EventArgs e)
        {

            this.Text = "PPM Processor " + ProductVersion;
            dataGridView.ColumnCount = 26;

            dataGridView.Columns[0].Name = "name";
            dataGridView.Columns[1].Name = "dm_number";
            dataGridView.Columns[2].Name = "vmag";
            dataGridView.Columns[2].DefaultCellStyle.Format = "N2";
            dataGridView.Columns[3].Name = "spect";
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

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(dataGridView.CurrentCell.Value.ToString());
        }
                
        public void List()
        {

            tabControl.SelectedTab = tabPageList;
            FormConsole.Focus();

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
                double ra = this.FormConsole.RA;
                double dec = this.FormConsole.Dec;
                double fov = this.FormConsole.FoV;
                streamWriter.WriteLine("ra " + ra.ToString(CultureInfo.InvariantCulture));
                streamWriter.WriteLine("dec " + dec.ToString(CultureInfo.InvariantCulture));
                streamWriter.WriteLine("fov " + fov.ToString(CultureInfo.InvariantCulture));
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
                        double raStar = AstronomyMath.DegToHour(double.Parse(data[4], CultureInfo.InvariantCulture)); // RA H format
                        if (FormConsole.RAFormat == "h") data[4] = (raStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "d") data[4] = AstronomyMath.HourToDeg(raStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "r") data[4] = AstronomyMath.DegToRad(AstronomyMath.HourToDeg(raStar)).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.RAFormat == "hms")
                        {
                            double[] hms = AstronomyMath.HourToHMS(raStar);
                            data[4] = hms[0].ToString(CultureInfo.InvariantCulture) + " " + hms[1].ToString(CultureInfo.InvariantCulture) + " " + hms[2].ToString("N3", CultureInfo.InvariantCulture);
                        }

                        // Dec 
                        double decStar = double.Parse(data[5], CultureInfo.InvariantCulture); // Dec D format
                        if (FormConsole.DecFormat == "d") data[5] = (decStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.DecFormat == "r") data[5] = AstronomyMath.DegToRad(decStar).ToString(CultureInfo.InvariantCulture);
                        else if (FormConsole.DecFormat == "dms")
                        {
                            double[] dms = AstronomyMath.DegToDMS(decStar);
                            data[5] = (decStar < 0 ? "-" : "") + dms[0].ToString(CultureInfo.InvariantCulture) + " " + dms[1].ToString(CultureInfo.InvariantCulture) + " " + dms[2].ToString("N3", CultureInfo.InvariantCulture);
                        }

                        // FoV
                        if (FormConsole.FoV > 0)
                        {

                            if (AstronomyMath.DegToRad(FormConsole.FoV) / 2 < AstronomyMath.AngularDistance(
                                                                                AstronomyMath.DegToRad(AstronomyMath.HourToDeg(FormConsole.RA)),
                                                                                AstronomyMath.DegToRad(FormConsole.Dec),
                                                                                AstronomyMath.DegToRad(AstronomyMath.HourToDeg(raStar)),
                                                                                AstronomyMath.DegToRad(decStar))) continue;
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
            FormConsole.Focus();

            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics3d graphics3d = new Graphics3d(
                bitmap,
                AstronomyMath.DegToRad(FormConsole.Dec),
                AstronomyMath.DegToRad(AstronomyMath.HourToDeg(FormConsole.RA)),
                AstronomyMath.DegToRad(FormConsole.FoV));

            graphics3d.DrawGrid(Color.Red, Color.Red, pictureBox.Height / 2);

            int index = 1;

            foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
            {

                try
                {

                    double vmag = double.Parse(dataGridViewRow.Cells[2].Value.ToString(), CultureInfo.InvariantCulture);
                    double ra = AstronomyMath.DegToRad(AstronomyMath.HourToDeg(double.Parse(dataGridViewRow.Cells[4].Value.ToString(), CultureInfo.InvariantCulture)));
                    double dec = AstronomyMath.DegToRad(double.Parse(dataGridViewRow.Cells[5].Value.ToString(), CultureInfo.InvariantCulture));

                    graphics3d.DrawPointSpherical(Color.Black, dec, ra, pictureBox.Height / 2, (int)(FormConsole.VMag2 - vmag));
                    graphics3d.DrawStringSpherical(Color.Black, 8, dec + FormConsole.FoV / 1800, ra + FormConsole.FoV / 1800, pictureBox.Height / 2, index.ToString());

                    index++;

                }
                catch (Exception)
                { }
                
            }

            graphics3d.DrawFoV(Color.Black);
            pictureBox.Image = bitmap;

        }

        public double AngularDistance(int recordIndexFrom, int recordIndexTo)
        {
            return AstronomyMath.RadToDeg(
                AstronomyMath.AngularDistance(
                        AstronomyMath.DegToRad(AstronomyMath.HourToDeg(double.Parse(dataGridView.Rows[recordIndexFrom - 1].Cells[4].Value.ToString(), CultureInfo.InvariantCulture))),
                        AstronomyMath.DegToRad(double.Parse(dataGridView.Rows[recordIndexFrom - 1].Cells[5].Value.ToString(), CultureInfo.InvariantCulture)),
                        AstronomyMath.DegToRad(AstronomyMath.HourToDeg(double.Parse(dataGridView.Rows[recordIndexTo - 1].Cells[4].Value.ToString(), CultureInfo.InvariantCulture))),
                        AstronomyMath.DegToRad(double.Parse(dataGridView.Rows[recordIndexTo - 1].Cells[5].Value.ToString(), CultureInfo.InvariantCulture))
                    )
                );
        }

        public void PrintList()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ((PrintableDataGridView)(dataGridView)).Print(
                int.Parse(config.AppSettings.Settings["PrinterLeftRightMargin"].Value),
                int.Parse(config.AppSettings.Settings["PrinterTopBottomMargin"].Value));
        }

        public void PrintChart()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ((PrintablePictureBox)(pictureBox)).Print(
                int.Parse(config.AppSettings.Settings["PrinterLeftRightMargin"].Value),
                int.Parse(config.AppSettings.Settings["PrinterTopBottomMargin"].Value));
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !(MessageBox.Show("Closing PPM Processor.\r\nAre you sure you want to quit ?", "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }

        private void HideColumns(int[] columns)
        {
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                c.Visible = !columns.Contains(c.Index);
            }
        }

    }
}
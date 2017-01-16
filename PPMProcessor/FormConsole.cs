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
using System.Diagnostics;
using System.Configuration;

namespace PPMProcessor
{
    public partial class FormConsole : Form
    {

        private string[] Commands = {"help", "clear", "records", "name", "vmag", "spect", "ra", "dec", "fov", "ra-format", "dec-format", "common-name", 
                                     "list basic 1", "center", "angular-distance", "chart", "exit"};

        public int Records = 500;
        public String PPMName = "";
        public double VMag1 = -2;
        public double VMag2 = 15;
        public String Spect = "";
        public double RA = 0;
        public double Dec = 0;
        public double FoV = 0;
        public String RAFormat = "h";
        public String DecFormat = "d";
        public String CommonName = "";
        public String TypeOfList = "full";
        public String FileName = null;
        public int Page = 1;

        public FormConsole()
        {
            InitializeComponent();
        }

        private void FormConsole_Shown(object sender, EventArgs e)
        {
            textBoxConsole.Focus();
        }

        private void textBoxConsole_KeyDown(object sender, KeyEventArgs e)
        {

            string command = "";
            try { command = textBoxConsole.Lines[textBoxConsole.GetLineFromCharIndex(textBoxConsole.SelectionStart)]; } catch(Exception) {}

            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                try { textBoxConsole.Text += Commands.First(s => s.StartsWith(command)).Substring(command.Length); } catch (Exception) { }
                textBoxConsole.SelectionStart = textBoxConsole.Text.Length;
                textBoxConsole.ScrollToCaret();
            }
            else 
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {

                if (textBoxConsole.Text == "") return;

                string[] args = command.Split(new char[] { ' ' });

                switch (args[0])
                {

                    case "help":
                    {
                        textBoxConsole.AppendText("\r\n");
                        textBoxConsole.AppendText("> clear\r\n");
                        textBoxConsole.AppendText("> records [max number of records]\r\n");
                        textBoxConsole.AppendText("> name [PPM name]\r\n");
                        textBoxConsole.AppendText("> vmag [max] [min]\r\n");
                        textBoxConsole.AppendText("> spect [type]\r\n");
                        textBoxConsole.AppendText("> ra [RA decimal]\r\n");
                        textBoxConsole.AppendText("> ra [RA H] [RA M] [RA S.ss]\r\n");
                        textBoxConsole.AppendText("> dec [+|-][Dec decimal]\r\n");
                        textBoxConsole.AppendText("> dec [+|-][Dec D] [Dec M] [Dec S.ss]\r\n");
                        textBoxConsole.AppendText("> fov [Dec decimal]\r\n");
                        textBoxConsole.AppendText("> ra-format [hms|h|d|r]\r\n");
                        textBoxConsole.AppendText("> dec-format [dms|d|r]\r\n");
                        textBoxConsole.AppendText("> common-name [common name]\r\n");
                        textBoxConsole.AppendText("> list [all|basic] [page number|filename]\r\n");
                        textBoxConsole.AppendText("> center [record index]\r\n");
                        textBoxConsole.AppendText("> angular-distance [record index from] [record index to]\r\n");
                        textBoxConsole.AppendText("> chart\r\n");
                        textBoxConsole.AppendText("> exit\r\n");
                    }
                    break;

                    case "clear":
                    {
                        textBoxConsole.Clear();
                        textBoxConsole.SelectionStart = 0;
                        e.SuppressKeyPress = true;
                    }
                    break;

                    case "records":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> records " + this.Records.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                            if (args.Count() == 2)
                            {
                                try
                                {
                                    this.Records = int.Parse(args[1], CultureInfo.InvariantCulture);
                                }
                                catch (Exception)
                                {
                                    textBoxConsole.AppendText("\r\n");
                                    textBoxConsole.AppendText("> error");
                                }
                            }
                    }
                    break;

                    case "name":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> name " + this.PPMName);
                        }
                        else
                            if (args.Count() == 2)
                            {
                                this.PPMName = args[1];
                            }
                    }
                    break;

                    case "vmag":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> vmag " + VMag1.ToString(CultureInfo.InvariantCulture) + " " + VMag2.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        if (args.Count() == 3)
                        {
                            try
                            {
                                this.VMag1 = double.Parse(args[1], CultureInfo.InvariantCulture);
                                this.VMag2 = double.Parse(args[2], CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                    }
                    break;

                    case "spect":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> spect " + this.Spect);
                        }
                        else
                            if (args.Count() == 2)
                            {
                                this.Spect = args[1];
                            }
                    }
                    break;

                    case "ra":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> ra " + (this.RA).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        if (args.Count() == 2)
                        {
                            try
                            {
                                double ra_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                this.RA = ra_;
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                        else
                        if (args.Count() == 4)
                        {
                            try
                            {
                                double ra_ = double.Parse(args[1], CultureInfo.InvariantCulture) + double.Parse(args[2], CultureInfo.InvariantCulture) / 60 + double.Parse(args[3], CultureInfo.InvariantCulture) / 3600;
                                this.RA = ra_;
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                        else
                        if ((args.Count() == 3) || (args.Count() > 4))
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> error");
                        }
                    }
                    break;

                    case "fov":
                    case "dec":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> " + args[0] + " " + (args[0] == "dec" ? this.Dec : this.FoV).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        if (args.Count() == 2)
                        {
                            try
                            {
                                double deg_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                if (args[0] == "dec") this.Dec = deg_;
                                else this.FoV = deg_;
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                        else
                        if (args.Count() == 4)
                        {
                            try
                            {
                                double deg_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                deg_ += double.Parse(args[2], CultureInfo.InvariantCulture) / 60 * (args[1][0] == '-' ? -1 : 1);
                                deg_ += double.Parse(args[3], CultureInfo.InvariantCulture) / 3600 * (args[1][0] == '-' ? -1 : 1);
                                if (args[0] == "dec") this.Dec = deg_;
                                else this.FoV = deg_;
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                        else
                        if ((args.Count() == 3) || (args.Count() > 4))
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> error");
                        }
                    }
                    break;

                    case "ra-format":
                    {
                        if (args.Length == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> ra-format " + this.RAFormat);
                        }
                        else
                        {
                            this.RAFormat = args[1];
                        }
                    }
                    break;

                    case "dec-format":
                    {
                        if (args.Length == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> dec-format " + this.DecFormat);
                        }
                        else
                        {
                            this.DecFormat = args[1];
                        }
                    }
                    break;

                    case "common-name":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> common-name " + this.CommonName);
                        }
                        else
                            if (args.Count() >= 2)
                            {
                                this.CommonName = args[1];
                            }
                    }
                    break;

                    case "list":
                    {
                        textBoxConsole.AppendText("\r\n");
                        textBoxConsole.AppendText("> processing, please wait ... ");
                        if (args.Length >= 2) this.TypeOfList = args[1];
                        if (args.Length >= 3)
                        {
                            try
                            {
                                this.Page = int.Parse(args[2]);
                                this.FileName = null;
                            }
                            catch (Exception)
                            {
                                this.FileName = args[2];
                            }                            
                        }
                        ((Main)this.Owner).List();
                        textBoxConsole.AppendText("\r\n");
                        textBoxConsole.AppendText("> done");
                    }
                    break;

                    case "center":
                    {
                        ((Main)this.Owner).Center(int.Parse(args[1]), ref this.RA, ref this.Dec);
                    }
                    break;

                    case "angular-distance":
                    {
                        textBoxConsole.AppendText("\r\n");
                        textBoxConsole.AppendText("> angular-distance " + ((Main)this.Owner).AngularDistance(int.Parse(args[1]), int.Parse(args[2])).ToString(CultureInfo.InvariantCulture));
                    }
                    break;

                    case "chart":
                    {
                        if ((RAFormat != "h") || (DecFormat != "d"))
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> ra-format has to be 'h' and dec-format has to be 'd'");                            
                        }
                        else
                        if (FoV == 0)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> fov has to be greater than zero");
                        }
                        else
                        {
                            ((Main)this.Owner).Chart();
                        }
                    }
                    break;

                    case "exit":
                    {
                        this.Owner.Close();
                    }
                    break;
                    
                    default:
                    {
                        if (args[0] != "")
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> error");
                        }
                    }
                    break;

                }

            }
        }

        private void FormConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                this.Hide();
            }
        }

        private void FormConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }

}
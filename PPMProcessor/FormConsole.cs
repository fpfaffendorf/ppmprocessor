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

namespace PPMProcessor
{
    public partial class FormConsole : Form
    {

        public int Records = 100;
        public String PPMName = "";
        public double VMag1 = -2;
        public double VMag2 = 15;
        public String Spect = "";
        public double RA = 0;
        public double RAFrom = 0;
        public double RATo = 24;
        public double Dec = 0;
        public double DecFrom = -90;
        public double DecTo = 90;
        public double FoV = 0;
        public String RAFormat = "hms";
        public String DecFormat = "dms";
        public String CommonName = "";
        public String TypeOfList = "full";
        public String FileName = "";
        public int Page = 0;

        private string LastCommand = "";

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

            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                textBoxConsole.Text += this.LastCommand;
                textBoxConsole.SelectionStart = textBoxConsole.Text.Length;
                textBoxConsole.ScrollToCaret();
            }
            else 
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {

                if (textBoxConsole.Text == "") return;

                string command = textBoxConsole.Lines[textBoxConsole.GetLineFromCharIndex(textBoxConsole.SelectionStart)];
                string[] args = command.Split(new char[] { ' ' });

                this.LastCommand = command;

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
                        textBoxConsole.AppendText("> ra-from [RA decimal]\r\n");
                        textBoxConsole.AppendText("> ra-from [RA H] [RA M] [RA S.ss]\r\n");
                        textBoxConsole.AppendText("> dec-from [+|-][Dec decimal]\r\n");
                        textBoxConsole.AppendText("> dec-from [+|-][Dec D] [Dec M] [Dec S.ss]\r\n");
                        textBoxConsole.AppendText("> ra-to [RA decimal]\r\n");
                        textBoxConsole.AppendText("> ra-to [RA H] [RA M] [RA S.ss]\r\n");
                        textBoxConsole.AppendText("> dec-to [+|-][Dec decimal]\r\n");
                        textBoxConsole.AppendText("> dec-to [+|-][Dec D] [Dec M] [Dec S.ss]\r\n");
                        textBoxConsole.AppendText("> fov [Dec decimal]\r\n");
                        textBoxConsole.AppendText("> ra-format [hms|h|d|r]\r\n");
                        textBoxConsole.AppendText("> dec-format [dms|d|r]\r\n");
                        textBoxConsole.AppendText("> common-name [common name]\r\n");
                        textBoxConsole.AppendText("> list [all|basic] [page number|filename]\r\n");
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
                    case "ra-from":
                    case "ra-to":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> " + args[0] + " " + (args[0] == "ra" ? this.RA : args[0] == "ra-from" ? this.RAFrom : this.RATo).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        if (args.Count() == 2)
                        {
                            try
                            {
                                double ra_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                if (args[0] == "ra") this.RA = ra_;
                                else if (args[0] == "ra-from") this.RAFrom = ra_;
                                else this.RATo = ra_;
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
                                if (args[0] == "ra") this.RA = ra_;
                                else if (args[0] == "ra-from") this.RAFrom = ra_;
                                else this.RATo = ra_;
                            }
                            catch (Exception)
                            {
                                textBoxConsole.AppendText("\r\n");
                                textBoxConsole.AppendText("> error");
                            }
                        }
                    }
                    break;

                    case "dec":
                    case "dec-from":
                    case "dec-to":
                    {
                        if (args.Count() == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> " + args[0] + " " + (args[0] == "dec" ? this.Dec : args[0] == "dec-from" ? this.DecFrom : this.DecTo).ToString(CultureInfo.InvariantCulture));
                        }
                        else
                            if (args.Count() == 2)
                            {
                                try
                                {
                                    double dec_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                    if (args[0] == "dec") this.Dec = dec_;
                                    else if (args[0] == "dec-from") this.DecFrom = dec_;
                                    else this.DecTo = dec_;
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
                                        double dec_ = double.Parse(args[1], CultureInfo.InvariantCulture);
                                        dec_ += double.Parse(args[2], CultureInfo.InvariantCulture) / 60 * (args[1][0] == '-' ? -1 : 1);
                                        dec_ += double.Parse(args[3], CultureInfo.InvariantCulture) / 3600 * (args[1][0] == '-' ? -1 : 1);
                                        if (args[0] == "dec") this.Dec = dec_;
                                        else if (args[0] == "dec-from") this.DecFrom = dec_;
                                        else this.DecTo = dec_;
                                    }
                                    catch (Exception)
                                    {
                                        textBoxConsole.AppendText("\r\n");
                                        textBoxConsole.AppendText("> error");
                                    }
                                }
                    }
                    break;

                    case "fov":
                    {
                        if (args.Length == 1)
                        {
                            textBoxConsole.AppendText("\r\n");
                            textBoxConsole.AppendText("> fov " + this.FoV);
                        }
                        else
                        {
                            this.FoV = double.Parse(args[1], CultureInfo.InvariantCulture);
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
                            if (args.Count() == 2)
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

                    case "exit":
                    {
                        this.Close();
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
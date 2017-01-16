using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;

namespace PPMProcessor
{
    class PrintablePictureBox : PictureBox
    {

        public void Print(int marginLeftRight, int marginTopBotton)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.Name = "Print preview ...";
                ((Form)printPreviewDialog).WindowState = FormWindowState.Maximized;
                PrintDocument printDocument = new PrintDocument();
                Margins margins = new Margins(marginLeftRight, marginLeftRight, marginTopBotton, marginTopBotton);
                printDocument.PrinterSettings = printDialog.PrinterSettings;
                printDocument.DefaultPageSettings.Margins = margins;
                printDocument.OriginAtMargins = true;
                printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printPageEventHandler);
                printPreviewDialog.Document = printDocument;
                printPreviewDialog.UseAntiAlias = true;
                printPreviewDialog.ShowDialog();
            }
        }

        private void printPageEventHandler(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(this.Image, e.MarginBounds.Left, e.MarginBounds.Top);
        }

    }
}

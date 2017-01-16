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
    class PrintableDataGridView : DataGridView
    {

        const int RecordsPerPage = 40;
        private int CurrentPage = 1;
        private bool FirstPage = true;

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

            if (this.Rows.Count == 0) return;

            if (FirstPage)
            {
                CurrentPage = 1;
                FirstPage = false;
            }

            Font fontBold = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
            Font fontRegular = new Font(FontFamily.GenericMonospace, 8, FontStyle.Regular);

            e.Graphics.DrawString("id", fontBold, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top);

            int offsetX = 0;
            int offsetY = this.Rows[0].Height;

            int id = 0; 
            for (id = ((CurrentPage - 1) * RecordsPerPage); (id < CurrentPage * RecordsPerPage) && (id < this.Rows.Count); id++)
            {
                e.Graphics.DrawString((id + 1).ToString(), fontRegular, Brushes.Black, e.MarginBounds.Left + offsetX, e.MarginBounds.Top + offsetY);
                offsetY += this.Rows[id].Height;
            }

            if (id < this.Rows.Count) e.HasMorePages = true;
            else
            {
                FirstPage = true;
            }

            offsetX = this.RowHeadersWidth;
            offsetY = this.Rows[0].Height;

            foreach(DataGridViewColumn dataGridViewColumn in this.Columns)
            {

                if (dataGridViewColumn.Visible)
                {

                    e.Graphics.DrawString(dataGridViewColumn.Name, fontBold, Brushes.Black, e.MarginBounds.Left + offsetX, e.MarginBounds.Top);

                    for (int index = ((CurrentPage - 1) * RecordsPerPage); (index < CurrentPage * RecordsPerPage) && (index < this.Rows.Count); index++)
                    {
                        string value = "";
                        if (this.Rows[index].Cells[dataGridViewColumn.Index].Value != null)
                            value = this.Rows[index].Cells[dataGridViewColumn.Index].Value.ToString();
                        e.Graphics.DrawString(value, fontRegular, Brushes.Black, e.MarginBounds.Left + offsetX, e.MarginBounds.Top + offsetY);
                        offsetY += this.Rows[index].Height;
                    }

                    offsetX += dataGridViewColumn.Width;

                }

                offsetY = this.Rows[0].Height;

            }

            CurrentPage++;            

        }

    }
}

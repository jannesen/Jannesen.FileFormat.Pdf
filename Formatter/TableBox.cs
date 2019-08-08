using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TableBox: Box
    {
        private readonly        TableColumns        _columns;
        private readonly        TableRows           _rows;
        private                 PdfDistance         _calcWidth;
        private                 PdfDistance         _calcHeight;

        public      override    PdfDistance         Width
        {
            get {
                return _calcWidth;
            }
            set {
                throw new NotImplementedException();
            }
        }
        public      override    PdfDistance         Height
        {
            get {
                return _calcHeight;
            }
        }

        public                  TableColumns        Columns
        {
            get {
                return _columns;
            }
        }
        public                  TableRows           Rows
        {
            get {
                return _rows;
            }
        }

        public                                      TableBox(PdfDistance[] columWidth)
        {
            _columns     = new TableColumns(this, columWidth);
            _rows        = new TableRows(this);
        }

        public                  TableRow            AddRow()
        {
            return _rows.AddRow();
        }
        public                  TableRow            AddRow(int[] colSpans)
        {
            return _rows.AddRow(colSpans);
        }
        public                  void                AddRows(int count)
        {
            _rows.AddRows(count);
        }
        public                  TableCell           Cell(int column, int row)
        {
            return Rows[row].Cells[column];
        }
        public   override       void                Format()
        {
            PdfDistance     top = PdfDistance.Zero;

            _calcWidth  = PdfDistance.Zero;
            _calcHeight = PdfDistance.Zero;

            for (int rowIndex = 0 ; rowIndex < _rows.Count ; ++rowIndex) {
                TableRow        Row    = _rows[rowIndex];
                PdfDistance     Height = PdfDistance.Zero;
                PdfDistance     Width  = PdfDistance.Zero;

                for (int colIndex = 0 ; colIndex < Row.Cells.Count ; ) {
                    TableCell   cell = Row.Cells[colIndex];

                    cell.boxMoveTo(top, cell.Column.Left);

                    if (cell.ColSpan == 1) {
                        cell.Width = cell.Column.Width;
                    }
                    else {
                        PdfDistance CellWidth = PdfDistance.Zero;

                        for (int i = 0 ; i < cell.ColSpan ; ++i)
                            CellWidth += _columns[colIndex + i].Width;

                        cell.Width = CellWidth;
                    }

                    cell.Format();

                    if (Height > cell.Height)
                        Height = cell.Height;

                    if (Width < cell.Left + cell.Width)
                        Width = cell.Left + cell.Width;

                    colIndex += cell.ColSpan;
                }

                Row.boxSetTopHeight(top, Height);

                top += Height;

                if (_calcWidth < Width)
                    _calcWidth = Width;

                if (_calcHeight > top)
                    _calcHeight = top;
            }
        }

        internal override       void                boxPrintBackground(PdfPoint upperLeftCorner, PrintBackground background)
        {
            upperLeftCorner += new PdfPoint(Left, Top);

            for (int rowIndex = 0 ; rowIndex < _rows.Count ; ++rowIndex) {
                TableRow        Row    = _rows[rowIndex];

                for (int ColIndex = 0 ; ColIndex < Row.Cells.Count ; ) {
                    Row.Cells[ColIndex].boxPrintBackground(upperLeftCorner, background);
                    ColIndex += Row.Cells[ColIndex].ColSpan;
                }
            }
        }
        internal override       void                boxPrintForground(PdfPoint upperLeftCorner, PdfContent content)
        {
            upperLeftCorner += new PdfPoint(Left, Top);

            for (int rowIndex = 0 ; rowIndex < _rows.Count ; ++rowIndex) {
                TableRow        Row    = _rows[rowIndex];

                for (int ColIndex = 0 ; ColIndex < Row.Cells.Count ; ) {
                    Row.Cells[ColIndex].boxPrintForground(upperLeftCorner, content);
                    ColIndex += Row.Cells[ColIndex].ColSpan;
                }
            }
        }
    }
}

/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TableCell: ContainerBox
    {
        private                 TableColumn         _column;
        private                 TableRow            _row;
        private                 int                 _colSpan;

        public                  TableColumn         Column
        {
            get {
                return _column;
            }
        }
        public                  TableRow            Row
        {
            get {
                return _row;
            }
        }
        public                  int                 ColSpan
        {
            get {
                return _colSpan;
            }
        }

        public                                      TableCell(TableRow row, TableColumn column, int colSpan)
        {
            _column     = column;
            _row        = row;
            _colSpan    = colSpan;

            if (column.hasMargin) {
                Margin.Top    += column.Margin.Top;
                Margin.Left   += column.Margin.Left;
                Margin.Right  += column.Margin.Right;
                Margin.Bottom += column.Margin.Bottom;
            }

            if (row.hasMargin) {
                Margin.Top    += row.Margin.Top;
                Margin.Left   += row.Margin.Left;
                Margin.Right  += row.Margin.Right;
                Margin.Bottom += row.Margin.Bottom;
            }

            if (column.hasBorder) {
                if (column.Border.Top != null)      Border.Top    = column.Border.Top;
                if (column.Border.Left != null)     Border.Left   = column.Border.Left;
                if (column.Border.Right != null)    Border.Right  = column.Border.Right;
                if (column.Border.Bottom != null)   Border.Bottom = column.Border.Bottom;
            }

            if (row.hasBorder) {
                if (row.Border.Top != null)     Border.Top    = row.Border.Top;
                if (row.Border.Left != null)    Border.Left   = row.Border.Left;
                if (row.Border.Right != null)   Border.Right  = row.Border.Right;
                if (row.Border.Bottom != null)  Border.Bottom = row.Border.Bottom;
            }

            if (column.hasPadding) {
                Padding.Top    += column.Padding.Top;
                Padding.Left   += column.Padding.Left;
                Padding.Right  += column.Padding.Right;
                Padding.Bottom += column.Padding.Bottom;
            }

            if (row.hasPadding) {
                Padding.Top    += row.Padding.Top;
                Padding.Left   += row.Padding.Left;
                Padding.Right  += row.Padding.Right;
                Padding.Bottom += row.Padding.Bottom;
            }

            boxLinkParent(row.Table);
        }

        public                  TextBox             AddText(string text)
        {
            PdfStyleText    style;
            PdfTextAlign    align;
            int             maxLines;

            if (Row.TextStyle != null)
                style = Row.TextStyle;
            else
            if (Column.TextStyle != null)
                style = Column.TextStyle;
            else
                throw new PdfException("Default text style not defined.");

            if (Row.TextAlign != PdfTextAlign.Unknown)
                align = Row.TextAlign;
            else
            if (Column.TextAlign != PdfTextAlign.Unknown)
                align = Column.TextAlign;
            else
                align = PdfTextAlign.Left;

            if (Row.TextMaxLines != 0)
                maxLines = Row.TextMaxLines;
            else
            if (Column.TextMaxLines != 0)
                maxLines = Column.TextMaxLines;
            else
                maxLines = 0;

            TextBox Box = new TextBox(style, align,  maxLines, text);

            Append(Box);

            return Box;
        }
    }

    public class TableCells: List<TableCell>
    {
        private                 TableRow            _row;
        private                 int[]               _colSpans;

        public      new         TableCell           this[int column]
        {
            get {
                if (column < 0 || column >=Count)
                    throw new ArgumentOutOfRangeException("Column out of range.");

                int ColSpan = (_colSpans != null) ? _colSpans[column] : 1;

                if (ColSpan == 0)
                    throw new ArgumentException("Invalid Column.");

                TableCell   Cell = base[column];


                if (Cell == null) {
                    Cell = new TableCell(_row, _row.Table.Columns[column], ColSpan);

                    for (int i = 0 ; i < ColSpan ; ++i)
                        base[column + i] = Cell;
                }

                return Cell;
            }
        }

        public                                      TableCells(TableRow row, int[] colSpans)
        {
            if (colSpans != null && colSpans.Length != row.Table.Columns.Count)
                throw new ArgumentException("Invalid ColSpan array.");

            _row    = row;

            for (int i = _row.Table.Columns.Count ; i > 0 ; --i)
                base.Add(null);

            _colSpans = colSpans;
        }

        public      new         void                Add(TableCell child)
        {
            throw new NotImplementedException("Not implemented TableCells.Add");
        }
        public      new         void                AddRange(IEnumerable<TableCell> list)
        {
            throw new NotImplementedException("Not implemented TableCells.AddRange");
        }
        public      new         void                Clear()
        {
            throw new NotImplementedException("Not implemented TableCells.Clear");
        }
        public      new         void                Insert(int index, TableCell child)
        {
            throw new NotImplementedException("Not implemented TableCells.Insert");
        }
        public      new         void                InsertRange(int index, IEnumerable<TableCell> list)
        {
            throw new NotImplementedException("Not implemented TableCells.InsertRange");
        }
        public      new         bool                Remove(TableCell child)
        {
            throw new NotImplementedException("Not implemented TableCells.Remove");
        }
        public      new         void                RemoveAll(Predicate<TableCell> match)
        {
            throw new NotImplementedException("Not implemented TableCells.RemoveAll");
        }
        public      new         void                RemoveAt(int index)
        {
            throw new NotImplementedException("Not implemented TableCells.RemoveAt");
        }
        public      new         void                RemoveRange(int index, int count)
        {
            throw new NotImplementedException("Not implemented TableCells.RemoveRange");
        }
    }
}

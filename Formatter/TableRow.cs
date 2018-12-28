/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TableRow: TableColumnRow
    {
        private                 TableBox            _table;
        private                 TableCells          _cells;
        private                 PdfDistance         _top;
        private                 PdfDistance         _height;

        public                  int                 Index
        {
            get {
                return _table.Rows.IndexOf(this);
            }
        }
        public                  TableBox            Table
        {
            get {
                return _table;
            }
        }
        public                  TableCells          Cells
        {
            get {
                return _cells;
            }
        }
        public                  PdfDistance         Top
        {
            get {
                return _top;
            }
        }
        public                  PdfDistance         Height
        {
            get {
                return _height;
            }
        }

        public                                      TableRow(TableBox table, int[] colSpans)
        {
            _table    = table;
            _cells    = new TableCells(this, colSpans);
        }

        public                  void                boxSetTopHeight(PdfDistance top, PdfDistance height)
        {
            _top    = top;
            _height = height;
        }
    }

    public class TableRows: List<TableRow>
    {
        private                 TableBox            _table;

        public                                      TableRows(TableBox table)
        {
            _table    = table;
        }

        public                  TableRow            AddRow()
        {
            return AddRow(null);
        }
        public                  TableRow            AddRow(int[] colSpans)
        {
            TableRow    Row = new TableRow(_table, colSpans);

            base.Add(Row);

            return Row;
        }
        public                  void                AddRows(int count)
        {
            for (int i = 0 ; i < count ; ++i)
                AddRow(null);
        }

        public      new         void                Add(TableRow child)
        {
            throw new NotImplementedException("Not implemented TableRows.Add");
        }
        public      new         void                AddRange(IEnumerable<TableRow> list)
        {
            throw new NotImplementedException("Not implemented TableRows.AddRange");
        }
        public      new         void                Clear()
        {
            throw new NotImplementedException("Not implemented TableRows.Clear");
        }
        public      new         void                Insert(int index, TableRow child)
        {
            throw new NotImplementedException("Not implemented TableRows.Insert");
        }
        public      new         void                InsertRange(int index, IEnumerable<TableRow> list)
        {
            throw new NotImplementedException("Not implemented TableRows.InsertRange");
        }
        public      new         bool                Remove(TableRow child)
        {
            throw new NotImplementedException("Not implemented TableRows.Remove");
        }
        public      new         void                RemoveAll(Predicate<TableRow> match)
        {
            throw new NotImplementedException("Not implemented TableRows.RemoveAll");
        }
        public      new         void                RemoveAt(int index)
        {
            throw new NotImplementedException("Not implemented TableRows.RemoveAt");
        }
        public      new         void                RemoveRange(int index, int count)
        {
            throw new NotImplementedException("Not implemented TableRows.RemoveRange");
        }
    }
}

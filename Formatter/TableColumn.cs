using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TableColumn: TableColumnRow
    {
        private readonly        TableBox            _table;
        private                 PdfDistance         _setLeft;
        private                 PdfDistance         _setWidth;

        public                  int                 Index
        {
            get {
                return _table.Columns.IndexOf(this);
            }
        }
        public                  TableBox            Table
        {
            get {
                return _table;
            }
        }
        public                  PdfDistance         Left
        {
            get {
                return _setLeft;
            }
        }
        public                  PdfDistance         Width
        {
            get {
                return _setWidth;
            }
        }

        public                                      TableColumn(TableBox table, PdfDistance left, PdfDistance width)
        {
            _table    = table;
            _setLeft  = left;
            _setWidth = width;
        }
    }

    public class TableColumns: List<TableColumn>
    {
        private readonly        TableBox            _table;

        public                                      TableColumns(TableBox table, PdfDistance[] columWidth)
        {
            ArgumentNullException.ThrowIfNull(table);
            ArgumentNullException.ThrowIfNull(columWidth);

            _table    = table;

            PdfDistance left = PdfDistance.Zero;

            for(int col = 0 ; col < columWidth.Length ; ++col) {
                base.Add(new TableColumn(_table, left, columWidth[col]));
                left += columWidth[col];
            }
        }

        public      new         void                Add(TableColumn child)
        {
            throw new NotImplementedException("Not implemented TableColumns.Add");
        }
        public      new         void                AddRange(IEnumerable<TableColumn> list)
        {
            throw new NotImplementedException("Not implemented TableColumns.AddRange");
        }
        public      new         void                Clear()
        {
            throw new NotImplementedException("Not implemented TableColumns.Clear");
        }
        public      new         void                Insert(int index, TableColumn child)
        {
            throw new NotImplementedException("Not implemented TableColumns.Insert");
        }
        public      new         void                InsertRange(int index, IEnumerable<TableColumn> list)
        {
            throw new NotImplementedException("Not implemented TableColumns.InsertRange");
        }
        public      new         bool                Remove(TableColumn child)
        {
            throw new NotImplementedException("Not implemented TableColumns.Remove");
        }
        public      new         void                RemoveAll(Predicate<TableColumn> match)
        {
            throw new NotImplementedException("Not implemented TableColumns.RemoveAll");
        }
        public      new         void                RemoveAt(int index)
        {
            throw new NotImplementedException("Not implemented TableColumns.RemoveAt");
        }
        public      new         void                RemoveRange(int index, int count)
        {
            throw new NotImplementedException("Not implemented TableColumns.RemoveRange");
        }
    }
}

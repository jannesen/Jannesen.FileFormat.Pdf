using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class PrintBackground
    {
        private sealed class Line
        {
            public  readonly    PdfStyleLine    LineStyle;
            public              PdfPoint        Begin;
            public              PdfPoint        End;

            public                              Line(PdfStyleLine lineStyle, PdfPoint begin, PdfPoint end)
            {
                this.LineStyle = lineStyle;
                this.Begin     = begin;
                this.End       = end;
            }
        }

        private sealed class Lines : List<Line>
        {
            public              void            Add(PdfStyleLine lineStyle, PdfPoint begin, PdfPoint end)
            {
                Line    Append = null;

                for (int i = 0 ; i < base.Count ; ++i) {
                    Line    Line = base[i];

                    if (Line.LineStyle == lineStyle) {
                        double  Error = Math.Abs(begin.x.pnts - Line.End.x.pnts) + Math.Abs(begin.y.pnts - Line.End.y.pnts);

                        if (Error < 0.001)
                            Append = base[i];
                    }
                }

                if (Append != null)
                    Append.End = end;
                else
                    base.Add(new Line(lineStyle, begin, end));
            }
            public              void            Print(PdfContent content)
            {
                foreach(Line Line in this)
                    content.DrawLine(Line.LineStyle, Line.Begin, Line.Begin - Line.End);
            }
        }

        private                 Lines                           _horizontalLines;
        private                 Lines                           _verticalLines;
        private                 Lines                           _diagonalLines;

        public                                                  PrintBackground()
        {
            _horizontalLines = null;
            _verticalLines   = null;
            _diagonalLines   = null;
        }

        public                  void                            DrawLineHorizontal(PdfStyleLine lineStyle, PdfPoint begin, PdfDistance length)
        {
            ArgumentNullException.ThrowIfNull(lineStyle);

            if (_horizontalLines == null)
                _horizontalLines = new Lines();

            _horizontalLines.Add(lineStyle, begin, new PdfPoint(begin.x + length, begin.y));
        }
        public                  void                            DrawLineVertical(PdfStyleLine lineStyle, PdfPoint begin, PdfDistance length)
        {
            ArgumentNullException.ThrowIfNull(lineStyle);

            if (_verticalLines == null)
                _verticalLines   = new Lines();

            _verticalLines.Add(lineStyle, begin, new PdfPoint(begin.x, begin.y + length));
        }
        public                  void                            DrawLineDiagonal(PdfStyleLine lineStyle, PdfPoint begin, PdfSize size)
        {
            ArgumentNullException.ThrowIfNull(lineStyle);

            if (_diagonalLines == null)
                _diagonalLines   = new Lines();

            _diagonalLines.Add(new Line(lineStyle, begin, begin + size));
        }

        public                  void                            Print(PdfContent content)
        {
            ArgumentNullException.ThrowIfNull(content);

            _horizontalLines?.Print(content);
            _verticalLines?.Print(content);
            _diagonalLines?.Print(content);
        }
    }
}

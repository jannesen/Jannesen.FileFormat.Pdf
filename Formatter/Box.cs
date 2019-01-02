/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public abstract class Box
    {
        private                 Box                 _parent;
        private                 bool                _fixed;
        private                 PdfDistance         _top;
        private                 PdfDistance         _left;

        public                  Box                 Parent
        {
            get {
                return _parent;
            }
        }
        public                  bool                Fixed
        {
            get {
                return _fixed;
            }
            set {
                if (_fixed != value) {
                    _fixed = value;
                }
            }
        }
        public                  PdfDistance         Top
        {
            get {
                return _top;
            }
            set {
                if (!_fixed)
                    throw new PdfException("Not allowed to to set top of relative box.");

                _top = value;
            }
        }
        public                  PdfDistance         Left
        {
            get {
                return _left;
            }
            set {
                if (!_fixed)
                    throw new PdfException("Not allowed to to set left of relative box.");

                _left = value;
            }
        }
        public      abstract    PdfDistance         Width   { get; set; }
        public      abstract    PdfDistance         Height  { get; }

        public                                      Box()
        {
            _parent = null;
            _top    = PdfDistance.Zero;
            _left   = PdfDistance.Zero;
        }

        public      virtual     void                Format()
        {
        }
        public                  void                Print(PdfPoint upperLeftCorner, PdfContent content)
        {
            Format();

            {
                PrintBackground background = new PrintBackground();

                boxPrintBackground(upperLeftCorner, background);

                background.Print(content);
            }

            boxPrintForground(upperLeftCorner, content);
        }

        public      virtual     void                boxPrintBackground(PdfPoint upperLeftCorner, PrintBackground background)
        {
        }
        public      virtual     void                boxPrintForground(PdfPoint upperLeftCorner, PdfContent content)
        {
        }
        public                  void                boxMoveTo(PdfDistance top, PdfDistance left)
        {
            _top  = top;
            _left = left;
        }

        public                  void                boxLinkParent(Box parent)
        {
            if (_parent != null)
                throw new PdfException("Box already linked to parent.");

            _parent = parent;
        }
        public                  void                boxUnlinkParent(Box parent)
        {
            if (_parent != parent)
                throw new PdfException("Internal error invalid UnlinkParent");

            _parent = null;
        }
    }

    public class BoxList: List<Box>
    {
        private                 Box                 _parent;

        public                  Box                 Parent
        {
            get {
                return _parent;
            }
        }

        public                                      BoxList(Box parent)
        {
            _parent = parent;
        }

        public      new         void                Add(Box child)
        {
            child.boxLinkParent(_parent);
            base.Add(child);
        }
        public      new         void                AddRange(IEnumerable<Box> list)
        {
            foreach(Box child in list)
                Add(child);
        }
        public      new         void                Clear()
        {
            RemoveRange(0, base.Count);
        }
        public      new         void                Insert(int index, Box child)
        {
            child.boxLinkParent(_parent);
            base.Insert(index, child);
        }
        public      new         void                InsertRange(int index, IEnumerable<Box> list)
        {
            foreach(Box child in list)
                Insert(index++, child);
        }
        public      new         bool                Remove(Box child)
        {
            if (!Remove(child))
                return false;

            child.boxUnlinkParent(_parent);
            return true;
        }
        public      new         void                RemoveAll(Predicate<Box> match)
        {
            throw new NotImplementedException("Not implemented Box.List RemoveAll");
        }
        public      new         void                RemoveAt(int index)
        {
            base[index].boxUnlinkParent(_parent);
            RemoveAt(index);
        }
        public      new         void                RemoveRange(int index, int count)
        {
            while (count > 0) {
                RemoveAt(index);
                --count;
            }
        }
    }
}

/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf.Internal
{
    public class PdfResourceEntry
    {
        public  readonly    string              Class;
        public  readonly    string              Name;
        public  readonly    PdfValue            Resource;

        public                                  PdfResourceEntry(string cls, string name, PdfValue resource)
        {
            this.Class    = cls;
            this.Name     = name;
            this.Resource = resource;
        }
    }

    public class PdfResourceEntryList: List<PdfResourceEntry>
    {
        public              void                InitFrom(PdfResourceEntryList list)
        {
            base.Clear();

            for (int i = 0 ; i < list.Count ; ++i)
                base.Add(list[i]);
        }
        public              PdfResourceEntry    FindByName(string name)
        {
            for (int i = 0 ; i < Count ; ++i) {
                if (base[i].Name == name)
                    return base[i];
            }

            return null;
        }
        public              PdfResourceEntry    FindByObject(PdfObject resource)
        {
            for (int i = 0 ; i < Count ; ++i) {
                if (base[i].Resource == resource)
                    return base[i];
            }

            return null;
        }
        public              string              GetName(PdfObject resource)
        {
            PdfResourceEntry    Entry = FindByObject(resource);
            if (Entry != null)
                return Entry.Name;

            string  cls     = resource.NamedType;
            string  name    = null;
            string  prefix;

            switch(cls) {
            case PdfObject.ntFont:      prefix = "F";   break;
            case PdfObject.ntXObject:   prefix = "I";   break;
            default:                    prefix = "X";   break;
            }

            for (int i = 0 ; i < Count ; ++i) {
                if (base[i].Name.StartsWith(prefix))
                    name = base[i].Name;
            }

            if (name != null) {
                if (!int.TryParse(name.Substring(prefix.Length), out var n))
                    n = 1;

                do {
                    name = prefix + (++n).ToString();
                }
                while (FindByName(name) != null);
            }
            else
                name = prefix+"1";

            Add(cls, name, resource);

            return name;
        }
        public              void                Add(string cls, string name, PdfValue resource)
        {
            Add(new PdfResourceEntry(cls, name, resource));
        }
        public  new         void                Add(PdfResourceEntry entry)
        {
            if (FindByName(entry.Name) != null)
                throw new PdfExceptionWriter("Resource name already in use.");

            base.Add(entry);
        }
        public              void                pdfWriteResources(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            bool[]      entryWriten = new bool[Count];

            for (;;) {
                string      CurrentClass = null;

                for (int i = 0 ; i < base.Count ; ++i) {
                    if (!entryWriten[i]) {
                        PdfResourceEntry    Entry = base[i];

                        if (CurrentClass == null) {
                            CurrentClass = Entry.Class;
                            writer.WriteName(CurrentClass);
                            writer.WriteDictionaryBegin();
                        }

                        if (Entry.Class == CurrentClass) {
                            writer.WriteName(Entry.Name);

                            if (Entry.Resource is PdfObject)
                                writer.WriteReference(document, (PdfObject)Entry.Resource);
                            else
                                Entry.Resource.pdfWriteToDocument(document, writer);

                            entryWriten[i] = true;
                        }
                    }
                }

                if (CurrentClass == null)
                    break;

                writer.WriteDictionaryEnd();
            }
        }
    }
}

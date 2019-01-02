/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfDocumentInfo : PdfObject
    {
        private             string              _title;
        private             string              _author;
        private             string              _subject;
        private             string              _keywords;
        private             string              _creator;
        private             string              _producer;
        private             DateTime            _creationDate;
        private             DateTime            _modDate;

        public              string              Title               { get { return _title;          }   set { _title = value;           }   }
        public              string              Author              { get { return _author;         }   set { _author = value;          }   }
        public              string              Subject             { get { return _subject;        }   set { _subject = value;         }   }
        public              string              Keywords            { get { return _keywords;       }   set { _keywords = value;        }   }
        public              string              Creator             { get { return _creator;        }   set { _creator = value;         }   }
        public              string              Producer            { get { return _producer;       }   set { _producer = value;        }   }
        public              DateTime            CreationDate        { get { return _creationDate;   }   set { _creationDate = value;    }   }
        public              DateTime            ModDate             { get { return _modDate;        }   set { _modDate = value;         }   }

        public                                  PdfDocumentInfo()
        {
            _producer     = "Jannesen.PdfWriter";
            _creationDate = DateTime.UtcNow;
            _modDate      = DateTime.UtcNow;
        }

        public override     void                pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteDictionaryBegin();

            if (_title != null)     {   writer.WriteName("Title");          writer.WriteString(_title);         }
            if (_author != null)    {   writer.WriteName("Author");         writer.WriteString(_author);        }
            if (_subject != null)   {   writer.WriteName("Subject");        writer.WriteString(_subject);       }
            if (_keywords != null)  {   writer.WriteName("Keywords");       writer.WriteString(_keywords);      }
            if (_creator != null)   {   writer.WriteName("Creator");        writer.WriteString(_creator);       }
            if (_producer != null)  {   writer.WriteName("Producer");       writer.WriteString(_producer);      }
                                    {   writer.WriteName("CreationDate");   writer.WriteDate(_creationDate);    }
                                    {   writer.WriteName("ModDate");        writer.WriteDate(_modDate);         }

            writer.WriteDictionaryEnd();
        }
    }
}

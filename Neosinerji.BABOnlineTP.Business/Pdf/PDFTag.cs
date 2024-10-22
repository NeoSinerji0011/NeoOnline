using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Pdf
{
    public class PDFTag
    {
        public string Tag { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int CurrentIndex { get; set; }
    }
}

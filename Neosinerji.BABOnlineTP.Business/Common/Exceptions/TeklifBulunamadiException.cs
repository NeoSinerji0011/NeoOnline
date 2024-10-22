using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TeklifBulunamadiException : Exception
    {
        public TeklifBulunamadiException()
            :base("Teklif bulunamadi.")
        {

        }
    }
}

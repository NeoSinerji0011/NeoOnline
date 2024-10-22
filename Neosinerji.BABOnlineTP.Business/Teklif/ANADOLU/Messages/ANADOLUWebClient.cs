using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUWebClient : WebClient
    {
        public int Timeout { get; set; }

        public ANADOLUWebClient()
            :base()
        {
            this.Timeout = 30000;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = this.Timeout;
            return w;
        }
    }
}

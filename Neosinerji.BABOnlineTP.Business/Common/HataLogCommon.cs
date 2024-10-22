using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class YetkiHataServisModel
    {
        public string logType { get; set; }
        public string clientIp { get; set; }
        public DateTime logDate { get; set; }
        public string hostName { get; set; }
        public string kullanici { get; set; }
        public string message { get; set; }
        public string url { get; set; }
    }


}

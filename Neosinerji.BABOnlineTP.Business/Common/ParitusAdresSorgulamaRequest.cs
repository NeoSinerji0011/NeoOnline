using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{

    [Serializable]
    public class ParitusAdresSorgulamaRequest
    {
        public string apikey { get; set; }
        public string id { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string quarter { get; set; }
        public string quarter2 { get; set; }
        public string street { get; set; }
        public string street2 { get; set; }
        public string mainstreet { get; set; }
        public string mainstreet2 { get; set; }
        public string housenumber { get; set; }
        public string unit { get; set; }
        public string building { get; set; }
        public string floor { get; set; }
        public string zipcode { get; set; }
        public string district { get; set; }
        public string town { get; set; }
        public string city { get; set; }
        public string towncode { get; set; }
        public string citycode { get; set; }
        public string firstlast { get; set; }
        public string company { get; set; }
        public string usedtcifnotexist { get; set; }
        public string capitalizeOutput { get; set; }
    }
}

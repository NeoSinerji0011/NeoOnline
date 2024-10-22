using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.Messages
{
    [XmlRootAttribute("DASKOuput", Namespace = "", IsNullable = false)]
   public  class TURKNIPPON_DASK_Response
    {
        [XmlElement]
        public string IsSuccess { get; set; }

        [XmlElement]
        public string StatusCode { get; set; }

        [XmlElement]
        public string StatusDescription { get; set; }

        [XmlElement]
        public string TrackingCode { get; set; }

        [XmlElement]
        public string UnitNo { get; set; }

        [XmlElement]
        public string UnitName { get; set; }

        [XmlElement]
        public string Premium { get; set; }

        [XmlElement]
        public string BeginDate { get; set; }

        [XmlElement]
        public string EndDate { get; set; }

        [XmlElement]
        public string PolicyNo { get; set; }

        [XmlElement]
        public string ClientNo { get; set; }


        [XmlElement]
        public string DASKInsuranceAmount { get; set; }

        [XmlElement]
        public string DASKWarning { get; set; }


        [XmlElement]
        public string ExternalPolicyNo { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("POLICY", Namespace = "", IsNullable = false)]
    public class RAY_TrafikTeklifKayitHata_Response
    {
        [XmlElement]
        public ERROR Error { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }
    }
    public class ERROR
    {
        [XmlElement]
        public string ErrDesc { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class trafikHasar
    {
        [XmlElement("TrafikHasarBilgi")] 
        public TrafikHasarBilgi[] hasarBilgi { get; set; }
    }
}

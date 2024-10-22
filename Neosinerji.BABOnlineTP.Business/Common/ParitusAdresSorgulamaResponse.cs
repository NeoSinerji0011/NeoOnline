using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.Paritus
{
    [Serializable, XmlRoot("AddressParseResult", IsNullable = true)]
    public class ParitusAdresSorgulamaResponse  //AddressParseResult
    {
        public string normalizedAddress { get; set; }

        public parsedAddress parsedAddress { get; set; }

        [XmlElement("streetHits")]
        public List<streetHits> streetHits { get; set; }

        [XmlElement("quarterHits")]
        public quarterHits quarterHits { get; set; }
        public metaData metaData { get; set; }

        public string streetVerificationScore { get; set; }
        public string poiVerificationScore { get; set; }
        public string verificationScore { get; set; }
        public string verificationType { get; set; }
        public string locationType { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

    }


    [Serializable]
    public class parsedAddress
    {
        public string id { get; set; }
        public string original { get; set; }

        [XmlElement("tokens")]
        public List<tokens> tokens { get; set; }

        public string quarter { get; set; }
        public string houseNumber { get; set; }
        public string floor { get; set; }
        public string zipCode { get; set; }
        public string undefined { get; set; }
        public string town { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string townCode { get; set; }
        public string cityCode { get; set; }
        public string townLatitude { get; set; }
        public string townLongitude { get; set; }
        public string cityLatitude { get; set; }
        public string cityLongitude { get; set; }
        public string uavtStreetCode { get; set; }
        public string uavtBuildingCode { get; set; }
        public string uavtAddressCode { get; set; }
    }


    [Serializable]
    public class streetHits
    {
        public string id { get; set; }
        public string original { get; set; }
        public string quarter { get; set; }
        public string zipCode { get; set; }
        public string undefined { get; set; }
        public string town { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string townCode { get; set; }
        public string cityCode { get; set; }
        public string townLatitude { get; set; }
        public string townLongitude { get; set; }
        public string cityLatitude { get; set; }
        public string cityLongitude { get; set; }

        public matchScore matchScore { get; set; }
    }


    [Serializable]
    public class quarterHits
    {
        public string id { get; set; }
        public string original { get; set; }
        public string quarter { get; set; }
        public string undefined { get; set; }
        public string town { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string townCode { get; set; }
        public string cityCode { get; set; }
        public string townLatitude { get; set; }
        public string townLongitude { get; set; }
        public string cityLatitude { get; set; }
        public string cityLongitude { get; set; }

        public matchScore matchScore { get; set; }
    }

    [Serializable]
    public class metaData
    {
        public string executionTime { get; set; }
        public string dataVersion { get; set; }
        public string libraryVersion { get; set; }
        public string neighborQuarter { get; set; }
        public string neighborTown { get; set; }
        public string townIgnored { get; set; }

        public bestMatchScore bestMatchScore { get; set; }
    }


    [Serializable]
    public class tokens
    {
        public string tokenType { get; set; }
        public string text { get; set; }
        public string alternative { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
    }


    [Serializable]
    public class matchScore
    {
        public string score { get; set; }
        public string fuzzyUsed { get; set; }
        public string partsUsed { get; set; }
        public string initialsUsed { get; set; }
        public string prefixUsed { get; set; }
    }


    [Serializable]
    public class bestMatchScore
    {
        public string score { get; set; }
        public string fuzzyUsed { get; set; }
        public string partsUsed { get; set; }
        public string initialsUsed { get; set; }
        public string prefixUsed { get; set; }
    }
}

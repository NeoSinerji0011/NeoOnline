using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    public partial class HDIEskiPoliceBilgileri
    {
        public string EskiPoliceSirket { get; set; }
        public string EskiPoliceAcente { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.ALLIANZ
{
    public class ALLIANZ_listProcessDescriptors
    {
        public const string AZS_WS_KSK_PROPOSAL = "AZS_WS_KSK_PROPOSAL";
        public const string AZS_WS_CSL_PROPOSAL = "AZS_WS_CSL_PROPOSAL";
        public const string AZS_WS_TRAFIK_PROPOSAL = "AZS_WS_TRAFIK_PROPOSAL";
        public const string AZS_PRINT_PROP_POL = "AZS_PRINT_PROP_POL";
        public const string AZS_WS_SEYAHAT_PROPOSAL = "AZS_WS_SEYAHAT_PROPOSAL";
        public const string AZS_PAYU3D = "AZS_PAYU3D";
        public const string AZS_WS_LOOKUP = "AZS_WS_LOOKUP";
    }
    public class ALLIANZ_listProcessName
    {
        public const string AZS_WS_KSK_PROPOSAL = "AZS KASKO YAPISI";
        public const string AZS_WS_CSL_PROPOSAL = "AZS KİŞİSEL GÜVENCEM YAPISI";
        public const string AZS_WS_TRAFIK_PROPOSAL = "AZS TRAFIK YAPISI";
        public const string AZS_WS_SEYAHAT_PROPOSAL = "AZS SEYAHAT YAPISI";
    }

    public class ALLIANZ_getProcessDescriptor
    {
        public const string AZS_WS_KSK_PROPOSAL = "AZS KASKO YAPISI";
        public const string AZS_WS_CSL_PROPOSAL = "AZS KİŞİSEL GÜVENCEM YAPISI";
        public const string AZS_WS_TRAFIK_PROPOSAL = "AZS TRAFIK YAPISI";
        public const string AZS_PRINT_PROP_POL = "AZS TEKLİF POLİÇE BASIM";
        public const string AZS_WS_SEYAHAT_PROPOSAL = "AZS SEYAHAT YAPISI";
        public const string AZS_PAYU3D = "PayU 3D Secure";
        public const string AZS_WS_LOOKUP = "AZS LOOK UP";
    }

    public class ALLIANZ_MusteriTipleri
    {
        public const string Ozel = "INS";
        public const string Tuzel = "SIR";
        public const string Yabanci = "FOR";
    }

    public class ALLIANZ_AdresTipleri
    {
        public const string Ev = "1";
        public const string Is = "2";
    }
}

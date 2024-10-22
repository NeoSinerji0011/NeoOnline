using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class MusteriTipleri
    {
        public const short Yok = 0;
        public const short TCMusteri = 1;
        public const short TuzelMusteri = 2;
        public const short SahisFirmasi = 3;
        public const short YabanciMusteri = 4;

        public static bool Ozel(short musteriTipKodu)
        {

            return musteriTipKodu == TCMusteri || musteriTipKodu == YabanciMusteri;
        }

        public static bool Tuzel(short musteriTipKodu)
        {
            return musteriTipKodu == TuzelMusteri || musteriTipKodu == SahisFirmasi;
        }


        public static string MusteriTipi(short? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
            {
                switch (kod)
                {
                    case MusteriTipleri.TCMusteri: result = ResourceHelper.GetString("Real_TC_Nationals_Customer"); break;
                    case MusteriTipleri.TuzelMusteri: result = ResourceHelper.GetString("Corporate_Customers"); break;
                    case MusteriTipleri.SahisFirmasi: result = ResourceHelper.GetString("Sole_Proprietorship_Customer"); break;
                    case MusteriTipleri.YabanciMusteri: result = ResourceHelper.GetString("Foreig_Real_Customer"); break;
                }
            }

            return result;
        }
    }
}

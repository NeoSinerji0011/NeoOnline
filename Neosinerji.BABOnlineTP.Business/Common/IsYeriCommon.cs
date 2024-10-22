using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class CatiTipleri
    {
        public const byte bodrumKat = 1;
        public const byte zeminKat = 2;
        public const byte digerKatlar = 3;
        public const byte ustKat = 4;
        public const byte tumKatlar = 5;
    }

    public class CatiTipi
    {
        public const string bodrumKat = "ÇELİK KONS. HARİÇ BÜTÜN BİN.";
        public const string zeminKat = "ÇELİK KONS. VE DİĞERLERİ    ";

        public static string Tipi(byte Kodu)
        {
            string result = String.Empty;

            switch (Kodu)
            {
                case CatiTipleri.bodrumKat: result = ResourceHelper.GetString("STEEL_CONSTRUCTION_THOUSANDS_EXCEPT_ALL"); break;
                case CatiTipleri.zeminKat: result = ResourceHelper.GetString("STEEL_CONSTRUCTION_AND_OTHERS"); break;

            }

            return result;
        }
    }

    public class KatTipleri
    {
        public const byte bodrumKat = 1;
        public const byte zeminKat = 2;
        public const byte digerKatlar = 3;
        public const byte ustKat = 4;
        public const byte tumKatlar = 5;
    }

    public class KatTipi
    {
        public const string bodrumKat = "Bodrum Kat";
        public const string zeminKat = "Zemin Kat";
        public const string digerKatlar = "Diğer Katlar";
        public const string ustKat = "Üst Katlar";
        public const string tumKatlar = "Tüm Katlar";

        public static string Tipi(byte Kodu)
        {
            string result = String.Empty;

            switch (Kodu)
            {
                case KatTipleri.bodrumKat: result = ResourceHelper.GetString("TheBasementFloor"); break;
                case KatTipleri.zeminKat: result = ResourceHelper.GetString("GroundFloor"); break;
                case KatTipleri.digerKatlar: result = ResourceHelper.GetString("OtherFloors"); break;
                case KatTipleri.ustKat: result = ResourceHelper.GetString("Upstairs"); break;
                case KatTipleri.tumKatlar: result = ResourceHelper.GetString("AllFloors"); break;
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON
{
    public class TURKNIPPONMessage
    {
        private static NumberFormatInfo _numberFormat;
        [XmlIgnore]
        private static NumberFormatInfo NumberFormat
        {
            get
            {
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ".";
                    _numberFormat.NumberDecimalDigits = 2;
                }

                return _numberFormat;
            }
        }

        public static decimal ToDecimal(string value)
        {
            if (String.IsNullOrEmpty(value))
                return decimal.Zero;

            decimal result = 0;
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormat, out result))
            {
                return result;
            }

            return decimal.Zero;
        }



    }
    public class TURKNIPPONInsurerType
    {
        public const int SigortaEttirenYok = 0;
        public const int MalSahibi = 1;
        public const int Kiraci = 2;
        public const int IntifaHakkiSahibi = 3;
        public const int Yonetici = 4;
        public const int Akraba = 5;
        public const int DainiMurtein = 6;
        public const int Diger = 7;
    }

    public class TURKNIPPONUsageType
    {
        public const int Mesken = 1;
        public const int Buro = 2;
        public const int Ticarethane = 3;
        public const int Diger = 4;
    }
    public class TURKNIPPONBuildType
    {
        public const int CelikBetonarmeKarkas = 1;
        public const int YigmaKagir = 2;
        public const int Diger = 3;
    }
    public class TURKNIPPONBuildYear
    {
        public const int Bir = 1; //1975 ve Öncesi
        public const int Iki = 2; //1976-1996
        public const int Uc = 3; //1997-19999
        public const int Dort = 4; //2000-2006
        public const int Bes = 5;//2007 ve Sonrası
    }
    public class TURKNIPPONAnteriorDamage
    {
        public const int Hasarsiz = 1;
        public const int AzHasarli = 2;
        public const int OrtaHasarli = 3;
    }
    public class TURKNIPPONLossPayeeType
    {
        public const string Yok = "Y";
        public const string Banka = "B";
        public const string FinansKurumu = "F";
    }
}

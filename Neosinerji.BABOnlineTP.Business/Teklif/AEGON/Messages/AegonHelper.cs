using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AegonHelper
    {
        public static string AegonReplace(string kelime)
        {
            StringBuilder sb = new StringBuilder(kelime);

            sb = sb.Replace("#231#", "ç");
            sb = sb.Replace("#199#", "Ç");
            sb = sb.Replace("#287#", "ğ");
            sb = sb.Replace("#286#", "Ğ");
            sb = sb.Replace("#305#", "ı");
            sb = sb.Replace("#304#", "İ");
            sb = sb.Replace("#246#", "ö");
            sb = sb.Replace("#214#", "Ö");
            sb = sb.Replace("#351#", "ş");
            sb = sb.Replace("#350#", "Ş");
            sb = sb.Replace("#252#", "ü");
            sb = sb.Replace("#220#", "Ü");
            sb = sb.Replace("#218#", "Ú");
            sb = sb.Replace("#194#", "Â");
            sb = sb.Replace("#213#", "Õ");
            sb = sb.Replace("#204#", "Ì");
            sb = sb.Replace("#205#", "Í");
            sb = sb.Replace("#206#", "Î");
            sb = sb.Replace("#207#", "Ï");
            sb = sb.Replace("#210#", "Ò");
            sb = sb.Replace("#211#", "Ó");
            sb = sb.Replace("#212#", "Ô");
            sb = sb.Replace("#217#", "Ù");
            sb = sb.Replace("#219#", "Û");
            sb = sb.Replace("#226#", "â");
            sb = sb.Replace("#227#", "ã");
            sb = sb.Replace("#236#", "ì");
            sb = sb.Replace("#237#", "í");
            sb = sb.Replace("#238#", "î");
            sb = sb.Replace("#239#", "ï");
            sb = sb.Replace("#242#", "ò");
            sb = sb.Replace("#243#", "ó");
            sb = sb.Replace("#244#", "ô");
            sb = sb.Replace("#245#", "õ");
            sb = sb.Replace("#249#", "ù");
            sb = sb.Replace("#250#", "ú");

            return sb.ToString();
        }
    }
}

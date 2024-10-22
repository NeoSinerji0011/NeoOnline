using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TrafikVergiler
    {
        /// <summary>
        /// THG Fonu
        /// </summary>
        public const int THGFonu = 1;

        /// <summary>
        /// Gider Vergisi
        /// </summary>
        public const int GiderVergisi = 2;

        /// <summary>
        /// Garanti Fonu
        /// </summary>
        public const int GarantiFonu = 3;
    }

    public class VergiOranlari
    {
        // % 
        public const int THGFonu = 5;
        public const int KTGSFonu = 2;
        public const int Gider = 5;
        public const int YSV = 10;
    }
}

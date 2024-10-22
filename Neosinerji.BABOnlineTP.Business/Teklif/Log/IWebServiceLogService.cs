using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.a
{
    public interface IWebServiceLogService : IDisposable
    {
        ITeklif Teklif { get; set; }
        byte IstekTipi { get; set; }
        string Istek { get; set; }
        string Cevap { get; set; }

        void SaveLog();
    }
}

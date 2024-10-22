using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IWebServisLogService
    {
        void BeginLog(string istek, byte istekTipi);
        void BeginLog(object request, Type type, byte istekTipi);
        void EndLog(string cevap, bool basarili);
        void EndLog(object response, bool basarili, Type type);
    }
}

using Neosinerji.BABOnlineTP.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IHataLogService
    {
        List<YetkiHataServisModel> GetHataList(string KullaniciAdi, DateTime Tarih, string Saat, string logType);
    }
}

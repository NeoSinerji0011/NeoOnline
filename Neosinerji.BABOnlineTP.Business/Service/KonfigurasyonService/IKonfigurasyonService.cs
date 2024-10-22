using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IKonfigurasyonService
    {
        string GetKonfigDeger(string Kod);
        KonfigTable GetKonfig(string[] Kodlar);
        Konfigurasyon GetKonfig(string Kod);
        List<Konfigurasyon> GetList();
        Konfigurasyon CreateItem(Konfigurasyon konfigurasyon);
        bool UpdateItem(Konfigurasyon konfigurasyon);
        void DeleteKonfig(string Kod);
    }
}

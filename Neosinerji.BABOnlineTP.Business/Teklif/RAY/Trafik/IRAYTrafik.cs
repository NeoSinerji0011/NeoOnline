using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    public interface IRAYTrafik : ITeklif
    {
        void SetClientIPAdres(string ipadres);
        string MusteriNo(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl);
    }
}

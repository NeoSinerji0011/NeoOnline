using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public interface IANADOLUTrafik : ITeklif
    {
        string MusteriKaydet(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl);
        string AracKullanimTipi(string modelYili, string aracKodu, string tarifeKodu, int tvmKodu, string servisUrl,string grupUrunAdi);
        string AracKullanimSekli(string modelYili, string aracKodu, string tarifeKodu, string kullanimTipi, int tvmKodu, string servisUrl,string grupUrunAdi);
        AnadoluReturnModel AracKullanimTipiList(string modelYili, string aracKodu, string tarifeKodu, int tvmKodu, string grupUrunAdi);
        AnadoluReturnModel AracKullanimSekliList(string anadoluMarkaKodu, string modelYili, string aracKodu, string tarifeKodu, string kullanimTipi, int tvmKodu);

        AnadoluReturnModel AracMarka(string aracKodu, int tvmKodu);

    }
}

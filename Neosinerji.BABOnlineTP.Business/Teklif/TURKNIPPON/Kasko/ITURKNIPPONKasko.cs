using Neosinerji.BABOnlineTP.Business.turknipponkasko;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko
{
    public interface ITURKNIPPONKasko : ITeklif
    {
        string GetKaskoIndirimi(string sigortaliKimlikNo, string sigortaEttirenKimlikNo, string plakaKodu, string plakaNo, DateTime policeBaslangicTarihi, int tvmKodu);
        void PDfGetir(TVMWebServisKullanicilari servisKullanici, MODService clnt, string islemTakipKodu, string policeNo, string musteriNo, int[] basimtipi, bool teklif);
    }
}

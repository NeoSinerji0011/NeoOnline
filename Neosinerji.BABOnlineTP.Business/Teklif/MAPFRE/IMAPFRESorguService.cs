using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public interface IMAPFRESorguService
    {
        EgmSorguResponse EgmSorgu(int tvmKodu, string plakaIlKodu, string plakaNo, string aracRuhsatSeri, string aracRuhsatNo, string asbisNo);
        KimlikSorguResponse KimlikSorgu(int tvmKodu, string kimlikNo);
        KimliktenAdresSorguResponse AdresSorgu(int tvmKodu, string kimlikNo);
        PoliceSorguTrafikResponse PoliceSorguTrafik(int tvmKodu, string kimlikNo, string plakaIlKodu, string plakaNo);
        PoliceSorguKaskoResponse PoliceSorguKasko(int tvmKodu, string kimlikNo, string plakaIlKodu, string plakaNo);
        OtorizasyonResponse OtorizasyonSorgu(int teklifId);
        EskiKaskoBilgiSorguResponse EskiPoliceSorguKasko(string policeNo, string acenteNo, string sirketNo, string yenilemeNo);
        EskiTrafikBilgiSorguResponse EskiPoliceSorguTrafik(string policeNo, string acenteNo, string sirketNo, string yenilemeNo);
        HasarsizlikResponse HasarsizlikSorgu(string kimlikNo, string policeNo, string acenteNo, string sirketNo, string yenilemeNo, string bransKodu);
        OncekiTescilResponse OncekiTescilSorgu(string kimlikNo, string policeNo, string acenteNo, string sirketNo, string yenilemeNo, string bransKodu);
        UrunSorguResponse UrunKoduSorgu(UrunKoduSorguModel sorgu);
        OtorizasyonMesajResponse TeklifOtorizasyonMesaji(string mapfreTeklifNo, string message);
        HazineYururlulukResponse HazineYururluluk(string bransKodu, string kimlikNo, string plakaIlKodu, string plakaNo, string aracTarifeGrupKodu);
        KaskoIkameResponse KaskoIkameSorgu(UrunKoduSorguModel sorgu);

        bool ValidateUser(string userName, string password, out string partajNo);
        bool ValidateUserWithIP(string userName, string password, out string partajNo);
    }
}

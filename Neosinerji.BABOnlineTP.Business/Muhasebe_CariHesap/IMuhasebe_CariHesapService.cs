using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap.Muhasebe_CariHesapService;

namespace Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap
{
    public interface IMuhasebe_CariHesapService
    {
        // Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap.Muhasebe_CariHesapService.
        MuhasebeCariHesapServiceModel MuhasebeCariHesapTcVknAra(string TcVkn, int tvmkodu, int yil);
        MuhasebeCariHesapServiceModel MuhasebeCariHesapUnvan(string Unvan, string UnvanSoyad, int tvmkodu, byte durum, int yil);
        MuhasebeCariHesapServiceModel MuhasebeCariHesapFirmaUnvan(string UnvanFirma, int tvmkodu, byte durum, int yil);
        MuhasebeCariHesapServiceModel MuhasebeCariHesapSatisKanaliAra(int tvmkodu, List<int> tvmlist, int ay, int yil);
        MuhasebeCariHesapServiceModel MuhasebeCariHesapGrupKoduAra(int tvmkodu, string grupKodu, int yil);
        // MuhasebeCariHesapServiceModel MuhasebeCariHesapTarihtenAra(int tvmkodu, DateTime baslangicTarhi, DateTime bitisTarihi);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapTcVknAra(string TcVkn, int tvmkodu, int devirYil);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapUnvan(string Unvan, string UnvanSoyad, int tvmkodu, byte durum);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapFirmaUnvan(string UnvanFirma, int tvmkodu, byte durum);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapSatisKanaliAra(int tvmkodu, List<int> tvmlist);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapGrupKoduAra(int tvmkodu, string grupKodu);
        List<CariHesapGelirGiderListModel> CariHesapGelirGiderListesi(int tvmkodu, string CariHesapTip, int donem, DateTime baslangicTarihi, DateTime bitisTarihi, int aramaTip);

        List<CariHesaplari> getCariHesaplar();
        int CariHesapEkle(CariHesaplari hesap);
        CariHesaplari GetCariDetayYetkili(int id);

        CariHareketleri CariHareketEkle(CariHareketleri hareket);
        CariHareketleri getCariHareketDetay(int id);
        Tuple<string, string> UpdateCariHesap(CariHesaplari carihesap);
        CariHesaplari GetCariHesap(string CariHesapKodu);
        List<CariEvrakTipleri> getCariEvrakTipler();
        int PoliceleriCariyeAktarma(MuhasebeAktarimKonfigurasyon model);
        string getCariEvrakTip(int kodu);
        string getCariOdemeTip(int kodu);
        int CariHesapBorcAlacakKayitEkle(CariHesapBorcAlacak borcAlacakEkle);
        CariHesaplari GetCariKodu(string cariHesapKodu);
        List<CariHesaplari> getCariHesapListesiByUnvan(string unvan, int tvmKodu);
        List<CariHesaplari> getCariHesapListesiByMusteriGrupKodu(string musteriGrupKodu, int tvmKodu);
        List<CariHesaplari> getCariHesapListesiByCariHesapKodu(string cariHesapKodu, int tvmKodu);
        List<CariHesaplari> getCariHesapListesiAll( int tvmKodu);
        int CariHesapGuncelle(CariHesaplari hesap);
        CariHesaplari GetCariDetayGuncelleme(int id);
        List<CariHesaplari> GetCariHesapList(string CariHesapAdi);
        CariHesapEkstreListModel CariHesapEkstre(string hesapkodu, int tvmkodu, string musteriGrupKodu, byte aramaTipi, string donemAraligi, DateTime baslangic, DateTime bitis, int mizanTipi ,int pdfTipi);
        Tuple<string, string> GetCariHesapAdi(string cariHesapKodu);
        List<CariHesapHareketListModel> CariHesapHareketKontrolListesi(string hesapkodu, int tvmkodu, int donem, string musteriGrupKodu, byte aramaTipi, Boolean aramaDurumu, DateTime baslangicTarihi, DateTime bitisTarihi);
        MuhasebeCariHesapServiceModel MuhasebeCariHesapOdemeBelgeNoAra(string OdemeBelgeNo, int tvmkodu, int yil);
        MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapOdemeBelgeNoAra(string OdemeBelgeNo, int tvmkodu, int devirYil);
        void DeleteCariHareket(int id);
        void DeleteCariHesap(int id);
        void DeleteCariHesapBorcAlacak(int id);

        void UpdateCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak);
        void UpdateYeniCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak, decimal brutPrim, decimal komisyonTutari);
      //edit

        Tuple<string, string> CreateCariHesap(string cariHesapKodu, string TCKN, string unvan, string adres, string ulkeKodu, string ilKodu, int? ilceKodu, string telefon, string cepTel, string email, int? postaKodu, string MusteriGrupKodu);
        Tuple<string, string> GetCariHesapAdiByTCKN(string TCKN);
        Tuple<string, string> GetBankaHesapByVKN(string VKN, int policeId);
        Tuple<string, string> GetCariHesapByVKN(string VKN, int policeId);

        CariHesapBorcalacakPeocedureModels GetBorcAlacak(int Donem, string CariHesapKodu);
        List<PoliceGenel> GetTopluPoliceTahsilatList(DateTime BaslangicTarihi, DateTime BitisTarihi, int tvmKodu, List<string> tumKoduList, List<int> tvmlist);
        List<CariHesaplari> GetCariKasaHesapList(string kasaHesabiKodu);
        void UpdatePolTahCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak);
        List<CariHesaplari> GetCariHesapAcenteBankaPosKasaHesapList(string acenteKK);

        List<CariHesapBAReturnModel> CariAktarimIslemleri(int tvmkodu, DateTime tanzimBaslangic, DateTime tanzimBitis);
        CariHareketleri PolTahCariHareketEkle(CariHareketleri hareket);
        void CariAktarimLogAdd(List<CariHesapBAReturnModel> list);
        List<CariHesapBAReturnModel> CariAktarimIslemleri(int tvmKodu);
        List<CariHesapBorcAlacak> CariHesapBorcAlacakGetirByCariHesapKodu(int Donem, string CariHesapKodu);
        List<CariHesapBorcAlacak> CariHesapBorcAlacakGetirByCariHesapKodu(string CariHesapKodu);
        List<CariOdemeTipleri> getCariOdemeTipleriList();
        List<MusteriPolice> GetMusteriPoliceleri(int tvmKodu, string kimlikNo);
        List<CariHareketleri> getCarihareketForPoliceTahsilatRaporu(string TCKNVKN, string EvrakNo);
        List<CariHareketleri> YaslandirmaTablosuCariHesaplari(string PoliceYenilemeEkNo);
        List<PoliceTahsilat> getPoliceTahsilat(string policeNo, string zeyilNo, int? yenilemeNo);

        List<MuhasebeAktarimKonfigurasyon> GetMuhasebeAktarimListesi();
        double GetMuhasebeAktarimYuzdesi(int konfigurasyonId);
        bool deleteCariHareket(string carihesapkodu, int TVMKodu, string evrakNo, string borcAlacakTipi);
    }
    public class MusteriPolice
    {
        public string PoliceId;
        public string PoliceNumarasi;
        public int? YenilemeNo;
        public int? Ekno;
        public decimal? BrutPrim;
        //Acente kk veya acente bireysel kk mi ?
        public Boolean OdemTipiKKveyaBKKmi;
    }
}

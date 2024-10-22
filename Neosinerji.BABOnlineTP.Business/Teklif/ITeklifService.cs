using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.HDI.DASK;
using Neosinerji.BABOnlineTP.Business.AEGON;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITeklifService
    {
        ITeklif Create(ITeklif teklif);
        void UpdateGenelBilgiler(TeklifGenel teklifGenel);

        /// <summary>
        /// Verilen teklifin vergi ve teminat tablolarını silip yeniden kaydeder.
        /// </summary>
        void UpdateTeklif(ITeklif teklif);

        ITeklif GetTeklif(int teklifId);
        TeklifGenel GetTeklifGenel(int teklifId);
        TeklifGenel GetTeklifGenel(int teklifNo, int tvmKodu, int tumKodu);
        ITeklif GetAnaTeklif(int teklifNo, int tvmkodu);
        List<TeklifTUMDetayPartialModel> GetAllListTeklif(int teklifid);
        List<ITeklif> GetTeklifler(int[] teklifIdList);
        List<ITeklif> GetTeklifListe(int teklifNo, int tvmkodu);
        List<TeklifAramaTableModel> PagedList(TeklifListe arama, out int totalRowCount, bool teklifMi);
        List<TeklifAramaTableModel1> PagedList1(TeklifListe1 arama, out int totalRowCount, bool teklifMi);
        List<TeklifOzelDetay> GetMusteriTeklifleri(int MusteriKodu, DateTime? baslangicTarihi, DateTime? bitisTarihi, byte tarihTipi);
        List<int> GetTeklifID(int MusteriKodu);
        List<string> GetTelefonlar(List<int> teklifID);
        List<string> GetAdresler(List<int> teklifID);
        List<TeklifOzelDetay> GetMusteriPoliceleri(int MusteriKodu, DateTime? baslangicTarihi, DateTime? bitisTarihi, byte tarihTipi);
        List<LilyumMusteriKartlari> GetMusteriLilyumKartlari(string kimlikNo);
        void LilyumKartTeminatKullanimCreate(LilyumTeminatKaydetModel kaydetModel, ref bool basarili, ref string mesaj);
        bool lilyumKartReferansGuncelle(string teklifId, string brut, string odemeSekli, string taksitSayisi, string referansNo);
        LilyumKartTeminatKullanim getLilyumTeminatKullanim(string referansNo);
        List<TeklifOtorizasyonTableModel> GetOtorizasyonTeklifler(TeklifOtorizasyonListe arama, out int totalRowCount);
        TeklifSigortali getTeklifSigortali(int teklifId);
        bool AddTeklifSigortali(TeklifSigortali sigortali);

        //Teklif Ara
        List<TeklifAraProcedureModel> TeklifAraPageList(TeklifAraListe arama, out int totalRowCount);
        List<AegonTeklifAraProcedureModel> Aegon_TeklifAraPageList(AegonTeklifAraListe arama, out int totalRowCount);
        List<MapfreTeklifAraProcedureModel> Mapfre_TeklifAraPageList(MapfreTeklifAraListe arama, out int totalRowCount);

        IsDurum GetIsDurumu(int isId);
        IsDurumDetay GetIsDurumDetay(int referansId);
        int ToplamTeklif();
        int ToplamTeklif(int tvmKodu);
        int GetTeklifUrunKodu(int teklifId);

        long GetOfflinePoliceNo(int tvmKodu, int urunKodu);
        HDIDASKEskiPoliceResponse EskiPoliceSorgula(string eskiPoliceNo);
        TeklifOzelAlan TeklifOzelAlan(int teklifId);
        string GetTeklifWebServisCevap(int TeklifId, int webServisCevapKodu);
        int SetIlgiliTeklifId(ITeklif teklif);

        //Teminatlar
        TeklifTeminat GetTeklifTeminat(int TeklifId, int TeminatKodu);
        TeklifTeminat GetAnaTeklifTeminat(int teklifNo, int tvmkodu, int TeminatKodu);

        //Sorular
        TeklifSoru GetTeklifSoru(int TeklifId, int SoruKodu);
        TeklifSoru GetAnaTeklifSoru(int teklifNo, int tvmkodu, int SoruKodu);

        //AEGON
        string AegonOnProvizyonParaBirimi(TeklifGenel teklif);
        decimal AegonOnProvizyonTutar(TeklifGenel teklif, out int gercekTeklifId);
        bool AegonOnProvizyonKontrol(int teklifId);
        bool AegonOnProvizyon(AegonOnProvizyonRequest request, TeklifGenel teklif, out string message);
        AegonOnProvizyonModelDetay AegonOnProvizyonDetay(int teklifId);

        //Metlife 
        TeklifSoru CreateSoru(TeklifSoru soru);

        IsTakip CreateIsTakip(IsTakip isTakip);
        void UpdateIsTakip(IsTakip isTakip);
        IsTakip GetIsTakip(int TeklifId, int MusteriKodu);
        IsTakip GetIsTakip(int TeklifId);
        IsTakip IsTakipGet(int IsTakipId);

        IslerimListeModel GetIslerim(int TVMKullaniciKodu);
        OnayladiklarimListeModel GetOnayladiklarim(int TVMKullaniciKodu);

        IsTakipDetay CreateIsTakipDetay(IsTakipDetay isTakipDetay);
        void UpdateIsTakipDetay(IsTakipDetay isTakipDetay);
        IsTakipDetay GetIsTakipDetay(int id);
        List<IsTakipDetay> GetListTarihceler(int teklifId);

        IsTakipIsTipleri CreateIsTakipIsTipleri(IsTakipIsTipleri isTakipIsTipleri);
        IsTakipIsTipleriDetay CreateIsTakipIsTipleriDetay(IsTakipIsTipleriDetay isTakipIsTipleriDetay);

        IsTakipSoru CreateIsTakipSoru(IsTakipSoru isTakipSoru);
        IsTakipSoru GetIsTakipSoru(int id);
        void DeleteIsTakipSoru(int teklifId);

        IsTakipDokuman GetIsTakipDokuman(int id);
        IsTakipDokuman CreateIsTakipDokuman(IsTakipDokuman isTakipDokuman);
        List<IsTakipDokuman> GetListDokumanlar(int IsTakipId);
        void DeleteIsTakipDokuman(int isTakipDokumanId);


        IsTakipKullaniciGruplari GetIsTakipKullaniciGruplari(int kgid);

        List<IsTakipDetayListeModel> IsTakipDetayPagedList(IsTakipDetayArama arama, out int totalRowCount);
        //---Metlife 
        List<PoliceSorguProcedurModel> PoliceSorgu(PoliceSorguListe arama);

        List<MeslekIndirimiKasko> GetMeslekList();
        CR_MeslekIndirimiKasko GetMeslekKod(string meslekKodu);
        CR_MeslekIndirimiKasko GetTUMMeslekKod(string meslekKodu, int tumKodu);

        int GetTUMPoliceTvmKodu(string tumPoliceNo, string tumBirlikKodu, string tumUrunKodu, string tcknVkn);
        List<KaskoYurticiTasiyiciKademeleri> getYuriciTasiyiciKademleri(string kullanimTarzi);
        List<KaskoTasinanYukKademeleri> getTasinanYukKademleri(string kullanimTarzi);

        bool CreateDigerSirketTeklif(TeklifDigerSirketler teklif);
        List<TeklifDigerSirketler> getDigerTeklifler(int teklifId);

        List<CR_MeslekIndirimiKasko> GetTUMMeslekList(int tumKodu);
        //Teklif Giriş Ekranında Plaka Bilgilerini sorgulamak için kullanılacak
        TeklifArac getTeklifAracDetay(string plakaKodu, string plakaNo);
        List<KaskoHukuksalKorumaBedel> getHukuksalKorumaBedelList();
        decimal getHkKademesi(int id);
        Cr_KaskoHukuksalKoruma getHukuksalKorumaBedel(int tumkodu, decimal bedel);
        TeklifAracDetayModel getTeklifArac(string plakaKodu, string plakaNo);
        bool UpdateTeklifSigortali(TeklifSigortali sigortali);
        string HDIHukuksalKorumaKademesi(string kullanimTarziKodu, string kod2, decimal bedel);

        List<AnadoluKullanimTipSekil> GetAnadoluKullanimTipleri(string kullanimTarzi, byte SorguTipi);

        List<LilyumKartTeminatKullanim> GetLilyumKullaniciTeminatKullanim(int tvmKodu, string referansNo);
        List<LilyumKartTeminatlar> GetLilyumTeminatlar();
        LilyumMusteriKartlari GetMusteriLilyumKartDetay(string kimlikNo, string referansNo, string tvmkodu, string kullanicikodu);
        bool lilyumKartIptalEt( string referansNo);
        TeklifGenel getLilyumReferans(string referansNo);
        List<LilyumMusteriKartlari> GetAcenteKullaniciLilyumKartlari(int tvmKodu, string adSoyad);
        List<LilyumMusteriKartlari> GetAcenteKullaniciLilyumKartlariByTCKN(int tvmKodu, string tckn);
        void UpdateLilyumKartTeminatKullanimlari(List<LilyumKartTeminatKullanimGuncelleModel> guncelTeminatlar);
        LilyumMusteriKartlari GetMusteriLilyumKartDetay(int tvmKodu, string referansNo);
        ReasurorGenel CreateReasurorGenel(ReasurorGenel reasurorGenel);
        List<PoliceDokuman> getReasurorGenelDokumanlar(int policeid);
        void UpdateReasurorGenel(ReasurorGenel reasurorGenel);
        ReasurorGenel getReasurorGenel(string policeid, string teklifid);
        ITeklif getTeklifReasuror( string teklifNo, string tumKodu, int tvmKodu);
        bool deleteReasurorGenel(string policeid, string teklifid);
        bool CreateUnderwriters(List<Underwriters> Underwriters);
        List<Underwriters> getUnderwriters(string policeid, string teklifid);
        bool deleteUnderwriters(string policeid, string teklifid);
        void CreateTeklifDokuman(TeklifDokuman teklifDokuman);
    }
    public class TeklifAracDetayModel
    {
        public TeklifArac teklifArac = new TeklifArac();
        public List<TeklifSoru> teklifSoru = new List<TeklifSoru>();
    }
    public class LilyumTeminatKaydetModel
    {
        public int TvmKodu { get; set; }
        public int KullaniciKodu { get; set; }
        public int KaydedenKullaniciKodu { get; set; }
        public string ReferansNo { get; set; }
    }

    public class LilyumKartTeminatKullanimGuncelleModel
    {
        public int Id { get; set; }
        public byte ToplamKullanilanAdet { get; set; }
        public DateTime? TeminatSonKullanilanTarihi { get; set; }
        public string LilyumKartNo { get; set; }
        public string LilyumReferansNo { get; set; }
    }
}

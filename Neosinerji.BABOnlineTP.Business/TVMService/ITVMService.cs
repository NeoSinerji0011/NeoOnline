using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITVMService
    {
        #region TVM Servis
        DataTableList PagedList(TVMListe tvmListe);

        List<TVMDetay> GetListTVMDetay();
        List<TVMDetay> GetListTVMDetayYetkili();
        List<SelectListItem> GetListTVMDetayForOfflinePolice();
        TVMDetay GetDetay(int kodu);
        TVMDetay GetDetayYetkili(int kodu);
        TVMDetay CreateDetay(TVMDetay detay);
        void UpdateDetay(TVMDetay detay);
        string GetTvmUnvan(int kodu);
        bool TvmTaliVarMi(int kodu);
        
        bool KullaniciTvmyiGormeyeYetkiliMi(int kodu);
        List<TVMDetay> GetListTvmByBolgeKodu(string BolgeKodu);
        TVMDetay GetDetayByVergiNo(String vergiNo);
        TVMDetay GetDetayByTCKimlikNo(String tckNo);
        string GetPoliceByVergiKimlikNo(string vkn, int tvmKodu);
        List<TVMDetay> GetListTVMDetayPoliceTransferTali();
        bool MerkezAcenteMi(int kodu);
        List<TVMOzetModel> GetNeosinerjiTVMTUMListesi(int? bagliOlduguTVMKodu);
        int GetMerkezAcenteSonSatisKanaliKodu(int merkezTVMKodu);
        List<TVMOzetModel> GetTVMListeNeosinerji(int? bagliOlduguTVMKodu);
        TVMDetay GetListTVMDetayInternet(int tvmKod);
        List<TVMOzetModel> GetSatisKanallariListesi(int TVMKodu);
        #endregion
        List<TVMAcentelikleri> GetListTanımliBransOdemeTipleri(int tvmKodu, string tumKodu);
        List<TVMAcentelikleri> GetListTanımliBransTvmSirketOdemeTipleri(int tvmKodu, string tumKodu, int? bransKodu);

        List<int> GetTVMListe(int? bagliOlduguTVMKodu);

        #region TVM Urun Yetkileri
        List<TVMUrunYetkileriOzelModel> GetTVMUrunYetki(int tvmkodu, int urunkodu);
        List<TVMUrunYetkileri> GetListTVMUrunYetkileri();
        List<TVMUrunYetkileri> GetListTVMUrunYetkileri(int id);
        TVMUrunYetkileri GetTVMUrunYetkisi(int tvmKodu, int babonlineUrunKodu, int tumKodu, string tumUrunKodu);
        TVMUrunYetkileri CreateUrunYetki(TVMUrunYetkileri tvm);
        void UpdateUrunYetkileri(TVMUrunYetkileri tvm);
        #endregion

        #region TVM İP Bağlantı Service
        TVMIPBaglanti GetTVMIPBaglanti(int siraNo, int tvmKodu);
        List<TVMIPBaglanti> GetListIPBaglanti();
        IQueryable<TVMIPBaglanti> GetListIPBaglanti(int tvmKodu);
        IQueryable<TVMIPBaglanti> GetListIPBaglantiAnaTvm(int tvmKodu);
        IQueryable<TVMIPBaglanti> GetListIPBaglanti(int tvmKodu, string ip);

        TVMIPBaglanti CreateIPBaglanti(TVMIPBaglanti baglanti);
        bool UpdateItem(TVMIPBaglanti baglanti);
        void DeleteBaglanti(int baglantiKodu, int tvmKodu);

        DataTableList PagedListIPBaglanti(DataTableParameters<TVMIPBaglanti> baglantiList, int tvmKodu);

        List<TVMIPBaglanti> GetTVMIPAra(string ip);
        List<TVMIPBaglanti> GetTVMIPAraByTvmKodu(int TVMKodu);
        Boolean GetTVMIPVarMi(string ip, int tvmKodu);
        #endregion

        #region Notlar Service
        TVMNotlar GetTVMNot(int siraNo, int tvmKodu);
        IQueryable<TVMNotlar> GetListNotlar();
        IQueryable<TVMNotlar> GetListNotlar(int tvmKodu);
        TVMNotlar CreateNot(TVMNotlar not);
        bool UpdateItem(TVMNotlar not);
        void DeleteNot(int NotKodu, int tvmKodu);

        DataTableList PagedListNot(DataTableParameters<TVMNotlar> notList, int tvmKodu);
        #endregion

        #region Dokumanlar Service
        TVMDokumanlar GetTVMDokuman(int siraNo, int tvmKodu);
        IQueryable<TVMDokumanlar> GetListDokumanlar();
        IQueryable<TVMDokumanlar> GetListDokumanlar(int tvmKodu);
        TVMDokumanlar CreateDokuman(TVMDokumanlar dokuman);
        bool UpdateItem(TVMDokumanlar dokuman);
        void DeleteDokuman(int dokumanKodu, int tvmKodu);
        void NeoConnectDeleteSirketSifreYetki(int id);

        bool CheckedFileName(string fileName);
        List<DokumanTurleri> GetListDokumanTurleri();

        DataTableList PagedListDokuman(DataTableParameters<TVMDokumanlar> dokumanList, int tvmKodu);
        #endregion

        #region Bolge Service
        TVMBolgeleri GetTVMBolge(int bolgeKodu, int tvmKodu);
        List<TVMBolgeleri> GetListBolgeler();
        List<TVMOzetModel> GetTVMListeKullaniciYetki(int? bagliOlduguTVMKodu);
        List<TVMOzetModel> GetTVMListeMapfre();
        List<TVMOzetModel> GetTVMListe(string unvan);

        List<TVMBolgeleri> GetListBolgeler(int tvmKodu);
        TVMBolgeleri CreateBolge(TVMBolgeleri bolge);
        bool UpdateItem(TVMBolgeleri bolge);
        void DeleteBolge(int bolgeKodu, int tvmKodu);

        DataTableList PagedListBolge(DataTableParameters<TVMBolgeleri> bolgeList, int tvmKodu);
        #endregion

        bool CreateOdemeGirisi(Kesintiler model);
        bool UpdateOdemeGirisi(Kesintiler model);
        List<Kesintiler> GetOdemeGirisiListe(int acenteKodu, int donem);
        Kesintiler GetOdemeGirisiList(int Id);
        List<Kesintiler> GetMasrafListesi(int acenteKodu, int donem);
        List<TVMDetay> GetListBolgeYetkilisi();
        List<NeoConnectLog> GetListNeoConnectLog(DateTime KullaniciGirisTarihi, DateTime KullaniciCikisTarihi, int tvmKodu, List<string> tumKoduList, List<int> tvmlist);
        TVMDetay getAnaAcenteTvmDetay(int tvmkodu);

        #region Departmanlar Service
        TVMDepartmanlar GetTVMDepartman(int departmanKodu, int tvmKodu);
        List<TVMDepartmanlar> GetListDepartmanlar();
        List<TVMDepartmanlar> GetListDepartmanlar(int tvmKodu);
        TVMDepartmanlar CreateDepartman(TVMDepartmanlar departman);
        bool UpdateItem(TVMDepartmanlar departman);
        void DeleteDepartman(int departmanKodu, int tvmKodu);

        DataTableList PagedListDepartman(DataTableParameters<TVMDepartmanlar> departmanList, int tvmKodu);
        #endregion

        #region Acentelikleri
        TVMAcentelikleri GetTVMAcente(int acenteKodu, int tvmKodu);
        List<TVMAcentelikleri> GetListAcenteler();
        List<TVMAcentelikleri> GetListAcenteler(int tvmKodu);
        TVMAcentelikleri CreateAcente(TVMAcentelikleri acente);
        bool UpdateItem(TVMAcentelikleri acente);
        void DeleteAcente(int acenteKodu, int tvmKodu);

        DataTableList PagedListAcente(DataTableParameters<TVMAcentelikleri> acenteList, int tvmKodu);
        #endregion

        #region TVMYetki Service

        //TVMYetkiGruplari GetYetkiGrup(int Id);
        //List<TVMYetkiGruplari> GetListYetkiGrup(int TVMKodu);
        //List<TVMYetkiGruplari> GetListYetkiGrup();
        //List<TVMYetkiGrupYetkileri> GetListYetkiGrupYetkileri();
        //List<TVMYetkiGrupYetkileri> GetListYetkiGrupYetkileri(int yetkiGrupKodu);
        //List<TVMYetkiGrupYetkileri> GetListYetkiGrupYetkileriTVM(int TvmKodu);

        #endregion

        #region BankaHesaplari Service
        TVMBankaHesaplari GetTVMBankaHesap(int SiraNo, int tvmKodu);
        List<TVMBankaHesaplari> GetListTVMBankaHesaplari();
        List<TVMBankaHesaplari> GetListTVMBankaHesaplari(int tvmKodu);
        List<TVMBankaHesaplari> GetListTVMBankaHesaplari(int tvmKodu, int hesaptipi);
        TVMBankaHesaplari GetListTVMBankaCariHesaplari(int tvmKodu, int hesaptipi, string cariHesapNo);
        bool CheckListTVMBankaCariHesaplari(int tvmKodu, int hesaptipi, string cariHesapNo);
        TVMBankaHesaplari CreateTVMBankaHesap(TVMBankaHesaplari bankahesap);
        bool UpdateBankaHesap(TVMBankaHesaplari bankahesap);
        void DeleteTVMBankaHesap(int SiraNo, int tvmKodu);

        DataTableList PagedListTVMBankaHesap(DataTableParameters<TVMBankaHesaplari> bankahesapList, int tvmKodu);
        #endregion

        #region IletisimYetkilileri Service
        TVMIletisimYetkilileri GetTVMIletisimYetkili(int SiraNo, int tvmKodu);
        List<TVMIletisimYetkilileri> GetListTVMIletisimYetkilileri();
        List<TVMIletisimYetkilileri> GetListTVMIletisimYetkilileri(int tvmKodu);
        TVMIletisimYetkilileri CreateTVMIletisimYetkili(TVMIletisimYetkilileri iletisimyetkili);
        bool UpdateIletisimYetkili(TVMIletisimYetkilileri iletisimYetkili);
        void DeleteTVMIletisimYetkili(int SiraNo, int tvmKodu);

        DataTableList PagedListTVMIletisimYetkili(DataTableParameters<TVMIletisimYetkilileri> iletisimYetkilileriList, int tvmKodu);
        #endregion

        #region TVMKullaniciNotlar Service
        TVMKullaniciNotlar CreateKullaniciNot(TVMKullaniciNotlar not);
        bool UpdateKullaniciNot(TVMKullaniciNotlar not);
        List<TVMKullaniciNotlar> GetListKullaniciNotlar();
        bool DeleteKullaniciNot(int KullaniciNotId);
        TVMKullaniciNotlar GetKullaniciNot(int notId);
        #endregion

        #region NeoConncect Kullanicilari Service
        OtoLoginSigortaSirketKullanicilar GetNeoConnectKullanicilari(int tvmKodu, int id);
        List<OtoLoginSigortaSirketKullanicilar> GetListNeoConnectKullanicilari(int tvmKodu);
        OtoLoginSigortaSirketKullanicilar GetNeoConnectKullanici(int anaTvmKodu, int TUMKodu);
        OtoLoginSigortaSirketKullanicilar CreateNeoConnectKullanicilari(OtoLoginSigortaSirketKullanicilar kullanici);
        bool UpdateNeoConnectKullanicilari(OtoLoginSigortaSirketKullanicilar kulllanici);
        void DeleteNeoConnectKullanicilari(int id);
        #endregion

        #region NeoConncectTvmSigortaSirket Kullanicilari Service
        NeoConnectTvmSirketYetkileri GetNeoConnectTvmSirketKullanicilari(int tvmKodu, string tumKodu);
        List<NeoConnectTvmSirketYetkileri> GetListNeoConnectTvmSirketKullanicilari(int tvmKodu);
        List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectSirketGrupKullaniciList(int tvmKodu, string tumKodu);
        NeoConnectSirketGrupKullaniciDetay GetNeoConnectSirketGrupKullaniciDetay(int grupKodu);
        NeoConnectTvmSirketYetkileri CreateNeoConnectTvmSirketKullanicilari(NeoConnectTvmSirketYetkileri kullanici);
        bool UpdateNeoConnectTvmSirketKullanicilari(NeoConnectTvmSirketYetkileri kulllanici);
        void DeleteNeoConnectTvmSirketKullanicilari(int tvmKodu, string tumKodu);
        List<TVMDetay> GetAcenteList(int bagliOlduguTvmKodu);
        #endregion

        #region NeoConncect Yasakli Urller Service
        NeoConnectYasakliUrller GetNeoConnectYasakliUrlKullanicilari(int tvmKodu, int tumKodu);
        List<NeoConnectYasakliUrller> GetListNeoConnectYasakliUrlKullanicilari(int tvmKodu);
        NeoConnectYasakliUrller CreateNeoConnectYasakliUrlKullanicilari(NeoConnectYasakliUrller kullanici);
        bool UpdateNeoConnectYasakliUrlKullanicilari(NeoConnectYasakliUrller kulllanici);
        void DeleteNeoConnectYasakliUrlKullanicilari(int tvmKodu, int tumKodu);
        #endregion

        #region WebServis Kullanicilari Service
        TVMWebServisKullanicilari GetTVMWebServisKullanicilari(int tvmKodu, int tumKodu);
        List<TVMWebServisKullanicilari> GetListTVMWebServisKullanicilari(int tvmKodu);
        TVMWebServisKullanicilari CreateTVMWebServisKullanicilari(TVMWebServisKullanicilari kullanici);
        bool UpdateTVMWebServisKullanicilari(TVMWebServisKullanicilari kulllanici);
        void DeleteTVMWebServisKullanicilari(int tvmKodu, int tumKodu);
        #endregion

        #region TVM Harita Adres

        TVMLocationModelList GetListTVMHarita();

        #endregion

        #region Acente Odeme Kesintileri
        List<Kesintiler> GetListKesintiler(int TVMKodu);
        List<KesintiTurleri> GetListKesintiTurleri();

        #endregion

        bool AddTaliTVM(TVMDetay tvmDetay);

        int GetYetkiGrupKodu(int tvmkod);

        OfflineUretimPerformans GetOffilenUretimPerformans(int tvmKodu, int donemYil, string taliTvmler);

        int GetServisKullaniciTVMKodu(int tvmKodu);

        List<TVMOzetModel> GetDisUretimTVMListeKullaniciYetki(int? bagliOlduguTVMKodu);
        List<ParaBirimleriModel> GetParaBirimleri();

        OtoLoginSigortaSirketKullanicilar GetAutoLoginKullanici(int id);
        NeoConnectTvmSirketYetkileri GetNeoConnectTvmSirketYetkileriKullanici(int id);
        NeoConnectYasakliUrller GetNeoConnectYasakliUrllerKullanici(int id);
        List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectGrupKullanicilist(int tvmKodu, string sirketKodu);

        List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectSirketGrupKullaniciDetayByTVMKodu();
        void DeleteNeoConnectSirketGrupKullaniciDetayByTVMKodu(int id);
        void CreateNeoConnectSirketGrupKullaniciDetayByTVMKodu(NeoConnectSirketGrupKullaniciDetay grup);
        void UpdateNeoConnectSirketGrupKullaniciDetayByTVMKodu(NeoConnectSirketGrupKullaniciDetay grup);


        List<NeoConnectGrupKullanicilistGuncelleSonucModel> NeoConnectGrupKullanicilistGuncelle(int tvmKodu, List<NeoConnectSirketGrupKullaniciDetay> listModel);
        List<NeoConnectGrupKullanicilistGuncelleSonucModel> NeoConnectMerkezKullanicilistGuncelle(int tvmKodu, List<OtoLoginSigortaSirketKullanicilar> listModel);
        List<OtoLoginSigortaSirketKullanicilar> NeoConnectSirketListesi(int tvmkodu);
        List<OtoLoginSigortaSirketKullanicilar> NeoConnectSirketListesi();
        List<NeoConnectSirketGrupKullaniciDetay> GetNeoconnectGruplist();
        List<NeoConnectSirketGrupKullaniciDetay> GetListNeoConnectGrupSirketleri(int? tumKodu, int tvmKodu);
        string GetGrupAdi(int grupkodu);
        TVMSMSKullaniciBilgi GetSmsKullaniciBilgileri(int tvmKodu);
        bool KullaniciMobilOnayKoduEkle(int tvmKodu, int kullanicikodu, string mobilOnayKodu);
        bool KullaniciMobilOnayKoduDogrula(int tvmKodu, int kullanicikodu, string mobilOnayKodu);
        bool KullaniciMobilOnayKoduSifirla(int tvmKodu, int kullanicikodu, string mobilOnayKodu);
        void MobilOnayDogrulandi(int tvmKodu, int kullanicikodu);
        List<OtoLoginSigortaSirketKullanicilar> GetNeoConnectAra(int? tumkodu, int? grupKodu, int tvmKodu);
        List<OtoLoginSigortaSirketKullanicilar> GetNeoConnectSirketAra(int? tumkodu, int tvmKodu);
        bool KokpitGuncelle(int tvmKodu, int donemYil, string taliAcenteler, string branslar);
        bool KokpitKullaniciGuncelle(int tvmKodu, int donemYil, string kullanicikodu, string branslar);
        void UnicoApiKeyVarMi(int tvmKodu, out string apiKey, out string autKey);
        ParaBirimleri GetParaBirimiByBirimi(string paraBirimi);
    }

    public class NeoConnectGrupKullanicilistGuncelleSonucModel
    {
        public string grupAdi { get; set; }
        public bool basarili { get; set; }
    }

}

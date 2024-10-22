using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Neosinerji.BABOnlineTP.Business.PoliceTransfer.PoliceService;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IPoliceService
    {
        void Add(List<Police> policeler);
        int AddPolice(Police police);
        Boolean AddMusteriGenelBilgisi(List<MusteriGenelBilgiler> musteriler);
        bool UpdatePolice(PoliceGenel police);
        void UpdateOfflinePolice(int policeId, byte Yeni_is);
        bool UpdatePoliceReasuror(Business.Police police);
        bool UpdatePoliceSigortaEttiren(PoliceSigortaEttiren police);

        bool DeletePolice(Police police);
        bool UpdatePoliceMuhasebe(Police police, decimal brutPrim, decimal komisyonTutari);
        bool DeleteSadecePolice(Police police);
        //bool DeletePoliceMuhasebe(Police police);
        //bool UpdatePoliceMuhasebe(Police police);
        void GencPolicelerAdd(GencListeler policeler);
        int getBasariliKayitlar();
        SigortaSirketleri getSirketBilgisi(string tumKodu);
        int getUpdateKayit();
        int getVarolanKayitlar();
        int getHataliEklenmeyenKayitlar();
        List<PoliceKontrol> getVarOlanPoliceler();
        List<PoliceKontrol> getEklenmeyenPoliceler();

        bool CreatePoliceTahsilat(PoliceTahsilat model);
        bool UpdatePoliceTahsilat(PoliceTahsilat model);
        Police GetPoliceById(int policeId);
        // Police PoliceOffLinePolNo(int policeId);
        PoliceOdemePlani GetPoliceOdemePlani(int policeId, int taksitSayisi);
        PoliceTahsilat GetPoliceTahsilat(int policeId, int taksitSayisi);
        PoliceOffLineServiceModel PoliceOffLinePolNo(bool merkezAcentemi, string policeNo, int tvmkodu, string donemAraligi);
        PoliceOffLineServiceModel PoliceOffLineMustNo(string MustNo, int tvmkodu, DateTime baslangic, DateTime bitis);
        PoliceOffLineServiceModel PoliceOffLineTvmKod(int tvmkodu, DateTime baslangic, DateTime bitis);
        //PoliceOffLineServiceModel PoliceOffLineTvm(string Tvm, int tvmkodu, DateTime baslangic, DateTime bitis); 
        PoliceOffLineServiceModel PoliceOffLineTcVkn(bool merkezAcentemi, string TcVkn, int tvmkodu, string donemAraligi);
        PoliceOffLineServiceModel PoliceOffLineTcVknSigortaEttiren(string TcVkn, int tvmkodu,int donem);
        PoliceOffLineServiceModel PoliceOffLineTcVknSigortaEttiren(string TcVkn, int tvmkodu);
        PoliceOffLineServiceModel PoliceOffLinePlakaNo(bool merkezAcentemi, string plakaNo, string plakaKodu, int tvmkodu, string donemAraligi);
        PoliceOffLineServiceModel PoliceOffLineUnvan(bool merkezAcentemi, string Unvan, string UnvanSoyad, int tvmkodu, byte durum, string donemAraligi);
        PoliceOffLineServiceModel PoliceOffLineUnvanSigortaEttiren(string Unvan, string UnvanSoyad, int tvmkodu, byte durum,int donem);
        PoliceOffLineServiceModel PoliceOffLineFirmaUnvan(bool merkezAcentemi, string UnvanFirma, int tvmkodu, byte durum, string donemAraligi);
        PoliceOffLineServiceModel PoliceOffLineFirmaUnvanSigortaEttiren(string UnvanFirma, int tvmkodu, byte durum, int donem);

        //PoliceOffLineServiceModel PoliceOffLinePolNoTali(string policeNo, int tvmkodu, int donem);
        //PoliceOffLineServiceModel PoliceOffLineTcVknTali(string TcVkn, int tvmkodu,int donem);
        //PoliceOffLineServiceModel PoliceOffLineUnvanTali(string Unvan, string UnvanSoyad, int tvmkodu, byte durum,int donem);
        //PoliceOffLineServiceModel PoliceOffLineFirmaUnvanTali(string UnvanFirma, int tvmkodu, byte durum,int donem);

        //PoliceOffLineServiceModel PoliceOffLinePlakaNoTali(string plakaNo, string plakaKodu, int tvmkodu,int donem);
        PoliceGenel getPolice(int? tvmKodu, string sigortaSirketiKodu, string policeNo, int ekNo);
        PoliceGenel GetPolice(int policeId);
        PoliceGenel getManuelPolice(string sigortaSirketiKodu, string policeNo, int ekNo, int yenilemeNo, int bransKodu);
        PoliceGenel getManuelPolice(string policeNo, int ekNo, int yenilemeNo);
        PoliceGenel getOfflinePolice(string sigortaSirketiKodu, string policeNo, int ekNo, int yenilemeNo, int bransKodu);
        PoliceGenel TaliAcnteKomisyonGuncelle(PoliceGenel police, out bool guncellendiMi);
        bool GetPoliceUretimHedefGerceklesen(PoliceGenel police);
        MusteriGenelBilgiler getMusteri(string kimlikNo, int tvmKodu);
        MusteriAdre getMusteriAdres(string adres, string ilKodu, string mahalle, string cadde, int ilceKodu, string apartman, string sokak, string binaNo, string daireNo, int musteriKodu);
        string getIl(string ilAdi);
        int musAdresSiraNo(int musteriKodu);
        int musTelefonSiraNo(int musteriKodu);
        MusteriTelefon getMusteriTelefon(string numara, int iletsimNumaraTipi, int musteriKodu);
        MusteriGenelBilgiler getMusteriler(string kimlikNo, int tvmKodu);
        TVMKullanicilar GetMusteriTVMKullanicilarByTVMKodu(int tvmKodu);
        PoliceGenel getTahsilatPolice(string sigortaSirketiKodu, string policeNo, int ekNo);
        PoliceOdemePlani GetTopluTahsilatPoliceOdemePlani(int policeId);
        List<OtoLoginSigortaSirketKullanicilar> getOtoLoginSigortaSirketKullanicilar(int tvmkodu);
        bool OdemeTipiGuncelle(int policeId, string odemeTipi);
        PoliceGenel getZeylinPolicesi(int tvmKodu, string sigortaSirketiKodu, string policeNo, int ekNo);
        bool PoliceKimlikGuncelle(int policeId, string kimlikNo);
        List<PoliceGenel> GetGuncellenecekPoliceZeyl(int tvmKodu, string TumBirlikKodu, string PoliceNo);
        List<IsTipleri> getListIsTipleri();
        KimlikNoUret GetKimlikNoUret();
        Tuple<string, string, string, string,string> GetTCKNBilgileriByTCKN(string kimlikNo);
        List<MusteriGenelBilgiler> GetTCKNBilgileriByAdSoyad(string ad, string soyad);
        PoliceArac getPoliceAracDetay(string plakaKodu, string plakaNo);
        SigortaSirketleri getSigortaSirketi(string sirketKodu);
        bool policeOdemePlaniIsOdemeTipiGuncelle(int policeId, string odemeTipi);
        List<CariHesaplari> getSirketler();

        MusteriBilgiModel getMusteriDetay(string ad, string soyad);
        List<PoliceDokuman> getPoliceDokumanlar(int policeId);
        bool createPoliceDokuman(PoliceDokuman model);
        PoliceYaslandirmaTablosuModel getBrokerPoliceRapor(PoliceYaslandirmaTablosuModel model);
        TeklifPoliceListesiUWDetay getBrokerPoliceTeklifListesiUWDetayli(TeklifPoliceListesiUWDetay model);




    }
    public class MusteriBilgiModel
    {
        public string kimlikNo { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string HataMesaji { get; set; }
    }
}

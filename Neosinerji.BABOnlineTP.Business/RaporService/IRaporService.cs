using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace Neosinerji.BABOnlineTP.Business
{
    public interface IRaporService
    {
        List<SubeSatisRaporProcedureModel> SubeSatisPagedList(SubeSatisListe arama, out int totalRowCount);
        List<MTSatisRaporProcedureModel> MtSatisPagedList(MtSatisListe arama, out int totalRowCount);
        List<PoliceRaporProcedureModel> PoliceRaporPagedList(PoliceRaporListe arama, out int totalRowCount);
        // List<PoliceListesiOfflineRaporProcedureModel> PolicelistesiOfflineRaporPagedList(PoliceRaporOfflineListe arama, out int totalRowCount);

        List<PoliceListesiOfflineRaporProcedureModel> PoliceListesiOfflineRaporGetir(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                               string sigortaSirketleriArray, string bransKoduArray,
                                               string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu,string uretimTvmlerArray);
        
        List<ReasurorPoliceListesiProcedureModel> ReasurorPoliceListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                  string sigortaSirketleriArray, string bransKoduArray,
                                                  string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray);
        List<ReasurorTeklifListesiProcedureModel> ReasurorTeklifListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                          string sigortaSirketleriArray, string bransKoduArray,
                                          string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray);
        List<KartListesiProcedureModel> KartListesiRaporGetir(DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray, byte? OdemeSekli, int anaTVMKodu, string ad, string soyad, string tckn,int durum);
        List<PoliceUretimRaporProcedureModel> PoliceUretimRaporPagedList(PoliceUretimRaporListe arama, int tvmKodu, out int totalRowCount);
        List<PoliceUretimRaporProcedureModelTali> PoliceUretimRaporTaliPagedList(PoliceUretimRaporListeTali arama, int tvmKodu, out int totalRowCount);
        List<PoliceHedefRaporModel> PoliceHedefGerceklesenRapor(int tvmKodu, int talitvmKodu, int yil);
        List<VadeTakipRaporuProcedureModel> VadeTakipRaporuPagedList(VadeTakipRaporuListe arama, out int totalRowCount);
        //List<OfflinePoliceProcedureModel> OfflinePolicePagedList(OfflinePoliceListe arama, out int totalRowCount);
        List<TeklifRaporuProcedureModel> TeklifRaporuPagedList(TeklifRaporuListe arama, out int totalRowCount);
        List<KrediliHayatPoliceRaporProcedureModel> KrediliHayatPoliceRaporPagedList(KrediliHayatPoliceRaporListe arama, out int totalRowCount);
        List<OzetRaporProcedureModel> OzetRaporPagedList(OzetRaporListe arama, out int totalRowCount);
        List<OzetRaporProcedureModel> OzetRaporPagedListTest(OzetRaporProcedureRequestModel data);
        List<AracSigortalariIstatistikRaporuProcedureModel> AracSigortaliIstatistikRaporPagedList(AracSigortalariIstatistikRaporuListe arama, out int totalRowCount);
        //List<GenelRaporProcedureModel> GenelRaporPagedList(GenelRaporListe arama, out int totalRowCount);
        OfflinePolouse CreateOfflinePolice(OfflinePolouse offlinePolice);
        string PoliceNoKontrol(string policeNo, int tvmkodu);
        PoliceHaritaModel GetPoliceHarita(string[] policeList);

        #region RaporUretimIcmal
        List<PoliceUretimIcmalRaporProcedureModel> PoliceIcmalRaporGetir(int tvmKodu, byte Merkez, int ay, int yil, string SigortaSirketleriListe, string bransKoduListe, string tvmlerListe, string disKaynakLsite, int raporTip);

        #endregion
        //tahsilatraporu
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string SigortaSirketleriArray, string TcVkn, string tvmlerListe, int RaporTipi);
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatMusteriGrupKoduRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string MusteriGrupKodu, int RaporTipi);
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatDisKaynakRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string disKaynakListe, int RaporTipi);

        #region Mapfre Rapor Metod
        List<MapfreBolgeUretimModel> MapfreBolgeUretimRapor(DateTime BaslangicTarih, DateTime BitisTarih, int? BolgeKodu, bool Acenteler);
        #endregion

        #region Aegon Rapor Method
        List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporuPagedList(AegonTeklifRaporuListe arama, out int totalRowCount);
        List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporuExcelAktar(AegonTeklifRaporuExcelAktar arama);
        #endregion
        //Vade Takip Raporundan İş Atama için kullanılıyor
        ResultModelAtananIs AtananIslerCreate(List<AtananIsler> AtanacakIsler);
        List<int> AtananIsListesi(List<int> polId);

        List<MusteriGenelBilgiler> TahsilatTakipRaporuGrupKoduAra(int tvmkodu, string grupKodu, DateTime baslangicTarhi, DateTime bitisTarihi, byte raportipi);
        List<PoliceTahsilat> PolTahKimlikNoSorgula(string kimlikno);

    }
    public class ResultModelAtananIs
    {
        public string hataMesaji { get; set; }
        public int basariliKayitSayisi { get; set; }
        public List<HataliKayit> hataliKayitlar { get; set; }
    }
    public class HataliKayit
    {
        public string policeNo { get; set; }
        public int? ekno { get; set; }
        public int? yenilemeNo { get; set; }
        public string sirketKodu { get; set; }
        public string hataMesaji { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Data;
using System.Data.SqlClient;
using System.Common;
using System.Data.Common;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface ITUMContext : IUnitOfWork
    {
        TUMBankaHesaplariRepository TUMBankaHesaplariRepository { get; }
        TUMDetayRepository TUMDetayRepository { get; }
        TUMDokumanlarRepository TUMDokumanlarRepository { get; }
        TUMDurumTarihcesiRepository TUMDurumTarihcesiRepository { get; }
        TUMIletisimYetkilileriRepository TUMIletisimYetkilileriRepository { get; }
        TUMIPBaglantiRepository TUMIPBaglantiRepository { get; }
        TUMNotlarRepository TUMNotlarRepository { get; }
        TUMUrunleriRepository TUMUrunleriRepository { get; }

        #region Procedure

        List<SubeSatisRaporProcedureModel> SubeRapor_Getir(int policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                                  string urunKoduArray, string bransKoduArray, int DovizTL, int TahIpt, int tvmKodu);

        List<MTSatisRaporProcedureModel> MTRapor_Getir(int policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                        string urunKoduArray, string bransKoduArray, string subeler, byte? OdemeSekli, byte? OdemeTipi);

        List<PoliceRaporProcedureModel> PoliceRapor_Getir(byte policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                          string urunKoduArray, string bransKoduArray, string subelerArray,
                                                          string PoliceNo, byte? OdemeSekli, byte? OdemeTipi);
        //pol listesi offline
        List<PoliceListesiOfflineRaporProcedureModel> PoliceListesiOfflineRaporGetir(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                  string sigortaSirketleriArray, string bransKoduArray,
                                                  string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray);
        List<ReasurorPoliceListesiProcedureModel> GetReasurorPoliceListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                           string sigortaSirketleriArray, string bransKoduArray,
                                                           string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray);
        List<ReasurorTeklifListesiProcedureModel> GetReasurorTeklifListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                   string sigortaSirketleriArray, string bransKoduArray,
                                                   string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray);
        List<KartListesiProcedureModel> KartListesiRaporuGetir(DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                            byte? OdemeSekli, int anaTVMKodu, string ad, string soyad, string tckn, int durum);
        List<PoliceUretimRaporProcedureModel> PoliceUretimRapor_Getir(DateTime basTarihi, DateTime bitTarihi,
                                                                 string SigortaSirketleriArray, string bransKoduArray, string tvmlerArray, string uretimTvmlerArray, int anatvmKodu);
        List<PoliceUretimIcmalRaporProcedureModel> PoliceIcmalRaporGetir(int tvmKodu, byte Merkez, int ay, int yil, string SigortaSirketleriListe, string bransKoduListe, string tvmlerListe, string disKaynakListe, int raporTip);

        //TAHSİLAT 
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string SigortaSirketleriArray, string TcVkn, string tvmlerListe, int RaporTipi);
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatMusteriGrupKoduRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string kimlikNo, int RaporTipi);
        List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatDisKaynakRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string disKaynakListe, int RaporTipi);

        List<PoliceUretimRaporProcedureModelTali> PoliceUretimRaporTali_Getir(int ay, int yil,
                                                                string SigortaSirketleriArray, string bransKoduArray, string tvmlerArray, int anatvmKodu);

        List<PoliceHedefRaporModel> PoliceHedefRaporGerceklesen(int TvmKod, int taliKodu, int yil);

        List<VadeTakipRaporuProcedureModel> SP_VadeTakipRaporu_Deneme(DateTime? baslangicTarihi, DateTime? bitisTarihi, string urunKoduArray, string bransKoduArray,
                                                                 string subelerArray, byte? TarihAraligi);

        List<KrediliHayatPoliceRaporProcedureModel> KrediliHayatPoliceRapor_Getir(byte TarihTipi, int? Doviztl, DateTime? baslangicTarihi,
                                                            DateTime? bitisTarihi, string subelerArray, string PoliceNo, byte? OdemeSekli, byte? OdemeTipi);

        List<TeklifRaporuProcedureModel> TeklifRaporu_Getir(byte TarihTipi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                            string subelerArray, string Brans, string Urun, int? DovizTl, int? TeklifNo);

        //Teklif Ara Stored Procedure çevrildi
        List<TeklifAraProcedureModel> TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu, int? hazirlayankullanici, int? tumKodu,
                                                             int? teklifDurumu, int aktifTVMKoduKodu, int Skip, int Take);

        //Aegon Teklif Ara Stored Procedure çevrildi
        List<AegonTeklifAraProcedureModel> Aegon_TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu, int? hazirlayankullanici,
                                                            int AktifTVMKodu, int Skip, int Take);
        //Mapfre Teklif Ara Stored Procedure çevrildi
        List<MapfreTeklifAraProcedureModel> Mapfre_TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, string TumTeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu, int? hazirlayankullanici,
            int? teklifDurumu, int AktifKullaniciKodu, int Skip, int Take);

        List<OzetRaporProcedureModel> OzetRapor_Getir(int TarihTipi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string urunKoduArray, string bransKoduArray, int? Doviztl, int? AramaKriteri, int? Data, int? Data2, string Data3, int TvmKodu, int YetkiKodu);
        List<AracSigortalariIstatistikRaporuProcedureModel> AracSigortalariIstatistikRaporu_Getir(int? TvmKodu, int? TarihTipi, DateTime? baslangicTarihi,
                                                            DateTime? bitisTarihi, int? urun, int? Doviztl, int? AramaKriteri, int? Kodu, int? Data2, string SorguTuru);

        GenelRaporProcedureModel GenelRapor_Getir(int? TvmKodu, string ProjeKodu);
        #endregion       

        #region Aegon Rapor Procedure

        // ==== AEGON Teklif Raporu Getirir ====//
        List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporu_Getir(int tarihtipi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subeler, string urunler,
                                                                             int parabirimi, int? teklifno, int? yillikprimmin, int? yillikprimmax, int skip, int take);
        #endregion


    }

    public class TUMContext : ITUMContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public TUMContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
            //  _dbContext.Database.Connection.CreateCommand().Parameters.Clear();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout = 180;


        }

        #region IUnitOfWork
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
        #endregion

        #region Procedure

        // ==== Şube Raporu Getirir ====//
        public List<SubeSatisRaporProcedureModel> SubeRapor_Getir(int policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                                  string urunKoduArray, string bransKoduArray, int DovizTL, int TahIpt, int tvmKodu)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter police = new SqlParameter("@policetarihi", SqlDbType.Int);
            police.Value = policeTarihi;

            SqlParameter urunkodu = new SqlParameter("@urunkodu", SqlDbType.NVarChar, 200);
            urunkodu.Value = urunKoduArray;

            SqlParameter branskodu = new SqlParameter("@branskodu", SqlDbType.NVarChar, 200);
            branskodu.Value = bransKoduArray;

            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = tvmKodu;

            List<SubeSatisRaporProcedureModel> model = _dbContext.Database.SqlQuery<SubeSatisRaporProcedureModel>
                                                       ("SP_SubeSatis @policetarihi, @baslamatarihi, @bitistarihi,@branskodu, @urunkodu,@tvmkodu",
                                                        police, baslama, bitis, branskodu, urunkodu, tvmkodu)
                                                       .ToList<SubeSatisRaporProcedureModel>();
            return model;
        }

        // ==== MT Satış Raporu Getirir ====//
        public List<MTSatisRaporProcedureModel> MTRapor_Getir(int policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                       string urunKoduArray, string bransKoduArray, string subeler, byte? OdemeSekli, byte? OdemeTipi)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter police = new SqlParameter("@policetarihi", SqlDbType.Int);
            police.Value = policeTarihi;

            SqlParameter urunkodu = new SqlParameter("@urunkodu", SqlDbType.NVarChar, 200);
            urunkodu.Value = urunKoduArray;

            SqlParameter branskodu = new SqlParameter("@branskodu", SqlDbType.NVarChar, 200);
            branskodu.Value = bransKoduArray;

            SqlParameter subekodu = new SqlParameter("@subeler", SqlDbType.NVarChar, 200);
            subekodu.Value = subeler;

            SqlParameter odemeSekli = new SqlParameter("@odemesekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemetipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;

            List<MTSatisRaporProcedureModel> model = _dbContext.Database.SqlQuery<MTSatisRaporProcedureModel>
                                                      ("SP_MTSatisTest @policetarihi, @baslamatarihi, @bitistarihi, @urunkodu,@branskodu,@subeler,@odemesekli,@odemetipi",
                                                       police, baslama, bitis, urunkodu, branskodu, subekodu, odemeSekli, odemeTipi)
                                                      .ToList<MTSatisRaporProcedureModel>();

            return model;
        }

        // ==== Poliçe Raporu Getirir online ====//
        public List<PoliceRaporProcedureModel> PoliceRapor_Getir(byte policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                                  string urunKoduArray, string bransKoduArray, string subelerArray,
                                                                  string PoliceNo, byte? OdemeSekli, byte? OdemeTipi)
        {

            if (!String.IsNullOrEmpty(PoliceNo.Trim()))
            {
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);


            }

            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter police = new SqlParameter("@policetarihi", SqlDbType.Int);
            police.Value = policeTarihi;

            SqlParameter urunkodu = new SqlParameter("@urunkodu", SqlDbType.NVarChar, 200);
            urunkodu.Value = urunKoduArray;

            SqlParameter branskodu = new SqlParameter("@branskodu", SqlDbType.NVarChar, 200);
            branskodu.Value = bransKoduArray;

            SqlParameter subekodu = new SqlParameter("@subeler", SqlDbType.NVarChar, 200);
            subekodu.Value = subelerArray;

            SqlParameter policeNo = new SqlParameter("@policeNo", SqlDbType.NVarChar, 200);
            policeNo.Value = PoliceNo;




            SqlParameter odemeSekli = new SqlParameter("@odemesekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemetipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;

            //SqlParameter durum = new SqlParameter("@durum", SqlDbType.TinyInt);
            //durum.Value = Durum ?? 0;


            List<PoliceRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<PoliceRaporProcedureModel>
                                                                ("SP_PoliceRapor @policetarihi, @baslamatarihi, @bitistarihi, @urunkodu," +
                                                                " @branskodu, @subeler,@odemesekli,@odemetipi,@policeNo"
                                                                , police, baslama, bitis, urunkodu, branskodu, subekodu, odemeSekli, odemeTipi, policeNo)
                                                                .ToList<PoliceRaporProcedureModel>();

            return rapor;
        }

        // offline poliçe listesi
        public List<PoliceListesiOfflineRaporProcedureModel> PoliceListesiOfflineRaporGetir(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                           string sigortaSirketleriArray, string bransKoduArray,
                                                           string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {

            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = anaTVMKodu;

            byte MerkezAcenteSorgulaniyorMu = 0;
            if (subelerArray.Split(',').Contains(anaTVMKodu.ToString()))
            {
                MerkezAcenteSorgulaniyorMu = 1;
            }
            SqlParameter merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            merkez.Value = MerkezAcenteSorgulaniyorMu; 

            SqlParameter policeTarihTip = new SqlParameter("@policeTarihTipi", SqlDbType.TinyInt);
            policeTarihTip.Value = policeTarihi;

            SqlParameter baslama = new SqlParameter("@baslangicTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter subekodu = new SqlParameter("@taliTvmList", SqlDbType.NVarChar, int.MaxValue);
            subekodu.Value = subelerArray ?? "";

            SqlParameter SigortaSirketlerii = new SqlParameter("@sigortaSirketList", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketlerii.Value = sigortaSirketleriArray ?? "";

            SqlParameter branskodu = new SqlParameter("@bransList", SqlDbType.NVarChar, int.MaxValue);
            branskodu.Value = bransKoduArray ?? "";

            SqlParameter policeNo = new SqlParameter("@policeNo", SqlDbType.NVarChar, 200);
            policeNo.Value = String.IsNullOrEmpty(PoliceNo) ? "" : PoliceNo;

            SqlParameter odemeSekli = new SqlParameter("@odemeSekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemeTipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;

            SqlParameter DisUretimTaliTVMKodlari = new SqlParameter("@DisTaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            DisUretimTaliTVMKodlari.Value = uretimTvmlerArray;


            List<PoliceListesiOfflineRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<PoliceListesiOfflineRaporProcedureModel>
                                                                ("SP_PoliceListesiOffline @TVMKodu,@Merkez, @policeTarihTipi, @baslangicTarihi, @bitisTarihi, @taliTvmList, @sigortaSirketList ,@bransList, @policeNo, @odemeSekli, @odemeTipi, @DisTaliTVMKodlari"
                                                                , TVMKodu, merkez, policeTarihTip, baslama, bitis, subekodu, SigortaSirketlerii, branskodu, policeNo, odemeSekli, odemeTipi, DisUretimTaliTVMKodlari)
                                                                .ToList<PoliceListesiOfflineRaporProcedureModel>();

            return rapor;

        }


        /// <summary>
        /// reasuror poliçe listesi 
        /// </summary>
        /// <param name="tvmKodu"></param>
        /// <param name="policeTarihi"></param>
        /// <param name="baslangicTarihi"></param>
        /// <param name="bitisTarihi"></param>
        /// <param name="subelerArray"></param>
        /// <param name="sigortaSirketleriArray"></param>
        /// <param name="bransKoduArray"></param>
        /// <param name="PoliceNo"></param>
        /// <param name="OdemeSekli"></param>
        /// <param name="OdemeTipi"></param>
        /// <param name="anaTVMKodu"></param>
        /// <param name="uretimTvmlerArray"></param>
        /// <returns></returns>
        public List<ReasurorPoliceListesiProcedureModel> GetReasurorPoliceListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                           string sigortaSirketleriArray, string bransKoduArray,
                                                           string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {

            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = anaTVMKodu;

            byte MerkezAcenteSorgulaniyorMu = 0;
            if (subelerArray.Split(',').Contains(anaTVMKodu.ToString()))
            {
                MerkezAcenteSorgulaniyorMu = 1;
            }
            SqlParameter merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            merkez.Value = MerkezAcenteSorgulaniyorMu;

            SqlParameter policeTarihTip = new SqlParameter("@policeTarihTipi", SqlDbType.TinyInt);
            policeTarihTip.Value = policeTarihi;

            SqlParameter baslama = new SqlParameter("@baslangicTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter subekodu = new SqlParameter("@taliTvmList", SqlDbType.NVarChar, int.MaxValue);
            subekodu.Value = subelerArray ?? "";

            SqlParameter SigortaSirketlerii = new SqlParameter("@sigortaSirketList", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketlerii.Value = sigortaSirketleriArray ?? "";

            SqlParameter branskodu = new SqlParameter("@bransList", SqlDbType.NVarChar, int.MaxValue);
            branskodu.Value = bransKoduArray ?? "";

            SqlParameter policeNo = new SqlParameter("@policeNo", SqlDbType.NVarChar, 200);
            policeNo.Value = String.IsNullOrEmpty(PoliceNo) ? "" : PoliceNo;

            SqlParameter odemeSekli = new SqlParameter("@odemeSekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemeTipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;

            SqlParameter DisUretimTaliTVMKodlari = new SqlParameter("@DisTaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            DisUretimTaliTVMKodlari.Value = uretimTvmlerArray;


            List<ReasurorPoliceListesiProcedureModel> rapor = _dbContext.Database.SqlQuery<ReasurorPoliceListesiProcedureModel>
                                                                ("SP_ReasurorPoliceListesi @TVMKodu,@Merkez, @policeTarihTipi, @baslangicTarihi, @bitisTarihi, @taliTvmList, @sigortaSirketList ,@bransList, @policeNo, @odemeSekli, @odemeTipi, @DisTaliTVMKodlari"
                                                                , TVMKodu, merkez, policeTarihTip, baslama, bitis, subekodu, SigortaSirketlerii, branskodu, policeNo, odemeSekli, odemeTipi, DisUretimTaliTVMKodlari)
                                                                .ToList<ReasurorPoliceListesiProcedureModel>();

            return rapor;

        }
        /// <summary>
        /// reasuror Teklif Listesi
        /// </summary>
        /// <param name="tvmKodu"></param>
        /// <param name="policeTarihi"></param>
        /// <param name="baslangicTarihi"></param>
        /// <param name="bitisTarihi"></param>
        /// <param name="subelerArray"></param>
        /// <param name="sigortaSirketleriArray"></param>
        /// <param name="bransKoduArray"></param>
        /// <param name="PoliceNo"></param>
        /// <param name="OdemeSekli"></param>
        /// <param name="OdemeTipi"></param>
        /// <param name="anaTVMKodu"></param>
        /// <param name="uretimTvmlerArray"></param>
        /// <returns></returns>
        public List<ReasurorTeklifListesiProcedureModel> GetReasurorTeklifListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                   string sigortaSirketleriArray, string bransKoduArray,
                                                   string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {

            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = anaTVMKodu;

            byte MerkezAcenteSorgulaniyorMu = 0;
            if (subelerArray.Split(',').Contains(anaTVMKodu.ToString()))
            {
                MerkezAcenteSorgulaniyorMu = 1;
            }
            SqlParameter merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            merkez.Value = MerkezAcenteSorgulaniyorMu;

            SqlParameter policeTarihTip = new SqlParameter("@policeTarihTipi", SqlDbType.TinyInt);
            policeTarihTip.Value = policeTarihi;

            SqlParameter baslama = new SqlParameter("@baslangicTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter subekodu = new SqlParameter("@taliTvmList", SqlDbType.NVarChar, int.MaxValue);
            subekodu.Value = subelerArray ?? "";

            SqlParameter SigortaSirketlerii = new SqlParameter("@sigortaSirketList", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketlerii.Value = sigortaSirketleriArray ?? "";

            SqlParameter branskodu = new SqlParameter("@bransList", SqlDbType.NVarChar, int.MaxValue);
            branskodu.Value = bransKoduArray ?? "";

            SqlParameter policeNo = new SqlParameter("@policeNo", SqlDbType.NVarChar, 200);
            policeNo.Value = String.IsNullOrEmpty(PoliceNo) ? "" : PoliceNo;

            SqlParameter odemeSekli = new SqlParameter("@odemeSekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemeTipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;

            SqlParameter DisUretimTaliTVMKodlari = new SqlParameter("@DisTaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            DisUretimTaliTVMKodlari.Value = uretimTvmlerArray;


            List<ReasurorTeklifListesiProcedureModel> rapor = _dbContext.Database.SqlQuery<ReasurorTeklifListesiProcedureModel>
                                                                ("SP_ReasurorTeklifListesi @TVMKodu,@Merkez, @policeTarihTipi, @baslangicTarihi, @bitisTarihi, @taliTvmList, @sigortaSirketList ,@bransList, @policeNo, @odemeSekli, @odemeTipi, @DisTaliTVMKodlari"
                                                                , TVMKodu, merkez, policeTarihTip, baslama, bitis, subekodu, SigortaSirketlerii, branskodu, policeNo, odemeSekli, odemeTipi, DisUretimTaliTVMKodlari)
                                                                .ToList<ReasurorTeklifListesiProcedureModel>();

            return rapor;

        }
        // Kart listesi
        public List<KartListesiProcedureModel> KartListesiRaporuGetir(DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                                     byte? OdemeSekli, int anaTVMKodu, string ad, string soyad, string tckn, int durum)
        {
            if (!String.IsNullOrEmpty(ad))
            {
                ad = ad.Trim();
            }
            if (!String.IsNullOrEmpty(soyad))
            {
                soyad = soyad.Trim();
            }
            if (!String.IsNullOrEmpty(tckn))
            {
                tckn = tckn.Trim();
            }
            SqlParameter TVMKodu = new SqlParameter("@tvmKodu", SqlDbType.Int);
            TVMKodu.Value = anaTVMKodu;
            SqlParameter subeler = new SqlParameter("@subeler", SqlDbType.NVarChar, int.MaxValue);
            subeler.Value = subelerArray ?? "";

            SqlParameter baslama = new SqlParameter("@baslangicTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter odemeSekli = new SqlParameter("@odemeSekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? Convert.ToByte(0);

            SqlParameter adi = new SqlParameter("@unvanAd", SqlDbType.NVarChar, 150);
            adi.Value = ad != "" && ad != null ? ad.ToLower().Replace('ı', 'i'): "";

            SqlParameter soyadi = new SqlParameter("@soyadi", SqlDbType.NVarChar, 50);
            soyadi.Value =soyad!="" && soyad!=null? soyad.ToLower().Replace('ı', 'i') : "";

            SqlParameter tc = new SqlParameter("@Tckn", SqlDbType.NVarChar, 11);
            tc.Value = tckn ?? "";

            SqlParameter Durum = new SqlParameter("@durum", SqlDbType.Int);
            Durum.Value = durum;

            List<KartListesiProcedureModel> rapor = _dbContext.Database.SqlQuery<KartListesiProcedureModel>
                                                                ("SP_KartListesi @tvmKodu, @subeler, @baslangicTarihi, @bitisTarihi, @odemeSekli, @unvanAd, @soyadi, @Tckn, @durum"
                                                                , TVMKodu, subeler, baslama, bitis, odemeSekli, adi, soyadi, tc, Durum)
                                                                .ToList<KartListesiProcedureModel>();

            return rapor;

        }

        //====POLİÇE ÜRETİM DETAY RAPORU =====//
        public List<PoliceUretimRaporProcedureModel> PoliceUretimRapor_Getir(DateTime basTarihi, DateTime bitTarihi,
                                                                 string SigortaSirketleriArray, string bransKoduArray, string tvmlerArray, string uretimTvmlerArray, int anatvmKodu)
        {


            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = anatvmKodu;

            SqlParameter Merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            Merkez.Value = 0;
            //if (tvmlerArray.Contains(anatvmKodu.ToString()))
            //{
            //    Merkez.Value = 1;
            //}
            if (tvmlerArray.Split(',').Contains(anatvmKodu.ToString()))
            {
                Merkez.Value = 1;
            }

            SqlParameter basT = new SqlParameter("@BaslangicTar", SqlDbType.DateTime);
            basT.Value = basTarihi;

            SqlParameter bitT = new SqlParameter("@BitisTar", SqlDbType.DateTime);
            bitT.Value = bitTarihi;

            SqlParameter Branslar = new SqlParameter("@Branslar", SqlDbType.NVarChar, int.MaxValue);

            Branslar.Value = !String.IsNullOrEmpty(bransKoduArray) ? bransKoduArray : "";
            // Branslar.Value = bransKoduArray;

            SqlParameter SigortaSirketleri = new SqlParameter("@SigortaSirketleri", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketleri.Value = !String.IsNullOrEmpty(SigortaSirketleriArray) ? SigortaSirketleriArray : "";
            // SigortaSirketleri.Value = SigortaSirketleriArray;


            SqlParameter TaliTVMKodlari = new SqlParameter("@TaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            TaliTVMKodlari.Value = tvmlerArray;

            SqlParameter DisUretimTaliTVMKodlari = new SqlParameter("@DisTaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            DisUretimTaliTVMKodlari.Value = uretimTvmlerArray;
            //TaliTVMKodlari.Value = tvmlerArray;



            List<PoliceUretimRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<PoliceUretimRaporProcedureModel>
                                                                ("SP_PoliceUretimListesi  @TVMKodu,@Merkez,@BaslangicTar,@BitisTar,@Branslar,@SigortaSirketleri,@TaliTVMKodlari,@DisTaliTVMKodlari"
                                                                , TVMKodu, Merkez, basT, bitT, Branslar, SigortaSirketleri, TaliTVMKodlari, DisUretimTaliTVMKodlari)
                                                                .ToList<PoliceUretimRaporProcedureModel>();

            return rapor;
        }
        // ==== Poliçe Üretim İcmal Raporu Getirir ====//
        public List<PoliceUretimIcmalRaporProcedureModel> PoliceIcmalRaporGetir(int tvmKodu, byte Merkez, int ay, int yil, string SigortaSirketleriListe, string bransKoduListe, string tvmlerListe, string disKaynakListe, int raporTip)
        {
            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = tvmKodu;

            byte MerkezAcenteSorgulaniyorMu = 0;
            if (Merkez == 1)
            {
                var parts = tvmlerListe.Split(',');
                foreach (var item in parts)
                {
                    if (item == tvmKodu.ToString())
                    {
                        MerkezAcenteSorgulaniyorMu = 1;
                    }
                }
            }
            SqlParameter merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            merkez.Value = MerkezAcenteSorgulaniyorMu;
            SqlParameter donemAy = new SqlParameter("@DonemAy", SqlDbType.Int);
            donemAy.Value = ay;

            SqlParameter donemYil = new SqlParameter("@DonemYil", SqlDbType.Int);
            donemYil.Value = yil;

            SqlParameter Branslar = new SqlParameter("@Branslar", SqlDbType.NVarChar, int.MaxValue);

            Branslar.Value = !String.IsNullOrEmpty(bransKoduListe) ? bransKoduListe : "";

            SqlParameter SigortaSirketleri = new SqlParameter("@SigortaSirketleri", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketleri.Value = !String.IsNullOrEmpty(SigortaSirketleriListe) ? SigortaSirketleriListe : "";

            SqlParameter TaliTVMKodlari = new SqlParameter("@TaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            TaliTVMKodlari.Value = tvmlerListe;

            SqlParameter DisKaynakKodlari = new SqlParameter("@DisKaynaklar", SqlDbType.NVarChar, int.MaxValue);
            DisKaynakKodlari.Value = disKaynakListe;

            SqlParameter RaporTip = new SqlParameter("@RaporTip", SqlDbType.Int);
            RaporTip.Value = raporTip;

            List<PoliceUretimIcmalRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<PoliceUretimIcmalRaporProcedureModel>
                                                                ("SP_PoliceUretimIcmal  @TVMKodu,@Merkez,@DonemAy,@DonemYil,@Branslar,@SigortaSirketleri,@TaliTVMKodlari,@DisKaynaklar,@RaporTip"
                                                                , TVMKodu, merkez, donemAy, donemYil, Branslar, SigortaSirketleri, TaliTVMKodlari, DisKaynakKodlari, RaporTip)
                                                                .ToList<PoliceUretimIcmalRaporProcedureModel>();

            return rapor;
        }

        //====Policetahsilat raporu getirir

        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi,
            DateTime? BitisTarihi, string SigortaSirketleriArray, string TcVkn, string tvmlerListe, int RaporTipi)
        {
            byte MerkezAcenteSorgulaniyorMu = 0;
            if (MerkezAcenteMi == 1)
            {
                var parts = tvmlerListe.Split(',');
                foreach (var item in parts)
                {
                    if (item == MerkezAcenteKodu.ToString())
                    {
                        MerkezAcenteSorgulaniyorMu = 1;
                    }
                }
            }
            SqlParameter merkezAcenteMi = new SqlParameter("@MerkezAcenteMi", SqlDbType.Bit);
            merkezAcenteMi.Value = MerkezAcenteSorgulaniyorMu;

            SqlParameter merkezAcenteKodu = new SqlParameter("@MerkezAcenteKodu", SqlDbType.Int);
            merkezAcenteKodu.Value = MerkezAcenteKodu;

            SqlParameter baslangicTarih = new SqlParameter("@BaslangicTarihi", SqlDbType.DateTime);
            baslangicTarih.Value = BaslangicTarihi;

            SqlParameter bitisTarih = new SqlParameter("@BitisTarihi", SqlDbType.DateTime);
            bitisTarih.Value = BitisTarihi;

            SqlParameter SigortaSirketleri = new SqlParameter("@SigortaSirketleri", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketleri.Value = !String.IsNullOrEmpty(SigortaSirketleriArray) ? SigortaSirketleriArray : "";

            SqlParameter tcVkn = new SqlParameter("@TcVkn", SqlDbType.NVarChar, 11);
            if (String.IsNullOrEmpty(TcVkn))
            {
                tcVkn.Value = "";
            }
            else
            {
                tcVkn.Value = TcVkn;
            }
            SqlParameter TaliTVMKodlari = new SqlParameter("@TaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
            TaliTVMKodlari.Value = tvmlerListe;

            SqlParameter raporTip = new SqlParameter("@RaporTipi", SqlDbType.Int);
            raporTip.Value = RaporTipi;

            try
            {
                List<PoliceTahsilatRaporProcedureModel> tahsilatRapor = _dbContext.Database.SqlQuery<PoliceTahsilatRaporProcedureModel>
                                                               ("SP_Police_Tahsilat_Raporu @MerkezAcenteMi, @MerkezAcenteKodu, @BaslangicTarihi,@BitisTarihi,@SigortaSirketleri,@TcVkn,@TaliTVMKodlari, @RaporTipi"
                                                               , merkezAcenteMi, merkezAcenteKodu, baslangicTarih, bitisTarih, SigortaSirketleri, tcVkn, TaliTVMKodlari, raporTip).ToList<PoliceTahsilatRaporProcedureModel>();

                return tahsilatRapor;
            }
            catch (Exception ex)
            {
                throw new TimeoutException("Data yoğunluğundan kaynaklı hata", ex);

            }

        }

        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatMusteriGrupKoduRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string kimlikNo, int RaporTipi)

        {

            byte MerkezAcenteSorgulaniyorMu = 0;
            if (MerkezAcenteMi == 1)
            {
                //var parts = tvmlerListe.Split(',');
                //foreach (var item in parts)
                //{
                //    if (item == MerkezAcenteKodu.ToString())
                //    {
                //        MerkezAcenteSorgulaniyorMu = 1;
                //    }
                //}
            }
            SqlParameter merkezAcenteMi = new SqlParameter("@MerkezAcenteMi", SqlDbType.Bit);
            merkezAcenteMi.Value = MerkezAcenteSorgulaniyorMu;

            SqlParameter merkezAcenteKodu = new SqlParameter("@MerkezAcenteKodu", SqlDbType.Int);
            merkezAcenteKodu.Value = MerkezAcenteKodu;

            SqlParameter baslangicTarih = new SqlParameter("@BaslangicTarihi", SqlDbType.DateTime);
            baslangicTarih.Value = BaslangicTarihi;

            SqlParameter bitisTarih = new SqlParameter("@BitisTarihi", SqlDbType.DateTime);
            bitisTarih.Value = BitisTarihi;



            SqlParameter kimlikno = new SqlParameter("@kimlikNo", SqlDbType.NVarChar, 30);
            kimlikno.Value = kimlikNo;



            SqlParameter raporTip = new SqlParameter("@RaporTipi", SqlDbType.Int);
            raporTip.Value = RaporTipi;

            try
            {
                List<PoliceTahsilatRaporProcedureModel> tahsilatRapor = _dbContext.Database.SqlQuery<PoliceTahsilatRaporProcedureModel>
                                                               ("SP_Police_Tahsilat_Raporu_MusteriGrupKodu @MerkezAcenteMi, @MerkezAcenteKodu, @BaslangicTarihi,@BitisTarihi,@kimlikNo, @RaporTipi"
                                                               , merkezAcenteMi, merkezAcenteKodu, baslangicTarih, bitisTarih, kimlikno, raporTip).ToList<PoliceTahsilatRaporProcedureModel>();

                return tahsilatRapor;
            }
            catch (Exception ex)
            {
                throw new TimeoutException("Data yoğunluğundan kaynaklı hata", ex);

            }

        }

        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatDisKaynakRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string disKaynakListe, int RaporTipi)
        {
            byte MerkezAcenteSorgulaniyorMu = 0;
            if (MerkezAcenteMi == 1)
            {
                var parts = disKaynakListe.Split(',');
                foreach (var item in parts)
                {
                    if (item == MerkezAcenteKodu.ToString())
                    {
                        MerkezAcenteSorgulaniyorMu = 1;
                    }
                }
            }
            SqlParameter merkezAcenteMi = new SqlParameter("@MerkezAcenteMi", SqlDbType.Bit);
            merkezAcenteMi.Value = MerkezAcenteSorgulaniyorMu;

            SqlParameter merkezAcenteKodu = new SqlParameter("@MerkezAcenteKodu", SqlDbType.Int);
            merkezAcenteKodu.Value = MerkezAcenteKodu;

            SqlParameter baslangicTarih = new SqlParameter("@BaslangicTarihi", SqlDbType.DateTime);
            baslangicTarih.Value = BaslangicTarihi;

            SqlParameter bitisTarih = new SqlParameter("@BitisTarihi", SqlDbType.DateTime);
            bitisTarih.Value = BitisTarihi;

            SqlParameter disKaynakListesi = new SqlParameter("@disKaynakListesi", SqlDbType.NVarChar, int.MaxValue);
            disKaynakListesi.Value = disKaynakListe;

            SqlParameter raporTip = new SqlParameter("@RaporTipi", SqlDbType.Int);
            raporTip.Value = RaporTipi;

            try
            {
                List<PoliceTahsilatRaporProcedureModel> tahsilatRapor = _dbContext.Database.SqlQuery<PoliceTahsilatRaporProcedureModel>
                                                               ("SP_Police_Tahsilat_Raporu_Dis_Kaynak @MerkezAcenteMi, @MerkezAcenteKodu, @BaslangicTarihi,@BitisTarihi,@DisKaynakListesi, @RaporTipi"
                                                               , merkezAcenteMi, merkezAcenteKodu, baslangicTarih, bitisTarih, disKaynakListesi, raporTip).ToList<PoliceTahsilatRaporProcedureModel>();

                return tahsilatRapor;
            }
            catch (Exception ex)
            {
                throw new TimeoutException("Data yoğunluğundan kaynaklı hata", ex);

            }
        }
        public List<PoliceUretimRaporProcedureModelTali> PoliceUretimRaporTali_Getir(int ay, int yil,
                                                                string SigortaSirketleriArray, string bransKoduArray, string tvmlerArray, int anatvmKodu)
        {


            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = anatvmKodu;

            SqlParameter Merkez = new SqlParameter("@Merkez", SqlDbType.Bit);
            Merkez.Value = 0;
            if (tvmlerArray.Contains(anatvmKodu.ToString()))
            {
                Merkez.Value = 1;
            }

            SqlParameter DonemAy = new SqlParameter("@DonemAy", SqlDbType.Int);
            DonemAy.Value = ay;

            SqlParameter DonemYil = new SqlParameter("@DonemYil", SqlDbType.Int);
            DonemYil.Value = yil;

            SqlParameter Branslar = new SqlParameter("@Branslar", SqlDbType.NVarChar, 200);

            Branslar.Value = !String.IsNullOrEmpty(bransKoduArray) ? bransKoduArray : "";

            SqlParameter SigortaSirketleri = new SqlParameter("@SigortaSirketleri", SqlDbType.NVarChar, 200);
            SigortaSirketleri.Value = !String.IsNullOrEmpty(SigortaSirketleriArray) ? SigortaSirketleriArray : "";


            SqlParameter TaliTVMKodlari = new SqlParameter("@TaliTVMKodlari", SqlDbType.NVarChar, 200);
            TaliTVMKodlari.Value = tvmlerArray;



            List<PoliceUretimRaporProcedureModelTali> rapor = _dbContext.Database.SqlQuery<PoliceUretimRaporProcedureModelTali>
                                                                ("SP_PoliceUretimListesi  @TVMKodu,@Merkez,@DonemAy,@DonemYil,@Branslar,@SigortaSirketleri,@TaliTVMKodlari"
                                                                , TVMKodu, Merkez, DonemAy, DonemYil, Branslar, SigortaSirketleri, TaliTVMKodlari)
                                                                .ToList<PoliceUretimRaporProcedureModelTali>();

            return rapor;
        }

        public List<PoliceHedefRaporModel> PoliceHedefRaporGerceklesen(int TvmKod, int taliKodu, int yil)
        {

            SqlParameter TVMKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            TVMKodu.Value = TvmKod;

            SqlParameter TaliTVMKodu = new SqlParameter("@TaliTVMKodu", SqlDbType.Int);
            TaliTVMKodu.Value = taliKodu;

            SqlParameter DonemYil = new SqlParameter("@DonemYil", SqlDbType.Int);
            DonemYil.Value = yil;

            List<PoliceHedefRaporModel> rapor = _dbContext.Database.SqlQuery<PoliceHedefRaporModel>
                                                                ("SP_PoliceUretimHedefGerceklesenListesi  @TVMKodu,@TaliTVMKodu,@DonemYil", TVMKodu, TaliTVMKodu, DonemYil)
                                                                .ToList<PoliceHedefRaporModel>();

            return rapor;
        }


        // ==== Vade Takip Raporu Getirir ====//
        public List<VadeTakipRaporuProcedureModel> SP_VadeTakipRaporu_Deneme(DateTime? baslangicTarihi, DateTime? bitisTarihi, string urunKoduArray, string bransKoduArray,
                                                                          string subelerArray, byte? TarihAraligi)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslamaT = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslamaT.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            //SqlParameter urunkodu = new SqlParameter("@urunkodu", SqlDbType.NVarChar, int.MaxValue);
            //urunkodu.Value = urunKoduArray;

            SqlParameter branskodu = new SqlParameter("@branskodu", SqlDbType.NVarChar, int.MaxValue);
            string bransKodlari = "";
            if (!String.IsNullOrEmpty(bransKoduArray))
            {
                string[] parts = bransKoduArray.Split(',');
                foreach (var item in parts)
                {
                    if (item != "multiselect-all")
                    {
                        bransKodlari += item + ",";
                    }
                }
            }

            branskodu.Value = bransKodlari;

            SqlParameter subekodu = new SqlParameter("@subeler", SqlDbType.NVarChar, int.MaxValue);
            string subeKodlari = "";
            if (!String.IsNullOrEmpty(subelerArray))
            {
                string[] parts = subelerArray.Split(',');
                foreach (var item in parts)
                {
                    if (item != "multiselect-all")
                    {
                        subeKodlari += item + ",";
                    }
                }
            }
            subekodu.Value = subeKodlari;


            List<VadeTakipRaporuProcedureModel> rapor = _dbContext.Database.SqlQuery<VadeTakipRaporuProcedureModel>
                                                                ("SP_VadeTakipRaporu_Deneme @baslamatarihi, @bitistarihi, @branskodu,@subeler", baslamaT, bitis, branskodu, subekodu)
                                                                 .ToList<VadeTakipRaporuProcedureModel>();

            return rapor;
        }

        // ==== Kredili Hayat Poliçe Raporu Getirir ====//
        public List<KrediliHayatPoliceRaporProcedureModel> KrediliHayatPoliceRapor_Getir(byte TarihTipi, int? Doviztl, DateTime? baslangicTarihi,
                                                                                DateTime? bitisTarihi, string subelerArray, string PoliceNo, byte? OdemeSekli, byte? OdemeTipi)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tarihtipi = new SqlParameter("@tarihtipi", SqlDbType.Int);
            tarihtipi.Value = TarihTipi;

            SqlParameter doviztl = new SqlParameter("@doviztl", SqlDbType.Int);
            doviztl.Value = Doviztl.HasValue ? Doviztl.Value : 0;

            SqlParameter subekodu = new SqlParameter("@subeler", SqlDbType.NVarChar, 200);
            subekodu.Value = subelerArray;

            SqlParameter policeNo = new SqlParameter("@policeNo", SqlDbType.NVarChar, 200);
            policeNo.Value = PoliceNo;

            SqlParameter odemeSekli = new SqlParameter("@odemesekli", SqlDbType.TinyInt);
            odemeSekli.Value = OdemeSekli ?? 0;

            SqlParameter odemeTipi = new SqlParameter("@odemetipi", SqlDbType.TinyInt);
            odemeTipi.Value = OdemeTipi ?? 0;



            List<KrediliHayatPoliceRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<KrediliHayatPoliceRaporProcedureModel>
                                                                ("SP_KrediliHayatPoliceRapor_Test @tarihtipi, @baslamatarihi, @bitistarihi, @subeler," +
                                                                "@doviztl,@policeNo,@odemesekli,@odemetipi"
                                                                , tarihtipi, baslama, bitis, subekodu, doviztl, policeNo, odemeSekli, odemeTipi)
                                                                .ToList<KrediliHayatPoliceRaporProcedureModel>();

            return rapor;
        }


        // ==== Teklif Raporu Getirir ====//
        public List<TeklifRaporuProcedureModel> TeklifRaporu_Getir(byte TarihTipi, DateTime? baslangicTarihi, DateTime? bitisTarihi,
                                                            string subelerArray, string Brans, string Urun, int? DovizTl, int? TeklifNo)
        {

            int dilkodu = 1;
            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            switch (lang)
            {
                case "en": dilkodu = 2; break;
                case "tr": dilkodu = 1; break;
            }


            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter police = new SqlParameter("@policetarihi", SqlDbType.Int);
            police.Value = TarihTipi;

            SqlParameter subekodu = new SqlParameter("@subeler", SqlDbType.NVarChar, 200);
            subekodu.Value = subelerArray;

            SqlParameter urun = new SqlParameter("@urunkodu", SqlDbType.NVarChar, 200);
            urun.Value = Urun;

            SqlParameter brans = new SqlParameter("@branskodu ", SqlDbType.NVarChar, 200);
            brans.Value = Brans;

            SqlParameter dilid = new SqlParameter("@dilid", SqlDbType.Int);
            dilid.Value = dilkodu;

            SqlParameter doviztl = new SqlParameter("@doviztl", SqlDbType.Int);
            doviztl.Value = DovizTl ?? 0;

            SqlParameter teklifNo = new SqlParameter("@teklifNo", SqlDbType.Int);
            teklifNo.Value = TeklifNo ?? 0;

            List<TeklifRaporuProcedureModel> tRapor = _dbContext.Database.SqlQuery<TeklifRaporuProcedureModel>
                                                     ("SP_TeklifRaporu_Test @policetarihi, @baslamatarihi, @bitistarihi, @subeler" +
                                                     ",@urunkodu,@branskodu,@doviztl,@dilid,@teklifNo",
                                                      police, baslama, bitis, subekodu, urun, brans, doviztl, dilid, teklifNo)
                                                     .ToList<TeklifRaporuProcedureModel>();

            return tRapor;
        }

        // ==== Teklif Ara Stored Procedure e cevrildi====//
        public List<TeklifAraProcedureModel> TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu, int? hazirlayankullanici, int? tumKodu,
                                                             int? teklifDurumu, int aktifTVMKoduKodu, int Skip, int Take)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);


            SqlParameter teklifNo = new SqlParameter("@TeklifNo", SqlDbType.Int);
            teklifNo.Value = TeklifNo ?? 0;

            SqlParameter tumKod = new SqlParameter("@TUMKodu", SqlDbType.Int);
            tumKod.Value = tumKodu ?? 0;


            SqlParameter baslama = new SqlParameter("@BaslamaTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@BitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tvmKod = new SqlParameter("@TVMKodu", SqlDbType.Int);
            tvmKod.Value = tvmKodu ?? 0;

            SqlParameter musteriKod = new SqlParameter("@MusteriKodu", SqlDbType.Int);
            musteriKod.Value = musteriKodu ?? 0;

            SqlParameter urun = new SqlParameter("@UrunKodu ", SqlDbType.Int);
            urun.Value = urunKodu;

            SqlParameter hazirlayan = new SqlParameter("@HazirlayanKodu", SqlDbType.Int);
            hazirlayan.Value = hazirlayankullanici;

            SqlParameter teklifDurum = new SqlParameter("@TeklifDurumu", SqlDbType.Int);
            teklifDurum.Value = teklifDurumu ?? 0;

            SqlParameter aktifTVMKod = new SqlParameter("@AktifKullaniciTVMKodu", SqlDbType.NVarChar, 35);
            aktifTVMKod.Value = aktifTVMKoduKodu;

            SqlParameter skip = new SqlParameter("@Skip", SqlDbType.Int);
            skip.Value = Skip;

            SqlParameter take = new SqlParameter("@Take", SqlDbType.Int);
            take.Value = Take;

            List<TeklifAraProcedureModel> tRapor = _dbContext.Database.SqlQuery<TeklifAraProcedureModel>
                                                     ("SP_KarsilastirmaliTeklifAra @BaslamaTarihi, @BitisTarihi, @TeklifNo, @TVMKodu" +
                                                     ",@MusteriKodu,@UrunKodu,@HazirlayanKodu,@TUMKodu,@TeklifDurumu, @AktifKullaniciTVMKodu, @Skip, @Take",
                                                      baslama, bitis, teklifNo, tvmKod, musteriKod, urun, hazirlayan, tumKod, teklifDurum, aktifTVMKod, skip, take)
                                                     .ToList<TeklifAraProcedureModel>();


            return tRapor;

        }
        // ---Aegon Teklif Ara Stored Procedure çevrildi
        public List<AegonTeklifAraProcedureModel> Aegon_TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu, int? hazirlayankullanici,
                                                                   int AktifTVMKodu, int Skip, int Take)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);


            SqlParameter teklifNo = new SqlParameter("@TeklifNo", SqlDbType.Int);
            teklifNo.Value = TeklifNo ?? 0;

            SqlParameter baslama = new SqlParameter("@BaslamaTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@BitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tvmKod = new SqlParameter("@TVMKodu", SqlDbType.Int);
            tvmKod.Value = tvmKodu ?? 0;

            SqlParameter musteriKod = new SqlParameter("@MusteriKodu", SqlDbType.Int);
            musteriKod.Value = musteriKodu ?? 0;

            SqlParameter urun = new SqlParameter("@UrunKodu ", SqlDbType.Int);
            urun.Value = urunKodu;

            SqlParameter hazirlayan = new SqlParameter("@HazirlayanKodu", SqlDbType.Int);
            hazirlayan.Value = hazirlayankullanici;

            SqlParameter aktifTVMKod = new SqlParameter("@AktifKullaniciTVMKodu", SqlDbType.Int);
            aktifTVMKod.Value = AktifTVMKodu;

            SqlParameter skip = new SqlParameter("@Skip", SqlDbType.Int);
            skip.Value = Skip;

            SqlParameter take = new SqlParameter("@Take", SqlDbType.Int);
            take.Value = Take;

            List<AegonTeklifAraProcedureModel> tRapor = _dbContext.Database.SqlQuery<AegonTeklifAraProcedureModel>
                                                     ("SP_AegonTeklifAra @BaslamaTarihi, @BitisTarihi, @TeklifNo, @TVMKodu" +
                                                     ",@MusteriKodu,@UrunKodu,@HazirlayanKodu,@AktifKullaniciTVMKodu, @Skip, @Take",
                                                      baslama, bitis, teklifNo, tvmKod, musteriKod, urun, hazirlayan, aktifTVMKod, skip, take)
                                                     .ToList<AegonTeklifAraProcedureModel>();


            return tRapor;

        }
        // ---Mapfre Teklif Ara Stored Procedure çevrildi
        public List<MapfreTeklifAraProcedureModel> Mapfre_TeklifAra_Getir(DateTime? baslangicTarihi, DateTime? bitisTarihi, int? TeklifNo, string TumTeklifNo, int? tvmKodu, int? musteriKodu, int? urunKodu,
                                                                          int? hazirlayankullanici, int? teklifDurumu, int AktifKullaniciKodu, int Skip, int Take)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);


            SqlParameter teklifNo = new SqlParameter("@TeklifNo", SqlDbType.Int);
            teklifNo.Value = TeklifNo ?? 0;

            SqlParameter tumteklifno = new SqlParameter("@TUMTeklifNo", SqlDbType.NVarChar, 20);
            tumteklifno.Value = TumTeklifNo;

            SqlParameter baslama = new SqlParameter("@BaslamaTarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@BitisTarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tvmKod = new SqlParameter("@TVMKodu", SqlDbType.Int);
            tvmKod.Value = tvmKodu ?? 0;

            SqlParameter musteriKod = new SqlParameter("@MusteriKodu", SqlDbType.Int);
            musteriKod.Value = musteriKodu ?? 0;

            SqlParameter urun = new SqlParameter("@UrunKodu ", SqlDbType.Int);
            urun.Value = urunKodu;

            SqlParameter hazirlayan = new SqlParameter("@HazirlayanKodu", SqlDbType.Int);
            hazirlayan.Value = hazirlayankullanici;

            SqlParameter teklifDurum = new SqlParameter("@TeklifDurumKodu", SqlDbType.Int);
            teklifDurum.Value = teklifDurumu ?? 0;

            SqlParameter aktifKullaniciTVMKod = new SqlParameter("@AktifKullaniciTVMKodu", SqlDbType.Int);
            aktifKullaniciTVMKod.Value = AktifKullaniciKodu;


            SqlParameter skip = new SqlParameter("@Skip", SqlDbType.Int);
            skip.Value = Skip;

            SqlParameter take = new SqlParameter("@Take", SqlDbType.Int);
            take.Value = Take;


            List<MapfreTeklifAraProcedureModel> tRapor = _dbContext.Database.SqlQuery<MapfreTeklifAraProcedureModel>
                                                     ("SP_MapfreTeklifAra @BaslamaTarihi, @BitisTarihi, @TeklifNo,@TUMTeklifNo, @TVMKodu" +
                                                     ",@MusteriKodu,@UrunKodu,@HazirlayanKodu,@TeklifDurumKodu,@AktifKullaniciTVMKodu, @Skip, @Take",
                                                      baslama, bitis, teklifNo, tumteklifno, tvmKod, musteriKod, urun, hazirlayan, teklifDurum, aktifKullaniciTVMKod, skip, take)
                                                     .ToList<MapfreTeklifAraProcedureModel>();


            return tRapor;
        }


        // ==== Ozet Raporu Getirir ====//
        public List<OzetRaporProcedureModel> OzetRapor_Getir(int TarihTipi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string urunKoduArray, string bransKoduArray, int? Doviztl, int? AramaKriteri, int? Data, int? Data2, string Data3, int TvmKodu, int YetkiKodu)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tarihtipi = new SqlParameter("@tarihtipi", SqlDbType.Int);
            tarihtipi.Value = TarihTipi;

            SqlParameter doviztl = new SqlParameter("@doviztl", SqlDbType.Int);
            doviztl.Value = Doviztl.HasValue ? Doviztl.Value : 0;

            SqlParameter aramakriteri = new SqlParameter("@aramakriteri", SqlDbType.Int);
            aramakriteri.Value = AramaKriteri ?? 0;

            SqlParameter data = new SqlParameter("@data", SqlDbType.Int);
            data.Value = Data ?? 0;

            SqlParameter data2 = new SqlParameter("@data2", SqlDbType.Int);
            data2.Value = Data2 ?? 0;

            SqlParameter data3 = new SqlParameter("@data3", SqlDbType.NVarChar, 100);
            data3.Value = Data3;

            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = TvmKodu;

            SqlParameter yetkikodu = new SqlParameter("@yetkikodu", SqlDbType.Int);
            yetkikodu.Value = YetkiKodu;

            //SP_OzetRaporTest olarak değiştirilecek
            List<OzetRaporProcedureModel> rapor = _dbContext.Database.SqlQuery<OzetRaporProcedureModel>
                                        ("SP_OzetRaporTest @tarihtipi, @baslamatarihi, @bitistarihi, @doviztl, @aramakriteri, @data, @data2, @data3,@tvmkodu,@yetkikodu"
                                        , tarihtipi, baslama, bitis, doviztl, aramakriteri, data, data2, data3, tvmkodu, yetkikodu)
                                        .ToList<OzetRaporProcedureModel>();

            return rapor;
        }

        // ==== Arac Sigortalari İstatistikRaporu Getirir ====//
        public List<AracSigortalariIstatistikRaporuProcedureModel> AracSigortalariIstatistikRaporu_Getir(int? TvmKodu, int? TarihTipi, DateTime? baslangicTarihi,
                                                            DateTime? bitisTarihi, int? urun, int? Doviztl, int? AramaKriteri, int? Kodu, int? Data2, string SorguTuru)
        {
            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            SqlParameter baslama = new SqlParameter("@baslamatarihi", SqlDbType.Date);
            baslama.Value = baslangicTarihi;

            SqlParameter bitis = new SqlParameter("@bitistarihi", SqlDbType.Date);
            bitis.Value = bitisTarihi;

            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = TvmKodu ?? 0;

            SqlParameter tarihtipi = new SqlParameter("@tarihtipi", SqlDbType.Int);
            tarihtipi.Value = TarihTipi ?? 0;

            SqlParameter doviztl = new SqlParameter("@doviztl", SqlDbType.Int);
            doviztl.Value = Doviztl ?? 0;

            SqlParameter aramakriteri = new SqlParameter("@aramakriteri", SqlDbType.Int);
            aramakriteri.Value = AramaKriteri ?? 0;

            SqlParameter kodu = new SqlParameter("@kodu", SqlDbType.Int);
            kodu.Value = Kodu ?? 0;

            SqlParameter data2 = new SqlParameter("@data2", SqlDbType.Int);
            data2.Value = Data2 ?? 0;

            SqlParameter sorguturu = new SqlParameter("@sorguturu", SqlDbType.NVarChar, 100);
            sorguturu.Value = SorguTuru;

            SqlParameter urunKodu = new SqlParameter("@urunkodu", SqlDbType.Int);
            urunKodu.Value = urun ?? 0;

            List<AracSigortalariIstatistikRaporuProcedureModel> rapor = _dbContext.Database.SqlQuery<AracSigortalariIstatistikRaporuProcedureModel>
                                                                ("SP_AracSigortasiIstatistik @tvmkodu, @tarihtipi, @baslamatarihi, @bitistarihi, @doviztl," +
                                                                               "@urunkodu, @aramakriteri, @kodu, @data2, @sorguturu"
                                                                , tvmkodu, tarihtipi, baslama, bitis, doviztl, urunKodu, aramakriteri, kodu, data2, sorguturu)
                                                                .ToList<AracSigortalariIstatistikRaporuProcedureModel>();

            return rapor;
        }

        //Genel Rapor
        public GenelRaporProcedureModel GenelRapor_Getir(int? TVMKodu, string ProjeKodu)
        {
            SqlParameter tvmkodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
            tvmkodu.Value = TVMKodu ?? 0;

            SqlParameter projeKodu = new SqlParameter("@ProjeKodu", SqlDbType.VarChar, 100);
            projeKodu.Value = ProjeKodu;

            GenelRaporProcedureModel rapor = _dbContext.Database.SqlQuery<GenelRaporProcedureModel>
                                                                ("SP_GenelRapor @TVMKodu, @ProjeKodu"
                                                                , tvmkodu, projeKodu).FirstOrDefault();
            return rapor;
        }
        #endregion

        #region Aegon Rapor Procedure

        // ==== AEGON Teklif Raporu Getirir ====//
        public List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporu_Getir(int tarihtipi,
                                                                            DateTime? baslangicTarihi,
                                                                            DateTime? bitisTarihi,
                                                                            string subeler,
                                                                            string urunler,
                                                                            int parabirimi,
                                                                            int? teklifno,
                                                                            int? yillikprimmin,
                                                                            int? yillikprimmax,
                                                                            int skip,
                                                                            int take)
        {

            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-20);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now.AddYears(+20);

            if (bitisTarihi.HasValue)
                bitisTarihi.Value.AddDays(1);

            int dilkodu = 1;
            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            switch (lang)
            {
                case "en": dilkodu = 2; break;
                case "tr": dilkodu = 1; break;
            }


            SqlParameter TarihTipi = new SqlParameter("@TarihTipi", SqlDbType.Int);
            TarihTipi.Value = tarihtipi;

            SqlParameter BaslamaTarihi = new SqlParameter("@BaslamaTarihi", SqlDbType.Date);
            BaslamaTarihi.Value = baslangicTarihi;

            SqlParameter BitisTarihi = new SqlParameter("@BitisTarihi", SqlDbType.Date);
            BitisTarihi.Value = bitisTarihi;

            SqlParameter Subeler = new SqlParameter("@Subeler", SqlDbType.NVarChar, 4000);
            Subeler.Value = subeler;

            SqlParameter Urunler = new SqlParameter("@Urunler", SqlDbType.NVarChar, 4000);
            Urunler.Value = urunler;

            SqlParameter DilId = new SqlParameter("@DilId", SqlDbType.Int);
            DilId.Value = dilkodu;

            SqlParameter TeklifNo = new SqlParameter("@TeklifNo", SqlDbType.Int);
            TeklifNo.Value = teklifno ?? 0;

            SqlParameter ParaBirimi = new SqlParameter("@ParaBirimi", SqlDbType.Int);
            ParaBirimi.Value = parabirimi;

            SqlParameter YillikPrimMin = new SqlParameter("@YillikPrimMin", SqlDbType.Int);
            YillikPrimMin.Value = yillikprimmin ?? 0;

            SqlParameter YillikPrimMax = new SqlParameter("@YillikPrimMax", SqlDbType.Int);
            YillikPrimMax.Value = yillikprimmax ?? 0;

            SqlParameter Skip = new SqlParameter("@Skip", SqlDbType.Int);
            Skip.Value = skip;

            SqlParameter Take = new SqlParameter("@Take", SqlDbType.Int);
            Take.Value = take;


            List<AegonTeklifRaporuProcedureModel> tRapor = _dbContext.Database.SqlQuery<AegonTeklifRaporuProcedureModel>
                                            ("SP_AegonTeklifRaporu @TarihTipi ,@BaslamaTarihi ,@BitisTarihi ,@Subeler,@Urunler," +
                                            "@DilId,@TeklifNo ,@ParaBirimi,@YillikPrimMin ,@YillikPrimMax,@Skip,@Take",
                                            TarihTipi, BaslamaTarihi, BitisTarihi, Subeler, Urunler, DilId, TeklifNo, ParaBirimi, YillikPrimMin, YillikPrimMax, Skip, Take)
                                            .ToList<AegonTeklifRaporuProcedureModel>();

            return tRapor;
        }

        #endregion      

        private TUMBankaHesaplariRepository _tUMBankaHesaplariRepository;
        public TUMBankaHesaplariRepository TUMBankaHesaplariRepository
        {
            get
            {
                if (_tUMBankaHesaplariRepository == null)
                {
                    _tUMBankaHesaplariRepository = new TUMBankaHesaplariRepository(_dbContext);
                }
                return _tUMBankaHesaplariRepository;
            }
        }

        private TUMDetayRepository _tUMDetayRepository;
        public TUMDetayRepository TUMDetayRepository
        {
            get
            {
                if (_tUMDetayRepository == null)
                {
                    _tUMDetayRepository = new TUMDetayRepository(_dbContext);
                }
                return _tUMDetayRepository;
            }
        }

        private TUMDokumanlarRepository _tUMDokumanlarRepository;
        public TUMDokumanlarRepository TUMDokumanlarRepository
        {
            get
            {
                if (_tUMDokumanlarRepository == null)
                {
                    _tUMDokumanlarRepository = new TUMDokumanlarRepository(_dbContext);
                }
                return _tUMDokumanlarRepository;
            }
        }

        private TUMDurumTarihcesiRepository _tUMDurumTarihcesiRepository;
        public TUMDurumTarihcesiRepository TUMDurumTarihcesiRepository
        {
            get
            {
                if (_tUMDurumTarihcesiRepository == null)
                {
                    _tUMDurumTarihcesiRepository = new TUMDurumTarihcesiRepository(_dbContext);
                }
                return _tUMDurumTarihcesiRepository;
            }
        }

        private TUMIletisimYetkilileriRepository _tUMIletisimYetkilileriRepository;
        public TUMIletisimYetkilileriRepository TUMIletisimYetkilileriRepository
        {
            get
            {
                if (_tUMIletisimYetkilileriRepository == null)
                {
                    _tUMIletisimYetkilileriRepository = new TUMIletisimYetkilileriRepository(_dbContext);
                }
                return _tUMIletisimYetkilileriRepository;
            }
        }

        private TUMIPBaglantiRepository _tUMIPBaglantiRepository;
        public TUMIPBaglantiRepository TUMIPBaglantiRepository
        {
            get
            {
                if (_tUMIPBaglantiRepository == null)
                {
                    _tUMIPBaglantiRepository = new TUMIPBaglantiRepository(_dbContext);
                }
                return _tUMIPBaglantiRepository;
            }
        }

        private TUMNotlarRepository _tUMNotlarRepository;
        public TUMNotlarRepository TUMNotlarRepository
        {
            get
            {
                if (_tUMNotlarRepository == null)
                {
                    _tUMNotlarRepository = new TUMNotlarRepository(_dbContext);
                }
                return _tUMNotlarRepository;
            }
        }

        private TUMUrunleriRepository _tUMUrunleriRepository;
        public TUMUrunleriRepository TUMUrunleriRepository
        {
            get
            {
                if (_tUMUrunleriRepository == null)
                {
                    _tUMUrunleriRepository = new TUMUrunleriRepository(_dbContext);
                }
                return _tUMUrunleriRepository;
            }
        }
    }
}

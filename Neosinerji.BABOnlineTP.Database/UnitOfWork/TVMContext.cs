using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Data.Entity.Infrastructure;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface ITVMContext : IUnitOfWork
    {
        List<KullaniciYetkiModel> KullaniciYetkileri_Getir(int yetkiGrupKodu);
        List<DuyuruProcedureModel> GetDuyuru(int tvmKodu);
        List<TVMUrunYetkileriProcedureModel> GetTVMUrunYetkileri(int tvmKodu);
        Performans Performansim(int tvmKodu, int kullaniciKodu, DateTime baslamatarihi, DateTime bitistarihi);
        OfflineUretimPerformans OfflineUretim(int tvmKodu, int donemYil, string taliTvmKodlari);
        OfflineUretimPerformansKullanici OfflineUretimKullanici(int tvmKodu, int donemYil, string kullaniciKodu);
        TVMAcentelikleriRepository TVMAcentelikleriRepository { get; }
        TVMBankaHesaplariRepository TVMBankaHesaplariRepository { get; }
        TVMBolgeleriRepository TVMBolgeleriRepository { get; }
        TVMDepartmanlarRepository TVMDepartmanlarRepository { get; }
        TVMDetayRepository TVMDetayRepository { get; }
        ParaBirimleriRepository ParaBirimleriRepository { get; }
        TVMDokumanlarRepository TVMDokumanlarRepository { get; }
        TVMDurumTarihcesiRepository TVMDurumTarihcesiRepository { get; }
        TVMIletisimYetkilileriRepository TVMIletisimYetkilileriRepository { get; }
        TVMIPBaglantiRepository TVMIPBaglantiRepository { get; }
        TVMKullaniciAtamaRepository TVMKullaniciAtamaRepository { get; }
        TVMKullaniciDurumTarihcesiRepository TVMKullaniciDurumTarihcesiRepository { get; }
        TVMKullanicilarRepository TVMKullanicilarRepository { get; }
        TVMKullaniciSifreTarihcesiRepository TVMKullaniciSifreTarihcesiRepository { get; }
        TVMKullaniciSifremiUnuttumRepository TVMKullaniciSifremiUnuttumRepository { get; }
        TVMNotlarRepository TVMNotlarRepository { get; }
        TVMUrunYetkileriRepository TVMUrunYetkileriRepository { get; }
        TVMYetkiGruplariRepository TVMYetkiGruplariRepository { get; }
        TVMYetkiGrupYetkileriRepository TVMYetkiGrupYetkileriRepository { get; }
        TVMWebServisKullanicilariRepository TVMWebServisKullanicilariRepository { get; }
        TVMKullaniciNotlarRepository TVMKullaniciNotlarRepository { get; }
        DuyurularRepository DuyurularRepository { get; }
        DuyuruTVMRepository DuyuruTVMRepository { get; }
        DuyuruDokumanRepository DuyuruDokumanRepository { get; }
        WEBServisLogRepository WEBServisLogRepository { get; }
        MapfreKullaniciRepository MapfreKullaniciRepository { get; }
        KesintiTurleriRepository KesintiTurleriRepository { get; }
        KesintilerRepository KesintilerRepository { get; }
        DokumanTurleriRepository DokumanTurleriRepository { get; }
        TVMSMSKullaniciBilgiRepository TVMSMSKullaniciBilgiRepository { get; }
        NeoConnectLogRepository NeoConnectLogRepository { get; }
        void spMapfreKullanici(int mapfeKullaniciId, string sifre);
        bool KokpitGuncelle(int tvmKodu, int donemYil, string taliTvmler, string branslar);
        bool KokpitKullaniciGuncelle(int tvmKodu, int donemYil, string kullanicikodu, string branslar);
        TVMYetkiGrupSablonuRepository TVMYetkiGrupSablonuRepository { get; }

    }

    public class TVMContext : ITVMContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public TVMContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
        }

        #region IUnitOfWork
        public void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                foreach (var item in ex.Entries)
                {
                    var ad = item.OriginalValues.PropertyNames;

                }
                throw;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dex)
            {
                foreach (var item in dex.EntityValidationErrors)
                {
                    foreach (var item1 in item.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Validation for {0} : {1}", item1.PropertyName, item1.ErrorMessage);
                    }
                }
                throw;
            }

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

        private TVMAcentelikleriRepository _tVMAcentelikleriRepository;
        private TVMBankaHesaplariRepository _tVMBankaHesaplariRepository;
        private TVMBolgeleriRepository _tVMBolgeleriRepository;
        private TVMDepartmanlarRepository _tVMDepartmanlarRepository;
        private TVMDetayRepository _tvmDetayRepository;
        private ParaBirimleriRepository _paraBirimleriRepository;
        private TVMDokumanlarRepository _tVMDokumanlarRepository;
        private TVMDurumTarihcesiRepository _tVMDurumTarihcesiRepository;
        private TVMIletisimYetkilileriRepository _tVMIletisimYetkilileriRepository;
        private TVMIPBaglantiRepository _tVMIPBaglantiRepository;
        private TVMKullaniciAtamaRepository _tVMKullaniciAtamaRepository;
        private TVMKullaniciDurumTarihcesiRepository _tVMKullaniciDurumTarihcesiRepository;
        private TVMKullanicilarRepository _tVMKullanicilarRepository;
        private TVMKullaniciSifreTarihcesiRepository _tVMKullaniciSifreTarihcesiRepository;
        private TVMKullaniciSifremiUnuttumRepository _tVMKullaniciSifremiUnuttumRepository;
        private TVMNotlarRepository _tVMNotlarRepository;
        private TVMUrunYetkileriRepository _tVMUrunYetkileriRepository;
        private TVMYetkiGruplariRepository _tVMYetkiGruplariRepository;
        private TVMYetkiGrupYetkileriRepository _tVMYetkiGrupYetkileriRepository;
        private WEBServisLogRepository _webservisLogRepository;
        private TVMWebServisKullanicilariRepository _tVMWebServisKullanicilariRepository;

        private TVMKullaniciNotlarRepository _tVMKullaniciNotlarRepository;
        private DuyurularRepository _duyurularRepository;
        private DuyuruTVMRepository _duyuruTVMRepository;
        private DuyuruDokumanRepository _duyuruDokumanRepository;

        private MapfreKullaniciRepository _mapfreKullaniciRepository;

        private KesintiTurleriRepository _kesintiTurleriRepository;
        private KesintilerRepository _kesintilerRepository;
        private DokumanTurleriRepository _dokumanTurleriRepository;
        private TVMSMSKullaniciBilgiRepository _tVMSMSKullaniciBilgiRepository;
        private NeoConnectLogRepository _neoConnectLogRepository;
        private TVMYetkiGrupSablonuRepository _tVMYetkiGrupSablonuRepository;


        #region Procedure

        public List<KullaniciYetkiModel> KullaniciYetkileri_Getir(int yetkiGrupKodu)
        {
            int lang = 1;
            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (!String.IsNullOrEmpty(language))
                switch (language)
                {
                    case "tr": lang = 1; break;
                    case "en": lang = 2; break;
                    case "it": lang = 3; break;
                    case "fr": lang = 4; break;
                    case "es": lang = 5; break;
                }

            List<KullaniciYetkiModel> model = _dbContext.Database.SqlQuery<KullaniciYetkiModel>("KullaniciYetkileri_GetirTest2 @YetkiGrupKodu,@dilkodu",
                                                        new System.Data.SqlClient.SqlParameter("@YetkiGrupKodu", yetkiGrupKodu),
                                                        new System.Data.SqlClient.SqlParameter("@dilkodu", lang)).ToList<KullaniciYetkiModel>();

            return model;
        }

        public List<DuyuruProcedureModel> GetDuyuru(int tvmKodu)
        {
            SqlParameter parameter = new SqlParameter("@tvmkodu", System.Data.SqlDbType.Int);
            parameter.Value = tvmKodu;

            List<DuyuruProcedureModel> model = _dbContext.Database.SqlQuery<DuyuruProcedureModel>("SP_GetDuyurular @tvmkodu",
                                                                                                  parameter).ToList<DuyuruProcedureModel>();

            return model;
        }

        public List<TVMUrunYetkileriProcedureModel> GetTVMUrunYetkileri(int tvmKodu)
        {
            int lang = 1;
            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (!String.IsNullOrEmpty(language))
                switch (language)
                {
                    case "tr": lang = 1; break;
                    case "en": lang = 2; break;
                    case "it": lang = 3; break;
                    case "fr": lang = 4; break;
                    case "es": lang = 5; break;
                }

            SqlParameter parameter = new SqlParameter("@tvmkodu", System.Data.SqlDbType.Int);
            parameter.Value = tvmKodu;

            SqlParameter langparam = new SqlParameter("@dilkodu", System.Data.SqlDbType.Int);
            langparam.Value = lang;

            List<TVMUrunYetkileriProcedureModel> model = _dbContext.Database.SqlQuery<TVMUrunYetkileriProcedureModel>("SP_GetTVMUrunYetkileri @tvmkodu,@dilkodu",
                                                                                                    parameter, langparam).ToList<TVMUrunYetkileriProcedureModel>();

            return model;
        }

        public Performans Performansim(int tvmKodu, int kullaniciKodu, DateTime baslamatarihi, DateTime bitistarihi)
        {
            Performans model = new Performans();

            string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BABOnlineContext"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_PerformansimTest", con);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
                tvmkodu.Value = tvmKodu;

                SqlParameter kullanicikodu = new SqlParameter("@kullanicikodu", SqlDbType.Int);
                kullanicikodu.Value = kullaniciKodu;

                SqlParameter BaslamaTarihi = new SqlParameter("@BaslamaTarihi", SqlDbType.Date);
                BaslamaTarihi.Value = baslamatarihi;

                SqlParameter BitisTarihi = new SqlParameter("@BitisTarihi", SqlDbType.Date);
                BitisTarihi.Value = bitistarihi;


                cmd.Parameters.Add(tvmkodu);
                cmd.Parameters.Add(kullanicikodu);
                cmd.Parameters.Add(BaslamaTarihi);
                cmd.Parameters.Add(BitisTarihi);

                con.Open();

                using (XmlReader reader = cmd.ExecuteXmlReader())
                {
                    while (reader.Read())
                    {
                        string result = reader.ReadOuterXml();
                        if (!String.IsNullOrEmpty(result))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(Performans));
                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] buffer = Encoding.UTF8.GetBytes(result);
                                ms.Write(buffer, 0, buffer.Length);
                                ms.Position = 0;
                                using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8))
                                {
                                    model = (Performans)xs.Deserialize(ms);
                                }
                            }
                        }
                    }
                }

                con.Close();
            }

            return model;
        }

        public OfflineUretimPerformans OfflineUretim(int tvmKodu, int donemYil, string taliTvmKodlari)
        {
            OfflineUretimPerformans returnModel = new OfflineUretimPerformans();
            List<OfflineUretimPerformansProcedureModel> performansList = new List<OfflineUretimPerformansProcedureModel>();
            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = tvmKodu;

            SqlParameter donem = new SqlParameter("@donem", SqlDbType.Int);
            donem.Value = donemYil;

            SqlParameter talitvmler = new SqlParameter("@taliTvmKodu", SqlDbType.NVarChar, int.MaxValue);
            talitvmler.Value = String.Empty;
            if (String.IsNullOrEmpty(taliTvmKodlari))
            {
                taliTvmKodlari = tvmKodu.ToString() + ",";
            }
            talitvmler.Value = taliTvmKodlari;

            performansList = _dbContext.Database.SqlQuery<OfflineUretimPerformansProcedureModel>("SP_OfflineUretimPerformans @tvmkodu,@taliTvmKodu,@donem",
                                                                                             tvmkodu, talitvmler, donem).ToList<OfflineUretimPerformansProcedureModel>();
            if (performansList != null)
            {
                returnModel.performansList = performansList;
            }

            return returnModel;
        }
        public OfflineUretimPerformansKullanici OfflineUretimKullanici(int tvmKodu, int donemYil, string kullaniciKodu)
        {
            OfflineUretimPerformansKullanici returnModel = new OfflineUretimPerformansKullanici();
            List<OfflineUretimPerformansKullaniciProcedureModel> performansList = new List<OfflineUretimPerformansKullaniciProcedureModel>();
            SqlParameter tvmkodu = new SqlParameter("@tvmkodu", SqlDbType.Int);
            tvmkodu.Value = tvmKodu;

            SqlParameter donem = new SqlParameter("@donem", SqlDbType.Int);
            donem.Value = donemYil;

            SqlParameter kullanicilar = new SqlParameter("@kullaniciKodu", SqlDbType.NVarChar, int.MaxValue);
            kullanicilar.Value = String.Empty;
            if (String.IsNullOrEmpty(kullaniciKodu))
            {
                kullaniciKodu = tvmKodu.ToString() + ",";
            }
            kullanicilar.Value = kullaniciKodu;


            performansList = _dbContext.Database.SqlQuery<OfflineUretimPerformansKullaniciProcedureModel>("SP_OfflineUretimPerformansKullanici @tvmkodu,@kullaniciKodu,@donem",
                                                                                             tvmkodu, kullanicilar, donem).ToList<OfflineUretimPerformansKullaniciProcedureModel>();
            if (performansList != null)
            {
                returnModel.performansList = performansList;
            }

            return returnModel;
        }
        #endregion
        #region Kokpit Güncelleme
        public bool KokpitGuncelle(int tvmKodu, int donemYil, string taliTvmler, string branslar)
        {
            try
            {
                SqlParameter TvmKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
                TvmKodu.Value = tvmKodu;

                SqlParameter DonemYil = new SqlParameter("@DonemYil", SqlDbType.Int);
                DonemYil.Value = donemYil;

                SqlParameter TaliTVMKodlari = new SqlParameter("@TaliTVMKodlari", SqlDbType.NVarChar, int.MaxValue);
                TaliTVMKodlari.Value = taliTvmler;

                SqlParameter Branslar = new SqlParameter("@Branslar", SqlDbType.NVarChar, int.MaxValue);
                Branslar.Value = branslar;

                _dbContext.Database.ExecuteSqlCommand("SP_KokpitGuncelle  @TVMKodu, @DonemYil, @TaliTVMKodlari,@Branslar",
                                                            TvmKodu, DonemYil, TaliTVMKodlari, Branslar);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

            return true;
        }


        #endregion
        #region Kokpit Kulllanıcı Güncelleme
        public bool KokpitKullaniciGuncelle(int tvmKodu, int donemYil, string kullanici, string branslar)
        {
            try
            {
                SqlParameter TvmKodu = new SqlParameter("@TVMKodu", SqlDbType.Int);
                TvmKodu.Value = tvmKodu;

                SqlParameter DonemYil = new SqlParameter("@DonemYil", SqlDbType.Int);
                DonemYil.Value = donemYil;

                SqlParameter Kullanicilar = new SqlParameter("@KullaniciKodu", SqlDbType.NVarChar, int.MaxValue);
                Kullanicilar.Value = kullanici;

                SqlParameter Branslar = new SqlParameter("@Branslar", SqlDbType.NVarChar, int.MaxValue);
                Branslar.Value = branslar;

                _dbContext.Database.ExecuteSqlCommand("SP_KokpitKullaniciGuncelle  @TVMKodu, @DonemYil, @KullaniciKodu,@Branslar",
                                                            TvmKodu, DonemYil, Kullanicilar, Branslar);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

            return true;
        }


        #endregion
        public WEBServisLogRepository WEBServisLogRepository
        {
            get
            {
                if (_webservisLogRepository == null)
                {
                    _webservisLogRepository = new WEBServisLogRepository(_dbContext);
                }

                return _webservisLogRepository;
            }
        }

        public TVMDetayRepository TVMDetayRepository
        {
            get
            {
                if (_tvmDetayRepository == null)
                {
                    _tvmDetayRepository = new TVMDetayRepository(_dbContext);
                }

                return _tvmDetayRepository;
            }
        }
        public ParaBirimleriRepository ParaBirimleriRepository
        {
            get
            {
                if (_paraBirimleriRepository == null)
                {
                    _paraBirimleriRepository = new ParaBirimleriRepository(_dbContext);
                }

                return _paraBirimleriRepository;
            }
        }

        public TVMAcentelikleriRepository TVMAcentelikleriRepository
        {
            get
            {
                if (_tVMAcentelikleriRepository == null)
                {
                    _tVMAcentelikleriRepository = new TVMAcentelikleriRepository(_dbContext);
                }
                return _tVMAcentelikleriRepository;
            }
        }

        public TVMBankaHesaplariRepository TVMBankaHesaplariRepository
        {
            get
            {
                if (_tVMBankaHesaplariRepository == null)
                {
                    _tVMBankaHesaplariRepository = new TVMBankaHesaplariRepository(_dbContext);
                }
                return _tVMBankaHesaplariRepository;
            }
        }

        public TVMBolgeleriRepository TVMBolgeleriRepository
        {
            get
            {
                if (_tVMBolgeleriRepository == null)
                {
                    _tVMBolgeleriRepository = new TVMBolgeleriRepository(_dbContext);
                }
                return _tVMBolgeleriRepository;
            }
        }

        public TVMDepartmanlarRepository TVMDepartmanlarRepository
        {
            get
            {
                if (_tVMDepartmanlarRepository == null)
                {
                    _tVMDepartmanlarRepository = new TVMDepartmanlarRepository(_dbContext);
                }
                return _tVMDepartmanlarRepository;
            }
        }

        public TVMDokumanlarRepository TVMDokumanlarRepository
        {
            get
            {
                if (_tVMDokumanlarRepository == null)
                {
                    _tVMDokumanlarRepository = new TVMDokumanlarRepository(_dbContext);
                }
                return _tVMDokumanlarRepository;
            }
        }

        public TVMDurumTarihcesiRepository TVMDurumTarihcesiRepository
        {
            get
            {
                if (_tVMDurumTarihcesiRepository == null)
                {
                    _tVMDurumTarihcesiRepository = new TVMDurumTarihcesiRepository(_dbContext);
                }
                return _tVMDurumTarihcesiRepository;
            }
        }

        public TVMIletisimYetkilileriRepository TVMIletisimYetkilileriRepository
        {
            get
            {
                if (_tVMIletisimYetkilileriRepository == null)
                {
                    _tVMIletisimYetkilileriRepository = new TVMIletisimYetkilileriRepository(_dbContext);
                }
                return _tVMIletisimYetkilileriRepository;
            }
        }

        public TVMIPBaglantiRepository TVMIPBaglantiRepository
        {
            get
            {
                if (_tVMIPBaglantiRepository == null)
                {
                    _tVMIPBaglantiRepository = new TVMIPBaglantiRepository(_dbContext);
                }
                return _tVMIPBaglantiRepository;
            }
        }

        public TVMKullaniciAtamaRepository TVMKullaniciAtamaRepository
        {
            get
            {
                if (_tVMKullaniciAtamaRepository == null)
                {
                    _tVMKullaniciAtamaRepository = new TVMKullaniciAtamaRepository(_dbContext);
                }
                return _tVMKullaniciAtamaRepository;
            }
        }

        public TVMKullaniciDurumTarihcesiRepository TVMKullaniciDurumTarihcesiRepository
        {
            get
            {
                if (_tVMKullaniciDurumTarihcesiRepository == null)
                {
                    _tVMKullaniciDurumTarihcesiRepository = new TVMKullaniciDurumTarihcesiRepository(_dbContext);
                }
                return _tVMKullaniciDurumTarihcesiRepository;
            }
        }

        public TVMKullanicilarRepository TVMKullanicilarRepository
        {
            get
            {
                if (_tVMKullanicilarRepository == null)
                {
                    _tVMKullanicilarRepository = new TVMKullanicilarRepository(_dbContext);
                }
                return _tVMKullanicilarRepository;
            }
        }

        public TVMKullaniciSifreTarihcesiRepository TVMKullaniciSifreTarihcesiRepository
        {
            get
            {
                if (_tVMKullaniciSifreTarihcesiRepository == null)
                {
                    _tVMKullaniciSifreTarihcesiRepository = new TVMKullaniciSifreTarihcesiRepository(_dbContext);
                }
                return _tVMKullaniciSifreTarihcesiRepository;
            }
        }

        public TVMKullaniciSifremiUnuttumRepository TVMKullaniciSifremiUnuttumRepository
        {
            get
            {
                if (_tVMKullaniciSifremiUnuttumRepository == null)
                {
                    _tVMKullaniciSifremiUnuttumRepository = new TVMKullaniciSifremiUnuttumRepository(_dbContext);
                }
                return _tVMKullaniciSifremiUnuttumRepository;
            }
        }

        public TVMNotlarRepository TVMNotlarRepository
        {
            get
            {
                if (_tVMNotlarRepository == null)
                {
                    _tVMNotlarRepository = new TVMNotlarRepository(_dbContext);
                }
                return _tVMNotlarRepository;
            }
        }

        public TVMUrunYetkileriRepository TVMUrunYetkileriRepository
        {
            get
            {
                if (_tVMUrunYetkileriRepository == null)
                {
                    _tVMUrunYetkileriRepository = new TVMUrunYetkileriRepository(_dbContext);
                }
                return _tVMUrunYetkileriRepository;
            }
        }

        public TVMYetkiGruplariRepository TVMYetkiGruplariRepository
        {
            get
            {
                if (_tVMYetkiGruplariRepository == null)
                {
                    _tVMYetkiGruplariRepository = new TVMYetkiGruplariRepository(_dbContext);
                }
                return _tVMYetkiGruplariRepository;
            }
        }

        public TVMYetkiGrupYetkileriRepository TVMYetkiGrupYetkileriRepository
        {
            get
            {
                if (_tVMYetkiGrupYetkileriRepository == null)
                {
                    _tVMYetkiGrupYetkileriRepository = new TVMYetkiGrupYetkileriRepository(_dbContext);
                }
                return _tVMYetkiGrupYetkileriRepository;
            }
        }

        public TVMWebServisKullanicilariRepository TVMWebServisKullanicilariRepository
        {
            get
            {
                if (_tVMWebServisKullanicilariRepository == null)
                {
                    _tVMWebServisKullanicilariRepository = new TVMWebServisKullanicilariRepository(_dbContext);
                }
                return _tVMWebServisKullanicilariRepository;
            }
        }

        public TVMKullaniciNotlarRepository TVMKullaniciNotlarRepository
        {
            get
            {
                if (_tVMKullaniciNotlarRepository == null)
                    _tVMKullaniciNotlarRepository = new TVMKullaniciNotlarRepository(_dbContext);
                return _tVMKullaniciNotlarRepository;
            }
        }

        public DuyurularRepository DuyurularRepository
        {
            get
            {
                if (_duyurularRepository == null)
                    _duyurularRepository = new DuyurularRepository(_dbContext);

                return _duyurularRepository;
            }
        }

        public DuyuruTVMRepository DuyuruTVMRepository
        {
            get
            {
                if (_duyuruTVMRepository == null)
                    _duyuruTVMRepository = new DuyuruTVMRepository(_dbContext);
                return _duyuruTVMRepository;
            }
        }

        public DuyuruDokumanRepository DuyuruDokumanRepository
        {
            get
            {
                if (_duyuruDokumanRepository == null)
                    _duyuruDokumanRepository = new DuyuruDokumanRepository(_dbContext);

                return _duyuruDokumanRepository;
            }
        }

        public MapfreKullaniciRepository MapfreKullaniciRepository
        {
            get
            {
                if (_mapfreKullaniciRepository == null)
                    _mapfreKullaniciRepository = new MapfreKullaniciRepository(_dbContext);

                return _mapfreKullaniciRepository;
            }
        }

        public KesintilerRepository KesintilerRepository
        {
            get
            {
                if (_kesintilerRepository == null)
                {
                    _kesintilerRepository = new KesintilerRepository(_dbContext);
                }
                return _kesintilerRepository;
            }
        }

        public KesintiTurleriRepository KesintiTurleriRepository
        {
            get
            {
                if (_kesintiTurleriRepository == null)
                {
                    _kesintiTurleriRepository = new KesintiTurleriRepository(_dbContext);
                }
                return _kesintiTurleriRepository;
            }
        }

        public DokumanTurleriRepository DokumanTurleriRepository
        {
            get
            {
                if (_dokumanTurleriRepository == null)
                {
                    _dokumanTurleriRepository = new DokumanTurleriRepository(_dbContext);
                }
                return _dokumanTurleriRepository;
            }
        }


        public TVMSMSKullaniciBilgiRepository TVMSMSKullaniciBilgiRepository
        {
            get
            {
                if (_tVMSMSKullaniciBilgiRepository == null)
                {
                    _tVMSMSKullaniciBilgiRepository = new TVMSMSKullaniciBilgiRepository(_dbContext);
                }
                return _tVMSMSKullaniciBilgiRepository;
            }
        }
        public NeoConnectLogRepository NeoConnectLogRepository
        {
            get
            {
                if (_neoConnectLogRepository == null)
                {
                    _neoConnectLogRepository = new NeoConnectLogRepository(_dbContext);
                }
                return _neoConnectLogRepository;
            }
        }
        public TVMYetkiGrupSablonuRepository TVMYetkiGrupSablonuRepository
        {
            get
            {
                if (_tVMYetkiGrupSablonuRepository == null)
                {
                    _tVMYetkiGrupSablonuRepository = new TVMYetkiGrupSablonuRepository(_dbContext);
                }
                return _tVMYetkiGrupSablonuRepository;
            }
        }
        #region Stored procedure
        public void spMapfreKullanici(int mapfeKullaniciId, string sifre)
        {
            SqlParameter mapfeKullaniciIdParameter = new System.Data.SqlClient.SqlParameter("@MapfeKullaniciId", mapfeKullaniciId);
            SqlParameter sifreParameter = new System.Data.SqlClient.SqlParameter("@Sifre", sifre);

            _dbContext.Database.ExecuteSqlCommand("exec SP_MapfreKullanici @MapfeKullaniciId, @Sifre", mapfeKullaniciIdParameter, sifreParameter);
        }
        #endregion
    }
}

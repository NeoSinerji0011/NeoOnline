using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface ITeklifContext : IUnitOfWork
    {
        int YeniTeklifNo(int tvmKodu);
        long OfflinePoliceNumara(int tvmKodu, int urunKodu);
        TeklifOzelAlan TeklifOzelAlan(int teklifId);

        void SetDbContext(DbContext dbContext);

        TeklifAracRepository TeklifAracRepository { get; }
        TeklifAracEkSoruRepository TeklifAracEkSoruRepository { get; }
        TeklifGenelRepository TeklifGenelRepository { get; }
        TeklifNoSayacRepository TeklifNoSayacRepository { get; }
        TeklifOdemePlaniRepository TeklifOdemePlaniRepository { get; }
        TeklifRizikoAdresiRepository TeklifRizikoAdresiRepository { get; }
        TeklifSigortaEttirenRepository TeklifSigortaEttirenRepository { get; }
        TeklifSigortaliRepository TeklifSigortaliRepository { get; }
        TeklifSoruRepository TeklifSoruRepository { get; }
        TeklifTeminatRepository TeklifTeminatRepository { get; }
        TeklifVergiRepository TeklifVergiRepository { get; }
        TeklifWebServisCevapRepository TeklifWebServisCevapRepository { get; }
        WEBServisLogRepository WEBServisLogRepository { get; }
        IsDurumRepository IsDurumRepository { get; }
        IsDurumDetayRepository IsDurumDetayRepository { get; }
        TeklifEMailLogRepository TeklifEMailLogRepository { get; }
        TeklifNotRepository TeklifNotRepository { get; }
        TeklifProvizyonRepository TeklifProvizyonRepository { get; }
        TeklifDigerSirketlerRepository TeklifDigerSirketlerRepository { get; }

        //Metlife
        IsTakipRepository IsTakipRepository { get; }
        IsTakipDetayRepository IsTakipDetayRepository { get; }
        IsTakipIsTipleriRepository IsTakipIsTipleriRepository { get; }
        IsTakipIsTipleriDetayRepository IsTakipIsTipleriDetayRepository { get; }
        IsTakipKullaniciGruplariRepository IsTakipKullaniciGruplariRepository { get; }
        IsTakipSoruRepository IsTakipSoruRepository { get; }
        IsTakipDokumanRepository IsTakipDokumanRepository { get; }
        IsTakipKullaniciGrupKullanicilariRepository IsTakipKullaniciGrupKullanicilariRepository { get; }
        KaskoHukuksalKorumaBedelRepository KaskoHukuksalKorumaBedelRepository { get; }
        Cr_KaskoHukuksalKorumaRepository Cr_KaskoHukuksalKorumaRepository { get; }
        HDIKaskoHukuksalKorumaBedelleriRepository HDIKaskoHukuksalKorumaBedelleriRepository { get; }
        AnadoluKullanimTipSekilRepository AnadoluKullanimTipSekilRepository { get; }

        //---Metlife

        //Lilyum 
        LilyumKartTeminatKullanimRepository LilyumKartTeminatKullanimRepository { get; } 
        LilyumKartTeminatlarRepository LilyumKartTeminatlarRepository { get; }
        //Reasuror 
        ReasurorGenelRepository ReasurorGenelRepository { get; }
        UnderwritersRepository UnderwritersRepository { get; }
        TeklifDokumanRepository TeklifDokumanRepository { get; }

    }

    public class TeklifContext : ITeklifContext
    {
        private DbContext _dbContext;
        private bool _disposed;

        public TeklifContext(IDbContextFactory dbContextFactory)
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
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("DbEntityValidationException: ", ex.Message);
                sb.AppendLine();
                foreach (var item in ex.EntityValidationErrors)
                {
                    foreach (var error in item.ValidationErrors)
                    {
                        sb.AppendFormat("Property: {0}, ErrorMessage: {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }
                throw new DbEntityValidationException(sb.ToString());
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

        #region ITeklifContext
        public int YeniTeklifNo(int tvmKodu)
        {
            List<TeklifNoSayac> sayac = _dbContext.Database.SqlQuery<TeklifNoSayac>("TeklifNoSayac_Create @TVMKodu",
                                                        new System.Data.SqlClient.SqlParameter("@TVMKodu", tvmKodu)).ToList<TeklifNoSayac>();

            if (sayac != null && sayac.Count == 1)
            {
                TeklifNoSayac s = sayac.FirstOrDefault();

                if (s != null)
                {
                    return s.TeklifNo;
                }
            }

            throw new Exception("Teklif no okunamadı...");
        }

        public long OfflinePoliceNumara(int tvmKodu, int urunKodu)
        {
            SqlParameter tvmKoduParameter = new System.Data.SqlClient.SqlParameter("@TVMKodu", tvmKodu);
            SqlParameter urunKoduParameter = new System.Data.SqlClient.SqlParameter("@UrunKodu", urunKodu);

            long policeNo = _dbContext.Database.SqlQuery<long>("OfflinePoliceNumara_Sonraki @TVMKodu, @UrunKodu",
                                                        tvmKoduParameter,
                                                        urunKoduParameter).First<long>();

            return policeNo;
        }

        public TeklifOzelAlan TeklifOzelAlan(int teklifId)
        {
            SqlParameter teklifIdParameter = new System.Data.SqlClient.SqlParameter("@TeklifId", teklifId);

            return _dbContext.Database.SqlQuery<TeklifOzelAlan>("SP_TeklifOzelAlan @TeklifId",
                                                        teklifIdParameter).FirstOrDefault<TeklifOzelAlan>();
        }

        public void SetDbContext(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private TeklifAracRepository _teklifAracRepository;
        public TeklifAracRepository TeklifAracRepository
        {
            get
            {
                if (_teklifAracRepository == null)
                {
                    _teklifAracRepository = new TeklifAracRepository(_dbContext);
                }
                return _teklifAracRepository;
            }
        }

        private TeklifAracEkSoruRepository _teklifAracEkSoruRepository;
        public TeklifAracEkSoruRepository TeklifAracEkSoruRepository
        {
            get
            {
                if (_teklifAracEkSoruRepository == null)
                {
                    _teklifAracEkSoruRepository = new TeklifAracEkSoruRepository(_dbContext);
                }
                return _teklifAracEkSoruRepository;
            }
        }

        private TeklifGenelRepository _teklifGenelRepository;
        public TeklifGenelRepository TeklifGenelRepository
        {
            get
            {
                if (_teklifGenelRepository == null)
                {
                    _teklifGenelRepository = new TeklifGenelRepository(_dbContext);
                }
                return _teklifGenelRepository;
            }
        }

        private TeklifNoSayacRepository _teklifNoSayacRepository;
        public TeklifNoSayacRepository TeklifNoSayacRepository
        {
            get
            {
                if (_teklifNoSayacRepository == null)
                {
                    _teklifNoSayacRepository = new TeklifNoSayacRepository(_dbContext);
                }
                return _teklifNoSayacRepository;
            }
        }

        private TeklifOdemePlaniRepository _teklifOdemePlaniRepository;
        public TeklifOdemePlaniRepository TeklifOdemePlaniRepository
        {
            get
            {
                if (_teklifOdemePlaniRepository == null)
                {
                    _teklifOdemePlaniRepository = new TeklifOdemePlaniRepository(_dbContext);
                }
                return _teklifOdemePlaniRepository;
            }
        }

        private TeklifRizikoAdresiRepository _teklifRizikoAdresiRepository;
        public TeklifRizikoAdresiRepository TeklifRizikoAdresiRepository
        {
            get
            {
                if (_teklifRizikoAdresiRepository == null)
                {
                    _teklifRizikoAdresiRepository = new TeklifRizikoAdresiRepository(_dbContext);
                }
                return _teklifRizikoAdresiRepository;
            }
        }

        private TeklifSigortaEttirenRepository _teklifSigortaEttirenRepository;
        public TeklifSigortaEttirenRepository TeklifSigortaEttirenRepository
        {
            get
            {
                if (_teklifSigortaEttirenRepository == null)
                {
                    _teklifSigortaEttirenRepository = new TeklifSigortaEttirenRepository(_dbContext);
                }
                return _teklifSigortaEttirenRepository;
            }
        }

        private TeklifSigortaliRepository _teklifSigortaliRepository;
        public TeklifSigortaliRepository TeklifSigortaliRepository
        {
            get
            {
                if (_teklifSigortaliRepository == null)
                {
                    _teklifSigortaliRepository = new TeklifSigortaliRepository(_dbContext);
                }
                return _teklifSigortaliRepository;
            }
        }

        private TeklifSoruRepository _teklifSoruRepository;
        public TeklifSoruRepository TeklifSoruRepository
        {
            get
            {
                if (_teklifSoruRepository == null)
                {
                    _teklifSoruRepository = new TeklifSoruRepository(_dbContext);
                }
                return _teklifSoruRepository;
            }
        }

        private TeklifTeminatRepository _teklifTeminatRepository;
        public TeklifTeminatRepository TeklifTeminatRepository
        {
            get
            {
                if (_teklifTeminatRepository == null)
                {
                    _teklifTeminatRepository = new TeklifTeminatRepository(_dbContext);
                }
                return _teklifTeminatRepository;
            }
        }

        private TeklifVergiRepository _teklifVergiRepository;
        public TeklifVergiRepository TeklifVergiRepository
        {
            get
            {
                if (_teklifVergiRepository == null)
                {
                    _teklifVergiRepository = new TeklifVergiRepository(_dbContext);
                }
                return _teklifVergiRepository;
            }
        }

        private TeklifWebServisCevapRepository _teklifWebServisCevapRepository;
        public TeklifWebServisCevapRepository TeklifWebServisCevapRepository
        {
            get
            {
                if (_teklifWebServisCevapRepository == null)
                {
                    _teklifWebServisCevapRepository = new TeklifWebServisCevapRepository(_dbContext);
                }
                return _teklifWebServisCevapRepository;
            }
        }

        private WEBServisLogRepository _webServisLogRepository;
        public WEBServisLogRepository WEBServisLogRepository
        {
            get
            {
                if (_webServisLogRepository == null)
                {
                    _webServisLogRepository = new WEBServisLogRepository(_dbContext);
                }
                return _webServisLogRepository;
            }
        }

        private IsDurumRepository _isDurumRepository;
        public IsDurumRepository IsDurumRepository
        {
            get
            {
                if (_isDurumRepository == null)
                {
                    _isDurumRepository = new IsDurumRepository(_dbContext);
                }

                return _isDurumRepository;
            }
        }

        private IsDurumDetayRepository _isDurumDetayRepository;
        public IsDurumDetayRepository IsDurumDetayRepository
        {
            get
            {
                if (_isDurumDetayRepository == null)
                {
                    _isDurumDetayRepository = new IsDurumDetayRepository(_dbContext);
                }

                return _isDurumDetayRepository;
            }
        }

        private TeklifEMailLogRepository _teklifEMailLogRepository;
        public TeklifEMailLogRepository TeklifEMailLogRepository
        {
            get
            {
                if (_teklifEMailLogRepository == null)
                {
                    _teklifEMailLogRepository = new TeklifEMailLogRepository(_dbContext);
                }

                return _teklifEMailLogRepository;
            }
        }

        private TeklifNotRepository _teklifNotRepository;
        public TeklifNotRepository TeklifNotRepository
        {
            get
            {
                if (_teklifNotRepository == null)
                {
                    _teklifNotRepository = new TeklifNotRepository(_dbContext);
                }
                return _teklifNotRepository;
            }
        }

        private TeklifProvizyonRepository _teklifProvizyonRepository;
        public TeklifProvizyonRepository TeklifProvizyonRepository
        {
            get
            {
                if (_teklifProvizyonRepository == null)
                {
                    _teklifProvizyonRepository = new TeklifProvizyonRepository(_dbContext);
                }
                return _teklifProvizyonRepository;
            }
        }

        private TeklifDigerSirketlerRepository _teklifDigerSirketlerRepository;
        public TeklifDigerSirketlerRepository TeklifDigerSirketlerRepository
        {
            get
            {
                if (_teklifDigerSirketlerRepository == null)
                {
                    _teklifDigerSirketlerRepository = new TeklifDigerSirketlerRepository(_dbContext);
                }
                return _teklifDigerSirketlerRepository;
            }
        }


        //Metlife
        private IsTakipRepository _isTakipRepository;
        public IsTakipRepository IsTakipRepository
        {
            get
            {
                if (_isTakipRepository == null)
                {
                    _isTakipRepository = new IsTakipRepository(_dbContext);
                }
                return _isTakipRepository;
            }
        }

        private IsTakipDetayRepository _isTakipDetayRepository;
        public IsTakipDetayRepository IsTakipDetayRepository
        {
            get
            {
                if (_isTakipDetayRepository == null)
                {
                    _isTakipDetayRepository = new IsTakipDetayRepository(_dbContext);
                }
                return _isTakipDetayRepository;
            }
        }

        private IsTakipIsTipleriRepository _isTakipIsTipleriRepository;
        public IsTakipIsTipleriRepository IsTakipIsTipleriRepository
        {
            get
            {
                if (_isTakipIsTipleriRepository == null)
                {
                    _isTakipIsTipleriRepository = new IsTakipIsTipleriRepository(_dbContext);
                }
                return _isTakipIsTipleriRepository;
            }
        }

        private IsTakipIsTipleriDetayRepository _isTakipIsTipleriDetayRepository;
        public IsTakipIsTipleriDetayRepository IsTakipIsTipleriDetayRepository
        {
            get
            {
                if (_isTakipIsTipleriDetayRepository == null)
                {
                    _isTakipIsTipleriDetayRepository = new IsTakipIsTipleriDetayRepository(_dbContext);
                }
                return _isTakipIsTipleriDetayRepository;
            }
        }

        private IsTakipKullaniciGruplariRepository _isTakipKullaniciGruplariRepository;
        public IsTakipKullaniciGruplariRepository IsTakipKullaniciGruplariRepository
        {
            get
            {
                if (_isTakipKullaniciGruplariRepository == null)
                {
                    _isTakipKullaniciGruplariRepository = new IsTakipKullaniciGruplariRepository(_dbContext);
                }
                return _isTakipKullaniciGruplariRepository;
            }
        }

        private IsTakipKullaniciGrupKullanicilariRepository _isTakipKullaniciGrupKullanicilariRepository;
        public IsTakipKullaniciGrupKullanicilariRepository IsTakipKullaniciGrupKullanicilariRepository
        {
            get
            {
                if (_isTakipKullaniciGrupKullanicilariRepository == null)
                {
                    _isTakipKullaniciGrupKullanicilariRepository = new IsTakipKullaniciGrupKullanicilariRepository(_dbContext);
                }
                return _isTakipKullaniciGrupKullanicilariRepository;
            }
        }

        private IsTakipSoruRepository _isTakipSoruRepository;
        public IsTakipSoruRepository IsTakipSoruRepository
        {
            get
            {
                if (_isTakipSoruRepository == null)
                {
                    _isTakipSoruRepository = new IsTakipSoruRepository(_dbContext);
                }
                return _isTakipSoruRepository;
            }
        }

        private IsTakipDokumanRepository _isTakipDokumanRepository;
        public IsTakipDokumanRepository IsTakipDokumanRepository
        {
            get
            {
                if (_isTakipDokumanRepository == null)
                {
                    _isTakipDokumanRepository = new IsTakipDokumanRepository(_dbContext);
                }
                return _isTakipDokumanRepository;
            }
        }

        //----Metlife

        private KaskoHukuksalKorumaBedelRepository _kaskoHukuksalKorumaBedelRepository;
        public KaskoHukuksalKorumaBedelRepository KaskoHukuksalKorumaBedelRepository
        {
            get
            {
                if (_kaskoHukuksalKorumaBedelRepository == null)
                {
                    _kaskoHukuksalKorumaBedelRepository = new KaskoHukuksalKorumaBedelRepository(_dbContext);
                }
                return _kaskoHukuksalKorumaBedelRepository;
            }
        }

        private Cr_KaskoHukuksalKorumaRepository _Cr_KaskoHukuksalKorumaRepository;
        public Cr_KaskoHukuksalKorumaRepository Cr_KaskoHukuksalKorumaRepository
        {
            get
            {
                if (_Cr_KaskoHukuksalKorumaRepository == null)
                {
                    _Cr_KaskoHukuksalKorumaRepository = new Cr_KaskoHukuksalKorumaRepository(_dbContext);
                }
                return _Cr_KaskoHukuksalKorumaRepository;
            }
        }

        private HDIKaskoHukuksalKorumaBedelleriRepository _HDIKaskoHukuksalKorumaBedelleriRepository;
        public HDIKaskoHukuksalKorumaBedelleriRepository HDIKaskoHukuksalKorumaBedelleriRepository
        {
            get
            {
                if (_HDIKaskoHukuksalKorumaBedelleriRepository == null)
                {
                    _HDIKaskoHukuksalKorumaBedelleriRepository = new HDIKaskoHukuksalKorumaBedelleriRepository(_dbContext);
                }
                return _HDIKaskoHukuksalKorumaBedelleriRepository;
            }
        }

        private AnadoluKullanimTipSekilRepository _AnadoluKullanimTipSekilRepository;
        public AnadoluKullanimTipSekilRepository AnadoluKullanimTipSekilRepository
        {
            get
            {
                if (_AnadoluKullanimTipSekilRepository == null)
                {
                    _AnadoluKullanimTipSekilRepository = new AnadoluKullanimTipSekilRepository(_dbContext);
                }
                return _AnadoluKullanimTipSekilRepository;
            }
        }

        //Lilyum
        private LilyumKartTeminatKullanimRepository _LilyumKartTeminatKullanimRepository;
        public LilyumKartTeminatKullanimRepository LilyumKartTeminatKullanimRepository
        {
            get
            {
                if (_LilyumKartTeminatKullanimRepository == null)
                {
                    _LilyumKartTeminatKullanimRepository = new LilyumKartTeminatKullanimRepository(_dbContext);
                }
                return _LilyumKartTeminatKullanimRepository;
            }
        }

        private LilyumKartTeminatlarRepository _LilyumKartTeminatlarRepository;
        public LilyumKartTeminatlarRepository LilyumKartTeminatlarRepository
        {
            get
            {
                if (_LilyumKartTeminatlarRepository == null)
                {
                    _LilyumKartTeminatlarRepository = new LilyumKartTeminatlarRepository(_dbContext);
                }
                return _LilyumKartTeminatlarRepository;
            }
        }
        //Reasuror
        private ReasurorGenelRepository _ReasurorGenelRepository;
        public ReasurorGenelRepository ReasurorGenelRepository
        {
            get
            {
                if (_ReasurorGenelRepository == null)
                {
                    _ReasurorGenelRepository = new ReasurorGenelRepository(_dbContext);
                }
                return _ReasurorGenelRepository;
            }
        }
        //Underwriters
        private UnderwritersRepository _UnderwritersRepository;
        public UnderwritersRepository UnderwritersRepository
        {
            get
            {
                if (_UnderwritersRepository == null)
                {
                    _UnderwritersRepository = new UnderwritersRepository(_dbContext);
                }
                return _UnderwritersRepository;
            }
        }

        private TeklifDokumanRepository _TeklifDokumanRepository;
        public TeklifDokumanRepository TeklifDokumanRepository
        {
            get
            {
                if (_TeklifDokumanRepository == null)
                {
                    _TeklifDokumanRepository = new TeklifDokumanRepository(_dbContext);
                }
                return _TeklifDokumanRepository;
            }
        }

        #endregion

    }
}

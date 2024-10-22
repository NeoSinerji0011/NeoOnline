using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.SqlClient;
using System.Data;
using Neosinerji.BABOnlineTP.Database.Common;

namespace Neosinerji.BABOnlineTP.Database.UnitOfWork
{
    public interface IPoliceContext : IUnitOfWork
    {
        PoliceGenelRepository PoliceGenelRepository { get; }
        PoliceSigortaliRepository PoliceSigortaliRepository { get; }
        PoliceSigortaEttirenRepository PoliceSigortaEttirenRepository { get; }
        PoliceAracRepository PoliceAracRepository { get; }
        PoliceRizikoAdresiRepository PoliceRizikoAdresiRepository { get; }
        PoliceVergiRepository PoliceVergiRepository { get; }
        PoliceOdemePlaniRepository PoliceOdemePlaniRepository { get; }
        PoliceTahsilatRepository PoliceTahsilatRepository { get; }
        List<PoliceGenel> PoliceAraOfflineByKimlik(int TvmKod, string kimlik);
        PaylasimliPoliceUretimRepository PaylasimliPoliceUretimRepository { get; }
        AutoPoliceTransferRepository AutoPoliceTransferRepository { get; }
        List<AutoPoliceTransferProcedureModel> AutoPoliceTransferGetir(DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string SigortaSirketleriListe, int tvmKodu);

        HasarGenelBilgilerRepository HasarGenelBilgilerRepository { get; }
        HasarAnlasmaliServislerRepository HasarAnlasmaliServislerRepository { get; }
        HasarAsistansFirmalariRepository HasarAsistansFirmalariRepository { get; }
        HasarEksperIslemleriRepository HasarEksperIslemleriRepository { get; }
        HasarEksperListesiRepository HasarEksperListesiRepository { get; }
        HasarNotlariRepository HasarNotlariRepository { get; }
        HasarZorunluEvraklariRepository HasarZorunluEvraklariRepository { get; }
        HasarZorunluEvrakListesiRepository HasarZorunluEvrakListesiRepository { get; }
        HasarBankaHesaplariRepository HasarBankaHesaplariRepository { get; }
        HasarIletisimYetkilileriRepository HasarIletisimYetkilileriRepository { get; }
        PoliceTransferReaderKullanicilariRepository PoliceTransferReaderKullanicilariRepository { get; }
        KimlikNoUretRepository KimlikNoUretRepository { get; }
        PoliceDokumanRepository PoliceDokumanRepository { get; }
        SigortaSirketleriRepository SigortaSirketleriRepository { get; }
    }

    public class PoliceContext : IPoliceContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public PoliceContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout = 180;
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

        public List<PoliceGenel> PoliceAraOfflineByKimlik(int TvmKod, string kimlik)
        {

            SqlParameter TVMKodu = new SqlParameter("@tvmKodu", SqlDbType.Int);
            TVMKodu.Value = TvmKod;

            SqlParameter Kimlik = new SqlParameter("@kimlik", SqlDbType.NVarChar);
            Kimlik.Value = kimlik;


            List<PoliceGenel> policeAraOffline = _dbContext.Database.SqlQuery<PoliceGenel>
                                                                ("SP_PoliceAraOffline  @kimlik,@tvmKodu", TVMKodu, Kimlik)
                                                                .ToList<PoliceGenel>();

            return policeAraOffline;
        }

        public List<AutoPoliceTransferProcedureModel> AutoPoliceTransferGetir(DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string SigortaSirketleriListe, int tvmKodu)
        {
            SqlParameter tanzimBaslangicTarih = new SqlParameter("@TanzimBaslangicTarihi", SqlDbType.DateTime);
            tanzimBaslangicTarih.Value = TanzimBaslangicTarihi;

            SqlParameter tanzimBitisTarih = new SqlParameter("@TanzimBitisTarihi", SqlDbType.DateTime);
            tanzimBitisTarih.Value = TanzimBitisTarihi;

            SqlParameter SigortaSirketleri = new SqlParameter("@SigortaSirketleri", SqlDbType.NVarChar, int.MaxValue);
            SigortaSirketleri.Value = !String.IsNullOrEmpty(SigortaSirketleriListe) ? SigortaSirketleriListe : "";


            SqlParameter TVMKodu = new SqlParameter("@TvmKodu", SqlDbType.Int);
            TVMKodu.Value = tvmKodu;

            List<AutoPoliceTransferProcedureModel> result = _dbContext.Database.SqlQuery<AutoPoliceTransferProcedureModel>
                                                                ("SP_AutoPoliceTransfer  @TanzimBaslangicTarihi,@TanzimBitisTarihi,@SigortaSirketleri,@TvmKodu"
                                                                , tanzimBaslangicTarih, tanzimBitisTarih, SigortaSirketleri, TVMKodu)
                                                                .ToList<AutoPoliceTransferProcedureModel>();

            return result;
        }

        #region IPoliceContext
        private PoliceGenelRepository _policeGenelRepository;
        public PoliceGenelRepository PoliceGenelRepository
        {
            get
            {
                if (_policeGenelRepository == null)
                    _policeGenelRepository = new PoliceGenelRepository(_dbContext);

                return _policeGenelRepository;
            }
        }

        private PoliceSigortaliRepository _policeSigortaliRepository;
        public PoliceSigortaliRepository PoliceSigortaliRepository
        {
            get
            {
                if (_policeSigortaliRepository == null)
                    _policeSigortaliRepository = new PoliceSigortaliRepository(_dbContext);

                return _policeSigortaliRepository;
            }
        }

        private PoliceSigortaEttirenRepository _policeSigortaEttirenRepository;
        public PoliceSigortaEttirenRepository PoliceSigortaEttirenRepository
        {
            get
            {
                if (_policeSigortaEttirenRepository == null)
                    _policeSigortaEttirenRepository = new PoliceSigortaEttirenRepository(_dbContext);

                return _policeSigortaEttirenRepository;
            }
        }

        private PoliceAracRepository _policeAracRepository;
        public PoliceAracRepository PoliceAracRepository
        {
            get
            {
                if (_policeAracRepository == null)
                    _policeAracRepository = new PoliceAracRepository(_dbContext);

                return _policeAracRepository;
            }
        }

        private PoliceRizikoAdresiRepository _policeRizikoAdresiRepository;
        public PoliceRizikoAdresiRepository PoliceRizikoAdresiRepository
        {
            get
            {
                if (_policeRizikoAdresiRepository == null)
                    _policeRizikoAdresiRepository = new PoliceRizikoAdresiRepository(_dbContext);

                return _policeRizikoAdresiRepository;
            }
        }

        private PoliceVergiRepository _policeVergiRepository;
        public PoliceVergiRepository PoliceVergiRepository
        {
            get
            {
                if (_policeVergiRepository == null)
                    _policeVergiRepository = new PoliceVergiRepository(_dbContext);

                return _policeVergiRepository;
            }
        }

        private PoliceOdemePlaniRepository _policeOdemePlaniRepository;
        public PoliceOdemePlaniRepository PoliceOdemePlaniRepository
        {
            get
            {
                if (_policeOdemePlaniRepository == null)
                    _policeOdemePlaniRepository = new PoliceOdemePlaniRepository(_dbContext);

                return _policeOdemePlaniRepository;
            }
        }

        private PoliceTahsilatRepository _policeTahsilatRepository;
        public PoliceTahsilatRepository PoliceTahsilatRepository
        {
            get
            {
                if (_policeTahsilatRepository == null)
                    _policeTahsilatRepository = new PoliceTahsilatRepository(_dbContext);

                return _policeTahsilatRepository;
            }
        }

        private PaylasimliPoliceUretimRepository _paylasimliPoliceUretimRepository;
        public PaylasimliPoliceUretimRepository PaylasimliPoliceUretimRepository
        {
            get
            {
                if (_paylasimliPoliceUretimRepository == null)
                    _paylasimliPoliceUretimRepository = new PaylasimliPoliceUretimRepository(_dbContext);

                return _paylasimliPoliceUretimRepository;
            }
        }
        private AutoPoliceTransferRepository _autoPoliceTransferRepository;
        public AutoPoliceTransferRepository AutoPoliceTransferRepository
        {
            get
            {
                if (_autoPoliceTransferRepository == null)
                    _autoPoliceTransferRepository = new AutoPoliceTransferRepository(_dbContext);

                return _autoPoliceTransferRepository;
            }
        }

        private HasarGenelBilgilerRepository _hasarGenelBilgilerRepository;
        public HasarGenelBilgilerRepository HasarGenelBilgilerRepository
        {
            get
            {
                if (_hasarGenelBilgilerRepository == null)
                    _hasarGenelBilgilerRepository = new HasarGenelBilgilerRepository(_dbContext);

                return _hasarGenelBilgilerRepository;
            }
        }

        private HasarAnlasmaliServislerRepository _hasarAnlasmaliServislerRepository;
        public HasarAnlasmaliServislerRepository HasarAnlasmaliServislerRepository
        {
            get
            {
                if (_hasarAnlasmaliServislerRepository == null)
                    _hasarAnlasmaliServislerRepository = new HasarAnlasmaliServislerRepository(_dbContext);

                return _hasarAnlasmaliServislerRepository;
            }
        }

        private HasarAsistansFirmalariRepository _hasarAsistansFirmalariRepository;
        public HasarAsistansFirmalariRepository HasarAsistansFirmalariRepository
        {
            get
            {
                if (_hasarAsistansFirmalariRepository == null)
                    _hasarAsistansFirmalariRepository = new HasarAsistansFirmalariRepository(_dbContext);

                return _hasarAsistansFirmalariRepository;
            }
        }

        private HasarEksperIslemleriRepository _hasarEksperIslemleriRepository;
        public HasarEksperIslemleriRepository HasarEksperIslemleriRepository
        {
            get
            {
                if (_hasarEksperIslemleriRepository == null)
                    _hasarEksperIslemleriRepository = new HasarEksperIslemleriRepository(_dbContext);

                return _hasarEksperIslemleriRepository;
            }
        }

        private HasarEksperListesiRepository _hasarEksperListesiRepository;
        public HasarEksperListesiRepository HasarEksperListesiRepository
        {
            get
            {
                if (_hasarEksperListesiRepository == null)
                    _hasarEksperListesiRepository = new HasarEksperListesiRepository(_dbContext);

                return _hasarEksperListesiRepository;
            }
        }

        private HasarNotlariRepository _hasarNotlariRepository;
        public HasarNotlariRepository HasarNotlariRepository
        {
            get
            {
                if (_hasarNotlariRepository == null)
                    _hasarNotlariRepository = new HasarNotlariRepository(_dbContext);

                return _hasarNotlariRepository;
            }
        }

        private HasarZorunluEvraklariRepository _hasarZorunluEvraklariRepository;
        public HasarZorunluEvraklariRepository HasarZorunluEvraklariRepository
        {
            get
            {
                if (_hasarZorunluEvraklariRepository == null)
                    _hasarZorunluEvraklariRepository = new HasarZorunluEvraklariRepository(_dbContext);

                return _hasarZorunluEvraklariRepository;
            }
        }

        private HasarZorunluEvrakListesiRepository _hasarZorunluEvrakListesiRepository;
        public HasarZorunluEvrakListesiRepository HasarZorunluEvrakListesiRepository
        {
            get
            {
                if (_hasarZorunluEvrakListesiRepository == null)
                    _hasarZorunluEvrakListesiRepository = new HasarZorunluEvrakListesiRepository(_dbContext);

                return _hasarZorunluEvrakListesiRepository;
            }
        }

        private HasarBankaHesaplariRepository _hasarBankaHesaplariRepository;
        public HasarBankaHesaplariRepository HasarBankaHesaplariRepository
        {
            get
            {
                if (_hasarBankaHesaplariRepository == null)
                    _hasarBankaHesaplariRepository = new HasarBankaHesaplariRepository(_dbContext);

                return _hasarBankaHesaplariRepository;
            }
        }

        private HasarIletisimYetkilileriRepository _hasarIletisimYetkilileriRepository;
        public HasarIletisimYetkilileriRepository HasarIletisimYetkilileriRepository
        {
            get
            {
                if (_hasarIletisimYetkilileriRepository == null)
                    _hasarIletisimYetkilileriRepository = new HasarIletisimYetkilileriRepository(_dbContext);

                return _hasarIletisimYetkilileriRepository;
            }
        }

        private PoliceTransferReaderKullanicilariRepository _policeTransferReaderKullanicilari;
        public PoliceTransferReaderKullanicilariRepository PoliceTransferReaderKullanicilariRepository
        {
            get
            {
                if (_policeTransferReaderKullanicilari == null)

                    _policeTransferReaderKullanicilari = new PoliceTransferReaderKullanicilariRepository(_dbContext);
                return _policeTransferReaderKullanicilari;
            }
        }

        private KimlikNoUretRepository _kimlikNoUretRepository;
        public KimlikNoUretRepository KimlikNoUretRepository
        {
            get
            {
                if (_kimlikNoUretRepository == null)
                    _kimlikNoUretRepository = new KimlikNoUretRepository(_dbContext);

                return _kimlikNoUretRepository;
            }
        }
        private PoliceDokumanRepository _PoliceDokumanRepository;
        public PoliceDokumanRepository PoliceDokumanRepository
        {
            get
            {
                if (_PoliceDokumanRepository == null)
                    _PoliceDokumanRepository = new PoliceDokumanRepository(_dbContext);

                return _PoliceDokumanRepository;
            }
        }
        private SigortaSirketleriRepository _SigortaSirketleriRepository;
        public SigortaSirketleriRepository SigortaSirketleriRepository
        {
            get
            {
                if (_SigortaSirketleriRepository == null)
                    _SigortaSirketleriRepository = new SigortaSirketleriRepository(_dbContext);

                return _SigortaSirketleriRepository;
            }
        }

        #endregion
    }
}
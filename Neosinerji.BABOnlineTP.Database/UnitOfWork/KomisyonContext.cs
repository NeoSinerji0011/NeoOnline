using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.UnitOfWork
{
    public interface IKomisyonContext : IUnitOfWork
    {
        TaliAcenteKomisyonOraniRepository TaliAcenteKomisyonOraniRepository { get; }
        PoliceGenelRepository PoliceGenelRepository { get; }
        PoliceTaliAcentelerRepository PoliceTaliAcentelerRepository { get; }
        PoliceTaliAcenteRaporRepository PoliceTaliAcenteRaporRepository { get; }
        PoliceUretimHedefPlanlananRepository PoliceUretimHedefPlanlananRepository { get; }
        PoliceUretimHedefGerceklesenRepository PoliceUretimHedefGerceklesenRepository { get; }
        OtoLoginSigortaSirketKullanicilarRepository OtoLoginSigortaSirketKullanicilarRepository { get; }
        NeoConnectTvmSirketYetkileriRepository NeoConnectTvmSirketYetkileriRepository { get; }
        NeoConnectYasakliUrllerRepository NeoConnectYasakliUrllerRepository { get; }
        NeoConnectSirketGrupKullaniciDetayRepository NeoConnectSirketGrupKullaniciDetayRepository { get; }

    }

    public class KomisyonContext : IKomisyonContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public KomisyonContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
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

        #region IKomisyonContext

        private TaliAcenteKomisyonOraniRepository _taliAcenteKomisyonOraniRepository;
        public TaliAcenteKomisyonOraniRepository TaliAcenteKomisyonOraniRepository
        {
            get
            {
                if (_taliAcenteKomisyonOraniRepository == null)
                    _taliAcenteKomisyonOraniRepository = new TaliAcenteKomisyonOraniRepository(_dbContext);

                return _taliAcenteKomisyonOraniRepository;
            }
        }

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


        private PoliceTaliAcentelerRepository _policeTaliAcentelerRepository;
        public PoliceTaliAcentelerRepository PoliceTaliAcentelerRepository
        {
            get
            {
                if (_policeTaliAcentelerRepository == null)
                    _policeTaliAcentelerRepository = new PoliceTaliAcentelerRepository(_dbContext);

                return _policeTaliAcentelerRepository;
            }
        }

        private PoliceTaliAcenteRaporRepository _policeTaliAcenteRaporRepository;
        public PoliceTaliAcenteRaporRepository PoliceTaliAcenteRaporRepository
        {
            get
            {
                if (_policeTaliAcenteRaporRepository == null)
                    _policeTaliAcenteRaporRepository = new PoliceTaliAcenteRaporRepository(_dbContext);

                return _policeTaliAcenteRaporRepository;
            }
        }

        private PoliceUretimHedefPlanlananRepository _policeUretimHedefPlanlananRepository;
        public PoliceUretimHedefPlanlananRepository PoliceUretimHedefPlanlananRepository
        {
            get
            {
                if (_policeUretimHedefPlanlananRepository == null)
                    _policeUretimHedefPlanlananRepository = new PoliceUretimHedefPlanlananRepository(_dbContext);

                return _policeUretimHedefPlanlananRepository;
            }
        }

        private PoliceUretimHedefGerceklesenRepository _policeUretimHedefGerceklesenRepository;
        public PoliceUretimHedefGerceklesenRepository PoliceUretimHedefGerceklesenRepository
        {
            get
            {
                if (_policeUretimHedefGerceklesenRepository == null)
                    _policeUretimHedefGerceklesenRepository = new PoliceUretimHedefGerceklesenRepository(_dbContext);

                return _policeUretimHedefGerceklesenRepository;
            }
        }

        private OtoLoginSigortaSirketKullanicilarRepository _otoLoginSigortaSirketKullanicilarRepository;
        public OtoLoginSigortaSirketKullanicilarRepository OtoLoginSigortaSirketKullanicilarRepository
        {
            get
            {
                if (_otoLoginSigortaSirketKullanicilarRepository == null)
                    _otoLoginSigortaSirketKullanicilarRepository = new OtoLoginSigortaSirketKullanicilarRepository(_dbContext);

                return _otoLoginSigortaSirketKullanicilarRepository;
            }
        }

        private NeoConnectTvmSirketYetkileriRepository _neoConnectTvmSirketYetkileriRepository;
        public NeoConnectTvmSirketYetkileriRepository NeoConnectTvmSirketYetkileriRepository
        {
            get
            {
                if (_neoConnectTvmSirketYetkileriRepository == null)
                    _neoConnectTvmSirketYetkileriRepository = new NeoConnectTvmSirketYetkileriRepository(_dbContext);

                return _neoConnectTvmSirketYetkileriRepository;
            }
        }

        private NeoConnectYasakliUrllerRepository _neoConnectYasakliUrllerRepository;
        public NeoConnectYasakliUrllerRepository NeoConnectYasakliUrllerRepository
        {
            get
            {
                if (_neoConnectYasakliUrllerRepository == null)
                    _neoConnectYasakliUrllerRepository = new NeoConnectYasakliUrllerRepository(_dbContext);

                return _neoConnectYasakliUrllerRepository;
            }
        }

        private NeoConnectSirketGrupKullaniciDetayRepository _neoConnectSirketGrupKullaniciDetayRepository;
        public NeoConnectSirketGrupKullaniciDetayRepository NeoConnectSirketGrupKullaniciDetayRepository
        {
            get
            {
                if (_neoConnectSirketGrupKullaniciDetayRepository == null)
                    _neoConnectSirketGrupKullaniciDetayRepository = new NeoConnectSirketGrupKullaniciDetayRepository(_dbContext);

                return _neoConnectSirketGrupKullaniciDetayRepository;
            }
        }

        
        #endregion
    }
}

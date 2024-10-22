using System;
using System.Collections.Generic;
using System.Data;
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
    public interface IMuhasebeContext : IUnitOfWork
    {
      
        CariHareketleriRepository CariHareketleriRepository { get; }
        CariHesapBorcAlacakRepository CariHesapBorcAlacakRepository { get; }
        CariHesaplariRepository CariHesaplariRepository { get; }
        MuhasebeAktarimKonfigurasyonRepository MuhasebeAktarimKonfigurasyonRepository { get; }
        CariEvrakTipleriRepository CariEvrakTipleriRepository { get; }
        CariAktarimLogRepository CariAktarimLogRepository { get; }
        CariOdemeTipleriRepository CariOdemeTipleriRepository { get; }

        CariHesapBorcalacakPeocedureModels BorcAlacak_Getir(int tvmkodu, int donem, string cariHesapKodu);
    }
    public class MuhasebeContext : IMuhasebeContext
    {
        private DbContext _dbContext;
        private bool _disposed;

        public MuhasebeContext(IDbContextFactory dbContextFactory)
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
     
        private CariHareketleriRepository _cariHareketleriRepository;
        public CariHareketleriRepository CariHareketleriRepository
        {
            get
            {
                if (_cariHareketleriRepository == null)
                {
                    _cariHareketleriRepository = new CariHareketleriRepository(_dbContext);
                }
                return _cariHareketleriRepository;
            }
        }
        private CariOdemeTipleriRepository _cariOdemeTipleriRepository;
        public CariOdemeTipleriRepository CariOdemeTipleriRepository
        {
            get
            {
                if (_cariOdemeTipleriRepository == null)
                {
                    _cariOdemeTipleriRepository = new CariOdemeTipleriRepository(_dbContext);
                }
                return _cariOdemeTipleriRepository;
            }
        }
        private CariHesapBorcAlacakRepository _cariHesapBorcAlacakRepository;
        public CariHesapBorcAlacakRepository CariHesapBorcAlacakRepository
        {
            get
            {
                if (_cariHesapBorcAlacakRepository == null)
                {
                    _cariHesapBorcAlacakRepository = new CariHesapBorcAlacakRepository(_dbContext);
                }
                return _cariHesapBorcAlacakRepository;
            }
        }

        private CariHesaplariRepository _cariHesaplariRepository;
        public CariHesaplariRepository CariHesaplariRepository
        {
            get
            {
                if (_cariHesaplariRepository == null)
                {
                    _cariHesaplariRepository = new CariHesaplariRepository(_dbContext);
                }
                return _cariHesaplariRepository;
            }
        }
        private MuhasebeAktarimKonfigurasyonRepository _muhasebeAktarimKonfigurasyonRepository;
        public MuhasebeAktarimKonfigurasyonRepository MuhasebeAktarimKonfigurasyonRepository
        {
            get
            {
                if (_muhasebeAktarimKonfigurasyonRepository == null)
                    _muhasebeAktarimKonfigurasyonRepository = new MuhasebeAktarimKonfigurasyonRepository(_dbContext);

                return _muhasebeAktarimKonfigurasyonRepository;
            }
        }
        private CariEvrakTipleriRepository _cariEvrakTipleriRepository;
        public CariEvrakTipleriRepository CariEvrakTipleriRepository
        {
            get
            {
                if (_cariEvrakTipleriRepository == null)
                {
                    _cariEvrakTipleriRepository = new CariEvrakTipleriRepository(_dbContext);
                }
                return _cariEvrakTipleriRepository;
            }
        }

        private CariAktarimLogRepository _cariAktarimLogRepository;
        public CariAktarimLogRepository CariAktarimLogRepository
        {
            get
            {
                if (_cariAktarimLogRepository == null)
                {
                    _cariAktarimLogRepository = new CariAktarimLogRepository(_dbContext);
                }
                return _cariAktarimLogRepository;
            }
        }

        // ==== Şube Raporu Getirir ====//
        public CariHesapBorcalacakPeocedureModels BorcAlacak_Getir(int tvmkodu, int donem, string cariHesapKodu)
        {
          

            SqlParameter tvmKodu = new SqlParameter("@TvmKodu", SqlDbType.Int);
            tvmKodu.Value = tvmkodu;

            SqlParameter Donem = new SqlParameter("@Donem", SqlDbType.Int);
            Donem.Value = donem;

            SqlParameter CariHesapKodu = new SqlParameter("@CariHesapKodu", SqlDbType.NVarChar,20);
            CariHesapKodu.Value = !String.IsNullOrEmpty(cariHesapKodu) ? cariHesapKodu :"";

            CariHesapBorcalacakPeocedureModels model = _dbContext.Database.SqlQuery<CariHesapBorcalacakPeocedureModels>
                                                       ("SP_CariHesapBorAlacak @TvmKodu, @Donem, @CariHesapKodu",
                                                        tvmKodu, Donem, CariHesapKodu)
                                                       .FirstOrDefault();
            return model;
        }

    }
}

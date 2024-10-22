using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Neosinerji.BABOnlineTP.Database.Repository;
using System.Data.SqlClient;
using System.Linq;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface IGorevTakipContext : IUnitOfWork
    {
        List<IslerimProcedureModel> SP_Islerim(int isAlanTvmKodu, int isAlanTvmKullanici, string durumList);
        List<AtananIslerProcedureModel> GorevDagilimRaporu(DateTime? atamaBaslangicTarihi, DateTime? atamaBitisTarihi, int? isAlanTvmKodu, int? isAlanTvmKullanici,
                                                                         byte? isTipi, byte? durum, byte? oncelikSeviyesi);

        AtananIsNotlarRepository AtananIsNotlarRepository { get; }
        IsTipleriRepository IsTipleriRepository { get; }
        AtananIslerRepository AtananIslerRepository { get; }
        AtananIsLogRepository AtananIsLogRepository { get; }
        TalepKanallariRepository TalepKanallariRepository { get; }
        AtananIsDokumanlarRepository AtananIsDokumanlarRepository { get; }
    }
    public class GorevTakipContext : IGorevTakipContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;
        public GorevTakipContext(IDbContextFactory dbContextFactory)
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

        #region Repository Encapsulation
        private AtananIsNotlarRepository _atananIsNotlarRepository;
        public AtananIsNotlarRepository AtananIsNotlarRepository
        {
            get
            {
                if (_atananIsNotlarRepository == null)
                {
                    _atananIsNotlarRepository = new AtananIsNotlarRepository(_dbContext);
                }
                return _atananIsNotlarRepository;
            }
        }

        private IsTipleriRepository _isTipleriRepository;
        public IsTipleriRepository IsTipleriRepository
        {
            get
            {
                if (_isTipleriRepository == null)
                    _isTipleriRepository = new IsTipleriRepository(_dbContext);

                return _isTipleriRepository;
            }
        }

        private AtananIslerRepository _atananIslerRepository;
        public AtananIslerRepository AtananIslerRepository
        {
            get
            {
                if (_atananIslerRepository == null)
                    _atananIslerRepository = new AtananIslerRepository(_dbContext);

                return _atananIslerRepository;
            }
        }

        private AtananIsLogRepository _atananIsLogRepository;
        public AtananIsLogRepository AtananIsLogRepository
        {
            get
            {
                if (_atananIsLogRepository == null)
                    _atananIsLogRepository = new AtananIsLogRepository(_dbContext);

                return _atananIsLogRepository;
            }
        }

        private TalepKanallariRepository _talepKanallariRepository;
        public TalepKanallariRepository TalepKanallariRepository
        {
            get
            {
                if (_talepKanallariRepository == null)
                    _talepKanallariRepository = new TalepKanallariRepository(_dbContext);

                return _talepKanallariRepository;
            }
        }

        private AtananIsDokumanlarRepository _AtananIsDokumanlarRepository;
        public AtananIsDokumanlarRepository AtananIsDokumanlarRepository
        {
            get
            {
                if (_AtananIsDokumanlarRepository == null)
                    _AtananIsDokumanlarRepository = new AtananIsDokumanlarRepository(_dbContext);

                return _AtananIsDokumanlarRepository;
            }
        }
        #endregion

        #region Stored Procedure

        public List<IslerimProcedureModel> SP_Islerim(int isAlanTvmKodu, int isAlanTvmKullanici, string durumList)
        {
            SqlParameter tvmKodlar = new SqlParameter("@IsAlanTVMKodu", SqlDbType.Int);
            tvmKodlar.Value = isAlanTvmKodu;

            SqlParameter tvmKulKodlar = new SqlParameter("@IsAlanKullaniciKodu", SqlDbType.Int);
            tvmKulKodlar.Value = isAlanTvmKullanici;

            SqlParameter durumListe = new SqlParameter("@DurumList", SqlDbType.NVarChar, int.MaxValue);
            durumListe.Value = durumList ?? "";

            List<IslerimProcedureModel> isList = _dbContext.Database.SqlQuery<IslerimProcedureModel>
                                                                ("SP_Islerim @IsAlanTVMKodu, @IsAlanKullaniciKodu, @DurumList",
                                                                tvmKodlar, tvmKulKodlar, durumListe)
                                                                 .ToList<IslerimProcedureModel>();

            return isList;
        }

        // ==== Atanan Is Listesi ====//
        public List<AtananIslerProcedureModel> GorevDagilimRaporu(DateTime? atamaBaslangicTarihi, DateTime? atamaBitisTarihi, int? isAlanTvmKodu, int? isAlanTvmKullanici,
                                                                          byte? isTipi, byte? durum, byte? oncelikSeviyesi)
        {

            SqlParameter tvmKodlar = new SqlParameter("@IsAlanTVMKodu", SqlDbType.Int);
            tvmKodlar.Value = isAlanTvmKodu.Value;

            SqlParameter tvmKulKodlar = new SqlParameter("@IsAlanKullaniciKodu", SqlDbType.Int);
            tvmKulKodlar.Value = isAlanTvmKullanici.HasValue ? isAlanTvmKullanici.Value : 0;

            SqlParameter isTip = new SqlParameter("@IsTipi", SqlDbType.TinyInt);
            isTip.Value = isTipi ?? 0;

            SqlParameter drm = new SqlParameter("@Durum", SqlDbType.TinyInt);
            drm.Value = durum ?? 0;

            SqlParameter oncelikSeviye = new SqlParameter("@OncelikSeviyesi", SqlDbType.TinyInt);
            oncelikSeviye.Value = oncelikSeviyesi ?? 0;

            SqlParameter baslamaT = new SqlParameter("@AtamaBasTarihi", SqlDbType.Date);
            baslamaT.Value = atamaBaslangicTarihi;

            SqlParameter bitisT = new SqlParameter("@AtamaBitTarihi", SqlDbType.Date);
            bitisT.Value = atamaBitisTarihi;
            List<AtananIslerProcedureModel> isList = _dbContext.Database.SqlQuery<AtananIslerProcedureModel>
                                                                ("SP_GorevDagilimRaporu @IsAlanTVMKodu, @IsAlanKullaniciKodu, @IsTipi,@Durum,@OncelikSeviyesi,@AtamaBasTarihi,@AtamaBitTarihi",
                                                                tvmKodlar, tvmKulKodlar, isTip, drm, oncelikSeviye, baslamaT, bitisT)
                                                                 .ToList<AtananIslerProcedureModel>();

            return isList;
        }

        #endregion
    }
}

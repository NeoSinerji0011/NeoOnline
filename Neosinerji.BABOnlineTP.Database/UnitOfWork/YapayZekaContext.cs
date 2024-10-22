using Neosinerji.BABOnlineTP.Database.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.SqlClient;
using System.Data;
using Neosinerji.BABOnlineTP.Database.Common;

namespace Neosinerji.BABOnlineTP.Database.UnitOfWork
{
    public interface IYapayZekaContext : IUnitOfWork
    {
        YZ_BelediyeNufusRepository YZ_BelediyeNufusRepository { get; }
        YZ_BuyuksehirNufusRepository YZ_BuyuksehirNufusRepository { get; }
        YZ_IlceKoordinatRepository YZ_IlceKoordinatRepository { get; }
        YZ_IlceNufusRepository YZ_IlceNufusRepository { get; }
        YZ_IlceYuzolcumRepository YZ_IlceYuzolcumRepository { get; }
        YZ_IlKoordinatRepository YZ_IlKoordinatRepository { get; }
        YZ_IlNufusRepository YZ_IlNufusRepository { get; }
        YZ_IlYuzolcumRepository YZ_IlYuzolcumRepository { get; }
        YZ_KoyKoordinatRepository YZ_KoyKoordinatRepository { get; }
        YZ_KoyNufusRepository YZ_KoyNufusRepository { get; }
        YZ_MahalleKoordinatRepository YZ_MahalleKoordinatRepository { get; }
        YZ_MahalleNufusRepository YZ_MahalleNufusRepository { get; }
    }

    public class YapayZekaContext : IYapayZekaContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public YapayZekaContext(IDbContextFactory dbContextFactory)
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

        #region IYapayZekaContext

        private YZ_BelediyeNufusRepository _YZ_BelediyeNufusRepository;
        public YZ_BelediyeNufusRepository YZ_BelediyeNufusRepository
        {
            get
            {
                if (_YZ_BelediyeNufusRepository == null)
                    _YZ_BelediyeNufusRepository = new YZ_BelediyeNufusRepository(_dbContext);

                return _YZ_BelediyeNufusRepository;
            }
        }

        private YZ_BuyuksehirNufusRepository _YZ_BuyuksehirNufusRepository;
        public YZ_BuyuksehirNufusRepository YZ_BuyuksehirNufusRepository
        {
            get
            {
                if (_YZ_BuyuksehirNufusRepository == null)
                    _YZ_BuyuksehirNufusRepository = new YZ_BuyuksehirNufusRepository(_dbContext);

                return _YZ_BuyuksehirNufusRepository;
            }
        }


        private YZ_IlceKoordinatRepository _YZ_IlceKoordinatRepository;
        public YZ_IlceKoordinatRepository YZ_IlceKoordinatRepository
        {
            get
            {
                if (_YZ_IlceKoordinatRepository == null)
                    _YZ_IlceKoordinatRepository = new YZ_IlceKoordinatRepository(_dbContext);

                return _YZ_IlceKoordinatRepository;
            }
        }

        private YZ_IlceNufusRepository _YZ_IlceNufusRepository;
        public YZ_IlceNufusRepository YZ_IlceNufusRepository
        {
            get
            {
                if (_YZ_IlceNufusRepository == null)
                    _YZ_IlceNufusRepository = new YZ_IlceNufusRepository(_dbContext);

                return _YZ_IlceNufusRepository;
            }
        }

        private YZ_IlceYuzolcumRepository _YZ_IlceYuzolcumRepository;
        public YZ_IlceYuzolcumRepository YZ_IlceYuzolcumRepository
        {
            get
            {
                if (_YZ_IlceYuzolcumRepository == null)
                    _YZ_IlceYuzolcumRepository = new YZ_IlceYuzolcumRepository(_dbContext);

                return _YZ_IlceYuzolcumRepository;
            }
        }

        private YZ_IlKoordinatRepository _YZ_IlKoordinatRepository;
        public YZ_IlKoordinatRepository YZ_IlKoordinatRepository
        {
            get
            {
                if (_YZ_IlKoordinatRepository == null)
                    _YZ_IlKoordinatRepository = new YZ_IlKoordinatRepository(_dbContext);

                return _YZ_IlKoordinatRepository;
            }
        }

        private YZ_IlNufusRepository _YZ_IlNufusRepository;
        public YZ_IlNufusRepository YZ_IlNufusRepository
        {
            get
            {
                if (_YZ_IlNufusRepository == null)
                    _YZ_IlNufusRepository = new YZ_IlNufusRepository(_dbContext);

                return _YZ_IlNufusRepository;
            }
        }

        private YZ_IlYuzolcumRepository _YZ_IlYuzolcumRepository;
        public YZ_IlYuzolcumRepository YZ_IlYuzolcumRepository
        {
            get
            {
                if (_YZ_IlYuzolcumRepository == null)
                    _YZ_IlYuzolcumRepository = new YZ_IlYuzolcumRepository(_dbContext);

                return _YZ_IlYuzolcumRepository;
            }
        }

        private YZ_KoyKoordinatRepository _YZ_KoyKoordinatRepository;
        public YZ_KoyKoordinatRepository YZ_KoyKoordinatRepository
        {
            get
            {
                if (_YZ_KoyKoordinatRepository == null)
                    _YZ_KoyKoordinatRepository = new YZ_KoyKoordinatRepository(_dbContext);

                return _YZ_KoyKoordinatRepository;
            }
        }


        private YZ_KoyNufusRepository _YZ_KoyNufusRepository;
        public YZ_KoyNufusRepository YZ_KoyNufusRepository
        {
            get
            {
                if (_YZ_KoyNufusRepository == null)
                    _YZ_KoyNufusRepository = new YZ_KoyNufusRepository(_dbContext);

                return _YZ_KoyNufusRepository;
            }
        }

        private YZ_MahalleKoordinatRepository _YZ_MahalleKoordinatRepository;
        public YZ_MahalleKoordinatRepository YZ_MahalleKoordinatRepository
        {
            get
            {
                if (_YZ_MahalleKoordinatRepository == null)
                    _YZ_MahalleKoordinatRepository = new YZ_MahalleKoordinatRepository(_dbContext);

                return _YZ_MahalleKoordinatRepository;
            }
        }

        private YZ_MahalleNufusRepository _YZ_MahalleNufusRepository;
        public YZ_MahalleNufusRepository YZ_MahalleNufusRepository
        {
            get
            {
                if (_YZ_MahalleNufusRepository == null)
                    _YZ_MahalleNufusRepository = new YZ_MahalleNufusRepository(_dbContext);

                return _YZ_MahalleNufusRepository;
            }
        }


        #endregion
    }
}

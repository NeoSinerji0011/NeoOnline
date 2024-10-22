using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface IMusteriContext : IUnitOfWork
    {
        MusteriAdreRepository MusteriAdreRepository { get; }
        MusteriDokumanRepository MusteriDokumanRepository { get; }
        MusteriGenelBilgilerRepository MusteriGenelBilgilerRepository { get; }
        MusteriNotRepository MusteriNotRepository { get; }
        MusteriTelefonRepository MusteriTelefonRepository { get; }

        //Potansiyel Müşteri
        PotansiyelMusteriGenelBilgilerRepository PotansiyelMusteriGenelBilgilerRepository { get; }
        PotansiyelMusteriAdresRepository PotansiyelMusteriAdresRepository { get; }
        PotansiyelMusteriDokumanRepository PotansiyelMusteriDokumanRepository { get; }
        PotansiyelMusteriNotRepository PotansiyelMusteriNotRepository { get; }
        PotansiyelMusteriTelefonRepository PotansiyelMusteriTelefonRepository { get; }

        TCKNRepository TCKNRepository { get; }
        VKNRepository VKNRepository { get; }
        YKNRepository YKNRepository { get; }

        TVMDetayRepository TVMDetayRepository { get; }
    }

    public class MusteriContext : IMusteriContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        private MusteriAdreRepository _musteriAdreRepository;
        private MusteriDokumanRepository _musteriDokumanRepository;
        private MusteriGenelBilgilerRepository _musteriGenelBilgilerRepository;
        private MusteriNotRepository _musteriNotRepository;
        private MusteriTelefonRepository _musteriTelefonRepository;

        //Potansiyelm Müşteri
        private PotansiyelMusteriGenelBilgilerRepository _potansiyelMusteriGenelBilgilerRepository;
        private PotansiyelMusteriAdresRepository _potansiyelMusteriAdresRepository;
        private PotansiyelMusteriDokumanRepository _potansiyelMusteriDokumanRepository;
        private PotansiyelMusteriNotRepository _potansiyelMusteriNotRepository;
        private PotansiyelMusteriTelefonRepository _potansiyelMusteriTelefonRepository;

        private TCKNRepository _tCKNRepository;
        private VKNRepository _vKNRepository;
        private YKNRepository _yKNRepository;
        private TVMDetayRepository _TVMDetayRepository;


        public MusteriContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
        }

        public void Commit()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("DbEntityValidationException: ", dex.Message);
                sb.AppendLine();
                foreach (var item in dex.EntityValidationErrors)
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

        public MusteriAdreRepository MusteriAdreRepository
        {
            get
            {
                if (_musteriAdreRepository == null)
                {
                    _musteriAdreRepository = new MusteriAdreRepository(_dbContext);
                }
                return _musteriAdreRepository;
            }
        }

        public MusteriDokumanRepository MusteriDokumanRepository
        {
            get
            {
                if (_musteriDokumanRepository == null)
                {
                    _musteriDokumanRepository = new MusteriDokumanRepository(_dbContext);
                }
                return _musteriDokumanRepository;
            }
        }

        public MusteriGenelBilgilerRepository MusteriGenelBilgilerRepository
        {
            get
            {
                if (_musteriGenelBilgilerRepository == null)
                {
                    _musteriGenelBilgilerRepository = new MusteriGenelBilgilerRepository(_dbContext);
                }
                return _musteriGenelBilgilerRepository;
            }
        }

        public MusteriNotRepository MusteriNotRepository
        {
            get
            {
                if (_musteriNotRepository == null)
                {
                    _musteriNotRepository = new MusteriNotRepository(_dbContext);
                }
                return _musteriNotRepository;
            }
        }

        public MusteriTelefonRepository MusteriTelefonRepository
        {
            get
            {
                if (_musteriTelefonRepository == null)
                {
                    _musteriTelefonRepository = new MusteriTelefonRepository(_dbContext);
                }
                return _musteriTelefonRepository;
            }
        }

        //Potansiyel Müsteri

        public PotansiyelMusteriGenelBilgilerRepository PotansiyelMusteriGenelBilgilerRepository
        {
            get
            {
                if (_potansiyelMusteriGenelBilgilerRepository == null)
                {
                    _potansiyelMusteriGenelBilgilerRepository = new PotansiyelMusteriGenelBilgilerRepository(_dbContext);
                }
                return _potansiyelMusteriGenelBilgilerRepository;
            }
        }
        public PotansiyelMusteriAdresRepository PotansiyelMusteriAdresRepository
        {
            get
            {
                if (_potansiyelMusteriAdresRepository == null)
                {
                    _potansiyelMusteriAdresRepository = new PotansiyelMusteriAdresRepository(_dbContext);
                }
                return _potansiyelMusteriAdresRepository;
            }
        }
        public PotansiyelMusteriDokumanRepository PotansiyelMusteriDokumanRepository
        {
            get
            {
                if (_potansiyelMusteriDokumanRepository == null)
                {
                    _potansiyelMusteriDokumanRepository = new PotansiyelMusteriDokumanRepository(_dbContext);
                }
                return _potansiyelMusteriDokumanRepository;
            }
        }
        public PotansiyelMusteriNotRepository PotansiyelMusteriNotRepository
        {
            get
            {
                if (_potansiyelMusteriNotRepository == null)
                {
                    _potansiyelMusteriNotRepository = new PotansiyelMusteriNotRepository(_dbContext);
                }
                return _potansiyelMusteriNotRepository;
            }
        }
        public PotansiyelMusteriTelefonRepository PotansiyelMusteriTelefonRepository
        {
            get
            {
                if (_potansiyelMusteriTelefonRepository == null)
                {
                    _potansiyelMusteriTelefonRepository = new PotansiyelMusteriTelefonRepository(_dbContext);
                }
                return _potansiyelMusteriTelefonRepository;
            }
        }


        public TCKNRepository TCKNRepository
        {
            get
            {
                if (_tCKNRepository == null)
                {
                    _tCKNRepository = new TCKNRepository(_dbContext);
                }
                return _tCKNRepository;
            }
        }

        public VKNRepository VKNRepository
        {
            get
            {
                if (_vKNRepository == null)
                {
                    _vKNRepository = new VKNRepository(_dbContext);
                }
                return _vKNRepository;
            }
        }

        public YKNRepository YKNRepository
        {
            get
            {
                if (_yKNRepository == null)
                {
                    _yKNRepository = new YKNRepository(_dbContext);
                }
                return _yKNRepository;
            }
        }

        public TVMDetayRepository TVMDetayRepository
        {
            get
            {
                if (_TVMDetayRepository == null)
                {
                    _TVMDetayRepository = new TVMDetayRepository(_dbContext);
                }
                return _TVMDetayRepository;
            }
        }

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
    }
}

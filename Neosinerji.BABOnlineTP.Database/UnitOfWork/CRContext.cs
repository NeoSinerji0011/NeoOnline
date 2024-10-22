using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface ICRContext : IUnitOfWork
    {
        CR_AracEkSoruRepository CR_AracEkSoruRepository { get; }
        CR_AracGrupRepository CR_AracGrupRepository { get; }
        CR_IlIlceRepository CR_IlIlceRepository { get; }
        CR_UlkeRepository CR_UlkeRepository { get; }
        CR_TescilIlIlceRepository CR_TescilIlIlceRepository { get; }
        CR_TrafikFKRepository CR_TrafikFKRepository { get; }
        CR_TrafikIMMRepository CR_TrafikIMMRepository { get; }
        CR_KullanimTarziRepository CR_KullanimTarziRepository { get; }
        CR_TUMMusteriRepository CR_TUMMusteriRepository { get; }
        CR_KaskoFKRepository CR_KaskoFKRepository { get; }
        CR_KaskoIMMRepository CR_KaskoIMMRepository { get; }
        CR_KaskoAMSRepository CR_KaskoAMSRepository { get; }
        CR_KaskoIkameTuruRepository CR_KaskoIkameTuruRepository { get; }
        CR_KrediHayatCarpanRepository CR_KrediHayatCarpanRepository { get; }
        CR_KaskoDMRepository CR_KaskoDMRepository { get; }

        TrafikFKRepository TrafikFKRepository { get; }
        TrafikIMMRepository TrafikIMMRepository { get; }
        KaskoIMMRepository KaskoIMMRepository { get; }
        KaskoFKRepository KaskoFKRepository { get; }

        MeslekIndirimiKaskoRepository MeslekIndirimiKaskoRepository { get; }
        CR_MeslekIndirimiKaskoRepository CR_MeslekIndirimiKaskoRepository { get; }
        UnicoIlIlceRepository UnicoIlIlceRepository { get; }
        ErgoIlIlceRepository ErgoIlIlceRepository { get; }        
            
    }

    public class CRContext : ICRContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        public CRContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
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

        #region ICRContext
        private CR_AracEkSoruRepository _CR_AracEkSoruRepository;
        public CR_AracEkSoruRepository CR_AracEkSoruRepository
        {
            get
            {
                if (_CR_AracEkSoruRepository == null)
                {
                    _CR_AracEkSoruRepository = new CR_AracEkSoruRepository(_dbContext);
                }
                return _CR_AracEkSoruRepository;
            }
        }

        private CR_AracGrupRepository _CR_AracGrupRepository;
        public CR_AracGrupRepository CR_AracGrupRepository
        {
            get
            {
                if (_CR_AracGrupRepository == null)
                {
                    _CR_AracGrupRepository = new CR_AracGrupRepository(_dbContext);
                }
                return _CR_AracGrupRepository;
            }
        }

        private CR_IlIlceRepository _CR_IlIlceRepository;
        public CR_IlIlceRepository CR_IlIlceRepository
        {
            get
            {
                if (_CR_IlIlceRepository == null)
                {
                    _CR_IlIlceRepository = new CR_IlIlceRepository(_dbContext);
                }
                return _CR_IlIlceRepository;
            }
        }

        private CR_UlkeRepository _CR_UlkeRepository;
        public CR_UlkeRepository CR_UlkeRepository
        {
            get
            {
                if (_CR_UlkeRepository == null)
                {
                    _CR_UlkeRepository = new CR_UlkeRepository(_dbContext);
                }
                return _CR_UlkeRepository;
            }
        }

        private CR_TescilIlIlceRepository _CR_TescilIlIlceRepository;
        public CR_TescilIlIlceRepository CR_TescilIlIlceRepository
        {
            get
            {
                if (_CR_TescilIlIlceRepository == null)
                {
                    _CR_TescilIlIlceRepository = new CR_TescilIlIlceRepository(_dbContext);
                }
                return _CR_TescilIlIlceRepository;
            }
        }

        private CR_TrafikFKRepository _CR_TrafikFKRepository;
        public CR_TrafikFKRepository CR_TrafikFKRepository
        {
            get
            {
                if (_CR_TrafikFKRepository == null)
                {
                    _CR_TrafikFKRepository = new CR_TrafikFKRepository(_dbContext);
                }
                return _CR_TrafikFKRepository;
            }
        }

        private CR_TrafikIMMRepository _CR_TrafikIMMRepository;
        public CR_TrafikIMMRepository CR_TrafikIMMRepository
        {
            get
            {
                if (_CR_TrafikIMMRepository == null)
                {
                    _CR_TrafikIMMRepository = new CR_TrafikIMMRepository(_dbContext);
                }
                return _CR_TrafikIMMRepository;
            }
        }

        private CR_KullanimTarziRepository _CR_KullanimTarziRepository;
        public CR_KullanimTarziRepository CR_KullanimTarziRepository
        {
            get
            {
                if (_CR_KullanimTarziRepository == null)
                {
                    _CR_KullanimTarziRepository = new CR_KullanimTarziRepository(_dbContext);
                }
                return _CR_KullanimTarziRepository;
            }
        }

        private CR_TUMMusteriRepository _CR_TUMMusteriRepository;
        public CR_TUMMusteriRepository CR_TUMMusteriRepository
        {
            get
            {
                if (_CR_TUMMusteriRepository == null)
                {
                    _CR_TUMMusteriRepository = new CR_TUMMusteriRepository(_dbContext);
                }
                return _CR_TUMMusteriRepository;
            }
        }

        private CR_KaskoFKRepository _CR_KaskoFKRepository;
        public CR_KaskoFKRepository CR_KaskoFKRepository
        {
            get
            {
                if (_CR_KaskoFKRepository == null)
                    _CR_KaskoFKRepository = new CR_KaskoFKRepository(_dbContext);

                return _CR_KaskoFKRepository;
            }
        }

        private CR_KaskoIMMRepository _CR_KaskoIMMRepository;
        public CR_KaskoIMMRepository CR_KaskoIMMRepository
        {
            get
            {
                if (_CR_KaskoIMMRepository == null)
                    _CR_KaskoIMMRepository = new CR_KaskoIMMRepository(_dbContext);

                return _CR_KaskoIMMRepository;
            }
        }

        private CR_KaskoAMSRepository _CR_KaskoAMSRepository;
        public CR_KaskoAMSRepository CR_KaskoAMSRepository
        {
            get
            {
                if (_CR_KaskoAMSRepository == null)
                    _CR_KaskoAMSRepository = new CR_KaskoAMSRepository(_dbContext);

                return _CR_KaskoAMSRepository;
            }
        }

        private CR_KaskoIkameTuruRepository _CR_KaskoIkameTuruRepository;
        public CR_KaskoIkameTuruRepository CR_KaskoIkameTuruRepository
        {
            get
            {
                if (_CR_KaskoIkameTuruRepository == null)
                    _CR_KaskoIkameTuruRepository = new CR_KaskoIkameTuruRepository(_dbContext);

                return _CR_KaskoIkameTuruRepository;
            }
        }

        private CR_KrediHayatCarpanRepository _CR_KrediHayatCarpanRepository;
        public CR_KrediHayatCarpanRepository CR_KrediHayatCarpanRepository
        {
            get
            {
                if (_CR_KrediHayatCarpanRepository == null)
                    _CR_KrediHayatCarpanRepository = new CR_KrediHayatCarpanRepository(_dbContext);

                return _CR_KrediHayatCarpanRepository;
            }
        }

        private CR_KaskoDMRepository _CR_KaskoDMRepository;
        public CR_KaskoDMRepository CR_KaskoDMRepository
        {
            get
            {
                if (_CR_KaskoDMRepository == null)
                    _CR_KaskoDMRepository = new CR_KaskoDMRepository(_dbContext);

                return _CR_KaskoDMRepository;
            }
        }
        
        private TrafikIMMRepository _TrafikIMMRepository;
        public TrafikIMMRepository TrafikIMMRepository
        {
            get
            {
                if (_TrafikIMMRepository == null)
                    _TrafikIMMRepository = new TrafikIMMRepository(_dbContext);

                return _TrafikIMMRepository;
            }
        }

        private TrafikFKRepository _TrafikFKRepository;
        public TrafikFKRepository TrafikFKRepository
        {
            get
            {
                if (_TrafikFKRepository == null)
                    _TrafikFKRepository = new TrafikFKRepository(_dbContext);

                return _TrafikFKRepository;
            }
        }

        private KaskoIMMRepository _KaskoIMMRepository;
        public KaskoIMMRepository KaskoIMMRepository
        {
            get
            {
                if (_KaskoIMMRepository == null)
                    _KaskoIMMRepository = new KaskoIMMRepository(_dbContext);

                return _KaskoIMMRepository;
            }
        }

        private KaskoFKRepository _KaskoFKRepository;
        public KaskoFKRepository KaskoFKRepository
        {
            get
            {
                if (_KaskoFKRepository == null)
                    _KaskoFKRepository = new KaskoFKRepository(_dbContext);

                return _KaskoFKRepository;
            }
        }

        private CR_MeslekIndirimiKaskoRepository _CR_MeslekIndirimiKaskoRepository;
        public CR_MeslekIndirimiKaskoRepository CR_MeslekIndirimiKaskoRepository
        {
            get
            {
                if (_CR_MeslekIndirimiKaskoRepository == null)
                    _CR_MeslekIndirimiKaskoRepository = new CR_MeslekIndirimiKaskoRepository(_dbContext);

                return _CR_MeslekIndirimiKaskoRepository;
            }
        }
        private MeslekIndirimiKaskoRepository _MeslekIndirimiKaskoRepository;
        public MeslekIndirimiKaskoRepository MeslekIndirimiKaskoRepository
        {
            get
            {
                if (_MeslekIndirimiKaskoRepository == null)
                    _MeslekIndirimiKaskoRepository = new MeslekIndirimiKaskoRepository(_dbContext);

                return _MeslekIndirimiKaskoRepository;
            }
        }
        private ErgoIlIlceRepository _ErgoIlIlceRepository;
        public ErgoIlIlceRepository ErgoIlIlceRepository
        {
            get
            {
                if (_ErgoIlIlceRepository == null)
                    _ErgoIlIlceRepository = new ErgoIlIlceRepository(_dbContext);

                return _ErgoIlIlceRepository;
            }
        }
        private UnicoIlIlceRepository _UnicoIlIlceRepository;
        public UnicoIlIlceRepository UnicoIlIlceRepository
        {
            get
            {
                if (_UnicoIlIlceRepository == null)
                    _UnicoIlIlceRepository = new UnicoIlIlceRepository(_dbContext);

                return _UnicoIlIlceRepository;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Database
{
    public interface IParametreContext : IUnitOfWork
    {
        KonfigurasyonRepository KonfigurasyonRepository { get; }
        EPostaFormatlariRepository EPostaFormatlariRepository { get; }
        SigortaSirketleriRepository SigortaSirketleriRepository { get; }

        UlkeRepository UlkeRepository { get; }
        IlRepository IlRepository { get; }
        IlceRepository IlceRepository { get; }

        AltMenuRepository AltMenuRepository { get; }
        AltMenuSekmeRepository AltMenuSekmeRepository { get; }
        AnaMenuRepository AnaMenuRepository { get; }
        BranRepository BranRepository { get; }
        BransUrunRepository BransUrunRepository { get; }
        MenuIslemRepository MenuIslemRepository { get; }
        GenelTanimlarRepository GenelTanimlarRepository { get; }

        DilAciklamaRepository DilAciklamaRepository { get; }

        //Müşteri COntext içerisinealınabilir...!!!!
        MeslekRepository MeslekRepository { get; }

        SoruRepository SoruRepository { get; }
        TeminatRepository TeminatRepository { get; }

        UrunRepository UrunRepository { get; }
        UrunSoruRepository UrunSoruRepository { get; }
        UrunTeminatRepository UrunTeminatRepository { get; }
        UrunVergiRepository UrunVergiRepository { get; }
        UrunParametreleriRepository UrunParametreleriRepository { get; }
        UserProfileRepository UserProfileRepository { get; }
        VergiRepository VergiRepository { get; }

        // ==== Dask ==== //
        DaskKurumlarRepository DaskKurumlarRepository { get; }
        DaskSubelerRepository DaskSubelerRepository { get; }
        DaskIlRepository DaskIlRepository { get; }
        DaskIlceRepository DaskIlceRepository { get; }
        DaskBeldeRepository DaskBeldeRepository { get; }

        // ==== KONUT ==== //
        BelediyeRepository BelediyeRepository { get; }
        BelediyeIlRepository BelediyeIlRepository { get; }
        DepremMuafiyetRepository DepremMuafiyetRepository { get; }


        // ====Seyehat Sağlık ==== //
        UlkeKodlariRepository UlkeKodlariRepository { get; }
        SchengenUlkeOranlariRepository SchengenUlkeOranlariRepository { get; }
        DigerUlkeOranlariRepository DigerUlkeOranlariRepository { get; }


        // ==== Offline Police Rapor ==== //
        OfflinePolouseRepository OfflinePolouseRepository { get; }

        // İkinci El Garanti Urunu
        El2Garanti_HesapCetveliRepository El2Garanti_HesapCetveliRepository { get; }

        //iş yeri
        IstigalRepository IstigalRepository { get; }

        //Banka Şubeleri
        BankaSubeleriRepository BankaSubeleriRepository { get; }

        KaskoYurticiTasiyiciKademeleriRepository KaskoYurticiTasiyiciKademeleriRepository { get; }
        KaskoTasinanYukKademeleriRepository KaskoTasinanYukKademeleriRepository { get; }
    }

    public class ParametreContext : IParametreContext
    {
        private readonly DbContext _dbContext;
        private bool _disposed;

        private KonfigurasyonRepository _konfigurasyonRepository;
        private EPostaFormatlariRepository _epostaFormatlariRepository;
        private SigortaSirketleriRepository _sigortaSirketleriRepository;

        private UlkeRepository _ulkeRepository;
        private IlRepository _ilRepository;
        private IlceRepository _ilceRepository;

        private AltMenuRepository _altMenuRepository;
        private AltMenuSekmeRepository _altMenuSekmeRepository;
        private AnaMenuRepository _anaMenuRepository;
        private BranRepository _branRepository;
        private BransUrunRepository _bransUrunRepository;
        private MenuIslemRepository _menuIslemRepository;

        private GenelTanimlarRepository _genelTanimlarRepository;
        private MeslekRepository _meslekRepository;


        private SoruRepository _soruRepository;
        private TeminatRepository _teminatRepository;

        private UrunRepository _urunRepository;
        private UrunSoruRepository _urunSoruRepository;
        private UrunTeminatRepository _urunTeminatRepository;
        private UrunVergiRepository _urunVergiRepository;
        private UrunParametreleriRepository _urunParametreleriRepository;

        private UserProfileRepository _userProfileRepository;
        private VergiRepository _vergiRepository;

        private DaskKurumlarRepository _daskKurumlarRepository;
        private DaskSubelerRepository _daskSubelerRepository;
        private DaskIlRepository _daskIlRepository;
        private DaskIlceRepository _daskIlceRepository;
        private DaskBeldeRepository _daskBeldeRepository;

        private BelediyeRepository _belediyeRepository;
        private BelediyeIlRepository _belediyeIlRepository;
        private DepremMuafiyetRepository _depremMuafiyetRepository;

        private UlkeKodlariRepository _ulkeKodlariRepository;
        private SchengenUlkeOranlariRepository _schengenUlkeOranlariRepository;
        private DigerUlkeOranlariRepository _digerUlkeOranlariRepository;
        private OfflinePolouseRepository _offlinePolouseRepository;
        private El2Garanti_HesapCetveliRepository _el2Garanti_HesapCetveliRepository;
        private IstigalRepository _istigalRepository;
        private BankaSubeleriRepository _bankaSubeleriRepository;

        private KaskoYurticiTasiyiciKademeleriRepository _kaskoYurticiTasiyiciKademeleriRepository;
        private KaskoTasinanYukKademeleriRepository _kaskoTasinanYukKademeleriRepository;

        public ParametreContext(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext();
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public EPostaFormatlariRepository EPostaFormatlariRepository
        {
            get
            {
                if (_epostaFormatlariRepository == null)
                {
                    _epostaFormatlariRepository = new EPostaFormatlariRepository(_dbContext);
                }
                return _epostaFormatlariRepository;
            }
        }

        public KonfigurasyonRepository KonfigurasyonRepository
        {
            get
            {
                if (_konfigurasyonRepository == null)
                {
                    _konfigurasyonRepository = new KonfigurasyonRepository(_dbContext);
                }
                return _konfigurasyonRepository;
            }
        }

        public SigortaSirketleriRepository SigortaSirketleriRepository
        {
            get
            {
                if (_sigortaSirketleriRepository == null)
                {
                    _sigortaSirketleriRepository = new SigortaSirketleriRepository(_dbContext);
                }
                return _sigortaSirketleriRepository;
            }
        }

        public UlkeRepository UlkeRepository
        {
            get
            {
                if (_ulkeRepository == null)
                {
                    _ulkeRepository = new UlkeRepository(_dbContext);
                }

                return _ulkeRepository;
            }
        }

        public IlRepository IlRepository
        {
            get
            {
                if (_ilRepository == null)
                {
                    _ilRepository = new IlRepository(_dbContext);
                }

                return _ilRepository;
            }
        }

        public IlceRepository IlceRepository
        {
            get
            {
                if (_ilceRepository == null)
                {
                    _ilceRepository = new IlceRepository(_dbContext);
                }

                return _ilceRepository;
            }
        }

        public UrunRepository UrunRepository
        {
            get
            {
                if (_urunRepository == null)
                {
                    _urunRepository = new UrunRepository(_dbContext);
                }

                return _urunRepository;
            }
        }

        public BranRepository BranRepository
        {
            get
            {
                if (_branRepository == null)
                {
                    _branRepository = new BranRepository(_dbContext);
                }

                return _branRepository;
            }
        }

        public BransUrunRepository BransUrunRepository
        {
            get
            {
                if (_bransUrunRepository == null)
                {
                    _bransUrunRepository = new BransUrunRepository(_dbContext);
                }

                return _bransUrunRepository;
            }
        }

        public SoruRepository SoruRepository
        {
            get
            {
                if (_soruRepository == null)
                {
                    _soruRepository = new SoruRepository(_dbContext);
                }
                return _soruRepository;
            }
        }

        public VergiRepository VergiRepository
        {
            get
            {
                if (_vergiRepository == null)
                {
                    _vergiRepository = new VergiRepository(_dbContext);
                }
                return _vergiRepository;
            }
        }

        public TeminatRepository TeminatRepository
        {
            get
            {
                if (_teminatRepository == null)
                {
                    _teminatRepository = new TeminatRepository(_dbContext);
                }
                return _teminatRepository;
            }
        }

        public UrunSoruRepository UrunSoruRepository
        {
            get
            {
                if (_urunSoruRepository == null)
                {
                    _urunSoruRepository = new UrunSoruRepository(_dbContext);
                }
                return _urunSoruRepository;
            }
        }

        public UrunTeminatRepository UrunTeminatRepository
        {
            get
            {
                if (_urunTeminatRepository == null)
                {
                    _urunTeminatRepository = new UrunTeminatRepository(_dbContext);
                }
                return _urunTeminatRepository;
            }
        }

        public UrunVergiRepository UrunVergiRepository
        {
            get
            {
                if (_urunVergiRepository == null)
                {
                    _urunVergiRepository = new UrunVergiRepository(_dbContext);
                }
                return _urunVergiRepository;
            }
        }

        public UrunParametreleriRepository UrunParametreleriRepository
        {
            get
            {
                if (_urunParametreleriRepository == null)
                {
                    _urunParametreleriRepository = new UrunParametreleriRepository(_dbContext);
                }
                return _urunParametreleriRepository;
            }
        }

        public AltMenuRepository AltMenuRepository
        {
            get
            {
                if (_altMenuRepository == null)
                {
                    _altMenuRepository = new AltMenuRepository(_dbContext);
                }
                return _altMenuRepository;
            }
        }

        public AltMenuSekmeRepository AltMenuSekmeRepository
        {
            get
            {
                if (_altMenuSekmeRepository == null)
                {
                    _altMenuSekmeRepository = new AltMenuSekmeRepository(_dbContext);
                }
                return _altMenuSekmeRepository;
            }
        }

        public AnaMenuRepository AnaMenuRepository
        {
            get
            {
                if (_anaMenuRepository == null)
                {
                    _anaMenuRepository = new AnaMenuRepository(_dbContext);
                }
                return _anaMenuRepository;
            }
        }

        public MenuIslemRepository MenuIslemRepository
        {
            get
            {
                if (_menuIslemRepository == null)
                {
                    _menuIslemRepository = new MenuIslemRepository(_dbContext);
                }
                return _menuIslemRepository;
            }
        }

        public GenelTanimlarRepository GenelTanimlarRepository
        {
            get
            {
                if (_genelTanimlarRepository == null)
                {
                    _genelTanimlarRepository = new GenelTanimlarRepository(_dbContext);
                }
                return _genelTanimlarRepository;
            }
        }
        
        private DilAciklamaRepository _dilAciklamaRepository;
        public DilAciklamaRepository DilAciklamaRepository
        {
            get
            {
                if (_dilAciklamaRepository == null)
                    _dilAciklamaRepository = new DilAciklamaRepository(_dbContext);

                return _dilAciklamaRepository;
            }
        }

        //Müşteri context içerisine taşınabilir....
        public MeslekRepository MeslekRepository
        {
            get
            {
                if (_meslekRepository == null)
                {
                    _meslekRepository = new MeslekRepository(_dbContext);
                }
                return _meslekRepository;
            }
        }
        
        public UserProfileRepository UserProfileRepository
        {
            get
            {
                if (_userProfileRepository == null)
                {
                    _userProfileRepository = new UserProfileRepository(_dbContext);
                }
                return _userProfileRepository;
            }
        }
        
        // ==== Dask ====//
        public DaskKurumlarRepository DaskKurumlarRepository
        {
            get
            {
                if (_daskKurumlarRepository == null)
                {
                    _daskKurumlarRepository = new DaskKurumlarRepository(_dbContext);
                }
                return _daskKurumlarRepository;
            }
        }

        public DaskSubelerRepository DaskSubelerRepository
        {
            get
            {
                if (_daskSubelerRepository == null)
                {
                    _daskSubelerRepository = new DaskSubelerRepository(_dbContext);
                }
                return _daskSubelerRepository;
            }
        }

        public DaskIlRepository DaskIlRepository
        {
            get
            {
                if (_daskIlRepository == null)
                {
                    _daskIlRepository = new DaskIlRepository(_dbContext);
                }
                return _daskIlRepository;
            }
        }

        public DaskIlceRepository DaskIlceRepository
        {
            get
            {
                if (_daskIlceRepository == null)
                {
                    _daskIlceRepository = new DaskIlceRepository(_dbContext);
                }
                return _daskIlceRepository;
            }
        }

        public DaskBeldeRepository DaskBeldeRepository
        {
            get
            {
                if (_daskBeldeRepository == null)
                {
                    _daskBeldeRepository = new DaskBeldeRepository(_dbContext);
                }
                return _daskBeldeRepository;
            }
        }

        // ==== KONUT ==== //
        public BelediyeRepository BelediyeRepository
        {
            get
            {
                if (_belediyeRepository == null)
                {
                    _belediyeRepository = new BelediyeRepository(_dbContext);
                }
                return _belediyeRepository;
            }
        }

        public BelediyeIlRepository BelediyeIlRepository
        {
            get
            {
                if (_belediyeIlRepository == null)
                {
                    _belediyeIlRepository = new BelediyeIlRepository(_dbContext);
                }
                return _belediyeIlRepository;
            }
        }

        public DepremMuafiyetRepository DepremMuafiyetRepository
        {
            get
            {
                if (_depremMuafiyetRepository == null)
                {
                    _depremMuafiyetRepository = new DepremMuafiyetRepository(_dbContext);
                }
                return _depremMuafiyetRepository;
            }
        }

        // ==== Seyehat Sağlık ==== //
        public UlkeKodlariRepository UlkeKodlariRepository
        {
            get
            {
                if (_ulkeKodlariRepository == null)
                {
                    _ulkeKodlariRepository = new UlkeKodlariRepository(_dbContext);
                }
                return _ulkeKodlariRepository;
            }
        }

        public SchengenUlkeOranlariRepository SchengenUlkeOranlariRepository
        {
            get
            {
                if (_schengenUlkeOranlariRepository == null)
                {
                    _schengenUlkeOranlariRepository = new SchengenUlkeOranlariRepository(_dbContext);
                }
                return _schengenUlkeOranlariRepository;
            }
        }

        public DigerUlkeOranlariRepository DigerUlkeOranlariRepository
        {
            get
            {
                if (_digerUlkeOranlariRepository == null)
                {
                    _digerUlkeOranlariRepository = new DigerUlkeOranlariRepository(_dbContext);
                }
                return _digerUlkeOranlariRepository;
            }
        }


        // OFFline police raporu
        public OfflinePolouseRepository OfflinePolouseRepository
        {
            get
            {
                if (_offlinePolouseRepository == null)
                {
                    _offlinePolouseRepository = new OfflinePolouseRepository(_dbContext);
                }
                return _offlinePolouseRepository;
            }
        }


        // İkinci El Garantili urunu
        public El2Garanti_HesapCetveliRepository El2Garanti_HesapCetveliRepository
        {
            get
            {
                if (_el2Garanti_HesapCetveliRepository == null)
                {
                    _el2Garanti_HesapCetveliRepository = new El2Garanti_HesapCetveliRepository(_dbContext);
                }
                return _el2Garanti_HesapCetveliRepository;
            }
        }

        //İş yeri
        public IstigalRepository IstigalRepository
        {
            get
            {
                if (_istigalRepository == null)
                {
                    _istigalRepository = new IstigalRepository(_dbContext);
                }
                return _istigalRepository;
            }
        }

        //Banka Şubeleri
        public BankaSubeleriRepository BankaSubeleriRepository
        {
            get
            {
                if (_bankaSubeleriRepository == null)
                    _bankaSubeleriRepository = new BankaSubeleriRepository(_dbContext);

                return _bankaSubeleriRepository;
            }
        }

        public KaskoYurticiTasiyiciKademeleriRepository KaskoYurticiTasiyiciKademeleriRepository
        {
            get
            {
                if (_kaskoYurticiTasiyiciKademeleriRepository == null)
                {
                    _kaskoYurticiTasiyiciKademeleriRepository = new KaskoYurticiTasiyiciKademeleriRepository(_dbContext);
                }
                return _kaskoYurticiTasiyiciKademeleriRepository;
            }
        }

        public KaskoTasinanYukKademeleriRepository KaskoTasinanYukKademeleriRepository
        {
            get
            {
                if (_kaskoTasinanYukKademeleriRepository == null)
                {
                    _kaskoTasinanYukKademeleriRepository = new KaskoTasinanYukKademeleriRepository(_dbContext);
                }
                return _kaskoTasinanYukKademeleriRepository;
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

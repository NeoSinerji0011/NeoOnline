using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AktifKullaniciService : IAktifKullaniciService
    {
        private const string USERINFO_SESSION_ID = "USER_INFO";
        private HttpSessionState mSession;
        ITVMContext _TVMContext;
        IKullaniciService _KullaniciService;

        public AktifKullaniciService(ITVMContext tvmContext,
                                     IKullaniciService kullanici,
                                     IYetkiService yetki,
                                     IMenuService menu)
        {
            mSession = HttpContext.Current.Session;
            _TVMContext = tvmContext;
            _KullaniciService = kullanici;
        }

        public void SetAnonymous()
        {
            this.Data.TVMKodu = 0;
            this.Data.BagliOlduguTvmKodu = -9999;
            this.Data.TVMUnvani = String.Empty;
            this.Data.KullaniciKodu = 0;
            this.Data.TCKN = String.Empty;
            this.Data.Adi = String.Empty;
            this.Data.Soyadi = String.Empty;
            this.Data.Email = String.Empty;
            this.Data.MuhasebeEntg = false;
            this.Data.YetkiGrubu = 0;
            this.Data.Yetkiler = new List<KullaniciYetkiModel>();
            this.Data.SonGirisTarihi = null;
            this.Data.FotografURL = String.Empty;
            this.Data.MTKodu = String.Empty;
            this.Data.TvmTipi = 0;
            
            if (mSession == null)
                mSession = HttpContext.Current.Session;

            mSession[USERINFO_SESSION_ID] = mData;
        }

        public void SetUser(string email)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.Email == email).FirstOrDefault();
            if (kullanici != null)
            {
                this.SetUser(kullanici);

                #region Mapfre Dis Acente

                TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);
                if (tvm != null && tvm.ProjeKodu == TVMProjeKodlari.Mapfre_DisAcente)
                {
                    TVMWebServisKullanicilari web = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                                                                                                                s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();
                    if (web != null)
                    {
                        this.TeknikPersonelKodu = web.KullaniciAdi;
                        this.MapfreBilgi = web.Sifre;
                    }

                    int countSube = _TVMContext.TVMDetayRepository.All().Count(c => c.GrupKodu == kullanici.TVMKodu);
                    this.MerkezAcente = countSube > 0;
                    //if (tvm.BagliOlduguTVMKodu==-9999)
                    //{
                    //    this.MerkezAcente = true;
                    //}
                    //else
                    //{
                    //    this.MerkezAcente = false;
                    //}
                }

                #endregion

                return;
            }

            throw new UserNotFoundException();
        }
        public void SetUser(int? tvmKodu)
        {
            //Tvmdetay
            if (tvmKodu != null)
            {
                int tvmKod = Convert.ToInt32(tvmKodu);
                TVMDetay tvmdetay = _TVMContext.TVMDetayRepository.Filter(f => f.Kodu == tvmKod).FirstOrDefault();
                if (tvmdetay != null )
                {
                    /// kontroller konulacak. tvm var mı diye birde tvm internet kullanıcısı mı internet tvmsi mi kontrol koy 
                    TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TVMKodu == tvmKodu).FirstOrDefault();
                    // kullanisiyi internet kullanıcısı mı diye yetki tanımlamada internet yetkisi mi 
                    if (kullanici != null && kullanici.KullaniciKodu== 11931)
                    {
                        this.SetUser(kullanici);
                        return;
                    }
                }
            }
            throw new UserNotFoundException();
        }
        public void SetUserAEGON(string acenteKodu)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == acenteKodu).FirstOrDefault();
            if (kullanici != null)
            {
                this.SetUser(kullanici);
                return;
            }

            throw new UserNotFoundException();
        }

        public void SetUserAEGON_SSO(string acenteKodu, string session)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == acenteKodu).FirstOrDefault();
            if (kullanici != null)
            {
                this.SetUser(kullanici);
                this.Data.AegonSession = session;

                return;
            }

            throw new UserNotFoundException();
        }

        public void SetUserMAPFRE(string teknikPersonelKodu, string password, string cod_agt)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TeknikPersonelKodu == teknikPersonelKodu).FirstOrDefault();
            if (kullanici != null)
            {
                this.SetUser(kullanici);
                this.TeknikPersonelKodu = teknikPersonelKodu;
                this.MapfreBilgi = password;
                this.MTKodu = cod_agt;
                this.MapfreMerkez = false;
                this.MapfreBolge = false;
                this.MapfreMerkezAcente = false;

                TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);
                if (kullanici.TVMKodu == 107)
                {
                    this.MapfreMerkez = true;
                }
                else if (tvm.Profili == 2)
                {
                    this.MapfreBolge = true;
                }
                else
                {
                    int countSube = _TVMContext.TVMDetayRepository.All().Count(c => c.GrupKodu == kullanici.TVMKodu);
                    this.MapfreMerkezAcente = countSube > 0;
                }

                return;
            }

            throw new UserNotFoundException();
        }

        public void ChangeLang(string email)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(f => f.Email == email).FirstOrDefault();
            if (kullanici != null)
            {
                this.Data.Yetkiler = _TVMContext.KullaniciYetkileri_Getir(kullanici.YetkiGrubu);
                this.Data.UrunYetkileri = _TVMContext.GetTVMUrunYetkileri(kullanici.TVMKodu);
                return;
            }
            throw new UserNotFoundException();
        }

        public void SetUser(TVMKullanicilar kullanici)
        {
            TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kullanici.TVMKodu);

            this.Data.TVMKodu = kullanici.TVMKodu;
            if(tvm.BagliOlduguTVMKodu!= -9999 && tvm.BagliOlduguTVMKodu!=0)
            {
                var anatvm = _TVMContext.TVMDetayRepository.FindById(tvm.BagliOlduguTVMKodu);
                this.Data.TvmTipi = anatvm.Tipi;
            }
            else
            {
                this.Data.TvmTipi = tvm.Tipi;
            }
            this.Data.BagliOlduguTvmKodu = tvm.BagliOlduguTVMKodu;
            this.Data.TVMUnvani = tvm.Unvani;
            this.Data.KullaniciKodu = kullanici.KullaniciKodu;
            this.Data.Gorevi = kullanici.Gorevi;
            this.Data.TCKN = kullanici.TCKN;
            this.Data.Adi = kullanici.Adi;
            this.Data.Soyadi = kullanici.Soyadi;
            this.Data.Email = kullanici.Email;
            this.Data.SifreTarihi = kullanici.SifreTarihi;
            this.Data.YetkiGrubu = kullanici.YetkiGrubu;
            this.Data.Yetkiler = _TVMContext.KullaniciYetkileri_Getir(kullanici.YetkiGrubu);
            this.Data.UrunYetkileri = _TVMContext.GetTVMUrunYetkileri(kullanici.TVMKodu);
            this.Data.FotografURL = kullanici.FotografURL;
            this.Data.SonGirisTarihi = kullanici.SonGirisTarihi;
            this.Data.ProjeKodu = tvm.ProjeKodu;
            this.Data.MTKodu = kullanici.MTKodu;

            if (String.IsNullOrEmpty(this.Data.ProjeKodu))
            {
                this.Data.ProjeKodu = "Default";
            }

            if (tvm.MuhasebeEntegrasyon.HasValue && tvm.MuhasebeEntegrasyon.Value)
                this.Data.MuhasebeEntg = true;
            else
                this.Data.MuhasebeEntg = false;

            kullanici.SonGirisTarihi = TurkeyDateTime.Now;
            _KullaniciService.UpdateKullanici(kullanici);

            mSession[USERINFO_SESSION_ID] = mData;
        }

        public void Logout()
        {
            this.SetAnonymous();
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.KullaniciKodu > 0;
            }
        }

        public int TVMKodu
        {
            get
            {
                return this.Data.TVMKodu;
            }
        }
        public int BagliOlduguTvmKodu
        {
            get
            {
                return this.Data.BagliOlduguTvmKodu;
            }
        }
        public string TVMUnvani
        {
            get
            {
                return this.Data.TVMUnvani;
            }
        }

        public int KullaniciKodu
        {
            get
            {
                return this.Data.KullaniciKodu;
            }
        }
        public byte Gorevi
        {
            get
            {
                return this.Data.Gorevi;
            }
        }

        public string TCKN
        {
            get
            {
                return this.Data.TCKN;
            }
        }
        public int TvmTipi
        {
            get
            {
                return this.Data.TvmTipi;
            }
        }

        public string Adi
        {
            get
            {
                return this.Data.Adi;
            }
        }

        public string Soyadi
        {
            get
            {
                return this.Data.Soyadi;
            }
        }

        public string AdiSoyadi
        {
            get
            {
                return String.Format("{0} {1}", this.Adi, this.Soyadi);
            }
        }

        public string Email
        {
            get
            {
                return this.Data.Email;
            }
        }

        public DateTime? SifreTarihi
        {
            get
            {
                return this.Data.SifreTarihi;
            }
        }

        public int YetkiGrubu
        {
            get
            {
                return this.Data.YetkiGrubu;
            }
        }

        public bool MuhasebeEntg
        {
            get
            {
                return this.Data.MuhasebeEntg;
            }
        }

        public string TeknikPersonelKodu
        {
            get
            {
                return this.Data.TeknikPersonelKodu;
            }
            set
            {
                this.Data.TeknikPersonelKodu = value;
            }
        }

        public string MapfreBilgi
        {
            get
            {
                return this.Data.MapfreBilgi;
            }
            set
            {
                this.Data.MapfreBilgi = value;
            }
        }

        public bool MapfreBolge
        {
            get
            {
                return this.Data.MapfreBolge;
            }
            set
            {
                this.Data.MapfreBolge = value;
            }
        }

        public bool MapfreMerkez
        {
            get
            {
                return this.Data.MapfreMerkez;
            }
            set
            {
                this.Data.MapfreMerkez = value;
            }
        }

        public bool MapfreMerkezAcente
        {
            get
            {
                return this.Data.MapfreMerkezAcente;
            }
            set
            {
                this.Data.MapfreMerkezAcente = value;
            }
        }

        public bool MerkezAcente
        {
            get
            {
                return this.Data.MerkezAcente;
            }
            set
            {
                this.Data.MerkezAcente = value;
            }
        }

        public string MTKodu
        {
            get
            {
                return this.Data.MTKodu;
            }
            set
            {
                this.Data.MTKodu = value;
            }
        }

        public string AegonSession
        {
            get
            {
                return this.Data.AegonSession;
            }
            set
            {
                this.Data.AegonSession = value;
            }
        }

        public List<KullaniciYetkiModel> Yetkiler
        {
            get
            {
                return this.Data.Yetkiler;
            }
        }

        public List<TVMUrunYetkileriProcedureModel> UrunYetkileri
        {
            get
            {
                return this.Data.UrunYetkileri;
            }
        }

        public DateTime? SonGirisTarihi
        {
            get
            {
                return this.Data.SonGirisTarihi;
            }
        }

        public string FotografURL
        {
            get { return this.Data.FotografURL; }
        }

        public string ProjeKodu
        {
            get { return this.Data.ProjeKodu; }
        }

        private Kullanici mData;
        private Kullanici Data
        {
            get
            {
                if (mData == null)
                {
                    object data = null;
                    if (mSession != null)
                        data = mSession[USERINFO_SESSION_ID];

                    if (data == null)
                    {
                        mData = new Kullanici();
                        this.SetAnonymous();
                    }
                    else
                    {
                        mData = data as Kullanici;
                    }
                }

                return mData;
            }
        }

        public bool Policelestirme { get { return this.Data.Policelestirme; } }
    }
}

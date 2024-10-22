using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UserNotFoundException : ApplicationException
    {

    }

    public interface IAktifKullaniciService
    {
        void SetAnonymous();
        void SetUser(string tckn);
        void SetUser(int? tvmKodu);
        void SetUserAEGON(string acenteKodu);
        void SetUserAEGON_SSO(string acenteKodu, string session);
        void SetUserMAPFRE(string teknikPersonelKodu, string password, string cod_agt);
        void ChangeLang(string email);

        void SetUser(TVMKullanicilar kullanici);

        void Logout();

        bool IsAuthenticated { get; }

        int TVMKodu { get; }
        int BagliOlduguTvmKodu { get; }
        string TVMUnvani { get; }
        int KullaniciKodu { get; }
        byte Gorevi { get; }
        string TCKN { get; }
        string Adi { get; }
        string Soyadi { get; }
        string AdiSoyadi { get; }
        string Email { get; }
        bool MuhasebeEntg { get; }
        string ProjeKodu { get; }
        DateTime? SifreTarihi { get; }
        int TvmTipi { get; }

        int YetkiGrubu { get; }
        List<KullaniciYetkiModel> Yetkiler { get; }
        List<TVMUrunYetkileriProcedureModel> UrunYetkileri { get; }
        DateTime? SonGirisTarihi { get; }

        string TeknikPersonelKodu { get; }
        /// <summary>
        /// Mapfre sigorta kullanıcıları login olduğunda şifreleri mapfre web servislerinde kullanılıyor.
        /// Web servise gönderilecek şifre bu alanda tutuluyor.
        /// </summary>
        string MapfreBilgi { get; }

        /// <summary>
        /// Mapfre bölge kullanıcısı, TVMDetay tablosu Profili = 2 olan kullanıcılar
        /// </summary>
        bool MapfreBolge { get; set; }

        /// <summary>
        /// Mapfre merkez tvm kullanıcısı, TVMDetay tablosu Kodu = 107 olan kullanıcılar
        /// </summary>
        bool MapfreMerkez { get; set; }

        /// <summary>
        /// Merkez acente kullanıcısı, alt şubeleri varsa true olur. TVMDetay.GrupKodu = login 
        /// </summary>
        bool MapfreMerkezAcente { get; set; }

        /// <summary>
        /// Merkez acente kullanıcısı, alt şubeleri (tali acentleri)  varsa true olur. TVMDetay.GrupKodu = login 
        /// </summary>
        bool MerkezAcente { get; set; }

        /// <summary>
        /// Mapfre sigorta partaj numarası bu alanda tutuluyor.
        /// </summary>
        string MTKodu { get; }

        /// <summary>
        /// Aegon tek şifre uygulamasından gelen AegonSession
        /// </summary>
        string AegonSession { get; }

        bool Policelestirme { get; }
    }
}

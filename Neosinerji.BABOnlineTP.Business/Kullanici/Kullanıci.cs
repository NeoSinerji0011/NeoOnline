using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    [Serializable]
    public class Kullanici
    {
        public Kullanici()
        {

        }

        public int TVMKodu { get; set; }
        public int BagliOlduguTvmKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int KullaniciKodu { get; set; }
        public byte Gorevi { get; set; }
        public string TCKN { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public DateTime? SifreTarihi { get; set; }
        public int YetkiGrubu { get; set; }
        public bool MuhasebeEntg { get; set; }
        public List<KullaniciYetkiModel> Yetkiler { get; set; }
        public List<TVMUrunYetkileriProcedureModel> UrunYetkileri { get; set; }
        public string FotografURL { get; set; }
        public string ProjeKodu { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public DashboardChartModel UstDortBilgiler { get; set; }
        public string TeknikPersonelKodu { get; set; }
        public string MapfreBilgi { get; set; }
        public bool MapfreBolge { get; set; }
        public bool MapfreMerkez { get; set; }
        public bool MapfreMerkezAcente { get; set; }
        public bool MerkezAcente {get; set; }

        public string MTKodu { get; set; }
        public int TvmTipi { get; set; }
        public string AegonSession { get; set; }

        public bool Policelestirme { get; set; }
    }

    public class KullaniciOzetModel
    {
        public int KullaniciKodu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
    }

    public class KullaniciArama : DataTableParameters<KullaniciListeModel>
    {
        public KullaniciArama(HttpRequestBase httpRequest, Expression<Func<KullaniciListeModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public KullaniciArama(HttpRequestBase httpRequest,
                                   Expression<Func<KullaniciListeModel, object>>[] selectColumns,
                                   Expression<Func<KullaniciListeModel, object>> rowIdColumn,
                                   Expression<Func<KullaniciListeModel, object>> linkColumn1,
                                   string linkColumn1Url,
                                   string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> TVMKodu { get; set; }
        public Nullable<short> TVMTipi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string TCKN { get; set; }
        public Nullable<byte> Durum { get; set; }
        public string TeknikPersonelKodu { get; set; }
    }

    public class KullaniciListeModel
    {
        public int KullaniciKodu { get; set; }
        public string TCKN { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TVMUnvani { get; set; }
        public short TVMTipi { get; set; }
        public string TVMTipiText { get; set; }
        public string DepartmanAdi { get; set; }
        public string Email { get; set; }
        public byte Durum { get; set; }
        public string DurumText { get; set; }
        public string YetkiGrupAdi { get; set; }
        public byte Gorevi { get; set; }
        public string GoreviText { get; set; }
        public string TeknikPersonelKodu { get; set; }
    }

    public class KullaniciOzelModel
    {
        public List<AnaMenuOzelModel> Menuler { get; set; }
    }

    public class KullaniciModelForList
    {
        public int KullaniciKodu { get; set; }
        public string AdiSoyadi { get; set; }
    }

    //Menulere ait islem url lerini tutan kullaniciya ozel menu modelleri
    public class MenuKokModel
    {
        public int AnaMenuKodu { get; set; }
        public string MenuAdi { get; set; }
        public string URL { get; set; }
        public string MVCAction { get; set; }
        public string MVCController { get; set; }
        public string MVCActionParameter { get; set; }

        public byte Durum { get; set; }
        public byte Gorme { get; set; }
        public byte YeniKayit { get; set; }
        public byte Degistirme { get; set; }
        public byte Silme { get; set; }
    }

    public class AnaMenuOzelModel : MenuKokModel
    {
        public AnaMenuOzelModel()
        {
            this.AltMenuler = new List<AltMenuOzelModel>();
        }
        public List<AltMenuOzelModel> AltMenuler { get; set; }
    }

    public class AltMenuOzelModel : MenuKokModel
    {
        public AltMenuOzelModel()
        {
            this.Sekmeler = new List<SekmeOzelModel>();
        }
        public int AltMenuKodu { get; set; }
        public List<SekmeOzelModel> Sekmeler { get; set; }
    }

    public class SekmeOzelModel : MenuKokModel
    {
        public int AltMenuKodu { get; set; }
        public int SekmeKodu { get; set; }
    }

    public class AcenteKullanicilariModel
    {
        public List<KullaniciSkypeModel> list { get; set; }
    }

    public class KullaniciSkypeModel
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string AdiSoyadi { get; set; }
        public string Yetki { get; set; }
        public int KullaniciKodu { get; set; }
        public string SkypeNumarasi { get; set; }
        public string FotoURL { get; set; }
        public string Telefon { get; set; }
        public string TVMUnvani { get; set; }
    }
}

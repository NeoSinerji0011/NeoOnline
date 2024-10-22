using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Web.Mvc;
using System.Data;
using Neosinerji.BABOnlineTP.Database;
using Microsoft.Practices.Unity;
using AutoMapper;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class SubeSatisListe : DataTableParameters<SubeSatisRaporProcedureModel>
    {
        public SubeSatisListe(HttpRequestBase httpRequest, Expression<Func<SubeSatisRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public SubeSatisListe(HttpRequestBase httpRequest,
                                      Expression<Func<SubeSatisRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<SubeSatisRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<SubeSatisRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public string BransList { get; set; }
        public string UrunList { get; set; }


        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<int> TahIpt { get; set; }


    }

    public class MtSatisListe : DataTableParameters<MTSatisRaporProcedureModel>
    {
        public MtSatisListe(HttpRequestBase httpRequest, Expression<Func<MTSatisRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MtSatisListe(HttpRequestBase httpRequest,
                                      Expression<Func<MTSatisRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<MTSatisRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<MTSatisRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public string BransList { get; set; }
        public string UrunList { get; set; }
        public string Subeler { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<int> TahIpt { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }

        // public string AdiSoyadi { get; set; }

    }

    public class PoliceRaporListe : DataTableParameters<PoliceRaporProcedureModel>
    {
        public PoliceRaporListe(HttpRequestBase httpRequest, Expression<Func<PoliceRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public PoliceRaporListe(HttpRequestBase httpRequest,
                                      Expression<Func<PoliceRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<PoliceRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<PoliceRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<byte> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public string BransList { get; set; }
        public string UrunList { get; set; }
        public string Subeler { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<byte> TahIpt { get; set; }
        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        //public Nullable<byte> Durumu { get; set; }
        //public List<SelectListItem> Durumlari { get; set; }


    }

    public class PoliceRaporOfflineListe : DataTableParameters<PoliceListesiOfflineRaporProcedureModel>
    {
        public PoliceRaporOfflineListe(HttpRequestBase httpRequest, Expression<Func<PoliceListesiOfflineRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public PoliceRaporOfflineListe(HttpRequestBase httpRequest,
                                      Expression<Func<PoliceListesiOfflineRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<PoliceListesiOfflineRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<PoliceListesiOfflineRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }
        public int TVMKodu { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public byte? PoliceTarihTipi { get; set; }
        // public Nullable<int> DovizTL { get; set; }
        public string Subeler { get; set; }
        public string SigortaSirketleri { get; set; }
        public string BransList { get; set; }         
        //public Nullable<byte> TahIpt { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

    }

    public class PoliceUretimRaporListe : DataTableParameters<PoliceUretimRaporProcedureModel>
    {
        public PoliceUretimRaporListe(HttpRequestBase httpRequest, Expression<Func<PoliceUretimRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public PoliceUretimRaporListe(HttpRequestBase httpRequest,
                                      Expression<Func<PoliceUretimRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<PoliceUretimRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<PoliceUretimRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public string BransList { get; set; }
        public string TVMList { get; set; }
        public string DisUretimTVMList { get; set; }
        public string SigortaSirketleri { get; set; }
        public DateTime Donem { get; set; }
        public DateTime BasTarihi { get; set; }
        public DateTime BitTarihi { get; set; }
        //public int Ay { get; set; }
        //public int Yil { get; set; }
        public byte Siralama { get; set; }


    }

    public class PoliceUretimRaporListeTali : DataTableParameters<PoliceUretimRaporProcedureModelTali>
    {
        public PoliceUretimRaporListeTali(HttpRequestBase httpRequest, Expression<Func<PoliceUretimRaporProcedureModelTali, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public PoliceUretimRaporListeTali(HttpRequestBase httpRequest,
                                      Expression<Func<PoliceUretimRaporProcedureModelTali, object>>[] selectColumns,
                                      Expression<Func<PoliceUretimRaporProcedureModelTali, object>> rowIdColumn,
                                      Expression<Func<PoliceUretimRaporProcedureModelTali, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public string BransList { get; set; }
        public string TVMList { get; set; }
        public string SigortaSirketleri { get; set; }
        public DateTime Donem { get; set; }
        public int Ay { get; set; }
        public int Yil { get; set; }
        public byte Siralama { get; set; }


    }

    public class VadeTakipRaporuListe : DataTableParameters<VadeTakipRaporuProcedureModel>
    {
        public VadeTakipRaporuListe(HttpRequestBase httpRequest, Expression<Func<VadeTakipRaporuProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public VadeTakipRaporuListe(HttpRequestBase httpRequest,
                                      Expression<Func<VadeTakipRaporuProcedureModel, object>>[] selectColumns,
                                      Expression<Func<VadeTakipRaporuProcedureModel, object>> rowIdColumn,
                                      Expression<Func<VadeTakipRaporuProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }


        public Nullable<int> DovizTL { get; set; }
        public string BransList { get; set; }
        public string UrunList { get; set; }
        public string Subeler { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<byte> TahIpt { get; set; }

        public Nullable<byte> TarihTipi { get; set; }
        public List<SelectListItem> TarihAraliklari { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }

        public Nullable<byte> Durumu { get; set; }
        public List<SelectListItem> Durumlari { get; set; }

    }

    public class KrediliHayatPoliceRaporListe : DataTableParameters<KrediliHayatPoliceRaporProcedureModel>
    {
        public KrediliHayatPoliceRaporListe(HttpRequestBase httpRequest, Expression<Func<KrediliHayatPoliceRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public KrediliHayatPoliceRaporListe(HttpRequestBase httpRequest,
                                      Expression<Func<KrediliHayatPoliceRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<KrediliHayatPoliceRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<KrediliHayatPoliceRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<byte> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public string Subeler { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<byte> TahIpt { get; set; }

        public string PoliceNo { get; set; }

        public Nullable<byte> OdemeSekli { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }

        public Nullable<byte> OdemeTipi { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class TeklifRaporuListe : DataTableParameters<TeklifRaporuProcedureModel>
    {
        public TeklifRaporuListe(HttpRequestBase httpRequest, Expression<Func<TeklifRaporuProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public TeklifRaporuListe(HttpRequestBase httpRequest,
                                      Expression<Func<TeklifRaporuProcedureModel, object>>[] selectColumns,
                                      Expression<Func<TeklifRaporuProcedureModel, object>> rowIdColumn,
                                      Expression<Func<TeklifRaporuProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<byte> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public string BransList { get; set; }
        public string UrunList { get; set; }
        public string Subeler { get; set; }


        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<byte> TahIpt { get; set; }

        public Nullable<int> TeklifNo { get; set; }


    }

    public class OzetRaporListe : DataTableParameters<OzetRaporProcedureModel>
    {
        public OzetRaporListe(HttpRequestBase httpRequest, Expression<Func<OzetRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public OzetRaporListe(HttpRequestBase httpRequest,
                                      Expression<Func<OzetRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<OzetRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<OzetRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public Nullable<int> OdemeTipi { get; set; }

        public string BransList { get; set; }
        public string UrunList { get; set; }
        public string Subeler { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<int> TahIpt { get; set; }
        public Nullable<int> aramakriteri { get; set; }
        public Nullable<int> asildeger { get; set; }
        public Nullable<int> data2 { get; set; }
        public string data3 { get; set; }
    }

    public class AracSigortalariIstatistikRaporuListe : DataTableParameters<AracSigortalariIstatistikRaporuProcedureModel>
    {
        public AracSigortalariIstatistikRaporuListe(HttpRequestBase httpRequest, Expression<Func<AracSigortalariIstatistikRaporuProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public AracSigortalariIstatistikRaporuListe(HttpRequestBase httpRequest,
                                      Expression<Func<AracSigortalariIstatistikRaporuProcedureModel, object>>[] selectColumns,
                                      Expression<Func<AracSigortalariIstatistikRaporuProcedureModel, object>> rowIdColumn,
                                      Expression<Func<AracSigortalariIstatistikRaporuProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public Nullable<int> TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public Nullable<int> PoliceTarihi { get; set; }
        public Nullable<int> DovizTL { get; set; }
        public Nullable<int> Urun { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Nullable<int> TahIpt { get; set; }
        public Nullable<int> customvalue { get; set; }
        public Nullable<int> kodu { get; set; }
        public Nullable<int> data2 { get; set; }
        public string sorguturu { get; set; }
    }

    public class PoliceHaritaModel
    {
        List<MusteriHaritaOzelDetay> Musteriler { get; set; }
        List<PoliceHaritaOzelDetay> Policeler { get; set; }
    }

    public class PoliceHaritaOzelDetay
    {
        public int TeklifId { get; set; }
        public int Urun { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    #region Police Aktarma Model


    public class OfflinePoliceADL
    {
        ITVMContext _TvmContext;
        IRaporService _RaporService;
        IAktifKullaniciService _AktifKullanici;
        IUrunService _UrunService;
        IKullaniciService _KullaniciService;
        IMusteriService _MusteriService;
        ITVMService _TVMService;

        public OfflinePoliceADL()
        {
            _TvmContext = DependencyResolver.Current.GetService<ITVMContext>();
            _RaporService = DependencyResolver.Current.GetService<IRaporService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _UrunService = DependencyResolver.Current.GetService<IUrunService>();
            _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
        }

        #region Conts

        //Müşteri Tipi
        private const string MusteriTipiGercek = "Gerçek";
        private const string MusteriTipiFirma = "Firma";
        private const string MusteriTipiTuzel = "Tüzel";
        private const string MusteriTipiYabanci = "Yabancı";

        //Ödeme Şekli
        private const string Peşin = "Peşin";
        private const string Vadeli = "Vadeli";

        //Ödeme Tipi
        private const string Nakit = "Nakit";
        private const string KrediKarti = "Kredi Kartı";
        private const string Havale = "Havale";

        //Satış Türü
        private const string YeniSatis = "Yeni Satış";
        private const string Yenileme = "Yenileme";

        #endregion

        public bool OfflinePoliceKaydet(string dosyaAdresi)
        {
            ExcelOfflinePoliceListModel model = ProcessFile(dosyaAdresi);

            foreach (var item in model.HatasizKayitlar)
            {
                OfflinePolouse police = new OfflinePolouse();

                decimal brutPrim;
                decimal komisyon;
                int tvmKodu;
                byte taksitAdedi;

                if (!String.IsNullOrEmpty(item.BrutPrim))
                    if (decimal.TryParse(item.BrutPrim, out brutPrim))
                        police.BrutPrim = brutPrim;

                if (!String.IsNullOrEmpty(item.Komisyon))
                    if (decimal.TryParse(item.Komisyon, out komisyon))
                        police.Komisyon = komisyon;

                if (!String.IsNullOrEmpty(item.TaksitAdedi))
                    if (byte.TryParse(item.TaksitAdedi, out taksitAdedi))
                        police.TaksitAdedi = taksitAdedi;

                if (!String.IsNullOrEmpty(item.TVMKodu))
                    if (int.TryParse(item.TVMKodu, out tvmKodu))
                        police.TVMKodu = tvmKodu;

                if (!String.IsNullOrEmpty(item.OdemeSekli))
                    police.OdemeSekli = item.OdemeSekli == Peşin ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli;

                if (!String.IsNullOrEmpty(item.OdemeTipi))
                    switch (item.OdemeTipi)
                    {
                        case Nakit: police.OdemeTipi = OdemeTipleri.Nakit; break;
                        case KrediKarti: police.OdemeTipi = OdemeTipleri.KrediKarti; break;
                        case Havale: police.OdemeTipi = OdemeTipleri.Havale; break;
                    }

                if (!String.IsNullOrEmpty(item.PoliceBaslangicTarihi))
                {
                    double basT = double.Parse(item.PoliceBitisTarihi);
                    police.PoliceBaslangicTarihi = DateTime.FromOADate(basT);
                }

                if (!String.IsNullOrEmpty(item.PoliceBitisTarihi))
                {
                    double basT = double.Parse(item.PoliceBitisTarihi);
                    police.PoliceBitisTarihi = DateTime.FromOADate(basT);
                }

                if (!String.IsNullOrEmpty(item.TanzimTarihi))
                {
                    double basT = double.Parse(item.TanzimTarihi);
                    police.TanzimTarihi = DateTime.FromOADate(basT);
                }

                if (!String.IsNullOrEmpty(item.SatisTuru))
                    police.SatisTuru = item.SatisTuru == YeniSatis ? SatisTurleri.YeniSatis : SatisTurleri.Yenileme;


                if (!String.IsNullOrEmpty(item.SEKimlikNo))
                    police.SEKimlikNo = item.SEKimlikNo;

                if (!String.IsNullOrEmpty(item.SKimlikNo))
                    police.SKimlikNo = item.SKimlikNo;
                else
                    police.SKimlikNo = item.SEKimlikNo;

                if (!String.IsNullOrEmpty(item.SatisTemsilcisiTCKN))
                    police.SatisTemsilcisiTCKN = item.SatisTemsilcisiTCKN;

                police.YenilemeNo = item.YenilemeNo;
                police.ZeyileNo = item.ZeyilNo;
                police.CreatedBy = _AktifKullanici.KullaniciKodu;
                police.CreatedDate = TurkeyDateTime.Now;
                police.KrediKartiBankaAdi = item.KrediKartiBankaAdi;
                police.PoliceNo = item.PoliceNo;


                if (!String.IsNullOrEmpty(item.UrunKodu))
                    police.UrunKodu = Convert.ToInt32(item.UrunKodu);

                _RaporService.CreateOfflinePolice(police);
            }
            return true;
        }

        private DataTable GetTable()
        {
            DataTable dt = new DataTable();

            DataColumn[] Coll = new DataColumn[22];

            Coll[0] = new DataColumn("UrunKodu", typeof(string));
            Coll[1] = new DataColumn("TVMKodu", typeof(string));
            Coll[2] = new DataColumn("SigortaEttirenKimlikNo", typeof(string));
            Coll[3] = new DataColumn("SigortaEttirenTVMMusteriKodu", typeof(string));
            Coll[4] = new DataColumn("SigortaliKimlikNo", typeof(string));
            Coll[5] = new DataColumn("SigortaliTVMMusteriKodu", typeof(string));
            Coll[6] = new DataColumn("PoliçeNo", typeof(string));
            Coll[7] = new DataColumn("YenilemeNo", typeof(string));
            Coll[8] = new DataColumn("ZeyilNo", typeof(string));
            Coll[9] = new DataColumn("BrutPrim", typeof(string));
            Coll[10] = new DataColumn("Komisyon", typeof(string));
            Coll[11] = new DataColumn("PoliceBaslangicTarihi", typeof(string));
            Coll[12] = new DataColumn("PoliceBitisTarihi", typeof(string));
            Coll[13] = new DataColumn("PoliceTanzimTarihi", typeof(string));
            Coll[14] = new DataColumn("TaksitAdedi", typeof(string));
            Coll[15] = new DataColumn("OdemeSekli", typeof(string));
            Coll[16] = new DataColumn("OdemeTipi", typeof(string));
            Coll[17] = new DataColumn("KrediKartiBankaAdi", typeof(string));
            Coll[18] = new DataColumn("SatisTuru", typeof(string));
            Coll[19] = new DataColumn("SatisTemsilcisiTCKN", typeof(string));

            dt.Columns.AddRange(Coll);

            return dt;
        }

        public ExcelOfflinePoliceListModel ProcessFile(string mFilePath)
        {
            XL2007 xl = null;
            xl = new XL2007(mFilePath, 1);
            ExcelOfflinePoliceListModel model = new ExcelOfflinePoliceListModel();

            if (xl.Open())
            {
                DataTable dt = GetTable();

                model.HataliKayitlar = new List<ExcelOfflinePoliceModel>();
                model.HatasizKayitlar = new List<ExcelOfflinePoliceModel>();
                xl.Fill(dt);
                xl.Dispose();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        ExcelOfflinePoliceModel mdl = new ExcelOfflinePoliceModel();

                        mdl.UrunKodu = item[0].ToString();
                        mdl.TVMKodu = item[1].ToString();
                        mdl.SEKimlikNo = item[2].ToString();
                        mdl.SETVMMusteriKodu = item[3].ToString();
                        mdl.SKimlikNo = item[4].ToString();
                        mdl.STVMMusteriKodu = item[5].ToString();
                        mdl.PoliceNo = item[6].ToString();
                        mdl.YenilemeNo = item[7].ToString();
                        mdl.ZeyilNo = item[8].ToString();
                        mdl.BrutPrim = item[9].ToString();
                        mdl.Komisyon = item[10].ToString();
                        mdl.PoliceBaslangicTarihi = item[11].ToString();
                        mdl.PoliceBitisTarihi = item[12].ToString();
                        mdl.TanzimTarihi = item[13].ToString();
                        mdl.TaksitAdedi = item[14].ToString();
                        mdl.OdemeSekli = item[15].ToString();
                        mdl.OdemeTipi = item[16].ToString();
                        mdl.KrediKartiBankaAdi = item[17].ToString();
                        mdl.SatisTuru = item[18].ToString();
                        mdl.SatisTemsilcisiTCKN = item[19].ToString();

                        mdl = ModelKontrol(mdl);

                        if (mdl.HataMesaj == "")
                        {
                            mdl.hatalist = mdl.HataMesaj.Split('.');
                            model.HatasizKayitlar.Add(mdl);
                        }
                        else
                        {
                            mdl.hatalist = mdl.HataMesaj.Split('.');
                            model.HataliKayitlar.Add(mdl);
                        }
                    }
                }
            }
            return model;
        }

        private ExcelOfflinePoliceModel ModelKontrol(ExcelOfflinePoliceModel model)
        {
            if (model != null)
            {
                model.HataMesaj += UrunKoduKontrol(model.UrunKodu);
                model.HataMesaj += TVMKoduKontrol(model.TVMKodu, model.SatisTemsilcisiTCKN, model.PoliceNo);
                model.HataMesaj += SigortaEttirenTipiKimlikNoKontrol(model.SEKimlikNo, model.TVMKodu);
                model.HataMesaj += SigortaliTipiKimlikNoKontrol(model.SKimlikNo, model.TVMKodu);
                model.HataMesaj += BrutPrimControl(model.BrutPrim);
                model.HataMesaj += KomisyonControl(model.Komisyon);
                model.HataMesaj += PoliceBaslangicTarihiControl(model.PoliceBaslangicTarihi);
                model.HataMesaj += PoliceBitisTarihiControl(model.PoliceBitisTarihi);
                model.HataMesaj += PoliceTanimTarihiControl(model.TanzimTarihi);
                model.HataMesaj += OdemeSekliKontrol(model.OdemeSekli);
                model.HataMesaj += OdemeTipiKontrol(model.OdemeTipi);
                model.HataMesaj += SatisTuruKontrol(model.SatisTuru);
                //model.HataMesaj += SatisTemsilcisiTCKNKontrol(model.SatisTemsilcisiTCKN);

            }
            return model;
        }

        #region Kontrol Metodları

        private string UrunKoduKontrol(string urunAdi)
        {
            if (!String.IsNullOrEmpty(urunAdi))
            {
                int urunkodu = 0;
                if (int.TryParse(urunAdi, out urunkodu))
                {
                    Urun urun = _UrunService.GetUrun(urunkodu);
                    if (urun == null) return "Bu ürün veri tabanına henüz eklenmemiş.";
                    return "";
                }
                return "Ürün kodu hatalı.";
            }
            else
                return "Ürün Adı Boş Olamaz.";
        }

        private string TVMKoduKontrol(string tvmKodu, string mtTCKN, string policeNo)
        {
            if (!String.IsNullOrEmpty(tvmKodu) && !String.IsNullOrEmpty(mtTCKN) && !String.IsNullOrEmpty(policeNo))
            {
                int kodu = 1;

                if (int.TryParse(tvmKodu, out kodu))
                {
                    int count = _TvmContext.TVMDetayRepository.Filter(s => s.Kodu == kodu).Count();
                    if (count == 0)
                    { return "Tvm kodu hatalı."; }
                    {
                        if (!_TVMService.KullaniciTvmyiGormeyeYetkiliMi(kodu))
                            return "TVM yetkiniz dışında.";
                    }

                    count = _TvmContext.TVMKullanicilarRepository.Filter(s => s.TCKN == mtTCKN && s.TVMKodu == kodu).Count();
                    if (count < 1)
                        return "MT kodu hatalı.";

                    return _RaporService.PoliceNoKontrol(policeNo, kodu);
                }
                else
                    return "Tvm kodu ve MT kodu rakam olmalı.";
            }
            else
                return "TVM Kodu, Poliçe no ve Mt boş olamaz.";
        }

        private string SigortaEttirenTipiKimlikNoKontrol(string kimlikNo, string tvmkodu)
        {
            if (!String.IsNullOrEmpty(kimlikNo) && !String.IsNullOrEmpty(tvmkodu))
            {
                int TVMkodu = 0;
                if (int.TryParse(tvmkodu, out TVMkodu))
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, TVMkodu);
                    if (musteri == null) return "Sigorta Ettiren müşteri bulunamadı.";
                    return "";
                }
                else
                    return "TVM kodu hatalı.";
            }
            else
                return "Sigorta Ettiren tipi ve kimlik no dolu olmalıdır.";
        }

        private string SigortaliTipiKimlikNoKontrol(string kimlikNo, string tvmkodu)
        {
            if (!String.IsNullOrEmpty(kimlikNo) && !String.IsNullOrEmpty(tvmkodu))
            {
                int TVMkodu = 0;
                if (int.TryParse(tvmkodu, out TVMkodu))
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, TVMkodu);
                    if (musteri == null) return "Sigortalı müşteri bulunamadı.";
                    return "";
                }
                else
                    return "TVM kodu hatalı.";
            }
            else
                return "";
        }

        private string OdemeSekliKontrol(string odemeSekli)
        {
            if (!String.IsNullOrEmpty(odemeSekli))
            {
                if (odemeSekli == Peşin || odemeSekli == Vadeli)
                    return "";
                else
                    return "Ödeme şekli hatalı.";
            }
            return "";
        }

        private string OdemeTipiKontrol(string odemeTipi)
        {
            if (!String.IsNullOrEmpty(odemeTipi))
            {
                if (odemeTipi == Nakit || odemeTipi == KrediKarti || odemeTipi == Havale)
                    return "";
                else
                    return "Ödeme tipi hatalı.";
            }
            return "";
        }

        private string SatisTuruKontrol(string satisTuru)
        {
            if (!String.IsNullOrEmpty(satisTuru))
            {
                if (satisTuru == YeniSatis || satisTuru == Yenileme)
                    return "";
                else
                    return "Satış türü hatalı.";
            }
            return "";
        }

        private string SatisTemsilcisiTCKNKontrol(string tckn)
        {
            if (!String.IsNullOrEmpty(tckn))
            {
                if (tckn.Length == 11)
                {
                    TVMKullanicilar MT = _KullaniciService.GetKullaniciByTCKN(tckn);
                    if (MT != null) return ""; else return "Satış temsilcisi bulunamadı";
                }
                else
                    return "Satış temsilcisi tckn 11 hane olmalı.";
            }
            return "";
        }

        private string BrutPrimControl(string brutPrim)
        {
            if (!String.IsNullOrEmpty(brutPrim))
            {
                decimal prim;

                if (decimal.TryParse(brutPrim, out prim))
                    return "";
                return "Brüt prim hatalı.";
            }
            else return "BrütPrim zorunludur.";
        }

        private string KomisyonControl(string komisyon)
        {
            if (!String.IsNullOrEmpty(komisyon))
            {
                decimal _komisyon;

                if (decimal.TryParse(komisyon, out _komisyon))
                    return "";
                return "Komisyon hatalı.";
            }
            else return "Komisyon zorunludur.";
        }

        private string PoliceBaslangicTarihiControl(string policeBaslangicTarihi)
        {
            if (!String.IsNullOrEmpty(policeBaslangicTarihi))
            {
                try
                {
                    double basT = double.Parse(policeBaslangicTarihi);
                    DateTime date = DateTime.FromOADate(basT);
                    return "";
                }
                catch (Exception)
                {
                    return "Police Başlangıç Tarihi hatalı";
                }
            }
            else return "Police Baslangic Tarihi zorunludur.";
        }

        private string PoliceBitisTarihiControl(string policeBitisTarihi)
        {
            if (!String.IsNullOrEmpty(policeBitisTarihi))
            {
                try
                {
                    double basT = double.Parse(policeBitisTarihi);
                    DateTime date = DateTime.FromOADate(basT);
                    return "";
                }
                catch (Exception)
                {
                    return "Police Bitiş Tarihi hatalı";
                }

            }
            else return "Police Bitiş Tarihi zorunludur.";
        }

        private string PoliceTanimTarihiControl(string policeTanzimTarihi)
        {
            if (!String.IsNullOrEmpty(policeTanzimTarihi))
            {
                try
                {
                    double basT = double.Parse(policeTanzimTarihi);
                    DateTime date = DateTime.FromOADate(basT);
                    return "";
                }
                catch (Exception)
                {
                    return "Police Tanzim Tarihi hatalı";
                }

            }
            else return "Police Tanzim Tarihi zorunludur.";
        }


        #endregion
    }

    public class ExcelOfflinePoliceListModel
    {
        public List<ExcelOfflinePoliceModel> HatasizKayitlar { get; set; }
        public List<ExcelOfflinePoliceModel> HataliKayitlar { get; set; }
        public string DosyaAdresi { get; set; }
    }

    public class ExcelOfflinePoliceModel
    {
        [IgnoreMap]
        public string HataMesaj { get; set; }

        [IgnoreMap]
        public string[] hatalist { get; set; }

        //Genel Bilgiler
        public string UrunKodu { get; set; }
        public string TVMKodu { get; set; }
        public string SEKimlikNo { get; set; }
        public string SETVMMusteriKodu { get; set; }
        public string SKimlikNo { get; set; }
        public string STVMMusteriKodu { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }
        public string BrutPrim { get; set; }
        public string Komisyon { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public string TanzimTarihi { get; set; }
        public string TaksitAdedi { get; set; }
        public string OdemeSekli { get; set; }
        public string OdemeTipi { get; set; }
        public string KrediKartiBankaAdi { get; set; }
        public string SatisTuru { get; set; }
        public string SatisTemsilcisiTCKN { get; set; }
    }

    #endregion


    #region AegonRapor

    public class AegonTeklifRaporuListe : DataTableParameters<AegonTeklifRaporuProcedureModel>
    {
        public AegonTeklifRaporuListe(HttpRequestBase httpRequest, Expression<Func<AegonTeklifRaporuProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public AegonTeklifRaporuListe(HttpRequestBase httpRequest,
                                      Expression<Func<AegonTeklifRaporuProcedureModel, object>>[] selectColumns,
                                      Expression<Func<AegonTeklifRaporuProcedureModel, object>> rowIdColumn,
                                      Expression<Func<AegonTeklifRaporuProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }


        public string Urunler { get; set; }
        public string Subeler { get; set; }
        public string BolgeKodu { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public Nullable<int> TeklifNo { get; set; }
        public Nullable<int> YillikMin { get; set; }
        public Nullable<int> YillikMax { get; set; }

        public int TeklifTarihi { get; set; }
        public int ParaBirimi { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }

    public class AegonTeklifRaporuExcelAktar
    {
        public AegonTeklifRaporuExcelAktar()
        {
            this.Urunler = String.Empty;
            this.Subeler = String.Empty;
            this.BolgeKodu = String.Empty;
        }

        public string Urunler { get; set; }
        public string Subeler { get; set; }
        public string BolgeKodu { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public int TeklifNo { get; set; }
        public int YillikMin { get; set; }
        public int YillikMax { get; set; }

        public int TeklifTarihi { get; set; }
        public int ParaBirimi { get; set; }

    }

    public class AegonTeklifAraListe : DataTableParameters<AegonTeklifAraProcedureModel>
    {
        public AegonTeklifAraListe(HttpRequestBase httpRequest, Expression<Func<AegonTeklifAraProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public AegonTeklifAraListe(HttpRequestBase httpRequest,
                                      Expression<Func<AegonTeklifAraProcedureModel, object>>[] selectColumns,
                                      Expression<Func<AegonTeklifAraProcedureModel, object>> rowIdColumn,
                                      Expression<Func<AegonTeklifAraProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public DateTime? BaslangisTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? TVMKodu { get; set; }
        public int? UrunKodu { get; set; }
        public int? HazirlayanKodu { get; set; }
        public int? TeklifNo { get; set; }
        public int? TeklifDurumu { get; set; }
        public int? MusteriKodu { get; set; }
        public string DetailIcon { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }
    
    #endregion

    #region Mapfre Teklif Ara 

    public class MapfreTeklifAraListe : DataTableParameters<MapfreTeklifAraProcedureModel>
    {
        public MapfreTeklifAraListe(HttpRequestBase httpRequest, Expression<Func<MapfreTeklifAraProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MapfreTeklifAraListe(HttpRequestBase httpRequest,
                                      Expression<Func<MapfreTeklifAraProcedureModel, object>>[] selectColumns,
                                      Expression<Func<MapfreTeklifAraProcedureModel, object>> rowIdColumn,
                                      Expression<Func<MapfreTeklifAraProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public DateTime? BaslangisTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? TVMKodu { get; set; }
        public int? UrunKodu { get; set; }
        public int? HazirlayanKodu { get; set; }
        public int? TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public int? TeklifDurumu { get; set; }
        public int? MusteriKodu { get; set; }
        public string DetailIcon { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }


    #endregion

    public class GenelRaporListe : DataTableParameters<GenelRaporProcedureModel>
    {
        public GenelRaporListe(HttpRequestBase httpRequest, Expression<Func<GenelRaporProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public GenelRaporListe(HttpRequestBase httpRequest,
                                      Expression<Func<GenelRaporProcedureModel, object>>[] selectColumns,
                                      Expression<Func<GenelRaporProcedureModel, object>> rowIdColumn,
                                      Expression<Func<GenelRaporProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public int TVMKodu { get; set; }
        public string ProjeKodu { get; set; }



    }

    public class TeklifAraListe : DataTableParameters<TeklifAraProcedureModel>
    {
        public TeklifAraListe(HttpRequestBase httpRequest, Expression<Func<TeklifAraProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public TeklifAraListe(HttpRequestBase httpRequest,
                                      Expression<Func<TeklifAraProcedureModel, object>>[] selectColumns,
                                      Expression<Func<TeklifAraProcedureModel, object>> rowIdColumn,
                                      Expression<Func<TeklifAraProcedureModel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }

        public DateTime? BaslangisTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        public int? TVMKodu { get; set; }
        public int? TUMKodu { get; set; }
        public int? UrunKodu { get; set; }
        public int? HazirlayanKodu { get; set; }

        public int? TeklifNo { get; set; }

        public int? TeklifDurumu { get; set; }
        public int? MusteriKodu { get; set; }
        public string DetailIcon { get; set; }


        public int skip { get; set; }
        public int take { get; set; }


    }

    public class AtananIslerimListe : DataTableParameters<AtananIslerProcedureModel>
    {
        public AtananIslerimListe(HttpRequestBase httpRequest, Expression<Func<AtananIslerProcedureModel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public AtananIslerimListe(HttpRequestBase httpRequest,
                                  Expression<Func<AtananIslerProcedureModel, object>>[] selectColumns,
                                  Expression<Func<AtananIslerProcedureModel, object>> rowIdColumn,
                                  Expression<Func<AtananIslerProcedureModel, object>> linkColumn1,
                                  string linkColumn1Url,
                                  string updateURL)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL)
        {

        }
        
        public Nullable<int> TvmKodu { get; set; }
        public Nullable<int> KullaniciList { get; set; }
        public Nullable<DateTime> IsBaslangicTarihi { get; set; }
        public Nullable<DateTime> IsBasBitisTarihi { get; set; }
        public Nullable<byte> Durum { get; set; }
        public Nullable<byte> IsTipi { get; set; }
        public Nullable<byte> OncelikSeviyesi { get; set; }        

    }
}

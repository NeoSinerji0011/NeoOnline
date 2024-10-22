using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class Musteri
    {
        public Musteri()
        {

        }

        public void AddAdres(MusteriAdre adres)
        {
            adres.Varsayilan = true;
            this.Adresler.Add(adres);
        }

        public void AddTelefon(MusteriTelefon telefon)
        {
            this.Telefonlar.Add(telefon);
        }

        public void AddDokuman(MusteriDokuman dokuman)
        {
            this.Dokumanlar.Add(dokuman);
        }

        public void AddNot(MusteriNot not)
        {
            this.Notlar.Add(not);
        }

        private List<MusteriAdre> _Adresler;
        public List<MusteriAdre> Adresler
        {
            get
            {
                if (_Adresler == null)
                    _Adresler = new List<MusteriAdre>();

                return _Adresler;
            }
            set
            {
                _Adresler = value;
            }
        }

        private List<MusteriTelefon> _Telefonlar;
        public List<MusteriTelefon> Telefonlar
        {
            get
            {
                if (_Telefonlar == null)
                    _Telefonlar = new List<MusteriTelefon>();
                return _Telefonlar;
            }
            set
            {
                _Telefonlar = value;
            }
        }

        private List<MusteriDokuman> _Dokumanlar;
        public List<MusteriDokuman> Dokumanlar
        {
            get
            {
                if (_Dokumanlar == null)
                    _Dokumanlar = new List<MusteriDokuman>();
                return _Dokumanlar;
            }
            set
            {
                _Dokumanlar = value;
            }
        }

        private List<MusteriNot> _Notlar;
        public List<MusteriNot> Notlar
        {
            get
            {
                if (_Notlar == null)
                    _Notlar = new List<MusteriNot>();
                return _Notlar;
            }
            set
            {
                _Notlar = value;
            }
        }
    }

    public class MusteriListe : DataTableParameters<MusteriListeModelOzel>
    {
        public MusteriListe(HttpRequestBase httpRequest, Expression<Func<MusteriListeModelOzel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MusteriListe(HttpRequestBase httpRequest,
                                      Expression<Func<MusteriListeModelOzel, object>>[] selectColumns,
                                      Expression<Func<MusteriListeModelOzel, object>> rowIdColumn,
                                      Expression<Func<MusteriListeModelOzel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL, string gorevUrl)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL, gorevUrl)
        {

        }

        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string EMail { get; set; }
        public Nullable<short> MusteriTipKodu { get; set; }

        public string PasaportNo { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public string KimlikNo { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }

    }

    public class MusteriListeModelOzel
    {
        public int TeklifId { get; set; }
        public int MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }

        public short MusteriTipKodu { get; set; }
        //public string MusteriTipiText { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string Cinsiyet { get; set; }

        public string DogumTarihi { get; set; }
        public string EMail { get; set; }
        public int TVMKodu { get; set; }
        public string BagliOlduguTvmText { get; set; }



    }




    public class MusteriListesi : DataTableParameters<MusteriListesiModelOzel>
    {
        public MusteriListesi(HttpRequestBase httpRequest, Expression<Func<MusteriListesiModelOzel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MusteriListesi(HttpRequestBase httpRequest,
                                      Expression<Func<MusteriListesiModelOzel, object>>[] selectColumns,
                                      Expression<Func<MusteriListesiModelOzel, object>> rowIdColumn,
                                      Expression<Func<MusteriListesiModelOzel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL, string gorevUrl)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL, gorevUrl)
        {

        }

        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string EMail { get; set; }
        public Nullable<short> MusteriTipKodu { get; set; }

        public string PasaportNo { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public string KimlikNo { get; set; }
        public Nullable<int> MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }

        public string YasGrubu { get; set; }

        public SelectList YasGruplari { get; set; }

        public string Cinsiyet { get; set; }

        public List<SelectListItem> Meslekler { get; set; }
        public int? MeslekKodu { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public int yasBaslangic { get; set; }
        public int yasBitis { get; set; }
        public string dogBas { get; set; }
        public string dogBit { get; set; }
        public string MeslekKoduText { get; set; }

    }



    public class MusteriListesiModelOzel
    {
        public string AdiSoyadiUnvan { get; set; }
        public string KimlikNo { get; set; }
        public int MusteriKodu { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string MusteriTipiText { get; set; }

        public string MeslekKoduText { get; set; }

        public string EgitimDurumuText { get; set; }

        public string MedeniDurumuText { get; set; }

        public string BagliOlduguTvmText { get; set; }
        public string CepTel { get; set; }
        public string EvTel { get; set; }
        public string EMail { get; set; }
        public string KaydedenKullanici { get; set; }
        public string IlIlce { get; set; }
        public short MusteriTipKodu { get; set; }
        public int? MeslekKodu { get; set; }
        public short? EgitimDurumu { get; set; }
        public byte? MedeniDurumu { get; set; }
        public int TVMKodu { get; set; }
        public List<MusteriTelefon> Telefons { get; set; }
        public List<SelectListItem> Meslekler { get; set; }

    }

    public class MusteriAdedi : DataTableParameters<MusteriAdediModelOzel>
    {
        public MusteriAdedi(HttpRequestBase httpRequest, Expression<Func<MusteriAdediModelOzel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public MusteriAdedi(HttpRequestBase httpRequest,
                                      Expression<Func<MusteriAdediModelOzel, object>>[] selectColumns,
                                      Expression<Func<MusteriAdediModelOzel, object>> rowIdColumn,
                                      Expression<Func<MusteriAdediModelOzel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL, string gorevUrl)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL, gorevUrl)
        {

        }
        public string TVMKodlari { get; set; }

    }

    public class MusteriAdediModelOzel
    {
       
        public string tvmUnvani { get; set; }
        public int TCMusteriCount { get; set; }
        public int TuzelMusteriCount { get; set; }
        public int SahisFirmasiCount { get; set; }
        public int YabanciMusteriCount { get; set; }
        public int MusteriToplamCount { get; set; }

        public string TCMusteriCountText { get; set; }
        public string TuzelMusteriCountText { get; set; }
        public string SahisFirmasiCountText { get; set; }
        public string YabanciMusteriCountText { get; set; }
        public string MusteriToplamCountText { get; set; }
        

    }


    //Musterilerim harita uzerinde gosteriliyor
    public class MusteriHaritaOzelDetay
    {
        public int MusteriKodu { get; set; }
        public string AdiSoyadi { get; set; }
        public string Tel { get; set; }
        public short MusteriTipi { get; set; }
        public string Email { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    //Musterilerim harita arama modeli
    public class MusteriharitaAramaModel
    {
        public int TVMKodu { get; set; }
        public int MusteriTipi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public byte MusteriSayisi { get; set; }
    }



    //Müşterinin Dökümanlarını dönüyor..
    public class DokumanDetayModel
    {
        public int MusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public string DokumanTuru { get; set; }
        public string DokumanURL { get; set; }
        public string DosyaAdi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int TVMKodu { get; set; }
        public int TVMPersonelKodu { get; set; }
        public string TvmPersonelAdi { get; set; }
    }

    public class NotModelDetay
    {
        public int MusteriKodu { get; set; }
        public string Konu { get; set; }
        public string NotAciklamasi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int TVMKodu { get; set; }
        public int TVMPersonelKodu { get; set; }
        public int SiraNo { get; set; }
        public string TvmPersonelAdi { get; set; }

        //Bu prop  guncelleme için kullanılıyor..
        public string sayfaadi { get; set; }
    }

    public class MusteriFinderOzetModel
    {
        public int MusteriKodu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class PotansiyelMusteri
    {
        public PotansiyelMusteri()
        {

        }

        public void AddAdres(PotansiyelMusteriAdre adres)
        {
            adres.Varsayilan = true;
            this.Adresler.Add(adres);
        }

        public void AddTelefon(PotansiyelMusteriTelefon telefon)
        {
            this.Telefonlar.Add(telefon);
        }

        public void AddDokuman(PotansiyelMusteriDokuman dokuman)
        {
            this.Dokumanlar.Add(dokuman);
        }

        public void AddNot(PotansiyelMusteriNot not)
        {
            this.Notlar.Add(not);
        }

        private List<PotansiyelMusteriAdre> _Adresler;
        public List<PotansiyelMusteriAdre> Adresler
        {
            get
            {
                if (_Adresler == null)
                    _Adresler = new List<PotansiyelMusteriAdre>();

                return _Adresler;
            }
            set
            {
                _Adresler = value;
            }
        }

        private List<PotansiyelMusteriTelefon> _Telefonlar;
        public List<PotansiyelMusteriTelefon> Telefonlar
        {
            get
            {
                if (_Telefonlar == null)
                    _Telefonlar = new List<PotansiyelMusteriTelefon>();
                return _Telefonlar;
            }
            set
            {
                _Telefonlar = value;
            }
        }

        private List<PotansiyelMusteriDokuman> _Dokumanlar;
        public List<PotansiyelMusteriDokuman> Dokumanlar
        {
            get
            {
                if (_Dokumanlar == null)
                    _Dokumanlar = new List<PotansiyelMusteriDokuman>();
                return _Dokumanlar;
            }
            set
            {
                _Dokumanlar = value;
            }
        }

        private List<PotansiyelMusteriNot> _Notlar;
        public List<PotansiyelMusteriNot> Notlar
        {
            get
            {
                if (_Notlar == null)
                    _Notlar = new List<PotansiyelMusteriNot>();
                return _Notlar;
            }
            set
            {
                _Notlar = value;
            }
        }
    }

    public class PotansiyelMusteriListe : DataTableParameters<PotansiyelMusteriListeModelOzel>
    {
        public PotansiyelMusteriListe(HttpRequestBase httpRequest, Expression<Func<PotansiyelMusteriListeModelOzel, object>>[] selectColumns)
            : base(httpRequest, selectColumns)
        {

        }

        public PotansiyelMusteriListe(HttpRequestBase httpRequest,
                                      Expression<Func<PotansiyelMusteriListeModelOzel, object>>[] selectColumns,
                                      Expression<Func<PotansiyelMusteriListeModelOzel, object>> rowIdColumn,
                                      Expression<Func<PotansiyelMusteriListeModelOzel, object>> linkColumn1,
                                      string linkColumn1Url,
                                      string updateURL,
                                      string gorevUrl)
            : base(httpRequest, selectColumns, rowIdColumn, linkColumn1, linkColumn1Url, updateURL,gorevUrl)
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

    public class PotansiyelMusteriListeModelOzel
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

    //Yönetim Ekranında son eklenen 5 müşteri için oluşturulmuş model
    public class PotansiyelMusteriOzelDetay
    {
        public int MusteriKodu { get; set; }
        public string AdiSoyadi { get; set; }
        public DateTime Kayittarihi { get; set; }
        public string MusteriTipi { get; set; }
        public string Email { get; set; }
    }

    //Müşterinin Dökümanlarını dönüyor..
    public class PotansiyelDokumanDetayModel
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

    public class PotansiyelNotModelDetay
    {
        public int PotansiyelMusteriKodu { get; set; }
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

    public class PotansiyelMusteriFinderOzetModel
    {
        public int MusteriKodu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
    }

}

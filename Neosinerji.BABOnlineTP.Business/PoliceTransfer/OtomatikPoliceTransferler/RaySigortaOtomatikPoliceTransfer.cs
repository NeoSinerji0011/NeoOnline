using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using DataSharetest.DataShare;
using System.IO;
using Neosinerji.BABOnlineTP.Business.axasigorta.hayat;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Services.Protocols;
using RayWebService;
using Newtonsoft.Json;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string[] SplitToChunks(this string source, int maxLength)
        {
            return source
                .Where((x, i) => i % maxLength == 0)
                .Select(
                    (x, i) => new string(source
                        .Skip(i * maxLength)
                        .Take(maxLength)
                        .ToArray()))
                .ToArray();
        }
    }

    public class RaySigortaOtomatikPoliceTransfer : IRaySigortaOtomatikPoliceTransfer
    {
        private int _tvmKodu;
        private string _acenteNo;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;
        IBransUrunService _BransUrunService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;
        public RaySigorta.PoliceListesiOutputEntity policeListesiOutputEntity;

        public class NameWrapper
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public NameWrapper()
            {
                this.FirstName = "";
                this.LastName = "";
            }
        }
        public static NameWrapper SplitName(string inputStr, char splitChar)
        {
            NameWrapper w = new NameWrapper();
            string[] strArray = inputStr.Trim().Split(splitChar);
            if (string.IsNullOrEmpty(inputStr))
            {
                return w;
            }


            for (int i = 0; i < strArray.Length; i++)
            {
                if (i == 0)
                {
                    w.FirstName = strArray[i];
                }
                else
                {
                    w.LastName += strArray[i] + " ";
                }
            }
            w.LastName = w.LastName.Trim();

            return w;

        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        //tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu, partajNo
        public RaySigortaOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu, string acenteNo)
        {
            this._tvmKodu = tvmKodu;
            this._acenteNo = acenteNo;
            this._sirketKodu = sirketKodu;
            this._serviceURL = serviceURL;
            this._KullaniciAdi = KullaniciAdi;
            this._Sifre = Sifre;
            this._TanzimBaslangicTarihi = TanzimBaslangicTarihi;
            this._TanzimBitisTarihi = TanzimBitisTarihi;
        }
        public List<Police> GetRAYHayatAutoPoliceTransfer()
        {
            #region Service DependencyResolver

            IPoliceTransferService _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();

            IPoliceContext _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
            _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();

            IPoliceTransferStorage _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();
            _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();

            IAktifKullaniciService _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            ITVMService _ITVMService = DependencyResolver.Current.GetService<ITVMService>();


            #endregion

            _BransUrunService = DependencyResolver.Current.GetService<IBransUrunService>();
            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList("042");

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();


            string acenteNo = this._acenteNo;
            string ipNo = "88.247.127.91";
            var data2 = AppDomain.CurrentDomain.BaseDirectory + @"\Files\reader_data.json";
            var logfile = AppDomain.CurrentDomain.BaseDirectory + @"\Files\log.json";
            var data = File.ReadAllText(data2);
            dynamic qwe = JsonConvert.DeserializeObject(data);
            foreach (var item in qwe)
            {
                if (_sirketKodu == item.TumBirlikKodu.Value)
                {
                    ipNo = item.Ip.Value;
                }
            }

            string kullaniciAdi = this._KullaniciAdi;
            string sifre = this._Sifre;
            RayApi rayApi = new RayApi(acenteNo, ipNo, kullaniciAdi, sifre);
            List<Police> returnPoliceler = new List<Police>();

            try
            {
                foreach (DateTime day in EachDay(this._TanzimBaslangicTarihi, this._TanzimBitisTarihi))
                {
                    var policeListe = rayApi.PoliceListe(day);

                    if (policeListe.Count > 0)
                    {
                        Police policeItem = new Police();
                        #region liste
                        try
                        {
                            if (policeListe.ToList().Count > 0)
                            {
                                for (int i = 0; i < policeListe.ToList().Count; i++)
                                {
                                    policeItem = new Police();
                                    policeItem.GenelBilgiler.PoliceArac = new PoliceArac();
                                    policeItem.GenelBilgiler.PoliceRizikoAdresi = new PoliceRizikoAdresi();
                                    var police = policeListe[i].Policeler[0];

                                    #region Poliçe Genel Bilgiler
                                    if (_tvmKodu > 0) policeItem.GenelBilgiler.TVMKodu = _tvmKodu;
                                    else policeItem.GenelBilgiler.TVMKodu = 0;
                                    policeItem.GenelBilgiler.PoliceNumarasi = police.PoliceNo.ToString();
                                    policeItem.GenelBilgiler.YenilemeNo = (int)police.YenilemeNo;
                                    policeItem.GenelBilgiler.EkNo = (int)0;//police.EkbelgeNo;
                                    policeItem.GenelBilgiler.ZeyilAdi = police.ZeyilNo.ToString(); //police.EkbelgeAdi;
                                    policeItem.GenelBilgiler.ZeyilKodu = police.ZeyilTipKodu.ToString(); // police.EkbelgeKodu.ToString();
                                    policeItem.GenelBilgiler.TUMBransAdi = ""; // police.Urun.BransAdi;
                                    policeItem.GenelBilgiler.BaslangicTarihi = police.BaslangicTarihi;
                                    policeItem.GenelBilgiler.BitisTarihi = police.BitisTarihi;
                                    policeItem.GenelBilgiler.TanzimTarihi = police.OnayTarihi; // police.TanzimTarihi;
                                    policeItem.GenelBilgiler.NetPrim = police.NetPrim;
                                    policeItem.GenelBilgiler.BrutPrim = police.BrutPrim;
                                    policeItem.GenelBilgiler.ToplamVergi = police.GiderVergisi;
                                    policeItem.GenelBilgiler.Komisyon = police.Komisyon;
                                    policeItem.GenelBilgiler.DovizKur = police.DovizKuru;
                                    policeItem.GenelBilgiler.ParaBirimi = police.DovizCinsi;
                                    policeItem.GenelBilgiler.Durum = 0;
                                    policeItem.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.RAYSIGORTA;

                                    var PoliceBransEslestir = Util.PoliceBransAdiEslestir(SigortaSirketiBransList, branslar, police.UrunAdi, police.UrunNo.ToString());
                                    policeItem.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                                    policeItem.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                                    policeItem.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                                    policeItem.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                                    policeItem.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;
                                    policeItem.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                                    #endregion

                                    #region Poliçe Sigortalı
                                    if (police.SigortaliAdiSoyadi != null)
                                    {
                                        if (!String.IsNullOrEmpty(police.SigortaliTcKimlikNo.ToString()) || police.SigortaliTcKimlikNo.ToString() != "0")
                                        {
                                            policeItem.GenelBilgiler.PoliceSigortali.KimlikNo = police.SigortaliTcKimlikNo.ToString();
                                            policeItem.GenelBilgiler.PoliceSigortali.AdiUnvan = StringExt.Truncate(SplitName(police.SigortaliAdiSoyadi, ' ').FirstName.Trim(), 150);  // 150 karakterden uzun girilemiyor
                                            policeItem.GenelBilgiler.PoliceSigortali.SoyadiUnvan = StringExt.Truncate(SplitName(police.SigortaliAdiSoyadi, ' ').LastName.Trim(), 50);    // 50 karakterden uzun girilemiyor
                                        }
                                        else if (!String.IsNullOrEmpty(police.SigortaliVergiNo.ToString()) || police.SigortaliVergiNo.ToString() != "0")
                                        {
                                            policeItem.GenelBilgiler.PoliceSigortali.VergiKimlikNo = police.SigortaliVergiNo.ToString();

                                            // şirket isminde ilk 150 karakteri alıp adına, kalan kısmı soyadına yazar (max 50 karakter)
                                            string[] tempUnvanList = StringExt.SplitToChunks(police.SigortaliAdiSoyadi.ToString().Trim(), 150);
                                            policeItem.GenelBilgiler.PoliceSigortali.AdiUnvan = StringExt.Truncate(tempUnvanList[0].Trim(), 150);
                                            if (tempUnvanList.Count() >= 2)
                                            {
                                                policeItem.GenelBilgiler.PoliceSigortali.SoyadiUnvan = StringExt.Truncate(tempUnvanList[1], 50); // 50 karakterden uzun girilemiyor
                                            }
                                        }


                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = "";
                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = police.MusteriDogumTarihi;
                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.Adres = police.MusteriAdresi;
                                    }

                                    #endregion

                                    #region Poliçe Sigorta Ettiren
                                    if (police.MusteriAdiSoyadi != null)
                                    {
                                        if (!String.IsNullOrEmpty(police.MusteriTcKimlikNo.ToString()))
                                        {
                                            policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = police.MusteriTcKimlikNo.ToString();
                                            policeItem.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = StringExt.Truncate(SplitName(police.MusteriAdiSoyadi, ' ').FirstName.Trim(), 150);  // 150 karakterden uzun girilemiyor
                                            policeItem.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = StringExt.Truncate(SplitName(police.MusteriAdiSoyadi, ' ').LastName.Trim(), 50);    // 50 karakterden uzun girilemiyor
                                        }
                                        else if (!String.IsNullOrEmpty(police.MusteriVergiNo.ToString()))
                                        {
                                            policeItem.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = police.MusteriVergiNo.ToString();

                                            // şirket isminde ilk 150 karakteri alıp adına, kalan kısmı soyadına yazar (max 50 karakter)
                                            string[] tempUnvanList = StringExt.SplitToChunks(police.MusteriAdiSoyadi.ToString().Trim(), 150);
                                            policeItem.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = StringExt.Truncate(tempUnvanList[0].Trim(), 150);
                                            if (tempUnvanList.Count() >= 2)
                                            {
                                                policeItem.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = StringExt.Truncate(tempUnvanList[1], 50); // 50 karakterden uzun girilemiyor
                                            }
                                        }


                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = "";
                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = police.MusteriDogumTarihi;
                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.Adres = police.MusteriAdresi;
                                    }

                                    #endregion
                                    #region vergiler
                                    PoliceVergi gv = new PoliceVergi();
                                    if (policeItem.GenelBilgiler.DovizKur != 1 && policeItem.GenelBilgiler.DovizKur != 0 && policeItem.GenelBilgiler.DovizKur != null)
                                    {
                                        policeItem.GenelBilgiler.ToplamVergi = police.GiderVergisiDVZ;
                                        gv.VergiTutari = police.GiderVergisiDVZ;
                                    }
                                    else
                                    {
                                        policeItem.GenelBilgiler.ToplamVergi = police.GiderVergisi;
                                        gv.VergiTutari = police.GiderVergisi;
                                    }
                                    gv.VergiKodu = 2;
                                    policeItem.GenelBilgiler.PoliceVergis.Add(gv);
                                    #endregion

                                    PoliceOdemePlani polOdeme = new PoliceOdemePlani();
                                    #region Taksitler
                                    if (police.Taksitler != null)
                                    {
                                        // taksitli ödeme
                                        foreach (var item in police.Taksitler)
                                        {
                                            polOdeme = new PoliceOdemePlani();
                                            polOdeme.TaksitNo = item.TaksitSayisi;
                                            polOdeme.TaksitTutari = Convert.ToDecimal(item.TaksitTutariTL);
                                            polOdeme.VadeTarihi = item.TaksitTarihi;
                                            polOdeme.DovizliTaksitTutari = Convert.ToDecimal(item.TaksitTutariDVZ);
                                            polOdeme.OdemeTipi = OdemeTipleri.Havale; // @todo: değişmesi gerekebilir
                                            policeItem.GenelBilgiler.PoliceOdemePlanis.Add(polOdeme);

                                            if (policeItem.GenelBilgiler.BransKodu.Value == 1 || policeItem.GenelBilgiler.BransKodu.Value == 2)
                                            {
                                                PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                polOdeme.OdemeTipi = OdemeTipleri.KrediKarti;
                                                tahsilat.OtomatikTahsilatiKkMi = 1;
                                                tahsilat.TaksitVadeTarihi = item.TaksitTarihi != null ? item.TaksitTarihi : policeItem.GenelBilgiler.BaslangicTarihi.Value;
                                                tahsilat.TaksitNo = item.TaksitSayisi;
                                                tahsilat.OdemeBelgeTarihi = item.TaksitTarihi;
                                                tahsilat.OdemeBelgeNo = "111111****1111";
                                                tahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                                tahsilat.OdenenTutar = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                                tahsilat.KalanTaksitTutari = 0;
                                                tahsilat.PoliceNo = policeItem.GenelBilgiler.PoliceNumarasi;
                                                tahsilat.ZeyilNo = policeItem.GenelBilgiler.EkNo.ToString();
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : policeItem.GenelBilgiler.PoliceSigortali.KimlikNo;
                                                tahsilat.BrutPrim = policeItem.GenelBilgiler.BrutPrim.HasValue ? policeItem.GenelBilgiler.BrutPrim.Value : 0;
                                                tahsilat.PoliceId = policeItem.GenelBilgiler.PoliceId;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = policeItem.GenelBilgiler.TVMKodu.Value;
                                                if (tahsilat.TaksitTutari != 0)
                                                {
                                                    policeItem.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                            else
                                            {
                                                PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                polOdeme.OdemeTipi = OdemeTipleri.Havale;
                                                tahsilat.TaksitVadeTarihi = item.TaksitTarihi != null ? item.TaksitTarihi : policeItem.GenelBilgiler.BaslangicTarihi.Value;
                                                tahsilat.TaksitNo = item.TaksitSayisi;
                                                tahsilat.OdemeBelgeTarihi = item.TaksitTarihi;
                                                //tahsilat.OdemeBelgeNo = "111111";
                                                tahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                                tahsilat.OdenenTutar = 0;
                                                tahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                                tahsilat.PoliceNo = policeItem.GenelBilgiler.PoliceNumarasi;
                                                tahsilat.ZeyilNo = policeItem.GenelBilgiler.EkNo.ToString();
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : policeItem.GenelBilgiler.PoliceSigortali.KimlikNo;
                                                tahsilat.BrutPrim = policeItem.GenelBilgiler.BrutPrim.HasValue ? policeItem.GenelBilgiler.BrutPrim.Value : 0;
                                                tahsilat.PoliceId = policeItem.GenelBilgiler.PoliceId;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = policeItem.GenelBilgiler.TVMKodu.Value;
                                                if (tahsilat.TaksitTutari != 0)
                                                {
                                                    policeItem.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        polOdeme.TaksitTutari = policeItem.GenelBilgiler.BrutPrim;
                                        if (policeItem.GenelBilgiler.DovizKur != 1 && policeItem.GenelBilgiler.DovizKur != 0 && policeItem.GenelBilgiler.DovizKur != null)
                                        {
                                            polOdeme.TaksitTutari = Math.Round(policeItem.GenelBilgiler.BrutPrim.Value * policeItem.GenelBilgiler.DovizKur.Value, 2);
                                        }
                                        if (policeItem.GenelBilgiler.ParaBirimi != "YTL" && policeItem.GenelBilgiler.ParaBirimi != "TL")
                                        {
                                            polOdeme.DovizliTaksitTutari = policeItem.GenelBilgiler.DovizliBrutPrim.Value;
                                        }
                                        if (polOdeme.VadeTarihi == null)
                                        {
                                            polOdeme.VadeTarihi = policeItem.GenelBilgiler.BaslangicTarihi;
                                        }
                                        polOdeme.OdemeTipi = OdemeTipleri.Havale;
                                        polOdeme.TaksitNo = 1;
                                        if (polOdeme.TaksitTutari != 0)
                                        {

                                            policeItem.GenelBilgiler.PoliceOdemePlanis.Add(polOdeme);
                                        }

                                        // tahsilat kısmı axa'dan alındı düzenlenmesi gerekebilir

                                        if (policeItem.GenelBilgiler.BransKodu.Value == 1 || policeItem.GenelBilgiler.BransKodu.Value == 2)
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                            polOdeme.OdemeTipi = OdemeTipleri.KrediKarti;
                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                            tahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : policeItem.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = polOdeme.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = polOdeme.VadeTarihi;
                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                            tahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.PoliceNo = policeItem.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = policeItem.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : policeItem.GenelBilgiler.PoliceSigortali.KimlikNo;
                                            tahsilat.BrutPrim = policeItem.GenelBilgiler.BrutPrim.HasValue ? policeItem.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilat.PoliceId = policeItem.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = policeItem.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                policeItem.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                        else
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            polOdeme.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : policeItem.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = polOdeme.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = polOdeme.VadeTarihi;
                                            //tahsilat.OdemeBelgeNo = "111111";
                                            tahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = policeItem.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = policeItem.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : policeItem.GenelBilgiler.PoliceSigortali.KimlikNo;
                                            tahsilat.BrutPrim = policeItem.GenelBilgiler.BrutPrim.HasValue ? policeItem.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilat.PoliceId = policeItem.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = policeItem.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                policeItem.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }

                                    }
                                    #endregion
                                    /*
                                    #region Poliçe Ödeme Planı

                                    if (police.OdemePlani != null)
                                    {

                                        for (int j = 0; j < police.OdemePlani.ToList().Count; j++)
                                        {
                                            polOdeme = new PoliceOdemePlani();
                                            var policeOdeme = police.OdemePlani[j];
                                            polOdeme.TaksitNo = j + 1; ;
                                            polOdeme.TaksitTutari = policeOdeme.VadeTutari;
                                            polOdeme.VadeTarihi = policeOdeme.VadeTarihi;
                                             polOdeme.OdemeTipi = policeOdeme.OdemeAraci;
                                            if (policeOdeme.OdemeAraci == "BANKA HAVALESİ")
                                            {
                                                polOdeme.OdemeTipi = OdemeTipleri.Havale;
                                            }
                                            policeItem.GenelBilgiler.PoliceOdemePlanis.Add(polOdeme);
                                        }

                                    }
                                    */

                                    #region Ödeme Şekli
                                    if (policeItem.GenelBilgiler.PoliceOdemePlanis != null)
                                    {
                                        if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count == 0) policeItem.GenelBilgiler.OdemeSekli = 0;
                                        if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count == 1) policeItem.GenelBilgiler.OdemeSekli = 1;
                                        if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count > 1) policeItem.GenelBilgiler.OdemeSekli = 2;
                                    }

                                    #endregion
                                    if (policeItem.GenelBilgiler.PoliceArac.TescilSeriNo != null)
                                    {
                                        policeItem.GenelBilgiler.PoliceArac.TescilSeriKod = policeItem.GenelBilgiler.PoliceArac.TescilSeriNo.Substring(0, 2);
                                        policeItem.GenelBilgiler.PoliceArac.TescilSeriNo = policeItem.GenelBilgiler.PoliceArac.TescilSeriNo.Substring(2);
                                    }

                                    // sorulara bak gerekli olanları kaydet
                                    try
                                    {
                                        if (police.Sorular != null)
                                        {
                                            foreach (var item in police.Sorular)
                                            {
                                                switch (item.SoruKodu.ToString())
                                                {
                                                    case "12": // SİGORTALI T. C. KİMLİK NO
                                                        policeItem.GenelBilgiler.PoliceSigortali.KimlikNo = item.Cevap.ToString();
                                                        break;

                                                    case "513": // TRAMER KULLANIM TARZI
                                                        policeItem.GenelBilgiler.PoliceArac.KullanimTarzi = item.Cevap.ToString();
                                                        break;

                                                    case "5000": // KULLANIM TARZI
                                                        policeItem.GenelBilgiler.PoliceArac.KullanimTarzi = item.Cevap.ToString();
                                                        break;

                                                    case "5002": // MARKA
                                                        policeItem.GenelBilgiler.PoliceArac.Marka = item.Cevap.ToString();
                                                        break;

                                                    case "5003": // TİP (SCIROCCO 2.0 TFSI 200 S.LINE PLUS T. DSG)
                                                        policeItem.GenelBilgiler.PoliceArac.AracinTipiAciklama = item.Cevap.ToString();
                                                        break;

                                                    case "5004": // MODEL (YIL)
                                                        policeItem.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(item.Cevap.ToString());
                                                        break;

                                                    case "5010": // MOTOR NO
                                                        policeItem.GenelBilgiler.PoliceArac.MotorNo = item.Cevap.ToString();
                                                        break;

                                                    case "5011": // ŞASİ NO
                                                        policeItem.GenelBilgiler.PoliceArac.SasiNo = item.Cevap.ToString();
                                                        break;

                                                    case "5166": // EGM KULLANIM ŞEKLİ
                                                        policeItem.GenelBilgiler.PoliceArac.KullanimSekli = item.Cevap.ToString();
                                                        break;

                                                    case "5165": // EGM ÜST CİNS
                                                        policeItem.GenelBilgiler.PoliceArac.Cinsi = item.Cevap.ToString().Substring(0,30);
                                                        break;

                                                    case "5171": // EGM KOLTUK SAYISI
                                                        policeItem.GenelBilgiler.PoliceArac.KoltukSayisi = int.Parse(item.Cevap.ToString());
                                                        break;


                                                    case "7018": // ARAÇ RENGİ
                                                        policeItem.GenelBilgiler.PoliceArac.Renk = item.Cevap.ToString();
                                                        break;

                                                    case "52516": // -----EGM_RENK
                                                        policeItem.GenelBilgiler.PoliceArac.Renk = item.Cevap.ToString();
                                                        break;

                                                    case "5014": // SİLİNDİR HACMİ
                                                        policeItem.GenelBilgiler.PoliceArac.SilindirHacmi = item.Cevap.ToString();
                                                        break;

                                                    case "7020": // MOTOR GÜCÜ
                                                        policeItem.GenelBilgiler.PoliceArac.MotorGucu = item.Cevap.ToString();
                                                        break;

                                                    case "5142": // TESCİL BELGE NO/ NOTER(ASBİS) REF.NO
                                                        policeItem.GenelBilgiler.PoliceArac.AsbisNo = item.Cevap.ToString();
                                                        break;

                                                    case "5164": // EGM YAKIT TİPİ
                                                        policeItem.GenelBilgiler.PoliceArac.YakitCinsi = item.Cevap.ToString();
                                                        break;

                                                    case "507": // TRAMER BELGE TARİHİ
                                                        if (!String.IsNullOrEmpty(item.Cevap.ToString().Trim()))
                                                        {
                                                            policeItem.GenelBilgiler.PoliceArac.TramerBelgeTarihi = DateTime.Parse(item.Cevap.ToString().Trim());
                                                        }
                                                        break;

                                                    case "5017": // TRAFİK TESCİL TARİHİ
                                                        if (!String.IsNullOrEmpty(item.Cevap.ToString().Trim()))
                                                        {
                                                            policeItem.GenelBilgiler.PoliceArac.TrafikTescilTarihi = DateTime.Parse(item.Cevap.ToString());
                                                        }
                                                        break;

                                                    case "52517": // EGM KOLTUK SAYISI
                                                        policeItem.GenelBilgiler.PoliceArac.KoltukSayisi = int.Parse(item.Cevap);
                                                        break;

                                                    case "52518": // CİNSİYET
                                                        policeItem.GenelBilgiler.PoliceSigortali.Cinsiyet = item.Cevap.ToString();
                                                        break;

                                                    case "55518": // SİGORTA ETTİREN TEL NO
                                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = item.Cevap.ToString();
                                                        policeItem.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo = item.Cevap.ToString();
                                                        break;

                                                    case "55519": // SİGORTALI TEL NO
                                                        policeItem.GenelBilgiler.PoliceSigortali.TelefonNo = item.Cevap.ToString();
                                                        policeItem.GenelBilgiler.PoliceSigortali.MobilTelefonNo = item.Cevap.ToString();
                                                        break;

                                                }


                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                    }



                                    // Plaka
                                    string plaka = police.Plaka;
                                    string plakaKodu = "";
                                    string plakaNo = "";
                                    bool plakaKodMu = true;
                                    if (!String.IsNullOrEmpty(plaka))
                                    {
                                        plaka = plaka.Trim();
                                    }
                                    for (int k = 0; k < plaka.Length; k++)
                                    {
                                        if (char.IsDigit(plaka[k]) && plakaKodMu)
                                        {
                                            plakaKodu += plaka[k];
                                        }
                                        else
                                        {
                                            plakaKodMu = false;
                                            plakaNo += plaka[k];
                                        }
                                    }
                                    if (String.IsNullOrEmpty(plakaNo))
                                    {
                                        if (!String.IsNullOrEmpty(plakaKodu) && plakaKodu.Substring(0, 1) == "0")
                                        {
                                            plakaNo = plakaKodu.Substring(3, (plakaKodu.Length - 4));
                                            plakaKodu = plakaKodu.Substring(0, 3);
                                        }
                                        else if (!String.IsNullOrEmpty(plakaKodu) && plakaKodu.Substring(0, 1) != "0")
                                        {
                                            plakaNo = plakaKodu.Substring(2, (plakaKodu.Length - 3));
                                            plakaKodu = plakaKodu.Substring(0, 2);
                                        }
                                    }
                                    policeItem.GenelBilgiler.PoliceArac.PlakaKodu = plakaKodu;
                                    policeItem.GenelBilgiler.PoliceArac.PlakaNo = plakaNo.Trim();

                                    returnPoliceler.Add(policeItem);

                                }
                            }

                            //if (returnPoliceler == null)
                            //{
                            //    _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu.");
                            //}
                        }
                        catch (Exception ex)
                        {
                            File.WriteAllText(logfile, ipNo + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

                            return null;
                            throw;
                        }
                        //Console.Write(returnPoliceler);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(logfile, ipNo + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine+DateTime.Now);
                //return null;
                //throw;
            }

            #endregion


            return returnPoliceler;
            // TUMUrunAdi = "KASKO", TUMUrunKodu = "525"
            // TUMUrunAdi = "TRAFİK", TUMUrunKodu = "555"
        }

        public RaySigorta.PoliceDetayOutputEntity PoliceDetayRequest(int PoliceNo, string UrunKodu, int ZeyilNo, int YenilemeNo)
        {
            RaySigorta.PoliceDetayInputEntity policeDetayInputEntity = new RaySigorta.PoliceDetayInputEntity();
            policeDetayInputEntity.AcenteNo = this._acenteNo;
            policeDetayInputEntity.PoliceNo = PoliceNo;
            policeDetayInputEntity.UrunKodu = UrunKodu;
            policeDetayInputEntity.ZeyilNo = ZeyilNo;
            policeDetayInputEntity.YenilemeNo = YenilemeNo;

            RaySigorta.ServisKullaniciBilgileriNesne servisKullaniciBilgileriNesne = new RaySigorta.ServisKullaniciBilgileriNesne();
            servisKullaniciBilgileriNesne.IpNo = "88.247.127.91";
            servisKullaniciBilgileriNesne.KullaniciAdi = this._KullaniciAdi; //"A5893001";
            servisKullaniciBilgileriNesne.Sifre = this._Sifre; //"Poyraz.1211";
            servisKullaniciBilgileriNesne.Ortam = RaySigorta.ConstsLokasyon.Canli;
            servisKullaniciBilgileriNesne.ReferansNo = "1234567";
            policeDetayInputEntity.ServisKullaniciBilgileri = servisKullaniciBilgileriNesne;

            //RaySigorta.TransferClient transferClient = new RaySigorta.TransferClient();
            //transferClient.Open();
            //var res = transferClient.PoliceDetayAsync(policeDetayInputEntity);
            //res.Wait();
            //RaySigorta.PoliceDetayOutputEntity sonuc = res.Result;
            //transferClient.Close();
            return null;
        }

        public class AxaPoliceListModel
        {
            public int PoliceNo { get; set; }
            public int ZeylSiraNo { get; set; }
        }
        public class AxaPoliceDetayListModel
        {

        }
        public NeoOnlineBransResult getNeoOnlineBranskodu(int axaBransKodu)
        {
            var NeoOnlineBranslar = _BransUrunService.getBranslar();
            NeoOnlineBransResult neoBrans = new NeoOnlineBransResult();
            switch (axaBransKodu)
            {
                case 1: neoBrans.BransKodu = BransKodlari.YillikHayat; break;
                case 2: neoBrans.BransKodu = BransKodlari.Saglik; break;
                case 3: neoBrans.BransKodu = BransKodlari.BES; break;
                case 4: neoBrans.BransKodu = BransKodlari.FerdiKaza; break;
                case 5: neoBrans.BransKodu = BransKodlari.Yangin; break;
                case 6: neoBrans.BransKodu = BransKodlari.Nakliyat; break;
                case 7: neoBrans.BransKodu = BransKodlari.Trafik; break;
                case 11: neoBrans.BransKodu = BransKodlari.Saglik; break;
                case 12: neoBrans.BransKodu = BransKodlari.YillikHayat; break;
                case 14: neoBrans.BransKodu = BransKodlari.Saglik; break;
                default: neoBrans.BransKodu = BransKodlari.TANIMSIZ; break;
            }
            if (NeoOnlineBranslar.Count > 0)
            {
                var neoBransDetay = NeoOnlineBranslar.Where(s => s.BransKodu == neoBrans.BransKodu).FirstOrDefault();
                if (neoBransDetay != null)
                {
                    neoBrans.BransAdi = neoBransDetay.BransAdi;
                }
            }
            return neoBrans;
        }
        public class NeoOnlineBransResult
        {
            public int BransKodu { get; set; }
            public string BransAdi { get; set; }
        }

        string tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas = null, PoliceGenel police = null)
        {
            foreach (var item in neoOnline_TahsilatKapatmas)
            {
                if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                {
                    return item.Kart_No.Trim();
                }
            }
            return "";
        }
    }

}

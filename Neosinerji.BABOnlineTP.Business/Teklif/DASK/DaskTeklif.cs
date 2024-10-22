using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;

namespace Neosinerji.BABOnlineTP.Business
{
    public class DaskTeklif : TeklifBase, IDaskTeklif
    {
        public DaskTeklif()
            : base()
        {

        }

        public DaskTeklif(int teklifId)
            : base(teklifId)
        {

        }

        public override IsDurum Hesapla(ITeklif teklif)
        {
            bool hdi = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.HDI) == 1;
            bool mapfre = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.MAPFRE) == 1;
            bool anadolu = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.ANADOLU) == 1;
            bool nippon = this.UretimMerkezleri.Count(c => c == TeklifUretimMerkezleri.TURKNIPPON) == 1;

            if (hdi)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    IHDIDask dask = DependencyResolver.Current.GetService<IHDIDask>();
                    dask.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(dask);
                }
            }
            if (nippon)
            {
                foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
                {
                    ITURKNIPPONDask dask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
                    dask.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
                    this.AddTeklif(dask);
                }
            }
            //if (mapfre)
            //{
            //    foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
            //    {
            //        IMAPFREDask dask = DependencyResolver.Current.GetService<IMAPFREDask>();
            //        dask.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
            //        this.AddTeklif(dask);
            //    }
            //}

            //if (anadolu)
            //{
            //    foreach (int odemePlaniAlternatifKodu in this.OdemePlaniKodlari)
            //    {
            //        IANADOLUDask dask = DependencyResolver.Current.GetService<IANADOLUDask>();
            //        dask.OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu;
            //        this.AddTeklif(dask);
            //    }
            //}

            return base.Hesapla(teklif);
        }

        public override void Policelestir(ITeklif teklif, Odeme odeme)
        {
            teklif.Policelestir(odeme);
        }

        public override void CreatePDF()
        {
            ITVMService tvm = base._TVMService;
            ITUMService tum = base._TUMService;
            IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            IKullaniciFotografStorage logoService = DependencyResolver.Current.GetService<IKullaniciFotografStorage>();

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.DASK_KARSILASTIRMA);

                pdf = new PDFHelper("Babonline", "DASK SİGORTASI KARŞILAŞTIRMA TABLOSU", "DASK SİGORTASI KARŞILAŞTIRMA TABLOSU", 8, _RootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());

                PDFParser parser = new PDFParser(template, pdf);

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string adUnvan = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                parser.SetVariable("$AdiSoyadiUnvani$", adUnvan);

                //TODO : TVM Logosu yoksa default bir icon koyulması
                TVMDetay tvmDetay = tvm.GetDetay(this.Teklif.GenelBilgiler.TVMKodu);
                //string tvmLogo = logoService.DownloadToFiles(tvmDetay.Logo);
                string tvmUnvani = tvmDetay.Unvani;

                #region Logo

                // Default Logo
                string tvmLogo = "https://neoonlineteststrg.blob.core.windows.net/kullanici-fotograf/neoonline-logo.jpg";
                if (!String.IsNullOrEmpty(tvmDetay.Logo))
                {
                    tvmLogo = tvmDetay.Logo;
                }

                #endregion

                parser.SetVariable("$TVMLogo$", tvmLogo);
                parser.SetVariable("$Tarih$", TurkeyDateTime.Today.ToString("dd.MM.yyyy"));
                parser.SetVariable("$TVMUnvani$", tvmUnvani);



                List<TUMDetay> tumler = new List<TUMDetay>();

                #endregion

                #region Fiyat Satırları
                string fiyatSatirTemplate = parser.GetTemplate("FiyatSatiri");

                var tumKodlari = this.TUMTeklifler.Where(w => w.GenelBilgiler.Basarili.Value)
                                                  .GroupBy(g => g.GenelBilgiler.TUMKodu)
                                                  .Select(s => new { TUMKodu = s.Key });
                foreach (var tumKodu in tumKodlari)
                {
                    TUMDetay tumDetay = tum.GetDetay(tumKodu.TUMKodu);
                    tumler.Add(tumDetay);

                    string fiyatSatir = String.Empty;

                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu)
                                                           .Select(f => f.GenelBilgiler).FirstOrDefault();

                    //string logo = logoService.DownloadToFiles(tumDetay.Logo);

                    fiyatSatir = fiyatSatirTemplate.Replace("$TUMLogo$", tumDetay.Logo)
                                                   .Replace("$TUMUnvani$", "DASK SİGORTASI");
                    if (teklif1 != null)
                    {
                        if (teklif1.TaksitSayisi.HasValue && teklif1.TaksitSayisi.Value > 1)
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL ({1} Taksit)", teklif1.BrutPrim, teklif1.TaksitSayisi));
                        else
                            fiyatSatir = fiyatSatir.Replace("$Fiyat1$", String.Format("{0:N2} TL", teklif1.BrutPrim));
                    }
                    else
                    {
                        fiyatSatir = fiyatSatir.Replace("$Fiyat1$", "-");
                    }

                    parser.AppendToPlaceHolder("fiyatSatirlari", fiyatSatir);
                }
                #endregion

                #region Adres Bilgiler

                TeklifRizikoAdresi adres = this.Teklif.RizikoAdresi;
                string Il = String.Empty;
                string Ilce = String.Empty;
                string Belde = String.Empty;
                string PostaKodu = adres.PostaKodu.HasValue ? adres.PostaKodu.Value.ToString() : "";
                string Adres = String.Empty;

                if (adres.IlKodu.HasValue)
                {
                    //HDIUAVTAdresResponse model = _HDIDask.GetUAVTAdres(adres.UAVTKodu);
                    //Neosinerji.BABOnlineTP.Business.HDI.HDIUAVTAdresResponse.KAYIT adreskayit = model.KAYITLAR.FirstOrDefault();

                    if (adres != null)
                    {
                        Il = adres.Il;
                        Ilce = adres.Ilce;
                        Belde = adres.SemtBelde;

                        Adres += adres.Mahalle;
                        Adres += adres.Cadde + adres.Sokak;
                        Adres += adres.Apartman;
                        Adres += adres.Bina + "/" + adres.Daire;
                    }
                }

                parser.SetVariable("$Il$", Il);
                parser.SetVariable("$Tarih$", this.Teklif.GenelBilgiler.TanzimTarihi.ToString("dd.MM.yyyy"));
                parser.SetVariable("$Ilce$", Ilce);
                parser.SetVariable("$TeklifNo$", this.Teklif.GenelBilgiler.TeklifNo.ToString());
                parser.SetVariable("$Belde$", Belde);
                parser.SetVariable("$PostaKodu$", PostaKodu);
                parser.SetVariable("$Adres$", Adres);

                #endregion

                #region Diger Bilgiler

                string yapiTarzi = this.Teklif.ReadSoru(DASKSorular.Yapi_Tarzi, String.Empty);
                string katSayisi = this.Teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                string binaInsaYili = this.Teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);

                parser.SetVariable("$YuzOlcumu$", this.Teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty));

                if (!String.IsNullOrEmpty(yapiTarzi))
                    parser.SetVariable("$YapiTarzi$", DaskYapiTarzlari.YapiTarzi(Convert.ToByte(yapiTarzi)));

                if (!String.IsNullOrEmpty(katSayisi))
                    parser.SetVariable("$KatSayisi$", DaskBinaKatSayilari.BinaKatSayisi(Convert.ToByte(katSayisi)));

                if (!String.IsNullOrEmpty(binaInsaYili))
                    parser.SetVariable("$InsaYili$", DaskBinaInsaYillari.BinaInsaYili(Convert.ToByte(binaInsaYili)));
                #endregion

                #region Karşılaştırma tablosu
                int tumCount = tumKodlari.Count();

                if (tumCount == 0)
                    return;

                int columnWidth = 350 / tumCount;
                int columnCount = tumCount + 1;
                int[] columns = new int[columnCount];
                columns[0] = 150;
                for (int i = 1; i < columnCount; i++)
                {
                    columns[i] = columnWidth;
                }
                parser.SetColumns("karsilastirmaBaslikColumns", columns);

                string karsilastirmaBaslikTemplate = parser.GetTemplate("karsilastirmaBaslik");

                foreach (var tumKodu in tumKodlari)
                {
                    TUMDetay detay = tumler.FirstOrDefault(f => f.Kodu == tumKodu.TUMKodu);
                    parser.AppendToPlaceHolder("karsilastirmaBaslik", karsilastirmaBaslikTemplate.Replace("$TUMUnvani$", detay.Unvani));
                }

                parser.SetColumns("karsilastirmaColumns", columns);

                string karsilastirma = parser.GetTemplate("karsilastirma");
                foreach (var tumKodu in tumKodlari)
                {
                    TeklifGenel teklif1 = this.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == tumKodu.TUMKodu)
                                                           .Select(f => f.GenelBilgiler).FirstOrDefault();

                    if (teklif1 == null)
                        return;

                    ITeklif teklif = this.TUMTeklifler.FirstOrDefault(f => f.GenelBilgiler.TeklifId == teklif1.TeklifId);

                    #region PDF Formati Değerlerini Alıyor

                    string karsilastirmaSon = "";
                    karsilastirmaSon = karsilastirma.Replace("$PesinPrim$", String.Format("{0:N2}", teklif1.BrutPrim));

                    TeklifTeminat daskTeminati = teklif1.TeklifTeminats.Where(s => s.TeminatKodu == DASKTeminatlar.DASK).FirstOrDefault();
                    if (daskTeminati != null)
                    {
                        if (daskTeminati.TeminatBedeli.HasValue)
                            karsilastirmaSon = karsilastirmaSon.Replace("$DaskTeminati$", String.Format("{0:N2}", daskTeminati.TeminatBedeli.Value));
                    }

                    #endregion

                    parser.SetColumnValues("karsilastirma", karsilastirmaSon);
                }
                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("dask_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("dask", fileName, fileData);

                this.Teklif.GenelBilgiler.PDFDosyasi = url;

                _TeklifService.UpdateGenelBilgiler(this.Teklif.GenelBilgiler);

                _LogService.Info("PDF dokumanı oluşturuldu : {0}", url);

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error("PDF Oluşturlamadı.");
                _LogService.Error(ex);
                throw;
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }

        public void CreatePolicePDF(ITeklif teklif)
        {
            ITVMService tvm = DependencyResolver.Current.GetService<ITVMService>();
            ITUMService tum = DependencyResolver.Current.GetService<ITUMService>();
            IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            IUlkeService ulkeService = DependencyResolver.Current.GetService<IUlkeService>();
            ITeklif daskTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            PDFHelper pdf = null;
            try
            {
                #region Template Hazırlama

                string template = PdfTemplates.GetTemplate(base._RootPath + "Content/templates/", PdfTemplates.DASK_POLICE);

                pdf = new PDFHelper("Babonline", "DASK SİGORTASI POLİÇESİ", "DASK SİGORTASI POLİÇESİ", 8, _RootPath + "Content/fonts/");
                PDFParser parser = new PDFParser(template, pdf);

                #endregion

                #region Data Fill

                #region Şirket Verileri
                string sigortaSirketi = String.Empty;
                string acenteAdi = String.Empty;
                string tel1 = String.Empty;
                string tel2 = String.Empty;

                TUMDetay tumDetay = tum.GetDetay(TeklifUretimMerkezleri.HDI);

                if (tumDetay != null)
                {
                    sigortaSirketi = tumDetay.Unvani;
                    tel1 = tumDetay.Telefon;
                }

                TVMDetay tvmDetay = tvm.GetDetay(teklif.GenelBilgiler.TVMKodu);
                if (tvmDetay != null)
                {
                    acenteAdi = tvmDetay.Unvani;
                    tel2 = tvmDetay.Telefon;
                }


                string policeNo = teklif.GenelBilgiler.TUMPoliceNo;
                string yenilemeEkBElgeNo = "0/0";
                string grupNo = "0";
                string tanzimTarihi = teklif.GenelBilgiler.TanzimTarihi.ToString("dd/MM/yyyy");
                string baslangicBitisTarihi = teklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy") + " - " + teklif.GenelBilgiler.BitisTarihi.ToString("dd/MM/yyyy");
                string indirimTipi = "";
                string indirimTipiVarYok = "";

                bool yururlukteDaskPolicesi = daskTeklif.ReadSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, false);
                if (yururlukteDaskPolicesi == true)
                {
                    indirimTipi = "Yenileme indirimi";
                    indirimTipiVarYok = "Poliçe primine %20 yenileme indirimi uygulanmıştır.";
                }

                parser.SetVariable("$PoliceNo$", policeNo);
                parser.SetVariable("$AcenteAdi$", acenteAdi);
                parser.SetVariable("$SigortaSirketi$", sigortaSirketi);
                parser.SetVariable("$Telefon$", tel1);
                parser.SetVariable("$Telefon2$", tel2);
                parser.SetVariable("$TanzimTarihi$", tanzimTarihi);
                parser.SetVariable("$YenilemeEkBelgeNo$", yenilemeEkBElgeNo);
                parser.SetVariable("$BaslangicBitisTarihi$", baslangicBitisTarihi);
                parser.SetVariable("$GrupNo$", grupNo);
                parser.SetVariable("$IndirimTipi$", indirimTipi);
                parser.SetVariable("$IndirimTipiVarmi$", indirimTipiVarYok);

                #endregion

                #region Sigortalı Bilgileri

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(this.Teklif.SigortaEttiren.MusteriKodu);
                string SAdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                string SUyruk = musteri.Uyruk == 0 ? "TC" : "Yabancı";
                string STCKN = musteri.KimlikNo;
                string SCepTelefonu = String.Empty;
                string SSabitTelefonu = String.Empty;

                MusteriTelefon SCep = musteri.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                MusteriTelefon SSabit = musteri.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev ||
                                                                          s.IletisimNumaraTipi == IletisimNumaraTipleri.Is).FirstOrDefault();

                if (SCep != null)
                    SCepTelefonu = SCep.Numara;

                if (SSabit != null)
                    SSabitTelefonu = SSabit.Numara;

                string SIletisimAdresi = string.Empty;


                MusteriAdre musteriAdres = musteri.MusteriAdres.FirstOrDefault();
                if (musteriAdres != null)
                {
                    if (!String.IsNullOrEmpty(musteriAdres.Mahalle))
                        SIletisimAdresi += musteriAdres.Mahalle + " MAH. ";
                    if (!String.IsNullOrEmpty(musteriAdres.Cadde))
                        SIletisimAdresi += musteriAdres.Cadde + " CAD. ";
                    if (!String.IsNullOrEmpty(musteriAdres.Apartman))
                        SIletisimAdresi += musteriAdres.Apartman + " APT. ";
                    if (!String.IsNullOrEmpty(musteriAdres.BinaNo))
                        SIletisimAdresi += " NO : " + musteriAdres.BinaNo;
                    if (!String.IsNullOrEmpty(musteriAdres.DaireNo))
                        SIletisimAdresi += " DAİRE : " + musteriAdres.DaireNo;


                    Il sigortaliIl = new Il();
                    Ilce sigortaliIlce = new Ilce();
                    sigortaliIl = ulkeService.GetIl(musteriAdres.UlkeKodu, musteriAdres.IlKodu);

                    if (musteriAdres.IlceKodu.HasValue)
                        sigortaliIlce = ulkeService.GetIlce(musteriAdres.IlceKodu.Value);

                    SIletisimAdresi += " " + sigortaliIlce.IlceAdi;
                    SIletisimAdresi += " " + sigortaliIl.IlAdi;
                }

                parser.SetVariable("$SAdiSoyadi$", SAdiSoyadi);
                parser.SetVariable("$SUyruk$", SUyruk);
                parser.SetVariable("$STCKN$", STCKN);
                parser.SetVariable("$SSabitTelefonu$", SSabitTelefonu);
                parser.SetVariable("$SCepTelefonu$", SCepTelefonu);
                parser.SetVariable("$SIletisimAdresi$", SIletisimAdresi);

                #endregion

                #region Sigorta Ettiren Bilgileri

                string SESifati = String.Empty;

                byte sifat = (byte)daskTeklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, 0);

                if (sifat > 0 & sifat < 8)
                {
                    SESifati = Dask_S_EttirenSifatlari.Sifati(sifat);
                }

                string SEAdiSoyadi = SAdiSoyadi;
                string SEUyruk = SUyruk;
                string SETCKN = STCKN;
                string SESabitTelefonu = SSabitTelefonu;
                string SECepTelefonu = SCepTelefonu;
                string SEEposta = musteri.EMail;

                parser.SetVariable("$SEAdiSoyadi$", SEAdiSoyadi);
                parser.SetVariable("$SESifati$", SESifati);
                parser.SetVariable("$SEUyruk$", SEUyruk);
                parser.SetVariable("$SETCKN$", SETCKN);
                parser.SetVariable("$SESabitTelefonu$", SESabitTelefonu);
                parser.SetVariable("$SECepTelefonu$", SECepTelefonu);
                parser.SetVariable("$SEEposta$", SEEposta);

                #endregion

                #region Sigortalı Yere İlişkin Bilgileri

                string IlIlceBelde = String.Empty;
                string Adres = String.Empty;
                string AdaPafta = daskTeklif.ReadSoru(DASKSorular.Riziko_Ada, String.Empty) + " / " +
                 daskTeklif.ReadSoru(DASKSorular.Riziko_Pafta_No, String.Empty);
                string ParselSayfa = daskTeklif.ReadSoru(DASKSorular.Riziko_Parsel, String.Empty) + " / " +
                 daskTeklif.ReadSoru(DASKSorular.Riziko_Sayfa_No, String.Empty);
                string BinaInsaTarzi = String.Empty;
                string BinaInsaYili = String.Empty;
                string ToplamKatSayisi = String.Empty;
                string DaireKullanimSekli = String.Empty;
                string HasarDurumu = String.Empty;
                string EvrakTarihSayi = String.Empty;
                string DaireYuzolcumu = daskTeklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                string TarifeFiyati = "0,001550";


                string SigortaBedeli = String.Empty;

                TeklifTeminat sigortaBedeli = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == DASKTeminatlar.DASK);
                if (sigortaBedeli != null)
                    SigortaBedeli = sigortaBedeli.TeminatBedeli.HasValue ? sigortaBedeli.TeminatBedeli.Value.ToString() + " TL" : "";


                string PolicePrimi = teklif.GenelBilgiler.BrutPrim.ToString() + " TL";
                string OtoYeniSuresi = "0";

                string rizikoIl = String.Empty;
                string rizikoIlce = String.Empty;
                string rizikoBelde = String.Empty;

                TeklifRizikoAdresi adres = daskTeklif.RizikoAdresi;
                if (adres != null)
                {
                    HDIUAVTAdresResponse model = _HDIDask.GetUAVTAdres(adres.UAVTKodu);
                    Neosinerji.BABOnlineTP.Business.HDI.HDIUAVTAdresResponse.KAYIT adreskayit = model.KAYITLAR.FirstOrDefault();

                    if (adreskayit != null)
                    {
                        rizikoIl = adreskayit.IlAd;
                        rizikoIlce = adreskayit.IlceAd;
                        rizikoBelde = adreskayit.BeldeAd;

                        IlIlceBelde = rizikoIl + " / " + rizikoIlce + " / " + rizikoBelde;

                        Adres += adreskayit.Mahalle;
                        Adres += adreskayit.CSBMAd;
                        Adres += adreskayit.BinaAd;
                        Adres += adreskayit.BinaNo + "/" + adreskayit.DaireNo;
                    }
                }

                string yapiTarzi = daskTeklif.ReadSoru(DASKSorular.Yapi_Tarzi, String.Empty);
                string katSayisi = daskTeklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                string binaInsaYili = daskTeklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
                string dairekullanimsekli = daskTeklif.ReadSoru(DASKSorular.Daire_KullanimSekli, String.Empty);
                string hasarDurumu = daskTeklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);


                if (!String.IsNullOrEmpty(yapiTarzi))
                    BinaInsaTarzi = DaskYapiTarzlari.YapiTarzi(Convert.ToByte(yapiTarzi));

                if (!String.IsNullOrEmpty(katSayisi))
                    ToplamKatSayisi = DaskBinaKatSayilari.BinaKatSayisi(Convert.ToByte(katSayisi));

                if (!String.IsNullOrEmpty(binaInsaYili))
                    BinaInsaYili = DaskBinaInsaYillari.BinaInsaYili(Convert.ToByte(binaInsaYili));

                if (!String.IsNullOrEmpty(yapiTarzi))
                    DaireKullanimSekli = DaskBinaKullanimSeklilleri.KullanimSekli(Convert.ToByte(dairekullanimsekli));

                if (!String.IsNullOrEmpty(yapiTarzi))
                    HasarDurumu = DaskBinaHasarDurumlari.HasarDurumu(Convert.ToByte(hasarDurumu));

                parser.SetVariable("$IlIlceBelde$", IlIlceBelde);
                parser.SetVariable("$Adres$", Adres);
                parser.SetVariable("$AdaPafta$", AdaPafta);
                parser.SetVariable("$ParselSayfa$", ParselSayfa);
                parser.SetVariable("$BinaInsaTarzi$", BinaInsaTarzi);
                parser.SetVariable("$BinaInsaYili$", BinaInsaYili);
                parser.SetVariable("$ToplamKatSayisi$", ToplamKatSayisi);
                parser.SetVariable("$DaireKullanimSekli$", DaireKullanimSekli);
                parser.SetVariable("$HasarDurumu$", HasarDurumu);
                parser.SetVariable("$EvrakTarihSayi$", EvrakTarihSayi);
                parser.SetVariable("$DaireYuzolcumu$", DaireYuzolcumu);
                parser.SetVariable("$TarifeFiyati$", TarifeFiyati);
                parser.SetVariable("$SigortaBedeli$", SigortaBedeli);
                parser.SetVariable("$PolicePrimi$", PolicePrimi);
                parser.SetVariable("$OtoYeniSuresi$", OtoYeniSuresi);

                #endregion

                #endregion

                #region Kayıt Log

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("dask_Police{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile("dask", fileName, fileData);

                teklif.GenelBilgiler.PDFPolice = url;

                _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);

                _LogService.Info("PDF dokumanı oluşturuldu : {0}", url);

                #endregion

            }
            catch (Exception ex)
            {
                _LogService.Error("PDF Oluşturlamadı.");
                _LogService.Error(ex);
                throw;
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }

        public override void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
            IEMailService email = DependencyResolver.Current.GetService<IEMailService>();
            bool pdfDosyasiVar = !String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFDosyasi);

            if (!pdfDosyasiVar)
            {
                try
                {
                    if (this.Teklif.GenelBilgiler.TUMKodu == 0)
                    {
                        this.CreatePDF();
                        pdfDosyasiVar = true;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(this.Teklif.GenelBilgiler.PDFPolice))
                            pdfDosyasiVar = true;
                    }
                }
                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
            }
            if (pdfDosyasiVar)
                email.SendDaskTeklif(this.Teklif, DigerAdSoyad, DigerEmail);
        }


    }
}

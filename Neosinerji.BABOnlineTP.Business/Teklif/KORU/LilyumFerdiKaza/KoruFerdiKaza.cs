using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Common.KORU;
using Neosinerji.BABOnlineTP.Business.KORU.Messages;
using Neosinerji.BABOnlineTP.Business.Paratika3DCode;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Neosinerji.BABOnlineTP.Business.Common.KORU.KORUCommon;

namespace Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza
{
    public class KoruFerdiKaza : Teklif, IKoruFerdiKaza
    {
        ITVMContext _TVMContext;
        ITeklifContext _TeklifContext;
        ITVMService _TVMService;
        ILogService _Log;
        IMusteriService _MusteriService;
        IMusteriContext _MusteriContext;
        ICRContext _CRContext;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifKullaniciService;
        IKonfigurasyonService _KonfigurasyonService;
        ICRService _CRService;
        IUlkeService _UlkeService;

        [InjectionConstructor]
        public KoruFerdiKaza(ITVMContext tvmContext, ITeklifContext teklifContext, ITVMService TVMService, ILogService log, IMusteriService musteriService, ICRContext crContext, ITeklifService teklifService,
            IAktifKullaniciService aktifKullaniciService, IUlkeService ulkeService, IKonfigurasyonService konfigurasyonService, ICRService cRService, IMusteriContext musteriContext)
            : base()
        {
            _TVMContext = tvmContext;
            _TVMService = TVMService;
            _Log = log;
            _MusteriService = musteriService;
            _CRContext = crContext;
            _TeklifService = teklifService;
            _UlkeService = ulkeService;
            _AktifKullaniciService = aktifKullaniciService;
            _KonfigurasyonService = konfigurasyonService;
            _CRService = cRService;
            _TeklifContext = teklifContext;
            _MusteriContext = musteriContext;
        }
        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.KORU;
            }
        }
        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Veri Hazırlama

                koruLilyumferdiKaza.AcentePoliceServisleri client = new koruLilyumferdiKaza.AcentePoliceServisleri();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleKoruLilyumFerdiKaza);
                client.Timeout = 150000;
                client.Url = konfig[Konfig.KoruLilyum_ServiceURL];
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.KORU });
                client.Credentials = new NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                MusteriAdre SEttirenadress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
                KoruMusteriModel koruMusteriModel = new KoruMusteriModel();
                string sigortaliEmail = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumEmail, "");
                string sigortaliCepTel = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumTelefon, "");
                string sigortaliTeslimatAdres = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumTeslimatAdres, "");
                string sigortaliIletisimAdres = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumIletisimAdres, "");
                string sigortaliIletisimIlKodu = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu, "");
                string sigortaliIletisimIlceKodu = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu, "");
                string sigortaliTeslimatIlKodu = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumTeslimatIlKodu, "");
                string sigortaliTeslimatIlceKodu = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumTeslimatIlceKodu, "");
                string sigortaliKimlikNo = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliTCKimlikNo, "");

                string sigortaliAdi = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliAdi, "");
                string sigortaliSoyadi = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliSoyadi, "");
                string sigortaliDogumTarihi = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliDogumTarihi, "");
                string sigortaliMeslek = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliMeslek, "");

                string brutPrim = "";
                string digerOdemeTutari = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumDigerOdemeTutari, "");
                if (!String.IsNullOrEmpty(digerOdemeTutari))
                {
                    brutPrim = digerOdemeTutari.Replace(",",".");
                }
                else
                {
                    if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Pesin)
                    {
                        brutPrim = "446.04";
                    }
                    else if (teklif.GenelBilgiler.OdemeSekli == 2)
                    {
                        brutPrim = "495.60";
                    }
                }
                teklif.GenelBilgiler.BrutPrim = Util.ToDecimal(brutPrim);
                #region Koru Müşteri Model Dolduruluyor
                koruMusteriModel.Email = sigortaliEmail;
                koruMusteriModel.CepTelefonu = sigortaliCepTel;
                koruMusteriModel.TeslimatAcikAdres = sigortaliTeslimatAdres;
                koruMusteriModel.TeslimatIlKodu = sigortaliTeslimatIlKodu;
                koruMusteriModel.TeslimatIlceKodu = Convert.ToInt32(sigortaliTeslimatIlceKodu);
                koruMusteriModel.IletisimAcikAdres = sigortaliIletisimAdres;
                koruMusteriModel.IletisimIlKodu = sigortaliIletisimIlKodu;
                koruMusteriModel.IletisimIlceKodu = Convert.ToInt32(sigortaliIletisimIlceKodu);
                koruMusteriModel.tvmKodu = tvmKodu;
                koruMusteriModel.TcVkn = sigortaliKimlikNo;
                koruMusteriModel.AdiUnvan = sigortaliAdi;
                koruMusteriModel.SoyadiUnvan = sigortaliSoyadi;
                if (!String.IsNullOrEmpty(sigortaliDogumTarihi))
                {
                    koruMusteriModel.DogumTarihi = Convert.ToDateTime(sigortaliDogumTarihi);
                }
                #endregion

                #endregion

                #region Service call
                var sorular = this.TeklifSorular(teklif, client, servisKullanici);
                
                BeginLog(sorular, sorular.GetType(), WebServisIstekTipleri.Teklif);

                //alttaki hata varsa kodu açıp çalıştır.
                //"kimlik numarası için adres sorgusu başarısız oldu. sigortalibeldekodu alan değerini kendiniz göndererek teklif alabilirsiniz."

                //koruLilyumferdiKaza.SoruCevap soru = new koruLilyumferdiKaza.SoruCevap();
                //soru.Soru = "SigortaliBeldeKodu";
                //soru.Cevap = "34";
                //soru.Zorunlu = false;
                //sorular.Add(soru);

                var urunHesapla = client.TekUrunHesapla(sorular.ToArray());

                string Hatalar = "";
                for (int i = 0; i < urunHesapla.Count(); i++)
                {
                    if (urunHesapla[i].KuralHatalari.Count() > 0)
                    {
                        var kurallar = urunHesapla[i].KuralHatalari;
                        for (int j = 0; j < kurallar.Count(); j++)
                        {
                            Hatalar += kurallar[j];
                        }
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(Hatalar))
                {
                    this.AddHata(Hatalar);
                    this.EndLog(urunHesapla, false, urunHesapla.GetType());
                }
                else
                {
                    this.EndLog(urunHesapla, true, urunHesapla.GetType());
                }
                #region sli/settiren

                var iletisimIlAdi = string.Empty;
                iletisimIlAdi = _UlkeService.GetIlAdi("TUR", koruMusteriModel.IletisimIlKodu);

                #endregion
                #endregion

                #region TeklifKaydet

                #region Basarı Kontrol

                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }

                #endregion

                #region Teklif kaydı

                #region Genel bilgiler

                this.Import(teklif);
                foreach (var item in urunHesapla)
                {
                    foreach (var itemSli in item.Sorular)
                    {
                        sigortaEttiren.MusteriGenelBilgiler.TVMKodu = tvmKodu;
                        if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliAdi)
                        {
                            koruMusteriModel.AdiUnvan = itemSli.Cevap;
                        }
                        else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliSoyadi)
                        {
                            koruMusteriModel.SoyadiUnvan = itemSli.Cevap;
                        }
                        else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliTcKimlikNo || itemSli.Soru == PoliKayitTeklifSorular.MusteriVergiNo)
                        {
                            koruMusteriModel.TcVkn = itemSli.Cevap;
                        }
                        //else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliSbmIlKodu)
                        //{
                        //    Sliadress.IlKodu = itemSli.Cevap;
                        //}
                        //else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliSbmIlceKodu)
                        //{
                        //    Sliadress.IlceKodu = Convert.ToInt32(itemSli.Cevap);
                        //}
                        //else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliSbmIlAdi)
                        //{
                        //    ilAdi = itemSli.Cevap;
                        //}
                        //else if (itemSli.Soru == PoliKayitTeklifSorular.SigortaliSbmIlceAdi)
                        //{
                        //    ilceAdi = itemSli.Cevap;
                        //}
                    }
                }
                #region Vergiler
                decimal BSV = 0;
                decimal GH = 0;
                decimal THG = 0;

                foreach (var itemVergiler in urunHesapla)
                {
                    foreach (var itemvergi in itemVergiler.HesaplamaSonuclari)
                    {
                        BSV = Convert.ToDecimal(itemvergi.GiderVergi);
                        GH = Convert.ToDecimal(itemvergi.GarantiFonu);
                        THG = Convert.ToDecimal(itemvergi.THGF);
                    }
                }
                this.AddVergi(TrafikVergiler.THGFonu, THG);
                this.AddVergi(TrafikVergiler.GiderVergisi, BSV);
                this.AddVergi(TrafikVergiler.GarantiFonu, GH);

                this.GenelBilgiler.ToplamVergi = BSV + GH + THG;
                #endregion

                #region Teminatlar

                foreach (var item in urunHesapla)
                {
                    foreach (var itemTeminatlar in item.Teminatlar)
                    {
                        if (itemTeminatlar.Ad == LilyumKoruFerdiKazaTeminatlar.FERDIKAZASUREKLISAKATLIK)
                        {
                            this.AddTeminat(LilyumTeminatKodlari.FERDIKAZASUREKLISAKATLIK, Convert.ToDecimal(itemTeminatlar.Bedel), 0, Convert.ToDecimal(itemTeminatlar.Prim), 0, 0);
                        }
                        else if (itemTeminatlar.Ad == LilyumKoruFerdiKazaTeminatlar.FERDIKAZAVEFAT)
                        {
                            this.AddTeminat(LilyumTeminatKodlari.FERDIKAZAVEFAT, Convert.ToDecimal(itemTeminatlar.Bedel), 0, Convert.ToDecimal(itemTeminatlar.Prim), 0, 0);
                        }
                        else if (itemTeminatlar.Ad == LilyumKoruFerdiKazaTeminatlar.KOMBIKLIMAKURUTEMIZLEMEVEHALIYIKAMAHIZMETLERI)
                        {
                            this.AddTeminat(LilyumTeminatKodlari.KOMBIKLIMAKURUTEMIZLEMEVEHALIYIKAMAHIZMETLERI, Convert.ToDecimal(itemTeminatlar.Bedel), 0, Convert.ToDecimal(itemTeminatlar.Prim), 0, 0);
                        }
                        else if (itemTeminatlar.Ad == LilyumKoruFerdiKazaTeminatlar.KORUYARDIM)
                        {
                            this.AddTeminat(LilyumTeminatKodlari.KORUYARDIM, Convert.ToDecimal(itemTeminatlar.Bedel), 0, Convert.ToDecimal(itemTeminatlar.Prim), 0, 0);
                        }
                        else if (itemTeminatlar.Ad == LilyumKoruFerdiKazaTeminatlar.SAGLIKASISTANS)
                        {
                            this.AddTeminat(LilyumTeminatKodlari.SAGLIKASISTANS, Convert.ToDecimal(itemTeminatlar.Bedel), 0, Convert.ToDecimal(itemTeminatlar.Prim), 0, 0);
                        }
                    }
                }

                #endregion

                int SigortaliMusteriKodu = MusteriKaydet(koruMusteriModel);

                if (SigortaliMusteriKodu != 0)
                {
                    TeklifSigortali TSigortali = new TeklifSigortali();
                    TSigortali.MusteriKodu = SigortaliMusteriKodu;
                    TSigortali.SiraNo = 1;

                    this.Sigortalilar.Add(TSigortali);

                    TSigortali = new TeklifSigortali();
                    TSigortali.MusteriKodu = SigortaliMusteriKodu;
                    TSigortali.SiraNo = 1;
                    TSigortali.TeklifId = teklif.GenelBilgiler.TeklifId;
                    _TeklifService.AddTeklifSigortali(TSigortali);
                }
                #region 3d paratika ya veri gönder

                ParatikaClient sessionToken = new ParatikaClient();
                string sigAdi = koruMusteriModel.AdiUnvan + " " + koruMusteriModel.SoyadiUnvan;
                sigortaliCepTel = "+90" + koruMusteriModel.CepTelefonu;
                string tokenGuidId = Guid.NewGuid().ToString();
                
                var teklifId = teklif.GenelBilgiler.TeklifId + 1;
                string returnNeoUrl = "https://app.neoonline.com.tr/Teklif/LilyumKart/Police?id=" + teklifId + "&odemeTutari=" + brutPrim;
                sessionToken.SETRequestParametre(brutPrim, iletisimIlAdi, sigortaliCepTel, sigortaliEmail, sigAdi, sigortaliTeslimatAdres, tokenGuidId, returnNeoUrl);
                BeginLog(sessionToken, sessionToken.GetType(), WebServisIstekTipleri.LilyumParaticaToken);
                string HataMesaji = "";
                JObject token = sessionToken.sessionToken(out HataMesaji);

                RequestParatikaJsonModel model = JsonConvert.DeserializeObject<RequestParatikaJsonModel>(token.ToString());
                if (model != null)
                {
                    if (String.IsNullOrEmpty(model.sessionToken))
                    {
                        if (!String.IsNullOrEmpty(HataMesaji))
                        {
                            this.AddHata(HataMesaji);
                        }
                        else
                        {
                            this.AddHata(model.responseCode + model.errorMsg);
                            this.EndLog(model, false, model.GetType());
                        }
                    }
                    else
                    {
                        this.EndLog(model, true, model.GetType());
                        this.AddWebServisCevap(Common.WebServisCevaplar.Koru3DParatikaToken, model.sessionToken);
                        this.AddWebServisCevap(Common.WebServisCevaplar.KoruTokenGuidId, tokenGuidId);
                        this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                        this.GenelBilgiler.KayitTarihi = TurkeyDateTime.Now;
                    }
                }
                else
                {
                    this.AddHata(HataMesaji);
                }
                #endregion
                #endregion
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }

        }

        public KoruPoliceResponseModel KoruPolicelestir(ITeklif teklif, int koruTeklifId)
        {
            KoruPoliceResponseModel model = new KoruPoliceResponseModel();

            try
            {
                koruLilyumferdiKaza.AcentePoliceServisleri client = new koruLilyumferdiKaza.AcentePoliceServisleri();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleKoruLilyumFerdiKaza);
                client.Timeout = 150000;
                client.Url = konfig[Konfig.KoruLilyum_ServiceURL];
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.KORU });
                client.Credentials = new NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var koruTeklifDetay = _TeklifService.GetTeklif(koruTeklifId);
                string sigortaliEmail = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumEmail, "");
                string sigortaliCepTel = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruLilyumTelefon, "");

                string hashCevap = "";
                var sorular = this.TeklifSorular(teklif, client, servisKullanici);
                BeginLog(sorular, sorular.GetType(), WebServisIstekTipleri.Teklif);

                var urunHesapla = client.TekUrunHesapla(sorular.ToArray());

                string Hatalar = "";
                for (int i = 0; i < urunHesapla.Count(); i++)
                {
                    if (urunHesapla[i].KuralHatalari.Count() > 0)
                    {
                        var kurallar = urunHesapla[i].KuralHatalari;
                        for (int j = 0; j < kurallar.Count(); j++)
                        {
                            Hatalar += urunHesapla[i].KuralHatalari[j];
                        }
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(Hatalar))
                {
                    koruTeklifDetay.AddHata(Hatalar);
                    koruTeklifDetay.EndLog(urunHesapla, false, urunHesapla.GetType());
                }
                else
                {
                    koruTeklifDetay.EndLog(urunHesapla, true, urunHesapla.GetType());
                }

                #region sorulara soru eklendi
                for (int i = 0; i < urunHesapla.Count(); i++)
                {
                    if (urunHesapla[i].Sorular != null)
                    {
                        for (int j = 0; j < urunHesapla[i].Sorular.Count(); j++)
                        {
                            switch (urunHesapla[i].Sorular[j].Soru)
                            {
                                case YardimTekUrunHesaplaDonenSoruCevaplar.TeklifHash:
                                    hashCevap = urunHesapla[i].Sorular[j].Cevap; break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                koruLilyumferdiKaza.SoruCevap soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.TeklifHash;
                soru.Cevap = hashCevap;
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.SigortaliEPosta;
                soru.Cevap = "gulru@lilyumkart.com";
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.SigortaliCepTelefonNo;
                soru.Cevap = "5357450205";
                soru.Zorunlu = false;
                sorular.Add(soru);

                //TEKRAR AÇILACAK
                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KayitTuru;
                soru.Cevap = "P";
                soru.Zorunlu = false;
                sorular.Add(soru);

                #region Kart Bilgileri
                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KrediKartiNo;
                //TEST İÇİN DEĞİŞMELI MUTLAKA
                soru.Cevap = "5521010195360126"; //gerçek 5521010195360126
                                                 // soru.Cevap = "0000000195360126";

                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KrediKartiUstundekiIsim;
                soru.Cevap = "SAFİYE GÜLRU ARDAN";
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KrediKartiGuvenlikNo;
                soru.Cevap = "017";
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KrediKartiSonKullanmaAy;
                soru.Cevap = "01";
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.KrediKartiSonKullanmaYil;
                soru.Cevap = "2021";
                soru.Zorunlu = false;
                sorular.Add(soru);
                

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.MusteriCepTelefonNo;
                soru.Cevap = "5424768224";
                soru.Zorunlu = false;
                sorular.Add(soru);

                soru = new koruLilyumferdiKaza.SoruCevap();
                soru.Soru = PoliKayitTeklifSorular.MusteriEPosta;
                soru.Cevap = "musteri@lilyumkart.com";
                soru.Zorunlu = false;
                sorular.Add(soru);


                #endregion

                #endregion
                koruTeklifDetay.BeginLog(sorular, sorular.GetType(), WebServisIstekTipleri.Police);
                var policeResponse = client.PoliceKayit(sorular.ToArray());
                Hatalar = "";
                for (int i = 0; i < policeResponse.Count(); i++)
                {
                    if (policeResponse[i].KuralHatalari != null)
                    {
                        var kurallar = policeResponse[i].KuralHatalari;
                        for (int j = 0; j < kurallar.Count(); j++)
                        {
                            Hatalar += policeResponse[i].KuralHatalari[j];
                        }
                        break;
                    }
                }
                if (!String.IsNullOrEmpty(Hatalar))
                {
                    koruTeklifDetay.AddHata(Hatalar);
                    koruTeklifDetay.EndLog(policeResponse, false, policeResponse.GetType());
                    model.HataMesaji = Hatalar;
                    model.Basarili = false;
                    IsDurumDetay durumDetay = _TeklifService.GetIsDurumDetay(koruTeklifDetay.GenelBilgiler.TeklifId);
                    if (durumDetay != null)
                    {
                        durumDetay.HataMesaji = Hatalar;
                        _TeklifContext.IsDurumDetayRepository.Update(durumDetay);
                        _TeklifContext.Commit();
                    }
                }
                else
                {
                    koruTeklifDetay.EndLog(policeResponse, true, policeResponse.GetType());

                    foreach (var item in policeResponse)
                    {
                        foreach (var itemCevaplar in item.Sorular)
                        {
                            koruTeklifDetay.GenelBilgiler.Basarili = true;
                            if (itemCevaplar.Soru == PoliKayitTeklifSorular.PoliceBasimiPDF)
                            {
                                koruTeklifDetay.GenelBilgiler.PDFDosyasi = itemCevaplar.Cevap;
                            }
                            else if (itemCevaplar.Soru == PoliKayitTeklifSorular.PoliceMakbuzPDF)
                            {
                                koruTeklifDetay.GenelBilgiler.PDFDekont = itemCevaplar.Cevap;
                            }
                            else if (itemCevaplar.Soru == PoliKayitTeklifSorular.PoliceNo)
                            {
                                koruTeklifDetay.GenelBilgiler.TUMPoliceNo = itemCevaplar.Cevap;
                            }
                        }
                    }
                    koruTeklifDetay.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                    koruTeklifDetay.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                    koruTeklifDetay.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;

                    string digerOdemeTutari = teklif.ReadSoru(LilyumFerdiKazaSorular.KorulilyumDigerOdemeTutari, "");
                    if (!String.IsNullOrEmpty(digerOdemeTutari))
                    {
                        koruTeklifDetay.GenelBilgiler.BrutPrim = Convert.ToDecimal(digerOdemeTutari);
                    }
                    else
                    {
                        if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Pesin)
                        {
                            float brut = 446.04f;
                            float net = 378f;
                            koruTeklifDetay.GenelBilgiler.BrutPrim = Convert.ToDecimal(brut);
                            koruTeklifDetay.GenelBilgiler.NetPrim = Convert.ToDecimal(net);
                        }
                        else if (teklif.GenelBilgiler.OdemeSekli == 2)
                        {
                            float brut = 495.60f;
                            float net = 420f;
                            koruTeklifDetay.GenelBilgiler.BrutPrim = Convert.ToDecimal(brut);
                            koruTeklifDetay.GenelBilgiler.NetPrim = Convert.ToDecimal(net);
                        }
                    }
                    _TeklifService.UpdateGenelBilgiler(koruTeklifDetay.GenelBilgiler);
                    LilyumTeminatKaydetModel kaydetModel = new LilyumTeminatKaydetModel();
                    kaydetModel.TvmKodu = teklif.GenelBilgiler.TVMKodu;
                    kaydetModel.KullaniciKodu = teklif.GenelBilgiler.TVMKullaniciKodu;
                    kaydetModel.ReferansNo = koruTeklifDetay.GenelBilgiler.TUMPoliceNo;
                    kaydetModel.KaydedenKullaniciKodu = teklif.GenelBilgiler.KaydiEKleyenTVMKullaniciKodu.HasValue ? teklif.GenelBilgiler.KaydiEKleyenTVMKullaniciKodu.Value : _AktifKullaniciService.KullaniciKodu;
                    string hataMesaji = "";
                    bool basarili = false;
                    _TeklifService.LilyumKartTeminatKullanimCreate(kaydetModel, ref basarili, ref hataMesaji);
                    model.Basarili = true;
                    if (!basarili)
                    {
                        _Log.Error("KoruLilyumTeminatDetayEkle", hataMesaji);
                    }
                }
            }
            catch (Exception ex)
            {
                model.HataMesaji = ex.Message;
                model.Basarili = false;
            }
            return model;

        }

        private List<koruLilyumferdiKaza.SoruCevap> TeklifSorular(ITeklif teklif, koruLilyumferdiKaza.AcentePoliceServisleri clntSoruCevap, TVMWebServisKullanicilari serviskullanici)
        {
            #region SORU

            List<koruLilyumferdiKaza.SoruCevap> soruListesi = new List<koruLilyumferdiKaza.SoruCevap>();
            koruLilyumferdiKaza.SoruCevap soru = new koruLilyumferdiKaza.SoruCevap();

            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.KORU });

            string urunKodu = FerdiKaza;
            string acenteno = serviskullanici.PartajNo_;
            string urunGrubu = "ferdiKaza";

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.UrunKodu;
            soru.Cevap = urunKodu;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.UrunGrubu;
            soru.Cevap = urunGrubu;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.KullaniciAdi;
            soru.Cevap = serviskullanici.KullaniciAdi;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.Parola;
            soru.Cevap = serviskullanici.Sifre;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.MusteriTcKimlikNo;
            soru.Cevap = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            string sigortaliKimlikNo = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliTCKimlikNo, "");
            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.SigortaliTcKimlikNo;
            soru.Cevap = sigortaliKimlikNo;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.AcenteNo;
            soru.Cevap = acenteno;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.SporlaUgrasiyor;
            soru.Cevap = "H";
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.MeslekSinifi;
            soru.Cevap = "İşadamı";
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            //soru = new koruLilyumferdiKaza.SoruCevap();
            //soru.Soru = "BeldeKodu";
            //soru.Cevap = "826";
            //soru.Zorunlu = false;
            //soruListesi.Add(soru);

            //bool motorsikletkullaniyorMu = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruMotorsikletKullaniyorMu, false);
            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = YardimTekUrunHesaplaSorular.MotosikletKullaniyor;
            soru.Cevap = "H";
            //if (motorsikletkullaniyorMu)
            //{
            //    soru.Cevap = "E";
            //}
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            if (teklif.Arac != null)
            {
                if (!String.IsNullOrEmpty(teklif.Arac.PlakaKodu))
                {
                    soru = new koruLilyumferdiKaza.SoruCevap();
                    soru.Soru = YardimTekUrunHesaplaSorular._AracVarmi;
                    soru.Cevap = "1";
                    soru.Zorunlu = false;
                    soruListesi.Add(soru);

                    soru = new koruLilyumferdiKaza.SoruCevap();
                    soru.Soru = YardimTekUrunHesaplaSorular._AracPlaka;
                    soru.Cevap = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                    soru.Zorunlu = false;
                    soruListesi.Add(soru);
                }
            }

            #endregion

            return soruListesi;
        }

        private List<koruLilyumferdiKaza.SoruCevap> TeklifKayitSorular(ITeklif teklif, koruLilyumferdiKaza.AcentePoliceServisleri clntSoruCevap, TVMWebServisKullanicilari serviskullanici)
        {
            #region SORU
            MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            List<koruLilyumferdiKaza.SoruCevap> soruListesi = new List<koruLilyumferdiKaza.SoruCevap>();
            koruLilyumferdiKaza.SoruCevap soru = new koruLilyumferdiKaza.SoruCevap();

            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.KORU });

            koruLilyumferdiKaza.AcentePoliceServisleri client = new koruLilyumferdiKaza.AcentePoliceServisleri();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleKoruLilyumFerdiKaza);
            client.Timeout = 150000;
            client.Url = konfig[Konfig.KoruLilyum_ServiceURL];
            client.Credentials = new NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);

            var sorular = this.TeklifSorular(teklif, client, servisKullanici);
            var urunHesapla = client.TekUrunHesapla(sorular.ToArray());


            string urunKodu = FerdiKaza;
            string acenteno = serviskullanici.PartajNo_;
            string urunGrubu = "ferdiKaza";

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.UrunKodu;
            soru.Cevap = urunKodu;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.UrunGrubu;
            soru.Cevap = urunGrubu;
            soru.Zorunlu = false;
            soruListesi.Add(soru);



            //soru = new koruLilyumferdiKaza.SoruCevap();
            //soru.Soru = "BeldeKodu";
            //soru.Cevap = "826";
            //soru.Zorunlu = false;
            //soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.KullaniciAdi;
            soru.Cevap = serviskullanici.KullaniciAdi;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.Parola;
            soru.Cevap = serviskullanici.Sifre;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.MusteriTcKimlikNo;
            soru.Cevap = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            string sigortaliKimlikNo = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSigortaliTCKimlikNo, "");
            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.SigortaliTcKimlikNo;
            soru.Cevap = sigortaliKimlikNo;
            soru.Zorunlu = false;
            soruListesi.Add(soru);



            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.AcenteNo;
            soru.Cevap = acenteno;
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            bool sporlaUgrasiyorMu = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruSporlaUgrasiyorMu, false);
            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.SporlaUgrasiyor;
            soru.Cevap = "H";
            if (sporlaUgrasiyorMu)
            {
                soru.Cevap = "E";
            }
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.MeslekSinifi;
            soru.Cevap = "İşadamı";
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            bool motorsikletkullaniyorMu = teklif.ReadSoru(LilyumFerdiKazaSorular.KoruMotorsikletKullaniyorMu, false);
            soru = new koruLilyumferdiKaza.SoruCevap();
            soru.Soru = PoliKayitTeklifSorular.MotosikletKullaniyor;
            soru.Cevap = "H";
            if (motorsikletkullaniyorMu)
            {
                soru.Cevap = "E";
            }
            soru.Zorunlu = false;
            soruListesi.Add(soru);

            #endregion

            return soruListesi;
        }

        public override void DekontPDF()
        {
            //koruLilyumferdiKaza.BasimHazirlaCompletedEventHandler request = new koruLilyumferdiKaza.BasimHazirlaCompletedEventHandler();
            try
            {
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                koruLilyumferdiKaza.AcentePoliceServisleri client = new koruLilyumferdiKaza.AcentePoliceServisleri();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleKoruLilyumFerdiKaza);
                client.Timeout = 150000;
                client.Url = konfig[Konfig.KoruLilyum_ServiceURL];
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.KORU });
                client.Credentials = new NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string ekNo = string.Empty;
                string yenilemeNo = string.Empty;
                string urunKodu = "240";
                var pdfBasimi = client.BasimHazirla(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.PartajNo_, urunKodu, teklif.GenelBilgiler.TUMPoliceNo, ekNo, yenilemeNo);
                if (pdfBasimi != null)
                {

                }
            }
            catch (Exception ex)
            {
                _Log.Error("KoruFerdiKaza.DekontPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        private int MusteriKaydet(KoruMusteriModel model)
        {
            int _TVMKodu = _AktifKullaniciService.TVMKodu;
            if (model.tvmKodu.HasValue && model.tvmKodu.Value > 0)
                _TVMKodu = model.tvmKodu.Value;

            MusteriGenelBilgiler genelBilgiler = _MusteriService.GetMusteriTeklifFor(model.TcVkn, _TVMKodu);
            if (genelBilgiler != null)
            {
                if (genelBilgiler.MusteriKodu > 0)
                {
                    genelBilgiler.MusteriKodu = genelBilgiler.MusteriKodu;

                    if (String.IsNullOrEmpty(model.AdiUnvan))
                    {
                        genelBilgiler.AdiUnvan = ".";
                    }
                    else
                    {
                        genelBilgiler.AdiUnvan = model.AdiUnvan;
                    }

                    genelBilgiler.SoyadiUnvan = model.SoyadiUnvan;
                    if (String.IsNullOrEmpty(genelBilgiler.SoyadiUnvan))
                        genelBilgiler.SoyadiUnvan = ".";

                    genelBilgiler.EMail = model.Email;

                    if (!String.IsNullOrEmpty(model.MeslekKodu))
                    {
                        genelBilgiler.MeslekKodu = Convert.ToInt32(model.MeslekKodu);
                    }
                    if (model.DogumTarihi.HasValue)
                    {
                        genelBilgiler.DogumTarihi = model.DogumTarihi;
                    }

                    if (genelBilgiler.MusteriAdres != null)
                    {
                        var Teslimat = genelBilgiler.MusteriAdres.Where(w => w.AdresTipi == AdresTipleri.Teslimat).FirstOrDefault();
                        if (Teslimat == null)
                        {
                            MusteriAdre sigortaliAdres = new MusteriAdre();

                            int? maxSiraNo = genelBilgiler.MusteriAdres.Where(s => s.MusteriKodu == genelBilgiler.MusteriKodu).Max(p => (int?)p.SiraNo);

                            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

                            sigortaliAdres.SiraNo = siraNo;

                            sigortaliAdres.AdresTipi = AdresTipleri.Teslimat;
                            sigortaliAdres.UlkeKodu = "TUR";
                            sigortaliAdres.IlKodu = model.TeslimatIlKodu;
                            sigortaliAdres.IlceKodu = model.TeslimatIlceKodu;
                            sigortaliAdres.Adres = model.TeslimatAcikAdres;
                            sigortaliAdres.Mahalle = "";
                            sigortaliAdres.Cadde = "";
                            sigortaliAdres.Sokak = "";
                            sigortaliAdres.Apartman = "";
                            sigortaliAdres.BinaNo = "";
                            sigortaliAdres.DaireNo = "";
                            sigortaliAdres.PostaKodu = 0;
                            genelBilgiler.MusteriAdres.Add(sigortaliAdres);
                        }
                        else
                        {
                            Teslimat.IlKodu = model.TeslimatIlKodu;
                            Teslimat.IlceKodu = model.TeslimatIlceKodu;
                            Teslimat.Adres = model.TeslimatAcikAdres;
                            genelBilgiler.MusteriAdres.Add(Teslimat);
                        }
                        var Iletism = genelBilgiler.MusteriAdres.Where(w => w.AdresTipi == AdresTipleri.Iletisim).FirstOrDefault();
                        if (Iletism == null)
                        {
                            MusteriAdre sigortaliAdres = new MusteriAdre();
                            int? maxSiraNo = genelBilgiler.MusteriAdres.Where(s => s.MusteriKodu == genelBilgiler.MusteriKodu).Max(p => (int?)p.SiraNo);
                            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;
                            if (Teslimat == null)
                            {
                                siraNo += 1;
                            }
                            sigortaliAdres.SiraNo = siraNo;
                            sigortaliAdres.AdresTipi = AdresTipleri.Iletisim;
                            sigortaliAdres.UlkeKodu = "TUR";
                            sigortaliAdres.IlKodu = model.IletisimIlKodu;
                            sigortaliAdres.IlceKodu = model.IletisimIlceKodu;
                            sigortaliAdres.Adres = model.IletisimAcikAdres;
                            sigortaliAdres.Mahalle = "";
                            sigortaliAdres.Cadde = "";
                            sigortaliAdres.Sokak = "";
                            sigortaliAdres.Apartman = "";
                            sigortaliAdres.BinaNo = "";
                            sigortaliAdres.DaireNo = "";
                            sigortaliAdres.PostaKodu = 0;
                            sigortaliAdres.Varsayilan = true;
                            genelBilgiler.MusteriAdres.Add(sigortaliAdres);
                        }
                        else
                        {
                            Iletism.IlKodu = model.IletisimIlKodu;
                            Iletism.IlceKodu = model.IletisimIlceKodu;
                            Iletism.Adres = model.IletisimAcikAdres;
                            genelBilgiler.MusteriAdres.Add(Iletism);
                        }
                    }
                    if (genelBilgiler.MusteriTelefons != null)
                    {
                        var cepTel = genelBilgiler.MusteriTelefons.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                        if (cepTel == null)
                        {
                            MusteriTelefon cep = new MusteriTelefon();
                            cep.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                            cep.Numara = model.CepTelefonu;
                            cep.NumaraSahibi = model.AdiUnvan + " " + model.SoyadiUnvan;
                            int? maxSiraNo = genelBilgiler.MusteriTelefons.Where(s => s.MusteriKodu == genelBilgiler.MusteriKodu).Max(p => (int?)p.SiraNo);
                            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;
                            cep.SiraNo = siraNo;
                            genelBilgiler.MusteriTelefons.Add(cep);
                        }
                        else
                        {
                            cepTel.Numara = model.CepTelefonu;
                            cepTel.NumaraSahibi = model.AdiUnvan + " " + model.SoyadiUnvan;
                            genelBilgiler.MusteriTelefons.Add(cepTel);
                        }
                    }


                    if (genelBilgiler.MusteriNots != null)
                    {
                        var nots = genelBilgiler.MusteriNots.Where(w => w.Konu == "Kullanıcı Sözleşmesi").FirstOrDefault();
                        if (nots == null)
                        {
                            //Musteri.Sigortali.MusteriKodu;
                            MusteriNot not = new MusteriNot();
                            not.Konu = "Kullanıcı Sözleşmesi";
                            not.SiraNo = 1;
                            not.TVMKodu = _AktifKullaniciService.TVMKodu;
                            not.NotAciklamasi = "Kullanıcı Sözleşmesi Onaylanmıştır.";
                            not.KayitTarihi = TurkeyDateTime.Now;
                            not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                            genelBilgiler.MusteriNots.Add(not);

                            not = new MusteriNot();
                            not.Konu = "E Ticaret İzni";
                            not.SiraNo = 2;
                            not.TVMKodu = _AktifKullaniciService.TVMKodu;
                            not.NotAciklamasi = "E Ticaret İzni Onaylanmıştır.";
                            not.KayitTarihi = TurkeyDateTime.Now;
                            not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                            genelBilgiler.MusteriNots.Add(not);

                            not = new MusteriNot();
                            not.Konu = "Lilyum Kart Hizmetleri";
                            not.SiraNo = 3;
                            not.TVMKodu = _AktifKullaniciService.TVMKodu;
                            not.NotAciklamasi = "Lilyum Kart Hizmetleri Onaylanmıştır.";
                            not.KayitTarihi = TurkeyDateTime.Now;
                            not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                            genelBilgiler.MusteriNots.Add(not);
                        }
                    }

                    _MusteriService.UpdateMusteri(genelBilgiler);
                    return genelBilgiler.MusteriKodu;
                }
                return 0;
            }
            try
            {
                MusteriGenelBilgiler sigortali = new MusteriGenelBilgiler();
                if (String.IsNullOrEmpty(model.AdiUnvan))
                {
                    sigortali.AdiUnvan = ".";
                }
                else
                {
                    sigortali.AdiUnvan = model.AdiUnvan;
                }

                sigortali.SoyadiUnvan = model.SoyadiUnvan;
                if (String.IsNullOrEmpty(sigortali.SoyadiUnvan))
                    sigortali.SoyadiUnvan = ".";

                sigortali.EMail = model.Email;
                sigortali.KimlikNo = model.TcVkn;
                sigortali.MusteriTipKodu = model.TcVkn.Length == 11 ? MusteriTipleri.TCMusteri : MusteriTipleri.TuzelMusteri;
                sigortali.Uyruk = UyrukTipleri.TC;
                sigortali.TVMMusteriKodu = "";
                sigortali.TVMKodu = _TVMKodu;
                sigortali.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                if (!String.IsNullOrEmpty(model.MeslekKodu))
                {
                    sigortali.MeslekKodu = Convert.ToInt32(model.MeslekKodu);
                }
                if (model.DogumTarihi.HasValue)
                {
                    sigortali.DogumTarihi = model.DogumTarihi;
                }

                MusteriAdre sigortaliAdres = new MusteriAdre();
                sigortaliAdres.SiraNo = 2;
                sigortaliAdres.AdresTipi = AdresTipleri.Teslimat;
                sigortaliAdres.UlkeKodu = "TUR";
                sigortaliAdres.IlKodu = model.TeslimatIlKodu;
                sigortaliAdres.IlceKodu = model.TeslimatIlceKodu;
                sigortaliAdres.Adres = model.TeslimatAcikAdres;
                sigortaliAdres.Mahalle = "";
                sigortaliAdres.Cadde = "";
                sigortaliAdres.Sokak = "";
                sigortaliAdres.Apartman = "";
                sigortaliAdres.BinaNo = "";
                sigortaliAdres.DaireNo = "";
                sigortaliAdres.PostaKodu = 0;
                sigortali.MusteriAdres.Add(sigortaliAdres);

                sigortaliAdres = new MusteriAdre();
                sigortaliAdres.SiraNo = 1;
                sigortaliAdres.AdresTipi = AdresTipleri.Iletisim;
                sigortaliAdres.UlkeKodu = "TUR";
                sigortaliAdres.IlKodu = model.IletisimIlKodu;
                sigortaliAdres.IlceKodu = model.IletisimIlceKodu;
                sigortaliAdres.Adres = model.IletisimAcikAdres;
                sigortaliAdres.Mahalle = "";
                sigortaliAdres.Cadde = "";
                sigortaliAdres.Sokak = "";
                sigortaliAdres.Apartman = "";
                sigortaliAdres.BinaNo = "";
                sigortaliAdres.DaireNo = "";
                sigortaliAdres.PostaKodu = 0;
                sigortaliAdres.Varsayilan = true;
                sigortali.MusteriAdres.Add(sigortaliAdres);

                MusteriTelefon cepTel = new MusteriTelefon();

                cepTel.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                cepTel.Numara = model.CepTelefonu;
                cepTel.NumaraSahibi = model.AdSoyadUnvan;
                cepTel.SiraNo = 1;
                sigortali.MusteriTelefons.Add(cepTel);

                //Musteri.Sigortali.MusteriKodu;
                MusteriNot not = new MusteriNot();
                not.SiraNo = 1;
                not.Konu = "Kullanıcı Sözleşmesi";
                not.TVMKodu = _AktifKullaniciService.TVMKodu;
                not.NotAciklamasi = "Kullanıcı Sözleşmesi Onaylanmıştır.";
                not.KayitTarihi = TurkeyDateTime.Now;
                not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                sigortali.MusteriNots.Add(not);

                not = new MusteriNot();
                not.SiraNo = 2;
                not.Konu = "E Ticaret İzni";
                not.TVMKodu = _AktifKullaniciService.TVMKodu;
                not.NotAciklamasi = "E Ticaret İzni Onaylanmıştır.";
                not.KayitTarihi = TurkeyDateTime.Now;
                not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                sigortali.MusteriNots.Add(not);

                not = new MusteriNot();
                not.SiraNo = 3;
                not.Konu = "Lilyum Kart Hizmetleri";
                not.TVMKodu = _AktifKullaniciService.TVMKodu;
                not.NotAciklamasi = "Lilyum Kart Hizmetleri Onaylanmıştır.";
                not.KayitTarihi = TurkeyDateTime.Now;
                not.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                sigortali.MusteriNots.Add(not);

                var MusteriCreate = _MusteriService.CreateMusteri(sigortali);
                if (MusteriCreate != null)
                {
                    return MusteriCreate.MusteriKodu;
                }
                else
                {
                    return 0;

                }
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        private class KoruMusteriModel
        {
            public string TcVkn { get; set; }

            public string AdiUnvan { get; set; }

            public string SoyadiUnvan { get; set; }

            public string TeslimatIlKodu { get; set; }
            public int TeslimatIlceKodu { get; set; }
            public string TeslimatAcikAdres { get; set; }

            public string IletisimIlKodu { get; set; }

            public int IletisimIlceKodu { get; set; }
            public string IletisimAcikAdres { get; set; }

            public string Email { get; set; }

            public string CepTelefonu { get; set; }

            public string AdSoyadUnvan
            {
                get
                {
                    return String.Format("{0} {1}", this.AdiUnvan, this.SoyadiUnvan);
                }
            }
            public int? tvmKodu { get; set; }

            public DateTime? DogumTarihi { get; set; }
            public string MeslekKodu { get; set; }
        }
        public class RequestParatikaJsonModel
        {
            public string sessionToken { get; set; }
            public string responseCode { get; set; }
            public string responseMsg { get; set; }
            public string errorMsg { get; set; }

        }

        public string LilyumParatika3DSSonOdemeDurumu(string guidId)
        {
            var odemeDurumu = "";

            ParatikaClient sessionToken = new ParatikaClient();

            JObject odemeSorgulama = sessionToken.sessionTokenGuid(guidId);
            if (odemeSorgulama != null)
            {
                RequestParatikaOdemeDurumuJsonModel model = JsonConvert.DeserializeObject<RequestParatikaOdemeDurumuJsonModel>(odemeSorgulama.ToString());
                if (!String.IsNullOrEmpty(model.transactionList.Count.ToString()))
                {
                    for (int i = 0; i < model.transactionList.Count; i++)
                    {
                        if (model.transactionList[i].transactionType == "SALE")
                        {
                            if (model.transactionList[i].transactionStatus == "FA")
                            {
                                model.transactionList[i].transactionStatus = "Başarısız";
                            }
                            else if (model.transactionList[i].transactionStatus == "IP")
                            {
                                model.transactionList[i].transactionStatus = "İşlem Devam Ediyor Lütfen Daha Sonra Tekrar Kontrol ediniz";
                            }
                            else if (model.transactionList[i].transactionStatus == "AP")
                            {
                                model.transactionList[i].transactionStatus = "Başarılı";
                            }
                            else if (model.transactionList[i].transactionStatus == "VD")
                            {
                                model.transactionList[i].transactionStatus = "Yapılan Ödeme İade edildi";
                            }
                            odemeDurumu = model.transactionList[i].transactionStatus + " " + model.transactionList[i].pgTranErrorText;
                        }
                    }
                    this.AddWebServisCevap(Common.WebServisCevaplar.Koru3DParatikaSonOdemeDurumu, odemeDurumu);
                }
            }
            else
            {
                odemeDurumu = "Paratika Ödeme Durumu Sorgulaması yapılırken hata oluştu. Lütfen yöneticinize başvurunuz.";
            }

            return odemeDurumu;
        }
        public class RequestParatikaOdemeDurumuJsonModel
        {
            public List<transactionList> transactionList = new List<transactionList>();
        }
        public class transactionList
        {
            public string transactionStatus { get; set; } //ödeme FA:ödeme başarısız,AP:odeme başarılı
            public string pgTranErrorText { get; set; } //hata mesajı
            public string transactionType { get; set; } //ödeme tipi SAle olan list dikkate alınacak
        }

    }
}
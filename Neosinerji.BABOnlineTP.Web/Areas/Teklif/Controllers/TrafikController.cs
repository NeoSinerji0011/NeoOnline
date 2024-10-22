using AutoMapper;
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.TeklifYapayZeka;
using Neosinerji.BABOnlineTP.Business.Common.SBM;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.TrafikSigortasi)]
    public class TrafikController : TeklifController
    {
        private ITeklifYapayZekaService _TeklifYapayZekaService;
        public TrafikController(ITVMService tvmService,
                                ITeklifService teklifService,
                                IMusteriService musteriService,
                                IKullaniciService kullaniciService,
                                IAktifKullaniciService aktifKullaniciService,
                                ITanimService tanimService,
                                IUlkeService ulkeService,
                                ICRService crService,
                                IAracService aracService,
                                IUrunService urunService,
                                ITUMService tumService
                                )
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService, ulkeService, crService, aracService, urunService, tumService)
        {
            _TeklifYapayZekaService = DependencyResolver.Current.GetService<ITeklifYapayZekaService>();
        }

        [HttpGet]
        public ActionResult Ekle(int? id)
        {
            TrafikModel model = EkleModel(id, null);
            model.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            //

            TrafikModel model = EkleModel(id, teklifId);
            model.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            return View(model);
        }

        public TrafikModel EkleModel(int? id, int? teklifId)
        {
            //

            ILogService log = DependencyResolver.Current.GetService<ILogService>();
            log.Visit();

            ITeklif teklif = null;
            TrafikModel model = new TrafikModel();
            int? sigortaliMusteriKodu = null;
            //Teklifi hazırlayan
            model.Hazirlayan = base.EkleHazirlayanModel();

            //Sigorta Ettiren / Sigortalı....
            model.Musteri = new SigortaliModel();
            model.Musteri.SigortaliAyni = true;
            model.Musteri.EMailRequired = false; //Email zorunluluğu kaldırılıyor.....

            if (teklifId.HasValue)
            {
                model.TekrarTeklif = true;
                teklif = _TeklifService.GetTeklif(teklifId.Value);
                id = teklif.SigortaEttiren.MusteriKodu;

                TeklifSigortali sigortali = teklif.Sigortalilar.FirstOrDefault();
                if (sigortali != null)
                {
                    if (id != sigortali.MusteriKodu)
                    {
                        model.Musteri.SigortaliAyni = false;
                        sigortaliMusteriKodu = sigortali.MusteriKodu;
                    }
                }
            }

            List<SelectListItem> ulkeler = new List<SelectListItem>();
            ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });

            if (id.HasValue)
            {
                model.Musteri.SigortaEttiren = base.EkleMusteriModel(id.Value);
                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu), "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();

                List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                model.Musteri.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text", model.Musteri.SigortaEttiren.MusteriTelTipKodu).ListWithOptionLabel();

            }
            else
            {
                model.Musteri.SigortaEttiren = new MusteriModel();
                model.Musteri.SigortaEttiren.UlkeKodu = "TUR";
                model.Musteri.SigortaEttiren.Cinsiyet = "E";
                model.Musteri.SigortaEttiren.CepTelefonu = "90";

                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi").ListWithOptionLabel();
                List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });
                model.Musteri.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();

            }

            if (sigortaliMusteriKodu.HasValue)
            {
                model.Musteri.Sigortali = base.EkleMusteriModel(sigortaliMusteriKodu.Value);

                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu), "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();
            }
            else
            {
                model.Musteri.Sigortali = new MusteriModel();
                model.Musteri.Sigortali.UlkeKodu = "TUR";
                model.Musteri.Sigortali.Cinsiyet = "E";
            }
            model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
            model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
            model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");

            //Araç bilgileri
            model.Arac = base.EkleAracModel();
            model.Arac.TescilIl = "34";
            model.Arac.TescilIller = new SelectList(_CRService.GetTescilIlList(), "Key", "Value", model.Arac.TescilIl).ListWithOptionLabel();
            model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value", "").ListWithOptionLabel(); ;
            model.Arac.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.Arac.AnadoluKullanimTipListe = new List<SelectListItem>();
            model.Arac.AnadoluKullanimSekilleri = new List<SelectListItem>();
            model.Arac.SigortaSirketleri = this.SigortaSirketleri;
            if (teklifId.HasValue)
            {
                model.Arac.PlakaKodu = teklif.Arac.PlakaKodu;
                model.Arac.PlakaNo = teklif.Arac.PlakaNo;
                model.Arac.KullanimSekliKodu = teklif.Arac.KullanimSekli;
                short kullanimSekliKodu = Convert.ToInt16(teklif.Arac.KullanimSekli);

                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    model.Arac.KullanimTarziKodu = teklif.Arac.KullanimTarzi;

                    List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekliKodu);
                    model.Arac.KullanimTarzlari = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
                }

                model.Arac.TescilIl = teklif.Arac.TescilIlKodu;
                model.Arac.TescilIlce = teklif.Arac.TescilIlceKodu;
                model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value").ListWithOptionLabel();

                model.Arac.MarkaKodu = teklif.Arac.Marka;
                List<AracMarka> markalar = _AracService.GetAracMarkaList(kullanimTarziKodu);
                model.Arac.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

                model.Arac.Model = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;

                model.Arac.TipKodu = teklif.Arac.AracinTipi;

                List<AracTip> tipler = _AracService.GetAracTipList(kullanimTarziKodu, model.Arac.MarkaKodu, model.Arac.Model);
                model.Arac.AracTipleri = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

                model.Arac.MotorNo = teklif.Arac.MotorNo;
                model.Arac.SaseNo = teklif.Arac.SasiNo;

                if (teklif.Arac.TrafikTescilTarihi.HasValue)
                    model.Arac.TrafikTescilTarihi = teklif.Arac.TrafikTescilTarihi.Value;

                if (teklif.Arac.TrafikCikisTarihi.HasValue)
                    model.Arac.TrafigeCikisTarihi = teklif.Arac.TrafikCikisTarihi.Value;

                model.Arac.PoliceBaslangicTarihi = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, DateTime.MinValue);

                model.Arac.TescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                model.Arac.TescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                model.Arac.AsbisNo = teklif.Arac.AsbisNo;

                if (teklif.Arac.KoltukSayisi.HasValue)
                { model.Arac.KisiSayisi = teklif.Arac.KoltukSayisi.Value; }

                model.Arac.MotorGucu = teklif.Arac.MotorGucu;
                model.Arac.SilindirHacmi = teklif.Arac.SilindirHacmi;
                //  model.Arac.ImalatYeri = teklif.Arac.ImalatYeri.HasValue ? teklif.Arac.ImalatYeri.Value : (byte)0;
                model.Arac.BelgeNumarasiTramer = teklif.Arac.TramerBelgeNo;
                model.Arac.BelgeTarihTramer = teklif.Arac.TramerBelgeTarihi.HasValue ? teklif.Arac.TramerBelgeTarihi.Value.ToString() : "";
                //  model.Arac.Renk = teklif.Arac.Renk;

                model.Arac.UygulananOncekiKademe = teklif.ReadSoru(TrafikSorular.UygulanmisTarifeBasamakKodu, String.Empty).ToString();
                model.Arac.HasarsizlikIndirim = teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.HasValue ? Convert.ToInt32(teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.Value).ToString() : "0";
                model.Arac.HasarSurprim = teklif.GenelBilgiler.HasarSurprimYuzdesi.HasValue ? Convert.ToInt32(teklif.GenelBilgiler.HasarSurprimYuzdesi.Value).ToString() : "0";
                model.Arac.UygulananKademe = teklif.GenelBilgiler.TarifeBasamakKodu.HasValue ? teklif.GenelBilgiler.TarifeBasamakKodu.Value.ToString() : "0";
            }


            //Eski poliçe bilgileri
            model.EskiPolice = base.EkleEskiPoliceModel();
            List<KeyValueItem<string, string>> islemTipleri = new List<KeyValueItem<string, string>>();
            islemTipleri.Add(new KeyValueItem<string, string>("1", babonline.PriorInformationWithIntroduction));
            islemTipleri.Add(new KeyValueItem<string, string>("11", babonline.PriorInformationSalesReferencesIntroduction));
            model.EskiPolice.TramerIslemTipleri = new SelectList(islemTipleri, "Key", "Value");
            model.EskiPolice.TramerIslemTipi = "1";
            if (teklifId.HasValue)
            {
                model.EskiPolice.EskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                if (model.EskiPolice.EskiPoliceVar)
                {
                    model.EskiPolice.PoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    model.EskiPolice.SigortaSirketiKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    model.EskiPolice.AcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    model.EskiPolice.YenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                    model.EskiPolice.TramerIslemTipi = teklif.ReadSoru(TrafikSorular.TramerIslemTipi, String.Empty);
                    model.EskiPolice.SigortaSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                }
            }

            //Taşıyıcı sorumluluk
            model.Tasiyici = base.EkleTasiyiciSorumlulukModel();
            if (teklifId.HasValue)
            {
                model.Tasiyici.YetkiBelgesi = teklif.ReadSoru(TrafikSorular.Tasima_Yetki_Belgesi_VarYok, false);
                model.Tasiyici.Sorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);
                if (model.Tasiyici.Sorumluluk)
                {
                    model.Tasiyici.PoliceNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                    model.Tasiyici.SigortaSirketiKodu = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                    model.Tasiyici.AcenteNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                    model.Tasiyici.YenilemeNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
                }
            }

            //Trafik teminat bilgileri
            model.Teminat = new TrafikTeminatModel();
            // model.Teminat.IMM = new SelectList(_CRService.GetTrafikIMM(), "Key", "Value", "").ListWithOptionLabel(); 
            //model.Teminat.FK = new SelectList(_CRService.GetTrafikFK(), "Key", "Value", "").ListWithOptionLabel();

            model.Teminat.IMM = new SelectList(_CRService.GetTrafikIMMListe("111", "11"), "Key", "Value", "").ListWithOptionLabel();
            model.Teminat.FK = new SelectList(_CRService.GetTrafikFKListe("111", "11"), "Key", "Value", "").ListWithOptionLabel();

            if (teklifId.HasValue)
            {
                string immKodu = teklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(immKodu))
                {
                    model.Teminat.IMMKodu = Convert.ToInt16(immKodu);
                }

                string fkKodu = teklif.ReadSoru(TrafikSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKodu))
                {
                    model.Teminat.FKKodu = Convert.ToInt16(fkKodu);
                }

                model.Teminat.Asistans = teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
                model.Teminat.SinirsizIMM = teklif.ReadSoru(TrafikSorular.Teminat_SinirsizIMM, false);
            }

            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.TrafikSigortasi);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

            #region Odeme
            model.TaksitliOdeme = new TrafikTeklifOdemeModel();

            model.TaksitliOdeme.OdemeSekli = true;
            model.TaksitliOdeme.OdemeTipi = 2;
            model.TaksitliOdeme.TaksitSayilari = new List<SelectListItem>();
            model.TaksitliOdeme.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", "2").ToList();

            model.TaksitliOdeme.TaksitSayilari.AddRange(
            new SelectListItem[]{
                 new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});

            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash,Value="1"},
                new SelectListItem(){Text=babonline.Forward,Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = odemeSekilleri;

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text").ToList();

            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
            taksitSeceneleri.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});
            model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();

            #endregion

            return model;
        }

        public ActionResult Detay(int id)
        {
            DetayTrafikModel model = new DetayTrafikModel();
            TrafikTeklif trafikTeklif = new TrafikTeklif(id);

            model.TeklifId = id;
            model.TeklifNo = trafikTeklif.Teklif.TeklifNo.ToString();

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(trafikTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(trafikTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (trafikTeklif.Teklif.Sigortalilar.Count > 0 &&
               (trafikTeklif.Teklif.SigortaEttiren.MusteriKodu != trafikTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(trafikTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            //Araç bilgileri
            model.Arac = base.DetayAracModel(trafikTeklif.Teklif);

            //Eski poliçe bilgileri
            model.EskiPolice = base.DetayEskiPoliceModel(trafikTeklif.Teklif);
            model.EskiPolice.TramerIslemTipi = String.Empty;
            string tramerIslemTipi = trafikTeklif.Teklif.ReadSoru(TrafikSorular.TramerIslemTipi, String.Empty);
            if (tramerIslemTipi == "1")
                model.EskiPolice.TramerIslemTipi = "1 - Önceki Bilgileri İle Giriş";
            else if (tramerIslemTipi == "11")
                model.EskiPolice.TramerIslemTipi = "11 - Önceki Bilgiler (Satış Referansılı Giriş)";

            //Taşıyıcı sorumluluk bilgileri
            model.Tasiyici = base.DetayTasiyiciSorumlulukModel(trafikTeklif.Teklif);

            model.Teminat = new DetayTrafikTeminatModel();
            string immKodu = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, "0");

            if (!String.IsNullOrEmpty(immKodu) && immKodu != "0" && immKodu != "H")
            {
                ITeklif teklif = trafikTeklif.Teklif;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');

                TrafikIMM imm = _CRService.GetTrafikIMMBedel(Convert.ToInt32(immKodu), parts[0], parts[1]);
                model.Teminat.IMM = imm != null ? imm.Text : String.Empty;
            }

            string fkKodu = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_FK_Kademe, "0");
            if (!String.IsNullOrEmpty(fkKodu) && fkKodu != "0" && fkKodu != "H")
            {
                ITeklif teklif = trafikTeklif.Teklif;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');

                TrafikFK fk = _CRService.GetTrafikFKBedel(Convert.ToInt32(fkKodu), parts[0], parts[1]);
                model.Teminat.FK = fk != null ? fk.Text : String.Empty;
            }
            model.Teminat.Asistans = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
            model.Teminat.SinirsizIMM = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_SinirsizIMM, false);

            model.Fiyat = TrafikFiyat(trafikTeklif);

            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = trafikTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = trafikTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = trafikTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
            taksitSeceneleri.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});
            model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();
            return View(model);
        }

        [HttpPost]
        //[PolicelestirmeYetkiAttribute]  //Kontrol edilecek, karşılaştırmalı ortama göre uyralanacak
        public ActionResult OdemeAl(OdemeTrafikModel model)
        {
            if (ViewBag.PolicelestirmeKontrol != null)
            {
                return Json(ViewBag.PolicelestirmeKontrol);
            }
            TeklifOdemeCevapModel cevap = new TeklifOdemeCevapModel();

            if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Nakit)
            {
                TryValidateModel(model);
                if (ModelState["KrediKarti.KartSahibi"] != null)
                    ModelState["KrediKarti.KartSahibi"].Errors.Clear();
                if (ModelState["KrediKarti.KartNumarasi"] != null)
                    ModelState["KrediKarti.KartNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.GuvenlikNumarasi"] != null)
                    ModelState["KrediKarti.GuvenlikNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaAy"] != null)
                    ModelState["KrediKarti.SonKullanmaAy"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaYil"] != null)
                    ModelState["KrediKarti.SonKullanmaYil"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                nsbusiness.ITeklif teklif = _TeklifService.GetTeklif(model.KrediKarti.KK_TeklifId);
                if (!String.IsNullOrEmpty(teklif.GenelBilgiler.TUMPoliceNo))
                {
                    string msg = String.Format("Bu teklif daha önce poliçeleştirilmiş : {0}. Lütfen yeni bir teklif alarak poliçeleştirin.", teklif.GenelBilgiler.TUMPoliceNo);
                    cevap.Hatalar = new string[] { msg };
                    cevap.RedirectUrl = String.Empty;
                    return Json(cevap);
                }
                nsbusiness.Odeme odeme = null;
                if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil, model.KrediKarti.KK_OdemeSekli);
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin)
                        odeme.TaksitSayisi = 1;
                    else
                        odeme.TaksitSayisi = model.KrediKarti.TaksitSayisi;
                }
                else
                {
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin || model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Havale)
                    {
                        odeme = new Odeme(OdemeSekilleri.Pesin, 1);
                    }
                    else
                    {
                        odeme = new Odeme(model.KrediKarti.KK_OdemeSekli, model.KrediKarti.TaksitSayisi);
                    }
                }
                ITeklif urun = TeklifUrunFactory.AsUrunClass(teklif);
                urun.Policelestir(odeme);

                if (urun.Hatalar.Count == 0)
                {
                    try
                    {
                        urun.PolicePDF();
                    }
                    catch (PolicePDFException ex)
                    {
                        _LogService.Error(ex);
                    }

                    cevap.Success = true;
                    cevap.RedirectUrl = TeklifSayfaAdresleri.PoliceAdres(urun.GenelBilgiler.UrunKodu) + urun.GenelBilgiler.TeklifId;
                    return Json(cevap);
                }
                if (teklif.TUMKodu == TeklifUretimMerkezleri.SOMPOJAPAN && odeme.TaksitSayisi > 1)
                {
                    cevap.SompoJapanTaksitliMi = true;
                }
                else
                {
                    cevap.SompoJapanTaksitliMi = false;
                }
                cevap.Hatalar = urun.Hatalar.ToArray();
                cevap.RedirectUrl = String.Empty;
                return Json(cevap);
            }

            cevap.Hatalar = new string[] { babonline.Message_RequiredValues };

            return Json(cevap);
        }

        public ActionResult Police(int id)
        {
            DetayTrafikModel model = new DetayTrafikModel();
            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif trafikTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            model.TeklifId = trafikTeklif.GenelBilgiler.TeklifId;

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(trafikTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(trafikTeklif.SigortaEttiren.MusteriKodu);

            //Araç bilgileri
            model.Arac = base.DetayAracModel(trafikTeklif);

            //Eski poliçe bilgileri
            model.EskiPolice = base.DetayEskiPoliceModel(trafikTeklif);

            //Taşıyıcı sorumluluk bilgileri
            model.Tasiyici = base.DetayTasiyiciSorumlulukModel(trafikTeklif);

            model.Teminat = new DetayTrafikTeminatModel();
            string immKodu = trafikTeklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, "0");

            if (!String.IsNullOrEmpty(immKodu) && immKodu != "0")
            {
                CR_TrafikIMM imm = _CRService.GetTrafikIMM(TeklifUretimMerkezleri.HDI, Convert.ToInt16(immKodu));
                model.Teminat.IMM = imm != null ? imm.Text : String.Empty;
            }

            string fkKodu = trafikTeklif.ReadSoru(TrafikSorular.Teminat_FK_Kademe, "0");
            if (!String.IsNullOrEmpty(fkKodu) && fkKodu != "0")
            {
                CR_TrafikFK fk = _CRService.GetTrafikFK(TeklifUretimMerkezleri.HDI, Convert.ToInt16(fkKodu));
                model.Teminat.FK = fk != null ? fk.Text : String.Empty;
            }
            model.Teminat.Asistans = trafikTeklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);

            model.OdemeBilgileri = new OdemeBilgileriModel();
            model.OdemeBilgileri.TeklifId = teklif.GenelBilgiler.TeklifId;
            model.OdemeBilgileri.NetPrim = teklif.GenelBilgiler.NetPrim;
            model.OdemeBilgileri.ToplamVergi = teklif.GenelBilgiler.ToplamVergi;
            model.OdemeBilgileri.ToplamTutar = teklif.GenelBilgiler.BrutPrim;
            model.OdemeBilgileri.PoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            model.OdemeBilgileri.PoliceURL = teklif.GenelBilgiler.PDFPolice;
            model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFGenelSartlari;

            if (teklif.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.MAPFRE)
                model.OdemeBilgileri.DekontPDFGoster = teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti;

            if (teklif.GenelBilgiler.PDFDekont != null)
            {
                model.OdemeBilgileri.DekontPDFGoster = true;
                model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFDekont;
            }
            else
            {
                model.OdemeBilgileri.DekontPDFGoster = false;
            }

            TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);
            if (tum != null)
            {
                model.OdemeBilgileri.TUMUnvani = tum.Unvani;
                model.OdemeBilgileri.TUMLogoURL = tum.Logo;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Hesapla(TrafikModel model)
        {
            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model.EskiPolice != null && !model.EskiPolice.EskiPoliceVar)
                {
                    if (ModelState["EskiPolice.SigortaSirketiKodu"] != null)
                        ModelState["EskiPolice.SigortaSirketiKodu"].Errors.Clear();

                    if (ModelState["EskiPolice.AcenteNo"] != null)
                        ModelState["EskiPolice.AcenteNo"].Errors.Clear();

                    if (ModelState["EskiPolice.PoliceNo"] != null)
                        ModelState["EskiPolice.PoliceNo"].Errors.Clear();

                    if (ModelState["EskiPolice.YenilemeNo"] != null)
                        ModelState["EskiPolice.YenilemeNo"].Errors.Clear();
                }

                if (model.Tasiyici != null && !model.Tasiyici.Sorumluluk)
                {
                    if (ModelState["Tasiyici.SigortaSirketiKodu"] != null)
                        ModelState["Tasiyici.SigortaSirketiKodu"].Errors.Clear();

                    if (ModelState["Tasiyici.AcenteNo"] != null)
                        ModelState["Tasiyici.AcenteNo"].Errors.Clear();

                    if (ModelState["Tasiyici.PoliceNo"] != null)
                        ModelState["Tasiyici.PoliceNo"].Errors.Clear();

                    if (ModelState["Tasiyici.YenilemeNo"] != null)
                        ModelState["Tasiyici.YenilemeNo"].Errors.Clear();
                }

                if (model.Arac.PlakaNo == "YK")
                {
                    if (ModelState["Arac.TescilBelgeSeriKod"] != null)
                        ModelState["Arac.TescilBelgeSeriKod"].Errors.Clear();
                    if (ModelState["Arac.TescilBelgeSeriNo"] != null)
                        ModelState["Arac.TescilBelgeSeriNo"].Errors.Clear();
                    if (ModelState["Arac.AsbisNo"] != null)
                        ModelState["Arac.AsbisNo"].Errors.Clear();
                    if (ModelState["Arac.TescilIlce"] != null)
                        ModelState["Arac.TescilIlce"].Errors.Clear();
                }

                if (!String.IsNullOrEmpty(model.Arac.AsbisNo))
                {
                    if (ModelState["Arac.TescilBelgeSeriKod"] != null)
                        ModelState["Arac.TescilBelgeSeriKod"].Errors.Clear();
                    if (ModelState["Arac.TescilBelgeSeriNo"] != null)
                        ModelState["Arac.TescilBelgeSeriNo"].Errors.Clear();
                }
                if (!String.IsNullOrEmpty(model.Arac.TescilBelgeSeriNo))
                {
                    if (ModelState["Arac.AsbisNo"] != null)
                        ModelState["Arac.AsbisNo"].Errors.Clear();
                }

                ModelStateMusteriClear(ModelState, model.Musteri);

                #endregion

                #region Teklif kaydı ve hesaplamanın başlatılması
                if (ModelState.IsValid)
                {
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.TrafikSigortasi, model.Hazirlayan.TVMKodu, model.Hazirlayan.TVMKullaniciKodu, model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    ////Sigortali
                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    //Araç Bilgileri
                    teklif.Arac.PlakaKodu = model.Arac.PlakaKodu;
                    teklif.Arac.PlakaNo = model.Arac.PlakaNo.ToUpperInvariant();
                    teklif.Arac.Marka = model.Arac.MarkaKodu;
                    teklif.Arac.AracinTipi = model.Arac.TipKodu;
                    teklif.Arac.Model = model.Arac.Model;
                    teklif.Arac.MotorNo = model.Arac.MotorNo;
                    teklif.Arac.SasiNo = model.Arac.SaseNo;
                    teklif.Arac.KullanimSekli = model.Arac.KullanimSekliKodu;
                    teklif.Arac.KullanimTarzi = model.Arac.KullanimTarziKodu;
                    teklif.Arac.TrafikTescilTarihi = model.Arac.TrafikTescilTarihi;
                    teklif.Arac.TrafikCikisTarihi = model.Arac.TrafigeCikisTarihi;
                    teklif.Arac.TescilSeriKod = model.Arac.TescilBelgeSeriKod;
                    teklif.Arac.TescilSeriNo = model.Arac.TescilBelgeSeriNo;
                    teklif.Arac.AsbisNo = model.Arac.AsbisNo;
                    teklif.Arac.TescilIlKodu = model.Arac.TescilIl;
                    teklif.Arac.TescilIlceKodu = model.Arac.TescilIlce;
                    teklif.Arac.KoltukSayisi = model.Arac.KisiSayisi;

                    teklif.Arac.MotorGucu = model.Arac.MotorGucu;
                    teklif.Arac.SilindirHacmi = model.Arac.SilindirHacmi;
                    teklif.Arac.Renk = model.Arac.Renk;
                    //teklif.Arac.ImalatYeri = model.Arac.ImalatYeri;
                    teklif.Arac.TramerBelgeNo = model.Arac.BelgeNumarasiTramer;

                    //Araç Hasarsızlık bilgileri

                    if (!String.IsNullOrEmpty(model.Arac.UygulananKademe))
                    {
                        teklif.Arac.TarifeBasamagi = Convert.ToInt16(model.Arac.UygulananKademe);
                        teklif.GenelBilgiler.TarifeBasamakKodu = Convert.ToInt16(model.Arac.UygulananKademe);
                        teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToDecimal(model.Arac.HasarsizlikIndirim);
                        teklif.GenelBilgiler.HasarSurprimYuzdesi = Convert.ToDecimal(model.Arac.HasarSurprim);

                        teklif.AddSoru(TrafikSorular.UygulanmisTarifeBasamakKodu, model.Arac.UygulananOncekiKademe);
                    }

                    if (!String.IsNullOrEmpty(model.Arac.BelgeTarihTramer))
                    {
                        teklif.Arac.TramerBelgeTarihi = Convert.ToDateTime(model.Arac.BelgeTarihTramer);
                    }

                    if (!String.IsNullOrEmpty(model.Arac.AnadoluKullanimTip))
                    {
                        teklif.AddSoru(TrafikSorular.AnadoluKullanimTipi, model.Arac.AnadoluKullanimTip);
                    }
                    if (!String.IsNullOrEmpty(model.Arac.AnadoluKullanimSekli))
                    {
                        teklif.AddSoru(TrafikSorular.AnadoluKullanimSekli, model.Arac.AnadoluKullanimSekli);
                    }

                    //Sorular
                    teklif.AddSoru(TrafikSorular.Police_Baslangic_Tarihi, model.Arac.PoliceBaslangicTarihi);
                    //// Fesih Tarihi
                    //if (model.Arac.FesihTarihi.HasValue)
                    //    teklif.AddSoru(TrafikSorular.FesihTarihi, model.Arac.FesihTarihi.Value);


                    //Eski Poliçe
                    teklif.AddSoru(TrafikSorular.Eski_Police_VarYok, model.EskiPolice.EskiPoliceVar);
                    if (model.EskiPolice.EskiPoliceVar)
                    {
                        teklif.AddSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, model.EskiPolice.SigortaSirketiKodu);
                        teklif.AddSoru(TrafikSorular.Eski_Police_Acente_No, model.EskiPolice.AcenteNo);
                        teklif.AddSoru(TrafikSorular.Eski_Police_No, model.EskiPolice.PoliceNo);
                        teklif.AddSoru(TrafikSorular.Eski_Police_Yenileme_No, model.EskiPolice.YenilemeNo);
                        teklif.AddSoru(TrafikSorular.TramerIslemTipi, model.EskiPolice.TramerIslemTipi);
                    }

                    //Taşıma yetki belgesi ve taşıyıcı sorumluluk
                    teklif.AddSoru(TrafikSorular.Tasima_Yetki_Belgesi_VarYok, model.Tasiyici.YetkiBelgesi);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, model.Tasiyici.Sorumluluk);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, model.Tasiyici.SigortaSirketiKodu);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, model.Tasiyici.AcenteNo);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, model.Tasiyici.PoliceNo);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, model.Tasiyici.YenilemeNo);

                    //Teminatlar
                    teklif.AddSoru(TrafikSorular.Teminat_IMM_Kademe, model.Teminat.IMMKodu.ToNullSafeString());
                    teklif.AddSoru(TrafikSorular.Teminat_FK_Kademe, model.Teminat.FKKodu.ToNullSafeString());
                    teklif.AddSoru(TrafikSorular.Teminat_Asistans, model.Teminat.Asistans);
                    teklif.AddSoru(TrafikSorular.Teminat_SinirsizIMM, model.Teminat.SinirsizIMM);

                    ITrafikTeklif trafikTeklif = new TrafikTeklif();

                    //Teklif alınacak şirketler
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            trafikTeklif.AddUretimMerkezi(item.TUMKodu);
                    }


                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.TaksitliOdeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.TaksitliOdeme.OdemeTipi;

                    if (!model.TaksitliOdeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.TaksitliOdeme.TaksitSayisi;
                        trafikTeklif.AddOdemePlani(model.TaksitliOdeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        trafikTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    teklif.AddSoru(TrafikSorular.SENumaratipi, model.Musteri.SigortaEttiren.MusteriTelTipKodu.ToString());

                    //Sadece Pesin Teklif Alınıyor
                    //trafikTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    //teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    //teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;
                    //teklif.GenelBilgiler.TaksitSayisi = 1;

                    //Teklif alınacak ödeme tipleri
                    //foreach (var item in model.Odeme)
                    //{
                    //    if (item.TeklifAl)
                    //        trafikTeklif.AddOdemePlani(item.OdemePlaniAlternatifKodu);
                    //}

                    IsDurum isDurum = trafikTeklif.Hesapla(teklif);

                    //#region yapay zeka 
                    //TeklifYapayZekaData data = new TeklifYapayZekaData();

                    //#region yapay zeka data doldurulması
                    //if (model.Musteri.SigortaEttiren.Cinsiyet == "E")
                    //{
                    //    data.cinsiyet = "ERKEK";
                    //}
                    //else if (model.Musteri.SigortaEttiren.Cinsiyet == "K")
                    //{
                    //    data.cinsiyet = "KADIN";
                    //}
                    //else
                    //{
                    //    data.cinsiyet = "NULL";
                    //}
                    //int yas = -1;
                    //if (model.Musteri.SigortaEttiren.DogumTarihi != null)
                    //{
                    //    data.DTVarYok = "VAR";
                    //    DateTime dogumGunu = Convert.ToDateTime(model.Musteri.SigortaEttiren.DogumTarihi);
                    //    yas = TurkeyDateTime.Now.Year - dogumGunu.Year;
                    //    if (dogumGunu > TurkeyDateTime.Now.AddYears(-yas))
                    //        yas--;
                    //}
                    //else
                    //{
                    //    data.DTVarYok = "YOK";
                    //}
                    //data.YasKategorisi = yasAraligi(yas);
                    //var iller = _UlkeService.GetIlList();
                    //var il = iller.Where(x => x.IlKodu == model.Musteri.SigortaEttiren.IlKodu).FirstOrDefault();
                    //if (il != null)
                    //{
                    //    data.AtananIl = il.IlAdi;
                    //}
                    //else
                    //{
                    //    data.AtananIl = "YOK";
                    //}
                    //var ilceler = _UlkeService.GetIlceList();
                    //var ilce = ilceler.Where(x => x.IlceKodu == model.Musteri.SigortaEttiren.IlceKodu).FirstOrDefault();
                    //if (ilce != null)
                    //{
                    //    data.AtananIlce = ilce.IlceAdi;
                    //}
                    //else
                    //{
                    //    data.AtananIlce = "YOK";
                    //}
                    //// atanan mah sonradan güncellenecek
                    //data.AtananMah = "YOK";
                    //data.BransKodu = "1";
                    //data.TUMBirlikKodu = "0";
                    //data.Ekno = "0";
                    //data.YenilemeNo = "0";
                    //data.TanzimTrYil = "0";
                    //data.TanzimTrAy = "0";
                    //data.TanzimTrGun = "0";
                    //data.BasTrYil = model.Arac.PoliceBaslangicTarihi.Year.ToString();
                    //data.BasTrAy = model.Arac.PoliceBaslangicTarihi.Month.ToString();
                    //data.BasTrGun = model.Arac.PoliceBaslangicTarihi.Day.ToString();
                    //data.Tanzim_BaslangicTarGunFark = "0";
                    //data.BitTrYil = "0";
                    //data.BitTrAy = "0";
                    //data.BitTrGun = "0";
                    //data.BrutPrim = "0";
                    //data.NetPrim = "0";
                    //data.Komisyon = "0";
                    //data.OdemeSekli = model.TaksitliOdeme.OdemeTipi.ToString();
                    //data.Durum = "0";
                    //data.TaliKomisyonOran = "0";
                    //data.TaliKomisyon = "0";
                    //data.PlakaKodu = model.Arac.PlakaKodu;
                    //data.Marka = model.Arac.MarkaKodu;
                    //var aracMarka = _AracService.GetAracMarka(model.Arac.MarkaKodu);
                    //data.MarkaAciklama = aracMarka != null ? aracMarka.MarkaAdi : "NULL";
                    //data.Model = model.Arac.Model.ToString();
                    //var fiyati = _AracService.GetAracDeger(model.Arac.MarkaKodu, model.Arac.TipKodu, model.Arac.Model);
                    //data.AracFiyati = fiyati.ToString();
                    //string Sinif = "NULL";
                    //if (fiyati > 0 && fiyati < 70001)
                    //{
                    //    Sinif = "H";
                    //}
                    //else if (fiyati >= 70001 && fiyati < 120001)
                    //{
                    //    Sinif = "G";
                    //}
                    //else if (fiyati >= 120001 && fiyati < 170001)
                    //{
                    //    Sinif = "F";
                    //}
                    //else if (fiyati >= 170001 && fiyati < 300001)
                    //{
                    //    Sinif = "E";
                    //}
                    //else if (fiyati >= 300001 && fiyati < 600001)
                    //{
                    //    Sinif = "D";
                    //}
                    //else if (fiyati >= 600001 && fiyati < 850001)
                    //{
                    //    Sinif = "C";
                    //}
                    //else if (fiyati >= 850001 && fiyati < 1100001)
                    //{
                    //    Sinif = "B";
                    //}
                    //else if (fiyati > 1100000)
                    //{
                    //    Sinif = "A";
                    //}
                    //data.AracSinifi = Sinif;
                    //data.BuyukSehirMi = _TeklifYapayZekaService.buyukSehirMi(data.AtananIl);
                    //var ilNufus_Yogunluk = _TeklifYapayZekaService.ilNufusVeYogunluk(data.AtananIl);
                    //var ilceNufus_Yogunluk_ilIleUzaklik = _TeklifYapayZekaService.ilceNufusVeYogunluk(data.AtananIl, data.AtananIlce);
                    //data.IlNufusu = ilNufus_Yogunluk.Item1;
                    //data.IlceNufusu = ilceNufus_Yogunluk_ilIleUzaklik.Item1.ToString();
                    //data.AdresMahalleMi = "0";
                    //data.AdresKoyMu = "0";
                    //data.MahalleKoyNufusu = "0";
                    //data.IlNufusYogunlugu = ilNufus_Yogunluk.Item2;
                    //data.IlceNufusYogunlugu = ilceNufus_Yogunluk_ilIleUzaklik.Item2.ToString();
                    ////data.IlceIleUzaklik = ilceNufus_Yogunluk_ilIleUzaklik.Item3.ToString();
                    //data.IlceIleUzaklik = "0";
                    //data.MahalleIleUzaklik = "0";
                    //data.Kategori = "0";
                    //#endregion

                    //TeklifYapayZekaModel result = _TeklifYapayZekaService.callAIService(data);
                    //Boolean trafik = false;
                    //Boolean kasko = false;
                    //Boolean dask = false;
                    //Boolean yangin = false;
                    //int enBuyukSonuc = EnBuyukOranliClass(result);
                    //switch (enBuyukSonuc)
                    //{
                    //    case 0:
                    //        trafik = false;
                    //        kasko = false;
                    //        dask = false;
                    //        yangin = false;
                    //        break;
                    //    case 1:
                    //        trafik = false;
                    //        kasko = false;
                    //        dask = false;
                    //        yangin = true;
                    //        break;
                    //    case 2:
                    //        trafik = false;
                    //        kasko = false;
                    //        dask = true;
                    //        yangin = false;
                    //        break;
                    //    case 3:
                    //        trafik = false;
                    //        kasko = false;
                    //        dask = true;
                    //        yangin = true;
                    //        break;
                    //    case 4:
                    //        trafik = false;
                    //        kasko = true;
                    //        dask = false;
                    //        yangin = false;
                    //        break;
                    //    case 5:
                    //        trafik = false;
                    //        kasko = true;
                    //        dask = false;
                    //        yangin = true;
                    //        break;
                    //    case 6:
                    //        trafik = false;
                    //        kasko = true;
                    //        dask = true;
                    //        yangin = false;
                    //        break;
                    //    case 7:
                    //        trafik = false;
                    //        kasko = true;
                    //        dask = true;
                    //        yangin = true;
                    //        break;
                    //    case 8:
                    //        trafik = true;
                    //        kasko = false;
                    //        dask = false;
                    //        yangin = false;
                    //        break;
                    //    case 9:
                    //        trafik = true;
                    //        kasko = false;
                    //        dask = false;
                    //        yangin = true;
                    //        break;
                    //    case 10:
                    //        trafik = true;
                    //        kasko = false;
                    //        dask = true;
                    //        yangin = false;
                    //        break;
                    //    case 11:
                    //        trafik = true;
                    //        kasko = false;
                    //        dask = true;
                    //        yangin = true;
                    //        break;
                    //    case 12:
                    //        trafik = true;
                    //        kasko = true;
                    //        dask = false;
                    //        yangin = false;
                    //        break;
                    //    case 13:
                    //        trafik = true;
                    //        kasko = true;
                    //        dask = false;
                    //        yangin = true;
                    //        break;
                    //    case 14:
                    //        trafik = true;
                    //        kasko = true;
                    //        dask = true;
                    //        yangin = false;
                    //        break;
                    //}

                    //#endregion
                    //return Json(new { id = isDurum.IsId, g = isDurum.Guid, trafikTavsiyeEt = trafik, kaskoTavsiyeEt = kasko, daskTavsiyeEt = dask, yanginTavsiyeEt = yangin });
                    return Json(new { id = isDurum.IsId, g = isDurum.Guid });
                }
                else
                {
                    _LogService.Info("ModelState is not valid");
                }
                #endregion

                #region Hata Log
                StringBuilder sb = new StringBuilder();
                foreach (var key in ModelState.Keys)
                {
                    ModelState state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.AppendFormat("key {0}", key);
                    }
                }

                if (sb.Length > 0)
                {
                    return Json(new { id = 0, hata = "Validasyon başarısız. Hatalı alanlar : " + sb.ToString() });
                }
                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }
        public string yasAraligi(int yas)
        {
            string yasKategorisi = "YOK";
            if (yas >= 0 && yas <= 4)
            {
                yasKategorisi = "[0-4]";
            }
            else if (yas >= 5 && yas <= 9)
            {
                yasKategorisi = "[5-9]";
            }
            else if (yas >= 10 && yas <= 14)
            {
                yasKategorisi = "[10-14]";
            }
            else if (yas >= 15 && yas <= 19)
            {
                yasKategorisi = "[15-19]";
            }
            else if (yas >= 20 && yas <= 24)
            {
                yasKategorisi = "[20-24]";
            }
            else if (yas >= 25 && yas <= 29)
            {
                yasKategorisi = "[25-29]";
            }
            else if (yas >= 30 && yas <= 34)
            {
                yasKategorisi = "[30-34]";
            }
            else if (yas >= 35 && yas <= 39)
            {
                yasKategorisi = "[35-39]";
            }
            else if (yas >= 40 && yas <= 44)
            {
                yasKategorisi = "[40-44]";
            }
            else if (yas >= 45 && yas <= 49)
            {
                yasKategorisi = "[45-49]";
            }
            else if (yas >= 50 && yas <= 54)
            {
                yasKategorisi = "[50-54]";
            }
            else if (yas >= 55 && yas <= 59)
            {
                yasKategorisi = "[55-59]";
            }
            else if (yas >= 60 && yas <= 64)
            {
                yasKategorisi = "[60-64]";
            }
            else if (yas >= 65 && yas <= 69)
            {
                yasKategorisi = "[65-69]";
            }
            else if (yas >= 70 && yas <= 74)
            {
                yasKategorisi = "[70-74]";
            }
            else if (yas >= 75 && yas <= 79)
            {
                yasKategorisi = "[75-79]";
            }
            else if (yas >= 80 && yas <= 84)
            {
                yasKategorisi = "[80-84]";
            }
            else if (yas >= 85 && yas <= 89)
            {
                yasKategorisi = "[85-89]";
            }
            else if (yas >= 90 && yas <= 94)
            {
                yasKategorisi = "[90-94]";
            }
            else if (yas >= 95)
            {
                yasKategorisi = "[94 - <]";
            }

            return yasKategorisi;
        }
        public int EnBuyukOranliClass(TeklifYapayZekaModel result)
        {
            float enBuyukSonuc = 0;
            int enYuksekOlasilikliClass = 0;
            if (result != null)
            {
                //if (enBuyukSonuc < result.score1)
                //{
                //    enBuyukSonuc = result.score1;
                //    enYuksekOlasilikliClass = 0;
                //}
                if (enBuyukSonuc < result.score2)
                {
                    enBuyukSonuc = result.score2;
                    enYuksekOlasilikliClass = 1;
                }
                if (enBuyukSonuc < result.score3)
                {
                    enBuyukSonuc = result.score3;
                    enYuksekOlasilikliClass = 2;
                }
                if (enBuyukSonuc < result.score4)
                {
                    enBuyukSonuc = result.score4;
                    enYuksekOlasilikliClass = 3;
                }
                if (enBuyukSonuc < result.score5)
                {
                    enBuyukSonuc = result.score5;
                    enYuksekOlasilikliClass = 4;
                }
                if (enBuyukSonuc < result.score6)
                {
                    enBuyukSonuc = result.score6;
                    enYuksekOlasilikliClass = 5;
                }
                if (enBuyukSonuc < result.score7)
                {
                    enBuyukSonuc = result.score7;
                    enYuksekOlasilikliClass = 6;
                }
                if (enBuyukSonuc < result.score8)
                {
                    enBuyukSonuc = result.score8;
                    enYuksekOlasilikliClass = 7;
                }
                if (enBuyukSonuc < result.score9)
                {
                    enBuyukSonuc = result.score9;
                    enYuksekOlasilikliClass = 8;
                }
                if (enBuyukSonuc < result.score10)
                {
                    enBuyukSonuc = result.score10;
                    enYuksekOlasilikliClass = 9;
                }
                if (enBuyukSonuc < result.score11)
                {
                    enBuyukSonuc = result.score11;
                    enYuksekOlasilikliClass = 10;
                }
                if (enBuyukSonuc < result.score12)
                {
                    enBuyukSonuc = result.score12;
                    enYuksekOlasilikliClass = 11;
                }
                if (enBuyukSonuc < result.score13)
                {
                    enBuyukSonuc = result.score13;
                    enYuksekOlasilikliClass = 12;
                }
                if (enBuyukSonuc < result.score14)
                {
                    enBuyukSonuc = result.score14;
                    enYuksekOlasilikliClass = 13;
                }
                if (enBuyukSonuc < result.score15)
                {
                    enBuyukSonuc = result.score15;
                    enYuksekOlasilikliClass = 14;
                }
            }
            return enYuksekOlasilikliClass;
        }
        [HttpPost]
        public ActionResult Fiyatlar(int teklifId)
        {
            TrafikTeklif trafikTeklif = new TrafikTeklif(teklifId);

            TeklifFiyatModel model = TrafikFiyat(trafikTeklif);

            return PartialView("_TrafikFiyat", model);
        }

        [AjaxException]
        public ActionResult PlakaSorgula(PlakaSorgulaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    if (musteri != null)
                    {
                        PlakaSorgu plakaSorgu = null;
                        if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre || _AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre_DisAcente)
                        {
                            plakaSorgu = this.MAPFREPlakaSorgula(model, musteri);
                        }
                        else
                        {
                            model.PlakaNo = model.PlakaNo.ToUpperInvariant();
                            SBMApi sbmApi = new SBMApi("78.189.231.24", 1244);

                            var result = sbmApi.getLastTrafficPolicyDetail(musteri.KimlikNo, Convert.ToInt32(model.PlakaKodu), model.PlakaNo);
                            if (result != null)
                            {
                                plakaSorgu = new PlakaSorgu();

                                plakaSorgu.HasarsizlikInd = result.HasarsizlikIndirimi;
                                plakaSorgu.HasarsizlikSur = result.HasarlılıkSurprim;
                                plakaSorgu.TramerBelgeNumarasi = result.TramerBelgeNo;
                                plakaSorgu.TramerBelgeTarihi = result.TramerBelgeTarihi;
                                plakaSorgu.UygulanmisHasarsizlikKademe = result.TarifeBasamakKodu;
                                plakaSorgu.AracMotorNo = result.MotorNo;
                                plakaSorgu.AracSasiNo = result.SasiNo;
                                plakaSorgu.TrafigeCikisTarihi = result.TrafigeCikisTarihi;

                                //plakaSorgu.TescilSeri = result.no;
                                plakaSorgu.AracKullanimSekli = result.KullanimSekli;
                                plakaSorgu.AracModelYili = result.ModelYili;

                                plakaSorgu.AracMarkaKodu = result.AracMarkasi;
                                plakaSorgu.AracTipKodu = result.AracTipi;
                                plakaSorgu.AracTescilTarih = result.TescilTarihi;

                                plakaSorgu.EskiPoliceSigortaSirkedKodu = result.OncekiSigortaSirketi;
                                plakaSorgu.EskiPoliceAcenteKod = result.OncekiAcenteNo;
                                plakaSorgu.EskiPoliceNo = result.OncekiPoliceNo;
                                plakaSorgu.EskiPoliceYenilemeNo = result.OncekiYenilemeNo;


                                plakaSorgu.MotorGucu = result.MotorGucu;
                                plakaSorgu.SilindirHacmi = result.SilindirHacmi;
                                plakaSorgu.Renk = result.Renk;
                                plakaSorgu.ImalatYeri = !String.IsNullOrEmpty(result.ImalatYeri) ? Convert.ToByte(result.ImalatYeri) : Convert.ToByte(0);

                                string koltukSayisi = "";
                                foreach (var item in result.YolcuSayisi)
                                {
                                    if (Char.IsDigit(item))
                                        koltukSayisi += item;

                                }
                                plakaSorgu.AracKoltukSayisi = koltukSayisi;

                                int modelYili = Convert.ToInt32(result.ModelYili);
                                // plakaSorgu.AracKullanimSekli = "2";

                                short kullanimSekli = Convert.ToInt16(plakaSorgu.AracKullanimSekli);
                                List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekli);
                                plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
                                plakaSorgu.AracKullanimTarzi = result.AracTarifeGrupKodu + "-10";

                                List<AracMarka> markalar = _AracService.GetAracMarkaList(result.AracTarifeGrupKodu);
                                plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

                                List<AracTip> tipler = _AracService.GetAracTipList(result.AracTarifeGrupKodu, result.AracMarkasi, modelYili);
                                plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

                                //if (!String.IsNullOrEmpty(result.TescilTarihi))
                                //    plakaSorgu.AracTescilTarih = HDIMessage.ToDateTime(result.seri).ToString("dd.MM.yyyy");

                                if (!String.IsNullOrEmpty(result.BitisTarihi))
                                {
                                    DateTime PoliceBitis = Convert.ToDateTime(result.BitisTarihi);

                                    if (PoliceBitis < TurkeyDateTime.Today)
                                        plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                                    else
                                        plakaSorgu.YeniPoliceBaslangicTarih = PoliceBitis.ToString("dd.MM.yyyy");
                                }

                            }
                            else { 
                                throw new Exception("Bu plaka ile kimlik numarasına ait veri bulunamadı!");
                            }
                        }


                        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    }
                }

                throw new Exception("Plaka sorgulama servisi çalıştırılırken hata oluştu.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult EgmSorgula(EgmSorgulaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                    EgmSorguResponse response = mapfreSorgu.EgmSorgu(_AktifKullaniciService.TVMKodu, model.PlakaKodu, model.PlakaNo, model.AracRuhsatSeriNo, model.AracRuhsatNo, model.AsbisNo);

                    if (response == null)
                    {
                        throw new Exception("Egm sorgusu yapılamadı.");
                    }
                    if (!String.IsNullOrEmpty(response.hata))
                    {
                        throw new Exception(response.hata);
                    }
                    if (response.aracBilgi == null || response.aracTescilBilgileri == null)
                    {
                        throw new Exception("Egm sorgusu yapılamadı.");
                    }

                    PlakaSorgu plakaSorgu = new PlakaSorgu();
                    //plakaSorgu.AracMarkaKodu = bilgi.marka;
                    //plakaSorgu.AracTipKodu = bilgi.model;
                    plakaSorgu.AracModelYili = response.aracBilgi.modelYili;
                    plakaSorgu.AracMotorNo = response.aracBilgi.motorNo;
                    plakaSorgu.AracSasiNo = response.aracBilgi.sasiNo;
                    plakaSorgu.AracKoltukSayisi = response.aracBilgi.koltukSayisi;
                    plakaSorgu.Renk = response.aracBilgi.renk;
                    plakaSorgu.SilindirHacmi = response.aracBilgi.silindirHacmi;


                    long time = Convert.ToInt64(response.aracTescilBilgileri.tescilTarihi.time);
                    plakaSorgu.AracTescilTarih = MapfreSorguResponse.FromJavaTime(time).ToString("dd.MM.yyyy");

                    return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                }
                throw new Exception("Plaka sorgulama servisi çalıştırılırken hata oluştu.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult DekontPDF(int id)
        {
            bool success = true;
            string url = String.Empty;
            ITeklif teklif = _TeklifService.GetTeklif(id);

            try
            {
                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDekont))
                {
                    IMAPFRETrafik urun = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFRETrafik;
                    urun.DekontPDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDekont))
                {
                    throw new Exception("Bilgilendirme pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.BilgilendirmePDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFDekont;
            return Json(new { Success = success, PDFUrl = url });
        }

        private PlakaSorgu MAPFREPlakaSorgula(PlakaSorgulaModel model, MusteriGenelBilgiler musteri)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            PoliceSorguTrafikResponse response = mapfreSorgu.PoliceSorguTrafik(_AktifKullaniciService.TVMKodu, musteri.KimlikNo, model.PlakaKodu, model.PlakaNo);

            if (response == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }
            if (!String.IsNullOrEmpty(response.hata))
            {
                throw new Exception(response.hata);
            }
            if (response.trafikHasar == null || response.trafikHasar.hasarBilgi == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();

            MapfreToHdiPlakaSorgu mapfreToHdi = _AracService.KarsilastirmaliPlakaSorgu_FillMapfre(response);

            if (mapfreToHdi != null)
            {
                plakaSorgu.Tarzlar = new SelectList(mapfreToHdi.TarzList, "Kod", "KullanimTarzi").ListWithOptionLabel();
                plakaSorgu.Markalar = new SelectList(mapfreToHdi.Markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                plakaSorgu.Tipler = new SelectList(mapfreToHdi.Tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                plakaSorgu.AracKullanimSekli = mapfreToHdi.AracKullanimSekli;
                plakaSorgu.AracKullanimTarzi = mapfreToHdi.AracKullanimTarzi;
                plakaSorgu.AracMarkaKodu = mapfreToHdi.AracMarkaKodu;

                plakaSorgu.AracTipKodu = mapfreToHdi.AracTipKodu;
                plakaSorgu.AracModelYili = mapfreToHdi.AracModelYili;
                plakaSorgu.AracMotorNo = mapfreToHdi.AracMotorNo;
                plakaSorgu.AracSasiNo = mapfreToHdi.AracSasiNo;
                plakaSorgu.AracTescilTarih = mapfreToHdi.AracTescilTarih;
                plakaSorgu.EskiPoliceSigortaSirkedKodu = mapfreToHdi.EskiPoliceSigortaSirkedKodu;
                plakaSorgu.EskiPoliceAcenteKod = mapfreToHdi.EskiPoliceAcenteKod;
                plakaSorgu.EskiPoliceNo = mapfreToHdi.EskiPoliceNo;
                plakaSorgu.EskiPoliceYenilemeNo = mapfreToHdi.EskiPoliceYenilemeNo;
                plakaSorgu.YeniPoliceBaslangicTarih = mapfreToHdi.YeniPoliceBaslangicTarih;
                plakaSorgu.AracKoltukSayisi = response.trafikHasar.hasarBilgi.Select(s => s.yolcuKapasitesi).FirstOrDefault().Substring(0, 1);
                plakaSorgu.MotorGucu = mapfreToHdi.motorGucu;
                plakaSorgu.SilindirHacmi = mapfreToHdi.silindirHacmi;
                plakaSorgu.TramerBelgeNumarasi = mapfreToHdi.belgeNo;
                plakaSorgu.TramerBelgeTarihi = mapfreToHdi.belgeTarihi;
                plakaSorgu.HasarsizlikKademe = mapfreToHdi.HasarsizlikKademe;
                plakaSorgu.HasarsizlikInd = mapfreToHdi.HasarsizlikInd;
                plakaSorgu.HasarsizlikSur = mapfreToHdi.HasarsizlikSur;
                plakaSorgu.UygulanmisHasarsizlikKademe = mapfreToHdi.UygulanmisHasarsizlikKademe;
                plakaSorgu.HasarsizlikHata = "";
            }

            //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz    

            var kTipModel = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Trafik);
            plakaSorgu.AnadoluKullanimTipleri = new SelectList(kTipModel.list, "key", "value").ListWithOptionLabel();
            plakaSorgu.AnadoluHata = kTipModel.hata;
            plakaSorgu.AnadoluMarkaKodu = kTipModel.anadoluMarkaKodu;
            return plakaSorgu;
        }

        private PlakaSorgu HDIPlakaSorgula(PlakaSorgulaModel model, MusteriGenelBilgiler musteri)
        {
            IHDITrafik HDITrafik = DependencyResolver.Current.GetService<IHDITrafik>();
            model.PlakaNo = model.PlakaNo.ToUpperInvariant();
            HDIPlakaSorgulamaResponse plakaSorguResponse = HDITrafik.PlakaSorgula(model.PlakaKodu, model.PlakaNo, musteri.MusteriTipKodu, musteri.KimlikNo);

            if (!String.IsNullOrEmpty(plakaSorguResponse.Durum) && plakaSorguResponse.Durum != "0")
            {
                throw new Exception(String.Format("{0} - {1}", plakaSorguResponse.Durum, plakaSorguResponse.DurumMesaj));
            }

            HDIPlakaSorgulamaResponseDetails details = plakaSorguResponse.HDISIGORTA;
            if (details != null && !String.IsNullOrEmpty(details.Durum) && details.Durum != "0" && !String.IsNullOrEmpty(details.Mesaj))
            {
                throw new Exception(String.Format("{0} - {1}", details.Durum, details.Mesaj));
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracKullanimSekli = details.AracKullanimSekli;
            plakaSorgu.AracMarkaKodu = details.AracMarkaKodu;
            plakaSorgu.AracTipKodu = details.AracTipKodu;
            plakaSorgu.AracModelYili = details.AracModelYili;
            plakaSorgu.AracMotorNo = details.AracMotorNo;
            plakaSorgu.AracSasiNo = details.AracSasiNo;
            plakaSorgu.AracTescilTarih = details.AracTescilTarih;
            plakaSorgu.EskiPoliceSigortaSirkedKodu = details.EskiPoliceSigortaSirkedKodu;
            plakaSorgu.EskiPoliceAcenteKod = details.EskiPoliceAcenteKod;
            plakaSorgu.EskiPoliceNo = details.EskiPoliceNo;
            plakaSorgu.EskiPoliceYenilemeNo = details.EskiPoliceYenilemeNo;
            plakaSorgu.TasiyiciSigSirkerKod = details.TasiyiciSigSirkerKod;
            plakaSorgu.TasiyiciSigAcenteNo = details.TasiyiciSigAcenteNo;
            plakaSorgu.TasiyiciSigPoliceNo = details.TasiyiciSigPoliceNo;
            plakaSorgu.TasiyiciSigYenilemeNo = details.TasiyiciSigYenilemeNoField;

            plakaSorgu.MotorGucu = details.AracMotorGucu;
            plakaSorgu.SilindirHacmi = details.AracSilindir;
            plakaSorgu.Renk = details.AracRenk;
            plakaSorgu.TramerBelgeNumarasi = details.TramerBelgeNo;
            plakaSorgu.TramerBelgeTarihi = HDIMessage.ToDateTime(details.TramerBelgeTarih).ToString("dd.MM.yyyy");
            plakaSorgu.ImalatYeri = !String.IsNullOrEmpty(details.AracImalatYeri) ? Convert.ToByte(details.AracImalatYeri) : Convert.ToByte(0);

            string koltukSayisi = "";
            foreach (var item in details.AracKoltukSayisi)
            {
                if (Char.IsDigit(item))
                    koltukSayisi += item;

            }
            plakaSorgu.AracKoltukSayisi = koltukSayisi;

            int modelYili = Convert.ToInt32(details.AracModelYili);
            // plakaSorgu.AracKullanimSekli = "2";

            short kullanimSekli = Convert.ToInt16(plakaSorgu.AracKullanimSekli);
            List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekli);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
            plakaSorgu.AracKullanimTarzi = details.AracTarifeGrupKodu + "-10";

            List<AracMarka> markalar = _AracService.GetAracMarkaList(details.AracTarifeGrupKodu);
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(details.AracTarifeGrupKodu, details.AracMarkaKodu, modelYili);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            if (!String.IsNullOrEmpty(details.AracTescilTarih))
                plakaSorgu.AracTescilTarih = HDIMessage.ToDateTime(details.AracTescilTarih).ToString("dd.MM.yyyy");

            if (!String.IsNullOrEmpty(details.PoliceBitisTarih))
            {
                DateTime hdiPoliceBitis = HDIMessage.ToDateTime(details.PoliceBitisTarih);

                if (hdiPoliceBitis < TurkeyDateTime.Today)
                    plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                else
                    plakaSorgu.YeniPoliceBaslangicTarih = hdiPoliceBitis.ToString("dd.MM.yyyy");
            }

            return plakaSorgu;
        }

        private TeklifFiyatModel TrafikFiyat(TrafikTeklif trafikTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = trafikTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = trafikTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            TeklifArac arac = trafikTeklif.Teklif.Arac;
            model.PlakaNo = String.Format("{0} {1}", arac.PlakaKodu, arac.PlakaNo);

            AracMarka marka = _AracService.GetAracMarka(arac.Marka);
            if (marka != null)
                model.MarkaAdi = marka.MarkaAdi;

            AracTip tip = _AracService.GetAracTip(arac.Marka, arac.AracinTipi);
            if (tip != null)
                model.TipAdi = tip.TipAdi;

            if (!String.IsNullOrEmpty(arac.KullanimTarzi))
            {
                string[] parts = arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    AracKullanimTarzi tarz = _AracService.GetAracKullanimTarzi(parts[0], parts[1]);
                    if (tarz != null)
                        model.KullanimTarziAdi = tarz.KullanimTarzi;
                }
            }

            model.PoliceBaslangicTarihi = trafikTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = trafikTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = trafikTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = trafikTeklif.GetIsDurum();
            List<IsDurumDetay> detaylar = null;
            if (durum != null)
            {
                detaylar = durum.IsDurumDetays.ToList<IsDurumDetay>();
            }

            foreach (var item in tumListe)
            {
                TeklifFiyatDetayModel fiyatModel = base.TeklifFiyat(item, detaylar);
                model.Fiyatlar.Add(fiyatModel);
            }

            return model;
        }

        //Plaka Sorgula butonuna tıklandığında çağırılıyor
        public AnadoluKullanimTipModels KullanimTipiSorgulaAnadolu(string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu, int TVMKodu, string Trafik)
        {
            AnadoluKullanimTipModels model = new AnadoluKullanimTipModels();
            ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
            IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
            string TRAMER_TARIFE_KODU = String.Empty;
            string[] parts = AracKullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                string kullanimTarziKodu = parts[0];
                string kod2 = parts[1];
                CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                              f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                              f.Kod2 == kod2)
                                                                                              .SingleOrDefault<CR_KullanimTarzi>();

                if (kullanimTarzi != null)
                {
                    TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                }
            }

            var AnadoluKullanimTipleri = _trafik.AracKullanimTipiList(AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, TVMKodu, Trafik);
            model.list = AnadoluKullanimTipleri.list;
            model.hata = AnadoluKullanimTipleri.hata;
            model.anadoluMarkaKodu = AnadoluKullanimTipleri.anadoluMarkaKodu;
            return model;
        }

        //İleri butonuna tıklandığında çağırılıyor
        [AjaxException]
        public ActionResult KullanimTipListAnadolu(string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu)
        {
            try
            {
                AnadoluKullanimResultView model = new AnadoluKullanimResultView();
                ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
                IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
                IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);

                string TRAMER_TARIFE_KODU = String.Empty;
                string[] parts = AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }

                var kTipModel = _trafik.AracKullanimTipiList(AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Trafik);
                model.list = new SelectList(kTipModel.list, "key", "value").ListWithOptionLabel();
                model.hata = kTipModel.hata;
                model.anadoluMarkaKodu = kTipModel.anadoluMarkaKodu;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult KullanimSekliSorgulaAnadolu(string AnadoluMarkaKodu, string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu, string KullanimTipi)
        {
            try
            {
                AnadoluKullanimResultView model = new AnadoluKullanimResultView();
                ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
                IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
                string TRAMER_TARIFE_KODU = String.Empty;
                string[] parts = AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }

                var AnadoluKullanimSekilleri = _trafik.AracKullanimSekliList(AnadoluMarkaKodu, AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, KullanimTipi, _AktifKullaniciService.TVMKodu);
                model.list = new SelectList(AnadoluKullanimSekilleri.list, "key", "value").ListWithOptionLabel();
                model.hata = AnadoluKullanimSekilleri.hata;
                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult EskiPoliceSorgula(string kimlikNo, string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo)
        {
            try
            {
                PlakaSorgu plakaSorgu = MAPFREEskiPoliceSorgula(sigortaSirketi, acenteNo, policeNo, yenilemeNo);

                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();

                if (sigortaSirketi == "050" || sigortaSirketi == "50")
                {
                    OncekiTescilResponse tescil = sorguService.OncekiTescilSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, "410");
                    if (tescil != null && String.IsNullOrEmpty(tescil.hata) && tescil.COD_ARAC_RUHSAT_SERI != "*")
                    {
                        plakaSorgu.TescilSeri = tescil.COD_ARAC_RUHSAT_SERI;
                        plakaSorgu.TescilSeriNo = tescil.COD_ARAC_RUHSAT_SERI_NO;
                    }
                }
                var kTipModel = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Trafik);
                plakaSorgu.AnadoluKullanimTipleri = new SelectList(kTipModel.list, "key", "value").ListWithOptionLabel();
                plakaSorgu.AnadoluHata = kTipModel.hata;
                plakaSorgu.AnadoluMarkaKodu = kTipModel.anadoluMarkaKodu;

                return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("TrafikKontroller.EskiPoliceSorgula", ex);
                throw;
            }
        }
        private PlakaSorgu MAPFREEskiPoliceSorgula(string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            EskiTrafikBilgiSorguResponse response = mapfreSorgu.EskiPoliceSorguTrafik(policeNo, acenteNo, sigortaSirketi, yenilemeNo);

            if (response == null)
            {
                throw new Exception("Poliçe bilgileri sorgulanamadı.");
            }
            if (!String.IsNullOrEmpty(response.hata))
            {
                throw new Exception(response.hata);
            }
            if (response.aracTemelBilgileri == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracMarkaKodu = response.aracTemelBilgileri.marka.kod;
            plakaSorgu.AracTipKodu = response.aracTemelBilgileri.tip.kod.TrimStart('0');
            plakaSorgu.AracModelYili = response.aracTemelBilgileri.modelYili;
            plakaSorgu.AracMotorNo = response.aracTemelBilgileri.motorNo;
            plakaSorgu.AracSasiNo = response.aracTemelBilgileri.sasiNo;

            if (response.aracTemelBilgileri.plaka != null)
            {
                if (response.aracTemelBilgileri.plaka.ilKodu.Length == 3)
                    plakaSorgu.PlakaKodu = response.aracTemelBilgileri.plaka.ilKodu.Substring(1, 2);
                else
                    plakaSorgu.PlakaKodu = response.aracTemelBilgileri.plaka.ilKodu;

                plakaSorgu.PlakaNo = response.aracTemelBilgileri.plaka.no;
            }

            long time = Convert.ToInt64(response.aracTemelBilgileri.tescilTarihi.time);
            plakaSorgu.AracTescilTarih = MapfreSorguResponse.FromJavaTime(time).ToString("dd.MM.yyyy");

            plakaSorgu.EskiPoliceSigortaSirkedKodu = sigortaSirketi;
            plakaSorgu.EskiPoliceAcenteKod = acenteNo;
            plakaSorgu.EskiPoliceNo = policeNo;
            plakaSorgu.EskiPoliceYenilemeNo = yenilemeNo;


            // Fesih Tarihi
            plakaSorgu.FesihTarih = String.Empty;
            if (response.fesihTarihi != null && response.fesihTarihi.time != null)
            {
                try
                {
                    plakaSorgu.FesihTarih = MapfreSorguResponse.FromJavaTime(Convert.ToInt64(response.fesihTarihi.time)).ToString("dd.MM.yyyy");
                }
                catch (Exception)
                { }
            }


            if (response.tarihBilgileri != null && response.tarihBilgileri.bitisTarihi != null &&
                !String.IsNullOrEmpty(response.tarihBilgileri.bitisTarihi.time) &&
                String.IsNullOrEmpty(plakaSorgu.FesihTarih))
            {
                time = Convert.ToInt64(response.tarihBilgileri.bitisTarihi.time);
                DateTime policeBitis = MapfreSorguResponse.FromJavaTime(time);

                if (policeBitis < TurkeyDateTime.Today)
                    plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                else
                {
                    plakaSorgu.YeniPoliceBaslangicTarih = policeBitis.ToString("dd.MM.yyyy");
                }
            }
            else
            {
                plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
            }

            if (response.aracTemelBilgileri.yolcuKapasitesi.Contains(".0"))
            {
                plakaSorgu.AracKoltukSayisi = response.aracTemelBilgileri.yolcuKapasitesi.Replace(".0", "");
            }
            else
            {
                plakaSorgu.AracKoltukSayisi = response.aracTemelBilgileri.yolcuKapasitesi;
            }
            int koltukSayisi = 0;
            int.TryParse(plakaSorgu.AracKoltukSayisi, out koltukSayisi);

            int modelYili = int.Parse(plakaSorgu.AracModelYili);
            string kullanimSekli = "111";
            plakaSorgu.AracKullanimSekli = "2";
            plakaSorgu.AracKullanimTarzi = "111-10";

            AracTip tip = _AracService.GetAracTip(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu);
            if (tip != null)
            {
                kullanimSekli = tip.KullanimSekli1;
                plakaSorgu.AracKullanimTarzi = tip.KullanimSekli1 + "-10";
            }


            #region Arac Tarife Grup Kodu

            ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
            IAracContext _AracContext = DependencyResolver.Current.GetService<IAracContext>();
            if (!String.IsNullOrEmpty(response.aracTemelBilgileri.aracTarifeGrupKod))
            {
                CR_AracGrup aracGrup = _CRContext.CR_AracGrupRepository.Filter(s => s.TarifeKodu == response.aracTemelBilgileri.aracTarifeGrupKod &&
                                                                                    s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();

                if (aracGrup != null)
                {
                    AracKullanimTarzi tarzi = _AracContext.AracKullanimTarziRepository.Filter(s => s.Kod2 == aracGrup.Kod2 &&
                                                                                                   s.KullanimTarziKodu == aracGrup.KullanimTarziKodu)
                                                                                      .FirstOrDefault();
                    if (tarzi != null)
                    {
                        if (tarzi.KullanimSekliKodu.HasValue)
                        {
                            plakaSorgu.AracKullanimSekli = tarzi.KullanimSekliKodu.ToString();
                            plakaSorgu.AracKullanimTarzi = String.Format("{0}-{1}", tarzi.KullanimTarziKodu, tarzi.Kod2);
                        }
                    }
                }
            }

            #endregion

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetAracGrupKodlari(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracMarka> markalar = _AracService.GetAracMarkaList();
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(plakaSorgu.AracMarkaKodu);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);
            if (aracModel != null)
                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;


            return plakaSorgu;
        }

        public class AnadoluKullanimTipModels
        {
            public List<ListModel> list { get; set; }
            public string hata { get; set; }
            public string anadoluMarkaKodu { get; set; }
        }

        public class AnadoluKullanimResultView
        {
            public List<SelectListItem> list { get; set; }
            public string hata { get; set; }
            public string anadoluMarkaKodu { get; set; }
        }

        //[AjaxException]
        //public ActionResult SBMSorgula(string plakaIlKod,string plakaNo,string kimlikNo)
        //{
        //    SBMSorgulaResultView model = new SBMSorgulaResultView();
        //    model.plakaIlKod = plakaIlKod;
        //    model.plakaNo = plakaNo;
        //    model.kimlikNo = kimlikNo;
        //    model.kimlikNo = kimlikNo;
        //    model.URL = "/Teklif/Trafik/SBMTrafikPoliceSorgula";
        //    return Json(model);
        //}
        //[AjaxException]
        //public ActionResult SBMTrafikPoliceSorgula()
        //{
        //    return View();
        //}
        //public class SBMSorgulaResultView
        //{
        //    public string plakaIlKod { get; set; }
        //    public string plakaNo { get; set; }
        //    public string kimlikNo { get; set; }
        //    public string URL { get; set; }
        //}


        //Mapfre Genel Sigorta hasarsızlık sorgulama metodu
        public HasarsizlikReturnModel HasarsizlikSorgula(string kimlikNo, string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo, string bransKodu)
        {
            try
            {
                HasarsizlikReturnModel model = new HasarsizlikReturnModel();
                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                HasarsizlikResponse hasarsizlik = sorguService.HasarsizlikSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, bransKodu);
                if (hasarsizlik != null)
                {
                    model.HasarsizlikInd = hasarsizlik.hasarsizlik_ind;
                    model.HasarsizlikSur = hasarsizlik.hasar_srp;
                    model.HasarsizlikKademe = hasarsizlik.uygulanacak_kademe;
                    model.hata = "";
                    return model;
                }
                model.hata = "Hasarsızlık bilgileri alınamadı.";
                return model;
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreKaskoController.HasarsizlikSorgula", ex);
                throw;
            }
        }

        public class HasarsizlikReturnModel
        {
            public string HasarsizlikInd { get; set; }
            public string HasarsizlikSur { get; set; }
            public string HasarsizlikKademe { get; set; }
            public string hata { get; set; }
        }
    }
}

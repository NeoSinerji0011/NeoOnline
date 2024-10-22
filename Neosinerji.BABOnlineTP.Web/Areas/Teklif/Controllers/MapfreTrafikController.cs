using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    // [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.MapfreTrafik)]
    public class MapfreTrafikController : TeklifController
    {
        public MapfreTrafikController(ITVMService tvmService, ITeklifService teklifService, IMusteriService musteriService,
                                IKullaniciService kullaniciService, IAktifKullaniciService aktifKullaniciService, ITanimService tanimService,
                                IUlkeService ulkeService, ICRService crService, IAracService aracService,
                                IUrunService urunService, ITUMService tumService)
            : base(tvmService, teklifService, musteriService,
                    kullaniciService, aktifKullaniciService, tanimService,
                    ulkeService, crService, aracService,
                    urunService, tumService)
        {

        }

        //
        // GET: /Teklif/Trafik/Ekle
        [HttpGet]
        public ActionResult Ekle(int? id)
        {
            MapfreTrafikModel model = EkleModel(id, null);
            return View(model);
        }

        //
        // POST: /Teklif/Trafik/Ekle
        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            MapfreTrafikModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public MapfreTrafikModel EkleModel(int? id, int? teklifId)
        {

            ILogService log = DependencyResolver.Current.GetService<ILogService>();
            log.Visit();

            ITeklif teklif = null;
            MapfreTrafikModel model = new MapfreTrafikModel();
            int? sigortaliMusteriKodu = null;

            //Teklifi hazırlayan
            model.Hazirlayan = new MapfreHazirlayanModel();
            model.Hazirlayan.FarkliAcenteSecebilir = _AktifKullaniciService.MapfreBolge || _AktifKullaniciService.MapfreMerkez;
            model.Hazirlayan.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.Hazirlayan.TVMUnvani = _AktifKullaniciService.TVMUnvani;

            //Kullanıcının partaj numarası belli değilse tvm seçmesi gerekecek
            if (String.IsNullOrEmpty(_AktifKullaniciService.MTKodu))
            {
                model.Hazirlayan.TVMKodu = null;
                model.Hazirlayan.TVMUnvani = null;
            }

            //Sigorta Ettiren / Sigortalı
            model.Musteri = new SigortaliModel();
            model.Musteri.CepTelefonuRequired = false;
            model.Musteri.EMailRequired = false;
            model.Musteri.AcikAdresRequired = true;
            model.Musteri.SigortaliAyni = true;
            model.Musteri.SadeceSigortaliGoster = true;

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

            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1960; yil--)
                yillar.Add(yil);
            model.Arac.Modeller = new SelectList(yillar).ListWithOptionLabel();

            model.Arac.PlakaKodu = "";
            List<SelectListItem> plakaKodListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu").ListWithOptionLabel();
            model.Arac.PlakaKoduListe = new SelectList(plakaKodListe, "Value", "Text");
            model.Arac.TescilIl = "";
            model.Arac.TescilIller = new SelectList(_CRService.GetTescilIlList(), "Key", "Value", model.Arac.TescilIl).ListWithOptionLabel();
            model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value", "").ListWithOptionLabel(); ;
            model.Arac.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.Arac.SigortaSirketleri = this.SigortaSirketleri;

            List<AracKullanimTarziServisModel> kullanimSekilleri = _CRService.GetAracGrupKodlari(TeklifUretimMerkezleri.MAPFRE);
            model.Arac.KullanimSekilleri = new SelectList(kullanimSekilleri, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
            model.Arac.KullanimTarzlari = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<CR_AracGrup> aracGruplar = _CRService.GetAracGruplari(TeklifUretimMerkezleri.MAPFRE);
            List<CR_KullanimTarzi> aracKullanimTarzi = _CRService.GetKullanimTarzlari(TeklifUretimMerkezleri.MAPFRE);
            var grupTarz = from a in aracGruplar
                           join b in aracKullanimTarzi on a.KullanimTarziTarife equals b.TarifeKodu
                           select new { grup = a.KullanimTarziKodu + "-" + a.Kod2, tarz = b.KullanimTarziKodu + "-" + b.Kod2 };

            model.AracGrupTarzListesi = grupTarz.Select(s => new KeyValuePair<string, string>(s.grup, s.tarz)).ToList<KeyValuePair<string, string>>();

            model.Arac.Markalar = new SelectList(_AracService.GetAracMarkaList(), "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
            model.Arac.TipKodu = String.Empty;
            model.Arac.AracTipleri = EmptySelectList.EmptyList();

            if (teklifId.HasValue)
            {
                model.Arac.PlakaKodu = teklif.Arac.PlakaKodu;
                model.Arac.PlakaNo = teklif.Arac.PlakaNo;
                model.Arac.KullanimSekliKodu = teklif.Arac.KullanimSekli;

                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    model.Arac.KullanimTarziKodu = teklif.Arac.KullanimTarzi;
                }

                model.Arac.TescilIl = teklif.Arac.TescilIlKodu;
                model.Arac.TescilIlce = teklif.Arac.TescilIlceKodu;
                model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value").ListWithOptionLabel();

                model.Arac.MarkaKodu = teklif.Arac.Marka;
                model.Arac.Model = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;
                model.Arac.TipKodu = teklif.Arac.AracinTipi;

                List<AracTip> tipler = _AracService.GetAracTipList(model.Arac.MarkaKodu);
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
                model.Arac.HasarsizlikIndirim = teklif.ReadSoru(TrafikSorular.HasarsizlikIndirim, String.Empty);
                model.Arac.HasarSurprim = teklif.ReadSoru(TrafikSorular.HasarSurprim, String.Empty);
                model.Arac.UygulananKademe = teklif.ReadSoru(TrafikSorular.UygulananKademe, String.Empty);
            }

            //Eski poliçe bilgileri
            model.EskiPolice = base.EkleEskiPoliceModel();
            List<KeyValueItem<string, string>> islemTipleri = new List<KeyValueItem<string, string>>();
            islemTipleri.Add(new KeyValueItem<string, string>("1", "1 - Önceki Bilgileri İle Giriş"));
            islemTipleri.Add(new KeyValueItem<string, string>("11", "11 - Önceki Bilgiler (Satış Referansılı Giriş)"));
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
            if (teklifId.HasValue)
            {
                model.Teminat.Asistans = teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
                model.Teminat.BelediyeHalkOtobusu = teklif.ReadSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, false);
            }

            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.MapfreTrafik);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

            model.Odeme = new TeklifOdemeListeModel();
            model.Odeme.Add(OdemePlaniAlternatifKodlari.Pesin, babonline.SinglePayment);

            model.KrediKarti = new MapfreOdemeModel();
            model.KrediKarti.KK_OdemeSekli = 1;
            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="PEŞİN",Value="1"},
                new SelectListItem(){Text="VADELİ",Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = odemeSekilleri;
            model.KrediKarti.KK_OdemeTipi = 1;
            List<SelectListItem> odemeTipleri = new List<SelectListItem>();
            odemeTipleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="NAKİT",Value="1"},
                new SelectListItem(){Text="KREDİ KARTI",Value="2"}
            });
            model.KrediKarti.OdemeTipleri = new SelectList(odemeTipleri, "Value", "Text", "1").ToList();

            model.KrediKarti.TaksitSayisi = 1;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" }});

            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            return model;
        }

        public ActionResult Detay(int id)
        {
            MapfreDetayTrafikModel model = new MapfreDetayTrafikModel();
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
            model.Arac = this.MapfreDetayAracBilgiModel(trafikTeklif.Teklif);

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
            model.Teminat.Asistans = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
            model.Teminat.BelediyeHalkOtobusu = trafikTeklif.Teklif.ReadSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, false);

            model.Fiyat = TrafikFiyat(trafikTeklif);

            model.KrediKarti = new MapfreOdemeModel();
            model.KrediKarti.KK_OdemeSekli = 1;
            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="PEŞİN",Value="1"},
                new SelectListItem(){Text="VADELİ",Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = odemeSekilleri;
            model.KrediKarti.KK_OdemeTipi = 1;
            List<SelectListItem> odemeTipleri = new List<SelectListItem>();
            odemeTipleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="NAKİT",Value="1"},
                new SelectListItem(){Text="KREDİ KARTI",Value="2"}
            });
            model.KrediKarti.OdemeTipleri = new SelectList(odemeTipleri, "Value", "Text", "2").ToList();

            model.KrediKarti.TaksitSayisi = trafikTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" }});

            ITeklif mapfreTeklif = trafikTeklif.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

            model.TUMTeklifNo = mapfreTeklif.GenelBilgiler.TUMTeklifNo;

            model.KrediKarti.KK_TeklifId = mapfreTeklif.GenelBilgiler.TeklifId;
            model.KrediKarti.Tutar = mapfreTeklif.GenelBilgiler.BrutPrim;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            model.Satinalinabilir = String.IsNullOrEmpty(mapfreTeklif.GenelBilgiler.TUMPoliceNo) &&
                                    mapfreTeklif.GenelBilgiler.GecerlilikBitisTarihi >= TurkeyDateTime.Today;

            return View(model);
        }

        [HttpPost]
        public ActionResult Hesapla(MapfreTrafikModel model)
        {
            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (ModelState["Hazirlayan.TVMKodu"] != null)
                    ModelState["Hazirlayan.TVMKodu"].Errors.Clear();
                if (ModelState["Hazirlayan.TVMKullaniciKodu"] != null)
                    ModelState["Hazirlayan.TVMKullaniciKodu"].Errors.Clear();

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

                    if (ModelState["EskiPolice.TramerIslemTipi"] != null)
                        ModelState["EskiPolice.TramerIslemTipi"].Errors.Clear();
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

                if (ModelState["Musteri.SigortaEttiren.CepTelefonu"] != null)
                    ModelState["Musteri.SigortaEttiren.CepTelefonu"].Errors.Clear();
                if (ModelState["Musteri.SigortaEttiren.Email"] != null)
                    ModelState["Musteri.SigortaEttiren.Email"].Errors.Clear();
                if (ModelState["Musteri.Sigortali.CepTelefonu"] != null)
                    ModelState["Musteri.Sigortali.CepTelefonu"].Errors.Clear();
                if (ModelState["Musteri.Sigortali.Email"] != null)
                    ModelState["Musteri.Sigortali.Email"].Errors.Clear();
                if (ModelState["Musteri.SigortaEttiren.VergiDairesi"] != null)
                    ModelState["Musteri.SigortaEttiren.VergiDairesi"].Errors.Clear();

                if (ModelState["Arac.KullanimSekliKodu"] != null)
                    ModelState["Arac.KullanimSekliKodu"].Errors.Clear();
                if (ModelState["Arac.TescilIl"] != null)
                    ModelState["Arac.TescilIl"].Errors.Clear();
                if (ModelState["Arac.TescilIlce"] != null)
                    ModelState["Arac.TescilIlce"].Errors.Clear();
                if (ModelState["Arac.TrafikTescilTarihi"] != null)
                    ModelState["Arac.TrafikTescilTarihi"].Errors.Clear();
                if (ModelState["Arac.TrafigeCikisTarihi"] != null)
                    ModelState["Arac.TrafigeCikisTarihi"].Errors.Clear();

                ModelStateMusteriClear(ModelState, model.Musteri);

                #endregion

                #region Teklif kaydı ve hesaplamanın başlatılması
                if (ModelState.IsValid)
                {
                    int tvmKodu = _AktifKullaniciService.TVMKodu;
                    if ((_AktifKullaniciService.MapfreBolge || _AktifKullaniciService.MapfreMerkez) && model.Hazirlayan.TVMKodu.HasValue)
                    {
                        tvmKodu = model.Hazirlayan.TVMKodu.Value;
                    }

                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.MapfreTrafik,
                                                                tvmKodu,
                                                                _AktifKullaniciService.KullaniciKodu,
                                                                model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

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
                    teklif.Arac.KoltukSayisi = _AracService.GetAracKisiSayisi(teklif.Arac.Marka, teklif.Arac.AracinTipi);
                    teklif.Arac.TrafikTescilTarihi = model.Arac.TrafikTescilTarihi;
                    teklif.Arac.TrafikCikisTarihi = model.Arac.TrafigeCikisTarihi;
                    teklif.Arac.TescilSeriKod = model.Arac.TescilBelgeSeriKod;
                    teklif.Arac.TescilSeriNo = model.Arac.TescilBelgeSeriNo;
                    teklif.Arac.AsbisNo = model.Arac.AsbisNo;
                    teklif.Arac.TescilIlKodu = model.Arac.TescilIl;
                    teklif.Arac.TescilIlceKodu = model.Arac.TescilIlce;

                    // Sorular
                    teklif.AddSoru(TrafikSorular.Police_Baslangic_Tarihi, model.Arac.PoliceBaslangicTarihi);

                    // Fesih Tarihi
                    if (model.Arac.FesihTarihi.HasValue)
                        teklif.AddSoru(TrafikSorular.FesihTarihi, model.Arac.FesihTarihi.Value);


                    //Eski Poliçe
                    teklif.AddSoru(TrafikSorular.Eski_Police_VarYok, model.EskiPolice.EskiPoliceVar);
                    if (model.EskiPolice.EskiPoliceVar)
                    {
                        teklif.AddSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, model.EskiPolice.SigortaSirketiKodu);
                        teklif.AddSoru(TrafikSorular.Eski_Police_Acente_No, model.EskiPolice.AcenteNo);
                        teklif.AddSoru(TrafikSorular.Eski_Police_No, model.EskiPolice.PoliceNo);
                        teklif.AddSoru(TrafikSorular.Eski_Police_Yenileme_No, model.EskiPolice.YenilemeNo);
                        teklif.AddSoru(TrafikSorular.TramerIslemTipi, model.EskiPolice.TramerIslemTipi);

                        // ==== Hasarsızlık Bilgileri //
                        if (!String.IsNullOrEmpty(model.Arac.HasarsizlikIndirim))
                            teklif.AddSoru(TrafikSorular.HasarsizlikIndirim, model.Arac.HasarsizlikIndirim);
                        if (!String.IsNullOrEmpty(model.Arac.HasarSurprim))
                            teklif.AddSoru(TrafikSorular.HasarSurprim, model.Arac.HasarSurprim);
                        if (!String.IsNullOrEmpty(model.Arac.UygulananKademe))
                            teklif.AddSoru(TrafikSorular.UygulananKademe, model.Arac.UygulananKademe);
                    }

                    //Taşıma yetki belgesi ve taşıyıcı sorumluluk
                    teklif.AddSoru(TrafikSorular.Tasima_Yetki_Belgesi_VarYok, model.Tasiyici.YetkiBelgesi);
                    teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, model.Tasiyici.Sorumluluk);
                    if (model.Tasiyici.Sorumluluk)
                    {
                        teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, model.Tasiyici.SigortaSirketiKodu);
                        teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, model.Tasiyici.AcenteNo);
                        teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, model.Tasiyici.PoliceNo);
                        teklif.AddSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, model.Tasiyici.YenilemeNo);
                    }

                    //Teminatlar
                    if (model.Teminat != null)
                    {
                        teklif.AddSoru(TrafikSorular.Teminat_Asistans, model.Teminat.Asistans);
                        teklif.AddSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, model.Teminat.BelediyeHalkOtobusu);
                    }
                    else
                    {
                        teklif.AddSoru(TrafikSorular.Teminat_Asistans, false);
                        teklif.AddSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, false);
                    }

                    IMapfreTrafikTeklif trafikTeklif = new MapfreTrafikTeklif();

                    //Teklif alınacak şirketler
                    trafikTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.MAPFRE);

                    //Sadece Pesin Teklif Alınıyor
                    trafikTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;
                    teklif.GenelBilgiler.TaksitSayisi = 1;

                    IsDurum isDurum = trafikTeklif.Hesapla(teklif);
                    ITeklif hesaplanan = _TeklifService.GetTeklif(isDurum.ReferansId);

                    TeklifDurumModel fiyatModel = new TeklifDurumModel();
                    var detaylar = isDurum.IsDurumDetays.ToList<IsDurumDetay>();

                    var tumListe = detaylar.GroupBy(g => g.TUMKodu).Select(s => new { TUMKodu = s.Key });

                    ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
                    fiyatModel.teklifler = new List<TeklifFiyatDetayModel>();
                    foreach (var item in tumListe)
                    {
                        TeklifFiyatDetayModel fiyat = this.TeklifFiyat(item.TUMKodu, detaylar);

                        fiyatModel.teklifler.Add(fiyat);
                    }

                    if (hesaplanan != null)
                    {
                        fiyatModel.teklifId = hesaplanan.GenelBilgiler.TeklifId;
                        fiyatModel.teklifNo = teklif.GenelBilgiler.TeklifNo.ToString();
                        fiyatModel.pdf = teklif.GenelBilgiler.PDFDosyasi;

                        var fiyat = fiyatModel.teklifler.FirstOrDefault();
                        if (fiyat != null)
                        {
                            fiyatModel.TUMTeklifNo = fiyat.TUMTeklifNo;
                        }
                    }

                    string html = RenderViewToString(this.ControllerContext, "_TeklifFiyat", fiyatModel);
                    return Json(new { id = isDurum.IsId, html = html });
                }
                else
                {
                    _LogService.Info("ModelState is not valid");
                }
                #endregion

                #region Validasyon hatası
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
                _LogService.Error("MapfreTrafikKontroller.Hesapla", ex);

                StringBuilder sb = new StringBuilder();
                if (model.Musteri != null && model.Musteri.SigortaEttiren != null)
                {
                    sb.AppendFormat("SigortaEttiren : {0} | ", model.Musteri.SigortaEttiren.MusteriKodu);
                    sb.AppendFormat("SigortaEttiren KimlikNo : {0} | ", model.Musteri.SigortaEttiren.KimlikNo);
                }
                if (model.Arac != null)
                    sb.AppendFormat("Plaka : {0}{1} | ", model.Arac.PlakaKodu, model.Arac.PlakaNo);

                _LogService.Error("MapfreTrafikKontroller.Hesapla", sb.ToString());
            }

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        [HttpPost]
        public ActionResult OdemeAl(MapfreOdemeAlModel model)
        {
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
                    odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil);
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin)
                        odeme.TaksitSayisi = 1;
                    else
                        odeme.TaksitSayisi = model.KrediKarti.TaksitSayisi;
                }
                else
                {
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin)
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
                        _LogService.Error("MapfreTrafikKontroller.OdemeAl", ex);
                    }

                    cevap.Success = true;
                    cevap.RedirectUrl = TeklifSayfaAdresleri.PoliceAdres(urun.GenelBilgiler.UrunKodu) + urun.GenelBilgiler.TeklifId;
                    return Json(cevap);
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
            model.Arac = this.MapfreDetayAracBilgiModel(trafikTeklif);

            //Eski poliçe bilgileri
            model.EskiPolice = base.DetayEskiPoliceModel(trafikTeklif);
            model.EskiPolice.TramerIslemTipi = String.Empty;
            string tramerIslemTipi = trafikTeklif.ReadSoru(TrafikSorular.TramerIslemTipi, String.Empty);
            if (tramerIslemTipi == "1")
                model.EskiPolice.TramerIslemTipi = "1 - Önceki Bilgileri İle Giriş";
            else if (tramerIslemTipi == "2")
                model.EskiPolice.TramerIslemTipi = "11 - Önceki Bilgiler (Satış Referansılı Giriş)";

            //Taşıyıcı sorumluluk bilgileri
            model.Tasiyici = base.DetayTasiyiciSorumlulukModel(trafikTeklif);

            model.Teminat = new DetayTrafikTeminatModel();
            model.Teminat.Asistans = trafikTeklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
            model.Teminat.BelediyeHalkOtobusu = trafikTeklif.ReadSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, false);

            model.OdemeBilgileri = new OdemeBilgileriModel();
            model.OdemeBilgileri.TeklifId = teklif.GenelBilgiler.TeklifId;
            model.OdemeBilgileri.NetPrim = teklif.GenelBilgiler.NetPrim;
            model.OdemeBilgileri.ToplamVergi = teklif.GenelBilgiler.ToplamVergi;
            model.OdemeBilgileri.ToplamTutar = teklif.GenelBilgiler.BrutPrim;
            model.OdemeBilgileri.PoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            model.OdemeBilgileri.PoliceURL = teklif.GenelBilgiler.PDFPolice;
            model.OdemeBilgileri.BilgilendirmePDF = teklif.GenelBilgiler.PDFBilgilendirme;
            model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFGenelSartlari;
            model.OdemeBilgileri.DekontPDFGoster = teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti;


            TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);
            if (tum != null)
            {
                model.OdemeBilgileri.TUMUnvani = tum.Unvani;
                model.OdemeBilgileri.TUMLogoURL = tum.Logo;
            }
            return View(model);
        }

        private TeklifFiyatModel TrafikFiyat(TrafikTeklif trafikTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = trafikTeklif.Teklif.GenelBilgiler.TeklifId;

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

        [HttpPost]
        [AjaxException]
        public ActionResult BilgilendirmePDF(int id)
        {
            bool success = true;
            string url = String.Empty;
            ITeklif teklif = _TeklifService.GetTeklif(id);

            try
            {
                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFBilgilendirme))
                {
                    IMAPFREProjeTrafik urun = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeTrafik;
                    urun.BilgilendirmePDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFBilgilendirme))
                {
                    throw new Exception("Bilgilendirme pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.BilgilendirmePDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFBilgilendirme;
            return Json(new { Success = success, PDFUrl = url });
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
                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFGenelSartlari))
                {
                    IMAPFREProjeTrafik urun = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeTrafik;
                    urun.DekontPDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFGenelSartlari))
                {
                    throw new Exception("Bilgilendirme pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.DekontPDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFGenelSartlari;
            return Json(new { Success = success, PDFUrl = url });
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
                        model.PlakaNo = model.PlakaNo.ToUpperInvariant();

                        PlakaSorgu plakaSorgu = MAPFREPlakaSorgula(model, musteri);

                        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    }
                }

                throw new Exception("Plaka sorgulama servisi çalıştırılırken hata oluştu.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreTrafikKontroller.PlakaSorgula", ex);
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
                HasarsizlikResponse hasarsizlik = sorguService.HasarsizlikSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, "410");
                if (hasarsizlik != null)
                {
                    plakaSorgu.HasarsizlikInd = hasarsizlik.hasarsizlik_ind;
                    plakaSorgu.HasarsizlikSur = hasarsizlik.hasar_srp;
                    plakaSorgu.HasarsizlikKademe = hasarsizlik.uygulanacak_kademe;
                }

                if (sigortaSirketi == "050" || sigortaSirketi == "50")
                {
                    OncekiTescilResponse tescil = sorguService.OncekiTescilSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, "410");
                    if (tescil != null && String.IsNullOrEmpty(tescil.hata) && tescil.COD_ARAC_RUHSAT_SERI != "*")
                    {
                        plakaSorgu.TescilSeri = tescil.COD_ARAC_RUHSAT_SERI;
                        plakaSorgu.TescilSeriNo = tescil.COD_ARAC_RUHSAT_SERI_NO;
                    }
                }

                return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreTrafikKontroller.EskiPoliceSorgula", ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult TVM()
        {
            try
            {
                List<TVMOzetModel> model = _TVMService.GetTVMListeMapfre();

                return PartialView("_TVM", model);
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.TVM", ex);
                return PartialView("_TVM", null);
            }
        }

        [AjaxException]
        public ActionResult TVMKodu(int? tvmKodu)
        {
            try
            {
                if (!tvmKodu.HasValue)
                {
                    return Json(new { success = false, message = "Partaj numarasını giriniz." });
                }

                if (_AktifKullaniciService.TVMKodu == 107 ||
                    _AktifKullaniciService.MapfreBolge ||
                    _AktifKullaniciService.MapfreMerkezAcente)
                {
                    TVMDetay tvm = _TVMService.GetDetay(tvmKodu.Value);
                    if (tvm == null)
                    {
                        return Json(new { success = false, message = "Girilen partaj numarasına uygun kayıt bulunamadı." });
                    }

                    if (_AktifKullaniciService.MapfreBolge)
                    {
                        TVMDetay kullaniciTvm = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                        if (tvm.BolgeKodu != kullaniciTvm.BolgeKodu)
                        {
                            return Json(new { success = false, message = "Girilen partaj numarası ile işlem yapma yetkiniz yok." });
                        }
                    }
                    else if (_AktifKullaniciService.MapfreMerkezAcente)
                    {
                        TVMDetay kullaniciTvm = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                        if (tvm.GrupKodu != kullaniciTvm.Kodu)
                        {
                            return Json(new { success = false, message = "Girilen partaj numarası ile işlem yapma yetkiniz yok." });
                        }
                    }

                    return Json(new { success = true, tvmKodu = tvm.Kodu, tvmUnvani = tvm.Unvani });
                }
                else
                {
                    return Json(new { success = false, message = "Yetkiniz bulunmuyor." });
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.TVMKodu", ex);
                return Json(new { success = false, message = "Partaj aranırken hata oluştu." });
            }
        }

        [AjaxException]
        public JsonResult HazineYururluluk(string kimlikNo, string plakaNo, string plakaIlKodu, string aracKullanimTarzi)
        {
            try
            {
                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();

                string tarifeGrupKodu = String.Empty;
                string[] ks = aracKullanimTarzi.Split('-');
                if (ks.Length == 2)
                {
                    string kullnimT = ks[0];
                    string kod2 = ks[1];
                    ICRService _CR = DependencyResolver.Current.GetService<ICRService>();
                    var aracgruplari = _CR.GetAracGruplari(TeklifUretimMerkezleri.MAPFRE);

                    CR_AracGrup kTarzi = aracgruplari.Where(f => f.KullanimTarziKodu == kullnimT &&
                                                                 f.Kod2 == kod2).SingleOrDefault<CR_AracGrup>();
                    if (kTarzi != null)
                    {
                        tarifeGrupKodu = kTarzi.TarifeKodu;
                    }
                }

                HazineYururlulukResponse response = sorguService.HazineYururluluk("410", kimlikNo, plakaIlKodu, plakaNo, tarifeGrupKodu);

                return Json(new { success = true, response = response });
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreTrafikKontroller.HazineYururluluk", ex);
                return Json(new { success = false, message = "Hazine yürürlülük kontrolünde hata oluştu :" + ex.Message });
            }
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

            DateTime eskiPoliceTarih = response.trafikHasar.hasarBilgi.Where(w => w.PoliceBitisTarihi < DateTime.Today.AddDays(30))
                                                                      .DefaultIfEmpty(new TrafikHasarBilgi() { polBitisTarihi = "01/01/1900" })
                                                                      .Max(m => m.PoliceBitisTarihi);
            DateTime maxPoliceTarih = response.trafikHasar.hasarBilgi.Max(m => m.PoliceBitisTarihi);
            TrafikHasarBilgi eskiPoliceBilgi = response.trafikHasar.hasarBilgi.FirstOrDefault(w => w.PoliceBitisTarihi == eskiPoliceTarih);
            TrafikHasarBilgi bilgi = response.trafikHasar.hasarBilgi.FirstOrDefault(w => w.PoliceBitisTarihi == maxPoliceTarih);

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracMarkaKodu = bilgi.marka;
            plakaSorgu.AracTipKodu = bilgi.model.TrimStart('0');
            plakaSorgu.AracModelYili = bilgi.modelYili;
            plakaSorgu.AracMotorNo = bilgi.motorNo;
            plakaSorgu.AracSasiNo = bilgi.sasiNo;

            if (eskiPoliceBilgi != null)
            {
                plakaSorgu.EskiPoliceSigortaSirkedKodu = eskiPoliceBilgi.sirketKodu;
                plakaSorgu.EskiPoliceAcenteKod = eskiPoliceBilgi.acenteKod;
                plakaSorgu.EskiPoliceNo = eskiPoliceBilgi.policeNo;
                plakaSorgu.EskiPoliceYenilemeNo = eskiPoliceBilgi.yenilemeNo;
                if (!String.IsNullOrEmpty(eskiPoliceBilgi.polBitisTarihi))
                {
                    DateTime policeBitis = MapfreSorguResponse.ToDateTime(eskiPoliceBilgi.polBitisTarihi);

                    if (policeBitis < TurkeyDateTime.Today)
                        plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                    else
                        plakaSorgu.YeniPoliceBaslangicTarih = policeBitis.ToString("dd.MM.yyyy");
                }
            }
            else
            {
                plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
            }

            if (bilgi.yolcuKapasitesi.Contains(".0"))
            {
                plakaSorgu.AracKoltukSayisi = bilgi.yolcuKapasitesi.Replace(".0", "");
            }
            else
            {
                plakaSorgu.AracKoltukSayisi = bilgi.yolcuKapasitesi;
            }
            int koltukSayisi = 0;
            int.TryParse(plakaSorgu.AracKoltukSayisi, out koltukSayisi);
            if (koltukSayisi == 5)
            {
                plakaSorgu.AracKoltukSayisi = "4";
            }

            int modelYili = int.Parse(bilgi.modelYili);
            plakaSorgu.AracKullanimSekli = "2";
            plakaSorgu.AracKullanimTarzi = "111-10";

            if (!String.IsNullOrEmpty(bilgi.aracTarifeGrupKod))
            {
                var aracgruplari = _CRService.GetAracGruplari(TeklifUretimMerkezleri.MAPFRE);
                CR_AracGrup aracGrup = aracgruplari.Where(s => s.TarifeKodu == bilgi.aracTarifeGrupKod).FirstOrDefault();

                if (aracGrup != null)
                {
                    var tarzi = _AracService.GetAracKullanimTarzi(aracGrup.KullanimTarziKodu, aracGrup.Kod2);

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

            //AracTip tip = _AracService.GetAracTip(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu);
            //if (tip != null)
            //{
            //    kullanimSekli = tip.KullanimSekli1;
            //    plakaSorgu.AracKullanimTarzi = tip.KullanimSekli1 + "-10";
            //}

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetAracGrupKodlari(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracMarka> markalar = _AracService.GetAracMarkaList();
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(bilgi.marka);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);
            if (aracModel != null)
            {
                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
            }

            plakaSorgu.AracTescilTarih = MapfreSorguResponse.ToDateTime(bilgi.tescilTarihi).ToString("dd.MM.yyyy");

            List<KeyValueItem<string, string>> amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, "111", "10");
            plakaSorgu.Ams = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();

            if (plakaSorgu.EskiPoliceSigortaSirkedKodu == "050" || plakaSorgu.EskiPoliceSigortaSirkedKodu == "50")
            {
                OncekiTescilResponse tescil = mapfreSorgu.OncekiTescilSorgu(musteri.KimlikNo, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceYenilemeNo, "410");
                if (tescil != null && String.IsNullOrEmpty(tescil.hata) && tescil.COD_ARAC_RUHSAT_SERI != "*")
                {
                    plakaSorgu.TescilSeri = tescil.COD_ARAC_RUHSAT_SERI;
                    plakaSorgu.TescilSeriNo = tescil.COD_ARAC_RUHSAT_SERI_NO;
                }
            }

            return plakaSorgu;
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

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetAracGrupKodlari(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracMarka> markalar = _AracService.GetAracMarkaList();
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(plakaSorgu.AracMarkaKodu);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);
            if (aracModel != null)
                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;

            List<KeyValueItem<string, string>> amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, "111", "10");
            plakaSorgu.Ams = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();

            return plakaSorgu;
        }

        private DetayAracBilgiModel MapfreDetayAracBilgiModel(ITeklif teklif)
        {
            DetayAracBilgiModel model = new DetayAracBilgiModel();

            TeklifArac arac = teklif.Arac;
            model.PlakaKodu = arac.PlakaKodu;
            model.PlakaNo = arac.PlakaNo;

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            if (!String.IsNullOrEmpty(arac.KullanimSekli))
            {
                string[] ks = arac.KullanimSekli.Split('-');
                if (ks.Length == 2)
                {
                    CR_AracGrup kullanimSekli = _CRService.GetAracGrupKodu(TeklifUretimMerkezleri.MAPFRE, ks[0], ks[1]);
                    model.KullanimSekli = kullanimSekli != null ? kullanimSekli.Aciklama : String.Empty;
                }
            }

            string[] parts = arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                CR_KullanimTarzi kullanimTarzi = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                model.KullanimTarzi = kullanimTarzi != null ? kullanimTarzi.Aciklama : String.Empty;
            }

            AracMarka marka = aracService.GetAracMarka(arac.Marka);
            model.Marka = marka != null ? marka.MarkaAdi : String.Empty;

            model.Model = arac.Model.HasValue ? arac.Model.Value : 0;

            AracTip tip = aracService.GetAracTip(arac.Marka, arac.AracinTipi);
            model.Tip = tip != null ? tip.TipAdi : String.Empty;

            model.MotorNo = arac.MotorNo;
            model.SaseNo = arac.SasiNo;

            string tescilIlKodu = arac.TescilIlKodu;
            string tescilIlceKodu = arac.TescilIlceKodu;

            if (!String.IsNullOrEmpty(tescilIlKodu))
            {
                CR_TescilIlIlce ilIlce = _CRService.GetTescilIlIlce(TeklifUretimMerkezleri.HDI, tescilIlKodu, tescilIlceKodu);

                if (ilIlce != null)
                {
                    model.TescilIl = ilIlce.TescilIlAdi;
                    model.TescilIlce = ilIlce.TescilIlceAdi;
                }
            }

            if (arac.TrafikTescilTarihi.HasValue)
            {
                model.TrafikTescilTarihi = arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy");
            }
            if (arac.TrafikCikisTarihi.HasValue)
            {
                model.TrafigeCikisTarihi = arac.TrafikCikisTarihi.Value.ToString("dd.MM.yyyy");
            }

            DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, DateTime.MinValue);
            if (polBaslangic != DateTime.MinValue)
            {
                model.PoliceBaslangicTarihi = polBaslangic.ToString("dd.MM.yyyy");
            }

            model.TescilBelgeSeriKod = arac.TescilSeriKod;
            model.TescilBelgeSeriNo = arac.TescilSeriNo;
            model.AsbisNo = arac.AsbisNo;
            model.Deger = arac.AracDeger;

            return model;
        }
    }
}

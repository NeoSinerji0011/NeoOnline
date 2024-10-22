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
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using System.Linq.Expressions;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    // [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.MapfreKasko)]
    public class MapfreKaskoController : TeklifController
    {
        public MapfreKaskoController(ITVMService tvmService, ITeklifService teklifService, IMusteriService musteriService,
                                IKullaniciService kullaniciService, IAktifKullaniciService aktifKullaniciService, ITanimService tanimService,
                                IUlkeService ulkeService, ICRService crService, IAracService aracService,
                                IUrunService urunService, ITUMService tumService)
            : base(tvmService, teklifService, musteriService,
                    kullaniciService, aktifKullaniciService, tanimService,
                    ulkeService, crService, aracService,
                    urunService, tumService)
        {

        }

        public ActionResult Ekle(int? id)
        {
            MapfreKaskoModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            MapfreKaskoModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public MapfreKaskoModel EkleModel(int? id, int? teklifId)
        {
            MapfreKaskoModel model = new MapfreKaskoModel();

            #region Teklif Genel
            ILogService log = DependencyResolver.Current.GetService<ILogService>();
            log.Visit();
            ITeklif teklif = null;
            #endregion

            #region Teklif Hazırlayan
            int? sigortaliMusteriKodu = null;

            //Teklifi hazırlayan
            model.Hazirlayan = new MapfreHazirlayanModel();
            model.Hazirlayan.FarkliAcenteSecebilir = (_AktifKullaniciService.MapfreBolge || _AktifKullaniciService.MapfreMerkez);
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
            model.Musteri.AcikAdresRequired = true;
            model.Musteri.EMailRequired = false;
            model.Musteri.SigortaliAyni = true;

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
                model.Musteri.Sigortali.CepTelefonu = "90";
            }
            model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
            model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
            model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
            model.Musteri.CinsiyetTipleri.First().Selected = true;
            #endregion

            #region Teklif Arac
            model.Arac = new MapfreKaskoAracBilgiModel();
            model.Arac.PlakaKodu = "";
            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            model.Arac.PlakaKoduListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu").ListWithOptionLabel();
            model.Arac.KullanimSekilleri = new SelectList(aracService.GetAracKullanimSekliList(), "KullanimSekliKodu", "KullanimSekli", "").ListWithOptionLabel();
            model.Arac.KullanimTarzlari = new SelectList(_CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE), "Kod", "KullanimTarzi").ListWithOptionLabel();
            model.Arac.MarkaKodu = String.Empty;
            model.Arac.Markalar = new SelectList(aracService.GetAracMarkaList(), "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
            model.Arac.TipKodu = String.Empty;
            model.Arac.AracTipleri = EmptySelectList.EmptyList();

            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
                yillar.Add(yil);

            List<short> kisiler = new List<short>();
            for (short sayi = 1; sayi < 60; sayi++)
                kisiler.Add(sayi);

            model.Arac.Modeller = new SelectList(yillar).ListWithOptionLabel();
            model.Arac.AracTipleri = EmptySelectList.EmptyList();
            model.Arac.KisiSayisiListe = new SelectList(kisiler).ListWithOptionLabel();
            model.Arac.TescilIller = new SelectList(_CRService.GetTescilIlList(), "Key", "Value", model.Arac.TescilIl).ListWithOptionLabel();
            model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value", "").ListWithOptionLabel(); ;
            model.Arac.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.Arac.SigortaSirketleri = this.SigortaSirketleri;

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

                    List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
                    model.Arac.KullanimTarzlari = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
                }

                if (!String.IsNullOrEmpty(teklif.Arac.TescilIlKodu))
                {
                    model.Arac.TescilIl = teklif.Arac.TescilIlKodu;
                    model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value").ListWithOptionLabel();
                }

                if (!String.IsNullOrEmpty(teklif.Arac.TescilIlceKodu))
                    model.Arac.TescilIlce = teklif.Arac.TescilIlceKodu;


                if (!String.IsNullOrEmpty(teklif.Arac.Marka))
                {
                    model.Arac.MarkaKodu = teklif.Arac.Marka;

                    if (teklif.Arac.Model.HasValue)
                        model.Arac.Model = teklif.Arac.Model.Value;

                    if (!String.IsNullOrEmpty(teklif.Arac.AracinTipi))
                    {
                        model.Arac.TipKodu = teklif.Arac.AracinTipi;

                        List<AracTip> tipler = _AracService.GetAracTipList(model.Arac.MarkaKodu);
                        model.Arac.AracTipleri = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                    }

                    if (teklif.Arac.AracDeger.HasValue)
                    {
                        model.Arac.AracDeger = ((int)teklif.Arac.AracDeger.Value).ToString();
                    }

                    if (teklif.Arac.KoltukSayisi.HasValue)
                        model.Arac.KisiSayisi = teklif.Arac.KoltukSayisi.Value;
                }

                if (teklif.Arac.TrafikTescilTarihi.HasValue)
                    model.Arac.TrafikTescilTarihi = teklif.Arac.TrafikTescilTarihi.Value;

                if (teklif.Arac.TrafikCikisTarihi.HasValue)
                    model.Arac.TrafigeCikisTarihi = teklif.Arac.TrafikCikisTarihi.Value;

                model.Arac.PoliceBaslangicTarihi = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, DateTime.MinValue);
                model.Arac.TescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                model.Arac.TescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                model.Arac.AsbisNo = teklif.Arac.AsbisNo;
                model.Arac.MotorNo = teklif.Arac.MotorNo;
                model.Arac.SaseNo = teklif.Arac.SasiNo;

                model.Arac.HasarsizlikIndirim = teklif.ReadSoru(KaskoSorular.HasarsizlikIndirim, String.Empty);
                model.Arac.HasarSurprim = teklif.ReadSoru(KaskoSorular.HasarSurprim, String.Empty);
                model.Arac.UygulananKademe = teklif.ReadSoru(KaskoSorular.UygulananKademe, String.Empty);
            }
            #endregion

            #region Teklif Eski Poliçe
            model.EskiPolice = base.EkleEskiPoliceModel();
            if (teklifId.HasValue)
            {
                model.EskiPolice.EskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
                if (model.EskiPolice.EskiPoliceVar)
                {
                    model.EskiPolice.PoliceNo = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                    model.EskiPolice.SigortaSirketiKodu = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    model.EskiPolice.AcenteNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                    model.EskiPolice.YenilemeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);
                }
            }
            #endregion

            #region Tasıyıcı Sorumluluk
            model.Tasiyici = base.EkleTasiyiciSorumlulukModel();
            if (teklifId.HasValue)
            {
                model.Tasiyici.YetkiBelgesi = teklif.ReadSoru(KaskoSorular.Tasima_Yetki_Belgesi_VarYok, false);
                model.Tasiyici.Sorumluluk = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_VarYok, false);
                if (model.Tasiyici.Sorumluluk)
                {
                    model.Tasiyici.PoliceNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                    model.Tasiyici.SigortaSirketiKodu = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                    model.Tasiyici.AcenteNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                    model.Tasiyici.YenilemeNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
                }
            }
            #endregion

            #region Daini Mürtein
            model.DainiMurtein = new MapfreKaskoDainiMurtein();
            model.DainiMurtein.DainiMurtein = false;
            List<KeyValueItem<int, string>> kurumTipleri = new List<KeyValueItem<int, string>>();
            kurumTipleri.Add(new KeyValueItem<int, string>(0, "[SEÇİNİZ]"));
            kurumTipleri.Add(new KeyValueItem<int, string>(1, "BANKA"));
            kurumTipleri.Add(new KeyValueItem<int, string>(2, "FİNANSAL KURUM"));
            kurumTipleri.Add(new KeyValueItem<int, string>(3, "DİĞER"));
            if (teklifId.HasValue)
            {
                model.DainiMurtein.DainiMurtein = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
                if (model.DainiMurtein.DainiMurtein)
                {
                    model.DainiMurtein.KimlikNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);
                    model.DainiMurtein.Unvan = teklif.ReadSoru(KaskoSorular.DainiMurtein_Unvan, String.Empty);

                    string kurumTipi = teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumTipi, String.Empty);
                    if (!String.IsNullOrEmpty(kurumTipi))
                    {
                        int kurumTipiId = 0;
                        if (int.TryParse(kurumTipi, out kurumTipiId))
                        {
                            model.DainiMurtein.KurumTipi = kurumTipiId;
                            model.DainiMurtein.KurumKodu = teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumKodu, String.Empty);
                            model.DainiMurtein.KurumKodu1 = model.DainiMurtein.KurumKodu;
                            model.DainiMurtein.SubeKodu = teklif.ReadSoru(KaskoSorular.DainiMurtein_SubeKodu, String.Empty);

                            var kurumlar = _CRService.GetKaskoDMListe(TeklifUretimMerkezleri.MAPFRE, model.DainiMurtein.KurumTipi);
                            model.DainiMurtein.Kurumlar = new SelectList(kurumlar, "KurumKodu", "KurumAdi", model.DainiMurtein.KurumKodu);
                        }
                    }
                }
                else
                {
                    model.DainiMurtein.Kurumlar = new SelectList(new List<CR_KaskoDM>(), "KurumKodu", "KurumAdi");
                }
            }
            else
            {
                model.DainiMurtein.KurumTipi = 0;
                model.DainiMurtein.Kurumlar = new SelectList(new List<CR_KaskoDM>(), "KurumKodu", "KurumAdi");
            }
            model.DainiMurtein.KurumTipleri = new SelectList(kurumTipleri, "Key", "Value", model.DainiMurtein.KurumTipi);
            #endregion

            #region Teklif Teminat
            model.Teminat = new MapfreKaskoTeminatModel();
            if (teklifId.HasValue)
            {
                string[] aracKullanim = teklif.Arac.KullanimTarzi.Split('-');
                model.Teminat.AMS = new SelectList(_CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE,
                                                                            _AktifKullaniciService.TVMKodu,
                                                                            aracKullanim[0],
                                                                            aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

                string amsKodu = teklif.ReadSoru(KaskoSorular.Teminat_AMS_Kodu, String.Empty);
                if (!String.IsNullOrEmpty(amsKodu))
                {
                    model.Teminat.AMSKodu = amsKodu;
                }
                decimal olumSakatlik = teklif.ReadSoru(KaskoSorular.Teminat_Olum_Sakatlik, decimal.Zero);
                model.Teminat.OlumSakatlikTeminat = (int)olumSakatlik;

                model.Teminat.Tedavi = teklif.ReadSoru(KaskoSorular.Teminat_Tedavi_VarYok, false);
                if (model.Teminat.Tedavi)
                {
                    decimal tedavi = teklif.ReadSoru(KaskoSorular.Teminat_Tedavi_Tutar, decimal.Zero);
                    model.Teminat.TedaviTeminat = (int)tedavi;
                }
                else
                    model.Teminat.TedaviTeminat = 0;

                model.Teminat.Deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
                model.Teminat.DepremMuafiyetKodu = teklif.ReadSoru(KaskoSorular.Teminat_Deprem_Muafiyet_Kodu, "0");
                model.Teminat.DepremMuafiyetKodlari = new SelectList(TeklifListeleri.DepremMuafiyetKodlari(), "Value", "Text");

                model.Teminat.Yutr_Disi_Teminat = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);
                model.Teminat.Yurt_Disi_Teminati_Sure = (int)teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, decimal.Zero);
                model.Teminat.Yurt_Disi_Teminat_Ulke = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Ulke, String.Empty);
                model.Teminat.Yurtdisi_Teminat_Sureleri = TeklifListeleri.MapfreKaskoYurtDisiTeminatSureleri();
                model.Teminat.Yurtdisi_Teminat_Ulkeler = new SelectList(_CRService.GetUlkeler(TeklifUretimMerkezleri.MAPFRE), "UlkeKodu", "UlkeAdi").ListWithOptionLabel();
                model.Teminat.Hukuksal_Koruma_Teminati = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, true);
                model.Teminat.Eskime_Payi_Teminati = teklif.ReadSoru(KaskoSorular.Eskime_VarYok, true);
                model.Teminat.Anahtar_Kaybi_Teminati = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, true);
                model.Teminat.Ozel_Esya_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, true);
                model.Teminat.Anahtarla_Calinma_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_Anahtarla_Calinma_VarYok, true);
                model.Teminat.FiloPolice_Teminati = teklif.ReadSoru(KaskoSorular.FiloPolice, false);

                model.Teminat.Kullanici_Teminat = teklif.ReadSoru(KaskoSorular.Arac_Kullanici_Teminat, false);
                if (model.Teminat.Kullanici_Teminat)
                {
                    model.Teminat.Kullanici_TCKN = teklif.ReadSoru(KaskoSorular.Arac_Kullanici_TCKN, String.Empty);
                    model.Teminat.Kullanici_Adi = teklif.ReadSoru(KaskoSorular.Arac_Kullanici_Adi, String.Empty);
                }

                model.Teminat.OnarimYeri = teklif.ReadSoru(KaskoSorular.OnarimYeri, "T");
                model.Teminat.OnarimYerleri = TeklifListeleri.OnarimYerleri();

                model.Teminat.Aksesuar_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_Ekstra_Aksesuar_VarYok, false);
                model.Teminat.AksesuarTipleri = new SelectList(_CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, "AKS"), "SoruKodu", "SoruAdi", "").ListWithOptionLabel();
                List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR).ToList<TeklifAracEkSoru>();
                model.Teminat.Aksesuarlar = new List<MapfreAksesuarModel>();
                if (aksesuarlar != null && aksesuarlar.Count > 0)
                {
                    foreach (var item in aksesuarlar)
                    {
                        model.Teminat.Aksesuarlar.Add(new MapfreAksesuarModel()
                        {
                            AksesuarTip = item.SoruKodu,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel
                        });
                    }
                }

                model.Teminat.ElektronikCihaz_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_ElektronikCihaz_VarYok, false);
                model.Teminat.ElektronikCihazTipleri = new SelectList(_CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, "ELK"), "SoruKodu", "SoruAdi", "").ListWithOptionLabel();
                List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ).ToList<TeklifAracEkSoru>();
                model.Teminat.Cihazlar = new List<MapfreAksesuarModel>();
                if (elekCihazlar != null && elekCihazlar.Count > 0)
                {
                    foreach (var item in elekCihazlar)
                    {
                        model.Teminat.Cihazlar.Add(new MapfreAksesuarModel()
                        {
                            AksesuarTip = item.SoruKodu,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel
                        });
                    }
                }

                model.Teminat.TasinanYuk_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, false);
                model.Teminat.TasinanYukTipleri = new SelectList(_CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, "YUK"), "SoruKodu", "SoruAdi", "").ListWithOptionLabel();
                List<TeklifAracEkSoru> tasinanYukler = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.TASINAN_YUK).ToList<TeklifAracEkSoru>();
                model.Teminat.TasinanYukler = new List<MapfreTasinanYukModel>();
                if (tasinanYukler != null && tasinanYukler.Count > 0)
                {
                    foreach (var item in tasinanYukler)
                    {
                        model.Teminat.TasinanYukler.Add(new MapfreTasinanYukModel()
                        {
                            TasinanYukTip = item.SoruKodu,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel,
                            Fiyat = (int)item.Fiyat
                        });
                    }
                }

                if (teklif.GenelBilgiler.TeklifNot != null)
                {
                    model.Aciklama = teklif.GenelBilgiler.TeklifNot.Aciklama;
                }

                model.Teminat.IkameTuru = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                model.Teminat.IkameTurleri = new List<SelectListItem>();
                if (!String.IsNullOrEmpty(model.Arac.KullanimTarziKodu))
                {
                    string[] parts = model.Arac.KullanimTarziKodu.Split('-');
                    if (parts.Length == 2)
                    {
                        CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                        if (tarz != null)
                        {
                            List<KeyValueItem<string, string>> list = _CRService.GetKaskoIkameTuruList(TeklifUretimMerkezleri.MAPFRE, tarz.TarifeKodu);
                            if (list != null && list.Count > 0)
                            {
                                model.Teminat.IkameTurleri = new SelectList(list, "Key", "Value").ListWithOptionLabel();
                            }
                        }
                    }
                }

                model.IndirimSurprim = new KaskoIndirimSurprimModel();
                model.IndirimSurprim.IndirimTipleri = TeklifListeleri.MapfreIndirimSurprimTipleri();
                model.IndirimSurprim.IndirimTipi = (int)teklif.ReadSoru(KaskoSorular.IndirimSurprimTipi, 0);
                decimal indirimOrani = teklif.ReadSoru(KaskoSorular.IndirimSurprimOrani, 0);
                if (indirimOrani > 0)
                {
                    model.IndirimSurprim.IndirimOrani = (int)indirimOrani;
                }
            }
            else
            {
                model.Teminat = new MapfreKaskoTeminatModel();
                model.Teminat.AMS = new SelectList(_CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE,
                                                                _AktifKullaniciService.TVMKodu,
                                                                "111", "10"), "Key", "Value", "").ListWithOptionLabel();
                model.Teminat.AMSKodu = String.Empty;
                model.Teminat.OlumSakatlikTeminat = 5000;

                model.Teminat.Tedavi = false;
                model.Teminat.TedaviTeminat = 0;

                model.Teminat.Deprem = true;
                model.Teminat.DepremMuafiyetKodu = "0";
                model.Teminat.DepremMuafiyetKodlari = new SelectList(TeklifListeleri.DepremMuafiyetKodlari(), "Value", "Text");

                model.Teminat.Yutr_Disi_Teminat = false;
                model.Teminat.Yurtdisi_Teminat_Sureleri = TeklifListeleri.MapfreKaskoYurtDisiTeminatSureleri();
                model.Teminat.Yurtdisi_Teminat_Ulkeler = new SelectList(_CRService.GetUlkeler(TeklifUretimMerkezleri.MAPFRE), "UlkeKodu", "UlkeAdi").ListWithOptionLabel();
                model.Teminat.Hukuksal_Koruma_Teminati = true;
                model.Teminat.Eskime_Payi_Teminati = true;
                model.Teminat.Anahtar_Kaybi_Teminati = true;
                model.Teminat.Ozel_Esya_Teminati = true;
                model.Teminat.Anahtarla_Calinma_Teminati = true;
                model.Teminat.Kullanici_Teminat = false;
                model.Teminat.FiloPolice_Teminati = false;
                model.Teminat.IkameTuru = String.Empty;

                model.Teminat.Aksesuar_Teminati = false;
                model.Teminat.AksesuarTipleri = TeklifListeleri.MapfreAksesuarTipleri();
                model.Teminat.ElektronikCihaz_Teminati = false;
                model.Teminat.ElektronikCihazTipleri = TeklifListeleri.MapfreElektronikCihazTipleri();
                model.Teminat.TasinanYuk_Teminati = false;
                model.Teminat.TasinanYukTipleri = TeklifListeleri.MapfreTasinanYukTipleri();
                model.Teminat.IkameTurleri = new List<SelectListItem>();

                model.IndirimSurprim = new KaskoIndirimSurprimModel();
                model.IndirimSurprim.IndirimTipi = 0;
                model.IndirimSurprim.IndirimTipleri = TeklifListeleri.MapfreIndirimSurprimTipleri();

                model.Teminat.OnarimYeri = "T";
                model.Teminat.OnarimYerleri = TeklifListeleri.OnarimYerleri();
            }

            #endregion

            #region TUM IMAGES

            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.MapfreKasko);
            foreach (var item in urunyetkileri)
            {
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);
            }
            #endregion

            #region Odeme
            model.Odeme = new KaskoTeklifOdemeModel();
            model.Odeme.OdemeSekli = true;
            model.Odeme.OdemeTipi = 1;
            model.Odeme.TaksitSayilari = new List<SelectListItem>();
            List<SelectListItem> odemeTipleri = new List<SelectListItem>();
            odemeTipleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="NAKİT",Value="1"},
                new SelectListItem(){Text="KREDİ KARTI",Value="2"}
            });
            model.Odeme.OdemeTipleri = new SelectList(odemeTipleri, "Value", "Text", "1").ToList();

            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "1", Value = "1" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "2", Value = "2" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "3", Value = "3" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "4", Value = "4" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "5", Value = "5" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "6", Value = "6" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "7", Value = "7" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "8", Value = "8" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "9", Value = "9" });

            model.KrediKarti = new MapfreOdemeModel();
            model.KrediKarti.KK_OdemeSekli = 1;
            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="PEŞİN",Value="1"},
                new SelectListItem(){Text="VADELİ",Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = odemeSekilleri;
            model.KrediKarti.KK_OdemeTipi = 1;
            model.KrediKarti.OdemeTipleri = new SelectList(odemeTipleri, "Value", "Text", "1").ToList();
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});

            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            #endregion

            return model;
        }

        public ActionResult Detay(int id)
        {
            DetayMapfreKaskoModel model = new DetayMapfreKaskoModel();

            #region Teklif Genel

            KaskoTeklif kaskoTeklif = new KaskoTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = kaskoTeklif.Teklif.TeklifNo.ToString();
            model.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(kaskoTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(kaskoTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (kaskoTeklif.Teklif.Sigortalilar.Count > 0 &&
               (kaskoTeklif.Teklif.SigortaEttiren.MusteriKodu != kaskoTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(kaskoTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Teklif Arac
            model.Arac = this.MapfreDetayAracBilgiModel(kaskoTeklif.Teklif);
            #endregion

            #region Eski Police
            //Eski poliçe bilgileri
            model.EskiPolice = base.DetayEskiPoliceModel(kaskoTeklif.Teklif);
            #endregion

            #region Tasiyıcı Sorumluluk
            //Taşıyıcı sorumluluk bilgileri
            model.Tasiyici = base.DetayTasiyiciSorumlulukModel(kaskoTeklif.Teklif);
            #endregion

            #region Dain-i Mürtein
            model.DainiMurtein = new MapfreKaskoDainiMurtein();
            model.DainiMurtein.DainiMurtein = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
            if (model.DainiMurtein.DainiMurtein)
            {
                model.DainiMurtein.KimlikNo = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);
                model.DainiMurtein.Unvan = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_Unvan, String.Empty);

                string kurumTipi = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumTipi, String.Empty);
                if (!String.IsNullOrEmpty(kurumTipi))
                {
                    int kurumTipiId = 0;
                    if (int.TryParse(kurumTipi, out kurumTipiId))
                    {
                        model.DainiMurtein.KurumTipi = kurumTipiId;
                        model.DainiMurtein.KurumKodu = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumKodu, String.Empty);
                        model.DainiMurtein.KurumKodu1 = model.DainiMurtein.KurumKodu;
                        model.DainiMurtein.SubeKodu = kaskoTeklif.Teklif.ReadSoru(KaskoSorular.DainiMurtein_SubeKodu, String.Empty);

                        switch (kurumTipiId)
                        {
                            case 1: model.DainiMurtein.KurumTipiAdi = "BANKA"; break;
                            case 2: model.DainiMurtein.KurumTipiAdi = "FİNANSAL KURUM"; break;
                            case 3: model.DainiMurtein.KurumTipiAdi = "DİĞER"; break;
                        }

                        CR_KaskoDM kurum = _CRService.GetKaskoDM(TeklifUretimMerkezleri.MAPFRE, kurumTipiId, model.DainiMurtein.KurumKodu);
                        if (kurum != null)
                        {
                            model.DainiMurtein.KurumAdi = kurum.KurumAdi;
                        }
                    }
                }
            }
            #endregion

            #region Teminatlar
            model.Teminat = TeklifTeminatDoldur(kaskoTeklif.Teklif);
            #endregion

            #region İndirim / Surprim
            model.IndirimSurprim = new DetayMapfreIndirimSurprimModel();
            model.IndirimSurprim.IndirimTipi = (int)kaskoTeklif.Teklif.ReadSoru(KaskoSorular.IndirimSurprimTipi, 0);
            if (model.IndirimSurprim.IndirimTipi > 0)
            {
                if (model.IndirimSurprim.IndirimTipi == MapfreIndirimSurprim.Indirim)
                    model.IndirimSurprim.IndirimTipAdi = "İndirim";
                else if (model.IndirimSurprim.IndirimTipi == MapfreIndirimSurprim.Surprim)
                    model.IndirimSurprim.IndirimTipAdi = "Surprim";

                model.IndirimSurprim.IndirimOrani = (int)kaskoTeklif.Teklif.ReadSoru(KaskoSorular.IndirimSurprimOrani, 0);
            }
            #endregion

            #region Açıklama
            if (kaskoTeklif.Teklif.GenelBilgiler.TeklifNot != null)
            {
                model.Aciklama = kaskoTeklif.Teklif.GenelBilgiler.TeklifNot.Aciklama;
            }
            #endregion

            #region Teklif Fiyat
            IsDurum durum = kaskoTeklif.GetIsDurum();
            IsDurumDetay durumDetay = durum.IsDurumDetays.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);
            model.Fiyat = this.MapfreTeklifFiyat(TeklifUretimMerkezleri.MAPFRE, durumDetay);
            model.KrediKarti = new MapfreOdemeModel();
            model.KrediKarti.KK_OdemeSekli = kaskoTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="PEŞİN",Value="1"},
                new SelectListItem(){Text="VADELİ",Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = odemeSekilleri;
            model.KrediKarti.KK_OdemeTipi = kaskoTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;
            List<SelectListItem> odemeTipleri = new List<SelectListItem>();
            odemeTipleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="NAKİT",Value="1"},
                new SelectListItem(){Text="KREDİ KARTI",Value="2"}
            });
            model.KrediKarti.OdemeTipleri = new SelectList(odemeTipleri, "Value", "Text", "2").ToList();

            model.KrediKarti.TaksitSayisi = kaskoTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});

            ITeklif mapfreTeklif = kaskoTeklif.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

            model.TUMTeklifNo = mapfreTeklif.GenelBilgiler.TUMTeklifNo;

            model.KrediKarti.KK_TeklifId = mapfreTeklif.GenelBilgiler.TeklifId;
            model.KrediKarti.Tutar = mapfreTeklif.GenelBilgiler.BrutPrim;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();


            model.Satinalinabilir = String.IsNullOrEmpty(mapfreTeklif.GenelBilgiler.TUMPoliceNo) &&
                                    mapfreTeklif.GenelBilgiler.GecerlilikBitisTarihi >= TurkeyDateTime.Today;
            #endregion

            return View(model);
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
                        _LogService.Error("MapfreKaskoController.OdemeAl", ex);
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

        [HttpPost]
        public ActionResult Hesapla(MapfreKaskoModel model)
        {
            #region Teklif kontrol (Valid)

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
                if (model.DainiMurtein != null && !model.DainiMurtein.DainiMurtein)
                {
                    if (ModelState["DainiMurtein.KimlikTipi"] != null)
                        ModelState["DainiMurtein.KimlikTipi"].Errors.Clear();
                    if (ModelState["DainiMurtein.KimlikNo"] != null)
                        ModelState["DainiMurtein.KimlikNo"].Errors.Clear();
                    if (ModelState["DainiMurtein.Unvan"] != null)
                        ModelState["DainiMurtein.Unvan"].Errors.Clear();
                    if (ModelState["DainiMurtein.KurumTipi"] != null)
                        ModelState["DainiMurtein.KurumTipi"].Errors.Clear();
                    if (ModelState["DainiMurtein.KurumKodu"] != null)
                        ModelState["DainiMurtein.KurumKodu"].Errors.Clear();
                    if (ModelState["DainiMurtein.KurumKodu1"] != null)
                        ModelState["DainiMurtein.KurumKodu1"].Errors.Clear();
                }
                if (ModelState["Teminat.AMSKodu"] != null)
                    ModelState["Teminat.AMSKodu"].Errors.Clear();
                if (ModelState["Teminat.OlumSakatlikTeminat"] != null)
                    ModelState["Teminat.OlumSakatlikTeminat"].Errors.Clear();

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
                if (!model.Teminat.Yutr_Disi_Teminat)
                {
                    if (ModelState["Teminat.Yurt_Disi_Teminat_Ulke"] != null)
                        ModelState["Teminat.Yurt_Disi_Teminat_Ulke"].Errors.Clear();
                }
                if (!model.Teminat.Kullanici_Teminat)
                {
                    if (ModelState["Teminat.Kullanici_TCKN"] != null)
                        ModelState["Teminat.Kullanici_TCKN"].Errors.Clear();
                    if (ModelState["Teminat.Kullanici_Adi"] != null)
                        ModelState["Teminat.Kullanici_Adi"].Errors.Clear();
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
                if (ModelState["Musteri.Sigortali.AcikAdres"] != null)
                    ModelState["Musteri.Sigortali.AcikAdres"].Errors.Clear();
                if (ModelState["Musteri.SigortaEttiren.AcikAdres"] != null)
                    ModelState["Musteri.SigortaEttiren.AcikAdres"].Errors.Clear();

                if (ModelState["Teminat.IkameTuru"] != null)
                    ModelState["Teminat.IkameTuru"].Errors.Clear();

                ModelStateMusteriClear(ModelState, model.Musteri);

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreKaskoController.Hesapla", ex);
            }

            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    #region Arac Değeri Kontrol
                    // ==== Arac Değeri kontrol ediliyor ==== //
                    if (!AracDegerKontrolServer(model.Arac))
                    { return Json(new { id = 0, hata = "Arac değeri 10% den fazla değiştirilemez" }); }
                    #endregion

                    int tvmKodu = _AktifKullaniciService.TVMKodu;
                    if ((_AktifKullaniciService.MapfreBolge || _AktifKullaniciService.MapfreMerkez) && model.Hazirlayan.TVMKodu.HasValue)
                    {
                        tvmKodu = model.Hazirlayan.TVMKodu.Value;
                    }

                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.MapfreKasko,
                                                                         tvmKodu,
                                                                         _AktifKullaniciService.KullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);
                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Arac Bilgileri
                    // ==== Araç Bilgileri ==== //
                    teklif.Arac.PlakaKodu = model.Arac.PlakaKodu;
                    teklif.Arac.PlakaNo = model.Arac.PlakaNo.ToUpperInvariant();
                    teklif.Arac.Marka = model.Arac.MarkaKodu;
                    teklif.Arac.AracinTipi = model.Arac.TipKodu;
                    teklif.Arac.Model = model.Arac.Model;
                    teklif.Arac.KullanimSekli = model.Arac.KullanimSekliKodu;
                    teklif.Arac.KullanimTarzi = model.Arac.KullanimTarziKodu;
                    teklif.Arac.TrafikTescilTarihi = model.Arac.TrafikTescilTarihi;
                    teklif.Arac.TrafikCikisTarihi = model.Arac.TrafigeCikisTarihi;
                    teklif.Arac.MotorNo = model.Arac.MotorNo;
                    teklif.Arac.SasiNo = model.Arac.SaseNo;
                    teklif.Arac.TescilSeriKod = model.Arac.TescilBelgeSeriKod;
                    teklif.Arac.TescilSeriNo = model.Arac.TescilBelgeSeriNo;
                    teklif.Arac.AsbisNo = model.Arac.AsbisNo;
                    teklif.Arac.TescilIlKodu = model.Arac.TescilIl;
                    teklif.Arac.TescilIlceKodu = model.Arac.TescilIlce;
                    teklif.Arac.TrafikCikisTarihi = model.Arac.TrafigeCikisTarihi;
                    teklif.Arac.TrafikTescilTarihi = model.Arac.TrafikTescilTarihi;

                    // ==== Arac değeri ve kişi sayısı ekleniyor ==== //
                    teklif.Arac.KoltukSayisi = model.Arac.KisiSayisi;
                    if (!String.IsNullOrEmpty(model.Arac.AracDeger))
                        teklif.Arac.AracDeger = Convert.ToDecimal(model.Arac.AracDeger.Replace(".", "").Replace(",", ""));
                    #endregion

                    #region EskiPoliçe
                    // ==== Eski Poliçe ==== //
                    teklif.AddSoru(KaskoSorular.Eski_Police_VarYok, model.EskiPolice.EskiPoliceVar);
                    if (model.EskiPolice.EskiPoliceVar)
                    {
                        teklif.AddSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, model.EskiPolice.SigortaSirketiKodu);
                        teklif.AddSoru(KaskoSorular.Eski_Police_Acente_No, model.EskiPolice.AcenteNo);
                        teklif.AddSoru(KaskoSorular.Eski_Police_No, model.EskiPolice.PoliceNo);
                        teklif.AddSoru(KaskoSorular.Eski_Police_Yenileme_No, model.EskiPolice.YenilemeNo);

                        // ==== Hasarsızlık Bilgileri //
                        if (!String.IsNullOrEmpty(model.Arac.HasarsizlikIndirim))
                            teklif.AddSoru(KaskoSorular.HasarsizlikIndirim, model.Arac.HasarsizlikIndirim);
                        if (!String.IsNullOrEmpty(model.Arac.HasarSurprim))
                            teklif.AddSoru(KaskoSorular.HasarSurprim, model.Arac.HasarSurprim);
                        if (!String.IsNullOrEmpty(model.Arac.UygulananKademe))
                            teklif.AddSoru(KaskoSorular.UygulananKademe, model.Arac.UygulananKademe);
                    }
                    #endregion

                    #region Taşıma Yetki Belgesi
                    // ==== Taşıma yetki belgesi ve taşıyıcı sorumluluk ==== //
                    teklif.AddSoru(KaskoSorular.Tasima_Yetki_Belgesi_VarYok, model.Tasiyici.YetkiBelgesi);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_VarYok, model.Tasiyici.Sorumluluk);
                    if (model.Tasiyici.Sorumluluk)
                    {
                        teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, model.Tasiyici.SigortaSirketiKodu);
                        teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Acente_No, model.Tasiyici.AcenteNo);
                        teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Police_No, model.Tasiyici.PoliceNo);
                        teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Yenileme_No, model.Tasiyici.YenilemeNo);
                    }
                    #endregion

                    #region Dain-i Mürtehin
                    teklif.AddSoru(KaskoSorular.DainiMurtein_VarYok, model.DainiMurtein.DainiMurtein);
                    if (model.DainiMurtein.DainiMurtein && model.DainiMurtein.KurumTipi > 0)
                    {
                        teklif.AddSoru(KaskoSorular.DainiMurtein_KurumTipi, model.DainiMurtein.KurumTipi.ToString());
                        teklif.AddSoru(KaskoSorular.DainiMurtein_KurumKodu, model.DainiMurtein.KurumKodu);
                        teklif.AddSoru(KaskoSorular.DainiMurtein_SubeKodu, model.DainiMurtein.SubeKodu);
                        teklif.AddSoru(KaskoSorular.DainiMurtein_KimlikNo, model.DainiMurtein.KimlikNo);
                        teklif.AddSoru(KaskoSorular.DainiMurtein_Unvan, model.DainiMurtein.Unvan);
                    }
                    #endregion

                    #region Teminatlar
                    //Sorular
                    //teklif.AddSoru(KaskoSorular.Trafige_Cikis_Tarihi, model.Arac.TrafigeCikisTarihi);
                    teklif.AddSoru(KaskoSorular.Police_Baslangic_Tarihi, model.Arac.PoliceBaslangicTarihi);

                    // ==== Zorunlu Teminatlar ==== //
                    teklif.AddSoru(KaskoSorular.Teminat_AMS_Kodu, model.Teminat.AMSKodu);
                    teklif.AddSoru(KaskoSorular.Teminat_Olum_Sakatlik, model.Teminat.OlumSakatlikTeminat);

                    // ==== Tedavi Teminatı ==== //
                    teklif.AddSoru(KaskoSorular.Teminat_Tedavi_VarYok, model.Teminat.Tedavi);
                    teklif.AddSoru(KaskoSorular.Teminat_Tedavi_Tutar, model.Teminat.TedaviTeminat);

                    // ==== İsteğe bağlı teminatlar ==== //
                    teklif.AddSoru(KaskoSorular.Deprem_VarYok, model.Teminat.Deprem);
                    teklif.AddSoru(KaskoSorular.Teminat_Deprem_Muafiyet_Kodu, model.Teminat.DepremMuafiyetKodu);
                    teklif.AddSoru(KaskoSorular.Eskime_VarYok, model.Teminat.Eskime_Payi_Teminati);
                    teklif.AddSoru(KaskoSorular.Anahtar_Kaybi_VarYok, model.Teminat.Anahtar_Kaybi_Teminati);
                    teklif.AddSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, model.Teminat.Ozel_Esya_Teminati);
                    teklif.AddSoru(KaskoSorular.Teminat_Anahtarla_Calinma_VarYok, model.Teminat.Anahtarla_Calinma_Teminati);
                    teklif.AddSoru(KaskoSorular.FiloPolice, model.Teminat.FiloPolice_Teminati);

                    // ==== Hukuksal Koruma Zorunlu ==== //
                    teklif.AddSoru(KaskoSorular.Hukuksal_Koruma_VarYok, model.Teminat.Hukuksal_Koruma_Teminati);

                    // ==== Yurtdışı Teminat Süresi ==== //
                    teklif.AddSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, model.Teminat.Yutr_Disi_Teminat);
                    if (model.Teminat.Yutr_Disi_Teminat)
                    {
                        teklif.AddSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, model.Teminat.Yurt_Disi_Teminati_Sure.ToString());
                        teklif.AddSoru(KaskoSorular.Yurt_Disi_Teminati_Ulke, model.Teminat.Yurt_Disi_Teminat_Ulke);
                    }

                    // ==== Araç Kullanıcı Bilgileri ==== //
                    teklif.AddSoru(KaskoSorular.Arac_Kullanici_Teminat, model.Teminat.Kullanici_Teminat);
                    if (model.Teminat.Kullanici_Teminat)
                    {
                        teklif.AddSoru(KaskoSorular.Arac_Kullanici_TCKN, model.Teminat.Kullanici_TCKN);
                        teklif.AddSoru(KaskoSorular.Arac_Kullanici_Adi, model.Teminat.Kullanici_Adi);
                    }

                    // ==== Onarım Yeri ==== //
                    teklif.AddSoru(KaskoSorular.OnarimYeri, model.Teminat.OnarimYeri);

                    // ==== İkame Türü ==== //
                    teklif.AddSoru(KaskoSorular.Ikame_Turu, model.Teminat.IkameTuru);

                    // ==== Ek aksesuarlar ==== //
                    if (model.Teminat.Aksesuar_Teminati && model.Teminat.Aksesuarlar.Count > 0)
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_Ekstra_Aksesuar_VarYok, model.Teminat.Aksesuar_Teminati);

                        foreach (var item in model.Teminat.Aksesuarlar)
                        {
                            teklif.AddAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.AKSESUAR,
                                                item.AksesuarTip, item.Aciklama, item.Bedel, 0);
                        }
                    }

                    // ==== Elektronik cihazlar ==== //
                    if (model.Teminat.ElektronikCihaz_Teminati && model.Teminat.Cihazlar.Count > 0)
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_ElektronikCihaz_VarYok, model.Teminat.ElektronikCihaz_Teminati);

                        foreach (var item in model.Teminat.Cihazlar)
                        {
                            teklif.AddAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ,
                                        item.AksesuarTip, item.Aciklama, item.Bedel, 0);
                        }
                    }

                    // ==== Taşınan yük teminatları ==== //
                    if (model.Teminat.TasinanYuk_Teminati && model.Teminat.TasinanYukler.Count > 0)
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, model.Teminat.TasinanYuk_Teminati);

                        foreach (var item in model.Teminat.TasinanYukler)
                        {
                            teklif.AddAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.TASINAN_YUK,
                                        item.TasinanYukTip, item.Aciklama, item.Bedel, item.Fiyat);
                        }
                    }

                    #endregion

                    #region İndirim / Surprim
                    if (model.IndirimSurprim != null && model.IndirimSurprim.IndirimOrani > 0)
                    {
                        teklif.AddSoru(KaskoSorular.IndirimSurprimTipi, model.IndirimSurprim.IndirimTipi);

                        decimal oran = model.IndirimSurprim.IndirimOrani.HasValue ? model.IndirimSurprim.IndirimOrani.Value : 0;
                        teklif.AddSoru(KaskoSorular.IndirimSurprimOrani, oran);
                    }
                    #endregion

                    #region Açıklama
                    if (!String.IsNullOrEmpty(model.Aciklama))
                    {
                        teklif.GenelBilgiler.TeklifNot = new TeklifNot();
                        teklif.GenelBilgiler.TeklifNot.Aciklama = model.Aciklama;
                    }
                    #endregion

                    #region Kasko-Jet için ürün kodu sorgulama
                    string responseUrunKodu = "";
                    if (model.Teminat.OnarimYeri == "G")
                    {
                        UrunKoduSorguModel urunKoduSorgu = new UrunKoduSorguModel();
                        if (model.Musteri.SigortaliAyni && model.Musteri.SigortaEttiren.MusteriKodu.HasValue)
                            urunKoduSorgu.kimlikNo = model.Musteri.SigortaEttiren.KimlikNo;
                        else if (model.Musteri.Sigortali.MusteriKodu.HasValue)
                            urunKoduSorgu.kimlikNo = model.Musteri.Sigortali.KimlikNo;

                        if (model.EskiPolice.EskiPoliceVar)
                        {
                            urunKoduSorgu.acenteNo = model.EskiPolice.AcenteNo;
                            urunKoduSorgu.policeNo = model.EskiPolice.PoliceNo;
                            urunKoduSorgu.sirketNo = model.EskiPolice.SigortaSirketiKodu;
                            urunKoduSorgu.yenilemeNo = model.EskiPolice.YenilemeNo;
                        }

                        urunKoduSorgu.plakaIlKodu = model.Arac.PlakaKodu.PadLeft(3, '0');
                        urunKoduSorgu.plakaNo = model.Arac.PlakaNo;

                        urunKoduSorgu.markaKodu = model.Arac.MarkaKodu;
                        urunKoduSorgu.markaTipi = model.Arac.TipKodu;

                        string[] parts = model.Arac.KullanimTarziKodu.Split('-');
                        if (parts.Length == 2)
                        {
                            CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                            if (tarz != null)
                                urunKoduSorgu.kullanimTarzi = tarz.TarifeKodu;
                        }

                        urunKoduSorgu.modelYili = model.Arac.Model.ToString();
                        urunKoduSorgu.aracRuhsatSeri = model.Arac.TescilBelgeSeriKod;
                        urunKoduSorgu.aracRuhsatSeriNo = model.Arac.TescilBelgeSeriNo;
                        urunKoduSorgu.asbisReferansNo = model.Arac.AsbisNo;
                        urunKoduSorgu.yerAdedi = model.Arac.KisiSayisi.ToString();
                        urunKoduSorgu.onarimYeri = model.Teminat.OnarimYeri;

                        IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                        UrunSorguResponse urunSorguSonuc = sorguService.UrunKoduSorgu(urunKoduSorgu);

                        // Onarım yeri G seçilebilmesi için urunkodu'nun 2AA gelmesi gerekiyor.
                        if ((urunSorguSonuc != null && !String.IsNullOrEmpty(urunSorguSonuc.urunkodu) && urunSorguSonuc.urunkodu != "2AA"))
                        {
                            return Json(new { id = 0, hata = "Kasko Jet Servisleri bu araç için seçilemez. Lütfen onarım yeri olarak Tüm Servisler seçiniz." });
                        }
                        if (urunSorguSonuc != null && !String.IsNullOrEmpty(urunSorguSonuc.hata) && urunSorguSonuc.urunkodu == null)
                        {
                            return Json(new { id = 0, hata = "Kasko Jet Servisleri bu araç için seçilemez. Lütfen onarım yeri olarak Tüm Servisler seçiniz. Hata: " + urunSorguSonuc.hata });
                        }
                        if (urunSorguSonuc.urunkodu != null)
                            responseUrunKodu = urunSorguSonuc.urunkodu;
                    }

                    #region Ikame türü sorgu
                    if (model.Arac.MarkaKodu == "009")
                    {
                        UrunKoduSorguModel ikameSorgu = new UrunKoduSorguModel();
                        ikameSorgu.plakaIlKodu = model.Arac.PlakaKodu.PadLeft(3, '0');
                        ikameSorgu.plakaNo = model.Arac.PlakaNo;
                        ikameSorgu.aracRuhsatSeri = model.Arac.TescilBelgeSeriKod;
                        ikameSorgu.aracRuhsatSeriNo = model.Arac.TescilBelgeSeriNo;
                        ikameSorgu.asbisReferansNo = model.Arac.AsbisNo;

                        string[] ikameParts = model.Arac.KullanimTarziKodu.Split('-');
                        if (ikameParts.Length == 2)
                        {
                            CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, ikameParts[0], ikameParts[1]);
                            if (tarz != null)
                                ikameSorgu.kullanimTarzi = tarz.TarifeKodu;
                        }
                        ikameSorgu.yerAdedi = model.Arac.KisiSayisi.ToString();
                        ikameSorgu.modelYili = model.Arac.Model.ToString();
                        ikameSorgu.urunKodu = responseUrunKodu;
                        ikameSorgu.ikameTuru = model.Teminat.IkameTuru;
                        ikameSorgu.markaKodu = model.Arac.MarkaKodu;
                        ikameSorgu.markaTipi = model.Arac.TipKodu;

                        IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                        KaskoIkameResponse ikameResponse = sorguService.KaskoIkameSorgu(ikameSorgu);

                        // Onarım yeri G seçilebilmesi için urunkodu'nun 2AA gelmesi gerekiyor.
                        if ((ikameResponse != null && !String.IsNullOrEmpty(ikameResponse.hata)))
                        {
                            return Json(new { id = 0, hata = "İkame Türü Hatası : " + ikameResponse.hata });
                        }
                    }
                    #endregion


                    #endregion

                    #region Teklif return

                    IMapfreKaskoTeklif kaskoTeklif = new MapfreKaskoTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    kaskoTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.MAPFRE);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        kaskoTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        kaskoTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = kaskoTeklif.Hesapla(teklif);
                    ITeklif hesaplanan = _TeklifService.GetTeklif(isDurum.ReferansId);

                    MapfreTeklifDurumModel fiyatModel = new MapfreTeklifDurumModel();
                    IsDurumDetay detay = isDurum.IsDurumDetays.FirstOrDefault();

                    ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
                    fiyatModel.teklif = this.MapfreTeklifFiyat(TeklifUretimMerkezleri.MAPFRE, detay);

                    if (hesaplanan != null)
                    {
                        fiyatModel.teklifId = hesaplanan.GenelBilgiler.TeklifId;
                        fiyatModel.teklifNo = teklif.GenelBilgiler.TeklifNo.ToString();
                        fiyatModel.pdf = teklif.GenelBilgiler.PDFDosyasi;
                        fiyatModel.mapfreTeklifNo = fiyatModel.teklif.MapfreTeklifNo;
                    }
                    #endregion

                    string html = RenderViewToString(this.ControllerContext, "_TeklifFiyat", fiyatModel);
                    return Json(new { id = isDurum.IsId, html = html });
                }

                catch (Exception ex)
                {
                    _LogService.Error("MapfreKaskoController.Hesapla", ex);

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
            }
            #endregion

            #region Hata Log

            StringBuilder sVal = new StringBuilder();
            foreach (var key in ModelState.Keys)
            {
                ModelState state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    if (sVal.Length > 0) sVal.Append(", ");
                    sVal.AppendFormat("key {0}", key);
                }
            }

            if (sVal.Length > 0)
            {
                return Json(new { id = 0, hata = "Validasyon başarısız. Hatalı alanlar : " + sVal.ToString() });
            }

            #endregion

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        public ActionResult Police(int id)
        {
            DetayMapfreKaskoModel model = new DetayMapfreKaskoModel();

            #region Teklif Genel

            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif kaskoTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = teklif.GenelBilgiler.TeklifId;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(kaskoTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(kaskoTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Teklif Arac

            model.Arac = this.MapfreDetayAracBilgiModel(kaskoTeklif);

            #endregion

            #region Eski Police

            //Eski poliçe bilgileri
            model.EskiPolice = base.DetayEskiPoliceModel(kaskoTeklif);

            #endregion

            #region Tasıyıcı Sorumluluk

            //Taşıyıcı sorumluluk bilgileri
            model.Tasiyici = base.DetayTasiyiciSorumlulukModel(kaskoTeklif);

            #endregion

            #region Dain-i Mürtein
            model.DainiMurtein = new MapfreKaskoDainiMurtein();
            model.DainiMurtein.DainiMurtein = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
            if (model.DainiMurtein.DainiMurtein)
            {
                model.DainiMurtein.KimlikNo = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);
                model.DainiMurtein.Unvan = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_Unvan, String.Empty);
                string kurumTipi = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_KurumTipi, String.Empty);
                if (!String.IsNullOrEmpty(kurumTipi))
                {
                    int kurumTipiId = 0;
                    if (int.TryParse(kurumTipi, out kurumTipiId))
                    {
                        model.DainiMurtein.KurumTipi = kurumTipiId;
                        model.DainiMurtein.KurumKodu = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_KurumKodu, String.Empty);
                        model.DainiMurtein.SubeKodu = kaskoTeklif.ReadSoru(KaskoSorular.DainiMurtein_SubeKodu, String.Empty);

                        switch (kurumTipiId)
                        {
                            case 1: model.DainiMurtein.KurumTipiAdi = "BANKA"; break;
                            case 2: model.DainiMurtein.KurumTipiAdi = "FİNANSAL KURUM"; break;
                            case 3: model.DainiMurtein.KurumTipiAdi = "DİĞER"; break;
                        }

                        CR_KaskoDM kurum = _CRService.GetKaskoDM(TeklifUretimMerkezleri.MAPFRE, kurumTipiId, model.DainiMurtein.KurumKodu);
                        if (kurum != null)
                        {
                            model.DainiMurtein.KurumAdi = kurum.KurumAdi;
                        }
                    }
                }
            }
            #endregion

            #region Açıklama
            if (kaskoTeklif.GenelBilgiler.TeklifNot != null)
            {
                model.Aciklama = kaskoTeklif.GenelBilgiler.TeklifNot.Aciklama;
            }
            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = base.KaskoPoliceOdemeModel(teklif);
            model.OdemeBilgileri.BilgilendirmePDF = teklif.GenelBilgiler.PDFBilgilendirme;
            model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFGenelSartlari;
            model.OdemeBilgileri.DekontPDFGoster = teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti;
            #endregion

            #region Teminatlar

            // ==== İsteğe bağlı ve zorunlu teminatlar dolduruluyor ==== //
            model.Teminat = TeklifTeminatDoldur(kaskoTeklif);

            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult GetKaskoAMSListe(string kullanimTarzi)
        {
            string[] kodlar = kullanimTarzi.Split('-');
            string kod1 = kodlar[0];
            string kod2 = kodlar[1];

            List<KeyValueItem<string, string>> amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, kod1, kod2);

            List<SelectListItem> model = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();

            return Json(model, JsonRequestBehavior.AllowGet);
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
                log.Error("MapfreKaskoController.PlakaSorgula", ex);
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
                HasarsizlikResponse hasarsizlik = sorguService.HasarsizlikSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, "420");
                if (hasarsizlik != null)
                {
                    plakaSorgu.HasarsizlikInd = hasarsizlik.hasarsizlik_ind;
                    plakaSorgu.HasarsizlikSur = hasarsizlik.hasar_srp;
                    plakaSorgu.HasarsizlikKademe = hasarsizlik.uygulanacak_kademe;
                }

                if (sigortaSirketi == "050" || sigortaSirketi == "50")
                {
                    OncekiTescilResponse tescil = sorguService.OncekiTescilSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, "420");
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
                log.Error("MapfreKaskoController.EskiPoliceSorgula", ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult HasarsizlikSorgula(string kimlikNo, string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo, string bransKodu)
        {
            try
            {
                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                HasarsizlikResponse hasarsizlik = sorguService.HasarsizlikSorgu(kimlikNo, policeNo, acenteNo, sigortaSirketi, yenilemeNo, bransKodu);
                if (hasarsizlik != null)
                {
                    return Json(new
                    {
                        success = true,
                        HasarsizlikInd = hasarsizlik.hasarsizlik_ind,
                        HasarsizlikSur = hasarsizlik.hasar_srp,
                        HasarsizlikKademe = hasarsizlik.uygulanacak_kademe
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "Hasarsızlık bilgileri alınamadı." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreKaskoController.HasarsizlikSorgula", ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult EgmSorgu(EgmSorgulaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.PlakaNo = model.PlakaNo.ToUpperInvariant();
                    model.PlakaKodu = model.PlakaKodu.PadLeft(3, '0');
                    PlakaSorgu plakaSorgu = MAPFREEgmSorgula(model);

                    return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                }

                throw new Exception("EGM sorgulama servisi çalıştırılırken hata oluştu.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreKaskoController.EgmSorgu", ex);
                throw;
            }
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
                    IMAPFREProjeKasko urun = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeKasko;
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
                _LogService.Error("MapfreKaskoController.BilgilendirmePDF", ex);
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
                    IMAPFREProjeKasko urun = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeKasko;
                    urun.DekontPDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFGenelSartlari))
                {
                    throw new Exception("Dekont pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreKaskoController.DekontPDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFGenelSartlari;
            return Json(new { Success = success, PDFUrl = url });
        }

        [HttpPost]
        [AjaxException]
        public ActionResult OtorizasyonMesajGonder(int teklifId, string mesaj)
        {
            try
            {
                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                TeklifGenel teklifGenel = _TeklifService.GetTeklifGenel(teklifId);

                List<ITeklif> teklifler = _TeklifService.GetTeklifListe(teklifGenel.TeklifNo, teklifGenel.TVMKodu);

                var teklif = teklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

                string mapfreTeklifNo = teklif.GenelBilgiler.TUMTeklifNo;

                OtorizasyonMesajResponse response = sorguService.TeklifOtorizasyonMesaji(mapfreTeklifNo, mesaj);

                if (response != null)
                {
                    if (response.row == "OK")
                    {
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }

                    if (!String.IsNullOrEmpty(response.hata))
                        return Json(new { success = false, message = response.hata }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreKaskoController.OtorizasyonMesajGonder", ex);
                throw;
            }
        }

        [AjaxException]
        public ActionResult KaskoDMSorgula(int kurumTipi)
        {
            try
            {
                var kurumlar = new SelectList(_CRService.GetKaskoDMListe(TeklifUretimMerkezleri.MAPFRE, kurumTipi), "KurumKodu", "KurumAdi");
                return Json(kurumlar, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("MapfreKaskoController.PlakaSorgula", ex);
                throw;
            }
        }

        public ActionResult OtorizasyonMesajlari(int teklifId)
        {
            ITeklif teklif = _TeklifService.GetTeklif(teklifId);

            IMapfreKaskoTeklif kaskoTeklif = new MapfreKaskoTeklif(teklifId);
            IsDurum isDurum = kaskoTeklif.GetIsDurum();
            MapfreTeklifDurumModel fiyatModel = new MapfreTeklifDurumModel();
            fiyatModel.teklifId = kaskoTeklif.Teklif.GenelBilgiler.TeklifId;
            fiyatModel.teklifNo = kaskoTeklif.Teklif.GenelBilgiler.TeklifNo.ToString();
            IsDurumDetay detay = isDurum.IsDurumDetays.FirstOrDefault();
            fiyatModel.teklif = this.MapfreTeklifFiyat(TeklifUretimMerkezleri.MAPFRE, detay);

            fiyatModel.mapfreTeklifNo = fiyatModel.teklif.MapfreTeklifNo;

            if (teklif.SigortaEttiren != null)
                fiyatModel.SigortaEttirenKodu = teklif.SigortaEttiren.MusteriKodu;

            string html = RenderViewToString(this.ControllerContext, "_OtorizasyonMesajlari", fiyatModel);
            return Json(new { id = isDurum.IsId, html = html });
        }

        public ActionResult OtorizasyonSorgula(int teklifId)
        {
            IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();

            OtorizasyonResponse response = sorguService.OtorizasyonSorgu(teklifId);

            ITeklif teklif = _TeklifService.GetTeklif(teklifId);
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            response.TeklifId = anaTeklif.GenelBilgiler.TeklifId;
            if (teklif != null && teklif.SigortaEttiren != null)
            {
                response.MusteriKodu = teklif.SigortaEttiren.MusteriKodu;
            }

            string html = RenderViewToString(this.ControllerContext, "_OtorizasyonDurumu", response);
            return Json(new { succes = true, html = html });
        }

        public ActionResult GetAracDegerKisiSayisi(string TipKodu, string MarkaKodu, int Model, string KullanimTarzi,
                                                   string PlakaKodu, string PlakaNo, string AracRuhsatSeriNo,
                                                   string AracRuhsatNo, string AsbisNo)
        {
            decimal aracDeger = _AracService.GetAracDeger(MarkaKodu, TipKodu, Model);
            short kisiSayisi = 0;
            bool egmSorguSuccess = false;

            if (PlakaNo != "YK" && (!String.IsNullOrEmpty(AracRuhsatSeriNo) || !String.IsNullOrEmpty(AracRuhsatNo) || !String.IsNullOrEmpty(AsbisNo)))
            {
                egmSorguSuccess = true;
                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                EgmSorguResponse response = sorguService.EgmSorgu(_AktifKullaniciService.TVMKodu, PlakaKodu, PlakaNo, AracRuhsatSeriNo, AracRuhsatNo, AsbisNo);

                if (response == null)
                    egmSorguSuccess = false;
                if (egmSorguSuccess && !String.IsNullOrEmpty(response.hata))
                    egmSorguSuccess = false;
                if (egmSorguSuccess && response.aracBilgi == null)
                    egmSorguSuccess = false;

                if (egmSorguSuccess)
                {
                    string aracKoltukSayisi = String.Empty;
                    if (response.aracBilgi.koltukSayisi.Contains(".0"))
                        aracKoltukSayisi = response.aracBilgi.koltukSayisi.Replace(".0", "");
                    else
                        aracKoltukSayisi = response.aracBilgi.koltukSayisi;

                    int koltukSayisi = 0;
                    int.TryParse(aracKoltukSayisi, out koltukSayisi);

                    if (koltukSayisi > 0)
                        kisiSayisi = (short)koltukSayisi;
                    else
                        egmSorguSuccess = false;
                }
            }

            if (!egmSorguSuccess)
            {
                kisiSayisi = _AracService.GetAracKisiSayisi(MarkaKodu, TipKodu);
            }

            AracDegerKisiSayisiModel model = new AracDegerKisiSayisiModel();
            model.AracDeger = aracDeger;
            model.KisiSayisi = kisiSayisi;
            model.EgmSorgu = egmSorguSuccess;

            // ==== IMM ve FK getiriliyor ===== //
            string[] kullanimtarzi = KullanimTarzi.Split('-');

            if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                model.ProjeKodu = _AktifKullaniciService.ProjeKodu;

                List<KeyValueItem<string, string>> amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, kullanimtarzi[0], kullanimtarzi[1]);
                model.IMMList = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();
            }
            else
            {
                List<KeyValueItem<string, string>> FKListesi = _CRService.GetKaskoFKList(1, kullanimtarzi[0], kullanimtarzi[1]);
                List<KeyValueItem<string, string>> IMMList = _CRService.GetKaskoIMMList(1, kullanimtarzi[0], kullanimtarzi[1]);

                model.FKList = new SelectList(FKListesi, "Key", "Value", "").ListWithOptionLabel();
                model.IMMList = new SelectList(IMMList, "Key", "Value", "").ListWithOptionLabel();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracDegerKontrol(decimal AracDeger, string TipKodu, string MarkaKodu, int Model)
        {
            AracDegerKontrolModel model = new AracDegerKontrolModel();
            decimal orjinalDeger = _AracService.GetAracDeger(MarkaKodu, TipKodu, Model);

            if (orjinalDeger > AracDeger)
            {
                model.Result = false;
                model.OrjinalDeger = orjinalDeger;
                model.message = "Araç fiyatı, aracın kasko değerinden küçük olamaz";
            }
            else
            {
                model.Result = true;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracMarkaGetir(string KullanimTarziKodu)
        {
            string[] parts = KullanimTarziKodu.Split('-');
            KullanimTarziKodu = parts[0];
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<string> kodlar = new List<string>();
            kodlar.Add(KullanimTarziKodu);
            if (KullanimTarziKodu == "111")
            {
                kodlar.Add("121");
            }

            List<AracMarka> markalar = _AracService.GetAracMarkaList(kodlar.ToArray());

            return Json(new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracTipiGetir(string MarkaKodu)
        {
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();

            List<AracTip> tipler = _AracService.GetAracTipList(MarkaKodu);

            return Json(new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AracTipleri(string q, int page_limit, int page, string marka)
        {
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();

            List<AracTip> tipler = _AracService.GetAracTipList(marka);

            var count = tipler.Where(w => w.TipAdi.Contains(q)).Count();

            var liste = tipler.Where(w => w.TipAdi.Contains(q))
                              .Skip(page_limit * page)
                              .Take(page_limit)
                              .Select(s => new { id = s.TipKodu, text = s.TipAdi })
                              .AsEnumerable();

            return Json(new { results = liste, total = count });
        }

        public ActionResult Otorizasyon()
        {
            MapfreOtorizasyonModel model = new MapfreOtorizasyonModel();

            model.BaslangicTarihi = TurkeyDateTime.Today;
            model.BitisTarihi = TurkeyDateTime.Today;

            return View(model);
        }

        public ActionResult OtorizasyonListe()
        {
            if (Request["sEcho"] != null)
            {
                TeklifOtorizasyonListe arama = new TeklifOtorizasyonListe(Request, new Expression<Func<TeklifOtorizasyonTableModel, object>>[]
                                                                    {
                                                                        t => t.TeklifNo,
                                                                        t => t.TVMUnvani,
                                                                        t => t.TanzimTarihi,
                                                                        t => t.TVMKullaniciAdSoyad,
                                                                        t => t.UrunAdi,
                                                                        t => t.MusteriAdSoyad,
                                                                        t => t.OzelAlan,
                                                                        t => t.TUMTeklifNo
                                                                    });

                arama.TVMKodu = _AktifKullaniciService.TVMKodu;
                arama.TeklifNo = arama.TryParseParamInt("TeklifNo");
                arama.MapfreTeklifNo = arama.TryParseParamString("MapfreTeklifNo");
                arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");

                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.ToString()));
                arama.AddFormatter(f => f.MusteriAdSoyad, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdSoyad));
                arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a>",
                                                                        "/Teklif/MapfreKasko/OtorizasyonOnay/", f.TeklifId, f.TeklifNo));
                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", RaporController.GetOzelAlan(s.TeklifId)));

                int totalRowCount = 0;
                List<TeklifOtorizasyonTableModel> list = _TeklifService.GetOtorizasyonTeklifler(arama, out totalRowCount);

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult OtorizasyonOnay(int id)
        {
            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            IMapfreKaskoTeklif kaskoTeklif = new MapfreKaskoTeklif(anaTeklif.GenelBilgiler.TeklifId);
            MapfreTeklifDurumModel fiyatModel = new MapfreTeklifDurumModel();

            IsDurum isDurum = kaskoTeklif.GetIsDurum();
            if (isDurum != null)
            {
                fiyatModel.teklifId = anaTeklif.GenelBilgiler.TeklifId;
                fiyatModel.teklifNo = anaTeklif.GenelBilgiler.TeklifNo.ToString();
                IsDurumDetay detay = isDurum.IsDurumDetays.FirstOrDefault();
                fiyatModel.teklif = this.MapfreTeklifFiyat(TeklifUretimMerkezleri.MAPFRE, detay);

                fiyatModel.mapfreTeklifNo = fiyatModel.teklif.MapfreTeklifNo;

                if (anaTeklif.SigortaEttiren != null)
                    fiyatModel.SigortaEttirenKodu = teklif.SigortaEttiren.MusteriKodu;
            }
            else
            {
                fiyatModel.hata = true;
                fiyatModel.mesaj = "Teklif bilgileri alınamadı.";
            }

            return View(fiyatModel);
        }

        [HttpPost]
        public ActionResult OtorizasyonOnayVer(int id)
        {
            OtorizasyonResponse otor = null;

            ITeklif teklif = _TeklifService.GetTeklif(id);
            if (teklif.UrunKodu == UrunKodlari.MapfreKasko)
            {
                IMAPFREProjeKasko kasko = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeKasko;
                otor = kasko.OtorizasyonOnay();
            }
            else
            {
                IMAPFREProjeTrafik kasko = TeklifUrunFactory.AsUrunClass(teklif) as IMAPFREProjeTrafik;
                otor = kasko.OtorizasyonOnay();
            }

            if (otor.Basarili)
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
                return Json(new { success = true, redirectUrl = TeklifSayfaAdresleri.DetayAdres(teklif.UrunKodu) + anaTeklif.GenelBilgiler.TeklifId.ToString() });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DainiKurum(string adSoyad)
        {
            try
            {
                var kurum = _CRService.GetKaskoDMAd(TeklifUretimMerkezleri.MAPFRE, adSoyad);

                if (kurum != null)
                {
                    var kurumlar = new SelectList(_CRService.GetKaskoDMListe(TeklifUretimMerkezleri.MAPFRE, kurum.KurumTipi), "KurumKodu", "KurumAdi", kurum.KurumKodu);
                    return Json(new { success = true, KurumTipi = kurum.KurumTipi, KurumKodu = kurum.KurumKodu, Kurumlar = kurumlar });
                }

                return Json(new { success = false, message = "Kurum bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult IkameTurleriListe(string KullanimTarziKodu, string plakaIlKodu, string plakaNo, string aracRuhsatSeri, string aracRuhsatNo, string asbisNo, string kisiSayisi, bool egmSorgu, string markaKodu)
        {
            if (!String.IsNullOrEmpty(KullanimTarziKodu))
            {
                string[] parts = KullanimTarziKodu.Split('-');
                if (parts.Length == 2)
                {
                    CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                    if (tarz != null)
                    {
                        if (tarz.TarifeKodu == "60" &&
                           (!String.IsNullOrEmpty(aracRuhsatSeri) || !String.IsNullOrEmpty(aracRuhsatNo) || !String.IsNullOrEmpty(asbisNo) ||
                           plakaNo == "YK"))
                        {
                            try
                            {
                                int sayfaKisiSayisi = 0;
                                int.TryParse(kisiSayisi, out sayfaKisiSayisi);
                                int koltukSayi = 0;

                                if (plakaNo != "YK")
                                {
                                    IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                                    EgmSorguResponse resp = mapfreSorgu.EgmSorgu(_AktifKullaniciService.TVMKodu, plakaIlKodu, plakaNo, aracRuhsatSeri, aracRuhsatNo, asbisNo);

                                    if (resp != null && resp.aracBilgi != null &&
                                        !String.IsNullOrEmpty(resp.aracBilgi.koltukSayisi))
                                    {
                                        int.TryParse(resp.aracBilgi.koltukSayisi, out koltukSayi);
                                    }
                                }
                                if (plakaNo == "YK")
                                {
                                    koltukSayi = sayfaKisiSayisi;
                                }

                                if (koltukSayi >= 4)
                                {
                                    List<KeyValueItem<string, string>> list1 = new List<KeyValueItem<string, string>>();
                                    list1.Add(new KeyValueItem<string, string>("ABC07", "ABC SEGMENT 7 GÜN"));
                                    list1.Add(new KeyValueItem<string, string>("ABC14", "ABC SEGMENT 14 GÜN"));
                                    list1.Add(new KeyValueItem<string, string>("Z-KASKOJET", "KASKOJET IKAME"));

                                    string defaultIkameKodu = "ABC14";
                                    List<SelectListItem> items1 = new SelectList(list1, "Key", "Value").ToList<SelectListItem>();

                                    return Json(new { success = true, def = defaultIkameKodu, list = items1 }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            catch (Exception e)
                            {
                                _LogService.Error("MapfreKaskoController.IkameTurleriListe", e);
                            }
                        }

                        List<KeyValueItem<string, string>> list = _CRService.GetKaskoIkameTuruList(TeklifUretimMerkezleri.MAPFRE, tarz.TarifeKodu);

                        if (list != null && list.Count > 0)
                        {
                            string defaultIkameKodu = "0";
                            if (markaKodu == "009")
                            {
                                list.Add(new KeyValueItem<string, string>("AUDI_ABC20", "AUDI ABC SEGMENT 20 GÜN"));
                                list.Add(new KeyValueItem<string, string>("AUDI_D20", "AUDI D SEGMENT 20 GÜN"));
                            }

                            List<SelectListItem> items = new SelectList(list, "Key", "Value").ToList<SelectListItem>();
                            if (tarz.TarifeKodu == "40")
                                defaultIkameKodu = "ABC07";

                            return Json(new { success = true, def = defaultIkameKodu, list = items }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public bool AracDegerKontrolServer(MapfreKaskoAracBilgiModel model)
        {
            if (!String.IsNullOrEmpty(model.AracDeger))
            {
                decimal aracDeger = Convert.ToDecimal(model.AracDeger.Replace(".", "").Replace(",", ""));
                decimal orjinalDeger = _AracService.GetAracDeger(model.MarkaKodu, model.TipKodu, model.Model);

                if (orjinalDeger > aracDeger)
                    return false;

                return true;
            }
            else
                return false;
        }

        private DetayMapfreKaskoTeminatModel TeklifTeminatDoldur(ITeklif kaskoTeklif)
        {
            DetayMapfreKaskoTeminatModel teminat = new DetayMapfreKaskoTeminatModel();
            string amsKodu = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_AMS_Kodu, "0");

            if (!String.IsNullOrEmpty(amsKodu) && amsKodu != "0")
            {
                CR_KaskoAMS ams = _CRService.GetKaskoAMS(TeklifUretimMerkezleri.MAPFRE, kaskoTeklif.GenelBilgiler.TVMKodu, amsKodu);
                teminat.AMS = ams != null ? ams.Aciklama : String.Empty;
            }
            decimal olumSakatlik = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Olum_Sakatlik, decimal.Zero);
            if (olumSakatlik > 0)
            {
                teminat.OlumSakatlik = olumSakatlik.ToString("N2");
            }
            teminat.Tedavi = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Tedavi_VarYok, false);
            if (teminat.Tedavi)
            {
                teminat.TedaviTeminat = Convert.ToString((int)kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Tedavi_Tutar, decimal.Zero));
            }

            teminat.Deprem = kaskoTeklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
            teminat.DepremMuafiyetKodu = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Deprem_Muafiyet_Kodu, "0");
            teminat.Hukuksal_Koruma_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, true);
            teminat.Eskime_Payi_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Eskime_VarYok, true);
            teminat.Anahtar_Kaybi_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, true);
            teminat.Ozel_Esya_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, true);
            teminat.Anahtarla_Calinma_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Anahtarla_Calinma_VarYok, true);
            teminat.FiloPolice_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.FiloPolice, false);

            teminat.Kullanici_Teminat = kaskoTeklif.ReadSoru(KaskoSorular.Arac_Kullanici_Teminat, false);
            if (teminat.Kullanici_Teminat)
            {
                teminat.Kullanici_TCKN = kaskoTeklif.ReadSoru(KaskoSorular.Arac_Kullanici_TCKN, String.Empty);
                teminat.Kullanici_Adi = kaskoTeklif.ReadSoru(KaskoSorular.Arac_Kullanici_Adi, String.Empty);
            }

            teminat.OnarimYeri = kaskoTeklif.ReadSoru(KaskoSorular.OnarimYeri, "T");

            teminat.Aksesuar_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Ekstra_Aksesuar_VarYok, false);
            List<TeklifAracEkSoru> aksesuarlar = kaskoTeklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR).ToList<TeklifAracEkSoru>();
            teminat.Aksesuarlar = new List<MapfreAksesuarModel>();
            if (aksesuarlar != null && aksesuarlar.Count > 0)
            {
                List<CR_AracEkSoru> eksorular = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.AKSESUAR);
                foreach (var item in aksesuarlar)
                {
                    var soru = eksorular.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                    if (soru != null)
                    {
                        teminat.Aksesuarlar.Add(new MapfreAksesuarModel()
                        {
                            AksesuarTip = soru.SoruAdi,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel
                        });
                    }
                }
            }

            teminat.ElektronikCihaz_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_ElektronikCihaz_VarYok, false);
            List<TeklifAracEkSoru> elekCihazlar = kaskoTeklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ).ToList<TeklifAracEkSoru>();
            teminat.Cihazlar = new List<MapfreAksesuarModel>();
            if (elekCihazlar != null && elekCihazlar.Count > 0)
            {
                List<CR_AracEkSoru> cihazlar = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ);
                foreach (var item in elekCihazlar)
                {
                    var soru = cihazlar.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                    if (soru != null)
                    {
                        teminat.Cihazlar.Add(new MapfreAksesuarModel()
                        {
                            AksesuarTip = soru.SoruAdi,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel
                        });
                    }
                }
            }

            teminat.TasinanYuk_Teminati = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, false);
            List<TeklifAracEkSoru> tasinanYukler = kaskoTeklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.TASINAN_YUK).ToList<TeklifAracEkSoru>();
            teminat.TasinanYukler = new List<MapfreTasinanYukModel>();
            if (tasinanYukler != null && tasinanYukler.Count > 0)
            {
                List<CR_AracEkSoru> yukler = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.TASINAN_YUK);
                foreach (var item in tasinanYukler)
                {
                    var soru = yukler.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                    if (soru != null)
                    {
                        teminat.TasinanYukler.Add(new MapfreTasinanYukModel()
                        {
                            TasinanYukTip = soru.SoruAdi,
                            Aciklama = item.Aciklama,
                            Bedel = (int)item.Bedel,
                            Fiyat = (int)item.Fiyat
                        });
                    }
                }
            }

            // ==== Yurtdışı teminat suresi ==== //
            teminat.Yutr_Disi_Teminat = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);
            if (teminat.Yutr_Disi_Teminat)
            {
                string YurtdisiTeminatSuresi = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "0");
                if (!String.IsNullOrEmpty(YurtdisiTeminatSuresi) && YurtdisiTeminatSuresi != "0")
                {
                    var sure = TeklifListeleri.KaskoYurtDisiTeminatSureleri().FirstOrDefault(s => s.Value == YurtdisiTeminatSuresi);
                    if (sure != null)
                    {
                        teminat.Yurt_Disi_Teminati_Sure = sure.Text;
                    }
                }

                string ulkeKodu = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Ulke, String.Empty);
                if (!String.IsNullOrEmpty(ulkeKodu))
                {
                    CR_Ulke ulke = _CRService.GetUlke(TeklifUretimMerkezleri.MAPFRE, ulkeKodu);
                    if (ulke != null)
                    {
                        teminat.Yurt_Disi_Teminati_Ulke = ulke.UlkeAdi;
                    }
                }
            }

            string ikameTuruKodu = kaskoTeklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
            if (!String.IsNullOrEmpty(ikameTuruKodu))
            {
                string tarifeKodu = String.Empty;
                string[] parts = kaskoTeklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    CR_KullanimTarzi kullanimTarzi = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                    if (kullanimTarzi != null)
                        tarifeKodu = kullanimTarzi.TarifeKodu;
                }

                if (!String.IsNullOrEmpty(tarifeKodu))
                {
                    CR_KaskoIkameTuru ikameTuru = _CRService.GetKaskoIkameTuru(TeklifUretimMerkezleri.MAPFRE, tarifeKodu, ikameTuruKodu);
                    if (ikameTuru != null)
                    {
                        teminat.IkameTuru = ikameTuru.IkameTuru;
                    }
                }
            }
            return teminat;
        }

        private TeklifFiyatModel KaskoFiyat(KaskoTeklif kaskoTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = kaskoTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = kaskoTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = kaskoTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = kaskoTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = kaskoTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            if (kaskoTeklif.Teklif.GenelBilgiler.Otorizasyon.HasValue && kaskoTeklif.Teklif.GenelBilgiler.Otorizasyon.Value == 1)
            {

            }

            IsDurum durum = kaskoTeklif.GetIsDurum();
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

        private PlakaSorgu MAPFREPlakaSorgula(PlakaSorgulaModel model, MusteriGenelBilgiler musteri)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            PoliceSorguKaskoResponse response = mapfreSorgu.PoliceSorguKasko(_AktifKullaniciService.TVMKodu, musteri.KimlikNo, model.PlakaKodu, model.PlakaNo);

            if (response == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }
            if (!String.IsNullOrEmpty(response.hata))
            {
                throw new Exception(response.hata);
            }
            if (!String.IsNullOrEmpty(response.durumAciklamasi))
            {
                throw new Exception(response.durumAciklamasi);
            }
            if (response.oncekiPoliceList == null && response.yururlukPoliceList == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }
            if (response.oncekiPoliceList.TramerSorguPolice == null && response.yururlukPoliceList.TramerSorguPolice == null)
            {
                throw new Exception("Plaka sorgulanamadı.");
            }

            TramerSorguPoliceValue eskiPoliceBilgi = null;

            if (response.yururlukPoliceList != null &&
                response.yururlukPoliceList.TramerSorguPolice != null &&
                response.yururlukPoliceList.TramerSorguPolice.Length > 0)
            {
                DateTime eskiPoliceTarih = response.yururlukPoliceList.TramerSorguPolice.Where(w => w.PoliceBitisTarihi < DateTime.Today.AddDays(30))
                                                                                      .DefaultIfEmpty(new TramerSorguPoliceValue() { policeBitisTarihi = "01/01/1900" })
                                                                                      .Max(m => m.PoliceBitisTarihi);
                if (eskiPoliceTarih > DateTime.MinValue)
                    eskiPoliceBilgi = response.yururlukPoliceList.TramerSorguPolice.FirstOrDefault(w => w.PoliceBitisTarihi == eskiPoliceTarih);
            }

            if (eskiPoliceBilgi == null &&
                response.oncekiPoliceList != null &&
                response.oncekiPoliceList.TramerSorguPolice != null &&
                response.oncekiPoliceList.TramerSorguPolice.Length > 0)
            {
                DateTime eskiPoliceTarih = response.oncekiPoliceList.TramerSorguPolice.Where(w => w.PoliceBitisTarihi < DateTime.Today.AddDays(30))
                                                                                      .DefaultIfEmpty(new TramerSorguPoliceValue() { policeBitisTarihi = "01/01/1900" })
                                                                                      .Max(m => m.PoliceBitisTarihi);

                if (eskiPoliceTarih > DateTime.MinValue)
                    eskiPoliceBilgi = response.oncekiPoliceList.TramerSorguPolice.FirstOrDefault(w => w.PoliceBitisTarihi == eskiPoliceTarih);
            }

            TramerSorguPoliceValue bilgi = null;

            DateTime maxPoliceTarih = DateTime.MinValue;
            if (response.yururlukPoliceList != null &&
                    response.yururlukPoliceList.TramerSorguPolice != null &&
                    response.yururlukPoliceList.TramerSorguPolice.Length > 0)
            {
                maxPoliceTarih = response.yururlukPoliceList.TramerSorguPolice.Max(m => m.PoliceBitisTarihi);
                bilgi = response.yururlukPoliceList.TramerSorguPolice.FirstOrDefault(w => w.PoliceBitisTarihi == maxPoliceTarih);
            }
            else if (response.oncekiPoliceList != null &&
                response.oncekiPoliceList.TramerSorguPolice != null &&
                response.oncekiPoliceList.TramerSorguPolice.Length > 0)
            {
                maxPoliceTarih = response.oncekiPoliceList.TramerSorguPolice.Max(m => m.PoliceBitisTarihi);
                bilgi = response.oncekiPoliceList.TramerSorguPolice.FirstOrDefault(w => w.PoliceBitisTarihi == maxPoliceTarih);
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracMarkaKodu = bilgi.aracMarkaKodu;
            plakaSorgu.AracTipKodu = bilgi.aracTipKodu.TrimStart('0');
            plakaSorgu.AracModelYili = bilgi.modelYili;
            plakaSorgu.AracMotorNo = bilgi.motorNo;
            plakaSorgu.AracSasiNo = bilgi.sasiNo;

            if (eskiPoliceBilgi != null)
            {
                plakaSorgu.EskiPoliceSigortaSirkedKodu = eskiPoliceBilgi.sigortaSirketKodu;
                plakaSorgu.EskiPoliceAcenteKod = eskiPoliceBilgi.acenteNo;
                plakaSorgu.EskiPoliceNo = eskiPoliceBilgi.policeNo;
                plakaSorgu.EskiPoliceYenilemeNo = eskiPoliceBilgi.yenilemeNo;

                if (!String.IsNullOrEmpty(eskiPoliceBilgi.policeBitisTarihi))
                {
                    DateTime policeBitis = MapfreSorguResponse.ToDateTime(eskiPoliceBilgi.policeBitisTarihi);

                    if (policeBitis < TurkeyDateTime.Today)
                        plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                    else
                        plakaSorgu.YeniPoliceBaslangicTarih = policeBitis.ToString("dd.MM.yyyy");
                }
            }


            if (bilgi.yolcuKapasitesi.Contains(".0"))
            {
                plakaSorgu.AracKoltukSayisi = bilgi.yolcuKapasitesi.Replace(".0", "");
            }
            else
            {
                plakaSorgu.AracKoltukSayisi = bilgi.yolcuKapasitesi;
            }

            int modelYili = int.Parse(bilgi.modelYili);
            string kullanimSekli = "111";

            // Mapfre Araç Kullanım Şekilleri
            // 0 - Özel
            // 1 - Ticari
            // 2 - Resmi
            string aracKullanimSekli = "0";
            if (bilgi.kullanimSekli == "0")
                aracKullanimSekli = "2";
            else if (bilgi.kullanimSekli == "1")
                aracKullanimSekli = "0";
            else
                aracKullanimSekli = "1";

            plakaSorgu.AracKullanimSekli = aracKullanimSekli;
            plakaSorgu.AracKullanimTarzi = "111-10";

            List<CR_AracGrup> aracGruplari = _CRService.GetAracGruplari(TeklifUretimMerkezleri.MAPFRE);
            CR_AracGrup grp = aracGruplari.FirstOrDefault(f => f.TarifeKodu == bilgi.aracTarifeGrupKodu);
            if (grp != null)
            {
                kullanimSekli = grp.KullanimTarziKodu;
                plakaSorgu.AracKullanimTarzi = grp.KullanimTarziKodu + "-" + grp.Kod2;
            }
            else
            {
                AracTip tip = _AracService.GetAracTip(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu);
                if (tip != null)
                {
                    kullanimSekli = tip.KullanimSekli1;
                    plakaSorgu.AracKullanimTarzi = tip.KullanimSekli1 + "-10";
                }
            }

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracMarka> markalar = _AracService.GetAracMarkaList(kullanimSekli);
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(bilgi.marka);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);
            if (aracModel != null)
            {
                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
            }

            DateTime dateTescilTarihi = MapfreSorguResponse.ToDateTime(bilgi.trafikTescilTarihi);
            if (dateTescilTarihi != DateTime.MinValue)
            {
                plakaSorgu.AracTescilTarih = dateTescilTarihi.ToString("dd.MM.yyyy");
            }
            else
            {
                plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
            }

            List<KeyValueItem<string, string>> amsList = null;
            if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
            {
                string[] parts = plakaSorgu.AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, parts[0], parts[1]);

                }
            }
            if (amsList == null)
            {
                amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, "111", "10");
            }
            plakaSorgu.Ams = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();


            if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
            {
                string[] parts = plakaSorgu.AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                    if (tarz != null)
                    {
                        List<KeyValueItem<string, string>> list = _CRService.GetKaskoIkameTuruList(TeklifUretimMerkezleri.MAPFRE, tarz.TarifeKodu);

                        if (list != null && list.Count > 0)
                        {
                            string defaultIkameKodu = "0";
                            plakaSorgu.IkameTurleri = new SelectList(list, "Key", "Value").ToList<SelectListItem>();
                            if (tarz.TarifeKodu == "40")
                                defaultIkameKodu = "ABC07";

                            plakaSorgu.IkameTuru = defaultIkameKodu;
                        }
                    }
                }
            }

            if (plakaSorgu.EskiPoliceSigortaSirkedKodu == "050" || plakaSorgu.EskiPoliceSigortaSirkedKodu == "50")
            {
                OncekiTescilResponse tescil = mapfreSorgu.OncekiTescilSorgu(musteri.KimlikNo, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceYenilemeNo, "420");
                if (tescil != null && String.IsNullOrEmpty(tescil.hata) && tescil.COD_ARAC_RUHSAT_SERI != "*")
                {
                    plakaSorgu.TescilSeri = tescil.COD_ARAC_RUHSAT_SERI;
                    plakaSorgu.TescilSeriNo = tescil.COD_ARAC_RUHSAT_SERI_NO;
                }
            }

            return plakaSorgu;
        }

        private PlakaSorgu MAPFREEgmSorgula(EgmSorgulaModel model)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            EgmSorguResponse response = mapfreSorgu.EgmSorgu(_AktifKullaniciService.TVMKodu, model.PlakaKodu, model.PlakaNo, model.AracRuhsatSeriNo, model.AracRuhsatNo, model.AsbisNo);

            if (response == null)
            {
                throw new Exception("Araç sorgulanamadı.");
            }
            if (!String.IsNullOrEmpty(response.hata))
            {
                throw new Exception(response.hata);
            }
            if (response.aracBilgi == null)
            {
                throw new Exception("Araç sorgulanamadı.");
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracMotorNo = response.aracBilgi.motorNo;
            plakaSorgu.AracSasiNo = response.aracBilgi.sasiNo;

            if (response.aracBilgi.koltukSayisi.Contains(".0"))
            {
                plakaSorgu.AracKoltukSayisi = response.aracBilgi.koltukSayisi.Replace(".0", "");
            }
            else
            {
                plakaSorgu.AracKoltukSayisi = response.aracBilgi.koltukSayisi;
            }

            int koltukSayisi = 0;
            int.TryParse(plakaSorgu.AracKoltukSayisi, out koltukSayisi);

            if (!String.IsNullOrEmpty(response.aracBilgi.modelYili))
            {
                plakaSorgu.AracModelYili = response.aracBilgi.modelYili;
            }

            if (response.aracTescilBilgileri != null && response.aracTescilBilgileri.tescilTarihi != null)
            {
                long time = 0;
                if (long.TryParse(response.aracTescilBilgileri.tescilTarihi.time, out time))
                {
                    plakaSorgu.AracTescilTarih = MapfreSorguResponse.FromJavaTime(time).ToString("dd.MM.yyyy");
                }
            }

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            return plakaSorgu;
        }

        private PlakaSorgu MAPFREEskiPoliceSorgula(string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            EskiKaskoBilgiSorguResponse response = mapfreSorgu.EskiPoliceSorguKasko(policeNo, acenteNo, sigortaSirketi, yenilemeNo);

            if (response == null)
            {
                throw new Exception("Poliçe bilgileri sorgulanamadı.");
            }
            if (!String.IsNullOrEmpty(response.hata))
            {
                throw new Exception(response.hata);
            }
            if (response.police == null)
            {
                throw new Exception("Poliçe bilgileri sorgulanamadı.");
            }
            if (response.police.arac == null)
            {
                throw new Exception("Poliçe bilgileri sorgulanamadı.");
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracMarkaKodu = response.police.arac.aracMarkaKodu;
            plakaSorgu.AracTipKodu = response.police.arac.aracTipKodu.TrimStart('0');
            plakaSorgu.AracModelYili = response.police.arac.modelYili;
            plakaSorgu.AracMotorNo = response.police.arac.motorNo;
            plakaSorgu.AracSasiNo = response.police.arac.sasiNo;

            if (response.police.arac.plakaIlKodu.Length == 3)
                plakaSorgu.PlakaKodu = response.police.arac.plakaIlKodu.Substring(1, 2);
            else
                plakaSorgu.PlakaKodu = response.police.arac.plakaIlKodu;
            plakaSorgu.PlakaNo = response.police.arac.plakaNo;

            plakaSorgu.EskiPoliceSigortaSirkedKodu = sigortaSirketi;
            plakaSorgu.EskiPoliceAcenteKod = acenteNo;
            plakaSorgu.EskiPoliceNo = policeNo;
            plakaSorgu.EskiPoliceYenilemeNo = yenilemeNo;

            if (response.police.policeBitisTarihi != null &&
                !String.IsNullOrEmpty(response.police.policeBitisTarihi.time))
            {
                long time = Convert.ToInt64(response.police.policeBitisTarihi.time);
                DateTime datePoliceBitisTarih = MapfreSorguResponse.FromJavaTime(time);
                if (datePoliceBitisTarih > DateTime.MinValue)
                {
                    DateTime policeBitis = datePoliceBitisTarih;

                    if (policeBitis < TurkeyDateTime.Today)
                        plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                    else
                        plakaSorgu.YeniPoliceBaslangicTarih = policeBitis.ToString("dd.MM.yyyy");
                }
            }

            if (!String.IsNullOrEmpty(response.police.arac.yolcuKapasitesi))
            {
                if (response.police.arac.yolcuKapasitesi.Contains(".0"))
                    plakaSorgu.AracKoltukSayisi = response.police.arac.yolcuKapasitesi.Replace(".0", "");
                else
                {
                    plakaSorgu.AracKoltukSayisi = response.police.arac.yolcuKapasitesi;
                }
            }

            int modelYili = int.Parse(response.police.arac.modelYili);
            string kullanimSekli = "111";

            // Mapfre Araç Kullanım Şekilleri
            // 0 - Özel
            // 1 - Ticari
            // 2 - Resmi
            string aracKullanimSekli = "0";
            if (response.police.arac.kullanimSekli == "0")
                aracKullanimSekli = "2";
            else if (response.police.arac.kullanimSekli == "1")
                aracKullanimSekli = "0";
            else
                aracKullanimSekli = "1";

            plakaSorgu.AracKullanimSekli = aracKullanimSekli;
            plakaSorgu.AracKullanimTarzi = "111-10";

            List<CR_AracGrup> aracGruplari = _CRService.GetAracGruplari(TeklifUretimMerkezleri.MAPFRE);
            CR_AracGrup grp = aracGruplari.FirstOrDefault(f => f.TarifeKodu == response.police.arac.aracTarifeGrupKodu);
            if (grp != null)
            {
                kullanimSekli = grp.KullanimTarziKodu;
                plakaSorgu.AracKullanimTarzi = grp.KullanimTarziKodu + "-" + grp.Kod2;
            }
            else
            {
                AracTip tip = _AracService.GetAracTip(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu);
                if (tip != null)
                {
                    kullanimSekli = tip.KullanimSekli1;
                    plakaSorgu.AracKullanimTarzi = tip.KullanimSekli1 + "-10";
                }
            }

            List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
            plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            List<AracMarka> markalar = _AracService.GetAracMarkaList();
            plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

            List<AracTip> tipler = _AracService.GetAracTipList(plakaSorgu.AracMarkaKodu);
            plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);
            if (aracModel != null)
            {
                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
            }

            plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

            List<KeyValueItem<string, string>> amsList = null;
            if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
            {
                string[] parts = plakaSorgu.AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, parts[0], parts[1]);

                }
            }
            if (amsList == null)
            {
                amsList = _CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE, _AktifKullaniciService.TVMKodu, "111", "10");
            }
            plakaSorgu.Ams = new SelectList(amsList, "Key", "Value", "").ListWithOptionLabel();


            if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
            {
                string[] parts = plakaSorgu.AracKullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    CR_KullanimTarzi tarz = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE, parts[0], parts[1]);
                    if (tarz != null)
                    {
                        List<KeyValueItem<string, string>> list = _CRService.GetKaskoIkameTuruList(TeklifUretimMerkezleri.MAPFRE, tarz.TarifeKodu);

                        if (list != null && list.Count > 0)
                        {
                            string defaultIkameKodu = "0";
                            plakaSorgu.IkameTurleri = new SelectList(list, "Key", "Value").ToList<SelectListItem>();
                            if (tarz.TarifeKodu == "40")
                                defaultIkameKodu = "ABC07";

                            plakaSorgu.IkameTuru = defaultIkameKodu;
                        }
                    }
                }
            }
            return plakaSorgu;
        }

        private DetayAracBilgiModel MapfreDetayAracBilgiModel(ITeklif teklif)
        {
            DetayAracBilgiModel model = new DetayAracBilgiModel();

            TeklifArac arac = teklif.Arac;
            model.PlakaKodu = arac.PlakaKodu;
            model.PlakaNo = arac.PlakaNo;

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            short kullanimSekliKodu = Convert.ToInt16(arac.KullanimSekli);
            AracKullanimSekli kullanimSekli = aracService.GetAracKullanimSekli(kullanimSekliKodu);

            model.KullanimSekli = kullanimSekli != null ? kullanimSekli.KullanimSekli : String.Empty;

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

        protected MapfreTeklifFiyatDetayModel MapfreTeklifFiyat(int tumKodu, IsDurumDetay detay)
        {
            ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
            TUMDetay tum = tumService.GetDetay(tumKodu);

            MapfreTeklifFiyatDetayModel fiyatModel = new MapfreTeklifFiyatDetayModel();
            fiyatModel.TUMKodu = tum.Kodu;
            fiyatModel.TUMUnvani = tum.Unvani;
            fiyatModel.TUMLogoUrl = tum.Logo;
            fiyatModel.Surprimler = new List<TeklifSurprimModel>();

            int teklifId = detay.ReferansId;
            ITeklif teklif = _TeklifService.GetTeklif(teklifId);
            fiyatModel.PDFDosyasi = teklif.GenelBilgiler.PDFDosyasi;
            fiyatModel.Fiyat1 = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL" : "";
            fiyatModel.Fiyat1_TeklifId = teklif.GenelBilgiler.TeklifId;
            fiyatModel.MapfreTeklifNo = teklif.GenelBilgiler.TUMTeklifNo;

            if (teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.HasValue && teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi > 0)
            {
                fiyatModel.Hasarsizlik = teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.Value.ToString("N0");
                fiyatModel.HasarIndirimSurprim = "I";
            }
            if (teklif.GenelBilgiler.HasarSurprimYuzdesi.HasValue && teklif.GenelBilgiler.HasarSurprimYuzdesi > 0)
            {
                fiyatModel.Hasarsizlik = "+ %" + teklif.GenelBilgiler.HasarSurprimYuzdesi.Value.ToString("N2");
                fiyatModel.HasarIndirimSurprim = "S";
            }
            if (teklif.GenelBilgiler.GecikmeZammiYuzdesi.HasValue && teklif.GenelBilgiler.GecikmeZammiYuzdesi > 0)
            {
                TeklifSurprimModel surprim = new TeklifSurprimModel();
                surprim.SurprimAciklama = babonline.DelayHike;
                surprim.Surprim = "%" + teklif.GenelBilgiler.GecikmeZammiYuzdesi.Value.ToString("N2");
                surprim.SurprimIS = "S";
                fiyatModel.Surprimler.Add(surprim);
            }
            if (teklif.GenelBilgiler.PlakaIndirimYuzdesi.HasValue && teklif.GenelBilgiler.PlakaIndirimYuzdesi > 0)
            {
                TeklifSurprimModel surprim = new TeklifSurprimModel();
                surprim.SurprimAciklama = "Plaka İndirimi";
                surprim.Surprim = "%" + teklif.GenelBilgiler.PlakaIndirimYuzdesi.Value.ToString("N2");
                surprim.SurprimIS = "I";
                fiyatModel.Surprimler.Add(surprim);
            }
            if (teklif.GenelBilgiler.Otorizasyon.HasValue && teklif.GenelBilgiler.Otorizasyon.Value == 1)
            {
                fiyatModel.Otorizasyon = true;

                fiyatModel.OtorizasyonMesajlari = new List<string>();
                string[] parts = detay.HataMesaji.Split('|');
                foreach (string h in parts)
                {
                    fiyatModel.OtorizasyonMesajlari.Add(h);
                }
            }
            if (String.IsNullOrEmpty(fiyatModel.Fiyat1))
            {
                fiyatModel.Hatalar = new List<string>();
                string[] parts = detay.HataMesaji.Split('|');

                foreach (string h in parts)
                {
                    fiyatModel.Hatalar.Add(h);
                }
            }

            return fiyatModel;
        }
    }
}

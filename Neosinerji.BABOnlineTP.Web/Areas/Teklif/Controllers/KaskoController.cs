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
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN;
using Neosinerji.BABOnlineTP.Business.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.GULF;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using System.IO;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.KaskoSigortasi)]
    public class KaskoController : TeklifController
    {
        public KaskoController(ITVMService tvmService,
                                ITeklifService teklifService,
                                IMusteriService musteriService,
                                IKullaniciService kullaniciService,
                                IAktifKullaniciService aktifKullaniciService,
                                ITanimService tanimService,
                                IUlkeService ulkeService,
                                ICRService crService,
                                IAracService aracService,
                                IUrunService urunService,
                                ITUMService tumService)
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService, ulkeService, crService, aracService, urunService, tumService)
        {

        }

        public ActionResult Ekle(int? id)
        {
            KaskoModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            KaskoModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public KaskoModel EkleModel(int? id, int? teklifId)
        {
            KaskoModel model = new KaskoModel();

            #region Teklif Genel

            ILogService log = DependencyResolver.Current.GetService<ILogService>();
            log.Visit();
            ITeklif teklif = null;

            model.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            #endregion

            #region Teklif Hazırlayan
            int? sigortaliMusteriKodu = null;

            //Teklifi hazırlayan
            model.Hazirlayan = base.EkleHazirlayanModel();

            //Sigorta Ettiren / Sigortalı
            model.Musteri = new SigortaliModel();
            model.Musteri.SigortaliAyni = true;
            model.Musteri.EMailRequired = false; //Email zorunluluğu kaldırılıyor.
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
                var gulfKimlikNo = teklif.ReadSoru(KaskoSorular.GulfKimlikNo, "");
                if (!String.IsNullOrEmpty(gulfKimlikNo))
                {
                    model.Musteri.SigortaEttiren.GulfKimlikNo = gulfKimlikNo;
                }

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

            List<SelectListItem> numaraTipleri = new List<SelectListItem>();
            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

            model.Musteri.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text", IletisimNumaraTipleri.Cep.ToString()).ListWithOptionLabel();

            model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
            model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
            model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
            model.Musteri.CinsiyetTipleri.First().Selected = true;

            #endregion

            #region Teklif Arac
            model.Arac = base.KaskoEkleAracModel();
            model.Arac.TescilIl = "34";
            model.Arac.TescilIller = new SelectList(_CRService.GetTescilIlList(), "Key", "Value", model.Arac.TescilIl).ListWithOptionLabel();
            model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value", "").ListWithOptionLabel();
            model.Arac.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.Arac.AnadoluKullanimTipListe = new List<SelectListItem>();
            model.Arac.AnadoluKullanimSekilleri = new List<SelectListItem>();
            model.Arac.IkameTurleriAnadolu = new List<SelectListItem>();
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

                    List<AracMarka> markalar = _AracService.GetAracMarkaList(kullanimTarziKodu);
                    model.Arac.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

                    if (teklif.Arac.Model.HasValue)
                        model.Arac.Model = teklif.Arac.Model.Value;

                    if (!String.IsNullOrEmpty(teklif.Arac.AracinTipi))
                    {
                        model.Arac.TipKodu = teklif.Arac.AracinTipi;

                        List<AracTip> tipler = _AracService.GetAracTipList(kullanimTarziKodu, model.Arac.MarkaKodu, model.Arac.Model);
                        model.Arac.AracTipleri = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                    }

                    if (teklif.Arac.AracDeger.HasValue)
                    {
                        model.Arac.AracDeger = teklif.Arac.AracDeger.Value.ToString("0");
                    }

                    if (teklif.Arac.KoltukSayisi.HasValue)
                        model.Arac.KisiSayisi = teklif.Arac.KoltukSayisi.Value;
                }

                if (!String.IsNullOrEmpty(teklif.Arac.TramerBelgeNo))
                {
                    model.Arac.TramerBelgeNumarasi = teklif.Arac.TramerBelgeNo;
                }

                if (teklif.Arac.TramerBelgeTarihi.HasValue)
                {
                    model.Arac.TramerBelgeTarihi = teklif.Arac.TramerBelgeTarihi.Value.ToString();
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

                string AnadoluMarkaKodu = teklif.ReadSoru(KaskoSorular.AnadoluMarkaKodu, "");
                model.Arac.AnadoluMarkaKodu = AnadoluMarkaKodu;

                string AnadoluKullanimTipi = teklif.ReadSoru(KaskoSorular.AnadoluKullanimTipi, "");
                model.Arac.AnadoluKullanimTip = AnadoluKullanimTipi;

                string AnadoluKullanimSekli = teklif.ReadSoru(KaskoSorular.AnadoluKullanimSekli, "");
                model.Arac.AnadoluKullanimSekli = AnadoluKullanimSekli;
                
                var liste = _TeklifService.GetAnadoluKullanimTipleri(parts[0], AnadoluKullanimTipiSorguTipi.KullanimTarzi);
                if (liste != null)
                {
                    model.Arac.AnadoluKullanimTipListe = new SelectList(liste, "KullanimTipi", "TipAdi",model.Arac.AnadoluKullanimTip).ListWithOptionLabel();               
                }
                var sekilListe = _TeklifService.GetAnadoluKullanimTipleri(model.Arac.AnadoluKullanimTip, AnadoluKullanimTipiSorguTipi.KullanimSekli);
                if (sekilListe != null)
                {
                    model.Arac.AnadoluKullanimSekilleri = new SelectList(sekilListe, "KullanimSekli", "SekilAdi").ListWithOptionLabel();
                }
                model.Arac.IkameTuruAnadolu= teklif.ReadSoru(KaskoSorular.AnadoluIkameTuru, "");
                IANADOLUKasko _kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();
                var result = _kasko.getAnadoluTeminatlar(model.Arac.MarkaKodu, model.Arac.AnadoluKullanimSekli, model.Arac.AnadoluKullanimTip, _AktifKullaniciService.TVMKodu);
                
                model.Arac.IkameTurleriAnadolu = new SelectList(result.ikameList, "key", "value", model.Arac.IkameTuruAnadolu).ListWithOptionLabel();

                model.Arac.HasarsizlikIndirim = teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.HasValue ? Convert.ToInt32(teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.Value).ToString() : "0";
                model.Arac.HasarSurprim = teklif.GenelBilgiler.HasarSurprimYuzdesi.HasValue ? Convert.ToInt32(teklif.GenelBilgiler.HasarSurprimYuzdesi.Value).ToString() : "0";
                model.Arac.UygulananKademe = teklif.GenelBilgiler.TarifeBasamakKodu.HasValue ? teklif.GenelBilgiler.TarifeBasamakKodu.Value.ToString() : "0";
            }
            model.Arac.SigortaSirketleri = this.SigortaSirketleri;
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
                    model.EskiPolice.SigortaSirketKodu = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
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

            #region Teklif Teminat

            model.Teminat = base.KaskoTeminatModel();
            model.Teminat.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            model.Teminat.OlumSakatlikTeminat = 5000;
            model.Teminat.FaaliyetKodlari = new List<SelectListItem>();
            model.Teminat.GroupamaYakinlikDereceleri = new List<SelectListItem>();
            model.Teminat.GroupamaRizikoFiyatlari = new List<SelectListItem>();
            model.Teminat.HKBedelleri = new List<SelectListItem>();
            model.Teminat.EngelliAraciMi = false;
            model.Teminat.UnicoHasarsizlikKorumaKlozu = false;
            model.Teminat.UnicoYeniDegerklozu = false;
            model.Teminat.UnicoTekSurucuMu = false;
            model.Teminat.UnicoTEBUyesi = false;
            model.Teminat.UnicoKiralikAracmi = false;
            model.Teminat.UnicoSurucuKursuAracimi = false;
            model.Teminat.UnicoSurucuSayisi = "0";
            model.Teminat.UnicoDepremSelMuafiyeti = "0";
            model.Teminat.UnicoMeslekKodu = "0";
            model.Teminat.UnicoKaskoMuafiyeti = "2";
            model.Teminat.UnicoIkameSecenegi = "0";
            model.Teminat.KiymetKazanma = true;
            model.Teminat.SigaraYanigi = true;
            model.Teminat.Seylap = true;
            model.Teminat.AnahtarliCalinma = true;
            model.Teminat.YetkiliOlmayanCekilme = true;
            model.Teminat.CamMuafiyetiKaldirilsinMi = true;
            model.Teminat.KisiselEsya = true;
            model.Teminat.HDIRayicDegerKoruma = true;
            model.Teminat.YanlisAkaryakitDolumu = true;
            model.Teminat.HDIPatlayiciMadde = true;

            if (teklifId.HasValue)
            {
                string[] aracKullanim = teklif.Arac.KullanimTarzi.Split('-');
                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    model.Teminat.IMM = new SelectList(_CRService.GetKaskoAMSList(TeklifUretimMerkezleri.MAPFRE,
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
                }
                else
                {
                    //model.Teminat.IMM = new SelectList(_CRService.GetKaskoIMMList(TeklifUretimMerkezleri.HDI, aracKullanim[0],
                    //                                                              aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

                    //model.Teminat.FK = new SelectList(_CRService.GetKaskoFKList(TeklifUretimMerkezleri.HDI, aracKullanim[0],
                    //                                                           aracKullanim[1]), "Key", "Value", "").ListWithOptionLabel();

                    string immKodu = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                    if (!String.IsNullOrEmpty(immKodu))
                    {
                        model.Teminat.IMMKodu = Convert.ToInt16(immKodu);
                    }

                    string fkKodu = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                    if (!String.IsNullOrEmpty(fkKodu))
                    {
                        model.Teminat.FKKodu = Convert.ToInt16(fkKodu);
                    }

                    model.Teminat.IMM = new SelectList(_CRService.GetKaskoIMMListe(aracKullanim[0],
                                                                                aracKullanim[1]), "Key", "Value", model.Teminat.IMMKodu).ListWithOptionLabel();

                    model.Teminat.FK = new SelectList(_CRService.GetKaskoFKListe(aracKullanim[0],
                                                                                aracKullanim[1]), "Key", "Value", model.Teminat.FKKodu).ListWithOptionLabel();

                }
                int yillikIndirimi = Convert.ToInt32(teklif.ReadSoru(KaskoSorular.NipponYetkiliInidirimi, 0));
                if (yillikIndirimi != null)
                {
                    model.Teminat.NipponYetkiliIndirimi = yillikIndirimi;
                }

                model.Teminat.NipponServisTuru = teklif.ReadSoru(KaskoSorular.TurkNipponServisTuru, "");
                model.Teminat.NipponMuafiyetTutari = teklif.ReadSoru(KaskoSorular.TurkNipponMuafiyetTutari, "");

                string kaskoTuru = teklif.ReadSoru(KaskoSorular.Kasko_Turu, String.Empty);
                if (!String.IsNullOrEmpty(kaskoTuru))
                {
                    model.Teminat.Kasko_Turu = Convert.ToInt16(kaskoTuru);
                }

                string servisTuru = teklif.ReadSoru(KaskoSorular.Servis_Turu, String.Empty);
                if (!String.IsNullOrEmpty(servisTuru))
                {
                    model.Teminat.Kasko_Servis = Convert.ToInt16(servisTuru);
                }

                string selectedIkame = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                if (!String.IsNullOrEmpty(selectedIkame))
                {
                    model.Teminat.IkameTuru = selectedIkame;
                }
                else
                {
                    model.Teminat.IkameTuru = "";
                }
                model.Teminat.IkameTurleri = new List<SelectListItem>() { new SelectListItem() { Text = babonline.NoSubstitution, Value = "0" } };

                if (!String.IsNullOrEmpty(model.Arac.KullanimTarziKodu))
                {
                    string[] parts = model.Arac.KullanimTarziKodu.Split('-');
                    if (parts.Length == 2 && parts[0] == "111" && parts[1] == "10")
                    {
                        model.Teminat.IkameTurleri = IkameTurleri(model.Teminat.IkameTuru);
                    }
                }

                model.Teminat.GLKHHT = teklif.ReadSoru(KaskoSorular.GLKHHT, false);
                model.Teminat.Deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
                model.Teminat.Sel_Su = teklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false);
                model.Teminat.Hasarsizlik_Koruma = teklif.ReadSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, false);
                model.Teminat.Saglik = false;
                model.Teminat.Yutr_Disi_Teminat = false;
                model.Teminat.LPGLi_Arac = false;

                model.Teminat.Hayvanlarin_Verecegi_Zarar_Teminati = teklif.ReadSoru(KaskoSorular.Hayvanlarin_Verecegi_Zarar_ZarYok, false);
                model.Teminat.Hukuksal_Koruma_Teminati = true;
                model.Teminat.Eskime_Payi_Teminati = teklif.ReadSoru(KaskoSorular.Eskime_VarYok, false);
                model.Teminat.Alarm_Teminati = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
                model.Teminat.Anahtar_Kaybi_Teminati = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
                model.Teminat.Yangin = teklif.ReadSoru(KaskoSorular.Yangin_VarYok, false);
                model.Teminat.Calinma = teklif.ReadSoru(KaskoSorular.Calinma_VarYok, false);

                model.Teminat.KiymetKazanma = teklif.ReadSoru(KaskoSorular.KiymetKazanma, false);
                model.Teminat.SigaraYanigi = teklif.ReadSoru(KaskoSorular.SigaraYanigi, false);
                model.Teminat.Seylap = teklif.ReadSoru(KaskoSorular.Seylap, false);
                model.Teminat.AnahtarliCalinma = teklif.ReadSoru(KaskoSorular.AnahtarliCalinma, false);
                model.Teminat.YetkiliOlmayanCekilme = teklif.ReadSoru(KaskoSorular.YetkiliOlmayanCekilme, false);
                model.Teminat.CamMuafiyetiKaldirilsinMi = teklif.ReadSoru(KaskoSorular.CamMuafiyetiKaldirilsinMi, true);

                //HDI Ozel Teminatlar
                model.Teminat.KisiselEsya = teklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, true);
                model.Teminat.HDIRayicDegerKoruma = teklif.ReadSoru(KaskoSorular.HDIRayicBedelKoruma, true);
                model.Teminat.YanlisAkaryakitDolumu = teklif.ReadSoru(KaskoSorular.HataliAkaryakitAlimi, true);
                model.Teminat.HDIPatlayiciMadde = teklif.ReadSoru(KaskoSorular.HDIPatlayiciParlayici, true);
                //-----

                model.Teminat.Elektrikli_Arac = teklif.ReadSoru(KaskoSorular.ElektrikliArac, false);
                if (model.Teminat.Elektrikli_Arac)
                {
                    model.Teminat.ElektirkliAracModel.Bedeli = teklif.ReadSoru(KaskoSorular.ElektrikliAracBedeli, "0");
                    model.Teminat.ElektirkliAracModel.PilId = teklif.ReadSoru(KaskoSorular.ElektrikliAracPilId, "");
                }

                model.Teminat.MeslekKodu = teklif.ReadSoru(KaskoSorular.Meslek, "99");
                var MeslekListesi = _TeklifService.GetMeslekList();
                if (MeslekListesi != null)
                {
                    model.Teminat.MeslekKodlari = new SelectList(MeslekListesi, "MeslekKodu", "Aciklama", "").ToList();
                }

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
                List<TeklifAracEkSoru> tasinanYukler = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.TASINAN_YUK).ToList<TeklifAracEkSoru>();
                model.Teminat.TasinanYukler = new List<MapfreTasinanYukModel>();
                if (tasinanYukler != null && tasinanYukler.Count > 0)
                {
                    foreach (var item in tasinanYukler)
                    {
                        model.Teminat.TasinanYukKademe = item.SoruKodu;
                        model.Teminat.TasinanYukAciklama = item.Aciklama;
                        model.Teminat.TasinanYukBedel = (int)item.Bedel;
                    }
                }
                var Kademeler = GetTasinanYukKademeleri(model.Arac.KullanimTarziKodu);
                model.Teminat.TasinanYukTipleri = new SelectList(Kademeler.list, "key", "value", model.Teminat.TasinanYukKademe).ListWithOptionLabel();

                bool sompoJapanKaskoTuru = teklif.ReadSoru(KaskoSorular.SompoJapanKaskoTuru, true);
                if (sompoJapanKaskoTuru)
                {
                    model.Teminat.TicariBireysel = true;
                }
                else
                {
                    model.Teminat.TicariBireysel = false;
                    var faaliyetKodu = teklif.ReadSoru(KaskoSorular.SompoJapanFaaliyetKodu, "99");
                    model.Teminat.FaaliyetKodu = faaliyetKodu;
                    var FaaliyetList = this.GetSompoJapanFaaliyetKodlari(teklif.GenelBilgiler.TVMKodu);
                    if (FaaliyetList != null)
                    {
                        model.Teminat.FaaliyetKodlari = new SelectList(FaaliyetList, "key", "value", model.Teminat.FaaliyetKodu).ListWithOptionLabel();
                    }
                    else
                    {
                        model.Teminat.FaaliyetKodlari = new List<SelectListItem>();
                    }
                }
                var SirketListesi = _TUMService.GetListTUMDetay();
                model.KaskoDigerTeklif = new KaskoDigerTeklifModel();
                model.KaskoDigerTeklif.SigortaSirketleri = new SelectList(SirketListesi, "Kodu", "Unvani", "").ListWithOptionLabel();
                model.KaskoDigerTeklif.DigerTeklifler = new List<DigerTeklifModel>();
                #region Groupama Özel Teminatlar
                bool KazaDestekVarMi = teklif.ReadSoru(KaskoSorular.KazaDestekVarmi, false);
                if (KazaDestekVarMi)
                {
                    model.Teminat.KazaDestekVarMi = true;

                    var teminatKodu = teklif.ReadSoru(KaskoSorular.TeminatLimiti, String.Empty);

                    var GroupamaTeminatLimitleri = this.GetGroupamaTeminatLimitleri();
                    if (GroupamaTeminatLimitleri.Count > 0)
                    {
                        model.Teminat.GroupamaTeminatLimitleri = new SelectList(GroupamaTeminatLimitleri, "key", "value", teminatKodu).ListWithOptionLabel();
                    }
                    else
                    {
                        model.Teminat.GroupamaTeminatLimitleri = new List<SelectListItem>();
                    }
                }
                else
                {
                    model.Teminat.KazaDestekVarMi = false;
                    model.Teminat.GroupamaTeminatLimitleri = new List<SelectListItem>();
                }

                bool PrimKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
                if (PrimKoruma)
                {
                    model.Teminat.PrimKoruma = true;
                }
                else
                {
                    model.Teminat.PrimKoruma = false;
                }

                bool AsistansPlusPaketi = teklif.ReadSoru(KaskoSorular.AsistansPlusPaketi, false);
                if (AsistansPlusPaketi)
                {
                    model.Teminat.AsistansPlusPaketi = true;
                }
                else
                {
                    model.Teminat.AsistansPlusPaketi = false;
                }
                bool Kolonlar = teklif.ReadSoru(KaskoSorular.Kolonlar, false);
                if (Kolonlar)
                {
                    model.Teminat.Kolonlar = true;
                }
                else
                {
                    model.Teminat.Kolonlar = false;
                }
                string AcenteOzelIndirim = teklif.ReadSoru(KaskoSorular.AcenteOzelIndirimi, String.Empty);
                model.Teminat.AcenteOzelIndirimi = AcenteOzelIndirim;

                bool PesinIndirimi = teklif.ReadSoru(KaskoSorular.PesinIndirimi, false);
                model.Teminat.PesinIndirimi = PesinIndirimi;

                bool YurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                model.Teminat.YurtDisiKasko = YurtDisiKasko;
                //groupoma
                bool GroupamaElitKaskomu = teklif.ReadSoru(KaskoSorular.GroupamaElitKaskomu, false);
                model.Teminat.ElitKaskomu = GroupamaElitKaskomu;

                string groupamaMeslekKodu = teklif.ReadSoru(KaskoSorular.GroupamaMeslekKodu, String.Empty);
                if (!String.IsNullOrEmpty(groupamaMeslekKodu))
                {
                    model.Teminat.GroupamaMeslekKodu = groupamaMeslekKodu;
                }
                model.Teminat.GroupamaMeslekKodlari = new SelectList(TeklifProvider.GroupamaMeslekGrupKodlari(), "Value", "Text", model.Teminat.GroupamaMeslekKodu).ToList();
                string ergoMeslekKodu = teklif.ReadSoru(KaskoSorular.ErgoMeslekKodu, String.Empty);
                if (!String.IsNullOrEmpty(ergoMeslekKodu))
                {
                    model.Teminat.ErgoMeslekKodu = ergoMeslekKodu;
                }
                model.Teminat.ErgoMeslekKodlari = new SelectList(TeklifProvider.ErgoMeslekGrupKodlari(), "Value", "Text", model.Teminat.ErgoMeslekKodu).ToList();
                string ergoServisTuru = teklif.ReadSoru(KaskoSorular.ErgoServisSecenegi, String.Empty);
                if (!String.IsNullOrEmpty(ergoServisTuru))
                {
                    model.Teminat.ErgoServisTuru = ergoServisTuru;
                }
                model.Teminat.ErgoServisTurleri = new SelectList(TeklifProvider.ErgoServisTurleri(), "Value", "Text", model.Teminat.ErgoServisTuru).ToList();

                string groupamaYakinlikDerecesi = teklif.ReadSoru(KaskoSorular.GroupamaYakinlikDerecesi, String.Empty);
                if (!String.IsNullOrEmpty(groupamaYakinlikDerecesi))
                {
                    model.Teminat.YakinlikDerecesi = groupamaYakinlikDerecesi;
                }
                model.Teminat.GroupamaYakinlikDereceleri = new SelectList(TeklifProvider.GroupamaYakinlikDereceleri(), "Value", "Text", model.Teminat.YakinlikDerecesi).ToList();

                string groupamaRizikoFiyati = teklif.ReadSoru(KaskoSorular.GroupamaRizikoFiyati, String.Empty);
                if (!String.IsNullOrEmpty(groupamaRizikoFiyati))
                {
                    model.Teminat.RizikoFiyati = groupamaRizikoFiyati;
                }
                model.Teminat.GroupamaRizikoFiyatlari = new SelectList(TeklifProvider.GroupamaRizikoFiyatlari(), "Value", "Text", model.Teminat.RizikoFiyati).ToList();

                string groupamaYHIMSKodu = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSKodu, String.Empty);
                if (!String.IsNullOrEmpty(groupamaYHIMSKodu))
                {
                    model.Teminat.GroupamaYHIMSKodu = groupamaYHIMSKodu;
                    model.Teminat.GroupamaYHIMSKodlari = new SelectList(TeklifProvider.GroupamaYHIMSSecimleri(), "Value", "Text", model.Teminat.GroupamaYHIMSKodu).ToList();
                }
                else
                {
                    model.Teminat.GroupamaYHIMSKodlari = new SelectList(TeklifProvider.GroupamaYHIMSSecimleri(), "Value", "Text", "").ToList();
                }

                string groupamaYHIMSBasamakKodu = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSBasamakKodu, String.Empty);
                if (!String.IsNullOrEmpty(groupamaYHIMSBasamakKodu))
                {
                    model.Teminat.GroupamaYHIMSBasamakKodu = groupamaYHIMSKodu;
                }

                var GroupamaYHIMSBasamakKodlari = this.GetGroupamaYHIMSBasamaklar();
                if (GroupamaYHIMSBasamakKodlari.Count > 0)
                {
                    model.Teminat.GroupamaYHIMSBasamakKodlari = new SelectList(GroupamaYHIMSBasamakKodlari, "key", "value", model.Teminat.GroupamaYHIMSBasamakKodu).ListWithOptionLabel();
                }
                else
                {
                    model.Teminat.GroupamaYHIMSBasamakKodlari = new List<SelectListItem>();
                }

                decimal groupamaAracHukuksalKoruma = teklif.ReadSoru(KaskoSorular.GroupamaAracHukuksalKoruma, 0);
                model.Teminat.GroupamaAracHukuksalKorumaBedel = Convert.ToInt32(groupamaAracHukuksalKoruma);

                decimal groupamaSurucuHukuksalKoruma = teklif.ReadSoru(KaskoSorular.GroupamaSurucuHukuksalKoruma, 0);
                model.Teminat.GroupamaSurucuHukuksalKorumaBedel = Convert.ToInt32(groupamaSurucuHukuksalKoruma);

                bool GroupamaManeviDahilMi = teklif.ReadSoru(KaskoSorular.GroupamaManeviDahilMi, false);
                if (GroupamaManeviDahilMi)
                {
                    model.Teminat.GroupamaManeviDahilMi = true;
                }
                else
                {
                    model.Teminat.GroupamaManeviDahilMi = false;
                }

                decimal GroupamaManeviDahilBedeli = teklif.ReadSoru(KaskoSorular.GroupamaManeviDahilBedeli, 0);
                if (GroupamaManeviDahilBedeli > 0)
                {
                    model.Teminat.GroupamaManeviDahilBedeli = Convert.ToInt32(GroupamaManeviDahilBedeli);
                }

                string GulfIkameTuru = teklif.ReadSoru(KaskoSorular.GulfIkameTuru, "");
                model.Teminat.IkameTuruGulf = GulfIkameTuru;

                string GulfYakitTuru = teklif.ReadSoru(KaskoSorular.GulfYakitTuru, "");
                model.Teminat.YakitTuruGulf = GulfYakitTuru;

                //string GulfHukuksalKoruma = teklif.ReadSoru(KaskoSorular.GulfHukuksalKorumaBedeli, "");
                //model.Teminat.HukuksalKorumaGulf = GulfHukuksalKoruma;

                #endregion

                string hukuksaKorumaKademesi = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, "");
                model.Teminat.HKBedeli = hukuksaKorumaKademesi;

                string kilitBedeli = teklif.ReadSoru(KaskoSorular.UnicokilitBedeli, "");
                model.Teminat.UnicoKilitBedeli = kilitBedeli;

                model.Teminat.UnicoGenisletilmisCam = teklif.ReadSoru(KaskoSorular.UnicoGenisletilmisCam, true);
                model.Teminat.UnicoManeviTazminat = teklif.ReadSoru(KaskoSorular.UnicoManeviTazminat, true);
                model.Teminat.EngelliAraciMi = teklif.ReadSoru(KaskoSorular.EngelliAracimi, true);
                model.Teminat.UnicoHasarsizlikKorumaKlozu = teklif.ReadSoru(KaskoSorular.UnicoHasarsizlikKorumaKlozu, false);
                model.Teminat.UnicoKaskoMuafiyeti = teklif.ReadSoru(KaskoSorular.UnicoKaskoMuafiyeti, "2");
                model.Teminat.UnicoKiralikAracmi = teklif.ReadSoru(KaskoSorular.UnicoKiralikAraciMi, true);
                model.Teminat.UnicoDepremSelMuafiyeti = teklif.ReadSoru(KaskoSorular.UnicoDepremSelMuafiyeti, "2");
                model.Teminat.UnicoMeslekKodu = teklif.ReadSoru(KaskoSorular.UNICOMeslekKodu, "0");
                model.Teminat.UnicoSurucuKursuAracimi = teklif.ReadSoru(KaskoSorular.UnicoSurucuKursuAraciMi, true);
                model.Teminat.UnicoTekSurucuMu = teklif.ReadSoru(KaskoSorular.UnicoTekSurucuMu, true);
                model.Teminat.UnicoTEBUyesi = teklif.ReadSoru(KaskoSorular.UnicoTEBUyesiMi, true);
                model.Teminat.UnicoYeniDegerklozu = teklif.ReadSoru(KaskoSorular.UnicoYeniDegerklozu, true);
                model.Teminat.UnicoSurucuSayisi = teklif.ReadSoru(KaskoSorular.UnicoSurucuSayisi, "1");
                model.Teminat.UnicoIkameSecenegi = teklif.ReadSoru(KaskoSorular.UnicoIkameSecenegi, "1");
                model.Teminat.UnicoAksesuarTuru = teklif.ReadSoru(KaskoSorular.UnicoIkameSecenegi, "0");
                model.Teminat.AxaHayatTeminatLimiti = teklif.ReadSoru(KaskoSorular.AxaHayatTeminatLimiti, "");
                model.Teminat.AxaAsistansHizmeti = teklif.ReadSoru(KaskoSorular.AxaAsistansHizmeti, "I");
                model.Teminat.AxaAracaBagliKaravanMi = teklif.ReadSoru(KaskoSorular.AxaAracaBagliKaravanVarMi, false);
                model.Teminat.KullanimGelirKaybiVarMi = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiVarMi, false);
                //model.Teminat.AxaIkameSecimi = teklif.ReadSoru(KaskoSorular.AxaIkameSecimi, "");
                if (teklif.Arac.Model >= TurkeyDateTime.Now.Year - 1)
                {
                    model.Teminat.AxaOnarimSecimi = teklif.ReadSoru(KaskoSorular.AxaOnarimSecimi, "");
                    model.Teminat.AxaPlakaYeniKayitMi = teklif.ReadSoru(KaskoSorular.AxaPlakaYeniKayitMi, false);
                    model.Teminat.AxaYeniDegerKlozu = teklif.ReadSoru(KaskoSorular.AxaYeniDegerKlozu, "");
                    model.Teminat.AxaDepremSelKoasuransi = teklif.ReadSoru(KaskoSorular.AxaDepremSelKoasuransi, "");
                    model.Teminat.AxaMuafiyetTutari = teklif.ReadSoru(KaskoSorular.AxaMuafiyetTutari, "");
                    model.Teminat.AxaCamFilmiLogo = teklif.ReadSoru(KaskoSorular.AxaCamFilmiLogo, false);
                    model.Teminat.AxaSorumlulukLimiti = teklif.ReadSoru(KaskoSorular.AxaSorumlulukLimiti, "");
                }

                model.Teminat.SompoArtiTeminatPlani = teklif.ReadSoru(KaskoSorular.SompoArtiTeminatPlani, false);
                model.Teminat.SompoArtiTeminatDegeri = teklif.ReadSoru(KaskoSorular.SompoArtiTeminatPlanDegeri, "");
            }
            else
            {
                model.Teminat.GLKHHT = true;
                model.Teminat.Deprem = true;
                model.Teminat.Sel_Su = true;
                model.Teminat.Hasarsizlik_Koruma = true;
                model.Teminat.Saglik = false;
                model.Teminat.Yutr_Disi_Teminat = false;
                model.Teminat.LPGLi_Arac = false;
                model.Teminat.Elektrikli_Arac = false;
                model.Teminat.Hayvanlarin_Verecegi_Zarar_Teminati = true;
                model.Teminat.Hukuksal_Koruma_Teminati = true;
                model.Teminat.Eskime_Payi_Teminati = true;
                model.Teminat.Alarm_Teminati = true;
                model.Teminat.Anahtar_Kaybi_Teminati = true;
                model.Teminat.Yangin = true;
                model.Teminat.Calinma = true;
                model.Teminat.Kasko_Turu = 3;
                model.Teminat.Kasko_Servis = 4;
                model.Teminat.AMSKodu = String.Empty;
                model.Teminat.OlumSakatlikTeminat = 5000;
                model.Teminat.IkameTurleri = new List<SelectListItem>() { new SelectListItem() { Text = babonline.NoSubstitution, Value = "0" } };
                model.Teminat.NipponYetkiliIndirimi = 0;

                var MeslekListesi = _TeklifService.GetMeslekList();
                if (MeslekListesi != null)
                {
                    model.Teminat.MeslekKodu = "99";
                    model.Teminat.MeslekKodlari = new SelectList(MeslekListesi, "MeslekKodu", "Aciklama", "").ToList();
                }

                model.Teminat.Aksesuar_Teminati = false;
                model.Teminat.AksesuarTipleri = TeklifListeleri.MapfreAksesuarTipleri();
                model.Teminat.ElektronikCihaz_Teminati = false;
                model.Teminat.ElektronikCihazTipleri = TeklifListeleri.MapfreElektronikCihazTipleri();

                model.Teminat.IMM = new List<SelectListItem>();
                model.Teminat.IMM.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "0" });
                model.Teminat.FK = new List<SelectListItem>();
                model.Teminat.FK.Add(new SelectListItem() { Text = babonline.PleaseSelect, Value = "0" });

                model.Teminat.TasinanYukTipleri = new List<SelectListItem>();
                model.Teminat.FaaliyetKodlari = new List<SelectListItem>();
                model.Teminat.TicariBireysel = true;

                model.Teminat.AsistansPlusPaketi = false;
                model.Teminat.PrimKoruma = true;
                model.Teminat.KazaDestekVarMi = false;
                model.Teminat.Kolonlar = false;
                model.Teminat.AcenteOzelIndirimi = "";
                model.Teminat.PesinIndirimi = false;
                model.Teminat.YurtDisiKasko = false;
                model.Teminat.ElitKaskomu = false;

                model.Teminat.GroupamaTeminatLimitleri = new List<SelectListItem>();
                model.Teminat.GroupamaMeslekKodlari = new SelectList(TeklifProvider.GroupamaMeslekGrupKodlari(), "Value", "Text", "").ToList();
                model.Teminat.ErgoMeslekKodlari = new SelectList(TeklifProvider.ErgoMeslekGrupKodlari(), "Value", "Text", "").ToList();
                model.Teminat.ErgoServisTurleri = new SelectList(TeklifProvider.ErgoServisTurleri(), "Value", "Text", "").ToList();
                model.Teminat.GroupamaYakinlikDereceleri = new SelectList(TeklifProvider.GroupamaYakinlikDereceleri(), "Value", "Text", "").ToList();
                model.Teminat.GroupamaRizikoFiyatlari = new SelectList(TeklifProvider.GroupamaRizikoFiyatlari(), "Value", "Text", "").ToList();
                model.Teminat.GroupamaYHIMSKodlari = new SelectList(TeklifProvider.GroupamaYHIMSSecimleri(), "Value", "Text", "").ToList();
                var GroupamaYHIMSBasamakKodlari = this.GetGroupamaYHIMSBasamaklar();
                if (GroupamaYHIMSBasamakKodlari.Count > 0)
                {
                    model.Teminat.GroupamaYHIMSBasamakKodlari = new SelectList(GroupamaYHIMSBasamakKodlari, "key", "value", "").ListWithOptionLabel();
                }
                else
                {
                    model.Teminat.GroupamaYHIMSBasamakKodlari = new List<SelectListItem>();
                }

                model.Teminat.GroupamaManeviDahilMi = false;
                model.KaskoDigerTeklif = new KaskoDigerTeklifModel();
                model.KaskoDigerTeklif.DigerTeklifVarMi = false;

                var SirketListesi = _TUMService.GetListTUMDetay();
                model.KaskoDigerTeklif.SigortaSirketleri = new SelectList(SirketListesi, "Kodu", "Unvani", "").ListWithOptionLabel();
                model.KaskoDigerTeklif.DigerTeklifler = new List<DigerTeklifModel>();

                model.Teminat.IkameTuruGulf = "";
                model.Teminat.YakitTuruGulf = "";
                //  model.Teminat.HukuksalKorumaGulf = "2";

                model.Teminat.HKBedeli = "1";
                model.Teminat.UnicoKilitBedeli = KilitBedeli.KB800;
                model.Teminat.UnicoGenisletilmisCam = true;
                model.Teminat.UnicoManeviTazminat = false;
                model.Teminat.UnicoHasarsizlikKorumaKlozu = false;
                model.Teminat.UnicoKaskoMuafiyeti = "2";
                model.Teminat.UnicoKiralikAracmi = false;
                model.Teminat.UnicoSurucuKursuAracimi = false;
                model.Teminat.UnicoTEBUyesi = false;
                model.Teminat.UnicoTekSurucuMu = false;
                model.Teminat.UnicoDepremSelMuafiyeti = "2";
                model.Teminat.UnicoMeslekKodu = "0";
                model.Teminat.UnicoSurucuSayisi = "1";
                model.Teminat.UnicoIkameSecenegi = "1";
                model.Teminat.AxaHayatTeminatLimiti = "";
                //model.Teminat.AxaIkameSecimi = "";
                model.Teminat.AxaAsistansHizmeti = "I";
                model.Teminat.AxaAracaBagliKaravanMi = false;
                model.Teminat.KullanimGelirKaybiVarMi = false;
            }

            model.Teminat.IkameTurleriGulf = new SelectList(TeklifProvider.GulfIkameTurleri(), "Value", "Text", model.Teminat.IkameTuruGulf).ToList();
            model.Teminat.YakitTurleriGulf = new SelectList(TeklifProvider.GulfYakitTurleri(), "Value", "Text", model.Teminat.YakitTuruGulf).ToList();
            //model.Teminat.HukuksalKorumaBedelleriGulf = new SelectList(TeklifProvider.GulfHukuksalKorumaBedelleri(), "Value", "Text", model.Teminat.HukuksalKorumaGulf).ToList();

            var gulfMeslekListesi = _TeklifService.GetTUMMeslekList(TeklifUretimMerkezleri.GULF);
            model.Teminat.MeslekListesiGulf = new SelectList(gulfMeslekListesi, "CR_MeslekKodu", "CR_Aciklama", "").ListWithOptionLabel();
            model.Teminat.NipponServisTurleri = new SelectList(TeklifProvider.TurkNipponKaskoServisTurleri(), "Value", "Text", model.Teminat.NipponServisTuru).ListWithOptionLabel();
            model.Teminat.NipponMuafiyetTutarlari = new SelectList(TeklifProvider.TurkNipponMuafiyetTutarlari(), "Value", "Text", model.Teminat.NipponMuafiyetTutari).ToList();

            //Unico Ozel Alanlar
            var hkBedelleri = _TeklifService.getHukuksalKorumaBedelList();
            model.Teminat.HKBedelleri = new SelectList(hkBedelleri, "Id", "Text", model.Teminat.HKBedeli).ListWithOptionLabel();
            model.Teminat.UnicoKilitBedelleri = new SelectList(UNICOCommon.KilitBedelleri(), "Value", "Text", model.Teminat.UnicoKilitBedeli).ToList();
            model.Teminat.UnicoKaskoMuafiyetleri = new SelectList(UNICOCommon.KaskoMuafiyetList(), "Value", "Text", model.Teminat.UnicoKaskoMuafiyeti).ToList();
            model.Teminat.UnicoIkameSecenekleri = new SelectList(UNICOCommon.IkameSecenekleri(), "Value", "Text", model.Teminat.UnicoIkameSecenegi).ToList();
            model.Teminat.UnicoDepremSelMuafiyetleri = new SelectList(UNICOCommon.DepremSelMuafiyetleri(), "Value", "Text", model.Teminat.UnicoDepremSelMuafiyeti).ToList();
            model.Teminat.UnicoAksesuarTurleri = new SelectList(UNICOCommon.AksesuarTurleri(), "Value", "Text", model.Teminat.UnicoAksesuarTuru).ToList();
            model.Teminat.UnicoMeslekler = new SelectList(UNICOCommon.UnicoMeslekList(), "Value", "Text", model.Teminat.UnicoMeslekKodu).ToList();
            //Axa ya özel alanlar

            model.Teminat.AxaHayatTeminatLimitleri = new SelectList(TeklifProvider.AxaHayatTeminatLimitleri(), "Value", "Text", model.Teminat.AxaHayatTeminatLimiti).ToList();
            model.Teminat.AxaAsistansHizmetleri = new SelectList(TeklifProvider.AxaAsistansHizmetleri(), "Value", "Text", model.Teminat.AxaAsistansHizmeti).ToList();
            //model.Teminat.AxaIkameSecimleri = new SelectList(TeklifProvider.AxaIkameSecimleri(), "Value", "Text", model.Teminat.AxaIkameSecimi).ToList();
            model.Teminat.AxaSorumlulukLimitleri = new SelectList(TeklifProvider.AxaSorumlulukLimitleri(), "Value", "Text", model.Teminat.AxaSorumlulukLimiti).ToList();
            model.Teminat.AxaMuafiyetTutarlari = new SelectList(TeklifProvider.AxaMuafiyetTutarlari(), "Value", "Text", model.Teminat.AxaMuafiyetTutari).ToList();
            model.Teminat.AxaDepremSelKoasuranslari = new SelectList(TeklifProvider.AxaDepremSelKoasuranslari(), "Value", "Text", model.Teminat.AxaDepremSelKoasuransi).ToList();
            model.Teminat.AxaYeniDegerKlozlari = new SelectList(TeklifProvider.AxaYeniDegerKlozlari(), "Value", "Text", model.Teminat.AxaYeniDegerKlozu).ToList();
            model.Teminat.AxaOnarimSecimleri = new SelectList(TeklifProvider.AxaOnarimSecimleri(), "Value", "Text", model.Teminat.AxaOnarimSecimi).ToList();

            List<SelectListItem> sompoArtiTeminatDegerleri = new List<SelectListItem>();
            sompoArtiTeminatDegerleri.Add(new SelectListItem() { Value = "1", Text = "Plan1 (60 TL)" });
            sompoArtiTeminatDegerleri.Add(new SelectListItem() { Value = "2", Text = "Plan2 (100 TL)" });
            sompoArtiTeminatDegerleri.Add(new SelectListItem() { Value = "3", Text = "Plan3 (160 TL)" });


            model.Teminat.SompoArtiTeminatDegerleri = new SelectList(sompoArtiTeminatDegerleri, "Value", "Text", model.Teminat.SompoArtiTeminatDegeri).ListWithOptionLabel();
            #endregion

            #region Daini Mürtein
            model.DainiMurtein = new KaskoDainiMurtein();
            model.DainiMurtein.DainiMurtein = false;
            List<KeyValueItem<int, string>> kurumTipleri = new List<KeyValueItem<int, string>>();
            kurumTipleri.Add(new KeyValueItem<int, string>(0, babonline.PleaseSelect));
            kurumTipleri.Add(new KeyValueItem<int, string>(1, babonline.Bank));
            kurumTipleri.Add(new KeyValueItem<int, string>(2, babonline.FinancialInstitution));
            // kurumTipleri.Add(new KeyValueItem<int, string>(3, "DİĞER"));
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

            #region TUM IMAGES

            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.KaskoSigortasi);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

            #endregion

            #region Odeme
            model.Odeme = new KaskoTeklifOdemeModel();
            model.KrediKarti = new KrediKartiOdemeModel();
            model.Odeme.OdemeSekli = false;
            model.Odeme.OdemeTipi = OdemeTipleri.BlokeliKrediKarti;
            model.Odeme.TaksitSayisi = (byte)9;
            model.KrediKarti.KK_OdemeSekli = OdemeSekilleri.Vadeli;
            if (id > 0)
            {
                model.Odeme.OdemeTipi = teklif.GenelBilgiler.OdemeTipi.HasValue ? teklif.GenelBilgiler.OdemeTipi.Value : OdemeTipleri.BlokeliKrediKarti;
                model.Odeme.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : (byte)9;
                model.KrediKarti.KK_OdemeSekli = teklif.GenelBilgiler.OdemeSekli.HasValue ? teklif.GenelBilgiler.OdemeSekli.Value : OdemeSekilleri.Vadeli;
            }


            model.Odeme.TaksitSayilari = new List<SelectListItem>();
            model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.Odeme.OdemeTipi).ToList();

            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "1", Value = "1" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "2", Value = "2" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "3", Value = "3" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "4", Value = "4" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "5", Value = "5" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "6", Value = "6" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "7", Value = "7" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "8", Value = "8" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "9", Value = "9" });
            model.Odeme.TaksitSayilari[model.Odeme.TaksitSayisi - 1].Selected = true;



            model.KrediKarti.OdemeSekilleri = new SelectList(OdemeSekilleri.OdemeSekilleriList(), "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();
            model.KrediKarti.KK_OdemeTipi = model.Odeme.OdemeTipi;
            model.KrediKarti.OdemeTipleri = new List<SelectListItem>();
            model.KrediKarti.OdemeTipleri = model.Odeme.OdemeTipleri;
            model.KrediKarti.TaksitSayisi = model.Odeme.TaksitSayisi;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari = model.Odeme.TaksitSayilari;
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
            DetayKaskoModel model = new DetayKaskoModel();

            #region Teklif Genel

            KaskoTeklif kaskoTeklif = new KaskoTeklif(id);
            model.TeklifId = id;
            model.KaskoDigerTeklif = new KaskoDigerTeklifModel();
            model.KaskoDigerTeklif.TeklifId = id;
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

            //Araç bilgileri
            model.Arac = base.DetayAracModel(kaskoTeklif.Teklif);

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
            model.DainiMurtein = new KaskoDainiMurtein();
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
                                //case 3: model.DainiMurtein.KurumTipiAdi = "DİĞER"; break;
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
            TeklifTeminatDoldur(model, kaskoTeklif.Teklif);
            #endregion

            #region Teklif Fiyat

            model.Fiyat = KaskoFiyat(kaskoTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();

            model.KrediKarti.KK_OdemeSekli = kaskoTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.KK_OdemeTipi = kaskoTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = kaskoTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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

            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = kaskoTeklif.Teklif.GenelBilgiler.BrutPrim;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            #endregion

            model.KaskoDigerTeklif = new KaskoDigerTeklifModel();
            model.KaskoDigerTeklif.DigerTeklifler = new List<DigerTeklifModel>();
            model.KaskoDigerTeklif.DigerTeklifVarMi = false;
            DigerTeklifModel teklifModel = new DigerTeklifModel();
            if (id > 0)
            {
                if (kaskoTeklif.Teklif.GenelBilgiler.TeklifDigerSirketlers.Count > 0)
                {
                    model.KaskoDigerTeklif.DigerTeklifVarMi = true;
                    foreach (var item in kaskoTeklif.Teklif.GenelBilgiler.TeklifDigerSirketlers)
                    {
                        teklifModel = new DigerTeklifModel();
                        teklifModel.TeklifTutar = item.BrutPrim;
                        teklifModel.TaksitSayisi = item.TaksitSayisi;
                        teklifModel.SirketTeklifNo = item.SigortaSirketTeklifNo;
                        teklifModel.HasarsizlikIndirimSurprim = item.HasarsizlikIndirim;
                        teklifModel.SirketKod = item.SigortaSirketKodu.ToString();
                        model.KaskoDigerTeklif.DigerTeklifler.Add(teklifModel);
                    }
                }
            }
            var SirketListesi = _TUMService.GetListTUMDetay();
            model.KaskoDigerTeklif.SigortaSirketleri = new SelectList(SirketListesi, "Kodu", "Unvani", teklifModel.SirketKod).ListWithOptionLabel();


            return View(model);
        }

        public ActionResult Police(int id)
        {
            DetayKaskoModel model = new DetayKaskoModel();

            #region Teklif Genel

            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif kaskoTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = kaskoTeklif.GenelBilgiler.TeklifId;
            model.PoliceId = id;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(kaskoTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(kaskoTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Teklif Arac

            //Araç bilgileri
            model.Arac = base.DetayAracModel(kaskoTeklif);

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
            model.DainiMurtein = new KaskoDainiMurtein();
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
                                //case 3: model.DainiMurtein.KurumTipiAdi = "DİĞER"; break;
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

            #region Teklif Odeme

            model.OdemeBilgileri = base.KaskoPoliceOdemeModel(teklif);
            model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFGenelSartlari;

            if (teklif.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.MAPFRE)
            {
                model.OdemeBilgileri.DekontPDFGoster = teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti;
            }
            else
            {
                model.OdemeBilgileri.DekontPDFGoster = true;
            }


            if (teklif.GenelBilgiler.PDFDekont != null)
            {
                model.OdemeBilgileri.DekontPDFGoster = true;
                model.OdemeBilgileri.DekontPDF = teklif.GenelBilgiler.PDFDekont;
            }
            else
            {
                model.OdemeBilgileri.DekontPDFGoster = false;
            }


            #endregion

            #region Teminatlar

            // ==== İsteğe bağlı ve zorunlu teminatlar dolduruluyor ==== //
            TeklifTeminatDoldur(model, kaskoTeklif);

            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult OdemeAl(OdemeKaskoModel model)
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

        [HttpPost]
        public ActionResult Hesapla(KaskoModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (!model.Teminat.TasinanYuk_Teminati)
                {
                    if (ModelState["Teminat.TasinanYukKademe"] != null)
                        ModelState["Teminat.TasinanYukKademe"].Errors.Clear();

                    if (ModelState["Teminat.TasinanYukAciklama"] != null)
                        ModelState["Teminat.TasinanYukAciklama"].Errors.Clear();

                    if (ModelState["Teminat.TasinanYukBedel"] != null)
                        ModelState["Teminat.TasinanYukBedel"].Errors.Clear();
                }

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
                if (model.Teminat != null && model.Teminat.LPG_Arac_Orjinalmi)
                {
                    if (ModelState["Teminat.LPGAracModel.Markasi"] != null)
                        ModelState["Teminat.LPGAracModel.Markasi"].Errors.Clear();

                    if (ModelState["Teminat.LPGAracModel.Bedeli"] != null)
                        ModelState["Teminat.LPGAracModel.Bedeli"].Errors.Clear();
                }
                if (model.Teminat != null && model.Teminat.Elektrikli_Arac)
                {
                    if (ModelState["Teminat.ElektirkliAracModel.PilId"] != null)
                        ModelState["Teminat.ElektirkliAracModel.PilId"].Errors.Clear();

                    if (ModelState["Teminat.ElektirkliAracModel.Bedeli"] != null)
                        ModelState["Teminat.ElektirkliAracModel.Bedeli"].Errors.Clear();
                }

                //if (model.DainiMurtein != null && !model.DainiMurtein.DainiMurtein)
                //{
                //    if (ModelState["DainiMurtein.KimlikTipi"] != null)
                //        ModelState["DainiMurtein.KimlikTipi"].Errors.Clear();
                //    if (ModelState["DainiMurtein.KimlikNo"] != null)
                //        ModelState["DainiMurtein.KimlikNo"].Errors.Clear();
                //    if (ModelState["DainiMurtein.Unvan"] != null)
                //        ModelState["DainiMurtein.Unvan"].Errors.Clear();
                //}

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

                if (_AktifKullaniciService.ProjeKodu != TVMProjeKodlari.Mapfre)
                {
                    if (ModelState["Teminat.AMSKodu"] != null)
                        ModelState["Teminat.AMSKodu"].Errors.Clear();
                    if (ModelState["Teminat.OlumSakatlikTeminat"] != null)
                        ModelState["Teminat.OlumSakatlikTeminat"].Errors.Clear();
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
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    //#region Arac Değeri Kontrol
                    //// ==== AracDEgeri kontrol ediliyor ==== //

                    //if (!AracDegerKontrolServer(model.Arac))
                    //{ return Json(new { id = 0, hata = "Arac değeri 10% den fazla değiştirilemez" }); }
                    //#endregion

                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.KaskoSigortasi,
                                                                         model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);


                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    teklif.AddSoru(KaskoSorular.YeniIsMi, model.Hazirlayan.YeniIsMi);

                    teklif.AddSoru(TrafikSorular.SENumaratipi, model.Musteri.SigortaEttiren.MusteriTelTipKodu.ToString());

                    teklif.AddSoru(KaskoSorular.GulfKimlikNo, model.Musteri.SigortaEttiren.GulfKimlikNo);
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
                    teklif.Arac.TramerBelgeNo = model.Arac.TramerBelgeNumarasi;
                    teklif.Arac.TramerBelgeTarihi = Convert.ToDateTime(model.Arac.TramerBelgeTarihi);
                    // ==== Arac değeri ve kişi sayısı ekleniyor ==== //
                    teklif.Arac.KoltukSayisi = model.Arac.KisiSayisi;
                    if (!String.IsNullOrEmpty(model.Arac.AracDeger))
                        teklif.Arac.AracDeger = Convert.ToDecimal(model.Arac.AracDeger.Replace(".", "").Replace(",", ""));

                    if (!String.IsNullOrEmpty(model.Arac.AnadoluKullanimTip))
                    {
                        teklif.AddSoru(KaskoSorular.AnadoluKullanimTipi, model.Arac.AnadoluKullanimTip);
                    }
                    if (!String.IsNullOrEmpty(model.Arac.AnadoluKullanimSekli))
                    {
                        teklif.AddSoru(KaskoSorular.AnadoluKullanimSekli, model.Arac.AnadoluKullanimSekli);
                    }
                    if (!String.IsNullOrEmpty(model.Arac.AnadoluMarkaKodu))
                    {
                        teklif.AddSoru(KaskoSorular.AnadoluMarkaKodu, model.Arac.AnadoluMarkaKodu);
                    }

                    teklif.AddSoru(KaskoSorular.AnadoluIkameTuru, model.Arac.IkameTuruAnadolu);

                    if (!String.IsNullOrEmpty(model.Arac.UygulananKademe))
                    {
                        teklif.Arac.TarifeBasamagi = Convert.ToInt16(model.Arac.UygulananKademe);
                        teklif.GenelBilgiler.TarifeBasamakKodu = Convert.ToInt16(model.Arac.UygulananKademe);
                        teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToDecimal(model.Arac.HasarsizlikIndirim);
                        teklif.GenelBilgiler.HasarSurprimYuzdesi = Convert.ToDecimal(model.Arac.HasarSurprim);
                    }
                    #endregion

                    #region EskiPoliçe
                    // ==== Eski Poliçe ==== //
                    teklif.AddSoru(KaskoSorular.Eski_Police_VarYok, model.EskiPolice.EskiPoliceVar);
                    teklif.AddSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, model.EskiPolice.SigortaSirketiKodu);
                    teklif.AddSoru(KaskoSorular.Eski_Police_Acente_No, model.EskiPolice.AcenteNo);
                    teklif.AddSoru(KaskoSorular.Eski_Police_No, model.EskiPolice.PoliceNo);
                    teklif.AddSoru(KaskoSorular.Eski_Police_Yenileme_No, model.EskiPolice.YenilemeNo);
                    #endregion

                    #region Taşıma Yetki Belgesi
                    // ==== Taşıma yetki belgesi ve taşıyıcı sorumluluk ==== //
                    teklif.AddSoru(KaskoSorular.Tasima_Yetki_Belgesi_VarYok, model.Tasiyici.YetkiBelgesi);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_VarYok, model.Tasiyici.Sorumluluk);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, model.Tasiyici.SigortaSirketiKodu);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Acente_No, model.Tasiyici.AcenteNo);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Police_No, model.Tasiyici.PoliceNo);
                    teklif.AddSoru(KaskoSorular.Tasiyici_Sorumluluk_Yenileme_No, model.Tasiyici.YenilemeNo);
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
                    if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_AMS_Kodu, model.Teminat.AMSKodu);
                        teklif.AddSoru(KaskoSorular.Teminat_Olum_Sakatlik, model.Teminat.OlumSakatlikTeminat);
                    }
                    else
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_IMM_Kademe, model.Teminat.IMMKodu.ToNullSafeString());
                        teklif.AddSoru(KaskoSorular.Teminat_FK_Kademe, model.Teminat.FKKodu.ToNullSafeString());
                        teklif.AddSoru(KaskoSorular.Kasko_Turu, model.Teminat.Kasko_Turu.ToNullSafeString());
                        teklif.AddSoru(KaskoSorular.Servis_Turu, model.Teminat.Kasko_Servis.ToNullSafeString());
                        teklif.AddSoru(KaskoSorular.Yedek_Parca_Turu, model.Teminat.Kasko_Yedek_Parca.ToNullSafeString());

                        if (model.Teminat.IMMKodu != null)
                        {
                            var IMMKombineManeviDahilMi = _CRService.IMMKombineManeviDahiMi(Convert.ToInt32(model.Teminat.IMMKodu));
                            if (IMMKombineManeviDahilMi)
                            {
                                teklif.AddSoru(KaskoSorular.ManeviDahilMi, true);
                            }
                            else
                            {
                                teklif.AddSoru(KaskoSorular.ManeviDahilMi, false);
                            }
                        }
                    }

                    teklif.AddSoru(KaskoSorular.Meslek, model.Teminat.MeslekKodu);

                    // ==== İsteğe bağlı teminatlar ==== //
                    teklif.AddSoru(KaskoSorular.GLKHHT, model.Teminat.GLKHHT);
                    teklif.AddSoru(KaskoSorular.Sel_Su_VarYok, model.Teminat.Sel_Su);
                    teklif.AddSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, model.Teminat.Hasarsizlik_Koruma);
                    teklif.AddSoru(KaskoSorular.Calinma_VarYok, model.Teminat.Calinma);
                    teklif.AddSoru(KaskoSorular.Hayvanlarin_Verecegi_Zarar_ZarYok, model.Teminat.Hayvanlarin_Verecegi_Zarar_Teminati);
                    teklif.AddSoru(KaskoSorular.Alarm_VarYok, model.Teminat.Alarm_Teminati);
                    teklif.AddSoru(KaskoSorular.Anahtar_Kaybi_VarYok, model.Teminat.Anahtar_Kaybi_Teminati);
                    teklif.AddSoru(KaskoSorular.Yangin_VarYok, model.Teminat.Yangin);
                    teklif.AddSoru(KaskoSorular.Eskime_VarYok, model.Teminat.Eskime_Payi_Teminati);
                    teklif.AddSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, model.Teminat.Yutr_Disi_Teminat);

                    teklif.AddSoru(KaskoSorular.Saglik_VarYok, model.Teminat.Saglik);
                    teklif.AddSoru(KaskoSorular.Deprem_VarYok, model.Teminat.Deprem);

                    teklif.AddSoru(KaskoSorular.Seylap, model.Teminat.Seylap);
                    teklif.AddSoru(KaskoSorular.KiymetKazanma, model.Teminat.KiymetKazanma);
                    teklif.AddSoru(KaskoSorular.AnahtarliCalinma, model.Teminat.AnahtarliCalinma);
                    teklif.AddSoru(KaskoSorular.SigaraYanigi, model.Teminat.SigaraYanigi);
                    teklif.AddSoru(KaskoSorular.YetkiliOlmayanCekilme, model.Teminat.YetkiliOlmayanCekilme);
                    teklif.AddSoru(KaskoSorular.CamMuafiyetiKaldirilsinMi, model.Teminat.CamMuafiyetiKaldirilsinMi);

                    if (model.Teminat.NipponYetkiliIndirimi.HasValue)
                    {
                        teklif.AddSoru(KaskoSorular.NipponYetkiliInidirimi, model.Teminat.NipponYetkiliIndirimi.Value);
                    }
                    teklif.AddSoru(KaskoSorular.TurkNipponServisTuru, model.Teminat.NipponServisTuru);
                    teklif.AddSoru(KaskoSorular.TurkNipponMuafiyetTutari, model.Teminat.NipponMuafiyetTutari);
                    // ==== Hukuksal Koruma Zorunlu ==== //
                    teklif.AddSoru(KaskoSorular.Hukuksal_Koruma_VarYok, true);

                    //Ikame Turu
                    teklif.AddSoru(KaskoSorular.Ikame_Turu, model.Teminat.IkameTuru);
                    if (!String.IsNullOrEmpty(model.Teminat.IkameTuru) && model.Teminat.IkameTuru != "0")
                        teklif.AddSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, true);

                    // ==== Sağlık teminatı kişi sayısı ==== //
                    if (model.Teminat.Saglik)
                        teklif.AddSoru(KaskoSorular.Saglik_Kisi_Sayisi, model.Teminat.Saglik_Kisi_Sayisi.ToNullSafeString());

                    // ==== Yurtdışı teminat süresi ==== //
                    if (model.Teminat.Yutr_Disi_Teminat)
                        teklif.AddSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, model.Teminat.Yurt_Disi_Teminati_Sure.ToNullSafeString());

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
                    if (model.Teminat.TasinanYuk_Teminati && !string.IsNullOrEmpty(model.Teminat.TasinanYukKademe))
                    {
                        teklif.AddSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, model.Teminat.TasinanYuk_Teminati);
                        teklif.AddAracEkSoru(TeklifUretimMerkezleri.MAPFRE, MapfreKaskoEkSoruTipleri.TASINAN_YUK,
                                    model.Teminat.TasinanYukKademe, model.Teminat.TasinanYukAciklama, model.Teminat.TasinanYukBedel, 0);
                    }

                    if (model.Teminat.TicariBireysel) //Sompo Japan Kasko Türü
                    {
                        teklif.AddSoru(KaskoSorular.SompoJapanKaskoTuru, model.Teminat.TicariBireysel);
                        teklif.AddSoru(KaskoSorular.SompoJapanFaaliyetKodu, model.Teminat.FaaliyetKodu);
                    }

                    //Groupoamaya Özel Teminatlar
                    teklif.AddSoru(KaskoSorular.AsistansPlusPaketi, model.Teminat.AsistansPlusPaketi);
                    teklif.AddSoru(KaskoSorular.KazaDestekVarmi, model.Teminat.KazaDestekVarMi);
                    teklif.AddSoru(KaskoSorular.PrimKoruma, model.Teminat.PrimKoruma);
                    teklif.AddSoru(KaskoSorular.AcenteOzelIndirimi, model.Teminat.AcenteOzelIndirimi);
                    teklif.AddSoru(KaskoSorular.Kolonlar, model.Teminat.Kolonlar);

                    if (model.Teminat.Kolonlar)
                    {
                        teklif.AddSoru(KaskoSorular.KolonBedel, model.Teminat.KolonBedel);
                        teklif.AddSoru(KaskoSorular.KolonMarka, model.Teminat.KolonMarka);
                    }
                    teklif.AddSoru(KaskoSorular.PesinIndirimi, model.Teminat.PesinIndirimi);
                    teklif.AddSoru(KaskoSorular.YurtdisiKasko, model.Teminat.YurtDisiKasko);
                    teklif.AddSoru(KaskoSorular.GroupamaElitKaskomu, model.Teminat.ElitKaskomu);

                    teklif.AddSoru(KaskoSorular.ErgoMeslekKodu, model.Teminat.ErgoMeslekKodu);
                    teklif.AddSoru(KaskoSorular.ErgoServisSecenegi, model.Teminat.ErgoServisTuru);
                    teklif.AddSoru(KaskoSorular.GroupamaMeslekKodu, model.Teminat.GroupamaMeslekKodu);
                    teklif.AddSoru(KaskoSorular.GroupamaYakinlikDerecesi, model.Teminat.YakinlikDerecesi);
                    teklif.AddSoru(KaskoSorular.GroupamaRizikoFiyati, model.Teminat.RizikoFiyati);

                    if (model.Teminat.GroupamaAracHukuksalKorumaBedel.HasValue)
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaAracHukuksalKoruma, model.Teminat.GroupamaAracHukuksalKorumaBedel.Value);
                    }
                    else
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaAracHukuksalKoruma, 0);
                    }
                    if (model.Teminat.GroupamaSurucuHukuksalKorumaBedel.HasValue)
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaSurucuHukuksalKoruma, model.Teminat.GroupamaSurucuHukuksalKorumaBedel.Value);
                    }
                    else
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaSurucuHukuksalKoruma, 0);
                    }

                    teklif.AddSoru(KaskoSorular.GroupamaYHIMSKodu, model.Teminat.GroupamaYHIMSKodu);
                    teklif.AddSoru(KaskoSorular.GroupamaYHIMSBasamakKodu, model.Teminat.GroupamaYHIMSBasamakKodu);

                    if (model.Teminat.GroupamaYHIMSSerbestLimit.HasValue)
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaYHIMSSerbestLimit, model.Teminat.GroupamaYHIMSSerbestLimit.Value);
                    }
                    else
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaYHIMSSerbestLimit, 0);
                    }

                    if (model.Teminat.KazaDestekVarMi)
                    {
                        teklif.AddSoru(KaskoSorular.TeminatLimiti, model.Teminat.GroupamaTeminatLimiti);
                    }
                    teklif.AddSoru(KaskoSorular.GroupamaManeviDahilMi, model.Teminat.GroupamaManeviDahilMi);
                    if (model.Teminat.GroupamaManeviDahilBedeli.HasValue)
                    {
                        teklif.AddSoru(KaskoSorular.GroupamaManeviDahilBedeli, model.Teminat.GroupamaManeviDahilBedeli.Value);
                    }
                    //==============Groupoamaya Özel Teminatlar
                    teklif.AddSoru(KaskoSorular.SompoArtiTeminatPlani, model.Teminat.SompoArtiTeminatPlani);
                    teklif.AddSoru(KaskoSorular.SompoArtiTeminatPlanDegeri, model.Teminat.SompoArtiTeminatDegeri);

                    // ==== Arac lpg li ise orjinalliği ve marka,bedel bilgileri alınıyor. ==== //
                    teklif.AddSoru(KaskoSorular.LPG_VarYok, model.Teminat.LPGLi_Arac);

                    if (model.Teminat.LPGLi_Arac)
                    {
                        teklif.AddSoru(KaskoSorular.LPG_Arac_Orjinalmi, model.Teminat.LPG_Arac_Orjinalmi);

                        if (!model.Teminat.LPG_Arac_Orjinalmi)
                        {
                            teklif.AddSoru(KaskoSorular.LPG_Markasi, model.Teminat.LPGAracModel.Markasi);
                            teklif.AddSoru(KaskoSorular.LPG_Bedel, model.Teminat.LPGAracModel.Bedeli.Value.ToString());
                        }

                        //if (model.Teminat.LPG_Arac_Orjinalmi)
                        //{
                        //    teklif.AddSoru(KaskoSorular.LPG_Markasi, model.Teminat.LPGAracModel.Markasi);
                        //    teklif.AddSoru(KaskoSorular.LPG_Bedel, model.Teminat.LPGAracModel.Bedeli.Value);
                        //}
                    }
                    if (model.Teminat.Elektrikli_Arac)
                    {
                        teklif.AddSoru(KaskoSorular.ElektrikliArac, model.Teminat.Elektrikli_Arac);
                        teklif.AddSoru(KaskoSorular.ElektrikliAracPilId, model.Teminat.ElektirkliAracModel.PilId);
                        teklif.AddSoru(KaskoSorular.ElektrikliAracBedeli, model.Teminat.ElektirkliAracModel.Bedeli);
                    }


                    //Gulf Özel alanlar
                    teklif.AddSoru(KaskoSorular.GulfIkameTuru, model.Teminat.IkameTuruGulf);
                    teklif.AddSoru(KaskoSorular.GulfYakitTuru, model.Teminat.YakitTuruGulf);
                    //teklif.AddSoru(KaskoSorular.GulfHukuksalKorumaBedeli, model.Teminat.HukuksalKorumaGulf);
                    teklif.AddSoru(KaskoSorular.GulfMeslekKodu, model.Teminat.MeslekKoduGulf);

                    //Unico Özel alanlar
                    teklif.AddSoru(KaskoSorular.UnicoManeviTazminat, model.Teminat.UnicoManeviTazminat);
                    teklif.AddSoru(KaskoSorular.UnicokilitBedeli, model.Teminat.UnicoKilitBedeli);
                    teklif.AddSoru(KaskoSorular.UnicoGenisletilmisCam, model.Teminat.UnicoGenisletilmisCam);
                    teklif.AddSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, model.Teminat.HKBedeli);
                    teklif.AddSoru(KaskoSorular.UnicoAksesuarTuru, model.Teminat.UnicoAksesuarTuru);
                    teklif.AddSoru(KaskoSorular.UnicoDepremSelMuafiyeti, model.Teminat.UnicoDepremSelMuafiyeti);
                    teklif.AddSoru(KaskoSorular.UNICOMeslekKodu, model.Teminat.UnicoMeslekKodu);
                    teklif.AddSoru(KaskoSorular.EngelliAracimi, model.Teminat.EngelliAraciMi);
                    teklif.AddSoru(KaskoSorular.UnicoHasarsizlikKorumaKlozu, model.Teminat.UnicoHasarsizlikKorumaKlozu);
                    teklif.AddSoru(KaskoSorular.UnicoIkameSecenegi, model.Teminat.UnicoIkameSecenegi);
                    teklif.AddSoru(KaskoSorular.UnicoKaskoMuafiyeti, model.Teminat.UnicoKaskoMuafiyeti);
                    teklif.AddSoru(KaskoSorular.UnicoKiralikAraciMi, model.Teminat.UnicoKiralikAracmi);
                    teklif.AddSoru(KaskoSorular.UnicoSurucuKursuAraciMi, model.Teminat.UnicoSurucuKursuAracimi);
                    teklif.AddSoru(KaskoSorular.UnicoSurucuSayisi, model.Teminat.UnicoSurucuSayisi);
                    teklif.AddSoru(KaskoSorular.UnicoTEBUyesiMi, model.Teminat.UnicoTEBUyesi);
                    teklif.AddSoru(KaskoSorular.UnicoTekSurucuMu, model.Teminat.UnicoTekSurucuMu);
                    teklif.AddSoru(KaskoSorular.UnicoYeniDegerklozu, model.Teminat.UnicoYeniDegerklozu);

                    //Axa ya özel sorular
                    teklif.AddSoru(KaskoSorular.AxaHayatTeminatLimiti, model.Teminat.AxaHayatTeminatLimiti);
                    teklif.AddSoru(KaskoSorular.AxaAracaBagliKaravanVarMi, model.Teminat.AxaAracaBagliKaravanMi);
                    if (model.Teminat.AxaAracaBagliKaravanMi)
                    {
                        teklif.AddSoru(KaskoSorular.AxaAracaBagliKaravanBedeli, model.Teminat.AxaAracaBagliKaravanBedel);
                    }

                    teklif.AddSoru(KaskoSorular.KullanimGelirKaybiVarMi, model.Teminat.KullanimGelirKaybiVarMi);
                    if (model.Teminat.KullanimGelirKaybiVarMi)
                    {
                        teklif.AddSoru(KaskoSorular.KullanimGelirKaybiBedel, model.Teminat.KullanimGelirKaybiBedel);
                    }
                    //if (!String.IsNullOrEmpty(model.Teminat.AxaIkameSecimi))
                    //{
                    //    teklif.AddSoru(KaskoSorular.AxaIkameSecimi, model.Teminat.AxaIkameSecimi);
                    //}
                    teklif.AddSoru(KaskoSorular.AxaAsistansHizmeti, model.Teminat.AxaAsistansHizmeti);

                    teklif.AddSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, model.Teminat.KisiselEsya);
                    teklif.AddSoru(KaskoSorular.HDIRayicBedelKoruma, model.Teminat.HDIRayicDegerKoruma);
                    teklif.AddSoru(KaskoSorular.HDIPatlayiciParlayici, model.Teminat.HDIPatlayiciMadde);
                    teklif.AddSoru(KaskoSorular.HataliAkaryakitAlimi, model.Teminat.YanlisAkaryakitDolumu);

                    if (model.Arac.Model >= TurkeyDateTime.Now.Year - 1)
                    {
                        teklif.AddSoru(KaskoSorular.AxaCamFilmiLogo, model.Teminat.AxaCamFilmiLogo);
                        teklif.AddSoru(KaskoSorular.AxaOnarimSecimi, model.Teminat.AxaOnarimSecimi);
                        teklif.AddSoru(KaskoSorular.AxaPlakaYeniKayitMi, model.Teminat.AxaPlakaYeniKayitMi);
                        if (!String.IsNullOrEmpty(model.Teminat.AxaYeniDegerKlozu))
                        {
                            teklif.AddSoru(KaskoSorular.AxaYeniDegerKlozu, model.Teminat.AxaYeniDegerKlozu);
                        }
                        if (!String.IsNullOrEmpty(model.Teminat.AxaDepremSelKoasuransi))
                        {
                            teklif.AddSoru(KaskoSorular.AxaDepremSelKoasuransi, model.Teminat.AxaDepremSelKoasuransi);
                        }
                        if (!String.IsNullOrEmpty(model.Teminat.AxaMuafiyetTutari))
                        {
                            teklif.AddSoru(KaskoSorular.AxaMuafiyetTutari, model.Teminat.AxaMuafiyetTutari);
                        }
                        if (!String.IsNullOrEmpty(model.Teminat.AxaSorumlulukLimiti))
                        {
                            teklif.AddSoru(KaskoSorular.AxaSorumlulukLimiti, model.Teminat.AxaSorumlulukLimiti);
                        }
                    }
                    #endregion

                    #region Teklif return

                    IKaskoTeklif kaskoTeklif = new KaskoTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            kaskoTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

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
                    //model.KrediKarti = new KrediKartiOdemeModel();
                    //model.KrediKarti.KK_OdemeSekli = model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli;
                    //model.KrediKarti.KK_OdemeTipi = model.Odeme.OdemeTipi;
                    //if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Vadeli)
                    //{
                    //    model.KrediKarti.TaksitSayisi = model.Odeme.TaksitSayisi;
                    //}

                    IsDurum isDurum = kaskoTeklif.Hesapla(teklif);
                    #endregion

                    return Json(new { id = isDurum.IsId, g = isDurum.Guid });
                }

                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
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

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        [HttpPost]
        public ActionResult DigerTeklifKaydetDetay(DetayKaskoModel model)
        {
            if (model.KaskoDigerTeklif.DigerTeklifler != null)
            {
                if (model.KaskoDigerTeklif.DigerTeklifler.Count > 0)
                {
                    TeklifDigerSirketler teklif = new TeklifDigerSirketler();
                    foreach (var item in model.KaskoDigerTeklif.DigerTeklifler)
                    {
                        teklif = new TeklifDigerSirketler();
                        teklif.TeklifId = model.TeklifId;
                        teklif.SigortaSirketKodu = Convert.ToInt32(item.SirketKod);
                        teklif.SigortaSirketTeklifNo = item.SirketTeklifNo;
                        teklif.HasarsizlikIndirim = item.HasarsizlikIndirimSurprim;
                        teklif.HasarsizlikSurprim = item.HasarsizlikIndirimSurprim;
                        teklif.BrutPrim = item.TeklifTutar;
                        teklif.TaksitSayisi = item.TaksitSayisi;
                        teklif.KayitEdenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                        teklif.KayitTarihi = TurkeyDateTime.Now;
                        teklif.KomisyonTutari = item.KomisyonTutari;
                        //if (file != null && file.ContentLength > 0)
                        //{
                        //    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        //    string url = storage.UploadFile("kasko", item.file.FileName, item.file.InputStream);
                        //    teklif.TeklifPDF = url;
                        //}


                        //if (item.file != null && item.file.ContentLength > 0)
                        //{
                        //    //string path = String.Empty;
                        //    //path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
                        //    //file.SaveAs(path);  //string fileName = String.Format("kasko_{0}.pdf", System.Guid.NewGuid().ToString());

                        //    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();                          
                        //    string url = storage.UploadFile("kasko", item.file.FileName, item.file.InputStream);
                        //    teklif.TeklifPDF = url;
                        //}
                        _TeklifService.CreateDigerSirketTeklif(teklif);
                    }
                }
            }
            return View(model);
        }

        //Kasko Teklif Ekle Sonuç Ekranında Ekleme Çalışıyor
        [HttpPost]
        public ActionResult DigerTeklifKaydetEkle(KaskoModel model)
        {
            if (model.KaskoDigerTeklif.DigerTeklifler != null)
            {
                if (model.KaskoDigerTeklif.DigerTeklifler.Count > 0)
                {
                    TeklifDigerSirketler teklif = new TeklifDigerSirketler();
                    foreach (var item in model.KaskoDigerTeklif.DigerTeklifler)
                    {
                        teklif = new TeklifDigerSirketler();
                        teklif.Id = model.KaskoDigerTeklif.TeklifId.Value;
                        teklif.SigortaSirketKodu = Convert.ToInt32(item.SirketKod);
                        teklif.SigortaSirketTeklifNo = item.SirketTeklifNo;
                        teklif.HasarsizlikIndirim = item.HasarsizlikIndirimSurprim;
                        teklif.HasarsizlikSurprim = item.HasarsizlikIndirimSurprim;
                        teklif.BrutPrim = item.TeklifTutar;
                        teklif.KayitEdenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                        teklif.KomisyonTutari = item.KomisyonTutari;
                        teklif.KayitTarihi = TurkeyDateTime.Now;
                        _TeklifService.CreateDigerSirketTeklif(teklif);
                    }
                }
            }
            return View(model);
        }

        [AjaxException]
        public ActionResult PlakaSorgula(PlakaSorgulaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<KonfigurasyonService>();
                    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                    string testMi = konfig[Konfig.TestMi];

                    //NeoOnline db den araç bilgi sorgulama
                    ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
                    IAracContext _AracContext = DependencyResolver.Current.GetService<IAracContext>();
                    List<AracKullanimTarziServisModel> TarzList = new List<AracKullanimTarziServisModel>();
                    List<AracMarka> Markalar = new List<AracMarka>();
                    List<AracTip> Tipler = new List<AracTip>();
                    //TeklifArac arac = _TeklifService.getTeklifAracDetay(model.PlakaKodu, model.PlakaNo);
                    var aracBilgi = _TeklifService.getTeklifArac(model.PlakaKodu, model.PlakaNo);
                    PoliceArac polArac = new PoliceArac();
                    ICommonService commonService = DependencyResolver.Current.GetService<ICommonService>();
                    IPoliceService policeService = DependencyResolver.Current.GetService<IPoliceService>();


                    #region Plaka Sorgu Mapfre/HDI

                    //if (Convert.ToBoolean(testMi))
                    //{
                    //MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    //if (musteri != null)
                    //{
                    //    //IHDITrafik HDITrafik = DependencyResolver.Current.GetService<IHDITrafik>();
                    //    IHDIKasko HDIKasko = DependencyResolver.Current.GetService<IHDIKasko>();
                    //    model.PlakaNo = model.PlakaNo.ToUpperInvariant();
                    //    HDIPlakaSorgulamaResponse plakaSorguResponse = HDIKasko.PlakaSorgula(model.PlakaKodu, model.PlakaNo, musteri.MusteriTipKodu, musteri.KimlikNo);

                    //    if (!String.IsNullOrEmpty(plakaSorguResponse.Durum) && plakaSorguResponse.Durum != "0")
                    //    {
                    //        throw new Exception(String.Format("{0} - {1}", plakaSorguResponse.Durum, plakaSorguResponse.DurumMesaj));
                    //    }

                    //    HDIPlakaSorgulamaResponseDetails details = plakaSorguResponse.HDISIGORTA;
                    //    if (details != null && !String.IsNullOrEmpty(details.Durum) && details.Durum != "0" && !String.IsNullOrEmpty(details.Mesaj))
                    //    {
                    //        throw new Exception(String.Format("{0} - {1}", details.Durum, details.Mesaj));
                    //    }

                    //    Mapper.CreateMap<HDIPlakaSorgulamaResponseDetails, PlakaSorgu>();
                    //    PlakaSorgu plakaSorgu = Mapper.Map<PlakaSorgu>(details);

                    //    plakaSorgu.AracKullanimSekli = "2";

                    //    string koltukSayisi = String.Empty;
                    //    if (!String.IsNullOrEmpty(plakaSorgu.AracKoltukSayisi))
                    //    {
                    //        foreach (char d in plakaSorgu.AracKoltukSayisi)
                    //        {
                    //            if (char.IsDigit(d))
                    //            {
                    //                koltukSayisi += d;
                    //            }
                    //        }
                    //    }
                    //    plakaSorgu.AracKoltukSayisi = koltukSayisi;

                    //    short kullanimSekli = Convert.ToInt16(plakaSorgu.AracKullanimSekli);
                    //    List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekli);
                    //    plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
                    //    plakaSorgu.AracKullanimTarzi = details.AracTarifeGrupKodu + "-10";

                    //    plakaSorgu.AracMarkaKodu = plakaSorgu.AracMarkaKodu.PadLeft(3, '0');
                    //    List<AracMarka> markalar = _AracService.GetAracMarkaList(details.AracTarifeGrupKodu);
                    //    plakaSorgu.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

                    //    int modelYili = Convert.ToInt32(details.AracModelYili);

                    //    AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);

                    //    if (aracModel != null)
                    //    {
                    //        plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
                    //    }

                    //    List<AracTip> tipler = _AracService.GetAracTipList(details.AracTarifeGrupKodu, plakaSorgu.AracMarkaKodu, modelYili);
                    //    plakaSorgu.Tipler = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

                    //    plakaSorgu.AracKoltukSayisi = _AracService.GetAracKisiSayisi(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu).ToString();

                    //    if (!String.IsNullOrEmpty(details.AracTescilTarih))
                    //        plakaSorgu.AracTescilTarih = HDIMessage.ToDateTime(details.AracTescilTarih).ToString("dd.MM.yyyy");

                    //    if (!String.IsNullOrEmpty(details.PoliceBitisTarih))
                    //    {
                    //        DateTime hdiPoliceBitis = HDIMessage.ToDateTimeForKasko(details.PoliceBitisTarih.PadLeft(8, '0'));

                    //        if (hdiPoliceBitis < TurkeyDateTime.Today)
                    //            plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                    //        else
                    //            plakaSorgu.YeniPoliceBaslangicTarih = hdiPoliceBitis.ToString("dd.MM.yyyy");
                    //    }

                    //    plakaSorgu.ProjeKodu = _AktifKullaniciService.ProjeKodu;

                    //    //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz

                    //    var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                    //    plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                    //    plakaSorgu.AnadoluHata = ktipleri.hata;

                    //    var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);

                    //    plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();

                    //    return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else
                    //{
                    //MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    //MusteriGenelBilgiler musteri = _MusteriService.GetMusteriTeklifFor("36862493292");
                    //if (musteri != null)
                    //{
                    //    model.PlakaNo = model.PlakaNo.ToUpperInvariant();

                    //    PlakaSorgu plakaSorgu = MAPFREPlakaSorgula(model, musteri);

                    //    var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);
                    //    plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();

                    //    if (!String.IsNullOrEmpty(plakaSorgu.EskiPoliceNo))
                    //    {
                    //        var hasarsizlik = HasarsizlikSorgula(musteri.KimlikNo, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceYenilemeNo, "420");
                    //        if (hasarsizlik != null)
                    //        {
                    //            plakaSorgu.HasarsizlikInd = hasarsizlik.HasarsizlikInd;
                    //            plakaSorgu.HasarsizlikSur = hasarsizlik.HasarsizlikSur;
                    //            plakaSorgu.HasarsizlikKademe = hasarsizlik.HasarsizlikKademe;
                    //            plakaSorgu.HasarsizlikHata = hasarsizlik.hata;
                    //        }
                    //    }

                    //    return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    //}
                    //}

                    #endregion

                    if (aracBilgi.teklifArac == null)
                    {

                        polArac = policeService.getPoliceAracDetay(model.PlakaKodu, model.PlakaNo);
                    }
                    if (aracBilgi.teklifArac != null)
                    {
                        TeklifArac arac = aracBilgi.teklifArac;
                        PlakaSorgu plakaSorgu = new PlakaSorgu();
                        plakaSorgu.AracKullanimSekli = arac.KullanimSekli;
                        plakaSorgu.AracModelYili = arac.Model.HasValue ? arac.Model.Value.ToString() : "";
                        plakaSorgu.MotorGucu = arac.MotorGucu;
                        plakaSorgu.AracMotorNo = arac.MotorNo;
                        plakaSorgu.AracSasiNo = arac.SasiNo;
                        plakaSorgu.AracSilindir = arac.SilindirHacmi;
                        plakaSorgu.AracMarkaKodu = arac.Marka;
                        plakaSorgu.AracTipKodu = arac.AracinTipi;
                        plakaSorgu.TramerBelgeNumarasi = "";
                        plakaSorgu.AracKoltukSayisi = arac.KoltukSayisi.HasValue ? arac.KoltukSayisi.Value.ToString() : "";
                        plakaSorgu.PlakaKodu = model.PlakaKodu;
                        plakaSorgu.PlakaNo = model.PlakaNo;
                        if (arac.TrafikTescilTarihi.HasValue)
                        {
                            plakaSorgu.AracTescilTarih = arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy");
                        }
                        else
                        {
                            plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                        }
                        if (arac.TrafikCikisTarihi.HasValue)
                        {
                            if (arac.TrafikCikisTarihi.Value.Year.ToString() == plakaSorgu.AracModelYili || (arac.TrafikCikisTarihi.Value.Year - this.ToDateTime(plakaSorgu.AracModelYili).Year) == 1)
                            {
                                plakaSorgu.TrafigeCikisTarihi = arac.TrafikCikisTarihi.Value.ToString("dd.MM.yyyy");
                            }
                            else
                            {
                                var parts = plakaSorgu.AracTescilTarih.Split('.');
                                plakaSorgu.TrafigeCikisTarihi = plakaSorgu.AracTescilTarih.Replace(parts[2], plakaSorgu.AracModelYili);
                            }

                        }
                        plakaSorgu.AracKullanimSekli = arac.KullanimSekli;
                        plakaSorgu.AracKullanimTarzi = arac.KullanimTarzi;
                        if (!String.IsNullOrEmpty(arac.KullanimSekli))
                        {
                            TarzList = _AracService.GetAracKullanimTarziTeklif(Convert.ToInt16(arac.KullanimSekli));
                        }
                        if (!String.IsNullOrEmpty(arac.KullanimTarzi))
                        {
                            var parts = arac.KullanimTarzi.Split('-');
                            Markalar = _AracService.GetAracMarkaList(parts[0]);
                        }
                        if (!String.IsNullOrEmpty(arac.KullanimTarzi) && !String.IsNullOrEmpty(plakaSorgu.AracMarkaKodu) && arac.Model.HasValue)
                        {
                            var parts = arac.KullanimTarzi.Split('-');
                            Tipler = _AracService.GetAracTipList(parts[0], plakaSorgu.AracMarkaKodu, arac.Model.Value);
                        }
                        plakaSorgu.ProjeKodu = _AktifKullaniciService.ProjeKodu;
                        if (TarzList != null)
                        {
                            plakaSorgu.Tarzlar = new SelectList(TarzList, "Kod", "KullanimTarzi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Tarzlar = new List<SelectListItem>();
                        }
                        if (TarzList != null)
                        {
                            plakaSorgu.Markalar = new SelectList(Markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Markalar = new List<SelectListItem>();
                        }
                        if (Tipler != null)
                        {
                            plakaSorgu.Tipler = new SelectList(Tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Tipler = new List<SelectListItem>();
                        }
                        AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, Convert.ToInt32(plakaSorgu.AracModelYili));

                        if (aracModel != null)
                        {
                            plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
                        }

                        plakaSorgu.AracKoltukSayisi = _AracService.GetAracKisiSayisi(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu).ToString();
                        //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz

                        var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                        plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                        plakaSorgu.AnadoluHata = ktipleri.hata;

                        var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);

                        plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();
                        var TeklifGenel = _TeklifService.GetTeklif(arac.TeklifId);
                        if (TeklifGenel != null)
                        {

                            var vadeTarihFarki = commonService.GunFarkikBul(TurkeyDateTime.Now, TeklifGenel.GenelBilgiler.BitisTarihi.Date);
                            if (vadeTarihFarki <= 45)
                            { //Vadesi gelmiş ise bizim sistemden poliçe yapıldı ise bu bilgileri getirebiliriz
                                plakaSorgu.EskiPoliceAcenteKod = "1111";
                                plakaSorgu.EskiPoliceSigortaSirkedKodu = "";
                                plakaSorgu.EskiPoliceNo = "";
                                plakaSorgu.EskiPoliceYenilemeNo = "0";
                            }
                            else
                            {
                                if (aracBilgi.teklifSoru != null)
                                {
                                    for (int i = 0; i < aracBilgi.teklifSoru.Count; i++)
                                    {
                                        switch (aracBilgi.teklifSoru[i].SoruKodu)
                                        {
                                            case KaskoSorular.Eski_Police_Acente_No: plakaSorgu.EskiPoliceAcenteKod = aracBilgi.teklifSoru[i].Cevap; break;
                                            case KaskoSorular.Eski_Police_Sigorta_Sirketi: plakaSorgu.EskiPoliceSigortaSirkedKodu = aracBilgi.teklifSoru[i].Cevap; break;
                                            case KaskoSorular.Eski_Police_No: plakaSorgu.EskiPoliceNo = aracBilgi.teklifSoru[i].Cevap; break;
                                            case KaskoSorular.Eski_Police_Yenileme_No: plakaSorgu.EskiPoliceYenilemeNo = aracBilgi.teklifSoru[i].Cevap; break;
                                        }
                                    }
                                }
                            }
                        }
                        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);

                    }
                    else if (polArac != null)
                    {
                        PlakaSorgu plakaSorgu = new PlakaSorgu();
                        plakaSorgu.AracKullanimSekli = polArac.KullanimSekli;
                        plakaSorgu.AracModelYili = polArac.Model.HasValue ? polArac.Model.Value.ToString() : "";
                        plakaSorgu.MotorGucu = polArac.MotorGucu;
                        plakaSorgu.AracMotorNo = polArac.MotorNo;
                        plakaSorgu.AracSasiNo = polArac.SasiNo;
                        plakaSorgu.AracSilindir = polArac.SilindirHacmi;
                        if (polArac.Marka != null)
                        {

                            if (polArac.Marka.Length == 8)
                            {
                                plakaSorgu.AracMarkaKodu = polArac.Marka.Substring(1, 3);
                                if (polArac.AracinTipiKodu == null)
                                {
                                    plakaSorgu.AracTipKodu = polArac.Marka.Substring(4, polArac.Marka.Length - 4);
                                }
                            }
                            else if (polArac.Marka.Length < 8)
                            {
                                plakaSorgu.AracMarkaKodu = polArac.Marka.Substring(0, 3);
                                if (polArac.AracinTipiKodu == null)
                                {
                                    plakaSorgu.AracTipKodu = polArac.Marka.Substring(3, polArac.Marka.Length - 3);
                                }
                            }
                            else
                            {
                                plakaSorgu.AracMarkaKodu = polArac.Marka;
                                plakaSorgu.AracTipKodu = polArac.AracinTipiKodu;
                            }

                        }
                        var policeGenel = policeService.GetPolice(polArac.PoliceId);

                        var vadeTarihFarki = commonService.GunFarkikBul(TurkeyDateTime.Now, policeGenel.BitisTarihi.Value);
                        if (vadeTarihFarki <= 45)
                        { //Vadesi gelmiş ise poliçenin üzerindeki poliçe bilgileri atanıyor
                            plakaSorgu.EskiPoliceAcenteKod = "";
                            plakaSorgu.EskiPoliceSigortaSirkedKodu = policeGenel.TUMBirlikKodu;
                            plakaSorgu.EskiPoliceNo = policeGenel.PoliceNumarasi;
                            plakaSorgu.EskiPoliceYenilemeNo = policeGenel.YenilemeNo.HasValue ? policeGenel.YenilemeNo.Value.ToString() : "0";
                        }

                        plakaSorgu.TramerBelgeNumarasi = "";
                        plakaSorgu.AracKoltukSayisi = polArac.KoltukSayisi.HasValue ? polArac.KoltukSayisi.Value.ToString() : "";
                        plakaSorgu.PlakaKodu = model.PlakaKodu;
                        plakaSorgu.PlakaNo = model.PlakaNo;
                        if (polArac.TrafikTescilTarihi.HasValue)
                        {
                            plakaSorgu.AracTescilTarih = polArac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy");
                        }
                        else
                        {
                            plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                        }

                        plakaSorgu.AracKullanimSekli = polArac.KullanimSekli;

                        //plakaSorgu.AracKullanimTarzi = polArac.KullanimTarzi;
                        //plakaSorgu.AracKullanimTarzi = "111-10";

                        if (String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
                        {
                            var aracTipDetay = _AracService.GetAracTip(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu);
                            if (aracTipDetay != null)
                            {
                                plakaSorgu.AracKullanimTarzi = aracTipDetay.KullanimSekli1;
                                plakaSorgu.AracKoltukSayisi = aracTipDetay.KisiSayisi.HasValue ? aracTipDetay.KisiSayisi.Value.ToString() : "";
                            }
                        }

                        if (!String.IsNullOrEmpty(polArac.KullanimSekli))
                        {
                            if (Char.IsDigit(polArac.KullanimSekli[0]))
                            {
                                TarzList = _AracService.GetAracKullanimTarziTeklif(Convert.ToInt16(polArac.KullanimSekli));
                            }
                        }
                        if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
                        {
                            var parts = plakaSorgu.AracKullanimTarzi.Split('-');
                            if (parts.Length > 0)
                            {
                                Markalar = _AracService.GetAracMarkaList(parts[0]);
                            }
                        }
                        if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi) && !String.IsNullOrEmpty(polArac.Marka) && polArac.Model.HasValue)
                        {
                            Tipler = _AracService.GetAracTipList(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracMarkaKodu, polArac.Model.Value);
                        }
                        plakaSorgu.ProjeKodu = _AktifKullaniciService.ProjeKodu;
                        if (TarzList != null)
                        {
                            plakaSorgu.Tarzlar = new SelectList(TarzList, "Kod", "KullanimTarzi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Tarzlar = new List<SelectListItem>();
                        }
                        if (Markalar != null)
                        {
                            plakaSorgu.Markalar = new SelectList(Markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Markalar = new List<SelectListItem>();
                        }
                        if (Tipler != null)
                        {
                            plakaSorgu.Tipler = new SelectList(Tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                        }
                        else
                        {
                            plakaSorgu.Tipler = new List<SelectListItem>();
                        }
                        if (!string.IsNullOrEmpty(plakaSorgu.AracModelYili))
                        {
                            AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, Convert.ToInt32(plakaSorgu.AracModelYili));

                            if (aracModel != null)
                            {
                                plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
                            }
                        }
                        if (String.IsNullOrEmpty(plakaSorgu.AracKoltukSayisi))
                        {
                            plakaSorgu.AracKoltukSayisi = _AracService.GetAracKisiSayisi(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu).ToString();
                        }

                        //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz
                        if (!String.IsNullOrEmpty(plakaSorgu.AracKullanimTarzi))
                        {
                            var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                            plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                            plakaSorgu.AnadoluHata = ktipleri.hata;
                            var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);
                            plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();

                        }
                        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);

                    }
                    else if (_AktifKullaniciService.TVMKodu == 140)
                    {
                        #region Plaka Sorgu Gulf Sigorta
                        MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                        if (musteri != null)
                        {
                            IGULFKasko GulfKasko = DependencyResolver.Current.GetService<IGULFKasko>();
                            GULFAracBilgileri plakaSorguGulf = GulfKasko.GetGULFAracBilgiSorgu(musteri.KimlikNo, model.PlakaKodu.PadLeft(3, '0'), model.PlakaNo);

                            if (String.IsNullOrEmpty(plakaSorguGulf.HataMesaji))
                            {
                                PlakaSorgu plakaSorgu = new PlakaSorgu();
                                plakaSorgu.AracKullanimSekli = plakaSorguGulf.KullanimSekli;
                                plakaSorgu.AracModelYili = plakaSorguGulf.ModelYili;
                                plakaSorgu.MotorGucu = plakaSorguGulf.MotorGucu;
                                plakaSorgu.AracMotorNo = plakaSorguGulf.MotorNo;
                                plakaSorgu.AracSasiNo = plakaSorguGulf.SasiNo;
                                plakaSorgu.AracSilindir = plakaSorguGulf.SilindirHacmi;
                                plakaSorgu.AracMarkaKodu = plakaSorguGulf.MarkaKodu;
                                plakaSorgu.AracTipKodu = plakaSorguGulf.AracTipKodu;
                                plakaSorgu.TramerBelgeNumarasi = plakaSorguGulf.SBMTramerNo;
                                plakaSorgu.AracKoltukSayisi = plakaSorguGulf.KoltukSayisi;
                                plakaSorgu.PlakaKodu = model.PlakaKodu;
                                plakaSorgu.PlakaNo = model.PlakaNo;
                                if (!String.IsNullOrEmpty(plakaSorguGulf.TescilTarihi))
                                {
                                    plakaSorgu.AracTescilTarih = this.ToDateTime(plakaSorguGulf.TescilTarihi).ToString("dd.MM.yyyy");

                                    if (this.ToDateTime(plakaSorguGulf.TescilTarihi).Year == this.ToDateTime(plakaSorgu.AracModelYili).Year || (this.ToDateTime(plakaSorguGulf.TescilTarihi).Year - this.ToDateTime(plakaSorgu.AracModelYili).Year) == 1)
                                    {
                                        plakaSorgu.TrafigeCikisTarihi = plakaSorgu.AracTescilTarih;
                                    }
                                    else
                                    {
                                        var parts = plakaSorgu.AracTescilTarih.Split('.');
                                        plakaSorgu.TrafigeCikisTarihi = plakaSorgu.AracTescilTarih.Replace(parts[2], plakaSorgu.AracModelYili);
                                    }
                                }
                                else
                                {
                                    plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                                }



                                if (plakaSorguGulf.policeBilgi != null)
                                {
                                    plakaSorgu.EskiPoliceNo = plakaSorguGulf.policeBilgi.PoliceNo;
                                    plakaSorgu.EskiPoliceYenilemeNo = plakaSorguGulf.policeBilgi.YenilemeNo;
                                    plakaSorgu.EskiPoliceAcenteKod = plakaSorguGulf.policeBilgi.AcenteKod;
                                    plakaSorgu.EskiPoliceSigortaSirkedKodu = plakaSorguGulf.policeBilgi.SirketKodu;
                                }

                                int modelYili = Convert.ToInt32(plakaSorgu.AracModelYili);



                                if (!String.IsNullOrEmpty(plakaSorguGulf.AracTarifeGrupKodu))
                                {

                                    CR_AracGrup aracGrup = _CRContext.CR_AracGrupRepository.Filter(s => s.TarifeKodu == plakaSorguGulf.AracTarifeGrupKodu &&
                                                                                                        s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();

                                    if (aracGrup != null)
                                    {
                                        AracKullanimTarzi tarzi = _AracContext.AracKullanimTarziRepository.Filter(s => s.Kod2 == aracGrup.Kod2 &&
                                                                                                                       s.KullanimTarziKodu == aracGrup.KullanimTarziKodu)
                                                                                                           .FirstOrDefault();
                                        if (tarzi != null && tarzi.KullanimSekliKodu.HasValue)
                                        {
                                            plakaSorgu.AracKullanimSekli = tarzi.KullanimSekliKodu.ToString();
                                            plakaSorgu.AracKullanimTarzi = String.Format("{0}-{1}", tarzi.KullanimTarziKodu, tarzi.Kod2);

                                            // LİST
                                            TarzList = _AracService.GetAracKullanimTarziTeklif(tarzi.KullanimSekliKodu.Value);
                                            Markalar = _AracService.GetAracMarkaList(tarzi.KullanimTarziKodu);
                                            Tipler = _AracService.GetAracTipList(tarzi.KullanimTarziKodu, plakaSorgu.AracMarkaKodu, modelYili);
                                        }

                                    }
                                }
                                if (TarzList != null)
                                {
                                    plakaSorgu.Tarzlar = new SelectList(TarzList, "Kod", "KullanimTarzi").ListWithOptionLabel();
                                }
                                else
                                {
                                    plakaSorgu.Tarzlar = new List<SelectListItem>();
                                }
                                if (TarzList != null)
                                {
                                    plakaSorgu.Markalar = new SelectList(Markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                                }
                                else
                                {
                                    plakaSorgu.Markalar = new List<SelectListItem>();
                                }
                                if (Tipler != null)
                                {
                                    plakaSorgu.Tipler = new SelectList(Tipler, "TipKodu", "TipAdi").ListWithOptionLabel();
                                }
                                else
                                {
                                    plakaSorgu.Tipler = new List<SelectListItem>();
                                }
                                AracModel aracModel = _AracService.GetAracModel(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu, modelYili);

                                if (aracModel != null)
                                {
                                    plakaSorgu.AracDegeri = aracModel.Fiyat.HasValue ? aracModel.Fiyat.Value.ToString("N0") : String.Empty;
                                }

                                plakaSorgu.AracKoltukSayisi = _AracService.GetAracKisiSayisi(plakaSorgu.AracMarkaKodu, plakaSorgu.AracTipKodu).ToString();

                                //if (!String.IsNullOrEmpty(plakaSorgu.AracTescilTarih))
                                //    plakaSorgu.AracTescilTarih = HDIMessage.ToDateTime(plakaSorgu.AracTescilTarih).ToString("dd.MM.yyyy");
                                //plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");

                                //if (!String.IsNullOrEmpty(details.PoliceBitisTarih))
                                //{
                                //    DateTime hdiPoliceBitis = HDIMessage.ToDateTimeForKasko(details.PoliceBitisTarih.PadLeft(8, '0'));

                                //    if (hdiPoliceBitis < TurkeyDateTime.Today)
                                //        plakaSorgu.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                                //    else
                                //        plakaSorgu.YeniPoliceBaslangicTarih = hdiPoliceBitis.ToString("dd.MM.yyyy");
                                //}

                                plakaSorgu.ProjeKodu = _AktifKullaniciService.ProjeKodu;

                                //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz

                                var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                                plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                                plakaSorgu.AnadoluHata = ktipleri.hata;

                                var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);

                                plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();

                                return Json(plakaSorgu, JsonRequestBehavior.AllowGet);

                            }
                            else
                            {
                                throw new Exception(plakaSorguGulf.HataMesaji);
                            }
                        }
                        #endregion
                    }
                    //else
                    //{
                    //    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    //    if (musteri != null)
                    //    {
                    //        model.PlakaNo = model.PlakaNo.ToUpperInvariant();

                    //        PlakaSorgu plakaSorgu = MAPFREPlakaSorgula(model, musteri);

                    //        var yurtIciKademeler = GetTasinanYukKademeleri(plakaSorgu.AracKullanimTarzi);
                    //        plakaSorgu.YurticiKademeler = new SelectList(yurtIciKademeler.list, "key", "value").ListWithOptionLabel();

                    //        if (!String.IsNullOrEmpty(plakaSorgu.EskiPoliceNo))
                    //        {
                    //            var hasarsizlik = HasarsizlikSorgula(musteri.KimlikNo, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceYenilemeNo, "420");
                    //            if (hasarsizlik != null)
                    //            {
                    //                plakaSorgu.HasarsizlikInd = hasarsizlik.HasarsizlikInd;
                    //                plakaSorgu.HasarsizlikSur = hasarsizlik.HasarsizlikSur;
                    //                plakaSorgu.HasarsizlikKademe = hasarsizlik.HasarsizlikKademe;
                    //                plakaSorgu.HasarsizlikHata = hasarsizlik.hata;
                    //            }
                    //        }

    
                    //            }
                    //        }

                    //        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    //    }
                    //}

                }

                throw new Exception("Plaka sorgulama yapılamadı. Lütfen manuel giriş yapınız.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
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

        [AjaxException]
        public ActionResult PlakaSorgulaMapfre(PlakaSorgulaModel model)
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
                        if (!String.IsNullOrEmpty(plakaSorgu.EskiPoliceNo))
                        {
                            var hasarsizlik = HasarsizlikSorgula(musteri.KimlikNo, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceYenilemeNo, "420");
                            if (hasarsizlik != null)
                            {
                                plakaSorgu.HasarsizlikInd = hasarsizlik.HasarsizlikInd;
                                plakaSorgu.HasarsizlikSur = hasarsizlik.HasarsizlikSur;
                                plakaSorgu.HasarsizlikKademe = hasarsizlik.HasarsizlikKademe;
                                plakaSorgu.HasarsizlikHata = hasarsizlik.hata;
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
                log.Error("MapfreKaskoController.PlakaSorgula", ex);
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
                    ITeklif urun = TeklifUrunFactory.AsUrunClass(teklif);
                    urun.DekontPDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDekont))
                {
                    throw new Exception("Dekont pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("MapfreKaskoController.DekontPDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFDekont;
            return Json(new { Success = success, PDFUrl = url });
        }

        private TeklifFiyatModel KaskoFiyat(KaskoTeklif kaskoTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = kaskoTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = kaskoTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            TeklifArac arac = kaskoTeklif.Teklif.Arac;
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

            model.PoliceBaslangicTarihi = kaskoTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = kaskoTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = kaskoTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

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

        public ActionResult AracTipiGetir(string MarkaKodu)
        {
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();

            List<AracTip> tipler = _AracService.GetAracTipList(MarkaKodu);

            return Json(new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAracDegerKisiSayisi(string TipKodu, string MarkaKodu, int Model, string KullanimTarzi)
        {
            decimal aracDeger = _AracService.GetAracDeger(MarkaKodu, TipKodu, Model);
            short kisiSayisi = _AracService.GetAracKisiSayisi(MarkaKodu, TipKodu);
            AracDegerKisiSayisiModel model = new AracDegerKisiSayisiModel();
            model.AracDeger = aracDeger;
            model.KisiSayisi = kisiSayisi;

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
            decimal carpan = (decimal)0.1;

            // ==== Aracın fiyatının 10% bulunuyor ==== //
            decimal yuzdelikFark = orjinalDeger * carpan;
            decimal fark = 0;

            if (orjinalDeger > AracDeger) fark = orjinalDeger - AracDeger;
            else fark = AracDeger - orjinalDeger;

            if (fark < yuzdelikFark)
                model.Result = true;
            else
            {
                model.Result = false;
                model.OrjinalDeger = orjinalDeger;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public bool AracDegerKontrolServer(KaskoAracBilgiModel model)
        {
            if (!String.IsNullOrEmpty(model.AracDeger))
            {
                decimal aracDeger = Convert.ToDecimal(model.AracDeger.Replace(".", "").Replace(",", ""));
                decimal orjinalDeger = _AracService.GetAracDeger(model.MarkaKodu, model.TipKodu, model.Model);
                decimal carpan = (decimal)0.1;

                // ==== Aracın fiyatının 10% bulunuyor ==== //
                decimal yuzdelikFark = orjinalDeger * carpan;
                decimal fark = 0;

                if (orjinalDeger > aracDeger) fark = orjinalDeger - aracDeger;
                else fark = aracDeger - orjinalDeger;

                if (fark < yuzdelikFark) return true;
                else return false;
            }
            else return false;
        }

        private void TeklifTeminatDoldur(DetayKaskoModel model, ITeklif kaskoTeklif)
        {
            model.Teminat = new DetayKaskoTeminatModel();
            model.Teminat.ProjeKodu = _AktifKullaniciService.ProjeKodu;
            if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                string amsKodu = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_AMS_Kodu, "0");

                if (!String.IsNullOrEmpty(amsKodu) && amsKodu != "0")
                {
                    CR_KaskoAMS ams = _CRService.GetKaskoAMS(TeklifUretimMerkezleri.MAPFRE, kaskoTeklif.GenelBilgiler.TVMKodu, amsKodu);
                    model.Teminat.AMS = ams != null ? ams.Aciklama : String.Empty;
                }

                decimal olumSakatlik = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_Olum_Sakatlik, decimal.Zero);
                if (olumSakatlik > 0)
                {
                    model.Teminat.OlumSakatlik = olumSakatlik.ToString("N2");
                }
            }
            else
            {
                string immKodu = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, "0");

                string[] parts = kaskoTeklif.Arac.KullanimTarzi.Split('-');

                KaskoIMM imm = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKodu), parts[0], parts[1]);
                model.Teminat.IMM = imm != null ? imm.Text : String.Empty;

                string fkKodu = kaskoTeklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, "0");


                KaskoFK fk = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKodu), parts[0], parts[1]);
                model.Teminat.FK = fk != null ? fk.Text : String.Empty;

            }

            string kaskoTuru = kaskoTeklif.ReadSoru(KaskoSorular.Kasko_Turu, "0");
            if (!String.IsNullOrEmpty(kaskoTuru) && kaskoTuru != "0")
            {
                model.Teminat.KaskoTurleri = TeklifListeleri.KaskoTurleri().FirstOrDefault(s => s.Value == kaskoTuru).Text;
            }

            string kaskoServisi = kaskoTeklif.ReadSoru(KaskoSorular.Servis_Turu, "0");
            if (!String.IsNullOrEmpty(kaskoServisi) && kaskoServisi != "0")
            {
                model.Teminat.KaskoServisleri = TeklifListeleri.KaskoServisTurleri().FirstOrDefault(s => s.Value == kaskoServisi).Text;
            }

            string kaskoYedekParca = kaskoTeklif.ReadSoru(KaskoSorular.Yedek_Parca_Turu, "0");
            if (!String.IsNullOrEmpty(kaskoYedekParca) && kaskoYedekParca != "0")
            {
                model.Teminat.KaskoYedekParcalari = TeklifListeleri.KaskoYedekParcaTuru().FirstOrDefault(s => s.Value == kaskoYedekParca).Text;
            }

            // ==== Yurtdışı teminat suresi ==== //
            model.Teminat.YurtDisiTeminati = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);
            if (model.Teminat.YurtDisiTeminati)
            {
                string YurtdisiTeminatSuresi = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "0");
                if (!String.IsNullOrEmpty(YurtdisiTeminatSuresi) && YurtdisiTeminatSuresi != "0")
                {
                    model.Teminat.YurtDisiTeminatiSuresi = TeklifListeleri.KaskoYurtDisiTeminatSureleri().FirstOrDefault(s => s.Value == YurtdisiTeminatSuresi).Text;
                }
            }

            // ==== Arac lpg limi (Eğer lpg li ise orjinal mi? (orjinal değilse markası bedeli nedir)) ==== //
            model.Teminat.AracLPGlimi = kaskoTeklif.ReadSoru(KaskoSorular.LPG_VarYok, false);
            //if (model.Teminat.AracLPGlimi)
            //{
            //    model.Teminat.LPGOrjinalmi = kaskoTeklif.ReadSoru(KaskoSorular.LPG_Arac_Orjinalmi, true);
            //    if (!model.Teminat.LPGOrjinalmi)
            //    {
            //        model.Teminat.LPGMarkasi = kaskoTeklif.ReadSoru(KaskoSorular.LPG_Markasi, string.Empty);
            //        model.Teminat.LPGBedeli = kaskoTeklif.ReadSoru(KaskoSorular.LPG_Bedeli, string.Empty);
            //    }
            //}

            // ==== Sağlık varsa kişi sayısı ==== //
            model.Teminat.Saglik = kaskoTeklif.ReadSoru(KaskoSorular.Saglik_VarYok, false);
            if (model.Teminat.Saglik)
            {
                string saglik = kaskoTeklif.ReadSoru(KaskoSorular.Saglik_Kisi_Sayisi, "0");
                if (!String.IsNullOrEmpty(saglik) && saglik != "0")
                {
                    model.Teminat.SaglikKisiSayisi = saglik;
                }
            }

            model.Teminat.G_L_K_H_H_T = kaskoTeklif.ReadSoru(KaskoSorular.GLKHHT, false);
            model.Teminat.Deprem = kaskoTeklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
            model.Teminat.Sel_Su = kaskoTeklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false);
            model.Teminat.HasarsizlikKoruma = kaskoTeklif.ReadSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, false);
            model.Teminat.Calinma = kaskoTeklif.ReadSoru(KaskoSorular.Calinma_VarYok, false);
            model.Teminat.HukuksalKoruma = kaskoTeklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, false);
            model.Teminat.HayvanlarinVerecegiZarar = kaskoTeklif.ReadSoru(KaskoSorular.Hayvanlarin_Verecegi_Zarar_ZarYok, false);

            model.Teminat.Alarm = kaskoTeklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
            model.Teminat.AnahtarKaybi = kaskoTeklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
            model.Teminat.Yangin = kaskoTeklif.ReadSoru(KaskoSorular.Yangin_VarYok, false);
            model.Teminat.Eskime = kaskoTeklif.ReadSoru(KaskoSorular.Eskime_VarYok, false);
            model.Teminat.YurtDisiTeminati = kaskoTeklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);

            model.Teminat.Ikame = "İKAME YOK";
            if (kaskoTeklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false))
            {
                string ikame = kaskoTeklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                switch (ikame)
                {
                    case "ABC07": model.Teminat.Ikame = "7 GÜN"; break;
                    case "ABC14": model.Teminat.Ikame = "14 GÜN"; break;
                }
            }
        }

        private PlakaSorgu MAPFREPlakaSorgula(PlakaSorgulaModel model, MusteriGenelBilgiler musteri)
        {
            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            PoliceSorguKaskoResponse response = mapfreSorgu.PoliceSorguKasko(_AktifKullaniciService.TVMKodu, musteri.KimlikNo, model.PlakaKodu, model.PlakaNo);

            //Kasko sorguda tescil tarihi gelmediği için trafik sorgu çalıştırtılıyor
            PoliceSorguTrafikResponse responseTrafik = mapfreSorgu.PoliceSorguTrafik(_AktifKullaniciService.TVMKodu, musteri.KimlikNo, model.PlakaKodu, model.PlakaNo);
            MapfreToHdiPlakaSorgu trafikAracBilgi = _AracService.KarsilastirmaliPlakaSorgu_FillMapfre(responseTrafik);
            //------


            IANADOLUKasko _kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();
            IAracContext _AracContext = DependencyResolver.Current.GetService<IAracContext>();

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
                if (bilgi.yolcuKapasitesi == "0")
                {
                    TramerSorguPoliceValue digerBilgi = response.oncekiPoliceList.TramerSorguPolice.FirstOrDefault(w => w.yolcuKapasitesi != "0");
                    if (digerBilgi != null)
                    {
                        bilgi.yolcuKapasitesi = digerBilgi.yolcuKapasitesi;
                    }
                }
            }

            PlakaSorgu plakaSorgu = new PlakaSorgu();
            plakaSorgu.AracTipKodu = bilgi.aracTipKodu.TrimStart('0');
            plakaSorgu.AracModelYili = bilgi.modelYili;
            plakaSorgu.AracMotorNo = bilgi.motorNo;
            plakaSorgu.AracSasiNo = bilgi.sasiNo;
            plakaSorgu.AracKoltukSayisi = bilgi.yolcuKapasitesi;

            if (plakaSorgu.AracKoltukSayisi == "0" || plakaSorgu.AracKoltukSayisi == "")
            {
                string koltukSayisi = responseTrafik.trafikHasar.hasarBilgi.Select(s => s.yolcuKapasitesi).FirstOrDefault();
                if (!String.IsNullOrEmpty(koltukSayisi))
                {
                    plakaSorgu.AracKoltukSayisi = (Convert.ToInt32(koltukSayisi.Substring(0, 1)) + 1).ToString();
                }
            }

            plakaSorgu.TramerBelgeNumarasi = bilgi.tramerBelgeNo;
            plakaSorgu.TramerBelgeTarihi = bilgi.tramerBelgeTarih;
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

            MapfreToHdiPlakaSorgu mapfreToHdi = _AracService.KarsilastirmaliPlakaSorgu_FillMapfreKasko(bilgi);

            if (mapfreToHdi != null)
            {
                plakaSorgu.Tarzlar = new SelectList(mapfreToHdi.TarzList, "Kod", "KullanimTarzi").ListWithOptionLabel();
                plakaSorgu.Markalar = new SelectList(mapfreToHdi.Markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                plakaSorgu.Tipler = new SelectList(mapfreToHdi.Tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

                plakaSorgu.AracKullanimSekli = mapfreToHdi.AracKullanimSekli;
                plakaSorgu.AracKullanimTarzi = mapfreToHdi.AracKullanimTarzi;
                plakaSorgu.AracMarkaKodu = bilgi.aracMarkaKodu;

                //Anadoluya Özel Kullanım Tipleri Listesini Okuyoruz
                var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                plakaSorgu.AnadoluHata = ktipleri.hata;
                // plakaSorgu.AnadoluIkameTurleri = new SelectList(_kasko.getTeminatlar("", "", "", 100), "", "").ListWithOptionLabel(); 
            }

            int modelYili = int.Parse(bilgi.modelYili);

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
            else if (trafikAracBilgi.AracTescilTarih != null && trafikAracBilgi.AracTescilTarih != "")
            {
                DateTime TescilTarihi = MapfreSorguResponse.ToDateTime(trafikAracBilgi.AracTescilTarih);
                plakaSorgu.AracTescilTarih = trafikAracBilgi.AracTescilTarih;
            }
            else
            {
                plakaSorgu.AracTescilTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
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
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullaniciService.TVMKodu);
            EgmSorguResponse response = mapfreSorgu.EgmSorgu(tvmKodu, model.PlakaKodu, model.PlakaNo, model.AracRuhsatSeriNo, model.AracRuhsatNo, model.AsbisNo);

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

            //List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziList("0");
            ////List<AracKullanimTarziServisModel> tarzlar = _CRService.GetKullanimTarzi(TeklifUretimMerkezleri.MAPFRE);
            //plakaSorgu.Tarzlar = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();

            return plakaSorgu;
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

        public ActionResult IkameTurleriListe(string KullanimTarziKodu)
        {
            if (!String.IsNullOrEmpty(KullanimTarziKodu))
            {
                string[] parts = KullanimTarziKodu.Split('-');
                if (parts.Length == 2 && parts[0] == "111" && parts[1] == "10")
                {
                    return Json(new { success = true, def = "ABC14", list = IkameTurleri("") }, JsonRequestBehavior.AllowGet);
                }
            }

            List<SelectListItem> items = new List<SelectListItem>() { new SelectListItem() { Text = "İKAME YOK", Value = "0" } };
            return Json(new { success = true, def = "0", list = items }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> IkameTurleri(string selectedIkame)
        {
            List<KeyValueItem<string, string>> list1 = new List<KeyValueItem<string, string>>();
            list1.Add(new KeyValueItem<string, string>("0", babonline.NoSubstitution));
            list1.Add(new KeyValueItem<string, string>("ABC07", babonline.Days7_24));
            list1.Add(new KeyValueItem<string, string>("ABC14", babonline.Days14_24));

            List<SelectListItem> items = new SelectList(list1, "Key", "Value", selectedIkame).ToList<SelectListItem>();
            return items;
        }

        public ActionResult GetTrafikTescilBilgi(string PlakaKodu, string PlakaNo, string AracRuhsatSeriNo, string AracRuhsatNo, string AsbisNo)
        {
            var trafikTescilTarihi = String.Empty;
            var motorNo = String.Empty;
            var sasiNo = String.Empty;
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

                if (egmSorguSuccess && response.aracTescilBilgileri == null)
                    egmSorguSuccess = false;

                if (egmSorguSuccess)
                {
                    if (response.aracTescilBilgileri.tescilTarihi != null)
                    {
                        long time = 0;
                        if (long.TryParse(response.aracTescilBilgileri.tescilTarihi.time, out time))
                        {
                            trafikTescilTarihi = MapfreSorguResponse.FromJavaTime(time).ToString("dd.MM.yyyy");
                        }
                    }
                    if (!String.IsNullOrEmpty(response.aracBilgi.motorNo))
                    {
                        motorNo = response.aracBilgi.motorNo;
                    }
                    if (!String.IsNullOrEmpty(response.aracBilgi.sasiNo))
                    {
                        sasiNo = response.aracBilgi.sasiNo;
                    }
                }
                if (String.IsNullOrEmpty(trafikTescilTarihi))
                {
                    trafikTescilTarihi = DateTime.Today.ToString("dd.MM.yyyy");
                }

            }

            AracTescilTarihiModel model = new AracTescilTarihiModel();
            model.AracTrafikTescilTarihi = trafikTescilTarihi;
            model.MotorNo = motorNo;
            model.SasiNo = sasiNo;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //Plaka Sorgula butonuna tıklandığında çağırılıyor
        public AnadoluKTipResultView KullanimTipiSorgulaAnadolu(string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu, int TVMKodu, string Kasko)
        {
            AnadoluKTipResultView model = new AnadoluKTipResultView();
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

            var AnadoluKullanimTipleri = _trafik.AracKullanimTipiList(AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, TVMKodu, Kasko);
            model.list = AnadoluKullanimTipleri.list;
            model.hata = AnadoluKullanimTipleri.hata;
            model.anadoluMarkaKodu = AnadoluKullanimTipleri.anadoluMarkaKodu;
            return model;
        }

        //İleri butonuna tıklandığında çağırılıyor
        //[AjaxException]
        //public ActionResult KullanimTipListAnadolu(string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu)
        //{
        //    try
        //    {
        //        AnadoluResultView model = new AnadoluResultView();
        //        ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
        //        IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
        //        IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
        //        KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);

        //        string TRAMER_TARIFE_KODU = String.Empty;
        //        string[] parts = AracKullanimTarzi.Split('-');
        //        if (parts.Length == 2)
        //        {
        //            string kullanimTarziKodu = parts[0];
        //            string kod2 = parts[1];
        //            CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
        //                                                                                          f.KullanimTarziKodu == kullanimTarziKodu &&
        //                                                                                          f.Kod2 == kod2)
        //                                                                                          .SingleOrDefault<CR_KullanimTarzi>();

        //            if (kullanimTarzi != null)
        //            {
        //                TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
        //            }
        //        }

        //        var anadoluModel = _trafik.AracKullanimTipiList(AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
        //        model.list = new SelectList(anadoluModel.list, "Key", "Value").ListWithOptionLabel();
        //        model.hata = anadoluModel.hata;
        //        model.anadoluMarkaKodu = anadoluModel.anadoluMarkaKodu;
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ILogService log = DependencyResolver.Current.GetService<ILogService>();
        //        log.Error(ex);
        //        throw;
        //    }
        //}

        [AjaxException]
        public ActionResult KullanimTipListAnadolu(string AracKullanimTarzi, string AracMarkaKodu)
        {
            try
            {
                AnadoluResultView model = new AnadoluResultView();

                string[] parts = AracKullanimTarzi.Split('-');

                IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
                IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);
                var AnadoluMarka = _trafik.AracMarka(AracMarkaKodu, _AktifKullaniciService.TVMKodu);
                if (!String.IsNullOrEmpty(AnadoluMarka.anadoluMarkaKodu))
                {
                    model.anadoluMarkaKodu = AnadoluMarka.anadoluMarkaKodu;
                }
                else
                {
                    model.anadoluMarkaKodu = "";
                }
                var liste = _TeklifService.GetAnadoluKullanimTipleri(parts[0], AnadoluKullanimTipiSorguTipi.KullanimTarzi);
                if (liste != null)
                {
                    model.list = new SelectList(liste, "KullanimTipi", "TipAdi").ListWithOptionLabel();
                    model.hata = "";
                }
                else
                {
                    model.hata = "Kullanım Tipleri yüklenirken bir hata oluştu.";
                    model.list = new List<SelectListItem>();
                }


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
        public ActionResult KullanimSekliSorgulaAnadolu(string AnadoluKullanimTipi)
        {
            try
            {
                AnadoluResultView model = new AnadoluResultView();
                var AnadoluKullanimSekilleri = _TeklifService.GetAnadoluKullanimTipleri(AnadoluKullanimTipi, AnadoluKullanimTipiSorguTipi.KullanimSekli);
                if (AnadoluKullanimSekilleri != null)
                {
                    model.list = new SelectList(AnadoluKullanimSekilleri, "KullanimSekli", "SekilAdi").ListWithOptionLabel();
                    model.hata = "";
                }
                else
                {
                    model.hata = "Kullanım Şekilleri yüklenirken hata oluştu.";
                    model.list = new List<SelectListItem>();
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }
        //[AjaxException]
        //public ActionResult KullanimSekliSorgulaAnadolu(string AnadoluMarkaKodu, string AracKullanimTarzi, string AracModelYili, string AracMarkaKodu, string KullanimTipi)
        //{
        //    try
        //    {
        //        AnadoluResultView model = new AnadoluResultView();
        //        ICRContext _CRContext = DependencyResolver.Current.GetService<ICRContext>();
        //        IANADOLUTrafik _trafik = DependencyResolver.Current.GetService<IANADOLUTrafik>();
        //        string TRAMER_TARIFE_KODU = String.Empty;
        //        string[] parts = AracKullanimTarzi.Split('-');
        //        if (parts.Length == 2)
        //        {
        //            string kullanimTarziKodu = parts[0];
        //            string kod2 = parts[1];
        //            CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
        //                                                                                          f.KullanimTarziKodu == kullanimTarziKodu &&
        //                                                                                          f.Kod2 == kod2)
        //                                                                                          .SingleOrDefault<CR_KullanimTarzi>();

        //            if (kullanimTarzi != null)
        //            {
        //                TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
        //            }
        //        }

        //        var AnadoluKullanimSekilleri = _trafik.AracKullanimSekliList(AnadoluMarkaKodu, AracModelYili, AracMarkaKodu, TRAMER_TARIFE_KODU, KullanimTipi, _AktifKullaniciService.TVMKodu);
        //        model.list = new SelectList(AnadoluKullanimSekilleri.list, "key", "value").ListWithOptionLabel();
        //        model.hata = AnadoluKullanimSekilleri.hata;
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ILogService log = DependencyResolver.Current.GetService<ILogService>();
        //        log.Error(ex);
        //        throw;
        //    }
        //}

        [AjaxException]
        public ActionResult IkameListAnadolu(string aracKodu, string kullanimSekliKodu, string kullanimTipKodu)
        {
            try
            {
                AnadoluResultView model = new AnadoluResultView();
                IANADOLUKasko _kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();

                var result = _kasko.getAnadoluTeminatlar(aracKodu, kullanimSekliKodu, kullanimTipKodu, _AktifKullaniciService.TVMKodu);
                model.list = new SelectList(result.ikameList, "key", "value").ListWithOptionLabel();
                model.hata = result.hataMesaji;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }

        }

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

        [HttpPost]
        [AjaxException]
        public ActionResult GetTurkNipponKaskoYetkiliIndirimi(string sigortaliKimlikNo, string sigortaEttirenKimlikNo, string plakaKodu, string plakaNo, DateTime policeBaslangicTarihi)
        {
            try
            {
                NipponYetkiliIndirimiReturnModel model = new NipponYetkiliIndirimiReturnModel();
                ITURKNIPPONKasko kasko = DependencyResolver.Current.GetService<ITURKNIPPONKasko>();
                var indirim = kasko.GetKaskoIndirimi(sigortaliKimlikNo, sigortaEttirenKimlikNo, plakaKodu, plakaNo, policeBaslangicTarihi, _AktifKullaniciService.TVMKodu);
                if (indirim.Length > 3)
                {
                    model.hata = indirim;
                    model.YetkiliIndirim = 0;
                }
                else
                {
                    model.hata = "";
                    model.YetkiliIndirim = Convert.ToInt32(indirim);
                }
                return Json(model);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("KaskoController.YetkiliIndirimiSorgula", ex);
                throw;
            }
        }

        public TasinanYukKademelerResultView GetTasinanYukKademeleri(string AracKullanimTarzi)
        {
            TasinanYukKademelerResultView model = new TasinanYukKademelerResultView();
            ITeklifService _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
            ListModel kademeList = new ListModel();
            model.list = new List<ListModel>();
            var kademeler = _TeklifService.getTasinanYukKademleri(AracKullanimTarzi);
            if (kademeler != null)
            {
                foreach (var item in kademeler)
                {
                    kademeList = new ListModel();
                    kademeList.key = item.Kademe.ToString();
                    kademeList.value = item.Aciklama;
                    model.list.Add(kademeList);
                }
            }
            return model;
        }

        public ActionResult TasinanYukKademeleri(string AracKullanimTarzi)
        {
            TasinanYukKademelerResultView model = new TasinanYukKademelerResultView();
            ITeklifService _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
            ListModel kademeList = new ListModel();
            model.list = new List<ListModel>();
            var kademeler = _TeklifService.getTasinanYukKademleri(AracKullanimTarzi);
            foreach (var item in kademeler)
            {
                kademeList = new ListModel();
                kademeList.key = item.Kademe.ToString();
                kademeList.value = item.Aciklama;
                model.list.Add(kademeList);

            }
            var liste = new SelectList(model.list, "key", "value").ListWithOptionLabel();
            return Json(new { list = liste }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SompoJapanFaaliyetKodlari(int tvmKodu)
        {
            Business.sompojapan.kasko.Casco clntKasko = new Business.sompojapan.kasko.Casco();
            ITVMContext _TVMContext = DependencyResolver.Current.GetService<ITVMContext>();
            ISOMPOJAPANKasko _Sompo = DependencyResolver.Current.GetService<ISOMPOJAPANKasko>();
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

            clntKasko.IdentityHeaderValue = new Business.sompojapan.kasko.IdentityHeader()
            {
                KullaniciAdi = servisKullanici.KullaniciAdi,
                KullaniciParola = servisKullanici.Sifre,
                KullaniciIP = _Sompo.IpGetir(tvmKodu),
                KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.ACENTE
            };

            List<ListModel> faaliyetList = new List<ListModel>();
            ListModel faaliyet = new ListModel();
            var sompoFaaliyetList = clntKasko.GetCorporateActivityCodesExt();
            clntKasko.Dispose();
            if (sompoFaaliyetList != null)
            {
                foreach (var item in sompoFaaliyetList)
                {
                    faaliyet = new ListModel();
                    faaliyet.key = item.ValueField;
                    faaliyet.value = item.TextField;
                    faaliyetList.Add(faaliyet);
                }
            }

            var liste = new SelectList(faaliyetList, "key", "value").ListWithOptionLabel();
            return Json(new { list = liste }, JsonRequestBehavior.AllowGet);
        }

        public List<ListModel> GetSompoJapanFaaliyetKodlari(int tvmKodu)
        {
            Business.sompojapan.kasko.Casco clntKasko = new Business.sompojapan.kasko.Casco();
            ITVMContext _TVMContext = DependencyResolver.Current.GetService<ITVMContext>();
            ISOMPOJAPANKasko _Sompo = DependencyResolver.Current.GetService<ISOMPOJAPANKasko>();
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

            clntKasko.IdentityHeaderValue = new Business.sompojapan.kasko.IdentityHeader()
            {
                KullaniciAdi = servisKullanici.KullaniciAdi,
                KullaniciParola = servisKullanici.Sifre,
                KullaniciIP = _Sompo.IpGetir(tvmKodu),
                KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.ACENTE
            };
            List<ListModel> faaliyetList = new List<ListModel>();
            ListModel faaliyet = new ListModel();
            var sompoFaaliyetList = clntKasko.GetCorporateActivityCodesExt();
            clntKasko.Dispose();
            if (sompoFaaliyetList != null)
            {
                foreach (var item in sompoFaaliyetList)
                {
                    faaliyet = new ListModel();
                    faaliyet.key = item.ValueField;
                    faaliyet.value = item.TextField;
                    faaliyetList.Add(faaliyet);
                }
            }
            return faaliyetList;
        }

        public List<ListModel> GetGroupamaTeminatLimitleri()
        {
            List<ListModel> teminatlimitList = new List<ListModel>();
            ListModel teminat = new ListModel();
            IGROUPAMAKasko groupama = DependencyResolver.Current.GetService<IGROUPAMAKasko>();
            var list = groupama.KazaDestekTeminatLimitleri();
            if (list != null)
            {
                foreach (var item in list)
                {
                    teminat = new ListModel();
                    teminat.key = item.kodu;
                    teminat.value = item.aciklama;
                    teminatlimitList.Add(teminat);
                }
            }
            return teminatlimitList;
        }

        public List<ListModel> GetGroupamaYHIMSBasamaklar()
        {
            List<ListModel> teminatlimitList = new List<ListModel>();
            ListModel teminat = new ListModel();
            IGROUPAMAKasko groupama = DependencyResolver.Current.GetService<IGROUPAMAKasko>();
            var list = groupama.YHIMSBasamakLimitleri();
            if (list != null)
            {
                foreach (var item in list)
                {
                    teminat = new ListModel();
                    teminat.key = item.kodu;
                    teminat.value = item.aciklama;
                    teminatlimitList.Add(teminat);
                }
            }
            return teminatlimitList;
        }

        //Kaza Destek Teminatı değiştiğinde çağırılıyor
        [AjaxException]
        public ActionResult GetGroupamaTeminatLimiti()
        {
            try
            {
                var teminatlimitList = new SelectList(this.GetGroupamaTeminatLimitleri(), "key", "value").ListWithOptionLabel();
                return Json(new { list = teminatlimitList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        //İleri butonuna tıklandığında kullanılıyor
        public class AnadoluResultView
        {
            public List<SelectListItem> list { get; set; }
            public string hata { get; set; }
            public string anadoluMarkaKodu { get; set; }
            public string hasarsizlik { get; set; }
        }

        //Plaka sorgula butonuna tıklandığında kullanılıyor
        public class AnadoluKTipResultView
        {
            public List<ListModel> list { get; set; }
            public string hata { get; set; }
            public string anadoluMarkaKodu { get; set; }
        }

        public class TasinanYukKademelerResultView
        {
            public List<ListModel> list { get; set; }
            public string hata { get; set; }
        }
        public class HasarsizlikReturnModel
        {
            public string HasarsizlikInd { get; set; }
            public string HasarsizlikSur { get; set; }
            public string HasarsizlikKademe { get; set; }
            public string hata { get; set; }
        }
        public class NipponYetkiliIndirimiReturnModel
        {
            public int YetkiliIndirim { get; set; }
            public string hata { get; set; }
        }

        //[AjaxException]
        //public ActionResult KaskoYurticiTasiyiciKademeler(string tumKodu, string kullanimTarzi1,string kullanimTarzi2)
        //{
        //    try
        //    {
        //        AnadoluResultView model = new AnadoluResultView();
        //        IANADOLUKasko _kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();

        //        var result = _kasko.getAnadoluTeminatlar(aracKodu, kullanimSekliKodu, kullanimTipKodu, _AktifKullaniciService.TVMKodu);
        //        model.list = new SelectList(result.ikameList, "key", "value").ListWithOptionLabel();
        //        model.hata = result.hataMesaji;
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ILogService log = DependencyResolver.Current.GetService<ILogService>();
        //        log.Error(ex);
        //        throw;
        //    }

        //}
        [AjaxException]
        public ActionResult EskiPoliceSorgula(string kimlikNo, string sigortaSirketi, string acenteNo, string policeNo, string yenilemeNo)
        {
            try
            {
                PlakaSorgu plakaSorgu = MAPFREEskiPoliceSorgula(sigortaSirketi, acenteNo, policeNo, yenilemeNo);

                IMAPFRESorguService sorguService = DependencyResolver.Current.GetService<IMAPFRESorguService>();

                if (!String.IsNullOrEmpty(plakaSorgu.EskiPoliceNo))
                {
                    var hasarsizlik = HasarsizlikSorgula(kimlikNo, plakaSorgu.EskiPoliceSigortaSirkedKodu, plakaSorgu.EskiPoliceAcenteKod, plakaSorgu.EskiPoliceNo, plakaSorgu.EskiPoliceYenilemeNo, "420");
                    if (hasarsizlik != null)
                    {
                        plakaSorgu.HasarsizlikInd = hasarsizlik.HasarsizlikInd;
                        plakaSorgu.HasarsizlikSur = hasarsizlik.HasarsizlikSur;
                        plakaSorgu.HasarsizlikKademe = hasarsizlik.HasarsizlikKademe;
                        plakaSorgu.HasarsizlikHata = hasarsizlik.hata;
                    }
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
                var ktipleri = KullanimTipiSorgulaAnadolu(plakaSorgu.AracKullanimTarzi, plakaSorgu.AracModelYili, plakaSorgu.AracMarkaKodu, _AktifKullaniciService.TVMKodu, ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                plakaSorgu.AnadoluKullanimTipleri = new SelectList(ktipleri.list, "key", "value").ListWithOptionLabel();
                plakaSorgu.AnadoluHata = ktipleri.hata;
                return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error("KaskoController.EskiPoliceSorgula", ex);
                throw;
            }
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

        public class GroupamaResultView
        {
            public List<ListModel> list { get; set; }
            public string hata { get; set; }
        }

        public DateTime ToDateTime(string dateValue)
        {
            string[] parts = dateValue.Split('-');

            if (parts.Length == 3)
            {
                int day = int.Parse(parts[2].Substring(0, 2));
                int mon = int.Parse(parts[1]);
                int year = int.Parse(parts[0]);

                return new DateTime(year, mon, day);
            }

            return DateTime.MinValue;
        }

        public ActionResult UrunleriGetir()
        {
            OnerilenUrunModel model = new OnerilenUrunModel();
            model.list = DummyPolicyApp.Test();
            model.list = model.list.OrderByDescending(s => s.probability).ToList();
            return PartialView("_OnerilenUrunler", model);
        }
    }

}

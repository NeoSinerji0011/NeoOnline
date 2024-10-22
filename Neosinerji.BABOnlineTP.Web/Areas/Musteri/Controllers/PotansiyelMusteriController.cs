using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using Neosinerji.BABOnlineTP.Business;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Net;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Service;
using System.IO;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using Neosinerji.BABOnlineTP.Web.Areas.PotansiyelMusteri.Models;
using musteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Musteri.Controllers
{
    [IDAuthority(Type = "PotansiyelMusteri")]
    [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = 0, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class PotansiyelMusteriController : Controller
    {
        IMusteriService _MusteriService;
        IUlkeService _UlkeService;
        IMusteriDokumanStorage _Storage;
        ITanimService _MeslekVeTanimService;
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;
        ITeklifService _TeklifService;
        IMusteriDokumanStorage _MusteriStorage;
        ILogService _Log;

        public PotansiyelMusteriController(IMusteriService potansiyelMusteri,
                                 IUlkeService ulke,
                                 IMusteriDokumanStorage storage,
                                 ITanimService meslekVeTanim,
                                 ITVMService tvm,
                                 IKullaniciService kullanici,
                                 IAktifKullaniciService aktifKullanici,
                                 ITeklifService teklifService,
                                 IMusteriDokumanStorage musteriStorage)
        {
            _UlkeService = ulke;
            _MusteriService = potansiyelMusteri;
            _Storage = storage;
            _MeslekVeTanimService = meslekVeTanim;
            _TVMService = tvm;
            _KullaniciService = kullanici;
            _AktifKullanici = aktifKullanici;
            _TeklifService = teklifService;
            _MusteriStorage = musteriStorage;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }


        #region Views
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Liste(string teklifTipi)
        {
            PotansiyelMusteriListeModel model = new PotansiyelMusteriListeModel();
            model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
            model.TVMKodu = _AktifKullanici.TVMKodu;
            model.TVMUnvani = _AktifKullanici.TVMUnvani;

            List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
            model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

            if (!String.IsNullOrEmpty(teklifTipi))
            {
                model.TeklifTipi = teklifTipi;
                model.TeklifAl = true;
            }

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult ListePager()
        {
            if (Request["sEcho"] != null)
            {
                PotansiyelMusteriListe arama = new PotansiyelMusteriListe(Request, new Expression<Func<PotansiyelMusteriListeModelOzel, object>>[]
                                                                    {
                                                                        t =>t.MusteriKodu,
                                                                        t =>t.TVMMusteriKodu,
                                                                        t =>t.AdiUnvan,
                                                                        t =>t.EMail,
                                                                        t =>t.Cinsiyet,
                                                                        t =>t.DogumTarihi,
                                                                        t =>t.BagliOlduguTvmText
                                                                    });

                arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu");
                arama.MusteriTipKodu = arama.TryParseParamShort("MusteriTipKodu");
                arama.KimlikNo = arama.TryParseParamString("KimlikNo");
                arama.AdiUnvan = arama.TryParseParamString("AdiUnvan");
                arama.SoyadiUnvan = arama.TryParseParamString("SoyadiUnvan");
                arama.EMail = arama.TryParseParamString("EMail");
                arama.TVMMusteriKodu = arama.TryParseParamString("TVMMusteriKodu");
                arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                arama.PasaportNo = arama.TryParseParamString("PasaportNo");


                int totalRowCount = 0;
                List<PotansiyelMusteriListeModelOzel> list = _MusteriService.PagedListPotansiyel(arama, out totalRowCount);

                arama.LinkColumn1Url = "/Musteri/PotansiyelMusteri/Detay/";
                arama.UpdateUrl = "/Musteri/PotansiyelMusteri/Guncelle/";
                arama.RowIdColumn = a => a.MusteriKodu;
                string teklifTipi = Request["TeklifTipi"];
                if (!String.IsNullOrEmpty(teklifTipi))
                {
                    string teklifUrl = String.Empty;
                    switch (teklifTipi)
                    {
                        case "trafik": teklifUrl = "/Teklif/Trafik/Ekle/"; break;
                    }

                    arama.AddFormatter(f => f.AdiUnvan, f => String.Format("<a href='{0}{1}'>{2}</a>", teklifUrl, f.MusteriKodu, f.AdiUnvan));
                }
                else
                {
                    arama.AddFormatter(f => f.AdiUnvan, f => String.Format("<a href='/Musteri/PotansiyelMusteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.AdiUnvan));
                }
                //string gorevGuncelleUrl = "/GorevTakip/GorevTakip/Guncelle";
                string gorevEkleUrl = "/GorevTakip/GorevTakip/Ekle/";
                arama.GorevUrl = gorevEkleUrl;

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Detay(int id)
        {
            try
            {
                PotansiyelMusteriModel model = new PotansiyelMusteriModel();
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(id);

                if (potansiyelMusteri == null)
                    return new RedirectResult("~/Error/ErrorPage/403");

                Mapper.CreateMap<PotansiyelMusteriGenelBilgiler, PotansiyelGenelBilgilerModel>();

                model.PotansiyelGenelBilgiler = Mapper.Map<PotansiyelMusteriGenelBilgiler, PotansiyelGenelBilgilerModel>(potansiyelMusteri);
                model.PotansiyelGenelBilgiler.TVMUnvani = _TVMService.GetTvmUnvan(potansiyelMusteri.TVMKodu);

                //Müşteri Tİpinin Adı  ve medeni durumu text olarak alınıyor..
                model.PotansiyelGenelBilgiler.MusteriTipiText = MusteriListProvider.GetMusteriTipiText(model.PotansiyelGenelBilgiler.MusteriTipKodu);
                if (model.PotansiyelGenelBilgiler.MedeniDurumu != null && model.PotansiyelGenelBilgiler.MedeniDurumu.Value > 0)
                    model.PotansiyelGenelBilgiler.MedeniDurumText = MusteriListProvider.GetMedeniDurumText(model.PotansiyelGenelBilgiler.MedeniDurumu.Value);
                if (model.PotansiyelGenelBilgiler.EgitimDurumu.HasValue && model.PotansiyelGenelBilgiler.EgitimDurumu.Value > 0)
                    model.PotansiyelGenelBilgiler.EgitimDurumuText = _MeslekVeTanimService.GetTanim("Egitim", model.PotansiyelGenelBilgiler.EgitimDurumu.ToString()).Aciklama;
                if (model.PotansiyelGenelBilgiler.MeslekKodu != null && model.PotansiyelGenelBilgiler.MeslekKodu.Value > 0)
                    model.PotansiyelGenelBilgiler.MeslekKoduText = _MeslekVeTanimService.GetMeslek(model.PotansiyelGenelBilgiler.MeslekKodu.Value).MeslekAdi;

                model.Adresleri = new PotansiyelAdresListModel();
                model.Telefonlari = new PotansiyelMusteriTelefonListModel();
                model.Dokumanlari = new PotansiyelDokumanListModel();
                model.Notlari = new PotansiyelNotListModel();

                model.Adresleri.sayfaAdi = "detay";
                model.Telefonlari.sayfaAdi = "detay";
                model.Dokumanlari.sayfaAdi = "detay";
                model.Notlari.sayfaAdi = "detay";
                model.Adresleri.Items = PotansiyelMusteriAdresleriGetir(potansiyelMusteri.PotansiyelMusteriKodu);
                model.Dokumanlari.Items = PotansiyelMusteriDokumanlariGetir(potansiyelMusteri.PotansiyelMusteriKodu);
                model.Telefonlari.Items = PotansiyelMusteriTelefonlariGetir(potansiyelMusteri.PotansiyelMusteriKodu);
                model.Notlari.Items = PotansiyelMusteriNotlariGetir(potansiyelMusteri.PotansiyelMusteriKodu);

                //Tüzel müşteri ise alanlar doluyor
                //if (musteri.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                //    TuzelMusteriDetayDoldur(model, musteri);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
            AltMenuKodu = AltMenuler.PotansiyelMusteri,
            SekmeKodu = AltMenuSekmeler.Ekle,
            menuPermission = MenuPermission.Ekleme,
            menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle()
        {
            try
            {

                PotansiyelMusteriModel model = new PotansiyelMusteriModel();

                model.PotansiyelMusteriTelefonModel = new PotansiyelMusteriTelefonModel();
                model.PotansiyelMusteriAdresModel = new PotansiyelAdresModel();
                model.PotansiyelGenelBilgiler = new PotansiyelGenelBilgilerModel();
                model.PotansiyelGenelBilgiler.TVMKodu = _AktifKullanici.TVMKodu;
                model.PotansiyelGenelBilgiler.TVMUnvani = _AktifKullanici.TVMUnvani;

                model.PotansiyelGenelBilgiler.MusteriTipKodu = 1;
                model.PotansiyelGenelBilgiler.PasaportGecerlilikBitisTarihi = TurkeyDateTime.Today;


                model.PotansiyelGenelBilgiler.Cinsiyet = null;
                model.PotansiyelMusteriAdresModel.AdresTipi = 8;
                model.PotansiyelMusteriAdresModel.UlkeKodu = "TUR";

                EkleModelDoldur(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
            AltMenuKodu = AltMenuler.PotansiyelMusteri,
            SekmeKodu = AltMenuSekmeler.Ekle,
            menuPermission = MenuPermission.Ekleme,
            menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle(PotansiyelMusteriModel model)
        {
            try
            {
                //İletişim telefon tipinin zorunluluğu kaldırılıyor
                if (model.PotansiyelMusteriTelefonModel != null)
                {
                    if (model.PotansiyelMusteriTelefonModel.Numara == null)
                    {
                        if (ModelState["PotansiyelMusteriTelefonModel.IletisimNumaraTipi"] != null)
                            ModelState["PotansiyelMusteriTelefonModel.IletisimNumaraTipi"].Errors.Clear();
                    }
                }

                if (ModelState.IsValid)
                {
                    if (_AktifKullanici != null && model.PotansiyelGenelBilgiler.TVMKodu.HasValue)
                    {
                        PotansiyelMusteriGenelBilgiler potansiyelMusteri = new PotansiyelMusteriGenelBilgiler();

                        if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(model.PotansiyelGenelBilgiler.TVMKodu.Value))
                            potansiyelMusteri.TVMKodu = model.PotansiyelGenelBilgiler.TVMKodu.Value;
                        else
                            return new RedirectResult("~/Error/ErrorPage/403");


                        if (!String.IsNullOrEmpty(model.PotansiyelGenelBilgiler.KimlikNo) && model.PotansiyelGenelBilgiler.TVMKodu.HasValue)
                            if (!TCKontrol(model.PotansiyelGenelBilgiler.KimlikNo, model.PotansiyelGenelBilgiler.TVMKodu.Value))
                            {
                                EkleModelDoldur(model);
                                ModelState.AddModelError("", babonline.Message_TCKN_AlreadyExist);
                                return View(model);
                            }

                        //Her Müşteri tipi için zorunlu olan alanlar ekleniyor (automapper kullanılmadan).....
                        potansiyelMusteri.AdiUnvan = model.PotansiyelGenelBilgiler.AdiUnvan;
                        potansiyelMusteri.SoyadiUnvan = model.PotansiyelGenelBilgiler.SoyadiUnvan;
                        potansiyelMusteri.KimlikNo = model.PotansiyelGenelBilgiler.KimlikNo;
                        potansiyelMusteri.MusteriTipKodu = model.PotansiyelGenelBilgiler.MusteriTipKodu;
                        potansiyelMusteri.WebUrl = model.PotansiyelGenelBilgiler.WebUrl;
                        potansiyelMusteri.EMail = model.PotansiyelGenelBilgiler.EMail;
                        potansiyelMusteri.Uyruk = model.PotansiyelGenelBilgiler.Uyruk;
                        potansiyelMusteri.TVMMusteriKodu = model.PotansiyelGenelBilgiler.TVMMusteriKodu;
                        potansiyelMusteri.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;


                        //Müşteri Tip kodu bir şekilde 0 gelirse model hata ile birlikte geri döndürülüyor...
                        if (model.PotansiyelGenelBilgiler.MusteriTipKodu == MusteriTipleri.Yok)
                        {
                            ModelState.AddModelError("", "Müşteri Tip Kodu Belirtiniz..");
                            EkleModelDoldur(model);
                            return View(model);
                        }

                        //TC Müşteri için eklenicek veriler...
                        if (model.PotansiyelGenelBilgiler.MusteriTipKodu == MusteriTipleri.TCMusteri || model.PotansiyelGenelBilgiler.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                        {
                            potansiyelMusteri.DogumTarihi = model.PotansiyelGenelBilgiler.DogumTarihi;
                            potansiyelMusteri.Cinsiyet = model.PotansiyelGenelBilgiler.Cinsiyet;
                            potansiyelMusteri.EgitimDurumu = model.PotansiyelGenelBilgiler.EgitimDurumu;
                            potansiyelMusteri.MedeniDurumu = model.PotansiyelGenelBilgiler.MedeniDurumu;
                            potansiyelMusteri.MeslekKodu = model.PotansiyelGenelBilgiler.MeslekKodu;
                        }
                        //Müsteri tip kodu kaydediliyor...
                        potansiyelMusteri.MusteriTipKodu = model.PotansiyelGenelBilgiler.MusteriTipKodu;

                        //Telefon Bilgileri telefon tablosuna ekleniyor..
                        PotansiyelMusteriTelefon telefon = new PotansiyelMusteriTelefon();
                        if (model.PotansiyelMusteriTelefonModel.IletisimNumaraTipi > 0 && !String.IsNullOrEmpty(model.PotansiyelMusteriTelefonModel.Numara) &&
                             model.PotansiyelMusteriTelefonModel.Numara.Length == 14)
                        {
                            Mapper.CreateMap<PotansiyelMusteriTelefonModel, PotansiyelMusteriTelefon>();
                            telefon = Mapper.Map<PotansiyelMusteriTelefon>(model.PotansiyelMusteriTelefonModel);
                        }

                        //Adres Bilgileri adres tablosuna ekleniyor....
                        PotansiyelMusteriAdre adres = new PotansiyelMusteriAdre();
                        if (model.PotansiyelMusteriAdresModel.AdresTipi.HasValue && model.PotansiyelMusteriAdresModel.AdresTipi.Value > 0)
                        {

                            Mapper.CreateMap<PotansiyelAdresModel, PotansiyelMusteriAdre>();
                            adres = Mapper.Map<PotansiyelMusteriAdre>(model.PotansiyelMusteriAdresModel);
                            adres.Adres = adres.Adres == null ? String.Empty : adres.Adres;
                        }

                        potansiyelMusteri = _MusteriService.CreatePotansiyelMusteri(potansiyelMusteri, adres, telefon);

                        return RedirectToAction("Detay", "PotansiyelMusteri", new { Id = potansiyelMusteri.PotansiyelMusteriKodu });
                    }
                    return new RedirectResult("~/Error/ErrorPage/403");
                }
                //Model valid değilse değerler dolduruluyor...Geri DOndürülücek.....
                EkleModelDoldur(model);
                ModelState.AddModelError("", babonline.Message_CustomerSaveError);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
            AltMenuKodu = AltMenuler.PotansiyelMusteri,
            SekmeKodu = AltMenuSekmeler.AraGuncelle,
            menuPermission = MenuPermission.Guncelleme,
            menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(int Id)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(Id);
                if (potansiyelMusteri == null)
                    return new RedirectResult("~/Error/ErrorPage/403");


                PotansiyelMusteriModel model = new PotansiyelMusteriModel();
                Mapper.CreateMap<PotansiyelMusteriGenelBilgiler, PotansiyelGuncelleModel>();

                model.PotansiyelMusteriGuncelleModel = Mapper.Map<PotansiyelMusteriGenelBilgiler, PotansiyelGuncelleModel>(potansiyelMusteri);
                model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", potansiyelMusteri.Cinsiyet);

                if (potansiyelMusteri.MusteriTipKodu != 4)
                    model.PotansiyelMusteriGuncelleModel.PasaportGecerlilikBitisTarihi = TurkeyDateTime.Today;

                GuncelleModelDoldur(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
            AltMenuKodu = AltMenuler.PotansiyelMusteri,
            SekmeKodu = AltMenuSekmeler.AraGuncelle,
            menuPermission = MenuPermission.Guncelleme,
            menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        [ValidateAntiForgeryToken]
        public ActionResult Guncelle(PotansiyelMusteriModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriGuncelleModel.PotansiyelMusteriKodu);

                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    potansiyelMusteri.AdiUnvan = model.PotansiyelMusteriGuncelleModel.AdiUnvan;
                    potansiyelMusteri.SoyadiUnvan = model.PotansiyelMusteriGuncelleModel.SoyadiUnvan;
                    potansiyelMusteri.KimlikNo = model.PotansiyelMusteriGuncelleModel.KimlikNo;
                    potansiyelMusteri.MusteriTipKodu = model.PotansiyelMusteriGuncelleModel.MusteriTipKodu;
                    potansiyelMusteri.WebUrl = model.PotansiyelMusteriGuncelleModel.WebUrl;
                    potansiyelMusteri.EMail = model.PotansiyelMusteriGuncelleModel.EMail;
                    potansiyelMusteri.TVMMusteriKodu = model.PotansiyelMusteriGuncelleModel.TVMMusteriKodu;

                    //Tüzel Müşteri ve sahıs vergi dairesini girmemişse hata ile model geri döndürülüyor...
                    if (model.PotansiyelMusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.TuzelMusteri || model.PotansiyelMusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                    {
                        if (model.PotansiyelMusteriGuncelleModel.VergiDairesi == "0")
                        {
                            ModelState.AddModelError("", babonline.Message_TaxOfficeRequired);
                            EkleModelDoldur(model);
                            return View(model);
                        }
                        else
                            potansiyelMusteri.VergiDairesi = model.PotansiyelMusteriGuncelleModel.VergiDairesi;
                    }

                    //Yabancı Müşteri için Ekstra olarak pasaport alanlari kaydediliyor diğerleri için null .....
                    if (model.PotansiyelMusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.YabanciMusteri && model.PotansiyelMusteriGuncelleModel.PasaportNo != null)
                    {
                        potansiyelMusteri.PasaportNo = model.PotansiyelMusteriGuncelleModel.PasaportNo;
                        potansiyelMusteri.PasaportGecerlilikBitisTarihi = model.PotansiyelMusteriGuncelleModel.PasaportGecerlilikBitisTarihi;
                    }

                    //TC Müşteri için eklenicek veriler...
                    if (model.PotansiyelMusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.TCMusteri || model.PotansiyelMusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                    {
                        potansiyelMusteri.DogumTarihi = model.PotansiyelMusteriGuncelleModel.DogumTarihi;
                        potansiyelMusteri.Cinsiyet = model.PotansiyelMusteriGuncelleModel.Cinsiyet;
                        potansiyelMusteri.EgitimDurumu = model.PotansiyelMusteriGuncelleModel.EgitimDurumu;
                        potansiyelMusteri.MedeniDurumu = model.PotansiyelMusteriGuncelleModel.MedeniDurumu;
                        potansiyelMusteri.MeslekKodu = model.PotansiyelMusteriGuncelleModel.MeslekKodu;
                    }

                    _MusteriService.UpdatePotansiyelMusteri(potansiyelMusteri);
                    return RedirectToAction("Detay", "PotansiyelMusteri", new { Id = potansiyelMusteri.PotansiyelMusteriKodu });
                }

                GuncelleModelDoldur(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Teklifleri(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(Id);

                    //Yetkisiz Erişim
                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                    model.BitisTarihi = TurkeyDateTime.Now;

                    if (potansiyelMusteri == null)
                    {
                        ModelState.AddModelError("", "Müşteri kodu hatalı");
                        model.Teklifleri = new List<TeklifOzelDetay>();
                        return View(model);
                    }

                    model.MusteriKodu = Id;
                    model.MusteriAdSoyad = potansiyelMusteri.AdiUnvan + " " + potansiyelMusteri.SoyadiUnvan;
                    model.Teklifleri = _TeklifService.GetMusteriTeklifleri(Id, model.BaslangicTarihi, model.BitisTarihi, 1);
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();

                    foreach (var item in model.Teklifleri)
                        item.OzelAlan = RaporController.GetOzelAlan(item.TeklifId);
                    return View(model);
                }
                else
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    model.Teklifleri = new List<TeklifOzelDetay>();
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }

        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Teklifleri(PotansiyelMusteriTeklifleriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.MusteriKodu);
                    //Yetkisiz Erişim
                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    //if (potansiyelMusteri == null)
                    //{
                    //    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    //    model.Teklifleri = new List<TeklifOzelDetay>();
                    //    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    //    return View(model);
                    //}
                    model.MusteriAdSoyad = potansiyelMusteri.AdiUnvan + " " + potansiyelMusteri.SoyadiUnvan;
                    model.Teklifleri = _TeklifService.GetMusteriTeklifleri(model.MusteriKodu, model.BaslangicTarihi, model.BitisTarihi, model.TeklifTarihi);

                    foreach (var item in model.Teklifleri)
                        item.OzelAlan = RaporController.GetOzelAlan(item.TeklifId);

                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    return View(model);
                }
                else
                {
                    model.Teklifleri = new List<TeklifOzelDetay>();
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Policeleri(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(Id);

                    //Yetkisiz Erişim
                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/Errorpage/403");

                    model.BaslangicTarihi = TurkeyDateTime.Now;
                    model.BitisTarihi = TurkeyDateTime.Now.AddDays(1);

                    if (potansiyelMusteri == null)
                    {
                        ModelState.AddModelError("", "Müşteri kodu hatalı");
                        model.Teklifleri = new List<TeklifOzelDetay>();
                        return View(model);
                    }
                    model.MusteriKodu = Id;
                    model.MusteriAdSoyad = potansiyelMusteri.AdiUnvan + " " + potansiyelMusteri.SoyadiUnvan;
                    model.Teklifleri = _TeklifService.GetMusteriPoliceleri(Id, TurkeyDateTime.Now.AddDays(-2), TurkeyDateTime.Now, 1);
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    foreach (var item in model.Teklifleri)
                        item.OzelAlan = RaporController.GetOzelAlan(item.TeklifId);
                    return View(model);

                }
                else
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    model.Teklifleri = new List<TeklifOzelDetay>();
                    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult Policeleri(PotansiyelMusteriTeklifleriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.MusteriKodu);

                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");
                    //if (musteri == null)
                    //{
                    //    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    //    model.Teklifleri = new List<TeklifOzelDetay>();
                    //    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    //    return View(model);
                    //}

                    model.MusteriAdSoyad = potansiyelMusteri.AdiUnvan + " " + potansiyelMusteri.SoyadiUnvan;
                    model.Teklifleri = _TeklifService.GetMusteriPoliceleri(model.MusteriKodu, model.BaslangicTarihi, model.BitisTarihi, model.TeklifTarihi);

                    foreach (var item in model.Teklifleri)
                    {
                        item.OzelAlan = RaporController.GetOzelAlan(item.TeklifId);
                    }
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    return View(model);

                }
                else
                {
                    model.Teklifleri = new List<TeklifOzelDetay>();
                    model.TeklifTarihiTipleri = new SelectList(MusteriListProvider.GetTeklifTarihiTipleri(), "Value", "Text", "0").ToList();
                    ModelState.AddModelError("", "Müşteri kodu hatalı");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }


        //Müşteri Arama PArtial 
        [HttpPost]
        public ActionResult PotansiyelMusteriAraPartial(string Tip)
        {
            PotansiyelMusteriListeModel model = new PotansiyelMusteriListeModel();
            try
            {
                model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMUnvani = _AktifKullanici.TVMUnvani;
                model.Tip = Tip;

                List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
                model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
            return PartialView("_PotansiyelMusteriAraPartial", model);
        }

        [HttpPost]
        public ActionResult PotansiyelMusteriAraPartialModel(int MusteriId)
        {
            PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(MusteriId);
            if (musteri != null)
                return Json(new { tckn = musteri.KimlikNo, adi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan });
            else
                return null;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.PotansiyelMusteri, SekmeKodu = AltMenuSekmeler.AraGuncelle)]
        public ActionResult PotansiyelMusteriSil(int Id)
        {
            _MusteriService.DeletePotansiyelMusteri(Id);
            return Json(new { redirectToUrl = Url.Action("Liste", "Musteri") });
        }

        #endregion

        #region TelefonEkleme Ve Silme

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                      AltMenuKodu = AltMenuler.PotansiyelMusteri,
                      SekmeKodu = AltMenuSekmeler.AraGuncelle,
                      menuPermission = MenuPermission.Ekleme,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public PartialViewResult TelefonEkle(int musteriKodu)
        {
            PotansiyelMusteriTelefonModel model = new PotansiyelMusteriTelefonModel();
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);

                if (potansiyelMusteri == null) return null;

                model.PotansiyelMusteriKodu = musteriKodu;
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
            return PartialView("_TelefonEkle", model);
        }


        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                     AltMenuKodu = AltMenuler.PotansiyelMusteri,
                     SekmeKodu = AltMenuSekmeler.AraGuncelle,
                     menuPermission = MenuPermission.Ekleme,
                     menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public PartialViewResult TelefonEkle(PotansiyelMusteriTelefonModel model)
        {
            try
            {
                if (model.Numara != null)
                {
                    if (ModelState.IsValid)
                    {
                        PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);

                        if (potansiyelMusteri != null)
                        {
                            PotansiyelMusteriTelefon telefon = new PotansiyelMusteriTelefon();

                            telefon.IletisimNumaraTipi = model.IletisimNumaraTipi;
                            telefon.PotansiyelMusteriKodu = potansiyelMusteri.PotansiyelMusteriKodu;
                            telefon.Numara = model.Numara;
                            telefon.NumaraSahibi = model.NumaraSahibi;
                            model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                            _MusteriService.CreatePotansiyelTelefon(telefon);
                            return null;

                        }
                        else
                        {
                            //Yetkisiz Erişim
                            ModelState.AddModelError("", babonline.Access_Is_Denied);
                            model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                            return PartialView("_TelefonEkle", model);
                        }

                        //Mapper.CreateMap<PotansiyelMusteriTelefonModel, PotansiyelMusteriTelefon>();
                        //PotansiyelMusteriTelefon telefon = Mapper.Map<PotansiyelMusteriTelefon>(model);
                        //_MusteriService.CreatePotansiyelTelefon(telefon);


                    }
                    ModelState.AddModelError("", babonline.Message_PhoneSaveError);
                    model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                    return PartialView("_TelefonEkle", model);

                }
                //Hata ile Model geri dondürülüyor...
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                ModelState.AddModelError("", babonline.Message_PhoneSaveError);
                return PartialView("_TelefonEkle", model);


            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                return PartialView("_TelefonEkle", model);
            }

        }


        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public PartialViewResult TelefonGuncelle(int musteriKodu, int siraNo)
        {
            PotansiyelMusteriTelefonModel model = new PotansiyelMusteriTelefonModel();

            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);

                //Yetkisiz Erişim
                if (potansiyelMusteri == null) return null;

                PotansiyelMusteriTelefon telefon = _MusteriService.GetPotansiyelTelefon(musteriKodu, siraNo);
                model.sayfaadi = "guncelle";

                if (telefon != null)
                {
                    model.IletisimNumaraTipi = telefon.IletisimNumaraTipi;
                    model.PotansiyelMusteriKodu = telefon.PotansiyelMusteriKodu;
                    model.Numara = telefon.Numara;
                    model.NumaraSahibi = telefon.NumaraSahibi;
                    model.SiraNo = telefon.SiraNo;
                    model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", model.IletisimNumaraTipi);

                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return PartialView("_TelefonEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public PartialViewResult TelefonGuncelle(PotansiyelMusteriTelefonModel model)
        {
            try
            {
                if (model.Numara != null)
                {
                    if (ModelState.IsValid)
                    {
                        PotansiyelMusteriTelefon telefon = _MusteriService.GetPotansiyelTelefon(model.PotansiyelMusteriKodu, model.SiraNo);
                        PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);

                        //Yetkisiz Erişim
                        if (potansiyelMusteri == null)
                        {
                            model.sayfaadi = "guncelle";
                            ModelState.AddModelError("", babonline.Access_Is_Denied);
                            model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", model.IletisimNumaraTipi);

                            return PartialView("_TelefonEkle", model);
                        }
                        if (telefon != null)
                        {
                            telefon.Numara = model.Numara;
                            telefon.IletisimNumaraTipi = model.IletisimNumaraTipi;
                            telefon.NumaraSahibi = model.NumaraSahibi;

                            _MusteriService.UpdatePotansiyelTelefon(telefon);
                            return null;
                        }
                    }
                    //Hata ile Model geri dondürülüyor...
                    model.sayfaadi = "guncelle";
                    model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "Value", "Text", model.IletisimNumaraTipi);
                    ModelState.AddModelError("", babonline.Message_PhoneSaveError);

                }
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                ModelState.AddModelError("", babonline.Message_PhoneSaveError);
                return PartialView("_TelefonEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
            return PartialView("_TelefonEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult TelefonSil(int MusteriKodu, int SiraNo)
        {
            PotansiyelMusteriTelefonListModel model = new PotansiyelMusteriTelefonListModel();
            try
            {
                PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(MusteriKodu);

                if (musteri == null)
                    return null;

                _MusteriService.DeletePotansiyelTelefon(MusteriKodu, SiraNo);

                model.Items = PotansiyelMusteriTelefonlariGetir(MusteriKodu);
                model.sayfaAdi = "guncelle";
                return null;

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_Telefonlar", model);
            }

        }


        [AjaxException]
        public PartialViewResult TelefonlariDoldur(int musteriKodu, string sayfaAdi)
        {
            PotansiyelMusteriTelefonListModel model = new PotansiyelMusteriTelefonListModel();
            model.Items = new List<PotansiyelMusteriTelefonDetayModel>();

            if (musteriKodu > 0)
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);

                if (potansiyelMusteri != null)
                {
                    foreach (var item in potansiyelMusteri.PotansiyelMusteriTelefons)
                    {
                        PotansiyelMusteriTelefonDetayModel tel = new PotansiyelMusteriTelefonDetayModel();

                        tel.IletisimNumaraTipi = item.IletisimNumaraTipi;
                        tel.IletisimNumaraText = MusteriListProvider.GetNumaraTipiText(item.IletisimNumaraTipi);
                        tel.PotansiyelMusteriKodu = item.PotansiyelMusteriKodu;
                        tel.Numara = item.Numara;
                        tel.NumaraSahibi = item.NumaraSahibi;
                        tel.SiraNo = item.SiraNo;

                        model.Items.Add(tel);
                    }
                    model.sayfaAdi = sayfaAdi;
                    return PartialView("_Telefonlar", model);
                }
            }
            return null;
        }

        #endregion

        #region AdresEkleme Ve Silme


        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult AdresEkle(int musteriKodu)
        {
            try
            {
                PotansiyelAdresModel model = new PotansiyelAdresModel();
                model.PotansiyelMusteriKodu = musteriKodu;
                model.sayfaadi = "ekle";
                model.UlkeKodu = "TUR";
                model.AdresTipi = 8;

                AdresModelDoldur(model);

                return PartialView("_AdresEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }


        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult AdresEkle(PotansiyelAdresModel model)
        {
            TryValidateModel(model);

            if (model.AdresTipi != null && model.UlkeKodu != null)
            {

                if (ModelState.IsValid && model.PotansiyelMusteriKodu > 0)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);
                    //Yetkisiz Erişim Denetimi
                    if (potansiyelMusteri == null)
                    {
                        model.sayfaadi = "ekle";
                        AdresModelDoldur(model);
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_AdresEkle", model);
                    };
                    PotansiyelMusteriAdre adres = new PotansiyelMusteriAdre();

                    adres.PotansiyelMusteriKodu = potansiyelMusteri.PotansiyelMusteriKodu;
                    adres.AdresTipi = model.AdresTipi;
                    adres.UlkeKodu = model.UlkeKodu;
                    adres.IlKodu = model.IlKodu;
                    adres.IlceKodu = model.IlceKodu;
                    adres.Semt = model.Semt;
                    adres.HanAptFab = model.HanAptFab;
                    adres.Adres = model.Adres;
                    adres.Varsayilan = model.Varsayilan;
                    adres.Mahalle = model.Mahalle;
                    adres.Cadde = model.Cadde;
                    adres.Sokak = model.Sokak;
                    adres.Apartman = model.Apartman;
                    adres.DaireNo = model.DaireNo;
                    adres.BinaNo = model.BinaNo;
                    adres.PostaKodu = model.PostaKodu ?? 10001;
                    adres.Diger = model.Diger;

                    if (String.IsNullOrEmpty(adres.Adres))
                        adres.Adres = "";

                    _MusteriService.CreatePotansiyelMusteriAdres(adres);
                    return null;
                }
                return PartialView("_AdresEkle", model);
            }
            model.sayfaadi = "ekle";
            AdresModelDoldur(model);
            ModelState.AddModelError("", babonline.Message_RequiredValues);
            return PartialView("_AdresEkle", model);
        }

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                        AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult AdresGuncelle(int musteriKodu, int siraNo)
        {
            PotansiyelAdresModel model = new PotansiyelAdresModel();
            try
            {
                if (musteriKodu > 0)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);
                    //Yetki Denetimi
                    if (potansiyelMusteri == null)
                        return null;

                    PotansiyelMusteriAdre adres = _MusteriService.GetPotansiyelAdres(musteriKodu, siraNo);
                    if (adres == null) return null;

                    Mapper.CreateMap<PotansiyelMusteriAdre, PotansiyelAdresModel>();
                    model = Mapper.Map<PotansiyelAdresModel>(adres);
                    model.sayfaadi = "guncelle";

                    AdresModelDoldur(model);
                }
                else return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return PartialView("_AdresEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                        AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult AdresGuncelle(PotansiyelAdresModel model)
        {
            try
            {
                TryValidateModel(model);

                if (ModelState.IsValid && model.PotansiyelMusteriKodu > 0)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);
                    if (potansiyelMusteri == null)
                    {
                        model.sayfaadi = "guncelle";
                        AdresModelDoldur(model);
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_AdresEkle", model);
                    }
                    PotansiyelMusteriAdre adres = potansiyelMusteri.PotansiyelMusteriAdres.Where(s => s.SiraNo == model.SiraNo).FirstOrDefault();
                    if (adres == null)
                    {
                        model.sayfaadi = "guncelle";
                        AdresModelDoldur(model);
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_AdresEkle", model);
                    }

                    adres.AdresTipi = model.AdresTipi;
                    adres.UlkeKodu = model.UlkeKodu;
                    adres.IlKodu = model.IlKodu;
                    adres.IlceKodu = model.IlceKodu;
                    adres.Semt = model.Semt;
                    adres.HanAptFab = model.HanAptFab;
                    adres.Adres = model.Adres;
                    adres.Varsayilan = model.Varsayilan;
                    adres.Mahalle = model.Mahalle;
                    adres.Cadde = model.Cadde;
                    adres.Sokak = model.Sokak;
                    adres.Apartman = model.Apartman;
                    adres.DaireNo = model.DaireNo;
                    adres.BinaNo = model.BinaNo;
                    adres.PostaKodu = model.PostaKodu ?? 10001;
                    adres.Diger = model.Diger;


                    if (String.IsNullOrEmpty(adres.Adres))
                        adres.Adres = "";
                    _MusteriService.UpdatePotansiyelAdres(adres);
                    return null;
                }

                model.sayfaadi = "guncelle";
                AdresModelDoldur(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
            return PartialView("_AdresEkle", model);

        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public bool AdresSilKontrol(int MusteriKodu, int SiraNo)
        {
            if (!_MusteriService.GetPotansiyelAdres(MusteriKodu, SiraNo).Varsayilan == true)
                return true;
            else
                return false;
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult AdresSil(int MusteriKodu, int SiraNo)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Yetkili = "false", Basarili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);

                PotansiyelMusteriAdre adres = musteri.PotansiyelMusteriAdres.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (adres == null)
                    return Json(new { Yetkili = "true", Basarili = "false", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);

                bool sonuc = _MusteriService.DeletePotansiyelAdres(MusteriKodu, SiraNo);

                if (sonuc)
                    return Json(new { Yetkili = "true", Basarili = "true", Message = babonline.TheOperationWasSuccessful }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Yetkili = "true", Basarili = "false", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(new { Yetkili = "true", Basarili = "false", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult AdresleriDoldur(int musteriKodu, string sayfaAdi)
        {
            try
            {
                PotansiyelAdresListModel model = new PotansiyelAdresListModel();
                model.Items = PotansiyelMusteriAdresleriGetir(musteriKodu);
                model.sayfaAdi = sayfaAdi;

                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);
                if (potansiyelMusteri == null) return null;

                foreach (var item in model.Items)
                {
                    if (item.AdresTipi.HasValue && item.AdresTipi > 0)
                        item.AdresTipiText = MusteriListProvider.GetAdresTipiText((short)item.AdresTipi.Value);
                }

                return PartialView("_Adresler", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        private void AdresModelDoldur(PotansiyelAdresModel model)
        {
            try
            {
                //Ulkelerdolduruluyor....
                List<Ulke> ulkeler = _UlkeService.GetUlkeList();
                List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu).OrderBy(o => o.IlAdi).ToList<Il>();
                List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu).OrderBy(o => o.IlceAdi).ToList<Ilce>();
                List<GenelTanimlar> adresTipleri = _MeslekVeTanimService.GetListTanimlar("AdresTipi");
                model.AdresTipleri = new SelectList(MusteriListProvider.AdresTipleri(), "Value", "Text", model.AdresTipi).ToList();
                model.UlkeLer = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel();
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }


        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.Ekle,
                       menuPermission = MenuPermission.Gorme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult ParitusAdresDogrulama(string paritusAdres)
        {
            ParitusAdresModel model = new ParitusAdresModel();
            model.Durum = ParitusAdresSorgulamaDurum.Basarisiz;

            if (!String.IsNullOrEmpty(paritusAdres))
            {
                model = _MusteriService.GetParitusAdres(paritusAdres);

                if (!String.IsNullOrEmpty(model.IlKodu))
                {
                    List<Il> iller = _UlkeService.GetIlList("TUR").OrderBy(o => o.IlAdi).ToList<Il>();
                    List<Ilce> ilceler = _UlkeService.GetIlceList("TUR", model.IlKodu).OrderBy(o => o.IlceAdi).ToList<Ilce>();
                    model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                    model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "").ListWithOptionLabel();
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region DokumanEkleme Ve Silme


        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                      AltMenuKodu = AltMenuler.PotansiyelMusteri,
                      SekmeKodu = AltMenuSekmeler.AraGuncelle,
                      menuPermission = MenuPermission.Ekleme,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Upload(int musteriKodu)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);
                if (potansiyelMusteri == null) return null;

                PotansiyelDokumanModel model = new PotansiyelDokumanModel();
                model.sayfaadi = "ekle";
                model.PotansiyelMusteriKodu = musteriKodu;

                return PartialView("_DokumanEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }


        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Upload(PotansiyelDokumanModel model, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid && file.ContentLength > 0 && file != null && model.PotansiyelMusteriKodu > 0)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);
                    if (potansiyelMusteri == null)
                    {//Yetkisiz Erişim
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_DokumanEkle", model);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    string url = _Storage.UploadFile(model.PotansiyelMusteriKodu.ToString(), fileName, file.InputStream);


                    PotansiyelMusteriDokuman dokuman = new PotansiyelMusteriDokuman();
                    dokuman.PotansiyelMusteriKodu = model.PotansiyelMusteriKodu;

                    dokuman.TVMKodu = _AktifKullanici.TVMKodu;
                    dokuman.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                    dokuman.KayitTarihi = TurkeyDateTime.Now;

                    dokuman.DosyaAdi = fileName;
                    dokuman.DokumanTuru = model.DokumanTuru;
                    dokuman.DokumanURL = url;

                    _MusteriService.CreatePotansiyelDokuman(dokuman);
                    return null;
                }
                //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                //ModelState.AddModelError("", babonline.ThereWas_anError);
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }

        }

        //[HttpPost]
        //[AjaxException]
        //public bool DosyaKontrol(DokumanModel model, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid && file != null && file.ContentLength > 0)
        //    {
        //        string fileName = System.IO.Path.GetFileName(file.FileName);

        //        if (_MusteriService.CheckedFileName(fileName, model.MusteriKodu))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult DokumanSil(int MusteriKodu, int SiraNo)
        {
            PotansiyelDokumanListModel model = new PotansiyelDokumanListModel();
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(MusteriKodu);
                if (potansiyelMusteri == null)
                    return PartialView("_Dokumanlar", model);

                _MusteriService.DeletePotansiyelDokuman(MusteriKodu, SiraNo);
                model.Items = PotansiyelMusteriDokumanlariGetir(MusteriKodu);
                model.sayfaAdi = "guncelle";

                return PartialView("_Dokumanlar", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return PartialView("_Dokumanlar", model);
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanlariDoldur(int musteriKodu, string sayfaAdi)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);
                if (potansiyelMusteri == null) return null; //Yetkisiz Erişim

                PotansiyelDokumanListModel model = new PotansiyelDokumanListModel();
                model.Items = PotansiyelMusteriDokumanlariGetir(musteriKodu);
                model.sayfaAdi = sayfaAdi;

                return PartialView("_Dokumanlar", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        #endregion

        #region NotEkleme VE Silme

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult NotEkle(int MusteriKodu)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(MusteriKodu);
                if (potansiyelMusteri == null) return null;

                PotansiyelNotModel model = new PotansiyelNotModel();
                model.PotansiyelMusteriKodu = MusteriKodu;
                model.sayfaadi = "ekle";

                return PartialView("_NotEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult NotEkle(PotansiyelNotModel model)
        {
            try
            {
                if (ModelState.IsValid && model.PotansiyelMusteriKodu > 0)
                {
                    if (model.Konu != null && model.NotAciklamasi != null)
                    {
                        PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu);
                        if (potansiyelMusteri == null)
                        {
                            ModelState.AddModelError("", babonline.Access_Is_Denied);
                            return PartialView("_NotEkle", model);
                        }
                        PotansiyelMusteriNot not = new PotansiyelMusteriNot();
                        //Daha Sonra modelden alınacak
                        not.PotansiyelMusteriKodu = model.PotansiyelMusteriKodu;
                        not.Konu = model.Konu;
                        not.NotAciklamasi = model.NotAciklamasi;
                        not.TVMKodu = _AktifKullanici.TVMKodu;
                        not.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                        not.KayitTarihi = TurkeyDateTime.Now;

                        _MusteriService.CreatePotansiyelNot(not);
                        return null;
                    }
                }
                ModelState.AddModelError("", babonline.Message_NotSaveError);
                return PartialView("_NotEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return PartialView("_NotEkle", model);
            }
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.PotansiyelMusteri,
                       SekmeKodu = AltMenuSekmeler.AraGuncelle,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult NotSil(int MusteriKodu, int SiraNo)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Basarili = "false", Yetkili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);

                PotansiyelMusteriNot not = musteri.PotansiyelMusteriNots.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (not == null)
                    return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);

                _MusteriService.DeletePotansiyelNot(not.PotansiyelMusteriKodu, not.SiraNo);
                return Json(new { Basarili = "true", Yetkili = "true", Message = babonline.TheOperationWasSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxException]
        public ActionResult NotlariDoldur(int musteriKodu, string sayfaAdi)
        {
            try
            {
                PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(musteriKodu);
                if (potansiyelMusteri == null) return null;

                PotansiyelNotListModel model = new PotansiyelNotListModel();
                model.Items = PotansiyelMusteriNotlariGetir(musteriKodu);
                model.sayfaAdi = sayfaAdi;

                return PartialView("_Notlar", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        #endregion

        #region Müşteriye bağlı notlar adresler dokumanlar telefonlar  getiriliyor
        //Musteriye ait Tüm Adresleri Getirir.....
        public List<PotansiyelAdresModel> PotansiyelMusteriAdresleriGetir(int musteriKodu)
        {
            List<PotansiyelMusteriAdre> adresler = _MusteriService.GetPotansiyelMusteriAdresleri(musteriKodu);
            Mapper.CreateMap<PotansiyelMusteriAdre, PotansiyelAdresModel>();
            List<PotansiyelAdresModel> model = Mapper.Map<List<PotansiyelMusteriAdre>, List<PotansiyelAdresModel>>(adresler);

            foreach (var item in model)
            {
                if (item.UlkeKodu != null)
                    item.UlkeAdi = _UlkeService.GetUlkeAdi(item.UlkeKodu);

                if (item.IlKodu != null)
                    item.IlAdi = _UlkeService.GetIlAdi(item.UlkeKodu, item.IlKodu);

                if (item.IlceKodu.HasValue)
                    item.IlceAdi = _UlkeService.GetIlceAdi(item.IlceKodu.Value);

                if (item.AdresTipi.HasValue && item.AdresTipi > 0)
                    item.AdresTipiText = MusteriListProvider.GetAdresTipiText((short)item.AdresTipi.Value);
            }
            return model;
        }

        //Musteriye ait Tüm Telefonlari Getirir.. 
        public List<PotansiyelMusteriTelefonDetayModel> PotansiyelMusteriTelefonlariGetir(int musteriKodu)
        {
            List<PotansiyelMusteriTelefon> telefonlar = _MusteriService.GetPotansiyelMusteriTelefon(musteriKodu);
            Mapper.CreateMap<PotansiyelMusteriTelefon, PotansiyelMusteriTelefonDetayModel>();
            List<PotansiyelMusteriTelefonDetayModel> model = Mapper.Map<List<PotansiyelMusteriTelefon>, List<PotansiyelMusteriTelefonDetayModel>>(telefonlar);

            foreach (var item in model)
                item.IletisimNumaraText = MusteriListProvider.GetNumaraTipiText(item.IletisimNumaraTipi);

            return model;
        }

        //Musteriye ait Tüm Dokumnalari getirir
        public List<PotansiyelDokumanDetayModel> PotansiyelMusteriDokumanlariGetir(int musteriKodu)
        {
            List<PotansiyelDokumanDetayModel> model = _MusteriService.GetPotansiyelMusteriDokumanlari(musteriKodu);
            return model;
        }

        //Musterite ait Tüm Notlari getirir
        public List<PotansiyelNotModel> PotansiyelMusteriNotlariGetir(int musteriKodu)
        {
            List<PotansiyelNotModelDetay> notlar = _MusteriService.GetPotansiyelMusteriNotlari(musteriKodu);

            Mapper.CreateMap<PotansiyelNotModelDetay, PotansiyelNotModel>();
            List<PotansiyelNotModel> model = Mapper.Map<List<PotansiyelNotModelDetay>, List<PotansiyelNotModel>>(notlar);

            return model;
        }
        #endregion

        #region ListePager Methodları
        //Tableleri doldurmak icin kullanılan methodlar
        public ActionResult ListePagerTelefon(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<PotansiyelMusteriTelefon> telefonList = new DataTableParameters<PotansiyelMusteriTelefon>(Request, new Expression<Func<PotansiyelMusteriTelefon, object>>[]
                                                                                        {
                                                                                            t => t.IletisimNumaraTipi,
                                                                                            t => t.Numara,
                                                                                            t => t.NumaraSahibi

                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListPotansiyelTelefon(telefonList, id);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerAdres(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<PotansiyelMusteriAdre> adresList = new DataTableParameters<PotansiyelMusteriAdre>(Request, new Expression<Func<PotansiyelMusteriAdre, object>>[]
                                                                                        {
                                                                                            t=>t.Apartman,
                                                                                            t => t.Adres,
                                                                                            t => t.Cadde,
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListPotansiyelAdres(adresList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerDokuman(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<PotansiyelMusteriDokuman> dokumanList = new DataTableParameters<PotansiyelMusteriDokuman>(Request, new Expression<Func<PotansiyelMusteriDokuman, object>>[]
                                                                                        {
                                                                                            t => t.DosyaAdi,
                                                                                            t => t.DokumanTuru
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListPotansiyelDokuman(dokumanList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerNot(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<PotansiyelMusteriNot> notList = new DataTableParameters<PotansiyelMusteriNot>(Request, new Expression<Func<PotansiyelMusteriNot, object>>[]
                                                                                        {
                                                                                            t => t.Konu,
                                                                                            t => t.NotAciklamasi
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListPotansiyelNot(notList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        #endregion

        #region Ulke ve Şehirleri Getirir.

        public ActionResult IlleriGetir(string UlkeKodu)
        {
            List<Il> iller = _UlkeService.GetIlList(UlkeKodu);
            IlListModel model = new IlListModel();
            Mapper.CreateMap<Il, IlModel>();
            model.Items = Mapper.Map<List<Il>, List<IlModel>>(iller);

            return Json(new SelectList(model.Items, "IlKodu", "IlAdi").ListWithOptionLabelIller(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult IlceleriGetir(string UlkeKodu, string IlKodu)
        {
            List<Ilce> ilceler = _UlkeService.GetIlceList(UlkeKodu, IlKodu);
            IlceListModel model = new IlceListModel();
            Mapper.CreateMap<Ilce, IlceModel>();
            model.Items = Mapper.Map<List<Ilce>, List<IlceModel>>(ilceler);

            return Json(new SelectList(model.Items, "IlceKodu", "IlceAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Coklu Müşteri Ekle

        public ActionResult Excelaktar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Excelaktar(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // var path = Path.Combine(Server.MapPath("~/Content/Excel-PDF"), file.FileName);
                var path = Path.Combine(Server.MapPath("~/Files"), file.FileName);
                file.SaveAs(path);

                return RedirectToAction("ExcelKontrol", "Musteri", new { path = path });
            }
            ModelState.AddModelError("", "Lütfen uygun formatta bir dosya seçiniz");
            return View();
        }

        public ActionResult ExcelKontrol(string path)
        {
            MusteriADL adl = new MusteriADL(_AktifKullanici);
            ExcelMusteriListModel model = adl.ProcessFile(path);
            model.DosyaAdresi = path;

            return View(model);
        }

        [HttpPost]
        public ActionResult ExcelKontrol(ExcelMusteriListModel model)
        {

            if (!String.IsNullOrEmpty(model.DosyaAdresi))
            {
                MusteriADL adl = new MusteriADL(_AktifKullanici);
                bool sonuc = adl.MusterileriKaydet(model.DosyaAdresi);
                if (sonuc)
                    return RedirectToAction("Liste", "Musteri");
                else
                {
                    ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Dosya adresi bulunamadı..Lütfen tekrar deneyin.");
                return View(model);
            }
        }


        #endregion

        #region GET
        public ActionResult GetMusteriByTVMKodu(int tvmKodu)
        {
            List<PotansiyelMusteriFinderOzetModel> model = _MusteriService.GetPotansiyelMusteriListByTvmKodu(tvmKodu);
            return PartialView("_PotansiyelMusteriListe", model);
        }
        #endregion

        public vCardResult DownloadVcard(int id)
        {
            PotansiyelMusteriGenelBilgiler musteri = _MusteriService.GetPotansiyelMusteri(id);
            VCard card = new VCard();

            if (musteri != null)
            {
                card.FirstName = musteri.AdiUnvan;
                card.LastName = musteri.SoyadiUnvan;
                card.Email = musteri.EMail;
                card.HomePage = musteri.WebUrl;

                //Adres Bilgileri
                if (musteri.PotansiyelMusteriAdres.Count > 0)
                {
                    PotansiyelMusteriAdre adres = musteri.PotansiyelMusteriAdres.Where(s => s.Varsayilan == true).FirstOrDefault();
                    if (adres != null)
                    {
                        Il il = _UlkeService.GetIl(adres.UlkeKodu, adres.IlKodu);
                        if (il != null)
                            card.City = il.IlAdi;

                        card.StreetAddress = adres.Adres;
                        card.Zip = adres.PostaKodu.ToString();
                        card.Latitude = adres.Latitude;
                        card.Longitude = adres.Longitude;

                    }
                }

                //Meslek Bilgisi
                if (musteri.MeslekKodu.HasValue)
                {
                    Meslek meslek = _MusteriService.GetMeslek(musteri.MeslekKodu.Value);
                    if (meslek != null)
                        card.JobTitle = meslek.MeslekAdi;
                }

                //Telefon bilgileri
                if (musteri.PotansiyelMusteriTelefons.Count > 0)
                {
                    List<PotansiyelMusteriTelefon> telefonlar = musteri.PotansiyelMusteriTelefons.ToList<PotansiyelMusteriTelefon>();
                    PotansiyelMusteriTelefon cepTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault<PotansiyelMusteriTelefon>();
                    PotansiyelMusteriTelefon evTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault<PotansiyelMusteriTelefon>();
                    PotansiyelMusteriTelefon isTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Is).FirstOrDefault<PotansiyelMusteriTelefon>();


                    if (evTelefonu != null)
                        card.Phone = evTelefonu.Numara;
                    if (cepTelefonu != null)
                        card.Mobile = cepTelefonu.Numara;
                    if (isTelefonu != null)
                        card.BussinesPhone = isTelefonu.Numara;
                }
            }

            return new vCardResult(card);
        }

        #region Extra

        private void EkleModelDoldur(PotansiyelMusteriModel model)
        {
            model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
            model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", null);
            model.UyrukTipleri = new SelectList(MusteriListProvider.UyrukTipleri(), "Value", "Text", "1");
            model.MedeniDurumTipleri = new SelectList(MusteriListProvider.MedeniDurumTipleri(), "Value", "Text", "0");

            List<Meslek> meslekler = _MeslekVeTanimService.GetListMeslek();

            model.EgitimDurumlari = new SelectList(MusteriListProvider.EgitimDurumlari(), "Value", "Text", "0").ToList();
            model.Meslekler = new SelectList(meslekler, "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
            model.IletisimNumaraTipleri = new SelectList(MusteriListProvider.IletisimNumaraTipleri(), "Value", "Text", "0").ToList();

            AdresModelDoldur(model.PotansiyelMusteriAdresModel);
        }

        private void GuncelleModelDoldur(PotansiyelMusteriModel model)
        {
            //Musteriye eklenmiş olan tüm adres,dokuman,telefon ve notlari getirir..
            model.Adresleri = new PotansiyelAdresListModel();
            model.Telefonlari = new PotansiyelMusteriTelefonListModel();
            model.Dokumanlari = new PotansiyelDokumanListModel();
            model.Notlari = new PotansiyelNotListModel();
            model.Adresleri.sayfaAdi = "guncelle";
            model.Telefonlari.sayfaAdi = "guncelle";
            model.Dokumanlari.sayfaAdi = "guncelle";
            model.Notlari.sayfaAdi = "guncelle";
            model.Adresleri.Items = PotansiyelMusteriAdresleriGetir(model.PotansiyelMusteriGuncelleModel.PotansiyelMusteriKodu);
            model.Dokumanlari.Items = PotansiyelMusteriDokumanlariGetir(model.PotansiyelMusteriGuncelleModel.PotansiyelMusteriKodu);
            model.Telefonlari.Items = PotansiyelMusteriTelefonlariGetir(model.PotansiyelMusteriGuncelleModel.PotansiyelMusteriKodu);
            model.Notlari.Items = PotansiyelMusteriNotlariGetir(model.PotansiyelMusteriGuncelleModel.PotansiyelMusteriKodu);


            List<Meslek> meslekler = _MeslekVeTanimService.GetListMeslek().ToList<Meslek>();
            // model.AdresTipleri = new SelectList(MusteriListProvider.AdresTipleri(), "Value", "Text", model.AdresTipleri).ToList();
            model.EgitimDurumlari = new SelectList(MusteriListProvider.EgitimDurumlari(), "Value", "Text", model.PotansiyelMusteriGuncelleModel.EgitimDurumu).ToList();
            model.MedeniDurumTipleri = new SelectList(MusteriListProvider.MedeniDurumTipleri(), "Value", "Text", model.PotansiyelMusteriGuncelleModel.MedeniDurumu);
            model.Meslekler = new SelectList(meslekler, "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
            model.UyrukTipleri = new SelectList(MusteriListProvider.UyrukTipleri(), "Value", "Text", model.PotansiyelMusteriGuncelleModel.Uyruk);
            model.PotansiyelMusteriGuncelleModel.MusteriTipiText = MusteriListProvider.GetMusteriTipiText(model.PotansiyelMusteriGuncelleModel.MusteriTipKodu);
            //Bağlı olduğu tvmnin unvanı text olarak getiriliyor..
            model.PotansiyelMusteriGuncelleModel.TVMUnvani = _TVMService.GetTvmUnvan(model.PotansiyelMusteriGuncelleModel.TVMKodu.Value);
        }

        public bool TCKontrol(string TCKN, int TVMKodu)
        {
            if (_MusteriService.GetPotansiyelMusteri(TCKN, TVMKodu) != null)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}

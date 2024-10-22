using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;

namespace Neosinerji.BABOnlineTP.Web.Areas.Musteri.Controllers
{
    [IDAuthority(Type = "Musteri")]
    [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = 0, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class MusteriController : Controller
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
        IPoliceService _PoliceService;
        IAktifKullaniciService _AktifKullaniciService;
        IMusteriService _MusteriContext;

        public MusteriController(IMusteriService musteri,
                                 IUlkeService ulke,
                                 IMusteriDokumanStorage storage,
                                 ITanimService meslekVeTanim,
                                 ITVMService tvm,
                                 IKullaniciService kullanici,
                                 IAktifKullaniciService aktifKullanici,
                                 IAktifKullaniciService aktifKullaniciService,
                                 IKullaniciService kullaniciService,
                                 IPoliceService policeService,
                                 ITeklifService teklifService,
                                 IMusteriDokumanStorage musteriStorage)
        {
            _UlkeService = ulke;
            _MusteriService = musteri;
            _Storage = storage;
            _MeslekVeTanimService = meslekVeTanim;
            _TVMService = tvm;
            _KullaniciService = kullanici;
            _AktifKullanici = aktifKullanici;
            
            _TeklifService = teklifService;
            _MusteriStorage = musteriStorage;
            _PoliceService = policeService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }


        #region Views

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Liste(string teklifTipi)
        {
            try
            {
                MusteriListeModel model = new MusteriListeModel();
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
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }

        public ActionResult ListePager()
        {
            try
            {
                if (Request["sEcho"] != null)
                {
                    MusteriListe arama = new MusteriListe(Request, new Expression<Func<MusteriListeModelOzel, object>>[]
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

                    //var parts=  arama.KimlikNo.Split('-');
                    //  if (parts!=null)
                    //  {
                    //     var yasBaslangic= parts[0];
                    //    var yasBitis=  parts[1];
                    //      var dogBas = 0;
                    //      var dogBit = 0;

                    //      if (Convert.ToInt32(yasBaslangic)>0)
                    //      {
                    //           dogBas = DateTime.Now.AddYears(Convert.ToInt32(yasBaslangic) * -1).Year;
                    //           dogBit = DateTime.Now.AddYears(Convert.ToInt32(yasBitis) * -1).Year;
                    //      }
                    //      arama.yasBaslangic = dogBas;
                    //      arama.yasBitis = dogBit;
                    //  }

                    int totalRowCount = 0;
                    List<MusteriListeModelOzel> list = _MusteriService.PagedList(arama, out totalRowCount);

                    arama.LinkColumn1Url = "/Musteri/Musteri/Detay/";
                    arama.UpdateUrl = "/Musteri/Musteri/Guncelle/";
                    arama.RowIdColumn = a => a.MusteriKodu;

                    //string gorevGuncelleUrl = "/GorevTakip/GorevTakip/Guncelle";
                    string gorevEkleUrl = "/GorevTakip/GorevTakip/Ekle/";
                    arama.GorevUrl = gorevEkleUrl;

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
                        arama.AddFormatter(f => f.AdiUnvan, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.AdiUnvan));
                    }

                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            DataTableList resultnull = new DataTableList();
            return Json(resultnull, JsonRequestBehavior.AllowGet);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.MusteriListesi, SekmeKodu = 0)]
        public ActionResult MusteriListesi()
        {
            try
            {
                MusteriListesiModel model = new MusteriListesiModel();
                model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMUnvani = _AktifKullanici.TVMUnvani;
                model.Meslekler = new SelectList(_MeslekVeTanimService.GetListMeslek(), "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
                List<Meslek> meslekler = _MeslekVeTanimService.GetListMeslek();
                model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", null);
                model.YasGruplari = new SelectList(MusteriListProvider.YasGruplari(), "Value", "Text", null);
                


                List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
                model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();


                return View(model);
            }


            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.MusteriListesi, SekmeKodu = 0)]


        public ActionResult MusteriListesiGetir()
        {
            try
            {
                if (Request["sEcho"] != null)
                {
                    MusteriListesi arama = new MusteriListesi(Request, new Expression<Func<MusteriListesiModelOzel, object>>[]
                                                                    {
                                                                        t =>t.MusteriKodu,
                                                                        t =>t.MusteriGrupKodu,
                                                                        t =>t.MusteriTipiText,
                                                                        t =>t.AdiSoyadiUnvan,
                                                                        t =>t.EMail,
                                                                        t =>t.Cinsiyet,
                                                                        t =>t.DogumTarihi,
                                                                        t =>t.BagliOlduguTvmText,
                                                                        t =>t.MeslekKoduText,
                                                                        t =>t.KimlikNo,
                                                                        t =>t.EgitimDurumuText,
                                                                        t =>t.MedeniDurumuText,
                                                                        t =>t.CepTel,
                                                                        t =>t.EvTel,
                                                                        t =>t.KaydedenKullanici,
                                                                        t =>t.IlIlce

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
                    arama.Cinsiyet = arama.TryParseParamString("Cinsiyet");
                    arama.DogumTarihi = arama.TryParseParamDate("DogumTarihi");
                    arama.MeslekKodu = arama.TryParseParamInt("MeslekKodu");
                    arama.YasGrubu = arama.TryParseParamString("YasGrubu");

                    

                    var parts = arama.YasGrubu.Split('-');
                    if (parts != null && parts.Length > 1)
                    {
                        var yasBaslangic = parts[0];
                        var yasBitis = parts[1];
                        var dogBas = 0;
                        var dogBit = 0;

                        if (Convert.ToInt32(yasBaslangic) > 0)
                        {
                            dogBas = DateTime.Now.AddYears(Convert.ToInt32(yasBaslangic) * -1).Year;
                            dogBit = DateTime.Now.AddYears(Convert.ToInt32(yasBitis) * -1).Year;
                        }
                        else
                        {
                            dogBas = DateTime.Now.AddYears(Convert.ToInt32(yasBaslangic) * -1).Year;
                            dogBit = DateTime.Now.AddYears(Convert.ToInt32(yasBitis) * -1).Year;
                        }
                        arama.yasBaslangic = dogBas;
                        arama.yasBitis = dogBit;

                    }
                    else if (parts != null && parts.Length == 1)
                    {
                        var yasBaslangic = parts[0];
                        if (!String.IsNullOrEmpty(yasBaslangic))
                        {
                            arama.yasBaslangic = DateTime.Now.AddYears(Convert.ToInt32(yasBaslangic) * -1).Year;
                            arama.yasBitis = 0;
                        }
                      
                    }

                    int totalRowCount = 0;
                    List<MusteriListesiModelOzel> list = _MusteriService.PagedMusteriList(arama, out totalRowCount);


                    foreach (var item in list)
                    {
                        if (item.EgitimDurumu.HasValue)
                        {
                            item.EgitimDurumuText = MusteriListProvider.GetEgitimDurumText(item.EgitimDurumu.Value);
                        }
                        else item.EgitimDurumuText = "";

                        if (item.MedeniDurumu.HasValue)
                        {
                            item.MedeniDurumuText = MusteriListProvider.GetMedeniDurumText(item.MedeniDurumu.Value);
                        }
                        else item.MedeniDurumuText = "";

                        item.MusteriTipiText = MusteriListProvider.GetMusteriTipiText(item.MusteriTipKodu);
                        var tvmName = item.BagliOlduguTvmText;
                        var tvmFullName = item.BagliOlduguTvmText;
                        if (item.BagliOlduguTvmText.Length>20)
                        {
                            tvmName = item.BagliOlduguTvmText.Substring(0,20);
                        }

                        item.BagliOlduguTvmText = String.Format("<span title='{0}'>{1} </span>", item.BagliOlduguTvmText, tvmName);
                    }



                    

                    arama.LinkColumn1Url = "/Musteri/Musteri/Detay/";
                    arama.UpdateUrl = "/Musteri/Musteri/Guncelle/";
                    arama.RowIdColumn = a => a.MusteriKodu;

                    //string gorevGuncelleUrl = "/GorevTakip/GorevTakip/Guncelle";
                    string gorevEkleUrl = "/GorevTakip/GorevTakip/Ekle/";
                    arama.GorevUrl = gorevEkleUrl;

                    string teklifTipi = Request["TeklifTipi"];
                    if (!String.IsNullOrEmpty(teklifTipi))
                    {
                        string teklifUrl = String.Empty;
                        switch (teklifTipi)
                        {
                            case "trafik": teklifUrl = "/Teklif/Trafik/Ekle/"; break;
                        }

                        arama.AddFormatter(f => f.AdiSoyadiUnvan, f => String.Format("<a href='{0}{1}'  target='_blank'>{2}</a>", teklifUrl, f.MusteriKodu, f.AdiSoyadiUnvan));
                    }
                    else
                    {
                        arama.AddFormatter(f => f.AdiSoyadiUnvan, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'  target='_blank'>{1}</a>", f.MusteriKodu, f.AdiSoyadiUnvan));
                    }

                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            DataTableList resultnull = new DataTableList();
            return Json(resultnull, JsonRequestBehavior.AllowGet);
        }



        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.MusteriAdedi, SekmeKodu = 0)]
        public ActionResult MusteriAdedi()
        {
            try
            {
                MusteriAdediModel model = new MusteriAdediModel();

                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMUnvani = _AktifKullanici.TVMUnvani;


                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");


                model.TCMusteriCountToplam = 0;
                
                return View(model);
            }


            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.MusteriAdedi, SekmeKodu = 0)]


        public ActionResult MusteriAdediGetir()
        {





            try
            {
                if (Request["sEcho"] != null)
                {
                    MusteriAdedi arama = new MusteriAdedi(Request, new Expression<Func<MusteriAdediModelOzel, object>>[]
                                                                    {
                                                                        t =>t.tvmUnvani,
                                                                        t =>t.TCMusteriCountText,
                                                                        t =>t.TuzelMusteriCountText,
                                                                        t =>t.SahisFirmasiCountText,
                                                                        t =>t.YabanciMusteriCountText,
                                                                        t =>t.MusteriToplamCountText,
                                                                    });

                    arama.TVMKodlari = arama.TryParseParamString("TVMKodu");

                    int totalRowCount = 0;

                    List<MusteriAdediModelOzel> list = _MusteriService.PagedMusteriAdetList(arama);
                    
                    foreach (var item in list)
                    {                        
                        var tvmName = item.tvmUnvani;
                        var tvmFullName = item.tvmUnvani;
                        item.MusteriToplamCountText = item.MusteriToplamCount.ToString("N2");
                        item.TCMusteriCountText = item.TCMusteriCount.ToString("N2");
                        item.TuzelMusteriCountText = item.TuzelMusteriCount.ToString("N2");
                        item.YabanciMusteriCountText = item.YabanciMusteriCount.ToString("N2");
                        item.SahisFirmasiCountText = item.SahisFirmasiCount.ToString("N2");
                      
                        

                        if (item.tvmUnvani.Length > 20)
                        {
                            tvmName = item.tvmUnvani.Substring(0, 20);
                        }
                      
                        item.tvmUnvani = String.Format("<span title='{0}'>{1} </span>", item.tvmUnvani, tvmName);
                    }
                   

                    totalRowCount = list.Count;
                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            DataTableList resultnull = new DataTableList();
            return Json(resultnull, JsonRequestBehavior.AllowGet);
        }


        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.Musterilerim, SekmeKodu = 0)]
        public ActionResult Musterilerim()
        {
            try
            {
                MusteriListeModelHarita model = new MusteriListeModelHarita();
                model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
                model.MusteriSayilar = new SelectList(MusteriListProvider.MusteriSayilari(), "Value", "Text", "1");
                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMUnvani = _AktifKullanici.TVMUnvani;

                List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
                model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }


       

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Detay(int id)
        {
            try
            {
                MusteriModel model = new MusteriModel();
                //Müşteriye yetki kontrolu
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(id);
                if (musteri == null)
                    return new RedirectResult("~/Error/ErrorPage/403");
                if (_AktifKullanici.ProjeKodu != TVMProjeKodlari.Lilyum)
                {
                    ViewBag.MusTeklifMenuGor = true;
                    ViewBag.MusPoliceMenuGor = true;
                }
                else
                {
                    ViewBag.MusTeklifMenuGor = false;
                    ViewBag.MusPoliceMenuGor = false;
                }

                Mapper.CreateMap<MusteriGenelBilgiler, GenelBilgilerModel>();

                model.GenelBilgiler = Mapper.Map<MusteriGenelBilgiler, GenelBilgilerModel>(musteri);
                model.GenelBilgiler.TVMUnvani = _TVMService.GetTvmUnvan(musteri.TVMKodu);

                if (model.GenelBilgiler.KimlikNo == "1AEGONAEGON")
                    model.GenelBilgiler.KimlikNo = "";


                //Müşteri Tİpinin Adı  ve medeni durumu text olarak alınıyor..
                model.GenelBilgiler.MusteriTipiText = MusteriListProvider.GetMusteriTipiText(model.GenelBilgiler.MusteriTipKodu);

                if (model.GenelBilgiler.MedeniDurumu.HasValue && model.GenelBilgiler.MedeniDurumu.Value > 0)
                    model.GenelBilgiler.MedeniDurumText = MusteriListProvider.GetMedeniDurumText(model.GenelBilgiler.MedeniDurumu.Value);

                if (model.GenelBilgiler.EgitimDurumu.HasValue && model.GenelBilgiler.EgitimDurumu.Value > 0)
                {
                    GenelTanimlar egitimDurumu = _MeslekVeTanimService.GetTanim("Egitim", model.GenelBilgiler.EgitimDurumu.ToString());
                    if (egitimDurumu != null)
                        model.GenelBilgiler.EgitimDurumuText = egitimDurumu.Aciklama;
                }

                if (model.GenelBilgiler.MeslekKodu.HasValue && model.GenelBilgiler.MeslekKodu.Value > 0)
                {
                    Meslek meslek = _MeslekVeTanimService.GetMeslek(model.GenelBilgiler.MeslekKodu.Value);
                    model.GenelBilgiler.MeslekKoduText = meslek.MeslekAdi;
                }

                model.Adresleri = new AdresListModel();
                model.Telefonlari = new MusteriTelefonListModel();
                model.Dokumanlari = new DokumanListModel();
                model.Notlari = new NotListModel();

                model.Adresleri.sayfaAdi = "detay";
                model.Telefonlari.sayfaAdi = "detay";
                model.Dokumanlari.sayfaAdi = "detay";
                model.Notlari.sayfaAdi = "detay";
                model.Adresleri.Items = MusteriAdresleriGetir(musteri.MusteriKodu);
                model.Dokumanlari.Items = MusteriDokumanlariGetir(musteri.MusteriKodu);
                model.Telefonlari.Items = MusteriTelefonlariGetir(musteri.MusteriKodu);
                model.Notlari.Items = MusteriNotlariGetir(musteri.MusteriKodu);

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
                       AltMenuKodu = AltMenuler.MusteriEkle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Ekle(int? id)
        {
            try
            {
                MusteriModel model = new MusteriModel();

                if (id.HasValue)
                {
                    PotansiyelMusteriGenelBilgiler potansiyelMusteri = _MusteriService.GetPotansiyelMusteri(id.Value);

                    if (potansiyelMusteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    model = PotansiyelMusteriAktar(potansiyelMusteri);
                    return View(model);
                }

                model.MusteriTelefonModel = new MusteriTelefonModel();
                model.MusteriAdresModel = new AdresModel();
                model.GenelBilgiler = new GenelBilgilerModel();
                model.GenelBilgiler.TVMKodu = _AktifKullanici.TVMKodu;
                model.GenelBilgiler.TVMUnvani = _AktifKullanici.TVMUnvani;

                model.MusteriAdresModel.UlkeKodu = "TUR";
                model.MusteriAdresModel.IlKodu = "34";

                model.MusteriTelefonModel.IletisimNumaraTipi = 1;
                model.GenelBilgiler.MusteriTipKodu = 1;
                model.GenelBilgiler.PasaportGecerlilikBitisTarihi = TurkeyDateTime.Today;

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
                       AltMenuKodu = AltMenuler.MusteriEkle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Ekle(MusteriModel model)
        {
            try
            {
                TryValidateModel(model);

                if (model != null)
                {
                    if (!String.IsNullOrEmpty(model.MusteriAdresModel.Cadde))
                        if (ModelState["MusteriAdresModel.Sokak"] != null)
                            ModelState["MusteriAdresModel.Sokak"].Errors.Clear();

                    if (!String.IsNullOrEmpty(model.MusteriAdresModel.Sokak))
                        if (ModelState["MusteriAdresModel.Cadde"] != null)
                            ModelState["MusteriAdresModel.Cadde"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    //Kimlik Numarası Kaydedilmişse hata dondürülüyor...
                    if (!TCKontrol(model.GenelBilgiler.KimlikNo, model.GenelBilgiler.TVMKodu.Value))
                    {
                        EkleModelDoldur(model);
                        ModelState.AddModelError("", babonline.Message_TCKN_AlreadyExist);
                        return View(model);
                    }

                    //Her Müşteri tipi için zorunlu olan alanlar ekleniyor (automapper kullanılmadan).....
                    MusteriGenelBilgiler musteri = new MusteriGenelBilgiler();
                    musteri.AdiUnvan = model.GenelBilgiler.AdiUnvan;
                    musteri.SoyadiUnvan = model.GenelBilgiler.SoyadiUnvan;
                    musteri.KimlikNo = model.GenelBilgiler.KimlikNo;
                    musteri.MusteriTipKodu = model.GenelBilgiler.MusteriTipKodu;
                    musteri.WebUrl = model.GenelBilgiler.WebUrl;
                    musteri.EMail = model.GenelBilgiler.EMail;
                    musteri.Uyruk = model.GenelBilgiler.Uyruk;
                    musteri.MuhasebeKodu = model.GenelBilgiler.MuhasebeKodu;
                    musteri.TVMMusteriKodu = model.GenelBilgiler.TVMMusteriKodu;

                    if (model.GenelBilgiler.TVMKodu.HasValue)
                    {
                        if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(model.GenelBilgiler.TVMKodu.Value))
                            musteri.TVMKodu = model.GenelBilgiler.TVMKodu.Value;
                        else
                            return new RedirectResult("~/Error/ErrorPage/403");
                    }
                    else
                    {
                        //Model valid değilse değerler dolduruluyor...Geri DOndürülücek.....
                        EkleModelDoldur(model);
                        ModelState.AddModelError("", babonline.Message_RequiredValues);
                        return View(model);
                    }


                    musteri.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;


                    //Müşteri Tip kodu bir şekilde 0 gelirse model hata ile birlikte geri döndürülüyor...
                    if (model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.Yok)
                    {
                        ModelState.AddModelError("", "Müşteri Tip Kodu Belirtiniz..");
                        EkleModelDoldur(model);
                        return View(model);
                    }

                    //Tüzel Müşteri ve sahıs vergi dairesini girmemişse hata ile model geri döndürülüyor...
                    if (model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.TuzelMusteri || model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                    {
                        if (String.IsNullOrEmpty(model.GenelBilgiler.VergiDairesi))
                        {
                            ModelState.AddModelError("", babonline.Message_TaxOfficeRequired);
                            EkleModelDoldur(model);
                            return View(model);
                        }
                        else
                            musteri.VergiDairesi = model.GenelBilgiler.VergiDairesi;
                    }

                    //if (model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                    //{
                    //    if (!String.IsNullOrEmpty(model.GenelBilgiler.FaaliyetGosterdigiAnaSektor) &
                    //        !String.IsNullOrEmpty(model.GenelBilgiler.FaaliyetGosterdigiAltSektor) &
                    //        !String.IsNullOrEmpty(model.GenelBilgiler.FaaliyetOlcegi_) &
                    //        !String.IsNullOrEmpty(model.GenelBilgiler.SabitVarlikBilgisi) &
                    //        !String.IsNullOrEmpty(model.GenelBilgiler.CiroBilgisi))
                    //    {
                    //        musteri.FaaliyetGosterdigiAnaSektor = model.GenelBilgiler.FaaliyetGosterdigiAnaSektor;
                    //        musteri.FaaliyetGosterdigiAltSektor = model.GenelBilgiler.FaaliyetGosterdigiAltSektor;
                    //        musteri.FaaliyetOlcegi_ = model.GenelBilgiler.FaaliyetOlcegi_;
                    //        musteri.SabitVarlikBilgisi = model.GenelBilgiler.SabitVarlikBilgisi;
                    //        musteri.CiroBilgisi = model.GenelBilgiler.CiroBilgisi;
                    //    }
                    //    else
                    //    {
                    //        ModelState.AddModelError("", babonline.Message_CustomerSaveError);
                    //        EkleModelDoldur(model);
                    //        return View(model);
                    //    }
                    //}

                    //Yabancı Müşteri için Ekstra olarak pasaport alanlari kaydediliyor diğerleri için null .....
                    if (model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.YabanciMusteri && model.GenelBilgiler.PasaportNo != "")
                    {
                        musteri.PasaportNo = model.GenelBilgiler.PasaportNo;
                        musteri.PasaportGecerlilikBitisTarihi = model.GenelBilgiler.PasaportGecerlilikBitisTarihi;
                    }

                    //TC Müşteri için eklenicek veriler...
                    if (model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.TCMusteri || model.GenelBilgiler.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                    {
                        musteri.DogumTarihi = model.GenelBilgiler.DogumTarihi;
                        musteri.Cinsiyet = model.GenelBilgiler.Cinsiyet;
                        musteri.EgitimDurumu = model.GenelBilgiler.EgitimDurumu;
                        musteri.MedeniDurumu = model.GenelBilgiler.MedeniDurumu;
                        musteri.MeslekKodu = model.GenelBilgiler.MeslekKodu;
                    }

                    //Müsteri tip kodu kaydediliyor...
                    musteri.MusteriTipKodu = model.GenelBilgiler.MusteriTipKodu;

                    //Telefon Bilgileri telefon tablosuna ekleniyor..
                    Mapper.CreateMap<MusteriTelefonModel, MusteriTelefon>();
                    MusteriTelefon telefon = Mapper.Map<MusteriTelefon>(model.MusteriTelefonModel);

                    //Adres Bilgileri adres tablosuna ekleniyor....
                    Mapper.CreateMap<AdresModel, MusteriAdre>();
                    MusteriAdre adres = Mapper.Map<MusteriAdre>(model.MusteriAdresModel);

                    if (!String.IsNullOrEmpty(model.MusteriAdresModel.Cadde))
                        if (String.IsNullOrEmpty(model.MusteriAdresModel.Sokak))
                            adres.Sokak = "";

                    if (!String.IsNullOrEmpty(model.MusteriAdresModel.Sokak))
                        if (String.IsNullOrEmpty(model.MusteriAdresModel.Cadde))
                            adres.Cadde = "";

                    if (String.IsNullOrEmpty(model.MusteriAdresModel.Adres))
                    {
                        if (!String.IsNullOrEmpty(model.MusteriAdresModel.Mahalle))
                            adres.Adres = model.MusteriAdresModel.Mahalle + " Mah.";

                        if (!String.IsNullOrEmpty(model.MusteriAdresModel.Cadde))
                            adres.Adres += model.MusteriAdresModel.Cadde + " Cad.";

                        if (!String.IsNullOrEmpty(model.MusteriAdresModel.Sokak))
                            adres.Adres += model.MusteriAdresModel.Mahalle + " SK.";

                        if (!String.IsNullOrEmpty(model.MusteriAdresModel.DaireNo) && !String.IsNullOrEmpty(model.MusteriAdresModel.BinaNo))
                            adres.Adres += model.MusteriAdresModel.BinaNo + " / " + model.MusteriAdresModel.BinaNo;
                    }

                    musteri = _MusteriService.CreateMusteri(musteri, adres, telefon);

                    if (model.PotansiyelMi && model.PotansiyelMusteriKodu.HasValue)
                    {
                        PotansiyelMusteriGenelBilgiler potansiyel = _MusteriService.GetPotansiyelMusteri(model.PotansiyelMusteriKodu.Value);
                        if (potansiyel != null)
                        {
                            List<PotansiyelMusteriDokuman> dokumanlari = potansiyel.PotansiyelMusteriDokumen.ToList<PotansiyelMusteriDokuman>();

                            foreach (var item in dokumanlari)
                            {
                                MusteriDokuman dokuman = new MusteriDokuman();
                                dokuman.DokumanTuru = item.DokumanTuru;
                                dokuman.DokumanURL = item.DokumanURL;
                                dokuman.DosyaAdi = item.DosyaAdi;
                                dokuman.MusteriKodu = musteri.MusteriKodu;
                                dokuman.TVMKodu = _AktifKullanici.TVMKodu;
                                dokuman.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                                _MusteriService.CreateDokuman(dokuman);
                            }

                            List<PotansiyelMusteriNot> notlari = potansiyel.PotansiyelMusteriNots.ToList<PotansiyelMusteriNot>();

                            foreach (var item in notlari)
                            {
                                MusteriNot not = new MusteriNot();
                                not.Konu = item.Konu;
                                not.MusteriKodu = musteri.MusteriKodu;
                                not.NotAciklamasi = item.NotAciklamasi;
                                not.TVMKodu = _AktifKullanici.TVMKodu;
                                not.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                                _MusteriService.CreateNot(not);
                            }
                        }
                    }

                    return RedirectToAction("Detay", "Musteri", new { Id = musteri.MusteriKodu });
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Guncelle(int Id)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(Id);

                if (musteri == null)
                    return new RedirectResult("~/Error/ErrorPage/403");
                if (_AktifKullanici.ProjeKodu != TVMProjeKodlari.Lilyum)
                {
                    ViewBag.MusTeklifMenuGor = true;
                    ViewBag.MusPoliceMenuGor = true;
                }
                else
                {
                    ViewBag.MusTeklifMenuGor = false;
                    ViewBag.MusPoliceMenuGor = false;
                }

                MusteriModel model = new MusteriModel();
                Mapper.CreateMap<MusteriGenelBilgiler, GuncelleModel>();
                model.MusteriGuncelleModel = Mapper.Map<MusteriGenelBilgiler, GuncelleModel>(musteri);

                if (model.MusteriGuncelleModel.KimlikNo == "1AEGONAEGON")
                    model.MusteriGuncelleModel.KimlikNo = "";

                model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", musteri.Cinsiyet);
                if (musteri.Cinsiyet == null)
                    model.MusteriGuncelleModel.Cinsiyet = "E";

                if (musteri.MusteriTipKodu != 4)
                    model.MusteriGuncelleModel.PasaportGecerlilikBitisTarihi = TurkeyDateTime.Today;

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
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Guncelle(MusteriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriGuncelleModel.MusteriKodu);

                    var pset = _PoliceService.PoliceOffLineTcVknSigortaEttiren(musteri.KimlikNo, musteri.TVMKodu);
                    List<PoliceSigortaEttiren> pseModel = new List<PoliceSigortaEttiren>();
                    if (pset.OfflineSigortaEttiren != null)
                    {
                        foreach (var item in pset.OfflineSigortaEttiren)
                        {

                            item.MusteriGrupKodu = model.MusteriGuncelleModel.TVMMusteriKodu;
                            _PoliceService.UpdatePoliceSigortaEttiren(item);

                        }
                    }

                    if (musteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    musteri.AdiUnvan = model.MusteriGuncelleModel.AdiUnvan;
                    musteri.SoyadiUnvan = model.MusteriGuncelleModel.SoyadiUnvan;
                    musteri.KimlikNo = model.MusteriGuncelleModel.KimlikNo;
                    musteri.MusteriTipKodu = model.MusteriGuncelleModel.MusteriTipKodu;
                    musteri.WebUrl = model.MusteriGuncelleModel.WebUrl;
                    musteri.EMail = model.MusteriGuncelleModel.EMail;
                    musteri.Uyruk = model.MusteriGuncelleModel.Uyruk;
                    musteri.MuhasebeKodu = model.MusteriGuncelleModel.MuhasebeKodu;
                    musteri.TVMMusteriKodu = model.MusteriGuncelleModel.TVMMusteriKodu;

                    //Tüzel Müşteri ve sahıs vergi dairesini girmemişse hata ile model geri döndürülüyor...
                    if (musteri.MusteriTipKodu == MusteriTipleri.TuzelMusteri || musteri.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                    {
                        if (model.MusteriGuncelleModel.VergiDairesi == "0")
                        {
                            ModelState.AddModelError("", babonline.Message_TaxOfficeRequired);
                            EkleModelDoldur(model);
                            return View(model);
                        }
                        else
                            musteri.VergiDairesi = model.MusteriGuncelleModel.VergiDairesi;
                    }

                    //if (model.MusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                    //{
                    //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetGosterdigiAnaSektor) &
                    //        !String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetGosterdigiAltSektor) &
                    //        !String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetOlcegi_) &
                    //        !String.IsNullOrEmpty(model.MusteriGuncelleModel.SabitVarlikBilgisi) &
                    //        !String.IsNullOrEmpty(model.MusteriGuncelleModel.CiroBilgisi))
                    //    {
                    //        musteri.FaaliyetGosterdigiAnaSektor = model.MusteriGuncelleModel.FaaliyetGosterdigiAnaSektor;
                    //        musteri.FaaliyetGosterdigiAltSektor = model.MusteriGuncelleModel.FaaliyetGosterdigiAltSektor;
                    //        musteri.FaaliyetOlcegi_ = model.MusteriGuncelleModel.FaaliyetOlcegi_;
                    //        musteri.SabitVarlikBilgisi = model.MusteriGuncelleModel.SabitVarlikBilgisi;
                    //        musteri.CiroBilgisi = model.MusteriGuncelleModel.CiroBilgisi;
                    //    }
                    //    else
                    //    {
                    //        ModelState.AddModelError("", babonline.Message_CustomerSaveError);
                    //        EkleModelDoldur(model);
                    //        return View(model);
                    //    }
                    //}

                    //Yabancı Müşteri için Ekstra olarak pasaport alanlari kaydediliyor diğerleri için null .....
                    if (musteri.MusteriTipKodu == MusteriTipleri.YabanciMusteri && musteri.PasaportNo != null)
                    {
                        musteri.PasaportNo = model.MusteriGuncelleModel.PasaportNo;
                        musteri.PasaportGecerlilikBitisTarihi = model.MusteriGuncelleModel.PasaportGecerlilikBitisTarihi;
                    }

                    //TC Müşteri için eklenicek veriler...
                    if (musteri.MusteriTipKodu == MusteriTipleri.TCMusteri || musteri.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                    {
                        musteri.DogumTarihi = model.MusteriGuncelleModel.DogumTarihi;
                        musteri.Cinsiyet = model.MusteriGuncelleModel.Cinsiyet;
                        musteri.EgitimDurumu = model.MusteriGuncelleModel.EgitimDurumu;
                        musteri.MedeniDurumu = model.MusteriGuncelleModel.MedeniDurumu;
                        musteri.MeslekKodu = model.MusteriGuncelleModel.MeslekKodu;
                    }

                    _MusteriService.UpdateMusteri(musteri);
                    return RedirectToAction("Detay", "Musteri", new { Id = musteri.MusteriKodu });
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

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Teklifleri(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(Id);

                    //Yetkisiz Erişim
                    if (musteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                    model.BitisTarihi = TurkeyDateTime.Now;

                    if (musteri == null)
                    {
                        ModelState.AddModelError("", "Müşteri kodu hatalı");
                        model.Teklifleri = new List<TeklifOzelDetay>();
                        return View(model);
                    }

                    model.MusteriKodu = Id;
                    model.MusteriAdSoyad = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
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
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Teklifleri(MusteriTeklifleriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    //Yetkisiz Erişim
                    if (musteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    model.MusteriAdSoyad = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
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

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Policeleri(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    MusteriTeklifleriModel model = new MusteriTeklifleriModel();
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(Id);

                    //Yetkisiz Erişim
                    if (musteri == null)
                        return new RedirectResult("~/Error/Errorpage/403");

                    model.BaslangicTarihi = TurkeyDateTime.Now;
                    model.BitisTarihi = TurkeyDateTime.Now.AddDays(1);

                    if (musteri == null)
                    {
                        ModelState.AddModelError("", "Müşteri kodu hatalı");
                        model.Teklifleri = new List<TeklifOzelDetay>();
                        return View(model);
                    }
                    model.MusteriKodu = Id;
                    model.MusteriAdSoyad = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
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
                    ModelState.AddModelError("", babonline.Message_RequiredValues);
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
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        public ActionResult Policeleri(MusteriTeklifleriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    if (musteri == null)
                        return new RedirectResult("~/Error/ErrorPage/403");

                    model.MusteriAdSoyad = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
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
                    ModelState.AddModelError("", babonline.Message_RequiredValues);
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
        public ActionResult MusteriAraPartial(string Tip)
        {
            MusteriListeModel model = new MusteriListeModel();

            try
            {
                List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();

                model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");

                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMUnvani = _AktifKullanici.TVMUnvani;
                model.Tip = Tip;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return PartialView("_MusteriAraPartial", model);
        }

        [HttpPost]
        public ActionResult MusteriAraPartialModel(int MusteriId)
        {
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriId);
            if (musteri != null)
                return Json(new { tckn = musteri.KimlikNo, adi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan });
            else
                return null;
        }

        //Müşteri silme aktif değil
        //[Authorization(AnaMenuKodu = AnaMenuler.Musteri, AltMenuKodu = AltMenuler.AraGuncelle, SekmeKodu = 0)]
        //public ActionResult MusteriSil(int Id)
        //{

        //    _MusteriService.DeleteMusteri(Id);
        //    return Json(new { redirectToUrl = Url.Action("Liste", "Musteri") });
        //}

        #endregion

        #region TelefonEkleme Ve Silme

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public PartialViewResult TelefonEkle(int musteriKodu)
        {
            MusteriTelefonModel model = new MusteriTelefonModel();

            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

                if (musteri == null) return null;

                model.MusteriKodu = musteriKodu;
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public PartialViewResult TelefonEkle(MusteriTelefonModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    if (musteri != null)
                    {
                        MusteriTelefon telefon = new MusteriTelefon();

                        telefon.IletisimNumaraTipi = model.IletisimNumaraTipi;
                        telefon.MusteriKodu = model.MusteriKodu;
                        telefon.Numara = model.Numara;
                        telefon.NumaraSahibi = model.NumaraSahibi;

                        _MusteriService.CreateTelefon(telefon);
                        return null;
                    }
                    else
                    {
                        //Yetkisiz Erişim
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
                        return PartialView("_TelefonEkle", model);
                    }
                }

                //Hata ile Model geri dondürülüyor...
                ModelState.AddModelError("", babonline.Message_PhoneSaveError);
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", "0");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return PartialView("_TelefonEkle", model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public PartialViewResult TelefonGuncelle(int musteriKodu, int siraNo)
        {
            MusteriTelefonModel model = new MusteriTelefonModel();

            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

                //Yetkisiz Erişim
                if (musteri == null) return null;

                MusteriTelefon telefon = musteri.MusteriTelefons.Where(s => s.SiraNo == siraNo).FirstOrDefault();

                //Yetkisiz Erişim
                if (telefon == null) return null;

                model.IletisimNumaraTipi = telefon.IletisimNumaraTipi;
                model.MusteriKodu = telefon.MusteriKodu;
                model.Numara = telefon.Numara;
                model.NumaraSahibi = telefon.NumaraSahibi;
                model.SiraNo = telefon.SiraNo;
                model.sayfaadi = "guncelle";

                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", model.IletisimNumaraTipi);
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public PartialViewResult TelefonGuncelle(MusteriTelefonModel model)
        {
            try
            {
                if (ModelState.IsValid && model.MusteriKodu > 0)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    //Yetkisiz Erişim
                    if (musteri == null)
                    {
                        model.sayfaadi = "guncelle";
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", model.IletisimNumaraTipi);

                        return PartialView("_TelefonEkle", model);
                    }

                    MusteriTelefon telefon = musteri.MusteriTelefons.Where(s => s.SiraNo == model.SiraNo).FirstOrDefault();

                    telefon.NumaraSahibi = model.NumaraSahibi;
                    telefon.Numara = model.Numara;
                    telefon.IletisimNumaraTipi = model.IletisimNumaraTipi;

                    _MusteriService.UpdateTelefon(telefon);

                    return null;
                }

                //Hata ile Model geri dondürülüyor...
                model.sayfaadi = "guncelle";
                ModelState.AddModelError("", babonline.Message_PhoneSaveError);
                model.IletisimNumaraTipleri = new SelectList(_MeslekVeTanimService.GetListTanimlar("TelefonTipi"), "TanimId", "Aciklama", model.IletisimNumaraTipi);
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult TelefonSil(int MusteriKodu, int SiraNo)
        {
            MusteriTelefonListModel model = new MusteriTelefonListModel();

            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);

                if (musteri == null)
                    return Json(babonline.Access_Is_Denied, JsonRequestBehavior.AllowGet);

                _MusteriService.DeleteTelefon(MusteriKodu, SiraNo);

                model.Items = MusteriTelefonlariGetir(MusteriKodu);
                model.sayfaAdi = "guncelle";

                return Json(babonline.TheOperationWasSuccessful, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(babonline.AnErrorHasOccurred, JsonRequestBehavior.AllowGet);
            }
        }

        [AjaxException]
        public PartialViewResult TelefonlariDoldur(int musteriKodu, string sayfaAdi)
        {
            MusteriTelefonListModel model = new MusteriTelefonListModel();
            model.Items = new List<MusteriTelefonDetayModel>();

            if (musteriKodu > 0)
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

                if (musteri != null)
                {
                    foreach (var item in musteri.MusteriTelefons)
                    {
                        MusteriTelefonDetayModel tel = new MusteriTelefonDetayModel();

                        tel.IletisimNumaraTipi = item.IletisimNumaraTipi;
                        tel.IletisimNumaraText = MusteriListProvider.GetNumaraTipiText(item.IletisimNumaraTipi);
                        tel.MusteriKodu = item.MusteriKodu;
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresEkle(int musteriKodu)
        {
            AdresModel model = new AdresModel();

            try
            {
                model.MusteriKodu = musteriKodu;
                model.sayfaadi = "ekle";

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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresEkle(AdresModel model)
        {
            try
            {
                TryValidateModel(model);

                if (model != null)
                {
                    if (!String.IsNullOrEmpty(model.Cadde))
                        if (ModelState["Sokak"] != null)
                            ModelState["Sokak"].Errors.Clear();

                    if (!String.IsNullOrEmpty(model.Sokak))
                        if (ModelState["Cadde"] != null)
                            ModelState["Cadde"].Errors.Clear();
                }

                if (ModelState.IsValid && model.MusteriKodu > 0)
                {


                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    //Yetkisiz Erişim Denetimi
                    if (musteri == null)
                    {
                        model.sayfaadi = "ekle";
                        AdresModelDoldur(model);
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_AdresEkle", model);
                    };

                    MusteriAdre adres = new MusteriAdre();

                    adres.MusteriKodu = musteri.MusteriKodu;
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


                    if (!String.IsNullOrEmpty(model.Cadde))
                        if (String.IsNullOrEmpty(model.Sokak))
                            adres.Sokak = "";

                    if (!String.IsNullOrEmpty(model.Sokak))
                        if (String.IsNullOrEmpty(model.Cadde))
                            adres.Cadde = "";

                    if (String.IsNullOrEmpty(model.Adres))
                    {
                        if (!String.IsNullOrEmpty(model.Mahalle))
                            adres.Adres = model.Mahalle + " Mah.";

                        if (!String.IsNullOrEmpty(model.Cadde))
                            adres.Adres += model.Cadde + " Cad.";

                        if (!String.IsNullOrEmpty(model.Sokak))
                            adres.Adres += model.Mahalle + " SK.";

                        if (!String.IsNullOrEmpty(model.DaireNo) && !String.IsNullOrEmpty(model.BinaNo))
                            adres.Adres += model.BinaNo + " / " + model.BinaNo;
                    }

                    _MusteriService.CreateMusteriAdres(adres);
                    return null;

                }
                return PartialView("_AdresEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);

            }
            model.sayfaadi = "ekle";
            AdresModelDoldur(model);
            ModelState.AddModelError("", babonline.Message_RequiredValues);
            return PartialView("_AdresEkle", model);
        }

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresGuncelle(int musteriKodu, int siraNo)
        {
            AdresModel model = new AdresModel();
            try
            {
                if (musteriKodu > 0)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);
                    //Yetki Denetimi
                    if (musteri == null)
                        return null;

                    MusteriAdre adres = _MusteriService.GetAdres(musteriKodu, siraNo);
                    if (adres == null) return null;

                    Mapper.CreateMap<MusteriAdre, AdresModel>();

                    model = Mapper.Map<AdresModel>(adres);
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresGuncelle(AdresModel model)
        {
            try
            {
                TryValidateModel(model);

                if (model != null)
                {
                    if (!String.IsNullOrEmpty(model.Cadde))
                        if (ModelState["Sokak"] != null)
                            ModelState["Sokak"].Errors.Clear();

                    if (!String.IsNullOrEmpty(model.Sokak))
                        if (ModelState["Cadde"] != null)
                            ModelState["Cadde"].Errors.Clear();
                }

                if (ModelState.IsValid && model.MusteriKodu > 0)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    if (musteri == null)
                    {
                        model.sayfaadi = "guncelle";
                        AdresModelDoldur(model);
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_AdresEkle", model);
                    }

                    MusteriAdre adres = musteri.MusteriAdres.Where(s => s.SiraNo == model.SiraNo).FirstOrDefault();
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

                    if (!String.IsNullOrEmpty(model.Cadde))
                        if (String.IsNullOrEmpty(model.Sokak))
                            adres.Sokak = "";

                    if (!String.IsNullOrEmpty(model.Sokak))
                        if (String.IsNullOrEmpty(model.Cadde))
                            adres.Cadde = "";

                    if (String.IsNullOrEmpty(model.Adres))
                    {
                        if (!String.IsNullOrEmpty(model.Mahalle))
                            adres.Adres = model.Mahalle + " Mah.";

                        if (!String.IsNullOrEmpty(model.Cadde))
                            adres.Adres += model.Cadde + " Cad.";

                        if (!String.IsNullOrEmpty(model.Sokak))
                            adres.Adres += model.Mahalle + " SK.";

                        if (!String.IsNullOrEmpty(model.DaireNo) && !String.IsNullOrEmpty(model.BinaNo))
                            adres.Adres += model.BinaNo + " / " + model.BinaNo;
                    }

                    _MusteriService.UpdateAdres(adres);
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresSilKontrol(int MusteriKodu, int SiraNo)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Basarili = "false", Yetkili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);


                MusteriAdre adres = musteri.MusteriAdres.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (adres == null)
                    return Json(new { Basarili = "false", Yetkili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);


                if (adres.Varsayilan != true)
                    return Json(new { Basarili = "true", Yetkili = "true", Message = babonline.TheOperationWasSuccessful }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Basarili = "true", Yetkili = "true", Message = "Varsayılan adres" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.AnErrorHasOccurred }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult AdresSil(int MusteriKodu, int SiraNo)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Yetkili = "false", Basarili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);

                MusteriAdre adres = musteri.MusteriAdres.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (adres == null)
                    return Json(new { Yetkili = "true", Basarili = "false", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);


                bool sonuc = _MusteriService.DeleteAdres(adres.MusteriKodu, adres.SiraNo);

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
                AdresListModel model = new AdresListModel();

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);
                if (musteri == null) return null;

                model.Items = MusteriAdresleriGetir(musteriKodu);
                model.sayfaAdi = sayfaAdi;

                return PartialView("_Adresler", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        private void AdresModelDoldur(AdresModel model)
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
                       AltMenuKodu = AltMenuler.MusteriEkle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Gorme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
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

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Upload(int musteriKodu)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);
                if (musteri == null) return null;

                DokumanModel model = new DokumanModel();
                model.sayfaadi = "ekle";
                model.MusteriKodu = musteri.MusteriKodu;

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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult Upload(DokumanModel model, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid && file != null && file.ContentLength > 0 && model.MusteriKodu > 0)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    if (musteri == null)
                    {//Yetkisiz Erişim
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_DokumanEkle", model);
                    }

                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string url = _Storage.UploadFile(model.MusteriKodu.ToString(), fileName, file.InputStream);

                    MusteriDokuman dokuman = new MusteriDokuman();
                    dokuman.MusteriKodu = musteri.MusteriKodu;
                    dokuman.TVMKodu = _AktifKullanici.TVMKodu;
                    dokuman.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                    dokuman.KayitTarihi = TurkeyDateTime.Now;
                    dokuman.DosyaAdi = fileName;
                    dokuman.DokumanTuru = model.DokumanTuru;
                    dokuman.DokumanURL = url;

                    _MusteriService.CreateDokuman(dokuman);
                    return null;
                }

                //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return PartialView("_DokumanEkle", model);
            }
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult DokumanSil(int MusteriKodu, int SiraNo)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Basarili = "false", Yetkili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);

                MusteriDokuman dokuman = musteri.MusteriDokumen.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (dokuman == null)
                    return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);

                bool sonuc = _MusteriService.DeleteDokuman(MusteriKodu, SiraNo);
                if (sonuc)
                    return Json(new { Basarili = "true", Yetkili = "true", Message = babonline.TheOperationWasSuccessful }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanlariDoldur(int musteriKodu, string sayfaAdi)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);
                if (musteri == null) return null; //Yetkisiz Erişim

                DokumanListModel model = new DokumanListModel();
                model.Items = MusteriDokumanlariGetir(musteri.MusteriKodu);
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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult NotEkle(int MusteriKodu)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
                if (musteri == null) return null;

                NotModel model = new NotModel();
                model.MusteriKodu = MusteriKodu;

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
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult NotEkle(NotModel model)
        {
            try
            {
                if (ModelState.IsValid && model.MusteriKodu > 0)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);
                    if (musteri == null)
                    {
                        ModelState.AddModelError("", babonline.Access_Is_Denied);
                        return PartialView("_NotEkle", model);
                    }

                    MusteriNot not = new MusteriNot();

                    not.Konu = model.Konu;
                    not.NotAciklamasi = model.NotAciklamasi;
                    not.TVMKodu = _AktifKullanici.TVMKodu;
                    not.TVMPersonelKodu = _AktifKullanici.KullaniciKodu;
                    not.KayitTarihi = TurkeyDateTime.Now;

                    int siraNo = 1;
                    if (musteri.MusteriNots.Count() > 0)
                        siraNo = musteri.MusteriNots.Max(s => s.SiraNo) + 1;

                    not.SiraNo = siraNo;

                    musteri.MusteriNots.Add(not);
                    _MusteriService.UpdateMusteri(musteri);

                    return null;
                }

                ModelState.AddModelError("", babonline.Message_RequiredValues);
                return PartialView("_NotEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return PartialView("_NotEkle", model);
            }
        }

        //[AjaxException]
        //[Authorization(AnaMenuKodu = AnaMenuler.Musteri,
        //               AltMenuKodu = AltMenuler.AraGuncelle,
        //               SekmeKodu = 0,
        //               menuPermission = MenuPermission.Guncelleme,
        //               menuPermissionType = MenuPermissionType.AltMenu)]
        //public ActionResult NotGuncelle(int MusteriKodu, int SiraNo)
        //{
        //    try
        //    {
        //        MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
        //        if (musteri == null) return null;

        //        MusteriNot not = musteri.MusteriNots.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
        //        if (not == null) return null;

        //        Mapper.CreateMap<MusteriNot, NotModel>();
        //        NotModel model = Mapper.Map<NotModel>(not);
        //        model.sayfaadi = "guncelle";

        //        return PartialView("_NotEkle", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _Log.Error(ex);
        //        return null;
        //    }
        //}

        //[HttpPost]
        //public ActionResult NotGuncelle(NotModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Mapper.CreateMap<NotModel, MusteriNot>();
        //        MusteriNot not = Mapper.Map<MusteriNot>(model);

        //        _MusteriService.UpdateNot(not);
        //        return null;
        //    }
        //    return PartialView("_NotEkle", model);
        //}

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Musteri,
                       AltMenuKodu = AltMenuler.AraGuncelle,
                       SekmeKodu = 0,
                       menuPermission = MenuPermission.Silme,
                       menuPermissionType = MenuPermissionType.AltMenu)]
        public ActionResult NotSil(int MusteriKodu, int SiraNo)
        {
            try
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(MusteriKodu);
                if (musteri == null)
                    return Json(new { Basarili = "false", Yetkili = "false", Message = babonline.Access_Is_Denied }, JsonRequestBehavior.AllowGet);

                MusteriNot not = musteri.MusteriNots.Where(s => s.SiraNo == SiraNo).FirstOrDefault();
                if (not == null)
                    return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);

                _MusteriService.DeleteNot(not.MusteriKodu, not.SiraNo);
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
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);
                if (musteri == null) return null;

                NotListModel model = new NotListModel();
                model.Items = MusteriNotlariGetir(musteriKodu);
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
        private List<AdresModel> MusteriAdresleriGetir(int musteriKodu)
        {
            List<MusteriAdre> adresler = _MusteriService.GetMusteriAdresleri(musteriKodu);
            Mapper.CreateMap<MusteriAdre, AdresModel>();
            List<AdresModel> model = Mapper.Map<List<MusteriAdre>, List<AdresModel>>(adresler);

            foreach (var item in model)
            {
                item.UlkeAdi = _UlkeService.GetUlkeAdi(item.UlkeKodu);
                item.IlAdi = _UlkeService.GetIlAdi(item.UlkeKodu, item.IlKodu);
                item.IlceAdi = _UlkeService.GetIlceAdi(item.IlceKodu);
                item.AdresTipiText = MusteriListProvider.GetAdresTipiText((short)item.AdresTipi.Value);
                if (_AktifKullanici.ProjeKodu == "Lilyum")
                {
                    List<int> teklifid = _TeklifService.GetTeklifID(item.MusteriKodu);
                    if (teklifid != null)
                    {
                        List<string> teklifSoruAdresler = _TeklifService.GetAdresler(teklifid);
                        if (teklifSoruAdresler.Contains(item.Adres))
                        {
                            item.guncellenebilecekMi = false;
                        }
                        else
                        {
                            item.guncellenebilecekMi = true;
                        }
                    }
                    else
                    {
                        item.guncellenebilecekMi = true;
                    }
                }
                else
                {
                    item.guncellenebilecekMi = true;
                }
            }
            return model;
        }

        //Musteriye ait Tüm Telefonlari Getirir.. 
        private List<MusteriTelefonDetayModel> MusteriTelefonlariGetir(int musteriKodu)
        {
            List<MusteriTelefon> telefonlar = _MusteriService.GetMusteriTelefon(musteriKodu);
            Mapper.CreateMap<MusteriTelefon, MusteriTelefonDetayModel>();
            List<MusteriTelefonDetayModel> model = Mapper.Map<List<MusteriTelefon>, List<MusteriTelefonDetayModel>>(telefonlar);

            foreach (var item in model)
            {
                item.IletisimNumaraText = MusteriListProvider.GetNumaraTipiText(item.IletisimNumaraTipi);
                if (_AktifKullanici.ProjeKodu == "Lilyum")
                {
                    List<int> teklifid = _TeklifService.GetTeklifID(item.MusteriKodu);
                    if (teklifid != null)
                    {
                        List<string> teklifSoruTelefonlar = _TeklifService.GetTelefonlar(teklifid);
                        if (teklifSoruTelefonlar.Contains(item.Numara))
                        {
                            item.guncellenebilecekMi = false;
                        }
                        else
                        {
                            item.guncellenebilecekMi = true;
                        }
                    }
                    else
                    {
                        item.guncellenebilecekMi = true;
                    }
                }
                else
                {
                    item.guncellenebilecekMi = true;
                }
            }

            return model;
        }

        //Musteriye ait Tüm Dokumnalari getirir
        private List<DokumanDetayModel> MusteriDokumanlariGetir(int musteriKodu)
        {
            List<DokumanDetayModel> model = _MusteriService.GetMusteriDokumanlari(musteriKodu);
            return model;
        }

        //Musterite ait Tüm Notlari getirir
        private List<NotModel> MusteriNotlariGetir(int musteriKodu)
        {
            List<NotModelDetay> notlar = _MusteriService.GetMusteriNotlari(musteriKodu);

            Mapper.CreateMap<NotModelDetay, NotModel>();
            List<NotModel> model = Mapper.Map<List<NotModelDetay>, List<NotModel>>(notlar);

            return model;
        }
        #endregion

        #region ListePager Methodları
        //Tableleri doldurmak icin kullanılan methodlar
        public ActionResult ListePagerTelefon(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<MusteriTelefon> telefonList = new DataTableParameters<MusteriTelefon>(Request, new Expression<Func<MusteriTelefon, object>>[]
                                                                                        {
                                                                                            t => t.IletisimNumaraTipi,
                                                                                            t => t.Numara,
                                                                                            t => t.NumaraSahibi

                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListTelefon(telefonList, id);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerAdres(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<MusteriAdre> adresList = new DataTableParameters<MusteriAdre>(Request, new Expression<Func<MusteriAdre, object>>[]
                                                                                        {
                                                                                           t=>t.Apartman,
                                                                                            t => t.Adres,
                                                                                            t => t.Cadde,
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListAdres(adresList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerDokuman(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<MusteriDokuman> dokumanList = new DataTableParameters<MusteriDokuman>(Request, new Expression<Func<MusteriDokuman, object>>[]
                                                                                        {
                                                                                            t => t.DosyaAdi,
                                                                                            t => t.DokumanTuru
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListDokuman(dokumanList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerNot(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<MusteriNot> notList = new DataTableParameters<MusteriNot>(Request, new Expression<Func<MusteriNot, object>>[]
                                                                                        {
                                                                                            t => t.Konu,
                                                                                            t => t.NotAciklamasi
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MusteriService.PagedListNot(notList, id);
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
            try
            {
                if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(tvmKodu))
                {
                    List<MusteriFinderOzetModel> model = _MusteriService.GetMusteriListByTvmKodu(tvmKodu);
                    return PartialView("_MusteriListe", model);
                }
                else return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [AjaxException]
        public ActionResult GetMusterilerim(MusteriharitaAramaModel model)
        {
            try
            {
                if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(model.TVMKodu))
                {
                    List<MusteriHaritaOzelDetay> musteriList = _MusteriService.MusterilerimHaritaArama(model);
                    return Json(new { Success = "True", Authority = "True", MusteriList = musteriList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = "True", Authority = "False" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(new { Success = "False" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        public vCardResult DownloadVcard(int id)
        {
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(id);
            VCard card = new VCard();

            if (musteri != null)
            {
                card.FirstName = musteri.AdiUnvan;
                card.LastName = musteri.SoyadiUnvan;
                card.Email = musteri.EMail;
                card.HomePage = musteri.WebUrl;

                //Adres Bilgileri
                if (musteri.MusteriAdres.Count > 0)
                {
                    MusteriAdre adres = musteri.MusteriAdres.Where(s => s.Varsayilan == true).FirstOrDefault();
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
                if (musteri.MusteriTelefons.Count > 0)
                {
                    List<MusteriTelefon> telefonlar = musteri.MusteriTelefons.ToList<MusteriTelefon>();
                    MusteriTelefon cepTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault<MusteriTelefon>();
                    MusteriTelefon evTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault<MusteriTelefon>();
                    MusteriTelefon isTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Is).FirstOrDefault<MusteriTelefon>();

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

        public MusteriModel PotansiyelMusteriAktar(PotansiyelMusteriGenelBilgiler potansiyelMusteri)
        {
            if (potansiyelMusteri != null)
            {
                MusteriModel model = new MusteriModel();
                model.MusteriTelefonModel = new MusteriTelefonModel();
                model.MusteriAdresModel = new AdresModel();
                model.GenelBilgiler = new GenelBilgilerModel();
                model.GenelBilgiler.TVMKodu = _AktifKullanici.TVMKodu;
                model.GenelBilgiler.TVMUnvani = _AktifKullanici.TVMUnvani;

                model.PotansiyelMi = true;
                model.PotansiyelMusteriKodu = potansiyelMusteri.PotansiyelMusteriKodu;

                model.GenelBilgiler.AdiUnvan = potansiyelMusteri.AdiUnvan;
                model.GenelBilgiler.SoyadiUnvan = potansiyelMusteri.SoyadiUnvan;
                model.GenelBilgiler.Cinsiyet = potansiyelMusteri.Cinsiyet;

                if (potansiyelMusteri.DogumTarihi.HasValue)
                    model.GenelBilgiler.DogumTarihi = potansiyelMusteri.DogumTarihi.Value;

                model.GenelBilgiler.EgitimDurumu = potansiyelMusteri.EgitimDurumu;
                model.GenelBilgiler.EMail = potansiyelMusteri.EMail;
                model.GenelBilgiler.KimlikNo = potansiyelMusteri.KimlikNo;
                model.GenelBilgiler.MedeniDurumu = potansiyelMusteri.MedeniDurumu;
                model.GenelBilgiler.MeslekKodu = potansiyelMusteri.MeslekKodu;
                model.GenelBilgiler.MusteriTipKodu = potansiyelMusteri.MusteriTipKodu;

                if (potansiyelMusteri.PasaportGecerlilikBitisTarihi.HasValue)
                    model.GenelBilgiler.PasaportGecerlilikBitisTarihi = potansiyelMusteri.PasaportGecerlilikBitisTarihi.Value;

                model.GenelBilgiler.PasaportNo = potansiyelMusteri.PasaportNo;

                model.GenelBilgiler.TVMKodu = potansiyelMusteri.TVMKodu;
                model.GenelBilgiler.TVMKullaniciKodu = potansiyelMusteri.TVMKullaniciKodu;
                model.GenelBilgiler.TVMMusteriKodu = potansiyelMusteri.TVMMusteriKodu;
                model.GenelBilgiler.Uyruk = potansiyelMusteri.Uyruk ?? 0;
                model.GenelBilgiler.VergiDairesi = potansiyelMusteri.VergiDairesi;
                model.GenelBilgiler.WebUrl = potansiyelMusteri.WebUrl;

                List<PotansiyelMusteriAdre> adresler = _MusteriService.GetPotansiyelMusteriAdresleri(potansiyelMusteri.PotansiyelMusteriKodu);

                if (adresler.Count > 0)
                {
                    PotansiyelMusteriAdre adres = adresler[0];

                    if (adres.IlceKodu.HasValue)
                        model.MusteriAdresModel.IlceKodu = adres.IlceKodu.Value;

                    model.MusteriAdresModel.UlkeKodu = adres.UlkeKodu;
                    model.MusteriAdresModel.IlKodu = adres.IlKodu;
                    model.MusteriAdresModel.Adres = adres.Adres;
                    model.MusteriAdresModel.AdresTipi = adres.AdresTipi;
                    model.MusteriAdresModel.Apartman = adres.Apartman;
                    model.MusteriAdresModel.BinaNo = adres.BinaNo;
                    model.MusteriAdresModel.Cadde = adres.Cadde;
                    model.MusteriAdresModel.DaireNo = adres.DaireNo;
                    model.MusteriAdresModel.Diger = adres.Diger;
                    model.MusteriAdresModel.HanAptFab = adres.HanAptFab;
                    model.MusteriAdresModel.Mahalle = adres.Mahalle;
                    model.MusteriAdresModel.PostaKodu = adres.PostaKodu;
                    model.MusteriAdresModel.Sokak = adres.Sokak;
                    model.MusteriAdresModel.Semt = adres.Semt;
                    model.MusteriAdresModel.Latitude = adres.Latitude;
                    model.MusteriAdresModel.Longitude = adres.Longitude;
                }

                List<PotansiyelMusteriTelefon> telefonlar = _MusteriService.GetPotansiyelMusteriTelefon(potansiyelMusteri.PotansiyelMusteriKodu);

                if (telefonlar.Count > 0)
                {
                    PotansiyelMusteriTelefon tel = telefonlar[0];

                    model.MusteriTelefonModel.IletisimNumaraTipi = tel.IletisimNumaraTipi;
                    model.MusteriTelefonModel.MusteriKodu = tel.PotansiyelMusteriKodu;
                    model.MusteriTelefonModel.Numara = tel.Numara;
                    model.MusteriTelefonModel.NumaraSahibi = tel.NumaraSahibi;
                }

                model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
                model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", null);
                model.UyrukTipleri = new SelectList(MusteriListProvider.UyrukTipleri(), "Value", "Text", "1");
                model.MedeniDurumTipleri = new SelectList(MusteriListProvider.MedeniDurumTipleri(), "Value", "Text", "0");
                model.EgitimDurumlari = new SelectList(MusteriListProvider.EgitimDurumlari(), "Value", "Text", "0").ToList();
                model.Meslekler = new SelectList(_MeslekVeTanimService.GetListMeslek(), "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
                model.IletisimNumaraTipleri = new SelectList(MusteriListProvider.IletisimNumaraTipleri(), "Value", "Text", "0").ToList();


                AdresModelDoldur(model.MusteriAdresModel);

                return (model);
            }

            return new MusteriModel();
        }

        #region Extra

        private void EkleModelDoldur(MusteriModel model)
        {
            model.MusteriTipleri = new SelectList(MusteriListProvider.MusteriTipleri(), "Value", "Text", "0");
            model.CinsiyetTipleri = new SelectList(MusteriListProvider.CinsiyetTipleri(), "Value", "Text", "1");
            model.UyrukTipleri = new SelectList(MusteriListProvider.UyrukTipleri(), "Value", "Text", "1");
            model.MedeniDurumTipleri = new SelectList(MusteriListProvider.MedeniDurumTipleri(), "Value", "Text", "0");

            List<Meslek> meslekler = _MeslekVeTanimService.GetListMeslek();


            model.EgitimDurumlari = new SelectList(MusteriListProvider.EgitimDurumlari(), "Value", "Text", "0").ToList();
            model.Meslekler = new SelectList(meslekler, "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
            model.IletisimNumaraTipleri = new SelectList(MusteriListProvider.IletisimNumaraTipleri(), "Value", "Text", "0").ToList();


            //Tüzel kişi için...
            //List<GenelTanimlar> faaliyetOlcegi = _MeslekVeTanimService.GetListTanimlar("FaaliyetOlcegi");
            //List<GenelTanimlar> ciroBilgisi = _MeslekVeTanimService.GetListTanimlar("CiroBilgisi");
            //List<GenelTanimlar> sabitVarlik = _MeslekVeTanimService.GetListTanimlar("SabitVarlik");
            //List<GenelTanimlar> faaliyetAnaSektor = _MeslekVeTanimService.GetListTanimlar("FaaliyetAnaSektor");
            //List<GenelTanimlar> faaliyetAltSektor = _MeslekVeTanimService.GetListAltSektor("1000");
            //model.FaaliyetOlcegiList = new SelectList(faaliyetOlcegi, "TanimId", "Aciklama", "0").ListWithOptionLabel();
            //model.FaaliyerAnaSektorList = new SelectList(faaliyetAnaSektor, "TanimId", "Aciklama", "0").ListWithOptionLabel();
            //model.FaaliyetAltSektorList = new SelectList(faaliyetAltSektor, "TanimId", "Aciklama", "0").ListWithOptionLabel();
            //model.SabitVarlikList = new SelectList(sabitVarlik, "TanimId", "Aciklama", "0").ListWithOptionLabel();
            //model.CiroBilgisiList = new SelectList(ciroBilgisi, "TanimId", "Aciklama", "0").ListWithOptionLabel();


            AdresModelDoldur(model.MusteriAdresModel);
        }
        private void GuncelleModelDoldur(MusteriModel model)
        {
            //Musteriye eklenmiş olan tüm adres,dokuman,telefon ve notlari getirir..
            model.Adresleri = new AdresListModel();
            model.Telefonlari = new MusteriTelefonListModel();
            model.Dokumanlari = new DokumanListModel();
            model.Notlari = new NotListModel();
            model.Adresleri.sayfaAdi = "guncelle";
            model.Telefonlari.sayfaAdi = "guncelle";
            model.Dokumanlari.sayfaAdi = "guncelle";
            model.Notlari.sayfaAdi = "guncelle";
            model.Adresleri.Items = MusteriAdresleriGetir(model.MusteriGuncelleModel.MusteriKodu);
            model.Dokumanlari.Items = MusteriDokumanlariGetir(model.MusteriGuncelleModel.MusteriKodu);
            model.Telefonlari.Items = MusteriTelefonlariGetir(model.MusteriGuncelleModel.MusteriKodu);
            model.Notlari.Items = MusteriNotlariGetir(model.MusteriGuncelleModel.MusteriKodu);


            List<Meslek> meslekler = _MeslekVeTanimService.GetListMeslek().ToList<Meslek>();
            // model.AdresTipleri = new SelectList(MusteriListProvider.AdresTipleri(), "Value", "Text", model.AdresTipleri).ToList();
            model.EgitimDurumlari = new SelectList(MusteriListProvider.EgitimDurumlari(), "Value", "Text", model.MusteriGuncelleModel.EgitimDurumu).ToList();
            model.MedeniDurumTipleri = new SelectList(MusteriListProvider.MedeniDurumTipleri(), "Value", "Text", model.MusteriGuncelleModel.MedeniDurumu);
            model.Meslekler = new SelectList(meslekler, "MeslekKodu", "MeslekAdi", "0").ListWithOptionLabel();
            model.UyrukTipleri = new SelectList(MusteriListProvider.UyrukTipleri(), "Value", "Text", model.MusteriGuncelleModel.Uyruk);
            model.MusteriGuncelleModel.MusteriTipiText = MusteriListProvider.GetMusteriTipiText(model.MusteriGuncelleModel.MusteriTipKodu);
            //Bağlı olduğu tvmnin unvanı text olarak getiriliyor..
            model.MusteriGuncelleModel.TVMUnvani = _TVMService.GetTvmUnvan(model.MusteriGuncelleModel.TVMKodu.Value);


            //Tüzel kişi için...
            //List<GenelTanimlar> faaliyetAltSektor = new List<GenelTanimlar>();
            //List<GenelTanimlar> faaliyetOlcegi = _MeslekVeTanimService.GetListTanimlar("FaaliyetOlcegi");
            //List<GenelTanimlar> ciroBilgisi = _MeslekVeTanimService.GetListTanimlar("CiroBilgisi");
            //List<GenelTanimlar> sabitVarlik = _MeslekVeTanimService.GetListTanimlar("SabitVarlik");
            //List<GenelTanimlar> faaliyetAnaSektor = _MeslekVeTanimService.GetListTanimlar("FaaliyetAnaSektor");
            //if (model.MusteriGuncelleModel.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
            //{
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetGosterdigiAltSektor))
            //        faaliyetAltSektor = _MeslekVeTanimService.GetListAltSektor(model.MusteriGuncelleModel.FaaliyetGosterdigiAnaSektor);
            //    else
            //        faaliyetAltSektor = _MeslekVeTanimService.GetListTanimlar("FaaliyetAnaSektor");
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetOlcegi_))
            //        model.FaaliyetOlcegiList = new SelectList(faaliyetOlcegi, "TanimId", "Aciklama", model.MusteriGuncelleModel.FaaliyetOlcegi_).ListWithOptionLabel();
            //    else
            //        model.FaaliyetOlcegiList = new SelectList(faaliyetOlcegi, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetGosterdigiAnaSektor))
            //        model.FaaliyerAnaSektorList = new SelectList(faaliyetAnaSektor, "TanimId", "Aciklama", model.MusteriGuncelleModel.FaaliyetGosterdigiAnaSektor).ListWithOptionLabel();
            //    else
            //        model.FaaliyerAnaSektorList = new SelectList(faaliyetAnaSektor, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.FaaliyetGosterdigiAltSektor))
            //        model.FaaliyetAltSektorList = new SelectList(faaliyetAltSektor, "TanimId", "Aciklama", model.MusteriGuncelleModel.FaaliyetGosterdigiAltSektor).ListWithOptionLabel();
            //    else
            //        model.FaaliyetAltSektorList = new SelectList(faaliyetAltSektor, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.SabitVarlikBilgisi))
            //        model.SabitVarlikList = new SelectList(sabitVarlik, "TanimId", "Aciklama", model.MusteriGuncelleModel.SabitVarlikBilgisi).ListWithOptionLabel();
            //    else
            //        model.SabitVarlikList = new SelectList(sabitVarlik, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    if (!String.IsNullOrEmpty(model.MusteriGuncelleModel.CiroBilgisi))
            //        model.CiroBilgisiList = new SelectList(ciroBilgisi, "TanimId", "Aciklama", model.MusteriGuncelleModel.CiroBilgisi).ListWithOptionLabel();
            //    else
            //        model.CiroBilgisiList = new SelectList(ciroBilgisi, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //}
            //else
            //{
            //    faaliyetAltSektor = _MeslekVeTanimService.GetListTanimlar("FaaliyetAltSektor");
            //    model.FaaliyetOlcegiList = new SelectList(faaliyetOlcegi, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    model.FaaliyerAnaSektorList = new SelectList(faaliyetAnaSektor, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    model.FaaliyetAltSektorList = new SelectList(faaliyetAltSektor, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    model.SabitVarlikList = new SelectList(sabitVarlik, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //    model.CiroBilgisiList = new SelectList(ciroBilgisi, "TanimId", "Aciklama", "").ListWithOptionLabel();
            //}
        }
        //private void TuzelMusteriDetayDoldur(MusteriModel model, MusteriGenelBilgiler musteri)
        //{
        //    if (!String.IsNullOrEmpty(musteri.FaaliyetOlcegi_))
        //        model.GenelBilgiler.FaaliyetOlcegiText = _MeslekVeTanimService.GetTanim("FaaliyetOlcegi", musteri.FaaliyetOlcegi_).Aciklama;

        //    if (!String.IsNullOrEmpty(musteri.FaaliyetGosterdigiAnaSektor))
        //        model.GenelBilgiler.FaaliyetAnaSektorText = _MeslekVeTanimService.GetTanim("FaaliyetAnaSektor", musteri.FaaliyetGosterdigiAnaSektor).Aciklama;

        //    if (!String.IsNullOrEmpty(musteri.FaaliyetGosterdigiAltSektor))
        //        model.GenelBilgiler.FaaliyetAltSektorText = _MeslekVeTanimService.GetTanim("FaaliyetAltSektor", musteri.FaaliyetGosterdigiAltSektor).Aciklama;

        //    if (!String.IsNullOrEmpty(musteri.SabitVarlikBilgisi))
        //        model.GenelBilgiler.SabitVarlikBilgisiText = _MeslekVeTanimService.GetTanim("SabitVarlik", musteri.SabitVarlikBilgisi).Aciklama;

        //    if (!String.IsNullOrEmpty(musteri.CiroBilgisi))
        //        model.GenelBilgiler.CiroBilgisiText = _MeslekVeTanimService.GetTanim("CiroBilgisi", musteri.CiroBilgisi).Aciklama;

        //}
        public bool TCKontrol(string TCKN, int TVMKodu)
        {
            if (_MusteriService.GetMusteri(TCKN, TVMKodu) != null)
            {
                return false;
            }
            return true;
        }

        public void MusteriAdreslerEnlemBoylamEkle()
        {
            _MusteriService.MusteriEnlemBoylamGetir();
        }
        #endregion

        public void XmlToDB()
        {
            _MusteriService.XmlToDB();
        }
    }
}

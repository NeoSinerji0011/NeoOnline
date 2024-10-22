using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Komisyon, AltMenuKodu = AltMenuler.OranBelirleme, SekmeKodu = 0)]
    public class OranController : Controller
    {
        #region ServiceVariable

        IKomisyonService _KomisyonService;
        ISigortaSirketleriService _ISigortaSirketleriService;
        IBransService _IBransService;
        ITVMService _ITVMService;

        #endregion

        #region Constructor

        public OranController(IKomisyonService komisyonService,
                              ISigortaSirketleriService sigortaSirketleriService,
                              IBransService bransService,
                              ITVMService tvmService)
        {
            _KomisyonService = komisyonService;
            _ISigortaSirketleriService = sigortaSirketleriService;
            _IBransService = bransService;
            _ITVMService = tvmService;
        }

        #endregion

        #region Public Method

        [Authorization(AnaMenuKodu = AnaMenuler.Komisyon, AltMenuKodu = AltMenuler.OranBelirleme, SekmeKodu = 0)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Komisyon, AltMenuKodu = AltMenuler.OranBelirleme, SekmeKodu = 0)]
        public ActionResult Liste(TaliKomisyonOranFiltreModel filtreModel)
        {
            try
            {
                var oranListe = new List<TaliKomisyonOranListeModel>();
                bool isCreate = true;
                int toplam = 0;
                if (filtreModel.Islem == TaliKomisyonOranFiltreIslem.OranBelirle)
                    isCreate = KomisyonOranOlustur(filtreModel);
                if (isCreate)
                    oranListe = KomisyonOranListeGetir(filtreModel, out toplam);
                else
                    return Json(new { Hata = true });
                return Json(new { Hata = false, Liste = oranListe, Toplam = toplam });
            }
            catch (Exception ex)
            {
                return Json(new { Hata = true });
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Komisyon, AltMenuKodu = AltMenuler.OranBelirleme, SekmeKodu = 0)]
        public ActionResult Guncelle(TaliKomisyonOranListeModel oranModel)
        {
            try
            {
                if (oranModel.KademeListesi.Count() > 0)
                {
                    bool guncellendiMi = false;
                    var kademeList = new List<KomisyonKademeModel>();
                    foreach (var k in oranModel.KademeListesi)
                    {
                        kademeList.Add(new KomisyonKademeModel()
                        {
                            MinUretim = k.MinUretim,
                            MaxUretim = k.MaxUretim,
                            Oran = k.Oran
                        });
                    }
                    var sonucModel = _KomisyonService.TaliAcenteKademeliKomisyonGuncelle(oranModel.KomisyonOranId, oranModel.GecerliYil, kademeList, out guncellendiMi);
                    var kademeliListe = _KomisyonService.TaliAcenteKademeliKomisyonListesi(oranModel.KomisyonOranId);

                    var model = new TaliKomisyonOranListeModel()
                    {
                        KomisyonOranId = kademeliListe[0].KomisyonOranId,
                        TaliUnvani = _ITVMService.GetDetay(Convert.ToInt32(kademeliListe[0].TaliTVMKodu)).Unvani,
                        BransAdi = _IBransService.GetBrans(Convert.ToInt32(kademeliListe[0].BransKodu)).BransAdi,
                        SigortaSirketiAdi = _ISigortaSirketleriService.GetSirket(kademeliListe[0].SigortaSirketKodu).SirketAdi,
                        GecerliYil = ((DateTime)kademeliListe[0].GecirlilikBaslangicTarihi).Year
                    };
                    int sira = 0;
                    foreach (var kItem in kademeliListe)
                    {
                        model.KademeListesi.Add(new KademeliOran()
                        {
                            Sira = sira++,
                            MinUretim = kItem.MinUretim ?? 0,
                            MaxUretim = kItem.MaxUretim ?? 0,
                            Oran = kItem.KomisyonOran ?? 0
                        });
                    }
                    return Json(new { Hata = !guncellendiMi, Model = model });
                }
                else
                {
                    bool guncellendiMi = false;
                    var sonucModel = _KomisyonService.TaliAcenteKomisyonGuncelle(oranModel.KomisyonOranId, Convert.ToDateTime(oranModel.BaslangicTarihi), oranModel.Oran, out guncellendiMi);

                    var model = new TaliKomisyonOranListeModel()
                    {
                        BaslangicTarihi = (sonucModel.GecirlilikBaslangicTarihi ?? new DateTime()).ToString("yyyy-MM-dd"),
                        BransAdi = _IBransService.GetBrans(Convert.ToInt32(sonucModel.BransKodu)).BransAdi,
                        KomisyonOranId = sonucModel.KomisyonOranId,
                        Oran = sonucModel.KomisyonOran ?? 0,
                        SigortaSirketiAdi = _ISigortaSirketleriService.GetSirket(sonucModel.SigortaSirketKodu).SirketAdi,
                        TaliUnvani = _ITVMService.GetDetay(Convert.ToInt32(sonucModel.TaliTVMKodu)).Unvani,
                        TaliDisKaynakUnvani = sonucModel.DisKaynakKodu.HasValue ? _ITVMService.GetDetay(Convert.ToInt32(sonucModel.DisKaynakKodu)).Unvani : "",
                    };
                    return Json(new { Hata = !guncellendiMi, Model = model });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Hata = true });
            }
        }

        #endregion

        #region Private Method

        private bool KomisyonOranOlustur(TaliKomisyonOranFiltreModel filtreModel)
        {
            if (filtreModel.Kademeli)
            {
                if (filtreModel.KaynakSecim == SatisKanaliDisKaynakFiltre.SatisKanali)
                {
                    if (filtreModel.TVMListe != null && filtreModel.TVMListe.Count > 0)
                    {
                        //Alt Kaynağın , dış kaynak üzerinden aldığı komisyon oranı tanımlanacak ise ekrandan seçilen TaliDisKaynakKodu parametresinin de3ğeri gönderilecek
                        return _KomisyonService.TaliAcenteKademeliKomisyonListesiOlustur(filtreModel.TVMListe, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.GecerliYil, filtreModel.KademeListe);
                    }
                }
                else if (filtreModel.KaynakSecim == SatisKanaliDisKaynakFiltre.DisKaynak)
                {
                    if (filtreModel.DisKaynakList != null && filtreModel.DisKaynakList.Count > 0)
                    {
                        filtreModel.TaliDisKaynakKodu = 0;// Dış Kaynağa komisyon tanımlanıyorsa bu değer 0 olacak.
                        return _KomisyonService.TaliAcenteKademeliKomisyonListesiOlustur(filtreModel.DisKaynakList, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.GecerliYil, filtreModel.KademeListe);
                    }
                }

                return false;
            }
            else
            {
                if (filtreModel.KaynakSecim == SatisKanaliDisKaynakFiltre.SatisKanali)
                {
                    if (filtreModel.TVMListe != null && filtreModel.TVMListe.Count > 0)
                    {
                        //Alt Kaynağın , dış kaynak üzerinden aldığı komisyon oranı tanımlanacak ise ekrandan seçilen TaliDisKaynakKodu parametresinin de3ğeri gönderilecek
                        return _KomisyonService.TaliAcenteKomisyonListesiOlustur(filtreModel.TVMListe, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.BaslangicTarihi, filtreModel.Oran);
                    }
                }

                else if (filtreModel.KaynakSecim == SatisKanaliDisKaynakFiltre.DisKaynak)
                {
                    if (filtreModel.DisKaynakList != null && filtreModel.DisKaynakList.Count > 0)
                    {
                        filtreModel.TaliDisKaynakKodu = 0;// Dış Kaynağa komisyon tanımlanıyorsa bu değer 0 olacak.
                        return _KomisyonService.TaliAcenteKomisyonListesiOlustur(filtreModel.DisKaynakList, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.BaslangicTarihi, filtreModel.Oran);
                    }
                }
                return false;
            }
        }

        private List<TaliKomisyonOranListeModel> KomisyonOranListeGetir(TaliKomisyonOranFiltreModel filtreModel, out int toplam)
        {
            toplam = 0;
            if (filtreModel.Kademeli)
            {
                var list = _KomisyonService.TaliAcenteKademeliKomisyonListesi(filtreModel.TVMListe, filtreModel.DisKaynakList, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.GecerliYil);
                var modelList = new List<TaliKomisyonOranListeModel>();
                var temp = (from l in list
                            group l by new { l.TaliTVMKodu, l.DisKaynakKodu, l.BransKodu, l.SigortaSirketKodu } into grp
                            select new
                            {
                                TaliTVMKodu = grp.Key.TaliTVMKodu,
                                DisKaynakKodu = grp.Key.DisKaynakKodu,
                                BransKodu = grp.Key.BransKodu,
                                SigortaSirketKodu = grp.Key.SigortaSirketKodu
                            }).ToList();
                toplam = temp.Count;
                int _skip = filtreModel.Sayfa * filtreModel.Adet >= toplam ? 0 : filtreModel.Sayfa * filtreModel.Adet;
                temp = temp.Skip(_skip).Take(filtreModel.Adet).ToList();
                foreach (var tItem in temp)
                {
                    var kademeList = list.Where(o => o.TaliTVMKodu == tItem.TaliTVMKodu && o.BransKodu == tItem.BransKodu && o.SigortaSirketKodu == tItem.SigortaSirketKodu)
                        .OrderBy(o => o.MinUretim).ToList();
                    var model = new TaliKomisyonOranListeModel()
                    {
                        KomisyonOranId = kademeList[0].KomisyonOranId,
                        TaliUnvani = _ITVMService.GetDetay(Convert.ToInt32(kademeList[0].TaliTVMKodu)).Unvani,
                        TaliDisKaynakUnvani = kademeList[0].DisKaynakKodu.HasValue ? _ITVMService.GetDetay(Convert.ToInt32(kademeList[0].DisKaynakKodu)).Unvani : "",
                        BransAdi = _IBransService.GetBrans(Convert.ToInt32(kademeList[0].BransKodu)).BransAdi,
                        SigortaSirketiAdi = _ISigortaSirketleriService.GetSirket(kademeList[0].SigortaSirketKodu).SirketAdi,
                        GecerliYil = ((DateTime)kademeList[0].GecirlilikBaslangicTarihi).Year
                    };
                    int sira = 0;
                    foreach (var kItem in kademeList)
                    {
                        model.KademeListesi.Add(new KademeliOran()
                        {
                            Sira = sira++,
                            MinUretim = kItem.MinUretim ?? 0,
                            MaxUretim = kItem.MaxUretim ?? 0,
                            Oran = kItem.KomisyonOran ?? 0
                        });
                    }
                    modelList.Add(model);
                }
                return modelList;
            }
            else
            {

                var list = _KomisyonService.TaliAcenteKomisyonListesi(filtreModel.TVMListe, filtreModel.DisKaynakList, filtreModel.TaliDisKaynakKodu, filtreModel.BransListe, filtreModel.SigortaSirketiListe, filtreModel.BaslangicTarihi, filtreModel.Sayfa, filtreModel.Adet, out toplam);
                var modelList = new List<TaliKomisyonOranListeModel>();
                foreach (var item in list)
                {
                    modelList.Add(
                        new TaliKomisyonOranListeModel()
                        {
                            KomisyonOranId = item.KomisyonOranId,
                            TaliUnvani = _ITVMService.GetDetay(Convert.ToInt32(item.TaliTVMKodu)).Unvani,
                            TaliDisKaynakUnvani = item.DisKaynakKodu.HasValue ? _ITVMService.GetDetay(Convert.ToInt32(item.DisKaynakKodu)).Unvani : "",
                            BransAdi = _IBransService.GetBrans(Convert.ToInt32(item.BransKodu)).BransAdi,
                            SigortaSirketiAdi = _ISigortaSirketleriService.GetSirket(item.SigortaSirketKodu).SirketAdi,
                            BaslangicTarihi = (item.GecirlilikBaslangicTarihi ?? new DateTime()).ToString("yyyy-MM-dd"),
                            Oran = item.KomisyonOran ?? 0
                        }
                        );
                }
                return modelList;
            }
        }

        #endregion
    }
}

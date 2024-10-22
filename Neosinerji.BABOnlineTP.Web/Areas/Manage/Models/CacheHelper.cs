using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public static class CacheHelper 
    {
        public static ITVMService _TVMService;
        public static ISigortaSirketleriService _SigortaSirketleriService;
        public static IUrunService _UrunService;
        public static ITUMService _TUMService;
       public static IYetkiService _YetkiService;
       public static IKullaniciFotografStorage _LogoStorage;
       public static ILogService _Log;
       public static IAktifKullaniciService _AktifKullanici;
       public static IBankaSubeleriService _BankaSubeleri;
       public static IKomisyonContext _KomisyonContext;
       public static INeoConnectService _NeoConnectService;
        public static IKullaniciService _KullaniciService;


        //public  CacheHelper(ITVMService tvmService,
        //                     ISigortaSirketleriService sigortaSirketleriService,
        //                     ITVMDokumanStorage storage,
        //                     IUlkeService ulkeService,
        //                     IYetkiService yetkiService,
        //                     ITUMService tumService,
        //                     IKomisyonContext komisyonContext,
        //                     IUrunService urunService,
        //                     IKullaniciFotografStorage logoStorage, INeoConnectService neoConnectService,
        //                     IKullaniciService kullaniciService)
        //{
        //    _TVMService = tvmService;
        //    _SigortaSirketleriService = sigortaSirketleriService;
        //    _UrunService = urunService;
        //    _TUMService = tumService;
        //    _KomisyonContext = komisyonContext;
        //    _YetkiService = yetkiService;
        //    _LogoStorage = logoStorage;
        //    _Log = DependencyResolver.Current.GetService<ILogService>();
        //    _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        //    _BankaSubeleri = DependencyResolver.Current.GetService<IBankaSubeleriService>();
        //    _NeoConnectService = neoConnectService;
        //    _KullaniciService = kullaniciService;
        //}

        //public static List<NeoConnectSifreIslemleriListModel> GetNeoKulYonetimFromCache()
        //{
        //    //_AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        //    //_TVMService = DependencyResolver.Current.GetService<ITVMService>();
        //    //_TUMService = DependencyResolver.Current.GetService<ITUMService>();

        //    //_BankaSubeleri = DependencyResolver.Current.GetService<IBankaSubeleriService>();
        //    //var result = WebCache.Get("neoKulYonetim-cache");
        //    //if (result == null)
        //    //{
        //    //    SirketWebEkranModel model = new SirketWebEkranModel();
        //    //    model.TVMKodu = _AktifKullanici.TVMKodu;
                
        //    //    List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);


        //    //    model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();

        //    //    model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();
        //    //    model.SirketGrupKullaniciListesi = new List<SelectListItem>();

        //    //    model.sifreList = new List<NeoConnectSifreIslemleriListModel>();
        //    //    NeoConnectSifreIslemleriListModel sifreItem = new NeoConnectSifreIslemleriListModel();
        //    //    var baglantilar = _TVMService.GetListNeoConnectKullanicilari(model.TVMKodu);
        //    //    if (baglantilar != null)
        //    //    {
        //    //        foreach (var item in baglantilar)
        //    //        {
        //    //            sifreItem = new NeoConnectSifreIslemleriListModel();
        //    //            sifreItem.TVMKodu = item.TVMKodu;
        //    //            sifreItem.Id = item.Id;
        //    //            sifreItem.TUMKodu = item.TUMKodu;
        //    //            sifreItem.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
        //    //            // sifreItem.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
        //    //            if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
        //    //            {
        //    //                var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

        //    //                if (altTvmKoduBosmu != null)
        //    //                {
        //    //                    sifreItem.AltTVMUnvani = _TVMService.GetDetay(item.AltTVMKodu.Value).Unvani;

        //    //                }
        //    //            }
        //    //            if (item.GrupKodu != null)
        //    //            {
        //    //                sifreItem.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value).GrupAdi;
        //    //            }
        //    //            sifreItem.KullaniciAdi = item.KullaniciAdi;
        //    //            sifreItem.AcenteKodu = item.AcenteKodu;
        //    //            sifreItem.Sifre = item.Sifre;
        //    //            sifreItem.ProxyIpPort = item.ProxyIpPort;
        //    //            model.sifreList.Add(sifreItem);
        //    //        }
        //    //    }
        //    //    result = model;
        //    //    WebCache.Set("neoKulYonetim-cache", result, 20, true);
        //    }
        //    return null;
        //}
        //public static void RemoveNeoKulYonetimFromCache()
        //{
        //    Remove("neoKulYonetim-cache");
        //}
        //public static void Remove(string key)
        //{
        //    WebCache.Remove(key);
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Business.DEMIR;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using Neosinerji.BABOnlineTP.Business.AEGON;
using Neosinerji.BABOnlineTP.Business.METLIFE;
using Neosinerji.BABOnlineTP.Business.RAY;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat;
using Neosinerji.BABOnlineTP.Business.Service.PoliceMuhasebeService;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Business.EUREKO.Kasko;
using Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using Neosinerji.BABOnlineTP.Business.UretimHedefPlanlanan;
using Neosinerji.BABOnlineTP.Business.ERGO.Kasko;
using Neosinerji.BABOnlineTP.Business.ERGO.Trafik;
using Neosinerji.BABOnlineTP.Business.AXA.Trafik;
using Neosinerji.BABOnlineTP.Business.AXA;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.ALLIANZ;
using Neosinerji.BABOnlineTP.Business.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;
using Neosinerji.BABOnlineTP.Business.GULF;
using Neosinerji.BABOnlineTP.Business.GorevTakip;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap;
using Neosinerji.BABOnlineTP.Business.UNICO.Kasko;
using Neosinerji.BABOnlineTP.Business.LilyumKoru;
using Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza;
using Neosinerji.BABOnlineTP.Business.TeklifYapayZeka;

namespace Neosinerji.BABOnlineTP.Web
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        IUnityContainer container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch( Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch
            {
                return new List<object>();
            }
        }
    }
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IMembershipService, MembershipService>();
            container.RegisterType<IAktifKullaniciService, AktifKullaniciService>(new HttpContextLifetimeManager<IAktifKullaniciService>());
            container.RegisterType<IFormsAuthenticationService, FormsAuthenticationService>();
            container.RegisterType<IEMailService, EMailService>();
#if DEBUG
            container.RegisterType<ILogService, MockLogService>();
            container.RegisterType<IMusteriDokumanStorage, MockStroageService>(); // bu kod azure ortamına dosya kaydetmiyor debug olduğu için
            container.RegisterType<IKullaniciFotografStorage, MockStroageService>();
            container.RegisterType<IWEBServiceLogStorage, MockStroageService>();
            container.RegisterType<ITVMDokumanStorage, MockStroageService>();
            container.RegisterType<ITeklifPDFStorage, MockStroageService>();
            container.RegisterType<IPolicePDFStorage, MockStroageService>();
            container.RegisterType<IPoliceTransferStorage, MockStroageService>();
            container.RegisterType<ILogService, TableStorageLogService>();
            container.RegisterType<IAtananIsLogStorage, AtananIsLogStorage>();
            container.RegisterType<IKaskoTeklif, KaskoTeklif>();
            container.RegisterType<IPoliceListesiPDFStorage, PoliceListesiPDFStorage>();

#else
            container.RegisterType<ILogService, TableStorageLogService>(); 
            container.RegisterType<IMusteriDokumanStorage, MusteriDokumanStorage>(); // bu kod azure ortamına dosya kaydediyor - release modda
            container.RegisterType<IKullaniciFotografStorage, KullaniciFotografStorage>();
            container.RegisterType<IWEBServiceLogStorage, WEBServiceLogStorage>();
            container.RegisterType<ITVMDokumanStorage, TVMDokumanStorage>();
            container.RegisterType<ITeklifPDFStorage, TeklifPDFStorage>();
            container.RegisterType<IPolicePDFStorage, PolicePDFStorage>();
            container.RegisterType<IPoliceTransferStorage, PoliceTransferStorage>();
            container.RegisterType<IPoliceListesiPDFStorage, PoliceListesiPDFStorage>();
#endif
            container.RegisterType<IDbContextFactory, DbContextFactory>(new HttpContextLifetimeManager<IDbContextFactory>());
            container.RegisterType<IParametreContext, ParametreContext>();
            container.RegisterType<IUrunParametreleriService, UrunParametreleriService>();
            container.RegisterType<IMusteriContext, MusteriContext>();
            container.RegisterType<ITVMContext, TVMContext>();
            container.RegisterType<ITUMContext, TUMContext>();
            container.RegisterType<ITeklifContext, TeklifContext>();
            container.RegisterType<IProcedureContext, ProcedureContext>();
            container.RegisterType<IAracContext, AracContext>();
            container.RegisterType<ICRContext, CRContext>();
            container.RegisterType<IKullaniciService, KullaniciService>();
            container.RegisterType<IUlkeService, UlkeService>();
            container.RegisterType<ICRService, CRService>();
            container.RegisterType<IUrunService, UrunService>();
            container.RegisterType<IBransService, BransService>();
            container.RegisterType<ISoruService, SoruService>();
            container.RegisterType<IVergiService, VergiService>();
            container.RegisterType<ITeminatService, TeminatService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IMusteriService, MusteriService>();
            container.RegisterType<ITVMService, TVMService>();
            container.RegisterType<ITUMService, TUMService>();
            container.RegisterType<ITanimService, TanimService>();
            container.RegisterType<IAracService, AracService>();
            container.RegisterType<IEPostaService, EPostaService>();
            container.RegisterType<ISigortaSirketleriService, SigortaSirketleriService>();
            container.RegisterType<IKonfigurasyonService, KonfigurasyonService>();
            container.RegisterType<IYetkiService, YetkiService>();
            container.RegisterType<ITeklifService, TeklifService>();
            container.RegisterType<ITrafikTeklif, TrafikTeklif>();
            container.RegisterType<IKrediliHayatTeklif, KrediliHayatTeklif>();
            container.RegisterType<IHDITrafik, HDITrafik>();
            container.RegisterType<IHDIKasko, HDIKasko>();
            container.RegisterType<IHDIDask, HDIDask>();
            container.RegisterType<IHDIKonut, HDIKonut>();
            container.RegisterType<IHDIIsYeri, HDIIsYeri>();
            container.RegisterType<IANADOLUKasko, ANADOLUKasko>();
            container.RegisterType<IANADOLUDask, ANADOLUDask>();
            container.RegisterType<IMAPFRETrafik, MAPFRETrafik>();
            container.RegisterType<IMAPFREKasko, MAPFREKasko>();
            container.RegisterType<IMAPFREProjeKasko, MAPFREProjeKasko>();
            container.RegisterType<IMAPFRETrafik, MAPFRETrafik>();
            container.RegisterType<IMAPFREProjeTrafik, MAPFREProjeTrafik>();
            container.RegisterType<IMAPFREDask, MAPFREDask>();
            container.RegisterType<IMAPFRESorguService, MAPFRESorguService>();
            container.RegisterType<IANADOLUTrafik, ANADOLUTrafik>();
            container.RegisterType<IDEMIRKrediliHayat, DEMIRKrediliHayat>();
            container.RegisterType<ICHARTISSeyehatSaglik, CHARTISSeyehatSaglik>();
            container.RegisterType<IANADOLUIkinciElGaranti, ANADOLUIkinciElGaranti>();
            container.RegisterType<IWebServiceLogListService, WebServiceLogListService>();
            container.RegisterType<IAracDegerService, AracDegerService>();
            container.RegisterType<IAracZipStorage, AracZipStorage>();
            container.RegisterType<IDuyuruService, DuyuruService>();
            container.RegisterType<IDuyuruDokumanStorage, DuyuruDokumanStorage>();
            container.RegisterType<IRaporService, RaporService>();
            container.RegisterType<IPoliceToXML, PoliceToXML>();
            container.RegisterType<IBankaSubeleriService, BankaSubeleriService>();
            container.RegisterType<IParitusService, ParitusService>();
            container.RegisterType<IAEGONTESabitPrimli, AEGONTESabitPrimli>();
            container.RegisterType<IAEGONOdulluBirikim, AEGONOdulluBirikim>();
            container.RegisterType<IAEGONPrimIadeli, AEGONPrimIadeli>();
            container.RegisterType<IAEGONEgitim, AEGONEgitim>();
            container.RegisterType<IAEGONOdemeGuvence, AEGONOdemeGuvence>();
            container.RegisterType<IAEGONKorunanGelecek, AEGONKorunanGelecek>();
            container.RegisterType<IAEGONPrimIadeli2, AEGONPrimIadeli2>();
            container.RegisterType<IHataLogService, HataLogService>();
            container.RegisterType<IWebServisLogService, WebServisLogService>();
            container.RegisterType<IMETLIFEFerdiKazaPlus, METLIFEFerdiKazaPlus>();
            container.RegisterType<IRAYTrafik, RAYTrafik>();
            container.RegisterType<IRAYKasko, RAYKasko>();
            container.RegisterType<ISOMPOJAPANTrafik, SOMPOJAPANTrafik>();
            container.RegisterType<ISOMPOJAPANKasko, SOMPOJAPANKasko>();
            container.RegisterType<ITURKNIPPONKasko, TURKNIPPONKasko>();
            container.RegisterType<ITURKNIPPONDask, TURKNIPPONDask>();
            container.RegisterType<ITURKNIPPONSeyahat, TURKNIPPONSeyahat>();
            container.RegisterType<ITURKNIPPONYabanciSaglik, TURKNIPPONYabanciSaglik>();
            
            container.RegisterType<IEUREKOTrafik, EUREKOTrafik>();
            container.RegisterType<IEUREKOKasko, EUREKOKasko>();
            container.RegisterType<IERGOTrafik, ERGOTrafik>();
            container.RegisterType<IERGOKasko, ERGOKasko>();

            container.RegisterType<IPoliceContext, PoliceContext>();
            container.RegisterType<IPoliceMuhasebeService, PoliceMuhasebeService>();
            container.RegisterType<IPoliceTransferService, PoliceTransferService>();
            container.RegisterType<IPoliceService, PoliceService>();
            container.RegisterType<IHasarService, HasarService>();
            container.RegisterType<IBransUrunService, BransUrunService>();

            container.RegisterType<IKomisyonContext, KomisyonContext>();
            container.RegisterType<IKomisyonService, KomisyonService>();

            container.RegisterType<ITaliPoliceService, TaliPoliceService>();
            container.RegisterType<ITaliAcenteTransferService, TaliAcenteTransferService>();

            container.RegisterType<IDenemeTVMService, DenemeTVMService>();

            container.RegisterType<IPoliceUretimHedefPlanlananService, PoliceUretimHedefPlanlananService>();
            container.RegisterType<IAXATrafik, AXATrafik>();
            container.RegisterType<IAXAKasko, AXAKasko>();
           // container.RegisterType<IAKKasko, AKKasko>();
            container.RegisterType<IALLIANZTrafik, ALLIANZTrafik>();
            container.RegisterType<IGROUPAMATrafik, GROUPAMATrafik>();
            container.RegisterType<IGROUPAMAKasko, GROUPAMAKasko>();
            container.RegisterType<IUNICOKasko, UNICOKasko>();
            container.RegisterType<IHasarStorageService, HasarStorageService>();
            container.RegisterType<ITSSTeklif, TSSTeklif>();
            container.RegisterType<INeoConnectService, NeoConnectService>();
            container.RegisterType<IKesintiTransferService, KesintiTransferService>();
            container.RegisterType<IGULFKasko, GULFKasko>();
            container.RegisterType<IGorevTakipService, GorevTakipService>();
            container.RegisterType<IGorevTakipDokumanStorage, GorevTakipDokumanStorage>();
            container.RegisterType<ICommonService, CommonService>();
            container.RegisterType<IGorevTakipContext, GorevTakipContext>();
            container.RegisterType<IMuhasebe_CariHesapService, Muhasebe_CariHesapService>();
            container.RegisterType<IMuhasebeContext, MuhasebeContext>();
            container.RegisterType<IKaskoTeklif, KaskoTeklif>();
            container.RegisterType<ICariHesapEkstrePDFStorage, CariHesapEkstrePDFStorage>();
            container.RegisterType<ILilyumKoruTeklif, LilyumKoruTeklif>();
            container.RegisterType<IKoruFerdiKaza, KoruFerdiKaza>();
            
            container.RegisterType<IYapayZekaContext, YapayZekaContext>();
            container.RegisterType<ITeklifYapayZekaService, TeklifYapayZekaService>();
             


            return container;
        }
    }

    public class HttpContextLifetimeManager<T> : LifetimeManager, IDisposable
    {
        public override object GetValue()
        {
            return HttpContext.Current.Items[typeof(T).AssemblyQualifiedName];
        }
        public override void RemoveValue()
        {
            HttpContext.Current.Items.Remove(typeof(T).AssemblyQualifiedName);
        }
        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[typeof(T).AssemblyQualifiedName] = newValue;
        }
        public void Dispose()
        {
            RemoveValue();
        }
    }
}

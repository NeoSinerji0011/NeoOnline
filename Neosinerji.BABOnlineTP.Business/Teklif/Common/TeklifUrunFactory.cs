using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.ANADOLU;
using Neosinerji.BABOnlineTP.Business.DEMIR;
using Neosinerji.BABOnlineTP.Business.CHARTIS;
using Neosinerji.BABOnlineTP.Business.RAY;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko;
using Neosinerji.BABOnlineTP.Business.EUREKO.Kasko;
using Neosinerji.BABOnlineTP.Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.ERGO.Trafik;
using Neosinerji.BABOnlineTP.Business.ERGO.Kasko;
using Neosinerji.BABOnlineTP.Business.AXA.Trafik;
using Neosinerji.BABOnlineTP.Business.AXA;
using Neosinerji.BABOnlineTP.Business.UNICO.Kasko;
using Neosinerji.BABOnlineTP.Business.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik;
using Neosinerji.BABOnlineTP.Business.GULF;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;
using Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TeklifUrunFactory
    {
        public static ITeklif AsUrunClass(ITeklif teklif)
        {
            ITeklif result = teklif;
            switch (teklif.UrunKodu)
            {
                case UrunKodlari.TrafikSigortasi:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.HDI:
                                {
                                    IHDITrafik hdi = DependencyResolver.Current.GetService<IHDITrafik>();

                                    Kopyala(teklif, hdi);

                                    return hdi;
                                };
                            case TeklifUretimMerkezleri.MAPFRE:
                                {
                                    IMAPFRETrafik mapfre = DependencyResolver.Current.GetService<IMAPFRETrafik>();

                                    Kopyala(teklif, mapfre);

                                    return mapfre;
                                }
                            case TeklifUretimMerkezleri.ANADOLU:
                                {
                                    IANADOLUTrafik anadolu = DependencyResolver.Current.GetService<IANADOLUTrafik>();
                                    Kopyala(teklif, anadolu);
                                    anadolu.SetClientIPAdres(GetClientIP());
                                    return anadolu;
                                }
                            case TeklifUretimMerkezleri.RAY:
                                {
                                    IRAYTrafik ray = DependencyResolver.Current.GetService<IRAYTrafik>();
                                    ray.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, ray);
                                    return ray;
                                }
                            case TeklifUretimMerkezleri.SOMPOJAPAN:
                                {
                                    ISOMPOJAPANTrafik sompojapan = DependencyResolver.Current.GetService<ISOMPOJAPANTrafik>();
                                    sompojapan.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, sompojapan);

                                    return sompojapan;
                                }
                            case TeklifUretimMerkezleri.EUREKO:
                                {
                                    IEUREKOTrafik eureko = DependencyResolver.Current.GetService<IEUREKOTrafik>();
                                    Kopyala(teklif, eureko);
                                    return eureko;
                                }
                            case TeklifUretimMerkezleri.ERGO:
                                {
                                    IERGOTrafik ergo = DependencyResolver.Current.GetService<IERGOTrafik>();
                                    Kopyala(teklif, ergo);
                                    return ergo;
                                }
                            case TeklifUretimMerkezleri.AXA:
                                {
                                    IAXATrafik axa = DependencyResolver.Current.GetService<IAXATrafik>();
                                    Kopyala(teklif, axa);
                                    return axa;
                                }
                            case TeklifUretimMerkezleri.GROUPAMA:
                                {
                                    IGROUPAMATrafik groupama = DependencyResolver.Current.GetService<IGROUPAMATrafik>();
                                    Kopyala(teklif, groupama);
                                    return groupama;
                                }     
                        }
                    }
                    break;
                case UrunKodlari.KaskoSigortasi:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.HDI:
                                {
                                    IHDIKasko hdi = DependencyResolver.Current.GetService<IHDIKasko>();

                                    Kopyala(teklif, hdi);

                                    return hdi;
                                };
                            case TeklifUretimMerkezleri.MAPFRE:
                                {
                                    IMAPFREKasko mapfre = DependencyResolver.Current.GetService<IMAPFREKasko>();

                                    Kopyala(teklif, mapfre);

                                    return mapfre;
                                }
                            case TeklifUretimMerkezleri.ANADOLU:
                                {
                                    IANADOLUKasko anadolu = DependencyResolver.Current.GetService<IANADOLUKasko>();
                                    anadolu.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, anadolu);

                                    return anadolu;
                                }
                            case TeklifUretimMerkezleri.RAY:
                                {
                                    IRAYKasko ray = DependencyResolver.Current.GetService<IRAYKasko>();
                                    ray.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, ray);

                                    return ray;
                                }
                            case TeklifUretimMerkezleri.SOMPOJAPAN:
                                {
                                    ISOMPOJAPANKasko sompojapan = DependencyResolver.Current.GetService<ISOMPOJAPANKasko>();
                                    sompojapan.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, sompojapan);

                                    return sompojapan;
                                }
                            case TeklifUretimMerkezleri.TURKNIPPON:
                                {
                                    ITURKNIPPONKasko turkNippon = DependencyResolver.Current.GetService<ITURKNIPPONKasko>();

                                    Kopyala(teklif, turkNippon);

                                    return turkNippon;
                                }

                            case TeklifUretimMerkezleri.EUREKO:
                                {
                                    IEUREKOKasko eureko = DependencyResolver.Current.GetService<IEUREKOKasko>();

                                    Kopyala(teklif, eureko);

                                    return eureko;
                                }
                            case TeklifUretimMerkezleri.ERGO:
                                {
                                    IERGOKasko ergo = DependencyResolver.Current.GetService<IERGOKasko>();

                                    Kopyala(teklif, ergo);

                                    return ergo;
                                }

                            case TeklifUretimMerkezleri.AXA:
                                {
                                    IAXAKasko axa = DependencyResolver.Current.GetService<IAXAKasko>();

                                    Kopyala(teklif, axa);

                                    return axa;
                                }

                            //case TeklifUretimMerkezleri.AK:
                            //    {
                            //        IAKKasko ak = DependencyResolver.Current.GetService<IAKKasko>();
                            //        ak.SetClientIPAdres(GetClientIP());
                            //        Kopyala(teklif, ak);

                            //        return ak;
                            //    }
                            case TeklifUretimMerkezleri.UNICO:
                                {
                                    IUNICOKasko unico = DependencyResolver.Current.GetService<IUNICOKasko>();
                                    Kopyala(teklif, unico);
                                    return unico;
                                }
                            case TeklifUretimMerkezleri.GROUPAMA:
                                {
                                    IGROUPAMAKasko groupama = DependencyResolver.Current.GetService<IGROUPAMAKasko>();
                                    Kopyala(teklif, groupama);
                                    return groupama;
                                }
                            case TeklifUretimMerkezleri.GULF:
                                {
                                    IGULFKasko gulf = DependencyResolver.Current.GetService<IGULFKasko>();
                                    gulf.SetClientIPAdres(GetClientIP());
                                    Kopyala(teklif, gulf);
                                    return gulf;
                                }
                        }
                    }
                    break;
                case UrunKodlari.MapfreKasko:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.MAPFRE:
                                {
                                    IMAPFREProjeKasko mapfre = DependencyResolver.Current.GetService<IMAPFREProjeKasko>();

                                    Kopyala(teklif, mapfre);

                                    return mapfre;
                                }
                        }
                    }
                    break;
                case UrunKodlari.MapfreTrafik:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.MAPFRE:
                                {
                                    IMAPFREProjeTrafik mapfre = DependencyResolver.Current.GetService<IMAPFREProjeTrafik>();

                                    Kopyala(teklif, mapfre);

                                    return mapfre;
                                }
                        }
                    }
                    break;
                case UrunKodlari.DogalAfetSigortasi_Deprem:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.HDI:
                                {
                                    IHDIDask hdi = DependencyResolver.Current.GetService<IHDIDask>();

                                    Kopyala(teklif, hdi);

                                    return hdi;
                                };
                            case TeklifUretimMerkezleri.MAPFRE:
                                {
                                    IMAPFREDask mapfre = DependencyResolver.Current.GetService<IMAPFREDask>();

                                    Kopyala(teklif, mapfre);

                                    return mapfre;
                                }
                            case TeklifUretimMerkezleri.ANADOLU:
                                {
                                    IANADOLUDask anadolu = DependencyResolver.Current.GetService<IANADOLUDask>();

                                    Kopyala(teklif, anadolu);

                                    return anadolu;
                                }
                            case TeklifUretimMerkezleri.TURKNIPPON:
                                {
                                    ITURKNIPPONDask tURKNIPPON = DependencyResolver.Current.GetService<ITURKNIPPONDask>();

                                    Kopyala(teklif, tURKNIPPON);

                                    return tURKNIPPON;
                                }
                        }
                    }
                    break;
                case UrunKodlari.KrediHayat:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.DEMIR:
                                {
                                    IDEMIRKrediliHayat demir = DependencyResolver.Current.GetService<DEMIRKrediliHayat>();

                                    Kopyala(teklif, demir);

                                    return demir;
                                };
                        }
                    }
                    break;
                case UrunKodlari.YurtDisiSeyehatSaglik:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.GULF:
                                {
                                    ICHARTISSeyehatSaglik chartis = DependencyResolver.Current.GetService<ICHARTISSeyehatSaglik>();

                                    Kopyala(teklif, chartis);

                                    return chartis;
                                }
                            case TeklifUretimMerkezleri.TURKNIPPON:
                                {
                                    ITURKNIPPONSeyahat turknippon = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();

                                    Kopyala(teklif, turknippon);

                                    return turknippon;
                                }
                                break;
                        }
                    }
                    break;
                case UrunKodlari.IkinciElGaranti:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.ANADOLU:
                                {
                                    IANADOLUIkinciElGaranti anadolu = DependencyResolver.Current.GetService<IANADOLUIkinciElGaranti>();

                                    Kopyala(teklif, anadolu);

                                    return anadolu;
                                };
                        }
                    }
                    break;
                case UrunKodlari.KonutSigortasi_Paket:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.HDI:
                                {
                                    IHDIKonut hdi = DependencyResolver.Current.GetService<IHDIKonut>();

                                    Kopyala(teklif, hdi);

                                    return hdi;
                                };
                        }
                    }
                    break;
                case UrunKodlari.IsYeri:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.HDI:
                                {
                                    IHDIIsYeri hdi = DependencyResolver.Current.GetService<IHDIIsYeri>();

                                    Kopyala(teklif, hdi);

                                    return hdi;
                                };
                        }
                    }
                    break;
                case UrunKodlari.TamamlayiciSaglik:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.TURKNIPPON:
                                {
                                    ITURKNIPPONYabanciSaglik turkNippon = DependencyResolver.Current.GetService<ITURKNIPPONYabanciSaglik>();

                                    Kopyala(teklif, turkNippon);

                                    return turkNippon;
                                };
                        }
                    }
                    break;
                case UrunKodlari.FerdiKazaSigortasi:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.KORU:
                                {
                                    IKoruFerdiKaza lilyumFerdiKaza = DependencyResolver.Current.GetService<IKoruFerdiKaza>();
                                    Kopyala(teklif, lilyumFerdiKaza);
                                    return lilyumFerdiKaza;
                                };
                        }
                    }
                    break;
                case UrunKodlari.SeyahatSigortasi:
                    {
                        switch (teklif.TUMKodu)
                        {
                            case TeklifUretimMerkezleri.TURKNIPPON:
                                {
                                    ITURKNIPPONSeyahat seyahatSaglikTeklif = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
                                    Kopyala(teklif, seyahatSaglikTeklif);
                                    return seyahatSaglikTeklif;
                                };
                        }
                    }
                    break;
            }

            return result;
        }

        public static void Kopyala(ITeklif from, ITeklif to)
        {
            to.GenelBilgiler = from.GenelBilgiler;
            to.Arac = from.Arac;
            to.OdemePlani = from.OdemePlani;
            to.RizikoAdresi = from.RizikoAdresi;
            to.SigortaEttiren = from.SigortaEttiren;
            to.Sigortalilar = from.Sigortalilar;
            to.Sorular = from.Sorular;
            to.Teminatlar = from.Teminatlar;
            to.Vergiler = from.Vergiler;
            to.WebServisCevaplar = from.WebServisCevaplar;
            foreach (var item in from.Log)
                to.Log.Add(item);
        }

        public static string GetClientIP()
        {
            if (System.Web.HttpContext.Current != null)
            {
                string ip = System.Web.HttpContext.Current.Request.UserHostAddress;

                if (String.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (String.IsNullOrEmpty(ip))
                        ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }

            return String.Empty;
        }
    }
    public static class AnadoluKullanimTipiSorguTipi
    {
        public const byte KullanimTarzi = 1;
        public const byte KullanimSekli = 2;
    }
}

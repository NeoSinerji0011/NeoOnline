using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MAPFRESorguService : IMAPFRESorguService
    {
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        IAktifKullaniciService _AktifKullaniciService;

        public MAPFRESorguService(IKonfigurasyonService konfigService, ITVMContext tvmContext, ILogService log, IAktifKullaniciService aktifKullaniciService)
        {
            _KonfigurasyonService = konfigService;
            _TVMContext = tvmContext;
            _Log = log;
            _AktifKullaniciService = aktifKullaniciService;
        }

        public EgmSorguResponse EgmSorgu(int tvmKodu, string plakaIlKodu, string plakaNo, string aracRuhsatSeri, string aracRuhsatNo, string asbisNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            EgmSorguResponse response = null;
            try
            {
                plakaIlKodu = plakaIlKodu.PadLeft(3, '0');
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);

                MapfreSorgu egm = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                egm.Url = konfig[Konfig.MAPFRE_ServiceURL];
                egm.AddParameter(MapfreSorguParametreler.PLAKAILKODU, plakaIlKodu);
                egm.AddParameter(MapfreSorguParametreler.PLAKANO, plakaNo);
                egm.AddParameter(MapfreSorguParametreler.ARACRUHSATSERI, aracRuhsatSeri);

                if (!String.IsNullOrEmpty(aracRuhsatNo))
                {
                    egm.AddParameter(MapfreSorguParametreler.ARACRUHSATNO, aracRuhsatNo);
                }
                else
                {
                    egm.AddParameter(MapfreSorguParametreler.ARACRUHSATNO, asbisNo);
                }
                wslog.BeginLog(egm.GetServiceMessage("egmSorgu", true), WebServisIstekTipleri.EgmSorgu);

                response = egm.CallService<EgmSorguResponse>("egmSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(EgmSorguResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public KimlikSorguResponse KimlikSorgu(int tvmKodu, string kimlikNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            KimlikSorguResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                 MapfreSorgu kimlik=null ;
                if (!String.IsNullOrEmpty(_AktifKullaniciService.TeknikPersonelKodu))
                {
                  kimlik= new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);
                }
                else if (String.IsNullOrEmpty(_AktifKullaniciService.TeknikPersonelKodu))
                {
                    TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { NeosinerjiTVM.NeosinerjiTVMKodu, TeklifUretimMerkezleri.MAPFRE });
                    kimlik = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                }

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                kimlik.Url = konfig[Konfig.MAPFRE_ServiceURL];
                kimlik.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                kimlik.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);

                wslog.BeginLog(kimlik.GetServiceMessage("kimlikSorgu", true), WebServisIstekTipleri.KimlikSorgu);

                response = kimlik.CallService<KimlikSorguResponse>("kimlikSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(KimlikSorguResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", true);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public KimliktenAdresSorguResponse AdresSorgu(int tvmKodu, string kimlikNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            KimliktenAdresSorguResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu adres = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                adres.Url = konfig[Konfig.MAPFRE_ServiceURL];
                adres.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                adres.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                if (kimlikTipi == "TCK" || kimlikTipi == "VRG")
                    adres.AddParameter(MapfreSorguParametreler.ADRESSORGUTIPI, MapfreAdresSorguTipleri.TcKisiAcikAdres);
                else if (kimlikTipi == "YTCK")
                    adres.AddParameter(MapfreSorguParametreler.ADRESSORGUTIPI, MapfreAdresSorguTipleri.YabanciKisiAcikAdres);

                wslog.BeginLog(adres.GetServiceMessage("kimliktenAdresSorgu", true), WebServisIstekTipleri.KimlikSorgu);

                response = adres.CallService<KimliktenAdresSorguResponse>("kimliktenAdresSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(KimliktenAdresSorguResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", true);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public PoliceSorguTrafikResponse PoliceSorguTrafik(int tvmKodu, string kimlikNo, string plakaIlKodu, string plakaNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            PoliceSorguTrafikResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                sorgu.AddParameter(MapfreSorguParametreler.PLAKAILKODU, plakaIlKodu);
                sorgu.AddParameter(MapfreSorguParametreler.PLAKANO, plakaNo);

                wslog.BeginLog(sorgu.GetServiceMessage("policeSorguTrafik", true), WebServisIstekTipleri.PlakaSorgu);

                response = sorgu.CallService<PoliceSorguTrafikResponse>("policeSorguTrafik");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(PoliceSorguTrafikResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public PoliceSorguKaskoResponse PoliceSorguKasko(int tvmKodu, string kimlikNo, string plakaIlKodu, string plakaNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            PoliceSorguKaskoResponse response = null;
            try
            {
                plakaIlKodu = plakaIlKodu.PadLeft(3, '0');
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                sorgu.AddParameter(MapfreSorguParametreler.PLAKAILKODU, plakaIlKodu);
                sorgu.AddParameter(MapfreSorguParametreler.PLAKANO, plakaNo);

                wslog.BeginLog(sorgu.GetServiceMessage("policeSorguKasko", true), WebServisIstekTipleri.PlakaSorgu);

                response = sorgu.CallService<PoliceSorguKaskoResponse>("policeSorguKasko");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(PoliceSorguKaskoResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public OtorizasyonResponse OtorizasyonSorgu(int teklifId)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            mapfre.ZeyilWS response = null;
            mapfre.TanzimWSService ws = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                ITeklifService _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
                ITeklif teklif = _TeklifService.GetTeklif(teklifId);

                MapfreBelgeSorguRequest request = new MapfreBelgeSorguRequest();
                request.userName = _AktifKullaniciService.TeknikPersonelKodu;
                request.passWord = _AktifKullaniciService.MapfreBilgi;
                if (teklif.UrunKodu == UrunKodlari.MapfreTrafik)
                    request.brbBransKodu = "410";
                else
                    request.brbBransKodu = "420";

                request.p_num_poliza = teklif.GenelBilgiler.TUMTeklifNo;
                request.p_num_spto = 0;
                request.p_num_apli = 0;
                request.p_num_spto_apli = 0;
                request.p_tip_emision = "C";

                MapfreBelgeSorguRequest log = request.CloneForLog();
                wslog.BeginLog(log, typeof(MapfreBelgeSorguRequest), WebServisIstekTipleri.Otorizasyon);

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.belgeSorgu(request.userName, request.passWord, request.brbBransKodu, request.p_num_poliza, request.p_num_spto, request.p_num_apli, request.p_num_spto_apli, request.p_tip_emision);
                ws.Dispose();
                if (response != null)
                {
                    wslog.EndLog(response, true, response.GetType());
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }

                if (response != null)
                {
                    OtorizasyonResponse otor = new OtorizasyonResponse();
                    if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                    {
                        otor.Hata = true;
                        otor.Hatalar = new List<string>();
                        foreach (var item in response.hataMesajlari)
                        {
                            otor.Hatalar.Add(item);
                        }
                    }
                    if (response.otorizasyonMesajlari != null && response.otorizasyonMesajlari.Length > 0)
                    {
                        otor.Otorizasyon = true;
                        otor.OtorizasyonMesajlari = new List<string>();
                        foreach (var item in response.otorizasyonMesajlari)
                        {
                            otor.OtorizasyonMesajlari.Add(item);
                        }
                    }

                    return otor;
                }

                throw new Exception("Otorizasyon sorgulaması yapılamadı.");
            }
            catch (Exception ex)
            {
                ws.Abort();
                _Log.Error(ex);
                throw;
            }
        }

        public EskiKaskoBilgiSorguResponse EskiPoliceSorguKasko(string policeNo, string acenteNo, string sirketNo, string yenilemeNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            EskiKaskoBilgiSorguResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.POLICENO, policeNo);
                sorgu.AddParameter(MapfreSorguParametreler.ACENTENO, acenteNo);
                sorgu.AddParameter(MapfreSorguParametreler.SIRKETNO, sirketNo);
                sorgu.AddParameter(MapfreSorguParametreler.YENILEMENO, yenilemeNo);
                sorgu.AddParameter(MapfreSorguParametreler.BRANSNO, "420");
                sorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "ONCKBILGIKASKO");

                wslog.BeginLog(sorgu.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.EskiPoliceSorgu);

                response = sorgu.CallService<EskiKaskoBilgiSorguResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(PoliceSorguTrafikResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public EskiTrafikBilgiSorguResponse EskiPoliceSorguTrafik(string policeNo, string acenteNo, string sirketNo, string yenilemeNo)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            EskiTrafikBilgiSorguResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.POLICENO, policeNo);
                sorgu.AddParameter(MapfreSorguParametreler.ACENTENO, acenteNo);
                sorgu.AddParameter(MapfreSorguParametreler.SIRKETNO, sirketNo);
                sorgu.AddParameter(MapfreSorguParametreler.YENILEMENO, yenilemeNo);
                sorgu.AddParameter(MapfreSorguParametreler.BRANSNO, "410");
                sorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "ONCKBILGITRAFIK");

                wslog.BeginLog(sorgu.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.EskiPoliceSorgu);

                response = sorgu.CallService<EskiTrafikBilgiSorguResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(EskiTrafikBilgiSorguResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public HasarsizlikResponse HasarsizlikSorgu(string kimlikNo, string policeNo, string acenteNo, string sirketNo, string yenilemeNo, string bransKodu)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            HasarsizlikResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                sorgu.AddParameter(MapfreSorguParametreler.POLICENO, policeNo);
                sorgu.AddParameter(MapfreSorguParametreler.ACENTENO, acenteNo);
                sorgu.AddParameter(MapfreSorguParametreler.SIRKETNO, sirketNo);
                sorgu.AddParameter(MapfreSorguParametreler.YENILEMENO, yenilemeNo);
                sorgu.AddParameter(MapfreSorguParametreler.BRANSNO, bransKodu);
                sorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "HSRSZ");

                wslog.BeginLog(sorgu.GetServiceMessage("policeIndSrpSorgu", true), WebServisIstekTipleri.HasarsizlikSorgu);

                response = sorgu.CallService<HasarsizlikResponse>("policeIndSrpSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(HasarsizlikResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public OncekiTescilResponse OncekiTescilSorgu(string kimlikNo, string policeNo, string acenteNo, string sirketNo, string yenilemeNo, string bransKodu)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            OncekiTescilResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu sorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);

                sorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                sorgu.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                sorgu.AddParameter(MapfreSorguParametreler.POLICENO, policeNo);
                sorgu.AddParameter(MapfreSorguParametreler.ACENTENO, acenteNo);
                sorgu.AddParameter(MapfreSorguParametreler.SIRKETNO, sirketNo);
                sorgu.AddParameter(MapfreSorguParametreler.YENILEMENO, yenilemeNo);
                sorgu.AddParameter(MapfreSorguParametreler.BRANSNO, bransKodu);
                sorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "ONCEKITESCIL");

                wslog.BeginLog(sorgu.GetServiceMessage("policeIndSrpSorgu", true), WebServisIstekTipleri.HasarsizlikSorgu);

                response = sorgu.CallService<OncekiTescilResponse>("policeIndSrpSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(OncekiTescilResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public UrunSorguResponse UrunKoduSorgu(UrunKoduSorguModel sorgu)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            UrunSorguResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu mapfreSorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(sorgu.kimlikNo);

                mapfreSorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                mapfreSorgu.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.KIMLIKNO, sorgu.kimlikNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.POLICENO, sorgu.policeNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.ACENTENO, sorgu.acenteNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.SIRKETNO, sorgu.sirketNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.YENILEMENO, sorgu.yenilemeNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.PLAKAILKODU, sorgu.plakaIlKodu);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.PLAKANO, sorgu.plakaNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.MARKAKODU, sorgu.markaKodu);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.MARKATIPI, sorgu.markaTipi);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.KULLANIMTARZI, sorgu.kullanimTarzi);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.MODELYILI, sorgu.modelYili);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.ARACRUHSATSERI, sorgu.aracRuhsatSeri);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.ARACRUHSATSERINO, sorgu.aracRuhsatSeriNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.ASBISREFERANSNO, sorgu.asbisReferansNo);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.YERADEDI, sorgu.yerAdedi);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.ONARIMYERI, sorgu.onarimYeri);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.BRANSNO, "420");
                mapfreSorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "URUNKODUSORGU");

                wslog.BeginLog(mapfreSorgu.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.UrunKoduSorgu);

                response = mapfreSorgu.CallService<UrunSorguResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(UrunSorguResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public KaskoIkameResponse KaskoIkameSorgu(UrunKoduSorguModel sorgu)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            KaskoIkameResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu mapfreSorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                mapfreSorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                mapfreSorgu.AddParameter("brbBransKodu", "420");
                mapfreSorgu.AddParameter("aracRuhsatSeri", sorgu.aracRuhsatSeri);
                mapfreSorgu.AddParameter("aracRuhsatSeriNo", sorgu.aracRuhsatSeriNo);
                mapfreSorgu.AddParameter("asbisReferansNo", sorgu.asbisReferansNo);
                mapfreSorgu.AddParameter("kullanimTarzi", sorgu.kullanimTarzi);
                mapfreSorgu.AddParameter("yerAdedi", sorgu.yerAdedi);
                mapfreSorgu.AddParameter("modelYili", sorgu.modelYili);
                mapfreSorgu.AddParameter("urunKodu", sorgu.urunKodu);
                mapfreSorgu.AddParameter("ikameTuru", sorgu.ikameTuru);
                mapfreSorgu.AddParameter("marka", sorgu.markaKodu);
                mapfreSorgu.AddParameter("model", sorgu.markaTipi);
                mapfreSorgu.AddParameter("rentacar", "H");
                mapfreSorgu.AddParameter("grupPoliceNo", "");
                mapfreSorgu.AddParameter("filoPolice", "H");
                mapfreSorgu.AddParameter("plakaili", sorgu.plakaIlKodu);
                mapfreSorgu.AddParameter("plakano", sorgu.plakaNo);
                mapfreSorgu.AddParameter("sorguTabloKodu", "KONTROL_IKAME_420");
              
                wslog.BeginLog(mapfreSorgu.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.IkameSorgu);

                response = mapfreSorgu.CallService<KaskoIkameResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(KaskoIkameResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public OtorizasyonMesajResponse TeklifOtorizasyonMesaji(string mapfreTeklifNo, string message)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            OtorizasyonMesajResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu mapfreSorgu = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                mapfreSorgu.Url = konfig[Konfig.MAPFRE_ServiceURL];
                mapfreSorgu.AddParameter("policeNo", mapfreTeklifNo);
                mapfreSorgu.AddParameter("zeyilNo", "0");
                mapfreSorgu.AddParameter("polSource", "C");
                mapfreSorgu.AddParameter("message", message);
                mapfreSorgu.AddParameter(MapfreSorguParametreler.SORGUTABLOKODU, "GUNCELLE_OTORIZASYON_MESAJ");

                wslog.BeginLog(mapfreSorgu.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.OtorizasyonMesaj);

                response = mapfreSorgu.CallService<OtorizasyonMesajResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(OtorizasyonMesajResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", false);
                }
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
            }

            return response;
        }

        public HazineYururlulukResponse HazineYururluluk(string bransKodu, string kimlikNo, string plakaIlKodu, string plakaNo, string aracTarifeGrupKodu)
        {
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            HazineYururlulukResponse response = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                MapfreSorgu adres = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", _AktifKullaniciService.TeknikPersonelKodu, _AktifKullaniciService.MapfreBilgi);

                string kimlikTipi = GetKimlikTipi(kimlikNo);
                plakaIlKodu = plakaIlKodu.PadLeft(3, '0');

                adres.Url = konfig[Konfig.MAPFRE_ServiceURL];
                adres.AddParameter("bransKodu", "410");
                adres.AddParameter(MapfreSorguParametreler.KIMLIKTIPI, kimlikTipi);
                adres.AddParameter(MapfreSorguParametreler.KIMLIKNO, kimlikNo);
                adres.AddParameter("plakail", plakaIlKodu);
                adres.AddParameter("plakano", plakaNo);
                adres.AddParameter("aracTarifeGrupKodu", aracTarifeGrupKodu);
                adres.AddParameter("sorguTabloKodu", "HAZINE_YURURLUK_KONTROL");

                wslog.BeginLog(adres.GetServiceMessage("tabloDegerSorgu", true), WebServisIstekTipleri.HazineYururluluk);

                response = adres.CallService<HazineYururlulukResponse>("tabloDegerSorgu");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(HazineYururlulukResponse));
                }
                else
                {
                    wslog.EndLog("Cevap alınamadı", true);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public bool ValidateUser(string userName, string password, out string partajNo)
        {
            partajNo = String.Empty;
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            try
            {
                mapfreutil.UtilitiesWSService ws = new mapfreutil.UtilitiesWSService();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);

                MapfreSorgu validateUser = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", userName.Trim(), password.Trim());
                validateUser.Url = konfig[Konfig.MAPFRE_UtilURL];

                wslog.BeginLog(validateUser.GetServiceMessage("validateUser", true), WebServisIstekTipleri.MapfreLogin);

                ValidateUserResponse response = validateUser.CallService<ValidateUserResponse>("validateUser");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(ValidateUserResponse));

                    if (!String.IsNullOrEmpty(response.hata))
                    {
                        if (response.hata.Contains("A-0-1"))
                            throw new MapfreValidationException("Validasyon başarısız.");
                        throw new MapfreValidationException(response.hata);
                    }

                    if (response.status == "OK")
                    {
                        partajNo = response.cod_agt;
                        return true;
                    }
                }
                else
                    wslog.EndLog("Cevap alınamadı", false);
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
                throw ex;
            }

            return false;
        }

        public bool ValidateUserWithIP(string userName, string password, out string partajNo)
        {
            partajNo = String.Empty;
            IWebServisLogService wslog = DependencyResolver.Current.GetService<WebServisLogService>();
            try
            {
                mapfreutil.UtilitiesWSService ws = new mapfreutil.UtilitiesWSService();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);

                MapfreSorgu validateUser = new MapfreSorgu("0I4eod77GFrsrHNPUeF4wA==", userName.Trim(), password.Trim(), GetClientIP());
                validateUser.Url = konfig[Konfig.MAPFRE_UtilURL];

                wslog.BeginLog(validateUser.GetServiceMessage("validateUserWithIP", true), WebServisIstekTipleri.MapfreLogin);

                ValidateUserResponse response = validateUser.CallService<ValidateUserResponse>("validateUserWithIP");

                if (response != null)
                {
                    wslog.EndLog(response, true, typeof(ValidateUserResponse));

                    if (!String.IsNullOrEmpty(response.hata))
                    {
                        if (response.hata.Contains("A-0-1"))
                            throw new MapfreValidationException("Validasyon başarısız.");
                        throw new MapfreValidationException(response.hata);
                    }

                    if (response.status == "OK")
                    {
                        partajNo = response.cod_agt;
                        return true;
                    }
                }
                else
                    wslog.EndLog("Cevap alınamadı", false);
            }
            catch (Exception ex)
            {
                wslog.EndLog(ex.Message, false);
                _Log.Error(ex);
                throw ex;
            }

            return false;
        }

        private string GetKimlikTipi(string kimlikNo)
        {
            string kimlikTipi = "TCK";
            if (kimlikNo.Length == 10)
                kimlikTipi = "VRG";
            if (kimlikTipi == "TCK" && kimlikNo[0] == '9')
                kimlikTipi = "YTCK";

            return kimlikTipi;
        }

        private static string GetClientIP()
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
}

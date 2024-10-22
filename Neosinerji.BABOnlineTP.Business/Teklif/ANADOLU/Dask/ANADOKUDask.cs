using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUDask : Teklif, IANADOLUDask
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IANADOLUTrafik _Trafik;
        ITVMService _TVMService; 
        [InjectionConstructor]
        public ANADOLUDask(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IANADOLUTrafik trafik,     ITVMService TVMService)
            : base()
        {
            _CRService = crService;
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _Trafik = trafik;
            _TVMService = TVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.ANADOLU;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Veri Hazırlama GENEL
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] {tvmKodu,
                                                                                                                      TeklifUretimMerkezleri.ANADOLU });

                PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA request = new PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA();

                //Swep kullanıcı adı gönderilmelidir.
                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi;

                //Swep şifresi gönderilmelidir.
                request.WEB_SIFRE = servisKullanici.Sifre;

                request.SESSION_ID = "";

                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2,ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                #endregion

                #region Sorgu Parametreleri ekleme

                //Daire yüzölçümü (m2)
                string YuzOlcumu = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, 0).ToString();
                if (!String.IsNullOrEmpty(YuzOlcumu))
                    request.DAIRE_YUZOLCUMU = YuzOlcumu;

                //Bina yapı tarzı kodu gönderilmelidir. Bkz. PR_SWEP_URT_DSK_WS_GET_DASK_BINA_BILGILERI
                string BinaYapiTarzi = teklif.ReadSoru(DASKSorular.Yapi_Tarzi, 0).ToString();
                if (!String.IsNullOrEmpty(BinaYapiTarzi))
                    request.BINA_YAPI_TARZI = "DİĞER YAPILAR";//BinaYapiTarzi;

                string IlceAdi = "";
                string BeldeAdi = "";
                string BELDE_KODU = "";
                string Ilkodu = teklif.RizikoAdresi.IlKodu.ToString();

                DaskIlce daskIlce = _CRService.GetDaskIlce(teklif.RizikoAdresi.IlceKodu.Value);
                DaskBelde daskBelde = _CRService.GetDaskBelde(Convert.ToInt32(teklif.RizikoAdresi.SemtBelde));


                if (daskIlce != null)
                    IlceAdi = daskIlce.IlceAdi;

                if (daskBelde != null)
                    BeldeAdi = daskBelde.BeldeAdi;

                //Belde kodu gönderilmelidir. Bkz. PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI
                PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response BeldeBilgileri = GetBeldeBilgileri(servis,
                                                                                                  "34",
                                                                                                  "BÜYÜKÇEKMECE", teklif);
                if (BeldeBilgileri != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var jsonArray = js.Deserialize<List<object>>(BeldeBilgileri.BELDE);

                    if (jsonArray != null && jsonArray.Count > 0)
                    {
                        for (int i = 0; i < jsonArray.Count; i++)
                        {
                            object[] array = (object[])jsonArray[i];

                            if (array[1].ToString() == "İLÇE MERKEZİ")
                                BELDE_KODU = array[0].ToString();
                        }
                    }
                }

                request.BELDE_KODU = "16710";
                request.RISK_ADRESI = "yasemin sk. no:5";
                #endregion

                #region Service Call

                string SESSION_ID = String.Empty;
                string NET_PRIM = String.Empty;
                string TOPLAM_SIGORTA_BEDELI = String.Empty;
                string ODEME_TIPI = String.Empty;
                string TAKSIT_SAYISI = String.Empty;
                string HATA_KODU = String.Empty;
                string HATA_TEXT = String.Empty;

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);

                PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA_Response response = servis.CallService<PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA_Response>(request);

                #endregion

                #region Varsa Hata Kaydı
                if (response.HATA_KODU != "0")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.HATA_TEXT);
                }
                else
                    this.EndLog(response, true, response.GetType());
                #endregion

                #region Başarı kontrolu
                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }
                #endregion

                #region Teklif kaydı

                #region Genel bilgiler
                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = ANADOLUMessage.ToDecimal(response.NET_PRIM);
                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    this.AddOdemePlani(this.GenelBilgiler.TaksitSayisi.Value, this.GenelBilgiler.BaslamaTarihi,
                                       this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response GetBeldeBilgileri(ANADOLUService servis, string RequestIL_KODU, string requestILCE_ADI, ITeklif teklif)
        {
            PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response response = new PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response();
            try
            {
                #region Belde Bilgisi Getiriliyor

                //PR_SWEP_MUS_WEB_GET_ILCE_KODU_Response IlceKoduServis = GetIlceKodu(servis, RequestIL_KODU, teklif);
                //if (IlceKoduServis != null && IlceKoduServis.HATA_KODU == "0")
                //{
                //    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                //    var jsonArray = js.Deserialize<List<object>>(IlceKoduServis.ILCE_KODU);

                //    object[] values = (object[])jsonArray.FirstOrDefault();

                //    //if (values != null && values.Length > 1)
                //    //{
                //    //    kullanimTipi = values[0].ToString();
                //    //}

                //    ILCE = IlceKoduServis.ILCE_KODU;
                //}

                PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI request = new PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI();
                request.IL = RequestIL_KODU;
                request.ILCE = requestILCE_ADI;
                response = servis.CallService<PR_SWEP_URT_DSK_WS_GET_DASK_BELDE_WITH_IL_KODU_ILCE_ADI_Response>(request);

                #endregion
            }
            catch (Exception)
            {
                #region Hata Log
                this.Hatalar.Add("ANADOLU SİGORTA sisteminde belde bilgisi bulunamadı.");
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;
                #endregion
            }

            return response;
        }
    }
}

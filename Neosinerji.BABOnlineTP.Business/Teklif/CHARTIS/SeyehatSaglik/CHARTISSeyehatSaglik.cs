using System;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Newtonsoft.Json;

namespace Neosinerji.BABOnlineTP.Business.CHARTIS
{
    public class CHARTISSeyehatSaglik : Teklif, ICHARTISSeyehatSaglik
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;

        [InjectionConstructor]
        public CHARTISSeyehatSaglik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService)
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
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.GULF;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                decimal primTutari = 0;
                decimal giderVergisi = 0;
                byte planTipi = 0;
                byte ulkeTipi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, UlkeTipleri.Schengen.ToString()));

                bool schengenMi = false;

                if (ulkeTipi == UlkeTipleri.Diger)
                {
                    planTipi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.PlanTipi, "0"));
                    schengenMi = false;
                }
                else
                {
                    schengenMi = true;
                }

                DateTime BasTarihi = (teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue));
                DateTime BitisTarihi = (teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue));

                TimeSpan fark = BitisTarihi - BasTarihi;

                int seyehatGunSayisi = fark.Days;

                byte kisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));

                int sayac = 103;
                for (int i = 0; i < kisiSayisi; i++)
                {
                    var sigortaliJson = teklif.ReadSoru(sayac, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaliJson))
                    {
                        SeyehatSaglikSigortalilar sigortali = JsonConvert.DeserializeObject<SeyehatSaglikSigortalilar>(sigortaliJson);

                        int sigortaliYas = (TurkeyDateTime.Now.Year - sigortali.DogumTarihi.Year);

                        primTutari = primTutari + _CRService.GetPrimTutari(sigortaliYas, seyehatGunSayisi, schengenMi, planTipi);
                    }
                    sayac++;
                }

                //Kayak teminatı profesyonel ve lisanslı sporcular için kapsam dışıdır. Poliçeye kayak teminatının eklenmesi durumunda %25 sürprim uygulanır.
                if (teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false))
                    primTutari = ((primTutari / 100) * 25) + primTutari;


                //Travel Guard %5 gider vergisi vardır.
                if (!schengenMi)
                    giderVergisi = (primTutari * 0.05m);

                #region Genel bilgiler

                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = BasTarihi;
                this.GenelBilgiler.BitisTarihi = BitisTarihi;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = primTutari;
                this.GenelBilgiler.NetPrim = primTutari;

                this.GenelBilgiler.ToplamVergi = giderVergisi;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                #endregion

                #region Teminatlar
                if (schengenMi)
                {
                    this.AddTeminat(SeyehatSaglikTeminatlar.SeyehatSaglik, (30000 * KurBilgileri.Euro), 0, 0, 0, 0);
                }
                else
                {
                    if (planTipi == SeyehatPlanlari.Gumus)
                        this.AddTeminat(SeyehatSaglikTeminatlar.SeyehatSaglik, (25000 * KurBilgileri.Dolar), 0, 0, 0, 0);
                    if (planTipi == SeyehatPlanlari.Altin)
                        this.AddTeminat(SeyehatSaglikTeminatlar.SeyehatSaglik, (50000 * KurBilgileri.Dolar), 0, 0, 0, 0);
                    if (planTipi == SeyehatPlanlari.Platin)
                        this.AddTeminat(SeyehatSaglikTeminatlar.SeyehatSaglik, (75000 * KurBilgileri.Dolar), 0, 0, 0, 0);
                }

                #endregion

                #region Vergiler

                if (schengenMi)
                {
                    //Schengen Vergi Yoktur.
                }
                else
                    this.AddVergi(SeyehatSaglikVergiler.GiderVergisi, giderVergisi);

                #endregion

                    #region Ödeme Planı

                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));

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
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.TeklifNo, this.GenelBilgiler.TVMKodu);
                Random rnd = new Random();
                int policeNo = rnd.Next((int.MaxValue / 2), int.MaxValue);

                this.GenelBilgiler.TUMPoliceNo = policeNo.ToString();
                this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                this.GenelBilgiler.TanzimTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BaslamaTarihi = anaTeklif.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = anaTeklif.GenelBilgiler.BitisTarihi;

                //Muhasebe aktarımı
                //this.SendMuhasebe();

                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                //Poliçe yazılıyor
                SeyahatSaglikTeklif seyahat = new SeyahatSaglikTeklif(this.GenelBilgiler.TeklifId);
                seyahat.CreatePolicePDF(this);
            }
            catch (Exception ex)
            {
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }


    }
}

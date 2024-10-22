using System;
using System.Data;
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
using System.IO;
using System.Globalization;
using Neosinerji.BABOnlineTP.Business.aegon;


namespace Neosinerji.BABOnlineTP.Business.AEGON
{
    public class AEGONTESabitPrimli : Teklif, IAEGONTESabitPrimli
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
        public AEGONTESabitPrimli(ICRService crService,
                                    ICRContext crContext,
                                    IMusteriService musteriService,
                                    IAracContext aracContext,
                                    IKonfigurasyonService konfigurasyonService,
                                    ITVMContext tvmContext,
                                    ILogService log,
                                    ITeklifService teklifService)
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
                return TeklifUretimMerkezleri.AEGON;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region REQUEST

                AegonTERequest request = new AegonTERequest();

                request.TeklifNo = teklif.TeklifNo;
                request.teklifTarihi = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                request.satisKanali = "5";

                //Sigorta Başlangıç Tarihi
                DateTime SigortaBaslangicTar = teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                TeklifSigortali teklifSigortali = teklif.Sigortalilar.FirstOrDefault();

                if (teklifSigortali != null)
                {
                    MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);

                    //Cinsiyeti
                    request.cinsiyet = sigortali.Cinsiyet;
                    request.musteriAdSoyad = String.Format("{0} {1}", sigortali.AdiUnvan, sigortali.SoyadiUnvan);

                    //pGVO
                    switch (sigortali.CiroBilgisi)
                    {
                        case "1": request.pGVO = 15.0; break;
                        case "2": request.pGVO = 20.0; break;
                        case "3": request.pGVO = 27.0; break;
                        case "4": request.pGVO = 35.0; break;
                        case "5": request.pGVO = 27.0; break;
                        default: request.pGVO = 27.0; break;
                    }


                    //Doğum Tarihi, Sigorta Başlangıç tarihi ve Yaş
                    if (sigortali.DogumTarihi.HasValue)
                    {
                        request.dogTar = sigortali.DogumTarihi.Value.ToString("dd.MM.yyyy");
                        request.sigBasTar = SigortaBaslangicTar.ToString("dd.MM.yyyy");
                        request.yas = AegonYasHesapla(sigortali.DogumTarihi.Value, SigortaBaslangicTar);
                    }
                }

                //Sigorta Süresi
                string SigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(SigortaSuresi))
                    request.sigortaSure = Convert.ToInt32(SigortaSuresi);

                //Prim Odeme Donemi
                switch (teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty))
                {
                    case "1": request.odeDonem = "Aylık"; break;
                    case "2": request.odeDonem = "3 Aylık"; break;
                    case "3": request.odeDonem = "6 Aylık"; break;
                    case "4": request.odeDonem = "Yıllık"; break;
                }

                //Para Birimi
                switch (teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty))
                {
                    case "1": request.parabirimi = "EUR"; break;
                    case "2": request.parabirimi = "USD"; break;
                }

                string AnaTeminatKodu = teklif.ReadSoru(TESabitPrimliSorular.AnaTeminat, String.Empty);
                switch (AnaTeminatKodu)
                {
                    case "1": request.anaTeminat = "VEF"; break;
                    case "2": request.anaTeminat = "VKH"; break;
                }


                int _hesaplamaSecenegi = (int)teklif.ReadSoru(TESabitPrimliSorular.HesaplamaSecenegi, decimal.Zero);
                switch (_hesaplamaSecenegi)
                {
                    case 1: //Ana Teminat
                        request.HesaplamaSecenegi = "AT";
                        {
                            switch (request.anaTeminat)
                            {
                                case "VEF":
                        TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                        if (vefat != null && vefat.TeminatBedeli.HasValue)
                                        request.anaTeminatTutar = Convert.ToDouble(vefat.TeminatBedeli.Value);
                        break;
                                case "VKH":
                        TeklifTeminat vkh = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                        if (vkh != null && vkh.TeminatBedeli.HasValue)
                                        request.anaTeminatTutar = Convert.ToDouble(vkh.TeminatBedeli.Value);
                        break;
                }
                        }

                        break;

                    case 2: //Yıllık Prim
                        request.HesaplamaSecenegi = "YP";
                        {
                            request.anaTeminatTutar = Convert.ToDouble(teklif.ReadSoru(TESabitPrimliSorular.YillikPrimTutari, decimal.Zero));
                        }
                        break;
                }



                //EkTeminatlar
                string EkTeminatlar = "";
                foreach (TeklifTeminat teminat in teklif.Teminatlar)
                {
                    if (teminat.TeminatBedeli.HasValue)
                    {
                        decimal bedel = teminat.TeminatBedeli.Value;

                        if (!String.IsNullOrEmpty(EkTeminatlar))
                            EkTeminatlar += ",";

                        switch (teminat.TeminatKodu)
                        {
                            case TESabitPrimliTeminatlar.KritikHastaliklar: EkTeminatlar += "KH=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.TamVeDaimiMaluliyet: EkTeminatlar += "HKTDM=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.KazaSonucuVefat: EkTeminatlar += "KVEF=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.TopluTasimaAraclariKSV: EkTeminatlar += "TTAV=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.MaluliyetYillikDestek: EkTeminatlar += "MALYD=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati:
                                EkTeminatlar += "HTED=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                            case TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme:
                                EkTeminatlar += "KHYT=" + bedel.ToString(CultureInfo.InvariantCulture); break;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(EkTeminatlar))
                    request.ekTeminatKodlari = EkTeminatlar;
                else
                    request.ekTeminatKodlari = "KH=0";

                #endregion

                #region RESPONSE | LOG

                this.BeginLog(request, typeof(AegonTERequest), WebServisIstekTipleri.Teklif);

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);

                aegon.Service1 servis = new aegon.Service1();
                servis.Timeout = 150000;
                servis.Url = serviceURL;


                aegon.TEPrimInfo[] prim = servis.TuruncuElma_TE02(request.TeklifNo, request.cinsiyet, request.dogTar, request.yas, request.sigBasTar,
                                                                  request.sigortaSure, request.odeDonem, request.parabirimi, request.anaTeminat,
                                                                  request.anaTeminatTutar, request.ekTeminatKodlari, request.pGVO, request.teklifTarihi,
                                                                  request.musteriAdSoyad, AegonCommon.FirmaKisaAdi, request.HesaplamaSecenegi);

                if (prim.Length == 0 && prim[0] == null)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    this.EndLog("Web Servise yanıt dönmedi", false);
                    this.AddHata("Web Servise yanıt dönmedi");
                    return;
                }

                aegon.TEPrimInfo response = prim[0];

                if (!String.IsNullOrEmpty(response.error))
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    string hata = AegonHelper.AegonReplace(response.error);
                    this.EndLog(hata, false);
                    this.AddHata(hata);
                    return;
                }
                else
                    this.EndLog(response, true, response.GetType());


                #endregion

                #region SUCCESS

                CultureInfo turkey = new CultureInfo("tr-TR");

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = SigortaBaslangicTar;
                this.GenelBilgiler.BitisTarihi = SigortaBaslangicTar.AddYears(request.sigortaSure);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Now.AddDays(30);

                if (!String.IsNullOrEmpty(response.topYillikPrim))
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.topYillikPrim, CultureInfo.InvariantCulture);

                if (!String.IsNullOrEmpty(response.topDonemselPrim))
                    this.GenelBilgiler.NetPrim = Convert.ToDecimal(response.topDonemselPrim, CultureInfo.InvariantCulture);

                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.Doviz;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;


                #region Sorular

                //YENİ SORULAR
                this.AddSoru(TESabitPrimliSorular.VergiAvantajiSonrasiPrimMaliyeti, Convert.ToDecimal(response.vas_prim_maliyeti, CultureInfo.InvariantCulture));
                this.AddSoru(TESabitPrimliSorular.PrimdenVergiAvantaji, Convert.ToDecimal(response.primdenVergiAvantaji, CultureInfo.InvariantCulture));
                this.AddSoru(TESabitPrimliSorular.SureBoyuncaOdenecekToplamP, Convert.ToDecimal(response.sbo_toplam_prim, CultureInfo.InvariantCulture));

                #endregion

                #region Teminatlar

                // ANA TEMINATLAR.
                switch (AnaTeminatKodu)
                {
                    case "1":
                        TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                        if (vefat != null && vefat.TeminatBedeli.HasValue &&
                            !String.IsNullOrEmpty(response.atTemTutar) &&
                            !String.IsNullOrEmpty(response.atDonemselPrim) &&
                            !String.IsNullOrEmpty(response.atYillikPrim) &&
                            response.atDonemselPrim != "0" &&
                            response.atYillikPrim != "0")
                        {
                            this.AddTeminat(TESabitPrimliTeminatlar.Vefat,
                                            Convert.ToDecimal(response.atTemTutar, CultureInfo.InvariantCulture), 0,
                                            Convert.ToDecimal(response.atDonemselPrim, CultureInfo.InvariantCulture),
                                            Convert.ToDecimal(response.atYillikPrim, CultureInfo.InvariantCulture), 0);
                        }
                        break;
                    case "2":
                        TeklifTeminat vefat_KH = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                        if (vefat_KH != null && vefat_KH.TeminatBedeli.HasValue &&
                            !String.IsNullOrEmpty(response.atTemTutar) &&
                            !String.IsNullOrEmpty(response.atDonemselPrim) &&
                            !String.IsNullOrEmpty(response.atYillikPrim) &&
                            response.atDonemselPrim != "0" &&
                            response.atYillikPrim != "0")
                        {
                            this.AddTeminat(TESabitPrimliTeminatlar.Vefat_KritikHastalik,
                                            Convert.ToDecimal(response.atTemTutar, CultureInfo.InvariantCulture), 0,
                                            Convert.ToDecimal(response.atDonemselPrim, CultureInfo.InvariantCulture),
                                            Convert.ToDecimal(response.atYillikPrim, CultureInfo.InvariantCulture), 0);
                        }
                        break;
                }


                //KritikHastaliklar
                TeklifTeminat kritikH = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KritikHastaliklar).FirstOrDefault();
                if (!String.IsNullOrEmpty(response.KH_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.KH_ektemDonemselPrim) &&
                    response.KH_ektemYillikPrim != "0" &&
                    kritikH != null &&
                    kritikH.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.KritikHastaliklar,
                                    Convert.ToDecimal(response.KH_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.KH_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.KH_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }

                //TamVeDaimiMaluliyet
                TeklifTeminat TamVDMAl = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TamVeDaimiMaluliyet).FirstOrDefault();
                if (!String.IsNullOrEmpty(response.HKTDM_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.HKTDM_ektemDonemselPrim) &&
                    response.HKTDM_ektemYillikPrim != "0" &&
                    TamVDMAl != null &&
                    TamVDMAl.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.TamVeDaimiMaluliyet,
                                    Convert.ToDecimal(response.HKTDM_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.HKTDM_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.HKTDM_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }

                //KazaSonucuVefat
                TeklifTeminat KazaSonucuV = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucuVefat).FirstOrDefault();
                if (!String.IsNullOrEmpty(response.KVEF_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.KVEF_ektemDonemselPrim) &&
                    response.KVEF_ektemYillikPrim != "0" &&
                    KazaSonucuV != null &&
                    KazaSonucuV.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.KazaSonucuVefat,
                                    Convert.ToDecimal(response.KVEF_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.KVEF_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.KVEF_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }

                //TopluTasimaAraclariKSV
                TeklifTeminat TopluTAKSV = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TopluTasimaAraclariKSV).FirstOrDefault();
                if (!String.IsNullOrEmpty(response.TTAV_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.TTAV_ektemDonemselPrim) &&
                    response.TTAV_ektemYillikPrim != "0" &&
                    TopluTAKSV != null &&
                    TopluTAKSV.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.TopluTasimaAraclariKSV,
                                    Convert.ToDecimal(response.TTAV_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.TTAV_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.TTAV_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }

                //MaluliyetYillikDestek
                TeklifTeminat maluliyetYD = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                if (!String.IsNullOrEmpty(response.MALYD_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.MALYD_ektemDonemselPrim) &&
                    response.MALYD_ektemYillikPrim != "0" &&
                    maluliyetYD != null &&
                    maluliyetYD.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.MaluliyetYillikDestek,
                                    Convert.ToDecimal(response.MALYD_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.MALYD_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.MALYD_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }


                //Kaza Sonucu Tedavi Masrafları Ek Teminatı 
                TeklifTeminat kazaSncTdvMsrflrEkT = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati)
                                                                            .FirstOrDefault();
                if (!String.IsNullOrEmpty(response.HTED_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.HTED_ektemDonemselPrim) &&
                    response.HTED_ektemYillikPrim != "0" &&
                    kazaSncTdvMsrflrEkT != null &&
                    kazaSncTdvMsrflrEkT.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati,
                                    Convert.ToDecimal(response.HTED_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.HTED_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.HTED_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }


                //Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Teminatı
                TeklifTeminat kazaSonucuHastanedeYatarakTedaviDHOT = teklif.Teminatlar
                                                                     .Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme)
                                                                     .FirstOrDefault();
                if (!String.IsNullOrEmpty(response.KHYT_ektemYillikPrim) &&
                    !String.IsNullOrEmpty(response.KHYT_ektemDonemselPrim) &&
                    response.HTED_ektemYillikPrim != "0" &&
                    kazaSonucuHastanedeYatarakTedaviDHOT != null &&
                    kazaSonucuHastanedeYatarakTedaviDHOT.TeminatBedeli.HasValue)
                {
                    this.AddTeminat(TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme,
                                    Convert.ToDecimal(response.KHYT_teminatTutar, CultureInfo.InvariantCulture), 0,
                                    Convert.ToDecimal(response.KHYT_ektemDonemselPrim, CultureInfo.InvariantCulture),
                                    Convert.ToDecimal(response.KHYT_ektemYillikPrim, CultureInfo.InvariantCulture), 0);
                }

                #endregion

                #region Ödeme Planı

                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));

                #endregion

                #region WebServiceResponse

                this.AddWebServisCevap(Common.WebServisCevaplar.SurumNo, response.surum_bilgi);

                //Boş Geldiğinde kaydetmiyor ve raporlama ekranında sorun yaratıyor.
                if (String.IsNullOrEmpty(response.tibbi_tetkik_sonucu))
                {
                    response.tibbi_tetkik_sonucu = " ";
                }

                //FORMATI DÜZELTİLİYOR.
                response.tibbi_tetkik_sonucu = AegonHelper.AegonReplace(response.tibbi_tetkik_sonucu);

                this.AddWebServisCevap(Common.WebServisCevaplar.TibbiTetkikSonucu, response.tibbi_tetkik_sonucu);

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

        public static int AegonYasHesapla(DateTime dogumTarihi, DateTime SigortaBasTar)
        {
            int yas = 0;

            //Hesaplama//
            int dogumTarihiYil = dogumTarihi.Year;
            int bastarihiYil = SigortaBasTar.Year;
            int dogumTarihigun = dogumTarihi.Day;

            int dogumTarihiAy = dogumTarihi.Month;
            int bastarAy = SigortaBasTar.Month;
            int bastargun = SigortaBasTar.Day;

            int bitdeger = 0;

            if (bastarAy < dogumTarihiAy)
                bitdeger = 1;
            else if (bastarAy == dogumTarihiAy && bastargun < dogumTarihigun)
                bitdeger = 1;

            yas = bastarihiYil - dogumTarihiYil - bitdeger;

            return yas;
        }
    }
}


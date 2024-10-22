using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using ilceler = Neosinerji.BABOnlineTP.Business.HDI.HDIIlcelerResponse;
using beldeler = Neosinerji.BABOnlineTP.Business.HDI.HDIBeldelerResponse;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public class HDIIsYeri : Teklif, IHDIIsYeri
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IUlkeService _UlkeService;
        ITVMService _TVMService; 

        [InjectionConstructor]
        public HDIIsYeri(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
                        ITVMContext tvmContext, ILogService log, ITeklifService teklifService,ITVMService TVMService )
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _TVMService=TVMService; 
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.HDI;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            HDIIsYeriResponse response = null;
            try
            {
                #region Teklif Hazırlama
                HDIIsYeriRequest request = this.TeklifRequest(teklif);
                #endregion

                #region Servis call,

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(requestXML, WebServisIstekTipleri.Teklif);

                response = request.HttpRequest<HDIIsYeriResponse>(serviceURL, requestXML, out responseXML);

                if (response == null)
                {
                    this.AddHata("Teklif bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumMesaj);
                    if (response.OTORIZASYONLISTE.Count > 0)
                    {
                        this.AddHata("Teklif isteği otorizasyona düştü");
                        foreach (var otorize in response.OTORIZASYONLISTE)
                            this.AddHata(String.Format("Sebep Kodu : {0}, Aciklama : {1}, Yetkili : {2}", otorize.SebepKodu, otorize.Aciklama, otorize.Yetkili));
                    }
                }
                else
                    this.EndLog(responseXML, true);

                #endregion

                #region Başarı Kontrolu
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
                this.GenelBilgiler.BaslamaTarihi = teklif.GenelBilgiler.BaslamaTarihi;// TurkeyDateTime.Now;//HDIMessage.ToDateTime(response.PolBasTar);
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = HDIMessage.ToDecimal(response.WSBRUT);
                this.GenelBilgiler.NetPrim = HDIMessage.ToDecimal(response.WSNPRM);
                decimal GiderVergisi = HDIMessage.ToDecimal(response.WSVERG);
                decimal YSV = HDIMessage.ToDecimal(response.WSFON);
                this.GenelBilgiler.ToplamVergi = GiderVergisi + YSV;
                this.GenelBilgiler.TaksitSayisi = (byte)(response.Taksitler != null ? response.Taksitler.Count() : 1);
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;

                #endregion

                #region Vergiler

                this.AddVergi(IsYeriVergileri.GiderVergisi, GiderVergisi);
                this.AddVergi(IsYeriVergileri.YanginSigortaVergisi, YSV);

                #endregion

                #region Teminatlar

                //ANA TEMİNATLAR
                foreach (var teminat in response.ANATeminatListe)
                {
                    if (teminat.ANAWSSIK2 == "X")
                        switch (teminat.ANAWSETK2)
                        {
                            case HDIKonutTeminatKod.BINA_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.BinaYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.EMTEA_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.EmteaYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.MAKINA_VE_TECHIZAT_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.MakinaVeTechizat, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.DEMIRBAS_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.DemirbasYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KASA_MUHTEVIYATI_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.KasaMuhteviyatYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.SAHIS_MALLARI_YANGIN_3:
                                this.AddTeminat(IsYeriTeminatlar.SahisMallariYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.DEKORASYON_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.DekorasyonYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.TEMELLER_YANGIN:
                                this.AddTeminat(IsYeriTeminatlar.TemellerYangin, HDIMessage.ToDecimal(teminat.ANAWSTEM2), 0, 0, 0, 0); break;
                        }
                }


                //EK TEMİNATLAR
                foreach (var teminat in response.EkTeminatListe)
                {
                    if (teminat.WSSIK2 == "X")
                        switch (teminat.WSETK2)
                        {
                            case HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_BINA:
                                this.AddTeminat(IsYeriTeminatlar.DepremYanardagPuskurmesiBina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_MUHTEVIYAT_ESYA:
                                this.AddTeminat(IsYeriTeminatlar.DepremYanardagPuskurmesiMuhteviyat, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.GLKHHKNH_VE_TEROR:
                                this.AddTeminat(IsYeriTeminatlar.GLKHHKNHTeror, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.BINA_EK_TEMINAT:
                                this.AddTeminat(IsYeriTeminatlar.EkTeminatBina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.MUHTEVIYAT_EKTEMINAT:
                                this.AddTeminat(IsYeriTeminatlar.EkTeminatMuhteviyat, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.SEL_VE_SU_BASMASI:
                                this.AddTeminat(IsYeriTeminatlar.SelVeSuBaskini, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KAR_AGIRLIGI:
                                this.AddTeminat(IsYeriTeminatlar.KarAgirligi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.YER_KAYMASI:
                                this.AddTeminat(IsYeriTeminatlar.YerKaymasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.FIRTINA:
                                this.AddTeminat(IsYeriTeminatlar.Firtina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KOMSULUK_MALI_SORUMLULUK_YANGIN_SU_DUMAN:
                                this.AddTeminat(IsYeriTeminatlar.KomsulukMaliSorumlulukYanginDahiliSuDuman, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KOMSULUK_MALI_SORUMLULUK_TEROR:
                                this.AddTeminat(IsYeriTeminatlar.KomsulukMaliSorumlulukTeror, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KIRACI_MALI_SORUMLULUK_YANGIN_DUMAN:
                                this.AddTeminat(IsYeriTeminatlar.KiraciMaliSorumlulukYanginDahiliSuDuman, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KIRACI_MALI_SORUMLULUK_TEROR:
                                this.AddTeminat(IsYeriTeminatlar.KiraciMaliSorumlulukTeror, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KIRA_KAYBI:
                                this.AddTeminat(IsYeriTeminatlar.KiraKaybi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.IS_DURMASI:
                                this.AddTeminat(IsYeriTeminatlar.YazDurmasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.ASISTAN:
                                this.AddTeminat(IsYeriTeminatlar.AsistanHizmeti, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.HIRSIZLIK:
                                this.AddTeminat(IsYeriTeminatlar.Hirsizlik, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.KASA_HIRSIZLIK:
                                this.AddTeminat(IsYeriTeminatlar.KasaHirsizlik, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.TASINAN_PARA_BEHER_SEFER_LIMIT:  //TASINAN_PARA_YILLIK_TOPLAM_LIMIT AYNI TEMİNATLAR DÖNÜYOR
                                if (teminat.WSTEA2 == "TAŞINAN PARA Beher Sefer için Limit")
                                    this.AddTeminat(IsYeriTeminatlar.TasinanParaBeherSeferIcinLimit, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                                if (teminat.WSTEA2 == "TAŞINAN PARA Yıllık Toplam Limit")
                                    this.AddTeminat(IsYeriTeminatlar.TasinanParaYillikToplamLimit, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.EMNIYET_SUISTIMAL_KISI_BASINA_YILLIK:
                                this.AddTeminat(IsYeriTeminatlar.EmniyetiSuistimalKisiBasina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.FERDI_KAZA: //Ferdi kaza tek bir teminatta donuyor
                                this.AddTeminat(IsYeriTeminatlar.FerdiKaza, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                                this.AddTeminat(IsYeriTeminatlar.FerdiKazaOlum, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                                this.AddTeminat(IsYeriTeminatlar.FerdiKazaSurekliSakatlik, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.CAM_KIRILMASI:
                                this.AddTeminat(IsYeriTeminatlar.CamKirilmasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.ISVEREN_MALI_MESULIYET_KAZA_BASI_BEDENI: //İŞVEREN MALİ MESULİYET Kişi Başına Bedeni Aynı teminatlar
                                if (teminat.WSTEA2 == "İŞVEREN MALİ MESULİYET Kişi Başına Bedeni")
                                {
                                    this.AddTeminat(IsYeriTeminatlar.IsVerenMaliMEsuliyetKisiBasinaBedeni, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                                }
                                else if (teminat.WSTEA2 == "İŞVEREN MALİ MESULİYET Kaza Başına Bedeni")
                                {
                                    this.AddTeminat(IsYeriTeminatlar.IsVerenMaliMesuliyetKazaBasinaBedeni, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                                }
                                break;
                            case HDIKonutTeminatKod.SAHIS_MALI_SORUMLULUK_KAZA_BASI_MADDI: //3.ŞAHIS MALİ SORUMLULUK Kişi Başına Bedeni Aynı teminatlar
                                if (teminat.WSTEA2 == "3.ŞAHIS MALİ SORUMLULUK Kaza Başına Bedeni")
                                {
                                    this.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                                }
                                else if (teminat.WSTEA2 == "3.ŞAHIS MALİ SORUMLULUK Kaza Başına Maddi")
                                {
                                    this.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaMaddi3, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                                }
                                else if (teminat.WSTEA2 == "3.ŞAHIS MALİ SORUMLULUK Kişi Başına Bedeni")
                                {
                                    this.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKisiBasinaBedeni3, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                                }
                                break;
                            case HDIKonutTeminatKod.HUKUKSAL_KORUMA:
                                this.AddTeminat(IsYeriTeminatlar.HukuksalKoruma, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.MAKINA_KIRILMASI:
                                this.AddTeminat(IsYeriTeminatlar.MakinaKirilmasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.ELEKTRONIK_CIHAZ:
                                this.AddTeminat(IsYeriTeminatlar.ElektronikCihazSigortasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                            case HDIKonutTeminatKod.MEDLINE:
                                this.AddTeminat(IsYeriTeminatlar.Medline, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0); break;
                        }
                }

                #endregion

                #region Ödeme Planı

                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    // ==== HDI Response taksit sayısı ve miktarlarını donuyor. ==== //
                    if (response.Taksitler != null && response.Taksitler.Count > 1)
                    {
                        int sayac = 1;
                        foreach (var item in response.Taksitler)
                        {
                            this.AddOdemePlani(sayac, DateTime.ParseExact(item.TaksitVade, "ddMMyyyy", CultureInfo.CurrentCulture),
                                               HDIMessage.ToDecimal(item.TaksitTutar), (teklif.GenelBilgiler.OdemeTipi ?? 0));
                            sayac++;
                        }
                    }
                    else
                        this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
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
            HDIIsYeriResponse response = null;
            try
            {
                #region Teklif Hazırlama

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                HDIIsYeriRequest request = this.TeklifRequest(teklif);
                string referansNo = this.ReadWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, String.Empty);

                //P=Poliçe prim sorma,O=Onaylı poliçe yaz,K=Kontrol (Poliçe ve Onay için)
                request.IstekTip = "O";



                #endregion

                #region Servis call

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);
                string requestXML = request.ToString();
                string responseXML = String.Empty;


                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);


                //Kart Bilgileri log dosyasına eklenmiyor.
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    string kk = String.Format("{0}{1}", odeme.KrediKarti.KartNo, odeme.KrediKarti.CVC);
                    if (!String.IsNullOrEmpty(kk))
                    {
                        char[] kkVal = new char[] { kk[7], kk[12], kk[10], kk[2], kk[14], kk[17], kk[5], kk[0], kk[15], kk[9],
                                                kk[4], kk[18], kk[1], kk[13], kk[8], kk[16], kk[11], kk[3], kk[6] };

                        request.TckR = String.Format("{0}{1}{2}", new string(kkVal), odeme.KrediKarti.SKA, odeme.KrediKarti.SKY);
                    }
                    else
                    {
                        request.TckR = "";
                    }
                }
                else
                {
                    request.TckR = "";
                }
                response = request.HttpRequest<HDIIsYeriResponse>(serviceURL, requestXML, out responseXML);

                if (response == null)
                {
                    this.AddHata("Poliçe bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumMesaj);
                }
                else
                    this.EndLog(responseXML, true);

                #endregion

                #region Poliçe Kaydı

                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.PoliceNo;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    if (response.Taksitler != null && response.Taksitler.Count() > 0)
                        if (this.GenelBilgiler.TaksitSayisi != response.Taksitler.Count())
                        {
                            //Yeni taksit miktarı güncelleniyor.
                            this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                            this.ResetOdemePlani();

                            int sayac = 1;
                            foreach (var item in response.Taksitler)
                            {
                                this.AddOdemePlani(sayac, DateTime.ParseExact(item.TaksitVade, "ddMMyyyy", CultureInfo.CurrentCulture),
                                                   HDIMessage.ToDecimal(item.TaksitTutar), (teklif.GenelBilgiler.OdemeTipi ?? 0));
                                sayac++;
                            }
                        }

                    this.PolicePDF();
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                this.GenelBilgiler.TeklifOdemePlanis = this.OdemePlani;
                this.GenelBilgiler.TeklifWebServisCevaps = this.WebServisCevaplar;

                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion

            }
            catch (Exception ex)
            {
                #region Hata Log

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                #endregion
            }
        }

        private HDIIsYeriRequest TeklifRequest(ITeklif teklif)
        {
            #region Ana Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIIsYeri);

            HDIIsYeriRequest request = new HDIIsYeriRequest();

            //P=Poliçe Prim Hesapla,  O=Poliçe Onayla, E=Eski Poliçe Sorgu (Eski Poliçe Bilgileri Döner)
            request.user = servisKullanici.KullaniciAdi;
            request.pwd = servisKullanici.Sifre;
            request.Uygulama = konfig[Konfig.HDI_UygulamaIsYeri];//PRYN703
            request.Refno = "";//Her istek için tek no,daha sonra bu no ile sorgulamada yapılabilecek
            request.IstekTip = "P"; //P=Poliçe prim sorma,O=Onaylı poliçe yaz,T=Teklif poliçe yaz,U=Teklif poliçe onayla ,I=İptal,B=Klozlar,K=Kontrol (Poliçe ve Onay için)
            request.OrjinalRefNo = "";//K ise=Poliçe kesimde kullanılan referans buraya girilir,Sadece kontrol tipi için
            request.HDIPoliceNumara = String.Empty;
            request.HDIYenilemeNo = String.Empty;
            request.IptalNedeni = String.Empty;
            request.Tarih = TurkeyDateTime.Now.ToString("ddMMyyyy");

            //Para Birimi
            request.WSDVCN = "TL";

            //Döviz Kuru
            request.WSDVKR = String.Empty;

            //Fronting (E/H)
            request.WSFRNT = "E";

            //Vergili   (E/H)         
            request.WSVER = "H";

            //Y.S.V.li (E/H)
            request.WSYVER = "E";

            //Sağlık Teminatı
            request.WSSAG = "H";

            //Peşin İndirim 
            request.WSIND = String.Empty;

            #endregion

            #region Musteri Bilgileri

            MusteriGenelBilgiler sigortaEttiren = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);
            if (MusteriTipleri.Ozel(sigortaEttiren.MusteriTipKodu))
            {
                request.OzelTuzel = "O";
                request.MSCnsTp = sigortaEttiren.Cinsiyet;
                request.MSDogYL = sigortaEttiren.DogumTarihi.HasValue ? sigortaEttiren.DogumTarihi.Value.Year.ToString() : String.Empty;

                //Eğitim kodu GenelTablolar.xls'de bilgiler mevcuttur    ==== 1	İLKOKUL
                request.MSEgKd = sigortaEttiren.EgitimDurumu.HasValue ? sigortaEttiren.EgitimDurumu.HasValue.ToString() : "10";

                //Meslek kodu GenelTablolar.xls bilgiler mevcuttur       ===== 10	DİĞER
                request.MSMesKd = sigortaEttiren.MeslekKodu.HasValue ? sigortaEttiren.MeslekKodu.HasValue.ToString() : "10";

                //Medeni bilgiler -GenelTablolar.xls                     ==== 1	BEKAR
                request.MSMedKd = sigortaEttiren.MedeniDurumu.HasValue ? sigortaEttiren.MedeniDurumu.HasValue.ToString() : "1";

                // Çocuk bilgiler - GenelTablolar.xls  ===== 1	0 cocuk
                request.MSCocKd = "1";

                // Ehliyet Yılı
                request.MSEhlYL = String.Empty;

                // Sektör Bilgileri - GenelTablolar.xls   ====2101	DİĞER
                request.MSSekKd = "2101";

                //İstemci Müşteri No
                request.LMUSNO = String.Empty;

                //Sigorta Ettiren 
                request.WSSIGE = sigortaEttiren.AdiUnvan + " " + sigortaEttiren.SoyadiUnvan;

                request.Uyruk = sigortaEttiren.Uyruk == UyrukTipleri.TC ? "0" : "1";

                if (request.Uyruk == "1")
                    request.YabanciKimlikNo = sigortaEttiren.KimlikNo;
                else
                    request.TcKimlikNo = sigortaEttiren.KimlikNo;

                request.PasaportNo = sigortaEttiren.PasaportNo;
            }
            else
            {
                request.OzelTuzel = "T";
                request.VergiKimlikNo = sigortaEttiren.KimlikNo;
            }

            #endregion

            #region Telefon

            request.IsTel1 = "";
            request.IsTel2 = "";
            request.EvTel1 = "";
            request.EvTel2 = "";

            // Tel numarası alanlarından en az biri dolu olmalı			
            List<MusteriTelefon> telefonlar = sigortaEttiren.MusteriTelefons.ToList<MusteriTelefon>();
            List<MusteriTelefon> isTelefonlari = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Is)
                                                           .Take(2)
                                                           .ToList<MusteriTelefon>();

            if (isTelefonlari.Count > 0)
                request.IsTel1 = HDIMessage.ToHDITelefon(isTelefonlari[0].Numara);
            if (isTelefonlari.Count > 1)
                request.IsTel2 = HDIMessage.ToHDITelefon(isTelefonlari[1].Numara);

            List<MusteriTelefon> evTelefonlari = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Ev)
                                                 .Take(2)
                                                 .ToList<MusteriTelefon>();

            if (evTelefonlari.Count > 0)
                request.EvTel1 = HDIMessage.ToHDITelefon(evTelefonlari[0].Numara);//BBBnnnNNNN
            if (evTelefonlari.Count > 1)
                request.EvTel2 = HDIMessage.ToHDITelefon(evTelefonlari[1].Numara);

            #endregion

            #region Adres

            MusteriAdre adres = sigortaEttiren.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
            if (adres != null)
                AdresDoldur(request, adres);

            #endregion

            #region iş yeri İlişkin Bilgiler

            #region Genel Bilgiler

            //Dask a dahil mi
            request.WSDSEH = teklif.ReadSoru(IsYeriSorular.Yururlukte_dask_policesi_VarYok, false) ? "E" : "H";

            //Daini mürtein (E/H)
            request.WSDAIN = teklif.ReadSoru(IsYeriSorular.RA_Dain_i_Muhtehin_VarYok, false) ? "E" : "H";

            //Daini mürtein/Ad/Soyad)
            if (request.WSDAIN == "E")
                request.WSDAMU = teklif.ReadSoru(IsYeriSorular.RA_Kurum_Banka, String.Empty);

            //Riziko & Müşteri adresi aynı mı
            request.WSRIZ = "H";

            //Değerli Eşya Liste (E/H) (Değerli eşya için)        
            //request.WSELST = "H";

            #endregion

            #region Adres Bilgileri

            //(WSRIZ 'H' ise Riziko ve müşteri adresi farklıysa istenir)
            if (teklif.RizikoAdresi.IlKodu.HasValue)
            {
                request.WSILK = teklif.RizikoAdresi.IlKodu.ToString();
            }

            if (teklif.RizikoAdresi.PostaKodu.HasValue)
                request.WSPOSK = teklif.RizikoAdresi.PostaKodu.Value.ToString();

            request.WSADR = teklif.RizikoAdresi.Mahalle + " mah." + teklif.RizikoAdresi.SemtBelde;//Adres
            request.WSKOY = teklif.RizikoAdresi.Mahalle;
            request.WSCAD = teklif.RizikoAdresi.Cadde;
            request.WSSOK = teklif.RizikoAdresi.Sokak;

            //Han Apartman tipi (1=Apartma,2=Han,3=Çarşı,4=Pasaj,5=Diğer)
            request.WSHTIP = teklif.RizikoAdresi.HanAptFab;
            request.WSHAN = teklif.RizikoAdresi.Apartman;//Han Apartman Adı
            request.AdresBilgi.BinaNo = teklif.RizikoAdresi.Bina;
            request.WSDRNO = teklif.RizikoAdresi.Daire;
            request.WSKAT = "";//Kat;
            request.WSSEMT = teklif.RizikoAdresi.SemtBelde;
            request.WSILCK = teklif.RizikoAdresi.IlceKodu.ToString();
            request.WSILK = teklif.RizikoAdresi.IlKodu.ToString();

            #endregion

            #region Diger Bilgiler

            //Kira kaybı bedeli
            request.WSKIRB = String.Empty;

            //Kira Kaybı AY
            request.WSKIRA = String.Empty;

            //Dahili Su Muaf.Klozu(E/H)
            request.WSPLTH = String.Empty;

            // Hırsızlık Muaf.Notu(E/H)
            request.WSHMUA = String.Empty;

            //Fiyat Basımı(E/H)
            request.WSFBAS = "H";

            //Diğer İndirim(E/H) 1A
            request.WSDGIN = "H";

            //Palet var mı E/H
            request.WSPLET = "H";

            //Yapı Tarzı
            request.WSYAPT = teklif.ReadSoru(IsYeriSorular.Yapi_Tarzi, String.Empty);

            //İştigal Kodu
            request.WSISTK = teklif.ReadSoru(IsYeriSorular.IstigalKonusu, String.Empty);

            //Daire MEtrekaresi
            request.WSMETK = teklif.ReadSoru(IsYeriSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);

            //Enflasyon oranı %
            request.WSENF = teklif.ReadSoru(IsYeriSorular.EnflasyonOrani, String.Empty);

            //Asansör sayısı
            request.WSASAN = teklif.ReadSoru(IsYeriSorular.AsansorSayisi, String.Empty);

            //İşyerinde çalışan kişi sayısı
            request.WSKSSY = teklif.ReadSoru(IsYeriSorular.IsYerindeCalisanKisiSayisi, String.Empty);

            //Enflasyon oranı %
            request.WSENF = teklif.ReadSoru(IsYeriSorular.EnflasyonOrani, String.Empty);

            //Çatı tipi
            request.WSCAT = teklif.ReadSoru(IsYeriSorular.CatiTipi, String.Empty);

            //KatTipi
            request.WSKKAT = teklif.ReadSoru(IsYeriSorular.KatTipi, String.Empty);

            // Kepenk ve/veya Demir E/H
            request.WSKEP = teklif.ReadSoru(IsYeriSorular.KepenkVeVeyaDemir, String.Empty);

            // Pasaj İçi/Üst Katlar E/H
            request.WSPSAJ = teklif.ReadSoru(IsYeriSorular.PasajIciUstKatlar, String.Empty);

            // Alarm E/H
            request.WSALRM = teklif.ReadSoru(IsYeriSorular.OzelGuvenlik_Alarm_VarMi_EH, String.Empty);

            // Bekçi E/H
            request.WSBEKC = teklif.ReadSoru(IsYeriSorular.Bekci, String.Empty);

            //Kamera (E/H)
            request.WSKAME = teklif.ReadSoru(IsYeriSorular.Kamera, String.Empty);

            //Temperli Cam (E/H)
            request.WSTCAM = teklif.ReadSoru(IsYeriSorular.TemperliCam, String.Empty);

            //Boş kalma süresi (ay olarak en fazla 11 olabilir...)
            request.WSSUR = teklif.ReadSoru(IsYeriSorular.BosKalmaSuresi, String.Empty);

            //EŞYA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse	DepremMuafiyetleri dosyasıdaki 1466 kademesi
            request.WSMUMD = "5";  //Bu muafiyet eşya beleli zorunlu olduğu için 5 olarak set ediliyor.

            //DASKa dahil mi (E/H)       (WSDSEH)            
            if (request.WSDSEH == "E")
            {
                //DASK Şirket No Daska dahilmi 'E' seçilirse
                request.WSRSKD = teklif.ReadSoru(IsYeriSorular.Dask_Police_Sigorta_Sirketi, String.Empty);

                //DASK Poliçe No Daska dahilmi 'E' seçilirse
                request.WSPLNO = teklif.ReadSoru(IsYeriSorular.Dask_Police_Numarasi, String.Empty);

                //Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse       1461	1	2	0,100000	DEPREM MUAFİYETLİ SİGORTA
                //request.WSMUD = "2";
            }
            //BİNA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse	DepremMuafiyetleri dosyasıdaki 1465 kademesi
            else
                if (teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiBina, false))
                    request.WSMUBD = "2";  //Bu muafiyet dask bedeli yoksa ve bina bedeli varsa 2 olarak set ediliyor.

            //Belediye Kodu
            request.WSBEL = teklif.ReadSoru(IsYeriSorular.Belediye_Kodu, String.Empty);


            /*
             
                      WSMUMD (MUHT - Muafiyet % (Deprem)) alanı için; 
                02 => % 02 - 0,100000
                03 => % 03 - 0,060000
                 04 => % 04 - 0,130000
                05=> % 05 - 0,190000
                 10 => % 10 - 0,300000

                    WSMSMD MUHT - Müşterek % (Deprem) alanı için; 
                20 => % 20 - 0,200000
                25 => % 25 - 0,062500
                30 => % 30 - 0,125000
                35 => % 35 - 0,187500
                40 => % 40 - 0,250000
                45 => % 45 - 0,312500
                50 => % 50 - 0,375000
                55 => % 55 - 0,437500
                60 => % 60 - 0,500000
             
             */

            request.WSMUMD = "03";
            request.WSMSMD = "30";

            #endregion

            #region Riziko Adres

            //(WSRIZ 'H' ise Riziko ve müşteri adresi farklıysa istenir)
            if (teklif.RizikoAdresi.IlKodu.HasValue)
            {
                request.WSILK = teklif.RizikoAdresi.IlKodu.ToString();
            }

            if (teklif.RizikoAdresi.PostaKodu.HasValue)
                request.WSPOSK = teklif.RizikoAdresi.PostaKodu.Value.ToString();

            request.WSADR = teklif.RizikoAdresi.Mahalle + " mah." + teklif.RizikoAdresi.SemtBelde;//Adres
            request.WSKOY = teklif.RizikoAdresi.Mahalle;
            request.WSCAD = teklif.RizikoAdresi.Cadde;
            request.WSSOK = teklif.RizikoAdresi.Sokak;

            //Han Apartman tipi (1=Apartma,2=Han,3=Çarşı,4=Pasaj,5=Diğer)
            request.WSHTIP = teklif.RizikoAdresi.HanAptFab;
            request.WSHAN = teklif.RizikoAdresi.Apartman;//Han Apartman Adı
            request.WSDRNO = teklif.RizikoAdresi.Daire;
            request.WSKAT = "";//Kat;
            request.WSSEMT = teklif.RizikoAdresi.SemtBelde;
            request.WSILCK = teklif.RizikoAdresi.IlceKodu.ToString();
            request.WSILK = teklif.RizikoAdresi.IlKodu.ToString();

            #endregion

            #region Teminatlar

            request.EKTM = TeminatDoldur(teklif);

            #endregion

            #region Notlar

            if (teklif.GenelBilgiler.TeklifNot != null)
            {
                string not = teklif.GenelBilgiler.TeklifNot.Aciklama;

                int start = 0;

                List<S5> notlar = new List<S5>();
                S5 kisaNot = new S5();
                if (not.Length > start)
                {
                    if (not.Length > 75)
                    {
                        int dongu = not.Length / 75;
                        if (dongu > 0)
                        {
                            for (int i = 0; i < dongu; i++)
                            {
                                kisaNot = new S5();
                                kisaNot.TX = not.Substring(start, 75);
                                notlar.Add(kisaNot);
                                start += 75;
                            }
                            kisaNot = new S5();
                            kisaNot.TX = not.Substring((not.Length - (not.Length - (dongu * 75))));
                            notlar.Add(kisaNot);
                        }
                    }
                    else
                    {
                        kisaNot = new S5();
                        kisaNot.TX = not;
                        notlar.Add(kisaNot);
                    }
                    request.NTS = notlar;
                }
            }

            #endregion

            #endregion

            #region Odeme

            //Ödeme Şekli (P/V)  Peşin için "P" Vadeli için "V"                
            string odemeSekli = "P";
            if (teklif.GenelBilgiler.OdemeSekli.HasValue)
                if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli) odemeSekli = "V";
            request.WSOS = odemeSekli;

            //Taksit sayısından önce <OdemeTipi>1</OdemeTipi> alanını eklerseniz taksit sayıları doğru dönecektir.
            request.OdemeTipi = "1";

            byte taksitSayisi = 1;

            if (teklif.GenelBilgiler.TaksitSayisi.HasValue)
                if (teklif.GenelBilgiler.TaksitSayisi.Value > 6)
                    taksitSayisi = 6;
                else
                    taksitSayisi = teklif.GenelBilgiler.TaksitSayisi.Value;

            request.TaksitSayisi = taksitSayisi.ToString();
            request.TckR = "";
            request.SubeKod = "";
            request.TemsilciNo = "";
            request.TemsilciAd = "";
            request.TemsilciSoyAd = "";

            #endregion

            return request;
        }

        public override void PolicePDF()
        {
            try
            {
                #region Request hazırlama
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] {tvmKodu,
                                                                                                                    TeklifUretimMerkezleri.HDI });
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_PDFServiceURL);

                HDIPDFRequest request = new HDIPDFRequest();
                request.PoliceNumarasi = this.GenelBilgiler.TUMPoliceNo;
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;
                request.Uygulama = "131"; //İşyeri Ürün Kodu 131.//_KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaKonut);

                #endregion

                #region Servis call

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serviceURL + "?" +
                                                                              "user=" + request.user +
                                                                              "&pwd=" + request.pwd +
                                                                              "&Uygulama=" + request.Uygulama +
                                                                              "&PoliceNumarasi=" + request.PoliceNumarasi);

                webRequest.Method = "POST";
                webRequest.Timeout = 60000;
                webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

                using (HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse())
                {
                    Stream data = resp.GetResponseStream();

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("isyeri_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("isyeri", fileName, data);
                    this.GenelBilgiler.PDFPolice = url;

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                    _Log.Info("PDF dokumanı oluşturuldu : {0}", url);
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                _Log.Error(ex);
                throw;

                #endregion
            }
        }

        private void AdresDoldur(HDIIsYeriRequest request, MusteriAdre adres)
        {
            AdresBilgi adresbilgi = new AdresBilgi();

            if (adres.UlkeKodu != "TUR")
            {
                adresbilgi.IlKod = "999";
                adresbilgi.Ilce = "YURTDIŞI";
            }
            else
            {
                CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.HDI &&
                                                                              f.IlKodu == adres.IlKodu &&
                                                                              f.IlceKodu == adres.IlceKodu)
                                                                                                         .Single<CR_IlIlce>();

                if (ililce != null)
                {
                    adresbilgi.Ilce = ililce.CRIlceAdi;
                    adresbilgi.IlKod = ililce.CRIlKodu;
                }
            }
            adresbilgi.Cadde = adres.Cadde;
            adresbilgi.Sokak = adres.Sokak;
            adresbilgi.Semt = adres.Semt;
            adresbilgi.KoyMahalle = adres.Mahalle;
            adresbilgi.BinaNo = adres.BinaNo;
            adresbilgi.Daire = adres.DaireNo;
            adresbilgi.HanApartmanAd = adres.Apartman;
            adresbilgi.PostaKod = adres.PostaKodu.ToString();
            adresbilgi.Adres = adres.Adres;

            request.AdresBilgi = adresbilgi;
        }

        private List<S2> TeminatDoldur(ITeklif teklif)
        {
            List<S2> teminatlar = new List<S2>();

            TeklifTeminat teminat = new TeklifTeminat();

            //BİNA (YANGIN)
            S2 BinaYangin = new S2();
            BinaYangin.S2KD = HDIKonutTeminatKod.BINA_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.BinaYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.BinaYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                BinaYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                BinaYangin.S2SC = "X";
            }
            teminatlar.Add(BinaYangin);


            //EMTEA (YANGIN)
            S2 EmteaYangin = new S2();
            EmteaYangin.S2KD = HDIKonutTeminatKod.EMTEA_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EmteaYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.EmteaYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                EmteaYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                EmteaYangin.S2SC = "X";
            }
            teminatlar.Add(EmteaYangin);


            // DEP.YANARD.PÜSK.(BİNA)
            S2 DepremYanardagPuskurmesiBina = new S2();
            DepremYanardagPuskurmesiBina.S2KD = HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_BINA;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiBina).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiBina, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                DepremYanardagPuskurmesiBina.S2BD = teminat.TeminatBedeli.Value.ToString();
                DepremYanardagPuskurmesiBina.S2SC = "X";
            }
            teminatlar.Add(DepremYanardagPuskurmesiBina);


            // DEP.YANARD.PÜSK.(MUHT)
            S2 DepremYanardagPuskurmesiMuhteviyat = new S2();
            DepremYanardagPuskurmesiMuhteviyat.S2KD = HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_MUHTEVIYAT_ESYA;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiMuhteviyat).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiMuhteviyat, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                DepremYanardagPuskurmesiMuhteviyat.S2BD = teminat.TeminatBedeli.Value.ToString();
                DepremYanardagPuskurmesiMuhteviyat.S2SC = "X";
            }
            teminatlar.Add(DepremYanardagPuskurmesiMuhteviyat);


            // GLKHHKNH VE TERÖR
            S2 GLKHHKNHTeror = new S2();
            GLKHHKNHTeror.S2KD = HDIKonutTeminatKod.GLKHHKNH_VE_TEROR;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.GLKHHKNHTeror).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.GLKHHKNHTeror, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                GLKHHKNHTeror.S2BD = teminat.TeminatBedeli.Value.ToString();
                GLKHHKNHTeror.S2SC = "X";
            }
            teminatlar.Add(GLKHHKNHTeror);


            // BİNA) EKTEMİNAT (*)
            S2 EkTeminatBina = new S2();
            EkTeminatBina.S2KD = HDIKonutTeminatKod.BINA_EK_TEMINAT;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EkTeminatBina).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.EkTeminatBina, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                EkTeminatBina.S2BD = teminat.TeminatBedeli.Value.ToString();
                EkTeminatBina.S2SC = "X";
            }
            teminatlar.Add(EkTeminatBina);


            // (MUHTEVİYAT) EKTEMİNAT (*)
            S2 MuhteviyatEkTeminat = new S2();
            MuhteviyatEkTeminat.S2KD = HDIKonutTeminatKod.MUHTEVIYAT_EKTEMINAT;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EkTeminatMuhteviyat).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.EkTeminatMuhteviyat, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                MuhteviyatEkTeminat.S2BD = teminat.TeminatBedeli.Value.ToString();
                MuhteviyatEkTeminat.S2SC = "X";
            }
            teminatlar.Add(MuhteviyatEkTeminat);


            // SEL VE SU BASMASI
            S2 SelVeSuBaskini = new S2();
            SelVeSuBaskini.S2KD = HDIKonutTeminatKod.SEL_VE_SU_BASMASI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SelVeSuBaskini).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.SelVeSuBaskini, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                SelVeSuBaskini.S2BD = teminat.TeminatBedeli.Value.ToString();
                SelVeSuBaskini.S2SC = "X";
            }
            teminatlar.Add(SelVeSuBaskini);


            // KAR AÄIRLIÄI
            S2 KarAgirligi = new S2();
            KarAgirligi.S2KD = HDIKonutTeminatKod.KAR_AGIRLIGI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KarAgirligi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KarAgirligi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KarAgirligi.S2BD = teminat.TeminatBedeli.Value.ToString();
                KarAgirligi.S2SC = "X";
            }
            teminatlar.Add(KarAgirligi);



            // YER KAYMASI
            S2 YerKaymasi = new S2();
            YerKaymasi.S2KD = HDIKonutTeminatKod.YER_KAYMASI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.YerKaymasi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.YerKaymasi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                YerKaymasi.S2BD = teminat.TeminatBedeli.Value.ToString();
                YerKaymasi.S2SC = "X";
            }
            teminatlar.Add(YerKaymasi);


            // FIRTINA
            S2 Firtina = new S2();
            Firtina.S2KD = HDIKonutTeminatKod.FIRTINA;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.Firtina).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.Firtina, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                Firtina.S2BD = teminat.TeminatBedeli.Value.ToString();
                Firtina.S2SC = "X";
            }
            teminatlar.Add(Firtina);


            // KOMŞULUK M.SORUM.(YANGIN,D.SU,DUMAN)
            S2 KomsulukMaliSorumlulukYanginDahiliSuDuman = new S2();
            KomsulukMaliSorumlulukYanginDahiliSuDuman.S2KD = HDIKonutTeminatKod.KOMSULUK_MALI_SORUMLULUK_YANGIN_SU_DUMAN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KomsulukMaliSorumlulukYanginDahiliSuDuman).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KomsulukMaliSorumlulukYanginDahiliSuDuman, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KomsulukMaliSorumlulukYanginDahiliSuDuman.S2BD = teminat.TeminatBedeli.Value.ToString();
                KomsulukMaliSorumlulukYanginDahiliSuDuman.S2SC = "X";
            }
            teminatlar.Add(KomsulukMaliSorumlulukYanginDahiliSuDuman);


            // KOMŞULUK M.SORUM.(TERÖR)
            S2 KomsulukMaliSorumlulukTeror = new S2();
            KomsulukMaliSorumlulukTeror.S2KD = HDIKonutTeminatKod.KOMSULUK_MALI_SORUMLULUK_TEROR;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KomsulukMaliSorumlulukTeror).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KomsulukMaliSorumlulukTeror, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KomsulukMaliSorumlulukTeror.S2BD = teminat.TeminatBedeli.Value.ToString();
                KomsulukMaliSorumlulukTeror.S2SC = "X";
            }
            teminatlar.Add(KomsulukMaliSorumlulukTeror);


            // KİRACI M.SORUM.(YANGIN,D.SU,DUMAN)
            S2 KiraciMaliSorumlulukYanginDahiliSuDuman = new S2();
            KiraciMaliSorumlulukYanginDahiliSuDuman.S2KD = HDIKonutTeminatKod.KIRACI_MALI_SORUMLULUK_YANGIN_DUMAN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraciMaliSorumlulukYanginDahiliSuDuman).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KiraciMaliSorumlulukYanginDahiliSuDuman, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KiraciMaliSorumlulukYanginDahiliSuDuman.S2BD = teminat.TeminatBedeli.Value.ToString();
                KiraciMaliSorumlulukYanginDahiliSuDuman.S2SC = "X";
            }
            teminatlar.Add(KiraciMaliSorumlulukYanginDahiliSuDuman);


            //  KİRACI M.SORUM.( TERÖR)
            S2 KiraciMaliSorumlulukTeror = new S2();
            KiraciMaliSorumlulukTeror.S2KD = HDIKonutTeminatKod.KIRACI_MALI_SORUMLULUK_TEROR;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraciMaliSorumlulukTeror).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KiraciMaliSorumlulukTeror, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KiraciMaliSorumlulukTeror.S2BD = teminat.TeminatBedeli.Value.ToString();
                KiraciMaliSorumlulukTeror.S2SC = "X";
            }
            teminatlar.Add(KiraciMaliSorumlulukTeror);


            //  KİRA KAYBI
            S2 KiraKaybi = new S2();
            KiraKaybi.S2KD = HDIKonutTeminatKod.KIRA_KAYBI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraKaybi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KiraKaybi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KiraKaybi.S2BD = teminat.TeminatBedeli.Value.ToString();
                KiraKaybi.S2SC = "X";
            }
            teminatlar.Add(KiraKaybi);


            //  İŞ DURMASI
            S2 YazDurmasi = new S2();
            YazDurmasi.S2KD = HDIKonutTeminatKod.IS_DURMASI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.YazDurmasi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.YazDurmasi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                YazDurmasi.S2BD = teminat.TeminatBedeli.Value.ToString();
                YazDurmasi.S2SC = "X";
            }
            teminatlar.Add(YazDurmasi);


            //  İŞYERİ ASİSTAN
            S2 AsistanHizmeti = new S2();
            AsistanHizmeti.S2KD = HDIKonutTeminatKod.ASISTAN;
            if (teklif.ReadSoru(IsYeriSorular.AsistanHizmeti, false))
                AsistanHizmeti.S2SC = "X";
            teminatlar.Add(AsistanHizmeti);


            //  HIRSIZLIK
            S2 Hirsizlik = new S2();
            Hirsizlik.S2KD = HDIKonutTeminatKod.HIRSIZLIK;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.Hirsizlik).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.Hirsizlik, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                Hirsizlik.S2BD = teminat.TeminatBedeli.Value.ToString();
                Hirsizlik.S2SC = "X";
            }
            teminatlar.Add(Hirsizlik);


            //  KASA HIRSIZLIK
            S2 KasaHirsizlik = new S2();
            KasaHirsizlik.S2KD = HDIKonutTeminatKod.KASA_HIRSIZLIK;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KasaHirsizlik).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KasaHirsizlik, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KasaHirsizlik.S2BD = teminat.TeminatBedeli.Value.ToString();
                KasaHirsizlik.S2SC = "X";
            }
            teminatlar.Add(KasaHirsizlik);


            //  TAŞINAN PARA Beher Sefer iÃ§in Limit
            S2 tasinanparaBeherSeferLimit = new S2();
            tasinanparaBeherSeferLimit.S2KD = HDIKonutTeminatKod.TASINAN_PARA_BEHER_SEFER_LIMIT;
            teminatlar.Add(tasinanparaBeherSeferLimit);


            //TAŞINAN PARA Yıllık Toplam Limit
            S2 tasinanparaYillikToplamLimit = new S2();
            tasinanparaYillikToplamLimit.S2KD = HDIKonutTeminatKod.TASINAN_PARA_YILLIK_TOPLAM_LIMIT;
            teminatlar.Add(tasinanparaYillikToplamLimit);



            //  EMNİYETİ SUİSTİMAL Kişi Başına/Yıllık azami
            S2 EmniyetSuistimal = new S2();
            EmniyetSuistimal.S2KD = HDIKonutTeminatKod.EMNIYET_SUISTIMAL_KISI_BASINA_YILLIK;
            teminatlar.Add(EmniyetSuistimal);


            //FERDİ KAZA Kişi Başına
            S2 FerdiKazaKisiBasina = new S2();
            FerdiKazaKisiBasina.S2KD = HDIKonutTeminatKod.FERDI_KAZA;
            if (teklif.ReadSoru(IsYeriSorular.FerdiKaza, false))
                FerdiKazaKisiBasina.S2SC = "X";
            teminatlar.Add(FerdiKazaKisiBasina);


            //CAM KIRILMASI
            S2 CamKirilmasi = new S2();
            CamKirilmasi.S2KD = HDIKonutTeminatKod.CAM_KIRILMASI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.CamKirilmasi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.CamKirilmasi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                CamKirilmasi.S2BD = teminat.TeminatBedeli.Value.ToString();
                CamKirilmasi.S2SC = "X";
            }
            teminatlar.Add(CamKirilmasi);


            //İŞVEREN MALİ MESULİYET Kişi Başına Bedeni
            S2 IsverenMaliMesuliyetKisiBasinaBedeni = new S2();
            IsverenMaliMesuliyetKisiBasinaBedeni.S2KD = HDIKonutTeminatKod.ISVEREN_MALI_MESULIYET_KISI_BASI_BEDENI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.IsVerenMaliMEsuliyetKisiBasinaBedeni).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.IsverenMaliMesuliyetKisiBasinaBedeni, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                IsverenMaliMesuliyetKisiBasinaBedeni.S2BD = teminat.TeminatBedeli.Value.ToString();
                IsverenMaliMesuliyetKisiBasinaBedeni.S2SC = "X";
            }
            teminatlar.Add(IsverenMaliMesuliyetKisiBasinaBedeni);



            //İŞVEREN MALİ MESULİYET Kaza Başına Bedeni
            S2 IsverenMaliMesuliyetKazaBasinaBedeni = new S2();
            IsverenMaliMesuliyetKazaBasinaBedeni.S2KD = HDIKonutTeminatKod.ISVEREN_MALI_MESULIYET_KAZA_BASI_BEDENI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.IsVerenMaliMesuliyetKazaBasinaBedeni).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.IsverenMaliMesuliyetKazaBasinaBedeni, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                IsverenMaliMesuliyetKazaBasinaBedeni.S2BD = teminat.TeminatBedeli.Value.ToString();
                IsverenMaliMesuliyetKazaBasinaBedeni.S2SC = "X";
            }
            teminatlar.Add(IsverenMaliMesuliyetKazaBasinaBedeni);



            //3.ŞAHIS MALİ SORUMLULUK Kişi Başına Bedeni
            S2 SahisMaliSorumlulukKisiBasinaBedeni = new S2();
            SahisMaliSorumlulukKisiBasinaBedeni.S2KD = HDIKonutTeminatKod.SAHIS_MALI_SORUMLULUK_KISI_BASI_BEDENI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKisiBasinaBedeni3, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                SahisMaliSorumlulukKisiBasinaBedeni.S2BD = teminat.TeminatBedeli.Value.ToString();
                SahisMaliSorumlulukKisiBasinaBedeni.S2SC = "X";
            }
            teminatlar.Add(SahisMaliSorumlulukKisiBasinaBedeni);



            //3.ŞAHIS MALİ SORUMLULUK Kaza Başına Bedeni
            S2 SahisMaliSorumlulukKazaBasinaBedeni = new S2();
            SahisMaliSorumlulukKazaBasinaBedeni.S2KD = HDIKonutTeminatKod.SAHIS_MALI_SORUMLULUK_KAZE_BASI_BEDENI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaBedeni3, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                SahisMaliSorumlulukKazaBasinaBedeni.S2BD = teminat.TeminatBedeli.Value.ToString();
                SahisMaliSorumlulukKazaBasinaBedeni.S2SC = "X";
            }
            teminatlar.Add(SahisMaliSorumlulukKazaBasinaBedeni);



            //3.ŞAHIS MALİ SORUMLULUK Kaza Başına Maddi
            S2 SahisMaliSorumlulukKazaBasinaMaddi = new S2();
            SahisMaliSorumlulukKazaBasinaMaddi.S2KD = HDIKonutTeminatKod.SAHIS_MALI_SORUMLULUK_KAZA_BASI_MADDI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaMaddi3).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaMaddi3, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                SahisMaliSorumlulukKazaBasinaMaddi.S2BD = teminat.TeminatBedeli.Value.ToString();
                SahisMaliSorumlulukKazaBasinaMaddi.S2SC = "X";
            }
            teminatlar.Add(SahisMaliSorumlulukKazaBasinaMaddi);



            //MAKİNA KIRILMASI
            S2 MakinaKirilmasi = new S2();
            MakinaKirilmasi.S2KD = HDIKonutTeminatKod.MAKINA_KIRILMASI;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.MakinaKirilmasi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.MakinaKirilmasi, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                MakinaKirilmasi.S2BD = teminat.TeminatBedeli.Value.ToString();
                MakinaKirilmasi.S2SC = "X";
            }
            teminatlar.Add(MakinaKirilmasi);



            //ELEKTRONİK CİHAZ 
            S2 ElektronikCihaz = new S2();
            ElektronikCihaz.S2KD = HDIKonutTeminatKod.ELEKTRONIK_CIHAZ;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.ElektronikCihazSigortasi).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.ElektronikCihaz, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                ElektronikCihaz.S2BD = teminat.TeminatBedeli.Value.ToString();
                ElektronikCihaz.S2SC = "X";
            }
            teminatlar.Add(ElektronikCihaz);



            //ACİL TIBBI/HASTANE/FERDİKAZA
            S2 AcilTibbiHastaneFerdiKaza = new S2();
            AcilTibbiHastaneFerdiKaza.S2KD = HDIKonutTeminatKod.ACIL_TIBBI_HASTANE_FERDIKAZA;
            teminatlar.Add(AcilTibbiHastaneFerdiKaza);


            //PROMED
            S2 Promed = new S2();
            Promed.S2KD = HDIKonutTeminatKod.PROMED;
            teminatlar.Add(Promed);


            //MEDLINE
            S2 Medline = new S2();
            Medline.S2KD = HDIKonutTeminatKod.MEDLINE;
            teminatlar.Add(Medline);


            //MAKİNA VE TECHİZAT (YANGIN)
            S2 MakinaVeTechizat = new S2();
            MakinaVeTechizat.S2KD = HDIKonutTeminatKod.MAKINA_VE_TECHIZAT_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.MakinaVeTechizat).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.MakinaTechizat, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                MakinaVeTechizat.S2BD = teminat.TeminatBedeli.Value.ToString();
                MakinaVeTechizat.S2SC = "X";
            }
            teminatlar.Add(MakinaVeTechizat);


            //DEMİRBAŞ (YANGIN)
            S2 DemirbasYangin = new S2();
            DemirbasYangin.S2KD = HDIKonutTeminatKod.DEMIRBAS_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DemirbasYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.DemirbasYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                DemirbasYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                DemirbasYangin.S2SC = "X";
            }
            teminatlar.Add(DemirbasYangin);


            //KASA MUHTEVİYATI(YangÄ±n)
            S2 KasaMuhteviyatYangin = new S2();
            KasaMuhteviyatYangin.S2KD = HDIKonutTeminatKod.KASA_MUHTEVIYATI_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KasaMuhteviyatYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.KasaMuhteviyatYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                KasaMuhteviyatYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                KasaMuhteviyatYangin.S2SC = "X";
            }
            teminatlar.Add(KasaMuhteviyatYangin);


            //3. ŞAHIS MALLARI (YANGIN)
            S2 SahisMallariYangin = new S2();
            SahisMallariYangin.S2KD = HDIKonutTeminatKod.SAHIS_MALLARI_YANGIN_3;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMallariYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.SahisMallariYangin3, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                SahisMallariYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                SahisMallariYangin.S2SC = "X";
            }
            teminatlar.Add(SahisMallariYangin);


            // DEKORASYON (YANGIN)
            S2 DekorasyonYangin = new S2();
            DekorasyonYangin.S2KD = HDIKonutTeminatKod.DEKORASYON_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DekorasyonYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.DekorasyonYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                DekorasyonYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                DekorasyonYangin.S2SC = "X";
            }
            teminatlar.Add(DekorasyonYangin);


            // TEMELLER (YANGIN)
            S2 TemellerYangin = new S2();
            TemellerYangin.S2KD = HDIKonutTeminatKod.TEMELLER_YANGIN;
            teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.TemellerYangin).FirstOrDefault();
            if (teklif.ReadSoru(IsYeriSorular.TemellerYangin, false) && teminat != null && teminat.TeminatBedeli > 0)
            {
                TemellerYangin.S2BD = teminat.TeminatBedeli.Value.ToString();
                TemellerYangin.S2SC = "X";
            }
            teminatlar.Add(TemellerYangin);


            // HUKUKSAL KORUMA
            S2 hukuksalKoruma = new S2();
            hukuksalKoruma.S2KD = HDIKonutTeminatKod.HUKUKSAL_KORUMA;
            if (teklif.ReadSoru(IsYeriSorular.HukuksalKoruma, false))
            {
                hukuksalKoruma.S2BD = "";
                hukuksalKoruma.S2SC = "X";
            }
            teminatlar.Add(hukuksalKoruma);


            return teminatlar;
        }
    }
}

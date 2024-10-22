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
    public class HDIKonut : Teklif, IHDIKonut
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
        public HDIKonut(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
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
            _TVMService = TVMService;
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
            HDIKonutResponse response = null;
            try
            {
                #region Teklif Hazırlama
                HDIKonutRequest request = this.TeklifRequest(teklif);
                #endregion

                #region Servis call
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(requestXML, WebServisIstekTipleri.Teklif);

                response = request.HttpRequest<HDIKonutResponse>(serviceURL, requestXML, out responseXML);

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
                this.GenelBilgiler.TaksitSayisi = (byte)(response.Taksitler.Count() > 0 ? response.Taksitler.Count() : 1);
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;

                #endregion

                #region Vergiler

                this.AddVergi(KonutVergiler.GiderVergisi, GiderVergisi);
                this.AddVergi(KonutVergiler.YanginSigortaVergisi, YSV);

                #endregion

                #region Teminatlar

                foreach (var teminat in response.EkTeminatListe)
                {
                    if (teminat.WSETK2 == HDIKonutTeminatKod.YANGIN_MALI_SORUMLULUK)
                        this.AddTeminat(KonutTeminatlar.MaliMesuliyetYangin, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.BINA_EK_TEMINAT)
                        this.AddTeminat(KonutTeminatlar.EkTeminatBina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.ESYA_EK_TEMINAT)
                        this.AddTeminat(KonutTeminatlar.EkTeminatEsya, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK)
                        this.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_BINA)
                        this.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesiBina, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.DEPREMVEYANARDAG_PUSK_MUHTEVIYAT_ESYA)
                        this.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesiEsya, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.ASISTAN)
                        this.AddTeminat(KonutTeminatlar.AsistanHizmeti, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.HIRSIZLIK)
                        this.AddTeminat(KonutTeminatlar.Hirsizlik, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.KAPKAC_TEMINATI)
                        this.AddTeminat(KonutTeminatlar.Kapkac, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.FERDI_KAZA)
                        this.AddTeminat(KonutTeminatlar.FerdiKaza, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.CAM_KIRILMASI)
                        this.AddTeminat(KonutTeminatlar.CamKirilmasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.MEDLINE)
                        this.AddTeminat(KonutTeminatlar.Medline, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.ELEKTRONIK_CIHAZ)
                        this.AddTeminat(KonutTeminatlar.ElektronikCihaz, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.YER_KAYMASI)
                        this.AddTeminat(KonutTeminatlar.YerKaymasi, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                    else if (teminat.WSETK2 == HDIKonutTeminatKod.SEL_VE_SU_BASMASI)
                        this.AddTeminat(KonutTeminatlar.SelVeSuBaskini, HDIMessage.ToDecimal(teminat.WSTEM2), 0, 0, 0, 0);
                }


                foreach (var ozel in response.OzelFiyatListe)
                {
                    if (ozel.WSETK3 == HDIKonutTeminatKod.ESYA_YANGIN)
                        this.AddTeminat(KonutTeminatlar.EsyaYangin, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.BINA_YANGIN)
                        this.AddTeminat(KonutTeminatlar.BinaYangin, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.TEMELLER_YANGIN)
                        this.AddTeminat(KonutTeminatlar.TemellerYangin, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.FIRTINA)
                        this.AddTeminat(KonutTeminatlar.Firtina, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.IZOLASYON_OLAY_BAS)
                        this.AddTeminat(KonutTeminatlar.IzolasOlayBsYil, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.SAHIS_MALI_SORUMLULUK_3)
                        this.AddTeminat(KonutTeminatlar.MaliMesuliyetEkTeminat, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
                    else if (ozel.WSETK3 == HDIKonutTeminatKod.HUKUKSAL_KORUMA)
                        this.AddTeminat(KonutTeminatlar.HukuksalKoruma, HDIMessage.ToDecimal(ozel.WSTEM3), 0, 0, 0, 0);
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

                #region Web servis cevapları

                this.AddWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, response.ReferansNo);

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
            HDIKonutResponse response = null;
            try
            {
                #region Teklif Hazırlama

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                HDIKonutRequest request = this.TeklifRequest(teklif);
                string referansNo = this.ReadWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, String.Empty);

                //P=Poliçe prim sorma,O=Onaylı poliçe yaz,K=Kontrol (Poliçe ve Onay için)
                request.IstekTip = "O";


                //string[] adSoyad = odeme.KrediKarti.KartSahibi.Split(' ');
                //if (adSoyad.Length == 2)
                //{
                //    request.KartAd = adSoyad[0];
                //    request.KartSoyad = adSoyad[1];
                //}

                #endregion

                #region Servis call

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);
                string requestXML = request.ToString();
                string responseXML = String.Empty;

                //Kart Bilgileri log dosyasına eklenmiyor.
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

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
                response = request.HttpRequest<HDIKonutResponse>(serviceURL, requestXML, out responseXML);

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

                            if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                            {
                                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                            }
                            else
                            {
                                // ==== HDI Response taksit sayısı ve miktarlarını donuyor. ==== //
                                int sayac = 1;
                                foreach (var item in response.Taksitler)
                                {
                                    this.AddOdemePlani(sayac, HDIMessage.ToDateTimeForKasko(item.TaksitVade), HDIMessage.ToDecimal(item.TaksitTutar),
                                                                                                            (teklif.GenelBilgiler.OdemeTipi ?? 0));
                                    sayac++;
                                }
                            }
                        }

                    #region Web servis cevapları

                    this.AddWebServisCevap(Common.WebServisCevaplar.HDIBIMReferans, response.BIMReferans);
                    this.AddWebServisCevap(Common.WebServisCevaplar.HDISeriNo, response.SeriNo);

                    #endregion

                    this.PolicePDF();
                    //_TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
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

        private HDIKonutRequest TeklifRequest(ITeklif teklif)
        {
            #region Ana Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIKonut);

            HDIKonutRequest request = new HDIKonutRequest();

            //P=Poliçe Prim Hesapla,  O=Poliçe Onayla, E=Eski Poliçe Sorgu (Eski Poliçe Bilgileri Döner)
            request.user = servisKullanici.KullaniciAdi;
            request.pwd = servisKullanici.Sifre;
            request.Uygulama = konfig[Konfig.HDI_UygulamaKonut];//PRYN721
            request.Refno = "";//Her istek için tek no,daha sonra bu no ile sorgulamada yapılabilecek
            request.IstekTip = "P";
            request.OrjinalRefNo = "";//K ise=Poliçe kesimde kullanılan referans buraya girilir,Sadece kontrol tipi için
            request.HDIPoliceNumara = String.Empty;
            request.HDIYenilemeNo = String.Empty;
            request.IptalNedeni = String.Empty;
            request.Tarih = TurkeyDateTime.Now.ToString("ddMMyyyy");

            //Para Birimi
            request.WSDVCN = "TL";

            //Döviz Kuru
            request.WSDVKR = "";

            //Fronting (E/H)
            request.WSFRNT = "E";

            //Vergili   (E/H)         
            request.WSVER = "H";

            //Y.S.V.li (E/H)
            request.WSYVER = "E";

            //HAsarsızlık İndirim(0 10 15 20 25 olabilir)
            request.WSHAS = "0";

            //Sağlık Teminatı
            request.WSSAG = "H";

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

            #region Konuta İlişkin Bilgiler

            #region Genel Bilgiler

            //Dask a dahil mi
            request.WSDSEH = teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false) ? "E" : "H";

            //Daini mürtein (E/H)
            request.WSDAIN = teklif.ReadSoru(KonutSorular.RA_Dain_i_Muhtehin_VarYok, false) ? "E" : "H";

            //Daini mürtein/Ad/Soyad)
            if (request.WSDAIN == "E")
                request.WSDAMU = teklif.ReadSoru(KonutSorular.RA_Kurum_Banka, String.Empty);

            //Riziko & Müşteri adresi aynı mı
            request.WSRIZ = "H";
            //request.WSRIZ = "E";

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
            request.BinaNo = teklif.RizikoAdresi.Bina;
            request.WSDRNO = teklif.RizikoAdresi.Daire;
            request.WSKAT = "";//Kat;
            request.WSSEMT = teklif.RizikoAdresi.SemtBelde;
            request.WSILCK = teklif.RizikoAdresi.IlceKodu.ToString();
            request.WSILK = teklif.RizikoAdresi.IlKodu.ToString();

            #endregion

            #region Diger Bilgiler

            //Yapı Tarzı
            request.WSYAPT = teklif.ReadSoru(KonutSorular.Yapi_Tarzi, String.Empty);

            //Daire MEtrekaresi
            request.WSMETK = teklif.ReadSoru(KonutSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);

            //Enflasyon oranı %
            request.WSENF = teklif.ReadSoru(KonutSorular.EnflasyonOrani, String.Empty);

            //Belediye Kodu
            string belediyeKodu = teklif.ReadSoru(KonutSorular.Belediye_Kodu, String.Empty);
            if (!String.IsNullOrEmpty(belediyeKodu))
            {
                string[] array = belediyeKodu.Split('-');
                if (array.Length == 2)
                    request.WSBEL = array[1];
            }
            //Çelik Kapı varmı?(E/H)
            request.WSCKAP = teklif.ReadSoru(KonutSorular.Celik_Kapı_VarMi_EH, false) ? "E" : "H";

            //Demir Parmaklık varmı?(E/H)
            request.WSDPAR = teklif.ReadSoru(KonutSorular.DemirPArmaklik_VarMi_EH, false) ? "E" : "H";

            //Özel Guvenlik/Alarm varmı?(E/H)
            request.WSOGUV = teklif.ReadSoru(KonutSorular.OzelGuvenlik_Alarm_VarMi_EH, false) ? "E" : "H";

            //Sigortalanacak yer kaçıncı katta 3,0 
            request.WSYKKAT = teklif.ReadSoru(KonutSorular.SigortalanacakYer_Kacinci_Katta, String.Empty);

            //Diğer İndirim(E/H) 1A
            request.WSDGIN = "H";

            //Fiyat Basımı(E/H)
            request.WSFBAS = "H";

            //Boş kalma süresi (ay olarak en fazla 11 olabilir...)
            request.WSSUR = teklif.ReadSoru(KonutSorular.BosKalmaSuresi, String.Empty);


            //Kışlık(E/H)  -- Yazlık kışlık değeri alınıyor.
            request.WSKIS = teklif.ReadSoru(KonutSorular.KislikMi, false) ? "E" : "H";


            /*
                 DASK’a dahil mi “E” ise;
                 1332 numaralı DEP.YANARD.PÜSK.(EŞYA) teminatı seçilirse WSMUMD alanının değerini default 5,

                 DASK’a dahil mi “H” ise;
                 1331 numaralı DEP.YANARD.PÜSK.(BİNA) teminatı seçilirse WSMUBD alanının değerini default 2
                 1332 numaralı DEP.YANARD.PÜSK.(EŞYA) teminatı seçilirse WSMUMD alanının değerini default 5 olarak
              
                  WSMUD  = Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse
                  WSMUBD = BİNA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse
                  WSMUMD = EŞYA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse
              */

            //EŞYA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse	DepremMuafiyetleri dosyasıdaki 1466 kademesi
            request.WSMUMD = "5";  //Bu muafiyet eşya beleli zorunlu olduğu için 5 olarak set ediliyor.

            //DASKa dahil mi (E/H)       (WSDSEH)            
            if (request.WSDSEH == "E")
            {
                //DASK Şirket No Daska dahilmi 'E' seçilirse
                request.WSRSKD = teklif.ReadSoru(KonutSorular.Dask_Police_Sigorta_Sirketi, String.Empty);

                //DASK Poliçe No Daska dahilmi 'E' seçilirse
                request.WSPLNO = teklif.ReadSoru(KonutSorular.Dask_Police_Numarasi, String.Empty);

                //Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse       1461	1	2	0,100000	DEPREM MUAFİYETLİ SİGORTA
                //request.WSMUD = "2";
            }
            //BİNA için Muafiyet % (Deprem) Daska dahilmi 'E' seçilirse	DepremMuafiyetleri dosyasıdaki 1465 kademesi
            else
                if (teklif.ReadSoru(KonutSorular.DepremYanardagPuskurmesiBina, false))
                    request.WSMUBD = "2";  //Bu muafiyet dask bedeli yoksa ve bina bedeli varsa 2 olarak set ediliyor.

            #endregion

            #region Teminat Bedelleri

            //Sigorta Kapsamı Bina(Yangın) Bedel girildiğinde teminat eklenir.
            request.ATBINA = teklif.ReadSoru(KonutSorular.BinaBedeli, String.Empty);

            //Sigorta Kapsamı Eşya(Yangın) Bedel girildiğinde teminat eklenir.
            request.ATESYA = teklif.ReadSoru(KonutSorular.EsyaBedeli, String.Empty);

            //Değerli Eşya  Liste (E/H)(Değerli eşya için)
            request.WSELST = "H";

            //string degerliEsyaTeminati = teklif.ReadSoru(KonutSorular.DegerliEsyaYangin, "0");
            //if (degerliEsyaTeminati != "" && degerliEsyaTeminati != "0")
            //{
            //    //Sigorta Kapsamı Değerli Eşya(Yangın) Bedel girildiğinde teminat eklenir.(DEğerli eşya Liste.E ise girilir)
            //    request.ATDGES = degerliEsyaTeminati;
            //    request.WSELST = "E";
            //}

            ////Sigorta Kapsamı TEmel(Yangın)(Tek başına seçilmez) Bedel girildiğinde teminat eklenir.
            if (request.ATBINA != "" && request.ATBINA != "0")
                request.ATTEMEL = request.ATBINA;

            #endregion

            #region Teminatlar

            List<S2> ekTaminatlar = new List<S2>();

            //YANGIN MALİ SORUMLULUK
            if (teklif.ReadSoru(KonutSorular.MaliMesuliyetYangin, false))
            {
                TeklifTeminat teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetYangin).FirstOrDefault();
                if (teminat != null && teminat.TeminatBedeli.HasValue)
                {
                    S2 S2 = new S2();
                    S2.S2KD = "1312";
                    S2.S2SC = "X";
                    S2.S2BD = teminat.TeminatBedeli.Value.ToString(); //Canlı sistemde kaldırılacak.
                    ekTaminatlar.Add(S2);
                }
            }

            //KAPKAÇ TEMİNATI
            if (teklif.ReadSoru(KonutSorular.Kapkac, false))
            {
                TeklifTeminat teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.Kapkac).FirstOrDefault();
                if (teminat != null && teminat.TeminatBedeli.HasValue)
                {
                    S2 S2 = new S2();
                    S2.S2KD = "3325";
                    S2.S2SC = "X";
                    S2.S2BD = teminat.TeminatBedeli.Value.ToString();
                    ekTaminatlar.Add(S2);
                }
            }

            //KONUT ASİSTAN
            if (teklif.ReadSoru(KonutSorular.AsistanHizmeti, false))
            {
                S2 S2 = new S2();
                S2.S2KD = "1999";
                S2.S2SC = "X";
                ekTaminatlar.Add(S2);
            }

            //HUKUKSAL KORUMA
            if (teklif.ReadSoru(KonutSorular.HukuksalKoruma, false))
            {
                S2 S2 = new S2();
                S2.S2KD = "2305";
                S2.S2SC = "X";
                ekTaminatlar.Add(S2);
            }

            //FERDİ KAZA
            if (teklif.ReadSoru(KonutSorular.FerdiKaza, false))
            {
                S2 S2 = new S2();
                S2.S2KD = "3120";
                S2.S2SC = "X";
                ekTaminatlar.Add(S2);
            }

            //MEDLINE
            if (teklif.ReadSoru(KonutSorular.Medline, false))
            {
                S2 S2 = new S2();
                S2.S2KD = "7999";
                S2.S2SC = "X";
                ekTaminatlar.Add(S2);
            }

            //ACİL TIBBI/HASTANE/FERDİKAZA
            if (teklif.ReadSoru(KonutSorular.AcilTibbiHastaneFerdiKaza, false))
            {
                S2 S2 = new S2();
                S2.S2KD = "7101";
                S2.S2SC = "X";
                ekTaminatlar.Add(S2);
            }


            //3.ŞAHIS MALİ SORUMLULUK
            if (teklif.ReadSoru(KonutSorular.MaliMesuliyetEkTeminat, false))
            {
                TeklifTeminat teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetEkTeminat).FirstOrDefault();
                if (teminat != null && teminat.TeminatBedeli.HasValue)
                {
                    S2 S2 = new S2();
                    S2.S2KD = "3104";
                    S2.S2SC = "X";
                    S2.S2BD = teminat.TeminatBedeli.Value.ToString();
                    ekTaminatlar.Add(S2);
                }
            }


            //CAM TEMİNATI
            if (teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
            {
                TeklifTeminat teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.CamKirilmasi).FirstOrDefault();
                if (teminat != null && teminat.TeminatBedeli.HasValue)
                {
                    S2 S2 = new S2();
                    S2.S2KD = "3102";
                    S2.S2SC = "X";
                    S2.S2BD = teminat.TeminatBedeli.Value.ToString();
                    ekTaminatlar.Add(S2);
                }
            }


            //1342	İZOLAS./OLAY BŞ/YIL.
            if (teklif.ReadSoru(KonutSorular.IzolasOlayBsYil, false))
            {
                TeklifTeminat teminat = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.IzolasOlayBsYil).FirstOrDefault();
                if (teminat != null && teminat.TeminatBedeli.HasValue)
                {
                    S2 S2 = new S2();
                    S2.S2KD = "1342";
                    S2.S2SC = "X";
                    S2.S2BD = teminat.TeminatBedeli.Value.ToString();
                    ekTaminatlar.Add(S2);
                }
            }


            request.EKTM = ekTaminatlar;

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

            //Peşin İndirim 
            request.WSIND = String.Empty;
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
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu,
                                                                                                                    TeklifUretimMerkezleri.HDI });
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_PDFServiceURL);

                HDIPDFRequest request = new HDIPDFRequest();
                request.PoliceNumarasi = this.GenelBilgiler.TUMPoliceNo;
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;

                request.Uygulama = "122";//_KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaKonut);
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
                    string fileName = String.Format("konut_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("konut", fileName, data);
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

        public void AdresDoldur(HDIKonutRequest request, MusteriAdre adres)
        {
            if (adres.UlkeKodu != "TUR")
            {
                request.IlKod = "999";
                request.Ilce = "YURTDIŞI";
            }
            else
            {
                CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.HDI &&
                                                                              f.IlKodu == adres.IlKodu &&
                                                                              f.IlceKodu == adres.IlceKodu)
                                                                                                         .Single<CR_IlIlce>();

                if (ililce != null)
                {
                    request.Ilce = ililce.CRIlceAdi;
                    request.IlKod = ililce.CRIlKodu;
                }
            }
            request.Cadde = adres.Cadde;
            request.Sokak = adres.Sokak;
            request.Semt = adres.Semt;
            request.KoyMahalle = adres.Mahalle;
            request.BinaNo = adres.BinaNo;
            request.Daire = adres.DaireNo;
            request.HanApartmanAd = adres.Apartman;
            request.PostaKod = adres.PostaKodu.ToString();
            request.Adres = adres.Adres;
        }
    }
}

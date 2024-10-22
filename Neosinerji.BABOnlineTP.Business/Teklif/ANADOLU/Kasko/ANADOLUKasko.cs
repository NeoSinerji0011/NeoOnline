using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUKasko : Teklif, IANADOLUKasko
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
        IAktifKullaniciService _AktifKullaniciService;
        [InjectionConstructor]
        public ANADOLUKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IANADOLUTrafik trafik, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
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
            _AktifKullaniciService = aktifKullaniciService;
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
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });
                ClientIPNo = this.IpGetir(_AktifKullaniciService.TVMKodu);
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET request = new PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET();

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                #endregion

                #region Anadolu sigorta müşterisi
                string MUSTERI_NO = _CRService.GetTUMMusteriKodu(this.TUMKodu, sigortali.MusteriKodu);

                if (String.IsNullOrEmpty(MUSTERI_NO))
                {
                    MUSTERI_NO = this.MusteriKaydet(teklif, sigortali, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl]);

                    if (String.IsNullOrEmpty(MUSTERI_NO))
                    {
                        this.Hatalar.Add("ANADOLU SİGORTA sisteminde müşteri oluşturulamadı.");
                        this.Import(teklif);
                        this.GenelBilgiler.Basarili = false;
                        return;
                    }
                }
                #endregion

                #region Genel Bilgiler

                //“TL” gönderilmelidir.
                request.URUN_DOVIZ_CINSI = "TL";

                DateTime baslangicTarihi = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                request.BASLANGIC_TARIHI = baslangicTarihi.ToString("yyyyMMdd");
                request.BITIS_TARIHI = baslangicTarihi.AddYears(1).ToString("yyyyMMdd");

                #endregion

                #region Müşteri Bilgileri
                request.MUSTERI_NO = MUSTERI_NO;
                request.E_POSTA = sigortali.EMail;
                request.SIGORTAETTIRENAYNIZAMANDASIGORTALIMI = "";
                #endregion

                #region Araç Bilgileri

                //AracinTip uzunluğu 1 veya 2 hane ise araç tipinin başına 0 ekliyor.
                string aracKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(3, '0');

                string plaka = teklif.Arac.PlakaNo;

                if (plaka.Length == 5)
                {
                    string YeniPlaka = "";
                    int sayac = 0;
                    foreach (var item in plaka)
                    {
                        if (char.IsDigit(item) && sayac == 0)
                        {
                            YeniPlaka += " ";
                            sayac++;
                        }
                        YeniPlaka += item;
                    }
                    plaka = YeniPlaka;
                }
                if (plaka == "YK" || plaka == "G 9999")
                {
                    request.PLAKA_NO = "G 9999";
                    request.PLAKA_TURU = "Gecici";
                }
                else
                {
                    request.PLAKA_NO = plaka;
                    request.PLAKA_TURU = "Yerli";
                }

                request.MODEL_YILI = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value.ToString() : String.Empty;
                request.PLAKA_IL_KODU = teklif.Arac.PlakaKodu;
                request.ARAC_KODU = aracKodu.TrimStart('0');
                request.ARAC_OPERASYONEL_KIRALIK = "0";
                request.YENI_ARAC_BEDELI = teklif.Arac.AracDeger.HasValue ? teklif.Arac.AracDeger.Value.ToString() : "";
                request.ARAC_TESCIL_KODU = !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) ? teklif.Arac.TescilSeriKod : "";
                request.ARAC_TESCIL_SERI_NO = !String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) ? teklif.Arac.TescilSeriNo : teklif.Arac.AsbisNo;


                //KASKO IKAME
                request.IKAME_ARAC_SECIM = teklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false) ? "1" : "0";
                if (request.IKAME_ARAC_SECIM == "1")
                {
                    string ikameKodu = teklif.ReadSoru(KaskoSorular.AnadoluIkameTuru, String.Empty);
                    request.IKAME_ARAC_BILGILERI = ikameKodu;
                }

                string TRAMER_TARIFE_KODU = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }
                #region Anadolu Sigorta Marka Kodu Getir

                var MarkaAdi = _AracContext.AracMarkaRepository.Find(s => s.MarkaKodu == teklif.Arac.Marka).MarkaAdi;

                if (teklif.Arac.Marka == "122")
                {
                    MarkaAdi = "RENAULT";
                }
                ANADOLUService servisMarka = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servisMarka.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL requestMarka = new PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL();
                PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response responseMarkaList = servisMarka.CallService<PR_SWEP_URT_OTO_WS_GET_MARKA_FOR_MOBIL_Response>(requestMarka);
                string AnadoluMarkaKodu = String.Empty;

                if (responseMarkaList.HATA_KODU == "0" && responseMarkaList.MARKA_BILGISI != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var jsonArray = js.Deserialize<List<object>>(responseMarkaList.MARKA_BILGISI);
                    if (jsonArray != null)
                    {
                        var anadoluList = jsonArray.ConvertToMarka();
                        var markaVarmi = anadoluList.Where(m => m.Marka == MarkaAdi).FirstOrDefault();
                        if (markaVarmi != null)
                        {
                            AnadoluMarkaKodu = markaVarmi.Value;
                        }
                    }
                }

                #endregion

                string AnadoluKullanimTipi = teklif.ReadSoru(KaskoSorular.AnadoluKullanimTipi, "");
                string AnadoluKullanimSekli = teklif.ReadSoru(KaskoSorular.AnadoluKullanimSekli, "");

                if (!String.IsNullOrEmpty(AnadoluKullanimTipi))
                {
                    request.KULLANIM_TIPI_KODU = AnadoluKullanimTipi;
                }
                else
                {
                    request.KULLANIM_TIPI_KODU = _Trafik.AracKullanimTipi(request.MODEL_YILI, AnadoluMarkaKodu, TRAMER_TARIFE_KODU, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl], ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                }
                if (!String.IsNullOrEmpty(AnadoluKullanimSekli))
                {
                    request.KULLANIM_SEKLI_KODU = AnadoluKullanimSekli;
                }
                else
                {
                    request.KULLANIM_SEKLI_KODU = _Trafik.AracKullanimSekli(request.MODEL_YILI, AnadoluMarkaKodu, TRAMER_TARIFE_KODU, request.KULLANIM_TIPI_KODU, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl], ANADOLUKullanimTipiUrunGrupAdi.Kasko);
                }


                switch (teklif.Arac.KullanimSekli)
                {
                    case "0": request.KULLANIM_AMACI = "Ticari"; break;
                    case "1": request.KULLANIM_AMACI = "Resmi"; break;
                    case "2": request.KULLANIM_AMACI = "Hususi"; break;
                }


                if (teklif.Arac.TrafikTescilTarihi.HasValue)
                    request.TESCIL_TARIHI = teklif.Arac.TrafikTescilTarihi.Value.ToString("yyyyMMdd");
                #endregion

                #region Teminatlar
                // ==== Zorunlu olmayan alanlar ==== //

                //KFK Yolcu adedi. Eğer boş girilir ise araç varsayılan değeri alınır
                request.YOLCU_SAYISI = String.Empty;

                //Hasarsızlık koruma sözleşme maddesi seçimi. (“0” ise sözleşme maddesi seçilmemiş, “1” ise tek hasar koruma, “2” ise iki hasar koruma)
                request.HASARSIZLIK_KORUMA_SZLM_SECIM = "";

                //Deprem muafiyeti verilecek mi? (“E”:Evet, “H”:Hayır) (Varsayılan E)
                bool depremVarYok = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
                if (depremVarYok)
                    request.DEPREM_MUAFIYET_SECIM = "E";
                else
                    request.DEPREM_MUAFIYET_SECIM = "H";
                request.KFK_TEMINAT_LISTESI = String.Empty;

                // ==== KFK_TEMINAT_LISTESI	Hayır	Bedel	Ölüm, sürekli sakatlık, tedavi limitleri JSON array==== //
                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKademe))
                {
                    CR_KaskoFK CRBedel = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                    int Vefat = 5000;
                    int Sakatlik = 5000;
                    int Tedavi = 500;
                    if (FKBedel.Vefat.HasValue)
                    {
                        if (FKBedel.Vefat.Value >= 5000 && FKBedel.Vefat.Value <= 50000)
                        {
                            Vefat = (int)FKBedel.Vefat.Value;
                            Sakatlik = (int)FKBedel.Sakatlik.Value;
                            Tedavi = (int)FKBedel.Tedavi.Value;
                        }
                        else if (FKBedel.Vefat.Value > 50000)
                        {
                            Vefat = 50000;
                            Sakatlik = 50000;
                            Tedavi = 5000;
                        }
                    }
                    request.KFK_TEMINAT_LISTESI = "[[\"Ölüm\",\"" + Vefat + "\"],[\"sürekli sakatlık\",\"" + Sakatlik + "\"],[\"tedavi \",\"" + Tedavi + "\"]]";

                }
                // request.KFK_TEMINAT_LISTESI = "[[\"Ölüm\",\"5000\"],[\"sürekli sakatlık\",\"5000 \"],[\"tedavi \",\"500\"]]";

                // ==== IMM ==== //
                string imm = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(imm))
                {
                    ////Artan Mali Sorumluluk kademesi (‘525’,’526’,’527’,’528’,’529’,’530’) değerlerinden biri gönderilmelidir.
                    //request.IMM_SECILI_KADEME = "525";

                    CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(imm), parts[0], parts[1]);

                    if (IMMBedel != null)
                    {
                        CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.ANADOLU, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                    }
                    if (CRKademeNo != null)
                    {
                        request.IMM_SECILI_KADEME = CRKademeNo.Kademe.ToString();
                    }

                    //Artan mali sorumluluk teminat yapısı (Şu an sadece Kombine Tek Limit olduğu için boş yada ‘KombineTekLimit’ gönderilecektir)
                    request.IMM_TEMINAT_YAPISI = "KombineTekLimit";
                }

                request.SES_CIHAZ_BEDEL = String.Empty;

                #region Araç aksesuarlar

                string donanimListesi = "";

                if (teklif.AracEkSorular.Count > 0)
                {
                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        donanimListesi += "[{\"";
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.LPG)
                            {
                                donanimListesi += "tip:\"\"" + ANADOLUAltDonanimListesi.LPGDonanimi + "\"],[\"model\":\"" + item.Aciklama + "\"],[\"bedel \":\"" + item.Bedel + "\"},";
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Jant)
                            {
                                donanimListesi += "tip:\"\"" + ANADOLUAltDonanimListesi.Jant + "\"],[\"model\":\"" + item.Aciklama + "\"],[\"bedel \":\"" + item.Bedel + "\"},";
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Kasa)
                            {
                                donanimListesi += "tip:\"\"" + ANADOLUAltDonanimListesi.Kasa + "\"],[\"model\":\"" + item.Aciklama + "\"],[\"bedel \":\"" + item.Bedel + "\"},";
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Diger)
                            {
                                donanimListesi += "tip:\"\"" + ANADOLUAltDonanimListesi.Diger1 + "\"],[\"model\":\"" + item.Aciklama + "\"],[\"bedel \":\"" + item.Bedel + "\"},";
                            }
                        }
                        if (aksesuarlar.Count > 1)
                        {
                            donanimListesi += "]";
                        }
                    }
                }

                #endregion

                request.DONANIM_LISTESI = donanimListesi;
                request.GORUNTU_CIHAZ_BEDEL = String.Empty;
                request.ILETISIM_CIHAZ_BEDEL = String.Empty;
                request.SOFOR_SAYISI = String.Empty;
                request.MUAVIN_SAYISI = String.Empty;

                request.KASKO_MUAFIYET_KADEME = String.Empty;
                request.KULLANIM_GELIR_KAYBI_SECIM = String.Empty;
                request.SIGORTALI_SIFATI = String.Empty;
                request.MENFAAT_SAHIBI_LISTESI = String.Empty;
                request.KISISEL_ESYA = String.Empty;
                request.RETURN_TEKLIF_PDF = String.Empty;
                request.RETURN_BILGILENDIRME_FORMU_PDF = String.Empty;
                request.RETURN_GENEL_SART_PDF = String.Empty;

                var camMuafiyeti = teklif.ReadSoru(KaskoSorular.CamMuafiyetiKaldirilsinMi, true);
                if (camMuafiyeti)
                {
                    request.CAM_HASARI_KORUMA_SZLM_SECIM = "1";
                }
                //“0” iletilirse gönderilmez. Bu parametre olmadığı veya 0 dan farklı olduğu durumlarda işleyiş mevcuttaki gibi devam edecektir.
                request.SEND_EPOSTA = "0";

                // this.GetAnadoluMenfaatler(tvmKodu,servisKullanici);

                #endregion

                #region Önceki Poliçe
                bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
                if (eskiPoliceVar)
                {
                    request.ONCEKI_SIRKET_KODU = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    request.ONCEKI_ACENTE_KODU = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                    request.ONCEKI_POLICE_NO = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                    request.ONCEKI_YENILEME_NO = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);
                }
                #endregion

                #region Ödeme Seçenekleri
                request.ODEME_TIPI = "";
                request.TAKSIT_SAYISI = "";
                //request.ODEME_TIPI = "DGR0010174";
                //request.TAKSIT_SAYISI = "TR00000196";

                byte? odemetipi = teklif.GenelBilgiler.OdemeTipi ?? 0;
                byte? odemesekli = teklif.GenelBilgiler.OdemeSekli ?? 0;
                byte taksitsayisi = teklif.GenelBilgiler.TaksitSayisi ?? 1;

                if (odemesekli == OdemeSekilleri.Vadeli)
                {
                    switch (odemetipi)
                    {
                        case OdemeTipleri.Nakit: request.ODEME_TIPI = AnadoluOdemeTipleri.Nakit; break;
                        case OdemeTipleri.KrediKarti: request.ODEME_TIPI = AnadoluOdemeTipleri.KrediKartiTalimati; break;
                        case OdemeTipleri.Havale: request.ODEME_TIPI = AnadoluOdemeTipleri.SanalPostMailOrder; break;
                        case OdemeTipleri.BlokeliKrediKarti: request.ODEME_TIPI = AnadoluOdemeTipleri.KrediKartiBlokeliTaksitGenel; break;
                    }

                    if (odemetipi == OdemeTipleri.KrediKarti || odemetipi == OdemeTipleri.BlokeliKrediKarti)
                        switch (taksitsayisi)
                        {
                            case 2: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_2; break;
                            case 3: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_3; break;
                            case 4: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_4; break;
                            case 5: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_5; break;
                            case 6: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_6; break;
                            case 7: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_7; break;
                            case 8: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_8; break;
                            case 9: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_9; break;
                            case 10: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                            case 11: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                            case 12: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                        }
                    else if (odemetipi == OdemeTipleri.Nakit)
                        request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                    else if (odemetipi == OdemeTipleri.Havale)
                        request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                }
                else
                {
                    request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                    request.ODEME_TIPI = AnadoluOdemeTipleri.SanalPostMailOrder;
                }
                #endregion

                #region Servis call

                //“Birleşik Kasko Sigortası Yeni” gönderilmelidir.
                request.GRUP_URUN_ADI = "Birleşik Kasko Sigortası Yeni";

                //Acente Kodu (İş Bankası’ndan şube kodu gönderiliyor. Satış kanallarımız test ortamı için “310145” değeri gönderebilir)
                //request.ACENTE_KODU = "310145";
                request.ACENTE_KODU = servisKullanici.PartajNo_;

                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;

                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Teklif);

                var response = new PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET_Response();
                if (teklif.Arac.Marka == "600")
                {
                    var res = new PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET_Response();
                    this.EndLog(res, false, res.GetType());
                    this.AddHata("Hata! Anadolu Sigorta, Markası Motosiklet olan araçlara Online Teklif düzenletmemektedir.");
                }
                else
                {
                    response = servis.CallService<PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET_Response>(request);

                    #region Varsa Hata Kaydı
                    if (response.HATA_KODU != "0")
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.HATA_TEXT);
                    }
                    else
                    {
                        this.EndLog(response, true, response.GetType());
                    }
                    #endregion
                }
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
                this.GenelBilgiler.BrutPrim = ANADOLUMessage.ToDecimal(response.PESIN_BRUT_PRIM);
                decimal GiderVergisi = ANADOLUMessage.ToDecimal(response.PESIN_BSMV);
                this.GenelBilgiler.ToplamVergi = GiderVergisi;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = ANADOLUMessage.ToDecimal(response.KOMISYON_TUTARI);
                this.GenelBilgiler.TUMTeklifNo = response.TEKLIF_NO;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                string HasarsizlikIndirimOrani = response.HASARSIZLIK_ORANI;
                if (!String.IsNullOrEmpty(HasarsizlikIndirimOrani))
                {
                    if (HasarsizlikIndirimOrani.Contains("-"))
                    {

                        var convert = Convert.ToDecimal(HasarsizlikIndirimOrani) * -1;
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = convert;
                    }
                    else
                    {
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToDecimal(HasarsizlikIndirimOrani);
                    }
                }

                #endregion

                #region Vergiler
                this.AddVergi(KaskoVergiler.GiderVergisi, GiderVergisi);
                #endregion

                #region Teminatlar

                //PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response Teminatlar = GetTeklifTeminat(request, konfig);

                //if (Teminatlar != null && Teminatlar.HATA_KODU == "0")
                //{

                //}

                /*
                 Olay başına azami avas, Olay başına azami kefalet, Olay başına azami limit, Sigorta süresi içinde azami limit, Teminatlarını içeriyor.
                (Teminatlar oluşturuluken bu teminatlar verilecek.) Teminat bedelleri sırayısyla 750,750,3750 ve 11000 TL olacak. (Default değerler) 
                */
                this.AddTeminat(KaskoTeminatlar.HK_Olay_Basina_Azami_Avans, 750, 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.HK_Olay_Basina_Azami_Kefalet, 750, 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.HK_Olay_Basina_Azami_Limit, 3550, 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.HK_Sigorta_Suresi_Icinde_Azami_Limit, 11000, 0, 0, 0, 0);

                /*  
                 Artan mali mesuliyet teminatı verilmiş ise IMM_TEMINAT_YAPISI KombineTekLimit olarak set edilecek.
                 Teminatları oluştururken KOMBİNE teminatı verilecek. Ve teminat limiti 100000 TL olarak set edilecek. (Default değer)
                 */
                this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, 100000, 0, 0, 0, 0);


                // KFK_TEMINAT_LISTESI olarak (Ölüm: 5000, Sürekli Sakatlık: 5000 ve Tedavi: 500)
                this.AddTeminat(KaskoTeminatlar.KFK_Olum, 5000, 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, 5000, 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, 500, 0, 0, 0, 0);


                // Mini Onarım Hizmet ve Hayvanların Vereceği zarar otomatik olarak veriliyor. 
                this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, 1, 0, 0, 0, 0);


                //Anadolu sigortaya Asistans teminatı default olarak verilecek. Ve tabloda teminat tikli gelecek.
                this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, 1, 0, 0, 0, 0);
                if (camMuafiyeti)
                {
                    this.AddTeminat(KaskoTeminatlar.CamMuafiyeti, 0, 0, 0, 0, 0);
                }

                /*
                 [[\"Hukuksal Koruma\",\"10.00\"],[\"Kasko\",\"1409.04\"],[\"Koltuk Ferdi Kaza\",\"14.16\"],
                 [\"Mini Onarım\",\"0.00\"],[\"Anadolu Hizmet ve İkame Araç\",\"180.00\"],[\"Artan Mali Sorumluluk\",\"97.75\"]]
                 */
                if (!String.IsNullOrEmpty(response.BILESEN_PRIM_DETAY))
                {
                    JavaScriptSerializer serialize = new JavaScriptSerializer();
                    object[] obj = new object[5];
                    obj = serialize.Deserialize<object[]>(response.BILESEN_PRIM_DETAY);

                    object[] hukuksalKoruma = (object[])obj[0];
                    object[] kasko = (object[])obj[1];
                    object[] KFK = (object[])obj[2];
                    object[] miniOnarim = (object[])obj[3];
                    object[] ikameArac = (object[])obj[4];
                    object[] artanMaliSorumluluk = (object[])obj[5];

                    //this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, decimal.Parse(hukuksalKoruma[1].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, 0);
                    //this.AddTeminat(KaskoTeminatlar.Kasko, decimal.Parse(kasko[1].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, 0);
                    //this.AddTeminat(KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu, decimal.Parse(KFK[1].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, 0);
                    //this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, decimal.Parse(miniOnarim[1].ToString(), CultureInfo.InvariantCulture), 0, 0, 0, 0);
                }

                #endregion

                #region PDF
                // ==== BILGILENDIRME_FORMU_PDF	String	PDF dosyasının şifreli hali Base64
                if (!String.IsNullOrEmpty(response.BILGILENDIRME_FORMU_PDF))
                {
                    byte[] data = Convert.FromBase64String(response.BILGILENDIRME_FORMU_PDF);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Anadolu_Kasko_Bilgilendirme_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("kasko", fileName, data);
                    this.GenelBilgiler.PDFBilgilendirme = url;

                    _Log.Info("PDF_Bilgilendirme url: {0}", url);
                }

                // ==== GENEL_SART_PDF	String	PDF dosyasının şifreli hali Base64
                if (!String.IsNullOrEmpty(response.GENEL_SART_PDF))
                {
                    byte[] data = Convert.FromBase64String(response.GENEL_SART_PDF);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Anadolu_Kasko_GenelSartlar_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("kasko", fileName, data);
                    this.GenelBilgiler.PDFGenelSartlari = url;

                    _Log.Info("PDF_Genel şartlar url: {0}", url);
                }

                // ==== TEKLIF_PDF	String	PDF dosyasının şifreli hali Base64
                if (!String.IsNullOrEmpty(response.TEKLIF_PDF))
                {
                    byte[] data = Convert.FromBase64String(response.TEKLIF_PDF);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Anadolu_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("kasko", fileName, data);
                    this.GenelBilgiler.PDFDosyasi = url;

                    _Log.Info("PDF_Teklif url: {0}", url);
                }
                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    this.AddOdemePlaniALL(teklif);
                }
                #endregion

                #region WebServis Cevap
                this.AddWebServisCevap(Common.WebServisCevaplar.ANADOLU_Teklif_Id, response.TEKLIF_NO);
                string AnadoluBilgiMesaji = "";
                if (!String.IsNullOrEmpty(response.BILGI_TEXT))
                {
                    AnadoluBilgiMesaji = response.BILGI_TEXT;
                    if (AnadoluBilgiMesaji.Length <= 1000)
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AnadoluBilgiMesaji);
                    }
                    else
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AnadoluBilgiMesaji.Substring(0, 999));
                    }
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
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                #endregion

                #region Anadolu sigorta müşterisi
                string MUSTERI_NO = _CRService.GetTUMMusteriKodu(this.TUMKodu, sigortali.MusteriKodu);

                if (String.IsNullOrEmpty(MUSTERI_NO))
                {
                    MUSTERI_NO = this.MusteriKaydet(this, sigortali, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.ANADOLU_ServiceUrl]);

                    if (String.IsNullOrEmpty(MUSTERI_NO))
                    {
                        this.Hatalar.Add("ANADOLU SİGORTA sisteminde müşteri oluşturulamadı.");
                        this.GenelBilgiler.Basarili = false;
                        this.GenelBilgiler.WEBServisLogs = this.Log;
                        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                        return;
                    }
                }
                #endregion

                #region Araç Bilgileri
                PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_POLICELENDIR request = new PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_POLICELENDIR();


                request.TEKLIF_NO = this.ReadWebServisCevap(Common.WebServisCevaplar.ANADOLU_Teklif_Id, "0");
                request.REVIZYON_NO = "0";
                request.MOTOR_NO = teklif.Arac.MotorNo;
                request.SASI_NO = teklif.Arac.SasiNo;


                /*RETURN_GENEL_SART_PDF	Hayır	String	“0” iletilirse, genel şart pdf’i gönderilmez. “ “ boş ise 
                  veya 0 dan farklı olduğu durumlarda poliçe pdf’i gönderilir.*/
                request.RETURN_GENEL_SART_PDF = "0";


                /* RETURN_BILGILENDIRME_ FORMU_PDF	Hayır	String	0” iletilirse, bilgilendirme formu pdf’i gönderilmez. “ “ boş ise 
                   veya 0 dan farklı olduğu durumlarda poliçe pdf’i gönderilir*/
                request.RETURN_BILGILENDIRME_FORMU_PDF = "0";


                //RETURN_POLICE_PDF	Hayır	String	“0” iletilirse, poliçe pdf’i gönderilmez. “ “ boş ise veya 0 dan farklı olduğu durumlarda poliçe pdf’i gönderilir.
                request.RETURN_POLICE_PDF = "";



                if (teklif.Arac.TrafikCikisTarihi.HasValue)
                    request.TRAFIGE_CIKIS_TARIHI = teklif.Arac.TrafikCikisTarihi.Value.ToString("yyyyMMdd");
                else
                    request.TRAFIGE_CIKIS_TARIHI = String.Empty;


                // ==== Zorunlu olmayan alanlar ==== //
                request.KILOMETRE = String.Empty;
                request.ARAC_RENGI = String.Empty;
                request.ARAC_ALT_RENGI = String.Empty;
                request.SIGORTAETTIRENAYNIZAMANDASIGORTALIMI = String.Empty;
                request.MUAYENE_GECERLILIK_TARIHI = String.Empty;
                request.SIGORTALI_SIFATI = String.Empty;
                request.MENFAAT_SAHIBI_LISTESI = String.Empty;
                request.TAKOMETRE = String.Empty;
                request.GORUNTU_CIHAZ_LISTESI = String.Empty;
                request.ILETISIM_CIHAZ_LISTESI = String.Empty;
                request.SES_CIHAZ_LISTESI = String.Empty;


                string aracKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi;


                string TRAMER_TARIFE_KODU = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }

                #endregion

                #region Ödeme Bilgileri

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.KREDI_KART_NO = odeme.KrediKarti.KartNo.Substring(0, 6);
                    request.CVV2 = "XXX";
                    request.SON_KULLANMA_TARIHI = "999999";
                    request.TAKSIT_SAYISI = odeme.TaksitSayisi.ToString();
                }

                byte taksitsayisi = odeme.TaksitSayisi;


                if (odeme.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    switch (odeme.OdemeTipi)
                    {
                        case OdemeTipleri.Nakit: request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.Nakit; break;
                        case OdemeTipleri.KrediKarti: request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.KrediKartiBlokeliTaksitGenel; break;
                        case OdemeTipleri.Havale: request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.Nakit; break;
                    }

                    if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                        switch (taksitsayisi)
                        {
                            case 2: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_2; break;
                            case 3: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_3; break;
                            case 4: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_4; break;
                            case 5: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_5; break;
                            case 6: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_6; break;
                            case 7: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_7; break;
                            case 8: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_8; break;
                            case 9: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_9; break;
                            case 10: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                            case 11: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                            case 12: request.TAKSIT_SAYISI = AnadoluOdemeTipiKrediK_Blokeli_T_Genel.BlokeliTaksit_10; break;
                        }
                    else if (odeme.OdemeTipi == OdemeTipleri.Nakit)
                        request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                    else if (odeme.OdemeTipi == OdemeTipleri.Havale)
                        request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                }
                else
                {
                    request.TAKSIT_SAYISI = AnadoluOdemeTipiNakit.Pesin;
                    request.SECILEN_ODEME_TIPI = AnadoluOdemeTipleri.SanalPostMailOrder;
                }

                //request.SECILEN_ODEME_TIPI = "DGR0010174";
                //request.TAKSIT_SAYISI = "TR00000196";

                request.PESIN_BLOKELI_TAKSIT = String.Empty;

                MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                if (telefon != null)
                {
                    string[] telParts = telefon.Numara.Split('-');

                    if (telParts.Length == 3)
                    {
                        request.KART_TEL_ALAN = telParts[1];
                        request.KART_TEL_NO = telParts[2];
                    }
                }
                #endregion

                //test
                //request.KREDI_KART_NO = "5126515200012347";
                //request.SON_KULLANMA_TARIHI = "012014";
                //request.CVV2 = "820";
                //request.KART_TEL_ALAN = "545";
                //request.KART_TEL_NO = "5568775";

                #region service call
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ANADOLU });

                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;
                ClientIPNo = this.IpGetir(_AktifKullaniciService.TVMKodu);
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.Police);

                //Kredi kartı bilgileri Loglanmıyor.
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.KREDI_KART_NO = odeme.KrediKarti.KartNo;
                    request.SON_KULLANMA_TARIHI = odeme.KrediKarti.SKA + odeme.KrediKarti.SKY;
                    request.CVV2 = odeme.KrediKarti.CVC;
                }
                else
                {
                    request.KREDI_KART_NO = "";
                    request.SON_KULLANMA_TARIHI = "";
                    request.CVV2 = "";
                }
                PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_POLICELENDIR_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_POLICELENDIR_Response>(request);
                #endregion

                #region Hata Kontrol Ve kayıt

                if (response.HATA_KODU != "0")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.HATA_TEXT);
                }
                else
                    this.EndLog(response, true, response.GetType());


                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.POLICE_NO;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                    if (!String.IsNullOrEmpty(response.KOMISYON_TUTARI))
                        this.GenelBilgiler.ToplamKomisyon = ANADOLUMessage.ToDecimal(response.KOMISYON_TUTARI);

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    // ==== POLICE_PDF	String	PDF dosyasının şifreli hali Base64
                    if (!String.IsNullOrEmpty(response.POLICE_PDF))
                    {
                        byte[] data = Convert.FromBase64String(response.POLICE_PDF);

                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        string fileName = String.Format("Anadolu_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                        string url = storage.UploadFile("kasko", fileName, data);
                        this.GenelBilgiler.PDFPolice = url;

                        _Log.Info("Police_PDF url: {0}", url);
                    }

                    #region OdemePlani


                    #endregion
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

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        public PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response GetTeklifTeminat(PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET request, KonfigTable konfig, TVMWebServisKullanicilari servisKullanici)
        {
            PR_SWEP_URT_OTO_WS_KASKO_TEMINAT req = new PR_SWEP_URT_OTO_WS_KASKO_TEMINAT();

            req.ARAC_KODU = request.ARAC_KODU;
            req.KULLANIM_SEKLI_KODU = request.KULLANIM_SEKLI_KODU;
            req.KULLANIM_TIPI_KODU = request.KULLANIM_TIPI_KODU;
            req.WEB_KULLANICI_ADI = request.WEB_KULLANICI_ADI;
            req.WEB_SIFRE = request.WEB_SIFRE;
            ClientIPNo = this.IpGetir(_AktifKullaniciService.TVMKodu);
            ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
            servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];

            PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response response = servis.CallService<PR_SWEP_URT_OTO_WS_KASKO_TEMINAT_Response>(req);

            return response;
        }

        public string MusteriKaydet(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl)
        {
            string MUSTERI_NO = String.Empty;
            try
            {
                #region Ana bilgiler
                PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET request = new PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET();
                switch (sigortali.MusteriTipKodu)
                {
                    case MusteriTipleri.TCMusteri: request.MUSTERI_TIPI = "tcozel"; break;
                    case MusteriTipleri.TuzelMusteri: request.MUSTERI_TIPI = "tctuzel"; break;
                    case MusteriTipleri.SahisFirmasi: request.MUSTERI_TIPI = "tctuzel"; break;
                    case MusteriTipleri.YabanciMusteri: request.MUSTERI_TIPI = "yozel"; break;
                    default: request.MUSTERI_TIPI = String.Empty; break;
                }

                if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                {
                    request.TCKN = sigortali.KimlikNo;
                    request.MUSTERI_ADI = sigortali.AdiUnvan;
                    request.MUSTERI_SOYADI = sigortali.SoyadiUnvan;
                    if (sigortali.DogumTarihi.HasValue)
                        request.DOGUM_TARIHI = sigortali.DogumTarihi.Value.ToString("yyyyMMdd");
                    request.CINSIYET = sigortali.Cinsiyet;
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri || sigortali.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                {
                    request.VKN = sigortali.KimlikNo;
                    request.UNVAN = String.Format("{0} {1}", sigortali.AdiUnvan, sigortali.SoyadiUnvan);
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                {
                    request.YABANCI_KIMLIK_NO = sigortali.KimlikNo;
                    request.ULKE_KODU = String.Empty;
                }

                request.MUSTERI_EPOSTA = sigortali.EMail;
                #endregion

                #region Adres ve telefon
                MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                if (adres != null)
                {
                    CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                  f.IlKodu == adres.IlKodu &&
                                                                                  f.IlceKodu == adres.IlceKodu)
                                                                    .SingleOrDefault<CR_IlIlce>();

                    if (ililce != null)
                    {
                        switch (adres.AdresTipi)
                        {
                            case AdresTipleri.Ev: request.ADRES_TIPI = "Ev"; break;
                            case AdresTipleri.Is: request.ADRES_TIPI = "İş"; break;
                            case AdresTipleri.Diger: request.ADRES_TIPI = "Diğer"; break;
                            default: request.ADRES_TIPI = "Diğer"; break;
                        }

                        request.ADRES_IL_KODU = ililce.CRIlKodu;
                        request.ILCE_ADI = ililce.CRIlceAdi;
                        request.ILCE_KODU = ililce.CRIlceKodu;
                    }

                    //request.MUSTERI_BINA_ADI = String.Empty;
                    //request.MUSTERI_MAHALLE_KOY = adres.Mahalle;
                    //request.MUSTERI_CADDE = adres.Cadde;
                    //request.MUSTERI_SOKAK = adres.Sokak;
                    //request.MUSTERI_KAPI_NO = adres.BinaNo;
                    //request.MUSTERI_DAIRE_NO = adres.DaireNo;
                    request.MUSTERI_SERBESTADRES = adres.Adres;
                }
                string SE_NumaraTipi = teklif.ReadSoru(TrafikSorular.SENumaratipi, "");
                short numaratipi = IletisimNumaraTipleri.Cep;

                MusteriTelefon telefon = new MusteriTelefon();

                if (!String.IsNullOrEmpty(SE_NumaraTipi))
                {
                    numaratipi = Convert.ToInt16(SE_NumaraTipi);

                }
                telefon = sigortali.MusteriTelefons.FirstOrDefault(s => s.IletisimNumaraTipi == numaratipi);
                if (telefon != null)
                {
                    string[] parts = telefon.Numara.Split('-');

                    if (parts.Length == 3)
                    {
                        switch (telefon.IletisimNumaraTipi)
                        {
                            case IletisimNumaraTipleri.Ev: request.MUSTERI_TEL_TIP = "Ev"; break;
                            case IletisimNumaraTipleri.Is: request.MUSTERI_TEL_TIP = "İş"; break;
                            case IletisimNumaraTipleri.Cep: request.MUSTERI_TEL_TIP = "Cep"; break;
                            case IletisimNumaraTipleri.Fax: request.MUSTERI_TEL_TIP = "Faks"; break;
                            default: request.MUSTERI_TEL_TIP = "Ev"; break;
                        }

                        request.MUSTERI_TEL_ALAN = parts[1];
                        request.MUSTERI_TEL_NO = parts[2];
                    }
                }
                #endregion

                #region service call

                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });

                request.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                request.WEB_SIFRE = servisKullanici.Sifre2;

                request.ANA_SEKTOR_KODU = "5200";
                request.ALT_SEKTOR_KODU = "5201";
                request.FAALIYET_OLCEGI = "DGR0010653";
                request.CIRO_ARALIGI = "DGR0021620";
                request.SABIT_VARLIK_ARALIGI = "DGR0022113";

                //Sonradan Eklendi
                request.MESLEK_GRUBU_KODU = "1300";
                request.MESLEK_TIPI_KODU = "1399";
                ClientIPNo = this.IpGetir(_AktifKullaniciService.TVMKodu);
                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = servisUrl;

                teklif = null;
                //HEADER ile LOGLANLANIYOR
                if (teklif != null)
                    teklif.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.MusteriKayit);
                else
                    this.BeginLog(servis.GetServiceMessage(request), WebServisIstekTipleri.MusteriKayit);


                PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response response = servis.CallService<PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET_Response>(request);

                MUSTERI_NO = response.MUSTERI_NO;
                #endregion

                #region Hata Kontrol
                if (response.HATA_KODU != "0")
                {
                    if (teklif != null)
                    {
                        teklif.EndLog(response, false, response.GetType());
                        teklif.AddHata(response.HATA_TEXT);
                    }
                    else
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.HATA_TEXT);
                    }
                }
                else
                    if (teklif != null) teklif.EndLog(response, true, response.GetType());
                else this.EndLog(response, true, response.GetType());


                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                if (teklif != null)
                {
                    teklif.EndLog(ex.Message, false);
                    teklif.AddHata(ex.Message);
                }
                else
                {
                    this.EndLog(ex.Message, false);
                    this.AddHata(ex.Message);
                }



                #endregion
            }
            return MUSTERI_NO;
        }

        public AnadoluModel getAnadoluTeminatlar(string aracKodu, string kullanimSekliKodu, string kullanimTipKodu, int tvmKodu)
        {
            AnadoluModel model = new AnadoluModel();
            try
            {
                AnadoluIkame ikame = new AnadoluIkame();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });
                IANADOLUKasko kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();
                KaskoTeklif kaskoTeklif = new KaskoTeklif();
                ClientIPNo = kaskoTeklif.GetClientIP();

                ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, ClientIPNo);
                servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
                PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR req = new PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR();

                req.ARAC_KODU = aracKodu.TrimStart('0');
                req.KULLANIM_SEKLI_KODU = kullanimSekliKodu;
                req.KULLANIM_TIPI_KODU = kullanimTipKodu;
                req.WEB_KULLANICI_ADI = servisKullanici.KullaniciAdi2;
                req.WEB_SIFRE = servisKullanici.Sifre2;
                req.GRUP_URUN_ADI = ANADOLUKullanimTipiUrunGrupAdi.Kasko;

                var response = servis.CallService<PR_SWEP_URT_OTO_WS_GET_BIRLESIK_KASKO_TEMINATLAR_Response>(req);
                // model.hasarsizlik = response.HASARSIZLIK_INDIRIM_HASARLILIK_EK_PRIM_ORANI;

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

                if (!String.IsNullOrEmpty(response.HATA_MESAJI))
                {
                    model.hataMesaji = response.HATA_MESAJI;
                }
                if (!String.IsNullOrEmpty(response.IKAME_ARAC_BILGILERI))
                {
                    if (response.IKAME_ARAC_BILGILERI.Contains("["))
                    {
                        var jsonArray = js.Deserialize<List<object>>(response.IKAME_ARAC_BILGILERI);
                        object[] values = (object[])jsonArray.FirstOrDefault();
                        if (values != null && values.Length > 1)
                        {
                            var tipList = jsonArray.ConvertToIkameler();
                            foreach (var item in tipList)
                            {
                                ikame = new AnadoluIkame();
                                ikame.key = item.Value;
                                ikame.value = item.Marka;
                                model.ikameList.Add(ikame);
                            }
                        }
                    }
                    else
                    {
                        ikame = new AnadoluIkame();
                        ikame.key = "1";
                        ikame.value = response.IKAME_ARAC_BILGILERI;
                        model.ikameList.Add(ikame);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                if (ex.Message.IndexOf("(500) Internal Server Error.") != -1)
                {
                    model.hataMesaji = "Hata! Anadolu Sigorta Şirketi sistemi ile bağlantı kurulamadı! Lüften, işleminizi 15-30 saniye sonra tekrar deneyiniz.";
                }
                else
                {
                    model.hataMesaji = "Hata! " + ex.Message;
                }
            }

            return model;
        }

        public string IpGetir(int tvmKodu)
        {
            //string cagriIP = "82.222.165.62";
            string sigortaShopIp = " 94.54.67.159";

            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo; // "88.249.209.253"; Birebir ip 
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                {
                    gonderilenIp = sigortaShopIp;
                }
            }
            //"81.214.50.9" MB Grup İp
            //Tuğra Ip "91.93.173.76"
            return gonderilenIp;
            //return "81.214.50.9";

        }

        public void GetAnadoluMenfaatler(int tvmKodu, TVMWebServisKullanicilari servisKullanici)
        {
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleANADOLUKasko);
            var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
            // TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.ANADOLU });
            ANADOLUService servis = new ANADOLUService(servisKullanici.KullaniciAdi, servisKullanici.Sifre, servisKullanici.KullaniciAdi2, servisKullanici.Sifre2, "");
            servis.Url = konfig[Konfig.ANADOLU_ServiceUrl];
            PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI req = new PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI();
            req.KURUM_TIPI = "Banka";
            req.MENFAAT_TURU = "Rehinli Alacaklı";
            req.MENFAAT_TIPI = "Tuzel Kisi";
            req.SESSION_ID = "LOGIN";
            var response = servis.CallService<PR_SWEP_URT_URT_WS_GET_MENFAAT_SAHIBI_BILGILERI_Response>(req);
        }
       
    }
   
    public static class ExtensionIkame
    {
        public static List<AnadoluIkames> ConvertToIkameler(this List<object> list)
        {
            var _list = new List<AnadoluIkames>();
            for (int i = 0; i < list.Count; i++)
            {
                object[] values = (object[])list[i];
                _list.Add(new AnadoluIkames() { Marka = values[1].ToString(), Value = values[0].ToString() });
            }
            return _list;
        }
        public class AnadoluIkames
        {
            public string Marka { get; set; }
            public string Value { get; set; }
        }
    }

    public class AnadoluModel
    {
        public List<AnadoluIkame> ikameList = new List<AnadoluIkame>();
        public string hataMesaji { get; set; }
        public string hasarsizlik { get; set; }
    }

    public class AnadoluIkame
    {
        public string key { get; set; }
        public string value { get; set; }
    }


}

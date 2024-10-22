using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.RAY.Messages.Kasko;
using System.IO;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Business.Common.RAY;
using Neosinerji.BABOnlineTP.Business.RAY.Message;
using System.Net;
using INET.Crypto;
using Neosinerji.BABOnlineTP.Business.RAY.Messages;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    public class RAYKasko : Teklif, IRAYKasko
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IUlkeService _UlkeService;
        IParametreContext _Context;
        IAracService _AracService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;
        [InjectionConstructor]
        public RAYKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IParametreContext context, IAracService aracService, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
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
            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _Context = context;
            _AracService = aracService;
            _TVMService = TVMService;
            _AktifKullaniciService = aktifKullaniciService;
        }


        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.RAY;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                string PDFUrl = String.Empty;
                #region Veri Hazırlama GENEL
                string MUSTERI_NO = String.Empty;

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);

                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 150000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                string RAYSigortaliIlKodu = String.Empty;
                string RAYSigortaliIlceKodu = String.Empty;
                string RAYSigortaliUlkeKodu = String.Empty;
                if (sigortaliAdres != null)
                {
                    CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.RAY &&
                                                                             f.IlKodu == sigortaliAdres.IlKodu &&
                                                                             f.IlceKodu == sigortaliAdres.IlceKodu)
                                                               .SingleOrDefault<CR_IlIlce>();
                    RAYSigortaliUlkeKodu = sigortaliAdres.UlkeKodu;
                    RAYSigortaliIlKodu = sigortaliAdres.IlKodu;
                    RAYSigortaliIlceKodu = sigortaliAdres.IlceKodu.ToString();
                }

                RAY_KaskoTeklifKayit_Request request = new RAY_KaskoTeklifKayit_Request();
                RAY_KaskoTeklifKayit_Response response = new RAY_KaskoTeklifKayit_Response();
                #endregion

                #region Genel Bilgiler

                request.USER_NAME = "oJLhTO69PRvDLrnZiw+LtA==";//INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                request.PASSWORD = "m4YKwOovSAQ="; //INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                request.PRODUCT_NO = RAY_UrunKodlari.Kasko; //RAY Kasko Ürün Kodu
                //request.CLIENT_IP = "78.188.203.193";

                //Ip kontrolünden dolayı Test ortamında wsleri test yapabilmek için 
                //TVM Kodu Neosinerji, Çağrı Sigorta veya Çağrı Brokerlık ise İp sabit olarak Çağrı sigortanın ip adresi gönderiliyor
                //Değilse Client İp gönderiliyor
                request.CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu);

                //request.MUN_CITY_CODE = RAYSigortaliIlKodu;
                //request.MUN_TOWN_CODE = RAYSigortaliIlceKodu;
                //request.MUN_CODE = "10062";
                //request.MUN_COUNTRY_CODE = RAYSigortaliUlkeKodu;
                request.PROCESS_ID = RAY_ProcesTipleri.TeklifKayit;
                request.POLICY_NO = "0";
                request.RENEWAL_NO = "0";
                request.ENDORS_NO = "0";
                request.ENDORS_TYPE_CODE = "";
                request.PROD_CANCEL = "T";
                request.CANCEL_REASON_CODE = "0";
                request.CHANNEL = "4229";// servisKullanici.PartajNo_;
                request.ISSUE_CHANNEL = "4229"; //servisKullanici.PartajNo_;
                // -- RAY Sigorta Müşterisi Sorgulanıyor ve Yoksa RAY Sigorta sistemine müşteri kayıt ediliyor...
                MUSTERI_NO = this.MusteriNo(teklif, sigortali, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.RAY_ServiceURL]);
                if (!String.IsNullOrEmpty(MUSTERI_NO))
                {
                    request.CLIENT_NO = MUSTERI_NO;
                    request.INSURED_NO = MUSTERI_NO;
                }
                else this.AddHata("RAY Sigorta Sisteminde Müşterisi oluşturulamadı");
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                request.BEG_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                request.END_DATE = polBaslangic.AddYears(1).ToString("dd/MM/yyyy").Replace(".", "/");
                request.ISSUE_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                request.CONFIRM_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/");
                request.TRANSFER_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                request.PROPOSAL_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                request.CUR_TYPE = "YTL";
                //request.CUR_ACCOUNT_TYPE = "1";
                request.EXCHANGE_RATE = "1";
                request.PAYMENT_TYPE = "0";
                request.QUERY_METHOD = "T";
                request.DESCRIPTION = "SDTeklif";
                request.FIRM_CODE = "2";
                request.COMPANY_CODE = "2";
                request.CATEGORY1 = "";
                request.CATEGORY2 = "";
                //request.PRINTOUT_TYPE = "0";
                request.PLATE = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;

                #endregion

                #region Teklif Sorular

                List<QUESTION> sorular = new List<QUESTION>();
                QUESTION question = new QUESTION();

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.UrunSecimi, "1");
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.OnarimYapilacakServis, "2"); // 1-Şirket 2- Sigortalı Beliryecek
                sorular.Add(question);

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.Meslek, "");
                //sorular.Add(question);

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.PersonelMusteriGrubu, "");
                //sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle("52570", "1");
                sorular.Add(question);
                if (teklif.Arac.TrafikTescilTarihi.HasValue)
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.TrafikTescilTarihi, teklif.Arac.TrafikTescilTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/"));
                    sorular.Add(question);
                }


                #region Önceki Poliçe
                bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
                string oncekiAcenteNo = String.Empty;
                string oncekiPoliceNo = String.Empty;
                string oncekiSirketKodu = String.Empty;
                string oncekiYenilemeNo = String.Empty;
                string OncekiPoliceMotorNo = String.Empty;
                string OncekiPoliceSasiNo = String.Empty;

                if (eskiPoliceVar)
                {
                    oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                    OncekiPoliceMotorNo = teklif.Arac.MotorNo;
                    OncekiPoliceSasiNo = teklif.Arac.SasiNo;

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiAcenteNo, oncekiAcenteNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceNo, oncekiPoliceNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiSirketKodu, oncekiSirketKodu);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiYenilemeNo, oncekiYenilemeNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "H");
                    sorular.Add(question);
                }
                else
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "E");
                    sorular.Add(question);
                }
                #endregion

                //if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) || !String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriKod, teklif.Arac.TescilSeriKod);
                //    sorular.Add(question);

                //    //Asbis no veya tescil belge no gönderiliyor
                //    question = new QUESTION();

                //    if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                //        question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriNoAsbis, teklif.Arac.AsbisNo);
                //    else question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriNoAsbis, teklif.Arac.TescilSeriNo);

                //    sorular.Add(question);
                //}
                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) || !String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriKod, "");
                    sorular.Add(question);

                    //Asbis no veya tescil belge no gönderiliyor
                    question = new QUESTION();

                    if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                        question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriNoAsbis, "");
                    else question = SoruEkle(RAY_KaskoSorular.TescilBelgeSeriNoAsbis, "");

                    sorular.Add(question);
                }
                question = new QUESTION();
                if (String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                    question = SoruEkle(RAY_TrafikSoruTipleri.ASBISMI, "H");
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.ASBISMI, "E");
                sorular.Add(question);
                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.ModelYili, teklif.Arac.Model.ToString());
                sorular.Add(question);

                question = new QUESTION();


                if (teklif.Arac.Model > 2002)
                {
                    question = SoruEkle(RAY_KaskoSorular.AracMarkaKodu, teklif.Arac.Marka + teklif.Arac.AracinTipi);
                }
                else
                {
                    question = SoruEkle(RAY_KaskoSorular.AracMarkaKodu, "999999");
                }
                sorular.Add(question);
                question = new QUESTION();
                var marka = _AracContext.AracMarkaRepository.Filter(f => f.MarkaKodu == teklif.Arac.Marka).FirstOrDefault();
                if (marka != null)
                    question = SoruEkle(RAY_TrafikSoruTipleri.AracMarka, "");
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.AracMarka, "");
                sorular.Add(question);

                question = new QUESTION();
                var tip = _AracContext.AracTipRepository.Filter(f => f.MarkaKodu == teklif.Arac.Marka && f.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
                if (tip != null)
                    question = SoruEkle(RAY_TrafikSoruTipleri.Tip, "");
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.Tip, "");
                sorular.Add(question);
                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.YerliYabanci, "1"); //?
                sorular.Add(question);
                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceMotorNo,"");
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceSasiNo, "");
                sorular.Add(question);

                string RAYKullanimTarziKodu = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                string kod2 = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.RAY &&
                                                                                                    f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                    f.Kod2 == kod2)
                                                                                                    .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                        RAYKullanimTarziKodu = kullanimTarzi.TarifeKodu;
                }

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.KullanimTarzi, RAYKullanimTarziKodu);
                sorular.Add(question);

                // --Tarife Sınıfı
                string RAYTarifeSinifi = GetTarifeSinifi(teklif.Arac.KullanimTarzi);
                question = new QUESTION();
                if (!String.IsNullOrEmpty(RAYTarifeSinifi))
                    question = SoruEkle(RAY_KaskoSorular.TarifeSinifi, RAYTarifeSinifi);
                sorular.Add(question);

                string aracKullanimSekli = String.Empty; ;
                switch (teklif.Arac.KullanimSekli)
                {
                    case "0": aracKullanimSekli = RAY_AracKullanimSeklilleri.Ticari; break;
                    case "1": aracKullanimSekli = RAY_AracKullanimSeklilleri.Resmi; break;
                    case "2": aracKullanimSekli = RAY_AracKullanimSeklilleri.Ozel; break;
                }

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.KullanimSekli, aracKullanimSekli);
                sorular.Add(question);

                //int modelYili = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;
                //AracModel aracModel = _AracService.GetAracModel(teklif.Arac.Marka, teklif.Arac.AracinTipi, modelYili);

                //if (aracModel != null)
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.AracBedeli, aracModel.Fiyat.ToString());
                //    sorular.Add(question);
                //}

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.MotorNo, teklif.Arac.MotorNo);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.SasiNo, teklif.Arac.SasiNo);
                sorular.Add(question);


                // ====IMM Teminatı   ==== //
                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                string RAYIMMKademe = String.Empty;
                //decimal bedeniSahis = 0;
                if (RAYKullanimTarziKodu == "1" || RAYKullanimTarziKodu == "3" || RAYKullanimTarziKodu == "5") RAYIMMKademe = "5";
                else
                {
                    if (!String.IsNullOrEmpty(immKademe) && !String.IsNullOrEmpty(kullanimTarziKodu))
                    {
                        CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();

                        //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                        var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                        if (IMMBedel != null)
                        {
                            //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                            CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.RAY, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                        }
                        if (CRKademeNo != null)
                        {
                            RAYIMMKademe = CRKademeNo.Kademe.ToString();
                        }

                        ////Ekran girilen teminat bedelinin RAY daki bedeni şahıs karşılığı alınıyor
                        //var iMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
                        //                                                       s.Kademe == immKademe && s.Kod2 == kod2 &&
                        //                                                       s.KullanimTarziKodu == kullanimTarziKodu).FirstOrDefault();

                        //if (iMM != null)
                        //{
                        //    bedeniSahis = iMM.BedeniSahis.Value;
                        //}
                        ////Bedeni şahıs karşılığına göre raydan kademe değeri alınıyor
                        //var IMMKademe = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.RAY &&
                        //                                                      s.Kod2 == kod2 && s.KullanimTarziKodu == kullanimTarziKodu);
                        //if (IMMKademe != null)
                        //{
                        //    foreach (var item in IMMKademe)
                        //    {
                        //        if (item.BedeniSahis < bedeniSahis || item.BedeniSahis == bedeniSahis)
                        //            RAYIMMKademe = item.Kademe;
                        //    }
                        //}
                    }
                }
                if (!String.IsNullOrEmpty(RAYIMMKademe))
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_KaskoSorular.IMMKademeNo, RAYIMMKademe);
                    sorular.Add(question);
                }
                //====Ferdi Kaza Teminatı   ==== //
                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                string RAY_FKKademe = "4";
                string FKMasraf = "250";
                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {

                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);


                    if (FKBedel != null)
                    {
                        CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.RAY, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);
                    }
                    if (CRKademeNo != null)
                    {
                        RAY_FKKademe = CRKademeNo.Kademe.ToString();
                        FKMasraf = CRKademeNo.Tedavi.Value.ToString();
                    }
                }

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.FerdiKazaKoltuk, RAY_FKKademe);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.TedaviMasraflari, FKMasraf); // Fiyat girilecekse FK teminatının %5 i girilmelidir.
                sorular.Add(question);

                //---Dain-i Murtein Var Mı
                //bool dainiMurteinVarMi = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
                //question = new QUESTION();
                ////if (dainiMurteinVarMi) question = SoruEkle(RAY_KaskoSorular.DainiMurteinVarmi, "1");
                ////else 
                //question = SoruEkle(RAY_KaskoSorular.DainiMurteinVarmi, "1");
                //sorular.Add(question);

                //if (dainiMurteinVarMi)
                //{
                //    string dainiMurteinVergiNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);

                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.DainiMurteinTCKimlikNo, "");
                //    sorular.Add(question);

                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.DainiMurteinVergiNo, dainiMurteinVergiNo);
                //    sorular.Add(question);
                //}

                //Alarm
                //bool alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
                //question = new QUESTION();
                //if (alarm) question = SoruEkle(RAY_KaskoSorular.SesliAlarm, "E");
                //else question = SoruEkle(RAY_KaskoSorular.SesliAlarm, "H");
                //sorular.Add(question);

                //---LPG Mi
                bool LPG = teklif.ReadSoru(KaskoSorular.LPG_VarYok, false);
                question = new QUESTION();
                if (RAYKullanimTarziKodu == "1" || RAYKullanimTarziKodu == "5")
                {
                    if (LPG) question = SoruEkle(RAY_KaskoSorular.LPGMi, "E");
                    else question = SoruEkle(RAY_KaskoSorular.LPGMi, "H");
                }
                else question = SoruEkle(RAY_KaskoSorular.LPGMi, "");
                sorular.Add(question);

                //---Deprem Var Mı
                bool Deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
                question = new QUESTION();
                if (Deprem) question = SoruEkle(RAY_KaskoSorular.DepremMuafiyeti, "E");
                else question = SoruEkle(RAY_KaskoSorular.DepremMuafiyeti, "H");
                sorular.Add(question);

                //---Sel ve Su Baskını
                bool SelSu = teklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false);
                question = new QUESTION();
                if (SelSu) question = SoruEkle(RAY_KaskoSorular.SelMuafiyeti, "E");
                else question = SoruEkle(RAY_KaskoSorular.SelMuafiyeti, "H");
                sorular.Add(question);

                //Yurt Dışı Teminatı
                bool YurtDisi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);
                string RAYYurtDisiSuresi = String.Empty;
                if (YurtDisi)
                {
                    string YurtDisiSuresi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "1");
                    if (!String.IsNullOrEmpty(YurtDisiSuresi))
                    {
                        switch (YurtDisiSuresi)
                        {                               //Ray Sigorta Kodları set ediliyor
                            case "1": RAYYurtDisiSuresi = "1"; break;
                            case "2": RAYYurtDisiSuresi = "1"; break;
                            case "3": RAYYurtDisiSuresi = "2"; break;
                            case "4": RAYYurtDisiSuresi = "3"; break;
                            case "5": RAYYurtDisiSuresi = "4"; break;
                        }
                    }
                    question = new QUESTION();
                    question = SoruEkle(RAY_KaskoSorular.YurtDisindaKalmaSuresi, RAYYurtDisiSuresi);
                    sorular.Add(question);
                }


                //E/H değeri hang durumlarda gönderilecek
                if (RAYKullanimTarziKodu == "5" || RAYKullanimTarziKodu == "6" || RAYKullanimTarziKodu == "9")
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_KaskoSorular.DamperVarmi, "E");
                    sorular.Add(question);
                }
                else
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_KaskoSorular.DamperVarmi, "H");
                    sorular.Add(question);
                }
                //if ((RAYKullanimTarziKodu == "4" && RAYTarifeSinifi == "5") || (RAYKullanimTarziKodu == "4" && RAYTarifeSinifi == "6"))
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.SehirIciIndirimi, "E");
                //    sorular.Add(question);
                //}
                //else
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.SehirIciIndirimi, "");
                //    sorular.Add(question);
                //}
                //if (teklif.Arac.KoltukSayisi.HasValue && teklif.Arac.KoltukSayisi.Value > 0)
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.YolcuAdedi, (teklif.Arac.KoltukSayisi.Value - 1).ToString());
                //    sorular.Add(question);

                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.SurucuAdedi, "1");
                //    sorular.Add(question);
                //}

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.TekSurucuIndirim, "");
                //sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.KaskoMuafiyeti, "H");
                sorular.Add(question);
             
                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.EnAz5YillikEhliyetIndirimi, "H");
                //sorular.Add(question);

                //if (RAYKullanimTarziKodu != "5" || RAYKullanimTarziKodu == "6")
                //{
                //    question = new QUESTION();
                //    question = SoruEkle(RAY_KaskoSorular.TasinanEmteaKodu, "");
                //    sorular.Add(question);
                //}

                /////E/H değeri hang durumlarda gönderilecek

                //Bu sorular bizim ekranda yok o yüzden yorum satırı ?

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.EkPrimOrani, "");
                //sorular.Add(question);         

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.YurtDisinaCikisTarihi, "");
                //sorular.Add(question);

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.YurtDisiTeminatiBitisTarihi, "");
                //sorular.Add(question);

                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.SurucuKursuEkPrimi, "");
                //sorular.Add(question);


                //question = new QUESTION();
                //question = SoruEkle(RAY_KaskoSorular.GorevliAdedi, "");
                //sorular.Add(question);

                request.QUESTION = sorular.ToArray();

                #endregion

                #region Service Call

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_KaskoTeklifKayit_Request));
                StringWriter Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, request);
                string responseKasko = clnt.ProductionIntegrator(Output.ToString());
                Output.Close();
                clnt.Dispose();
                _Serialize = new XmlSerializer(typeof(RAY_KaskoTeklifKayit_Response));

                using (TextReader reader = new StringReader(responseKasko))
                {
                    var kaskoHata = responseKasko.Contains("Error");
                    if (!kaskoHata)
                    {
                        response = (RAY_KaskoTeklifKayit_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        this.EndLog(response, true, response.GetType());

                        RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                        RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                        pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                        pdfRequest.FIRM_CODE = "2";
                        pdfRequest.COMPANY_CODE = "2";
                        pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Kasko;
                        pdfRequest.POLICY_NO = response.POLMAS.POLICY_NO;
                        pdfRequest.RENEWAL_NO = "0";
                        pdfRequest.ENDORS_NO = "0";
                        pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.PolicePDF;

                        this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.Police);

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                        Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(Output, pdfRequest);
                        string responseTeklifPDF = clnt.ProductionIntegrator(Output.ToString());
                        clnt.Dispose();
                        Output.Close();

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                        using (TextReader readerPDF = new StringReader(responseTeklifPDF))
                        {

                            pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                            readerPDF.ReadToEnd();
                            this.EndLog(pdfResponse, true, pdfResponse.GetType());

                            if (pdfResponse.STATUS_CODE != "0")
                            {
                                WebClient myClient = new WebClient();
                                byte[] data = myClient.DownloadData(pdfResponse.LINK);

                                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                                string fileName = String.Format("RAY_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                                PDFUrl = storage.UploadFile("kasko", fileName, data);

                                _Log.Info("Teklif_PDF url: {0}", PDFUrl);
                            }
                            else
                            {
                                this.EndLog(pdfResponse, false, pdfResponse.GetType());
                                this.AddHata(pdfResponse.STATUS_DESC);
                            }
                        }
                    }
                    else
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_TrafikTeklifKayitHata_Response));
                        using (StringReader readerTrafikHata = new StringReader(responseKasko))
                        {
                            RAY_TrafikTeklifKayitHata_Response trafikResponseHata = new RAY_TrafikTeklifKayitHata_Response();
                            trafikResponseHata = (RAY_TrafikTeklifKayitHata_Response)_Serialize.Deserialize(readerTrafikHata);
                            readerTrafikHata.ReadToEnd();

                            this.EndLog(trafikResponseHata, false, trafikResponseHata.GetType());
                            this.AddHata(trafikResponseHata.Error.ErrDesc);
                        }
                    }
                }

                #endregion

                #region Basarı Kontrol
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
                this.GenelBilgiler.TUMTeklifNo = response.POLMAS.POLICY_NO;
                this.GenelBilgiler.PDFDosyasi = PDFUrl;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;

                this.GenelBilgiler.BrutPrim = RAYMessages.ToDecimal(response.POLMAS.GROSS_PREMIUM);

                //---RAY Vergiler ve Komisyonlar
                var PoldidList = response.POLMAS.POLDID.ToList();
                decimal RAY_GiderVergi = 0;
                decimal RAY_KaynakKomisyonu = 0;

                for (int i = 0; i < PoldidList.Count; i++)
                {
                    var DeductionCode = response.POLMAS.POLDID[i].DEDUCTION_CODE;
                    var DeductionTypeCode = response.POLMAS.POLDID[i].DEDUCTION_TYPE_CODE;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Vergiler && DeductionCode == RAY_TrafikVergiKesintiKodlari.GiderVergisi)
                        RAY_GiderVergi = !String.IsNullOrEmpty(response.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(response.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Komisyonlar && DeductionCode == RAY_TrafikKomisyonKesintiKodlari.KaynakKomisyonu)
                        RAY_KaynakKomisyonu = !String.IsNullOrEmpty(response.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(response.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;

                }

                this.GenelBilgiler.ToplamVergi = RAY_GiderVergi;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = RAY_KaynakKomisyonu;

                // ---RAY Hasarsızlık İndirim ve Arttırım Oranları 
                decimal RAY_HasarsizlikIndirimOrani = 0;
                decimal RAY_HasarsizlikArttirimOrani = 0;

                var hasarsizlikIndirimOrani = response.POLMAS.PSRCVP.FirstOrDefault(f => f.QUESTION_CODE == RAY_KaskoResponseSoruKodlari.HasarsizlikIndirimOrani);
                if (hasarsizlikIndirimOrani != null)
                    RAY_HasarsizlikIndirimOrani = !String.IsNullOrEmpty(hasarsizlikIndirimOrani.ANSWER) ? RAYMessages.ToDecimal(hasarsizlikIndirimOrani.ANSWER) : 0;

                var hasarsizlikArttirimOrani = response.POLMAS.PSRCVP.FirstOrDefault(f => f.QUESTION_CODE == RAY_KaskoResponseSoruKodlari.HasarsizlikArttirimOrani);
                if (hasarsizlikArttirimOrani != null)
                    RAY_HasarsizlikArttirimOrani = !String.IsNullOrEmpty(hasarsizlikArttirimOrani.ANSWER) ? RAYMessages.ToDecimal(hasarsizlikArttirimOrani.ANSWER) : 0;

                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = RAY_HasarsizlikIndirimOrani;
                this.GenelBilgiler.HasarSurprimYuzdesi = RAY_HasarsizlikArttirimOrani;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Vergiler
                this.AddVergi(KaskoVergiler.GiderVergisi, RAY_GiderVergi);
                #endregion

                #region Teminatlar

                //---KASKO TEMİNATI
                var Kasko = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.Kasko);
                decimal TeminatBedeli = 0;
                decimal BrutPrim = 0;
                if (Kasko != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(Kasko.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.Kasko &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Kasko, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(Kasko.NET_PREMIUM), BrutPrim, 0);
                }
                //---DEPREM YANARDAĞ PÜSKÜRME
                var DepremYanardag = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.DepremYanardagPuskurme);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (DepremYanardag != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(DepremYanardag.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.DepremYanardagPuskurme &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Deprem, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(DepremYanardag.NET_PREMIUM), BrutPrim, 0);
                }
                //---SEL VE SU BASMASI
                var SelSuBasmasi = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SelveSuBasmasi);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (SelSuBasmasi != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(SelSuBasmasi.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SelveSuBasmasi &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Seylap, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(SelSuBasmasi.NET_PREMIUM), BrutPrim, 0);
                }
                //---GLKHH ve TERÖR
                var GLKHHT = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.GLKHHT);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (GLKHHT != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(GLKHHT.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.GLKHHT &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.GLKHHT, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(GLKHHT.NET_PREMIUM), BrutPrim, 0);
                }
                //---SİGARA VB. MADDE ZARARLARI
                var SigaraMadde = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SigaraMaddeZararlari);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (SigaraMadde != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(SigaraMadde.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SigaraMaddeZararlari &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Sigara_Ve_Benzeri_Madde_Zararlari, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(SigaraMadde.NET_PREMIUM), BrutPrim, 0);
                }
                //---ÇEKME ÇEKİLME ZARARLARI
                var CekmeCekilme = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.CekmeCekilmeZararlari);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (CekmeCekilme != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(CekmeCekilme.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.CekmeCekilmeZararlari &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.YetkisizCekilme, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(CekmeCekilme.NET_PREMIUM), BrutPrim, 0);
                }
                //---ANAHTAR KAYBI
                //var AnahtarKaybi = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.AnahtarKaybi);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (AnahtarKaybi != null)
                //{
                //    TeminatBedeli = RAYMessages.ToDecimal(AnahtarKaybi.COVER_AMOUNT);

                //    #region Teminat Vergiler
                //    decimal ToplamVergi = 0;
                //    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.AnahtarKaybi &&
                //                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                //                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                //    if (GiderVergi != null)
                //        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                //    #endregion

                //    BrutPrim = TeminatBedeli + ToplamVergi;
                //    this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(AnahtarKaybi.NET_PREMIUM), BrutPrim, 0);
                //}
                //---KEMİRGEN HAYVAN ZARARLARI
                //var KemirgenHayvanZarar = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.KemirgenHayvanZararlari);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (KemirgenHayvanZarar != null)
                //{
                //    TeminatBedeli = RAYMessages.ToDecimal(KemirgenHayvanZarar.COVER_AMOUNT);

                //    #region Teminat Vergiler
                //    decimal ToplamVergi = 0;
                //    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.KemirgenHayvanZararlari &&
                //                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                //                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                //    if (GiderVergi != null)
                //        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                //    #endregion

                //    BrutPrim = TeminatBedeli + ToplamVergi;
                //    this.AddTeminat(KaskoTeminatlar.Hayvanlarin_Verecegi_Zarar, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(KemirgenHayvanZarar.NET_PREMIUM), BrutPrim, 0);
                //}
                //---İHTİYARİ MALİ MESULİYET
                var IMM = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.IMM);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (IMM != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(IMM.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.IMM &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(IMM.NET_PREMIUM), BrutPrim, 0);
                }
                //---ÖLÜM SAKATLIK KİŞİ BAŞINA
                var OlumSakatlikKisiBasina = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.OlumSakatlikKisiBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKisiBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(OlumSakatlikKisiBasina.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.OlumSakatlikKisiBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(OlumSakatlikKisiBasina.NET_PREMIUM), BrutPrim, 0);
                }
                //---ÖLÜM SAKATLIK KAZA BAŞINA
                var OlumSakatlikKazaBasina = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.OlumSakatlikKazaBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKazaBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(OlumSakatlikKazaBasina.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.OlumSakatlikKazaBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(OlumSakatlikKazaBasina.NET_PREMIUM), BrutPrim, 0);
                }
                //---MADDİ KAZA BAŞINA
                var MaddiKazaBasina = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.MaddiKazaBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (MaddiKazaBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(MaddiKazaBasina.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.MaddiKazaBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(MaddiKazaBasina.NET_PREMIUM), BrutPrim, 0);
                }
                //---FERDİ KAZA KOLTUK
                var FerdiKazaKoltuk = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.FerdiKazaKoltuk);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (FerdiKazaKoltuk != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(FerdiKazaKoltuk.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.FerdiKazaKoltuk &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(FerdiKazaKoltuk.NET_PREMIUM), BrutPrim, 0);
                }
                //---VEFAT
                var Vefat = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.Vefat);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (Vefat != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(Vefat.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.Vefat &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.KFK_Olum, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(Vefat.NET_PREMIUM), BrutPrim, 0);
                }
                //---SÜREKLİ SAKATLIK
                var SurekliSakatlik = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SurekliSakatlik);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (SurekliSakatlik != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(SurekliSakatlik.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.SurekliSakatlik &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(SurekliSakatlik.NET_PREMIUM), BrutPrim, 0);
                }
                //---HUKUKSAL KORUMA
                var HukuksalKoruma = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKoruma);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (HukuksalKoruma != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(HukuksalKoruma.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKoruma &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(HukuksalKoruma.NET_PREMIUM), BrutPrim, 0);
                }
                //---HUKUKSAL KORUMA MOTORLU ARACA BAĞLI
                var HK_MotorluAracaBagli = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKorumaMotorluAracaBagli);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (HK_MotorluAracaBagli != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(HK_MotorluAracaBagli.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKorumaMotorluAracaBagli &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(HK_MotorluAracaBagli.NET_PREMIUM), BrutPrim, 0);
                }
                //---HUKUKSAL KORUMA SÜRÜCÜYE BAĞLI
                var HK_SurucuyeBagli = response.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKorumaSurucuyeBagli);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (HK_SurucuyeBagli != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(HK_SurucuyeBagli.COVER_AMOUNT);

                    #region Teminat Vergiler
                    decimal ToplamVergi = 0;
                    var GiderVergi = response.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_KaskoTeminatlar.HukuksalKorumaSurucuyeBagli &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);

                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(HK_SurucuyeBagli.NET_PREMIUM), BrutPrim, 0);
                }

                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion

                #region Web servis cevapları
                this.AddWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, response.POLMAS.POLICY_NO);
                this.AddWebServisCevap(Common.WebServisCevaplar.RAY_Musteri_No, MUSTERI_NO);
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                clnt.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }

        }

        public override void Policelestir(Odeme odeme)
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                #endregion

                #region Policelestirme Request

                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 300000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });

                RAY_KaskoPolicelestirme_Request request = new RAY_KaskoPolicelestirme_Request();
                RAY_KaskoPolicelestirme_Response response = new RAY_KaskoPolicelestirme_Response();

                string SigortaliNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Musteri_No, "0");

                string TUMPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, "0");
                request.USER_NAME = "oJLhTO69PRvDLrnZiw+LtA==";//INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                request.PASSWORD = "m4YKwOovSAQ="; //INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                //request.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                //request.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                request.CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu);
                request.PRODUCT_NO = RAY_UrunKodlari.Kasko;
                request.PROCESS_ID = RAY_ProcesTipleri.Policelestirme;
                request.FIRM_CODE = "2";
                request.COMPANY_CODE = "2";
                request.POLICY_NO = TUMPoliceNo;
                request.RENEWAL_NO = "0";
                request.ENDORS_NO = "0";
                // request.ENDORS_TYPE_CODE = "0";
                // request.PROD_CANCEL = "T";
                // request.CANCEL_REASON_CODE = "0";
                request.CHANNEL = "4229";// servisKullanici.PartajNo_;
                request.ISSUE_CHANNEL = "4229";// servisKullanici.PartajNo_;
                //request.BEG_DATE = teklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy").Replace(".", "/");
                //request.END_DATE = teklif.GenelBilgiler.BaslamaTarihi.AddYears(1).ToString("dd/MM/yyyy").Replace(".", "/");
                request.CONFIRM_DATE = TurkeyDateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"); //teklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy").Replace(".", "/");
                //request.CUR_TYPE = "TL";
                //request.EXCHANGE_RATE = "1";
                request.PAYMENT_TYPE = "0";
                request.QUERY_METHOD = "O";
                //request.DESCRIPTION = "";


                //request.CLIENT_NO = SigortaliNo;
                //request.INSURED_NO = SigortaliNo;
                //request.PRINTOUT_TYPE = "0";
                //request.PLATE = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                request.HAS_INST_CARD = "1";


                //if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                //{
                //    request.PAYMENTS.CREDIT_CARD_NAME = odeme.KrediKarti.KartSahibi;
                //    request.PAYMENTS.CREDIT_CARD_NO = odeme.KrediKarti.KartNo.Substring(0, 6);
                //    request.PAYMENTS.CREDIT_CARD_CVV = "XXX";
                //    request.PAYMENTS.CREDIT_CARD_VALID_MONTH = "99";
                //    request.PAYMENTS.CREDIT_CARD_VALID_YEAR = "9999";
                //    request.PAYMENTS.INSTALLMENT_NUMBER = odeme.TaksitSayisi.ToString();
                //}

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

                ////Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                //if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                //{
                //    string[] adparts = odeme.KrediKarti.KartSahibi.Split(' ');

                //    if (adparts.Length == 1)
                //    {
                //        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartSahibi);
                //    }
                //    else if (adparts.Length == 2)
                //    {
                //        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(adparts[0]);
                //        request.PAYMENTS.CREDIT_CARD_SURNAME = INETTripleDesCrypto.EncryptMessage(adparts[1]);
                //    }
                //    else if (adparts.Length == 3)
                //    {
                //        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(adparts[0] + " " + adparts[1]);
                //        request.PAYMENTS.CREDIT_CARD_SURNAME = INETTripleDesCrypto.EncryptMessage(adparts[2]);
                //    }

                //    request.PAYMENTS.CREDIT_CARD_NO = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartNo);
                //    request.PAYMENTS.CREDIT_CARD_VALID_MONTH = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKA);
                //    request.PAYMENTS.CREDIT_CARD_VALID_YEAR = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKY);
                //    request.PAYMENTS.CREDIT_CARD_CVV = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.CVC);
                //    request.PAYMENTS.INSTALLMENT_NUMBER = odeme.TaksitSayisi.ToString();
                //    request.PAYMENTS.CC_PREFIX = odeme.KrediKarti.KartNo.Substring(0, 6);
                //    if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                //        request.PAYMENTS.CREDIT_CARD_TYPE = "2"; //Master Card
                //    else request.PAYMENTS.CREDIT_CARD_TYPE = "1"; //Visa
                //}
                //else
                //{
                //    request.PAYMENTS.CREDIT_CARD_NAME = "";
                //    request.PAYMENTS.CREDIT_CARD_NO = "";
                //    request.PAYMENTS.CREDIT_CARD_CVV = "";
                //    request.PAYMENTS.CREDIT_CARD_VALID_MONTH = "";
                //    request.PAYMENTS.CREDIT_CARD_VALID_YEAR = "";
                //    request.PAYMENTS.INSTALLMENT_NUMBER = "";
                //    request.PAYMENTS.CC_PREFIX = "";
                //    request.PAYMENTS.CREDIT_CARD_TYPE = "0";
                //}
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_KaskoPolicelestirme_Request));
                StringWriter Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, request);
                string responsePolice = clnt.ProductionIntegrator(Output.ToString());
                Output.Close();
                clnt.Dispose();
                _Serialize = new XmlSerializer(typeof(RAY_KaskoPolicelestirme_Response));

                using (TextReader reader = new StringReader(responsePolice))
                {
                    var policeHata = responsePolice.Contains("Error");
                    if (!policeHata)
                    {
                        response = (RAY_KaskoPolicelestirme_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        this.EndLog(response, true, response.GetType());

                        this.GenelBilgiler.TUMPoliceNo = response.POLMAS.POLICY_NO;
                        this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                        RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                        RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                        pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                        pdfRequest.FIRM_CODE = "2";
                        pdfRequest.COMPANY_CODE = "2";
                        pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Kasko;
                        pdfRequest.POLICY_NO = response.POLMAS.POLICY_NO;
                        pdfRequest.RENEWAL_NO = "0";
                        pdfRequest.ENDORS_NO = "0";
                        pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.PolicePDF; //Police PDF

                        this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.Police);

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                        Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(Output, pdfRequest);
                        string responsePolicePDF = clnt.ProductionIntegrator(Output.ToString());
                        Output.Close();
                        clnt.Dispose();
                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                        using (TextReader readerPDF = new StringReader(responsePolicePDF))
                        {

                            pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                            readerPDF.ReadToEnd();
                            this.EndLog(pdfResponse, true, pdfResponse.GetType());

                            if (pdfResponse.STATUS_CODE != "0")
                            {
                                WebClient myClient = new WebClient();
                                byte[] data = myClient.DownloadData(pdfResponse.LINK);

                                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                                string fileName = String.Format("RAY_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                                string url = storage.UploadFile("kasko", fileName, data);
                                this.GenelBilgiler.PDFPolice = url;

                                _Log.Info("Police_PDF url: {0}", url);
                            }
                            else
                            {
                                this.EndLog(pdfResponse, false, pdfResponse.GetType());
                                this.AddHata(pdfResponse.STATUS_DESC);
                            }
                        }
                    }
                    else if (responsePolice.Contains("INET_ERROR"))
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_KaskoPolicelestirmeHata_Response));
                        using (StringReader readerPoliceHata2 = new StringReader(responsePolice))
                        {
                            RAY_KaskoPolicelestirmeHata_Response policeResponseHata = new RAY_KaskoPolicelestirmeHata_Response();
                            policeResponseHata = (RAY_KaskoPolicelestirmeHata_Response)_Serialize.Deserialize(readerPoliceHata2);
                            readerPoliceHata2.ReadToEnd();

                            this.EndLog(policeResponseHata, false, policeResponseHata.GetType());
                            this.AddHata(policeResponseHata.ERROR_MESSAGE);
                        }
                    }
                    else
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_KaskoPolicelestirmeHata2_Response));
                        using (StringReader readerPoliceHata = new StringReader(responsePolice))
                        {
                            RAY_KaskoPolicelestirmeHata2_Response policeResponseHata = new RAY_KaskoPolicelestirmeHata2_Response();
                            policeResponseHata = (RAY_KaskoPolicelestirmeHata2_Response)_Serialize.Deserialize(readerPoliceHata);
                            readerPoliceHata.ReadToEnd();

                            this.EndLog(policeResponseHata, false, policeResponseHata.GetType());
                            this.AddHata(policeResponseHata.Error.ErrDesc);
                        }
                    }
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        public override void DekontPDF()
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);
                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 150000;

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });

                RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                string RAYPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, "0");

                pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                pdfRequest.FIRM_CODE = "2";
                pdfRequest.COMPANY_CODE = "2";
                pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Kasko;
                pdfRequest.POLICY_NO = RAYPoliceNo;
                pdfRequest.RENEWAL_NO = "0";
                pdfRequest.ENDORS_NO = "0";
                pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.DekontPDF; //Dekont PDF

                this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.DekontPDF);
                StringWriter Output = new StringWriter(new StringBuilder());
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, pdfRequest);
                string responsePolicePDF = clnt.ProductionIntegrator(Output.ToString());
                Output.Close();
                clnt.Dispose();
                _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                using (TextReader readerPDF = new StringReader(responsePolicePDF))
                {
                    pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                    readerPDF.ReadToEnd();
                    this.EndLog(pdfResponse, true, pdfResponse.GetType());

                    if (pdfResponse.STATUS_CODE != "0")
                    {
                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(pdfResponse.LINK);

                        IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("RAY_Kasko_Police_Dokont_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = storage.UploadFile("kasko", fileName, data);
                        this.GenelBilgiler.PDFDekont = url;

                        _Log.Info("Dekont_PDF url: {0}", url);
                        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                    }
                    else
                    {
                        this.EndLog(pdfResponse, false, pdfResponse.GetType());
                        this.AddHata("PDF dosyası alınamadı.");
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        private QUESTION SoruEkle(string soruKodu, string cevap)
        {
            QUESTION question = new QUESTION();
            question.QUESTION_CODE = soruKodu;
            if (cevap == "0,00")
            {
                question.ANSWER = "0";
            }
            else
            {
                question.ANSWER = cevap;
            }

            return question;
        }

        private string GetTarifeSinifi(string KullanimTarzi)
        {
            string tarifeSinifi = String.Empty;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            //   babonline k.tarzi, ray kullanım tarzı-tarife sınıfı
            dictionary.Add("111-10", "1-1");
            dictionary.Add("121-10", "1-3");
            dictionary.Add("111-12", "2-1");
            dictionary.Add("111-14", "2-2");
            dictionary.Add("311-10", "3-1");
            dictionary.Add("311-11", "3-2");
            dictionary.Add("311-12", "3-3");
            dictionary.Add("311-14", "3-4");
            dictionary.Add("411-10", "4-5");
            dictionary.Add("421-10", "4-6");
            dictionary.Add("411-13", "4-7");
            dictionary.Add("421-12", "4-8");
            dictionary.Add("511-10", "5-1");
            dictionary.Add("521-10", "6-1");
            dictionary.Add("523-10", "6-2");
            dictionary.Add("526-10", "6-4");
            dictionary.Add("521-14", "6-6");
            dictionary.Add("521-13", "6-7");
            dictionary.Add("521-17", "6-8");
            dictionary.Add("521-19", "6-9");
            dictionary.Add("611-10", "7-1");
            dictionary.Add("711-10", "8-1");
            dictionary.Add("532-10", "9-1");
            dictionary.Add("911-10", "10-1");
            dictionary.Add("111-11", "11-2");

            if (dictionary.ContainsKey(KullanimTarzi))
            {
                string val = dictionary[KullanimTarzi];
                string[] vals = val.Split('-');
                tarifeSinifi = vals[1];
            }
            return tarifeSinifi;
        }

        public string MusteriNo(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl)
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            #region Müşteri No
            string MusteriNo = String.Empty;
            try
            {
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.RAY });
                clnt.Url = servisUrl;
                clnt.Timeout = 150000;

                #region RAY Müşteri Database Sorgu

                RAY_Musteri_DatabaseSorgu_Request musDatabaseSorguReq = new RAY_Musteri_DatabaseSorgu_Request();
                RAY_Musteri_DatabaseSorgu_Response musDatabaseSorguRes = new RAY_Musteri_DatabaseSorgu_Response();

                musDatabaseSorguReq.PROCESS_ID = RAY_ProcesTipleri.MusteriDataBaseSorgula;
                musDatabaseSorguReq.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                musDatabaseSorguReq.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                musDatabaseSorguReq.CHANNEL = servisKullanici.PartajNo_;

                if (sigortali.KimlikNo.Length == 11)
                {
                    musDatabaseSorguReq.CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                    musDatabaseSorguReq.TAX_NUMBER = "";
                }
                else if (sigortali.KimlikNo.Length == 10)
                {
                    musDatabaseSorguReq.CITIZENSHIP_NUMBER = "";
                    musDatabaseSorguReq.TAX_NUMBER = sigortali.KimlikNo;
                }
                musDatabaseSorguReq.FOREIGN_CITIZENSHIP_NUMBER = "";
                this.BeginLog(musDatabaseSorguReq, musDatabaseSorguReq.GetType(), WebServisIstekTipleri.MusteriKayit);
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_Musteri_DatabaseSorgu_Request));
                StringWriter _Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(_Output, musDatabaseSorguReq);
                string responseMusteriDatabaseSorgu = clnt.ProductionIntegrator(_Output.ToString());
                _Output.Close();
                clnt.Dispose();
                #endregion

                #region RAY Sigortaya Müşteri Kaydet

                _Serialize = new XmlSerializer(typeof(RAY_Musteri_DatabaseSorgu_Response));
                using (StringReader reader = new StringReader(responseMusteriDatabaseSorgu))
                {
                    var hata = responseMusteriDatabaseSorgu.Contains("INFO");//Müşteri db de kayıtlı değil
                    var hata2 = responseMusteriDatabaseSorgu.Contains("ERROR"); // diğer hatalar

                    if (hata2)
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_KaydetmeHata_Response));
                        using (StringReader readerMusteriHata = new StringReader(responseMusteriDatabaseSorgu))
                        {
                            RAY_Musteri_KaydetmeHata_Response musterihatares = new RAY_Musteri_KaydetmeHata_Response();

                            musterihatares = (RAY_Musteri_KaydetmeHata_Response)_Serialize.Deserialize(readerMusteriHata);
                            readerMusteriHata.ReadToEnd();
                            this.EndLog(musterihatares, false, musterihatares.GetType());
                            this.AddHata(musterihatares.ERROR_DESC);
                        }
                    }

                    else if (!hata)
                    {
                        musDatabaseSorguRes = (RAY_Musteri_DatabaseSorgu_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        MusteriNo = musDatabaseSorguRes.UNSMAS.UNIT_NO;
                        this.EndLog(musDatabaseSorguRes, true, musDatabaseSorguRes.GetType());
                    }
                    else
                    {

                        this.EndLog(musDatabaseSorguRes, true, musDatabaseSorguRes.GetType());

                        RAY_Musteri_Kaydetme_Request musKayReq = new RAY_Musteri_Kaydetme_Request();
                        RAY_Musteri_Kaydetme_Response musKaydRes = new RAY_Musteri_Kaydetme_Response();

                        MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();

                        #region Genel Bilgiler
                        musKayReq.UNSMAS = new UNSMAS();
                        musKayReq.UNSMAS.PROCESS_ID = RAY_ProcesTipleri.MusteriKaydetme;

                        musKayReq.UNSMAS.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        musKayReq.UNSMAS.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        musKayReq.UNSMAS.CHANNEL = servisKullanici.PartajNo_;
                        // musKayReq.UNSMAS.CLIENT_IP = "78.188.203.193";

                        musKayReq.UNSMAS.CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu);
                        if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                        {
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                            musKayReq.UNSMAS.GENDER = sigortali.Cinsiyet;
                            musKayReq.UNSMAS.BIRTH_DATE = sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("dd/MM/yyyy") : null;
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.TAX_NUMBER = ""; //Vergi No
                        }
                        else if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri || sigortali.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                        {
                            musKayReq.UNSMAS.TAX_NUMBER = sigortali.KimlikNo; //Vergi No
                            musKayReq.UNSMAS.GENDER = "";
                            musKayReq.UNSMAS.BIRTH_DATE = "";
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = "";

                        }
                        else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                        {
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                            musKayReq.UNSMAS.BIRTH_DATE = sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("dd/mm/yyyy") : null;
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.TAX_NUMBER = ""; //Vergi No
                            musKayReq.UNSMAS.GENDER = sigortali.Cinsiyet;

                        }

                        musKayReq.UNSMAS.TAX_OFFICE = "";
                        musKayReq.UNSMAS.IDENTITY_NO = "";
                        musKayReq.UNSMAS.COUNTRY_CODE = "90";
                        //musKayReq.UNSMAS.NATIONALITY = "90";
                        musKayReq.UNSMAS.PERSONAL_COMMERCIAL = "0";
                        musKayReq.UNSMAS.FIRM_NAME = "";
                        musKayReq.UNSMAS.NAME = sigortali.AdiUnvan;
                        musKayReq.UNSMAS.SURNAME = sigortali.SoyadiUnvan;
                        musKayReq.UNSMAS.BIRTH_PLACE = "";
                        musKayReq.UNSMAS.EMAIL = sigortali.EMail;
                        musKayReq.UNSMAS.URL_ADDRESS1 = "";
                        musKayReq.UNSMAS.WORK_AREA = "3";
                        musKayReq.UNSMAS.SECTOR = "5";
                        musKayReq.UNSMAS.FATHER_NAME = ""; //?
                        musKayReq.UNSMAS.MOTHER_NAME = ""; //?
                        musKayReq.UNSMAS.CONNECT_ADDRESS = "1";
                        musKayReq.UNSMAS.MARITAL_STATUS = "X";
                        musKayReq.UNSMAS.OCCUPATION = "3";
                        musKayReq.UNSMAS.RESIDENT_IN_STATE = "E";

                        musKayReq.UNSMAS.EMPLOYEE_COUNT = "10";
                        musKayReq.UNSMAS.BLACK_LIST_CODE = "0";
                        musKayReq.UNSMAS.BLACK_LIST_ENTRY_REASON = "";
                        musKayReq.UNSMAS.BLACK_LIST_ENRTY_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                        musKayReq.UNSMAS.SCORE = "5";

                        #endregion

                        #region Telefon

                        if (telefon != null)
                        {
                            musKayReq.UNSMAS.GSM_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.GSM_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.GSM_NUMBER1 = telefon.Numara.Substring(7, 7);

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.PHONE_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.PHONE_NUMBER1 = telefon.Numara.Substring(7, 7);
                            musKayReq.UNSMAS.PHONE_LINE1 = "56";

                            musKayReq.UNSMAS.FAX_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.FAX_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.FAX_NUMBER1 = telefon.Numara.Substring(7, 7);
                            musKayReq.UNSMAS.FAX_LINE1 = "12";
                        }
                        else
                        {
                            musKayReq.UNSMAS.GSM_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.GSM_CODE1 = "999";
                            musKayReq.UNSMAS.GSM_NUMBER1 = "9999999";

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.PHONE_CODE1 = "999";
                            musKayReq.UNSMAS.PHONE_NUMBER1 = "9999999";
                            musKayReq.UNSMAS.PHONE_LINE1 = "56";

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.PHONE_CODE1 = "999";
                            musKayReq.UNSMAS.PHONE_NUMBER1 = "9999999";
                            musKayReq.UNSMAS.PHONE_LINE1 = "12";
                        }
                        #endregion

                        #region Adres
                        List<UNSADRR> adress = new List<UNSADRR>();
                        musKayReq.UNSADR = new UNSADRR[2];


                        UNSADRR unsadr = new UNSADRR();

                        MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                        if (adres != null)
                        {
                            CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.RAY &&
                                                                                     f.IlKodu == adres.IlKodu &&
                                                                                     f.IlceKodu == adres.IlceKodu)
                                                                       .SingleOrDefault<CR_IlIlce>();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Ulke;

                            if (adres.UlkeKodu == "TUR") unsadr.ADR_DATA = RAY_UlkeKodlari.Turkiye;
                            else unsadr.ADR_DATA = RAY_UlkeKodlari.Yabanci;

                            adress.Add(unsadr);

                            string il = String.Empty;
                            string ilce = String.Empty;

                            if (ililce != null)
                            {
                                il = ililce.CRIlAdi;
                                ilce = ililce.CRIlceAdi;
                            }
                            else
                            {
                                il = "";
                                ilce = "";
                            }
                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Il;
                            unsadr.ADR_DATA = il;
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Ilce;
                            unsadr.ADR_DATA = ilce;
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Cadde;
                            unsadr.ADR_DATA = adres.Cadde.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Mahalle;
                            unsadr.ADR_DATA = adres.Mahalle.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Sokak;
                            unsadr.ADR_DATA = adres.Sokak.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Bina;
                            if (!String.IsNullOrEmpty(adres.BinaNo))
                                unsadr.ADR_DATA = adres.BinaNo;
                            else unsadr.ADR_DATA = "1";
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.PostaKodu;
                            unsadr.ADR_DATA = adres.PostaKodu.ToString();
                            adress.Add(unsadr);
                        }
                        musKayReq.UNSADR = adress.ToArray();
                        #endregion

                        #region Sorular
                        List<USRCVP> lists = new List<USRCVP>();
                        musKayReq.USRCVP = new USRCVP[1];
                        USRCVP usrcvp = new USRCVP();

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "24";//İrtibat Kurulabilecek Kişi
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "27";//İrtibat Kurulabilecek Kişinin mail adresi
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "23";//Araç Sayısı
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "26";//Şirket Türü, zorunludur.
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);
                        musKayReq.USRCVP = lists.ToArray();
                        #endregion

                        this.BeginLog(musKayReq, musKayReq.GetType(), WebServisIstekTipleri.MusteriKayit);

                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_Kaydetme_Request));
                        _Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(_Output, musKayReq);
                        string responseMusteri = clnt.ProductionIntegrator(_Output.ToString());
                        clnt.Dispose();
                        _Output.Close();

                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_Kaydetme_Response));
                        using (StringReader readerMus = new StringReader(responseMusteri))
                        {
                            var error = responseMusteri.Contains("ERROR");
                            if (!error)
                            {
                                musKaydRes = (RAY_Musteri_Kaydetme_Response)_Serialize.Deserialize(readerMus);
                                readerMus.ReadToEnd();
                                MusteriNo = musKaydRes.INFO_DESC;

                                this.EndLog(musKaydRes, true, musKaydRes.GetType());
                            }
                            else
                            {
                                _Serialize = new XmlSerializer(typeof(RAY_Musteri_KaydetmeHata_Response));
                                using (StringReader readerMusteriHata = new StringReader(responseMusteri))
                                {
                                    RAY_Musteri_KaydetmeHata_Response musterihatares = new RAY_Musteri_KaydetmeHata_Response();

                                    musterihatares = (RAY_Musteri_KaydetmeHata_Response)_Serialize.Deserialize(readerMusteriHata);
                                    readerMusteriHata.ReadToEnd();
                                    this.EndLog(musterihatares, false, musterihatares.GetType());
                                    this.AddHata(musterihatares.ERROR_DESC);
                                }
                            }
                        }
                    }
                #endregion

                }
            }

            catch (Exception ex)
            {
                #region Hata Log
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                #endregion
            }
            #endregion

            return MusteriNo;
        }

        public string IpGetir(int tvmKodu)
        {
            //string cagriIP = "82.222.165.62";
            string sigortaShopIp = "94.54.67.159";

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
            // return "78.186.150.79";
            return sigortaShopIp;

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;


namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MAPFREDask : Teklif, IMAPFREDask
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        ITVMService _TVMService;
        [InjectionConstructor]
        public MAPFREDask(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _TVMService = TVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.MAPFRE;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            mapfre.ZeyilWS response = null;
            mapfre.TanzimWSService ws = null;
            try
            {
                #region Veri Hazırlama
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFREKasko);
                mapfre.ZeyilWS request = this.TeklifRequest(teklif, konfig);
                #endregion

                #region Servis call
                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                this.BeginLog(request, typeof(mapfre.ZeyilWS), WebServisIstekTipleri.Teklif);
                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.issueTeklif(request);
                ws.Dispose();
                this.EndLog(response, true, typeof(mapfre.ZeyilWS));

                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                }
                #endregion

                #region Basarılı kontrolu
                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }
                #endregion

                #region Teklif Kaydı
                #region Genel Bilgiler
                this.Import(teklif);
                //this.GenelBilgiler.Basarili = true;
                //this.GenelBilgiler.BaslamaTarihi = MAPFREHelper.ToDateTime(response.polBaslamaTarihiLong);
                ////this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                //this.GenelBilgiler.BitisTarihi = MAPFREHelper.ToDateTime(response.polBitisTarihiLong);
                //this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                //this.GenelBilgiler.BrutPrim = (decimal)response.polBrutPrimYTL;
                //this.GenelBilgiler.NetPrim = (decimal)response.polNetPrimYTL;
                //decimal GiderVergisi = (decimal)response.zeyilVergileri.Where(w => w.vrgVergiKodu == "GV").Sum(s => s.pvrVergiYTLTutari);
                //this.GenelBilgiler.ToplamVergi = GiderVergisi;
                //this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;// Convert.ToByte(response.zeyilMusteriTaksitleri.Length);
                //this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                //this.GenelBilgiler.ToplamKomisyon = (decimal)response.polAcenteKomisyonuYTL;

                //// ==== Güncellenecek ==== //
                //this.GenelBilgiler.DovizKodu = response.polDovizKodu;
                //this.GenelBilgiler.DovizKurBedeli = (decimal)response.polDovizKuru;
                //this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                //this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                #endregion

                #region Vergiler
                //this.AddVergi(KaskoVergiler.GiderVergisi, GiderVergisi);
                #endregion

                #region Teminatlar
                // ==== 420	 A	 KASKO KASKO   	 ARÇ ==== //
                //var kasko = response.zeyilTaminalariRizikolari.FirstOrDefault(f => f.tetTeminatKodu == "KASKO");
                //if (kasko != null)
                //    this.AddTeminat(KaskoTeminatlar.Kasko, (decimal)kasko.pteYTLBedeli, 0, 0, 0, 0);

                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    this.AddOdemePlani(this.GenelBilgiler.TaksitSayisi.Value, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    //foreach (var item in response.zeyilMusteriTaksitleri)
                    //{
                    //    this.AddOdemePlani(1, MAPFREHelper.ToDateTime(item.vadeTarihiLong), (decimal)item.tutarYTL);
                    //}
                }
                #endregion

                #region Web servis cevapları
                //this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_No, response.polPoliceNo.ToString());
                //this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_Id, response.polPoliceID.ToString());
                //this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Zeyil_Id, response.polZeyilID.ToString());
                //this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBaslama_Tarih, response.polBaslamaTarihiLong.ToString());
                //this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBitis_Tarih, response.polBitisTarihiLong.ToString());
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            mapfre.ZeyilWS response = null;
            mapfre.TanzimWSService ws = null;
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                mapfre.ZeyilWS request = this.TeklifRequest(teklif, konfig);

                string mapPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_No, "0");
                string mapPoliceID = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_Id, "0");
                string mapZeyilID = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Zeyil_Id, "0");
                string mapPolBaslamaTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBaslama_Tarih, "0");
                string mapPolBitisTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBitis_Tarih, "0");

                //request.polPoliceNo = Convert.ToInt64(mapPoliceNo);
                //request.polPoliceID = Convert.ToInt64(mapPoliceID);
                //request.polZeyilID = Convert.ToInt64(mapZeyilID);
                //request.polBaslamaTarihiLong = Convert.ToInt64(mapPolBaslamaTarih);
                //request.polBitisTarihiLong = Convert.ToInt64(mapPolBitisTarih);

                request.vpos = new mapfre.PaymentRequestWS();


                #endregion

                #region Servis call

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];


                this.BeginLog(request, typeof(mapfre.ZeyilWS), WebServisIstekTipleri.Police);


                //Kart Bilgileri log dosyasına eklenmiyor.
                request.vpos.kartHamili = odeme.KrediKarti.KartSahibi;
#if DEBUG
                request.vpos.kartNo = odeme.KrediKarti.KartNo.Substring(0, 6);
#else
            request.vpos.kartNo = odeme.KrediKarti.KartNo;
#endif
                request.vpos.cvc = odeme.KrediKarti.CVC;
                request.vpos.expMonth = Convert.ToInt32(odeme.KrediKarti.SKA);
                request.vpos.expYear = Convert.ToInt32(odeme.KrediKarti.SKY);
                request.vpos.taksitSayisi = 1;

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.issueTekliftoPolice(request);
                ws.Dispose();
                this.EndLog(response, true, typeof(mapfre.ZeyilWS));

                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                }

                #endregion

                #region Hata Kontrol ve Kayıt

                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    TeklifWebServisCevap c = new TeklifWebServisCevap();
                    c.CevapKodu = Common.WebServisCevaplar.MAPFRE_Police_No;
                    c.CevapTipi = SoruCevapTipleri.Metin;
                    c.Cevap = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifWebServisCevaps.Add(c);
                    this.WebServisCevaplar.Add(c);

                    /*
                    DASK Hariç  Diğer Tüm Branşlarımızda  Kullanmakta Olduğumuz Poliçe Basım Programımız Maalesef Test Ortamında Çalışmamaktadır.
                    Bu Sebepten dolayı Trafik ve Kasko Branşları için Kesilen Poliçelerinizin Pdfleri Web Servisimizden Alınamamaktadır.
                    */

#if DEBUG
                    //this.PolicePDF();
#else 
                    //this.PolicePDF();
#endif
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void PolicePDF()
        {
            try
            {
                #region Veri  Hazırlama
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

                mapfre.ZeyilWS request = new mapfre.ZeyilWS();
                request.userName = servisKullanici.KullaniciAdi;
                request.passWord = servisKullanici.Sifre;
                request.brbBransKodu = "860";

                string mapPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Police_No, "0");
                string mapPoliceID = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Police_Id, "0");
                string mapZeyilID = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Zeyil_Id, "0");
                string mapPolBaslamaTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBaslama_Tarih, "0");
                string mapPolBitisTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBitis_Tarih, "0");

                //request.polPoliceNo = Convert.ToInt64(mapPoliceNo);
                //request.polPoliceID = Convert.ToInt64(mapPoliceID);
                //request.polZeyilID = Convert.ToInt64(mapZeyilID);
                //request.polBaslamaTarihiLong = Convert.ToInt64(mapPolBaslamaTarih);
                //request.polBitisTarihiLong = Convert.ToInt64(mapPolBitisTarih);
                #endregion

                #region Servis call
                //string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                //mapfre.PolicePrintValueWS response = null;

                //this.BeginLog(request, typeof(mapfre.ZeyilImplWs), WebServisIstekTipleri.Police);

                //mapfre.TeklifGenelGirisWSService ws = new mapfre.TeklifGenelGirisWSService();
                //ws.Url = serviceURL;
                //response = ws.policeBasim(request);

                //this.EndLog(response, true, typeof(mapfre.PolicePrintValueWS));

                //if (response.hataMesaji != null && response.hataMesaji.Length > 0)
                //{
                //    this.AddHata(response.hataMesaji);
                //}

                //if (response.printPdf == null && response.printPdf.Length == 0)
                //{
                //    this.AddHata("PDF dosyası alınamadı.");
                //}
                #endregion

                #region Hata Kontrol ve Kayıt
                //if (this.Hatalar.Count == 0)
                //{
                //    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                //    string fileName = String.Format("mapfre_{0}", System.Guid.NewGuid().ToString());

                //    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, response.printPdf);
                //    this.GenelBilgiler.PDFDosyasi = pdfUrl;

                //    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //}
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

        private mapfre.ZeyilWS TeklifRequest(ITeklif teklif, KonfigTable konfig)
        {
            #region Genel Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

            mapfre.ZeyilWS request = new mapfre.ZeyilWS();
            request.userName = servisKullanici.KullaniciAdi;
            request.passWord = servisKullanici.Sifre;

            DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            //request.polBaslamaTarihiLong = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            //request.polBitisTarihiLong = MAPFREHelper.ToMAPFREDateLong(policeBaslangic.AddYears(1));
            //request.polTanzimTarihiLong = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            //request.polDovizKodu = "YTL";
            request.brbBransKodu = "860";

            // "ÖZL" //ÖZEL
            // "BYN" //BEYAN
            // "DEN" //DÖVIZE ENDEKSLI POLIÇE
            // "DVZ" //DÖVIZLI POLIÇE
            // "ENF" //ENFLASYON ENDEKSLI POLIÇE
            //request.potPoliceTipKodu = "ENF";
            //request.polOdemeKodu = "002";
            request.processID = 1;

            mapfre.TramerBilgileriWS tramerBilgileri = new mapfre.TramerBilgileriWS();
            tramerBilgileri.islem = "Surprim";
            tramerBilgileri.zkymsVar = "0";
            request.tramerBilgileri = tramerBilgileri;
            #endregion

            #region Sigortali
            //TeklifSigortali teklifSigortali = teklif.Sigortalilar.FirstOrDefault();
            //if (teklifSigortali != null)
            //{
            //    MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);

            //    request.polSigortaliAdi = sigortali.AdiUnvan;
            //    request.polSigortaliSoyadi = sigortali.SoyadiUnvan;
            //    request.polSigortali = sigortali.AdiUnvan + " " + sigortali.SoyadiUnvan;

            //    if (MusteriTipleri.Ozel(sigortali.MusteriTipKodu))
            //    {
            //        request.polSigortaliTurKodu = sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri ? "OZ" : "OY";
            //        request.polSigortaliTCKimlikNo = Convert.ToInt64(sigortali.KimlikNo);
            //    }
            //    else
            //    {
            //        request.polSigortaliTurKodu = "TT";
            //        request.polSigortaliVergiDaire = sigortali.VergiDairesi;
            //        request.polSigortaliVergiNo = Convert.ToInt64(sigortali.KimlikNo);
            //    }

            //    MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
            //    if (sigortaliAdres != null)
            //        request.polSigortaliAdres = sigortaliAdres.Adres;

            //}
            #endregion

            #region Konut Bilgileri
            //int sahaSiraNo = 1;
            //List<mapfre.ZeyilSahaWS> sahalar = new List<mapfre.ZeyilSahaWS>();

            //// RIZIKO ADRESI 1.  SATIR Bir 25 Karakterden Fazla Veri Alamaz
            //mapfre.ZeyilSahaWS rizikoAdresiBir = new mapfre.ZeyilSahaWS();
            //rizikoAdresiBir.shtSahaKodu = "Y0007";
            //rizikoAdresiBir.pshSahaSiraNumara = sahaSiraNo;
            //rizikoAdresiBir.pshSahaDeger = teklif.RizikoAdresi.Mahalle + "mah.";
            //sahalar.Add(rizikoAdresiBir);
            //sahaSiraNo++;


            //// RIZIKO ADRESI 2.  SATIR Bir 25 Karakterden Fazla Veri Alamaz
            //mapfre.ZeyilSahaWS rizikoAdresiIki = new mapfre.ZeyilSahaWS();
            //rizikoAdresiIki.shtSahaKodu = "Y0008";
            //rizikoAdresiIki.pshSahaSiraNumara = sahaSiraNo;
            //rizikoAdresiIki.pshSahaDeger = teklif.RizikoAdresi.Cadde + "cad.";
            //sahalar.Add(rizikoAdresiIki);
            //sahaSiraNo++;


            //// RIZIKO ADRESI 3.  SATIR Bir 25 Karakterden Fazla Veri Alamaz
            //mapfre.ZeyilSahaWS rizikoAdresiUc = new mapfre.ZeyilSahaWS();
            //rizikoAdresiUc.shtSahaKodu = "Y0009";
            //rizikoAdresiUc.pshSahaSiraNumara = sahaSiraNo;
            //rizikoAdresiUc.pshSahaDeger = teklif.RizikoAdresi.Sokak + "sk. No:" + teklif.RizikoAdresi.Bina + "";
            //sahalar.Add(rizikoAdresiUc);
            //sahaSiraNo++;

            //// RIZIKO ADRESI 4.  SATIR Bir 25 Karakterden Fazla Veri Alamaz
            //mapfre.ZeyilSahaWS rizikoAdresiDort = new mapfre.ZeyilSahaWS();
            //rizikoAdresiDort.shtSahaKodu = "Y0009";
            //rizikoAdresiDort.pshSahaSiraNumara = sahaSiraNo;
            //rizikoAdresiDort.pshSahaDeger = "";
            //sahalar.Add(rizikoAdresiDort);
            //sahaSiraNo++;


            //// YAPI TARZI : "03" = ADI KAGIR ,"04" = AHSAP,"01" = TAM KAGIR,"02" = YIGMA KAGIR
            //mapfre.ZeyilSahaWS yapiTarzi = new mapfre.ZeyilSahaWS();
            //yapiTarzi.shtSahaKodu = "Y0010";
            //yapiTarzi.pshSahaSiraNumara = sahaSiraNo;
            //yapiTarzi.pshSahaDeger = "01";// teklif.ReadSoru(DASKSorular.Yapi_Tarzi, "0"); ;
            //sahalar.Add(yapiTarzi);
            //sahaSiraNo++;


            //// KULLANIM SEKLI : "S\u00dcREKL\u0130" , "YAZLIK"
            //mapfre.ZeyilSahaWS kullanimSekli = new mapfre.ZeyilSahaWS();
            //kullanimSekli.shtSahaKodu = "Y0051";
            //kullanimSekli.pshSahaSiraNumara = sahaSiraNo;
            //kullanimSekli.pshSahaDeger = "S\u00dcREKL\u0130";
            //sahalar.Add(kullanimSekli);
            //sahaSiraNo++;


            //if (kullanimSekli.pshSahaDeger == "YAZLIK")
            //{
            //    // BOS KALAN AY SAYISI: KULLANIM SEKLI "YAZLIK" ise Zorunludur
            //    mapfre.ZeyilSahaWS bosKalanAySayisi = new mapfre.ZeyilSahaWS();
            //    bosKalanAySayisi.shtSahaKodu = "Y0053";
            //    bosKalanAySayisi.pshSahaSiraNumara = sahaSiraNo;
            //    bosKalanAySayisi.pshSahaDeger = "0";
            //    sahalar.Add(bosKalanAySayisi);
            //    sahaSiraNo++;
            //}

            //// RIZIKO ILI : 1-81 Arasi Deger Alir
            //mapfre.ZeyilSahaWS rizikoIli = new mapfre.ZeyilSahaWS();
            //rizikoIli.shtSahaKodu = "Y0136";
            //rizikoIli.pshSahaSiraNumara = sahaSiraNo;
            //rizikoIli.pshSahaDeger = "1";//teklif.RizikoAdresi.IlKodu.ToString();
            //sahalar.Add(rizikoIli);
            //sahaSiraNo++;


            //// RIZIKO ILCESI
            //mapfre.ZeyilSahaWS rizikoIlcesi = new mapfre.ZeyilSahaWS();
            //rizikoIlcesi.shtSahaKodu = "Y0137";
            //rizikoIlcesi.pshSahaSiraNumara = sahaSiraNo;
            //rizikoIlcesi.pshSahaDeger = "1";// teklif.RizikoAdresi.IlceKodu.ToString();
            //sahalar.Add(rizikoIlcesi);
            //sahaSiraNo++;


            ///// RIZIKO BELDESI
            //mapfre.ZeyilSahaWS rizikoBeldesi = new mapfre.ZeyilSahaWS();
            //rizikoBeldesi.shtSahaKodu = "Y0138";
            //rizikoBeldesi.pshSahaSiraNumara = sahaSiraNo;
            //rizikoBeldesi.pshSahaDeger = "1";//teklif.RizikoAdresi.SemtBelde;
            //sahalar.Add(rizikoBeldesi);
            //sahaSiraNo++;


            //// SIGORTALANAN DAIRE SAYISI
            //mapfre.ZeyilSahaWS sigortalananDaireSayisi = new mapfre.ZeyilSahaWS();
            //sigortalananDaireSayisi.shtSahaKodu = "Y0052";
            //sigortalananDaireSayisi.pshSahaSiraNumara = sahaSiraNo;
            //sigortalananDaireSayisi.pshSahaDeger = "1";
            //sahalar.Add(sigortalananDaireSayisi);
            //sahaSiraNo++;


            //// DASK  KAPSAMINDA MI : E/H
            //mapfre.ZeyilSahaWS daskKapsamindami = new mapfre.ZeyilSahaWS();
            //daskKapsamindami.shtSahaKodu = "Y0065";
            //daskKapsamindami.pshSahaSiraNumara = sahaSiraNo;
            //daskKapsamindami.pshSahaDeger = "H";
            //sahalar.Add(daskKapsamindami);
            //sahaSiraNo++;


            //if (daskKapsamindami.pshSahaDeger == "E")
            //{
            //    // DASK BASLAMA TARIHI : DASK KAPSAMINDA MI == E DOLU OLMALI
            //    mapfre.ZeyilSahaWS daskBaslamaTarihi = new mapfre.ZeyilSahaWS();
            //    daskBaslamaTarihi.shtSahaKodu = "Y0038";
            //    daskBaslamaTarihi.pshSahaSiraNumara = sahaSiraNo;
            //    daskBaslamaTarihi.pshSahaDeger = "09/10/2012";  //dd/MM/yyyy
            //    sahalar.Add(daskBaslamaTarihi);
            //    sahaSiraNo++;


            //    // DASK BEDELI : DASK KAPSAMINDA MI == E DOLU OLMALI
            //    mapfre.ZeyilSahaWS daskBedeli = new mapfre.ZeyilSahaWS();
            //    daskBedeli.shtSahaKodu = "Y0068";
            //    daskBedeli.pshSahaSiraNumara = sahaSiraNo;
            //    daskBedeli.pshSahaDeger = "330000";
            //    sahalar.Add(daskBedeli);
            //    sahaSiraNo++;


            //    //DASK POLIÇE NUMARASI: DASK KAPSAMINDA MI == E DOLU OLMALI
            //    mapfre.ZeyilSahaWS daskPoliceNo = new mapfre.ZeyilSahaWS();
            //    daskPoliceNo.shtSahaKodu = "Y0068";
            //    daskPoliceNo.pshSahaSiraNumara = sahaSiraNo;
            //    daskPoliceNo.pshSahaDeger = "330000";
            //    sahalar.Add(daskPoliceNo);
            //    sahaSiraNo++;
            //}


            //// BINANIN BRÜT ALANI  (M2
            //mapfre.ZeyilSahaWS binaninBrutAlani = new mapfre.ZeyilSahaWS();
            //binaninBrutAlani.shtSahaKodu = "Y0066";
            //binaninBrutAlani.pshSahaSiraNumara = sahaSiraNo;
            //binaninBrutAlani.pshSahaDeger = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, 0).ToString();
            //sahalar.Add(binaninBrutAlani);
            //sahaSiraNo++;


            //// METREKARE BIRIM FIYATI
            //mapfre.ZeyilSahaWS metrekareBirimFiyati = new mapfre.ZeyilSahaWS();
            //metrekareBirimFiyati.shtSahaKodu = "Y0067";
            //metrekareBirimFiyati.pshSahaSiraNumara = sahaSiraNo;
            //metrekareBirimFiyati.pshSahaDeger = "120";
            //sahalar.Add(metrekareBirimFiyati);
            //sahaSiraNo++;


            //// RISK TEFTIS SARTI E/H
            //mapfre.ZeyilSahaWS riskTeftisSarti = new mapfre.ZeyilSahaWS();
            //riskTeftisSarti.shtSahaKodu = "RZKTF";
            //riskTeftisSarti.pshSahaSiraNumara = sahaSiraNo;
            //riskTeftisSarti.pshSahaDeger = "H";
            //sahalar.Add(riskTeftisSarti);
            //sahaSiraNo++;


            //if (riskTeftisSarti.pshSahaDeger == "E")
            //{
            //    // EKSPERTIZ TARIHI
            //    mapfre.ZeyilSahaWS ekspertizTarihi = new mapfre.ZeyilSahaWS();
            //    ekspertizTarihi.shtSahaKodu = "Y0072";
            //    ekspertizTarihi.pshSahaSiraNumara = sahaSiraNo;
            //    ekspertizTarihi.pshSahaDeger = "09/10/2012"; //dd/MM/yyyy
            //    sahalar.Add(ekspertizTarihi);
            //    sahaSiraNo++;


            //    // EKSPERTIZ RAPOR NO
            //    mapfre.ZeyilSahaWS ekspertizRaporNo = new mapfre.ZeyilSahaWS();
            //    ekspertizRaporNo.shtSahaKodu = "Y0073";
            //    ekspertizRaporNo.pshSahaSiraNumara = sahaSiraNo;
            //    ekspertizRaporNo.pshSahaDeger = "4654654";
            //    sahalar.Add(ekspertizRaporNo);
            //    sahaSiraNo++;
            //}

            //// FERDI KAZA KISI SAYISI
            //mapfre.ZeyilSahaWS ferdiKazaKisiSayisi = new mapfre.ZeyilSahaWS();
            //ferdiKazaKisiSayisi.shtSahaKodu = "F0063";
            //ferdiKazaKisiSayisi.pshSahaSiraNumara = sahaSiraNo;
            //ferdiKazaKisiSayisi.pshSahaDeger = "1";
            //sahalar.Add(ferdiKazaKisiSayisi);
            //sahaSiraNo++;


            //// FERDI KAZA ÖLÜM/SS TEM. LIMITI:
            //mapfre.ZeyilSahaWS FKOlumSS = new mapfre.ZeyilSahaWS();
            //FKOlumSS.shtSahaKodu = "F0102";
            //FKOlumSS.pshSahaSiraNumara = sahaSiraNo;
            //FKOlumSS.pshSahaDeger = "10000";
            //sahalar.Add(FKOlumSS);
            //sahaSiraNo++;


            //// F.K. TEDAVI LIMITI:
            //mapfre.ZeyilSahaWS FKTedaviLimiti = new mapfre.ZeyilSahaWS();
            //FKTedaviLimiti.shtSahaKodu = "F0103";
            //FKTedaviLimiti.pshSahaSiraNumara = sahaSiraNo;
            //FKTedaviLimiti.pshSahaDeger = "1000";
            //sahalar.Add(FKTedaviLimiti);
            //sahaSiraNo++;


            //// AZAMI TAZMINAT LIMITI:
            //mapfre.ZeyilSahaWS FKAzamiTazminat = new mapfre.ZeyilSahaWS();
            //FKAzamiTazminat.shtSahaKodu = "F0155";
            //FKAzamiTazminat.pshSahaSiraNumara = sahaSiraNo;
            //FKAzamiTazminat.pshSahaDeger = "30000";
            //sahalar.Add(FKAzamiTazminat);
            //sahaSiraNo++;


            //// GV ALINACAK MI (E/H)
            //mapfre.ZeyilSahaWS GVAlinacakmi = new mapfre.ZeyilSahaWS();
            //GVAlinacakmi.shtSahaKodu = "Y0036";
            //GVAlinacakmi.pshSahaSiraNumara = sahaSiraNo;
            //GVAlinacakmi.pshSahaDeger = "E";
            //sahalar.Add(GVAlinacakmi);
            //sahaSiraNo++;


            //// YSV ALINACAK MI (E/H)
            //mapfre.ZeyilSahaWS YSVAlinacakmi = new mapfre.ZeyilSahaWS();
            //YSVAlinacakmi.shtSahaKodu = "Y0037";
            //YSVAlinacakmi.pshSahaSiraNumara = sahaSiraNo;
            //YSVAlinacakmi.pshSahaDeger = "E";
            //sahalar.Add(YSVAlinacakmi);
            //sahaSiraNo++;


            //// BEDEL NOTU: "2" MUTABAKAT BEDEL, "1" RAYIC BEDEL
            //mapfre.ZeyilSahaWS bedelNotu = new mapfre.ZeyilSahaWS();
            //bedelNotu.shtSahaKodu = "BDLNT";
            //bedelNotu.pshSahaSiraNumara = sahaSiraNo;
            //bedelNotu.pshSahaDeger = "1";
            //sahalar.Add(bedelNotu);
            //sahaSiraNo++;


            ////  TOPLAM SIG.BEDELI % 100
            //mapfre.ZeyilSahaWS toplamSigortaBedeli = new mapfre.ZeyilSahaWS();
            //toplamSigortaBedeli.shtSahaKodu = "Y0106";
            //toplamSigortaBedeli.pshSahaSiraNumara = sahaSiraNo;
            //toplamSigortaBedeli.pshSahaDeger = "330000";
            //sahalar.Add(toplamSigortaBedeli);
            //sahaSiraNo++;


            //request.zeyilSahalari = sahalar.ToArray();
            #endregion

            return request;
        }
    }
}

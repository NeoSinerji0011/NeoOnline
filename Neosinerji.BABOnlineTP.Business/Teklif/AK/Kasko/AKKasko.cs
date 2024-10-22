using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages;
using Neosinerji.BABOnlineTP.Business.Common.SOMPOJAPANCommon;
using Neosinerji.BABOnlineTP.Business.aksigorta.kasko;
using System.Web.Security;
using System.Security.Cryptography;
using OutSourceLib;
using System.IO;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.AK
{
    public class AKKasko : Teklif, IAKKasko
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
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;
        [InjectionConstructor]
        public AKKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
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
            _TVMService = TVMService;
            _AktifKullaniciService = aktifKullaniciService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.AK;
            }
        }


        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "JGE9VyT933aQ*DM2";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new
                    Rfc2898DeriveBytes(EncryptionKey, new byte[]
                    { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public override void Hesapla(ITeklif teklif)
        {

            //kaskoTeklifOlusturmaInput kaskoTeklifOlusturmaInput = new kaskoTeklifOlusturmaInput();


            //kaskoTeklifOlusturmaInput.kanalBilgileri = new kanalBilgileriType();
            //kaskoTeklifOlusturmaInput.kanalBilgileri.branchId = "1481";
            //kaskoTeklifOlusturmaInput.kanalBilgileri.kanalId = "127";
            //kaskoTeklifOlusturmaInput.kanalBilgileri.token = "l38oAKDG3L7R1b6y";

            //kaskoTeklifOlusturmaInput.policeBilgileri = new kaskoPoliceBilgileriType();
            //kaskoTeklifOlusturmaInput.policeBilgileri.policeBaslamaTarihi = "17/04/2021";
            //kaskoTeklifOlusturmaInput.policeBilgileri.kaskoTarifeKod = "K11";

            //kaskoTeklifOlusturmaInput.sigortaEttiren = new sigortaEttirenType();
            //kaskoTeklifOlusturmaInput.sigortaEttiren.sigortaliSigortaEttirenFarkliMi = "H";
            //kaskoTeklifOlusturmaInput.sigortaEttiren.kimlikNo = "31708709780";

            //kaskoTeklifOlusturmaInput.sigortali = new sigortaliType();

            //kaskoTeklifOlusturmaInput.aracBilgileri = new aracBilgileriType();
            //kaskoTeklifOlusturmaInput.aracBilgileri.aracMarkaKodu = "114";
            //kaskoTeklifOlusturmaInput.aracBilgileri.aracTipKodu = "1029";
            //kaskoTeklifOlusturmaInput.aracBilgileri.modelYili = "2013";
            //kaskoTeklifOlusturmaInput.aracBilgileri.koltukSayisi = "1";
            //kaskoTeklifOlusturmaInput.aracBilgileri.kullanimTarzi = "01";
            //kaskoTeklifOlusturmaInput.aracBilgileri.tescilBelgeSeriNo = "147132";
            //kaskoTeklifOlusturmaInput.aracBilgileri.plakaIlKodu = "06";
            //kaskoTeklifOlusturmaInput.aracBilgileri.plakaNo = "AE9247";
            //kaskoTeklifOlusturmaInput.aracBilgileri.yeniAracMi = "H";

            //kaskoTeklifOlusturmaInput.tahsilatBilgileri = new tahsilatBilgileriType();
            //kaskoTeklifOlusturmaInput.tahsilatBilgileri.pesinVadeli = "P";

            //kaskoTeklifOlusturmaInput.teminatListesi = new teminatListesiType();


            //istatistikType[] istatistikTypes = new istatistikType[10];

            //istatistikTypes[0] = new istatistikType { kod = "KUL", deger = "026" };
            //istatistikTypes[0] = new istatistikType { kod = "795", deger = "003" };
            //istatistikTypes[0] = new istatistikType { kod = "533", deger = "001" };
            //istatistikTypes[0] = new istatistikType { kod = "791", deger = "002" };
            //istatistikTypes[0] = new istatistikType { kod = "773", deger = "001" };
            //istatistikTypes[0] = new istatistikType { kod = "ANK", deger = "002" };
            //istatistikTypes[0] = new istatistikType { kod = "YAB", deger = "000" };
            //istatistikTypes[0] = new istatistikType { kod = "608", deger = "012" };
            //istatistikTypes[0] = new istatistikType { kod = "582", deger = "001" };

            //kaskoTeklifOlusturmaInput.istatistikListesi = new istatistikListesiType();
            //kaskoTeklifOlusturmaInput.istatistikListesi.istatistik = istatistikTypes;



            //var res = new kaskoTeklifOlusturRequest(kaskoTeklifOlusturmaInput);

            webservicesgenelv2KaskoWebService client = new webservicesgenelv2KaskoWebService();
            client.Url = "https://testapi.aksigorta.com.tr/api/kaskoWS-V3.0/KaskoWebService";
            client.Timeout = 150000;
            client.Credentials = new System.Net.NetworkCredential("ardaSigorta", "b78eac5da4f540a090a6e790b");
            //kaskoClient.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AK_UserName], konfig[Konfig.AK_Password]);
            //var response = client.kaskoTeklifOlustur(kaskoTeklifOlusturmaInput);
            var sifre = Encrypt("4355084355084358");


            try
            {
                
                string pdfUrl = String.Empty;
               

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AK });

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Url = konfig[Konfig.AK_CascoServiceURLV2];
                client.Timeout = 150000;
                client.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AK_UserName], konfig[Konfig.AK_Password]);

                //List<CPartnerBilgi> parners = new List<CPartnerBilgi>();
                //List<string> mesajlar = new List<string>();
                //mesajlar.Add("deneme");

                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                //string polNo=teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //CYenilemeDetay detay = new CYenilemeDetay();
                //detay.PoliceNo =Convert.ToDecimal(polNo);
                //detay.TanzimTarihi = polBaslangic;
                //detay.RefKullanici = servisKullanici.KullaniciAdi;
                //var rs=client.PoliceYenile(detay);

                #region Sigortali / Sigorta Ettiren Bilgileri Hazırlama

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriAdre sigortaliAdres = null;
                if (sigortali != null)
                {
                    sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                }

                var sigEttiren = teklif.SigortaEttiren;
                MusteriAdre sigEttirenAdres = null;
                if (sigEttiren != null)
                {
                    sigEttirenAdres = sigEttiren.MusteriGenelBilgiler.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                }

                string akSigIlKodu = String.Empty;
                string akSigIlceKodu = String.Empty;

                string akSigEttirenIlKodu = String.Empty;
                string akSigEttirenIlceKodu = String.Empty;
                MusteriTelefon sigortaliCepTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                MusteriTelefon sigortaliEvTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault();
                string MusteriEvTel = "";
                string MusteriCepTel = "";

                if (sigortaliCepTelefon == null)
                {
                    sigortaliCepTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                }
                if (sigortaliEvTelefon == null)
                {
                    sigortaliEvTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                }
                if (sigortaliCepTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliCepTelefon.Numara) && sigortaliCepTelefon.Numara.Trim() != "90" && sigortaliCepTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliCepTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            MusteriCepTel = sigortaliCepTelefon.Numara.Substring(1, 1) + "(";
                            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(3, 3) + ")";
                            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(7, 7);
                        }
                    }
                    else
                    {
                        MusteriCepTel = "0(555)9999999";
                    }
                }
                if (sigortaliEvTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliEvTelefon.Numara) && sigortaliEvTelefon.Numara.Trim() != "90" && sigortaliEvTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliEvTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            MusteriEvTel = sigortaliEvTelefon.Numara.Substring(1, 1) + "(";
                            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(3, 3) + ")";
                            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(7, 7);
                        }
                    }
                    else
                    {
                        MusteriEvTel = "0(212)9999999";
                    }
                }

                bool MusteriAyniMi = false;
                if (sigEttiren.MusteriKodu == sigortali.MusteriKodu)
                {
                    MusteriAyniMi = true;
                    if (sigortaliAdres != null)
                    {
                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AK &&
                                                                                 f.IlKodu == sigortaliAdres.IlKodu &&
                                                                                 f.IlceKodu == sigortaliAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();
                        if (ililce != null)
                        {
                            akSigIlKodu = ililce.CRIlKodu;
                            akSigIlceKodu = ililce.CRIlceKodu;
                        }
                    }
                }
                else
                {
                    MusteriAyniMi = false;

                    if (sigEttirenAdres != null)
                    {

                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AK &&
                                                                                 f.IlKodu == sigEttirenAdres.IlKodu &&
                                                                                 f.IlceKodu == sigEttirenAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();
                        if (ililce != null)
                        {
                            akSigEttirenIlKodu = ililce.CRIlKodu;
                            akSigEttirenIlceKodu = ililce.CRIlceKodu;
                        }
                    }
                }

                #endregion

                #region Eski Poliçe Bilgileri Hazırlama

                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

                string TramerPoliceNo = "";
                string TramerAcenteNo = "";
                string TramerSirketKodu = "";
                string TramerYenilemeNo = "";
                if (eskiPoliceVar)
                {
                    TramerPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    TramerAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    TramerSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    TramerYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                }

                #endregion

                #region Partner Bilgileri Hazırlama
                //  List<CMatbuBilgi> matBuList = new List<CMatbuBilgi>();


                #endregion

                #region Police Aciklama Bilgileri Hazırlama
                List<string> aciklamalar = new List<string>();


                #endregion

               




                kaskoTeklifOlusturmaInput kaskoTeklifOlusturmaInput = new kaskoTeklifOlusturmaInput();


                kaskoTeklifOlusturmaInput.kanalBilgileri = new kanalBilgileriType();
                kaskoTeklifOlusturmaInput.kanalBilgileri.branchId = "1481";
                kaskoTeklifOlusturmaInput.kanalBilgileri.kanalId = "127";
                kaskoTeklifOlusturmaInput.kanalBilgileri.token = "l38oAKDG3L7R1b6y";

                kaskoTeklifOlusturmaInput.policeBilgileri = new kaskoPoliceBilgileriType();
                kaskoTeklifOlusturmaInput.policeBilgileri.policeBaslamaTarihi = polBaslangic.ToShortDateString();
                kaskoTeklifOlusturmaInput.policeBilgileri.kaskoTarifeKod = "K11";

                kaskoTeklifOlusturmaInput.sigortaEttiren = new sigortaEttirenType();
                kaskoTeklifOlusturmaInput.sigortaEttiren.sigortaliSigortaEttirenFarkliMi = "H";
                kaskoTeklifOlusturmaInput.sigortaEttiren.kimlikNo = sigEttiren.MusteriGenelBilgiler.KimlikNo;

                kaskoTeklifOlusturmaInput.sigortali = new sigortaliType();

                kaskoTeklifOlusturmaInput.aracBilgileri = new aracBilgileriType();
                kaskoTeklifOlusturmaInput.aracBilgileri.aracMarkaKodu = teklif.Arac.Marka;
                kaskoTeklifOlusturmaInput.aracBilgileri.aracTipKodu = teklif.Arac.AracinTipi;
                kaskoTeklifOlusturmaInput.aracBilgileri.modelYili = teklif.Arac.Model.ToString();
                kaskoTeklifOlusturmaInput.aracBilgileri.koltukSayisi = teklif.Arac.KoltukSayisi.ToString();


                string kullanimT = String.Empty;
                string kod2 = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    kullanimT = parts[0];
                    kod2 = parts[1];
                }
                kaskoTeklifOlusturmaInput.aracBilgileri.kullanimTarzi = kullanimT;
                kaskoTeklifOlusturmaInput.aracBilgileri.tescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                kaskoTeklifOlusturmaInput.aracBilgileri.plakaIlKodu = teklif.Arac.PlakaKodu;
                kaskoTeklifOlusturmaInput.aracBilgileri.plakaNo = teklif.Arac.PlakaNo;
                kaskoTeklifOlusturmaInput.aracBilgileri.yeniAracMi = "H";

                kaskoTeklifOlusturmaInput.tahsilatBilgileri = new tahsilatBilgileriType();
                kaskoTeklifOlusturmaInput.tahsilatBilgileri.pesinVadeli = "P";

                kaskoTeklifOlusturmaInput.teminatListesi = new teminatListesiType();


                istatistikType[] istatistikTypes = new istatistikType[10];

                istatistikTypes[0] = new istatistikType { kod = "KUL", deger = "026" };
                istatistikTypes[0] = new istatistikType { kod = "795", deger = "003" };
                istatistikTypes[0] = new istatistikType { kod = "533", deger = "001" };
                istatistikTypes[0] = new istatistikType { kod = "791", deger = "002" };
                istatistikTypes[0] = new istatistikType { kod = "773", deger = "001" };
                istatistikTypes[0] = new istatistikType { kod = "ANK", deger = "002" };
                istatistikTypes[0] = new istatistikType { kod = "YAB", deger = "000" };
                istatistikTypes[0] = new istatistikType { kod = "608", deger = "012" };
                istatistikTypes[0] = new istatistikType { kod = "582", deger = "001" };

                kaskoTeklifOlusturmaInput.istatistikListesi = new istatistikListesiType();
                kaskoTeklifOlusturmaInput.istatistikListesi.istatistik = istatistikTypes;




                #region Police Header
                bool tcMi = true;
                if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                {
                    tcMi = false;
                }
                //CPoliceBaslik3 policeBaslik = new CPoliceBaslik3()
                //{
                //    SubeKod = 0,
                //    KaynakKod = 300,
                //    AcenteNo = servisKullanici.PartajNo_,
                //    TaliKaynak = 0,
                //    UrunKodu = AXA_UrunKodlari.Kasko,
                //    Doviz_Kod = "TL",
                //    Teklif_Tarih = DateTime.Now,
                //    Tanzim_Tarih = DateTime.Now,
                //    Baslama_Tarih = polBaslangic,
                //    Bitis_Tarih = polBaslangic.AddYears(1),
                //    PlakaNo = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo,
                //    TescilBelgeNo = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo,
                //    OtomatikYenileme = 0,
                //    RefKullanici = servisKullanici.KullaniciAdi2,
                //    UretimYeri = 0,

                //    //Hasarsızlık bilgisinin sorgulanması için gönderiliyor
                //    TramerPoliceNo = TramerPoliceNo,
                //    TramerSirketKod = TramerSirketKodu,
                //    TramerAcenteNo = TramerAcenteNo,
                //    TramerYenilemeNo = TramerYenilemeNo,

                //    ClientIPAddress = this.IpGetir(teklif.GenelBilgiler.TVMKodu),
                //    //EskiPlakaNo = "",
                //    //MTKod = "",
                //    //OtrMail = "",
                //    //PoliceCinsi = "", // boş olabilir
                //    //TaliAcente = "",                  
                //    // IngEkAciklamalar = aciklamalar.ToArray(),                  
                //    //TeknikPersonel = "",
                //    // TurceEkAciklamalar = aciklamalar.ToArray(),

                //    RizikoAdresi = new CMusteriAdres()
                //    {
                //        //UavtAdresId=0,
                //        Ulke_Kod = "TÜR",
                //        Il_Kod = akSigIlKodu,
                //        Ilce_Kod = akSigIlceKodu,
                //        Semt_Kod = "000",
                //        Mahalle = ".",
                //        Bina_No = "1",
                //        Daire_No = "1",
                //        Posta_Kod = "340600",
                //        Han_Apt_Fab = ".",

                //    },
                //    SigortaEttiren = new CMusteriAxa()
                //    {
                //        Adres = new CMusteriAdres()
                //        {
                //            //Zorunlu alanlar.
                //            Ulke_Kod = "TÜR",
                //            Il_Kod = MusteriAyniMi ? akSigIlKodu : akSigEttirenIlKodu,
                //            Ilce_Kod = MusteriAyniMi ? akSigIlceKodu : akSigEttirenIlceKodu,
                //            Semt_Kod = "000",
                //            Mahalle = MusteriAyniMi ? sigortaliAdres.Mahalle : sigEttirenAdres.Mahalle,
                //            Sokak = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : sigEttirenAdres.Sokak) : ".",
                //            Cadde = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : sigEttirenAdres.Cadde) : ".",
                //            Daire_No = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : sigEttirenAdres.DaireNo) : "1",
                //            //-------Zorunlu alanlar

                //            //Posta_Kod = "",
                //            //Han_Apt_Fab = "",
                //            //Bina_No = "13",
                //            //UavtAdresId = 0,
                //        },
                //        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                //        {

                //            UlkeKod = "TÜR",
                //        },
                //        IletisimBilgileri = new CMusteriIletisimBilgi()
                //        {
                //            Email = MusteriAyniMi ? sigortali.EMail : sigEttiren.MusteriGenelBilgiler.EMail,// "deneme@deneme.com",
                //            Fax_No = "",
                //            GSM_No = MusteriCepTel,
                //            Tel_No = MusteriEvTel,
                //        },
                //        //Mesajlar = mesajlar.ToArray(),
                //        //PartnerBilgiler = parners.ToArray()

                //    },
                //    Sigortali = new CMusteriAxa()
                //    {
                //        Adres = new CMusteriAdres()
                //        {
                //            //Zorunlu alanlar.
                //            Ulke_Kod = "TÜR",
                //            Il_Kod = akSigIlKodu,
                //            Ilce_Kod = akSigIlceKodu,
                //            Semt_Kod = "000",
                //            Mahalle = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Mahalle : ".",
                //            Sokak = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : ".",
                //            Cadde = !String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : ".",
                //            Daire_No = !String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : "1",
                //            //-------Zorunlu alanlar

                //            //Posta_Kod = "",
                //            //Han_Apt_Fab = "",
                //            //Bina_No = "13",
                //            //UavtAdresId = 0,
                //        },
                //        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                //        {

                //            UlkeKod = "TÜR",
                //        },
                //        IletisimBilgileri = new CMusteriIletisimBilgi()
                //        {
                //            Email = sigortali.EMail,//"deneme@deneme.com",
                //            Fax_No = "",//"0(212)9999999",
                //            GSM_No = MusteriCepTel,//"0(543)9999999",
                //            Tel_No = MusteriEvTel,//"0(212)9999999",
                //        },
                //        // Mesajlar = mesajlar.ToArray(),
                //        // PartnerBilgiler = parners.ToArray()
                //    },


                //};

                //if (tcMi)
                //{
                //    policeBaslik.SigortaEttiren.GenelBilgiler.TcNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttiren.MusteriGenelBilgiler.KimlikNo;
                //    policeBaslik.Sigortali.GenelBilgiler.TcNo = sigortali.KimlikNo;
                //}
                //else
                //{
                //    policeBaslik.SigortaEttiren.GenelBilgiler.VergiNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttiren.MusteriGenelBilgiler.KimlikNo;
                //    policeBaslik.Sigortali.GenelBilgiler.VergiNo = sigortali.KimlikNo;
                //}

                #endregion

                //   #region Poliçe Detail

                //baseKaskoPrimHesaplamaOutput policeDetay = new baseKaskoPrimHesaplamaOutput();

                //#region Teminatlar

                //List<teminatSonucBilgisi> teminatList = new List<teminatSonucBilgisi>();

                //#region Zorunlu Teminatlar

                //teminatSonucBilgisi teminat = new teminatSonucBilgisi()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    teminatKodu = Ak_KaskoTeminatlar.KASKO

                //};
                //teminatList.Add(teminat);

                //teminat = new teminatSonucBilgisi()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    teminatKodu = Ak_KaskoTeminatlar.GLKHH_TERÖR,

                //};
                //teminatList.Add(teminat);


                //string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                //if (!String.IsNullOrEmpty(fkKademe))
                //{
                //    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);
                //    if (FKBedel != null)
                //    {
                //        teminat = new teminatSonucBilgisi()
                //        {
                //            Adet = 1,
                //            Bedel = FKBedel.Tedavi,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.TedaviMasraflari

                //        };
                //        teminatList.Add(teminat);

                //        teminat = new teminatSonucBilgisi()
                //        {
                //            Adet = 1,
                //            Bedel = FKBedel.Sakatlik,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.FerdiKazaOlum

                //        };
                //        teminatList.Add(teminat);
                //        teminat = new teminatSonucBilgisi()
                //        {
                //            Adet = 1,
                //            Bedel = FKBedel.Vefat,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.FerdiKaza

                //        };
                //        teminatList.Add(teminat);
                //    }
                //}

                //decimal hukukBedel = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, 0);


                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = hukukBedel,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.HukuksalKoruma

                //};
                //teminatList.Add(teminat);

                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.Asistans

                //};
                //teminatList.Add(teminat);

                //#endregion

                //#region Seçimli Default
                //bool ManeviDahil = false;
                //decimal IMMSoru = 0;
                //decimal IMMKombineSoru = 0;
                //string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                //KaskoIMM CR_IMMBedel = new KaskoIMM();
                //CR_KaskoIMM CRIMMKombineBedel = new CR_KaskoIMM();
                //if (!String.IsNullOrEmpty(immKademe))
                //{
                //    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                //    if (IMMBedel != null)
                //    {
                //        //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                //        CR_IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                //        if (CR_IMMBedel.Text.Contains("Manevi Dahil")) ManeviDahil = true;

                //        if (CR_IMMBedel != null)
                //        {
                //            //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                //            CRIMMKombineBedel = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.AXA, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                //        }
                //        if (CRIMMKombineBedel != null)
                //        {
                //            IMMSoru = CRIMMKombineBedel.BedeniSahis.Value;
                //            IMMKombineSoru = CRIMMKombineBedel.Kombine.Value;
                //        }

                //        if (IMMSoru > 0)
                //        {
                //            teminat = new CTeminat()
                //            {
                //                Adet = 1,
                //                Bedel = IMMSoru,
                //                Iptal = false,
                //                TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                //            };
                //            teminatList.Add(teminat);

                //        }
                //        if (IMMKombineSoru > 0)
                //        {
                //            teminat = new CTeminat()
                //            {
                //                Adet = 1,
                //                Bedel = IMMKombineSoru,
                //                Iptal = false,
                //                TeminatKod = AXA_KaskoTeminatlar.IMSKombine

                //            };
                //            teminatList.Add(teminat);
                //        }

                //    }
                //}
                //else
                //{
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = 0,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                //    };
                //    teminatList.Add(teminat);
                //}

                //#endregion

                //#region Seçimli
                //var aracaBagliKaravan = teklif.ReadSoru(KaskoSorular.AxaAracaBagliKaravanVarMi, false);
                //if (aracaBagliKaravan)
                //{
                //    var aracaBagliKaravanBedel = teklif.ReadSoru(KaskoSorular.AxaAracaBagliKaravanBedeli, "0");
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = Convert.ToDecimal(aracaBagliKaravanBedel),
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.AracaBagliKaravan
                //    };
                //}
                //teminatList.Add(teminat);

                //var kullanimGelirKaybi = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiVarMi, false);
                //if (kullanimGelirKaybi)
                //{
                //    var kullanimGelirKaybiBedel = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiBedel, "0");
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = Convert.ToDecimal(kullanimGelirKaybiBedel),
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.KullanimGelirKaybi
                //    };
                //}
                //teminatList.Add(teminat);

                //var elektrikliArac = teklif.ReadSoru(KaskoSorular.ElektrikliArac, false);
                //if (elektrikliArac)
                //{
                //    var elektrikliAracBedel = teklif.ReadSoru(KaskoSorular.ElektrikliAracBedeli, "0");
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = Convert.ToDecimal(elektrikliAracBedel),
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.ElektrikliAracBedeli
                //    };
                //}


                //decimal? KasaBedeli = 0;
                //decimal digerAksesuarBedel = 0;
                //#region Araç aksesuarlar
                //if (teklif.AracEkSorular.Count > 0)
                //{
                //    #region Aksesuarlar
                //    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR ||
                //                                                                         w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                //                                             .ToList<TeklifAracEkSoru>();

                //    if (aksesuarlar.Count > 0)
                //    {
                //        foreach (TeklifAracEkSoru item in aksesuarlar)
                //        {
                //            if (item.SoruKodu == MapfreAksesuarTipleri.Kasa)
                //            {
                //                KasaBedeli = item.Bedel;
                //            }
                //            else
                //            {
                //                digerAksesuarBedel += item.Bedel.HasValue ? item.Bedel.Value : 0;
                //            }
                //        }
                //    }
                //    #endregion

                //    #region Elektronik Cihaz Listesi
                //    //List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                //    //                                                          .ToList<TeklifAracEkSoru>();
                //    //if (elekCihazlar.Count > 0)
                //    //{
                //    //    foreach (TeklifAracEkSoru item in elekCihazlar)
                //    //    {
                //    //        //element.listElements = new mapfre.SahaWS[3];
                //    //        //element.listElements[0] = new mapfre.SahaWS() { p_cod_campo = "COD_ELEKTRONIK_CIHAZ", p_val_campo = item.SoruKodu };
                //    //        //element.listElements[1] = new mapfre.SahaWS() { p_cod_campo = "TXT_ELEKTRONIK_CIHAZ_ACIKLAMA", p_val_campo = item.Aciklama };
                //    //        //element.listElements[2] = new mapfre.SahaWS() { p_cod_campo = "VAL_ELEKTRONIK_CIHAZ_BEDEL", p_val_campo = Convert.ToString(item.Bedel) };

                //    //    }
                //    //}
                //    #endregion
                //}
                //#endregion

                //if (KasaBedeli != 0)
                //{
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = KasaBedeli,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.KasaTank,
                //    };
                //    teminatList.Add(teminat);
                //}
                //if (digerAksesuarBedel != 0)
                //{
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = digerAksesuarBedel,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.DigerAksesuarlar,
                //    };
                //    teminatList.Add(teminat);
                //}

                ////teminat = new CTeminat()
                ////{
                ////    Adet = 1,
                ////    Bedel = 0,
                ////    Iptal = false,
                ////    TeminatKod = AXA_KaskoTeminatlar.TasinanYuk

                ////};
                ////teminatList.Add(teminat);

                //var yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                //if (yurtDisiKasko)
                //{
                //    var yurtDisiSuresi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "");
                //    if (!string.IsNullOrEmpty(yurtDisiSuresi))
                //    {
                //        teminat = new CTeminat()
                //        {
                //            Adet = 1,
                //            Bedel = 0,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.YurtDisiKasko

                //        };
                //        teminatList.Add(teminat);
                //    }
                //}

                //string HayatLimiti = teklif.ReadSoru(KaskoSorular.AxaHayatTeminatLimiti, "");
                //if (!String.IsNullOrEmpty(HayatLimiti))
                //{
                //    decimal bedel = 0;

                //    if (HayatLimiti == "1")
                //    {
                //        bedel = AxaHayatLimiti.BesBin;
                //    }
                //    else
                //    {
                //        bedel = AxaHayatLimiti.YediBinBesYuz;
                //    }
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = bedel,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.HayatTeminati

                //    };
                //    teminatList.Add(teminat);
                //}

                //string AsistansHizmeti = teklif.ReadSoru(KaskoSorular.AxaAsistansHizmeti, "T");
                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.Asistans

                //};
                //teminatList.Add(teminat);
                //#endregion

                //policeDetay.Teminatlar = teminatList.ToArray();

                //#endregion

                #region İstatistikler

                string AKKullanimTarzi = String.Empty;
                string AKKullanimSekli = String.Empty;

                if (parts.Length == 2)
                {
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                                  f.KullanimTarziKodu == kullanimT &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        string[] part = kullanimTarzi.TarifeKodu.Split('-');
                        if (part.Length > 0)
                        {
                            AKKullanimTarzi = part[0];
                            AKKullanimSekli = part[1];
                        }
                    }
                }

                //List<CSoru> soruList = new List<CSoru>();

                //CSoru soru = new CSoru()
                //{
                //    CevapKod = AXAKullanimTarzi,
                //    SoruKod = AXA_KaskoSoruKodlari.KullanimTarzi

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = AXAKullanimSekli,
                //    SoruKod = AXA_KaskoSoruKodlari.KullanimSekli

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.Marka,
                //    SoruKod = AXA_KaskoSoruKodlari.Marka

                //};
                //soruList.Add(soru);


                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.AracinTipi.PadLeft(3, '0'),
                //    SoruKod = AXA_KaskoSoruKodlari.MarkaTip

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.Model.ToString().Substring(2, 2),
                //    SoruKod = AXA_KaskoSoruKodlari.Model

                //};
                //soruList.Add(soru);
                //bool yeniKayitMi = teklif.ReadSoru(KaskoSorular.AxaPlakaYeniKayitMi, false);
                //soru = new CSoru()
                //{
                //    CevapKod = yeniKayitMi ? AXA_KayitTipleri.Evet : AXA_KayitTipleri.Hayir,
                //    SoruKod = AXA_KaskoSoruKodlari.PlakaYeniKayitMi

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.PlakaKodu,
                //    SoruKod = AXA_KaskoSoruKodlari.PlakaIlKodu

                //};
                //soruList.Add(soru);

                string cevapkod = "";
                if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    switch (teklif.GenelBilgiler.TaksitSayisi.Value)
                    {
                        case 2: cevapkod = AK_OdemeSekilleri.Iki; break;
                        case 3: cevapkod = AK_OdemeSekilleri.Uc; break;
                        case 4: cevapkod = AK_OdemeSekilleri.Dort; break;
                        case 5: cevapkod = AK_OdemeSekilleri.Bes; break;
                        case 6: cevapkod = AK_OdemeSekilleri.Alti; break;
                        case 7: cevapkod = AK_OdemeSekilleri.Yedi; break;
                        default:
                            cevapkod = AK_OdemeSekilleri.Pesin;
                            break;
                    }
                }
                else
                {
                    cevapkod = AK_OdemeSekilleri.Pesin;
                }
                //soru = new CSoru()
                //{
                //    CevapKod = cevapkod,
                //    SoruKod = AK_KaskoSoruKodlari.OdemeSekli

                //};
                //soruList.Add(soru);

                if (AKKullanimTarzi == "1")
                {
                    //string ikameTuru = teklif.ReadSoru(KaskoSorular.AxaIkameSecimi, String.Empty);
                    //if (!String.IsNullOrEmpty(ikameTuru))
                    //{
                    //    //Sadece Husisi Araçlar için
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = ikameTuru,
                    //        SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                    //    };
                    //    soruList.Add(soru);
                    //}
                    //if (teklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false))
                    //{
                    //    string ikame = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                    //    switch (ikame)
                    //    {
                    //        case "ABC07":
                    //            {
                    //                soru = new CSoru()
                    //                {
                    //                    CevapKod = "7", //7GÜN
                    //                    SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                    //                };
                    //                soruList.Add(soru);
                    //            }
                    //            break;
                    //        case "ABC14":
                    //            {
                    //                soru = new CSoru()
                    //                {
                    //                    CevapKod = "15", //15GÜN
                    //                    SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                    //                };
                    //                soruList.Add(soru);
                    //            }
                    //            break;
                    //    }
                    //}

                    //}
                    //if (CRIMMKombineBedel != null && CRIMMKombineBedel.BedeniSahis > 0)
                    //{
                    //    string[] part = CRIMMKombineBedel.Kademe.Split('-');
                    //    string kademe = part[0];
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = kademe,
                    //        SoruKod = AXA_KaskoSoruKodlari.IMS3lü
                    //    };
                    //    soruList.Add(soru);
                    //}

                    //if (CRIMMKombineBedel != null && CRIMMKombineBedel.Kombine > 0)
                    //{
                    //    string[] part = CRIMMKombineBedel.Kademe.Split('-');
                    //    string kademe = part[0];
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = kademe,
                    //        SoruKod = AXA_KaskoSoruKodlari.IMSKombine

                    //    };
                    //    soruList.Add(soru);
                    //}
                    //if (KasaBedeli != 0)
                    //{
                    //    soru = new CSoru()  /////???????
                    //    {
                    //        CevapKod = "2", //1Sonradan Monte 2 Orjinal
                    //        SoruKod = AXA_KaskoSoruKodlari.KasaTank

                    //    };
                    //    soruList.Add(soru);
                    //}
                    ////ekranda olmayan sorular 
                    //if (teklif.Arac.Marka == "053") //Frod Araçlarda zorunlu istiyor. Diğer markalarda zorunlu değil
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = "5",
                    //        SoruKod = AXA_KaskoSoruKodlari.MarkaKasko

                    //    };
                    //    soruList.Add(soru);
                    //}

                    //soru = new CSoru()
                    //{
                    //    CevapKod = "",
                    //    SoruKod = AXA_KaskoSoruKodlari.DainiMurtein

                    //};
                    //soru = new CSoru()
                    //{
                    //    CevapKod = "",
                    //    SoruKod = AXA_KaskoSoruKodlari.SigortaliListesi

                    //};               

                    //if (digerAksesuarBedel != 0)
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = "1",
                    //        SoruKod = AXA_KaskoSoruKodlari.DigerAksesuar
                    //    };
                    //    soruList.Add(soru);
                    //}


                    //soru = new CSoru()
                    //{
                    //    CevapKod = "",
                    //    SoruKod = AXA_KaskoSoruKodlari.EmtiaCinsi

                    //};
                    //var yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko,false);
                    //if (yurtDisiKasko)
                    //{
                    //    soru = new CSoru()    /////???????
                    //    {
                    //        CevapKod = "1",
                    //        SoruKod = AXA_KaskoSoruKodlari.YurtDisiSuresi
                    //    };
                    //    soruList.Add(soru);
                    //}

                    //if (ManeviDahil)
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = "1",//Evet
                    //        SoruKod = AXA_KaskoSoruKodlari.ManeviTazminat

                    //    };
                    //    soruList.Add(soru);
                    //}
                    //else
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = "0",//Hayır
                    //        SoruKod = AXA_KaskoSoruKodlari.ManeviTazminat

                    //    };
                    //    soruList.Add(soru);
                    //}


                    //soru = new CSoru()
                    //{
                    //    CevapKod = AsistansHizmeti,
                    //    SoruKod = AXA_KaskoSoruKodlari.AsistansSecimi

                    //};
                    //soruList.Add(soru);

                    //if (!String.IsNullOrEmpty(HayatLimiti))
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = HayatLimiti,
                    //        SoruKod = AXA_KaskoSoruKodlari.HayatTeminatLimiti //Sadece Şahıs Müşterileri için verilebilir.
                    //    };
                    //    soruList.Add(soru);

                    //}

                    //if (teklif.Arac.Model >= TurkeyDateTime.Now.Year - 1)
                    //{

                    //    //Araç 1 Yaş Soruları
                    //    var OnarimSecimi = teklif.ReadSoru(KaskoSorular.AxaOnarimSecimi, "");
                    //    if (OnarimSecimi != "")
                    //    {
                    //        soru = new CSoru()
                    //        {
                    //            CevapKod = OnarimSecimi,
                    //            SoruKod = AXA_KaskoSoruKodlari.OnarimSecimi
                    //        };
                    //        soruList.Add(soru);
                    //    }

                    //    var PlakaYeniKayitMi = teklif.ReadSoru(KaskoSorular.AxaPlakaYeniKayitMi, false);
                    //    string PlakaYeniMi = "2";
                    //    if (PlakaYeniKayitMi)
                    //    {
                    //        PlakaYeniMi = "1";
                    //    }
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = PlakaYeniMi,
                    //        SoruKod = AXA_KaskoSoruKodlari.PlakaYeniKayitMi
                    //    };
                    //    soruList.Add(soru);
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = teklif.Arac.PlakaKodu,
                    //        SoruKod = AXA_KaskoSoruKodlari.PlakaIlKodu
                    //    };
                    //    soruList.Add(soru);
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = teklif.ReadSoru(KaskoSorular.AxaYeniDegerKlozu, ""),
                    //        SoruKod = AXA_KaskoSoruKodlari.YeniDegerKlozu
                    //    };
                    //    soruList.Add(soru);
                    //    string depremSel = teklif.ReadSoru(KaskoSorular.AxaDepremSelKoasuransi, "");
                    //    if (depremSel != "")
                    //    {
                    //        soru = new CSoru()
                    //        {
                    //            CevapKod = depremSel,
                    //            SoruKod = AXA_KaskoSoruKodlari.DepremSelKoasuransi
                    //        };
                    //    }

                    //    soruList.Add(soru);
                    //    string AxaMuafiyet = teklif.ReadSoru(KaskoSorular.AxaMuafiyetTutari, "");
                    //    if (AxaMuafiyet != "")
                    //    {
                    //        soru = new CSoru()
                    //        {
                    //            CevapKod = AxaMuafiyet,
                    //            SoruKod = AXA_KaskoSoruKodlari.MuafiyetTutari
                    //        };
                    //        soruList.Add(soru);
                    //    }


                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = teklif.ReadSoru(KaskoSorular.AxaSorumlulukLimiti, ""),
                    //        SoruKod = AXA_KaskoSoruKodlari.SorumlulukLimiti
                    //    };
                    //    soruList.Add(soru);
                    //    bool engelliAracimi = teklif.ReadSoru(KaskoSorular.EngelliAracimi, false);
                    //    string engelliAraci = "0";
                    //    if (engelliAracimi)
                    //    {
                    //        engelliAraci = "1";
                    //    }
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = engelliAraci,
                    //        SoruKod = AXA_KaskoSoruKodlari.EngelliAraciMi

                    //    };
                    //    soruList.Add(soru);

                    //    var CamFilmiLogoVarMi = teklif.ReadSoru(KaskoSorular.AxaCamFilmiLogo, false);
                    //    string CamFilmiLogo = "0";
                    //    if (CamFilmiLogoVarMi)
                    //    {
                    //        CamFilmiLogo = "1";
                    //    }
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = CamFilmiLogo,
                    //        SoruKod = AXA_KaskoSoruKodlari.CamFilmiLogoVarMi
                    //    };
                    //    soruList.Add(soru);
                    //}
                    //if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                    //{
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = "28", //BÜRO VE YAZIHANELER
                    //        SoruKod = AXA_KaskoSoruKodlari.FaliyetKodu
                    //    };
                    //    soruList.Add(soru);
                    //}

                    //policeDetay.Sorular = soruList.ToArray();

                    //#endregion

                    //#region  Matbular

                    //CMatbuBilgi matbu = new CMatbuBilgi();
                    //matbu.BilgiAdi = "MOTOR NO";
                    //matbu.Aciklama = teklif.Arac.MotorNo;
                    //matBuList.Add(matbu);

                    //matbu = new CMatbuBilgi();
                    //matbu.BilgiAdi = "ŞASİ NO";
                    //matbu.Aciklama = teklif.Arac.SasiNo;
                    //matBuList.Add(matbu);

                    //matbu = new CMatbuBilgi();
                    //matbu.BilgiAdi = "PLAKA NO";
                    //matbu.Aciklama = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                    //matBuList.Add(matbu);

                    //matbu = new CMatbuBilgi();
                    //matbu.BilgiAdi = "MARKA TİPİ";
                    //matbu.Aciklama = teklif.Arac.Marka;
                    //matBuList.Add(matbu);


                    //matbu = new CMatbuBilgi();
                    //matbu.BilgiAdi = "Ruhsat Belge/ASBIS Numarası";
                    //if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                    //{
                    //    matbu.Aciklama = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo;
                    //}
                    //else
                    //{
                    //    matbu.Aciklama = teklif.Arac.AsbisNo;
                    //}
                    //matBuList.Add(matbu);


                    //if (elektrikliArac)
                    //{
                    //    var elektrikliAracPil = teklif.ReadSoru(KaskoSorular.ElektrikliAracPilId, "0");
                    //    matbu = new CMatbuBilgi();
                    //    matbu.BilgiAdi = "Pil ID No:";
                    //    matbu.Aciklama = elektrikliAracPil;
                    //    matBuList.Add(matbu);
                    //}
                    //policeBaslik.MatbuBilgiler = matBuList.ToArray();

                    //#endregion

                    //#endregion

                    //#endregion

                    #region Servis
                    Request req = new Request();
                    req.kaskoTeklifOlusturmaInput = kaskoTeklifOlusturmaInput;


                    this.BeginLog(req, req.GetType(), WebServisIstekTipleri.Teklif);
                    var response = client.kaskoTeklifOlustur(kaskoTeklifOlusturmaInput);
                    client.Dispose();


                    if (response.errorCode != null)
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.errorMessage.ToString());

                    }
                    else
                    {
                        this.EndLog(response, true, response.GetType());
                        KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                        string testMi = konfigTestMi[Konfig.TestMi];
                        if (!Convert.ToBoolean(testMi))
                        {


                            //#region PDF URL                      
                            //                        AXAPoliceBasim.AxaPoliceBasim pdfClient = new AXAPoliceBasim.AxaPoliceBasim();
                            //                            pdfClient.Url = konfig[Konfig.AXA_PoliceBasim];
                            //                            pdfClient.Timeout = 150000;
                            //                            pdfClient.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);

                            //                            AOCPoliceBasimParametre pdfReq = new AOCPoliceBasimParametre()
                            //                            {
                            //                                BasimTipi = BasimType.Binary,
                            //                                DilSecimi = BasimDil.Turkce,
                            //                                Kullanici = servisKullanici.KullaniciAdi2,
                            //                                PoliceNo = response.TeklifNumarasi.Value,
                            //                                ZeylSiraNo = 0
                            //                            };

                            //                            var responsePDF = pdfClient.PoliceBasim(pdfReq);
                            //                            pdfClient.Dispose();

                            //                            if (responsePDF.HataMesajlari.ToList().Count > 0)
                            //                            {
                            //                                //this.EndLog(responsePDF, false, responsePDF.GetType());

                            //                                //for (int i = 0; i < responsePDF.HataMesajlari.Count(); i++)
                            //                                //{
                            //                                //    this.AddHata(responsePDF.HataMesajlari[i].ToString());
                            //                                //}

                            //                                //this.AddHata("PDF dosyası alınamadı.");
                            //                            }
                            //                            else
                            //                            {
                            //                                this.EndLog(responsePDF, true, responsePDF.GetType());
                            //                                byte[] data = responsePDF.Binary;

                            //                                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                            //                                string fileName = String.Empty;
                            //                                fileName = String.Format("AXA_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                            //                                pdfUrl = storage.UploadFile("kasko", fileName, data);

                            //                                _Log.Info("Teklif_PDF url: {0}", pdfUrl);
                            //                            }
                            //                            #endregion

                        }
                    }

                    #endregion

                    #region TeklifKaydet

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
                    this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                    this.GenelBilgiler.BrutPrim = response.primBilgileri[0].prim;
                    //this.GenelBilgiler.TUMTeklifNo = response.TeklifNumarasi.ToString();
                    this.GenelBilgiler.PDFDosyasi = pdfUrl;


                    #region Vergiler
                    //decimal BSV = 0;
                    //decimal GH = 0;
                    //decimal THG = 0;
                    //foreach (var item in response.)
                    //{
                    //    if (item.VergiKod == "BSV")
                    //    {
                    //        BSV = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    //    }
                    //    else if (item.VergiKod == "GH")
                    //    {
                    //        GH = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    //    }
                    //    else if (item.VergiKod == "THG")
                    //    {
                    //        THG = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    //    }
                    //}

                    //this.AddVergi(TrafikVergiler.THGFonu, THG);
                    //this.AddVergi(TrafikVergiler.GiderVergisi, BSV);
                    //this.AddVergi(TrafikVergiler.GarantiFonu, GH);

                    //this.GenelBilgiler.ToplamVergi = BSV + GH + THG;
                    #endregion

                    this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                    this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                    //this.GenelBilgiler.GecikmeZammiYuzdesi = ANADOLUMessage.ToDecimal(response.GECIKME_SURPRIM_ORANI) * 100;
                    //this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                    //this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                    //Odeme Bilgileri
                    this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                    this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                    this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                    //if (response.Sorular != null)
                    //{
                    //    foreach (var item in response.Sorular)
                    //    {
                    //        if (item.SoruKod == AXA_KaskoSoruKodlari.HasarsizlikIndirimi)
                    //        {
                    //            if (item.CevapKod != "1")
                    //            {
                    //                string hasarsizlikIndirimYuzdesi = String.Empty;
                    //                foreach (char ch in item.CevapAd)
                    //                {
                    //                    if (Char.IsDigit(Convert.ToChar(ch)))
                    //                    {
                    //                        hasarsizlikIndirimYuzdesi += ch;
                    //                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(hasarsizlikIndirimYuzdesi);
                    //                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                    //                    }
                    //                }
                    //            }
                    //            else
                    //            {
                    //                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                    //                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                    //            }
                    //        }
                    //    }
                    //}

                    //#region Taksitler

                    //if (response.Taksitler.Count() > 0)
                    //{
                    //    this.GenelBilgiler.ToplamKomisyon = response.Taksitler.FirstOrDefault().Komisyon;
                    //}
                    //if (response.Taksitler.Count() == 1)
                    //{
                    //    this.GenelBilgiler.TaksitSayisi = 1;
                    //}
                    //else
                    //{
                    //    this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                    //}
                    //#endregion

                    //#endregion

                    #region Teminatlar



                    if (response.teminatBilgileri.Count() > 0)
                    {
                        foreach (var item in response.teminatBilgileri)
                        {

                            if (item.teminatKodu == Ak_KaskoTeminatlar.KASKO)
                            {
                                this.AddTeminat(KaskoTeminatlar.Kasko, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.GLKHH_TEROR)
                            {
                                this.AddTeminat(KaskoTeminatlar.GLKHHT, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }

                            else if (item.teminatKodu == Ak_KaskoTeminatlar.DOGAL_AFET)
                            {
                                this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.DEPREM)
                            {
                                this.AddTeminat(KaskoTeminatlar.Deprem, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.SEL_VE_SU_BASKINI)
                            {
                                this.AddTeminat(KaskoTeminatlar.SelSuBaskini, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.YANGIN)
                            {
                                this.AddTeminat(KaskoTeminatlar.Arac_Yanmasi, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.HUKUKSAL_KORUMA_ARAC)
                            {
                                this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.HUKUKSAL_KORUMA_SURUCU)
                            {
                                this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.FERDI_KAZA)
                            {
                                this.AddTeminat(KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.KAZAEN_OLUM)
                            {
                                this.AddTeminat(KaskoTeminatlar.KFK_Olum, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.KAZAEN_SUREKLI_SAKATLIK)
                            {
                                this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.TEDAVI_MASRAFLARI)
                            {
                                this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.AKSIGORTA_YARDIM)
                            {
                                this.AddTeminat(KaskoTeminatlar.Saglik, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.MINI_HASAR_ONARIM)
                            {
                                this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }
                            else if (item.teminatKodu == Ak_KaskoTeminatlar.ACIL_TIBBI_YARDIM)
                            {
                                this.AddTeminat(KaskoTeminatlar.Medline, Convert.ToDecimal(item.bedel), item.vergiTutari, item.netPrim, item.brutPrim, 0);
                            }

                        }
                    }
                    #endregion

                    #region Ödeme Planı

                    if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                    {
                        this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    }
                    #endregion
                    #region Web Servis Cevaplar


                    string AxaUyariMesaji = "";
                    if (response.errorCode != null)
                    {

                        AxaUyariMesaji = response.errorMessage;

                        if (AxaUyariMesaji.Length <= 1000)
                        {
                            this.AddWebServisCevap(Common.WebServisCevaplar.TeklifUyariMesaji, AxaUyariMesaji);
                        }
                        else
                        {
                            this.AddWebServisCevap(Common.WebServisCevaplar.TeklifUyariMesaji, AxaUyariMesaji.Substring(0, 999));
                        }
                    }
                    string AxaBilgiMesaji = "";
                    //if (response.BilgiMesajlari != null)
                    //{
                    //    for (int i = 0; i < response.BilgiMesajlari.Count(); i++)
                    //    {
                    //        AxaBilgiMesaji += response.BilgiMesajlari[i];
                    //    }
                    //    if (AxaBilgiMesaji.Length <= 1000)
                    //    {
                    //        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AxaBilgiMesaji);
                    //    }
                    //    else
                    //    {
                    //        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AxaBilgiMesaji.Substring(0, 999));
                    //    }
                    //}

                    #endregion
                    #endregion

                    #endregion


                    #endregion
                    #endregion

                    //    #region Genel Bigiler

                    //    ak.kasko.ProposalParameters cascoparameters = new ak.kasko.ProposalParameters();

                    //    //Plaka Bilgileri
                    //    string rakam = "";
                    //    string harf = "";
                    //    foreach (char ch in teklif.Arac.PlakaNo)
                    //    {
                    //        if (Char.IsDigit(ch))
                    //            rakam += ch;
                    //        if (Char.IsLetter(ch))
                    //            harf += ch;
                    //    }
                    //    cascoparameters.PlateState = teklif.Arac.PlakaKodu;
                    //    cascoparameters.PlateChar = harf;
                    //    cascoparameters.PlateNumber = rakam;
                    //    cascoparameters.TypeCode = 2;// Muafiyetsiz=1  Muafiyetli=2

                    //    #endregion

                    //    #region Sorular

                    //    cascoparameters.Parameters = this.TeklifSorular(teklif, clntKasko, servisKullanici);

                    //    #endregion

                    //    #region Sigortali / Sigorta Ettiren Bilgileri

                    //    cascoparameters.Insured = this.SigortaEttirenBilgileri(teklif, clntKasko);
                    //    cascoparameters.Customer = this.SigortaliBilgileri(teklif, clntKasko);

                    //    #endregion

                    //    #region Service call

                    //    this.BeginLog(cascoparameters, cascoparameters.GetType(), WebServisIstekTipleri.Teklif);

                    //    ak.kasko.MethodResultOfProposalOutput teklifResult = clntKasko.GetProposal(cascoparameters);
                    //    clntKasko.Dispose();
                    //    if (teklifResult.Result == null && teklifResult.ResultCode == ak.kasko.ProposalResultCodes.Fail)
                    //    {
                    //        if (!String.IsNullOrEmpty(teklifResult.Description))
                    //        {
                    //            this.EndLog(teklifResult, true, teklifResult.GetType());
                    //            this.AddHata(teklifResult.Description);
                    //        }
                    //        else
                    //        {
                    //            this.EndLog(teklifResult, true, teklifResult.GetType());
                    //            this.AddHata("Bu ip işlem yapmaya yetkili değildir.");
                    //        }
                    //    }
                    //    else if (teklifResult.ResultCode == ak.kasko.ProposalResultCodes.Success && teklifResult.Result == null)
                    //    {
                    //        this.EndLog(teklifResult, true, teklifResult.GetType());
                    //        this.AddHata("Web Servisten yanıt alınamadı.");
                    //    }
                    //    else if (teklifResult.ResultCode == ak.kasko.ProposalResultCodes.Success && teklifResult.Result != null && teklifResult.Result.RESULT != null)
                    //    {
                    //        if (teklifResult.Result.RESULT.ERROR != null)
                    //        {
                    //            this.EndLog(teklifResult, true, teklifResult.GetType());

                    //            string hata = String.Empty;
                    //            var list = teklifResult.Result.RESULT.NOTIFICATION_LIST;
                    //            if (list != null)
                    //            {
                    //                foreach (var item in list)
                    //                {
                    //                    hata += item.NOTIFICATION_DESCRIPTION;
                    //                }
                    //            }
                    //            this.AddHata(teklifResult.Result.RESULT.ERROR.ERROR_DESCRIPTION + hata);
                    //        }
                    //        else
                    //        {
                    //            this.EndLog(teklifResult, true, teklifResult.GetType());
                    //        }
                    //    }
                    //    else
                    //    {
                    //        this.EndLog(teklifResult, true, teklifResult.GetType());
                    //    }
                    //    #endregion
                    //    #endregion

                    //    #region Eski ws

                    //    //#region Veri Hazırlama

                    //    //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);

                    //    //sompojapankasko.Casco clnt = new sompojapankasko.Casco();
                    //    //clnt.Url = konfig[Konfig.SOMPOJAPAN_CascoServiceURL];
                    //    //clnt.Timeout = 150000;

                    //    //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

                    //    //#endregion

                    //    //#region Genel Bigiler

                    //    //CascoParameters cascoparameters = new CascoParameters();

                    //    //cascoparameters.Username = servisKullanici.KullaniciAdi;
                    //    //cascoparameters.Password = servisKullanici.Sifre;
                    //    //cascoparameters.PlateState = teklif.Arac.PlakaKodu;

                    //    //string rakam = "";
                    //    //string harf = "";
                    //    //foreach (char ch in teklif.Arac.PlakaNo)
                    //    //{
                    //    //    if (Char.IsDigit(ch))
                    //    //        rakam += ch;
                    //    //    if (Char.IsLetter(ch))
                    //    //        harf += ch;
                    //    //}

                    //    //cascoparameters.PlateChar = harf;
                    //    //cascoparameters.PlateNumber = rakam;
                    //    //cascoparameters.TypeCode = 1;// Muafiyetsiz=1  Muafiyetli=2

                    //    //if (String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) && String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) && !String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                    //    //{
                    //    //    cascoparameters.EgmTescilBelgeSeriKod = "";
                    //    //    cascoparameters.EgmTescilBelgeSeriNo = teklif.Arac.AsbisNo;
                    //    //}
                    //    //else
                    //    //{
                    //    //    cascoparameters.EgmTescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                    //    //    cascoparameters.EgmTescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                    //    //}

                    //    //#endregion

                    //    //#region Sorular

                    //    //cascoparameters.Parameters = this.TeklifSorular(teklif, servisKullanici, clnt);

                    //    //#endregion

                    //    //#region Sigortali Bilgileri

                    //    //cascoparameters.Unit = this.SigortaliBilgileri(teklif, servisKullanici);

                    //    //#endregion

                    //    //#region Service call

                    //    //this.BeginLog(cascoparameters, cascoparameters.GetType(), WebServisIstekTipleri.Teklif);

                    //    //MethodResultOfCascoResult teklifResult = clnt.GetProposal3(cascoparameters);
                    //    //string nipponHata = teklifResult.Result.Result.resultcode.ToString();
                    //    //if (nipponHata == "Error" || nipponHata == "None")
                    //    //{
                    //    //    this.EndLog(teklifResult, true, teklifResult.GetType());
                    //    //    this.AddHata(teklifResult.Result.Result.description);
                    //    //}
                    //    //else
                    //    //{
                    //    //    this.EndLog(teklifResult, true, teklifResult.GetType());

                    //    //}
                    //    //#endregion
                    //    #endregion

                    //    #region Başarı kontrolu
                    //    if (!this.Basarili)
                    //    {
                    //        this.Import(teklif);
                    //        this.GenelBilgiler.Basarili = false;
                    //        return;
                    //    }
                    //    #endregion

                    //    #region Teklif kaydı

                    //    #region Genel bilgiler
                    //    this.Import(teklif);
                    //    this.GenelBilgiler.Basarili = true;
                    //    this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                    //    this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                    //    this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                    //    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(teklifResult.Result.PAYMENT.GROSS_PREMIUM);
                    //    this.GenelBilgiler.TUMTeklifNo = teklifResult.Result.PROPOSAL_NO.HasValue ? teklifResult.Result.PROPOSAL_NO.Value.ToString() : "";

                    //    #region Teklif PDF
                    //    ak.common.ExtendConfirmParameters confirmRequest = new ak.common.ExtendConfirmParameters();
                    //    ak.common.Common client = new ak.common.Common();
                    //    KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleAKCommonV2);
                    //    client.Url = konfigCommon[Konfig.AK_PoliceBasim]; ;
                    //    client.Timeout = 150000;

                    //    client.IdentityHeaderValue = new ak.common.IdentityHeader()
                    //    {
                    //        KullaniciAdi = servisKullanici.KullaniciAdi,
                    //        KullaniciParola = servisKullanici.Sifre,
                    //        KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    //        KullaniciTipi = ak.common.ClientType.ACENTE,
                    //    };
                    //    confirmRequest.Policy = new ak.common.Policy()
                    //    {
                    //        PolicyNumber = teklifResult.Result.PROPOSAL_NO.Value,
                    //        ProductName = ak.common.ProductName.BireyselKasko,
                    //        CompanyCode = 0,
                    //        EndorsNr = 0,
                    //        FirmCode = 0,
                    //        RenewalNr = 0,

                    //    };
                    //    string TeklifPDFUrl = this.GetPDFURL(client, confirmRequest, ak.common.PrintoutType.Policy);
                    //    if (!String.IsNullOrEmpty(TeklifPDFUrl))
                    //    {
                    //        if (TeklifPDFUrl != null)
                    //        {
                    //            WebClient myClient = new WebClient();
                    //            byte[] data = myClient.DownloadData(TeklifPDFUrl);

                    //            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    //            string fileName = String.Empty;
                    //            string url = String.Empty;
                    //            fileName = String.Format("AK_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                    //            url = storage.UploadFile("kasko", fileName, data);
                    //            this.GenelBilgiler.PDFDosyasi = url;
                    //            _Log.Info("Teklif_PDF url: {0}", url);
                    //        }
                    //        else
                    //        {
                    //            this.AddHata("PDF dosyası alınamadı.");
                    //        }
                    //    }
                    //    #endregion


                    //    var SompoVergiKomisyon = teklifResult.Result.PAYMENT.TAXES;
                    //    decimal giderVergi = 0;
                    //    decimal komisyon = 0;

                    //    foreach (var item in SompoVergiKomisyon)
                    //    {
                    //        if (item.DEDUCTION_DESCRIPTION.Trim() == "Vergiler")
                    //            giderVergi = item.DEDUCTION_AMOUNT;

                    //        if (item.DEDUCTION_DESCRIPTION.Trim() == "Komisyonyonlar")
                    //            komisyon = item.DEDUCTION_AMOUNT;
                    //    }

                    //    this.GenelBilgiler.ToplamVergi = giderVergi;
                    //    this.GenelBilgiler.NetPrim = Convert.ToDecimal(teklifResult.Result.PAYMENT.NET_PREMIUM);
                    //    this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;

                    //    if (teklifResult.Result.QUESTIONS != null && teklifResult.Result.QUESTIONS.Length > 0)
                    //    {
                    //        var hasarsizlikKademe = teklifResult.Result.QUESTIONS.FirstOrDefault(s => s.QUESTION_CODE == 224);
                    //        if (hasarsizlikKademe != null)
                    //        {
                    //            string HasarsizlikIndirim = hasarsizlikKademe.ANSWER.Trim();

                    //            if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                    //            { this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(HasarsizlikIndirim); }

                    //            if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                    //            {
                    //                if (HasarsizlikIndirim == "1")
                    //                {
                    //                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 30;
                    //                }
                    //                else if (HasarsizlikIndirim == "2")
                    //                {
                    //                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 40;
                    //                }
                    //                else if (HasarsizlikIndirim == "3")
                    //                {
                    //                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 50;
                    //                }
                    //                else if (HasarsizlikIndirim == "4")
                    //                {
                    //                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 60;
                    //                }
                    //                else
                    //                {
                    //                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                    //    this.GenelBilgiler.ToplamKomisyon = komisyon;

                    //    //Odeme Bilgileri
                    //    this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                    //    this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                    //    this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                    //    #endregion

                    //    #region Vergiler
                    //    this.AddVergi(KaskoVergiler.GiderVergisi, giderVergi);
                    //    #endregion

                    //    #region Teminatlar

                    //    var AkTeminatlar = teklifResult.Result.COVERS;
                    //    decimal teminatBedel = 0;
                    //    decimal teminatBrutPrim = 0;

                    //    if (AkTeminatlar != null)
                    //    {
                    //        for (int i = 0; i < AkTeminatlar.Count(); i++)
                    //        {
                    //            switch (AkTeminatlar[i].COVER_CODE)
                    //            {
                    //                case Ak_KaskoTeminatlar.Arac724Hizmet:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.CarpmaCarpilma:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.CarpmaCarpilma, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.Yanma:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Arac_Yanmasi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.Calinma:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Arac_Calinmasi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.GLKHHT:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.GLKHHT, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.SelBaskini:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Seylap, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.Deprem:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Deprem, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.AnahtarKaybiDiger:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.HayvanZarari:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Hayvanlarin_Verecegi_Zarar, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.SigaraBenzeri:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Sigara_Ve_Benzeri_Madde_Zararlari, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.CekmeCekilme:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.YetkisizCekilme, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.FK_Vefat:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.KFK_Olum, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.FK_SurekliSakat:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.HK_MotorluArac:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.HK_SurucuyeBagli:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.IMM_Sahis:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.IMM_Kombine:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.IMM_Kaza:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.IMM_Maddi:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //                case Ak_KaskoTeminatlar.HasarsizlikKoruma:
                    //                    {
                    //                        teminatBedel = AkTeminatlar[i].COVER_AMOUNT.Value;
                    //                        teminatBrutPrim = AkTeminatlar[i].GROSS_PREMIUM.Value;
                    //                        this.AddTeminat(KaskoTeminatlar.HasarsizlikKoruma, teminatBedel, 0, 0, teminatBrutPrim, 0);
                    //                    }
                    //                    break;
                    //            }

                    //        }

                    //        //foreach (var item in SompoJapanTeminatlar)
                    //        //{
                    //        //}
                    //    }

                    //    #endregion

                    //    //#region Ödeme Planı (Sompo Japan Sigortada Teklifler Peşin olarak hesaplanıyor. Poliçeleştirmede Taksit gönderiliyor.)

                    //    //var sompoTaksitler = teklifResult.Result.PAYMENT.INSTALLMENTS;
                    //    //if (sompoTaksitler == null || sompoTaksitler.Length == 0)
                    //    //{
                    //    //    this.GenelBilgiler.TaksitSayisi = 1;
                    //    //    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    //    //}
                    //    //else
                    //    //{
                    //    //    this.GenelBilgiler.TaksitSayisi = (byte)sompoTaksitler.Length;

                    //    //    foreach (var taksit in sompoTaksitler)
                    //    //    {
                    //    //        this.AddOdemePlani(taksit.INSTALLMENT_ORDER_NO, taksit.INSTALLMENT_DATE, taksit.INSTALLMENT_AMOUNT, OdemeTipleri.KrediKarti);
                    //    //    }
                    //    //}
                    //    //#endregion

                    //    #region WebServis Cevap
                    //    this.AddWebServisCevap(Common.WebServisCevaplar.AK_Teklif_Police_No, teklifResult.Result.PROPOSAL_NO.ToString());
                    //    string AkBilgiMesaji = "";
                    //    if (!String.IsNullOrEmpty(teklifResult.Description))
                    //    {
                    //        AkBilgiMesaji = teklifResult.Description;
                    //        if (AkBilgiMesaji.Length <= 1000)
                    //        {
                    //            this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AkBilgiMesaji);
                    //        }
                    //        else
                    //        {
                    //            this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AkBilgiMesaji.Substring(0, 999));
                    //        }
                    //    }
                    //    //this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Kasko_Session_No, teklifResult.Result..ToString());
                    //    #endregion

                    //    #endregion

                    //}
                    //catch (Exception ex)
                    //{
                    //    #region Hata Log
                    //    clntKasko.Abort();
                    //    this.Import(teklif);
                    //    this.GenelBilgiler.Basarili = false;

                    //    this.EndLog(ex.Message, false);
                    //    this.AddHata(ex.Message);
                    //    #endregion
                    //}

                }

                //public override void Policelestir(Odeme odeme)
                //{
                //    ak.common.Common client = new ak.common.Common();
                //    try
                //    {
                //        #region Yeni WS

                //        #region Veri Hazırlama GENEL
                //        KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleAKCommonV2);
                //        ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                //        var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AK });
                //        MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //        MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                //        // Cryptography sifrele = new Cryptography();
                //        Crypto crypto = new Crypto();

                //        client.Url = konfigCommon[Konfig.AK_CommonServiceURLV2]; ;
                //        client.Timeout = 150000;

                //        ak.common.ExtendConfirmParameters confirmRequest = new ak.common.ExtendConfirmParameters();

                //        client.IdentityHeaderValue = new ak.common.IdentityHeader()
                //        {
                //            KullaniciAdi = servisKullanici.KullaniciAdi,
                //            KullaniciParola = servisKullanici.Sifre,
                //            KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                //            KullaniciTipi = ak.common.ClientType.ACENTE,
                //        };

                //        #endregion

                //        #region Genel Bilgiler

                //        confirmRequest.Policy = this.GetPolicy(teklif);
                //        confirmRequest.Unit = this.SigortaEttirenBilgileriCommon(teklif, servisKullanici);

                //        List<ak.common.CustomParameter> parameters = new List<ak.common.CustomParameter>();
                //        ak.common.CustomParameter parametre = new ak.common.CustomParameter();
                //        parametre.code = "GSM";
                //        parametre.value = telefon.Numara.Substring(3, 3) + telefon.Numara.Substring(7, 7);
                //        parameters.Add(parametre);

                //        confirmRequest.Parameters = parameters.ToArray();

                //        this.BeginLog(confirmRequest, confirmRequest.GetType(), WebServisIstekTipleri.Police);

                //        //Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                //        string cardHolderName = String.Empty;
                //        string cardNumber = String.Empty;
                //        string month = String.Empty;
                //        string year = String.Empty;
                //        string cvv = String.Empty;
                //        int taksitSayisi = odeme.TaksitSayisi;
                //        if (odeme.KrediKarti != null && (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti))
                //        {
                //            cardHolderName = crypto.Encript(odeme.KrediKarti.KartSahibi);
                //            cardNumber = crypto.Encript(odeme.KrediKarti.KartNo);
                //            month = crypto.Encript(odeme.KrediKarti.SKA);
                //            year = crypto.Encript(odeme.KrediKarti.SKY);
                //            cvv = crypto.Encript(odeme.KrediKarti.CVC);
                //        }

                //        confirmRequest.PaymentInput = GetPaymentInfo(odeme);
                //        #endregion

                //        #region Service Call

                //        ak.common.ConfirmOutput response = client.Onay(confirmRequest);
                //        client.Dispose();
                //        #endregion

                //        #region Hata Kontrol ve Kayıt

                //        if (!String.IsNullOrEmpty(response.RESULT.ERROR.ERROR_DESCRIPTION))
                //        {
                //            this.EndLog(response, false, response.GetType());
                //            this.AddHata(response.RESULT.ERROR.ERROR_DESCRIPTION);
                //        }
                //        else
                //        {
                //            long? sompoPoliceNo = response.POLICY_NUMBER.Value;
                //            long sompoMessageNo = Convert.ToInt64(ReadSoru(Common.WebServisCevaplar.AK_Trafik_Session_No, "0"));
                //            this.GenelBilgiler.TUMPoliceNo = sompoPoliceNo.Value.ToString();
                //            this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                //            confirmRequest.Policy = new ak.common.Policy();
                //            confirmRequest.Policy = this.GetPolicyPDF(teklif);

                //            #region Poliçe PDF
                //            string PolicePDFUrl = this.GetPDFURL(client, confirmRequest, ak.common.PrintoutType.Policy);
                //            if (!String.IsNullOrEmpty(PolicePDFUrl))
                //            {
                //                if (PolicePDFUrl != null)
                //                {
                //                    WebClient myClient = new WebClient();
                //                    byte[] data = myClient.DownloadData(PolicePDFUrl);

                //                    IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                //                    string fileName = String.Empty;
                //                    string url = String.Empty;
                //                    fileName = String.Format("AK_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                //                    url = storage.UploadFile("kasko", fileName, data);
                //                    this.GenelBilgiler.PDFPolice = url;
                //                    _Log.Info("Police_PDF url: {0}", url);
                //                }
                //                else
                //                {
                //                    this.AddHata("PDF dosyası alınamadı.");
                //                }
                //            }
                //            #endregion

                //            #region Bilgilendirme PDF
                //            string BilgilendirmeURL = this.GetPDFURL(client, confirmRequest, ak.common.PrintoutType.CustomerInformationForm);
                //            if (!String.IsNullOrEmpty(BilgilendirmeURL))
                //            {
                //                if (BilgilendirmeURL != null)
                //                {
                //                    WebClient myClient = new WebClient();
                //                    byte[] data = myClient.DownloadData(BilgilendirmeURL);

                //                    IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                //                    string fileName = String.Empty;
                //                    string url = String.Empty;
                //                    fileName = String.Format("AK_Kasko_Bilgilendirme_Formu_{0}.pdf", System.Guid.NewGuid().ToString());
                //                    url = storage.UploadFile("kasko", fileName, data);
                //                    this.GenelBilgiler.PDFBilgilendirme = url;
                //                    _Log.Info("Police_Bilgilendirme_Formu url: {0}", url);
                //                }
                //                else
                //                {
                //                    this.AddHata("Police Bilgilendirme Formu alınamadı.");
                //                }
                //            }
                //            #endregion
                //        }
                //        #endregion

                //        #endregion

                //        this.GenelBilgiler.WEBServisLogs = this.Log;
                //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //    }
                //    catch (Exception ex)
                //    {
                //        #region Hata Log
                //        client.Abort();
                //        this.EndLog(ex.Message, false);
                //        this.AddHata(ex.Message);

                //        this.GenelBilgiler.WEBServisLogs = this.Log;
                //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //        #endregion
                //    }
                //}

                //private ak.kasko.Unit SigortaliBilgileri(ITeklif teklif, ak.kasko.Casco clnt)
                //{
                //    MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //    MusteriAdre adress = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //    MusteriTelefon cepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                //    MusteriTelefon evTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                //    Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
                //    ak.kasko.Unit unit = new ak.kasko.Unit();

                //    ak.kasko.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
                //    clnt.Dispose();
                //    ak.kasko.MethodResultOfListOfAddressItem ilceler = new ak.kasko.MethodResultOfListOfAddressItem();

                //    try
                //    {
                //        List<ak.kasko.Address> addressList = new List<ak.kasko.Address>();
                //        ak.kasko.Address address = new ak.kasko.Address();

                //        address.ADDRESS_TYPE = AK_AdresKodlari.Il;
                //        address.ADDRESS_DATA = "İSTANBUL";
                //        //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                //        if (il != null)
                //        {
                //            var sompoIlAdi = il.Result.Where(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).FirstOrDefault();
                //            if (sompoIlAdi != null)
                //            {
                //                address.ADDRESS_DATA = sompoIlAdi.ItemName;
                //            }
                //        }
                //        addressList.Add(address);

                //        ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                //        clnt.Dispose();
                //        address = new ak.kasko.Address();
                //        address.ADDRESS_TYPE = AK_AdresKodlari.Ilce;
                //        address.ADDRESS_DATA = "MERKEZ";
                //        //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                //        if (ilceler != null && ilceler.Result != null)
                //        {
                //            var sompoIlceAdi = ilceler.Result.Where(S => S.ItemName == ilce.IlceAdi).FirstOrDefault();
                //            if (sompoIlceAdi != null)
                //            {
                //                address.ADDRESS_DATA = sompoIlceAdi.ItemName;
                //            }
                //        }
                //        addressList.Add(address);

                //        if (!String.IsNullOrEmpty(adress.Mahalle))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = adress.Mahalle;
                //            addressList.Add(address);

                //        }
                //        else if (!String.IsNullOrEmpty(adress.Semt))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Semt;
                //            address.ADDRESS_DATA = adress.Semt;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = "Mah.";
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.Cadde))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = adress.Cadde;
                //            addressList.Add(address);
                //        }
                //        else if (!String.IsNullOrEmpty(adress.Sokak))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Sokak;
                //            address.ADDRESS_DATA = adress.Sokak;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = "Cad.";
                //            addressList.Add(address);
                //        }
                //        if (!String.IsNullOrEmpty(adress.Apartman))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Apartmani;
                //            address.ADDRESS_DATA = adress.Apartman;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.BinaNo))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.BinaNo;
                //            address.ADDRESS_DATA = adress.BinaNo;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.DaireNo))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.DaireNo;
                //            address.ADDRESS_DATA = adress.DaireNo;
                //            addressList.Add(address);
                //        }

                //        unit.ADDRESS_LIST = addressList.ToArray();

                //        if (sigortali.KimlikNo.Length == 11)
                //        {
                //            unit.IDENTITY_NO = Convert.ToInt64(sigortali.KimlikNo);
                //            unit.PERSONAL_COMMERCIAL = "O";
                //        }
                //        else
                //        {
                //            unit.TAX_NO = sigortali.KimlikNo;
                //            unit.PERSONAL_COMMERCIAL = "T";
                //        }

                //        //unit.PASSPORT_NO = 0;
                //        unit.NAME = sigortali.AdiUnvan;
                //        unit.SURNAME = sigortali.SoyadiUnvan;
                //        if (sigortali.DogumTarihi.HasValue)
                //        {
                //            unit.BIRTHDATE = sigortali.DogumTarihi.Value;
                //        }
                //        unit.FIRM_NAME = "";
                //        unit.FATHER_NAME = "";
                //        unit.MARITAL_STATUS = "";
                //        unit.BIRTH_PLACE = "";
                //        unit.OCCUPATION = Ak_KaskoSoruCevapTipleri.Diger;
                //        unit.NATIONALITY = 1;
                //        unit.PERSONAL_COMMERCIAL = "";

                //        if (evTel != null)
                //        {
                //            if (evTel.Numara.Length > 10)
                //            {

                //                unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                //                unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                //                unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                //            }
                //        }
                //        if (cepTel != null)
                //        {
                //            if (cepTel.Numara.Length > 10)
                //            {

                //                unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                //                unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                //                unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                //            }
                //        }

                //        unit.EMAIL_ADDRESS = sigortali.EMail;
                //        unit.GENDER = sigortali.Cinsiyet;

                //        return unit;
                //    }
                //    catch (Exception ex)
                //    {
                //        clnt.Abort();
                //        string hataMesaji = "";
                //        if (!string.IsNullOrEmpty(il.Description))
                //        {
                //            if (il.Description != "Success")
                //            {
                //                hataMesaji += il.Description;
                //            }
                //        }
                //        else
                //        {
                //            hataMesaji += il.ResultCode.ToString();
                //        }
                //        if (!string.IsNullOrEmpty(ilceler.Description))
                //        {
                //            hataMesaji += ilceler.ResultCode.ToString() + ilceler.Description;
                //        }
                //        hataMesaji += ex.Message;
                //        throw new Exception(hataMesaji);
                //    }
                //}

                //private ak.kasko.Unit SigortaEttirenBilgileri(ITeklif teklif, ak.kasko.Casco clnt)
                //{
                //    TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                //    MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                //    MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
                //    MusteriTelefon cepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                //    MusteriTelefon evTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                //    Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
                //    ak.kasko.Unit unit = new ak.kasko.Unit();
                //    ak.kasko.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
                //    clnt.Dispose();
                //    ak.kasko.MethodResultOfListOfAddressItem ilceler = new ak.kasko.MethodResultOfListOfAddressItem();
                //    try
                //    {
                //        List<ak.kasko.Address> addressList = new List<ak.kasko.Address>();
                //        ak.kasko.Address address = new ak.kasko.Address();
                //        address.ADDRESS_TYPE = AK_AdresKodlari.Il;
                //        address.ADDRESS_DATA = "İSTANBUL";
                //        //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                //        if (il != null)
                //        {
                //            var sompoIlAdi = il.Result.Where(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).FirstOrDefault();
                //            if (sompoIlAdi != null)
                //            {
                //                address.ADDRESS_DATA = sompoIlAdi.ItemName;
                //            }
                //        }
                //        addressList.Add(address);

                //        ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                //        clnt.Dispose();
                //        address = new ak.kasko.Address();
                //        address.ADDRESS_TYPE = AK_AdresKodlari.Ilce;
                //        address.ADDRESS_DATA = "MERKEZ";
                //        //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                //        if (ilceler != null && ilceler.Result != null)
                //        {
                //            var sompoIlceAdi = ilceler.Result.Where(S => S.ItemName == ilce.IlceAdi).FirstOrDefault();
                //            if (sompoIlceAdi != null)
                //            {
                //                address.ADDRESS_DATA = sompoIlceAdi.ItemName;
                //            }
                //        }

                //        addressList.Add(address);

                //        if (!String.IsNullOrEmpty(adress.Mahalle))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = adress.Mahalle;
                //            addressList.Add(address);

                //        }
                //        else if (!String.IsNullOrEmpty(adress.Semt))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Semt;
                //            address.ADDRESS_DATA = adress.Semt;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = "Mah.";
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.Cadde))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = adress.Cadde;
                //            addressList.Add(address);
                //        }
                //        else if (!String.IsNullOrEmpty(adress.Sokak))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Sokak;
                //            address.ADDRESS_DATA = adress.Sokak;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = "Cad.";
                //            addressList.Add(address);
                //        }
                //        if (!String.IsNullOrEmpty(adress.Apartman))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Apartmani;
                //            address.ADDRESS_DATA = adress.Apartman;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.BinaNo))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.BinaNo;
                //            address.ADDRESS_DATA = adress.BinaNo;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.DaireNo))
                //        {
                //            address = new ak.kasko.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.DaireNo;
                //            address.ADDRESS_DATA = adress.DaireNo;
                //            addressList.Add(address);
                //        }

                //        unit.ADDRESS_LIST = addressList.ToArray();

                //        if (SEGenelBilgiler.KimlikNo.Length == 11)
                //        {
                //            unit.IDENTITY_NO = Convert.ToInt64(SEGenelBilgiler.KimlikNo);
                //            unit.PERSONAL_COMMERCIAL = "O";
                //        }
                //        else
                //        {
                //            unit.TAX_NO = SEGenelBilgiler.KimlikNo;
                //            unit.PERSONAL_COMMERCIAL = "T";
                //        }

                //        //unit.PASSPORT_NO = 0;
                //        unit.NAME = SEGenelBilgiler.AdiUnvan;
                //        unit.SURNAME = SEGenelBilgiler.SoyadiUnvan;
                //        if (SEGenelBilgiler.DogumTarihi.HasValue)
                //        {
                //            unit.BIRTHDATE = SEGenelBilgiler.DogumTarihi.Value;
                //        }
                //        unit.FIRM_NAME = "";
                //        unit.FATHER_NAME = "";
                //        unit.MARITAL_STATUS = "";
                //        unit.BIRTH_PLACE = "";
                //        unit.OCCUPATION = Ak_KaskoSoruCevapTipleri.Diger;
                //        unit.NATIONALITY = 1;

                //        if (evTel != null)
                //        {
                //            if (evTel.Numara.Length > 10)
                //            {

                //                unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                //                unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                //                unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                //            }
                //        }
                //        if (cepTel != null)
                //        {
                //            if (cepTel.Numara.Length > 10)
                //            {

                //                unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                //                unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                //                unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                //            }
                //        }
                //        unit.EMAIL_ADDRESS = SEGenelBilgiler.EMail;
                //        unit.GENDER = SEGenelBilgiler.Cinsiyet;

                //        return unit;
                //    }
                //    catch (Exception)
                //    {
                //        clnt.Abort();
                //        string hataMesaji = "";
                //        if (!string.IsNullOrEmpty(il.Description))
                //            hataMesaji += il.Description;
                //        else
                //        {
                //            hataMesaji += il.ResultCode.ToString();
                //        }
                //        if (!string.IsNullOrEmpty(ilceler.Description))
                //            hataMesaji += ilceler.ResultCode.ToString() + ilceler.Description;

                //        throw new Exception(hataMesaji);
                //    }
                //}

                //private ak.kasko.CustomParameter[] TeklifSorular(ITeklif teklif, ak.kasko.Casco clntKasko, TVMWebServisKullanicilari serviskullanici)
                //{
                //    #region İp li ws
                //    List<ak.kasko.CustomParameter> customParameter = new List<ak.kasko.CustomParameter>();
                //    ak.kasko.CustomParameter parametre = new ak.kasko.CustomParameter();
                //    ak.kasko.CascoParameters cascoparameters = new ak.kasko.CascoParameters();


                //    //Tescil veya asbis no gönderiliyor
                //    string TescilSeriNo = teklif.Arac.TescilSeriNo;
                //    string TescilSeriKod = teklif.Arac.TescilSeriKod;
                //    if (!String.IsNullOrEmpty(TescilSeriNo) && !String.IsNullOrEmpty(TescilSeriKod))
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak.TescilBelgeKod;
                //        parametre.value = TescilSeriKod;
                //        customParameter.Add(parametre);

                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak.AsbisTescilSeriNo;
                //        parametre.value = TescilSeriNo;
                //        customParameter.Add(parametre);
                //    }
                //    else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_TrafikSoruTipleri.AsbisTescilSeriNo;
                //        parametre.value = teklif.Arac.AsbisNo;
                //        customParameter.Add(parametre);
                //    }

                //    string kullanimT = String.Empty;
                //    string kod2 = String.Empty;
                //    string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //    if (parts.Length == 2)
                //    {
                //        kullanimT = parts[0];
                //        kod2 = parts[1];
                //    }

                //    #region IMM ve FK

                //    string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                //    decimal bedeniSahis = 0;
                //    decimal Kombine = 0;
                //    if (!String.IsNullOrEmpty(immKademe))
                //    {
                //        //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                //        var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                //        if (IMMBedel != null)
                //        {
                //            bedeniSahis = IMMBedel.BedeniSahis.Value;
                //            Kombine = IMMBedel.Kombine.Value;

                //            ak.kasko.MethodResultOfListOfImmValues AkIMMList = clntKasko.GetIMMListExt();
                //            clntKasko.Dispose();

                //            // Sompo Japan IMM Limit listesi wsinden, ekrandan girilen bedelin değer karşılığı alınıyor
                //            if (AkIMMList.Result != null)
                //            {
                //                if (bedeniSahis > 0)
                //                {
                //                    var immBedel = AkIMMList.Result.Where(s => s.KisiBasiBedeni == bedeniSahis).FirstOrDefault();
                //                    if (immBedel == null)
                //                    {
                //                        immBedel = AkIMMList.Result.Where(s => s.KisiBasiBedeni <= bedeniSahis).OrderByDescending(s => s.KisiBasiBedeni).FirstOrDefault();
                //                    }
                //                    if (immBedel != null)
                //                    {
                //                        parametre = new ak.kasko.CustomParameter();
                //                        parametre.code = Ak_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
                //                        parametre.value = Ak_KaskoSoruCevapTipleri.KisiKazaBasina;
                //                        customParameter.Add(parametre);

                //                        parametre = new ak.kasko.CustomParameter();
                //                        parametre.code = Ak_KaskoSoruTipleri.IMMBedelSecimi;
                //                        parametre.value = immBedel.Kod.ToString();
                //                        customParameter.Add(parametre);
                //                    }
                //                }
                //                else
                //                {
                //                    var kombineBedel = AkIMMList.Result.Where(s => s.Kombine == Kombine).FirstOrDefault();
                //                    if (kombineBedel == null)
                //                    {
                //                        kombineBedel = AkIMMList.Result.Where(s => s.Kombine <= Kombine).OrderByDescending(s => s.Kombine).FirstOrDefault();
                //                    }
                //                    if (kombineBedel != null)
                //                    {
                //                        parametre = new ak.kasko.CustomParameter();
                //                        parametre.code = Ak_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
                //                        parametre.value = Ak_KaskoSoruCevapTipleri.Kombine;
                //                        customParameter.Add(parametre);

                //                        parametre = new ak.kasko.CustomParameter();
                //                        parametre.code = Ak_KaskoSoruTipleri.IMMBedelSecimi;
                //                        parametre.value = kombineBedel.Kod.ToString();
                //                        customParameter.Add(parametre);
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    string ferdiKaza = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                //    decimal FKVefat = 0;
                //    decimal FKMasraf = 0;
                //    if (!String.IsNullOrEmpty(ferdiKaza))
                //    {
                //        CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                //        var FK = _CRService.GetKaskoFKBedel(Convert.ToInt32(ferdiKaza), parts[0], parts[1]);
                //        if (FK != null)
                //        {
                //            FKVefat = FK.Vefat.Value;
                //            FKMasraf = FK.Tedavi.Value;
                //        }
                //        var AkFKTedaviMasraflari = clntKasko.PersonalAccidentCoverLimitsExt(Ak_KaskoTeminatKodlari.FerdiKazaTedaviMasraflari).Result;
                //        clntKasko.Dispose();
                //        if (AkFKTedaviMasraflari != null && FKMasraf > 0)
                //        {
                //            parametre = new ak.kasko.CustomParameter();
                //            parametre.code = Ak_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;

                //            foreach (var item in AkFKTedaviMasraflari)
                //            {
                //                if ((item.ItemText == "Min" && FKMasraf > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKMasraf < Convert.ToDecimal(item.ItemValue)))
                //                    parametre.value = FKMasraf.ToString();
                //                else
                //                    parametre.value = AkFKTedaviMasraflari.FirstOrDefault().ItemValue;
                //            }
                //            customParameter.Add(parametre);
                //        }
                //    }
                //    //else
                //    //{
                //    //    parametre = new sompojapan.kasko.CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;
                //    //    parametre.value = "500";
                //    //    customParameter.Add(parametre);
                //    //}
                //    var AkFKVefatTeminati = clntKasko.PersonalAccidentCoverLimitsExt(Ak_KaskoTeminatKodlari.FerdiKazaVefatTeminati).Result;
                //    clntKasko.Dispose();
                //    if (AkFKVefatTeminati != null && FKVefat > 0)
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.FerdiKazaVefatTeminati;
                //        foreach (var item in AkFKVefatTeminati)
                //        {
                //            if ((item.ItemText == "Min" && FKVefat > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKVefat < Convert.ToDecimal(item.ItemValue)))
                //                parametre.value = FKVefat.ToString();
                //            else
                //                parametre.value = AkFKVefatTeminati.FirstOrDefault().ItemValue;
                //        }

                //        customParameter.Add(parametre);
                //    }
                //    //else
                //    //{
                //    //    parametre = new sompojapan.kasko.CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
                //    //    parametre.value = "500";
                //    //    customParameter.Add(parametre);
                //    //}

                //    #endregion

                //    #region Marka Model

                //    var akMarkaModel = "";
                //    var akMarkaList = clntKasko.BrandsWithCodeExt();
                //    if (akMarkaList.Result != null)
                //    {
                //        // sompojapan.trafik.Traffic clntTrafik = new sompojapan.trafik.Traffic();

                //        clntKasko.IdentityHeaderValue = new ak.kasko.IdentityHeader()
                //        {
                //            KullaniciAdi = serviskullanici.KullaniciAdi,
                //            KullaniciParola = serviskullanici.Sifre,
                //            KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                //            KullaniciTipi = Neosinerji.BABOnlineTP.Business.ak.kasko.ClientType.ACENTE
                //        };

                //        var akMarka = akMarkaList.Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
                //        if (akMarka != null)
                //        {
                //            string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

                //            if (aracTipi.Length < 3)
                //            {
                //                aracTipi = "0" + aracTipi;
                //            }
                //            string markaModel = akMarka.Trim() + aracTipi;
                //            var akMarkaModelList = clntKasko.ModelsViaBrandCodeExt(akMarka.ToString()).Result;
                //            clntKasko.Dispose();

                //            if (akMarkaModelList != null)
                //            {
                //                akMarkaModel = akMarkaModelList.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue.Trim();
                //                if (akMarkaModel != null)
                //                {
                //                    parametre = new ak.kasko.CustomParameter();
                //                    parametre.code = Ak_KaskoSoruTipleri.MarkaAdi;
                //                    parametre.value = akMarkaModel;
                //                    customParameter.Add(parametre);
                //                }
                //            }
                //        }
                //    }

                //    #endregion

                //    #region Diğer Araç Bilgileri

                //    parametre = new ak.kasko.CustomParameter();
                //    parametre.code = Ak_KaskoSoruTipleri.TrafigeCikisTarihi;
                //    parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //    customParameter.Add(parametre);

                //    parametre = new ak.kasko.CustomParameter();
                //    parametre.code = Ak_KaskoSoruTipleri.KullanimKodu;

                //    string kullanimtarzi = teklif.Arac.KullanimTarzi;
                //    int akKullanimTarzi = 1;
                //    if (!String.IsNullOrEmpty(kullanimtarzi))
                //    {
                //        switch (kullanimtarzi)
                //        {
                //            case "111-10": parametre.value = Ak_KaskoTarifeKodlari.Otomobil; break;
                //            case "111-14": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "311-11": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "311-12": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "411-10": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "411-11": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "411-12": parametre.value = Ak_KaskoTarifeKodlari.Minibus; break;
                //            case "311-15": parametre.value = Ak_KaskoTarifeKodlari.Kamyonet; break;
                //            case "511-10": parametre.value = Ak_KaskoTarifeKodlari.Kamyonet; break;
                //            case "511-11": parametre.value = Ak_KaskoTarifeKodlari.Kamyonet; break;
                //            case "511-12": parametre.value = Ak_KaskoTarifeKodlari.Kamyonet; break;
                //            case "511-15": parametre.value = Ak_KaskoTarifeKodlari.Kamyonet; break;
                //            case "611-10": parametre.value = Ak_KaskoTarifeKodlari.Traktor; break;
                //            case "711-10": parametre.value = Ak_KaskoTarifeKodlari.Motorsiklet; break;
                //            case "521-10": parametre.value = Ak_KaskoTarifeKodlari.Kamyon; break;
                //            case "521-11": parametre.value = Ak_KaskoTarifeKodlari.Kamyon; break;
                //            case "421-10": parametre.value = Ak_KaskoTarifeKodlari.BOtobus; break;
                //            case "421-11": parametre.value = Ak_KaskoTarifeKodlari.BOtobus; break;
                //            case "121-10": parametre.value = Ak_KaskoTarifeKodlari.Jeep; break;
                //            case "532-10": parametre.value = Ak_KaskoTarifeKodlari.Romork; break;
                //            case "526-10": parametre.value = Ak_KaskoTarifeKodlari.Cekici; break;
                //            case "911-10": parametre.value = Ak_KaskoTarifeKodlari.IsMakinesi; break;
                //            case "911-11": parametre.value = Ak_KaskoTarifeKodlari.IsMakinesi; break;
                //            case "911-12": parametre.value = Ak_KaskoTarifeKodlari.IsMakinesi; break;
                //            case "523-10": parametre.value = Ak_KaskoTarifeKodlari.IsMakinesi; break;
                //            case "611-11": parametre.value = Ak_KaskoTarifeKodlari.IsMakinesi; break;
                //        }
                //    }
                //    else parametre.value = "1";

                //    akKullanimTarzi = Convert.ToInt32(parametre.value);

                //    customParameter.Add(parametre);

                //    if (teklif.Arac.KoltukSayisi.HasValue)
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.Koltukadedi;
                //        parametre.value = teklif.Arac.KoltukSayisi.Value.ToString();
                //        customParameter.Add(parametre);
                //    }
                //    bool anahtarKaybi = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
                //    if (anahtarKaybi)
                //    {
                //        var akAnahtarKaybi = clntKasko.LostIgnitionKeyCoversExt();
                //        clntKasko.Dispose();
                //        string akAnahtarKaybiBedel = "";
                //        if (akAnahtarKaybi.Result != null)
                //        {
                //            akAnahtarKaybiBedel = akAnahtarKaybi.Result.FirstOrDefault(s => s.ItemValue == "1").ItemValue;
                //        }
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.AnahtarKaybiTeminatLimiti;
                //        parametre.value = akAnahtarKaybiBedel;
                //        customParameter.Add(parametre);
                //    }

                //    bool artiTeminatPlani = teklif.ReadSoru(KaskoSorular.AkArtiTeminatPlani, false);
                //    if (artiTeminatPlani)
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.ArtiTeminatIstiyorMu;
                //        parametre.value = "E";
                //        customParameter.Add(parametre);

                //        string artiTeminatPlanDegeri = teklif.ReadSoru(KaskoSorular.AkArtiTeminatPlanDegeri, "");
                //        if (!String.IsNullOrEmpty(artiTeminatPlanDegeri))
                //        {
                //            parametre = new ak.kasko.CustomParameter();
                //            parametre.code = Ak_KaskoSoruTipleri.ArtiTeminatDegeri;
                //            parametre.value = artiTeminatPlanDegeri;
                //            customParameter.Add(parametre);

                //        }
                //    }
                //    else
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.ArtiTeminatIstiyorMu;
                //        parametre.value = "H";
                //        customParameter.Add(parametre);
                //    }

                //    #endregion

                //    #region Eski Poliçesi Olmayan Araç için Sorular

                //    bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                //    if (!eskiPoliceVar)
                //    {
                //        //parametre = new sompojapan.kasko.CustomParameter();

                //        //if (sompoJapanMarkaModel != null)
                //        //{
                //        //    parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
                //        //    parametre.value = sompoJapanMarkaModel;
                //        //    customParameter.Add(parametre);
                //        //}
                //        //parametre = new sompojapan.kasko.CustomParameter();
                //        //parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
                //        //parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //        //customParameter.Add(parametre);

                //        //parametre = new sompojapan.kasko.CustomParameter();
                //        //parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

                //        //if (!String.IsNullOrEmpty(kullanimtarzi))
                //        //{
                //        //    switch (kullanimtarzi)
                //        //    {
                //        //        case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
                //        //        case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                //        //        case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //        //        case "511-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //        //        case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
                //        //        case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
                //        //        case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                //        //        case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                //        //        case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
                //        //        case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
                //        //        case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
                //        //        case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                //        //        case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
                //        //    }
                //        //}
                //        //else parametre.value = "1";
                //        //customParameter.Add(parametre);

                //        //parametre = new sompojapan.kasko.CustomParameter();
                //        //parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
                //        //parametre.value = teklif.Arac.KoltukSayisi.ToString();
                //        //customParameter.Add(parametre);             
                //    }
                //    #endregion

                //    #region Kasa Tipi

                //    //var sompoJapanKasaTipiList = clntKasko.VehicleCaseTypesExt(false, sompoKullanimTarzi);
                //    //clntKasko.Dispose();
                //    //if (sompoJapanKasaTipiList != null && sompoJapanKasaTipiList.Result != null && sompoJapanKasaTipiList.Description == "Success")
                //    //{
                //    //    parametre = new sompojapan.kasko.CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.KasaTipi;
                //    //    parametre.value = sompoJapanKasaTipiList.Result;
                //    //    customParameter.Add(parametre);
                //    //}

                //    if (akKullanimTarzi == 3 || akKullanimTarzi == 4 || akKullanimTarzi == 7 || akKullanimTarzi == 12 || akKullanimTarzi == 13)
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.KasaTipi;
                //        parametre.value = "3";
                //        customParameter.Add(parametre);
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.KasaTeminati;
                //        parametre.value = "H";
                //        customParameter.Add(parametre);
                //    }

                //    #endregion

                //    #region Kasko Türü
                //    bool kaskoTuru = teklif.ReadSoru(KaskoSorular.AkKaskoTuru, true);
                //    string faaliyetKodu = teklif.ReadSoru(KaskoSorular.AkFaaliyetKodu, String.Empty);
                //    if (!kaskoTuru)
                //    {
                //        parametre = new ak.kasko.CustomParameter();
                //        parametre.code = Ak_KaskoSoruTipleri.FaaliyetKodu;
                //        parametre.value = faaliyetKodu;
                //        customParameter.Add(parametre);
                //    }
                //    else
                //    {
                //        string MeslekKodu = teklif.ReadSoru(KaskoSorular.Meslek, "99");
                //        var AkMeslekKodu = _TeklifService.GetTUMMeslekKod(MeslekKodu, TeklifUretimMerkezleri.AK);

                //        if (AkMeslekKodu != null)
                //        {
                //            parametre = new ak.kasko.CustomParameter();
                //            parametre.code = Ak_KaskoSoruTipleri.MeslekKodu;
                //            parametre.value = AkMeslekKodu.CR_MeslekKodu;
                //            customParameter.Add(parametre);
                //        }
                //    }
                //    #endregion
                //    cascoparameters.Parameters = customParameter.ToArray();
                //    #endregion

                //    #region Kullanıcı Adı, Şifre ws
                //    //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);

                //    //clnt = new sompojapankasko.Casco();
                //    //clnt.Url = konfig[Konfig.SOMPOJAPAN_CascoServiceURL];
                //    //clnt.Timeout = 150000;

                //    //List<CustomParameter> customParameter = new List<CustomParameter>();
                //    //CustomParameter parametre = new CustomParameter();
                //    //CascoParameters cascoparameters = new CascoParameters();

                //    //parametre.code = SompoJapan_KaskoSoruTipleri.MeslekKodu;
                //    //parametre.value = "99";
                //    //customParameter.Add(parametre);

                //    //string kullanimT = String.Empty;
                //    //string kod2 = String.Empty;
                //    //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //    //if (parts.Length == 2)
                //    //{
                //    //    kullanimT = parts[0];
                //    //    kod2 = parts[1];
                //    //}

                //    //string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                //    //decimal bedeniSahis = 0;
                //    //decimal Kombine = 0;
                //    //if (!String.IsNullOrEmpty(immKademe))
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
                //    //    parametre.value = SompoJapan_KaskoSoruCevapTipleri.KisiKazaBasina;
                //    //    customParameter.Add(parametre);

                //    //    //var iMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
                //    //    //                                                       s.Kademe == immKademe && s.Kod2 == kod2 &&
                //    //    //                                                       s.KullanimTarziKodu == kullanimT).FirstOrDefault();

                //    //    //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                //    //    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                //    //    if (IMMBedel != null)
                //    //    {
                //    //        bedeniSahis = IMMBedel.BedeniSahis.Value;
                //    //        Kombine = IMMBedel.Kombine.Value;

                //    //        MethodResultOfListOfImmValues SompoIMMList = clnt.GetIMMList(servisKullanici.KullaniciAdi, servisKullanici.Sifre);

                //    //        // Sompo Japan IMM Limit listesi wsinden, ekrandan girilen bedelin değer karşılığı alınıyor

                //    //        if (SompoIMMList.Result != null)
                //    //        {
                //    //            parametre = new CustomParameter();
                //    //            foreach (var immlist in SompoIMMList.Result)
                //    //            {
                //    //                if (bedeniSahis > 0)
                //    //                {
                //    //                    if (Convert.ToDecimal(immlist.KisiBasiBedeni) == bedeniSahis || Convert.ToDecimal(immlist.KisiBasiBedeni) < bedeniSahis)
                //    //                    {
                //    //                        parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
                //    //                        parametre.value = immlist.Kod.ToString();

                //    //                    }
                //    //                }
                //    //                else
                //    //                {
                //    //                    if (Convert.ToDecimal(immlist.Kombine) == Kombine || Convert.ToDecimal(immlist.Kombine) < Kombine)
                //    //                    {
                //    //                        parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
                //    //                        parametre.value = immlist.Kod.ToString();

                //    //                    }
                //    //                }
                //    //            }
                //    //            customParameter.Add(parametre);
                //    //        }
                //    //    }
                //    //}
                //    ////sompojapan.kasko.Casco clntKasko = new sompojapan.kasko.Casco();
                //    ////clntKasko.IdentityHeaderValue = new sompojapan.kasko.IdentityHeader()
                //    ////{
                //    ////    KullaniciAdi = servisKullanici.KullaniciAdi,
                //    ////    KullaniciParola = servisKullanici.Sifre,
                //    ////    KullaniciIP = "85.105.78.56",
                //    ////    KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.DEVELOP
                //    ////};

                //    ////KonfigTable konfigKasko = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKaskoV2);
                //    ////clntKasko.Url = konfigKasko[Konfig.SOMPOJAPAN_CascoServiceURLV2];

                //    //////var sompoJapanMarka2 = clntKasko.BrandsWithCodeExt().Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue; 
                //    ////var sompojapanMarkaModel = clntKasko.ModelsViaBrandCodeExt(sompoJapanMarka2);

                //    //string sompoJapanMarka = clnt.BrandsWithCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
                //    //string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

                //    //if (aracTipi.Length < 3)
                //    //{
                //    //    aracTipi = "0" + aracTipi;
                //    //}
                //    //string markaModel = sompoJapanMarka.Trim() + aracTipi;
                //    //var sompoJapanMarkaModel = clnt.ModelsViaBrandCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre, sompoJapanMarka).Result.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue.Trim();

                //    //if (sompoJapanMarkaModel != null)
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
                //    //    parametre.value = sompoJapanMarkaModel;
                //    //    customParameter.Add(parametre);
                //    //}

                //    //parametre = new CustomParameter();
                //    //parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
                //    //parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //    //customParameter.Add(parametre);

                //    //parametre = new CustomParameter();
                //    //parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

                //    //string kullanimtarzi = teklif.Arac.KullanimTarzi;
                //    //if (!String.IsNullOrEmpty(kullanimtarzi))
                //    //{
                //    //    switch (kullanimtarzi)
                //    //    {
                //    //        case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
                //    //        case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                //    //        case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //    //        case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
                //    //        case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
                //    //        case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                //    //        case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                //    //        case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
                //    //        case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
                //    //        case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
                //    //        case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                //    //        case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
                //    //    }
                //    //}
                //    //else parametre.value = "1";

                //    //customParameter.Add(parametre);

                //    //if (teklif.Arac.KoltukSayisi.HasValue)
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
                //    //    parametre.value = teklif.Arac.KoltukSayisi.Value.ToString();
                //    //    customParameter.Add(parametre);
                //    //}
                //    //bool anahtarKaybi = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
                //    //if (anahtarKaybi)
                //    //{
                //    //    string sompoJapanAnahtarKaybiBedel = clnt.LostIgnitionKeyCovers(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(s => s.ItemValue == "1").ItemValue;

                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.AnahtarKaybiTeminatLimiti;
                //    //    parametre.value = sompoJapanAnahtarKaybiBedel;
                //    //    customParameter.Add(parametre);
                //    //}

                //    //string ferdiKaza = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                //    //decimal FKVefat = 0;
                //    //decimal FKMasraf = 0;
                //    //if (!String.IsNullOrEmpty(ferdiKaza))
                //    //{
                //    //    //var FK = _CRContext.CR_KaskoFKRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
                //    //    //                                                      s.Kademe == ferdiKaza && s.Kod2 == kod2 &&
                //    //    //                                                      s.KullanimTarziKodu == kullanimT).FirstOrDefault();
                //    //    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                //    //    var FK = _CRService.GetKaskoFKBedel(Convert.ToInt32(ferdiKaza), parts[0], parts[1]);
                //    //    if (FK != null)
                //    //    {
                //    //        FKVefat = FK.Vefat.Value;
                //    //        FKMasraf = FK.Tedavi.Value;
                //    //    }
                //    //    var SompoFKTedaviMasraflari = clnt.PersonalAccidentCoverLimits(servisKullanici.KullaniciAdi, servisKullanici.Sifre, SompoJapan_KaskoTeminatKodlari.FerdiKazaTedaviMasraflari).Result;
                //    //    if (SompoFKTedaviMasraflari != null && FKMasraf > 0)
                //    //    {
                //    //        parametre = new CustomParameter();
                //    //        parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;

                //    //        foreach (var item in SompoFKTedaviMasraflari)
                //    //        {
                //    //            if ((item.ItemText == "Min" && FKMasraf > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKMasraf < Convert.ToDecimal(item.ItemValue)))
                //    //                parametre.value = FKMasraf.ToString();
                //    //            else
                //    //                parametre.value = SompoFKTedaviMasraflari.FirstOrDefault().ItemValue;
                //    //        }
                //    //        customParameter.Add(parametre);
                //    //    }
                //    //}
                //    //else
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;
                //    //    parametre.value = "500";
                //    //    customParameter.Add(parametre);
                //    //}
                //    //var SompoFKVefatTeminati = clnt.PersonalAccidentCoverLimits(servisKullanici.KullaniciAdi, servisKullanici.Sifre, SompoJapan_KaskoTeminatKodlari.FerdiKazaVefatTeminati).Result;
                //    //if (SompoFKVefatTeminati != null && FKVefat > 0)
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
                //    //    foreach (var item in SompoFKVefatTeminati)
                //    //    {
                //    //        if ((item.ItemText == "Min" && FKVefat > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKVefat < Convert.ToDecimal(item.ItemValue)))
                //    //            parametre.value = FKMasraf.ToString();
                //    //        else
                //    //            parametre.value = SompoFKVefatTeminati.FirstOrDefault().ItemValue;
                //    //    }

                //    //    customParameter.Add(parametre);
                //    //}
                //    //else
                //    //{
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
                //    //    parametre.value = "500";
                //    //    customParameter.Add(parametre);
                //    //}
                //    //cascoparameters.Parameters = customParameter.ToArray();

                //    //#region Yeni Arac

                //    //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                //    //if (!eskiPoliceVar)
                //    //{
                //    //    parametre = new CustomParameter();

                //    //    if (sompoJapanMarkaModel != null)
                //    //    {
                //    //        parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
                //    //        parametre.value = sompoJapanMarkaModel;
                //    //        customParameter.Add(parametre);
                //    //    }
                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
                //    //    parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //    //    customParameter.Add(parametre);

                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

                //    //    if (!String.IsNullOrEmpty(kullanimtarzi))
                //    //    {
                //    //        switch (kullanimtarzi)
                //    //        {
                //    //            case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
                //    //            case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                //    //            case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //    //            case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
                //    //            case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
                //    //            case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                //    //            case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                //    //            case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
                //    //            case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
                //    //            case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
                //    //            case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                //    //            case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
                //    //        }
                //    //    }
                //    //    else parametre.value = "1";
                //    //    customParameter.Add(parametre);

                //    //    parametre = new CustomParameter();
                //    //    parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
                //    //    parametre.value = teklif.Arac.KoltukSayisi.ToString();
                //    //    customParameter.Add(parametre);

                //    //    cascoparameters.Parameters = customParameter.ToArray();
                //    //}
                //    //#endregion
                //    #endregion

                //    return cascoparameters.Parameters;
                //}

                //private ak.common.Unit SigortaEttirenBilgileriCommon(ITeklif teklif, TVMWebServisKullanicilari servisKullanici)
                //{
                //    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAKTrafikV2);

                //    ak.trafik.Traffic clnt = new ak.trafik.Traffic();
                //    clnt.Url = konfig[Konfig.AK_TrafikServiceURLV2];
                //    clnt.Timeout = 150000;

                //    clnt.IdentityHeaderValue = new ak.trafik.IdentityHeader()
                //    {
                //        KullaniciAdi = servisKullanici.KullaniciAdi,
                //        KullaniciParola = servisKullanici.Sifre,
                //        KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                //        KullaniciTipi = ak.trafik.ClientType.ACENTE,
                //    };


                //    TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;

                //    MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                //    MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
                //    MusteriTelefon cepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                //    MusteriTelefon evTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                //    Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
                //    ak.common.Unit unit = new ak.common.Unit();

                //    ak.trafik.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
                //    clnt.Dispose();
                //    ak.trafik.MethodResultOfListOfAddressItem ilceler = new sompojapan.trafik.MethodResultOfListOfAddressItem();

                //    try
                //    {
                //        List<ak.common.Address> addressList = new List<ak.common.Address>();
                //        ak.common.Address address = new ak.common.Address();

                //        address.ADDRESS_TYPE = AK_AdresKodlari.Il;
                //        address.ADDRESS_DATA = il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName;

                //        addressList.Add(address);

                //        ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                //        clnt.Dispose();
                //        address = new ak.common.Address();
                //        address.ADDRESS_TYPE = AK_AdresKodlari.Ilce;
                //        address.ADDRESS_DATA = ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName;
                //        addressList.Add(address);

                //        if (!String.IsNullOrEmpty(adress.Mahalle))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = adress.Mahalle;
                //            addressList.Add(address);

                //        }
                //        else if (!String.IsNullOrEmpty(adress.Semt))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Semt;
                //            address.ADDRESS_DATA = adress.Semt;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Mahallesi;
                //            address.ADDRESS_DATA = "Mah.";
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.Cadde))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = adress.Cadde;
                //            addressList.Add(address);
                //        }
                //        else if (!String.IsNullOrEmpty(adress.Sokak))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Sokak;
                //            address.ADDRESS_DATA = adress.Sokak;
                //            addressList.Add(address);
                //        }
                //        else
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Cadde;
                //            address.ADDRESS_DATA = "Cad.";
                //            addressList.Add(address);
                //        }
                //        if (!String.IsNullOrEmpty(adress.Apartman))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.Apartmani;
                //            address.ADDRESS_DATA = adress.Apartman;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.BinaNo))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.BinaNo;
                //            address.ADDRESS_DATA = adress.BinaNo;
                //            addressList.Add(address);
                //        }

                //        if (!String.IsNullOrEmpty(adress.DaireNo))
                //        {
                //            address = new ak.common.Address();
                //            address.ADDRESS_TYPE = AK_AdresKodlari.DaireNo;
                //            address.ADDRESS_DATA = adress.DaireNo;
                //            addressList.Add(address);
                //        }

                //        unit.ADDRESS_LIST = addressList.ToArray();

                //        if (SEGenelBilgiler.KimlikNo.Length == 11)
                //        {
                //            unit.IDENTITY_NO = Convert.ToInt64(SEGenelBilgiler.KimlikNo);
                //            unit.PERSONAL_COMMERCIAL = "O";
                //        }
                //        else
                //        {
                //            unit.TAX_NO = SEGenelBilgiler.KimlikNo;
                //            unit.PERSONAL_COMMERCIAL = "T";
                //        }

                //        //unit.PASSPORT_NO = 0;
                //        unit.NAME = SEGenelBilgiler.AdiUnvan;
                //        unit.SURNAME = SEGenelBilgiler.SoyadiUnvan;
                //        if (SEGenelBilgiler.DogumTarihi.HasValue)
                //        {
                //            unit.BIRTHDATE = SEGenelBilgiler.DogumTarihi.Value;
                //        }
                //        unit.FIRM_NAME = "";
                //        unit.FATHER_NAME = "";
                //        unit.MARITAL_STATUS = "";
                //        unit.BIRTH_PLACE = "";
                //        // unit.OCCUPATION = Ak_KaskoSoruCevapTipleri.Diger;
                //        unit.NATIONALITY = 1;

                //        if (evTel != null)
                //        {
                //            unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                //            unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                //            unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                //        }
                //        if (cepTel != null)
                //        {
                //            unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                //            unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                //            unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                //        }
                //        unit.EMAIL_ADDRESS = SEGenelBilgiler.EMail;
                //        unit.GENDER = SEGenelBilgiler.Cinsiyet;

                //        return unit;
                //    }
                //    catch (Exception)
                //    {
                //        string hataMesaji = "";
                //        if (!string.IsNullOrEmpty(il.Description))
                //            hataMesaji += il.Description;
                //        if (!string.IsNullOrEmpty(ilceler.Description))
                //            hataMesaji += ilceler.Description;
                //        if (String.IsNullOrEmpty(hataMesaji))
                //            hataMesaji = "Bir hata oluştu.";
                //        throw new Exception(hataMesaji);
                //    }
                //}

                //private ak.common.Policy GetPolicy(ITeklif teklif)
                //{
                //    ak.common.Policy policy = new ak.common.Policy()
                //    {
                //        ProductName = ak.common.ProductName.BireyselKasko,
                //        CompanyCode = 0,
                //        EndorsNr = 0,
                //        FirmCode = 0,
                //        RenewalNr = 0
                //    };
                //    string TUMTeklifNo = this.ReadWebServisCevap(Common.WebServisCevaplar.AK_Teklif_Police_No, "0");
                //    policy.PolicyNumber = Convert.ToInt32(TUMTeklifNo);
                //    return policy;
                //}

                //private ak.common.PaymentInfo GetPaymentInfo(Odeme odeme)
                //{
                //    ak.common.PaymentInfo paymentInfo = new ak.common.PaymentInfo()
                //    {
                //        AccountBankNo = 0,
                //        AccountBranchCode = 0,
                //        AccountNo = 0,
                //        AccountOwnerName = "",
                //        CreditCardCvv = odeme.KrediKarti.CVC,
                //        CreditCardEndMonth = odeme.KrediKarti.SKA,
                //        CreditCardEndYear = odeme.KrediKarti.SKY,
                //        CreditCardNameSurname = odeme.KrediKarti.KartSahibi,
                //        CreditCardNumber = odeme.KrediKarti.KartNo,
                //        Installment = odeme.TaksitSayisi,
                //        PaymentType = ak.common.PaymentMethod.WithCreditCard,

                //    };

                //    return paymentInfo;
                //}

                //private ak.common.Policy GetPolicyPDF(ITeklif teklif)
                //{
                //    ak.common.Policy policy = new ak.common.Policy()
                //    {
                //        PolicyNumber = Convert.ToInt32(teklif.GenelBilgiler.TUMPoliceNo),
                //        ProductName = ak.common.ProductName.BireyselKasko,
                //        CompanyCode = 0,
                //        EndorsNr = 0,
                //        FirmCode = 0,
                //        RenewalNr = 0
                //    };

                //    return policy;
                //}

                //private string GetPDFURL(ak.common.Common client, ak.common.ExtendConfirmParameters confirmRequest, ak.common.PrintoutType type)
                //{
                //    string url = String.Empty;
                //    ak.common.PrintResponse response = client.Basim(confirmRequest.Policy, type);
                //    client.Dispose();
                //    if (!response.success)
                //    {
                //        this.AddHata(response.description);
                //    }
                //    else
                //    {
                //        url = response.downloadurl;
                //    }

                //    return url;
                //}

                //public override void DekontPDF()
                //{
                //    AkDekontRequest request = new AkDekontRequest();
                //    try
                //    {

                //        #region Yeni ws
                //        ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                //        KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleAKCommonV2);
                //        var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AK });
                //        ak.common.ExtendConfirmParameters confirmRequest = new ak.common.ExtendConfirmParameters();
                //        ak.common.Common client = new ak.common.Common();
                //        client.Url = konfigCommon[Konfig.AK_CommonServiceURLV2]; ;
                //        client.Timeout = 150000;

                //        client.IdentityHeaderValue = new ak.common.IdentityHeader()
                //        {
                //            KullaniciAdi = servisKullanici.KullaniciAdi,
                //            KullaniciParola = servisKullanici.Sifre,
                //            KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                //            KullaniciTipi = ak.common.ClientType.ACENTE,
                //        };

                //        #region Service call
                //        ak.common.PrintResponse response = client.Basim(confirmRequest.Policy, ak.common.PrintoutType.Slip);

                //        #endregion

                //        #endregion              

                //        #region Hata Kontrol ve Kayıt
                //        if (!response.success)
                //        {
                //            this.AddHata(response.description);
                //        }
                //        else
                //        {
                //            var policeKrediKartiSlipPDF = response.downloadurl;
                //            if (policeKrediKartiSlipPDF != null)
                //            {
                //                WebClient myClient = new WebClient();
                //                byte[] data = myClient.DownloadData(policeKrediKartiSlipPDF);

                //                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                //                string fileName = String.Empty;
                //                string url = String.Empty;

                //                fileName = String.Format("sompo_dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                //                url = storage.UploadFile("kasko", fileName, data);
                //                this.GenelBilgiler.PDFDekont = url;

                //                _Log.Info("Dekont_PDF url: {0}", url);
                //                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //            }
                //            else
                //            {
                //                this.AddHata("PDF dosyası alınamadı.");
                //                return;
                //            }
                //        }
                //        #endregion
                //    }
                //    catch (Exception ex)
                //    {
                //        _Log.Error("AkKasko.DekontPDF", ex);

                //        this.EndLog(ex.Message, false);
                //        this.AddHata(ex.Message);
                //    }
                //}
            }
            catch
            {

            }

        
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
            //gonderilenIp= "81.214.50.9";//MB Grup Sigorta
            //gonderilenIp = "88.249.80.115";//Tuğra Sigorta
            return gonderilenIp;


        }
    }
    public class Request
    {
        [XmlElement]
        public kaskoTeklifOlusturmaInput kaskoTeklifOlusturmaInput { get; set; }



    }

}
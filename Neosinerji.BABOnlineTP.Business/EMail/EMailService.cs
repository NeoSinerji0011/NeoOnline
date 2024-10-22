using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI;
using ilceler = Neosinerji.BABOnlineTP.Business.HDI.HDIIlcelerResponse;
using beldeler = Neosinerji.BABOnlineTP.Business.HDI.HDIBeldelerResponse;

namespace Neosinerji.BABOnlineTP.Business
{
    public class EMailService : IEMailService
    {
        ITVMContext _TVMContext;
        IKonfigurasyonService _KonfigurasyonService;
        IEPostaService _EPostaService;
        ITeklifService _TeklifService;
        ITeklifContext _TeklifContext;

        public EMailService(ITVMContext TVMContext, IKonfigurasyonService KonfigurasyonService, IEPostaService EPostaService,
                            ITeklifService teklifService, ITeklifContext TeklifContext)
        {
            _TVMContext = TVMContext;
            _KonfigurasyonService = KonfigurasyonService;
            _EPostaService = EPostaService;
            _TeklifService = teklifService;
            _TeklifContext = TeklifContext;

        }
        public void SendYeniKullaniciEMail(TVMKullanicilar kullanici, string password, string projeKodu)
        {
            string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
            Hashtable data = new Hashtable();
            data.Add("$NameSurName$", name);
            data.Add("$UserName$", kullanici.Email);
            data.Add("$Password$", password);
            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(kullanici.Email, name);
            if (projeKodu == TVMProjeKodlari.Lilyum)
            {
                EPostaFormatlari format = _EPostaService.GetEPosta("LilyumYeniMusteriKullanici");
                SendOnlyEmail(format, data, to, TVMProjeKodlari.Lilyum);
            }
            else
            {
                SendEMail("YeniKullanici", data, to);
            }

        }
        public void SendSigortaliYeniKullaniciEMail(TVMKullanicilar kullanici, string password)
        {
            string aktive = "";
            if (kullanici.EmailOnayKodu != null)
            {
                string confirmationGuid = kullanici.EmailOnayKodu;
                string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
                               "/Common/Verify?EmailKontrolKodu=" +
                               confirmationGuid;
                aktive = "Üyeliğiniz başarıyla oluşturulmuştur. Aşağıdaki linke tıkladığınızda hesabınız aktif olacaktır.\n";
                aktive += verifyUrl;
            }

            string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
            Hashtable data = new Hashtable();
            data.Add("$NameSurName$", name);
            data.Add("$UserName$", kullanici.Email);
            data.Add("$Password$", password);
            data.Add("$Aktivasyon$", aktive);
            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(kullanici.Email, name);

            SendEMail("SigortaliYeniKullanici", data, to);
        }

        public void SendLilyumYeniKullaniciEMail(TVMKullanicilar kullanici, string password)
        {
            string aktive = "";
            if (kullanici.EmailOnayKodu != null)
            {
                string confirmationGuid = kullanici.EmailOnayKodu;
                string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
                               "/Common/Verify?EmailKontrolKodu=" +
                               confirmationGuid;
                aktive = "Üyeliğiniz başarıyla oluşturulmuştur. Aşağıdaki linke tıkladığınızda hesabınız aktif olacaktır.\n";
                aktive += verifyUrl;
            }

            string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
            Hashtable data = new Hashtable();
            data.Add("$NameSurName$", name);
            data.Add("$UserName$", kullanici.Email);
            data.Add("$Password$", password);
            data.Add("$Aktivasyon$", aktive);
            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(kullanici.Email, name);

            //SendEMail("LilyumYeniMusteriKullanici", data, to);
            EPostaFormatlari format = _EPostaService.GetEPosta("LilyumYeniMusteriKullanici");
            SendOnlyEmail(format, data, to, TVMProjeKodlari.Lilyum);
        }
        public void SendLilyumBilgilendirme(string  adSoyad, string referansNo , string yapilanOdeme, string OdemeDurumu, string email,string odemeTutari)
        {
            //odemeTutari korudan oluşmamışlar için kullanılacak
            Hashtable data = new Hashtable();
            data.Add("$NameSurName$", adSoyad);
            if(OdemeDurumu== "Başarılı " && yapilanOdeme != "")
            {
                data.Add("$YapilanOdeme$", yapilanOdeme);
                data.Add("$ReferansNo$", referansNo);
            }
            else if  (OdemeDurumu == "Başarılı " && yapilanOdeme == "")
            {
                data.Add("$YapilanOdeme$", odemeTutari);
            }
            data.Add("$OdemeDurumu$", OdemeDurumu);
            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, adSoyad);
            //SendEMail("LilyumYeniMusteriKullanici", data, to);
            EPostaFormatlari format = new EPostaFormatlari();
            if (OdemeDurumu== "Başarılı " && yapilanOdeme != "")
            {
              format = _EPostaService.GetEPosta("BasariliLilyumKart");
            }
            else if (OdemeDurumu == "Başarılı " && yapilanOdeme == "")
            {
                format = _EPostaService.GetEPosta("BasariliKoruOlusmamisLilyumKart");
            }
            else
            {
                format = _EPostaService.GetEPosta("BasarisizLilyumkart");
            }
            SendOnlyEmail(format, data, to, TVMProjeKodlari.Lilyum);
        }
        public bool SendYeniKullaniciEMail_Update(TVMKullanicilar kullanici, string password, string projeKodu)
        {
            bool result = false;

            try
            {
                MailClientModel model = new MailClientModel();
                string link = String.Empty;
                Uri uri = HttpContext.Current.Request.Url;

                switch (projeKodu)
                {
                    case TVMProjeKodlari.Aegon:
                        model.Baslik = "Aegon Emeklilik ve Hayat A.Ş acente kullanıcı hesabınız oluşturulmuştur. Kullanıcı bilgileriniz aşağıdadır.";
                        model.LogoUrl = AegonCommon.LogoURL;
                        model.LogoSrc = AegonCommon.LogoSrc;
                        model.LogoAlt = "Aegon Logo";
                        model.Aciklama = "Şifrenizle giriş yaptıktan sonra sistem sizi sifre değiştirme sayfasına yönlendirecektir.";
                        model.Kullanici = "Partaj No: " + kullanici.TeknikPersonelKodu;
                        model.Sifre = "Sifre: " + password;
                        link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], "Aegon/Login/");
                        break;
                    case TVMProjeKodlari.Mapfre:
                        model.Baslik = "Mapfre Genel Sigorta acente kullanıcı hesabınız oluşturulmuştur. Kullanıcı bilgileriniz aşağıdadır.";
                        model.LogoUrl = MapfreCommon.LogoURL;
                        model.LogoSrc = MapfreCommon.LogoSrc;
                        model.LogoAlt = "Mapfre Logo";
                        model.Aciklama = "Şifrenizle giriş yaptıktan sonra sistem sizi sifre değiştirme sayfasına yönlendirecektir.";
                        model.Kullanici = "Email : " + kullanici.Email;
                        model.Sifre = "Sifre: " + password;
                        link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], "Mapfre/Login/");
                        break;
                }

                string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
                Hashtable data = new Hashtable();
                data.Add("$AdSoyad$", name);
                data.Add("$Baslik$", model.Baslik);
                data.Add("$Aciklama$", model.Aciklama);
                data.Add("$Kullanici$", model.Kullanici);
                data.Add("$Sifre$", model.Sifre);
                data.Add("$logo-url$", model.LogoUrl);
                data.Add("$logo-src$", model.LogoSrc);
                data.Add("$logo-alt$", model.LogoSrc);
                data.Add("$link$", link);

                Dictionary<string, string> to = new Dictionary<string, string>();
                to.Add(kullanici.Email, name);

                EPostaFormatlari format = _EPostaService.GetEPosta("EmailYeniKullanici");

                if (format != null)
                    result = SendOnlyEmail(format, data, to, projeKodu);
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        public void SendSifreYenileEMail(TVMKullanicilar kullanici, string password)
        {
            string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
            Hashtable data = new Hashtable();
            data.Add("$Name$", name);
            data.Add("$Password$", password);

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(kullanici.Email, name);

            SendEMail("SifreSifirlama", data, to);
        }

        public bool SendSifreYenileEMail_Update(TVMKullanicilar kullanici, string password, string projeKodu)
        {
            bool result = false;

            try
            {
                MailClientModel model = new MailClientModel();
                string link = String.Empty;
                Uri uri = HttpContext.Current.Request.Url;

                switch (projeKodu)
                {
                    case TVMProjeKodlari.Aegon:
                        model.Baslik = kullanici.Adi + " " + kullanici.Soyadi;
                        model.LogoUrl = AegonCommon.LogoURL;
                        model.LogoSrc = AegonCommon.LogoSrc;
                        model.LogoAlt = "Aegon Logo";
                        link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], "Aegon/Login/");
                        break;
                    case TVMProjeKodlari.Mapfre:
                        model.Baslik = kullanici.Adi + " " + kullanici.Soyadi;
                        model.LogoUrl = MapfreCommon.LogoURL;
                        model.LogoSrc = MapfreCommon.LogoSrc;
                        model.LogoAlt = "Mapfre Logo";
                        link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], "Mapfre/Login/");
                        break;
                }

                Hashtable data = new Hashtable();
                data.Add("$AdSoyad$", model.Baslik);
                data.Add("$Sifre$", password);
                data.Add("$logo-url$", model.LogoUrl);
                data.Add("$logo-src$", model.LogoSrc);
                data.Add("$logo-alt$", model.LogoAlt);
                data.Add("$link$", link);

                Dictionary<string, string> to = new Dictionary<string, string>();
                to.Add(kullanici.Email, model.Baslik);

                EPostaFormatlari format = _EPostaService.GetEPosta("YeniSifreCommon");

                result = SendOnlyEmail(format, data, to, projeKodu);
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        public void SendSifreYenileLink(TVMKullanicilar kullanici, string link)
        {
            string name = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);
            Hashtable data = new Hashtable();
            data.Add("$Name$", name);
            data.Add("$tiklayiniz$", String.Format("<a href='{0}'>tıklayınız</a>", link));

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(kullanici.Email, name);

            SendEMail("SifreYenilemeTalebi", data, to);
        }

        public bool SendSifreYenileLink_Update(TVMKullanicilar kullanici, string link, string projeKodu)
        {
            bool result = false;

            try
            {
                MailClientModel model = GetClientInfo(kullanici, projeKodu);

                Hashtable data = new Hashtable();
                data.Add("$logo-url$", model.LogoUrl);
                data.Add("$logo-src$", model.LogoSrc);
                data.Add("$logo-alt$", model.LogoAlt);
                data.Add("$Baslik$", model.Baslik);
                data.Add("$IsletimSistemi$", model.OperatinSystem);
                data.Add("$link$", link);

                Dictionary<string, string> to = new Dictionary<string, string>();
                to.Add(kullanici.Email, String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi));

                EPostaFormatlari format = _EPostaService.GetEPosta("SifremiUnuttumCommon");

                result = SendOnlyEmail(format, data, to, projeKodu);
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        #region Urun Email Method

        public bool GenelUrunMailGonder(ITeklifBase teklifBase, string digerAdSoyad, string digerEmail)
        {
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            string name = String.Empty;
            string teklifSahibi = String.Empty;
            string email = String.Empty;
            string tiklayiniz = String.Empty;
            string musteriAdiSoyadi = String.Empty;
            string urunAdi = String.Empty;

            string header = String.Empty;
            string content = String.Empty;
            string footer = String.Empty;

            string LogoUrl = "";
            string LogoSrc = "";
            string LogoAlt = "";
            bool isPolice = false;

            ITeklif teklif = teklifBase.Teklif;
            string belgeNo = teklif.GenelBilgiler.TeklifNo.ToString();
            string kayitTarihi = teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy");
            isPolice = teklif.GenelBilgiler.TeklifDurumKodu == TeklifDurumlari.Police;
            string pdfURL = "";
            if (isPolice)
            {
                var tumTeklif = teklifBase.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON);
                belgeNo = teklif.GenelBilgiler.TUMPoliceNo;
                pdfURL = tumTeklif.GenelBilgiler.PDFPolice;
            }
            else
            {
                if (teklif.GenelBilgiler.UrunKodu == UrunKodlari.TamamlayiciSaglik)
                {
                    var tumTeklif = teklifBase.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON);
                    if (tumTeklif != null)
                    {
                        ITeklif t = _TeklifService.GetTeklif(tumTeklif.GenelBilgiler.TeklifId);
                        belgeNo = tumTeklif.GenelBilgiler.TUMTeklifNo;
                        pdfURL = tumTeklif.GenelBilgiler.PDFDosyasi;
                    }

                }
                else
                {
                    var tumTeklif = teklifBase.TUMTeklifler.FirstOrDefault(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE);
                    if (tumTeklif != null)
                    {
                        ITeklif t = _TeklifService.GetTeklif(tumTeklif.GenelBilgiler.TeklifId);
                        belgeNo = t.ReadWebServisCevap(WebServisCevaplar.MAPFRE_Teklif_Police_No, belgeNo);
                    }
                }

            }

            MusteriGenelBilgiler musteri = teklif.SigortaEttiren.MusteriGenelBilgiler;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail) && musteri != null)
            {
                name = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);
                teklifSahibi = name;
                if (musteri.EMail == null) return false;
                email = musteri.EMail;

                header = " &nbsp; &nbsp;  Sayın " + name + " " + kayitTarihi + " tarihinde hazırlanan " +
                           belgeNo + " numaralı ";
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
                teklifSahibi = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

                header = " &nbsp; &nbsp;   Sayın " + name + " " + kayitTarihi + " tarihinde " + teklifSahibi + " için hazırlanan " +
                           belgeNo + " numaralı ";
            }

            Hashtable data = new Hashtable();

            EPostaFormatlari format = _EPostaService.GetEPosta("GenelTeklifMailFormat");

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);
            if (teklif.GenelBilgiler.UrunKodu == UrunKodlari.TamamlayiciSaglik)
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", pdfURL);
            }

            switch (teklif.UrunKodu)
            {
                case UrunKodlari.MapfreKasko:
                    #region MapfreKasko
                    urunAdi = "Mapfre Kasko Sigortası";
                    LogoUrl = MapfreCommon.LogoURL;
                    LogoSrc = MapfreCommon.EmailLogoSrc;
                    LogoAlt = "Mapfre Logo";

                    if (!isPolice)
                    {
                        format.Konu = "Mapfre Kasko Sigorta Teklifi";
                        header = header + " Kasko Sigortası teklifi ekte yer almaktadır.";

                        content = String.Format("Teklife ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", teklif.GenelBilgiler.PDFDosyasi);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "MapfreKasko.pdf"
                        });
                    }
                    else
                    {
                        format.Konu = "Mapfre Kasko Sigorta Poliçesi";
                        header = header + " Kasko Sigortası poliçesi ekte yer almaktadır.";

                        content = String.Format("Poliçeye ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", teklif.GenelBilgiler.PDFPolice);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice),
                            FileName = "MapfreKaskoPolice.pdf"
                        });
                    }


                    footer = MailHelper.GetAracDegerler(teklif, _TeklifService, isPolice, belgeNo);
                    #endregion
                    break;
                case UrunKodlari.MapfreTrafik:
                    #region MapfreTrafik
                    urunAdi = "Mapfre Trafik Sigortası";
                    LogoUrl = MapfreCommon.LogoURL;
                    LogoSrc = MapfreCommon.EmailLogoSrc;
                    LogoAlt = "Mapfre Logo";

                    if (!isPolice)
                    {
                        format.Konu = "Mapfre Trafik Sigorta Teklifi";
                        header = header + " Trafik Sigortası teklifi ekte yer almaktadır.";

                        content = String.Format("Teklife ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", teklif.GenelBilgiler.PDFDosyasi);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "MapfreTrafik.pdf"
                        });
                    }
                    else
                    {
                        format.Konu = "Mapfre Trafik Sigorta Poliçesi";
                        header = header + " Trafik Sigortası poliçesi ekte yer almaktadır.";

                        content = String.Format("Poliçeye ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", teklif.GenelBilgiler.PDFPolice);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice),
                            FileName = "MapfreTrafikPolice.pdf"
                        });
                    }

                    footer = MailHelper.GetAracDegerler(teklif, _TeklifService, isPolice, belgeNo);
                    #endregion
                    break;

                case UrunKodlari.TamamlayiciSaglik:
                    #region MapfreTrafik
                    urunAdi = "Sağlığınız Bizde- HMO Sigortası";
                    LogoUrl = teklif.GenelBilgiler.TVMDetay.Logo;
                    LogoSrc = System.Web.HttpContext.Current.Request.RawUrl;
                    LogoAlt = "Türk Nippon Logo";

                    if (!isPolice)
                    {
                        format.Konu = "Türk Nippon Sağlığınız Bizde-HMO Sigorta Teklifi";
                        header = header + " Türk Nippon Sağlığınız Bizde-HMO Sigortası teklifi ekte yer almaktadır.";

                        content = String.Format("Teklife ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", pdfURL);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(pdfURL),
                            FileName = "TurkNipponSagliginizBizdeHMOTeklif.pdf"
                        });
                    }
                    else
                    {
                        format.Konu = "Türk Nippon Sağlığınız Bizde-HMO Sigorta Poliçesi";
                        header = header + " Türk Nippon Sağlığınız Bizde-HMO poliçesi ekte yer almaktadır.";

                        content = String.Format("Poliçeye ulaşmak için lütfen <a href='{0}'>tıklayınız.</a>", pdfURL);
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(pdfURL),
                            FileName = "TurkNipponSagliginizBizdeHMOPolice.pdf"
                        });
                    }

                    footer = "";
                    #endregion
                    break;
            }



            data.Add("$logo-url$", LogoUrl);
            data.Add("$logo-src$", LogoSrc);
            data.Add("$logo-alt$", LogoAlt);
            data.Add("$Header$", header);
            data.Add("$Content$", content);
            data.Add("$Footer$", footer);

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;

            result = SendEMailPDF(teklif, format, data, to, attachments);

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "TeklifPoliceMail", email, result);

            return result;
        }

        public void SendTrafikTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            try
            {
                #region Fields and Props

                string name = String.Empty;
                string email = String.Empty;
                string logonAddress = System.Web.HttpContext.Current.Request.RawUrl;
                string tiklayiniz = String.Empty;
                string aracMarkaTip = String.Empty;
                bool result = false;

                Hashtable data = new Hashtable();
                List<FileAtachmentList> attachments = new List<FileAtachmentList>();
                AracMarka aracMarka = new AracMarka();
                AracTip aracTip = new AracTip();
                IAracService aracService = DependencyResolver.Current.GetService<IAracService>();
                System.Net.WebClient webClient = new System.Net.WebClient();

                #endregion

                #region Set

                // ==== AD SOYAD
                if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
                {
                    name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                    email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
                }
                else
                {
                    name = digerAdSoyad;
                    email = digerEmail;
                }


                if (teklif.GenelBilgiler.TUMKodu == 0)
                {
                    aracMarka = aracService.GetAracMarka(teklif.Arac.Marka);
                    aracTip = aracService.GetAracTip(teklif.Arac.Marka, teklif.Arac.AracinTipi);
                    data.Add("$RegistrationDate$", teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                    data.Add("$Plate$", String.Format("{0} {1}", teklif.Arac.PlakaKodu, teklif.Arac.PlakaNo));
                    tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);

                    attachments.Add(new FileAtachmentList() { File = webClient.DownloadData(teklif.GenelBilgiler.PDFDosyasi), FileName = "TrafikSigortasiTeklifi.pdf" });
                }
                else
                {
                    ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
                    aracMarka = aracService.GetAracMarka(anaTeklif.Arac.Marka);
                    aracTip = aracService.GetAracTip(anaTeklif.Arac.Marka, anaTeklif.Arac.AracinTipi);
                    data.Add("$RegistrationDate$", anaTeklif.Arac.TrafikTescilTarihi.HasValue ? anaTeklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                    data.Add("$Plate$", String.Format("{0} {1}", anaTeklif.Arac.PlakaKodu, anaTeklif.Arac.PlakaNo));
                    tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);

                    attachments.Add(new FileAtachmentList() { File = webClient.DownloadData(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });
                }

                aracMarkaTip = String.Format("{0} - {1}", aracMarka.MarkaAdi, aracTip.TipAdi);

                data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
                data.Add("$LogonAddress$", logonAddress);
                data.Add("$tiklayiniz$", tiklayiniz);
                data.Add("$Name$", name);
                data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
                data.Add("$VehicleTypeName$", aracMarkaTip);
                data.Add("$Urun$", "Trafik Sigortası");

                Dictionary<string, string> to = new Dictionary<string, string>();
                to.Add(email, name);


                #endregion

                #region Send And Log

                if (teklif.GenelBilgiler.TUMKodu == 0)
                {
                    EPostaFormatlari format = _EPostaService.GetEPosta("TrafikTeklif");
                    result = SendEMailPDF(teklif, format, data, to, attachments);
                }
                else
                {
                    ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();
                    data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);

                    EPostaFormatlari format = _EPostaService.GetEPosta("Police");
                    result = SendEMailPDF(teklif, format, data, to, attachments);
                }

                TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "TrafikTeklif", email, result);

                #endregion
            }
            catch (Exception)
            {
            }
        }

        public void SendKaskoTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            string name = String.Empty;
            string email = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;
            string tiklayiniz = String.Empty;
            string aracMarkaTip = String.Empty;
            Hashtable data = new Hashtable();


            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();
            AracMarka aracMarka = new AracMarka();
            AracTip aracTip = new AracTip();

            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                aracMarka = aracService.GetAracMarka(teklif.Arac.Marka);
                aracTip = aracService.GetAracTip(teklif.Arac.Marka, teklif.Arac.AracinTipi);
                data.Add("$RegistrationDate$", teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                data.Add("$Plate$", String.Format("{0} {1}", teklif.Arac.PlakaKodu, teklif.Arac.PlakaNo));
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);
            }
            else
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
                aracMarka = aracService.GetAracMarka(anaTeklif.Arac.Marka);
                aracTip = aracService.GetAracTip(anaTeklif.Arac.Marka, anaTeklif.Arac.AracinTipi);
                data.Add("$RegistrationDate$", anaTeklif.Arac.TrafikTescilTarihi.HasValue ? anaTeklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                data.Add("$Plate$", String.Format("{0} {1}", anaTeklif.Arac.PlakaKodu, anaTeklif.Arac.PlakaNo));
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);
            }

            aracMarkaTip = String.Format("{0} - {1}", aracMarka.MarkaAdi, aracTip.TipAdi);

            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$Name$", name);
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$VehicleTypeName$", aracMarkaTip);


            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;
            if (teklif.TUMKodu == 0)
            {
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi), FileName = "KaskoSigortasiTeklifi.pdf" });
                EPostaFormatlari format = _EPostaService.GetEPosta("KaskoTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                switch (teklif.UrunKodu)
                {
                    case UrunKodlari.TrafikSigortasi:
                    case UrunKodlari.MapfreTrafik:
                        data.Add("$Urun$", "Trafik Sigortası");
                        break;
                    case UrunKodlari.KaskoSigortasi:
                    case UrunKodlari.MapfreKasko:
                        data.Add("$Urun$", "Kasko Sigortası");
                        break;
                    default:
                        break;
                }
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });
                EPostaFormatlari format = _EPostaService.GetEPosta("Police");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "KaskoTeklif", email, result);
        }

        public void SendDaskTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            string name = String.Empty;
            string email = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;

            string aracMarkaTip = String.Empty;
            Hashtable data = new Hashtable();

            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            string ilText = String.Empty;
            string ilceText = String.Empty;
            string beldeText = String.Empty;

            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                if (teklif.RizikoAdresi.IlKodu.HasValue)
                {
                    DaskIl il = _CRService.GetDaskIl(teklif.RizikoAdresi.IlKodu.Value);
                    if (il != null)
                        ilText = il.IlAdi;
                }

                if (teklif.RizikoAdresi.IlceKodu.HasValue)
                {
                    HDIIlcelerResponse ilcemodel = _HDIDask.GetUAVTIlcelerList(teklif.RizikoAdresi.IlKodu.Value);
                    if (ilcemodel != null)
                    {
                        ilceler.KAYIT ilce = ilcemodel.KAYITLAR.Where(s => s.Kod == teklif.RizikoAdresi.IlceKodu.Value.ToString()).FirstOrDefault();
                        if (ilce != null)
                            ilceText = ilce.Aciklama;
                    }
                }
                if (!String.IsNullOrEmpty(teklif.RizikoAdresi.SemtBelde))
                {
                    HDIBeldelerResponse beldemodel = _HDIDask.GetUAVTBeldelerList(teklif.RizikoAdresi.IlceKodu.Value);
                    if (beldemodel != null)
                    {
                        beldeler.KAYIT belde = beldemodel.KAYITLAR.Where(s => s.Kod == teklif.RizikoAdresi.SemtBelde).FirstOrDefault();
                        if (belde != null)
                            beldeText = belde.Aciklama;
                    }
                }
            }
            else
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

                if (anaTeklif.RizikoAdresi.IlKodu.HasValue)
                {
                    DaskIl il = _CRService.GetDaskIl(anaTeklif.RizikoAdresi.IlKodu.Value);
                    if (il != null)
                        ilText = il.IlAdi;
                }

                if (anaTeklif.RizikoAdresi.IlceKodu.HasValue)
                {
                    HDIIlcelerResponse ilcemodel = _HDIDask.GetUAVTIlcelerList(anaTeklif.RizikoAdresi.IlKodu.Value);
                    if (ilcemodel != null)
                    {
                        ilceler.KAYIT ilce = ilcemodel.KAYITLAR.Where(s => s.Kod == anaTeklif.RizikoAdresi.IlceKodu.Value.ToString()).FirstOrDefault();
                        if (ilce != null)
                            ilceText = ilce.Aciklama;
                    }
                }
                if (!String.IsNullOrEmpty(anaTeklif.RizikoAdresi.SemtBelde))
                {
                    HDIBeldelerResponse beldemodel = _HDIDask.GetUAVTBeldelerList(anaTeklif.RizikoAdresi.IlceKodu.Value);
                    if (beldemodel != null)
                    {
                        beldeler.KAYIT belde = beldemodel.KAYITLAR.Where(s => s.Kod == anaTeklif.RizikoAdresi.SemtBelde).FirstOrDefault();
                        if (belde != null)
                            beldeText = belde.Aciklama;
                    }
                }
            }


            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);

            data.Add("$Name$", name);
            data.Add("$Date$", teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy"));
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$Il$", ilText);
            data.Add("$Ilce$", ilceText);
            data.Add("$Belde$", beldeText);

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;

            if (teklif.TUMKodu == 0)
            {
                string tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);
                data.Add("$tiklayiniz$", tiklayiniz);

                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi), FileName = "DaskSigortasiTeklifi.pdf" });
                EPostaFormatlari format = _EPostaService.GetEPosta("DaskTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolNo$", teklif.GenelBilgiler.TUMPoliceNo);

                string tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);
                data.Add("$tiklayiniz$", tiklayiniz);

                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });
                EPostaFormatlari format = _EPostaService.GetEPosta("DaskPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "DaskTeklif", email, result);
        }

        public void SendKrediHayat(IKrediliHayatTeklif krediHayat, string digerAdSoyad, string digerEmail)
        {
            string name = String.Empty;
            string email = String.Empty;

            ITeklif teklif = krediHayat.Teklif;
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", anaTeklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, anaTeklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = anaTeklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            Hashtable data = new Hashtable();

            IMusteriService musteriService = DependencyResolver.Current.GetService<IMusteriService>();

            TeklifSigortali teklifSigortali = anaTeklif.Sigortalilar.FirstOrDefault();
            MusteriGenelBilgiler sigortali = musteriService.GetMusteri(teklifSigortali.MusteriKodu);

            ITeklif primTeklif = krediHayat.TUMTeklifler.Where(w => w.TUMKodu == TeklifUretimMerkezleri.DEMIR).FirstOrDefault();

            string logonAddress = System.Web.HttpContext.Current.Request.RawUrl;
            string babaAdi = anaTeklif.ReadSoru(KrediliHayatSorular.Baba_Adi, String.Empty);
            decimal krediTutari = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Tutari, decimal.Zero);
            string krediSuresi = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Suresi, String.Empty);
            string krediTuru = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Turu, String.Empty);
            string krediTuruText = String.Empty;
            int krediTuruInt = 0;


            if (!String.IsNullOrEmpty(krediTuru) && int.TryParse(krediTuru, out krediTuruInt))
            {
                switch (krediTuruInt)
                {
                    case KrediTurleri.Araba: krediTuruText = "Araba"; break;
                    case KrediTurleri.Konut: krediTuruText = "Konut"; break;
                    case KrediTurleri.KrediKarti: krediTuruText = "Kredi Kartı"; break;
                    case KrediTurleri.KrediMevduat: krediTuruText = "Kredi Mevduat"; break;
                    case KrediTurleri.Tuketici: krediTuruText = "Tüketici"; break;
                    case KrediTurleri.CekKarnesi: krediTuruText = "Çek Karnesi"; break;
                }
            }

            int krediSuresiInt = 1;
            int.TryParse(krediSuresi, out krediSuresiInt);

            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$Name$", name);
            data.Add("$ProposalNo$", teklif.TeklifNo);
            data.Add("$CreditType$", krediTuruText);
            data.Add("$CustomerName$", name);
            data.Add("$BirthDate$", sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("dd.MM.yyyy") : "-");
            data.Add("$FatherName$", babaAdi);
            data.Add("$PolicyStartDate$", teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy"));
            data.Add("$PolicyEndDate$", teklif.GenelBilgiler.BitisTarihi.ToString("dd.MM.yyyy"));
            data.Add("$YasamKaybiTeminati$", krediTutari.ToString("N2") + " TL");
            data.Add("$Prim$", primTeklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL");

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;
            if (teklif.TUMKodu == 0)
                result = SendEMail("KrediliHayatTeklif", data, to);
            else
            {
                switch (teklif.UrunKodu)
                {
                    case UrunKodlari.KrediHayat:
                        data.Add("$Urun$", "Kredi Hayat Sigorta Sertifikası");
                        break;
                }
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolicyNo$", teklif.GenelBilgiler.TUMPoliceNo);

                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();

                List<FileAtachmentList> attachments = new List<FileAtachmentList>();

                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });

                EPostaFormatlari format = _EPostaService.GetEPosta("KrediliHayatPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }


            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "KrediliHayatTeklif", email, result);
        }

        public void SendSeyahatSaglikTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            string name = String.Empty;
            string email = String.Empty;
            string tiklayiniz = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;

            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;



            string aracMarkaTip = String.Empty;
            Hashtable data = new Hashtable();

            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            string ulke = String.Empty;
            string kisiSayisi = String.Empty;


            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", anaTeklif.GenelBilgiler.PDFDosyasi);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi), FileName = "SeyahatSaglikSigortasiTeklifi.pdf" });
                string kisisayisi = teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1");

                string ulkeKodu = teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);

                if (!String.IsNullOrEmpty(ulkeKodu))
                {
                    UlkeKodlari Ulke = _CRService.GetSeyehatUlkesi(ulkeKodu);
                    if (Ulke != null)
                        ulke = Ulke.UlkeAdi;
                }
            }
            else
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });

                string kisisayisi = anaTeklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1");

                string ulkeKodu = anaTeklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);

                if (!String.IsNullOrEmpty(ulkeKodu))
                {
                    UlkeKodlari Ulke = _CRService.GetSeyehatUlkesi(ulkeKodu);
                    if (Ulke != null)
                        ulke = Ulke.UlkeAdi;
                }
            }


            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$Name$", name);
            data.Add("$Date$", teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy"));
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$Ulke$", ulke);
            data.Add("$KisiSayisi$", kisiSayisi);


            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;

            if (teklif.TUMKodu == 0)
            {
                EPostaFormatlari format = _EPostaService.GetEPosta("SeyahatTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolNo$", teklif.GenelBilgiler.TUMPoliceNo);

                EPostaFormatlari format = _EPostaService.GetEPosta("SeyahatPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "SeyahatSaglikTeklif", email, result);
        }

        public void SendKonutTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();

            string name = String.Empty;
            string email = String.Empty;
            string tiklayiniz = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;

            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;
            List<FileAtachmentList> attachments = new List<FileAtachmentList>();
            string ilText = String.Empty;
            string ilceText = String.Empty;

            Hashtable data = new Hashtable();


            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", anaTeklif.GenelBilgiler.PDFDosyasi);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi), FileName = "KonutTeklifi.pdf" });

                Il il = _UlkeService.GetIl("TUR", teklif.RizikoAdresi.IlKodu.HasValue ? teklif.RizikoAdresi.IlKodu.Value.ToString() : "0");
                if (il != null)
                    ilText = il.IlAdi;

                Ilce ilce = _UlkeService.GetIlce(teklif.RizikoAdresi.IlceKodu.HasValue ? teklif.RizikoAdresi.IlceKodu.Value : 0);
                if (ilce != null)
                    ilceText = ilce.IlceAdi;
            }
            else
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });

                Il il = _UlkeService.GetIl("TUR", anaTeklif.RizikoAdresi.IlKodu.HasValue ? anaTeklif.RizikoAdresi.IlKodu.Value.ToString() : "0");
                if (il != null)
                    ilText = il.IlAdi;

                Ilce ilce = _UlkeService.GetIlce(anaTeklif.RizikoAdresi.IlceKodu.HasValue ? anaTeklif.RizikoAdresi.IlceKodu.Value : 0);
                if (ilce != null)
                    ilceText = ilce.IlceAdi;
            }


            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$Name$", name);
            data.Add("$Date$", teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy"));
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$Il$", ilText);
            data.Add("$Ilce$", ilceText);


            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;

            if (teklif.TUMKodu == 0)
            {
                EPostaFormatlari format = _EPostaService.GetEPosta("KonutTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolNo$", teklif.GenelBilgiler.TUMPoliceNo);

                EPostaFormatlari format = _EPostaService.GetEPosta("KonutPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "KonutTeklif", email, result);
        }

        public void SendIsYeriTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            ICRService _CRService = DependencyResolver.Current.GetService<ICRService>();
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();

            string name = String.Empty;
            string email = String.Empty;
            string tiklayiniz = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;
            List<FileAtachmentList> attachments = new List<FileAtachmentList>();
            string ilText = String.Empty;
            string ilceText = String.Empty;

            Hashtable data = new Hashtable();


            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", anaTeklif.GenelBilgiler.PDFDosyasi);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi), FileName = "IsYeriTeklifi.pdf" });

                Il il = _UlkeService.GetIl("TUR", teklif.RizikoAdresi.IlKodu.HasValue ? teklif.RizikoAdresi.IlKodu.Value.ToString() : "0");
                if (il != null)
                    ilText = il.IlAdi;

                Ilce ilce = _UlkeService.GetIlce(teklif.RizikoAdresi.IlceKodu.HasValue ? teklif.RizikoAdresi.IlceKodu.Value : 0);
                if (ilce != null)
                    ilceText = ilce.IlceAdi;
            }
            else
            {
                tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFPolice);
                attachments.Add(new FileAtachmentList() { File = storage.DownloadFile(teklif.GenelBilgiler.PDFPolice), FileName = "Police.pdf" });

                Il il = _UlkeService.GetIl("TUR", anaTeklif.RizikoAdresi.IlKodu.HasValue ? anaTeklif.RizikoAdresi.IlKodu.Value.ToString() : "0");
                if (il != null)
                    ilText = il.IlAdi;

                Ilce ilce = _UlkeService.GetIlce(anaTeklif.RizikoAdresi.IlceKodu.HasValue ? anaTeklif.RizikoAdresi.IlceKodu.Value : 0);
                if (ilce != null)
                    ilceText = ilce.IlceAdi;
            }


            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$Name$", name);
            data.Add("$Date$", teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy"));
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$Il$", ilText);
            data.Add("$Ilce$", ilceText);


            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;

            if (teklif.TUMKodu == 0)
            {
                EPostaFormatlari format = _EPostaService.GetEPosta("IsYeriTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolNo$", teklif.GenelBilgiler.TUMPoliceNo);

                EPostaFormatlari format = _EPostaService.GetEPosta("IsYeriPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "IsYeriTeklif", email, result);
        }

        public void SendIkinciElGarantiTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail)
        {
            string name = String.Empty;
            string email = String.Empty;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail))
            {
                name = String.Format("{0} {1}", teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan, teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan);
                email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }
            Hashtable data = new Hashtable();

            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string logonAddress = url;
            string tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);
            string aracMarkaTip = String.Empty;

            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            //attachments.Add(storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi));
            attachments.Add(new FileAtachmentList() { File = storage.DownloadFile("https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/176/WOAK_COLLINSON_TASIT%20TURKEY_Sample%20Used%20Vehicle%20Warranty%20Program%20Booklet....pdf"), FileName = "Manual" });

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();
            AracMarka aracMarka = new AracMarka();
            AracTip aracTip = new AracTip();

            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                aracMarka = aracService.GetAracMarka(teklif.Arac.Marka);
                aracTip = aracService.GetAracTip(teklif.Arac.Marka, teklif.Arac.AracinTipi);
                data.Add("$RegistrationDate$", teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                data.Add("$Plate$", String.Format("{0} {1}", teklif.Arac.PlakaKodu, teklif.Arac.PlakaNo));
            }
            else
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
                aracMarka = aracService.GetAracMarka(anaTeklif.Arac.Marka);
                aracTip = aracService.GetAracTip(anaTeklif.Arac.Marka, anaTeklif.Arac.AracinTipi);
                data.Add("$RegistrationDate$", anaTeklif.Arac.TrafikTescilTarihi.HasValue ? anaTeklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "");
                data.Add("$Plate$", String.Format("{0} {1}", anaTeklif.Arac.PlakaKodu, anaTeklif.Arac.PlakaNo));
            }

            if (aracMarka != null && aracTip != null)
                aracMarkaTip = String.Format("{0} - {1}", aracMarka.MarkaAdi, aracTip.TipAdi);

            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$Name$", name);
            data.Add("$ProposalNo$", teklif.GenelBilgiler.TeklifNo);
            data.Add("$VehicleTypeName$", aracMarkaTip);

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;
            if (teklif.TUMKodu == 0)
            {
                EPostaFormatlari format = _EPostaService.GetEPosta("IkinciElGarantiTeklif");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }
            else
            {
                ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();

                data.Add("$SigortaSirketi$", _TUMService.GetDetay(teklif.TUMKodu).Unvani);
                data.Add("$PolicyNo$", teklif.GenelBilgiler.TUMPoliceNo);

                EPostaFormatlari format = _EPostaService.GetEPosta("IkinciElGarantiPolice");
                result = SendEMailPDF(teklif, format, data, to, attachments);
            }

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "IkinciElTeklif", email, result);
        }

        #region Aegon Ürünleri

        public bool SendAegonEMailTeklif(ITeklif teklif, string digerAdSoyad, string digerEmail, bool acenteyeMI)
        {
            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();

            string name = String.Empty;
            string email = String.Empty;
            string tiklayiniz = String.Empty;
            string urunAciklama = String.Empty;
            string musteriAdiSoyadi = String.Empty;
            string urunAdi = String.Empty;
            string acenteYetkilsiAdiSoyadi = String.Empty;

            MusteriGenelBilgiler musteri = teklif.SigortaEttiren.MusteriGenelBilgiler;

            if (String.IsNullOrEmpty(digerAdSoyad) && String.IsNullOrEmpty(digerEmail) && musteri != null)
            {
                name = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);
                if (musteri.EMail == null && !acenteyeMI) return false;
                email = musteri.EMail;
            }
            else
            {
                name = digerAdSoyad;
                email = digerEmail;
            }

            string url = "www.aegon.com.tr";
            string logonAddress = url;

            TVMKullanicilar yetkili = teklif.GenelBilgiler.TVMKullanicilar;

            if (yetkili != null)
                acenteYetkilsiAdiSoyadi = yetkili.Adi + " " + yetkili.Soyadi;

            Hashtable data = new Hashtable();


            EPostaFormatlari format = _EPostaService.GetEPosta("AegonMailFormat");

            List<FileAtachmentList> attachments = new List<FileAtachmentList>();

            tiklayiniz = String.Format("<a href='{0}'>tıklayınız</a>", teklif.GenelBilgiler.PDFDosyasi);

            switch (teklif.UrunKodu)
            {
                case UrunKodlari.TESabitPrimli:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "TuruncuElmaHayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Turuncu Elma";
                        format.Konu = "Turuncu Elma Hayat Sigortası";
                    }
                    break;
                case UrunKodlari.PrimIadeli:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "PrimIadeliHayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Prim İadeli";
                        format.Konu = "Prim İadeli Hayat Sigortası";
                    }
                    break;
                case UrunKodlari.OdulluBirikim:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "ÖdüllüBirikimHayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Ödüllü Birikim";
                        format.Konu = "Ödüllü Birikim Hayat Sigortası";
                    }
                    break;
                case UrunKodlari.Egitim:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "EgitimHayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Eğitim İçin ";
                        format.Konu = "Eğitim İçin Hayat Sigortası";
                    }
                    break;
                case UrunKodlari.OdemeGuvence:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "OdemeGuvenceSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Ödeme Güvence ";
                        format.Konu = "Ödeme Güvence Sigortası";
                    }
                    break;
                case UrunKodlari.KorunanGelecek:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "KorunanGelecekHayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Korunan Gelecek";
                        format.Konu = "Korunan Gelecek Hayat Sigortası";
                    }
                    break;
                case UrunKodlari.PrimIadeli2:
                    {
                        attachments.Add(new FileAtachmentList()
                        {
                            File = storage.DownloadFile(teklif.GenelBilgiler.PDFDosyasi),
                            FileName = "Primİadeli(5Yıllık)HayatSigortasiTeklifi.pdf"
                        });
                        urunAdi = "Prim İadeli(5Yıllık)";
                        format.Konu = "Prim İadeli (5Yıllık) Hayat Sigortası";
                    }
                    break;
            }


            urunAciklama = "AEGON Hayat ve Emeklilik A.Ş. acente yetkilisi " + acenteYetkilsiAdiSoyadi + " tarafından " + teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy") + " tarihinde hazırlanan  " + teklif.GenelBilgiler.TeklifNo + " numaralı " + urunAdi + " Hayat Sigortası teklifiniz ekte yer almaktadır.";


            if (acenteyeMI)
            {
                urunAciklama = "AEGON Hayat ve Emeklilik A.Ş. ‘den " + teklif.GenelBilgiler.KayitTarihi.ToString("dd.MM.yyyy") + " tarihinde "
                + musteri.AdiUnvan + " " + musteri.SoyadiUnvan + " adına hazırladığınız " + teklif.GenelBilgiler.TeklifNo + " numaralı " + urunAdi + " Hayat Sigortası teklifiniz ekte yer almaktadır.";

                name = acenteYetkilsiAdiSoyadi;

                if (yetkili != null)
                    email = yetkili.Email;
            }

            data.Add("$TVMLogo$", teklif.GenelBilgiler.TVMDetay.Logo);
            data.Add("$LogonAddress$", logonAddress);
            data.Add("$tiklayiniz$", tiklayiniz);
            data.Add("$AdiSoyad$", name);
            data.Add("$UrunAciklama$", urunAciklama);

            Dictionary<string, string> to = new Dictionary<string, string>();
            to.Add(email, name);

            bool result = false;



            result = SendEMailPDF(teklif, format, data, to, attachments);

            TeklifEMailLog(teklif.GenelBilgiler.TeklifId, "AegonMailFormat", email, result);

            return result;
        }

        #endregion

        #endregion

        #region Metlife Email
        public void SendMetlifeEmail(int KullaniciGrupKodu, int teklifId, int asama)
        {
            IAktifKullaniciService _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == 444).FirstOrDefault();

            List<IsTakipKullaniciGrupKullanicilari> kullaniciGrupKullanicilari = _TeklifContext.IsTakipKullaniciGrupKullanicilariRepository.Filter(s => s.IsTakipKullaniciGrupId == KullaniciGrupKodu).ToList<IsTakipKullaniciGrupKullanicilari>();
            List<TVMKullanicilar> tvmKullanicilar = _TVMContext.TVMKullanicilarRepository.All().ToList();

            EmailMetlifeKullanicilar model = new EmailMetlifeKullanicilar();

            IsTakip isTakip = _TeklifService.GetIsTakip(teklifId);

            TVMKullanicilar TVMKullanici = _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == _AktifKullanici.KullaniciKodu).FirstOrDefault();

            string KullaniciAdiSoyadi = String.Empty;
            string IsOlusturmaTarihi = String.Empty;
            int isNo = 0;
            string IsDurumu = String.Empty;

            var query = (from tvmK in tvmKullanicilar
                         join k in kullaniciGrupKullanicilari on tvmK.KullaniciKodu equals k.KullaniciKodu
                         select tvmK).ToList();

            model.Items = new List<EmailKullanicilari>();
            foreach (var items in query)
            {
                KullaniciAdiSoyadi = TVMKullanici.Adi + " " + TVMKullanici.Soyadi;
                isNo = isTakip.IsTakipId;
                IsOlusturmaTarihi = isTakip.KayitTarihi.ToShortDateString();
                EmailKullanicilari k = new EmailKullanicilari();
                k.adi = items.Adi;
                k.soyadi = items.Soyadi;
                k.email = items.Email;
                model.Items.Add(k);
            }

            if (model.Items != null)
            {
                foreach (var item in model.Items)
                {
                    string grupAdi = String.Empty;
                    grupAdi = MetlifeKullaniciGruplari.MetlifeKullaniciGruplariText(KullaniciGrupKodu.ToString()) + " grubu";

                    if (asama == 1)
                        IsDurumu = "Oluşturmuş olduğunuz işte Eksik belge veya eksik imza bulunmaktadır.";
                    else if (asama == 2)
                        IsDurumu = "iş oluşturuldu ve 2. adıma ilerletildi";
                    else if (asama == 3)
                        IsDurumu = "işte hiç bir eksiklik olmadığı onaylandı ve iş 3. adıma ilerletildi.";
                    else if (asama == 4)
                        IsDurumu = "işin kuryeye teslim edildiği onaylandı ve 4. adıma ilerletildi.";
                    else if (asama == 5)
                        IsDurumu = "form aslının geldiği onaylandı ve kutu numarası girildi. İş tamamlandı";

                    Hashtable data = new Hashtable();

                    data.Add("$LogonAddress$", "https://www.babonline.com");
                    data.Add("$TVMLogo$", tvm.Logo);
                    data.Add("$AdSoyad$", grupAdi);
                    data.Add("$MusAdSoyad$", KullaniciAdiSoyadi);
                    data.Add("$IsNo$", isNo);
                    data.Add("$Date$", IsOlusturmaTarihi);
                    data.Add("$IsDurumu$", IsDurumu);

                    Dictionary<string, string> to = new Dictionary<string, string>();
                    to.Add(item.email, grupAdi);
                    SendEMail("MetlifeEMail", data, to);
                }
            }
        }

        #endregion

        public bool SendHaritaIletisimForm(int TVMKodu, string AdSoyad, string Email, string Tel)
        {
            TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == TVMKodu).FirstOrDefault();
            if (tvm != null)
            {
                List<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvm.Kodu).ToList<TVMKullanicilar>();

                if (kullanicilar != null)
                {
                    foreach (var item in kullanicilar)
                    {
                        string UserName = item.Adi + ' ' + item.Soyadi;

                        Hashtable data = new Hashtable();
                        data.Add("$LogonAddress$", "https://www.babonline.com");
                        data.Add("$TVMLogo$", tvm.Logo);
                        data.Add("$TVMUnvani$", tvm.Unvani);
                        data.Add("$TVMUserName$", UserName);
                        data.Add("$MusAdSoyad$", AdSoyad);
                        data.Add("$MusEmail$", Email);
                        data.Add("$Tel$", Tel);
                        data.Add("$Date$", TurkeyDateTime.Now.ToString());

                        Dictionary<string, string> to = new Dictionary<string, string>();
                        to.Add(item.Email, UserName);

                        SendEMail("HaritaIletisim", data, to);
                        KullaniciNotEkle(item.KullaniciKodu, AdSoyad, Tel, Email);
                    }
                }
                return true;
            }
            else return false;
        }

        public void KullaniciNotEkle(int kullaniciKodu, string musAdSoyad, string musTelefon, string musEmail)
        {
            TVMKullaniciNotlar not = new TVMKullaniciNotlar();
            not.Konu = "Müşteri İletişim" + ' ' + musAdSoyad;
            not.Aciklama = String.Format("Ad Soyad  :{0}</br>Email &nbsp; &nbsp; &nbsp;&nbsp; :{1}</br>Telefon &nbsp; &nbsp; : {2}", musAdSoyad, musEmail, musTelefon);
            not.BitisTarihi = TurkeyDateTime.Now.AddDays(30);
            not.EklemeTarihi = TurkeyDateTime.Now;
            not.Oncelik = 1;
            not.KullaniciKodu = kullaniciKodu;

            _TVMContext.TVMKullaniciNotlarRepository.Create(not);
            _TVMContext.Commit();
        }

        public bool SendOnlyEmail(EPostaFormatlari format, Hashtable data, Dictionary<string, string> to, string projeKodu)
        {
            bool result = false;

            try
            {
                string fromEMail = String.Empty;
                string fromEMailName = String.Empty;
                string smtpAddress = String.Empty;
                string smtpPort = String.Empty;
                string smtpUserName = String.Empty;
                string smtpPassword = String.Empty;
                bool enableSSL = false;

                //Email configuration PROJEYE GÖRE
                KonfigTable konfig;

                switch (projeKodu)
                {
                    case TVMProjeKodlari.Aegon:

                        konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAegonEMail);

                        fromEMail = konfig[Konfig.AegonFromAddress];
                        fromEMailName = konfig[Konfig.AegonFromDisplayName];
                        smtpAddress = konfig[Konfig.AegonSMTPAddress];
                        smtpPort = konfig[Konfig.AegonSMTPPort];
                        smtpUserName = konfig[Konfig.AegonSMTPUserName];
                        smtpPassword = konfig[Konfig.AegonSMTPPassword];
                        bool.TryParse(konfig[Konfig.AegonEnableSSL], out enableSSL);
                        break;
                    case TVMProjeKodlari.Lilyum:
                        konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleLilyumEMail);
                        fromEMail = konfig[Konfig.LilyumEMailFromAddress];
                        fromEMailName = konfig[Konfig.LilyumEMailFromDisplayName];
                        smtpAddress = konfig[Konfig.LilyumEMailSMTPAddress];
                        smtpPort = konfig[Konfig.LilyumEMailSMTPPort];
                        smtpUserName = konfig[Konfig.LilyumEMailSMTPUserName];
                        smtpPassword = konfig[Konfig.LilyumEMailSMTPPassword];
                        bool.TryParse(konfig[Konfig.LilyumEMailEnableSSL], out enableSSL);
                        break;
                    default:
                        konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEMail);
                        fromEMail = konfig[Konfig.EMailFromAddress];
                        fromEMailName = konfig[Konfig.EMailFromDisplayName];
                        smtpAddress = konfig[Konfig.EMailSMTPAddress];
                        smtpPort = konfig[Konfig.EMailSMTPPort];
                        smtpUserName = konfig[Konfig.EMailSMTPUserName];
                        smtpPassword = konfig[Konfig.EMailSMTPPassword];
                        bool.TryParse(konfig[Konfig.EMailEnableSSL], out enableSSL);
                        break;
                }
                
                MailMessage message = new MailMessage();
              
                //From
                message.From = new MailAddress(fromEMail, fromEMailName, Encoding.UTF8);

                //To
                foreach (KeyValuePair<string, string> toItem in to)
                {
                    message.To.Add(new MailAddress(toItem.Key, toItem.Value, Encoding.UTF8));
                }

                //Set mail body data
                string mailBody = format.Icerik;
                foreach (DictionaryEntry item in data)
                {
                    mailBody = mailBody.Replace(item.Key.ToString(), item.Value.ToString());
                }
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                message.AlternateViews.Add(htmlView);

                //Subject
                message.Subject = format.Konu;
                SmtpClient smtp = new SmtpClient(smtpAddress);

                if (!String.IsNullOrEmpty(smtpPort))
                {
                    int port = 25;
                    if (int.TryParse(smtpPort, out port))
                    {
                        smtp.Port = port;
                    }
                }
                if (!String.IsNullOrEmpty(smtpUserName))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                }
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = enableSSL;
             
                smtp.Send(message);

                result = true;
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        public bool SendEMail(string formatName, Hashtable data, Dictionary<string, string> to)
        {
            try
            {
                EPostaFormatlari format = _EPostaService.GetEPosta(formatName);

                //Email configuration
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEMail);

                string fromEMail = konfig[Konfig.EMailFromAddress];
                string fromEMailName = konfig[Konfig.EMailFromDisplayName];
                string smtpAddress = konfig[Konfig.EMailSMTPAddress];
                string smtpPort = konfig[Konfig.EMailSMTPPort];
                string smtpUserName = konfig[Konfig.EMailSMTPUserName];
                string smtpPassword = konfig[Konfig.EMailSMTPPassword];
                bool enableSSL = false;
                bool.TryParse(konfig[Konfig.EMailEnableSSL], out enableSSL);

                MailMessage message = new MailMessage();

                //From
                message.From = new MailAddress(fromEMail, fromEMailName, Encoding.UTF8);

                //To
                foreach (KeyValuePair<string, string> toItem in to)
                {
                    message.To.Add(new MailAddress(toItem.Key, toItem.Value, Encoding.UTF8));
                }

                //Set mail body data
                string mailBody = format.Icerik;
                foreach (DictionaryEntry item in data)
                {
                    mailBody = mailBody.Replace(item.Key.ToString(), item.Value.ToString());
                }
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                message.AlternateViews.Add(htmlView);

                //Subject
                message.Subject = format.Konu;

                SmtpClient smtp = new SmtpClient(smtpAddress);

                if (!String.IsNullOrEmpty(smtpPort))
                {
                    int port = 25;
                    if (int.TryParse(smtpPort, out port))
                    {
                        smtp.Port = port;
                    }
                }

                if (!String.IsNullOrEmpty(smtpUserName))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                }

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = enableSSL;
                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);

                return false;
            }
        }

        public bool SendEMailPDF(ITeklif teklif, EPostaFormatlari format, Hashtable data, Dictionary<string, string> to, List<FileAtachmentList> attachments)
        {
            bool result = true;
            try
            {
                MailMessage message = new MailMessage();

                string fromEMail = String.Empty;
                string fromEMailName = String.Empty;
                string smtpAddress = String.Empty;
                string smtpPort = String.Empty;
                string smtpUserName = String.Empty;
                string smtpPassword = String.Empty;
                bool enableSSL = false;

                if (teklif.GenelBilgiler.TVMDetay != null && teklif.GenelBilgiler.TVMDetay.ProjeKodu == TVMProjeKodlari.Aegon)
                {
                    TVMKullanicilar Acentekullanicisi = teklif.GenelBilgiler.TVMKullanicilar;
                    if (Acentekullanicisi != null)
                        message.ReplyToList.Add(new MailAddress(Acentekullanicisi.Email));

                    //Email configuration   
                    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAegonEMail);

                    fromEMail = konfig[Konfig.AegonFromAddress];
                    fromEMailName = konfig[Konfig.AegonFromDisplayName];
                    smtpAddress = konfig[Konfig.AegonSMTPAddress];
                    smtpPort = konfig[Konfig.AegonSMTPPort];
                    smtpUserName = konfig[Konfig.AegonSMTPUserName];
                    smtpPassword = konfig[Konfig.AegonSMTPPassword];
                    bool.TryParse(konfig[Konfig.AegonEnableSSL], out enableSSL);
                }
                else
                {
                    //Email configuration
                    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEMail);

                    fromEMail = konfig[Konfig.EMailFromAddress];
                    fromEMailName = konfig[Konfig.EMailFromDisplayName];
                    smtpAddress = konfig[Konfig.EMailSMTPAddress];
                    smtpPort = konfig[Konfig.EMailSMTPPort];
                    smtpUserName = konfig[Konfig.EMailSMTPUserName];
                    smtpPassword = konfig[Konfig.EMailSMTPPassword];
                    bool.TryParse(konfig[Konfig.EMailEnableSSL], out enableSSL);
                }


                //From
                message.From = new MailAddress(fromEMail, fromEMailName, Encoding.UTF8);

                //To
                foreach (KeyValuePair<string, string> toItem in to)
                {
                    message.To.Add(new MailAddress(toItem.Key, toItem.Value, Encoding.UTF8));
                }

                //Set mail body data
                string mailBody = format.Icerik;
                foreach (DictionaryEntry item in data)
                {
                    mailBody = mailBody.Replace(item.Key.ToString(), item.Value.ToString());
                }
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                message.AlternateViews.Add(htmlView);

                int sayac = 0;
                foreach (var item in attachments)
                {
                    System.IO.Stream stream = new System.IO.MemoryStream(item.File);
                    Attachment att = new Attachment(stream, item.FileName);
                    att.ContentId = "file" + sayac;
                    att.ContentType = new System.Net.Mime.ContentType("application/pdf");
                    message.Attachments.Add(att);
                    sayac++;
                }


                //Subject
                message.Subject = format.Konu;

                SmtpClient smtp = new SmtpClient(smtpAddress);

                if (!String.IsNullOrEmpty(smtpPort))
                {
                    int port = 25;
                    if (int.TryParse(smtpPort, out port))
                    {
                        smtp.Port = port;
                    }
                }

                if (!String.IsNullOrEmpty(smtpUserName))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                }

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = enableSSL;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);

                result = false;
            }

            return result;
        }

        private void TeklifEMailLog(int teklifId, string formatAdi, string email, bool basarili)
        {
            TeklifEMailLog log = new TeklifEMailLog();
            log.TeklifId = teklifId;
            log.Tarih = TurkeyDateTime.Now;
            log.FormatAdi = formatAdi;
            log.Email = email;
            log.Basarili = basarili;

            ITeklifContext teklifContext = DependencyResolver.Current.GetService<ITeklifContext>();
            teklifContext.TeklifEMailLogRepository.Create(log);
            teklifContext.Commit();
        }

        private MailClientModel GetClientInfo(TVMKullanicilar kullanici, string projeKodu)
        {
            MailClientModel model = new MailClientModel();

            try
            {
                switch (projeKodu)
                {
                    case TVMProjeKodlari.Aegon:
                        model.Baslik = "Aegon hesabınız için bir parola değiştirme isteği aldık : " + kullanici.Adi + " " + kullanici.Soyadi;
                        model.LogoUrl = AegonCommon.LogoURL;
                        model.LogoSrc = AegonCommon.LogoSrc;
                        model.LogoAlt = "Aegon Logo";
                        break;
                    case TVMProjeKodlari.Mapfre:
                        model.Baslik = "Mapfre hesabınız için bir parola değiştirme isteği aldık : " + kullanici.Adi + " " + kullanici.Soyadi;
                        model.LogoUrl = MapfreCommon.LogoURL;
                        model.LogoSrc = MapfreCommon.LogoSrc;
                        model.LogoAlt = "Mapfre Logo";
                        break;
                }

                if (HttpContext.Current != null)
                {
                    ////IP ADDRESS
                    //string ip = HttpContext.Current.Request.UserHostAddress;

                    //if (String.IsNullOrEmpty(ip))
                    //{
                    //    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    //    if (String.IsNullOrEmpty(ip))
                    //        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //}

                    //model.IpAddress = ip;

                    //OPERATION SYSTEM
                    HttpBrowserCapabilities browse = HttpContext.Current.Request.Browser;
                    if (browse != null)
                    {
                        string operationSystem = String.Empty;

                        switch (browse.Platform)
                        {
                            case "WinNT": operationSystem = "Windows"; break;
                            default: operationSystem = browse.Platform; break;
                        }

                        model.OperatinSystem = operationSystem + " / " + browse.Browser;
                    }
                }
            }
            catch (Exception ex)
            {
                ILogService _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }

            return model;
        }
    }

    public class FileAtachmentList
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }

    public class MailClientModel
    {
        public string Baslik { get; set; }
        public string LogoUrl { get; set; }
        public string LogoSrc { get; set; }
        public string LogoAlt { get; set; }
        public string IpAddress { get; set; }
        public string Aciklama { get; set; }
        public string Kullanici { get; set; }
        public string Sifre { get; set; }
        public string OperatinSystem { get; set; }
    }

    public class MailHelper
    {
        public static string GetAracDegerler(ITeklif teklif, ITeklifService _TeklifService, bool police, string belgeNo)
        {
            StringBuilder sb = new StringBuilder();

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();
            AracMarka aracMarka = new AracMarka();
            AracTip aracTip = new AracTip();

            string plaka = "";
            string tescilTarihi = "";

            if (police)
            {
                sb.AppendLine(String.Format("<strong>Poliçe No :</strong> {0}", belgeNo));
            }
            else
            {
                sb.AppendLine(String.Format("<strong>Teklif No :</strong> {0}", belgeNo));
            }

            if (teklif.GenelBilgiler.TUMKodu == 0)
            {
                aracMarka = aracService.GetAracMarka(teklif.Arac.Marka);
                aracTip = aracService.GetAracTip(teklif.Arac.Marka, teklif.Arac.AracinTipi);
                plaka = teklif.Arac.PlakaKodu + " " + teklif.Arac.PlakaNo;
                tescilTarihi = teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "";
            }
            else
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

                aracMarka = aracService.GetAracMarka(anaTeklif.Arac.Marka);
                aracTip = aracService.GetAracTip(anaTeklif.Arac.Marka, anaTeklif.Arac.AracinTipi);
                plaka = anaTeklif.Arac.PlakaKodu + " " + anaTeklif.Arac.PlakaNo;
                tescilTarihi = anaTeklif.Arac.TrafikTescilTarihi.HasValue ? anaTeklif.Arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy") : "";
            }

            sb.AppendLine(String.Format("<br/><strong>Plaka :</strong> {0}", plaka));
            sb.AppendLine(String.Format("<br/><strong>Trafik Tescil Tarihi : </strong>{0}", tescilTarihi));
            sb.AppendLine(String.Format("<br/><strong>Araç Türü : </strong>{0} - {1}", aracMarka.MarkaAdi, aracTip.TipAdi));


            return sb.ToString();
        }
    }

}

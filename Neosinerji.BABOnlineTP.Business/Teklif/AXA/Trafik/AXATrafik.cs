using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.AXAPoliceBasim;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using Neosinerji.BABOnlineTP.Business.axa.tahsilat;
using Neosinerji.BABOnlineTP.Business.AxaKasko;

namespace Neosinerji.BABOnlineTP.Business.AXA.Trafik
{
    public class AXATrafik : Teklif, IAXATrafik
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

        [InjectionConstructor]
        public AXATrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService,ITVMService TVMService )
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
            _TVMService = TVMService ; 
        }
        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.AXA;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            AxaApplicationWebSrvV01 client = new AxaApplicationWebSrvV01();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AXA });
                string pdfUrl = "";
                client.Url = konfig[Konfig.AXA_ServiceURL];
                client.Timeout = 150000;
                client.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);
                List<CPartnerBilgi> parners = new List<CPartnerBilgi>();
                List<string> mesajlar = new List<string>();
                mesajlar.Add("deneme");

                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                #region Sigortali / Sigorta Ettiren Bilgileri Hazırlama

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

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

                var sigEttiren = teklif.SigortaEttiren;
                MusteriAdre sigEttirenAdres = sigEttiren.MusteriGenelBilgiler.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                string axaSigIlKodu = String.Empty;
                string axaSigIlceKodu = String.Empty;

                string axaSigEttirenIlKodu = String.Empty;
                string axaSigEttirenIlceKodu = String.Empty;


                bool MusteriAyniMi = false;
                if (sigEttiren.MusteriKodu == sigortali.MusteriKodu)
                {
                    MusteriAyniMi = true;
                    if (sigortaliAdres != null)
                    {
                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                 f.IlKodu == sigortaliAdres.IlKodu &&
                                                                                 f.IlceKodu == sigortaliAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();
                        if (ililce != null)
                        {
                            axaSigIlKodu = ililce.CRIlKodu;
                            axaSigIlceKodu = ililce.CRIlceKodu;
                        }
                        else
                        {
                            axaSigIlKodu = "";
                            axaSigIlceKodu = "";
                        }

                    }
                }
                else
                {
                    MusteriAyniMi = false;

                    if (sigEttirenAdres != null)
                    {

                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                 f.IlKodu == sigEttirenAdres.IlKodu &&
                                                                                 f.IlceKodu == sigEttirenAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();

                        axaSigEttirenIlKodu = ililce.CRIlKodu;
                        axaSigEttirenIlceKodu = ililce.CRIlceKodu;
                    }
                }

                #endregion

                #region Eski Poliçe Bilgileri Hazırlama

                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                string oncekiAcenteNo = String.Empty;
                string oncekiPoliceNo = String.Empty;
                string oncekiSirketKodu = String.Empty;
                string oncekiYenilemeNo = String.Empty;

                if (eskiPoliceVar)
                {
                    oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);

                }
                #endregion

                #region Partner Bilgileri Hazırlama
                List<CMatbuBilgi> matBuList = new List<CMatbuBilgi>();
                CMatbuBilgi matBu = new CMatbuBilgi()
                {
                    Aciklama = teklif.Arac.MotorNo,
                    BilgiAdi = AXA_TrafikMatbuList.MotorNo
                };
                matBuList.Add(matBu);
                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.SasiNo,
                    Aciklama = teklif.Arac.SasiNo
                };
                matBuList.Add(matBu);
                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.YolcuYerAdedi,
                    Aciklama = teklif.Arac.KoltukSayisi.HasValue ? teklif.Arac.KoltukSayisi.Value.ToString() : "5"
                };
                matBuList.Add(matBu);

                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.TrafigeCikisTarihi,
                    Aciklama = teklif.Arac.TrafikCikisTarihi.HasValue ? teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyy").Replace('.', '/').ToString() : TurkeyDateTime.Now.ToString("dd/MM/yyyy").Replace('.', '/').ToString()
                };
                matBuList.Add(matBu);

                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.TrafikTescilTarihi,
                    Aciklama = teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString("dd/MM/yyy").Replace('.', '/').ToString() : TurkeyDateTime.Now.ToString("dd/MM/yyyy").Replace('.', '/').ToString()
                };
                matBuList.Add(matBu);

                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.SilindirHacmi,
                    Aciklama = teklif.Arac.SilindirHacmi
                };
                matBuList.Add(matBu);

                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.MotorHacmi,
                    Aciklama = teklif.Arac.MotorGucu
                };
                matBuList.Add(matBu);

                //teklif.Arac.Renk
                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.Renk,
                    Aciklama = "GRİ (KUM)"
                };
                matBuList.Add(matBu);

                matBu = new CMatbuBilgi()
                {
                    BilgiAdi = AXA_TrafikMatbuList.PlakaNo,
                    Aciklama = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo
                };
                matBuList.Add(matBu);
                if (eskiPoliceVar)
                {
                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.EskiAcenteNo,
                        Aciklama = oncekiAcenteNo
                    };
                    matBuList.Add(matBu);

                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.EskiPoliceNo,
                        Aciklama = oncekiPoliceNo
                    };
                    matBuList.Add(matBu);

                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.EskiYenilemeNo,
                        Aciklama = oncekiYenilemeNo
                    };
                    matBuList.Add(matBu);

                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.TramerNo,
                        Aciklama = teklif.Arac.TramerBelgeNo
                    };
                    matBuList.Add(matBu);
                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.TramerBelgeTarihi,
                        Aciklama = teklif.Arac.TramerBelgeTarihi.HasValue ? teklif.Arac.TramerBelgeTarihi.Value.ToString("dd/MM/yyyy").Replace('.', '/').ToString() : ""
                    };
                    matBuList.Add(matBu);
                    matBu = new CMatbuBilgi()
                    {
                        BilgiAdi = AXA_TrafikMatbuList.RuhsatBelgeNumarasi,
                        Aciklama = teklif.Arac.AsbisNo != null ? teklif.Arac.AsbisNo : teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo
                    };
                    matBuList.Add(matBu);
                }

                #endregion

                #region Police Aciklama Bilgileri Hazırlama
                List<string> aciklamalar = new List<string>();
                #endregion

                #region Request

                CPoliceBaslik3 policeBaslik = new CPoliceBaslik3()
                {
                    SubeKod = 0,
                    KaynakKod = 0,//300
                    AcenteNo = "", //62734641
                    //TaliAcente = "",
                    TaliKaynak = 0,
                    PoliceCinsi = "", // boş olabilir
                    UrunKodu = AXA_UrunKodlari.Trafik,
                    Doviz_Kod = "TL",
                    Teklif_Tarih = DateTime.Now,
                    Tanzim_Tarih = DateTime.Now,
                    Baslama_Tarih = polBaslangic,
                    Bitis_Tarih = polBaslangic.AddYears(1),
                    //MTKod = "",
                    //TramerSirketKod = "",
                    //TramerAcenteNo = "",
                    //TramerPoliceNo = "",
                    //TramerYenilemeNo = "",
                    PlakaNo = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo,
                    //EskiPlakaNo = "",
                    TescilBelgeNo = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo,
                    // TurceEkAciklamalar = aciklamalar.ToArray(),
                    // IngEkAciklamalar = aciklamalar.ToArray(), 

                    Sigortali = new CMusteriAxa()
                    {
                        Adres = new CMusteriAdres()
                        {
                            //Zorunlu alanlar.
                            Ulke_Kod = "TÜR",
                            Il_Kod = axaSigIlKodu,
                            Ilce_Kod = axaSigIlceKodu,
                            Semt_Kod = "000",
                            Mahalle = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Mahalle : ".",
                            Sokak = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : ".",
                            Cadde = !String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : ".",
                            Daire_No = !String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : "1",
                            //-------Zorunlu alanlar

                            //Posta_Kod = "",
                            //Han_Apt_Fab = "",
                            //Bina_No = "13",
                            //UavtAdresId = 0,
                        },
                        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                        {
                            TcNo = sigortali.KimlikNo,
                            UlkeKod = "TÜR",
                        },
                        IletisimBilgileri = new CMusteriIletisimBilgi()
                        {
                            Email = sigortali.EMail,//"deneme@deneme.com",
                            Fax_No = "",//"0(212)9999999",
                            GSM_No = MusteriCepTel,//"0(543)9999999",
                            Tel_No = MusteriEvTel,//"0(212)9999999",
                        },
                        // Mesajlar = mesajlar.ToArray(),
                        // PartnerBilgiler = parners.ToArray()
                    },

                    SigortaEttiren = new CMusteriAxa()
                    {
                        Adres = new CMusteriAdres()
                        {
                            //Zorunlu alanlar.
                            Ulke_Kod = "TÜR",
                            Il_Kod = MusteriAyniMi ? axaSigIlKodu : axaSigEttirenIlKodu,
                            Ilce_Kod = MusteriAyniMi ? axaSigIlceKodu : axaSigEttirenIlceKodu,
                            Semt_Kod = "000",
                            Mahalle = MusteriAyniMi ? sigortaliAdres.Mahalle : sigEttirenAdres.Mahalle,
                            Sokak = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : sigEttirenAdres.Sokak) : ".",
                            Cadde = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : sigEttirenAdres.Cadde) : ".",
                            Daire_No = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : sigEttirenAdres.DaireNo) : "1",
                            //-------Zorunlu alanlar

                            //Posta_Kod = "",
                            //Han_Apt_Fab = "",
                            //Bina_No = "13",
                            //UavtAdresId = 0,
                        },
                        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                        {
                            TcNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttiren.MusteriGenelBilgiler.KimlikNo,
                            UlkeKod = "TÜR",
                        },
                        IletisimBilgileri = new CMusteriIletisimBilgi()
                        {
                            Email = MusteriAyniMi ? sigortali.EMail : sigEttiren.MusteriGenelBilgiler.EMail,// "deneme@deneme.com",
                            Fax_No = "",
                            GSM_No = MusteriCepTel,
                            Tel_No = MusteriEvTel,
                        },
                        //Mesajlar = mesajlar.ToArray(),
                        //PartnerBilgiler = parners.ToArray()

                    },

                    RizikoAdresi = new CMusteriAdres()
                    {
                        //UavtAdresId=0,
                        Ulke_Kod = "TÜR",
                        Il_Kod = axaSigIlKodu,
                        Ilce_Kod = axaSigIlceKodu,
                        Semt_Kod = "000",
                        Mahalle = ".",
                        Bina_No = "1",
                        Daire_No = "1",
                        Posta_Kod = "340600",
                        Han_Apt_Fab = ".",
                    },
                    MatbuBilgiler = matBuList.ToArray(),
                    //OtrMail = "",
                    RefKullanici = servisKullanici.KullaniciAdi2,
                    //TeknikPersonel = "",
                    //OtomatikYenileme = 0,
                    //UretimYeri = 0,
                    //ClientIPAddress = "85.105.78.56", //this.IpGetir(teklif.GenelBilgiler.TVMKodu),

                };

                CPoliceDetay policeDetay = new CPoliceDetay();
                List<CTeminat> teminatList = new List<CTeminat>();
                CTeminat teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = 0,
                    Iptal = false,
                    TeminatKod = AXA_TrafikTeminatlar.Trafik

                };
                teminatList.Add(teminat);
                policeDetay.Teminatlar = teminatList.ToArray();

                string AXAKullanimTarzi = String.Empty;
                string AXAKullanimSekli = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');

                if (parts.Length == 2)
                {
                    string kullanimT = parts[0];
                    string kod2 = parts[1];


                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                                  f.KullanimTarziKodu == kullanimT &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        string[] part = kullanimTarzi.TarifeKodu.Split('-');
                        if (part.Length > 0)
                        {
                            AXAKullanimTarzi = part[0];

                            if (teklif.Arac.KullanimTarzi == "532-10")
                            {
                                AXAKullanimTarzi = "17";
                            }
                        }
                    }
                }

                if (teklif.Arac.KullanimSekli == "0")
                {
                    AXAKullanimSekli = AXA_KullanimSekilleri.Ticari;
                }
                else if (teklif.Arac.KullanimSekli == "1")
                {
                    AXAKullanimSekli = AXA_KullanimSekilleri.Resmi;
                }
                else
                {
                    AXAKullanimSekli = AXA_KullanimSekilleri.Ozel;
                }

                List<CSoru> soruList = new List<CSoru>();
                CSoru soru = new CSoru()
                {
                    CevapKod = teklif.Arac.Marka,
                    SoruKod = AXA_TrafikSoruKodlari.Marka

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.AracinTipi,
                    SoruKod = AXA_TrafikSoruKodlari.MarkaTip

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.Model.ToString(),
                    SoruKod = AXA_TrafikSoruKodlari.Model

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = eskiPoliceVar ? AXA_KayitTipleri.Hayir : AXA_KayitTipleri.Evet,
                    SoruKod = AXA_TrafikSoruKodlari.PlakaYeniKayitMi

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = AXAKullanimSekli,
                    SoruKod = AXA_TrafikSoruKodlari.KullanimSekli

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = AXAKullanimTarzi,
                    SoruKod = AXA_TrafikSoruKodlari.KullanimTarzi

                };
                soruList.Add(soru);

                if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    string cevapkod = "";
                    switch (teklif.GenelBilgiler.TaksitSayisi.Value)
                    {
                        case 2: cevapkod = AXA_OdemeSekilleri.Iki; break;
                        case 3: cevapkod = AXA_OdemeSekilleri.Uc; break;
                        case 4: cevapkod = AXA_OdemeSekilleri.Dort; break;
                        case 5: cevapkod = AXA_OdemeSekilleri.Bes; break;
                        case 6: cevapkod = AXA_OdemeSekilleri.Alti; break;
                        case 7: cevapkod = AXA_OdemeSekilleri.Yedi; break;
                        default:
                            cevapkod = AXA_OdemeSekilleri.Pesin;
                            break;
                    }
                    soru = new CSoru()
                    {

                        CevapKod = cevapkod,
                        SoruKod = AXA_TrafikSoruKodlari.OdemeSekli

                    };
                    soruList.Add(soru);
                }
                else
                {
                    soru = new CSoru()
                    {
                        CevapKod = AXA_OdemeSekilleri.Pesin,
                        SoruKod = AXA_TrafikSoruKodlari.OdemeSekli

                    };
                    soruList.Add(soru);
                }
                string AXA_OdemeTipi = AXA_OdemeTipleri.Nakit;
                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti && teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    AXA_OdemeTipi = AXA_OdemeTipleri.KrediKartiTaksitli;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti && teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Pesin)
                {
                    AXA_OdemeTipi = AXA_OdemeTipleri.KrediKartiPesin;
                }

                soru = new CSoru()
                {
                    CevapKod = AXA_OdemeTipi,
                    SoruKod = AXA_TrafikSoruKodlari.OdemeTipi

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.PlakaKodu,
                    SoruKod = AXA_TrafikSoruKodlari.PlakaIlKodu

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = AXA_ImalatYerleri.Yerli,
                    SoruKod = AXA_TrafikSoruKodlari.ImalatYeri

                };
                soruList.Add(soru);
                policeDetay.Sorular = soruList.ToArray();

                #endregion

                #region Servis

                Request req = new Request();
                req.CPoliceBaslik3 = policeBaslik;
                req.CPoliceDetay = policeDetay;

                this.BeginLog(req, req.GetType(), WebServisIstekTipleri.Teklif);
                // this.BeginLog(policeDetay, policeDetay.GetType(), WebServisIstekTipleri.Teklif2);

                var response = client.TeklifHazirla(req.CPoliceBaslik3, req.CPoliceDetay);
                client.Dispose();

                if (response.HataMesajlari != null && (response.TeklifNumarasi == null || response.TeklifNumarasi == 0))
                {
                    this.EndLog(response, false, response.GetType());

                    for (int i = 0; i < response.HataMesajlari.Count(); i++)
                    {
                        this.AddHata(response.HataMesajlari[i].ToString());
                    }
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                    string testMi = konfigTestMi[Konfig.TestMi];
                    if (!Convert.ToBoolean(testMi))
                    {
                        #region PDF URL
                        AXAPoliceBasim.AxaPoliceBasim pdfClient = new AXAPoliceBasim.AxaPoliceBasim();
                        pdfClient.Url = konfig[Konfig.AXA_PoliceBasim];
                        pdfClient.Timeout = 150000;
                        pdfClient.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);

                        AOCPoliceBasimParametre pdfReq = new AOCPoliceBasimParametre()
                        {
                            BasimTipi = BasimType.Binary,
                            DilSecimi = BasimDil.Turkce,
                            Kullanici = servisKullanici.KullaniciAdi2,
                            PoliceNo = response.TeklifNumarasi.Value,
                            ZeylSiraNo = 0
                        };

                        var responsePDF = pdfClient.PoliceBasim(pdfReq);
                        pdfClient.Dispose();

                        if (responsePDF.HataMesajlari.ToList().Count > 0)
                        {
                            this.EndLog(responsePDF, false, responsePDF.GetType());
                            for (int i = 0; i < responsePDF.HataMesajlari.Count(); i++)
                            {
                                this.AddHata(responsePDF.HataMesajlari[i].ToString());
                            }

                            this.AddHata("PDF dosyası alınamadı.");
                        }
                        else
                        {
                            this.EndLog(responsePDF, true, responsePDF.GetType());
                            byte[] data = responsePDF.Binary;
                            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                            string fileName = String.Format("AXA_Trafik_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                            pdfUrl = storage.UploadFile("trafik", fileName, data);

                            _Log.Info("Teklif_PDF url: {0}", pdfUrl);
                        }
                        #endregion

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
                this.GenelBilgiler.BrutPrim = response.PrimBorcu;
                this.GenelBilgiler.TUMTeklifNo = response.TeklifNumarasi.ToString();
                this.GenelBilgiler.PDFDosyasi = pdfUrl;
                #region Vergiler
                decimal BSV = 0;
                decimal GH = 0;
                decimal THG = 0;
                foreach (var item in response.Vergiler)
                {
                    if (item.VergiKod == "BSV")
                    {
                        BSV = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                    else if (item.VergiKod == "GH")
                    {
                        GH = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                    else if (item.VergiKod == "THG")
                    {
                        THG = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                }

                this.AddVergi(TrafikVergiler.THGFonu, THG);
                this.AddVergi(TrafikVergiler.GiderVergisi, BSV);
                this.AddVergi(TrafikVergiler.GarantiFonu, GH);

                this.GenelBilgiler.ToplamVergi = BSV + GH + THG;
                #endregion

                #region Taksitler

                if (response.Taksitler.Count() > 0)
                {
                    this.GenelBilgiler.ToplamKomisyon = response.Taksitler.FirstOrDefault().Komisyon;
                }
                if (response.Taksitler.Count() == 1)
                {
                    this.GenelBilgiler.TaksitSayisi = 1;
                }
                else
                {
                    this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                }
                #endregion

                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                //this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                //this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                if (response.Sorular != null)
                {
                    foreach (var item in response.Sorular)
                    {
                        if (item.SoruKod == AXA_TrafikSoruKodlari.TarifeBasamagi)
                        {
                            this.GenelBilgiler.TarifeBasamakKodu = Convert.ToInt16(item.CevapKod);
                        }
                        else if (item.SoruKod == AXA_TrafikSoruKodlari.HasarsizlikIndirimi)
                        {
                            if (item.CevapKod != "1")
                            {
                                string hasarsizlikYuzdesi = String.Empty;
                                foreach (char ch in item.CevapAd)
                                {
                                    if (Char.IsDigit(Convert.ToChar(ch)))
                                    {
                                        hasarsizlikYuzdesi += ch;
                                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(hasarsizlikYuzdesi);
                                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                                    }
                                }
                            }
                            else
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                            }
                        }
                        else if (item.SoruKod == AXA_TrafikSoruKodlari.HasarsizlikSurprimi)
                        {
                            string hasarsizlikYuzdesi = String.Empty;
                            foreach (char ch in item.CevapAd)
                            {
                                if (Char.IsDigit(Convert.ToChar(ch)))
                                {
                                    hasarsizlikYuzdesi += ch;
                                    this.GenelBilgiler.HasarSurprimYuzdesi = Convert.ToInt32(hasarsizlikYuzdesi);
                                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Teminatlar

                if (response.Teminatlar.Count() > 0)
                {
                    foreach (var item in response.Teminatlar)
                    {
                        if (item.TeminatKod == AXA_TrafikTeminatlar.Trafik)
                        {
                            this.AddTeminat(TrafikTeminatlar.Trafik, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.KisiBasiSakatlanmaOlum)
                        {
                            this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.KazaBasiSakatlanmaOlum)
                        {
                            this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.KisiBasiTedaviMasraflari)
                        {
                            this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.KazaBasiTedaviMasraflari)
                        {
                            this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.AracBasinaMaddiZaralar)
                        {
                            this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_TrafikTeminatlar.KazaBasinaMaddiZaralar)
                        {
                            this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
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

                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                #region Hata Log
                client.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            AxaApplicationWebSrvV01 client = new AxaApplicationWebSrvV01();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);

                client.Url = konfig[Konfig.AXA_ServiceURL];
                client.Timeout = 150000;
                client.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                bool KrediKartMi = false;

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    KrediKartMi = true;
                }

                List<AOCMustTahsilatBilgileri> list = new List<AOCMustTahsilatBilgileri>();
                if (KrediKartMi)
                {
                    string[] isim = odeme.KrediKarti.KartSahibi.Split(' ');

                    AOCMustTahsilatBilgileri item = new AOCMustTahsilatBilgileri()
                    {
                        SiraNo = 0,
                        Tutar = teklif.GenelBilgiler.BrutPrim,
                        CardNumber = odeme.KrediKarti.KartNo,
                        LastExpDate = TurkeyDateTime.Now,
                        CVC = odeme.KrediKarti.CVC,
                        BorcluIlkAd = isim[0],
                        BorcluOrtaAd = isim.Length == 3 ? isim[1] : "",
                        BorcluSoyadAd = isim.Length == 3 ? isim[2] : isim[1],
                        Load = null
                    };
                    list.Add(item);
                }

                CIslemDetay req = new CIslemDetay()
                {
                    IslemTip = AXA_IslemTipleri.Police,
                    MustTahsilatBilgileriList = new AOCMustTahsilatBilgileri_List()
                    {
                        Items = KrediKartMi ? list.ToArray() : null,

                    },
                    PoliceNo = Convert.ToDecimal(teklif.GenelBilgiler.TUMTeklifNo),
                    //RedNedeni="",
                    RefKullanici = "",
                    ZeylNo = 0,

                };
                this.BeginLog(req, req.GetType(), WebServisIstekTipleri.Police);
                var response = client.TeklifOnayRedIptal(req);
                client.Dispose();
                if (response.HataMesajlari.ToList().Count > 0)
                {
                    this.EndLog(response, false, response.GetType());
                    for (int i = 0; i < response.HataMesajlari.Count(); i++)
                    {
                        this.AddHata(response.HataMesajlari[i].ToString());
                    }
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.PoliceNo.Value.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    #region PDF URL
                    AXAPoliceBasim.AxaPoliceBasim pdfClient = new AXAPoliceBasim.AxaPoliceBasim();
                    pdfClient.Url = konfig[Konfig.AXA_PoliceBasim];
                    pdfClient.Timeout = 150000;

                    AOCPoliceBasimParametre pdfReq = new AOCPoliceBasimParametre()
                    {
                        BasimTipi = BasimType.Path,
                        DilSecimi = BasimDil.Turkce,
                        // Kullanici="",
                        PoliceNo = response.PoliceNo.Value,
                        ZeylSiraNo = response.ZeylNo.Value
                    };

                    var responsePDF = pdfClient.PoliceBasim(pdfReq);
                    pdfClient.Dispose();

                    if (responsePDF.HataMesajlari.ToList().Count > 0)
                    {
                        this.EndLog(responsePDF, false, responsePDF.GetType());

                        for (int i = 0; i < responsePDF.HataMesajlari.Count(); i++)
                        {
                            this.AddHata(responsePDF.HataMesajlari[i].ToString());
                        }

                        this.AddHata("PDF dosyası alınamadı.");
                    }
                    else
                    {
                        this.EndLog(responsePDF, true, responsePDF.GetType());

                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(responsePDF.PolicePath);

                        IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Empty;
                        string url = String.Empty;
                        fileName = String.Format("AXA_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                        url = storage.UploadFile("trafik", fileName, data);
                        this.GenelBilgiler.PDFPolice = url;

                        _Log.Info("Police_PDF url: {0}", url);
                    }
                    #endregion

                }
                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
            catch (Exception ex)
            {
                #region Hata Log
                client.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        public string IpGetir(int tvmKodu)
        {
            string cagriIP = "82.222.165.62";
            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo;
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.CagriTVMKodu || tvmKodu == NeosinerjiTVM.CagriBrokerTVMKodu)
                {
                    gonderilenIp = cagriIP;
                }
            }
            return gonderilenIp;
        }

        public EntegGetTokenResult GetBKMToken(ITeklif teklif, Odeme odeme)
        {
            //Kısmi kredi kartı bilgisi ile BKM sisteminden token bilgisi almak için kullanılan metot.
            //
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AXA });

            AxaTahsilatWeb tahsilatClient = new AxaTahsilatWeb();
            string TokenId = String.Empty;
            EntegGetTokenParam prm = new EntegGetTokenParam()
            {
                first_6_digits = odeme.KrediKarti.KartNo.Substring(0, 6),
                last_4_digits = odeme.KrediKarti.KartNo.Substring(12, 4),
                RefKullanici = servisKullanici.KullaniciAdi2,
                tckn = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo,
            };

            var token = tahsilatClient.BKMGetToken(prm);
            //bkmYonlen 1 gelirse BKM ara ekranına yönlenme yapılmalıdır. bkmURL=BKM ara ekranını açmak için kullanılması gereken url bilgisi gelir

            return token;
        }

        public EntegGetTokenResult BKMQueryTokenGet(ITeklif teklif)
        {
            //BKM sistemine kayıt edilmiş token bilgisi okumak için kullanılan metot.
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AXA });

            AxaTahsilatWeb tahsilatClient = new AxaTahsilatWeb();
            string TokenId = String.Empty;
            QueryTokenParam queryprm = new QueryTokenParam()
            {
                RefKullanici = servisKullanici.KullaniciAdi2,
                transactionId = "",  //NEoOnline sistemine Kaydedilen id okunacak            
                uniqueReferans = "", //NEoOnline sistemine Kaydedilen Referans okunacak  
            };

            var token = tahsilatClient.BKMQueryToken(queryprm);

            return token;
        }

        public class Request
        {
            [XmlElement]
            public CPoliceBaslik3 CPoliceBaslik3 { get; set; }

            [XmlElement]
            public CPoliceDetay CPoliceDetay { get; set; }
        }
    }
}

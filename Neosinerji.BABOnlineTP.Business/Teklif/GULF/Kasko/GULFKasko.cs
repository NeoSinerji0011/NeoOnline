using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.GULF.Messages;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Arac;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Arac.EGM;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Police;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri.Kaydet;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.PDF;
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF
{
    public class GULFKasko : Teklif, IGULFKasko
    {
        ITVMContext _TVMContext;
        ITVMService _TVMService;
        ILogService _Log;
        IMusteriService _MusteriService;
        ICRContext _CRContext;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifKullaniciService;
        IKonfigurasyonService _KonfigurasyonService;
        ICRService _CRService;

        [InjectionConstructor]
        public GULFKasko(ITVMContext tvmContext, ITVMService TVMService, ILogService log, IMusteriService musteriService, ICRContext crContext, ITeklifService teklifService,
            IAktifKullaniciService aktifKullaniciService, IKonfigurasyonService konfigurasyonService, ICRService cRService)
            : base()
        {
            _TVMContext = tvmContext;
            _TVMService = TVMService;
            _Log = log;
            _MusteriService = musteriService;
            _CRContext = crContext;
            _TeklifService = teklifService;
            _AktifKullaniciService = aktifKullaniciService;
            _KonfigurasyonService = konfigurasyonService;
            _CRService = cRService;
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
                #region Veri Hazırlama

                gulfsigorta.kasko.SFSPolicyIntegrationService clnt = new gulfsigorta.kasko.SFSPolicyIntegrationService();
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGulf);
                clnt.Timeout = 150000;
                clnt.Url = konfig[Konfig.GULF_KaskoServisURL];
                GULF_KaskoTeklifKayit_Request request = new GULF_KaskoTeklifKayit_Request();
              
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.GULF });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                string GulfMusteri = teklif.ReadSoru(KaskoSorular.GulfKimlikNo, "0");
                if (String.IsNullOrEmpty(GulfMusteri) || GulfMusteri == "0")
                {
                    GulfMusteri = teklif.ReadWebServisCevap(Common.WebServisCevaplar.Gulf_Musteri_No, "0");
                }

                // var gulfMusteriResult = GetGULFMusteriNo(sigortali.KimlikNo);
                //if (!String.IsNullOrEmpty(gulfMusteriResult.HataMesaji))
                //{
                //    this.Import(teklif);
                //    this.GenelBilgiler.Basarili = false;
                //    this.EndLog(gulfMusteriResult.HataMesaji, false);
                //    this.AddHata(gulfMusteriResult.HataMesaji);
                //}
                //else
                //{

                request.AUTH = new AUTH()
                {
                    EXT_CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    CALLER_GUID = servisKullanici.KullaniciAdi2,
                    USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                    PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                };
                if (String.IsNullOrEmpty(GulfMusteri) || GulfMusteri == "0")
                {
                    var gMusteri = this.GetGULFMusteriNo(sigortali.KimlikNo);
                    if (gMusteri != null)
                    {
                        GulfMusteri = gMusteri.MusteriNo;
                    }
                }
                request.POLICY = new Messages.Kasko.Teklif.POLICY()
                {
                    MASTER = new Messages.Kasko.Teklif.MASTER()
                    {
                        BEG_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/"),
                        END_DATE = polBaslangic.AddYears(1).ToString("dd/MM/yyyy").Replace(".", "/"),
                        CONFIRM_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                        ISSUE_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/"),
                        PRODUCT_NO = "135",
                        CLIENT_NO = !String.IsNullOrEmpty(GulfMusteri) ? GulfMusteri : "",
                        CHANNEL = servisKullanici.PartajNo_,
                        INSURED_NAME = sigortali.AdiUnvan,
                        INSURED_SURNAME = sigortali.SoyadiUnvan,
                        PLATE = teklif.Arac.PlakaKodu + " " + teklif.Arac.PlakaNo,
                        CATEGORY1 = "A",
                        EXCHANGE_RATE = "1",
                        HAS_INST_CARD = "0",
                    },
                    RISK_ADDRESS = "",

                };
                #region Teklif Sorular

                List<Messages.Kasko.Teklif.QUESTION> questionList = new List<Messages.Kasko.Teklif.QUESTION>();
                Messages.Kasko.Teklif.QUESTION question = new Messages.Kasko.Teklif.QUESTION();

                //İkinci El Yeni Araç Sorular
                question = new Messages.Kasko.Teklif.QUESTION();
                question.QUESTION_CODE = GULF_KaskoSorular.TescilBelgeSeriNoAsbis;
                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriNo))
                {
                    question.ANSWER = teklif.Arac.TescilSeriNo;
                }
                else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    question.ANSWER = teklif.Arac.AsbisNo;
                }
                questionList.Add(question);

                //****Yeni Kayıt Soruları
                if (teklif.Arac.Model.HasValue)
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.ModelYili;
                    question.ANSWER = teklif.Arac.Model.Value.ToString();
                    questionList.Add(question);
                }
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                string kod2 = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.GULF &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();
                    if (kullanimTarzi != null)
                    {
                        question = new Messages.Kasko.Teklif.QUESTION();
                        question.QUESTION_CODE = GULF_KaskoSorular.KullanimSekli;
                        question.ANSWER = kullanimTarzi.TarifeKodu;
                        questionList.Add(question);
                    }
                }

                question = new Messages.Kasko.Teklif.QUESTION();
                question.QUESTION_CODE = GULF_KaskoSorular.MarkaKodu;
                question.ANSWER = teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(3, '0');
                questionList.Add(question);

                question = new Messages.Kasko.Teklif.QUESTION();
                question.QUESTION_CODE = GULF_KaskoSorular.MotorNo;
                question.ANSWER = teklif.Arac.MotorNo;
                questionList.Add(question);

                question = new Messages.Kasko.Teklif.QUESTION();
                question.QUESTION_CODE = GULF_KaskoSorular.SasiNo;
                question.ANSWER = teklif.Arac.SasiNo;
                questionList.Add(question);

                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {
                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);
                    if (FKBedel != null)
                    {
                        CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.GULF, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);
                    }
                    if (CRKademeNo != null)
                    {
                        question = new Messages.Kasko.Teklif.QUESTION();
                        question.QUESTION_CODE = GULF_KaskoSorular.FerdiKazaKoltukBasina;
                        question.ANSWER = CRKademeNo.Kademe.ToString();
                        questionList.Add(question);
                    }
                }
                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(immKademe) && !String.IsNullOrEmpty(kullanimTarziKodu))
                {
                    CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();

                    //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                    if (IMMBedel != null)
                    {
                        //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                        CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.GULF, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                    }
                    if (CRKademeNo != null)
                    {
                        question = new Messages.Kasko.Teklif.QUESTION();
                        question.QUESTION_CODE = GULF_KaskoSorular.IMMLimit;
                        question.ANSWER = CRKademeNo.Kademe.ToString();
                        questionList.Add(question);
                    }
                }

                string GULFMeslekKodu = teklif.ReadSoru(KaskoSorular.GulfMeslekKodu, "99");

                if (GULFMeslekKodu != null)
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.MeslekIndirimi;
                    question.ANSWER = GULFMeslekKodu;
                    questionList.Add(question);
                }
                else
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.MeslekIndirimi;
                    question.ANSWER = "999";//  999 UYGULANMAYACAK
                    questionList.Add(question);
                }
                //Yenileme Soruları
                //question = new QUESTION();
                //question.QUESTION_CODE = GULF_KaskoSorular.FerdiKazaKoltukBasina;
                //question.ANSWER = "";
                //questionList.Add(question);


                string yakitTuru = teklif.ReadSoru(KaskoSorular.GulfYakitTuru, "1");
                if (!String.IsNullOrEmpty(yakitTuru))
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.YakitTuru;
                    question.ANSWER = yakitTuru;
                    questionList.Add(question);
                }

                string ikameTuru = teklif.ReadSoru(KaskoSorular.GulfIkameTuru, "1");
                if (!String.IsNullOrEmpty(ikameTuru))
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.KiralikAracSuresi;
                    question.ANSWER = ikameTuru; //< !--KİRALIK ARAÇ SÜRESİ -SABİT-- >
                    questionList.Add(question);
                }

                if (teklif.GenelBilgiler.TaksitSayisi.HasValue)
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.TaksitSecenegi;
                    question.ANSWER = teklif.GenelBilgiler.TaksitSayisi.Value.ToString(); //< !--Taksit Seçeneği-- >
                    questionList.Add(question);
                }

                string kaskoServisi = teklif.ReadSoru(KaskoSorular.Servis_Turu, "0");
                if (kaskoServisi == "5") //Tüm servisler ise
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.AlternatifKasko;
                    question.ANSWER = "E";
                    questionList.Add(question);
                }
                else
                {
                    question = new Messages.Kasko.Teklif.QUESTION();
                    question.QUESTION_CODE = GULF_KaskoSorular.AlternatifKasko;
                    question.ANSWER = "H";
                    questionList.Add(question);
                }
                string hukuksalKoruma = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, "1");
                question = new Messages.Kasko.Teklif.QUESTION();
                question.QUESTION_CODE = GULF_KaskoSorular.HukuksalKoruma;
                question.ANSWER = hukuksalKoruma;
                //string hkKademesi = teklif.ReadSoru(KaskoSorular.GulfHukuksalKorumaBedeli, String.Empty);
                if (!String.IsNullOrEmpty(hukuksalKoruma))
                {
                    var hkBedel = _TeklifService.getHkKademesi(Convert.ToInt32(hukuksalKoruma));
                    var unicoHkKademesi = _TeklifService.getHukuksalKorumaBedel(TeklifUretimMerkezleri.GULF, hkBedel);
                    question.ANSWER = unicoHkKademesi.DegerKodu;
                }
                questionList.Add(question);

                request.POLICY.QUESTIONS = questionList.ToArray();

                #endregion

                List<INSURED> sigortaliList = new List<INSURED>();
                INSURED insured = new INSURED();
                insured.INSURED_NO = GulfMusteri;
                insured.MAIN_INSURED = "1";
                sigortaliList.Add(insured);
                request.POLICY.INSUREDS = sigortaliList.ToArray();

                request.POLICY.COLLECTION = new Messages.Kasko.Teklif.COLLECTION();

                var odemeTipi = "";
                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.Nakit)
                {
                    odemeTipi = GULF_OdemeTipleri.Nakit;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti || teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    odemeTipi = GULF_OdemeTipleri.KrediKarti;
                }
                List<Messages.Kasko.Teklif.DEBTOR> deptorList = new List<Messages.Kasko.Teklif.DEBTOR>();
                Messages.Kasko.Teklif.DEBTOR deptor = new Messages.Kasko.Teklif.DEBTOR()
                {
                    ADVANCE_PAYMENT_TOOL_TYPE = odemeTipi, //Taksit Sayısı
                    INSTALLMENTS_PAYMENT_TYPE = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value.ToString() : "", //Peşinat ödeme tipi Nakit: NK, Kredi Kartı:TK, Otomatik Ödeme:OO
                    INSTALLMENTS_TOOL_TYPE = odemeTipi //Taksit ödeme tipi Nakit: NK, Kredi Kartı:TK, Otomatik Ödeme:OO
                };

                request.POLICY.COLLECTION.DEBTORS = deptorList.ToArray();

                #endregion

                #region Service Call

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);
                XmlSerializer _Serialize = new XmlSerializer(typeof(GULF_KaskoTeklifKayit_Request));
                StringWriter Output = new StringWriter(new StringBuilder());
                List<TeklifTeminat> teminatList = new List<TeklifTeminat>();
                TeklifTeminat teminat = new TeklifTeminat();
                _Serialize.Serialize(Output, request);
                var responseKasko = clnt.CreateProposal(Output.ToString());
                Output.Close();
                clnt.Dispose();

                bool Hata = false;
                string policeNo = "";
                string netPrim = "";
                string brutPrim = "";
                string musteriNo = "";
                string teminatKodu = "";
                string temNetPrim = "";
                string temCoverAmount = "";
                string operationId = "";

                var xmlNode = responseKasko.ChildNodes;
                for (int i = 0; i < xmlNode.Count; i++)
                {
                    if (xmlNode[i].Name == "OPERATION_ID")
                    {
                        operationId = xmlNode[i].InnerText;
                    }
                    else if (xmlNode[i].Name == "RESULT")
                    {
                        var resultContent = xmlNode[i].InnerText;
                        if (resultContent == "1")
                        {
                            _Serialize = new XmlSerializer(typeof(Messages.Kasko.GULF_KaskoTeklifKayit_Response));
                            using (StringReader readerKasko = new StringReader(responseKasko.OuterXml.ToString()))
                            {
                                Messages.Kasko.GULF_KaskoTeklifKayit_Response kaskoResponse = new Messages.Kasko.GULF_KaskoTeklifKayit_Response();
                                kaskoResponse = (Messages.Kasko.GULF_KaskoTeklifKayit_Response)_Serialize.Deserialize(readerKasko);
                                readerKasko.ReadToEnd();
                                this.EndLog(kaskoResponse, true, kaskoResponse.GetType());
                            }
                        }
                        else if (resultContent == "0")
                        {
                            Hata = true;
                        }
                    }
                    else if (xmlNode[i].Name == "DATA")
                    {
                        if (xmlNode[i].ChildNodes != null)
                        {
                            var SfsProductionSchemaNode = xmlNode[i].ChildNodes;
                            for (int l = 0; l < SfsProductionSchemaNode.Count; l++)
                            {
                                var SfsProductionSchemaChildNodes = SfsProductionSchemaNode[l].ChildNodes;
                                for (int j = 0; j < SfsProductionSchemaChildNodes.Count; j++)
                                {
                                    if (SfsProductionSchemaChildNodes[j].Name == "MASTER")
                                    {
                                        XmlNode elmMaster = SfsProductionSchemaChildNodes.Item(j);
                                        policeNo = elmMaster["POLICY_NO"].InnerText;
                                        netPrim = elmMaster["NET_PREMIUM"].InnerText;
                                        brutPrim = elmMaster["GROSS_PREMIUM"].InnerText;
                                        musteriNo = elmMaster["INSURED_NO"].InnerText;

                                        //netPrim = elmMaster["NET_PREMIUM"].InnerText.Replace(".", ",");
                                        //brutPrim = elmMaster["GROSS_PREMIUM"].InnerText.Replace(".", ",");
                                    }
                                    else if (SfsProductionSchemaChildNodes[j].Name == "COVERAGES")
                                    {
                                        var CovarageNodes = SfsProductionSchemaChildNodes[j].ChildNodes;
                                        for (int y = 0; y < CovarageNodes.Count; y++)
                                        {
                                            XmlNode elm = CovarageNodes.Item(y);
                                            temNetPrim = "";
                                            temCoverAmount = "";
                                            temNetPrim = elm["NET_PREMIUM"].InnerText;
                                            temCoverAmount = elm["COVER_AMOUNT"].InnerText;
                                            //temNetPrim = elm["NET_PREMIUM"].InnerText.Replace(".", ",");
                                            //temCoverAmount = elm["COVER_AMOUNT"].InnerText.Replace(".", ",");
                                            switch (elm["COVER_CODE"].InnerText)
                                            {
                                                case GULF_KaskoTeminatlar.AracTemianti:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Kasko, Convert.ToDecimal(temCoverAmount), 0, Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }

                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.ArtanMaliSorumluluk:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.GLKHHT:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.GLKHHT, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.Deprem:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Deprem, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.Seylap:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Seylap, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.FerdiKaza:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.KFK_Olum, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.KFK_Surekli_Sakatlik, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                            if (!String.IsNullOrEmpty(temCoverAmount))
                                                            {
                                                                decimal tedaviTemianti = Convert.ToDecimal(temCoverAmount) / 10;
                                                                teminatList.Add(this.TeminatEkle(KaskoTeminatlar.KFK_Tedavi, tedaviTemianti, 0,
                                                               Convert.ToDecimal(temNetPrim), 0, 0));
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.AnahtarKaybi:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Anahtar_Kaybi, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.MiniOnarim:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Mini_Onrarim_Hizmeti, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.MedLine:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Medline, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.TasinanEmtia:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.TasinanYuk, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.HukuksalKoruma:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Hukuksal_Koruma, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                case GULF_KaskoTeminatlar.Yardim:
                                                    {
                                                        if (!String.IsNullOrEmpty(temNetPrim) && !String.IsNullOrEmpty(temCoverAmount))
                                                        {
                                                            teminatList.Add(this.TeminatEkle(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, Convert.ToDecimal(temCoverAmount), 0,
                                                                Convert.ToDecimal(temNetPrim), 0, 0));
                                                        }
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else if (xmlNode[i].Name == "ERROR" && Hata)
                    {
                        string hata = xmlNode[i].InnerText;
                        this.AddHata(hata);
                        _Serialize = new XmlSerializer(typeof(GULF_KaskoTeklifKayitHata_Response));
                        using (StringReader readerKaskoHata = new StringReader(responseKasko.OuterXml.ToString()))
                        {
                            GULF_KaskoTeklifKayitHata_Response kaskoResponseHata = new GULF_KaskoTeklifKayitHata_Response();
                            kaskoResponseHata = (GULF_KaskoTeklifKayitHata_Response)_Serialize.Deserialize(readerKaskoHata);
                            readerKaskoHata.ReadToEnd();
                            this.EndLog(kaskoResponseHata, false, kaskoResponseHata.GetType());
                        }
                    }
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
                if (!Hata)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = true;
                    this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                    this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                    this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                    //Local de bu kod gerekli canlı da gerekmiyor
                    //if (!String.IsNullOrEmpty(netPrim))
                    //{
                    //    netPrim = netPrim.Replace(".", ",");
                    //    this.GenelBilgiler.NetPrim = Convert.ToDecimal(netPrim);
                    //}
                    //if (!String.IsNullOrEmpty(brutPrim))
                    //{
                    //    brutPrim = brutPrim.Replace(".", ",");
                    //    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(brutPrim);
                    //}
                    this.GenelBilgiler.NetPrim = Convert.ToDecimal(netPrim);
                    this.GenelBilgiler.BrutPrim = Convert.ToDecimal(brutPrim);
                    this.GenelBilgiler.TUMTeklifNo = policeNo;
                    this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                    this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                    this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                    //Odeme Bilgileri
                    this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                    this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                    this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                    string pdfURL = this.GetGulfPdfURL(clnt, servisKullanici, policeNo, 1);
                    if (!String.IsNullOrEmpty(pdfURL))
                    {
                        this.GenelBilgiler.PDFDosyasi = pdfURL;
                    }
                }

                #endregion

                #region Web servis cevapları
                this.AddWebServisCevap(Common.WebServisCevaplar.Gulf_Teklif_Police_No, policeNo);
                this.AddWebServisCevap(Common.WebServisCevaplar.Gulf_Musteri_No, musteriNo);
                this.AddWebServisCevap(Common.WebServisCevaplar.Gulf_IslemTakipKodu, operationId);
                #endregion

                if (teminatList.Count > 0)
                {
                    this.Teminatlar = teminatList;
                }

                #endregion
                // }
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
            gulfsigorta.kasko.SFSPolicyIntegrationService clnt = new gulfsigorta.kasko.SFSPolicyIntegrationService();
            try
            {

                #region Veri Hazırlama GENEL

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.GULF });
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGulf);
                clnt.Timeout = 150000;
                clnt.Url = konfig[Konfig.GULF_KaskoServisURL];
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                GULF_KaskoPolice_Request request = new GULF_KaskoPolice_Request();
             
                #endregion

                #region Policelestirme Request

                string SigortaliNo = this.ReadWebServisCevap(Common.WebServisCevaplar.Gulf_Musteri_No, "0");
                string TUMPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.Gulf_Teklif_Police_No, "0");
                request.AUTH = new AUTH()
                {
                    EXT_CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    CALLER_GUID = servisKullanici.KullaniciAdi2,
                    USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                    PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),

                };
                request.POLICY = new Messages.Kasko.Police.POLICY()
                {
                    MASTER = new Messages.Kasko.Police.MASTER()
                    {
                        FIRM_CODE = "2",
                        COMPANY_CODE = "2",
                        PRODUCT_NO = "135",
                        POLICY_NO = TUMPoliceNo,
                        RENEWAL_NO = "0",
                        ENDORS_NO = "0",
                        CONFIRM_DATE = TurkeyDateTime.Now.ToString("dd/MM/yyyy"),
                    },

                };
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);
                request.POLICY.COLLECTION = new Messages.Kasko.Police.COLLECTION();
                List<Messages.Kasko.Police.DEBTOR> list = new List<Messages.Kasko.Police.DEBTOR>();
                //Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                string cardHolderName = String.Empty;
                string cardNumber = String.Empty;
                string month = String.Empty;
                string year = String.Empty;
                string cvv = String.Empty;
                int taksitSayisi = odeme.TaksitSayisi;

                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    cardHolderName = INET.Crypto.INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartSahibi);
                    cardNumber = INET.Crypto.INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartNo);
                    month = INET.Crypto.INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKA);
                    year = INET.Crypto.INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKY);
                    cvv = INET.Crypto.INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.CVC);
                }
                var odemeTipi = "";
                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.Nakit)
                {
                    odemeTipi = GULF_OdemeTipleri.Nakit;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti || teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    odemeTipi = GULF_OdemeTipleri.KrediKarti;
                }

                List<Messages.Kasko.Police.DEBTOR> deptorList = new List<Messages.Kasko.Police.DEBTOR>();
                Messages.Kasko.Police.DEBTOR deptor = new Messages.Kasko.Police.DEBTOR()
                {
                    DEBTOR_NO = SigortaliNo,
                    ADVANCED_PAYMENT_AMOUNT = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.ToString() : "0",
                    NUMBER_OF_INSTALLMENT = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value.ToString() : "1", //Taksit Sayısı
                    ADVANCE_PAYMENT_TOOL_TYPE = odemeTipi,
                    INSTALLMENTS_TOOL_TYPE = odemeTipi,//Taksit ödeme tipi Nakit: NK, Kredi Kartı:TK, Otomatik Ödeme:OO

                };
                List<CREDIT_CARD> krediKartBilgileri = new List<CREDIT_CARD>();
                CREDIT_CARD krediKartBilgi = new CREDIT_CARD();

                if (odemeTipi == GULF_OdemeTipleri.KrediKarti)
                {
                    krediKartBilgi.CARD_NO = cardNumber;
                    krediKartBilgi.CVV = cvv;
                    krediKartBilgi.NAME = cardHolderName;
                    krediKartBilgi.SURNAME = "";
                    krediKartBilgi.VALID_DATE = year + "/" + month;
                    krediKartBilgileri.Add(krediKartBilgi);
                    deptor.CREDIT_CARDS = krediKartBilgileri.ToArray();
                };
                deptorList.Add(deptor);
                request.POLICY.COLLECTION.DEBTORS = deptorList.ToArray();

                XmlSerializer _Serialize = new XmlSerializer(typeof(GULF_KaskoPolice_Request));
                StringWriter Output = new StringWriter(new StringBuilder());
                _Serialize.Serialize(Output, request);
                var responseKasko = clnt.CreateProposal(Output.ToString());
                Output.Close();
                clnt.Dispose();
                var xmlNode = responseKasko.ChildNodes;
                bool Hata = false;
                byte[] pdfUrlArray = new byte[1];
                for (int i = 0; i < xmlNode.Count; i++)
                {
                    if (xmlNode[i].Name == "RESULT")
                    {
                        var resultContent = xmlNode[i].InnerText;
                        if (resultContent == "1")
                        {
                            _Serialize = new XmlSerializer(typeof(Messages.Kasko.GULF_KaskoTeklifKayit_Response));
                            using (StringReader readerKasko = new StringReader(responseKasko.OuterXml.ToString()))
                            {
                                Messages.Kasko.GULF_KaskoTeklifKayit_Response kaskoResponse = new Messages.Kasko.GULF_KaskoTeklifKayit_Response();
                                kaskoResponse = (Messages.Kasko.GULF_KaskoTeklifKayit_Response)_Serialize.Deserialize(readerKasko);
                                readerKasko.ReadToEnd();
                                this.EndLog(kaskoResponse, true, kaskoResponse.GetType());
                            }
                        }
                        else if (resultContent == "0")
                        {
                            Hata = true;
                        }
                    }
                    else if (xmlNode[i].Name == "ERROR" && Hata)
                    {
                        string hata = xmlNode[i].InnerText;
                        this.AddHata(hata);
                        _Serialize = new XmlSerializer(typeof(GULF_PDFHata_Response));
                        using (StringReader readerKaskoHata = new StringReader(responseKasko.OuterXml.ToString()))
                        {
                            GULF_KaskoTeklifKayitHata_Response kaskoResponseHata = new GULF_KaskoTeklifKayitHata_Response();
                            kaskoResponseHata = (GULF_KaskoTeklifKayitHata_Response)_Serialize.Deserialize(readerKaskoHata);
                            readerKaskoHata.ReadToEnd();

                            this.EndLog(kaskoResponseHata, false, kaskoResponseHata.GetType());
                        }
                    }

                    if (!Hata)
                    {
                        this.GenelBilgiler.TUMPoliceNo = TUMPoliceNo;
                        this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                        this.GenelBilgiler.OdemeSekli = odeme.OdemeSekli;
                        this.GenelBilgiler.OdemeTipi = odeme.OdemeTipi;
                        this.GenelBilgiler.TaksitSayisi = odeme.TaksitSayisi;

                        string pdfURL = this.GetGulfPdfURL(clnt, servisKullanici, TUMPoliceNo, 2);
                        if (!String.IsNullOrEmpty(pdfURL))
                        {
                            this.GenelBilgiler.PDFPolice = pdfURL;
                        }
                        pdfURL = this.GetGulfPdfURL(clnt, servisKullanici, TUMPoliceNo, 19);
                        if (!String.IsNullOrEmpty(pdfURL))
                        {
                            this.GenelBilgiler.PDFDekont = pdfURL;
                        }
                        pdfURL = this.GetGulfPdfURL(clnt, servisKullanici, TUMPoliceNo, 6);
                        if (!String.IsNullOrEmpty(pdfURL))
                        {
                            this.GenelBilgiler.PDFBilgilendirme = pdfURL;
                        }
                    }
                    this.GenelBilgiler.WEBServisLogs = this.Log;
                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }


                #endregion

            }
            catch (Exception ex)
            {
                _Log.Error("GulfKasko.Policelestir", ex);
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }
        public string GetGulfPdfURL(gulfsigorta.kasko.SFSPolicyIntegrationService clntKasko, TVMWebServisKullanicilari servisKullanici, string policeNo, byte pdfTipi)
        {
            try
            {
                GULF_PDF_Request request = new GULF_PDF_Request();
                request.AUTH = new AUTH()
                {
                    EXT_CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    CALLER_GUID = servisKullanici.KullaniciAdi2,
                    USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                    PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                };
                request.POLICY_PRINTOUT = new Messages.PDF.POLICY_PRINTOUT()
                {
                    COMPANY_CODE = "2",
                    ENDORS_NO = "0",
                    FIRM_CODE = "2",
                    OUTPUT_FORMAT = "4",
                    POLICY_NO = policeNo,
                    PRINT_TYPE = "1",
                    PRODUCT_NO = "135",
                    RENEWAL_NO = "0",
                    CLIENT_IP = this.IpGetir(_AktifKullaniciService.TVMKodu)
                };
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

                XmlSerializer _Serialize = new XmlSerializer(typeof(GULF_PDF_Request));
                StringWriter Output = new StringWriter(new StringBuilder());
                _Serialize.Serialize(Output, request);
                var responsePdf = clntKasko.PrintDocument(Output.ToString());
                Output.Close();
                clntKasko.Dispose();
                var xmlNode = responsePdf.ChildNodes;
                bool Hata = false;
                byte[] pdfUrlArray = new byte[1];
                for (int i = 0; i < xmlNode.Count; i++)
                {
                    if (xmlNode[i].Name == "RESULT")
                    {
                        var resultContent = xmlNode[i].InnerText;
                        if (resultContent == "1")
                        {
                            _Serialize = new XmlSerializer(typeof(GULF_PDF_Response));
                            using (StringReader readerPdf = new StringReader(responsePdf.OuterXml.ToString()))
                            {
                                GULF_PDF_Response pdfResponse = new GULF_PDF_Response();
                                pdfResponse = (GULF_PDF_Response)_Serialize.Deserialize(readerPdf);
                                readerPdf.ReadToEnd();
                                this.EndLog(pdfResponse, true, pdfResponse.GetType());
                            }
                        }
                        else if (resultContent == "0")
                        {
                            Hata = true;
                        }
                    }
                    else if (xmlNode[i].Name == "DATA")
                    {
                        var policy_printout = xmlNode[i].ChildNodes;

                        for (int j = 0; j < policy_printout.Count; j++)
                        {
                            var pdf = policy_printout[j].ChildNodes;
                            for (int k = 0; k < pdf.Count; k++)
                            {
                                if (pdf[k].Name == "FILE")
                                {
                                    pdfUrlArray = Convert.FromBase64String(pdf[k].InnerText);
                                }
                            }
                        }
                    }
                    else if (xmlNode[i].Name == "ERROR" && Hata)
                    {
                        string hata = xmlNode[i].InnerText;
                        this.AddHata(hata);
                        _Serialize = new XmlSerializer(typeof(GULF_PDFHata_Response));
                        using (StringReader readerKaskoHata = new StringReader(responsePdf.OuterXml.ToString()))
                        {
                            GULF_PDFHata_Response kaskoResponseHata = new GULF_PDFHata_Response();
                            kaskoResponseHata = (GULF_PDFHata_Response)_Serialize.Deserialize(readerKaskoHata);
                            readerKaskoHata.ReadToEnd();

                            this.EndLog(kaskoResponseHata, false, kaskoResponseHata.GetType());
                        }
                    }
                }
                if (!Hata)
                {
                    if (pdfTipi == 1) //Teklif ise Kodu biz belirledik
                    {
                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        string fileName = String.Empty;
                        string url = String.Empty;
                        fileName = String.Format("GULF_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                        url = storage.UploadFile("kasko", fileName, pdfUrlArray);
                        return url;
                    }
                    else if (pdfTipi == 2) //Poliçe ise
                    {
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("GULF_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = pdfStorage.UploadFile("kasko", fileName, pdfUrlArray);
                        return url;
                    }
                    else if (pdfTipi == 19) //Kredi Kartı Slip ise
                    {
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("GULF_Kasko_Police_KrediKartiSlip{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = pdfStorage.UploadFile("kasko", fileName, pdfUrlArray);
                        return url;
                    }
                    else if (pdfTipi == 6) //Bilgilendirme formu ise
                    {
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("GULF_Kasko_Police_BilgilendirmeFormu{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = pdfStorage.UploadFile("kasko", fileName, pdfUrlArray);
                        return url;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _Log.Error("GulfKasko.TeklifPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                return String.Empty;
            }
        }

        public GULFMusteriBigileri GetGULFMusteriNo(string kimlikNo)
        {
            gulfsigorta.musteri.SFSEntityIntegrationService musteriClient = new gulfsigorta.musteri.SFSEntityIntegrationService();
            #region Müşteri No
            string MusteriNo = String.Empty;
            bool musteriKaydet = false;
            bool musteriVarMi = false;
            GULFMusteriBigileri gulfMusteriBilgi = new GULFMusteriBigileri();

            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGulf);
                int tvmkodu = _AktifKullaniciService.TVMKodu;
                var acente = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (acente.BagliOlduguTVMKodu != -9999)
                {
                    tvmkodu = acente.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.GULF });
                musteriClient.Url = konfig[Konfig.GULF_MusteriServisURL];
                musteriClient.Timeout = 150000;

                #endregion

                #region GULF Sigorta Müşteri Database Sorgu

                GULF_Musteri_DatabaseSorgu_Request musDatabaseSorguReq = new GULF_Musteri_DatabaseSorgu_Request();
                GULF_Musteri_DatabaseSorgu_Response musDatabaseSorguRes = new GULF_Musteri_DatabaseSorgu_Response();
                KaskoTeklif kasko = new KaskoTeklif();
                musDatabaseSorguReq.AUTH = new AUTH()
                {
                    USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                    PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                    EXT_CLIENT_IP = kasko.GetClientIP(),
                    CALLER_GUID = servisKullanici.KullaniciAdi2 // Db de başka alan olmadığı için caller guid id yi kullanıcı adı 2 alanına ekledik.
                };
                var gulfMusteriTipi = "";
                if (kimlikNo.Length > 10 && kimlikNo.Substring(0, 1) != "9")
                {
                    gulfMusteriTipi = GULF_MusteriTipleri.KimlikNo;
                }
                else if (kimlikNo.Length == 10)
                {
                    gulfMusteriTipi = GULF_MusteriTipleri.VergiNo;
                }
                else if (kimlikNo.Length > 10 && kimlikNo.Substring(0, 1) == "9")
                {
                    gulfMusteriTipi = GULF_MusteriTipleri.YabanciKimlikNo;
                }
                else
                {
                    gulfMusteriTipi = GULF_MusteriTipleri.PasaportNo;
                }

                musDatabaseSorguReq.ENTITY = new GULF.Messages.Musteri.ENTITY()
                {
                    ID_NUMBER = kimlikNo,
                    ID_TYPE = gulfMusteriTipi,
                    FIRM_CODE = "2"
                };
                this.BeginLog(musDatabaseSorguReq, musDatabaseSorguReq.GetType(), WebServisIstekTipleri.MusteriKayit);

                XmlSerializer _Serialize = new XmlSerializer(typeof(GULF_Musteri_DatabaseSorgu_Request));
                StringWriter _Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(_Output, musDatabaseSorguReq);
                var responseMusteriDatabaseSorgu = musteriClient.SearchEntity(_Output.ToString());
                _Output.Close();
                musteriClient.Dispose();

                var xmlNode = responseMusteriDatabaseSorgu.ChildNodes;
                for (int i = 0; i < xmlNode.Count; i++)
                {
                    if (xmlNode[i].Name == "RESULT")
                    {
                        var resultContent = xmlNode[i].InnerText;
                        if (resultContent == "1")
                        {
                            musteriVarMi = true;
                            _Serialize = new XmlSerializer(typeof(GULF_Musteri_DatabaseSorgu_Response));
                            using (StringReader readerMusteriSorgu = new StringReader(responseMusteriDatabaseSorgu.OuterXml.ToString()))
                            {
                                Business.GULF.Messages.GULF_Musteri_DatabaseSorgu_Response MusteriSorgu = new GULF_Musteri_DatabaseSorgu_Response();
                                MusteriSorgu = (GULF_Musteri_DatabaseSorgu_Response)_Serialize.Deserialize(readerMusteriSorgu);
                                readerMusteriSorgu.ReadToEnd();
                                this.EndLog(MusteriSorgu, true, MusteriSorgu.GetType());
                            }
                        }
                        else if (resultContent == "0")
                        {
                            musteriVarMi = false;
                            _Serialize = new XmlSerializer(typeof(GULF_Musteri_DatabaseSorguHata_Response));
                            using (StringReader readerMusteriSorgu = new StringReader(responseMusteriDatabaseSorgu.OuterXml.ToString()))
                            {
                                GULF_Musteri_DatabaseSorguHata_Response MusteriSorgu = new GULF_Musteri_DatabaseSorguHata_Response();
                                MusteriSorgu = (GULF_Musteri_DatabaseSorguHata_Response)_Serialize.Deserialize(readerMusteriSorgu);
                                readerMusteriSorgu.ReadToEnd();
                                this.EndLog(MusteriSorgu, false, MusteriSorgu.GetType());
                            }
                        }
                    }
                    else if (xmlNode[i].Name == "DATA" && musteriVarMi)
                    {
                        var xmlNodeEntity = xmlNode[i].ChildNodes;
                        if (xmlNodeEntity != null)
                        {
                            for (int j = 0; j < xmlNodeEntity.Count; j++)
                            {
                                var xmlNodeMaster = xmlNodeEntity[j].FirstChild;

                                if (xmlNodeMaster != null)
                                {
                                    var xmlNodeMasterChilds = xmlNodeMaster.ChildNodes;
                                    for (int k = 0; k < xmlNodeMasterChilds.Count; k++)
                                    {
                                        switch (xmlNodeMasterChilds[k].Name)
                                        {
                                            case "UNIT_NO": gulfMusteriBilgi.MusteriNo = xmlNodeMasterChilds[k].InnerText; break;
                                            case "NAME": gulfMusteriBilgi.Adi = xmlNodeMasterChilds[k].InnerText; break;
                                            case "FIRM_NAME": gulfMusteriBilgi.FirmaAdi = xmlNodeMasterChilds[k].InnerText; break;
                                            case "SURNAME": gulfMusteriBilgi.Soyadi = xmlNodeMasterChilds[k].InnerText; break;
                                            case "FATHER_NAME": gulfMusteriBilgi.BabaAdi = xmlNodeMasterChilds[k].InnerText; break;
                                            case "BIRTH_DATE": gulfMusteriBilgi.DogumTarihi = xmlNodeMasterChilds[k].InnerText; break;
                                            case "BIRTH_PLACE": gulfMusteriBilgi.DogumYeri = xmlNodeMasterChilds[k].InnerText; break;
                                            case "GENDER": gulfMusteriBilgi.Cinsiyeti = xmlNodeMasterChilds[k].InnerText; break;
                                            case "TOWN_CODE": gulfMusteriBilgi.IlceKodu = xmlNodeMasterChilds[k].InnerText; break;
                                            case "CITY_CODE": gulfMusteriBilgi.IlKodu = xmlNodeMasterChilds[k].InnerText; break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region GULF Sigorta Müşteri Kaydet
                if (!musteriVarMi)
                {
                    GULF_Musteri_Kaydetme_Request musKayReq = new GULF_Musteri_Kaydetme_Request();
                    GULF_Musteri_Kaydetme_Response musKaydRes = new GULF_Musteri_Kaydetme_Response();

                    musKayReq.AUTH = new AUTH()
                    {
                        USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                        PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                        EXT_CLIENT_IP = kasko.GetClientIP(),
                        CALLER_GUID = servisKullanici.KullaniciAdi2
                    };
                    musKayReq.ENTITY = new Messages.Musteri.Kaydet.ENTITYY();

                    musKayReq.ENTITY.MASTER = new Messages.Musteri.Kaydet.MASTER()
                    {
                        COUNTRY_CODE = "90",
                        NATIONALITY = "90",
                        CONNECT_ADDRESS = "1",
                        MARITAL_STATUS = "E",
                        RESIDENT_IN_STATE = "E",
                    };
                    if (gulfMusteriTipi == GULF_MusteriTipleri.KimlikNo)
                    {
                        musKayReq.ENTITY.MASTER.CITIZENSHIP_NUMBER = kimlikNo;
                        musKayReq.ENTITY.MASTER.PERSONAL_COMMERCIAL = "O";
                        musKayReq.ENTITY.MASTER.ADDRESS1 = "MERKEZ";
                    }
                    else if (gulfMusteriTipi == GULF_MusteriTipleri.VergiNo)
                    {
                        musKayReq.ENTITY.MASTER.TAX_NO = kimlikNo;
                        musKayReq.ENTITY.MASTER.PERSONAL_COMMERCIAL = "T";
                        musKayReq.ENTITY.MASTER.ADDRESS1 = "İstanbul";

                    }
                    else if (gulfMusteriTipi == GULF_MusteriTipleri.YabanciKimlikNo)
                    {
                        musKayReq.ENTITY.MASTER.FOREIGN_CITIZENSHIP_NUMBER = kimlikNo;
                    }
                    musKayReq.ENTITY.MASTER.PHONE_COUNTRY_CODE1 = "90";
                    musKayReq.ENTITY.MASTER.PHONE_CODE1 = "212";
                    musKayReq.ENTITY.MASTER.PHONE_NUMBER1 = "555";
                    musKayReq.ENTITY.MASTER.PHONE_LINE1 = "55";

                    #region Adres


                    List<Messages.Musteri.Kaydet.ADDRESS> adresList = new List<Messages.Musteri.Kaydet.ADDRESS>();
                    Messages.Musteri.Kaydet.ADDRESS adres = new Messages.Musteri.Kaydet.ADDRESS();

                    if (gulfMusteriTipi == GULF_MusteriTipleri.VergiNo)
                    {
                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "UL";
                        adres.ADR_DATA = "TÜRKİYE";
                        adresList.Add(adres);

                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "İL";
                        adres.ADR_DATA = "İSTANBUL";
                        adresList.Add(adres);

                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "İÇ";
                        adres.ADR_DATA = "MERKEZ";
                        adresList.Add(adres);
                    }
                    else
                    {
                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "UL";
                        adres.ADR_DATA = "";
                        adresList.Add(adres);

                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "İL";
                        adres.ADR_DATA = "";
                        adresList.Add(adres);

                        adres = new Messages.Musteri.Kaydet.ADDRESS();
                        adres.WHICH_ADRESS = "1";
                        adres.ADR_TYPE = "İÇ";
                        adres.ADR_DATA = "";
                        adresList.Add(adres);
                    }

                    adres = new Messages.Musteri.Kaydet.ADDRESS();
                    adres.WHICH_ADRESS = "1";
                    adres.ADR_TYPE = "MH";
                    adres.ADR_DATA = "";
                    adresList.Add(adres);

                    adres = new Messages.Musteri.Kaydet.ADDRESS();
                    adres.WHICH_ADRESS = "1";
                    adres.ADR_TYPE = "CD";
                    adres.ADR_DATA = "";
                    adresList.Add(adres);

                    adres = new Messages.Musteri.Kaydet.ADDRESS();
                    adres.WHICH_ADRESS = "1";
                    adres.ADR_TYPE = "SK";
                    adres.ADR_DATA = "";
                    adresList.Add(adres);

                    adres = new Messages.Musteri.Kaydet.ADDRESS();
                    adres.WHICH_ADRESS = "1";
                    adres.ADR_TYPE = "BN";
                    adres.ADR_DATA = "";
                    adresList.Add(adres);

                    adres = new Messages.Musteri.Kaydet.ADDRESS();
                    adres.WHICH_ADRESS = "1";
                    adres.ADR_TYPE = "PK";
                    adres.ADR_DATA = "";
                    adresList.Add(adres);

                    musKayReq.ENTITY.ADDRESSES = adresList.ToArray();

                    #endregion

                    this.BeginLog(musKayReq, musKayReq.GetType(), WebServisIstekTipleri.MusteriKayit);

                    _Serialize = new XmlSerializer(typeof(GULF_Musteri_Kaydetme_Request));
                    _Output = new StringWriter(new StringBuilder());

                    _Serialize.Serialize(_Output, musKayReq);
                    var responseMusteriKaydet = musteriClient.SaveEntity(_Output.ToString());
                    _Output.Close();
                    musteriClient.Dispose();
                    gulfMusteriBilgi = new GULFMusteriBigileri();

                    var xmlNodeMusteriKaydet = responseMusteriKaydet.ChildNodes;
                    for (int i = 0; i < xmlNodeMusteriKaydet.Count; i++)
                    {
                        if (xmlNodeMusteriKaydet[i].Name == "RESULT")
                        {
                            var resultContent = xmlNodeMusteriKaydet[i].InnerText;
                            if (resultContent == "1")
                            {
                                musteriKaydet = true;
                                _Serialize = new XmlSerializer(typeof(GULF_Musteri_Kaydetme_Response));
                                using (StringReader readerMusteriKaydet = new StringReader(responseMusteriKaydet.OuterXml.ToString()))
                                {
                                    GULF_Musteri_Kaydetme_Response MusteriKaydet = new GULF_Musteri_Kaydetme_Response();
                                    MusteriKaydet = (GULF_Musteri_Kaydetme_Response)_Serialize.Deserialize(readerMusteriKaydet);
                                    readerMusteriKaydet.ReadToEnd();
                                    this.EndLog(MusteriKaydet, true, MusteriKaydet.GetType());
                                }
                            }
                            else if (resultContent == "0")
                            {
                                musteriKaydet = false;
                                _Serialize = new XmlSerializer(typeof(GULF_Musteri_KaydetmeHata_Response));
                                using (StringReader readerMusteriKaydet = new StringReader(responseMusteriKaydet.OuterXml.ToString()))
                                {
                                    GULF_Musteri_KaydetmeHata_Response MusteriKaydet = new GULF_Musteri_KaydetmeHata_Response();
                                    MusteriKaydet = (GULF_Musteri_KaydetmeHata_Response)_Serialize.Deserialize(readerMusteriKaydet);
                                    readerMusteriKaydet.ReadToEnd();
                                    this.EndLog(MusteriKaydet, true, MusteriKaydet.GetType());
                                }

                            }
                        }
                        else if (xmlNodeMusteriKaydet[i].Name == "DATA" && musteriKaydet)
                        {
                            var xmlNodeEntity = xmlNodeMusteriKaydet[i].ChildNodes;
                            if (xmlNodeEntity != null)
                            {
                                for (int j = 0; j < xmlNodeEntity.Count; j++)
                                {
                                    var xmlNodeEntityChilds = xmlNodeEntity[j].ChildNodes;
                                    for (int k = 0; k < xmlNodeEntityChilds.Count; k++)
                                    {
                                        if (xmlNodeEntityChilds[k].Name == "UNIT_NO")
                                        {
                                            gulfMusteriBilgi.MusteriNo = xmlNodeEntityChilds[k].InnerText;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (xmlNodeMusteriKaydet[i].Name == "ERROR" && !musteriKaydet)
                        {
                            gulfMusteriBilgi.HataMesaji = xmlNodeMusteriKaydet[i].InnerText;
                            _Serialize = new XmlSerializer(typeof(GULF_KaskoTeklifKayitHata_Response));
                            using (StringReader readerKaskoHata = new StringReader(responseMusteriKaydet.OuterXml.ToString()))
                            {
                                GULF_KaskoTeklifKayitHata_Response kaskoResponseHata = new GULF_KaskoTeklifKayitHata_Response();
                                kaskoResponseHata = (GULF_KaskoTeklifKayitHata_Response)_Serialize.Deserialize(readerKaskoHata);
                                readerKaskoHata.ReadToEnd();
                                this.EndLog(kaskoResponseHata, false, kaskoResponseHata.GetType());
                                gulfMusteriBilgi.HataMesaji = kaskoResponseHata.ERROR;
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                gulfMusteriBilgi.HataMesaji = "Gulf Sigorta müşteri sorgulama/kaydetme işleminde bir hata oluştu." + ex.Message;
            }

            if (musteriKaydet)
            {
                return this.GetGULFMusteriNo(kimlikNo);
                //musteriKaydet = false;
            }
            return gulfMusteriBilgi;

        }

        public GULFAracBilgileri GetGULFAracBilgiSorgu(string kimlikNo, string PlakaKodu, string PlakaNo)
        {
            gulfsigorta.plakasorgu.SFSPolicyIntegrationService Client = new gulfsigorta.plakasorgu.SFSPolicyIntegrationService();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGulf);
            Client.Timeout = 150000;
            Client.Url = konfig[Konfig.GULF_PlakaSorguServisURL];
            GULF_AracBilgiSorgu_Request request = new GULF_AracBilgiSorgu_Request();
            int tvmkodu = _AktifKullaniciService.TVMKodu;
            var acente = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
            if (acente.BagliOlduguTVMKodu != -9999)
            {
                tvmkodu = acente.BagliOlduguTVMKodu;
            }

            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.GULF });
            GULFAracBilgileri aracBilgi = new GULFAracBilgileri();
            aracBilgi.policeBilgi = new PoliceBilgi();

            KaskoTeklif kasko = new KaskoTeklif();

            string gulfKimlikTipi = "1";
            if (kimlikNo.Length == 11)
            {
                gulfKimlikTipi = GULF_KimlikTipleri.TCKimlik;
            }
            else if (kimlikNo.Length == 10)
            {
                gulfKimlikTipi = GULF_KimlikTipleri.VergiKimlik;
            }

            try
            {
                #region Gulf Kasko Poliçe Sorgu
                request.AUTH = new AUTH()
                {
                    USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                    PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                    EXT_CLIENT_IP = kasko.GetClientIP(), //"81.214.50.9",
                    CALLER_GUID = servisKullanici.KullaniciAdi2
                };
                request.MASTER = new Messages.Arac.MASTER();
                request.MASTER.KIMLIKNO = kimlikNo;
                request.MASTER.KIMLIKTIPI = gulfKimlikTipi;
                request.MASTER.PLAKAILKODU = PlakaKodu;
                request.MASTER.PLAKANO = PlakaNo;

                XmlSerializer _Serialize = new XmlSerializer(typeof(GULF_AracBilgiSorgu_Request));
                StringWriter Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, request);
                var responseAracBilgi = Client.kaskopoliceSorguByKimlikPlaka(Output.ToString());
                bool aracBilgiVarMi = false;
                var xmlNode = responseAracBilgi.ChildNodes;
                for (int i = 0; i < xmlNode.Count; i++)
                {
                    if (xmlNode[i].Name == "RESULT")
                    {
                        var resultContent = xmlNode[i].InnerText;
                        if (resultContent == "1")
                        {
                            aracBilgiVarMi = true;
                        }
                        else if (resultContent == "0")
                        {
                            aracBilgiVarMi = false;
                            this.EndLog(xmlNode, false, xmlNode.GetType());
                        }
                    }
                    else if (xmlNode[i].Name == "DATA" && aracBilgiVarMi)
                    {
                        var aracBilgileri = xmlNode[i].FirstChild.ChildNodes;
                        if (aracBilgileri != null)
                        {
                            for (int j = 0; j < aracBilgileri.Count; j++)
                            {
                                switch (aracBilgileri[j].Name)
                                {
                                    case "arac":

                                    default:
                                        break;
                                }

                                if (aracBilgileri[j].Name == "arac")
                                {
                                    var xmlNodeArac = aracBilgileri[j].ChildNodes;

                                    for (int aracIndex = 0; aracIndex < xmlNodeArac.Count; aracIndex++)
                                    {
                                        if (xmlNodeArac[aracIndex].Name == "aracMarkaKodu")
                                        {
                                            aracBilgi.MarkaKodu = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "aracTarifeGrupKodu")
                                        {
                                            aracBilgi.AracTarifeGrupKodu = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "aracTipKodu")
                                        {
                                            aracBilgi.AracTipKodu = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "kullanimSekli")
                                        {
                                            aracBilgi.KullanimSekli = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "modelYili")
                                        {
                                            aracBilgi.ModelYili = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "motorNo")
                                        {
                                            aracBilgi.MotorNo = xmlNodeArac[aracIndex].InnerText;
                                        }
                                        else if (xmlNodeArac[aracIndex].Name == "sasiNo")
                                        {
                                            aracBilgi.SasiNo = xmlNodeArac[aracIndex].InnerText;
                                        }
                                    }
                                }
                                //else if (aracBilgileri[j].Name == "oncekiAcenteNo")
                                //{
                                //    aracBilgi.policeBilgi.AcenteKod = aracBilgileri[j].InnerText;
                                //}
                                //else if (aracBilgileri[j].Name == "oncekiPoliceNo")
                                //{
                                //    aracBilgi.policeBilgi.PoliceNo = aracBilgileri[j].InnerText;
                                //}
                                //else if (aracBilgileri[j].Name == "oncekiSirketKodu")
                                //{
                                //    aracBilgi.policeBilgi.SirketKodu = aracBilgileri[j].InnerText;
                                //}
                                //else if (aracBilgileri[j].Name == "oncekiYenilemeNo")
                                //{
                                //    aracBilgi.policeBilgi.YenilemeNo = aracBilgileri[j].InnerText;
                                //}
                                else if (aracBilgileri[j].Name == "acenteNo")
                                {
                                    aracBilgi.policeBilgi.AcenteKod = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "policeNo")
                                {
                                    aracBilgi.policeBilgi.PoliceNo = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "sirketKodu")
                                {
                                    aracBilgi.policeBilgi.SirketKodu = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "yenilemeNo")
                                {
                                    aracBilgi.policeBilgi.YenilemeNo = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "policeBaslamaTarihi")
                                {
                                    aracBilgi.PoliceBaslangicTarihi = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "policeBitisTarihi")
                                {
                                    aracBilgi.PoliceBitisTarihi = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "sbmTramerNo")
                                {
                                    aracBilgi.SBMTramerNo = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "tanzimTarihi")
                                {
                                    aracBilgi.TanzimTarihi = aracBilgileri[j].InnerText;
                                }
                                else if (aracBilgileri[j].Name == "uygulananKademe")
                                {
                                    aracBilgi.UygulananKademe = aracBilgileri[j].InnerText;
                                }
                            }
                        }
                    }

                    else if (xmlNode[i].Name == "ERROR" && !aracBilgiVarMi)
                    {
                        aracBilgi.HataMesaji = xmlNode[i].InnerText;
                    }
                }

                #endregion
                #region GULF EGM Sorgu
                if (!String.IsNullOrEmpty(aracBilgi.MotorNo) && !String.IsNullOrEmpty(aracBilgi.SasiNo) && !String.IsNullOrEmpty(aracBilgi.ModelYili))
                {
                    GULF_EGMSorgu_Request egmRequest = new GULF_EGMSorgu_Request();
                    egmRequest.AUTH = new AUTH()
                    {
                        USER_NAME = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi),
                        PASSWORD = INET.Crypto.INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre),
                        EXT_CLIENT_IP = kasko.GetClientIP(),
                        CALLER_GUID = servisKullanici.KullaniciAdi2
                    };
                    egmRequest.MASTER = new Messages.Arac.EGM.MASTER();
                    egmRequest.MASTER.CACHESTYLE = "1";
                    egmRequest.MASTER.MODELYILI = aracBilgi.ModelYili;
                    egmRequest.MASTER.MOTORNO = aracBilgi.MotorNo;
                    egmRequest.MASTER.SASINO = aracBilgi.SasiNo;

                    _Serialize = new XmlSerializer(typeof(GULF_EGMSorgu_Request));
                    Output = new StringWriter(new StringBuilder());

                    _Serialize.Serialize(Output, egmRequest);
                    var responseEGM = Client.EgmMsmSorgu(Output.ToString());
                    Client.Dispose();
                    bool EGMSonucVarMi = false;
                    var xmlNodeEGM = responseEGM.ChildNodes;
                    for (int i = 0; i < xmlNodeEGM.Count; i++)
                    {
                        if (xmlNodeEGM[i].Name == "RESULT")
                        {
                            var resultContent = xmlNodeEGM[i].InnerText;
                            if (resultContent == "1")
                            {
                                EGMSonucVarMi = true;
                            }
                            else if (resultContent == "0")
                            {
                                EGMSonucVarMi = false;
                                this.EndLog(responseEGM, false, responseEGM.GetType());
                                break;
                            }
                        }
                        else if (xmlNodeEGM[i].Name == "DATA" && EGMSonucVarMi)
                        {
                            var aracBilgileri = xmlNodeEGM[i].FirstChild.ChildNodes;
                            if (aracBilgileri != null)
                            {
                                for (int j = 0; j < aracBilgileri.Count; j++)
                                {

                                    if (aracBilgileri[j].Name == "aracBilgi")
                                    {
                                        var xmlNodeArac = aracBilgileri[j].ChildNodes;

                                        for (int aracIndex = 0; aracIndex < xmlNodeArac.Count; aracIndex++)
                                        {
                                            switch (xmlNodeArac[aracIndex].Name)
                                            {
                                                case "koltukSayisi": aracBilgi.KoltukSayisi = xmlNodeArac[aracIndex].InnerText; break;
                                                case "silindirHacmi": aracBilgi.SilindirHacmi = xmlNodeArac[aracIndex].InnerText; break;
                                                case "yakitTipi": aracBilgi.YakitTipi = xmlNodeArac[aracIndex].InnerText; break;
                                                case "motorGucu": aracBilgi.KoltukSayisi = aracBilgi.MotorGucu = xmlNodeArac[aracIndex].InnerText; break;
                                                case "renk": aracBilgi.Renk = xmlNodeArac[aracIndex].InnerText; break;
                                            }
                                        }
                                    }
                                    else if (aracBilgileri[j].Name == "aracTescilBilgileri")
                                    {
                                        var xmlNodeTescil = aracBilgileri[j].ChildNodes;

                                        for (int aracIndex = 0; aracIndex < xmlNodeTescil.Count; aracIndex++)
                                        {
                                            if (xmlNodeTescil[aracIndex].Name == "tescilTarihi")
                                            {
                                                aracBilgi.TescilTarihi = xmlNodeTescil[aracIndex].InnerText;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                aracBilgi.HataMesaji = "Plaka Sorgulama servisinde bir hata oluştu. " + ex.Message;
            }
            return aracBilgi;
        }

        public string IpGetir(int tvmKodu)
        {
            //string cagriIP = "82.222.165.62";
            //string sigortaShopIp = " 94.54.67.159";
            string mbGrupIp = "81.214.50.9";//MB Grup İp
            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo; // "88.249.209.253"; Birebir ip 
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                {
                    gonderilenIp = mbGrupIp;
                }
            }
        // return mbGrupIp;
          return gonderilenIp;

        }
        public TeklifTeminat TeminatEkle(int teminatKodu, decimal tutar, decimal vergi, decimal netprim, decimal brutprim, int adet)
        {
            TeklifTeminat teminat = new TeklifTeminat();
            teminat.TeklifId = this.GenelBilgiler.TeklifId;
            teminat.TeminatKodu = teminatKodu;
            teminat.TeminatBedeli = tutar;
            teminat.TeminatVergi = vergi;
            teminat.TeminatNetPrim = netprim;
            teminat.TeminatBrutPrim = brutprim;
            teminat.Adet = adet;

            return teminat;
        }
    }
}
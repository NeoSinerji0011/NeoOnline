using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System;
using System.Collections.Generic;
using EurekoSigorta_Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.eurekosigorta.musteriV2;
using System.Xml;
using System.Xml.Serialization;
using EurekoSigorta_Business.Common.EUREKO;
using Neosinerji.BABOnlineTP.Business.allianz;
using Neosinerji.BABOnlineTP.Business.Common.ALLIANZ;
namespace Neosinerji.BABOnlineTP.Business.ALLIANZ
{
    public class ALLIANZTrafik : Teklif, IALLIANZTrafik
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifkullaniciService;
        IUlkeService _UlkeService;

        [InjectionConstructor]
        public ALLIANZTrafik(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IAktifKullaniciService aktifkullanici, IUlkeService ulkeService)
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _AktifkullaniciService = aktifkullanici;
            _UlkeService = ulkeService;
        }


        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.ALLIANZ;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Veri Hazırlama

                GenericProcessExecutorService client = new GenericProcessExecutorService();
                client.Url = "http://test.services.allianz.com.tr/gws/GPE?wsdl";
                client.Timeout = 150000;


                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.KaydiEKleyenTVMKodu, TeklifUretimMerkezleri.ALLIANZ });
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                TeklifSigortali sigortali = teklif.Sigortalilar.FirstOrDefault(f => f.TeklifId == teklif.GenelBilgiler.TeklifId);
                string AllianzMusteriTipi = "";
                bool MusteriAyniMi = false;

                MusteriGenelBilgiler musteriBilgileri = _MusteriService.GetMusteri(sigortali.MusteriKodu);
                MusteriTelefon musteriTelefon = musteriBilgileri.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortali.MusteriKodu);
                MusteriAdre musteriAdres = musteriBilgileri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                MusteriGenelBilgiler SE_Bilgileri = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                MusteriTelefon SE_Telefon = SE_Bilgileri.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortaEttiren.MusteriKodu);
                MusteriAdre SE_Adres = SE_Bilgileri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);


                if (sigortali.MusteriKodu == sigortaEttiren.MusteriKodu)
                {
                    MusteriAyniMi = true;
                }
                if (musteriBilgileri.MusteriTipKodu == MusteriTipleri.TCMusteri)
                {
                    AllianzMusteriTipi = ALLIANZ_MusteriTipleri.Ozel;
                }
                else if (musteriBilgileri.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                {
                    AllianzMusteriTipi = ALLIANZ_MusteriTipleri.Tuzel;
                }
                else if (musteriBilgileri.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                {
                    AllianzMusteriTipi = ALLIANZ_MusteriTipleri.Yabanci;
                }

                #region fields
                List<field> fields = new List<field>();

                field field = new allianz.field();
                //field.fieldName = "noasync";
                //field.value = "true";
                //fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.authToken";
                field.value = "B|" + servisKullanici.KullaniciAdi + "|" + servisKullanici.Sifre + ""; //kullanıcı adı ve şifreyi gireceğiniz alan B|<kullanıcı adı>|<şifre> formatında doldurulmalıdır. Buradaki ‘B’ brokerı temsil eder. 
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.uniqueId";
                field.value = "";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.agencyRepIdentityNo"; //bu işlemi gerçekleştiren ve sistemde kayıtlı olan kişini TCK’si olmalıdır.
                field.value = "47236273778"; //Teknik Personel tc num. 31516261982"
                fields.Add(field);

                #region Önceki Poliçe
                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                string oncekiPoliceNo = "";
                string isRenewal = "H";

                //if (eskiPoliceVar)
                //{
                //    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //    isRenewal = "E";
                //}
                #endregion


                field = new allianz.field();
                field.fieldName = "obj.isRenewal";
                field.value = isRenewal;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.oldPolicyNum";
                field.value = oncekiPoliceNo;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.roleType";
                field.value = AllianzMusteriTipi;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.citizenshipNo";
                field.value = musteriBilgileri.MusteriTipKodu == MusteriTipleri.TCMusteri ? musteriBilgileri.KimlikNo : "";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.taxNo";
                field.value = musteriBilgileri.MusteriTipKodu == MusteriTipleri.TuzelMusteri ? musteriBilgileri.KimlikNo : "";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.foreignerIdentityNo";
                field.value = musteriBilgileri.MusteriTipKodu == MusteriTipleri.YabanciMusteri ? musteriBilgileri.KimlikNo : "";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.passportNo";
                field.value = "";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.countryCode";
                field.value = "TR";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.address_type";
                field.value = musteriBilgileri.MusteriTipKodu == MusteriTipleri.TCMusteri ? ALLIANZ_AdresTipleri.Ev : ALLIANZ_AdresTipleri.Is;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.country";
                field.value = "TR";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.city";
                field.value = musteriAdres.IlKodu.PadLeft(3, '0');
                fields.Add(field);

                Ilce ilce = _UlkeService.GetIlce(musteriAdres.IlceKodu.Value);

                string AllianzIlceKodu = getAllianzIlceKodu(client, musteriAdres.IlKodu, ilce.IlceAdi);

                field = new allianz.field();
                field.fieldName = "obj.address.town";
                field.value = !String.IsNullOrEmpty(AllianzIlceKodu) ? AllianzIlceKodu : "MERKEZ";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.neighborhood";
                field.value = !String.IsNullOrEmpty(musteriAdres.Mahalle) ? musteriAdres.Mahalle : "MERKEZ";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.street";
                field.value = !String.IsNullOrEmpty(musteriAdres.Cadde) ? musteriAdres.Cadde : "MERKEZ";
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.address.substreet";
                field.value = !String.IsNullOrEmpty(musteriAdres.Sokak) ? musteriAdres.Sokak : "MERKEZ";
                fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.address.road";
                //field.value = "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.address.buildingname";
                //field.value = !String.IsNullOrEmpty(musteriAdres.Apartman) ? musteriAdres.Apartman : "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.address.doornumber";
                //field.value = !String.IsNullOrEmpty(musteriAdres.BinaNo) ? musteriAdres.BinaNo : "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.address.floornumber";
                //field.value = !String.IsNullOrEmpty(musteriAdres.DaireNo) ? musteriAdres.DaireNo : "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.address.zipcode";
                //field.value = musteriAdres.PostaKodu != null ? musteriAdres.PostaKodu.ToString() : "";
                //fields.Add(field);

                string cepTelefon="5559999999";
                if (!String.IsNullOrEmpty(musteriTelefon.Numara))
                {
                    cepTelefon = musteriTelefon.Numara.Substring(3, 3);
                    cepTelefon += musteriTelefon.Numara.Substring(7, 7);
                }
                field = new allianz.field();
                field.fieldName = "obj.inforDetails.gsmNum";
                field.value = cepTelefon;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.inforDetails.homeNum";
                field.value = cepTelefon;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.inforDetails.workNum";
                field.value = cepTelefon;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.inforDetails.email";
                field.value = musteriBilgileri.EMail;
                fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.isInsurerInsuredSame";
                field.value = MusteriAyniMi ? "E" : "H"; //Sigortalı ve ettiren aynı mı
                fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerRoleType";
                //field.value = AllianzMusteriTipi;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerCitizenshipNo";
                //field.value = MusteriAyniMi ? "" : sigortaEttiren.MusteriGenelBilgiler.KimlikNo;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerTaxNo";
                //field.value = MusteriAyniMi ? "" : musteriBilgileri.MusteriTipKodu == MusteriTipleri.TuzelMusteri ? musteriBilgileri.KimlikNo : "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerForeignerIdentityNo";
                //field.value = MusteriAyniMi ? "" : musteriBilgileri.MusteriTipKodu == MusteriTipleri.YabanciMusteri ? musteriBilgileri.KimlikNo : "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerPassportNo";
                //field.value = "";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerCountryCode";
                //field.value = "TR";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.address_type";
                //field.value = MusteriAyniMi ?  musteriBilgileri.MusteriTipKodu == MusteriTipleri.TCMusteri ? ALLIANZ_AdresTipleri.Ev : ALLIANZ_AdresTipleri.Is : sigortaEttiren.MusteriGenelBilgiler.MusteriTipKodu == MusteriTipleri.TCMusteri ? ALLIANZ_AdresTipleri.Ev : ALLIANZ_AdresTipleri.Is;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.country";
                //field.value = "TR";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.city";
                //field.value = MusteriAyniMi ? musteriAdres.IlKodu.PadLeft(3, '0') : SE_Adres.IlKodu.PadLeft(3, '0');
                //fields.Add(field);


                //Ilce se_ilce = _UlkeService.GetIlce(SE_Adres.IlceKodu.Value);

                //string AllianzSEIlceKodu = getAllianzIlceKodu(client, SE_Adres.IlKodu, se_ilce.IlceAdi);


                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.town";
                //field.value = MusteriAyniMi ? AllianzIlceKodu : !String.IsNullOrEmpty(AllianzSEIlceKodu) ? AllianzSEIlceKodu : "-1";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.neighborhood";
                //field.value = "MERKEZ";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.street";
                //field.value = "MERKEZ";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.substreet";
                //field.value = "MERKEZ";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.road";
                //field.value = "MERKEZ";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.buildingname";
                //field.value = "MERKEZ";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.doornumber";
                //field.value = "1";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerAddress.floornumber";
                //field.value = "1";
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerInforDetails.gsmNum";
                //field.value = cepTelefon;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerInforDetails.homeNum";
                //field.value = cepTelefon;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerInforDetails.workNum";
                //field.value = cepTelefon;
                //fields.Add(field);

                //field = new allianz.field();
                //field.fieldName = "obj.insurerInforDetails.email";
                //field.value = musteriBilgileri.EMail;
                //fields.Add(field);

                field = new allianz.field();
                field.fieldName = "obj.permissionBasedMarketing";
                field.value = "E";
                fields.Add(field);

                #endregion

                objectData objectData = new objectData()
                {
                    fields = fields.ToArray(),
                    objcetName = "TrafikServiceExternalQuery",
                };
                #endregion

                #region Servis call
                var trafikResponse = this.SorguYap(objectData, client);
                string AllianzHatalar = "";
                if (trafikResponse.serviceMessages.ToList().Count > 1)
                {
                    foreach (var item in trafikResponse.serviceMessages.ToList())
                    {
                        AllianzHatalar += item.message;
                    }

                    this.AddHata(AllianzHatalar);
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

                //this.GenelBilgiler.TUMTeklifNo = response.OutPolicyMaster.PolicyNum;
                //this.GenelBilgiler.Basarili = true;
                //this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                //this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                //this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                //this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;


                //decimal GrfTl = 0;
                //decimal ThfTl = 0;
                //decimal YsvTl = 0;
                //decimal GdvTl = 0;
                //if (InfoResonseSuccess)
                //{

                //    this.GenelBilgiler.BrutPrim = InfoResponse.OutPolicyMaster.TotalGrossPremiumAmount;
                //    this.GenelBilgiler.NetPrim = InfoResponse.OutPolicyMaster.TotalNetPremiumAmount;
                //    this.GenelBilgiler.ToplamKomisyon = InfoResponse.OutPolicyMaster.AgencyTotalCommissionAmount;
                //    this.GenelBilgiler.ToplamVergi = InfoResponse.OutPolicyMaster.TotalTaxAmount;
                //    this.GenelBilgiler.DovizKurBedeli = InfoResponse.OutPolicyMaster.ExchangeRate;
                //    GrfTl = InfoResponse.OutComponentTaxTotals.GrfTl;
                //    ThfTl = InfoResponse.OutComponentTaxTotals.ThfTl;
                //    YsvTl = InfoResponse.OutComponentTaxTotals.YsvTl;
                //    GdvTl = InfoResponse.OutComponentTaxTotals.GdvTl;
                //}
                ////this.GenelBilgiler.TaksitSayisi = 1;
                ////this.GenelBilgiler.GecikmeZammiYuzdesi = RAYMessages.ToDecimal(RAY_GecikmeOrani);
                ////this.GenelBilgiler.ZKYTMSYüzdesi = RAYMessages.ToDecimal(RAY_ZKYTMSIndirimi);


                //// ==== Güncellenicek. ==== //
                ////this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                ////this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                //// Odeme Bilgileri
                //this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                //this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                //this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                //#endregion

                ////trafikvergilerde ysv yok ??
                //#region Vergiler
                //this.AddVergi(TrafikVergiler.THGFonu, ThfTl);
                //this.AddVergi(TrafikVergiler.GiderVergisi, GdvTl);
                //this.AddVergi(TrafikVergiler.GarantiFonu, GrfTl);
                ////this.AddVergi(TrafikVergiler.YanginVergisi, YsvTl);
                #endregion

                ////tüm teminatlar bunlar mı??
                //#region Teminatlar
                //if (teminatResponseSuccess)
                //{
                //    decimal tutar = 0;
                //    decimal brutPrim = 0;
                //    decimal netPrim = 0;
                //    decimal vergi = 0;

                //    var ucuncuSahisMaddiZararlar = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisMaddiZararlar);

                //    if (ucuncuSahisMaddiZararlar != null)
                //    {
                //        tutar = ucuncuSahisMaddiZararlar.OutPolicySumInsured.SumInsuredAmount;
                //        netPrim = ucuncuSahisMaddiZararlar.OutPolicySumInsured.AccruePremiumAmount;
                //        vergi = ucuncuSahisMaddiZararlar.OutPolicyTax.TaxAmount;
                //        brutPrim = ucuncuSahisMaddiZararlar.OutGrossPolicySumInsured.AccruePremiumAmount;
                //        this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, tutar, vergi, netPrim, brutPrim, 0);
                //    }

                //    var UcuncuSahisBedeniZararlar = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisBedeniZararlar);

                //    if (UcuncuSahisBedeniZararlar != null)
                //    {
                //        tutar = UcuncuSahisBedeniZararlar.OutPolicySumInsured.SumInsuredAmount;
                //        netPrim = UcuncuSahisBedeniZararlar.OutPolicySumInsured.AccruePremiumAmount;
                //        vergi = UcuncuSahisBedeniZararlar.OutPolicyTax.TaxAmount;
                //        brutPrim = UcuncuSahisBedeniZararlar.OutGrossPolicySumInsured.AccruePremiumAmount;
                //        this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, tutar, vergi, netPrim, brutPrim, 0);
                //    }

                //    var UcuncuSahisTedaviMasrafları = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisTedaviMasraflari);

                //    if (UcuncuSahisTedaviMasrafları != null)
                //    {
                //        tutar = UcuncuSahisTedaviMasrafları.OutPolicySumInsured.SumInsuredAmount;
                //        netPrim = UcuncuSahisTedaviMasrafları.OutPolicySumInsured.AccruePremiumAmount;
                //        vergi = UcuncuSahisTedaviMasrafları.OutPolicyTax.TaxAmount;
                //        brutPrim = UcuncuSahisTedaviMasrafları.OutGrossPolicySumInsured.AccruePremiumAmount;
                //        this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, tutar, vergi, netPrim, brutPrim, 0);
                //    }

                //    //var SGK = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.SGK);

                //    //if (SGK != null)
                //    //{
                //    //    tutar = SGK.OutPolicySumInsured.SumInsuredAmount;
                //    //    netPrim = SGK.OutPolicySumInsured.AccruePremiumAmount;
                //    //    vergi = SGK.OutPolicyTax.TaxAmount;
                //    //    brutPrim = SGK.OutGrossPolicySumInsured.AccruePremiumAmount;
                //    //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, tutar, vergi, netPrim, brutPrim, 0);
                //    //}

                //}

                //#endregion

                //#region Ödeme Planı
                //if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                //{
                //    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                //}
                //#endregion

                //#region Web servis cevapları

                //this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, response.OutPolicyMaster.ChangeSequenceNum);
                //if (MusteriAyniMi)
                //{
                //    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, EurekoMusteriNo);
                //    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, EurekoMusteriNo);
                //}
                //else
                //{
                //    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, EurekoMusteriNo);
                //    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, EurekoSEMusteriNo);
                //}


                //#endregion

                #endregion

                // var url = GetPDFAndBilgilendirmePDF(true, response.OutPolicyMaster.ChangeSequenceNum, response.OutPolicyMaster.PolicyNum, response.OutPolicyMaster.PolicyGroupNum, response.OutPolicyMaster.RenewalNum, response.OutPolicyMaster.EndorsementNum, response.OutPolicyMaster.InternalEndorsementNum);
                // this.GenelBilgiler.PDFDosyasi = url.pdfURL;
                _Log.Info("Teklif_PDF url: {0}", this.GenelBilgiler.PDFDosyasi);


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

        //public override void Policelestir(Odeme odeme)
        //{
        //    try
        //    {
        //        #region Veri Hazırlama GENEL
        //        ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
        //        MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
        //        var sigortaEttiren = teklif.SigortaEttiren;
        //        #endregion

        //        #region Policelestirme

        //        ProposalAndPolicySrV4Service trafikClient = new ProposalAndPolicySrV4Service();
        //        KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEurekoTrafik);
        //        KonfigTable konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);
        //        trafikClient.Url = konfig[Konfig.EUREKO_TrafikServiceURL];
        //        trafikClient.Timeout = 150000;

        //        eurekosigorta.trafikV4.ExecuteRequest requestP = new eurekosigorta.trafikV4.ExecuteRequest();

        //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.EUREKO });
        //        DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

        //        string SEMusteriNo = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, "0");
        //        string MusteriNo = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, "0");
        //        var TUMTeklif = _TeklifService.GetTeklifListe(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu).FirstOrDefault(s => s.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.EUREKO);

        //        #region Veri Hazırlama



        //        #endregion

        //        #endregion

        //        this.BeginLog(requestP, requestP.GetType(), WebServisIstekTipleri.Police);

        //        eurekosigorta.trafikV4.ExecuteResponseResponse responseP = trafikClient.Execute(requestP);

        //        if (responseP.OutHeader.IsSuccessfull == "false")
        //        {
        //            this.EndLog(responseP, false, responseP.GetType());
        //            this.AddHata(responseP.OutHeader.ResponseMessage);
        //        }
        //        else
        //        {
        //            this.EndLog(responseP, true, responseP.GetType());

        //            this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, responseP.OutPolicyMaster.ChangeSequenceNum);
        //            this.GenelBilgiler.TUMPoliceNo = responseP.OutPolicyMaster.PolicyNum;
        //            this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

        //           // var url = GetPDFAndBilgilendirmePDF(false, responseP.OutPolicyMaster.ChangeSequenceNum, responseP.OutPolicyMaster.PolicyNum,
        //                  //    responseP.OutPolicyMaster.PolicyGroupNum, responseP.OutPolicyMaster.RenewalNum, responseP.OutPolicyMaster.EndorsementNum,
        //                  //    responseP.OutPolicyMaster.InternalEndorsementNum);
        //           // this.GenelBilgiler.PDFDosyasi = url.pdfURL;
        //           // this.GenelBilgiler.PDFBilgilendirme = url.bilgilendirmeURL;
        //            _Log.Info("Police_PDF url: {0}", this.GenelBilgiler.PDFDosyasi);
        //            _Log.Info("Police_Bilgilendirme_PDF url: {0}", this.GenelBilgiler.PDFBilgilendirme);
        //            var cevap = this.GenelBilgiler.TeklifWebServisCevaps.Where(c => c.CevapKodu == Common.WebServisCevaplar.EUREKO_PolicyGroupNum).FirstOrDefault();
        //            if (cevap == null)
        //            {
        //                cevap = new TeklifWebServisCevap();
        //                cevap.CevapKodu = Common.WebServisCevaplar.EUREKO_PolicyGroupNum;
        //                cevap.Cevap = responseP.OutPolicyMaster.PolicyGroupNum;
        //                cevap.CevapTipi = SoruCevapTipleri.Metin;
        //                this.GenelBilgiler.TeklifWebServisCevaps.Add(cevap);
        //            }
        //            else
        //            {
        //                cevap.Cevap = responseP.OutPolicyMaster.PolicyGroupNum;
        //            }



        //        }

        //        this.GenelBilgiler.WEBServisLogs = this.Log;
        //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Hata Log
        //        this.EndLog(ex.Message, false);
        //        this.AddHata(ex.Message);
        //        #endregion
        //    }
        //}

        public override void DekontPDF()
        {
            try
            {

            }
            catch (Exception ex)
            {
                #region Hata Log
                _Log.Error(ex);
                this.EndLog(ex.Message, false);
                throw;
                #endregion
            }
        }

        public serviceCommandResult SorguYap(objectData data, GenericProcessExecutorService client)
        {
            serviceCommand sCommand = new serviceCommand();
            conversationKey key = new conversationKey();
            command reqCommand = new command()
            {
                commandName = "MAKEEXTERNALQUERY",
                commandText = "Sorgu Yap"
            };
            sCommand.command = reqCommand;
            sCommand.clientIP=ClientIPNo;
            key.clientKey = "1234"; //istemci key i her işlemde farklı olması gerekiyor
            sCommand.key = key;

            sCommand.objectData = data;
            sCommand.processCode = "AZS_WS_TRAFIK_PROPOSAL";
            sCommand.processStepName = "TRAFIKMAKEEXTERNALQUERY";
            this.BeginLog(sCommand, sCommand.GetType(), WebServisIstekTipleri.Teklif);
            serviceCommandResult response = client.execute(sCommand);

            if (response.serviceMessages.ToList().Count > 1)
            {
                this.EndLog(response, false, response.GetType());
            }
            client.Dispose();

            return response;
        }

        public string getAllianzIlceKodu(GenericProcessExecutorService client, string ilKodu, string ilceAdi)
        {

            field f = new field();
            f.fieldName = "obj.address.town";
            f.value = "-1";

            List<field> fff = new List<field>();
            field f2 = new field();
            f2.fieldName = "obj.address.city";
            f2.value = ilKodu;
            fff.Add(f2);
            var res = client.getValueSet("AZS_WS_TRAFIK_PROPOSAL", "TRAFIKMAKEEXTERNALQUERY", f, fff.ToArray());

            string AllianIlceKodu = String.Empty;
            AllianIlceKodu = res.values.ToList().FirstOrDefault(s => s.value1 == ilceAdi).key.ToString();

            return AllianIlceKodu;
        }

    }    
}



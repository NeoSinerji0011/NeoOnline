using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;
using static Neosinerji.BABOnlineTP.Web.Tools.Helpers.DummyPolicyApp;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public static class TeklifListeleri
    {
        public static List<SelectListItem> TeklifHazirlayanTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Proposal_UserSelf },
                new SelectListItem() { Value = "2", Text = babonline.Proposal_UserOther }
            });

            return list;
        }

        public static List<SelectListItem> TeklifDurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text =babonline.AllOf },
                new SelectListItem() { Value = "2", Text = babonline.Active },
                new SelectListItem() { Value = "3", Text = babonline.Pasive }
            });

            return list;
        }

        public static List<SelectListItem> KaskoTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Value="1",Text=babonline.NarrowInsurance},
                new SelectListItem(){Value="2",Text=babonline.Insurance},
                new SelectListItem(){Value="3",Text=babonline.ExtendedInsurance},
                new SelectListItem(){Value="4",Text=babonline.FullInsurance}
            });

            return list;
        }

        public static List<SelectListItem> KaskoServisTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Value="1",Text=babonline.ContractedSpecialService},
                new SelectListItem(){Value="2",Text=babonline.CustomService},
                new SelectListItem(){Value="3",Text=babonline.NegotiatedAuthorizedService},
                new SelectListItem(){Value="4",Text=babonline.AuthorizedService},
                new SelectListItem(){Value="5",Text=babonline.AllServices}
            });

            return list;
        }

        public static List<SelectListItem> KaskoYedekParcaTuru()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Value="1",Text=babonline.Original},
                new SelectListItem(){Value="2",Text=babonline.EquivalentPart}
            });

            return list;
        }

        public static List<SelectListItem> KaskoYurtDisiTeminatSureleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Daily_15 ,Value="1"},
                new SelectListItem(){Text=babonline.Monthly_1 , Value="2"},
                new SelectListItem(){Text=babonline.Monthly_1_3 ,Value="3"},
                new SelectListItem(){Text=babonline.Monthly_3_6 ,Value="4"},
                new SelectListItem(){Text=babonline.Monthly_6_12,Value="5"},
            });

            return list;
        }

        public static List<SelectListItem> MapfreKaskoYurtDisiTeminatSureleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="15 Gün",Value="15"},
                new SelectListItem(){Text="1 Ay", Value="30"},
                new SelectListItem(){Text="2 Ay",Value="60"},
                new SelectListItem(){Text="3 Ay",Value="90"},
                new SelectListItem(){Text="4 Ay",Value="120"},
                new SelectListItem(){Text="5 Ay",Value="150"},
                new SelectListItem(){Text="6 Ay",Value="180"},
                new SelectListItem(){Text="1 Yıl",Value="365"},
            });

            return list;
        }

        public static List<SelectListItem> DepremMuafiyetKodlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "%0" },
                new SelectListItem() { Value = "1", Text = "%5" }
            });

            return list;
        }

        public static List<SelectListItem> OnarimYerleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "T", Text = "Tüm Servisler" },
                new SelectListItem() { Value = "G", Text = "KASKO JET Servisleri" }
            });

            return list;
        }

        public static List<SelectListItem> MapfreAksesuarTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "LPG" },
                new SelectListItem() { Value = "2", Text = "KASA" },
                new SelectListItem() { Value = "3", Text = "JANT" },
                new SelectListItem() { Value = "4", Text = "DİĞER" }
            });

            return list;
        }

        public static List<SelectListItem> MapfreElektronikCihazTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "R/T" },
                new SelectListItem() { Value = "2", Text = "DVD" },
                new SelectListItem() { Value = "3", Text = "LCD" },
                new SelectListItem() { Value = "4", Text = "NAVİGATÖR" },
                new SelectListItem() { Value = "5", Text = "UYDU" },
                new SelectListItem() { Value = "6", Text = "DİĞER" }
            });

            return list;
        }

        public static List<SelectListItem> MapfreTasinanYukTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "AHŞAP ÜRÜNLERİ" },
                new SelectListItem() { Value = "2", Text = "ANTİKA DEĞERLİ EŞYA" },
                new SelectListItem() { Value = "3", Text = "CAM MERMER SERAMİK" },
                new SelectListItem() { Value = "4", Text = "CANLI HAYVAN" },
                new SelectListItem() { Value = "5", Text = "ELEKTRİK ELEKTRONİK" },
                new SelectListItem() { Value = "7", Text = "GIDA" },
                new SelectListItem() { Value = "8", Text = "KİMYEVİ MADDE" },
                new SelectListItem() { Value = "9", Text = "KAĞIT VE TÜREVLERİ" },
                new SelectListItem() { Value = "10", Text = "MAKİNA, MEKANİK EKİPMAN" },
                new SelectListItem() { Value = "11", Text = "METAL GRUBU" },
                new SelectListItem() { Value = "12", Text = "PETROL VE PETROL ÜRÜNLERİ" },
                new SelectListItem() { Value = "13", Text = "SİGARA İÇKİ" },
                new SelectListItem() { Value = "14", Text = "TEKSTİL AKSESUAR VE HAM" },
                new SelectListItem() { Value = "15", Text = "TEMİZLİK ÜRÜNLERİ" },
                new SelectListItem() { Value = "16", Text = "DİĞER" }
            });

            return list;
        }

        public static List<SelectListItem> MapfreIndirimSurprimTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Yok" },
                new SelectListItem() { Value = "1", Text = "İndirim" },
                new SelectListItem() { Value = "2", Text = "Surprim" }
            });

            return list;
        }
    }

    //Teklif Arama Ekranında Kullanilan Model
    public class TeklifAramaModel
    {
        public string TeklifNo { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int? TUMKodu { get; set; }
        public int UrunKodu { get; set; }
        public int TeklifDurumu { get; set; }
        public int HazirlayanKodu { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int MusteriKodu { get; set; }
        public string MusteriAdSoyad { get; set; }
        public string AktifProjeKodu { get; set; }

        public List<SelectListItem> Kullanicilar { get; set; }
        public List<SelectListItem> TUMler { get; set; }
        public List<SelectListItem> Urunler { get; set; }
        public SelectList Durumlar { get; set; }
    }

    public class TVMModel
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
    }

    public class HazirlayanModel
    {
        public int KendiAdima { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKullaniciKodu { get; set; }

        public string TVMUnvani { get; set; }
        public string TVMKullaniciAdi { get; set; }

        public SelectList KendiAdimaList { get; set; }

        public bool YeniIsMi { get; set; }
    }
    public class NipponSeyahatHazirlayanModel : HazirlayanModel
    {

    }
    public class DetayHazirlayanModel
    {
        public string TVMUnvani { get; set; }
        public string TVMKullaniciAdi { get; set; }
    }

    public class MusteriKaydetModel
    {
        //public int? TVMKodu { get; set; }
        public SigortaliModel Musteri { get; set; }
    }
    public class DaskMusteriKaydetModel
    {
        //public int? TVMKodu { get; set; }
        public SigortaliModel Musteri { get; set; }
    }
    public class MusteriModel
    {
        public MusteriModel()
        {
            this.SorgulamaSonuc = true;
            this.DisableControls = false;
            this.DisableManualGiris = false;
        }

        public int? TVMKodu { get; set; }

        public int? MusteriKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? MusteriTipKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }

        public string PasaportNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiDairesi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdiUnvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SoyadiUnvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IlceKodu { get; set; }

        public int AdresTipi { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AcikAdres { get; set; }
        public int PostaKodu { get; set; }

        public DateTime? DogumTarihi { get; set; }

        public string DogumTarihiText { get; set; }

        public string Cinsiyet { get; set; }

       // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [EPostaAdresi]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string CepTelefonu { get; set; }

        public string GulfKimlikNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short MusteriTelTipKodu { get; set; }

        public List<SelectListItem> MusteriTelTipleri;

        public short Uyruk { get; set; }

        public string AdSoyadUnvan
        {
            get
            {
                return String.Format("{0} {1}", this.AdiUnvan, this.SoyadiUnvan);
            }
        }

        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> Ilceler { get; set; }

        public bool SorgulamaSonuc { get; set; }
        public string HataMesaj { get; set; }

        //FOR AEGON
        public byte? GelirVergisiOrani { get; set; }

        /// <summary>
        /// Unvan gibi otomatik gelen alanları client tarafında disable etmek için.
        /// Mapfre sigorta için eklendi
        /// </summary>
        public bool DisableControls { get; set; }

        /// <summary>
        /// Sorgulama sonucu hatalı ise elle girişe müsade edilmeyecek.
        /// </summary>
        public bool DisableManualGiris { get; set; }
        public void SorgulamaHata(string mesaj)
        {
            this.SorgulamaSonuc = false;
            this.HataMesaj = mesaj;
        }
    }

    public class SigortaliModel
    {
        public SigortaliModel()
        {
            this.CepTelefonuRequired = true;
            this.EMailRequired = true;
            this.AcikAdresRequired = false;
        }

        public bool SadeceSigortaliGoster { get; set; }
        public bool SigortaliAyni { get; set; }
        public MusteriModel SigortaEttiren { get; set; }
        public MusteriModel Sigortali { get; set; }

        public List<SelectListItem> MusteriTipleri;
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> Ilceler { get; set; }
        public SelectList UyrukTipleri { get; set; }
        public SelectList CinsiyetTipleri { get; set; }

        public List<SelectListItem> MusteriTelTipleri;

        //FOR AEGON
        public SelectList GelirVergisiTipleri { get; set; }

        public bool CepTelefonuRequired { get; set; }
        public bool EMailRequired { get; set; }
        public bool AcikAdresRequired { get; set; }

        public int? TVMKodu { get; set; }
    }

    public class NipponSeyahatSigortaliModel : SigortaliModel
    {

    }
    public class DetayMusteriModel
    {
        public int MusteriKodu { get; set; }
        public string MusteriTipText { get; set; }
        public string KimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string VergiDairesi { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string EgitimDurumu { get; set; }
        public string MeslekAdi { get; set; }
        public string EhliyetYili { get; set; }
        public string Email { get; set; }
        public TelefonModel CepTelefonu { get; set; }
    }

    public class AracBilgiModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PlakaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PlakaNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimSekliKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimTarziKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MarkaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Model { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TipKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilIl { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilIlce { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TrafikTescilTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TrafigeCikisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime PoliceBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MotorNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SaseNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriKod { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriNo { get; set; }

        public int? KisiSayisi { get; set; }

        public DateTime? FesihTarihi { get; set; }

        public string AsbisNo { get; set; }
        public string HasarsizlikIndirim { get; set; }
        public string HasarSurprim { get; set; }
        public string UygulananKademe { get; set; }
        public string UygulananOncekiKademe { get; set; }

        public string MotorGucu { get; set; }
        public string SilindirHacmi { get; set; }
        public byte ImalatYeri { get; set; }
        public string Renk { get; set; }
        public string AnadoluMarkaKodu { get; set; }
        //public string TramerBelgeNo { get; set; }
        //public string TramerBelgeTarihi { get; set; }
        public string BelgeNumarasiTramer { get; set; }
        public string BelgeTarihTramer { get; set; }

        public SelectList PlakaKoduListe { get; set; }

        public List<SelectListItem> KullanimTarzlari { get; set; }
        public List<SelectListItem> KullanimSekilleri { get; set; }
        public List<SelectListItem> Markalar { get; set; }
        public List<SelectListItem> AracTipleri { get; set; }
        public List<SelectListItem> Modeller { get; set; }

        public List<SelectListItem> TescilIller { get; set; }
        public List<SelectListItem> TescilIlceler { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }

        //Anadolu Sigortaya özel alanlar
        public List<SelectListItem> AnadoluKullanimTipListe { get; set; }
        public string AnadoluKullanimTip { get; set; }

        public List<SelectListItem> AnadoluKullanimSekilleri { get; set; }
        public string AnadoluKullanimSekli { get; set; }
    }

    public class DetayAracBilgiModel
    {
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string KullanimSekli { get; set; }
        public string KullanimTarzi { get; set; }
        public string Marka { get; set; }
        public int Model { get; set; }
        public string Tip { get; set; }
        public string TescilIl { get; set; }
        public string TescilIlce { get; set; }
        public string TrafikTescilTarihi { get; set; }
        public string TrafigeCikisTarihi { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public string MotorNo { get; set; }
        public string SaseNo { get; set; }
        public string TescilBelgeSeriKod { get; set; }
        public string TescilBelgeSeriNo { get; set; }
        public string AsbisNo { get; set; }
        public decimal? Deger { get; set; }

        //public string MotorGucu { get; set; }
        //public string SilindirHacmi { get; set; }
        //public byte ImalatYeri { get; set; }
        //public string TramerBelgeNo { get; set; }
        //public DateTime? TramerBelgeTarihi { get; set; }
        //public string Renk { get; set; }
    }

    public class EskiPoliceModel
    {
        public bool EskiPoliceVar { get; set; }

        public string SigortaSirketKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AcenteNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YenilemeNo { get; set; }

        public string TramerIslemTipi { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public SelectList TramerIslemTipleri { get; set; }
    }

    public class DetayEskiPoliceModel
    {
        public bool EskiPoliceVar { get; set; }

        public string SigortaSirketiAdi { get; set; }
        public string AcenteNo { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string TramerIslemTipi { get; set; }
    }

    public class TasiyiciSorumlulukModel
    {
        public bool YetkiBelgesi { get; set; }
        public bool Sorumluluk { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AcenteNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PoliceNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YenilemeNo { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
    }

    public class DetayTasiyiciSorumlulukModel
    {
        public bool YetkiBelgesi { get; set; }
        public bool Sorumluluk { get; set; }

        public string SigortaSirketiAdi { get; set; }
        public string AcenteNo { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
    }

    public class TeklifUMListeModel : List<TeklifUMModel>
    {
        public void Add(int tumKodu, string tumUnvani, string logoUrl)
        {
            TeklifUMModel model = new TeklifUMModel();
            model.TUMKodu = tumKodu;
            model.TUMUnvani = tumUnvani;
            model.TUMLogo = logoUrl;
            model.TeklifAl = true;

            this.Add(model);
        }
    }

    public class TeklifUMModel
    {
        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogo { get; set; }
        public bool TeklifAl { get; set; }
    }

    public class TeklifOdemeListeModel : List<TeklifOdemeModel>
    {
        public void Add(int odemePlaniAlternatifKodu, string odemePlaniAdi)
        {
            this.Add(new TeklifOdemeModel() { OdemePlaniAlternatifKodu = odemePlaniAlternatifKodu, OdemePlaniAdi = odemePlaniAdi, TeklifAl = true });
        }
    }

    public class TeklifOdemeModel
    {
        public int OdemePlaniAlternatifKodu { get; set; }
        public string OdemePlaniAdi { get; set; }
        public bool TeklifAl { get; set; }
    }

    public class TeklifFiyatModel
    {
        public TeklifFiyatModel()
        {
            this.Otorizasyon = false;
        }

        public int TeklifId { get; set; }
        public int TeklifNo { get; set; }
        public int MusteriKodu { get; set; }
        public string AdSoyadUnvan { get; set; }
        public string PlakaNo { get; set; }
        public string MarkaAdi { get; set; }
        public string TipAdi { get; set; }
        public string KullanimTarziAdi { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public int TUMSayisi { get; set; }
        public string PDFDosyasi { get; set; }
        public bool Otorizasyon { get; set; }
        public List<TeklifFiyatDetayModel> Fiyatlar { get; set; }
        public List<TeklifFiyatHataModel> Hatalar { get; set; }
    }

    public class TeklifDurumModel
    {
        public int id { get; set; }
        public string teklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public bool tamamlandi { get; set; }
        public int teklifId { get; set; }
        public string pdf { get; set; }
        public string mesaj { get; set; }

        public List<TeklifFiyatDetayModel> teklifler { get; set; }
    }

    public class TeklifFiyatHataModel
    {
        public string TUMUnvani { get; set; }
        public string HataMesaji { get; set; }
    }

    public class TeklifFiyatDetayModel
    {
        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoUrl { get; set; }
        public string TUMTeklifNo { get; set; }
        public string TUMTeklifPDF { get; set; }
        public string TUMTeklifUyariMesaji { get; set; }
        public string TUMTeklifBilgiMesaji { get; set; }
        public string Hasarsizlik { get; set; }
        public string HasarIndirimSurprim { get; set; }
        public List<TeklifSurprimModel> Surprimler { get; set; }

        public string Fiyat1 { get; set; }
        public int Fiyat1_TeklifId { get; set; }

        public string Fiyat2 { get; set; }
        public int Fiyat2_TeklifId { get; set; }

        public string Fiyat3 { get; set; }
        public int Fiyat3_TeklifId { get; set; }

        public string KomisyonTutari { get; set; }
        public string KomisyonOrani { get; set; }

        public bool merkezAcenteMi { get; set; }

        public List<string> Hatalar { get; set; }

        public string TUMPDF { get; set; }
        public string DaskUyariMesaji { get; set; }
        public string LilyumParaticaURL { get; set; }
    }

    public class TeklifSurprimModel
    {
        public string SurprimAciklama { get; set; }
        public string Surprim { get; set; }
        public string SurprimIS { get; set; }
    }

    public class TeklifTeminatModel
    {
        public string TeminatAciklama { get; set; }
        public string Teminat { get; set; }
    }

    public class TeklifOdemeCevapModel
    {
        public bool Success { get; set; }
        public string RedirectUrl { get; set; }
        public string[] Hatalar { get; set; }
        public bool SompoJapanTaksitliMi { get; set; }

    }

    public class TeklifMailGonderModel
    {
        public int TeklifId { get; set; }
        public int SigortaEttirenId { get; set; }
        public string SigortaEttirenAdSoyad { get; set; }
        public string SigortaEttirenMail { get; set; }
        public bool SigortaEttirenMailGonder { get; set; }

        [EPostaAdresi]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DigerEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DigerAdSoyad { get; set; }

        public bool DigerMailGonder { get; set; }
        public string TeklifPDF { get; set; }
    }


}

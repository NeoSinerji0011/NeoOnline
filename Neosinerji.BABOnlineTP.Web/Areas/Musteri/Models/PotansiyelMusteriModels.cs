using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Business.Service;
using common = Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business;
using AutoMapper;

namespace Neosinerji.BABOnlineTP.Web.Areas.PotansiyelMusteri.Models
{
    //public static class MusteriListProvider
    //{
    //    public static List<SelectListItem> CinsiyetTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "E", Text = babonline.Man},
    //            new SelectListItem() { Value = "K", Text = babonline.Women }
    //        });

    //        return list;
    //    }
    //    public static List<SelectListItem> GetTeklifTarihiTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value="1",Text=babonline.Disposal},
    //            new SelectListItem() { Value="2",Text=babonline.Starting},
    //            new SelectListItem() { Value="3",Text=babonline.Finish}                
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> AdresTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem(){Value="",Text=babonline.PleaseSelect},
    //            new SelectListItem(){Value="8",Text=babonline.Home},
    //            new SelectListItem(){Value="9",Text=babonline.Work},
    //            new SelectListItem(){Value="10",Text=babonline.Other}
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> UyrukTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "0", Text = "TC"},
    //            new SelectListItem() { Value = "1", Text = babonline.Foreign }
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> MusteriTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
    //            new SelectListItem() { Value = "1", Text = babonline.Real_TC_Nationals_Customer },
    //            new SelectListItem() { Value = "2", Text = babonline.Corporate_Customers },
    //            new SelectListItem() { Value = "3", Text = babonline.Sole_Proprietorship_Customer },
    //            new SelectListItem() { Value = "4", Text = babonline.Foreig_Real_Customer},
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> MusteriTipleriRadio()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "1", Text = babonline.Real_TC_Nationals_Customer },
    //            new SelectListItem() { Value = "2", Text = babonline.Corporate_Customers },
    //            new SelectListItem() { Value = "3", Text = babonline.Sole_Proprietorship_Customer },
    //            new SelectListItem() { Value = "4", Text = babonline.Foreig_Real_Customer},
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> IletisimNumaraTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
    //            new SelectListItem() { Value = "11", Text = babonline.Mobile_Phone},
    //            new SelectListItem() { Value = "12", Text = babonline.Work_Phone },
    //            new SelectListItem() { Value = "13", Text = babonline.Home_Phone},
    //            new SelectListItem() { Value = "14", Text = babonline.Fax_Number},
    //            new SelectListItem() { Value = "15", Text = babonline.Other},
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> MedeniDurumTipleri()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
    //            new SelectListItem() { Value = "1", Text = babonline.Married },
    //            new SelectListItem() { Value = "2", Text = babonline.Single},
    //            new SelectListItem() { Value = "3", Text = babonline.Widow},
    //            new SelectListItem() { Value = "4", Text = babonline.Divorced},
    //        });

    //        return list;
    //    }

    //    public static List<SelectListItem> EgitimDurumlari()
    //    {
    //        List<SelectListItem> list = new List<SelectListItem>();

    //        list.AddRange(new SelectListItem[] {
    //            new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
    //            new SelectListItem() { Value = "1", Text = babonline.PrimaryEducation },
    //            new SelectListItem() { Value = "2", Text = babonline.HighSchool_J_HighSchool},
    //            new SelectListItem() { Value = "3", Text = babonline.Associate},
    //            new SelectListItem() { Value = "4", Text = babonline.License},
    //            new SelectListItem() { Value = "5", Text = babonline.MastersDegree},
    //            new SelectListItem() { Value = "6", Text = babonline.Doctorate},
    //            new SelectListItem() { Value = "7", Text = babonline.Student},
    //        });

    //        return list;
    //    }

    //    public static string GetMusteriTipiText(short tipi)
    //    {
    //        switch (tipi)
    //        {
    //            case common.MusteriTipleri.TCMusteri: return babonline.Real_TC_Nationals_Customer;
    //            case common.MusteriTipleri.TuzelMusteri: return babonline.Corporate_Customers;
    //            case common.MusteriTipleri.SahisFirmasi: return babonline.Sole_Proprietorship_Customer;
    //            case common.MusteriTipleri.YabanciMusteri: return babonline.Foreig_Real_Customer;
    //        }

    //        return String.Empty;
    //    }

    //    public static string GetMusteriTipiText1(short tipi)
    //    {
    //        switch (tipi)
    //        {
    //            case common.MusteriTipleri.TCMusteri: return "Ş";
    //            case common.MusteriTipleri.TuzelMusteri: return "T";
    //            case common.MusteriTipleri.SahisFirmasi: return "ŞF";
    //            case common.MusteriTipleri.YabanciMusteri: return "Y";
    //        }

    //        return String.Empty;
    //    }

    //    public static string GetNumaraTipiText(short tipi)
    //    {
    //        switch (tipi)
    //        {
    //            case common.IletisimNumaraTipleri.Cep: return babonline.Mobile_Phone;
    //            case common.IletisimNumaraTipleri.Is: return babonline.Work_Phone;
    //            case common.IletisimNumaraTipleri.Ev: return babonline.Home_Phone;
    //            case common.IletisimNumaraTipleri.Fax: return babonline.Fax_Number;
    //            case common.IletisimNumaraTipleri.Diger: return babonline.Other;
    //        }
    //        return String.Empty;
    //    }

    //    public static string GetMedeniDurumText(short tipi)
    //    {
    //        switch (tipi)
    //        {
    //            case common.MedeniDurumTipleri.Evli: return babonline.Married;
    //            case common.MedeniDurumTipleri.Bekar: return babonline.Single;
    //            case common.MedeniDurumTipleri.Dul: return babonline.Widow;
    //            case common.MedeniDurumTipleri.Bosanmis: return babonline.Divorced;
    //        }
    //        return String.Empty;
    //    }

    //    public static string GetAdresTipiText(short tipi)
    //    {
    //        switch (tipi)
    //        {
    //            case common.AdresTipleri.Ev: return babonline.Home;
    //            case common.AdresTipleri.Is: return babonline.Work;
    //            case common.AdresTipleri.Diger: return babonline.Other;
    //        }
    //        return String.Empty;
    //    }
    //}

    public class PotansiyelMusteriModel
    {
        public PotansiyelGenelBilgilerModel PotansiyelGenelBilgiler { get; set; }
        public PotansiyelAdresModel PotansiyelMusteriAdresModel { get; set; }
        public PotansiyelMusteriTelefonModel PotansiyelMusteriTelefonModel { get; set; }
        public PotansiyelNotModel PotansiyelMusteriNotModel { get; set; }
        public PotansiyelGuncelleModel PotansiyelMusteriGuncelleModel { get; set; }

        public SelectList MusteriTipleri { get; set; }
        public SelectList CinsiyetTipleri { get; set; }
        public SelectList UyrukTipleri { get; set; }
        public SelectList MedeniDurumTipleri { get; set; }
        public List<SelectListItem> Meslekler { get; set; }
        public List<SelectListItem> EgitimDurumlari { get; set; }
        public List<SelectListItem> AdresTipleri { get; set; }
        public List<SelectListItem> IletisimNumaraTipleri { get; set; }

        //Tüzel kişi için zorunlu alanlar.
        public List<SelectListItem> FaaliyetOlcegiList { get; set; }
        public List<SelectListItem> FaaliyerAnaSektorList { get; set; }
        public List<SelectListItem> FaaliyetAltSektorList { get; set; }
        public List<SelectListItem> SabitVarlikList { get; set; }
        public List<SelectListItem> CiroBilgisiList { get; set; }

        public PotansiyelMusteriTelefonListModel Telefonlari { get; set; }
        public PotansiyelDokumanListModel Dokumanlari { get; set; }
        public PotansiyelNotListModel Notlari { get; set; }
        public PotansiyelAdresListModel Adresleri { get; set; }
    }

    public class PotansiyelMusteriListeModel
    {
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string EMail { get; set; }
        public short MusteriTipKodu { get; set; }

        public int? TVMKodu { get; set; }
        public string KimlikNo { get; set; }
        public List<SelectListItem> TVMList { get; set; }
        public int? MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string PasaportNo { get; set; }

        public string Tip { get; set; }

        public string TeklifTipi { get; set; }
        public bool TeklifAl { get; set; }
        public string TeklifUrl { get; set; }

        public SelectList MusteriTipleri { get; set; }

    }

    public class PotansiyelGenelBilgilerListModel
    {
        public List<PotansiyelGenelBilgilerModel> Items { get; set; }
    }

    public class PotansiyelGenelBilgilerModel
    {
        //Zorunlu alanlar
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int? TVMKullaniciKodu { get; set; }
        public short MusteriTipKodu { get; set; }
        public int PotansiyelMusteriKodu { get; set; }

        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Identification_Number_Length")]
        public string KimlikNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxOffice_Length")]
        public string VergiDairesi { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Name_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdiUnvan { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Surname_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SoyadiUnvan { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Code_Length")]
        public string TVMMusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Web_URL_Length")]
        public string WebUrl { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Passport_Number_Length")]
        public string PasaportNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "Passport_Expiry_Date")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public Nullable<DateTime> PasaportGecerlilikBitisTarihi { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "DateOfBirth")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public Nullable<DateTime> DogumTarihi { get; set; }


        public string Cinsiyet { get; set; }
        public short? EgitimDurumu { get; set; }
        public short? Uyruk { get; set; }
        public int? MeslekKodu { get; set; }
        public byte? MedeniDurumu { get; set; }


        //Text Olarak Yazılabilmeleri için....       
        public string MusteriTipiText { get; set; }
        public string MedeniDurumText { get; set; }
        public string MeslekKoduText { get; set; }
        public string EgitimDurumuText { get; set; }


        //Text Alanları
        [IgnoreMap]
        public string FaaliyetOlcegiText { get; set; }
        [IgnoreMap]
        public string FaaliyetAnaSektorText { get; set; }
        [IgnoreMap]
        public string FaaliyetAltSektorText { get; set; }
        [IgnoreMap]
        public string SabitVarlikBilgisiText { get; set; }
        [IgnoreMap]
        public string CiroBilgisiText { get; set; }
    }

    public class PotansiyelDetayModel
    {
        public int PotansiyelMusteriKodu { get; set; }
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string TVMMusteriKodu { get; set; }

        public string KimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public Nullable<System.DateTime> PasaportGecerlilikBitisTarihi { get; set; }
        public string VergiDairesi { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string Cinsiyet { get; set; }
        public Nullable<System.DateTime> DogumTarihi { get; set; }
        public string EMail { get; set; }
        public string WebUrl { get; set; }
        public short Uyruk { get; set; }

        public string MusteriKoduText { get; set; }
        public string MusteriTipiText { get; set; }
        public string MedeniDurumText { get; set; }
        public string MeslekKoduText { get; set; }
        public string EgitimDurumuText { get; set; }
    }

    public class PotansiyelGuncelleModel
    {
        //Zorunlu alanlar
        public int? TVMKodu { get; set; }
        public int? TVMKullaniciKodu { get; set; }
        public short MusteriTipKodu { get; set; }
        public int PotansiyelMusteriKodu { get; set; }

        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Identification_Number_Length")]
        public string KimlikNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxOffice_Length")]
        public string VergiDairesi { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Name_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdiUnvan { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Surname_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SoyadiUnvan { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Code_Length")]
        public string TVMMusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Web_URL_Length")]
        public string WebUrl { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Passport_Number_Length")]
        public string PasaportNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "Passport_Expiry_Date")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public Nullable<DateTime> PasaportGecerlilikBitisTarihi { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "DateOfBirth")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public Nullable<DateTime> DogumTarihi { get; set; }

        public string Cinsiyet { get; set; }
        public short? EgitimDurumu { get; set; }
        public int? MeslekKodu { get; set; }
        public byte? MedeniDurumu { get; set; }
        public short? Uyruk { get; set; }

        //Text Olarak Yazılabilmeleri için....
        public string TVMUnvani { get; set; }
        public string MusteriTipiText { get; set; }
        public string MedeniDurumText { get; set; }
        public string MeslekKoduText { get; set; }
        public string EgitimDurumuText { get; set; }

        //Text Alanları
        [IgnoreMap]
        public string FaaliyetOlcegiText { get; set; }
        [IgnoreMap]
        public string FaaliyetAnaSektorText { get; set; }
        [IgnoreMap]
        public string FaaliyetAltSektorText { get; set; }
        [IgnoreMap]
        public string SabitVarlikBilgisiText { get; set; }
        [IgnoreMap]
        public string CiroBilgisiText { get; set; }
    }


    //Müşteri Döküman Modelleri
    public class PotansiyelDokumanListModel
    {
        public List<PotansiyelDokumanDetayModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class PotansiyelDokumanModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int PotansiyelMusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Document_Type_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanTuru { get; set; }

        public string sayfaadi { get; set; }
    }

    //Müşteri Not Modelleri
    public class PotansiyelNotListModel
    {
        public List<PotansiyelNotModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class PotansiyelNotModel
    {
        public int PotansiyelMusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Topic_Length")]
        public string Konu { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Node_Length")]
        public string NotAciklamasi { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        public System.DateTime KayitTarihi { get; set; }

        public int TVMKodu { get; set; }
        public int TVMPersonelKodu { get; set; }
        public int SiraNo { get; set; }
        public string TvmPersonelAdi { get; set; }

        //Bu prop  guncelleme için kullanılıyor..
        public string sayfaadi { get; set; }
    }

    //Müşteri Telefon Modelleri
    public class PotansiyelMusteriTelefonListModel
    {
        public List<PotansiyelMusteriTelefonDetayModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class PotansiyelMusteriTelefonModel
    {
        public int PotansiyelMusteriKodu { get; set; }
        public string Numara { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short IletisimNumaraTipi { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Number_Owner_Length")]
        public string NumaraSahibi { get; set; }

        public SelectList IletisimNumaraTipleri { get; set; }
        public int SiraNo { get; set; }
        public string NumaraTipiText { get; set; }
        public string sayfaadi { get; set; }
    }
    public class PotansiyelMusteriTelefonDetayModel
    {
        public int PotansiyelMusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public short IletisimNumaraTipi { get; set; }
        public string IletisimNumaraText { get; set; }
        public string Numara { get; set; }
        public string NumaraSahibi { get; set; }
    }

    //Müşteri Adres Modelleri
    public class PotansiyelAdresListModel
    {
        public List<PotansiyelAdresModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class PotansiyelAdresModel
    {
        public int PotansiyelMusteriKodu { get; set; }
        public int? SiraNo { get; set; }
        public bool Varsayilan { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Neighborhood_Length")]
        public string Semt { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Han_Apt_Fab_Length")]
        public string HanAptFab { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_OtherLength")]
        public string Diger { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_AddressLength")]
        public string Adres { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ParishLength")]
        public string Mahalle { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_AvenueLength")]
        public string Cadde { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_StreetLength")]
        public string Sokak { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ApartmentLength")]
        public string Apartman { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        public string BinaNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        public string DaireNo { get; set; }

        public int? PostaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? AdresTipi { get; set; }

        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int? IlceKodu { get; set; }
        public string ParitusAdresDogrulama { get; set; }

        public List<SelectListItem> UlkeLer { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public List<SelectListItem> AdresTipleri { get; set; }


        public string AdresTipiText { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string sayfaadi { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}





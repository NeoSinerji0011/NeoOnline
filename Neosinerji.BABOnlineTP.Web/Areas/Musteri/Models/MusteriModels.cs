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

namespace Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models
{
    public static class MusteriListProvider
    {
        public static List<SelectListItem> CinsiyetTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "E", Text = babonline.Man},
                new SelectListItem() { Value = "K", Text = babonline.Women },
                new SelectListItem() { Value = "", Text = "Boş" }
            });

            return list;
        }

        public static List<SelectListItem> GetTeklifTarihiTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value="1",Text=babonline.Disposal},
                new SelectListItem() { Value="2",Text=babonline.Starting},
                new SelectListItem() { Value="3",Text=babonline.Finish}
            });

            return list;
        }

        public static List<SelectListItem> AdresTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem(){Value="",Text=babonline.PleaseSelect},
                new SelectListItem(){Value="8",Text=babonline.Home},
                new SelectListItem(){Value="9",Text=babonline.Work},
                new SelectListItem(){Value="10",Text=babonline.Other}
            });

            return list;
        }

        public static List<SelectListItem> UyrukTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "TC"},
                new SelectListItem() { Value = "1", Text = babonline.Foreign }
            });

            return list;
        }

        public static List<SelectListItem> MusteriTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.Real_TC_Nationals_Customer },
                new SelectListItem() { Value = "2", Text = babonline.Corporate_Customers },
                new SelectListItem() { Value = "3", Text = babonline.Sole_Proprietorship_Customer },
                new SelectListItem() { Value = "4", Text = babonline.Foreig_Real_Customer},
            });

            return list;
        }

       
        public static List<SelectListItem> MusteriTipleriRadio()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Real_TC_Nationals_Customer },
                new SelectListItem() { Value = "2", Text = babonline.Corporate_Customers },
                new SelectListItem() { Value = "3", Text = babonline.Sole_Proprietorship_Customer },
                new SelectListItem() { Value = "4", Text = babonline.Foreig_Real_Customer},
            });

            return list;
        }

        public static List<SelectListItem> IletisimNumaraTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "11", Text = babonline.Mobile_Phone},
                new SelectListItem() { Value = "12", Text = babonline.Work_Phone },
                new SelectListItem() { Value = "13", Text = babonline.Home_Phone},
                new SelectListItem() { Value = "14", Text = babonline.Fax_Number},
                new SelectListItem() { Value = "15", Text = babonline.Other},
            });

            return list;
        }

        public static List<SelectListItem> MedeniDurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.Married },
                new SelectListItem() { Value = "2", Text = babonline.Single},
                new SelectListItem() { Value = "3", Text = babonline.Widow},
                new SelectListItem() { Value = "4", Text = babonline.Divorced},
            });

            return list;
        }

        public static List<SelectListItem> EgitimDurumlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.PrimaryEducation },
                new SelectListItem() { Value = "2", Text = babonline.HighSchool_J_HighSchool},
                new SelectListItem() { Value = "3", Text = babonline.Associate},
                new SelectListItem() { Value = "4", Text = babonline.License},
                new SelectListItem() { Value = "5", Text = babonline.MastersDegree},
                new SelectListItem() { Value = "6", Text = babonline.Doctorate},
                new SelectListItem() { Value = "7", Text = babonline.Student},
            });

            return list;
        }
        public static string GetEgitimDurumText(short tipi)
        {
            switch (tipi)
            {
                case 1: return babonline.PrimaryEducation;
                case 2: return babonline.HighSchool_J_HighSchool;
                case 3: return babonline.Associate;
                case 4: return babonline.License;
                case 5: return babonline.MastersDegree;
                case 6: return babonline.Doctorate;
                case 7: return babonline.Student;
            }

            return String.Empty;
        }
        public static List<SelectListItem> MusteriSayilari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "1 - 25"},
                new SelectListItem() { Value = "2", Text = "50"},
                new SelectListItem() { Value = "3", Text = "75"},
                new SelectListItem() { Value = "4", Text = "100"},
                new SelectListItem() { Value = "5", Text = "100 üzeri"},
            });

            return list;
        }

        public static string GetMusteriTipiText(short tipi)
        {
            switch (tipi)
            {
                case common.MusteriTipleri.TCMusteri: return babonline.Real_TC_Nationals_Customer;
                case common.MusteriTipleri.TuzelMusteri: return babonline.Corporate_Customers;
                case common.MusteriTipleri.SahisFirmasi: return babonline.Sole_Proprietorship_Customer;
                case common.MusteriTipleri.YabanciMusteri: return babonline.Foreig_Real_Customer;
            }

            return String.Empty;
        }

        public static string GetMusteriTipiText1(short tipi)
        {
            switch (tipi)
            {
                case common.MusteriTipleri.TCMusteri: return "Ş";
                case common.MusteriTipleri.TuzelMusteri: return "T";
                case common.MusteriTipleri.SahisFirmasi: return "ŞF";
                case common.MusteriTipleri.YabanciMusteri: return "Y";
            }

            return String.Empty;
        }

        public static string GetNumaraTipiText(short tipi)
        {
            switch (tipi)
            {
                case common.IletisimNumaraTipleri.Cep: return babonline.Mobile_Phone;
                case common.IletisimNumaraTipleri.Is: return babonline.Work_Phone;
                case common.IletisimNumaraTipleri.Ev: return babonline.Home_Phone;
                case common.IletisimNumaraTipleri.Fax: return babonline.Fax_Number;
                case common.IletisimNumaraTipleri.Diger: return babonline.Other;
            }
            return String.Empty;
        }

        public static string GetMedeniDurumText(short tipi)
        {
            switch (tipi)
            {
                case common.MedeniDurumTipleri.Evli: return babonline.Married;
                case common.MedeniDurumTipleri.Bekar: return babonline.Single;
                case common.MedeniDurumTipleri.Dul: return babonline.Widow;
                case common.MedeniDurumTipleri.Bosanmis: return babonline.Divorced;
            }
            return String.Empty;
        }

        public static string GetAdresTipiText(short tipi)
        {
            switch (tipi)
            {
                case common.AdresTipleri.Ev: return babonline.Home;
                case common.AdresTipleri.Is: return babonline.Work;
                case common.AdresTipleri.Diger: return babonline.Other;
                case common.AdresTipleri.Teslimat: return "Teslimat";
                    //Bu tip Lilyuma özel çalışıyor. Bu yüzden Konut Adresi olarak görünecek
                case common.AdresTipleri.Iletisim: return "Konut Adresi";
            }
            return String.Empty;
        }

        public static List<SelectListItem> YasGruplari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "0-20", Text = "0-20"},
                new SelectListItem() { Value = "21-40", Text = "21-40" },
                new SelectListItem() { Value = "41-60", Text = "41-60" },
                new SelectListItem() { Value = "61", Text = "61+" },
            });

            return list;
        }
    }

    public class MusteriModel
    {
        public GenelBilgilerModel GenelBilgiler { get; set; }
        public AdresModel MusteriAdresModel { get; set; }
        public MusteriTelefonModel MusteriTelefonModel { get; set; }
        public NotModel MusteriNotModel { get; set; }
        public GuncelleModel MusteriGuncelleModel { get; set; }

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


        public MusteriTelefonListModel Telefonlari { get; set; }
        public DokumanListModel Dokumanlari { get; set; }
        public NotListModel Notlari { get; set; }
        public AdresListModel Adresleri { get; set; }

        public bool PotansiyelMi { get; set; }
        public int? PotansiyelMusteriKodu { get; set; }
    }

    public class MusteriListeModel
    {
        public GenelBilgilerModel GenelBilgiler { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string EMail { get; set; }
        public short MusteriTipKodu { get; set; }

        public int? TVMKodu { get; set; }
        public string KimlikNo { get; set; }

        public int? MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string PasaportNo { get; set; }

        public string Tip { get; set; }
        public string TeklifTipi { get; set; }
        public bool TeklifAl { get; set; }
        public string TeklifUrl { get; set; }

        public SelectList MusteriTipleri { get; set; }
        public List<SelectListItem> TVMList { get; set; }
        
        
    }
    public class MusteriListesiModel
    {
        public GenelBilgilerModel GenelBilgiler { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string EMail { get; set; }
        public short MusteriTipKodu { get; set; }

        public int? TVMKodu { get; set; }
        public string KimlikNo { get; set; }

        public int? MusteriKodu { get; set; }
        public string TVMMusteriKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string PasaportNo { get; set; }

        public DateTime? DogumTarihi { get; set; }

        public string YasGrubu { get; set; }

        public int? MeslekKodu { get; set; }

        public List<SelectListItem> Meslekler { get; set; }
        public SelectList CinsiyetTipleri { get; set; }

        public SelectList MusteriTipleri { get; set; }
        public List<SelectListItem> TVMList { get; set; }
        public SelectList YasGruplari { get; set; }

        public String yasBaslangic { get; set; }
        public String yasBitis { get; set; }

        public string dogBas { get; set; }
        public string dogBit { get; set; }

        public string MeslekKoduText { get; set; }
    }
    public class MusteriListeModelHarita
    {
        public int? TVMKodu { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public short MusteriTipKodu { get; set; }
        public string TVMUnvani { get; set; }
        public byte MusteriSayisi { get; set; }

        public SelectList MusteriTipleri { get; set; }
        public SelectList MusteriSayilar { get; set; }
        public List<SelectListItem> TVMList { get; set; }
    }

  
    public class MusteriAdediModel
    {
       
        public List<GenelBilgilerModel> Items { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public string TVMListe { get; set; }

        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
      
        public MultiSelectList tvmler { get; set; }
        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }

        public string MusteriTipiText { get; set; }

        public decimal? TCMusteriCount { get; set; }
        public decimal? TCMusteriCountToplam { get; set; }
        public decimal? TuzelMusteriCountToplam { get; set; }
        public decimal? SahisFirmasiCountToplam { get; set; }
        public decimal? YabanciMusteriCountToplam { get; set; }
        public decimal? MusteriToplamCountToplam { get; set; } 
        public decimal? TuzelMusteriCount { get; set; } 
        public int TCMusteri { get; set; }
        public int TuzelMusteri { get; set; }
        public decimal? SahisFirmasiCount { get; set; }
       
        public int SahisFirmasi { get; set; }
        public decimal? YabanciMusteriCount { get; set; }
        
        public int YabanciMusteri { get; set; }
        public decimal? MusteriToplamCount { get; set; }
        
        public string tvmUnvani { get; set; }
     

    }

    public class GenelBilgilerListModel
    {
        public List<GenelBilgilerModel> Items { get; set; }
    }

    public class GenelBilgilerModel
    {
        //Zorunlu alanlar
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public int? TVMKullaniciKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short MusteriTipKodu { get; set; }

        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Identification_Number_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }

        //Vergi dairesi alanı tüzel kişiler icin zorunlu ....
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxOffice_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiDairesi { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Name_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdiUnvan { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Surname_Title_Length")]
        public string SoyadiUnvan { get; set; }


        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short Uyruk { get; set; }

        //Null olabilir alanlar

        [StringLength(30, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Code_Length")]
        public string TVMMusteriKodu { get; set; }
        public string MuhasebeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Web_URL_Length")]
        public string WebUrl { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Passport_Number_Length")]
        public string PasaportNo { get; set; }

        //Pasaport tarihi alanı zorunlu değil (pasaport numarası girilmemişse....)
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "Passport_Expiry_Date")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public DateTime PasaportGecerlilikBitisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "DateOfBirth")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public DateTime DogumTarihi { get; set; }

        public int MusteriKodu { get; set; }
        public string Cinsiyet { get; set; }
        public short? EgitimDurumu { get; set; }
        public int? MeslekKodu { get; set; }
        public byte? MedeniDurumu { get; set; }
        //Text Olarak Yazılabilmeleri için....       
        public string MusteriTipiText { get; set; }
        public string MedeniDurumText { get; set; }
        public string MeslekKoduText { get; set; }
        public string EgitimDurumuText { get; set; }

        
       

        public static List<SelectListItem> CinsiyetTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "E", Text = babonline.Man},
                new SelectListItem() { Value = "K", Text = babonline.Women },
                new SelectListItem() { Value = "NULL", Text = "Boş" },

            });

            return list;
        }




        //Sonradan Eklenen alanlar (Tüzel)
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetOlcegi_ { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetGosterdigiAnaSektor { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetGosterdigiAltSektor { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string SabitVarlikBilgisi { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string CiroBilgisi { get; set; }

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

    public class DetayModel
    {
        public int MusteriKodu { get; set; }
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
    public class GuncelleModel
    {
        //Zorunlu alanlar

        public int? TVMKodu { get; set; }
        public int? TVMKullaniciKodu { get; set; }
        public short MusteriTipKodu { get; set; }

        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Identification_Number_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KimlikNo { get; set; }

        //Vergi dairesi alanı tüzel kişiler icin zorunlu ....
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxOffice_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiDairesi { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Name_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdiUnvan { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Surname_Title_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SoyadiUnvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short Uyruk { get; set; }

        //Null olabilir alanlar

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Code_Length")]
        public string TVMMusteriKodu { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Customer_Code_Length")]
        public string MuhasebeKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Web_URL_Length")]
        public string WebUrl { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Passport_Number_Length")]
        public string PasaportNo { get; set; }

        //Pasaport tarihi alanı zorunlu değil (pasaport numarası girilmemişse....)
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [Display(ResourceType = typeof(babonline), Name = "Passport_Expiry_Date")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public DateTime PasaportGecerlilikBitisTarihi { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        //[Display(ResourceType = typeof(babonline), Name = "DateOfBirth")]
        //[DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public DateTime? DogumTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Cinsiyet { get; set; }

        public int MusteriKodu { get; set; }
        public short? EgitimDurumu { get; set; }
        public int? MeslekKodu { get; set; }
        public byte? MedeniDurumu { get; set; }

        //Text Olarak Yazılabilmeleri için....
        public string TVMUnvani { get; set; }
        public string MusteriTipiText { get; set; }
        public string MedeniDurumText { get; set; }
        public string MeslekKoduText { get; set; }
        public string EgitimDurumuText { get; set; }

        //Sonradan Eklenen alanlar (Tüzel)
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetOlcegi_ { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetGosterdigiAnaSektor { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string FaaliyetGosterdigiAltSektor { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string SabitVarlikBilgisi { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string CiroBilgisi { get; set; }


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
    public class DokumanListModel
    {
        public List<DokumanDetayModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class DokumanModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int MusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Document_Type_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanTuru { get; set; }

        public string sayfaadi { get; set; }
    }

    //Müşteri Not Modelleri
    public class NotListModel
    {
        public List<NotModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class NotModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int MusteriKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Topic_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Konu { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Node_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
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
    public class MusteriTelefonListModel
    {
        public List<MusteriTelefonDetayModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class MusteriTelefonModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int MusteriKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short IletisimNumaraTipi { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //[TelefonRequiredValidation(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PhoneNumber")]
        //public TelefonModel Numara { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Numara { get; set; }


        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Number_Owner_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string NumaraSahibi { get; set; }

        public SelectList IletisimNumaraTipleri { get; set; }
        public int SiraNo { get; set; }
        public string NumaraTipiText { get; set; }
        public string sayfaadi { get; set; }
    }
    public class MusteriTelefonDetayModel
    {
        public int MusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public short IletisimNumaraTipi { get; set; }
        public string IletisimNumaraText { get; set; }
        public string Numara { get; set; }
        public string NumaraSahibi { get; set; }
        public Boolean guncellenebilecekMi { get; set; }
    }

    //Müşteri Adres Modelleri
    public class AdresListModel
    {

        public List<AdresModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class AdresModel
    {
        public Boolean guncellenebilecekMi { get; set; }
        //Null olabilen alanlar
        public int MusteriKodu { get; set; }
        public int? SiraNo { get; set; }
        public bool Varsayilan { get; set; }

        public string ParitusAdresDogrulama { get; set; }


        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Neighborhood_Length")]
        public string Semt { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Han_Apt_Fab_Length")]
        public string HanAptFab { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_OtherLength")]
        public string Diger { get; set; }

        //Zorunlu alanlar....

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? AdresTipi { get; set; }

        //Database de ulke kodu , ilkodu, ilce kodu alanlari null olabiliyor ... Düzenlenecek...
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IlceKodu { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_AddressLength")]
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adres { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ParishLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Mahalle { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_AvenueLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Cadde { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_StreetLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Sokak { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ApartmentLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Apartman { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BinaNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaireNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? PostaKodu { get; set; }

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





using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using AutoMapper;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TUMListeModel
    {
        public int? Kodu { get; set; }
        public string Unvani { get; set; }
        public string BirlikKodu { get; set; }

        public byte Durum { get; set; }
    }

    public class TUMEkleModel
    {

        public int? Kodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Unvani { get; set; }

        public string BirlikKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiDairesi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength", MinimumLength = 10)]
        public string VergiNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Durum { get; set; }

        public DateTime? DurumGuncellemeTarihi { get; set; }

        public DateTime? TUMBaslangicTarihi { get; set; }

        public DateTime? TUMBitisTarihi { get; set; }

        public string Telefon { get; set; }

        public string Fax { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string Email { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Web_URL_Length")]
        public string WebAdresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }
        public List<SelectListItem> Iller { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? IlceKodu { get; set; }
        public List<SelectListItem> IlceLer { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Semt { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adres { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte BaglantiSiniri { get; set; }
        public SelectList BaglantiSiniriVarYok { get; set; }

        public Nullable<short> UcretlendirmeKodu { get; set; }

        public string Banner { get; set; }

        public string Logo { get; set; }
    }

    public class TUMDetayModel
    {
        public int? Kodu { get; set; }
        public string Unvani { get; set; }
        public string BirlikKodu { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public byte Durum { get; set; }

        public DateTime? DurumGuncellemeTarihi { get; set; }
        public DateTime? TUMBaslangicTarihi { get; set; }
        public DateTime? TUMBitisTarihi { get; set; }

        public string Telefon { get; set; }
        public string Fax { get; set; }

        public string Email { get; set; }
        public string WebAdresi { get; set; }

        public string UlkeKodu { get; set; }
        public string UlkeAdi { get; set; }

        public string IlKodu { get; set; }
        public string IlAdi { get; set; }

        public short IlceKodu { get; set; }
        public string IlceAdi { get; set; }

        public string Semt { get; set; }
        public string Adres { get; set; }

        public byte BaglantiSiniri { get; set; }
        public string BaglantiSiniriText { get; set; }
        public Nullable<short> UcretlendirmeKodu { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }

        public TUMNotlarListModel NotlarList { get; set; }
        public TUMIPBaglantiListModel IPBaglantilariList { get; set; }
        public TUMDokumanlarListModel DokumanlariList { get; set; }
        public TUMBankaHesaplariListModel BankaHesaplariList { get; set; }
        public TUMIletisimYetkilileriListModel IletisimYetkilileriList { get; set; }
        public TUMUrunleriListModel UrunleriList { get; set; }

        [IgnoreMap]
        public TUMLogoModel LogoModel { get; set; }

    }

    public class TUMLogoModel
    {
        public int Kodu { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }
    }

    public class TUMGuncelleModel : TUMEkleModel
    {
        public SelectList Durumlar { get; set; }

        public TUMNotlarListModel NotlarList { get; set; }
        public TUMIPBaglantiListModel IPBaglantilariList { get; set; }
        public TUMDokumanlarListModel DokumanlariList { get; set; }
        public TUMBankaHesaplariListModel BankaHesaplariList { get; set; }
        public TUMIletisimYetkilileriListModel IletisimYetkilileriList { get; set; }
        public TUMUrunleriListModel UrunleriList { get; set; }

        [IgnoreMap]
        public TUMLogoModel LogoModel { get; set; }
    }

    public static class TUMListProvider
    {
        public static List<SelectListItem> VarYokTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Absent },
                new SelectListItem() { Value = "1", Text = babonline.Exists }
            });

            return list;
        }

        public static string GetVarYok(short value)
        {
            switch (value)
            {
                case TUMAcenteSubeVar.Yok: return babonline.Absent;
                case TUMAcenteSubeVar.Var: return babonline.Exists;
            }

            return String.Empty;
        }
    }
}
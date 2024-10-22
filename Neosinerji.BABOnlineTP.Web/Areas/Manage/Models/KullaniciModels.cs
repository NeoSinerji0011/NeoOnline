using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class KullaniciListeProvider
    {
        public static List<SelectListItem> GorevTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.Manager },
                new SelectListItem() { Value = "2", Text = babonline.AsistantManager },
                new SelectListItem() { Value = "3", Text = babonline.Personel },
                new SelectListItem() { Value = "4", Text = babonline.PersonelKomisyonsuz }
            });

            return list;
        }

        public static List<SelectListItem> DurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Pasive },
                new SelectListItem() { Value = "1", Text = babonline.Active },
                new SelectListItem() { Value = "2", Text = babonline.Suspended }
            });

            return list;
        }

        public static List<SelectListItem> DurumTipleriListe()
        {
            List<SelectListItem> list = DurumTipleri();

            list.Insert(0, new SelectListItem() { Value = "", Text = babonline.All });

            return list;
        }

        public static List<SelectListItem> TeklifPoliceUretimTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.No},
                new SelectListItem() { Value = "1", Text = babonline.Yes }
            });

            return list;
        }
    }

    public class KullaniciModel
    {
        public int? KullaniciKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TVMKodu { get; set; }

        public string TVMUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TCKNLength", MinimumLength = 11)]
        public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Gorevi { get; set; }

        [IgnoreMap]
        public string GorevAdi { get; set; }

        public int? YoneticiKodu { get; set; }

        public string YoneticiAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int DepartmanKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YetkiGrubu { get; set; }

        [IgnoreMap]
        public string YetkiAdi { get; set; }

        [IgnoreMap]
        public string DepartmanAdi { get; set; }

        public string TeknikPersonelKodu { get; set; }

        //[TelefonValidation(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PhoneNumber")]
        public string Telefon { get; set; }

        //[TelefonValidation(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PhoneNumber")]
        public string CepTelefon { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [EPostaAdresi]
        public string Email { get; set; }

        public string MTKodu { get; set; }

        public byte Durum { get; set; }

        public byte TeklifPoliceUretimi { get; set; }

        public string TeklifPoliceUretimiText { get; set; }

        public DateTime? KayitTarihi { get; set; }

        public byte SifreDurumKodu { get; set; }

        public string SkypeNumara { get; set; }

        //public byte? APYmi { get; set; }

        public string ProjeKodu { get; set; }

        public List<SelectListItem> Yetkiler { get; set; }
        public List<SelectListItem> Departmanlar { get; set; }
        public List<SelectListItem> GorevTipleri { get; set; }
        public List<SelectListItem> TeklifPoliceUretimTipleri { get; set; }
    }

    public class KullaniciGuncelleModel
    {
        public int? KullaniciKodu { get; set; }
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Gorevi { get; set; }

        public int? YoneticiKodu { get; set; }

        public string YoneticiAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int DepartmanKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int YetkiGrubu { get; set; }

        public string TeknikPersonelKodu { get; set; }

        public string Telefon { get; set; }

        public string CepTelefon { get; set; }

        [EPostaAdresi]
        public string Email { get; set; }

        public string MTKodu { get; set; }

        public byte Durum { get; set; }

        public byte TeklifPoliceUretimi { get; set; }

        public byte SifreDurumKodu { get; set; }

        public string SkypeNumara { get; set; }

        // public byte? APYmi { get; set; }

        public List<SelectListItem> Yetkiler { get; set; }
        public List<SelectListItem> DurumTipleri { get; set; }
        public List<SelectListItem> Departmanlar { get; set; }
        public List<SelectListItem> GorevTipleri { get; set; }
        public List<SelectListItem> TeklifPoliceUretimTipleri { get; set; }
    }

    public class KullaniciListeEkranModel
    {
        public int? TVMKodu { get; set; }
        public short TVMTipi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string TCKN { get; set; }
        public byte? Durum { get; set; }
        public string TeknikPersonelKodu { get; set; }

        public List<SelectListItem> TVMTipleri { get; set; }
        public List<SelectListItem> DurumTipleri { get; set; }
    }



}
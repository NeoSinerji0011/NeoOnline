using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Models
{
    public class UlkeListModel
    {
        public List<UlkeModel> Items { get; set; }
    }

    public class IlListModel
    {
        public List<IlModel> Items { get; set; }
    }

    public class IlceListModel
    {
        public List<IlceModel> Items { get; set; }
    }

    public class UlkeModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeAdi { get; set; }
    }

    public class IlModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlAdi { get; set; }
    }

    public class IlceModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IlceKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlceAdi { get; set; }

    }

    public class TelefonModel
    {
        public TelefonModel()
        {
            UluslararasiKod = "90";
        }

        public TelefonModel(string telefonNo)
            : base()
        {
            if (!String.IsNullOrEmpty(telefonNo))
            {
                string[] parts = telefonNo.Split('-');

                if (parts.Length == 3)
                {
                    this.UluslararasiKod = parts[0];
                    this.AlanKodu = parts[1];
                    this.Numara = parts[2];
                }
            }
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.AlanKodu) && String.IsNullOrEmpty(this.Numara))
                return String.Empty;

            return String.Format("{0}-{1}-{2}", this.UluslararasiKod, this.AlanKodu, this.Numara);
        }

        public string UluslararasiKod { get; set; }
        public string AlanKodu { get; set; }
        public string Numara { get; set; }
    }

    public enum Culture
    {
        tr = 1,
        en = 2,
        it = 3,
        fr = 4,
        es = 5
    }

    public static class CultureText
    {
        public static string TR = "tr";
        public static string EN = "en";
    }

    public class HaritaIletisimFormModel
    {

        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }
        public string Telefon { get; set; }

        public HaritaTeklifPoliceFormModel teklifPoliceModel = new HaritaTeklifPoliceFormModel();
    }
    public class HaritaTeklifPoliceFormModel
    {
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string tcVkn { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

         [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Telefon { get; set; }
    }

    public class KullaniciKayitModel
    {
        public bool KayitYapildiMi { get; set; }
        public string KayitMesaj { get; set; }
        public string HataMesaj { get; set; }
        public void SorgulamaHata(string mesaj)
        {
            this.KayitYapildiMi = false;
            this.HataMesaj = mesaj;
        }

    }
}
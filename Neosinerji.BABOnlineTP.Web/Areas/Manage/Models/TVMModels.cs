using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using common = Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using AutoMapper;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMListeModel
    {
        public int? Kodu { get; set; }
        public string Unvani { get; set; }
        public short? Tipi { get; set; }
        public int? BagliOlduguTVMKodu { get; set; }

        public SelectList TVMTipleri { get; set; }
    }

    public class NeoConnectListeModel
    {
        public int TVMKodu { get; set; }
        public List<SelectListItem> TVMKodlari { get; set; }

        public List<NeoConnectSifreListModel> tableList = new List<NeoConnectSifreListModel>();
    }
    public class NeoConnectSifreListModel
    {
        public int TVMKodu { get; set; }
        public int TVMUnvani { get; set; }
        public int? AltTVMKodu { get; set; }
        public string AltTVMUnvani { get; set; }
        public string SirketAdi { get; set; }
        public string GrupAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string ProxyIpPort { get; set; }
    }

    public static class TVMListProvider
    {
        //Liste Ekranında Kullanılıyor
        public static List<SelectListItem> TVMTipleriFull()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "2", Text = babonline.TVM_Agency },
                new SelectListItem() { Value = "11", Text = babonline.TVM_Agency+"(i)" },
                new SelectListItem() { Value = "3", Text = babonline.TVM_Bank },
                new SelectListItem() { Value = "1", Text = babonline.TVM_Broker },
                new SelectListItem() { Value = "4", Text = babonline.TVM_Broker_Yurtici },
                new SelectListItem() { Value = "5", Text = babonline.CustomerRepresentative },
                new SelectListItem() { Value = "6", Text = babonline.SalesChannel },
                new SelectListItem() { Value = "7", Text = babonline.SubResourceSharedAgent },
                new SelectListItem() { Value = "8", Text = babonline.InternetDistanceSales},
                new SelectListItem() { Value = "9", Text = babonline.OutsourcedBusinessAgent },
                new SelectListItem() { Value = "10", Text = babonline.Substation },
            });

            return list;
        }

        public static List<SelectListItem> TVMTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "2", Text = babonline.TVM_Agency },
                new SelectListItem() { Value = "11", Text = babonline.TVM_Agency+"(i)" },
                new SelectListItem() { Value = "3", Text = babonline.TVM_Bank },
                new SelectListItem() { Value = "1", Text = babonline.TVM_Broker },
                new SelectListItem() { Value = "4", Text = babonline.TVM_Broker_Yurtici },
            });

            return list;
        }
        public static List<SelectListItem> TVMTipleriSubeAcente()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "5", Text = babonline.CustomerRepresentative +" (" + babonline.Salesman + ")"},
                new SelectListItem() { Value = "6", Text = babonline.Producer },
                new SelectListItem() { Value = "7", Text = babonline.TaliAgency },
                new SelectListItem() { Value = "8", Text = babonline.InternetDistanceSales},
                new SelectListItem() { Value = "9", Text = babonline.OutsourcedBusinessAgent },
                new SelectListItem() { Value = "10", Text = babonline.Substation },

            });

            return list;
        }

        public static List<SelectListItem> ProjeTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = TVMProjeKodlari.Aegon , Text = TVMProjeKodlari.Aegon },
                new SelectListItem() { Value = TVMProjeKodlari.Mapfre , Text =  TVMProjeKodlari.Mapfre},
                new SelectListItem() { Value = TVMProjeKodlari.Mapfre_DisAcente, Text = TVMProjeKodlari.Mapfre_DisAcente},
                new SelectListItem() { Value = TVMProjeKodlari.Lilyum, Text = TVMProjeKodlari.Lilyum},
            });

            return list;
        }

        public static List<SelectListItem> TVMTipleriListe()
        {
            List<SelectListItem> list = TVMTipleri();

            list.Insert(0, new SelectListItem() { Value = "", Text = babonline.All });

            return list;
        }

        public static string GetTVMTipiText(short tipi)
        {
            switch (tipi)
            {
                case common.TVMTipleri.Broker: return babonline.TVM_Broker;
                case common.TVMTipleri.Acente: return babonline.TVM_Agency;
                case common.TVMTipleri.Banka: return babonline.TVM_Bank;
                case common.TVMTipleri.YurticiBroker: return babonline.TVM_Broker_Yurtici;
                case common.TVMTipleri.MusteriTemsilcisi: return babonline.CustomerRepresentative;
                case common.TVMTipleri.SatisKanali: return babonline.SalesChannel;
                case common.TVMTipleri.PaylasimliAcente: return babonline.SubResourceSharedAgent;
                case common.TVMTipleri.Internet: return babonline.InternetDistanceSales;
                case common.TVMTipleri.UzerindenIsYapilanACente: return babonline.OutsourcedBusinessAgent;
                case common.TVMTipleri.Sube: return babonline.Substation;
                case common.TVMTipleri.AcenteIhsanYazilim: return babonline.TVM_Agency + "(i)";
            }

            return String.Empty;
        }
        public static List<SelectListItem> BolgeYetkiliTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.No },
                new SelectListItem() { Value = "1", Text = babonline.Yes }
            });

            return list;
        }
        public static List<SelectListItem> PoliceTransferAcentesi()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.No },
                new SelectListItem() { Value = "1", Text = babonline.Yes }
            });

            return list;
        }
        public static List<SelectListItem> ProfilTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.TVM_HQ },
                new SelectListItem() { Value = "0", Text = babonline.TVM_Branch }
            });

            return list;
        }

        public static string ProfilTipiText(byte kodu)
        {
            string result = String.Empty;

            switch (kodu)
            {
                case 0: result = babonline.TVM_Branch; break;
                case 1: result = babonline.TVM_HQ; break;
            }

            return result;
        }

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
                case TVMAcenteSubeVar.Yok: return babonline.Absent;
                case TVMAcenteSubeVar.Var: return babonline.Exists;
            }

            return String.Empty;
        }

        public static List<SelectListItem> NotOncelikTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Low},
                new SelectListItem() { Value = "2", Text = babonline.Medium },
                new SelectListItem() { Value = "3", Text = babonline.High }
            });

            return list;
        }

        public static string GetNotOnceligiText(short tipi)
        {
            switch (tipi)
            {
                case common.KullaniciNotOncelikTipleri.Dusuk: return babonline.Low;
                case common.KullaniciNotOncelikTipleri.Orta: return babonline.Medium;
                case common.KullaniciNotOncelikTipleri.Yuksek: return babonline.High;
            }

            return String.Empty;
        }

        public static List<SelectListItem> GetIslemTipleri()
        {
            List<SelectListItem> islemTipleri = new List<SelectListItem>();

            islemTipleri.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Center },
                new SelectListItem() { Value = "1", Text = babonline.Group }
            });
            return islemTipleri;
        }
    }

    public class TVMEkleModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? Kodu { get; set; }

        public int MerkezTVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Unvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short MerkezTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short SubeAcenteTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KayitNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiDairesi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength", MinimumLength = 10)]
        public string VergiNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TCKNLength", MinimumLength = 11)]
        public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Profili { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? BagliOlduguTVMKodu { get; set; }
        public byte AcentSuvbeVar { get; set; }

        public byte BolgeYetkilisiMi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Durum { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        public DateTime? SozlesmeBaslamaTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_DateFormat")]
        [Display(ResourceType = typeof(babonline), Name = "TVM_ContractEndDate")]
        public DateTime? SozlesmeDondurmaTarihi { get; set; }
        public int? SonPoliceOnayTarihi { get; set; }

        public int? bolgeYetkilisi { get; set; }

        public byte PoliceTransferiYapilacakMi { get; set; }

        [EPostaAdresi]
        public string Email { get; set; }

        public string WebAdresi { get; set; }
        public string TVMUnvani { get; set; }
        public string ProjeKodu { get; set; }


        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? IlceKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Telefon { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Fax { get; set; }

        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }
        public int? BolgeKodu { get; set; }

        public short? SifreKontralSayisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? SifreDegistirmeGunu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? SifreIkazGunu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte BaglantiSiniri { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte MuhasebeEntegrasyonu { get; set; }

        public SelectList TVMMerkezTipleri { get; set; }
        public SelectList TVMSubeAcenteTipleri { get; set; }
        public SelectList ProjeTipleri { get; set; }
        public SelectList ProfilTipleri { get; set; }
        public SelectList AcenteSubeVarTipleri { get; set; }
        public SelectList BaglantiSiniriVarYok { get; set; }
        public List<SelectListItem> BolgeYetkilileri { get; set; }
        public SelectList BolgeYetkiliTipleri { get; set; }

        public List<SelectListItem> Bolgeler { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }


        public SelectList PoliceTransferAcentesi { get; set; }
    }

    public class TVMLogoModel
    {
        public int Kodu { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }
    }

    public class TVMDetayModel
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public short Tipi { get; set; }
        public string TipiText { get; set; }
        public string BolgeYetkilisiMiText { get; set; }
        public string PoliceTransferiYapilacakmiText { get; set; }
        public string KayitNo { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string TCKN { get; set; }
        public byte Profili { get; set; }
        public int BagliOlduguTVMKodu { get; set; }
        public string BagliOlduguTVMAdi { get; set; }
        public int? GrupKodu { get; set; }
        public string BolgeYetkilisiUnvani { get; set; }

        public byte AcentSuvbeVar { get; set; }
        public string AcentSuvbeVarText { get; set; }
        public string MuhasebeEntegrasyonVarText { get; set; }
        public byte Durum { get; set; }
        public DateTime SozlesmeBaslamaTarihi { get; set; }
        public DateTime SozlesmeDondurmaTarihi { get; set; }
        public int? SonPoliceOnayTarihi { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }

        public int BolgeKodu { get; set; }
        public string ProjeKodu { get; set; }

        public short SifreKontralSayisi { get; set; }
        public short SifreDegistirmeGunu { get; set; }
        public short SifreIkazGunu { get; set; }
        public string BaglantiSiniriText { get; set; }

        public TVMIPBaglantiListModel IPBaglantilariList { get; set; }
        public TVMBolgeleriListModel BolgeleriList { get; set; }
        public TVMDepartmanlarListModel DepartmanlarList { get; set; }
        public TVMNotlarListModel NotlarList { get; set; }
        public TVMDokumanlarListModel DokumanlariList { get; set; }
        public TVMBankaHesaplariListModel BankaHesaplari { get; set; }
        public TVMIletisimYetkilileriListModel IletisimYetkilileri { get; set; }

        [IgnoreMap]
        public TVMLogoModel LogoModel { get; set; }
    }

    public class TVMGuncelleModel : TVMEkleModel
    {
        public SelectList Durumlar { get; set; }

        public TVMIPBaglantiListModel IPBaglantilariList { get; set; }
        public TVMBolgeleriListModel BolgeleriList { get; set; }
        public TVMDepartmanlarListModel DepartmanlarList { get; set; }
        public TVMNotlarListModel NotlarList { get; set; }
        public TVMDokumanlarListModel DokumanlariList { get; set; }
        public TVMAcentelikleriListModel AcentelikleriList { get; set; }
        public TVMBankaHesaplariListModel BankaHesaplariList { get; set; }
        public TVMIletisimYetkilileriListModel IletisimYetkilileriList { get; set; }
        public List<TVMWebServisKullanicilariModel> WebServisKullanicilari { get; set; }
        public List<SirketWebEkranModel> NeoConnectKullanicilari { get; set; } //neoconnnect şifre değiştirme modeli
        public List<NeoConnectTvmSirketYetkileriModels> NeoConnectTvmSigortaSirketiKullanicilari { get; set; } //neoconnnect tvm sigorta modeli
        public List<NeoConnectYasakliUrlModels> NeoConnectYasakliUrlModelsKullanicilari { get; set; } //neoconnnect yasakli url modeli

        public TVMBolgeleriModel Bolgeleri { get; set; }
        public TVMDepartmanlarModel Departmanlari { get; set; }

        public string AcentSuvbeVarText { get; set; }
        public string BagliOlduguTVMText { get; set; }
        public string BolgeYetkilisiText { get; set; }
        public string TVMProfiliText { get; set; }
        public int id { get; set; }

        [IgnoreMap]
        public TVMLogoModel Logo { get; set; }
    }


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models
{
    public class TopluPoliceTahsilatModel
    {
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<int> TVMListe { get; set; }
        public string[] tvmList { get; set; }

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string BransList { get; set; }
        public bool BolgeYetkilisiMi { get; set; }

        public string SigortaSirket { get; set; }
        public SelectList SigortaSirketleri { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] SigortaSirketleriSelectList { get; set; }
        public List<string> SigortaSirketleriListe { get; set; }
        public SelectList tvmler { get; set; }
        public string[] BransSelectList { get; set; }
        public MultiSelectList BranslarItems { get; set; }
        public MultiSelectList Branslar { get; set; }
        public byte PoliceTarihTipi { get; set; }
        public string PoliceNo { get; set; }
        public string[] TVMLerSelectList { get; set; }
        public MultiSelectList TVMLerItems { get; set; }
        public List<TopluTahsilatListModel> list { get; set; }
    }
    public class TopluTahsilatListModel
    {
   
        public string ParaBirimi { get; set; }
        public decimal? OdenenTutar { get; set; }
        public decimal? KalanTutar { get; set; }
        public string OdemeBelgeNo { get; set; }
        public DateTime VadeTarihi { get; set; }
        public string OdemeTipi { get; set; }
        public string OdemeSekli { get; set; }
        public string PoliceNo { get; set; }
        public int PoliceId { get; set; }
        public string SigortaEttirenUnvani { get; set; }
        public string SigortaliUnvani { get; set; }
        public string YenilemeNo { get; set; }
        public string EkNo { get; set; }
        public string UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TaliAcenteKodu { get; set; }
        public string TaliAcenteAdi { get; set; }
        public string DisKaynakKodu { get; set; }
        public string DisKaynakAdi { get; set; }
        public string SigortaSirketi { get; set; }
        public string AcenteKodu { get; set; }
        public string AcenteUnvani { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal? NetPrim { get; set; }
        public decimal? BrütPrim { get; set; }
        public decimal? Komisyon { get; set; } // Tali acente bu alanı göremiyecek. Merkez acentenin aldığı komisyon.
        public decimal? VerilenKomisyon { get; set; }
        public int TaksitSayisi { get; set; }
        public string KimlikNo { get; set; }
    }
}
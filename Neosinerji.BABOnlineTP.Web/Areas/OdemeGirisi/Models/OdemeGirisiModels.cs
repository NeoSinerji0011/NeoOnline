using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.OdemeGirisi.Models
{
    public class OdemeGirisiModel
    {
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public int AcenteTVMKoduTali { get; set; }
        public int Donem { get; set; }
        public byte IslemTipi { get; set; }
        public string TVMListe { get; set; }
        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }
        public List<OdemeGirisiList> odemelerGirisiListe { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public List<KesintiTurleri> KesintiTurleri { get; set; }
        public SelectList Islemler { get; set; }
    }

    public class OdemeGirisiList
    {
        public int KesintiTuruKodu { get; set; }
        public string KesintiTuruAdi { get; set; }

        public string OcakBorc { get; set; }
        public string OcakAlacak { get; set; }
        public string SubatBorc { get; set; }
        public string SubatAlacak { get; set; }
        public string MartBorc { get; set; }
        public string MartAlacak { get; set; }
        public string NisanBorc { get; set; }
        public string NisanAlacak { get; set; }
        public string MayisBorc { get; set; }
        public string MayisAlacak { get; set; }
        public string HaziranBorc { get; set; }
        public string HaziranAlacak { get; set; }
        public string TemmuzBorc { get; set; }
        public string TemmuzAlacak { get; set; }
        public string AgustosBorc { get; set; }
        public string AgustosAlacak { get; set; }
        public string EylulBorc { get; set; }
        public string EylulAlacak { get; set; }
        public string EkimBorc { get; set; }
        public string EkimAlacak { get; set; }
        public string KasimBorc { get; set; }
        public string KasimAlacak { get; set; }
        public string AralikBorc { get; set; }
        public string AralikAlacak { get; set; }

        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public int Donem { get; set; }
        public int Year { get; set; }
        public int KayitId { get; set; }
    }

    public class OdemeGirisiTransferModel
    {
        public string Path { get; set; }
        public int AcenteTVMKodu { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Yil { get; set; }
    }
}
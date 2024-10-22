using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceUretimHedefPlanlanan.Models
{
    public class PoliceUretimHedefPlanlananModel
    {
        public int KayitId { get; set; }
        public int Yil { get; set; }
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public int Brans { get; set; }
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public int PoliceAdedi { get; set; }
        public decimal Prim { get; set; }
        public List<Bran> BransListe { get; set; }
        public List<PoliceUretimHedefPlanlananListe> policeUretimHedefPlanlananListe { get; set; }

        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Year { get; set; }

    }
    public class PoliceUretimHedefPlanlananListe
    {
        public int BransKodu { get; set; }
        public string BransAdi { get; set; }

        public int OcakAdedi { get; set; }
        public decimal OcakPrim { get; set; }
        public int SubatAdedi { get; set; }
        public decimal SubatPrim { get; set; }
        public int MartAdedi { get; set; }
        public decimal MartPrim { get; set; }
        public int NisanAdedi { get; set; }
        public decimal NisanPrim { get; set; }
        public int MayisAdedi { get; set; }
        public decimal MayisPrim { get; set; }
        public int HaziranAdedi { get; set; }
        public decimal HaziranPrim { get; set; }
        public int TemmuzAdedi { get; set; }
        public decimal TemmuzPrim { get; set; }
        public int AgustosAdedi { get; set; }
        public decimal AgustosPrim { get; set; }
        public int EylulAdedi { get; set; }
        public decimal EylulPrim { get; set; }
        public int EkimAdedi { get; set; }
        public decimal EkimPrim { get; set; }
        public int KasimAdedi { get; set; }
        public decimal KasimPrim { get; set; }
        public int AralikAdedi { get; set; }
        public decimal AralikPrim { get; set; }

        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public int AcenteTVMKodu { get; set; }
        public string AcenteTVMUnvani { get; set; }
        public int Yil { get; set; }
        public int Year { get; set; }
        public int KayitId { get; set; }
    }


}
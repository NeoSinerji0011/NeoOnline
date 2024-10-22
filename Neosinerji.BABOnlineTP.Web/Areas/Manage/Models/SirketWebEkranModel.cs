using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class SirketWebEkranModel
    {
        public int TVMKodu { get; set; }
        public Nullable<int> AltTVMKodu { get; set; }
        public string AltTvmUnvan { get; set; }
        public int TUMKodu { get; set; }
        public string SigortaSirketAdi { get; set; }
        public string TUMUnvan { get; set; }
        public string TVMUnvan { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string InputTextKullaniciId { get; set; }
        public string InputTextAcenteKoduId { get; set; }
        public string InputTextSifreId { get; set; }
        public string InputTextGirisId { get; set; }
        public string LoginUrl { get; set; }
        public string ProxyIpPort { get; set; }
        public string ProxyKullaniciAdi { get; set; }
        public string ProxySifre { get; set; }
        public int Id { get; set; }
        public int? GrupKodu { get; set; }
        public string GrupAdi { get; set; }
        public string IslemMesaji { get; set; }

        public List<SelectListItem> TUMListesi { get; set; }
        public List<SelectListItem> TVMListesi { get; set; }

        public List<SelectListItem> GrupListesi { get; set; }

        public List<SelectListItem> SirketGrupKullaniciListesi { get; set; }
        public List<NeoConnectSifreIslemleriListModel> sifreList = new List<NeoConnectSifreIslemleriListModel>();
        public byte IslemTipi { get; set; }
        public SelectList IslemTipleri { get; set; }


    }
    public class NeoConnectSifreIslemleriListModel
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public int? AltTVMKodu { get; set; }
        public string AltTVMUnvani { get; set; }
        public int TUMKodu { get; set; }
        public int? GrupKodu { get; set; }
        public string TUMUnvan { get; set; }
        public string GrupAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string ProxyIpPort { get; set; }
        public string ProxyKullaniciAdi { get; set; }
        public string ProxySifre { get; set; }
        public List<SelectListItem> TVMListesi { get; set; }
    }
    public class NeoConnectListGrupTanımlama
    {
        public string GrupAdi { get; set; }
        public string SirketKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int id { get; set; }

    }
    public class NeoConnectGrupTanımlama
    {
        public List<SelectListItem> TUMListesi { get; set; }
        public string TUMKodu { get; set; }
        public string GrupAdi { get; set; }
        public string SirketKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }

        public List<NeoConnectListGrupTanımlama> grupListesi = new List<NeoConnectListGrupTanımlama>();

    }
}


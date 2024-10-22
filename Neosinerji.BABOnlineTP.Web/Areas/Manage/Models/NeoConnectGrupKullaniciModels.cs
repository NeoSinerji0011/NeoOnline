using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class NeoConnectListModel
    {
        public List<NeoConnectGrupKullaniciModels> listGrup { get; set; }
        public List<NeoConnectMerkezKullaniciModels> listMerkez { get; set; }

        public int SirketKodu { get; set; }
        public List<SelectListItem> SigortaSirketleri { get; set; }
        public int AktifTvmKodu { get; set; }
        public string returnURL { get; set; }

        public byte IslemTipi { get; set; }
        public SelectList IslemTipleri { get; set; }
    
    }
    public class NeoConnectGrupKullaniciModels
    {
        public int GrupKodu { get; set; }
        public string GrupAdi { get; set; }
        public string SirketKodu { get; set; }
        public string SirketUnvani { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int TvmKodu { get; set; }
        public string TvmUnvani { get; set; }
        public string SmsKodTelNo { get; set; }

        public string SmsKodSecretKey1 { get; set; }

        public string SmsKodSecretKey2 { get; set; }


    }
    public class NeoConnectMerkezKullaniciModels
    {
        public int? GrupKodu { get; set; }
        public string GrupAdi { get; set; }
        public int TUMKodu { get; set; }
        public string SirketUnvani { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int TvmKodu { get; set; }
        public string TvmUnvani { get; set; }
        public string ProxyIpPort { get; set; }
        public string SmsKodTelNo { get; set; }

        public string SmsKodSecretKey1 { get; set; }
        public string SmsKodSecretKey2 { get; set; }
    }
}
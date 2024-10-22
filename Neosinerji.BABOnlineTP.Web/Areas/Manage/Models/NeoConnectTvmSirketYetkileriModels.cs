using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class NeoConnectTvmSirketYetkileriModels
    {
        public int TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public int Id { get; set; }
        public string TUMKodu { get; set; }
        public string TUMUnvan { get; set; }
        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
        public List<SelectListItem> TVMListesi { get; set; }
        public List<SelectListItem> TUMListesi { get; set; }
        public string SigortaSirket { get; set; }

        public List<NeoConnectTvmSirketYetkileriModel> sirketYetkliList = new List<NeoConnectTvmSirketYetkileriModel>();

        public string[] TVMKoduSelectList { get; set; }// ************************************************
        public MultiSelectList TVMMultiSelectList { get; set; } // ************************************************
    }

    public class NeoConnectSirketYetkileriListModel
    {
        public int TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public List<SelectListItem> TVMListesi { get; set; }
        public List<NeoConnectTvmSirketYetkileriModels> NeoConnectTvmSigortaSirketiKullanicilari { get; set; } //neoconnnect tvm sigorta modeli
        public List<NeoConnectTvmSirketYetkileriModel> sirketYetkliList = new List<NeoConnectTvmSirketYetkileriModel>();
    }

    public class NeoConnectTvmSirketYetkileriModel
    {
        public int Id { get; set; }
        public string TUMUnvan { get; set; }
        public string TVMUnvan { get; set; }
        public int TVMKodu { get; set; }
        public string Durum { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class MeslekModel
    {
        public int MeslekKodu { get; set; }
        public string MeslekAdi { get; set; }
    }
    public class TanimModel
    {
        public string TanimTipi { get; set; }
        public short TanimId { get; set; }
        public string Aciklama { get; set; }
    }



}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectMerkezProxyKullanicilari
    {
        public int Id { get; set; }
        public string SigortaSirketKodu { get; set; }
        public Nullable<int> TvmKodu { get; set; }
        public Nullable<int> KullaniciKodu { get; set; }

    }
}

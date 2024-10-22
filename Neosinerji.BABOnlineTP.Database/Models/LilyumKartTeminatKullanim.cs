using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
       public partial class LilyumKartTeminatKullanim
    {
        public LilyumKartTeminatKullanim()
        {
          
        }

        public int Id { get; set; }
        public int TvmKodu { get; set; }
        public int KullaniciKodu { get; set; }
        public string ReferansNo { get; set; }
        public string LilyumKartNo { get; set; }
        public int TeminatId { get; set; }
        public byte ToplamKullanimHakkiAdet { get; set; }
        public byte ToplamKullanilanAdet { get; set; }
        public Nullable<DateTime> TeminatSonKullanilanTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public Nullable<DateTime>  GuncellemeTarihi { get; set; }
        public int EkleyenKullanici { get; set; }
        public Nullable<int> GuncelleyenKullanici { get; set; }
    }
}

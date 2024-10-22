using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using static Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap.Muhasebe_CariHesapService;

namespace Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap
{
   public class CariHesapBorcAlacakServiceModel
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string CariHesapKodu { get; set; }
        public int CariHesapId { get; set; }
        public string KimlikNo { get; set; }
        public int Donem { get; set; }
        public Nullable<decimal> Borc1 { get; set; }
        public Nullable<decimal> Alacak1 { get; set; }
        public Nullable<decimal> Borc2 { get; set; }
        public Nullable<decimal> Alacak2 { get; set; }
        public Nullable<decimal> Borc3 { get; set; }
        public Nullable<decimal> Alacak3 { get; set; }
        public Nullable<decimal> Borc4 { get; set; }
        public Nullable<decimal> Alacak4 { get; set; }
        public Nullable<decimal> Borc5 { get; set; }
        public Nullable<decimal> Alacak5 { get; set; }
        public Nullable<decimal> Borc6 { get; set; }
        public Nullable<decimal> Alacak6 { get; set; }
        public Nullable<decimal> Borc7 { get; set; }
        public Nullable<decimal> Alacak7 { get; set; }
        public Nullable<decimal> Borc8 { get; set; }
        public Nullable<decimal> Alacak8 { get; set; }
        public Nullable<decimal> Borc9 { get; set; }
        public Nullable<decimal> Alacak9 { get; set; }
        public Nullable<decimal> Borc10 { get; set; }
        public Nullable<decimal> Alacak10 { get; set; }
        public Nullable<decimal> Borc11 { get; set; }
        public Nullable<decimal> Alacak11 { get; set; }
        public Nullable<decimal> Borc12 { get; set; }
        public Nullable<decimal> Alacak12 { get; set; }
        public DateTime KayitTarihi { get; set; }
        public Nullable<DateTime> GuncellemeTarihi { get; set; }
        public int PoliceId { get; set; }
       public  List<CariHareketKaydetModel> cariHareketList = new List<CariHareketKaydetModel>();
    }
}

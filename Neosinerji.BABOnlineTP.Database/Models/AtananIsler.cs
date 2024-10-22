using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AtananIsler
    {
        public AtananIsler()
        {
            this.AtananIsNots = new List<AtananIsNotlar>();
        }
        public int IsId { get; set; }
        public byte IsTipi { get; set; }
        public int TalepKanaliKodu { get; set; }        
        public int IsAlanTVMKodu { get; set; }
        public Nullable<int> IsAlanKullaniciKodu { get; set; }
        public string Baslik { get; set; }
        public Nullable<int> PoliceId { get; set; }
        public string PoliceNumarasi { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string SigortaSirketKodu { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public byte Durum { get; set; }
        public string Aciklama { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime TahminiBitisTarihi { get; set; }
        public Nullable<DateTime> TamamlanmaTarihi { get; set; }
        public string TalepYapanAcente { get; set; }
        public string GonderenTCVKN { get; set; }
        public string GonderenAdiSoyadi { get; set; }
        public string GonderenEmail { get; set; }
        public string GonderenTel { get; set; }
        public string GonderenFax { get; set; }
        public byte OncelikSeviyesi { get; set; }
        public int IsAtayanTVMKodu { get; set; }
        public int IsAtayanKullaniciKodu { get; set; }
        public string EvrakNo { get; set; }
        public virtual ICollection<AtananIsNotlar> AtananIsNots { get; set; }
    }

}

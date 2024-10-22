using Neosinerji.BABOnlineTP.Database.Common;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    [Serializable]
    public class IslerimProcedureModel
    {
        public int IsNumarasi { get; set; }  
        public string TipAciklama { get; set; }
        public byte Durum { get; set; }
        public byte OncelikSeviyesi { get; set; }    
        public string  Baslik { get; set; }
        public string TalepAdi { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime TahminiBitisTarihi { get; set; }      
        public string TalepYapanAcente { get; set; }
        //public string IsAtayanTvmUnvani { get; set; }
        //public int IsAtayanKullaniciKodu { get; set; }
        //public string IsAtayanKullaniciUnvani { get; set; }
    } 
}


namespace Neosinerji.BABOnlineTP.Database.Models
{    
        public partial class HDIKaskoHukuksalKorumaBedelleri
        {
            public int Id { get; set; }
            public string KullanimTarziKodu { get; set; }
            public string Kod2 { get; set; }
            public string Kademe { get; set; }
            public decimal MotorluAracBedeli { get; set; }
            public decimal SurucuyeBagliBedeli { get; set; }
            public string Aciklama { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekoSigorta_Business.EUREKO
{
    public class EurekoMusteriRequestModel
    {
        public string KimlikNo { get; set; }
        public string IlKodu { get; set; }
        public string IlceKodu { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public int TVMKodu { get; set; }
    }

    public class EurekoMusteriResponseModel
    {
        public string Hata { get; set; }
        public string MusteriNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
    }


}

using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifProvizyon
    {
        public int ProvizyonId { get; set; }
        public int TeklifId { get; set; }
        public string PartajNo { get; set; }
        public string BasvuruNumarasi { get; set; }
        public int OdemeTuru { get; set; }
        public string OdemeyiYapanTCKN { get; set; }
        public string KrediKarti_IlkAltiSonDort { get; set; }
        public string ParaBirimi { get; set; }
        public System.DateTime ProvizyonTarihi { get; set; }
        public decimal Tutar { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int KaydedenKullanici { get; set; }
        public string OnayKodu { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}

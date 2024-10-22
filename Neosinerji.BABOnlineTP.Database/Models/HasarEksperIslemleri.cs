using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarEksperIslemleri
    {
        public int EksperId { get; set; }
        public int HasarId { get; set; }
        public Nullable<int> EksperSorumlusuKodu { get; set; }
        public Nullable<decimal> TahminiHasarBedeli { get; set; }
        public Nullable<short> TahminiHasarParaBirimi { get; set; }
        public Nullable<decimal> AnlasmaliServisBedeli { get; set; }
        public Nullable<short> AnlasmaliServisParaBirimi { get; set; }
        public Nullable<decimal> TahakkukBedeli { get; set; }
        public Nullable<short> TahakkukParaBirimi { get; set; }
        public Nullable<decimal> RucuBedeli { get; set; }
        public Nullable<short> RucuBedeliParaBirimi { get; set; }
        public Nullable<decimal> RedBedeli { get; set; }
        public Nullable<short> RedParaBirimi { get; set; }
        public string AsistansFirma { get; set; }
        public Nullable<decimal> AsistansFirmaBedeli { get; set; }
        public Nullable<short> AsistansFirmaParaBirimi { get; set; }
        public Nullable<System.DateTime> OdemeTarihi { get; set; }
        public Nullable<System.DateTime> BildirimTarihi { get; set; }
        public string GelisSaati { get; set; }
        public string BeklemeSuresi { get; set; }
        public virtual HasarGenelBilgiler HasarGenelBilgiler { get; set; }
    }
}

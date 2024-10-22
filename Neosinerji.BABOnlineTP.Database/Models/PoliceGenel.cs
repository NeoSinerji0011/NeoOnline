using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceGenel
    {
        public PoliceGenel()
        {
            this.HasarGenelBilgilers = new List<HasarGenelBilgiler>();
            this.PoliceOdemePlanis = new List<PoliceOdemePlani>();
            this.PoliceTahsilats = new List<PoliceTahsilat>();
            this.PoliceVergis = new List<PoliceVergi>();
        }

        public int PoliceId { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public Nullable<int> TUMKodu { get; set; }
        public Nullable<int> UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }
        public string TUMBirlikKodu { get; set; }
        public string PoliceNumarasi { get; set; }
        public Nullable<int> EkNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public Nullable<System.DateTime> TanzimTarihi { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> NetPrim { get; set; }
        public Nullable<decimal> ToplamVergi { get; set; }
        public Nullable<decimal> Komisyon { get; set; }
        public Nullable<decimal> DovizliBrutPrim { get; set; }
        public Nullable<decimal> DovizliNetPrim { get; set; }
        public Nullable<decimal> DovizliKomisyon { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public string ParaBirimi { get; set; }
        public Nullable<int> TaliAcenteKodu { get; set; }
        public Nullable<int> UretimTaliAcenteKodu { get; set; }
        public string HashCode { get; set; }
        public Nullable<byte> Durum { get; set; }
        public Nullable<int> TaliKomisyonOrani { get; set; }
        public Nullable<decimal> TaliKomisyonOran { get; set; }
        public Nullable<decimal> TaliKomisyon { get; set; }
        public Nullable<int> TaliKomisyonGuncelleyenKullanici { get; set; }
        public Nullable<System.DateTime> TaliKomisyonGuncellemeTarihi { get; set; }
        public string ZeyilAdi { get; set; }
        public string GrupZeyilNo { get; set; }
        public Nullable<decimal> EkKomisyonTutari { get; set; }
        public string ZeyilKodu { get; set; }
        public string SirketZeyilAdi { get; set; }
        public Nullable<decimal> DovizKuru { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public Nullable<DateTime> CariHareketKayitTarihi { get; set; }
        public Nullable<byte> MuhasebeyeAktarildiMi { get; set; }   
        public virtual ICollection<HasarGenelBilgiler> HasarGenelBilgilers { get; set; }
        public virtual PoliceArac PoliceArac { get; set; }
        public virtual SigortaSirketleri SigortaSirketleri { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar{ get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
        public virtual ICollection<PoliceOdemePlani> PoliceOdemePlanis { get; set; }
 
        public virtual PoliceRizikoAdresi PoliceRizikoAdresi { get; set; }
        public virtual PoliceSigortaEttiren PoliceSigortaEttiren { get; set; }
        public virtual PoliceSigortali PoliceSigortali { get; set; }
        public virtual ICollection<PoliceTahsilat> PoliceTahsilats { get; set; }
        public virtual ICollection<PoliceVergi> PoliceVergis { get; set; }
        public Nullable<byte> Yeni_is {get ; set;}
 
        public Nullable<int> OnaylayanKullaniciKodu { get ; set;}
       
    }
}

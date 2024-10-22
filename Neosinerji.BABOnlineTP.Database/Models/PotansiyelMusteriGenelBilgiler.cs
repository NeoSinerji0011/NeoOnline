using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PotansiyelMusteriGenelBilgiler
    {
        public PotansiyelMusteriGenelBilgiler()
        {
            this.PotansiyelMusteriAdres = new List<PotansiyelMusteriAdre>();
            this.PotansiyelMusteriDokumen = new List<PotansiyelMusteriDokuman>();
            this.PotansiyelMusteriNots = new List<PotansiyelMusteriNot>();
            this.PotansiyelMusteriTelefons = new List<PotansiyelMusteriTelefon>();
        }

        public int PotansiyelMusteriKodu { get; set; }
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string TVMMusteriKodu { get; set; }
        public short MusteriTipKodu { get; set; }
        public string KimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public Nullable<System.DateTime> PasaportGecerlilikBitisTarihi { get; set; }
        public string VergiDairesi { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string Cinsiyet { get; set; }
        public Nullable<System.DateTime> DogumTarihi { get; set; }
        public string EMail { get; set; }
        public string WebUrl { get; set; }
        public Nullable<short> Uyruk { get; set; }
        public Nullable<short> EgitimDurumu { get; set; }
        public Nullable<int> MeslekKodu { get; set; }
        public Nullable<byte> MedeniDurumu { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public string FaaliyetOlcegi_ { get; set; }
        public string FaaliyetGosterdigiAnaSektor { get; set; }
        public string FaaliyetGosterdigiAltSektor { get; set; }
        public string SabitVarlikBilgisi { get; set; }
        public string CiroBilgisi { get; set; }
        public virtual ICollection<PotansiyelMusteriAdre> PotansiyelMusteriAdres { get; set; }
        public virtual ICollection<PotansiyelMusteriDokuman> PotansiyelMusteriDokumen { get; set; }
        public virtual ICollection<PotansiyelMusteriNot> PotansiyelMusteriNots { get; set; }
        public virtual ICollection<PotansiyelMusteriTelefon> PotansiyelMusteriTelefons { get; set; }
    }
}

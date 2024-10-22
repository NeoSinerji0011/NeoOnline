using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CariHesaplariMap : EntityTypeConfiguration<CariHesaplari>
    {
        public CariHesaplariMap()
        {
            // Primary Key
           this.HasKey(t => t.CariHesapId);

            this.Property(t => t.CariHesapKodu)
                .IsRequired()              
               .HasMaxLength(20);       


            this.Property(t => t.KimlikNo)
               .IsRequired()
               .HasMaxLength(11);

            this.Property(t => t.CariHesapTipi)
              .IsRequired()
              .HasMaxLength(10);

            this.Property(t => t.Unvan)
             .HasMaxLength(250);

            this.Property(t => t.TVMUnvani)
            .HasMaxLength(100);      

            this.Property(t => t.MusteriGrupKodu)
                .HasMaxLength(30);

            this.Property(t => t.VergiDairesi)
                .HasMaxLength(50);

            this.Property(t => t.Telefon1)
                .HasMaxLength(14);

            this.Property(t => t.Telefon2)
                .HasMaxLength(14);

            this.Property(t => t.CepTel)
              .HasMaxLength(14);

            this.Property(t => t.Email)
              .HasMaxLength(100);

            this.Property(t => t.DisaktarimMuhasebeKodu)
              .HasMaxLength(50);

            this.Property(t => t.DisaktarimCariKodu)
             .HasMaxLength(50);

            this.Property(t => t.KomisyonGelirleriMuhasebeKodu)
            .HasMaxLength(50);

            this.Property(t => t.WebAdresi)
            .HasMaxLength(50);

            this.Property(t => t.UlkeKodu)
            .HasMaxLength(3);

            this.Property(t => t.IlKodu)
            .HasMaxLength(3);

            this.Property(t => t.Adres)
            .HasMaxLength(500);

            this.Property(t => t.UyariNotu)
            .HasMaxLength(50);

            this.Property(t => t.BilgiNotu)
            .HasMaxLength(50);

            this.Property(t => t.SatisIadeleriMuhasebeKodu)
          .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("CariHesaplari");
            this.Property(t => t.CariHesapId).HasColumnName("CariHesapId");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.CariHesapKodu).HasColumnName("CariHesapKodu");
            this.Property(t => t.CariHesapTipi).HasColumnName("CariHesapTipi");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.Unvan).HasColumnName("Unvan");
            this.Property(t => t.TVMUnvani).HasColumnName("TVMUnvani");
            this.Property(t => t.MusteriGrupKodu).HasColumnName("MusteriGrupKodu");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.Telefon1).HasColumnName("Telefon1");
            this.Property(t => t.Telefon2).HasColumnName("Telefon2");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.DisaktarimMuhasebeKodu).HasColumnName("DisaktarimMuhasebeKodu");
            this.Property(t => t.DisaktarimCariKodu).HasColumnName("DisaktarimCariKodu");
            this.Property(t => t.KomisyonGelirleriMuhasebeKodu).HasColumnName("KomisyonGelirleriMuhasebeKodu");
            this.Property(t => t.WebAdresi).HasColumnName("WebAdresi");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.PostaKodu).HasColumnName("PostaKodu");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.UyariNotu).HasColumnName("UyariNotu");
            this.Property(t => t.BilgiNotu).HasColumnName("BilgiNotu");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.SatisIadeleriMuhasebeKodu).HasColumnName("SatisIadeleriMuhasebeKodu");
        }
    }
}

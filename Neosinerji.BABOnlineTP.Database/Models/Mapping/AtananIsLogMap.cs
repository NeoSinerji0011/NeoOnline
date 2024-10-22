using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AtananIsLogMap : EntityTypeConfiguration<AtananIsLog>
    {
        public AtananIsLogMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LogId });

            this.Property(t => t.LogDosyasiURL)
             .IsRequired()
             .HasMaxLength(int.MaxValue);

            this.Property(t => t.LogDosyasiAdi)
           .IsRequired()
           .HasMaxLength(200);


            this.ToTable("AtananIsLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.LogDosyasiAdi).HasColumnName("LogDosyasiAdi");
            this.Property(t => t.LogDosyasiURL).HasColumnName("LogDosyasiURL");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

        }
    }
}
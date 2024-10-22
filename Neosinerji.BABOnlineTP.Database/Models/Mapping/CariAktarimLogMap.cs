using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CariAktarimLogMap : EntityTypeConfiguration<CariAktarimLog>
    {
        public CariAktarimLogMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id });

            // Properties
            this.Property(t => t.TvmUnvan)
                .HasMaxLength(100);

            this.Property(t => t.PoliceNo)
                 .HasMaxLength(20);

            this.Property(t => t.YenilemeNo)
                 .HasMaxLength(20);

            this.Property(t => t.EkNo)
                 .HasMaxLength(20);

            this.Property(t => t.CariHesapKodu)
                 .HasMaxLength(20);

            this.Property(t => t.CariHesapUnvani)
                 .HasMaxLength(250);

            this.Property(t => t.PoliceNo)
                 .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("CariAktarimLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.TvmUnvan).HasColumnName("TvmUnvan");
            this.Property(t => t.Donem).HasColumnName("Donem");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.EkNo).HasColumnName("EkNo");
            this.Property(t => t.CariHesapKodu).HasColumnName("CariHesapKodu");
            this.Property(t => t.CariHesapUnvani).HasColumnName("CariHesapUnvani");
            this.Property(t => t.Basarili).HasColumnName("Basarili");
            this.Property(t => t.Mesaj).HasColumnName("Mesaj");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
        }

       
    }
}

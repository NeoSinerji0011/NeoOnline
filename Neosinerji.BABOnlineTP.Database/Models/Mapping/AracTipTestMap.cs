using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracTipTestMap : EntityTypeConfiguration<AracTipTest>
    {
        public AracTipTestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MarkaKodu)
                .HasMaxLength(10);

            this.Property(t => t.TipKodu)
                .HasMaxLength(10);

            this.Property(t => t.TipAdi)
                .HasMaxLength(100);

            this.Property(t => t.KullanimSekli1)
                .HasMaxLength(20);

            this.Property(t => t.KullanimSekli2)
                .HasMaxLength(20);

            this.Property(t => t.KullanimSekli3)
                .HasMaxLength(20);

            this.Property(t => t.KullanimSekli4)
                .HasMaxLength(20);

            this.Property(t => t.KisiSayisi)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("AracTipTest");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.KullanimSekli1).HasColumnName("KullanimSekli1");
            this.Property(t => t.KullanimSekli2).HasColumnName("KullanimSekli2");
            this.Property(t => t.KullanimSekli3).HasColumnName("KullanimSekli3");
            this.Property(t => t.KullanimSekli4).HasColumnName("KullanimSekli4");
            this.Property(t => t.KisiSayisi).HasColumnName("KisiSayisi");
        }
    }
}

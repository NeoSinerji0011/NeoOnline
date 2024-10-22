using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracTipMap : EntityTypeConfiguration<AracTip>
    {
        public AracTipMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MarkaKodu, t.TipKodu });

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TipKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TipAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.KullanimSekli1)
                .HasMaxLength(5);

            this.Property(t => t.KullanimSekli2)
                .HasMaxLength(5);

            this.Property(t => t.KullanimSekli3)
                .HasMaxLength(5);

            this.Property(t => t.KullanimSekli4)
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("AracTip");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.KullanimSekli1).HasColumnName("KullanimSekli1");
            this.Property(t => t.KullanimSekli2).HasColumnName("KullanimSekli2");
            this.Property(t => t.KullanimSekli3).HasColumnName("KullanimSekli3");
            this.Property(t => t.KullanimSekli4).HasColumnName("KullanimSekli4");
            this.Property(t => t.KisiSayisi).HasColumnName("KisiSayisi");

            // Relationships
            this.HasRequired(t => t.AracMarka)
                .WithMany(t => t.AracTips)
                .HasForeignKey(d => d.MarkaKodu);

        }
    }
}

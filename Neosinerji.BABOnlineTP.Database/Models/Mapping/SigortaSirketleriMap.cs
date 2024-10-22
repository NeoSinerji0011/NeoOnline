using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SigortaSirketleriMap : EntityTypeConfiguration<SigortaSirketleri>
    {
        public SigortaSirketleriMap()
        {
            // Primary Key
            this.HasKey(t => t.SirketKodu);

            // Properties
            this.Property(t => t.SirketKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.SirketAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.VergiDairesi)
              .IsRequired()
              .HasMaxLength(100);

            this.Property(t => t.VergiNumarasi)
              .IsRequired()
              .HasMaxLength(50);

            this.Property(t => t.SirketLogo)
              .IsRequired()
              .HasMaxLength(300);

            this.Property(t => t.Telefon)             
              .HasMaxLength(20);

            this.Property(t => t.Email)             
              .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("SigortaSirketleri");
            this.Property(t => t.SirketKodu).HasColumnName("SirketKodu");
            this.Property(t => t.SirketAdi).HasColumnName("SirketAdi");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.VergiNumarasi).HasColumnName("VergiNumarasi");
            this.Property(t => t.SirketLogo).HasColumnName("SirketLogo");
            this.Property(t => t.UygulamaKodu).HasColumnName("UygulamaKodu");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Email).HasColumnName("Email");
        }
    }
}

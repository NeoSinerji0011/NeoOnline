using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_AracGrupMap : EntityTypeConfiguration<CR_AracGrup>
    {
        public CR_AracGrupMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.KullanimTarziKodu, t.Kod2 });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KullanimTarziKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Kod2)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TarifeKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.KullanimTarziTarife)
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("CR_AracGrup");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.TarifeKodu).HasColumnName("TarifeKodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.KullanimTarziTarife).HasColumnName("KullanimTarziTarife");
        }
    }
}

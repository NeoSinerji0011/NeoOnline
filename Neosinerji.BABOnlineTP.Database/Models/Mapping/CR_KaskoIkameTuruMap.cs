using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_KaskoIkameTuruMap : EntityTypeConfiguration<CR_KaskoIkameTuru>
    {
        public CR_KaskoIkameTuruMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.TarifeKodu, t.IkameTuruKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TarifeKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.IkameTuruKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.IkameTuru)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CR_KaskoIkameTuru");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TarifeKodu).HasColumnName("TarifeKodu");
            this.Property(t => t.IkameTuruKodu).HasColumnName("IkameTuruKodu");
            this.Property(t => t.IkameTuru).HasColumnName("IkameTuru");
        }
    }
}

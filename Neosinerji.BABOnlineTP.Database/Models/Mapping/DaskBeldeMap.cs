using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DaskBeldeMap : EntityTypeConfiguration<DaskBelde>
    {
        public DaskBeldeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BeldeKodu, t.IlKodu, t.IlceKodu });

            // Properties
            this.Property(t => t.BeldeKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlceKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BeldeAdi)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("DaskBelde");
            this.Property(t => t.BeldeKodu).HasColumnName("BeldeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.BeldeAdi).HasColumnName("BeldeAdi");

            // Relationships
            this.HasRequired(t => t.DaskIl)
                .WithMany(t => t.DaskBeldes)
                .HasForeignKey(d => d.IlKodu);

        }
    }
}

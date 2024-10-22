using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DaskIlceMap : EntityTypeConfiguration<DaskIlce>
    {
        public DaskIlceMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IlceKodu, t.IlKodu });

            // Properties
            this.Property(t => t.IlceKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlceAdi)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("DaskIlce");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");

            // Relationships
            this.HasRequired(t => t.DaskIl)
                .WithMany(t => t.DaskIlces)
                .HasForeignKey(d => d.IlKodu);

        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipDokumanMap : EntityTypeConfiguration<IsTakipDokuman>
    {
        public IsTakipDokumanMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipDokumanId);

            // Properties
            this.Property(t => t.DokumanURL)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("IsTakipDokuman");
            this.Property(t => t.IsTakipDokumanId).HasColumnName("IsTakipDokumanId");
            this.Property(t => t.IsTakipId).HasColumnName("IsTakipId");
            this.Property(t => t.DokumanTipi).HasColumnName("DokumanTipi");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.KaydedenKullanici).HasColumnName("KaydedenKullanici");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.IsTakip)
                .WithMany(t => t.IsTakipDokumen)
                .HasForeignKey(d => d.IsTakipId);

        }
    }
}

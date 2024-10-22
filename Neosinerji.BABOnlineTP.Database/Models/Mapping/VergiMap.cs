using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class VergiMap : EntityTypeConfiguration<Vergi>
    {
        public VergiMap()
        {
            // Primary Key
            this.HasKey(t => t.VergiKodu);

            // Properties
            this.Property(t => t.VergiKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VergiAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Vergi");
            this.Property(t => t.VergiKodu).HasColumnName("VergiKodu");
            this.Property(t => t.VergiAdi).HasColumnName("VergiAdi");
        }
    }
}

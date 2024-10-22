using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IstigalMap : EntityTypeConfiguration<Istigal>
    {
        public IstigalMap()
        {
            // Primary Key
            this.HasKey(t => t.Kod);

            // Properties
            this.Property(t => t.Kod)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(70);

            // Table & Column Mappings
            this.ToTable("Istigal");
            this.Property(t => t.Kod).HasColumnName("Kod");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}

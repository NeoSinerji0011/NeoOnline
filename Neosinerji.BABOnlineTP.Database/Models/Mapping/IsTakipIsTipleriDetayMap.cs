using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipIsTipleriDetayMap : EntityTypeConfiguration<IsTakipIsTipleriDetay>
    {
        public IsTakipIsTipleriDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipIsTipleriDetayId);

            // Properties
            this.Property(t => t.IsTakipIsTipleriDetayId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IsTipiDetay)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("IsTakipIsTipleriDetay");
            this.Property(t => t.IsTakipIsTipleriDetayId).HasColumnName("IsTakipIsTipleriDetayId");
            this.Property(t => t.IsTakipIsTipiId).HasColumnName("IsTakipIsTipiId");
            this.Property(t => t.IsTipiDetay).HasColumnName("IsTipiDetay");
        }
    }
}

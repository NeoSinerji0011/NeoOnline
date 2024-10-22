using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipIsTipleriMap : EntityTypeConfiguration<IsTakipIsTipleri>
    {
        public IsTakipIsTipleriMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipIsTipleriId);

            // Properties
            this.Property(t => t.IsTakipIsTipleriId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IsTipi)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("IsTakipIsTipleri");
            this.Property(t => t.IsTakipIsTipleriId).HasColumnName("IsTakipIsTipleriId");
            this.Property(t => t.IsTipi).HasColumnName("IsTipi");
        }
    }
}

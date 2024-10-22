using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class BelediyeIlMap : EntityTypeConfiguration<BelediyeIl>
    {
        public BelediyeIlMap()
        {
            // Primary Key
            this.HasKey(t => t.IlKodu);

            // Properties
            this.Property(t => t.IlKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Adi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("BelediyeIl");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.Adi).HasColumnName("Adi");
        }
    }
}

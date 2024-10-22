using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DaskIlMap : EntityTypeConfiguration<DaskIl>
    {
        public DaskIlMap()
        {
            // Primary Key
            this.HasKey(t => t.IlKodu);

            // Properties
            this.Property(t => t.IlKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlAdi)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("DaskIl");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
        }
    }
}

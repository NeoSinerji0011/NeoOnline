using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MeslekMap : EntityTypeConfiguration<Meslek>
    {
        public MeslekMap()
        {
            // Primary Key
            this.HasKey(t => t.MeslekKodu);

            // Properties
            this.Property(t => t.MeslekKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MeslekAdi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Meslek");
            this.Property(t => t.MeslekKodu).HasColumnName("MeslekKodu");
            this.Property(t => t.MeslekAdi).HasColumnName("MeslekAdi");
        }
    }
}

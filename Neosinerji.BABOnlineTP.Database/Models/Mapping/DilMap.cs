using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DilMap : EntityTypeConfiguration<Dil>
    {
        public DilMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DilAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DilKodu)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Dil");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DilAdi).HasColumnName("DilAdi");
            this.Property(t => t.DilKodu).HasColumnName("DilKodu");
        }
    }
}

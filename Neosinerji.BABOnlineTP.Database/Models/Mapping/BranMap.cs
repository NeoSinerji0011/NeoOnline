using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class BranMap : EntityTypeConfiguration<Bran>
    {
        public BranMap()
        {
            // Primary Key
            this.HasKey(t => t.BransKodu);

            // Properties
            this.Property(t => t.BransKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BransAdi)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Brans");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.BransAdi).HasColumnName("BransAdi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}

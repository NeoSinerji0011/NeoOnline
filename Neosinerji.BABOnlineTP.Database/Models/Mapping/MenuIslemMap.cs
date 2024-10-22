using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MenuIslemMap : EntityTypeConfiguration<MenuIslem>
    {
        public MenuIslemMap()
        {
            // Primary Key
            this.HasKey(t => t.IslemKodu);

            // Properties
            this.Property(t => t.IslemKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IslemId)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.URL)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Icon)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("MenuIslem");
            this.Property(t => t.IslemKodu).HasColumnName("IslemKodu");
            this.Property(t => t.IslemId).HasColumnName("IslemId");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.Icon).HasColumnName("Icon");
        }
    }
}

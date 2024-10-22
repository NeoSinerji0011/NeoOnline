using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SchengenUlkeOranlariMap : EntityTypeConfiguration<SchengenUlkeOranlari>
    {
        public SchengenUlkeOranlariMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Yas1, t.Yas2, t.Gun1, t.Gun2 });

            // Properties
            this.Property(t => t.Yas1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Yas2)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Gun1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Gun2)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("SchengenUlkeOranlari");
            this.Property(t => t.Yas1).HasColumnName("Yas1");
            this.Property(t => t.Yas2).HasColumnName("Yas2");
            this.Property(t => t.Gun1).HasColumnName("Gun1");
            this.Property(t => t.Gun2).HasColumnName("Gun2");
            this.Property(t => t.Oran).HasColumnName("Oran");
            this.Property(t => t.SuprimOrani).HasColumnName("SuprimOrani");
        }
    }
}

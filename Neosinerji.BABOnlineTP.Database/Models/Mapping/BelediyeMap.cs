using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class BelediyeMap : EntityTypeConfiguration<Belediye>
    {
        public BelediyeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IlKodu, t.BelediyeKodu });

            // Properties
            this.Property(t => t.IlKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BelediyeKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BelediyeAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Belediye");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.BelediyeKodu).HasColumnName("BelediyeKodu");
            this.Property(t => t.BelediyeAdi).HasColumnName("BelediyeAdi");

            // Relationships
            this.HasRequired(t => t.BelediyeIl)
                .WithMany(t => t.Belediyes)
                .HasForeignKey(d => d.IlKodu);

        }
    }
}

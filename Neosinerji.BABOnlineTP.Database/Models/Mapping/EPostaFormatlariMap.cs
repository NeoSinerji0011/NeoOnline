using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class EPostaFormatlariMap : EntityTypeConfiguration<EPostaFormatlari>
    {
        public EPostaFormatlariMap()
        {
            // Primary Key
            this.HasKey(t => t.FormatId);

            // Properties
            this.Property(t => t.FormatAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Icerik)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("EPostaFormatlari");
            this.Property(t => t.FormatId).HasColumnName("FormatId");
            this.Property(t => t.FormatAdi).HasColumnName("FormatAdi");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.Icerik).HasColumnName("Icerik");
        }
    }
}

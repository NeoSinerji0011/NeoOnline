using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TrafikFKMap : EntityTypeConfiguration<TrafikFK>
    {
        public TrafikFKMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.KullanimTarziKodu)
                .HasMaxLength(5);

            this.Property(t => t.Kod2)
                .HasMaxLength(5);

            this.Property(t => t.Text)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("TrafikFK");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.Vefat).HasColumnName("Vefat");
            this.Property(t => t.Sakatlik).HasColumnName("Sakatlik");
            this.Property(t => t.Tedavi).HasColumnName("Tedavi");
            this.Property(t => t.Kombine).HasColumnName("Kombine");
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}

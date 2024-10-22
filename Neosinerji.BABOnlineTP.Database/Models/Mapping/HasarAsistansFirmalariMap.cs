using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarAsistansFirmalariMap : EntityTypeConfiguration<HasarAsistansFirmalari>
    {
        public HasarAsistansFirmalariMap()
        {
            // Primary Key
            this.HasKey(t => t.AsistansKodu);

            // Properties
            this.Property(t => t.AsistansAdUnvan)
                .HasMaxLength(255);

            this.Property(t => t.AsistansSoyadUnvan)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("HasarAsistansFirmalari");
            this.Property(t => t.AsistansKodu).HasColumnName("AsistansKodu");
            this.Property(t => t.AsistansAdUnvan).HasColumnName("AsistansAdUnvan");
            this.Property(t => t.AsistansSoyadUnvan).HasColumnName("AsistansSoyadUnvan");
            this.Property(t => t.Durumu).HasColumnName("Durumu");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarAnlasmaliServislerMap : EntityTypeConfiguration<HasarAnlasmaliServisler>
    {
        public HasarAnlasmaliServislerMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Unvani)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("HasarAnlasmaliServisler");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Unvani).HasColumnName("Unvani");
            this.Property(t => t.Durumu).HasColumnName("Durumu");
        }
    }
}

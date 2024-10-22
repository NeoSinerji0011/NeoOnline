using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarIletisimYetkilileriMap : EntityTypeConfiguration<HasarIletisimYetkilileri>
    {
        public HasarIletisimYetkilileriMap()
        {
            // Primary Key
            this.HasKey(t => t.IletisimYetkiliId);

            // Properties
            this.Property(t => t.GorusulenKisi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Gorevi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TelefonNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HasarIletisimYetkilileri");
            this.Property(t => t.IletisimYetkiliId).HasColumnName("IletisimYetkiliId");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.GorusulenKisi).HasColumnName("GorusulenKisi");
            this.Property(t => t.Gorevi).HasColumnName("Gorevi");
            this.Property(t => t.TelefonTipi).HasColumnName("TelefonTipi");
            this.Property(t => t.TelefonNo).HasColumnName("TelefonNo");
            this.Property(t => t.Email).HasColumnName("Email");

            // Relationships
            this.HasRequired(t => t.HasarGenelBilgiler)
                .WithMany(t => t.HasarIletisimYetkilileris)
                .HasForeignKey(d => d.HasarId);

        }
    }
}

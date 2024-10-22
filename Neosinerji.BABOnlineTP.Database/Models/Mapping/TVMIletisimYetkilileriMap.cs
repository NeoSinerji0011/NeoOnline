using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMIletisimYetkilileriMap : EntityTypeConfiguration<TVMIletisimYetkilileri>
    {
        public TVMIletisimYetkilileriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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
            this.ToTable("TVMIletisimYetkilileri");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.GorusulenKisi).HasColumnName("GorusulenKisi");
            this.Property(t => t.Gorevi).HasColumnName("Gorevi");
            this.Property(t => t.TelefonTipi).HasColumnName("TelefonTipi");
            this.Property(t => t.TelefonNo).HasColumnName("TelefonNo");
            this.Property(t => t.Email).HasColumnName("Email");
        }
    }
}

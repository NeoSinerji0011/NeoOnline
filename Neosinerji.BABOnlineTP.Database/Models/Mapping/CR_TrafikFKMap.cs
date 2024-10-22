using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_TrafikFKMap : EntityTypeConfiguration<CR_TrafikFK>
    {
        public CR_TrafikFKMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TumKodu, t.KullanimTarziKodu, t.Kod2, t.Kademe });

            // Properties
            this.Property(t => t.TumKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KullanimTarziKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Kod2)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Kademe)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Text)
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("CR_TrafikFK");
            this.Property(t => t.TumKodu).HasColumnName("TumKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.Kademe).HasColumnName("Kademe");
            this.Property(t => t.Vefat).HasColumnName("Vefat");
            this.Property(t => t.Sakatlik).HasColumnName("Sakatlik");
            this.Property(t => t.Tedavi).HasColumnName("Tedavi");
            this.Property(t => t.Kombine).HasColumnName("Kombine");
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}

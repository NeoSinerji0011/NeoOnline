using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_TrafikIMMMap : EntityTypeConfiguration<CR_TrafikIMM>
    {
        public CR_TrafikIMMMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.KullanimTarziKodu, t.Kod2, t.Kademe });

            // Properties
            this.Property(t => t.TUMKodu)
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
            this.ToTable("CR_TrafikIMM");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.Kademe).HasColumnName("Kademe");
            this.Property(t => t.BedeniSahis).HasColumnName("BedeniSahis");
            this.Property(t => t.BedeniKaza).HasColumnName("BedeniKaza");
            this.Property(t => t.Maddi).HasColumnName("Maddi");
            this.Property(t => t.Kombine).HasColumnName("Kombine");
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KaskoIMMMap : EntityTypeConfiguration<KaskoIMM>
    {
        public KaskoIMMMap()
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
            this.ToTable("KaskoIMM");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.BedeniSahis).HasColumnName("BedeniSahis");
            this.Property(t => t.BedeniKaza).HasColumnName("BedeniKaza");
            this.Property(t => t.Maddi).HasColumnName("Maddi");
            this.Property(t => t.Kombine).HasColumnName("Kombine");
            this.Property(t => t.Text).HasColumnName("Text");
        }
    }
}

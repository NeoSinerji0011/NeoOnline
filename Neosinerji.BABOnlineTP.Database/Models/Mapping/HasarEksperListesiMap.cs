using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarEksperListesiMap : EntityTypeConfiguration<HasarEksperListesi>
    {
        public HasarEksperListesiMap()
        {
            // Primary Key
            this.HasKey(t => t.EksperKodu);

            // Properties
            this.Property(t => t.EksperAdSoyadUnvan)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CepTel)
                .HasMaxLength(50);

            this.Property(t => t.IsTel)
                .HasMaxLength(50);

            this.Property(t => t.Fax)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("HasarEksperListesi");
            this.Property(t => t.EksperKodu).HasColumnName("EksperKodu");
            this.Property(t => t.EksperAdSoyadUnvan).HasColumnName("EksperAdSoyadUnvan");
            this.Property(t => t.CepTel).HasColumnName("CepTel");
            this.Property(t => t.IsTel).HasColumnName("IsTel");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Durumu).HasColumnName("Durumu");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceDokumanMap : EntityTypeConfiguration<PoliceDokuman>
    {
        public PoliceDokumanMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DokumanURL)
                .HasMaxLength(255);
            this.Property(t => t.DokumanAdi)
                .HasMaxLength(255);
            this.Property(t => t.TVMKullaniciKodu)
                .HasMaxLength(255);


            // Table & Column Mappings
            this.ToTable("PoliceDokuman");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Police_ID).HasColumnName("Police_ID");
            this.Property(t => t.DokumanAdi).HasColumnName("DokumanAdi");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");

        }
    }
}

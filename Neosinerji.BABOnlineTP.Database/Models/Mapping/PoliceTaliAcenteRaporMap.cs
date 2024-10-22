using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceTaliAcenteRaporMap : EntityTypeConfiguration<PoliceTaliAcenteRapor>
    {
        public PoliceTaliAcenteRaporMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("PoliceTaliAcenteRapor");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.UretimVAR_YOK).HasColumnName("UretimVAR/YOK");
            this.Property(t => t.Police_EkAdedi).HasColumnName("Police/EkAdedi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.GunKapamaDurumu).HasColumnName("GunKapamaDurumu");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceTaliAcentelerMap : EntityTypeConfiguration<PoliceTaliAcenteler>
    {
        public PoliceTaliAcentelerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.KimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.AdUnvan_)
                .HasMaxLength(150);

            this.Property(t => t.SoyadUnvan)
                .HasMaxLength(150);

            this.Property(t => t.PoliceNo)
                .HasMaxLength(50);

            this.Property(t => t.SigortaSirketNo_)
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("PoliceTaliAcenteler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.AdUnvan_).HasColumnName("AdUnvan ");
            this.Property(t => t.SoyadUnvan).HasColumnName("SoyadUnvan");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.EkNo).HasColumnName("EkNo");
            this.Property(t => t.SigortaSirketNo_).HasColumnName("SigortaSirketNo ");
            this.Property(t => t.KayitTarihi_).HasColumnName("KayitTarihi ");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.PoliceTransferEslestimi).HasColumnName("PoliceTransferEslestimi");
        }
    }
}

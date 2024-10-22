using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class OfflinePolouseMap : EntityTypeConfiguration<OfflinePolouse>
    {
        public OfflinePolouseMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SEKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.SETVMMusteriKodu)
                .HasMaxLength(50);

            this.Property(t => t.SKimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.STVMMusteriKodu)
                .HasMaxLength(50);

            this.Property(t => t.PoliceNo)
                .HasMaxLength(50);

            this.Property(t => t.YenilemeNo)
                .HasMaxLength(50);

            this.Property(t => t.ZeyileNo)
                .HasMaxLength(50);

            this.Property(t => t.KrediKartiBankaAdi)
                .HasMaxLength(100);

            this.Property(t => t.SatisTemsilcisiTCKN)
                .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("OfflinePolice");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.SEKimlikNo).HasColumnName("SEKimlikNo");
            this.Property(t => t.SETVMMusteriKodu).HasColumnName("SETVMMusteriKodu");
            this.Property(t => t.SKimlikNo).HasColumnName("SKimlikNo");
            this.Property(t => t.STVMMusteriKodu).HasColumnName("STVMMusteriKodu");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.ZeyileNo).HasColumnName("ZeyileNo");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.Komisyon).HasColumnName("Komisyon");
            this.Property(t => t.PoliceBaslangicTarihi).HasColumnName("PoliceBaslangicTarihi");
            this.Property(t => t.PoliceBitisTarihi).HasColumnName("PoliceBitisTarihi");
            this.Property(t => t.TanzimTarihi).HasColumnName("TanzimTarihi");
            this.Property(t => t.TaksitAdedi).HasColumnName("TaksitAdedi");
            this.Property(t => t.OdemeSekli).HasColumnName("OdemeSekli");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
            this.Property(t => t.KrediKartiBankaAdi).HasColumnName("KrediKartiBankaAdi");
            this.Property(t => t.SatisTuru).HasColumnName("SatisTuru");
            this.Property(t => t.SatisTemsilcisiTCKN).HasColumnName("SatisTemsilcisiTCKN");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");

            // Relationships
            this.HasOptional(t => t.TVMKullanicilar)
                .WithMany(t => t.OfflinePolice)
                .HasForeignKey(d => d.CreatedBy);

        }
    }
}

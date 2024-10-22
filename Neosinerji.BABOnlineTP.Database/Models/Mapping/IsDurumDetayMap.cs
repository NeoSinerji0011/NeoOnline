using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsDurumDetayMap : EntityTypeConfiguration<IsDurumDetay>
    {
        public IsDurumDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.IsDetayId);

            // Properties
            // Table & Column Mappings
            this.ToTable("IsDurumDetay");
            this.Property(t => t.IsDetayId).HasColumnName("IsDetayId");
            this.Property(t => t.IsId).HasColumnName("IsId");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.OdemePlaniAlternatifKodu).HasColumnName("OdemePlaniAlternatifKodu");
            this.Property(t => t.Durumu).HasColumnName("Durumu");
            this.Property(t => t.ReferansId).HasColumnName("ReferansId");
            this.Property(t => t.HataMesaji).HasColumnName("HataMesaji");
            this.Property(t => t.BilgiMesaji).HasColumnName("BilgiMesaji");
            this.Property(t => t.Baslangic).HasColumnName("Baslangic");
            this.Property(t => t.Bitis).HasColumnName("Bitis");

            // Relationships
            this.HasRequired(t => t.IsDurum)
                .WithMany(t => t.IsDurumDetays)
                .HasForeignKey(d => d.IsId);

        }
    }
}

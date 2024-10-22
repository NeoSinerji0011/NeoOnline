using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SaglikTeminatPrimleriMap : EntityTypeConfiguration<SaglikTeminatPrimleri>
    {
        public SaglikTeminatPrimleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TeminatTipKodu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BolgeKodu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.YasAraligi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SaglikTeminatPrimleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.TeminatTipKodu).HasColumnName("TeminatTipKodu");
            this.Property(t => t.BolgeKodu).HasColumnName("BolgeKodu");
            this.Property(t => t.PlanKodu).HasColumnName("PlanKodu");
            this.Property(t => t.YasAraligi).HasColumnName("YasAraligi");
            this.Property(t => t.Prim).HasColumnName("Prim");
            this.Property(t => t.PrimGecerlilikBaslangicTarihi).HasColumnName("PrimGecerlilikBaslangicTarihi");
            this.Property(t => t.PrimGecerlilikBitisTarihi).HasColumnName("PrimGecerlilikBitisTarihi");

            // Relationships
            this.HasRequired(t => t.SaglikBolgeleri)
                .WithMany(t => t.SaglikTeminatPrimleris)
                .HasForeignKey(d => d.BolgeKodu);
            this.HasRequired(t => t.SaglikTeminatTipleri)
                .WithMany(t => t.SaglikTeminatPrimleris)
                .HasForeignKey(d => d.TeminatTipKodu);

        }
    }
}

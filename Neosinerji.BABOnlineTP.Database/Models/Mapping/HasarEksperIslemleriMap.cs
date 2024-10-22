using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarEksperIslemleriMap : EntityTypeConfiguration<HasarEksperIslemleri>
    {
        public HasarEksperIslemleriMap()
        {
            // Primary Key
            this.HasKey(t => t.EksperId);

            // Properties
            this.Property(t => t.AsistansFirma)
                .HasMaxLength(255);

            this.Property(t => t.GelisSaati)
                .HasMaxLength(255);

            this.Property(t => t.BeklemeSuresi)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("HasarEksperIslemleri");
            this.Property(t => t.EksperId).HasColumnName("EksperId");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.EksperSorumlusuKodu).HasColumnName("EksperSorumlusuKodu");
            this.Property(t => t.TahminiHasarBedeli).HasColumnName("TahminiHasarBedeli");
            this.Property(t => t.TahminiHasarParaBirimi).HasColumnName("TahminiHasarParaBirimi");
            this.Property(t => t.AnlasmaliServisBedeli).HasColumnName("AnlasmaliServisBedeli");
            this.Property(t => t.AnlasmaliServisParaBirimi).HasColumnName("AnlasmaliServisParaBirimi");
            this.Property(t => t.TahakkukBedeli).HasColumnName("TahakkukBedeli");
            this.Property(t => t.TahakkukParaBirimi).HasColumnName("TahakkukParaBirimi");
            this.Property(t => t.RucuBedeli).HasColumnName("RucuBedeli");
            this.Property(t => t.RucuBedeliParaBirimi).HasColumnName("RucuBedeliParaBirimi");
            this.Property(t => t.RedBedeli).HasColumnName("RedBedeli");
            this.Property(t => t.RedParaBirimi).HasColumnName("RedParaBirimi");
            this.Property(t => t.AsistansFirma).HasColumnName("AsistansFirma");
            this.Property(t => t.AsistansFirmaBedeli).HasColumnName("AsistansFirmaBedeli");
            this.Property(t => t.AsistansFirmaParaBirimi).HasColumnName("AsistansFirmaParaBirimi");
            this.Property(t => t.OdemeTarihi).HasColumnName("OdemeTarihi");
            this.Property(t => t.BildirimTarihi).HasColumnName("BildirimTarihi");
            this.Property(t => t.GelisSaati).HasColumnName("GelisSaati");
            this.Property(t => t.BeklemeSuresi).HasColumnName("BeklemeSuresi");

            // Relationships
            this.HasRequired(t => t.HasarGenelBilgiler)
                .WithMany(t => t.HasarEksperIslemleris)
                .HasForeignKey(d => d.HasarId);

        }
    }
}

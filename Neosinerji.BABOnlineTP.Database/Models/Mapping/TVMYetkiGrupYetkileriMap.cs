using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMYetkiGrupYetkileriMap : EntityTypeConfiguration<TVMYetkiGrupYetkileri>
    {
        public TVMYetkiGrupYetkileriMap()
        {
            // Primary Key
            this.HasKey(t => t.YetkiId);

            // Properties
            // Table & Column Mappings
            this.ToTable("TVMYetkiGrupYetkileri");
            this.Property(t => t.YetkiId).HasColumnName("YetkiId");
            this.Property(t => t.YetkiGrupKodu).HasColumnName("YetkiGrupKodu");
            this.Property(t => t.AnaMenuKodu).HasColumnName("AnaMenuKodu");
            this.Property(t => t.AltMenuKodu).HasColumnName("AltMenuKodu");
            this.Property(t => t.SekmeKodu).HasColumnName("SekmeKodu");
            this.Property(t => t.Gorme).HasColumnName("Gorme");
            this.Property(t => t.YeniKayit).HasColumnName("YeniKayit");
            this.Property(t => t.Degistirme).HasColumnName("Degistirme");
            this.Property(t => t.Silme).HasColumnName("Silme");

            // Relationships
            this.HasRequired(t => t.TVMYetkiGruplari)
                .WithMany(t => t.TVMYetkiGrupYetkileris)
                .HasForeignKey(d => d.YetkiGrupKodu);

        }
    }
}

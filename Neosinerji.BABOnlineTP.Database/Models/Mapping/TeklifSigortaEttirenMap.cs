using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifSigortaEttirenMap : EntityTypeConfiguration<TeklifSigortaEttiren>
    {
        public TeklifSigortaEttirenMap()
        {
            // Primary Key
            this.HasKey(t => t.SigortaEttirenId);

            // Properties
            // Table & Column Mappings
            this.ToTable("TeklifSigortaEttiren");
            this.Property(t => t.SigortaEttirenId).HasColumnName("SigortaEttirenId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");

            // Relationships
            this.HasRequired(t => t.MusteriGenelBilgiler)
                .WithMany(t => t.TeklifSigortaEttirens)
                .HasForeignKey(d => d.MusteriKodu);
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifSigortaEttirens)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}

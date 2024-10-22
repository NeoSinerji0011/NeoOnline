using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifTeminatMap : EntityTypeConfiguration<TeklifTeminat>
    {
        public TeklifTeminatMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeklifId, t.TeminatKodu });

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TeminatKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TeklifTeminat");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TeminatKodu).HasColumnName("TeminatKodu");
            this.Property(t => t.TeminatBedeli).HasColumnName("TeminatBedeli");
            this.Property(t => t.TeminatVergi).HasColumnName("TeminatVergi");
            this.Property(t => t.TeminatNetPrim).HasColumnName("TeminatNetPrim");
            this.Property(t => t.TeminatBrutPrim).HasColumnName("TeminatBrutPrim");
            this.Property(t => t.Komisyon_).HasColumnName("Komisyon ");
            this.Property(t => t.Adet).HasColumnName("Adet");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifTeminats)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}

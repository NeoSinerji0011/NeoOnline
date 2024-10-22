using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DilAciklamaMap : EntityTypeConfiguration<DilAciklama>
    {
        public DilAciklamaMap()
        {
            // Primary Key
            this.HasKey(t => new { t.DilId, t.TabloAdi, t.TabloId, t.TabloId_2, t.AnaMenuKodu });

            // Properties
            this.Property(t => t.DilId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TabloAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TabloId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TabloId_2)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AnaMenuKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DilAciklama_1)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.DilAciklama_2)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DilAciklama");
            this.Property(t => t.DilId).HasColumnName("DilId");
            this.Property(t => t.TabloAdi).HasColumnName("TabloAdi");
            this.Property(t => t.TabloId).HasColumnName("TabloId");
            this.Property(t => t.TabloId_2).HasColumnName("TabloId_2");
            this.Property(t => t.AnaMenuKodu).HasColumnName("AnaMenuKodu");
            this.Property(t => t.DilAciklama_1).HasColumnName("DilAciklama_1");
            this.Property(t => t.DilAciklama_2).HasColumnName("DilAciklama_2");

            // Relationships
            this.HasRequired(t => t.Dil)
                .WithMany(t => t.DilAciklamas)
                .HasForeignKey(d => d.DilId);

        }
    }
}

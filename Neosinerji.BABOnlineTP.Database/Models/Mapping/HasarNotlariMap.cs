using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarNotlariMap : EntityTypeConfiguration<HasarNotlari>
    {
        public HasarNotlariMap()
        {
            // Primary Key
            this.HasKey(t => t.NotId);

            // Properties
            this.Property(t => t.NotKonu)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.NotAciklama)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("HasarNotlari");
            this.Property(t => t.NotId).HasColumnName("NotId");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.NotKonu).HasColumnName("NotKonu");
            this.Property(t => t.NotAciklama).HasColumnName("NotAciklama");
            this.Property(t => t.NotKayitTarihi).HasColumnName("NotKayitTarihi");

            // Relationships
            this.HasRequired(t => t.HasarGenelBilgiler)
                .WithMany(t => t.HasarNotlaris)
                .HasForeignKey(d => d.HasarId);

        }
    }
}

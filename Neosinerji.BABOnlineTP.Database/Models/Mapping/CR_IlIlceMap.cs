using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_IlIlceMap : EntityTypeConfiguration<CR_IlIlce>
    {
        public CR_IlIlceMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.IlKodu, t.IlceKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.IlceKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CRIlKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.CRIlceKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.CRIlAdi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.CRIlceAdi)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("CR_IlIlce");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.CRIlKodu).HasColumnName("CRIlKodu");
            this.Property(t => t.CRIlceKodu).HasColumnName("CRIlceKodu");
            this.Property(t => t.CRIlAdi).HasColumnName("CRIlAdi");
            this.Property(t => t.CRIlceAdi).HasColumnName("CRIlceAdi");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AltMenuSekmeMap : EntityTypeConfiguration<AltMenuSekme>
    {
        public AltMenuSekmeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AnaMenuKodu, t.AltMenuKodu, t.SekmeKodu });

            // Properties
            this.Property(t => t.AnaMenuKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AltMenuKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SekmeKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.YardimAciklama)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AltMenuSekme");
            this.Property(t => t.AnaMenuKodu).HasColumnName("AnaMenuKodu");
            this.Property(t => t.AltMenuKodu).HasColumnName("AltMenuKodu");
            this.Property(t => t.SekmeKodu).HasColumnName("SekmeKodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.YardimAciklama).HasColumnName("YardimAciklama");
            this.Property(t => t.SiraNumarasi).HasColumnName("SiraNumarasi");
            this.Property(t => t.IslemKodu).HasColumnName("IslemKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}

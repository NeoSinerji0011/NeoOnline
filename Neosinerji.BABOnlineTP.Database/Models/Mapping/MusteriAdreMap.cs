using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MusteriAdreMap : EntityTypeConfiguration<MusteriAdre>
    {
        public MusteriAdreMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.MusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UlkeKodu)
                .HasMaxLength(10);

            this.Property(t => t.IlKodu)
                .HasMaxLength(10);

            this.Property(t => t.Adres)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Semt)
                .HasMaxLength(50);

            this.Property(t => t.Mahalle)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Cadde)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Sokak)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Apartman)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BinaNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.DaireNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.HanAptFab)
                .HasMaxLength(50);

            this.Property(t => t.Diger)
                .HasMaxLength(100);

            this.Property(t => t.Latitude)
                .HasMaxLength(20);

            this.Property(t => t.Longitude)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("MusteriAdres");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.AdresTipi).HasColumnName("AdresTipi");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Semt).HasColumnName("Semt");
            this.Property(t => t.Mahalle).HasColumnName("Mahalle");
            this.Property(t => t.Cadde).HasColumnName("Cadde");
            this.Property(t => t.Sokak).HasColumnName("Sokak");
            this.Property(t => t.Apartman).HasColumnName("Apartman");
            this.Property(t => t.BinaNo).HasColumnName("BinaNo");
            this.Property(t => t.DaireNo).HasColumnName("DaireNo");
            this.Property(t => t.HanAptFab).HasColumnName("HanAptFab");
            this.Property(t => t.PostaKodu).HasColumnName("PostaKodu");
            this.Property(t => t.Diger).HasColumnName("Diger");
            this.Property(t => t.Varsayilan).HasColumnName("Varsayilan");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");

            // Relationships
            this.HasRequired(t => t.MusteriGenelBilgiler)
                .WithMany(t => t.MusteriAdres)
                .HasForeignKey(d => d.MusteriKodu);

        }
    }
}

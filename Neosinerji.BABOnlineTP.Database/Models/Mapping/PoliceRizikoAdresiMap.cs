using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceRizikoAdresiMap : EntityTypeConfiguration<PoliceRizikoAdresi>
    {
        public PoliceRizikoAdresiMap()
        {
            // Primary Key
            this.HasKey(t => t.PoliceId);

            // Properties
            this.Property(t => t.PoliceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Il)
                .HasMaxLength(100);

            this.Property(t => t.Ilce)
                .HasMaxLength(100);

            this.Property(t => t.SemtBeldeKodu)
                .HasMaxLength(50);

            this.Property(t => t.SemtBelde)
                .HasMaxLength(100);

            this.Property(t => t.MahalleKodu)
                .HasMaxLength(50);

            this.Property(t => t.Mahalle)
                .HasMaxLength(100);

            this.Property(t => t.CaddeKodu)
                .HasMaxLength(50);

            this.Property(t => t.Cadde)
                .HasMaxLength(100);

            this.Property(t => t.SokakKodu)
                .HasMaxLength(50);

            this.Property(t => t.Sokak)
                .HasMaxLength(100);

            this.Property(t => t.Apartman)
                .HasMaxLength(100);

            this.Property(t => t.BinaKodu)
                .HasMaxLength(50);

            this.Property(t => t.Bina)
                .HasMaxLength(50);

            this.Property(t => t.DaireKodu)
                .HasMaxLength(50);

            this.Property(t => t.Daire)
                .HasMaxLength(50);

            this.Property(t => t.HanAptFab)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(500);

            this.Property(t => t.UAVTKodu)
                .HasMaxLength(20);

            this.Property(t => t.Latitude)
                .HasMaxLength(50);

            this.Property(t => t.Longitude)
                .HasMaxLength(50);

            this.Property(t => t.BinaKullanimTarzi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PoliceRizikoAdresi");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.Il).HasColumnName("Il");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.Ilce).HasColumnName("Ilce");
            this.Property(t => t.SemtBeldeKodu).HasColumnName("SemtBeldeKodu");
            this.Property(t => t.SemtBelde).HasColumnName("SemtBelde");
            this.Property(t => t.MahalleKodu).HasColumnName("MahalleKodu");
            this.Property(t => t.Mahalle).HasColumnName("Mahalle");
            this.Property(t => t.CaddeKodu).HasColumnName("CaddeKodu");
            this.Property(t => t.Cadde).HasColumnName("Cadde");
            this.Property(t => t.SokakKodu).HasColumnName("SokakKodu");
            this.Property(t => t.Sokak).HasColumnName("Sokak");
            this.Property(t => t.Apartman).HasColumnName("Apartman");
            this.Property(t => t.BinaKodu).HasColumnName("BinaKodu");
            this.Property(t => t.Bina).HasColumnName("Bina");
            this.Property(t => t.DaireKodu).HasColumnName("DaireKodu");
            this.Property(t => t.Daire).HasColumnName("Daire");
            this.Property(t => t.HanAptFab).HasColumnName("HanAptFab");
            this.Property(t => t.PostaKodu).HasColumnName("PostaKodu");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.UAVTKodu).HasColumnName("UAVTKodu");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.BinaKullanimTarzi).HasColumnName("BinaKullanimTarzi");
            this.Property(t => t.BinaBedel).HasColumnName("BinaBedel");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithOptional(t => t.PoliceRizikoAdresi);

        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullaniciSifremiUnuttumMap : EntityTypeConfiguration<TVMKullaniciSifremiUnuttum>
    {
        public TVMKullaniciSifremiUnuttumMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PasswordVerificationToken_)
                .IsRequired();

            this.Property(t => t.EskiSifre)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("TVMKullaniciSifremiUnuttum");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.PasswordVerificationToken_).HasColumnName("PasswordVerificationToken ");
            this.Property(t => t.SendDate).HasColumnName("SendDate");
            this.Property(t => t.ResetDate).HasColumnName("ResetDate");
            this.Property(t => t.EskiSifre).HasColumnName("EskiSifre");
            this.Property(t => t.YeniSifre).HasColumnName("YeniSifre");
            this.Property(t => t.Status).HasColumnName("Status");

            // Relationships
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.TVMKullaniciSifremiUnuttums)
                .HasForeignKey(d => d.KullaniciKodu);

        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifProvizyonMap : EntityTypeConfiguration<TeklifProvizyon>
    {
        public TeklifProvizyonMap()
        {
            // Primary Key
            this.HasKey(t => t.ProvizyonId);

            // Properties
            this.Property(t => t.PartajNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.BasvuruNumarasi)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OdemeyiYapanTCKN)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.KrediKarti_IlkAltiSonDort)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ParaBirimi)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OnayKodu)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("TeklifProvizyon");
            this.Property(t => t.ProvizyonId).HasColumnName("ProvizyonId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.PartajNo).HasColumnName("PartajNo");
            this.Property(t => t.BasvuruNumarasi).HasColumnName("BasvuruNumarasi");
            this.Property(t => t.OdemeTuru).HasColumnName("OdemeTuru");
            this.Property(t => t.OdemeyiYapanTCKN).HasColumnName("OdemeyiYapanTCKN");
            this.Property(t => t.KrediKarti_IlkAltiSonDort).HasColumnName("KrediKarti_IlkAltiSonDort");
            this.Property(t => t.ParaBirimi).HasColumnName("ParaBirimi");
            this.Property(t => t.ProvizyonTarihi).HasColumnName("ProvizyonTarihi");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.KaydedenKullanici).HasColumnName("KaydedenKullanici");
            this.Property(t => t.OnayKodu).HasColumnName("OnayKodu");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifProvizyons)
                .HasForeignKey(d => d.TeklifId);
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.TeklifProvizyons)
                .HasForeignKey(d => d.KaydedenKullanici);

        }
    }
}

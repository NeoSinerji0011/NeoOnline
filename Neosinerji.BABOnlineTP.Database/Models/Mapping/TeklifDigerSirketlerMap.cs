using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifDigerSirketlerMap : EntityTypeConfiguration<TeklifDigerSirketler>
    {
        public TeklifDigerSirketlerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketTeklifNo)
                .HasMaxLength(50);

            this.Property(t => t.HasarsizlikIndirim)
                .HasMaxLength(50);

            this.Property(t => t.HasarsizlikSurprim)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TeklifDigerSirketler");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.SigortaSirketTeklifNo).HasColumnName("SigortaSirketTeklifNo");
            this.Property(t => t.HasarsizlikIndirim).HasColumnName("HasarsizlikIndirim");
            this.Property(t => t.HasarsizlikSurprim).HasColumnName("HasarsizlikSurprim");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.TaksitSayisi).HasColumnName("TaksitSayisi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.KayitEdenKullaniciKodu).HasColumnName("KayitEdenKullaniciKodu");
            this.Property(t => t.KomisyonTutari).HasColumnName("KomisyonTutari");
            //this.Property(t => t.TeklifPDF).HasColumnName("TeklifPDF");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifDigerSirketlers)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}

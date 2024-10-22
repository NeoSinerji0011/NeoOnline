using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class WEBServisLogMap : EntityTypeConfiguration<WEBServisLog>
    {
        public WEBServisLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.IstekUrl)
                .HasMaxLength(255);

            this.Property(t => t.CevapUrl)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("WEBServisLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.IstekTipi).HasColumnName("IstekTipi");
            this.Property(t => t.IstekTarihi).HasColumnName("IstekTarihi");
            this.Property(t => t.CevapTarihi).HasColumnName("CevapTarihi");
            this.Property(t => t.BasariliBasarisiz).HasColumnName("BasariliBasarisiz");
            this.Property(t => t.IstekUrl).HasColumnName("IstekUrl");
            this.Property(t => t.CevapUrl).HasColumnName("CevapUrl");

            // Relationships
            this.HasOptional(t => t.TeklifGenel)
                .WithMany(t => t.WEBServisLogs)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}

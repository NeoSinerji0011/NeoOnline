using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifEMailLogMap : EntityTypeConfiguration<TeklifEMailLog>
    {
        public TeklifEMailLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.FormatAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TeklifEMailLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.Tarih).HasColumnName("Tarih");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.FormatAdi).HasColumnName("FormatAdi");
            this.Property(t => t.Basarili).HasColumnName("Basarili");
        }
    }
}

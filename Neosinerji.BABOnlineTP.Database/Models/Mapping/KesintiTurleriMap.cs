using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KesintiTurleriMap : EntityTypeConfiguration<KesintiTurleri>
    {
        public KesintiTurleriMap()
        {
            // Primary Key
            this.HasKey(t => t.KesintiKodu);

            // Properties
            this.Property(t => t.KesintiAciklamasi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.YetkiliMail)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("KesintiTurleri");
            this.Property(t => t.KesintiKodu).HasColumnName("KesintiKodu");
            this.Property(t => t.KesintiAciklamasi).HasColumnName("KesintiAciklamasi");
            this.Property(t => t.OdemeGunu).HasColumnName("OdemeGunu");
            this.Property(t => t.YetkiliMail).HasColumnName("YetkiliMail");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
        }
    }
}

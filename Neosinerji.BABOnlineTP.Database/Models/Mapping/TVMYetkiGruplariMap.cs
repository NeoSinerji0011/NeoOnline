using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMYetkiGruplariMap : EntityTypeConfiguration<TVMYetkiGruplari>
    {
        public TVMYetkiGruplariMap()
        {
            // Primary Key
            this.HasKey(t => t.YetkiGrupKodu);

            // Properties
            this.Property(t => t.YetkiGrupAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TVMYetkiGruplari");
            this.Property(t => t.YetkiGrupKodu).HasColumnName("YetkiGrupKodu");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.YetkiGrupAdi).HasColumnName("YetkiGrupAdi");
            this.Property(t => t.YetkiSeviyesi).HasColumnName("YetkiSeviyesi");

            // Relationships
            this.HasRequired(t => t.TVMDetay)
                .WithMany(t => t.TVMYetkiGruplaris)
                .HasForeignKey(d => d.TVMKodu);

        }
    }
}

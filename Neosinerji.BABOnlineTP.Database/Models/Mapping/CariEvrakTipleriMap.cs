using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CariEvrakTipleriMap : EntityTypeConfiguration<CariEvrakTipleri>
    {
        public CariEvrakTipleriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Kodu });

            // Properties
            this.Property(t => t.Kodu)
               .IsRequired();

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Durum)
              .IsRequired();

            // Table & Column Mappings
            this.ToTable("CariEvrakTipleri");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}

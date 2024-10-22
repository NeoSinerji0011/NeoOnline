using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTipleriMap : EntityTypeConfiguration<IsTipleri>
    {
        public IsTipleriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TipKodu });

            // Properties
            this.Property(t => t.TipKodu)
               .IsRequired();

            this.Property(t => t.TipAciklama)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Durum)
              .IsRequired();

            // Table & Column Mappings
            this.ToTable("IsTipleri");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.TipAciklama).HasColumnName("TipAciklama");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
